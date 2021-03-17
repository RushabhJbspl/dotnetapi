using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    public class TokenSupplyHistory : BizBase
    {
        [Required]
        public long WalletTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string ContractAddress { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Required]
        public short IsIncrease { get; set; }

        [Required]
        public string Remarks { get; set; }

        public string TrnHash { get; set; }
    }
}
