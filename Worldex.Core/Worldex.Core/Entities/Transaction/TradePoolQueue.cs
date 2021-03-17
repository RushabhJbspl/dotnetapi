using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Transaction
{
    public class TradePoolQueueV1 : BizBase
    {
        [Required]
        public long PairID { get; set; }       

        public long MakerTrnNo { get; set; }
        public string MakerType { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MakerPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MakerQty { get; set; }       

        public long TakerTrnNo { get; set; }
        public string TakerType { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerDisc { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerLoss { get; set; }

        public short IsAPITrade { get; set; } = 0;

        public long StatusCode { get; set; }//Rita 23-3-19 added for check as Txn side amount debited
        public string StatusMsg { get; set; }
    }

    public class TradePoolQueueMarginV1 : BizBase
    {
        [Required]
        public long PairID { get; set; }

        public long MakerTrnNo { get; set; }
        public string MakerType { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MakerPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MakerQty { get; set; }

        public long TakerTrnNo { get; set; }
        public string TakerType { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerDisc { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerLoss { get; set; }

        public short IsAPITrade { get; set; } = 0;

        public long StatusCode { get; set; }//Rita 23-3-19 added for check as Txn side amount debited
        public string StatusMsg { get; set; }
    }

    public class TradePoolQueueArbitrageV1 : BizBase
    {
        [Required]
        public long PairID { get; set; }

        public long MakerTrnNo { get; set; }
        public string MakerType { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MakerPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MakerQty { get; set; }

        public long TakerTrnNo { get; set; }
        public string TakerType { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerDisc { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerLoss { get; set; }

        public short IsAPITrade { get; set; } = 0;

        public long StatusCode { get; set; }//Rita 23-3-19 added for check as Txn side amount debited
        public string StatusMsg { get; set; }
    }
}
