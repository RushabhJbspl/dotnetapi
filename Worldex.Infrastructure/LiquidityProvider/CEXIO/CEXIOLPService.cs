using CCXT.NET.Bitfinex;
using CCXT.NET.CEXIO;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.LiquidityProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CCXT.NET.CEXIO.Trade;


namespace Worldex.Infrastructure.LiquidityProvider.CEXIO
{
    public class CEXIOLPService : ICEXIOLPService
    {

        public static string ControllerName = "CexioLPService";
        CexioClient _client = new CexioClient("", CEXIOGlobalSetting.API_Key, CEXIOGlobalSetting.Secret, "", "");

        //public async Task<CEXIOBlanceCheckres> GetBalance(string market)
        //{
        //    try
        //    {
        //        string data = "{ \"timestamp\": \"1513177918\", \"username\": \"ud000000000\", \"BTC\": { \"available\": \"1.38000000\", \"orders\": \"0.00000000\" }, \"BCH\": { \"available\": \"1.00000000\", \"orders\": \"0.00000000\" }, \"ETH\": { \"available\": \"100.00000000\", \"orders\": \"0.00000000\" }, \"LTC\": { \"available\": \"1.00000000\" }, \"DASH\": { \"available\": \"1.00000000\", \"orders\": \"0.00000000\" }, \"ZEC\": { \"available\": \"1.00000000\", \"orders\": \"0.00000000\" }, \"USD\": { \"available\": \"998087.07\", \"orders\": \"0.00\" }, \"EUR\": { \"available\": \"999562.56\", \"orders\": \"0.00\" }, \"GBP\": { \"available\": \"1000000.00\", \"orders\": \"0.00\" }, \"RUB\": { \"available\": \"1000000.00\", \"orders\": \"0.00\" }, \"GHS\": { \"available\": \"0.00000000\", \"orders\": \"0.00000000\" } }";
        //        //data = "{\"result\":" + data + "}";

        //        var result = JsonConvert.DeserializeObject<CEXIOBlanceCheckres>(data);

        //        //var jsonDataRes = JsonConvert.DeserializeObject<Dictionary<string, CEXIOBlanceCheckres>>(data.ToString());
        //        // return jsonDataRes;
        //        //var i = "";
        //        return result;

        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }

        //}

        public async Task<bool> CancelOrder(string OredrId)
        {
            try
            {

                string URL = "/api/cancel_order/";
                CEXIOGlobalSetting.API_Key = "HLixH1aYdOIWOoRyoHkFpHlawrM";
                CEXIOGlobalSetting.Secret = "0BDEDB9339DCE83D46F09A1527663762B52C8B12BCD1910E685BEAFEEABECFF9";
                CexioClient _client = new CexioClient("private", CEXIOGlobalSetting.API_Key, CEXIOGlobalSetting.Secret, "", "");
                var _params = new Dictionary<string, object>
                {
                    { "order_id", Convert.ToInt64(OredrId) }
                };
                var _request = await _client.CreatePostRequest(URL, _params);
                foreach (var _param in _request.Parameters)
                {
                    _params.Add(_param.Name, _param.Value);
                }


                _request.Parameters.Clear();
                var _nonce = DateTime.Now.Ticks.ToString();
                var result = _client.CallApiPost1Async(URL, _params).Result;
                if (result.Response.IsSuccessful)
                {
                    var data = result.Response.Content;
                    var data2 = result.Content;
                    var Resp = JsonConvert.DeserializeObject<bool>(data2);
                    return Resp;
                }
                HelperForLog.WriteLogIntoFile("cancelOrder", ControllerName, result.Content);
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("cancelOrder", ControllerName, ex);
                return false;
            }

        }

