using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.User;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Worldex.Core.Services;
using Worldex.Core.ViewModels.AccountViewModels.Log;
using Worldex.Core.Interfaces.Log;
using Newtonsoft.Json;
using Worldex.Core.ViewModels;
using Worldex.Core.Interfaces;
using Worldex.Infrastructure.BGTask;
using Worldex.Core.Interfaces.Referral;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.ViewModels.Referral;

namespace BackofficeWorldex.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferralController : BaseController
    {
        #region Field
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userdata;
        private readonly ITempUserRegisterService _tempUserRegisterService;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IRegisterTypeService _registerTypeService;
        private readonly ISignupLogService _IsignupLogService;
        private readonly IMessageConfiguration _messageConfiguration;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly IReferralUser _ReferralUser;
        private readonly IMessageService _messageService;
        private readonly IReferralPayType _ReferralPayType;
        private readonly IReferralChannelType _ReferralChannelType;
        private readonly IReferralServiceType _ReferralServiceType;
        private readonly IReferralService _ReferralService;
        private readonly IReferralChannel _ReferralChannel;
        private readonly IReferralUserClick _ReferralUserClick;
        private readonly IReferralRewards _ReferralRewards;

        #endregion

        #region Ctore
        public ReferralController(UserManager<ApplicationUser> userManager, IUserService userdata, ITempUserRegisterService tempUserRegisterService,
            EncyptedDecrypted encdecAEC, Microsoft.Extensions.Configuration.IConfiguration configuration, IRegisterTypeService registerTypeService,
            ISignupLogService IsignupLogService, IMessageConfiguration messageConfiguration, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, IReferralUser ReferralUser,
            IMessageService MessageService, IReferralPayType ReferralPayType, IReferralChannelType ReferralChannelType, IReferralServiceType ReferralServiceType, IReferralService ReferralService,
            IReferralChannel ReferralChannel, IReferralUserClick ReferralUserClick, IReferralRewards ReferralRewards)
        {
            _userManager = userManager;
            _userdata = userdata;
            _tempUserRegisterService = tempUserRegisterService;
            _encdecAEC = encdecAEC;
            _configuration = configuration;
            _registerTypeService = registerTypeService;
            _IsignupLogService = IsignupLogService;
            _messageConfiguration = messageConfiguration;
            _pushNotificationsQueue = pushNotificationsQueue;
            _ReferralUser = ReferralUser;
            _messageService = MessageService;
            _ReferralPayType = ReferralPayType;
            _ReferralChannelType = ReferralChannelType;
            _ReferralServiceType = ReferralServiceType;
            _ReferralService = ReferralService;
            _ReferralChannel = ReferralChannel;
            _ReferralUserClick = ReferralUserClick;
            _ReferralRewards = ReferralRewards;
        }
        #endregion

        [HttpPost("RegisterWithReferral")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterWithReferral(RegisterViewModel model, string returnUrl = null)
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
                var ipmodel = await _userdata.GetIPWiseData(model.IPAddress);
                if (!string.IsNullOrEmpty(ipmodel?.CountryCode) && ipmodel?.CountryCode == "fail")
                {
                    return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
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

                        UserId = RefUser != 0 ? RefUser : 0,
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

        [HttpPost("GetUserReferralCode")]
        [Authorize]
        public async Task<IActionResult> GetUserReferralCode()
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new UserReferralCodeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                string ReferralCode = await _ReferralUser.GetUserReferralCode(user.Id);
                if (!String.IsNullOrEmpty(ReferralCode))
                {
                    return new OkObjectResult(
                        new UserReferralCodeResponse
                        {
                            ReturnCode = enResponseCode.Success,
                            ReturnMsg = EnResponseMessage.ReferralCodeStatus,
                            UserReferralCode = ReferralCode
                        });
                }
                else
                {
                    return BadRequest(new UserReferralCodeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralCodeStatus, ErrorCode = enErrorCode.Status32022ReferralCodeNotAvailable });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralUserResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #region Referral Service
        [HttpPost("AddReferralService")]
        [Authorize]
        public async Task<IActionResult> AddReferralService(ReferralServiceViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                int result = DateTime.Compare(model.ActiveDate.Date, model.ExpireDate.Date);
                if (result > 0 || result == 0)
                {
                    return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ExpireDateMustBeGraterThanActiveDate, ErrorCode = enErrorCode.Status32037ExpireDateMustBeGraterThanActiveDate });
                }
                if (model.ReferralPayTypeId != 0 && model.ReferralPayTypeId != 0 && model.CurrencyId != 0)
                {
                    var obj = new ReferralServiceViewModel()
                    {
                        ReferralServiceTypeId = model.ReferralServiceTypeId,
                        ReferralPayTypeId = model.ReferralPayTypeId,
                        CurrencyId = model.CurrencyId,
                        Description = model.Description,
                        ReferMinCount = model.ReferMinCount,
                        ReferMaxCount = model.ReferMaxCount,
                        RewardsPay = model.RewardsPay,
                        ActiveDate = model.ActiveDate.Date,
                        ExpireDate = model.ExpireDate.Date
                    };

                    long id = _ReferralService.AddReferralService(obj, user.Id);
                    if (id != 0)
                    {
                        return new OkObjectResult(
                        new ReferralServiceResponse
                        {
                            ReturnCode = enResponseCode.Success,
                            ReturnMsg = EnResponseMessage.SuccessfullAddReferralService
                        });
                    }
                    else
                    {
                        return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralService, ErrorCode = enErrorCode.Status32023InvalidReferralService });
                    }
                }
                else
                {
                    return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralService, ErrorCode = enErrorCode.Status32023InvalidReferralService });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("GetReferralServiceById")]
        [Authorize]
        public async Task<ActionResult> GetReferralServiceById(ReferralServiceStatusViewModel model)
        {
            try
            {
                var getReferralServiceData = await Task.FromResult(_ReferralService.GetReferralServiceById(model.Id));
                if (getReferralServiceData != null)
                    return Ok(new ReferralServiceObjResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetReferralServiceById, ReferralServiceObj = getReferralServiceData });
                else
                    return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.GetFailReferralServiceById, ErrorCode = enErrorCode.Status32027GetFailReferralServiceById });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetReferralService")]
        [Authorize]
        public async Task<ActionResult> GetReferralService()
        {
            try
            {
                var getReferralServiceData = await Task.FromResult(_ReferralService.GetReferralService());
                if (getReferralServiceData != null)
                    return Ok(new ReferralServiceObjResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetReferralService, ReferralServiceObj = getReferralServiceData });
                else
                    return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.GetFailReferralService, ErrorCode = enErrorCode.Status32060GetFailReferralService });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ListReferralService")]
        [Authorize]
        public async Task<ActionResult> ListReferralService(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var getReferralServiceData = await Task.FromResult(_ReferralService.ListReferralService(PageIndex, Page_Size));
                if (getReferralServiceData != null)
                    return Ok(new ReferralServiceListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralServiceList, ReferralServiceList = getReferralServiceData.ReferralServiceList, TotalCount = getReferralServiceData.TotalCount });
                else
                    return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralServiceList, ErrorCode = enErrorCode.Status32028FailReferralServiceList });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("DropDownReferralService")]
        [Authorize]
        public async Task<ActionResult> DropDownReferralService(int PayTypeId)
        {
            try
            {
                var getReferralServiceData = await Task.FromResult(_ReferralService.DropDownReferralService(PayTypeId));
                if (getReferralServiceData != null)
                    return Ok(new ReferralServiceDropDownResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralServicesDropDown, ReferralServiceDropDownList = getReferralServiceData });
                else
                    return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralServiecsDropDown, ErrorCode = enErrorCode.Status32040FailReferralServiecsDropDown });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("UpdateReferralService")]
        [Authorize]
        public async Task<IActionResult> UpdateReferralService(ReferralServiceUpdateViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
             
                var obj = new ReferralServiceUpdateViewModel()
                {
                    Id = model.Id,
                    ReferralServiceTypeId = model.ReferralServiceTypeId,
                    ReferralPayTypeId = model.ReferralPayTypeId,
                    CurrencyId = model.CurrencyId,
                    Description = model.Description,
                    ReferMinCount = model.ReferMinCount,
                    ReferMaxCount = model.ReferMaxCount,
                    RewardsPay = model.RewardsPay,
                    ActiveDate = model.ActiveDate,
                    ExpireDate = model.ExpireDate
                };

                long id = _ReferralService.UpdateReferralService(obj, user.Id);
                if (id != 0)
                {
                    return new OkObjectResult(
                    new ReferralServiceResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.SuccessfullUpdateReferralService
                    });
                }
                else
                {
                    return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidUpdateReferralService, ErrorCode = enErrorCode.Status32024InvalidUpdateReferralService });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("DisableReferralService")]
        [Authorize]
        public async Task<IActionResult> DisableReferralService(ReferralServiceStatusViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var obj = new ReferralServiceStatusViewModel()
                {
                    Id = model.Id

                };
                bool id = _ReferralService.DisableReferralService(obj, user.Id);
                if (id == true)
                {
                    return new OkObjectResult(
                    new ReferralServiceResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.DisableReferralService
                    });
                }
                else
                {
                    return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailDisableReferralServiceStatus, ErrorCode = enErrorCode.Status32025FailDisableReferralServiceStatus });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("EnableReferralService")]
        [Authorize]
        public async Task<IActionResult> EnableReferralService(ReferralPayTypeStatusViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new ReferralServiceStatusResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var obj = new ReferralServiceStatusViewModel()
                {
                    Id = model.Id
                };
                bool id = _ReferralService.EnableReferralService(obj, user.Id);
                if (id == true)
                {
                    return new OkObjectResult(
                    new ReferralServiceStatusResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.EnableReferralService
                    });
                }
                else
                {
                    return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailEnableReferralServiceStatus, ErrorCode = enErrorCode.Status32026FailEnableReferralServiceStatus });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Referral Channel

        [HttpPost("AddReferralChannel")]
        [Authorize]
        public async Task<IActionResult> AddReferralChannel(ReferralChannelViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                var obj = new ReferralChannelViewModel()
                {
                    ReferralChannelTypeId = model.ReferralChannelTypeId,
                    ReferralChannelServiceId = model.ReferralChannelServiceId,
                    ReferralReceiverAddress = model.ReferralReceiverAddress,
                };
                long id = _ReferralChannel.AddReferralChannel(obj, user.Id);
                if (id != 0)
                {
                    return new OkObjectResult(
                    new ReferralChannelResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.SuccessfullAddReferralChannel
                    });
                }
                else
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralChannel, ErrorCode = enErrorCode.Status32029InvalidReferralChannel });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("ListAdminReferralChannelInvite")]
        [Authorize]
        public async Task<ActionResult> ListAdminReferralChannelInvite(int PageIndex = 0, int Page_Size = 0, long ReferralChannelTypeId = 0, long ReferralPayTypeId = 0, long ReferralServiceId = 0, string UserName = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (FromDate != null && FromDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralFromDateGreterThanToday, ErrorCode = enErrorCode.Status32065ReferralFromDateGreterThanToday });
                }

                if (ToDate != null && ToDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralToDateGreterThanToday, ErrorCode = enErrorCode.Status32066ReferralToDateGreterThanToday });
                }

                if (FromDate != null && ToDate != null && (FromDate > ToDate) )
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralCompareFromDateTodate, ErrorCode = enErrorCode.Status32067ReferralCompareFromDateTodate });
                }

                if ((FromDate != null && ToDate == null) || (FromDate == null && ToDate != null))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralBothDateFromDateTodateRequired, ErrorCode = enErrorCode.Status32068ReferralBothDateFromDateTodateRequired });
                }

                var getReferralChannelData = await Task.FromResult(_ReferralChannel.ListAdminReferralChannelInvite(PageIndex, Page_Size, ReferralChannelTypeId, ReferralPayTypeId, ReferralServiceId, UserName, FromDate, ToDate));
                if (getReferralChannelData != null)
                    return Ok(new ReferralChannelListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralChannelList, ReferralChannelList = getReferralChannelData.ReferralChannelList, TotalCount = getReferralChannelData.TotalCount });
                else
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralChannelList, ErrorCode = enErrorCode.Status32030FailReferralChannelList });

            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("ListAdminReferralChannelWithChannelType")]
        [Authorize]
        public async Task<ActionResult> ListAdminReferralChannelWithChannelType(long ReferralChannelTypeId, int PageIndex = 0, int Page_Size = 0, long ReferralPayTypeId = 0, long ReferralServiceId = 0, string UserName = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (FromDate != null && FromDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralFromDateGreterThanToday, ErrorCode = enErrorCode.Status32065ReferralFromDateGreterThanToday });
                }

                if (ToDate != null && ToDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralToDateGreterThanToday, ErrorCode = enErrorCode.Status32066ReferralToDateGreterThanToday });
                }

                if (FromDate != null && ToDate != null && (FromDate > ToDate))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralCompareFromDateTodate, ErrorCode = enErrorCode.Status32067ReferralCompareFromDateTodate });
                }

                if ((FromDate != null && ToDate == null) || (FromDate == null && ToDate != null))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralBothDateFromDateTodateRequired, ErrorCode = enErrorCode.Status32068ReferralBothDateFromDateTodateRequired });
                }
                var getReferralChannelData = await Task.FromResult(_ReferralChannel.ListAdminReferralChannelWithChannelType(ReferralChannelTypeId, PageIndex, Page_Size, ReferralPayTypeId, ReferralServiceId, UserName, FromDate, ToDate));
                if (getReferralChannelData != null)
                    return Ok(new ReferralChannelListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralChannelListByChannelTypeId, ReferralChannelList = getReferralChannelData.ReferralChannelList, TotalCount = getReferralChannelData.TotalCount });
                else
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralChannelListByChannelTypeId, ErrorCode = enErrorCode.Status32033FailReferralChannelListByChannelTypeId });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("ListUserReferralChannelInvite")]
        [Authorize]
        public async Task<ActionResult> ListUserReferralChannelInvite(int PageIndex = 0, int Page_Size = 0, long ReferralChannelTypeId = 0, long ReferralPayTypeId = 0, long ReferralServiceId = 0, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (FromDate != null && FromDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralFromDateGreterThanToday, ErrorCode = enErrorCode.Status32065ReferralFromDateGreterThanToday });
                }

                if (ToDate != null && ToDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralToDateGreterThanToday, ErrorCode = enErrorCode.Status32066ReferralToDateGreterThanToday });
                }

                if (FromDate != null && ToDate != null && (FromDate > ToDate))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralCompareFromDateTodate, ErrorCode = enErrorCode.Status32067ReferralCompareFromDateTodate });
                }

                if ((FromDate != null && ToDate == null) || (FromDate == null && ToDate != null))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralBothDateFromDateTodateRequired, ErrorCode = enErrorCode.Status32068ReferralBothDateFromDateTodateRequired });
                }

                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                var getReferralChannelData = await Task.FromResult(_ReferralChannel.ListUserReferralChannelInvite(user.Id, PageIndex, Page_Size, ReferralChannelTypeId, ReferralPayTypeId, ReferralServiceId, FromDate, ToDate));
                if (getReferralChannelData != null)
                    return Ok(new ReferralChannelListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetReferralChannelCountForUserInvite, ReferralChannelList = getReferralChannelData.ReferralChannelList, TotalCount = getReferralChannelData.TotalCount });
                else
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralChannelCountForUserInvite, ErrorCode = enErrorCode.Status32034FailReferralChannelCountForUserInvite });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ListUserReferralChannelWithChannelType")]
        [Authorize]
        public async Task<ActionResult> ListUserReferralChannelWithChannelType(long ReferralChannelTypeId, int PageIndex = 0, int Page_Size = 0, long ReferralPayTypeId = 0, long ReferralServiceId = 0, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (FromDate != null && FromDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralFromDateGreterThanToday, ErrorCode = enErrorCode.Status32065ReferralFromDateGreterThanToday });
                }

                if (ToDate != null && ToDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralToDateGreterThanToday, ErrorCode = enErrorCode.Status32066ReferralToDateGreterThanToday });
                }

                if (FromDate != null && ToDate != null && (FromDate > ToDate))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralCompareFromDateTodate, ErrorCode = enErrorCode.Status32067ReferralCompareFromDateTodate });
                }

                if ((FromDate != null && ToDate == null) || (FromDate == null && ToDate != null))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralBothDateFromDateTodateRequired, ErrorCode = enErrorCode.Status32068ReferralBothDateFromDateTodateRequired });
                }
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var getReferralChannelData = await Task.FromResult(_ReferralChannel.ListUserReferralChannelWithChannelType(ReferralChannelTypeId, user.Id, PageIndex, Page_Size, ReferralPayTypeId, ReferralServiceId, FromDate, ToDate));
                if (getReferralChannelData != null)
                    return Ok(new ReferralChannelListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralChannelListByUserChannelTypeId, ReferralChannelList = getReferralChannelData.ReferralChannelList, TotalCount = getReferralChannelData.TotalCount });
                else
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralChannelListByUserChannelTypeId, ErrorCode = enErrorCode.Status32035FailReferralChannelListByUserChannelTypeId });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ListAdminParticipateReferralUser")]
        [Authorize]
        public async Task<ActionResult> ListAdminParticipateReferralUser(int PageIndex = 0, int Page_Size = 0, long ReferralServiceId = 0, string UserName = null, string ReferUserName = null, DateTime? FromDate = null, DateTime? ToDate = null, int ReferralChannelTypeId = 0)
        {
            try
            {
                if (FromDate != null && FromDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralFromDateGreterThanToday, ErrorCode = enErrorCode.Status32065ReferralFromDateGreterThanToday });
                }

                if (ToDate != null && ToDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralToDateGreterThanToday, ErrorCode = enErrorCode.Status32066ReferralToDateGreterThanToday });
                }

                if (FromDate != null && ToDate != null && (FromDate > ToDate))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralCompareFromDateTodate, ErrorCode = enErrorCode.Status32067ReferralCompareFromDateTodate });
                }

                if ((FromDate != null && ToDate == null) || (FromDate == null && ToDate != null))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralBothDateFromDateTodateRequired, ErrorCode = enErrorCode.Status32068ReferralBothDateFromDateTodateRequired });
                }

                var getReferralUserData = await Task.FromResult(_ReferralUser.ListAdminParticipateReferralUser(PageIndex, Page_Size, ReferralServiceId, UserName, ReferUserName, FromDate, ToDate, ReferralChannelTypeId));
                if (getReferralUserData != null)
                    return Ok(new ReferralUserListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralUserListForAdmin, ReferralUserList = getReferralUserData.ReferralUserList, TotalCount = getReferralUserData.TotalCount });
                else
                    return BadRequest(new ReferralUserResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralUserListForAdmin, ErrorCode = enErrorCode.Status32038FailReferralUserListForAdmin });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralUserResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ListUserParticipateReferralUser")]
        [Authorize]
        public async Task<ActionResult> ListUserParticipateReferralUser(int PageIndex = 0, int Page_Size = 0, long ReferralServiceId = 0, string ReferUserName = null, DateTime? FromDate = null, DateTime? ToDate = null, int ReferralChannelTypeId = 0)
        {
            try
            {
                if (FromDate != null && FromDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralFromDateGreterThanToday, ErrorCode = enErrorCode.Status32065ReferralFromDateGreterThanToday });
                }

                if (ToDate != null && ToDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralToDateGreterThanToday, ErrorCode = enErrorCode.Status32066ReferralToDateGreterThanToday });
                }

                if (FromDate != null && ToDate != null && (FromDate > ToDate))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralCompareFromDateTodate, ErrorCode = enErrorCode.Status32067ReferralCompareFromDateTodate });
                }

                if ((FromDate != null && ToDate == null) || (FromDate == null && ToDate != null))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralBothDateFromDateTodateRequired, ErrorCode = enErrorCode.Status32068ReferralBothDateFromDateTodateRequired });
                }
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var getReferralUserData = await Task.FromResult(_ReferralUser.ListUserParticipateReferralUser(user.Id, PageIndex, Page_Size, ReferralServiceId, ReferUserName, FromDate, ToDate, ReferralChannelTypeId));
                if (getReferralUserData != null)
                    return Ok(new ReferralUserListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralUserListForUser, ReferralUserList = getReferralUserData.ReferralUserList, TotalCount = getReferralUserData.TotalCount });
                else
                    return BadRequest(new ReferralUserResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralUserListForUser, ErrorCode = enErrorCode.Status32039FailReferralUserListForUser });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralUserResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ListAdminReferralUserClick")]
        [Authorize]
        public async Task<ActionResult> ListAdminReferralUserClick(int PageIndex = 0, int Page_Size = 0, long ReferralChannelTypeId = 0, long ReferralServiceId = 0, string UserName = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (FromDate != null && FromDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralFromDateGreterThanToday, ErrorCode = enErrorCode.Status32065ReferralFromDateGreterThanToday });
                }

                if (ToDate != null && ToDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralToDateGreterThanToday, ErrorCode = enErrorCode.Status32066ReferralToDateGreterThanToday });
                }

                if (FromDate != null && ToDate != null && (FromDate > ToDate))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralCompareFromDateTodate, ErrorCode = enErrorCode.Status32067ReferralCompareFromDateTodate });
                }

                if ((FromDate != null && ToDate == null) || (FromDate == null && ToDate != null))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralBothDateFromDateTodateRequired, ErrorCode = enErrorCode.Status32068ReferralBothDateFromDateTodateRequired });
                }
                var getReferralUserClickData = await Task.FromResult(_ReferralUserClick.ListAdminReferralUserClick(PageIndex, Page_Size, ReferralChannelTypeId, ReferralServiceId, UserName, FromDate, ToDate));
                if (getReferralUserClickData != null)
                    return Ok(new ReferralUserClickListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralUserClickListForAdmin, ReferralUserClickList = getReferralUserClickData.ReferralUserClickList, TotalCount = getReferralUserClickData.TotalCount });
                else
                    return BadRequest(new ReferralUserClickResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralUserClickListForAdmin, ErrorCode = enErrorCode.Status32058FailReferralUserClickListForAdmin });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralUserClickResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ListUserReferralUserClick")]
        [Authorize]
        public async Task<ActionResult> ListUserReferralUserClick(int PageIndex = 0, int Page_Size = 0, long ReferralChannelTypeId = 0, long ReferralServiceId = 0, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (FromDate != null && FromDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralFromDateGreterThanToday, ErrorCode = enErrorCode.Status32065ReferralFromDateGreterThanToday });
                }

                if (ToDate != null && ToDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralToDateGreterThanToday, ErrorCode = enErrorCode.Status32066ReferralToDateGreterThanToday });
                }

                if (FromDate != null && ToDate != null && (FromDate > ToDate))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralCompareFromDateTodate, ErrorCode = enErrorCode.Status32067ReferralCompareFromDateTodate });
                }

                if ((FromDate != null && ToDate == null) || (FromDate == null && ToDate != null))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralBothDateFromDateTodateRequired, ErrorCode = enErrorCode.Status32068ReferralBothDateFromDateTodateRequired });
                }
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var getReferralUserClickData = await Task.FromResult(_ReferralUserClick.ListUserReferralUserClick(user.Id, PageIndex, Page_Size, ReferralChannelTypeId, ReferralServiceId, FromDate, ToDate));
                if (getReferralUserClickData != null)
                    return Ok(new ReferralUserClickListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralUserClickListForUser, ReferralUserClickList = getReferralUserClickData.ReferralUserClickList, TotalCount = getReferralUserClickData.TotalCount });
                else
                    return BadRequest(new ReferralUserClickResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralUserClickListForUser, ErrorCode = enErrorCode.Status32059FailReferralUserClickListForUser });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralUserClickResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("AllCountForAdminReferralChannel")]
        [Authorize]
        public async Task<ActionResult> AllCountForAdminReferralChannel()
        {
            try
            {
                var getReferralData = await Task.FromResult(_ReferralChannel.AllCountForAdminReferralChannel());
                if (getReferralData != null)
                    return Ok(new ReferralChannelAdminCountObjResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetReferralChannelCountForAdmin, ReferralChannelAdminCount = getReferralData });
                else
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralChannelCountForAdmin, ErrorCode = enErrorCode.Status32031FailReferralChannelCountForAdmin });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("AllCountForUserReferralChannel")]
        [Authorize]
        public async Task<ActionResult> AllCountForUserReferralChannel()
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var getReferralData = await Task.FromResult(_ReferralChannel.AllCountForUserReferralChannel(user.Id));
                if (getReferralData != null)
                    return Ok(new ReferralChannelUserCountObjResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetReferralChannelCountForUser, ReferralChannelUserCount = getReferralData });
                else
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralChannelCountForUser, ErrorCode = enErrorCode.Status32036FailReferralChannelCountForUser });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }



        #endregion

        #region PayType
        [HttpPost("AddPayType")]
        [Authorize]
        public async Task<IActionResult> AddPayType(ReferralPayTypeViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                if (string.IsNullOrEmpty(model.PayTypeName))
                {
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralPayType, ErrorCode = enErrorCode.Status32001InvalidReferralPayType });
                }

                bool payname = _ReferralPayType.IsReferralPayTypeExist(model.PayTypeName);
                if (payname)
                {
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ExistReferralPayType, ErrorCode = enErrorCode.Status32019ReferralPayTypeExist });
                }

                var obj = new ReferralPayTypeViewModel()
                {
                    PayTypeName = model.PayTypeName
                };

                long id = _ReferralPayType.AddReferralPayType(obj, user.Id);
                if (id != 0)
                {
                    return new OkObjectResult(
                    new ReferralPayTypeResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.SuccessfullReferralPayType
                    });
                }
                else
                {
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralPayType, ErrorCode = enErrorCode.Status32001InvalidReferralPayType });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetReferralPayType")]
        [Authorize]
        public async Task<ActionResult> GetReferralPayType(ReferralPayTypeStatusViewModel model)
        {
            try
            {
                var getReferralData = await Task.FromResult(_ReferralPayType.GetReferralPayType(model.Id));
                if (getReferralData != null)
                    return Ok(new ReferralPayTypeObjResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetReferralPayType, ReferralPayTypeObj = getReferralData });
                else
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.GetFailReferralPayType, ErrorCode = enErrorCode.Status32002GetFailReferralPayType });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("ListReferralPayType")]
        [Authorize]
        public async Task<ActionResult> ListReferralPayType()
        {
            try
            {
                var getReferralData = await Task.FromResult(_ReferralPayType.ListReferralPayType());
                if (getReferralData != null)
                    return Ok(new ReferralPayTypeListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralPayTypeList, ReferralPayTypeList = getReferralData });
                else
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralPayTypeList, ErrorCode = enErrorCode.Status32003FailReferralPayTypeList });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("DropDownReferralPayType")]
        [Authorize]
        public async Task<ActionResult> DropDownReferralPayType()
        {
            try
            {
                var getReferralData = await Task.FromResult(_ReferralPayType.DropDownReferralPayType());
                if (getReferralData != null)
                    return Ok(new ReferralPayTypeDropDownResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralPayTypeDropDown, ReferralPayTypeDropDownList = getReferralData });
                else
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralPayTypeDropDown, ErrorCode = enErrorCode.Status32004FailReferralPayTypeDropDown });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("UpdateReferralPayType")]
        [Authorize]
        public async Task<IActionResult> UpdateReferralPayType(ReferralPayTypeUpdateViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (string.IsNullOrEmpty(model.PayTypeName))
                {
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralPayType, ErrorCode = enErrorCode.Status32001InvalidReferralPayType });
                }

                bool payname = _ReferralPayType.IsReferralPayTypeExist(model.PayTypeName);
                if (payname)
                {
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ExistReferralPayType, ErrorCode = enErrorCode.Status32019ReferralPayTypeExist });
                }
                var obj = new ReferralPayTypeUpdateViewModel()
                {
                    Id = model.Id,
                    PayTypeName = model.PayTypeName
                };

                long id = _ReferralPayType.UpdateReferralPayType(obj, user.Id);
                if (id != 0)
                {
                    return new OkObjectResult(
                    new ReferralPayTypeResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.SuccessfullUpdateReferralPayType
                    });
                }
                else
                {
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidUpdateReferralPayType, ErrorCode = enErrorCode.Status32005InvalidUpdateReferralPayType });

                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("DisableReferralPayType")]
        [Authorize]
        public async Task<IActionResult> DisableReferralPayType(ReferralPayTypeStatusViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var obj = new ReferralPayTypeStatusViewModel()
                {
                    Id = model.Id

                };
                bool id = _ReferralPayType.DisableReferralPayType(obj, user.Id);
                if (id == true)
                {
                    return new OkObjectResult(
                    new ReferralPayTypeResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.DisableReferralPayTypeStatus
                    });
                }
                else
                {
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralPayTypeStatus, ErrorCode = enErrorCode.Status32006FailReferralPayTypeStatus });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("EnableReferralPayType")]
        [Authorize]
        public async Task<IActionResult> EnableReferralPayType(ReferralPayTypeStatusViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var obj = new ReferralPayTypeStatusViewModel()
                {
                    Id = model.Id

                };
                bool id = _ReferralPayType.EnableReferralPayType(obj, user.Id);
                if (id == true)
                {
                    return new OkObjectResult(
                    new ReferralPayTypeResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.EnableReferralPayTypeStatus

                    });
                }
                else
                {
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralPayTypeStatus, ErrorCode = enErrorCode.Status32006FailReferralPayTypeStatus });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ReferralPayTypeExist")]
        [Authorize]
        public async Task<ActionResult> IsReferralPayTypeExist(ReferralPayTypeViewModel model)
        {
            try
            {
                bool getReferralData = await Task.FromResult(_ReferralPayType.IsReferralPayTypeExist(model.PayTypeName));
                if (getReferralData)
                    return Ok(new ReferralPayTypeExistResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ExistReferralPayType, ReferralPayTypeExist = true });
                else
                    return Ok(new ReferralPayTypeExistResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.NotExistReferralPayType, ReferralPayTypeExist = false });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion 

        #region ChannelType
        [HttpPost("AddChannelType")]
        [Authorize]
        public async Task<IActionResult> AddChannelType(ReferralChannelTypeViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                if (string.IsNullOrEmpty(model.ChannelTypeName))
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralChannelType, ErrorCode = enErrorCode.Status32007InvalidReferralChannelType });
                }
                if (model.HourlyLimit <= 0)
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralHourlyLimitRequired, ErrorCode = enErrorCode.Status32053ReferralHourlyLimitRequired });
                }
                if (model.DailyLimit <= 0)
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralDailyLimitRequired, ErrorCode = enErrorCode.Status32054ReferralDailyLimitRequired });
                }
                if (model.WeeklyLimit <= 0)
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralWeeklyLimitRequired, ErrorCode = enErrorCode.Status32055ReferralWeeklyLimitRequired });
                }
                if (model.MonthlyLimit <= 0)
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralMonthlyLimitRequired, ErrorCode = enErrorCode.Status32056ReferralMonthlyLimitRequired });
                }
                bool getReferralChannelDataExist = _ReferralChannelType.IsReferralChannelTypeExist(model.ChannelTypeName);
                if (getReferralChannelDataExist)
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ExistReferralChannelType, ErrorCode = enErrorCode.Status32020ReferralChannelTypeExist });
                }

                var obj = new ReferralChannelTypeViewModel()
                {
                    ChannelTypeName = model.ChannelTypeName,
                    HourlyLimit = model.HourlyLimit,
                    DailyLimit = model.DailyLimit,
                    WeeklyLimit = model.WeeklyLimit,
                    MonthlyLimit = model.MonthlyLimit
                };

                long id = _ReferralChannelType.AddReferralChannelType(obj, user.Id);
                if (id != 0)
                {
                    return new OkObjectResult(
                    new ReferralChannelTypeResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.SuccessfullReferralChannelType
                    });
                }
                else
                {
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralChannelType, ErrorCode = enErrorCode.Status32007InvalidReferralChannelType });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetReferralChannelType")]
        [Authorize]
        public async Task<ActionResult> GetReferralChannelType(ReferralChannelTypeStatusViewModel model)
        {
            try
            {
                var getReferralChannelData = await Task.FromResult(_ReferralChannelType.GetReferralChannelType(model.Id));
                if (getReferralChannelData != null)
                    return Ok(new ReferralChannelTypeObjResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetReferralChannelType, ReferralChannelTypeObj = getReferralChannelData });
                else
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.GetFailReferralChannelType, ErrorCode = enErrorCode.Status32008GetFailReferralChannelType });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ListReferralChannelType")]
        [Authorize]
        public async Task<ActionResult> ListReferralChannelType()
        {
            try
            {
                var getReferralChannelData = await Task.FromResult(_ReferralChannelType.ListReferralChannelType());
                if (getReferralChannelData != null)
                    return Ok(new ReferralChannelTypeListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralChannelTypeList, ReferralChannelTypeList = getReferralChannelData });
                else
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralChannelTypeList, ErrorCode = enErrorCode.Status32009FailReferralChannelTypeList });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("DropDownReferralChannelType")]
        [Authorize]
        public async Task<ActionResult> DropDownReferralChannelType()
        {
            try
            {
                var getReferralChannelData = await Task.FromResult(_ReferralChannelType.DropDownReferralChannelType());
                if (getReferralChannelData != null)
                    return Ok(new ReferralChannelTypeDropDownResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralChannelTypeDropDown, ReferralChannelTypeDropDownList = getReferralChannelData });
                else
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralChannelTypeDropDown, ErrorCode = enErrorCode.Status32010FailReferralChannelTypeDropDown });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("UpdateReferralChannelType")]
        [Authorize]
        public async Task<IActionResult> UpdateReferralChannelType(ReferralChannelTypeUpdateViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (string.IsNullOrEmpty(model.ChannelTypeName))
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralChannelType, ErrorCode = enErrorCode.Status32007InvalidReferralChannelType });
                }
                if (model.HourlyLimit <= 0)
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralHourlyLimitRequired, ErrorCode = enErrorCode.Status32053ReferralHourlyLimitRequired });
                }
                if (model.DailyLimit <= 0)
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralDailyLimitRequired, ErrorCode = enErrorCode.Status32054ReferralDailyLimitRequired });
                }
                if (model.WeeklyLimit <= 0)
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralWeeklyLimitRequired, ErrorCode = enErrorCode.Status32055ReferralWeeklyLimitRequired });
                }
                if (model.MonthlyLimit <= 0)
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralMonthlyLimitRequired, ErrorCode = enErrorCode.Status32056ReferralMonthlyLimitRequired });
                }
                bool getReferralChannelDataExist = _ReferralChannelType.IsReferralChannelTypeExist(model.ChannelTypeName);
                if (getReferralChannelDataExist)
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ExistReferralChannelType, ErrorCode = enErrorCode.Status32020ReferralChannelTypeExist });
                }
                var obj = new ReferralChannelTypeUpdateViewModel()
                {
                    Id = model.Id,
                    ChannelTypeName = model.ChannelTypeName,
                    HourlyLimit = model.HourlyLimit,
                    DailyLimit = model.DailyLimit,
                    WeeklyLimit = model.WeeklyLimit,
                    MonthlyLimit = model.MonthlyLimit
                };

                long id = _ReferralChannelType.UpdateReferralChannelType(obj, user.Id);
                if (id != 0)
                {
                    return new OkObjectResult(
                    new ReferralChannelTypeResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.SuccessfullUpdateReferralChannelType
                    });
                }
                else
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidUpdateReferralChannelType, ErrorCode = enErrorCode.Status32011InvalidUpdateReferralChannelType });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("DisableReferralChannelType")]
        [Authorize]
        public async Task<IActionResult> DisableReferralChannelType(ReferralChannelTypeStatusViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var obj = new ReferralChannelTypeStatusViewModel()
                {
                    Id = model.Id

                };
                bool id = _ReferralChannelType.DisableReferralChannelType(obj, user.Id);
                if (id == true)
                {
                    return new OkObjectResult(
                    new ReferralChannelTypeResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.DisableReferralChannelTypeStatus
                    });
                }
                else
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralChannelTypeStatus, ErrorCode = enErrorCode.Status32012FailReferralChannelTypeStatus });

                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("EnableReferralChannelType")]
        [Authorize]
        public async Task<IActionResult> EnableReferralChannelType(ReferralChannelTypeStatusViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var obj = new ReferralChannelTypeStatusViewModel()
                {
                    Id = model.Id

                };
                bool id = _ReferralChannelType.EnableReferralChannelType(obj, user.Id);
                if (id == true)
                {
                    return new OkObjectResult(
                    new ReferralChannelTypeResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.EnableReferralChannelTypeStatus
                    });
                }
                else
                {
                    return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralChannelTypeStatus, ErrorCode = enErrorCode.Status32012FailReferralChannelTypeStatus });

                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ReferralChannelTypeExist")]
        [Authorize]
        public async Task<ActionResult> IsReferralChannelTypeExist(ReferralChannelTypeViewModel model)
        {
            try
            {
                bool getReferralChannelData = await Task.FromResult(_ReferralChannelType.IsReferralChannelTypeExist(model.ChannelTypeName));
                if (getReferralChannelData)
                    return Ok(new ReferralChannelTypeExistResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ExistReferralChannelType, ReferralChannelTypeExist = true });
                else
                    return Ok(new ReferralChannelTypeExistResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.NotExistReferralChannelType, ReferralChannelTypeExist = false });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region ServiceType
        [HttpPost("AddServiceType")]
        [Authorize]
        public async Task<IActionResult> AddServiceType(ReferralServiceTypeViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (string.IsNullOrEmpty(model.ServiceTypeName))
                {
                    return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralServiceType, ErrorCode = enErrorCode.Status32013InvalidReferralServiceType });
                }
                bool getReferralServiceDataExist = _ReferralServiceType.IsReferralServiceTypeExist(model.ServiceTypeName);
                if (getReferralServiceDataExist)
                {
                    return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ExistReferralServiceType, ErrorCode = enErrorCode.Status32021ReferralServiceTypeExist });
                }

                var obj = new ReferralServiceTypeViewModel()
                {
                    ServiceTypeName = model.ServiceTypeName
                };

                long id = _ReferralServiceType.AddReferralServiceType(obj, user.Id);
                if (id != 0)
                {
                    return new OkObjectResult(
                    new ReferralServiceTypeResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.SuccessfullReferralServiceType
                    });
                }
                else
                {
                    return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralServiceType, ErrorCode = enErrorCode.Status32013InvalidReferralServiceType });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetReferralServiceType")]
        [Authorize]
        public async Task<ActionResult> GetReferralServiceType(ReferralServiceTypeStatusViewModel model)
        {
            try
            {
                var getReferralServiceData = await Task.FromResult(_ReferralServiceType.GetReferralServiceType(model.Id));
                if (getReferralServiceData != null)
                    return Ok(new ReferralServiceTypeObjResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetReferralServiceType, ReferralServiceTypeObj = getReferralServiceData });
                else
                    return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.GetFailReferralServiceType, ErrorCode = enErrorCode.Status32014GetFailReferralServiceType });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ListReferralServiceType")]
        [Authorize]
        public async Task<ActionResult> ListReferralServiceType()
        {
            try
            {
                var getReferralServiceData = await Task.FromResult(_ReferralServiceType.ListReferralServiceType());
                if (getReferralServiceData != null)
                    return Ok(new ReferralServiceTypeListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralSeriesTypeList, ReferralServiceTypeList = getReferralServiceData });
                else
                    return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralSeriesTypeList, ErrorCode = enErrorCode.Status32015FailReferralSeriesTypeList });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("DropDownReferralServiceType")]
        [Authorize]
        public async Task<ActionResult> DropDownReferralServiceType()
        {
            try
            {
                var getReferralServiceData = await Task.FromResult(_ReferralServiceType.DropDownReferralServiceType());
                if (getReferralServiceData != null)
                    return Ok(new ReferralServiceTypeDropDownResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralSeriesTypeDropDown, ReferralServiceTypeDropDownList = getReferralServiceData });
                else
                    return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralSeriesTypeDropDown, ErrorCode = enErrorCode.Status32016FailReferralSeriesTypeDropDown });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("UpdateReferralServiceType")]
        [Authorize]
        public async Task<IActionResult> UpdateReferralServiceType(ReferralServiceTypeUpdateViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                if (string.IsNullOrEmpty(model.ServiceTypeName))
                {
                    return BadRequest(new ReferralPayTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralServiceType, ErrorCode = enErrorCode.Status32013InvalidReferralServiceType });
                }
                bool getReferralServiceDataExist = _ReferralServiceType.IsReferralServiceTypeExist(model.ServiceTypeName);
                if (getReferralServiceDataExist)
                {
                    return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ExistReferralServiceType, ErrorCode = enErrorCode.Status32021ReferralServiceTypeExist });
                }

                var obj = new ReferralServiceTypeUpdateViewModel()
                {
                    Id = model.Id,
                    ServiceTypeName = model.ServiceTypeName
                };

                long id = _ReferralServiceType.UpdateReferralServiceType(obj, user.Id);
                if (id != 0)
                {
                    return new OkObjectResult(
                    new ReferralServiceTypeResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.SuccessfullUpdateReferralServiceType
                    });
                }
                else
                {
                    return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidUpdateReferralServiceType, ErrorCode = enErrorCode.Status32017InvalidUpdateReferralServiceType });

                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ReferralServiceTypeExist")]
        [Authorize]
        public async Task<ActionResult> IsReferralServiceTypeExist(ReferralServiceTypeViewModel model)
        {
            try
            {
                bool getReferralServiceData = await Task.FromResult(_ReferralServiceType.IsReferralServiceTypeExist(model.ServiceTypeName));
                if (getReferralServiceData)
                    return Ok(new ReferralServiceTypeExistResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ExistReferralServiceType, ReferralServiceTypeExist = true });
                else
                    return Ok(new ReferralServiceTypeExistResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.NotExistReferralServiceType, ReferralServiceTypeExist = false });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("DisableReferralServiceType")]
        [Authorize]
        public async Task<IActionResult> DisableReferralServiceType(ReferralServiceTypeStatusViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var obj = new ReferralServiceTypeStatusViewModel()
                {
                    Id = model.Id
                };
                bool id = _ReferralServiceType.DisableReferralServiceType(obj, user.Id);
                if (id == true)
                {
                    return new OkObjectResult(
                    new ReferralServiceTypeResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.DisableReferralServiceTypeStatus
                    });
                }
                else
                {
                    return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralServiceTypeStatus, ErrorCode = enErrorCode.Status32018FailReferralServiceTypeStatus });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("EnableReferralServiceType")]
        [Authorize]
        public async Task<IActionResult> EnableReferralServiceType(ReferralServiceTypeStatusViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var obj = new ReferralServiceTypeStatusViewModel()
                {
                    Id = model.Id

                };
                bool id = _ReferralServiceType.EnableReferralServiceType(obj, user.Id);
                if (id == true)
                {
                    return new OkObjectResult(
                    new ReferralServiceTypeResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.EnableReferralServiceTypeStatus

                    });
                }
                else
                {
                    return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralServiceTypeStatus, ErrorCode = enErrorCode.Status32018FailReferralServiceTypeStatus });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralServiceTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        [Authorize]
        [HttpPost("GetReferralURL")]
        public async Task<IActionResult> GetReferralURL()
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                string ReferralCode = await _ReferralUser.GetUserReferralCode(user.Id);
                long GetServiceId = await Task.FromResult(_ReferralService.ReferralServiceId());

                ReferralChannelShareViewModel objShare = new ReferralChannelShareViewModel();
                objShare.ReferralCode = ReferralCode;
                objShare.ServiceId = GetServiceId;
                objShare.ChannelTypeId = 1;

                var shareURL = _ReferralChannel.GetReferralURL(objShare, user.Id);

                if (shareURL != null)
                {
                    return new OkObjectResult(
                       new ReferralShareURLResponse
                       {
                           ReturnCode = enResponseCode.Success,
                           ReturnMsg = EnResponseMessage.ReferralURL,
                           ShareURL = shareURL
                       });
                }
                else
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralURL, ErrorCode = enErrorCode.Status32057FailReferralURL });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [Authorize]
        [HttpPost("SendReferralEmail")]
        public async Task<IActionResult> SendReferralEmail(SendReferralEmailRequest Request)
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

                if (Request.EmailAddress.Count() > 0)
                {
                    var SendEmail = _ReferralChannel.SendReferralEmail(Request, user);

                    if (SendEmail == 1)
                    {
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ReturnMsg = EnResponseMessage.ReferralEmailSendSuccess;
                    }
                    else if (SendEmail == 4) //Houly limit exceed
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.Status32045ReferralEmailHourlyLimitExceed;
                        Response.ReturnMsg = EnResponseMessage.ReferralEmailHourlyLimitExceed;
                    }
                    else if (SendEmail == 5) //Daily Limit exceed
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.Status32046ReferralEmailDailyLimitExceed;
                        Response.ReturnMsg = EnResponseMessage.ReferralEmailDailyLimitExceed;
                    }
                    else if (SendEmail == 6) //Daily Limit exceed
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.Status32047ReferralEmailWeeklyLimitExceed;
                        Response.ReturnMsg = EnResponseMessage.ReferralEmailWeeklyLimitExceed;
                    }
                    else if (SendEmail == 7) //Daily Limit exceed
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.Status32048ReferralEmailMonthlyLimitExceed;
                        Response.ReturnMsg = EnResponseMessage.ReferralEmailMonthlyLimitExceed;
                    }
                    return Ok(Response);

                }
                else
                {
                    return Ok(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralSendEmailBlankRequest, ErrorCode = enErrorCode.Status32043ReferralSendEmailBlankRequest });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [Authorize]
        [HttpPost("SendReferralSMS")]
        public async Task<IActionResult> SendReferralSMS(SendReferralSMSRequest Request)
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
                if (Request.MobileNumber.Count() > 0)
                {
                    var SendSMS = _ReferralChannel.SendReferralSMS(Request, user);

                    if (SendSMS == 1)
                    {
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ReturnMsg = EnResponseMessage.ReferralSMSSendSuccess;
                    }
                    else if (SendSMS == 4) //Houly limit exceed
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.Status32049ReferralSMSHourlyLimitExceed;
                        Response.ReturnMsg = EnResponseMessage.ReferralSMSHourlyLimitExceed;
                    }
                    else if (SendSMS == 5) //Daily Limit exceed
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.Status32050ReferralSMSDailyLimitExceed;
                        Response.ReturnMsg = EnResponseMessage.ReferralSMSDailyLimitExceed;
                    }
                    else if (SendSMS == 6) //Daily Limit exceed
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.Status32051ReferralSMSWeeklyLimitExceed;
                        Response.ReturnMsg = EnResponseMessage.ReferralSMSWeeklyLimitExceed;
                    }
                    else if (SendSMS == 7) //Daily Limit exceed
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.Status32052ReferralSMSMonthlyLimitExceed;
                        Response.ReturnMsg = EnResponseMessage.ReferralSMSMonthlyLimitExceed;
                    }
                    return Ok(Response);
                }
                else
                {
                    return Ok(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralSendSMSBlankRequest, ErrorCode = enErrorCode.Status32044ReferralSendSMSBlankRequest });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralChannelTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [AllowAnonymous]
        [HttpPost("AddReferralUserClick")]
        public IActionResult AddReferralUserClick(AddReferralClickRequest Request)
        {
            try
            {
                var response = _ReferralChannel.AddReferralUserClick(Request);
                if (response != null)
                {
                    return Ok(new ReferralUserClickResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.CommRecordInsertSuccess, ReferralCode = response.ReferralCode,ReferralServiceId=response.ReferralServiceId,ReferralChannelTypeId=response.ReferralChannelTypeId });
                }
                else
                {
                    return BadRequest(new ReferralUserClickResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralUserClickData, ErrorCode = enErrorCode.Status32064InvalidReferralUserClickData });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #region ReferralSchemeTypeMapping CRUD

        [Authorize]
        [HttpPost("AddUpdateReferralSchemeTypeMapping")]
        public async Task<IActionResult> AddUpdateReferralSchemeTypeMapping(AddReferralSchemeTypeMappingReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
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
                    Response = await _ReferralService.AddUpdateReferralSchemeTypeMapping(Request,user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [Authorize]
        [HttpPost("ChangeReferralSchemeTypeMappingStatus/{Id}/{Status}")]
        public async Task<IActionResult> ChangeReferralSchemeTypeMappingStatus(long Id,ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
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
                    Response = await _ReferralService.ChangeReferralSchemeTypeMappingStatus(Id, Status, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [Authorize]
        [HttpPost("GetReferralSchemeTypeMappingByid/{Id}")]
        public async Task<IActionResult> GetReferralSchemeTypeMappingByid(long Id)
        {
            GetReferralSchemeTypeMappingRes Response = new GetReferralSchemeTypeMappingRes();
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
                    Response = await _ReferralService.GetReferralSchemeTypeMappingById(Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [Authorize]
        [HttpPost("ListReferralSchemeTypeMapping")]
        public async Task<IActionResult> ListReferralSchemeTypeMapping(long? PayTypeId,long? ServiceTypeMstId,short? Status)
        {
            ListReferralSchemeTypeMappingRes Response = new ListReferralSchemeTypeMappingRes();
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
                    Response = await _ReferralService.ListReferralSchemeTypeMapping(PayTypeId, ServiceTypeMstId, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region ReferralServiceDetail

        [Authorize]
        [HttpPost("AddUpdateReferralServiceDetail")]
        public async Task<IActionResult> AddUpdateReferralServiceDetail(AddServiceDetail Request)
        {
            BizResponseClass Response = new BizResponseClass();
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
                    Response = await _ReferralService.AddUpdateReferralServiceDetail(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [Authorize]
        [HttpPost("ChangeReferralServiceDetailStatus/{Id}/{Status}")]
        public async Task<IActionResult> ChangeReferralServiceDetailStatus(long Id, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
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
                    Response = await _ReferralService.ChangeReferralServiceDetailStatus(Id, Status, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [Authorize]
        [HttpPost("GetReferralServiceDetailByid/{Id}")]
        public async Task<IActionResult> GetReferralServiceDetailByid(long Id)
        {
            GetReferralServiceDetailRes Response = new GetReferralServiceDetailRes();
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
                    Response = await _ReferralService.GetReferralServiceDetailByid(Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [Authorize]
        [HttpPost("ListReferralServiceDetail")]
        public async Task<IActionResult> ListReferralServiceDetail(long? SchemeTypeMappingId, long? CreditWalletTypeId, short? Status)
        {
            ListReferralServiceDetailRes Response = new ListReferralServiceDetailRes();
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
                    Response = await _ReferralService.ReferralServiceDetail(SchemeTypeMappingId, CreditWalletTypeId, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Referral Rewards
        [HttpPost("ListAdminReferralRewards")]
        [Authorize]
        public async Task<ActionResult> ListAdminReferralRewards(int PageIndex = 0, int Page_Size = 0, long ReferralServiceId = 0, int UserId = 0, int TrnUserId =0, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (FromDate != null && FromDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralFromDateGreterThanToday, ErrorCode = enErrorCode.Status32065ReferralFromDateGreterThanToday });
                }

                if (ToDate != null && ToDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralToDateGreterThanToday, ErrorCode = enErrorCode.Status32066ReferralToDateGreterThanToday });
                }

                if (FromDate != null && ToDate != null && (FromDate > ToDate))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralCompareFromDateTodate, ErrorCode = enErrorCode.Status32067ReferralCompareFromDateTodate });
                }

                if ((FromDate != null && ToDate == null) || (FromDate == null && ToDate != null))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralBothDateFromDateTodateRequired, ErrorCode = enErrorCode.Status32068ReferralBothDateFromDateTodateRequired });
                }
                var getReferralRewardsData = await Task.FromResult(_ReferralRewards.ListAdminReferralRewards(PageIndex, Page_Size, ReferralServiceId, UserId, TrnUserId, FromDate, ToDate));
                if (getReferralRewardsData != null)
                    return Ok(new ReferralRewardsListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralRewardsListForAdmin, ReferralRewardsList = getReferralRewardsData.ReferralRewardsList, TotalCount = getReferralRewardsData.TotalCount });
                else
                    return BadRequest(new ReferralRewardsResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralRewardsListForAdmin, ErrorCode = enErrorCode.Status32058FailReferralUserClickListForAdmin });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralRewardsResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ListUserReferralRewards")]
        [Authorize]
        public async Task<ActionResult> ListUserReferralRewards(int PageIndex = 0, int Page_Size = 0, long ReferralServiceId = 0,int TrnUserId=0, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (FromDate != null && FromDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralFromDateGreterThanToday, ErrorCode = enErrorCode.Status32065ReferralFromDateGreterThanToday });
                }

                if (ToDate != null && ToDate > DateTime.UtcNow.Date)
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralToDateGreterThanToday, ErrorCode = enErrorCode.Status32066ReferralToDateGreterThanToday });
                }

                if (FromDate != null && ToDate != null && (FromDate > ToDate))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralCompareFromDateTodate, ErrorCode = enErrorCode.Status32067ReferralCompareFromDateTodate });
                }

                if ((FromDate != null && ToDate == null) || (FromDate == null && ToDate != null))
                {
                    return BadRequest(new ReferralChannelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ReferralBothDateFromDateTodateRequired, ErrorCode = enErrorCode.Status32068ReferralBothDateFromDateTodateRequired });
                }
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var getReferralRewardsData = await Task.FromResult(_ReferralRewards.ListUserReferralRewards(user.Id, PageIndex, Page_Size, ReferralServiceId, TrnUserId, FromDate, ToDate));
                if (getReferralRewardsData != null)
                    return Ok(new ReferralRewardsListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ReferralRewardsListForUser, ReferralRewardsList = getReferralRewardsData.ReferralRewardsList, TotalCount = getReferralRewardsData.TotalCount });
                else
                    return BadRequest(new ReferralRewardsResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailReferralRewardsListForUser, ErrorCode = enErrorCode.Status32062FailReferralRewardsListForUser });
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralRewardsResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("AddReferralRewards")]
        [Authorize]
        public async Task<IActionResult> AddReferralRewards(ReferralRewardsViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                var obj = new ReferralRewardsViewModel()
                {
                    UserId = model.UserId,
                    ReferralServiceId = model.ReferralServiceId,
                    ReferralPayRewards = model.ReferralPayRewards,
                    LifeTimeUserCount= model.LifeTimeUserCount,
                    NewUserCount = model.NewUserCount,
                    CommissionCroneID = model.CommissionCroneID,
                    CommissionCurrecyId = model.CommissionCurrecyId,
                    ReferralPayTypeId = model.ReferralPayTypeId,
                    SumChargeAmount = model.SumChargeAmount,
                    TransactionCurrencyId = model.TransactionCurrencyId,
                    SumOfTransaction = model.SumOfTransaction,
                    TrnUserId = model.TrnUserId,
                    FromWalletId = model.FromWalletId,
                    ToWalletId = model.ToWalletId,
                    TrnRefNo = model.TrnRefNo,
                    CommissionAmount = model.CommissionAmount,
                    TransactionAmount = model.TransactionAmount,
                    TrnDate = model.TrnDate
                };
                long id = _ReferralRewards.AddReferralRewards(obj, user.Id);
                if (id != 0)
                {
                    return new OkObjectResult(
                    new ReferralRewardsResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.SuccessfullAddReferralRewards
                    });
                }
                else
                {
                    return BadRequest(new ReferralRewardsResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReferralRewards, ErrorCode = enErrorCode.Status32063InvalidReferralRewards });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ReferralRewardsResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Helpers
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion

    }
}