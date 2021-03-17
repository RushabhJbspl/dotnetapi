using Binance.Net.Objects;
using Bittrex.Net.Objects;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider1;
using CoinbasePro.Services.Orders.Models.Responses;
using CoinbasePro.Services.Products.Models;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Huobi.Net.Objects;
using CCXT.NET.Kraken.Trade;
using CCXT.NET.CEXIO.Trade;
using ExchangeSharp;

namespace Worldex.Core.Interfaces.LiquidityProvider
{
    public interface IBinanceLPService
    {
        Task<CallResult<BinanceCanceledOrder>> CancelOrderAsync(string symbol, long? orderId = null, string origClientOrderId = null, string newClientOrderId = null, long? receiveWindow = null);
        Task<CallResult<BinanceOrderBook>> GetOrderBookAsync(string market, int? limit = 100);
        Task GetMarketSummaryAsync(string market);
        Task<WebCallResult<BinanceAggregatedTrades[]>> GetTradeHistoryAsync(string market, int? limit = null);//Get recent trades // default limit 500
        Task<CallResult<BinancePlacedOrder>> PlaceOrderAsync(Binance.Net.Objects.OrderSide side, string market, Binance.Net.Objects.OrderType type, decimal quantity, string newClientOrderId = null, decimal? price = null, Binance.Net.Objects.TimeInForce? timeInForce = null, decimal? stopPrice = null, decimal? icebergQty = null, OrderResponseType? orderResponseType = null, int? receiveWindow = null);
        Task<WebCallResult<BinanceOrder>> GetOrderInfoAsync(string symbol, long? orderId = null, string origClientOrderId = null, long? receiveWindow = null);
        Task GetOpenOrdersAsync(string market = null, int? receiveWindow = null);
        //Task GetBalanceAsync(string currency);
        Task<WebCallResult<BinanceAccountInfo>> GetBalancesAsync();
        Task GetOrderHistoryAsync(string market = null);
        Task GetTradeFeeAsync(string market = null, int? receiveWindow = null);
        Task GetSystemStatusAsync();
        Task<WebCallResult<BinanceExchangeInfo>> GetExchangeInfoAsync();
        CallResult<BinanceTradeFee[]> GetTradeFee(string symbol);
    }
    
    public interface IBitrexLPService
    {
        Task<WebCallResult<BittrexOrderBook>> GetOrderBookAsync(string market);
        Task GetMarketSummaryAsync(string market);
        Task<CallResult<BittrexMarketHistory[]>> GetTradeHistoryAsync(string market);//GetMarketHistoryAsync
        Task<CallResult<BittrexGuid>> PlaceOrderAsync(Bittrex.Net.Objects.OrderSide side, string market, decimal quantity, decimal rate);
        Task<WebCallResult<object>> CancelOrderAsync(Guid guid);
        Task<CallResult<BittrexAccountOrder>> GetOrderInfoAsync(Guid guid);
        Task GetOpenOrdersAsync(string market = null);
        Task<CallResult<BittrexBalance>> GetBalanceAsync(string currency);
        Task GetBalancesAsync();
        Task GetOrderHistoryAsync(string market = null);
        Task<WebCallResult<Bittrex.Net.Objects.V3.BittrexOrderV3>> PlaceConditionalOrder(Bittrex.Net.Objects.OrderSide side, string market, decimal quantity, decimal rate);
    }

    public interface ICoinBaseService
    {
        void Connect();
        Task<OrderResponse> PlaceOrder(enTransactionMarketType marketType, CoinbasePro.Services.Orders.Types.OrderSide side, String Pair, decimal size, decimal limitPrice, decimal stopPrice, bool postOnly = false, Guid? clientOid = null);
        Task<CoinbaseCancelOrderRes> CancelOrderById(string id);
        Task<object> CancelAllOrders();
        Task<OrderResponse> GetOrderById(string id);
        Task<object> GetAllOrders(CoinbasePro.Services.Orders.Types.OrderStatus[] orderStatus, int limit = 100, int numberOfPages = 0);
        Task<object> GetAllCurrencies();
        Task<object> GetProductOrderBook(string Pair);
        Task<object> GetAllProducts();
        Task<object> GetProductStats(string Pair);
        Task<object> GetProductTicker(string Pair);
        Task<IList<IList<ProductTrade>>> GetTrades(string Pair, int limit = 100, int numberOfPages = 0);
        Task<object> GetFillsByProductId(string Pair, int limit = 100, int numberOfPages = 0);
        Task<IEnumerable<CoinbasePro.Services.Accounts.Models.Account>> GetAllAccountsAsync();
    }

