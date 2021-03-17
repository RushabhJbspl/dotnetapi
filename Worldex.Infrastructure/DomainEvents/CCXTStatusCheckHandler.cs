using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.CCXT;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.DomainEvents
{
    public class CCXTStatusCheckHandler : IRequestHandler<CCXTStatusCheckHandlerRequest>
    {
        private readonly IMediator _mediator;
        private IMemoryCache _cache;
        private readonly ICommonRepository<CronMaster> _cronMaster;
        private readonly IWebApiRepository _WebApiRepository;
        public static short IsRunCron=0;

        public CCXTStatusCheckHandler(ICommonRepository<CronMaster> cronMaster, IMediator mediator, IMemoryCache cache, IWebApiRepository WebApiRepository)
        {
            _cronMaster = cronMaster;
            _mediator = mediator;
            _cache = cache;
            _WebApiRepository = WebApiRepository;
        }

        public async Task<Unit> Handle(CCXTStatusCheckHandlerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var cronMaster = _cronMaster.FindBy(e => e.Id == (short)enCronMaster.CCXTStatusCheck).FirstOrDefault();
                if (cronMaster != null )
                {
                    if (cronMaster.Status != (short)ServiceStatus.Active)
                        return await Task.FromResult(new Unit());

                    if (IsRunCron == 1)
                        return await Task.FromResult(new Unit());
                    IsRunCron = 1;

                    var list = _cache.Get<List<TransactionProviderArbitrageResponse>>("RouteDataArbitrage");
                    if (list == null)
                    {
                        var GetProListResult = _WebApiRepository.GetProviderDataListArbitrageAsync(new TransactionApiConfigurationRequest { PairID = 0, trnType = 4, LPType = 0 });
                        list = _cache.Get<List<TransactionProviderArbitrageResponse>>("RouteDataArbitrage");
                    }
                    if (list != null)
                    {
                        IsRunCron = 1;
                        list = list.Where(e => e.TrnType == 4 && e.TrnTypeID == 4 && e.ProTypeID == 7).ToList();
                        var GroupByLpTypeList = list.GroupBy(e => e.LPType).ToList();
                        foreach (var ExchangeObjData in GroupByLpTypeList)//Group DATA
                        {
                            //HelperForLog.WriteLogIntoFile(" CCXTStatusCheckHandler ", ""," start: " +DateTime.UtcNow+ " LPType :" + ExchangeObjData.Key);
                            var res =await _mediator.Send(new CCXTStatusCheckCallLpReq()
                            {
                                LpType = ExchangeObjData.Key,
                                transactionProviderArbitrageResponses = ExchangeObjData.ToList()
                            });
                            //HelperForLog.WriteLogIntoFile(" CCXTStatusCheckHandler ","", " End: " + DateTime.UtcNow +" LPType :" + ExchangeObjData.Key);
                            //Thread.Sleep(500);
                        }
                       IsRunCron = 0;
                    }
                    else
                        IsRunCron = 0;
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                IsRunCron = 0;
                HelperForLog.WriteErrorLog("CCXTStatusCheckHandler", "CCXTStatusCheckHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
    //public class CCXTStatusCheckCall : IRequestHandler<CCXTStatusCheckCallLpReq>
    //{
    //    private IMemoryCache _cache;
    //    private readonly IFrontTrnRepository _frontTrnRepository;
    //    private readonly ITransactionProcessArbitrageV1 _transactionProcessArbitrageV1;
    //    private readonly IGetWebRequest _IGetWebRequest;
    //    private readonly IWebApiSendRequest _IWebApiSendRequest;
    //    private readonly IWebApiRepository _WebApiRepository;
    //    private readonly ICommonRepository<TradeTransactionQueueArbitrage> _tradeTrnRepositiory;
    //    private readonly ISettlementRepositoryArbitrageAPI<BizResponse> _SettlementRepositoryAPI;
    //    private readonly ICommonRepository<TransactionQueueArbitrage> _trnRepositiory;
    //    private readonly ITransactionQueue<NewCancelOrderArbitrageRequestCls> _TransactionQueueCancelOrderArbitrage;
    //    private readonly ICommonRepository<TransactionStatusCheckRequestArbitrage> _transactionStatusCheckRequest;

    //    public CCXTStatusCheckCall(IMemoryCache cache, ITransactionProcessArbitrageV1 transactionProcessArbitrageV1, 
    //        IGetWebRequest IGetWebRequest, IWebApiSendRequest IWebApiSendRequest, IWebApiRepository WebApiRepository,
    //        ICommonRepository<TradeTransactionQueueArbitrage> tradeTrnRepositiory,
    //        ISettlementRepositoryArbitrageAPI<BizResponse> SettlementRepositoryAPI,
    //        ICommonRepository<TransactionQueueArbitrage> trnRepositiory, IFrontTrnRepository frontTrnRepository,
    //        ITransactionQueue<NewCancelOrderArbitrageRequestCls> TransactionQueueCancelOrderArbitrage,
    //        ICommonRepository<TransactionStatusCheckRequestArbitrage> transactionStatusCheckRequest)
    //    {
    //        _cache = cache;
    //        _transactionProcessArbitrageV1 = transactionProcessArbitrageV1;
    //        _IGetWebRequest = IGetWebRequest;
    //        _IWebApiSendRequest = IWebApiSendRequest;
    //        _WebApiRepository = WebApiRepository;
    //        _tradeTrnRepositiory = tradeTrnRepositiory;
    //        _SettlementRepositoryAPI = SettlementRepositoryAPI;
    //        _trnRepositiory = trnRepositiory;
    //        _TransactionQueueCancelOrderArbitrage = TransactionQueueCancelOrderArbitrage;
    //        _frontTrnRepository = frontTrnRepository;
    //        _transactionStatusCheckRequest = transactionStatusCheckRequest;
    //    }

    //    public async Task<Unit> Handle(CCXTStatusCheckCallLpReq request, CancellationToken cancellationToken)
    //    {
    //        TransactionProviderArbitrageResponse Provider;
    //        string cacheToken = "";
    //        short IsAPIProceed=0;
    //        string APIResponse = "";
    //        List<CCXTOrdersResponseObj> APIOrderList;
    //        CCXTOrdersResponseObj APIOrder;
    //        List<CCXTMyTradeInfoResponse> MyTradeInfo;
    //        ThirdPartyAPIRequestArbitrage ThirdPartyAPIRequestOnj;
    //        LPProcessTransactionCls LPProcessTransactionClsObj;
    //        BizResponse _Resp;
    //        try
    //        {
    //            Thread.Sleep(1000);
    //            var list = request.transactionProviderArbitrageResponses.ToList();
    //            foreach (var TxnPairWiseObj in list)
    //            {
    //                var ListOfHoldTrn = _WebApiRepository.CCXTLpHoldTransaction((int)request.LpType, TxnPairWiseObj.PairId);
    //                if (ListOfHoldTrn.Count == 0)
    //                    continue;
    //                    //return await Task.FromResult(new Unit());

    //                cacheToken = _cache.Get<string>("LPType" + request.LpType);
    //                if (cacheToken == null)
    //                {
    //                    Provider = new TransactionProviderArbitrageResponse();
    //                    var RequestObj = request.transactionProviderArbitrageResponses.FirstOrDefault();
    //                    Provider.SerProDetailID = RequestObj.SerProDetailID;
    //                    Provider.RouteID = RequestObj.RouteID;
    //                    Provider.LPType = request.LpType;
    //                    Provider.ThirdPartyAPIID = RequestObj.ThirdPartyAPIID;
    //                    cacheToken = await _transactionProcessArbitrageV1.ConnectToExchangeAsync(Provider, new TransactionQueueArbitrage(), 0);
    //                    _cache.Set<string>("LPType" + Provider.LPType, cacheToken);
    //                }
    //                if (string.IsNullOrEmpty(cacheToken))
    //                    continue;
    //                //return await Task.FromResult(new Unit());

    //                short IsBulkOrderCheck = Convert.ToInt16(ListOfHoldTrn.FirstOrDefault().IsBulkOrder);
    //                if (IsBulkOrderCheck == 1)//Fetch orders(Bulk order)
    //                {
    //                    _Resp = new BizResponse();
    //                    LPProcessTransactionClsObj = new LPProcessTransactionCls();
    //                    APIOrderList = new List<CCXTOrdersResponseObj>();
    //                    ThirdPartyAPIRequestOnj = new ThirdPartyAPIRequestArbitrage();
    //                    ThirdPartyAPIRequestOnj = _IGetWebRequest.ArbitrageMakeWebRequest(TxnPairWiseObj.RouteID, TxnPairWiseObj.ThirdPartyAPIID, TxnPairWiseObj.SerProDetailID, IsValidateUrl: 4, Token: cacheToken);
    //                    APIResponse = await _IWebApiSendRequest.SendRequestAsyncLPArbitrage(ThirdPartyAPIRequestOnj.RequestURL, ref IsAPIProceed, ThirdPartyAPIRequestOnj.RequestBody, ThirdPartyAPIRequestOnj.MethodType, ThirdPartyAPIRequestOnj.ContentType, ThirdPartyAPIRequestOnj.keyValuePairsHeader, 30000,IsWrite:false);
    //                    try
    //                    {
    //                        APIOrderList = JsonConvert.DeserializeObject<List<CCXTOrdersResponseObj>>(APIResponse);
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        HelperForLog.WriteErrorLog("CCXTStatusCheckCall", " case 1 : Object Pracing APIResponse : " + APIResponse, ex);
    //                        //return await Task.FromResult(new Unit());
    //                        continue;
    //                    }
    //                    if (APIOrderList.Count > 0)
    //                    {
    //                        foreach (var TrnObj in ListOfHoldTrn)
    //                        {
    //                            var ExistToAPI = APIOrderList.Where(e => e.id == TrnObj.TrnRefNo).FirstOrDefault();
    //                            if (ExistToAPI != null)
    //                            {                                    
    //                                switch (ExistToAPI.status)
    //                                {
    //                                    case "canceled":
    //                                    case "canceling":
    //                                        if (ExistToAPI.remaining.Contains("E") || ExistToAPI.remaining.Contains("e"))
    //                                        {
    //                                            ExistToAPI.remaining = decimal.Parse(ExistToAPI.remaining, System.Globalization.NumberStyles.Float).ToString();
    //                                        }
    //                                        if (ExistToAPI.filled.Contains("E") || ExistToAPI.filled.Contains("e"))
    //                                        {
    //                                            ExistToAPI.filled = decimal.Parse(ExistToAPI.filled, System.Globalization.NumberStyles.Float).ToString();
    //                                        }
    //                                        LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(ExistToAPI.remaining);
    //                                        LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(ExistToAPI.filled);
    //                                        LPProcessTransactionClsObj.TotalQty = (decimal)ExistToAPI.amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                        _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                        HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " Handle ", " CCXTStatusCheckCall API_LP_Cancel  :" + "##TrnNo:" + TrnObj.TrnNo + " Response : " + JsonConvert.SerializeObject(ExistToAPI));
    //                                        break;
    //                                    case "closed":
    //                                        if (ExistToAPI.remaining.Contains("E") || ExistToAPI.remaining.Contains("e"))
    //                                        {
    //                                            ExistToAPI.remaining = decimal.Parse(ExistToAPI.remaining, System.Globalization.NumberStyles.Float).ToString();
    //                                        }
    //                                        if (ExistToAPI.filled.Contains("E") || ExistToAPI.filled.Contains("e"))
    //                                        {
    //                                            ExistToAPI.filled = decimal.Parse(ExistToAPI.filled, System.Globalization.NumberStyles.Float).ToString();
    //                                        }
    //                                        LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(ExistToAPI.remaining);
    //                                        LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(ExistToAPI.filled);
    //                                        LPProcessTransactionClsObj.TotalQty = (decimal)ExistToAPI.amount;

    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " Handle ", " CCXTStatusCheckCall API_LP_Filled  :" + "##TrnNo:" + TrnObj.TrnNo + " Response : " + JsonConvert.SerializeObject(ExistToAPI));
    //                                        break;
    //                                    case "open":
    //                                        if (ExistToAPI.remaining.Contains("E") || ExistToAPI.remaining.Contains("e"))
    //                                        {
    //                                            ExistToAPI.remaining = decimal.Parse(ExistToAPI.remaining, System.Globalization.NumberStyles.Float).ToString();
    //                                        }
    //                                        if (ExistToAPI.filled.Contains("E") || ExistToAPI.filled.Contains("e"))
    //                                        {
    //                                            ExistToAPI.filled = decimal.Parse(ExistToAPI.filled, System.Globalization.NumberStyles.Float).ToString();
    //                                        }
    //                                        LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(ExistToAPI.remaining);
    //                                        LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(ExistToAPI.filled);
    //                                        LPProcessTransactionClsObj.TotalQty = (decimal)ExistToAPI.amount;

    //                                        if (TrnObj.SettledQty != Convert.ToDecimal(ExistToAPI.filled))
    //                                        {
    //                                            _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
    //                                            HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " Handle ", " CCXTStatusCheckCall API_LP_PartialFilled  :" + "##TrnNo:" + TrnObj.TrnNo + " Response : " + JsonConvert.SerializeObject(ExistToAPI));
    //                                        }  
    //                                        else
    //                                            _Resp.ErrorCode = enErrorCode.API_LP_Hold;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        break;
    //                                    default:
    //                                        break;
    //                                }
    //                                await UpdateTransactionStatus(TrnObj, _Resp, LPProcessTransactionClsObj, JsonConvert.SerializeObject(ExistToAPI));
    //                            }
    //                        }
    //                    }
    //                }
    //                else if(IsBulkOrderCheck == 0) //single Ordre
    //                {
    //                    foreach (var TrnObj in ListOfHoldTrn)
    //                    {
    //                        _Resp = new BizResponse();
    //                        LPProcessTransactionClsObj = new LPProcessTransactionCls();
    //                        APIOrder = new CCXTOrdersResponseObj();
    //                        ThirdPartyAPIRequestOnj = new ThirdPartyAPIRequestArbitrage();
    //                        ThirdPartyAPIRequestOnj = _IGetWebRequest.ArbitrageMakeWebRequest(TxnPairWiseObj.RouteID, TxnPairWiseObj.ThirdPartyAPIID, TxnPairWiseObj.SerProDetailID, IsValidateUrl: 6, Token: cacheToken, TrnNo: TrnObj.TrnRefNo);
    //                        APIResponse = await _IWebApiSendRequest.SendRequestAsyncLPArbitrage(ThirdPartyAPIRequestOnj.RequestURL, ref IsAPIProceed, ThirdPartyAPIRequestOnj.RequestBody, ThirdPartyAPIRequestOnj.MethodType, ThirdPartyAPIRequestOnj.ContentType, ThirdPartyAPIRequestOnj.keyValuePairsHeader, 30000, IsWrite: false);
    //                        //APIResponse = "{\"id\":\"554129918\",\"timestamp\":1564581674143,\"datetime\":\"2019-07-31T14:01:14.143Z\",\"symbol\":\"BTC/USDT\",\"type\":\"limit\",\"side\":\"buy\",\"price\":9980.22,\"amount\":0.0011,\"cost\":10.978242,\"filled\":0.0011,\"remaining\":0,\"status\":\"canceled\",\"info\":{\"symbol\":\"BTCUSDT\",\"orderId\":554129918,\"clientOrderId\":\"iaDtziDk26S1k3KhLjDJB5\",\"price\":\"9980.22000000\",\"origQty\":\"0.00110000\",\"executedQty\":\"0.00110000\",\"cummulativeQuoteQty\":\"10.97824200\",\"status\":\"FILLED\",\"timeInForce\":\"GTC\",\"type\":\"LIMIT\",\"side\":\"BUY\",\"stopPrice\":\"0.00000000\",\"icebergQty\":\"0.00000000\",\"time\":1564581674143,\"updateTime\":1564581684553,\"isWorking\":true}}";
    //                        try
    //                        {
    //                                APIOrder = JsonConvert.DeserializeObject<CCXTOrdersResponseObj>(APIResponse);
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            HelperForLog.WriteErrorLog("CCXTStatusCheckCall", " case 0 : Object Pracing APIResponse : " + APIResponse, ex);
    //                            //return await Task.FromResult(new Unit());
    //                            continue;
    //                        }
    //                        switch (APIOrder.status)
    //                        {
    //                            case "canceled":
    //                            case "canceling":
    //                                if (APIOrder.remaining.Contains("E") || APIOrder.remaining.Contains("e"))
    //                                {
    //                                    APIOrder.remaining = decimal.Parse(APIOrder.remaining, System.Globalization.NumberStyles.Float).ToString();
    //                                }
    //                                if (APIOrder.filled.Contains("E") || APIOrder.filled.Contains("e"))
    //                                {
    //                                    APIOrder.filled = decimal.Parse(APIOrder.filled, System.Globalization.NumberStyles.Float).ToString();
    //                                }
    //                                LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(APIOrder.remaining);
    //                                LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(APIOrder.filled);
    //                                LPProcessTransactionClsObj.TotalQty = (decimal)APIOrder.amount;
    //                                _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " Handle ", " CCXTStatusCheckCall API_LP_Cancel  :" + "##TrnNo:" + TrnObj.TrnNo + " Response : " + JsonConvert.SerializeObject(APIOrder));
    //                                break;
    //                            case "closed":
    //                                if (APIOrder.remaining.Contains("E") || APIOrder.remaining.Contains("e"))
    //                                {
    //                                    APIOrder.remaining = decimal.Parse(APIOrder.remaining, System.Globalization.NumberStyles.Float).ToString();
    //                                }
    //                                if (APIOrder.filled.Contains("E") || APIOrder.filled.Contains("e"))
    //                                {
    //                                    APIOrder.filled = decimal.Parse(APIOrder.filled, System.Globalization.NumberStyles.Float).ToString();
    //                                }
    //                                LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(APIOrder.remaining);
    //                                LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(APIOrder.filled);
    //                                LPProcessTransactionClsObj.TotalQty = (decimal)APIOrder.amount;

    //                                _Resp.ErrorCode = enErrorCode.API_LP_Filled;
    //                                _Resp.ReturnCode = enResponseCodeService.Success;
    //                                HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " Handle ", " CCXTStatusCheckCall API_LP_Filled  :" + "##TrnNo:" + TrnObj.TrnNo + " Response : " + JsonConvert.SerializeObject(APIOrder));
    //                                break;
    //                            case "open":
    //                                if (APIOrder.remaining.Contains("E") || APIOrder.remaining.Contains("e"))
    //                                {
    //                                    APIOrder.remaining = decimal.Parse(APIOrder.remaining, System.Globalization.NumberStyles.Float).ToString();
    //                                }
    //                                if (APIOrder.filled.Contains("E") || APIOrder.filled.Contains("e"))
    //                                {
    //                                    APIOrder.filled = decimal.Parse(APIOrder.filled, System.Globalization.NumberStyles.Float).ToString();
    //                                }
    //                                LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(APIOrder.remaining);
    //                                LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(APIOrder.filled);
    //                                LPProcessTransactionClsObj.TotalQty = (decimal)APIOrder.amount;

    //                                if (TrnObj.SettledQty != Convert.ToDecimal(APIOrder.filled))
    //                                {
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
    //                                    HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " Handle ", " CCXTStatusCheckCall API_LP_PartialFilled  :" + "##TrnNo:" + TrnObj.TrnNo + " Response : " + JsonConvert.SerializeObject(APIOrder));
    //                                }
    //                                else
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_Hold;

    //                                _Resp.ReturnCode = enResponseCodeService.Success;
    //                                break;
    //                            default:
    //                                break;
    //                        }
    //                        await UpdateTransactionStatus(TrnObj, _Resp, LPProcessTransactionClsObj, JsonConvert.SerializeObject(APIOrder));
    //                    }
    //                }
    //                else if(IsBulkOrderCheck == 2) ///trades/mine
    //                {
    //                    _Resp = new BizResponse();
    //                    LPProcessTransactionClsObj = new LPProcessTransactionCls();
    //                    MyTradeInfo = new List<CCXTMyTradeInfoResponse>();
    //                    ThirdPartyAPIRequestOnj = new ThirdPartyAPIRequestArbitrage();
    //                    ThirdPartyAPIRequestOnj = _IGetWebRequest.ArbitrageMakeWebRequest(TxnPairWiseObj.RouteID, TxnPairWiseObj.ThirdPartyAPIID, TxnPairWiseObj.SerProDetailID, IsValidateUrl: 4, Token: cacheToken);
    //                    ThirdPartyAPIRequestOnj.RequestURL = ThirdPartyAPIRequestOnj.RequestURL.Replace("/orders", "/trades/mine");
    //                    APIResponse = await _IWebApiSendRequest.SendRequestAsyncLPArbitrage(ThirdPartyAPIRequestOnj.RequestURL, ref IsAPIProceed, ThirdPartyAPIRequestOnj.RequestBody, ThirdPartyAPIRequestOnj.MethodType, ThirdPartyAPIRequestOnj.ContentType, ThirdPartyAPIRequestOnj.keyValuePairsHeader, 30000, IsWrite: false);
    //                    try
    //                    {
    //                        MyTradeInfo = JsonConvert.DeserializeObject<List<CCXTMyTradeInfoResponse>>(APIResponse);
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        HelperForLog.WriteErrorLog("CCXTStatusCheckCall", " case 2 : Object Pracing APIResponse : " + APIResponse, ex);
    //                        //return await Task.FromResult(new Unit());
    //                        continue;
    //                    }
    //                    if(MyTradeInfo.Count > 0)
    //                    {
    //                        foreach (var TrnObj in ListOfHoldTrn)
    //                        {
    //                            var ExistToAPI = MyTradeInfo.Where(e => e.info.id == TrnObj.TrnRefNo).FirstOrDefault();
    //                            if (ExistToAPI != null)
    //                            {
    //                                switch (ExistToAPI.info.status)
    //                                {
    //                                    case "canceled":
    //                                    case "canceling":
    //                                        if (ExistToAPI.info.remaining.Contains("E") || ExistToAPI.info.remaining.Contains("e"))
    //                                        {
    //                                            ExistToAPI.info.remaining = decimal.Parse(ExistToAPI.info.remaining, System.Globalization.NumberStyles.Float).ToString();
    //                                        }
    //                                        if (ExistToAPI.info.filled.Contains("E") || ExistToAPI.info.filled.Contains("e"))
    //                                        {
    //                                            ExistToAPI.info.filled = decimal.Parse(ExistToAPI.info.filled, System.Globalization.NumberStyles.Float).ToString();
    //                                        }
    //                                        LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(ExistToAPI.info.remaining);
    //                                        LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(ExistToAPI.info.filled);
    //                                        LPProcessTransactionClsObj.TotalQty = Convert.ToDecimal(ExistToAPI.amount);
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                        _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                        HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " Handle ", " CCXTStatusCheckCall API_LP_Cancel  :" + "##TrnNo:" + TrnObj.TrnNo + " Response : " + JsonConvert.SerializeObject(ExistToAPI));
    //                                        break;
    //                                    case "closed":
    //                                        if (ExistToAPI.info.remaining.Contains("E") || ExistToAPI.info.remaining.Contains("e"))
    //                                        {
    //                                            ExistToAPI.info.remaining = decimal.Parse(ExistToAPI.info.remaining, System.Globalization.NumberStyles.Float).ToString();
    //                                        }
    //                                        if (ExistToAPI.info.filled.Contains("E") || ExistToAPI.info.filled.Contains("e"))
    //                                        {
    //                                            ExistToAPI.info.filled = decimal.Parse(ExistToAPI.info.filled, System.Globalization.NumberStyles.Float).ToString();
    //                                        }
    //                                        LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(ExistToAPI.info.remaining);
    //                                        LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(ExistToAPI.info.filled);
    //                                        LPProcessTransactionClsObj.TotalQty = Convert.ToDecimal(ExistToAPI.amount);

    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " Handle ", " CCXTStatusCheckCall API_LP_Filled  :" + "##TrnNo:" + TrnObj.TrnNo + " Response : " + JsonConvert.SerializeObject(ExistToAPI));
    //                                        break;
    //                                    case "open":
    //                                        if (ExistToAPI.info.remaining.Contains("E") || ExistToAPI.info.remaining.Contains("e"))
    //                                        {
    //                                            ExistToAPI.info.remaining = decimal.Parse(ExistToAPI.info.remaining, System.Globalization.NumberStyles.Float).ToString();
    //                                        }
    //                                        if (ExistToAPI.info.filled.Contains("E") || ExistToAPI.info.filled.Contains("e"))
    //                                        {
    //                                            ExistToAPI.info.filled = decimal.Parse(ExistToAPI.info.filled, System.Globalization.NumberStyles.Float).ToString();
    //                                        }
    //                                        LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(ExistToAPI.info.remaining);
    //                                        LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(ExistToAPI.info.filled);
    //                                        LPProcessTransactionClsObj.TotalQty = Convert.ToDecimal(ExistToAPI.amount);

    //                                        if (TrnObj.SettledQty != Convert.ToDecimal(ExistToAPI.info.filled))
    //                                        {
    //                                            _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
    //                                            HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " Handle ", " CCXTStatusCheckCall API_LP_PartialFilled  :" + "##TrnNo:" + TrnObj.TrnNo + " Response : " + JsonConvert.SerializeObject(ExistToAPI));
    //                                        }
    //                                        else
    //                                            _Resp.ErrorCode = enErrorCode.API_LP_Hold;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        break;
    //                                    default:
    //                                        break;
    //                                }
    //                                await UpdateTransactionStatus(TrnObj, _Resp, LPProcessTransactionClsObj, JsonConvert.SerializeObject(ExistToAPI));
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //            return await Task.FromResult(new Unit());
    //        }
    //        catch (Exception ex)
    //        {
    //            HelperForLog.WriteErrorLog("CCXTStatusCheckCall", "CCXTStatusCheckCall handle", ex);
    //            return await Task.FromResult(new Unit());
    //        }
    //    }

    //    public async Task UpdateTransactionStatus(CCXTTranNo TrnObj, BizResponse _Resp, LPProcessTransactionCls LPProcessTransactionClsObj,string APIResponse)
    //    {
    //        TransactionStatusCheckRequestArbitrage NewtransactionReq;
    //        try
    //        { 
    //            var updateddata = _trnRepositiory.GetById(TrnObj.TrnNo);
    //            var NewTradetransaction = _tradeTrnRepositiory.FindBy(e => e.TrnNo == TrnObj.TrnNo).FirstOrDefault();
    //            NewtransactionReq = _transactionStatusCheckRequest.FindBy(e => e.TrnNo == TrnObj.TrnNo).FirstOrDefault();
    //            if (NewtransactionReq == null)
    //            {
    //                NewtransactionReq = new TransactionStatusCheckRequestArbitrage()
    //                {
    //                    TrnNo = TrnObj.TrnNo,
    //                    SerProDetailID = TrnObj.SerProDetailID,
    //                    CreatedDate = Helpers.UTC_To_IST(),
    //                    CreatedBy = 1,
    //                    OprTrnID = TrnObj.TrnRefNo,
    //                    TrnID = TrnObj.TrnRefNo,
    //                    Status = 0
    //                };
    //                NewtransactionReq = _transactionStatusCheckRequest.Add(NewtransactionReq);
    //            }
    //            NewtransactionReq.UpdatedDate = Helpers.UTC_To_IST();
    //            if (_Resp.ReturnCode == enResponseCodeService.Success && _Resp.ErrorCode == enErrorCode.API_LP_Filled)
    //            {
    //                NewTradetransaction.APIStatus = "Fully Succcess";
    //                BizResponse SettlementResp = await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPI(_Resp, TrnObj.TrnNo, 0, TrnObj.Amount, TrnObj.Price);
    //                HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " UpdateTransactionStatus ", " CCXTStatusCheckCall Success  :" + "##TrnNo:" + TrnObj.TrnNo + " Response : " + JsonConvert.SerializeObject(SettlementResp));
    //                if (SettlementResp.ReturnCode == enResponseCodeService.Success && SettlementResp.ErrorCode == enErrorCode.Settlement_FullSettlementDone)
    //                {
    //                    updateddata.CallStatus = 1;
    //                }
    //                else
    //                {
    //                    updateddata.CallStatus = 0;
    //                }
    //                //HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlderArbitrage", "UpdateTransactionStatus", "LPStatusCheckSingleHanlder " + "##TrnNo:" + TrnObj.TrnNo + "##PROCESSSETLLEMENTAPI Response:" + JsonConvert.SerializeObject(_Resp));
    //                _frontTrnRepository.UpdateTradeTransactionQueueAPIStatus(TrnObj.TrnNo, NewTradetransaction.APIStatus);
    //                NewtransactionReq.RequestData = "";
    //                NewtransactionReq.ResponseData = APIResponse;
    //                _transactionStatusCheckRequest.Update(NewtransactionReq);
    //                //Thread.Sleep(500);
    //            }
    //            else if (_Resp.ReturnCode == enResponseCodeService.Success && _Resp.ErrorCode == enErrorCode.API_LP_PartialFilled)
    //            {
    //                NewTradetransaction.APIStatus = "Partial Success";
    //                BizResponse SettlementResp = await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPI(_Resp, TrnObj.TrnNo, LPProcessTransactionClsObj.RemainingQty, LPProcessTransactionClsObj.SettledQty, TrnObj.Price);
    //                HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " UpdateTransactionStatus ", " CCXTStatusCheckCall Parcial Success  :" + "##TrnNo:" + TrnObj.TrnNo + " Response : " + JsonConvert.SerializeObject(SettlementResp));
    //                _frontTrnRepository.UpdateTradeTransactionQueueAPIStatus(TrnObj.TrnNo, NewTradetransaction.APIStatus);
    //                //Thread.Sleep(500);
    //            }
    //            else if (_Resp.ReturnCode == enResponseCodeService.Fail && _Resp.ErrorCode == enErrorCode.API_LP_Cancel)
    //            {
    //                NewTradetransaction.APIStatus = "Cancel";
    //                _TransactionQueueCancelOrderArbitrage.Enqueue(new NewCancelOrderArbitrageRequestCls()
    //                {
    //                    MemberID = updateddata.MemberID,
    //                    TranNo = TrnObj.TrnNo,
    //                    accessToken = "",
    //                    CancelAll = 0,
    //                    OrderType = (enTransactionMarketType)TrnObj.Ordertype,
    //                    IsFromStuckOrder = 1
    //                });
    //                HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " UpdateTransactionStatus ", " CCXTStatusCheckCall Cancel  :" + "##TrnNo:" + TrnObj.TrnNo);
    //                Task.Delay(5000).Wait();
    //                _frontTrnRepository.UpdateTradeTransactionQueueAPIStatus(TrnObj.TrnNo, NewTradetransaction.APIStatus);
    //                NewtransactionReq.RequestData = "";
    //                NewtransactionReq.ResponseData = APIResponse;
    //                _transactionStatusCheckRequest.Update(NewtransactionReq);
    //            }
    //            else if (_Resp.ReturnCode == enResponseCodeService.Fail && _Resp.ErrorCode == enErrorCode.API_LP_Order_Not_Found)
    //            {
    //                NewTradetransaction.APIStatus = "Force Cancel";
    //                _TransactionQueueCancelOrderArbitrage.Enqueue(new NewCancelOrderArbitrageRequestCls()
    //                {
    //                    MemberID = updateddata.MemberID,
    //                    TranNo = TrnObj.TrnNo,
    //                    accessToken = "",
    //                    CancelAll = 0,
    //                    OrderType = (enTransactionMarketType)TrnObj.Ordertype,
    //                    IsFromStuckOrder = 1
    //                });
    //                HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " UpdateTransactionStatus ", " CCXTStatusCheckCall Force Cancel  :" + "##TrnNo:" + TrnObj.TrnNo);
    //                Task.Delay(1000).Wait();
    //                _frontTrnRepository.UpdateTradeTransactionQueueAPIStatus(TrnObj.TrnNo, NewTradetransaction.APIStatus);
    //                NewtransactionReq.RequestData = "";
    //                NewtransactionReq.ResponseData = APIResponse;
    //                _transactionStatusCheckRequest.Update(NewtransactionReq);
    //            }
    //            else if (_Resp.ReturnCode == enResponseCodeService.Success && _Resp.ErrorCode == enErrorCode.API_LP_Hold)
    //            {
    //                NewTradetransaction.APIStatus = "Hold";
    //                _frontTrnRepository.UpdateTradeTransactionQueueAPIStatus(TrnObj.TrnNo, NewTradetransaction.APIStatus);
    //                //HelperForLog.WriteLogIntoFile(" CCXTStatusCheckCall ", " UpdateTransactionStatus ", " CCXTStatusCheckCall Hold  :" + "##TrnNo:" + TrnObj.TrnNo);
    //                NewtransactionReq.RequestData = "";
    //                NewtransactionReq.ResponseData = APIResponse;
    //                _transactionStatusCheckRequest.Update(NewtransactionReq);
    //            }
    //            //else //((_Resp.ReturnCode == enResponseCodeService.Fail && _Resp.ErrorCode == enErrorCode.API_LP_Fail) || (_Resp.ReturnCode == enResponseCodeService.Fail && _Resp.ErrorCode == enErrorCode.API_LP_Order_Not_Found))
    //            //{     
    //            //    NewTradetransaction.APIStatus = "2";
    //            //    updateddata.CallStatus = 0;
    //            //}
    //            //_tradeTrnRepositiory.UpdateField(NewTradetransaction, o => o.APIStatus);
    //            //_trnRepositiory.UpdateField(updateddata, e => e.CallStatus);
    //        }
    //        catch (Exception ex)
    //        {
    //            HelperForLog.WriteErrorLog("CCXTStatusCheckCall", "UpdateTransactionStatus ###TrnNo### "+ TrnObj.TrnNo, ex);
    //        }
    //    }
    //}
}
