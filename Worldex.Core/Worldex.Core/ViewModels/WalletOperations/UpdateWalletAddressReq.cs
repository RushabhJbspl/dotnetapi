using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class UpdateWalletAddressReq
    {
        [Required]
        [JsonProperty(PropertyName = "WalletId")]
        public string walletId { get; set; }
        [Required]
        [JsonProperty(PropertyName = "AddressOrId")]
        public string addressOrId { get;set;}
        [JsonProperty(PropertyName = "Label")]
        public string label { get; set; }
    }
}
