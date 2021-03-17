using Worldex.Core.ApiModels;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Entities.Wallet;

namespace Worldex.Core.ViewModels.WalletConfiguration
{
    public class WalletTypeMasterResponse:BizResponseClass
    {
        public WalletTypeMaster walletTypeMaster { get; set; }
    }
    public class MarginWalletTypeMasterResponse : BizResponseClass
    {
        public MarginWalletTypeMaster walletTypeMaster { get; set; }
    }
}
