using Worldex.Core.Entities;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using Worldex.Core.ViewModels.FiatBankIntegration;
using System.Linq;
using Worldex.Infrastructure.Data;

namespace Worldex.Infrastructure.Services
{
    public class TrnMasterConfiguration : ITrnMasterConfiguration
    {
        private List<ServiceMaster> serviceMasterList;
        private List<LPTPairFiat> LPTPairFiatList;
        private List<ServiceProviderMaster> serviceProviderMasterList;
        private List<ServiceProviderDetail> serviceProviderDetailList;
        private List<RouteConfiguration> routeConfigurationList;
        private List<TradePairMaster> tradePairMasterList;
        private List<TradePairDetail> tradePairDetailList;
        private IMemoryCache _cache;
        private List<CronMaster> CronMasterList;
        private readonly ICommonRepository<ServiceMaster> _serviceMasterRepo;
        private readonly ICommonRepository<ServiceProviderMaster> _serviceProviderMasterRepo;
        private readonly ICommonRepository<ServiceProviderDetail> _serviceProviderDetailRepo;
        private readonly ICommonRepository<RouteConfiguration> _routeConfigurationRepo;
        private readonly ICommonRepository<TradePairMaster> _tradePairMasterRepo;
        private readonly ICommonRepository<TradePairDetail> _tradePairDetailRepo;
        private readonly ICommonRepository<AppType> _AppTypeRepo;
        //Rita 15-2-19 Margin Trading
        private List<ServiceMasterMargin> serviceMasterMarginList;
        private readonly ICommonRepository<ServiceMasterMargin> _serviceMasterMarginRepo;
        private List<TradePairMasterMargin> tradePairMasterMarginList;
        private List<TradePairDetailMargin> tradePairDetailMarginList;
        private readonly ICommonRepository<TradePairMasterMargin> _tradePairMasterMarginRepo;
        private readonly ICommonRepository<TradePairDetailMargin> _tradePairDetailMarginRepo;
        private List<Market> MarketList;
        private readonly ICommonRepository<Market> _MarketRepo;
        private List<MarketMargin> MarketMarginList;
        private readonly ICommonRepository<MarketMargin> _MarketMarginRepo;
        //==============================
        //Rita 31-05-19 Arbitrage Trading
        private List<ServiceMasterArbitrage> serviceMasterArbitrageList;
        private readonly ICommonRepository<ServiceMasterArbitrage> _serviceMasterArbitrageRepo;
        private List<TradePairMasterArbitrage> tradePairMasterArbitrageList;
        private List<TradePairDetailArbitrage> tradePairDetailArbitrageList;
        private readonly ICommonRepository<TradePairMasterArbitrage> _tradePairMasterArbitrageRepo;
        private readonly ICommonRepository<TradePairDetailArbitrage> _tradePairDetailArbitrageRepo;
        private List<MarketArbitrage> MarketArbitrageList;
        private readonly ICommonRepository<MarketArbitrage> _MarketArbitrageRepo;
        // khushali 05-06-2019 for Exchange Configuration
        private readonly ICommonRepository<ServiceProviderMasterArbitrage> _serviceProviderMasterArbitrageRepo;
        private readonly ICommonRepository<ServiceProviderDetailArbitrage> _serviceProviderDetailArbitrageRepo;
        private readonly ICommonRepository<RouteConfigurationArbitrage> _routeConfigurationArbitrageRepo;
        private List<ServiceProviderMasterArbitrage> serviceProviderMasterArbitrageList;
        private List<ServiceProviderDetailArbitrage> serviceProviderDetailArbitrageList;
        private List<RouteConfigurationArbitrage> routeConfigurationArbitrageList;
        private readonly ICommonRepository<LPFeedConfiguration> _LPFeedConfigurationRepo; // khushali 11-07-2019
        private readonly ICommonRepository<TradingConfiguration> _TradingConfigurationRepo; // khushali 11-07-2019
        private readonly ICommonRepository<CronMaster> _CronMaster;
        private List<AppType> AppTypeList;
        //==============================
        private IFiatIntegrateRepository _fiatIntegrateRepository;
        //private readonly IMemoryCache _cache;

