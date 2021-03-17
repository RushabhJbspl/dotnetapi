using Worldex.Core.ApiModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.ResetPassword
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "1, The {0} must be at least {2} and at max {1} characters long.,4024", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "2,The password and confirmation password do not match.,4027")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

        public DateTime Expirytime { get; set; }
    }
    public class   ResetPassWordResponse : BizResponseClass
    {

    }
    public class ForgotPassWordResponse : BizResponseClass
    {
        public Guid Id { get; set;   }
    }
}
