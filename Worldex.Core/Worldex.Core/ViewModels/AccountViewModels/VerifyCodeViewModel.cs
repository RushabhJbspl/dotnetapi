using Worldex.Core.ApiModels;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels
{
    public class VerifyCodeViewModel
    {
        [Required(ErrorMessage = "1,Please enter a twofactore authentication code,4065")]
        public string Code { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class VerifyCodeResponse : BizResponseClass
    {
        
    }

    public class VerifySuccResponse : BizResponseClass
    {
        public bool Thememode { get; set; }
        public string PreferedLanguage { get; set; }
    }
}