        //public async Task<List<CEXIOTradeHistory>> GetCEXIOTradeHistory(string Market)
        //{
        //    try
        //    {
        //       // CexioClient _client = new CexioClient("", CEXIOGlobalSetting.API_Key, CEXIOGlobalSetting.Secret, "", "");
        //        string URL = "https://cex.io/api/trade_history/#symbol1#";
        //        Market = Market.Contains("-") ? Market.Replace("-", "/") : Market.Contains("_") ? Market.Replace("_", "/") : Market;
        //        URL = URL.Replace("#symbol1#", Market);
        //        var ApiResp = await _client.CallApiGet1Async(URL);
        //        //ApiResp.Content = "{\"result\":" + ApiResp.Content + "}";
        //        if (ApiResp.Response.IsSuccessful)
        //        {
        //            var data = ApiResp.Response.Content;
        //            var data2 = ApiResp.Content;

        //            var Resp = JsonConvert.DeserializeObject<List<CEXIOTradeHistory>>(data2);
        //            return Resp;
        //        }
        //        HelperForLog.WriteLogIntoFile("GetTradeHistory", ControllerName, ApiResp.Content);
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog("GetTradeHistory", ControllerName, ex);
        //        return null;
        //    }
        //}

        //public async Task<CEXIOGetOrederbookres> GetOrederBook(string Market)
        //{
        //    try
        //    {
        //        CexioClient _client = new CexioClient("", CEXIOGlobalSetting.API_Key, CEXIOGlobalSetting.Secret,"","");
        //        string URL = "https://cex.io/api/order_book/#symbol1#";
        //        Market = Market.Contains("-") ? Market.Replace("-", "/") : Market.Contains("_") ? Market.Replace("_", "/") : Market;
        //        URL = URL.Replace("#symbol1#", Market);
        //        var ApiResp = await _client.CallApiGet1Async(URL);
        //       // ApiResp.Content = "{\"Data\":" + ApiResp.Content + "}";
        //        if (ApiResp.Response.IsSuccessful)
        //        {
        //            var data = ApiResp.Response.Content;
        //            var data2 = ApiResp.Content;
        //            var Resp = JsonConvert.DeserializeObject<CEXIOGetOrederbookres>(data2);
        //            return Resp;
        //        }
        //        HelperForLog.WriteLogIntoFile("GetOrderbook", ControllerName, ApiResp.Content);
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog("GetOrderBook", ControllerName, ex);
        //        return null;
        //    }
        //}

        public async Task<CEXIOBlanceCheckres> GetBalance(string market)
        {
            try
            {

                //CEXIOGlobalSetting.API_Key = "HLixH1aYdOIWOoRyoHkFpHlawrM";
                //CEXIOGlobalSetting.Secret = "0BDEDB9339DCE83D46F09A1527663762B52C8B12BCD1910E685BEAFEEABECFF9";
                //CexioClient _client = new CexioClient("private", CEXIOGlobalSetting.API_Key, CEXIOGlobalSetting.Secret, "", "");
                //var _params = new Dictionary<string, object>
                //{
                //    { }
                //};
                //var _request = await _client.CreatePostRequest(URL, _params);
                //foreach (var _param in _request.Parameters)
                //{
                //    _params.Add(_param.Name, _param.Value);
                //}


                //_request.Parameters.Clear();
                //var _nonce = DateTime.Now.Ticks.ToString();
                //var result = _client.CallApiPost1Async(URL, _params).Result;
                //if (result.Response.IsSuccessful)
                //{
                //    var data = result.Response.Content;
                //    var data2 = result.Content;
                //    var Resp = JsonConvert.DeserializeObject<CEXIOBlanceCheckres>(data2);
                //    return Resp;
                //}
                //HelperForLog.WriteLogIntoFile("BalanceCheck", ControllerName, result.Content);
                //return false;


                string data = "{ \"timestamp\": \"1513177918\", \"username\": \"ud000000000\", \"BTC\": { \"available\": \"1.38000000\", \"orders\": \"0.00000000\" }, \"BCH\": { \"available\": \"1.00000000\", \"orders\": \"0.00000000\" }, \"ETH\": { \"available\": \"100.00000000\", \"orders\": \"0.00000000\" }, \"LTC\": { \"available\": \"1.00000000\" }, \"DASH\": { \"available\": \"1.00000000\", \"orders\": \"0.00000000\" }, \"ZEC\": { \"available\": \"1.00000000\", \"orders\": \"0.00000000\" }, \"USD\": { \"available\": \"998087.07\", \"orders\": \"0.00\" }, \"EUR\": { \"available\": \"999562.56\", \"orders\": \"0.00\" }, \"GBP\": { \"available\": \"1000000.00\", \"orders\": \"0.00\" }, \"RUB\": { \"available\": \"1000000.00\", \"orders\": \"0.00\" }, \"GHS\": { \"available\": \"0.00000000\", \"orders\": \"0.00000000\" } }";
                //data = "{\"result\":" + data + "}";

                data = data.Replace(market, "symbol");
                var result = JsonConvert.DeserializeObject<CEXIOBlanceCheckres>(data);


                return result;

            }
            catch (Exception e)
            {
                return null;
            }

        }

