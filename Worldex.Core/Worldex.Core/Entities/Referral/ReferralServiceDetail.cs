using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Referral
{
    public class ReferralServiceDetail : BizBase
    {
        [Required]
        public long SchemeTypeMappingId { get; set; }

        public long MaximumLevel { get; set; }

        public long MaximumCoin { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinimumValue { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaximumValue { get; set; }

        [Required]
        public long CreditWalletTypeId { get; set; }

        [Required]
        public int CommissionType { get; set; } // EnAffiCommissionType

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal CommissionValue { get; set; }
    }
}
