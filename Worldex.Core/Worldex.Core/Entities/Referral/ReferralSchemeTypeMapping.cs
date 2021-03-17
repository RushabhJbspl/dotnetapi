using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Referral
{
    public class ReferralSchemeTypeMapping : BizBase
    {
        [Required]
        public long PayTypeId { get; set; }

        [Required]
        public long ServiceTypeMstId { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinimumDepositionRequired { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }
    }
}
