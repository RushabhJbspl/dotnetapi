using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.User;
using Worldex.Core.Services;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.AccountViewModels;
using Worldex.Core.ViewModels.AccountViewModels.ForgotPassword;
using Worldex.Core.ViewModels.AccountViewModels.Login;
using Worldex.Core.ViewModels.AccountViewModels.Logoff;
using Worldex.Core.ViewModels.AccountViewModels.OTP;
using Worldex.Core.ViewModels.AccountViewModels.ResetPassword;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using Worldex.Core.ViewModels.ManageViewModels.TwoFA;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using TwoFactorAuthNet;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.ViewModels.AccountViewModels.Log;
using Worldex.Core.Interfaces;
using Worldex.Core.ApiModels;
using Worldex.Infrastructure.BGTask;
using Worldex.Core.ApiModels.Chat;
////using Worldex.Web.Helper;
using Worldex.Core.Interfaces.BackOffice;
using System.Net;
using Worldex.Core.Interfaces.PasswordPolicy;
using Worldex.Core.PasswordPolicy;
using Worldex.Core.ViewModels.BackOffice.PasswordPolicy;

namespace BackofficeWorldex.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SigninController : BaseController
    {
        #region Field
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        //private readonly IMediator _mediator;
        private readonly IUserService _userService;
        private readonly IOtpMasterService _otpMasterService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IBasePage _basePage;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly ICustomPassword _custompassword;
        private readonly Dictionary<string, IUserTwoFactorTokenProvider<ApplicationUser>> _tokenProviders =
            new Dictionary<string, IUserTwoFactorTokenProvider<ApplicationUser>>();
        private readonly ITempUserRegisterService _tempUserRegisterService;
        private readonly IUserKeyMasterService _userKeyMasterService;
        private readonly IipHistory _iipHistory;
        //private ApplicationUser _ApplicationUser; //komal 03 May 2019, Cleanup
        private readonly ILoginHistory _loginHistory;
        private readonly IMessageService _messageService;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue; //24-11-2018 komal make Email Enqueue
        private readonly ISignalRService _signalRService;
        private readonly IDeviceIdService _IdeviceIdService; // added by nirav savariya for device white list report on 12-07-2018
        private readonly IipAddressService _iipAddressService; //added by nirav savariya for ip address white list report on 12-07-2018
        private readonly IIPRange _IIPRange;
        private readonly IUserPasswordPolicyMaster _UserPasswordPolicyMaster;  /// Added by pankaj for check the user configuration
        private readonly IUserLinkMaster _IUserLinkMaster;
        #endregion

        #region Ctore
        public SigninController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory, IUserService userService, IOtpMasterService otpMasterService, Microsoft.Extensions.Configuration.IConfiguration configuration, IBasePage basePage,
            EncyptedDecrypted encypted, ICustomPassword custompassword, ITempUserRegisterService tempUserRegisterService, IUserKeyMasterService userKeyMasterService, IipHistory iipHistory, ILoginHistory loginHistory,
            IMessageService MessageService, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, ISignalRService signalRService,
            IDeviceIdService IdeviceIdService, IipAddressService iipAddressService, IIPRange IIPRange,
             IUserPasswordPolicyMaster userPasswordPolicyMaster, IUserLinkMaster userLinkMaster)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<SigninController>();
            //_mediator = mediator;
            _userService = userService;
            _otpMasterService = otpMasterService;
            _configuration = configuration;
            _basePage = basePage;
            _encdecAEC = encypted;
            _custompassword = custompassword;
            _tempUserRegisterService = tempUserRegisterService;
            _userKeyMasterService = userKeyMasterService;
            _iipHistory = iipHistory;
            _loginHistory = loginHistory;
            _messageService = MessageService;
            _pushNotificationsQueue = pushNotificationsQueue;
            _signalRService = signalRService;
            _IdeviceIdService = IdeviceIdService; // added by nirav savariya on 12-05-2018
            _iipAddressService = iipAddressService; // added by nirav savariya on 12-05-2018
            _IIPRange = IIPRange;///  Added by pankaj kathiriya on 04-01-2019 for valid the user ip in particular range   
            _UserPasswordPolicyMaster = userPasswordPolicyMaster;
            _IUserLinkMaster = userLinkMaster;
        }
        #endregion

        #region Utilities

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // Login method to call direct send code
        [HttpGet("SendCode")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return BadRequest(new LoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CommFailMsgInternal, ErrorCode = enErrorCode.Status400BadRequest });

            }
            //else

            //{
            //    var otpData = _otpMasterService.AddOtp(user.Id, user.Email, "");
            //    var message = "Your security code is: " + otpData;

            //        SendEmailRequest request = new SendEmailRequest();
            //        request.Recepient = user.Email;
            //        request.Subject = "Security Code";
            //        request.Body = message;

            //        await _mediator.Send(request);
            //        //await _emailSender.SendEmailAsync(user.Email, "Security Code", message);

            //}
            //return null;
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();

            var data = new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe, SelectedProvider = "Worldex" };
            return Ok(data);
            //return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost("SendCode")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SendCode([FromBody]SendCodeViewModel model)
        {

            try
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                    return BadRequest(new LoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CommFailMsgInternal, ErrorCode = enErrorCode.Status400BadRequest });

                // Generate the token and send it
                var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
                if (string.IsNullOrWhiteSpace(code))
                {

                    return BadRequest(new LoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CommFailMsgInternal, ErrorCode = enErrorCode.Status400BadRequest });
                }

                var message = "Your security code is: " + code;
                if (model.SelectedProvider == "Email")
                {
                    SendEmailRequest request = new SendEmailRequest();
                    request.Recepient = user.Email;
                    request.Subject = "Security Code";
                    request.Body = message;
                    _pushNotificationsQueue.Enqueue(request); //24-11-2018 komal make Email Enqueue
                    //await _mediator.Send(request);
                    //await _emailSender.SendEmailAsync(user.Email, "Security Code", message);
                }
                // else if (model.SelectedProvider == "Phone")
                // {
                //     await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
                // }

                return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // Login after verify code
        [HttpGet("VerifyCode")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {

                return BadRequest(new LoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CommFailMsgInternal, ErrorCode = enErrorCode.Status400BadRequest });
            }
            return View(new VerifyCodeViewModel { RememberMe = rememberMe });
        }

        [HttpPost("VerifyCode")]
        [AllowAnonymous]
        // [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> VerifyCode(TwoFACodeVerifyViewModel model)
        {
            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            //  var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
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
                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                // var Key = await _custompassword.GetPassword(user.Id);
                var TwoFAToken = _userKeyMasterService.GetUserUniqueKey(model.TwoFAKey);
                if (TwoFAToken != null)
                {
                    if (TwoFAToken.UniqueKey != model.TwoFAKey)
                        return BadRequest(new VerifyCodeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FactorFail, ErrorCode = enErrorCode.Status4054FactorFail });


                    var user = await _userManager.FindByIdAsync(TwoFAToken.UserId.ToString());
                    if (user != null)
                    {
                        //if (user.lo .IsLockedOut)
                        //{
                        //    return BadRequest(new VerifyCodeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginLockOut, ErrorCode = enErrorCode.Status423Locked });
                        //}

                        TwoFactorAuth TFAuth = new TwoFactorAuth();
                        //sKey = key; //TFAuth.CreateSecret(160);
                        string code = TFAuth.GetCode(user.PhoneNumber);
                        if (model.Code == code)
                        //bool status = TFAuth.VerifyCode(user.PhoneNumber, model.Code, 1, dt);
                        //if (status)
                        {
                            string Location = await _userService.GetLocationByIP(model.IPAddress);
                            ////bool checkvalidip = _iipAddressService.CheckValidIpaddress(user.Id, model.IPAddress);
                            bool checkvaliddevice = _IdeviceIdService.CheckValidDevice(user.Id, model.DeviceId);
                            if (checkvaliddevice)
                            {
                                //// Valid Key and status Disable
                                _userKeyMasterService.UpdateOtp(TwoFAToken.Id);
                                
                                //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                                // khushali 30-01-2019 for Common Template Method call 
                                TemplateMasterData TemplateData = new TemplateMasterData();
                                CommunicationParamater communicationParamater = new CommunicationParamater();
                                SendEmailRequest request = new SendEmailRequest();
                                communicationParamater.Param1 = user.UserName; //Username
                                communicationParamater.Param2 = model.DeviceId; //Devicename
                                communicationParamater.Param3 = DateTime.UtcNow.ToString(); //Time
                                communicationParamater.Param4 = model.IPAddress; //Ipaddress
                                communicationParamater.Param5 = Location; //Location
                                TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SuccessfulLogin, communicationParamater, enCommunicationServiceType.Email).Result;
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


                                // added by nirav savariya for Thememode for user base showing mode for Thememode=0(false) means Day mode and Thememode=1(true) on 1-10-2019 03-20 PM
                                return Ok(new VerifySuccResponse {PreferedLanguage=user.PreferedLanguage, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardLoginSuccess, Thememode = user.Thememode });
                            }
                            else
                            {
                                string[] DeviceDetails = null;
                                if (model.DeviceId.Contains('|'))
                                    DeviceDetails = model.DeviceId.Split('|');

                                AuthorizeDeviceViewModel authorizeDeviceView = new AuthorizeDeviceViewModel();
                                authorizeDeviceView.UserId = user.Id;
                                authorizeDeviceView.Location = Location;
                                authorizeDeviceView.IPAddress = model.IPAddress;
                                authorizeDeviceView.DeviceName = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                                authorizeDeviceView.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                                authorizeDeviceView.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                                authorizeDeviceView.CurrentTime = DateTime.UtcNow;
                                authorizeDeviceView.Expirytime = DateTime.UtcNow + TimeSpan.FromHours(Convert.ToInt32(_configuration["DefaultValidateAuthorizedDevice"]));


                                byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                                string UserDetails = JsonConvert.SerializeObject(authorizeDeviceView);
                                string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);
                                byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);

                                string ctokenlink = _configuration["AuthorizedDeviceURL"].ToString() + Convert.ToBase64String(plainTextBytes);
                                
                                //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                                // khushali 30-01-2019 for Common Template Method call 
                                TemplateMasterData TemplateData = new TemplateMasterData();
                                CommunicationParamater communicationParamater = new CommunicationParamater();
                                SendEmailRequest request = new SendEmailRequest();
                                communicationParamater.Param1 = Location; //Location
                                communicationParamater.Param2 = model.IPAddress; //IPAddress
                                communicationParamater.Param3 = DeviceDetails != null ? DeviceDetails[0] : string.Empty; //Device
                                communicationParamater.Param4 = ctokenlink; //Link
                                TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.AuthorizedNewDevice, communicationParamater, enCommunicationServiceType.Email).Result;
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
                                var UnAuthorizedDeviceMessage = EnResponseMessage.UnAuthorizedDevice.Replace("###SiteName###", _configuration["SiteNameForWelcomeMsg"]);
                                return Ok(new LoginWithEmailDataResponse { ReturnCode = enResponseCode.Success, ReturnMsg = UnAuthorizedDeviceMessage, ErrorCode = enErrorCode.Status4137UnAuthorizedDevice });
                            }
                        }
                        else
                        {
                            return BadRequest(new VerifyCodeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FactorFail, ErrorCode = enErrorCode.Status4054FactorFail });
                        }
                    }
                    else
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                    }
                }
                else
                {
                    return BadRequest(new VerifyCodeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FactorKeyFail, ErrorCode = enErrorCode.Status4107TwoFAKeyinvalid });
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(new LoginResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("DeviceAuthorizeV1")]
        [AllowAnonymous]
        public async Task<IActionResult> DeviceAuthorizeV1(String authorizecode)
        {
            string TokenID = "";
            try
            {
                TokenID = HttpContext.Request.Headers["AuthTokenID"];
                if (!String.IsNullOrEmpty(authorizecode) && !string.IsNullOrEmpty(TokenID))
                {
                    var dmodel = _IdeviceIdService.GetDeviceDetailsByGuid(TokenID);
                    if(dmodel==null)
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "UnAuthorize", ErrorCode = enErrorCode.UnAuthorize });
                    authorizecode =HttpContext.Request.QueryString.Value;
                    authorizecode=authorizecode.Replace("?authorizecode=", "");

                    //authorizecode= HttpUtility.UrlDecode(authorizecode);
                    var user = await _userManager.FindByIdAsync(dmodel.UserId.ToString());
                    var Result = await _userManager.ConfirmEmailAsync(user, authorizecode);
                    if (!Result.Succeeded)
                    {
                        _IdeviceIdService.UpdateDevaiceAuthorize(dmodel.ID, dmodel.UserId, 9);
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "UnAuthorize", ErrorCode = enErrorCode.UnAuthorize });
                    }
                    if (dmodel.Expirytime >= DateTime.UtcNow)
                    {
                        if (dmodel.UserId == 0)
                        {
                            _IdeviceIdService.UpdateDevaiceAuthorize(dmodel.ID, dmodel.UserId, 9);
                            return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AuthorizedUser, ErrorCode = enErrorCode.Status4135AuthorizedUser });
                        }
                        else
                        {
                            
                            if (_configuration["IPByPass"].ToString()=="False")
                            {
                                if (HttpContext.Connection.LocalIpAddress.ToString() != dmodel.IPAddress)
                                {
                                    _IdeviceIdService.UpdateDevaiceAuthorize(dmodel.ID, dmodel.UserId, 9);
                                    return BadRequest(new AuthorizeDeviceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidIPNDevice, ErrorCode = enErrorCode.Status4136InvalidIPNDevice });
                                }
                            }
                            if (!string.IsNullOrEmpty(dmodel.IPAddress) && !string.IsNullOrEmpty(dmodel.Device))
                            {
                                var flag = _IdeviceIdService.UpdateDevaiceAuthorize(dmodel.ID, dmodel.UserId, 1);
                                if (!flag)
                                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AuthorizedUser, ErrorCode = enErrorCode.Status4135AuthorizedUser });

                                IpMasterViewModel ipMaster = new IpMasterViewModel();
                                ipMaster.UserId = dmodel.UserId;
                                ipMaster.IpAddress = dmodel.IPAddress;
                                ipMaster.Status = 1;

                                _iipAddressService.AddIpAddressV1(ipMaster);

                                //DeviceMasterViewModel device = new DeviceMasterViewModel();
                                //device.Device = dmodel.Device;
                                //device.DeviceOS = dmodel.DeviceOS;
                                //device.DeviceId = dmodel.DeviceId;
                                //device.UserId = dmodel.UserId;
                                //_IdeviceIdService.AddDeviceProcess(device);

                                AuthorizeDeviceData authorizeDeviceData = new AuthorizeDeviceData();
                                authorizeDeviceData.DeviceName = dmodel.Device;
                                authorizeDeviceData.Location = dmodel.Location;
                                authorizeDeviceData.IPAddress = dmodel.IPAddress;

                                TypeLogRequest obj = new TypeLogRequest();
                                obj.UserID = dmodel.UserId;
                                obj.ActivityType = enActivityType.DeviceChange;
                                obj.OldValue = "";
                                obj.NewValue = dmodel.IPAddress;
                                //obj.ActivityType = enActivityType.ForgotPassword;
                                _custompassword.AddActivityTypeLog(obj);
                                //2019-6-18

                                return Ok(new AuthorizeDeviceDataResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.AuthorizedAddSuccessfully, AuthorizeData = authorizeDeviceData });
                            }
                            else
                            {
                                _IdeviceIdService.UpdateDevaiceAuthorize(dmodel.ID, dmodel.UserId, 9);
                                return BadRequest(new AuthorizeDeviceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidIPNDevice, ErrorCode = enErrorCode.Status4136InvalidIPNDevice });
                            }
                        }
                    }
                    else
                    {
                        _IdeviceIdService.UpdateDevaiceAuthorize(dmodel.ID, dmodel.UserId, 9);
                        return BadRequest(new AuthorizeDeviceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AuthorizedNewExpired, ErrorCode = enErrorCode.Status4133ResetAuthorizedLinkExpired });
                    }
                }
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AuthorizedLinkBlank, ErrorCode = enErrorCode.Status4134AuthorizedLinkBlanck });
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //[HttpGet("AuthorizeIP")]
        //[AllowAnonymous]
        //public async Task<IActionResult> AuthorizeIP(string authorizecode)
        //{
        //    string TokenID = "";
        //    try
        //    {
        //        TokenID = HttpContext.Request.Headers["AuthTokenID"];
        //        //TokenID = ReqTokenID;//For Testing Purpose
        //        if (!string.IsNullOrEmpty(authorizecode) && !string.IsNullOrEmpty(TokenID))
        //        {
        //            var dmodel = await _iipHistory.GetIPMasterDetailsByGuid(TokenID);
        //            if (dmodel == null)
        //            {
        //                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = "UnAuthorize AuthToken", ErrorCode = enErrorCode.UnAuthorize });
        //            }
        //            authorizecode = HttpContext.Request.QueryString.Value;
        //            authorizecode = authorizecode.Replace("?authorizecode=", "");
        //            var user = await _userManager.FindByIdAsync(dmodel.UserId.ToString());
        //            var Result = await _userManager.ConfirmEmailAsync(user, authorizecode);
        //            if (!Result.Succeeded)
        //            {
        //                await _iipHistory.UpdateIPAddressAuthorization(dmodel.Id, dmodel.UserId, 9);
        //                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = "UnAuthorize Link", ErrorCode = enErrorCode.Status1450invalidelinkdata });
        //            }
        //            DateTime ExpiryTime = dmodel.CreatedDate.AddMinutes(Convert.ToInt32(_configuration["DefaultValidateAuthorizedDevice"]));
        //            if (ExpiryTime >= DateTime.UtcNow)
        //            {
        //                if (dmodel.UserId == 0)
        //                {
        //                    await _iipHistory.UpdateIPAddressAuthorization(dmodel.Id, dmodel.UserId, 9);
        //                    return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AuthorizedUser, ErrorCode = enErrorCode.Status4135AuthorizedUser });
        //                }
        //                else
        //                {
        //                    if (_configuration["IPByPass"].ToString() == "False")
        //                    {
        //                        if (HttpContext.Connection.RemoteIpAddress.ToString() != dmodel.IpAddress)
        //                        {
        //                            await _iipHistory.UpdateIPAddressAuthorization(dmodel.Id, dmodel.UserId, 9);
        //                            return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidIPNDevice, ErrorCode = enErrorCode.Status4136InvalidIPNDevice });
        //                        }
        //                    }
        //                    if (!string.IsNullOrEmpty(dmodel.IpAddress))
        //                    {
        //                        var flag = await _iipHistory.UpdateIPAddressAuthorization(dmodel.Id, dmodel.UserId, 1);
        //                        if (!flag)
        //                        {
        //                            return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AuthorizedUser, ErrorCode = enErrorCode.Status4135AuthorizedUser });
        //                        }
        //                        return Ok(new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.AuthorizedIPSuccessfully });
        //                    }
        //                    else
        //                    {
        //                        await _iipHistory.UpdateIPAddressAuthorization(dmodel.Id, dmodel.UserId, 9);
        //                        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidIPNDevice, ErrorCode = enErrorCode.Status4136InvalidIPNDevice });
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                await _iipHistory.UpdateIPAddressAuthorization(dmodel.Id, dmodel.UserId, 9);
        //                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AuthorizedNewExpired, ErrorCode = enErrorCode.Status4133ResetAuthorizedLinkExpired });
        //            }
        //        }
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AuthorizedLinkBlank, ErrorCode = enErrorCode.Status4134AuthorizedLinkBlanck });
        //    }
        //    catch (Exception ex)
        //    {
        //        Worldex.Core.Helpers.HelperForLog.WriteErrorLog("AuthorizeIP", "SignInController", ex);
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = "Internal Server Error", ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        #endregion

        #region Methods

        [HttpPost("login")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Login(Worldex.Core.ViewModels.AccountViewModels.LoginViewModel model)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);
                    return Ok(new LoginResponse {PreferedLanguage= user.PreferedLanguage, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardLoginSuccess });
                    //return AppUtils.SignIn(user, roles);
                }
                if (result.RequiresTwoFactor)
                {
                    return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.FactorRequired, ErrorCode = enErrorCode.Status4060VerifyMethod });

                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return BadRequest(new LoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginLockOut, ErrorCode = enErrorCode.Status423Locked });
                }
                else
                {
                    return BadRequest(new LoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginWithOtpInvalidAttempt, ErrorCode = enErrorCode.Status423Locked });
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Date: " + _basePage.UTC_To_IST() + ",\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nControllername=" + this.GetType().Name, LogLevel.Error);
                //return BadRequest();
                return BadRequest(new LoginResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }

        }

        #region Standerd Login

        //[HttpPost("BackOfficeStandardLoginV1")]
        //[AllowAnonymous]
        //public async Task<IActionResult> BackOfficeStandardLoginV1(StandardLoginViewModel model)
        //{
        //    try
        //    {
        //        if (_configuration["IPByPassLogin"].ToString() == "False")
        //        {
        //            if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
        //            {
        //                model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
        //            }
        //            else
        //            {
        //                return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
        //            }
        //        }
        //        ////// Ip Address Validate or not
        //        var Ipwisedata = await _userService.GetIPWiseData(model.IPAddress);
        //        //if (!string.IsNullOrEmpty(Ipwisedata.CountryCode) && Ipwisedata.CountryCode == "fail")
        //        if (Ipwisedata.IsValid == false)
        //            return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

        //        var Taskresult = _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);
        //        var user = await _userManager.FindByNameAsync(model.Username);

        //        if (user == null || string.IsNullOrEmpty(user?.UserName)) //Check bizUser  table in username  Exist or not
        //            return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

        //        if (!user.EmailConfirmed) //  for verify pending 
        //            return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpTempUserEmailVerifyPending, ErrorCode = enErrorCode.Status4036VerifyPending });

        //        if (!user.IsEnabled) //  for check user Active
        //            return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });

        //        var result = await Taskresult;

        //        // khushali 22-03-2019 for Backoffice Admin Role Check 
        //        //var roles = await _userManager.GetRolesAsync(user);

        //        //if(roles == null)
        //        //    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "This Admin User not found.", ErrorCode = enErrorCode.Status14060AdminUserNotAvailable });

        //        //var MatchingRoles = roles.Where(o => o.ToLower() == "admin").FirstOrDefault();

        //        //if (MatchingRoles == null)
        //        //    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "This Admin User not found.", ErrorCode = enErrorCode.Status14060AdminUserNotAvailable });

        //        // khushali 05-04-2019 for Backoffice Admin IsCreatedByAdmin bit check
        //        if (user.IsCreatedByAdmin == 0)
        //            return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "This Admin User not found.", ErrorCode = enErrorCode.Status14060AdminUserNotAvailable });


        //        if (result.Succeeded || (result.IsLockedOut && user.LockoutEnd <= DateTime.UtcNow || user.LockoutEnd == null))
        //        {
        //            //_ApplicationUser = user; // remove this line not used in this method
        //            HttpContext.Items["UserId"] = user.Id;
        //            if (user.IsEnabled) // check user is enable
        //            {
        //                var UserOTPPolicy = _UserPasswordPolicyMaster.GetUserPasswordPolicyConfiguration(user.Id);   /// Get user wise password Configuration if User not found then get default configuration
        //                if (UserOTPPolicy != null)
        //                {
        //                    bool CheckOTPExpiration = _UserPasswordPolicyMaster.CheckPasswordPolicyExpiration(user.Id, UserOTPPolicy.PwdExpiretime, passwordPolicy.PasswordExpire);  /// Check  password Expiration
        //                    if (CheckOTPExpiration != true)
        //                    {
        //                        CheckOTPExpiration = _UserPasswordPolicyMaster.CheckPasswordDefaultPolicyExpiration(user.Id, UserOTPPolicy.PwdExpiretime, passwordPolicy.DefaultPolicy);  /// this Condition check only one when user signup and conform the mail and then process to login that time check this policy
        //                        if (CheckOTPExpiration != true)
        //                            return Ok(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PasswordExpiration, ErrorCode = enErrorCode.Status4158PasswordExpiration });
        //                    }
        //                }


        //                //var roles = await _userManager.GetRolesAsync(user);
        //                bool flag = _IIPRange.IPAddressinrange(model.IPAddress, user.Id);

        //                if (!flag)
        //                {
        //                    return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IPNotValid, ErrorCode = enErrorCode.Status4152IPNotValid });
        //                }

        //                //++++++++++++++++++++++++++++++++++++++//
        //                // AddIpHistory and AddLoginHistory is comment by Pratik :: 12-3-2019
        //                bool IPExist = _iipHistory.IsIpHistoryExist(user.Id, model.IPAddress);
        //                if (!IPExist)
        //                {
        //                    //// added by nirav savariya for ip history user wise on 11-03-2018
        //                    var IpHistory = new IpHistoryViewModel()
        //                    {
        //                        IpAddress = model.IPAddress,
        //                        Location = Ipwisedata.Location,
        //                        UserId = user.Id,
        //                    };
        //                    _iipHistory.AddIpHistory(IpHistory);
        //                }

        //                var LoginhistoryViewModel = new LoginhistoryViewModel()
        //                {
        //                    UserId = user.Id,
        //                    IpAddress = model.IPAddress,
        //                    Device = model.DeviceId,
        //                    Location = Ipwisedata.Location
        //                };
        //                _loginHistory.AddLoginHistory(LoginhistoryViewModel);


        //                //bool checkvalidip = _iipAddressService.CheckValidIpaddress(user.Id, model.IPAddress);
        //                bool checkvaliddevice = _IdeviceIdService.CheckValidDevice(user.Id, model.DeviceId);
        //                if (checkvaliddevice)
        //                {

        //                    //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
        //                    // khushali 30-01-2019 for Common Template Method call 
        //                    TemplateMasterData TemplateData = new TemplateMasterData();
        //                    CommunicationParamater communicationParamater = new CommunicationParamater();
        //                    SendEmailRequest request = new SendEmailRequest();
        //                    communicationParamater.Param1 = user.UserName; //Username
        //                    communicationParamater.Param2 = model.DeviceId; //Devicename
        //                    communicationParamater.Param3 = DateTime.UtcNow.ToString(); //Time
        //                    communicationParamater.Param4 = model.IPAddress; //Ipaddress
        //                    communicationParamater.Param5 = Ipwisedata.Location; //Location
        //                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SuccessfulLogin, communicationParamater, enCommunicationServiceType.Email).Result;
        //                    if (TemplateData != null)
        //                    {
        //                        if (TemplateData.IsOnOff == 1)
        //                        {
        //                            request.Recepient = user.Email;
        //                            request.Body = TemplateData.Content;
        //                            request.Subject = TemplateData.AdditionalInfo;
        //                            _pushNotificationsQueue.Enqueue(request);
        //                        }
        //                    }
        //                    // added by nirav savariya for Thememode for user base showing mode for Thememode=0(false) means Day mode and Thememode=1(true) on 1-10-2019 03-20 PM
        //                    return Ok(new StandardSuccessLoginResponse {PreferedLanguage=user.PreferedLanguage, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardLoginSuccess, Thememode = user.Thememode });
        //                }
        //                else
        //                {
        //                    Guid AuthorizeToken = Guid.NewGuid();
        //                    string[] DeviceDetails = null;
        //                    if (model.DeviceId.Contains('|'))
        //                        DeviceDetails = model.DeviceId.Split('|');

        //                    DeviceMasterViewModelV1 AuthorizeDeviceModel = new DeviceMasterViewModelV1();
        //                    AuthorizeDeviceModel.UserId = user.Id;
        //                    AuthorizeDeviceModel.Location = Ipwisedata.Location;
        //                    AuthorizeDeviceModel.IPAddress = model.IPAddress;
        //                    AuthorizeDeviceModel.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
        //                    AuthorizeDeviceModel.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
        //                    AuthorizeDeviceModel.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
        //                    DateTime currenttime = DateTime.UtcNow;
        //                    int min = Convert.ToInt32(_configuration["DefaultValidateAuthorizedDevice"]);
        //                    AuthorizeDeviceModel.Expirytime = currenttime.AddMinutes(min);
        //                    AuthorizeDeviceModel.AuthorizeToken = AuthorizeToken;
        //                    AuthorizeDeviceModel.Status = 0;
        //                    var id = _IdeviceIdService.AddDeviceProcessV1(AuthorizeDeviceModel);

        //                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //                    //Rushabh 16-08-2019
        //                    //AuthorizeDeviceViewModel authorizeDeviceView = new AuthorizeDeviceViewModel();
        //                    //authorizeDeviceView.UserId = user.Id;
        //                    //authorizeDeviceView.Location = Ipwisedata.Location;
        //                    //authorizeDeviceView.IPAddress = model.IPAddress;
        //                    //authorizeDeviceView.DeviceName = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
        //                    //authorizeDeviceView.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
        //                    //authorizeDeviceView.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
        //                    //authorizeDeviceView.CurrentTime = DateTime.UtcNow;
        //                    //authorizeDeviceView.Expirytime = DateTime.UtcNow + TimeSpan.FromHours(Convert.ToInt32(_configuration["DefaultValidateAuthorizedDevice"]));


        //                    //byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
        //                    //string UserDetails = JsonConvert.SerializeObject(authorizeDeviceView);
        //                    //string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);
        //                    //byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
        //                    //string ctokenlink = _configuration["AuthorizedDeviceURL"].ToString() + Convert.ToBase64String(plainTextBytes);

        //                    //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
        //                    // khushali 30-01-2019 for Common Template Method call 
        //                    TemplateMasterData TemplateData = new TemplateMasterData();
        //                    CommunicationParamater communicationParamater = new CommunicationParamater();
        //                    SendEmailRequest request = new SendEmailRequest();
        //                    communicationParamater.Param1 = Ipwisedata.Location; //Location
        //                    communicationParamater.Param2 = model.IPAddress; //IPAddress
        //                    communicationParamater.Param3 = DeviceDetails != null ? DeviceDetails[0] : string.Empty; //Device
        //                    //communicationParamater.Param4 = ctokenlink; //Link

        //                    if (AuthorizeDeviceModel.DeviceOS.ToUpper().Contains("ANDROID") || AuthorizeDeviceModel.DeviceOS.ToUpper().Contains("IOS"))// add mansi 15-07-2019
        //                    {
        //                        string Authcode = AuthorizeDeviceModel.AuthorizeToken.ToString();
        //                        //var rev = Authcode.Reverse();
        //                        char[] array = Authcode.ToCharArray();
        //                        Array.Reverse(array);
        //                        string rev = new string(array);
        //                        var enccode = Convert.ToBase64String(Encoding.UTF8.GetBytes(rev.ToString()));
        //                        if (!string.IsNullOrEmpty(_configuration["IsAppAuth"]))
        //                        {
        //                            // Commnet below logic as not tested and not working. -Nishit Jani on A 2019-07-27 7:15 PM
        //                            if (_configuration["IsAppAuth"].ToString() == "True")
        //                                communicationParamater.Param4 = _configuration["AuthorizedDeviceURLAndroid"].ToString() + WebUtility.UrlEncode(code) + ";" + WebUtility.UrlEncode(enccode);
        //                            else
        //                                communicationParamater.Param4 = _configuration["AuthorizedDeviceURLAndroid"].ToString() + WebUtility.UrlEncode(code);
        //                        }
        //                        else
        //                            communicationParamater.Param4 = _configuration["AuthorizedDeviceURLAndroid"].ToString() + WebUtility.UrlEncode(code);
        //                    }
        //                    else
        //                    {
        //                        communicationParamater.Param4 = _configuration["AuthorizedDeviceURL"].ToString() + WebUtility.UrlEncode(code); //komal 18-06-2019 set token 

        //                    }
        //                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.AuthorizedNewDevice, communicationParamater, enCommunicationServiceType.Email).Result;
        //                    if (TemplateData != null)
        //                    {
        //                        if (TemplateData.IsOnOff == 1)
        //                        {
        //                            request.Recepient = user.Email;
        //                            request.Body = TemplateData.Content;
        //                            request.Subject = TemplateData.AdditionalInfo;
        //                            _pushNotificationsQueue.Enqueue(request);
        //                        }
        //                    }
        //                    var UnAuthorizedDeviceMessage = EnResponseMessage.UnAuthorizedDevice.Replace("###SiteName###", _configuration["SiteNameForWelcomeMsg"]);
        //                    //return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Success, ReturnMsg = UnAuthorizedDeviceMessage, ErrorCode = enErrorCode.Status4137UnAuthorizedDevice });
        //                    return Ok(new StandardLoginAuthorizeFailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = UnAuthorizedDeviceMessage, ErrorCode = enErrorCode.Status4137UnAuthorizedDevice, AllowAuthorizeToken = AuthorizeToken.ToString() });
        //                }
        //            }
        //            else
        //                return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });
        //        }
        //        if (result.RequiresTwoFactor)
        //        {
        //            HttpContext.Items["UserId"] = user.Id; // store user id on activity data
        //            if (user.IsEnabled)
        //            {
        //                //++++++++++++++++++++++++++++++++++++++//
        //                // AddIpHistory and AddLoginHistory is comment by Pratik :: 12-3-2019
        //                //bool IPExist = _iipHistory.IsIpHistoryExist(user.Id, model.IPAddress);
        //                //if (!IPExist)
        //                //{
        //                //    //// added by nirav savariya for ip history user wise on 11-03-2018
        //                //    var IpHistory = new IpHistoryViewModel()
        //                //    {
        //                //        IpAddress = model.IPAddress,
        //                //        Location = Ipwisedata.Location,
        //                //        UserId = user.Id,
        //                //    };
        //                //    _iipHistory.AddIpHistory(IpHistory);
        //                //}
        //                //var LoginhistoryViewModel = new LoginhistoryViewModel()
        //                //{
        //                //    UserId = user.Id,
        //                //    IpAddress = model.IPAddress,
        //                //    Device = model.DeviceId,
        //                //    Location = Ipwisedata.Location
        //                //};
        //                //_loginHistory.AddLoginHistory(LoginhistoryViewModel);

        //                string TwoFAToken = _userKeyMasterService.Get2FACustomToken(user.Id);
        //                //// End 2FA in Custome token Create 
        //                return Ok(new StandardLogin2FAResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.FactorRequired, ErrorCode = enErrorCode.Status4060VerifyMethod, TwoFAToken = TwoFAToken });
        //            }
        //            else
        //                return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });
        //        }
        //        if (result.IsLockedOut)
        //            return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginLockOut, ErrorCode = enErrorCode.Status423Locked });
        //        if (result.ToString() == "Failed")
        //            return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.Status4032LoginFailed });
        //        //////////////////// check TempUser  table in username Exist and Verify Pending // remove code for now tempuser table not consider this condition on 28-01-2019
        //        //bool IsSignUserName = _tempUserRegisterService.GetUserName(model.Username);
        //        //if (!IsSignUserName)
        //        //{
        //        //    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpTempUserNameVerifyPending, ErrorCode = enErrorCode.Status4036VerifyPending });
        //        //}

        //        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.Status4032LoginFailed });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new StandardLoginResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}



        //Rushabh 08-07-2020 Commented Below StandardLoginV1 Method And Added Previous Version Method Which Is Used Before 28-04-2020
        [HttpPost("BackOfficeStandardLoginV1")]
        [AllowAnonymous]
        public async Task<IActionResult> BackOfficeStandardLoginV1(StandardLoginViewModel model)
        {
            try
            {
                //Helpers.HelperForLog.WriteLogIntoFile("StandardLoginV1", "SigninController ", Environment.NewLine + "Request: " + " Username = " + model.Username + "  DeviceId = " + model.DeviceId + "  HostName = " + model.HostName + "  IPAddress = " + model.IPAddress + " Mode = " + model.Mode);//Write Requset log by mansi 06-11-2019
                if (_configuration["IPByPassLogin"].ToString() == "False")
                {
                    //Core.Helpers.HelperForLog.WriteLogIntoFile("StandardLoginV1", "StandardLoginV1", HttpContext.Connection.RemoteIpAddress.ToString());
                    if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
                    {
                        model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                ////// Ip Address Validate or not
                var Ipwisedata = await _userService.GetIPWiseData(model.IPAddress);
                //if (!string.IsNullOrEmpty(Ipwisedata.CountryCode) && Ipwisedata.CountryCode == "fail")
                if (Ipwisedata == null || Ipwisedata.IsValid == false)
                {
                    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                }

                //Rushabh 05-09-2019 As Above condition will return same error so merged this condition with that.
                //if (Ipwisedata.IsValid == false)
                //{
                //    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                //}

                var Taskresult = _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: true); //komal 18-06-2019 set lockoutOnFailure-true
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user == null || string.IsNullOrEmpty(user?.UserName)) //Check bizUser  table in username  Exist or not
                {
                    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });
                }
                if (!user.EmailConfirmed) //  for verify pending
                {
                    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpTempUserEmailVerifyPending, ErrorCode = enErrorCode.Status4036VerifyPending });
                }
                if (_configuration["SignUPWithMobile"].ToString() == "true")
                {
                    if (!user.PhoneNumberConfirmed) //  for verify pending // khushali 15-11-2019 check phone number verified
                    {
                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpTempUserEmailVerifyPending, ErrorCode = enErrorCode.Status4036VerifyPending });
                    }
                }
                if (!user.IsEnabled) //  for check user Active
                {
                    return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });
                }
                //Rushabh 21-08-2019 Added Condition For Blocked User As Per The Bug Report Of Salt Pepper 
                //if (user.Status == Convert.ToInt16(EnMyAccountUserStatus.Blocked))
                if (user.Status == Convert.ToInt16(EnMyAccountUserStatus.UnConfirmed))//changes in condition by mansi-20-09-2019//vsolanki 2019-12-12 remove status=0 condition -confirm with khushali
                {
                    return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserIsBlocked, ErrorCode = enErrorCode.Status4168UserNotActive });
                }
                //var roles = await _userManager.GetRolesAsync(user);
                bool flag = _IIPRange.IPAddressinrange(model.IPAddress, user.Id);

                if (!flag)
                {
                    return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IPNotValid, ErrorCode = enErrorCode.Status4152IPNotValid });
                }

                //++++++++++++++++++++++++++++++++++++++//
                // AddIpHistory and AddLoginHistory is comment by Pratik :: 12-3-2019
                bool IPExist = _iipHistory.IsIpHistoryExist(user.Id, model.IPAddress);
                if (!IPExist)
                {
                    //// added by nirav savariya for ip history user wise on 11-03-2018
                    var IpHistory = new IpHistoryViewModel()
                    {
                        IpAddress = model.IPAddress,
                        Location = Ipwisedata.Location,
                        UserId = user.Id,
                    };
                    _iipHistory.AddIpHistory(IpHistory);
                }
                var LoginhistoryViewModel = new LoginhistoryViewModel()
                {
                    UserId = user.Id,
                    IpAddress = model.IPAddress,
                    Device = model.DeviceId,
                    Location = Ipwisedata.Location
                };
                _loginHistory.AddLoginHistory(LoginhistoryViewModel);

                //komal 19-06-2019 validate remote ip
                bool checkvaliddevice = _IdeviceIdService.CheckValidDeviceV1(user.Id, model.DeviceId, model.IPAddress);
                var result = await Taskresult;
                //Rushabh 20-08-2019 As Device Authorization Error Message Should Be Return Only After User Validation
                #region Device Authorization Code
                if (user.IsDeviceAuthEnable == 1 && (result.Succeeded && (!result.IsLockedOut && user.LockoutEnd <= DateTime.UtcNow || user.LockoutEnd == null) || result.RequiresTwoFactor))////Mansi 03-10-2019 add DeviceAuth bit for front user preference
                {
                    if (!checkvaliddevice)
                    {
                        Guid AuthorizeToken = Guid.NewGuid();
                        string[] DeviceDetails = null;
                        if (model.DeviceId.Contains('|'))
                            DeviceDetails = model.DeviceId.Split('|');

                        //komal 18-06-2019 add to DEviceMaster with status=0
                        DeviceMasterViewModelV1 AuthorizeDeviceModel = new DeviceMasterViewModelV1();
                        AuthorizeDeviceModel.UserId = user.Id;
                        AuthorizeDeviceModel.Location = Ipwisedata.Location;
                        AuthorizeDeviceModel.IPAddress = model.IPAddress;
                        AuthorizeDeviceModel.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                        AuthorizeDeviceModel.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                        AuthorizeDeviceModel.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                        DateTime currenttime = DateTime.UtcNow;
                        int min = Convert.ToInt32(_configuration["DefaultValidateAuthorizedDevice"]);
                        AuthorizeDeviceModel.Expirytime = currenttime.AddMinutes(min);
                        AuthorizeDeviceModel.AuthorizeToken = AuthorizeToken;
                        AuthorizeDeviceModel.Status = 0;
                        var id = _IdeviceIdService.AddDeviceProcessV1(AuthorizeDeviceModel);

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                        // khushali 30-01-2019 for Common Template Method call 
                        TemplateMasterData TemplateData = new TemplateMasterData();
                        CommunicationParamater communicationParamater = new CommunicationParamater();
                        SendEmailRequest request = new SendEmailRequest();
                        communicationParamater.Param1 = Ipwisedata.Location; //Location
                        communicationParamater.Param2 = model.IPAddress; //IPAddress
                        communicationParamater.Param3 = DeviceDetails != null ? DeviceDetails[0] : string.Empty; //Device
                                                                                                                 //communicationParamater.Param4 = ctokenlink; //Link
                        if (AuthorizeDeviceModel.DeviceOS.ToUpper().Contains("ANDROID") || AuthorizeDeviceModel.DeviceOS.ToUpper().Contains("IOS"))// add mansi 15-07-2019
                        {
                            string Authcode = AuthorizeDeviceModel.AuthorizeToken.ToString();
                            //var rev = Authcode.Reverse();
                            char[] array = Authcode.ToCharArray();
                            Array.Reverse(array);
                            string rev = new string(array);
                            var enccode = Convert.ToBase64String(Encoding.UTF8.GetBytes(rev.ToString()));
                            if (!string.IsNullOrEmpty(_configuration["IsAppAuth"]))
                            {
                                // Commnet below logic as not tested and not working. -Nishit Jani on A 2019-07-27 7:15 PM
                                if (_configuration["IsAppAuth"].ToString() == "True")
                                    communicationParamater.Param4 = _configuration["AuthorizedDeviceURLAndroid"].ToString() + WebUtility.UrlEncode(code) + ";" + WebUtility.UrlEncode(enccode);
                                else
                                    communicationParamater.Param4 = _configuration["AuthorizedDeviceURLAndroid"].ToString() + WebUtility.UrlEncode(code);
                            }
                            else
                                communicationParamater.Param4 = _configuration["AuthorizedDeviceURLAndroid"].ToString() + WebUtility.UrlEncode(code);
                        }
                        else
                        {
                            communicationParamater.Param4 = _configuration["AuthorizedDeviceURL"].ToString() + WebUtility.UrlEncode(code); //komal 18-06-2019 set token 

                        }
                        TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.AuthorizedNewDevice, communicationParamater, enCommunicationServiceType.Email).Result;
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
                        var UnAuthorizedDeviceMessage = EnResponseMessage.UnAuthorizedDevice.Replace("###SiteName###", _configuration["SiteNameForWelcomeMsg"]);
                        //return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Success, ReturnMsg = UnAuthorizedDeviceMessage, ErrorCode = enErrorCode.Status4137UnAuthorizedDevice });
                        return Ok(new StandardLoginAuthorizeFailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = UnAuthorizedDeviceMessage, ErrorCode = enErrorCode.Status4137UnAuthorizedDevice, AllowAuthorizeToken = AuthorizeToken.ToString() });
                    }
                }
                #endregion

                if (result.Succeeded && (!result.IsLockedOut && user.LockoutEnd <= DateTime.UtcNow || user.LockoutEnd == null))
                {
                    //_ApplicationUser = user; // remove this line not used in this method
                    HttpContext.Items["UserId"] = user.Id;
                    if (user.IsEnabled) // check user is enable
                    {
                        var UserOTPPolicy = _UserPasswordPolicyMaster.GetUserPasswordPolicyConfiguration(user.Id);   /// Get user wise password Configuration if User not found then get default configuration
                        if (UserOTPPolicy != null)
                        {
                            bool CheckOTPExpiration = _UserPasswordPolicyMaster.CheckPasswordPolicyExpiration(user.Id, UserOTPPolicy.PwdExpiretime, passwordPolicy.PasswordExpire);  /// Check  password Expiration
                            if (CheckOTPExpiration != true)
                            {
                                CheckOTPExpiration = _UserPasswordPolicyMaster.CheckPasswordDefaultPolicyExpiration(user.Id, UserOTPPolicy.PwdExpiretime, passwordPolicy.DefaultPolicy);  /// this Condition check only one when user signup and conform the mail and then process to login that time check this policy
                                if (CheckOTPExpiration != true)
                                {
                                    return Ok(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PasswordExpiration, ErrorCode = enErrorCode.Status4158PasswordExpiration });
                                }
                            }
                        }
                        if (checkvaliddevice && IPExist && user.IsDeviceAuthEnable == 1)//Mansi 03-10-2019 add DeviceAuth bit for front user preference
                        {
                            //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                            // khushali 30-01-2019 for Common Template Method call 
                            TemplateMasterData TemplateData = new TemplateMasterData();
                            CommunicationParamater communicationParamater = new CommunicationParamater();
                            SendEmailRequest request = new SendEmailRequest();
                            communicationParamater.Param1 = user.UserName; //Username
                            communicationParamater.Param2 = model.DeviceId; //Devicename
                            communicationParamater.Param3 = DateTime.UtcNow.ToString(); //Time
                            communicationParamater.Param4 = model.IPAddress; //Ipaddress
                            communicationParamater.Param5 = Ipwisedata.Location; //Location
                            TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SuccessfulLogin, communicationParamater, enCommunicationServiceType.Email).Result;
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
                            // added by nirav savariya for Thememode for user base showing mode for Thememode=0(false) means Day mode and Thememode=1(true) on 1-10-2019 03-20 PM
                            return Ok(new StandardSuccessLoginResponse { PreferedLanguage = user.PreferedLanguage, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardLoginSuccess, Thememode = user.Thememode });
                        }
                        else
                        {
                            return Ok(new StandardSuccessLoginResponse { PreferedLanguage = user.PreferedLanguage, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardLoginSuccess, Thememode = user.Thememode });
                        }
                    }
                    else
                    {
                        return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    //khushali 07-11-2019 In two factor login , manually access failed count set 0 after successfully logged in user
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = null;
                    await _userManager.UpdateAsync(user);
                    HttpContext.Items["UserId"] = user.Id; // store user id on activity data
                    if (user.IsEnabled)
                    {
                        var AllowToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        string TwoFAToken = _userKeyMasterService.Get2FACustomToken(user.Id);
                        //// End 2FA in Custome token Create 
                        return Ok(new StandardLogin2FAResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.FactorRequired, ErrorCode = enErrorCode.Status4060VerifyMethod, TwoFAToken = TwoFAToken, AllowToken = WebUtility.UrlEncode(AllowToken) });
                    }
                    else
                    {
                        return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });
                    }
                }
                if (result.IsLockedOut)
                {
                    return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginLockOut, ErrorCode = enErrorCode.Status423Locked });
                }

                if (result.ToString() == "Failed")
                {
                    return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.Status4032LoginFailed });
                }
                return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.Status4032LoginFailed });
            }
            catch (Exception ex)
            {
                //Core.Helpers.HelperForLog.WriteErrorLog("StandardLoginV1", "SignIn", ex);
                return BadRequest(new StandardLoginResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = "Internal Server Error", ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #region Rushabh 08-07-2020 Commented Below Code And Added New Code

        //[HttpPost("BackOfficeStandardLoginV1")]
        //[AllowAnonymous]
        //public async Task<IActionResult> BackOfficeStandardLoginV1(StandardLoginViewModel model)
        //{
        //    try
        //    {
        //        Worldex.Core.Helpers.HelperForLog.WriteLogIntoFile("StandardLoginV1", "SigninController ", Environment.NewLine + "Request: " + " Username = " + model.Username + "  DeviceId = " + model.DeviceId + "  HostName = " + model.HostName + "  IPAddress = " + model.IPAddress + " Mode = " + model.Mode);//Write Requset log by mansi 06-11-2019

        //        if (_configuration["IPByPassLogin"].ToString() == "False")
        //        {
        //            if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
        //            {
        //                model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
        //            }
        //            else
        //            {
        //                return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
        //            }
        //        }
        //        ////// Ip Address Validate or not
        //        var Ipwisedata = await _userService.GetIPWiseData(model.IPAddress);
        //        //if (!string.IsNullOrEmpty(Ipwisedata.CountryCode) && Ipwisedata.CountryCode == "fail")
        //        if (Ipwisedata.IsValid == false)
        //            return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

        //        var Taskresult = _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);
        //        var user = await _userManager.FindByNameAsync(model.Username);

        //        if (user == null || string.IsNullOrEmpty(user?.UserName)) //Check bizUser  table in username  Exist or not
        //            return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

        //        if (!user.EmailConfirmed) //  for verify pending 
        //            return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpTempUserEmailVerifyPending, ErrorCode = enErrorCode.Status4036VerifyPending });

        //        if (!user.IsEnabled) //  for check user Active
        //            return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });
                
        //        //Rushabh 21-08-2019 Added Condition For Blocked User As Per The Bug Report Of Salt Pepper 
        //        if (user.Status == Convert.ToInt16(EnMyAccountUserStatus.Blocked))
        //            return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserIsBlocked, ErrorCode = enErrorCode.Status4168UserNotActive });


        //        var result = await Taskresult;

        //        // khushali 22-03-2019 for Backoffice Admin Role Check 
        //        //var roles = await _userManager.GetRolesAsync(user);

        //        //if(roles == null)
        //        //    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "This Admin User not found.", ErrorCode = enErrorCode.Status14060AdminUserNotAvailable });

        //        //var MatchingRoles = roles.Where(o => o.ToLower() == "admin").FirstOrDefault();

        //        //if (MatchingRoles == null)
        //        //    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "This Admin User not found.", ErrorCode = enErrorCode.Status14060AdminUserNotAvailable });

        //        // khushali 05-04-2019 for Backoffice Admin IsCreatedByAdmin bit check
        //        if (user.IsCreatedByAdmin == 0)
        //            return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "This Admin User not found.", ErrorCode = enErrorCode.Status14060AdminUserNotAvailable });


        //        if (result.Succeeded || (result.IsLockedOut && user.LockoutEnd <= DateTime.UtcNow || user.LockoutEnd == null))
        //        {
        //            //_ApplicationUser = user; // remove this line not used in this method
        //            HttpContext.Items["UserId"] = user.Id;
        //            if (user.IsEnabled) // check user is enable
        //            {
        //                var UserOTPPolicy = _UserPasswordPolicyMaster.GetUserPasswordPolicyConfiguration(user.Id);   /// Get user wise password Configuration if User not found then get default configuration
        //                if (UserOTPPolicy != null)
        //                {
        //                    bool CheckOTPExpiration = _UserPasswordPolicyMaster.CheckPasswordPolicyExpiration(user.Id, UserOTPPolicy.PwdExpiretime, passwordPolicy.PasswordExpire);  /// Check  password Expiration
        //                    if (CheckOTPExpiration != true)
        //                    {
        //                        CheckOTPExpiration = _UserPasswordPolicyMaster.CheckPasswordDefaultPolicyExpiration(user.Id, UserOTPPolicy.PwdExpiretime, passwordPolicy.DefaultPolicy);  /// this Condition check only one when user signup and conform the mail and then process to login that time check this policy
        //                        if (CheckOTPExpiration != true)
        //                            return Ok(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PasswordExpiration, ErrorCode = enErrorCode.Status4158PasswordExpiration });
        //                    }
        //                }


        //                //var roles = await _userManager.GetRolesAsync(user);
        //                bool flag = _IIPRange.IPAddressinrange(model.IPAddress, user.Id);

        //                if (!flag)
        //                {
        //                    return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IPNotValid, ErrorCode = enErrorCode.Status4152IPNotValid });
        //                }

        //                //++++++++++++++++++++++++++++++++++++++//
        //                // AddIpHistory and AddLoginHistory is comment by Pratik :: 12-3-2019
        //                bool IPExist = _iipHistory.IsIpHistoryExist(user.Id, model.IPAddress);
        //                if (!IPExist)
        //                {
        //                    //// added by nirav savariya for ip history user wise on 11-03-2018
        //                    var IpHistory = new IpHistoryViewModel()
        //                    {
        //                        IpAddress = model.IPAddress,
        //                        Location = Ipwisedata.Location,
        //                        UserId = user.Id,
        //                    };
        //                    _iipHistory.AddIpHistory(IpHistory);
        //                }

        //                var LoginhistoryViewModel = new LoginhistoryViewModel()
        //                {
        //                    UserId = user.Id,
        //                    IpAddress = model.IPAddress,
        //                    Device = model.DeviceId,
        //                    Location = Ipwisedata.Location
        //                };
        //                _loginHistory.AddLoginHistory(LoginhistoryViewModel);


        //                Guid AuthorizeToken = Guid.NewGuid();
        //                string[] DeviceDetails = null;
        //                if (model.DeviceId.Contains('|'))
        //                {
        //                    DeviceDetails = model.DeviceId.Split('|');
        //                }

        //                //bool checkvalidip = _iipAddressService.CheckValidIpaddress(user.Id, model.IPAddress);
        //                bool checkvaliddevice = _IdeviceIdService.CheckValidDevice(user.Id, model.DeviceId);

        //                //Rushabh 28-04-2020 Added Separate Code For IP Whitelisting 
        //                bool IsIPWhiteListed = _iipHistory.IsIPWhiteListed(user.Id, model.IPAddress);
        //                //Rushabh 28-04-2020 Added Separate Code For IP Whitelisting 
        //                if (!IsIPWhiteListed)
        //                {
        //                    AuthorizeIPViewModel AuthorizeIPModel = new AuthorizeIPViewModel();
        //                    AuthorizeIPModel.UserId = user.Id;
        //                    AuthorizeIPModel.Location = Ipwisedata.Location;
        //                    AuthorizeIPModel.IPAddress = model.IPAddress;
        //                    DateTime currenttime = DateTime.UtcNow;
        //                    int min = Convert.ToInt32(_configuration["DefaultValidateAuthorizedDevice"]);
        //                    AuthorizeIPModel.Expirytime = currenttime.AddMinutes(min);
        //                    AuthorizeIPModel.Status = 0;
        //                    var id = await _iipHistory.AddIPAddressProcess(user.Id, model.IPAddress, AuthorizeToken);

        //                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //                    // khushali 30-01-2019 for Common Template Method call 
        //                    TemplateMasterData TemplateData = new TemplateMasterData();
        //                    CommunicationParamater communicationParamater = new CommunicationParamater();
        //                    SendEmailRequest request = new SendEmailRequest();
        //                    communicationParamater.Param1 = Ipwisedata.Location; //Location
        //                    communicationParamater.Param2 = model.IPAddress; //IPAddress
        //                    communicationParamater.Param3 = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
        //                    if (DeviceDetails[1].ToUpper().Contains("ANDROID") || DeviceDetails[1].ToUpper().Contains("IOS"))
        //                    {
        //                        string Authcode = AuthorizeToken.ToString();
        //                        char[] array = Authcode.ToCharArray();
        //                        Array.Reverse(array);
        //                        string rev = new string(array);
        //                        var enccode = Convert.ToBase64String(Encoding.UTF8.GetBytes(rev.ToString()));
        //                        if (!string.IsNullOrEmpty(_configuration["IsAppAuth"]))
        //                        {
        //                            // Commnet below logic as not tested and not working. -Nishit Jani on A 2019-07-27 7:15 PM
        //                            if (_configuration["IsAppAuth"].ToString() == "True")
        //                            {
        //                                communicationParamater.Param4 = _configuration["AuthorizedIPURLAndroid"].ToString() + WebUtility.UrlEncode(code) + ";" + WebUtility.UrlEncode(enccode);
        //                            }
        //                            else
        //                            {
        //                                communicationParamater.Param4 = _configuration["AuthorizedIPURLAndroid"].ToString() + WebUtility.UrlEncode(code);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            communicationParamater.Param4 = _configuration["AuthorizedIPURLAndroid"].ToString() + WebUtility.UrlEncode(code);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        communicationParamater.Param4 = _configuration["AuthorizedIPURL"].ToString() + WebUtility.UrlEncode(code);
        //                    }
        //                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.AuthorizeNewIPAddress, communicationParamater, enCommunicationServiceType.Email).Result;
        //                    if (TemplateData != null)
        //                    {
        //                        if (TemplateData.IsOnOff == 1)
        //                        {
        //                            request.Recepient = user.Email;
        //                            request.Body = TemplateData.Content;
        //                            request.Subject = TemplateData.AdditionalInfo;
        //                            _pushNotificationsQueue.Enqueue(request);
        //                        }
        //                    }
        //                    return Ok(new StandardLoginAuthorizeFailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.UnAuthorizedIP, ErrorCode = enErrorCode.Status4137UnAuthorizedDevice, AllowAuthorizeToken = AuthorizeToken.ToString() });
        //                }


        //                if (checkvaliddevice)
        //                {

        //                    //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
        //                    // khushali 30-01-2019 for Common Template Method call 
        //                    TemplateMasterData TemplateData = new TemplateMasterData();
        //                    CommunicationParamater communicationParamater = new CommunicationParamater();
        //                    SendEmailRequest request = new SendEmailRequest();
        //                    communicationParamater.Param1 = user.UserName; //Username
        //                    communicationParamater.Param2 = model.DeviceId; //Devicename
        //                    communicationParamater.Param3 = DateTime.UtcNow.ToString(); //Time
        //                    communicationParamater.Param4 = model.IPAddress; //Ipaddress
        //                    communicationParamater.Param5 = Ipwisedata.Location; //Location
        //                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SuccessfulLogin, communicationParamater, enCommunicationServiceType.Email).Result;
        //                    if (TemplateData != null)
        //                    {
        //                        if (TemplateData.IsOnOff == 1)
        //                        {
        //                            request.Recepient = user.Email;
        //                            request.Body = TemplateData.Content;
        //                            request.Subject = TemplateData.AdditionalInfo;
        //                            _pushNotificationsQueue.Enqueue(request);
        //                        }
        //                    }
        //                    // added by nirav savariya for Thememode for user base showing mode for Thememode=0(false) means Day mode and Thememode=1(true) on 1-10-2019 03-20 PM
        //                    return Ok(new StandardSuccessLoginResponse { PreferedLanguage = user.PreferedLanguage, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardLoginSuccess, Thememode = user.Thememode });
        //                }
        //                else
        //                {
        //                    DeviceMasterViewModelV1 AuthorizeDeviceModel = new DeviceMasterViewModelV1();
        //                    AuthorizeDeviceModel.UserId = user.Id;
        //                    AuthorizeDeviceModel.Location = Ipwisedata.Location;
        //                    AuthorizeDeviceModel.IPAddress = model.IPAddress;
        //                    AuthorizeDeviceModel.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
        //                    AuthorizeDeviceModel.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
        //                    AuthorizeDeviceModel.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
        //                    DateTime currenttime = DateTime.UtcNow;
        //                    int min = Convert.ToInt32(_configuration["DefaultValidateAuthorizedDevice"]);
        //                    AuthorizeDeviceModel.Expirytime = currenttime.AddMinutes(min);
        //                    AuthorizeDeviceModel.AuthorizeToken = AuthorizeToken;
        //                    AuthorizeDeviceModel.Status = 0;
        //                    var id = _IdeviceIdService.AddDeviceProcessV1(AuthorizeDeviceModel);

        //                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //                    //Rushabh 16-08-2019
        //                    //AuthorizeDeviceViewModel authorizeDeviceView = new AuthorizeDeviceViewModel();
        //                    //authorizeDeviceView.UserId = user.Id;
        //                    //authorizeDeviceView.Location = Ipwisedata.Location;
        //                    //authorizeDeviceView.IPAddress = model.IPAddress;
        //                    //authorizeDeviceView.DeviceName = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
        //                    //authorizeDeviceView.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
        //                    //authorizeDeviceView.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
        //                    //authorizeDeviceView.CurrentTime = DateTime.UtcNow;
        //                    //authorizeDeviceView.Expirytime = DateTime.UtcNow + TimeSpan.FromHours(Convert.ToInt32(_configuration["DefaultValidateAuthorizedDevice"]));


        //                    //byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
        //                    //string UserDetails = JsonConvert.SerializeObject(authorizeDeviceView);
        //                    //string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);
        //                    //byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
        //                    //string ctokenlink = _configuration["AuthorizedDeviceURL"].ToString() + Convert.ToBase64String(plainTextBytes);

        //                    //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
        //                    // khushali 30-01-2019 for Common Template Method call 
        //                    TemplateMasterData TemplateData = new TemplateMasterData();
        //                    CommunicationParamater communicationParamater = new CommunicationParamater();
        //                    SendEmailRequest request = new SendEmailRequest();
        //                    communicationParamater.Param1 = Ipwisedata.Location; //Location
        //                    communicationParamater.Param2 = model.IPAddress; //IPAddress
        //                    communicationParamater.Param3 = DeviceDetails != null ? DeviceDetails[0] : string.Empty; //Device
        //                    //communicationParamater.Param4 = ctokenlink; //Link

        //                    if (AuthorizeDeviceModel.DeviceOS.ToUpper().Contains("ANDROID") || AuthorizeDeviceModel.DeviceOS.ToUpper().Contains("IOS"))// add mansi 15-07-2019
        //                    {
        //                        string Authcode = AuthorizeDeviceModel.AuthorizeToken.ToString();
        //                        //var rev = Authcode.Reverse();
        //                        char[] array = Authcode.ToCharArray();
        //                        Array.Reverse(array);
        //                        string rev = new string(array);
        //                        var enccode = Convert.ToBase64String(Encoding.UTF8.GetBytes(rev.ToString()));
        //                        if (!string.IsNullOrEmpty(_configuration["IsAppAuth"]))
        //                        {
        //                            // Commnet below logic as not tested and not working. -Nishit Jani on A 2019-07-27 7:15 PM
        //                            if (_configuration["IsAppAuth"].ToString() == "True")
        //                                communicationParamater.Param4 = _configuration["AuthorizedDeviceURLAndroid"].ToString() + WebUtility.UrlEncode(code) + ";" + WebUtility.UrlEncode(enccode);
        //                            else
        //                                communicationParamater.Param4 = _configuration["AuthorizedDeviceURLAndroid"].ToString() + WebUtility.UrlEncode(code);
        //                        }
        //                        else
        //                            communicationParamater.Param4 = _configuration["AuthorizedDeviceURLAndroid"].ToString() + WebUtility.UrlEncode(code);
        //                    }
        //                    else
        //                    {
        //                        communicationParamater.Param4 = _configuration["AuthorizedDeviceURL"].ToString() + WebUtility.UrlEncode(code); //komal 18-06-2019 set token 

        //                    }
        //                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.AuthorizedNewDevice, communicationParamater, enCommunicationServiceType.Email).Result;
        //                    if (TemplateData != null)
        //                    {
        //                        if (TemplateData.IsOnOff == 1)
        //                        {
        //                            request.Recepient = user.Email;
        //                            request.Body = TemplateData.Content;
        //                            request.Subject = TemplateData.AdditionalInfo;
        //                            _pushNotificationsQueue.Enqueue(request);
        //                        }
        //                    }
        //                    var UnAuthorizedDeviceMessage = EnResponseMessage.UnAuthorizedDevice.Replace("###SiteName###", _configuration["SiteNameForWelcomeMsg"]);
        //                    //return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Success, ReturnMsg = UnAuthorizedDeviceMessage, ErrorCode = enErrorCode.Status4137UnAuthorizedDevice });
        //                    return Ok(new StandardLoginAuthorizeFailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = UnAuthorizedDeviceMessage, ErrorCode = enErrorCode.Status4137UnAuthorizedDevice, AllowAuthorizeToken = AuthorizeToken.ToString() });
        //                }
        //            }
        //            else
        //                return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });
        //        }
        //        if (result.RequiresTwoFactor)
        //        {
        //            HttpContext.Items["UserId"] = user.Id; // store user id on activity data
        //            if (user.IsEnabled)
        //            {
        //                //++++++++++++++++++++++++++++++++++++++//
        //                // AddIpHistory and AddLoginHistory is comment by Pratik :: 12-3-2019
        //                //bool IPExist = _iipHistory.IsIpHistoryExist(user.Id, model.IPAddress);
        //                //if (!IPExist)
        //                //{
        //                //    //// added by nirav savariya for ip history user wise on 11-03-2018
        //                //    var IpHistory = new IpHistoryViewModel()
        //                //    {
        //                //        IpAddress = model.IPAddress,
        //                //        Location = Ipwisedata.Location,
        //                //        UserId = user.Id,
        //                //    };
        //                //    _iipHistory.AddIpHistory(IpHistory);
        //                //}
        //                //var LoginhistoryViewModel = new LoginhistoryViewModel()
        //                //{
        //                //    UserId = user.Id,
        //                //    IpAddress = model.IPAddress,
        //                //    Device = model.DeviceId,
        //                //    Location = Ipwisedata.Location
        //                //};
        //                //_loginHistory.AddLoginHistory(LoginhistoryViewModel);

        //                string TwoFAToken = _userKeyMasterService.Get2FACustomToken(user.Id);
        //                //// End 2FA in Custome token Create 
        //                return Ok(new StandardLogin2FAResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.FactorRequired, ErrorCode = enErrorCode.Status4060VerifyMethod, TwoFAToken = TwoFAToken });
        //            }
        //            else
        //                return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });
        //        }
        //        if (result.IsLockedOut)
        //            return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginLockOut, ErrorCode = enErrorCode.Status423Locked });
        //        if (result.ToString() == "Failed")
        //            return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.Status4032LoginFailed });
        //        //////////////////// check TempUser  table in username Exist and Verify Pending // remove code for now tempuser table not consider this condition on 28-01-2019
        //        //bool IsSignUserName = _tempUserRegisterService.GetUserName(model.Username);
        //        //if (!IsSignUserName)
        //        //{
        //        //    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpTempUserNameVerifyPending, ErrorCode = enErrorCode.Status4036VerifyPending });
        //        //}

        //        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.Status4032LoginFailed });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new StandardLoginResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        #endregion

        #endregion

        #region Login With Email
        [HttpPost("BackOfficeLoginWithEmailV1")]
        [AllowAnonymous]
        public async Task<IActionResult> BackOfficeLoginWithEmailV1(LoginWithEmailViewModel model)
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
                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                ////// Ip Address Validate or not
                string CountryCode = await _userService.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(CountryCode) && CountryCode == "fail")
                    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || string.IsNullOrEmpty(user?.Email))
                    return Ok(new LoginWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginFailEmailNotAvailable, ErrorCode = enErrorCode.Status4086LoginFailEmailNotAvailable });

                if (!user.EmailConfirmed) //  for verify pending 
                    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpTempUserEmailVerifyPending, ErrorCode = enErrorCode.Status4036VerifyPending });

                if (!user.IsEnabled) //  for check user Active
                    return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });

                //// khushali 22-03-2019 for Backoffice Admin Role Check 
                //var roles = await _userManager.GetRolesAsync(user);

                //if (roles == null)
                //    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "This Admin User not found.", ErrorCode = enErrorCode.Status14060AdminUserNotAvailable });

                //var MatchingRoles = roles.Where(o => o.ToLower() == "admin").FirstOrDefault();

                //if (MatchingRoles == null)
                //    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "This Admin User not found.", ErrorCode = enErrorCode.Status14060AdminUserNotAvailable });

                // khushali 05-04-2019 for Backoffice Admin IsCreatedByAdmin bit check
                if (user.IsCreatedByAdmin == 0)
                    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "This Admin User not found.", ErrorCode = enErrorCode.Status14060AdminUserNotAvailable });


                HttpContext.Items["UserId"] = user.Id; // for user id store in activity log 
                var otpData = await _otpMasterService.AddOtp(user.Id, user.Email, "");
                if (otpData != null)
                {
                    CustomtokenViewModel data = new CustomtokenViewModel(); //  for login with mobile and email
                    data.Password = otpData.Password;
                    data.UserId = otpData.UserId;
                    data.EnableStatus = false;
                    await _custompassword.AddPassword(data);
                    return Ok(new LoginWithEmailResponse {ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.LoginWithEmailSuccessSend, Appkey = otpData.appkey });
                }
                else
                    return Ok(new LoginWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginWithOtpDatanotSend, ErrorCode = enErrorCode.Status4085LoginWithOtpDatanotSend });
            }
            catch (Exception ex)
            {
                return BadRequest(new LoginWithEmailResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        

        /// This method are used login with email otp base verification
        [HttpPost("EmailOtpVerification")]
        [AllowAnonymous]
        public async Task<IActionResult> EmailOtpVerification(OTPWithEmailViewModel model)
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
                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                ////// Ip Address Validate or not
                var Ipwisedata = await _userService.GetIPWiseData(model.IPAddress);
                //if (!string.IsNullOrEmpty(Ipwisedata.CountryCode) && Ipwisedata.CountryCode == "fail")
                if (Ipwisedata.IsValid == false)
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                //var logindata = await _userService.FindByEmail(model.Email);
                var checkmail = await _userManager.FindByEmailAsync(model.Email);
                if (!checkmail.IsEnabled) // added by nirav savariya for check user is active on 28-01-2019
                    return BadRequest(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });

                if (checkmail != null)
                {
                    #region This functionality for check the otp expiration validation     Added By pankaj
                    var UserOTPPolicy = _UserPasswordPolicyMaster.GetUserPasswordPolicyConfiguration(checkmail.Id);   /// Get user wise password Configuration if User not found then get default configuration
                    if (UserOTPPolicy != null)
                    {
                        bool CheckOTPExpiration = _UserPasswordPolicyMaster.CheckOTPAndLinkExpiration(checkmail.Id, UserOTPPolicy.OTPExpiryTime, passwordPolicy.LoginWithEmail);  /// Base on parameter this methis check that is valid otp or not
                        if (CheckOTPExpiration != true)
                        {
                            return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpResendOTP, ErrorCode = enErrorCode.Status4076SignUpReSendOTP });
                        }
                    }
                    #endregion


                    bool flag = _IIPRange.IPAddressinrange(model.IPAddress, checkmail.Id);  /// Added by pankaj for check ip in range or not  04-01-2019
                    if (!flag)
                        return BadRequest(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IPNotValid, ErrorCode = enErrorCode.Status4152IPNotValid });
                }
                if (!string.IsNullOrEmpty(checkmail?.Email) && (checkmail.LockoutEnd <= DateTime.UtcNow || checkmail.LockoutEnd == null))
                {
                    if (checkmail?.Id > 0)
                    {
                        var tempotp = await _otpMasterService.GetOtpData(Convert.ToInt32(checkmail.Id),2);
                        if (tempotp != null)
                        {
                            if (tempotp?.ExpirTime >= DateTime.UtcNow)
                            {
                                if (model.OTP == tempotp.OTP)
                                {
                                    _otpMasterService.UpdateOtp(tempotp.Id, 2, "");  /// Added by pankaj for update the opt enable status
                                    if (checkmail.TwoFactorEnabled)
                                    {
                                        var user = await _userManager.FindByEmailAsync(model.Email);

                                        //++++++++++++++++++++++++++++++++++++++//
                                        // AddIpHistory and AddLoginHistory is comment by Pratik :: 12-3-2019
                                        bool IPExist = _iipHistory.IsIpHistoryExist(user.Id, model.IPAddress);
                                        if (!IPExist)
                                        {
                                            //// added by nirav savariya for ip history user wise on 11-03-2018
                                            var IpHistory = new IpHistoryViewModel()
                                            {
                                                IpAddress = model.IPAddress,
                                                Location = Ipwisedata.Location,
                                                UserId = user.Id,
                                            };
                                            _iipHistory.AddIpHistory(IpHistory);
                                        }

                                        var Loginhistory = new LoginhistoryViewModel()
                                        {
                                            UserId = user.Id,
                                            IpAddress = model.IPAddress,
                                            Device = model.DeviceId,
                                            Location = Ipwisedata.Location
                                        };
                                        _loginHistory.AddLoginHistory(Loginhistory);

                                        string TwoFAToken = _userKeyMasterService.Get2FACustomToken(user.Id);
                                        //// End 2FA in Custome token Create 
                                        HttpContext.Items["UserId"] = user.Id;
                                        return Ok(new LoginWithEmail2FAResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.FactorRequired, ErrorCode = enErrorCode.Status4060VerifyMethod, TwoFAToken = TwoFAToken });
                                    }
                                    else
                                    {
                                        //var roles = await _userManager.GetRolesAsync(checkmail);

                                        //++++++++++++++++++++++++++++++++++++++//
                                        // AddIpHistory and AddLoginHistory is comment by Pratik :: 12-3-2019
                                        bool IPExist = _iipHistory.IsIpHistoryExist(checkmail.Id, model.IPAddress);
                                        if (!IPExist)
                                        {
                                            //// added by nirav savariya for ip history user wise on 11-03-2018
                                            var IpHistorydet = new IpHistoryViewModel()
                                            {
                                                IpAddress = model.IPAddress,
                                                Location = Ipwisedata.Location,
                                                UserId = checkmail.Id,
                                            };
                                            _iipHistory.AddIpHistory(IpHistorydet);
                                        }
                                        var LoginhistoryViewModel = new LoginhistoryViewModel()
                                        {
                                            UserId = checkmail.Id,
                                            IpAddress = model.IPAddress,
                                            Device = model.DeviceId,
                                            Location = Ipwisedata.Location
                                        };
                                        _loginHistory.AddLoginHistory(LoginhistoryViewModel);

                                        //bool checkvalidip = _iipAddressService.CheckValidIpaddress(checkmail.Id, model.IPAddress);
                                        bool checkvaliddevice = _IdeviceIdService.CheckValidDevice(checkmail.Id, model.DeviceId);
                                        if (checkvaliddevice)
                                        {
                                            //SendEmailRequest request = new SendEmailRequest();
                                            //request.Recepient = checkmail.Email;
                                            //IQueryable Result = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.SuccessfulLogin), 0);
                                            //foreach (TemplateMasterData Provider in Result)
                                            //{
                                            //    Provider.Content = Provider.Content.Replace("###Username###".ToUpper(), checkmail.UserName);
                                            //    Provider.Content = Provider.Content.Replace("###Devicename###".ToUpper(), model.DeviceId);
                                            //    Provider.Content = Provider.Content.Replace("###Time###".ToUpper(), DateTime.UtcNow.ToString());
                                            //    Provider.Content = Provider.Content.Replace("###Ipaddress###".ToUpper(), model.IPAddress);
                                            //    Provider.Content = Provider.Content.Replace("###Location###".ToUpper(), Location);
                                            //    Provider.Content = Provider.Content.Replace("###year###".ToUpper(), DateTime.Now.Year.ToString());
                                            //    request.Body = Provider.Content;
                                            //    request.Subject = Provider.AdditionalInfo;
                                            //}
                                            //_pushNotificationsQueue.Enqueue(request);

                                            //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                                            // khushali 30-01-2019 for Common Template Method call 
                                            TemplateMasterData TemplateData = new TemplateMasterData();
                                            CommunicationParamater communicationParamater = new CommunicationParamater();
                                            SendEmailRequest request = new SendEmailRequest();
                                            communicationParamater.Param1 = checkmail.UserName; //Username
                                            communicationParamater.Param2 = model.DeviceId; //Devicename
                                            communicationParamater.Param3 = DateTime.UtcNow.ToString(); //Time
                                            communicationParamater.Param4 = model.IPAddress; //Ipaddress
                                            communicationParamater.Param5 = Ipwisedata.Location; //Location
                                            TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SuccessfulLogin, communicationParamater, enCommunicationServiceType.Email).Result;
                                            if (TemplateData != null)
                                            {
                                                if (TemplateData.IsOnOff == 1)
                                                {
                                                    request.Recepient = checkmail.Email;
                                                    request.Body = TemplateData.Content;
                                                    request.Subject = TemplateData.AdditionalInfo;
                                                    _pushNotificationsQueue.Enqueue(request);
                                                }
                                            }
                                            HttpContext.Items["UserId"] = checkmail.Id;
                                            // added by nirav savariya for Thememode for user base showing mode for Thememode=0(false) means Day mode and Thememode=1(true) on 1-10-2019 03-20 PM
                                            return Ok(new OTPEmailVerificationResponse {PreferedLanguage= checkmail.PreferedLanguage, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardLoginSuccess, Thememode = checkmail.Thememode });
                                        }
                                        else
                                        {
                                            string[] DeviceDetails = null;
                                            if (model.DeviceId.Contains('|'))
                                                DeviceDetails = model.DeviceId.Split('|');

                                            AuthorizeDeviceViewModel authorizeDeviceView = new AuthorizeDeviceViewModel();
                                            authorizeDeviceView.UserId = checkmail.Id;
                                            authorizeDeviceView.Location = Ipwisedata.Location;
                                            authorizeDeviceView.IPAddress = model.IPAddress;
                                            authorizeDeviceView.DeviceName = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                                            authorizeDeviceView.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                                            authorizeDeviceView.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                                            authorizeDeviceView.CurrentTime = DateTime.UtcNow;
                                            authorizeDeviceView.Expirytime = DateTime.UtcNow + TimeSpan.FromHours(Convert.ToInt32(_configuration["DefaultValidateAuthorizedDevice"]));


                                            byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                                            string UserDetails = JsonConvert.SerializeObject(authorizeDeviceView);
                                            string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);
                                            byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);

                                            string ctokenlink = _configuration["AuthorizedDeviceURL"].ToString() + Convert.ToBase64String(plainTextBytes);

                                            //SendEmailRequest request = new SendEmailRequest();
                                            //request.Recepient = checkmail.Email;

                                            //IQueryable Result = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.AuthorizedNewDevice), 0);
                                            //foreach (TemplateMasterData Provider in Result)
                                            //{

                                            //    Provider.Content = Provider.Content.Replace("###Location###".ToUpper(), Location);
                                            //    Provider.Content = Provider.Content.Replace("###IPAddress###".ToUpper(), model.IPAddress);
                                            //    Provider.Content = Provider.Content.Replace("###Device###".ToUpper(), DeviceDetails != null ? DeviceDetails[0] : string.Empty);
                                            //    Provider.Content = Provider.Content.Replace("###Link###".ToUpper(), ctokenlink);
                                            //    Provider.Content = Provider.Content.Replace("###year###".ToUpper(), DateTime.Now.Year.ToString());
                                            //    request.Body = Provider.Content;
                                            //    request.Subject = Provider.AdditionalInfo;
                                            //}
                                            //_pushNotificationsQueue.Enqueue(request);

                                            //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                                            // khushali 30-01-2019 for Common Template Method call 
                                            TemplateMasterData TemplateData = new TemplateMasterData();
                                            CommunicationParamater communicationParamater = new CommunicationParamater();
                                            SendEmailRequest request = new SendEmailRequest();
                                            communicationParamater.Param1 = Ipwisedata.Location; //Location
                                            communicationParamater.Param2 = model.IPAddress; //IPAddress
                                            communicationParamater.Param3 = DeviceDetails != null ? DeviceDetails[0] : string.Empty; //Device
                                            communicationParamater.Param4 = ctokenlink; //Link
                                            TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.AuthorizedNewDevice, communicationParamater, enCommunicationServiceType.Email).Result;
                                            if (TemplateData != null)
                                            {
                                                if (TemplateData.IsOnOff == 1)
                                                {
                                                    request.Recepient = checkmail.Email;
                                                    request.Body = TemplateData.Content;
                                                    request.Subject = TemplateData.AdditionalInfo;
                                                    _pushNotificationsQueue.Enqueue(request);
                                                }
                                            }
                                            HttpContext.Items["UserId"] = checkmail.Id;
                                            var UnAuthorizedDeviceMessage = EnResponseMessage.UnAuthorizedDevice.Replace("###SiteName###", _configuration["SiteNameForWelcomeMsg"]);
                                            return Ok(new LoginWithEmailDataResponse { ReturnCode = enResponseCode.Success, ReturnMsg = UnAuthorizedDeviceMessage, ErrorCode = enErrorCode.Status4137UnAuthorizedDevice });
                                        }
                                    }
                                }
                                else
                                {
                                    if (checkmail?.AccessFailedCount < Convert.ToInt16(_configuration["MaxFailedAttempts"]))
                                    {
                                        checkmail.AccessFailedCount = checkmail.AccessFailedCount + 1;
                                        if (checkmail.AccessFailedCount == Convert.ToInt16(_configuration["MaxFailedAttempts"]))
                                        {
                                            checkmail.LockoutEnd = DateTime.UtcNow.AddHours(Convert.ToInt64(_configuration["DefaultLockoutTimeSpan"]));
                                            checkmail.AccessFailedCount = 0;
                                            await _userManager.UpdateAsync(checkmail);
                                            return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginLockOut, ErrorCode = enErrorCode.Status423Locked });
                                        }
                                        else
                                        {
                                            await _userManager.UpdateAsync(checkmail);
                                            return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginWithOtpInvalidAttempt, ErrorCode = enErrorCode.Status4088LoginWithOtpInvalidAttempt });
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginWithOtpInvalidAttempt, ErrorCode = enErrorCode.Status4088LoginWithOtpInvalidAttempt });
                                    }
                                }
                            }
                            else
                                return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpResendOTP, ErrorCode = enErrorCode.Status4076SignUpReSendOTP });
                        }
                        else
                            return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                    }
                    else
                        return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginFailEmailNotAvailable, ErrorCode = enErrorCode.Status4086LoginFailEmailNotAvailable });
                }
                else
                {
                    if (checkmail?.LockoutEnd >= DateTime.UtcNow)
                        return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginLockOut, ErrorCode = enErrorCode.Status423Locked });
                    else
                        return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.EmailFail, ErrorCode = enErrorCode.Status4087EmailFail });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        /// This method are used login with email resend otp base verify
        [HttpPost("ReSendOtpWithEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ReSendOtpWithEmail(LoginWithEmailViewModel model)
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
                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                ////// Ip Address Validate or not
                string CountryCode = await _userService.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(CountryCode) && CountryCode == "fail")
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                var userdt = await _userManager.FindByEmailAsync(model.Email);
                if (!userdt.IsEnabled) // added by nirav savariya for check user is active on 28-01-2019
                    return BadRequest(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });

                if (!string.IsNullOrEmpty(userdt?.Email))
                {
                    HttpContext.Items["UserId"] = userdt.Id; // added by nirav savariya for add user id in activity log on 28-1-2019
                    var otpcheck = await _otpMasterService.GetOtpData(Convert.ToInt32(userdt?.Id),2);
                    if (otpcheck != null)
                    {
                        //if (otpcheck.ExpirTime <= DateTime.UtcNow && !otpcheck.EnableStatus) // Remove expiretime as per discuss with nishit bhai 10-09-2018
                        //if (!otpcheck.EnableStatus)
                        //{
                            _otpMasterService.UpdateOtp(otpcheck.Id, 2, "");
                            var custompwd = await _custompassword.GetPassword(otpcheck.UserId);
                            if (custompwd != null)
                                _custompassword.UpdateOtp(custompwd.Id);
                            var otpData = await _otpMasterService.AddOtp(Convert.ToInt32(userdt.Id), userdt.Email, "");
                            if (otpData != null)
                            {
                                CustomtokenViewModel data = new CustomtokenViewModel(); // added by nirav savariya for login with mobile and email on 16-10-2018
                                data.Password = otpData.Password;
                                data.UserId = otpData.UserId;
                                data.EnableStatus = false;
                                await _custompassword.AddPassword(data);
                                return Ok(new LoginWithEmailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.LoginUserEmailOTP, Appkey = otpData.appkey });
                            //}
                            //else
                            //    return BadRequest(new LoginWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginEmailOTPNotsend, ErrorCode = enErrorCode.Status4089LoginEmailOTPNotsend });
                        }
                        else
                            return BadRequest(new LoginWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.EmailFail, ErrorCode = enErrorCode.Status4087EmailFail });
                    }
                    else
                        return BadRequest(new LoginWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status14073OTPDataNotFound });
                }
                else
                    return BadRequest(new LoginWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginFailEmailNotAvailable, ErrorCode = enErrorCode.Status4086LoginFailEmailNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new LoginWithEmailResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Login With Mobile
        //khushali 20-03-2019 for Backoffice Admin
        [HttpPost("BackOfficeLoginWithMobileV1")]
        [AllowAnonymous]
        public async Task<IActionResult> BackOfficeLoginWithMobileV1(LoginWithMobileViewModel model)
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
                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                ////// Ip Address Validate or not
                string CountryCode = await _userService.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(CountryCode) && CountryCode == "fail")
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                var userdt = await _userService.FindByMobileNumber(model.Mobile);
                if (userdt == null && string.IsNullOrEmpty(userdt?.Mobile))
                    return Ok(new LoginWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginWithMobileOtpLoginFailed, ErrorCode = enErrorCode.Status4106LoginFailMobileNotAvailable });

                var userResult = _userManager.FindByIdAsync(userdt.Id.ToString());

                if (!userdt.PhoneNumberConfirmed) //  for verify pending
                    return Ok(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUpTempUserMobileExistAndVerificationPending, ErrorCode = enErrorCode.Status4036VerifyPending });

                if (!userdt.IsEnabled) // for check user is active
                    return Ok(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });


                var user = await userResult;
                //// khushali 22-03-2019 for Backoffice Admin Role Check 
                //var roles = await _userManager.GetRolesAsync(user);

                //if (roles == null)
                //    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "This Admin User not found.", ErrorCode = enErrorCode.Status14060AdminUserNotAvailable });

                //var MatchingRoles = roles.Where(o => o.ToLower() == "admin").FirstOrDefault();

                //if (MatchingRoles == null)
                //    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "This Admin User not found.", ErrorCode = enErrorCode.Status14060AdminUserNotAvailable });

                // khushali 05-04-2019 for Backoffice Admin IsCreatedByAdmin bit check
                if (user.IsCreatedByAdmin == 0)
                    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "This Admin User not found.", ErrorCode = enErrorCode.Status14060AdminUserNotAvailable });


                HttpContext.Items["UserId"] = userdt.Id; // for user id store in activity log
                var otpData = await _otpMasterService.AddOtp(Convert.ToInt32(userdt.Id), "", userdt.Mobile);
                if (otpData != null)
                {
                    CustomtokenViewModel data = new CustomtokenViewModel(); // for login with mobile and email
                    data.Password = otpData.Password;
                    data.UserId = otpData.UserId;
                    data.EnableStatus = false;
                    await _custompassword.AddPassword(data);
                    return Ok(new LoginWithMobileResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.OTPSendOnMobile, Appkey = otpData.appkey });
                }
                else
                    return Ok(new LoginWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.OTPNotSendOnMobile, ErrorCode = enErrorCode.Status4090OTPSendOnMobile });
            }
            catch (Exception ex)
            {
                return BadRequest(new LoginWithMobileResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        /// <summary>
        /// This method are used login with Mobile otp base verification 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("MobileOtpVerification")]
        [AllowAnonymous]
        public async Task<IActionResult> MobileOtpVerification(OTPWithMobileViewModel model)
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
                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                ////// Ip Address Validate or not
                var IpwiseData = await _userService.GetIPWiseData(model.IPAddress);
               //if (!string.IsNullOrEmpty(IpwiseData.CountryCode) && IpwiseData.CountryCode == "fail")
                if (IpwiseData.IsValid == false)
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                var logindata = await _userService.FindByMobileNumber(model.Mobile);
                if (!logindata.IsEnabled) // added by nirav savariya for check user is active on 28-01-2019
                    return BadRequest(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });

                var result = await _userManager.FindByIdAsync(logindata?.Id.ToString());
                if (result != null)
                {
                    bool flag = _IIPRange.IPAddressinrange(model.IPAddress, result.Id);  /// Added by pankaj for check ip in range or not  04-01-2019
                    if (!flag)
                    {
                        return BadRequest(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IPNotValid, ErrorCode = enErrorCode.Status4152IPNotValid });
                    }
                }
                if (result != null)
                {
                    #region This functionality for check the otp expiration validation     Added By pankaj
                    var UserOTPPolicy = _UserPasswordPolicyMaster.GetUserPasswordPolicyConfiguration(result.Id);   /// Get user wise password Configuration if User not found then get default configuration
                    if (UserOTPPolicy != null)
                    {
                        bool CheckOTPExpiration = _UserPasswordPolicyMaster.CheckOTPAndLinkExpiration(result.Id, UserOTPPolicy.OTPExpiryTime, passwordPolicy.LoginWithMobile);  /// Base on parameter this methis check that is valid otp or not
                        if (CheckOTPExpiration != true)
                        {
                            return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpResendOTP, ErrorCode = enErrorCode.Status4076SignUpReSendOTP });
                        }
                    }
                    #endregion
                }
                //string Location = IpwiseData.Location;//await _userService.GetLocationByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(result?.Mobile) && (result.LockoutEnd <= DateTime.UtcNow || result.LockoutEnd == null))
                {
                    if (logindata?.Id > 0)
                    {
                        var tempotp = await _otpMasterService.GetOtpData(Convert.ToInt32(logindata.Id),2);
                        if (tempotp != null)
                        {
                            if (tempotp?.ExpirTime >= DateTime.UtcNow)
                            {
                                if (model.OTP == tempotp.OTP)
                                {
                                    _otpMasterService.UpdateOtp(tempotp.Id, 2, "");  /// Added by pankaj for update the opt enable status
                                    if (result.TwoFactorEnabled)   /// Addede By Pankaj For TwoFactor Authentication Purporse
                                    {

                                        //++++++++++++++++++++++++++++++++++++++//
                                        // AddIpHistory and AddLoginHistory is comment by Pratik :: 12-3-2019
                                        bool IPExist = _iipHistory.IsIpHistoryExist(result.Id, model.IPAddress);
                                        if (!IPExist)
                                        {
                                            //// added by nirav savariya for ip history user wise on 11-03-2018
                                            var IpHistory = new IpHistoryViewModel()
                                            {
                                                IpAddress = model.IPAddress,
                                                Location = IpwiseData.Location,
                                                UserId = result.Id,
                                            };
                                            _iipHistory.AddIpHistory(IpHistory);
                                        }

                                        var Loginhistory = new LoginhistoryViewModel()
                                        {
                                            UserId = result.Id,
                                            IpAddress = model.IPAddress,
                                            Device = model.DeviceId,
                                            Location = IpwiseData.Location
                                        };
                                        _loginHistory.AddLoginHistory(Loginhistory);

                                        ////// Start 2FA in Custome token Create                                    
                                        string TwoFAToken = _userKeyMasterService.Get2FACustomToken(logindata.Id);
                                        ////// End 2FA in Custome token Create 

                                        ///// Check valid device detail and ip address for valid authorized on 10-12-2018  by nirav savariya
                                        bool checkvalidip = _iipAddressService.CheckValidIpaddress(result.Id, model.IPAddress);
                                        bool checkvaliddevice = _IdeviceIdService.CheckValidDevice(result.Id, model.DeviceId);
                                        if (!checkvalidip)
                                        {
                                            ///// added by nirav savariya for ipwhitelist on 10-12-2018
                                            IpMasterViewModel ipmodel = new IpMasterViewModel();
                                            ipmodel.IpAddress = model.IPAddress;
                                            ipmodel.UserId = Convert.ToInt32(result.Id);
                                            _iipAddressService.AddIpAddress(ipmodel);
                                        }

                                        if (!checkvaliddevice)
                                        {
                                            string[] DeviceDetails = null;
                                            if (model.DeviceId.Contains('|'))
                                                DeviceDetails = model.DeviceId.Split('|');

                                            ///// added by nirav savariya for devicewhitelisting on 10-12-2018
                                            DeviceMasterViewModel devicemodel = new DeviceMasterViewModel();
                                            devicemodel.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty; ;
                                            devicemodel.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                                            devicemodel.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                                            devicemodel.UserId = Convert.ToInt32(result.Id);
                                            _IdeviceIdService.AddDeviceProcess(devicemodel);
                                        }
                                        HttpContext.Items["UserId"] = result.Id;
                                        return Ok(new OtpWithMobile2FAResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.FactorRequired, ErrorCode = enErrorCode.Status4060VerifyMethod, TwoFAToken = TwoFAToken });
                                    }
                                    else
                                    {
                                        //++++++++++++++++++++++++++++++++++++++//
                                        // AddIpHistory and AddLoginHistory is comment by Pratik :: 12-3-2019
                                        bool IPExist = _iipHistory.IsIpHistoryExist(result.Id, model.IPAddress);
                                        if (!IPExist)
                                        {
                                            //// added by nirav savariya for ip history user wise on 11-03-2018
                                            var IpHistorydet = new IpHistoryViewModel()
                                            {
                                                IpAddress = model.IPAddress,
                                                Location = IpwiseData.Location,
                                                UserId = result.Id,
                                            };
                                            _iipHistory.AddIpHistory(IpHistorydet);
                                        }
                                        var LoginhistoryViewModel = new LoginhistoryViewModel()
                                        {
                                            UserId = result.Id,
                                            IpAddress = model.IPAddress,
                                            Device = model.DeviceId,
                                            Location = IpwiseData.Location
                                        };
                                        _loginHistory.AddLoginHistory(LoginhistoryViewModel);



                                        //_otpMasterService.UpdateOtp(tempotp.Id);
                                        //var roles = await _userManager.GetRolesAsync(result);

                                        ///// Check valid device detail and ip address for valid authorized on 10-12-2018  by nirav savariya
                                        bool checkvalidip = _iipAddressService.CheckValidIpaddress(result.Id, model.IPAddress);
                                        bool checkvaliddevice = _IdeviceIdService.CheckValidDevice(result.Id, model.DeviceId);
                                        if (!checkvalidip)
                                        {
                                            ///// added by nirav savariya for ipwhitelist on 10-12-2018
                                            IpMasterViewModel ipmodel = new IpMasterViewModel();
                                            ipmodel.IpAddress = model.IPAddress;
                                            ipmodel.UserId = Convert.ToInt32(result.Id);
                                            _iipAddressService.AddIpAddress(ipmodel);
                                        }

                                        if (!checkvaliddevice)
                                        {
                                            string[] DeviceDetails = null;
                                            if (model.DeviceId.Contains('|'))
                                                DeviceDetails = model.DeviceId.Split('|');

                                            ///// added by nirav savariya for devicewhitelisting on 10-12-2018
                                            DeviceMasterViewModel devicemodel = new DeviceMasterViewModel();
                                            devicemodel.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty; ;
                                            devicemodel.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                                            devicemodel.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                                            devicemodel.UserId = Convert.ToInt32(result.Id);
                                            _IdeviceIdService.AddDeviceProcess(devicemodel);
                                        }
                                        HttpContext.Items["UserId"] = result.Id;
                                        // added by nirav savariya for Thememode for user base showing mode for Thememode=0(false) means Day mode and Thememode=1(true) on 1-10-2019 03-20 PM
                                        return Ok(new OTPMobileVerificationResponse { PreferedLanguage=result.PreferedLanguage, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardLoginSuccess, Thememode = result.Thememode });
                                    }
                                }
                                if (result?.AccessFailedCount < Convert.ToInt16(_configuration["MaxFailedAttempts"]))
                                {
                                    result.AccessFailedCount = result.AccessFailedCount + 1;
                                    if (result.AccessFailedCount == Convert.ToInt16(_configuration["MaxFailedAttempts"]))
                                    {
                                        result.LockoutEnd = DateTime.UtcNow.AddHours(Convert.ToInt64(_configuration["DefaultLockoutTimeSpan"]));
                                        result.AccessFailedCount = 0;
                                        await _userManager.UpdateAsync(result);
                                        return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginLockOut, ErrorCode = enErrorCode.Status423Locked });
                                    }
                                    else
                                    {
                                        await _userManager.UpdateAsync(result);
                                        return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginWithOtpInvalidAttempt, ErrorCode = enErrorCode.Status4088LoginWithOtpInvalidAttempt });
                                    }
                                }
                                else
                                    return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginWithOtpInvalidAttempt, ErrorCode = enErrorCode.Status4088LoginWithOtpInvalidAttempt });
                            }
                            else
                                return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginWithOtpInvalidAttempt, ErrorCode = enErrorCode.Status4088LoginWithOtpInvalidAttempt });
                        }
                    }
                    else
                        return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginWithOtpInvalidAttempt, ErrorCode = enErrorCode.Status4088LoginWithOtpInvalidAttempt });
                }
                else
                {
                    if (result?.LockoutEnd >= DateTime.UtcNow)
                        return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginLockOut, ErrorCode = enErrorCode.Status423Locked });
                    else
                        return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginMobileNumberInvalid, ErrorCode = enErrorCode.Status4091LoginMobileNumberInvalid });
                }
                return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginMobileNumberInvalid, ErrorCode = enErrorCode.Status4091LoginMobileNumberInvalid });
            }
            catch (Exception ex)
            {
                return BadRequest(new OTPWithMobileResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        /// <summary>
        /// This method are used login with mobile resend otp base verify 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("ReSendOtpWithMobile")]
        [AllowAnonymous]
        public async Task<IActionResult> ReSendOtpWithMobile(LoginWithMobileViewModel model)
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
                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                ////// Ip Address Validate or not
                string CountryCode = await _userService.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(CountryCode) && CountryCode == "fail")
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                var userdt = await _userService.FindByMobileNumber(model.Mobile);
                if (!userdt.IsEnabled) // added by nirav savariya for check user is active on 01-02-2019
                    return BadRequest(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });

                if (!string.IsNullOrEmpty(userdt?.Mobile))
                {
                    var otpcheck = await _otpMasterService.GetOtpData(Convert.ToInt32(userdt?.Id),2);
                    if (otpcheck != null)
                    {
                        //if (otpcheck.ExpirTime <= DateTime.UtcNow && !otpcheck.EnableStatus)  // Remove expiretime as per discuss with nishit bhai 10-09-2018
                        //if (!otpcheck.EnableStatus)
                        //{
                            _otpMasterService.UpdateOtp(otpcheck.Id, 2, "");
                            var custompwd = await _custompassword.GetPassword(otpcheck.UserId);
                            if (custompwd != null)
                                _custompassword.UpdateOtp(custompwd.Id);
                            var otpData = await _otpMasterService.AddOtp(Convert.ToInt32(userdt.Id), "", userdt.Mobile);
                            if (otpData != null)
                            {
                                CustomtokenViewModel data = new CustomtokenViewModel(); // added by nirav savariya for login with mobile and email on 16-10-2018
                                data.Password = otpData.Password;
                                data.UserId = otpData.UserId;
                                data.EnableStatus = false;
                                await _custompassword.AddPassword(data);
                                HttpContext.Items["UserId"] = userdt.Id;
                                return Ok(new LoginWithMobileResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.OTPSendOnMobile, Appkey = otpData.appkey });
                            }
                            else
                                return BadRequest(new LoginWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.OTPNotSendOnMobile, ErrorCode = enErrorCode.Status4090OTPSendOnMobile });
                        //}
                        //else
                        //    return BadRequest(new LoginWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginMobileNumberInvalid, ErrorCode = enErrorCode.Status4090OTPSendOnMobile });
                    }
                    else
                        return BadRequest(new LoginWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                }
                else
                    return BadRequest(new LoginWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LoginMobileNumberInvalid, ErrorCode = enErrorCode.Status4090OTPSendOnMobile });
            }
            catch (Exception ex)
            {
                return BadRequest(new LoginWithMobileResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Social Login

        /// <summary>
        ///  This method are use to Social Login method for google
        /// </summary>
        //[HttpPost("ExternalLoginForGoogle")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ExternalLoginForGoogle([FromBody] SocialLoginWithGoogleViewModel model)
        //{
        //    try
        //    {
        //        var httpClient = new HttpClient();
        //        var appAccessTokenResponse = (dynamic)null;
        //        try
        //        {
        //            appAccessTokenResponse = await httpClient.GetStringAsync(_configuration["SocialGoogle"].ToString() + model.access_token);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(new SocialLoginGoogleResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidGoogleToken, ErrorCode = enErrorCode.Status4068InvalidGoogleToken });
        //        }
        //        if (appAccessTokenResponse != null)
        //        {
        //            var userAccessTokenValidation = JsonConvert.DeserializeObject<GoogleSocial>(appAccessTokenResponse);
        //            if (userAccessTokenValidation.user_id == model.ProviderKey)
        //            {
        //                var userLogin = await _userManager.FindByEmailAsync(userAccessTokenValidation.email);
        //                if (userLogin != null)
        //                {
        //                    var user = await _userManager.FindByLoginAsync(model.ProviderName, model.ProviderKey);
        //                    SocialCustomPasswordViewMoel socialCustomPasswordViewMoel = _userService.GenerateRandomSocialPassword(model.ProviderKey);

        //                    if (socialCustomPasswordViewMoel != null)
        //                    {
        //                        CustomtokenViewModel data = new CustomtokenViewModel();
        //                        data.Password = socialCustomPasswordViewMoel.Password;
        //                        data.UserId = userLogin.Id;
        //                        data.EnableStatus = false;
        //                        await _custompassword.AddPassword(data);

        //                    }
        //                    HttpContext.Items["UserId"] = userLogin.Id;
        //                    return Ok(new SocialLoginGoogleResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardLoginSuccess, Appkey = socialCustomPasswordViewMoel.AppKey });
        //                }
        //                else
        //                {
        //                    var user = new ApplicationUser { UserName = userAccessTokenValidation.email, Email = userAccessTokenValidation.email };
        //                    var userdet = await _userManager.CreateAsync(user);
        //                    var infodet = new UserLoginInfo(model.ProviderName, model.ProviderKey, model.ProviderName);
        //                    if (userdet.Succeeded)
        //                    {
        //                        var userlogin = await _userManager.AddLoginAsync(user, infodet);
        //                        if (userlogin.Succeeded)
        //                        {
        //                            SocialCustomPasswordViewMoel socialCustomPasswordViewMoel = _userService.GenerateRandomSocialPassword(model.ProviderKey);
        //                            if (socialCustomPasswordViewMoel != null)
        //                            {
        //                                CustomtokenViewModel data = new CustomtokenViewModel();
        //                                data.Password = socialCustomPasswordViewMoel.Password;
        //                                data.UserId = user.Id;
        //                                data.EnableStatus = false;
        //                                await _custompassword.AddPassword(data);
        //                            }
        //                            HttpContext.Items["UserId"] = user.Id;
        //                            return Ok(new SocialLoginGoogleResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardLoginSuccess, Appkey = socialCustomPasswordViewMoel.AppKey });
        //                        }
        //                        else
        //                            return BadRequest(new SocialLoginGoogleResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SocialUserInsertError, ErrorCode = enErrorCode.Status4070SocialUserInsertError });
        //                    }
        //                    else
        //                        return BadRequest(new SocialLoginGoogleResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SocialUserInsertError, ErrorCode = enErrorCode.Status4070SocialUserInsertError });
        //                }
        //            }
        //            else
        //                return BadRequest(new SocialLoginGoogleResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidGoogleProviderKey, ErrorCode = enErrorCode.Status4069InvalidGoogleProviderKey });
        //        }
        //        else
        //            return BadRequest(new SocialLoginGoogleResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidGoogleToken, ErrorCode = enErrorCode.Status4068InvalidGoogleToken });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new SocialLoginGoogleResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        ///// <summary>
        ///// This method created by pankaj for user perform to  External login with facebook.
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost("ExternalLoginForFacebook")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ExternalLoginForFacebook([FromBody] SocialLoginWithfacebookViewModel model)
        //{
        //    try
        //    {
        //        var appAccessTokenResponse = (dynamic)null;
        //        var httpClient = new HttpClient();

        //        try
        //        {
        //            appAccessTokenResponse = await httpClient.GetStringAsync(_configuration["SocialFacebookToken"].ToString()
        //                + _configuration["Authentication:Facebook:AppId"].ToString() + "&client_secret=" + _configuration["Authentication:Facebook:AppSecret"].ToString() +
        //                "&grant_type=client_credentials");
        //            var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessTokenViewModel>(appAccessTokenResponse);

        //            string userAccessTokenValidationResponse = await httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={model.access_token}&access_token=" + appAccessToken.access_token);
        //            var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidationViewModel>(userAccessTokenValidationResponse);

        //            if (!userAccessTokenValidation.data.is_valid)
        //                return BadRequest(new SocialLoginfacebookResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidFaceBookToken, ErrorCode = enErrorCode.Status4096InvalidFaceBookToken });

        //            appAccessTokenResponse = await httpClient.GetStringAsync(_configuration["SocialFacebook"].ToString() + model.access_token);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(new SocialLoginfacebookResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidFaceBookToken, ErrorCode = enErrorCode.Status4096InvalidFaceBookToken });
        //        }
        //        if (appAccessTokenResponse != null)
        //        {
        //            var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookSocial>(appAccessTokenResponse);
        //            var userLogin = await _userManager.FindByEmailAsync(userAccessTokenValidation.email);
        //            if (userLogin != null)
        //            {
        //                SocialCustomPasswordViewMoel socialCustomPasswordViewMoel = _userService.GenerateRandomSocialPassword(model.ProviderKey);

        //                if (socialCustomPasswordViewMoel != null)
        //                {
        //                    CustomtokenViewModel data = new CustomtokenViewModel();
        //                    data.Password = socialCustomPasswordViewMoel.Password;
        //                    data.UserId = userLogin.Id;
        //                    data.EnableStatus = false;
        //                    await _custompassword.AddPassword(data);

        //                }
        //                HttpContext.Items["UserId"] = userLogin.Id;
        //                return Ok(new SocialLoginfacebookResponse { ReturnCode = enResponseCode.Success, Appkey = socialCustomPasswordViewMoel.AppKey, ReturnMsg = EnResponseMessage.StandardLoginSuccess });
        //            }
        //            else
        //            {

        //                var user =
        //                new ApplicationUser
        //                {
        //                    UserName = userAccessTokenValidation.email,
        //                    Email = userAccessTokenValidation.email,
        //                    FirstName = userAccessTokenValidation.first_name,
        //                    LastName = userAccessTokenValidation.last_name
        //                };   /// Here email address not set bacause of user can login with mobile number as well as email so.

        //                var userdet = await _userManager.CreateAsync(user);
        //                var infodet = new UserLoginInfo(model.ProviderName, model.ProviderKey, model.ProviderName);
        //                if (userdet.Succeeded)
        //                {
        //                    var userlogin = await _userManager.AddLoginAsync(user, infodet);

        //                    if (userlogin.Succeeded)
        //                    {
        //                        SocialCustomPasswordViewMoel socialCustomPasswordViewMoel = _userService.GenerateRandomSocialPassword(model.ProviderKey);
        //                        if (socialCustomPasswordViewMoel != null)
        //                        {
        //                            CustomtokenViewModel data = new CustomtokenViewModel();
        //                            data.Password = socialCustomPasswordViewMoel.Password;
        //                            data.UserId = user.Id;
        //                            data.EnableStatus = false;
        //                            await _custompassword.AddPassword(data);
        //                        }
        //                        HttpContext.Items["UserId"] = user.Id;
        //                        return Ok(new SocialLoginfacebookResponse { ReturnCode = enResponseCode.Success, Appkey = socialCustomPasswordViewMoel.AppKey, ReturnMsg = EnResponseMessage.StandardLoginSuccess });
        //                    }
        //                    else
        //                        return BadRequest(new SocialLoginfacebookResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SocialUserInsertError, ErrorCode = enErrorCode.Status4070SocialUserInsertError });
        //                }
        //                else
        //                    return BadRequest(new SocialLoginfacebookResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SocialUserInsertError, ErrorCode = enErrorCode.Status4070SocialUserInsertError });
        //            }
        //        }
        //        else
        //            return BadRequest(new SocialLoginGoogleResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidGoogleToken, ErrorCode = enErrorCode.Status4068InvalidGoogleToken });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new SocialLoginGoogleResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}
        #endregion

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                var currentUser = await _userManager.FindByEmailAsync(model.Email);

                if (currentUser == null) // || !(await _userManager.IsEmailConfirmedAsync(currentUser)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return BadRequest(new ForgotpassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ResetUserNotAvailable, ErrorCode = enErrorCode.Status4037UserNotAvailable });
                }
                //================Rita 3-10-19 check User status
                if (!currentUser.EmailConfirmed) //  for verify pending 
                    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Verificationpending, ErrorCode = enErrorCode.Status4036VerifyPending });

                if (!currentUser.IsEnabled) //  for verify pending 
                    return BadRequest(new StandardLoginResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotActive, ErrorCode = enErrorCode.Status4168UserNotActive });
                //================================

                #region This functionality for check the otp expiration validation     Added By pankaj
                var UserPasswordForgotInDay = _UserPasswordPolicyMaster.GetUserPasswordPolicyConfiguration(currentUser.Id);   /// Get user wise password Configuration if User not found then get default configuration
                if (UserPasswordForgotInDay != null)
                {
                    bool CheckOTPExpiration = _UserPasswordPolicyMaster.CheckForgotPasswordInday(currentUser.Id, UserPasswordForgotInDay.MaxfppwdDay, passwordPolicy.PasswordExpire);  /// Base on parameter this methis check that is valid otp or not
                    if (CheckOTPExpiration != true)
                    {
                        return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PasswordforgotInday, ErrorCode = enErrorCode.Status4159PasswordForgotinday });
                    }
                }
                #endregion

                #region This functionality for check the otp expiration validation     Added By pankaj
                var UserPasswordForgotInMonth = _UserPasswordPolicyMaster.GetUserPasswordPolicyConfiguration(currentUser.Id);   /// Get user wise password Configuration if User not found then get default configuration
                //if (UserPasswordForgotInMonth != null)
                //{
                //    bool CheckOTPExpiration = _UserPasswordPolicyMaster.CheckForgotPasswordInMonth(currentUser.Id, UserPasswordForgotInMonth.MaxfppwdMonth, passwordPolicy.PasswordExpire);  /// Base on parameter this methis check that is valid otp or not
                //    if (CheckOTPExpiration != true)
                //    {
                //        return BadRequest(new OTPWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PasswordforgotInMonth, ErrorCode = enErrorCode.Status4160PasswordForgotinMonth });
                //    }
                //}
                #endregion

                #region Add the functionality to forgotpassword flow
                var LinkExpiryTime = 2; // khushali 19-03-2019 for check null condition 
                if (UserPasswordForgotInMonth != null)
                {
                    LinkExpiryTime = UserPasswordForgotInMonth.LinkExpiryTime;
                }
                UserLinkMasterViewModel userLinkMaster = new UserLinkMasterViewModel()
                {
                    LinkvalidTime = LinkExpiryTime,
                    UserLinkData = model.Email,
                    UserId = currentUser.Id
                };

                Guid id = _IUserLinkMaster.Add(userLinkMaster);
                ForgotPasswordDataViewModel forgotPassword = new ForgotPasswordDataViewModel()
                {
                    Id = id,
                    LinkvalidTime = LinkExpiryTime 
                };
                byte[] Userdata = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                string UserForgotPassword = JsonConvert.SerializeObject(forgotPassword);
                string ForgotPasswordKey = EncyptedDecrypted.Encrypt(UserForgotPassword, Userdata);
                byte[] ForgotWordplainTextBytes = Encoding.UTF8.GetBytes(ForgotPasswordKey);

                string Forgotctokenlink = _configuration["Forgotverifylink"].ToString() + Convert.ToBase64String(ForgotWordplainTextBytes);

                //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                // khushali 30-01-2019 for Common Template Method call 
                TemplateMasterData TemplateData = new TemplateMasterData();
                CommunicationParamater communicationParamater = new CommunicationParamater();
                SendEmailRequest request = new SendEmailRequest();
                communicationParamater.Param1 = currentUser.FirstName;
                communicationParamater.Param2 = Forgotctokenlink;
                TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.ForgotPassword, communicationParamater, enCommunicationServiceType.Email).Result;
                if (TemplateData != null)
                {
                    if (TemplateData.IsOnOff == 1)
                    {
                        request.Recepient = model.Email;
                        request.Body = TemplateData.Content;
                        request.Subject = TemplateData.AdditionalInfo;
                        _pushNotificationsQueue.Enqueue(request);
                    }
                }



                var ForgotPasswordresult = await _userManager.FindByEmailAsync(model.Email);
                if (ForgotPasswordresult != null)
                {
                    //ForgotPasswordresult.EmailConfirmed = false;
                    await _userManager.UpdateAsync(ForgotPasswordresult);
                    HttpContext.Items["UserId"] = currentUser.Id;
                    return Ok(new ForgotpassWordResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ResetConfirmedLink });
                }
                else
                {
                    return BadRequest(new ForgotpassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser });
                }
                #endregion
            }
            catch (Exception ex)
            {
                return BadRequest(new ForgotpassWordResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpGet("Forgotverifylink")]
        [AllowAnonymous]
        public async Task<IActionResult> Forgotverifylink(string LinkData)  //ResetPasswordViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(LinkData))
                {
                    byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                    var bytes = Convert.FromBase64String(LinkData);
                    var encodedString = Encoding.UTF8.GetString(bytes);
                    string DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);

                    ForgotPasswordDataViewModel model = JsonConvert.DeserializeObject<ForgotPasswordDataViewModel>(DecryptToken);
                    if (model != null)
                    {
                        var ForgotpasswordData = _IUserLinkMaster.VerifyUserLink(model.Id);
                        if (ForgotpasswordData != null)
                        {
                            if (ForgotpasswordData.CreatedDate.AddMinutes(ForgotpasswordData.LinkvalidTime) > DateTime.UtcNow)
                            {
                                HttpContext.Items["UserId"] = ForgotpasswordData.UserId;

                                //2019-6-18
                                TypeLogRequest obj = new TypeLogRequest();
                                obj.UserID = ForgotpasswordData.UserId;
                                obj.ActivityType = enActivityType.ForgotPassword;
                                obj.OldValue = "";
                                obj.NewValue = "";
                               // obj.ActivityType = enActivityType.ForgotPassword;
                                _custompassword.AddActivityTypeLog(obj);
                                //2019-6-18

                                return Ok(new ForgotPassWordResponse { ReturnCode = enResponseCode.Success, Id = ForgotpasswordData.Id, ReturnMsg = EnResponseMessage.EmialSuccessfullyVerify });
                            }
                            else
                                return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ResetPasswordEmailExpire, ErrorCode = enErrorCode.Status4039ResetPasswordLinkExpired });
                        }
                        else
                            return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AllreadyLinkVerify, ErrorCode = enErrorCode.Status4163AllreadyLinkVerify });
                    }
                    else
                        return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ResetPasswordEmailExpire, ErrorCode = enErrorCode.Status4039ResetPasswordLinkExpired });
                }
                else
                    return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ResetPasswordEmailLinkBlank, ErrorCode = enErrorCode.Status4040ResetPasswordLinkempty });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("setpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> SetPassword([FromBody]ForgotPasswordSetViewModel model)
        {
            try
            {
                var ForgotpasswordData = _IUserLinkMaster.GetUserLinkData(model.Id);
                if (ForgotpasswordData != null)
                {
                    var currentUser = await _userManager.FindByIdAsync(ForgotpasswordData.UserId.ToString());
                    if (currentUser != null)
                    {
                        string hashedNewPassword = string.Empty;

                        hashedNewPassword = _userManager.PasswordHasher.HashPassword(currentUser, model?.Password);
                        currentUser.PasswordHash = hashedNewPassword;
                        //currentUser.EmailConfirmed = true;
                        var result = await _userManager.UpdateAsync(currentUser);
                        if (result.Succeeded)
                        {
                            UserLinkMasterUpdateViewModel userLinkMaster = new UserLinkMasterUpdateViewModel()
                            {
                                Id = model.Id,
                                LinkvalidTime = ForgotpasswordData.LinkvalidTime,
                                UserId = ForgotpasswordData.UserId,
                                UserLinkData = ForgotpasswordData.UserLinkData
                            };
                            Guid id = _IUserLinkMaster.Update(userLinkMaster);
                            HttpContext.Items["UserId"] = currentUser.Id;
                            return Ok(new ForgotPasswordSetResponseViewModel { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ChangePassword });
                        }
                        else
                            return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UnableChangePassword, ErrorCode = enErrorCode.Status4164UnableChangePassword });
                    }
                    else
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                }
                else
                    return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UnableChangePassword, ErrorCode = enErrorCode.Status4164UnableChangePassword });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        /// <summary>
        ///  This method are use to create the Random Password
        /// </summary>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 6,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true,

            };

            string[] randomChars = new[] {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
        "abcdefghijkmnopqrstuvwxyz",    // lowercase
        "0123456789",                   // digits
         "!@$^*"                        // non-alphanumeric
    };
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }



        [HttpGet("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string emailConfirmCode)  //ResetPasswordViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailConfirmCode))
                {
                    //add validation LinkData valid or not -mansi 23-09-2019
                    if (string.IsNullOrEmpty(emailConfirmCode) || emailConfirmCode.Length % 4 != 0 || emailConfirmCode.Contains(" ") || emailConfirmCode.Contains("\t") || emailConfirmCode.Contains("\r") || emailConfirmCode.Contains("\n"))

                        return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Invalidlinkdata, ErrorCode = enErrorCode.Status1450invalidelinkdata });

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

                    ResetPasswordViewModel model = JsonConvert.DeserializeObject<ResetPasswordViewModel>(DecryptToken);
                    if (model?.Expirytime >= DateTime.UtcNow)
                    {
                        var user = await _userManager.FindByEmailAsync(model.Email);

                        if (user == null)
                        {
                            // Don't reveal that the user does not exist
                            //return Ok("Reset confirmed");
                            return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ResetPasswordUseNotexist, ErrorCode = enErrorCode.Status4038ResetUserNotAvailable });
                            //return AppUtils.Standerdlogin("Reset confirmed");
                        }

                        if (user.EmailConfirmed)
                        {
                            return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ResetConfirm, ErrorCode = enErrorCode.Status4041ResetPasswordConfirm });
                        }

                        string hashedNewPassword = string.Empty;

                        hashedNewPassword = _userManager.PasswordHasher.HashPassword(user, model?.Password);

                        user.PasswordHash = hashedNewPassword;
                        user.EmailConfirmed = true;
                        var result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                            // khushali 30-01-2019 for Common Template Method call 
                            TemplateMasterData TemplateData = new TemplateMasterData();
                            CommunicationParamater communicationParamater = new CommunicationParamater();
                            SendEmailRequest request = new SendEmailRequest();
                            communicationParamater.Param1 = user.UserName;
                            communicationParamater.Param2 = model.Password;
                            TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.ResetPassword, communicationParamater, enCommunicationServiceType.Email).Result;
                            if (TemplateData != null)
                            {
                                if (TemplateData.IsOnOff == 1)
                                {
                                    request.Recepient = model.Email;
                                    request.Body = TemplateData.Content;
                                    request.Subject = TemplateData.AdditionalInfo;
                                    _pushNotificationsQueue.Enqueue(request);
                                }
                            }
                            HttpContext.Items["UserId"] = user.Id;
                            return Ok(new ResetPassWordResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ResetResendEmail });
                        }
                    }
                    else
                        return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ResetPasswordEmailExpire, ErrorCode = enErrorCode.Status4039ResetPasswordLinkExpired });
                }
                else
                    return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ResetPasswordEmailLinkBlank, ErrorCode = enErrorCode.Status4040ResetPasswordLinkempty });
                return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = "null", ErrorCode = enErrorCode.Status400BadRequest });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResetPassWordResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #region UnLockUser
        /// <summary>
        /// This method are used for user unlock. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("UnLockUser")]
        [AllowAnonymous] // added by nirav savariya for with out token using unlock user on 2-1-2019
        public async Task<IActionResult> UnLockUser(UnLockUserViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());
                if (user != null)
                {
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = null;
                    var userUpdate = await _userManager.UpdateAsync(user);
                    if (userUpdate.Succeeded)
                    {
                        HttpContext.Items["UserId"] = user.Id;
                        return Ok(new UnLockUserResponseViewModel { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.UnLockUser });
                    }
                    else
                        return BadRequest(new UnLockUserResponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UnLockUserError, ErrorCode = enErrorCode.Status4077UserUnlockError });
                }
                else
                    return BadRequest(new UnLockUserResponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
            }
            catch (Exception ex)
            {
                return BadRequest(new UnLockUserResponseViewModel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Logout        
        [HttpPost("logout")]
        public async Task<IActionResult> LogOff()
        {
            try
            {
                await _signInManager.SignOutAsync();
                foreach (var cookieKey in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookieKey);
                }
                _logger.LogInformation(4, "User logged out.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Date: " + _basePage.UTC_To_IST() + ",\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nControllername=" + this.GetType().Name, LogLevel.Error);

                return BadRequest(new Logoffresponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion
        [HttpPost("GetSocailkey")]
        [AllowAnonymous]
        public IActionResult GetSocailkey(string Providername)
        {
            try
            {
                if (string.IsNullOrEmpty(Providername))
                {
                    return BadRequest(new SocialLoginGoogleResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InputProvider, ErrorCode = enErrorCode.Status4101InputProvider });
                }
                SocialKeyDetailViewModel socialKeyDetailViewModel = new SocialKeyDetailViewModel();
                if (Providername == "Facebook")
                {
                    socialKeyDetailViewModel.ProviderName = Providername;
                    socialKeyDetailViewModel.ClientId = _configuration["Authentication:Facebook:AppId"].ToString();
                    socialKeyDetailViewModel.ClientSecret = _configuration["Authentication:Facebook:AppSecret"].ToString();

                }
                else if (Providername == "Google")
                {
                    socialKeyDetailViewModel.ProviderName = Providername;
                    socialKeyDetailViewModel.ClientId = _configuration["Authentication:Google:ClientId"].ToString();
                    socialKeyDetailViewModel.ClientSecret = _configuration["Authentication:Google:ClientSecret"].ToString();
                }

                else if (Providername == "Twitter")
                {
                    socialKeyDetailViewModel.ProviderName = Providername;
                    socialKeyDetailViewModel.ClientId = _configuration["Authentication:Twitter:ConsumerKey"].ToString();
                    socialKeyDetailViewModel.ClientSecret = _configuration["Authentication:Twitter:ConsumerSecret"].ToString();

                }
                else if (Providername == "Microsoft")
                {
                    socialKeyDetailViewModel.ProviderName = Providername;
                    socialKeyDetailViewModel.ClientId = _configuration["Authentication:Microsoft:ClientId"].ToString();
                    socialKeyDetailViewModel.ClientSecret = _configuration["Authentication:Microsoft:ClientSecret"].ToString();

                }
                else
                {
                    return BadRequest(new SocialKeyDetailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.provideDetailNotAvailable, ErrorCode = enErrorCode.Status4100provideDetailNotAvailable });
                }
                return Ok(new SocialKeyDetailResponse { ReturnCode = enResponseCode.Success, socialKeyDetailViewModel = socialKeyDetailViewModel, ReturnMsg = EnResponseMessage.SocialLoginKey });


            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Date: " + _basePage.UTC_To_IST() + ",\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nControllername=" + this.GetType().Name, LogLevel.Error);

                return BadRequest(new SocialKeyDetailResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion
    }

}