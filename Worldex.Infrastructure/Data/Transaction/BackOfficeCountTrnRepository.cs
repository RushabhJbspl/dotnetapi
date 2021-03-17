using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Transaction.BackOfficeCount;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Data.Transaction
{
    public class BackOfficeCountTrnRepository : IBackOfficeCountTrnRepository
    {
        private readonly WorldexContext _dbContext;

        public BackOfficeCountTrnRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Count method

        public ActiveTradeUserCount GetActiveTradeUserCount()
        {
            try
            {
                IQueryable<ActiveTradeUserCount> Result;

                Result = _dbContext.ActiveTradeUserCount.FromSql(
                            @"Select Cast(Count(TTQ.TrnNo) As BigInt) As TotalCount,
                                Cast(Count(Case When TTQ.Status In (1,2,4,3) And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As TodayCount,
                                Cast(Count(Case When TTQ.Status In (4) And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodayActiveCount,
                                Cast(Count(Case When TTQ.Status In (2) And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodayCancelCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 0 And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodaySettleCount,
                                Cast(Count(Case When TTQ.Status In (3) And DAY(TTQ.TrnDate) = DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodaySystemFailCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 1 And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodayPartialCancelCount,
                                Cast(Count(Case When TTQ.Status In (1,2,4,3) And TTQ.TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) 
                                AND TTQ.TrnDate <  dateadd(day, 8-datepart(dw, getdate()), CONVERT(date,getdate())) Then 1 End) As BigInt) As WeekCount,
                                Cast(Count(Case When TTQ.Status In (4) And TTQ.TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) 
                                AND TTQ.TrnDate <  dateadd(day, 8-datepart(dw, getdate()), CONVERT(date,getdate())) Then 1 End) As BigInt) As WeekActiveCount,
                                Cast(Count(Case When TTQ.Status In (2) And TTQ.TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) 
                                AND TTQ.TrnDate <  dateadd(day, 8-datepart(dw, getdate()), CONVERT(date,getdate())) Then 1 End) As BigInt) As WeekCancelCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 0 And TTQ.TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) 
                                AND TTQ.TrnDate <  dateadd(day, 8-datepart(dw, getdate()), CONVERT(date,getdate())) Then 1 End) As BigInt) As WeekSettleCount,
                                Cast(Count(Case When TTQ.Status In (3) And TTQ.TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) 
                                AND TTQ.TrnDate <  dateadd(day, 8-datepart(dw, getdate()), CONVERT(date,getdate())) Then 1 End) As BigInt) As WeekSystemFailCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 1 And TTQ.TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) 
                                AND TTQ.TrnDate <  dateadd(day, 8-datepart(dw, getdate()), CONVERT(date,getdate())) Then 1 End) As BigInt) As WeekPartialCancelCount,
                                Cast(Count(Case When TTQ.Status In (1,2,4,3) And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthCount,
                                Cast(Count(Case When TTQ.Status In (4) And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthActiveCount,
                                Cast(Count(Case When TTQ.Status In (2) And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthCancelCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 0 And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthSettleCount,
                                Cast(Count(Case When TTQ.Status In (3) And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthSystemFailCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 1 And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthPartialCancelCount,
                                Cast(Count(Case When TTQ.Status In (1,2,4,3) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearCount,
                                Cast(Count(Case When TTQ.Status In (4) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearActiveCount,
                                Cast(Count(Case When TTQ.Status In (2) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearCancelCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 0 And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearSettleCount,
                                Cast(Count(Case When TTQ.Status In (3) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearSystemFailCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 1 And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearPartialCancelCount
                                From TradeTransactionQueue TTQ WHERE TTQ.Status in (1,2,4,3)");

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ConfigurationCount GetConfigurationCount()
        {
            try
            {
                IQueryable<ConfigurationCount> Result = null;
                Result = _dbContext.ConfigurationCount.FromSql(
                                    @"select (Select Cast(count(SM.Id)as bigint)AS ID From ServiceMaster SM Inner Join ServiceDetail SD On SD.ServiceId = SM.Id Inner Join ServiceStastics SS On SS.ServiceId = SM.Id Where SM.Status in (1,0))AS CoinCount, 
                                    (SELECT Cast(count(Id)as bigint)AS ID from TradePairMaster where status<>9)AS PairCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from DemonConfiguration where status<>9)AS DaemonCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from ServiceProConfiguration where status<>9)AS ProviderCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from Market where status<>9)AS MarketCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from ServiceProviderDetail where status<>9)AS LiquidityCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from ThirdPartyAPIConfiguration where status<>9)AS APICount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from ThirdPartyAPIResponseConfiguration where status<>9)AS APIResponseCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from TradePairDetail where status<>9) AS MarketCapTickerCount,
                                    (Select Cast(count(RC.Id)as bigint)AS ID from RouteConfiguration RC Inner Join TradePairMaster TPM On TPM.Id = RC.PairId Inner Join ServiceProviderDetail SPD On SPD.Id = RC.SerProDetailID Left Outer Join ThirdPartyAPIConfiguration TPA On TPA.Id = SPD.ThirPartyAPIID where RC.Status<>9)AS TradeRouteCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from SiteTokenMaster WHERE status<>9)AS SiteToken,
                                    (SELECT Cast(count(Id)as bigint)AS ID from ServiceProviderMaster WHERE status<>9)AS ServiceProviderCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from SocketFeedConfiguration WHERE status<>9)AS ExchangeFeedConfigCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from SocketFeedLimits WHERE status<>9)AS ExchangeFeedLimitsCount,
                                    cast(0 as bigint)as Count");
                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        
        public LedgerCountInfo GetLedgerCount()
        {
            try
            {
                IQueryable<LedgerCountInfo> Result =_dbContext.LedgerCount.FromSql(
                    @"select cast( count(distinct(TTQ.TrnNo)) as bigint) AS LedgerCount from TradeTransactionQueue TTQ INNER Join TradePoolQueueV1 TP ON 
                      (TP.MakerTrnNo=TTQ.TrnNo or TP.TakerTrnNo=TTQ.TrnNo) where  TTQ.Status in (1,4)");
                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        
        public OrderSummaryCount GetOrderSummaryCount()
        {
            try
            {
                IQueryable<OrderSummaryCount> Result;

                //Only Success Transaction Count
                Result = _dbContext.OrderSummaryCount.FromSql(
                            @"Select Cast((Select Count(ID) From SettledTradeTransactionQueue Where Status = 1 And (TrnType = 4 Or TrnType = 5)) As BigInt) As TotalCount,
                            Cast((Select Count(ID) From SettledTradeTransactionQueue Where TrnType = 4 And  Status = 1) As BigInt) As BuyCount,
                            Cast((Select Count(ID) From SettledTradeTransactionQueue Where TrnType = 5 And  Status = 1) As BigInt) As SellCount");

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        
        public TradeSummaryCount GetTradeSummaryCount()
        {
            try
            {
                IQueryable<TradeSummaryCount> Result;

                //Only Success Transaction Count
                // WHERE TTQ.Status in (1,2)"); komal 13 Aug 2019 change status for only settled trade
                Result = _dbContext.TradeSummaryCount.FromSql(
                            @"SELECT CAST(COUNT(TTQ.TrnNo)AS bigint)AS TOTALTRADE,CAST(COUNT(CASE WHEN TTQ.ordertype=1 THEN 1 END)AS bigint)AS LIMIT,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=2 THEN 1 END)AS bigint)AS MARKET,CAST(COUNT(CASE WHEN TTQ.ordertype=3 THEN 1 END)AS bigint)AS SPOT,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=4 THEN 1 END)AS bigint)AS STOP_Limit,CAST(COUNT(CASE WHEN TTQ.ordertype=5 THEN 1 END)AS bigint)AS STOP,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=1 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS LIMIT_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=1 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS LIMIT_SELL,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=2 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS MARKET_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=2 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS MARKET_SELL,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=3 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS SPOT_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=3 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS SPOT_SELL,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=4 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS STOP_Limit_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=4 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS STOP_Limit_SELL,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=5 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS STOP_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=5 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS STOP_SELL
                            FROM TradeTransactionQueue TTQ 
                            WHERE (TTQ.Status =1 or (TTQ.status=4 AND TTQ.SettledSellQty>0))");
                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        
        public ProfitSummaryCount GetProfitSummaryCount()
        {
            try
            {
                IQueryable<ProfitSummaryCount> Result;

                Result = _dbContext.ProfitSummaryCount.FromSql(
                            @"Select Cast((Select 0) As BigInt) As TotalCount,Cast((Select Count(ID) from SettledTradeTransactionQueue Where TrnType = 4 And Status = 1 And (OrderTotalQty-SettledSellQty) > 0) As BigInt) As BuyCount
                              ,Cast((Select Count(ID) from SettledTradeTransactionQueue Where TrnType = 5  And Status = 1 And (OrderTotalQty-SettledSellQty) > 0) As BigInt) As SellCount
                              ,Cast((Select 0) As BigInt) As DepositCount,Cast((Select 0) As BigInt) As WithdrawlCount");
            
                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
       
        public TradeSummaryCount GetTradeUserMarketTypeCount(string Type)
        {
            try
            {
                IQueryable<TradeSummaryCount> Result;

                string query = "SELECT CAST(COUNT(TTQ.TrnNo)AS bigint)AS TOTALTRADE,CAST(COUNT(CASE WHEN TTQ.ordertype=1 THEN 1 END)AS bigint)AS LIMIT," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 2 THEN 1 END)AS bigint)AS MARKET, CAST(COUNT(CASE WHEN TTQ.ordertype = 3 THEN 1 END)AS bigint)AS SPOT," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 4 THEN 1 END)AS bigint)AS STOP_Limit, CAST(COUNT(CASE WHEN TTQ.ordertype = 5 THEN 1 END)AS bigint)AS STOP," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 1 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS LIMIT_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 1 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS LIMIT_SELL," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 2 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS MARKET_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 2 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS MARKET_SELL," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 3 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS SPOT_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 3 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS SPOT_SELL," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 4 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS STOP_Limit_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 4 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS STOP_Limit_SELL," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 5 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS STOP_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 5 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS STOP_SELL" +
                        " FROM TradeTransactionQueue TTQ WHERE TTQ.Status in (1,2,4,3)";

                if (Type.Equals("Today"))
                {
                    query = query + " And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())";
                }
                else if (Type.Equals("Week"))
                {
                    query = query + " And TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) AND TrnDate<dateadd(day, 8 - datepart(dw, getdate()), CONVERT(date, getdate()))";
                }
                else if (Type.Equals("Month"))
                {
                    query = query + " And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate())";
                }
                else if (Type.Equals("Year"))
                {
                    query = query + " And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate())";
                }

                Result = _dbContext.TradeSummaryCount.FromSql(query);

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
       
        public TransactionReportCountResponse TransactionReportCount()
        {
            TransactionReportCountResponse _Res = new TransactionReportCountResponse();
            try
            {
                IQueryable<TransactionReportCountQryRes> Result = null;
                Result = _dbContext.TransactionReportCountQryRes.FromSql(@"SELECT 
                            (SELECT CAST(COUNT(ID) AS bigint) FROM SiteTokenConversion)AS SiteTokenConversionCount,
                            (SELECT CAST(COUNT(TTQ.TrnNo) AS bigint) from TradeTransactionQueue TTQ   LEFT JOIN TransactionQueue TQ ON TQ.Id = TTQ.TrnNo 
                            INNER JOIN ServiceProviderDetail SD ON TQ.SerProID=SD.ServiceProID 
                            WHERE TTQ.TrnType in (4,5) And TTQ.Status In (1,2,3,4) )AS TradeRoutingCount,
                            (SELECT CAST(COUNT(ID) AS bigint) FROM TradingRecon )AS TradeReconCount,cast(0 as bigint)as TotalCount");
                _Res.Response = Result.FirstOrDefault();
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Margin Method
        //Rita 6-3-19 for Margin Trading
        public ActiveTradeUserCount GetActiveTradeUserCountMargin()
        {
            try
            {
                IQueryable<ActiveTradeUserCount> Result;

                Result = _dbContext.ActiveTradeUserCount.FromSql(
                            @"Select Cast(Count(TTQ.TrnNo) As BigInt) As TotalCount,
                                Cast(Count(Case When TTQ.Status In (1,2,4,3) And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As TodayCount,
                                Cast(Count(Case When TTQ.Status In (4) And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodayActiveCount,
                                Cast(Count(Case When TTQ.Status In (2) And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodayCancelCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 0 And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodaySettleCount,
                                Cast(Count(Case When TTQ.Status In (3) And DAY(TTQ.TrnDate) = DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodaySystemFailCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 1 And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodayPartialCancelCount,
                                Cast(Count(Case When TTQ.Status In (1,2,4,3) And TTQ.TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) 
                                AND TTQ.TrnDate <  dateadd(day, 8-datepart(dw, getdate()), CONVERT(date,getdate())) Then 1 End) As BigInt) As WeekCount,
                                Cast(Count(Case When TTQ.Status In (4) And TTQ.TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) 
                                AND TTQ.TrnDate <  dateadd(day, 8-datepart(dw, getdate()), CONVERT(date,getdate())) Then 1 End) As BigInt) As WeekActiveCount,
                                Cast(Count(Case When TTQ.Status In (2) And TTQ.TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) 
                                AND TTQ.TrnDate <  dateadd(day, 8-datepart(dw, getdate()), CONVERT(date,getdate())) Then 1 End) As BigInt) As WeekCancelCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 0 And TTQ.TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) 
                                AND TTQ.TrnDate <  dateadd(day, 8-datepart(dw, getdate()), CONVERT(date,getdate())) Then 1 End) As BigInt) As WeekSettleCount,
                                Cast(Count(Case When TTQ.Status In (3) And TTQ.TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) 
                                AND TTQ.TrnDate <  dateadd(day, 8-datepart(dw, getdate()), CONVERT(date,getdate())) Then 1 End) As BigInt) As WeekSystemFailCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 1 And TTQ.TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) 
                                AND TTQ.TrnDate <  dateadd(day, 8-datepart(dw, getdate()), CONVERT(date,getdate())) Then 1 End) As BigInt) As WeekPartialCancelCount,
                                Cast(Count(Case When TTQ.Status In (1,2,4,3) And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthCount,
                                Cast(Count(Case When TTQ.Status In (4) And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthActiveCount,
                                Cast(Count(Case When TTQ.Status In (2) And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthCancelCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 0 And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthSettleCount,
                                Cast(Count(Case When TTQ.Status In (3) And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthSystemFailCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 1 And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthPartialCancelCount,
                                Cast(Count(Case When TTQ.Status In (1,2,4,3) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearCount,
                                Cast(Count(Case When TTQ.Status In (4) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearActiveCount,
                                Cast(Count(Case When TTQ.Status In (2) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearCancelCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 0 And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearSettleCount,
                                Cast(Count(Case When TTQ.Status In (3) And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearSystemFailCount,
                                Cast(Count(Case When TTQ.Status In (1) And TTQ.IsCancelled = 1 And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearPartialCancelCount
                                From TradeTransactionQueueMargin TTQ WHERE TTQ.Status in (1,2,4,3)");

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rita 6-3-19 for Margin Trading
        public ConfigurationCount GetConfigurationCountMargin()
        {
            try
            {
                IQueryable<ConfigurationCount> Result = null;
                Result = _dbContext.ConfigurationCount.FromSql(
                                    @"select (Select Cast(count(SM.Id)as bigint)AS ID From ServiceMasterMargin SM Inner Join ServiceDetailMargin SD On SD.ServiceId = SM.Id Inner Join ServiceStasticsMargin SS On SS.ServiceId = SM.Id Where SM.Status in (1,0))AS CoinCount, 
                                    (SELECT Cast(count(Id)as bigint)AS ID from TradePairMasterMargin where status<>9)AS PairCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from MarketMargin where status<>9)AS MarketCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from TradePairDetailMargin where status<>9) AS MarketCapTickerCount,
                                    Cast(0 as bigint) As APICount,Cast(0 as bigint) AS APIResponseCount,Cast(0 as bigint) As DaemonCount,Cast(0 as bigint) AS LiquidityCount,
                                    Cast(0 as bigint) AS ProviderCount,Cast(0 as bigint) As TradeRouteCount,cast(0 as bigint)as Count,
                                    (SELECT Cast(count(Id)as bigint)AS ID from SiteTokenMasterMargin WHERE status<>9)AS SiteToken,Cast(0 as bigint) AS ServiceProviderCount,Cast(0 as bigint) AS ExchangeFeedConfigCount,Cast(0 as bigint) AS ExchangeFeedLimitsCount");
                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rita 6-3-19 for Margin Trading
        public LedgerCountInfo GetLedgerCountMargin()
        {
            try
            {
                IQueryable<LedgerCountInfo> Result = _dbContext.LedgerCount.FromSql(
                    @"select cast( count(distinct(TTQ.TrnNo)) as bigint) AS LedgerCount from TradeTransactionQueueMargin TTQ INNER Join TradePoolQueueMarginV1 TP ON 
                      (TP.MakerTrnNo=TTQ.TrnNo or TP.TakerTrnNo=TTQ.TrnNo) where  TTQ.Status in (1,4)");
                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rita 6-3-19 for Margin Trading
        public OrderSummaryCount GetOrderSummaryCountMargin()
        {
            try
            {
                IQueryable<OrderSummaryCount> Result;

                Result = _dbContext.OrderSummaryCount.FromSql(
                            @"Select Cast((Select Count(ID) From SettledTradeTransactionQueueMargin Where Status = 1 And (TrnType = 4 Or TrnType = 5)) As BigInt) As TotalCount,
                            Cast((Select Count(ID) From SettledTradeTransactionQueueMargin Where TrnType = 4 And  Status = 1) As BigInt) As BuyCount,
                            Cast((Select Count(ID) From SettledTradeTransactionQueueMargin Where TrnType = 5 And  Status = 1) As BigInt) As SellCount");


                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rita 6-3-19 for Margin Trading
        public TradeSummaryCount GetTradeSummaryCountMargin()
        {
            try
            {
                IQueryable<TradeSummaryCount> Result;

                //Only Success Transaction Count
                Result = _dbContext.TradeSummaryCount.FromSql(
                            @"SELECT CAST(COUNT(TTQ.TrnNo)AS bigint)AS TOTALTRADE,CAST(COUNT(CASE WHEN TTQ.ordertype=1 THEN 1 END)AS bigint)AS LIMIT,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=2 THEN 1 END)AS bigint)AS MARKET,CAST(COUNT(CASE WHEN TTQ.ordertype=3 THEN 1 END)AS bigint)AS SPOT,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=4 THEN 1 END)AS bigint)AS STOP_Limit,CAST(COUNT(CASE WHEN TTQ.ordertype=5 THEN 1 END)AS bigint)AS STOP,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=1 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS LIMIT_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=1 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS LIMIT_SELL,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=2 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS MARKET_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=2 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS MARKET_SELL,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=3 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS SPOT_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=3 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS SPOT_SELL,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=4 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS STOP_Limit_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=4 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS STOP_Limit_SELL,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=5 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS STOP_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=5 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS STOP_SELL
                            FROM TradeTransactionQueueMargin TTQ 
                            WHERE (TTQ.Status =1 or (TTQ.status=4 AND TTQ.SettledSellQty>0))");
                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rita 6-3-19 for Margin Trading
        public ProfitSummaryCount GetProfitSummaryCountMargin()
        {
            try
            {
                IQueryable<ProfitSummaryCount> Result;

                Result = _dbContext.ProfitSummaryCount.FromSql(
                            @"Select Cast((Select 0) As BigInt) As TotalCount,Cast((Select Count(ID) from SettledTradeTransactionQueueMargin Where TrnType = 4 And Status = 1 And (OrderTotalQty-SettledSellQty) > 0) As BigInt) As BuyCount
                              ,Cast((Select Count(ID) from SettledTradeTransactionQueueMargin Where TrnType = 5  And Status = 1 And (OrderTotalQty-SettledSellQty) > 0) As BigInt) As SellCount
                              ,Cast((Select 0) As BigInt) As DepositCount,Cast((Select 0) As BigInt) As WithdrawlCount");

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rita 6-3-19 for Margin Trading
        public TradeSummaryCount GetTradeUserMarketTypeCountMargin(string Type)
        {
            try
            {
                IQueryable<TradeSummaryCount> Result;

                string query = "SELECT CAST(COUNT(TTQ.TrnNo)AS bigint)AS TOTALTRADE,CAST(COUNT(CASE WHEN TTQ.ordertype=1 THEN 1 END)AS bigint)AS LIMIT," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 2 THEN 1 END)AS bigint)AS MARKET, CAST(COUNT(CASE WHEN TTQ.ordertype = 3 THEN 1 END)AS bigint)AS SPOT," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 4 THEN 1 END)AS bigint)AS STOP_Limit, CAST(COUNT(CASE WHEN TTQ.ordertype = 5 THEN 1 END)AS bigint)AS STOP," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 1 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS LIMIT_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 1 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS LIMIT_SELL," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 2 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS MARKET_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 2 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS MARKET_SELL," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 3 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS SPOT_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 3 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS SPOT_SELL," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 4 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS STOP_Limit_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 4 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS STOP_Limit_SELL," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 5 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS STOP_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 5 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS STOP_SELL" +
                        " FROM TradeTransactionQueueMargin TTQ WHERE TTQ.Status in (1,2,4,3)";

                if (Type.Equals("Today"))
                {
                    query = query + " And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())";
                }
                else if (Type.Equals("Week"))
                {
                    query = query + " And TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) AND TrnDate<dateadd(day, 8 - datepart(dw, getdate()), CONVERT(date, getdate()))";
                }
                else if (Type.Equals("Month"))
                {
                    query = query + " And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate())";
                }
                else if (Type.Equals("Year"))
                {
                    query = query + " And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate())";
                }

                Result = _dbContext.TradeSummaryCount.FromSql(query);

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Arbitrage Method

        public ActiveTradeUserCount GetActiveTradeUserCountArbitrage(string Status)
        {
            String Qry = "",Condition= " TTQ.Status in (1, 2, 4, 3) ";
            IQueryable<ActiveTradeUserCount> Result;
            try
            {
                //default All =>  TTQ.Status in (1, 2, 4, 3)
                if (Status == "Active")
                    Condition = " TTQ.Status in (4) ";
                if (Status == "SuccessCancel")
                    Condition = " TTQ.Status in (1, 2) ";

                Qry = "Select Cast(Count(TTQ.TrnNo) As BigInt) As TotalCount, "+
                        "Cast(Count(Case When TTQ.Status In(1, 2, 4, 3) And DAY(TTQ.TrnDate) = DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As TodayCount, "+
                        "Cast(Count(Case When TTQ.Status In(4) And DAY(TTQ.TrnDate) = DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodayActiveCount, "+
                        "Cast(Count(Case When TTQ.Status In(2) And DAY(TTQ.TrnDate) = DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodayCancelCount, "+
                        "Cast(Count(Case When TTQ.Status In(1) And TTQ.IsCancelled = 0 And DAY(TTQ.TrnDate) = DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodaySettleCount, "+
                        "Cast(Count(Case When TTQ.Status In(3) And DAY(TTQ.TrnDate) = DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodaySystemFailCount, "+
                        "Cast(Count(Case When TTQ.Status In(1) And TTQ.IsCancelled = 1 And DAY(TTQ.TrnDate) = DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())  Then 1 End) As BigInt) As TodayPartialCancelCount, "+
                        "Cast(Count(Case When TTQ.Status In(1, 2, 4, 3) And TTQ.TrnDate >= dateadd(day, 1 - datepart(dw, getdate()), CONVERT(date, getdate())) AND TTQ.TrnDate < dateadd(day, 8 - datepart(dw, getdate()), CONVERT(date, getdate())) Then 1 End) As BigInt) As WeekCount, "+
                        "Cast(Count(Case When TTQ.Status In(4) And TTQ.TrnDate >= dateadd(day, 1 - datepart(dw, getdate()), CONVERT(date, getdate())) AND TTQ.TrnDate < dateadd(day, 8 - datepart(dw, getdate()), CONVERT(date, getdate())) Then 1 End) As BigInt) As WeekActiveCount, "+
                        "Cast(Count(Case When TTQ.Status In(2) And TTQ.TrnDate >= dateadd(day, 1 - datepart(dw, getdate()), CONVERT(date, getdate())) AND TTQ.TrnDate < dateadd(day, 8 - datepart(dw, getdate()), CONVERT(date, getdate())) Then 1 End) As BigInt) As WeekCancelCount, "+
                        "Cast(Count(Case When TTQ.Status In(1) And TTQ.IsCancelled = 0 And TTQ.TrnDate >= dateadd(day, 1 - datepart(dw, getdate()), CONVERT(date, getdate())) AND TTQ.TrnDate < dateadd(day, 8 - datepart(dw, getdate()), CONVERT(date, getdate())) Then 1 End) As BigInt) As WeekSettleCount, "+
                        "Cast(Count(Case When TTQ.Status In(3) And TTQ.TrnDate >= dateadd(day, 1 - datepart(dw, getdate()), CONVERT(date, getdate())) AND TTQ.TrnDate < dateadd(day, 8 - datepart(dw, getdate()), CONVERT(date, getdate())) Then 1 End) As BigInt) As WeekSystemFailCount, "+
                        "Cast(Count(Case When TTQ.Status In(1) And TTQ.IsCancelled = 1 And TTQ.TrnDate >= dateadd(day, 1 - datepart(dw, getdate()), CONVERT(date, getdate())) AND TTQ.TrnDate < dateadd(day, 8 - datepart(dw, getdate()), CONVERT(date, getdate())) Then 1 End) As BigInt) As WeekPartialCancelCount, "+
                        "Cast(Count(Case When TTQ.Status In(1, 2, 4, 3) And Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthCount, "+
                        "Cast(Count(Case When TTQ.Status In(4) And Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthActiveCount, "+
                        "Cast(Count(Case When TTQ.Status In(2) And Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthCancelCount, "+
                        "Cast(Count(Case When TTQ.Status In(1) And TTQ.IsCancelled = 0 And Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthSettleCount, "+
                        "Cast(Count(Case When TTQ.Status In(3) And Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthSystemFailCount, "+
                        "Cast(Count(Case When TTQ.Status In(1) And TTQ.IsCancelled = 1 And Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As MonthPartialCancelCount, "+
                        "Cast(Count(Case When TTQ.Status In(1, 2, 4, 3) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearCount, "+
                        "Cast(Count(Case When TTQ.Status In(4) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearActiveCount, "+
                        "Cast(Count(Case When TTQ.Status In(2) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearCancelCount, "+
                        "Cast(Count(Case When TTQ.Status In(1) And TTQ.IsCancelled = 0 And Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearSettleCount, "+
                        "Cast(Count(Case When TTQ.Status In(3) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearSystemFailCount, "+
                        "Cast(Count(Case When TTQ.Status In(1) And TTQ.IsCancelled = 1 And Year(TTQ.TrnDate) = Year(dbo.GetISTDate()) Then 1 End) As BigInt) As YearPartialCancelCount "+
                        "From TradeTransactionQueueArbitrage TTQ WHERE "+ Condition;

                Result = _dbContext.ActiveTradeUserCount.FromSql(Qry);

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ConfigurationCount GetConfigurationCountArbitrage()
        {
            try
            {
                IQueryable<ConfigurationCount> Result = null;
                Result = _dbContext.ConfigurationCount.FromSql(
                                    @"select (Select Cast(count(SM.Id)as bigint)AS ID From ServiceMasterArbitrage SM Inner Join ServiceDetailArbitrage SD On SD.ServiceId = SM.Id Inner Join ServiceStasticsArbitrage SS On SS.ServiceId = SM.Id Where SM.Status in (1,0))AS CoinCount, 
                                    (SELECT Cast(count(Id)as bigint)AS ID from TradePairMasterArbitrage where status<>9)AS PairCount,
                                    cast(0 as bigint)AS DaemonCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from ServiceProConfigurationArbitrage where status<>9)AS ProviderCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from MarketArbitrage where status<>9)AS MarketCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from ServiceProviderDetailArbitrage where status<>9)AS LiquidityCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from ArbitrageThirdPartyAPIConfiguration where status<>9)AS APICount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from ArbitrageThirdPartyAPIResponseConfiguration where status<>9)AS APIResponseCount,
                                    (SELECT Cast(count(Id)as bigint)AS ID from TradePairDetailArbitrage where status<>9) AS MarketCapTickerCount,
                                    (Select Cast(count(RC.Id)as bigint)AS ID from RouteConfigurationArbitrage RC Inner Join TradePairMasterArbitrage TPM On TPM.Id = RC.PairId Inner Join ServiceProviderDetailArbitrage SPD On SPD.Id = RC.SerProDetailID Left Outer Join ArbitrageThirdPartyAPIConfiguration TPA On TPA.Id = SPD.ThirPartyAPIID where RC.Status<>9)AS TradeRouteCount,
                                    cast(0 as bigint)AS SiteToken,
                                    (SELECT Cast(count(Id)as bigint)AS ID from ServiceProviderMasterArbitrage WHERE status<>9)AS ServiceProviderCount,
                                    cast(0 as bigint)AS ExchangeFeedConfigCount,
                                    cast(0 as bigint)AS ExchangeFeedLimitsCount,
                                    cast(0 as bigint)as Count");
                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TradeSummaryCount GetTradeSummaryCountArbitrage()
        {
            try
            {
                IQueryable<TradeSummaryCount> Result;
                //Only Success Transaction Count
                Result = _dbContext.TradeSummaryCount.FromSql(
                            @"SELECT CAST(COUNT(TTQ.TrnNo)AS bigint)AS TOTALTRADE,CAST(COUNT(CASE WHEN TTQ.ordertype=1 THEN 1 END)AS bigint)AS LIMIT,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=2 THEN 1 END)AS bigint)AS MARKET,CAST(COUNT(CASE WHEN TTQ.ordertype=3 THEN 1 END)AS bigint)AS SPOT,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=4 THEN 1 END)AS bigint)AS STOP_Limit,CAST(COUNT(CASE WHEN TTQ.ordertype=5 THEN 1 END)AS bigint)AS STOP,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=1 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS LIMIT_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=1 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS LIMIT_SELL,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=2 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS MARKET_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=2 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS MARKET_SELL,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=3 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS SPOT_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=3 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS SPOT_SELL,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=4 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS STOP_Limit_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=4 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS STOP_Limit_SELL,
                            CAST(COUNT(CASE WHEN TTQ.ordertype=5 AND TTQ.TrnType=4 THEN 1 END)AS bigint)AS STOP_BUY,CAST(COUNT(CASE WHEN TTQ.ordertype=5 AND TTQ.TrnType=5 THEN 1 END)AS bigint)AS STOP_SELL
                            FROM TradeTransactionQueueArbitrage TTQ 
                            WHERE (TTQ.Status =1 or (TTQ.status=4 AND TTQ.SettledSellQty>0))");
                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TradeSummaryCount GetTradeUserMarketTypeCountArbitrage(string Type, string Status)
        {
            try
            {
                IQueryable<TradeSummaryCount> Result;
                string Condition = " TTQ.Status in (1, 2, 4, 3) ";

                if (Status == "Active")
                    Condition = " TTQ.Status in (4) ";
                if (Status == "SuccessCancel")
                    Condition = " TTQ.Status in (1, 2) ";

                string query = "SELECT CAST(COUNT(TTQ.TrnNo)AS bigint)AS TOTALTRADE,CAST(COUNT(CASE WHEN TTQ.ordertype=1 THEN 1 END)AS bigint)AS LIMIT," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 2 THEN 1 END)AS bigint)AS MARKET, CAST(COUNT(CASE WHEN TTQ.ordertype = 3 THEN 1 END)AS bigint)AS SPOT," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 4 THEN 1 END)AS bigint)AS STOP_Limit, CAST(COUNT(CASE WHEN TTQ.ordertype = 5 THEN 1 END)AS bigint)AS STOP," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 1 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS LIMIT_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 1 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS LIMIT_SELL," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 2 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS MARKET_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 2 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS MARKET_SELL," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 3 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS SPOT_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 3 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS SPOT_SELL," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 4 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS STOP_Limit_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 4 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS STOP_Limit_SELL," +
                        " CAST(COUNT(CASE WHEN TTQ.ordertype = 5 AND TTQ.TrnType = 4 THEN 1 END)AS bigint)AS STOP_BUY, CAST(COUNT(CASE WHEN TTQ.ordertype = 5 AND TTQ.TrnType = 5 THEN 1 END)AS bigint)AS STOP_SELL" +
                        " FROM TradeTransactionQueueArbitrage TTQ WHERE "+ Condition;

                if (Type.Equals("Today"))
                {
                    query = query + " And DAY(TTQ.TrnDate) =  DAY(dbo.GetISTDate()) and Month(TTQ.TrnDate) = Month(dbo.GetISTDate()) and Year(TTQ.TrnDate) = Year(dbo.GetISTDate())";
                }
                else if (Type.Equals("Week"))
                {
                    query = query + " And TrnDate >= dateadd(day, 1-datepart(dw, getdate()), CONVERT(date,getdate())) AND TrnDate<dateadd(day, 8 - datepart(dw, getdate()), CONVERT(date, getdate()))";
                }
                else if (Type.Equals("Month"))
                {
                    query = query + " And Month(TTQ.TrnDate) =  Month(dbo.GetISTDate()) And Year(TTQ.TrnDate) = Year(dbo.GetISTDate())";
                }
                else if (Type.Equals("Year"))
                {
                    query = query + " And Year(TTQ.TrnDate) =  Year(dbo.GetISTDate())";
                }

                Result = _dbContext.TradeSummaryCount.FromSql(query);

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

    }
}
