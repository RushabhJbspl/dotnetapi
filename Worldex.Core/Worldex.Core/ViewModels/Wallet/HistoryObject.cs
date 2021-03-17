using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Wallet
{
    public class HistoryObject
    {
        public string CoinName { get; set; }//coin
        public string Information { get; set; }//information
        public DateTime Date { get; set; }
        public short Status { get; set; }
        public decimal Amount { get; set; }
        public string Address { get; set; }
        public string StatusStr { get; set; }
        public long Confirmations { get; set; }
        public long TrnNo { get; set; } // ntrivedi 10-12-2018
        public string TrnId { get; set; } // ntrivedi 10-12-2018
        public string ExplorerLink { get; set; }
        public short IsInternalTrn { get; set; }
    }

    public class WithdrawHistoryObject   //Uday 15-01-2019 Add new Parameter create new class becuase old class use in multiple place
    {
        public string CoinName { get; set; }//coin
        public string Information { get; set; }//information
        public DateTime Date { get; set; }
        public short Status { get; set; }
        public decimal Amount { get; set; }
        public string Address { get; set; }
        public string StatusStr { get; set; }
        public long Confirmations { get; set; }
        public long TrnNo { get; set; } // ntrivedi 10-12-2018
        public string TrnId { get; set; } // ntrivedi 10-12-2018
        public string ExplorerLink { get; set; }
        public short IsVerified { get; set; }
        public DateTime EmailSendDate { get; set; }
        public short IsInternalTrn { get; set; }
        public decimal ChargeRs { get; set; }
        public string ChargeCurrency { get; set; }
    }

    public class DepoHistoryObject
    {
        public string CoinName { get; set; }//coin
        public string Information { get; set; }//information
        public DateTime Date { get; set; }
        public short Status { get; set; }
        public decimal Amount { get; set; }
        public string Address { get; set; }
        public string StatusStr { get; set; }
        public long Confirmations { get; set; }
        public long TrnNo { get; set; } // ntrivedi 10-12-2018
        public string TrnId { get; set; } // ntrivedi 10-12-2018
        public List<ExplorerData> ExplorerLink { get; set; }
    }
}
