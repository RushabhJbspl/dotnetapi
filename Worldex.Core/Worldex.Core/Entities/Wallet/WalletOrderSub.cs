using System;
using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.Enums;
using Worldex.Core.Events;

namespace Worldex.Core.Entities.Wallet
{
    class WalletOrderSub : BizBase
    {
        [Required]
        public long WalletOrderId { get; set; }

        [Required]
        [StringLength(100)]
        public string OBranchName { get; set; }

        [Required]
        [StringLength(20)]
        public string OAccountNo { get; set; }

        [Required]
        [StringLength(20)]
        public string OChequeNo { get; set; }

        [Required]
        public DateTime? OChequeDate { get; set; }

        public long? RefNo { get; set; }

        public void SetAsSuccess()
        {
            Status = Convert.ToInt16(enOrderStatus.Success);
            Events.Add(new ServiceStatusEvent<WalletOrderSub>(this));
        }
        public void SetAsRejected()
        {
            Status = Convert.ToInt16(enOrderStatus.Rejected);
            Events.Add(new ServiceStatusEvent<WalletOrderSub>(this));
        }
    }
}
