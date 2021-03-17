using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.Login
{
    public class SocialLoginWithfacebookViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter ProviderKey,4092")]
        public string ProviderKey { get; set; }
        [Required(ErrorMessage = "1,Please Enter ProviderName,4093")]
        public string ProviderName { get; set; }
        [Required(ErrorMessage = "1,Please Enter access_token,4094")]
        public string access_token { get; set; }
    }
    public class SocialLoginfacebookResponse : BizResponseClass
    {
        public string Appkey { get; set; }
    }
}
