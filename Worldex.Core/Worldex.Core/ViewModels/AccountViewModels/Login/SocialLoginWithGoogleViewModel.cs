using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.Login
{
    public class SocialLoginWithGoogleViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "2,Please Enter ProviderKey,4092")]
        public string ProviderKey { get; set; }
        [Required(ErrorMessage = "3,Please Enter ProviderName,4093")]
        public string ProviderName { get; set; }
        [Required(ErrorMessage = "4,Please Enter access_token,4094")]
        public string access_token { get; set; }
    }

    public class SocialLoginGoogleResponse : BizResponseClass
    {
        public string Appkey { get; set; }
    }
}
