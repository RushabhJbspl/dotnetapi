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

namespace Worldex.Infrastructure.LiquidityProvider.TradeSatoshiAPI
{
    public class APICall
    {
        #region Public API
        //private string PUBLIC_API_KEY = GlobalSettings.API_Key;
        //private string PRIVATE_API_KEY = GlobalSettings.Secret;
        /// <summary> khushali 11-01-2019 tradesatoshi wrapper  </summary>
        /// <returns><seealso cref="GetCurrenciesReturn"/></returns>
        public async static Task<GetCurrenciesReturn> GetCurrencies()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    return JsonConvert.DeserializeObject<GetCurrenciesReturn>(await client.GetAsync("https://tradesatoshi.com/api/public/getcurrencies").Result.Content.ReadAsStringAsync());
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary> khushali 11-01-2019 tradesatoshi wrapper </summary>
        /// <param name="market">market: The market name e.g. 'LTC_BTC' (required)</param>
        /// <returns><seealso cref="GetTickerReturn"/></returns>
        public async static Task<GetTickerReturn> GetTicker(string market)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    return JsonConvert.DeserializeObject<GetTickerReturn>(await client.GetAsync("https://tradesatoshi.com/api/public/getticker?market=" + market).Result.Content.ReadAsStringAsync());
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary> khushali 11-01-2019 tradesatoshi wrapper </summary>
        /// <param name="market">The market name e.g. 'LTC_BTC' (required)</param>
        /// <param name="count">The max amount of records to return (optional, default: 20)</param>
        /// <returns><seealso cref="GetMarketHistoryReturn"/></returns>
        public async static Task<GetMarketHistoryReturn> GetMarketHistory(string market, int? count = null)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    return JsonConvert.DeserializeObject<GetMarketHistoryReturn>(await client.GetAsync("https://tradesatoshi.com/api/public/getmarkethistory?market=" + market + "&count=" + count.ToString()).Result.Content.ReadAsStringAsync());
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary> khushali 11-01-2019 tradesatoshi wrapper </summary>
        /// <param name="market">The market name e.g. 'LTC_BTC' (required)</param>
        /// <returns><seealso cref="GetMarketSummaryReturn"/></returns>
        public async static Task<GetMarketSummaryReturn> GetMarketSummary(string market)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    return JsonConvert.DeserializeObject<GetMarketSummaryReturn>(await client.GetAsync("https://tradesatoshi.com/api/public/getmarketsummary?market=" + market).Result.Content.ReadAsStringAsync());
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary>khushali 11-01-2019 tradesatoshi wrapper </summary>
        /// <returns><seealso cref="GetMarketSummariesReturn"/></returns>
        public async static Task<GetMarketSummariesReturn> GetMarketSummaries()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    return JsonConvert.DeserializeObject<GetMarketSummariesReturn>(await client.GetAsync("https://tradesatoshi.com/api/public/getmarketsummaries").Result.Content.ReadAsStringAsync());
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary> khushali 11-01-2019 tradesatoshi wrapper </summary>
        /// <param name="market">The market name e.g. 'LTC_BTC' (required)</param>
        /// <param name="type">The order book type 'buy', 'sell', 'both' (optional, default: 'both')</param>
        /// <param name="depth">Max of records to return (optional, default: 20)</param>
        /// <returns><seealso cref="GetOrderBookReturn"/></returns>
        public async static Task<GetOrderBookReturn> GetOrderBook(string market, string type = "both", int? depth = null)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var Result = await client.GetAsync("https://tradesatoshi.com/api/public/getorderbook?market=" + market + "&type=" + type + "&depth=" + depth.ToString()).Result.Content.ReadAsStringAsync();
                    if(!Result.Contains("<!DOCTYPE html>") && Result != null)
                    {
                        try
                        {
                            var Response = JsonConvert.DeserializeObject<GetOrderBookReturn>(Result);
                            return Response;
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
                    throw e;
                }
            }
        }
        #endregion

        #region Private API

