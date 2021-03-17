using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Affiliate
{
    public class AffiliateSchemeDetail : BizBase
    {
        public long SchemeMappingId { get; set; }  // Reference From  SchemeTypeMapping

        public int Level { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinimumValue { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaximumValue { get; set; }

        public long CreditWalletTypeId { get; set; } // Reference From  WalletTypeMasters

        public int CommissionType { get; set; } // EnAffiCommissionType

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal CommissionValue { get; set; }

        public int DistributionType { get; set; } // EnAffiDistributedType

        public long TrnWalletTypeId { get; set; }
    }
}
