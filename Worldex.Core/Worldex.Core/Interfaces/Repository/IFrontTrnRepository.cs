using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.Arbitrage;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Worldex.Core.ViewModels.Transaction.MarketMaker;

namespace Worldex.Core.Interfaces.Repository
{
    public interface IFrontTrnRepository
    {

        //Rita 31-5-19 for arbitrage traging methods
        //Darshan Dholakiya added this methods for arbitrage changes:07-06-2019
        //Komal 15-07-20189 new Optimizes Method
        List<GetOrderHistoryInfo> GetOrderHistory(long PairId);
        List<GetOrderHistoryInfo> GetOrderHistoryMargin(long PairId);
        List<GetOrderHistoryInfoArbitrageV1> GetOrderHistoryArbitrage(long PairId);
        List<RecentOrderInfoV1> GetRecentOrderV1(long PairId, long MemberID);
        List<RecentOrderInfoV1> GetRecentOrderMarginV1(long PairId, long MemberID);
        List<RecentOrderInfoArbitrageV1> GetRecentOrderArbitrageV1(long PairId, long MemberID);
        List<GetTradeHistoryInfoV1> GetTradeHistoryV1(long MemberID, string sCondition, string FromDate, string ToDate, int page, int IsAll, long TrnNo = 0);
        List<GetTradeHistoryInfoV1> GetTradeHistoryMarginV1(long MemberID, string sCondition, string FromDate, string ToDate, int page, int IsAll, long TrnNo = 0);
        List<GetTradeHistoryInfoArbitrageV1> GetTradeHistoryArbitrageV1(long MemberID, string sCondition, string FromDate, string ToDate, int page, int IsAll, long TrnNo = 0);
        List<ActiveOrderInfoV1> GetActiveOrderV1(long MemberID, string FromDate, string ToDate, long PairId, short trnType);
        List<ActiveOrderInfoV1> GetActiveOrderMarginV1(long MemberID, string FromDate, string TodDate, long PairId, short trnType);
        List<ActiveOrderInfoArbitrageV1> GetActiveOrderArbitrageV1(long MemberID, string FromDate, string ToDate, long PairId, short trnType);
        List<CopiedLeaderOrdersInfoV1> GetCopiedLeaderOrdersV1(long MemberID, string FromDate = null, string ToDate = null, long PairId = 999, short trnType = 999, string FollowTradeType = "", long FollowingTo = 0);


        List<CopiedLeaderOrdersQryRes> GetCopiedLeaderOrders(long MemberID, string FromDate = null, string ToDate = null, long PairId = 999, short trnType = 999, string FollowTradeType = "", long FollowingTo = 0);
        long GetPairIdByName(string pair);
        List<GetBuySellBook> GetBuyerBook(long id, decimal Price = -1);
        List<GetBuySellBook> GetSellerBook(long id, decimal Price = -1);
        List<GetGraphDetailInfo> GetGraphData(long id, int IntervalTime, string IntervalData, DateTime Minute, int socket = 0);
        decimal LastPriceByPair(long PairId, ref short UpDownBit);
        PairRatesResponse GetPairRates(long PairId);
        List<TradePairTableResponse> GetTradePairAsset(long BaseId = 0);
        List<ServiceMasterResponse> GetAllServiceConfiguration(int StatusData = 0, long CurrencyTypeId = 999);
        List<GetGraphResponsePairWise> GetGraphDataEveryLastMin(string Interval);
        HighLowViewModel GetHighLowValue(long PairId, int Day);
        List<FavouritePairInfo> GetFavouritePairs(long UserId);
        List<PairStatisticsCalculation> GetPairStatisticsCalculation();
        void UpdatePairStatisticsCalculation(List<TradePairStastics> PairDataUpdated);
        GetTradeSettlePrice GetTradeSettlementPrice(long TrnNo);
        //OpenOrderInfo CheckOpenOrderRange(long TrnNo);
        List<TopLooserGainerPairData> GetFrontTopGainerPair(int Type);
        List<TopLooserGainerPairData> GetFrontTopLooserPair(int Type);
        List<TopLooserGainerPairData> GetFrontTopLooserGainerPair();
        decimal GetHistoricalPerformanceData(long UserId, int Type);
        List<StopLimitBuySellBook> GetStopLimitBuySellBooks(decimal LTP, long Pair, enOrderType OrderType, short IsCancel = 0);
        List<LPStatusCheckData> LPstatusCheck(); // khushali 23-01-2019  for LP status check 
        List<LPStatusCheckDataArbitrage> LPstatusCheckArbitrage(short ActionStage); // khushali 23-01-2019  for LP status check
        List<StopLossArbitargeResponse> StopLossArbitargeCron(); // khushali 27-07-2019  for stop loss cron
        List<TopLeaderListInfo> TopLeaderList(int IsAll = 0);
        List<TradeWatchLists> getTradeWatchList(List<TradeWatchLists> TradeWatcher);
        List<SiteTokenConversionQueryRes> GetSiteTokenConversionData(long? UserId, string SourceCurrency = "", string TargerCurrency = "", string FromDate = "", string ToDate = "", short IsMargin = 0);


        //============================================
        //Rita 20-2-19 for Margin Trading
        List<StopLimitBuySellBook> GetStopLimitBuySellBooksMargin(decimal LTP, long Pair, enOrderType OrderType, short IsCancel = 0);
        List<GetBuySellBook> GetSellerBookMargin(long id, decimal Price = -1);
        List<GetBuySellBook> GetBuyerBookMargin(long id, decimal Price = -1);
        HighLowViewModel GetHighLowValueMargin(long PairId, int Day);
        List<GetGraphDetailInfo> GetGraphDataMargin(long id, int IntervalTime, string IntervalData, DateTime Minute, int socket = 0);
        List<TradePairTableResponse> GetTradePairAssetMargin(long BaseId = 0);
        PairRatesResponse GetPairRatesMargin(long PairId);
        List<FavouritePairInfo> GetFavouritePairsMargin(long UserId);
        List<GetGraphResponsePairWise> GetGraphDataEveryLastMinMargin(string Interval);
        List<ServiceMasterResponse> GetAllServiceConfigurationMargin(int StatusData = 0);
        GetTradeSettlePrice GetTradeSettlementPriceMargin(long TrnNo);

