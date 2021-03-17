using AutoMapper.Configuration;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.WalletConfiguration;
using Worldex.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Worldex.Infrastructure.DTOClasses;

namespace Worldex.Infrastructure.Services.Configuration
{
    public class TransactionConfigService : ITransactionConfigService
    {
        #region Constructor
        private readonly ICommonRepository<ServiceMaster> _serviceMasterRepository;
        private readonly ICommonRepository<ServiceDetail> _serviceDetailRepository;
        private readonly ICommonRepository<ServiceStastics> _serviceStasticsRepository;
        private readonly ICommonRepository<ServiceProviderMaster> _ServiceProviderMaster;
        private readonly ICommonRepository<AppType> _ApptypeRepository;
        private readonly ICommonRepository<ServiceProviderType> _ProviderTypeRepository;
        private readonly ICommonRepository<ServiceProConfiguration> _ProviderConfiguration;
        private readonly ICommonRepository<DemonConfiguration> _DemonRepository;
        private readonly ICommonRepository<ServiceProviderDetail> _ProDetailRepository;
        private readonly ILogger<TransactionConfigService> _logger;
        private readonly ICommonRepository<ProductConfiguration> _productConfigRepository;
        private readonly ICommonRepository<CountryMaster> _countryMasterRepository;
        private readonly ICommonRepository<RouteConfiguration> _routeConfigRepository;
        private readonly ICommonRepository<ThirdPartyAPIConfiguration> _thirdPartyAPIRepository;
        private readonly ICommonRepository<ThirdPartyAPIResponseConfiguration> _thirdPartyAPIResRepository;
        private readonly ICommonRepository<TradePairMaster> _tradePairMasterRepository;
        private readonly ICommonRepository<TradePairDetail> _tradePairDetailRepository;
        private readonly ICommonRepository<Limits> _limitRepository;
        private readonly ICommonRepository<ServiceTypeMapping> _serviceTypeMapping;
        private readonly ICommonRepository<WalletTypeMaster> _walletTypeService;
        private readonly IWalletService _walletService;
        private readonly ICommonRepository<TradePairStastics> _tradePairStastics;
        private readonly ICommonRepository<Market> _marketRepository;
        private readonly IFrontTrnRepository _frontTrnRepository;
        private readonly IBackOfficeTrnRepository _backOfficeTrnRepository;
        private readonly IBasePage _basePage;
        private readonly ISignalRService _signalRService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly ICommonRepository<CoinListRequest> _coinListRequestRepository;
        private readonly ICommonRepository<SiteTokenRateType> _SiteTokenRateTypeRepository;
        private readonly ICommonRepository<SiteTokenMaster> _SiteTokenMasterRepository;
        private IMemoryCache _cache;
        //Rita 28-2-19 for margin trading
        private readonly ICommonRepository<ServiceMasterMargin> _serviceMasterRepositoryMargin;
        private readonly ICommonRepository<ServiceDetailMargin> _serviceDetailRepositoryMargin;
        private readonly ICommonRepository<ServiceStasticsMargin> _serviceStasticsRepositoryMargin;
        private readonly ICommonRepository<ServiceTypeMappingMargin> _serviceTypeMappingMargin;
        private readonly ICommonRepository<MarginWalletTypeMaster> _MarginwalletTypeService;
        private readonly IMarginTransactionWallet _MarginWalletService;
        private readonly ICommonRepository<TradePairMasterMargin> _tradePairMasterRepositoryMargin;
        private readonly ICommonRepository<TradePairDetailMargin> _tradePairDetailRepositoryMargin;
        private readonly ICommonRepository<MarketMargin> _marketRepositoryMargin;
        private readonly ICommonRepository<TradePairStasticsMargin> _tradePairStasticsMargin;
        private readonly ICommonRepository<SiteTokenMasterMargin> _SiteTokenMasterRepositoryMargin;
        //Darshan Dholakiya added for Arbitrage  configuration.. :04-06-2019
        private readonly ICommonRepository<TradePairMasterArbitrage> _tradePairMasterArbitrageRepository;
        private readonly ICommonRepository<ServiceMasterArbitrage> _serviceMasterArbitrageRepository;
        private readonly ICommonRepository<TradePairDetailArbitrage> _tradePairDetailsArbitrage;
        private readonly ICommonRepository<TradePairStasticsArbitrage> _tradePairStasticsArbitrage;
        private readonly ICommonRepository<ServiceProviderDetailArbitrage> _proDetailArbitrageRepository;
        private readonly ICommonRepository<ServiceProConfigurationArbitrage> _proConfigurationArbitrageRepository;
        private readonly ICommonRepository<AllowesOrderTypeArbitrage> _allowesOrderTypeArbitrageRepository;
        private readonly ICommonRepository<ServiceProviderMasterArbitrage> _ServiceProviderMasterArbitrage;
        private readonly ICommonRepository<ServiceDetailArbitrage> _serviceDetailRepositoryArbitrage;
        private readonly ICommonRepository<ServiceStasticsArbitrage> _serviceStasticsRepositoryArbitrage;
        private readonly ICommonRepository<ServiceTypeMappingArbitrage> _serviceTypeMappingArbitrage;
        private readonly ICommonRepository<ArbitrageWalletTypeMaster> _walletTypeServiceArbitrage;
        private readonly ICommonRepository<ProductConfigurationArbitrage> _productConfigRepositoryArbitrage;
        private readonly ICommonRepository<MarketArbitrage> _marketRepositoryArbitrage;
        private readonly ICommonRepository<RouteConfigurationArbitrage> _routeConfigRepositoryArbitrage;
        private readonly ICommonRepository<ServiceProviderTypeArbitrage> _ProviderTypeRepositoryArbitrage;
        private readonly ICommonRepository<LimitsArbitrage> _limitRepositoryArbitrage;
        private readonly ICommonRepository<ServiceProConfigurationArbitrage> _ProviderConfigurationArbitrage;
        private readonly ICommonRepository<ArbitrageThirdPartyAPIConfiguration> _thirdPartyAPIRepositoryArbitrage;
        private readonly ICommonRepository<DemonConfigurationArbitrage> _DemonRepositoryArbitrage;
        private readonly ICommonRepository<ArbitrageThirdPartyAPIResponseConfiguration> _thirdPartyAPIResRepositoryArbitrage;
        private readonly IWebApiRepository _webApiRepository;
        private readonly IWebApiSendRequest _webApiSendRequest;
        private readonly IGetWebRequest _getWebRequest;
        private readonly WebApiParseResponse _WebApiParseResponse;
        private readonly ICommonRepository<DepositCounterMaster> _DepositCounterMasterRepo;
        private readonly ICommonRepository<ProviderWalletTypeMapping> _ProviderWalletTypeMappingRepo;
        private readonly ICommonRepository<TrnTypeMaster> _TrnTypeMasterRepo;

        public TransactionConfigService(
            ICommonRepository<ServiceMaster> serviceMasterRepository, ICommonRepository<ServiceDetail> serviceDetailRepository,
            ICommonRepository<ServiceStastics> serviceStasticsRepository, ILogger<TransactionConfigService> logger,
            ICommonRepository<ServiceProviderMaster> ServiceProviderMaster, ICommonRepository<AppType> ApptypeRepository,
            ICommonRepository<ServiceProviderType> ProviderTypeRepository, ICommonRepository<ServiceProConfiguration> ProviderConfiguration,
            ICommonRepository<DemonConfiguration> DemonRepository, ICommonRepository<ServiceProviderDetail> ProDetailRepository,
            ICommonRepository<ProductConfiguration> productConfigRepository, ICommonRepository<CountryMaster> countryMasterRepository,
            ICommonRepository<RouteConfiguration> routeConfigRepository, ICommonRepository<ThirdPartyAPIConfiguration> thirdPartyAPIRepository,
            ICommonRepository<ThirdPartyAPIResponseConfiguration> thirdPartyAPIResRepository, ICommonRepository<TradePairMaster> tradePairMasterRepository,
            ICommonRepository<TradePairDetail> tradePairDetailRepository, ICommonRepository<Limits> limitRepository,
            ICommonRepository<ServiceTypeMapping> serviceTypeMapping, ICommonRepository<WalletTypeMaster> walletTypeService,
            IWalletService walletService, ICommonRepository<TradePairStastics> tradePairStastics, ICommonRepository<Market> marketRepository,
            IFrontTrnRepository frontTrnRepository, IBackOfficeTrnRepository backOfficeTrnRepository, IBasePage basePage,
            ISignalRService signalRService, Microsoft.Extensions.Configuration.IConfiguration configuration, ICommonRepository<ProviderWalletTypeMapping> ProviderWalletTypeMappingRepo,
            ICommonRepository<CoinListRequest> coinListRequestRepository, ICommonRepository<SiteTokenRateType> SiteTokenRateTypeRepository,
            ICommonRepository<SiteTokenMaster> SiteTokenMasterRepository, IMemoryCache Cache, ICommonRepository<ServiceMasterMargin> serviceMasterRepositoryMargin,
            ICommonRepository<ServiceDetailMargin> serviceDetailRepositoryMargin, ICommonRepository<ServiceStasticsMargin> serviceStasticsRepositoryMargin,
            ICommonRepository<ServiceTypeMappingMargin> serviceTypeMappingMargin, ICommonRepository<MarginWalletTypeMaster> MarginwalletTypeService,
            IMarginTransactionWallet MarginWalletService, ICommonRepository<TradePairMasterMargin> tradePairMasterRepositoryMargin,
            ICommonRepository<TradePairDetailMargin> tradePairDetailRepositoryMargin, ICommonRepository<MarketMargin> marketRepositoryMargin,
            ICommonRepository<TradePairStasticsMargin> tradePairStasticsMargin, WebApiParseResponse WebApiParseResponse, ICommonRepository<TrnTypeMaster> TrnTypeMasterRepo,
            ICommonRepository<SiteTokenMasterMargin> SiteTokenMasterRepositoryMargin, ICommonRepository<TradePairMasterArbitrage> tradePairMasterArbitrageRepository, ICommonRepository<ServiceMasterArbitrage> serviceMasterArbitrageRepository, ICommonRepository<TradePairDetailArbitrage> tradePairDetailsArbitrage, ICommonRepository<TradePairStasticsArbitrage> tradePairStasticsArbitrage,
            ICommonRepository<AllowesOrderTypeArbitrage> AllowesOrderTypeArbitrageRepository, IGetWebRequest getWebRequest, IWebApiSendRequest webApiSendRequest,
            ICommonRepository<ServiceProviderDetailArbitrage> ProDetailArbitrageRepository, IWebApiRepository webApiRepository, ICommonRepository<DepositCounterMaster> DepositCounterMasterRepo,
            ICommonRepository<ServiceProConfigurationArbitrage> ProConfigurationArbitrageRepository, ICommonRepository<ServiceProviderMasterArbitrage> ServiceProviderMasterArbitrage, ICommonRepository<ServiceDetailArbitrage> serviceDetailRepositoryArbitrage, ICommonRepository<ServiceStasticsArbitrage> serviceStasticsRepositoryArbitrage, ICommonRepository<ServiceTypeMappingArbitrage> serviceTypeMappingArbitrage, ICommonRepository<ArbitrageWalletTypeMaster> walletTypeServiceArbitrage, ICommonRepository<ProductConfigurationArbitrage> productConfigRepositoryArbitrage, ICommonRepository<MarketArbitrage> marketRepositoryArbitrage, ICommonRepository<RouteConfigurationArbitrage> routeConfigRepositoryArbitrage, ICommonRepository<ServiceProviderTypeArbitrage> ProviderTypeRepositoryArbitrage, ICommonRepository<LimitsArbitrage> limitRepositoryArbitrage, ICommonRepository<ServiceProConfigurationArbitrage> ProviderConfigurationArbitrage, ICommonRepository<ArbitrageThirdPartyAPIConfiguration> thirdPartyAPIRepositoryArbitrage, ICommonRepository<DemonConfigurationArbitrage> DemonRepositoryArbitrage, ICommonRepository<ArbitrageThirdPartyAPIResponseConfiguration> thirdPartyAPIResRepositoryArbitrage)
        {
            _serviceMasterRepository = serviceMasterRepository;
            _serviceDetailRepository = serviceDetailRepository;
            _serviceStasticsRepository = serviceStasticsRepository;
            _ServiceProviderMaster = ServiceProviderMaster;
            _ApptypeRepository = ApptypeRepository;
            _ProviderTypeRepository = ProviderTypeRepository;
            _ProviderConfiguration = ProviderConfiguration;
            _DemonRepository = DemonRepository;
            _ProDetailRepository = ProDetailRepository;
            _logger = logger;
            _productConfigRepository = productConfigRepository;
            _countryMasterRepository = countryMasterRepository;
            _routeConfigRepository = routeConfigRepository;
            _thirdPartyAPIRepository = thirdPartyAPIRepository;
            _thirdPartyAPIResRepository = thirdPartyAPIResRepository;
            _tradePairMasterRepository = tradePairMasterRepository;
            _tradePairDetailRepository = tradePairDetailRepository;
            _limitRepository = limitRepository;
            _serviceTypeMapping = serviceTypeMapping;
            _walletTypeService = walletTypeService;
            _walletService = walletService;
            _tradePairStastics = tradePairStastics;
            _marketRepository = marketRepository;
            _frontTrnRepository = frontTrnRepository;
            _backOfficeTrnRepository = backOfficeTrnRepository;
            _basePage = basePage;
            _signalRService = signalRService;
            _configuration = configuration;
            _coinListRequestRepository = coinListRequestRepository;
            _SiteTokenRateTypeRepository = SiteTokenRateTypeRepository;
            _SiteTokenMasterRepository = SiteTokenMasterRepository;
            _cache = Cache;
            //Rita 28-2-19 for margin trading
            _serviceMasterRepositoryMargin = serviceMasterRepositoryMargin;
            _serviceDetailRepositoryMargin = serviceDetailRepositoryMargin;
            _serviceStasticsRepositoryMargin = serviceStasticsRepositoryMargin;
            _serviceTypeMappingMargin = serviceTypeMappingMargin;
            _MarginwalletTypeService = MarginwalletTypeService;
            _MarginWalletService = MarginWalletService;
            _tradePairMasterRepositoryMargin = tradePairMasterRepositoryMargin;
            _tradePairDetailRepositoryMargin = tradePairDetailRepositoryMargin;
            _marketRepositoryMargin = marketRepositoryMargin;
            _tradePairStasticsMargin = tradePairStasticsMargin;
            _SiteTokenMasterRepositoryMargin = SiteTokenMasterRepositoryMargin;
            //Darshan Dholakiya added for Arbitrage Configuration Changes:04-06-2019
            _tradePairMasterArbitrageRepository = tradePairMasterArbitrageRepository;
            _serviceMasterArbitrageRepository = serviceMasterArbitrageRepository;
            _tradePairDetailsArbitrage = tradePairDetailsArbitrage;
            _tradePairStasticsArbitrage = tradePairStasticsArbitrage;
            _allowesOrderTypeArbitrageRepository = AllowesOrderTypeArbitrageRepository;
            _proDetailArbitrageRepository = ProDetailArbitrageRepository;
            _proConfigurationArbitrageRepository = ProConfigurationArbitrageRepository;
            _ServiceProviderMasterArbitrage = ServiceProviderMasterArbitrage;
            _serviceDetailRepositoryArbitrage = serviceDetailRepositoryArbitrage;
            _serviceStasticsRepositoryArbitrage = serviceStasticsRepositoryArbitrage;
            _serviceTypeMappingArbitrage = serviceTypeMappingArbitrage;
            _walletTypeServiceArbitrage = walletTypeServiceArbitrage;
            _productConfigRepositoryArbitrage = productConfigRepositoryArbitrage;
            _marketRepositoryArbitrage = marketRepositoryArbitrage;
            _routeConfigRepositoryArbitrage = routeConfigRepositoryArbitrage;
            _ProviderTypeRepositoryArbitrage = ProviderTypeRepositoryArbitrage;
            _limitRepositoryArbitrage = limitRepositoryArbitrage;
            _ProviderConfigurationArbitrage = ProviderConfigurationArbitrage;
            _thirdPartyAPIRepositoryArbitrage = thirdPartyAPIRepositoryArbitrage;
            _DemonRepositoryArbitrage = DemonRepositoryArbitrage;
            _thirdPartyAPIResRepositoryArbitrage = thirdPartyAPIResRepositoryArbitrage;
            _webApiRepository = webApiRepository;
            _webApiSendRequest = webApiSendRequest;
            _getWebRequest = getWebRequest;
            _WebApiParseResponse = WebApiParseResponse;
            _DepositCounterMasterRepo = DepositCounterMasterRepo;
            _ProviderWalletTypeMappingRepo = ProviderWalletTypeMappingRepo;
            _TrnTypeMasterRepo = TrnTypeMasterRepo;
        }

        #endregion

