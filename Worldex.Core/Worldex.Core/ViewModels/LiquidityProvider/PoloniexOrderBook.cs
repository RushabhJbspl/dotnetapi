using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.LiquidityProvider1
{
    public class Rootobject
    {
        public Object Res { get; set; }
    }
    public class PoloniexOrderBook
    {
        public List<List<Object>> asks { get; set; }
        public List<List<Object>> bids { get; set; }
        public string isFrozen { get; set; }
        public int seq { get; set; }
    }
    public class PoloniexTradeHistory
    {
        public long globalTradeID { get; set; }
        public long tradeID { get; set; }
        public DateTime date { get; set; }
        public string type { get; set; }
        public decimal rate { get; set; }
        public decimal amount { get; set; }
        public decimal total { get; set; }
    }
    public class PoloniexChartData
    {
        public int date { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double open { get; set; }
        public double close { get; set; }
        public double volume { get; set; }
        public double quoteVolume { get; set; }
        public double weightedAverage { get; set; }
    }
    public class PoloniexOpenOrder
    {
        public string orderNumber { get; set; }
        public string type { get; set; }
        public string rate { get; set; }
        public string amount { get; set; }
        public string total { get; set; }
    }

    public class PoloniexResultingTrade
    {
        public string amount { get; set; }
        public string date { get; set; }
        public string rate { get; set; }
        public string total { get; set; }
        public string tradeID { get; set; }
        public string type { get; set; }
    }

    public class PoloniexOrderResult
    {
        public int orderNumber { get; set; }
        public IList<PoloniexResultingTrade> resultingTrades { get; set; }
    }

    public class PoloniexOrderState
    {
        public string status { get; set; }
        public decimal rate { get; set; }
        public decimal amount { get; set; }
        public string currencyPair { get; set; }
        public string date { get; set; }
        public decimal total { get; set; }
        public string type { get; set; }
        public decimal startingAmount { get; set; }
    }
    public class PoloniexErrorObj
    {
        public string error { get; set; }
    }
    public class PoloniexCancelOrderObj
    {
        public int success { get; set; }
        public string amount { get; set; }
        public string message { get; set; }
    }
    public class PoloniexCancelOrderErrorObj
    {
        public string error { get; set; }
        public int success { get; set; }
    }
}
