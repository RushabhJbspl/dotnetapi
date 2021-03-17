using Newtonsoft.Json;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class AddressMasterResponse
    {
        public string Address { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? IsDefaultAddress { get; set; }
        public string AddressLabel { get; set; }
    }
}
