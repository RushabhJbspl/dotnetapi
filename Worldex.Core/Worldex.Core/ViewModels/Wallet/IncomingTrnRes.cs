using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Wallet
{
    public class ListIncomingTrnRes
    {
        public List<IncomingTrnRes> IncomingTransactions { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
    public class IncomingTrnRes
    {
        public  long AutoNo { get; set; }

        public string TrnID { get; set; }

        public string WalletType { get; set; }

        public string Address { get; set; }
        
        public long Confirmations { get; set; }

        public decimal Amount { get; set; }

        public short? ConfirmationCount { get; set; }

        public DateTime Date { get; set; }
        public long TrnNo { get; set; }
        // public List<ExplorerData> ExplorerLink { get; set; }
        public string ExplorerLink { get; set; }
    }

    public class ListIncomingTrnResv2
    {
        public List<IncomingTrnResv2> IncomingTransactions { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
    public class IncomingTrnResv2
    {
        public long AutoNo { get; set; }

        public string TrnID { get; set; }

        public string WalletType { get; set; }

        public string Address { get; set; }

        public long Confirmations { get; set; }

        public decimal Amount { get; set; }

        public short? ConfirmationCount { get; set; }

        public DateTime Date { get; set; }

        public string TrnNo { get; set; }

        public string ExplorerLink { get; set; }
    }
}
