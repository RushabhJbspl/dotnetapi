using CCXT.NET.Bitfinex;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.LiquidityProvider.Bitfinex
{
    public class BitfinexLPService : IBitfinexLPService
    {
        public static string ControllerName = "BitfinexLPService";
        
        public async Task<BitfinexOrderBookResponse> GetOrderbook(string Market)
        {
            try
            {
                BitfinexClient _client = new BitfinexClient("", BitfinexGlobalSettings.API_Key, BitfinexGlobalSettings.Secret);
                string URL = "https://api.bitfinex.com/v1/book/#symbol#";
                Market = Market.Contains("-") ? Market.Replace("-", "") : Market.Contains("_") ? Market.Replace("_", "") : Market;
                URL = URL.Replace("#symbol#", Market);
                var ApiResp = await _client.CallApiGet1Async(URL);
                ApiResp.Content = "{\"Data\":" + ApiResp.Content + "}";
                if (ApiResp.Response.IsSuccessful)
                {
                    var data = ApiResp.Response.Content;
                    var data2 = ApiResp.Content;
                    var Resp = JsonConvert.DeserializeObject<BitfinexOrderBookResponse>(data2);
                    return Resp;
                }
                HelperForLog.WriteLogIntoFile("GetOrderbook", ControllerName, ApiResp.Content);
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetOrderBook", ControllerName, ex);
                return null;
            }
        }

        public async Task<BitfinexTradeHistoryResult> GetTradeHistory(string Symbol)
        {
            try
            {
                Symbol = Symbol.Replace("_", "");
                string url = "https://api.bitfinex.com/v1/trades/###";
                url = url.Replace("###", Symbol);
                BitfinexClient _bitfinexClient = new BitfinexClient("", "", "");

                var result = await _bitfinexClient.CallApiGet1Async(url);
                if (result.Response.IsSuccessful)
                {
                    var data = result.Response.Content;
                    data = "{ result: " + data + "}";
                    return JsonConvert.DeserializeObject<BitfinexTradeHistoryResult>(data);
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTradeHistory", ControllerName, ex);
                return null;
            }
        }

        public async Task<BitfinexStatusCheckResponse> GetStatusCheck(int OrderId)
        {
            try
            {
                string url = "/v1/order/status";

                BitfinexGlobalSettings.API_Key = "zbYe8IlS4MVfFK1uABbcUrPz80wAyaMCr2dyzi2Yult";
                BitfinexGlobalSettings.Secret = "AjKpagyCYFAewEbfJYmCEftgAyxKp6IpF4ius62e7Em";

                BitfinexClient _bitfinexClient = new BitfinexClient("private", BitfinexGlobalSettings.API_Key, BitfinexGlobalSettings.Secret);

                var _params = new Dictionary<string, object>();
                _params.Add("order_id", OrderId);
                var _request = await _bitfinexClient.CreatePostRequest(url, _params);

                foreach (var _param in _request.Parameters)
                {
                    _params.Add(_param.Name, _param.Value);
                }
                _request.Parameters.Clear();
                var _nonce = DateTime.Now.Ticks.ToString();


                var result = _bitfinexClient.CallApiPost1Async(url, _params).Result;
                if (result.Response.IsSuccessful)
                {
                    var data = result.Response.Content;
                    return JsonConvert.DeserializeObject<BitfinexStatusCheckResponse>(data);
                }
                else
                {
                    var data = result.Response.Content;
                    return JsonConvert.DeserializeObject<BitfinexStatusCheckResponse>(data);
                }

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetStatusCheck", ControllerName, ex);
                return null;
            }
        }


        public async Task<BitfinexBalanceResponse> GetBalanceData(string Symbol)
        {
            try
            {
                string url = "/v1/history";
                BitfinexGlobalSettings.API_Key = "zbYe8IlS4MVfFK1uABbcUrPz80wAyaMCr2dyzi2Yult";
                BitfinexGlobalSettings.Secret = "AjKpagyCYFAewEbfJYmCEftgAyxKp6IpF4ius62e7Em";

                BitfinexClient _bitfinexClient = new BitfinexClient("private", BitfinexGlobalSettings.API_Key, BitfinexGlobalSettings.Secret);

                var _params = new Dictionary<string, object>();
                _params.Add("currency", Symbol);
                _params.Add("until", DateTime.Now.Ticks);
                _params.Add("limit", 500);
                _params.Add("wallet", "exchange");

                var _request = await _bitfinexClient.CreatePostRequest(url, _params);

                foreach (var _param in _request.Parameters)
                {
                    _params.Add(_param.Name, _param.Value);
                }
                _request.Parameters.Clear();
                var _nonce = DateTime.Now.Ticks.ToString();


                var result = _bitfinexClient.CallApiPost1Async(url, _params).Result;
                if (result.Response.IsSuccessful)
                {
                    var data = result.Response.Content;
                    return JsonConvert.DeserializeObject<BitfinexBalanceResponse>(data);
                }
                else
                {
                    var data = result.Response.Content;
                    return JsonConvert.DeserializeObject<BitfinexBalanceResponse>(data);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetBalanceData", ControllerName, ex);
                return null;
            }

        }

        public async Task<BitfinexPlaceOrderResponse> PlaceOrder(string Symbol, decimal amount, decimal price, string side, string type, string exchange, bool is_hidden, bool is_postonly,
            Int32 use_all_available, bool ocoorder, decimal buy_price_oco, decimal sell_price_oco, Int32 lev)
        {
            try
            {
                string url = "/v1/order/new";
                BitfinexGlobalSettings.API_Key = "zbYe8IlS4MVfFK1uABbcUrPz80wAyaMCr2dyzi2Yult";
                BitfinexGlobalSettings.Secret = "AjKpagyCYFAewEbfJYmCEftgAyxKp6IpF4ius62e7Em";

                BitfinexClient _bitfinexClient = new BitfinexClient("private", BitfinexGlobalSettings.API_Key, BitfinexGlobalSettings.Secret);

                var _params = new Dictionary<string, object>();
                _params.Add("symbol", Symbol);

                _params.Add("amount", amount.ToString());
                _params.Add("price", price.ToString());
                _params.Add("side", side);
                _params.Add("type", type);
                _params.Add("exchange", exchange);
                _params.Add("is_hidden", is_hidden);
                _params.Add("is_postonly", is_postonly);
                _params.Add("use_all_available", use_all_available);
                _params.Add("ocoorder", ocoorder);
                _params.Add("buy_price_oco", buy_price_oco);
                _params.Add("sell_price_oco", sell_price_oco);
                _params.Add("lev", lev.ToString());


                var _request = await _bitfinexClient.CreatePostRequest(url, _params);

                foreach (var _param in _request.Parameters)
                {
                    _params.Add(_param.Name, _param.Value);
                }
                _request.Parameters.Clear();
                var _nonce = DateTime.Now.Ticks.ToString();


                var result = _bitfinexClient.CallApiPost1Async(url, _params).Result;
                if (result.Response.IsSuccessful)
                {
                    var data = result.Response.Content;
                    return JsonConvert.DeserializeObject<BitfinexPlaceOrderResponse>(data);
                }
                else
                {
                    var data = result.Response.Content;
                    return JsonConvert.DeserializeObject<BitfinexPlaceOrderResponse>(data);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PlaceOrder", ControllerName, ex);
                return null;
            }

        }

        public async Task<BitfinexCancelOrderResponse> cancelOrderAsync(string order_id)
        {
            BitfinexCancelOrderResponse response = new BitfinexCancelOrderResponse();
            try
            {
                string url = "/v1/order/cancel";
                BitfinexGlobalSettings.API_Key = "zbYe8IlS4MVfFK1uABbcUrPz80wAyaMCr2dyzi2Yult";
                BitfinexGlobalSettings.Secret = "AjKpagyCYFAewEbfJYmCEftgAyxKp6IpF4ius62e7Em";

                BitfinexClient _bitfinexClient = new BitfinexClient("private", BitfinexGlobalSettings.API_Key, BitfinexGlobalSettings.Secret);

                var _params = new Dictionary<string, object>
                {
                    { "order_id", Convert.ToInt64(order_id) }
                };
                var _request = await _bitfinexClient.CreatePostRequest(url, _params);

                foreach (var _param in _request.Parameters)
                {
                    _params.Add(_param.Name, _param.Value);
                }
                _request.Parameters.Clear();
                var _nonce = DateTime.Now.Ticks.ToString();
                var result = _bitfinexClient.CallApiPost1Async(url, _params).Result;
                if (result.Response.IsSuccessful)
                {
                    var data = result.Response.Content;
                    var data2 = result.Content;
                    var Resp = JsonConvert.DeserializeObject<BitfinexCancelOrderResponse>(data2);
                    return Resp;
                }
                HelperForLog.WriteLogIntoFile("cancelOrderAsync", ControllerName, result.Content);
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("cancelOrderAsync", ControllerName, ex);
                response = null;
            }
            return response;
        }

    }
}
