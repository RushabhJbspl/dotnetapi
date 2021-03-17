using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    public class UserProfitStatistics :BizBase
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public int Day { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public string CurrencyName { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal StartingBalance { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal EndingBalance { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal DepositionAmount { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal WithdrawAmount { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal ProfitAmount { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal ProfitPer { get; set; }
    }

    public class ArbitrageUserProfitStatistics : BizBase
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public int Day { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public string CurrencyName { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal USDStartingBalance { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal StartingBalance { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal EndingBalance { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal USDEndingBalance { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal DepositionAmount { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal USDDepositionAmount { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal WithdrawAmount { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal USDWithdrawAmount { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal ProfitAmount { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal USDProfitAmount { get; set; }

        [Required]
        [Range(0, 9999999999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(35, 18)")]
        public decimal ProfitPer { get; set; }
    }
}