        public TrnMasterConfiguration(ICommonRepository<ServiceMaster> serviceMasterRepo, ICommonRepository<ServiceMasterMargin> serviceMasterMarginRepo,
            ICommonRepository<ServiceProviderMaster> serviceProviderMasterRepo,
            ICommonRepository<ServiceProviderDetail> serviceProviderDetailRepo,
            ICommonRepository<RouteConfiguration> routeConfigurationRepo,
            ICommonRepository<TradePairMaster> tradePairMasterRepo, ICommonRepository<TradePairDetail> tradePairDetailRepo, IMemoryCache cache, IFiatIntegrateRepository fiatIntegrateRepository,
            ICommonRepository<TradePairMasterMargin> tradePairMasterMarginRepo, ICommonRepository<TradePairDetailMargin> tradePairDetailMarginRepo,
            ICommonRepository<Market> MarketRepo, ICommonRepository<MarketMargin> MarketMarginRepo,
            ICommonRepository<ServiceMasterArbitrage> serviceMasterArbitrageRepo, ICommonRepository<TradePairMasterArbitrage> tradePairMasterArbitrageRepo,
            ICommonRepository<TradePairDetailArbitrage> tradePairDetailArbitrageRepo, ICommonRepository<MarketArbitrage> MarketArbitrageRepo,
            ICommonRepository<RouteConfigurationArbitrage> RouteConfigurationArbitrageRepo,
            ICommonRepository<ServiceProviderDetailArbitrage> ServiceProviderDetailArbitrageRepo,
            ICommonRepository<ServiceProviderMasterArbitrage> ServiceProviderMasterArbitrageRepo, ICommonRepository<AppType> AppTypeRepo, ICommonRepository<CronMaster> CronMaster
            )
        {
            _AppTypeRepo = AppTypeRepo;
            _serviceMasterRepo = serviceMasterRepo;
            _serviceProviderMasterRepo = serviceProviderMasterRepo;
            _serviceProviderDetailRepo = serviceProviderDetailRepo;
            _routeConfigurationRepo = routeConfigurationRepo;
            _tradePairMasterRepo = tradePairMasterRepo;
            _tradePairDetailRepo = tradePairDetailRepo;
            _cache = cache;
            _fiatIntegrateRepository = fiatIntegrateRepository;
            _CronMaster = CronMaster;
            //serviceMasterList = _serviceMasterRepo.List();
            //serviceProviderMasterList = _serviceProviderMasterRepo.List();
            //serviceProviderDetailList = _serviceProviderDetailRepo.List();
            //routeConfigurationList = _routeConfigurationRepo.List();
            //tradePairMasterList = _tradePairMasterRepo.List();
            //tradePairDetailList = _tradePairDetailRepo.List();

            serviceMasterList = _cache.Get<List<ServiceMaster>>("ServiceMaster");
            if (serviceMasterList == null)
            {
                var Result = _serviceMasterRepo.List();
                _cache.Set("ServiceMaster", Result);
            }
            LPTPairFiatList = _cache.Get<List<LPTPairFiat>>("LPTPairFiat");
            if (LPTPairFiatList == null)
            {
                var Result = _fiatIntegrateRepository.GetPairForBinnance().ToList();
                _cache.Set("LPTPairFiat", Result);
            }

            serviceProviderMasterList = _cache.Get<List<ServiceProviderMaster>>("ServiceProviderMasterList");
            if (serviceProviderMasterList == null)
            {
                var Result = _serviceProviderMasterRepo.List();
                _cache.Set("ServiceProviderMasterList", Result);
            }

            serviceProviderDetailList = _cache.Get<List<ServiceProviderDetail>>("ServiceProviderDetailList");
            if (serviceProviderDetailList == null)
            {
                var Result = _serviceProviderDetailRepo.List();
                _cache.Set("ServiceProviderDetailList", Result);
            }

            routeConfigurationList = _cache.Get<List<RouteConfiguration>>("RouteConfigurationList");
            if (serviceProviderDetailList == null)
            {
                var Result = _routeConfigurationRepo.List();
                _cache.Set("RouteConfigurationList", Result);
            }

            tradePairMasterList = _cache.Get<List<TradePairMaster>>("TradePairMasterList");
            if (tradePairMasterList == null)
            {
                var Result = _tradePairMasterRepo.List();
                _cache.Set("TradePairMasterList", Result);
            }

            tradePairDetailList = _cache.Get<List<TradePairDetail>>("TradePairDetailList");
            if (tradePairDetailList == null)
            {
                var Result = _tradePairDetailRepo.List();
                _cache.Set("TradePairDetailList", Result);
            }
            //Rita 15-2-19 Margin Trading========
            _serviceMasterMarginRepo = serviceMasterMarginRepo;
            _tradePairMasterMarginRepo = tradePairMasterMarginRepo;
            _tradePairDetailMarginRepo = tradePairDetailMarginRepo;
            _MarketRepo = MarketRepo;
            _MarketMarginRepo = MarketMarginRepo;

            serviceMasterMarginList = _cache.Get<List<ServiceMasterMargin>>("ServiceMasterMargin");
            if (serviceMasterMarginList == null)
            {
                var Result = _serviceMasterMarginRepo.List();
                _cache.Set("ServiceMasterMargin", Result);
            }
            tradePairMasterMarginList = _cache.Get<List<TradePairMasterMargin>>("TradePairMasterMarginList");
            if (tradePairMasterMarginList == null)
            {
                var Result = _tradePairMasterMarginRepo.List();
                _cache.Set("TradePairMasterMarginList", Result);
            }

            tradePairDetailMarginList = _cache.Get<List<TradePairDetailMargin>>("TradePairDetailMarginList");
            if (tradePairDetailMarginList == null)
            {
                var Result = _tradePairDetailMarginRepo.List();
                _cache.Set("TradePairDetailMarginList", Result);
            }
            MarketList = _cache.Get<List<Market>>("Market");//taken from cache
            if (MarketList == null)
            {
                var Result = _MarketRepo.List();
                _cache.Set("Market", Result);
            }
            MarketMarginList = _cache.Get<List<MarketMargin>>("MarketMargin");
            if (MarketList == null)
            {
                var Result = _MarketMarginRepo.List();
                _cache.Set("MarketMargin", Result);
            }
            //====================================
            //Rita 31-2-19 Arbitrage Trading========
            _serviceMasterArbitrageRepo = serviceMasterArbitrageRepo;
            _tradePairMasterArbitrageRepo = tradePairMasterArbitrageRepo;
            _tradePairDetailArbitrageRepo = tradePairDetailArbitrageRepo;
            _MarketRepo = MarketRepo;
            _MarketArbitrageRepo = MarketArbitrageRepo;
            _routeConfigurationArbitrageRepo = RouteConfigurationArbitrageRepo;
            _serviceProviderDetailArbitrageRepo = ServiceProviderDetailArbitrageRepo;
            _serviceProviderMasterArbitrageRepo = ServiceProviderMasterArbitrageRepo;

            serviceMasterArbitrageList = _cache.Get<List<ServiceMasterArbitrage>>("ServiceMasterArbitrage");
            if (serviceMasterArbitrageList == null)
            {
                var Result = _serviceMasterArbitrageRepo.List();
                _cache.Set("ServiceMasterArbitrage", Result);
            }
            tradePairMasterArbitrageList = _cache.Get<List<TradePairMasterArbitrage>>("TradePairMasterArbitrageList");
            if (tradePairMasterArbitrageList == null)
            {
                var Result = _tradePairMasterArbitrageRepo.List();
                _cache.Set("TradePairMasterArbitrageList", Result);
            }

            tradePairDetailArbitrageList = _cache.Get<List<TradePairDetailArbitrage>>("TradePairDetailArbitrageList");
            if (tradePairDetailArbitrageList == null)
            {
                var Result = _tradePairDetailArbitrageRepo.List();
                _cache.Set("TradePairDetailArbitrageList", Result);
            }
            MarketList = _cache.Get<List<Market>>("Market");//taken from cache
            if (MarketList == null)
            {
                var Result = _MarketRepo.List();
                _cache.Set("Market", Result);
            }
            MarketArbitrageList = _cache.Get<List<MarketArbitrage>>("MarketArbitrage");
            if (MarketArbitrageList == null)
            {
                var Result = _MarketArbitrageRepo.List();
                _cache.Set("MarketArbitrage", Result);
            }
            //====================================

            //khushali 05-06-2019 for Arbitrage exchange configuration==========
            serviceProviderMasterArbitrageList = _cache.Get<List<ServiceProviderMasterArbitrage>>("ServiceProviderMasterArbitrageList");
            if (serviceProviderMasterArbitrageList == null)
            {
                var Result = _serviceProviderMasterArbitrageRepo.List();
                _cache.Set("ServiceProviderMasterArbitrageList", Result);
            }

            serviceProviderDetailArbitrageList = _cache.Get<List<ServiceProviderDetailArbitrage>>("ServiceProviderDetailArbitrageList");
            if (serviceProviderDetailArbitrageList == null)
            {
                var Result = _serviceProviderDetailArbitrageRepo.List();
                _cache.Set("ServiceProviderDetailArbitrageList", Result);
            }

            routeConfigurationArbitrageList = _cache.Get<List<RouteConfigurationArbitrage>>("RouteConfigurationArbitrageList");
            if (routeConfigurationArbitrageList == null)
            {
                var Result = _routeConfigurationArbitrageRepo.List();
                _cache.Set("RouteConfigurationArbitrageList", Result);
            }
            //====================================

            AppTypeList = _cache.Get<List<AppType>>("AppTypeList");
            if (AppTypeList == null)
            {
                var Result = _AppTypeRepo.List();
                _cache.Set("AppTypeList", Result);
            }
            CronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (CronMasterList == null)
            {
                var Result = _CronMaster.List();
                _cache.Set("CronMaster", Result);
            }
        }
        public List<CronMaster> GetCronMaster()
        {
            CronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            return CronMasterList;
        }
        public List<LPTPairFiat> LPTPairFiat()
        {
            //IList<LPTPairFiat> Result = _fiatIntegrateRepository.GetPairForBinnance();
            //List<LPTPairFiat> ConfigurationList = Result.ToList();
            //_cache.Set<IReadOnlyList<LPTPairFiat>>("LPTPairFiat", ConfigurationList);
            LPTPairFiatList = _cache.Get<List<LPTPairFiat>>("LPTPairFiat");
            return LPTPairFiatList;
        }
        public void UpdateCronMasterList()
        {
            var Result = _CronMaster.List();
            _cache.Set("CronMaster", Result);
        }
        public List<AppType> GetAppTypes()
        {
            AppTypeList = _cache.Get<List<AppType>>("AppTypeList");
            return AppTypeList;
        }

