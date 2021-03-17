using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.BackOffice;
using Worldex.Core.Interfaces.EmailMaster;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.Interfaces.PasswordPolicy;
using Worldex.Core.Interfaces.PhoneMaster;
using Worldex.Core.Services;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.AccountViewModels.Login;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using Worldex.Core.ViewModels.BackOffice;
using Worldex.Core.ViewModels.BackOffice.PasswordPolicy;
using Worldex.Core.ViewModels.EmailMaster;
using Worldex.Core.ViewModels.GroupManagement;
using Worldex.Core.ViewModels.MobileMaster;
using Worldex.Core.ViewModels.Profile_Management;
using Worldex.Core.ViewModels.RoleConfig;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace BackofficeWorldex.Web.API.BackOffice
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    //[Authorize("MustHaveAuthority")] // Commented this as currently Method n Controller list not added. -Nishit Jani on A 2019-04-02 12:49 PM
    public class BackofficeRoleManagementController : BaseController
    {
        #region ctor

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRoleManagementServices _roleManagementServices;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IBasePage _basePage;
        private readonly IRuleManageService _ruleManageService;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly ISignupLogService _IsignupLogService;
        private readonly IMessageService _messageService;
        private readonly IEmailMaster _IemailMaster;
        private readonly Iphonemaster _Iphonemaster;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly IUserPasswordPolicyMaster _userPasswordPolicyMaster; //  khushali 05-03-2019 for Add Reset password  module
        private readonly IUserLinkMaster _userLinkMaster;  //  khushali 05-03-2019 for Add Reset password module
        private readonly IGroupManagementServices _GroupManagementServices;

        public BackofficeRoleManagementController(UserManager<ApplicationUser> userManager, EncyptedDecrypted encdecAEC,
            ISignupLogService IsignupLogService, IRoleManagementServices roleManagementServices, IEmailMaster IemailMaster, Iphonemaster Iphonemaster,
            Microsoft.Extensions.Configuration.IConfiguration configuration, IMessageService MessageService,
            IBasePage basePage, IRuleManageService RuleManageService, RoleManager<ApplicationRole> RoleManager,
            IUserPasswordPolicyMaster UserPasswordPolicyMaster, IUserLinkMaster UserLinkMaster, IPushNotificationsQueue<SendEmailRequest> PushNotificationsQueue
            , IGroupManagementServices GroupManagementServices)
        {
            _userManager = userManager;
            _roleManagementServices = roleManagementServices;
            _configuration = configuration;
            _basePage = basePage;
            _ruleManageService = RuleManageService;
            _roleManager = RoleManager;
            _encdecAEC = encdecAEC;
            _IsignupLogService = IsignupLogService;
            _messageService = MessageService;
            _IemailMaster = IemailMaster;
            _Iphonemaster = Iphonemaster;
            _userPasswordPolicyMaster = UserPasswordPolicyMaster;
            _userLinkMaster = UserLinkMaster;
            _pushNotificationsQueue = PushNotificationsQueue;
            _GroupManagementServices = GroupManagementServices;
        }

        #endregion

        #region Role Management

        [HttpPost]
        public async Task<IActionResult> CreateUserRole([FromBody]CreateRoleReq Req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                // Removed check whether user exist or not as If userID will not get in token then it will throw exception bcz we used getValue method for get UserID of Token. -Nishit Jani on A 2019-04-01 1:49 PM
                Response = await _roleManagementServices.CreateRole(Req, Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole([FromBody]CreateRoleReq Req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (Req.RoleId == null || Req.RoleId <= 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.RoleIdRequired;
                    Response.ErrorCode = enErrorCode.RoleIdRequiredAndCanNotBeZero;
                    return Ok(Response);
                }
                Response = await _roleManagementServices.UpdateRoleDetail(Req, Convert.ToInt64(Req.RoleId), Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRoleStatus([FromBody]ChangeRoleStatusReq Request)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _roleManagementServices.ChangeUserRoleStatus(Request.RoleId, Request.Status, Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageNo}")]
        public async Task<IActionResult> ListRoleDetails(int PageNo, int? PageSize, short? Status, short? AllRecords)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            ListRoleDetail Response = new ListRoleDetail();
            try
            {
                int PageSizeDef = Helpers.PageSize;
                if (PageSize != null)
                {
                    PageSizeDef = Convert.ToInt32(PageSize);
                }

                Response = await _roleManagementServices.ListRoleDetail(PageNo + 1, PageSizeDef, Status, AllRecords);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{RoleId}")]
        public async Task<IActionResult> GetRoleByID(long RoleId)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            GetRoleDetail22 Response = new GetRoleDetail22();
            try
            {
                Response = await _roleManagementServices.GetRoleDetail(RoleId);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{RoleId}/{UserId}")]
        public async Task<IActionResult> AssignRole(long RoleId, long UserId)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _roleManagementServices.AssignRoleDetail(RoleId, UserId, Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{RoleId}/{UserId}")]
        public async Task<IActionResult> RemoveAndAssignRole(long RoleId, long UserId)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _roleManagementServices.AssignRoleDetail(RoleId, UserId, Convert.ToInt64(userID), true);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageNo}")]
        public async Task<IActionResult> GetRoleHistory(int PageNo, int? PageSize, long? UserId, DateTime? FromDate, DateTime? ToDate, long? ModuleId, short? Status)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            ListRoleHistoryData Response = new ListRoleHistoryData();
            try
            {
                Response = await _roleManagementServices.GetRoleHistory(PageNo + 1, Convert.ToInt32(PageSize == null ? Helpers.PageSize : PageSize), UserId, FromDate, ToDate, ModuleId, Status);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Permission Group Methods

        [HttpPost]
        public async Task<IActionResult> CreatePermissionGroup([FromBody]CreatePermissionGrpReq Req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _roleManagementServices.CreatePermissionGroup(Req, Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePermissionGroup([FromBody]CreatePermissionGrpReq req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (req.PermissionGroupID == null || req.PermissionGroupID <= 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.PermissionGrpIdRequired;
                    Response.ErrorCode = enErrorCode.PermissionGrpIdRequiredAndCanNotBeZero;
                    return Ok(Response);
                }
                Response = await _roleManagementServices.UpdatePermissionGroup(Convert.ToInt64(req.PermissionGroupID), req, Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePermissionGroupStatus([FromBody]ChangePermissionGroupStatusReq req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _roleManagementServices.ChangePermissionGroupStatus(req.PermissionGroupID, req.Status, Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PermissionGroupId}")]
        public async Task<IActionResult> GetPermissionGroupDetailById(long PermissionGroupId)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            GetPermissionGroupRes Response = new GetPermissionGroupRes();
            try
            {
                Response = await _roleManagementServices.GetPermissionGroupDetailById(PermissionGroupId);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageNo}")]
        //[AllowAnonymous]
        public async Task<IActionResult> ListPermissionGroup(int PageNo, int? PageSize, short? AllRecords, DateTime? FromDate, DateTime? Todate, long? RoleId, short? Status)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            ListPermissionGrp Response = new ListPermissionGrp();
            try
            {
                int PageSizeDef = Helpers.PageSize;
                if (PageSize != null)
                {
                    PageSizeDef = Convert.ToInt32(PageSize);
                }

                Response = await _roleManagementServices.ListPermissionGroup(PageNo + 1, PageSizeDef, AllRecords, FromDate, Todate, RoleId, Status);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region New Permission Group Methods
        [HttpPost]
        public async Task<IActionResult> CreatePermissionGroupV1([FromBody]CreateGroupRequest Req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _roleManagementServices.CreatePermissionGroupV1(Req, Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePermissionGroupV1([FromBody]CreateGroupRequest req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (string.IsNullOrEmpty(req.GroupName) || string.IsNullOrWhiteSpace(req.GroupName))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.PermissionGrpIdRequired;
                    Response.ErrorCode = enErrorCode.PermissionGrpIdRequiredAndCanNotBeZero;
                    return Ok(Response);
                }
                Response = await _roleManagementServices.UpdatePermissionGroupV1(req, Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region User Management Methods

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserReq Req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _roleManagementServices.CreateNewUser(Req, Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUser([FromBody]EditUserReq Req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _roleManagementServices.EditUserDetail(Req, Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{UserId}/{Status}")]
        public async Task<IActionResult> ChangeUserStatus(long UserId, EnMyAccountUserStatus Status)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _roleManagementServices.ChangeUserStatus(UserId, Status);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageNo}")]
        public async Task<IActionResult> ListUserDetail(int PageNo, int? PageSize, short? AllRecords, string Username = null, string Mobile = null, string Email = null, DateTime? FromDate = null, DateTime? ToDate = null, int IsAdmin = 9)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            ListUserDetails Response = new ListUserDetails();
            try
            {
                int PageSizeDef = Helpers.PageSize;
                if (PageSize != null)
                {
                    PageSizeDef = Convert.ToInt32(PageSize);
                }
                //Response = await _roleManagementServices.ListUserDetail(PageNo + 1, PageSizeDef, AllRecords);
                Response = await _roleManagementServices.ListUserDetail(PageNo + 1, PageSizeDef, AllRecords, Username, Mobile, Email, FromDate, ToDate, IsAdmin);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetailById(long UserId)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            GetUserDetailResp Response = new GetUserDetailResp();
            try
            {

                Response = await _roleManagementServices.GetUserDetailById(UserId);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpGet("{SearchText}")]
        public async Task<IActionResult> SearchUser(string SearchText)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            ListUserDetails2 Response = new ListUserDetails2();
            try
            {

                Response = await _roleManagementServices.SearchUserDetail(SearchText);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        public static bool TryGetFromBase64String(string input, out byte[] output)
        {
            output = null;
            try
            {
                output = Convert.FromBase64String(input);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmInvitation(string emailConfirmCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailConfirmCode))   ////  Create the standard signup method data check is null or not
                {
                    //add validation LinkData valid or not -mansi 23-09-2019

                    var emailConfirmCode1 = emailConfirmCode.Trim();
                    Boolean ans = (emailConfirmCode1.Length % 4 == 0) && Regex.IsMatch(emailConfirmCode1, @"^[a-zA-Z0-9\+/]*={0,2}$", RegexOptions.None);
                    if (ans == false)
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Invalidlinkdata, ErrorCode = enErrorCode.Status1450invalidelinkdata });
                    byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                    byte[] bytes = null;
                    string DecryptToken;
                    try
                    {
                        bytes = Convert.FromBase64String(emailConfirmCode);
                        var encodedString = Encoding.UTF8.GetString(bytes);
                         DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);
                    }
                    catch (FormatException ex)
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "Invalid Invite Link", ErrorCode = enErrorCode.Status14058InvalidemailConfirmCode });
                    }
                    //var bytes = Convert.FromBase64String(emailConfirmCode);
                    //var encodedString = Encoding.UTF8.GetString(bytes);
                    //string DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);


                    EmailLinkTokenViewModel dmodel = JsonConvert.DeserializeObject<EmailLinkTokenViewModel>(DecryptToken);

                    if (dmodel?.Expirytime >= DateTime.UtcNow)   /// Check the link expiration time 
                    {
                        if (dmodel.Id == 0)
                        {
                            return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignEmailUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                        }
                        else
                        {
                            var user = await _userManager.FindByEmailAsync(dmodel.Email);
                            if (user == null)
                            {
                                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignEmailUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                            }
                            else if (user != null)
                            {
                                if (user.EmailConfirmed == true)     ///  Check the email allready conform or not
                                {
                                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPAlreadyConfirm, ErrorCode = enErrorCode.Status4167SignUPAlreadyConfirm });
                                }
                                user.EmailConfirmed = true;
                                user.IsEnabled = true;
                                user.Status = 1;
                                var result = await _userManager.UpdateAsync(user);
                                if (result.Succeeded)
                                {
                                    if (user.Mobile != null)
                                    {
                                        var officeClaim = new Claim(OpenIdConnectConstants.Claims.PhoneNumber, user.Mobile.ToString(), ClaimValueTypes.Integer);
                                        await _userManager.AddClaimAsync(user, officeClaim);
                                    }
                                    var roleAddResult1 = _userManager.RemoveFromRoleAsync(user, "User").Result;

                                    var roleAddResult = await _userManager.AddToRoleAsync(user, "User");
                                    if (roleAddResult.Succeeded)
                                    {
                                        user.EmailConfirmed = true;   /// set the bizuser table bit for conform set 
                                        var resultupdate = await _userManager.UpdateAsync(user);


                                        HttpContext.Items["UserId"] = user.Id;
                                        //// added by nirav savariya for create profile subscription plan on 11-04-2018
                                        SubscriptionViewModel subscriptionmodel = new SubscriptionViewModel()
                                        {
                                            UserId = user.Id
                                        };

                                        //// added by nirav savariya for verify user status on 12-05-2015
                                        _IsignupLogService.UpdateVerifiedUser(Convert.ToInt32(user.Id), user.Id);

                                        ///// added by nirav savariya for devicewhitelisting on 12-06-2018
                                        //DeviceMasterViewModel model = new DeviceMasterViewModel();
                                        //model.Device = dmodel.Device;
                                        //model.DeviceOS = dmodel.DeviceOS;
                                        //model.DeviceId = dmodel.DeviceID;
                                        //model.UserId = Convert.ToInt32(user.Id);
                                        //_IdeviceIdService.AddDeviceProcess(model);

                                        ///// added by nirav savariya for ipwhitelist on 12-07-2018
                                        //IpMasterViewModel ipmodel = new IpMasterViewModel();
                                        //ipmodel.IpAddress = dmodel.IpAddress;
                                        //ipmodel.UserId = Convert.ToInt32(user.Id);
                                        //_iipAddressService.AddIpAddress(ipmodel);


                                        TemplateMasterData TemplateData = new TemplateMasterData();
                                        CommunicationParamater communicationParamater = new CommunicationParamater();
                                        SendEmailRequest request = new SendEmailRequest();
                                        communicationParamater.Param1 = user.UserName; //Username
                                        communicationParamater.Param2 = user.FirstName; //FirstName
                                        communicationParamater.Param3 = user.LastName; //LastName
                                        communicationParamater.Param4 = user.Email; //Email
                                        communicationParamater.Param5 = user.Mobile; //Mobile
                                        TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.Registration, communicationParamater, enCommunicationServiceType.Email).Result;
                                        if (TemplateData != null)
                                        {
                                            if (TemplateData.IsOnOff == 1)
                                            {
                                                request.Recepient = user.Email;
                                                request.Body = TemplateData.Content;
                                                request.Subject = TemplateData.AdditionalInfo;
                                                _pushNotificationsQueue.Enqueue(request);
                                            }
                                        }


                                        ///User Primary Email Define

                                        EmailMasterReqViewModel emailMasterReqViewModel = new EmailMasterReqViewModel();
                                        {
                                            emailMasterReqViewModel.Email = user.Email;
                                            emailMasterReqViewModel.IsPrimary = true;
                                            emailMasterReqViewModel.Userid = user.Id;
                                        }
                                        Guid emailid = _IemailMaster.Add(emailMasterReqViewModel);
                                        ///  Create the Primary Phone Number Define

                                        if (!string.IsNullOrEmpty(user.Mobile))
                                        {
                                            PhoneMasterReqViewModel phoneMasterReqViewModel = new PhoneMasterReqViewModel();
                                            phoneMasterReqViewModel.IsPrimary = true;
                                            phoneMasterReqViewModel.MobileNumber = user.Mobile;
                                            phoneMasterReqViewModel.Userid = user.Id;
                                            Guid phoneID = _Iphonemaster.Add(phoneMasterReqViewModel);
                                        }

                                        // Commented by khushali 05-03-2019 for unused Template Method call 
                                        //if (!string.IsNullOrEmpty(dmodel.Password))  // This condition only use for back office standard register time call.
                                        //{

                                        //    TemplateMasterData TemplateData1 = new TemplateMasterData();
                                        //    CommunicationParamater communicationParamater1 = new CommunicationParamater();
                                        //    SendEmailRequest request1 = new SendEmailRequest();
                                        //    communicationParamater1.Param1 = dmodel.Email;
                                        //    communicationParamater1.Param2 = dmodel.Password;
                                        //    TemplateData1 = _messageService.ReplaceTemplateMasterData(EnTemplateType.LoginPassword, communicationParamater1, enCommunicationServiceType.Email).Result;
                                        //    if (TemplateData1 != null)
                                        //    {
                                        //        if (TemplateData1.IsOnOff == 1)
                                        //        {
                                        //            request1.Recepient = dmodel.Email;
                                        //            request1.Body = TemplateData1.Content;
                                        //            request1.Subject = TemplateData1.AdditionalInfo;
                                        //            _pushNotificationsQueue.Enqueue(request1);
                                        //        }
                                        //    }
                                        //}
                                        var UserPasswordForgotInMonth = _userPasswordPolicyMaster.GetUserPasswordPolicyConfiguration(user.Id);
                                        var LinkExpiryTime = 2;
                                        if (UserPasswordForgotInMonth != null)
                                        {
                                            LinkExpiryTime = UserPasswordForgotInMonth.LinkExpiryTime;
                                        }
                                        UserLinkMasterViewModel userLinkMaster = new UserLinkMasterViewModel()
                                        {
                                            LinkvalidTime = LinkExpiryTime,
                                            UserLinkData = user.Email,
                                            UserId = user.Id
                                        };
                                        Guid forgotPasswordID = _userLinkMaster.Add(userLinkMaster);
                                        ForgotPasswordDataViewModel forgotPassword = new ForgotPasswordDataViewModel()
                                        {
                                            Id = forgotPasswordID,
                                            LinkvalidTime = LinkExpiryTime
                                        };
                                        byte[] Userdata = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                                        string UserForgotPassword = JsonConvert.SerializeObject(forgotPassword);
                                        string ForgotPasswordKey = EncyptedDecrypted.Encrypt(UserForgotPassword, Userdata);
                                        byte[] ForgotWordplainTextBytes = Encoding.UTF8.GetBytes(ForgotPasswordKey);

                                        string Forgotctokenlink = _configuration["Forgotverifylink"].ToString() + Convert.ToBase64String(ForgotWordplainTextBytes);

                                        //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                                        // khushali 05-03-2019 for Common Template Method call 
                                        TemplateMasterData TemplateData2 = new TemplateMasterData();
                                        CommunicationParamater communicationParamater2 = new CommunicationParamater();
                                        SendEmailRequest request2 = new SendEmailRequest();
                                        communicationParamater2.Param1 = user.FirstName;
                                        communicationParamater2.Param2 = Forgotctokenlink;
                                        TemplateData2 = _messageService.ReplaceTemplateMasterData(EnTemplateType.ForgotPassword, communicationParamater2, enCommunicationServiceType.Email).Result;
                                        if (TemplateData2 != null)
                                        {
                                            if (TemplateData2.IsOnOff == 1)
                                            {
                                                request2.Recepient = user.Email;
                                                request2.Body = TemplateData2.Content;
                                                request2.Subject = TemplateData2.AdditionalInfo;
                                                request2.EmailType = 1;
                                                _pushNotificationsQueue.Enqueue(request2);
                                            }
                                        }

                                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessInviteUser, ErrorCode = enErrorCode.SuccessInviteUser });
                                    }
                                    else if (roleAddResult.Errors != null && roleAddResult.Errors.Count() > 0)
                                    {
                                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = roleAddResult.Errors.First().Description, ErrorCode = enErrorCode.Status4063UserNotRegister });
                                    }
                                }
                                else
                                {
                                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserNotRegister, ErrorCode = enErrorCode.Status4063UserNotRegister });
                                }
                            }
                        }
                    }
                    else
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpEmailExpired, ErrorCode = enErrorCode.Status4039ResetPasswordLinkExpired });
                    }



                }
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignEmailLink, ErrorCode = enErrorCode.Status4064EmailLinkBlanck });

            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReInviteUser(ReInviteUserViewModel Request)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            try
            {
                var user = await _userManager.FindByEmailAsync(Request.Email);
                if (user == null)
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "User not found", ErrorCode = enErrorCode.Status4033NotFoundRecored });
                else
                {
                    byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                    if (passwordBytes.Length < 0)
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = "AESSalt Key Not Found", ErrorCode = enErrorCode.Status500InternalServerError });
                    //var Currentuser = await _userManager.FindByNameAsync(user.Username);
                    //if (Currentuser == null)
                    //return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = "User Not Found", ErrorCode = enErrorCode.Status500InternalServerError });
                    EmailLinkTokenViewModel linkToken = new EmailLinkTokenViewModel();
                    linkToken.Id = user.Id;
                    linkToken.Username = user.UserName;
                    linkToken.Email = user.Email;
                    linkToken.CurrentTime = Helpers.UTC_To_IST();
                    linkToken.Expirytime = Helpers.UTC_To_IST() + TimeSpan.FromHours(2);

                    string UserDetails = JsonConvert.SerializeObject(linkToken);
                    string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);

                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
                    string ctokenlink = _configuration["InviteMailURL"].ToString() + Convert.ToBase64String(plainTextBytes);

                    // khushali 30-01-2019 for Common Template Method call 
                    TemplateMasterData TemplateData = new TemplateMasterData();
                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    SendEmailRequest request = new SendEmailRequest();
                    if (!string.IsNullOrEmpty(user.UserName))
                        communicationParamater.Param1 = user.UserName;
                    else
                        communicationParamater.Param1 = string.Empty;
                    communicationParamater.Param2 = ctokenlink;
                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_SendInvitation, communicationParamater, enCommunicationServiceType.Email).Result;
                    if (TemplateData != null)
                    {
                        if (TemplateData.IsOnOff == 1)
                        {
                            request.Recepient = user.Email;
                            request.Body = TemplateData.Content;
                            request.Subject = TemplateData.AdditionalInfo;
                            _pushNotificationsQueue.Enqueue(request);
                        }
                    }
                    return Ok(new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessReInviteUser, ErrorCode = enErrorCode.SuccessReInviteUser });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Velocity Rule khushali - 21-02-2019

        [HttpGet]
        //[AllowAnonymous]
        public async Task<IActionResult> GetConfigurePermissions()
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            UserAccessRightsResponse Response = new UserAccessRightsResponse();
            try
            {
                Response.Result = await _ruleManageService.GetConfigurePermissions();
                if (Response.Result == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Ok(Response);
                }
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                return Ok(Response);

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{GroupID:long}")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetConfigurePermissionsByGroupID(long GroupID)
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 1;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            UserAccessRightsResponse Response = new UserAccessRightsResponse();
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response.Result = await _ruleManageService.GetAssignedRule(GroupID);
                    if (Response.Result == null)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                        Response.ErrorCode = enErrorCode.NoRecordFound;
                        return Ok(Response);
                    }
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                    Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                    return Ok(Response);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpGet("{RoleID:long}")]
        public async Task<IActionResult> ViewUsersByRole(long RoleID)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            try
            {
                if (user == null)
                {
                    var BizResponseClass = new BizResponseClass();
                    BizResponseClass.ReturnCode = enResponseCode.Fail;
                    BizResponseClass.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    BizResponseClass.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(BizResponseClass);
                }
                else
                {
                    var Role = await _roleManager.FindByIdAsync(RoleID.ToString());
                    ICollection<ApplicationUser> users = await _userManager.GetUsersInRoleAsync(Role.Name);
                    ViewUserDetailRes Resp;
                    #region Old Code
                    //var result = new
                    //{
                    //    //draw = draw,
                    //    recordsTotal = users.Count(),
                    //    //recordsFiltered = qry.Count(),
                    //    data = users.Select(u => new
                    //    {
                    //        Id = u.Id,
                    //        Email = u.Email,
                    //        UserName = u.UserName,
                    //        Role  = Role.Name,
                    //        CreatedDate = u.CreatedDate,
                    //        Status = u.Status
                    //    }).ToArray()
                    //};
                    #endregion
                    Resp = new ViewUserDetailRes()
                    {
                        TotalRecords = users.Count(),
                        Data = users.Select(u => new ViewUserRes
                        {
                            Id = u.Id,
                            Email = u.Email,
                            UserName = u.UserName,
                            RoleName = Role.Name,
                            CreatedDate = u.CreatedDate,
                            Status = u.Status
                        }).ToList()
                    };
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                    Resp.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                    return Ok(Resp);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageNo}")]
        public async Task<IActionResult> ViewUnassignedUsers(int PageNo, int? PageSize, string UserName, DateTime? FromDate, DateTime? ToDate, short? Status)
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 1;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListViewUnAssignedUserRes Response = new ListViewUnAssignedUserRes();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    Response.PageNo = PageNo;
                    Response = await _roleManagementServices.ViewUnassignedUsers(PageNo + 1, Convert.ToInt32(PageSize == null ? Helpers.PageSize : PageSize), UserName, FromDate, ToDate, Status);
                    if (Response.Result == null)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                        Response.ErrorCode = enErrorCode.NoRecordFound;
                        return Ok(Response);
                    }
                    Response.PageSize = Convert.ToInt32(PageSize == null ? Helpers.PageSize : PageSize);

                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                    Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                    return Ok(Response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Apply rules and access permission to logged in user  khushali - 04-03-2019

        [HttpGet]
        public async Task<IActionResult> GetAccessRightsByUser()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            UserAccessRightsResponse Response = new UserAccessRightsResponse();
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    IList<string> RoleList = await _userManager.GetRolesAsync(user);
                    if (RoleList != null && RoleList.Count > 0)
                    {
                        string Role = RoleList.First();
                        int RoleID = _roleManager.FindByNameAsync(Role).Result.Id;
                        var Result = await _roleManagementServices.GetPermissionGroupIDByLinkedRole(RoleID);
                        if (Result.ReturnCode == enResponseCode.Success)
                        {
                            Response.Result = await _ruleManageService.GetAssignedRuleV1(Result.GroupID);
                            if (Response.Result == null)
                            {
                                Response.ReturnCode = enResponseCode.Fail;
                                Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                                Response.ErrorCode = enErrorCode.NoRecordFound;
                                return Ok(Response);
                            }
                            Response.ReturnCode = enResponseCode.Success;
                            Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                            Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                            return Ok(Response);
                        }
                        else
                        {
                            Response.ReturnCode = Result.ReturnCode;
                            Response.ReturnMsg = Result.ReturnMsg;
                            Response.ErrorCode = Result.ErrorCode;
                        }
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.UserRoleNoDataFound;
                        Response.ErrorCode = enErrorCode.UserRoleNoDataFound;
                        return Ok(Response);
                    }
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpGet]
        //[AllowAnonymous]
        public async Task<IActionResult> GetMenuAccessRightsByUser()
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 57;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            UserAccessRightsResponse Response = new UserAccessRightsResponse();
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    IList<string> RoleList = await _userManager.GetRolesAsync(user);
                    if (RoleList != null && RoleList.Count > 0)
                    {
                        string Role = RoleList.Last();
                        int RoleID = _roleManager.FindByNameAsync(Role).Result.Id;
                        var Result = await _roleManagementServices.GetPermissionGroupIDByLinkedRole(RoleID);
                        if (Result.ReturnCode == enResponseCode.Success)
                        {
                            Response.Result = await _ruleManageService.GetMenuAssignedRule(Result.GroupID);
                            if (Response.Result == null)
                            {
                                Response.ReturnCode = enResponseCode.Fail;
                                Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                                Response.ErrorCode = enErrorCode.NoRecordFound;
                                return Ok(Response);
                            }
                            Response.ReturnCode = enResponseCode.Success;
                            Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                            Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                            return Ok(Response);
                        }
                        else
                        {
                            Response.ReturnCode = Result.ReturnCode;
                            Response.ReturnMsg = Result.ReturnMsg;
                            Response.ErrorCode = Result.ErrorCode;
                        }
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.UserRoleNoDataFound;
                        Response.ErrorCode = enErrorCode.UserRoleNoDataFound;
                        return Ok(Response);
                    }
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Revert Menu access list of User by their Gorup ID. -Nishit Jani on A 2019-03-28 11:37 AM

        [HttpGet]
        //[Authorize("MustHaveAuthority")]
        [Authorize]
        public async Task<IActionResult> GetAccessRightsByUserV1()
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            try
            {
                var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
                var user = await _userManager.FindByIdAsync(userID);
                var groupID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "GroupID").Value;
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    //Response = await _roleManagementServices.GetMenuWithDetails(Convert.ToInt64(groupID));
                    Response = await _roleManagementServices.GetMenuWithDetailsAsyncV2(Convert.ToInt64(groupID));
                    //Response = null;
                    if (Response.Result==null)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                        Response.ErrorCode = enErrorCode.NoRecordFound;
                        return Ok(Response);
                    }
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                    Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                    return Ok(Response);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        //[Authorize("MustHaveAuthority")]
        [Authorize]
        public async Task<IActionResult> GetAllMenu()
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            try
            {
                var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
                var user = await _userManager.FindByIdAsync(userID);                
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {                    
                    Response = await _roleManagementServices.GetDefaultMenusV2();
                    
                    if (Response.Result == null)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                        Response.ErrorCode = enErrorCode.NoRecordFound;
                        return Ok(Response);
                    }
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                    Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                    return Ok(Response);
                }               
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        //[Authorize("MustHaveAuthority")]
        [Authorize]
        public async Task<IActionResult> GetAccessRights([FromForm]Guid ParentID)//, long GroupID)
        //public async Task<IActionResult> GetAccessRights(Guid ParentID, long GroupID)
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            try
            {
                var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
                var user = await _userManager.FindByIdAsync(userID);
                var groupID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "GroupID").Value;
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {                    
                    Response = await _roleManagementServices.GetMenuWithDetailsAsyncV2MainMenu(Convert.ToInt64(groupID), ParentID);
                    //Response = null;
                    if (Response.Result == null)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                        Response.ErrorCode = enErrorCode.NoRecordFound;
                        return Ok(Response);
                    }
                    if (Response.Result.Modules.Count == 0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                        Response.ErrorCode = enErrorCode.NoRecordFound;
                        return Ok(Response);
                    }
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                    Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                    return Ok(Response);
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        //[Authorize("MustHaveAuthority")]
        [Authorize]
        public async Task<IActionResult> GetAccessRightsV2([FromForm]Guid ParentID)//, long GroupID)
        //public async Task<IActionResult> GetAccessRights(Guid ParentID, long GroupID)
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            try
            {
                var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
                var user = await _userManager.FindByIdAsync(userID);
                //var groupID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "GroupID").Value;
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    Response = await _roleManagementServices.GetGroupAccessRightsGroupWise(user.GroupID, ParentID.ToString(), true);
                    //Response = null;
                    if (Response.Result == null)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                        Response.ErrorCode = enErrorCode.NoRecordFound;
                        return Ok(Response);
                    }
                    if (Response.Result.Modules.Count == 0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                        Response.ErrorCode = enErrorCode.NoRecordFound;
                        return Ok(Response);
                    }
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                    Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                    return Ok(Response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        //[Authorize("MustHaveAuthority")]
        [Authorize]
        public async Task<IActionResult> GetMasterList([FromForm]Guid ParentID)
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            try
            {
                var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
                var user = await _userManager.FindByIdAsync(userID);                
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {                    
                    Response = await _roleManagementServices.GetMasterList(ParentID);
                    
                    if (Response.Result == null)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                        Response.ErrorCode = enErrorCode.NoRecordFound;
                        return Ok(Response);
                    }
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                    Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                    return Ok(Response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        //[Authorize("MustHaveAuthority")]
        [Authorize]
        public async Task<IActionResult> GetMasterListLight([FromForm]Guid ParentID)
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            try
            {
                var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
                var user = await _userManager.FindByIdAsync(userID);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    Response = await _roleManagementServices.GetGroupAccessRightsGroupWise(user.GroupID,ParentID.ToString(),true,true); /// khushali 21-05-2019 for group wise menu access 

                    if (Response.Result == null)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                        Response.ErrorCode = enErrorCode.NoRecordFound;
                        return Ok(Response);
                    }
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                    Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                    return Ok(Response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //Khushali 30-04-2019 Admin Get Rights for perticular group
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MenuAccessResponse>> ConfigureGroupAccessRights([FromForm]string ParentID, [FromForm]long GroupID)
        {

            MenuAccessResponse Response = new MenuAccessResponse();
            try
            {
                var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
                var user = await _userManager.FindByIdAsync(userID);
                //var groupID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "GroupID").Value;
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    //Response = await _roleManagementServices.GetMenuWithDetails(Convert.ToInt64(groupID));
                    Response = await _roleManagementServices.GetGroupAccessRightsGroupWise(Convert.ToInt64(GroupID), ParentID);
                    //Response = null;
                    if (Response.Result == null || Response.Result?.Modules.Count == 0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                        Response.ErrorCode = enErrorCode.NoRecordFound;
                        return Ok(Response);
                    }
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                    Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                    return Ok(Response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateModuleGroupAccess([FromBody]MenuSubDetailReq Req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _roleManagementServices.UpdateModuleGroupAccess(Req.Data, Req.GroupID, Convert.ToInt64(userID));
                if(Response == null)
                {
                    return Ok(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError });
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateModuleFieldpAccess([FromBody]ChildNodeFiledReq Req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _roleManagementServices.UpdateModuleFieldpAccess(Req.Data, Convert.ToInt64(userID));
                if (Response == null)
                {
                    return Ok(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError });
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Group Management -Nishit Jani on A 2019-04-25 2:28 PM
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody]CreateGroupRequest Req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {                
                Response = await _GroupManagementServices.CreateGroupAsync(Req, Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGroup([FromBody]ChangeGroupRequest Req)
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _GroupManagementServices.ChangeGroupAsync(Req, Convert.ToInt64(userID));

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupList()
        {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _GroupManagementServices.GetGroupList();

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion


        [HttpPost("UpdateFieldDataTest")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateFieldDataTest([FromBody]FieldDataViewModelv1 request)
        {
            BizResponseClass res = new BizResponseClass();
            try
            {

                res = await _roleManagementServices.UpdateFieldDataTest(request, 1);
                if (res == null)
                {
                    res.ReturnMsg = "Internal Error";
                    res.ReturnCode = enResponseCode.InternalError;
                    res.ErrorCode = enErrorCode.Status500InternalServerError;
                    return Ok(res);
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }

        }

        [HttpPost("UpdateSubmoduleDataTest")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateSubmoduleDataTest([FromBody]SubModuleDataViewModelv1 request)
        {
            BizResponseClass res = new BizResponseClass();
            try
            {

                res = await _roleManagementServices.UpdateSubmoduleDataTest(request, 1);
                if (res == null)
                {
                    res.ReturnMsg = "Internal Error";
                    res.ReturnCode = enResponseCode.InternalError;
                    res.ErrorCode = enErrorCode.Status500InternalServerError;
                    return Ok(res);
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }

        }

    }
}