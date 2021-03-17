using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.IEO
{
    public class IEOCronMaster : BizBase
    {
        [Required]
        public string Guid { get; set; }
        [Required]
        public long IEOPurchaseHistoryId { get; set; }
        [Required]
        public DateTime MaturityDate { get; set; }
        [Required]
        public long RoundId { get; set; }
        [Required]
        public long UserId{ get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal DeliveryQuantity { get; set; }
        [Required]
        public string DeliveryCurrency { get; set; }
        [Required]
        public long CrWalletId { get; set; }
        [Required]
        public long DrWalletId { get; set; }
        [Required]
        public long SlabID { get; set; }
        public string StatusMsg { get; set; }
        public int ErrorCode { get; set; }
        public long EmailBatchNo { get; set; }
    }
}
