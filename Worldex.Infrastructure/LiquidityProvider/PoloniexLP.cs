using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider1;
using CCXT.NET.Poloniex;

namespace Worldex.Infrastructure.LiquidityProvider
{
    public class PoloniexGlobalSettings
    {
        public static string API_Key;
        public static string Secret;
    }

    public class PoloniexService : IPoloniexService
    {
        private readonly ILogger _logger;        
        string APIKey = PoloniexGlobalSettings.API_Key;
        string SecretKay = PoloniexGlobalSettings.Secret;
        PoloniexClient poloniex = new PoloniexClient("Poloniex");
        string nonce = "";
        //String APIKey = "YO5APXVF-VNWQD7N9-JTSJ3LT9-ZS49O0YD";
        //String SecretKay = "16beb7c5aec1fd95d60d3603680810d371255637ceccbed12fe432a364eaa6d5f4bc9d87f54623679f650554abbf6c622f574ea42a90f84315444a00ec86358e";
        //String nonce = "";
        public PoloniexService(ILogger<PoloniexService> logger)
        {
            _logger = logger;
        }

        //public void Connect()
        //{
        //    try
        //    {
        //        poloniex = new PoloniexClient("WQPQOK5A-M6UPYYTL-31W94IPB-DVPTI9K5", "b8e2390a1bc89c2dac73442f31e7d37126c824bd5f3bd2738f1d7d729af8fb2afe37114f2bf6a034cd18280b48fa0602b728c7ddcf760f98db9b4664c9d86a0b");
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        public async Task<Object> GetPoloniexOrderBooksAsync(string pair, long level)
        {
            try
            {
                PoloniexOrderBook Res = new PoloniexOrderBook();
                var data = await poloniex.CallApiGetAsync<Object>("https://poloniex.com/public?command=returnOrderBook&currencyPair=" + pair + "&depth="+level);
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<Object> GetPoloniexTradeHistories(string BaseCur, string secondCur, DateTime start, DateTime End)
        {
            try
            {
                Int32 StartunixTimestamp = (Int32)(DateTime.UtcNow.Subtract(start)).TotalSeconds;
                Int32 EndunixTimestamp = (Int32)(DateTime.UtcNow.Subtract(End)).TotalSeconds;
                List<PoloniexTradeHistory> Res = new List<PoloniexTradeHistory>();
                var Data = await poloniex.CallApiGetAsync<Object>("https://poloniex.com/public?command=returnTradeHistory&currencyPair="+BaseCur+"_"+secondCur+"&start="+ StartunixTimestamp + "&end="+ EndunixTimestamp);
                //Res = await poloniex.CallApiGetAsync<List<PoloniexTradeHistory>>("https://poloniex.com/public?command=returnTradeHistory&currencyPair=BTC_NXT&start=1546304461&end=1546423261");
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<List<PoloniexTradeHistory>> GetPoloniexTradeHistoriesV1(string Market,int Limit)
        {
            try
            {
                List<PoloniexTradeHistory> Res = new List<PoloniexTradeHistory>();
                Res = await poloniex.CallApiGetAsync<List<PoloniexTradeHistory>>("https://poloniex.com/public?command=returnTradeHistory&currencyPair=" + Market +"&limit=" + Limit);
                //Res = await poloniex.CallApiGetAsync<List<PoloniexTradeHistory>>("https://poloniex.com/public?command=returnTradeHistory&currencyPair=BTC_NXT&start=1546304461&end=1546423261");
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        

        public async Task<Object> poloniexChartData(string BaseCur, string secondCur, DateTime start, DateTime End, long? period = 14400)
        {
            try
            {
                List<PoloniexChartData> Res = new List<PoloniexChartData>();
                string pair = BaseCur + "_" + secondCur;
                Int32 StartunixTimestamp = (Int32)(DateTime.UtcNow.Subtract(start)).TotalSeconds;
                Int32 EndunixTimestamp = (Int32)(DateTime.UtcNow.Subtract(End)).TotalSeconds;
                var data = await poloniex.CallApiGetAsync<Object>("https://poloniex.com/public?command=returnChartData&currencyPair=" + pair + "&start=" + StartunixTimestamp + "&end=" + EndunixTimestamp + "&period="+period);
                //Res= await poloniex.CallApiGetAsync<List<PoloniexChartData>>("https://poloniex.com/public?command=returnChartData&currencyPair=BTC_LTC&start=1405699200&end=9999999999&period=14400");
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<Object> GetPoloniex24Volume()
        {
            try
            {
                var Data = await poloniex.CallApiGetAsync<List<PoloniexChartData>>("https://poloniex.com/public?command=return24hVolume");
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<Object> GetPoloniexCurrency()
        {
            try
            {
                var Data = await poloniex.CallApiGetAsync<List<PoloniexChartData>>("https://poloniex.com/public?command=returnCurrencies");
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<Object> GetPoloniexTicker()
        {
            try
            {
                var Data = await poloniex.CallApiGetAsync<Object>(" https://poloniex.com/public?command=returnTicker");
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<object> GetPoloniexOpenOrder(string BaseCur, string secondCur)
        {
            try
            {
                nonce = Helpers.GetTimeStamp().ToString();
                string Sign = "command=returnOpenOrders&currencyPair="+BaseCur+"_"+secondCur+"&nonce=" + nonce;
                Dictionary<string, object> args = new Dictionary<string, object>();
                args.Add("command", "returnOpenOrders");
                args.Add("currencyPair", BaseCur + "_" + secondCur);
                args.Add("Key", APIKey);
                args.Add("Sign", this.GetHash(Sign, SecretKay));
                var Data = await poloniex.CallApiPostAsync<Object>("https://poloniex.com/tradingApi", args);
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<Object> GetPoloniexOrderTrade(string orderNumber)
        {
            try
            {
                nonce = Helpers.GetTimeStamp().ToString();
                string Sign = "command=returnOrderTrades&orderNumber=" + orderNumber + "&nonce=" + nonce;
                Dictionary<string, object> args = new Dictionary<string, object>();
                args.Add("command", "returnOrderTrades");
                args.Add("orderNumber", orderNumber);
                args.Add("Key", APIKey);
                args.Add("Sign", this.GetHash(Sign, SecretKay));

                var Data = await poloniex.CallApiPostAsync<Object>("https://poloniex.com/tradingApi", args);
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<Object> GetPoloniexOrderState(string orderNumber)
        {
            try
            {
                nonce = Helpers.GetTimeStamp().ToString();
                string Sign = "command=returnOrderStatus&orderNumber=" + orderNumber + "&nonce=" + nonce;
                Dictionary<string, object> args = new Dictionary<string, object>();
                args.Add("command", "returnOrderStatus");
                args.Add("orderNumber", orderNumber);
                args.Add("Key", APIKey);
                args.Add("Sign", this.GetHash(Sign, SecretKay));
                var Data = await poloniex.CallApiPostAsync<Object>("https://poloniex.com/tradingApi", args);
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<Object> CancelPoloniexOrder(string orderNumber)
        {
            try
            {
                nonce = Helpers.GetTimeStamp().ToString();
                string Sign = "command=cancelOrder&orderNumber=" + orderNumber + "&nonce=" + nonce;
                Dictionary<string, object> args = new Dictionary<string, object>();
                args.Add("command", "cancelOrder");
                args.Add("orderNumber", orderNumber);
                args.Add("Key", APIKey);
                args.Add("Sign", this.GetHash(Sign, SecretKay));
                var Data = await poloniex.CallApiPostAsync<Object>("https://poloniex.com/tradingApi", args);
                var res = JsonConvert.SerializeObject(Data);
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<String> PlacePoloniexOrder(string BaseCur, string secondCur, decimal amount, decimal rate, enOrderType orderType)
        {
            try
            {
                string Sign = "";
                nonce = Helpers.GetTimeStamp().ToString();
                Dictionary<string, object> args = new Dictionary<string, object>();
                if (orderType == enOrderType.BuyOrder)
                {
                    args.Add("command", "buy");
                    Sign = "command=buy&currencyPair=" + BaseCur + "_" + secondCur + "&rate=" + rate + "&amount=" + amount + "&nonce=" + nonce;
                }
                else if (orderType == enOrderType.SellOrder)
                {
                    args.Add("command", "sell");
                    Sign = "command=sell&currencyPair=" + BaseCur + "_" + secondCur + "&rate=" + rate + "&amount=" + amount + "&nonce=" + nonce;

                }
                args.Add("Key", APIKey);
                args.Add("currencyPair", BaseCur + "_" + secondCur);
                args.Add("Sign", this.GetHash(Sign, SecretKay));
                
                var Data = await poloniex.CallApiPostAsync<Object>("https://poloniex.com/tradingApi", args);
                return JsonConvert.SerializeObject( Data);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public string GetHash(string Sign, string secretKey)
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] keyBytes = encoding.GetBytes(secretKey);
                Byte[] textBytes = encoding.GetBytes(Sign);
                Byte[] hashBytes;
                using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                    hashBytes = hash.ComputeHash(textBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<Dictionary<string, decimal>> PoloniexGetBalance()
        {
            try
            {
                nonce = Helpers.GetTimeStamp().ToString();
                string Sign = "command=returnBalances&nonce=" + nonce;
                Dictionary<string, object> args = new Dictionary<string, object>();
                args.Add("command", "returnBalances");
                args.Add("Key", APIKey);
                args.Add("Sign", this.GetHash(Sign,SecretKay));
                var Data = await poloniex.CallApiPostAsync<Dictionary<string, decimal>>("https://poloniex.com/tradingApi", args);
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
    }


    public class Rootobject
    {
        public Object Res { get; set; }
    }
    //public class PoloniexOrderBook
    //{
    //    public List<List<Object>> asks { get; set; }
    //    public List<List<Object>> bids { get; set; }
    //    public string isFrozen { get; set; }
    //    public int seq { get; set; }
    //}

    //public class PoloniexChartData
    //{
    //    public int date { get; set; }
    //    public double high { get; set; }
    //    public double low { get; set; }
    //    public double open { get; set; }
    //    public double close { get; set; }
    //    public double volume { get; set; }
    //    public double quoteVolume { get; set; }
    //    public double weightedAverage { get; set; }
    //}
    //public class PoloniexOpenOrder
    //{
    //    public string orderNumber { get; set; }
    //    public string type { get; set; }
    //    public string rate { get; set; }
    //    public string amount { get; set; }
    //    public string total { get; set; }
    //}

    //public class PoloniexResultingTrade
    //{
    //    public string amount { get; set; }
    //    public string date { get; set; }
    //    public string rate { get; set; }
    //    public string total { get; set; }
    //    public string tradeID { get; set; }
    //    public string type { get; set; }
    //}

    //public class PoloniexOrderResult
    //{
    //    public int orderNumber { get; set; }
    //    public IList<PoloniexResultingTrade> resultingTrades { get; set; }
    //}

    //public class PoloniexOrderState
    //{
    //    public string status { get; set; }
    //    public string rate { get; set; }
    //    public string amount { get; set; }
    //    public string currencyPair { get; set; }
    //    public string date { get; set; }
    //    public string total { get; set; }
    //    public string type { get; set; }
    //    public string startingAmount { get; set; }
    //}
    //public class PoloniexErrorObj
    //{
    //    public string error { get; set; }
    //}
}
