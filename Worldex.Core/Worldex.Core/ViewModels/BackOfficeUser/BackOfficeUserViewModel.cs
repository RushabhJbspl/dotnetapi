using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.BackOfficeUser
{
    public class BackOfficeUserViewModel : TrackerViewModel
    {
        [StringLength(50, ErrorMessage = "1,Please Enter Valid Email Id,4008")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }
        [StringLength(50, ErrorMessage = "1,Please enter a valid User Name,4002")]
        public string Username { get; set; }

        [StringLength(50, ErrorMessage = "1,Please Enter Valid First Name,4004")]
        [Display(Name = "Firstname")]
        public string Firstname { get; set; }

        [StringLength(50, ErrorMessage = "1,Please Enter Valid Last Name,4006")]
        [Display(Name = "Lastname")]
        public string Lastname { get; set; }

        [Display(Name = "CountryCode")]
        [StringLength(5, ErrorMessage = "1,Please enter a valid Contry Code,4131")]
        public string CountryCode { get; set; }

        [StringLength(5, ErrorMessage = "1,Please enter a valid PreferedLanguage,4186")]
        public string PreferedLanguage { get; set; } = "en";
    }

    public class BackOfficeUserResponse : BizResponseClass
    {

    }
}
