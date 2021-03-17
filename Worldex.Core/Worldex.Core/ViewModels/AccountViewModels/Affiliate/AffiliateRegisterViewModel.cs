using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
    public class AffiliateRegisterViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please enter a User Name,4001")]
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage = "1,Please enter a valid User Name,4002")]
        public string Username { get; set; }

        [Required(ErrorMessage = "1,Please Enter First Name,4003")]
        [StringLength(50, ErrorMessage = "1,Please Enter Valid First Name,4004")]
        [Display(Name = "Firstname")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "1,Please Enter Last Name,4005")]
        [StringLength(50, ErrorMessage = "1,Please Enter Valid Last Name,4006")]
        [Display(Name = "Lastname")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "1,Please Enter Email Id,4007")]
        [StringLength(50, ErrorMessage = "1,Please Enter Valid Email Id,4008")]
        [RegularExpression(@"^[-a-zA-Z0-9~!$%^&*_=+}{\'?]+(\.[-a-zA-Z0-9~!$%^&*_=+}{\'?]+)*@([a-zA-Z0-9_][-a-zA-Z0-9_]*(\.[-a-zA-Z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-zA-Z]{2,3})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$", ErrorMessage = "1,Please enter a valid Email Address,4009")]
        //[EmailAddress(ErrorMessage ="1,Please Enter Valid Email Id, 4009")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "1,Please Enter Password,4010")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "1,The Password must be at least 6 and at max 50 characters long,17320", MinimumLength = 6)] //change by mansi-05-11-2019
       // [StringLength(50, ErrorMessage = "1,The {0} must be at least {2} and at max {1} characters long,4011", MinimumLength = 6)]
        [Display(Name = "Password")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$", ErrorMessage = "1,Invalid email or password.Try clicking 'Forgot Password' If you're having trouble signing in.,4028")]
        public string Password { get; set; }

        [Required(ErrorMessage = "1,Please Enter Mobile Number,4012")]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "1,Please Provide country code,4132")]
        [Display(Name = "CountryCode")]
        [StringLength(5, ErrorMessage = "1,Please enter a valid Contry Code,4131")]
        public string CountryCode { get; set; }

        public string ReferCode { get; set; }

        public long SchemeType { get; set; }
    }

    public class AffiliateRegisterResponse : BizResponseClass
    {

    }
}
