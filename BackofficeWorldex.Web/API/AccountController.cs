using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using AspNet.Security.OpenIdConnect.Primitives;
using CleanArchitecture.Web.Filters;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.ViewModels.AccountViewModels;
using PhoneNumbers;
using CleanArchitecture.Core.ViewModels.AccountViewModels.Login;
using CleanArchitecture.Core.ViewModels.AccountViewModels.SignUp;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.ViewModels.AccountViewModels.ForgotPassword;
using CleanArchitecture.Core.ViewModels.AccountViewModels.ResetPassword;
using CleanArchitecture.Core.ViewModels.AccountViewModels.OTP;
using CleanArchitecture.Core.Interfaces.User;

namespace CleanArchitecture.Web.API
{
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        /*
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IUserService _userdata;
        private readonly IMessageSender _messageSender;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> identityOptions,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILoggerFactory loggerFactory,
            IUserService userdata,
            IMessageSender messageSender)
        {
            _userManager = userManager;
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _userdata = userdata;
            _messageSender = messageSender;
        }
        */

        #region Login

        /*
        [HttpPost("login")]
        [AllowAnonymous]
        [ApiExplorerSettings()]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);
                _logger.LogInformation(1, "User logged in.");
                return AppUtils.SignIn(user, roles);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { RememberMe = model.RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(2, "User account locked out.");
                return BadRequest(new ApiError("Lockout"));
            }
            else
            {
                return BadRequest(new ApiError("Invalid login attempt."));
            }

        }
        
        #region Standerd Login
        /// <summary>
        /// Thid method are used standard login 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("StandardLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> StandardLogin([FromBody]StandardLoginViewModel model)
        {
            StandardLoginResponse response = new StandardLoginResponse();
            response.ReturnCode = 200;
            response.ReturnMsg = "Success";
            response.StatusCode = 200;
            response.StatusMessage = "Success";
            return Ok(response);
        }
        #endregion
        
        #region Login With Email
        /// <summary>
        /// This method are used login with notify to your email. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("LoginWithEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithEmail([FromBody]LoginWithEmailViewModel model)
        {
            LoginWithEmailResponse response = new LoginWithEmailResponse();
            response.ReturnCode = 200;
            response.ReturnMsg = "Success";
            response.StatusCode = 200;
            response.StatusMessage = "Success";
            return Ok(response);
        }
        #endregion

        #region Login With Mobile
        /// <summary>
        /// This method are used login with otp base verify 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("LoginWithMobile")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithMobile([FromBody]LoginWithMobileViewModel model)
        {
            LoginWithMobileResponse response = new LoginWithMobileResponse();
            response.ReturnCode = 200;
            response.ReturnMsg = "Success";
            response.StatusCode = 200;
            response.StatusMessage = "Success";
            return Ok(response);
        }
        #endregion
        
        #region Social Login

        ///// <summary>
        /////  This method are used social media using to login.
        ///// </summary>  
        //[HttpPost("SocialLogin")]
        //[AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        //public IActionResult Sociallogin([FromBody] SocialLoginWithEmailViewModel model, string returnUrl = null)
        //{
        //    var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(model.ProviderName, redirectUrl);
        //    return new ChallengeResult(model.ProviderName, properties);
        //}

        /// <summary>
        ///  This method are used social media using to login.
        /// </summary>  
        [HttpPost("SocialLogin")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public IActionResult Sociallogin(string ProviderName, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(ProviderName, redirectUrl);
            return new ChallengeResult(ProviderName, properties);
        }

        [HttpGet("ExternalLoginCallback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                //return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new SocialLoginWithEmailViewModel { Email = email });
            }
        }
        #endregion
        */

        #endregion
                       
