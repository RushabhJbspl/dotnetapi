using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class TradeLedgerResponce : BizResponseClass
    {
        public TradeLedgerInfo response { get; set; }
    }
    public class TradeLedgerInfo
    {
        public string symbol { get; set; }
        public long id { get; set; }
        public long orderId { get; set; }
        public decimal price { get; set; }
        public decimal qty { get; set; }
        public decimal commission { get; set; }
        public string commissionAsset { get; set; }
        public long time { get; set; }
        public bool isBuyer { get; set; }
        public bool isMaker { get; set; }
        public bool isBestMatch { get; set; }
    }
}
