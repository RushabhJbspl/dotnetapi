using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Worldex.Core.ViewModels.LiquidityProvider;
using Newtonsoft.Json;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.LiquidityProvider;
using System.Collections.Generic;

namespace Worldex.Infrastructure.LiquidityProvider.UpbitAPI
{
    public class UpbitAPIClient
    {
        public MyHttpClient httpClient;
        string accessKey = UpBitGlobalSettings.API_Key;
        string secretKey = UpBitGlobalSettings.Secret;

        public UpbitAPIClient()
        {
            this.httpClient = new MyHttpClient(accessKey, secretKey);
        }

        public class MyHttpClient : HttpClient
        {
            private string _accessKey;
            private string _secretKey;

            public string AccessKey
            {
                get { return _accessKey; }
                set { _accessKey = value; }
            }
            public string SecretKey
            {
                get { return _secretKey; }
                set { _secretKey = value; }
            }
            public MyHttpClient(string accessKey, string secretKey)
            {
                if (string.IsNullOrWhiteSpace(accessKey)) { throw new ArgumentNullException("accessKey"); }
                if (string.IsNullOrWhiteSpace(secretKey)) { throw new ArgumentNullException("secretKey"); }
                _accessKey = accessKey;
                _secretKey = secretKey;
            }
        }

        #region API Methods

