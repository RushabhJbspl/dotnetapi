using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.SignUp
{
    public class SignUpWithMobileViewModel : TrackerViewModel
    {      
        [Required(ErrorMessage = "1,Please Enter Mobile Number, 4012")]
        [Phone(ErrorMessage = "1,Please Enter Valid Mobile Number, 4013")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "1,Please Provide country code,4132")]
        [Display(Name = "CountryCode")]
        [StringLength(5, ErrorMessage = "1,Please enter a valid Contry Code,4131")]
        public string CountryCode { get; set; }
        [StringLength(5, ErrorMessage = "1,Please enter a valid PreferedLanguage,4186")]
        public string PreferedLanguage { get; set; } = "en";
        public string ReferralCode { get; set; }
        public int ReferralServiceId { get; set; }
        public int ReferralChannelTypeId { get; set; }
        public bool Thememode { get; set; } = false;//Rita 29-6-19 default value
    }

    public class SignUpWithMobileResponse : BizResponseClass
    {

    }
}
