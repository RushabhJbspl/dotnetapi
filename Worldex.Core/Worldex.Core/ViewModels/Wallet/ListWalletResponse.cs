using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Wallet
{
    public class ListWalletResponse : BizResponseClass
    {
         public List<WalletMasterResponse> Wallets { get; set; }
         public short? IsWhitelisting { get; set; }
    }

    public class ListWalletResNew : BizResponseClass
    {
        public List<WalletMasterRes> Wallets { get; set; }
        public short? IsWhitelisting { get; set; }
    }
    //komal 13-09-2019 for getSystem user balance for settledbatch transaction
    public class TransactionWalletResponse : BizResponseClass
    {
        public WalletMasterResponsev2 Wallet { get; set; }
    }
}
