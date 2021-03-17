using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    public class StakingChargeMaster : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [Key]
        public long WalletTypeID { get; set; }

        [Required]
        [Key]
        public long UserID { get; set; }

        [Required]
        public long StakingPolicyDetailID { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MakerCharge { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerCharge { get; set; }

        [Required]
        public long StakingHistoryID { get; set; }
    }
}
