using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    //vsolnkki 24-10-2018
    public class UserStacking : BizBase
    {
        [Required]
        public long SchemeId { get; set; }//fk

        [Required]
        public long WalletId { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal StackingAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string WalletType { get; set; }//wallettype

        public string Remarks { get; set; }
    }
}
