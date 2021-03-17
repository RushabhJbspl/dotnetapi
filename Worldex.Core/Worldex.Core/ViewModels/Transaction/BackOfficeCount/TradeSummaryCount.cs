using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction.BackOfficeCount
{
    public class TradeSummaryCount
    {
        //TOTALTRADE LIMIT   MARKET SPOT    STOP_Limit STOP    LIMIT_BUY LIMIT_SELL  MARKET_BUY MARKET_SELL SPOT_BUY SPOT_SELL   STOP_Limit_BUY STOP_Limit_SELL
        //STOP_BUY STOP_SELL
        public long TOTALTRADE { get; set; }

        public long LIMIT { get; set; }
        public long LIMIT_BUY { get; set; }
        public long LIMIT_SELL { get; set; }

        public long MARKET { get; set; }
        public long MARKET_BUY { get; set; }
        public long MARKET_SELL { get; set; }

        public long SPOT { get; set; }
        public long SPOT_BUY { get; set; }
        public long SPOT_SELL { get; set; }

        public long STOP_Limit { get; set; }
        public long STOP_Limit_BUY { get; set; }
        public long STOP_Limit_SELL { get; set; }

        public long STOP { get; set; } 
        public long STOP_BUY { get; set; }
        public long STOP_SELL { get; set; }
    }

    public class TradeSummaryCountResponse : BizResponseClass
    {
        public TradeSummaryCountResponseInfo Response { get; set; }
    }
    public class TradeSummaryCountResponseInfo
    {
        public long TOTALTRADE { get; set; }
        public LimitCls LIMIT { get; set; }
        public MarketCls MARKET { get; set; }
        public SpotCls SPOT { get; set; }
        public Stop_LimitCls STOP_Limit { get; set; }
        public StopCls STOP { get; set; }
    }
    public class LimitCls
    {
        public long TotLIMIT { get; set; }
        public long LIMIT_BUY { get; set; }
        public long LIMIT_SELL { get; set; }
    }
    public class MarketCls
    {
        public long TotMARKET { get; set; }
        public long MARKET_BUY { get; set; }
        public long MARKET_SELL { get; set; }
    }
    public class SpotCls
    {
        public long TotSPOT { get; set; }
        public long SPOT_BUY { get; set; }
        public long SPOT_SELL { get; set; }
    }
    public class Stop_LimitCls
    {
        public long TotSTOP_Limit { get; set; }
        public long STOP_Limit_BUY { get; set; }
        public long STOP_Limit_SELL { get; set; }
    }
    public class StopCls
    {
        public long TotSTOP { get; set; }
        public long STOP_BUY { get; set; }
        public long STOP_SELL { get; set; }
    }
}
