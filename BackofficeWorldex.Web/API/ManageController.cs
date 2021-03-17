using System.Linq;
using System.Threading.Tasks;
//using AspNetCoreSpa.Core.ViewModels.ManageViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Encodings.Web;
using Worldex.Infrastructure;
using Worldex.Core.Entities;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.ManageViewModels;

using Worldex.Core.ViewModels.AccountViewModels;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using Worldex.Core.Entities.User;
using Microsoft.AspNetCore.Identity.UI.Services;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Core.Services.Session;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.Interfaces.User;
using Worldex.Core.Enums;
using Worldex.Infrastructure.Interfaces;
using Worldex.Core.ViewModels.AccountViewModels.Log;
using Worldex.Core.Interfaces.UserChangeLog;
using Worldex.Core.ViewModels.ManageViewModels.UserChangeLog;
using Newtonsoft.Json;
using Worldex.Core.ApiModels.Chat;
using TwoFactorAuthNet;
using Worldex.Core.ViewModels.ManageViewModels.TwoFA;
using Microsoft.AspNetCore.Authentication;
using Worldex.Core.ViewModels.Organization;
using Worldex.Core.Interfaces.Activity_Log;
using Worldex.Core.ViewModels.AccountViewModels.Login;
using Worldex.Core.Services;
using Worldex.Infrastructure.BGTask;
using Worldex.Core.ViewModels;
using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.AccountViewModels.OTP;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BackofficeWorldex.Web.Filters;
using BackofficeWorldex.Web.Extensions;

namespace BackofficeWorldex.Web.API
{
    [Route("api/[controller]")]
    [Authorize]
    public class ManageController : BaseController
    {
        #region Field 

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UrlEncoder _urlEncoder;
        private readonly RedisConnectionFactory _fact;
        private readonly RedisSessionStorage _redisSessionStorage;
        private readonly IUserService _userdata;
        private readonly IipAddressService _ipAddressService;
        private readonly IDeviceIdService _iDeviceIdService;
        private readonly IBasePage _basePage;
        private readonly IUserChangeLog _iuserChangeLog;
        private readonly IipHistory _iipHistory;
        private readonly ILoginHistory _loginHistory;
        private readonly ISignalRService _signalRService;
        private readonly ISignupLogService _signupLogService;
        private readonly IUserKeyMasterService _userKeyMasterService;
        private readonly IActivityRegisterData _IactivityRegisterService;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IMessageService _messageService;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private readonly IOtpMasterService _otpMasterService;
        private readonly IManageService _manageService;
        private readonly ICustomPassword _custompassword;
        #endregion

