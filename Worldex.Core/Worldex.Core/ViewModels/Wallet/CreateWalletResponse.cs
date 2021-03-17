using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels.Wallet
{
    public class CreateWalletResponse : BizResponseClass
    {
        public string PublicAddress { get; set; }
        public string AccWalletID { get; set; }
    }
}
