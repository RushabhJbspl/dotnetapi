using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Affiliate
{
    public class AffiliateCommissionHistory : BizBase
    {
        public long TrnRefNo { get; set; }

        public long CronRefNo { get; set; }

        public long FromWalletId { get; set; }

        public long ToWalletId { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        public long AffiliateUserId { get; set; }

        public long SchemeMappingId { get; set; }

        public long TrnUserId { get; set; }

        public string Remarks { get; set; }

        public long TrnWalletTypeId { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TransactionAmount { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal CommissionPer { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TrnDate { get; set; }

        public short Level { get; set; }
    }
}
