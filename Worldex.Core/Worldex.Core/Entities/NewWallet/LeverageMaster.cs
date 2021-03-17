using Worldex.Core.Enums;
using Worldex.Core.SharedKernel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.NewWallet
{
    public class LeverageMaster:BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Key]
        [Required]
        public long WalletTypeID { get; set; } //fk of marginwallettype

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LeveragePer { get; set; } 

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal SafetyMarginPer { get; set; }

      
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MarginChargePer { get; set; } //recurring

        [Required]
        public short IsAutoApprove { get; set; } //1-true,0-false 

        [Required]
        [DefaultValue(0)]
        public enLeverageChargeDeductionType LeverageChargeDeductionType { get; set; }//MarginChargePer is monthly or daily 

        //ntrivedi 26-03-2019
        [Required]
        [DefaultValue(0)]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal InstantChargePer { get; set; } // instant leverage take time charge

    }
}