    public interface IPoloniexService
    {
        //void Connect();
        Task<Object> GetPoloniexTicker();
        Task<Object> GetPoloniex24Volume();
        Task<Object> GetPoloniexCurrency();
        Task<Object> GetPoloniexOrderBooksAsync(string pair, long level);
        Task<Object> GetPoloniexTradeHistories(string BaseCur, string secondCur, DateTime start, DateTime End);
        Task<List<PoloniexTradeHistory>> GetPoloniexTradeHistoriesV1(string Market, int Limit);
        Task<Object> poloniexChartData(string BaseCur, string secondCur, DateTime start, DateTime End, long? period = 14400);
        Task<Object> GetPoloniexOpenOrder(string BaseCur, string secondCur);
        Task<Object> GetPoloniexOrderTrade(String orderNumber);
        Task<Object> GetPoloniexOrderState(String orderNumber);
        Task<Object> CancelPoloniexOrder(String orderNumber);
        Task<Dictionary<string, decimal>> PoloniexGetBalance();
        Task<String> PlacePoloniexOrder(string BaseCur, string secondCur, decimal amount, decimal rate, enOrderType orderType);
        string GetHash(string Sign, string secretKey);
    }

    public interface ITradeSatoshiLPService
    {
        //public API
        Task GetCurrenciesAsync();
        Task<GetOrderBookReturn> GetOrderBookAsync(string market, string type = "both", int? depth = null);
        Task GetMarketSummaryAsync(string market);
        Task<GetMarketHistoryReturn> GetTradeHistoryAsync(string market, int? count = null);//GetMarketHistoryAsync
        Task GetTickerAsync(string market);

         // private API
        Task<GetBalancesReturn> GetBalanceAsync(string currency);
        Task<GetBalanceReturn> GetBalancesAsync();
        Task<SubmitOrderReturn> PlaceOrderAsync(OrderSide side, string market, decimal quantity, decimal rate); // SubmitOrder
        Task<CancelOrderReturn> CancelOrderAsync(CancelOrderType type, long? orderID = null, string market = "");
        Task<GetOrderReturn> GetOrderInfoAsync(long orderID);
        Task<GetOrdersReturn> GetOpenOrdersAsync(string market, int? limit = 20);  //GetOrders      
        //Task GetOrderHistoryAsync(string market = null); // GetTradeHistory
    }
    public interface IHuobiLPService    
    {
        Task<WebCallResult<long>> CancelOrderAsync(long orderId);
        Task<WebCallResult<HuobiMarketDepth>>GetOrderBookAsync(string market, int limit = 100);
        Task GetMarketSummaryAsync(string market);
        Task<WebCallResult<List<HuobiMarketTrade>>> GetTradeHistoryAsync(string market, int limit = 20);//Get recent trades // default limit 500
       Task<WebCallResult<HuobiOrder>> GetOrderInfoAsync(long orderId );
        Task GetOpenOrdersAsync(string market = null, int? receiveWindow = null);
        Task<WebCallResult<List<HuobiBalance>>> GetBalancesAsync(long accountId);
        //Task GetOrderHistoryAsync(string market = null);
        Task<WebCallResult<List<HuobiOrderTrade>>> GetOrderHistoryAsync(string symbol, IEnumerable<HuobiOrderType> types = null, DateTime? startTime = null, DateTime? endTime = null, long? fromId = null, HuobiFilterDirection? direction = null, int? limit = null);

        Task<WebCallResult<List<HuobiSymbol>>> GetExchangeInfoAsync();
        Task<WebCallResult<long>> PlaceOrder(long accountId, string symbol, HuobiOrderType orderType, decimal amount, decimal? price = null);
     }

    public interface IUpbitService
    {
        Task<ListCurrencyResult> GetCurrenciesAsync();
        Task<UpbitOrderbookResponse> GetOrderBookAsync(string market);
        Task<TickerResponse> GetTickerAsync(string market);
        
        Task<CreateOrCancelOrderResponse> PlaceOrderAsync(string market, UpbitOrderSide side, decimal volume, decimal price, UpbitOrderType ord_type = UpbitOrderType.limit); 
        Task<UpbitCancelOrderResponse> CancelOrderAsync(string OrderId);
        Task<OrdersResponse> GetOrderInfoAsync(string OrderId);
        Task<TrandeHistoryResponse> GetTrandHistory(string market); 
    }
    /// <summary>
    /// Generate/Add new Interface for Implement New API OKEx By Pushpraj as on : 10-06-2019 Task Assign by: Khushali Medam
    /// </summary>

    #region OKEx Method Class
    public interface IOKExLPService 
    {
        //Private API
        Task<OKExCancelOrderReturn> CancelOrderAsync(string instrument_id, string order_id, string client_oid);
        Task<OKExPlaceOrderReturn> PlaceOrderAsync(string instrument_id, string OrderSide, decimal price, decimal size, int leverage, string client_oid, string match_price,string OrderType);
        Task<OKExGetOrderInfoReturn> GetOrderInfoAsync(string instrument_id, string order_id, string client_oid);
        Task<OKExGetAllOpenOrderReturn> GetOpenOrderAsync(string instrument_id, int? from, int? to, int? limit);
        Task<OKEBalanceResult> GetWalletBalanceAsync();
        Task<OKExGetWithdrawalFeeReturn> GetWithDrawalFeeAsync(string currency);

