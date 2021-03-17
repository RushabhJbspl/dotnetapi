using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.LiquidityProvider;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.Configuration
{
    public interface ITransactionConfigService
    {
        Task<ServiceConfigurationResponse> AddServiceConfiguration(ServiceConfigurationRequestNew Request, long UserId);
        long UpdateServiceConfiguration(ServiceConfigurationRequest Request, long UserId);
        ServiceConfigurationRequest GetServiceConfiguration(long ServiceId);
        List<ServiceConfigurationRequest> GetAllServiceConfiguration(int StatusData = 0, short IsMargin = 0, long CurrencyTypeId = 999);//Rita 5-3-19 for Margin Trading)
        List<ServiceCurrencyData> GetAllServiceConfigurationByBase(String Base);
        int SetActiveService(long ServiceId);
        int SetInActiveService(long ServiceId);
        GetServiceByBaseReasponse GetCurrency(short ActiveOnly = 0);

        IEnumerable<ServiceProviderViewModel> GetAllProvider();
        ServiceProviderViewModel GetPoviderByID(long ID);
        long AddProviderService(ServiceProviderRequest request, long UserId);
        bool UpdateProviderService(ServiceProviderRequest request, long UserId);
        bool SetActiveProvider(long id);
        bool SetInActiveProvider(long id);

        IEnumerable<AppTypeViewModel> GetAppType();
        AppTypeViewModel GetAppTypeById(long id);
        long AddAppType(AppTypeRequest request, long UserId);
        bool UpdateAppType(AppTypeRequest request, long UserId);
        bool SetActiveAppType(long id);
        bool SetInActiveAppType(long id);

        IEnumerable<ProviderTypeViewModel> GetProviderType();
        ProviderTypeViewModel GetProviderTypeById(long id);
        long AddProviderType(ProviderTypeRequest request, long UserId);
        bool UpdateProviderType(ProviderTypeRequest request, long UserId);
        bool SetActiveProviderType(long id);
        bool SetInActiveProviderType(long id);

        ProviderConfigurationViewModel GetProviderConfiguration(long id);
        long AddProviderConfiguration(ProviderConfigurationRequest request, long UserId);
        bool SetActiveProviderConfiguration(long id);
        bool SetInActiveProviderConfiguration(long id);
        bool UpdateProviderConfiguration(ProviderConfigurationRequest request, long UserId);
        AllProConfigResponse GetAllProviderConfiguration();
        ListProConfigResponse ListProviderConfiguration();

        DemonconfigurationViewModel GetDemonConfiguration(long id);
        long AddDemonConfiguration(DemonConfigurationRequest request, long UserId);
        bool UpdateDemonConfiguration(DemonConfigurationRequest request, long UserId);
        bool SetActiveDemonConfig(long id);
        bool SetInActiveDemonConfig(long id);
        ListDemonConfigResponse GetAllDemonConfigV1();
        ListDemonConfigResponseV1 ListDemonConfigV1();

        IEnumerable<ProviderDetailViewModel> GetProviderDetailList();
        IEnumerable<ProviderDetailGetAllResponse2> getProviderDetailsDataList(IEnumerable<ProviderDetailViewModel> dataList);
        ProviderDetailGetAllResponse2 getProviderDetailDataById(ProviderDetailViewModel viewModel);
        ProviderDetailViewModel GetProviderDetailById(long id);
        long AddProviderDetail(ProviderDetailRequest request, long UserId);
        BizResponseClass UpdateProviderDetail(ProviderDetailRequest request, long UserId);
        bool SetActiveProviderDetail(long id);
        bool SetInActiveProviderDetail(long id);

        long AddProductConfiguration(ProductConfigurationRequest Request, long UserId);
        long UpdateProductConfiguration(ProductConfigurationRequest Request, long UserId);
        ProductConfigrationGetInfo GetProductConfiguration(long ProductId);
        List<ProductConfigrationGetInfo> GetAllProductConfiguration();
        int SetActiveProduct(long ProductId);
        int SetInActiveProduct(long ProductId);

        long UpdateWithdrawRouteConfig(WithdrawRouteConfigRequest Request, long UserId);
        WithdrawConfigResponse2 GetRouteConfiguration(long RouteId, enTrnType TrnType);
        WithdrawConfigResponse GetAllRouteConfiguration(enTrnType TrnType);
        int SetActiveRoute(long RouteId);
        int SetInActiveRoute(long RouteId);
        BizResponseClass AddWithdrawRouteConfig(WithdrawRouteConfigRequest Request, long UserId);
        AvailableRouteResponse GetAvailableRoute();

        List<ThirdPartyAPIConfigViewModel> GetAllThirdPartyAPIConfig();
        ThirdPartyAPIConfigViewModel GetThirdPartyAPIConfigById(long Id);
        long AddThirdPartyAPI(ThirdPartyAPIConfigRequest request, long UserId);
        bool UpdateThirdPartyAPI(ThirdPartyAPIConfigRequest request, long UserId);
        bool SetActiveThirdPartyAPI(long id);
        bool SetInActiveThirdPartyAPI(long id);

        List<ThirdPartyAPIConfigurationResponseViewModel> GetAllThirdPartyAPIResponse(); //change by jagdish 12-02-2020
        ThirdPartyAPIConfigurationResponseViewModel GetThirdPartyAPIResponseById(long id); //change by jagdish 12-02-2020
        long AddThirdPartyAPIResponse(ThirdPartyAPIConfigurationResponseViewModel Request, long UserId); //old ThirdPartyAPIResponseConfigViewModel change by jagdish 12-02-2020
        bool UpdateThirdPartyAPIResponse(ThirdPartyAPIConfigurationResponseViewModel Request, long UserId); //change by jagdish 12-02-2020
        bool SetActiveThirdPartyAPIResponse(long id);
        bool SetInActiveThirdPartyAPIResponse(long id);

        long AddPairConfiguration(TradePairConfigRequest Request, long UserId);

        long UpdatePairConfiguration(TradePairConfigRequest Request, long UserId);
        TradePairConfigRequest GetPairConfiguration(long PairId);
        List<TradePairConfigRequest> GetAllPairConfiguration(short IsMargin = 0);//Rita 12-3-19 for Margin Trading
        int SetActivePair(long PairId);
        int SetInActivePair(long PairId);
        ViewModels.Configuration.ListPairResponse ListPair(short IsMargin = 0);//Rita 12-3-19 for Margin Trading

        List<ServiceTypeMasterInfo> GetAllServiceTypeMaster();
        List<TransactionTypeInfo> GetAllTransactionType();
        OrderTypeResponse GetOrderType();

        List<LimitViewModel> GetAllLimitData();
        LimitViewModel GetLimitById(long id);
        long AddLimitData(LimitRequest Request, long UserId);
        bool UpdateLimitData(LimitRequest Request, long UserId);
        bool SetActiveLimit(long id);
        bool SetInActiveLimit(long id);

        MarketViewModel AddMarketData(MarketViewModel viewModel, long UserId);
        List<MarketViewModel> GetAllMarketData(short ActiveOnly = 1);
        MarketViewModel GetMarketDataByMarket(long Id);
        BizResponseClass AddMarketDataV2(MarketDataRequest Request, long UserId);
        MarketDataResponse UpdateMarketDataV2(MarketDataRequest Request, long UserId);

        int AddLiquidityAPIManager(LiquidityAPIManagerRequest Request, long UserId);
        List<LiquidityAPIManagerData> GetAllLiquidityAPIManager();
        LiquidityAPIManagerData GetLiquidityAPIManager(long Id);
        int UpdateLiquidityAPIManager(LiquidityAPIManagerUpdateRequest Request, long UserId);

        long AddTradeRouteConfiguration(TradeRouteConfigRequest Request, long UserId);
        long UpdateTradeRouteConfiguration(TradeRouteConfigRequest Request, long UserId);
        List<GetTradeRouteConfigurationData> GetAllTradeRouteConfiguration();
        GetTradeRouteConfigurationData GetTradeRouteConfiguration(long Id);
        List<AvailableRoute> GetAvailableTradeRoute(int TrnType);
        List<GetTradeRouteConfigurationData> GetTradeRouteForPriority(long PairId, long OrderType, int TrnType);
        long UpdateTradeRoutePriority(TradeRoutePriorityUpdateRequest Request, long UserId);

        List<MarketTickerPairData> GetMarketTickerPairData(short IsMargin = 0);//Rita 5-3-19 for Margin Trading
        int UpdateMarketTickerPairData(UpdateMarketTickerPairData Request, long UserId, short IsMargin = 0);//Rita 5-3-19 for Margin Trading

        BizResponseClass AddCoinRequest(CoinListRequestRequest Request, long UserID);
        CoinListRequestResponse GetUserCoinRequest(long UserId);
        CoinListRequestResponse GetAllCoinRequest(GetCoinRequestListRequest Request);
        BizResponseClass SetCoinRequestStatus(SetCoinRequestStatusRequest Request, long UserID);

        SiteTokenTypeResponse GetSiteTokenType();
        BizResponseClass AddSiteToken(SiteTokenMasterRequest request, long UserID);
        BizResponseClass UpdateSiteToken(SiteTokenMasterRequest request, long UserID);
        SiteTokenMasterResponse GetAllSiteToken(short ActiveOnly = 0);

        string[] LpPairListConvertor(string[] LpSymbol, short LP);
        string[] LpPairListConvertorV1(string[] LpSymbol, short LP); // khushali 14-05-2019
        ConfigureLP[] LpPairListConvertorWithLocalPair(short LP); // khushali 15-05-2019 //string[] LpSymbol,
        ConfigureLP[] TradePairConfigurationV1(); // khushali 15-05-2019
        //Rita 28-2-19 for Margin trading
        Task<ServiceConfigurationResponse> AddServiceConfigurationMargin(ServiceConfigurationRequestNew Request, long UserId);
        long UpdateServiceConfigurationMargin(ServiceConfigurationRequest Request, long UserId);
        ServiceConfigurationRequest GetServiceConfigurationMargin(long ServiceId);
        List<MarketViewModel> GetAllMarketDataMargin(short ActiveOnly = 1);
        List<ServiceCurrencyData> GetAllServiceConfigurationByBaseMargin(String Base);
        int SetActiveServiceMargin(long ServiceId);
        int SetInActiveServiceMargin(long ServiceId);
        GetServiceByBaseReasponse GetCurrencyMargin(short ActiveOnly = 0);
        BizResponseClass AddMarketDataV2Margin(MarketDataRequest Request, long UserId);
        MarketDataResponse UpdateMarketDataV2Margin(MarketDataRequest Request, long UserId);
        long AddPairConfigurationMargin(TradePairConfigRequest Request, long UserId);
        long UpdatePairConfigurationMargin(TradePairConfigRequest Request, long UserId);
        TradePairConfigRequest GetPairConfigurationMargin(long PairId);
        int SetActivePairMargin(long PairId);
        int SetInActivePairMargin(long PairId);
        BizResponseClass AddSiteTokenMargin(SiteTokenMasterRequest request, long UserID);
        BizResponseClass UpdateSiteTokenMargin(SiteTokenMasterRequest request, long UserID);
        SiteTokenMasterResponse GetAllSiteTokenMargin(short ActiveOnly = 0);

        #region Arbitrage Liquidity API Manager

        //khushali 04-06-2019 for Arbitrage Exchange configuration
        GetAllLiquidityAPIManagerArbitrage GetAllLiquidityAPIManagerArbitrage(int Page, int? PageSize);
        LiquidityAPIManagerDataArbitrage GetLiquidityAPIManagerArbitrage(long Id);
        int UpdateLiquidityAPIManagerArbitrage(LiquidityAPIManagerArbitrageUpdateRequest Request, long UserId);

        GetAllLiquidityAPIProviderManagerArbitrage GetAllLiquidityAPIProviderManagerArbitrage(int? PageSize, int Page);
        LiquidityAPIProviderManagerArbitrage GetLiquidityAPIProviderManagerArbitrage(long Id);
        int UpdateLiquidityAPIProviderManagerArbitrage(LiquidityAPIProviderManagerArbitrageUpdateRequest Request, long UserId);
        int AddLiquidityAPIProviderManagerArbitrage(LiquidityAPIProviderManagerArbitrageUpdateRequest Request, long UserId);
        #endregion
        long AddArbitragePairConfiguration(TradePairConfigRequest Request, long UserId);//Darshan Dholakiya Added from Arbitrage configration changes:04-06-2019
        long UpdatePairConfigurationArbitrage(TradePairConfigRequest Request, long UserId);//Darshan Dholakiya Added from Arbitrage configration changes:05-06-2019
        TradePairConfigRequest GetPairConfigurationArbitrage(long PairId); //Darshan Dholakiya Added from Arbitrage configration changes:05-06-2019
        GetAllTradePairConfigResponse GetAllPairConfigurationArbitrage(int? PageSize, int Page=0,short IsMargin = 0);//Darshan Dholakiya Added from Arbitrage configration changes:05-06-2019
        int SetActivePairArbitrage(long     PairId);//Darshan Dholakiya Added from Arbitrage configration changes:05-06-2019
        int SetInActivePairArbitrage(long PairId);//Darshan Dholakiya Added from Arbitrage configration changes:05-06-2019
        ViewModels.Configuration.ListPairResponse ListPairArbitrage(short IsMargin = 0);//Darshan Dholakiya Added from Arbitrage configration changes:06-06-2019

        #region Arbitrage Provider Methods
        GetAllServiceProvideViewModel GetAllProviderArbitrage(int? PageSize, int Page);//Darshan Dholakiya Added for Arbitrage configration changes:07-06-2019
        ServiceProviderViewModel GetPoviderByIDArbitrage(long ID);//Darshan Dholakiya Added for Arbitrage configration changes:07-06-2019
        long AddProviderServiceArbitrage(ServiceProviderRequest request, long UserId);//Darshan Dholakiya Added for Arbitrage configration changes:07-06-2019
        bool UpdateProviderServiceArbitrage(ServiceProviderRequest request, long UserId);//Darshan Dholakiya Added for Arbitrage configration changes:07-06-2019

        Task<long> AddServiceConfigurationArbitrage(ServiceConfigurationRequest Request, long UserId);//Darshan Dholakiya Added for Arbitrage Service configration changes:10-06-2019

        long UpdateServiceConfigurationArbitrage(ServiceConfigurationRequest Request, long UserId);//Darshan Dholakiya Added for Arbitrage Service configration changes:11-06-2019

        ServiceConfigurationRequest GetServiceConfigurationArbitrage(long ServiceId);//Darshan Dholakiya Added for Arbitrage Service configration changes:11-06-2019

        ServiceConfigurationGetAllResponse GetAllServiceConfigurationArbitrage(int? PageSize,int Page=0,int StatusData = 0, short IsMargin = 0);//Darshan Dholakiya Added for Arbitrage Service configration changes:11-06-2019

        List<MarketViewModel> GetAllMarketDataArbitrage(short ActiveOnly = 1);//Darshan Dholakiya Added for Market Data Service changes:11-06-2019

        List<ServiceCurrencyData> GetAllServiceConfigurationByBaseArbitrage(String Base); //Darshan Dholakiya Added for  Data Service changes:11-06-2019

        int SetActiveServiceArbitrage(long ServiceId);//Darshan Dholakiya Added  for  Service changes:11-06-2019
        int SetInActiveServiceArbitrage(long ServiceId);//Darshan Dholakiya Added for the Service Changes:11-06-2019 

        GetServiceByBaseReasponse GetCurrencyArbitrage(short ActiveOnly = 0);//Darshan Dholakiya for the Service Changes:11-06-2019

        #endregion

        #region Arbitrage Trade
        long AddTradeRouteConfigurationArbitrage(TradeRouteConfigRequest Request, long UserId);//Darshan Dholakiya added this method for Arbitrage trade changes:12-06-2019
        long UpdateTradeRouteConfigurationArbitrage(TradeRouteConfigRequest Request, long UserId);//Darshan Dholakiya added this method for Arbitrage trade changes:12-06-2019
        List<GetTradeRouteConfigurationData> GetAllTradeRouteConfigurationArbitrage();//Darshan Dholakiya added this method for Arbitrage trade changes:12-06-2019

        GetTradeRouteConfigurationData GetTradeRouteConfigurationArbitrageInfo(long Id);//Darshan Dholakiya added this method for Arbitrage trade changes:13-06-2019

        long UpdateTradeRoutePriorityArbitrage(TradeRoutePriorityUpdateRequest Request, long UserId);//Darshan Dholakiya added this method for Arbitrage trade changes:13-06-2019

        List<GetTradeRouteConfigurationData> GetTradeRouteForPriorityArbitrage(long PairId, long OrderType, int TrnType);//Darshan Dholakiya added this method for Arbitrage trade changes:13-06-2019

        List<AvailableRoute> GetAvailableTradeRouteArbitrage(int TrnType);//Darshan Dholakiya added this method for Arbitrage trade changes:13-06-2019

        BizResponseClass AddMarketDataV2Arbitrage(MarketDataRequest Request, long UserId);//Darshan Dholakiya added this method for Arbitrage market changes:14-06-2019

        MarketDataResponse UpdateMarketDataV2Arbitrage(MarketDataRequest Request, long UserId);//Darshan Dholakiya added this method for Arbitrage market changes:14-06-2019


        #endregion
        #region ArbitrageProvideDetails
        IEnumerable<ProviderDetailViewModel> GetProviderDetailListArbitrage();//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019

        IEnumerable<ProviderDetailGetAllResponse> getProviderDetailsDataListArbitrage(IEnumerable<ProviderDetailViewModel> dataList);//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019

        ProviderDetailViewModel GetProviderDetailByIdArbitrage(long id);//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019

        ProviderDetailGetAllResponse getProviderDetailDataByIdArbitrage(ProviderDetailViewModel viewModel);//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019

        long AddProviderDetailArbitrage(ProviderDetailRequest request, long UserId);//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019

        ProviderTypeViewModel GetProviderTypeByIdArbitrage(long id);//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019
        LimitViewModel GetLimitByIdArbitrage(long id);//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019
        ProviderConfigurationViewModel GetProviderConfigurationArbitrage(long id);//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019
        ThirdPartyAPIConfigViewModel GetThirdPartyAPIConfigByIdArbitrage(long Id);//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019
        DemonconfigurationViewModel GetDemonConfigurationArbitrage(long id);//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019

        bool UpdateProviderDetailArbitrage(ProviderDetailRequest request, long UserId);//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019

        bool SetActiveProviderDetailArbitrage(long id);//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019

        bool SetInActiveProviderDetailArbitrage(long id);//Darshan Dholakiya added this method for Arbitrage provider changes:17-06-2019

        #endregion

        #region ArbitrageProductConfig
        long AddProductConfigurationArbitrage(ProductConfigurationRequest Request, long UserId);//Darshan Dholakiya added this method for Arbitrage product changes:18-06-2019

        long UpdateProductConfigurationArbitrage(ProductConfigurationRequest Request, long UserId);//Darshan Dholakiya added this method for Arbitrage product changes:18-06-2019

        ProductConfigrationGetInfo GetProductConfigurationArbitrage(long ProductId);//Darshan Dholakiya added this method for Arbitrage product changes:18-06-2019

        List<ProductConfigrationGetInfo> GetAllProductConfigurationArbitrage();//Darshan Dholakiya added this method for Arbitrage product changes:18-06-2019

        int SetActiveProductArbitrage(long ProductId);//Darshan Dholakiya added this method for Arbitrage product changes:18-06-2019
        int SetInActiveProductArbitrage(long ProductId);//Darshan Dholakiya added this method for Arbitrage product changes:18-06-2019

        #endregion
        #region ArbitrageThirdPartyConfig
        ThirdPartyAPIConfigResponseAllData GetAllThirdPartyAPIConfigArbitrage( int? PageSize, int Page);//Darshan Dholakiya added this method for Arbitrage product changes:18-06-2019

        long AddThirdPartyAPIArbitrage(ThirdPartyAPIConfigRequest request, long UserId);//Darshan Dholakiya added this method for Arbitrage Third party API changes:19-06-2019

        bool UpdateThirdPartyAPIArbitrage(ThirdPartyAPIConfigRequest request, long UserId);//Darshan Dholakiya added this method for Arbitrage Third party API changes:19-06-2019

        bool SetActiveThirdPartyAPIArbitrage(long id);//Darshan Dholakiya added this method for Arbitrage Third party API changes:19-06-2019

        bool SetInActiveThirdPartyAPIArbitrage(long id);//Darshan Dholakiya added this method for Arbitrage Third party API changes:19-06-2019

        #endregion
        #region ArbitrageThirdPartyAPIResponse
        ThirdPartyAPIResponseConfigResponseAllData GetAllThirdPartyAPIResponseArbitrage(int? PageSize, int Page);//Darshan Dholakiya added this method for Arbitrage Third party API changes:19-06-2019

        ThirdPartyAPIResponseConfigViewModel GetThirdPartyAPIResponseByIdArbitrage(long id);//Darshan Dholakiya added this method for Arbitrage Third party API changes:19-06-2019

        long AddThirdPartyAPIResponseArbitrage(ThirdPartyAPIResponseConfigViewModel Request, long UserId);//Darshan Dholakiya added this method for Arbitrage Third party API changes:19-06-2019

        bool UpdateThirdPartyAPIResponseArbitrage(ThirdPartyAPIResponseConfigViewModel Request, long UserId);//Darshan Dholakiya added this method for Arbitrage Third party API changes:19-06-2019

        bool SetActiveThirdPartyAPIResponseArbitrage(long id);//Darshan Dholakiya added this method for Arbitrage Third party API changes:20-06-2019
        bool SetInActiveThirdPartyAPIResponseArbitrage(long id);//Darshan Dholakiya added this method for Arbitrage Third party API changes:20-06-2019


        #endregion
        #region Demon Config Arbitrage
        long AddDemonConfigurationArbitrage(DemonConfigurationRequest request, long UserId);//Darshan Dholakiya added this method for demon Arbitrage changes:21-06-2019

        bool UpdateDemonConfigurationArbitrage(DemonConfigurationRequest request, long UserId);//Darshan Dholakiya added this method for demon Arbitrage changes:21-06-2019

        bool SetActiveDemonConfigArbitrage(long id);//Darshan Dholakiya added this method for demon Arbitrage changes:22-06-2019

        bool SetInActiveDemonConfigArbitrage(long id);//Darshan Dholakiya added this method for demon Arbitrage changes:22-06-2019

        ListDemonConfigResponse GetAllDemonConfigV1Arbitrage();//Darshan Dholakiya added this method for demon Arbitrage changes:22-06-2019

        ListDemonConfigResponseV1 ListDemonConfigV1Arbitrage();//Darshan Dholakiya added this method for demon Arbitrage changes:22-06-2019

        #endregion
        #region ArbitragemethodsRequiredByFront
        List<MarketTickerPairData> GetMarketTickerPairDataArbitrage(short IsMargin = 0);//Darshan Dholakiya added this method for Market Ticker Pair Arbitrage changes:22-06-2019

        int UpdateMarketTickerPairDataArbitrage(UpdateMarketTickerPairData Request, long UserId, short IsMargin = 0);//Darshan Dholakiya added this method for Market Ticker Pair Arbitrage changes:22-06-2019

        List<LimitViewModel> GetAllLimitDataArbitrage();//Darshan Dholakiya added this method for Limit Arbitrage changes:22-06-2019

        int AddLiquidityAPIManagerArbitrage(LiquidityAPIManagerRequest Request, long UserId);//Darshan Dholakiya added this method for add Liquidity Arbitrage changes:22-06-2019

        int UpdateLiquidityAPIManagerArbitrage(LiquidityAPIManagerUpdateRequest Request, long UserId);//Darshan Dholakiya added this method for update Liquidity Arbitrage changes:22-06-2019
        #endregion
        ConfigureLPArbitrage[] TradePairConfigurationArbitrageV1();

        Task UpdateFrontCache(string MethodName);
    }
}