        #region SignUp
           /*
        #region Default register
            
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model, string returnUrl = null)
        {
            var currentUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.Firstname,
                LastName = model.Lastname,
                Mobile = model.Mobile
                //Balance = model.Balance
            };

            var result = await _userManager.CreateAsync(currentUser, model.Password);
            if (result.Succeeded)
            {

                //// ASP.NET Identity does not remember claim value types. So, if it was important that the office claim be an integer(rather than a string)
                var officeClaim = new Claim(OpenIdConnectConstants.Claims.PhoneNumber, currentUser.Mobile.ToString(), ClaimValueTypes.Integer);

                await _userManager.AddClaimAsync(currentUser, officeClaim);

                // Add to roles
                var roleAddResult = await _userManager.AddToRoleAsync(currentUser, "User");
                if (roleAddResult.Succeeded)
                {
                    string ctoken = _userManager.GenerateEmailConfirmationTokenAsync(currentUser).Result;
                    string ctokenlink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userId = currentUser.Id,
                        emailConfirmCode = ctoken
                    }, protocol: HttpContext.Request.Scheme);

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(currentUser);

                    //var host = Request.Scheme + "://" + Request.Host;
                    //var callbackUrl = host+ "/api/Account/ConfirmEmail" + "?userId=" + currentUser.Id + "&emailConfirmCode=" + code;
                    var confirmationLink = "<a class='btn-primary' href=\"" + ctokenlink + "\">Confirm email address</a>";
                    _logger.LogInformation(3, "User created a new account with password.");
                    await _emailSender.SendEmailAsync(model.Email, "Registration confirmation email", confirmationLink);
                    return Ok("User created a new account with password.");
                }
            }
            AddErrors(result);
            // If we got this far, something failed, redisplay form
            return BadRequest(new ApiError(ModelState));
        }
        
        #endregion

        #region DirectSignUpWithEmail
        
        /// <summary>
        ///  This method are Direct signUp with email using verified link
        /// </summary>        
        [HttpPost("DirectSignUpWithEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> DirectSignUpWithEmail([FromBody]SignUpWithEmailViewModel model)
        {
            var checkemail = await _userManager.FindByEmailAsync(model.Email);
            if (string.IsNullOrEmpty(checkemail?.Email))
            {
                var currentUser = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email
                };

                
                //var result = await _userManager.CreateAsync(currentUser);
                //if (result.Succeeded)
                //{
                //    // Add to roles
                //    var roleAddResult = await _userManager.AddToRoleAsync(currentUser, "User");

                //    if (roleAddResult.Succeeded)
                //    {
                //        string ctoken = _userManager.GenerateEmailConfirmationTokenAsync(currentUser).Result;
                //        string ctokenlink = Url.Action("ConfirmEmail", "Account", new
                //        {
                //            userId = currentUser.Id,
                //            emailConfirmCode = ctoken
                //        }, protocol: HttpContext.Request.Scheme);

                //        var confirmationLink = "<a class='btn-primary' href=\"" + ctokenlink + "\">Confirm email address</a>";
                //        _logger.LogInformation(3, "User created a new account with password.");
                //        await _emailSender.SendEmailAsync(model.Email, "Registration confirmation email", confirmationLink);

                //        SignUpMobileWithOTPResponse response = new SignUpMobileWithOTPResponse();
                //        response.ReturnCode = 200;
                //        response.StatusMessage = "Success";
                //        response.StatusCode = 200;
                //        response.ReturnMsg = "Your account has been created, <br /> please verify it by clicking the activation link that has been send to your email.";
                                             
                //        return Ok(response);
                //    }
                //}
                //AddErrors(result);
                
               
                SignUpMobileWithOTPResponse response = new SignUpMobileWithOTPResponse();
                response.ReturnCode = 200;
                response.StatusMessage = "Success";
                response.StatusCode = 200;
                response.ReturnMsg = "Your account has been created, <br /> please verify it by clicking the activation link that has been send to your email.";

                return Ok(response);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "This email is already registered.");
                return BadRequest(new ApiError(ModelState));
            }
            // If we got this far, something failed, redisplay form
            return BadRequest(new ApiError(ModelState));
        }

        #endregion

        #region DirectSignUpWithMobile

        /// <summary>
        ///  This method are Direct signUp with mobile sms using verified opt.
        /// </summary>        
        [HttpPost("DirectSignUpWithMobile")]
        [AllowAnonymous]
        public async Task<IActionResult> DirectSignUpWithMobile([FromBody]SignUpWithMobileViewModel model)
        {
            SignUpMobileWithOTPResponse response = new SignUpMobileWithOTPResponse();

            PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();

            string countryCode = "IN";
            PhoneNumbers.PhoneNumber phoneNumber = phoneUtil.Parse(model.Mobile, countryCode);


            bool isValidNumber = phoneUtil.IsValidNumber(phoneNumber); // returns true for valid number    
            if (!isValidNumber)
            {
                response.ReturnCode = 200;
                response.ReturnMsg = "This mobile number is not Valid";
                response.ErrorCode = 401;
                response.StatusCode = 200;
                response.StatusMessage = "error";
                return Ok(response);
                //   return Ok("This mobile number is  Valid");
            }



            bool IsSignMobile = _userdata.GetMobileNumber(model.Mobile);
            if (IsSignMobile)
            {
                
                //var currentUser = new ApplicationUser
                //{
                //    Mobile = model.Mobile,
                //    UserName = model.Mobile,
                //    OTP = _userdata.GenerateRandomOTP()
                //};

                //var result = await _userManager.CreateAsync(currentUser);
                //if (result.Succeeded)
                //{
                //    var officeClaim = new Claim(OpenIdConnectConstants.Claims.PhoneNumber, currentUser.Mobile, ClaimValueTypes.Integer);
                //    await _userManager.AddClaimAsync(currentUser, officeClaim);
                //    // Add to roles
                //    var roleAddResult = await _userManager.AddToRoleAsync(currentUser, "User");

                //    if (roleAddResult.Succeeded)
                //    {
                //        //await _messageSender.SendSMSAsync(model.Mobile, "");
                //        SignUpMobileWithOTPResponse response = new SignUpMobileWithOTPResponse();
                //        response.ReturnCode = 200;
                //        response.ReturnMsg = "Success";
                //        response.StatusCode = 200;
                //        response.StatusMessage = "Done";
                //        return Ok(response);
                //    }
                //}
                //AddErrors(result);
                
                response.ReturnCode = 200;
                response.ReturnMsg = "Success";
                response.StatusCode = 200;
                response.StatusMessage = "Done";
                return Ok(response);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "This mobile number is already registered.");
                return BadRequest(new ApiError(ModelState));
            }
            // If we got this far, something failed, redisplay form
            return BadRequest(new ApiError(ModelState));
        }

        #endregion
        
        #region BlockChainSignUp

        [HttpPost("BlockChainSignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> BlockChainSignUp([FromBody] BlockChainViewModel model)
        {
            BlockChainResponse response = new BlockChainResponse();
            response.ReturnCode = 200;
            response.ReturnMsg = "Success";
            response.StatusCode = 200;
            response.StatusMessage = "Done";

            return Ok(response);
        }

        #endregion
        */
        #endregion

