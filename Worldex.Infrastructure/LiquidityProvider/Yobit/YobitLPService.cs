using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ExchangeSharp;
using System.Security;



namespace Worldex.Infrastructure.LiquidityProvider.Yobit
{
    public class YobitLPService : IYobitLPService
    {
        public static string ControllerName = "YobitLPService";
        private IYobitLPService _yobitLPService;
        ExchangeYobitAPI _client = new ExchangeYobitAPI();
        public string PrivateURL { get; set; } = "https://yobit.net/tapi";
        public static string TradeApiUrl = "https://yobit.net/tapi";


        public YobitLPService()
        {
           
        }

        public async Task<ExchangeOrderBook> GetOrderBook(string Pair, int MaxCount)
        {
            ExchangeOrderBook result = await _client.GetOrderBookAsync(Pair, MaxCount);
            return result;     
        }

        public async Task<List<ExchangeTrade>> GetTradeHistory(string Pair)
        {
            List<ExchangeTrade> trades = new List<ExchangeTrade>();
            var result = await _client.GetRecentTradesAsync(Pair);
            foreach (var res in result)
            {
                trades.Add(res);
            }
            return trades;
        }

        public async Task<YobitAPIReqRes.GetInfoResponse> GetBalance()
        {
            try
            {
                var _yobitApiClient = new YobitApiClient(YobitGlobalSettings.API_Key, YobitGlobalSettings.Secret,true);
                YobitAPIReqRes.GetInfoResponse response = _yobitApiClient.GetInfo();
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("Getbalance", "YobitAPI", ex);
                return null;
            }
        }

        public async Task<YobitAPIReqRes.ExchangeOrderResult> GetLPStatusCheck(string OrderId,string Makret)
        {
            try
            {               
                var _yobitApiClient = new YobitApiClient(YobitGlobalSettings.API_Key, YobitGlobalSettings.Secret, true);
                var methoduri = new Uri(TradeApiUrl);
                var parmas = new Dictionary<string, string>
                {
                    { "method", "OrderInfo" },
                    { "order_id", OrderId.ToString() },
                    { "nonce", ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString() }
                };
                var _res = _yobitApiClient.ProcessTradeRequest(methoduri,parmas).Result;
                var response = JsonConvert.DeserializeObject<YobitAPIReqRes.ExchangeOrderResult>(_res);
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("Getbalance", "YobitAPI", ex);
                return null;
            }
        }

        public async Task<YobitAPIReqRes.TradeResponse> PlaceOrder(string Pair, string OrderType, decimal rate, decimal amount)
        {
            try
            {
                YobitGlobalSettings.API_Key = "0522C054E8D0722C7FA4D88A1ECC8C02";
                YobitGlobalSettings.Secret = "81a8fd9805b89be8791d27ac2ef45788";
                YobitAPIReqRes.TradeResponse response = new YobitAPIReqRes.TradeResponse();
                var _yobitApiClient = new YobitApiClient(YobitGlobalSettings.API_Key, YobitGlobalSettings.Secret, true);
                response = _yobitApiClient.Trade(Pair, OrderType, rate, amount);
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PlaceOrder", "YobitLPServices", ex);
                return null;
            }
        }

        public async Task<YobitAPIReqRes.YobitCancelOrderResult> CancelOrder(string OrderId)
        {
            try
            {   
                var _yobitApiClient = new YobitApiClient(YobitGlobalSettings.API_Key, YobitGlobalSettings.Secret, true);
                var methoduri = new Uri(TradeApiUrl);
                var parmas = new Dictionary<string, string>
                {
                    { "method", "CancelOrder" },
                    { "order_id", OrderId.ToString() },
                    { "nonce", ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString() }
                };
                var _res = _yobitApiClient.ProcessTradeRequest(methoduri, parmas).Result;
                var response = JsonConvert.DeserializeObject<YobitAPIReqRes.YobitCancelOrderResult>(_res);
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CancelOrder", "YobitLPServices", ex);
                return null;
            }
        }
    }
}
