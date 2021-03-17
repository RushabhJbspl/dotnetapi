using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    public class TokenStakingHistory : BizBase
    {
        public string GUID { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal StakingAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal DeductionAmount { get; set; }

        [Required]
        public long StakingPolicyDetailID { get; set; }

        //[Range(0, 9999999999.99999999), DataType(DataType.Currency)]
        //[Column(TypeName = "decimal(18, 8)")]
        //public decimal Charge { get; set; }

        [Required]
        public DateTime MaturityDate { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaturityAmount { get; set; }


        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmount { get; set; }


        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MakerCharges { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerCharges { get; set; }

        [Required]
        public short EnableAutoUnstaking { get; set; } // if it is enable then user choice is asked in input as priority

        [Required]
        public short EnableStakingBeforeMaturity { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal EnableStakingBeforeMaturityCharge { get; set; }

        public long ChannelID { get; set; }

        public long WalletID { get; set; }

        public long UserID { get; set; }

        public long WalletOwnerID { get; set; }

        public long WalletTypeID { get; set; }

        public long InterestWalletTypeID { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal InterestValueMst { get; set; } //master table entry value

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal InterestValue { get; set; } // calculated value amount 

        public short InterestType { get; set; }

        [Required]
        [DefaultValue(0)]
        public short RenewUnstakingEnable { get; set; }

        [Required]
        [DefaultValue(0)]
        public short RenewUnstakingPeriod { get; set; }

        [Required]
        [DefaultValue(0)]
        public short SlabType { get; set; }

        [Required]
        [DefaultValue(0)]
        public short StakingType { get; set; }

        [Required]
        [DefaultValue(0)]
        public long StakingRequestID { get; set; }

        [StringLength(200)]
        public string Remarks { get; set; }
    }
}
