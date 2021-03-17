using Worldex.Core.SharedKernel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Wallet
{
    public class BlockUnblockUserAddress : BizBase
    {
        [Required]
        public long UserID { get; set; }

        [Required]
        public long WalletID { get; set; }        

        [Required]
        [StringLength(50)]
        public string Address { get; set; }

        [Required]
        public long WalletTypeID { get; set; }

        public string TrnHash { get; set; }

        [DefaultValue(0)]
        public short IsDestroyed { get; set; }

        [StringLength(150)]
        public string Remarks { get; set; }
    }
}
