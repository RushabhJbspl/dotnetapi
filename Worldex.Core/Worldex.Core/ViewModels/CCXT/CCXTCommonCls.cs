using Worldex.Core.ApiModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.CCXT
{
    //=========================================Ticker cls
    public class CCXTTickerHandlerRequest : IRequest
    {
    }
    public class CCXTTickerExchange : IRequest
    {
        public string Pair { get; set; }
        public long PairID { get; set; }
        public long LpType { get; set; }
        public string ExchangeName { get; set; }
        public long RouteID { get; set; }
        public long ThirdPartyAPIID { get; set; }
        public long SerProDetailID { get; set; }
    }
    public class CCXTTickerResponse
    {
        public string symbol { get; set; }
        public string timestamp { get; set; }
        public string datetime { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal high { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal low { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal bid { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal ask { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal vwap { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal close { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal last { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal baseVolume { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal quoteVolume { get; set; }
        public CCXTinfo info { get; set; }

    }
    public class CCXTinfo
    {
        public string symbol { get; set; }
        public string priceChange { get; set; }
        public string priceChangePercent { get; set; }
        public string weightedAvgPrice { get; set; }
        public string prevClosePrice { get; set; }
        public string lastPrice { get; set; }
        public string lastQty { get; set; }
        public string bidPrice { get; set; }
        public string bidQty { get; set; }
        public string askPrice { get; set; }
        public string askQty { get; set; }
        public string openPrice { get; set; }
        public string highPrice { get; set; }
        public string lowPrice { get; set; }
        public string volume { get; set; }
        public string quoteVolume { get; set; }
        public long openTime { get; set; }
        public long closeTime { get; set; }
        public int firstId { get; set; }
        public int lastId { get; set; }
        public int count { get; set; }
    }

    public class CCXTTickerResObj
    {
        [Range(0, 9999999999.999999999999999999)]
        public decimal LTP { get; set; }
        public string Pair { get; set; }
        public long LPType { get; set; }
        public long PairId { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal Volume { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal Fees { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal ChangePer { get; set; }
        public short UpDownBit { get; set; }
    }
    public class CCXTTickerQryObj
    {
        [Range(0, 9999999999.999999999999999999)]
        public decimal LTP { get; set; }
        public string Pair { get; set; }
        public short LPType { get; set; }
        public long PairId { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal Volume { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal Fees { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal ChangePer { get; set; }
        public short UpDownBit { get; set; }
    }

    //==========================================================================
    public class CCXTStatusCheckHandlerRequest : IRequest
    {
    }
    public class CCXTStatusCheckCallReq : IRequest
    {
        public string Pair { get; set; }
        public long PairID { get; set; }
        public short LpType { get; set; }
        public long SerProDetailID { get; set; }
        public long RouteID { get; set; }
        public string ExchangeName { get; set; }
        public long ThirdPartyAPIID { get; set; }
    }
    //Darshan Dholakiya added this class for LpTypeHold Transaction changes----------------------
    public class CCXTStatusCheckCallLpReq : IRequest
    {
        public long LpType { get; set; }
        public List<TransactionProviderArbitrageResponse> transactionProviderArbitrageResponses { get; set; }    
    }

    public class CCXTOrdersInfoResponseObj
    {
        public string symbol { get; set; }
        public int orderId { get; set; }
        public string clientOrderId { get; set; }
        public string price { get; set; }
        public string origQty { get; set; }
        public string executedQty { get; set; }
        public string cummulativeQuoteQty { get; set; }
        public string status { get; set; }
        public string timeInForce { get; set; }
        public string type { get; set; }
        public string side { get; set; }
        public string stopPrice { get; set; }
        public string icebergQty { get; set; }
        public object time { get; set; }
        public object updateTime { get; set; }
        public bool isWorking { get; set; }
    }

    public class CCXTOrdersResponseObj
    {
        public string id { get; set; }
        public object timestamp { get; set; }
        public DateTime datetime { get; set; }
        public string symbol { get; set; }
        public string type { get; set; }
        public string side { get; set; }
        public double price { get; set; }
        public double amount { get; set; }
        public double cost { get; set; }
        public string filled { get; set; }
        public string  remaining { get; set; }
        public string status { get; set; }
        public CCXTOrdersInfoResponseObj info { get; set; }
    }

    //==========================trade history response obj 

    public class CCXTMyTradeInfoFee
    {
        public double cost { get; set; }
        public string currency { get; set; }
    }

    public class CCXTMyTradeInfo2
    {
        public string id { get; set; }
        public string marketSymbol { get; set; }
        public string direction { get; set; }
        public string type { get; set; }
        public string quantity { get; set; }
        public string limit { get; set; }
        public string timeInForce { get; set; }
        public string fillQuantity { get; set; }
        public string commission { get; set; }
        public string proceeds { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime closedAt { get; set; }
    }

    public class CCXTMyTradeInfo
    {
        public string id { get; set; }
        public string order_id { get; set; }
        public long timestamp { get; set; }
        public DateTime datetime { get; set; }
        public string lastTradeTimestamp { get; set; }
        public string symbol { get; set; }
        public string type { get; set; }
        public string side { get; set; }
        public string price { get; set; }
        public string cost { get; set; }
        public string average { get; set; }
        public string amount { get; set; }
        public string filled { get; set; }
        public string remaining { get; set; }
        public string status { get; set; }
        public CCXTMyTradeInfoFee fee { get; set; }
        public CCXTMyTradeInfo2 info { get; set; }
    }

    public class CCXTMyTradeInfoResponse
    {
        public string id { get; set; }
        public CCXTMyTradeInfo info { get; set; }
        public long timestamp { get; set; }
        public DateTime datetime { get; set; }
        public string symbol { get; set; }
        public string side { get; set; }
        public string price { get; set; }
        public string amount { get; set; }
    }
    //============================CCXT Balance check class

    public class CCXTBalanceObj
    {
        public string currency { get; set; }
        public double free { get; set; }
        public double used { get; set; }
        public double total { get; set; }
    }
}
