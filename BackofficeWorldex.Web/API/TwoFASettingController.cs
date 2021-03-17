using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.UserChangeLog;
using Worldex.Core.ViewModels.AccountViewModels;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using Worldex.Core.ViewModels.ManageViewModels;
using Worldex.Core.ViewModels.ManageViewModels.TwoFA;
using Worldex.Core.ViewModels.ManageViewModels.UserChangeLog;
using Worldex.Infrastructure;
using Worldex.Infrastructure.Interfaces;
using BackofficeWorldex.Web.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwoFactorAuthNet;

namespace BackofficeWorldex.Web.API
{
    [Route("api/[controller]")]
    // [ApiExplorerSettings(IgnoreApi = true)]
    public class TwoFASettingController : BaseController
    {
        #region Field 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        //private readonly IBasePage _basePage; //komal 03 May 2019, Cleanup
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserChangeLog _iuserChangeLog;
        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private readonly ISignalRService _signalRService;
        #endregion

        #region Ctore
        public TwoFASettingController(
        UserManager<ApplicationUser> userManager,
        ILoggerFactory loggerFactory,
        UrlEncoder urlEncoder, SignInManager<ApplicationUser> signInManager, IUserChangeLog userChangeLog, ISignalRService signalRService)
        {
            _userManager = userManager;
          //  _logger = loggerFactory.CreateLogger<ManageController>();
            _urlEncoder = urlEncoder;
            _signInManager = signInManager;

            _iuserChangeLog = userChangeLog;
            _signalRService = signalRService;
        }
        #endregion

        #region Method

        [HttpPost("Twofactorauthentication")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return Ok(model);
        }

        //[HttpPost("Authentication2FA")]
        //public async Task<IActionResult> Authentication2FA(TwoFactorAuthViewModel model)
        //{

        //    if (model != null)
        //    {
        //        return Ok("Success");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Invalid");
        //        //  return BadRequest(new ApiError(ModelState));
        //    }

        //    var user = await GetCurrentUserAsync();


        //    var model1 = new TwoFactorAuthenticationViewModel
        //    {
        //        HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
        //        Is2faEnabled = user.TwoFactorEnabled,
        //        RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
        //    };

        //    return Ok(model);
        //}

        [HttpPost("Disable2fa")]
        public async Task<IActionResult> Disable2fa([FromBody]DisableAuthenticatorViewModel model)
        {
            var user = await GetCurrentUserAsync();
            try
            {
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                if (!user.TwoFactorEnabled)
                    return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.TwoFAalreadyDisable, ErrorCode = enErrorCode.Status4108TwoFAalreadydisable });

