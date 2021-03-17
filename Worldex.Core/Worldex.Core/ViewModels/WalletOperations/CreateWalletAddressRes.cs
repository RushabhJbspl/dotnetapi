using Worldex.Core.ApiModels;
using Newtonsoft.Json;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class CreateWalletAddressRes : BizResponseClass
    {
        [JsonProperty(PropertyName = "Address")]
        public string address { get; set; }
    }
}
