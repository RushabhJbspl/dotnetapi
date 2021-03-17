using Worldex.Core.Entities.Transaction;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.Configuration.FeedConfiguration;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using System;
using System.Collections.Generic;
using Worldex.Core.ViewModels.Transaction.MarketMaker;

namespace Worldex.Core.Interfaces.Repository
{
    public interface IBackOfficeTrnRepository
    {
        List<TradeSettledHistory> TradeSettledHistory(int PageSize, int PageNo, ref long TotalPages, ref long TotalCount, ref int PageSize1, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, long TrnNo = 0);
        //Rita 4-2-19 for Margin Trading
        List<TradeSettledHistory> TradeSettledHistoryMargin(int PageSize, int PageNo, ref long TotalPages, ref long TotalCount, ref int PageSize1, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, long TrnNo = 0);

        List<TradePairConfigRequest> GetAllPairConfiguration();
        List<ProductConfigrationGetInfo> GetAllProductConfiguration();
        List<TrnChargeSummaryViewModel> ChargeSummary(string FromDate, string ToDate, short trade);
        bool WithdrawalRecon(TransactionRecon transactionRecon, TransactionQueue TransactionQueue, WithdrawHistory _WithdrawHistory = null, WithdrawERCTokenQueue _WithdrawERCTokenQueueObj = null, TransactionRequest TransactionRequestobj= null,short IsInsert=2);
        List<WithdrawalSummaryViewModel> GetWithdrawalSummary(WithdrawalSummaryRequest Request);
        List<PairTradeSummaryQryResponse> PairTradeSummary(long PairID, short Market, short Range);
        List<AvailableRoute> GetAvailableRoute();
        List<ListPairInfo> ListPairInfo();
        List<GetTradeRouteConfigurationData> GetTradeRouteConfiguration(long Id);
        List<WithdrawRouteConfig> GetWithdrawRoute(long ID,enTrnType? TrnType);
        List<AvailableRoute> GetAvailableTradeRoute(int TrnType);
        List<GetTradeRouteConfigurationData> GetTradeRouteForPriority(long PairId, long OrderType, int TrnType);
        
        List<MarketTickerPairData> GetMarketTickerPairData();
        int UpdateMarketTickerPairData(List<long> PairId, long UserId);
        List<VolumeDataRespose> GetUpdatedMarketTicker();
        List<TopLooserGainerPairData> GetTopGainerPair(int Type);
        List<TopLooserGainerPairData> GetTopLooserPair(int Type);
        List<TopLooserGainerPairData> GetTopLooserGainerPair();
        //khuhsali 23-01-2019 trade summery for LP wise data
        List<SocketFeedConfigQueryRes> GetAllFeedConfiguration();

        List<VolumeDataRespose> GetUpdatedMarketTickerMargin();
        List<PairTradeSummaryQryResponse> PairTradeSummaryMargin(long PairID, short Market, short Range);
        List<TopLooserGainerPairData> GetTopGainerPairMargin(int Type);
        List<TopLooserGainerPairData> GetTopLooserPairMargin(int Type);
        List<TopLooserGainerPairData> GetTopLooserGainerPairMargin();
        List<MarketTickerPairData> GetMarketTickerPairDataMargin();
        int UpdateMarketTickerPairDataMargin(List<long> PairId, long UserId);
        List<TradePairConfigRequest> GetAllPairConfigurationMargin();
        List<ListPairInfo> ListPairInfoMargin();
        List<TradePairConfigRequest> GetAllPairConfigurationArbitrageData(); //Darshan Dholakiya added method for the arbitrage configuration changes:06-06-2019
        List<ListPairInfo> ListPairArbitrageInfo(); //Darshan Dholakiya added method for the arbitrage configuration changes:06-06-2019
        
        //Darshan Dholakiya added method for the Trading Settlement History changes:08-06-2019
        List<TradeSettledHistory> TradeSettledHistoryArbitrageInfo(int PageSize, int PageNo, ref long TotalPages, ref long TotalCount, ref int PageSize1, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, long TrnNo = 0);
        List<GetTradeRouteConfigurationData> GetTradeRouteConfigurationArbitrage(long Id);
        List<GetTradeRouteConfigurationData> GetTradeRouteForPriorityArbitrage(long PairId, long OrderType, int TrnType);

        List<AvailableRoute> GetAvailableTradeRouteArbitrageInfo(int TrnType);//Darshan Dholakiya added this method for Trade arbitrage changes:12-06-2019
        List<ProductConfigrationGetInfo> GetAllProductConfigurationArbitrageInfo(); //Darshan Dholakiya added this method for arbitrage Product changes:18-06-2019
        List<MarketTickerPairData> GetMarketTickerPairDataArbitrageInfo();//Darshan Dholakiya added this method for arbitrage Market changes:22-06-2019
        int UpdateMarketTickerPairDataArbitrageInfo(List<long> PairId, long UserId);
        
        //Darshan Dholakiya added this method for arbitrage Market changes:22-06-2019
        //komal 13-07-2019 optimized method
        List<TradingSummaryViewModel> GetTradingSummaryV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market);
        List<TradingSummaryLPViewModel> GetTradingSummaryLPV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market, string LPType);
        List<TradingReconHistoryViewModel> GetTradingReconHistoryV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, long PairID, short trade, short Market, int PageSize, int PageNo, int LPType, short? IsProcessing);
        List<TradingSummaryViewModel> GetTradingSummaryMarginV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market);
        
        List<TradeSettledHistoryV1> TradeSettledHistoryV1(long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, string TrnNo = "");
        List<TradeSettledHistoryV1> TradeSettledHistoryMarginV1(long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, string TrnNo = "");

        List<TradingReconHistoryViewModel> GetTradingReconHistoryArbitrageV1(long MemberID, string FromDate, string ToDate, String TrnNo, short status, long PairID, short trade, short Market, int LPType, short? IsProcessing);
        List<TradingSummaryViewModel> GetTradingSummaryArbitrageInfoV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market);
        List<TradingSummaryLPViewModel> GetTradingSummaryLPArbitrageInfoV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market, string LPType);
        List<TradeSettledHistoryV1> TradeSettledHistoryArbitrageInfoV1(long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, string TrnNo = "");
        //komal 13-11-2019 12:03 PM Market Maker Performance 
        long GetMarketMakerUser();
        List<MarketMakerBalancePerformanceViewModel> GetMarketMakerBalancePerformance(long Userid);
         List<MarketMakerTradePerformance> MarketMakerTradePerformance(long Userid, long PairID, string FromDate, string ToDate);

    }
}
