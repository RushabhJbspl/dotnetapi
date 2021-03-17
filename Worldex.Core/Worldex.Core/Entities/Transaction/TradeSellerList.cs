using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Transaction
{
    public class TradeSellerListV1 : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }
        [Key]
        public long TrnNo { get; set; }

        public long PairID { get; set; }
        public string PairName { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Price { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Qty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ReleasedQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SelledQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RemainQty { get; set; }

        public short OrderType { get; set; }

        public short IsProcessing { get; set; }
        public short IsAPITrade { get; set; } //Rita 30-1-19 API trading bit set to 1, rest all 0

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
        public void MakeTransactionOperatorFail()
        {
            Status = Convert.ToInt16(enTransactionStatus.OperatorFail);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<TradeSellerListV1>(this));
        }
    }

    public class TradeSellerListMarginV1 : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }
        [Key]
        public long TrnNo { get; set; }

        public long PairID { get; set; }
        public string PairName { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Price { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Qty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ReleasedQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SelledQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RemainQty { get; set; }

        public short OrderType { get; set; }

        public short IsProcessing { get; set; }
        public short IsAPITrade { get; set; } //Rita 30-1-19 API trading bit set to 1, rest all 0
        //Only for margin trading
        public short IsWithoutAmtHold { get; set; } = 0;//Rita 29-3-19 default 0 , 1 for NO amount hold and direct Cr/Dr as settlement time
        public short ISOrderBySystem { get; set; } = 0;//Rita 29-3-19 default 0 , 1 for NO amount hold and direct Cr/Dr as settlement time

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
        public void MakeTransactionOperatorFail()
        {
            Status = Convert.ToInt16(enTransactionStatus.OperatorFail);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<TradeSellerListMarginV1>(this));
        }
    }

    public class TradeSellerListArbitrageV1 : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }
        [Key]
        public long TrnNo { get; set; }

        public long PairID { get; set; }
        public string PairName { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Price { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Qty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ReleasedQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SelledQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RemainQty { get; set; }

        public short OrderType { get; set; }

        public short IsProcessing { get; set; }
        public short IsAPITrade { get; set; } //Rita 30-1-19 API trading bit set to 1, rest all 0

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
        public void MakeTransactionOperatorFail()
        {
            Status = Convert.ToInt16(enTransactionStatus.OperatorFail);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }
        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<TradeSellerListArbitrageV1>(this));
        }
    }
}
