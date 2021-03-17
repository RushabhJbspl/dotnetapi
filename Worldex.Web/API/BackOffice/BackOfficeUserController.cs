using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.ApiModels.Chat;
using CleanArchitecture.Core.Entities.Configuration;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Interfaces.BackOffice;
using CleanArchitecture.Core.Interfaces.EmailMaster;
using CleanArchitecture.Core.Interfaces.Log;
using CleanArchitecture.Core.Interfaces.PhoneMaster;
using CleanArchitecture.Core.Interfaces.Profile_Management;
using CleanArchitecture.Core.Interfaces.User;
using CleanArchitecture.Core.Services;
using CleanArchitecture.Core.ViewModels;
using CleanArchitecture.Core.ViewModels.AccountViewModels.Log;
using CleanArchitecture.Core.ViewModels.AccountViewModels.OTP;
using CleanArchitecture.Core.ViewModels.AccountViewModels.SignUp;
using CleanArchitecture.Core.ViewModels.BackOfficeUser;
using CleanArchitecture.Core.ViewModels.EmailMaster;
using CleanArchitecture.Core.ViewModels.MobileMaster;
using CleanArchitecture.Core.ViewModels.Profile_Management;
using CleanArchitecture.Infrastructure.BGTask;
using CleanArchitecture.Infrastructure.DTOClasses;
using CleanArchitecture.Infrastructure.Interfaces;
using CleanArchitecture.Infrastructure.Services.BackOffice;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CleanArchitecture.Web.API.BackOffice
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BackOfficeUserController : BaseController
    {
        #region Field

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userdata;
        //private readonly IMediator _mediator;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly ITempUserRegisterService _tempUserRegisterService;
        private readonly IRegisterTypeService _registerTypeService;
        private readonly ITempOtpService _tempOtpService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IWalletService _IwalletService;
        private readonly ISubscriptionMaster _IsubscriptionMaster;
        //private readonly IMessageConfiguration _messageConfiguration;
        private readonly IMessageService _messageService;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private readonly ISignalRService _signalRService;
        private readonly ISignupLogService _IsignupLogService;
        private readonly IDeviceIdService _IdeviceIdService;
        private readonly IipAddressService _iipAddressService;
        private readonly IMediator _mediator;
        private readonly ICreateWalletQueue<WalletReqRes> _IcreateWalletQueue;
        private readonly IEmailMaster _IemailMaster;
        private readonly Iphonemaster _Iphonemaster;
        private readonly IOtpMasterService _otpMasterService;
        private readonly IRuleManageService _ruleManageService;
        private readonly ICommonRepository<LanguagePreferenceMaster> _LanguagePreferenceMaster;

        #endregion

        #region Ctore

        public BackOfficeUserController(UserManager<ApplicationUser> userManager,
            IUserService userdata, ITempUserRegisterService tempUserRegisterService,
            EncyptedDecrypted encdecAEC, IRegisterTypeService registerTypeService,
            ITempOtpService tempOtpService, Microsoft.Extensions.Configuration.IConfiguration configuration,
            IWalletService walletService, ISubscriptionMaster IsubscriptionMaster, IMessageService MessageService,
            IPushNotificationsQueue<SendSMSRequest> PushSMSQueue, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue,
            ISignalRService signalRService, ISignupLogService IsignupLogService, IDeviceIdService IdeviceIdService, IipAddressService iipAddressService,
            IMediator mediator, ICreateWalletQueue<WalletReqRes> IcreateWalletQueue,
            IEmailMaster IemailMaster, Iphonemaster Iphonemaster, IOtpMasterService otpMasterService, IRuleManageService RuleManageService, ICommonRepository<LanguagePreferenceMaster> LanguagePreferenceMaster)
        {
            _userManager = userManager;
            _userdata = userdata;
            _tempUserRegisterService = tempUserRegisterService;
            _encdecAEC = encdecAEC;
            _registerTypeService = registerTypeService;
            _tempOtpService = tempOtpService;
            _configuration = configuration;
            _IwalletService = walletService;
            _IsubscriptionMaster = IsubscriptionMaster;
            _messageService = MessageService;
            _pushNotificationsQueue = pushNotificationsQueue;
            _pushSMSQueue = PushSMSQueue;
            _signalRService = signalRService;
            _IsignupLogService = IsignupLogService;
            _IdeviceIdService = IdeviceIdService;
            _iipAddressService = iipAddressService;
            _mediator = mediator;
            _IcreateWalletQueue = IcreateWalletQueue;
            _IemailMaster = IemailMaster;
            _Iphonemaster = Iphonemaster;
            _otpMasterService = otpMasterService;
            _ruleManageService = RuleManageService;
            _LanguagePreferenceMaster = LanguagePreferenceMaster;
        }

        #endregion

        #region Method
        [HttpPost("RegisterUser")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(BackOfficeUserViewModel model)
        {
            try
            {
                var language = _LanguagePreferenceMaster.GetSingle(i => i.PreferedLanguage == model.PreferedLanguage);
                if (language == null)
                {
                    return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidLang, ErrorCode = enErrorCode.InvalidLang });
                }

                ////// Ip Address Validate or not
                var ipmodel = await _userdata.GetIPWiseData(model.IPAddress);
                if (!string.IsNullOrEmpty(ipmodel?.CountryCode) && ipmodel?.CountryCode == "fail")
                {
                    return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status9029IpInvalid });
                }

                if (!string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.Mobile))
                {
                    //////////////////// Check bizUser  table in Email Exist or not
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        var result = await _userManager.FindByEmailAsync(model.Email);
                        if (result != null)
                        {
                            if (!string.IsNullOrEmpty(result?.Email))
                            {
                                if (result.EmailConfirmed)
                                    return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserEmailExist, ErrorCode = enErrorCode.Status9030BizUserEmailExist });
                                else
                                    return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpTempUserEmailVerifyPending, ErrorCode = enErrorCode.Status9031VerifyPending });
                            }
                        }
                    }

                    bool IsEmail = Regex.IsMatch(model.Email, @"^[-a-zA-Z0-9~!$%^&*_=+}{\'?]+(\.[-a-zA-Z0-9~!$%^&*_=+}{\'?]+)*@([a-zA-Z0-9_][-a-zA-Z0-9_]*(\.[-a-zA-Z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-zA-Z]{2,3})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$");
                    if (!IsEmail)
                        return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOfficeInvalidEmail, ErrorCode = enErrorCode.Status9050BackOfficeInvalidEmail });


                    var currentUser = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        CreatedDate = DateTime.UtcNow,
                        EmailConfirmed = false,
                        RegTypeId = await _registerTypeService.GetRegisterId(enRegisterType.Email),
                        PreferedLanguage=model.PreferedLanguage
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

                        var result = await _userManager.FindByEmailAsync(model.Email);
                        //// added by nirav savariya for register log user wise on 12-05-2018

                        OtpMasterViewModel OptData = await _otpMasterService.AddOtpForSignupuser(result.Id, result.Email);
                        if (OptData == null)
                        {
                            return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserRegisterError, ErrorCode = enErrorCode.Status4102SignUpUserRegisterError });
                        }

                        var SignUplog = new SignUpLogViewModel()
                        {
                            TempUserId = Convert.ToInt32(result.Id),
                            RegisterType = await _registerTypeService.GetRegisterId(enRegisterType.Email),
                            Device = model.DeviceId,
                            Mode = model.Mode,
                            IpAddress = model.IPAddress,
                            Location = ipmodel != null ? ipmodel.Location : string.Empty,
                            HostName = model.HostName,
                            RegisterStatus = false
                        };
                        _IsignupLogService.AddSignUpLog(SignUplog);

                        // Need to Change here. -Nishit Jani on A 2019-07-06 5:29 PM
                        var resultotp = await _otpMasterService.GetOtpData(Convert.ToInt32(OptData.UserId), 1);
                        //SendEmailRequest requestemail = new SendEmailRequest();
                        //requestemail.Recepient = model.Email;

                        //IQueryable ResultEamil = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.SignupEmailWithOTP), 0);
                        //foreach (TemplateMasterData Provider in ResultEamil)
                        //{
                        //    if (!string.IsNullOrEmpty(result.UserName))
                        //        Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), result.UserName);
                        //    else
                        //        Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), string.Empty);
                        //    Provider.Content = Provider.Content.Replace("###Password###".ToUpper(), resultotp.OTP);
                        //    requestemail.Body = Provider.Content;
                        //    requestemail.Subject = Provider.AdditionalInfo;
                        //}
                        //_pushNotificationsQueue.Enqueue(requestemail);

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
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserRegisterError, ErrorCode = enErrorCode.Status9033SignUpUserRegisterError });
                }
                else if (!string.IsNullOrEmpty(model.Mobile) && string.IsNullOrEmpty(model.Email))
                {

                    //////////check mobile valid or not
                    if (!string.IsNullOrEmpty(model.Mobile) && !string.IsNullOrEmpty(model.CountryCode))
                    {
                        bool isValidNumber = await _userdata.IsValidPhoneNumber(model.Mobile, model.CountryCode);
                        if (!isValidNumber)
                        {
                            return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardSignUpPhonevalid, ErrorCode = enErrorCode.Status9034MobileInvalid });
                        }
                    }

                    ///////////////// Check bizUser  table in username  Exist or not
                    var resultMobileUserName = await _userManager.FindByNameAsync(model.Mobile);
                    if (resultMobileUserName != null)
                    {
                        if (!string.IsNullOrEmpty(resultMobileUserName?.UserName))
                        {
                            if (resultMobileUserName.PhoneNumberConfirmed)
                                return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status9035SignUPMobileValidation });
                            else
                                return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUpTempUserMobileExistAndVerificationPending, ErrorCode = enErrorCode.Status9036VerifyPending });

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
                        PreferedLanguage = model.PreferedLanguage
                    };

                    var UserCreate = await _userManager.CreateAsync(currentUser);

                    //  var result = await _tempUserRegisterService.AddTempRegister(tempcurrentUser);
                    if (UserCreate.Succeeded)
                    {
                        if (currentUser.Mobile != null)
                        {
                            var officeClaim = new Claim(OpenIdConnectConstants.Claims.PhoneNumber, currentUser.Mobile.ToString(), ClaimValueTypes.Integer);
                            await _userManager.AddClaimAsync(currentUser, officeClaim);
                        }
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
                        var resultUserName = await _userManager.FindByNameAsync(model.Mobile);
                        OtpMasterViewModel OptData = await _otpMasterService.AddOtpForSignupuser(resultUserName.Id, null, model.Mobile);

                        SendSMSRequest request = new SendSMSRequest();
                        request.MobileNo = Convert.ToInt64(model.Mobile);
                        request.Message = EnResponseMessage.SendMailBody + OptData.OTP;
                        _pushSMSQueue.Enqueue(request);
                        return Ok(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUpWithMobile });
                    }
                    else
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserRegisterError, ErrorCode = enErrorCode.Status9033SignUpUserRegisterError });
                }
                else if (string.IsNullOrEmpty(model.Mobile) && string.IsNullOrEmpty(model.Email))
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackofficeEmailoPhone, ErrorCode = enErrorCode.Status9041EmailOrPhone });
                else if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Mobile) && !string.IsNullOrEmpty(model.Username))
                {

                    //////////check mobile valid or not
                    if (!string.IsNullOrEmpty(model.Mobile) && !string.IsNullOrEmpty(model.CountryCode))
                    {
                        bool isValidNumber = await _userdata.IsValidPhoneNumber(model.Mobile, model.CountryCode);
                        if (!isValidNumber)
                        {
                            return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardSignUpPhonevalid, ErrorCode = enErrorCode.Status9034MobileInvalid });
                        }
                    }
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
                    //////////////////// Check bizUser  table in Email Exist or not
                    var resultUserName = await _userManager.FindByNameAsync(model.Username);
                    if (!string.IsNullOrEmpty(resultUserName?.UserName))
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserNameExist, ErrorCode = enErrorCode.Status4099BizUserNameExist });
                    }
                    var resultMobileUserName = await _userManager.FindByNameAsync(model.Mobile);
                    if (resultMobileUserName != null)
                    {
                        if (resultMobileUserName.PhoneNumberConfirmed)
                        {
                            return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status4074SignUPMobileValidation });
                        }
                        else
                        {
                            return Ok(new BackOfficeUserResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUpTempUserMobileExistAndVerificationPending, ErrorCode = enErrorCode.Status9036VerifyPending });
                        }
                    }
                    var resultMobile = await _userManager.FindByNameAsync(model.Mobile);
                    if (!string.IsNullOrEmpty(resultMobile?.UserName))
                    {
                        //return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserNameAs_a_MobileExist, ErrorCode = enErrorCode.Status4103BizUserNameAs_a_MobileExist });
                        return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status9035SignUPMobileValidation });
                    }


                    byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                    string Password = _userdata.GenerateRandomPassword();
                    var currentUser = new ApplicationUser
                    {
                        UserName = model.Username,
                        Email = model.Email,
                        FirstName = model.Firstname,
                        LastName = model.Lastname,
                        Mobile = model.Mobile,
                        PasswordHash = Password,
                        RegTypeId = await _registerTypeService.GetRegisterId(enRegisterType.Standerd),
                        CountryCode = model.CountryCode,
                        CreatedDate = DateTime.UtcNow,
                        PreferedLanguage = model.PreferedLanguage
                    };
                    var resultdata = await _userManager.CreateAsync(currentUser, currentUser.PasswordHash);

                    if (resultdata.Succeeded)
                    {
                        var Currentuser = await _userManager.FindByNameAsync(model.Username);

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
                        linkToken.Firstname = model.Firstname != string.Empty ? model.Firstname : string.Empty;
                        linkToken.Lastname = model.Lastname != string.Empty ? model.Lastname : string.Empty;
                        linkToken.Mobile = model.Mobile;
                        linkToken.CurrentTime = DateTime.UtcNow;
                        linkToken.Expirytime = DateTime.UtcNow + TimeSpan.FromHours(2);
                        linkToken.CountryCode = model.CountryCode;
                        linkToken.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                        linkToken.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                        linkToken.DeviceID = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                        linkToken.IpAddress = model.IPAddress;
                        linkToken.Password = Password;

                        string UserDetails = JsonConvert.SerializeObject(linkToken);
                        string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);

                        byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
                        string ctokenlink = _configuration["ConfirmMailURL"].ToString() + Convert.ToBase64String(plainTextBytes);

                        //SendEmailRequest request = new SendEmailRequest();
                        //request.Recepient = model.Email;

                        //IQueryable Result = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.ConfirmationMail), 0);
                        //foreach (TemplateMasterData Provider in Result)
                        //{
                        //    Provider.Content = Provider.Content.Replace("###Link###".ToUpper(), ctokenlink);
                        //    if (!string.IsNullOrEmpty(Currentuser.UserName))
                        //        Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), Currentuser.UserName);
                        //    else
                        //        Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), string.Empty);
                        //    request.Body = Provider.Content;
                        //    request.Subject = Provider.AdditionalInfo;
                        //}

                        //_pushNotificationsQueue.Enqueue(request);
                        // khushali 31-01-2019 for Common Template Method call 
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
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserRegisterError, ErrorCode = enErrorCode.Status9033SignUpUserRegisterError });
                    }
                }
                else
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackofficeEmailoPhone, ErrorCode = enErrorCode.Status9041EmailOrPhone });
            }
            catch (Exception ex)
            {
                return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("BackOfficeReSendRegisterlink")]
        [AllowAnonymous]
        public async Task<IActionResult> BackOfficeReSendRegisterlink(SignUpWithEmailViewModel model)
        {
            try
            {
                var language = _LanguagePreferenceMaster.GetSingle(i => i.PreferedLanguage == model.PreferedLanguage);
                if (language == null)
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidLang, ErrorCode = enErrorCode.InvalidLang });
                }

                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status9029IpInvalid });
                }
                var result = await _userManager.FindByEmailAsync(model.Email);
                if (string.IsNullOrEmpty(result?.Email))
                {
                    var tempdata = await _tempUserRegisterService.GetEmailDet(model.Email);
                    if (tempdata != null)
                    {
                        if (!tempdata.RegisterStatus)
                        {
                            byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                            string[] DeviceDetails = null;
                            if (model.DeviceId.Contains('|'))
                                DeviceDetails = model.DeviceId.Split('|');

                            LinkTokenViewModel linkToken = new LinkTokenViewModel();
                            linkToken.Id = tempdata.Id;
                            linkToken.Username = tempdata.UserName;
                            linkToken.Email = model.Email;
                            linkToken.CurrentTime = DateTime.UtcNow;
                            linkToken.Expirytime = DateTime.UtcNow + TimeSpan.FromHours(2);
                            //linkToken.Password = tempdata.PasswordHash;
                            linkToken.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                            linkToken.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                            linkToken.DeviceID = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                            linkToken.IpAddress = model.IPAddress;

                            string UserDetails = JsonConvert.SerializeObject(linkToken);
                            string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);

                            byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
                            string ctokenlink = _configuration["ConfirmMailURL"].ToString() + Convert.ToBase64String(plainTextBytes);

                            //SendEmailRequest request = new SendEmailRequest();
                            //request.Recepient = model.Email;

                            //IQueryable Result = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.ConfirmationMail), 0);
                            //foreach (TemplateMasterData Provider in Result)
                            //{
                            //    Provider.Content = Provider.Content.Replace("###Link###".ToUpper(), ctokenlink);
                            //    if (!string.IsNullOrEmpty(tempdata.UserName))
                            //        Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), tempdata.UserName);
                            //    else
                            //        Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), string.Empty);
                            //    request.Body = Provider.Content;
                            //    request.Subject = Provider.AdditionalInfo;
                            //}
                            //_pushNotificationsQueue.Enqueue(request);

                            //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                            // khushali 31-01-2019 for Common Template Method call 
                            TemplateMasterData TemplateData = new TemplateMasterData();
                            CommunicationParamater communicationParamater = new CommunicationParamater();
                            SendEmailRequest request = new SendEmailRequest();
                            if (!string.IsNullOrEmpty(tempdata.UserName))
                                communicationParamater.Param1 = tempdata.UserName;
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

                            return Ok(new RegisterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardResendSignUp });
                        }
                        else
                        {
                            return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpValidation, ErrorCode = enErrorCode.Status9051UseralreadRegister });
                        }
                    }
                    else
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status9046NotFoundRecored });
                    }
                }
                else
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status9046NotFoundRecored });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("BackOfficeConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> BackOfficeConfirmEmail(string emailConfirmCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailConfirmCode))
                {
                    byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                    var bytes = Convert.FromBase64String(emailConfirmCode);
                    var encodedString = Encoding.UTF8.GetString(bytes);
                    string DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);

                    LinkTokenViewModel dmodel = JsonConvert.DeserializeObject<LinkTokenViewModel>(DecryptToken);
                    if (dmodel?.Expirytime >= DateTime.UtcNow)
                    {
                        if (dmodel.Id == 0)
                        {
                            return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignEmailUser, ErrorCode = enErrorCode.Status9042NotFoundRecored });
                        }
                        else
                        {
                            var user = await _tempUserRegisterService.FindById(dmodel.Id);
                            if (user == null)
                            {
                                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignEmailUser, ErrorCode = enErrorCode.Status9042NotFoundRecored });
                            }
                            else if (!user.RegisterStatus)
                            {
                                string Password = _userdata.GenerateRandomPassword();
                                var currentUser = new ApplicationUser
                                {
                                    UserName = user.UserName,
                                    Email = user.Email,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    Mobile = user.Mobile,
                                    //PasswordHash = EncyptedDecrypted.Decrypt(user.PasswordHash, DecpasswordBytes),
                                    PasswordHash = Password,
                                    CountryCode = user.CountryCode,
                                    CreatedDate = DateTime.UtcNow,
                                    PreferedLanguage= user.PreferedLanguage
                                };
                                var result = await _userManager.CreateAsync(currentUser, currentUser.PasswordHash);
                                if (result.Succeeded)
                                {
                                    if (currentUser.Mobile != null)
                                    {
                                        var officeClaim = new Claim(OpenIdConnectConstants.Claims.PhoneNumber, currentUser.Mobile.ToString(), ClaimValueTypes.Integer);
                                        await _userManager.AddClaimAsync(currentUser, officeClaim);
                                    }
                                    // Add to roles
                                    var roleAddResult = await _userManager.AddToRoleAsync(currentUser, "User");
                                    if (roleAddResult.Succeeded)
                                    {
                                        currentUser.EmailConfirmed = true;
                                        var resultupdate = await _userManager.UpdateAsync(currentUser);
                                        _tempUserRegisterService.Update(user.Id);

                                        //// added by nirav savariya for create profile subscription plan on 11-04-2018
                                        SubscriptionViewModel subscriptionmodel = new SubscriptionViewModel()
                                        {
                                            UserId = currentUser.Id
                                        };
                                        _IsubscriptionMaster.AddSubscription(subscriptionmodel);

                                        ///   define the wallet services..
                                        //_IwalletService.CreateDefaulWallet(currentUser.Id);
                                        WalletReqRes walletReq = new WalletReqRes();
                                        walletReq.UserId = currentUser.Id;
                                        _IcreateWalletQueue.Enqueue(walletReq);
                                        //_mediator.Send(walletReq);


                                        //// added by nirav savariya for verify user status on 12-05-2015
                                        _IsignupLogService.UpdateVerifiedUser(Convert.ToInt32(user.Id), currentUser.Id);

                                        ///// added by nirav savariya for devicewhitelisting on 12-06-2018
                                        DeviceMasterViewModel model = new DeviceMasterViewModel();
                                        model.Device = dmodel.Device;
                                        model.DeviceOS = dmodel.DeviceOS;
                                        model.DeviceId = dmodel.DeviceID;
                                        model.UserId = Convert.ToInt32(currentUser.Id);
                                        _IdeviceIdService.AddDeviceProcess(model);

                                        ///// added by nirav savariya for ipwhitelist on 12-07-2018
                                        IpMasterViewModel ipmodel = new IpMasterViewModel();
                                        ipmodel.IpAddress = dmodel.IpAddress;
                                        ipmodel.UserId = Convert.ToInt32(currentUser.Id);
                                        _iipAddressService.AddIpAddress(ipmodel);


                                        //SendEmailRequest requestemail = new SendEmailRequest();
                                        //requestemail.Recepient = currentUser.Email;
                                        //IQueryable ResultEmail = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.Registration), 0);
                                        //foreach (TemplateMasterData Provider in ResultEmail)
                                        //{
                                        //    Provider.Content = Provider.Content.Replace("###Firstname###".ToUpper(), currentUser.FirstName);
                                        //    Provider.Content = Provider.Content.Replace("###Lastname###".ToUpper(), currentUser.LastName);
                                        //    Provider.Content = Provider.Content.Replace("###Username###".ToUpper(), currentUser.UserName);
                                        //    Provider.Content = Provider.Content.Replace("###Emailaddress###".ToUpper(), currentUser.Email);
                                        //    Provider.Content = Provider.Content.Replace("###Mobile###".ToUpper(), currentUser.Mobile);
                                        //    Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), currentUser.UserName);
                                        //    requestemail.Body = Provider.Content;
                                        //    requestemail.Subject = Provider.AdditionalInfo;
                                        //}
                                        //_pushNotificationsQueue.Enqueue(requestemail);


                                        //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                                        // khushali 30-01-2019 for Common Template Method call 
                                        TemplateMasterData TemplateData = new TemplateMasterData();
                                        CommunicationParamater communicationParamater = new CommunicationParamater();
                                        SendEmailRequest request = new SendEmailRequest();
                                        communicationParamater.Param1 = currentUser.UserName; //Username
                                        communicationParamater.Param2 = currentUser.FirstName; //FirstName
                                        communicationParamater.Param3 = currentUser.LastName; //LastName
                                        communicationParamater.Param4 = currentUser.Email; //Email
                                        communicationParamater.Param5 = currentUser.Mobile; //Mobile
                                        TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.Registration, communicationParamater, enCommunicationServiceType.Email).Result;
                                        if (TemplateData != null)
                                        {
                                            if (TemplateData.IsOnOff == 1)
                                            {
                                                request.Recepient = currentUser.Email;
                                                request.Body = TemplateData.Content;
                                                request.Subject = TemplateData.AdditionalInfo;
                                                _pushNotificationsQueue.Enqueue(request);
                                            }
                                        }

                                        ///User Primary Email Define
                                        ///
                                        EmailMasterReqViewModel emailMasterReqViewModel = new EmailMasterReqViewModel();
                                        {
                                            emailMasterReqViewModel.Email = currentUser.Email;
                                            emailMasterReqViewModel.IsPrimary = true;
                                            emailMasterReqViewModel.Userid = currentUser.Id;
                                        }
                                        Guid emailid = _IemailMaster.Add(emailMasterReqViewModel);

                                        ///  Create the Primary Phone Number Define
                                        if (!string.IsNullOrEmpty(currentUser.Mobile))
                                        {
                                            PhoneMasterReqViewModel phoneMasterReqViewModel = new PhoneMasterReqViewModel();
                                            phoneMasterReqViewModel.IsPrimary = true;
                                            phoneMasterReqViewModel.MobileNumber = currentUser.Mobile;
                                            phoneMasterReqViewModel.Userid = currentUser.Id;
                                            Guid phoneID = _Iphonemaster.Add(phoneMasterReqViewModel);
                                        }

                                        //SendEmailRequest requestdata = new SendEmailRequest();
                                        //requestdata.Recepient = currentUser.Email;

                                        //IQueryable ResultData = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.LoginPassword), 0);
                                        //foreach (TemplateMasterData Provider in ResultData)
                                        //{
                                        //    Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), currentUser.Email);
                                        //    Provider.Content = Provider.Content.Replace("###Password###".ToUpper(), Password);
                                        //    requestdata.Body = Provider.Content;
                                        //    requestdata.Subject = Provider.AdditionalInfo;
                                        //}
                                        //_pushNotificationsQueue.Enqueue(requestdata);

                                        //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                                        // khushali 31-01-2019 for Common Template Method call 
                                        TemplateMasterData TemplateData1 = new TemplateMasterData();
                                        CommunicationParamater communicationParamater1 = new CommunicationParamater();
                                        SendEmailRequest request1 = new SendEmailRequest();
                                        communicationParamater1.Param1 = currentUser.Email;
                                        communicationParamater1.Param2 = Password;
                                        TemplateData1 = _messageService.ReplaceTemplateMasterData(EnTemplateType.LoginPassword, communicationParamater1, enCommunicationServiceType.Email).Result;
                                        if (TemplateData1 != null)
                                        {
                                            if (TemplateData1.IsOnOff == 1)
                                            {
                                                request1.Recepient = currentUser.Email;
                                                request1.Body = TemplateData1.Content;
                                                request1.Subject = TemplateData1.AdditionalInfo;
                                                _pushNotificationsQueue.Enqueue(request1);
                                            }
                                        }

                                        return Ok(new RegisterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUpEmailConfirm });
                                    }
                                }
                                else
                                {
                                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserNotRegister, ErrorCode = enErrorCode.Status9043UserNotRegister });
                                }
                            }
                            else
                            {
                                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpEmailValidation, ErrorCode = enErrorCode.Status9032UseralreadRegister });
                            }
                        }
                    }
                    else
                    {
                        return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpEmailExpired, ErrorCode = enErrorCode.Status9044ResetPasswordLinkExpired });
                    }
                }
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignEmailLink, ErrorCode = enErrorCode.Status9045EmailLinkBlanck });
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("BackOfficeReSendOtpWithEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> BackOfficeReSendOtpWithEmail(SignUpWithEmailViewModel model)
        {
            try
            {
                var language = _LanguagePreferenceMaster.GetSingle(i => i.PreferedLanguage == model.PreferedLanguage);
                if (language == null)
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidLang, ErrorCode = enErrorCode.InvalidLang });
                }

                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status9029IpInvalid });

                var result = await _userManager.FindByEmailAsync(model.Email);
                if (string.IsNullOrEmpty(result?.Email))
                {
                    var tempdata = await _tempUserRegisterService.GetEmailDet(model.Email);
                    var tempotp = await _tempOtpService.GetTempData(Convert.ToInt16(tempdata?.Id));
                    //if (!tempdata.RegisterStatus && !tempotp.EnableStatus && tempotp.ExpirTime <= DateTime.UtcNow)  // Remove expiretime as per discuss with nishit bhai 10-09-2018
                    if (tempotp != null && tempdata != null)
                    {
                        if (!tempdata.RegisterStatus && !tempotp.EnableStatus)
                        {
                            _tempOtpService.Update(tempotp.Id);
                            var resultdata = await _tempOtpService.AddTempOtp(Convert.ToInt16(tempdata.Id), Convert.ToInt16(enRegisterType.Email));

                            //SendEmailRequest request = new SendEmailRequest();
                            //request.Recepient = model.Email;
                            // request.Subject = EnResponseMessage.ReSendMailSubject;
                            // request.Body = EnResponseMessage.SendMailBody + resultdata.OTP;


                            //IQueryable Result = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.SignupEmailWithOTP), 0);                            //foreach (TemplateMasterData Provider in Result)
                            //{
                            //    if (!string.IsNullOrEmpty(tempdata.UserName))
                            //        Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), tempdata.UserName);
                            //    else
                            //        Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), string.Empty);
                            //    Provider.Content = Provider.Content.Replace("###Password###".ToUpper(), resultdata.OTP);
                            //    request.Body = Provider.Content;
                            //    request.Subject = Provider.AdditionalInfo;
                            //}
                            //_pushNotificationsQueue.Enqueue(request); 

                            //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                            // khushali 31-01-2019 for Common Template Method call 
                            TemplateMasterData TemplateData = new TemplateMasterData();
                            CommunicationParamater communicationParamater = new CommunicationParamater();
                            SendEmailRequest request = new SendEmailRequest();
                            if (!string.IsNullOrEmpty(tempdata.UserName))
                                communicationParamater.Param1 = tempdata.UserName;
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
                            return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpEmailValidation, ErrorCode = enErrorCode.Status9032UseralreadRegister });
                        }
                    }
                    else
                    {
                        return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status9046NotFoundRecored });
                    }
                }
                else
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status9046NotFoundRecored });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("BackOfficeReSendOtpWithMobile")]
        [AllowAnonymous]
        public async Task<IActionResult> BackOfficeReSendOtpWithMobile(SignUpWithMobileViewModel model)
        {
            try
            {
                var language = _LanguagePreferenceMaster.GetSingle(i => i.PreferedLanguage == model.PreferedLanguage);
                if (language == null)
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidLang, ErrorCode = enErrorCode.InvalidLang });
                }

                string CountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(CountryCode) && CountryCode == "fail")
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status9029IpInvalid });
                }


                bool isValidNumber = await _userdata.IsValidPhoneNumber(model.Mobile, model.CountryCode);

                if (!isValidNumber)
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardSignUpPhonevalid, ErrorCode = enErrorCode.Status9034MobileInvalid });
                }

                bool IsSignMobile = _userdata.GetMobileNumber(model.Mobile);
                if (IsSignMobile)
                {
                    var tempdata = await _tempUserRegisterService.GetMobileNo(model.Mobile);
                    var tempotp = await _tempOtpService.GetTempData(Convert.ToInt16(tempdata?.Id));
                    //if (!tempdata.RegisterStatus && !tempotp.EnableStatus && tempotp.ExpirTime <= DateTime.UtcNow) // Remove expiretime as per discuss with nishit bhai 10-09-2018
                    if (tempdata != null && tempotp != null)
                    {
                        if (!tempdata.RegisterStatus && !tempotp.EnableStatus)
                        {
                            _tempOtpService.Update(tempotp.Id);
                            var result = await _tempOtpService.AddTempOtp(Convert.ToInt16(tempdata.Id), Convert.ToInt16(enRegisterType.Mobile));

                            SendSMSRequest request = new SendSMSRequest();
                            request.MobileNo = Convert.ToInt64(model.Mobile);
                            request.Message = EnResponseMessage.SendSMSSubject + result.OTP;
                            _pushSMSQueue.Enqueue(request); //24-11-2018 komal make SMS Enqueue
                            //await _mediator.Send(request);
                            return Ok(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUpWithResendMobile });
                        }
                        else
                        {
                            return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status9035SignUPMobileValidation });
                        }
                    }
                    else
                    {
                        return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status9046NotFoundRecored });
                    }
                }
                else
                {
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status9035SignUPMobileValidation });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("BackOfficeMobileOtpVerification")]
        [AllowAnonymous]
        public async Task<IActionResult> BackOfficeMobileOtpVerification(OTPWithMobileViewModel model)
        {
            try
            {
                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status9029IpInvalid });
                }
                var tempdata = await _tempUserRegisterService.GetMobileNo(model.Mobile);
                if (tempdata?.Id > 0)
                {
                    var tempotp = await _tempOtpService.GetTempData(Convert.ToInt16(tempdata.Id));
                    if (tempotp != null)
                    {
                        if (tempotp?.ExpirTime >= DateTime.UtcNow)
                        {
                            if (tempdata.Id == 0 && tempotp.Id == 0)
                            {
                                return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status9046NotFoundRecored });
                            }
                            else if (model.OTP == tempotp.OTP)
                            {
                                if (!tempdata.RegisterStatus && !tempotp.EnableStatus)
                                {
                                    var currentUser = new ApplicationUser
                                    {
                                        UserName = tempdata.Mobile,
                                        Mobile = tempdata.Mobile,
                                        CountryCode = tempdata.CountryCode,
                                        CreatedDate = DateTime.UtcNow,
                                        PhoneNumberConfirmed = true,
                                        PreferedLanguage=tempdata.PreferedLanguage
                                    };
                                    var result = await _userManager.CreateAsync(currentUser);
                                    if (result.Succeeded)
                                    {
                                        if (currentUser.Mobile != null)
                                        {
                                            var officeClaim = new Claim(OpenIdConnectConstants.Claims.PhoneNumber, currentUser.Mobile.ToString(), ClaimValueTypes.Integer);
                                            await _userManager.AddClaimAsync(currentUser, officeClaim);
                                        }
                                        // Add to roles
                                        var roleAddResult = await _userManager.AddToRoleAsync(currentUser, "User");
                                        if (roleAddResult.Succeeded)
                                        {
                                            _tempUserRegisterService.Update(tempdata.Id);
                                            _tempOtpService.Update(tempotp.Id);
                                            var mobileconfirmed = await _userManager.IsPhoneNumberConfirmedAsync(currentUser);

                                            WalletReqRes walletReq = new WalletReqRes();
                                            walletReq.UserId = currentUser.Id;
                                            _IcreateWalletQueue.Enqueue(walletReq);

                                            //// added by nirav savariya for verify user status on 12-05-2015
                                            _IsignupLogService.UpdateVerifiedUser(Convert.ToInt32(tempdata.Id), currentUser.Id);

                                            string[] DeviceDetails = null;
                                            if (model.DeviceId.Contains('|'))
                                                DeviceDetails = model.DeviceId.Split('|');

                                            DeviceMasterViewModel devicemodel = new DeviceMasterViewModel();
                                            devicemodel.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                                            devicemodel.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                                            devicemodel.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                                            devicemodel.UserId = Convert.ToInt32(currentUser.Id);
                                            _IdeviceIdService.AddDeviceProcess(devicemodel);

                                            ///// added by nirav savariya for ipwhitelist on 12-07-2018
                                            IpMasterViewModel ipmodel = new IpMasterViewModel();
                                            ipmodel.IpAddress = model.IPAddress;
                                            ipmodel.UserId = Convert.ToInt32(currentUser.Id);
                                            _iipAddressService.AddIpAddress(ipmodel);


                                            //// added by nirav savariya for create profile subscription plan on 11-04-2018
                                            SubscriptionViewModel subscriptionmodel = new SubscriptionViewModel()
                                            {
                                                UserId = currentUser.Id
                                            };
                                            _IsubscriptionMaster.AddSubscription(subscriptionmodel);
                                            /// Primary Mobile Register
                                            PhoneMasterReqViewModel phoneMasterReqViewModel = new PhoneMasterReqViewModel();
                                            phoneMasterReqViewModel.IsPrimary = true;
                                            phoneMasterReqViewModel.MobileNumber = currentUser.Mobile;
                                            phoneMasterReqViewModel.Userid = currentUser.Id;
                                            Guid phoneID = _Iphonemaster.Add(phoneMasterReqViewModel);
                                            return Ok(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUPVerification });
                                        }
                                        else
                                        {
                                            return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpRole, ErrorCode = enErrorCode.Status9047SignUpRole });
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status9035SignUPMobileValidation });
                                    }
                                }
                                else
                                {
                                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUPMobileValidation, ErrorCode = enErrorCode.Status9035SignUPMobileValidation });
                                }
                            }
                            else
                            {
                                return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpOTP, ErrorCode = enErrorCode.Status9048SignUPOTP });
                            }
                        }
                        else
                        {
                            return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpResendOTP, ErrorCode = enErrorCode.Status9049SignUpReSendOTP });
                        }
                    }
                    else
                    {
                        return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status9046NotFoundRecored });
                    }
                }
                else
                {
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status9046NotFoundRecored });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("BackOfficeEmailOtpVerification")]
        [AllowAnonymous]
        public async Task<IActionResult> BackOfficeEmailOtpVerification(OTPWithEmailViewModel model)
        {
            try
            {
                var language = _LanguagePreferenceMaster.GetSingle(i => i.PreferedLanguage == model.PreferedLanguage);
                if (language == null)
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidLang, ErrorCode = enErrorCode.InvalidLang });
                }

                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status9029IpInvalid });
                }
                var tempdata = await _tempUserRegisterService.GetEmailDet(model.Email);
                if (tempdata?.Id > 0)
                {
                    var tempotp = await _tempOtpService.GetTempData(Convert.ToInt16(tempdata.Id));
                    if (tempotp != null)
                    {
                        if (tempotp?.ExpirTime >= DateTime.UtcNow)
                        {
                            if (tempdata.Id == 0 && tempotp.Id == 0)
                            {
                                return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status9046NotFoundRecored });
                            }
                            else if (model.OTP == tempotp.OTP)
                            {
                                if (!tempdata.RegisterStatus && !tempotp.EnableStatus)
                                {
                                    var currentUser = new ApplicationUser
                                    {
                                        UserName = tempdata.Email,
                                        Email = tempdata.Email,
                                        CreatedDate = DateTime.UtcNow,
                                        EmailConfirmed = true,
                                        PreferedLanguage = model.PreferedLanguage,
                                    };
                                    var result = await _userManager.CreateAsync(currentUser);
                                    if (result.Succeeded)
                                    {
                                        if (currentUser.Mobile != null)
                                        {
                                            var officeClaim = new Claim(OpenIdConnectConstants.Claims.PhoneNumber, currentUser.Mobile.ToString(), ClaimValueTypes.Integer);
                                            await _userManager.AddClaimAsync(currentUser, officeClaim);
                                        }
                                        // Add to roles
                                        var roleAddResult = await _userManager.AddToRoleAsync(currentUser, "User");
                                        if (roleAddResult.Succeeded)
                                        {
                                            _tempUserRegisterService.Update(tempdata.Id);
                                            _tempOtpService.Update(tempotp.Id);
                                            var emailconfirmed = await _userManager.IsEmailConfirmedAsync(currentUser);
                                            ///   define the wallet services..

                                            //_IwalletService.CreateDefaulWallet(currentUser.Id);
                                            WalletReqRes walletReq = new WalletReqRes();
                                            walletReq.UserId = currentUser.Id;
                                            _IcreateWalletQueue.Enqueue(walletReq);
                                            //Task.Run(() => _mediator.Send(walletReq));
                                            //_mediator.Send(walletReq);
                                            //// added by nirav savariya for verify user status on 12-05-2015
                                            _IsignupLogService.UpdateVerifiedUser(Convert.ToInt32(tempdata.Id), currentUser.Id);

                                            string[] DeviceDetails = null;
                                            if (model.DeviceId.Contains('|'))
                                                DeviceDetails = model.DeviceId.Split('|');

                                            DeviceMasterViewModel devicemodel = new DeviceMasterViewModel();
                                            devicemodel.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                                            devicemodel.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                                            devicemodel.DeviceId = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                                            devicemodel.UserId = Convert.ToInt32(currentUser.Id);
                                            _IdeviceIdService.AddDeviceProcess(devicemodel);

                                            ///// added by nirav savariya for ipwhitelist on 12-07-2018
                                            IpMasterViewModel ipmodel = new IpMasterViewModel();
                                            ipmodel.IpAddress = model.IPAddress;
                                            ipmodel.UserId = Convert.ToInt32(currentUser.Id);
                                            _iipAddressService.AddIpAddress(ipmodel);

                                            //// added by nirav savariya for create profile subscription plan on 11-04-2018
                                            SubscriptionViewModel subscriptionmodel = new SubscriptionViewModel()
                                            {
                                                UserId = currentUser.Id
                                            };
                                            _IsubscriptionMaster.AddSubscription(subscriptionmodel);


                                            //SendEmailRequest request = new SendEmailRequest();
                                            //request.Recepient = currentUser.Email;
                                            //IQueryable Result = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.EmailRegistration), 0);
                                            //foreach (TemplateMasterData Provider in Result)
                                            //{

                                            //    Provider.Content = Provider.Content.Replace("###Username###".ToUpper(), currentUser.UserName);
                                            //    Provider.Content = Provider.Content.Replace("###Emailaddress###".ToUpper(), currentUser.Email);
                                            //    Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), currentUser.UserName);
                                            //    request.Body = Provider.Content;
                                            //    request.Subject = Provider.AdditionalInfo;
                                            //}
                                            //_pushNotificationsQueue.Enqueue(request);

                                            //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                                            // khushali 30-01-2019 for Common Template Method call 
                                            TemplateMasterData TemplateData = new TemplateMasterData();
                                            CommunicationParamater communicationParamater = new CommunicationParamater();
                                            SendEmailRequest request = new SendEmailRequest();
                                            communicationParamater.Param1 = currentUser.UserName; //Username
                                            communicationParamater.Param2 = currentUser.Email; //Email
                                            TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EmailRegistration, communicationParamater, enCommunicationServiceType.Email).Result;
                                            if (TemplateData != null)
                                            {
                                                if (TemplateData.IsOnOff == 1)
                                                {
                                                    request.Recepient = currentUser.Email;
                                                    request.Body = TemplateData.Content;
                                                    request.Subject = TemplateData.AdditionalInfo;
                                                    _pushNotificationsQueue.Enqueue(request);
                                                }
                                            }

                                            EmailMasterReqViewModel emailMasterReqViewModel = new EmailMasterReqViewModel();
                                            {
                                                emailMasterReqViewModel.Email = currentUser.Email;
                                                emailMasterReqViewModel.IsPrimary = true;
                                                emailMasterReqViewModel.Userid = currentUser.Id;
                                            }
                                            Guid emailid = _IemailMaster.Add(emailMasterReqViewModel);


                                            return Ok(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SignUPVerification });
                                        }
                                        else
                                        {
                                            return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpRole, ErrorCode = enErrorCode.Status9047SignUpRole });
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserNotRegister, ErrorCode = enErrorCode.Status9043UserNotRegister });
                                    }
                                }
                                else
                                {
                                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpEmailValidation, ErrorCode = enErrorCode.Status9032UseralreadRegister });
                                }
                            }
                            else
                            {
                                return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpOTP, ErrorCode = enErrorCode.Status9048SignUPOTP });
                            }
                        }
                        else
                        {
                            return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpResendOTP, ErrorCode = enErrorCode.Status9049SignUpReSendOTP });
                        }
                    }
                    else
                    {
                        return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status9046NotFoundRecored });
                    }
                }
                else
                {
                    return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status9046NotFoundRecored });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new SignUpWithEmailResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        #endregion

        #region Helpers

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion

        //[HttpGet("ModuleuserAssign")]
        //public async Task<IActionResult> ModuleuserAssign()
        //{
        //    ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //    var Data = await _ruleManageService.GetAssignedRuleUserWise(Convert.ToInt64(user.Id));

        //    return Ok(Data);
        //}
    }
}