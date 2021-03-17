using Worldex.Core.SharedKernel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    public class StakingPolicyDetail : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        public long StakingPolicyID { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmount { get; set; }

        [Required]
        public short StakingDurationWeek { get; set; }

        [Required]
        public short StakingDurationMonth { get; set; }

        [Required]
        public short InterestType { get; set; } // (1-fixed or 2-percentage)

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal InterestValue { get; set; }

        [Required]
        public long InterestWalletTypeID { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal MakerCharges { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal TakerCharges { get; set; }

        [Required]
        public short EnableAutoUnstaking { get; set; } // if it is enable then user choice is asked in input as priority

        [Required]
        public short EnableStakingBeforeMaturity { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal EnableStakingBeforeMaturityCharge { get; set; }

        [Required]
        [DefaultValue(0)]
        public short RenewUnstakingEnable { get; set; }

        [Required]
        [DefaultValue(0)]
        public short RenewUnstakingPeriod { get; set; }
    }
    public class StakingPolicyMaster : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Key]
        [Required]
        public long WalletTypeID { get; set; }

        [Required] // 1=FD 2=Discount
        [Key]
        public short StakingType { get; set; }

        [Required] //fix or range 
        public short SlabType { get; set; } //       

    }

}
