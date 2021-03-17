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
using Worldex.Web.API;
using System.Text.RegularExpressions;

namespace CleanArchitecture.Web.API.BackOffice
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    //[Authorize("MustHaveAuthority")] // Commented this as currently Method n Controller list not added. -Nishit Jani on A 2019-04-02 12:49 PM
    public class BackofficeRoleManagementController : BaseController
    {
        #region ctor

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly ISignupLogService _IsignupLogService;
        private readonly IMessageService _messageService;
        private readonly IEmailMaster _IemailMaster;
        private readonly Iphonemaster _Iphonemaster;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly IUserPasswordPolicyMaster _userPasswordPolicyMaster; //  khushali 05-03-2019 for Add Reset password  module
        private readonly IUserLinkMaster _userLinkMaster;  //  khushali 05-03-2019 for Add Reset password module

        public BackofficeRoleManagementController(UserManager<ApplicationUser> userManager, EncyptedDecrypted encdecAEC,
            ISignupLogService IsignupLogService, IEmailMaster IemailMaster, Iphonemaster Iphonemaster,
            Microsoft.Extensions.Configuration.IConfiguration configuration, IMessageService MessageService,
            IUserPasswordPolicyMaster UserPasswordPolicyMaster, IUserLinkMaster UserLinkMaster, IPushNotificationsQueue<SendEmailRequest> PushNotificationsQueue)
        {
            _userManager = userManager;
            _configuration = configuration;
            _encdecAEC = encdecAEC;
            _IsignupLogService = IsignupLogService;
            _messageService = MessageService;
            _IemailMaster = IemailMaster;
            _Iphonemaster = Iphonemaster;
            _userPasswordPolicyMaster = UserPasswordPolicyMaster;
            _userLinkMaster = UserLinkMaster;
            _pushNotificationsQueue = PushNotificationsQueue;
        }

        #endregion

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


    }
}