using System;

namespace Worldex.Core.ViewModels.Wallet
{
    public class WalletServiceData
    {
        public Int32 ServiceID { get; set; }
        public string SMSCode { get; set; }
        public int WallletStatus { get; set; }
        public int ServiceStatus { get; set; }
        public int RecordCount { get; set; }
    }
}
