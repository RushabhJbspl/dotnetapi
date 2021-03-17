using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.CCXT;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.Arbitrage;
using Worldex.Infrastructure.Data.Transaction;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Worldex.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.DomainEvents
{
    public class CCXTTickerHandler : IRequestHandler<CCXTTickerHandlerRequest>
    {
        private readonly ICCXTCommonService _iCCXTCommonService;
        private readonly IMediator _mediator;
        private IMemoryCache _cache;
        private readonly ICommonRepository<CronMaster> _cronMaster;
        private readonly IWebApiRepository _WebApiRepository;

        public CCXTTickerHandler(ICCXTCommonService iCCXTCommonService, IMediator mediator, IMemoryCache cache,
            ICommonRepository<CronMaster> cronMaster, IWebApiRepository WebApiRepository)
        {
            _iCCXTCommonService = iCCXTCommonService;
            _mediator = mediator;
            _cache = cache;
            _cronMaster = cronMaster;
            _WebApiRepository = WebApiRepository;
        }

        public async Task<Unit> Handle(CCXTTickerHandlerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var cronMaster = _cronMaster.FindBy(e => e.Id == (short)enCronMaster.CCXTTicker).FirstOrDefault();
                if (cronMaster != null )
                {
                    if(cronMaster.Status != (short)ServiceStatus.Active)
                        return await Task.FromResult(new Unit());

                    var list = _cache.Get<List<TransactionProviderArbitrageResponse>>("RouteDataArbitrage");
                    if (list == null)
                    {
                        var GetProListResult = _WebApiRepository.GetProviderDataListArbitrageAsync(new TransactionApiConfigurationRequest { PairID = 0, trnType = 4, LPType = 0 });
                        list = _cache.Get<List<TransactionProviderArbitrageResponse>>("RouteDataArbitrage");
                    }
                    if (list!=null)
                    {
                        //List<CCXTTickerExchange> ExchangeName = GetExchage();
                        list = list.Where(e => e.TrnType == 4 && e.TrnTypeID == 4 && e.ProTypeID==7).ToList();
                        foreach (var ExchangeObj in list)
                        {
                            var res=_mediator.Send(new CCXTTickerExchange()
                            {
                                ExchangeName = ExchangeObj.ProviderName,
                                LpType = ExchangeObj.LPType,
                                PairID = ExchangeObj.PairId,
                                Pair =ExchangeObj.PairName,
                                RouteID=ExchangeObj.RouteID,
                                SerProDetailID=ExchangeObj.SerProDetailID,
                                ThirdPartyAPIID=ExchangeObj.ThirdPartyAPIID
                            });
                            Thread.Sleep(500);
                        }
                    }
                    return await Task.FromResult(new Unit());
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CCXTTickerHandler", "CCXTTickerHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
        public List<CCXTTickerExchange> GetExchage()
        {
            try
            {
                return _iCCXTCommonService.GetCCXTExchange();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CCXTTickerHandler", "GetExchage ", ex);
                return null;
            }

        }
    }

    public class CCXTTickerCall : IRequestHandler<CCXTTickerExchange>
    {
        private static string ExchangeURL = "";
        private readonly IConfiguration _configuration;
        private readonly ICCXTCommonService _iCCXTCommonService;
        private readonly ISignalRService _iSignalRService;
        private readonly IFrontTrnService _frontTrnService;
        private readonly IGetWebRequest _IGetWebRequest;
        private readonly WebApiDataRepository _webapiDataRepository;
        private readonly IWebApiSendRequest _IWebApiSendRequest;
        private ThirdPartyAPIRequestArbitrage ThirdPartyAPIRequestOnj;
        private GetDataForParsingAPI txnWebAPIParsingData;
        WebApiParseResponse _WebApiParseResponseObj;

        public CCXTTickerCall(IConfiguration configuration, ICCXTCommonService iCCXTCommonService, 
            ISignalRService iSignalRService, IFrontTrnService frontTrnService, IGetWebRequest IGetWebRequest,
            WebApiDataRepository webapiDataRepository, WebApiParseResponse WebApiParseResponseObj,
            IWebApiSendRequest IWebApiSendRequest)
        {
            _configuration = configuration;
            _iCCXTCommonService = iCCXTCommonService;
            _iSignalRService = iSignalRService;
            _frontTrnService = frontTrnService;
            _IGetWebRequest = IGetWebRequest;
            _webapiDataRepository = webapiDataRepository;
            _WebApiParseResponseObj = WebApiParseResponseObj;
            _IWebApiSendRequest = IWebApiSendRequest;
            //ExchangeURL = _configuration["CCXTExchangeURL"];
            //ThirdPartyAPIRequestOnj = _IGetWebRequest.ArbitrageMakeWebRequest(0, 3, 3000112, IsValidateUrl: 3);
            
        }
        public async Task<Unit> Handle(CCXTTickerExchange ExchangeObj, CancellationToken cancellationToken)
        {
            ArbitrageBuySellViewModel BuySellmodel;
            ExchangeProviderListArbitrage exchangeProvider;
            LastPriceViewModelArbitrage lastPriceObj;
            WebAPIParseResponseCls _webapiParseResponse;
            //CCXTTickerResponse Response = new CCXTTickerResponse();
            string APIResponse = "";
            short IsAPIProceed = 0;
            try
            {
                txnWebAPIParsingData = _webapiDataRepository.ArbitrageGetDataForParsingAPI(ExchangeObj.ThirdPartyAPIID);
                var ThirdPartyAPIRequestOnj = _IGetWebRequest.ArbitrageMakeWebRequest(ExchangeObj.RouteID, ExchangeObj.ThirdPartyAPIID, ExchangeObj.SerProDetailID, IsValidateUrl: 3);
                APIResponse =await  _IWebApiSendRequest.SendRequestAsyncLPArbitrage(ThirdPartyAPIRequestOnj.RequestURL, ref IsAPIProceed, ThirdPartyAPIRequestOnj.RequestBody, ThirdPartyAPIRequestOnj.MethodType, ThirdPartyAPIRequestOnj.ContentType, ThirdPartyAPIRequestOnj.keyValuePairsHeader, 30000, IsWrite: false);
                
                if(APIResponse== "The operation has timed out.")
                    return await Task.FromResult(new Unit());
                _webapiParseResponse = _WebApiParseResponseObj.ParseResponseViaRegex(APIResponse, txnWebAPIParsingData, 3);

                HelperForLog.WriteLogForActivity(" CCXTTickerCall ", " Exchange Name : " + ExchangeObj.ExchangeName + " Pair : " + ExchangeObj.Pair, " " + APIResponse);
                if (_webapiParseResponse.Param7 == "2")
                    return await  Task.FromResult(new Unit());

                CCXTTickerResObj cCXTTicker = new CCXTTickerResObj();
                cCXTTicker.ChangePer = _webapiParseResponse.Param4 == "" ? 0 : Convert.ToDecimal(_webapiParseResponse.Param4);
                cCXTTicker.LPType = ExchangeObj.LpType;
                cCXTTicker.LTP =_webapiParseResponse.Param5 == "" ? 0 : Convert.ToDecimal(_webapiParseResponse.Param5);
                cCXTTicker.Pair = ExchangeObj.Pair;
                cCXTTicker.PairId = ExchangeObj.PairID;
                cCXTTicker.Volume = _webapiParseResponse.Param6==""? 0: Convert.ToDecimal(_webapiParseResponse.Param6);
                cCXTTicker.UpDownBit = 1;
                var TickerInfo = _iCCXTCommonService.InsertUpdateTickerData(cCXTTicker);

                lastPriceObj = new LastPriceViewModelArbitrage();
                lastPriceObj.LastPrice = TickerInfo.LTP;
                lastPriceObj.UpDownBit = TickerInfo.UpDownBit;
                lastPriceObj.LPType = TickerInfo.LPType;
                lastPriceObj.ExchangeName = ExchangeObj.ExchangeName;
                _iSignalRService.LastPriceArbitrage(lastPriceObj, TickerInfo.Pair, "0");

                BuySellmodel = new ArbitrageBuySellViewModel();
                BuySellmodel.LPType = TickerInfo.LPType;
                BuySellmodel.LTP = TickerInfo.LTP;
                BuySellmodel.ProviderName = ExchangeObj.ExchangeName;
                BuySellmodel.Fees = TickerInfo.Fees;
                _iSignalRService.BuyerBookArbitrage(BuySellmodel, TickerInfo.Pair, "0");
                _iSignalRService.SellerBookArbitrage(BuySellmodel, TickerInfo.Pair, "0");

                exchangeProvider = new ExchangeProviderListArbitrage();
                exchangeProvider.LPType = TickerInfo.LPType;
                exchangeProvider.LTP = TickerInfo.LTP;
                exchangeProvider.ProviderName = ExchangeObj.ExchangeName;
                exchangeProvider.UpDownBit = TickerInfo.UpDownBit;
                exchangeProvider.Volume = TickerInfo.Volume;
                exchangeProvider.ChangePer = TickerInfo.ChangePer;
                _iSignalRService.ProviderMarketDataArbitrage(exchangeProvider, TickerInfo.Pair);

                //Rita 17-6-19 send Profit Indicator data and also smart arbitrage data                           
                ProfitIndicatorInfo responsedata = _frontTrnService.GetProfitIndicatorArbitrage(TickerInfo.PairId, 0);
                if (responsedata != null)
                    _iSignalRService.ProfitIndicatorArbitrage(responsedata, TickerInfo.Pair);

                List<ExchangeListSmartArbitrage> responsedata1 = _frontTrnService.ExchangeListSmartArbitrageService(TickerInfo.PairId, TickerInfo.Pair, 5, 0);
                if (responsedata1 != null)
                    _iSignalRService.ExchangeListSmartArbitrage(responsedata1, TickerInfo.Pair);

                return await  Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CCXTTickerCall", "CCXTTickerCall handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
}