                TwoFactorAuth TFAuth = new TwoFactorAuth();
                //sKey = key; //TFAuth.CreateSecret(160);
                string code = TFAuth.GetCode(user.PhoneNumber);
                if (model.Code != code)
                //    bool status = TFAuth.VerifyCode(user.PhoneNumber, model.Code, 5);
                //if (!status)
                {
                    return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.TwoFactorVerificationDisable, ErrorCode = enErrorCode.Status4071TwoFactorVerificationDisable });
                }
                else
                {
                    //user.TwoFactorEnabled = true;
                    //await _userManager.UpdateAsync(user);
                    var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
                    //return Ok(new EnableAuthenticationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.EnableTwoFactor });
                    if (disable2faResult.Succeeded)
                    {
                        string oldvalue = JsonConvert.SerializeObject(user);
                        user.TwoFactorEnabled = false;
                        await _userManager.UpdateAsync(user);
                        string Newvalue = JsonConvert.SerializeObject(user);
                        UserChangeLogViewModel userChangeLogViewModel = new UserChangeLogViewModel();
                        userChangeLogViewModel.Id = user.Id;
                        userChangeLogViewModel.Newvalue = Newvalue;
                        userChangeLogViewModel.Type = EnuserChangeLog.TwofactoreChange.ToString();
                        userChangeLogViewModel.Oldvalue = oldvalue;

                        long userlog = _iuserChangeLog.AddPassword(userChangeLogViewModel);
                        //_logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);


                        // Start Notification Send login success full
                        ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                        ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.TwoFADeactiveNotification);
                        ActivityNotification.Param1 = user.UserName;
                        ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Success);
                        _signalRService.SendActivityNotificationV2(ActivityNotification, user.Id.ToString(), 2);
                        // End Notification Send login success full


                        return Ok(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DisableTroFactor });
                    }
                    else
                    {
                        return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DisableTroFactorError, ErrorCode = enErrorCode.Status4055DisableTroFactorError });
                    }
                }



                /*
                // Strip spaces and hypens
                var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

                var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                    user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

                if (!is2faTokenValid)
                {
                    return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.TwoFactorVerificationDisable, ErrorCode = enErrorCode.Status4071TwoFactorVerificationDisable });

                }
                else
                {
                    var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
                    if (disable2faResult.Succeeded)
                    {
                        string oldvalue = JsonConvert.SerializeObject(user);
                        user.TwoFactorEnabled = false;
                        await _userManager.UpdateAsync(user);
                        string Newvalue = JsonConvert.SerializeObject(user);
                        UserChangeLogViewModel userChangeLogViewModel = new UserChangeLogViewModel();
                        userChangeLogViewModel.Id = user.Id;
                        userChangeLogViewModel.Newvalue = Newvalue;
                        userChangeLogViewModel.Type = EnuserChangeLog.TwofactoreChange.ToString();
                        userChangeLogViewModel.Oldvalue = oldvalue;

                        long userlog = _iuserChangeLog.AddPassword(userChangeLogViewModel);
                        _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
                        return Ok(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DisableTroFactor });
                    }
                    else
                    {
                        return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DisableTroFactorError, ErrorCode = enErrorCode.Status4055DisableTroFactorError });
                    }
                }
                */

            }
            catch (Exception ex)
            {
                return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("Disable2faUserwise")]
        public async Task<IActionResult> Disable2faUserwise([FromBody]DisableAuthenticatorViewModelV1 model)
        {
            var user = await GetCurrentUserAsync();
            try
            {
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                if (model.UserId < 0)
                    return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidUserIdv1, ErrorCode = enErrorCode.Status4072UserIdNotValid });

                var user1 = await _userManager.FindByIdAsync(model.UserId.ToString());
                if (user1 != null)
                {
                    if (!user1.TwoFactorEnabled)
                        return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.TwoFAalreadyDisable, ErrorCode = enErrorCode.Status4108TwoFAalreadydisable });

                    user1.TwoFactorEnabled = false;
                    // user.LockoutEnd = null;
                    var userUpdate = await _userManager.UpdateAsync(user1);
                    if (userUpdate.Succeeded)
                    {
                        HttpContext.Items["UserId"] = user.Id;

                        return Ok(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DisableTroFactor });
                    }
                    else
                    {
                        return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DisableTroFactorError, ErrorCode = enErrorCode.Status4055DisableTroFactorError });
                    }
                }
                else
                {
                    return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });
                }


            }
            catch (Exception ex)
            {
                return BadRequest(new DisableAuthenticatorResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpGet("Enableauthenticator")]
        public async Task<IActionResult> EnableAuthenticator()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new EnableAuthenticationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;
                //// Update Status

                string oldvalue = JsonConvert.SerializeObject(user);
                //user.TwoFactorEnabled = true;
                //await _userManager.UpdateAsync(user);

                //// Update Status

                //return Ok(new TwoFactorAuthResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.EnableTroFactor });


                var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                if (string.IsNullOrEmpty(unformattedKey))
                {
                    await _userManager.ResetAuthenticatorKeyAsync(user);
                    unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                }
                string Newvalue = JsonConvert.SerializeObject(user);
                UserChangeLogViewModel userChangeLogViewModel = new UserChangeLogViewModel();
                userChangeLogViewModel.Id = user.Id;
                userChangeLogViewModel.Newvalue = Newvalue;
                userChangeLogViewModel.Type = EnuserChangeLog.TwofactoreChange.ToString();
                userChangeLogViewModel.Oldvalue = oldvalue;

                long userlog = _iuserChangeLog.AddPassword(userChangeLogViewModel);

                TwoFactorAuth TFAuth = new TwoFactorAuth();
                //string URL;
                string sKey = string.Empty;
                //  string sName = string.Empty;
                //sKey = TFAuth.CreateSecret(160);
                // sName = user.UserName; // dSetReq.Tables(0).Rows(0)("NAME");
                sKey = TFAuth.CreateSecret(160);
                //URL = TFAuth.GetQrCodeImageAsDataUri(sName, sKey);
                // string value = URL + "" + sKey;
                // string code123 = TFAuth.GetQrCodeImageAsDataUri(, string secret)
                user.PhoneNumber = sKey;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var model = new EnableAuthenticatorViewModel
                    {
                        SharedKey = FormatKey(sKey),
                        //AuthenticatorUri = GenerateQrCodeUri(user.UserName, unformattedKey)
                        // UserName = user.UserName,
                        AuthenticatorUri = TFAuth.GetQrCodeImageAsDataUri(user.UserName, sKey)
                    };
                    return Ok(new EnableAuthenticationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.TwoFactorActiveRequest, EnableAuthenticatorViewModel = model });
                }

                return BadRequest(new EnableAuthenticationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.TwoFactorActiveRequest, ErrorCode = enErrorCode.NotFound });

                //if (string.IsNullOrEmpty(user.Email))   ////  This Condition by pankaj for when user login with molile the email field is null so.
                //{                   
                //        var model = new EnableAuthenticatorViewModel
                //        {
                //            SharedKey = FormatKey(unformattedKey),
                //            AuthenticatorUri = GenerateQrCodeUri(user.UserName, unformattedKey)

                //        };
                //        return Ok(new EnableAuthenticationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.TwoFactorActiveRequest, EnableAuthenticatorViewModel = model });                   
                //}
                //else
                //{

                //    var model = new EnableAuthenticatorViewModel
                //    {
                //        SharedKey = FormatKey(unformattedKey),
                //        AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
                //    };
                //    return Ok(new EnableAuthenticationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.TwoFactorActiveRequest, EnableAuthenticatorViewModel = model });

                //}

            }
            catch (Exception ex)
            {
                return BadRequest(new TwoFactorAuthResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("Enableauthenticator")]
        public async Task<IActionResult> EnableAuthenticator([FromBody]EnableAuthenticatorCodeViewModel model)
        {
            try
            {
                // var user = await _userManager.FindByNameAsync(model.UserName);
                var user = await GetCurrentUserAsync();

                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new EnableAuthenticationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                if (user != null)
                {
                    TwoFactorAuth TFAuth = new TwoFactorAuth();
                    //sKey = key; //TFAuth.CreateSecret(160);
                    string code = TFAuth.GetCode(user.PhoneNumber);
                    if (model.Code == code)
                    //    bool st = TFAuth.VerifyCode(user.PhoneNumber, model.Code, 5);
                    //if (st)
                    {
                        user.TwoFactorEnabled = true;
                        await _userManager.UpdateAsync(user);

                        // Start Notification Send login success full
                        ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                        ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.TwoFAActiveNotification);
                        ActivityNotification.Param1 = user.UserName;
                        ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Success);
                        _signalRService.SendActivityNotificationV2(ActivityNotification, user.Id.ToString(), 2);
                        // End Notification Send login success full

                        return Ok(new EnableAuthenticationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.EnableTwoFactor });
                    }
                    else
                        return BadRequest(new EnableAuthenticationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.TwoFactorVerification, ErrorCode = enErrorCode.Status4079TwoFAcodeInvalide });
                }
                return BadRequest(new EnableAuthenticationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.TwoFactorVerification, ErrorCode = enErrorCode.Status4079TwoFAcodeInvalide });

                //var user = await GetCurrentUserAsync();
                //// Strip spaces and hypens
                //var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

                //var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                //    user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

                //if (!is2faTokenValid)
                //{
                //    return BadRequest(new EnableAuthenticationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.TwoFactorVerification, ErrorCode = enErrorCode.Status4079TwoFAcodeInvalide });
                //}

                //await _userManager.SetTwoFactorEnabledAsync(user, true);
                //return Ok(new EnableAuthenticationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.EnableTwoFactor });
            }
            catch (Exception ex)
            {
                return BadRequest(new TwoFactorAuthResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //[HttpPost("resetauthenticator")]
        //public async Task<IActionResult> ResetAuthenticator()
        //{
        //    var user = await GetCurrentUserAsync();

        //    await _userManager.SetTwoFactorEnabledAsync(user, false);
        //    await _userManager.ResetAuthenticatorKeyAsync(user);
        //    _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

        //    return NoContent();
        //}

        //[HttpPost("generaterecoverycodes")]
        //public async Task<IActionResult> GenerateRecoveryCodes()
        //{
        //    var user = await GetCurrentUserAsync();

        //    if (!user.TwoFactorEnabled)
        //    {
        //        return BadRequest(new ApiError($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled."));
        //    }

        //    var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        //    var model = new GenerateRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

        //    _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

        //    return Ok(model);
        //}
        //

        [HttpPost("VerifyCode")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            try
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new VerifyCodeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                var authenticatorCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

                var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, model.RememberMe, model.RememberBrowser);
                if (result.Succeeded)
                {
                    // return RedirectToLocal(model.ReturnUrl);
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid code.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new VerifyCodeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        #endregion

        #region Utility 

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

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                _urlEncoder.Encode("Worldex"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        #endregion
    }
}