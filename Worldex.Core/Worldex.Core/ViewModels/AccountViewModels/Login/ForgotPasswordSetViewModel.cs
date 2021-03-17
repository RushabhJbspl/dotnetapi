using Worldex.Core.ApiModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.Login
{
    public class ForgotPasswordSetViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Password,4010")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "1,The Password must be at least 6 and at max 50 characters long,17321", MinimumLength = 6)]//change by mansi-05-11-2019
       // [StringLength(50, ErrorMessage = "1,The {0} must be at least {2} and at max {1} characters long,4011", MinimumLength = 6)]
        [Display(Name = "Password")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$", ErrorMessage = "1,Passwords must be at least 6 characters and contain at 3 of 4 of the following: upper case (A-Z) lower case (a-z) number (0-9) and special character (e.g. !@#$%^&*),4028")]
        public string Password { get; set; }

        [Display(Name = "Guid")]
        [Required(ErrorMessage = "1,Please enter the Forgot user id,4163")]
        public Guid Id { get; set; }
    }

    public class ForgotPasswordSetResponseViewModel : BizResponseClass
    {

    }
}
