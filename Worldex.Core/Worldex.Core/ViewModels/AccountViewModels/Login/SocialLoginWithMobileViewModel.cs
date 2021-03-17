using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.Login
{
    public class SocialLoginWithMobileViewModel
    {
        [Required]
        [Phone]
        public string Mobile { get; set; }
    }
}