        #region Service
        public async Task<ServiceConfigurationResponse> AddServiceConfiguration(ServiceConfigurationRequestNew Request, long UserId)
        {
            ServiceConfigurationResponse Response = new ServiceConfigurationResponse();
            try
            {
                //Uday 08-01-2019 Check Coin Already available or not                
                var oldCoin = _serviceMasterRepository.GetSingle(x => x.SMSCode.ToLower() == Request.SMSCode.ToLower() || x.Name.ToLower() == Request.Name);
                if (oldCoin != null)
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.CoinAlreadyAvailable,
                        ReturnMsg = "Coin Already Available.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }
                if (Request.SMSCode.Contains(' ') || Request.SMSCode.Contains('_')) //Coin name not contains space or _
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.CoinNotContainSpecialCharacter,
                        ReturnMsg = "Coin not contain any special character.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }
                //Rushabh 13-01-2020 Added Code For Node API Call Process For New Coin Configuration At Node API Side.
                if (Request.ServiceProviderId == 0)
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.InvalidServiceProviderIdForNewCoinConfig,
                        ReturnMsg = "Invalid Service Provider.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }
                var IsProviderExist = _ServiceProviderMaster.GetActiveById(Request.ServiceProviderId);
                if (IsProviderExist == null)
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.ServiceProviderDataNotFoundForNewCoinConfig,
                        ReturnMsg = "No Data Found.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }
                #region Node API Call Process
                var CallNodeAPIForAllCoinConfig = _configuration["CallNodeAPIForAllCoinConfig"].ToUpper().ToString();
                //False Which Means Call Node API Only For PAYU Provider
                if (IsProviderExist.ProviderName.ToUpper().Equals("PAYUAPI") || CallNodeAPIForAllCoinConfig.Equals("TRUE"))
                {
                    var providerdata = await _webApiRepository.GetProviderDataListAsyncForCoinConfig(new TransactionApiConfigurationRequest { SMSCode = "", amount = 0, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.Configure_Coin) }, Request.ServiceProviderId);//05-12-2018
                    if (providerdata == null || providerdata.Count() == 0)
                    {
                        return new ServiceConfigurationResponse
                        {
                            ReturnCode = enResponseCode.Fail,
                            ErrorCode = enErrorCode.ItemNotFoundForNewCoinConfig,
                            ReturnMsg = "Please Try After Sometime.",
                            Response = new ServiceConfigurationInfo
                            {
                                ServiceId = 0
                            }
                        };
                    }
                    if (providerdata[0].ThirPartyAPIID == 0)
                    {
                        return new ServiceConfigurationResponse
                        {
                            ReturnCode = enResponseCode.Fail,
                            ErrorCode = enErrorCode.InvalidThirdPartyIdForNewCoinConfig,
                            ReturnMsg = "Please Try After Sometime.",
                            Response = new ServiceConfigurationInfo
                            {
                                ServiceId = 0
                            }
                        };
                    }
                    var thirdPartyAPIConfiguration = await _thirdPartyAPIRepository.GetByIdAsync(providerdata[0].ThirPartyAPIID);
                    if (thirdPartyAPIConfiguration == null || providerdata.Count() == 0)
                    {
                        return new ServiceConfigurationResponse
                        {
                            ReturnCode = enResponseCode.Fail,
                            ErrorCode = enErrorCode.ThirdPartyDataNotFoundForNewCoinConfig,
                            ReturnMsg = "No Data Found.",
                            Response = new ServiceConfigurationInfo
                            {
                                ServiceId = 0
                            }
                        };
                    }
                    var thirdPartyAPIRequest = _getWebRequest.MakeWebRequest(providerdata[0].RouteID, providerdata[0].ThirPartyAPIID, providerdata[0].SerProDetailID);
                    if (thirdPartyAPIRequest == null)
                    {
                        return new ServiceConfigurationResponse
                        {
                            ReturnCode = enResponseCode.Fail,
                            ErrorCode = enErrorCode.WebRequestCreationFailForNewCoinConfig,
                            ReturnMsg = "Fail To Create a Web Request.",
                            Response = new ServiceConfigurationInfo
                            {
                                ServiceId = 0
                            }
                        };
                    }
                    thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#CoinName#", Request.SMSCode);
                    string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);

                    // parse response logic 
                    WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, providerdata[0].ThirPartyAPIID);
                    if (!String.IsNullOrEmpty(ParsedResponse.StatusMsg) && !String.IsNullOrEmpty(ParsedResponse.OperatorRefNo))
                    {
                        HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, "Node API Response : " + Helpers.JsonSerialize(apiResponse));
                        if (ParsedResponse.StatusMsg != "0" && ParsedResponse.StatusMsg != "3")
                        {
                            return new ServiceConfigurationResponse
                            {
                                ReturnCode = enResponseCode.Fail,
                                ErrorCode = enErrorCode.OperatorFailForNewCoinConfig,
                                ReturnMsg = "Fail",
                                Response = new ServiceConfigurationInfo
                                {
                                    ServiceId = 0
                                }
                            };
                        }
                    }
                    else
                    {
                        return new ServiceConfigurationResponse
                        {
                            ReturnCode = enResponseCode.Fail,
                            ErrorCode = enErrorCode.ResponseParseErrorForNewCoinConfig,
                            ReturnMsg = "Please Try After Sometime.",
                            Response = new ServiceConfigurationInfo
                            {
                                ServiceId = 0
                            }
                        };
                    }
                }
                #endregion

                ServiceMaster serviceMaster = new ServiceMaster()
                {
                    Name = Request.Name,
                    SMSCode = Request.SMSCode,
                    //ServiceType = Request.Type,
                    Status = Request.Status,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };
                var newServiceMaster = _serviceMasterRepository.Add(serviceMaster);

                ServiceDetailJsonData serviceDetailJsonData = new ServiceDetailJsonData()
                {
                    //ImageUrl = Request.ImageUrl,
                    TotalSupply = Request.TotalSupply,
                    MaxSupply = Request.MaxSupply,
                    //ProofType = Request.ProofType,
                    Community = Request.Community,
                    Explorer = Request.Explorer,
                    //EncryptionAlgorithm = Request.EncryptionAlgorithm,
                    WebsiteUrl = Request.WebsiteUrl,
                    //WhitePaperPath = Request.WhitePaperPath,
                    Introduction = Request.Introduction,

                };

                ServiceDetail serviceDetail = new ServiceDetail()
                {
                    ServiceId = newServiceMaster.Id,
                    Status = Request.Status,//rita 2-5-19 pass as active
                    ServiceDetailJson = JsonConvert.SerializeObject(serviceDetailJsonData)
                };
                var newServiceDetail = _serviceDetailRepository.Add(serviceDetail);

                ServiceStastics serviceStastics = new ServiceStastics()
                {
                    ServiceId = newServiceMaster.Id,
                    IssueDate = Request.IssueDate,
                    IssuePrice = Request.IssuePrice,
                    MaxSupply = Request.MaxSupply,
                    CirculatingSupply = Request.CirculatingSupply,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };
                var newServiceStastics = _serviceStasticsRepository.Add(serviceStastics);

                var depositSerMapping = new ServiceTypeMapping
                {
                    ServiceId = newServiceMaster.Id,
                    TrnType = Convert.ToInt16(enTrnType.Deposit),
                    Status = Request.IsDeposit,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };
                var newdepositSerMapping = _serviceTypeMapping.Add(depositSerMapping);

                var withdrawSerMapping = new ServiceTypeMapping
                {
                    ServiceId = newServiceMaster.Id,
                    TrnType = Convert.ToInt16(enTrnType.Withdraw),
                    Status = Request.IsWithdraw,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };
                var newwithdrawSerMapping = _serviceTypeMapping.Add(withdrawSerMapping);

                var tranSerMapping = new ServiceTypeMapping
                {
                    ServiceId = newServiceMaster.Id,
                    TrnType = Convert.ToInt16(enTrnType.Transaction),
                    Status = Request.IsTransaction,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };
                var newtranSerMapping = _serviceTypeMapping.Add(tranSerMapping);

                //Add Default WalletType Master
                var walletTypeMaster = new WalletTypeMaster
                {
                    WalletTypeName = Request.SMSCode,
                    Description = Request.SMSCode,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    IsDepositionAllow = Request.IsDeposit,
                    IsWithdrawalAllow = Request.IsWithdraw,
                    IsTransactionWallet = Request.IsTransaction,
                    CurrencyTypeID = Request.CurrencyTypeId,
                    Rate = Request.Rate,
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST(),
                    IsLocal = 8,
                    IsDefaultWallet = 0,
                    ConfirmationCount = 3
                };
                var newwalletTypeMaster = _walletTypeService.Add(walletTypeMaster);

                //Update WalletTypeId In ServiceMaster
                newServiceMaster.WalletTypeID = newwalletTypeMaster.Id;
                _serviceMasterRepository.Update(newServiceMaster);

                ////Add Into WalletMaster For Default Organization
                int[] AllowTrnType = new int[3] { Convert.ToInt16(enTrnType.Deposit), Convert.ToInt16(enTrnType.Withdraw), Convert.ToInt16(enTrnType.Transaction) };
                var walletMaster = await _walletService.InsertIntoWalletMaster(" Default Org " + Request.SMSCode, Request.SMSCode, 1, AllowTrnType, UserId, null, 1);
                _walletService.CreateWalletForAllUser_NewService(Request.SMSCode);

                //Add BaseCurrency In MarketEntity
                if (Request.IsBaseCurrency == 1)
                {
                    var marketViewModel = new MarketViewModel { CurrencyName = Request.SMSCode, isBaseCurrency = 1, ServiceID = newServiceMaster.Id };
                    AddMarketData(marketViewModel, UserId);
                }
                //komal 3-04-2019 single entry with country =1
                var Product = _productConfigRepository.Add(new ProductConfiguration() { Status = 1, CountryID = 1, CreatedBy = 1, CreatedDate = DateTime.UtcNow, ProductName = serviceMaster.SMSCode, ServiceID = serviceMaster.Id });

                //Rushabh 15-01-2020 Add Entry In Deposit Counter Master If Allow Deposition = True

                #region Add Entry In DepositCounterMaster & ProviderWalletTypeMapping
                ProviderWalletTypeMapping ProWalletTypeMapping = new ProviderWalletTypeMapping
                {
                    ServiceProviderId = IsProviderExist.Id,
                    WalletTypeId = newwalletTypeMaster.Id,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST()
                };
                var ProWalletTypeMappingobj = await _ProviderWalletTypeMappingRepo.AddAsync(ProWalletTypeMapping);

                if (Request.IsDeposit == 1)
                {
                    DepositCounterMaster NewObj = new DepositCounterMaster
                    {
                        AppType = Convert.ToInt16(enAppType.RestAPI),
                        LastTrnID = "",
                        PreviousTrnID = "",
                        WalletTypeID = newwalletTypeMaster.Id,
                        Status = Convert.ToInt16(ServiceStatus.Active),
                        Limit = 0,
                        prevIterationID = "",
                        MaxLimit = 100,
                        TPSPickupStatus = 0,
                        RecordCount = 100,
                        SerProId = IsProviderExist.Id,
                        StartTime = 0,
                        EndTime = 0,
                        FlushAddressEnable = 0,
                        CreatedBy = UserId,
                        CreatedDate = _basePage.UTC_To_IST()
                    };
                    var DepositCntMasterObj = await _DepositCounterMasterRepo.AddAsync(NewObj);
                }
                #endregion

                return new ServiceConfigurationResponse { Response = new ServiceConfigurationInfo { ServiceId = newServiceMaster.Id }, ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Service Added Successfully." };
                //return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 28-2-19 for Margin trading
        public async Task<ServiceConfigurationResponse> AddServiceConfigurationMargin(ServiceConfigurationRequestNew Request, long UserId)
        {
            ServiceConfigurationResponse Response = new ServiceConfigurationResponse();
            try
            {
                //Uday 08-01-2019 Check Coin Already available or not
                var oldCoin = _serviceMasterRepositoryMargin.GetSingle(x => x.SMSCode.ToLower() == Request.SMSCode.ToLower() || x.Name.ToLower() == Request.Name);
                if (oldCoin != null)
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.CoinAlreadyAvailable,
                        ReturnMsg = "Coin Already Available.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }
                if (Request.SMSCode.Contains(' ') || Request.SMSCode.Contains('_')) //Coin name not contains space or _
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.CoinNotContainSpecialCharacter,
                        ReturnMsg = "Coin not contain any special character.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }

                //Rushabh 13-01-2020 Added Code For Node API Call Process For New Coin Configuration At Node API Side.
                if (Request.ServiceProviderId == 0)
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.InvalidServiceProviderIdForNewCoinConfig,
                        ReturnMsg = "Invalid Service Provider.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }
                var IsProviderExist = await _ServiceProviderMaster.FindByAsync(i => i.Id == Request.ServiceProviderId && i.Status == 1);
                if (IsProviderExist == null)
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.ServiceProviderDataNotFoundForNewCoinConfig,
                        ReturnMsg = "No Data Found.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }

                #region Node API Call Process
                var providerdata = await _webApiRepository.GetProviderDataListAsyncForCoinConfig(new TransactionApiConfigurationRequest { SMSCode = "", amount = 0, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.Configure_Coin) }, Request.ServiceProviderId);//05-12-2018

                if (providerdata == null || providerdata.Count() == 0)
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.ItemNotFoundForNewCoinConfig,
                        ReturnMsg = "Please Try After Sometime.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }
                if (providerdata[0].ThirPartyAPIID == 0)
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.InvalidThirdPartyIdForNewCoinConfig,
                        ReturnMsg = "Please Try After Sometime.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }
                var thirdPartyAPIConfiguration = await _thirdPartyAPIRepository.GetByIdAsync(providerdata[0].ThirPartyAPIID);
                if (thirdPartyAPIConfiguration == null || providerdata.Count() == 0)
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.ThirdPartyDataNotFoundForNewCoinConfig,
                        ReturnMsg = "No Data Found.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }
                var thirdPartyAPIRequest = _getWebRequest.MakeWebRequest(providerdata[0].RouteID, providerdata[0].ThirPartyAPIID, providerdata[0].SerProDetailID);
                if (thirdPartyAPIRequest == null)
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.WebRequestCreationFailForNewCoinConfig,
                        ReturnMsg = "Fail To Create a Web Request.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }
                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#CoinName#", Request.SMSCode);
                string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);

                // parse response logic 
                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, providerdata[0].ThirPartyAPIID);
                if (!String.IsNullOrEmpty(ParsedResponse.StatusMsg) && !String.IsNullOrEmpty(ParsedResponse.OperatorRefNo))
                {
                    if (ParsedResponse.Status != enTransactionStatus.Success && ParsedResponse.StatusMsg != "0")
                    {
                        return new ServiceConfigurationResponse
                        {
                            ReturnCode = enResponseCode.Fail,
                            ErrorCode = enErrorCode.OperatorFailForNewCoinConfig,
                            ReturnMsg = ParsedResponse.OperatorRefNo.ToString(),
                            Response = new ServiceConfigurationInfo
                            {
                                ServiceId = 0
                            }
                        };
                    }
                }
                else
                {
                    return new ServiceConfigurationResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.ResponseParseErrorForNewCoinConfig,
                        ReturnMsg = "Please Try After Sometime.",
                        Response = new ServiceConfigurationInfo
                        {
                            ServiceId = 0
                        }
                    };
                }

                #endregion

                ServiceMasterMargin serviceMaster = new ServiceMasterMargin()
                {
                    Name = Request.Name,
                    SMSCode = Request.SMSCode,
                    //ServiceType = Request.Type,
                    Status = Request.Status,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };
                var newServiceMaster = _serviceMasterRepositoryMargin.Add(serviceMaster);

                ServiceDetailJsonData serviceDetailJsonData = new ServiceDetailJsonData()//Take same class as common json field only data Diff
                {
                    //ImageUrl = Request.ImageUrl,
                    TotalSupply = Request.TotalSupply,
                    MaxSupply = Request.MaxSupply,
                    //ProofType = Request.ProofType,
                    Community = Request.Community,
                    Explorer = Request.Explorer,
                    //EncryptionAlgorithm = Request.EncryptionAlgorithm,
                    WebsiteUrl = Request.WebsiteUrl,
                    //WhitePaperPath = Request.WhitePaperPath,
                    Introduction = Request.Introduction,

                };

                ServiceDetailMargin serviceDetail = new ServiceDetailMargin()
                {
                    ServiceId = newServiceMaster.Id,
                    Status = Request.Status,//rita 2-5-19 pass as active
                    ServiceDetailJson = JsonConvert.SerializeObject(serviceDetailJsonData)
                };
                var newServiceDetail = _serviceDetailRepositoryMargin.Add(serviceDetail);

                ServiceStasticsMargin serviceStastics = new ServiceStasticsMargin()
                {
                    ServiceId = newServiceMaster.Id,
                    IssueDate = Request.IssueDate,
                    IssuePrice = Request.IssuePrice,
                    MaxSupply = Request.MaxSupply,
                    CirculatingSupply = Request.CirculatingSupply,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };
                var newServiceStastics = _serviceStasticsRepositoryMargin.Add(serviceStastics);

                var depositSerMapping = new ServiceTypeMappingMargin
                {
                    ServiceId = newServiceMaster.Id,
                    TrnType = Convert.ToInt16(enTrnType.Deposit),
                    Status = Request.IsDeposit,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };
                var newdepositSerMapping = _serviceTypeMappingMargin.Add(depositSerMapping);

                var withdrawSerMapping = new ServiceTypeMappingMargin
                {
                    ServiceId = newServiceMaster.Id,
                    TrnType = Convert.ToInt16(enTrnType.Withdraw),
                    Status = Request.IsWithdraw,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };
                var newwithdrawSerMapping = _serviceTypeMappingMargin.Add(withdrawSerMapping);

                var tranSerMapping = new ServiceTypeMappingMargin
                {
                    ServiceId = newServiceMaster.Id,
                    TrnType = Convert.ToInt16(enTrnType.Transaction),
                    Status = Request.IsTransaction,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };
                var newtranSerMapping = _serviceTypeMappingMargin.Add(tranSerMapping);

                //Add Default WalletType Master
                var walletTypeMaster = new MarginWalletTypeMaster
                {
                    WalletTypeName = Request.SMSCode,
                    Description = Request.SMSCode,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    IsDepositionAllow = Request.IsDeposit,
                    IsWithdrawalAllow = Request.IsWithdraw,
                    IsTransactionWallet = Request.IsTransaction,
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST()
                };
                var newwalletTypeMaster = _MarginwalletTypeService.Add(walletTypeMaster);

                //Update WalletTypeId In ServiceMaster
                newServiceMaster.WalletTypeID = newwalletTypeMaster.Id;
                _serviceMasterRepositoryMargin.Update(newServiceMaster);

                ////Add Into WalletMaster For Default Organization
                int[] AllowTrnType = new int[3] { Convert.ToInt16(enTrnType.Deposit), Convert.ToInt16(enTrnType.Withdraw), Convert.ToInt16(enTrnType.Transaction) };

                //Add BaseCurrency In MarketEntity
                if (Request.IsBaseCurrency == 1)
                {
                    var marketViewModel = new MarketViewModel { CurrencyName = Request.SMSCode, isBaseCurrency = 1, ServiceID = newServiceMaster.Id };
                    AddMarketData(marketViewModel, UserId);
                }
                return new ServiceConfigurationResponse { Response = new ServiceConfigurationInfo { ServiceId = newServiceMaster.Id }, ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Service Added Successfully." };
                //return newServiceMaster.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long UpdateServiceConfiguration(ServiceConfigurationRequest Request, long UserId)
        {
            try
            {
                //Uday 08-01-2019 SMSCode is not same as old coin
                var oldCoin = _serviceMasterRepository.GetSingle(x => (x.SMSCode.ToLower() == Request.SMSCode.ToLower() || x.Name.ToLower() == Request.Name.ToLower()) && x.Id != Request.ServiceId);
                if (oldCoin != null)
                {
                    return -1;
                }
                if (Request.SMSCode.Contains(' ') || Request.SMSCode.Contains('_')) //Coin name not contains space or _
                {
                    return -2;
                }

                string OldSMSCode = "";
                var serviceMaster = _serviceMasterRepository.GetById(Request.ServiceId);
                if (serviceMaster != null)
                {
                    OldSMSCode = serviceMaster.SMSCode;

                    serviceMaster.Name = Request.Name;
                    serviceMaster.SMSCode = Request.SMSCode;
                    // serviceMaster.ServiceType = Request.Type;
                    serviceMaster.UpdatedBy = UserId;
                    serviceMaster.UpdatedDate = _basePage.UTC_To_IST();
                    serviceMaster.Status = Request.Status;

                    _serviceMasterRepository.Update(serviceMaster);

                    ServiceDetailJsonData serviceDetailJsonData = new ServiceDetailJsonData()
                    {
                        //ImageUrl = Request.ImageUrl,
                        TotalSupply = Request.TotalSupply,
                        MaxSupply = Request.MaxSupply,
                        //ProofType = Request.ProofType,
                        Community = Request.Community,
                        Explorer = Request.Explorer,
                        //EncryptionAlgorithm = Request.EncryptionAlgorithm,
                        WebsiteUrl = Request.WebsiteUrl,
                        //WhitePaperPath = Request.WebsiteUrl,
                        Introduction = Request.Introduction
                    };

                    var serviceDetail = _serviceDetailRepository.GetSingle(service => service.ServiceId == Request.ServiceId);
                    serviceDetail.ServiceDetailJson = JsonConvert.SerializeObject(serviceDetailJsonData);
                    _serviceDetailRepository.Update(serviceDetail);

                    var serviceStastics = _serviceStasticsRepository.GetSingle(service => service.ServiceId == Request.ServiceId);
                    serviceStastics.IssueDate = Request.IssueDate;
                    serviceStastics.IssuePrice = Request.IssuePrice;
                    serviceStastics.MaxSupply = Request.MaxSupply;
                    serviceStastics.CirculatingSupply = Request.CirculatingSupply;
                    serviceStastics.UpdatedDate = _basePage.UTC_To_IST();
                    serviceStastics.UpdatedBy = UserId;
                    _serviceStasticsRepository.Update(serviceStastics);

                    var depositSerMapping = _serviceTypeMapping.GetSingle(x => x.ServiceId == Request.ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Deposit));
                    if (depositSerMapping != null)
                    {
                        depositSerMapping.Status = Request.IsDeposit;
                        depositSerMapping.UpdatedBy = UserId;
                        depositSerMapping.UpdatedDate = _basePage.UTC_To_IST();
                        _serviceTypeMapping.Update(depositSerMapping);
                    }

                    var withdrawSerMapping = _serviceTypeMapping.GetSingle(x => x.ServiceId == Request.ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Withdraw));
                    if (withdrawSerMapping != null)
                    {
                        withdrawSerMapping.Status = Request.IsWithdraw;
                        withdrawSerMapping.UpdatedBy = UserId;
                        withdrawSerMapping.UpdatedDate = _basePage.UTC_To_IST();
                        _serviceTypeMapping.Update(withdrawSerMapping);
                    }

                    var tranSerMapping = _serviceTypeMapping.GetSingle(x => x.ServiceId == Request.ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Transaction));
                    if (tranSerMapping != null)
                    {
                        tranSerMapping.Status = Request.IsTransaction;
                        tranSerMapping.UpdatedBy = UserId;
                        tranSerMapping.UpdatedDate = _basePage.UTC_To_IST();
                        _serviceTypeMapping.Update(tranSerMapping);
                    }

                    var walletType = _walletTypeService.GetById(serviceMaster.WalletTypeID);
                    if (walletType != null)
                    {
                        walletType.IsDepositionAllow = Request.IsDeposit;
                        walletType.IsWithdrawalAllow = Request.IsWithdraw;
                        walletType.IsTransactionWallet = Request.IsTransaction;
                        walletType.CurrencyTypeID = Request.CurrencyTypeId;
                        walletType.Rate = Request.Rate;

                        tranSerMapping.UpdatedBy = UserId;
                        tranSerMapping.UpdatedDate = _basePage.UTC_To_IST();

                        _walletTypeService.Update(walletType);
                    }
                    var BaseCur = _marketRepository.GetSingle(e => e.ServiceID == Request.ServiceId);
                    if (BaseCur != null)
                    {
                        BaseCur.Status = Request.IsBaseCurrency;
                        BaseCur.UpdatedBy = UserId;
                        _marketRepository.Update(BaseCur);
                    }
                    //Check For SMSCode If Change Than Replace Name In Pair
                    if (OldSMSCode.Equals(Request.SMSCode) == false)
                    {
                        //Change In Pair Name Whre Coin Is As BaseCurrency

                        var BasePairData = _tradePairMasterRepository.FindBy(x => x.BaseCurrencyId == serviceMaster.Id).ToList();

                        if (BasePairData.Count > 0)
                        {
                            foreach (var Pair in BasePairData)
                            {
                                var secondCurrency = _serviceMasterRepository.GetById(Pair.SecondaryCurrencyId);
                                Pair.PairName = secondCurrency.SMSCode + "_" + serviceMaster.SMSCode;

                                _tradePairMasterRepository.Update(Pair);
                            }
                        }
                        //Change In Pair Name Whre Coin Is As BaseCurrency

                        var SecondPairData = _tradePairMasterRepository.FindBy(x => x.SecondaryCurrencyId == serviceMaster.Id).ToList();

                        if (SecondPairData.Count > 0)
                        {
                            foreach (var Pair1 in SecondPairData)
                            {
                                var baseCurrency = _serviceMasterRepository.GetById(Pair1.BaseCurrencyId);
                                Pair1.PairName = serviceMaster.SMSCode + "_" + baseCurrency.SMSCode;

                                _tradePairMasterRepository.Update(Pair1);
                            }
                        }

                        //Update WalletType Master WalletType Name Laster On Call WalletService

                        var walletTypeMaster = _walletTypeService.GetSingle(x => x.WalletTypeName == OldSMSCode);
                        if (walletTypeMaster != null)
                        {
                            walletTypeMaster.WalletTypeName = Request.SMSCode;
                            _walletTypeService.Update(walletTypeMaster);
                        }

                    }

                    return Request.ServiceId;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rita 28-2-19 for Margin trading
        public long UpdateServiceConfigurationMargin(ServiceConfigurationRequest Request, long UserId)
        {
            try
            {
                //Uday 08-01-2019 SMSCode is not same as old coin
                var oldCoin = _serviceMasterRepositoryMargin.GetSingle(x => (x.SMSCode.ToLower() == Request.SMSCode.ToLower() || x.Name.ToLower() == Request.Name.ToLower()) && x.Id != Request.ServiceId);
                if (oldCoin != null)
                {
                    return -1;
                }
                if (Request.SMSCode.Contains(' ') || Request.SMSCode.Contains('_')) //Coin name not contains space or _
                {
                    return -2;
                }

                string OldSMSCode = "";
                var serviceMaster = _serviceMasterRepositoryMargin.GetById(Request.ServiceId);
                if (serviceMaster != null)
                {
                    OldSMSCode = serviceMaster.SMSCode;

                    serviceMaster.Name = Request.Name;
                    serviceMaster.SMSCode = Request.SMSCode;
                    // serviceMaster.ServiceType = Request.Type;
                    serviceMaster.UpdatedBy = UserId;
                    serviceMaster.UpdatedDate = _basePage.UTC_To_IST();
                    serviceMaster.Status = Request.Status;

                    _serviceMasterRepositoryMargin.Update(serviceMaster);

                    ServiceDetailJsonData serviceDetailJsonData = new ServiceDetailJsonData()
                    {
                        //ImageUrl = Request.ImageUrl,
                        TotalSupply = Request.TotalSupply,
                        MaxSupply = Request.MaxSupply,
                        //ProofType = Request.ProofType,
                        Community = Request.Community,
                        Explorer = Request.Explorer,
                        //EncryptionAlgorithm = Request.EncryptionAlgorithm,
                        WebsiteUrl = Request.WebsiteUrl,
                        //WhitePaperPath = Request.WebsiteUrl,
                        Introduction = Request.Introduction
                    };

                    var serviceDetail = _serviceDetailRepositoryMargin.GetSingle(service => service.ServiceId == Request.ServiceId);
                    serviceDetail.ServiceDetailJson = JsonConvert.SerializeObject(serviceDetailJsonData);
                    _serviceDetailRepositoryMargin.Update(serviceDetail);

                    var serviceStastics = _serviceStasticsRepositoryMargin.GetSingle(service => service.ServiceId == Request.ServiceId);
                    serviceStastics.IssueDate = Request.IssueDate;
                    serviceStastics.IssuePrice = Request.IssuePrice;
                    serviceStastics.MaxSupply = Request.MaxSupply;
                    serviceStastics.CirculatingSupply = Request.CirculatingSupply;
                    serviceStastics.UpdatedDate = _basePage.UTC_To_IST();
                    serviceStastics.UpdatedBy = UserId;
                    _serviceStasticsRepositoryMargin.Update(serviceStastics);

                    var depositSerMapping = _serviceTypeMapping.GetSingle(x => x.ServiceId == Request.ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Deposit));
                    if (depositSerMapping != null)
                    {
                        depositSerMapping.Status = Request.IsDeposit;
                        depositSerMapping.UpdatedBy = UserId;
                        depositSerMapping.UpdatedDate = _basePage.UTC_To_IST();
                        _serviceTypeMapping.Update(depositSerMapping);
                    }

                    var withdrawSerMapping = _serviceTypeMapping.GetSingle(x => x.ServiceId == Request.ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Withdraw));
                    if (withdrawSerMapping != null)
                    {
                        withdrawSerMapping.Status = Request.IsWithdraw;
                        withdrawSerMapping.UpdatedBy = UserId;
                        withdrawSerMapping.UpdatedDate = _basePage.UTC_To_IST();
                        _serviceTypeMapping.Update(withdrawSerMapping);
                    }

                    var tranSerMapping = _serviceTypeMapping.GetSingle(x => x.ServiceId == Request.ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Transaction));
                    if (tranSerMapping != null)
                    {
                        tranSerMapping.Status = Request.IsTransaction;
                        tranSerMapping.UpdatedBy = UserId;
                        tranSerMapping.UpdatedDate = _basePage.UTC_To_IST();
                        _serviceTypeMapping.Update(tranSerMapping);
                    }

                    var walletType = _MarginwalletTypeService.GetById(serviceMaster.WalletTypeID);
                    if (walletType != null)
                    {
                        walletType.IsDepositionAllow = Request.IsDeposit;
                        walletType.IsWithdrawalAllow = Request.IsWithdraw;
                        walletType.IsTransactionWallet = Request.IsTransaction;
                        tranSerMapping.UpdatedBy = UserId;
                        tranSerMapping.UpdatedDate = _basePage.UTC_To_IST();

                        _MarginwalletTypeService.Update(walletType);
                    }
                    var BaseCur = _marketRepositoryMargin.GetSingle(e => e.ServiceID == Request.ServiceId);
                    if (BaseCur != null)
                    {
                        BaseCur.Status = Request.IsBaseCurrency;
                        BaseCur.UpdatedBy = UserId;
                        _marketRepositoryMargin.Update(BaseCur);
                    }
                    //Check For SMSCode If Change Than Replace Name In Pair
                    if (OldSMSCode.Equals(Request.SMSCode) == false)
                    {
                        //Change In Pair Name Whre Coin Is As BaseCurrency
                        var BasePairData = _tradePairMasterRepositoryMargin.FindBy(x => x.BaseCurrencyId == serviceMaster.Id).ToList();

                        if (BasePairData.Count > 0)
                        {
                            foreach (var Pair in BasePairData)
                            {
                                var secondCurrency = _serviceMasterRepository.GetById(Pair.SecondaryCurrencyId);
                                Pair.PairName = secondCurrency.SMSCode + "_" + serviceMaster.SMSCode;
                                _tradePairMasterRepositoryMargin.Update(Pair);
                            }

                        }
                        //Change In Pair Name Whre Coin Is As BaseCurrency
                        var SecondPairData = _tradePairMasterRepositoryMargin.FindBy(x => x.SecondaryCurrencyId == serviceMaster.Id).ToList();
                        if (SecondPairData.Count > 0)
                        {
                            foreach (var Pair1 in SecondPairData)
                            {
                                var baseCurrency = _serviceMasterRepository.GetById(Pair1.BaseCurrencyId);
                                Pair1.PairName = serviceMaster.SMSCode + "_" + baseCurrency.SMSCode;
                                _tradePairMasterRepositoryMargin.Update(Pair1);
                            }
                        }
                        //Update WalletType Master WalletType Name Laster On Call WalletService
                        var walletTypeMaster = _MarginwalletTypeService.GetSingle(x => x.WalletTypeName == OldSMSCode);
                        if (walletTypeMaster != null)
                        {
                            walletTypeMaster.WalletTypeName = Request.SMSCode;
                            _MarginwalletTypeService.Update(walletTypeMaster);
                        }
                    }
                    return Request.ServiceId;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ServiceConfigurationRequest> GetAllServiceConfiguration(int StatusData = 0, short IsMargin = 0, long CurrencyTypeId = 999)//Rita 5-3-19 for Margin Trading
        {
            List<ServiceConfigurationRequest> responsedata;
            try
            {
                responsedata = new List<ServiceConfigurationRequest>();
                List<ServiceMasterResponse> ServiceData = new List<ServiceMasterResponse>();
                if (IsMargin == 1)
                {
                    if (StatusData == 0)
                    {
                        ServiceData = _frontTrnRepository.GetAllServiceConfigurationMargin();
                    }
                    else
                    {
                        ServiceData = _frontTrnRepository.GetAllServiceConfigurationMargin(StatusData);
                    }
                }
                else
                {
                    if (StatusData == 0)
                    {
                        ServiceData = _frontTrnRepository.GetAllServiceConfiguration(0, CurrencyTypeId);
                    }
                    else
                    {
                        ServiceData = _frontTrnRepository.GetAllServiceConfiguration(StatusData, CurrencyTypeId);
                    }
                }
                if (ServiceData != null && ServiceData.Count > 0)
                {
                    foreach (var service in ServiceData)
                    {
                        ServiceConfigurationRequest response = new ServiceConfigurationRequest();
                        response.ServiceId = service.ServiceId;
                        response.Name = service.ServiceName;
                        response.SMSCode = service.SMSCode;
                        //response.Type = service.ServiceType;
                        response.StatusText = Enum.GetName(typeof(ServiceStatus), service.Status);
                        response.Status = service.Status;
                        response.Rate = service.Rate;
                        response.CurrencyTypeId = service.CurrencyTypeId;

                        //var serviceDetail = _serviceDetailRepository.GetSingle(ser => ser.ServiceId == service.Id);
                        var serviceDetailJson = JsonConvert.DeserializeObject<ServiceDetailJsonData>(service.ServiceDetailJson);

                        //response.ImageUrl = serviceDetailJson.ImageUrl;
                        response.TotalSupply = serviceDetailJson.TotalSupply;
                        response.MaxSupply = serviceDetailJson.MaxSupply;
                        //response.ProofType = serviceDetailJson.ProofType;
                        //response.EncryptionAlgorithm = serviceDetailJson.EncryptionAlgorithm;
                        response.WebsiteUrl = serviceDetailJson.WebsiteUrl;
                        response.Explorer = serviceDetailJson.Explorer;
                        response.Community = serviceDetailJson.Community;
                        //response.WhitePaperPath = serviceDetailJson.WhitePaperPath;
                        response.Introduction = serviceDetailJson.Introduction;

                        response.CirculatingSupply = service.CirculatingSupply;
                        response.IssueDate = service.IssueDate;
                        response.IssuePrice = service.IssuePrice;


                        var IsBaseCur = _marketRepository.GetSingle(e => e.ServiceID == service.ServiceId);
                        if (IsBaseCur != null)
                        {
                            response.IsBaseCurrency = IsBaseCur.Status;
                        }
                        response.IsDeposit = service.DepositBit;
                        response.IsWithdraw = service.WithdrawBit;
                        response.IsTransaction = service.TransactionBit;
                        response.WalletTypeID = service.WalletTypeID;
                        response.IsOnlyIntAmountAllow = service.IsOnlyIntAmountAllow;
                        responsedata.Add(response);
                    }
                    return responsedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ServiceConfigurationRequest GetServiceConfiguration(long ServiceId)
        {
            ServiceConfigurationRequest responsedata;
            try
            {
                responsedata = new ServiceConfigurationRequest();
                var serviceMaster = _serviceMasterRepository.GetById(ServiceId);
                if (serviceMaster != null)
                {
                    responsedata.ServiceId = serviceMaster.Id;
                    responsedata.Name = serviceMaster.Name;
                    responsedata.SMSCode = serviceMaster.SMSCode;
                    responsedata.StatusText = Enum.GetName(typeof(ServiceStatus), serviceMaster.Status);
                    responsedata.Status = serviceMaster.Status;
                    // responsedata.Type = serviceMaster.ServiceType;

                    var serviceDetail = _serviceDetailRepository.GetSingle(service => service.ServiceId == ServiceId);
                    var serviceDetailJson = JsonConvert.DeserializeObject<ServiceDetailJsonData>(serviceDetail.ServiceDetailJson);

                    // responsedata.ImageUrl = serviceDetailJson.ImageUrl;
                    responsedata.TotalSupply = serviceDetailJson.TotalSupply;
                    responsedata.MaxSupply = serviceDetailJson.MaxSupply;
                    //responsedata.ProofType = serviceDetailJson.ProofType;
                    //responsedata.EncryptionAlgorithm = serviceDetailJson.EncryptionAlgorithm;
                    responsedata.WebsiteUrl = serviceDetailJson.WebsiteUrl;
                    responsedata.Explorer = serviceDetailJson.Explorer;
                    responsedata.Community = serviceDetailJson.Community;
                    //responsedata.WhitePaperPath = serviceDetailJson.WhitePaperPath;
                    responsedata.Introduction = serviceDetailJson.Introduction;

                    var serviceStastics = _serviceStasticsRepository.GetSingle(service => service.ServiceId == ServiceId);
                    responsedata.CirculatingSupply = serviceStastics.CirculatingSupply;
                    responsedata.IssueDate = serviceStastics.IssueDate;
                    responsedata.IssuePrice = serviceStastics.IssuePrice;

                    var depositSerMapping = _serviceTypeMapping.GetSingle(x => x.ServiceId == ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Deposit));
                    if (depositSerMapping != null)
                    {
                        responsedata.IsDeposit = depositSerMapping.Status;
                    }

                    var withdrawSerMapping = _serviceTypeMapping.GetSingle(x => x.ServiceId == ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Withdraw));
                    if (withdrawSerMapping != null)
                    {
                        responsedata.IsWithdraw = withdrawSerMapping.Status;
                    }

                    var tranSerMapping = _serviceTypeMapping.GetSingle(x => x.ServiceId == ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Transaction));
                    if (tranSerMapping != null)
                    {
                        responsedata.IsTransaction = tranSerMapping.Status;
                    }
                    var IsBaseCur = _marketRepository.GetSingle(e => e.ServiceID == ServiceId);
                    if (IsBaseCur != null)
                    {
                        responsedata.IsBaseCurrency = IsBaseCur.Status;
                    }
                    return responsedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 5-3-19 for Margin trading
        public ServiceConfigurationRequest GetServiceConfigurationMargin(long ServiceId)
        {
            ServiceConfigurationRequest responsedata;
            try
            {
                responsedata = new ServiceConfigurationRequest();
                var serviceMaster = _serviceMasterRepositoryMargin.GetById(ServiceId);
                if (serviceMaster != null)
                {
                    responsedata.ServiceId = serviceMaster.Id;
                    responsedata.Name = serviceMaster.Name;
                    responsedata.SMSCode = serviceMaster.SMSCode;
                    responsedata.StatusText = Enum.GetName(typeof(ServiceStatus), serviceMaster.Status);
                    responsedata.Status = serviceMaster.Status;

                    var serviceDetail = _serviceDetailRepositoryMargin.GetSingle(service => service.ServiceId == ServiceId);
                    var serviceDetailJson = JsonConvert.DeserializeObject<ServiceDetailJsonData>(serviceDetail.ServiceDetailJson);

                    responsedata.TotalSupply = serviceDetailJson.TotalSupply;
                    responsedata.MaxSupply = serviceDetailJson.MaxSupply;
                    responsedata.WebsiteUrl = serviceDetailJson.WebsiteUrl;
                    responsedata.Explorer = serviceDetailJson.Explorer;
                    responsedata.Community = serviceDetailJson.Community;
                    responsedata.Introduction = serviceDetailJson.Introduction;

                    var serviceStastics = _serviceStasticsRepositoryMargin.GetSingle(service => service.ServiceId == ServiceId);
                    responsedata.CirculatingSupply = serviceStastics.CirculatingSupply;
                    responsedata.IssueDate = serviceStastics.IssueDate;
                    responsedata.IssuePrice = serviceStastics.IssuePrice;

                    var depositSerMapping = _serviceTypeMappingMargin.GetSingle(x => x.ServiceId == ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Deposit));
                    if (depositSerMapping != null)
                    {
                        responsedata.IsDeposit = depositSerMapping.Status;
                    }

                    var withdrawSerMapping = _serviceTypeMappingMargin.GetSingle(x => x.ServiceId == ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Withdraw));
                    if (withdrawSerMapping != null)
                    {
                        responsedata.IsWithdraw = withdrawSerMapping.Status;
                    }

                    var tranSerMapping = _serviceTypeMappingMargin.GetSingle(x => x.ServiceId == ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Transaction));
                    if (tranSerMapping != null)
                    {
                        responsedata.IsTransaction = tranSerMapping.Status;
                    }
                    var IsBaseCur = _marketRepositoryMargin.GetSingle(e => e.ServiceID == ServiceId);
                    if (IsBaseCur != null)
                    {
                        responsedata.IsBaseCurrency = IsBaseCur.Status;
                    }
                    return responsedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int SetActiveService(long ServiceId)
        {
            try
            {
                var servicedata = _serviceMasterRepository.GetById(ServiceId);
                if (servicedata != null)
                {
                    servicedata.SetActiveService();
                    _serviceMasterRepository.Update(servicedata);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 5-3-19 for Margin trading
        public int SetActiveServiceMargin(long ServiceId)
        {
            try
            {
                var servicedata = _serviceMasterRepository.GetById(ServiceId);
                if (servicedata != null)
                {
                    servicedata.SetActiveService();
                    _serviceMasterRepository.Update(servicedata);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int SetInActiveService(long ServiceId)
        {
            try
            {
                var servicedata = _serviceMasterRepository.GetById(ServiceId);
                if (servicedata != null)
                {
                    servicedata.SetInActiveService();
                    _serviceMasterRepository.Update(servicedata);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 5-3-19 for Margin trading
        public int SetInActiveServiceMargin(long ServiceId)
        {
            try
            {
                var servicedata = _serviceMasterRepositoryMargin.GetById(ServiceId);
                if (servicedata != null)
                {
                    servicedata.SetInActiveService();
                    _serviceMasterRepositoryMargin.Update(servicedata);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ServiceCurrencyData> GetAllServiceConfigurationByBase(String Base)
        {
            List<ServiceCurrencyData> responsedata;
            try
            {
                var CheckBase = _marketRepository.GetSingle(m => m.CurrencyName == Base);
                if (CheckBase == null)
                    return null;
                responsedata = new List<ServiceCurrencyData>();
                var model = _serviceMasterRepository.GetSingle(ser => ser.SMSCode == Base);
                if (model == null)
                    return null;
                var serviceid = model.Id;

                var modellist = _serviceMasterRepository.List();
                foreach (var modelData in modellist)
                {
                    if (modelData.Id == serviceid)
                        continue;
                    responsedata.Add(new ServiceCurrencyData()
                    {
                        Name = modelData.Name,
                        ServiceId = modelData.Id,
                        SMSCode = modelData.SMSCode
                    });
                }
                return responsedata;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 5-3-19 for Margin trading
        public List<ServiceCurrencyData> GetAllServiceConfigurationByBaseMargin(String Base)
        {
            List<ServiceCurrencyData> responsedata;
            try
            {
                var CheckBase = _marketRepositoryMargin.GetSingle(m => m.CurrencyName == Base);
                if (CheckBase == null)
                    return null;

                responsedata = new List<ServiceCurrencyData>();
                var model = _serviceMasterRepositoryMargin.GetSingle(ser => ser.SMSCode == Base);
                if (model == null)
                    return null;

                var serviceid = model.Id;

                var modellist = _serviceMasterRepositoryMargin.List();
                foreach (var modelData in modellist)
                {
                    if (modelData.Id == serviceid)
                        continue;
                    responsedata.Add(new ServiceCurrencyData()
                    {
                        Name = modelData.Name,
                        ServiceId = modelData.Id,
                        SMSCode = modelData.SMSCode
                    });
                }
                return responsedata;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetServiceByBaseReasponse GetCurrency(short ActiveOnly = 0) //komal For Active Currency only
        {
            try
            {
                GetServiceByBaseReasponse _Res = new GetServiceByBaseReasponse();
                List<ServiceCurrencyData> list = new List<ServiceCurrencyData>();
                var modelList = _serviceMasterRepository.List();
                if (ActiveOnly == 1)
                    modelList = modelList.Where(e => e.Status == 1).ToList();
                foreach (var modelData in modelList)
                {
                    list.Add(new ServiceCurrencyData()
                    {
                        Name = modelData.Name,
                        ServiceId = modelData.Id,
                        SMSCode = modelData.SMSCode
                    });
                }
                _Res.Response = list;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 5-3-19 for Margin Trading
        public GetServiceByBaseReasponse GetCurrencyMargin(short ActiveOnly = 0)
        {
            try
            {
                GetServiceByBaseReasponse _Res = new GetServiceByBaseReasponse();
                List<ServiceCurrencyData> list = new List<ServiceCurrencyData>();
                var modelList = _serviceMasterRepositoryMargin.List();
                if (ActiveOnly == 1)
                    modelList = modelList.Where(e => e.Status == 1).ToList();
                foreach (var modelData in modelList)
                {
                    list.Add(new ServiceCurrencyData()
                    {
                        Name = modelData.Name,
                        ServiceId = modelData.Id,
                        SMSCode = modelData.SMSCode
                    });
                }
                _Res.Response = list;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region ProviderMaster
        public IEnumerable<ServiceProviderViewModel> GetAllProvider()
        {
            try
            {
                var list = _ServiceProviderMaster.List();
                List<ServiceProviderViewModel> providerList = new List<ServiceProviderViewModel>();
                foreach (ServiceProviderMaster model in list)
                {
                    providerList.Add(new ServiceProviderViewModel
                    {
                        Id = model.Id,
                        ProviderName = model.ProviderName,
                        Status = model.Status,

                    });
                }
                return providerList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public ServiceProviderViewModel GetPoviderByID(long ID)
        {
            try
            {
                ServiceProviderMaster model = _ServiceProviderMaster.GetById(ID);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new ServiceProviderViewModel
                {
                    Id = model.Id,
                    ProviderName = model.ProviderName,
                    Status = model.Status,

                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public long AddProviderService(ServiceProviderRequest request, long UserId)
        {
            try
            {
                var model = new ServiceProviderMaster
                {
                    Id = request.Id,
                    ProviderName = request.ProviderName,
                    Status = request.Status,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };
                var newModel = _ServiceProviderMaster.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public bool UpdateProviderService(ServiceProviderRequest request, long UserId)
        {
            try
            {
                ServiceProviderMaster model = _ServiceProviderMaster.GetById(request.Id);
                if (model == null)
                {
                    return false;
                }
                model.UpdatedDate = _basePage.UTC_To_IST();
                model.UpdatedBy = UserId;
                model.Status = request.Status;
                _ServiceProviderMaster.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetActiveProvider(long id)
        {
            try
            {
                ServiceProviderMaster model = _ServiceProviderMaster.GetById(id);
                if (model != null)
                {
                    model.EnableProvider();
                    _ServiceProviderMaster.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetInActiveProvider(long id)
        {
            try
            {
                ServiceProviderMaster model = _ServiceProviderMaster.GetById(id);
                if (model != null)
                {
                    model.DisableProvider();
                    _ServiceProviderMaster.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region AppType
        public IEnumerable<AppTypeViewModel> GetAppType()
        {
            try
            {
                var list = _ApptypeRepository.List();
                List<AppTypeViewModel> AppList = new List<AppTypeViewModel>();
                foreach (AppType model in list)
                {
                    AppList.Add(new AppTypeViewModel
                    {
                        Id = model.Id,
                        AppTypeName = model.AppTypeName,
                    });
                }
                return AppList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public AppTypeViewModel GetAppTypeById(long id)
        {
            try
            {
                AppType model = _ApptypeRepository.GetById(id);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new AppTypeViewModel
                {
                    Id = model.Id,
                    AppTypeName = model.AppTypeName
                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public long AddAppType(AppTypeRequest request, long UserId)
        {
            try
            {

                var model = new AppType
                {
                    Id = request.Id,
                    AppTypeName = request.AppTypeName,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };
                var newModel = _ApptypeRepository.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public bool UpdateAppType(AppTypeRequest request, long UserId)
        {
            try
            {
                AppType model = _ApptypeRepository.GetById(request.Id);
                if (model == null)
                {
                    return false;
                }
                model.AppTypeName = request.AppTypeName;
                model.UpdatedDate = _basePage.UTC_To_IST();
                model.UpdatedBy = UserId;

                _ApptypeRepository.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public bool SetActiveAppType(long id)
        {
            try
            {
                AppType model = _ApptypeRepository.GetById(id);
                if (model != null)
                {
                    model.EnableAppType();
                    _ApptypeRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetInActiveAppType(long id)
        {
            try
            {
                AppType model = _ApptypeRepository.GetById(id);
                if (model != null)
                {
                    model.DisableAppType();
                    _ApptypeRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region Transaction Type

        public IEnumerable<TrnTypeViewModel> GetTrnType()
        {
            try
            {
                var list = _TrnTypeMasterRepo.List();
                List<TrnTypeViewModel> TrnTypeList = new List<TrnTypeViewModel>();
                foreach (TrnTypeMaster model in list)
                {
                    TrnTypeList.Add(new TrnTypeViewModel
                    {
                        Id = Convert.ToInt64(model.TrnTypeId),
                        TrnTypeName = model.TrnTypeName,
                    });
                }
                return TrnTypeList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public TrnTypeViewModel GetTrnTypeById(long id)
        {
            try
            {
                TrnTypeMaster model = _TrnTypeMasterRepo.GetById(id);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new TrnTypeViewModel
                {
                    Id = Convert.ToInt64(model.TrnTypeId),
                    TrnTypeName = model.TrnTypeName
                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        #endregion

        #region ProviderType

        public IEnumerable<ProviderTypeViewModel> GetProviderType()
        {
            try
            {
                var list = _ProviderTypeRepository.List();
                List<ProviderTypeViewModel> ProTypeList = new List<ProviderTypeViewModel>();
                foreach (ServiceProviderType model in list)
                {
                    ProTypeList.Add(new ProviderTypeViewModel
                    {
                        Id = model.Id,
                        ServiveProTypeName = model.ServiveProTypeName
                    });
                }
                return ProTypeList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public ProviderTypeViewModel GetProviderTypeById(long id)
        {
            try
            {
                ServiceProviderType model = _ProviderTypeRepository.GetById(id);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new ProviderTypeViewModel
                {
                    Id = model.Id,
                    ServiveProTypeName = model.ServiveProTypeName

                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public long AddProviderType(ProviderTypeRequest request, long UserId)
        {
            try
            {
                var model = new ServiceProviderType
                {
                    Id = request.Id,
                    ServiveProTypeName = request.ServiveProTypeName,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };
                var newModel = _ProviderTypeRepository.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public bool UpdateProviderType(ProviderTypeRequest request, long UserId)
        {
            try
            {
                var model = _ProviderTypeRepository.GetById(request.Id);
                if (model == null)
                {
                    return false;
                }
                model.ServiveProTypeName = request.ServiveProTypeName;
                model.UpdatedDate = _basePage.UTC_To_IST();
                model.UpdatedBy = UserId;
                _ProviderTypeRepository.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public bool SetActiveProviderType(long id)
        {
            try
            {
                ServiceProviderType model = _ProviderTypeRepository.GetById(id);
                if (model != null)
                {
                    model.EnableProviderType();
                    _ProviderTypeRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetInActiveProviderType(long id)
        {
            try
            {
                ServiceProviderType model = _ProviderTypeRepository.GetById(id);
                if (model != null)
                {
                    model.DisableProviderType();
                    _ProviderTypeRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion 

        #region provider Configuration

        public ProviderConfigurationViewModel GetProviderConfiguration(long id)
        {
            try
            {
                ServiceProConfiguration model = _ProviderConfiguration.GetById(id);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new ProviderConfigurationViewModel
                {
                    Id = model.Id,
                    APIKey = model.APIKey,
                    AppKey = model.AppKey,
                    SecretKey = model.SecretKey,
                    Password = model.Password,
                    UserName = model.UserName,
                    status = model.Status,
                    StatusText = Enum.GetName(typeof(ServiceStatus), model.Status)
                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long AddProviderConfiguration(ProviderConfigurationRequest request, long UserId)
        {
            try
            {
                ServiceProConfiguration model = new ServiceProConfiguration
                {
                    APIKey = request.APIKey,
                    AppKey = request.AppKey,
                    SecretKey = request.SecretKey,
                    Password = request.Password,
                    UserName = request.UserName,
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST(),
                    Status = request.status
                };
                var newModel = _ProviderConfiguration.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetActiveProviderConfiguration(long id)
        {
            try
            {
                ServiceProConfiguration model = _ProviderConfiguration.GetById(id);
                if (model != null)
                {
                    model.EnableProConfiguration();
                    _ProviderConfiguration.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetInActiveProviderConfiguration(long id)
        {
            try
            {
                ServiceProConfiguration model = _ProviderConfiguration.GetById(id);
                if (model != null)
                {
                    model.DisableProConfiguration();
                    _ProviderConfiguration.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool UpdateProviderConfiguration(ProviderConfigurationRequest request, long UserId)
        {
            try
            {
                ServiceProConfiguration model = _ProviderConfiguration.GetById(request.Id);
                if (model == null)
                {
                    return false;
                }
                model.APIKey = request.APIKey;
                model.AppKey = request.AppKey;
                model.SecretKey = request.SecretKey;
                model.UserName = request.UserName;
                model.Password = request.Password;
                model.UpdatedDate = _basePage.UTC_To_IST();
                model.UpdatedBy = UserId;
                model.Status = request.status;
                _ProviderConfiguration.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public AllProConfigResponse GetAllProviderConfiguration()
        {
            try
            {
                AllProConfigResponse _Res = new AllProConfigResponse();
                List<ProviderConfigurationViewModel> list = new List<ProviderConfigurationViewModel>();
                var ModelList = _ProviderConfiguration.List();
                if (ModelList.Count == 0)
                {
                    _Res.Response = null;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                foreach (var model in ModelList)
                {
                    list.Add(new ProviderConfigurationViewModel()
                    {
                        APIKey = model.APIKey,
                        AppKey = model.AppKey,
                        Id = model.Id,
                        Password = model.Password,
                        SecretKey = model.SecretKey,
                        UserName = model.UserName,
                        status = model.Status,
                        StatusText = Enum.GetName(typeof(ServiceStatus), model.Status)
                    });
                }
                _Res.Response = list;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ListProConfigResponse ListProviderConfiguration()
        {
            try
            {
                ListProConfigResponse _Res = new ListProConfigResponse();
                List<ListProviderConfigInfo> list = new List<ListProviderConfigInfo>();
                var ModelList = _ProviderConfiguration.List();
                if (ModelList.Count == 0)
                {
                    _Res.Response = null;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                foreach (var model in ModelList)
                {
                    list.Add(new ListProviderConfigInfo()
                    {
                        Id = model.Id,
                        Name = model.UserName + ", " + model.Password
                    });
                }
                _Res.Response = list;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region "Demon Configuration"
        public DemonconfigurationViewModel GetDemonConfiguration(long id)
        {
            try
            {
                var model = _DemonRepository.GetById(id);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new DemonconfigurationViewModel
                {
                    Id = model.Id,
                    IPAdd = model.IPAdd,
                    PortAdd = model.PortAdd,
                    Url = model.Url,
                    Status = model.Status,
                    StatusText = Enum.GetName(typeof(ServiceStatus), model.Status)
                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long AddDemonConfiguration(DemonConfigurationRequest request, long UserId)
        {
            try
            {
                var model = new DemonConfiguration
                {
                    IPAdd = request.IPAdd,
                    PortAdd = request.PortAdd,
                    Url = request.Url,
                    Status = request.Status,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };

                var newModel = _DemonRepository.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool UpdateDemonConfiguration(DemonConfigurationRequest request, long UserId)
        {
            try
            {
                DemonConfiguration model = _DemonRepository.GetById(request.Id);
                if (model == null)
                {
                    return false;
                }
                model.IPAdd = request.IPAdd;
                model.PortAdd = request.PortAdd;
                model.Url = request.Url;
                model.Status = request.Status;
                model.UpdatedBy = UserId;
                model.UpdatedDate = _basePage.UTC_To_IST();
                _DemonRepository.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetActiveDemonConfig(long id)
        {
            try
            {
                DemonConfiguration model = _DemonRepository.GetById(id);
                if (model != null)
                {
                    model.EnableConfiguration();
                    _DemonRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetInActiveDemonConfig(long id)
        {
            try
            {
                DemonConfiguration model = _DemonRepository.GetById(id);
                if (model != null)
                {
                    model.DisableConfiguration();
                    _DemonRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ListDemonConfigResponse GetAllDemonConfigV1()
        {
            try
            {
                ListDemonConfigResponse _Res = new ListDemonConfigResponse();
                List<DemonconfigurationViewModel> modelList = new List<DemonconfigurationViewModel>();
                var list = _DemonRepository.List();
                if (list.Count == 0)
                {
                    _Res.Response = null;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                foreach (var Model in list)
                {
                    modelList.Add(new DemonconfigurationViewModel
                    {
                        Id = Model.Id,
                        IPAdd = Model.IPAdd,
                        PortAdd = Model.PortAdd,
                        Status = Model.Status,
                        Url = Model.Url,
                        StatusText = Enum.GetName(typeof(ServiceStatus), Model.Status)
                    });
                }
                _Res.Response = modelList;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ListDemonConfigResponseV1 ListDemonConfigV1()
        {
            try
            {
                ListDemonConfigResponseV1 _Res = new ListDemonConfigResponseV1();
                List<ListDEmonConfig> modelList = new List<ListDEmonConfig>();
                var list = _DemonRepository.List();
                if (list.Count == 0)
                {
                    _Res.Response = null;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                foreach (var Model in list)
                {
                    modelList.Add(new ListDEmonConfig
                    {
                        Id = Model.Id,
                        Name = Model.IPAdd + ", " + Model.PortAdd + ", " + Model.Url
                    });
                }
                _Res.Response = modelList;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region ProviderDetail
        public IEnumerable<ProviderDetailViewModel> GetProviderDetailList()
        {
            try
            {
                var list = _ProDetailRepository.List();
                List<ProviderDetailViewModel> providerList = new List<ProviderDetailViewModel>();
                foreach (ServiceProviderDetail model in list)
                {
                    providerList.Add(new ProviderDetailViewModel
                    {
                        Id = model.Id,
                        ServiceProID = model.ServiceProID,
                        ProTypeID = model.ProTypeID,
                        AppTypeID = model.AppTypeID,
                        TrnTypeID = model.TrnTypeID,
                        LimitID = model.LimitID,
                        DemonConfigID = model.DemonConfigID,
                        ServiceProConfigID = model.ServiceProConfigID,
                        ThirPartyAPIID = model.ThirPartyAPIID,
                        SerProDetailName = model.SerProDetailName,//Rushabh 15-01-2020
                        Status = model.Status//Rushabh 15-01-2020
                    });
                }
                return providerList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ProviderDetailViewModel GetProviderDetailById(long id)
        {
            try
            {
                var model = _ProDetailRepository.GetById(id);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new ProviderDetailViewModel
                {
                    Id = model.Id,
                    ServiceProID = model.ServiceProID,
                    ProTypeID = model.ProTypeID,
                    AppTypeID = model.AppTypeID,
                    TrnTypeID = model.TrnTypeID,
                    LimitID = model.LimitID,
                    DemonConfigID = model.DemonConfigID,
                    ServiceProConfigID = model.ServiceProConfigID,
                    ThirPartyAPIID = model.ThirPartyAPIID,
                    Status = model.Status,
                    SerProDetailName = model.SerProDetailName
                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long AddProviderDetail(ProviderDetailRequest request, long UserId)
        {
            try
            {
                ServiceProviderDetail model = new ServiceProviderDetail
                {
                    ServiceProID = request.ServiceProID,
                    ProTypeID = request.ProTypeID,
                    AppTypeID = request.AppTypeID,
                    TrnTypeID = request.TrnTypeID,
                    LimitID = request.LimitID,
                    DemonConfigID = request.DemonConfigID,
                    ServiceProConfigID = request.ServiceProConfigID,
                    ThirPartyAPIID = request.ThirPartyAPIID,
                    Status = request.Status,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId,
                    SerProDetailName = request.SerProDetailName
                };

                var newModel = _ProDetailRepository.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass UpdateProviderDetail(ProviderDetailRequest request, long UserId)
        {
            try
            {
                ServiceProviderDetail model = _ProDetailRepository.GetById(request.Id);
                if (model == null)
                {
                    //Rushabh 16-01-2020 Changes Done Related To Response
                    return new BizResponseClass
                    {
                        ReturnCode = enResponseCode.Fail,
                        ErrorCode = enErrorCode.ServiceProDetailNoDataFound,
                        ReturnMsg = EnResponseMessage.NotFound
                    };
                }
                model.ServiceProID = request.ServiceProID;
                model.ProTypeID = request.ProTypeID;
                model.AppTypeID = request.AppTypeID;
                model.TrnTypeID = request.TrnTypeID;
                model.LimitID = request.LimitID;
                model.DemonConfigID = request.DemonConfigID;
                model.ServiceProConfigID = request.ServiceProConfigID;
                model.ThirPartyAPIID = request.ThirPartyAPIID;
                model.UpdatedBy = UserId;
                model.UpdatedDate = _basePage.UTC_To_IST();
                model.SerProDetailName = request.SerProDetailName;
                model.Status = request.Status;
                _ProDetailRepository.Update(model);
                //Rushabh 16-01-2020 Changes Done Related To Response
                return new BizResponseClass
                {
                    ReturnCode = enResponseCode.Success,
                    ErrorCode = enErrorCode.Success,
                    ReturnMsg = EnResponseMessage.RecordUpdated
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetActiveProviderDetail(long id)
        {
            try
            {
                ServiceProviderDetail model = _ProDetailRepository.GetById(id);
                if (model != null)
                {
                    model.EnableProvider();
                    _ProDetailRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetInActiveProviderDetail(long id)
        {
            try
            {
                ServiceProviderDetail model = _ProDetailRepository.GetById(id);
                if (model != null)
                {
                    model.DisableProvider();
                    _ProDetailRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public IEnumerable<ProviderDetailGetAllResponse2> getProviderDetailsDataList(IEnumerable<ProviderDetailViewModel> dataList)
        {
            try
            {
                List<ProviderDetailGetAllResponse2> responcesData = new List<ProviderDetailGetAllResponse2>();
                foreach (ProviderDetailViewModel viewmodel in dataList)
                {
                    responcesData.Add(new ProviderDetailGetAllResponse2
                    {
                        Id = viewmodel.Id,
                        SerProDetailName = viewmodel.SerProDetailName,//Rushabh 15-01-2020
                        Provider = GetPoviderByID(viewmodel.ServiceProID),
                        ProviderType = GetProviderTypeById(viewmodel.ProTypeID),
                        AppType = GetAppTypeById(viewmodel.AppTypeID),
                        TrnType = GetTrnTypeById(viewmodel.TrnTypeID),
                        Limit = GetLimitById(viewmodel.LimitID),
                        DemonConfiguration = GetDemonConfiguration(viewmodel.DemonConfigID),
                        ProviderConfiguration = GetProviderConfiguration(viewmodel.Id),
                        thirdParty = GetThirdPartyAPIConfigById(viewmodel.ThirPartyAPIID),
                        Status = viewmodel.Status //Rushabh 15-01-2020
                    });
                }
                return responcesData;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ProviderDetailGetAllResponse2 getProviderDetailDataById(ProviderDetailViewModel viewModel)
        {
            try
            {
                ProviderDetailGetAllResponse2 res = new ProviderDetailGetAllResponse2
                {
                    Id = viewModel.Id,
                    Provider = GetPoviderByID(viewModel.ServiceProID),
                    ProviderType = GetProviderTypeById(viewModel.ProTypeID),
                    AppType = GetAppTypeById(viewModel.AppTypeID),
                    TrnType = GetTrnTypeById(viewModel.TrnTypeID),
                    Limit = GetLimitById(viewModel.LimitID),
                    DemonConfiguration = GetDemonConfiguration(viewModel.DemonConfigID),
                    ProviderConfiguration = GetProviderConfiguration(viewModel.Id),
                    thirdParty = GetThirdPartyAPIConfigById(viewModel.ThirPartyAPIID),
                    SerProDetailName = viewModel.SerProDetailName,
                    Status = viewModel.Status
                };
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region ProductConfiguration
        public long AddProductConfiguration(ProductConfigurationRequest Request, long UserId)
        {
            try
            {
                ProductConfiguration product = new ProductConfiguration()
                {
                    ProductName = Request.ProductName,
                    ServiceID = Request.ServiceID,
                    CountryID = Request.CountryID,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                };
                var newProduct = _productConfigRepository.Add(product);
                return newProduct.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public long UpdateProductConfiguration(ProductConfigurationRequest Request, long UserId)
        {
            try
            {
                var product = _productConfigRepository.GetById(Request.Id);
                if (product != null)
                {
                    product.ProductName = Request.ProductName;
                    product.ServiceID = Request.ServiceID;
                    product.CountryID = Request.CountryID;
                    product.UpdatedDate = _basePage.UTC_To_IST();
                    product.UpdatedBy = UserId;

                    _productConfigRepository.Update(product);
                    return product.Id;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public ProductConfigrationGetInfo GetProductConfiguration(long ProductId)
        {
            ProductConfigrationGetInfo responsedata;
            try
            {
                responsedata = new ProductConfigrationGetInfo();
                var product = _productConfigRepository.GetById(ProductId);
                if (product != null)
                {
                    responsedata.Id = product.Id;
                    responsedata.ProductName = product.ProductName;

                    var serviceMaster = _serviceMasterRepository.GetById(product.ServiceID);
                    var countryMaster = _countryMasterRepository.GetById(product.CountryID);
                    responsedata.CountryID = product.CountryID;
                    responsedata.ServiceID = product.ServiceID;
                    responsedata.ServiceName = serviceMaster.Name;
                    responsedata.CountryName = countryMaster.CountryName;

                    return responsedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<ProductConfigrationGetInfo> GetAllProductConfiguration()
        {
            List<ProductConfigrationGetInfo> responsedata;
            try
            {
                responsedata = new List<ProductConfigrationGetInfo>();
                //Uday 15-11-2018 applied direct query because one by one record pick has take long time to response
                var productall = _backOfficeTrnRepository.GetAllProductConfiguration();
                if (productall != null && productall.Count > 0)
                {
                    foreach (var product in productall)
                    {
                        ProductConfigrationGetInfo response = new ProductConfigrationGetInfo();
                        response.Id = product.Id;
                        response.ProductName = product.ProductName;

                        //var serviceMaster = _serviceMasterRepository.GetById(product.ServiceID);
                        //var countryMaster = _countryMasterRepository.GetById(product.CountryID);
                        response.CountryID = product.CountryID;
                        response.ServiceID = product.ServiceID;
                        response.ServiceName = product.ServiceName;
                        response.CountryName = product.CountryName;

                        responsedata.Add(response);
                    }
                    return responsedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public int SetActiveProduct(long ProductId)
        {
            try
            {
                var product = _productConfigRepository.GetById(ProductId);
                if (product != null)
                {
                    product.SetActiveProduct();
                    _productConfigRepository.Update(product);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public int SetInActiveProduct(long ProductId)
        {
            try
            {
                var product = _productConfigRepository.GetById(ProductId);
                if (product != null)
                {
                    product.SetInActiveProduct();
                    _productConfigRepository.Update(product);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region RouteConfiguration
        //public long AddRouteConfiguration(RouteConfigurationRequest Request, long UserId)
        //{
        //    try
        //    {
        //        RouteConfiguration route = new RouteConfiguration()
        //        {
        //            RouteName = Request.RouteName,
        //            ServiceID = Request.ServiceID,
        //            SerProDetailID = Request.ServiceProDetailId,
        //            ProductID = Request.ProductID,
        //            Priority = Request.Priority,
        //            StatusCheckUrl = Request.StatusCheckUrl,
        //            ValidationUrl = Request.ValidationUrl,
        //            TransactionUrl = Request.TransactionUrl,
        //            LimitId = Request.LimitId,
        //            OpCode = Request.OpCode,
        //            TrnType = Request.TrnType,
        //            IsDelayAddress = Request.IsDelayAddress,
        //            ProviderWalletID = Request.ProviderWalletID,
        //            Status = Convert.ToInt16(ServiceStatus.Active),
        //            CreatedDate = _basePage.UTC_To_IST(),
        //            CreatedBy = UserId
        //            //UpdatedDate = DateTime.UtcNow,
        //            //UpdatedBy = null
        //        };
        //        var newProduct = _routeConfigRepository.Add(route);
        //        return newProduct.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        public WithdrawConfigResponse2 GetRouteConfiguration(long RouteId, enTrnType TrnType)
        {

            try
            {
                WithdrawConfigResponse2 _Res = new WithdrawConfigResponse2();
                List<WithdrawRouteConfig> list = new List<WithdrawRouteConfig>();
                WithdrawRouteConfig obj = new WithdrawRouteConfig();
                list = _backOfficeTrnRepository.GetWithdrawRoute(RouteId, TrnType);
                obj = list.FirstOrDefault(); //ntrivedi 17-04-2019 instead of singleordefault
                _Res.Response = obj;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {

                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public WithdrawConfigResponse GetAllRouteConfiguration(enTrnType TrnType)
        {

            try
            {
                WithdrawConfigResponse _Res = new WithdrawConfigResponse();
                List<WithdrawRouteConfig> list = new List<WithdrawRouteConfig>();

                list = _backOfficeTrnRepository.GetWithdrawRoute(0, TrnType);
                _Res.Response = new List<GetWithdrawRouteConfig>();
                foreach (var x in list)
                {
                    GetWithdrawRouteConfig newobj = new GetWithdrawRouteConfig
                    {
                        AvailableRoute = x.AvailableRoute,
                        CurrencyName = x.CurrencyName,
                        ServiceID = x.ServiceID,
                        status = x.status,
                        TrnType = x.TrnType
                    };
                    _Res.Response.Add(newobj);
                }
                //_Res.Response=list;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int SetActiveRoute(long RouteId)
        {
            try
            {
                var route = _routeConfigRepository.GetById(RouteId);
                if (route != null)
                {
                    route.SetActiveRoute();
                    _routeConfigRepository.Update(route);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int SetInActiveRoute(long RouteId)
        {
            try
            {
                var route = _routeConfigRepository.GetById(RouteId);
                if (route != null)
                {
                    route.SetInActiveRoute();
                    _routeConfigRepository.Update(route);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass AddWithdrawRouteConfig(WithdrawRouteConfigRequest Request, long UserId)
        {
            try
            {
                BizResponseClass _Res = new BizResponseClass();
                var IsExistservice = _routeConfigRepository.GetSingle(e => e.ServiceID == Request.Request.ServiceID && e.TrnType == Request.Request.TrnType && e.Status != 9);
                if (IsExistservice != null)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.Service_Already_Exist;
                    _Res.ReturnMsg = "Service Already  Exist.";
                    return _Res;
                }
                foreach (var req in Request.Request.AvailableRoute)
                {
                    RouteConfiguration route = new RouteConfiguration()
                    {
                        RouteName = Request.Request.CurrencyName + "_" + (enTrnType)Request.Request.TrnType,
                        ServiceID = Request.Request.ServiceID,
                        ProductID = _productConfigRepository.GetSingle(e => e.ServiceID == Request.Request.ServiceID).Id,
                        SerProDetailID = req.ServiceProDetailId,
                        Priority = req.Priority,
                        //StatusCheckUrl = Request.StatusCheckUrl,
                        //ValidationUrl = Request.ValidationUrl,
                        //TransactionUrl = Request.TransactionUrl,
                        LimitId = 1,
                        OpCode = req.AssetName,
                        TrnType = Request.Request.TrnType,
                        IsDelayAddress = 0,
                        ProviderWalletID = req.ProviderWalletID,
                        ConvertAmount = req.ConvertAmount,
                        ConfirmationCount = req.ConfirmationCount,
                        Status = Request.Request.status,
                        CreatedDate = _basePage.UTC_To_IST(),
                        CreatedBy = UserId,
                        UpdatedDate = DateTime.UtcNow,
                        AccountNoLen = Convert.ToInt32(req.AccountNoLen == null ? 0 : req.AccountNoLen),
                        AccNoStartsWith = req.AccNoStartsWith,
                        AccNoValidationRegex = req.AccNoValidationRegex,
                        UpdatedBy = null
                    };
                    var newProduct = _routeConfigRepository.Add(route);

                }
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Insert Successfully.";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long UpdateWithdrawRouteConfig(WithdrawRouteConfigRequest Request, long UserId)
        {
            long Count = 0;
            try
            {
                var list = _routeConfigRepository.FindBy(e => e.ServiceID == Request.Request.ServiceID && e.TrnType == Request.Request.TrnType && e.Status != 9).Select(e => e.Id).ToList();
                foreach (var req in Request.Request.AvailableRoute)
                {
                    if (req.Id == 0)
                    {
                        RouteConfiguration route2 = new RouteConfiguration
                        {
                            RouteName = Request.Request.CurrencyName + "_" + (enTrnType)Request.Request.TrnType,
                            ServiceID = Request.Request.ServiceID,
                            ProductID = _productConfigRepository.GetSingle(e => e.ServiceID == Request.Request.ServiceID).Id,
                            SerProDetailID = req.ServiceProDetailId,
                            Priority = req.Priority,
                            //StatusCheckUrl = Request.StatusCheckUrl,
                            //ValidationUrl = Request.ValidationUrl,
                            //TransactionUrl = Request.TransactionUrl,
                            LimitId = 1,
                            OpCode = req.AssetName,
                            TrnType = Request.Request.TrnType,
                            IsDelayAddress = 0,
                            ProviderWalletID = req.ProviderWalletID,
                            ConvertAmount = req.ConvertAmount,
                            ConfirmationCount = req.ConfirmationCount,
                            Status = Request.Request.status,
                            CreatedDate = _basePage.UTC_To_IST(),
                            CreatedBy = UserId,
                            UpdatedDate = DateTime.UtcNow,
                            AccountNoLen = Convert.ToInt32(req.AccountNoLen == null ? 0 : req.AccountNoLen),
                            AccNoStartsWith = req.AccNoStartsWith,
                            AccNoValidationRegex = req.AccNoValidationRegex,
                            UpdatedBy = null
                        };

                        var newProduct = _routeConfigRepository.Add(route2);
                        continue;
                    }
                    else
                    {

                        if (list.Contains(req.Id))
                        {
                            var route = _routeConfigRepository.GetById(req.Id);
                            if (route != null)
                            {
                                //route.RouteName = Request.Request.CurrencyName + "_" + "Withdraw";
                                //route.ServiceID = Request.Request.ServiceID;
                                route.SerProDetailID = req.ServiceProDetailId;
                                route.Priority = req.Priority;
                                route.OpCode = req.AssetName;
                                route.ProviderWalletID = req.ProviderWalletID;
                                route.ConfirmationCount = req.ConfirmationCount;
                                route.ConvertAmount = req.ConvertAmount;
                                route.UpdatedDate = _basePage.UTC_To_IST();
                                route.UpdatedBy = UserId;
                                route.Status = Request.Request.status;
                                route.AccountNoLen = Convert.ToInt32(req.AccountNoLen == null ? 0 : req.AccountNoLen);
                                route.AccNoStartsWith = req.AccNoStartsWith;
                                route.AccNoValidationRegex = req.AccNoValidationRegex;
                                //route.ProductID = _productConfigRepository.GetSingle(e => e.ServiceID == Request.Request.ServiceID).Id;
                                //route.StatusCheckUrl = route.StatusCheckUrl;
                                //route.ValidationUrl = route.ValidationUrl;
                                //route.TransactionUrl = route.TransactionUrl;
                                //route.LimitId = route.LimitId;
                                //route.TrnType = enTrnType.Withdraw;
                                //route.IsDelayAddress = route.IsDelayAddress;
                                _routeConfigRepository.Update(route);
                            }
                            else
                            {
                                Count++;
                            }
                            list.Remove(req.Id);
                        }
                    }
                }
                foreach (var DisableObj in list)
                {
                    var route3 = _routeConfigRepository.GetById(DisableObj);
                    route3.Status = 9;
                    route3.UpdatedDate = _basePage.UTC_To_IST();
                    route3.UpdatedBy = UserId;
                    _routeConfigRepository.Update(route3);
                }
                return Count;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public AvailableRouteResponse GetAvailableRoute()
        {
            try
            {
                AvailableRouteResponse _Res = new AvailableRouteResponse();
                _Res.Response = _backOfficeTrnRepository.GetAvailableRoute();
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        #endregion

        #region ThirdPartyAPIConfig
        public List<ThirdPartyAPIConfigViewModel> GetAllThirdPartyAPIConfig()
        {
            try
            {
                var list = _thirdPartyAPIRepository.List();
                List<ThirdPartyAPIConfigViewModel> thirdPartyAPIs = new List<ThirdPartyAPIConfigViewModel>();
                foreach (ThirdPartyAPIConfiguration model in list)
                {
                    thirdPartyAPIs.Add(new ThirdPartyAPIConfigViewModel()
                    {
                        Id = model.Id,
                        APIBalURL = model.APIBalURL,
                        APIName = model.APIName,
                        APIRequestBody = model.APIRequestBody,
                        APISendURL = model.APISendURL,
                        APIStatusCheckURL = model.APIStatusCheckURL,
                        APIValidateURL = model.APIValidateURL,
                        AppType = model.AppType,
                        AuthHeader = model.AuthHeader,
                        ContentType = model.ContentType,
                        HashCode = model.HashCode,
                        HashCodeRecheck = model.HashCodeRecheck,
                        HashType = model.HashType,
                        MerchantCode = model.MerchantCode,
                        MethodType = model.MethodType,
                        ParsingDataID = model.ParsingDataID,
                        ResponseFailure = model.ResponseFailure,
                        ResponseHold = model.ResponseHold,
                        ResponseSuccess = model.ResponseSuccess,
                        TransactionIdPrefix = model.TransactionIdPrefix,
                        AppTypeText = ((enAppType)model.AppType).ToString(),
                        Status = model.Status
                    });
                }

                return thirdPartyAPIs;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ThirdPartyAPIConfigViewModel GetThirdPartyAPIConfigById(long Id)
        {
            try
            {
                ThirdPartyAPIConfiguration model = _thirdPartyAPIRepository.GetById(Id);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new ThirdPartyAPIConfigViewModel
                {
                    Id = model.Id,
                    APIBalURL = model.APIBalURL,
                    APIName = model.APIName,
                    APIRequestBody = model.APIRequestBody,
                    APISendURL = model.APISendURL,
                    APIStatusCheckURL = model.APIStatusCheckURL,
                    APIValidateURL = model.APIValidateURL,
                    AppType = model.AppType,
                    AuthHeader = model.AuthHeader,
                    ContentType = model.ContentType,
                    HashCode = model.HashCode,
                    HashCodeRecheck = model.HashCodeRecheck,
                    HashType = model.HashType,
                    MerchantCode = model.MerchantCode,
                    MethodType = model.MethodType,
                    ParsingDataID = model.ParsingDataID,
                    ResponseFailure = model.ResponseFailure,
                    ResponseHold = model.ResponseHold,
                    ResponseSuccess = model.ResponseSuccess,
                    TransactionIdPrefix = model.TransactionIdPrefix,
                    AppTypeText = ((enAppType)model.AppType).ToString(),
                    Status = model.Status
                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long AddThirdPartyAPI(ThirdPartyAPIConfigRequest request, long UserId)
        {
            try
            {
                var model = new ThirdPartyAPIConfiguration()
                {
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST(),
                    Status = request.Status,
                    APIBalURL = request.APIBalURL,
                    APIName = request.APIName,
                    APIRequestBody = request.APIRequestBody,
                    APISendURL = request.APISendURL,
                    APIStatusCheckURL = request.APIStatusCheckURL,
                    APIValidateURL = request.APIValidateURL,
                    AppType = request.AppType,
                    AuthHeader = request.AuthHeader,
                    ContentType = request.ContentType,
                    HashCode = "",
                    HashCodeRecheck = "",
                    HashType = 0,
                    MerchantCode = request.MerchantCode,
                    MethodType = request.MethodType,
                    ParsingDataID = request.ParsingDataID,
                    ResponseFailure = request.ResponseFailure,
                    ResponseHold = request.ResponseHold,
                    ResponseSuccess = request.ResponseSuccess,
                    TransactionIdPrefix = request.TransactionIdPrefix

                };
                var newModel = _thirdPartyAPIRepository.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool UpdateThirdPartyAPI(ThirdPartyAPIConfigRequest request, long UserId)
        {
            try
            {
                var model = _thirdPartyAPIRepository.GetById(request.Id);
                if (model == null)
                {
                    return false;
                }

                model.APIBalURL = request.APIBalURL;
                model.APIName = request.APIName;
                model.APIRequestBody = request.APIRequestBody;
                model.APISendURL = request.APISendURL;
                model.APIStatusCheckURL = request.APIStatusCheckURL;
                model.APIValidateURL = request.APIValidateURL;
                model.AppType = request.AppType;
                model.AuthHeader = request.AuthHeader;
                model.ContentType = request.ContentType;
                model.MerchantCode = request.MerchantCode;
                model.MethodType = request.MethodType;
                model.ParsingDataID = request.ParsingDataID;
                model.ResponseFailure = request.ResponseFailure;
                model.ResponseHold = request.ResponseHold;
                model.ResponseSuccess = request.ResponseSuccess;
                model.TransactionIdPrefix = request.TransactionIdPrefix;
                model.UpdatedDate = _basePage.UTC_To_IST();
                model.UpdatedBy = UserId;
                model.Status = request.Status;
                _thirdPartyAPIRepository.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetActiveThirdPartyAPI(long id)
        {
            try
            {
                ThirdPartyAPIConfiguration model = _thirdPartyAPIRepository.GetById(id);
                if (model != null)
                {
                    model.SetActive();
                    _thirdPartyAPIRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetInActiveThirdPartyAPI(long id)
        {
            try
            {
                ThirdPartyAPIConfiguration model = _thirdPartyAPIRepository.GetById(id);
                if (model != null)
                {
                    model.SetInActive();
                    _thirdPartyAPIRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region ThirdPartyAPIResponseConfig

        public List<ThirdPartyAPIConfigurationResponseViewModel> GetAllThirdPartyAPIResponse()
        {
            try
            {
                var list = _thirdPartyAPIResRepository.List();
                List<ThirdPartyAPIConfigurationResponseViewModel> APIResponseList = new List<ThirdPartyAPIConfigurationResponseViewModel>();
                foreach (var model in list)
                {
                    APIResponseList.Add(new ThirdPartyAPIConfigurationResponseViewModel()
                    {
                        ParsingName = model.ParsingName,
                        BalanceRegex = model.BalanceRegex,
                        ErrorCodeRegex = model.ErrorCodeRegex,
                        Id = model.Id,
                        OprTrnRefNoRegex = model.OprTrnRefNoRegex,
                        Param1Regex = model.Param1Regex,
                        Param2Regex = model.Param2Regex,
                        Param3Regex = model.Param3Regex,
                        ResponseCodeRegex = model.ResponseCodeRegex,
                        StatusMsgRegex = model.StatusMsgRegex,
                        StatusRegex = model.StatusRegex,
                        TrnRefNoRegex = model.TrnRefNoRegex,
                        Status = model.Status
                    });
                }
                return APIResponseList;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ThirdPartyAPIConfigurationResponseViewModel GetThirdPartyAPIResponseById(long id)
        {
            try
            {
                var model = _thirdPartyAPIResRepository.GetById(id);
                if (model == null)
                {
                    return null;
                }
                var viewModel = new ThirdPartyAPIConfigurationResponseViewModel()
                {
                    ParsingName = model.ParsingName,
                    BalanceRegex = model.BalanceRegex,
                    ErrorCodeRegex = model.ErrorCodeRegex,
                    Id = model.Id,
                    OprTrnRefNoRegex = model.OprTrnRefNoRegex,
                    Param1Regex = model.Param1Regex,
                    Param2Regex = model.Param2Regex,
                    Param3Regex = model.Param3Regex,
                    ResponseCodeRegex = model.ResponseCodeRegex,
                    StatusMsgRegex = model.StatusMsgRegex,
                    StatusRegex = model.StatusRegex,
                    TrnRefNoRegex = model.TrnRefNoRegex,
                    Status = model.Status
                };
                return viewModel;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long AddThirdPartyAPIResponse(ThirdPartyAPIConfigurationResponseViewModel Request, long UserId) //change by jagdish 12-02-2020
        {
            try
            {
                var model = new ThirdPartyAPIResponseConfiguration()
                {
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST(),
                    Status = Request.Status,
                    ParsingName = Request.ParsingName,
                    BalanceRegex = Request.BalanceRegex,
                    ErrorCodeRegex = Request.ErrorCodeRegex,
                    OprTrnRefNoRegex = Request.OprTrnRefNoRegex,
                    Param1Regex = Request.Param1Regex,
                    Param2Regex = Request.Param2Regex,
                    Param3Regex = Request.Param3Regex,
                    ResponseCodeRegex = Request.ResponseCodeRegex,
                    StatusMsgRegex = Request.StatusMsgRegex,
                    StatusRegex = Request.StatusRegex,
                    TrnRefNoRegex = Request.TrnRefNoRegex
                };
                var newModel = _thirdPartyAPIResRepository.Add(model);

                if (newModel != null)
                    return newModel.Id;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool UpdateThirdPartyAPIResponse(ThirdPartyAPIConfigurationResponseViewModel Request, long UserId) //change by jagdish 12-02-2020
        {
            try
            {
                var model = _thirdPartyAPIResRepository.GetById(Request.Id);
                if (model == null)
                {
                    return false;
                }
                model.ParsingName = Request.ParsingName;
                model.BalanceRegex = Request.BalanceRegex;
                model.ErrorCodeRegex = Request.ErrorCodeRegex;
                model.OprTrnRefNoRegex = Request.OprTrnRefNoRegex;
                model.Param1Regex = Request.Param1Regex;
                model.Param2Regex = Request.Param2Regex;
                model.Param3Regex = Request.Param3Regex;
                model.ResponseCodeRegex = Request.ResponseCodeRegex;
                model.StatusMsgRegex = Request.StatusMsgRegex;
                model.StatusRegex = Request.StatusRegex;
                model.TrnRefNoRegex = Request.TrnRefNoRegex;
                model.UpdatedBy = UserId;
                model.UpdatedDate = _basePage.UTC_To_IST();
                model.Status = Request.Status;
                _thirdPartyAPIResRepository.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetActiveThirdPartyAPIResponse(long id)
        {
            try
            {
                ThirdPartyAPIResponseConfiguration model = _thirdPartyAPIResRepository.GetById(id);
                if (model != null)
                {
                    model.SetActive();
                    _thirdPartyAPIResRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public bool SetInActiveThirdPartyAPIResponse(long id)
        {
            try
            {
                ThirdPartyAPIResponseConfiguration model = _thirdPartyAPIResRepository.GetById(id);
                if (model != null)
                {
                    model.SetInActive();
                    _thirdPartyAPIResRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region TradePairConfiguration
        public long AddPairConfiguration(TradePairConfigRequest Request, long UserId)
        {
            try
            {
                //Check Pair Already Available
                var OldPairData = _tradePairMasterRepository.GetSingle(x => x.BaseCurrencyId == Request.BaseCurrencyId && x.SecondaryCurrencyId == Request.SecondaryCurrencyId);

                if (OldPairData != null)
                {
                    return -1;
                }

                var baseCurrency = _serviceMasterRepository.GetById(Request.BaseCurrencyId);
                var secondCurrency = _serviceMasterRepository.GetById(Request.SecondaryCurrencyId);
                var pairName = secondCurrency.SMSCode + "_" + baseCurrency.SMSCode;

                var pairMaster = new TradePairMaster()
                {
                    PairName = pairName,
                    SecondaryCurrencyId = Request.SecondaryCurrencyId,
                    WalletMasterID = 0,
                    BaseCurrencyId = Request.BaseCurrencyId,
                    Status = Request.Status,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };

                var newPairMaster = _tradePairMasterRepository.Add(pairMaster);

                var pairDetail = new TradePairDetail()
                {
                    PairId = newPairMaster.Id,
                    BuyMinQty = Request.BuyMinQty,
                    BuyMaxQty = Request.BuyMaxQty,
                    SellMinQty = Request.SellMinQty,
                    SellMaxQty = Request.SellMaxQty,
                    SellPrice = Request.SellPrice,
                    BuyPrice = Request.BuyPrice,
                    BuyMinPrice = Request.BuyMinPrice,
                    BuyMaxPrice = Request.BuyMaxPrice,
                    SellMinPrice = Request.SellMinPrice,
                    SellMaxPrice = Request.SellMaxPrice,
                    SellFees = Request.SellFees,
                    BuyFees = Request.BuyFees,
                    FeesCurrency = Request.FeesCurrency,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    CreatedDate = _basePage.UTC_To_IST(),
                    ChargeType = Request.ChargeType,
                    OpenOrderExpiration = Request.OpenOrderExpiration,
                    CreatedBy = UserId,
                    AmtLength = Request.AmtLength,
                    PriceLength = Request.PriceLength,
                    QtyLength = Request.QtyLength,
                    PairPercentage = Request.PairPercentage //Rushabh 06-01-2020 Added As Per Front Requirement.
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };
                var newPairDetail = _tradePairDetailRepository.Add(pairDetail);

                var pairStastic = new TradePairStastics
                {
                    PairId = newPairMaster.Id,
                    CurrentRate = Request.Currentrate,
                    LTP = Request.Currentrate,  // Uday 01-01-2019 When Pair add also add LTP Column
                    ChangeVol24 = Request.Volume,
                    CurrencyPrice = Request.CurrencyPrice,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };
                var newPairStatics = _tradePairStastics.Add(pairStastic);

                return newPairMaster.Id;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 12-3-19 Margin Trading
        public long AddPairConfigurationMargin(TradePairConfigRequest Request, long UserId)
        {
            try
            {
                //Check Pair Already Available
                var OldPairData = _tradePairMasterRepositoryMargin.GetSingle(x => x.BaseCurrencyId == Request.BaseCurrencyId && x.SecondaryCurrencyId == Request.SecondaryCurrencyId);

                if (OldPairData != null)
                {
                    return -1;
                }

                var baseCurrency = _serviceMasterRepositoryMargin.GetById(Request.BaseCurrencyId);
                var secondCurrency = _serviceMasterRepositoryMargin.GetById(Request.SecondaryCurrencyId);
                var pairName = secondCurrency.SMSCode + "_" + baseCurrency.SMSCode;

                var pairMaster = new TradePairMasterMargin()
                {
                    PairName = pairName,
                    SecondaryCurrencyId = Request.SecondaryCurrencyId,
                    WalletMasterID = 0,
                    BaseCurrencyId = Request.BaseCurrencyId,
                    Status = Request.Status,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };

                var newPairMaster = _tradePairMasterRepositoryMargin.Add(pairMaster);

                var pairDetail = new TradePairDetailMargin()
                {
                    PairId = newPairMaster.Id,
                    BuyMinQty = Request.BuyMinQty,
                    BuyMaxQty = Request.BuyMaxQty,
                    SellMinQty = Request.SellMinQty,
                    SellMaxQty = Request.SellMaxQty,
                    SellPrice = Request.SellPrice,
                    BuyPrice = Request.BuyPrice,
                    BuyMinPrice = Request.BuyMinPrice,
                    BuyMaxPrice = Request.BuyMaxPrice,
                    SellMinPrice = Request.SellMinPrice,
                    SellMaxPrice = Request.SellMaxPrice,
                    SellFees = Request.SellFees,
                    BuyFees = Request.BuyFees,
                    FeesCurrency = Request.FeesCurrency,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    CreatedDate = _basePage.UTC_To_IST(),
                    ChargeType = Request.ChargeType,
                    OpenOrderExpiration = Request.OpenOrderExpiration,
                    CreatedBy = UserId

                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null


                };
                var newPairDetail = _tradePairDetailRepositoryMargin.Add(pairDetail);

                var pairStastic = new TradePairStasticsMargin
                {
                    PairId = newPairMaster.Id,
                    CurrentRate = Request.Currentrate,
                    LTP = Request.Currentrate,  // Uday 01-01-2019 When Pair add also add LTP Column
                    ChangeVol24 = Request.Volume,
                    CurrencyPrice = Request.CurrencyPrice,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };
                var newPairStatics = _tradePairStasticsMargin.Add(pairStastic);

                return newPairMaster.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long UpdatePairConfiguration(TradePairConfigRequest Request, long UserId)
        {
            try
            {
                var pairMaster = _tradePairMasterRepository.GetById(Request.Id);

                if (pairMaster != null)
                {
                    //var baseCurrency = _serviceMasterRepository.GetById(Request.BaseCurrencyId);
                    //var secondCurrency = _serviceMasterRepository.GetById(Request.SecondaryCurrencyId);
                    //var pairName = secondCurrency.SMSCode + "_" + baseCurrency.SMSCode;

                    //pairMaster.PairName = pairName;
                    //pairMaster.SecondaryCurrencyId = Request.SecondaryCurrencyId;
                    pairMaster.WalletMasterID = 0;
                    //pairMaster.BaseCurrencyId = Request.BaseCurrencyId;
                    pairMaster.UpdatedDate = _basePage.UTC_To_IST();
                    pairMaster.UpdatedBy = UserId;
                    pairMaster.Status = Request.Status;
                    _tradePairMasterRepository.Update(pairMaster);

                    var pairDetail = _tradePairDetailRepository.GetSingle(pair => pair.PairId == Request.Id);

                    pairDetail.BuyMinQty = Request.BuyMinQty;
                    pairDetail.BuyMaxQty = Request.BuyMaxQty;
                    pairDetail.SellMinQty = Request.SellMinQty;
                    pairDetail.SellMaxQty = Request.SellMaxQty;
                    pairDetail.SellPrice = Request.SellPrice;
                    pairDetail.BuyPrice = Request.BuyPrice;
                    pairDetail.BuyMinPrice = Request.BuyMinPrice;
                    pairDetail.BuyMaxPrice = Request.BuyMaxPrice;
                    pairDetail.SellMinPrice = Request.SellMinPrice;
                    pairDetail.SellMaxPrice = Request.SellMaxPrice;
                    pairDetail.SellFees = Request.SellFees;
                    pairDetail.BuyFees = Request.BuyFees;
                    pairDetail.FeesCurrency = Request.FeesCurrency;
                    pairDetail.UpdatedDate = _basePage.UTC_To_IST();
                    pairDetail.UpdatedBy = UserId;
                    pairDetail.ChargeType = Request.ChargeType;
                    pairDetail.OpenOrderExpiration = Request.OpenOrderExpiration;
                    pairDetail.PairPercentage = Request.PairPercentage;//Rushabh 06-01-2020 Added As Per Front Requirement.
                    _tradePairDetailRepository.Update(pairDetail);

                    var pairStastics = _tradePairStastics.GetSingle(pair => pair.PairId == Request.Id);
                    pairStastics.CurrentRate = Request.Currentrate;
                    pairStastics.ChangeVol24 = Request.Volume;
                    pairStastics.CurrencyPrice = Request.CurrencyPrice;
                    pairStastics.UpdatedDate = _basePage.UTC_To_IST();
                    pairStastics.UpdatedBy = UserId;
                    _tradePairStastics.Update(pairStastics);

                    return Request.Id;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 12-3-19 Margin Trading
        public long UpdatePairConfigurationMargin(TradePairConfigRequest Request, long UserId)
        {
            try
            {
                var pairMaster = _tradePairMasterRepositoryMargin.GetById(Request.Id);

                if (pairMaster != null)
                {
                    //pairMaster.PairName = pairName;
                    //pairMaster.SecondaryCurrencyId = Request.SecondaryCurrencyId;
                    pairMaster.WalletMasterID = 0;
                    //pairMaster.BaseCurrencyId = Request.BaseCurrencyId;
                    pairMaster.UpdatedDate = _basePage.UTC_To_IST();
                    pairMaster.UpdatedBy = UserId;
                    pairMaster.Status = Request.Status;
                    _tradePairMasterRepositoryMargin.Update(pairMaster);

                    var pairDetail = _tradePairDetailRepositoryMargin.GetSingle(pair => pair.PairId == Request.Id);

                    pairDetail.BuyMinQty = Request.BuyMinQty;
                    pairDetail.BuyMaxQty = Request.BuyMaxQty;
                    pairDetail.SellMinQty = Request.SellMinQty;
                    pairDetail.SellMaxQty = Request.SellMaxQty;
                    pairDetail.SellPrice = Request.SellPrice;
                    pairDetail.BuyPrice = Request.BuyPrice;
                    pairDetail.BuyMinPrice = Request.BuyMinPrice;
                    pairDetail.BuyMaxPrice = Request.BuyMaxPrice;
                    pairDetail.SellMinPrice = Request.SellMinPrice;
                    pairDetail.SellMaxPrice = Request.SellMaxPrice;
                    pairDetail.SellFees = Request.SellFees;
                    pairDetail.BuyFees = Request.BuyFees;
                    pairDetail.FeesCurrency = Request.FeesCurrency;
                    pairDetail.UpdatedDate = _basePage.UTC_To_IST();
                    pairDetail.UpdatedBy = UserId;
                    pairDetail.ChargeType = Request.ChargeType;
                    pairDetail.OpenOrderExpiration = Request.OpenOrderExpiration;
                    _tradePairDetailRepositoryMargin.Update(pairDetail);

                    var pairStastics = _tradePairStasticsMargin.GetSingle(pair => pair.PairId == Request.Id);
                    pairStastics.CurrentRate = Request.Currentrate;
                    pairStastics.ChangeVol24 = Request.Volume;
                    pairStastics.CurrencyPrice = Request.CurrencyPrice;
                    pairStastics.UpdatedDate = _basePage.UTC_To_IST();
                    pairStastics.UpdatedBy = UserId;
                    _tradePairStasticsMargin.Update(pairStastics);

                    return Request.Id;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TradePairConfigRequest GetPairConfiguration(long PairId)
        {
            TradePairConfigRequest responsedata;
            try
            {
                responsedata = new TradePairConfigRequest();
                var pairMaster = _tradePairMasterRepository.GetById(PairId);
                if (pairMaster != null)
                {
                    responsedata.Id = pairMaster.Id;
                    responsedata.PairName = pairMaster.PairName;
                    responsedata.SecondaryCurrencyId = pairMaster.SecondaryCurrencyId;
                    //responsedata.WalletMasterID = pairMaster.WalletMasterID;
                    responsedata.BaseCurrencyId = pairMaster.BaseCurrencyId;
                    responsedata.StatusText = Enum.GetName(typeof(ServiceStatus), pairMaster.Status);
                    responsedata.Status = pairMaster.Status;
                    var pairDetail = _tradePairDetailRepository.GetSingle(pair => pair.PairId == PairId);

                    responsedata.BuyMinQty = pairDetail.BuyMinQty;
                    responsedata.BuyMaxQty = pairDetail.BuyMaxQty;
                    responsedata.SellMinQty = pairDetail.SellMinQty;
                    responsedata.SellMaxQty = pairDetail.SellMaxQty;
                    responsedata.SellPrice = pairDetail.SellPrice;
                    responsedata.BuyPrice = pairDetail.BuyPrice;
                    responsedata.BuyMinPrice = pairDetail.BuyMinPrice;
                    responsedata.BuyMaxPrice = pairDetail.BuyMaxPrice;
                    responsedata.SellMinPrice = pairDetail.SellMinPrice;
                    responsedata.SellMaxPrice = pairDetail.SellMaxPrice;
                    responsedata.BuyFees = pairDetail.BuyFees;
                    responsedata.SellFees = pairDetail.SellFees;
                    responsedata.FeesCurrency = pairDetail.FeesCurrency;
                    responsedata.OpenOrderExpiration = Convert.ToInt64(pairDetail.OpenOrderExpiration);
                    responsedata.ChargeType = Convert.ToInt16(pairDetail.ChargeType);
                    responsedata.PairPercentage = pairDetail.PairPercentage;
                    var pairStastics = _tradePairStastics.GetSingle(pair => pair.PairId == PairId);
                    responsedata.Volume = pairStastics.ChangeVol24;
                    //responsedata.Currentrate = pairStastics.CurrentRate;
                    //responsedata.CurrencyPrice = pairStastics.CurrencyPrice;

                    var service = _serviceMasterRepository.GetSingle(pair => pair.Id == pairMaster.SecondaryCurrencyId);
                    responsedata.MarketName = service.Name;
                    return responsedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 12-3-19 Margin Trading
        public TradePairConfigRequest GetPairConfigurationMargin(long PairId)
        {
            TradePairConfigRequest responsedata;
            try
            {
                responsedata = new TradePairConfigRequest();
                var pairMaster = _tradePairMasterRepositoryMargin.GetById(PairId);
                if (pairMaster != null)
                {
                    responsedata.Id = pairMaster.Id;
                    responsedata.PairName = pairMaster.PairName;
                    responsedata.SecondaryCurrencyId = pairMaster.SecondaryCurrencyId;
                    //responsedata.WalletMasterID = pairMaster.WalletMasterID;
                    responsedata.BaseCurrencyId = pairMaster.BaseCurrencyId;
                    responsedata.StatusText = Enum.GetName(typeof(ServiceStatus), pairMaster.Status);
                    responsedata.Status = pairMaster.Status;
                    var pairDetail = _tradePairDetailRepositoryMargin.GetSingle(pair => pair.PairId == PairId);

                    responsedata.BuyMinQty = pairDetail.BuyMinQty;
                    responsedata.BuyMaxQty = pairDetail.BuyMaxQty;
                    responsedata.SellMinQty = pairDetail.SellMinQty;
                    responsedata.SellMaxQty = pairDetail.SellMaxQty;
                    responsedata.SellPrice = pairDetail.SellPrice;
                    responsedata.BuyPrice = pairDetail.BuyPrice;
                    responsedata.BuyMinPrice = pairDetail.BuyMinPrice;
                    responsedata.BuyMaxPrice = pairDetail.BuyMaxPrice;
                    responsedata.SellMinPrice = pairDetail.SellMinPrice;
                    responsedata.SellMaxPrice = pairDetail.SellMaxPrice;
                    responsedata.BuyFees = pairDetail.BuyFees;
                    responsedata.SellFees = pairDetail.SellFees;
                    responsedata.FeesCurrency = pairDetail.FeesCurrency;
                    responsedata.OpenOrderExpiration = Convert.ToInt64(pairDetail.OpenOrderExpiration);
                    responsedata.ChargeType = Convert.ToInt16(pairDetail.ChargeType);
                    var pairStastics = _tradePairStasticsMargin.GetSingle(pair => pair.PairId == PairId);
                    responsedata.Volume = pairStastics.ChangeVol24;
                    //responsedata.Currentrate = pairStastics.CurrentRate;
                    //responsedata.CurrencyPrice = pairStastics.CurrencyPrice;

                    var service = _serviceMasterRepositoryMargin.GetSingle(pair => pair.Id == pairMaster.SecondaryCurrencyId);
                    responsedata.MarketName = service.Name;
                    return responsedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TradePairConfigRequest> GetAllPairConfiguration(short IsMargin = 0)//Rita 5-3-19 for Margin Trading
        {
            List<TradePairConfigRequest> responsedata;
            try
            {
                responsedata = new List<TradePairConfigRequest>();

                //Uday 15-11-2018 applied direct query because one by one record pick has take long time to response
                //var pairMaster = _tradePairMasterRepository.List();
                List<TradePairConfigRequest> pairMaster;
                if (IsMargin == 1)
                    pairMaster = _backOfficeTrnRepository.GetAllPairConfigurationMargin();
                else
                    pairMaster = _backOfficeTrnRepository.GetAllPairConfiguration();

                if (pairMaster != null && pairMaster.Count > 0)
                {
                    foreach (var pair in pairMaster)
                    {
                        TradePairConfigRequest response = new TradePairConfigRequest();
                        response.Id = pair.Id;
                        response.PairName = pair.PairName;
                        response.MarketName = pair.MarketName;
                        response.SecondaryCurrencyId = pair.SecondaryCurrencyId;
                        //response.WalletMasterID = pair.WalletMasterID;
                        response.BaseCurrencyId = pair.BaseCurrencyId;
                        response.StatusText = Enum.GetName(typeof(ServiceStatus), pair.Status);
                        //var pairDetail = _tradePairDetailRepository.GetSingle(x => x.PairId == pair.Id);
                        response.Status = pair.Status;
                        response.BuyMinQty = pair.BuyMinQty;
                        response.BuyMaxQty = pair.BuyMaxQty;
                        response.SellMinQty = pair.SellMinQty;
                        response.SellMaxQty = pair.SellMaxQty;
                        response.SellPrice = pair.SellPrice;
                        response.BuyPrice = pair.BuyPrice;
                        response.BuyMinPrice = pair.BuyMinPrice;
                        response.BuyMaxPrice = pair.BuyMaxPrice;
                        response.SellMinPrice = pair.SellMinPrice;
                        response.SellMaxPrice = pair.SellMaxPrice;
                        response.BuyFees = pair.BuyFees;
                        response.SellFees = pair.SellFees;
                        response.FeesCurrency = pair.FeesCurrency;
                        response.ChargeType = pair.ChargeType;
                        response.OpenOrderExpiration = pair.OpenOrderExpiration;
                        //var pairStastics = _tradePairStastics.GetSingle(x => x.PairId == pair.Id);
                        response.Volume = pair.Volume;
                        response.Currentrate = pair.Currentrate;
                        response.CurrencyPrice = pair.CurrencyPrice;
                        response.PairPercentage = pair.PairPercentage;//Rushabh 06-01-2020 Added As Per Front Requirement.

                        responsedata.Add(response);
                    }
                    return responsedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int SetActivePair(long PairId)
        {
            try
            {
                var pairdata = _tradePairMasterRepository.GetById(PairId);
                if (pairdata != null)
                {
                    pairdata.MakePairActive();
                    _tradePairMasterRepository.Update(pairdata);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 12-3-19 Margin Trading
        public int SetActivePairMargin(long PairId)
        {
            try
            {
                var pairdata = _tradePairMasterRepositoryMargin.GetById(PairId);
                if (pairdata != null)
                {
                    pairdata.MakePairActive();
                    _tradePairMasterRepositoryMargin.Update(pairdata);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int SetInActivePair(long PairId)
        {
            try
            {
                var pairdata = _tradePairMasterRepository.GetById(PairId);
                if (pairdata != null)
                {
                    pairdata.MakePairInActive();
                    _tradePairMasterRepository.Update(pairdata);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 12-3-19 Margin Trading
        public int SetInActivePairMargin(long PairId)
        {
            try
            {
                var pairdata = _tradePairMasterRepositoryMargin.GetById(PairId);
                if (pairdata != null)
                {
                    pairdata.MakePairInActive();
                    _tradePairMasterRepositoryMargin.Update(pairdata);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public Core.ViewModels.Configuration.ListPairResponse ListPair(short IsMargin = 0)//Rita 12-3-19 for Margin Trading
        {
            try
            {
                Core.ViewModels.Configuration.ListPairResponse _Res = new Core.ViewModels.Configuration.ListPairResponse();
                List<ListPairInfo> list;
                if (IsMargin == 1)
                    list = _backOfficeTrnRepository.ListPairInfoMargin();
                else
                    list = _backOfficeTrnRepository.ListPairInfo();

                if (list.Count == 0)
                {
                    _Res.Response = null;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                _Res.Response = list;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region Other Configuration
        public List<ServiceTypeMasterInfo> GetAllServiceTypeMaster()
        {
            List<ServiceTypeMasterInfo> responsedata;
            try
            {
                //17-12-2018 komal Update code
                responsedata = new List<ServiceTypeMasterInfo>();
                foreach (enServiceType foo in Enum.GetValues(typeof(enServiceType)))
                {
                    responsedata.Add(new ServiceTypeMasterInfo { Id = (int)foo, SerTypeName = foo.ToString() });
                }
                //String[] serviceTypeName = Enum.GetNames(typeof(enServiceType));
                //int[] serviceTypeValues = (int[])Enum.GetValues(typeof(enServiceType));
                //if (serviceTypeName.Length > 0)
                //{
                //    for (int i = 0; i < serviceTypeName.Length; i++)
                //    {
                //        ServiceTypeMasterInfo response = new ServiceTypeMasterInfo();
                //        response.Id = serviceTypeValues[i];
                //        response.SerTypeName = serviceTypeName[i];

                //        responsedata.Add(response);
                //    }
                return responsedata;
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<TransactionTypeInfo> GetAllTransactionType()
        {
            List<TransactionTypeInfo> responsedata;
            try
            {
                //17-12-2018 komal Update code
                responsedata = new List<TransactionTypeInfo>();
                foreach (enTrnType foo in Enum.GetValues(typeof(enTrnType)))
                {
                    responsedata.Add(new TransactionTypeInfo { Id = (int)foo, TrnTypeName = foo.ToString() });
                }
                //String[] trnTypeName = Enum.GetNames(typeof(enTrnType));
                //int[] trnTypeValues = (int[])Enum.GetValues(typeof(enTrnType));
                //if (trnTypeName.Length > 0)
                //{
                //    for (int i = 0; i < trnTypeName.Length; i++)
                //    {
                //        TransactionTypeInfo response = new TransactionTypeInfo();
                //        response.Id = trnTypeValues[i];
                //        response.TrnTypeName = trnTypeName[i];

                //        responsedata.Add(response);
                //    }
                return responsedata;
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public OrderTypeResponse GetOrderType()
        {
            OrderTypeResponse _Res = new OrderTypeResponse();
            List<OrderTypeInfo> responsedata = new List<OrderTypeInfo>();
            try
            {
                foreach (enTransactionMarketType foo in Enum.GetValues(typeof(enTransactionMarketType)))
                {
                    responsedata.Add(new OrderTypeInfo { ID = (int)foo, OrderType = foo.ToString() });
                }
                _Res.Response = responsedata;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region Limit

        public List<LimitViewModel> GetAllLimitData()
        {
            try
            {
                var list = _limitRepository.List();
                List<LimitViewModel> limitList = new List<LimitViewModel>();
                foreach (var model in list)
                {
                    limitList.Add(new LimitViewModel()
                    {
                        Id = model.Id,
                        MaxAmt = model.MaxAmt,
                        MaxAmtDaily = model.MaxAmtDaily,
                        MaxAmtMonthly = model.MaxAmtMonthly,
                        MaxAmtWeekly = model.MaxAmtWeekly,
                        Maxrange = model.Maxrange,
                        MaxRangeDaily = model.MaxRangeDaily,
                        MaxRangeMonthly = model.MaxRangeMonthly,
                        MaxRangeWeekly = model.MaxRangeWeekly,
                        MinAmt = model.MinAmt,
                        MinAmtDaily = model.MinAmtDaily,
                        MinAmtMonthly = model.MinAmtMonthly,
                        MinAmtWeekly = model.MinAmtWeekly,
                        MinRange = model.MinRange,
                        MinRangeDaily = model.MinRangeDaily,
                        MinRangeMonthly = model.MinRangeMonthly,
                        MinRangeWeekly = model.MinRangeWeekly,
                        Status = model.Status

                    });
                }

                return limitList;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public LimitViewModel GetLimitById(long id)
        {
            try
            {
                var model = _limitRepository.GetById(id);
                if (model == null)
                    return null;

                var viewModel = new LimitViewModel()
                {
                    Id = model.Id,
                    MaxAmt = model.MaxAmt,
                    MaxAmtDaily = model.MaxAmtDaily,
                    MaxAmtMonthly = model.MaxAmtMonthly,
                    MaxAmtWeekly = model.MaxAmtWeekly,
                    Maxrange = model.Maxrange,
                    MaxRangeDaily = model.MaxRangeDaily,
                    MaxRangeMonthly = model.MaxRangeMonthly,
                    MaxRangeWeekly = model.MaxRangeWeekly,
                    MinAmt = model.MinAmt,
                    MinAmtDaily = model.MinAmtDaily,
                    MinAmtMonthly = model.MinAmtMonthly,
                    MinAmtWeekly = model.MinAmtWeekly,
                    MinRange = model.MinRange,
                    MinRangeDaily = model.MinRangeDaily,
                    MinRangeMonthly = model.MinRangeMonthly,
                    MinRangeWeekly = model.MinRangeWeekly,
                    Status = model.Status
                };
                return viewModel;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long AddLimitData(LimitRequest Request, long UserId)
        {
            try
            {
                var model = new Limits()
                {
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST(),
                    Status = Request.Status,
                    MaxAmt = Request.MaxAmt,
                    MaxAmtDaily = Request.MaxAmtDaily,
                    MaxAmtMonthly = Request.MaxAmtMonthly,
                    MaxAmtWeekly = Request.MaxAmtWeekly,
                    Maxrange = Request.Maxrange,
                    MaxRangeDaily = Request.MaxRangeDaily,
                    MaxRangeMonthly = Request.MaxRangeMonthly,
                    MaxRangeWeekly = Request.MaxRangeWeekly,
                    MinAmt = Request.MinAmt,
                    MinAmtDaily = Request.MinAmtDaily,
                    MinAmtMonthly = Request.MinAmtMonthly,
                    MinAmtWeekly = Request.MinAmtWeekly,
                    MinRange = Request.MinRange,
                    MinRangeDaily = Request.MinRangeDaily,
                    MinRangeMonthly = Request.MinRangeMonthly,
                    MinRangeWeekly = Request.MinRangeWeekly
                };
                var newModel = _limitRepository.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool UpdateLimitData(LimitRequest Request, long UserId)
        {
            try
            {
                var model = _limitRepository.GetById(Request.Id);
                if (model == null)
                    return false;

                model.UpdatedBy = UserId;
                model.UpdatedDate = _basePage.UTC_To_IST();
                model.MaxAmt = Request.MaxAmt;
                model.MaxAmtDaily = Request.MaxAmtDaily;
                model.MaxAmtMonthly = Request.MaxAmtMonthly;
                model.MaxAmtWeekly = Request.MaxAmtWeekly;
                model.Maxrange = Request.Maxrange;
                model.MaxRangeDaily = Request.MaxRangeDaily;
                model.MaxRangeMonthly = Request.MaxRangeMonthly;
                model.MaxRangeWeekly = Request.MaxRangeWeekly;
                model.MinAmt = Request.MinAmt;
                model.MinAmtDaily = Request.MinAmtDaily;
                model.MinAmtMonthly = Request.MinAmtMonthly;
                model.MinAmtWeekly = Request.MinAmtWeekly;
                model.MinRange = Request.MinRange;
                model.MinRangeDaily = Request.MinRangeDaily;
                model.MinRangeMonthly = Request.MinRangeMonthly;
                model.MinRangeWeekly = Request.MinRangeWeekly;
                model.Status = Request.Status;
                _limitRepository.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetActiveLimit(long id)
        {
            try
            {
                Limits model = _limitRepository.GetById(id);
                if (model != null)
                {
                    model.SetActiveLimit();
                    _limitRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetInActiveLimit(long id)
        {
            try
            {
                Limits model = _limitRepository.GetById(id);
                if (model != null)
                {
                    model.SetInActiveLimit();
                    _limitRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Market
        public MarketViewModel AddMarketData(MarketViewModel viewModel, long UserId)
        {
            try
            {
                Market market = new Market
                {
                    CurrencyName = viewModel.CurrencyName,
                    ServiceID = viewModel.ServiceID,
                    isBaseCurrency = 1,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST()
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };
                var newMarket = _marketRepository.Add(market);

                return viewModel;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<MarketViewModel> GetAllMarketData(short ActiveOnly = 1)
        {
            List<Market> modellist = new List<Market>();
            try
            {
                List<MarketViewModel> list = new List<MarketViewModel>();
                modellist = _marketRepository.List();

                if (ActiveOnly == 1)
                    modellist = modellist.Where(e => e.Status == 1).ToList();

                foreach (var model in modellist)
                {
                    list.Add(new MarketViewModel()
                    {
                        ID = model.Id,
                        CurrencyName = model.CurrencyName,
                        ServiceID = model.ServiceID,
                        isBaseCurrency = model.isBaseCurrency,
                        Status = Enum.GetName(typeof(ServiceStatus), model.Status),
                        CurrencyDesc = _serviceMasterRepository.GetSingle(s => s.Id == model.ServiceID).Name,
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<MarketViewModel> GetAllMarketDataMargin(short ActiveOnly = 1)
        {
            List<MarketMargin> modellist = new List<MarketMargin>();
            try
            {
                List<MarketViewModel> list = new List<MarketViewModel>();
                modellist = _marketRepositoryMargin.List();

                if (ActiveOnly == 1)
                    modellist = modellist.Where(e => e.Status == 1).ToList();

                foreach (var model in modellist)
                {
                    list.Add(new MarketViewModel()
                    {
                        ID = model.Id,
                        CurrencyName = model.CurrencyName,
                        ServiceID = model.ServiceID,
                        isBaseCurrency = model.isBaseCurrency,
                        Status = Enum.GetName(typeof(ServiceStatus), model.Status),
                        CurrencyDesc = _serviceMasterRepositoryMargin.GetSingle(s => s.Id == model.ServiceID).Name,
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

        public MarketViewModel GetMarketDataByMarket(long Id)
        {
            try
            {
                MarketViewModel marketView = new MarketViewModel();
                var model = _marketRepository.GetById(Id);
                if (model == null)
                    return null;

                marketView.CurrencyName = model.CurrencyName;
                marketView.ID = model.Id;
                marketView.isBaseCurrency = model.isBaseCurrency;
                marketView.ServiceID = model.ServiceID;
                return marketView;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass AddMarketDataV2(MarketDataRequest Request, long UserId)
        {
            try
            {
                BizResponseClass res = new BizResponseClass();
                var obj = _marketRepository.GetSingle(e => e.ServiceID == Request.ServiceID);
                if (obj != null)
                {
                    if (obj.ServiceID == Request.ServiceID)
                    {
                        res.ErrorCode = enErrorCode.MarketAlreadyExist;
                        res.ReturnCode = enResponseCode.Fail;
                        return res;
                    }
                }
                Market market = new Market
                {
                    CurrencyName = Request.CurrencyName,
                    ServiceID = Request.ServiceID,
                    isBaseCurrency = 1,
                    Status = Request.Status,
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST()
                };
                var newMarket = _marketRepository.Add(market);
                if (newMarket.Id != 0)
                {
                    res.ErrorCode = enErrorCode.Success;
                    res.ReturnCode = enResponseCode.Success;
                    return res;
                }
                res.ErrorCode = enErrorCode.DataInsertFail;
                res.ReturnCode = enResponseCode.Fail;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 3-5-19 for margin Trading
        public BizResponseClass AddMarketDataV2Margin(MarketDataRequest Request, long UserId)
        {
            try
            {
                BizResponseClass res = new BizResponseClass();
                var obj = _marketRepositoryMargin.GetSingle(e => e.ServiceID == Request.ServiceID);
                if (obj != null)
                {
                    if (obj.ServiceID == Request.ServiceID)
                    {
                        res.ErrorCode = enErrorCode.MarketAlreadyExist;
                        res.ReturnCode = enResponseCode.Fail;
                        return res;
                    }
                }
                MarketMargin market = new MarketMargin
                {
                    CurrencyName = Request.CurrencyName,
                    ServiceID = Request.ServiceID,
                    isBaseCurrency = 1,
                    Status = Request.Status,
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST()
                };
                var newMarket = _marketRepositoryMargin.Add(market);
                if (newMarket.Id != 0)
                {
                    res.ErrorCode = enErrorCode.Success;
                    res.ReturnCode = enResponseCode.Success;
                    return res;
                }
                res.ErrorCode = enErrorCode.DataInsertFail;
                res.ReturnCode = enResponseCode.Fail;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public MarketDataResponse UpdateMarketDataV2(MarketDataRequest Request, long UserId)
        {
            try
            {
                MarketDataResponse res = new MarketDataResponse();
                var model = _marketRepository.GetById(Request.ID);
                if (model == null)
                {
                    res.Response = null;
                    res.ErrorCode = enErrorCode.DataInsertFail;
                    res.ReturnCode = enResponseCode.Fail;
                    return res;
                }
                //model.ServiceID = Request.ServiceID;
                model.Status = Request.Status;
                // model.CurrencyName = Request.CurrencyName;
                model.isBaseCurrency = 1;
                model.UpdatedBy = UserId;
                model.UpdatedDate = DateTime.UtcNow;

                _marketRepository.Update(model);

                model = _marketRepository.GetById(Request.ID);
                MarketInfo market = new MarketInfo();
                market.ID = model.Id;
                market.ServiceID = model.ServiceID;
                market.Status = model.Status;
                market.CurrencyName = model.CurrencyName;

                res.Response = market;
                res.ErrorCode = enErrorCode.Success;
                res.ReturnCode = enResponseCode.Success;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 3-5-19 for margin Trading
        public MarketDataResponse UpdateMarketDataV2Margin(MarketDataRequest Request, long UserId)
        {
            try
            {
                MarketDataResponse res = new MarketDataResponse();
                var model = _marketRepositoryMargin.GetById(Request.ID);
                if (model == null)
                {
                    res.Response = null;
                    res.ErrorCode = enErrorCode.DataInsertFail;
                    res.ReturnCode = enResponseCode.Fail;
                    return res;
                }
                //model.ServiceID = Request.ServiceID;
                model.Status = Request.Status;
                // model.CurrencyName = Request.CurrencyName;
                model.isBaseCurrency = 1;
                model.UpdatedBy = UserId;
                model.UpdatedDate = DateTime.UtcNow;

                _marketRepositoryMargin.Update(model);

                model = _marketRepositoryMargin.GetById(Request.ID);
                MarketInfo market = new MarketInfo();
                market.ID = model.Id;
                market.ServiceID = model.ServiceID;
                market.Status = model.Status;
                market.CurrencyName = model.CurrencyName;

                res.Response = market;
                res.ErrorCode = enErrorCode.Success;
                res.ReturnCode = enResponseCode.Success;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region Liquidity API Manager
        public int AddLiquidityAPIManager(LiquidityAPIManagerRequest Request, long UserId)
        {
            try
            {
                //Get ThirdParty API Data
                var ThirdPartyObj = _thirdPartyAPIRepository.GetById(Request.APIProviderId);
                var ServiceProviderMasterObj = _ServiceProviderMaster.GetById(Request.ProviderMasterId);
                var ServiceProviderType = _ProviderTypeRepository.GetById(Request.ProviderTypeId);

                if (ThirdPartyObj != null)
                {
                    for (int i = 0; i < Request.TransationType.Length; i++)
                    {
                        var Data = _ProDetailRepository.GetSingle(x => x.ServiceProID == Request.ProviderMasterId && x.ProTypeID == Request.ProviderTypeId && x.AppTypeID == ThirdPartyObj.AppType && x.TrnTypeID == Request.TransationType[i] && x.LimitID == Request.LimitId && x.DemonConfigID == Request.DeamonConfigId && x.ServiceProConfigID == Request.ServiceProviderCongigId && x.ThirPartyAPIID == Request.APIProviderId);

                        if (Data != null)
                        {
                            continue;
                        }

                        ProviderDetailRequest providerDetailRequest = new ProviderDetailRequest();

                        providerDetailRequest.SerProDetailName = ServiceProviderMasterObj.ProviderName + "_" + (enTrnType)Request.TransationType[i] + "_" + ServiceProviderType.ServiveProTypeName;
                        providerDetailRequest.ServiceProID = Request.ProviderMasterId;
                        providerDetailRequest.ProTypeID = Request.ProviderTypeId;
                        providerDetailRequest.AppTypeID = ThirdPartyObj.AppType;
                        providerDetailRequest.TrnTypeID = Request.TransationType[i];
                        providerDetailRequest.LimitID = Request.LimitId;
                        providerDetailRequest.DemonConfigID = Request.DeamonConfigId;
                        providerDetailRequest.ServiceProConfigID = Request.ServiceProviderCongigId;
                        providerDetailRequest.ThirPartyAPIID = Request.APIProviderId;
                        providerDetailRequest.Status = Request.Status;

                        AddProviderDetail(providerDetailRequest, UserId);
                    }
                    return 1;
                }

                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;

            }
        }

        public List<LiquidityAPIManagerData> GetAllLiquidityAPIManager()
        {
            try
            {
                List<LiquidityAPIManagerData> list = new List<LiquidityAPIManagerData>();

                var ProviderData = _ProDetailRepository.List();

                foreach (var ProData in ProviderData)
                {
                    LiquidityAPIManagerData Data = new LiquidityAPIManagerData();

                    Data.APIProviderId = ProData.ThirPartyAPIID;
                    Data.DeamonConfigId = ProData.DemonConfigID;
                    Data.Id = ProData.Id;
                    Data.LimitId = ProData.LimitID;
                    Data.Name = ProData.SerProDetailName;
                    Data.ProviderTypeId = ProData.ProTypeID;
                    Data.TransationType = Convert.ToInt32(ProData.TrnTypeID);
                    Data.ServiceProviderCongigId = ProData.ServiceProConfigID;
                    Data.Status = ProData.Status;
                    Data.StatusText = ((ServiceStatus)ProData.Status).ToString();
                    Data.TransactionTypeName = ((enTrnType)ProData.TrnTypeID).ToString();
                    Data.ProviderMasterId = ProData.ServiceProID;

                    list.Add(Data);
                }
                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public LiquidityAPIManagerData GetLiquidityAPIManager(long Id)
        {
            try
            {
                LiquidityAPIManagerData Data = new LiquidityAPIManagerData();

                var ProviderData = _ProDetailRepository.GetById(Id);

                if (ProviderData != null)
                {
                    Data.APIProviderId = ProviderData.ThirPartyAPIID;
                    Data.DeamonConfigId = ProviderData.DemonConfigID;
                    Data.Id = ProviderData.Id;
                    Data.LimitId = ProviderData.LimitID;
                    Data.Name = ProviderData.SerProDetailName;
                    Data.ProviderTypeId = ProviderData.ProTypeID;
                    Data.ServiceProviderCongigId = ProviderData.ServiceProConfigID;
                    Data.Status = ProviderData.Status;
                    Data.StatusText = ((ServiceStatus)ProviderData.Status).ToString();
                    Data.TransactionTypeName = ((enTrnType)ProviderData.Status).ToString();
                    Data.ProviderMasterId = ProviderData.ServiceProID;
                    Data.TransationType = Convert.ToInt32(ProviderData.TrnTypeID);

                    return Data;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int UpdateLiquidityAPIManager(LiquidityAPIManagerUpdateRequest Request, long UserId)
        {
            try
            {
                //Get ThirdParty API Data

                var ThirdPartyObj = _thirdPartyAPIRepository.GetById(Request.APIProviderId);
                var ServiceProviderMasterObj = _ServiceProviderMaster.GetById(Request.ProviderMasterId);
                var ServiceProviderType = _ProviderTypeRepository.GetById(Request.ProviderTypeId);

                if (ThirdPartyObj != null)
                {
                    ProviderDetailRequest providerDetailRequest = new ProviderDetailRequest();

                    providerDetailRequest.Id = Request.Id;
                    providerDetailRequest.SerProDetailName = ServiceProviderMasterObj.ProviderName + "_" + (enTrnType)Request.TransationType + "_" + ServiceProviderType.ServiveProTypeName;
                    providerDetailRequest.ServiceProID = Request.ProviderMasterId;
                    providerDetailRequest.ProTypeID = Request.ProviderTypeId;
                    providerDetailRequest.AppTypeID = ThirdPartyObj.AppType;
                    providerDetailRequest.TrnTypeID = Request.TransationType;
                    providerDetailRequest.LimitID = Request.LimitId;
                    providerDetailRequest.DemonConfigID = Request.DeamonConfigId;
                    providerDetailRequest.ServiceProConfigID = Request.ServiceProviderCongigId;
                    providerDetailRequest.ThirPartyAPIID = Request.APIProviderId;
                    providerDetailRequest.Status = Request.Status;
                    //Rushabh 16-01-2020 Changes done as per the change in UpdateProviderDetail method
                    var response = UpdateProviderDetail(providerDetailRequest, UserId);
                    //if (UpdateProviderDetail(providerDetailRequest, UserId))
                    //{
                    //    return 1;
                    //}
                    if (response.ReturnCode == enResponseCode.Success && response.ErrorCode == enErrorCode.Success)
                    {
                        return 1;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Trade Routing
        public long AddTradeRouteConfiguration(TradeRouteConfigRequest Request, long UserId)
        {
            try
            {
                //Check Already Available Or Not
                var Data = _routeConfigRepository.GetSingle(x => x.PairId == Request.PairId && x.OrderType == Request.OrderType && x.TrnType == (enTrnType)Request.TrnType && x.SerProDetailID == Request.RouteUrlId);
                if (Data != null)
                {
                    return -1;
                }
                else
                {
                    var PairData = _tradePairMasterRepository.GetById(Request.PairId);

                    RouteConfiguration route = new RouteConfiguration()
                    {
                        RouteName = PairData.PairName + "_" + (enTrnType)Request.TrnType,
                        ServiceID = 0,
                        SerProDetailID = Request.RouteUrlId,
                        ProductID = 0,
                        Priority = 0,
                        StatusCheckUrl = null,
                        ValidationUrl = null,
                        TransactionUrl = null,
                        LimitId = 0,
                        OpCode = Request.AssetName,
                        TrnType = (enTrnType)Request.TrnType,
                        IsDelayAddress = 0,
                        ProviderWalletID = "0",
                        Status = Convert.ToInt16(ServiceStatus.Active),
                        PairId = Request.PairId,
                        OrderType = Request.OrderType,
                        CreatedDate = _basePage.UTC_To_IST(),
                        CreatedBy = UserId,
                        ConfirmationCount = Request.ConfirmationCount,
                        ConvertAmount = Request.ConvertAmount
                    };
                    var newProduct = _routeConfigRepository.Add(route);

                    return newProduct.Id;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;

            }
        }

        public long UpdateTradeRouteConfiguration(TradeRouteConfigRequest Request, long UserId)
        {
            try
            {
                //Check Same As Previous Data
                var PreviousData = _routeConfigRepository.GetById(Request.Id);
                if (PreviousData != null)
                {
                    if (PreviousData.PairId == Request.PairId && PreviousData.TrnType == (enTrnType)Request.TrnType && PreviousData.OrderType == Request.OrderType)
                    {
                        PreviousData.OpCode = Request.AssetName;
                        PreviousData.Status = Convert.ToInt16(ServiceStatus.Active);
                        PreviousData.ConfirmationCount = Request.ConfirmationCount;
                        PreviousData.ConvertAmount = Request.ConvertAmount;

                        _routeConfigRepository.Update(PreviousData);

                        return PreviousData.Id;
                    }
                }

                //Check Already Available Or Not
                var Data = _routeConfigRepository.GetSingle(x => x.PairId == Request.PairId && x.OrderType == Request.OrderType && x.TrnType == (enTrnType)Request.TrnType && x.SerProDetailID == Request.RouteUrlId);
                if (Data != null)
                {
                    return -1;
                }
                else
                {
                    var PairData = _tradePairMasterRepository.GetById(Request.PairId);
                    var RouteConfigurationData = _routeConfigRepository.GetById(Request.Id);

                    if (RouteConfigurationData != null)
                    {
                        RouteConfigurationData.RouteName = PairData.PairName + "_" + (enTrnType)Request.TrnType;
                        RouteConfigurationData.OpCode = Request.AssetName;
                        RouteConfigurationData.TrnType = (enTrnType)Request.TrnType;
                        RouteConfigurationData.Status = Convert.ToInt16(ServiceStatus.Active);
                        RouteConfigurationData.PairId = Request.PairId;
                        RouteConfigurationData.OrderType = Request.OrderType;
                        RouteConfigurationData.ConfirmationCount = Request.ConfirmationCount;
                        RouteConfigurationData.ConvertAmount = Request.ConvertAmount;

                        _routeConfigRepository.Update(RouteConfigurationData);

                        return RouteConfigurationData.Id;
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetTradeRouteConfigurationData> GetAllTradeRouteConfiguration()
        {
            try
            {
                var Data = _backOfficeTrnRepository.GetTradeRouteConfiguration(0);

                var Data1 = from TradeRoute in Data
                            select new GetTradeRouteConfigurationData
                            {
                                Id = TradeRoute.Id,
                                AssetName = TradeRoute.AssetName,
                                PairId = TradeRoute.PairId,
                                PairName = TradeRoute.PairName,
                                OrderType = TradeRoute.OrderType,
                                OrderTypeText = ((enTransactionMarketType)TradeRoute.OrderType).ToString(),
                                Status = TradeRoute.Status,
                                StatusText = ((ServiceStatus)TradeRoute.Status).ToString(),
                                TrnType = TradeRoute.TrnType,
                                TrnTypeText = ((enTrnType)TradeRoute.TrnType).ToString(),
                                TradeRouteName = TradeRoute.TradeRouteName,
                                ConfirmationCount = TradeRoute.ConfirmationCount,
                                ConvertAmount = TradeRoute.ConvertAmount,
                                Priority = TradeRoute.Priority,
                                RouteUrl = TradeRoute.RouteUrl,
                                RouteUrlId = TradeRoute.RouteUrlId
                            };

                return Data1.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public GetTradeRouteConfigurationData GetTradeRouteConfiguration(long Id)
        {
            try
            {
                var Data = _backOfficeTrnRepository.GetTradeRouteConfiguration(Id);
                if (Data.Count > 0)
                {
                    var Data1 = from TradeRoute in Data
                                select new GetTradeRouteConfigurationData
                                {
                                    Id = TradeRoute.Id,
                                    AssetName = TradeRoute.AssetName,
                                    PairId = TradeRoute.PairId,
                                    PairName = TradeRoute.PairName,
                                    OrderType = TradeRoute.OrderType,
                                    OrderTypeText = ((enTransactionMarketType)TradeRoute.OrderType).ToString(),
                                    Status = TradeRoute.Status,
                                    StatusText = ((ServiceStatus)TradeRoute.Status).ToString(),
                                    TrnType = TradeRoute.TrnType,
                                    TrnTypeText = ((enTrnType)TradeRoute.TrnType).ToString(),
                                    TradeRouteName = TradeRoute.TradeRouteName,
                                    ConfirmationCount = TradeRoute.ConfirmationCount,
                                    ConvertAmount = TradeRoute.ConvertAmount,
                                    Priority = TradeRoute.Priority,
                                    RouteUrl = TradeRoute.RouteUrl,
                                    RouteUrlId = TradeRoute.RouteUrlId
                                };

                    return Data1.ToList().FirstOrDefault();
                }

                return null;
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
                var Data = _backOfficeTrnRepository.GetAvailableTradeRoute(TrnType);
                return Data;
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
                var Data = _backOfficeTrnRepository.GetTradeRouteForPriority(PairId, OrderType, TrnType);

                var Data1 = from TradeRoute in Data
                            select new GetTradeRouteConfigurationData
                            {
                                Id = TradeRoute.Id,
                                AssetName = TradeRoute.AssetName,
                                PairId = TradeRoute.PairId,
                                PairName = TradeRoute.PairName,
                                OrderType = TradeRoute.OrderType,
                                OrderTypeText = ((enTransactionMarketType)TradeRoute.OrderType).ToString(),
                                Status = TradeRoute.Status,
                                StatusText = ((ServiceStatus)TradeRoute.Status).ToString(),
                                TrnType = TradeRoute.TrnType,
                                TrnTypeText = ((enTrnType)TradeRoute.TrnType).ToString(),
                                TradeRouteName = TradeRoute.TradeRouteName,
                                ConfirmationCount = TradeRoute.ConfirmationCount,
                                ConvertAmount = TradeRoute.ConvertAmount,
                                Priority = TradeRoute.Priority,
                                RouteUrl = TradeRoute.RouteUrl,
                                RouteUrlId = TradeRoute.RouteUrlId
                            };

                return Data1.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public long UpdateTradeRoutePriority(TradeRoutePriorityUpdateRequest Request, long UserId)
        {
            try
            {
                foreach (var TradeRoute in Request.TradeRoute)
                {
                    var Data = _routeConfigRepository.GetById(TradeRoute.Id);

                    if (Data != null)
                    {
                        Data.Priority = TradeRoute.Priority;
                        _routeConfigRepository.Update(Data);
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region MarketTicker Configuration
        public List<MarketTickerPairData> GetMarketTickerPairData(short IsMargin = 0)//Rita 5-3-19 for Margin Trading)
        {
            try
            {
                List<MarketTickerPairData> Data;
                if (IsMargin == 1)
                    Data = _backOfficeTrnRepository.GetMarketTickerPairDataMargin();
                else
                    Data = _backOfficeTrnRepository.GetMarketTickerPairData();

                return Data;
            }
            catch (Exception ex)

            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int UpdateMarketTickerPairData(UpdateMarketTickerPairData Request, long UserId, short IsMargin = 0)
        {
            try
            {
                if (Request.Request.Count > 0)
                {
                    List<long> PairId = new List<long>();

                    foreach (var pair in Request.Request)
                    {
                        if (pair.IsMarketTicker == 1)
                            PairId.Add(pair.PairId);
                    }

                    if (IsMargin == 1)
                        _backOfficeTrnRepository.UpdateMarketTickerPairDataMargin(PairId, UserId);
                    else
                        _backOfficeTrnRepository.UpdateMarketTickerPairData(PairId, UserId);


                    //Uday 25-12-2018 SingalR Call For Front Update
                    try
                    {
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_configuration["FrontUrl"].ToString() + "api/Transaction/GetMarketTickerSignalR?IsMargin=" + IsMargin);
                        httpWebRequest.Method = "GET";
                        string responseText = string.Empty;
                        HttpWebResponse httpResponse;

                        httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            responseText = streamReader.ReadToEnd();
                        }
                    }
                    catch (Exception ex)
                    {
                        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + "  #MarketTicker SignalR Update#", this.GetType().Name, ex);
                    }

                    //Call Fornt Side API For SignalR Update So Comment this code
                    //var Data = _backOfficeTrnRepository.GetUpdatedMarketTicker();
                    //if (Data.Count != 0)
                    //    _signalRService.MarketTicker(Data);

                }

                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region CoinListRequest

        public BizResponseClass AddCoinRequest(CoinListRequestRequest Request, long UserID)
        {
            try
            {
                BizResponseClass _Res = new BizResponseClass();
                var CoinMaster = _serviceMasterRepository.FindBy(e => e.Name == Request.CoinName || e.SMSCode == Request.CoinAbbreviationCode).ToList();
                if (CoinMaster.Count > 0)
                {
                    _Res.ErrorCode = enErrorCode.InvalidCoinOrToken;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Duplicate Coin Request.";
                    return _Res;
                }
                CoinListRequest Model = new CoinListRequest();
                Model.IconUrl = Request.ImageUrl;
                Model.CoinType = Request.CoinType;
                Model.CoinName = Request.CoinName;
                Model.CoinAbbreviationCode = Request.CoinAbbreviationCode;
                Model.Introduction = Request.Introduction;
                Model.CoinTokenAddress = Request.CoinTokenAddress;
                Model.APIDocumentPath = Request.APIDocumentPath;
                Model.GithubLink = Request.GithubLink;
                Model.DecimalPlace = Request.DecimalPlace;
                Model.TotalSupply = Request.TotalSupply;
                Model.CirculatingSupply = Request.CirculatingSupply;
                Model.MaxSupply = Request.MaxSupply;
                Model.TrnFee = Request.TrnFee;
                Model.IssueDate = Request.IssueDate;
                Model.IssuePrice = Request.IssuePrice;
                Model.FirstName = Request.FirstName;
                Model.LastName = Request.LastName;
                Model.Address = Request.Address;
                Model.StreetAddress = Request.StreetAddress;
                Model.AddressLine2 = Request.AddressLine2;
                Model.City = Request.City;
                Model.State = Request.State;
                Model.ZipCode = Request.ZipCode;
                Model.Phone = Request.Phone;
                Model.Email = Request.Email;
                Model.ProjectName = Request.ProjectName;
                Model.ProjectWebsiteLink = Request.ProjectWebsiteLink;
                Model.Community = JsonConvert.SerializeObject(Request.Community);
                Model.Explorer = JsonConvert.SerializeObject(Request.Explorer);
                Model.HowFundsWereRaised = Request.HowFundsWereRaised;
                Model.Premine = Request.Premine;
                Model.CurListOnOtherExng = Request.CurListOnOtherExng;
                Model.WebsiteFAQ = Request.WebsiteFAQ;
                Model.WebsiteUrl = Request.WebsiteUrl;
                Model.WhitePaper = Request.WhitePaper;
                Model.Status = 0;// Request.Status; 0-User Req
                Model.CreatedBy = UserID;
                Model.CreatedDate = DateTime.UtcNow;
                try
                {
                    _coinListRequestRepository.Add(Model);
                }
                catch (Exception e)
                {
                    _Res.ErrorCode = enErrorCode.DataInsertFail;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Insert Fail.";
                    return _Res;
                }
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Insert Successfully.";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public CoinListRequestResponse GetUserCoinRequest(long UserId)
        {
            try
            {
                CoinListRequestResponse _Res = new CoinListRequestResponse();
                List<CoinListRequestViewModel> coinList = new List<CoinListRequestViewModel>();
                var list = _coinListRequestRepository.FindBy(e => e.CreatedBy == UserId);
                if (list.ToList().Count == 0)
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "No Data Found";
                    return _Res;
                }
                foreach (var Model in list)
                {
                    coinList.Add(new CoinListRequestViewModel()
                    {
                        ID = Model.Id,
                        CirculatingSupply = Model.CirculatingSupply,
                        CoinAbbreviationCode = Model.CoinAbbreviationCode,
                        CoinName = Model.CoinName,
                        Community = JsonConvert.DeserializeObject<List<CommunityData>>(Model.Community),
                        Explorer = JsonConvert.DeserializeObject<List<ExplorerData>>(Model.Explorer),
                        ImageUrl = Model.IconUrl,
                        Introduction = Model.Introduction,
                        IssueDate = Model.IssueDate,
                        IssuePrice = Model.IssuePrice,
                        MaxSupply = Model.MaxSupply,
                        TotalSupply = Model.TotalSupply,
                        WebsiteUrl = Model.WebsiteUrl,
                        Address = Model.Address,
                        AddressLine2 = Model.AddressLine2,
                        APIDocumentPath = Model.APIDocumentPath,
                        WhitePaper = Model.WhitePaper,
                        WebsiteFAQ = Model.WebsiteFAQ,
                        City = Model.City,
                        CoinTokenAddress = Model.CoinTokenAddress,
                        CoinType = Model.CoinType,
                        CurListOnOtherExng = Model.CurListOnOtherExng,
                        DecimalPlace = Model.DecimalPlace,
                        Email = Model.Email,
                        FirstName = Model.FirstName,
                        GithubLink = Model.GithubLink,
                        HowFundsWereRaised = Model.HowFundsWereRaised,
                        LastName = Model.LastName,
                        Phone = Model.Phone,
                        Premine = Model.Premine,
                        ProjectName = Model.ProjectName,
                        ProjectWebsiteLink = Model.ProjectWebsiteLink,
                        State = Model.State,
                        StreetAddress = Model.StreetAddress,
                        TrnFee = Model.TrnFee,
                        ZipCode = Model.ZipCode,
                        Status = Model.Status
                    });
                }
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.Response = coinList;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public CoinListRequestResponse GetAllCoinRequest(GetCoinRequestListRequest Request)
        {
            try
            {
                short Status = 999;
                CoinListRequestResponse _Res = new CoinListRequestResponse();
                List<CoinListRequestViewModel> coinList = new List<CoinListRequestViewModel>();
                List<CoinListRequest> list = new List<CoinListRequest>();
                if (string.IsNullOrEmpty(Request.Status))
                    Status = 999;
                if (Request.Status == "Request")
                    Status = 0;
                if (Request.Status == "Accept")
                    Status = 1;
                if (Request.Status == "Reject")
                    Status = 2;

                if (Request.ID != 0 && Status != 999)
                {
                    list = _coinListRequestRepository.FindBy(e => e.Id == Request.ID && e.Status == Status).ToList();
                }
                else if (Request.ID != 0)
                {
                    list = _coinListRequestRepository.FindBy(e => e.Id == Request.ID).ToList();
                }
                else if (Status != 999)
                {
                    list = _coinListRequestRepository.FindBy(e => e.Status == Status).ToList();
                }
                else
                {
                    list = _coinListRequestRepository.GetAll().ToList();
                }

                if (list.ToList().Count == 0)
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "No Data Found";
                    return _Res;
                }
                foreach (var Model in list)
                {
                    coinList.Add(new CoinListRequestViewModel()
                    {
                        ID = Model.Id,
                        CirculatingSupply = Model.CirculatingSupply,
                        CoinAbbreviationCode = Model.CoinAbbreviationCode,
                        CoinName = Model.CoinName,
                        Community = JsonConvert.DeserializeObject<List<CommunityData>>(Model.Community),
                        Explorer = JsonConvert.DeserializeObject<List<ExplorerData>>(Model.Explorer),
                        ImageUrl = Model.IconUrl,
                        Introduction = Model.Introduction,
                        IssueDate = Model.IssueDate,
                        IssuePrice = Model.IssuePrice,
                        MaxSupply = Model.MaxSupply,
                        TotalSupply = Model.TotalSupply,
                        WebsiteUrl = Model.WebsiteUrl,
                        Address = Model.Address,
                        AddressLine2 = Model.AddressLine2,
                        APIDocumentPath = Model.APIDocumentPath,
                        WhitePaper = Model.WhitePaper,
                        WebsiteFAQ = Model.WebsiteFAQ,
                        City = Model.City,
                        CoinTokenAddress = Model.CoinTokenAddress,
                        CoinType = Model.CoinType,
                        CurListOnOtherExng = Model.CurListOnOtherExng,
                        DecimalPlace = Model.DecimalPlace,
                        Email = Model.Email,
                        FirstName = Model.FirstName,
                        GithubLink = Model.GithubLink,
                        HowFundsWereRaised = Model.HowFundsWereRaised,
                        LastName = Model.LastName,
                        Phone = Model.Phone,
                        Premine = Model.Premine,
                        ProjectName = Model.ProjectName,
                        ProjectWebsiteLink = Model.ProjectWebsiteLink,
                        State = Model.State,
                        StreetAddress = Model.StreetAddress,
                        TrnFee = Model.TrnFee,
                        ZipCode = Model.ZipCode,
                        Status = Model.Status
                    });
                }
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.Response = coinList;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass SetCoinRequestStatus(SetCoinRequestStatusRequest Request, long UserID)
        {
            try
            {
                BizResponseClass _Res = new BizResponseClass();
                var Model = _coinListRequestRepository.GetById(Request.ID);
                if (Model == null)
                {
                    _Res.ErrorCode = enErrorCode.CoinReqIDNotFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Coin Req ID NotFound";
                    return _Res;
                }
                Model.Status = Request.Status;
                Model.UpdatedBy = UserID;
                Model.UpdatedDate = DateTime.UtcNow;

                _coinListRequestRepository.Update(Model);
                if (Request.Status == 1)
                {
                    ServiceConfigurationRequestNew AddCoin = new ServiceConfigurationRequestNew()
                    {
                        CirculatingSupply = Convert.ToInt64(Model.CirculatingSupply),
                        Status = 1,
                        Introduction = Model.Introduction,
                        Community = JsonConvert.DeserializeObject<List<CommunityData>>(Model.Community),
                        Explorer = JsonConvert.DeserializeObject<List<ExplorerData>>(Model.Explorer),
                        IsBaseCurrency = 0,
                        IsDeposit = 0,
                        IssueDate = Model.IssueDate,
                        IssuePrice = Model.IssuePrice,
                        IsTransaction = 0,
                        IsWithdraw = 0,
                        MaxSupply = Convert.ToInt64(Model.MaxSupply),
                        Name = Model.CoinName,
                        SMSCode = Model.CoinAbbreviationCode,
                        TotalSupply = Convert.ToInt64(Model.TotalSupply),
                        WebsiteUrl = Model.WebsiteUrl
                    };
                    var res = this.AddServiceConfiguration(AddCoin, UserID);
                }
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

        #region SiteToken

        public SiteTokenTypeResponse GetSiteTokenType()
        {
            SiteTokenTypeResponse _Res = new SiteTokenTypeResponse();
            List<SiteTokenTypeInfo> siteTokens = new List<SiteTokenTypeInfo>();
            try
            {
                var modelList = _SiteTokenRateTypeRepository.FindBy(e => e.Status == 1);
                if (modelList == null)
                {
                    _Res.Response = siteTokens;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                foreach (var model in modelList)
                {
                    siteTokens.Add(new SiteTokenTypeInfo()
                    {
                        Id = model.Id,
                        SiteTokenType = model.TokenType,
                        Status = model.Status
                    });
                }
                _Res.Response = siteTokens;
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

        public BizResponseClass AddSiteToken(SiteTokenMasterRequest request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            TradePairMaster TradePairObj = new TradePairMaster();

            try
            {
                var IsExistService = _serviceMasterRepository.FindBy(e => e.Id == request.CurrencyID && e.Status != 9).SingleOrDefault();
                if (IsExistService == null)
                {
                    _Res.ErrorCode = enErrorCode.InValid_CurrencyID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Enter Valid Currency ID";
                    return _Res;
                }
                var IsExistBaseService = _serviceMasterRepository.FindBy(e => e.Id == request.BaseCurrencyID && e.Status != 9).SingleOrDefault();
                if (IsExistBaseService == null)
                {
                    _Res.ErrorCode = enErrorCode.InValid_BaseCurrencyID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Enter Valid Base Currency ID";
                    return _Res;
                }
                TradePairObj = _tradePairMasterRepository.FindBy(item => item.BaseCurrencyId == request.BaseCurrencyID &&
                                                 item.SecondaryCurrencyId == request.CurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (request.BaseCurrencyID == request.CurrencyID || TradePairObj == null)
                {
                    _Res.ErrorCode = enErrorCode.InValid_Currency_Pair;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "InValid_Currency_Pair";
                    return _Res;
                }
                var IsExistPair = _SiteTokenMasterRepository.FindBy(item => item.BaseCurrencyID == request.BaseCurrencyID &&
                                                 item.CurrencyID == request.CurrencyID).FirstOrDefault();
                if (IsExistPair != null)
                {
                    _Res.ErrorCode = enErrorCode.Service_Already_Exist;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "This service already exist";
                    return _Res;
                }
                //if(request.RateType== 2)
                //{
                //    var ltp = _tradePairStastics.FindBy(e => e.PairId == TradePairObj.Id).SingleOrDefault().LTP;
                //    request.Rate = ltp;
                //}
                var newModel = _SiteTokenMasterRepository.Add(new SiteTokenMaster()
                {
                    BaseCurrencyID = request.BaseCurrencyID,
                    CurrencyID = request.CurrencyID,
                    CurrencySMSCode = request.CurrencySMSCode,
                    BaseCurrencySMSCode = request.BaseCurrencySMSCode,
                    CreatedBy = UserID,
                    CreatedDate = DateTime.UtcNow,
                    DailyLimit = request.DailyLimit,
                    MaxLimit = request.MaxLimit,
                    MinLimit = request.MinLimit,
                    MonthlyLimit = request.MonthlyLimit,
                    Rate = request.Rate,
                    Note = request.Note,
                    RateType = request.RateType,
                    WeeklyLimit = request.WeeklyLimit,
                    Status = request.Status,
                    PairID = TradePairObj.Id
                });
                if (newModel.Id > 0)
                {
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "Data Insert Successfully.";
                }
                else
                {
                    _Res.ErrorCode = enErrorCode.DataInsertFail;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Insert Fail.";
                }
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 16-4-19 for Margin Trading , Allow only Secound Currency  for conversion
        public BizResponseClass AddSiteTokenMargin(SiteTokenMasterRequest request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            TradePairMasterMargin TradePairObj = new TradePairMasterMargin();

            try
            {
                var IsExistService = _serviceMasterRepositoryMargin.FindBy(e => e.Id == request.CurrencyID && e.Status != 9).SingleOrDefault();
                if (IsExistService == null)
                {
                    _Res.ErrorCode = enErrorCode.InValid_CurrencyID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Enter Valid Currency ID";
                    return _Res;
                }
                var IsExistBaseService = _serviceMasterRepositoryMargin.FindBy(e => e.Id == request.BaseCurrencyID && e.Status != 9).SingleOrDefault();
                if (IsExistBaseService == null)
                {
                    _Res.ErrorCode = enErrorCode.InValid_BaseCurrencyID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Enter Valid Base Currency ID";
                    return _Res;
                }
                //Rita 16-4-19 Convert to --SECOND-- Currency
                MarketMargin MarketMarginTargetObj = _marketRepositoryMargin.FindBy(item => item.ServiceID == request.CurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (MarketMarginTargetObj != null)
                {
                    _Res.ReturnMsg = "Target Currency Shoul Not be from Base Market";
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.Site_Token_TargetCurrencyShoulNotBeFromBaseMarket;
                    return _Res;
                }

                TradePairObj = _tradePairMasterRepositoryMargin.FindBy(item => item.BaseCurrencyId == request.BaseCurrencyID &&
                                                 item.SecondaryCurrencyId == request.CurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (request.BaseCurrencyID == request.CurrencyID || TradePairObj == null)
                {
                    _Res.ErrorCode = enErrorCode.InValid_Currency_Pair;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "InValid_Currency_Pair";
                    return _Res;
                }
                var IsExistPair = _SiteTokenMasterRepositoryMargin.FindBy(item => item.BaseCurrencyID == request.BaseCurrencyID &&
                                                 item.CurrencyID == request.CurrencyID).FirstOrDefault();
                if (IsExistPair != null)
                {
                    _Res.ErrorCode = enErrorCode.Service_Already_Exist;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "This service already exist";
                    return _Res;
                }
                //if(request.RateType== 2)
                //{
                //    var ltp = _tradePairStastics.FindBy(e => e.PairId == TradePairObj.Id).SingleOrDefault().LTP;
                //    request.Rate = ltp;
                //}
                var newModel = _SiteTokenMasterRepositoryMargin.Add(new SiteTokenMasterMargin()
                {
                    BaseCurrencyID = request.BaseCurrencyID,
                    CurrencyID = request.CurrencyID,
                    CurrencySMSCode = request.CurrencySMSCode,
                    BaseCurrencySMSCode = request.BaseCurrencySMSCode,
                    CreatedBy = UserID,
                    CreatedDate = DateTime.UtcNow,
                    DailyLimit = request.DailyLimit,
                    MaxLimit = request.MaxLimit,
                    MinLimit = request.MinLimit,
                    MonthlyLimit = request.MonthlyLimit,
                    Rate = request.Rate,
                    Note = request.Note,
                    RateType = request.RateType,
                    WeeklyLimit = request.WeeklyLimit,
                    Status = request.Status,
                    PairID = TradePairObj.Id
                });
                if (newModel.Id > 0)
                {
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "Data Insert Successfully.";
                }
                else
                {
                    _Res.ErrorCode = enErrorCode.DataInsertFail;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Insert Fail.";
                }
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass UpdateSiteToken(SiteTokenMasterRequest request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                if (!request.ID.HasValue)
                {
                    _Res.ErrorCode = enErrorCode.InValid_ID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Please Enter Valid ID";
                    return _Res;
                }
                var model = _SiteTokenMasterRepository.GetById(Convert.ToInt64(request.ID));
                model.MaxLimit = request.MaxLimit;
                model.MinLimit = request.MinLimit;
                model.DailyLimit = request.DailyLimit;
                model.MonthlyLimit = request.MonthlyLimit;
                model.WeeklyLimit = request.WeeklyLimit;
                model.Note = request.Note;
                model.Rate = request.Rate;
                model.RateType = request.RateType;
                model.UpdatedBy = UserID;
                model.UpdatedDate = DateTime.UtcNow;
                model.Status = request.Status;
                try
                {
                    _SiteTokenMasterRepository.Update(model);
                }
                catch (Exception e)
                {
                    _Res.ErrorCode = enErrorCode.DataUpdateFail;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Update Fail.";
                    return _Res;
                }

                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Data Update Successfully.";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 16-4-19 for Margin Trading
        public BizResponseClass UpdateSiteTokenMargin(SiteTokenMasterRequest request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                if (!request.ID.HasValue)
                {
                    _Res.ErrorCode = enErrorCode.InValid_ID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Please Enter Valid ID";
                    return _Res;
                }
                var model = _SiteTokenMasterRepositoryMargin.GetById(Convert.ToInt64(request.ID));
                model.MaxLimit = request.MaxLimit;
                model.MinLimit = request.MinLimit;
                model.DailyLimit = request.DailyLimit;
                model.MonthlyLimit = request.MonthlyLimit;
                model.WeeklyLimit = request.WeeklyLimit;
                model.Note = request.Note;
                model.Rate = request.Rate;
                model.RateType = request.RateType;
                model.UpdatedBy = UserID;
                model.UpdatedDate = DateTime.UtcNow;
                model.Status = request.Status;
                try
                {
                    _SiteTokenMasterRepositoryMargin.Update(model);
                }
                catch (Exception e)
                {
                    _Res.ErrorCode = enErrorCode.DataUpdateFail;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Update Fail.";
                    return _Res;
                }

                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Data Update Successfully.";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public SiteTokenMasterResponse GetAllSiteToken(short ActiveOnly = 0)
        {
            SiteTokenMasterResponse _Res = new SiteTokenMasterResponse();
            List<SiteTokenMasterInfo> siteTokens = new List<SiteTokenMasterInfo>();
            List<SiteTokenMaster> modelList = new List<SiteTokenMaster>();

            try
            {
                if (ActiveOnly == 0)
                    modelList = _SiteTokenMasterRepository.List();
                else
                    modelList = _SiteTokenMasterRepository.FindBy(e => e.Status == 1).ToList();

                var LtpData = _tradePairStastics.List();
                if (modelList.Count == 0)
                {
                    _Res.Response = siteTokens;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }


                foreach (var model in modelList)
                {
                    decimal LTP = 0;
                    LTP = LtpData.Where(e => e.PairId == model.PairID).SingleOrDefault().LTP;
                    LTP = model.RateType == 2 ? LTP : model.Rate;

                    siteTokens.Add(new SiteTokenMasterInfo()
                    {
                        BaseCurrencyID = model.BaseCurrencyID,
                        BaseCurrencySMSCode = model.BaseCurrencySMSCode,
                        CurrencyID = model.CurrencyID,
                        CurrencySMSCode = model.CurrencySMSCode,
                        DailyLimit = model.DailyLimit,
                        ID = model.Id,
                        MaxLimit = model.MaxLimit,
                        MinLimit = model.MinLimit,
                        MonthlyLimit = model.MonthlyLimit,
                        Note = model.Note,
                        Rate = LTP,
                        RateType = model.RateType,
                        Status = model.Status,
                        WeeklyLimit = model.WeeklyLimit
                    });
                }
                _Res.Response = siteTokens;
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
        //Rita 16-4-19 for Margin Trading
        public SiteTokenMasterResponse GetAllSiteTokenMargin(short ActiveOnly = 0)
        {
            SiteTokenMasterResponse _Res = new SiteTokenMasterResponse();
            List<SiteTokenMasterInfo> siteTokens = new List<SiteTokenMasterInfo>();
            List<SiteTokenMasterMargin> modelList = new List<SiteTokenMasterMargin>();

            try
            {
                if (ActiveOnly == 0)
                    modelList = _SiteTokenMasterRepositoryMargin.List();
                else
                    modelList = _SiteTokenMasterRepositoryMargin.FindBy(e => e.Status == 1).ToList();

                var LtpData = _tradePairStasticsMargin.List();
                if (modelList.Count == 0)
                {
                    _Res.Response = siteTokens;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }


                foreach (var model in modelList)
                {
                    decimal LTP = 0;
                    LTP = LtpData.Where(e => e.PairId == model.PairID).SingleOrDefault().LTP;
                    LTP = model.RateType == 2 ? LTP : model.Rate;

                    siteTokens.Add(new SiteTokenMasterInfo()
                    {
                        BaseCurrencyID = model.BaseCurrencyID,
                        BaseCurrencySMSCode = model.BaseCurrencySMSCode,
                        CurrencyID = model.CurrencyID,
                        CurrencySMSCode = model.CurrencySMSCode,
                        DailyLimit = model.DailyLimit,
                        ID = model.Id,
                        MaxLimit = model.MaxLimit,
                        MinLimit = model.MinLimit,
                        MonthlyLimit = model.MonthlyLimit,
                        Note = model.Note,
                        Rate = LTP,
                        RateType = model.RateType,
                        Status = model.Status,
                        WeeklyLimit = model.WeeklyLimit
                    });
                }
                _Res.Response = siteTokens;
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

        #region Arbitrage
        //Darshan Dholakiya added method for Arbitrage Changes : 04-06-2019
        public long AddArbitragePairConfiguration(TradePairConfigRequest Request, long UserId)
        {
            try
            {
                //Check Pair Already Available
                var OldPairData = _tradePairMasterArbitrageRepository.GetSingle(x => x.BaseCurrencyId == Request.BaseCurrencyId && x.SecondaryCurrencyId == Request.SecondaryCurrencyId);

                if (OldPairData != null)
                {
                    return -1;
                }

                var baseCurrency = _serviceMasterArbitrageRepository.GetById(Request.BaseCurrencyId);
                var secondCurrency = _serviceMasterArbitrageRepository.GetById(Request.SecondaryCurrencyId);
                var pairName = secondCurrency.SMSCode + "_" + baseCurrency.SMSCode;

                var pairMaster = new TradePairMasterArbitrage()
                {
                    PairName = pairName,
                    SecondaryCurrencyId = Request.SecondaryCurrencyId,
                    WalletMasterID = 0,
                    BaseCurrencyId = Request.BaseCurrencyId,
                    Status = Request.Status,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };

                var newPairMaster = _tradePairMasterArbitrageRepository.Add(pairMaster);

                var pairDetail = new TradePairDetailArbitrage()
                {
                    PairId = newPairMaster.Id,
                    BuyMinQty = Request.BuyMinQty,
                    BuyMaxQty = Request.BuyMaxQty,
                    SellMinQty = Request.SellMinQty,
                    SellMaxQty = Request.SellMaxQty,
                    SellPrice = Request.SellPrice,
                    BuyPrice = Request.BuyPrice,
                    BuyMinPrice = Request.BuyMinPrice,
                    BuyMaxPrice = Request.BuyMaxPrice,
                    SellMinPrice = Request.SellMinPrice,
                    SellMaxPrice = Request.SellMaxPrice,
                    SellFees = Request.SellFees,
                    BuyFees = Request.BuyFees,
                    FeesCurrency = Request.FeesCurrency,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    CreatedDate = _basePage.UTC_To_IST(),
                    ChargeType = Request.ChargeType,
                    OpenOrderExpiration = Request.OpenOrderExpiration,
                    CreatedBy = UserId,
                    AmtLength = Request.AmtLength,
                    PriceLength = Request.PriceLength,
                    QtyLength = Request.QtyLength

                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null


                };
                var newPairDetail = _tradePairDetailsArbitrage.Add(pairDetail);

                var pairStastic = new TradePairStasticsArbitrage
                {
                    PairId = newPairMaster.Id,
                    CurrentRate = Request.Currentrate,
                    LTP = Request.Currentrate,
                    ChangeVol24 = Request.Volume,
                    CurrencyPrice = Request.CurrencyPrice,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };
                var newPairStatics = _tradePairStasticsArbitrage.Add(pairStastic);

                return newPairMaster.Id;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }
        //Darshan Dholakiya added method for Arbitrage  Changes: 05-06-2019
        public long UpdatePairConfigurationArbitrage(TradePairConfigRequest Request, long UserId)
        {
            try
            {
                var pairMaster = _tradePairMasterArbitrageRepository.GetById(Request.Id);

                if (pairMaster != null)
                {
                    //var baseCurrency = _serviceMasterRepository.GetById(Request.BaseCurrencyId);
                    //var secondCurrency = _serviceMasterRepository.GetById(Request.SecondaryCurrencyId);
                    //var pairName = secondCurrency.SMSCode + "_" + baseCurrency.SMSCode;

                    //pairMaster.PairName = pairName;
                    //pairMaster.SecondaryCurrencyId = Request.SecondaryCurrencyId;
                    pairMaster.WalletMasterID = 0;
                    //pairMaster.BaseCurrencyId = Request.BaseCurrencyId;
                    pairMaster.UpdatedDate = _basePage.UTC_To_IST();
                    pairMaster.UpdatedBy = UserId;
                    pairMaster.Status = Request.Status;
                    _tradePairMasterArbitrageRepository.Update(pairMaster);

                    var pairDetail = _tradePairDetailsArbitrage.GetSingle(pair => pair.PairId == Request.Id);

                    pairDetail.BuyMinQty = Request.BuyMinQty;
                    pairDetail.BuyMaxQty = Request.BuyMaxQty;
                    pairDetail.SellMinQty = Request.SellMinQty;
                    pairDetail.SellMaxQty = Request.SellMaxQty;
                    pairDetail.SellPrice = Request.SellPrice;
                    pairDetail.BuyPrice = Request.BuyPrice;
                    pairDetail.BuyMinPrice = Request.BuyMinPrice;
                    pairDetail.BuyMaxPrice = Request.BuyMaxPrice;
                    pairDetail.SellMinPrice = Request.SellMinPrice;
                    pairDetail.SellMaxPrice = Request.SellMaxPrice;
                    pairDetail.SellFees = Request.SellFees;
                    pairDetail.BuyFees = Request.BuyFees;
                    pairDetail.FeesCurrency = Request.FeesCurrency;
                    pairDetail.UpdatedDate = _basePage.UTC_To_IST();
                    pairDetail.UpdatedBy = UserId;
                    pairDetail.ChargeType = Request.ChargeType;
                    pairDetail.OpenOrderExpiration = Request.OpenOrderExpiration;
                    _tradePairDetailsArbitrage.Update(pairDetail);

                    var pairStastics = _tradePairStasticsArbitrage.GetSingle(pair => pair.PairId == Request.Id);
                    pairStastics.CurrentRate = Request.Currentrate;
                    pairStastics.ChangeVol24 = Request.Volume;
                    pairStastics.CurrencyPrice = Request.CurrencyPrice;
                    pairStastics.UpdatedDate = _basePage.UTC_To_IST();
                    pairStastics.UpdatedBy = UserId;
                    _tradePairStasticsArbitrage.Update(pairStastics);

                    return Request.Id;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }
        //Darshan Dholakiya added method for Arbitrage Changes: 05-06-2019
        public TradePairConfigRequest GetPairConfigurationArbitrage(long PairId)
        {
            TradePairConfigRequest responsedata;
            try
            {
                responsedata = new TradePairConfigRequest();
                var pairMaster = _tradePairMasterArbitrageRepository.GetById(PairId);
                if (pairMaster != null)
                {
                    responsedata.Id = pairMaster.Id;
                    responsedata.PairName = pairMaster.PairName;
                    responsedata.SecondaryCurrencyId = pairMaster.SecondaryCurrencyId;
                    //responsedata.WalletMasterID = pairMaster.WalletMasterID;
                    responsedata.BaseCurrencyId = pairMaster.BaseCurrencyId;
                    responsedata.StatusText = Enum.GetName(typeof(ServiceStatus), pairMaster.Status);
                    responsedata.Status = pairMaster.Status;
                    var pairDetail = _tradePairDetailsArbitrage.GetSingle(pair => pair.PairId == PairId);

                    responsedata.BuyMinQty = pairDetail.BuyMinQty;
                    responsedata.BuyMaxQty = pairDetail.BuyMaxQty;
                    responsedata.SellMinQty = pairDetail.SellMinQty;
                    responsedata.SellMaxQty = pairDetail.SellMaxQty;
                    responsedata.SellPrice = pairDetail.SellPrice;
                    responsedata.BuyPrice = pairDetail.BuyPrice;
                    responsedata.BuyMinPrice = pairDetail.BuyMinPrice;
                    responsedata.BuyMaxPrice = pairDetail.BuyMaxPrice;
                    responsedata.SellMinPrice = pairDetail.SellMinPrice;
                    responsedata.SellMaxPrice = pairDetail.SellMaxPrice;
                    responsedata.BuyFees = pairDetail.BuyFees;
                    responsedata.SellFees = pairDetail.SellFees;
                    responsedata.FeesCurrency = pairDetail.FeesCurrency;
                    responsedata.OpenOrderExpiration = Convert.ToInt64(pairDetail.OpenOrderExpiration);
                    responsedata.ChargeType = Convert.ToInt16(pairDetail.ChargeType);
                    var pairStastics = _tradePairStasticsArbitrage.GetSingle(pair => pair.PairId == PairId);
                    responsedata.Volume = pairStastics.ChangeVol24;
                    //responsedata.Currentrate = pairStastics.CurrentRate;
                    //responsedata.CurrencyPrice = pairStastics.CurrencyPrice;

                    var service = _serviceMasterArbitrageRepository.GetSingle(pair => pair.Id == pairMaster.SecondaryCurrencyId);
                    responsedata.MarketName = service.Name;
                    return responsedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }
        //Darshan Dholakiya added method for Arbitrage Changes: 05-06-2019
        public GetAllTradePairConfigResponse GetAllPairConfigurationArbitrage(int? PageSize, int Page = 0, short IsMargin = 0)
        {
            List<TradePairConfigRequest> responsedata;
            GetAllTradePairConfigResponse getAllTradePairConfigResponse = new GetAllTradePairConfigResponse();
            try
            {
                responsedata = new List<TradePairConfigRequest>();
                //Uday 15-11-2018 applied direct query because one by one record pick has take long time to response
                //var pairMaster = _tradePairMasterRepository.List();
                List<TradePairConfigRequest> pairMaster;
                //if (IsMargin == 1)
                //    pairMaster = _backOfficeTrnRepository.GetAllPairConfigurationMargin();
                // else
                pairMaster = _backOfficeTrnRepository.GetAllPairConfigurationArbitrageData();

                if (pairMaster != null && pairMaster.Count > 0)
                {
                    foreach (var pair in pairMaster)
                    {
                        TradePairConfigRequest response = new TradePairConfigRequest();
                        response.Id = pair.Id;
                        response.PairName = pair.PairName;
                        response.MarketName = pair.MarketName;
                        response.SecondaryCurrencyId = pair.SecondaryCurrencyId;
                        //response.WalletMasterID = pair.WalletMasterID;
                        response.BaseCurrencyId = pair.BaseCurrencyId;
                        response.StatusText = Enum.GetName(typeof(ServiceStatus), pair.Status);
                        //var pairDetail = _tradePairDetailRepository.GetSingle(x => x.PairId == pair.Id);
                        response.Status = pair.Status;
                        response.BuyMinQty = pair.BuyMinQty;
                        response.BuyMaxQty = pair.BuyMaxQty;
                        response.SellMinQty = pair.SellMinQty;
                        response.SellMaxQty = pair.SellMaxQty;
                        response.SellPrice = pair.SellPrice;
                        response.BuyPrice = pair.BuyPrice;
                        response.BuyMinPrice = pair.BuyMinPrice;
                        response.BuyMaxPrice = pair.BuyMaxPrice;
                        response.SellMinPrice = pair.SellMinPrice;
                        response.SellMaxPrice = pair.SellMaxPrice;
                        response.BuyFees = pair.BuyFees;
                        response.SellFees = pair.SellFees;
                        response.FeesCurrency = pair.FeesCurrency;
                        response.ChargeType = pair.ChargeType;
                        response.OpenOrderExpiration = pair.OpenOrderExpiration;
                        //var pairStastics = _tradePairStastics.GetSingle(x => x.PairId == pair.Id);
                        response.Volume = pair.Volume;
                        response.Currentrate = pair.Currentrate;
                        response.CurrencyPrice = pair.CurrencyPrice;
                        response.QtyLength = pair.QtyLength;
                        response.PriceLength = pair.PriceLength;
                        response.AmtLength = pair.AmtLength;
                        responsedata.Add(response);
                    }
                    getAllTradePairConfigResponse.Response = responsedata;
                    getAllTradePairConfigResponse.Count = getAllTradePairConfigResponse.Response.Count;
                    if (PageSize != null && Page > 0)
                    {
                        int skip1 = Convert.ToInt32(PageSize) * (Page - 1);
                        var it = Convert.ToDouble(getAllTradePairConfigResponse.Response.Count) / PageSize;
                        var fl = Math.Ceiling((decimal)it);
                        getAllTradePairConfigResponse.Response = getAllTradePairConfigResponse.Response.Skip(skip1).Take(Convert.ToInt32(PageSize)).ToList();
                        getAllTradePairConfigResponse.PageSize = Convert.ToInt64(PageSize);
                        getAllTradePairConfigResponse.TotalPage = Convert.ToInt64(fl);
                    }
                    else
                    {
                        getAllTradePairConfigResponse.PageSize = responsedata.Count;
                        getAllTradePairConfigResponse.TotalPage = 0;
                    }
                    return getAllTradePairConfigResponse;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }
        //Darshan Dholakiya added method for Arbitrage Changes: 05-06-2019
        public int SetActivePairArbitrage(long PairId)
        {
            try
            {
                var pairdata = _tradePairMasterArbitrageRepository.GetById(PairId);
                if (pairdata != null)
                {
                    pairdata.MakePairActive();
                    _tradePairMasterArbitrageRepository.Update(pairdata);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }
        //Darshan Dholakiya added method for Arbitrage Changes: 05-06-2019
        public int SetInActivePairArbitrage(long PairId)
        {
            try
            {
                var pairdata = _tradePairMasterArbitrageRepository.GetById(PairId);
                if (pairdata != null)
                {
                    pairdata.MakePairInActive();
                    _tradePairMasterArbitrageRepository.Update(pairdata);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Darshan Dholakiya added method for Arbitrage Changes: 06-06-2019
        public Core.ViewModels.Configuration.ListPairResponse ListPairArbitrage(short IsMargin = 0)
        {
            try
            {
                Core.ViewModels.Configuration.ListPairResponse _Res = new Core.ViewModels.Configuration.ListPairResponse();
                List<ListPairInfo> list;
                // if (IsMargin == 1)
                //     list = _backOfficeTrnRepository.ListPairInfoMargin();
                // else
                list = _backOfficeTrnRepository.ListPairArbitrageInfo();
                if (list.Count == 0)
                {
                    _Res.Response = null;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnMsg = EnResponseMessage.CommFailMsgInternal;
                    return _Res;
                }
                _Res.Response = list;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = EnResponseMessage.CommSuccessMsgInternal;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        public string[] LpPairListConvertor(string[] LpSymbol, short LP)
        {
            try
            {
                string[] CommonSymbol;
                List<LPConverPair> pairs = new List<LPConverPair>();
                var PairList = _cache.Get<string[]>("TradePairConfiguration");
                if (PairList == null)
                {
                    PairList = _tradePairMasterRepository.FindBy(e => e.Status == 1).Select(e => e.PairName).ToArray();
                    _cache.Set<string[]>("TradePairConfiguration", PairList);
                }
                else if (PairList.Length == 0)
                {
                    PairList = _tradePairMasterRepository.FindBy(e => e.Status == 1).Select(e => e.PairName).ToArray();
                    _cache.Set<string[]>("TradePairConfiguration", PairList);
                }
                //var PairList = _tradePairMasterRepository.FindBy(e => e.Status == 1).Select(e => e.PairName);
                switch (LP)
                {
                    case (short)enAppType.Binance://LTCBTC
                        foreach (var pair in PairList)
                        {
                            if (LpSymbol.Contains(pair.Replace("_", "")))
                                pairs.Add(new LPConverPair() { Pair = pair.Replace("_", "") });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;

                    case (short)enAppType.Bittrex: //BTC_LTC
                        foreach (var pair in PairList)
                        {
                            if (LpSymbol.Contains(pair.Split("_")[1] + "_" + pair.Split("_")[0]))
                                pairs.Add(new LPConverPair() { Pair = pair.Split("_")[1] + "-" + pair.Split("_")[0] });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;

                    case (short)enAppType.Coinbase: //LtcBtc
                        //var obj2 = Char.ToUpperInvariant(str[0]) + str.Substring(1);
                        foreach (var pair in PairList)
                        {
                            if (LpSymbol.Contains(Char.ToUpperInvariant(pair.Split("_")[1][0]) + pair.Split("_")[1].Substring(1).ToLower() + Char.ToUpperInvariant(pair.Split("_")[0][0]) + pair.Split("_")[0].Substring(1).ToLower()))
                                pairs.Add(new LPConverPair() { Pair = Char.ToUpperInvariant(pair.Split("_")[1][0]) + pair.Split("_")[1].Substring(1).ToLower() + Char.ToUpperInvariant(pair.Split("_")[0][0]) + pair.Split("_")[0].Substring(1).ToLower() });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;

                    case (short)enAppType.Poloniex: //BTC_LTC
                        foreach (var pair in PairList)
                        {
                            if (LpSymbol.Contains(pair.Split("_")[1] + "_" + pair.Split("_")[0]))
                                pairs.Add(new LPConverPair() { Pair = pair.Split("_")[1] + "_" + pair.Split("_")[0] });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;

                    case (short)enAppType.TradeSatoshi://LTC_BTC
                        foreach (var pair in PairList)
                        {
                            if (LpSymbol.Contains(pair))
                                pairs.Add(new LPConverPair() { Pair = pair });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;
                    case (short)enAppType.UpBit://LTC_BTC
                        foreach (var pair in PairList)
                        {
                            if (LpSymbol.Contains(pair))
                                pairs.Add(new LPConverPair() { Pair = pair });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;
                }

                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #region
        //add by akshay for testing purpose 20/02/2020 4:58 pm 
        public string[] LpPairListConvertorV2(short LP)
        {
            try
            {
                string[] CommonSymbol;
                List<LPConverPair> pairs = new List<LPConverPair>();
                var PairList = _cache.Get<ConfigureLP[]>("TradePairConfigurationV1");
                if (PairList == null)
                {
                    PairList = _frontTrnRepository.GetLiquidityConfigurationData(LP).ToArray();
                    _cache.Set<ConfigureLP[]>("TradePairConfigurationV1", PairList);
                }
                else if (PairList.Length == 0)
                {
                    PairList = _frontTrnRepository.GetLiquidityConfigurationData(LP).ToArray();
                    _cache.Set<ConfigureLP[]>("TradePairConfigurationV1", PairList);
                }
                PairList = PairList.Where(e => e.LPType == LP).Distinct().ToArray();
                switch (LP)
                {
                    case (short)enAppType.Binance://LTCBTC
                        foreach (var Data in PairList)
                        {
                            Data.Pair.Replace("_", "");
                           pairs.Add(new LPConverPair() { Pair = Data.Pair.Replace("_", "") });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;
                        
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        #endregion
        public string[] LpPairListConvertorV1(string[] LpSymbol, short LP)
        {
            try
            {
                string[] CommonSymbol;
                List<LPConverPair> pairs = new List<LPConverPair>();
                var PairList = _cache.Get<ConfigureLP[]>("TradePairConfigurationV1");
                if (PairList == null)
                {
                    PairList = _frontTrnRepository.GetLiquidityConfigurationData(LP).ToArray();
                    _cache.Set<ConfigureLP[]>("TradePairConfigurationV1", PairList);
                }
                else if (PairList.Length == 0)
                {
                    PairList = _frontTrnRepository.GetLiquidityConfigurationData(LP).ToArray();
                    _cache.Set<ConfigureLP[]>("TradePairConfigurationV1", PairList);
                }
                PairList = PairList.Where(e => e.LPType == LP).Distinct().ToArray();
                switch (LP)
                {
                    case (short)enAppType.Binance://LTCBTC
                        foreach (var Data in PairList)
                        {
                            if (LpSymbol.Contains(Data.Pair.Replace("_", "")))
                                pairs.Add(new LPConverPair() { Pair = Data.Pair.Replace("_", "") });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;

                    case (short)enAppType.Bittrex: //BTC_LTC
                        foreach (var Data in PairList)
                        {
                            if (LpSymbol.Contains(Data.Pair.Split("_")[1] + "_" + Data.Pair.Split("_")[0]))
                                pairs.Add(new LPConverPair() { Pair = Data.Pair.Split("_")[1] + "-" + Data.Pair.Split("_")[0] });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;

                    case (short)enAppType.Coinbase: //LtcBtc
                        //var obj2 = Char.ToUpperInvariant(str[0]) + str.Substring(1);
                        foreach (var Data in PairList)
                        {
                            if (LpSymbol.Contains(Char.ToUpperInvariant(Data.Pair.Split("_")[1][0]) + Data.Pair.Split("_")[1].Substring(1).ToLower() + Char.ToUpperInvariant(Data.Pair.Split("_")[0][0]) + Data.Pair.Split("_")[0].Substring(1).ToLower()))
                                pairs.Add(new LPConverPair() { Pair = Char.ToUpperInvariant(Data.Pair.Split("_")[1][0]) + Data.Pair.Split("_")[1].Substring(1).ToLower() + Char.ToUpperInvariant(Data.Pair.Split("_")[0][0]) + Data.Pair.Split("_")[0].Substring(1).ToLower() });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;

                    case (short)enAppType.Poloniex: //BTC_LTC
                        foreach (var Data in PairList)
                        {
                            if (LpSymbol.Contains(Data.Pair.Split("_")[1] + "_" + Data.Pair.Split("_")[0]))
                                pairs.Add(new LPConverPair() { Pair = Data.Pair.Split("_")[1] + "_" + Data.Pair.Split("_")[0] });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;

                    case (short)enAppType.TradeSatoshi://LTC_BTC
                        foreach (var Data in PairList)
                        {
                            if (LpSymbol.Contains(Data.Pair))
                                pairs.Add(new LPConverPair() { Pair = Data.Pair });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;
                    case (short)enAppType.UpBit://LTC_BTC
                        foreach (var Data in PairList)
                        {
                            if (LpSymbol.Contains(Data.Pair))
                                pairs.Add(new LPConverPair() { Pair = Data.Pair });
                        }
                        CommonSymbol = new string[pairs.Count];
                        CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                        return CommonSymbol;
                }

                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public ConfigureLP[] TradePairConfigurationV1()
        {
            try
            {
                var PairList = _cache.Get<ConfigureLP[]>("TradePairConfigurationV1");
                if (PairList == null)
                {
                    PairList = _frontTrnRepository.GetLiquidityConfigurationData(0).ToArray();
                    _cache.Set<ConfigureLP[]>("TradePairConfigurationV1", PairList);
                }
                else if (PairList.Length == 0)
                {
                    PairList = _frontTrnRepository.GetLiquidityConfigurationData(0).ToArray();
                    _cache.Set<ConfigureLP[]>("TradePairConfigurationV1", PairList);
                }

                return PairList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public ConfigureLP[] LpPairListConvertorWithLocalPair(short LP) //string[] LpSymbol,
        {
            try
            {
                //string[] CommonSymbol;
                //List<LPConverPairV1> pairs = new List<LPConverPairV1>();
                var PairList = _cache.Get<ConfigureLP[]>("TradePairConfigurationV1");
                if (PairList == null)
                {
                    PairList = _frontTrnRepository.GetLiquidityConfigurationData(LP).ToArray();
                    _cache.Set<ConfigureLP[]>("TradePairConfigurationV1", PairList);
                }
                else if (PairList.Length == 0)
                {
                    PairList = _frontTrnRepository.GetLiquidityConfigurationData(LP).ToArray();
                    _cache.Set<ConfigureLP[]>("TradePairConfigurationV1", PairList);
                }
                PairList = PairList.Where(e => e.LPType == LP).Distinct().ToArray();
                return PairList;
                //switch (LP)
                //{
                //    case (short)enAppType.Binance://LTCBTC
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair.Replace("_", "")))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair.Replace("_", ""), LocalPair = Data.Pair });
                //        }
                //        // CommonSymbol = new string[pairs.Count];
                //        // CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        // return CommonSymbol;
                //        return pairs.ToArray();


                //    case (short)enAppType.Huobi://ETH_USDT
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair.Replace("_", "")))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair.Replace("_", ""), LocalPair = Data.Pair });
                //        }
                //        // CommonSymbol = new string[pairs.Count];
                //        // CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        // return CommonSymbol;
                //        return pairs.ToArray();


                //    case (short)enAppType.Bittrex: //BTC_LTC
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair.Split("_")[1] + "_" + Data.Pair.Split("_")[0]))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair.Split("_")[1] + "-" + Data.Pair.Split("_")[0], LocalPair = Data.Pair });
                //        }
                //        //CommonSymbol = new string[pairs.Count];
                //        //CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        //return CommonSymbol;
                //        return pairs.ToArray();
                //    case (short)enAppType.Coinbase: //LtcBtc
                //        //var obj2 = Char.ToUpperInvariant(str[0]) + str.Substring(1);
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Char.ToUpperInvariant(Data.Pair.Split("_")[1][0]) + Data.Pair.Split("_")[1].Substring(1).ToLower() + Char.ToUpperInvariant(Data.Pair.Split("_")[0][0]) + Data.Pair.Split("_")[0].Substring(1).ToLower()))
                //            pairs.Add(new LPConverPairV1() { Pair = Char.ToUpperInvariant(Data.Pair.Split("_")[0][0]) + Data.Pair.Split("_")[0].Substring(1).ToLower() + Char.ToUpperInvariant(Data.Pair.Split("_")[1][0]) + Data.Pair.Split("_")[1].Substring(1).ToLower(), LocalPair = Data.Pair });
                //        }
                //        //CommonSymbol = new string[pairs.Count];
                //        //CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        //return CommonSymbol;
                //        return pairs.ToArray();
                //    case (short)enAppType.Poloniex: //BTC_LTC
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair.Split("_")[1] + "_" + Data.Pair.Split("_")[0]))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair.Split("_")[1] + "_" + Data.Pair.Split("_")[0], LocalPair = Data.Pair });
                //        }
                //        //CommonSymbol = new string[pairs.Count];
                //        //CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        //return CommonSymbol;
                //        return pairs.ToArray();
                //    case (short)enAppType.TradeSatoshi://LTC_BTC
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair, LocalPair = Data.Pair });
                //        }
                //        return pairs.ToArray();

                //    //Add New Case for OKEx by Pushpraj as on 11-06-2019
                //    case (short)enAppType.OKEx://LTC_BTC
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair, LocalPair = Data.Pair });
                //        }
                //        //CommonSymbol = new string[pairs.Count];
                //        //CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        //return CommonSymbol;
                //        return pairs.ToArray();
                //    case (short)enAppType.UpBit://BTC_LTC
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair, LocalPair = Data.Pair });
                //        }
                //        //CommonSymbol = new string[pairs.Count];
                //        //CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        //return CommonSymbol;
                //        return pairs.ToArray();
                //    case (short)enAppType.Kraken://EOS-USD
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair, LocalPair = Data.Pair });
                //        }
                //        //CommonSymbol = new string[pairs.Count];
                //        //CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        //return CommonSymbol;
                //        return pairs.ToArray();

                //    case (short)enAppType.Bitfinex://BTC_USD
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair, LocalPair = Data.Pair });
                //        }
                //        //CommonSymbol = new string[pairs.Count];
                //        //CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        //return CommonSymbol;
                //        return pairs.ToArray();

                //    case (short)enAppType.Gemini://BTC_USD
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair, LocalPair = Data.Pair });
                //        }
                //        //CommonSymbol = new string[pairs.Count];
                //        //CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        //return CommonSymbol;
                //        return pairs.ToArray();

                //    case (short)enAppType.CEXIO://BTC_USD
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair, LocalPair = Data.Pair });
                //        }
                //        //CommonSymbol = new string[pairs.Count];
                //        //CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        //return CommonSymbol;
                //        return pairs.ToArray();
                //    case (short)enAppType.EXMO://BTC_USD
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair, LocalPair = Data.Pair });
                //        }
                //        //CommonSymbol = new string[pairs.Count];
                //        //CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        //return CommonSymbol;
                //        return pairs.ToArray();

                //    case (short)enAppType.Yobit://BTC_USD
                //        foreach (var Data in PairList)
                //        {
                //            //if (LpSymbol.Contains(Data.Pair))
                //            pairs.Add(new LPConverPairV1() { Pair = Data.Pair, LocalPair = Data.Pair });
                //        }
                //        //CommonSymbol = new string[pairs.Count];
                //        //CommonSymbol = Array.ConvertAll<LPConverPair, string>(pairs.ToArray(), x => (string)x.Pair);
                //        //return CommonSymbol;
                //        return pairs.ToArray();
                //}

                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }


        #region Arbitrage Liquidity API Manager
        //khushali 04-06-2019 for Arbitrage Exchange configuration

        public GetAllLiquidityAPIManagerArbitrage GetAllLiquidityAPIManagerArbitrage(int Page, int? PageSize)
        {
            GetAllLiquidityAPIManagerArbitrage Res = new GetAllLiquidityAPIManagerArbitrage();
            try
            {

                Res.Response = new List<LiquidityAPIManagerDataArbitrage>();
                var ProviderData = _proDetailArbitrageRepository.List();

                ProviderData = ProviderData.Where(o => o.Status == 1).ToList();
                foreach (var ProData in ProviderData)
                {
                    LiquidityAPIManagerDataArbitrage Data = new LiquidityAPIManagerDataArbitrage();
                    var AllowedOrderType = new List<AllowOrderType>();
                    var FilteredAllowedorderType = _allowesOrderTypeArbitrageRepository.FindBy(e => e.SerProDetailID == ProData.Id);
                    if (FilteredAllowedorderType.Count() > 0)
                    {
                        AllowedOrderType = _allowesOrderTypeArbitrageRepository.FindBy(e => e.SerProDetailID == ProData.Id).Select(
                        o => new AllowOrderType { OrderType = o.OrderType, Status = o.Status, ProviderDetailID = o.SerProDetailID }).ToList();

                    }
                    Data.APIProviderId = ProData.ThirPartyAPIID;
                    Data.Id = ProData.Id;
                    Data.Name = ProData.SerProDetailName;
                    Data.ProviderTypeId = ProData.ProTypeID;
                    Data.ServiceProviderCongigId = ProData.ServiceProConfigID;
                    Data.Status = ProData.Status;
                    Data.ProviderMasterId = ProData.ServiceProID;
                    Data.Trntype = ProData.TrnTypeID;
                    Data.OrderType = AllowedOrderType;

                    Res.Response.Add(Data);
                }
                var pagesize = (PageSize == null) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                var it = Convert.ToDouble(Res.Response.Count) / pagesize;
                var fl = Math.Ceiling(it);
                Res.TotalPage = Convert.ToInt64(fl);
                Res.Count = Res.Response.Count;
                if (Page > 0)
                {
                    if (PageSize == null)
                    {
                        int skip = Helpers.PageSize * (Page - 1);
                        Res.Response = Res.Response.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                    else
                    {
                        int skip = Convert.ToInt32(PageSize) * (Page - 1);
                        Res.Response = Res.Response.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    }
                }
                Res.PageSize = pagesize;
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public LiquidityAPIManagerDataArbitrage GetLiquidityAPIManagerArbitrage(long Id)
        {
            try
            {
                LiquidityAPIManagerDataArbitrage Data = new LiquidityAPIManagerDataArbitrage();

                var ProData = _proDetailArbitrageRepository.GetById(Id);

                if (ProData != null)
                {
                    Data.APIProviderId = ProData.ThirPartyAPIID;
                    Data.Id = ProData.Id;
                    Data.Name = ProData.SerProDetailName;
                    Data.ProviderTypeId = ProData.ProTypeID;
                    Data.ServiceProviderCongigId = ProData.ServiceProConfigID;
                    Data.Status = ProData.Status;
                    Data.Trntype = ProData.TrnTypeID;
                    Data.OrderType = _allowesOrderTypeArbitrageRepository.FindBy(e => e.SerProDetailID == ProData.Id).Select(
                        o => new AllowOrderType { OrderType = o.OrderType, Status = o.Status, ProviderDetailID = o.SerProDetailID }).ToList();

                    return Data;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public int UpdateLiquidityAPIManagerArbitrage(LiquidityAPIManagerArbitrageUpdateRequest Request, long UserId)
        {
            try
            {
                //Get ThirdParty API Data

                //var ThirdPartyObj = _thirdPartyAPIRepository.GetById(Request.APIProviderId);
                //var ServiceProviderMasterObj = _ServiceProviderMaster.GetById(Request.ProviderMasterId);
                //var ServiceProviderType = _ProviderTypeRepository.GetById(Request.ProviderTypeId);
                var ServiceProviderDetail = _proDetailArbitrageRepository.GetById(Request.Id);
                var configDetail = _proConfigurationArbitrageRepository.GetById(ServiceProviderDetail.ServiceProConfigID);
                if (ServiceProviderDetail != null)
                {
                    //ServiceProviderMasterObj.Status = Request.Status;
                    ServiceProviderDetail.Status = Request.Status;
                }
                foreach (var Data in Request.OrderType)
                {
                    var AllowOrderType = _allowesOrderTypeArbitrageRepository.FindBy(e => e.OrderType == Data.OrderType && e.SerProDetailID == Request.Id).FirstOrDefault();
                    if (AllowOrderType != null)
                    {
                        AllowOrderType.Status = Data.Status;
                        _allowesOrderTypeArbitrageRepository.Update(AllowOrderType);
                    }
                    else
                    {
                        AllowesOrderTypeArbitrage NewAllowOrderType = new AllowesOrderTypeArbitrage()
                        {
                            OrderType = Data.OrderType,
                            SerProDetailID = Data.ProviderDetailID,
                            Status = Data.Status,
                            CreatedBy = UserId,
                            CreatedDate = Helpers.UTC_To_IST()
                        };

                        _allowesOrderTypeArbitrageRepository.Add(NewAllowOrderType);
                    }
                }
                //_ProConfigurationArbitrageRepository.Update(configDetail);
                _proDetailArbitrageRepository.Update(ServiceProviderDetail);


                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public GetAllLiquidityAPIProviderManagerArbitrage GetAllLiquidityAPIProviderManagerArbitrage(int? PageSize, int Page = 0)
        {
            GetAllLiquidityAPIProviderManagerArbitrage Res = new GetAllLiquidityAPIProviderManagerArbitrage();
            try
            {
                Res.Response = new List<LiquidityAPIProviderManagerArbitrage>();

                var ProviderData = _proConfigurationArbitrageRepository.List();

                ProviderData = ProviderData.Where(e => e.Status != (short)ServiceStatus.Disable).ToList();

                foreach (var ProData in ProviderData)
                {
                    LiquidityAPIProviderManagerArbitrage Data = new LiquidityAPIProviderManagerArbitrage();

                    Data.Id = ProData.Id;
                    Data.AppKey = ProData.AppKey;
                    Data.Status = ProData.Status;
                    Data.APIKey = ProData.APIKey;
                    Data.APISecret = ProData.SecretKey;
                    Data.UserName = ProData.UserName;
                    Data.Password = ProData.Password;
                    Res.Response.Add(Data);
                }
                Res.Count = Res.Response.Count;
                //This is customize pagination donot change without asking mobile and web team--DarshanDholakiya:25-07-2019
                if (PageSize != null && Page > 0)
                {
                    var pagesize = Convert.ToInt64(PageSize);
                    var it = Convert.ToDouble(Res.Response.Count) / pagesize;
                    var fl = Math.Ceiling(it);
                    int skip = Convert.ToInt32(PageSize) * (Page - 1);
                    Res.Response = Res.Response.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    Res.TotalPage = Convert.ToInt64(fl);
                    Res.PageSize = pagesize;
                }
                else
                {
                    Res.PageSize = Res.Response.Count;
                    Res.TotalPage = 0;
                }
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public LiquidityAPIProviderManagerArbitrage GetLiquidityAPIProviderManagerArbitrage(long Id)
        {
            try
            {
                LiquidityAPIProviderManagerArbitrage Data = new LiquidityAPIProviderManagerArbitrage();

                var ProData = _proConfigurationArbitrageRepository.GetById(Id);

                if (ProData != null)
                {
                    Data.Id = ProData.Id;
                    Data.AppKey = ProData.AppKey;
                    Data.Status = ProData.Status;
                    Data.APIKey = ProData.APIKey;
                    Data.APISecret = ProData.SecretKey;
                    Data.UserName = ProData.UserName;
                    Data.Password = ProData.Password;
                    return Data;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public int UpdateLiquidityAPIProviderManagerArbitrage(LiquidityAPIProviderManagerArbitrageUpdateRequest Request, long UserId)
        {
            try
            {
                var configDetail = _proConfigurationArbitrageRepository.GetById(Request.Id);
                if (configDetail != null)
                {
                    var IsDuplicate = _proConfigurationArbitrageRepository.GetSingle(x => x.AppKey == Request.AppKey && x.Id != Request.Id);
                    if (IsDuplicate != null)
                    {
                        return 0;
                    }
                    configDetail.APIKey = Request.APIKey;
                    configDetail.SecretKey = Request.APISecret;
                    configDetail.AppKey = Request.AppKey;
                    configDetail.UserName = Request.UserName;
                    configDetail.Password = Request.Password;
                    configDetail.Status = Request.Status;

                    _proConfigurationArbitrageRepository.Update(configDetail);
                }
                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public int AddLiquidityAPIProviderManagerArbitrage(LiquidityAPIProviderManagerArbitrageUpdateRequest Request, long UserId)
        {
            try
            {
                var configDetailRes = _proConfigurationArbitrageRepository.GetSingle(x => x.AppKey == Request.AppKey);
                if (configDetailRes == null)
                {
                    var configDetail = new ServiceProConfigurationArbitrage();
                    configDetail.APIKey = Request.APIKey;
                    configDetail.SecretKey = Request.APISecret;
                    configDetail.AppKey = Request.AppKey;
                    configDetail.UserName = Request.UserName;
                    configDetail.Password = Request.Password;
                    configDetail.Status = Request.Status;
                    configDetail.CreatedBy = UserId;
                    configDetail.CreatedDate = Helpers.UTC_To_IST();
                    _proConfigurationArbitrageRepository.Add(configDetail);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }


        #endregion

        #region ArbitrageServiceProvider
        //Darshan Dholakiya added this method for Arbitrage Service provider related change:07-06-2019
        public GetAllServiceProvideViewModel GetAllProviderArbitrage(int? PageSize, int Page)
        {
            try
            {
                GetAllServiceProvideViewModel serviceProviderResponse = new GetAllServiceProvideViewModel();
                serviceProviderResponse.Response = new List<ServiceProviderViewModel>();
                var list = _ServiceProviderMasterArbitrage.List();
                foreach (var model in list)
                {
                    ServiceProviderViewModel providerList = new ServiceProviderViewModel();
                    providerList.Id = model.Id;
                    providerList.ProviderName = model.ProviderName;
                    providerList.Status = model.Status;
                    serviceProviderResponse.Response.Add(providerList);
                }
                serviceProviderResponse.Count = serviceProviderResponse.Response.Count;

                //This is customize pagination donot change without asking mobile and web team---DarshanDholakiya-:25-07-2019
                if (PageSize != null && Page > 0)
                {
                    var pagesize = Convert.ToInt64(PageSize);
                    var it = Convert.ToDouble(serviceProviderResponse.Response.Count) / pagesize;
                    var fl = Math.Ceiling(it);
                    int skip = Convert.ToInt32(PageSize) * (Page - 1);
                    serviceProviderResponse.Response = serviceProviderResponse.Response.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    serviceProviderResponse.TotalPage = Convert.ToInt64(fl);
                    serviceProviderResponse.PageSize = pagesize;
                }
                else
                {
                    serviceProviderResponse.Count = serviceProviderResponse.Response.Count;
                    serviceProviderResponse.TotalPage = 0;
                }
                return serviceProviderResponse;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Darshan Dholakiya added this method for Arbitrage Service provider related change:07-06-2019
        public ServiceProviderViewModel GetPoviderByIDArbitrage(long ID)
        {
            try
            {
                ServiceProviderMasterArbitrage model = _ServiceProviderMasterArbitrage.GetById(ID);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new ServiceProviderViewModel
                {
                    Id = model.Id,
                    ProviderName = model.ProviderName,
                    Status = model.Status,


                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }
        //Darshan Dholakiya added this method for Arbitrage Service provider related change:07-06-2019
        public long AddProviderServiceArbitrage(ServiceProviderRequest request, long UserId)
        {
            try
            {
                var model = new ServiceProviderMasterArbitrage
                {
                    Id = request.Id,
                    ProviderName = request.ProviderName,
                    Status = request.Status,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };
                var newModel = _ServiceProviderMasterArbitrage.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Darshan Dholakiya added this method for Arbitrage Service provider related change:07-06-2019
        public bool UpdateProviderServiceArbitrage(ServiceProviderRequest request, long UserId)
        {
            try
            {
                ServiceProviderMasterArbitrage model = _ServiceProviderMasterArbitrage.GetById(request.Id);
                if (model == null)
                {
                    return false;
                }
                //model.ProviderName = request.ProviderName;
                model.UpdatedDate = _basePage.UTC_To_IST();
                model.UpdatedBy = UserId;
                model.Status = request.Status;
                _ServiceProviderMasterArbitrage.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        //komal 10-06-2019 Arbitrage 
        public ConfigureLPArbitrage[] TradePairConfigurationArbitrageV1()
        {
            try
            {
                var PairList = _cache.Get<ConfigureLPArbitrage[]>("TradePairConfigurationArbitrageV1");
                if (PairList == null)
                {
                    PairList = _frontTrnRepository.GetLiquidityConfigurationDataArbitrage(0).ToArray();
                    _cache.Set<ConfigureLPArbitrage[]>("TradePairConfigurationArbitrageV1", PairList);
                }
                else if (PairList.Length == 0)
                {
                    PairList = _frontTrnRepository.GetLiquidityConfigurationDataArbitrage(0).ToArray();
                    _cache.Set<ConfigureLPArbitrage[]>("TradePairConfigurationArbitrageV1", PairList);
                }

                return PairList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<long> AddServiceConfigurationArbitrage(ServiceConfigurationRequest Request, long UserId)
        {
            try
            {
                var oldCoin = _serviceMasterArbitrageRepository.GetSingle(x => x.SMSCode.ToLower() == Request.SMSCode.ToLower() || x.Name.ToLower() == Request.Name);
                if (oldCoin != null)
                {
                    return -1;
                }
                if (Request.SMSCode.Contains(' ') || Request.SMSCode.Contains('_')) //Coin name not contains space or _
                {
                    return -2;
                }

                ServiceMasterArbitrage serviceMaster = new ServiceMasterArbitrage()
                {
                    Name = Request.Name,
                    SMSCode = Request.SMSCode,
                    //ServiceType = Request.Type,
                    Status = Request.Status,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };
                var newServiceMaster = _serviceMasterArbitrageRepository.Add(serviceMaster);

                ServiceDetailJsonData serviceDetailJsonData = new ServiceDetailJsonData()
                {
                    //ImageUrl = Request.ImageUrl,
                    TotalSupply = Request.TotalSupply,
                    MaxSupply = Request.MaxSupply,
                    //ProofType = Request.ProofType,
                    Community = Request.Community,
                    Explorer = Request.Explorer,
                    //EncryptionAlgorithm = Request.EncryptionAlgorithm,
                    WebsiteUrl = Request.WebsiteUrl,
                    //WhitePaperPath = Request.WhitePaperPath,
                    Introduction = Request.Introduction,

                };

                ServiceDetailArbitrage serviceDetail = new ServiceDetailArbitrage()
                {
                    ServiceId = newServiceMaster.Id,
                    Status = Request.Status,//rita 2-5-19 pass as active
                    ServiceDetailJson = JsonConvert.SerializeObject(serviceDetailJsonData)
                };
                var newServiceDetail = _serviceDetailRepositoryArbitrage.Add(serviceDetail);

                ServiceStasticsArbitrage serviceStastics = new ServiceStasticsArbitrage()
                {
                    ServiceId = newServiceMaster.Id,
                    IssueDate = Request.IssueDate,
                    IssuePrice = Request.IssuePrice,
                    MaxSupply = Request.MaxSupply,
                    CirculatingSupply = Request.CirculatingSupply,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId,
                    // UpdatedDate = DateTime.UtcNow,
                    // UpdatedBy = null
                };
                var newServiceStastics = _serviceStasticsRepositoryArbitrage.Add(serviceStastics);

                var depositSerMapping = new ServiceTypeMappingArbitrage
                {
                    ServiceId = newServiceMaster.Id,
                    TrnType = Convert.ToInt16(enTrnType.Deposit),
                    Status = Request.IsDeposit,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };
                var newdepositSerMapping = _serviceTypeMappingArbitrage.Add(depositSerMapping);

                var withdrawSerMapping = new ServiceTypeMappingArbitrage
                {
                    ServiceId = newServiceMaster.Id,
                    TrnType = Convert.ToInt16(enTrnType.Withdraw),
                    Status = Request.IsWithdraw,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    // UpdatedBy = null
                };
                var newwithdrawSerMapping = _serviceTypeMappingArbitrage.Add(withdrawSerMapping);

                var tranSerMapping = new ServiceTypeMappingArbitrage
                {
                    ServiceId = newServiceMaster.Id,
                    TrnType = Convert.ToInt16(enTrnType.Transaction),
                    Status = Request.IsTransaction,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };
                var newtranSerMapping = _serviceTypeMappingArbitrage.Add(tranSerMapping);

                //Add Default WalletType Master
                var walletTypeMaster = new ArbitrageWalletTypeMaster
                {
                    WalletTypeName = Request.SMSCode,
                    Description = Request.SMSCode,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    IsDepositionAllow = Request.IsDeposit,
                    IsWithdrawalAllow = Request.IsWithdraw,
                    IsTransactionWallet = Request.IsTransaction,
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST()
                };
                var newwalletTypeMaster = _walletTypeServiceArbitrage.Add(walletTypeMaster);

                //Update WalletTypeId In ServiceMaster
                newServiceMaster.WalletTypeID = newwalletTypeMaster.Id;
                _serviceMasterArbitrageRepository.Update(newServiceMaster);

                ////Add Into WalletMaster For Default Organization

                // int[] AllowTrnType = new int[3] { Convert.ToInt16(enTrnType.Deposit), Convert.ToInt16(enTrnType.Withdraw), Convert.ToInt16(enTrnType.Transaction) };
                // var walletMaster = await _walletService.InsertIntoWalletMaster(" Default Org " + Request.SMSCode, Request.SMSCode, 1, AllowTrnType, UserId, null, 1);
                // _walletService.CreateWalletForAllUser_NewService(Request.SMSCode);

                //Add BaseCurrency In MarketEntity
                if (Request.IsBaseCurrency == 1)
                {
                    var marketViewModel = new MarketViewModel { CurrencyName = Request.SMSCode, isBaseCurrency = 1, ServiceID = newServiceMaster.Id };
                    //    AddMarketData(marketViewModel, UserId);
                }
                //komal 3-04-2019 single entry with country =1
                var Product = _productConfigRepositoryArbitrage.Add(new ProductConfigurationArbitrage() { Status = 1, CountryID = 1, CreatedBy = 1, CreatedDate = DateTime.UtcNow, ProductName = serviceMaster.SMSCode, ServiceID = serviceMaster.Id });

                return newServiceMaster.Id;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Darshan Dholakiya added this method for Arbitrage Service provider related change:10-06-2019

        //Darshan Dholakiya added this method for Arbitrage Service provider related change:10-06-2019
        public long UpdateServiceConfigurationArbitrage(ServiceConfigurationRequest Request, long UserId)
        {
            try
            {
                //Uday 08-01-2019 SMSCode is not same as old coin
                var oldCoin = _serviceMasterArbitrageRepository.GetSingle(x => (x.SMSCode.ToLower() == Request.SMSCode.ToLower() || x.Name.ToLower() == Request.Name.ToLower()) && x.Id != Request.ServiceId);
                if (oldCoin != null)
                {
                    return -1;
                }
                if (Request.SMSCode.Contains(' ') || Request.SMSCode.Contains('_')) //Coin name not contains space or _
                {
                    return -2;
                }

                string OldSMSCode = "";
                var serviceMaster = _serviceMasterArbitrageRepository.GetById(Request.ServiceId);
                if (serviceMaster != null)
                {
                    OldSMSCode = serviceMaster.SMSCode;

                    serviceMaster.Name = Request.Name;
                    serviceMaster.SMSCode = Request.SMSCode;
                    // serviceMaster.ServiceType = Request.Type;
                    serviceMaster.UpdatedBy = UserId;
                    serviceMaster.UpdatedDate = _basePage.UTC_To_IST();
                    serviceMaster.Status = Request.Status;

                    _serviceMasterArbitrageRepository.Update(serviceMaster);

                    ServiceDetailJsonData serviceDetailJsonData = new ServiceDetailJsonData()
                    {
                        //ImageUrl = Request.ImageUrl,
                        TotalSupply = Request.TotalSupply,
                        MaxSupply = Request.MaxSupply,
                        //ProofType = Request.ProofType,
                        Community = Request.Community,
                        Explorer = Request.Explorer,
                        //EncryptionAlgorithm = Request.EncryptionAlgorithm,
                        WebsiteUrl = Request.WebsiteUrl,
                        //WhitePaperPath = Request.WebsiteUrl,
                        Introduction = Request.Introduction
                    };

                    var serviceDetail = _serviceDetailRepositoryArbitrage.GetSingle(service => service.ServiceId == Request.ServiceId);
                    serviceDetail.ServiceDetailJson = JsonConvert.SerializeObject(serviceDetailJsonData);
                    _serviceDetailRepositoryArbitrage.Update(serviceDetail);

                    var serviceStastics = _serviceStasticsRepositoryArbitrage.GetSingle(service => service.ServiceId == Request.ServiceId);
                    serviceStastics.IssueDate = Request.IssueDate;
                    serviceStastics.IssuePrice = Request.IssuePrice;
                    serviceStastics.MaxSupply = Request.MaxSupply;
                    serviceStastics.CirculatingSupply = Request.CirculatingSupply;
                    serviceStastics.UpdatedDate = _basePage.UTC_To_IST();
                    serviceStastics.UpdatedBy = UserId;
                    _serviceStasticsRepositoryArbitrage.Update(serviceStastics);

                    var depositSerMapping = _serviceTypeMappingArbitrage.GetSingle(x => x.ServiceId == Request.ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Deposit));
                    if (depositSerMapping != null)
                    {
                        depositSerMapping.Status = Request.IsDeposit;
                        depositSerMapping.UpdatedBy = UserId;
                        depositSerMapping.UpdatedDate = _basePage.UTC_To_IST();
                        _serviceTypeMappingArbitrage.Update(depositSerMapping);
                    }

                    var withdrawSerMapping = _serviceTypeMappingArbitrage.GetSingle(x => x.ServiceId == Request.ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Withdraw));
                    if (withdrawSerMapping != null)
                    {
                        withdrawSerMapping.Status = Request.IsWithdraw;
                        withdrawSerMapping.UpdatedBy = UserId;
                        withdrawSerMapping.UpdatedDate = _basePage.UTC_To_IST();
                        _serviceTypeMappingArbitrage.Update(withdrawSerMapping);
                    }

                    var tranSerMapping = _serviceTypeMappingArbitrage.GetSingle(x => x.ServiceId == Request.ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Transaction));
                    if (tranSerMapping != null)
                    {
                        tranSerMapping.Status = Request.IsTransaction;
                        tranSerMapping.UpdatedBy = UserId;
                        tranSerMapping.UpdatedDate = _basePage.UTC_To_IST();
                        _serviceTypeMappingArbitrage.Update(tranSerMapping);
                    }

                    var walletType = _walletTypeServiceArbitrage.GetById(serviceMaster.WalletTypeID);
                    if (walletType != null)
                    {
                        walletType.IsDepositionAllow = Request.IsDeposit;
                        walletType.IsWithdrawalAllow = Request.IsWithdraw;
                        walletType.IsTransactionWallet = Request.IsTransaction;
                        tranSerMapping.UpdatedBy = UserId;
                        tranSerMapping.UpdatedDate = _basePage.UTC_To_IST();

                        _walletTypeServiceArbitrage.Update(walletType);
                    }
                    var BaseCur = _marketRepositoryArbitrage.GetSingle(e => e.ServiceID == Request.ServiceId);
                    if (BaseCur != null)
                    {
                        BaseCur.Status = Request.IsBaseCurrency;
                        BaseCur.UpdatedBy = UserId;
                        _marketRepositoryArbitrage.Update(BaseCur);
                    }
                    //Check For SMSCode If Change Than Replace Name In Pair
                    if (OldSMSCode.Equals(Request.SMSCode) == false)
                    {
                        //Change In Pair Name Whre Coin Is As BaseCurrency

                        var BasePairData = _tradePairMasterArbitrageRepository.FindBy(x => x.BaseCurrencyId == serviceMaster.Id).ToList();

                        if (BasePairData.Count > 0)
                        {
                            foreach (var Pair in BasePairData)
                            {
                                var secondCurrency = _serviceMasterArbitrageRepository.GetById(Pair.SecondaryCurrencyId);
                                Pair.PairName = secondCurrency.SMSCode + "_" + serviceMaster.SMSCode;

                                _tradePairMasterArbitrageRepository.Update(Pair);
                            }

                        }

                        //Change In Pair Name Whre Coin Is As BaseCurrency

                        var SecondPairData = _tradePairMasterArbitrageRepository.FindBy(x => x.SecondaryCurrencyId == serviceMaster.Id).ToList();

                        if (SecondPairData.Count > 0)
                        {
                            foreach (var Pair1 in SecondPairData)
                            {
                                var baseCurrency = _serviceMasterArbitrageRepository.GetById(Pair1.BaseCurrencyId);
                                Pair1.PairName = serviceMaster.SMSCode + "_" + baseCurrency.SMSCode;

                                _tradePairMasterArbitrageRepository.Update(Pair1);
                            }
                        }

                        //Update WalletType Master WalletType Name Laster On Call WalletService

                        var walletTypeMaster = _walletTypeServiceArbitrage.GetSingle(x => x.WalletTypeName == OldSMSCode);
                        if (walletTypeMaster != null)
                        {
                            walletTypeMaster.WalletTypeName = Request.SMSCode;

                            _walletTypeServiceArbitrage.Update(walletTypeMaster);
                        }

                    }

                    return Request.ServiceId;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ServiceConfigurationRequest GetServiceConfigurationArbitrage(long ServiceId)
        {
            ServiceConfigurationRequest responsedata;
            try
            {
                responsedata = new ServiceConfigurationRequest();
                var serviceMaster = _serviceMasterArbitrageRepository.GetById(ServiceId);
                if (serviceMaster != null)
                {
                    responsedata.ServiceId = serviceMaster.Id;
                    responsedata.Name = serviceMaster.Name;
                    responsedata.SMSCode = serviceMaster.SMSCode;
                    responsedata.StatusText = Enum.GetName(typeof(ServiceStatus), serviceMaster.Status);
                    responsedata.Status = serviceMaster.Status;
                    // responsedata.Type = serviceMaster.ServiceType;

                    var serviceDetail = _serviceDetailRepositoryArbitrage.GetSingle(service => service.ServiceId == ServiceId);
                    var serviceDetailJson = JsonConvert.DeserializeObject<ServiceDetailJsonData>(serviceDetail.ServiceDetailJson);

                    // responsedata.ImageUrl = serviceDetailJson.ImageUrl;
                    responsedata.TotalSupply = serviceDetailJson.TotalSupply;
                    responsedata.MaxSupply = serviceDetailJson.MaxSupply;
                    //responsedata.ProofType = serviceDetailJson.ProofType;
                    //responsedata.EncryptionAlgorithm = serviceDetailJson.EncryptionAlgorithm;
                    responsedata.WebsiteUrl = serviceDetailJson.WebsiteUrl;
                    responsedata.Explorer = serviceDetailJson.Explorer;
                    responsedata.Community = serviceDetailJson.Community;
                    //responsedata.WhitePaperPath = serviceDetailJson.WhitePaperPath;
                    responsedata.Introduction = serviceDetailJson.Introduction;

                    var serviceStastics = _serviceStasticsRepositoryArbitrage.GetSingle(service => service.ServiceId == ServiceId);
                    responsedata.CirculatingSupply = serviceStastics.CirculatingSupply;
                    responsedata.IssueDate = serviceStastics.IssueDate;
                    responsedata.IssuePrice = serviceStastics.IssuePrice;

                    var depositSerMapping = _serviceTypeMappingArbitrage.GetSingle(x => x.ServiceId == ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Deposit));
                    if (depositSerMapping != null)
                    {
                        responsedata.IsDeposit = depositSerMapping.Status;
                    }

                    var withdrawSerMapping = _serviceTypeMappingArbitrage.GetSingle(x => x.ServiceId == ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Withdraw));
                    if (withdrawSerMapping != null)
                    {
                        responsedata.IsWithdraw = withdrawSerMapping.Status;
                    }

                    var tranSerMapping = _serviceTypeMappingArbitrage.GetSingle(x => x.ServiceId == ServiceId && x.TrnType == Convert.ToInt16(enTrnType.Transaction));
                    if (tranSerMapping != null)
                    {
                        responsedata.IsTransaction = tranSerMapping.Status;
                    }
                    var IsBaseCur = _marketRepositoryArbitrage.GetSingle(e => e.ServiceID == ServiceId);
                    if (IsBaseCur != null)
                    {
                        responsedata.IsBaseCurrency = IsBaseCur.Status;
                    }
                    return responsedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ServiceConfigurationGetAllResponse GetAllServiceConfigurationArbitrage(int? PageSize, int Page = 0, int StatusData = 0, short IsMargin = 0)
        {
            List<ServiceConfigurationRequest> responsedata;
            try
            {
                ServiceConfigurationGetAllResponse serviceConfigurationGetAllResponse = new ServiceConfigurationGetAllResponse();
                responsedata = new List<ServiceConfigurationRequest>();
                List<ServiceMasterResponse> ServiceData = new List<ServiceMasterResponse>();
                // if (IsMargin == 1)
                // {
                //     if (StatusData == 0)
                //     {
                //         ServiceData = _frontTrnRepository.GetAllServiceConfigurationMargin();
                //     }
                //     else
                //     {
                //         ServiceData = _frontTrnRepository.GetAllServiceConfigurationMargin(StatusData);
                //     } 
                // }
                // else
                // {
                if (StatusData == 0)
                {
                    ServiceData = _frontTrnRepository.GetAllServiceConfigurationArbitrage();
                }
                else
                {
                    ServiceData = _frontTrnRepository.GetAllServiceConfigurationArbitrage(StatusData);
                }
                //}
                //var serviceMaster = _serviceMasterRepository.List();
                if (ServiceData != null && ServiceData.Count > 0)
                {
                    foreach (var service in ServiceData)
                    {
                        ServiceConfigurationRequest response = new ServiceConfigurationRequest();
                        response.ServiceId = service.ServiceId;
                        response.Name = service.ServiceName;
                        response.SMSCode = service.SMSCode;
                        //response.Type = service.ServiceType;
                        response.StatusText = Enum.GetName(typeof(ServiceStatus), service.Status);
                        response.Status = service.Status;

                        //var serviceDetail = _serviceDetailRepository.GetSingle(ser => ser.ServiceId == service.Id);
                        var serviceDetailJson = JsonConvert.DeserializeObject<ServiceDetailJsonData>(service.ServiceDetailJson);

                        //response.ImageUrl = serviceDetailJson.ImageUrl;
                        response.TotalSupply = serviceDetailJson.TotalSupply;
                        response.MaxSupply = serviceDetailJson.MaxSupply;
                        //response.ProofType = serviceDetailJson.ProofType;
                        //response.EncryptionAlgorithm = serviceDetailJson.EncryptionAlgorithm;
                        response.WebsiteUrl = serviceDetailJson.WebsiteUrl;
                        response.Explorer = serviceDetailJson.Explorer;
                        response.Community = serviceDetailJson.Community;
                        //response.WhitePaperPath = serviceDetailJson.WhitePaperPath;
                        response.Introduction = serviceDetailJson.Introduction;

                        //var serviceStastics = _serviceStasticsRepository.GetSingle(ser => ser.ServiceId == service.Id);
                        response.CirculatingSupply = service.CirculatingSupply;
                        response.IssueDate = service.IssueDate;
                        response.IssuePrice = service.IssuePrice;

                        //var depositSerMapping = _serviceTypeMapping.GetSingle(x => x.ServiceId == service.Id && x.TrnType == Convert.ToInt16(enTrnType.Deposit));
                        //if (depositSerMapping != null)
                        //{
                        //    response.IsDeposit = depositSerMapping.Status;
                        //}

                        //var withdrawSerMapping = _serviceTypeMapping.GetSingle(x => x.ServiceId == service.Id && x.TrnType == Convert.ToInt16(enTrnType.Withdraw));
                        //if (withdrawSerMapping != null)
                        //{
                        //    response.IsWithdraw = withdrawSerMapping.Status;
                        //}

                        //var tranSerMapping = _serviceTypeMapping.GetSingle(x => x.ServiceId == service.Id && x.TrnType == Convert.ToInt16(enTrnType.Transaction));
                        //if (tranSerMapping != null)
                        //{
                        //    response.IsTransaction = tranSerMapping.Status;
                        //}
                        var IsBaseCur = _marketRepositoryArbitrage.GetSingle(e => e.ServiceID == service.ServiceId);
                        if (IsBaseCur != null)
                        {
                            response.IsBaseCurrency = IsBaseCur.Status;
                        }
                        response.IsDeposit = service.DepositBit;
                        response.IsWithdraw = service.WithdrawBit;
                        response.IsTransaction = service.TransactionBit;
                        response.WalletTypeID = service.WalletTypeID;
                        response.IsOnlyIntAmountAllow = service.IsOnlyIntAmountAllow;
                        responsedata.Add(response);
                    }
                    serviceConfigurationGetAllResponse.Response = responsedata;
                    serviceConfigurationGetAllResponse.Count = responsedata.Count;
                    if (PageSize != null && Page > 0)
                    {
                        int skip1 = Convert.ToInt32(PageSize) * (Page - 1);
                        var it = Convert.ToDouble(serviceConfigurationGetAllResponse.Response.Count) / PageSize;
                        var fl = Math.Ceiling((decimal)it);
                        serviceConfigurationGetAllResponse.Response = serviceConfigurationGetAllResponse.Response.Skip(skip1).Take(Convert.ToInt32(PageSize)).ToList();
                        serviceConfigurationGetAllResponse.PageSize = Convert.ToInt64(PageSize);
                        serviceConfigurationGetAllResponse.TotalPage = Convert.ToInt64(fl);
                    }
                    else
                    {
                        serviceConfigurationGetAllResponse.PageSize = responsedata.Count;
                        serviceConfigurationGetAllResponse.TotalPage = 0;
                    }
                    return serviceConfigurationGetAllResponse;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<MarketViewModel> GetAllMarketDataArbitrage(short ActiveOnly = 1)
        {
            List<MarketArbitrage> modellist = new List<MarketArbitrage>();
            try
            {
                List<MarketViewModel> list = new List<MarketViewModel>();
                modellist = _marketRepositoryArbitrage.List();

                if (ActiveOnly == 1)
                    modellist = modellist.Where(e => e.Status == 1).ToList();

                foreach (var model in modellist)
                {
                    list.Add(new MarketViewModel()
                    {
                        ID = model.Id,
                        CurrencyName = model.CurrencyName,
                        ServiceID = model.ServiceID,
                        isBaseCurrency = model.isBaseCurrency,
                        Status = Enum.GetName(typeof(ServiceStatus), model.Status),
                        CurrencyDesc = _serviceMasterArbitrageRepository.GetSingle(s => s.Id == model.ServiceID).Name,
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }
        public List<ServiceCurrencyData> GetAllServiceConfigurationByBaseArbitrage(String Base)
        {
            List<ServiceCurrencyData> responsedata;
            try
            {
                var CheckBase = _marketRepositoryArbitrage.GetSingle(m => m.CurrencyName == Base);
                if (CheckBase == null)
                    return null;
                responsedata = new List<ServiceCurrencyData>();
                var model = _serviceMasterArbitrageRepository.GetSingle(ser => ser.SMSCode == Base);
                if (model == null)
                    return null;
                var serviceid = model.Id;

                var modellist = _serviceMasterArbitrageRepository.List();
                foreach (var modelData in modellist)
                {
                    if (modelData.Id == serviceid)
                        continue;
                    responsedata.Add(new ServiceCurrencyData()
                    {
                        Name = modelData.Name,
                        ServiceId = modelData.Id,
                        SMSCode = modelData.SMSCode
                    });
                }
                return responsedata;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int SetActiveServiceArbitrage(long ServiceId)
        {
            try
            {
                var servicedata = _serviceMasterArbitrageRepository.GetById(ServiceId);
                if (servicedata != null)
                {
                    servicedata.SetActiveService();
                    _serviceMasterArbitrageRepository.Update(servicedata);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public int SetInActiveServiceArbitrage(long ServiceId)
        {
            try
            {
                var servicedata = _serviceMasterArbitrageRepository.GetById(ServiceId);
                if (servicedata != null)
                {
                    servicedata.SetInActiveService();
                    _serviceMasterArbitrageRepository.Update(servicedata);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetServiceByBaseReasponse GetCurrencyArbitrage(short ActiveOnly = 0)
        {
            try
            {
                GetServiceByBaseReasponse _Res = new GetServiceByBaseReasponse();
                List<ServiceCurrencyData> list = new List<ServiceCurrencyData>();
                var modelList = _serviceMasterArbitrageRepository.List();
                if (ActiveOnly == 1)
                    modelList = modelList.Where(e => e.Status == 1).ToList();
                foreach (var modelData in modelList)
                {
                    list.Add(new ServiceCurrencyData()
                    {
                        Name = modelData.Name,
                        ServiceId = modelData.Id,
                        SMSCode = modelData.SMSCode
                    });
                }
                _Res.Response = list;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = EnResponseMessage.CommSuccessMsgInternal;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long AddTradeRouteConfigurationArbitrage(TradeRouteConfigRequest Request, long UserId)
        {
            try
            {
                //Check Already Available Or Not
                var Data = _routeConfigRepositoryArbitrage.GetSingle(x => x.PairId == Request.PairId && x.OrderType == Request.OrderType && x.TrnType == (enTrnType)Request.TrnType && x.SerProDetailID == Request.RouteUrlId);
                if (Data != null)
                {
                    return -1;
                }
                else
                {
                    var PairData = _tradePairMasterArbitrageRepository.GetById(Request.PairId);
                    if (PairData == null)
                        return -2;
                    RouteConfigurationArbitrage route = new RouteConfigurationArbitrage()
                    {
                        RouteName = PairData.PairName + "_" + (enTrnType)Request.TrnType,
                        ServiceID = 0,
                        SerProDetailID = Request.RouteUrlId,
                        ProductID = 0,
                        Priority = 0,
                        //StatusCheckUrl = null,
                        //ValidationUrl = null,
                        //TransactionUrl = null,
                        LimitId = 0,
                        OpCode = Request.AssetName,
                        TrnType = (enTrnType)Request.TrnType,
                        IsDelayAddress = 0,
                        ProviderWalletID = "0",
                        Status = Convert.ToInt16(ServiceStatus.Active),
                        PairId = Request.PairId,
                        OrderType = Request.OrderType,
                        CreatedDate = _basePage.UTC_To_IST(),
                        CreatedBy = UserId,
                        ConfirmationCount = Request.ConfirmationCount,
                        ConvertAmount = Request.ConvertAmount
                    };
                    var newProduct = _routeConfigRepositoryArbitrage.Add(route);
                    return newProduct.Id;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;

            }
        }

        public long UpdateTradeRouteConfigurationArbitrage(TradeRouteConfigRequest Request, long UserId)
        {
            try
            {
                //Check Same As Previous Data
                var PreviousData = _routeConfigRepositoryArbitrage.GetById(Request.Id);
                if (PreviousData != null)
                {
                    if (PreviousData.PairId == Request.PairId && PreviousData.TrnType == (enTrnType)Request.TrnType && PreviousData.OrderType == Request.OrderType)
                    {
                        PreviousData.OpCode = Request.AssetName;
                        PreviousData.Status = Convert.ToInt16(ServiceStatus.Active);
                        PreviousData.ConfirmationCount = Request.ConfirmationCount;
                        PreviousData.ConvertAmount = Request.ConvertAmount;

                        _routeConfigRepositoryArbitrage.Update(PreviousData);

                        return PreviousData.Id;
                    }
                }

                //Check Already Available Or Not
                var Data = _routeConfigRepositoryArbitrage.GetSingle(x => x.PairId == Request.PairId && x.OrderType == Request.OrderType && x.TrnType == (enTrnType)Request.TrnType && x.SerProDetailID == Request.RouteUrlId);
                if (Data != null)
                {
                    return -1;
                }
                else
                {
                    var PairData = _tradePairMasterArbitrageRepository.GetById(Request.PairId);
                    if (PairData == null)
                        return -2;
                    var RouteConfigurationData = _routeConfigRepositoryArbitrage.GetById(Request.Id);

                    if (RouteConfigurationData != null)
                    {
                        RouteConfigurationData.RouteName = PairData.PairName + "_" + (enTrnType)Request.TrnType;
                        RouteConfigurationData.OpCode = Request.AssetName;
                        RouteConfigurationData.TrnType = (enTrnType)Request.TrnType;
                        RouteConfigurationData.Status = Convert.ToInt16(ServiceStatus.Active);
                        RouteConfigurationData.PairId = Request.PairId;
                        RouteConfigurationData.OrderType = Request.OrderType;
                        RouteConfigurationData.ConfirmationCount = Request.ConfirmationCount;
                        RouteConfigurationData.ConvertAmount = Request.ConvertAmount;

                        _routeConfigRepositoryArbitrage.Update(RouteConfigurationData);

                        return RouteConfigurationData.Id;
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }


        }

        public List<GetTradeRouteConfigurationData> GetAllTradeRouteConfigurationArbitrage()
        {
            try
            {
                var Data = _backOfficeTrnRepository.GetTradeRouteConfigurationArbitrage(0);

                var Data1 = from TradeRoute in Data
                            select new GetTradeRouteConfigurationData
                            {
                                Id = TradeRoute.Id,
                                AssetName = TradeRoute.AssetName,
                                PairId = TradeRoute.PairId,
                                PairName = TradeRoute.PairName,
                                OrderType = TradeRoute.OrderType,
                                OrderTypeText = ((enTransactionMarketType)TradeRoute.OrderType).ToString(),
                                Status = TradeRoute.Status,
                                StatusText = ((ServiceStatus)TradeRoute.Status).ToString(),
                                TrnType = TradeRoute.TrnType,
                                TrnTypeText = ((enTrnType)TradeRoute.TrnType).ToString(),
                                TradeRouteName = TradeRoute.TradeRouteName,
                                ConfirmationCount = TradeRoute.ConfirmationCount,
                                ConvertAmount = TradeRoute.ConvertAmount,
                                Priority = TradeRoute.Priority,
                                RouteUrl = TradeRoute.RouteUrl,
                                RouteUrlId = TradeRoute.RouteUrlId
                            };

                return Data1.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public GetTradeRouteConfigurationData GetTradeRouteConfigurationArbitrageInfo(long Id)
        {
            try
            {
                var Data = _backOfficeTrnRepository.GetTradeRouteConfigurationArbitrage(Id);

                var Data1 = from TradeRoute in Data
                            select new GetTradeRouteConfigurationData
                            {
                                Id = TradeRoute.Id,
                                AssetName = TradeRoute.AssetName,
                                PairId = TradeRoute.PairId,
                                PairName = TradeRoute.PairName,
                                OrderType = TradeRoute.OrderType,
                                OrderTypeText = ((enTransactionMarketType)TradeRoute.OrderType).ToString(),
                                Status = TradeRoute.Status,
                                StatusText = ((ServiceStatus)TradeRoute.Status).ToString(),
                                TrnType = TradeRoute.TrnType,
                                TrnTypeText = ((enTrnType)TradeRoute.TrnType).ToString(),
                                TradeRouteName = TradeRoute.TradeRouteName,
                                ConfirmationCount = TradeRoute.ConfirmationCount,
                                ConvertAmount = TradeRoute.ConvertAmount,
                                Priority = TradeRoute.Priority,
                                RouteUrl = TradeRoute.RouteUrl,
                                RouteUrlId = TradeRoute.RouteUrlId
                            };

                return Data1.ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public long UpdateTradeRoutePriorityArbitrage(TradeRoutePriorityUpdateRequest Request, long UserId)
        {
            try
            {
                foreach (var TradeRoute in Request.TradeRoute)
                {
                    var Data = _routeConfigRepositoryArbitrage.GetById(TradeRoute.Id);

                    if (Data != null)
                    {
                        Data.Priority = TradeRoute.Priority;
                        _routeConfigRepositoryArbitrage.Update(Data);
                    }
                }
                return 1;
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
                var Data = _backOfficeTrnRepository.GetTradeRouteForPriorityArbitrage(PairId, OrderType, TrnType);

                var Data1 = from TradeRoute in Data
                            select new GetTradeRouteConfigurationData
                            {
                                Id = TradeRoute.Id,
                                AssetName = TradeRoute.AssetName,
                                PairId = TradeRoute.PairId,
                                PairName = TradeRoute.PairName,
                                OrderType = TradeRoute.OrderType,
                                OrderTypeText = ((enTransactionMarketType)TradeRoute.OrderType).ToString(),
                                Status = TradeRoute.Status,
                                StatusText = ((ServiceStatus)TradeRoute.Status).ToString(),
                                TrnType = TradeRoute.TrnType,
                                TrnTypeText = ((enTrnType)TradeRoute.TrnType).ToString(),
                                TradeRouteName = TradeRoute.TradeRouteName,
                                ConfirmationCount = TradeRoute.ConfirmationCount,
                                ConvertAmount = TradeRoute.ConvertAmount,
                                Priority = TradeRoute.Priority,
                                RouteUrl = TradeRoute.RouteUrl,
                                RouteUrlId = TradeRoute.RouteUrlId
                            };

                return Data1.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<AvailableRoute> GetAvailableTradeRouteArbitrage(int TrnType)
        {
            try
            {
                var Data = _backOfficeTrnRepository.GetAvailableTradeRouteArbitrageInfo(TrnType);
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public BizResponseClass AddMarketDataV2Arbitrage(MarketDataRequest Request, long UserId)
        {
            try
            {
                BizResponseClass res = new BizResponseClass();
                var obj = _marketRepositoryArbitrage.GetSingle(e => e.ServiceID == Request.ServiceID);
                if (obj != null)
                {
                    if (obj.ServiceID == Request.ServiceID)
                    {
                        res.ErrorCode = enErrorCode.MarketAlreadyExist;
                        res.ReturnCode = enResponseCode.Fail;
                        res.ReturnMsg = EnResponseMessage.CommFailMsgInternal;
                        return res;
                    }
                }
                MarketArbitrage market = new MarketArbitrage
                {
                    CurrencyName = Request.CurrencyName,
                    ServiceID = Request.ServiceID,
                    isBaseCurrency = 1,
                    Status = Request.Status,
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST()
                };
                var newMarket = _marketRepositoryArbitrage.Add(market);
                if (newMarket.Id != 0)
                {
                    res.ErrorCode = enErrorCode.Success;
                    res.ReturnCode = enResponseCode.Success;
                    res.ReturnMsg = EnResponseMessage.CommRecordInsertSuccess;
                    return res;
                }
                res.ErrorCode = enErrorCode.DataInsertFail;
                res.ReturnCode = enResponseCode.Fail;
                res.ReturnMsg = EnResponseMessage.CommFailMsgInternal;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public MarketDataResponse UpdateMarketDataV2Arbitrage(MarketDataRequest Request, long UserId)
        {
            try
            {
                MarketDataResponse res = new MarketDataResponse();
                var model = _marketRepositoryArbitrage.GetById(Request.ID);
                if (model == null)
                {
                    res.Response = null;
                    res.ErrorCode = enErrorCode.DataInsertFail;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.CommFailMsgInternal;
                    return res;
                }
                //model.ServiceID = Request.ServiceID;
                model.Status = Request.Status;
                // model.CurrencyName = Request.CurrencyName;
                model.isBaseCurrency = 1;
                model.UpdatedBy = UserId;
                model.UpdatedDate = DateTime.UtcNow;

                _marketRepositoryArbitrage.Update(model);

                model = _marketRepositoryArbitrage.GetById(Request.ID);
                MarketInfo market = new MarketInfo();
                market.ID = model.Id;
                market.ServiceID = model.ServiceID;
                market.Status = model.Status;
                market.CurrencyName = model.CurrencyName;

                res.Response = market;
                res.ErrorCode = enErrorCode.Success;
                res.ReturnCode = enResponseCode.Success;
                res.ReturnMsg = EnResponseMessage.CommSuccessMsgInternal;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public IEnumerable<ProviderDetailViewModel> GetProviderDetailListArbitrage()
        {
            try
            {
                var list = _proDetailArbitrageRepository.List();
                List<ProviderDetailViewModel> providerList = new List<ProviderDetailViewModel>();
                foreach (ServiceProviderDetailArbitrage model in list)
                {
                    providerList.Add(new ProviderDetailViewModel
                    {
                        Id = model.Id,
                        ServiceProID = model.ServiceProID,
                        SerProDetailName = model.SerProDetailName,
                        Status = model.Status,
                        ProTypeID = model.ProTypeID,
                        AppTypeID = model.AppTypeID,
                        TrnTypeID = model.TrnTypeID,
                        LimitID = model.LimitID,
                        DemonConfigID = model.DemonConfigID,
                        ServiceProConfigID = model.ServiceProConfigID,
                        ThirPartyAPIID = model.ThirPartyAPIID
                    });
                }
                return providerList;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public IEnumerable<ProviderDetailGetAllResponse> getProviderDetailsDataListArbitrage(IEnumerable<ProviderDetailViewModel> dataList)
        {
            try
            {
                List<ProviderDetailGetAllResponse> responcesData = new List<ProviderDetailGetAllResponse>();
                foreach (ProviderDetailViewModel viewmodel in dataList)
                {
                    responcesData.Add(new ProviderDetailGetAllResponse
                    {
                        Id = viewmodel.Id,
                        SerProDetailName = viewmodel.SerProDetailName,
                        Status = viewmodel.Status,
                        Provider = GetPoviderByIDArbitrage(viewmodel.ServiceProID),
                        ProviderType = GetProviderTypeByIdArbitrage(viewmodel.ProTypeID),
                        AppType = GetAppTypeById(viewmodel.AppTypeID),
                        TrnType = viewmodel.TrnTypeID,
                        Limit = GetLimitByIdArbitrage(viewmodel.LimitID),
                        DemonConfiguration = GetDemonConfigurationArbitrage(viewmodel.DemonConfigID),
                        ProviderConfiguration = GetProviderConfigurationArbitrage(viewmodel.ServiceProConfigID),
                        thirdParty = GetThirdPartyAPIConfigByIdArbitrage(viewmodel.ThirPartyAPIID),
                    });
                }
                return responcesData;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        #endregion

        #region ArbitrageProviderRelatedMethods
        //Darshan Dholakiya added this method for the Provider Arbitrage method changes:17-06-2019
        public ProviderTypeViewModel GetProviderTypeByIdArbitrage(long id)
        {
            try
            {
                ServiceProviderTypeArbitrage model = _ProviderTypeRepositoryArbitrage.GetById(id);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new ProviderTypeViewModel
                {
                    Id = model.Id,
                    ServiveProTypeName = model.ServiveProTypeName,
                    //Status = model.Status

                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public LimitViewModel GetLimitByIdArbitrage(long id)
        {
            try
            {
                var model = _limitRepositoryArbitrage.GetById(id);
                if (model == null)
                    return null;

                var viewModel = new LimitViewModel()
                {
                    Id = model.Id,
                    MaxAmt = model.MaxAmt,
                    MaxAmtDaily = model.MaxAmtDaily,
                    MaxAmtMonthly = model.MaxAmtMonthly,
                    MaxAmtWeekly = model.MaxAmtWeekly,
                    Maxrange = model.Maxrange,
                    MaxRangeDaily = model.MaxRangeDaily,
                    MaxRangeMonthly = model.MaxRangeMonthly,
                    MaxRangeWeekly = model.MaxRangeWeekly,
                    MinAmt = model.MinAmt,
                    MinAmtDaily = model.MinAmtDaily,
                    MinAmtMonthly = model.MinAmtMonthly,
                    MinAmtWeekly = model.MinAmtWeekly,
                    MinRange = model.MinRange,
                    MinRangeDaily = model.MinRangeDaily,
                    MinRangeMonthly = model.MinRangeMonthly,
                    MinRangeWeekly = model.MinRangeWeekly,
                    Status = model.Status
                };
                return viewModel;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public ProviderConfigurationViewModel GetProviderConfigurationArbitrage(long id)
        {
            try
            {
                ServiceProConfigurationArbitrage model = _ProviderConfigurationArbitrage.GetById(id);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new ProviderConfigurationViewModel
                {
                    Id = model.Id,
                    APIKey = model.APIKey,
                    AppKey = model.AppKey,
                    SecretKey = model.SecretKey,
                    Password = model.Password,
                    UserName = model.UserName,
                    status = model.Status,
                    StatusText = Enum.GetName(typeof(ServiceStatus), model.Status)
                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ThirdPartyAPIConfigViewModel GetThirdPartyAPIConfigByIdArbitrage(long Id)
        {
            try
            {
                ArbitrageThirdPartyAPIConfiguration model = _thirdPartyAPIRepositoryArbitrage.GetById(Id);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new ThirdPartyAPIConfigViewModel
                {
                    Id = model.Id,
                    APIBalURL = model.APIBalURL,
                    APIName = model.APIName,
                    APIRequestBody = model.APIRequestBody,
                    APISendURL = model.APISendURL,
                    APIStatusCheckURL = model.APIStatusCheckURL,
                    APIValidateURL = model.APIValidateURL,
                    AppType = model.AppType,
                    AuthHeader = model.AuthHeader,
                    ContentType = model.ContentType,
                    HashCode = model.HashCode,
                    HashCodeRecheck = model.HashCodeRecheck,
                    HashType = model.HashType,
                    MerchantCode = model.MerchantCode,
                    MethodType = model.MethodType,
                    ParsingDataID = model.ParsingDataID,
                    ResponseFailure = model.ResponseFailure,
                    ResponseHold = model.ResponseHold,
                    ResponseSuccess = model.ResponseSuccess,
                    TransactionIdPrefix = model.TransactionIdPrefix,
                    AppTypeText = ((enAppType)model.AppType).ToString(),
                    Status = model.Status
                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public DemonconfigurationViewModel GetDemonConfigurationArbitrage(long id)
        {
            try
            {
                var model = _DemonRepositoryArbitrage.GetById(id);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new DemonconfigurationViewModel
                {
                    Id = model.Id,
                    IPAdd = model.IPAdd,
                    PortAdd = model.PortAdd,
                    Url = model.Url,
                    Status = model.Status,
                    StatusText = Enum.GetName(typeof(ServiceStatus), model.Status)
                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ProviderDetailViewModel GetProviderDetailByIdArbitrage(long id)
        {
            try
            {
                var model = _proDetailArbitrageRepository.GetById(id);
                if (model == null)
                {
                    return null;
                }
                var viewmodel = new ProviderDetailViewModel
                {
                    Id = model.Id,
                    ServiceProID = model.ServiceProID,
                    ProTypeID = model.ProTypeID,
                    AppTypeID = model.AppTypeID,
                    TrnTypeID = model.TrnTypeID,
                    LimitID = model.LimitID,
                    SerProDetailName = model.SerProDetailName,
                    DemonConfigID = model.DemonConfigID,
                    ServiceProConfigID = model.ServiceProConfigID,
                    ThirPartyAPIID = model.ThirPartyAPIID,
                };
                return viewmodel;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ProviderDetailGetAllResponse getProviderDetailDataByIdArbitrage(ProviderDetailViewModel viewModel)
        {
            try
            {
                ProviderDetailGetAllResponse res = new ProviderDetailGetAllResponse();

                res.Id = viewModel.Id;
                res.SerProDetailName = viewModel.SerProDetailName;
                res.Status = viewModel.Status;
                res.Provider = GetPoviderByIDArbitrage(viewModel.ServiceProID);
                res.ProviderType = GetProviderTypeByIdArbitrage(viewModel.ProTypeID);
                res.AppType = GetAppTypeById(viewModel.AppTypeID);
                res.TrnType = viewModel.TrnTypeID;
                res.Limit = GetLimitByIdArbitrage(viewModel.LimitID);
                res.DemonConfiguration = GetDemonConfigurationArbitrage(viewModel.DemonConfigID);
                res.ProviderConfiguration = GetProviderConfigurationArbitrage(viewModel.ServiceProConfigID);
                res.thirdParty = GetThirdPartyAPIConfigByIdArbitrage(viewModel.ThirPartyAPIID);

                return res;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long AddProviderDetailArbitrage(ProviderDetailRequest request, long UserId)
        {
            try
            {
                ServiceProviderDetailArbitrage model = new ServiceProviderDetailArbitrage
                {
                    ServiceProID = request.ServiceProID,
                    ProTypeID = request.ProTypeID,
                    AppTypeID = request.AppTypeID,
                    TrnTypeID = request.TrnTypeID,
                    LimitID = request.LimitID,
                    DemonConfigID = request.DemonConfigID,
                    ServiceProConfigID = request.ServiceProConfigID,
                    ThirPartyAPIID = request.ThirPartyAPIID,
                    Status = request.Status,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId,
                    SerProDetailName = request.SerProDetailName
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };

                var newModel = _proDetailArbitrageRepository.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool UpdateProviderDetailArbitrage(ProviderDetailRequest request, long UserId)
        {
            try
            {
                ServiceProviderDetailArbitrage model = _proDetailArbitrageRepository.GetById(request.Id);
                if (model == null)
                {
                    return false;
                }
                model.ServiceProID = request.ServiceProID;
                model.ProTypeID = request.ProTypeID;
                model.AppTypeID = request.AppTypeID;
                model.TrnTypeID = request.TrnTypeID;
                model.LimitID = request.LimitID;
                model.DemonConfigID = request.DemonConfigID;
                model.ServiceProConfigID = request.ServiceProConfigID;
                model.ThirPartyAPIID = request.ThirPartyAPIID;
                model.UpdatedBy = UserId;
                model.UpdatedDate = _basePage.UTC_To_IST();
                model.SerProDetailName = request.SerProDetailName;
                model.Status = request.Status;
                _proDetailArbitrageRepository.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetActiveProviderDetailArbitrage(long id)
        {
            try
            {
                ServiceProviderDetailArbitrage model = _proDetailArbitrageRepository.GetById(id);
                if (model != null)
                {
                    model.EnableProvider();
                    _proDetailArbitrageRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetInActiveProviderDetailArbitrage(long id)
        {
            try
            {
                ServiceProviderDetailArbitrage model = _proDetailArbitrageRepository.GetById(id);
                if (model != null)
                {
                    model.DisableProvider();
                    _proDetailArbitrageRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }



        #endregion
        #region ArbitrageProductConfig
        public long AddProductConfigurationArbitrage(ProductConfigurationRequest Request, long UserId)
        {
            try
            {
                ProductConfigurationArbitrage product = new ProductConfigurationArbitrage()
                {
                    ProductName = Request.ProductName,
                    ServiceID = Request.ServiceID,
                    CountryID = Request.CountryID,
                    Status = Convert.ToInt16(ServiceStatus.Active),
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };
                var newProduct = _productConfigRepositoryArbitrage.Add(product);
                return newProduct.Id;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long UpdateProductConfigurationArbitrage(ProductConfigurationRequest Request, long UserId)
        {
            try
            {
                var product = _productConfigRepositoryArbitrage.GetById(Request.Id);
                if (product != null)
                {
                    product.ProductName = Request.ProductName;
                    product.ServiceID = Request.ServiceID;
                    product.CountryID = Request.CountryID;
                    product.UpdatedDate = _basePage.UTC_To_IST();
                    product.UpdatedBy = UserId;

                    _productConfigRepositoryArbitrage.Update(product);
                    return product.Id;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ProductConfigrationGetInfo GetProductConfigurationArbitrage(long ProductId)
        {

            ProductConfigrationGetInfo responsedata;
            try
            {
                responsedata = new ProductConfigrationGetInfo();
                var product = _productConfigRepositoryArbitrage.GetById(ProductId);
                if (product != null)
                {
                    responsedata.Id = product.Id;
                    responsedata.ProductName = product.ProductName;

                    var serviceMaster = _serviceMasterArbitrageRepository.GetById(product.ServiceID);
                    var countryMaster = _countryMasterRepository.GetById(product.CountryID);
                    responsedata.CountryID = product.CountryID;
                    responsedata.ServiceID = product.ServiceID;
                    responsedata.ServiceName = serviceMaster.Name;
                    responsedata.CountryName = countryMaster.CountryName;

                    return responsedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ProductConfigrationGetInfo> GetAllProductConfigurationArbitrage()
        {
            List<ProductConfigrationGetInfo> responsedata;
            try
            {
                responsedata = new List<ProductConfigrationGetInfo>();
                //Uday 15-11-2018 applied direct query because one by one record pick has take long time to response
                //var productall = _productConfigRepository.List();
                var productall = _backOfficeTrnRepository.GetAllProductConfigurationArbitrageInfo();
                if (productall != null && productall.Count > 0)
                {
                    foreach (var product in productall)
                    {
                        ProductConfigrationGetInfo response = new ProductConfigrationGetInfo();
                        response.Id = product.Id;
                        response.ProductName = product.ProductName;

                        //var serviceMaster = _serviceMasterRepository.GetById(product.ServiceID);
                        //var countryMaster = _countryMasterRepository.GetById(product.CountryID);
                        response.CountryID = product.CountryID;
                        response.ServiceID = product.ServiceID;
                        response.ServiceName = product.ServiceName;
                        response.CountryName = product.CountryName;

                        responsedata.Add(response);
                    }
                    return responsedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public int SetActiveProductArbitrage(long ProductId)
        {
            try
            {
                var product = _productConfigRepositoryArbitrage.GetById(ProductId);
                if (product != null)
                {
                    product.SetActiveProduct();
                    _productConfigRepositoryArbitrage.Update(product);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public int SetInActiveProductArbitrage(long ProductId)
        {
            try
            {
                var product = _productConfigRepositoryArbitrage.GetById(ProductId);
                if (product != null)
                {
                    product.SetInActiveProduct();
                    _productConfigRepositoryArbitrage.Update(product);
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public ThirdPartyAPIConfigResponseAllData GetAllThirdPartyAPIConfigArbitrage(int? PageSize, int Page)
        {
            try
            {
                var list = _thirdPartyAPIRepositoryArbitrage.List();
                ThirdPartyAPIConfigResponseAllData ObjThirdPartyPagination = new ThirdPartyAPIConfigResponseAllData();

                List<ThirdPartyAPIConfigViewModel> thirdPartyAPIs = new List<ThirdPartyAPIConfigViewModel>();
                foreach (ArbitrageThirdPartyAPIConfiguration model in list)
                {
                    thirdPartyAPIs.Add(new ThirdPartyAPIConfigViewModel()
                    {
                        Id = model.Id,
                        APIBalURL = model.APIBalURL,
                        APIName = model.APIName,
                        APIRequestBody = model.APIRequestBody,
                        APISendURL = model.APISendURL,
                        APIStatusCheckURL = model.APIStatusCheckURL,
                        APIValidateURL = model.APIValidateURL,
                        AppType = model.AppType,
                        AuthHeader = model.AuthHeader,
                        ContentType = model.ContentType,
                        HashCode = model.HashCode,
                        HashCodeRecheck = model.HashCodeRecheck,
                        HashType = model.HashType,
                        MerchantCode = model.MerchantCode,
                        MethodType = model.MethodType,
                        ParsingDataID = model.ParsingDataID,
                        ResponseFailure = model.ResponseFailure,
                        ResponseHold = model.ResponseHold,
                        ResponseSuccess = model.ResponseSuccess,
                        TransactionIdPrefix = model.TransactionIdPrefix,
                        AppTypeText = ((enAppType)model.AppType).ToString(),
                        Status = model.Status
                    });
                }
                ObjThirdPartyPagination.Count = thirdPartyAPIs.Count;
                ObjThirdPartyPagination.Response = thirdPartyAPIs;
                //This Customize pagination donot change without asking mobile and web team ----Darshan Dholakiya:25-07-2019
                if (PageSize != null && Page > 0)
                {
                    int skip1 = Convert.ToInt32(PageSize) * (Page - 1);
                    var it = Convert.ToDouble(ObjThirdPartyPagination.Response.Count) / PageSize;
                    var fl = Math.Ceiling((decimal)it);
                    ObjThirdPartyPagination.Response = ObjThirdPartyPagination.Response.Skip(skip1).Take(Convert.ToInt32(PageSize)).ToList();
                    ObjThirdPartyPagination.PageSize = Convert.ToInt64(PageSize);
                    ObjThirdPartyPagination.TotalPage = Convert.ToInt64(fl);
                }
                else
                {
                    ObjThirdPartyPagination.PageSize = thirdPartyAPIs.Count;
                    ObjThirdPartyPagination.TotalPage = 0;
                }
                return ObjThirdPartyPagination;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public long AddThirdPartyAPIArbitrage(ThirdPartyAPIConfigRequest request, long UserId)
        {
            try
            {
                var model = new ArbitrageThirdPartyAPIConfiguration()
                {
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST(),
                    Status = request.Status,
                    APIBalURL = request.APIBalURL,
                    APIName = request.APIName,
                    APIRequestBody = request.APIRequestBody,
                    APISendURL = request.APISendURL,
                    APIStatusCheckURL = request.APIStatusCheckURL,
                    APIValidateURL = request.APIValidateURL,
                    AppType = request.AppType,
                    AuthHeader = request.AuthHeader,
                    ContentType = request.ContentType,
                    HashCode = "",
                    HashCodeRecheck = "",
                    HashType = 0,
                    MerchantCode = request.MerchantCode,
                    MethodType = request.MethodType,
                    ParsingDataID = request.ParsingDataID,
                    ResponseFailure = request.ResponseFailure,
                    ResponseHold = request.ResponseHold,
                    ResponseSuccess = request.ResponseSuccess,
                    TransactionIdPrefix = request.TransactionIdPrefix

                };
                var newModel = _thirdPartyAPIRepositoryArbitrage.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public bool UpdateThirdPartyAPIArbitrage(ThirdPartyAPIConfigRequest request, long UserId)
        {
            try
            {
                var model = _thirdPartyAPIRepositoryArbitrage.GetById(request.Id);
                if (model == null)
                {
                    return false;
                }

                model.APIBalURL = request.APIBalURL;
                model.APIName = request.APIName;
                model.APIRequestBody = request.APIRequestBody;
                model.APISendURL = request.APISendURL;
                model.APIStatusCheckURL = request.APIStatusCheckURL;
                model.APIValidateURL = request.APIValidateURL;
                model.AppType = request.AppType;
                model.AuthHeader = request.AuthHeader;
                model.ContentType = request.ContentType;
                model.MerchantCode = request.MerchantCode;
                model.MethodType = request.MethodType;
                model.ParsingDataID = request.ParsingDataID;
                model.ResponseFailure = request.ResponseFailure;
                model.ResponseHold = request.ResponseHold;
                model.ResponseSuccess = request.ResponseSuccess;
                model.TransactionIdPrefix = request.TransactionIdPrefix;
                model.UpdatedDate = _basePage.UTC_To_IST();
                model.UpdatedBy = UserId;
                model.Status = request.Status;
                _thirdPartyAPIRepositoryArbitrage.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetActiveThirdPartyAPIArbitrage(long id)
        {
            try
            {
                ArbitrageThirdPartyAPIConfiguration model = _thirdPartyAPIRepositoryArbitrage.GetById(id);
                if (model != null)
                {
                    model.SetActive();
                    _thirdPartyAPIRepositoryArbitrage.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetInActiveThirdPartyAPIArbitrage(long id)
        {
            try
            {
                ArbitrageThirdPartyAPIConfiguration model = _thirdPartyAPIRepositoryArbitrage.GetById(id);
                if (model != null)
                {
                    model.SetInActive();
                    _thirdPartyAPIRepositoryArbitrage.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ThirdPartyAPIResponseConfigResponseAllData GetAllThirdPartyAPIResponseArbitrage(int? PageSize, int Page)
        {
            try
            {
                ThirdPartyAPIResponseConfigResponseAllData ObjTpartyRespPage = new ThirdPartyAPIResponseConfigResponseAllData();
                var list = _thirdPartyAPIResRepositoryArbitrage.List();
                List<ThirdPartyAPIResponseConfigViewModel> APIResponseList = new List<ThirdPartyAPIResponseConfigViewModel>();
                foreach (var model in list)
                {
                    APIResponseList.Add(new ThirdPartyAPIResponseConfigViewModel()
                    {
                        BalanceRegex = model.BalanceRegex,
                        ErrorCodeRegex = model.ErrorCodeRegex,
                        Id = model.Id,
                        OprTrnRefNoRegex = model.OprTrnRefNoRegex,
                        Param1Regex = model.Param1Regex,
                        Param2Regex = model.Param2Regex,
                        Param3Regex = model.Param3Regex,
                        ResponseCodeRegex = model.ResponseCodeRegex,
                        StatusMsgRegex = model.StatusMsgRegex,
                        StatusRegex = model.StatusRegex,
                        TrnRefNoRegex = model.TrnRefNoRegex,
                        Status = model.Status
                    });
                }
                ObjTpartyRespPage.Response = APIResponseList;
                ObjTpartyRespPage.Count = ObjTpartyRespPage.Response.Count;
                //This is customize pagination donot change without asking mobile and web team Darshan Dholakiya---25-07-2019
                if (PageSize != null && Page > 0)
                {
                    var it = Convert.ToDouble(ObjTpartyRespPage.Response.Count) / PageSize;
                    var fl = Math.Ceiling((decimal)it);
                    int skip = Convert.ToInt32(PageSize) * (Page - 1);
                    ObjTpartyRespPage.Response = ObjTpartyRespPage.Response.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    ObjTpartyRespPage.TotalPage = Convert.ToInt64(fl);
                    ObjTpartyRespPage.PageSize = Convert.ToInt64(PageSize);
                }
                else
                {
                    ObjTpartyRespPage.PageSize = APIResponseList.Count;
                    ObjTpartyRespPage.TotalPage = 0;
                }
                return ObjTpartyRespPage;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public ThirdPartyAPIResponseConfigViewModel GetThirdPartyAPIResponseByIdArbitrage(long id)
        {
            try
            {
                var model = _thirdPartyAPIResRepositoryArbitrage.GetById(id);
                if (model == null)
                {
                    return null;
                }
                var viewModel = new ThirdPartyAPIResponseConfigViewModel()
                {
                    BalanceRegex = model.BalanceRegex,
                    ErrorCodeRegex = model.ErrorCodeRegex,
                    Id = model.Id,
                    OprTrnRefNoRegex = model.OprTrnRefNoRegex,
                    Param1Regex = model.Param1Regex,
                    Param2Regex = model.Param2Regex,
                    Param3Regex = model.Param3Regex,
                    ResponseCodeRegex = model.ResponseCodeRegex,
                    StatusMsgRegex = model.StatusMsgRegex,
                    StatusRegex = model.StatusRegex,
                    TrnRefNoRegex = model.TrnRefNoRegex,
                    Status = model.Status
                };
                return viewModel;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public long AddThirdPartyAPIResponseArbitrage(ThirdPartyAPIResponseConfigViewModel Request, long UserId)
        {
            try
            {
                var model = new ArbitrageThirdPartyAPIResponseConfiguration()
                {
                    CreatedBy = UserId,
                    CreatedDate = _basePage.UTC_To_IST(),
                    Status = Request.Status,
                    BalanceRegex = Request.BalanceRegex,
                    ErrorCodeRegex = Request.ErrorCodeRegex,
                    OprTrnRefNoRegex = Request.OprTrnRefNoRegex,
                    Param1Regex = Request.Param1Regex,
                    Param2Regex = Request.Param2Regex,
                    Param3Regex = Request.Param3Regex,
                    ResponseCodeRegex = Request.ResponseCodeRegex,
                    StatusMsgRegex = Request.StatusMsgRegex,
                    StatusRegex = Request.StatusRegex,
                    TrnRefNoRegex = Request.TrnRefNoRegex
                };
                var newModel = _thirdPartyAPIResRepositoryArbitrage.Add(model);

                if (newModel != null)
                    return newModel.Id;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool UpdateThirdPartyAPIResponseArbitrage(ThirdPartyAPIResponseConfigViewModel Request, long UserId)
        {
            try
            {
                var model = _thirdPartyAPIResRepositoryArbitrage.GetById(Request.Id);
                if (model == null)
                {
                    return false;
                }
                model.BalanceRegex = Request.BalanceRegex;
                model.ErrorCodeRegex = Request.ErrorCodeRegex;
                model.OprTrnRefNoRegex = Request.OprTrnRefNoRegex;
                model.Param1Regex = Request.Param1Regex;
                model.Param2Regex = Request.Param2Regex;
                model.Param3Regex = Request.Param3Regex;
                model.ResponseCodeRegex = Request.ResponseCodeRegex;
                model.StatusMsgRegex = Request.StatusMsgRegex;
                model.StatusRegex = Request.StatusRegex;
                model.TrnRefNoRegex = Request.TrnRefNoRegex;
                model.UpdatedBy = UserId;
                model.UpdatedDate = _basePage.UTC_To_IST();
                model.Status = Request.Status;
                _thirdPartyAPIResRepositoryArbitrage.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public bool SetActiveThirdPartyAPIResponseArbitrage(long id)
        {
            try
            {
                ArbitrageThirdPartyAPIResponseConfiguration model = _thirdPartyAPIResRepositoryArbitrage.GetById(id);
                if (model != null)
                {
                    model.SetActive();
                    _thirdPartyAPIResRepositoryArbitrage.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }

        }

        public bool SetInActiveThirdPartyAPIResponseArbitrage(long id)
        {
            try
            {
                ArbitrageThirdPartyAPIResponseConfiguration model = _thirdPartyAPIResRepositoryArbitrage.GetById(id);
                if (model != null)
                {
                    model.SetInActive();
                    _thirdPartyAPIResRepositoryArbitrage.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }

        }

        public long AddDemonConfigurationArbitrage(DemonConfigurationRequest request, long UserId)
        {
            try
            {
                var model = new DemonConfigurationArbitrage
                {
                    IPAdd = request.IPAdd,
                    PortAdd = request.PortAdd,
                    Url = request.Url,
                    Status = request.Status,
                    CreatedDate = _basePage.UTC_To_IST(),
                    CreatedBy = UserId
                    //UpdatedDate = DateTime.UtcNow,
                    //UpdatedBy = null
                };

                var newModel = _DemonRepositoryArbitrage.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public bool UpdateDemonConfigurationArbitrage(DemonConfigurationRequest request, long UserId)
        {
            try
            {
                DemonConfigurationArbitrage model = _DemonRepositoryArbitrage.GetById(request.Id);
                if (model == null)
                {
                    return false;
                }
                model.IPAdd = request.IPAdd;
                model.PortAdd = request.PortAdd;
                model.Url = request.Url;
                model.Status = request.Status;
                model.UpdatedBy = UserId;
                model.UpdatedDate = _basePage.UTC_To_IST();
                _DemonRepositoryArbitrage.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public bool SetActiveDemonConfigArbitrage(long id)
        {
            try
            {
                DemonConfigurationArbitrage model = _DemonRepositoryArbitrage.GetById(id);
                if (model != null)
                {
                    model.EnableConfiguration();
                    _DemonRepositoryArbitrage.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetInActiveDemonConfigArbitrage(long id)
        {
            try
            {
                DemonConfigurationArbitrage model = _DemonRepositoryArbitrage.GetById(id);
                if (model != null)
                {
                    model.DisableConfiguration();
                    _DemonRepositoryArbitrage.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public ListDemonConfigResponse GetAllDemonConfigV1Arbitrage()
        {
            try
            {
                ListDemonConfigResponse _Res = new ListDemonConfigResponse();
                List<DemonconfigurationViewModel> modelList = new List<DemonconfigurationViewModel>();
                var list = _DemonRepositoryArbitrage.List();
                if (list.Count == 0)
                {
                    _Res.Response = null;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                foreach (var Model in list)
                {
                    modelList.Add(new DemonconfigurationViewModel
                    {
                        Id = Model.Id,
                        IPAdd = Model.IPAdd,
                        PortAdd = Model.PortAdd,
                        Status = Model.Status,
                        Url = Model.Url,
                        StatusText = Enum.GetName(typeof(ServiceStatus), Model.Status)
                    });
                }
                _Res.Response = modelList;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public ListDemonConfigResponseV1 ListDemonConfigV1Arbitrage()
        {
            try
            {
                ListDemonConfigResponseV1 _Res = new ListDemonConfigResponseV1();
                List<ListDEmonConfig> modelList = new List<ListDEmonConfig>();
                var list = _DemonRepositoryArbitrage.List();
                if (list.Count == 0)
                {
                    _Res.Response = null;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                foreach (var Model in list)
                {
                    modelList.Add(new ListDEmonConfig
                    {
                        Id = Model.Id,
                        Name = Model.IPAdd + ", " + Model.PortAdd + ", " + Model.Url
                    });
                }
                _Res.Response = modelList;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<MarketTickerPairData> GetMarketTickerPairDataArbitrage(short IsMargin = 0)
        {
            try
            {
                List<MarketTickerPairData> Data;
                //if (IsMargin == 1)
                //    Data = _backOfficeTrnRepository.GetMarketTickerPairDataMargin();
                //else
                Data = _backOfficeTrnRepository.GetMarketTickerPairDataArbitrageInfo();

                return Data;
            }
            catch (Exception ex)

            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int UpdateMarketTickerPairDataArbitrage(UpdateMarketTickerPairData Request, long UserId, short IsMargin = 0)
        {
            try
            {
                if (Request.Request.Count > 0)
                {
                    List<long> PairId = new List<long>();

                    foreach (var pair in Request.Request)
                    {
                        if (pair.IsMarketTicker == 1)
                            PairId.Add(pair.PairId);
                    }

                    //   if (IsMargin == 1)
                    //       _backOfficeTrnRepository.UpdateMarketTickerPairDataMargin(PairId, UserId);
                    //   else
                    _backOfficeTrnRepository.UpdateMarketTickerPairDataArbitrageInfo(PairId, UserId);


                    //Uday 25-12-2018 SingalR Call For Front Update
                    //This block try till catch not in use...    
                    //try
                    //{
                    //    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_configuration["FrontUrl"].ToString() + "api/Transaction/GetMarketTickerSignalR?IsMargin=" + IsMargin);
                    //    httpWebRequest.Method = "GET";
                    //    string responseText = string.Empty;
                    //    HttpWebResponse httpResponse;

                    //    httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    //    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    //    {
                    //        responseText = streamReader.ReadToEnd();
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + "  #MarketTicker SignalR Update#", this.GetType().Name, ex);
                    //}

                    //Call Fornt Side API For SignalR Update So Comment this code
                    //var Data = _backOfficeTrnRepository.GetUpdatedMarketTicker();
                    //if (Data.Count != 0)
                    //    _signalRService.MarketTicker(Data);

                }

                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<LimitViewModel> GetAllLimitDataArbitrage()
        {
            try
            {
                var list = _limitRepositoryArbitrage.List();
                List<LimitViewModel> limitList = new List<LimitViewModel>();
                foreach (var model in list)
                {
                    limitList.Add(new LimitViewModel()
                    {
                        Id = model.Id,
                        MaxAmt = model.MaxAmt,
                        MaxAmtDaily = model.MaxAmtDaily,
                        MaxAmtMonthly = model.MaxAmtMonthly,
                        MaxAmtWeekly = model.MaxAmtWeekly,
                        Maxrange = model.Maxrange,
                        MaxRangeDaily = model.MaxRangeDaily,
                        MaxRangeMonthly = model.MaxRangeMonthly,
                        MaxRangeWeekly = model.MaxRangeWeekly,
                        MinAmt = model.MinAmt,
                        MinAmtDaily = model.MinAmtDaily,
                        MinAmtMonthly = model.MinAmtMonthly,
                        MinAmtWeekly = model.MinAmtWeekly,
                        MinRange = model.MinRange,
                        MinRangeDaily = model.MinRangeDaily,
                        MinRangeMonthly = model.MinRangeMonthly,
                        MinRangeWeekly = model.MinRangeWeekly,
                        Status = model.Status

                    });
                }

                return limitList;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int AddLiquidityAPIManagerArbitrage(LiquidityAPIManagerRequest Request, long UserId)
        {
            try
            {
                //Get ThirdParty API Data
                var ThirdPartyObj = _thirdPartyAPIRepositoryArbitrage.GetById(Request.APIProviderId);
                var ServiceProviderMasterObj = _ServiceProviderMasterArbitrage.GetById(Request.ProviderMasterId);
                var ServiceProviderType = _ProviderTypeRepositoryArbitrage.GetById(Request.ProviderTypeId);

                if (ThirdPartyObj != null)
                {
                    for (int i = 0; i < Request.TransationType.Length; i++)
                    {
                        var Data = _proDetailArbitrageRepository.GetSingle(x => x.ServiceProID == Request.ProviderMasterId && x.ProTypeID == Request.ProviderTypeId && x.AppTypeID == ThirdPartyObj.AppType && x.TrnTypeID == Request.TransationType[i] && x.LimitID == Request.LimitId && x.DemonConfigID == Request.DeamonConfigId && x.ServiceProConfigID == Request.ServiceProviderCongigId && x.ThirPartyAPIID == Request.APIProviderId);

                        if (Data != null)
                        {
                            continue;
                        }

                        ProviderDetailRequest providerDetailRequest = new ProviderDetailRequest();

                        providerDetailRequest.SerProDetailName = ServiceProviderMasterObj.ProviderName + "_" + (enTrnType)Request.TransationType[i] + "_" + ServiceProviderType.ServiveProTypeName;
                        providerDetailRequest.ServiceProID = Request.ProviderMasterId;
                        providerDetailRequest.ProTypeID = Request.ProviderTypeId;
                        providerDetailRequest.AppTypeID = ThirdPartyObj.AppType;
                        providerDetailRequest.TrnTypeID = Request.TransationType[i];
                        providerDetailRequest.LimitID = Request.LimitId;
                        providerDetailRequest.DemonConfigID = Request.DeamonConfigId;
                        providerDetailRequest.ServiceProConfigID = Request.ServiceProviderCongigId;
                        providerDetailRequest.ThirPartyAPIID = Request.APIProviderId;
                        providerDetailRequest.Status = Request.Status;

                        AddProviderDetailArbitrage(providerDetailRequest, UserId);
                    }
                    return 1;
                }

                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;

            }

        }

        public int UpdateLiquidityAPIManagerArbitrage(LiquidityAPIManagerUpdateRequest Request, long UserId)
        {
            try
            {
                //Get ThirdParty API Data

                var ThirdPartyObj = _thirdPartyAPIRepositoryArbitrage.GetById(Request.APIProviderId);
                var ServiceProviderMasterObj = _ServiceProviderMasterArbitrage.GetById(Request.ProviderMasterId);
                var ServiceProviderType = _ProviderTypeRepositoryArbitrage.GetById(Request.ProviderTypeId);

                if (ThirdPartyObj != null)
                {
                    ProviderDetailRequest providerDetailRequest = new ProviderDetailRequest();
                    ServiceProviderMasterObj = new ServiceProviderMasterArbitrage();
                    providerDetailRequest.Id = Request.Id;
                    providerDetailRequest.SerProDetailName = ServiceProviderMasterObj.ProviderName + "_" + (enTrnType)Request.TransationType + "_" + ServiceProviderType.ServiveProTypeName;
                    providerDetailRequest.ServiceProID = Request.ProviderMasterId;
                    providerDetailRequest.ProTypeID = Request.ProviderTypeId;
                    providerDetailRequest.AppTypeID = ThirdPartyObj.AppType;
                    providerDetailRequest.TrnTypeID = Request.TransationType;
                    providerDetailRequest.LimitID = Request.LimitId;
                    providerDetailRequest.DemonConfigID = Request.DeamonConfigId;
                    providerDetailRequest.ServiceProConfigID = Request.ServiceProviderCongigId;
                    providerDetailRequest.ThirPartyAPIID = Request.APIProviderId;
                    providerDetailRequest.Status = Request.Status;

                    if (UpdateProviderDetailArbitrage(providerDetailRequest, UserId))
                    {
                        return 1;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        #endregion

        public async Task UpdateFrontCache(string MethodName)
        {
            try
            {
                var URL = _configuration["FrontAPIUrl"].ToString() + MethodName;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Method = "GET";
                string responseText = string.Empty;
                HttpWebResponse httpResponse;

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseText = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }
    }

}
