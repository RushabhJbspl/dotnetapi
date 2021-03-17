using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Worldex.Infrastructure.LiquidityProvider.Yobit.Models.PublicApi;
using Worldex.Infrastructure.LiquidityProvider.Yobit.Models.PublicApi.Info;
using Worldex.Infrastructure.LiquidityProvider.Yobit.Models.PublicApi.Ticker;
using Worldex.Infrastructure.LiquidityProvider.Yobit.Models.TradeApi;
using Worldex.Infrastructure.LiquidityProvider.Yobit.Models.TradeApi.Trade;
using Worldex.Infrastructure.LiquidityProvider.Yobit.Models.TradeApi.GetInfo;
using Worldex.Core.ViewModels.LiquidityProvider;

namespace Worldex.Infrastructure.LiquidityProvider.Yobit
{
    public class YobitApiClient
    {
        private readonly IYobitClientLogger Logger;
        private readonly string ApiKey;
        private readonly string ApiSecret;
        private byte[] ApiSecretBytes { get { return Encoding.UTF8.GetBytes(ApiSecret); } }
        private readonly bool IgnoreInvalidPairs;
        public static string PublicApiUrl = "https://yobit.net/api/3";
        public static Uri PublicApiUri = new Uri(PublicApiUrl);
        public static string TradeApiUrl = "https://yobit.net/tapi";
        public static Uri TradeApiUri = new Uri(TradeApiUrl);

        public YobitApiClient(string key, string secret, bool ignoreInvalidPairs = true)
        {
            ApiKey = key;
            ApiSecret = secret;            
            IgnoreInvalidPairs = ignoreInvalidPairs;
        }

        
        public Models.PublicApi.Ticker.TickerResponse Ticker(CoinsPairs pairs)
        {
            Logger.Log("Called 'Ticker' method");
            var methodUri = PublicMethodUri("ticker", pairs);
            var _res = ProcessPublicRequest(methodUri);
            var response = JsonConvert.DeserializeObject<Models.PublicApi.Ticker.TickerResponse>(_res);
            return response;
        }

       

        public YobitAPIReqRes.GetInfoResponse GetInfo()
        {
            //Logger.Log("Called 'GetInfo' method");
            var methodUri = new Uri(TradeApiUrl);
            var parameters = new Dictionary<string, string>
            {
                { "method", "getInfo" },
                { "nonce", ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString() }
            };
            var _res = ProcessTradeRequest(methodUri, parameters).Result;
            var response = JsonConvert.DeserializeObject<YobitAPIReqRes.GetInfoResponse>(_res);
            return response;
        }

        /// <summary>
        /// Метод, с помощью которого создаются новые ордера для торговли на бирже
        /// </summary>
        /// <param name="pair">пара (пример: ltc_btc)</param>
        /// <param name="type">тип операции (пример: buy или sell)</param>
        /// <param name="rate">курс, по которому необходимо купить или продать (значение: числовое)</param>
        /// <param name="amount">количество, которое необходимо купить или продать (значение: числовое)</param>
        /// <returns>Ответ биржи</returns>
        public YobitAPIReqRes.TradeResponse Trade(string pair, string type, decimal rate, decimal amount)
        {
            //Logger.Log("Called 'Trade' method");
            var methodUri = new Uri(TradeApiUrl);
            var parameters = new Dictionary<string, string>
            {
                { "method", "Trade" },
                { "pair", pair.ToString() },
                { "type", type },
                { "rate", rate.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture) },
                { "amount", amount.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture) },
                { "nonce", ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString() }
            };
            var _res = ProcessTradeRequest(methodUri, parameters).Result;
            var response = JsonConvert.DeserializeObject<YobitAPIReqRes.TradeResponse>(_res);
            return response;
        }

        private Uri PublicMethodUri(string method, CoinsPairs pairs)
        {
            var url = $"{PublicApiUrl}/{method}";
            if (pairs != null && pairs.Count != 0)
                url += $"/{pairs.UrlPath}";
            if (IgnoreInvalidPairs) url += "?ignore_invalid=1";
            return new Uri(url);
        }

        public class CoinPair
        {
            public string Coin1 { get; set; }
            public string Coin2 { get; set; }
            public bool Reverted { get; set; }

            public CoinPair(string coin1, string coin2)
            {
                Coin1 = coin1;
                Coin2 = coin2;
                if (Coin1 == "rur" && Coin2 == "usd")
                {
                    Coin1 = "usd";
                    Coin2 = "rur";
                    Reverted = true;
                    return;
                }
                
            }

            public override string ToString()
            {
                return Coin1.ToLower() + "_" + Coin2.ToLower();
            }
        }

        public class CoinsPairs : List<CoinPair>
        {
            public CoinsPairs()
            {

            }

            public CoinsPairs(IEnumerable<CoinPair> coinPairsList)
            {
                this.AddRange(coinPairsList);
            }

            public string UrlPath
            {
                get
                {
                    return string.Join("-", this.Select(cp => cp.ToString()));
                }
            }
        }

        public enum TradeOperationType
        {
            Buy,
            Sell
        }

        private static Dictionary<TradeOperationType, string> TradeOperationTypeNames = new Dictionary<TradeOperationType, string> { { TradeOperationType.Buy, "buy" }, { TradeOperationType.Sell, "sell" } };

        private string ProcessPublicRequest(Uri uri)
        {
            //Ответы сервера кешируются каждые 2 секунды, поэтому делать запросы чаще не имеет смысла

            Logger.Log($"ProcessPublicRequest for url: {uri.ToString()}");
            var webRequest = WebRequest.Create(uri);
            var result = ProcessWebRequest(webRequest);
            Logger.Log($"Response: {result}");
            return result;
        }

        public async Task<string> ProcessTradeRequest(Uri uri, Dictionary<string, string> parameters)
        {
            //Logger.Log($"ProcessTradeRequest for url: {uri.ToString()}");
            HttpClient client = new HttpClient();
            var content = new FormUrlEncodedContent(parameters);
            content.Headers.Add("Key", ApiKey);
            var sign = CalculateSign(parameters);
            content.Headers.Add("Sign", sign);
            var response = await client.PostAsync(uri, content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                //Logger.Log($"Получен ответ со статусом {response.StatusCode.ToString()}");
                throw new Exception($"На запрос по адресу {uri.ToString()} получен ответ со статусом {response.StatusCode.ToString()}");
            }
            var responseString = string.Empty;
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                responseString = readStream.ReadToEnd();
            //Logger.Log($"Response: {responseString}");
            return responseString;
        }

        private string CalculateSign(Dictionary<string, string> parameters)
        {
            var parametersString = string.Join("&", parameters.Select(kv => kv.Key + "=" + kv.Value));
            string result = string.Empty;
            byte[] inputBytes = Encoding.UTF8.GetBytes(parametersString);
            using (var hmac = new HMACSHA512(ApiSecretBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                StringBuilder hex1 = new StringBuilder(hashValue.Length * 2);
                foreach (byte b in hashValue)
                {
                    hex1.AppendFormat("{0:x2}", b);
                }
                result = hex1.ToString();
            }
            return result;
        }

        private string ProcessWebRequest(WebRequest webRequest)
        {
            var response = webRequest.GetResponse();
            var result = string.Empty;
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }

        public interface IYobitClientLogger
        {
            void Log(string message);
        }
    }

   

   
}
