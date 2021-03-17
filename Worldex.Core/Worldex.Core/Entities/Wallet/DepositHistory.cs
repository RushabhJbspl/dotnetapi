using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Wallet
{
    public class DepositHistory : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        public string GUID { get; set; }

        [Key]
        [StringLength(100)]
        public string TrnID { get; set; }

        [Required]
        public string SMSCode { get; set; }

        [Key]
        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        public long Confirmations { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string StatusMsg { get; set; }

        [Required]
        public string TimeEpoch { get; set; }

        [Required]
        public string ConfirmedTime { get; set; }


        public string EpochTimePure { get; set; } // time converted from epoch time 

        public long OrderID { get; set; }

        [DefaultValue(0)]
        public byte IsProcessing { get; set; }

        [Required]
        [StringLength(200)]
        public string FromAddress { get; set; }

        public string APITopUpRefNo { get; set; }

        public string SystemRemarks { get; set; }

        public string RouteTag { get; set; }

        public long SerProID { get; set; }

        //vsolanki 2018-10-16
        public long UserId { get; set; }


        public short? IsInternalTrn { get; set; }       //Uday 22-01-2019 Check for internal transaction

        [DefaultValue(0)]
        public long WalletId { get; set; }

        public int IsFlushAddProcess { get; set; }
        public long IsConfirmed { get; set; }

        public string FlushTrnHash { get; set; }

        public short IsDescending { get; set; }

        public void OrderIdUpdated(long orderid)
        {
            try
            {
                OrderID = orderid;
                Events.Add(new ServiceStatusEvent<DepositHistory>(this));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void SetAsSuccess(string msg)
        {
            try
            {
                StatusMsg = msg;
                Status = Convert.ToInt16(enOrderStatus.Success);
                Events.Add(new ServiceStatusEvent<DepositHistory>(this));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void SetAsRejected(string msg)
        {
            try
            {
                StatusMsg = msg;
                Status = Convert.ToInt16(enOrderStatus.Rejected);
                Events.Add(new ServiceStatusEvent<DepositHistory>(this));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ConfirmationUpdated(long Confirmation)
        {
            try
            {
                Confirmations = Confirmation;
                Events.Add(new ServiceStatusEvent<DepositHistory>(this));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}
