using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.OTP
{
    public class OTPWithMobileViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please enter a OTP number,400")]
        [StringLength(6, ErrorMessage = "1,The OTP must be between 6 digits,400", MinimumLength = 6)]
        [Range(6, Int64.MaxValue)]
        public string OTP { get; set; }

        [Required(ErrorMessage = "1,Please Enter Mobile Number, 4012")]
        [Phone(ErrorMessage = "1,Please Enter Valid Mobile Number, 4013")]
        public string Mobile { get; set; }

        [StringLength(5, ErrorMessage = "1,Please enter a valid PreferedLanguage,4186")]
        public string PreferedLanguage { get; set; } = "en";
    }

    public class OTPWithMobileResponse : BizResponseClass
    {

    }

    public class OTPMobileVerificationResponse : BizResponseClass
    {
        public bool Thememode { get; set; }
        public string PreferedLanguage { get; set; }
    }


    public class OtpWithMobile2FAResponse : BizResponseClass
    {
        public string TwoFAToken { get; set; }
        public string AllowToken { get; set; }
    }
}