        public List<ServiceMaster> GetServices()
        {
            serviceMasterList = _cache.Get<List<ServiceMaster>>("ServiceMaster");
            return serviceMasterList;
        }

        public List<ServiceProviderMaster> GetServiceProviderMaster()
        {
            serviceProviderMasterList = _cache.Get<List<ServiceProviderMaster>>("ServiceProviderMasterList");
            return serviceProviderMasterList;
        }

        public List<ServiceProviderDetail> GetServiceProviderDetail()
        {
            serviceProviderDetailList = _cache.Get<List<ServiceProviderDetail>>("ServiceProviderDetailList");
            return serviceProviderDetailList;
        }

        public List<RouteConfiguration> GetRouteConfiguration()
        {
            routeConfigurationList = _cache.Get<List<RouteConfiguration>>("RouteConfigurationList");
            return routeConfigurationList;
        }

        public List<TradePairMaster> GetTradePairMaster()
        {
            tradePairMasterList = _cache.Get<List<TradePairMaster>>("TradePairMasterList");
            return tradePairMasterList;
        }

        public List<TradePairDetail> GetTradePairDetail()
        {
            tradePairDetailList = _cache.Get<List<TradePairDetail>>("TradePairDetailList");
            return tradePairDetailList;
        }
        //Rita 15-2-19 Margin Trading
        public List<ServiceMasterMargin> GetServicesMargin()
        {
            serviceMasterMarginList = _cache.Get<List<ServiceMasterMargin>>("ServiceMasterMargin");
            return serviceMasterMarginList;
        }

