using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities
{
    public class TradeTransactionQueue : BizBase
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

        //[Required]
        //public short TrnMode { get; set; }

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

        //public short Status { get; set; }

        public long StatusCode { get; set; }

        public string StatusMsg { get; set; }

        //public long ServiceID { get; set; }

        //public long ProductID { get; set; }

        //public long SerProID { get; set; }

        //public int RouteID { get; set; }//change column as new structure

        //public long? TrnRefNo { get; set; }

        public short IsCancelled { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SettledBuyQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SettledSellQty { get; set; }

        public DateTime? SettledDate { get; set; }

        //[Range(0, 9999999999.99999999), DataType(DataType.Currency)]
        //[Column(TypeName = "decimal(18, 8)")]
        //public decimal TakerPer { get; set; }
        public short ordertype { get; set; } //type of enTransactionMarketType
        public short IsAPITrade { get; set; } //Rita 30-1-19 API trading bit set to 1, rest all 0
        public short IsExpired { get; set; } //Rita 30-1-19 API expired order, always may fail
        public string APIStatus { get; set; } //Rita 30-1-19 API Status record
        public short IsAPICancelled { get; set; } //Rita 30-1-19 API Cancellation initiated

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal APIPrice { get; set; }

        [DefaultValue("0")]
        public short IsPrivateAPITarde { get; set; } //komal 03-10-2019 for private api trade

        public object Clone()//for copy object
        {
            return MemberwiseClone();
        }
        public void MakeTransactionInProcess()
        {
            Status = Convert.ToInt16(enTransactionStatus.Pending);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionSuccess()
        {
            Status = Convert.ToInt16(enTransactionStatus.Success);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            SettledDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionSystemFail()
        {
            Status = Convert.ToInt16(enTransactionStatus.SystemFail);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionOperatorFail()
        {
            Status = Convert.ToInt16(enTransactionStatus.OperatorFail);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionHold()
        {
            Status = Convert.ToInt16(enTransactionStatus.Hold);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionInActive()
        {
            Status = Convert.ToInt16(enTransactionStatus.InActive);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void SetTransactionCode(long statuscode)
        {
            StatusCode = statuscode;
            AddValueChangeEvent();
        }
        public void SetTransactionStatusMsg(string statusMsg)
        {
            StatusMsg = statusMsg;
            AddValueChangeEvent();
        }
        //public void SetServiceProviderData(long iServiceID, long iSerProID, long iProductID, long iRouteID)
        //{
        //    ServiceID = iServiceID;
        //    SerProID = iSerProID;
        //    ProductID = iProductID;
        //    RouteID = iRouteID;
        //    AddValueChangeEvent();
        //}
        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<TradeTransactionQueue>(this));
        }
    }

    public class TradeTransactionQueueMargin : BizBase
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
        public short IsCancelled { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SettledBuyQty { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SettledSellQty { get; set; }
        public DateTime? SettledDate { get; set; }
        public short ordertype { get; set; }
        public short IsAPITrade { get; set; }
        public short IsExpired { get; set; }
        public string APIStatus { get; set; }
        public short IsAPICancelled { get; set; }
        //Only for margin trading
        public short IsWithoutAmtHold { get; set; } = 0;//Rita 29-3-19 default 0 , 1 for NO amount hold and direct Cr/Dr as settlement time
        public short ISOrderBySystem { get; set; } = 0;//Rita 29-3-19 default 0 , 1 for NO amount hold and direct Cr/Dr as settlement time

        [DefaultValue("0")]
        public short IsPrivateAPITarde { get; set; } //komal 03-10-2019 for private api tarde 

        public object Clone()//for copy object
        {
            return MemberwiseClone();
        }
        public void MakeTransactionInProcess()
        {
            Status = Convert.ToInt16(enTransactionStatus.Pending);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionSuccess()
        {
            Status = Convert.ToInt16(enTransactionStatus.Success);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            SettledDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionSystemFail()
        {
            Status = Convert.ToInt16(enTransactionStatus.SystemFail);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionOperatorFail()
        {
            Status = Convert.ToInt16(enTransactionStatus.OperatorFail);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionHold()
        {
            Status = Convert.ToInt16(enTransactionStatus.Hold);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionInActive()
        {
            Status = Convert.ToInt16(enTransactionStatus.InActive);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void SetTransactionCode(long statuscode)
        {
            StatusCode = statuscode;
            AddValueChangeEvent();
        }
        public void SetTransactionStatusMsg(string statusMsg)
        {
            StatusMsg = statusMsg;
            AddValueChangeEvent();
        }
        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<TradeTransactionQueueMargin>(this));
        }
    }


    public class TradeTransactionQueueArbitrage : BizBase
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
        public short IsCancelled { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SettledBuyQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SettledSellQty { get; set; }

        public DateTime? SettledDate { get; set; }

        public short ordertype { get; set; } //type of enTransactionMarketType
        public short IsAPITrade { get; set; } //Rita 30-1-19 API trading bit set to 1, rest all 0
        public short IsExpired { get; set; } //Rita 30-1-19 API expired order, always may fail
        public string APIStatus { get; set; } //Rita 30-1-19 API Status record
        public short IsAPICancelled { get; set; } //Rita 30-1-19 API Cancellation initiated

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal APIPrice { get; set; }

        public object Clone()//for copy object
        {
            return MemberwiseClone();
        }
        public void MakeTransactionInProcess()
        {
            Status = Convert.ToInt16(enTransactionStatus.Pending);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionSuccess()
        {
            Status = Convert.ToInt16(enTransactionStatus.Success);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            SettledDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionSystemFail()
        {
            Status = Convert.ToInt16(enTransactionStatus.SystemFail);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionOperatorFail()
        {
            Status = Convert.ToInt16(enTransactionStatus.OperatorFail);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionHold()
        {
            Status = Convert.ToInt16(enTransactionStatus.Hold);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void MakeTransactionInActive()
        {
            Status = Convert.ToInt16(enTransactionStatus.InActive);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void SetTransactionCode(long statuscode)
        {
            StatusCode = statuscode;
            AddValueChangeEvent();
        }
        public void SetTransactionStatusMsg(string statusMsg)
        {
            StatusMsg = statusMsg;
            AddValueChangeEvent();
        }
        public void AddValueChangeEvent()  
        {
            Events.Add(new ServiceStatusEvent<TradeTransactionQueueArbitrage>(this));
        }
    }

}
