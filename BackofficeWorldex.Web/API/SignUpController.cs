using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.ApiModels.Chat;
using CleanArchitecture.Core.Entities.Configuration;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Interfaces.EmailMaster;
using CleanArchitecture.Core.Interfaces.Log;
using CleanArchitecture.Core.Interfaces.PhoneMaster;
using CleanArchitecture.Core.Interfaces.Profile_Management;
using CleanArchitecture.Core.Interfaces.Referral;
using CleanArchitecture.Core.Interfaces.User;
using CleanArchitecture.Core.Services;
using CleanArchitecture.Core.ViewModels;
using CleanArchitecture.Core.ViewModels.AccountViewModels;
using CleanArchitecture.Core.ViewModels.AccountViewModels.Log;
using CleanArchitecture.Core.ViewModels.AccountViewModels.Login;
using CleanArchitecture.Core.ViewModels.AccountViewModels.OTP;
using CleanArchitecture.Core.ViewModels.AccountViewModels.SignUp;
using CleanArchitecture.Core.ViewModels.EmailMaster;
using CleanArchitecture.Core.ViewModels.MobileMaster;
using CleanArchitecture.Core.ViewModels.Profile_Management;
using CleanArchitecture.Core.ViewModels.Referral;
using CleanArchitecture.Core.ViewModels.Wallet;
using CleanArchitecture.Infrastructure.BGTask;
using CleanArchitecture.Infrastructure.DTOClasses;
using CleanArchitecture.Infrastructure.Interfaces;
using CleanArchitecture.Infrastructure.Services;
using CleanArchitecture.Web.Filters;
using CleanArchitecture.Web.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhoneNumbers;

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : BaseController
    {
        #region Field
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommonRepository<LanguagePreferenceMaster> _LanguagePreferenceMaster;
        private readonly IUserService _userdata;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly IRegisterTypeService _registerTypeService;
        private readonly ITempOtpService _tempOtpService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IBasePage _basePage;
        private readonly IWalletService _IwalletService;
        private readonly ISubscriptionMaster _IsubscriptionMaster;
        private readonly IMessageService _messageService;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue; //24-11-2018 komal make Email Enqueue
        private IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;//24-11-2018 komal make SMS Enqueue
        private readonly ISignalRService _signalRService;
        private readonly ISignupLogService _IsignupLogService; // added by nirav savariya for generate signup log report on 12-05-2018
        private readonly IDeviceIdService _IdeviceIdService; // added by nirav savariya for device white list report on 12-07-2018
        private readonly IipAddressService _iipAddressService; //added by nirav savariya for ip address white list report on 12-07-2018
        private readonly IMediator _mediator;
        private readonly ICreateWalletQueue<WalletReqRes> _IcreateWalletQueue;
        private readonly IEmailMaster _IemailMaster;
        private readonly Iphonemaster _Iphonemaster;
        private readonly IOtpMasterService _otpMasterService;
        private readonly IReferralService _ReferralService;
        
        private readonly IReferralUser _ReferralUser;
        private readonly IReferralChannelType _ReferralChannelType;
        #endregion

        #region Ctore
        public SignUpController(UserManager<ApplicationUser> userManager,
            //ILoggerFactory loggerFactory, 
            IUserService userdata,  EncyptedDecrypted encdecAEC, IRegisterTypeService registerTypeService, ITempOtpService tempOtpService, Microsoft.Extensions.Configuration.IConfiguration configuration,
            IBasePage basePage, IWalletService walletService, ISubscriptionMaster IsubscriptionMaster, IMessageService MessageService,
            IPushNotificationsQueue<SendSMSRequest> PushSMSQueue, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, ISignalRService signalRService,
            ISignupLogService IsignupLogService, IDeviceIdService IdeviceIdService, IipAddressService iipAddressService, IMediator mediator, ICreateWalletQueue<WalletReqRes> IcreateWalletQueue,
            IEmailMaster IemailMaster, Iphonemaster Iphonemaster, IOtpMasterService otpMasterService, ICommonRepository<LanguagePreferenceMaster> LanguagePreferenceMaster, IReferralService ReferralService, 
            IReferralUser ReferralUser, IReferralChannelType ReferralChannelType)
        {
            _userManager = userManager;
            _userdata = userdata;
            _encdecAEC = encdecAEC;
            _LanguagePreferenceMaster = LanguagePreferenceMaster;
            _registerTypeService = registerTypeService;
            _tempOtpService = tempOtpService;
            _configuration = configuration;
            _basePage = basePage;
            _IwalletService = walletService;
            _IsubscriptionMaster = IsubscriptionMaster;
            _messageService = MessageService;
            _pushNotificationsQueue = pushNotificationsQueue; //24-11-2018 komal make Email Enqueue
            _pushSMSQueue = PushSMSQueue; //24-11-2018 komal make SMS Enqueue
            _signalRService = signalRService;
            _IsignupLogService = IsignupLogService; // added by nirav savariya on 12-05-2018
            _IdeviceIdService = IdeviceIdService; // added by nirav savariya on 12-05-2018
            _iipAddressService = iipAddressService; // added by nirav savariya on 12-05-2018
            _mediator = mediator;
            _IcreateWalletQueue = IcreateWalletQueue;
            _IemailMaster = IemailMaster;
            _Iphonemaster = Iphonemaster;
            _otpMasterService = otpMasterService;
            _ReferralService = ReferralService;
            _ReferralChannelType = ReferralChannelType;
            _ReferralUser = ReferralUser;
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


        #endregion

        #region Methods

        #region Default register

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            try
            {
                ////// Ip Address Validate or not
                var ipmodel = await _userdata.GetIPWiseData(model.IPAddress);
                if (!string.IsNullOrEmpty(ipmodel?.CountryCode) && ipmodel?.CountryCode == "fail")
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                }
                var language = _LanguagePreferenceMaster.GetSingle(i => i.PreferedLanguage == model.PreferedLanguage);
                if (language == null)
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidLang, ErrorCode = enErrorCode.InvalidLang });
                }
                //////////check mobile valid or not
                bool isValidNumber = await _userdata.IsValidPhoneNumber(model.Mobile, model.CountryCode);
                if (!isValidNumber)
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardSignUpPhonevalid, ErrorCode = enErrorCode.Status4013MobileInvalid });
                }

                //////////////////// Check bizUser  table in Email Exist or not
                var result = await _userManager.FindByEmailAsync(model.Email);
                if (result != null)
                {
                    if (result.EmailConfirmed == false)
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpTempUserEmailVerifyPending, ErrorCode = enErrorCode.Status4036VerifyPending });
                    }
                    else
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserEmailExist, ErrorCode = enErrorCode.Status4098BizUserEmailExist });
                    }
                }
                ///////////////// Check bizUser  table in username  Exist or not
                var resultUserName = await _userManager.FindByNameAsync(model.Username);
                if (!string.IsNullOrEmpty(resultUserName?.UserName))
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserNameExist, ErrorCode = enErrorCode.Status4099BizUserNameExist });
                }
                ///////////////// Check bizUser  table in username  Exist or not
                var resultMobileUserName = await _userManager.FindByNameAsync(model.Mobile);
                if (!string.IsNullOrEmpty(resultMobileUserName?.UserName))
                {
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status4074SignUPMobileValidation });
                }

                ///////////////// Check bizUser  table in mobile number  Exist or not
                bool IsSignMobile = _userdata.GetMobileNumber(model.Mobile);
                if (!IsSignMobile)
                {
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status4074SignUPMobileValidation });
                }
                // Add by Pratik for inert data to Referral user table :: 25-02-2019

                int ServiceId = Convert.ToInt32(model.ReferralServiceId) > 0 ? Convert.ToInt32(model.ReferralServiceId) : 1;
                if (ServiceId != 0)
                {
                    bool ReferralServiceId = _ReferralService.ReferralServiceExist(ServiceId);
                    if (!ReferralServiceId)
                    {
                        return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralServiceNotExist, ErrorCode = enErrorCode.Status32041ReferralServiceNotExist });
                    }
                }

                int ChannelTypeId = Convert.ToInt32(model.ReferralChannelTypeId) > 0 ? Convert.ToInt32(model.ReferralChannelTypeId) : 1;
                if (ChannelTypeId != 0)
                {
                    bool ChannelType = _ReferralChannelType.IsReferralChannelTypeExistById(ChannelTypeId);
                    if (!ChannelType)
                    {
                        return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralChannelTypeNotExist, ErrorCode = enErrorCode.Status32042ReferralChannelTypeNotExist });
                    }
                }


                // Add by Pratik :: 9-2-2019
                a:
                string NewReferralCode = _ReferralUser.GenerateRandomReferralCode();
                bool ReferralCodeDupe = _ReferralUser.FindDuplicateReferralCode(NewReferralCode);
                if (ReferralCodeDupe)
                {
                    goto a;
                }
                // End by Pratik                               

                byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                var currentUser = new ApplicationUser   ///  Create the Bizuser table 
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FirstName = model.Firstname,
                    LastName = model.Lastname,
                    Mobile = model.Mobile,
                    PasswordHash = model.Password,
                    RegTypeId = await _registerTypeService.GetRegisterId(enRegisterType.Standerd),
                    CountryCode = model.CountryCode,
                    CreatedDate = DateTime.UtcNow,
                    PreferedLanguage = model.PreferedLanguage,
                    ReferralCode = NewReferralCode // Add by Pratik :: 9-2-2019
                };
                var resultdata = await _userManager.CreateAsync(currentUser, currentUser.PasswordHash);

                if (resultdata.Succeeded)
                {
                    var Currentuser = await _userManager.FindByNameAsync(model.Username);
                    int RefUser = 0;

                    if (!string.IsNullOrEmpty(model.ReferralCode))
                    {
                        RefUser = await _ReferralUser.GetReferCodeUser(model.ReferralCode);

                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                            var bytes = Convert.FromBase64String(returnUrl);
                            var encodedString = Encoding.UTF8.GetString(bytes);
                            string DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);

                            ReferralUserViewModel dmodel = JsonConvert.DeserializeObject<ReferralUserViewModel>(DecryptToken);
                            ChannelTypeId = dmodel.ReferralChannelTypeId;  // Particular Promotion Type
                        }

                    }
                    var ObjReferralUser = new ReferralUserViewModel()
                    {

                        UserId = RefUser != 0 ? RefUser : 3,
                        ReferUserId = Convert.ToInt32(Currentuser.Id),
                        ReferralServiceId = ServiceId,
                        ReferralChannelTypeId = ChannelTypeId,
                        IsCommissionCredited = 0
                    };
                    _ReferralUser.AddReferralUser(ObjReferralUser);


                    //// added by nirav savariya for register log user wise on 12-05-2018
                    var SignUplog = new SignUpLogViewModel()
                    {
                        TempUserId = Convert.ToInt32(Currentuser.Id),
                        RegisterType = await _registerTypeService.GetRegisterId(enRegisterType.Standerd),
                        Device = model.DeviceId,
                        Mode = model.Mode,
                        IpAddress = model.IPAddress,
                        Location = ipmodel != null ? ipmodel.Location : string.Empty,
                        HostName = model.HostName,
                        RegisterStatus = false,
                    };
                    _IsignupLogService.AddSignUpLog(SignUplog);

                    string[] DeviceDetails = null;
                    if (model.DeviceId.Contains('|'))
                        DeviceDetails = model.DeviceId.Split('|');

                    LinkTokenViewModel linkToken = new LinkTokenViewModel();
                    linkToken.Id = Currentuser.Id;
                    linkToken.Username = model.Username;
                    linkToken.Email = model.Email;
                    linkToken.Firstname = model.Firstname;
                    linkToken.Lastname = model.Lastname;
                    linkToken.Mobile = model.Mobile;
                    linkToken.CurrentTime = DateTime.UtcNow;
                    linkToken.Expirytime = DateTime.UtcNow + TimeSpan.FromHours(2);
                    linkToken.CountryCode = model.CountryCode;
                    linkToken.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                    linkToken.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                    linkToken.DeviceID = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                    linkToken.IpAddress = model.IPAddress;


                    string UserDetails = JsonConvert.SerializeObject(linkToken);
                    string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);



                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
                    string ctokenlink = _configuration["ConfirmMailURL"].ToString() + Convert.ToBase64String(plainTextBytes);

                    // khushali 30-01-2019 for Common Template Method call 
                    TemplateMasterData TemplateData = new TemplateMasterData();
                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    SendEmailRequest request = new SendEmailRequest();
                    if (!string.IsNullOrEmpty(Currentuser.UserName))
                        communicationParamater.Param1 = Currentuser.UserName;
                    else
                        communicationParamater.Param1 = string.Empty;
                    communicationParamater.Param2 = ctokenlink;
                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.ConfirmationMail, communicationParamater, enCommunicationServiceType.Email).Result;
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
                    return Ok(new RegisterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardSignUp });
                }
                else
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserRegisterError, ErrorCode = enErrorCode.Status4102SignUpUserRegisterError });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        // Email Link to direct call this method
        [HttpGet("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string emailConfirmCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailConfirmCode))   ////  Create the standard signup method data check is null or not
                {
                    byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                    var bytes = Convert.FromBase64String(emailConfirmCode);
                    var encodedString = Encoding.UTF8.GetString(bytes);
                    string DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);

                    LinkTokenViewModel dmodel = JsonConvert.DeserializeObject<LinkTokenViewModel>(DecryptToken);

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
                                var result = await _userManager.UpdateAsync(user);
                                if (result.Succeeded)
                                {
                                    if (user.Mobile != null)
                                    {
                                        var officeClaim = new Claim(OpenIdConnectConstants.Claims.PhoneNumber, user.Mobile.ToString(), ClaimValueTypes.Integer);
                                        await _userManager.AddClaimAsync(user, officeClaim);
                                    }

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
                                        _IsubscriptionMaster.AddSubscription(subscriptionmodel);

                                        ///   define the wallet services..
                                        //_IwalletService.CreateDefaulWallet(currentUser.Id);
                                        WalletReqRes walletReq = new WalletReqRes();
                                        walletReq.UserId = user.Id;
                                        _IcreateWalletQueue.Enqueue(walletReq);
                                      //  _ICreateMarginWalletQueue.Enqueue(walletReq);
                                        //_mediator.Send(walletReq);


                                        //// added by nirav savariya for verify user status on 12-05-2015
                                        _IsignupLogService.UpdateVerifiedUser(Convert.ToInt32(user.Id), user.Id);

                                        ///// added by nirav savariya for devicewhitelisting on 12-06-2018
                                        DeviceMasterViewModel model = new DeviceMasterViewModel();
                                        model.Device = dmodel.Device;
                                        model.DeviceOS = dmodel.DeviceOS;
                                        model.DeviceId = dmodel.DeviceID;
                                        model.UserId = Convert.ToInt32(user.Id);
                                        _IdeviceIdService.AddDeviceProcess(model);


                                        //++++++++++++++++++++++++++++++++++++++//
                                        // AddIpAddress is comment by Pratik :: 12-3-2019
                                        /////// added by nirav savariya for ipwhitelist on 12-07-2018
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

                                        if (!string.IsNullOrEmpty(dmodel.Password))  // This condition only use for back office standard register time call.
                                        {

                                            TemplateMasterData TemplateData1 = new TemplateMasterData();
                                            CommunicationParamater communicationParamater1 = new CommunicationParamater();
                                            SendEmailRequest request1 = new SendEmailRequest();
                                            communicationParamater1.Param1 = dmodel.Email;
                                            communicationParamater1.Param2 = dmodel.Password;
                                            TemplateData1 = _messageService.ReplaceTemplateMasterData(EnTemplateType.LoginPassword, communicationParamater1, enCommunicationServiceType.Email).Result;
                                            if (TemplateData1 != null)
                                            {
                                                if (TemplateData1.IsOnOff == 1)
                                                {
                                                    request1.Recepient = dmodel.Email;
                                                    request1.Body = TemplateData1.Content;
                                                    request1.Subject = TemplateData1.AdditionalInfo;
                                                    _pushNotificationsQueue.Enqueue(request1);
                                                }
                                            }
                                        }
                                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUpEmailConfirm });
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
        
        ///  This method are resend Email Link to direct call this method
        [HttpPost("ReSendRegisterlink")]
        [AllowAnonymous]
        public async Task<IActionResult> ReSendRegisterlink(SignUpWithEmailViewModel model)
        {
            try
            {
                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                }

                var result = await _userManager.FindByEmailAsync(model.Email);
                if (!string.IsNullOrEmpty(result?.Email))
                {

                    if (result.EmailConfirmed == false)
                    {
                        byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                        string[] DeviceDetails = null;
                        if (model.DeviceId.Contains('|'))
                            DeviceDetails = model.DeviceId.Split('|');

                        LinkTokenViewModel linkToken = new LinkTokenViewModel();
                        linkToken.Id = result.Id;
                        linkToken.Username = result.UserName;
                        linkToken.Email = model.Email;
                        linkToken.CurrentTime = DateTime.UtcNow;
                        linkToken.Expirytime = DateTime.UtcNow + TimeSpan.FromHours(2);
                        linkToken.Password = result.PasswordHash;
                        linkToken.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                        linkToken.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                        linkToken.DeviceID = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                        linkToken.IpAddress = model.IPAddress;

                        string UserDetails = JsonConvert.SerializeObject(linkToken);
                        string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);
                        byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
                        string ctokenlink = _configuration["ConfirmMailURL"].ToString() + Convert.ToBase64String(plainTextBytes);

                        TemplateMasterData TemplateData = new TemplateMasterData();
                        CommunicationParamater communicationParamater = new CommunicationParamater();
                        SendEmailRequest request = new SendEmailRequest();
                        if (!string.IsNullOrEmpty(result.UserName))
                            communicationParamater.Param1 = result.UserName;
                        else
                            communicationParamater.Param1 = string.Empty;
                        communicationParamater.Param2 = ctokenlink;
                        TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.ConfirmationMail, communicationParamater, enCommunicationServiceType.Email).Result;
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

                        //await _mediator.Send(request);                            
                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardResendSignUp });
                    }
                    else
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpValidation, ErrorCode = enErrorCode.Status4062UseralreadRegister });
                    }

                }
                else
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region SignUpWithEmail
        
        ///  This method are Direct signUp with email using verified link       
        [HttpPost("SignUpWithEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUpWithEmail(SignUpWithEmailViewModel model, string returnUrl = null)
        {
            try
            {
                var language = _LanguagePreferenceMaster.GetSingle(i => i.PreferedLanguage == model.PreferedLanguage);
                if (language == null)
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidLang, ErrorCode = enErrorCode.InvalidLang });
                }
                var ipmodel = await _userdata.GetIPWiseData(model.IPAddress);
                if (!string.IsNullOrEmpty(ipmodel?.CountryCode) && ipmodel.CountryCode == "fail")
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                }
                //////////////////// Check bizUser  table in Email Exist or not
                var result = await _userManager.FindByEmailAsync(model.Email);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result?.Email) && result.EmailConfirmed)  /// Check the user email is not null and check the Email OTP Is verified or not
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserEmailExist, ErrorCode = enErrorCode.Status4098BizUserEmailExist });
                    }
                    else
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpTempUserEmailVerifyPending, ErrorCode = enErrorCode.Status4036VerifyPending });
                    }
                }
                // Add by Pratik for inert data to Referral user table :: 25-02-2019

                int ServiceId = Convert.ToInt32(model.ReferralServiceId) > 0 ? Convert.ToInt32(model.ReferralServiceId) : 1;
                if (ServiceId != 0)
                {
                    bool ReferralServiceId = _ReferralService.ReferralServiceExist(ServiceId);
                    if (!ReferralServiceId)
                    {
                        return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralServiceNotExist, ErrorCode = enErrorCode.Status32041ReferralServiceNotExist });
                    }
                }

                int ChannelTypeId = Convert.ToInt32(model.ReferralChannelTypeId) > 0 ? Convert.ToInt32(model.ReferralChannelTypeId) : 1;
                if (ChannelTypeId != 0)
                {
                    bool ChannelType = _ReferralChannelType.IsReferralChannelTypeExistById(ChannelTypeId);
                    if (!ChannelType)
                    {
                        return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralChannelTypeNotExist, ErrorCode = enErrorCode.Status32042ReferralChannelTypeNotExist });
                    }
                }


                // Add by Pratik :: 9-2-2019
                a:
                string NewReferralCode = _ReferralUser.GenerateRandomReferralCode();
                bool ReferralCodeDupe = _ReferralUser.FindDuplicateReferralCode(NewReferralCode);
                if (ReferralCodeDupe)
                {
                    goto a;
                }
                // End by Pratik                               
                var currentUser = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    CreatedDate = DateTime.UtcNow,
                    EmailConfirmed = false,
                    RegTypeId = await _registerTypeService.GetRegisterId(enRegisterType.Email),
                    PreferedLanguage = model.PreferedLanguage,
                    ReferralCode = NewReferralCode // Add by Pratik :: 9-2-2019
                };

                var UserCreate = await _userManager.CreateAsync(currentUser);
                if (UserCreate.Succeeded)
                {
                    if (currentUser.Email != null)
                    {
                        var officeClaim = new Claim(OpenIdConnectConstants.Claims.PhoneNumber, currentUser.Email.ToString(), ClaimValueTypes.Integer);
                        await _userManager.AddClaimAsync(currentUser, officeClaim);
                    }
                    // Add to roles
                    var roleAddResult = await _userManager.AddToRoleAsync(currentUser, "User");

                    if (!roleAddResult.Succeeded)
                    {
                        return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpRole, ErrorCode = enErrorCode.Status4066UserRoleNotAvailable });
                    }

                    result = await _userManager.FindByEmailAsync(model.Email);
                    OtpMasterViewModel OptData = await _otpMasterService.AddOtpForSignupuser(result.Id, result.Email);

                    if (OptData != null)
                    {
                        int RefUser = 0;

                        if (!string.IsNullOrEmpty(model.ReferralCode))
                        {
                            RefUser = await _ReferralUser.GetReferCodeUser(model.ReferralCode);

                            if (!string.IsNullOrEmpty(returnUrl))
                            {
                                byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                                var bytes = Convert.FromBase64String(returnUrl);
                                var encodedString = Encoding.UTF8.GetString(bytes);
                                string DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);

                                ReferralUserViewModel dmodel = JsonConvert.DeserializeObject<ReferralUserViewModel>(DecryptToken);
                                ChannelTypeId = dmodel.ReferralChannelTypeId;  // Particular Promotion Type
                            }

                        }
                        var ObjReferralUser = new ReferralUserViewModel()
                        {

                            UserId = RefUser != 0 ? RefUser : 3,
                            ReferUserId = Convert.ToInt32(currentUser.Id),
                            ReferralServiceId = ServiceId,
                            ReferralChannelTypeId = ChannelTypeId,
                            IsCommissionCredited = 0
                        };
                        _ReferralUser.AddReferralUser(ObjReferralUser);


                        var SignUplog = new SignUpLogViewModel()
                        {
                            TempUserId = Convert.ToInt32(OptData.Id),
                            RegisterType = await _registerTypeService.GetRegisterId(enRegisterType.Email),
                            Device = model.DeviceId,
                            Mode = model.Mode,
                            IpAddress = model.IPAddress,
                            Location = ipmodel != null ? ipmodel.Location : string.Empty,
                            HostName = model.HostName,
                            RegisterStatus = false
                        };
                        _IsignupLogService.AddSignUpLog(SignUplog);

                        var resultotp = await _otpMasterService.GetOtpData(Convert.ToInt32(OptData.UserId));
                        // khushali 30-01-2019 for Common Template Method call 
                        TemplateMasterData TemplateData = new TemplateMasterData();
                        CommunicationParamater communicationParamater = new CommunicationParamater();
                        SendEmailRequest request = new SendEmailRequest();
                        if (!string.IsNullOrEmpty(result.UserName))
                            communicationParamater.Param1 = result.UserName;
                        else
                            communicationParamater.Param1 = string.Empty;
                        communicationParamater.Param2 = resultotp.OTP;
                        TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SignupEmailWithOTP, communicationParamater, enCommunicationServiceType.Email).Result;
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
                        return Ok(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignWithEmail });
                    }
                    else
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserRegisterError, ErrorCode = enErrorCode.Status4102SignUpUserRegisterError });
                    }
                }
                else
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserNotRegister, ErrorCode = enErrorCode.Status4063UserNotRegister });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        
        ///  This method are Direct signUp with email otp using verified.
        [HttpPost("EmailOtpVerification")]
        [AllowAnonymous]
        public async Task<IActionResult> EmailOtpVerification(OTPWithEmailViewModel model)
        {
            try
            {
                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                }
                var resultUser = await _userManager.FindByEmailAsync(model.Email);

                if (resultUser?.Id > 0)
                {

                    if (!string.IsNullOrEmpty(resultUser?.Email) && resultUser.EmailConfirmed)  /// Check the user email is not null and check the Email OTP Is verified or not
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserEmailExist, ErrorCode = enErrorCode.Status4098BizUserEmailExist });
                    }


                    var otp = await _otpMasterService.GetOtpData(Convert.ToInt16(resultUser.Id));
                    if (otp != null)
                    {
                        if (otp?.ExpirTime >= DateTime.UtcNow)
                        {
                            if (resultUser.Id == 0 && otp.Id == 0)
                            {
                                return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                            }
                            else if (model.OTP == otp.OTP)
                            {
                                if (!otp.EnableStatus)
                                {

                                    resultUser.EmailConfirmed = true;
                                    resultUser.IsEnabled = true;
                                    var result = await _userManager.UpdateAsync(resultUser);

                                    if (result.Succeeded)
                                    {
                                        resultUser = await _userManager.FindByEmailAsync(model.Email);


                                        _otpMasterService.UpdateEmailAndMobileOTP(otp.Id);

                                        var emailconfirmed = await _userManager.IsEmailConfirmedAsync(resultUser);

                                        WalletReqRes walletReq = new WalletReqRes();
                                        walletReq.UserId = resultUser.Id;
                                        _IcreateWalletQueue.Enqueue(walletReq);
                                        //_ICreateMarginWalletQueue.Enqueue(walletReq);
                                        string[] DeviceDetails = null;
                                        if (model.DeviceId.Contains('|'))
                                            DeviceDetails = model.DeviceId.Split('|');

                                        DeviceMasterViewModel devicemodel = new DeviceMasterViewModel();
                                        devicemodel.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                                        devicemodel.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                                        devicemodel.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                                        devicemodel.UserId = Convert.ToInt32(resultUser.Id);
                                        _IdeviceIdService.AddDeviceProcess(devicemodel);

                                        ///// added by nirav savariya for ipwhitelist on 12-07-2018
                                        IpMasterViewModel ipmodel = new IpMasterViewModel();
                                        ipmodel.IpAddress = model.IPAddress;
                                        ipmodel.UserId = Convert.ToInt32(resultUser.Id);
                                        _iipAddressService.AddIpAddress(ipmodel);

                                        SubscriptionViewModel subscriptionmodel = new SubscriptionViewModel()
                                        {
                                            UserId = resultUser.Id
                                        };
                                        _IsubscriptionMaster.AddSubscription(subscriptionmodel);
                                        
                                        TemplateMasterData TemplateData = new TemplateMasterData();
                                        CommunicationParamater communicationParamater = new CommunicationParamater();
                                        SendEmailRequest request = new SendEmailRequest();
                                        communicationParamater.Param1 = resultUser.UserName; //Username
                                        communicationParamater.Param2 = resultUser.Email; //Email
                                        TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EmailRegistration, communicationParamater, enCommunicationServiceType.Email).Result;
                                        if (TemplateData != null)
                                        {
                                            if (TemplateData.IsOnOff == 1)
                                            {
                                                request.Recepient = resultUser.Email;
                                                request.Body = TemplateData.Content;
                                                request.Subject = TemplateData.AdditionalInfo;
                                                _pushNotificationsQueue.Enqueue(request);
                                            }
                                        }

                                        EmailMasterReqViewModel emailMasterReqViewModel = new EmailMasterReqViewModel();
                                        {
                                            emailMasterReqViewModel.Email = resultUser.Email;
                                            emailMasterReqViewModel.IsPrimary = true;
                                            emailMasterReqViewModel.Userid = resultUser.Id;
                                        }
                                        Guid emailid = _IemailMaster.Add(emailMasterReqViewModel);

                                        HttpContext.Items["UserId"] = resultUser.Id;

                                        return Ok(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUPVerification });
                                    }
                                    else
                                    {
                                        return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserNotRegister, ErrorCode = enErrorCode.Status4063UserNotRegister });
                                    }
                                }
                                else
                                {
                                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpEmailValidation, ErrorCode = enErrorCode.Status4062UseralreadRegister });
                                }
                            }
                            else
                            {
                                return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpOTP, ErrorCode = enErrorCode.Status4067InvalidOTPorexpired });
                            }
                        }
                        else
                        {
                            return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpResendOTP, ErrorCode = enErrorCode.Status4067InvalidOTPorexpired });
                        }
                    }
                    else
                    {
                        return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                    }
                }
                else
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        
        ///  This method are Auto generate resend otp in Email
        [HttpPost("ReSendOtpWithEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ReSendOtpWithEmail(SignUpWithEmailViewModel model)
        {
            try
            {
                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

                }

                var result = await _userManager.FindByEmailAsync(model.Email);
                if (!string.IsNullOrEmpty(result?.Email))
                {
                    if (!string.IsNullOrEmpty(result?.Email) && result.EmailConfirmed)  /// Check the user email is not null and check the Email OTP Is verified or not
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserEmailExist, ErrorCode = enErrorCode.Status4098BizUserEmailExist });
                    }

                    var otp = await _otpMasterService.GetOtpData(Convert.ToInt16(result.Id));
                    if (otp != null)
                    {
                        if (!otp.EnableStatus)
                        {
                            _otpMasterService.UpdateEmailAndMobileOTP(otp.Id);
                            var resultdata = await _otpMasterService.AddOtpForSignupuser(Convert.ToInt16(result.Id), result.Email);


                            // khushali 30-01-2019 for Common Template Method call 
                            TemplateMasterData TemplateData = new TemplateMasterData();
                            CommunicationParamater communicationParamater = new CommunicationParamater();
                            SendEmailRequest request = new SendEmailRequest();
                            if (!string.IsNullOrEmpty(result.UserName))
                                communicationParamater.Param1 = result.UserName;
                            else
                                communicationParamater.Param1 = string.Empty;

                            communicationParamater.Param2 = resultdata.OTP;
                            TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SignupEmailWithOTP, communicationParamater, enCommunicationServiceType.Email).Result;
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

                            return Ok(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUpWithResendEmail });
                        }
                        else
                        {
                            return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpEmailValidation, ErrorCode = enErrorCode.Status4062UseralreadRegister });
                        }
                    }
                    else
                    {
                        return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                    }
                }
                else
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region SignUpWithMobile
        
        ///  This method are Direct signUp with mobile sms using verified opt.   
        [HttpPost("SignUpWithMobile")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUpWithMobile(SignUpWithMobileViewModel model, string returnUrl = null)
        {
            try
            {
                var ipmodel = await _userdata.GetIPWiseData(model.IPAddress);
                if (!string.IsNullOrEmpty(ipmodel?.CountryCode) && ipmodel.CountryCode == "fail")
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                }

                var language = _LanguagePreferenceMaster.GetSingle(i=>i.PreferedLanguage==model.PreferedLanguage);
                if (language == null)
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidLang, ErrorCode = enErrorCode.InvalidLang });
                }
                bool isValidNumber = await _userdata.IsValidPhoneNumber(model.Mobile, model.CountryCode);
                if (!isValidNumber)
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardSignUpPhonevalid, ErrorCode = enErrorCode.Status4013MobileInvalid });
                }
                // Add by Pratik for inert data to Referral user table :: 25-02-2019

                int ServiceId = Convert.ToInt32(model.ReferralServiceId) > 0 ? Convert.ToInt32(model.ReferralServiceId) : 1;
                if (ServiceId != 0)
                {
                    bool ReferralServiceId = _ReferralService.ReferralServiceExist(ServiceId);
                    if (!ReferralServiceId)
                    {
                        return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralServiceNotExist, ErrorCode = enErrorCode.Status32041ReferralServiceNotExist });
                    }
                }

                int ChannelTypeId = Convert.ToInt32(model.ReferralChannelTypeId) > 0 ? Convert.ToInt32(model.ReferralChannelTypeId) : 1;
                if (ChannelTypeId != 0)
                {
                    bool ChannelType = _ReferralChannelType.IsReferralChannelTypeExistById(ChannelTypeId);
                    if (!ChannelType)
                    {
                        return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralChannelTypeNotExist, ErrorCode = enErrorCode.Status32042ReferralChannelTypeNotExist });
                    }
                }


                // Add by Pratik :: 9-2-2019
                a:
                string NewReferralCode = _ReferralUser.GenerateRandomReferralCode();
                bool ReferralCodeDupe = _ReferralUser.FindDuplicateReferralCode(NewReferralCode);
                if (ReferralCodeDupe)
                {
                    goto a;
                }
                // End by Pratik                               
                var resultUserName = await _userManager.FindByNameAsync(model.Mobile);
                if (resultUserName != null)
                {
                    if (!string.IsNullOrEmpty(resultUserName?.UserName) && resultUserName.PhoneNumberConfirmed)
                    {
                        return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status4074SignUPMobileValidation });
                    }
                    else
                    {
                        return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUpTempUserMobileExistAndVerificationPending, ErrorCode = enErrorCode.Status4036VerifyPending });
                    }
                }

                bool MobileNumberExist = _userdata.CheckMobileNumberExists(model.Mobile);
                if (!MobileNumberExist)
                {
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status4074SignUPMobileValidation });
                }

                var currentUser = new ApplicationUser
                {
                    UserName = model.Mobile,
                    Mobile = model.Mobile,
                    CreatedDate = DateTime.UtcNow,
                    PhoneNumberConfirmed = false,
                    RegTypeId = await _registerTypeService.GetRegisterId(enRegisterType.Mobile),
                    PreferedLanguage = model.PreferedLanguage,
                    CountryCode = model.CountryCode,
                    ReferralCode = NewReferralCode // Add by Pratik :: 9-2-2019
                };

                var UserCreate = await _userManager.CreateAsync(currentUser);
                if (UserCreate.Succeeded)
                {
                    int RefUser = 0;

                    if (!string.IsNullOrEmpty(model.ReferralCode))
                    {
                        RefUser = await _ReferralUser.GetReferCodeUser(model.ReferralCode);

                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                            var bytes = Convert.FromBase64String(returnUrl);
                            var encodedString = Encoding.UTF8.GetString(bytes);
                            string DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);

                            ReferralUserViewModel dmodel = JsonConvert.DeserializeObject<ReferralUserViewModel>(DecryptToken);
                            ChannelTypeId = dmodel.ReferralChannelTypeId;  // Particular Promotion Type
                        }

                    }
                    var ObjReferralUser = new ReferralUserViewModel()
                    {

                        UserId = RefUser != 0 ? RefUser : 3,
                        ReferUserId = Convert.ToInt32(currentUser.Id),
                        ReferralServiceId = ServiceId,
                        ReferralChannelTypeId = ChannelTypeId,
                        IsCommissionCredited = 0
                    };
                    _ReferralUser.AddReferralUser(ObjReferralUser);
                    if (currentUser.Mobile != null)
                    {
                        var officeClaim = new Claim(OpenIdConnectConstants.Claims.PhoneNumber, currentUser.Mobile.ToString(), ClaimValueTypes.Integer);
                        await _userManager.AddClaimAsync(currentUser, officeClaim);
                    }
                    // Add to roles
                    var roleAddResult = await _userManager.AddToRoleAsync(currentUser, "User");

                    if (!roleAddResult.Succeeded)
                    {
                        return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpRole, ErrorCode = enErrorCode.Status4066UserRoleNotAvailable });
                    }

                    //// added by nirav savariya for register log user wise on 12-05-2018
                    var SignUplog = new SignUpLogViewModel()
                    {
                        TempUserId = Convert.ToInt32(currentUser.Id),
                        RegisterType = await _registerTypeService.GetRegisterId(enRegisterType.Mobile),
                        Device = model.DeviceId,
                        Mode = model.Mode,
                        IpAddress = model.IPAddress,
                        Location = ipmodel != null ? ipmodel.Location : string.Empty,
                        HostName = model.HostName,
                        RegisterStatus = false
                    };
                    _IsignupLogService.AddSignUpLog(SignUplog);


                    resultUserName = await _userManager.FindByNameAsync(model.Mobile);
                    OtpMasterViewModel OptData = await _otpMasterService.AddOtpForSignupuser(resultUserName.Id, null, model.Mobile);


                    SendSMSRequest request = new SendSMSRequest();
                    request.MobileNo = Convert.ToInt64(model.Mobile);
                    request.Message = EnResponseMessage.SendMailBody + OptData.OTP;
                    _pushSMSQueue.Enqueue(request); //24-11-2018 komal make SMS Enqueue

                    return Ok(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUpWithMobile });
                }
                else
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserRegisterError, ErrorCode = enErrorCode.Status4102SignUpUserRegisterError });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new SignUpWithMobileResponse
                {
                    ReturnCode = enResponseCode.InternalError,
                    ReturnMsg = ex.ToString(),
                    ErrorCode = enErrorCode.Status500InternalServerError
                });
            }
        }
        
        ///  This method are Direct signUp with mobile sms using verified otp.
        [HttpPost("MobileOtpVerification")]
        [AllowAnonymous]
        public async Task<IActionResult> MobileOtpVerification(OTPWithMobileViewModel model)
        {
            try
            {
                var language = _LanguagePreferenceMaster.GetSingle(i => i.PreferedLanguage == model.PreferedLanguage);
                if (language == null)
                {
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidLang, ErrorCode = enErrorCode.InvalidLang });
                }
                var resultUserName = await _userManager.FindByNameAsync(model.Mobile);
                if (resultUserName == null)
                {
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status400BadRequest });
                }
                OtpMasterViewModel OtpData = await _otpMasterService.GetOtpData(resultUserName.Id);


                if (OtpData != null)
                {
                    if (OtpData?.ExpirTime >= DateTime.UtcNow)
                    {
                        if (OtpData.Id == 0)
                        {
                            return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                        }
                        else if (model.OTP == OtpData.OTP)
                        {
                            if (!OtpData.EnableStatus)
                            {
                                resultUserName.PhoneNumberConfirmed = true;
                                resultUserName.IsEnabled = true;
                                resultUserName.PreferedLanguage = model.PreferedLanguage;
                                var result = await _userManager.UpdateAsync(resultUserName);
                                if (result.Succeeded)
                                {
                                    resultUserName = await _userManager.FindByNameAsync(model.Mobile);
                                    _otpMasterService.UpdateEmailAndMobileOTP(OtpData.Id);
                                    var mobileconfirmed = await _userManager.IsPhoneNumberConfirmedAsync(resultUserName);

                                    WalletReqRes walletReq = new WalletReqRes();
                                    walletReq.UserId = resultUserName.Id;
                                    _IcreateWalletQueue.Enqueue(walletReq);
                                  //  _ICreateMarginWalletQueue.Enqueue(walletReq);

                                    string[] DeviceDetails = null;
                                    if (model.DeviceId.Contains('|'))
                                        DeviceDetails = model.DeviceId.Split('|');

                                    DeviceMasterViewModel devicemodel = new DeviceMasterViewModel();
                                    devicemodel.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                                    devicemodel.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                                    devicemodel.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                                    devicemodel.UserId = Convert.ToInt32(resultUserName.Id);
                                    _IdeviceIdService.AddDeviceProcess(devicemodel);

                                    ///// added by nirav savariya for ipwhitelist on 12-07-2018
                                    IpMasterViewModel ipmodel = new IpMasterViewModel();
                                    ipmodel.IpAddress = model.IPAddress;
                                    ipmodel.UserId = Convert.ToInt32(resultUserName.Id);
                                    _iipAddressService.AddIpAddress(ipmodel);

                                    SubscriptionViewModel subscriptionmodel = new SubscriptionViewModel()
                                    {
                                        UserId = resultUserName.Id
                                    };
                                    _IsubscriptionMaster.AddSubscription(subscriptionmodel);
                                    /// Primary Mobile Register
                                    PhoneMasterReqViewModel phoneMasterReqViewModel = new PhoneMasterReqViewModel();
                                    phoneMasterReqViewModel.IsPrimary = true;
                                    phoneMasterReqViewModel.MobileNumber = resultUserName.Mobile;
                                    phoneMasterReqViewModel.Userid = resultUserName.Id;
                                    Guid phoneID = _Iphonemaster.Add(phoneMasterReqViewModel);

                                    HttpContext.Items["UserId"] = resultUserName.Id;

                                    return Ok(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUPVerification });
                                }
                                else
                                {
                                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status4074SignUPMobileValidation });
                                }
                            }
                            else
                            {
                                return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpOTP, ErrorCode = enErrorCode.Status4074SignUPMobileValidation });
                            }
                        }
                        else
                        {
                            return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpOTP, ErrorCode = enErrorCode.Status4075SignUPOTP });
                        }
                    }
                    else
                    {
                        return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpResendOTP, ErrorCode = enErrorCode.Status4076SignUpReSendOTP });
                    }
                }
                else
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status400BadRequest });
                }


            }
            catch (Exception ex)
            {
                return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        
        ///  This method are Auto generate resend otp in Mobile
        [HttpPost("ReSendOtpWithMobile")]
        [AllowAnonymous]
        public async Task<IActionResult> ReSendOtpWithMobile(SignUpWithMobileViewModel model)
        {
            try
            {
                var resultUserName = await _userManager.FindByNameAsync(model.Mobile);
                if (resultUserName == null)
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                }

                string CountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(CountryCode) && CountryCode == "fail")
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                }


                bool isValidNumber = await _userdata.IsValidPhoneNumber(model.Mobile, model.CountryCode);

                if (!isValidNumber)
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardSignUpPhonevalid, ErrorCode = enErrorCode.Status4013MobileInvalid });
                }
                var otp = await _otpMasterService.GetOtpData(Convert.ToInt16(resultUserName.Id));

                if (otp != null)
                {

                    if (!resultUserName.PhoneNumberConfirmed && otp.EnableStatus == false)
                    {
                        _otpMasterService.UpdateEmailAndMobileOTP(otp.Id);
                        var result = await _otpMasterService.AddOtp(Convert.ToInt16(resultUserName.Id), model.Mobile);

                        SendSMSRequest request = new SendSMSRequest();
                        request.MobileNo = Convert.ToInt64(model.Mobile);
                        request.Message = EnResponseMessage.SendSMSSubject + result.OTP;
                        _pushSMSQueue.Enqueue(request); //24-11-2018 komal make SMS Enqueue
                                                        //await _mediator.Send(request);
                        return Ok(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUpWithResendMobile });
                    }
                    else
                    {
                        return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status4074SignUPMobileValidation });
                    }
                }
                else
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region BlockChainSignUp

        [HttpPost("BlockChainSignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> BlockChainSignUp(BlockChainViewModel model)
        {
            try
            {
                return AppUtils.StanderdSignUp("Success");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        #endregion

        #endregion
    }
}