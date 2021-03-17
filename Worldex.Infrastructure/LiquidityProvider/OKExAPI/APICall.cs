using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.LiquidityProvider;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
//using OKExSDK;
//using OKExSDK.Models;
using System.Net.Http.Headers;
using System.Threading;


namespace Worldex.Infrastructure.LiquidityProvider.OKExAPI
{
    static class Encryptor
    {
        public static string HmacSHA256(string infoStr, string secret)
        {
            byte[] sha256Data = Encoding.UTF8.GetBytes(infoStr);
            byte[] secretData = Encoding.UTF8.GetBytes(secret);
            using (var hmacsha256 = new HMACSHA256(secretData))
            {
                byte[] buffer = hmacsha256.ComputeHash(sha256Data);
                return Convert.ToBase64String(buffer);
            }
        }

        public static string MakeSign(string apiKey, string secret, string phrase)
        {
            var timeStamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var sign = Encryptor.HmacSHA256($"{timeStamp}GET/users/self/verify", secret);
            var info = new
            {
                op = "login",
                args = new List<string>()
                        {
                        apiKey,phrase,timeStamp.ToString(),sign
                        }
            };
            return JsonConvert.SerializeObject(info);
        }
    }

    public class objRootObject
    {
        public DateTime iso { get; set; }
        public string epoch { get; set; }
    }

    public class APICall
    {
        public static string ControllerName = "LiquidityProvider.OKExAPI.APICall";

        #region Private API for cancel Order
        public async static Task<OKExCancelOrderReturn> cancelOrderAsync(string instrument_id, string order_id, string client_oid)
        {
            OKExCancelOrderReturn response = new OKExCancelOrderReturn();
            try
            {
                string BASEURL = "https://www.okex.com/";
                string SPOT_SEGMENT = "api/spot/v3";
                var url = $"{BASEURL}{SPOT_SEGMENT}/cancel_orders/{order_id}";
                var body = new {
                    instrument_id = instrument_id,
                    client_oid = client_oid
                };
                var bodyStr = JsonConvert.SerializeObject(body);
                using (var client = new HttpClient(new HttpInterceptor(OKEXGlobalSettings.API_Key, OKEXGlobalSettings.Secret, OKEXGlobalSettings.PassPhrase, bodyStr)))
                {
                    var res = await client.PostAsync(url, new StringContent(bodyStr, Encoding.UTF8, "application/json"));
                    var contentStr = await res.Content.ReadAsStringAsync();
                    if (!(contentStr.ToLower().Contains("error_code") || contentStr.ToLower().Contains("code")) && !contentStr.ToLower().Contains("error_message"))
                    {
                        contentStr = "{\"Data\":" + contentStr + "}";
                    }
                    else
                    {
                        HelperForLog.WriteLogIntoFile("cancelOrderAsync", ControllerName, "API Response : " + contentStr);
                    }

                    response = JsonConvert.DeserializeObject<OKExCancelOrderReturn>(contentStr);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("cancelOrderAsync", ControllerName, ex);
                response = null;
            }
            return response;
        }

        #endregion

        #region public API for GetOrderBook
        public async static Task<OKExGetOrderBookReturn> getBookAsync(string instrument_id, int? size, int? depth)
        {
            OKExGetOrderBookReturn Response = new OKExGetOrderBookReturn();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var Result = await client.GetAsync("https://www.okex.com/api/spot/v3/instruments/" + instrument_id + "/book?size=" + size.ToString() + "&depth=" + depth.ToString()).Result.Content.ReadAsStringAsync();
                    //var Result = "{\"asks\":[[\"7980\",\"3.23064069\",\"11\"],[\"7990\",\"1.8687824\",\"6\"]],\"bids\":[[\"7970\",\"8.16271663\",\"18\"]],\"timestamp\":\"2019-06-12T12:57:35.483Z\"}";
                    if (!Result.Contains("<!DOCTYPE html>") && Result != null)
                    {
                        try
                        {
                            if (!Result.ToLower().Contains("code") && !Result.ToLower().Contains("message"))
                            {
                                Response = JsonConvert.DeserializeObject<OKExGetOrderBookReturn>(Result);
                            }
                            else
                            {
                                HelperForLog.WriteLogIntoFile("getBookAsync", ControllerName, "API Response : " + Result);
                            }

                        }
                        catch (Exception e)
                        {
                            HelperForLog.WriteErrorLog("getBookAsync", ControllerName, e);
                            Response = null;
                        }
                    }
                    else
                    {
                        HelperForLog.WriteLogIntoFile("getBookAsync", ControllerName, "API Response : " + Result);
                        Response = null;
                    }

                }
                catch (Exception ex)
                {

                    HelperForLog.WriteErrorLog("getBookAsync", ControllerName, ex);
                    Response = null;
                }
            }
            return Response;
        }
        #endregion

