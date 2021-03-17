using CCXT.NET.Kraken;
using Worldex.Core.Interfaces.LiquidityProvider;
using OdinSdk.BaseLib.Coin;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Worldex.Core.Helpers;
using Newtonsoft.Json;
using CCXT.NET.Kraken.Public;
using CCXT.NET.Kraken.Trade;
using Worldex.Core.ViewModels.LiquidityProvider;
using System.Security.Cryptography;
using RestSharp;
using System.Net;
using System.Linq;
using CCXT.NET.Kraken.Private;

namespace Worldex.Infrastructure.LiquidityProvider.KrakenAPI
{

    public class KrakenLPService : IKrakenLPService
    {
        string responseFromServer;
        public static string ControllerName = "KrakenLPService";

        public KrakenClient _client = new KrakenClient("", KrakenGlobalSettings.API_Key, KrakenGlobalSettings.Secret);

        public async Task<KrakenOrderBookResponse> GetOrderBook(string Market)
        {
            //var _public_api = new CCXT.NET.Korbit.Public.PublicApi();
            try
            {
                KrakenOrderBookResponse Resp = new KrakenOrderBookResponse();
                string url = "https://api.kraken.com/0/public/Depth?pair=##";
                Market = Market.Contains("-") ? Market.Replace("-", "") : Market.Contains("_") ? Market.Replace("_", "") : Market;
                url = url.Replace("###", Market);

                var Result = await _client.CallApiGet1Async(url);
                if (Result.Response.IsSuccessful)
                {
                    var data = Result.Response.Content;
                    data = data.Replace(Market, "Data");
                    if (!String.IsNullOrEmpty(data))
                    {
                        //var RespData = JsonConvert.DeserializeObject<KOrderBook>(data);
                        Resp = JsonConvert.DeserializeObject<KrakenOrderBookResponse>(data);
                    }
                    if (Resp.error.Count > 0)
                    {
                        HelperForLog.WriteLogIntoFile("", ControllerName, "--KrakenAPI-->GetOrderBook :" + Helpers.JsonSerialize(Resp));
                    }
                    return Resp;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetOrderBook", ControllerName, ex);
                return null;
            }
        }

        public async Task<KrakenGetAssetInfoResponse> GetTradableAsset()
        {
            try
            {
                string url = "https://api.kraken.com/0/public/AssetPairs";
                var Result = await _client.CallApiGet1Async(url);
                if (Result.Response.IsSuccessful)
                {
                    var data = Result.Response.Content;
                    return JsonConvert.DeserializeObject<KrakenGetAssetInfoResponse>(data);
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTradableAsset", ControllerName, ex);
                return null;
            }
        }

        public async Task<KrakenGetTradeHistoryResponse> GetTradeHistory(string Market)
        {
            try
            {
                Market = Market.Replace("-", "");
                string url = "https://api.kraken.com/0/public/Trades?pair=###";
                url = url.Replace("###", Market);
                KrakenClient _client = new KrakenClient("", "", "");

                var Result = await _client.CallApiGet1Async(url);
                if (Result.Response.IsSuccessful)
                {
                    var data = Result.Response.Content;
                    data = data.Replace(Market, "Data");
                    var result = JsonConvert.DeserializeObject<KrakenGetTradeHistoryResponse>(data);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTradeHistory", ControllerName, ex);
                return null;
            }
        }

        public async Task<KrakenBalanceResponse> GetBalances()
        {
            try
            {
                //string url = "https://api.kraken.com/0/private/Balance";
                string url = "/0/private/Balance";

                KrakenClient _client = new KrakenClient("private", KrakenGlobalSettings.API_Key, KrakenGlobalSettings.Secret);

                var _params = new Dictionary<string, object>();

                var _request = await _client.CreatePostRequest(url, _params);

                //_params.Add(_request.Parameters[0].Name, _request.Parameters[0].Value);
                _params.Add(_request.Parameters[1].Name, _request.Parameters[1].Value);
                _params.Add(_request.Parameters[2].Name, _request.Parameters[2].Value);
                _params.Add(_request.Parameters[3].Name, _request.Parameters[3].Value);


                var Result = await _client.CallApiPost1Async(url, _params);
                if (Result.Response.IsSuccessful)
                {
                    var data = Result.Response.Content;
                    var res = JsonConvert.DeserializeObject<KrakenBalanceResponse>(data);
                    return res;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetBalances", ControllerName, ex);
                return null;
            }
        }


        public async Task<KMyOrderItem> GetLPStatusCheck(bool trades, string userref, string txid)
        {
            try
            {
                string url = "https://api.kraken.com/0/private/QueryOrders";
                // khushali 13-07-2019 comment because we are using dynamic configuartion
                //KrakenGlobalSettings.API_Key = "6rbVgzroV1Nvdm3zENUHMcsuLcLgXcjnikR5abviXkYiJ4bQi/SVhShS";
                //KrakenGlobalSettings.Secret = "g8ETNZAx1TpjzCBBiRD03oMSa7uKYB1KiRm1Q5N14k0OUNy3PBXiQSI2ou6Rqqg6XPXaC30PRw1x/ql9Wk0yKg==";


                KrakenClient _client = new KrakenClient("", KrakenGlobalSettings.API_Key, KrakenGlobalSettings.Secret);

                var _params = new Dictionary<string, object>();
                {
                    _params.Add("trades", trades);
                    _params.Add("userref", userref);
                    _params.Add("txid", txid);

                    CCXT.NET.Kraken.Private.PrivateApi privateApi = new CCXT.NET.Kraken.Private.PrivateApi(KrakenGlobalSettings.API_Key, KrakenGlobalSettings.Secret, "MYAPIKEY");

                    privateApi.privateClient.MergeParamsAndArgs(_params, null);
                }

                var Result = await _client.CallApiPost1Async(url, _params);
                if (Result.Response.IsSuccessful)
                {
                    var data = Result.Response.Content;
                    //data = data.Replace(Market, "Data");
                    var result = JsonConvert.DeserializeObject<KMyOrderItem>(data);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetLPStatusCheck", ControllerName, ex);
                return null;
            }
        }
        public async Task<KrakenLTPCheckResponse> GetLTPCheck(string Market)
        {
            try
            {
                Market = Market.Replace("-", "");
                string url = "https://api.kraken.com/0/public/Ticker?pair=###";
                url = url.Replace("###", Market);
                KrakenClient _client = new KrakenClient("", "", "");

                var Result = await _client.CallApiGet1Async(url);
                if (Result.Response.IsSuccessful)
                {
                    var data = Result.Response.Content;
                    data = data.Replace(Market, "Data");
                    var result = JsonConvert.DeserializeObject<KrakenLTPCheckResponse>(data);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTradeHistory", ControllerName, ex);
                return null;
            }
        }


        public async Task<KMyCancelOrder> CancelOrderAsync(string txid)
        {
            try
            {
                string url = "/0/private/CancelOrder";
                // khushali 13-07-2019 comment because we are using dynamic configuartion
                //KrakenGlobalSettings.API_Key = "6rbVgzroV1Nvdm3zENUHMcsuLcLgXcjnikR5abviXkYiJ4bQi/SVhShS";
                //KrakenGlobalSettings.Secret = "g8ETNZAx1TpjzCBBiRD03oMSa7uKYB1KiRm1Q5N14k0OUNy3PBXiQSI2ou6Rqqg6XPXaC30PRw1x/ql9Wk0yKg==";
                

                KrakenClient _client = new KrakenClient("private", KrakenGlobalSettings.API_Key, KrakenGlobalSettings.Secret);

                var _params = new Dictionary<string, object>();
                var _request = await _client.CreatePostRequest(url, _params);

                {
                    _params.Add("txid", txid);
                }

                _params.Add(_request.Parameters[1].Name, _request.Parameters[1].Value);
                _params.Add(_request.Parameters[2].Name, _request.Parameters[2].Value);
                _params.Add(_request.Parameters[3].Name, _request.Parameters[3].Value);

                var Result = await _client.CallApiPost1Async(url, _params);
                if (Result.Response.IsSuccessful)
                {
                    var data = Result.Response.Content;
                    //data = data.Replace(Market, "Data");
                    var result = JsonConvert.DeserializeObject<KMyCancelOrder>(data);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetCancelOrder", ControllerName, ex);
                return null;
            }
        }


        public async Task<KrakenPlaceOrderResponse> PlaceOrderAsyn(string market, string type, string orederType, decimal volume, decimal price)
        {
            try
            {
                string url = "0/private/AddOrder";
                // khushali 13-07-2019 comment because we are using dynamic configuartion
                //KrakenGlobalSettings.API_Key = "34WAvvHR7afSnO80kjl65c8k0Pn2CRqn/Rev/2OOI+cctfkKAT0IdMMC";
                //KrakenGlobalSettings.Secret = "oXPcSZTN1A2PLfbJKmUjkqVku5mJr4Y7/TXpV5fRrH8F3H5RGFdv3y/ZotXPYGZeRzM7uFrW1FDTaBQIibdh4A==";

                if (market == "ETH_BTC")
                    market = "XETH-XXBT";

                KrakenClient _client = new KrakenClient("private", KrakenGlobalSettings.API_Key, KrakenGlobalSettings.Secret);
                market = market.Replace("-", "");

                var _params = new Dictionary<string, object>();

                var _request = await _client.CreatePostRequest(url, _params);

                {
                    _params.Add("pair", market);
                    _params.Add("type", type);
                    _params.Add("ordertype", orederType.ToLower());
                    _params.Add("price", price);
                    _params.Add("price2", price);
                    _params.Add("volume", volume);
                    _params.Add("leverage", "none");
                    //_params.Add("oflags", "viqc");
                    _params.Add("starttm", 0);
                    _params.Add("expiretm", 0);
                    _params.Add("userref", 1);
                    _params.Add("validate", true);
                }

                _params.Add(_request.Parameters[1].Name, _request.Parameters[1].Value);
                _params.Add(_request.Parameters[2].Name, _request.Parameters[2].Value);
                _params.Add(_request.Parameters[3].Name, _request.Parameters[3].Value);

                var Result = _client.CallApiPost1Async(url, _params).Result;

                if (Result.Response.IsSuccessful)
                {
                    var data = Result.Response.Content;                    
                    //data = data.Replace("descr", "description");
                    //data = data.Replace(Market, "Data");
                    var result = JsonConvert.DeserializeObject<KrakenPlaceOrderResponse>(data);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("placeoreder ", ControllerName, ex);
                return null;
            }

        }


        private JsonObject QueryPrivate(string a_sMethod, string props = null)
        {
            // generate a 64 bit nonce using a timestamp at tick resolution
            Int64 nonce = DateTime.Now.Ticks;
            props = "nonce=" + nonce + props;


            string path = string.Format("/{0}/private/{1}", 0, a_sMethod);
            string address = _client.ApiUrl + path;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(address);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            webRequest.Headers.Add("API-Key", KrakenGlobalSettings.API_Key);


            byte[] base64DecodedSecred = Convert.FromBase64String(KrakenGlobalSettings.Secret);

            var np = nonce + Convert.ToChar(0) + props;

            var pathBytes = Encoding.UTF8.GetBytes(path);
            var hash256Bytes = sha256_hash(np);
            var z = new byte[pathBytes.Count() + hash256Bytes.Count()];
            pathBytes.CopyTo(z, 0);
            hash256Bytes.CopyTo(z, pathBytes.Count());

            var signature = getHash(base64DecodedSecred, z);

            webRequest.Headers.Add("API-Sign", Convert.ToBase64String(signature));

            if (props != null)
            {

                using (var writer = new StreamWriter(webRequest.GetRequestStream()))
                {
                    writer.Write(props);
                }
            }

            //Make the request
            try
            {
                //Wait for RateGate
                //_rateGate.WaitToProceed();

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (Stream str = webResponse.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            responseFromServer = sr.ReadToEnd();
                            sr.Close();
                            sr.Dispose();
                            return (JsonObject)JsonConvert.DeserializeObject(responseFromServer);
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            if (response.StatusCode != HttpStatusCode.InternalServerError)
                            {
                                throw;
                            }
                            return null;
                            //return (JsonObject)JsonConvert.Import(sr);
                        }
                    }
                }

            }
        }

        private byte[] sha256_hash(String value)
        {
            using (SHA512 hash = SHA512Managed.Create())
            {
                Encoding enc = Encoding.UTF8;

                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                return result;
            }
        }

        private byte[] getHash(byte[] keyByte, byte[] messageBytes)
        {
            using (var hmacsha512 = new HMACSHA512(keyByte))
            {

                Byte[] result = hmacsha512.ComputeHash(messageBytes);

                return result;

            }
        }
    }
}
