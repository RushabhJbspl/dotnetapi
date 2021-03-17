using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Transaction
{
    public class PoolOrder : BizBase
    {
        [Required]
        public long OMemberID { get; set; }
        [Required]
        public long DMemberID { get; set; }
        public long TrnNo { get; set; }
        public long PoolID { get; set; }       
        [Required]
        public long UserID { get; set; }       
        [Required]
        public short PayMode { get; set; }
        [Required]
        public short TrnMode { get; set; }
        public string ORemarks { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal OrderAmt { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal DiscPer { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal DiscRs { get; set; }
       
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal DeliveryAmt { get; set; }
        public string DRemarks { get; set; }
        public long UserWalletID { get; set; }
        public string UserWalletAccID { get; set; }
        public long CancelID { get; set; }
        //public short OBankID { get; set; }
        //public string OBranchName { get; set; }
        //public string OAccountNo { get; set; }
        //public string OChequeNo { get; set; }
        //public DateTime OChequeDate { get; set; }       
        //[Required]
        //public short DBankID { get; set; }
        //[Required]
        //public string DAccountNo { get; set; }
        //public long DeliveryGivenBy { get; set; }        
        //public DateTime DeliveryGivenDate { get; set; }
        //[Required]
        //public byte AlertRec { get; set; }
        //[Required]
        //public byte AlertRec { get; set; }       
        //[Required]
        //public double CashChargePer { get; set; }
        //[Required]
        //[Range(0, 9999999999.99999999), DataType(DataType.Currency)]
        //[Column(TypeName = "decimal(18, 8)")]
        //public decimal CashChargeRs { get; set; }
        //[Required]
        //[Range(0, 9999999999.99999999), DataType(DataType.Currency)]
        //[Column(TypeName = "decimal(18, 8)")]
        //public decimal WalletAmt { get; set; }
        //public int PGId { get; set; }
        ////public long CouponNo { get; set; }
        //public bool IsChargeAccepted { get; set; }
        //public bool IsDebited { get; set; }
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
        public void MakeTransactionOperatorFail()
        {
            Status = Convert.ToInt16(enTransactionStatus.OperatorFail);
            AddValueChangeEvent();
        }
        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<PoolOrder>(this));
        }
    }
}
