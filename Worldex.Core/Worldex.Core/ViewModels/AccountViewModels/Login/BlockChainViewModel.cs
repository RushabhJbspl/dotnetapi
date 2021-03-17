using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.Login
{
    public class BlockChainViewModel : TrackerViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string KeyPassword { get; set; }
    }
}
