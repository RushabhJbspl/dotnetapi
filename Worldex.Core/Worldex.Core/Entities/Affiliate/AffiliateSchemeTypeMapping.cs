using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Affiliate
{
    public class AffiliateSchemeTypeMapping : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [Key]
        public long SchemeMstId { get; set; }  // Reference From  AffiliateSchemeMaster

        [Required]
        [Key]
        public long SchemeTypeMstId { get; set; }  // Reference From  AffiliateSchemeTypeMaster

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinimumDepositionRequired { get; set; }

        public long DepositWalletTypeId { get; set; }  // Reference From  WalletTypeMasters

        public int CommissionTypeInterval { get; set; }  // EnAffiCommisionTypeInterval

        public string Description { get; set; }

        public long CommissionHour { get; set; } // Total Commission Hour 

        
    }
}
