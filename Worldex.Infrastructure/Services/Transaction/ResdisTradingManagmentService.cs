using CachingFramework.Redis;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Transaction
{
    public class ResdisTradingManagmentService : IResdisTradingManagment
    {
        private readonly IConfiguration Configuration;
        protected readonly RedisConnectionFactory ConnectionFactory;
        string ControllerName = "ResdisTradingManagmentService";

        public ResdisTradingManagmentService(IConfiguration configuration, RedisConnectionFactory connectionFactory)
        {
            Configuration = configuration;
            ConnectionFactory = connectionFactory;
        }
        public async Task TransactionOrderCacheEntry(BizResponse _Resp, long TrnNo, long PairID, string PairName, decimal Price, decimal Qty, decimal RemainQty, short OrderType, string OrderSide, short IsAPITrade = 0)
        {
            try
            {
                var Redis = new RadisServices<CacheOrderData>(ConnectionFactory);

                CacheOrderData CacheOrder = new CacheOrderData()
                {
                    IsProcessing = 1,
                    PairID = PairID,
                    Price = Price,
                    Qty = Qty,
                    RemainQty = RemainQty,
                    TrnNo = TrnNo,
                    OrderType = OrderType,
                    IsAPITrade = IsAPITrade
                };


                string RedisPath = GetMainPathKey(PairName, OrderSide);

                Redis.SaveWithOrigionalKey(RedisPath + TrnNo, CacheOrder, "");//without tag , Int value save as #name

                Redis.SaveToSortedSetByPrice(RedisPath + Configuration.GetValue<string>("TradingKeys:SortedSetName"), TrnNo.ToString(), Helpers.TradingPriceToRedis(Price));//without tag , Int value save as #name


                _Resp.ReturnCode = enResponseCodeService.Success;
                _Resp.ErrorCode = enErrorCode.TransactionInsertSuccess;
                _Resp.ReturnMsg = "Success";

              await  Task.WhenAll();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("TransactionOrderCacheEntry Internal Error ##TrnNo:" + TrnNo, ControllerName, ex);
            }
        }

        public async Task<BizResponse> MakeNewTransactionEntry(BizResponse _Resp)
        {
            try
            {
                var Redis = new RadisServices<CacheOrderData>(ConnectionFactory);
                CacheOrderData CacheOrder = new CacheOrderData()
                {
                    IsProcessing = 1,
                    PairID = 10031001,
                    Price = 0.000004520000000000M,
                    Qty = 7021.690265490000000000M,
                    RemainQty = 7021.690265490000000000M,
                    TrnNo = 6587
                };


                string RedisPath = GetMainPathKey("ATCC_BTC", "Buy");

                Redis.SaveWithOrigionalKey(RedisPath + 6587, CacheOrder, "");

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MakeNewTransactionEntry Internal Error:##TrnNo " + 0, ControllerName, ex);
            }
            return await Task.FromResult(_Resp);
        }
        public string GetMainPathKey(string PairName, string OrderSide)
        {
            string MainPathKay = "";
            try
            {
                //string Pair = "ATCC_BTC:Buy:1000";
                //ParoStagingTrading:ATCC_BTC:Buy:

                PairName += ":";
                OrderSide += ":";
                MainPathKay = Configuration.GetValue<string>("TradingKeys:RedisClientName") + PairName + OrderSide;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetMainPathKey Internal Error:##PairName " + PairName + " OrderSide:" + OrderSide, ControllerName, ex);
            }
            return MainPathKay;

        }

        public async Task<BizResponse> StoreDataToRedis(BizResponse _Resp, List<string> LPList, List<string> PairName, RedisTickerData data = null)
        {
            try
            {
                #region test Code
                //List<string> LPList = new List<string>();
                LPList.Add("binance");
                LPList.Add("okex3");
                LPList.Add("exmo");

                //List<string> PairName = new List<string>();
                PairName.Add("BTC_USDT");
                PairName.Add("LTC_BTC");
                PairName.Add("ATCC_BTC");


                var Redis = new RadisServices<TickerData>(ConnectionFactory);

                foreach (var LP in LPList)
                {
                    foreach (var pair in PairName)
                    {
                        TickerData CacheDatar = new TickerData()
                        {
                            LTP = (double)0.000004M,
                            Pair = pair,
                            LPType = 0,
                            Volume = 7021,
                            Fees = 0,
                            ChangePer = 0,
                            UpDownBit = 1,
                            UpdatedBy = 99,
                            UpdateDate = Helpers.UTC_To_IST()
                        };

                        Redis.SaveWithOrigionalKey("TickerData:" + LP + ":" + pair, CacheDatar, "");
                    }
                }
                #endregion

                #region Old Code
                //var Redis = new RadisServices<TickerData>(ConnectionFactory);
                //if (data == null)
                //{
                //    foreach (var LP in LPList)
                //    {
                //        foreach (var pair in PairName)
                //        {
                //            TickerData CacheDatar = new TickerData()
                //            {
                //                LTP = 0.000004M,
                //                Pair = pair,
                //                LPType = 0,
                //                Volume = 7021,
                //                Fees = 0,
                //                ChangePer = 0,
                //                UpDownBit = 1,
                //                UpdatedBy = 99,
                //                UpdateDate = Helpers.UTC_To_IST()
                //            };
                //            Redis.SaveWithOrigionalKey("TickerData:" + LP + ":" + pair, CacheDatar, "");
                //        }
                //    }
                //}
                //else
                //{
                //    //var Redisdata = Redis.GetData("9");
                //    foreach (var LP in LPList)
                //    {
                //        foreach (var pair in PairName)
                //        {
                //            if (LP.ToUpper() == data.LPName.ToUpper() && pair.ToUpper() == data.Pair.ToUpper())
                //            {
                //                //var Redisdata = Redis.GetData("TickerData:" + LP + ":" + pair);
                //                TickerData CacheDatar = new TickerData()
                //                {
                //                    LTP = data.LTP,
                //                    Pair = pair,
                //                    LPType = data.LPType,
                //                    Volume = data.Volume,
                //                    Fees = 0,
                //                    ChangePer = data.ChangePer,
                //                    UpDownBit = 1,
                //                    UpdatedBy = 99,
                //                    UpdateDate = Helpers.UTC_To_IST()
                //                };
                //                Redis.SaveWithOrigionalKey("TickerData:" + LP + ":" + pair, CacheDatar, "");
                //            }
                //        }
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MakeNewTransactionEntry Internal Error:##TrnNo " + 0, ControllerName, ex);
            }
            return _Resp;
        }

        //Rita 19-8-19 for global Ticker and Orderbook data save to redis=============================================================
        public string GetTickerPathKey(string LPName, string PairName)
        {
            string MainPathKay = "";
            try
            {
                //TickerData:Binance:BTC_USDT                               
                MainPathKay = "TickerData:" + LPName + ":" + PairName;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTickerPathKey Internal Error:##LPName " + LPName + " PairName:" + PairName, ControllerName, ex);
            }
            return MainPathKay;
        }
        public string GetOrderBookPathKey(string PairName, string OrderType, string LPName="")
        {
            string MainPathKay = "";
            try
            {//for match-engine taken pair and order type first
                //OrderBook:BTC_USDT:BUY:Binance
                //OrderBook:BTC_USDT:SELL:Binance
                if(LPName=="")
                    MainPathKay = "OrderBook:" + PairName + ":" + OrderType; //for getting all provider list,for match-engine
                else
                    MainPathKay = "OrderBook:" + PairName + ":" + OrderType + ":" + LPName;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTickerPathKey Internal Error:##LPName " + LPName + " PairName:" + PairName, ControllerName, ex);
            }
            return MainPathKay;
        }

        public async Task<BizResponse> StoreDataToRedisv2(BizResponse _Resp, RedisTickerData data = null)
        {
            try
            {
                var Redis = new RadisServices<TickerData>(ConnectionFactory);
                if (data != null)
                {
                    TickerData CacheDatar = new TickerData()
                    {
                        LTP = (double)data.LTP,
                        Pair = data.Pair,
                        LPType = data.LPType,
                        Volume = (double)data.Volume,
                        Fees = 0,
                        ChangePer = (double)data.ChangePer,
                        UpDownBit = data.UpDownBit,
                        UpdatedBy = 99,
                        UpdateDate = Helpers.UTC_To_IST()
                    };
                    //Redis.SaveWithOrigionalKey("TickerData:" + data.LPName + ":" + data.Pair, CacheDatar, "");
                    Redis.SaveWithOrigionalKey(GetTickerPathKey(data.LPName, data.Pair), CacheDatar, "");
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MakeNewTransactionEntry Internal Error:##TrnNo " + 0, ControllerName, ex);
            }
            return await Task.FromResult(_Resp);
        }

        public async Task<BizResponse> StoreOrderBookToRedis(BizResponse _Resp, List<GetBuySellBook> OrderList,string LPName, string PairName, string OrderType)
        {
            try
            {
                var Redis = new RadisServices<TickerData>(ConnectionFactory);
                string Orderbookpath = GetOrderBookPathKey(PairName, OrderType, LPName);
                if (OrderList != null)
                {
                    foreach(GetBuySellBook order in OrderList)
                    {
                        Redis.SaveToSortedSetByPrice(Orderbookpath, order.Amount.ToString(), Helpers.TradingPriceToRedis(order.Price));
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MakeNewTransactionEntry Internal Error:##TrnNo " + 0, ControllerName, ex);
            }
            return await Task.FromResult(_Resp);
        }
        public async Task<BizResponse> GetOrderBookFromRedis(BizResponse _Resp, string PairName, string OrderType,string LPName)
        {

            try
            {
                var Redis = new RadisServices<TickerData>(ConnectionFactory);
                string Orderbookpath = GetOrderBookPathKey(PairName, OrderType, LPName);

                var dfdg = Redis.GetSortedSetDataByScore(Orderbookpath, 1, 20);//Redis.GetSortedSet(Orderbookpath,"");

                var Messages = Redis.GetSortedSet(Orderbookpath);

                
                    int i = 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MakeNewTransactionEntry Internal Error:##TrnNo " + 0, ControllerName, ex);
            }
            return await Task.FromResult(_Resp);
        }

        public async Task<List<GetBuySellBook>> GetTopOrderBookFromRedis(string LPName, string PairName, string OrderType, int MaxListCount)
        {
            try
            {
                var Redis = new RadisServices<GetBuySellBook>(ConnectionFactory);
                string Orderbookpath = GetOrderBookPathKey(PairName, OrderType, LPName);
                string resultJson = Redis.GetSortedSetList(Orderbookpath,MaxListCount,OrderType);
                if(!string.IsNullOrEmpty(resultJson))
                {
                    List<GetBuySellBook>  OrderList = JsonConvert.DeserializeObject<List<GetBuySellBook>>(resultJson);
                    return await Task.FromResult(OrderList);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTopTransactionEntry Internal Error:##TrnNo " + 0, ControllerName, ex);
            }
            return null;
        }
        //ntrivedi added from beta 02-11-2019
        public Task<TickerData> GetTickerDataAsync(string lpName, string pairName)
        {
            TickerData data = null;
            try
            {
                TickerDataViewModel redisobj = null;
                var redisServices = new RadisServices<TickerDataViewModel>(ConnectionFactory);
                string key = $"TickerData:{lpName.ToUpper()}:{pairName.ToUpper()}";
                redisobj = redisServices.GetV2(key) ?? throw new Exception($"Ticker Data retrieve from redis failed for key: {key}");
                try
                {
                    data = JsonConvert.DeserializeObject<TickerData>(JsonConvert.SerializeObject(redisobj));
                }
                catch
                {
                    return Task.FromResult(data);
                }
                return Task.FromResult(data);
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetTickerDataAsync", ControllerName, e);
                return Task.FromResult(data);
            }
        }

        //===============================================================================================================================
    }
    public class CacheOrderData
    {
        public long TrnNo { get; set; }

        public long PairID { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Price { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Qty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal RemainQty { get; set; }

        public short OrderType { get; set; }

        public short IsProcessing { get; set; }

        public short IsAPITrade { get; set; } = 0;//Rita 30-1-19 API trading bit set to 1, rest all 0
    }

    public class TickerData
    {
        //change from decimal to double for ignorance of error -Sahil 05-11-2019 06:13 PM
        //[Required]
        //[Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        //[Column(TypeName = "decimal(28, 18)")]
        public double LTP { get; set; }

        [Required]
        public string Pair { get; set; }

        [Key]
        [Required]
        public short LPType { get; set; }

        //[Key]
        //public long PairId { get; set; }

        //change from decimal to double for ignorance of error -Sahil 05-11-2019 06:13 PM
        //[Required]
        //[Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        //[Column(TypeName = "decimal(28, 18)")]
        public double Volume { get; set; }

        //change from decimal to double for ignorance of error -Sahil 05-11-2019 06:13 PM
        //[Required]
        //[Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        //[Column(TypeName = "decimal(28, 18)")]
        public double Fees { get; set; }

        //change from decimal to double for ignorance of error -Sahil 05-11-2019 06:13 PM
        //[Required]
        //[Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        //[Column(TypeName = "decimal(28, 18)")]
        public double ChangePer { get; set; }

        public short UpDownBit { get; set; }

        public long UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public class RedisTickerData
    {
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal LTP { get; set; }

        [Required]
        public string Pair { get; set; }

        [Key]
        [Required]
        public short LPType { get; set; }

        public string LPName { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Volume { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Fees { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ChangePer { get; set; }

        public short UpDownBit { get; set; }

    }

    public class TickerDataViewModel
    {
        public string LTP { get; set; }

        public string Pair { get; set; }

        public string LPType { get; set; }

        public string LPName { get; set; }

        public string Volume { get; set; }

        public string Fees { get; set; }

        public string ChangePer { get; set; }

        public string UpDownBit { get; set; }

        public string UpdateDate { get; set; }
    }

}