        #region Public API for GetMarketData
        public async static Task<OKExGetMarketDataReturn> getCandlesAsync(string instrument_id, DateTime? start, DateTime? end, int? granularity)
        {
            OKExGetMarketDataReturn Response = new OKExGetMarketDataReturn();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var Result = await client.GetAsync("https://www.okex.com/api/spot/v3/instruments" + instrument_id + "/candles?granularity" + granularity.ToString() + "&start=" + start.ToString() + "&end=" + end.ToString()).Result.Content.ReadAsStringAsync();
                    if (!Result.Contains("<!DOCTYPE html>") && Result != null)
                    {
                        if (!Result.ToLower().Contains("code") && !Result.ToLower().Contains("message"))
                        {
                            Response = JsonConvert.DeserializeObject<OKExGetMarketDataReturn>(Result);
                        }
                        else
                        {
                            HelperForLog.WriteLogIntoFile("getCandlesAsync", ControllerName, "API Response : " + Result);
                            Response = JsonConvert.DeserializeObject<OKExGetMarketDataReturn>(Result);
                        }
                    }
                    else
                    {
                        HelperForLog.WriteLogIntoFile("getCandlesAsync", ControllerName, "API Response : " + Result);
                        Response = null;
                    }
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteLogIntoFile("getCandlesAsync", ControllerName, "API Response : " + ex.Message);
                    Response = null;
                }
                return Response;
            }
        }
        #endregion

        #region Public API for GetFillerInformation(Trade History)
        public async static Task<GetOKEXTradeHistoryResult> getTradesAasync(string instrument_id, int? from, int? to, int? limit)
        {
            GetOKEXTradeHistoryResult Response = new GetOKEXTradeHistoryResult();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    //var Result = await client.GetAsync("https://www.okex.com/api/spot/v3/instruments" + instrument_id + "/trades?limit" + limit.ToString() + "&from=" + from.ToString() + "&to=" + to.ToString()).Result.Content.ReadAsStringAsync();
                    var Result = await client.GetAsync("https://www.okex.com/api/spot/v3/instruments/" + instrument_id + "/trades").Result.Content.ReadAsStringAsync();
                    //var Result = "{\"result\":[{\"time\":\"2019-06-17T06:28:34.535Z\",\"timestamp\":\"2019-06-17T06:28:34.535Z\",\"trade_id\":\"1612709497\",\"price\":\"9103.2\",\"size\":\"0.0373\",\"side\":\"buy\"}]}";
                    if (!Result.Contains("<!DOCTYPE html>") && Result != null)
                    {

                        if (!Result.ToLower().Contains("code") && !Result.ToLower().Contains("message"))
                        {
                            Result = "{\"result\":" + Result + "}";
                            Response = JsonConvert.DeserializeObject<GetOKEXTradeHistoryResult>(Result);
                        }
                        else
                        {
                            HelperForLog.WriteLogIntoFile("getTradesAasync", ControllerName, "API Response : " + Result);
                            Response = JsonConvert.DeserializeObject<GetOKEXTradeHistoryResult>(Result);
                        }
                    }
                    else
                    {
                        HelperForLog.WriteLogIntoFile("getTradesAasync", ControllerName, "API Response : " + Result);
                        Response = null;
                    }
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteLogIntoFile("getTradesAasync", ControllerName, "API Response : " + ex.Message);
                    Response = null;
                }
            }
            return Response;
        }
        #endregion

        #region Private API for Place an Order


        public async static Task<OKExPlaceOrderReturn> makeOrderAsync(string instrument_id, string OrderSide, decimal price, decimal size, int leverage, string client_oid, string match_price, string Ordertype)
        {
            OKExPlaceOrderReturn response = new OKExPlaceOrderReturn();
            try
            {
                //instrument_id = "BTC_USDT";
                OKEXGlobalSettings.API_Key = "39a840cd-63ed-426b-8d7a-572325414ffe";
                OKEXGlobalSettings.Secret = "D357D1DDC20952B6B540E93F20C6BFB5";
                OKEXGlobalSettings.PassPhrase = "paRo@1$##";
                string BASEURL = "https://www.okex.com/";
                string SPOT_SEGMENT = "api/spot/v3";
                var url = $"{BASEURL}{SPOT_SEGMENT}/orders";

                OKEXPlaceOrderRequest request = new OKEXPlaceOrderRequest();
                request.client_oid = client_oid;
                request.instrument_id = instrument_id;
                request.margin_trading = 1;
                request.notional = price.ToString();
                request.side = OrderSide;
                request.size = size.ToString();
                request.type = Ordertype;
                request.price = price.ToString();

                //var bodyStr = "{\"notional\":\"121544\",\"client_oid\":null,\"type\":\"market\",\"side\":\"buy\",\"instrument_id\":\"BTC_USDT\",\"margin_trading\":1,\"size\":\"0.007\"}";
                var bodyStr = JsonConvert.SerializeObject(request);
                using (var client = new HttpClient(new HttpInterceptor(OKEXGlobalSettings.API_Key, OKEXGlobalSettings.Secret, OKEXGlobalSettings.PassPhrase, bodyStr)))
                {
                    var res = await client.PostAsync(url, new StringContent(bodyStr, Encoding.UTF8, "application/json"));
                    var contentStr = await res.Content.ReadAsStringAsync();
                    //var jObject = JObject.Parse(contentStr);
                    if (!contentStr.ToLower().Contains("error_code") && !contentStr.ToLower().Contains("error_message"))
                    {
                        contentStr = "{\"Data\":" + contentStr + "}";
                    }
                    else
                    {
                        HelperForLog.WriteLogIntoFile("makeOrderAsync", ControllerName, "API Response : " + contentStr);
                    }
                    response = JsonConvert.DeserializeObject<OKExPlaceOrderReturn>(contentStr);
                    return response;
                }

            }
            catch (Exception e)
            {
                response = null;
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, ControllerName, e);
            };
            return response;
        }
        #endregion

        #region Private API for GetOrderInformation
        public async static Task<OKExGetOrderInfoReturn> getOrdersAsync(string instrument_id, string order_id, string client_oid)
        {
            OKExGetOrderInfoReturn response = new OKExGetOrderInfoReturn();
            try
            {
                string BASEURL = "https://www.okex.com/";
                string SPOT_SEGMENT = "api/spot/v3";
                var url = $"{BASEURL}{SPOT_SEGMENT}/orders/{order_id}";
                
                using (var client = new HttpClient(new HttpInterceptor(OKEXGlobalSettings.API_Key, OKEXGlobalSettings.Secret, OKEXGlobalSettings.PassPhrase, null)))
                {
                    var queryParams = new Dictionary<string, string>();
                    queryParams.Add("instrument_id", instrument_id);
                    var encodedContent = new FormUrlEncodedContent(queryParams);                    
                    var paramsStr = await encodedContent.ReadAsStringAsync();
                    var res = await client.GetAsync($"{url}?{paramsStr}");
                    var contentStr = await res.Content.ReadAsStringAsync();                    
                    if (!contentStr.ToLower().Contains("code") && !contentStr.ToLower().Contains("message"))
                    {
                        contentStr = "{\"Data\":" + contentStr + "}";
                    }
                    else
                    {
                        HelperForLog.WriteLogIntoFile("getOrdersAsync", ControllerName, "API Response : " + contentStr);
                    }
                    response = JsonConvert.DeserializeObject<OKExGetOrderInfoReturn>(contentStr);
                }
            }
            catch (Exception ex)
            {
                response = null;
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, "", ex);
            }
            return response;
        }

        #endregion

        #region Private API for GetAllOpenOrder
        public async static Task<OKExGetAllOpenOrderReturn> getPendingOrdersAsync(string instrument_id, int? from, int? to, int? limit)
        {
            OKExGetAllOpenOrderReturn response = new OKExGetAllOpenOrderReturn();
            try
            {
                var url = $"{"https://www.okex.com/"}{"api/futures/v3"}/cancel_order/{instrument_id}/{from}";
                using (var client = new HttpClient(new HttpInterceptor(OKEXGlobalSettings.API_Key, OKEXGlobalSettings.Secret, OKEXGlobalSettings.PassPhrase, null)))
                {
                    var queryParams = new Dictionary<string, string>();
                    queryParams.Add("instrument_id", instrument_id);
                    if (from.HasValue)
                    {
                        queryParams.Add("from", from.Value.ToString());
                    }
                    if (to.HasValue)
                    {
                        queryParams.Add("to", to.Value.ToString());
                    }
                    if (limit.HasValue)
                    {
                        queryParams.Add("limit", limit.Value.ToString());
                    }
                    var encodedContent = new FormUrlEncodedContent(queryParams);
                    var paramsStr = await encodedContent.ReadAsStringAsync();
                    var res = await client.GetAsync($"{url}?{paramsStr}");
                    var contentStr = await res.Content.ReadAsStringAsync();
                    if (!contentStr.ToLower().Contains("code") && !contentStr.ToLower().Contains("message"))
                    {
                        contentStr = "{\"Data\":" + contentStr + "}";
                    }
                    else
                    {
                        HelperForLog.WriteLogIntoFile("getWalletInfoAsync", ControllerName, "API Response : " + contentStr);
                    }
                    response = JsonConvert.DeserializeObject<OKExGetAllOpenOrderReturn>(contentStr);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, "", ex);
                response = null;
            }
            return response;
        }
        #endregion

        #region Private API for GetWalletBalance(Funding Account Information)
        //public async static Task<OKExGetWalletBalanceReturn> getWalletInfoAsync()
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            var uri =  new Uri("https://www.okex.com/api/account/v3/wallet");
        //            string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        //            //JObject post_params = new JObject();
        //            //string serializedParms = JsonConvert.SerializeObject(post_params);
        //            //string signature = GetSignature(uri, nonce).Result;
        //            //string authenticationString = "Basic " + GlobalSettings.API_Key + ":" + signature + ":" + nonce;
        //            //client.DefaultRequestHeaders.Add("Authentication", authenticationString);
        //            //string result = await client.PostAsync(uri, null).Result.Content.ReadAsStringAsync();
        //            JObject post_params = new JObject();
        //            //post_params.Add("Currency", "XMR"); //not needed for getbalances, but I left it in here to show how post params would be used
        //            string serializedParms = JsonConvert.SerializeObject(post_params);
        //            string signature = CreateSignature(uri, serializedParms, nonce);
        //            var content = CreateHttpContent(serializedParms);
        //            var request = CreateHttpRequestMessage(uri, signature, content, nonce);
        //            //var result = await client.SendAsync(request).Result.Content.ReadAsStringAsync();   
        //            var result ="{\"available\":37.11827078,\"balance\":37.11827078,\"currency\":\"EOS\",\"hold\":\"0\"}";
        //            var response = JsonConvert.DeserializeObject<OKExGetWalletBalanceReturn>(result);
        //            return response;
        //        }
        //        catch (Exception e) { throw e; };
        //    }
        //}

        //public async static Task<OKExGetWalletBalanceReturn> getWalletInfoAsync()
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            var uri = new Uri("https://www.okex.com/api/account/v3/wallet");
        //            string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        //            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.okex.com/api/general/v3/time");
        //            string responseFromServer = "";
        //            httpWebRequest.Method = "GET";
        //            HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
        //            using (StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream()))
        //            {
        //                responseFromServer = sr.ReadToEnd();
        //                sr.Close();
        //                sr.Dispose();
        //            }
        //            httpWebResponse.Close();
        //            if (!String.IsNullOrEmpty(responseFromServer))
        //            {
        //                var data = JsonConvert.DeserializeObject<objRootObject>(responseFromServer);
        //                nonce = data.epoch;
        //            }
        //            //JObject post_params = new JObject();
        //            //string serializedParms = JsonConvert.SerializeObject(post_params);
        //            //string signature = GetSignature(uri, nonce).Result;
        //            //string authenticationString = "Basic " + GlobalSettings.API_Key + ":" + signature + ":" + nonce;
        //            //client.DefaultRequestHeaders.Add("Authentication", authenticationString);
        //            //string result = await client.PostAsync(uri, null).Result.Content.ReadAsStringAsync();
        //            JObject post_params = new JObject();
        //            //post_params.Add("Currency", "XMR"); //not needed for getbalances, but I left it in here to show how post params would be used                    
        //            string serializedParms = JsonConvert.SerializeObject(post_params);
        //            string signature = CreateSignature(uri,"", nonce, "GET");
        //            //string signature = CreateSignature(uri, serializedParms, nonce, "GET");
        //            //string signature = "Nwg8+NaDZjU7euoeNNKgy+VNAV/h5gk+J1X38wBasmI=";
        //            var content = CreateHttpContent(serializedParms);
        //            var request = CreateHttpRequestMessage(uri, signature, content, nonce);
        //            var result = await client.SendAsync(request).Result.Content.ReadAsStringAsync();
        //            //var result ="{\"available\":37.11827078,\"balance\":37.11827078,\"currency\":\"EOS\",\"hold\":\"0\"}";
        //            var response = JsonConvert.DeserializeObject<OKExGetWalletBalanceReturn>(result);
        //            return response;
        //        }
        //        catch (Exception e) { throw e; };
        //    }
        //}

        public async static Task<OKEBalanceResult> getWalletInfoAsync()
        {
            OKEBalanceResult response = new OKEBalanceResult();
            try
            {
                var url = $"{"https://www.okex.com/"}{"api/account/v3"}/wallet";
                using (var client = new HttpClient(new HttpInterceptor(OKEXGlobalSettings.API_Key, OKEXGlobalSettings.Secret, OKEXGlobalSettings.PassPhrase, null)))
                {
                    var res = await client.GetAsync(url);
                    var contentStr = await res.Content.ReadAsStringAsync();
                    if (!contentStr.ToLower().Contains("code") && !contentStr.ToLower().Contains("message"))
                    {
                        contentStr = "{\"Data\":" + contentStr + "}";
                    }
                    else
                    {
                        HelperForLog.WriteLogIntoFile("getWalletInfoAsync", ControllerName, "API Response : " + contentStr);
                    }
                    response = JsonConvert.DeserializeObject<OKEBalanceResult>(contentStr);
                    return response;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("getWalletInfoAsync", ControllerName, ex);
                response = null;
            }
            return response;
        }

        #endregion

        #region Private API for GetWithdrawlFee (Trade Fee)
        //public async static Task<OKExGetWithdrawalFeeReturn> getWithDrawalFeeAsync(string currency)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            var uri = new Uri("https://www.okex.com/api/account/v3/withdrawal/fee");
        //            string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        //            JObject post_params = new JObject();
        //            post_params.Add("currency", currency);
        //            string serializedParms = JsonConvert.SerializeObject(post_params);
        //            string signature = CreateSignature(uri, serializedParms, nonce, "GET");
        //            var content = CreateHttpContent(serializedParms);
        //            var request = CreateHttpRequestMessage(uri, signature, content, nonce);
        //            var result = await client.SendAsync(request).Result.Content.ReadAsStringAsync();
        //            var response = JsonConvert.DeserializeObject<OKExGetWithdrawalFeeReturn>(result);
        //            return response;
        //        }
        //        catch (Exception e) { throw e; };
        //    }
        //}

        public async static Task<OKExGetWithdrawalFeeReturn> getWithDrawalFeeAsync(string currency)
        {
            OKExGetWithdrawalFeeReturn response = new OKExGetWithdrawalFeeReturn();
            try
            {
                var url = $"{"https://www.okex.com/"}{"api/futures/v3"}/withdrawal/fee";
                using (var client = new HttpClient(new HttpInterceptor(OKEXGlobalSettings.API_Key, OKEXGlobalSettings.Secret, OKEXGlobalSettings.PassPhrase, null)))
                {
                    var queryParams = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(currency))
                    {
                        queryParams.Add("currency", currency);
                    }
                    var encodedContent = new FormUrlEncodedContent(queryParams);
                    var paramsStr = await encodedContent.ReadAsStringAsync();
                    var res = await client.GetAsync($"{url}?{paramsStr}");
                    var contentStr = await res.Content.ReadAsStringAsync();
                    if (!contentStr.ToLower().Contains("code") && !contentStr.ToLower().Contains("message"))
                    {
                        contentStr = "{\"Data\":" + contentStr + "}";
                    }
                    else
                    {
                        HelperForLog.WriteLogIntoFile("getWalletInfoAsync", ControllerName, "API Response : " + contentStr);
                    }
                    response = JsonConvert.DeserializeObject<OKExGetWithdrawalFeeReturn>(contentStr);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, "", ex);
                response = null;
            }
            return response;
        }
        #endregion

        #region public API for GetExchangeInformation
        public async static Task<OKExGetExchangeRateInfoReturn> getRateAsync()
        {
            OKExGetExchangeRateInfoReturn Response = new OKExGetExchangeRateInfoReturn();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    //var Result = await client.GetAsync("https://www.okex.com/api/futures/v3/rate").Result.Content.ReadAsStringAsync();
                    var Result = await client.GetAsync("https://www.okex.com/api/spot/v3/instruments/ticker").Result.Content.ReadAsStringAsync();
                    //var Result = "[  {    \"instrument_id\": \"ETH-USD-190705\",    \"last\": \"311.836\",    \"best_bid\": \"311.817\",    \"best_ask\": \"311.818\",    \"high_24h\": \"329.464\",    \"low_24h\": \"302.001\",    \"volume_24h\": \"3697356\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"LTC-BTC-190705\",    \"last\": \"0.4702\",    \"best_bid\": \"0.4699\",    \"best_ask\": \"0.4702\",    \"high_24h\": \"0.5\",    \"low_24h\": \"0.4532\",    \"volume_24h\": \"1018297\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"BCH-USD-190927\",    \"last\": \"500.66\",    \"best_bid\": \"500.65\",    \"best_ask\": \"500.66\",    \"high_24h\": \"534.53\",    \"low_24h\": \"489.2\",    \"volume_24h\": \"16572683\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"EOS-USD-190927\",    \"last\": \"7.602\",    \"best_bid\": \"7.602\",    \"best_ask\": \"7.603\",    \"high_24h\": \"8.135\",    \"low_24h\": \"7.42\",    \"volume_24h\": \"50257095\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"BSV-USD-190628\",    \"last\": \"240.47\",    \"best_bid\": \"240.32\",    \"best_ask\": \"240.46\",    \"high_24h\": \"248.56\",    \"low_24h\": \"230.81\",    \"volume_24h\": \"779352\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"TRX-USD-190705\",    \"last\": \"0.03944\",    \"best_bid\": \"0.03934\",    \"best_ask\": \"0.03959\",    \"high_24h\": \"0.03999\",    \"low_24h\": \"0.03669\",    \"volume_24h\": \"68925\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"EOS-USD-190628\",    \"last\": \"7.215\",    \"best_bid\": \"7.214\",    \"best_ask\": \"7.215\",    \"high_24h\": \"7.619\",    \"low_24h\": \"7.044\",    \"volume_24h\": \"10595465\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"XRP-USD-190927\",    \"last\": \"0.4894\",    \"best_bid\": \"0.4894\",    \"best_ask\": \"0.4895\",    \"high_24h\": \"0.5241\",    \"low_24h\": \"0.4681\",    \"volume_24h\": \"12888696\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"BCH-USD-190705\",    \"last\": \"480.43\",    \"best_bid\": \"480.17\",    \"best_ask\": \"480.35\",    \"high_24h\": \"506.59\",    \"low_24h\": \"467.96\",    \"volume_24h\": \"982301\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"LTC-USD-190927\",    \"last\": \"142.709\",    \"best_bid\": \"142.707\",    \"best_ask\": \"142.708\",    \"high_24h\": \"152.411\",    \"low_24h\": \"141.11\",    \"volume_24h\": \"16933277\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"ETC-USD-190628\",    \"last\": \"9.252\",    \"best_bid\": \"9.251\",    \"best_ask\": \"9.252\",    \"high_24h\": \"9.75\",    \"low_24h\": \"8.931\",    \"volume_24h\": \"1576339\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"EOS-USD-190705\",    \"last\": \"7.302\",    \"best_bid\": \"7.299\",    \"best_ask\": \"7.3\",    \"high_24h\": \"7.719\",    \"low_24h\": \"7.138\",    \"volume_24h\": \"2351388\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"TRX-USD-190927\",    \"last\": \"0.04187\",    \"best_bid\": \"0.04185\",    \"best_ask\": \"0.04187\",    \"high_24h\": \"0.04312\",    \"low_24h\": \"0.03913\",    \"volume_24h\": \"2300537\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"ETC-USD-190927\",    \"last\": \"9.694\",    \"best_bid\": \"9.693\",    \"best_ask\": \"9.697\",    \"high_24h\": \"10.336\",    \"low_24h\": \"9.408\",    \"volume_24h\": \"3420211\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"BSV-USD-190705\",    \"last\": \"242.45\",    \"best_bid\": \"241.96\",    \"best_ask\": \"242.45\",    \"high_24h\": \"251.11\",    \"low_24h\": \"231.86\",    \"volume_24h\": \"270339\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"TRX-USD-190628\",    \"last\": \"0.03901\",    \"best_bid\": \"0.03901\",    \"best_ask\": \"0.03906\",    \"high_24h\": \"0.03947\",    \"low_24h\": \"0.03653\",    \"volume_24h\": \"665006\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"ETC-USD-190705\",    \"last\": \"9.384\",    \"best_bid\": \"9.385\",    \"best_ask\": \"9.391\",    \"high_24h\": \"9.908\",    \"low_24h\": \"9.07\",    \"volume_24h\": \"408032\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"XRP-USD-190628\",    \"last\": \"0.4653\",    \"best_bid\": \"0.4652\",    \"best_ask\": \"0.4653\",    \"high_24h\": \"0.4921\",    \"low_24h\": \"0.448\",    \"volume_24h\": \"3371408\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"LTC-USD-190628\",    \"last\": \"136.365\",    \"best_bid\": \"136.373\",    \"best_ask\": \"136.417\",    \"high_24h\": \"143.255\",    \"low_24h\": \"134.662\",    \"volume_24h\": \"5113848\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"LTC-USD-190705\",    \"last\": \"138.014\",    \"best_bid\": \"138.081\",    \"best_ask\": \"138.088\",    \"high_24h\": \"144.896\",    \"low_24h\": \"135.972\",    \"volume_24h\": \"968047\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"BSV-USD-190927\",    \"last\": \"258.68\",    \"best_bid\": \"258.62\",    \"best_ask\": \"258.92\",    \"high_24h\": \"269.32\",    \"low_24h\": \"247.35\",    \"volume_24h\": \"2857150\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"ETH-USD-190628\",    \"last\": \"308.904\",    \"best_bid\": \"308.915\",    \"best_ask\": \"308.943\",    \"high_24h\": \"325.28\",    \"low_24h\": \"298.666\",    \"volume_24h\": \"14391934\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"BTC-USD-190628\",    \"last\": \"10866.15\",    \"best_bid\": \"10866.15\",    \"best_ask\": \"10866.16\",    \"high_24h\": \"11452.07\",    \"low_24h\": \"10639.65\",    \"volume_24h\": \"3996221\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"INR-BTC-190927\",    \"last\": \"323.873\",    \"best_bid\": \"323.87\",    \"best_ask\": \"323.871\",    \"high_24h\": \"346.747\",    \"low_24h\": \"311.5\",    \"volume_24h\": \"55463264\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"BCH-USD-190628\",    \"last\": \"475.21\",    \"best_bid\": \"475.17\",    \"best_ask\": \"475.3\",    \"high_24h\": \"500.75\",    \"low_24h\": \"462\",    \"volume_24h\": \"4331754\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"BTC-USD-190705\",    \"last\": \"10965.78\",    \"best_bid\": \"10965.88\",    \"best_ask\": \"10970.97\",    \"high_24h\": \"11552.98\",    \"low_24h\": \"10722.84\",    \"volume_24h\": \"873278\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  },  {    \"instrument_id\": \"BTC-USD-190927\",    \"last\": \"11335.64\",    \"best_bid\": \"11335.64\",    \"best_ask\": \"11335.65\",    \"high_24h\": \"12037.48\",    \"low_24h\": \"11107\",    \"volume_24h\": \"13412003\",    \"timestamp\": \"2019-06-24T07:48:02.616Z\",    \"open_24h\": null,    \"open_interest\": null  }]";
                    if (!Result.Contains("<!DOCTYPE html>") && Result != null)
                    {
                        if (!Result.ToLower().Contains("code") && !Result.ToLower().Contains("message"))
                        {
                            Result = "{\"Data\":" + Result + "}";

                        }
                        else
                        {
                            HelperForLog.WriteLogIntoFile("getRateAsync", ControllerName, "API Response : " + Result);
                        }
                        Response = JsonConvert.DeserializeObject<OKExGetExchangeRateInfoReturn>(Result);
                    }
                    else
                    {
                        HelperForLog.WriteLogIntoFile("getRateAsync", ControllerName, "API Response : " + Result);
                        Response = null;
                    }
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteLogIntoFile("getRateAsync", ControllerName, "API Response : " + ex);
                    Response = null;
                }
                return Response;
            }
        }
        #endregion

        #region Public API For LTC - Token Pair Detail
        public async static Task<OKExGetTokenPairDetailReturn> getTokenPairDetailAsycn()
        {
            OKExGetTokenPairDetailReturn Response = new OKExGetTokenPairDetailReturn();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var Result = await client.GetAsync("https://www.okex.com/api/spot/v3/instruments").Result.Content.ReadAsStringAsync();
                    if (!Result.Contains("<!DOCTYPE html>") && Result != null)
                    {
                        try
                        {
                            if (!Result.ToLower().Contains("code") && !Result.ToLower().Contains("message"))
                            {
                                Response = JsonConvert.DeserializeObject<OKExGetTokenPairDetailReturn>(Result);
                            }
                            else
                            {
                                HelperForLog.WriteLogIntoFile("getRateAsync", ControllerName, "API Response : " + Result);
                                Response = JsonConvert.DeserializeObject<OKExGetTokenPairDetailReturn>(Result);
                            }
                        }
                        catch (Exception e)
                        {
                            HelperForLog.WriteLogIntoFile("getRateAsync", ControllerName, "API Response : " + Result);
                            Response = null;
                        }

                    }
                    else
                    {
                        HelperForLog.WriteLogIntoFile("getRateAsync", ControllerName, "API Response : " + Result);
                        Response = null;
                    }
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteLogIntoFile("getRateAsync", ControllerName, "API Response : " + ex);
                    Response = null;
                }
            }
            return Response;
        }
        #endregion

        #region OLd Code
        //public static string CreateSignature(Uri uri, string parameters, string nonce)
        //{

        //    //SIGNATURE: API_KEY + "POST" + URI + NONCE + POST_PARAMS(signed by secret key according to HMAC - SHA512 method.)
        //    string endpoint = WebUtility.UrlEncode(uri.ToString()).ToLower();
        //    parameters = Convert.ToBase64String(Encoding.UTF8.GetBytes(parameters ?? ""));
        //    var signature = $"{OKEXGlobalSettings.API_Key }POST{endpoint}{nonce}{parameters}";
        //    using (var hashAlgo = new HMACSHA512(Convert.FromBase64String(OKEXGlobalSettings.Secret)))
        //    {
        //        var signedBytes = hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(signature));
        //        return Convert.ToBase64String(signedBytes);
        //    }
        //}

        //public static HttpContent CreateHttpContent(string jsonParams)
        //{
        //    return new StringContent(jsonParams ?? "", Encoding.UTF8, "application/json");
        //}

        //public static HttpRequestMessage CreateHttpRequestMessage(Uri uri, string signature, HttpContent content, string nonce)
        //{
        //    var header = $"Basic {OKEXGlobalSettings.API_Key }:{signature}:{nonce}";
        //    var message = new HttpRequestMessage(HttpMethod.Post, uri);
        //    message.Headers.Add("Authorization", header);
        //    message.Content = content;
        //    return message;
        //}

        //private static Task<string> GetSignature(string uri, string nonce, string post_params = null)
        //{
        //    string signature = "";
        //    ASCIIEncoding encoding = new ASCIIEncoding();
        //    if (post_params != null)
        //    {
        //        post_params = Convert.ToBase64String(encoding.GetBytes(post_params));
        //        signature = OKEXGlobalSettings.API_Key + "POST" + uri + nonce + post_params;
        //    }
        //    else
        //    {
        //        signature = OKEXGlobalSettings.API_Key + "POST" + uri + nonce;
        //    }
        //    byte[] messageBytes = encoding.GetBytes(signature);
        //    using (HMACSHA512 _object = new HMACSHA512(OKEXGlobalSettings.Secret_Key))
        //    {
        //        byte[] hashmessage = _object.ComputeHash(messageBytes);
        //        return Task.FromResult(Convert.ToBase64String(hashmessage));
        //    }
        //}

        //public static string CreateSignature(Uri uri, string parameters, string nonce)
        //{

        //    //SIGNATURE: API_KEY + "POST" + URI + NONCE + POST_PARAMS(signed by secret key according to HMAC - SHA512 method.)
        //    string endpoint = WebUtility.UrlEncode(uri.ToString()).ToLower();
        //    parameters = Convert.ToBase64String(Encoding.UTF8.GetBytes(parameters ?? ""));
        //    var signature = $"{OKEXGlobalSettings.API_Key }POST{endpoint}{nonce}{parameters}";
        //    using (var hashAlgo = new HMACSHA512(Convert.FromBase64String(OKEXGlobalSettings.Secret)))
        //    {
        //        var signedBytes = hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(signature));
        //        return Convert.ToBase64String(signedBytes);
        //    }
        //}


        //public async static Task<JContainer> getWalletInfoAsync()
        //{

        //    var url = $"{"https://www.okex.com/"}{"api/account/v3"}/wallet";
        //    using (var client = new HttpClient(new HttpInterceptor(OKEXGlobalSettings.API_Key, OKEXGlobalSettings.Secret, OKEXGlobalSettings.PassPhrase, null)))
        //    {
        //        var res = await client.GetAsync(url);
        //        var contentStr = await res.Content.ReadAsStringAsync();
        //        if (contentStr[0] == '[')
        //        {
        //            return JArray.Parse(contentStr);
        //        }
        //        return JObject.Parse(contentStr);
        //    }
        //}

        #endregion

        public static string CreateSignature(Uri uri, string parameters, string nonce, string MethodType)
        {
            try
            {
                //sign = HmacSHA256Base64Utils.sign(timestamp, method(request), requestPath(request),
                // queryString(request), body(request), this.credentials.getSecretKey());
                //SIGNATURE: API_KEY + "POST" + URI + NONCE + POST_PARAMS(signed by secret key according to HMAC - SHA512 method.)
                string url = uri.ToString();
                url = url.Replace("https://www.okex.com/api", "");
                string endpoint = WebUtility.UrlEncode(uri.ToString()).ToLower();
                parameters = Convert.ToBase64String(Encoding.UTF8.GetBytes(parameters ?? ""));
                var signature = $"{nonce}{MethodType}{url}{parameters}{OKEXGlobalSettings.Secret }";
                //byte[] key = Encoding.ASCII.GetBytes(signature);
                //var hashAlgo = new HMACSHA256(Encoding.ASCII.GetBytes(Convert.ToBase64String(key)));
                //var signedBytes = hashAlgo.ComputeHash(key);
                //return Convert.ToBase64String(signedBytes);
                string sign = "";
                if (!String.IsNullOrEmpty(parameters))
                {
                    sign = Encryptor.HmacSHA256($"{nonce}{MethodType}{url}{parameters}", OKEXGlobalSettings.Secret);
                }
                else
                {
                    sign = Encryptor.HmacSHA256($"{nonce}{MethodType}{url}", OKEXGlobalSettings.Secret);
                }
                return sign;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateSignature", ControllerName, ex);
                return null;
            }
        }

        public static HttpContent CreateHttpContent(string jsonParams)
        {
            return new StringContent(jsonParams ?? "", Encoding.UTF8, "application/json");
        }

        //public static HttpRequestMessage CreateHttpRequestMessage(Uri uri, string signature, HttpContent content, string nonce)
        //{
        //    var header = $"Basic {OKEXGlobalSettings.API_Key }:{signature}:{nonce}";
        //    var message = new HttpRequestMessage(HttpMethod.Post, uri);
        //    message.Headers.Add("Authorization", header);
        //    message.Content = content;
        //    return message;
        //}

      
    }

    class HttpInterceptor : DelegatingHandler
    {
        public static string ControllerName = "LiquidityProvider.OKExAPI.APICall";
        private string _apiKey;
        private string _passPhrase;
        private string _secret;
        private string _bodyStr;
        public HttpInterceptor(string apiKey, string secret, string passPhrase, string bodyStr)
        {
            this._apiKey = apiKey;
            this._passPhrase = passPhrase;
            this._secret = secret;
            this._bodyStr = bodyStr;
            InnerHandler = new HttpClientHandler();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var method = request.Method.Method;
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("OK-ACCESS-KEY", this._apiKey);

                var now = DateTime.Now;
                var timeStamp = TimeZoneInfo.ConvertTimeToUtc(now).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                var requestUrl = request.RequestUri.PathAndQuery;
                string sign = "";
                if (!String.IsNullOrEmpty(this._bodyStr))
                {
                    sign = Encryptor.HmacSHA256($"{timeStamp}{method}{requestUrl}{this._bodyStr}", this._secret);
                }
                else
                {
                    sign = Encryptor.HmacSHA256($"{timeStamp}{method}{requestUrl}", this._secret);
                }

                request.Headers.Add("OK-ACCESS-SIGN", sign);
                request.Headers.Add("OK-ACCESS-TIMESTAMP", timeStamp.ToString());
                request.Headers.Add("OK-ACCESS-PASSPHRASE", this._passPhrase);

                return base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SendAsync", ControllerName, ex);
                return null;
            }
        }
    }
}
