using System;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.ApiModels.Chat;
using CleanArchitecture.Core.Entities.Communication;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Services.RadisDatabase;
using CleanArchitecture.Core.SignalR;
using CleanArchitecture.Core.ViewModels.Chat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : Controller
    {
        //private SocketHub _chat;
        private readonly ILogger<SocketController> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private ChatHub _chat;
        private RedisConnectionFactory _fact;
        private readonly PresenceTracker presenceTracker;
        private readonly IConfiguration Configuration;

        public ChatController(ChatHub Chat,PresenceTracker presenceTracker,RedisConnectionFactory Factory, UserManager<ApplicationUser>  UserManager , ILogger<SocketController> logger, IMediator mediator, IConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = UserManager;
            _fact = Factory;
            this.presenceTracker = presenceTracker;
            _chat = Chat;
            Configuration = configuration;
        }
       
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
    }
}
