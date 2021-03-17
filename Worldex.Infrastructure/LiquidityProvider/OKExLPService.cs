using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Infrastructure.LiquidityProvider.OKExAPI;
using System.Threading.Tasks;
using System;

namespace Worldex.Infrastructure.LiquidityProvider
{
    public class OKExLPService : IOKExLPService
    {
        public OKExLPService()
        {

        }

        /////////Private API Implementation ////////////////////////
        /// <summary>
        /// Private API For CancelOrder
        /// </summary>
        /// <param name="instrument_id"></param>
        /// <param name="order_id"></param>
        /// <returns></returns>
        public Task<OKExCancelOrderReturn> CancelOrderAsync(string instrument_id, string order_id,string client_oid)
        {
            OKExCancelOrderReturn Result = APICall.cancelOrderAsync(instrument_id, order_id,client_oid).Result;
            return Task.FromResult(Result);
        }

        /// <summary>
        /// Private API for Make an Order
        /// </summary>
        /// <param name="instrument_id"></param>
        /// <param name="OrderSide"></param>
        /// <param name="price"></param>
        /// <param name="size"></param>
        /// <param name="leverage"></param>
        /// <param name="client_oid"></param>
        /// <param name="match_price"></param>
        /// <returns></returns>
        public Task<OKExPlaceOrderReturn> PlaceOrderAsync(string instrument_id, string OrderSide, decimal price, decimal size, int leverage, string client_oid, string match_price,string OrderType)
        {
            OKExPlaceOrderReturn Result = APICall.makeOrderAsync(instrument_id, OrderSide, price, size, leverage, client_oid, match_price,OrderType).Result;
            return Task.FromResult(Result);
        }

        /// <summary>
        /// Private API For Get OrderInformation
        /// </summary>
        /// <param name="instrument_id"></param>
        /// <param name="order_id"></param>
        /// <param name="client_oid"></param>
        /// <returns></returns>
        public Task<OKExGetOrderInfoReturn> GetOrderInfoAsync(string instrument_id, string order_id, string client_oid)
        {
            OKExGetOrderInfoReturn Result = APICall.getOrdersAsync(instrument_id,order_id,client_oid).Result;
            return Task.FromResult(Result);
        }

        /// <summary>
        /// Private API for Get Open/Pending Orders
        /// </summary>
        /// <param name="instrument_id"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public Task<OKExGetAllOpenOrderReturn> GetOpenOrderAsync(string instrument_id, int? from, int? to, int? limit)
        {
            OKExGetAllOpenOrderReturn Result = APICall.getPendingOrdersAsync(instrument_id, from, to, limit).Result;
            return Task.FromResult(Result);
        }

        /// <summary>
        /// Private API for GetWalletBalacne 
        /// </summary>
        /// <returns></returns>
        public Task<OKEBalanceResult> GetWalletBalanceAsync()
        {
            OKEBalanceResult Result = APICall.getWalletInfoAsync().Result;
            return Task.FromResult(Result);
        }

        /// <summary>
        /// Private API for GetWithdrawlFee/TradeFee
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public Task<OKExGetWithdrawalFeeReturn> GetWithDrawalFeeAsync(string currency)
        {
            OKExGetWithdrawalFeeReturn Result = APICall.getWithDrawalFeeAsync(currency).Result;
            return Task.FromResult(Result);
        }
        /////////End Private API Implementation ////////////////////////


        /////////Public API Implementation ////////////////////////

        /// <summary>
        /// Public API for Get Order book
        /// </summary>
        /// <param name="instrument_id"></param>
        /// <param name="size"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public Task<OKExGetOrderBookReturn> GetOrderBookAsync(string instrument_id, int? size, int? depth)
        {
            OKExGetOrderBookReturn Result = APICall.getBookAsync(instrument_id, size, depth).Result;
            return Task.FromResult(Result);
        }

        /// <summary>
        /// Public API for Get MarketSummaryData
        /// </summary>
        /// <param name="instrument_id"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="granularity"></param>
        /// <returns></returns>
        public Task<OKExGetMarketDataReturn> GetMarketSummaryDataAsync(string instrument_id, DateTime? start, DateTime? end, int? granularity)
        {
            OKExGetMarketDataReturn Result = APICall.getCandlesAsync(instrument_id, start, end, granularity).Result;
            return Task.FromResult(Result);
        }

        /// <summary>
        /// Public API For GetTradeHistory
        /// </summary>
        /// <param name="instrument_id"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public Task<GetOKEXTradeHistoryResult> GetTradeHistoryAasync(string instrument_id, int? from, int? to, int? limit)
        {
            GetOKEXTradeHistoryResult Result = APICall.getTradesAasync(instrument_id, from, to, limit).Result;
            return Task.FromResult(Result);
        }

        /// <summary>
        /// Public API for GetExchangeRate Information
        /// </summary>
        /// <returns></returns>
        public Task<OKExGetExchangeRateInfoReturn> GetExchangeRateAsync()
        {
            OKExGetExchangeRateInfoReturn Result = APICall.getRateAsync().Result;
            return Task.FromResult(Result);
        }

        public Task<OKExGetTokenPairDetailReturn> GetTokenPairDetailAsync()
        {
            OKExGetTokenPairDetailReturn Result = APICall.getTokenPairDetailAsycn().Result;
            return Task.FromResult(Result);
        }

        /////////End Public API Implementation ////////////////////////


    }
}
