using Binance.Net.Objects;
using Bittrex.Net.Objects;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.LiquidityProvider1;
using CoinbasePro.Services.Products.Models;
using Huobi.Net.Objects;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.Interfaces.LiquidityProvider;
using ExchangeSharp;

namespace Worldex.Core.ViewModels.LiquidityProvider
{
    public class LPConverPair
    {
        public string Pair { get; set; }
    }

    public class LPConverPairV1
    {
        public string Pair { get; set; }
        public string LocalPair { get; set; }
    }

    public class ConfigureLP
    {
        public string Pair { get; set; }
        public string OpCode { get; set; }        
        public short LPType { get; set; }
    }

    public class ConfigureLPArbitrage
    {
        public string Pair { get; set; }
        public string ProviderName { get; set; }
        public short LPType { get; set; }
        public long PairID { get; set; }
        public string ProviderPair { get; set; }
    }
    public class BinanceBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<BinanceOrderBookEntry> Bids { get; set; }
        public List<BinanceOrderBookEntry> Asks { get; set; }
    }
    // add huobi class
    public class HuobiBuySellBook : IRequest            

    {
        public string Symbol { get; set; }
        public List<HuobiOrderBookEntry> Bids { get; set; }
        public List<HuobiOrderBookEntry> Asks { get; set; }
    }

    public class KrakenTradeHistory : IRequest
    {
        public KrakenTradeHistoryResult History { get; set; }
        public string Symbol { get; set; }
    }
    public class GeminiTradeHistory : IRequest
    {
        public List<GeminiTradeHistoryRes> History { get; set; }
        public string Symbol { get; set; }
    }

    public class ExmoTradeHistory : IRequest
    {
        public List<EXMOMarketData> History { get; set; }
        public string Symbol { get; set; }
    }

    public class BittrexBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<BittrexOrderBookEntry> Bids { get; set; }
        public List<BittrexOrderBookEntry> Asks { get; set; }
    }

    public class TradesatoshiBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<TradeSatoshiOrderBookBuySell> Bids { get; set; }
        public List<TradeSatoshiOrderBookBuySell> Asks { get; set; }
    }

    public class BitfinexTradeHistoryRes : IRequest
    {
        public List<BitfinexTradeHistory> History { get; set; }
        public string Symbol { get; set; }
    }
    public class CEXIOTradeHistoryRes:IRequest
    {
        public List<Interfaces.LiquidityProvider.CEXIOTradeHistory> History { get; set; }
        public string Symbol { get; set; }
    }

    public class YobitTradeHistoryResult : IRequest
    {
        public List<ExchangeTrade> Hisory { get; set; }
        public string Symbol { get; set; }
    }

    #region
    /// <summary>
    ///Add new method for OKEx Trade History Implementation by Pushpraj as on 17-06-19
    /// </summary>
    public class OKExTradeHistory : IRequest
    {
        public List<Interfaces.LiquidityProvider.OKExGetFilledInformationReturn> History { get; set; }
        public string Symbol { get; set; }
    }
    #endregion

    /// <summary>
    /// New Pair Add for Integrate New API OKEx by Pushpraj as on 11-06-2019  Task Assign by : Khushalil Medam
    /// </summary>

    public class OKExBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<OKExOrderBookBuySell> Bids { get; set; }
        public List<OKExOrderBookBuySell> Asks { get; set; }
    }

    /// <summary>
    /// New Pair Add for Integrate New API OKEx by Pushpraj as on 11-06-2019  Task Assign by : Khushalil Medam
    /// </summary>

    public class CoinbaseBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<CoinbasePro.Services.Products.Models.Bid> Bids { get; set; }
        public List<CoinbasePro.Services.Products.Models.Ask> Asks { get; set; }
    }

    public class UpbitBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<OrderbookUnit> Bids { get; set; }
        public List<OrderbookUnit> Asks { get; set; }
    }

    public class GeminiBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<GeminiOrderbookUnit> Bids { get; set; }
        public List<GeminiOrderbookUnit> Asks { get; set; }
    }

    public class ExmoBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<GeminiOrderbookUnit> Bids { get; set; }
        public List<GeminiOrderbookUnit> Asks { get; set; }
    }

    public class GeminiOrderbookUnit
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ask_price { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal bid_price { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ask_size { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal bid_size { get; set; }
    }

    public class KrakenBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<OrderbookUnit> Bids { get; set; }
        public List<OrderbookUnit> Asks { get; set; }
    }

    public class BitfinexBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<OrderbookUnit> Bids { get; set; }
        public List<OrderbookUnit> Asks { get; set; }
    }

    public class YobitBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<YobitOrderBookUnit> Bids { get; set; }
        public List<YobitOrderBookUnit> Asks { get; set; }
    }

    public class YobitOrderBookUnit
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal amount { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal price { get; set; }
    }

    public class PoloniexBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<List<Object>> Bids { get; set; }
        public List<List<Object>> Asks { get; set; }
    }

    public class CEXIOBuySellBook : IRequest
    {
        public string Symbol { get; set; }
        public List<OrderbookUnit> Bids { get; set; }
        public List<OrderbookUnit> Asks { get; set; }

    }

    public class BinanceTradeHistory : IRequest
    {
        public List<BinanceAggregatedTrades> History { get; set; }  
        public string Symbol { get; set; }
    }
    // add class
    public class HuobiTradeHistory : IRequest   
    {
        public List<HuobiMarketTrade> History { get; set; }
        public string Symbol { get; set; }
    }
        public class BittrexTradeHistory : IRequest
    {
        public List<BittrexMarketHistory> History { get; set; }
        public string Symbol { get; set; }
    }

    public class TradesatoshiTradeHistory : IRequest
    {
        public List<Interfaces.LiquidityProvider.GetMarketHistoryReturn.MarketHistoryResult> History { get; set; }
        public string Symbol { get; set; }
    }
    public class UpbitTradesHistory : IRequest  
    {
        public List<UpbitTrandeHistory> History { get; set; }
        public string Symbol { get; set; }
    }

    public class CoinbaseTradeHistory : IRequest
    {
        public List<ProductTrade> History { get; set; }
        public string Symbol { get; set; }
    }

    public class PoloniexTradeHistoryV1 : IRequest
    {
        public List<PoloniexTradeHistory> History { get; set; }
        public string Symbol { get; set; }
    }

    public class CommonOrderBookRequest : IRequest
    {
        //public string[] Symbol { get; set; }
        public enAppType LpType { get; set; }
    }

    public class CryptoWatcherReq : IRequest
    {

    }
    public class CryptoWatcherArbitrageReq : IRequest //komal 10-06-2019 for Arbitrage 
    {

    }

    public class RealTimeLtpChecker : IRequest<RealTimeLtpChecker>
    {
        public string Pair { get; set; }
        public List<LTPcls> List { get; set; }
    }

    public class LTPcls : IRequest<LTPcls>
    {
        public string Pair { get; set; }
        public short LpType { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Price { get; set; }
    }

    public class ArbitrageLTPCls : IRequest<ArbitrageLTPCls>
    {
        public string Pair { get; set; }
        public short LpType { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Price { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Volume { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChangePer { get; set; }
        public long PairID { get; set; }
        public short UpDownBit { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Fees { get; set; }
        public string ProviderPair { get; set; }
    }

    public class GetLTPDataLPwise
    {
        public string LpType { get; set; }
        public string Pair { get; set; }
    }


    public class Result1
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal price { get; set; }
    }

    public class Allowance
    {
        public int cost { get; set; }
        public long remaining { get; set; }
    }

    public class CryptoWatcherAPIResponse
    {
        public string error { get; set; } = string.Empty;
        public Result1 result { get; set; }
        public Allowance allowance { get; set; }
    }

    public class CommonWatcherResult
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal bid { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ask { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal last { get; set; }
        public string market { get; set; }
    }

    public class CommonWatcherAPIResponse
    {
        public bool success { get; set; }
        public object message { get; set; }
        public CommonWatcherResult result { get; set; }
    }


    public class BinanceWatcherAPIResponse
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal price { get; set; }
    }

    public class poloniexWatcherAPIResponse
    {
        public int id { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal last { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal lowestAsk { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal highestBid { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal percentChange { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal baseVolume { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal quoteVolume { get; set; }
        public string isFrozen { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal high24hr { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal low24hr { get; set; }
    }


    public class UpbitWatcherAPIResponse
    {
        public string market { get; set; }
        public string trade_date_utc { get; set; }
        public string trade_time_utc { get; set; }
        public long timestamp { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal trade_price { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal trade_volume { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal prev_closing_price { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal change_price { get; set; }
        public string ask_bid { get; set; }
        public long sequential_id { get; set; }
    }


    public class HuboiTick
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal amount { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal open { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal close { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal high { get; set; }
        public long id { get; set; }
        public int count { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal low { get; set; }
        public long version { get; set; }
        public IList<decimal> ask { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal vol { get; set; }
        public IList<decimal> bid { get; set; }
    }

    public class HuboiTickResult
    {
        public string status { get; set; }
        public string ch { get; set; }
        public long ts { get; set; }
        public HuboiTick tick { get; set; }
    }

   
    #region Tradesatoshi

    public class TradeSatoshiOrderBookBuySell
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal quantity { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal rate { get; set; }
    }
    public class TradeSatoshiOrderBookResult
    {
        public List<TradeSatoshiOrderBookBuySell> buy { get; set; }
        public List<TradeSatoshiOrderBookBuySell> sell { get; set; }
    }

    public class TradeSatoshiOrderBook
    {
        public bool success { get; set; }
        public object message { get; set; }
        public TradeSatoshiOrderBookResult result { get; set; }
    }

    public class TradeSatoshiResponse
    {
        public TradeSatoshiOrderBook Result { get; set; }
        public int Id { get; set; }
        public object Exception { get; set; }
        public int Status { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCompletedSuccessfully { get; set; }
        public int CreationOptions { get; set; }
        public object AsyncState { get; set; }
        public bool IsFaulted { get; set; }
    }

    public class LPBalanceCheck : IRequest<LPBalanceCheck>
    {
        public long SerProID { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public long RefNo { get; set; }
        public decimal SystemBal { get; set; }
        public string Type { get; set; }
    }

    public class LPBalanceCheckArbitrage : IRequest<LPBalanceCheckArbitrage>
    {
        public long SerProID { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Balance { get; set; }
        public decimal Hold { get; set; }
        public decimal Free { get; set; }
        public string Currency { get; set; }
        public long RefNo { get; set; }
        public decimal SystemBal { get; set; }
        public string Type { get; set; }
        public string ReturnMsg { get; set; }
        public enErrorCode ErrorCode { get; set; }
    }

    public class LPKeyVault
    {
        public string APIKey { get; set; }
        public string SecretKey { get; set; }
        public long AppTypeID { get; set; }
    }

    #endregion

    public class ArbitrageBinanceCryptoWatcherCls
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

    public class ArbitrageBittrexCryptoWatcherCls
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<ArbitrageBittrexCryptoWatcherResultCls> result { get; set; }
    }
    public class ArbitrageBittrexCryptoWatcherResultCls
    {
        public string MarketName { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal High { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Low { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Volume { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Last { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal BaseVolume { get; set; }

        public DateTime TimeStamp { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Bid { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Ask { get; set; }
        public int OpenBuyOrders { get; set; }
        public int OpenSellOrders { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal PrevDay { get; set; }
        public DateTime Created { get; set; }
    }
    public class ArbitrageCoinbaseCryptoWatcherCls
    {
        public int trade_id { get; set; }
        public string price { get; set; }
        public string size { get; set; }
        public DateTime time { get; set; }
        public string bid { get; set; }
        public string ask { get; set; }
        public string volume { get; set; }
    }

    public class ArbitrageCryptoWatcherQryRes 
    {
        //select Pair,LPType,LTP as Price,Volume,PairId,UpDownBit,ChangePer,Fees from  CryptoWatcherArbitrage Where LpType={0} AND PairID={1}
        public string Pair { get; set; }
        public short LpType { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Price { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Volume { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChangePer { get; set; }
        public long PairID { get; set; }
        public short UpDownBit { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Fees { get; set; }
    }

    public class ArbitrageTradeSatoshiResultCls
    {
        public string market { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal high { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal low { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal volume { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal baseVolume { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal last { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal bid { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ask { get; set; }
        public int openBuyOrders { get; set; }
        public int openSellOrders { get; set; }
        public object marketStatus { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal change { get; set; }
    }

    public class ArbitrageTradeSatoshiCryptoWatcherCls
    {
        public bool success { get; set; }
        public object message { get; set; }
        public ArbitrageTradeSatoshiResultCls result { get; set; }
    }


    #region OKEx 
    /// <summary>
    /// New Method add by Pushpraj for Implement New API.
    /// </summary>
    public class OKExOrderBookBuySell
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal quantity { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal rate { get; set; }
    }

    public class OKExOrderBookResult
    {
        public List<OKExOrderBookBuySell> bids { get; set; }
        public List<OKExOrderBookBuySell> asks { get; set; }
    }

    public class OKExOrderBook
    {
        public bool success { get; set; }
        public object message { get; set; }
        public OKExOrderBookResult result { get; set; }
    }
    #endregion
    /// <summary>
    /// New Method add by Pushpraj for Implement New API.
    /// </summary>
}