        //Public API
        Task<OKExGetOrderBookReturn> GetOrderBookAsync(string instrument_id, int? size, int? depth);
        Task<OKExGetMarketDataReturn> GetMarketSummaryDataAsync(string instrument_id, DateTime? start, DateTime? end, int? granularity);
        Task<GetOKEXTradeHistoryResult> GetTradeHistoryAasync(string instrument_id, int? from, int? to, int? limit);
        Task<OKExGetExchangeRateInfoReturn> GetExchangeRateAsync();
        Task<OKExGetTokenPairDetailReturn> GetTokenPairDetailAsync();
    }
    #endregion

    /// <summary>
    /// End Generate/Add new Interface for Implement New API OKEx By Pushpraj as on : 10-06-2019 Task Assign by: Khushali Medam
    /// </summary>

    

    public interface IKrakenLPService
    {
        Task<KrakenOrderBookResponse> GetOrderBook(string Market);
        Task<KrakenGetAssetInfoResponse> GetTradableAsset();
        Task<KrakenGetTradeHistoryResponse> GetTradeHistory(string Market);
        Task<KMyOrderItem> GetLPStatusCheck(bool trades, string userref, string txid);
        Task<KMyCancelOrder> CancelOrderAsync(string txid);
        Task<KrakenBalanceResponse> GetBalances();
        Task<KrakenPlaceOrderResponse> PlaceOrderAsyn(string market, string type, string orederType, decimal volume, decimal price);
    }

    public interface IBitfinexLPService
    {
        Task<BitfinexOrderBookResponse> GetOrderbook(string Market);
        Task<BitfinexTradeHistoryResult> GetTradeHistory(string Symbol);
        Task<BitfinexStatusCheckResponse> GetStatusCheck(int OrderId);
        Task<BitfinexBalanceResponse> GetBalanceData(string Symbol);
        Task<BitfinexPlaceOrderResponse> PlaceOrder(string Symbol, decimal amount, decimal price, string side, string type, string exchange, bool is_hidden, bool is_postonly, Int32 use_all_available, bool ocoorder, decimal buy_price_oco, decimal sell_price_oco, Int32 lev);
        Task<BitfinexCancelOrderResponse> cancelOrderAsync(string order_id);
    }

    public interface IGeminiLPService
    {
        Task<GeminiOrderbookResponse> GetOrderbook(string Market);
        Task<GeminiBalanceResponse> GetBalancesAsync();
        Task<GeminiTradeHistoryResponse> GetTradeHistory(string Symbol);
        Task<GeminiPlaceOrderRes> PlaceOrderAsync(OrderSide side, string market, decimal quantity, decimal rate);
        Task<GeminiStatusCheckResponse> GetStatusCheck(int OrderId);
    }

    public interface IYobitLPService
    {
        Task<ExchangeOrderBook> GetOrderBook(string Pair, int MaxCount);
        Task<List<ExchangeTrade>> GetTradeHistory(string Pair);
        Task<YobitAPIReqRes.GetInfoResponse> GetBalance();
        Task<YobitAPIReqRes.ExchangeOrderResult> GetLPStatusCheck(string OrderId, string Makret);
        Task<YobitAPIReqRes.TradeResponse> PlaceOrder(string Pair,string OrderType,decimal rate,decimal amount);
        Task<YobitAPIReqRes.YobitCancelOrderResult> CancelOrder(string OrderId);
    }

    public interface IEXMOLPService
    {
        Task<EXMOOrderbookResponse> GetOrederBook(string Market);
        Task<EXMOTradeHistoryResponse> GetTradeHistory(string Market);
        Task<EXMOBalanceResponse> GetBalance(string Currency);
        Task<EXMOPlaceOrderResponse> PlaceOrder(OrderSide side, string market, decimal quantity, decimal rate);
        Task<EXMOAPIReqRes.ExmoPlaceOrderResult> PlaceOrder(string Pair, string OrderType, decimal Qty, decimal price);
        Task<EXMOAPIReqRes.ExmoCancelOrderResult> CancelOrder(string OrderID);
        Task<EXMOOpenOrderResp> GetOpenOrders();
        Task<EXMOOrderTradeResponse> GetTradeByOrderId(string OrderID);
        Task<EXMOCancelOrderListResponse> GetCancelOrderList();
    }

    public interface ICEXIOLPService
    {
        Task<CEXIOGetOrederbookres> GetOrederBook(string Market);
        Task<List<CEXIOTradeHistory>> GetCEXIOTradeHistory(string market);
        Task<CEXIOBlanceCheckres> GetBalance(string market);
        Task<CPlaceOrderItem> PlaceOrder(string Symbol1, string Symbol2, decimal price, string type, decimal amount);
        Task<COpenOrderItem> GetStatusCheck(int OrderId);
        Task<Boolean> CancelOrder(string OredrId);
    }
}
