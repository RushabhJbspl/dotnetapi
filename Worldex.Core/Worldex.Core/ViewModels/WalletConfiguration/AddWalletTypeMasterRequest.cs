using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.WalletConfiguration
{
    public class AddWalletTypeMasterRequest 
    {
        [Required]
        [StringLength(50)]
        public string WalletTypeName { get; set; }

        [Required]
        [StringLength(100)]
        public string Discription { get; set; }

        [Required]
        public short IsDepositionAllow { get; set; }

        [Required]
        public short IsWithdrawalAllow { get; set; }

        [Required]
        public short IsTransactionWallet { get; set; }
    }
}
