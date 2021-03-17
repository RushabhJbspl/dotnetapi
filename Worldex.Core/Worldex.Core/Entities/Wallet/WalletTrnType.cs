using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Wallet
{
    public class WalletTrnType : BizBase
    {
        [Required]
        [StringLength(50)]
        public string TypeName { get; set; }
    }
}
