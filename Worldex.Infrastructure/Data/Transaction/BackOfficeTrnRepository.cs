using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.Configuration.FeedConfiguration;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Worldex.Core.ViewModels.Transaction.MarketMaker;

namespace Worldex.Infrastructure.Data.Transaction
{
    public class BackOfficeTrnRepository : IBackOfficeTrnRepository
    {
        private readonly WorldexContext _dbContext;
        private readonly ILogger<BackOfficeTrnRepository> _logger;

        public BackOfficeTrnRepository(WorldexContext dbContext, ILogger<BackOfficeTrnRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        #region History methods

        public List<TradeSettledHistory> TradeSettledHistory(int PageSize, int PageNo, ref long TotalPages, ref long TotalCount, ref int PageSize1, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, long TrnNo = 0)
        {
            try
            {
                List<TradeSettledHistory> list = new List<TradeSettledHistory>();
                List<TradePoolHistory> TradesH;
                IQueryable<TradeSettledHistoryQueryResponse2> Result;
                DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
                string str = "";
                string Condition = "";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    Todate = Todate + " 23:59:59";
                    tDate = DateTime.ParseExact(Todate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND TTQ.TrnDate between {0} AND {1} ";
                }
                else
                {
                    Condition += " AND TTQ.TrnDate > DATEADD(day, -7,dbo.GetISTDate())";
                }

                if (PairID != 999)
                    Condition += " AND TTQ.PairID=" + PairID;
                if (TrnType != 999)
                    Condition += " AND TTQ.TrnType=" + TrnType;
                if (OrderType != 999)
                    Condition += " AND TTQ.orderType=" + OrderType;
                if (MemberID != 0)
                    Condition += " AND TTQ.MemberID=" + MemberID;
                if (TrnNo != 0)
                    Condition += " AND TTQ.TrnNo=" + TrnNo;

                str = "select TTQ.PairID,TTQ.PairName,TTQ.TrnDate,TTQ.MemberID,OT.orderType, " +
                            "CASE WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice END AS Price1, " +
                            "CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty END AS Qty1, TTQ.TrnNo, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerTrnNo WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerTrnNo END AS Trade, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerPrice WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerPrice END AS Price, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerQty WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerQty END AS QTY, TTQ.TrnTypeName, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerType WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerType END AS TradeType " +
                            "from TradeTransactionQueue TTQ INNER JOIN TransactionQueue TQ ON TQ.Id = TTQ.TrnNo INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.ID " +
                            "INNER Join TradePoolQueueV1 TP ON(TP.MakerTrnNo= TTQ.TrnNo or TP.TakerTrnNo= TTQ.TrnNo) " +
                            "where TTQ.Status in (1, 4) " + Condition + " order by TTQ.TrnNo";

                Result = _dbContext.SettledHistory2.FromSql(str, fDate, tDate);
                var HistoryData = Result.ToList().GroupBy(e => e.TrnNo);
                var Count = 0;
                foreach (var History in HistoryData.ToList())
                {

                    TradesH = new List<TradePoolHistory>();
                    TradeSettledHistory obj = null;
                    Count += 1;
                    var cnt = 0;
                    foreach (var subHistory in History)
                    {
                        TradesH.Add(new TradePoolHistory()
                        {
                            Price = subHistory.Price,
                            Qty = subHistory.QTY,
                            TrnNo = subHistory.Trade,
                            TrnType = subHistory.TradeType,
                        });
                        if (cnt == 0)
                        {
                            obj = new TradeSettledHistory()
                            {
                                MemberID = subHistory.MemberID,
                                PairID = subHistory.PairID,
                                PairName = subHistory.PairName,
                                Price = subHistory.Price1,
                                Qty = subHistory.Qty1,
                                TrnDate = subHistory.TrnDate,
                                TrnType = subHistory.TrnTypeName,
                                TrnNo = subHistory.TrnNo,
                                OrderType = subHistory.orderType,
                                Trades = TradesH
                            };
                            cnt = 1;
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TradingSummaryViewModel> GetTradingSummaryV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market)
        {
            string Qry = "";
            string sCondition = "";
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                if (PairID != 999)
                    sCondition += " AND TTQ.PairId=" + PairID;

                if (MemberID > 0)
                    sCondition += " AND TTQ.MemberID=" + MemberID;
                if (!string.IsNullOrEmpty(TrnNo))
                    sCondition += " AND TQ.GUID like '" + TrnNo + "' ";

                if (!String.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCondition += " AND TTQ.TrnDate Between {0} And {1} And TTQ.Status>0 ";
                }
                else
                    sCondition += " AND TTQ.TrnDate > DATEADD(DAY, -10,dbo.GetISTDate())";

                if (!string.IsNullOrEmpty(SMSCode))
                    sCondition += " AND TTQ.Order_Currency='" + SMSCode + "'";

                if (trade != 999)
                    sCondition += " AND TTQ.TrnType=" + trade;
                if (Market != 999)
                    sCondition += " AND TTQ.ordertype=" + Market;

                if (status == 91) // Order History
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.Success) + " And IsCancelled=0 ";  //uday 27-12-2018 because its give partial cancel also
                else if (status == 95) //Active Order
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.Hold) + " ";
                else if (status == 92) // partial settled
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.Hold);
                else if (status == 93) // cancel
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.OperatorFail) + " ";  //uday 27-12-2018 In Spot Order IsCanceled = 0 So its also consider as systemfail
                else if (status == 94) //fail
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.SystemFail) + " ";
                else if (status == 96) // partial Cancel
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.Success) + " AND IsCancelled=1 ";
                else if (status == 97) // For User Trade count
                    sCondition += " And TTQ.Status in (1,4,2)";
                else if (status == 99) // For User Trade count
                    sCondition += " And TTQ.Status=" + Convert.ToInt16(enTransactionStatus.InActive) + " ";

