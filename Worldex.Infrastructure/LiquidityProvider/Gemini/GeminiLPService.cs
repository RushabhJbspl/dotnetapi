using CCXT.NET.Gemini;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.LiquidityProvider.Gemini
{
    public class GeminiLPService : IGeminiLPService
    {
        public static string ControllerName = "GeminiLPService";

        public async Task<GeminiBalanceResponse> GetBalancesAsync()
        {
            try
            {
                GeminiClient _client = new GeminiClient("private", GeminiGlobalSettings.API_Key, GeminiGlobalSettings.Secret);
                string URL = "https://api.gemini.com/v1/balances";
                var ApiResp = await _client.CallApiPost1Async(URL);
                //ApiResp.Content = "[  {    \"type\": \"exchange\",    \"currency\": \"BTC\",    \"amount\": \"1154.62034001\",    \"available\": \"1129.10517279\",    \"availableForWithdrawal\": \"1129.10517279\"  },  {    \"type\": \"exchange\",    \"currency\": \"USD\",    \"amount\": \"18722.79\",    \"available\": \"14481.62\",    \"availableForWithdrawal\": \"14481.62\"  },  {    \"type\": \"exchange\",    \"currency\": \"ETH\",    \"amount\": \"20124.50369697\",    \"available\": \"20124.50369697\",    \"availableForWithdrawal\": \"20124.50369697\"  }]";
                
                if (ApiResp.Response != null && ApiResp.Response.IsSuccessful)
                {
                    ApiResp.Content = "{\"Data\":" + ApiResp.Content + "}";
                    var data = ApiResp.Content;
                    //HelperForLog.WriteLogIntoFile("GetOrderbook", ControllerName, ApiResp.Content);
                    var Resp = JsonConvert.DeserializeObject<GeminiBalanceResponse>(data);                    
                    return Resp;
                }
                //HelperForLog.WriteLogIntoFile("GetOrderbook", ControllerName, ApiResp.Content);
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetBalancesAsync", ControllerName, ex);
                return null;
            }
        }

        public async Task<GeminiOrderbookResponse> GetOrderbook(string Market)
        {
            try
            {
                GeminiClient _client = new GeminiClient("public", GeminiGlobalSettings.API_Key, GeminiGlobalSettings.Secret);
                string URL = "https://api.gemini.com/v1/book/#symbol#";
                Market = Market.Contains("-") ? Market.Replace("-", "") : Market.Contains("_") ? Market.Replace("_", "") : Market;
                URL = URL.Replace("#symbol#", Market);
                var ApiResp = await _client.CallApiGet1Async(URL);
                ApiResp.Content = "{\"Data\":" + ApiResp.Content + "}";
                if (ApiResp.Response.IsSuccessful)
                {
                    var data = ApiResp.Response.Content;
                    var data2 = ApiResp.Content;
                    var Resp = JsonConvert.DeserializeObject<GeminiOrderbookResponse>(data2);
                    return Resp;
                }
                //HelperForLog.WriteLogIntoFile("GetOrderbook", ControllerName, ApiResp.Content);
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetOrderBook", ControllerName, ex);
                return null;
            }
        }

        public async Task<GeminiStatusCheckResponse> GetStatusCheck(int OrderId)
        {
            try
            {
                string url = "/v1/order/status";
                GeminiGlobalSettings.API_Key = "6rbVgzroV1Nvdm3zENUHMcsuLcLgXcjnikR5abviXkYiJ4bQi/SVhShS";
                GeminiGlobalSettings.Secret = "g8ETNZAx1TpjzCBBiRD03oMSa7uKYB1KiRm1Q5N14k0OUNy3PBXiQSI2ou6Rqqg6XPXaC30PRw1x/ql9Wk0yKg==";
                GeminiClient _client = new GeminiClient("private", GeminiGlobalSettings.API_Key, GeminiGlobalSettings.Secret);                
                var _params = new Dictionary<string, object>();
                var _request = await _client.CreatePostRequest(url, _params);
                {
                    _params.Add("order_id", OrderId);                    
                }
                _params.Add(_request.Parameters[0].Name, _request.Parameters[0].Value);
                _params.Add(_request.Parameters[1].Name, _request.Parameters[1].Value);
                _params.Add(_request.Parameters[2].Name, _request.Parameters[2].Value);

                var Result = await _client.CallApiPost1Async(url, _params);
                //var Result = "{  \"order_id\": \"44375901\",  \"id\": \"44375901\",  \"symbol\": \"btcusd\",  \"exchange\": \"gemini\",  \"avg_execution_price\": \"400.00\",  \"side\": \"buy\",  \"type\": \"exchange limit\",  \"timestamp\": \"1494870642\",  \"timestampms\": 1494870642156,  \"is_live\": false,  \"is_cancelled\": false,  \"is_hidden\": false,  \"was_forced\": false,  \"executed_amount\": \"3\",  \"remaining_amount\": \"0\",  \"options\": [],  \"price\": \"400.00\",  \"original_amount\": \"3\"}";
                
                //HelperForLog.WriteLogIntoFile("GetStatusCheck", ControllerName, Helpers.JsonSerialize(Result));
                if (Result.Content != null)
                {
                    if(Result.Content.Contains("message"))
                    {
                        var results = JsonConvert.DeserializeObject<GeminiStatusCheckResponse>(Result.Content);
                        return results;
                    }
                    Result.Content = "{\"Data\":" + Result.Content + "}";
                    var result = JsonConvert.DeserializeObject<GeminiStatusCheckResponse>(Result.Content);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetStatusCheck", ControllerName, ex);
                return null;
            }
        }

        public async Task<GeminiTradeHistoryResponse> GetTradeHistory(string Symbol)
        {
            try
            {
                GeminiClient _client = new GeminiClient("public", GeminiGlobalSettings.API_Key, GeminiGlobalSettings.Secret);
                string URL = "https://api.gemini.com/v1/trades/#Symbol#";
                Symbol = Symbol.Contains("-") ? Symbol.Replace("-", "") : Symbol.Contains("_") ? Symbol.Replace("_", "") : Symbol;
                URL = URL.Replace("#Symbol#", Symbol);
                var ApiResp = await _client.CallApiGet1Async(URL);
                ApiResp.Content = "{\"Data\":" + ApiResp.Content + "}";
                if (ApiResp.Response.IsSuccessful)
                {
                    var data = ApiResp.Response.Content;
                    var data2 = ApiResp.Content;
                    var Resp = JsonConvert.DeserializeObject<GeminiTradeHistoryResponse>(data2);
                    return Resp;
                }
                //HelperForLog.WriteLogIntoFile("GetTradeHistory", ControllerName, ApiResp.Content);
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTradeHistory", ControllerName, ex);
                return null;
            }
        }

        public async Task<GeminiPlaceOrderRes> PlaceOrderAsync(OrderSide side, string market, decimal quantity, decimal rate)
        {
            try
            {
                string url = "/v1/order/new";
                GeminiGlobalSettings.API_Key = "6rbVgzroV1Nvdm3zENUHMcsuLcLgXcjnikR5abviXkYiJ4bQi/SVhShS";
                GeminiGlobalSettings.Secret = "g8ETNZAx1TpjzCBBiRD03oMSa7uKYB1KiRm1Q5N14k0OUNy3PBXiQSI2ou6Rqqg6XPXaC30PRw1x/ql9Wk0yKg==";
                GeminiClient _client = new GeminiClient("private", GeminiGlobalSettings.API_Key, GeminiGlobalSettings.Secret);
                market = market.Contains("-") ? market.Replace("-", "") : market.Contains("_") ? market.Replace("_", "") : market;
                var _params = new Dictionary<string, object>();
                var _request = await _client.CreatePostRequest(url, _params);
                {
                    _params.Add("symbol", market);
                    _params.Add("side", side==OrderSide.Buy?"buy":"sell");
                    _params.Add("type", "exchange limit");
                    _params.Add("price", rate.ToString());
                    _params.Add("amount", quantity.ToString());
                }

                _params.Add(_request.Parameters[0].Name, _request.Parameters[0].Value);
                _params.Add(_request.Parameters[1].Name, _request.Parameters[1].Value);
                _params.Add(_request.Parameters[2].Name, _request.Parameters[2].Value);

                //var Result = await _client.CallApiPost1Async(url, _params);
                var Result = "{  \"order_id\": \"106817811\",  \"id\": \"106817811\",  \"symbol\": \"btcusd\",  \"exchange\": \"gemini\",  \"avg_execution_price\": \"3632.8508430064554\",  \"side\": \"buy\",  \"type\": \"exchange limit\",  \"timestamp\": \"1547220404\",  \"timestampms\": 1547220404836,  \"is_live\": true,  \"is_cancelled\": false,  \"is_hidden\": false,  \"was_forced\": false,  \"executed_amount\": \"3.7567928949\",  \"remaining_amount\": \"1.2432071051\",  \"client_order_id\": \"20190110-4738721\",  \"options\": [],  \"price\": \"3633.00\",  \"original_amount\": \"5\"}";
                //HelperForLog.WriteLogIntoFile("PlaceOrderAsync", ControllerName, Helpers.JsonSerialize(Result));
                if (Result!=null)
                {
                    //var data = Result.Response.Content;
                    //data = data.Replace(Market, "Data");
                    var result = JsonConvert.DeserializeObject<GeminiPlaceOrderRes>(Result);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PlaceOrderAsync", ControllerName, ex);
                return null;
            }
        }
    }
}
