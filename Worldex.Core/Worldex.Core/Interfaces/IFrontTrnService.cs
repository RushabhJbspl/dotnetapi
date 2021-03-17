using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.Arbitrage;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces
{
    public interface IFrontTrnService
    {
        CopiedLeaderOrdersResponse GetCopiedLeaderOrders(long MemberID, string FromDate = "", string TodDate = "", long PairId = 999, short trnType = 999, string FollowTradeType = "", long FollowingTo = 0, int PageSize = 0, int PageNo = 0);
        List<BasePairResponse> GetTradePairAsset();
        List<VolumeDataRespose> GetVolumeData(long BasePairId, short IsMargin = 0);//Rita 22-2-19 for Margin Trading Data bit
        List<GetOrderHistoryInfo> GetOrderHistory(long PairId, short IsMargin = 0);
        List<GetTradeHistoryInfoV1> GetTradeHistoryV1(long MemberID, string sCondition, string FromDate, string TodDate, int page, int IsAll, short IsMargin = 0);
        List<RecentOrderInfoV1> GetRecentOrderV1(long PairId, long MemberID, short IsMargin = 0);
        List<ActiveOrderInfoV1> GetActiveOrderV1(long MemberID, string FromDate, string TodDate, long PairId, int Page, short trnType, short IsMargin = 0);
        List<GetOrderHistoryInfoArbitrageV1> GetOrderHistoryArbitrageV1(long PairId);
        List<GetTradeHistoryInfoArbitrageV1> GetTradeHistoryArbitrageV1(long MemberID, string sCondition, string FromDate, string TodDate, int page, int IsAll);
        List<RecentOrderInfoArbitrageV1> GetRecentOrderArbitrageV1(long PairId, long MemberID);
        List<ActiveOrderInfoArbitrageV1> GetActiveOrderArbitrageV1(long MemberID, string FromDate, string TodDate, long PairId, int Page, short trnType);
        GetBuySellBookResponse GetBuyerSellerBookV1(long id, short TrnType, short IsMargin = 0);
        List<GetBuySellBook> GetBuyerSellerBookArbitrageV1(long id, short TrnType, short IsMargin = 0);
        CopiedLeaderOrdersResponseV1 GetCopiedLeaderOrdersV1(long MemberID, string FromDate = "", string TodDate = "", long PairId = 999, short trnType = 999, string FollowTradeType = "", long FollowingTo = 0, int PageSize = 0, int PageNo = 0);

        long GetPairIdByName(string pair, short IsMargin = 0);//Rita 22-2-19 for Margin Trading Data bit
        bool IsValidPairName(string Pair);
        bool IsValidDateFormate(string date);
        Int16 IsValidTradeType(string Type);
        Int16 IsValidMarketType(string type);
        Int16 IsValidStatus(string status);
        BizResponseClass ValidatePairCommonMethod(string PairName, ref long PairId, short IsMargin);
        BizResponseClass ValidateFromDateToDateCommonMethod(string FromDate, string ToDate);
        BizResponseClass ValidatePairCommonMethodArbitrage(string PairName, ref long PairId, short IsMargin);

        long GetBasePairIdByName(string BasePair, short IsMargin = 0);//Rita 22-2-19 for Margin Trading Data bit
        GetTradePairByName GetTradePairByName(long id);
        List<GetGraphDetailInfo> GetGraphDetail(long PairId, int IntervalTime, string IntervalData, short IsMargin = 0);//Rita 23-2-19 for Margin Trading Data bit
        MarketCapData GetMarketCap(long PairId);
        VolumeDataRespose GetVolumeDataByPair(long PairId);
        bool addSetteledTradeTransaction(SettledTradeTransactionQueue queueData);
        PairRatesResponse GetPairRates(long PairId, short IsMargin = 0);//Rita 23-2-19 for Margin Trading Data bit
        int AddToFavouritePair(long PairId, long UserId);
        int RemoveFromFavouritePair(long PairId, long UserId);
        List<FavouritePairInfo> GetFavouritePair(long UserId, short IsMargin = 0);//Rita 23-2-19 for Margin Trading Data bit
        Task GetPairAdditionalVal(long PairId, decimal CurrentRate, long TrnNo, decimal Quantity, DateTime TranDate, string UserID = "");
        void GetIntervalTimeValue(string Interval, ref int IntervalTime, ref string IntervalData);
        List<VolumeDataRespose> GetMarketTicker(short IsMargin = 0);//Rita 23-2-19 for Margin Trading Data bit
        int GetMarketTickerSignalR(short IsMargin = 0);//Rita 23-2-19 for Margin Trading Data bit
        List<TopLooserGainerPairData> GetFrontTopGainerPair(int Type);
        List<TopLooserGainerPairData> GetFrontTopLooserPair(int Type);
        List<TopLooserGainerPairData> GetFrontTopLooserGainerPair();
        GetBuySellMarketBook GetMarketDepthChart(long PairId);
        List<HistoricalPerformanceYear> GetHistoricalPerformance(long UserId);
        GetWithdrawalTransactionData GetWithdrawalTransaction(string RefId);
        TopLeadersListResponse TopLeadersList();
        TradeWatchListResponse getTradeWatchList(long userId);
        TopProfitGainerLoserResponse GetTopProfitGainer(DateTime date, int size);
        TopProfitGainerLoserResponse TopProfitLoser(DateTime date, int size);
        List<HistoricalPerformanceYear> GetHistoricalPerformanceV1(long UserId);
        SiteTokenConvertFundResponse GetSiteTokenConversionData(long? UserID, string SourceCurrency = "", string TargerCurrency = "", string FromDate = "", string ToDate = "", short IsMargin = 0);
        //Rita 20-2-19 for margin trading
        Task GetPairAdditionalValMargin(long PairId, decimal CurrentRate, long TrnNo, decimal Quantity, DateTime TranDate, string UserID = "");
        List<BasePairResponse> GetTradePairAssetMargin();
        GetTradePairByName GetTradePairByNameMargin(long id);
        MarketCapData GetMarketCapMargin(long PairId);
        VolumeDataRespose GetVolumeDataByPairMargin(long PairId);
        GetBuySellMarketBook GetMarketDepthChartMargin(long PairId);
        int AddToFavouritePairMargin(long PairId, long UserId);
        int RemoveFromFavouritePairMargin(long PairId, long UserId);

        //khushali 27-05-2019 for trading configuration
        TradingConfigurationList TradingConfiguration();
        BizResponseClass ChangeTradingConfigurationStatus(long ConfigID, short Status, long UserID);

        //Rita 31-5-19 for arbitrage traging methods
        long GetPairIdByNameArbitrage(string pair, short IsMargin = 0);
        List<GetGraphDetailInfo> GetGraphDetailArbitrage(long PairId, int IntervalTime, string IntervalData, short IsMargin = 0);//Rita 23-2-19 for Margin Trading Data bit
        Task GetPairAdditionalValArbitrage(long PairId, decimal CurrentRate, long TrnNo, decimal Quantity, DateTime TranDate, string UserID = "");
        ProfitIndicatorInfo GetProfitIndicatorArbitrage(long PairId, short IsMargin = 0);
        List<BasePairResponse> GetTradePairAssetArbitrage();//Darshan Dholakiya added method this for the Arbitrage Trading changes:07-06-2019
        List<ExchangeProviderListArbitrage> ExchangeProviderListArbitrage(long PairId, short IsMargin = 0);
        List<ArbitrageBuySellViewModel> GetExchangeProviderBuySellBookArbitrage(long PairId, short TrnType);
        List<ExchangeListSmartArbitrage> ExchangeListSmartArbitrageService(long PairId,string PairName, short ProviderCount, short IsMargin = 0);
        List<SmartArbitrageHistoryInfo> SmartArbitrageHistoryList(long PairId,long MemberID, string FromDate, string TodDate, short IsMargin = 0);
        AllowArbitrageTradingViewModel GetAllowedUserArbitrageTrading(long UserID);
        BizResponseClass SetAllowedUserArbitrageTrading(enArbitrageTradingAllowToUser TradingType, long UserID);
        //============================================

        //komal 18-07-2019 test method
        Task<string> CCXTBalanceCheckAsync(string exchangeName, string id, string APIKey, string SecretKey);

        int GetMarketMakerUserRole();
    }
}