        public List<TradePairMasterMargin> GetTradePairMasterMargin()
        {
            tradePairMasterMarginList = _cache.Get<List<TradePairMasterMargin>>("TradePairMasterMarginList");
            return tradePairMasterMarginList;
        }

        public List<TradePairDetailMargin> GetTradePairDetailMargin()
        {
            tradePairDetailMarginList = _cache.Get<List<TradePairDetailMargin>>("TradePairDetailMarginList");
            return tradePairDetailMarginList;
        }

        public List<Market> GetMarket()
        {
            MarketList = _cache.Get<List<Market>>("Market");
            return MarketList;
        }

        public List<MarketMargin> GetMarketMargin()
        {
            MarketMarginList = _cache.Get<List<MarketMargin>>("MarketMargin");
            return MarketMarginList;
        }

        public void UpdateServiceMarginList()//23-2-19 for update Margin tables
        {
            var Result = _serviceMasterMarginRepo.List();
            _cache.Set("ServiceMasterMargin", Result);
        }

        public void UpdateTradePairMasterMarginList()
        {
            var Result = _tradePairMasterMarginRepo.List();
            _cache.Set("TradePairMasterMarginList", Result);
        }

        public void UpdateTradePairDetailMarginList()
        {
            var Result = _tradePairDetailMarginRepo.List();
            _cache.Set("TradePairDetailMarginList", Result);
        }

