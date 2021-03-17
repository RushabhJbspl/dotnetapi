using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using CCXT.NET.Kraken.Trade;

namespace Worldex.Core.Interfaces.LiquidityProvider
{
    #region Public API
    public class GetCurrenciesReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public CurrencyResult[] result { get; set; }

        public class CurrencyResult
        {
            public string currency { get; set; }
            public string currencyLong { get; set; }
            public int minConfirmation { get; set; }
            public decimal txFee { get; set; }
            public string status { get; set; }
        }
    }

    public class GetTickerReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public TickerResult result { get; set; }

        public class TickerResult
        {
            public decimal bid { get; set; }
            public decimal ask { get; set; }
            public decimal last { get; set; }
            public string market { get; set; }
        }

    }

    public class GetMarketHistoryReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<MarketHistoryResult> result { get; set; }

        public class MarketHistoryResult
        {
            public long id { get; set; }
            public DateTime timeStamp { get; set; }
            public decimal quantity { get; set; }
            public decimal price { get; set; }
            public string orderType { get; set; }
            public decimal total { get; set; }
        }
    }

    public class GetMarketSummaryReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public MarketSummaryResult result { get; set; }
    }

    public class GetMarketSummariesReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public MarketSummaryResult[] result { get; set; }
    }

    public class GetOrderBookReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public OrderBookResult result { get; set; }

        public class OrderBookResult
        {
            public OrderBookItem[] buy { get; set; }
            public OrderBookItem[] sell { get; set; }
        }

        public class OrderBookItem
        {
            public decimal quantity { get; set; }
            public decimal rate { get; set; }
        }
    }
    #endregion

    #region Private API
    public class GetBalancesReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public BalanceResult result { get; set; }
    }

    public class GetOrderReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public OrderResult result { get; set; }
    }


    public class GetOrdersReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public OrderResult[] result { get; set; }
    }

    public class CancelOrderReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public CancelOrderResult result { get; set; }

        public class CancelOrderResult
        {
            public IList<long> CanceledOrders { get; set; }
        }
    }

    public class GetTradeHistoryReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int totalRecords { get; set; }
        public GetTradeHistoryResult result { get; set; }

        public class GetTradeHistoryResult
        {
            public int Id { get; set; }
            public string Market { get; set; }
            public string Type { get; set; }
            public decimal Amount { get; set; }
            public decimal Rate { get; set; }
            public decimal Fee { get; set; }
            public decimal Total { get; set; }
            public DateTime Timestamp { get; set; }
            public bool IsApi { get; set; }
        }
    }

    public class GenerateAddressReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public GenerateAddressResult result { get; set; }

        public class GenerateAddressResult
        {
            public string Currency { get; set; }
            public string Address { get; set; }
        }
    }

    public class SubmitWithdrawReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public SubmitWithdrawResult result { get; set; }

        public class SubmitWithdrawResult
        {
            public int WithdrawalId { get; set; }
        }
    }

    public class GetDepositsReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public GetDepositsResult[] result { get; set; }

        public class GetDepositsResult
        {
            public int Id { get; set; }
            public string Currency { get; set; }
            public string CurrencyLong { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; }
            public string Txid { get; set; }
            public int Confirmations { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    public class GetWithdrawlsReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public GetWithdrawalsResult[] result { get; set; }

        public class GetWithdrawalsResult
        {
            public int Id { get; set; }
            public string Currency { get; set; }
            public string CurrencyLong { get; set; }
            public decimal Amount { get; set; }
            public decimal Fee { get; set; }
            public string Address { get; set; }
            public string Status { get; set; }
            public string Txid { get; set; }
            public int Confirmations { get; set; }
            public DateTime Timestamp { get; set; }
            public bool IsApi { get; set; }
        }
    }

    public class SubmitTransferReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public SubmitTransferResult result { get; set; }

        public class SubmitTransferResult
        {
            public string data { get; set; }
        }
    }

    #endregion

    #region Shared Result Models
    public class MarketSummaryResult
    {
        public string market { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public decimal volume { get; set; }
        public decimal baseVolume { get; set; }
        public decimal last { get; set; }
        public decimal bid { get; set; }
        public decimal ask { get; set; }
        public int openBuyOrders { get; set; }
        public int openSellOrders { get; set; }
    }

    public class BalanceResult
    {
        public string currency { get; set; }
        public string currencyLong { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal available { get; set; }
        public double total { get; set; }
        public double heldForTrades { get; set; }
        public double unconfirmed { get; set; }
        public double pendingWithdraw { get; set; }
        public object address { get; set; }
    }

    public class GetBalanceReturn
    {
        public bool success { get; set; }
        public object message { get; set; }
        public List<BalanceResult>[] result { get; set; }
    }

    public class SubmitOrderReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public SubmitOrder result { get; set; }

        public class SubmitOrder
        {
            public long? OrderId { get; set; }
            public IList<long> Filled { get; set; }
        }
    }

    public class OrderResult
    {
        public int Id { get; set; }
        public string Market { get; set; }
        public string Type { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal Amount { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal Rate { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal Remaining { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal Total { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsApi { get; set; }
    }

    public enum OrderSide
    {
        Buy = 0,
        Sell = 1
    }

    public enum CancelOrderType
    {
        Single = 0,
        Market = 1,
        MarketBuys = 2,
        MarketSells = 3,
        AllBuys = 4,
        AllSells = 5,
        All = 6
    }

    public class BalanceInfo
    {
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "CurrencyLong")]
        public string CurrencyLongName { get; set; }
        public decimal Available { get; set; }
        public decimal Total { get; set; }
        public decimal HeldForTrades { get; set; }
        public decimal Unconfirmed { get; set; }
        public decimal PendingWithdraw { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            return $"Currency: {Currency}, CurrencyLongName: {CurrencyLongName}, Available: {Available}, Total: {Total}, HeldForTrades: {HeldForTrades}, Unconfirmed: {Unconfirmed}, PendingWithdrawl: {PendingWithdraw}, Address: {Address}";
        }
    }

    public class TradeSatoshiResponse<T>
    {
        [JsonProperty(PropertyName = "success")]
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        [JsonProperty(PropertyName = "result")]
        public T Data { get; set; }
    }

    public enum UpbitMinuteCandleType
    {
        _1 = 1,
        _3 = 3,
        _5 = 5,
        _10 = 10,
        _15 = 15,
        _30 = 30,
        _60 = 60,
        _240 = 240
    }
    public enum UpbitOrderSide
    {
        ask,
        bid
    }
    public enum UpbitOrderType
    {
        limit,
        price,
        market
    }

    public enum BitfinexOrderSide
    {
        buy,
        sell
    }


    public enum BitfinexOrderType
    {
        limit,
        price,
        market,
        stop
    }

    public enum CEXIOOrderSide
    {
        buy,
        sell
    }

    public enum CEXIOOrderType
    {
        limit,
        price,
        market,
        stop
    }
    #endregion

    /// <summary>
    /// Generate/Add new Class for Implement New API OKEx By Pushpraj as on : 10-06-2019 Task Assign by: Khushali Medam
    /// </summary>
    /// 
    #region OKEx Method Return class
    public class OKExCancelOrderReturn
    {
        public bool result { get; set; }
        public string client_oid { get; set; }
        public string order_id { get; set; }
        public string instrument_id { get; set; }
        public string error_message { get; set; }
        public int error_code { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class OKExGetOrderBookReturn
    {
        public List<decimal[]> asks { get; set; }
        public List<decimal[]> bids { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class OKExGetMarketDataReturn
    {
        public string time { get; set; }
        public string open { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string close { get; set; }
        public string volume { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class OKExGetFilledInformationReturn
    {
        public DateTime time { get; set; }
        public DateTime timestamp { get; set; }
        public string trade_id { get; set; }
        public string price { get; set; }
        public string size { get; set; }
        public string side { get; set; }
    }

    public class OKExPlaceOrderReturn
    {
        public bool result { get; set; }
        public string error_message { get; set; }
        public string error_code { get; set; }
        public string client_oid { get; set; }
        public string order_id { get; set; }
    }

    public class OKEXPlaceOrderRequest
    {
        public string notional { get; set; }
        public string client_oid { get; set; }
        public string type { get; set; }
        public string side { get; set; }
        public string instrument_id { get; set; }
        public int margin_trading { get; set; }
        public string size { get; set; }
        public string price { get; set; }
    }

    public class OKEXStatusCheckRequest
    {
        public string instrument_id { get; set; }
        public string order_id { get; set; }
        public string client_oid { get; set; }
    }

    public class OKExGetOrderInfoReturn
    {

        public string instrument_id { get; set; }
        public string size { get; set; }
        public DateTime timestamp { get; set; }
        public string filled_qty { get; set; }
        public string fee { get; set; }
        public string order_id { get; set; }
        public string price { get; set; }
        public string price_avg { get; set; }
        public string status { get; set; }
        public string state { get; set; }
        public string type { get; set; }
        public string contract_val { get; set; }
        public string leverage { get; set; }
        public string client_oid { get; set; }
        public string pnl { get; set; }
        public string order_type { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class GetOKEXTradeHistoryResult
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<OKExGetFilledInformationReturn> result { get; set; }
    }

    public class OKExGetAllOpenOrderReturn
    {
        public string client_oid { get; set; }
        public DateTime created_at { get; set; }
        public string filled_notional { get; set; }
        public string filled_size { get; set; }
        public string funds { get; set; }
        public string instrument_id { get; set; }
        public string notional { get; set; }
        public string order_id { get; set; }
        public string order_type { get; set; }
        public string price { get; set; }
        public string product_id { get; set; }
        public string side { get; set; }
        public string size { get; set; }
        public string status { get; set; }
        public string state { get; set; }
        public DateTime timestamp { get; set; }
        public string type { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class OKExGetWalletBalanceReturn
    {
        public decimal available { get; set; }
        public decimal balance { get; set; }
        public string currency { get; set; }
        public string hold { get; set; }
    }

    public class OKEBalanceResult
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<OKExGetWalletBalanceReturn> Data { get; set; }
    }

    public class OKExGetWithdrawalFeeReturn
    {
        public string currency { get; set; }
        public string max_fee { get; set; }
        public string min_fee { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class OKExGetExchangeRateInfoReturn
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<OKExGetTokeInformationReturn> Data { get; set; }
    }


    public class OKExGetTokeInformationReturn
    {
        public string instrument_id { get; set; }
        public string last { get; set; }
        public string best_bid { get; set; }
        public string best_ask { get; set; }
        public string high_24h { get; set; }
        public string low_24h { get; set; }
        public string volume_24h { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class OKExGetTokenPairDetailReturn
    {
        public string best_ask { get; set; }
        public string best_bid { get; set; }
        public string instrument_id { get; set; }
        public string product_id { get; set; }
        public string last { get; set; }
        public string ask { get; set; }
        public string bid { get; set; }
        public string open_24h { get; set; }
        public string high_24h { get; set; }
        public string low_24h { get; set; }
        public string base_volume_24h { get; set; }
        public DateTime timestamp { get; set; }
        public string quote_volume_24h { get; set; }
    }
    public class OkexData
    {
        public int code { get; set; }
        public string message { get; set; }
    }

    public class OKEXErrorObject
    {
        public OkexData Data { get; set; }
    }
    #endregion
    /// <summary>
    /// End Generate/Add new Class for Implement New API OKEx By Pushpraj as on : 10-06-2019 Task Assign by: Khushali Medam
    /// </summary>
    /// 
    #region KrakenAPI

    public class RespData
    {
        public string altname { get; set; }
        public string wsname { get; set; }
        public string aclass_base { get; set; }
        public string @base { get; set; }
        public string aclass_quote { get; set; }
        public string quote { get; set; }
        public string lot { get; set; }
        public int pair_decimals { get; set; }
        public int lot_decimals { get; set; }
        public int lot_multiplier { get; set; }
        public List<object> leverage_buy { get; set; }
        public List<object> leverage_sell { get; set; }
        public List<List<decimal>> fees { get; set; }
        public List<List<decimal>> fees_maker { get; set; }
        public string fee_volume_currency { get; set; }
        public int margin_call { get; set; }
        public int margin_stop { get; set; }
    }

    public class APIResultData
    {
        public RespData Data { get; set; }
    }

    public class KrakenGetAssetInfoResponse
    {
        public List<object> error { get; set; }
        public APIResultData result { get; set; }
    }

    public class KrakenGetOrderBookReturn
    {
        public List<decimal[]> asks { get; set; }
        public List<decimal[]> bids { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class KrakenGetTradeHistoryResponse
    {
        public List<object> error { get; set; }
        public KrakenTradeHistoryResult result { get; set; }
    }

    public class KrakenTradeHistoryResult
    {
        public List<List<object>> Data { get; set; }
        public string last { get; set; }
    }

    public class KrakenLTPData
    {
        public List<string> a { get; set; }
        public List<string> b { get; set; }
        public List<string> c { get; set; }
        public List<string> v { get; set; }
        public List<string> p { get; set; }
        public List<int> t { get; set; }
        public List<string> l { get; set; }
        public List<string> h { get; set; }
        public string o { get; set; }
    }

    public class KrakenLTPCheckResult
    {
        public KrakenLTPData Data { get; set; }
    }

    public class KrakenLTPCheckResponse
    {
        public List<object> error { get; set; }
        public KrakenLTPCheckResult result { get; set; }
    }

    public class Descr
    {
        public string order { get; set; }
        public string close { get; set; }
    }

    public class Result
    {
        public Descr descr { get; set; }
    }

    public class KrakenPlaceOrderResponse
    {
        public IList<object> error { get; set; }
        public Result result { get; set; }
        public string[] transactionIds { get; set; }
    }

    public class BitfinexGetOrderBookReturn
    {
        public List<List<Object>> asks { get; set; }
        public List<List<Object>> bids { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class BitfinexCancelOrderResponse
    {
        public int id { get; set; }
        public string symbol { get; set; }
        public object exchange { get; set; }
        public string price { get; set; }
        public string avg_execution_price { get; set; }
        public string side { get; set; }
        public string type { get; set; }
        public string timestamp { get; set; }
        public bool is_live { get; set; }
        public bool is_cancelled { get; set; }
        public bool is_hidden { get; set; }
        public bool was_forced { get; set; }
        public string original_amount { get; set; }
        public string remaining_amount { get; set; }
        public string executed_amount { get; set; }
    }

    public class KrakenBalanceResponse
    {
        public List<object> error { get; set; }
        public APIResultData result { get; set; }
    }

    public class BitfinexTradeHistory
    {
        public int timestamp { get; set; }
        public int tid { get; set; }
        public string price { get; set; }
        public string amount { get; set; }
        public string exchange { get; set; }
        public string type { get; set; }
    }

    public class BitfinexTradeHistoryResult
    {
        public List<BitfinexTradeHistory> result { get; set; }
    }

    public class BitfinexStatusCheckResponse
    {
        public int id { get; set; }
        public string symbol { get; set; }
        public object exchange { get; set; }
        public string price { get; set; }
        public string avg_execution_price { get; set; }
        public string side { get; set; }
        public string type { get; set; }
        public string timestamp { get; set; }
        public bool is_live { get; set; }
        public bool is_cancelled { get; set; }
        public bool is_hidden { get; set; }
        public object oco_order { get; set; }
        public bool was_forced { get; set; }
        public string original_amount { get; set; }
        public string remaining_amount { get; set; }
        public string executed_amount { get; set; }
        public string message { get; set; }
    }

    public class BitfinexTLTPResponse
    {
        public string mid { get; set; }
        public string bid { get; set; }
        public string ask { get; set; }
        public string last_price { get; set; }
        public string low { get; set; }
        public string high { get; set; }
        public string volume { get; set; }
        public string timestamp { get; set; }
    }

    public class Volume
    {
        public string C1 { get; set; }
        public string C2 { get; set; }
        public long timestamp { get; set; }
    }

    public class GeminiLTPResponse
    {
        public string ask { get; set; }
        public string bid { get; set; }
        public string last { get; set; }
        public Volume volume { get; set; }
    }

    public class BitfinexBalanceResponse
    {
        public string currency { get; set; }
        public string amount { get; set; }
        public string balance { get; set; }
        public string description { get; set; }
        public string timestamp { get; set; }
        public string message { get; set; }
    }

    public class BitfinexPlaceOrderResponse
    {
        public int id { get; set; }
        public string symbol { get; set; }
        public string exchange { get; set; }
        public string price { get; set; }
        public string avg_execution_price { get; set; }
        public string side { get; set; }
        public string type { get; set; }
        public string timestamp { get; set; }
        public bool is_live { get; set; }
        public bool is_cancelled { get; set; }
        public bool is_hidden { get; set; }
        public bool was_forced { get; set; }
        public string original_amount { get; set; }
        public string remaining_amount { get; set; }
        public string executed_amount { get; set; }
        public int order_id { get; set; }
        public string message { get; set; }
    }
    #endregion

    public class CEXIOTikerReturn
    {
        public string lprice { get; set; }
        public string curr1 { get; set; }
        public string curr2 { get; set; }
    }

    public class CEXIOGetOrederbookres
    {
        public int timestamp { get; set; }
        public List<List<double>> bids { get; set; }
        public List<List<double>> asks { get; set; }
        public string pair { get; set; }
        public int id { get; set; }
        public string sell_total { get; set; }
        public string buy_total { get; set; }
    }

    public class CEXIOTradeHistory
    {
        public string type { get; set; }
        public string date { get; set; }
        public string amount { get; set; }
        public string price { get; set; }
        public string tid { get; set; }
    }

    public class CEXIOBlanceCheckres
    {
        public string timestamp { get; set; }
        public string username { get; set; }
        public CEXIOBalance symbol { get; set; }
    }
    public class CEXIOBalance
    {
        public string available { get; set; }
        public string orders { get; set; }
    }
}