        //Balance Check & Currency Listing
        public ListCurrencyResult GetAccount()
        {
            try
            {
                string url = "https://api.upbit.com/v1/accounts";
                var data = CallAPI_NoParam(url, HttpMethod.Get);
                //var data = "[  {    \"currency\":\"KRW\",    \"balance\":\"1000000.0\",    \"locked\":\"0.0\",    \"avg_krw_buy_price\":\"0\",    \"modified\":false  },  {    \"currency\":\"BTC\",    \"balance\":\"2.0\",    \"locked\":\"0.0\",    \"avg_krw_buy_price\":\"101000\",    \"modified\":false  }]";
                if (data != null)
                {
                    ListCurrencyResult resp = new ListCurrencyResult();
                    try
                    {
                        var x = JsonConvert.DeserializeObject<List<CurrencyResult>>(data);
                        resp.Result = x;
                        return resp;
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetAccount", "UpbitAPI", e);
                return null;
            }
        }

        //Market Ticker
        public TickerResponse GetTicker(string markets)
        {
            try
            {
                string url = "https://api.upbit.com/v1/ticker";
                var data = CallAPI_WithParam(url, new NameValueCollection { { "markets", markets } }, HttpMethod.Get);
                if (data != null)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<TickerResponse>(data);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetTicker", "UpbitAPI", e);
                return null;
            }
        }

        //Pair Listing
        public ListPairsResponse GetMarkets()
        {
            try
            {
                string url = "https://api.upbit.com/v1/market/all";
                var data = CallAPI_NoParam(url, HttpMethod.Get);
                if (data != null)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<ListPairsResponse>(data);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetMarkets", "UpbitAPI", e);
                return null;
            }
        }

        //Order List
        public ListOrderResponse GetAllOrder()
        {
            try
            {
                string url = "https://api.upbit.com/v1/orders";
                var data = CallAPI_NoParam(url, HttpMethod.Get);
                if (data != null)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<ListOrderResponse>(data);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetAllOrder", "UpbitAPI", e);
                return null;
            }

        }

        //Get Order By Order ID
        public OrdersResponse GetOrder(string uuid)
        {
            try
            {
                string url = "https://api.upbit.com/v1/order";
                var data = "{\"uuid\":\"a08f09b1-1718-42e2-9358-f0e5e083d3ee\",\"side\":\"bid\",\"ord_type\":\"limit\",\"price\":\"17417000.0\",\"state\":\"done\",\"market\":\"KRW-BTC\",\"created_at\":\"2018-04-05T14:09:14+09:00\",\"volume\":\"1.0\",\"remaining_volume\":\"0.0\",\"reserved_fee\":\"26125.5\",\"remaining_fee\":\"25974.0\",\"paid_fee\":\"151.5\",\"locked\":\"17341974.0\",\"executed_volume\":\"1.0\",\"trades_count\":2,\"trades\":[{\"market\":\"KRW-BTC\",\"uuid\":\"78162304-1a4d-4524-b9e6-c9a9e14d76c3\",\"price\":\"101000.0\",\"volume\":\"0.77368323\",\"funds\":\"78142.00623\",\"side\":\"bid\"},{\"market\":\"KRW-BTC\",\"uuid\":\"f73da467-c42f-407d-92fa-e10d86450a20\",\"price\":\"101000.0\",\"volume\":\"0.22631677\",\"funds\":\"22857.99377\",\"side\":\"bid\"}]}";
                //var data = CallAPI_WithParam(url, new NameValueCollection { { "uuid", uuid } }, HttpMethod.Get);
                if (data != null)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<OrdersResponse>(data);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetOrder", "UpbitAPI", e);
                return null;
            }

        }

        //Place Order
        public CreateOrCancelOrderResponse MakeOrder(string market, UpbitOrderSide side, decimal volume, decimal price, UpbitOrderType ord_type = UpbitOrderType.limit)
        {
            try
            {
                string url = "https://api.upbit.com/v1/orders";
                var data = "{  \"uuid\": \"cdd92199-2897-4e14-9448-f923320408ad\",  \"side\": \"bid\",  \"ord_type\": \"limit\",  \"price\": \"100.0\",  \"avg_price\": \"0.0\",  \"state\": \"wait\",  \"market\": \"KRW-BTC\",  \"created_at\": \"2018-04-10T15:42:23+09:00\",  \"volume\": \"0.01\",  \"remaining_volume\": \"0.01\",  \"reserved_fee\": \"0.0015\",  \"remaining_fee\": \"0.0015\",  \"paid_fee\": \"0.0\",  \"locked\": \"1.0015\",  \"executed_volume\": \"0.0\",  \"trades_count\": 0}";
                //var data = CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "side", side.ToString() }, { "volume", volume.ToString() }, { "price", price.ToString() }, { "ord_type", ord_type.ToString() } }, HttpMethod.Post);
                if (data != null)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<CreateOrCancelOrderResponse>(data);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("MakeOrder", "UpbitAPI", e);
                return null;
            }
        }

        //Cancel Order
        public UpbitCancelOrderResponse CancelOrder(string uuid)
        {
            try
            {
                string url = "https://api.upbit.com/v1/order";
                //var data = CallAPI_WithParam(url, new NameValueCollection { { "uuid", uuid } }, HttpMethod.Delete);
                var data = "{\"uuid\":\"cdd92199-2897-4e14-9448-f923320408ad\",\"side\":\"bid\",\"ord_type\":\"limit\",\"price\":\"0.000266379747475288\",\"state\":\"cancel\",\"market\":\"BTC-LTC\",\"created_at\":\"2018-04-10T15:42:23+09:00\",\"volume\":\"7021.690265490000000000\",\"remaining_volume\":\"3510.845132745000000000\",\"reserved_fee\":\"0.0015\",\"remaining_fee\":\"0.0015\",\"paid_fee\":\"0.0\",\"locked\":\"1.0015\",\"executed_volume\":\"3510.845132745000000000\",\"trades_count\":0}";
                if (data != null)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<UpbitCancelOrderResponse>(data);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("CancelOrder", "UpbitAPI", e);
                return null;
            }

        }

        //Order Book
        public UpbitOrderbookResponse GetOrderbook(string markets)
        {
            try
            {
                string url = "https://api.upbit.com/v1/orderbook";
                //var data = CallAPI_WithParam(url, new NameValueCollection { { "markets", markets } }, HttpMethod.Get);
                var data = "{\"market\":\"BTC-LTC\",\"timestamp\":1529910247984,\"total_ask_size\":8.83621228,\"total_bid_size\":2.43976741,\"orderbook_units\":[{\"ask_price\":6956000,\"bid_price\":6954000,\"ask_size\":0.24078656,\"bid_size\":0.00718341},{\"ask_price\":6958000,\"bid_price\":6953000,\"ask_size\":1.12919,\"bid_size\":0.11500074},{\"ask_price\":6960000,\"bid_price\":6952000,\"ask_size\":0.08614137,\"bid_size\":0.19019028},{\"ask_price\":6962000,\"bid_price\":6950000,\"ask_size\":0.0837203,\"bid_size\":0.28201649},{\"ask_price\":6964000,\"bid_price\":6949000,\"ask_size\":0.501885,\"bid_size\":0.01822085},{\"ask_price\":6965000,\"bid_price\":6946000,\"ask_size\":1.12517189,\"bid_size\":0.0002},{\"ask_price\":6968000,\"bid_price\":6945000,\"ask_size\":2.89900477,\"bid_size\":0.03597913},{\"ask_price\":6970000,\"bid_price\":6944000,\"ask_size\":0.2044231,\"bid_size\":0.39291445},{\"ask_price\":6972000,\"bid_price\":6939000,\"ask_size\":2.55280097,\"bid_size\":0.12963816},{\"ask_price\":6974000,\"bid_price\":6937000,\"ask_size\":0.01308832,\"bid_size\":1.2684239}]}";
                if (data != null)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<UpbitOrderbookResponse>(data);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetOrderbook", "UpbitAPI", e);
                return null;
            }

        }
        //TrandeHistory
        public TrandeHistoryResponse GetTrandeHistory(string market)
        {
            try
            {
                //string url = "https://api.upbit.com/v1/ticker?markets=KRW-BTC";
                string url = "https://crix-api.upbit.com/v1/crix/trades/ticks?code=CRIX.UPBIT.KRW-XRP&count=1";
                var data = CallAPI_NoParam(url, HttpMethod.Get);
                //var data = "{\"market\":\"BTC-LTC\",\"timestamp\":1529910247984,\"total_ask_size\":8.83621228,\"total_bid_size\":2.43976741,\"orderbook_units\":[{\"ask_price\":6956000,\"bid_price\":6954000,\"ask_size\":0.24078656,\"bid_size\":0.00718341},{\"ask_price\":6958000,\"bid_price\":6953000,\"ask_size\":1.12919,\"bid_size\":0.11500074},{\"ask_price\":6960000,\"bid_price\":6952000,\"ask_size\":0.08614137,\"bid_size\":0.19019028},{\"ask_price\":6962000,\"bid_price\":6950000,\"ask_size\":0.0837203,\"bid_size\":0.28201649},{\"ask_price\":6964000,\"bid_price\":6949000,\"ask_size\":0.501885,\"bid_size\":0.01822085},{\"ask_price\":6965000,\"bid_price\":6946000,\"ask_size\":1.12517189,\"bid_size\":0.0002},{\"ask_price\":6968000,\"bid_price\":6945000,\"ask_size\":2.89900477,\"bid_size\":0.03597913},{\"ask_price\":6970000,\"bid_price\":6944000,\"ask_size\":0.2044231,\"bid_size\":0.39291445},{\"ask_price\":6972000,\"bid_price\":6939000,\"ask_size\":2.55280097,\"bid_size\":0.12963816},{\"ask_price\":6974000,\"bid_price\":6937000,\"ask_size\":0.01308832,\"bid_size\":1.2684239}]}";
                //var data = "";
                if (data != null)
                {
                    try
                    {

                        return JsonConvert.DeserializeObject<TrandeHistoryResponse>(data);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetOrderbook", "UpbitAPI", e);
                return null;
            }



        }


        //public string GetOrderChance(string market)
        //{
        //    string url = "https://api.upbit.com/v1/orders/chance";
        //    return CallAPI_WithParam(url, new NameValueCollection { { "market", market } }, HttpMethod.Get);
        //    return JsonConvert.DeserializeObject<TickerResponse>(data);
        //}


        //public string GetCandles_Minute(string market, UpbitMinuteCandleType unit, DateTime to = default(DateTime), int count = 1)
        //{
        //    string url = "https://api.upbit.com/v1/candles/minutes/" + (int)unit;
        //    return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "to", (to == default(DateTime)) ? DateTime2String(DateTime.Now) : DateTime2String(to) }, { "count", count.ToString() } }, HttpMethod.Get);
        //}


        //public string GetCandles_Day(string market, DateTime to = default(DateTime), int count = 1)
        //{
        //    string url = "https://api.upbit.com/v1/candles/days";
        //    return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "to", (to == default(DateTime)) ? DateTime2String(DateTime.Now) : DateTime2String(to) }, { "count", count.ToString() } }, HttpMethod.Get);
        //}


        //public string GetCandles_Week(string market, DateTime to = default(DateTime), int count = 1)
        //{
        //    string url = "https://api.upbit.com/v1/candles/weeks";
        //    return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "to", (to == default(DateTime)) ? DateTime2String(DateTime.Now) : DateTime2String(to) }, { "count", count.ToString() } }, HttpMethod.Get);
        //}


        //public string GetCandles_Month(string market, DateTime to = default(DateTime), int count = 1)
        //{
        //    string url = "https://api.upbit.com/v1/candles/months";
        //    return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "to", (to == default(DateTime)) ? DateTime2String(DateTime.Now) : DateTime2String(to) }, { "count", count.ToString() } }, HttpMethod.Get);
        //}


        //public string GetTicks(string market, int count = 1)
        //{
        //    string url = "https://api.upbit.com/v1/trades/ticks";
        //    return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "count", count.ToString() } }, HttpMethod.Get);
        //}

        #endregion

        #region Thirdparty API Call

        private string CallAPI_NoParam(string url, HttpMethod httpMethod)
        {
            var requestMessage = new HttpRequestMessage(httpMethod, new Uri(url));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JWT_NoParameter());
            var response = httpClient.SendAsync(requestMessage).Result;
            var contents = response.Content.ReadAsStringAsync().Result;
            return contents;
        }
        private string CallAPI_WithParam(string url, NameValueCollection nvc, HttpMethod httpMethod)
        {
            var requestMessage = new HttpRequestMessage(httpMethod, new Uri(url + "?" + ToQueryString(nvc, true)));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JWT_WithParameter(nvc));
            var response = httpClient.SendAsync(requestMessage).Result;
            var contents = response.Content.ReadAsStringAsync().Result;
            return contents;
        }

        private string JWT_NoParameter()
        {
            TimeSpan diff = DateTime.Now - new DateTime(1970, 1, 1);
            var nonce = Convert.ToInt64(diff.TotalMilliseconds);
            var payload = new JwtPayload { { "access_key", httpClient.AccessKey }, { "nonce", nonce } };
            byte[] keyBytes = Encoding.Default.GetBytes(httpClient.SecretKey);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
            var header = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(header, payload);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
            var authorizationToken = jwtToken;
            return authorizationToken;
        }
        private string JWT_WithParameter(NameValueCollection nvc)
        {
            TimeSpan diff = DateTime.Now - new DateTime(1970, 1, 1);
            var nonce = Convert.ToInt64(diff.TotalMilliseconds);

            string queryString = ToQueryString(nvc, false);
            var payload = new JwtPayload { { "access_key", httpClient.AccessKey }, { "nonce", nonce }, { "query", queryString } };
            byte[] keyBytes = Encoding.Default.GetBytes(httpClient.SecretKey);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
            var header = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(header, payload);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
            var authorizationToken = jwtToken;
            return authorizationToken;
        }
        private string ToQueryString(NameValueCollection nvc, bool isURI)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", (isURI) ? HttpUtility.UrlEncode(key) : key, (isURI) ? HttpUtility.UrlEncode(value) : value))
                .ToArray();
            return string.Join("&", array);
        }
        private string DateTime2String(DateTime to)
        {
            return to.ToString("s") + "+09:00";
        }

        #endregion

        #region Enums

        //public enum UpbitMinuteCandleType { _1 = 1, _3 = 3, _5 = 5, _10 = 10, _15 = 15, _30 = 30, _60 = 60, _240 = 240 }
        //public enum UpbitOrderSide { ask, bid }
        //public enum UpbitOrderType { limit,price,market }

        #endregion
    }
}
