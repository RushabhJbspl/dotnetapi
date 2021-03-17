using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Transaction.BackOffice
{
    public class PairTradeSummaryViewModel
    {
        public long PairId { get; set; }
        public string PairName { get; set; }
        public Int32 TradeCount { get; set; }
        public Int32 buy { get; set; }
        public Int32 sell { get; set; }
        public Int32 Settled { get; set; }
        public Int32 Cancelled { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public decimal Volume { get; set; }
        public decimal OpenP { get; set; }
        public decimal LTP { get; set; }
        public decimal CloseP { get; set; }
        public decimal ChargePer { get; set; }
        public string OrderType { get; set; }
    }
    public class PairTradeSummaryQryResponse
    {
        public long Id { get; set; }
        public string PairName { get; set; }
        public Int32 TradeCount { get; set; }
        public Int32 buy { get; set; }
        public Int32 sell { get; set; }
        public Int32 Settled { get; set; }
        public Int32 Cancelled { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public decimal Volume { get; set; }
        public decimal OpenP { get; set; }
        public decimal LTP { get; set; }
        public short ordertype { get; set; }
    }
    public class PairTradeSummaryRequest
    {
        public string Pair { get; set; }

        public short Range { get; set; }

        public string MarketType { get; set; }

        public short IsMargin { get; set; }//Rita 5-3-19 for Margin Trading
    }
    public class PairTradeSummaryResponse : BizResponseClass
    {
        public List<PairTradeSummaryViewModel> Response { get; set; }
    }
}
