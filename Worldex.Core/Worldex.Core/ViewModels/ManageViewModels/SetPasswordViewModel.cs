using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.ManageViewModels
{
    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "1,The {0} must be at least {2} and at max {1} characters long.,4024", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "2,The new password and confirmation password do not match.,4027")]
        public string ConfirmPassword { get; set; }

    }
}
