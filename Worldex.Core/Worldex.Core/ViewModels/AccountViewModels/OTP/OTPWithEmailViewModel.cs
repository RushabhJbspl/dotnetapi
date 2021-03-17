using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.OTP
{
    public class OTPWithEmailViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please enter a OTP number,400")]
        [StringLength(6, ErrorMessage = "1,The OTP must be between 6 digits,400", MinimumLength = 6)]
        [Range(6, Int64.MaxValue)]
        public string OTP { get; set; }

        [Required(ErrorMessage = "1,Please enter a Email Address,4007")]
        [StringLength(50, ErrorMessage = "1,Please Enter Valid Email Id,4008")]
        [RegularExpression(@"^[-a-zA-Z0-9~!$%^&*_=+}{\'?]+(\.[-a-zA-Z0-9~!$%^&*_=+}{\'?]+)*@([a-zA-Z0-9_][-a-zA-Z0-9_]*(\.[-a-zA-Z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-zA-Z]{2,3})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$", ErrorMessage = "0,Please enter a valid Email Address,4009")]
        public string Email { get; set; }
        [StringLength(5, ErrorMessage = "1,Please enter a valid PreferedLanguage,4186")]
        public string PreferedLanguage { get; set; } = "en";
    }
    public class OTPWithEmailResponse : BizResponseClass
    {

    }

    public class OTPEmailVerificationResponse : BizResponseClass
    {
        public bool Thememode { get; set; }
        public string PreferedLanguage { get; set; }
    }
}
