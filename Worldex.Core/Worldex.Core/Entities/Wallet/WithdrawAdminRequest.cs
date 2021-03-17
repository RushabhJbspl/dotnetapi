using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Wallet
{
    public class WithdrawAdminRequest : BizBase
    {
        [Required]
        public long TrnNo { get; set; }
        public long ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        [StringLength(150)]
        public string Remarks { get; set; }
    }

}
