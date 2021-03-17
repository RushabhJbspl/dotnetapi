namespace Worldex.Core.PasswordPolicy
{
  public  class passwordPolicy
    {
        public static string LoginWithEmail = "/api/Signin/LoginWithEmail";
        public static string LoginWithMobile = "/api/Signin/LoginWithMobile";
        public static string PasswordExpire = "/api/Signin/setpassword";
        public static string LinkExpire = "/api/Signin/ForgotPassword";
        public static string DefaultPolicy = "/api/SignUp/ConfirmEmail";
        // added by nirav savariya for user profile get history
        public static string AddProfile = "/api/Profile/AddProfile";
        public static string ResendOTPWithMobile = "/api/Signin/ReSendOtpWithMobile";
        public static string ResendOTPWithEmail = "/api/Signin/ReSendOtpWithEmail";

    }
}
