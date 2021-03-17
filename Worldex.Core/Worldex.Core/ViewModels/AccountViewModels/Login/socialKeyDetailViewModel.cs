using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels.AccountViewModels.Login
{
    public class SocialKeyDetailViewModel
    {
        public string ProviderName { get; set; }
        public string ClientId  { get; set; }
        public string ClientSecret { get; set; }
    }

    public class SocialKeyDetailResponse : BizResponseClass
    {
      public  SocialKeyDetailViewModel socialKeyDetailViewModel { get; set; }
    }
}
