using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Wallet
{
    //ntrivedi table not in use 19-02-2019
    public class WalletOrder : BizBase // Similler to MemberOrder table
    {
        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public enOrderType OrderType { get; set; }

        [Required]
        public long OWalletMasterID { get; set; }

        [Required]
        public long DWalletMasterID { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal OrderAmt { get; set; }

        [Required]
        public new enOrderStatus Status { get; set; }

        [Required]
        [StringLength(100)]
        public string ORemarks { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal DeliveryAmt { get; set; }

        public string DRemarks { get; set; }

        public long? DeliveryGivenBy { get; set; }

        public DateTime? DeliveryGivenDate { get; set; }

        public void SetAsSuccess()
        {
            try
            {
                Status = enOrderStatus.Success;
                Events.Add(new ServiceStatusEvent<WalletOrder>(this));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void SetAsRejected()
        {
            try
            {
                Status = enOrderStatus.Rejected;
                Events.Add(new ServiceStatusEvent<WalletOrder>(this));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
