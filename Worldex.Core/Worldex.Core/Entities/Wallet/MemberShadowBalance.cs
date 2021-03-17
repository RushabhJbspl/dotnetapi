using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    //vsolnkki 24-10-2018
    public class MemberShadowBalance : BizBase
    {
        [Required]
        public long MemberShadowLimitId { get; set; }//fk 

        [Required] // ntrivedi walletid instead of userid 
        public long WalletID { get; set; }//User Id 

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ShadowAmount { get; set; }

        // ntrivedi walletid is used instead of userid 
        [Required]
        public long WalletTypeId { get; set; }//fk

        public string Remarks { get; set; }
    }
}