        public async Task<List<CEXIOTradeHistory>> GetCEXIOTradeHistory(string Market)
        {
            try
            {
                // CexioClient _client = new CexioClient("", CEXIOGlobalSetting.API_Key, CEXIOGlobalSetting.Secret, "", "");
                string URL = "https://cex.io/api/trade_history/#symbol1#";
                Market = Market.Contains("-") ? Market.Replace("-", "/") : Market.Contains("_") ? Market.Replace("_", "/") : Market;
                URL = URL.Replace("#symbol1#", Market);
                var ApiResp = await _client.CallApiGet1Async(URL);
                //ApiResp.Content = "{\"result\":" + ApiResp.Content + "}";
                if (ApiResp.Response.IsSuccessful)
                {
                    var data = ApiResp.Response.Content;
                    var data2 = ApiResp.Content;

                    var Resp = JsonConvert.DeserializeObject<List<CEXIOTradeHistory>>(data2);
                    return Resp;
                }
                HelperForLog.WriteLogIntoFile("GetTradeHistory", ControllerName, ApiResp.Content);
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTradeHistory", ControllerName, ex);
                return null;
            }
        }

        public async Task<CEXIOGetOrederbookres> GetOrederBook(string Market)
        {
            try
            {
                CexioClient _client = new CexioClient("", CEXIOGlobalSetting.API_Key, CEXIOGlobalSetting.Secret, "", "");
                string URL = "https://cex.io/api/order_book/#symbol1#";
                Market = Market.Contains("-") ? Market.Replace("-", "/") : Market.Contains("_") ? Market.Replace("_", "/") : Market;
                URL = URL.Replace("#symbol1#", Market);
                var ApiResp = await _client.CallApiGet1Async(URL);
                // ApiResp.Content = "{\"Data\":" + ApiResp.Content + "}";
                if (ApiResp.Response.IsSuccessful)
                {
                    var data = ApiResp.Response.Content;
                    var data2 = ApiResp.Content;
                    var Resp = JsonConvert.DeserializeObject<CEXIOGetOrederbookres>(data2);
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


        public async Task<CPlaceOrderItem> PlaceOrder(string Symbol1, string Symbol2, decimal price, string type, decimal amount)
        {
            try
            {
                string url = "https://cex.io/api/get_order/" + Symbol1 + "/" + Symbol2;
                CEXIOGlobalSetting.API_Key = "zbYe8IlS4MVfFK1uABbcUrPz80wAyaMCr2dyzi2Yult";
                CEXIOGlobalSetting.Secret = "AjKpagyCYFAewEbfJYmCEftgAyxKp6IpF4ius62e7Em";

                CexioClient _cexioClient = new CexioClient("private", CEXIOGlobalSetting.API_Key, CEXIOGlobalSetting.Secret, "pkrana170@gmail.com", "pkrana@9944");

                var _params = new Dictionary<string, object>();
                //_params.Add("symbol1", Symbol1);
                //_params.Add("symbol2", Symbol2);                
                _params.Add("price", price);
                _params.Add("type", type);
                _params.Add("amount", amount);

                var _request = await _cexioClient.CreatePostRequest(url, _params);

                _params.Add(_request.Parameters[3].Name, _request.Parameters[3].Value);
                _params.Add(_request.Parameters[4].Name, _request.Parameters[4].Value);
                _params.Add(_request.Parameters[5].Name, _request.Parameters[5].Value);


                _request.Parameters.Clear();
                var _nonce = DateTime.Now.Ticks.ToString();

                var result = "{\r\n  \"complete\": false,\r\n  \"id\": \"89067468\",\r\n  \"time\": 1512054972480,\r\n  \"pending\": \"12.00000000\",\r\n  \"amount\": \"12.00000000\",\r\n  \"type\": \"buy\",\r\n  \"price\": \"1155.67\"\r\n}";
                var data = result;
                return JsonConvert.DeserializeObject<CPlaceOrderItem>(data);

                //var result = _cexioClient.CallApiPost1Async(url, _params).Result;               
                //if (result.Response.IsSuccessful)
                //{
                //    var data = result.Response.Content;
                //    return JsonConvert.DeserializeObject<CPlaceOrderItem>(data);
                //}
                //else
                //{
                //    var data = result.Response.Content;
                //    return JsonConvert.DeserializeObject<CPlaceOrderItem>(data);
                //}
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PlaceOrder", ControllerName, ex);
                return null;
            }

        }

        public async Task<COpenOrderItem> GetStatusCheck(int OrderId)
        {
            try
            {
                string url = "https://cex.io/api/get_order/";

                CEXIOGlobalSetting.API_Key = "HLixH1aYdOIWOoRyoHkFpHlawrM";
                CEXIOGlobalSetting.Secret = "0BDEDB9339DCE83D46F09A1527663762B52C8B12BCD1910E685BEAFEEABECFF9";

                CexioClient _cexioClient = new CexioClient("private", CEXIOGlobalSetting.API_Key, CEXIOGlobalSetting.Secret, "pkrana170@gmail.com", "pkrana@9944");

                var _params = new Dictionary<string, object>();
                _params.Add("order_id", OrderId);
                var _request = await _cexioClient.CreatePostRequest(url, _params);

                _params.Add(_request.Parameters[1].Name, _request.Parameters[1].Value);
                _params.Add(_request.Parameters[2].Name, _request.Parameters[2].Value);
                _params.Add(_request.Parameters[3].Name, _request.Parameters[3].Value);

                _request.Parameters.Clear();
                var _nonce = DateTime.Now.Ticks.ToString();

                var result = "{\r\n  \"id\": \"22347874\",\r\n  \"type\": \"buy\",\r\n  \"time\": 1470302860316,\r\n  \"lastTxTime\": \"2016-08-04T09:27:47.527Z\",\r\n  \"lastTx\": \"22347950\",\r\n  \"pos\": null,\r\n  \"user\": \"userId\",\r\n  \"status\": \"cd\",\r\n  \"symbol1\": \"BTC\",\r\n  \"symbol2\": \"USD\",\r\n  \"amount\": \"1.00000000\",\r\n  \"price\": \"564\",\r\n  \"fa:USD\": \"0.00\",\r\n  \"ta:USD\": \"359.72\",\r\n  \"remains\": \"0.36219371\",\r\n  \"a:BTC:cds\": \"0.63780629\",\r\n  \"a:USD:cds\": \"565.13\",\r\n  \"f:USD:cds\": \"0.00\",\r\n  \"tradingFeeMaker\": \"0\",\r\n  \"tradingFeeTaker\": \"0.2\",\r\n  \"tradingFeeStrategy\": \"Promo000Maker\",\r\n  \"orderId\": \"22347874\"\r\n}";
                var data = result;
                return JsonConvert.DeserializeObject<COpenOrderItem>(data);

                //var result = _cexioClient.CallApiPost1Async(url, _params).Result;
                //if (result.Response.IsSuccessful)
                //{
                //    var data = result.Response.Content;
                //    return JsonConvert.DeserializeObject<COpenOrderItem>(data);
                //}
                //else
                //{
                //    var data = result.Response.Content;
                //    return JsonConvert.DeserializeObject<COpenOrderItem>(data);
                //}

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetStatusCheck", ControllerName, ex);
                return null;
            }
        }
    }
}