        #region SignUpOtpVerification
        /*
        /// <summary>
        ///  This method are Direct signUp with mobile sms using verified opt.
        /// </summary>        
        [HttpPost("DirectSignUpOtpVerification")]
        [AllowAnonymous]
        public async Task<IActionResult> DirectSignUpOtpVerification([FromBody]OTPViewModel model)
        {

            if (model!= null)
            {
                SignUpMobileWithOTPResponse response = new SignUpMobileWithOTPResponse();
                response.ReturnCode = 200;
                response.ReturnMsg = "Success";
                response.StatusCode = 200;
                response.StatusMessage = "Done";
                return Ok(response);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP.");
                return BadRequest(new ApiError(ModelState));
            }

        }
        */
        #endregion


        /*
        [HttpGet("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string emailConfirmCode)
        {
            if (userId == null || emailConfirmCode == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, emailConfirmCode);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }*/
        /*
        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordViewModel model)
        {
            var currentUser = await _userManager.FindByNameAsync(model.Email);
            if (currentUser == null || !(await _userManager.IsEmailConfirmedAsync(currentUser)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return NoContent();
            }
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            // Send an email with this link
            var code = await _userManager.GeneratePasswordResetTokenAsync(currentUser);

            //string ctoken = _userManager.GeneratePasswordResetTokenAsync(currentUser).Result;
            //string ctokenlink = Url.Action("ResetPassword", "Account", new
            //{
            //    Email = currentUser.Email,
            //    emailConfirmCode = ctoken
            //}, protocol: HttpContext.Request.Scheme);


            var host = Request.Scheme + "://" + Request.Host;
            var callbackUrl = host + "?userId=" + currentUser.Id + "&passwordResetCode=" + code;
            var confirmationLink = "<a class='btn-primary' href=\"" + callbackUrl + "\">Reset your password</a>";
            //var confirmationLink = "<a class='btn-primary' href=\"" + ctokenlink + "\">Reset your password</a>";
            await _emailSender.SendEmailAsync(model.Email, "Forgotten password email", confirmationLink);
            return NoContent(); // sends 204
        }

        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok("Reset confirmed");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return Ok("Reset confirmed"); ;
            }
            AddErrors(result);
            return BadRequest(new ApiError(ModelState));
        }
        */
        /*
        [HttpGet("SendCode")]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return BadRequest(new ApiError("Error"));
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost("SendCode")]
        [AllowAnonymous]
        public async Task<IActionResult> SendCode([FromBody]SendCodeViewModel model)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return BadRequest(new ApiError("Error"));
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new ApiError("Error"));
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(user.Email, "Security Code", message);
            }
            // else if (model.SelectedProvider == "Phone")
            // {
            //     await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            // }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }*/

            /*
        [HttpGet("VerifyCode")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return BadRequest(new ApiError("Error"));
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost("VerifyCode")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                // return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }
        */

            /*
        [HttpPost("logout")]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return NoContent();
        }
        */
        #region Helpers
            /*
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        */
        #endregion
    }
}
