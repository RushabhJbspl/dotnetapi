using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Transaction;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces
{
    public interface ITrnMasterConfiguration
    {
		void UpdateCronMasterList();
        List<CronMaster> GetCronMaster();
        List<AppType> GetAppTypes();
        List<ServiceMaster> GetServices();
        List<ServiceProviderMaster> GetServiceProviderMaster();
        List<ServiceProviderDetail> GetServiceProviderDetail();
        List<RouteConfiguration> GetRouteConfiguration();
        List<TradePairMaster> GetTradePairMaster();
        List<TradePairDetail> GetTradePairDetail();
        //Rita 15-2-19 Margin Trading==============
        List<ServiceMasterMargin> GetServicesMargin();
        List<TradePairMasterMargin> GetTradePairMasterMargin();
        List<TradePairDetailMargin> GetTradePairDetailMargin();
        void UpdateServiceMarginList();
        void UpdateTradePairMasterMarginList();
        void UpdateTradePairDetailMarginList();
        List<Market> GetMarket();
        List<MarketMargin> GetMarketMargin();
        void UpdateMarket();
        void UpdateMarketMargin();
        //=========================================
        void UpdateServiceList();
        void UpdateServiceProividerMasterList();
        void UpdateServiceProviderDetailList();
        void UpdateRouteConfigurationList();
        void UpdateTradePairMasterList();
        void UpdateTradePairDetailList();

        //Rita 31-05-19 Arbitrage Trading==============
        List<ServiceMasterArbitrage> GetServicesArbitrage();
        List<TradePairMasterArbitrage> GetTradePairMasterArbitrage();
        List<TradePairDetailArbitrage> GetTradePairDetailArbitrage();
        List<MarketArbitrage> GetMarketArbitrage();
        void UpdateServiceArbitrageList();
        void UpdateTradePairMasterArbitrageList();
        void UpdateTradePairDetailArbitrageList();
        void UpdateMarketArbitrage();
        List<ServiceProviderDetailArbitrage> GetServiceProviderDetailArbitrage();
        List<ServiceProviderMasterArbitrage> GetServiceProviderMasterArbitrageList();
        //=========================================

        //khushali 05-06-2019 for Arbitrage exchange configuration==========
        void UpdateServiceProividerMasterArbitrageList();
        void UpdateServiceProviderDetailArbitrageList();
        void UpdateRouteConfigurationArbitrageList();

        //=========================================
        //Darshan Dholakiya added this method for Arbitrage Reload Configuration Changes : 20-06-2019
        void UpdateServiceListArbitrage();

        void UpdateServiceProividerMasterListArbitrage();

        void UpdateServiceProviderDetailListArbitrage();

        void UpdateRouteConfigurationListArbitrage();

        void UpdateTradePairMasterListArbitrage();

        void UpdateTradePairDetailListArbitrage();

        //=======================================================================================

        //khushali 11-07-2019 fo LP feed configuration==========
        void LPFeedConfigurationList();
        void TradingConfigurationList();

        //=========================================


    }
}