        #region Ctore
        public ManageController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILoggerFactory loggerFactory,
        UrlEncoder urlEncoder,
        //IRedisConnectionFactory factory,
        RedisConnectionFactory factory,
        RedisSessionStorage redisSessionStorage,
        IUserService userdata,
        IipAddressService ipAddressService,
         IBasePage basePage,
         IDeviceIdService iDeviceIdService, IUserChangeLog userChangeLog,
         IipHistory iipHistory,
         ILoginHistory loginHistory, ISignalRService signalRService,
         ISignupLogService signupLogService,
         IUserKeyMasterService userKeyMasterService,
         IActivityRegisterData IactivityRegisterService,
         EncyptedDecrypted encdecAEC,
         Microsoft.Extensions.Configuration.IConfiguration configuration,
         IMessageService MessageService,
         IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue,
         IPushNotificationsQueue<SendSMSRequest> pushSMSQueue,
         IOtpMasterService otpMasterService, IManageService manageService, ICustomPassword custompassword)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_emailSender = emailSender;
            //  _logger = loggerFactory.CreateLogger<ManageController>();
            _urlEncoder = urlEncoder;
            _fact = factory;
            _redisSessionStorage = redisSessionStorage;
            _userdata = userdata;
            _ipAddressService = ipAddressService;
            _basePage = basePage;
            _iDeviceIdService = iDeviceIdService;
            _iuserChangeLog = userChangeLog;
            _iipHistory = iipHistory;
            _loginHistory = loginHistory;
            _signalRService = signalRService;
            _signupLogService = signupLogService;
            _userKeyMasterService = userKeyMasterService;
            _IactivityRegisterService = IactivityRegisterService;
            _encdecAEC = encdecAEC;
            _configuration = configuration;
            _messageService = MessageService;
            _pushNotificationsQueue = pushNotificationsQueue;
            _pushSMSQueue = pushSMSQueue;
            _otpMasterService = otpMasterService;
            _manageService = manageService;
            _custompassword = custompassword;
        }
        #endregion

        #region Method

        //// OLD
        //[HttpGet("userinfo")]
        //public async Task<IActionResult> UserInfoOLD() //[FromHeader] string RedisDBKey)
        //{
        //    try
        //    {
        //        var user = await GetCurrentUserAsync();


        //        var UserData = new IndexViewModel
        //        {
        //            FirstName = user.FirstName,
        //            LastName = user.LastName,
        //            Username = user.UserName,
        //            IsEmailConfirmed = user.EmailConfirmed,
        //            Email = user.Email,
        //            PhoneNumber = user.PhoneNumber,
        //            MobileNo = user.Mobile,
        //            TwoFactorEnabled = user.TwoFactorEnabled,
        //            SocialProfile = "No"
        //        };
        //        var UserId = await HttpContext.GetTokenAsync("access_token");

        //        HttpContext.Items[UserId] = user.Id;
        //        return Ok(new UserInfoResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessfullGetUserData, UserData = UserData });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        // khushali commented by for testing CORS method

        [HttpGet("userinfo")]
        public async Task<IActionResult> UserInfo() //[FromHeader] string RedisDBKey)
        {
            try
            {
                var UserId = await HttpContext.GetTokenAsync("access_token");

                //HttpContext.Items[UserId] = user.Id;
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                //var user = await GetCurrentUserAsync();
                var userObjData = _manageService.GetUserInfo(user.Id);

                var UserData = new IndexViewModel
                {
                    FirstName = userObjData.UserData.FirstName,
                    LastName = userObjData.UserData.LastName,
                    Username = userObjData.UserData.Username,
                    IsEmailConfirmed = userObjData.UserData.IsEmailConfirmed,
                    Email = userObjData.UserData.Email,
                    PhoneNumber = userObjData.UserData.PhoneNumber,
                    MobileNo = userObjData.UserData.MobileNo,
                    TwoFactorEnabled = userObjData.UserData.TwoFactorEnabled,
                    IsDeviceEnabled = userObjData.UserData.IsDeviceEnabled,
                    SocialProfile = userObjData.UserData.SocialProfile
                };
                //var UserId = await HttpContext.GetTokenAsync("access_token");

                HttpContext.Items[UserId] = user.Id;
                return Ok(new UserInfoResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessfullGetUserData, UserData = UserData });
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("userinfo")]
        public async Task<IActionResult> UserInfo([FromBody]UserInfoViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 04-04-2019 for check use exist or not
                    return BadRequest(new UserInfoResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                string Oldvalue = JsonConvert.SerializeObject(user);
                if (!string.IsNullOrEmpty(model.FirstName))
                    user.FirstName = model.FirstName;
                if (!string.IsNullOrEmpty(model.LastName))
                    user.LastName = model.LastName;


                var result = await _userManager.UpdateAsync(user);
                if (result == IdentityResult.Success)
                {
                    string Newvalue = JsonConvert.SerializeObject(user);

                    UserChangeLogViewModel userChangeLogViewModel = new UserChangeLogViewModel();
                    userChangeLogViewModel.Id = user.Id;
                    userChangeLogViewModel.Newvalue = Newvalue;
                    userChangeLogViewModel.Type = EnuserChangeLog.UserProfile.ToString();
                    userChangeLogViewModel.Oldvalue = Oldvalue;

                    long userlog = _iuserChangeLog.AddPassword(userChangeLogViewModel);
                    var UserData = new IndexViewModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Username = user.UserName,
                        IsEmailConfirmed = user.EmailConfirmed,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        MobileNo = user.Mobile,
                        TwoFactorEnabled = user.TwoFactorEnabled
                    };

                    ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                    ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.UserProfileUpdateSuccess);
                    //ActivityNotification.Param1 = coinName;                    
                    ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Success);
                    _signalRService.SendActivityNotificationV2(ActivityNotification, user.Id.ToString(), 2);

                    var UserId = await HttpContext.GetTokenAsync("access_token");
                    HttpContext.Items[UserId] = user.Id;
                    return Ok(new UserInfoResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessfullUpdateUserData, UserData = UserData });
                }
                return BadRequest(new UserInfoResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Unableupdateuserinfo, ErrorCode = enErrorCode.Status4034UnableUpdateUser });
            }
            catch (Exception ex)
            {

                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("TwoFAVerifyCode")]
        public async Task<IActionResult> TwoFAVerifyCode([FromBody]UserTwoFACodeVerifyViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();

                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                if (user.TwoFactorEnabled)
                {
                    TwoFactorAuth TFAuth = new TwoFactorAuth();
                    string code = TFAuth.GetCode(user.PhoneNumber);
                    var UserId = await HttpContext.GetTokenAsync("access_token");
                    HttpContext.Items[UserId] = user.Id;
                    if (model.Code == code)
                    {
                        return Ok(new VerifyCodeResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.TwoFAVerify });
                    }
                    else
                    {
                        return BadRequest(new VerifyCodeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FactorFail, ErrorCode = enErrorCode.Status4054FactorFail });
                    }
                }
                else
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.TwoFactorRequired, ErrorCode = enErrorCode.Status4140TwoFactorRequired });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new LoginResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #region IpAddress
        [HttpPost("IpAddress")]
        public async Task<IActionResult> AddIpAddress([FromBody]IpAddressReqViewModel model)
        {
            try
            {
                if (_configuration["IPByPassLogin"].ToString() == "False")
                {
                    if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
                    {
                        model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        return Ok(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                }
                string UserCountryCode = await _userdata.GetCountryByIP(model.SelectedIPAddress);
                if (!string.IsNullOrEmpty(UserCountryCode) && UserCountryCode == "fail")
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidUserSelectedIp, ErrorCode = enErrorCode.Status4045InvalidUserSelectedIp });
                }

                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });


                long getIp = await _ipAddressService.GetIpAddressByUserIdandAddress(model.SelectedIPAddress, user.Id);
                if (getIp > 0)
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAlreadyExist, ErrorCode = enErrorCode.Status4083IpAddressExist });
                }

                IpMasterViewModel imodel = new IpMasterViewModel();
                imodel.UserId = user.Id;
                imodel.IpAddress = model.SelectedIPAddress;
                if (!string.IsNullOrEmpty(model.IpAliasName))
                {
                    imodel.IpAliasName = model.IpAliasName;
                }

                long id = await _ipAddressService.AddIpAddress(imodel);
                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;
                if (id > 0)
                {
                    return Ok(new IpAddressResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessAddIpData });
                }
                else
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInsertError, ErrorCode = enErrorCode.Status4081IpAddressNotInsert });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }


        [HttpPost("UpdateIpAddress")]
        public async Task<IActionResult> UpdateIpAddress([FromBody]IpAddressReqViewModel model)
        {
            try
            {
                if (_configuration["IPByPassLogin"].ToString() == "False")
                {
                    if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
                    {
                        model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        return Ok(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                }
                string UserCountryCode = await _userdata.GetCountryByIP(model.SelectedIPAddress);
                if (!string.IsNullOrEmpty(UserCountryCode) && UserCountryCode == "fail")
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidUserSelectedIp, ErrorCode = enErrorCode.Status4045InvalidUserSelectedIp });
                }

                var user = await GetCurrentUserAsync();

                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });


                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                long getIp = await _ipAddressService.GetIpAddressByUserIdandAddress(model.SelectedIPAddress, user.Id);
                if (getIp > 0)
                {


                    IpMasterViewModel imodel = new IpMasterViewModel();
                    imodel.UserId = user.Id;
                    imodel.Id = getIp;
                    imodel.IpAddress = model.SelectedIPAddress;
                    if (!string.IsNullOrEmpty(model.IpAliasName))
                    {
                        imodel.IpAliasName = model.IpAliasName;
                    }

                    long id = await _ipAddressService.UpdateIpAddress(imodel);

                    if (id > 0)
                    {
                        return Ok(new IpAddressResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessupdateIpData });
                    }
                    else
                    {
                        return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInsertError, ErrorCode = enErrorCode.Status4081IpAddressNotInsert });
                    }
                }
                else
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInsertError, ErrorCode = enErrorCode.Status4081IpAddressNotInsert });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }



        [HttpPost("DisableIpAddress")]
        public async Task<IActionResult> DisableIpAddress([FromBody]IpAddressReqViewModel model)
        {
            try
            {
                if (_configuration["IPByPassLogin"].ToString() == "False")
                {
                    if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
                    {
                        model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        return Ok(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                }
                string UserCountryCode = await _userdata.GetCountryByIP(model.SelectedIPAddress);
                if (!string.IsNullOrEmpty(UserCountryCode) && UserCountryCode == "fail")
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidUserSelectedIp, ErrorCode = enErrorCode.Status4045InvalidUserSelectedIp });
                }

                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                IpMasterViewModel imodel = new IpMasterViewModel();
                imodel.UserId = user.Id;
                imodel.IpAddress = model.SelectedIPAddress;

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                long id = await _ipAddressService.DesableIpAddress(imodel);
                if (id > 0)
                {
                    return Ok(new IpAddressResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDesableIpStatus });
                }
                else
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressUpdateError, ErrorCode = enErrorCode.Status4046NotUpdateIpStatus });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("EnableIpAddress")]
        public async Task<IActionResult> EnableIpAddress([FromBody]IpAddressReqViewModel model)
        {
            try
            {
                if (_configuration["IPByPassLogin"].ToString() == "False")
                {
                    if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
                    {
                        model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        return Ok(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                }
                string UserCountryCode = await _userdata.GetCountryByIP(model.SelectedIPAddress);
                if (!string.IsNullOrEmpty(UserCountryCode) && UserCountryCode == "fail")
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidUserSelectedIp, ErrorCode = enErrorCode.Status4045InvalidUserSelectedIp });
                }

                var user = await GetCurrentUserAsync();

                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });


                IpMasterViewModel imodel = new IpMasterViewModel();
                imodel.UserId = user.Id;
                imodel.IpAddress = model.SelectedIPAddress;

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                long id = await _ipAddressService.EnableIpAddress(imodel);
                if (id > 0)
                {
                    return Ok(new IpAddressResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessEnableIpStatus });
                }
                else
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressUpdateError, ErrorCode = enErrorCode.Status4046NotUpdateIpStatus });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("DeleteIpAddress")]
        public async Task<IActionResult> DeleteIpAddress([FromBody]IpAddressReqViewModel model)
        {
            try
            {
                if (_configuration["IPByPassLogin"].ToString() == "False")
                {
                    if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
                    {
                        model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        return Ok(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                }
                string UserCountryCode = await _userdata.GetCountryByIP(model.SelectedIPAddress);
                if (!string.IsNullOrEmpty(UserCountryCode) && UserCountryCode == "fail")
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidUserSelectedIp, ErrorCode = enErrorCode.Status4045InvalidUserSelectedIp });
                }

                var user = await GetCurrentUserAsync();

                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                IpMasterViewModel imodel = new IpMasterViewModel();
                imodel.UserId = user.Id;
                imodel.IpAddress = model.SelectedIPAddress;

                long id = await _ipAddressService.DeleteIpAddress(imodel);
                if (id > 0)
                {
                    return Ok(new IpAddressResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDeleteIpAddress });
                }
                else
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressdeleteError, ErrorCode = enErrorCode.Status4047NotDeleteIp });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpGet("GetIpAddress/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetIpAddress(int PageIndex = 0, int Page_Size = 0, string IPAddress = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                var Response = new IpMasterGetResponse();
                Response = await _ipAddressService.GetIpAddressListByUserId(user.Id, PageIndex, Page_Size, IPAddress, FromDate, ToDate);
                long TotalRowCount = 0;
                if (Response != null)
                {
                    if (Response.TotalCount > 0)
                    {
                        TotalRowCount = Response.TotalCount;
                    }
                    if(Response.Result.Count() == 0)
                    {
                        return Ok(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound, IpList = Response.Result, TotalRow = TotalRowCount });
                    }
                }
                return Ok(new IpAddressResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetIpData, IpList = Response.Result, TotalRow = TotalRowCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region DeviceId
        [HttpPost("AddDevice")]
        public async Task<IActionResult> AddDevice([FromBody]DeviceIdReqViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                long DeviceId = _iDeviceIdService.GetDeviceByUserIdandId(model.SelectedDeviceId, user.Id);
                if (DeviceId > 0)
                {
                    return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeviceIdAlreadyExist, ErrorCode = enErrorCode.Status4084DeviceIdExist });
                }

                DeviceMasterViewModel imodel = new DeviceMasterViewModel();
                imodel.UserId = user.Id;
                imodel.DeviceId = model.SelectedDeviceId;

                long id = _iDeviceIdService.AddDeviceId(imodel);

                if (id > 0)
                {
                    return Ok(new DeviceIdResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessAddDeviceData });
                }
                else
                {
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeviceidInsertError, ErrorCode = enErrorCode.Status4057DeviceIdNotAdd });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("DisableDeviceId")]
        public async Task<IActionResult> DisableDeviceId([FromBody]DeviceIdDetReqViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                DeviceMasterViewModel imodel = new DeviceMasterViewModel();
                imodel.UserId = user.Id;
                imodel.Id = model.SelectedDeviceId;
                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                long id = _iDeviceIdService.DesableDeviceId(imodel);
                if (id > 0)
                {
                    return Ok(new DeviceIdResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDisableDeviceId });
                }
                else
                {
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeviceAddressUpdateError, ErrorCode = enErrorCode.Status4058DeviceAddress });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpPost("EnableDeviceId")]
        public async Task<IActionResult> EnableDeviceId([FromBody]DeviceIdDetReqViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                DeviceMasterViewModel imodel = new DeviceMasterViewModel();
                imodel.UserId = user.Id;
                imodel.Id = model.SelectedDeviceId;

                long id = _iDeviceIdService.EnableDeviceId(imodel);
                if (id > 0)
                {
                    return Ok(new DeviceIdResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessEnableDeviceId });
                }
                else
                {
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeviceAddressUpdateError, ErrorCode = enErrorCode.Status4058DeviceAddress });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("DeleteDeviceId")]
        public async Task<IActionResult> DeleteDeviceId([FromBody]DeviceIdDetReqViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                DeviceMasterViewModel imodel = new DeviceMasterViewModel();
                imodel.UserId = user.Id;
                imodel.Id = model.SelectedDeviceId;

                long id = _iDeviceIdService.DeleteDeviceId(imodel);
                if (id > 0)
                {
                    return Ok(new DeviceIdResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDeleteDevice });
                }
                else
                {
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeviceAddressdeleteError, ErrorCode = enErrorCode.Status4059NotDeleteDevice });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }


        [HttpPost("DisableDeviceIdByAdmin")]
        public async Task<IActionResult> DisableDeviceIdByAdmin([FromBody]DeviceIdDetReqViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                DeviceMasterViewModel imodel = new DeviceMasterViewModel();
                imodel.UserId = user.Id;
                imodel.Id = model.SelectedDeviceId;
                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                long id = _iDeviceIdService.DisableDeviceIdByAdmin(imodel, user.Id);
                if (id > 0)
                {
                    return Ok(new DeviceIdResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDisableDeviceId });
                }
                else
                {
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeviceIdNotDisableByAdmin, ErrorCode = enErrorCode.Status4173DeviceIdNotDisable });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("EnableDeviceIdByAdmin")]
        public async Task<IActionResult> EnableDeviceIdByAdmin([FromBody]DeviceIdDetReqViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                DeviceMasterViewModel imodel = new DeviceMasterViewModel();
                imodel.UserId = user.Id;
                imodel.Id = model.SelectedDeviceId;

                long id = _iDeviceIdService.EnableDeviceIdByAdmin(imodel, user.Id);
                if (id > 0)
                {
                    return Ok(new DeviceIdResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessEnableDeviceId });
                }
                else
                {
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeviceIdNotEnableByAdmin, ErrorCode = enErrorCode.Status4174DeviceIdNotEnable });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("DeleteDeviceIdByAdmin")]
        public async Task<IActionResult> DeleteDeviceIdByAdmin([FromBody]DeviceIdDetReqViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                DeviceMasterViewModel imodel = new DeviceMasterViewModel();
                imodel.UserId = user.Id;
                imodel.Id = model.SelectedDeviceId;

                long id = _iDeviceIdService.DeleteDeviceIdByAdmin(imodel, user.Id);
                if (id > 0)
                {
                    return Ok(new DeviceIdResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDeleteDevice });
                }
                else
                {
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeviceIdNotDeleteByAdmin, ErrorCode = enErrorCode.Status4175DeviceIdNotEnable });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }


        [HttpGet("GetDeviceId/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetDeviceId(int PageIndex = 0, int Page_Size = 0, string device = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                var user = await GetCurrentUserAsync();

                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                var DeviceList = _iDeviceIdService.GetDeviceListByUserId(user.Id, PageIndex, Page_Size, device, FromDate, ToDate);
                if (DeviceList == null)
                    return Ok(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound, DeviceList = new List<GetDeviceData>(), TotalCount = 0 });
                if (DeviceList?.DeviceList.Count() == 0)
                {
                    return Ok(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound, DeviceList = DeviceList.DeviceList, TotalCount = DeviceList.TotalCount });
                }
                return Ok(new DeviceIdResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetDeviceData, DeviceList = DeviceList.DeviceList, TotalCount = DeviceList.TotalCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetDeviceDataForAdmin")]
        public async Task<IActionResult> GetDeviceDataForAdmin(int PageIndex, int Page_Size = 0, string UserName = null, string DeviceId = null, string Device = null, string DeviceOs = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                var DeviceList = await Task.FromResult(_iDeviceIdService.GetDeviceDataForAdmin(PageIndex + 1, Page_Size, UserName, DeviceId, Device, DeviceOs, FromDate, ToDate));
                if(DeviceList == null)
                {
                    return Ok(new DeviceIdResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound, DeviceList = new List<GetDeviceData>(), TotalCount = 0 });
                }
                return Ok(new DeviceIdResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetDeviceData, DeviceList = DeviceList.DeviceList, TotalCount = DeviceList.TotalCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new DeviceIdResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region IpHistory
        [HttpGet("GetIpHistory/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetIpHistory(int PageIndex = 0, int Page_Size = 0, string IPAddress = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new IpHistoryResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;
                var IpHistoryList = _iipHistory.GetIpHistoryListByUserId(user.Id, PageIndex, Page_Size, IPAddress, FromDate, ToDate);
                if(IpHistoryList == null)
                    return Ok(new IpHistoryResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound, Totalcount = 0, IpHistoryList = new List<IpHistoryDataViewModel>() });
                if (IpHistoryList?.IpHistoryList.Count() == 0)
                {
                    return Ok(new IpHistoryResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound, Totalcount = IpHistoryList.Totalcount, IpHistoryList = IpHistoryList.IpHistoryList });
                }
                return Ok(new IpHistoryResponse { ReturnCode = enResponseCode.Success, Totalcount = IpHistoryList.Totalcount, IpHistoryList = IpHistoryList.IpHistoryList, ReturnMsg = EnResponseMessage.SuccessGetIpHistory });

            }
            catch (Exception ex)
            {
                return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }





        #endregion

        #region LoginHistory
        [HttpGet("GetLoginHistory/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetLoginHistory(int PageIndex = 0, int Page_Size = 0, string IPAddress = null, string Device = null, string Location = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new LoginHistoryResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                var LoginHistoryList = _loginHistory.GetLoginHistoryByUserId(user.Id, PageIndex, Page_Size, IPAddress, Device, Location, FromDate, ToDate);
                if(LoginHistoryList == null)
                    return Ok(new LoginHistoryResponse { ReturnCode = enResponseCode.Fail, LoginHistoryList = new List<LoginHistoryDataViewModel>(), ReturnMsg = EnResponseMessage.NotFound });
                if (LoginHistoryList?.LoginHistoryList.Count() == 0)
                {
                    return Ok(new LoginHistoryResponse { ReturnCode = enResponseCode.Fail, LoginHistoryList = LoginHistoryList.LoginHistoryList, ReturnMsg = EnResponseMessage.NotFound });
                }
                return Ok(new LoginHistoryResponse { ReturnCode = enResponseCode.Success, TotalCount = LoginHistoryList.TotalCount, LoginHistoryList = LoginHistoryList.LoginHistoryList, ReturnMsg = EnResponseMessage.SuccessGetLoginHistory });

            }
            catch (Exception ex)
            {
                return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }
        #endregion

        #region SignUpLogReport
        [HttpGet("GetSignUpUserLog/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetSignUpUserLog(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new SignUpLogResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;


                var SignupLogHistoryList = await _signupLogService.GetSignUpLogHistoryByUserId(user.Id, PageIndex, Page_Size);

                return Ok(new SignUpLogResponse { ReturnCode = enResponseCode.Success, SignUpLogHistoryList = SignupLogHistoryList.SignUpLogHistoryList, TotalCount = SignupLogHistoryList.TotalCount, ReturnMsg = EnResponseMessage.SuccessGetsignUpLogHistory });
            }
            catch (Exception ex)
            {
                return BadRequest(new IpAddressResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        #endregion

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel model)
        {

            try
            {
                if (model.OldPassword == model.NewPassword)//add validation by mansi-24-09-2019
                {
                    return BadRequest(new ChangePasswordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Passwordmatch, ErrorCode = enErrorCode.Status4701newandoldPassMatch });
                }
                if (!model.NewPassword.Equals(model.ConfirmPassword))
                {
                    return BadRequest(new ChangePasswordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ResetConfirmPassMatch, ErrorCode = enErrorCode.Status4042ResetConfirmPassMatch });
                }
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new ChangePasswordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                string oldvalue = JsonConvert.SerializeObject(user);


                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    user = await GetCurrentUserAsync();

                    string Newvalue = JsonConvert.SerializeObject(user);
                    UserChangeLogViewModel userChangeLogViewModel = new UserChangeLogViewModel();
                    userChangeLogViewModel.Id = user.Id;
                    userChangeLogViewModel.Newvalue = Newvalue;
                    userChangeLogViewModel.Type = EnuserChangeLog.ChangePassword.ToString();
                    userChangeLogViewModel.Oldvalue = oldvalue;

                    long userlog = _iuserChangeLog.AddPassword(userChangeLogViewModel);

                    //2019-6-18
                    TypeLogRequest obj = new TypeLogRequest();
                    obj.UserID = user.Id;
                    obj.ActivityType = enActivityType.ResetPassword;
                    obj.OldValue = oldvalue;
                    obj.NewValue = Newvalue;
                    _custompassword.AddActivityTypeLog(obj);
                    //2019-6-18

                    return Ok(new ChangePasswordResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ChangePassword });

                }

                return BadRequest(new ChangePasswordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ResetConfirmOldNotMatch, ErrorCode = enErrorCode.Status4043ResetConfirmOldNotMatch });

            }
            catch (Exception ex)
            {

                return BadRequest(new ChangePasswordResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }


            //return BadRequest(new ApiError("Unable to change password"));
        }


        [HttpPost("setpassword")]
        public async Task<IActionResult> SetPassword([FromBody]SetPasswordViewModel model)
        {

            var user = await GetCurrentUserAsync();
            if (user == null) // khushali 05-04-2019 for check use exist or not
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

            var UserId = await HttpContext.GetTokenAsync("access_token");
            HttpContext.Items[UserId] = user.Id;

            string oldvalue = JsonConvert.SerializeObject(user);
            var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (result.Succeeded)
            {
                user = await GetCurrentUserAsync();
                string Newvalue = JsonConvert.SerializeObject(user);
                UserChangeLogViewModel userChangeLogViewModel = new UserChangeLogViewModel();
                userChangeLogViewModel.Id = user.Id;
                userChangeLogViewModel.Newvalue = Newvalue;
                userChangeLogViewModel.Type = EnuserChangeLog.SetPassword.ToString();
                userChangeLogViewModel.Oldvalue = oldvalue;

                long userlog = _iuserChangeLog.AddPassword(userChangeLogViewModel);
                return NoContent();
            }
            return BadRequest(new ApiError("Unable to set password"));
        }

        //khushali 04-04-2019 Optimize method
        [HttpGet("photo")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UserPhoto()
        {
            try
            {
                var profileImage = _userdata.GetUserPhoto(User.GetUserId());

                if (profileImage == null)
                {
                    return NotFound();
                }
                return Ok(Convert.ToBase64String(profileImage.Content));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Unable to found User Photo."));
            }
        }

        //khushali 04-04-2019 Optimize method
        [HttpPost("photo")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UserPhoto(IFormFile file)
        {
            try
            {
                var Result = await _userdata.PostUserPhotoAsync(file, User.GetUserId());
                if (Result == null)
                {
                    return BadRequest(new BizResponseClass() { ReturnMsg = "User photo upload Failed", ReturnCode = enResponseCode.Fail });
                }
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass() { ReturnMsg = "User photo upload Failed", ReturnCode = enResponseCode.InternalError });
            }
        }

        [HttpGet("getlogins")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetLogins()
        {
            var user = await GetCurrentUserAsync();
            return Ok(await _userManager.GetLoginsAsync(user));
        }

        [HttpPost("removelogin")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> RemoveLogin([FromBody]RemoveLoginViewModel account)
        {
            var user = await GetCurrentUserAsync();
            var result = await _userManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(new ApiError("Login cannot be removed"));
        }

        [HttpGet("managelogins")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ManageLogins()
        {
            var user = await GetCurrentUserAsync();
            var userLogins = await _userManager.GetLoginsAsync(user);
            var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            var otherLogins = schemes.Where(auth => userLogins.All(ul => auth.Name != ul.LoginProvider)).ToList();
            // ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;
            return Ok(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        [HttpPost("linklogin")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("LinkLoginCallback", "Manage");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return Challenge(properties, provider);
        }

        [HttpGet("linklogincallback")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUserAsync();
            var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
            if (info == null)
            {
                return BadRequest(new ApiError("Unable to find linked login info"));
            }
            var result = await _userManager.AddLoginAsync(user, info);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(new ApiError("Unable to link login"));

        }

        [HttpGet("GetUserActivityLog/{PageIndex}/{Page_Size}/{FromDate}/{ToDate}")]
        public async Task<IActionResult> GetUserActivityLog(int PageIndex, int Page_Size, DateTime FromDate, DateTime ToDate, string IpAddress, string DeviceId, string ActivityAliasName, string ModuleType, long? StatusCode)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new GetActivityLogResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var Userid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[Userid] = user.Id;
                //var GetAllActivityData = _IactivityRegisterService.GetBackofficeAllActivityLog_Old(user.Id, PageIndex, Page_Size, IpAddress, DeviceId, ActivityAliasName, "", ModuleType, StatusCode, FromDate, ToDate);
                var GetAllActivityData = _IactivityRegisterService.GetBackofficeAllActivityLog(user.Id, PageIndex, Page_Size, IpAddress, DeviceId, ActivityAliasName, ModuleType, StatusCode, FromDate, ToDate); // New change by Pratik 25-3-2019 Remove URL parameter to unnecessary pass to SP
                if (GetAllActivityData != null)
                    return Ok(new GetActivityLogResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetBackOffActivityData, GetActivityLogList = GetAllActivityData.GetActivityLogList, TotalCount = GetAllActivityData.TotalCount });
                else
                    return Ok(new ActivityRegisterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ActivityDataNotAvailable, ErrorCode = enErrorCode.Status4149ActivityDataNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new ActivityRegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetLoginStepProcess")]
        public async Task<IActionResult> GetLoginStepProcess()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new LoginStepResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var Userid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[Userid] = user.Id;

                LoginStepViewModel model = new LoginStepViewModel();
                model.EmailConfirmed = user.EmailConfirmed;
                model.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                model.TwoFactorEnabled = user.TwoFactorEnabled;

                return Ok(new LoginStepResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetLoginStepProcess, LoginStepProcess = model });
            }
            catch (Exception ex)
            {
                return BadRequest(new LoginStepResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("EmailVerification")]
        public async Task<IActionResult> EmailVerification([FromBody]EmailVerificationViewModel emailVerification)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var Userid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[Userid] = user.Id;

                //////////////////// Check bizUser  table in Email Exist or not
                var result = await _userManager.FindByEmailAsync(emailVerification.Email);
                if (!string.IsNullOrEmpty(result?.Email) && result.EmailConfirmed)
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserEmailExist, ErrorCode = enErrorCode.Status4098BizUserEmailExist });
                }

                if (!user.EmailConfirmed)
                {
                    user.Email = emailVerification.Email;
                    await _userManager.UpdateAsync(user);

                    byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                    EmailLinkTokenViewModel emailLinkTokenViewModel = new EmailLinkTokenViewModel();
                    emailLinkTokenViewModel.CurrentTime = DateTime.UtcNow;
                    emailLinkTokenViewModel.Expirytime = DateTime.UtcNow + TimeSpan.FromHours(Convert.ToInt64(_configuration["DefaultValidateLinkTimeSpan"]));
                    emailLinkTokenViewModel.Username = user.UserName;
                    emailLinkTokenViewModel.Email = emailVerification.Email;
                    emailLinkTokenViewModel.Id = user.Id;

                    string UserDetails = JsonConvert.SerializeObject(emailLinkTokenViewModel);
                    string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);

                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
                    //string ctokenlink = "http://localhost:60029/api/Manage/ConfirmEmailProcess?emailConfirmCode=" + Convert.ToBase64String(plainTextBytes);

                    string ctokenlink = _configuration["ConfirmEmailURL"].ToString() + Convert.ToBase64String(plainTextBytes);
                    //var confirmationLink = "<a class='btn-primary' href=\"" + ctokenlink + "\">Confirm email address</a>";
                    //SendEmailRequest request = new SendEmailRequest();
                    //request.Recepient = emailVerification.Email;

                    //IQueryable Result = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.ConfirmationMail), 0);
                    //foreach (TemplateMasterData Provider in Result)
                    //{

                    //    Provider.Content = Provider.Content.Replace("###Link###".ToUpper(), ctokenlink);
                    //    Provider.Content = Provider.Content.Replace("###year###".ToUpper(), DateTime.Now.Year.ToString());
                    //    if (!string.IsNullOrEmpty(user.Email))
                    //        Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), emailVerification.Email);
                    //    else
                    //        Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), string.Empty);
                    //    request.Body = Provider.Content;
                    //    request.Subject = Provider.AdditionalInfo;
                    //}
                    //_pushNotificationsQueue.Enqueue(request);


                    //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                    // khushali 30-01-2019 for Common Template Method call 
                    TemplateMasterData TemplateData = new TemplateMasterData();
                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    SendEmailRequest request = new SendEmailRequest();
                    if (!string.IsNullOrEmpty(emailVerification.Email))
                        communicationParamater.Param1 = emailVerification.Email;
                    else
                        communicationParamater.Param1 = string.Empty;
                    communicationParamater.Param2 = ctokenlink;
                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.ConfirmationMail, communicationParamater, enCommunicationServiceType.Email).Result;
                    if (TemplateData != null)
                    {
                        if (TemplateData.IsOnOff == 1)
                        {
                            request.Recepient = emailVerification.Email;
                            request.Body = TemplateData.Content;
                            request.Subject = TemplateData.AdditionalInfo;
                            _pushNotificationsQueue.Enqueue(request);
                        }
                    }

                    return Ok(new EmailVerificationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SendEmailVerfication });
                }
                else
                    return Ok(new EmailVerificationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.AlreadyEmailVef, ErrorCode = enErrorCode.Status4150AlreadyEmailVef });
            }
            catch (Exception ex)
            {
                return BadRequest(new EmailVerificationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ResendEmailVerification")]
        public async Task<IActionResult> ReSendEmailVerification([FromBody]EmailVerificationViewModel emailVerification)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var Userid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[Userid] = user.Id;

                //////////////////// Check bizUser  table in Email Exist or not
                var result = await _userManager.FindByEmailAsync(emailVerification.Email);
                if (!string.IsNullOrEmpty(result?.Email) && result.EmailConfirmed)
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserEmailExist, ErrorCode = enErrorCode.Status4098BizUserEmailExist });
                }

                if (!user.EmailConfirmed)
                {
                    user.Email = emailVerification.Email;
                    await _userManager.UpdateAsync(user);

                    byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                    EmailLinkTokenViewModel emailLinkTokenViewModel = new EmailLinkTokenViewModel();
                    emailLinkTokenViewModel.CurrentTime = DateTime.UtcNow;
                    emailLinkTokenViewModel.Expirytime = DateTime.UtcNow + TimeSpan.FromHours(Convert.ToInt64(_configuration["DefaultValidateLinkTimeSpan"]));
                    emailLinkTokenViewModel.Username = user.UserName;
                    emailLinkTokenViewModel.Email = emailVerification.Email;
                    emailLinkTokenViewModel.Id = user.Id;

                    string UserDetails = JsonConvert.SerializeObject(emailLinkTokenViewModel);
                    string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);

                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);

                    string ctokenlink = _configuration["ConfirmEmailURL"].ToString() + Convert.ToBase64String(plainTextBytes);

                    //SendEmailRequest request = new SendEmailRequest();
                    //request.Recepient = emailVerification.Email;

                    //IQueryable Result = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.ConfirmationMail), 0);
                    //foreach (TemplateMasterData Provider in Result)
                    //{

                    //    Provider.Content = Provider.Content.Replace("###Link###".ToUpper(), ctokenlink);
                    //    Provider.Content = Provider.Content.Replace("###year###".ToUpper(), DateTime.Now.Year.ToString());
                    //    if (!string.IsNullOrEmpty(user.Email))
                    //        Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), emailVerification.Email);
                    //    else
                    //        Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), string.Empty);
                    //    request.Body = Provider.Content;
                    //    request.Subject = Provider.AdditionalInfo;
                    //}
                    //_pushNotificationsQueue.Enqueue(request);

                    //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                    // khushali 30-01-2019 for Common Template Method call 
                    TemplateMasterData TemplateData = new TemplateMasterData();
                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    SendEmailRequest request = new SendEmailRequest();
                    if (!string.IsNullOrEmpty(emailVerification.Email))
                        communicationParamater.Param1 = emailVerification.Email;
                    else
                        communicationParamater.Param1 = string.Empty;
                    communicationParamater.Param2 = ctokenlink;
                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.ConfirmationMail, communicationParamater, enCommunicationServiceType.Email).Result;
                    if (TemplateData != null)
                    {
                        if (TemplateData.IsOnOff == 1)
                        {
                            request.Recepient = emailVerification.Email;
                            request.Body = TemplateData.Content;
                            request.Subject = TemplateData.AdditionalInfo;
                            _pushNotificationsQueue.Enqueue(request);
                        }
                    }
                    return Ok(new EmailVerificationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SendEmailVerfication });
                }
                else
                    return Ok(new EmailVerificationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.AlreadyEmailVef, ErrorCode = enErrorCode.Status4150AlreadyEmailVef });
            }
            catch (Exception ex)
            {
                return BadRequest(new EmailVerificationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("ConfirmEmailProcess")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailProcess(string emailConfirmCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailConfirmCode))
                {
                    //add validation LinkData valid or not -mansi 23-09-2019
                    if (string.IsNullOrEmpty(emailConfirmCode) || emailConfirmCode.Length % 4 != 0 || emailConfirmCode.Contains(" ") || emailConfirmCode.Contains("\t") || emailConfirmCode.Contains("\r") || emailConfirmCode.Contains("\n"))

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
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Invalidlinkdata, ErrorCode = enErrorCode.Status1450invalidelinkdata });
                    }
                    
                    //var encodedString = Encoding.UTF8.GetString(bytes);
                    //string DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);

                    EmailLinkTokenViewModel dmodel = JsonConvert.DeserializeObject<EmailLinkTokenViewModel>(DecryptToken);
                    if (dmodel?.Expirytime >= DateTime.UtcNow)
                    {
                        if (dmodel.Id == 0)
                            return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignEmailUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                        else
                        {
                            var user = await _userManager.FindByIdAsync(dmodel.Id.ToString());
                            if (user == null)
                                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignEmailUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                            else
                            {
                                user.EmailConfirmed = true;
                                await _userManager.UpdateAsync(user);
                                return Ok(new EmailVerificationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessEmailConfirm });
                            }
                        }

                    }
                    else
                        return BadRequest(new EmailVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpEmailExpired, ErrorCode = enErrorCode.Status4039ResetPasswordLinkExpired });
                }
                else
                    return BadRequest(new EmailVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignEmailLink, ErrorCode = enErrorCode.Status4064EmailLinkBlanck });
            }
            catch (Exception ex)
            {
                return BadRequest(new EmailVerificationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("MobileVerification")]
        public async Task<IActionResult> MobileVerification([FromBody]MobileVerificationViewModel MobileVerification)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var Userid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[Userid] = user.Id;

                ///////////////// Check bizUser  table in username  Exist or not
                var resultUserName = await _userManager.FindByNameAsync(MobileVerification.Mobile);
                if (!string.IsNullOrEmpty(resultUserName?.UserName) && resultUserName.PhoneNumberConfirmed)
                {
                    //return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserNameAs_a_MobileExist, ErrorCode = enErrorCode.Status4103BizUserNameAs_a_MobileExist });
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status4074SignUPMobileValidation });
                }

                ///////////////// Check bizUser  table in mobile number  Exist or not
                bool IsSignMobile = _userdata.GetMobileNumber(MobileVerification.Mobile);
                if (!IsSignMobile)
                {
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status4074SignUPMobileValidation });
                }

                if (!user.PhoneNumberConfirmed)
                {
                    user.Mobile = MobileVerification.Mobile;
                    await _userManager.UpdateAsync(user);

                    var otpData = await _otpMasterService.AddOtp(Convert.ToInt32(user.Id), "", MobileVerification.Mobile);

                    return Ok(new MobileVerificationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.OTPSendOnMobile });
                }
                else
                    return Ok(new MobileVerificationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.AlreadyMobileVef, ErrorCode = enErrorCode.Status4151AlreadyMobileVef });
            }
            catch (Exception ex)
            {
                return BadRequest(new EmailVerificationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("ReSendMobileVerification")]
        public async Task<IActionResult> ReSendMobileVerification([FromBody]MobileVerificationViewModel MobileVerification)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var Userid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[Userid] = user.Id;

                ///////////////// Check bizUser  table in username  Exist or not
                var resultUserName = await _userManager.FindByNameAsync(MobileVerification.Mobile);
                if (!string.IsNullOrEmpty(resultUserName?.UserName) && resultUserName.PhoneNumberConfirmed)
                {
                    //return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserNameAs_a_MobileExist, ErrorCode = enErrorCode.Status4103BizUserNameAs_a_MobileExist });
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status4074SignUPMobileValidation });
                }

                ///////////////// Check bizUser  table in mobile number  Exist or not
                bool IsSignMobile = _userdata.GetMobileNumber(MobileVerification.Mobile);
                if (!IsSignMobile)
                {
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status4074SignUPMobileValidation });
                }


                if (!user.PhoneNumberConfirmed)
                {
                    user.Mobile = MobileVerification.Mobile;
                    await _userManager.UpdateAsync(user);
                    var otpData = await _otpMasterService.AddOtp(Convert.ToInt32(user.Id), "", MobileVerification.Mobile);

                    return Ok(new MobileVerificationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.OTPSendOnMobile });
                }
                else
                    return Ok(new MobileVerificationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.AlreadyMobileVef, ErrorCode = enErrorCode.Status4151AlreadyMobileVef });
            }
            catch (Exception ex)
            {
                return BadRequest(new EmailVerificationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("MobileOtpVerification")]
        public async Task<IActionResult> MobileOtpVerification([FromBody]OTPWithMobileViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var Userid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[Userid] = user.Id;

                var result = await _userManager.FindByIdAsync(user.Id.ToString());
                if (!string.IsNullOrEmpty(result?.Mobile))
                {
                    var otpdata = await _otpMasterService.GetOtpData(Convert.ToInt32(user.Id),2);
                    if (otpdata != null)
                    {
                        if (otpdata?.ExpirTime >= DateTime.UtcNow)
                        {
                            if (model.OTP == otpdata.OTP)
                            {
                                _otpMasterService.UpdateOtp(otpdata.Id, 2, "");
                                user.PhoneNumberConfirmed = true;
                                await _userManager.UpdateAsync(user);
                                return Ok(new OTPWithMobileResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessMobileConfirm });
                            }
                            else
                                return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginWithOtpInvalidAttempt, ErrorCode = enErrorCode.Status4088LoginWithOtpInvalidAttempt });
                        }
                        else
                            return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpResendOTP, ErrorCode = enErrorCode.Status4067InvalidOTPorexpired });
                    }
                    else
                        return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginMobileNumberInvalid, ErrorCode = enErrorCode.Status4091LoginMobileNumberInvalid });
                }
                else
                    return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginMobileNumberInvalid, ErrorCode = enErrorCode.Status4091LoginMobileNumberInvalid });
            }
            catch (Exception ex)
            {
                return BadRequest(new MobileVerificationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("DayNightModePreference")]
        public async Task<IActionResult> DayNightModePreference([FromBody]DayNightModeViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new DayNightModeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                HttpContext.Items["UserId"] = user.Id;
                if (user.Thememode == model.DayNightMode)
                {
                    if (user.Thememode)
                        return Ok(new DayNightModeResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.NightModeAlreadyActivated, ErrorCode = enErrorCode.Status4162NightModeAlreadyActivated });
                    else
                        return Ok(new DayNightModeResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DayModeAlreadyActivated, ErrorCode = enErrorCode.Status4161DayModeAlreadyActivated });
                }

                user.Thememode = model.DayNightMode;
                var result = await _userManager.UpdateAsync(user);
                return Ok(new DayNightModeResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessfullUpdateUserTheme });
            }
            catch (Exception ex)
            {
                return BadRequest(new DayNightModeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Helpers

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        #endregion

        #region Language Prefernce

        [HttpPost("UpdateLanguagePreference/{PreferedLanguage}")]
        public async Task<IActionResult> UpdateLanguagePreference(string PreferedLanguage)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                HttpContext.Items["UserId"] = user.Id;

                var language = _userdata.GetLanguagePreferenceMaster(PreferedLanguage);
                if (language == null)
                {
                    return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidLang, ErrorCode = enErrorCode.InvalidLang });
                }

                user.PreferedLanguage = PreferedLanguage;
                var result = await _userManager.UpdateAsync(user);
                return Ok(new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessfullUpdateUserLang, ErrorCode = enErrorCode.SuccessfullUpdateUserLang });
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion


        #region NewsLetter

        [HttpPost("SubscribeNewsLetter/{Email}")]
        [AllowAnonymous]
        public ActionResult<BizResponseClass> SubscribeNewsLetter(String Email)
        {
            try
            {
                Regex regex = new Regex(@"^[-a-zA-Z0-9~!$%^&*_=+}{\'?]+(\.[-a-zA-Z0-9~!$%^&*_=+}{\'?]+)*@([a-zA-Z0-9_][-a-zA-Z0-9_]*(\.[-a-zA-Z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-zA-Z]{2,3})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$");
                Match match = regex.Match(Email);
                if (!match.Success)
                {
                    return new BizResponseClass() {ErrorCode=enErrorCode.EnterValidEmail,ReturnCode=enResponseCode.Fail, ReturnMsg= "EnterValidEmail" };
                }
                return _manageService.AddSubscribeNewsLetter(Email);
            }
            catch (Exception ex)
            {
                return BadRequest(new DayNightModeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("RemoveSubscribeNewsLetter/{Email}")]
        [AllowAnonymous]
        public ActionResult<BizResponseClass> RemoveSubscribeNewsLetter(String Email)
        {
            try
            {
                Regex regex = new Regex(@"^[-a-zA-Z0-9~!$%^&*_=+}{\'?]+(\.[-a-zA-Z0-9~!$%^&*_=+}{\'?]+)*@([a-zA-Z0-9_][-a-zA-Z0-9_]*(\.[-a-zA-Z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-zA-Z]{2,3})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$");
                Match match = regex.Match(Email);
                if (!match.Success)
                {
                    return new BizResponseClass() { ErrorCode = enErrorCode.EnterValidEmail, ReturnCode = enResponseCode.Fail, ReturnMsg = "EnterValidEmail" };
                }
                return _manageService.RemoveSubscribeNewsLetter(Email);
            }
            catch (Exception ex)
            {
                return BadRequest(new DayNightModeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion
    }
}

