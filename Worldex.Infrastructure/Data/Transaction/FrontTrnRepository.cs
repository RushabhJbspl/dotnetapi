using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.Arbitrage;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using Worldex.Infrastructure.DTOClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.ViewModels.Transaction.MarketMaker;

namespace Worldex.Infrastructure.Data.Transaction
{
    public class FrontTrnRepository : IFrontTrnRepository
    {
        private readonly WorldexContext _dbContext;
        private readonly WorldexContext _dbContext2;
        private readonly ILogger<FrontTrnRepository> _logger;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private IMemoryCache _cache;
        private readonly IWebApiRepository _WebApiRepository;

        public FrontTrnRepository(WorldexContext dbContext, ILogger<FrontTrnRepository> logger, Microsoft.Extensions.Configuration.IConfiguration configuration, IMemoryCache cache, IWebApiRepository WebApiRepository)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext;
            _logger = logger;
            _dbContext.Database.SetCommandTimeout(180);
            _configuration = configuration;
            _cache = cache;
            _WebApiRepository = WebApiRepository;
        }

        #region History method

        public GetTradeSettlePrice GetTradeSettlementPrice(long TrnNo)
        {
            try
            {
                var res = _dbContext.GetTradeSettlePrice.FromSql("SELECT avg(TakerPrice) AS SettlementPrice FROM TradePoolQueueV1 WHERE (TakerTrnNo={0} OR MakerTrnNo={0}) AND STATUS=1", TrnNo).FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public GetTradeSettlePrice GetTradeSettlementPriceMargin(long TrnNo)
        {
            try
            {
                var res = _dbContext.GetTradeSettlePrice.FromSql("SELECT avg(TakerPrice) AS SettlementPrice FROM TradePoolQueueMarginV1 WHERE (TakerTrnNo={0} OR MakerTrnNo={0}) AND STATUS=1", TrnNo).FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //public OpenOrderInfo CheckOpenOrderRange(long TrnNo)
        //{
        //    try
        //    {
        //        IQueryable<OpenOrderQryResponse> Result;
        //        Result = _dbContext.OpenOrderRespose.FromSql(@"Select TTQ.TrnNo,TTQ.ordertype,TTQ.PairName,TTQ.PairId,TTQ.TrnDate,TTQ.TrnTypeName as Type,TTQ.Order_Currency,TTQ.Delivery_Currency,  
        //                CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount,
        //                CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price,
        //                TTQ.IsCancelled from TradeTransactionQueue TTQ  INNER join TradeStopLoss TSL on TTQ.TrnNo = TSL.TrnNo INNER join TradePairStastics TPS on TTQ.PairID = TPS.PairId
        //                where ((TSL.MarketIndicator = 0 AND TSL.StopPrice <= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice >= TPS.LTP)) AND TTQ.Status = 4
        //                AND TTQ.ordertype = 4 and TTQ.TrnNo = {0}", TrnNo);

        //        if (Result.SingleOrDefault() != null)
        //        {
        //            var model = Result.SingleOrDefault();
        //            OpenOrderInfo _Res = new OpenOrderInfo()
        //            {
        //                Amount = model.Amount,
        //                Delivery_Currency = model.Delivery_Currency,
        //                Id = model.TrnNo,
        //                IsCancelled = model.IsCancelled,
        //                Order_Currency = model.Order_Currency,
        //                Price = model.Price,
        //                TrnDate = model.TrnDate,
        //                Type = model.Type,
        //                PairName = model.PairName,
        //                PairId = model.PairId,
        //                OrderType = Enum.GetName(typeof(enTransactionMarketType), model.ordertype)
        //            };
        //            return _Res;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        public List<TopLooserGainerPairData> GetFrontTopGainerPair(int Type)
        {
            try
            {
                IQueryable<TopLooserGainerPairData> Result = null;

                if (Type == Convert.ToInt32(EnTopLossGainerFilterType.VolumeWise)) //Volume Wise (High to Low Volume Wise Pair Data)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                   @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                    TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMaster TPM Inner Join TradePairStastics TPS ON TPS.PairId = TPM.Id
                                    Where TPM.Status = 1 Order By TPS.ChangeVol24 Desc");
                }
                else if (Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangePerWise)) //Chnage Per Wise (High to Low ChangePer Wise Pair Data, And Only > 0 ChnagePer)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                 @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                    TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMaster TPM Inner Join TradePairStastics TPS ON TPS.PairId = TPM.Id
                                    Where TPS.ChangePer24 > 0 And TPM.Status = 1 Order By TPS.ChangePer24 Desc ");
                }
                else if (Type == Convert.ToInt32(EnTopLossGainerFilterType.LTPWise))  //LTP Wise (High to Low LTP Wise Pair Data)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                       @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                        TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMaster TPM Inner Join TradePairStastics TPS ON TPS.PairId = TPM.Id
                                        Where TPS.LTP > 0 And TPM.Status = 1 Order By TPS.LTP Desc ");
                }
                else if (Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangeValueWise))  //Change Value Wise (High to Low ChangeValue Wise Pair Data, And Only > 0 Chnagevalue)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                       @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                        TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMaster TPM Inner Join TradePairStastics TPS ON TPS.PairId = TPM.Id
                                        Where TPS.ChangeValue > 0 And TPM.Status = 1 Order By TPS.ChangeValue Desc ");
                }

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TopLooserGainerPairData> GetFrontTopLooserPair(int Type)
        {
            try
            {
                IQueryable<TopLooserGainerPairData> Result = null;

                if (Type == Convert.ToInt32(EnTopLossGainerFilterType.VolumeWise))  //Volume Wise (Low to High Volume Wise Pair Data) 
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                   @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                    TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMaster TPM Inner Join TradePairStastics TPS ON TPS.PairId = TPM.Id
                                    Where TPM.Status = 1 Order By TPS.ChangeVol24 Asc");
                }
                else if (Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangePerWise))  //Chnage Per Wise (Low to High ChangePer Wise Pair Data, And Only < 0 ChnagePer)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                 @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                    TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMaster TPM Inner Join TradePairStastics TPS ON TPS.PairId = TPM.Id
                                    Where TPS.ChangePer24 < 0 And TPM.Status = 1 Order By TPS.ChangePer24 Asc");
                }
                else if (Type == Convert.ToInt32(EnTopLossGainerFilterType.LTPWise))  // LTP Wise (Low to High LTP Wise Pair Data)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                       @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                        TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMaster TPM Inner Join TradePairStastics TPS ON TPS.PairId = TPM.Id
                                        Where TPS.LTP > 0 And TPM.Status = 1 Order By TPS.LTP Asc");
                }
                else if (Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangeValueWise))  // Change Value Wise (Low to High ChangeValue Wise Pair Data, And Only < 0 Chnagevalue)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                       @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                        TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMaster TPM Inner Join TradePairStastics TPS ON TPS.PairId = TPM.Id
                                        Where TPS.ChangeValue < 0 And TPM.Status = 1 Order By TPS.ChangeValue Asc");
                }

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TopLooserGainerPairData> GetFrontTopLooserGainerPair()
        {
            try
            {
                IQueryable<TopLooserGainerPairData> Result = null;
                //Uday 04-01-2019  Pair Name Wise Filteration in Ascending Order 
                Result = _dbContext.TopLooserPairData.FromSql(
                                @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMaster TPM Inner Join TradePairStastics TPS ON TPS.PairId = TPM.Id
                                Where TPM.Status = 1 Order By TPM.PairName");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public decimal GetHistoricalPerformanceData(long UserId, int Type)
        {
            try
            {
                HistoricalPerformance Data = null;
                if (Type == 1) // Deposit Value
                {
                    Data = _dbContext.HistoricalPerformance.FromSql(@"select Isnull(Sum(Amount * 72),0) As Amount from DepositHistory Where Status = 1 
                                                                 And UserId = {0} And MONTH(CreatedDate) = MONTH(dbo.getistdate())", UserId).FirstOrDefault();

                }
                else if (Type == 2) // Withdrwal Value
                {
                    Data = _dbContext.HistoricalPerformance.FromSql(@"Select Isnull(Sum(Amount * 72),0) As Amount from TransactionQueue 
                                                            Where TrnType = 6 And Status = 1 And MemberId = {0} And MONTH(TrnDate) = MONTH(dbo.getistdate())", UserId).FirstOrDefault();
                }

                return Data.Amount;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<CopiedLeaderOrdersQryRes> GetCopiedLeaderOrders(long MemberID, string FromDate = null, string ToDate = null, long PairId = 999, short trnType = 999, string FollowTradeType = "", long FollowingTo = 0)
        {
            IQueryable<CopiedLeaderOrdersQryRes> Result = null;
            string qry = "", sCon = " ";
            DateTime fDate, tDate;
            try
            {
                if (PairId != 999)
                    sCon = " AND TTQ.PairID =" + PairId;
                if (trnType != 999)
                    sCon += " AND TTQ.TrnType =" + trnType;
                if (FollowingTo != 0)
                    sCon += " AND TSL.FollowingTo =" + FollowingTo;
                if (!string.IsNullOrEmpty(FollowTradeType))
                    sCon += " AND TSL.FollowTradeType ='" + FollowTradeType + "' ";
                if (MemberID != 0)
                    sCon += " AND TTQ.MemberID =" + MemberID;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCon += " AND TTQ.TrnDate Between {0} AND {1} ";
                }

                qry = "Select TTQ.TrnNo,TTQ.StatusCode,TSL.ordertype,TTQ.TrnTypeName as Type, TTQ.TrnDate as DateTime, TTQ.Status, 'Success' as StatusText,TTQ.PairName,cast(0 as decimal) as ChargeRs,TTQ.IsCancelled, " +
                        "CASE WHEN TSL.ordertype = 2 THEN CAST(0 AS DECIMAL(28,18)) WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price, " +
                        "CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, " +
                        "CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,TTQ.SettledDate,TSL.FollowingTo,TSL.FollowTradeType " +
                        "from SettledTradeTransactionQueue TTQ INNER JOIN TradeStopLoss TSL ON TSL.TrnNo = TTQ.TrnNo " +
                        "WHERE TTQ.Status in (1, 4) and TTQ.IsCancelled = 0 AND TSL.ISFollowersReq = 1 " + sCon +
                        "UNION ALL Select TTQ.TrnNo,TTQ.StatusCode,TSL.ordertype,TTQ.TrnTypeName as Type, TTQ.TrnDate as DateTime, TTQ.Status, " +
                        "CASE WHEN TTQ.Status = 2 THEN 'Cancel' WHEN TTQ.Status=4 THEN 'Hold' ELSE 'Cancel' END as StatusText,TTQ.PairName,cast(0 as decimal) as ChargeRs,TTQ.IsCancelled, " +
                        "CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price," +
                        "CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount," +
                        "CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,TTQ.SettledDate,TSL.FollowingTo,TSL.FollowTradeType " +
                        "from TradeTransactionQueue TTQ INNER JOIN TradeStopLoss TSL ON TSL.TrnNo = TTQ.TrnNo " +
                        "WHERE(TTQ.Status = 2 OR(TTQ.Status = 1 and TTQ.IsCancelled = 1) OR TTQ.Status=4) AND TSL.ISFollowersReq = 1 " + sCon + " Order By TTQ.TrnDate desc";

                Result = _dbContext.copiedLeaderOrders.FromSql(qry, FromDate, ToDate);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TopLeaderListInfo> TopLeaderList(int IsAll = 0)
        {
            try
            {
                IQueryable<TopLeaderListInfo> Result;
                if (IsAll == 0)
                {
                    Result = _dbContext.topLeaderLists.FromSql(
                      @"select top 5 count(FM.FolowerId) NoOfFollowers,FM.LeaderId,BU.username as LeaderName from FollowerMaster FM Inner join Bizuser BU on FM.LeaderId=BU.Id where FM.status=1 
                        group by FM.LeaderId, BU.username order by NoOfFollowers desc");
                }
                else
                {
                    Result = _dbContext.topLeaderLists.FromSql(
                      @"select count(FM.FolowerId) NoOfFollowers,FM.LeaderId,BU.username as LeaderName from FollowerMaster FM Inner join Bizuser BU on FM.LeaderId=BU.Id where FM.status=1 
                        group by FM.LeaderId, BU.username order by NoOfFollowers desc");
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TradeWatchLists> getTradeWatchList(List<TradeWatchLists> TradeWatcher)
        {
            List<TradeWatchLists> watchLists = new List<TradeWatchLists>();
            try
            {

                long[] leaderArray = TradeWatcher.Select(x => (long)x.LeaderId).ToArray();
                string leaderIDlist = String.Join(",", leaderArray);
                string Qry = "select TTQ.MemberID as LeaderId,cast(Count(TTQ.TrnNo) as bigint) as Total,count(CASE WHEN TTQ.TrnType = 4 then 1 end) as buy,count(CASE WHEN TTQ.TrnType = 5 then 1 end) as sell,'' as LeaderName" +
                            " from SettledTradeTransactionQueue TTQ where TTQ.MemberID in (" + leaderIDlist + ") group by TTQ.MemberID";
                IQueryable<TradeWatchListsQryRes> Result = _dbContext.tradeWatchLists.FromSql(Qry);

                foreach (var obj in Result.ToList())
                {
                    decimal Max;
                    string trnType;
                    decimal per;

                    if (obj.sell >= obj.buy)
                    {
                        Max = obj.sell;
                        trnType = "SELL";
                    }
                    else
                    {
                        Max = obj.buy;
                        trnType = "BUY";
                    }
                    if (obj.Total == 0)
                        per = 0;
                    else
                        per = (Max * 100) / obj.Total;
                    watchLists.Add(new TradeWatchLists()
                    {
                        LeaderId = obj.LeaderId,
                        MaxTrade = Max,
                        Total = obj.Total,
                        TrnType = trnType,
                        Percentage = per,
                        LeaderName = obj.LeaderName
                    });
                }
                foreach (var obj in watchLists)
                {
                    var model = TradeWatcher.SingleOrDefault(e => e.LeaderId == obj.LeaderId);
                    model.MaxTrade = obj.MaxTrade;
                    model.Percentage = obj.Percentage;
                    model.Total = obj.Total;
                    model.TrnType = obj.TrnType;
                }
                return TradeWatcher;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<SiteTokenConversionQueryRes> GetSiteTokenConversionData(long? UserId, string SourceCurrency = "", string TargetCurrency = "", string FromDate = "", string ToDate = "", short IsMargin = 0)
        {
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                String Condition = " ";
                if (UserId != null)
                    Condition += " AND UserID ={0} ";
                if (!string.IsNullOrEmpty(SourceCurrency))
                    Condition += " AND SourceCurrency ='" + SourceCurrency + "'";
                if (!string.IsNullOrEmpty(TargetCurrency))
                    Condition += " AND TargerCurrency ='" + TargetCurrency + "'";

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND CreatedDate Between {1} AND {2} ";
                }
                string Qry;//Rita 19-4-19 for margin data report
                if (IsMargin == 1)
                    Qry = "select UserID, SourceCurrencyID, SourceCurrency, TargerCurrencyID, TargerCurrency, SourceToBaseQty, SourceCurrencyQty, TargerCurrencyQty,SourceToBasePrice, TokenPrice,CreatedDate from SiteTokenConversionMargin Where status=1 " + Condition;
                else
                    Qry = "select UserID, SourceCurrencyID, SourceCurrency, TargerCurrencyID, TargerCurrency, SourceToBaseQty, SourceCurrencyQty, TargerCurrencyQty,SourceToBasePrice, TokenPrice,CreatedDate from SiteTokenConversion Where status=1 " + Condition;

                IQueryable<SiteTokenConversionQueryRes> Result = _dbContext.siteTokenConversions.FromSql(Qry, UserId, fDate, tDate);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Optimize method
        public List<CopiedLeaderOrdersInfoV1> GetCopiedLeaderOrdersV1(long MemberID, string FromDate = null, string ToDate = null, long PairId = 999, short trnType = 999, string FollowTradeType = "", long FollowingTo = 0)
        {
            IQueryable<CopiedLeaderOrdersInfoV1> Result = null;
            string qry = "", sCon = " ";
            DateTime fDate, tDate;
            try
            {
                if (PairId != 999)
                    sCon = " AND TTQ.PairID =" + PairId;
                if (trnType != 999)
                    sCon += " AND TTQ.TrnType =" + trnType;
                if (FollowingTo != 0)
                    sCon += " AND TSL.FollowingTo =" + FollowingTo;
                if (!string.IsNullOrEmpty(FollowTradeType))
                    sCon += " AND TSL.FollowTradeType ='" + FollowTradeType + "' ";
                if (MemberID != 0)
                    sCon += " AND TTQ.MemberID =" + MemberID;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCon += " AND TTQ.TrnDate Between {0} AND {1} ";
                }

                qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as TrnNo,TTQ.StatusCode,OT.ordertype,TTQ.TrnTypeName as Type, TTQ.TrnDate as DateTime, TTQ.Status, 'Success' as StatusText,TTQ.PairName,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel, 
                                    CASE WHEN TSL.ordertype = 2 THEN CAST(0 AS DECIMAL(28,18)) WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price, 
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount,
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total, 
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,TTQ.SettledDate,TSL.FollowingTo,TSL.FollowTradeType 
                                    from SettledTradeTransactionQueue TTQ INNER JOIN TradeStopLoss TSL ON TSL.TrnNo = TTQ.TrnNo
                                    INNER JOIN TransactionQueue TQ ON TTQ.TrnNo=TQ.ID INNER JOIN OrderTypeMaster OT ON  TSL.OrderType=OT.ID
                                    WHERE TTQ.Status in (1, 4) and TTQ.IsCancelled = 0 AND TSL.ISFollowersReq = 1 {0}
                                    UNION ALL Select CAST(TQ.GUID as varchar(50)) as TrnNo,TTQ.StatusCode,OT.ordertype,TTQ.TrnTypeName as Type, TTQ.TrnDate as DateTime, TTQ.Status, 
                                    CASE WHEN TTQ.Status = 2 THEN 'Cancel' WHEN TTQ.Status=4 THEN 'Hold' ELSE 'Cancel' END as StatusText,TTQ.PairName,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel, 
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price,
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount,
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,TTQ.SettledDate,TSL.FollowingTo,TSL.FollowTradeType 
                                    from TradeTransactionQueue TTQ INNER JOIN TradeStopLoss TSL ON TSL.TrnNo = TTQ.TrnNo 
                                    INNER JOIN TransactionQueue TQ ON TTQ.TrnNo=TQ.ID INNER JOIN OrderTypeMaster OT ON  TSL.OrderType=OT.ID
                                    WHERE(TTQ.Status = 2 OR(TTQ.Status = 1 and TTQ.IsCancelled = 1) OR TTQ.Status=4) AND TSL.ISFollowersReq = 1 {0} Order By TTQ.TrnDate desc", sCon);

                Result = _dbContext.CopiedLeaderOrdersInfoV1.FromSql(qry, FromDate, ToDate);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetOrderHistoryInfo> GetOrderHistory(long PairId)
        {
            IQueryable<GetOrderHistoryInfo> Result = null;
            string sCon = "";
            try
            {
                if (PairId != 999)
                    sCon = " and TTQ.PairID =" + PairId;

                Result = _dbContext.GetOrderHistoryInfo.FromSql(string.Format(
                    @"Select top 100 CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo,TTQ.TrnDate as DateTime,TPQ.TakerType as Type,TTQ.PairName,OT.OrderType AS OrderType,
                    TPQ.TakerQty as Amount,TPQ.TakerQty as SettledQty,TTQ.IsCancelled as IsCancel,TPQ.CreatedDate as SettledDate,TPQ.TakerPrice AS Price,TPQ.TakerPrice AS SettlementPrice,
                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total
                    from TradeTransactionQueue TTQ INNER JOIN TransactionQueue TQ ON TTQ.TrnNo=TQ.ID
                    INNER JOIN TradePoolQueueV1 TPQ ON TTQ.TrnNo=TPQ.TakerTrnNo
                    INNER JOIN OrderTypeMaster OT ON  TTQ.OrderType=OT.ID
                    WHERE TTQ.Status in (1,4)  {0} Order By TPQ.CreatedDate desc", sCon));

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetOrderHistoryInfo> GetOrderHistoryMargin(long PairId)
        {
            IQueryable<GetOrderHistoryInfo> Result = null;
            string sCon = "";
            try
            {
                if (PairId != 999)
                    sCon = " and TTQ.PairID =" + PairId;

                Result = _dbContext.GetOrderHistoryInfo.FromSql(string.Format(
                    @"Select top 100 CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo,TTQ.TrnDate as DateTime,TPQ.TakerType as Type,TTQ.PairName,OT.OrderType AS OrderType,
                    TPQ.TakerQty as Amount,TPQ.TakerQty as SettledQty,TTQ.IsCancelled as IsCancel,TPQ.CreatedDate as SettledDate,TPQ.TakerPrice AS Price,TPQ.TakerPrice AS SettlementPrice,
                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total
                    from TradeTransactionQueueMargin TTQ INNER JOIN TransactionQueueMargin TQ ON TTQ.TrnNo=TQ.ID
                    INNER JOIN TradePoolQueueMarginV1 TPQ ON TTQ.TrnNo=TPQ.TakerTrnNo
                    INNER JOIN OrderTypeMaster OT ON  TTQ.OrderType=OT.ID
                    WHERE TTQ.Status in (1,4) {0} Order By TPQ.CreatedDate desc", sCon));
                //komal 20 aug 2019 remove AND TTQ.IsAPITrade=0
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetOrderHistoryInfoArbitrageV1> GetOrderHistoryArbitrage(long PairId)
        {
            IQueryable<GetOrderHistoryInfoArbitrageV1> Result = null;
            string sCon = "";
            try
            {
                if (PairId != 999)
                    sCon = " and TTQ.PairID =" + PairId;

                Result = _dbContext.GetOrderHistoryInfoArbitrageV1.FromSql(string.Format(
                    @"Select top 100 CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo,TTQ.TrnDate as DateTime,TPQ.TakerType as Type,TTQ.PairName,OT.OrderType AS OrderType,
                    TPQ.TakerQty as Amount,TPQ.TakerQty as SettledQty,TTQ.IsCancelled as IsCancel,TPQ.CreatedDate as SettledDate,TPQ.TakerPrice AS Price,TPQ.TakerPrice AS SettlementPrice,
                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                    (select AppTypeName from AppType where id=TQ.LPType) as ExchangeName
                    from TradeTransactionQueueArbitrage TTQ INNER JOIN TransactionQueueArbitrage TQ ON TTQ.TrnNo=TQ.ID
                    INNER JOIN TradePoolQueueArbitrageV1 TPQ ON TTQ.TrnNo=TPQ.TakerTrnNo
                    INNER JOIN OrderTypeMaster OT ON  TTQ.OrderType=OT.ID
                    WHERE TTQ.Status in (1,4) AND TTQ.IsAPITrade=0 {0} Order By TPQ.CreatedDate desc", sCon));

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetTradeHistoryInfoV1> GetTradeHistoryV1(long MemberID, string sCondition, string FromDate, string ToDate, int page, int IsAll, long TrnNo = 0)
        {
            IQueryable<GetTradeHistoryInfoV1> Result = null;
            string qry = "";
            DateTime fDate, tDate;
            try
            {

                if (IsAll == 1)//success
                {
                    //Uday 24-11-2018 Optimize The Query
                    //komal 30 April 2019 add charge
                    //Rita 22-3-19 order by settledDate
                    //komal 01-7-2019 add taker price average
                    qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.ID as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TSL.ordertype = 2 THEN CAST(0 AS DECIMAL(28,18)) WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price, 
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueV1 WHERE(TakerTrnNo = TTQ.TrnNo OR MakerTrnNo = TTQ.TrnNo) AND STATUS = 1),0) AS SettlementPrice,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,'Success' as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            OT.OrderType AS OrderType  from SettledTradeTransactionQueue TTQ INNER JOIN TradeStopLoss TSL ON TSL.TrnNo = TTQ.TrnNo INNER JOIN TransactionQueue TQ ON TTQ.TrnNo = TQ.ID 
                            INNER JOIN OrderTypeMaster OT ON  TSL.OrderType=OT.ID
                            WHERE {0} AND TTQ.Status in (1, 4) and TTQ.IsCancelled = 0 AND TTQ.MemberID ={1} Order By TTQ.SettledDate desc", sCondition, MemberID);
                }
                else if (IsAll == 2) //system fail
                {
                    //Rita 17-1-19 4:05:00 added Inner join with TradeStopLoss as sCondition parameter required TSL.ordertype
                    //Uday 24-11-2018 Optimize The Query
                    //komal 30 April 2019 add charge
                    //komal 01-7-2019 add taker price average
                    qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.ID as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price,
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,CASE WHEN TTQ.Status=2 THEN 'Cancel' ELSE 'Cancel' END as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            OT.OrderType AS OrderType from TradeTransactionQueue TTQ INNER JOIN OrderTypeMaster OT ON  TTQ.OrderType=OT.ID INNER JOIN TransactionQueue TQ ON TTQ.TrnNo=TQ.ID 
                            WHERE {0} AND TTQ.Status=3 AND TTQ.MemberID={1} Order By TTQ.TrnDate desc", sCondition, MemberID);

                }
                else if (IsAll == 9) // Cancel
                {
                    //Rita 17-1-19 4:05:00 added Inner join with TradeStopLoss as sCondition parameter required TSL.ordertype
                    //Uday 24-11-2018 Optimize The Query
                    //komal 30 April 2019 add charge
                    //komal 01-7-2019 add taker price average
                    qry = string.Format(@" Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price,
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,CASE WHEN TTQ.Status=2 THEN 'Cancel' ELSE 'Cancel' END as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            OT.OrderType AS OrderType from TradeTransactionQueue TTQ INNER JOIN OrderTypeMaster OT ON  TTQ.OrderType=OT.ID INNER JOIN TransactionQueue TQ ON TTQ.TrnNo=TQ.ID 
                            WHERE {0} AND TTQ.IsCancelled=1 AND TTQ.MemberID={1} Order By TTQ.TrnDate desc", sCondition, MemberID);
                }
                else //settle,cancel,fail
                {
                    //Uday 24-11-2018 Optimize The Query
                    //komal 30 April 2019 add charge
                    //komal 01-7-2019 add taker price average
                    qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TSL.ordertype=2 THEN CAST (0 AS DECIMAL(28,18)) WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price, 
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,'Success' as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            OT.OrderType AS OrderType from SettledTradeTransactionQueue TTQ INNER JOIN TradeStopLoss TSL ON TSL.TrnNo =TTQ.TrnNo INNER JOIN TransactionQueue TQ ON TTQ.TrnNo=TQ.ID INNER JOIN OrderTypeMaster OT ON  TSL.OrderType=OT.ID
                            WHERE {0} AND TTQ.Status in (1,4) and TTQ.IsCancelled = 0 AND TTQ.MemberID={1}
                            UNION ALL Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price,
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,CASE WHEN TTQ.Status=2 THEN 'Cancel' ELSE 'Cancel' END as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            OT.OrderType AS OrderType from TradeTransactionQueue TTQ INNER JOIN OrderTypeMaster OT ON  TTQ.OrderType=OT.ID INNER JOIN TransactionQueue TQ ON TTQ.TrnNo=TQ.ID 
                            WHERE {0} AND (TTQ.Status=2 OR (TTQ.Status =1 and TTQ.IsCancelled = 1)) AND TTQ.MemberID={1} Order By TTQ.TrnDate desc", sCondition, MemberID);

                }
                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Result = _dbContext.GetTradeHistoryInfo.FromSql(qry, FromDate, ToDate);
                }
                else
                    Result = _dbContext.GetTradeHistoryInfo.FromSql(qry);
                return Result.ToList();
            }
            catch (Exception ex)

            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<GetTradeHistoryInfoV1> GetTradeHistoryMarginV1(long MemberID, string sCondition, string FromDate, string ToDate, int page, int IsAll, long TrnNo = 0)
        {
            IQueryable<GetTradeHistoryInfoV1> Result = null;
            string qry = "";
            DateTime fDate, tDate;
            try
            {

                if (IsAll == 1)//success
                {
                    qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TSL.ordertype=2 THEN CAST (0 AS DECIMAL(28,18)) WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price, 
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueMarginV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,'Success' as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            OT.OrderType AS OrderType from SettledTradeTransactionQueueMargin TTQ INNER JOIN TradeStopLossMargin TSL ON TSL.TrnNo =TTQ.TrnNo 
                            INNER JOIN TransactionQueueMargin TQ ON TTQ.TrnNo=TQ.ID INNER JOIN OrderTypeMaster OT ON  TSL.OrderType=OT.ID
                            WHERE {0} AND TTQ.Status in (1,4) and TTQ.IsCancelled = 0 AND TTQ.MemberID={1}", sCondition, MemberID);

                    //Rita 22-3-19 order by settledDate
                }
                else if (IsAll == 2) //system fail
                {
                    qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TSL.ordertype=2 THEN CAST (0 AS DECIMAL(28,18)) WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price, 
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueMarginV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,'Success' as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            OT.OrderType AS OrderType from TradeTransactionQueueMargin TTQ INNER JOIN TransactionQueueMargin TQ ON TTQ.TrnNo=TQ.ID 
                            INNER JOIN OrderTypeMaster OT ON  TTQ.OrderType=OT.ID
                            WHERE {0} AND TTQ.Status=3 AND TTQ.MemberID={1}  Order By TTQ.TrnDate desc", sCondition, MemberID);
                }
                else if (IsAll == 9) // Cancel
                {
                    qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price,
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueMarginV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,CASE WHEN TTQ.Status=2 THEN 'Cancel' ELSE 'Cancel' END as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            OT.OrderType AS OrderType from TradeTransactionQueueMargin TTQ INNER JOIN OrderTypeMaster OT ON  TTQ.OrderType=OT.ID INNER JOIN TransactionQueueMargin TQ ON TTQ.TrnNo=TQ.ID 
                            WHERE {0} AND (TTQ.Status=2 OR (TTQ.Status =1 and TTQ.IsCancelled = 1)) AND TTQ.MemberID={1} Order By TTQ.TrnDate desc", sCondition, MemberID);
                }
                else //settle,cancel,fail
                {
                    qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TSL.ordertype=2 THEN CAST (0 AS DECIMAL(28,18)) WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price, 
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueMarginV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,'Success' as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            OT.OrderType AS OrderType from SettledTradeTransactionQueueMargin TTQ INNER JOIN TradeStopLossMargin TSL ON TSL.TrnNo =TTQ.TrnNo 
                            INNER JOIN TransactionQueueMargin TQ ON TTQ.TrnNo=TQ.ID INNER JOIN OrderTypeMaster OT ON  TSL.OrderType=OT.ID
                            WHERE {0} AND TTQ.Status in (1,4) and TTQ.IsCancelled = 0 AND TTQ.MemberID={1}
                            UNION ALL Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price,
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueMarginV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,CASE WHEN TTQ.Status=2 THEN 'Cancel' ELSE 'Cancel' END as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            OT.OrderType AS OrderType from TradeTransactionQueueMargin TTQ INNER JOIN OrderTypeMaster OT ON  TTQ.OrderType=OT.ID INNER JOIN TransactionQueueMargin TQ ON TTQ.TrnNo=TQ.ID 
                            WHERE {0} AND (TTQ.Status=2 OR (TTQ.Status =1 and TTQ.IsCancelled = 1)) AND TTQ.MemberID={1} Order By TTQ.TrnDate desc", sCondition, MemberID);

                }
                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Result = _dbContext.GetTradeHistoryInfo.FromSql(qry, FromDate, ToDate);
                }
                else
                    Result = _dbContext.GetTradeHistoryInfo.FromSql(qry);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<GetTradeHistoryInfoArbitrageV1> GetTradeHistoryArbitrageV1(long MemberID, string sCondition, string FromDate, string ToDate, int page, int IsAll, long TrnNo = 0)
        {
            IQueryable<GetTradeHistoryInfoArbitrageV1> Result = null;
            string qry = "";
            DateTime fDate, tDate;
            try
            {

                if (IsAll == 1)//success
                {
                    //Uday 24-11-2018 Optimize The Query
                    //komal 30 April 2019 add charge
                    //Rita 22-3-19 order by settledDate
                    //komal 01-7-2019 add taker price average
                    qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TSL.ordertype = 2 THEN CAST(0 AS DECIMAL(28,18)) WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price, 
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueArbitrageV1 WHERE(TakerTrnNo = TTQ.TrnNo OR MakerTrnNo = TTQ.TrnNo) AND STATUS = 1),0) AS SettlementPrice,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,'Success' as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            (select AppTypeName from AppType where id=TQ.LPType) as ExchangeName,
                            OT.OrderType AS OrderType  from SettledTradeTransactionQueueArbitrage TTQ INNER JOIN TradeStopLossArbitrage TSL ON TSL.TrnNo = TTQ.TrnNo INNER JOIN TransactionQueueArbitrage TQ ON TTQ.TrnNo = TQ.ID 
                            INNER JOIN OrderTypeMaster OT ON  TSL.OrderType=OT.ID
                            WHERE {0} AND TTQ.Status in (1, 4) and TTQ.IsCancelled = 0 AND TTQ.MemberID ={1} Order By TTQ.SettledDate desc", sCondition, MemberID);
                }
                else if (IsAll == 2) //system fail
                {
                    //Rita 17-1-19 4:05:00 added Inner join with TradeStopLoss as sCondition parameter required TSL.ordertype
                    //Uday 24-11-2018 Optimize The Query
                    //komal 30 April 2019 add charge
                    //komal 01-7-2019 add taker price average
                    qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price,
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueArbitrageV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,CASE WHEN TTQ.Status=2 THEN 'Cancel' ELSE 'Cancel' END as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            (select AppTypeName from AppType where id=TQ.LPType) as ExchangeName,
                            OT.OrderType AS OrderType from TradeTransactionQueueArbitrage TTQ INNER JOIN OrderTypeMaster OT ON  TTQ.OrderType=OT.ID INNER JOIN TransactionQueueArbitrage TQ ON TTQ.TrnNo=TQ.ID 
                            WHERE {0} AND TTQ.Status=3 AND TTQ.MemberID={1} Order By TTQ.TrnDate desc", sCondition, MemberID);

                }
                else if (IsAll == 9) // Cancel
                {
                    //Rita 17-1-19 4:05:00 added Inner join with TradeStopLoss as sCondition parameter required TSL.ordertype
                    //Uday 24-11-2018 Optimize The Query
                    //komal 30 April 2019 add charge
                    //komal 01-7-2019 add taker price average
                    qry = string.Format(@" Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrencyy,TTQ.SettledDate,
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price,
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueArbitrageV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,CASE WHEN TTQ.Status=2 THEN 'Cancel' ELSE 'Cancel' END as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            (select AppTypeName from AppType where id=TQ.LPType) as ExchangeName,
                            OT.OrderType AS OrderType from TradeTransactionQueueArbitrage TTQ INNER JOIN OrderTypeMaster OT ON  TTQ.OrderType=OT.ID INNER JOIN TransactionQueueArbitrage TQ ON TTQ.TrnNo=TQ.ID 
                            WHERE {0} AND TTQ.IsCancelled=1 AND TTQ.MemberID={1} Order By TTQ.TrnDate desc", sCondition, MemberID);
                }
                else //settle,cancel,fail
                {
                    //Uday 24-11-2018 Optimize The Query
                    //komal 30 April 2019 add charge
                    //komal 01-7-2019 add taker price average
                    qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TSL.ordertype=2 THEN CAST (0 AS DECIMAL(28,18)) WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price, 
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueArbitrageV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            (select AppTypeName from AppType where id=TQ.LPType) as ExchangeName,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,'Success' as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            OT.OrderType AS OrderType from SettledTradeTransactionQueueArbitrage TTQ INNER JOIN TradeStopLossArbitrage TSL ON TSL.TrnNo =TTQ.TrnNo INNER JOIN TransactionQueueArbitrage TQ ON TTQ.TrnNo=TQ.ID INNER JOIN OrderTypeMaster OT ON  TSL.OrderType=OT.ID
                            WHERE {0} AND TTQ.Status in (1,4) and TTQ.IsCancelled = 0 AND TTQ.MemberID={1}
                            UNION ALL Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnDate as DateTime,ISNULL(TQ.Chargecurrency,'') as Chargecurrency,TTQ.SettledDate,
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice Else 0 END as Price,
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueArbitrageV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty Else 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty Else 0 END as SettledQty,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                            (select AppTypeName from AppType where id=TQ.LPType) as ExchangeName,
                            TTQ.Status,TTQ.TrnTypeName as Type,TTQ.PairName,CASE WHEN TTQ.Status=2 THEN 'Cancel' ELSE 'Cancel' END as StatusText,TQ.ChargeRs,TTQ.IsCancelled AS IsCancel,
                            OT.OrderType AS OrderType from TradeTransactionQueueArbitrage TTQ INNER JOIN OrderTypeMaster OT ON  TTQ.OrderType=OT.ID INNER JOIN TransactionQueueArbitrage TQ ON TTQ.TrnNo=TQ.ID 
                            WHERE {0} AND (TTQ.Status=2 OR (TTQ.Status =1 and TTQ.IsCancelled = 1)) AND TTQ.MemberID={1} Order By TTQ.TrnDate desc", sCondition, MemberID);

                }
                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Result = _dbContext.GetTradeHistoryInfoArbitrageV1.FromSql(qry, FromDate, ToDate);
                }
                else
                    Result = _dbContext.GetTradeHistoryInfoArbitrageV1.FromSql(qry);
                return Result.ToList();
            }
            catch (Exception ex)

            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<RecentOrderInfoV1> GetRecentOrderV1(long PairId, long MemberID)
        {
            string sCondition = "";
            IQueryable<RecentOrderInfoV1> Result;
            try
            {
                if (PairId != 999)
                    sCondition = " AND TTQ.PairID =" + PairId;

                //Uday 23-11-2018 Optimize the Query
                //Rita 18-1-19 Remove in above Qry as partial settlement also in SettledTradeTransactionQueue --> OR (TTQ.Status = 1 and TTQ.IsCancelled = 1)
                //change from (TTQ.ordertype<>3 AND TTQ.Status = 4) to (TTQ.ordertype<>3 OR (TTQ.ordertype=3 AND TTQ.Status <> 4))
                //rita 27-6-19 in settledTQ change query line
                //"CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty END as Qty ," +
                string Qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnTypeName as Type,ISNULL(TQ.Chargecurrency,'') as Chargecurrency, TQ.ChargeRs,
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price,
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueV1 WHERE(TakerTrnNo = TTQ.TrnNo OR MakerTrnNo = TTQ.TrnNo) AND STATUS = 1),0) AS SettlementPrice,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty END as Qty , 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty END as SettledQty,TTQ.TrnDate as DateTime,TTQ.SettledDate,
                            case when TTQ.Status = 4  then 'Hold' when TTQ.Status = 2 OR TTQ.Status = 1 then 'Cancel' end as status, 
                            TTQ.Status as StatusCode,OT.OrderType,TTQ.PairName,TTQ.PairId,TSL.ISFollowersReq,cast( 0 as smallint)as IsCancel
                            from TradeTransactionQueue TTQ INNER JOIN TransactionQueue TQ ON TTQ.TrnNo = TQ.Id
                            INNER JOIN TradeStopLoss TSL ON TSL.TrnNo = TTQ.TrnNo INNER JOIN OrderTypeMaster OT ON TTQ.orderType = OT.Id
                            WHERE TTQ.Status in (4, 2) AND(TTQ.ordertype <> 3 OR(TTQ.ordertype = 3 AND TTQ.Status <> 4)) And TTQ.MemberID = {0} AND TTQ.TrnDate > DATEADD(HOUR, -24, dbo.GetISTDate())
                            UNION ALL Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo,TTQ.TrnTypeName as Type,ISNULL(TQ.Chargecurrency,'') as Chargecurrency, TQ.ChargeRs,
                            CASE WHEN TSL.ordertype = 2 THEN CAST(0 AS DECIMAL(28,18)) WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price, 
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueV1 WHERE(TakerTrnNo = TTQ.TrnNo OR MakerTrnNo = TTQ.TrnNo) AND STATUS = 1),0) AS SettlementPrice,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty END as Qty ,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty END as SettledQty,TTQ.TrnDate as DateTime,TTQ.SettledDate,
                            case when TTQ.Status = 1  then 'Success'  end as status ,
                            TTQ.Status as StatusCode,OT.OrderType,TTQ.PairName,TTQ.PairId,TSL.ISFollowersReq,cast( 0 as smallint)as IsCancel
                            from SettledTradeTransactionQueue TTQ INNER JOIN TransactionQueue TQ ON TTQ.TrnNo = TQ.Id
                            INNER JOIN TradeStopLoss TSL ON TSL.TrnNo = TTQ.TrnNo INNER JOIN OrderTypeMaster OT ON TSL.orderType = OT.Id
                            WHERE TTQ.Status in (1)  And TTQ.MemberID = {0} AND TSL.ordertype <> 3 AND TTQ.TrnDate > DATEADD(HOUR, -24, dbo.GetISTDate()) {1} order by TTQ.TrnDate desc", MemberID, sCondition);

                Result = _dbContext.RecentOrderInfoV1.FromSql(Qry);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<RecentOrderInfoV1> GetRecentOrderMarginV1(long PairId, long MemberID)
        {
            string sCondition = "";
            string sCondition1 = "";
            IQueryable<RecentOrderInfoV1> Result;
            try
            {
                if (PairId != 999)
                    sCondition = " AND TTQ.PairID =" + PairId;
                //Rita 01-05-19 margin system order does not display , also cancellation not allowed
                //rita 27-6-19 in settledTQ change query line
                sCondition1 = sCondition;
                sCondition1 += "";

                var Qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo,TTQ.Status as StatusCode,OT.OrderType,TTQ.PairName,TTQ.PairId,TTQ.TrnTypeName as Type,TSL.ISFollowersReq,ISNULL(TQ.Chargecurrency,'') as Chargecurrency, TQ.ChargeRs,
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price, 
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueMarginV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty END as Qty ,cast( 0 as smallint)as IsCancel,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty END as SettledQty,TTQ.SettledDate,
                            TTQ.TrnDate as DateTime,case when TTQ.Status = 4  then 'Hold' when TTQ.Status = 2 OR TTQ.Status = 1 then 'Cancel' end as status 
                            from TradeTransactionQueueMargin TTQ INNER JOIN TransactionQueueMargin TQ ON TTQ.TrnNo = TQ.Id INNER JOIN TradeStopLossMargin TSL ON TSL.TrnNo = TTQ.TrnNo
                            INNER JOIN OrderTypeMaster OT ON TTQ.orderType = OT.Id 
                            WHERE TTQ.Status in (4,2) AND (TTQ.ordertype<>3 OR (TTQ.ordertype=3 AND TTQ.Status <> 4)) And TTQ.MemberID ={0} AND TTQ.TrnDate > DATEADD(HOUR, -24, dbo.GetISTDate())  
                            AND (TTQ.OrderType!=4 OR (TTQ.IsWithoutAmtHold=0 AND TTQ.ISOrderBySystem=0)) 
                            UNION ALL Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo,TTQ.Status as StatusCode,OT.OrderType,TTQ.PairName,TTQ.PairId,TTQ.TrnTypeName as Type,TSL.ISFollowersReq,ISNULL(TQ.Chargecurrency,'') as Chargecurrency, TQ.ChargeRs,
                            CASE WHEN TSL.ordertype=2 THEN CAST (0 AS DECIMAL(28,18)) WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price, 
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueMarginV1 WHERE (TakerTrnNo=TTQ.TrnNo OR MakerTrnNo=TTQ.TrnNo) AND STATUS=1),0) AS SettlementPrice,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty END as Qty ,cast( 0 as smallint)as IsCancel,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty END as SettledQty,TTQ.SettledDate,
                            TTQ.TrnDate as DateTime,case when TTQ.Status = 1  then 'Success'  end as status 
                            from SettledTradeTransactionQueueMargin TTQ INNER JOIN TransactionQueueMargin TQ ON TTQ.TrnNo = TQ.Id
                            INNER JOIN TradeStopLossMargin TSL ON TSL.TrnNo = TTQ.TrnNo INNER JOIN OrderTypeMaster OT ON TSL.orderType = OT.Id 
                            WHERE TTQ.Status in (1)  And TTQ.MemberID ={0} AND TSL.ordertype<>3 AND TTQ.TrnDate > DATEADD(HOUR, -24, dbo.GetISTDate()) {1} order by TTQ.TrnDate desc", MemberID, sCondition);

                if (PairId == 999)
                    Result = _dbContext.RecentOrderInfoV1.FromSql(Qry, MemberID, Convert.ToInt16(enTransactionStatus.Success), Convert.ToInt16(enTransactionStatus.Hold), Convert.ToInt16(enTransactionStatus.OperatorFail));
                else
                    Result = _dbContext.RecentOrderInfoV1.FromSql(Qry, MemberID, Convert.ToInt16(enTransactionStatus.Success), Convert.ToInt16(enTransactionStatus.Hold), Convert.ToInt16(enTransactionStatus.OperatorFail), PairId);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<RecentOrderInfoArbitrageV1> GetRecentOrderArbitrageV1(long PairId, long MemberID)
        {
            string sCondition = "";
            IQueryable<RecentOrderInfoArbitrageV1> Result;
            try
            {
                if (PairId != 999)
                    sCondition = " AND TTQ.PairID =" + PairId;

                //Uday 23-11-2018 Optimize the Query
                //Rita 18-1-19 Remove in above Qry as partial settlement also in SettledTradeTransactionQueue --> OR (TTQ.Status = 1 and TTQ.IsCancelled = 1)
                //change from (TTQ.ordertype<>3 AND TTQ.Status = 4) to (TTQ.ordertype<>3 OR (TTQ.ordertype=3 AND TTQ.Status <> 4))
                //rita 27-6-19 in settledTQ change query line
                string Qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo, TTQ.TrnTypeName as Type,ISNULL(TQ.Chargecurrenc
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price,
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueV1 WHERE(TakerTrnNo = TTQ.TrnNo OR MakerTrnNo = TTQ.TrnNo) AND STATUS = 1),0) AS SettlementPrice,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty END as Qty , 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty END as SettledQty,TTQ.TrnDate as DateTime,TTQ.SettledDate,
                            case when TTQ.Status = 4  then 'Hold' when TTQ.Status = 2 OR TTQ.Status = 1 then 'Cancel' end as status, 
                            (select AppTypeName from AppType where id=TQ.LPType) as ExchangeName,
                            TTQ.Status as StatusCode,OT.OrderType,TTQ.PairName,TTQ.PairId,TSL.ISFollowersReq,cast( 0 as smallint)as IsCancel
                            from TradeTransactionQueueArbitrage TTQ INNER JOIN TransactionQueueArbitrage TQ ON TTQ.TrnNo = TQ.Id
                            INNER JOIN TradeStopLossArbitrage TSL ON TSL.TrnNo = TTQ.TrnNo INNER JOIN OrderTypeMaster OT ON TTQ.orderType = OT.Id
                            WHERE TTQ.Status in (4, 2) AND(TTQ.ordertype <> 3 OR(TTQ.ordertype = 3 AND TTQ.Status <> 4)) And TTQ.MemberID = {0} AND TTQ.TrnDate > DATEADD(HOUR, -24, dbo.GetISTDate())
                            UNION ALL Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as TrnNo,TTQ.TrnTypeName as Type,
                            CASE WHEN TSL.ordertype = 2 THEN CAST(0 AS DECIMAL(28,18)) WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price, 
                            ISNULL((SELECT avg(TakerPrice) FROM TradePoolQueueArbitrageV1 WHERE(TakerTrnNo = TTQ.TrnNo OR MakerTrnNo = TTQ.TrnNo) AND STATUS = 1),0) AS SettlementPrice,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty END as Qty ,
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty END as SettledQty,TTQ.TrnDate as DateTime,TTQ.SettledDate,
                            case when TTQ.Status = 1  then 'Success'  end as status ,
                            (select AppTypeName from AppType where id=TQ.LPType) as ExchangeName,
                            TTQ.Status as StatusCode,OT.OrderType,TTQ.PairName,TTQ.PairId,TSL.ISFollowersReq,cast( 0 as smallint)as IsCancel
                            from SettledTradeTransactionQueueArbitrage TTQ INNER JOIN TransactionQueueArbitrage TQ ON TTQ.TrnNo = TQ.Id
                            INNER JOIN TradeStopLossArbitrage TSL ON TSL.TrnNo = TTQ.TrnNo INNER JOIN OrderTypeMaster OT ON TSL.orderType = OT.Id
                            WHERE TTQ.Status in (1)  And TTQ.MemberID = {0} AND TSL.ordertype <> 3 AND TTQ.TrnDate > DATEADD(HOUR, -24, dbo.GetISTDate()) {1} order by TTQ.TrnDate desc", MemberID, sCondition);

                Result = _dbContext.RecentOrderInfoArbitrageV1.FromSql(Qry);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ActiveOrderInfoV1> GetActiveOrderV1(long MemberID, string FromDate, string ToDate, long PairId, short trnType)
        {
            string Qry = "";
            string sCondition = " ";
            IQueryable<ActiveOrderInfoV1> Result;
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                if (PairId != 999)
                    sCondition += " AND TTQ.PairId =" + PairId;
                if (trnType != 999)
                    sCondition += " AND TTQ.TrnType =" + trnType;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCondition += " AND TTQ.TrnDate Between {0} AND {1} ";
                }
                //Uday 23-11-2018 Optimize the query
                //komal 12-01-2018 remove stop-limit order condition 
                //Rita 12-3-19 added for needed at front side
                Qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as ID,OT.ordertype,TTQ.PairName,TTQ.PairId,TTQ.TrnDate,TTQ.TrnTypeName as Type,TTQ.Order_Currency,TTQ.Delivery_Currency, ISNULL(TQ.Chargecurrency,'') as Chargecurrency, TQ.ChargeRs,
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty END as SettledQty,TTQ.SettledDate,
                            TTQ.IsCancelled from TradeTransactionQueue TTQ   INNER join TradePairStastics TPS on TTQ.PairID = TPS.PairId 
                            INNER JOIN TransactionQueue TQ ON TTQ.TrnNo=TQ.Id INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.Id
                            where TTQ.Status =4 AND TTQ.MemberID = {1} AND TTQ.ordertype<>3 {0} Order By TTQ.TrnDate desc", sCondition, MemberID);
                Result = _dbContext.ActiveOrderInfoV1.FromSql(Qry, fDate, tDate);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<ActiveOrderInfoV1> GetActiveOrderMarginV1(long MemberID, string FromDate, string ToDate, long PairId, short trnType)
        {
            string Qry = "";
            string sCondition = " ";
            IQueryable<ActiveOrderInfoV1> Result;
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                if (PairId != 999)
                    sCondition += " AND TTQ.PairId =" + PairId;
                if (trnType != 999)
                    sCondition += " AND TTQ.TrnType =" + trnType;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCondition += " AND TTQ.TrnDate Between {2} AND {3} ";
                }
                //Rita 01-05-19 margin system order does not display , also cancellation not allowed
                sCondition += " AND (TTQ.OrderType!=4 OR (TTQ.IsWithoutAmtHold=0 AND TTQ.ISOrderBySystem=0))";

                //Rita 12-3-19 added for needed at front side
                //komal 20-09-2019 added ChargeRs and ChargeCurrency as need at front
                Qry = "Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as ID,OT.OrderType,TTQ.PairName,TTQ.PairId,TTQ.TrnDate,TTQ.TrnTypeName as Type,TTQ.Order_Currency,TTQ.Delivery_Currency, ISNULL(TQ.Chargecurrency,'') as Chargecurrency, TQ.ChargeRs,  " +
                   "CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount, " +
                   "CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price, " +
                   "CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty END as SettledQty,TTQ.SettledDate," +
                   "TTQ.IsCancelled from TradeTransactionQueueMargin TTQ  INNER join TradeStopLossMargin TSL on TTQ.TrnNo = TSL.TrnNo INNER JOIN TransactionQueueMargin TQ ON TTQ.TrnNo=TQ.Id " +
                   "INNER join TradePairStasticsMargin TPS on TTQ.PairID = TPS.PairId INNER JOIN OrderTypeMaster OT ON TSL.orderType = OT.Id  " +
                   "where TTQ.Status = {1} AND TTQ.MemberID = {0} AND TTQ.ordertype<>3 " + sCondition + " Order By TTQ.TrnDate desc";

                Result = _dbContext.ActiveOrderInfoV1.FromSql(Qry, MemberID, Convert.ToInt16(enTransactionStatus.Hold), fDate, tDate);

                return Result.ToList();

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<ActiveOrderInfoArbitrageV1> GetActiveOrderArbitrageV1(long MemberID, string FromDate, string ToDate, long PairId, short trnType)
        {
            string Qry = "";
            string sCondition = " ";
            IQueryable<ActiveOrderInfoArbitrageV1> Result;
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                if (PairId != 999)
                    sCondition += " AND TTQ.PairId =" + PairId;
                if (trnType != 999)
                    sCondition += " AND TTQ.TrnType =" + trnType;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCondition += " AND TTQ.TrnDate Between {0} AND {1} ";
                }
                //Uday 23-11-2018 Optimize the query
                //komal 12-01-2018 remove stop-limit order condition 
                //Rita 12-3-19 added for needed at front side
                Qry = string.Format(@"Select CAST(TQ.GUID as varchar(50)) as GUID,cast(TQ.id as varchar(50)) as ID,OT.ordertype,TTQ.PairName,TTQ.PairId,TTQ.TrnDate,TTQ.TrnTypeName as Type,TTQ.Order_Currency,TTQ.Delivery_Currency , ISNULL(TQ.Chargecurrency,'') as Chargecurrency, TQ.ChargeRs, 
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount, 
                            CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price, 
                            CASE WHEN TTQ.TrnType = 4 THEN TTQ.SettledBuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SettledSellQty END as SettledQty,TTQ.SettledDate,
                            (select AppTypeName from AppType where id=TQ.LPType) as ExchangeName,
                            TTQ.IsCancelled from TradeTransactionQueueArbitrage TTQ   INNER join TradePairStasticsArbitrage TPS on TTQ.PairID = TPS.PairId 
                            INNER JOIN TransactionQueueArbitrage TQ ON TTQ.TrnNo=TQ.Id INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.Id
                            where TTQ.Status =4 AND TTQ.MemberID = {1} AND TTQ.ordertype<>3 {0} Order By TTQ.TrnDate desc", sCondition, MemberID);
                Result = _dbContext.ActiveOrderInfoArbitrageV1.FromSql(Qry, fDate, tDate);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        #endregion

        #region Trade data Method

        public long GetPairIdByName(string pair)
        {

            try
            {
                var model = _dbContext.TradePairMaster.Where(p => p.PairName == pair && p.Status == 1).FirstOrDefault();
                if (model == null)
                    return 0;

                return model.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<GetBuySellBook> GetBuyerBook(long id, decimal Price = -1)
        {
            try
            {
                IQueryable<GetBuySellBook> Result;
                //Uday  05-11-2018 As Per Instruction by ritamam not get the OrderId From TradePoolMaster
                //Uday 19 - 11 - 2018 As Per ritamam instruction get all status record but check condition sum(TTQ.OrderTotalQty) -Sum(TTQ.SettledSellQty)
                //komal 29-12-2018 modify for Stop-limit order
                //Uday 03-01-2019 add condition for amount > 0
                //komal 05-02-2019 add AND TTQ.IsAPITrade=0 
                if (Price != -1)//SignalR call
                {
                    //Rita 16-1-19 OrderType=4 goes Separate , so remove from here
                    Result = _dbContext2.BuyerSellerInfo.FromSql(
                                  @"Select Top 1 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsStopLimit 
                                    From TradeTransactionQueue TTQ  INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo 
                                    Where (TTQ.ordertype != 4 ) AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.PairID ={0} AND TTQ.IsAPITrade=0 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.BidPrice={1}
                                    Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 Order By TTQ.BidPrice desc", id, Price);
                }
                else//API call
                {
                    //Rita 16-1-19 separate Order Type=4 records with bit , front side handle separate array
                    Result = _dbContext2.BuyerSellerInfo.FromSql(
                                  @"SELECT Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsStopLimit
                                    FROM TradeTransactionQueue TTQ  INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo 
                                    WHERE TTQ.ordertype <> 4 AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.PairID ={0} AND TTQ.IsAPITrade=0 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                                    GROUP By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 UNION ALL
                                    SELECT Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsStopLimit
                                    FROM TradeTransactionQueue TTQ  INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo INNER join TradePairStastics TPS on TTQ.PairID=TPS.PairId
                                    WHERE (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= TPS.LTP)) AND TTQ.ordertype = 4) 
                                    AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0} AND TTQ.IsAPITrade=0
                                    GROUP By TTQ.BidPrice,TTQ.PairID HAVING (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 Order By TTQ.BidPrice desc", id);
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " #PairId# : " + id + " #Price# : " + Price, this.GetType().Name, ex);
                return new List<GetBuySellBook>();
            }
        }
        //Rita 20-2-19 for Margin Trading
        public List<GetBuySellBook> GetBuyerBookMargin(long id, decimal Price = -1)
        {
            try
            {
                IQueryable<GetBuySellBook> Result;

                if (Price != -1)//SignalR call
                {
                    Result = _dbContext2.BuyerSellerInfo.FromSql(
                                  @"Select Top 1 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsStopLimit 
                                    From TradeTransactionQueueMargin TTQ  INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo 
                                    Where (TTQ.ordertype != 4 ) AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.PairID ={0} AND TTQ.IsAPITrade=0 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.BidPrice={1}
                                    Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 Order By TTQ.BidPrice desc", id, Price);
                }
                else//API call
                {
                    Result = _dbContext2.BuyerSellerInfo.FromSql(
                                  @"SELECT Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsStopLimit
                                    FROM TradeTransactionQueueMargin TTQ  INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo 
                                    WHERE TTQ.ordertype <> 4 AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.PairID ={0} AND TTQ.IsAPITrade=0 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                                    GROUP By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 UNION ALL
                                    SELECT Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsStopLimit
                                    FROM TradeTransactionQueueMargin TTQ  INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo INNER join TradePairStasticsMargin TPS on TTQ.PairID=TPS.PairId
                                    WHERE (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= TPS.LTP)) AND TTQ.ordertype = 4) 
                                    AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0} AND TTQ.IsAPITrade=0
                                    GROUP By TTQ.BidPrice,TTQ.PairID HAVING (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 Order By TTQ.BidPrice desc", id);
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " #PairId# : " + id + " #Price# : " + Price, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetBuySellBook> GetSellerBook(long id, decimal Price = -1)
        {
            try
            {
                IQueryable<GetBuySellBook> Result;
                //Uday  05-11-2018 As Per Instruction by ritamam not get the OrderId From TradePoolMaster
                //Uday 19 - 11 - 2018 As Per ritamam instruction get all status record but check condition sum(TTQ.OrderTotalQty) -Sum(TTQ.SettledSellQty)
                //komal 29-12-2018 modify for Stop-limit order
                //Uday 03-01-2019 add condition for amount > 0
                //komal 05-02-2019 add AND TTQ.IsAPITrade=0 
                if (Price != -1)
                {
                    //Rita 16-1-19 OrderType=4 goes Separate , so remove from here
                    Result = _dbContext2.BuyerSellerInfo.FromSql(
                                @"SELECT Top 1 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,
                                Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsStopLimit 
                                FROM TradeTransactionQueue TTQ INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo
                                WHERE (TTQ.ordertype != 4 ) AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsAPITrade=0 AND TTQ.AskPrice={1}
                                AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 order by TTQ.AskPrice", id, Price);
                }
                else//API Call
                {
                    //Rita 16-1-19 separate Order Type=4 records with bit , front side handle separate array
                    Result = _dbContext2.BuyerSellerInfo.FromSql(
                              @"SELECT Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsStopLimit
                                    FROM TradeTransactionQueue TTQ INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo 
                                    WHERE TTQ.ordertype != 4 AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsAPITrade=0
                                    AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 UNION ALL
                                    SELECT TOP 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsStopLimit
                                    FROM TradeTransactionQueue TTQ INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo INNER JOIN TradePairStastics TPS on TTQ.PairID=TPS.PairId
                                    WHERE (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= TPS.LTP)) AND TTQ.ordertype = 4) 
                                    AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsAPITrade=0 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                                    GROUP by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 order by TTQ.AskPrice", id);
                }

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " #PairId# : " + id + " #Price# : " + Price, this.GetType().Name, ex);
                return new List<GetBuySellBook>();
            }
        }
        //Rita 20-2-19 for Margin Trading
        public List<GetBuySellBook> GetSellerBookMargin(long id, decimal Price = -1)
        {
            try
            {
                IQueryable<GetBuySellBook> Result;
                if (Price != -1)
                {
                    Result = _dbContext2.BuyerSellerInfo.FromSql(
                                @"SELECT Top 1 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,
                                Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsStopLimit 
                                FROM TradeTransactionQueueMargin TTQ INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo
                                WHERE (TTQ.ordertype != 4 ) AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsAPITrade=0 AND TTQ.AskPrice={1}
                                AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 order by TTQ.AskPrice", id, Price);
                }
                else
                {
                    Result = _dbContext2.BuyerSellerInfo.FromSql(
                               @"SELECT Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsStopLimit
                                    FROM TradeTransactionQueueMargin TTQ INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo 
                                    WHERE TTQ.ordertype != 4 AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsAPITrade=0
                                    AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 UNION ALL
                                    SELECT TOP 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsStopLimit
                                    FROM TradeTransactionQueueMargin TTQ INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo INNER JOIN TradePairStasticsMargin TPS on TTQ.PairID=TPS.PairId
                                    WHERE (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= TPS.LTP)) AND TTQ.ordertype = 4) 
                                    AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsAPITrade=0 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                                    GROUP by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 order by TTQ.AskPrice", id);
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " #PairId# : " + id + " #Price# : " + Price, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<StopLimitBuySellBook> GetStopLimitBuySellBooks(decimal LTP, long Pair, enOrderType OrderType, short IsCancel = 0)
        {
            try
            {
                IQueryable<StopLimitBuySellBook> Result = null;
                if (IsCancel == 0)
                {
                    if (OrderType == enOrderType.BuyOrder)
                    {
                        Result = _dbContext2.StopLimitBuyerSellerBook.FromSql(
                            @"Select Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsAdd
                            From TradeTransactionQueue TTQ  INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo 
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= {1}) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= {1})) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0}
                            Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 UNION ALL
                            Select Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsAdd
                            From TradeTransactionQueue TTQ  INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo 
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice < {1}) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice > {1})) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0}
                            Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 Order By TTQ.BidPrice desc"
                            , Pair, LTP);
                    }
                    else if (OrderType == enOrderType.SellOrder)
                    {
                        Result = _dbContext2.StopLimitBuyerSellerBook.FromSql(
                            @"Select Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsAdd
                            from TradeTransactionQueue TTQ INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo 
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= {1}) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= {1})) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                            Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 UNION ALL
                            Select Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsAdd
                            from TradeTransactionQueue TTQ INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo 
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice < {1}) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice > {1})) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                            Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 order by TTQ.AskPrice"
                            , Pair, LTP);
                    }
                }
                else
                {
                    if (OrderType == enOrderType.BuyOrder)
                    {
                        Result = _dbContext2.StopLimitBuyerSellerBook.FromSql(
                            @"Select Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsAdd
                            From TradeTransactionQueue TTQ  INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo INNER Join TradePairStastics TPS on TTQ.PairID=TPS.PairId
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= TPS.LTP)) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0}
                            Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 UNION ALL
                            Select Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsAdd
                            From TradeTransactionQueue TTQ  INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo  INNER Join TradePairStastics TPS on TTQ.PairID=TPS.PairId
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice < TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice > TPS.LTP)) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0}
                            Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 Order By TTQ.BidPrice desc"
                            , Pair, LTP);
                    }
                    else if (OrderType == enOrderType.SellOrder)
                    {
                        Result = _dbContext2.StopLimitBuyerSellerBook.FromSql(
                            @"Select Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsAdd
                            from TradeTransactionQueue TTQ INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo INNER Join TradePairStastics TPS on TTQ.PairID=TPS.PairId
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= TPS.LTP)) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                            Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 UNION ALL
                            Select Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsAdd
                            from TradeTransactionQueue TTQ INNER join TradeStopLoss TSL on TTQ.TrnNo=TSL.TrnNo INNER Join TradePairStastics TPS on TTQ.PairID=TPS.PairId
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice < TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice > TPS.LTP)) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                            Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 order by TTQ.AskPrice"
                            , Pair, LTP);
                    }
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 20-2-19 for Margin Trading
        public List<StopLimitBuySellBook> GetStopLimitBuySellBooksMargin(decimal LTP, long Pair, enOrderType OrderType, short IsCancel = 0)
        {
            try
            {
                IQueryable<StopLimitBuySellBook> Result = null;
                if (IsCancel == 0)
                {
                    if (OrderType == enOrderType.BuyOrder)
                    {
                        Result = _dbContext2.StopLimitBuyerSellerBook.FromSql(
                            @"Select Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsAdd
                            From TradeTransactionQueueMargin TTQ  INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo 
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= {1}) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= {1})) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0}
                            Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 UNION ALL
                            Select Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsAdd
                            From TradeTransactionQueueMargin TTQ  INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo 
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice < {1}) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice > {1})) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0}
                            Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 Order By TTQ.BidPrice desc"
                            , Pair, LTP);
                    }
                    else if (OrderType == enOrderType.SellOrder)
                    {
                        Result = _dbContext2.StopLimitBuyerSellerBook.FromSql(
                            @"Select Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsAdd
                            from TradeTransactionQueueMargin TTQ INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo 
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= {1}) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= {1})) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                            Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 UNION ALL
                            Select Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsAdd
                            from TradeTransactionQueueMargin TTQ INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo 
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice < {1}) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice > {1})) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                            Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 order by TTQ.AskPrice"
                            , Pair, LTP);
                    }
                }
                else
                {
                    if (OrderType == enOrderType.BuyOrder)
                    {
                        Result = _dbContext2.StopLimitBuyerSellerBook.FromSql(
                            @"Select Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsAdd
                            From TradeTransactionQueueMargin TTQ  INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo INNER Join TradePairStasticsMargin TPS on TTQ.PairID=TPS.PairId
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= TPS.LTP)) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0}
                            Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 UNION ALL
                            Select Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsAdd
                            From TradeTransactionQueueMargin TTQ  INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo  INNER Join TradePairStasticsMargin TPS on TTQ.PairID=TPS.PairId
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice < TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice > TPS.LTP)) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0}
                            Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 Order By TTQ.BidPrice desc"
                            , Pair, LTP);
                    }
                    else if (OrderType == enOrderType.SellOrder)
                    {
                        Result = _dbContext2.StopLimitBuyerSellerBook.FromSql(
                            @"Select Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsAdd
                            from TradeTransactionQueueMargin TTQ INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo INNER Join TradePairStasticsMargin TPS on TTQ.PairID=TPS.PairId
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= TPS.LTP)) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                            Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 UNION ALL
                            Select Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsAdd
                            from TradeTransactionQueueMargin TTQ INNER join TradeStopLossMargin TSL on TTQ.TrnNo=TSL.TrnNo INNER Join TradePairStasticsMargin TPS on TTQ.PairID=TPS.PairId
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice < TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice > TPS.LTP)) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                            Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 order by TTQ.AskPrice"
                            , Pair, LTP);
                    }
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetGraphDetailInfo> GetGraphData(long id, int IntervalTime, string IntervalData, DateTime Minute, int socket = 0)
        {
            try
            {
                string Query = "";
                IQueryable<GetGraphDetailInfo> Result;
                if (socket == 0)
                {
                    //Uday 30-01-2019 Give Data In Proper Interval wise
                    Query = "SELECT CONVERT(BIGINT,DATEDIFF(ss,'01-01-1970 00:00:00',DATEADD(#IntervalData#, DATEDIFF(#IntervalData#, 0, CreatedDate) / #IntervalTime# * #IntervalTime#, 0))) * 1000 DataDate," +
                         "MAX(LTP) AS High, MIN(LTP) AS Low, SUM(Quantity * LTP) AS Volume," +
                         "(SELECT LTP FROM dbo.TradeGraphDetail AS T1 WHERE(T1.Id = MIN(T.Id))) AS[Open]," +
                         "(SELECT LTP FROM dbo.TradeGraphDetail AS T1 WHERE(T1.Id = MAX(T.Id))) AS[Close]" +
                         " FROM dbo.TradeGraphDetail AS T Where T.PairId = {0} " +
                         " GROUP BY DATEADD(#IntervalData#, DATEDIFF(#IntervalData#, 0, CreatedDate) / #IntervalTime# * #IntervalTime#, 0)";

                    Query = Query.Replace("#IntervalData#", IntervalData).Replace("#IntervalTime#", IntervalTime.ToString());
                    Result = _dbContext.GetGraphResponse.FromSql(Query, id);
                }
                else
                {
                    Query = "SELECT CONVERT(BIGINT,DATEDIFF(ss,'01-01-1970 00:00:00',DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0))) * 1000 DataDate," +
                        "MAX(LTP) AS High, MIN(LTP) AS Low, SUM(Quantity * LTP) AS Volume," +
                        "(SELECT LTP FROM dbo.TradeGraphDetail AS T1 WHERE(T1.Id = MIN(T.Id))) AS[Open]," +
                        "(SELECT LTP FROM dbo.TradeGraphDetail AS T1 WHERE(T1.Id = MAX(T.Id))) AS[Close]" +
                        " FROM dbo.TradeGraphDetail AS T Where T.PairId = {0} And DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0) = {1}" +
                        " GROUP BY DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0)";

                    string MinuteData = Minute.ToString("yyyy-MM-dd HH:mm:00:000");
                    Result = _dbContext.GetGraphResponse.FromSql(Query, id, MinuteData);
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetGraphDetailInfo> GetGraphDataMargin(long id, int IntervalTime, string IntervalData, DateTime Minute, int socket = 0)
        {
            try
            {
                string Query = "";
                IQueryable<GetGraphDetailInfo> Result;
                if (socket == 0)
                {

                    Query = "SELECT CONVERT(BIGINT,DATEDIFF(ss,'01-01-1970 00:00:00',DATEADD(#IntervalData#, DATEDIFF(#IntervalData#, 0, DataDate) / #IntervalTime# * #IntervalTime#, 0))) * 1000 DataDate," +
                          "MAX(LTP) AS High, MIN(LTP) AS Low, SUM(Quantity * LTP) AS Volume," +
                          "(SELECT LTP FROM dbo.TradeGraphDetailMargin AS T1 WHERE(T1.TranNo = MIN(T.TranNo))) AS[Open]," +
                          "(SELECT LTP FROM dbo.TradeGraphDetailMargin AS T1 WHERE(T1.TranNo = MAX(T.TranNo))) AS[Close]" +
                          " FROM dbo.TradeGraphDetailMargin AS T Where T.PairId = {0} " +
                          " GROUP BY DATEADD(#IntervalData#, DATEDIFF(#IntervalData#, 0, DataDate) / #IntervalTime# * #IntervalTime#, 0) Order By DataDate";

                    Query = Query.Replace("#IntervalData#", IntervalData).Replace("#IntervalTime#", IntervalTime.ToString());
                    Result = _dbContext.GetGraphResponse.FromSql(Query, id);
                }
                else
                {
                    Query = "SELECT CONVERT(BIGINT,DATEDIFF(ss,'01-01-1970 00:00:00',DATEADD(MINUTE, DATEDIFF(MINUTE, 0, DataDate) / 1 * 1, 0))) DataDate," +
                          "MAX(LTP) AS High, MIN(LTP) AS Low, SUM(Quantity * LTP) AS Volume," +
                          "(SELECT LTP FROM dbo.TradeGraphDetailMargin AS T1 WHERE(T1.TranNo = MIN(T.TranNo))) AS[Open]," +
                          "(SELECT LTP FROM dbo.TradeGraphDetailMargin AS T1 WHERE(T1.TranNo = MAX(T.TranNo))) AS[Close]" +
                          " FROM dbo.TradeGraphDetailMargin AS T Where T.PairId = {0} And DATEADD(MINUTE, DATEDIFF(MINUTE, 0, DataDate) / 1 * 1, 0) = {1}" +
                          " GROUP BY DATEADD(MINUTE, DATEDIFF(MINUTE, 0, DataDate) / 1 * 1, 0) Order By DataDate";

                    string MinuteData = Minute.ToString("yyyy-MM-dd HH:mm:00:000");
                    Result = _dbContext.GetGraphResponse.FromSql(Query, id, MinuteData);
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public decimal LastPriceByPair(long PairId, ref short UpDownBit)
        {
            try
            {
                Decimal lastPrice = 0;
                var pairStastics = _dbContext.TradePairStastics.Where(x => x.PairId == PairId).SingleOrDefault();
                UpDownBit = pairStastics.UpDownBit;
                lastPrice = pairStastics.LTP;
                return lastPrice;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public PairRatesResponse GetPairRates(long PairId)
        {
            try
            {
                decimal BuyPrice = 0;
                decimal SellPrice = 0;
                PairRatesResponse response = new PairRatesResponse();
                var BidPriceRes = _dbContext.SettledTradeTransactionQueue.Where(e => e.TrnType == Convert.ToInt16(enTrnType.Buy_Trade) && e.Status == Convert.ToInt16(enTransactionStatus.Success) && e.PairID == PairId).OrderByDescending(e => e.TrnNo).FirstOrDefault();
                if (BidPriceRes != null)
                {
                    BuyPrice = BidPriceRes.BidPrice;
                }
                else
                {
                    BuyPrice = 0;
                }

                var AskPriceRes = _dbContext.TradeTransactionQueue.Where(e => e.TrnType == Convert.ToInt16(enTrnType.Sell_Trade) && e.Status == Convert.ToInt16(enTransactionStatus.Success) && e.PairID == PairId).OrderByDescending(e => e.TrnNo).FirstOrDefault();
                if (AskPriceRes != null)
                {
                    SellPrice = AskPriceRes.AskPrice;
                }
                else
                {
                    SellPrice = 0;
                }

                var PairResponse = _dbContext.TradePairDetail.Where(e => e.PairId == PairId).FirstOrDefault();

                if (PairResponse != null)
                {
                    if (BuyPrice == 0)
                    {
                        response.BuyPrice = PairResponse.BuyPrice;
                    }
                    else
                    {
                        response.BuyPrice = BuyPrice;
                    }
                    if (SellPrice == 0)
                    {
                        response.SellPrice = PairResponse.SellPrice;
                    }
                    else
                    {
                        response.SellPrice = SellPrice;
                    }
                    response.BuyMaxPrice = PairResponse.BuyMaxPrice;
                    response.BuyMinPrice = PairResponse.BuyMinPrice;
                    response.SellMaxPrice = PairResponse.SellMaxPrice;
                    response.SellMinPrice = PairResponse.SellMinPrice;
                }

                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 20-2-19 for Margin Trading
        public PairRatesResponse GetPairRatesMargin(long PairId)
        {
            try
            {
                decimal BuyPrice = 0;
                decimal SellPrice = 0;
                PairRatesResponse response = new PairRatesResponse();
                var BidPriceRes = _dbContext.SettledTradeTransactionQueueMargin.Where(e => e.TrnType == Convert.ToInt16(enTrnType.Buy_Trade) && e.Status == Convert.ToInt16(enTransactionStatus.Success) && e.PairID == PairId).OrderByDescending(e => e.TrnNo).FirstOrDefault();
                if (BidPriceRes != null)
                {
                    BuyPrice = BidPriceRes.BidPrice;
                }
                else
                {
                    BuyPrice = 0;
                }

                var AskPriceRes = _dbContext.TradeTransactionQueueMargin.Where(e => e.TrnType == Convert.ToInt16(enTrnType.Sell_Trade) && e.Status == Convert.ToInt16(enTransactionStatus.Success) && e.PairID == PairId).OrderByDescending(e => e.TrnNo).FirstOrDefault();
                if (AskPriceRes != null)
                {
                    SellPrice = AskPriceRes.AskPrice;
                }
                else
                {
                    SellPrice = 0;
                }

                var PairResponse = _dbContext.TradePairDetailMargin.Where(e => e.PairId == PairId).FirstOrDefault();

                if (PairResponse != null)
                {
                    if (BuyPrice == 0)
                    {
                        response.BuyPrice = PairResponse.BuyPrice;
                    }
                    else
                    {
                        response.BuyPrice = BuyPrice;
                    }
                    if (SellPrice == 0)
                    {
                        response.SellPrice = PairResponse.SellPrice;
                    }
                    else
                    {
                        response.SellPrice = SellPrice;
                    }
                    response.BuyMaxPrice = PairResponse.BuyMaxPrice;
                    response.BuyMinPrice = PairResponse.BuyMinPrice;
                    response.SellMaxPrice = PairResponse.SellMaxPrice;
                    response.SellMinPrice = PairResponse.SellMinPrice;
                }

                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TradePairTableResponse> GetTradePairAsset(long BaseId = 0)
        {
            try
            {
                IQueryable<TradePairTableResponse> Result;

                if (BaseId == 0)
                {
                    Result = _dbContext.TradePairTableResponse.FromSql(
                                @"Select ISNULL(TPD.QtyLength,0) as QtyLength,ISNULL(TPD.PriceLength,0) as PriceLength,ISNULL(TPD.AmtLength,0) as AmtLength,SM1.Id As BaseId,SM1.Name As BaseName,SM1.SMSCode As BaseCode,TPM.ID As PairId,TPM.PairName As Pairname,TPS.CurrentRate As Currentrate,TPD.BuyFees As BuyFees,TPD.SellFees As SellFees,
                                    SM2.Name As ChildCurrency,SM2.SMSCode As Abbrevation,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,TPS.High24Hr AS High24Hr,TPS.Low24Hr As Low24Hr,TPD.PairPercentage,
                                    TPS.HighWeek As HighWeek,TPS.LowWeek As LowWeek,TPS.High52Week AS High52Week,TPS.Low52Week As Low52Week,TPS.UpDownBit As UpDownBit,TPM.Priority from Market M 
                                    Inner Join TradePairMaster TPM ON TPM.BaseCurrencyId = M.ServiceID
                                    Inner Join TradePairDetail TPD ON TPD.PairId = TPM.Id
                                    Inner Join TradePairStastics TPS ON TPS.PairId = TPM.Id
                                    Inner Join ServiceMaster SM1 ON SM1.Id = TPM.BaseCurrencyId
                                    Inner Join ServiceMaster SM2 ON SM2.Id = TPM.SecondaryCurrencyId Where TPM.Status = 1 And M.Status = 1 Order By M.ID");

                }
                else
                {
                    Result = _dbContext.TradePairTableResponse.FromSql(
                                @"Select ISNULL(TPD.QtyLength,0) as QtyLength,ISNULL(TPD.PriceLength,0) as PriceLength,ISNULL(TPD.AmtLength,0) as AmtLength,SM1.Id As BaseId,SM1.Name As BaseName,SM1.SMSCode As BaseCode,TPM.ID As PairId,TPM.PairName As Pairname,TPS.CurrentRate As Currentrate,TPD.BuyFees As BuyFees,TPD.SellFees As SellFees,
                                    SM2.Name As ChildCurrency,SM2.SMSCode As Abbrevation,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,TPS.High24Hr AS High24Hr,TPS.Low24Hr As Low24Hr,TPD.PairPercentage,
                                    TPS.HighWeek As HighWeek,TPS.LowWeek As LowWeek,TPS.High52Week AS High52Week,TPS.Low52Week As Low52Week,TPS.UpDownBit As UpDownBit,TPM.Priority from Market M 
                                    Inner Join TradePairMaster TPM ON TPM.BaseCurrencyId = M.ServiceID
                                    Inner Join TradePairDetail TPD ON TPD.PairId = TPM.Id
                                    Inner Join TradePairStastics TPS ON TPS.PairId = TPM.Id
                                    Inner Join ServiceMaster SM1 ON SM1.Id = TPM.BaseCurrencyId
                                    Inner Join ServiceMaster SM2 ON SM2.Id = TPM.SecondaryCurrencyId Where TPM.Status = 1 And M.Status = 1 And M.ServiceID = {0}", BaseId);
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 22-2-19 for Margin Trading
        public List<TradePairTableResponse> GetTradePairAssetMargin(long BaseId = 0)
        {
            try
            {
                IQueryable<TradePairTableResponse> Result;

                if (BaseId == 0)
                {
                    Result = _dbContext.TradePairTableResponse.FromSql(
                                @"Select 1 as QtyLength,1 as PriceLength,1 as AmtLength,SM1.Id As BaseId,SM1.Name As BaseName,SM1.SMSCode As BaseCode,TPM.ID As PairId,TPM.PairName As Pairname,TPS.CurrentRate As Currentrate,TPD.BuyFees As BuyFees,TPD.SellFees As SellFees,
                                    SM2.Name As ChildCurrency,SM2.SMSCode As Abbrevation,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,TPS.High24Hr AS High24Hr,TPS.Low24Hr As Low24Hr,CAST(0 AS DECIMAL) AS PairPercentage,
                                    TPS.HighWeek As HighWeek,TPS.LowWeek As LowWeek,TPS.High52Week AS High52Week,TPS.Low52Week As Low52Week,TPS.UpDownBit As UpDownBit,TPM.Priority from MarketMargin M 
                                    Inner Join TradePairMasterMargin TPM ON TPM.BaseCurrencyId = M.ServiceID
                                    Inner Join TradePairDetailMargin TPD ON TPD.PairId = TPM.Id
                                    Inner Join TradePairStasticsMargin TPS ON TPS.PairId = TPM.Id
                                    Inner Join ServiceMasterMargin SM1 ON SM1.Id = TPM.BaseCurrencyId
                                    Inner Join ServiceMasterMargin SM2 ON SM2.Id = TPM.SecondaryCurrencyId Where TPM.Status = 1 And M.Status = 1 Order By M.ID");

                }
                else
                {
                    Result = _dbContext.TradePairTableResponse.FromSql(
                                @"Select 1 as QtyLength,1 as PriceLength,1 as AmtLength,SM1.Id As BaseId,SM1.Name As BaseName,SM1.SMSCode As BaseCode,TPM.ID As PairId,TPM.PairName As Pairname,TPS.CurrentRate As Currentrate,TPD.BuyFees As BuyFees,TPD.SellFees As SellFees,
                                    SM2.Name As ChildCurrency,SM2.SMSCode As Abbrevation,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,TPS.High24Hr AS High24Hr,TPS.Low24Hr As Low24Hr,CAST(0 AS DECIMAL) AS PairPercentage,
                                    TPS.HighWeek As HighWeek,TPS.LowWeek As LowWeek,TPS.High52Week AS High52Week,TPS.Low52Week As Low52Week,TPS.UpDownBit As UpDownBit,TPM.Priority from MarketMargin M 
                                    Inner Join TradePairMasterMargin TPM ON TPM.BaseCurrencyId = M.ServiceID
                                    Inner Join TradePairDetailMargin TPD ON TPD.PairId = TPM.Id
                                    Inner Join TradePairStasticsMargin TPS ON TPS.PairId = TPM.Id
                                    Inner Join ServiceMasterMargin SM1 ON SM1.Id = TPM.BaseCurrencyId
                                    Inner Join ServiceMasterMargin SM2 ON SM2.Id = TPM.SecondaryCurrencyId Where TPM.Status = 1 And M.Status = 1 And M.ServiceID = {0}", BaseId);
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ServiceMasterResponse> GetAllServiceConfiguration(int StatusData = 0, long CurrencyTypeid = 999)
        {
            try
            {
                IQueryable<ServiceMasterResponse> Result = null;

                if (StatusData == 0)
                {
                    Result = _dbContext.ServiceMasterResponse.FromSql(
                                @"Select  WT.CurrencyTypeId,Rate,SM.IsIntAmountAllow as IsOnlyIntAmountAllow,SM.Id As ServiceId,SM.Name As ServiceName,SM.SMSCode,SM.ServiceType,SD.ServiceDetailJson,
                            SS.CirculatingSupply,SS.IssueDate,SS.IssuePrice,SM.Status,SM.WalletTypeID,
                            ISNULL((Select STM.Status From ServiceTypeMapping STM Where STM.ServiceId = SM.Id and TrnType = 1),0) TransactionBit,
                            ISNULL((Select STM.Status From ServiceTypeMapping STM Where STM.ServiceId = SM.Id and TrnType = 6),0) WithdrawBit,
                            ISNULL((Select STM.Status From ServiceTypeMapping STM Where STM.ServiceId = SM.Id and TrnType = 8),0) DepositBit
                            From ServiceMaster SM
                            Inner Join ServiceDetail SD On SD.ServiceId = SM.Id
                            Inner Join ServiceStastics SS On SS.ServiceId = SM.Id Inner Join WalletTypeMasters WT On WT.WalletTypeName=SM.SMSCode Where SM.Status = 1 AND (WT.CurrencyTypeId={0} OR {0}=999)", CurrencyTypeid);

                }
                else
                {
                    Result = _dbContext.ServiceMasterResponse.FromSql(
                                @"Select  WT.CurrencyTypeId,Rate,SM.IsIntAmountAllow as IsOnlyIntAmountAllow,SM.Id As ServiceId,SM.Name As ServiceName,SM.SMSCode,SM.ServiceType,SD.ServiceDetailJson,
                            SS.CirculatingSupply,SS.IssueDate,SS.IssuePrice,SM.Status,SM.WalletTypeID, 
                            ISNULL((Select STM.Status From ServiceTypeMapping STM Where STM.ServiceId = SM.Id and TrnType = 1),0) TransactionBit,
                            ISNULL((Select STM.Status From ServiceTypeMapping STM Where STM.ServiceId = SM.Id and TrnType = 6),0) WithdrawBit,
                            ISNULL((Select STM.Status From ServiceTypeMapping STM Where STM.ServiceId = SM.Id and TrnType = 8),0) DepositBit
                            From ServiceMaster SM
                            Inner Join ServiceDetail SD On SD.ServiceId = SM.Id
                            Inner Join ServiceStastics SS On SS.ServiceId = SM.Id Inner Join WalletTypeMasters WT On WT.WalletTypeName=SM.SMSCode Where (SM.Status = 1 Or SM.Status = 0) AND (WT.CurrencyTypeId={0} OR {0}=999)", CurrencyTypeid);
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 5-3-19 for Margin Tradin
        public List<ServiceMasterResponse> GetAllServiceConfigurationMargin(int StatusData = 0)
        {
            try
            {
                IQueryable<ServiceMasterResponse> Result = null;

                if (StatusData == 0)
                {
                    Result = _dbContext.ServiceMasterResponse.FromSql(
                                @"Select cast(0 as bigint) as CurrencyTypeId,cast(0 as decimal(28,18)) as Rate,cast (0 as smallint) as IsOnlyIntAmountAllow,SM.Id As ServiceId,SM.Name As ServiceName,SM.SMSCode,SM.ServiceType,SD.ServiceDetailJson,
                            SS.CirculatingSupply,SS.IssueDate,SS.IssuePrice,SM.Status,SM.WalletTypeID,
                            ISNULL((Select STM.Status From ServiceTypeMappingMargin STM Where STM.ServiceId = SM.Id and TrnType = 1),0) TransactionBit,
                            ISNULL((Select STM.Status From ServiceTypeMappingMargin STM Where STM.ServiceId = SM.Id and TrnType = 6),0) WithdrawBit,
                            ISNULL((Select STM.Status From ServiceTypeMappingMargin STM Where STM.ServiceId = SM.Id and TrnType = 8),0) DepositBit
                            From ServiceMasterMargin SM
                            Inner Join ServiceDetailMargin SD On SD.ServiceId = SM.Id
                            Inner Join ServiceStasticsMargin SS On SS.ServiceId = SM.Id Where SM.Status = 1");

                }
                else
                {
                    Result = _dbContext.ServiceMasterResponse.FromSql(
                                @"Select cast(0 as bigint) as CurrencyTypeId,cast(0 as decimal(28,18)) as Rate,cast (0 as smallint) as IsOnlyIntAmountAllow,SM.Id As ServiceId,SM.Name As ServiceName,SM.SMSCode,SM.ServiceType,SD.ServiceDetailJson,
                            SS.CirculatingSupply,SS.IssueDate,SS.IssuePrice,SM.Status,SM.WalletTypeID, 
                            ISNULL((Select STM.Status From ServiceTypeMappingMargin STM Where STM.ServiceId = SM.Id and TrnType = 1),0) TransactionBit,
                            ISNULL((Select STM.Status From ServiceTypeMappingMargin STM Where STM.ServiceId = SM.Id and TrnType = 6),0) WithdrawBit,
                            ISNULL((Select STM.Status From ServiceTypeMappingMargin STM Where STM.ServiceId = SM.Id and TrnType = 8),0) DepositBit
                            From ServiceMasterMargin SM
                            Inner Join ServiceDetailMargin SD On SD.ServiceId = SM.Id
                            Inner Join ServiceStasticsMargin SS On SS.ServiceId = SM.Id Where SM.Status = 1 Or SM.Status = 0");
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetGraphResponsePairWise> GetGraphDataEveryLastMin(string Interval)
        {
            try
            {
                Interval = Interval.Replace(".", ":");  // Uday 01-03-2019 Solve error for convertion
                string Query = "";
                IQueryable<GetGraphResponsePairWise> Result;

                //Uday 28-02-2019  Give Transaction date wise data so give data as per crteated date wise
                Query = " SELECT (Select Top 1 PairName From TradePairMaster TPM Where TPM.Id = T.PairId) As PairName," +
                        "CONVERT(BIGINT, DATEDIFF(ss, '01-01-1970 00:00:00', DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0))) * 1000 DataDate, " +
                        "MAX(LTP) AS High, MIN(LTP) AS Low, SUM(Quantity * LTP) AS Volume, " +
                        "(SELECT LTP FROM dbo.TradeGraphDetail AS T1 WHERE(T1.Id = MIN(T.Id))) AS[OpenVal], " + //komal solve error
                        "(SELECT LTP FROM dbo.TradeGraphDetail AS T1 WHERE(T1.Id = MAX(T.Id))) AS[CloseVal] " +
                        "FROM dbo.TradeGraphDetail AS T Where T.PairId In(Select TM.Id From TradePairMaster TM) " +
                        "And DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0) = {0} " +
                        "GROUP BY DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0),PairId";
                Result = _dbContext.GetGraphResponseByPair.FromSql(Query, Interval);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
                return null;
            }
        }
        //Rita 5-3-19 for Margin Tradin
        public List<GetGraphResponsePairWise> GetGraphDataEveryLastMinMargin(string Interval)
        {
            try
            {
                Interval = Interval.Replace(".", ":");  // Uday 01-03-2019 Solve error for convertion
                string Query = "";
                IQueryable<GetGraphResponsePairWise> Result;

                Query = " SELECT (Select Top 1 PairName From TradePairMasterMargin TPM Where TPM.Id = T.PairId) As PairName," +
                        "CONVERT(BIGINT, DATEDIFF(ss, '01-01-1970 00:00:00', DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0))) * 1000 DataDate, " +
                        "MAX(LTP) AS High, MIN(LTP) AS Low, SUM(Quantity * LTP) AS Volume, " +
                        "(SELECT LTP FROM dbo.TradeGraphDetailMargin AS T1 WHERE(T1.Id = MIN(T.Id))) AS[OpenVal], " + //komal solve error
                        "(SELECT LTP FROM dbo.TradeGraphDetailMargin AS T1 WHERE(T1.Id = MAX(T.Id))) AS[CloseVal] " +
                        "FROM dbo.TradeGraphDetailMargin AS T Where T.PairId In(Select TM.Id From TradePairMasterMargin TM) " +
                        "And DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0) = {0} " +
                        "GROUP BY DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0),PairId";

                Result = _dbContext.GetGraphResponseByPair.FromSql(Query, Interval);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public HighLowViewModel GetHighLowValue(long PairId, int Day)
        {
            try
            {
                IQueryable<HighLowViewModel> Result;
                Result = _dbContext.HighLowViewModel.FromSql(
                            @"Select IsNull(MIN(T.Price),0) As LowPrice,IsNull(MAX(T.Price),0) As HighPrice From 
                                (Select Case TTQ.TrnType WHEN 4 Then TTQ.BidPrice WHEN 5 Then TTQ.AskPrice END As Price From SettledTradeTransactionQueue TTQ Where TTQ.Status = 1 And PairId = {0}
                                And TTQ.TrnDate Between DateAdd(Day,{1},dbo.GetISTDate()) And dbo.GetISTDate()) As T", PairId, Day);

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public HighLowViewModel GetHighLowValueMargin(long PairId, int Day)
        {
            try
            {
                IQueryable<HighLowViewModel> Result;
                Result = _dbContext.HighLowViewModel.FromSql(
                            @"Select IsNull(MIN(T.Price),0) As LowPrice,IsNull(MAX(T.Price),0) As HighPrice From 
                                (Select Case TTQ.TrnType WHEN 4 Then TTQ.BidPrice WHEN 5 Then TTQ.AskPrice END As Price From SettledTradeTransactionQueueMargin TTQ Where TTQ.Status = 1 And PairId = {0}
                                And TTQ.TrnDate Between DateAdd(Day,{1},dbo.GetISTDate()) And dbo.GetISTDate()) As T", PairId, Day);

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<FavouritePairInfo> GetFavouritePairs(long UserId)
        {
            try
            {
                IQueryable<FavouritePairInfo> Result;

                Result = _dbContext.FavouritePairViewModel.FromSql(
                            @"Select 1 as QtyLength,1 as PriceLength,1 as AmtLength,FP.PairId,TPM.PairName As Pairname,TPS.Currentrate,TPD.BuyFees,TPD.SellFees,
                            SM1.Name As ChildCurrency,SM1.SMSCode As Abbrevation,SM2.Name As BaseCurrency,SM2.SMSCode As BaseAbbrevation,TPD.PairPercentage,
                            TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,TPS.High24Hr,TPS.Low24Hr,TPS.HighWeek,TPS.LowWeek,
                            TPS.High52Week,TPS.Low52Week,TPS.UpDownBit  From FavouritePair FP 
                            Inner Join TradePairMaster TPM On TPM.Id = FP.PairId
                            Inner Join TradePairDetail TPD On TPD.PairId = TPM.Id
                            Inner Join ServiceMaster SM1 On SM1.Id = TPM.SecondaryCurrencyId
                            Inner Join ServiceMaster SM2 On SM2.Id = TPM.BaseCurrencyId
                            Inner Join TradePairStastics TPS On TPS.PairId = TPM.Id
                            Where FP.UserId = {0} And FP.Status = 1 and TPM.Status=1", UserId);//Rita 23-5-19 added enable pair condition

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 23-2-19 for Margin Trading
        public List<FavouritePairInfo> GetFavouritePairsMargin(long UserId)
        {
            try
            {
                IQueryable<FavouritePairInfo> Result;

                Result = _dbContext.FavouritePairViewModel.FromSql(
                            @"Select 1 as QtyLength,1 as PriceLength,1 as AmtLength,FP.PairId,TPM.PairName As Pairname,TPS.Currentrate,TPD.BuyFees,TPD.SellFees,
                            SM1.Name As ChildCurrency,SM1.SMSCode As Abbrevation,SM2.Name As BaseCurrency,SM2.SMSCode As BaseAbbrevation,CAST (0 AS DECIMAL) AS PairPercentage,
                            TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,TPS.High24Hr,TPS.Low24Hr,TPS.HighWeek,TPS.LowWeek,
                            TPS.High52Week,TPS.Low52Week,TPS.UpDownBit  From FavouritePairMargin FP 
                            Inner Join TradePairMasterMargin TPM On TPM.Id = FP.PairId
                            Inner Join TradePairDetailMargin TPD On TPD.PairId = TPM.Id
                            Inner Join ServiceMasterMargin SM1 On SM1.Id = TPM.SecondaryCurrencyId
                            Inner Join ServiceMasterMargin SM2 On SM2.Id = TPM.BaseCurrencyId
                            Inner Join TradePairStasticsMargin TPS On TPS.PairId = TPM.Id
                            Where FP.UserId = {0} And FP.Status = 1 and TPM.Status=1", UserId);//Rita 23-5-19 added enable pair condition

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<PairStatisticsCalculation> GetPairStatisticsCalculation()
        {
            try
            {
                IQueryable<PairStatisticsCalculation> Result;

                Result = _dbContext.PairStatisticsCalculation.FromSql(
                            @"Select TPM.Id As PairId,
                            SUM((Case When STTQ.TrnType = 4 Then STTQ.BidPrice When STTQ.TrnType = 5 Then STTQ.AskPrice Else 0 End) * 
                            (Case When STTQ.TrnType = 4 Then STTQ.BuyQty When STTQ.TrnType = 5 Then STTQ.SellQty Else 0 End)) As Volume,
                            IsNull((((Select Top 1 (Case When STTQ.TrnType = 4 Then STTQ.BidPrice When STTQ.TrnType = 5 Then STTQ.AskPrice Else 0 End) As Price From SettledTradeTransactionQueue STTQ Where STTQ.PairId=TPM.Id And STTQ.TrnDate >= DateAdd(Day,-1,dbo.GetISTDate()) And STTQ.Status = 1 Order By Id desc) * 100 ) /
                            (Select Top 1 (Case When STTQ.TrnType = 4 Then STTQ.BidPrice When STTQ.TrnType = 5 Then STTQ.AskPrice Else 0 End) As Price From SettledTradeTransactionQueue STTQ Where STTQ.PairId=TPM.Id And STTQ.TrnDate >= DateAdd(Day,-1,dbo.GetISTDate()) And STTQ.Status = 1 Order By Id Asc) - 100),0) As ChangePer,
                            IsNull((Select Top 1 (Case When STTQ.TrnType = 4 Then STTQ.BidPrice When STTQ.TrnType = 5 Then STTQ.AskPrice Else 0 End) As Price From SettledTradeTransactionQueue STTQ Where STTQ.PairId=TPM.Id And STTQ.TrnDate >= DateAdd(Day,-1,dbo.GetISTDate()) And STTQ.Status = 1 Order By Id desc) -
                            (Select Top 1 (Case When STTQ.TrnType = 4 Then STTQ.BidPrice When STTQ.TrnType = 5 Then STTQ.AskPrice Else 0 End) As Price From SettledTradeTransactionQueue STTQ Where STTQ.PairId=TPM.Id And STTQ.TrnDate >= DateAdd(Day,-1,dbo.GetISTDate()) And STTQ.Status = 1 Order By Id Asc),0) As ChangeValue From TradePairMaster TPM 
                            Left Outer Join SettledTradeTransactionQueue STTQ On STTQ.PairID = TPM.Id And STTQ.TrnDate >= DateAdd(Day,-1,dbo.GetISTDate()) And STTQ.Status = 1
                            Group By TPM.Id");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null; // Uday 07-01-2019 not throw the exception
            }
        }

        public void UpdatePairStatisticsCalculation(List<TradePairStastics> PairDataUpdated)
        {
            try
            {
                _dbContext.Database.BeginTransaction();

                foreach (var pair in PairDataUpdated)
                {
                    _dbContext.Entry(pair).State = EntityState.Modified;
                }

                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public List<LPStatusCheckData> LPstatusCheck()
        {
            try
            {
                List<LPStatusCheckData> Result;

                Result = _dbContext.LPStatusCheckData.FromSql("dbo.sp_LPstatusCheck @ActionStage={0},@ReturnCode={1}", 1, 0).ToList();

                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public List<LPStatusCheckDataArbitrage> LPstatusCheckArbitrage(short ActionStage)
        {
            try
            {
                List<LPStatusCheckDataArbitrage> Result;

                Result = _dbContext.LPStatusCheckDataArbitrage.FromSql("dbo.sp_LPstatusCheck @ActionStage={0},@ReturnCode={1}", ActionStage, 0).ToList();

                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public List<LPStatusCheckData> LPstatusCheckOLD()
        {
            try
            {
                IQueryable<LPStatusCheckData> Result;

                string Addminute = _configuration["CallStatusCheck"].ToString() == null ? 2.ToString() : _configuration["CallStatusCheck"].ToString();

                string Qry = @"Select SD.ID AS SerProDetailID, ISnull(TR.TrnID,'') AS TrnRefNo, SD.AppTypeID ,TTQ.TrnNo,CallStatus,TTQ.ordertype,TTQ.TrnType, TTQ.Status,TTQ.PairName As Pair,
                                CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price, 
                                CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount,  
                                TTQ.TrnDate as DateTime from TradeTransactionQueue TTQ INNER JOIN TransactionQueue TQ ON TQ.Id = TTQ.TrnNo INNER JOIN TransactionRequest TR 
                                ON TR.TrnNo = TQ.Id
                                INNER JOIN ServiceProviderDetail SD ON SD.ID = TQ.SerProDetailId INNER JOIN RouteConfiguration RC ON RC.id  = Tq.RouteID
                                WHERE SD.ProTypeID = 3 AND TTQ.IsAPITrade = 1  AND TTQ.Status = 4 AND TQ.Status = 4  AND TQ.CallStatus = 0 AND dbo.GetISTDate() >= DATEADD(MINUTE," + Addminute + " ,TTQ.CreatedDate) Order By TTQ.TrnNo Desc";

                Result = _dbContext.LPStatusCheckData.FromSql(Qry);

                return Result.ToList();

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null; // Uday 07-01-2019 not throw the exception
            }
        }

        public List<LPStatusCheckDataArbitrage> LPstatusCheckArbitrageOLD()
        {
            try
            {
                IQueryable<LPStatusCheckDataArbitrage> Result;
                //Commented by khushali old

                string Addminute = _configuration["CallStatusCheck"].ToString() == null ? 2.ToString() : _configuration["CallStatusCheck"].ToString();

                string qry = @"Select SD.ID AS SerProDetailID, ISnull(TR.TrnID,'') AS TrnRefNo, SD.AppTypeID ,TTQ.TrnNo,CallStatus,TTQ.ordertype,TTQ.TrnType, TTQ.Status,TTQ.PairName As Pair,
                                CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price, CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount,  
                                TTQ.TrnDate as DateTime from TradeTransactionQueueArbitrage TTQ INNER JOIN TransactionQueueArbitrage TQ ON TQ.Id = TTQ.TrnNo left JOIN ArbitrageTransactionRequest TR 
                                ON TR.TrnNo = TQ.Id INNER JOIN ServiceProviderDetailArbitrage SD ON SD.ID = TQ.SerProDetailId INNER JOIN RouteConfigurationArbitrage RC ON RC.id  = Tq.RouteID
                                WHERE SD.ProTypeID = 3 AND TTQ.IsAPITrade = 1  AND TTQ.Status = 4 AND TQ.Status = 4  AND TQ.CallStatus = 0 and dbo.GetISTDate() >= DATEADD(MINUTE," + Addminute + " ,TTQ.CreatedDate) Order By TTQ.TrnNo Desc";

                Result = _dbContext.LPStatusCheckDataArbitrage.FromSql(qry);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null; // Uday 07-01-2019 not throw the exception
            }
        }

        // khushali 27-07-2019  for stop loss cron
        public List<StopLossArbitargeResponse> StopLossArbitargeCron()
        {
            List<StopLossArbitargeResponse> list;
            try
            {
                IQueryable<StopLossArbitargeResponse> Result = _dbContext.StopLossArbitargeResponse.FromSql(
                      @"select TTQ.MemberID,TTQ.TrnNo,TTQ.PairName,
                            ISNULL(SP.APIKey,'') as APIKey, ISNULL(SP.SecretKey,'') as SecretKey,SD.AppTypeID As LPType, TQ.TrnRefno AS LPOrderID 
                            FROM TradeTransactionQueueArbitrage TTQ 
                            INNER JOIN  TransactionQueueArbitrage TQ ON  TQ.Id = TTQ.Trnno 
                            INNER JOIN Tradestoplossarbitrage TS ON TS.Trnno = TQ.ID 
                            INNER JOIN  ServiceProviderDetailArbitrage SD ON SD.Id = TQ.SerProDetailID
                            INNER JOIN cryptowatcherarbitrage C ON C.LPType = SD.AppTypeID and C.PairID=TTQ.PairID                     
                            INNER JOIN ServiceProConfigurationarbitrage SP ON SP.ID = SD.ServiceProConfigID
                            WHERE SD.Status = 1 and TQ.status = 4 and TTQ.status = 4 and C.LTP > TS.stopprice and SP.param2 = '1'");

                list = new List<StopLossArbitargeResponse>(Result.ToList());

                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetProviderDataListArbitrageAsync", "WebApiDataRepository", ex);
                return null;
            }
        }

        public List<ReleaseAndStuckOrdercls> ReleaseAndStuckOrder(DateTime Date)
        {
            IQueryable<ReleaseAndStuckOrdercls> Result = null;
            string Qry = "";

            try
            {
                Qry = "Select TTQ.TrnNo " +
                  "from TradeTransactionQueue TTQ" +
                  " LEFT JOIN  TradeSellerListV1 TS ON TS.TrnNo = TTQ.TrnNo" +
                  " LEFT JOIN  TradeBuyerListV1 TB ON TB.TrnNo = TTQ.TrnNo" +
                   " WHERE TTQ.Status = 4 AND ( TS.IsProcessing = 1 OR TB.IsProcessing = 1 ) AND TTQ.UpdatedDate <= {0} Order By TTQ.TrnNo Desc ";

                Result = _dbContext.ReleaseAndStuckOrder.FromSql(Qry, Date);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }

        }
        //khuhsali 15-05-2019 for Marging trading ReleaseAndStuckOrder cron 
        public List<ReleaseAndStuckOrdercls> MarginReleaseAndStuckOrder(DateTime Date)
        {
            IQueryable<ReleaseAndStuckOrdercls> Result = null;
            string Qry = "";
            try
            {
                Qry = @"Select TTQ.TrnNo 
                        from TradeTransactionQueueMargin TTQ 
                        LEFT JOIN TradeSellerListMarginV1 TS ON TS.TrnNo = TTQ.TrnNo 
                        LEFT JOIN  TradeBuyerListMarginV1 TB ON TB.TrnNo = TTQ.TrnNo 
                        WHERE TTQ.Status = 4 AND(TS.IsProcessing = 1 OR TB.IsProcessing = 1) AND TTQ.UpdatedDate <= {0} 
                        Order By TTQ.TrnNo Desc";
                Result = _dbContext.ReleaseAndStuckOrder.FromSql(Qry, Date);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }

        }

        #endregion

        #region Liquidity Configuration

        //khuhsali 14-05-2019 for Liquidity configuration
        public List<ConfigureLP> GetLiquidityConfigurationData(short LPType)
        {
            IQueryable<ConfigureLP> Result = null;
            string Qry = "";

            try
            {
                /// Change in query add AP.Id 20 For OkEx by Pushpraj as on 11-06-2019
                Qry = @"select distinct TM.PairName as Pair , cast (AP.Id as smallint) as LPType , RC.OpCode
                        FROM RouteConfiguration RC
                        INNER JOIN ServiceProviderDetail SD ON  SD.Id = RC.SerProDetailID
                        INNER JOIN  AppType AP ON AP.Id = SD.AppTypeID
                        INNER JOIN  ServiceProviderMaster SM ON SM.Id = SD.ServiceProID
                        INNER JOIN TradePairMaster TM ON TM.id = RC.PairId
                        WHERE RC.TrnType in (4, 5) and 
                        SD.Status = 1 AND SM.Status = 1 AND RC.Status = 1 AND TM.Status = 1 and AP.Id in (9,10,11,12,13,18,19,20,21,22,23,24,25,26)"; //AP.Id = {0} and

                Result = _dbContext.ConfigureLP.FromSql(Qry); //LPType

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }

        }
        //komal 10-06-2019 make Arbitrage Method
        public List<ConfigureLPArbitrage> GetLiquidityConfigurationDataArbitrage(short LPType)
        {
            IQueryable<ConfigureLPArbitrage> Result = null;
            string Qry = "";
            try
            {
                Qry = @"select distinct TM.PairName as Pair , RC.OpCode AS 'ProviderPair', cast (AP.Id as smallint) as LPType,SM.ProviderName,TM.Id AS PairID
                        FROM RouteConfigurationArbitrage RC
                        INNER JOIN ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID
                        INNER JOIN  AppType AP ON AP.Id = SD.AppTypeID
                        INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID
                        INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId
                        WHERE RC.TrnType in (4, 5) and SD.protypeid = 3 and  SM.Status = 1 and 
                        SD.Status = 1 AND SM.Status = 1 AND RC.Status = 1 AND TM.Status = 1 and AP.Id in (select ID from AppType) and AP.ID <>8"; //AP.Id = {0} and

                Result = _dbContext.ConfigureLPArbitrage.FromSql(Qry); //LPType

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public bool GetLocalConfigurationData(short LPType)
        {
            string Qry = "";
            try
            {
                Qry = @"insert into cryptowatcher (LTP,pair,Lptype) 
                        select distinct TP.LTP, TM.PairName as Pair , 8  as Lptype 
                        FROM  TradePairMaster TM 
                        INNER JOIN  TradePairStastics TP ON TP.PairId = TM.id 
                        WHERE TM.Status = 1 and TM.PairName not in (select pair from cryptowatcher where LPType = {0});
                        UPDATE cryptowatcher SET cryptowatcher.LTP = TP.LTP 
                        FROM  TradePairMaster TM 
                        INNER JOIN  TradePairStastics TP ON TP.PairId = TM.id 
                        WHERE TM.Status = 1 and  cryptowatcher.LPType = {0} and cryptowatcher.pair = TM.PairName";

                var res = _dbContext.Database.ExecuteSqlCommand(Qry, LPType);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        public bool UpdateLTPData(LTPcls LTPData)
        {
            string Qry = "";
            try
            {
                Qry = @"update CryptoWatcher set LTP = {0} where  Pair = {1} and LPType = {2} ";
                var res = _dbContext.Database.ExecuteSqlCommand(Qry, LTPData.Price, LTPData.Pair.Trim(), LTPData.LpType);

                if (res > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        public List<CryptoWatcher> GetPairWiseLTPData(GetLTPDataLPwise LTPData)
        {
            IQueryable<CryptoWatcher> Result = null;
            string Qry = "";

            try
            {
                Qry = @"select * from CryptoWatcher where Pair = {0} and LPType in ( " + LTPData.LpType + " )";
                Result = _dbContext.CryptoWatcher.FromSql(Qry, LTPData.Pair.Trim());

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public bool InsertLTPData(LTPcls LTPData)
        {
            string Qry = "";
            try
            {
                Qry = @"insert into CryptoWatcher values ({0} ,{1} ,{2})";

                var res = _dbContext.Database.ExecuteSqlCommand(Qry, LTPData.Price, LTPData.Pair.Trim(), LTPData.LpType);

                if (res > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        public LPKeyVault BalanceCheckLP(long SerproID)
        {
            IQueryable<LPKeyVault> Result = null;
            string Qry = "";
            LPKeyVault Data = new LPKeyVault();
            try
            {
                Qry = @"SELECT TOP 1 APIKey , SecretKey , AppTypeID FROM ServiceProviderMaster SM 
                    INNER JOIN ServiceProviderDetail SD ON SM.Id = SD.ServiceProID 
                    INNER JOIN ServiceProConfiguration SC ON SC.Id = SD.ServiceProConfigID where SM.ID = {0} and SD.status = 1";

                Result = _dbContext.LPKeyVault.FromSql(Qry, SerproID);

                if (Result != null)
                    Data = Result.ToList().FirstOrDefault();

                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return Data;
            }
        }

        public LPKeyVault BalanceCheckLPArbitrage(long SerproID)
        {
            IQueryable<LPKeyVault> Result = null;
            string Qry = "";
            LPKeyVault Data = new LPKeyVault();
            try
            {
                Qry = @"SELECT TOP 1 APIKey , SecretKey , AppTypeID FROM ServiceProviderMasterArbitrage SM 
                    INNER JOIN ServiceProviderDetailArbitrage SD ON SM.Id = SD.ServiceProID 
                    INNER JOIN ServiceProConfigurationArbitrage SC ON SC.Id = SD.ServiceProConfigID where SM.ID = {0} and SD.status = 1";

                //Removed by Rushabh as null response returned due to query over write
                //Qry = @"SELECT TOP 1 APIKey , SecretKey , AppTypeID FROM ServiceProviderMaster SM 
                //    INNER JOIN ServiceProviderDetail SD ON SM.Id = SD.ServiceProID 
                //    INNER JOIN ServiceProConfiguration SC ON SC.Id = SD.ServiceProConfigID where SM.ID = {0}";

                Result = _dbContext.LPKeyVault.FromSql(Qry, SerproID);

                if (Result != null)
                    Data = Result.ToList().FirstOrDefault();

                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return Data;
            }
        }
        #endregion

        #region Arbitrage Trade data Method
        public List<GetBuySellBook> GetBuyerBookArbitrage(long id, decimal Price = -1)
        {
            try
            {
                IQueryable<GetBuySellBook> Result;
                if (Price != -1)//SignalR call
                {
                    Result = _dbContext2.BuyerSellerInfo.FromSql(
                                  @"Select Top 1 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsStopLimit 
                                    From TradeTransactionQueueArbitrage TTQ  INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo 
                                    Where (TTQ.ordertype != 4 ) AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.PairID ={0} AND TTQ.IsAPITrade=0 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.BidPrice={1}
                                    Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 Order By TTQ.BidPrice desc", id, Price);
                }
                else//API call
                {
                    Result = _dbContext2.BuyerSellerInfo.FromSql(
                                  @"SELECT Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsStopLimit
                                    FROM TradeTransactionQueueArbitrage TTQ  INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo 
                                    WHERE TTQ.ordertype <> 4 AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.PairID ={0} AND TTQ.IsAPITrade=0 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                                    GROUP By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 UNION ALL
                                    SELECT Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsStopLimit
                                    FROM TradeTransactionQueueArbitrage TTQ  INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo INNER join TradePairStasticsArbitrage TPS on TTQ.PairID=TPS.PairId
                                    WHERE (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= TPS.LTP)) AND TTQ.ordertype = 4) 
                                    AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0} AND TTQ.IsAPITrade=0
                                    GROUP By TTQ.BidPrice,TTQ.PairID HAVING (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 Order By TTQ.BidPrice desc", id);
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " #PairId# : " + id + " #Price# : " + Price, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<GetBuySellBook> GetSellerBookArbitrage(long id, decimal Price = -1)
        {
            try
            {
                IQueryable<GetBuySellBook> Result;

                if (Price != -1)
                {
                    Result = _dbContext2.BuyerSellerInfo.FromSql(
                                @"SELECT Top 1 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,
                                Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsStopLimit 
                                FROM TradeTransactionQueueArbitrage TTQ INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo
                                WHERE (TTQ.ordertype != 4 ) AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsAPITrade=0 AND TTQ.AskPrice={1}
                                AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 order by TTQ.AskPrice", id, Price);
                }
                else
                {
                    Result = _dbContext2.BuyerSellerInfo.FromSql(
                               @"SELECT Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsStopLimit
                                    FROM TradeTransactionQueueArbitrage TTQ INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo 
                                    WHERE TTQ.ordertype != 4 AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsAPITrade=0
                                    AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 UNION ALL
                                    SELECT TOP 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsStopLimit
                                    FROM TradeTransactionQueueArbitrage TTQ INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo INNER JOIN TradePairStasticsArbitrage TPS on TTQ.PairID=TPS.PairId
                                    WHERE (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= TPS.LTP)) AND TTQ.ordertype = 4) 
                                    AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsAPITrade=0 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                                    GROUP by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 order by TTQ.AskPrice", id);
                }

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " #PairId# : " + id + " #Price# : " + Price, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<GetGraphDetailInfo> GetGraphDataArbitrage(long id, int IntervalTime, string IntervalData, DateTime Minute, int socket = 0)
        {
            try
            {
                string Query = "";
                IQueryable<GetGraphDetailInfo> Result;
                if (socket == 0)
                {
                    Query = "SELECT CONVERT(BIGINT,DATEDIFF(ss,'01-01-1970 00:00:00',DATEADD(#IntervalData#, DATEDIFF(#IntervalData#, 0, CreatedDate) / #IntervalTime# * #IntervalTime#, 0))) * 1000 DataDate," +
                         "MAX(LTP) AS High, MIN(LTP) AS Low, SUM(Quantity * LTP) AS Volume," +
                         "(SELECT LTP FROM dbo.TradeGraphDetailArbitrage AS T1 WHERE(T1.Id = MIN(T.Id))) AS[Open]," +
                         "(SELECT LTP FROM dbo.TradeGraphDetailArbitrage AS T1 WHERE(T1.Id = MAX(T.Id))) AS[Close]" +
                         " FROM dbo.TradeGraphDetailArbitrage AS T Where T.PairId = {0} " +
                         " GROUP BY DATEADD(#IntervalData#, DATEDIFF(#IntervalData#, 0, CreatedDate) / #IntervalTime# * #IntervalTime#, 0)";

                    Query = Query.Replace("#IntervalData#", IntervalData).Replace("#IntervalTime#", IntervalTime.ToString());
                    Result = _dbContext.GetGraphResponse.FromSql(Query, id);
                }
                else
                {
                    Query = "SELECT CONVERT(BIGINT,DATEDIFF(ss,'01-01-1970 00:00:00',DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0))) * 1000 DataDate," +
                        "MAX(LTP) AS High, MIN(LTP) AS Low, SUM(Quantity * LTP) AS Volume," +
                        "(SELECT LTP FROM dbo.TradeGraphDetailArbitrage AS T1 WHERE(T1.Id = MIN(T.Id))) AS[Open]," +
                        "(SELECT LTP FROM dbo.TradeGraphDetailArbitrage AS T1 WHERE(T1.Id = MAX(T.Id))) AS[Close]" +
                        " FROM dbo.TradeGraphDetailArbitrage AS T Where T.PairId = {0} And DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0) = {1}" +
                        " GROUP BY DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0)";

                    string MinuteData = Minute.ToString("yyyy-MM-dd HH:mm:00:000");
                    Result = _dbContext.GetGraphResponse.FromSql(Query, id, MinuteData);
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<GetGraphResponsePairWise> GetGraphDataEveryLastMinArbitrage(string Interval)
        {
            try
            {
                Interval = Interval.Replace(".", ":");
                string Query = "";
                IQueryable<GetGraphResponsePairWise> Result;
                Query = " SELECT (Select Top 1 PairName From TradePairMasterArbitrage TPM Where TPM.Id = T.PairId) As PairName," +
                        "CONVERT(BIGINT, DATEDIFF(ss, '01-01-1970 00:00:00', DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0))) * 1000 DataDate, " +
                        "MAX(LTP) AS High, MIN(LTP) AS Low, SUM(Quantity * LTP) AS Volume, " +
                        "(SELECT LTP FROM dbo.TradeGraphDetailArbitrage AS T1 WHERE(T1.Id = MIN(T.Id))) AS[OpenVal], " +
                        "(SELECT LTP FROM dbo.TradeGraphDetailArbitrage AS T1 WHERE(T1.Id = MAX(T.Id))) AS[CloseVal] " +
                        "FROM dbo.TradeGraphDetailArbitrage AS T Where T.PairId In(Select TM.Id From TradePairMasterArbitrage TM) " +
                        "And DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0) = {0} " +
                        "GROUP BY DATEADD(MINUTE, DATEDIFF(MINUTE, 0, CreatedDate) / 1 * 1, 0),PairId";

                Result = _dbContext.GetGraphResponseByPair.FromSql(Query, Interval);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        public List<StopLimitBuySellBook> GetStopLimitBuySellBooksArbitrage(decimal LTP, long Pair, enOrderType OrderType, short IsCancel = 0)
        {
            try
            {
                IQueryable<StopLimitBuySellBook> Result = null;
                if (IsCancel == 0)
                {
                    if (OrderType == enOrderType.BuyOrder)
                    {
                        Result = _dbContext2.StopLimitBuyerSellerBook.FromSql(
                            @"Select Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsAdd
                            From TradeTransactionQueueArbitrage TTQ  INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo 
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= {1}) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= {1})) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0}
                            Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 UNION ALL
                            Select Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsAdd
                            From TradeTransactionQueueArbitrage TTQ  INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo 
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice < {1}) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice > {1})) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0}
                            Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 Order By TTQ.BidPrice desc"
                            , Pair, LTP);
                    }
                    else if (OrderType == enOrderType.SellOrder)
                    {
                        Result = _dbContext2.StopLimitBuyerSellerBook.FromSql(
                            @"Select Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsAdd
                            from TradeTransactionQueueArbitrage TTQ INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo 
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= {1}) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= {1})) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                            Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 UNION ALL
                            Select Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsAdd
                            from TradeTransactionQueueArbitrage TTQ INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo 
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice < {1}) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice > {1})) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                            Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 order by TTQ.AskPrice"
                            , Pair, LTP);
                    }
                }
                else
                {
                    if (OrderType == enOrderType.BuyOrder)
                    {
                        Result = _dbContext2.StopLimitBuyerSellerBook.FromSql(
                            @"Select Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsAdd
                            From TradeTransactionQueueArbitrage TTQ  INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo INNER Join TradePairStasticsArbitrage TPS on TTQ.PairID=TPS.PairId
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= TPS.LTP)) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0}
                            Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 UNION ALL
                            Select Top 100 TTQ.BidPrice As Price, Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty) As Amount,Count(TTQ.BidPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsAdd
                            From TradeTransactionQueueArbitrage TTQ  INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo  INNER Join TradePairStasticsArbitrage TPS on TTQ.PairID=TPS.PairId
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice < TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice > TPS.LTP)) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 4 AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 AND TTQ.PairID ={0}
                            Group By TTQ.BidPrice,TTQ.PairID Having (Sum(TTQ.DeliveryTotalQty) - Sum(TTQ.SettledBuyQty)) > 0 Order By TTQ.BidPrice desc"
                            , Pair, LTP);
                    }
                    else if (OrderType == enOrderType.SellOrder)
                    {
                        Result = _dbContext2.StopLimitBuyerSellerBook.FromSql(
                            @"Select Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(1 AS smallint)AS IsAdd
                            from TradeTransactionQueueArbitrage TTQ INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo INNER Join TradePairStasticsArbitrage TPS on TTQ.PairID=TPS.PairId
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice >= TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice <= TPS.LTP)) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                            Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 UNION ALL
                            Select Top 100 TTQ.AskPrice As Price,sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty) as Amount,Count(TTQ.AskPrice) As RecordCount,NEWID() As OrderId,CAST(0 AS smallint)AS IsAdd
                            from TradeTransactionQueueArbitrage TTQ INNER join TradeStopLossArbitrage TSL on TTQ.TrnNo=TSL.TrnNo INNER Join TradePairStasticArbitrage TPS on TTQ.PairID=TPS.PairId
                            Where (((TSL.MarketIndicator = 0 AND TSL.StopPrice < TPS.LTP) OR(TSL.MarketIndicator = 1 AND TSL.StopPrice > TPS.LTP)) AND TTQ.ordertype = 4) 
                            AND TTQ.Status = 4 and TTQ.TrnType = 5 AND TTQ.pairID = {0} AND TTQ.IsCancelled = 0 AND TTQ.ordertype<>3 
                            Group by TTQ.AskPrice,TTQ.PairID Having (sum(TTQ.OrderTotalQty) - Sum(TTQ.SettledSellQty)) > 0 order by TTQ.AskPrice"
                            , Pair, LTP);
                    }
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public HighLowViewModel GetHighLowValueArbitrage(long PairId, int Day)
        {
            try
            {
                IQueryable<HighLowViewModel> Result;

                Result = _dbContext.HighLowViewModel.FromSql(
                            @"Select IsNull(MIN(T.Price),0) As LowPrice,IsNull(MAX(T.Price),0) As HighPrice From 
                                (Select Case TTQ.TrnType WHEN 4 Then TTQ.BidPrice WHEN 5 Then TTQ.AskPrice END As Price From SettledTradeTransactionQueueArbitrage TTQ Where TTQ.Status = 1 And PairId = {0}
                                And TTQ.TrnDate Between DateAdd(Day,{1},dbo.GetISTDate()) And dbo.GetISTDate()) As T", PairId, Day);

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<ExchangeProviderListArbitrage> GetExchangeProviderListArbitrage(long PairId)
        {
            try
            {
                IQueryable<ExchangeProviderListArbitrage> Result;

                Result = _dbContext.ExchangeProviderListArbitrage.FromSql(
                            @"select CAST(C.LPType as bigint) as LPType,RC.ID as RouteID,rc.ordertype,RC.RouteName,SM.ID as ProviderID,SM.ProviderName,
                            SD.ID as SerProDetailID,SD.TrnTypeID as TrnType,C.LTP,C.Volume,C.ChangePer,C.UpDownBit
                            FROM RouteConfigurationArbitrage RC 
                            INNER JOIN  ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID  
                            INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID 
                            INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId
                            INNER JOIN cryptowatcherarbitrage C ON C.LPType = SD.AppTypeID and c.Pair=TM.PairName
                            WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1 AND RC.PairId = {0}", PairId);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //rita 12-8-19 taken route from cache and LTP data from redis
        public List<ExchangeProviderListArbitrage> GetExchangeProviderListArbitrageCache(long PairId)
        {
            try
            {
                //IQueryable<ExchangeProviderListArbitrage> Result;

                var list = _cache.Get<List<TransactionProviderArbitrageResponse>>("RouteDataArbitrage");
                if (list == null)
                {
                    var GetProListResult = _WebApiRepository.GetProviderDataListArbitrageAsync(new TransactionApiConfigurationRequest { PairID = 0, trnType = 4, LPType = 0 });
                    list = _cache.Get<List<TransactionProviderArbitrageResponse>>("RouteDataArbitrage");
                }
                if (list != null)
                {
                    IQueryable<ArbitrageCryptoWatcherQryRes> Result1 = _dbContext.ArbitrageCryptoWatcherQryRes.FromSql(
                             "select Pair,LPType,LTP as Price,Volume,PairId,UpDownBit,ChangePer,Fees from " +
                             "CryptoWatcherArbitrage Where PairID={0}", PairId);

                    List<ArbitrageCryptoWatcherQryRes> LPData = Result1.ToList();

                    List<ExchangeProviderListArbitrage> ProviderList = new List<ExchangeProviderListArbitrage>();
                    ProviderList = list.Select(Provider => new ExchangeProviderListArbitrage
                    {
                        LPType = Provider.LPType,
                        RouteID = Provider.RouteID,
                        RouteName = Provider.RouteName,
                        ProviderID = Provider.ProviderID,
                        ProviderName = Provider.ProviderName,
                        SerProDetailID = Provider.SerProDetailID,
                        TrnType = Provider.TrnType,
                        LTP = LPData.Where(e => e.LpType == Provider.LPType).FirstOrDefault().Price,
                        Volume = LPData.Where(e => e.LpType == Provider.LPType).FirstOrDefault().Volume,
                        ChangePer = LPData.Where(e => e.LpType == Provider.LPType).FirstOrDefault().ChangePer,
                        UpDownBit = LPData.Where(e => e.LpType == Provider.LPType).FirstOrDefault().UpDownBit
                    }).ToList();

                    return ProviderList;
                }
                else
                    return null;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<SmartArbitrageHistoryInfo> SmartArbitrageHistoryList(long PairId, long MemberID, string FromDat, string ToDate)
        {
            string sCondition = " ";
            string Qry;
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {

                if (PairId != 999)
                    sCondition = " AND TT.PairID ={1} ";
                if (!string.IsNullOrEmpty(FromDat) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDat, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCondition += " AND tt.TrnDate Between '" + fDate + "' AND '" + tDate + "' ";
                }


                IQueryable<SmartArbitrageHistoryInfo> Result;
                Qry = @"select t.GUID,tt.PairName,min(tt.TrnDate) TrnDate,string_agg(sp.ProviderName, ', ') Market,
                            sum(CASE WHEN tt.trntype=5 THEN DeliveryTotalQty ELSE 0 END) - sum(CASE WHEN tt.trntype=4 THEN orderTotalQty ELSE 0 END) Profit
                            ,string_agg(CASE WHEN tt.trntype=4 THEN Order_Currency ELSE '' END, '') ProfitCurrency
                            ,sum(CASE WHEN tt.trntype=4 THEN BuyQty ELSE 0 END) as  FundUsed ,
                            string_agg(CASE WHEN tt.trntype=4 THEN Delivery_Currency ELSE '' END, '') FundUsedCurrency
                            from transactionqueuearbitrage t inner join tradetransactionqueuearbitrage tt on t.id=tt.TrnNo
                            inner join ServiceProviderMasterArbitrage SP on t.SerProID=SP.ID
                            where t.IsSmartArbitrage=1 and tt.MemberID={0} " + sCondition + " group by t.GUID,tt.PairName order by min(tt.TrnDate) desc";

                if (PairId == 999)
                    Result = _dbContext.SmartArbitrageHistoryInfo.FromSql(Qry, MemberID);
                else
                    Result = _dbContext.SmartArbitrageHistoryInfo.FromSql(Qry, MemberID, PairId);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<ArbitrageBuySellViewModel> GetExchangeProviderBuySellBookArbitrage(long PairId, short TrnType)
        {
            IQueryable<ArbitrageBuySellViewModel> Result;
            try
            {
                if (TrnType == 4)//Buy
                {
                    //Result = _dbContext.ExchangeProviderBuySellBookArbitrage.FromSql(
                    //@"SELECT SD.AppTypeID as LPType,SM.ProviderName,C.LTP,C.Fees FROM RouteConfigurationArbitrage RC 
                    //    INNER JOIN  ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID  INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID 
                    //    INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId INNER JOIN cryptowatcherarbitrage C ON C.LPType = SD.AppTypeID and c.Pair=TM.PairName 
                    //    WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1 AND TrnType=4 AND RC.PairId = {0} AND SD.AppTypeID <>{1}
                    //    UNION ALL SELECT SD.AppTypeID,SM.ProviderName,
                    //    CASE WHEN (select top 1 BidPrice from TradeTransactionQueueArbitrage where status=4 and TrnType=4 and PairID=TM.ID) is null THEN
                    //    (select CurrentRate from TradePairStasticsArbitrage where PairId=TM.ID) END AS LTP , CAST(0 as decimal(28,18))AS Fees
                    //    FROM RouteConfigurationArbitrage RC INNER JOIN  ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID  
                    //    INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId
                    //    WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1 AND TrnType=4 AND RC.PairId = {0} AND SD.AppTypeID ={1}
                    //    Order by LTP", PairId,(long)enAppType.COINTTRADINGLocal);
                    Result = _dbContext.ExchangeProviderBuySellBookArbitrage.FromSql(
                                @"SELECT ISNULL(MinAmt,0) as MinNotional,ISNULL(MaxAmt,0) as MaxNotional,
                                        ISNULL(MinPrice,0) as MinPrice,ISNULL(MaxPrice,0) as MaxPrice,
                                        ISNULL(MinQty,0) as MinQty,ISNULL(MaxQty,0) as MaxQty,
                                        SD.AppTypeID as LPType,SM.ProviderName,
                                        CASE WHEN SD.AppTypeID=8 THEN ISNULL((select top 1 BidPrice from TradeTransactionQueueArbitrage TTQ  
                                        where TTQ.status=4 and TTQ.TrnType=4 and TTQ.PairID=TM.Id AND TTQ.IsAPITrade=0 Order by TTQ.TrnNo desc ),
                                        (select LTP from TradePairStasticsArbitrage where PairId=TM.id))ELSE C.LTP END AS LTP,C.Fees FROM RouteConfigurationArbitrage RC 
                                        INNER JOIN  ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID  INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID 
                                        INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId 
                                        INNER JOIN cryptowatcherarbitrage C ON C.LPType = SD.AppTypeID and C.Pairid=TM.Id 
                                        Left JOIN LimitsArbitrage LM ON LM.id = RC.limitid 
                                        WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1 AND TrnType=4 AND SD.TrnTypeID=4 AND RC.PairId = {0} Order by LTP", PairId);

                }
                else  //Sell ,TrnType==5
                {
                    //Result = _dbContext.ExchangeProviderBuySellBookArbitrage.FromSql(
                    //        @"SELECT SD.AppTypeID as LPType,SM.ProviderName,C.LTP,C.Fees FROM RouteConfigurationArbitrage RC 
                    //            INNER JOIN  ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID  INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID 
                    //            INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId INNER JOIN cryptowatcherarbitrage C ON C.LPType = SD.AppTypeID and c.Pair=TM.PairName
                    //            WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1 AND TrnType=5 AND RC.PairId = {0} AND SD.AppTypeID <> {1}
                    //            UNION ALL SELECT SD.AppTypeID,SM.ProviderName, 
                    //            CASE WHEN (select top 1 BidPrice from TradeTransactionQueueArbitrage where status=4 and TrnType=5 and PairID=TM.ID) is null THEN
                    //            (select CurrentRate from TradePairStasticsArbitrage where PairId=TM.ID) END AS LTP , CAST(0 as decimal(28,18))AS Fees 
                    //            FROM RouteConfigurationArbitrage RC INNER JOIN  ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID  
                    //            INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId
                    //            WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1 AND TrnType=5 AND RC.PairId = {0} AND SD.AppTypeID={1}
                    //            Order by LTP", PairId, (long)enAppType.COINTTRADINGLocal);
                    Result = _dbContext.ExchangeProviderBuySellBookArbitrage.FromSql(
                            @"SELECT ISNULL(MinAmt,0) as MinNotional,ISNULL(MaxAmt,0) as MaxNotional,
                            ISNULL(MinPrice,0) as MinPrice,ISNULL(MaxPrice,0) as MaxPrice,
                            ISNULL(MinQty,0) as MinQty,ISNULL(MaxQty,0) as MaxQty,
                            SD.AppTypeID as LPType,SM.ProviderName,
                            CASE WHEN SD.AppTypeID=8 THEN ISNULL((select top 1 AskPrice from TradeTransactionQueueArbitrage TTQ  
                            where TTQ.status=4 and TTQ.TrnType=5 and TTQ.PairID=TM.Id AND TTQ.IsAPITrade=0 Order by TTQ.TrnNo desc ),
                            (select LTP from TradePairStasticsArbitrage where PairId=TM.id))ELSE C.LTP END AS LTP,C.Fees FROM RouteConfigurationArbitrage RC 
                            INNER JOIN  ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID  INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID 
                            INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId 
                            INNER JOIN cryptowatcherarbitrage C ON C.LPType = SD.AppTypeID and C.Pairid=TM.Id
                            Left JOIN LimitsArbitrage LM ON LM.id = RC.limitid 
                            WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1 AND TrnType=5 AND SD.TrnTypeID=5 AND RC.PairId = {0} Order by LTP", PairId);

                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //komal 10-06-2019 For Arbitrage 
        public async Task<ArbitrageCryptoWatcherQryRes> UpdateLTPDataArbitrage(ArbitrageLTPCls LTPData)
        {
            string Qry = "";
            IQueryable<ArbitrageCryptoWatcherQryRes> Result;
            try
            {
                if (LTPData.Volume == 0)
                {
                    Qry = @"update CryptoWatcherArbitrage set LTP = {0},ChangePer = {1}, UpDownBit=CASE WHEN LTP > {0} THEN 1  WHEN LTP < {0} THEN 0 ELSE UpDownBit END,UpdatedBy=1 ,UpdateDate=dbo.getistdate() where  PairId = {2} and LPType = {3}";
                    _dbContext.Database.ExecuteSqlCommand(Qry, LTPData.Price, LTPData.ChangePer, LTPData.PairID, LTPData.LpType);

                    Result = _dbContext.ArbitrageCryptoWatcherQryRes.FromSql(
                        "select Pair,LPType,LTP as Price,Volume,PairId,UpDownBit,ChangePer,Fees from " +
                        "CryptoWatcherArbitrage Where LpType={0} AND PairID={1}", LTPData.LpType, LTPData.PairID);
                    //var Res = Result.FirstOrDefault();
                }
                else
                {
                    Qry = @"update CryptoWatcherArbitrage set LTP = {0},Volume= {1} ,ChangePer = {2}, UpDownBit=CASE WHEN LTP > {0} THEN 1  WHEN LTP < {0} THEN 0 ELSE UpDownBit END ,UpdatedBy=1,UpdateDate=dbo.getistdate() where  PairId = {3} and LPType = {4} ";
                    _dbContext.Database.ExecuteSqlCommand(Qry, LTPData.Price, LTPData.Volume, LTPData.ChangePer, LTPData.PairID, LTPData.LpType);

                    Result = _dbContext.ArbitrageCryptoWatcherQryRes.FromSql(
                        "select Pair,LPType,LTP as Price,Volume,PairId,UpDownBit,ChangePer,Fees from " +
                        "CryptoWatcherArbitrage Where LpType={0} AND PairID={1}", LTPData.LpType, LTPData.PairID);
                    //var Res = Result.FirstOrDefault();
                }
                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public ArbitrageCryptoWatcherQryRes InsertLTPDataArbitrage(ArbitrageLTPCls LTPData)
        {
            string Qry = "";
            IQueryable<ArbitrageCryptoWatcherQryRes> Result;
            try
            {
                //case Added for Duplicate record
                Result = _dbContext.ArbitrageCryptoWatcherQryRes.FromSql(
                         "select Pair,LPType,LTP as Price,Volume,PairId,UpDownBit,ChangePer,Fees from " +
                         "CryptoWatcherArbitrage Where LpType={0} AND PairID={1}", LTPData.LpType, LTPData.PairID);
                if (Result.FirstOrDefault() == null)
                {
                    Qry = @"insert into CryptoWatcherArbitrage values ({0} ,{1},{2},{3},{4},{5},{6},{7},1,dbo.getistdate())";
                    var res = _dbContext.Database.ExecuteSqlCommand(Qry, LTPData.Price, LTPData.Pair, LTPData.LpType, LTPData.ChangePer, LTPData.Fees, LTPData.PairID, LTPData.UpDownBit, LTPData.Volume, 1);

                    if (res > 0)
                    {
                        Result = _dbContext.ArbitrageCryptoWatcherQryRes.FromSql(
                             "select Pair,LPType,LTP as Price,Volume,PairId,UpDownBit,ChangePer,Fees from " +
                             "CryptoWatcherArbitrage Where LpType={0} AND PairID={1}", LTPData.LpType, LTPData.PairID);
                        var Res = Result.FirstOrDefault();
                        return Res;
                    }
                }
                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        public bool GetLocalConfigurationDataArbitrage(short LPType)
        {
            string Qry = "";
            try
            {
                Qry = @"insert into cryptowatcherArbitrage (LTP,pair,Lptype,ChangePer,Fees,PairID,UpDownBit,Volume) 
                        select distinct TP.LTP, TM.PairName as Pair , 8  as Lptype ,TP.ChangePer24 as ChangePer,0 as Fees,TP.PairID,1 as UpDownBit, TP.ChangeVol24 as Volume
                        FROM  TradePairMasterArbitrage TM 
                        INNER JOIN  TradePairStasticsArbitrage TP ON TP.PairId = TM.id 
                        WHERE TM.Status = 1 and TM.PairName not in (select pair from cryptowatcherArbitrage where LPType = {0});
                        UPDATE cryptowatcherArbitrage SET cryptowatcherArbitrage.LTP = TP.LTP ,cryptowatcherArbitrage.Volume=TP.ChangeVol24,cryptowatcherArbitrage.ChangePer=TP.ChangePer24,cryptowatcherArbitrage.UpDownBit=TP.UpDownBit
                        FROM  TradePairMasterArbitrage TM 
                        INNER JOIN  TradePairStasticsArbitrage TP ON TP.PairId = TM.id 
                        WHERE TM.Status = 1 and  cryptowatcherArbitrage.LPType = {0} and cryptowatcherArbitrage.pair = TM.PairName";

                var res = _dbContext.Database.ExecuteSqlCommand(Qry, LPType);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }
        public LPKeyVault GetTradeFeesLPArbitrage(long LPType)
        {
            IQueryable<LPKeyVault> Result = null;
            string Qry = "";
            LPKeyVault Data = new LPKeyVault();
            try
            {
                Qry = @"select top 1 APIKey,SecretKey,AppTypeID from ServiceProviderDetailArbitrage SD
                        INNER JOIN ServiceProConfigurationArbitrage SC ON SC.Id = SD.ServiceProConfigID
                        where AppTypeID={0}";

                Result = _dbContext.LPKeyVault.FromSql(Qry, LPType);

                if (Result != null)
                    Data = Result.ToList().FirstOrDefault();

                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return Data;
            }
        }
        //Darshan dholakiya added this method for Trade pair arbitrage changes:07-06-2019
        public List<TradePairTableResponse> GetTradePairAssetArbitrageInfo(long BaseId = 0)
        {
            try
            {
                IQueryable<TradePairTableResponse> Result;
                if (BaseId == 0)
                {
                    Result = _dbContext.TradePairTableResponse.FromSql(
                                @"Select ISnull(TPD.QtyLength,0) as QtyLength,ISnull(TPD.PriceLength,0) as PriceLength,ISnull(TPD.AmtLength,0) as AmtLength, SM1.Id As BaseId,SM1.Name As BaseName,SM1.SMSCode As BaseCode,TPM.ID As PairId,TPM.PairName As Pairname,TPS.CurrentRate As Currentrate,TPD.BuyFees As BuyFees,TPD.SellFees As SellFees,
                                    SM2.Name As ChildCurrency,SM2.SMSCode As Abbrevation,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,TPS.High24Hr AS High24Hr,TPS.Low24Hr As Low24Hr,
                                    TPS.HighWeek As HighWeek,TPS.LowWeek As LowWeek,TPS.High52Week AS High52Week,TPS.Low52Week As Low52Week,TPS.UpDownBit As UpDownBit,TPM.Priority from MarketArbitrage M 
                                    Inner Join TradePairMasterArbitrage TPM ON TPM.BaseCurrencyId = M.ServiceID
                                    Inner Join TradePairDetailArbitrage TPD ON TPD.PairId = TPM.Id
                                    Inner Join TradePairStasticsArbitrage TPS ON TPS.PairId = TPM.Id
                                    Inner Join ServiceMasterArbitrage SM1 ON SM1.Id = TPM.BaseCurrencyId
                                    Inner Join ServiceMasterArbitrage SM2 ON SM2.Id = TPM.SecondaryCurrencyId Where TPM.Status = 1 And M.Status = 1 Order By M.ID");
                }
                else
                {
                    Result = _dbContext.TradePairTableResponse.FromSql(
                                @"Select ISnull(TPD.QtyLength,0) as QtyLength,ISnull(TPD.PriceLength,0) as PriceLength,ISnull(TPD.AmtLength,0) as AmtLength,SM1.Id As BaseId,SM1.Name As BaseName,SM1.SMSCode As BaseCode,TPM.ID As PairId,TPM.PairName As Pairname,TPS.CurrentRate As Currentrate,TPD.BuyFees As BuyFees,TPD.SellFees As SellFees,
                                    SM2.Name As ChildCurrency,SM2.SMSCode As Abbrevation,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,TPS.High24Hr AS High24Hr,TPS.Low24Hr As Low24Hr,
                                    TPS.HighWeek As HighWeek,TPS.LowWeek As LowWeek,TPS.High52Week AS High52Week,TPS.Low52Week As Low52Week,TPS.UpDownBit As UpDownBit,TPM.Priority from MarketArbitrage M 
                                    Inner Join TradePairMasterArbitrage TPM ON TPM.BaseCurrencyId = M.ServiceID
                                    Inner Join TradePairDetailArbitrage TPD ON TPD.PairId = TPM.Id
                                    Inner Join TradePairStasticsArbitrage TPS ON TPS.PairId = TPM.Id
                                    Inner Join ServiceMasterArbitrage SM1 ON SM1.Id = TPM.BaseCurrencyId
                                    Inner Join ServiceMasterArbitrage SM2 ON SM2.Id = TPM.SecondaryCurrencyId Where TPM.Status = 1 And M.Status = 1 And M.ServiceID = {0}", BaseId);
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //khuhsali 10-06-2019 Route configuration wise exchange info
        public ExchangeProviderListArbitrage GetExchangeProviderListArbitrageRouteWise(long RouteID)
        {
            try
            {
                IQueryable<ExchangeProviderListArbitrage> Result;
                Result = _dbContext.ExchangeProviderListArbitrage.FromSql(
                            @"select C.LPType,RC.ID as RouteID,rc.ordertype,RC.RouteName,SM.ID as ProviderID,SM.ProviderName,
                            SD.ID as SerProDetailID,SD.TrnTypeID as TrnType,C.LTP
                            FROM RouteConfigurationArbitrage RC 
                            INNER JOIN  ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID  
                            INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID 
                            INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId
                            INNER JOIN cryptowatcherarbitrage C ON C.LPType = SD.AppTypeID and c.Pair=TM.PairName
                            WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1 AND RC.ID = {0}", RouteID);

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        public List<ServiceMasterResponse> GetAllServiceConfigurationArbitrage(int StatusData = 0)
        {
            try
            {
                IQueryable<ServiceMasterResponse> Result = null;
                if (StatusData == 0)
                {
                    Result = _dbContext.ServiceMasterResponse.FromSql(
                                @"Select SM.IsIntAmountAllow as IsOnlyIntAmountAllow,SM.Id As ServiceId,SM.Name As ServiceName,SM.SMSCode,SM.ServiceType,SD.ServiceDetailJson,
                            SS.CirculatingSupply,SS.IssueDate,SS.IssuePrice,SM.Status,SM.WalletTypeID,
                            ISNULL((Select STM.Status From ServiceTypeMappingArbitrage STM Where STM.ServiceId = SM.Id and TrnType = 1),0) TransactionBit,
                            ISNULL((Select STM.Status From ServiceTypeMappingArbitrage STM Where STM.ServiceId = SM.Id and TrnType = 6),0) WithdrawBit,
                            ISNULL((Select STM.Status From ServiceTypeMappingArbitrage STM Where STM.ServiceId = SM.Id and TrnType = 8),0) DepositBit
                            From ServiceMasterArbitrage SM
                            Inner Join ServiceDetailArbitrage SD On SD.ServiceId = SM.Id
                            Inner Join ServiceStasticsArbitrage SS On SS.ServiceId = SM.Id Where SM.Status = 1");
                }
                else
                {
                    Result = _dbContext.ServiceMasterResponse.FromSql(
                                @"Select SM.IsIntAmountAllow as IsOnlyIntAmountAllow,SM.Id As ServiceId,SM.Name As ServiceName,SM.SMSCode,SM.ServiceType,SD.ServiceDetailJson,
                            SS.CirculatingSupply,SS.IssueDate,SS.IssuePrice,SM.Status,SM.WalletTypeID, 
                            ISNULL((Select STM.Status From ServiceTypeMappingArbitrage STM Where STM.ServiceId = SM.Id and TrnType = 1),0) TransactionBit,
                            ISNULL((Select STM.Status From ServiceTypeMappingArbitrage STM Where STM.ServiceId = SM.Id and TrnType = 6),0) WithdrawBit,
                            ISNULL((Select STM.Status From ServiceTypeMappingArbitrage STM Where STM.ServiceId = SM.Id and TrnType = 8),0) DepositBit
                            From ServiceMasterArbitrage SM
                            Inner Join ServiceDetailArbitrage SD On SD.ServiceId = SM.Id
                            Inner Join ServiceStasticsArbitrage SS On SS.ServiceId = SM.Id Where SM.Status = 1 Or SM.Status = 0");
                }
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public LocalPairStatisticsQryRes GetLocalPairStatistics(long Pair)
        {
            try
            {
                IQueryable<LocalPairStatisticsQryRes> Result = null;
                Result = _dbContext.LocalPairStatisticsQryRes.FromSql(
                    "select ChangePer24 as ChangePer,ChangeVol24 as Volume,UpDownBit from TradePairStasticsArbitrage where PairID={0}", Pair);
                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        public GetTradeSettlePrice GetTradeSettlementPriceArbitrage(long TrnNo)
        {
            try
            {
                var res = _dbContext.GetTradeSettlePrice.FromSql("SELECT avg(TakerPrice) AS SettlementPrice FROM TradePoolQueueArbitrageV1 WHERE (TakerTrnNo={0} OR MakerTrnNo={0}) AND STATUS=1", TrnNo).FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
                return null;
            }
        }
        #endregion

        public void UpdateTradeTransactionQueueAPIStatus(long TrnNo, string APIStatus)
        {
            try
            {
                _dbContext.Database.ExecuteSqlCommand("update TradeTransactionQueueArbitrage set APIStatus={0},UpdatedDate=dbo.GetISTDate() where Trnno={1}", APIStatus, TrnNo);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

        public CheckArbitrageTransactionStatus CheckArbitrageTransactionStatus(string TrnNo, short SmaartTradePriority)
        {
            CheckArbitrageTransactionStatus res = new CheckArbitrageTransactionStatus();
            short TrnType = 0;
            try
            {
                if (SmaartTradePriority == 1)
                    TrnType = 4;
                else
                    TrnType = 5;
                res = _dbContext.CheckArbitrageTransactionStatus.FromSql("SELECT TOP 1 TTQ.Status FROM TransactionQueueArbitrage TQ INNER JOIN TradeTransactionQueueArbitrage TTQ ON TQ.ID=TTQ.Trnno WHERE TQ.GUID LIKE {0} AND TTQ.TrnType={1}", TrnNo, TrnType).FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        #region Market Maker Queries
        public int GetMarketMakerUserRole()
        {
            try
            {
                /* Check for MarkerMaker User is exist or not with his Role. -Sahil 16-09-2019 03:51 PM
               Where roleId = MARKETMAKER: value is for MarketMaker Role defined from BizRoles table and join has been performed on role id
               Return: BizUserRole if record present else return null
            */
                string query = @"SELECT Top(1) u.* FROM BizUserRole u INNER JOIN BizRoles r ON (u.RoleId = r.Id) WHERE r.NormalizedName = 'MARKETMAKER'  AND r.Status = 1";

                var result = _dbContext.BizUserRole.FromSql(query).FirstOrDefault();
                if (result != null) return result.UserId; // return userId if given userId is presents in table with its role else return null -Sahil 02-10-2019 7:30 PM 

                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public string GetMarketMakerHoldOrderRateChange(long pairId)
        {

            try
            {
                string query = @"SELECT Top(1) HoldOrderRateChange
	                            FROM BizUserRole u INNER JOIN BizRoles r
	                            ON (u.RoleId = r.Id) 
	                            INNER JOIN MarketMakerPreferences m 
	                            ON (u.UserId = m.UserId) 
	                            WHERE m.PairId = {0} AND r.NormalizedName = 'MARKETMAKER'
			                            AND m.Status = 1 AND r.Status = 1";

                //var result = _dbContext.MarketMakerBuyPreferences.FromSql(query, pairId).FirstOrDefault();
                var result = _dbContext.GetMarketMakerHoldOrderRateChange.FromSql(query, pairId).FirstOrDefault();


                if (result != null) return result.HoldOrderRateChange;

                return ""; // return "" if rate HoldOrderRateChange not get -Sahil 16-10-2019 04:46 PM
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return "";
            }
        }

        public List<MarketMakerSettleTrxnByTakeViewModel> GetMarketMakerSettledByTakerList(long TakerTrnno, long MarketMakerID)
        {
            List<MarketMakerSettleTrxnByTakeViewModel> Result = null;
            try
            {
                Result = _dbContext.MarketMakerSettleTrxnByTakeViewModel.FromSql(
                    @"select TTQ.TrnType,TTQ.Order_Currency as SMSCode,TTQ.PairID,TTQ.MemberId,TTQ.orderWalletID,TTQ.deliveryWalletID,TTQ.OrderType,PQ.MakerPrice as Price,PQ.TakerQty as Qty,'' as TransactionAccount
                    from TradePoolQueueV1 PQ INNER JOIN TradeTransactionQueue TTQ ON PQ.MakerTrnNo=TTQ.TrnNo
                    where PQ.takertrnno in ({0}) and PQ.status=1 and TTQ.MemberID={1} ", TakerTrnno, MarketMakerID).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        public MarketMakerBuyPreferencesViewModel GetMarketMakerUserBuyPreferences(long pairId)
        {
            try
            {
                /* Get list of User's Buy Exchange and trading preferences -Sahil 27-09-2019
                   Remarks: Query eliminates records whose buy LP id has not presence in ServiceProviderMaster table
                */
                string query = @"SELECT m.Id, m.UserId, m.PairId, t.PairName, m.BuyLTPPrefProID, m.BuyUpPercentage, m.BuyDownPercentage, m.BuyThreshold, m.BuyLTPRangeType , s.ProviderName
                                    FROM BizUserRole u INNER JOIN BizRoles r
									ON (u.RoleId = r.Id) 
									INNER JOIN MarketMakerPreferences m 
									ON (u.UserId = m.UserId) 
									INNER JOIN ServiceProviderMaster s 
                                    ON (m.BuyLTPPrefProID = s.Id)
                                    INNER JOIN TradePairMaster t
                                    ON (m.PairId = t.Id)
                                    WHERE m.PairId = {0} AND r.NormalizedName = 'MARKETMAKER'
	                                      AND m.Status = 1 AND s.Status = 1 AND t.Status = 1 AND r.Status = 1";

                var result = _dbContext.MarketMakerBuyPreferences.FromSql(query, pairId).FirstOrDefault();

                return result; //return null if preference not found -Sahil 12-10-2019 12:36 PM
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);

                return null;
            }
        }

        public MarketMakerSellPreferencesViewModel GetMarketMakerUserSellPreferences(long pairId)
        {
            try
            {
                /* Get list of User's Sell Exchange and trading preferences -Sahil 27-09-2019
                   Remarks: Query eliminates records whose sell LP id has not presence in ServiceProviderMaster table
                */
                string query = @"SELECT m.Id, m.UserId, m.PairId, t.PairName, m.SellLTPPrefProID, m.SellUpPercentage, m.SellDownPercentage, m.SellThreshold, m.SellLTPRangeType , s.ProviderName
                                    FROM BizUserRole u INNER JOIN BizRoles r
									ON (u.RoleId = r.Id) 
									INNER JOIN MarketMakerPreferences m 
									ON (u.UserId = m.UserId) 
									INNER JOIN ServiceProviderMaster s 
                                    ON (m.SellLTPPrefProID = s.Id)
                                    INNER JOIN TradePairMaster t
                                    ON (m.PairId = t.Id)
                                    WHERE m.PairId = {0} AND r.NormalizedName = 'MARKETMAKER'
	                                      AND m.Status = 1 AND s.Status = 1 AND t.Status = 1 AND r.Status = 1";

                var result = _dbContext.MarketMakerSellPreferences.FromSql(query, pairId).FirstOrDefault();

                return result; //return null if preference not found -Sahil 12-10-2019 12:36 PM
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);

                return null;
            }
        }
        #endregion
    }
}
