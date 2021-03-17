using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class OpenPositionMaster : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }
        [Key]
        public long PairID { get; set; }
        [Key]
        public Guid BatchNo { get; set; }
        [Key]
        public long UserID { get; set; }

        [Required]
        [StringLength(10)]
        public string BaseCurrency { get; set; }

        public long LoanID { get; set; } 

    }
    public class OpenPositionDetail : BizBase
    {
        [Required]
        public long OpenPositionMasterID { get; set; } //fk of OpenPositionMaster
        public Guid Guid { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Qty { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal BidPrice { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LandingPrice { get; set; }
        [Required]
        [StringLength(10)]
        public string CurrencyName { get; set; }
        [Required]
        public short TrnType { get; set; } //4 =buy , 5= sell
        [Required]
        public long TrnNo { get; set; } // TQ trnno
        [Required]
        public long WTrnNo { get; set; } //WalletTQ TrnNo
        [Required]
        [StringLength(100)]
        public string SystemRemarks { get; set; }
    }

    public class MarginPNLAccount : BizBase
    {
        [Required]
        public long TrnNo { get; set; } // TQ trnno
        [Required]
        public long WTrnNo { get; set; } //WalletTQ TrnNo
        [Required]
        public long OpenPositionMasterID { get; set; } //fk of OpenPositionMaster
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal SettledQty { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal AvgLandingBuy { get; set; } // price of base currency
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal AvgLandingSell { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ProfitAmount { get; set; }
        public long UserID { get; set; }
        [StringLength(10)]
        public string ProfitCurrencyName { get; set; }
        public long ProfitWalletID { get; set; }
        public long LoanID { get; set; }
    }
}
