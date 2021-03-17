using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Transaction
{
    public class SettledTradeTransactionQueue : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }
        [Key]
        public long TrnNo { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DefaultValue("dbo.GetISTdate()")]
        public DateTime TrnDate { get; set; }

        [Required]
        public long MemberID { get; set; }

        [Required]
        public short TrnType { get; set; }

        public string TrnTypeName { get; set; }

        [Required]
        public long PairID { get; set; }

        [Required]
        public string PairName { get; set; }

        public long OrderWalletID { get; set; }
        public string OrderAccountID { get; set; }

        public long DeliveryWalletID { get; set; }
        public string DeliveryAccountID { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BuyQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BidPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SellQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal AskPrice { get; set; }

        public string Order_Currency { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal OrderTotalQty { get; set; }

        public string Delivery_Currency { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal DeliveryTotalQty { get; set; }

        public long StatusCode { get; set; }

        public string StatusMsg { get; set; }

        public long? TrnRefNo { get; set; }

        public short IsCancelled { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SettledBuyQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SettledSellQty { get; set; }

        public DateTime? SettledDate { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerPer { get; set; }
    }

    public class SettledTradeTransactionQueueMargin : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }
        [Key]
        public long TrnNo { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DefaultValue("dbo.GetISTdate()")]
        public DateTime TrnDate { get; set; }

        [Required]
        public long MemberID { get; set; }

        [Required]
        public short TrnType { get; set; }

        public string TrnTypeName { get; set; }

        [Required]
        public long PairID { get; set; }

        [Required]
        public string PairName { get; set; }

        public long OrderWalletID { get; set; }
        public string OrderAccountID { get; set; }

        public long DeliveryWalletID { get; set; }
        public string DeliveryAccountID { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BuyQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BidPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SellQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal AskPrice { get; set; }

        public string Order_Currency { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal OrderTotalQty { get; set; }

        public string Delivery_Currency { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal DeliveryTotalQty { get; set; }

        public long StatusCode { get; set; }

        public string StatusMsg { get; set; }

        public long? TrnRefNo { get; set; }

        public short IsCancelled { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SettledBuyQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SettledSellQty { get; set; }

        public DateTime? SettledDate { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerPer { get; set; }
    }

    public class SettledTradeTransactionQueueArbitrage : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }
        [Key]
        public long TrnNo { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DefaultValue("dbo.GetISTdate()")]
        public DateTime TrnDate { get; set; }

        [Required]
        public long MemberID { get; set; }

        [Required]
        public short TrnType { get; set; }

        public string TrnTypeName { get; set; }

        [Required]
        public long PairID { get; set; }

        [Required]
        public string PairName { get; set; }

        public long OrderWalletID { get; set; }
        public string OrderAccountID { get; set; }

        public long DeliveryWalletID { get; set; }
        public string DeliveryAccountID { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BuyQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BidPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SellQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal AskPrice { get; set; }

        public string Order_Currency { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal OrderTotalQty { get; set; }

        public string Delivery_Currency { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal DeliveryTotalQty { get; set; }

        public long StatusCode { get; set; }

        public string StatusMsg { get; set; }

        public long? TrnRefNo { get; set; }

        public short IsCancelled { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SettledBuyQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SettledSellQty { get; set; }

        public DateTime? SettledDate { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TakerPer { get; set; }
    }
}
