using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Infrastructure.LiquidityProvider.UpbitAPI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.LiquidityProvider
{
    public class UpBitService : IUpbitService
    {
        public UpbitAPIClient _client;
        public UpBitService(UpbitAPIClient client)
        {
            _client = client;
        }
        public Task<UpbitCancelOrderResponse> CancelOrderAsync(string OrderId)
        {
            try
            {
                UpbitCancelOrderResponse Result = _client.CancelOrder(OrderId);
                return Task.FromResult(Result);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CancelOrderAsync", "UpBitService", ex);
                return null;
            }
        }

        public Task<ListCurrencyResult> GetCurrenciesAsync()
        {
            try
            {
                ListCurrencyResult Result = _client.GetAccount();
                return Task.FromResult(Result);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetCurrenciesAsync", "UpBitService", ex);
                return null;
            }
        }

        public Task<UpbitOrderbookResponse> GetOrderBookAsync(string market)
        {
            try
            {
                UpbitOrderbookResponse Result = _client.GetOrderbook(market);
                return Task.FromResult(Result);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetOrderBookAsync", "UpBitService", ex);
                return null;
            }
        }

        public Task<OrdersResponse> GetOrderInfoAsync(string OrderId)
        {
            try
            {
                OrdersResponse Result = _client.GetOrder(OrderId);
                return Task.FromResult(Result);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetOrderInfoAsync", "UpBitService", ex);
                return null;
            }
        }

        public Task<TickerResponse> GetTickerAsync(string market)
        {
            TickerResponse Result = _client.GetTicker(market);
            return Task.FromResult(Result);
        }

        public Task<TrandeHistoryResponse> GetTrandHistory(string market)
        {
            TrandeHistoryResponse Result = _client.GetTrandeHistory(market);
            return Task.FromResult(Result);
            //throw new NotImplementedException();
        }

        public Task<CreateOrCancelOrderResponse> PlaceOrderAsync(string market, UpbitOrderSide side, decimal volume, decimal price, UpbitOrderType ord_type = UpbitOrderType.limit)
        {
            try
            {
                CreateOrCancelOrderResponse Result = _client.MakeOrder(market, side, volume, price, ord_type);
                return Task.FromResult(Result);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PlaceOrderAsync", "UpBitService", ex);
                return null;
            }
        }
    }
}