        // khushali 11-01-2019 tradesatoshi wrapper
        public static async Task<GetBalancesReturn> GetBalance(string Currency)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var uri = new Uri("https://tradesatoshi.com/api/private/getbalance");
                    string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    JObject post_params = new JObject();
                    post_params.Add("Currency", Currency.ToUpper()); //not needed for getbalances, but I left it in here to show how post params would be used
                    string serializedParms = JsonConvert.SerializeObject(post_params);
                    string signature = CreateSignature(uri, serializedParms, nonce);
                    var content = CreateHttpContent(serializedParms);
                    var request = CreateHttpRequestMessage(uri, signature, content, nonce);
                    var result = await client.SendAsync(request).Result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<GetBalancesReturn>(result);
                    // var balance = ((IList<BalanceInfo>)response.Data).Single(x => x.Currency == "BTC");
                    //System.Console.WriteLine(balance);
                    return response;
                }
                catch (Exception e) { throw e; };
            }
        }     



        public static async Task<SubmitOrderReturn> PlaceOrderAsync(OrderSide side, string market, decimal quantity, decimal rate)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var uri = new Uri("https://tradesatoshi.com/api/private/submitorder");
                    string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    JObject post_params = new JObject();
                    post_params.Add("Market", market);
                    if(OrderSide.Sell == side)
                    {
                        post_params.Add("Type", "sell"); 
                    }
                    else
                    {
                        post_params.Add("Type", "buy");
                    }
                    post_params.Add("Amount", quantity);
                    post_params.Add("Price", rate);
                    string serializedParms = JsonConvert.SerializeObject(post_params);
                    string signature = CreateSignature(uri, serializedParms, nonce);
                    var content = CreateHttpContent(serializedParms);
                    var request = CreateHttpRequestMessage(uri, signature, content, nonce);
                    var result = await client.SendAsync(request).Result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<SubmitOrderReturn>(result);
                    // var balance = ((IList<BalanceInfo>)response.Data).Single(x => x.Currency == "BTC");
                    //System.Console.WriteLine(balance);
                    return response;
                }
                catch (Exception e) { throw e; };
            }
        }


        public static async Task<CancelOrderReturn> CancelOrderAsync(CancelOrderType type, long? orderID = null, string market = "")
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {

                    // Type: The cancel type, options: 'Single','Market','MarketBuys','MarketSells','AllBuys','AllSells','All'(required)
                    //OrderId: The order to cancel(required if cancel type 'Single')
                    //Market: The order to cancel(required if cancel type 'Market','MarketBuys','MarketSells')

                    var uri = new Uri(" https://tradesatoshi.com/api/private/cancelorder");
                    string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    JObject post_params = new JObject();
                    post_params.Add("Type", type.ToString());
                    if (CancelOrderType.Single == type)
                    {
                        post_params.Add("OrderId", orderID);
                    }
                    else if(CancelOrderType.Market == type || CancelOrderType.MarketBuys == type || CancelOrderType.MarketSells == type)
                    {
                        post_params.Add("Market", market);
                    }
                    
                    string serializedParms = JsonConvert.SerializeObject(post_params);
                    string signature = CreateSignature(uri, serializedParms, nonce);
                    var content = CreateHttpContent(serializedParms);
                    var request = CreateHttpRequestMessage(uri, signature, content, nonce);
                    var result = await client.SendAsync(request).Result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<CancelOrderReturn>(result);
                    // var balance = ((IList<BalanceInfo>)response.Data).Single(x => x.Currency == "BTC");
                    //System.Console.WriteLine(balance);
                    return response;
                }
                catch (Exception e) { throw e; };
            }
        }


        public static async Task<GetOrderReturn> GetOrderAsync(long orderID)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {

                    var uri = new Uri("https://tradesatoshi.com/api/private/getorder");
                    string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    JObject post_params = new JObject();
                    post_params.Add("OrderId", orderID);
                    string serializedParms = JsonConvert.SerializeObject(post_params);
                    string signature = CreateSignature(uri, serializedParms, nonce);
                    var content = CreateHttpContent(serializedParms);
                    var request = CreateHttpRequestMessage(uri, signature, content, nonce);
                    var result = await client.SendAsync(request).Result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<GetOrderReturn>(result);
                    // var balance = ((IList<BalanceInfo>)response.Data).Single(x => x.Currency == "BTC");
                    //System.Console.WriteLine(balance);
                    return response;
                }
                catch (Exception e) { throw e; };
            }
        }

        public static async Task<GetOrdersReturn> GetOrdersAsync(string market, int? limit = 20)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {

                   
                    var uri = new Uri("https://tradesatoshi.com/api/private/getorders");
                    string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    JObject post_params = new JObject();
                    post_params.Add("Market", market);
                    post_params.Add("count", limit);
                    string serializedParms = JsonConvert.SerializeObject(post_params);
                    string signature = CreateSignature(uri, serializedParms, nonce);
                    var content = CreateHttpContent(serializedParms);
                    var request = CreateHttpRequestMessage(uri, signature, content, nonce);
                    var result = await client.SendAsync(request).Result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<GetOrdersReturn>(result);
                    // var balance = ((IList<BalanceInfo>)response.Data).Single(x => x.Currency == "BTC");
                    //System.Console.WriteLine(balance);
                    return response;
                }
                catch (Exception e) { throw e; };
            }
        }

        public static string CreateSignature(Uri uri, string parameters, string nonce)
        {

            //SIGNATURE: API_KEY + "POST" + URI + NONCE + POST_PARAMS(signed by secret key according to HMAC - SHA512 method.)
            string endpoint = WebUtility.UrlEncode(uri.ToString()).ToLower();
            parameters = Convert.ToBase64String(Encoding.UTF8.GetBytes(parameters ?? ""));
            var signature = $"{GlobalSettings.API_Key }POST{endpoint}{nonce}{parameters}";
            using (var hashAlgo = new HMACSHA512(Convert.FromBase64String(GlobalSettings.Secret)))
            {
                var signedBytes = hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(signature));
                return Convert.ToBase64String(signedBytes);
            }
        }

        public static HttpContent CreateHttpContent(string jsonParams)
        {
            return new StringContent(jsonParams ?? "", Encoding.UTF8, "application/json");
        }

        public static HttpRequestMessage CreateHttpRequestMessage(Uri uri, string signature, HttpContent content, string nonce)
        {
            var header = $"Basic {GlobalSettings.API_Key }:{signature}:{nonce}";
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Headers.Add("Authorization", header);
            message.Content = content;
            return message;
        }

        //  khushali 11-01-2019 tradesatoshi wrapper
        public static async Task<GetBalanceReturn> GetBalances()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var uri = new Uri("https://tradesatoshi.com/api/private/getbalance");
                    string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    JObject post_params = new JObject();
                    post_params.Add("Currency", ""); //not needed for getbalances, but I left it in here to show how post params would be used
                    string serializedParms = JsonConvert.SerializeObject(post_params);
                    string signature = CreateSignature(uri, serializedParms, nonce);
                    var content = CreateHttpContent(serializedParms);
                    var request = CreateHttpRequestMessage(uri, signature, content, nonce);
                    var result = await client.SendAsync(request).Result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<GetBalanceReturn>(result);
                    // var balance = ((IList<BalanceInfo>)response.Data).Single(x => x.Currency == "BTC");
                    //System.Console.WriteLine(balance);
                    return response;
                }
                catch (Exception e) { throw e; };
            }
        }

        private static Task<string> GetSignature(string uri, string nonce, string post_params = null)
        {
            string signature = "";
            ASCIIEncoding encoding = new ASCIIEncoding();
            if (post_params != null)
            {
                post_params = Convert.ToBase64String(encoding.GetBytes(post_params));
                signature = GlobalSettings.API_Key + "POST" + uri + nonce + post_params;
            }
            else
            {
                signature = GlobalSettings.API_Key + "POST" + uri + nonce;
            }
            byte[] messageBytes = encoding.GetBytes(signature);
            using (HMACSHA512 _object = new HMACSHA512(GlobalSettings.Secret_Key))
            {
                byte[] hashmessage = _object.ComputeHash(messageBytes);
                return Task.FromResult(Convert.ToBase64String(hashmessage));
            }
        }

        public static string GetHash(string uri, string nonce, string post_params = null)
        {
            try
            {
                string signature = "";
                ASCIIEncoding encoding = new ASCIIEncoding();
                if (post_params != null)
                {
                    post_params = Convert.ToBase64String(encoding.GetBytes(post_params));
                    signature = GlobalSettings.API_Key + "POST" + uri + nonce + post_params;
                }
                else
                {
                    signature = GlobalSettings.API_Key + "POST" + uri + nonce;
                }                
                Byte[] keyBytes = encoding.GetBytes(GlobalSettings.Secret);
                Byte[] textBytes = encoding.GetBytes(signature);
                Byte[] hashBytes;
                using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                    hashBytes = hash.ComputeHash(textBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, "", ex);
                throw ex;
            }
        }
        #endregion
    }
}