        public void UpdateMarket()
        {
            var Result = _MarketRepo.List();
            _cache.Set("Market", Result);
        }

        public void UpdateMarketMargin()
        {
            var Result = _MarketMarginRepo.List();
            _cache.Set("MarketMargin", Result);
        }

        //====================================
        //Rita 31-05-19 Arbitrage Trading
        public List<ServiceMasterArbitrage> GetServicesArbitrage()
        {
            serviceMasterArbitrageList = _cache.Get<List<ServiceMasterArbitrage>>("ServiceMasterArbitrage");
            return serviceMasterArbitrageList;
        }

        public List<TradePairMasterArbitrage> GetTradePairMasterArbitrage()
        {
            tradePairMasterArbitrageList = _cache.Get<List<TradePairMasterArbitrage>>("TradePairMasterArbitrageList");
            return tradePairMasterArbitrageList;
        }

        public List<TradePairDetailArbitrage> GetTradePairDetailArbitrage()
        {
            tradePairDetailArbitrageList = _cache.Get<List<TradePairDetailArbitrage>>("TradePairDetailArbitrageList");
            return tradePairDetailArbitrageList;
        }

        public List<MarketArbitrage> GetMarketArbitrage()
        {
            MarketArbitrageList = _cache.Get<List<MarketArbitrage>>("MarketArbitrage");
            return MarketArbitrageList;
        }

        public void UpdateServiceArbitrageList()//23-2-19 for update Arbitrage tables
        {
            var Result = _serviceMasterArbitrageRepo.List();
            _cache.Set("ServiceMasterArbitrage", Result);
        }

        public void UpdateTradePairMasterArbitrageList()
        {
            var Result = _tradePairMasterArbitrageRepo.List();
            _cache.Set("TradePairMasterArbitrageList", Result);
        }

        public void UpdateTradePairDetailArbitrageList()
        {
            var Result = _tradePairDetailArbitrageRepo.List();
            _cache.Set("TradePairDetailArbitrageList", Result);
        }
        public void UpdateMarketArbitrage()
        {
            var Result = _MarketArbitrageRepo.List();
            _cache.Set("MarketArbitrage", Result);
        }
        //====================================

        //khushali 05-06-2019 for Arbitrage exchange configuration==========

        public List<ServiceProviderMasterArbitrage> GetServiceProviderMasterArbitrageList()
        {
            serviceProviderMasterArbitrageList = _cache.Get<List<ServiceProviderMasterArbitrage>>("ServiceProviderMasterArbitrageList");
            return serviceProviderMasterArbitrageList;
        }

        public void UpdateServiceProividerMasterArbitrageList()
        {
            // serviceProviderMasterList = _serviceProviderMasterRepo.List();
            var Result = _serviceProviderMasterArbitrageRepo.List();
            _cache.Set("ServiceProviderMasterArbitrageList", Result);
        }

        public List<ServiceProviderDetailArbitrage> GetServiceProviderDetailArbitrage()
        {
            serviceProviderDetailArbitrageList = _cache.Get<List<ServiceProviderDetailArbitrage>>("ServiceProviderDetailArbitrageList");
            return serviceProviderDetailArbitrageList;
        }

