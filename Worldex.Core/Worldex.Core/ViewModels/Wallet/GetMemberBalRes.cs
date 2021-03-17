using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels.Wallet
{
    public class GetMemberBalRes:BizResponseClass
    {
        public decimal WalletBalance { get; set; }
        public decimal WalletOutboundBalance { get; set; }
        public decimal WalletInboundBalance { get; set; }
        public long WalletID { get; set; }
    }
}
