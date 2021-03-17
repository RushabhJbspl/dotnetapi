using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    //vsolnkki 24-10-2018
    public class StckingScheme : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaxLimitAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinLimitAmount { get; set; }

        [Required]
        [Key]
        public long WalletType { get; set; }//wallettypemaster fk

        [Required]
        public string TimePeriod { get; set; }

        [Required]
        public long Percent { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public new long Id { get; set; }

        //[Required]
        //[Key]
        //public long MemberTypeId { get; set; }//organization Id=0

        //[Required]
        //[Column(TypeName = "decimal(18, 8)")]
        //public decimal LimitAmount { get; set; }
    }
}