        public void UpdateServiceProviderDetailArbitrageList()
        {
            //serviceProviderDetailList = _serviceProviderDetailRepo.List();
            var Result = _serviceProviderDetailArbitrageRepo.List();
            _cache.Set("ServiceProviderDetailArbitrageList", Result);
        }

        public void UpdateRouteConfigurationArbitrageList()
        {
            // routeConfigurationList = _routeConfigurationRepo.List();
            var Result = _routeConfigurationArbitrageRepo.List();
            _cache.Set("RouteConfigurationArbitrageList", Result);
        }
        //====================================

        public void UpdateServiceList()
        {
            //serviceMasterList = _serviceMasterRepo.List();
            var Result = _serviceMasterRepo.List();
            _cache.Set("ServiceMaster", Result);
        }

        public void UpdateServiceProividerMasterList()
        {
            // serviceProviderMasterList = _serviceProviderMasterRepo.List();
            var Result = _serviceProviderMasterRepo.List();
            _cache.Set("ServiceProviderMasterList", Result);
        }

        public void UpdateServiceProviderDetailList()
        {
            //serviceProviderDetailList = _serviceProviderDetailRepo.List();
            var Result = _serviceProviderDetailRepo.List();
            _cache.Set("ServiceProviderDetailList", Result);
        }

        public void UpdateRouteConfigurationList()
        {
            // routeConfigurationList = _routeConfigurationRepo.List();
            var Result = _routeConfigurationRepo.List();
            _cache.Set("RouteConfigurationList", Result);
        }

        public void UpdateTradePairMasterList()
        {
            // tradePairMasterList = _tradePairMasterRepo.List();
            var Result = _tradePairMasterRepo.List();
            _cache.Set("TradePairMasterList", Result);
        }

        public void UpdateTradePairDetailList()
        {
            //tradePairDetailList = _tradePairDetailRepo.List();
            var Result = _tradePairDetailRepo.List();
            _cache.Set("TradePairDetailList", Result);
        }

        public void UpdateServiceListArbitrage()
        {
            //serviceMasterArbitrageList = _serviceMasterArbitrageRepo.List();
            var Result = _serviceMasterArbitrageRepo.List();
            _cache.Set("ServiceMasterArbitrage", Result);
        }

        public void UpdateServiceProividerMasterListArbitrage()
        {

            //serviceProviderMasterArbitrageList = _serviceProviderMasterArbitrageRepo.List();
            var Result = _serviceProviderMasterArbitrageRepo.List();
            _cache.Set("ServiceProviderMasterArbitrage", Result);
        }

        public void UpdateServiceProviderDetailListArbitrage()
        {
            //serviceProviderDetailArbitrageList = _serviceProviderDetailArbitrageRepo.List();
            var Result = _serviceProviderDetailArbitrageRepo.List();
            _cache.Set("ServiceProviderDetailArbitrage", Result);
        }

        public void UpdateRouteConfigurationListArbitrage()
        {

            // routeConfigurationArbitrageList = _routeConfigurationArbitrageRepo.List();
            var Result = _routeConfigurationArbitrageRepo.List();
            _cache.Set("RouteConfigurationArbitrage", Result);
        }

        public void UpdateTradePairMasterListArbitrage()
        {

            //tradePairMasterArbitrageList = _tradePairMasterArbitrageRepo.List();
            var Result = _tradePairMasterArbitrageRepo.List();
            _cache.Set("TradePairMasterArbitrage", Result);
        }

        public void UpdateTradePairDetailListArbitrage()
        {
            //tradePairDetailArbitrageList = _tradePairDetailArbitrageRepo.List();
            var Result = _tradePairDetailArbitrageRepo.List();
            _cache.Set("TradePairDetailArbitrage", Result);
        }

        public void LPFeedConfigurationList()
        {

            var Result = _LPFeedConfigurationRepo.List();
            _cache.Set("LPFeedConfiguration", Result);
        }

        public void TradingConfigurationList()
        {
            var Result = _TradingConfigurationRepo.List();
            _cache.Set("TradingConfiguration", Result);
        }
    }
}