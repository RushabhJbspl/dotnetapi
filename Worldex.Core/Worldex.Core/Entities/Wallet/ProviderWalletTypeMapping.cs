using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Wallet
{
    public class ProviderWalletTypeMapping : BizBase
    {
        [Required]
        public long WalletTypeId { get; set; }

        [Required]
        public long ServiceProviderId { get; set; }
    }
}
