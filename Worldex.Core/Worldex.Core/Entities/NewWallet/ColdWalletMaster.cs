using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.NewWallet
{
    public class ColdWalletMaster:BizBase
    {
        [Required]
        public string KeyId1 { get; set; }

        [Required]
        public string KeyId2 { get; set; }

        [Required]
        public string KeyId3 { get; set; }

        [Required]
        public string BackUpKey { get; set; }

        [Required]
        public string PublicKey { get; set; }

        public string UserKey { get; set; }
        public short Recoverable { get; set; }//0-false,1-true

        [Required]
        public long WalletId { get; set; }//fk of walletmaster
    }
}
