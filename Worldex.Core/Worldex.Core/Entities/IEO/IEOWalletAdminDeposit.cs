using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.IEO
{
    public class IEOWalletAdminDeposit : BizBase
    {
        [Required]
        public string GUID { get; set; }

        [Required]
        public long WalletId { get; set; }

        [Required]
        [StringLength(7)]
        public string CurrencyName { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        [StringLength(500)]
        public string Remarks { get; set; }

        [Required]
        [StringLength(500)]
        public string SystemRemarks { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        public long? ApprovedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ApprovedDate { get; set; }
    }
}
