using System;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Core.SignalR;
using Worldex.Core.ViewModels.Chat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using BackofficeWorldex.Web.Helper;
using Worldex.Core.Helpers;

namespace BackofficeWorldex.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : Controller
    {
        //private SocketHub _chat;
        private readonly ILogger<ChatController> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private ChatHub _chat;
        private RedisConnectionFactory _fact;
        private readonly PresenceTracker presenceTracker;
        private readonly IConfiguration Configuration;

        public ChatController(ChatHub Chat,PresenceTracker presenceTracker,RedisConnectionFactory Factory, UserManager<ApplicationUser>  UserManager , ILogger<ChatController> logger, IMediator mediator, IConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = UserManager;
            _fact = Factory;
            this.presenceTracker = presenceTracker;
            _chat = Chat;
            Configuration = configuration;
        }

        //Rushabh 22-08-2019 Commented BlockUnblockUser Method As Per Instruction By Khushali
        [HttpPost("BlockUser")]
        [Authorize]
        public async Task<IActionResult> BlockUnblockUser(UserViewModel Request)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    if (Request.IsBlocked)
                    {
                        ApplicationUser UserInfo = _userManager.FindByNameAsync(Request.Username).GetAwaiter().GetResult();
                        var Redis = new RadisServices<USerDetail>(this._fact);
                        USerDetail User = new USerDetail()
                        {
                            //UserID = UserID,
                            UserName = Request.Username,
                            Reason = Request.Reason
                        };
                        UserInfo.IsBlocked = true;
                        //user.IsEnabled = true;

                        var userUpdate = await _userManager.UpdateAsync(UserInfo);
                        if (userUpdate.Succeeded)
                        {
                            Redis.SaveToSortedSetByID(Configuration.GetValue<string>("SignalRKey:RedisBlockUserList"), User, UserInfo.Id);
                            BlockedUserViewModel blockedUserView = new BlockedUserViewModel()
                            {
                                IsBlocked = true
                            };

                            BlockedUserUpdate blockedUserUpdate = new BlockedUserUpdate()
                            {
                                Message = $"{ Request.Username } is blocked"
                            };

                            SignalRComm<BlockedUserViewModel> CommonData = new SignalRComm<BlockedUserViewModel>();
                            CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Nofification);
                            CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.MarkBlockUnblockUser);
                            CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBlockUnblockUser);
                            CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                            CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.UserInfo);
                            CommonData.Data = blockedUserView;
                            CommonData.Parameter = UserInfo.Id.ToString();

                            SignalRComm<BlockedUserUpdate> CommonData1 = new SignalRComm<BlockedUserUpdate>();
                            CommonData1.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Nofification);
                            CommonData1.Method = Enum.GetName(typeof(enMethodName), enMethodName.BlockUserUpdate);
                            CommonData1.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBlockUserUpdate);
                            CommonData1.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                            CommonData1.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.Base);
                            CommonData1.Data = blockedUserUpdate;
                            CommonData1.Parameter = UserInfo.Id.ToString();

                            await _chat.MarkUserBlock(UserInfo.Id, JsonConvert.SerializeObject(CommonData));
                            await _chat.BlockedUserUpdate(JsonConvert.SerializeObject(CommonData1));
                            Response.ReturnCode = enResponseCode.Success;
                            Response.ReturnMsg = EnResponseMessage.BlockUserSuccess;
                            Response.ErrorCode = enErrorCode.BlockUser;
                        }
                        else
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ReturnMsg = EnResponseMessage.BlockUserFail;
                            Response.ErrorCode = enErrorCode.BlockUserFail;
                        }
                    }
                    else
                    {
                        var Redis = new RadisServices<USerDetail>(this._fact);
                        ApplicationUser UserInfo = _userManager.FindByNameAsync(Request.Username).GetAwaiter().GetResult();
                        USerDetail User = new USerDetail()
                        {
                            //UserID = UserID,
                            UserName = Request.Username,
                            Reason = Request.Reason
                        };

                        UserInfo.IsBlocked = false;
                        //user.IsEnabled = false;

                        var userUpdate = await _userManager.UpdateAsync(UserInfo);
                        if (userUpdate.Succeeded)
                        {
                            //Redis.RemoveSortedSetByID(Configuration.GetValue<string>("SignalRKey:RedisBlockUserList"), User);
                            Redis.RemoveSortedSetByIDV1(Configuration.GetValue<string>("SignalRKey:RedisBlockUserList"), UserInfo.Id);
                            BlockedUserViewModel blockedUserView = new BlockedUserViewModel()
                            {
                                IsBlocked = false
                            };

                            SignalRComm<BlockedUserViewModel> CommonData = new SignalRComm<BlockedUserViewModel>();
                            CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Nofification);
                            CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.MarkBlockUnblockUser);
                            CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBlockUnblockUser);
                            CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                            CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.UserInfo);
                            CommonData.Data = blockedUserView;
                            CommonData.Parameter = UserInfo.Id.ToString();
                            await _chat.MarkUserBlock(UserInfo.Id, JsonConvert.SerializeObject(CommonData));
                            Response.ReturnCode = enResponseCode.Success;
                            Response.ReturnMsg = EnResponseMessage.UnblockUserSuccess;
                            Response.ErrorCode = enErrorCode.UnblockUser;
                        }
                        else
                        {
                            Response.ReturnCode = enResponseCode.Success;
                            Response.ReturnMsg = EnResponseMessage.UnblockUserFail;
                            Response.ErrorCode = enErrorCode.UnblockUser;
                        }
                    }
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //[HttpPost("UnblockUser")]
        //[Authorize]
        //public async Task<IActionResult> UnblockUser(UserViewModel Request)
        //{
        //    ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var Redis = new RadisServices<USerDetail>(this._fact);
        //            ApplicationUser UserInfo = _userManager.FindByNameAsync(Request.Username).GetAwaiter().GetResult();
        //            USerDetail User = new USerDetail()
        //            {
        //                //UserID = UserID,
        //                UserName = Request.Username
        //            };

        //            user.IsBlocked = false;
        //            //user.IsEnabled = false;

        //            var userUpdate = await _userManager.UpdateAsync(user);
        //            if (userUpdate.Succeeded)
        //            {
        //                Redis.RemoveSortedSetByID(Configuration.GetValue<string>("SignalRKey:RedisBlockUserList"), User);
        //                BlockedUserViewModel blockedUserView = new BlockedUserViewModel()
        //                {
        //                    IsBlocked = false
        //                };

        //                SignalRComm<BlockedUserViewModel> CommonData = new SignalRComm<BlockedUserViewModel>();
        //                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Nofification);
        //                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.MarkBlockUnblockUser);
        //                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBlockUnblockUser);
        //                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
        //                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.UserInfo);
        //                CommonData.Data = blockedUserView;
        //                CommonData.Parameter = UserInfo.Id.ToString();
        //                await _chat.MarkUserBlock(UserInfo.Id, JsonConvert.SerializeObject(CommonData));
        //                Response.ReturnCode = enResponseCode.Success;
        //                Response.ReturnMsg = EnResponseMessage.UnblockUserSuccess;
        //                Response.ErrorCode = enErrorCode.UnblockUser;
        //            }
        //            else
        //            {
        //                Response.ReturnCode = enResponseCode.Success;
        //                Response.ReturnMsg = EnResponseMessage.UnblockUserFail;
        //                Response.ErrorCode = enErrorCode.UnblockUser;
        //            }
        //        }
        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        [HttpGet("GetUserList")]
        [Authorize]
        public async Task<IActionResult> GetUserList()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ChatUserResponseView Response = new ChatUserResponseView();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response.Users = _userManager.Users.ToList().AsReadOnly();
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetOnlineUserCount")]
        [Authorize]
        public async Task<IActionResult> GetOnlineUserCount()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            CouterResponseView Response = new CouterResponseView();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var Redis = new RadisServices<USerDetail>(this._fact);
                    Response.Count = Redis.SortedSetLength(Configuration.GetValue<string>("SignalRKey:RedisOnlineUserList"));
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetActiveUserCount")]
        [Authorize]
        public async Task<IActionResult> GetActiveUserCount()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            CouterResponseView Response = new CouterResponseView();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var Redis = new RadisServices<USerDetail>(this._fact);
                    Response.Count = Redis.SortedSetCombineAndStore(Configuration.GetValue<string>("SignalRKey:RedisOnlineUserList"), Configuration.GetValue<string>("SignalRKey:RedisBlockUserList"));
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetOfflineUserCount")]
        [Authorize]
        public async Task<IActionResult> GetOfflineUserCount()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            CouterResponseView Response = new CouterResponseView();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var Redis = new RadisServices<USerDetail>(this._fact);
                    long OnlineUserCount = Redis.SortedSetLength(Configuration.GetValue<string>("SignalRKey:RedisOnlineUserList"));
                    long Total = _userManager.Users.Count();
                    Response.Count = Total - OnlineUserCount;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetBlockedUserCount")]
        [Authorize]
        public async Task<IActionResult> GetBlockedUserCount()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            CouterResponseView Response = new CouterResponseView();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var Redis = new RadisServices<USerDetail>(this._fact);
                    Response.Count = Redis.SortedSetLength(Configuration.GetValue<string>("SignalRKey:RedisBlockUserList"));
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetUserWiseChat")]
        [Authorize]
        public async Task<IActionResult> GetUserWiseChat(UserChatViewModel Request)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ChatHistoryViewModel Response = new ChatHistoryViewModel();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var Redis = new RadisServices<ChatHistory>(this._fact);
                    var ChatData = Redis.GetSortedSetDataByusername(Configuration.GetValue<string>("SignalRKey:RedisChatHistory"), Request);
                    if(ChatData.Data == null)
                    {
                        Response = ChatData;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ReturnMsg = "No Data Found";
                        Response.ErrorCode = enErrorCode.NoDataFound;
                    }
                    else
                    {
                        Response = ChatData;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ReturnMsg = EnResponseMessage.FindRecored;
                        Response.ErrorCode = enErrorCode.Success;
                    }
                    
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #region Listing Methods

        [HttpGet("GetOnlineUserList/{PageNo}/{PageSize}")]
        [Authorize]
        public async Task<IActionResult> GetOnlineUserList(int PageNo, int PageSize)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ChatUserResponseView Response = new ChatUserResponseView();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    List<long> UserIDList = new List<long>();
                    var Redis = new RadisServices<USerDetail>(this._fact);
                    var OnlineUsers = Redis.SortedSetScan(Configuration.GetValue<string>("SignalRKey:RedisOnlineUserList"));
                    if (OnlineUsers != null)
                    {
                        foreach (var data in OnlineUsers)
                        {
                            if (data != null)
                            {
                                UserIDList.Add(Convert.ToInt64(data.Score));
                            }
                        }
                    }
                    Worldex.Core.Helpers.HelperForLog.WriteLogIntoFile("OnlineUserCount", "Test", Helpers.JsonSerialize(UserIDList));
                    Response.Users = _userManager.Users.Where(p => UserIDList.Any(x => x == p.Id)).ToList();
                    Response.TotalCount = Response.Users.Count();
                    Response.PageSize = Convert.ToInt64(PageSize);
                    Response.PageNo = PageNo;
                    PageNo = PageNo + 1;
                    if (Response.Users.Count > 0)
                    {
                        if (PageNo > 0)
                        {
                            int skip = PageSize * (PageNo - 1);
                            Response.Users = Response.Users.Skip(skip).Take(PageSize).ToList();
                        }
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ReturnMsg = EnResponseMessage.FindRecored;
                        Response.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.NoDataFound;
                        Response.ReturnMsg = EnResponseMessage.NotFound;
                    }
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetActiveUserList/{PageNo}/{PageSize}")]
        [Authorize]
        public async Task<IActionResult> GetActiveUserList(int PageNo, int PageSize)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ChatUserResponseView Response = new ChatUserResponseView();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response.Users = _userManager.Users.Where(e => e.IsEnabled == true).ToList().AsReadOnly();
                    Response.TotalCount = Response.Users.Count();
                    Response.PageSize = Convert.ToInt64(PageSize);
                    Response.PageNo = PageNo;
                    PageNo = PageNo + 1;
                    if (Response.Users.Count > 0)
                    {
                        if (PageNo > 0)
                        {
                            int skip = PageSize * (PageNo - 1);
                            Response.Users = Response.Users.Skip(skip).Take(PageSize).ToList();
                        }
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ReturnMsg = EnResponseMessage.FindRecored;
                        Response.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.NoDataFound;
                        Response.ReturnMsg = EnResponseMessage.NotFound;
                    }
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetOfflineUserList/{PageNo}/{PageSize}")]
        [Authorize]
        public async Task<IActionResult> GetOfflineUserList(int PageNo, int PageSize)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ChatUserResponseView Response = new ChatUserResponseView();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    //string[] str;
                    List<long> UserIDList = new List<long>();
                    var Redis = new RadisServices<USerDetail>(this._fact);
                    var OnlineUsers = Redis.SortedSetScan(Configuration.GetValue<string>("SignalRKey:RedisOnlineUserList"));
                    //str = new string[OnlineUsers.Count];
                    //int i = 0;
                    if (OnlineUsers != null)
                    {
                        foreach (var data in OnlineUsers)
                        {
                            if (data != null)
                            {
                                //str[i] = data.Score.ToString();
                                UserIDList.Add(Convert.ToInt64(data.Score));
                            }
                            //i++;
                        }
                    }
                    //string UserId = string.Join(",", str);
                    Response.Users = _userManager.Users.Where(p => !UserIDList.Any(x => x == p.Id)).ToList();
                    Response.TotalCount = Response.Users.Count();
                    Response.PageSize = Convert.ToInt64(PageSize);
                    Response.PageNo = PageNo;
                    PageNo = PageNo + 1;
                    if (Response.Users.Count > 0)
                    {
                        if (PageNo > 0)
                        {
                            int skip = PageSize * (PageNo - 1);
                            Response.Users = Response.Users.Skip(skip).Take(PageSize).ToList();
                        }

                        Response.ReturnCode = enResponseCode.Success;
                        Response.ReturnMsg = EnResponseMessage.FindRecored;
                        Response.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.NoDataFound;
                        Response.ReturnMsg = EnResponseMessage.NotFound;
                    }
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetBlockedUserList/{PageNo}/{PageSize}")]
        [Authorize]
        public async Task<IActionResult> GetBlockedUserList(int PageNo, int PageSize)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ChatUserResponseView Response = new ChatUserResponseView();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response.Users = _userManager.Users.Where(e => e.IsBlocked == true).ToList().AsReadOnly();
                    Response.TotalCount = Response.Users.Count();
                    Response.PageSize = Convert.ToInt64(PageSize);
                    Response.PageNo = PageNo;
                    PageNo = PageNo + 1;
                    if (Response.Users.Count > 0)
                    {
                        if (PageNo > 0)
                        {
                            int skip = PageSize * (PageNo - 1);
                            Response.Users = Response.Users.Skip(skip).Take(PageSize).ToList();
                        }
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ReturnMsg = EnResponseMessage.FindRecored;
                        Response.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.NoDataFound;
                        Response.ReturnMsg = EnResponseMessage.NotFound;
                    }
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion
    }
}
