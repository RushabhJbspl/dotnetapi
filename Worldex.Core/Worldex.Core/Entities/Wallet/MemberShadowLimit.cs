using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    //vsolnkki 24-10-2018
    public class MemberShadowLimit : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [Key]
        public long MemberTypeId { get; set; }//organization Id = 0

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ShadowLimitAmount { get; set; }

        [Required]        
        public long WalletType { get; set; }//wallettypemaster fk
    }
}
