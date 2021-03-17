using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Transaction
{
    public class TradeStopLoss : BizBase
    {      
        public long TrnNo { get; set; }

        public short ordertype { get; set; } //type of enTransactionMarketType

        //for Maket type Stop-Limit
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal StopPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal LTP { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RangeMin { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RangeMax { get; set; }

        public short MarketIndicator { get; set; }//0-Low  1-High

        [Required]
        public long PairID { get; set; }

        public short ISFollowersReq { get; set; } = 0;//Rita 21-1-19 main req always 0
        public long FollowingTo { get; set; } = 0;//Rita 21-1-19 main req always 0
        public long LeaderTrnNo { get; set; } = 0;//Rita 21-1-19 main req always 0
        public string FollowTradeType { get; set; } = "";//Rita 22-1-19 main req always 0
    }

    public class TradeStopLossMargin : BizBase
    {
        public long TrnNo { get; set; }

        public short ordertype { get; set; } //type of enTransactionMarketType

        //for Maket type Stop-Limit
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal StopPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal LTP { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RangeMin { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RangeMax { get; set; }

        public short MarketIndicator { get; set; }//0-Low  1-High

        [Required]
        public long PairID { get; set; }

        public short ISFollowersReq { get; set; } = 0;//Rita 21-1-19 main req always 0
        public long FollowingTo { get; set; } = 0;//Rita 21-1-19 main req always 0
        public long LeaderTrnNo { get; set; } = 0;//Rita 21-1-19 main req always 0
        public string FollowTradeType { get; set; } = "";//Rita 22-1-19 main req always 0
    }


    public class TradeStopLossArbitrage : BizBase
    {
        public long TrnNo { get; set; }

        public short ordertype { get; set; } //type of enTransactionMarketType

        //for Maket type Stop-Limit
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal StopPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal LTP { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RangeMin { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RangeMax { get; set; }

        public short MarketIndicator { get; set; }//0-Low  1-High

        [Required]
        public long PairID { get; set; }

        public short ISFollowersReq { get; set; } = 0;//Rita 21-1-19 main req always 0
        public long FollowingTo { get; set; } = 0;//Rita 21-1-19 main req always 0
        public long LeaderTrnNo { get; set; } = 0;//Rita 21-1-19 main req always 0
        public string FollowTradeType { get; set; } = "";//Rita 22-1-19 main req always 0
    }
}
