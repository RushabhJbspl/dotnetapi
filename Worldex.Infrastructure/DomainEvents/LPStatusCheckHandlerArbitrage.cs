using Binance.Net.Objects;
using Bittrex.Net.Objects;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Infrastructure.LiquidityProvider.OKExAPI;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.LiquidityProvider1;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Worldex.Infrastructure.LiquidityProvider;
using Worldex.Infrastructure.LiquidityProvider.TradeSatoshiAPI;
using CoinbasePro.Services.Orders.Models.Responses;
using CryptoExchange.Net.Objects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Huobi.Net.Objects;
using Worldex.Infrastructure.LiquidityProvider.UpbitAPI;
using Worldex.Core.Entities.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Worldex.Infrastructure.DomainEvents
{
    public class LPStatusCheckHandlerArbitrage : IRequestHandler<LPStatusCheckClsArbitrage>
    {
        private readonly ICommonRepository<TradeTransactionQueueArbitrage> _tradeTrnRepositiory;
        private readonly ICommonRepository<TransactionQueueArbitrage> _trnRepositiory;
        private readonly IFrontTrnRepository _frontTrnRepository;
        private readonly IMediator _mediator;
        private readonly ILPStatusCheckArbitrage<LPStatusCheckDataArbitrage> _lPStatusCheckQueue;
        TransactionQueueArbitrage TransactionQueuecls;
        string ControllerName = "LPStatusCheckHandlerArbitrage";
        private readonly ICommonRepository<CronMaster> _cronMaster;
        private IMemoryCache _cache;

        public LPStatusCheckHandlerArbitrage(IFrontTrnRepository FrontTrnRepository, ICommonRepository<TradeTransactionQueueArbitrage> TradeTrnRepositiory,
            ICommonRepository<TransactionQueueArbitrage> TrnRepositiory, IMediator mediator, ILPStatusCheckArbitrage<LPStatusCheckDataArbitrage> LPStatusCheckQueue,
            ICommonRepository<CronMaster> CronMaster, IMemoryCache cache)
        {
            _tradeTrnRepositiory = TradeTrnRepositiory;
            _trnRepositiory = TrnRepositiory;
            _frontTrnRepository = FrontTrnRepository;
            _mediator = mediator;
            _lPStatusCheckQueue = LPStatusCheckQueue;
            _cronMaster = CronMaster;
            _cache = cache;
        }

        public async Task<Unit> Handle(LPStatusCheckClsArbitrage request, CancellationToken cancellationToken)
        {
            List<LPStatusCheckDataArbitrage> Data = new List<LPStatusCheckDataArbitrage>();
            CronMaster cronMaster = new CronMaster();
            try
            {
                List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
                if (cronMasterList == null)
                {
                    cronMasterList = _cronMaster.List();
                    _cache.Set("CronMaster", cronMasterList);
                }
                else if (cronMasterList.Count() == 0)
                {
                    cronMasterList = _cronMaster.List();
                    _cache.Set("CronMaster", cronMasterList);
                }
                cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.LPStatusCheckArbitrage).FirstOrDefault();
                //cronMaster = _cronMaster.FindBy(e => e.Id == (short)enCronMaster.LPStatusCheckArbitrage).FirstOrDefault();
                if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
                {
                    Data = _frontTrnRepository.LPstatusCheckArbitrage(2);
                    foreach (var item in Data)
                    {
                        TransactionQueuecls = _trnRepositiory.GetById(item.TrnNo);
                        TransactionQueuecls.CallStatus = 1;
                        TransactionQueuecls.UpdatedDate = Helpers.UTC_To_IST();
                        _trnRepositiory.UpdateField(TransactionQueuecls, e=>e.CallStatus,e => e.UpdatedDate);
                        _lPStatusCheckQueue.Enqueue(item);
                    }
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LPStatusCheckHandlerArbitrage Error:##GUID " + request.uuid, ControllerName, ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    //public class LPStatusCheckSingleHanlderArbitrage : IRequestHandler<LPStatusCheckDataArbitrage>
    //{
    //    private readonly ICommonRepository<TradeTransactionQueueArbitrage> _tradeTrnRepositiory;
    //    private readonly ICommonRepository<TransactionQueueArbitrage> _trnRepositiory;
    //    private readonly BinanceLPService _binanceLPService;
    //    private readonly UserManager<ApplicationUser> _userManager;
    //    private readonly BitrexLPService _bitrexLPService;
    //    private readonly ICoinBaseService _coinBaseService;
    //    private readonly IPoloniexService _poloniexService;
    //    private readonly ITradeSatoshiLPService _tradeSatoshiLPService;
    //    private readonly IMediator _mediator;
    //    private readonly ITransactionQueue<NewCancelOrderArbitrageRequestCls> _TransactionQueueCancelOrderArbitrage;
    //    TransactionStatusCheckRequestArbitrage NewtransactionReq;
    //    private readonly ICommonRepository<TransactionStatusCheckRequestArbitrage> _transactionStatusCheckRequest;
    //    private readonly ICommonRepository<ArbitrageTransactionRequest> _transactionRequestArbitrage;
    //    private readonly IGetWebRequest _IGetWebRequest;
    //    private readonly IUpbitService _upbitService;
    //    private readonly ISettlementRepositoryArbitrageAPI<BizResponse> _SettlementRepositoryAPI;
    //    private readonly HuobiLPService _huobiLPService;

    //    private readonly IOKExLPService _oKExLPService; //Add new variable for OKEx by Pushpraj as on 20-06-2019
    //    string ControllerName = "LPStatusCheckSingleHanlderArbitrage";

    //    public LPStatusCheckSingleHanlderArbitrage(ICommonRepository<TradeTransactionQueueArbitrage> TradeTrnRepositiory, ITransactionQueue<NewCancelOrderArbitrageRequestCls> TransactionQueueCancelOrderArbitrage,
    //        ICommonRepository<TransactionQueueArbitrage> TrnRepositiory, IMediator mediator, BinanceLPService BinanceLPService, UserManager<ApplicationUser> userManager,
    //        BitrexLPService BitrexLPService, ICoinBaseService CoinBaseService, IPoloniexService PoloniexService, IUpbitService upbitService,
    //        ITradeSatoshiLPService TradeSatoshiLPService, ICommonRepository<TransactionStatusCheckRequestArbitrage> TransactionStatusCheckRequest, IGetWebRequest IGetWebRequest, HuobiLPService huobiLPService,
    //        ISettlementRepositoryArbitrageAPI<BizResponse> SettlementRepositoryAPI, IOKExLPService oKExLPService, ICommonRepository<ArbitrageTransactionRequest> TransactionRequestArbitrage)
    //    {
    //        _tradeTrnRepositiory = TradeTrnRepositiory;
    //        _TransactionQueueCancelOrderArbitrage = TransactionQueueCancelOrderArbitrage;
    //        _upbitService = upbitService;
    //        _userManager = userManager;
    //        _trnRepositiory = TrnRepositiory;
    //        _mediator = mediator;
    //        _binanceLPService = BinanceLPService;
    //        _bitrexLPService = BitrexLPService;
    //        _coinBaseService = CoinBaseService;
    //        _poloniexService = PoloniexService;
    //        _tradeSatoshiLPService = TradeSatoshiLPService;
    //        _transactionStatusCheckRequest = TransactionStatusCheckRequest;
    //        _IGetWebRequest = IGetWebRequest;
    //        _SettlementRepositoryAPI = SettlementRepositoryAPI;
    //        _huobiLPService = huobiLPService;
    //        _oKExLPService = oKExLPService; //Add new variable assignmnet for OKEx API by Pushpraj as on 20-06-2019
    //        _transactionRequestArbitrage = TransactionRequestArbitrage;
    //    }

    //    public async Task<Unit> Handle(LPStatusCheckDataArbitrage Request, CancellationToken cancellationToken)
    //    {
    //        LPProcessTransactionCls LPProcessTransactionClsObj = new LPProcessTransactionCls();
    //        BizResponse _Resp = new BizResponse();

    //        try
    //        {
    //            var updateddata = _trnRepositiory.GetById(Request.TrnNo);
    //            if (updateddata.CallStatus != 1)
    //            {
    //                return await Task.FromResult(new Unit());
    //            }
    //            var NewTradetransaction = _tradeTrnRepositiory.FindBy(e => e.TrnNo == Request.TrnNo).FirstOrDefault();
    //            NewtransactionReq = _transactionStatusCheckRequest.FindBy(e => e.TrnNo == Request.TrnNo).FirstOrDefault();
    //            if (NewtransactionReq == null)
    //            {
    //                NewtransactionReq = new TransactionStatusCheckRequestArbitrage()
    //                {
    //                    TrnNo = Request.TrnNo,
    //                    SerProDetailID = Request.AppTypeID,
    //                    CreatedDate = Helpers.UTC_To_IST(),
    //                    CreatedBy = 1,
    //                    OprTrnID = Request.TrnRefNo,
    //                    TrnID = Request.TrnRefNo,
    //                    Status = 0
    //                };
    //                NewtransactionReq = _transactionStatusCheckRequest.Add(NewtransactionReq);
    //            }
    //            NewtransactionReq.UpdatedDate = Helpers.UTC_To_IST();
    //            var ServiceProConfiguration = _IGetWebRequest.GetServiceProviderConfigurationArbitrage(Request.SerProDetailID);
    //            if (ServiceProConfiguration == null)
    //            {
    //                updateddata.CallStatus = 0;
    //                _trnRepositiory.Update(updateddata);
    //                HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlderArbitrage", "status Check hanlder", "LPStatusCheckSingleHanlderArbitrage Call web API creadential not found liquidity provider---" + "##TrnNo:" + Request.TrnNo);
    //            }
    //            else
    //            {
    //                ArbitrageTransactionRequest TransactionResponse = _transactionRequestArbitrage.FindBy(e => e.TrnNo == Request.TrnNo).FirstOrDefault();
    //                if (TransactionResponse == null)
    //                {
    //                    LPProcessTransactionClsObj.RemainingQty = Request.Amount;
    //                    LPProcessTransactionClsObj.SettledQty = 0.0m;
    //                    LPProcessTransactionClsObj.TotalQty = Request.Amount;

    //                    _Resp.ErrorCode = enErrorCode.API_LP_Order_Not_Found;
    //                    _Resp.ReturnCode = enResponseCodeService.Fail;
    //                    _Resp.ReturnMsg = "No update";

    //                    NewtransactionReq.ResponseData = "LP API call not procceed";
    //                    NewtransactionReq.RequestData = "TransactionResponse is null";
    //                    _transactionStatusCheckRequest.Update(NewtransactionReq);
    //                    goto SuccessTrade;
    //                }
    //                else if (string.IsNullOrEmpty(TransactionResponse.ResponseData) || string.IsNullOrEmpty(TransactionResponse.TrnID))
    //                {
    //                    LPProcessTransactionClsObj.RemainingQty = Request.Amount;
    //                    LPProcessTransactionClsObj.SettledQty = 0.0m;
    //                    LPProcessTransactionClsObj.TotalQty = Request.Amount;

    //                    _Resp.ErrorCode = enErrorCode.API_LP_Order_Not_Found;
    //                    _Resp.ReturnCode = enResponseCodeService.Fail;
    //                    _Resp.ReturnMsg = "No update";

    //                    NewtransactionReq.ResponseData = "LP API call not procceed";
    //                    NewtransactionReq.RequestData = "ResponseData is null and TrnRefNo is null";
    //                    _transactionStatusCheckRequest.Update(NewtransactionReq);
    //                    goto SuccessTrade;
    //                }

    //                switch (Request.AppTypeID)
    //                {
    //                    case (long)enAppType.Binance:
    //                        _binanceLPService._client.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);

    //                        //CallResult<BinancePlacedOrder> BinanceResult = JsonConvert.DeserializeObject<CallResult<BinancePlacedOrder>>(TransactionResponse.ResponseData);
    //                        string BinancePair = Request.Pair.Replace("_", "");
    //                        CallResult<BinanceOrder> BinanceResult = await _binanceLPService.GetOrderInfoAsync(BinancePair, Convert.ToInt64(Request.TrnRefNo), origClientOrderId: TransactionResponse.OprTrnID, receiveWindow: 5000);
    //                        //string Result1 = @"{""ResponseStatusCode"":200,""ResponseHeaders"":[{""Item1"":""Transfer-Encoding"",""Item2"":""chunked""},{""Item1"":""Connection"",""Item2"":""keep-alive""},{""Item1"":""Date"",""Item2"":""Sat, 22 Jun 2019 03:29:31 GMT""},{""Item1"":""Server"",""Item2"":""nginx""},{""Item1"":""Vary"",""Item2"":""Accept-Encoding""},{""Item1"":""X-MBX-USED-WEIGHT"",""Item2"":""30""},{""Item1"":""Strict-Transport-Security"",""Item2"":""max-age=31536000; includeSubdomains""},{""Item1"":""X-Frame-Options"",""Item2"":""SAMEORIGIN""},{""Item1"":""X-Xss-Protection"",""Item2"":""1; mode=block""},{""Item1"":""X-Content-Type-Options"",""Item2"":""nosniff""},{""Item1"":""Content-Security-Policy"",""Item2"":""default-src 'self'""},{""Item1"":""X-Content-Security-Policy"",""Item2"":""default-src 'self'""},{""Item1"":""X-WebKit-CSP"",""Item2"":""default-src 'self'""},{""Item1"":""Cache-Control"",""Item2"":""no-store, must-revalidate, no-cache""},{""Item1"":""Pragma"",""Item2"":""no-cache""},{""Item1"":""X-Cache"",""Item2"":""Miss from cloudfront""},{""Item1"":""Via"",""Item2"":""1.1 f9a9e5a2fe899e7acf3e13d8d7a34642.cloudfront.net (CloudFront)""},{""Item1"":""X-Amz-Cf-Pop"",""Item2"":""SIN5-C1""},{""Item1"":""X-Amz-Cf-Id"",""Item2"":""qGaBNTFMr4kkvJImYeixrSkjlIOa2agpv6Jg9m909kesG5mp9lgxpA==""},{""Item1"":""Content-Type"",""Item2"":""application/json; charset=utf-8""},{""Item1"":""Expires"",""Item2"":""0""}],""Data"":{""Symbol"":""BTCUSDT"",""OrderId"":450850454,""ClientOrderId"":""QuAnbO3AIFRmZD0Ku62loD"",""TransactTime"":1561174171083,""Price"":10823.40000000,""origQty"":0.01000000,""executedQty"":0.01000000,""cummulativeQuoteQty"":108.34010000,""Status"":""FILLED"",""TimeInForce"":""IOC"",""Type"":""LIMIT"",""Side"":""SELL"",""Fills"":[{""TradeId"":138565925,""Price"":10834.01000000,""qty"":0.01000000,""Commission"":0.00212126,""CommissionAsset"":""BNB""}]},""Error"":null,""Success"":true}";
    //                        if (BinanceResult != null)
    //                        {
    //                            if (BinanceResult.Success)
    //                            {
    //                                if (BinanceResult.Data != null)
    //                                {
    //                                    var Status = BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.New ? enTransactionStatus.Initialize :
    //                                        (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Filled ? enTransactionStatus.Success :
    //                                        (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PartiallyFilled ? enTransactionStatus.Hold :
    //                                        (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Rejected ? enTransactionStatus.OperatorFail :
    //                                        (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PendingCancel ? enTransactionStatus.Hold :
    //                                        (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Expired ? enTransactionStatus.OperatorFail :
    //                                        (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Canceled ? enTransactionStatus.OperatorFail : enTransactionStatus.OperatorFail))))));

    //                                    if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Filled)
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        _Resp.ReturnMsg = "Transaction fully Success On Binanace";
    //                                    }
    //                                    else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PartiallyFilled)
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        _Resp.ReturnMsg = "Transaction partial Success On Binanace";
    //                                    }
    //                                    else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Canceled)
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                        _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                        _Resp.ReturnMsg = "Transaction fully Cancel On Binanace";
    //                                    }
    //                                    else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.New)
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Hold;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        _Resp.ReturnMsg = "Transaction fully Hold On Binanace";
    //                                    }
    //                                    else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PendingCancel)
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                        _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                        _Resp.ReturnMsg = "Transaction Cancellation On Binanace";
    //                                    }
    //                                    else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Rejected)
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                        _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                        _Resp.ReturnMsg = "Transaction fully Rejected On Binanace";
    //                                    }
    //                                    else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Expired)
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                        _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                        _Resp.ReturnMsg = "Transaction Expired On Binanace";
    //                                    }
    //                                    else
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;

    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                        _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                        _Resp.ReturnMsg = "No update";
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                                    _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                    _Resp.ReturnMsg = "Transaction Fail On Binanace";
    //                                }
    //                            }
    //                            else
    //                            {
    //                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                                _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                _Resp.ReturnMsg = "Transaction Fail On Binanace";
    //                            }
    //                        }
    //                        else
    //                        {
    //                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                            _Resp.ReturnCode = enResponseCodeService.Fail;
    //                            _Resp.ReturnMsg = "Transaction Fail On Binanace";
    //                        }
    //                        NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
    //                        NewtransactionReq.ResponseData = JsonConvert.SerializeObject(BinanceResult);
    //                        _transactionStatusCheckRequest.Update(NewtransactionReq);
    //                        goto SuccessTrade;

    //                    case (long)enAppType.Bittrex:
    //                        _bitrexLPService._client.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);
    //                        CallResult<BittrexAccountOrder> BittrexResult = await _bitrexLPService.GetOrderInfoAsync(Guid.Parse(Request.TrnRefNo));
    //                        if (BittrexResult != null)
    //                        {
    //                            if (BittrexResult.Success)
    //                            {
    //                                if (BittrexResult.Data != null)
    //                                {
    //                                    if (BittrexResult.Data.QuantityRemaining == 0)
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BittrexResult.Data.QuantityRemaining;
    //                                        LPProcessTransactionClsObj.SettledQty = BittrexResult.Data.Quantity - BittrexResult.Data.QuantityRemaining;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        _Resp.ReturnMsg = "Transaction fully Success On Bittrex";
    //                                    }
    //                                    else if (BittrexResult.Data.QuantityRemaining < BittrexResult.Data.Quantity) // partial
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BittrexResult.Data.QuantityRemaining;
    //                                        LPProcessTransactionClsObj.SettledQty = BittrexResult.Data.Quantity - BittrexResult.Data.QuantityRemaining;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        _Resp.ReturnMsg = "Transaction partial Success On Bittrex";
    //                                    }
    //                                    else if (BittrexResult.Data.QuantityRemaining == BittrexResult.Data.Quantity) // hold
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BittrexResult.Data.QuantityRemaining;
    //                                        LPProcessTransactionClsObj.SettledQty = BittrexResult.Data.Quantity - BittrexResult.Data.QuantityRemaining;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Hold;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        _Resp.ReturnMsg = "Transaction processing Success On Bittrex";
    //                                    }
    //                                    else if (BittrexResult.Data.CancelInitiated)
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BittrexResult.Data.QuantityRemaining;
    //                                        LPProcessTransactionClsObj.SettledQty = BittrexResult.Data.Quantity - BittrexResult.Data.QuantityRemaining;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                        _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                        _Resp.ReturnMsg = "Transaction Cancel On Bittrex";
    //                                    }
    //                                    else
    //                                    {
    //                                        LPProcessTransactionClsObj.RemainingQty = BittrexResult.Data.QuantityRemaining;
    //                                        LPProcessTransactionClsObj.SettledQty = BittrexResult.Data.Quantity - BittrexResult.Data.QuantityRemaining;
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                        _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                        _Resp.ReturnMsg = "Transaction Cancel On Bittrex";
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                                    _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                    _Resp.ReturnMsg = "Transaction Fail On Bittrex";
    //                                }
    //                            }
    //                            else
    //                            {
    //                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                                _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                _Resp.ReturnMsg = "Transaction Fail On Bittrex";
    //                            }
    //                        }
    //                        else
    //                        {
    //                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                            _Resp.ReturnCode = enResponseCodeService.Fail;
    //                            _Resp.ReturnMsg = "Transaction Fail On Bittrex";
    //                        }
    //                        NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
    //                        NewtransactionReq.ResponseData = JsonConvert.SerializeObject(BittrexResult);
    //                        _transactionStatusCheckRequest.Update(NewtransactionReq);
    //                        goto SuccessTrade;

    //                    case (long)enAppType.TradeSatoshi:
    //                        //GlobalSettings.API_Key = ServiceProConfiguration.APIKey;
    //                        //GlobalSettings.Secret = ServiceProConfiguration.SecretKey;
    //                        //GetOrderReturn TradeSatoshiResult = await _tradeSatoshiLPService.GetOrderInfoAsync(Convert.ToInt64(Request.TrnRefNo));

    //                        GlobalSettings.API_Key = ServiceProConfiguration.APIKey;
    //                        GlobalSettings.Secret = ServiceProConfiguration.SecretKey;
    //                        //GlobalSettings.API_Key = "39b7d529117a42f29695c035619ce22b";
    //                        //GlobalSettings.Secret = "02cXvn92LTRRoQ3FSmbcblH555YXBtkehg+tdqNpzOY=";
    //                        GetOrderReturn TradeSatoshiResult = await _tradeSatoshiLPService.GetOrderInfoAsync(Convert.ToInt64(Request.TrnRefNo));
    //                        //GetOrdersReturn TradeSatoshiResultv1 = _tradeSatoshiLPService.GetOpenOrdersAsync(Request.Pair).Result;
    //                        HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlderArbitrage LP Response : #OrderInfo", ControllerName, " ##TrnNo:" + Request.TrnNo + " ##Response : " + JsonConvert.SerializeObject(TradeSatoshiResult));
    //                        //var Result1 = @"{""success"":true,""message"":null,""result"":{""Id"":140876176,""Market"":""ETH_BTC"",""Type"":""Sell"",""Amount"":0.01544508,""Rate"":0.03198508,""Remaining"":0.00000000,""Total"":0.00049401,""Status"":""Complete"",""Timestamp"":""2019-05-29T11:16:11.527"",""IsApi"":true}}";
    //                        //GetOrderReturn TradeSatoshiResult = JsonConvert.DeserializeObject<GetOrderReturn>(Result1);
    //                        if (TradeSatoshiResult != null)
    //                        {
    //                            if (TradeSatoshiResult.success)
    //                            {
    //                                if (TradeSatoshiResult.result == null && TradeSatoshiResult.message == null)
    //                                {
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                    _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                    _Resp.ReturnMsg = "Transaction cancel On TradeSatoshi";
    //                                }
    //                                else if (TradeSatoshiResult.result?.Status.ToLower() == "complete")
    //                                {
    //                                    LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult.result.Remaining;
    //                                    LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult.result.Amount;
    //                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_Filled;
    //                                    _Resp.ReturnCode = enResponseCodeService.Success;
    //                                    _Resp.ReturnMsg = "Transaction fully Success On TradeSatoshi";
    //                                }
    //                                else if (TradeSatoshiResult.result?.Status.ToLower() == "partial")
    //                                {
    //                                    LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult.result.Remaining;
    //                                    LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult.result.Amount - TradeSatoshiResult.result.Remaining;
    //                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
    //                                    _Resp.ReturnCode = enResponseCodeService.Success;
    //                                    _Resp.ReturnMsg = "Transaction partial Success On TradeSatoshi";
    //                                }
    //                                else if (TradeSatoshiResult.result?.Status.ToLower() == "pending")
    //                                {
    //                                    LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult.result.Remaining;
    //                                    LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult.result.Amount - TradeSatoshiResult.result.Remaining;
    //                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_Hold;
    //                                    _Resp.ReturnCode = enResponseCodeService.Success;
    //                                    _Resp.ReturnMsg = "Transaction Hold On TradeSatoshi";
    //                                } 
    //                                else
    //                                {
    //                                    //LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult.result.Remaining;
    //                                    //LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult.result.Amount - TradeSatoshiResult.result.Remaining;
    //                                    //LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                    _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                    _Resp.ReturnMsg = "Transaction cancel On TradeSatoshi";
    //                                }
    //                            }
    //                        }
    //                        else
    //                        {
    //                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                            _Resp.ReturnCode = enResponseCodeService.Fail;
    //                            _Resp.ReturnMsg = "Transaction Fail On TradeSatoshi";
    //                        }
    //                        NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
    //                        NewtransactionReq.ResponseData = JsonConvert.SerializeObject(TradeSatoshiResult);
    //                        _transactionStatusCheckRequest.Update(NewtransactionReq);
    //                        goto SuccessTrade;

    //                    case (long)enAppType.Poloniex:
    //                        PoloniexGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
    //                        PoloniexGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
    //                        object PoloniexRep = await _poloniexService.GetPoloniexOrderState(Request.TrnRefNo);
    //                        JObject Data = JObject.Parse(PoloniexRep.ToString());
    //                        var Success = Convert.ToUInt16(Data["result"]["success"]);
    //                        if (Success == 1)
    //                        {
    //                            JToken Result = Data["result"][Request.TrnRefNo];
    //                            PoloniexOrderState PoloniexResult = JsonConvert.DeserializeObject<PoloniexOrderState>(Result.ToString());

    //                            if (PoloniexResult.status == "Partially filled")
    //                            {
    //                                updateddata.MakeTransactionHold();
    //                                LPProcessTransactionClsObj.RemainingQty = PoloniexResult.amount - PoloniexResult.startingAmount;
    //                                LPProcessTransactionClsObj.SettledQty = PoloniexResult.amount;
    //                                LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
    //                                _Resp.ReturnCode = enResponseCodeService.Success;
    //                                _Resp.ReturnMsg = "Transaction partial Success On Poloniex";
    //                            }
    //                            else if (PoloniexResult.status == "Filled")
    //                            {
    //                                updateddata.MakeTransactionSuccess();
    //                                LPProcessTransactionClsObj.RemainingQty = 0;
    //                                LPProcessTransactionClsObj.SettledQty = PoloniexResult.startingAmount;
    //                                LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                _Resp.ErrorCode = enErrorCode.API_LP_Filled;
    //                                _Resp.ReturnCode = enResponseCodeService.Success;
    //                                _Resp.ReturnMsg = "Transaction fully Success On Poloniex";
    //                            }
    //                            else
    //                            {
    //                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                                _Resp.ReturnCode = enResponseCodeService.Success;
    //                                _Resp.ReturnMsg = "No update";
    //                            }
    //                        }
    //                        else
    //                        {
    //                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                            _Resp.ReturnCode = enResponseCodeService.Fail;
    //                            _Resp.ReturnMsg = "Transaction Fail On Poloniex";
    //                        }
    //                        NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
    //                        NewtransactionReq.ResponseData = JsonConvert.SerializeObject(PoloniexRep);
    //                        _transactionStatusCheckRequest.Update(NewtransactionReq);
    //                        goto SuccessTrade;

    //                    case (long)enAppType.Coinbase:
    //                        CoinBaseGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
    //                        CoinBaseGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
    //                        OrderResponse CoinbaseResult = await _coinBaseService.GetOrderById(Request.TrnRefNo);
    //                        if (CoinbaseResult.Settled)
    //                        {
    //                            updateddata.MakeTransactionSuccess();
    //                            _Resp.ErrorCode = enErrorCode.API_LP_Filled;
    //                            _Resp.ReturnCode = enResponseCodeService.Success;
    //                            _Resp.ReturnMsg = "Transaction fully Success On Coinbase";
    //                        }
    //                        else
    //                        {
    //                            _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                            _Resp.ReturnCode = enResponseCodeService.Fail;
    //                            _Resp.ReturnMsg = "No update";
    //                        }
    //                        NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
    //                        NewtransactionReq.ResponseData = JsonConvert.SerializeObject(CoinbaseResult);
    //                        _transactionStatusCheckRequest.Update(NewtransactionReq);
    //                        goto SuccessTrade;

    //                    case (long)enAppType.UpBit:
    //                        UpBitGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
    //                        UpBitGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
    //                        //Request.TrnRefNo = "a08f09b1-1718-42e2-9358-f0e5e083d3ee";
    //                        var UpBitResult = await _upbitService.GetOrderInfoAsync(Request.TrnRefNo);
    //                        if (UpBitResult != null)
    //                        {
    //                            if (UpBitResult.state.ToLower() == "done")
    //                            {
    //                                updateddata.MakeTransactionSuccess();
    //                                LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(UpBitResult.remaining_volume);
    //                                LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(UpBitResult.executed_volume);
    //                                LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                _Resp.ErrorCode = enErrorCode.API_LP_Filled;
    //                                _Resp.ReturnCode = enResponseCodeService.Success;
    //                                _Resp.ReturnMsg = "Transaction Fully Success On Upbit";
    //                            }
    //                            else if (UpBitResult.state.ToLower() == "cancel")
    //                            {
    //                                updateddata.MakeTransactionInProcess();
    //                                LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(UpBitResult.remaining_volume);
    //                                LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(UpBitResult.executed_volume);
    //                                LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                _Resp.ReturnMsg = "Transaction Cancelled On Upbit";
    //                            }
    //                            else if (UpBitResult.state.ToLower() == "wait")
    //                            {
    //                                updateddata.MakeTransactionHold();
    //                                LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(UpBitResult.remaining_volume);
    //                                LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(UpBitResult.executed_volume);
    //                                LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                _Resp.ErrorCode = enErrorCode.API_LP_Hold;
    //                                _Resp.ReturnCode = enResponseCodeService.Success;
    //                                _Resp.ReturnMsg = "Transaction Hold On Upbit";
    //                            }
    //                            else
    //                            {
    //                                _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
    //                                _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                _Resp.ReturnMsg = "No update";
    //                            }
    //                        }
    //                        else
    //                        {
    //                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                            _Resp.ReturnCode = enResponseCodeService.Fail;
    //                            _Resp.ReturnMsg = "Transaction Fail On UpBit";
    //                        }
    //                        NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
    //                        NewtransactionReq.ResponseData = JsonConvert.SerializeObject(UpBitResult);
    //                        _transactionStatusCheckRequest.Update(NewtransactionReq);
    //                        goto SuccessTrade;

    //                    case (long)enAppType.Huobi:
    //                        _huobiLPService._client.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);
    //                        WebCallResult<HuobiOrder> webCall = await _huobiLPService.GetOrderInfoAsync(Request.TrnNo);
    //                        if (webCall != null)
    //                        {
    //                            if (webCall.Success)
    //                            {
    //                                if (webCall.Data.State == HuobiOrderState.Filled)
    //                                {
    //                                    LPProcessTransactionClsObj.RemainingQty = 0;
    //                                    LPProcessTransactionClsObj.SettledQty = webCall.Data.FilledAmount;
    //                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_Filled;
    //                                    _Resp.ReturnCode = enResponseCodeService.Success;
    //                                    _Resp.ReturnMsg = "Transaction fully Success On Huobi";


    //                                }
    //                                else if (webCall.Data.State == HuobiOrderState.PartiallyFilled)
    //                                {
    //                                    LPProcessTransactionClsObj.RemainingQty = webCall.Data.Amount - webCall.Data.FilledAmount;
    //                                    LPProcessTransactionClsObj.SettledQty = webCall.Data.FilledAmount;
    //                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
    //                                    _Resp.ReturnCode = enResponseCodeService.Success;
    //                                    _Resp.ReturnMsg = "Transaction partial Success On huobi";

    //                                }
    //                                else if (webCall.Data.State == HuobiOrderState.PartiallyCanceled)
    //                                {
    //                                    LPProcessTransactionClsObj.RemainingQty = webCall.Data.FilledAmount;
    //                                    LPProcessTransactionClsObj.SettledQty = webCall.Data.FilledAmount;
    //                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
    //                                    _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                    _Resp.ReturnMsg = "Transaction partial FAIL On huobi";

    //                                }
    //                                else if (webCall.Data.State == HuobiOrderState.Canceled)
    //                                {
    //                                    updateddata.MakeTransactionOperatorFail();
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                                    _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                    _Resp.ReturnMsg = "Transaction Fail On huobi";
    //                                }
    //                                else if (webCall.Data.State == HuobiOrderState.Created)
    //                                {
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_Success;
    //                                    _Resp.ReturnCode = enResponseCodeService.Success;
    //                                    _Resp.ReturnMsg = "Transaction processing Success On huobi";

    //                                }
    //                                else
    //                                {
    //                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                                    _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                    _Resp.ReturnMsg = "Transaction Fail On huobi";
    //                                }
    //                            }
    //                        }
    //                        else
    //                        {
    //                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                            _Resp.ReturnCode = enResponseCodeService.Fail;
    //                            _Resp.ReturnMsg = "Transaction Fail On huobi";
    //                        }
    //                        NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
    //                        NewtransactionReq.ResponseData = JsonConvert.SerializeObject(webCall.Data);
    //                        _transactionStatusCheckRequest.Update(NewtransactionReq);
    //                        goto SuccessTrade;

    //                    ///Add new case for OKEx API by Pushpraj as on 20-06-2019
    //                    case (long)enAppType.OKEx:
    //                        OKEXGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
    //                        OKEXGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
    //                        OKEXGlobalSettings.PassPhrase = "paRo@1$##";

    //                        OKExGetOrderInfoReturn OKEXResult = await _oKExLPService.GetOrderInfoAsync(Request.Pair, Request.TrnRefNo, Request.TrnRefNo);
    //                        //GetOrderReturn TradeSatoshiResult = await _tradeSatoshiLPService.GetOrderInfoAsync(Convert.ToInt64(Request.TrnRefNo));

    //                        //var Result1 = @"{""success"":true,""message"":null,""result"":{""Id"":140876176,""Market"":""ETH_BTC"",""Type"":""Sell"",""Amount"":0.01544508,""Rate"":0.03198508,""Remaining"":0.00000000,""Total"":0.00049401,""Status"":""Complete"",""Timestamp"":""2019-05-29T11:16:11.527"",""IsApi"":true}}";
    //                        //GetOrderReturn TradeSatoshiResult = JsonConvert.DeserializeObject<GetOrderReturn>(Result1);
    //                        if (OKEXResult != null)
    //                        {
    //                            if (OKEXResult.code == 0)
    //                            {
    //                                if (OKEXResult.instrument_id != null)
    //                                {
    //                                    if (OKEXResult.status == "2")
    //                                    {
    //                                        updateddata.MakeTransactionSuccess();
    //                                        LPProcessTransactionClsObj.RemainingQty = decimal.Parse(OKEXResult.size) - decimal.Parse(OKEXResult.filled_qty);
    //                                        LPProcessTransactionClsObj.SettledQty = decimal.Parse(OKEXResult.filled_qty);
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        _Resp.ReturnMsg = "Transaction fully Success On OKEX";
    //                                    }
    //                                    else if (OKEXResult.status == "1")
    //                                    {
    //                                        updateddata.MakeTransactionHold();
    //                                        LPProcessTransactionClsObj.RemainingQty = decimal.Parse(OKEXResult.size) - decimal.Parse(OKEXResult.filled_qty);
    //                                        LPProcessTransactionClsObj.SettledQty = decimal.Parse(OKEXResult.filled_qty);
    //                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        _Resp.ReturnMsg = "Transaction partial Success On OKEX";
    //                                    }
    //                                    else
    //                                    {
    //                                        _Resp.ErrorCode = enErrorCode.API_LP_Success;
    //                                        _Resp.ReturnCode = enResponseCodeService.Success;
    //                                        _Resp.ReturnMsg = "No update";
    //                                    }
    //                                }
    //                            }
    //                            else
    //                            {
    //                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                                _Resp.ReturnCode = enResponseCodeService.Fail;
    //                                _Resp.ReturnMsg = OKEXResult.message;
    //                                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, "Status Check Handler arbritage", OKEXResult.message, OKEXResult.code.ToString());
    //                            }
    //                        }
    //                        else
    //                        {
    //                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
    //                            _Resp.ReturnCode = enResponseCodeService.Fail;
    //                            _Resp.ReturnMsg = "Transaction Fail On OKEX";
    //                        }
    //                        NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
    //                        NewtransactionReq.ResponseData = JsonConvert.SerializeObject(OKEXResult);
    //                        _transactionStatusCheckRequest.Update(NewtransactionReq);
    //                        goto SuccessTrade;
    //                    default:
    //                        HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlderArbitrage", "status Check hanlder", "LPStatusCheckSingleHanlderArbitrage Call web API  not found liquidity provider---" + "##TrnNo:" + Request.TrnNo);
    //                        break;
    //                }

    //                SuccessTrade:

    //                HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlderArbitrage", "status Check hanlder", "LPStatusCheckSingleHanlderArbitrage " + "##TrnNo:" + Request.TrnNo + "##Response:" + JsonConvert.SerializeObject(_Resp) + "##APIResponse" + JsonConvert.SerializeObject(LPProcessTransactionClsObj));
    //                if (_Resp.ReturnCode == enResponseCodeService.Success && _Resp.ErrorCode == enErrorCode.API_LP_Filled)
    //                {
    //                    NewTradetransaction.APIStatus = "1";
    //                    BizResponse SettlementResp = await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPI(_Resp, Request.TrnNo, 0, Request.Amount, Request.Price);
    //                    if (SettlementResp.ReturnCode == enResponseCodeService.Success && SettlementResp.ErrorCode == enErrorCode.Settlement_FullSettlementDone)
    //                    {
    //                        updateddata.CallStatus = 1;
    //                    }
    //                    else
    //                    {
    //                        updateddata.CallStatus = 0;
    //                    }
    //                    HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlderArbitrage", "status Check hanlder", "LPStatusCheckSingleHanlder " + "##TrnNo:" + Request.TrnNo + "##PROCESSSETLLEMENTAPI Response:" + JsonConvert.SerializeObject(_Resp));
    //                }
    //                else if (_Resp.ReturnCode == enResponseCodeService.Success && _Resp.ErrorCode == enErrorCode.API_LP_PartialFilled)
    //                {
    //                    NewTradetransaction.APIStatus = "4";
    //                    BizResponse SettlementResp = await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPI(_Resp, Request.TrnNo, LPProcessTransactionClsObj.RemainingQty, LPProcessTransactionClsObj.SettledQty, Request.Price);
    //                    //if (SettlementResp.ReturnCode == enResponseCodeService.Success && SettlementResp.ErrorCode == enErrorCode.Settlement_PartialSettlementDone)
    //                    //{
    //                        updateddata.CallStatus = 0;
    //                    //}
    //                    //else
    //                    //{
    //                    //    updateddata.CallStatus = 0;
    //                    //}
    //                    HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlderArbitrage", "status Check hanlder", "LPStatusCheckSingleHanlder " + "##TrnNo:" + Request.TrnNo + "##PROCESSSETLLEMENTAPI Response:" + JsonConvert.SerializeObject(_Resp));

    //                }
    //                else if (_Resp.ReturnCode == enResponseCodeService.Fail && _Resp.ErrorCode == enErrorCode.API_LP_Cancel)
    //                {
    //                    NewTradetransaction.APIStatus = "5";
    //                    _TransactionQueueCancelOrderArbitrage.Enqueue(new NewCancelOrderArbitrageRequestCls()
    //                    {
    //                        MemberID = updateddata.MemberID,
    //                        TranNo = Request.TrnNo,
    //                        accessToken = "",
    //                        CancelAll = 0,
    //                        OrderType = (enTransactionMarketType)Request.Ordertype,
    //                        IsFromStuckOrder = 0
    //                    });
    //                    Task.Delay(5000).Wait();
    //                    updateddata.CallStatus = 0;
    //                }
    //                else if (_Resp.ReturnCode == enResponseCodeService.Fail && _Resp.ErrorCode == enErrorCode.API_LP_Order_Not_Found)
    //                {
    //                    NewTradetransaction.APIStatus = "5";
    //                    _TransactionQueueCancelOrderArbitrage.Enqueue(new NewCancelOrderArbitrageRequestCls()
    //                    {
    //                        MemberID = updateddata.MemberID,
    //                        TranNo = Request.TrnNo,
    //                        accessToken = "",
    //                        CancelAll = 0,
    //                        OrderType = (enTransactionMarketType)Request.Ordertype,
    //                        IsFromStuckOrder = 1
    //                    });
    //                    Task.Delay(1000).Wait();
    //                    updateddata.CallStatus = 0;
    //                }
    //                else if (_Resp.ReturnCode == enResponseCodeService.Success && _Resp.ErrorCode == enErrorCode.API_LP_Hold)
    //                {
    //                    NewTradetransaction.APIStatus = "4";
    //                    updateddata.CallStatus = 0;
    //                }
    //                else //((_Resp.ReturnCode == enResponseCodeService.Fail && _Resp.ErrorCode == enErrorCode.API_LP_Fail) || (_Resp.ReturnCode == enResponseCodeService.Fail && _Resp.ErrorCode == enErrorCode.API_LP_Order_Not_Found))
    //                {
    //                    NewTradetransaction.APIStatus = "2";
    //                    updateddata.CallStatus = 0;
    //                }
    //                _tradeTrnRepositiory.UpdateField(NewTradetransaction, o => o.APIStatus);
    //                _trnRepositiory.UpdateField(updateddata, e => e.CallStatus);
    //                HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlderArbitrage", "status Check hanlder", "LPStatusCheckSingleHanlder " + "##TrnNo:" + Request.TrnNo + "##Update callstatus:" + updateddata.CallStatus);
    //            }
    //            return await Task.FromResult(new Unit());
    //        }
    //        catch (Exception ex)
    //        {
    //            HelperForLog.WriteErrorLog("LPStatusCheckSingleHanlderArbitrage Error:##TrnNo " + Request.TrnNo, ControllerName, ex);
    //            return await Task.FromResult(new Unit());
    //        }
    //    }


    //}


}
