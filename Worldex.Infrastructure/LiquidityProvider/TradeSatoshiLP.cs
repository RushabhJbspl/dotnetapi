using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Infrastructure.LiquidityProvider.TradeSatoshiAPI;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.LiquidityProvider
{
    public class TradeSatoshiLPService : ITradeSatoshiLPService
    {
        public TradeSatoshiLPService()
        {
            //GlobalSettings.API_Key = "39b7d529117a42f29695c035619ce22b";
            //GlobalSettings.Secret = "02cXvn92LTRRoQ3FSmbcblH555YXBtkehg+tdqNpzOY=";
        }

        // public API

        public Task<GetMarketHistoryReturn> GetTradeHistoryAsync(string market,int? count = null)
        {
            GetMarketHistoryReturn Result = APICall.GetMarketHistory(market,count).Result;
            return Task.FromResult(Result);
        }

        public Task GetMarketSummaryAsync(string market)
        {
            var Result = APICall.GetMarketSummary(market).Result;
            return Task.FromResult(Result);
        }

        public Task<GetOrderBookReturn> GetOrderBookAsync(string market, string type = "both", int? depth = null)
        {
            var Result = APICall.GetOrderBook(market,type,depth).Result;
            return Task.FromResult(Result);
        }

        public Task GetCurrenciesAsync()
        {
            var Result = APICall.GetCurrencies().Result;
            return Task.FromResult(Result);
        }

        public Task GetTickerAsync(string market)
        {
            var Result = APICall.GetTicker(market).Result;
            return Task.FromResult(Result);
        }

        // private API

        public Task<CancelOrderReturn> CancelOrderAsync(CancelOrderType type, long? orderID = null, string market = "")
        {
            CancelOrderReturn Result = APICall.CancelOrderAsync(type,orderID,market).Result;
            return Task.FromResult(Result);
        }

        public async Task<GetBalanceReturn> GetBalancesAsync()
        {
            var Result = APICall.GetBalances().Result;
            return Result;
        }
        
        public  Task<GetBalancesReturn> GetBalanceAsync(string currency)
        {
            var Result = APICall.GetBalance(currency).Result;
            //var Result1 = APICall.PlaceOrderAsync(currency).Result;
          return Task.FromResult(Result);
            //return Task.FromResult(Result);
        }        

        public Task GetOrderHistoryAsync(string market = null)
        {
            var Result = APICall.GetBalances().Result;
            return Task.FromResult(Result);
        }

        public Task<GetOrderReturn> GetOrderInfoAsync(long orderID) // order status
        {
            var Result = APICall.GetOrderAsync(orderID).Result;
            return Task.FromResult(Result);
        }

        public Task<GetOrdersReturn> GetOpenOrdersAsync(string market,int? limit = 20)
        {
            var Result = APICall.GetOrdersAsync(market, limit).Result;
            return Task.FromResult(Result);
        }
        
        public Task<SubmitOrderReturn> PlaceOrderAsync(OrderSide side, string market, decimal quantity, decimal rate)
        {
            var Result = APICall.PlaceOrderAsync(side,market, quantity, rate).Result;
            return Task.FromResult(Result);
        }        
    }    
   
}
