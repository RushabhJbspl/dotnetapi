using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Transaction
{
    public class TradeBuyerListV1 : BizBase
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
        public decimal DeliveredQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RemainQty { get; set; }

        public short OrderType { get; set; }

        public short IsProcessing { get; set; }
        public short IsAPITrade { get; set; } //Rita 30-1-19 API trading bit set to 1, rest all 0

        public void MakeTransactionInProcess()
        {
            Status = Convert.ToInt16(enTransactionStatus.Pending);
            AddValueChangeEvent();
        }
        public void MakeTransactionSuccess()
        {
            Status = Convert.ToInt16(enTransactionStatus.Success);
            AddValueChangeEvent();
        }
        public void MakeTransactionHold()
        {
            Status = Convert.ToInt16(enTransactionStatus.Hold);
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
            AddValueChangeEvent();
        }
        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<TradeBuyerListV1>(this));
        }

    }

    public class TradeBuyerListMarginV1 : BizBase
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
        public decimal DeliveredQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RemainQty { get; set; }

        public short OrderType { get; set; }

        public short IsProcessing { get; set; }
        public short IsAPITrade { get; set; } //Rita 30-1-19 API trading bit set to 1, rest all 0

        public void MakeTransactionInProcess()
        {
            Status = Convert.ToInt16(enTransactionStatus.Pending);
            AddValueChangeEvent();
        }
        public void MakeTransactionSuccess()
        {
            Status = Convert.ToInt16(enTransactionStatus.Success);
            AddValueChangeEvent();
        }
        public void MakeTransactionHold()
        {
            Status = Convert.ToInt16(enTransactionStatus.Hold);
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
            AddValueChangeEvent();
        }
        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<TradeBuyerListMarginV1>(this));
        }
    }

    public class TradeBuyerListArbitrageV1 : BizBase
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
        public decimal DeliveredQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RemainQty { get; set; }

        public short OrderType { get; set; }

        public short IsProcessing { get; set; }
        public short IsAPITrade { get; set; } //Rita 30-1-19 API trading bit set to 1, rest all 0

        public void MakeTransactionInProcess()
        {
            Status = Convert.ToInt16(enTransactionStatus.Pending);
            AddValueChangeEvent();
        }
        public void MakeTransactionSuccess()
        {
            Status = Convert.ToInt16(enTransactionStatus.Success);
            AddValueChangeEvent();
        }
        public void MakeTransactionHold()
        {
            Status = Convert.ToInt16(enTransactionStatus.Hold);
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
            AddValueChangeEvent();
        }
        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<TradeBuyerListArbitrageV1>(this));
        }

    }
}
