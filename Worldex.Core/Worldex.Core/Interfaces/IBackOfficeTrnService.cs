using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using System.Collections.Generic;
using System.Threading.Tasks;
using Worldex.Core.ViewModels.Transaction.MarketMaker;

namespace Worldex.Core.Interfaces
{
    public interface IBackOfficeTrnService
    {
        Task<BizResponseClass> TradeRecon(long TranNo, string ActionMessage, long UserId, string accessToken);
        TransactionChargeResponse ChargeSummary(string FromDate, string ToDate, short trade);
        BizResponseClass WithdrawalRecon(WithdrawalReconRequest request, long UserId, string accessToken);
        WithdrawalSummaryResponse GetWithdrawalSummary(WithdrawalSummaryRequest Request);

        PairTradeSummaryResponse pairTradeSummary(long PairID, short Market, short Range,short IsMargin=0);//Rita 5-3-19 for Margin Trading
        TradeSettledHistoryResponse TradeSettledHistory(int PageSize, int PageNo,long PairID=999,short TrnType=999,short OrderType=999,string FromDate="",string Todate="",long MemberID=0, long  TrnNo=0,short IsMargin=0); //Rita 22-2-19 for Margin Trading Data bit
        List<TopLooserGainerPairData> GetTopGainerPair(int Type, short IsMargin = 0);//Rita 5-3-19 for Margin Trading
        List<TopLooserGainerPairData> GetTopLooserPair(int Type, short IsMargin = 0);//Rita 5-3-19 for Margin Trading
        List<TopLooserGainerPairData> GetTopLooserGainerPair(short IsMargin = 0);//Rita 5-3-19 for Margin Trading
        Task<BizResponseClass> TradeReconV1(enTradeReconActionType ActionType, long TranNo, string ActionMessage, long UserId, string accessToken);
        TradeSettledHistoryResponse TradeSettledHistoryArbitrage(int PageSize, int PageNo, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, long TrnNo = 0, short IsMargin = 0);
        Task<BizResponseClass> ArbitrageTradeReconV1(enTradeReconActionType ActionType, long TranNo, string ActionMessage, long UserId, string accessToken);

        //khushali 23-01-2019 for LP wise Trade history
        //Darshan Dholakiya added this method for Arbitrage summary Lp changes:06-06-2019      
        //komal 13-07-2019 optimized method
        TradingSummaryResponse GetTradingSummaryV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market, int PageSize, int PageNo, short IsMargin = 0);
        TradingSummaryLPResponse GetTradingSummaryLPV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market, int PageSize, int PageNo, string LPType);
        TradingReconHistoryResponse GetTradingReconHistoryV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, long PairID, short trade, short Market, int PageSize, int PageNo, int LPType, short? IsProcessing);
        TradeSettledHistoryResponseV1 TradeSettledHistoryV1(int PageSize, int PageNo, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, string TrnNo = "", short IsMargin = 0);
        TradingReconHistoryResponse GetTradingReconHistoryArbitrageV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, long PairID, short trade, short Market, int PageSize, int PageNo, int LPType, short? IsProcessing);
        TradingSummaryResponse GetTradingSummaryArbitrageV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market, int PageSize, int PageNo);
        TradingSummaryLPResponse GetTradingSummaryLPArbitrageV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market, int PageSize, int PageNo, string LPType);
        TradeSettledHistoryResponseV1 TradeSettledHistoryArbitrageV1(int PageSize, int PageNo, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, string TrnNo = "", short IsMargin = 0);

       
        MarketMakerBalancePerformanceResponse GetMarketMakerBalancePerformance();
        MarketMakerTradePerformanceResponse MarketMakerTradePerformance(long PairID, string FromDate, string ToDate);
    }
}
