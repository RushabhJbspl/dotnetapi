using Worldex.Core.SharedKernel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Charges
{
    public class ChargeConfigurationDetail:BizBase
    {
        [Required]
        public long ChargeConfigurationMasterID { get; set; }

        [Required]
        public short ChargeDistributionBasedOn { get; set; }//2.(1- Regular,2 - Volume ,3 - Day end balance)

        [Required]
        public long ChargeType { get; set; }//(1.Regular,2.Recurring , fk)

        [Required]
        public long DeductionWalletTypeId { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChargeValue { get; set; }

        [Required]
        public short ChargeValueType { get; set; }//(1.Fixed,2.Percentage)

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MakerCharge { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal TakerCharge { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmount { get; set; }

        public string Remarks { get; set; }

        [DefaultValue(0)]
        public short IsCurrencyConverted { get; set; }//0-not convert,1 -Convert

        [DefaultValue(1)]
        public short? DeductChargetType { get; set; }//1-BaseCurrency, 2-Trading Qty
    }

    public class ChargeConfigurationDetailArbitrage : BizBase
    {
        [Required]
        public long ChargeConfigurationMasterID { get; set; }

        [Required]
        public short ChargeDistributionBasedOn { get; set; }//2.(1- Regular,2 - Volume ,3 - Day end balance)

        [Required]
        public long ChargeType { get; set; }//(1.Regular,2.Recurring , fk)

        [Required]
        public long DeductionWalletTypeId { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChargeValue { get; set; }

        [Required]
        public short ChargeValueType { get; set; }//(1.Fixed,2.Percentage)

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MakerCharge { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal TakerCharge { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmount { get; set; }

        public string Remarks { get; set; }

        [DefaultValue(0)]
        public short IsCurrencyConverted { get; set; }//0-not convert,1 -Convert

        [DefaultValue(1)]
        public short? DeductChargetType { get; set; }//1-BaseCurrency, 2-Trading Qty
    }
}
