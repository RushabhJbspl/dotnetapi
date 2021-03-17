using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.Enums;

namespace Worldex.Core.Entities.NewWallet
{
    public class TradingChargeTypeMaster : BizBase
    {
        [Required]
        public EnTradingChargeType Type { get; set; }

        [Required]
        public string TypeName { get; set; }

        [Required]
        public short IsChargeFreeMarketEnabled { get; set; }//1 enable,0-disable

        [Required]
        public short IsCommonCurrencyDeductEnable { get; set; }// enable,0-disable

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DiscountPercent { get; set; }

        [Required]
        public string DeductCurrency { get; set; }

        [Required]
        public short IsDeductChargeMarketCurrency { get; set; }
    }
}
