using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.NewWallet
{
    public class AddRemoveUserWalletRequest : BizBase
    {
        [Required]
        public long ToUserId { get; set; }

        [Required]
        public long WalletOwnerUserID { get; set; }

        [Required]
        public long WalletID { get; set; }

        [Required]
        public long RoleId { get; set; }

        public string Message { get; set; }

        [Required]
        [StringLength(100)]
        public string ReceiverEmail { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? RecieverApproveDate { get; set; }

        [Required]
        public long? RecieverApproveBy { get; set; }

        [Required]
        public long FromUserId { get; set; }

        [Required]
        public short OwnerApprovalStatus { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? OwnerApprovalDate { get; set; }

        public long? OwnerApprovalBy { get; set; }

        [Required]
        public short Type { get; set; }//1-add , 2-remove
    }
}
