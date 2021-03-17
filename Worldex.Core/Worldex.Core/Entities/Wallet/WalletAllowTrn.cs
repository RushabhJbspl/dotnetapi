using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Wallet
{
    public class WalletAllowTrn : BizBase
    {
        [Required]
        public long WalletId { get; set; }

        [Required]
        public byte TrnType { get; set; } // fk of wallettrntype

    }
}
