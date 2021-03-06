using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Affiliate;
using Worldex.Core.Interfaces.EmailMaster;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.Interfaces.PhoneMaster;
using Worldex.Core.Interfaces.Profile_Management;
using Worldex.Core.Interfaces.User;
using Worldex.Core.Services;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.AccountViewModels.Affiliate;
using Worldex.Core.ViewModels.AccountViewModels.Log;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using Worldex.Core.ViewModels.EmailMaster;
using Worldex.Core.ViewModels.MobileMaster;
using Worldex.Core.ViewModels.Profile_Management;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.DTOClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Worldex.Core.Interfaces.Referral;

namespace Worldex.Web.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AffiliateController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userdata;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IRegisterTypeService _registerTypeService;
        private readonly ISignupLogService _IsignupLogService;
        private readonly IMessageService _messageService;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly IAffiliateService _affiliateService;
        private readonly ISubscriptionMaster _IsubscriptionMaster;
        private readonly ICreateWalletQueue<WalletReqRes> _IcreateWalletQueue;
        private readonly IDeviceIdService _IdeviceIdService;
        private readonly IipAddressService _iipAddressService;
        private readonly IEmailMaster _IemailMaster;
        private readonly Iphonemaster _Iphonemaster;
        private readonly IReferralUser _ReferralUser;


        public AffiliateController(UserManager<ApplicationUser> userManager,
            IUserService userdata,
            EncyptedDecrypted encdecAEC,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            IRegisterTypeService registerTypeService,
            ISignupLogService IsignupLogService,
            IMessageService messageService,
            IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue,
            IAffiliateService affiliateService,
            ISubscriptionMaster IsubscriptionMaster,
            ICreateWalletQueue<WalletReqRes> IcreateWalletQueue,
            IDeviceIdService IdeviceIdService,
            IipAddressService iipAddressService,
            IEmailMaster IemailMaster,
            Iphonemaster Iphonemaster,
            IReferralUser ReferralUser)
        {
            _userManager = userManager;
            _userdata = userdata;
            _encdecAEC = encdecAEC;
            _configuration = configuration;
            _registerTypeService = registerTypeService;
            _IsignupLogService = IsignupLogService;
            _messageService = messageService;
            _pushNotificationsQueue = pushNotificationsQueue;
            _affiliateService = affiliateService;
            _IsubscriptionMaster = IsubscriptionMaster;
            _IcreateWalletQueue = IcreateWalletQueue;
            _IdeviceIdService = IdeviceIdService;
            _iipAddressService = iipAddressService;
            _IemailMaster = IemailMaster;
            _Iphonemaster = Iphonemaster;
            _ReferralUser = ReferralUser;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AffiliateRegister(AffiliateRegisterViewModel model, string passdata = null)
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
                        return Ok(new AffiliateRegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                ////// Ip Address Validate or not
                var ipmodel = await _userdata.GetIPWiseData(model.IPAddress);
                if (!string.IsNullOrEmpty(ipmodel?.CountryCode) && ipmodel?.CountryCode == "fail")
                {
                    return BadRequest(new AffiliateRegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                }

                //////////check mobile valid or not
                bool isValidNumber = await _userdata.IsValidPhoneNumber(model.Mobile, model.CountryCode);
                if (!isValidNumber)
                {
                    return BadRequest(new AffiliateRegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardSignUpPhonevalid, ErrorCode = enErrorCode.Status4013MobileInvalid });
                }

                //////////////////// Check bizUser  table in Email Exist or not
                var result = await _userManager.FindByEmailAsync(model.Email);
                if (result != null)
                {
                    if (result.EmailConfirmed == false)
                    {
                        return BadRequest(new AffiliateRegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpTempUserEmailVerifyPending, ErrorCode = enErrorCode.Status4036VerifyPending });
                    }
                    else
                    {
                        return BadRequest(new AffiliateRegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserEmailExist, ErrorCode = enErrorCode.Status4098BizUserEmailExist });
                    }
                }
                ///////////////// Check bizUser  table in username  Exist or not
                var resultUserName = await _userManager.FindByNameAsync(model.Username);
                if (!string.IsNullOrEmpty(resultUserName?.UserName))
                {
                    return BadRequest(new AffiliateRegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserNameExist, ErrorCode = enErrorCode.Status4099BizUserNameExist });
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

                // Check Scheme Type Is Valid Or Not
                if (model.SchemeType == 0)
                {
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAffliateSchemeType, ErrorCode = enErrorCode.InvalidAffliateSchemeType });
                }
                var SchemeTypeAvailable = _affiliateService.GeAffiliateSchemeType(model.SchemeType);
                if (SchemeTypeAvailable == 0)
                {
                    return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAffliateSchemeType, ErrorCode = enErrorCode.InvalidAffliateSchemeType });
                }

                //Uday 13-02-2019 Check User is Simple Signup Or By Promotion
                long ParentId = 0;
                long PromotionType = 0;
                short UserBit = 0;
                if (!string.IsNullOrEmpty(model.ReferCode)) // Simple Affiliate USer
                {
                    ParentId = _affiliateService.GetAffiliateParentUser(model.ReferCode);

                    if (ParentId == 0)
                    {
                        return BadRequest(new SignUpWithMobileResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAffliateReferCode, ErrorCode = enErrorCode.InvalidAffliateReferCode });
                    }

                    //Check If PassData availabe than decrypte it and based on get the promotion type
                    if (!string.IsNullOrEmpty(passdata))
                    {
                        byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                        var bytes = Convert.FromBase64String(passdata);
                        var encodedString = Encoding.UTF8.GetString(bytes);
                        string DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);

                        AffiliatePromotionLinkViewModel dmodel = JsonConvert.DeserializeObject<AffiliatePromotionLinkViewModel>(DecryptToken);
                        PromotionType = dmodel.PromotionType;  // Particular Promotion Type
                    }
                    else
                    {
                        PromotionType = 5; // Refer Default
                    }
                    UserBit = 2;
                }
                else // Affiliate Promotional User
                {
                    PromotionType = 4; // Default AffiliateUser
                    ParentId = _affiliateService.GetAdminUser(); // Admin User Id
                    UserBit = 1;
                }

                // Add by Akshay  13/12/2019 4:21 pm reson ReferralCode is add in bizuser table during affiliate register
                a:
                string NewReferralCode = _ReferralUser.GenerateRandomReferralCode();
                bool ReferralCodeDupe = _ReferralUser.FindDuplicateReferralCode(NewReferralCode);
                if (ReferralCodeDupe)
                {
                    goto a;
                }

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
                    ReferralCode= NewReferralCode        // add by akshay 13/12/2019 4:22pm reson ReferralCode is add in bizuser table during affiliate register 
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

                    //Add Into AffiliateUserMaster
                    var affiliateUser = _affiliateService.AddAffiliateUser(ParentId, PromotionType, UserBit, model.SchemeType, Currentuser.Id);

                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
                    string ctokenlink = _configuration["ConfirmAffiliateMailURL"].ToString() + Convert.ToBase64String(plainTextBytes);

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
                    return Ok(new AffiliateRegisterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardSignUp });
                }
                else
                {
                    return BadRequest(new AffiliateRegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUserRegisterError, ErrorCode = enErrorCode.Status4102SignUpUserRegisterError });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new AffiliateRegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AffiliateConfirmEmail(string emailConfirmCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailConfirmCode))   ////  Create the standard signup method data check is null or not
                {
                    //add validation LinkData valid or not -mansi 23-09-2019

                    var emailConfirmCode1 = emailConfirmCode.Trim();
                    Boolean ans = (emailConfirmCode1.Length % 4 == 0) && Regex.IsMatch(emailConfirmCode1, @"^[a-zA-Z0-9\+/]*={0,2}$", RegexOptions.None);
                    if (ans == false)
                        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Invalidlinkdata, ErrorCode = enErrorCode.Status1450invalidelinkdata });

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
                                user.Status = 1;
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
                                        WalletReqRes walletReq = new WalletReqRes();
                                        walletReq.UserId = user.Id;
                                        _IcreateWalletQueue.Enqueue(walletReq);

                                        //// added by nirav savariya for verify user status on 12-05-2015
                                        _IsignupLogService.UpdateVerifiedUser(Convert.ToInt32(user.Id), user.Id);

                                        ///// added by nirav savariya for devicewhitelisting on 12-06-2018
                                        DeviceMasterViewModel model = new DeviceMasterViewModel();
                                        model.Device = dmodel.Device;
                                        model.DeviceOS = dmodel.DeviceOS;
                                        model.DeviceId = dmodel.DeviceID;
                                        model.UserId = Convert.ToInt32(user.Id);
                                        _IdeviceIdService.AddDeviceProcess(model);

                                        ///// added by nirav savariya for ipwhitelist on 12-07-2018
                                        IpMasterViewModel ipmodel = new IpMasterViewModel();
                                        ipmodel.IpAddress = dmodel.IpAddress;
                                        ipmodel.UserId = Convert.ToInt32(user.Id);
                                        _iipAddressService.AddIpAddress(ipmodel);


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

                                        //Uday 14-02-2019 Activate The Affiliate Account
                                        _affiliateService.ActiveAffiliateAccount(dmodel.Id);


                                        //Uday 04-03-2019 Add Promotion Link By Default All Active
                                        AddAffiliatePromotionTypeRequest addAffiliatePromotionTypeRequest = new AddAffiliatePromotionTypeRequest();
                                        addAffiliatePromotionTypeRequest.PromotionType = new long[] { 1, 2, 6, 8 };
                                        _affiliateService.AddAffiliatePromotionType(addAffiliatePromotionTypeRequest, user);

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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<GetAffiliatePromotionTypeResponse> GetAffiliatePromotionType()
        {
            try
            {
                GetAffiliatePromotionTypeResponse Response = new GetAffiliatePromotionTypeResponse();

                var PromotionType = _affiliateService.GetAffiliatePromotionType();

                if (PromotionType.Count > 0)
                {
                    Response.Response = PromotionType;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;

                    return Ok(Response);
                }
                else
                {
                    return Ok(new GetAffiliatePromotionTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound, ErrorCode = enErrorCode.NotFound });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new GetAffiliatePromotionTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAffiliatePromotionType(AddAffiliatePromotionTypeRequest Request)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }

                if (Request.PromotionType.Length > 0)
                {
                    var response = _affiliateService.AddAffiliatePromotionType(Request, user);

                    if (response == 1)
                    {
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ErrorCode = enErrorCode.Success;
                        Response.ReturnMsg = EnResponseMessage.CommRecordInsertSuccess;

                        return Ok(Response);
                    }
                    else if (response == 2)
                    {
                        return Ok(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserIsNotAsffiliateUser, ErrorCode = enErrorCode.UserIsNotAsffiliateUser });
                    }
                    else
                    {
                        return Ok(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PromotionInsertFail, ErrorCode = enErrorCode.DataInsertFail });
                    }
                }
                else
                {
                    return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAffiliatePromotionType, ErrorCode = enErrorCode.InvalidAffiliatePromotionType });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{Detail}")]
        [AllowAnonymous]
        public IActionResult GetAffiliateSchemeType(int Detail)
        {
            try
            {
                GetAffiliateSchemeTypeResponse Response1 = new GetAffiliateSchemeTypeResponse();
                GetDetailAffiliateSchemeTypeResponse Response2 = new GetDetailAffiliateSchemeTypeResponse();

                if (Detail == 0)
                {
                    var SchemeType = _affiliateService.GetAffiliateSchemeType();

                    if (SchemeType.Count > 0)
                    {
                        Response1.Response = SchemeType;
                        Response1.ReturnCode = enResponseCode.Success;
                        Response1.ErrorCode = enErrorCode.Success;
                        Response1.ReturnMsg = EnResponseMessage.FindRecored;

                        return Ok(Response1);
                    }
                    else
                    {
                        return Ok(new GetAffiliateSchemeTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound, ErrorCode = enErrorCode.NotFound });
                    }
                }
                else
                {

                    var SchemeDetailType = _affiliateService.GetDetailAffiliateSchemeType();

                    if (SchemeDetailType.Count > 0)
                    {
                        Response2.Response = SchemeDetailType;
                        Response2.ReturnCode = enResponseCode.Success;
                        Response2.ErrorCode = enErrorCode.Success;
                        Response2.ReturnMsg = EnResponseMessage.FindRecored;

                        return Ok(Response2);
                    }
                    else
                    {
                        return Ok(new GetAffiliateSchemeTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound, ErrorCode = enErrorCode.NotFound });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new GetAffiliateSchemeTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ReSendAffiliateRegisterlink(AffiliateSignUpWithEmailViewModel model)
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
                        return Ok(new AffiliateSignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                string IpCountryCode = await _userdata.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(IpCountryCode) && IpCountryCode == "fail")
                {
                    return Ok(new AffiliateSignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });

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
                        string ctokenlink = _configuration["ConfirmAffiliateMailURL"].ToString() + Convert.ToBase64String(plainTextBytes);

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

                        return Ok(new AffiliateSignUpWithEmailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.StandardResendSignUp });
                    }
                    else
                    {
                        return Ok(new AffiliateSignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpValidation, ErrorCode = enErrorCode.Status4062UseralreadRegister });
                    }

                }
                else
                {
                    return Ok(new AffiliateSignUpWithEmailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4033NotFoundRecored });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new AffiliateSignUpWithEmailResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<ActionResult<GetAffiliatePromotionLinkResponse>> GetAffiliatePromotionLink()
        {
            try
            {
                GetAffiliatePromotionLinkResponse Response = new GetAffiliatePromotionLinkResponse();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }

                var PromotionType = _affiliateService.GetAffiliatePromotionLink(user);

                if (PromotionType != null)
                {
                    Response.Response = PromotionType;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;

                    return Ok(Response);
                }
                else
                {
                    return Ok(new GetAffiliatePromotionLinkResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound, ErrorCode = enErrorCode.NotFound });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new GetAffiliatePromotionLinkResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendAffiliateEmail(SendAffiliateEmailRequest Request)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }

                if (Request.EmailList.Count() > 0)
                {
                    var SendEmail = _affiliateService.SendAffiliateEmail(Request, user);

                    if (SendEmail == 1)
                    {
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ErrorCode = enErrorCode.AffiliateEmailSendSuccess;
                        Response.ReturnMsg = EnResponseMessage.AffiliateEmailSendSuccess;
                    }
                    else if (SendEmail == 2) //Not Affiliate User
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.UserIsNotAsffiliateUser;
                        Response.ReturnMsg = EnResponseMessage.UserIsNotAsffiliateUser;
                    }
                    else if (SendEmail == 3) //Not Select Email Promotion
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.UserIsNotSelectEmailPromotion;
                        Response.ReturnMsg = EnResponseMessage.UserIsNotSelectEmailPromotion;
                    }
                    else if (SendEmail == 4) //Houly limit exceed
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.AffiliateEmailHourlyLimitExceed;
                        Response.ReturnMsg = EnResponseMessage.AffiliateEmailHourlyLimitExceed;
                    }
                    else if (SendEmail == 5) //Daily Limit exceed
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.AffiliateEmailDailyLimitExceed;
                        Response.ReturnMsg = EnResponseMessage.AffiliateEmailDailyLimitExceed;
                    }
                    return Ok(Response);

                }
                else
                {
                    return Ok(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AffiliateSendEmailBlankRequest, ErrorCode = enErrorCode.AffiliateSendEmailBlankRequest });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new GetAffiliatePromotionLinkResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendAffiliateSMS(SendAffiliateSMSRequest Request)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }

                if (Request.MobileNumberList.Count() > 0)
                {
                    var SendSMS = _affiliateService.SendAffiliateSMS(Request, user);

                    if (SendSMS == 1)
                    {
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ErrorCode = enErrorCode.AffiliateSMSSendSuccess;
                        Response.ReturnMsg = EnResponseMessage.AffiliateSMSSendSuccess;
                    }
                    else if (SendSMS == 2) //Not Affiliate User
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.UserIsNotAsffiliateUser;
                        Response.ReturnMsg = EnResponseMessage.UserIsNotAsffiliateUser;
                    }
                    else if (SendSMS == 3) //Not Select SMS Promotion
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.UserIsNotSelectSMSPromotion;
                        Response.ReturnMsg = EnResponseMessage.UserIsNotSelectSMSPromotion;
                    }
                    else if (SendSMS == 4) //Houly limit exceed
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.AffiliateSMSHourlyLimitExceed;
                        Response.ReturnMsg = EnResponseMessage.AffiliateSMSHourlyLimitExceed;
                    }
                    else if (SendSMS == 5) //Daily Limit exceed
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.AffiliateSMSDailyLimitExceed;
                        Response.ReturnMsg = EnResponseMessage.AffiliateSMSDailyLimitExceed;
                    }
                    return Ok(Response);
                }
                else
                {
                    return Ok(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AffiliateSendSMSBlankRequest, ErrorCode = enErrorCode.AffiliateSendSMSBlankRequest });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new GetAffiliatePromotionLinkResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddPromotionLinkClick(AddPromotionLinkClickRequest Request, string passdata)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();

                if (string.IsNullOrEmpty(passdata))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.Status14199PassDataNotFound;
                    Response.ReturnMsg = EnResponseMessage.PassDataNotFound;
                    return Ok(Response);
                }
                //Add ipaddress null condition by  Rushali (25-12-2019)
                if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
                {
                    Request.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                }
                else
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound;
                    Response.ReturnMsg = EnResponseMessage.IpAddressInvalid;
                    return Ok(Response);
                }

                var response = _affiliateService.AddPromotionLinkClick(Request, passdata);

                if (response == 1)
                {
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.CommRecordInsertSuccess;
                }
                //Add Response fail by Rushali (25-12-2019)
                if (response == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.Status14200DataNotInsert;
                    Response.ReturnMsg = EnResponseMessage.DataNotInsert;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        #region Count method  //2019-3-15

        [HttpGet]
        public async Task<ActionResult<AffiliateDashboardCountResponse>> GetAffiliateDashboardCount()
        {
            AffiliateDashboardCountResponse Response = new AffiliateDashboardCountResponse();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response.Response = _affiliateService.GetAffiliateDashboardCount(user.Id);
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnCode = enResponseCode.Success;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new AffiliateDashboardCountResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<ActionResult<GetAffiateUserRegisteredResponse>> GetAffiateUserRegistered(GetAffiateUserRegisteredRequest Request)
        {
            try
            {
                GetAffiateUserRegisteredResponse Response = new GetAffiateUserRegisteredResponse();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    int Status = 999, SchemeType = 999;
                    long ParentUser = 999;
                    string sCondition = "";
                    //add !=0 condition  in status ,SchemeType and ParentUser -mansi 01-11-2019
                    if (Request.Status != null && Request.Status != 0)
                    {
                        Status = Convert.ToInt16(Request.Status);
                    }

                    if (Request.SchemeType != null && Request.SchemeType != 0)
                    {
                        SchemeType = Convert.ToInt16(Request.SchemeType);
                    }

                    if (Request.ParentUser != null && Request.ParentUser != 0)
                    {
                        ParentUser = Convert.ToInt64(Request.ParentUser);
                    }

                    if (!string.IsNullOrEmpty(Request.FromDate))
                    {
                        if (string.IsNullOrEmpty(Request.ToDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        if (!_affiliateService.IsValidDateFormate(Request.FromDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        if (!_affiliateService.IsValidDateFormate(Request.ToDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        sCondition += " AND AUM.CreatedDate Between {4} AND {5}";
                    }

                    Response = _affiliateService.GetAffiateUserRegistered(user.Id, Request.FromDate, Request.ToDate, Status, SchemeType, ParentUser, sCondition, Request.PageNo, Request.PageSize);
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new AffiliateDashboardCountResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<ActionResult<GetReferralLinkClickResponse>> GetReferralLinkClick(GetReferralLinkClickRequest Request)
        {
            try
            {
                GetReferralLinkClickResponse Response = new GetReferralLinkClickResponse();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    long UserId = 999;
                    string sCondition = "";
                    UserId = user.Id;
                    if (!string.IsNullOrEmpty(Request.FromDate))
                    {
                        if (string.IsNullOrEmpty(Request.ToDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        if (!_affiliateService.IsValidDateFormate(Request.FromDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        if (!_affiliateService.IsValidDateFormate(Request.ToDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }
                        sCondition += " AND ALC.CreatedDate Between {1} AND {2}";
                    }
                    Response = _affiliateService.GetReferralLinkClick(Request.FromDate, Request.ToDate, UserId, sCondition, Request.PageNo, Request.PageSize);
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new AffiliateDashboardCountResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<ActionResult<GetFacebookLinkClickResponse>> GetFacebookLinkClick(GetFacebookLinkClickRequest Request)
        {
            try
            {
                GetFacebookLinkClickResponse Response = new GetFacebookLinkClickResponse();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    long UserId = 999;
                    string sCondition = "";
                    UserId = user.Id;
                    if (!string.IsNullOrEmpty(Request.FromDate))
                    {
                        if (string.IsNullOrEmpty(Request.ToDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        if (!_affiliateService.IsValidDateFormate(Request.FromDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        if (!_affiliateService.IsValidDateFormate(Request.ToDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        sCondition += " AND ALC.CreatedDate Between {1} AND {2}";
                    }

                    Response = _affiliateService.GetFacebookLinkClick(Request.FromDate, Request.ToDate, UserId, sCondition, Request.PageNo, Request.PageSize);
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new AffiliateDashboardCountResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<ActionResult<GetTwitterLinkClickResponse>> GetTwitterLinkClick(GetTwitterLinkClickRequest Request)
        {
            try
            {
                GetTwitterLinkClickResponse Response = new GetTwitterLinkClickResponse();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    long UserId = 999;
                    string sCondition = "";
                    UserId = user.Id;
                    if (!string.IsNullOrEmpty(Request.FromDate))
                    {
                        if (string.IsNullOrEmpty(Request.ToDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        if (!_affiliateService.IsValidDateFormate(Request.FromDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        if (!_affiliateService.IsValidDateFormate(Request.ToDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        sCondition += " AND ALC.CreatedDate Between {1} AND {2}";
                    }

                    Response = _affiliateService.GetTwitterLinkClick(Request.FromDate, Request.ToDate, UserId, sCondition, Request.PageNo, Request.PageSize);
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new AffiliateDashboardCountResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<ActionResult<GetEmailSentResponse>> GetEmailSent(GetEmailSentRequest Request)
        {
            try
            {
                GetEmailSentResponse Response = new GetEmailSentResponse();
                // ApplicationUser user = new ApplicationUser(); user.Id = 352;
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    long UserId = 999;
                    string sCondition = "";
                    UserId = user.Id;
                    if (!string.IsNullOrEmpty(Request.FromDate))
                    {
                        if (string.IsNullOrEmpty(Request.ToDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        if (!_affiliateService.IsValidDateFormate(Request.FromDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        if (!_affiliateService.IsValidDateFormate(Request.ToDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        sCondition += " AND APS.CreatedDate Between {1} AND {2}";
                    }

                    Response = _affiliateService.GetEmailSent(Request.FromDate, Request.ToDate, UserId, sCondition, Request.PageNo, Request.PageSize);
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new AffiliateDashboardCountResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<ActionResult<GetSMSSentResponse>> GetSMSSent(GetSMSSentRequest Request)
        {
            try
            {
                GetSMSSentResponse Response = new GetSMSSentResponse();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    long UserId = 999;
                    string sCondition = "";
                    UserId = user.Id;
                    if (!string.IsNullOrEmpty(Request.FromDate))
                    {
                        if (string.IsNullOrEmpty(Request.ToDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        if (!_affiliateService.IsValidDateFormate(Request.FromDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        if (!_affiliateService.IsValidDateFormate(Request.ToDate))
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                            Response.ReturnMsg = "Invalid Date Format";
                            return Response;
                        }

                        sCondition += " AND APS.CreatedDate Between {1} AND {2}";
                    }

                    Response = _affiliateService.GetSMSSent(Request.FromDate, Request.ToDate, UserId, sCondition, Request.PageNo, Request.PageSize);
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new AffiliateDashboardCountResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<ActionResult<GetAllAffiliateUserResponse>> GetAllAffiliateUser()
        {
            try
            {
                GetAllAffiliateUserResponse Response = new GetAllAffiliateUserResponse();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var _Res = _affiliateService.GetAllAffiliateUser();

                    if (_Res.Count > 0)
                    {
                        Response.Response = _Res;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ErrorCode = enErrorCode.Success;
                        Response.ReturnMsg = EnResponseMessage.FindRecored;
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.NoDataFound;
                        Response.ReturnMsg = "No Data Found";
                    }
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new AffiliateDashboardCountResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Commission history Report

        [HttpGet("{PageNo}/{PageSize}")]
        public async Task<IActionResult> AffiliateCommissionHistoryReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, long? TrnUserId, long? SchemeMappingId, long? TrnRefNo)
        {
            ListAffiliateCommissionHistoryReport Response = new ListAffiliateCommissionHistoryReport();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _affiliateService.AffiliateCommissionHistoryReport(PageNo, PageSize, FromDate, ToDate, TrnUserId, user.Id, SchemeMappingId, TrnRefNo);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Graph API

        [HttpGet]
        public async Task<IActionResult> GetAffiliateInvitieChartDetail()
        {
            ListInviteFrdClaas Response = new ListInviteFrdClaas();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _affiliateService.GetAffiliateInviteFrieds(user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthWiseCommissionChartDetail(int? Year)
        {
            ListGetMonthWiseCommissionData Response = new ListGetMonthWiseCommissionData();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _affiliateService.GetMonthWiseCommissionChartDetail(Year, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion
    }
}