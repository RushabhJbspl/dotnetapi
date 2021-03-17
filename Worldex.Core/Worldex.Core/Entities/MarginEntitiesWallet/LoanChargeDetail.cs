using Worldex.Core.Enums;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class LoanChargeDetail : BizBase
    {
        public long BatchNo { get; set; }
        public long LoanID { get; set; }
        public long UserID { get; set; }
        public long SafetyWalletID { get; set; }
        public long MarginWalletID { get; set; }       

        public DateTime TrnDate { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LoanAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Leverage { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LeverageMax { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal SafetyPreBal { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MarginPreBal { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DailyChargePer { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DailyChargeValue { get; set; }

        public MarginChargeCase MarginChargeCase { get; set; }

        public int ErrorCode { get; set; }
        [StringLength(250)]
        public string ErrorMsg { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LoanProfit { get; set; } //ntrivedi 17-04-2018

    }
}
