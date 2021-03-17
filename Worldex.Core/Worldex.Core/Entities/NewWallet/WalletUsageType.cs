using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.NewWallet
{
    public class WalletUsageType :BizBase
    {
        [Key]
        [Required]
        public new long Id { get; set; }

        [Required]
        public string WalletUsageTypeName { get; set; }
    }
}
