using Worldex.Core.ApiModels;
using Worldex.Core.Enums;

namespace Worldex.Core.ViewModels.Wallet
{
    public class WalletDrCrResponse : BizResponseClass
    {
        public long TrnNo { get; set; }

        public enTransactionStatus Status { get; set; }

        public string StatusMsg { get; set; }

        public string TimeStamp { get; set; }

        public string MinimumAmount { get; set; }

        public string MaximumAmount { get; set; }

        public decimal Charge { get; set; } = 0;

        public string ChargeCurrency { get; set; } = "";
    }
}