                //Rita 4-3-19 remove pre-post bal ,not required
                Qry = string.Format(@"Select cast(TQ.GUID as varchar(50)) as TrnNo,OT.ordertype,TTQ.MemberID AS MemberID,TTQ.TrnTypeName as Type, TTQ.Status as StatusCode,TTQ.IsCancelled,
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price, 
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount,
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                                    TTQ.TrnDate as DateTime, TTQ.StatusMsg as StatusText, TTQ.PairID,TTQ.PairName,isnull(TQ.chargeRs,0) as ChargeRs, 
                                    Case When TTQ.TrnType = 4 Then TTQ.SettledBuyQty When TTQ.TrnType = 5 Then TTQ.SettledSellQty End As SettleQty 
                                    from TradeTransactionQueue TTQ  INNER JOIN TransactionQueue TQ ON TTQ.TrnNo = TQ.ID
                                    INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.ID
                                    LEFT JOIN BizUser MM On MM.Id = TTQ.MemberID LEFT JOIN WalletLedgers WL ON WL.Id = TTQ.MemberID  
                                    WHERE TTQ.TrnType in (4,5) And TTQ.Status In (1,2,3,4,9) and TTQ.PairName is not null {0} Order By TTQ.TrnNo Desc ", sCondition);

                return _dbContext.TradingSummaryViewModel.FromSql(Qry, fDate, tDate).ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TradingSummaryLPViewModel> GetTradingSummaryLPV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market, string LPType)
        {
            string Qry = "";
            string sCondition = "";
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                if (!string.IsNullOrEmpty(LPType))//Rita 27-3-19 if no filter then display all lists
                    sCondition += " AND SD.AppTypeID In (" + LPType + ") ";

                if (PairID != 999)
                    sCondition += " AND TTQ.PairId=" + PairID;

                if (MemberID > 0)
                    sCondition += " AND TTQ.MemberID=" + MemberID;
                if (!string.IsNullOrEmpty(TrnNo))
                    sCondition += " AND TQ.GUID like '" + TrnNo + "' ";

                if (!String.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCondition += " AND TTQ.TrnDate Between {0} And {1} And TTQ.Status>0 ";
                }
                else
                    sCondition += " AND TTQ.TrnDate > DATEADD(DAY, -10,dbo.GetISTDate())";

                if (!string.IsNullOrEmpty(SMSCode))
                    sCondition += " AND TTQ.Order_Currency='" + SMSCode + "'";

                if (trade != 999)
                    sCondition += " AND TTQ.TrnType=" + trade;
                if (Market != 999)
                    sCondition += " AND TTQ.ordertype=" + Market;

                if (status == 91) // Order History
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.Success) + " And IsCancelled=0 ";  //uday 27-12-2018 because its give partial cancel also
                else if (status == 95) //Active Order
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.Hold) + " ";
                else if (status == 92) // partial settled
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.Hold);
                else if (status == 93) // cancel
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.OperatorFail) + " ";  //uday 27-12-2018 In Spot Order IsCanceled = 0 So its also consider as systemfail
                else if (status == 94) //fail
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.SystemFail) + " ";
                else if (status == 96) // partial Cancel
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.Success) + " AND IsCancelled=1 ";
                else if (status == 97) // For User Trade count
                    sCondition += " And TTQ.Status in (1,4,2)";

                //komal 01-02-2018 solve error add ServiceProviderDetail join
                Qry = string.Format(@"Select cast(TQ.GUID as varchar(50)) as TrnNo,OT.ordertype,TTQ.MemberID AS MemberID,TTQ.TrnTypeName as Type, TTQ.Status as StatusCode,TTQ.IsCancelled, 
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price,  
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount,  
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                                    TTQ.TrnDate as DateTime, TTQ.StatusMsg as StatusText, TTQ.PairID,TTQ.PairName,isnull(TQ.chargeRs,0) as ChargeRs, 
                                    Case When TTQ.TrnType = 4 Then TTQ.SettledBuyQty When TTQ.TrnType = 5 Then TTQ.SettledSellQty End As SettleQty , Case When SD.AppTypeID = 9 Then 'Binance' When SD.AppTypeID = 10 Then 'Bittrex'
                                    When SD.AppTypeID = 11 Then 'TradeSatoshi' When SD.AppTypeID = 12 Then 'Poloniex' When SD.AppTypeID = 13 Then 'Coinbase' 
                                    End As ProviderName from TradeTransactionQueue TTQ  
                                    LEFT JOIN BizUser MM On MM.Id = TTQ.MemberID LEFT JOIN TransactionQueue TQ ON TQ.Id = TTQ.TrnNo 
                                    INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.ID
                                    INNER JOIN ServiceProviderDetail SD ON TQ.SerProDetailID=SD.Id 
                                    WHERE TTQ.TrnType in (4,5) And TTQ.Status In (1,2,3,4) {0} Order By TTQ.TrnNo Desc ", sCondition);

                return _dbContext.TradingSummaryLPViewModel.FromSql(Qry, fDate, tDate).ToList();

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TradingReconHistoryViewModel> GetTradingReconHistoryV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, long PairID, short trade, short Market, int PageSize, int PageNo, int LPType, short? IsProcessing)
        {
            string Qry = "";
            string sCondition = "";
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                if (trade != 999)
                    sCondition += " TTQ.TrnType=" + trade;
                else
                    sCondition += " TTQ.TrnType in (4, 5) ";

                if (LPType != 0)//Rita 27-3-19 if no filter then display all lists
                    sCondition += " AND SD.AppTypeID In (" + LPType + ") ";

                if (IsProcessing != 999 && IsProcessing != null) // khushali 30-03-2019 for IsProcessing filteration
                    sCondition += " AND (TS.IsProcessing = " + IsProcessing + " OR TB.IsProcessing = " + IsProcessing + " )";

                if (PairID != 999)
                    sCondition += " AND TTQ.PairId=" + PairID;

                if (MemberID > 0)
                    sCondition += " AND TTQ.MemberID=" + MemberID;
                if (!string.IsNullOrEmpty(TrnNo))
                    sCondition += " AND TQ.ID='" + TrnNo + "'";

                if (!String.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCondition += " AND TTQ.TrnDate Between {0} And {1} And TTQ.Status>0 ";
                }
                else
                    sCondition += " AND TTQ.TrnDate > DATEADD(DAY, -10,dbo.GetISTDate())";

                if (Market != 999)
                    sCondition += " AND TTQ.ordertype=" + Market;

                if (status != 0)
                    sCondition += " And TTQ.Status =" + status;
                else if (status == 0) // For All status
                    sCondition += " And TTQ.Status In (1,2,3,4,9)";

                Qry = string.Format(@"Select Isnull(MM.UserName,'') AS UserName, Isnull(TTQ.IsAPITrade,0) AS IsAPITrade ,cast(TQ.ID as varchar(50)) as TrnNo,OT.ordertype,TTQ.MemberID AS MemberID,TTQ.TrnTypeName as Type, TTQ.Status as StatusCode,TTQ.IsCancelled, 
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price,
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount, 
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                                    TTQ.TrnDate as DateTime, TTQ.StatusMsg as StatusText, TTQ.PairID,TTQ.PairName,isnull(TQ.chargeRs,0) as ChargeRs, 
                                    Case When TTQ.TrnType = 4 Then TTQ.SettledBuyQty When TTQ.TrnType = 5 Then TTQ.SettledSellQty End As SettleQty,
                                    (Select Top 1 AppTypeName From Apptype Where ID = SD.AppTypeID) As ProviderName,
                                    CASE WHEN TTQ.TrnType = 5 THEN Isnull(TS.IsProcessing,0) WHEN TTQ.TrnType = 4 THEN Isnull(TB.IsProcessing,0) ELSE 0 END as IsProcessing, 
                                    Case When TTQ.Status = 1 Then '13' When TTQ.Status = 2 Then '13' When TTQ.Status = 3 Then '13' When TTQ.Status = 4 Then '8,9,10,12' When TTQ.Status = 9 Then '11' Else '0' End As ActionStage 
                                    from TradeTransactionQueue TTQ
                                    LEFT JOIN BizUser MM On MM.Id = TTQ.MemberID LEFT JOIN TransactionQueue TQ ON TQ.Id = TTQ.TrnNo 
                                    LEFT JOIN ServiceProviderDetail SD ON TQ.SerProDetailID=SD.Id
                                    LEFT JOIN  TradeSellerListV1 TS ON TS.TrnNo = TTQ.TrnNo
                                    LEFT JOIN  TradeBuyerListV1 TB ON TB.TrnNo = TTQ.TrnNo
                                    INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.ID
                                    WHERE {0} Order By TTQ.TrnNo Desc", sCondition);
                return _dbContext.TradingReconHistoryViewModel.FromSql(Qry, fDate, tDate).ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TradeSettledHistoryV1> TradeSettledHistoryV1(long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, string TrnNo = "")
        {
            try
            {
                List<TradeSettledHistoryV1> list = new List<TradeSettledHistoryV1>();
                List<TradePoolHistoryV1> TradesH;
                IQueryable<TradeSettledHistoryQueryResponse2V1> Result;
                DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
                string str = "";
                string Condition = "";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    Todate = Todate + " 23:59:59";
                    tDate = DateTime.ParseExact(Todate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND TTQ.TrnDate between {0} AND {1} ";
                }
                else
                {
                    Condition += " AND TTQ.TrnDate > DATEADD(day, -7,dbo.GetISTDate())";
                }

                if (PairID != 999)
                    Condition += " AND TTQ.PairID=" + PairID;
                if (TrnType != 999)
                    Condition += " AND TTQ.TrnType=" + TrnType;
                if (OrderType != 999)
                    Condition += " AND TTQ.orderType=" + OrderType;
                if (MemberID != 0)
                    Condition += " AND TTQ.MemberID=" + MemberID;
                if (!string.IsNullOrEmpty(TrnNo))
                    Condition += " AND TQ.GUID like '" + TrnNo + "'";

                //komal 13 Aug 2019 change status for only settled trade
                // "where TTQ.Status in (1, 4) " + Condition + " order by TTQ.TrnNo";
                str = "select TTQ.PairID,TTQ.PairName,TTQ.TrnDate,TTQ.MemberID,OT.orderType, " +
                            "CASE WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice END AS Price1, " +
                            "CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty END AS Qty1, cast(TQ.GUID as varchar(50)) as TrnNo, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN (select cast(TQ.GUID as varchar(50)) from TransactionQueue where id=TP.TakerTrnNo) WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN (select cast(TQ.GUID as varchar(50)) from TransactionQueue where id=TP.MakerTrnNo) END AS Trade, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerPrice WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerPrice END AS Price, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerQty WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerQty END AS QTY, TTQ.TrnTypeName, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerType WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerType END AS TradeType " +
                            "from TradeTransactionQueue TTQ INNER JOIN TransactionQueue TQ ON TQ.Id = TTQ.TrnNo INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.ID " +
                            "INNER Join TradePoolQueueV1 TP ON(TP.MakerTrnNo= TTQ.TrnNo or TP.TakerTrnNo= TTQ.TrnNo) " +
                            "where (TTQ.Status =1 or (TTQ.status=4 AND TTQ.SettledSellQty>0)) " + Condition + " order by TTQ.TrnNo";


                Result = _dbContext.SettledHistory2V1.FromSql(str, fDate, tDate);
                var HistoryData = Result.ToList().GroupBy(e => e.TrnNo);
                var Count = 0;
                foreach (var History in HistoryData.ToList())
                {

                    TradesH = new List<TradePoolHistoryV1>();
                    TradeSettledHistoryV1 obj = null;
                    Count += 1;
                    var cnt = 0;
                    foreach (var subHistory in History)
                    {
                        TradesH.Add(new TradePoolHistoryV1()
                        {
                            Price = subHistory.Price,
                            Qty = subHistory.QTY,
                            TrnNo = subHistory.Trade,
                            TrnType = subHistory.TradeType,
                        });
                        if (cnt == 0)
                        {
                            obj = new TradeSettledHistoryV1()
                            {
                                MemberID = subHistory.MemberID,
                                PairID = subHistory.PairID,
                                PairName = subHistory.PairName,
                                Price = subHistory.Price1,
                                Qty = subHistory.Qty1,
                                TrnDate = subHistory.TrnDate,
                                TrnType = subHistory.TrnTypeName,
                                TrnNo = subHistory.TrnNo,
                                OrderType = subHistory.orderType,
                                Trades = TradesH
                            };
                            cnt = 1;
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Arbitrage History

        public List<TradeSettledHistory> TradeSettledHistoryArbitrageInfo(int PageSize, int PageNo, ref long TotalPages, ref long TotalCount, ref int PageSize1, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, long TrnNo = 0)
        {
            try
            {
                List<TradeSettledHistory> list = new List<TradeSettledHistory>();
                List<TradePoolHistory> TradesH;
                IQueryable<TradeSettledHistoryQueryResponse2> Result;
                DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
                string str = "";
                string Condition = "";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    Todate = Todate + " 23:59:59";
                    tDate = DateTime.ParseExact(Todate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND TTQ.TrnDate between {0} AND {1} ";
                }
                else
                {
                    Condition += " AND TTQ.TrnDate > DATEADD(day, -7,dbo.GetISTDate())";
                }

                if (PairID != 999)
                    Condition += " AND TTQ.PairID=" + PairID;
                if (TrnType != 999)
                    Condition += " AND TTQ.TrnType=" + TrnType;
                if (OrderType != 999)
                    Condition += " AND TTQ.orderType=" + OrderType;
                if (MemberID != 0)
                    Condition += " AND TTQ.MemberID=" + MemberID;
                if (TrnNo != 0)
                    Condition += " AND TTQ.TrnNo=" + TrnNo;

                str = "select TTQ.PairID,TTQ.PairName,TTQ.TrnDate,TTQ.MemberID,TTQ.orderType, " +
                            "CASE WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice END AS Price1, " +
                            "CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty END AS Qty1, TTQ.TrnNo, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerTrnNo WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerTrnNo END AS Trade, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerPrice WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerPrice END AS Price, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerQty WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerQty END AS QTY, TTQ.TrnTypeName, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerType WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerType END AS TradeType " +
                            "from TradeTransactionQueueArbitrage TTQ INNER Join TradePoolQueueArbitrageV1 TP ON(TP.MakerTrnNo= TTQ.TrnNo or TP.TakerTrnNo= TTQ.TrnNo) " +
                            "where TTQ.Status in (1, 4) " + Condition + " order by TTQ.TrnNo";

                Result = _dbContext.SettledHistory2.FromSql(str, fDate, tDate);
                var HistoryData = Result.ToList().GroupBy(e => e.TrnNo);
                var Count = 0;
                foreach (var History in HistoryData.ToList())
                {

                    TradesH = new List<TradePoolHistory>();
                    TradeSettledHistory obj = null;
                    Count += 1;
                    var cnt = 0;
                    foreach (var subHistory in History)
                    {
                        TradesH.Add(new TradePoolHistory()
                        {
                            Price = subHistory.Price,
                            Qty = subHistory.QTY,
                            TrnNo = subHistory.Trade,
                            TrnType = subHistory.TradeType,
                        });
                        if (cnt == 0)
                        {
                            obj = new TradeSettledHistory()
                            {
                                MemberID = subHistory.MemberID,
                                PairID = subHistory.PairID,
                                PairName = subHistory.PairName,
                                Price = subHistory.Price1,
                                Qty = subHistory.Qty1,
                                TrnDate = subHistory.TrnDate,
                                TrnType = subHistory.TrnTypeName,
                                TrnNo = subHistory.TrnNo,
                                OrderType = Enum.GetName(typeof(enTransactionMarketType), subHistory.orderType),
                                Trades = TradesH
                            };
                            cnt = 1;
                        }
                    }
                    list.Add(obj);
                }
                return list;
                //Uday 12-01-2019 Add Pagination
                //var items = list;
                //TotalCount = items.Count;
                //var pagesize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                //var it = Convert.ToDouble(items.Count) / pagesize;
                //var fl = Math.Ceiling(it);
                //var t1 = Convert.ToDouble(TotalCount) / pagesize;
                //TotalPages = Convert.ToInt64(Math.Ceiling(t1));
                //PageNo = PageNo + 1;
                //if (PageNo > 0)
                //{
                //    if (PageSize == 0)
                //    {
                //        int skip = Helpers.PageSize * (PageNo - 1);
                //        items = items.Skip(skip).Take(Helpers.PageSize).ToList();
                //    }
                //    else
                //    {
                //        int skip = Convert.ToInt32(PageSize) * (PageNo - 1);
                //        items = items.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                //    }
                //}

                //PageSize1 = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);

                //return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TradingReconHistoryViewModel> GetTradingReconHistoryArbitrageV1(long MemberID, string FromDate, string ToDate, String TrnNo, short status, long PairID, short trade, short Market, int LPType, short? IsProcessing)
        {
            string Qry = "";
            string sCondition = "";
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                if (trade != 999)
                    sCondition += " TTQ.TrnType=" + trade;
                else
                    sCondition += " TTQ.TrnType in (4, 5) ";

                if (LPType != 0)//Rita 27-3-19 if no filter then display all lists
                    sCondition += " AND SD.AppTypeID In (" + LPType + ") ";

                if (IsProcessing != 999 && IsProcessing != null) // khushali 30-03-2019 for IsProcessing filteration
                    sCondition += " AND (TS.IsProcessing = " + IsProcessing + " OR TB.IsProcessing = " + IsProcessing + " )";

                if (PairID != 999)
                    sCondition += " AND TTQ.PairId=" + PairID;

                if (MemberID > 0)
                    sCondition += " AND TTQ.MemberID=" + MemberID;
                if (!string.IsNullOrEmpty(TrnNo))
                    sCondition += " AND TQ.ID='" + TrnNo + "'";

                if (!String.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCondition += " AND TTQ.TrnDate Between {0} And {1} And TTQ.Status>0 ";
                }
                else
                    sCondition += " AND TTQ.TrnDate > DATEADD(DAY, -10,dbo.GetISTDate())";

                if (Market != 999)
                    sCondition += " AND TTQ.ordertype=" + Market;

                if (status == 3)
                    sCondition += " And TTQ.Status =3 and LPType=8";
                else if (status != 0)
                    sCondition += " And TTQ.Status =" + status;
                else if (status == 0) // For All status
                    sCondition += " And (TTQ.Status In (1,2,4,9) or (TTQ.Status=3 and LPType=8))";

                //khuhsali 04-04-2019 for with Fail mark - Success Mark case , success and debit
                Qry = string.Format(@"Select Isnull(MM.UserName,'') AS UserName, Isnull(TTQ.IsAPITrade,0) AS IsAPITrade ,cast(TQ.ID as varchar(50)) as TrnNo,OT.ordertype,TTQ.MemberID AS MemberID,TTQ.TrnTypeName as Type, TTQ.Status as StatusCode,TTQ.IsCancelled, 
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price,
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount, 
                                    TTQ.TrnDate as DateTime, TTQ.StatusMsg as StatusText, TTQ.PairID,TTQ.PairName,isnull(TQ.chargeRs,0) as ChargeRs, 
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                                    Case When TTQ.TrnType = 4 Then TTQ.SettledBuyQty When TTQ.TrnType = 5 Then TTQ.SettledSellQty End As SettleQty,
                                    (Select Top 1 AppTypeName From Apptype Where AppTypeID = TQ.LPType) As ProviderName,
                                    CASE WHEN TTQ.TrnType = 5 THEN Isnull(TS.IsProcessing,0) WHEN TTQ.TrnType = 4 THEN Isnull(TB.IsProcessing,0) ELSE 0 END as IsProcessing, 
                                    Case When TTQ.Status = 1 Then '13' When TTQ.Status = 2 Then '13' When TTQ.Status = 3 Then '13' When TTQ.Status = 4 Then '8,9,10,12' When TTQ.Status = 9 Then '11' Else '0' End As ActionStage 
                                    from TradeTransactionQueueArbitrage TTQ
                                    LEFT JOIN BizUser MM On MM.Id = TTQ.MemberID LEFT JOIN TransactionQueueArbitrage TQ ON TQ.Id = TTQ.TrnNo 
                                    LEFT JOIN ServiceProviderDetailArbitrage SD ON TQ.SerProDetailID = SD.ID and SD.status=1
                                    LEFT JOIN  TradeSellerListArbitrageV1 TS ON TS.TrnNo = TTQ.TrnNo
                                    LEFT JOIN  TradeBuyerListArbitrageV1 TB ON TB.TrnNo = TTQ.TrnNo INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.ID
                                    WHERE {0} Order By TTQ.TrnNo Desc", sCondition);
                return _dbContext.TradingReconHistoryViewModel.FromSql(Qry, fDate, tDate).ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TradingSummaryViewModel> GetTradingSummaryArbitrageInfoV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market)
        {
            string Qry = "";
            string sCondition = "";
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                if (PairID != 999)
                    sCondition += " AND TTQ.PairId=" + PairID;

                if (MemberID > 0)
                    sCondition += " AND TTQ.MemberID=" + MemberID;
                if (!string.IsNullOrEmpty(TrnNo))
                    sCondition += " AND TQ.GUID like '" + TrnNo + "'";

                if (!String.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCondition += " AND TTQ.TrnDate Between {0} And {1} And TTQ.Status>0 ";
                }
                else
                    sCondition += " AND TTQ.TrnDate > DATEADD(DAY, -10,dbo.GetISTDate())";

                if (!string.IsNullOrEmpty(SMSCode))
                    sCondition += " AND TTQ.Order_Currency='" + SMSCode + "'";

                if (trade != 999)
                    sCondition += " AND TTQ.TrnType=" + trade;
                if (Market != 999)
                    sCondition += " AND TTQ.ordertype=" + Market;

                if (status == 91) // Order History
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.Success) + " And IsCancelled=0 ";  //uday 27-12-2018 because its give partial cancel also
                else if (status == 95) //Active Order
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.Hold) + " ";
                else if (status == 92) // partial settled
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.Hold);
                else if (status == 93) // cancel
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.OperatorFail) + " ";  //uday 27-12-2018 In Spot Order IsCanceled = 0 So its also consider as systemfail
                else if (status == 94) //fail
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.SystemFail) + " ";
                else if (status == 96) // partial Cancel
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.Success) + " AND IsCancelled=1 ";
                else if (status == 97) // For User Trade count
                    sCondition += " And TTQ.Status in (1,4,2)";
                //Rita 4-3-19 remove pre-post bal ,not required
                Qry = string.Format(@"Select cast(TQ.GUID as varchar(50)) as TrnNo,OT.ordertype,TTQ.MemberID AS MemberID,TTQ.TrnTypeName as Type, TTQ.Status as StatusCode,TTQ.IsCancelled, 
                                CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price, 
                                CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount,  
                                TTQ.TrnDate as DateTime, TTQ.StatusMsg as StatusText, TTQ.PairID,TTQ.PairName,isnull(TQ.chargeRs,0) as ChargeRs,
                                CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                                Case When TTQ.TrnType = 4 Then TTQ.SettledBuyQty When TTQ.TrnType = 5 Then TTQ.SettledSellQty End As SettleQty 
                                from TradeTransactionQueueArbitrage TTQ  LEFT JOIN TransactionQueueArbitrage TQ ON TQ.Id = TTQ.TrnNo
                                INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.ID
                                LEFT JOIN BizUser MM On MM.Id = TTQ.MemberID LEFT JOIN ArbitrageWalletLedger WL ON WL.Id = TTQ.MemberID  
                                WHERE TTQ.TrnType in (4,5) And TTQ.Status In (1,2,3,4) {0} Order By TTQ.TrnNo Desc ", sCondition);
                return _dbContext.TradingSummaryViewModel.FromSql(Qry, fDate, tDate).ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<TradingSummaryLPViewModel> GetTradingSummaryLPArbitrageInfoV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market, string LPType)
        {
            string Qry = "";
            string sCondition = "";
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                if (!string.IsNullOrEmpty(LPType))//Rita 27-3-19 if no filter then display all lists
                    sCondition += " AND TQ.LpType In (" + LPType + ") ";

                if (PairID != 999)
                    sCondition += " AND TTQ.PairId=" + PairID;

                if (MemberID > 0)
                    sCondition += " AND TTQ.MemberID=" + MemberID;

                if (!string.IsNullOrEmpty(TrnNo))
                    sCondition += " AND TQ.GUID like '" + TrnNo + "'";

                if (!String.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCondition += " AND TTQ.TrnDate Between {0} And {1} And TTQ.Status>0 ";
                }
                else
                    sCondition += " AND TTQ.TrnDate > DATEADD(DAY, -10,dbo.GetISTDate())";

                if (!string.IsNullOrEmpty(SMSCode))
                    sCondition += " AND TTQ.Order_Currency='" + SMSCode + "'";

                if (trade != 999)
                    sCondition += " AND TTQ.TrnType=" + trade;
                if (Market != 999)
                    sCondition += " AND TTQ.ordertype=" + Market;

                if (status == 91) // Order History
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.Success) + " And IsCancelled=0 ";  //uday 27-12-2018 because its give partial cancel also
                else if (status == 95) //Active Order
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.Hold) + " ";
                else if (status == 92) // partial settled
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.Hold);
                else if (status == 93) // cancel
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.OperatorFail) + " ";  //uday 27-12-2018 In Spot Order IsCanceled = 0 So its also consider as systemfail
                else if (status == 94) //fail
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.SystemFail) + " ";
                else if (status == 96) // partial Cancel
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.Success) + " AND IsCancelled=1 ";
                else if (status == 97) // For User Trade count
                    sCondition += " And TTQ.Status in (1,4,2)";

                //komal 01-02-2018 solve error add ServiceProviderDetail join
                //Darshan Dholakiya added this query bcz of LpType changes :26-06-2019
                //Rita 05-08-2019 SM.ProviderName to APP.AppTypeName,also added inner join , as system fail order has no LP name
                Qry = string.Format(@"Select cast(TQ.GUID as varchar(50)) as TrnNo,OT.ordertype,TTQ.MemberID AS MemberID,TTQ.TrnTypeName as Type, TTQ.Status as StatusCode,TTQ.IsCancelled,
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price,  
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount,   
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total, 
                                    TTQ.TrnDate as DateTime, TTQ.StatusMsg as StatusText, TTQ.PairID,TTQ.PairName,isnull(TQ.chargeRs,0) as ChargeRs,   
                                    Case When TTQ.TrnType = 4 Then TTQ.SettledBuyQty When TTQ.TrnType = 5 Then TTQ.SettledSellQty End As SettleQty ,   
                                    APP.AppTypeName as ProviderName from TradeTransactionQueueArbitrage TTQ  
                                    INNER JOIN BizUser MM On MM.Id = TTQ.MemberID INNER JOIN TransactionQueueArbitrage TQ ON TQ.Id = TTQ.TrnNo   
                                    LEFT JOIN ServiceProviderDetailArbitrage  SD ON  SD.ID=TQ.SerProDetailID   
                                    LEFT JOIN  ServiceProviderMasterArbitrage SM ON  SM.Id=SD.ServiceProID    
                                    INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.ID
                                    INNER JOIN Apptype APP ON TQ.LPType=APP.AppTypeID
                                    WHERE TTQ.TrnType in (4,5) And TTQ.Status In (1,2,3,4) {0} Order By TTQ.TrnNo Desc", sCondition);
                return _dbContext.TradingSummaryLPViewModel.FromSql(Qry, fDate, tDate).ToList();

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<TradeSettledHistoryV1> TradeSettledHistoryArbitrageInfoV1(long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, string TrnNo = "")
        {
            try
            {
                List<TradeSettledHistoryV1> list = new List<TradeSettledHistoryV1>();
                List<TradePoolHistoryV1> TradesH;
                IQueryable<TradeSettledHistoryQueryResponse2V1> Result;
                DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
                string str = "";
                string Condition = "";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    Todate = Todate + " 23:59:59";
                    tDate = DateTime.ParseExact(Todate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND TTQ.TrnDate between {0} AND {1} ";
                }
                else
                {
                    Condition += " AND TTQ.TrnDate > DATEADD(day, -7,dbo.GetISTDate())";
                }

                if (PairID != 999)
                    Condition += " AND TTQ.PairID=" + PairID;
                if (TrnType != 999)
                    Condition += " AND TTQ.TrnType=" + TrnType;
                if (OrderType != 999)
                    Condition += " AND TTQ.orderType=" + OrderType;
                if (MemberID != 0)
                    Condition += " AND TTQ.MemberID=" + MemberID;

                if (!string.IsNullOrEmpty(TrnNo))
                    Condition += " AND TQ.GUID like '" + TrnNo + "'";

                str = string.Format(@"select TTQ.PairID,TTQ.PairName,TTQ.TrnDate,TTQ.MemberID,OT.orderType, 
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice END AS Price1, 
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty END AS Qty1, cast(TQ.GUID as varchar(50)) as TrnNo, 
                                    CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN (select cast(TQ.GUID as varchar(50)) from TransactionQueueArbitrage where id=TP.TakerTrnNo) WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN (select cast(TQ.GUID as varchar(50)) from TransactionQueueArbitrage where id=TP.MakerTrnNo) END AS Trade,
                                    CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerPrice WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerPrice END AS Price, 
                                    CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerQty WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerQty END AS QTY, TTQ.TrnTypeName, 
                                    CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerType WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerType END AS TradeType 
                                    from TradeTransactionQueueArbitrage TTQ INNER Join TradePoolQueueArbitrageV1 TP ON(TP.MakerTrnNo= TTQ.TrnNo or TP.TakerTrnNo= TTQ.TrnNo) 
                                    INNER JOIN TransactionQueueArbitrage TQ ON TQ.Id = TTQ.TrnNo INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.ID
                                    where (TTQ.Status =1 or (TTQ.status=4 AND TTQ.SettledSellQty>0)) {0} order by TTQ.TrnNo", Condition);

                Result = _dbContext.SettledHistory2V1.FromSql(str, fDate, tDate);
                var HistoryData = Result.ToList().GroupBy(e => e.TrnNo);
                var Count = 0;
                foreach (var History in HistoryData.ToList())
                {

                    TradesH = new List<TradePoolHistoryV1>();
                    TradeSettledHistoryV1 obj = null;
                    Count += 1;
                    var cnt = 0;
                    foreach (var subHistory in History)
                    {
                        TradesH.Add(new TradePoolHistoryV1()
                        {
                            Price = subHistory.Price,
                            Qty = subHistory.QTY,
                            TrnNo = subHistory.Trade,
                            TrnType = subHistory.TradeType,
                        });
                        if (cnt == 0)
                        {
                            obj = new TradeSettledHistoryV1()
                            {
                                MemberID = subHistory.MemberID,
                                PairID = subHistory.PairID,
                                PairName = subHistory.PairName,
                                Price = subHistory.Price1,
                                Qty = subHistory.Qty1,
                                TrnDate = subHistory.TrnDate,
                                TrnType = subHistory.TrnTypeName,
                                TrnNo = subHistory.TrnNo,
                                OrderType = subHistory.orderType,
                                Trades = TradesH
                            };
                            cnt = 1;
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region Margin History
        //Rita 4-2-19 for Margin Trading        
        //Rita 22-2-19 for Margin Trading Data bit
        public List<TradeSettledHistory> TradeSettledHistoryMargin(int PageSize, int PageNo, ref long TotalPages, ref long TotalCount, ref int PageSize1, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, long TrnNo = 0)
        {
            try
            {
                List<TradeSettledHistory> list = new List<TradeSettledHistory>();
                List<TradePoolHistory> TradesH;
                IQueryable<TradeSettledHistoryQueryResponse2> Result;
                DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
                string str = "";
                string Condition = "";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    Todate = Todate + " 23:59:59";
                    tDate = DateTime.ParseExact(Todate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND TTQ.TrnDate between {0} AND {1} ";
                }
                else
                {
                    Condition += " AND TTQ.TrnDate > DATEADD(day, -7,dbo.GetISTDate())";
                }

                if (PairID != 999)
                    Condition += " AND TTQ.PairID=" + PairID;
                if (TrnType != 999)
                    Condition += " AND TTQ.TrnType=" + TrnType;
                if (OrderType != 999)
                    Condition += " AND TTQ.orderType=" + OrderType;
                if (MemberID != 0)
                    Condition += " AND TTQ.MemberID=" + MemberID;
                if (TrnNo != 0)
                    Condition += " AND TTQ.TrnNo=" + TrnNo;

                str = "select TTQ.PairID,TTQ.PairName,TTQ.TrnDate,TTQ.MemberID,OT.orderType, " +
                            "CASE WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice END AS Price1, " +
                            "CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty END AS Qty1, TTQ.TrnNo, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerTrnNo WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerTrnNo END AS Trade, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerPrice WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerPrice END AS Price, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerQty WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerQty END AS QTY, TTQ.TrnTypeName, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerType WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerType END AS TradeType " +
                            "from TradeTransactionQueueMargin TTQ TTQ INNER JOIN TransactionQueueMargin TQ ON TQ.Id = TTQ.TrnNo INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.ID" +
                            "INNER Join TradePoolQueueMarginV1 TP ON(TP.MakerTrnNo= TTQ.TrnNo or TP.TakerTrnNo= TTQ.TrnNo) " +
                            "where TTQ.Status =1 or (TTQ.status=4 AND TTQ.SettledSellQty>0) " + Condition + " order by TTQ.TrnNo";

                Result = _dbContext.SettledHistory2.FromSql(str, fDate, tDate);
                var HistoryData = Result.ToList().GroupBy(e => e.TrnNo);
                var Count = 0;
                foreach (var History in HistoryData.ToList())
                {

                    TradesH = new List<TradePoolHistory>();
                    TradeSettledHistory obj = null;
                    Count += 1;
                    var cnt = 0;
                    foreach (var subHistory in History)
                    {
                        TradesH.Add(new TradePoolHistory()
                        {
                            Price = subHistory.Price,
                            Qty = subHistory.QTY,
                            TrnNo = subHistory.Trade,
                            TrnType = subHistory.TradeType,
                        });
                        if (cnt == 0)
                        {
                            obj = new TradeSettledHistory()
                            {
                                MemberID = subHistory.MemberID,
                                PairID = subHistory.PairID,
                                PairName = subHistory.PairName,
                                Price = subHistory.Price1,
                                Qty = subHistory.Qty1,
                                TrnDate = subHistory.TrnDate,
                                TrnType = subHistory.TrnTypeName,
                                TrnNo = subHistory.TrnNo,
                                OrderType = subHistory.orderType,
                                Trades = TradesH
                            };
                            cnt = 1;
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TradingSummaryViewModel> GetTradingSummaryMarginV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market)
        {
            string Qry = "";
            string sCondition = "";
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                if (PairID != 999)
                    sCondition += " AND TTQ.PairId=" + PairID;

                if (MemberID > 0)
                    sCondition += " AND TTQ.MemberID=" + MemberID;
                if (!string.IsNullOrEmpty(TrnNo))
                    sCondition += " AND TQ.GUID like '" + TrnNo + "'";

                if (!String.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    sCondition += " AND TTQ.TrnDate Between {0} And {1} And TTQ.Status>0 ";
                }
                else
                    sCondition += " AND TTQ.TrnDate > DATEADD(DAY, -10,dbo.GetISTDate())";

                if (!string.IsNullOrEmpty(SMSCode))
                    sCondition += " AND TTQ.Order_Currency='" + SMSCode + "'";

                if (trade != 999)
                    sCondition += " AND TTQ.TrnType=" + trade;
                if (Market != 999)
                    sCondition += " AND TTQ.ordertype=" + Market;

                if (status == 91) // Order History
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.Success) + " And IsCancelled=0 ";  //uday 27-12-2018 because its give partial cancel also
                else if (status == 95) //Active Order
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.Hold) + " ";
                else if (status == 92) // partial settled
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.Hold);
                else if (status == 93) // cancel
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.OperatorFail) + " ";  //uday 27-12-2018 In Spot Order IsCanceled = 0 So its also consider as systemfail
                else if (status == 94) //fail
                    sCondition += " And TTQ.Status =" + Convert.ToInt16(enTransactionStatus.SystemFail) + " ";
                else if (status == 96) // partial Cancel
                    sCondition += " And TTQ.Status = " + Convert.ToInt16(enTransactionStatus.Success) + " AND IsCancelled=1 ";
                else if (status == 97) // For User Trade count
                    sCondition += " And TTQ.Status in (1,4,2)";

                //Rita 4-3-19 remove pre-post bal ,not required
                Qry = string.Format(@"Select cast(TQ.GUID as varchar(50)) as TrnNo,OT.ordertype,TTQ.MemberID AS MemberID,TTQ.TrnTypeName as Type, TTQ.Status as StatusCode,TTQ.IsCancelled,
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice ELSE 0 END as Price, 
                                    CASE WHEN TTQ.TrnType = 5 THEN TTQ.SellQty WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty ELSE 0 END as Amount,  
                                    CASE WHEN TTQ.TrnType = 4 THEN TTQ.OrderTotalQty WHEN TTQ.TrnType = 5 THEN TTQ.DeliveryTotalQty END AS Total,
                                    TTQ.TrnDate as DateTime, TTQ.StatusMsg as StatusText, TTQ.PairID,TTQ.PairName,isnull(TQ.chargeRs,0) as ChargeRs,
                                    Case When TTQ.TrnType = 4 Then TTQ.SettledBuyQty When TTQ.TrnType = 5 Then TTQ.SettledSellQty End As SettleQty 
                                    from TradeTransactionQueueMargin TTQ  
                                    LEFT JOIN TransactionQueueMargin TQ ON TQ.Id = TTQ.TrnNo
                                    INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.ID
                                    WHERE TTQ.TrnType in (4,5) And TTQ.Status In (1,2,3,4) {0} Order By TTQ.TrnNo Desc ", sCondition);
                return _dbContext.TradingSummaryViewModel.FromSql(Qry, fDate, tDate).ToList();

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TradeSettledHistoryV1> TradeSettledHistoryMarginV1(long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, string TrnNo = "")
        {
            try
            {
                List<TradeSettledHistoryV1> list = new List<TradeSettledHistoryV1>();
                List<TradePoolHistoryV1> TradesH;
                IQueryable<TradeSettledHistoryQueryResponse2V1> Result;
                DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
                string str = "";
                string Condition = "";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    Todate = Todate + " 23:59:59";
                    tDate = DateTime.ParseExact(Todate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND TTQ.TrnDate between {0} AND {1} ";
                }
                else
                {
                    Condition += " AND TTQ.TrnDate > DATEADD(day, -7,dbo.GetISTDate())";
                }

                if (PairID != 999)
                    Condition += " AND TTQ.PairID=" + PairID;
                if (TrnType != 999)
                    Condition += " AND TTQ.TrnType=" + TrnType;
                if (OrderType != 999)
                    Condition += " AND TTQ.orderType=" + OrderType;
                if (MemberID != 0)
                    Condition += " AND TTQ.MemberID=" + MemberID;
                if (!string.IsNullOrEmpty(TrnNo))
                    Condition += " AND TQ.GUID like '" + TrnNo + "'";

                str = "select TTQ.PairID,TTQ.PairName,TTQ.TrnDate,TTQ.MemberID,OT.orderType, " +
                            "CASE WHEN TTQ.TrnType = 4 THEN TTQ.BidPrice WHEN TTQ.TrnType = 5 THEN TTQ.AskPrice END AS Price1, " +
                            "CASE WHEN TTQ.TrnType = 4 THEN TTQ.BuyQty WHEN TTQ.TrnType = 5 THEN TTQ.SellQty END AS Qty1, cast(TQ.GUID as varchar(50)) as TrnNo, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN (select cast(TQ.GUID as varchar(50)) from TransactionQueueMargin where id=TP.TakerTrnNo) WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN (select cast(TQ.GUID as varchar(50)) from TransactionQueueMargin where id=TP.MakerTrnNo) END AS Trade, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerPrice WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerPrice END AS Price, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerQty WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerQty END AS QTY, TTQ.TrnTypeName, " +
                            "CASE WHEN TP.MakerTrnNo = TTQ.TrnNo THEN TP.TakerType WHEN  TP.TakerTrnNo = TTQ.TrnNo THEN TP.MakerType END AS TradeType " +
                            "from TradeTransactionQueueMargin TTQ INNER JOIN TransactionQueueMargin TQ ON TQ.Id = TTQ.TrnNo INNER JOIN OrderTypeMaster OT ON TTQ.ordertype=OT.ID " +
                            "INNER Join TradePoolQueueMarginV1 TP ON(TP.MakerTrnNo= TTQ.TrnNo or TP.TakerTrnNo= TTQ.TrnNo) " +
                            "where (TTQ.Status =1 or (TTQ.status=4 AND TTQ.SettledSellQty>0)) " + Condition + " order by TTQ.TrnNo";

                Result = _dbContext.SettledHistory2V1.FromSql(str, fDate, tDate);
                var HistoryData = Result.ToList().GroupBy(e => e.TrnNo);
                var Count = 0;
                foreach (var History in HistoryData.ToList())
                {

                    TradesH = new List<TradePoolHistoryV1>();
                    TradeSettledHistoryV1 obj = null;
                    Count += 1;
                    var cnt = 0;
                    foreach (var subHistory in History)
                    {
                        TradesH.Add(new TradePoolHistoryV1()
                        {
                            Price = subHistory.Price,
                            Qty = subHistory.QTY,
                            TrnNo = subHistory.Trade,
                            TrnType = subHistory.TradeType,
                        });
                        if (cnt == 0)
                        {
                            obj = new TradeSettledHistoryV1()
                            {
                                MemberID = subHistory.MemberID,
                                PairID = subHistory.PairID,
                                PairName = subHistory.PairName,
                                Price = subHistory.Price1,
                                Qty = subHistory.Qty1,
                                TrnDate = subHistory.TrnDate,
                                TrnType = subHistory.TrnTypeName,
                                TrnNo = subHistory.TrnNo,
                                OrderType = subHistory.orderType,
                                Trades = TradesH
                            };
                            cnt = 1;
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region  Configuration And BackOffice Get methods

        public List<TradePairConfigRequest> GetAllPairConfiguration()
        {
            try
            {
                IQueryable<TradePairConfigRequest> Result;

                Result = _dbContext.PairConfigurationResponse.FromSql(
                                    @"Select CAST(1 AS smallint)AS QtyLength,CAST(1 AS smallint)AS PriceLength,CAST(1 AS smallint)AS AmtLength,TPM.Id,TPM.PairName,SM.Name as MarketName,TPM.SecondaryCurrencyId,TPM.WalletMasterID,TPM.BaseCurrencyId,TPM.Status,'' as StatusText,TPD.ChargeType, ISNULL(TPD.OpenOrderExpiration,0)as OpenOrderExpiration,
                                    TPD.BuyMinQty,TPD.BuyMaxQty,TPD.SellMinQty,TPD.SellMaxQty,TPD.SellPrice,TPD.BuyPrice,TPD.BuyMinPrice,TPD.BuyMaxPrice,TPD.PairPercentage,
                                    TPD.SellMinPrice,TPD.SellMaxPrice,TPD.BuyFees,TPD.SellFees,TPD.FeesCurrency,TPS.ChangeVol24 As Volume,TPS.CurrentRate As Currentrate,TPS.CurrencyPrice
                                    from TradePairMaster TPM Inner Join TradePairDetail TPD On TPD.PairId = TPM.Id  Inner Join TradePairStastics TPS On TPS.PairId = TPM.Id Inner join ServiceMaster SM on TPM.BaseCurrencyId=SM.Id Where TPM.Status = 1 or TPM.Status = 0 Order By TPM.Id");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 22-2-19 for Margin Trading Data bit
        public List<TradePairConfigRequest> GetAllPairConfigurationMargin()
        {
            try
            {
                IQueryable<TradePairConfigRequest> Result;

                Result = _dbContext.PairConfigurationResponse.FromSql(
                                    @"Select CAST(1 AS smallint)AS QtyLength,CAST(1 AS smallint)AS PriceLength,CAST(1 AS smallint)AS AmtLength,TPM.Id,TPM.PairName,SM.Name as MarketName,TPM.SecondaryCurrencyId,TPM.WalletMasterID,TPM.BaseCurrencyId,TPM.Status,'' as StatusText,TPD.ChargeType, ISNULL(TPD.OpenOrderExpiration,0)as OpenOrderExpiration,
                                    TPD.BuyMinQty,TPD.BuyMaxQty,TPD.SellMinQty,TPD.SellMaxQty,TPD.SellPrice,TPD.BuyPrice,TPD.BuyMinPrice,TPD.BuyMaxPrice,CAST(0 AS DECIMAL) AS PairPercentage,
                                    TPD.SellMinPrice,TPD.SellMaxPrice,TPD.BuyFees,TPD.SellFees,TPD.FeesCurrency,TPS.ChangeVol24 As Volume,TPS.CurrentRate As Currentrate,TPS.CurrencyPrice
                                    from TradePairMasterMargin TPM Inner Join TradePairDetailMargin TPD On TPD.PairId = TPM.Id  Inner Join TradePairStastics TPS On TPS.PairId = TPM.Id Inner join ServiceMaster SM on TPM.BaseCurrencyId=SM.Id Where TPM.Status = 1 or TPM.Status = 0 Order By TPM.Id");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ProductConfigrationGetInfo> GetAllProductConfiguration()
        {
            try
            {
                IQueryable<ProductConfigrationGetInfo> Result;

                Result = _dbContext.ProductConfigrationResponse.FromSql(
                                    @"Select PC.Id,PC.ProductName,PC.ServiceID,PC.CountryID,SM.Name As ServiceName,CM.CountryName From ProductConfiguration PC
                                      Inner Join ServiceMaster SM On SM.Id = PC.ServiceID Inner Join CountryMaster CM ON CM.Id = PC.CountryID Where PC.Status = 1 Order By PC.Id");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<AvailableRoute> GetAvailableRoute()
        {
            try
            {
                IQueryable<AvailableRoute> Result;

                Result = _dbContext.AvailableRoutes.FromSql(
                                    @"select PM.ProviderName,TPC.APIName,TPC.APISendURL,PD.Id from ServiceProviderMaster PM
                                        INNER JOIN ServiceProviderDetail PD ON PD.ServiceProID=PM.Id INNER JOIN ThirdPartyAPIConfiguration TPC ON PD.ThirPartyAPIID=TPC.Id
                                        Where PD.TrnTypeID=6 AND PD.Status=1");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<WithdrawRouteConfig> GetWithdrawRoute(long ID, enTrnType? TrnType)
        {
            try
            {
                IQueryable<IGrouping<long, RouteConfiguration>> listserviceid;
                IQueryable<GetAllWithdrawQueryResponse> Result;
                List<WithdrawRouteConfig> list = new List<WithdrawRouteConfig>();
                List<ProviderRoute> routes;
                if (TrnType != null)
                {
                    listserviceid = _dbContext.RouteConfiguration.Where(e => ((e.ServiceID == ID || ID == 0) && e.Status != 9 && e.TrnType == TrnType)).GroupBy(e => e.ServiceID);
                }
                else
                {
                    listserviceid = _dbContext.RouteConfiguration.Where(e => ((e.ServiceID == ID || ID == 0) && e.Status != 9)).GroupBy(e => e.ServiceID);
                }
                foreach (var service in listserviceid)
                {
                    Result = _dbContext.WithdrawRoute.FromSql(
                                   @"select SM.SMSCode,RC.Id,RouteName,RC.status,Priority ,ServiceID ,OpCode ,ProviderWalletID ,SerProDetailID,(select top 1 PM.ProviderName from ServiceProviderMaster PM
                                        INNER JOIN ServiceProviderDetail PD ON PD.ServiceProID=PM.Id Where PD.Status=1  AND (PD.TrnTypeID={1} OR {1}=0)) as route ,ConvertAmount,ConfirmationCount,
                                        TrnType,ISNULL(AccNoStartsWith,'')AS 'AccNoStartsWith',ISNULL(AccNoValidationRegex,'')AS 'AccNoValidationRegex',AccountNoLen
                                        from RouteConfiguration RC INNER JOIN ServiceMaster SM ON SM.Id = ServiceID where ServiceID={0} AND (TrnType={1} OR {1}=0) and RC.Status<>9 order by RC.CreatedDate ", service.Key, Convert.ToInt16(TrnType == null ? 0 : TrnType));//TrnType=6 AND PD.TrnTypeID=6 Rushabh
                    var CurName = "";
                    short st = 1;
                    int Ttype = 0;
                    routes = new List<ProviderRoute>();
                    foreach (var model in Result.ToList())
                    {
                        CurName = model.SMSCode;
                        st = model.status;
                        Ttype = Convert.ToInt32(model.TrnType);

                        routes.Add(new ProviderRoute()
                        {
                            AssetName = model.OpCode,
                            ConfirmationCount = model.ConfirmationCount,
                            ConvertAmount = model.ConvertAmount,
                            Id = model.Id,
                            Priority = model.Priority,
                            ProviderWalletID = model.ProviderWalletID,
                            ServiceProDetailId = model.SerProDetailID,
                            RouteName = model.RouteName,
                            AccNoStartsWith = model.AccNoStartsWith,
                            AccNoValidationRegex = model.AccNoValidationRegex,
                            AccountNoLen = model.AccountNoLen
                        });
                    }
                    list.Add(new WithdrawRouteConfig()
                    {
                        CurrencyName = CurName,
                        TrnType = (enTrnType)Ttype,
                        status = st,
                        ServiceID = service.Key,
                        AvailableRoute = routes
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ListPairInfo> ListPairInfo()
        {
            try
            {
                IQueryable<ListPairInfo> Result;

                Result = _dbContext.ListPairInfo.FromSql(
                                    @"select TP.Id as PairId,TP.PairName,TP.Status,(Select top 1 Name from ServiceMaster Where id=TP.BaseCurrencyId) as BaseCurrency,
                                        (Select top 1 Name from ServiceMaster Where id=TP.SecondaryCurrencyId) as ChildCurrency  from TradePairMaster TP  where TP.Status=1");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rita 12-3-19 for Margin Trading
        public List<ListPairInfo> ListPairInfoMargin()
        {
            try
            {
                IQueryable<ListPairInfo> Result;

                Result = _dbContext.ListPairInfo.FromSql(
                                    @"select TP.Id as PairId,TP.PairName,TP.Status,(Select top 1 Name from ServiceMasterMargin Where id=TP.BaseCurrencyId) as BaseCurrency,
                                        (Select top 1 Name from ServiceMasterMargin Where id=TP.SecondaryCurrencyId) as ChildCurrency  from TradePairMasterMargin TP  where TP.Status=1");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetTradeRouteConfigurationData> GetTradeRouteConfiguration(long Id)
        {
            try
            {
                IQueryable<GetTradeRouteConfigurationData> Result;

                Result = _dbContext.TradeRouteConfigurationData.FromSql(
                                    @"Select RC.Id,RC.RouteName As TradeRouteName,RC.PairId,TPM.PairName,RC.OrderType,'' As OrderTypeText,RC.TrnType,'' As TrnTypeText,
                                        RC.Status,'' As StatusText,RC.SerProDetailID As RouteUrlId,TPA.APISendURL As RouteUrl,RC.OpCode As AssetName,
                                        RC.ConvertAmount,RC.ConfirmationCount,RC.Priority from RouteConfiguration RC
                                        Inner Join TradePairMaster TPM On TPM.Id = RC.PairId
                                        Inner Join ServiceProviderDetail SPD On SPD.Id = RC.SerProDetailID
                                        Left Outer Join ThirdPartyAPIConfiguration TPA On TPA.Id = SPD.ThirPartyAPIID
                                        Where RC.ID = {0} Or {0} = 0", Id);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<AvailableRoute> GetAvailableTradeRoute(int TrnType)
        {
            try
            {
                IQueryable<AvailableRoute> Result;

                Result = _dbContext.AvailableRoutes.FromSql(
                                    @"Select PM.ProviderName,TPC.APIName,TPC.APISendURL,PD.Id from ServiceProviderMaster PM
                                      INNER JOIN ServiceProviderDetail PD ON PD.ServiceProID=PM.Id Left Outer JOIN ThirdPartyAPIConfiguration TPC ON PD.ThirPartyAPIID=TPC.Id
                                      Where PD.TrnTypeID= {0} AND PD.Status=1", TrnType);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetTradeRouteConfigurationData> GetTradeRouteForPriority(long PairId, long OrderType, int TrnType)
        {
            try
            {
                IQueryable<GetTradeRouteConfigurationData> Result;

                Result = _dbContext.TradeRouteConfigurationData.FromSql(
                                    @"Select RC.Id,RC.RouteName As TradeRouteName,RC.PairId,TPM.PairName,RC.OrderType,'' As OrderTypeText,RC.TrnType,'' As TrnTypeText,
                                        RC.Status,'' As StatusText,RC.SerProDetailID As RouteUrlId,TPA.APISendURL As RouteUrl,RC.OpCode As AssetName,
                                        RC.ConvertAmount,RC.ConfirmationCount,RC.Priority from RouteConfiguration RC
                                        Inner Join TradePairMaster TPM On TPM.Id = RC.PairId
                                        Inner Join ServiceProviderDetail SPD On SPD.Id = RC.SerProDetailID
                                        Left Outer Join ThirdPartyAPIConfiguration TPA On TPA.Id = SPD.ThirPartyAPIID
                                        Where RC.PairId = {0} And RC.OrderType = {1} And RC.TrnType = {2}", PairId, OrderType, TrnType);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<MarketTickerPairData> GetMarketTickerPairData()
        {
            try
            {
                IQueryable<MarketTickerPairData> Result;

                Result = _dbContext.MarketTickerPairData.FromSql(
                                    @"Select TPM.Id As PairId,TPM.PairName,TPD.IsMarketTicker from TradePairMaster TPM Inner Join TradePairDetail TPD ON TPD.PairId = TPM.Id Order By TPM.BaseCurrencyId,TPM.SecondaryCurrencyId");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<MarketTickerPairData> GetMarketTickerPairDataMargin()
        {
            try
            {
                IQueryable<MarketTickerPairData> Result;

                Result = _dbContext.MarketTickerPairData.FromSql(
                                    @"Select TPM.Id As PairId,TPM.PairName,TPD.IsMarketTicker from TradePairMasterMargin TPM Inner Join TradePairDetailMargin TPD ON TPD.PairId = TPM.Id Order By TPM.BaseCurrencyId,TPM.SecondaryCurrencyId");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int UpdateMarketTickerPairData(List<long> PairId, long UserId)
        {
            try
            {
                var MarketTickerIsOn = _dbContext.TradePairDetail.Where(x => PairId.Contains(x.PairId));

                var MarketTickerIsOff = _dbContext.TradePairDetail.Where(x => !PairId.Contains(x.PairId));

                _dbContext.Database.BeginTransaction();

                foreach (var pair in MarketTickerIsOn)
                {
                    pair.IsMarketTicker = 1;
                    _dbContext.Entry(pair).State = EntityState.Modified;
                }

                foreach (var pair in MarketTickerIsOff)
                {
                    pair.IsMarketTicker = 0;
                    _dbContext.Entry(pair).State = EntityState.Modified;
                }

                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();

                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //Rita 5-3-19 for Margin Trading
        public int UpdateMarketTickerPairDataMargin(List<long> PairId, long UserId)
        {
            try
            {
                var MarketTickerIsOn = _dbContext.TradePairDetailMargin.Where(x => PairId.Contains(x.PairId));

                var MarketTickerIsOff = _dbContext.TradePairDetailMargin.Where(x => !PairId.Contains(x.PairId));

                _dbContext.Database.BeginTransaction();

                foreach (var pair in MarketTickerIsOn)
                {
                    pair.IsMarketTicker = 1;
                    _dbContext.Entry(pair).State = EntityState.Modified;
                }

                foreach (var pair in MarketTickerIsOff)
                {
                    pair.IsMarketTicker = 0;
                    _dbContext.Entry(pair).State = EntityState.Modified;
                }

                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();

                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public List<VolumeDataRespose> GetUpdatedMarketTicker()
        {
            try
            {
                IQueryable<VolumeDataRespose> Result;

                Result = _dbContext.MarketTickerVolumeDataRespose.FromSql(
                                    @"Select TPM.ID As PairId,TPM.PairName As Pairname,TPS.CurrentRate As Currentrate,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume24,TPS.High24Hr AS High24Hr,TPS.Low24Hr As Low24Hr,
                                    TPS.HighWeek As HighWeek,TPS.LowWeek As LowWeek,TPS.High52Week AS High52Week,TPS.Low52Week As Low52Week,TPS.UpDownBit As UpDownBit from TradePairMaster TPM
                                    Inner Join TradePairDetail TPD ON TPD.PairId = TPM.Id
                                    Inner Join TradePairStastics TPS ON TPS.PairId = TPM.Id
                                    Where TPD.IsMarketTicker = 1");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rita 5-3-19 for Margin Trading
        public List<VolumeDataRespose> GetUpdatedMarketTickerMargin()
        {
            try
            {
                IQueryable<VolumeDataRespose> Result;

                Result = _dbContext.MarketTickerVolumeDataRespose.FromSql(
                                    @"Select TPM.ID As PairId,TPM.PairName As Pairname,TPS.CurrentRate As Currentrate,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume24,TPS.High24Hr AS High24Hr,TPS.Low24Hr As Low24Hr,
                                    TPS.HighWeek As HighWeek,TPS.LowWeek As LowWeek,TPS.High52Week AS High52Week,TPS.Low52Week As Low52Week,TPS.UpDownBit As UpDownBit from TradePairMasterMargin TPM
                                    Inner Join TradePairDetailMargin TPD ON TPD.PairId = TPM.Id
                                    Inner Join TradePairStasticsMargin TPS ON TPS.PairId = TPM.Id
                                    Where TPD.IsMarketTicker = 1");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region top gainer and looser method

        public List<TopLooserGainerPairData> GetTopGainerPair(int Type)
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

        //Rita 5-3-19 for Margin Trading
        public List<TopLooserGainerPairData> GetTopGainerPairMargin(int Type)
        {
            try
            {
                IQueryable<TopLooserGainerPairData> Result = null;

                if (Type == Convert.ToInt32(EnTopLossGainerFilterType.VolumeWise)) //Volume Wise (High to Low Volume Wise Pair Data)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                   @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                    TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMasterMargin TPM Inner Join TradePairStasticsMargin TPS ON TPS.PairId = TPM.Id
                                    Where TPM.Status = 1 Order By TPS.ChangeVol24 Desc");
                }
                else if (Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangePerWise)) //Chnage Per Wise (High to Low ChangePer Wise Pair Data, And Only > 0 ChnagePer)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                 @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                    TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMasterMargin TPM Inner Join TradePairStasticsMargin TPS ON TPS.PairId = TPM.Id
                                    Where TPS.ChangePer24 > 0 And TPM.Status = 1 Order By TPS.ChangePer24 Desc ");
                }
                else if (Type == Convert.ToInt32(EnTopLossGainerFilterType.LTPWise))  //LTP Wise (High to Low LTP Wise Pair Data)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                       @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                        TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMasterMargin TPM Inner Join TradePairStasticsMargin TPS ON TPS.PairId = TPM.Id
                                        Where TPS.LTP > 0 And TPM.Status = 1 Order By TPS.LTP Desc ");
                }
                else if (Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangeValueWise))  //Change Value Wise (High to Low ChangeValue Wise Pair Data, And Only > 0 Chnagevalue)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                       @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                        TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMasterMargin TPM Inner Join TradePairStasticsMargin TPS ON TPS.PairId = TPM.Id
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

        public List<TopLooserGainerPairData> GetTopLooserPair(int Type)
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

        //Rita 5-3-19 for Margin Trading
        public List<TopLooserGainerPairData> GetTopLooserPairMargin(int Type)
        {
            try
            {
                IQueryable<TopLooserGainerPairData> Result = null;

                if (Type == Convert.ToInt32(EnTopLossGainerFilterType.VolumeWise))  //Volume Wise (Low to High Volume Wise Pair Data) 
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                   @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                    TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMasterMargin TPM Inner Join TradePairStasticsMargin TPS ON TPS.PairId = TPM.Id
                                    Where TPM.Status = 1 Order By TPS.ChangeVol24 Asc");
                }
                else if (Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangePerWise))  //Chnage Per Wise (Low to High ChangePer Wise Pair Data, And Only < 0 ChnagePer)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                 @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                    TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMasterMargin TPM Inner Join TradePairStasticsMargin TPS ON TPS.PairId = TPM.Id
                                    Where TPS.ChangePer24 < 0 And TPM.Status = 1 Order By TPS.ChangePer24 Asc");
                }
                else if (Type == Convert.ToInt32(EnTopLossGainerFilterType.LTPWise))  // LTP Wise (Low to High LTP Wise Pair Data)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                       @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                        TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMasterMargin TPM Inner Join TradePairStasticsMargin TPS ON TPS.PairId = TPM.Id
                                        Where TPS.LTP > 0 And TPM.Status = 1 Order By TPS.LTP Asc");
                }
                else if (Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangeValueWise))  // Change Value Wise (Low to High ChangeValue Wise Pair Data, And Only < 0 Chnagevalue)
                {
                    Result = _dbContext.TopLooserPairData.FromSql(
                                       @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                        TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMasterMargin TPM Inner Join TradePairStasticsMargin TPS ON TPS.PairId = TPM.Id
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

        public List<TopLooserGainerPairData> GetTopLooserGainerPair()
        {
            try
            {
                IQueryable<TopLooserGainerPairData> Result = null;

                //Uday 01-01-2019  Pair Name Wise Filteration in Ascending Order 
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

        //Rita 5-3-19 for Margin Trading
        public List<TopLooserGainerPairData> GetTopLooserGainerPairMargin()
        {
            try
            {
                IQueryable<TopLooserGainerPairData> Result = null;

                //Uday 01-01-2019  Pair Name Wise Filteration in Ascending Order 
                Result = _dbContext.TopLooserPairData.FromSql(
                                @"Select TPM.Id As PairId,TPM.PairName,TPS.ChangePer24 As ChangePer,TPS.ChangeVol24 As Volume,
                                TPS.LTP,TPS.High24Hr As High,TPS.Low24Hr As Low,TPS.ChangeValue,TPS.UpDownBit From TradePairMasterMargin TPM Inner Join TradePairStasticsMargin TPS ON TPS.PairId = TPM.Id
                                Where TPM.Status = 1 Order By TPM.PairName");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region ReconMethod
        public bool WithdrawalRecon(TransactionRecon transactionRecon, TransactionQueue TransactionQueue, WithdrawHistory _WithdrawHistory = null, WithdrawERCTokenQueue _WithdrawERCTokenQueueObj = null, TransactionRequest TransactionRequestobj = null, short IsInsert = 2)
        {
            try
            {
                _dbContext.Database.BeginTransaction();
                _dbContext.Entry(TransactionQueue).State = EntityState.Modified;
                if (_WithdrawHistory != null)
                {
                    if (IsInsert == 1)
                    {
                        _dbContext.Entry(_WithdrawHistory).State = EntityState.Added;
                    }
                    else if (IsInsert == 0)
                    {
                        _dbContext.Entry(_WithdrawHistory).State = EntityState.Modified;
                    }
                }
                if (_WithdrawERCTokenQueueObj != null)
                {
                    _dbContext.Entry(_WithdrawERCTokenQueueObj).State = EntityState.Modified;
                }
                if (TransactionRequestobj != null)
                {
                    _dbContext.Entry(TransactionRequestobj).State = EntityState.Modified;
                }
                _dbContext.Entry(transactionRecon).State = EntityState.Added;
                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WithdrawalRecon", "BackOfficeTrnRepository", ex);
                return false;
            }
        }
        #endregion

        #region FeedConfiguration
        public List<SocketFeedConfigQueryRes> GetAllFeedConfiguration()
        {
            try
            {
                IQueryable<SocketFeedConfigQueryRes> Result = null;
                Result = _dbContext.feedConfigQueryRes.FromSql(
                               @"SELECT SC.Id,SM.id as MethodID,sl.Id as LimitID, SM.MethodName,SM.EnumCode,SL.LimitDesc,SL.LimitType,SL.MaxLimit,SL.MinLimit,SL.MaxSize,SL.MinSize,SL.MaxRecordCount,SL.RowLenghtSize,SL.MaxRowCount,SC.Status from SocketFeedConfiguration SC
                                INNER JOIN SocketMethods SM ON SC.MethodID=SM.Id  INNER JOIN SocketFeedLimits SL ON SC.FeedLimitID = SL.Id AND SL.Status=1 WHERE SC.Status in(1,0)");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        //Darshan Dholakiya added region for the arbitrage configuration changes:05-06-2019
        #region Arbitrage
        public List<TradePairConfigRequest> GetAllPairConfigurationArbitrageData()
        {
            try
            {
                IQueryable<TradePairConfigRequest> Result;
                Result = _dbContext.PairConfigurationResponse.FromSql(
                                    @"Select TPM.Id,TPM.PairName,SM.Name as MarketName,TPM.SecondaryCurrencyId,TPM.WalletMasterID,TPM.BaseCurrencyId,TPM.Status,'' as StatusText,TPD.ChargeType, ISNULL(TPD.OpenOrderExpiration,0)as OpenOrderExpiration,
                                    TPD.BuyMinQty,TPD.BuyMaxQty,TPD.SellMinQty,TPD.SellMaxQty,TPD.SellPrice,TPD.BuyPrice,TPD.BuyMinPrice,TPD.BuyMaxPrice,CAST(TPD.AmtLength AS smallint) AS 'AmtLength',CAST(TPD.PriceLength AS smallint) AS 'PriceLength',CAST(TPD.QtyLength AS smallint) AS 'QtyLength',
                                    TPD.SellMinPrice,TPD.SellMaxPrice,TPD.BuyFees,TPD.SellFees,TPD.FeesCurrency,TPS.ChangeVol24 As Volume,TPS.CurrentRate As Currentrate,TPS.CurrencyPrice
                                    from TradePairMasterArbitrage TPM Inner Join TradePairDetailArbitrage TPD On TPD.PairId = TPM.Id  Inner Join TradePairStasticsArbitrage TPS On TPS.PairId = TPM.Id Inner join ServiceMasterArbitrage SM on TPM.BaseCurrencyId=SM.Id Where TPM.Status = 1 or TPM.Status = 0 Order By TPM.Id");
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<ListPairInfo> ListPairArbitrageInfo()
        {
            try
            {
                IQueryable<ListPairInfo> Result;
                Result = _dbContext.ListPairInfo.FromSql(
                                    @"select TP.Id as PairId,TP.PairName,TP.Status,(Select top 1 Name from ServiceMasterArbitrage Where id=TP.BaseCurrencyId) as BaseCurrency,
                                        (Select top 1 Name from ServiceMasterArbitrage Where id=TP.SecondaryCurrencyId) as ChildCurrency  from TradePairMasterArbitrage TP  where TP.Status=1");
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        #endregion

        //Darshan Dholakiya added region for the arbitrage Trade changes:12-06-2019
        #region Arbitrage Trade
        public List<GetTradeRouteConfigurationData> GetTradeRouteConfigurationArbitrage(long Id)
        {
            try
            {
                IQueryable<GetTradeRouteConfigurationData> Result;

                Result = _dbContext.TradeRouteConfigurationData.FromSql(
                                    @"Select RC.Id,RC.RouteName As TradeRouteName,RC.PairId,TPM.PairName,RC.OrderType,'' As OrderTypeText,RC.TrnType,'' As TrnTypeText,
                                        RC.Status,'' As StatusText,RC.SerProDetailID As RouteUrlId,TPA.APISendURL As RouteUrl,RC.OpCode As AssetName,
                                        RC.ConvertAmount,RC.ConfirmationCount,RC.Priority from RouteConfigurationArbitrage RC
                                        Inner Join TradePairMasterArbitrage TPM On TPM.Id = RC.PairId
                                        Inner Join ServiceProviderDetailArbitrage SPD On SPD.Id = RC.SerProDetailID
                                        Left Outer Join ArbitrageThirdPartyAPIConfiguration TPA On TPA.Id = SPD.ThirPartyAPIID
                                        Where RC.ID = {0} Or {0} = 0", Id);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<GetTradeRouteConfigurationData> GetTradeRouteForPriorityArbitrage(long PairId, long OrderType, int TrnType)
        {
            try
            {
                IQueryable<GetTradeRouteConfigurationData> Result;

                Result = _dbContext.TradeRouteConfigurationData.FromSql(
                                    @"Select RC.Id,RC.RouteName As TradeRouteName,RC.PairId,TPM.PairName,RC.OrderType,'' As OrderTypeText,RC.TrnType,'' As TrnTypeText,
                                        RC.Status,'' As StatusText,RC.SerProDetailID As RouteUrlId,TPA.APISendURL As RouteUrl,RC.OpCode As AssetName,
                                        RC.ConvertAmount,RC.ConfirmationCount,RC.Priority from RouteConfigurationArbitrage RC
                                        Inner Join TradePairMasterArbitrage TPM On TPM.Id = RC.PairId
                                        Inner Join ServiceProviderDetailArbitrage SPD On SPD.Id = RC.SerProDetailID
                                        Left Outer Join ArbitrageThirdPartyAPIConfiguration TPA On TPA.Id = SPD.ThirPartyAPIID
                                        Where RC.PairId = {0} And RC.OrderType = {1} And RC.TrnType = {2}", PairId, OrderType, TrnType);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<AvailableRoute> GetAvailableTradeRouteArbitrageInfo(int TrnType)
        {
            try
            {
                IQueryable<AvailableRoute> Result;

                Result = _dbContext.AvailableRoutes.FromSql(
                                    @"Select PM.ProviderName,TPC.APIName,TPC.APISendURL,PD.Id from ServiceProviderMasterArbitrage PM
                                      INNER JOIN ServiceProviderDetailArbitrage PD ON PD.ServiceProID=PM.Id Left Outer JOIN ArbitrageThirdPartyAPIConfiguration TPC ON PD.ThirPartyAPIID=TPC.Id
                                      Where PD.TrnTypeID= {0} AND PD.Status=1", TrnType);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ProductConfigrationGetInfo> GetAllProductConfigurationArbitrageInfo()
        {
            try
            {
                IQueryable<ProductConfigrationGetInfo> Result;

                Result = _dbContext.ProductConfigrationResponse.FromSql(
                                    @"Select PC.Id,PC.ProductName,PC.ServiceID,PC.CountryID,SM.Name As ServiceName,CM.CountryName From ProductConfigurationArbitrage PC
                                      Inner Join ServiceMasterArbitrage SM On SM.Id = PC.ServiceID Inner Join CountryMaster CM ON CM.Id = PC.CountryID Where PC.Status = 1 Order By PC.Id");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<MarketTickerPairData> GetMarketTickerPairDataArbitrageInfo()
        {
            try
            {
                IQueryable<MarketTickerPairData> Result;

                Result = _dbContext.MarketTickerPairData.FromSql(
                                    @"Select TPM.Id As PairId,TPM.PairName,TPD.IsMarketTicker from TradePairMasterArbitrage TPM Inner Join TradePairDetailArbitrage TPD ON TPD.PairId = TPM.Id Order By TPM.BaseCurrencyId,TPM.SecondaryCurrencyId");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public int UpdateMarketTickerPairDataArbitrageInfo(List<long> PairId, long UserId)
        {
            try
            {
                var MarketTickerIsOn = _dbContext.TradePairDetailArbitrage.Where(x => PairId.Contains(x.PairId));

                var MarketTickerIsOff = _dbContext.TradePairDetailArbitrage.Where(x => !PairId.Contains(x.PairId));

                _dbContext.Database.BeginTransaction();

                foreach (var pair in MarketTickerIsOn)
                {
                    pair.IsMarketTicker = 1;
                    _dbContext.Entry(pair).State = EntityState.Modified;
                }

                foreach (var pair in MarketTickerIsOff)
                {
                    pair.IsMarketTicker = 0;
                    _dbContext.Entry(pair).State = EntityState.Modified;
                }

                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();

                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }

        }
        #endregion

        #region Not in use method

        public List<TrnChargeSummaryViewModel> ChargeSummary(string FromDate, string ToDate, short trade)
        {
            string Qry = "";
            string sCondition = " 1=1 ";
            DateTime fDate, tDate;
            try
            {
                IQueryable<TrnChargeSummaryViewModel> Result;

                if (!String.IsNullOrEmpty(FromDate))
                    sCondition += " AND TTQ.TrnDate Between {0} And {1} And TQ.Status>0 ";

                if (trade != 999)
                    sCondition += " AND TTQ.TrnType=" + trade;
                Qry = "select TTQ.TrnNo,TTQ.TrnTypeName,TTQ.TrnDate,TTQ.PairName,TQ.Amount,TQ.ChargePer,TQ.ChargeRs, " +
                                        " CASE WHEN TQ.ChargeType = 1 THEN 'Percentage' WHEN TQ.ChargeType = 1 THEN 'Fixed' END as ChargeType " +
                                        " from TransactionQueue TQ Inner Join TradeTransactionQueue TTQ on TTQ.TrnNo = TQ.Id where " + sCondition + " AND TQ.Status = 1";

                if (!String.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                    Result = _dbContext.chargeSummaryViewModels.FromSql(Qry, fDate, tDate);
                }
                else
                    Result = _dbContext.chargeSummaryViewModels.FromSql(Qry);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<WithdrawalSummaryViewModel> GetWithdrawalSummary(WithdrawalSummaryRequest Request)
        {
            try
            {
                IQueryable<WithdrawalSummaryViewModel> Result;
                DateTime fDate, tDate;
                string Qry = "";

                fDate = DateTime.ParseExact(Request.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                Request.ToDate = Request.ToDate + " 23:59:59";
                tDate = DateTime.ParseExact(Request.ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                Qry = "Select TQ.Id as TrnNo,TQ.MemberID,TQ.Amount,TQ.TrnDate,TQ.TransactionAccount As DestAddress,ISNULL(TQ.DebitAccountId,'') As DebitAccountId," +
                      " CASE TQ.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 2 THEN 'OperatorFail' WHEN 3 THEN" +
                      "'SystemFail'  WHEn 4 THEN 'Hold' WHEN 5 THEN 'Refunded' WHEN 6 THEN 'Pending' ELSE 'Other' END AS 'StatusText'," +
                      " TQ.SMSCode As ServiceName,ISNULL(TQ.ChargeRs,0) As ChargeRs From TransactionQueue TQ" +
                      " Where TQ.TrnType = 6 And TQ.TrnDate Between {0} And {1} And (TQ.MemberID = {2} Or {2} = 0) And (TQ.Id = {3} Or {3} = 0) " +
                      " And (TQ.Status = {4} Or {4} = 0) And (TQ.SMSCode = {5} or {5} = '')";

                Result = _dbContext.WithdrawalSummaryViewModel.FromSql(Qry, fDate, tDate, Request.MemberID, Request.TrnNo, Request.Status, Request.SMSCode);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<PairTradeSummaryQryResponse> PairTradeSummary(long PairID, short Market, short Range)
        {
            try
            {
                using (_dbContext)
                {
                    String Condi = " ";
                    String str = "";

                    if (Range == 1)//day
                        Condi += " AND TTQ.TrnDate > DATEADD(HOUR, -24,dbo.GetISTDate()) ";
                    if (Range == 2)//week
                        Condi += " AND TTQ.TrnDate > DATEADD(DAY, -7,dbo.GetISTDate()) ";
                    if (Range == 3)//month
                        Condi += " AND TTQ.TrnDate > DATEADD(MONTH, -1,dbo.GetISTDate()) ";
                    if (Range == 4)//3month
                        Condi += " AND TTQ.TrnDate > DATEADD(MONTH, -3,dbo.GetISTDate()) ";
                    if (Range == 5)//6month
                        Condi += " AND TTQ.TrnDate > DATEADD(MONTH, -6,dbo.GetISTDate()) ";
                    if (Range == 6)//1year
                        Condi += " AND TTQ.TrnDate > DATEADD(YEAR, -1,dbo.GetISTDate()) ";
                    if (Market != 999)
                        Condi += " AND TTQ.ordertype=" + Market;

                    IQueryable<PairTradeSummaryQryResponse> Result = null;

                    str = "select TPM.Id,TPM.PairName,TTQ.ordertype,count(TTQ.TrnNo) as TradeCount, " +
                        "count(CASE WHEN TTQ.TrnType = 4 then 1 end) as buy,count(CASE WHEN TTQ.TrnType = 5 then 1 end) as sell, " +
                        "count(CASE WHEN TTQ.status = 1 then 1 end) as Settled,count(CASE WHEN TTQ.status = 2 then 1 end) as Cancelled, " +
                        "max(case when TTQ.status = 1 then(CASE WHEN TTQ.TrnType = 4 then BidPrice WHEN TTQ.TrnType = 5 then AskPrice end) else 0 end) as high," +
                        "ISNULL(min(case when TTQ.status = 1 then(CASE WHEN TTQ.TrnType = 4 then BidPrice WHEN TTQ.TrnType = 5 then AskPrice end) end), 0) as low," +
                        "ISNULL(sum(SettledBuyQty * BidPrice) + sum(SettledSellQty * askprice), 0) as Volume," +
                        "ISNULL((Select Top 1 Case When TTQ.TrnType = 4 Then TTQ.BidPrice When TTQ.TrnType = 5 Then TTQ.AskPrice END From TradeTransactionQueue TTQ " +
                        "Where TTQ.PairID = TPM.Id And TTQ.Status = 1 " + Condi + " Order By TTQ.TrnNo Desc),0) As LTP, " +
                        "ISNULL((Select Top 1 Case When TTQ.TrnType = 4 Then TTQ.BidPrice When TTQ.TrnType = 5 Then TTQ.AskPrice END From TradeTransactionQueue TTQ " +
                        "Where TTQ.PairID = TPM.Id And TTQ.Status = 1 " + Condi + " Order By TTQ.TrnNo),0) As OpenP " +
                        "from TradePairMaster TPM INNER Join TradeTransactionQueue TTQ On TTQ.PairID = TPM.Id " + Condi + " Group By TPM.Id,TPM.PairName,TTQ.ordertype order by TPM.PairName";
                    if (PairID != 999)
                    {
                        str = "select TPM.Id,TPM.PairName,TTQ.ordertype,count(TTQ.TrnNo) as TradeCount, " +
                       "count(CASE WHEN TTQ.TrnType = 4 then 1 end) as buy,count(CASE WHEN TTQ.TrnType = 5 then 1 end) as sell, " +
                       "count(CASE WHEN TTQ.status = 1 then 1 end) as Settled,count(CASE WHEN TTQ.status = 2 then 1 end) as Cancelled, " +
                       "max(case when TTQ.status = 1 then(CASE WHEN TTQ.TrnType = 4 then BidPrice WHEN TTQ.TrnType = 5 then AskPrice end) else 0 end) as high," +
                       "ISNULL(min(case when TTQ.status = 1 then(CASE WHEN TTQ.TrnType = 4 then BidPrice WHEN TTQ.TrnType = 5 then AskPrice end) end), 0) as low," +
                       "ISNULL(sum(SettledBuyQty * BidPrice) + sum(SettledSellQty * askprice), 0) as Volume," +
                       "ISNULL((Select Top 1 Case When TTQ.TrnType = 4 Then TTQ.BidPrice When TTQ.TrnType = 5 Then TTQ.AskPrice END From TradeTransactionQueue TTQ " +
                       "Where TTQ.PairID = TPM.Id And TTQ.Status = 1 " + Condi + " Order By TTQ.TrnNo Desc),0) As LTP, " +
                       "ISNULL((Select Top 1 Case When TTQ.TrnType = 4 Then TTQ.BidPrice When TTQ.TrnType = 5 Then TTQ.AskPrice END From TradeTransactionQueue TTQ " +
                       "Where TTQ.PairID = TPM.Id And TTQ.Status = 1 " + Condi + " Order By TTQ.TrnNo),0) As OpenP " +
                       "from TradePairMaster TPM INNER Join TradeTransactionQueue TTQ On TTQ.PairID = TPM.Id Where TPM.Id={0}" + Condi + " Group By TPM.Id,TPM.PairName,TTQ.ordertype order by TPM.PairName";
                    }
                    Result = _dbContext.PairTradeSummaryViewModel.FromSql(str, PairID);

                    return Result.ToList();
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rita 5-3-19 for Margin Trading Data bit
        public List<PairTradeSummaryQryResponse> PairTradeSummaryMargin(long PairID, short Market, short Range)
        {
            try
            {
                using (_dbContext)
                {
                    String Condi = " ";
                    String str = "";

                    if (Range == 1)//day
                        Condi += " AND TTQ.TrnDate > DATEADD(HOUR, -24,dbo.GetISTDate()) ";
                    if (Range == 2)//week
                        Condi += " AND TTQ.TrnDate > DATEADD(DAY, -7,dbo.GetISTDate()) ";
                    if (Range == 3)//month
                        Condi += " AND TTQ.TrnDate > DATEADD(MONTH, -1,dbo.GetISTDate()) ";
                    if (Range == 4)//3month
                        Condi += " AND TTQ.TrnDate > DATEADD(MONTH, -3,dbo.GetISTDate()) ";
                    if (Range == 5)//6month
                        Condi += " AND TTQ.TrnDate > DATEADD(MONTH, -6,dbo.GetISTDate()) ";
                    if (Range == 6)//1year
                        Condi += " AND TTQ.TrnDate > DATEADD(YEAR, -1,dbo.GetISTDate()) ";
                    if (Market != 999)
                        Condi += " AND TTQ.ordertype=" + Market;

                    IQueryable<PairTradeSummaryQryResponse> Result = null;

                    str = "select TPM.Id,TPM.PairName,TTQ.ordertype,count(TTQ.TrnNo) as TradeCount, " +
                        "count(CASE WHEN TTQ.TrnType = 4 then 1 end) as buy,count(CASE WHEN TTQ.TrnType = 5 then 1 end) as sell, " +
                        "count(CASE WHEN TTQ.status = 1 then 1 end) as Settled,count(CASE WHEN TTQ.status = 2 then 1 end) as Cancelled, " +
                        "max(case when TTQ.status = 1 then(CASE WHEN TTQ.TrnType = 4 then BidPrice WHEN TTQ.TrnType = 5 then AskPrice end) else 0 end) as high," +
                        "ISNULL(min(case when TTQ.status = 1 then(CASE WHEN TTQ.TrnType = 4 then BidPrice WHEN TTQ.TrnType = 5 then AskPrice end) end), 0) as low," +
                        "ISNULL(sum(SettledBuyQty * BidPrice) + sum(SettledSellQty * askprice), 0) as Volume," +
                        "ISNULL((Select Top 1 Case When TTQ.TrnType = 4 Then TTQ.BidPrice When TTQ.TrnType = 5 Then TTQ.AskPrice END From TradeTransactionQueueMargin TTQ " +
                        "Where TTQ.PairID = TPM.Id And TTQ.Status = 1 " + Condi + " Order By TTQ.TrnNo Desc),0) As LTP, " +
                        "ISNULL((Select Top 1 Case When TTQ.TrnType = 4 Then TTQ.BidPrice When TTQ.TrnType = 5 Then TTQ.AskPrice END From TradeTransactionQueueMargin TTQ " +
                        "Where TTQ.PairID = TPM.Id And TTQ.Status = 1 " + Condi + " Order By TTQ.TrnNo),0) As OpenP " +
                        "from TradePairMasterMargin TPM INNER Join TradeTransactionQueueMargin TTQ On TTQ.PairID = TPM.Id " + Condi + " Group By TPM.Id,TPM.PairName,TTQ.ordertype order by TPM.PairName";
                    if (PairID != 999)
                    {
                        str = "select TPM.Id,TPM.PairName,TTQ.ordertype,count(TTQ.TrnNo) as TradeCount, " +
                       "count(CASE WHEN TTQ.TrnType = 4 then 1 end) as buy,count(CASE WHEN TTQ.TrnType = 5 then 1 end) as sell, " +
                       "count(CASE WHEN TTQ.status = 1 then 1 end) as Settled,count(CASE WHEN TTQ.status = 2 then 1 end) as Cancelled, " +
                       "max(case when TTQ.status = 1 then(CASE WHEN TTQ.TrnType = 4 then BidPrice WHEN TTQ.TrnType = 5 then AskPrice end) else 0 end) as high," +
                       "ISNULL(min(case when TTQ.status = 1 then(CASE WHEN TTQ.TrnType = 4 then BidPrice WHEN TTQ.TrnType = 5 then AskPrice end) end), 0) as low," +
                       "ISNULL(sum(SettledBuyQty * BidPrice) + sum(SettledSellQty * askprice), 0) as Volume," +
                       "ISNULL((Select Top 1 Case When TTQ.TrnType = 4 Then TTQ.BidPrice When TTQ.TrnType = 5 Then TTQ.AskPrice END From TradeTransactionQueueMargin TTQ " +
                       "Where TTQ.PairID = TPM.Id And TTQ.Status = 1 " + Condi + " Order By TTQ.TrnNo Desc),0) As LTP, " +
                       "ISNULL((Select Top 1 Case When TTQ.TrnType = 4 Then TTQ.BidPrice When TTQ.TrnType = 5 Then TTQ.AskPrice END From TradeTransactionQueueMargin TTQ " +
                       "Where TTQ.PairID = TPM.Id And TTQ.Status = 1 " + Condi + " Order By TTQ.TrnNo),0) As OpenP " +
                       "from TradePairMasterMargin TPM INNER Join TradeTransactionQueueMargin TTQ On TTQ.PairID = TPM.Id Where TPM.Id={0}" + Condi + " Group By TPM.Id,TPM.PairName,TTQ.ordertype order by TPM.PairName";
                    }
                    Result = _dbContext.PairTradeSummaryViewModel.FromSql(str, PairID);

                    return Result.ToList();
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Market Maker Performance

        public long GetMarketMakerUser()
        {
            try
            {
                var Result = _dbContext.GetMarketMakerUser.FromSql(@"SELECT Top(1) cast(u.UserID as bigint)as UserID FROM BizUserRole u INNER JOIN BizRoles r ON (u.RoleId = r.Id) WHERE r.NormalizedName = 'MARKETMAKER'  AND r.Status = 1").FirstOrDefault();
                if (Result != null)
                {
                    return Result.UserID;
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<MarketMakerBalancePerformanceViewModel> GetMarketMakerBalancePerformance(long Userid)
        {
            try
            {
                return _dbContext.MarketMakerBalancePerformances.FromSql(@"select WTM.WalletTypeName,Isnull((select top 1 sum(CrAmt)-sum(DrAmt) from WalletLedgers where walletid=WM.id and trntype = 0),0) as OldBalance,
                            Balance+OutBoundBalance as NewBalance from WalletMasters WM INNER JOIN WalletTypeMasters WTM ON WM.WalletTypeID=WTM.id
                            where UserID={0}", Userid).ToList();

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<MarketMakerTradePerformance> MarketMakerTradePerformance(long Userid, long PairID, string FromDate, string ToDate)
        {
            List<MarketMakerTradePerformance> Result = new List<MarketMakerTradePerformance>();
            string sCondition = " ";
            DateTime fDate = DateTime.Now, tDate = DateTime.Now;
            try
            {
                if (PairID != 0)
                    sCondition += " AND PairID=" + PairID;

                if (!String.IsNullOrEmpty(FromDate))
                {
                    sCondition += " AND TrnDate Between {1} And {2} ";
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }

                List<MarketMakerTradePairList> makerTradePairList = _dbContext.MarketMakerTradePairList.FromSql("select PairID,PairName from TradeTransactionQueue where Memberid={0} and status=1 " + sCondition + " Group by PairID,PAirName", Userid, fDate, tDate).ToList();
                List<MarketMakerTradeCountListQryRes> BuyTradeCount = _dbContext.MarketMakerTradeCountListQryRes.FromSql("select cast(count(Trnno) as bigint) as Count, avg(BidPrice) as AvgPrice,PairID from TradeTransactionQueue where Trntype=4 and Memberid={0} and status=1 " + sCondition + " Group by PairID", Userid, fDate, tDate).ToList();
                List<MarketMakerTradeCountListQryRes> SellTradeCount = _dbContext.MarketMakerTradeCountListQryRes.FromSql("select cast(count(Trnno) as bigint) as Count, avg(AskPrice) as AvgPrice,PairID from TradeTransactionQueue where Trntype=5 and Memberid={0} and status=1 " + sCondition + " Group by PairID", Userid, fDate, tDate).ToList();

                if (makerTradePairList.Count > 0)
                {
                    MarketMakerBuySellCount TradeCountObj = null;
                    foreach (var Pair in makerTradePairList)
                    {
                        TradeCountObj = new MarketMakerBuySellCount() { BuyAvgPrice = 0, BuyCount = 0, SellAvgPrice = 0, SellCount = 0 };
                        var BuyObj = BuyTradeCount.Where(e => e.PairID == Pair.PairID).FirstOrDefault();
                        var SellObj = SellTradeCount.Where(e => e.PairID == Pair.PairID).FirstOrDefault();
                        if (BuyObj != null)
                        {
                            TradeCountObj.BuyCount = BuyObj.Count;
                            TradeCountObj.BuyAvgPrice = BuyObj.AvgPrice;
                        }
                        if (SellObj != null)
                        {
                            TradeCountObj.SellCount = SellObj.Count;
                            TradeCountObj.SellAvgPrice = SellObj.AvgPrice;
                        }
                        Result.Add(new MarketMakerTradePerformance()
                        {
                            PairName = Pair.PairName,
                            TradeCount = TradeCountObj
                        });
                    }
                }
                return Result;
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