        //============================================
        //khuhsali 03-04-2019 for ReleaseAndStuckOrder cron 
        List<ReleaseAndStuckOrdercls> ReleaseAndStuckOrder(DateTime Date);
        //khuhsali 14-05-2019 for Liquidity configuration
        List<ConfigureLP> GetLiquidityConfigurationData(short LPType);
        //khuhsali 15-05-2019 for Marging trading ReleaseAndStuckOrder cron 
        List<ReleaseAndStuckOrdercls> MarginReleaseAndStuckOrder(DateTime Date);

        //khushali for Liquidity Provider configuration
        bool UpdateLTPData(LTPcls LTPData);
        bool InsertLTPData(LTPcls LTPData);
        List<CryptoWatcher> GetPairWiseLTPData(GetLTPDataLPwise LTPData);
        bool GetLocalConfigurationData(short LPType);
        LPKeyVault BalanceCheckLP(long SerproID);
        LPKeyVault BalanceCheckLPArbitrage(long SerproID);
        GetTradeSettlePrice GetTradeSettlementPriceArbitrage(long TrnNo);

        //============================================
        List<GetBuySellBook> GetBuyerBookArbitrage(long id, decimal Price = -1);
        List<GetBuySellBook> GetSellerBookArbitrage(long id, decimal Price = -1);
        List<GetGraphDetailInfo> GetGraphDataArbitrage(long id, int IntervalTime, string IntervalData, DateTime Minute, int socket = 0);
        List<GetGraphResponsePairWise> GetGraphDataEveryLastMinArbitrage(string Interval);
        List<StopLimitBuySellBook> GetStopLimitBuySellBooksArbitrage(decimal LTP, long Pair, enOrderType OrderType, short IsCancel = 0);
        HighLowViewModel GetHighLowValueArbitrage(long PairId, int Day);
        List<ExchangeProviderListArbitrage> GetExchangeProviderListArbitrage(long PairId);
        List<ExchangeProviderListArbitrage> GetExchangeProviderListArbitrageCache(long PairId);
        List<SmartArbitrageHistoryInfo> SmartArbitrageHistoryList(long PairId,long MemberID,string FromDat, string ToDate);
        List<ConfigureLPArbitrage> GetLiquidityConfigurationDataArbitrage(short LPType);
        Task<ArbitrageCryptoWatcherQryRes> UpdateLTPDataArbitrage(ArbitrageLTPCls LTPData);
        ArbitrageCryptoWatcherQryRes InsertLTPDataArbitrage(ArbitrageLTPCls LTPData);
        bool GetLocalConfigurationDataArbitrage(short LPType);
        List<ArbitrageBuySellViewModel> GetExchangeProviderBuySellBookArbitrage(long PairId, short TrnType);
        LPKeyVault GetTradeFeesLPArbitrage(long LPType);
        //Darshan Dholakiya added this methods for arbitrage changes:07-06-2019
        List<TradePairTableResponse> GetTradePairAssetArbitrageInfo(long BaseId = 0);
        //khushali 10-06-2019 Route configuration wise exchange info
        ExchangeProviderListArbitrage GetExchangeProviderListArbitrageRouteWise(long RouteID);
        //Darshan Dholakiya added this methods for Service Config changes:11-06-2019
        List<ServiceMasterResponse> GetAllServiceConfigurationArbitrage(int StatusData = 0);
        LocalPairStatisticsQryRes GetLocalPairStatistics(long Pair);

        void UpdateTradeTransactionQueueAPIStatus(long TrnNo, string APIStatus);
        CheckArbitrageTransactionStatus CheckArbitrageTransactionStatus(string TrnNo, short SmaartTradePriority);

        /// <summary>
        /// Method retrieve information about MarketMaker role from BizRole table
        /// </summary>
        /// <returns>return 0 if role is not present else return role fetched from database</returns>
        /// <remarks>-Sahil 16-10-2019 03:47 PM</remarks>
        int GetMarketMakerUserRole();

        /// <summary>
        /// method retrieve market maker sell preferences
        /// </summary>
        /// <param name="userId"> market maket user id</param>
        /// <param name="pairId"> currency pair id</param>
        /// <returns>buy preferences viewmodel if set else null</returns>
        /// <remarks>-Sahil 12-10-2019 12:39 PM</remarks>
        MarketMakerBuyPreferencesViewModel GetMarketMakerUserBuyPreferences(long pairId);

        /// <summary>
        /// method retrieve market maker sell preferences
        /// </summary>
        /// <param name="userId"> market maket user id</param>
        /// <param name="pairId"> currency pair id</param>
        /// <returns>buy preferences viewmodel if set else null</returns>
        /// <remarks>-Sahil 12-10-2019 12:39 PM</remarks>
        MarketMakerSellPreferencesViewModel GetMarketMakerUserSellPreferences(long pairId);

        /// <summary>
        /// method get HoldOrderRateChange from market maker preference
        /// </summary>
        /// <param name="pairId"> currency pair id</param>
        /// <returns>percentage of rate change</returns>
        string GetMarketMakerHoldOrderRateChange(long pairId);
        List<MarketMakerSettleTrxnByTakeViewModel> GetMarketMakerSettledByTakerList(long TakerTrnno, long MarketMakerID);
    }
}
