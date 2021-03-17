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
using CCXT.NET.Kraken.Trade;
using CCXT.NET.CEXIO.Trade;
using Worldex.Core.ViewModels.LiquidityProvider;
using Microsoft.Extensions.Caching.Memory;
using ExchangeSharp;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Infrastructure.LiquidityProvider.Yobit;
using Worldex.Infrastructure.LiquidityProvider.EXMO;

namespace Worldex.Infrastructure.Services
{

    public class LPstatusCheckHandler : IRequestHandler<LPStatusCheckCls>
    {
        private readonly ICommonRepository<TradeTransactionQueue> _tradeTrnRepositiory;
        private readonly ICommonRepository<TransactionQueue> _trnRepositiory;
        private readonly IFrontTrnRepository _frontTrnRepository;
        private readonly IMediator _mediator;
        private readonly ILPStatusCheck<LPStatusCheckData> _lPStatusCheckQueue;
        TransactionQueue TransactionQueuecls;
        string ControllerName = "LPstatusCheckHandler";
        private readonly ICommonRepository<CronMaster> _cronMaster;
        private IMemoryCache _cache;

        public LPstatusCheckHandler(IFrontTrnRepository FrontTrnRepository, ICommonRepository<TradeTransactionQueue> TradeTrnRepositiory,
            ICommonRepository<TransactionQueue> TrnRepositiory, IMediator mediator, ILPStatusCheck<LPStatusCheckData> LPStatusCheckQueue,
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

        public async Task<Unit> Handle(LPStatusCheckCls Request, CancellationToken cancellationToken)
        {
            List<LPStatusCheckData> Data = new List<LPStatusCheckData>();
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
                cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.LPstatusCheck).FirstOrDefault();
                //cronMaster = _cronMaster.FindBy(e => e.Id == (short)enCronMaster.LPstatusCheck).FirstOrDefault();
                if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
                {
                    Data = _frontTrnRepository.LPstatusCheck();
                    foreach (var item in Data)
                    {
                        TransactionQueuecls = _trnRepositiory.GetById(item.TrnNo);
                        TransactionQueuecls.CallStatus = 1;
                        _trnRepositiory.Update(TransactionQueuecls);
                        Task.Delay(500).Wait();
                        //_lPStatusCheckQueue.Enqueue(item);
                        await _mediator.Send(item);
                    }
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LPstatusCheckHandler Error:##GUID " + Request.uuid, ControllerName, ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    public class LPStatusCheckSingleHanlder : IRequestHandler<LPStatusCheckData>
    {
        private readonly ICommonRepository<TradeTransactionQueue> _tradeTrnRepositiory;
        private readonly ICommonRepository<TransactionQueue> _trnRepositiory;
        private readonly BinanceLPService _binanceLPService;
        private readonly HuobiLPService _huobiLPService;
        private readonly BitrexLPService _bitrexLPService;
        private readonly ICoinBaseService _coinBaseService;
        private readonly IPoloniexService _poloniexService;
        private readonly ITradeSatoshiLPService _tradeSatoshiLPService;
        private readonly IOKExLPService _oKExLPService; // Add new variable for OKEx API by Pushpraj as on 18-06-2019
        private readonly IMediator _mediator;
        TransactionStatusCheckRequest NewtransactionReq;
        private readonly ICommonRepository<TransactionStatusCheckRequest> _transactionStatusCheckRequest;
        private readonly ICommonRepository<TransactionRequest> _transactionRequest;
        private readonly IGetWebRequest _IGetWebRequest;
        private readonly IUpbitService _upbitService;
        private readonly ISettlementRepositoryAPI<BizResponse> _SettlementRepositoryAPI;
        private readonly ITransactionQueue<NewCancelOrderRequestCls> _transactionQueueCancelOrder;
        private readonly IKrakenLPService _krakenLPService; //Add new variable for Kraken Exchange by Pushpraj as on 02-07-2019
        private readonly IBitfinexLPService _bitfinexLPService;//Add new variable for Bitfinex Exchange by Pushpraj as on 05-07-2019
        private readonly ICEXIOLPService _cEXIOLPService; //Add new variable for CEXIO Exchange by Pushpraj as on 13-07-2019
        private readonly IYobitLPService _yobitLPService; //Add new variable for Yobit Exchange by Pushpraj as on 15-07-2019
        private readonly IGeminiLPService _GeminiLPService;
        private readonly IEXMOLPService _eXMOLPService;
        string ControllerName = "LPStatusCheckSingleHanlder";

        public LPStatusCheckSingleHanlder( //IFrontTrnRepository FrontTrnRepository,
            ICommonRepository<TradeTransactionQueue> TradeTrnRepositiory,
            ICommonRepository<TransactionQueue> TrnRepositiory, IMediator mediator, BinanceLPService BinanceLPService, HuobiLPService huobiLPService,
            BitrexLPService BitrexLPService, ICoinBaseService CoinBaseService, IPoloniexService PoloniexService, IUpbitService upbitService,
            ITradeSatoshiLPService TradeSatoshiLPService, ICommonRepository<TransactionStatusCheckRequest> TransactionStatusCheckRequest, IGetWebRequest IGetWebRequest,
            ISettlementRepositoryAPI<BizResponse> SettlementRepositoryAPI, IOKExLPService oKExLPService, ICommonRepository<TransactionRequest> TransactionRequest,
            ITransactionQueue<NewCancelOrderRequestCls> TransactionQueueCancelOrder, IKrakenLPService krakenLPService, IBitfinexLPService bitfinexLPService
            ,ICEXIOLPService cEXIOLPService, IGeminiLPService geminiLPService,IEXMOLPService eXMOLPService, IYobitLPService yobitLPService)
        {
            _tradeTrnRepositiory = TradeTrnRepositiory;
            _GeminiLPService = geminiLPService;
            _upbitService = upbitService;
            _trnRepositiory = TrnRepositiory;
            //_frontTrnRepository = FrontTrnRepository;
            _mediator = mediator;
            _binanceLPService = BinanceLPService;
            _huobiLPService = huobiLPService;
            _bitrexLPService = BitrexLPService;
            _coinBaseService = CoinBaseService;
            _poloniexService = PoloniexService;
            _tradeSatoshiLPService = TradeSatoshiLPService;
            _transactionStatusCheckRequest = TransactionStatusCheckRequest;
            _IGetWebRequest = IGetWebRequest;
            _SettlementRepositoryAPI = SettlementRepositoryAPI;
            _oKExLPService = oKExLPService; // Add new varible assign for OKEx API by Pushpraj as on 18-06-2019
            _transactionRequest = TransactionRequest;
            _transactionQueueCancelOrder = TransactionQueueCancelOrder;
            _krakenLPService = krakenLPService; //Add new variable assignment for Kraken Exchange by Pushpraj as on 02-07-2019
            _bitfinexLPService = bitfinexLPService; //Add new variable assignment for Bitfinex Exchange by Pushpraj as on 05-07-2019
            _cEXIOLPService = cEXIOLPService;  //Add new variable for CEXIO Exchange by Pushpraj as on 13-07-2019
            _yobitLPService = yobitLPService; //Add new variable for Yobit Exchange by Pushpraj as on 15-07-2019
            _eXMOLPService = eXMOLPService;
        }

        public async Task<Unit> Handle(LPStatusCheckData Request, CancellationToken cancellationToken)
        {
            LPProcessTransactionCls LPProcessTransactionClsObj = new LPProcessTransactionCls();
            BizResponse _Resp = new BizResponse();

            try
            {
                var updateddata = _trnRepositiory.GetById(Request.TrnNo);
                if (updateddata.CallStatus != 1)
                {
                    return await Task.FromResult(new Unit());
                }
                var NewTradetransaction = _tradeTrnRepositiory.FindBy(e => e.TrnNo  == Request.TrnNo).FirstOrDefault();
                NewtransactionReq = _transactionStatusCheckRequest.FindBy(e => e.TrnNo == Request.TrnNo).FirstOrDefault();
                if (NewtransactionReq == null)
                {
                    NewtransactionReq = new TransactionStatusCheckRequest()
                    {
                        TrnNo = Request.TrnNo,
                        SerProDetailID = Request.AppTypeID,
                        CreatedDate = Helpers.UTC_To_IST(),
                        CreatedBy = 1,
                        OprTrnID = Request.TrnRefNo,
                        TrnID = Request.TrnRefNo,
                        Status = 0
                    };
                    NewtransactionReq = _transactionStatusCheckRequest.Add(NewtransactionReq);
                }
                NewtransactionReq.UpdatedDate = Helpers.UTC_To_IST();
                var ServiceProConfiguration = _IGetWebRequest.GetServiceProviderConfiguration(Request.SerProDetailID);
                if (ServiceProConfiguration == null)
                {
                    updateddata.CallStatus = 0;
                    _trnRepositiory.Update(updateddata);
                    HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlder", "status Check hanlder", "LPStatusCheckSingleHanlder Call web API creadential not found liquidity provider---" + "##TrnNo:" + Request.TrnNo);
                }
                else
                {
                    TransactionRequest TransactionResponse = _transactionRequest.FindBy(e => e.TrnNo == Request.TrnNo).FirstOrDefault();
                    if (TransactionResponse == null)
                    {
                        LPProcessTransactionClsObj.RemainingQty = Request.Amount;
                        LPProcessTransactionClsObj.SettledQty = 0.0m;
                        LPProcessTransactionClsObj.TotalQty = Request.Amount;

                        _Resp.ErrorCode = enErrorCode.API_LP_Order_Not_Found;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "No update";

                        NewtransactionReq.ResponseData = "LP API call not procceed";
                        NewtransactionReq.RequestData = "TransactionResponse is null";
                        _transactionStatusCheckRequest.Update(NewtransactionReq);
                        goto SuccessTrade;
                    }
                    else if (string.IsNullOrEmpty(TransactionResponse.ResponseData) && string.IsNullOrEmpty(TransactionResponse.TrnID))
                    {
                        LPProcessTransactionClsObj.RemainingQty = Request.Amount;
                        LPProcessTransactionClsObj.SettledQty = 0.0m;
                        LPProcessTransactionClsObj.TotalQty = Request.Amount;

                        _Resp.ErrorCode = enErrorCode.API_LP_Order_Not_Found;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "No update";

                        NewtransactionReq.ResponseData = "LP API call not procceed";
                        NewtransactionReq.RequestData = "ResponseData is null and TrnRefNo is null";
                        _transactionStatusCheckRequest.Update(NewtransactionReq);
                        goto SuccessTrade;
                    }

                    switch (Request.AppTypeID)
                    {
                        case (long)enAppType.Binance:
                            _binanceLPService._client.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);

                            //CallResult<BinancePlacedOrder> BinanceResult = JsonConvert.DeserializeObject<CallResult<BinancePlacedOrder>>(TransactionResponse.ResponseData);
                            string BinancePair = Request.Pair.Replace("_", "");
                            CallResult<BinanceOrder> BinanceResult = await _binanceLPService.GetOrderInfoAsync(BinancePair, Convert.ToInt64(Request.TrnRefNo), origClientOrderId: TransactionResponse.OprTrnID, receiveWindow: 5000);
                            //string Result1 = @"{""ResponseStatusCode"":200,""ResponseHeaders"":[{""Item1"":""Transfer-Encoding"",""Item2"":""chunked""},{""Item1"":""Connection"",""Item2"":""keep-alive""},{""Item1"":""Date"",""Item2"":""Sat, 22 Jun 2019 03:29:31 GMT""},{""Item1"":""Server"",""Item2"":""nginx""},{""Item1"":""Vary"",""Item2"":""Accept-Encoding""},{""Item1"":""X-MBX-USED-WEIGHT"",""Item2"":""30""},{""Item1"":""Strict-Transport-Security"",""Item2"":""max-age=31536000; includeSubdomains""},{""Item1"":""X-Frame-Options"",""Item2"":""SAMEORIGIN""},{""Item1"":""X-Xss-Protection"",""Item2"":""1; mode=block""},{""Item1"":""X-Content-Type-Options"",""Item2"":""nosniff""},{""Item1"":""Content-Security-Policy"",""Item2"":""default-src 'self'""},{""Item1"":""X-Content-Security-Policy"",""Item2"":""default-src 'self'""},{""Item1"":""X-WebKit-CSP"",""Item2"":""default-src 'self'""},{""Item1"":""Cache-Control"",""Item2"":""no-store, must-revalidate, no-cache""},{""Item1"":""Pragma"",""Item2"":""no-cache""},{""Item1"":""X-Cache"",""Item2"":""Miss from cloudfront""},{""Item1"":""Via"",""Item2"":""1.1 f9a9e5a2fe899e7acf3e13d8d7a34642.cloudfront.net (CloudFront)""},{""Item1"":""X-Amz-Cf-Pop"",""Item2"":""SIN5-C1""},{""Item1"":""X-Amz-Cf-Id"",""Item2"":""qGaBNTFMr4kkvJImYeixrSkjlIOa2agpv6Jg9m909kesG5mp9lgxpA==""},{""Item1"":""Content-Type"",""Item2"":""application/json; charset=utf-8""},{""Item1"":""Expires"",""Item2"":""0""}],""Data"":{""Symbol"":""BTCUSDT"",""OrderId"":450850454,""ClientOrderId"":""QuAnbO3AIFRmZD0Ku62loD"",""TransactTime"":1561174171083,""Price"":10823.40000000,""origQty"":0.01000000,""executedQty"":0.01000000,""cummulativeQuoteQty"":108.34010000,""Status"":""FILLED"",""TimeInForce"":""IOC"",""Type"":""LIMIT"",""Side"":""SELL"",""Fills"":[{""TradeId"":138565925,""Price"":10834.01000000,""qty"":0.01000000,""Commission"":0.00212126,""CommissionAsset"":""BNB""}]},""Error"":null,""Success"":true}";
                            if (BinanceResult != null)
                            {
                                if (BinanceResult.Success)
                                {
                                    if (BinanceResult.Data != null)
                                    {
                                        var Status = BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.New ? enTransactionStatus.Initialize :
                                            (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Filled ? enTransactionStatus.Success :
                                            (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PartiallyFilled ? enTransactionStatus.Hold :
                                            (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Rejected ? enTransactionStatus.OperatorFail :
                                            (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PendingCancel ? enTransactionStatus.Hold :
                                            (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Expired ? enTransactionStatus.OperatorFail :
                                            (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Canceled ? enTransactionStatus.OperatorFail : enTransactionStatus.OperatorFail))))));

                                        if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Filled)
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                            _Resp.ReturnCode = enResponseCodeService.Success;
                                            _Resp.ReturnMsg = "Transaction fully Success On Binanace";
                                        }
                                        else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PartiallyFilled)
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                            _Resp.ReturnCode = enResponseCodeService.Success;
                                            _Resp.ReturnMsg = "Transaction partial Success On Binanace";
                                        }
                                        else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Canceled)
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                            _Resp.ReturnCode = enResponseCodeService.Fail;
                                            _Resp.ReturnMsg = "Transaction fully Cancel On Binanace";
                                        }
                                        else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.New)
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_Hold;
                                            _Resp.ReturnCode = enResponseCodeService.Success;
                                            _Resp.ReturnMsg = "Transaction fully Hold On Binanace";
                                        }
                                        else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PendingCancel)
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                            _Resp.ReturnCode = enResponseCodeService.Fail;
                                            _Resp.ReturnMsg = "Transaction Cancellation On Binanace";
                                        }
                                        else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Rejected)
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                            _Resp.ReturnCode = enResponseCodeService.Fail;
                                            _Resp.ReturnMsg = "Transaction fully Rejected On Binanace";
                                        }
                                        else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Expired)
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                            _Resp.ReturnCode = enResponseCodeService.Fail;
                                            _Resp.ReturnMsg = "Transaction Expired On Binanace";
                                        }
                                        else
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;

                                            _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                            _Resp.ReturnCode = enResponseCodeService.Fail;
                                            _Resp.ReturnMsg = "No update";
                                        }
                                    }
                                    else
                                    {
                                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                        _Resp.ReturnCode = enResponseCodeService.Fail;
                                        _Resp.ReturnMsg = "Transaction Fail On Binanace";
                                    }
                                }
                                else
                                {
                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                    _Resp.ReturnCode = enResponseCodeService.Fail;
                                    _Resp.ReturnMsg = "Transaction Fail On Binanace";
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On Binanace";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(BinanceResult);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;

                        case (long)enAppType.Bittrex:
                            _bitrexLPService._client.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);
                            CallResult<BittrexAccountOrder> BittrexResult = await _bitrexLPService.GetOrderInfoAsync(Guid.Parse(Request.TrnRefNo));
                            if (BittrexResult != null)
                            {
                                if (BittrexResult.Success)
                                {
                                    if (BittrexResult.Data != null)
                                    {
                                        if (BittrexResult.Data.QuantityRemaining == 0)
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BittrexResult.Data.QuantityRemaining;
                                            LPProcessTransactionClsObj.SettledQty = BittrexResult.Data.Quantity - BittrexResult.Data.QuantityRemaining;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                            _Resp.ReturnCode = enResponseCodeService.Success;
                                            _Resp.ReturnMsg = "Transaction fully Success On Bittrex";
                                        }
                                        else if (BittrexResult.Data.QuantityRemaining < BittrexResult.Data.Quantity) // partial
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BittrexResult.Data.QuantityRemaining;
                                            LPProcessTransactionClsObj.SettledQty = BittrexResult.Data.Quantity - BittrexResult.Data.QuantityRemaining;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                            _Resp.ReturnCode = enResponseCodeService.Success;
                                            _Resp.ReturnMsg = "Transaction partial Success On Bittrex";
                                        }
                                        else if (BittrexResult.Data.QuantityRemaining == BittrexResult.Data.Quantity) // hold
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BittrexResult.Data.QuantityRemaining;
                                            LPProcessTransactionClsObj.SettledQty = BittrexResult.Data.Quantity;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_Success;
                                            _Resp.ReturnCode = enResponseCodeService.Success;
                                            _Resp.ReturnMsg = "Transaction processing Success On Bittrex";
                                        }
                                        else if (BittrexResult.Data.CancelInitiated)
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BittrexResult.Data.QuantityRemaining;
                                            LPProcessTransactionClsObj.SettledQty = BittrexResult.Data.Quantity - BittrexResult.Data.QuantityRemaining;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                            _Resp.ReturnCode = enResponseCodeService.Fail;
                                            _Resp.ReturnMsg = "Transaction Cancel On Bittrex";
                                        }
                                        else
                                        {
                                            LPProcessTransactionClsObj.RemainingQty = BittrexResult.Data.QuantityRemaining;
                                            LPProcessTransactionClsObj.SettledQty = BittrexResult.Data.Quantity - BittrexResult.Data.QuantityRemaining;
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                            _Resp.ReturnCode = enResponseCodeService.Fail;
                                            _Resp.ReturnMsg = "Transaction Cancel On Bittrex";
                                        }
                                    }
                                    else
                                    {
                                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                        _Resp.ReturnCode = enResponseCodeService.Fail;
                                        _Resp.ReturnMsg = "Transaction Fail On Bittrex";
                                    }

                                }
                                else
                                {
                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                    _Resp.ReturnCode = enResponseCodeService.Fail;
                                    _Resp.ReturnMsg = "Transaction Fail On Bittrex";
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On Bittrex";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(BittrexResult);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;

                        case (long)enAppType.TradeSatoshi:
                            //GlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                            //GlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                            //GetOrderReturn TradeSatoshiResult = await _tradeSatoshiLPService.GetOrderInfoAsync(Convert.ToInt64(Request.TrnRefNo));

                            GlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                            GlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                            //GlobalSettings.API_Key = "39b7d529117a42f29695c035619ce22b";
                            //GlobalSettings.Secret = "02cXvn92LTRRoQ3FSmbcblH555YXBtkehg+tdqNpzOY=";
                            GetOrderReturn TradeSatoshiResult = await _tradeSatoshiLPService.GetOrderInfoAsync(Convert.ToInt64(Request.TrnRefNo));
                            //GetOrdersReturn TradeSatoshiResultv1 = _tradeSatoshiLPService.GetOpenOrdersAsync(Request.Pair).Result;

                            //var Result1 = @"{""success"":true,""message"":null,""result"":{""Id"":140876176,""Market"":""ETH_BTC"",""Type"":""Sell"",""Amount"":0.01544508,""Rate"":0.03198508,""Remaining"":0.00000000,""Total"":0.00049401,""Status"":""Complete"",""Timestamp"":""2019-05-29T11:16:11.527"",""IsApi"":true}}";
                            //GetOrderReturn TradeSatoshiResult = JsonConvert.DeserializeObject<GetOrderReturn>(Result1);
                            if (TradeSatoshiResult != null)
                            {
                                if (TradeSatoshiResult.success)
                                {
                                    if (TradeSatoshiResult.result?.Status.ToLower() == "complete")
                                    {
                                        LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult.result.Remaining;
                                        LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult.result.Amount;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction fully Success On TradeSatoshi";
                                    }
                                    else if (TradeSatoshiResult.result?.Status.ToLower() == "partial")
                                    {
                                        LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult.result.Remaining;
                                        LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult.result.Amount - TradeSatoshiResult.result.Remaining;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction partial Success On TradeSatoshi";
                                    }
                                    else if (TradeSatoshiResult.result?.Status.ToLower() == "pending")
                                    {
                                        LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult.result.Remaining;
                                        LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult.result.Amount - TradeSatoshiResult.result.Remaining;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Hold;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction Hold On TradeSatoshi";
                                    }
                                    else if (TradeSatoshiResult.result == null && TradeSatoshiResult.message == null)
                                    {
                                        //LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult.result.Remaining;
                                        //LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult.result.Amount - TradeSatoshiResult.result.Remaining;
                                        //LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                        _Resp.ReturnCode = enResponseCodeService.Fail;
                                        _Resp.ReturnMsg = "Transaction cancel On TradeSatoshi";
                                    }
                                    else
                                    {
                                        //LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult.result.Remaining;
                                        //LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult.result.Amount - TradeSatoshiResult.result.Remaining;
                                        //LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                        _Resp.ReturnCode = enResponseCodeService.Fail;
                                        _Resp.ReturnMsg = "Transaction cancel On TradeSatoshi";
                                    }
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On TradeSatoshi";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(TradeSatoshiResult);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;

                        case (long)enAppType.Poloniex:
                            PoloniexGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                            PoloniexGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                            object PoloniexRep = await _poloniexService.GetPoloniexOrderState(Request.TrnRefNo);
                            JObject Data = JObject.Parse(PoloniexRep.ToString());
                            var Success = Convert.ToUInt16(Data["result"]["success"]);
                            if (Success == 1)
                            {
                                JToken Result = Data["result"][Request.TrnRefNo];
                                PoloniexOrderState PoloniexResult = JsonConvert.DeserializeObject<PoloniexOrderState>(Result.ToString());

                                if (PoloniexResult.status == "Partially filled")
                                {
                                    updateddata.MakeTransactionHold();
                                    LPProcessTransactionClsObj.RemainingQty = PoloniexResult.amount - PoloniexResult.startingAmount;
                                    LPProcessTransactionClsObj.SettledQty = PoloniexResult.amount;
                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                    _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                    _Resp.ReturnCode = enResponseCodeService.Success;
                                    _Resp.ReturnMsg = "Transaction partial Success On Poloniex";
                                }
                                else if (PoloniexResult.status == "Filled")
                                {
                                    updateddata.MakeTransactionSuccess();
                                    LPProcessTransactionClsObj.RemainingQty = 0;
                                    LPProcessTransactionClsObj.SettledQty = PoloniexResult.startingAmount;
                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                    _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                    _Resp.ReturnCode = enResponseCodeService.Success;
                                    _Resp.ReturnMsg = "Transaction fully Success On Poloniex";
                                }
                                else
                                {
                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                    _Resp.ReturnCode = enResponseCodeService.Success;
                                    _Resp.ReturnMsg = "No update";
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On Poloniex";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(PoloniexRep);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;

                        case (long)enAppType.Coinbase:
                            CoinBaseGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                            CoinBaseGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                            OrderResponse CoinbaseResult = await _coinBaseService.GetOrderById(Request.TrnRefNo);
                            if (CoinbaseResult.Settled)
                            {
                                updateddata.MakeTransactionSuccess();
                                _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                _Resp.ReturnCode = enResponseCodeService.Success;
                                _Resp.ReturnMsg = "Transaction fully Success On Coinbase";
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "No update";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(CoinbaseResult);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;

                        case (long)enAppType.UpBit:
                            UpBitGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                            UpBitGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                            //Request.TrnRefNo = "a08f09b1-1718-42e2-9358-f0e5e083d3ee";
                            var UpBitResult = await _upbitService.GetOrderInfoAsync(Request.TrnRefNo);
                            if (UpBitResult != null)
                            {
                                if (UpBitResult.state.ToLower() == "done")
                                {
                                    updateddata.MakeTransactionSuccess();
                                    LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(UpBitResult.remaining_volume);
                                    LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(UpBitResult.executed_volume);
                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                    _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                    _Resp.ReturnCode = enResponseCodeService.Success;
                                    _Resp.ReturnMsg = "Transaction Fully Success On Upbit";
                                }
                                else if (UpBitResult.state.ToLower() == "cancel")
                                {
                                    updateddata.MakeTransactionInProcess();
                                    LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(UpBitResult.remaining_volume);
                                    LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(UpBitResult.executed_volume);
                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                    _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                    _Resp.ReturnCode = enResponseCodeService.Fail;
                                    _Resp.ReturnMsg = "Transaction Cancelled On Upbit";
                                }
                                else if (UpBitResult.state.ToLower() == "wait")
                                {
                                    updateddata.MakeTransactionHold();
                                    LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(UpBitResult.remaining_volume);
                                    LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(UpBitResult.executed_volume);
                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                    _Resp.ErrorCode = enErrorCode.API_LP_Hold;
                                    _Resp.ReturnCode = enResponseCodeService.Success;
                                    _Resp.ReturnMsg = "Transaction Hold On Upbit";
                                }
                                else
                                {
                                    _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                    _Resp.ReturnCode = enResponseCodeService.Fail;
                                    _Resp.ReturnMsg = "No update";
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On UpBit";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(UpBitResult);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;

                        case (long)enAppType.Huobi:
                            _huobiLPService._client.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);
                            WebCallResult<HuobiOrder> webCall = await _huobiLPService.GetOrderInfoAsync(Request.TrnNo);
                            if (webCall != null)
                            {
                                if (webCall.Success)
                                {
                                    if (webCall.Data.State == HuobiOrderState.Filled)
                                    {
                                        LPProcessTransactionClsObj.RemainingQty = 0;
                                        LPProcessTransactionClsObj.SettledQty = webCall.Data.FilledAmount;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction fully Success On Huobi";


                                    }
                                    else if (webCall.Data.State == HuobiOrderState.PartiallyFilled)
                                    {
                                        LPProcessTransactionClsObj.RemainingQty = webCall.Data.Amount - webCall.Data.FilledAmount;
                                        LPProcessTransactionClsObj.SettledQty = webCall.Data.FilledAmount;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction partial Success On huobi";

                                    }
                                    else if (webCall.Data.State == HuobiOrderState.PartiallyCanceled)
                                    {
                                        LPProcessTransactionClsObj.RemainingQty = webCall.Data.FilledAmount;
                                        LPProcessTransactionClsObj.SettledQty = webCall.Data.FilledAmount;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                        _Resp.ReturnCode = enResponseCodeService.Fail;
                                        _Resp.ReturnMsg = "Transaction partial FAIL On huobi";

                                    }
                                    else if (webCall.Data.State == HuobiOrderState.Canceled)
                                    {
                                        updateddata.MakeTransactionOperatorFail();
                                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                        _Resp.ReturnCode = enResponseCodeService.Fail;
                                        _Resp.ReturnMsg = "Transaction Fail On huobi";
                                    }
                                    else if (webCall.Data.State == HuobiOrderState.Created)
                                    {
                                        _Resp.ErrorCode = enErrorCode.API_LP_Success;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction processing Success On huobi";

                                    }
                                    else
                                    {
                                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                        _Resp.ReturnCode = enResponseCodeService.Fail;
                                        _Resp.ReturnMsg = "Transaction Fail On huobi";
                                    }
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On huobi";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(webCall.Data);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;

                        ///Add new case for OKEx API by Pushpraj as on 20-06-2019
                        case (long)enAppType.OKEx:
                            OKEXGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                            OKEXGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                            OKEXGlobalSettings.PassPhrase = "paRo@1$##";

                            OKExGetOrderInfoReturn OKEXResult = await _oKExLPService.GetOrderInfoAsync(Request.Pair, Request.TrnRefNo, Request.TrnRefNo);
                            //GetOrderReturn TradeSatoshiResult = await _tradeSatoshiLPService.GetOrderInfoAsync(Convert.ToInt64(Request.TrnRefNo));

                            //var Result1 = @"{""success"":true,""message"":null,""result"":{""Id"":140876176,""Market"":""ETH_BTC"",""Type"":""Sell"",""Amount"":0.01544508,""Rate"":0.03198508,""Remaining"":0.00000000,""Total"":0.00049401,""Status"":""Complete"",""Timestamp"":""2019-05-29T11:16:11.527"",""IsApi"":true}}";
                            //GetOrderReturn TradeSatoshiResult = JsonConvert.DeserializeObject<GetOrderReturn>(Result1);
                            if (OKEXResult != null)
                            {
                                if (OKEXResult.code == 0)
                                {
                                    if (OKEXResult.instrument_id != null)
                                    {
                                        if (OKEXResult.status == "2")
                                        {
                                            updateddata.MakeTransactionSuccess();
                                            LPProcessTransactionClsObj.RemainingQty = decimal.Parse(OKEXResult.size) - decimal.Parse(OKEXResult.filled_qty);
                                            LPProcessTransactionClsObj.SettledQty = decimal.Parse(OKEXResult.filled_qty);
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                            _Resp.ReturnCode = enResponseCodeService.Success;
                                            _Resp.ReturnMsg = "Transaction fully Success On OKEX";
                                        }
                                        else if (OKEXResult.status == "1")
                                        {
                                            updateddata.MakeTransactionHold();
                                            LPProcessTransactionClsObj.RemainingQty = decimal.Parse(OKEXResult.size) - decimal.Parse(OKEXResult.filled_qty);
                                            LPProcessTransactionClsObj.SettledQty = decimal.Parse(OKEXResult.filled_qty);
                                            LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                            _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                            _Resp.ReturnCode = enResponseCodeService.Success;
                                            _Resp.ReturnMsg = "Transaction partial Success On OKEX";
                                        }
                                        else
                                        {
                                            _Resp.ErrorCode = enErrorCode.API_LP_Success;
                                            _Resp.ReturnCode = enResponseCodeService.Success;
                                            _Resp.ReturnMsg = "No update";
                                        }
                                    }
                                }
                                else
                                {
                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                    _Resp.ReturnCode = enResponseCodeService.Fail;
                                    _Resp.ReturnMsg = OKEXResult.message;
                                    HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, "Status Check Handler arbritage", OKEXResult.message, OKEXResult.code.ToString());
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On OKEX";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(OKEXResult);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;

                        //Add new case for Kraken Exchange by Pushpraj as on 02-07-2019
                        case (long)enAppType.Kraken:
                            //OKEXGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                            //OKEXGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                            //OKEXGlobalSettings.PassPhrase = "paRo@1$##";

                            KMyOrderItem KrakenResult = await _krakenLPService.GetLPStatusCheck(false, Request.TrnRefNo, Request.TrnNo.ToString());
                            //GetOrderReturn TradeSatoshiResult = await _tradeSatoshiLPService.GetOrderInfoAsync(Convert.ToInt64(Request.TrnRefNo));

                            //var Result1 = @"{""success"":true,""message"":null,""result"":{""Id"":140876176,""Market"":""ETH_BTC"",""Type"":""Sell"",""Amount"":0.01544508,""Rate"":0.03198508,""Remaining"":0.00000000,""Total"":0.00049401,""Status"":""Complete"",""Timestamp"":""2019-05-29T11:16:11.527"",""IsApi"":true}}";
                            //GetOrderReturn TradeSatoshiResult = JsonConvert.DeserializeObject<GetOrderReturn>(Result1);
                            if (KrakenResult != null)
                            {
                                if (KrakenResult.orderId != null)
                                {
                                    if (KrakenResult.status == "Partially")
                                    {
                                        //updateddata.MakeTransactionSuccess();
                                        LPProcessTransactionClsObj.RemainingQty = KrakenResult.amount - KrakenResult.filled;
                                        LPProcessTransactionClsObj.SettledQty = KrakenResult.filled;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction fully Success On Kraken";
                                    }
                                    else if (KrakenResult.status == "closed")
                                    {
                                        //updateddata.MakeTransactionHold();
                                        LPProcessTransactionClsObj.RemainingQty = 0;
                                        LPProcessTransactionClsObj.SettledQty = KrakenResult.quantity;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction partial Success On Kraken";
                                    }
                                    else
                                    {
                                        _Resp.ErrorCode = enErrorCode.API_LP_Success;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "No update";
                                    }

                                }
                                else
                                {
                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                    _Resp.ReturnCode = enResponseCodeService.Fail;
                                    _Resp.ReturnMsg = "error";
                                    HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, "Status Check Handler arbritage", "", "");
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On OKEX";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(KrakenResult);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;
                        //Add new case for Bitfinex Exchange by Pushpraj as on 05-07-2019
                        case (long)enAppType.Bitfinex:

                            BitfinexStatusCheckResponse BitfinexResult = await _bitfinexLPService.GetStatusCheck(int.Parse(Request.TrnRefNo));
                            if (BitfinexResult != null)
                            {
                                if (BitfinexResult.id != 0)
                                {
                                    decimal OriginalQty = (decimal.Parse(BitfinexResult.original_amount) / decimal.Parse(BitfinexResult.price));
                                    decimal RemainingQty = (decimal.Parse(BitfinexResult.remaining_amount) / decimal.Parse(BitfinexResult.price));
                                    if ((OriginalQty - RemainingQty) == 0)
                                    {
                                        updateddata.MakeTransactionSuccess();
                                        LPProcessTransactionClsObj.RemainingQty = OriginalQty - RemainingQty;
                                        LPProcessTransactionClsObj.SettledQty = RemainingQty - OriginalQty;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction fully Success On Bitfinex";
                                    }
                                    else if (RemainingQty < OriginalQty)
                                    {
                                        updateddata.MakeTransactionSuccess();
                                        LPProcessTransactionClsObj.RemainingQty = RemainingQty;
                                        LPProcessTransactionClsObj.SettledQty = RemainingQty - OriginalQty;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction partial Success On Bitfinex";
                                    }
                                    else if (RemainingQty == OriginalQty)
                                    {
                                        updateddata.MakeTransactionInProcess();
                                        LPProcessTransactionClsObj.RemainingQty = RemainingQty;
                                        LPProcessTransactionClsObj.SettledQty = OriginalQty;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Hold;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction processing Success On Bitfinex";
                                    }
                                    else if (BitfinexResult.is_cancelled == true)
                                    {
                                        LPProcessTransactionClsObj.RemainingQty = RemainingQty;
                                        LPProcessTransactionClsObj.SettledQty = OriginalQty - RemainingQty;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                        _Resp.ReturnCode = enResponseCodeService.Fail;
                                        _Resp.ReturnMsg = "Transaction Cancel On Bitfinex";
                                    }
                                    else
                                    {
                                        _Resp.ErrorCode = enErrorCode.API_LP_Success;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "No update";
                                    }

                                }
                                else
                                {
                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                    _Resp.ReturnCode = enResponseCodeService.Fail;
                                    _Resp.ReturnMsg = BitfinexResult.message;
                                    HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, "Status Check Handler arbritage", "", "");
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On Bitfinex";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(BitfinexResult);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;

                        case (long)enAppType.Gemini:

                            GeminiStatusCheckResponse GeminiResult = await _GeminiLPService.GetStatusCheck(int.Parse(Request.TrnRefNo));
                            if (GeminiResult != null && String.IsNullOrEmpty(GeminiResult.message) && GeminiResult.Data != null)
                            {
                                if (Convert.ToInt64(GeminiResult.Data.id) != 0)
                                {
                                    decimal OriginalQty = (decimal.Parse(GeminiResult.Data.original_amount));
                                    decimal RemainingQty = (decimal.Parse(GeminiResult.Data.remaining_amount));
                                    if (RemainingQty == 0)
                                    {
                                        updateddata.MakeTransactionSuccess();
                                        LPProcessTransactionClsObj.RemainingQty = OriginalQty - RemainingQty;
                                        LPProcessTransactionClsObj.SettledQty = RemainingQty - OriginalQty;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction fully Success On Gemini";
                                    }
                                    else if (RemainingQty < OriginalQty)
                                    {
                                        updateddata.MakeTransactionSuccess();
                                        LPProcessTransactionClsObj.RemainingQty = RemainingQty;
                                        LPProcessTransactionClsObj.SettledQty = RemainingQty - OriginalQty;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction partial Success On Gemini";
                                    }
                                    else if (RemainingQty == OriginalQty)
                                    {
                                        updateddata.MakeTransactionInProcess();
                                        LPProcessTransactionClsObj.RemainingQty = RemainingQty;
                                        LPProcessTransactionClsObj.SettledQty = OriginalQty;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Hold;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction processing Success On Gemini";
                                    }
                                    else if (GeminiResult.Data.is_cancelled == true)
                                    {
                                        LPProcessTransactionClsObj.RemainingQty = RemainingQty;
                                        LPProcessTransactionClsObj.SettledQty = OriginalQty - RemainingQty;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                        _Resp.ReturnCode = enResponseCodeService.Fail;
                                        _Resp.ReturnMsg = "Transaction Cancel On Gemini";
                                    }
                                    else
                                    {
                                        _Resp.ErrorCode = enErrorCode.API_LP_Success;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "No update";
                                    }
                                }
                                else
                                {
                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                    _Resp.ReturnCode = enResponseCodeService.Fail;
                                    _Resp.ReturnMsg = GeminiResult.message;
                                    HelperForLog.WriteLogIntoFile("StatusCheck-Gemini", "Status Check Handler", "", "");
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On Gemini";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(GeminiResult);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;

                        //Add new case for CEXIO Exchange by Pushpraj as on 13-07-2019
                        case (long)enAppType.CEXIO:
                            COpenOrderItem CEXIOResult = await _cEXIOLPService.GetStatusCheck(int.Parse(Request.TrnRefNo));
                            if (CEXIOResult != null)
                            {
                                if (CEXIOResult.orderId != "")
                                {
                                    if (CEXIOResult.orderStatus.ToString() == "2")
                                    {
                                        updateddata.MakeTransactionSuccess();
                                        LPProcessTransactionClsObj.RemainingQty = CEXIOResult.remaining - CEXIOResult.quantity;
                                        LPProcessTransactionClsObj.SettledQty = CEXIOResult.quantity;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction fully Success On CEXIO";
                                    }
                                    else if (CEXIOResult.orderStatus.ToString() == "1")
                                    {
                                        updateddata.MakeTransactionHold();
                                        LPProcessTransactionClsObj.RemainingQty = CEXIOResult.remaining - CEXIOResult.quantity;
                                        LPProcessTransactionClsObj.SettledQty = CEXIOResult.quantity;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction partial Success On CEXIO";
                                    }
                                    else
                                    {
                                        _Resp.ErrorCode = enErrorCode.API_LP_Success;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "No update";
                                    }

                                }
                                else
                                {
                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                    _Resp.ReturnCode = enResponseCodeService.Fail;
                                    _Resp.ReturnMsg = "error";
                                    HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, "Status Check Handler arbritage", "", "");
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On CEXIO";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(CEXIOResult);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;

                        case (long)enAppType.EXMO:
                            EXMOGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                            EXMOGlobalSettings.Secret = ServiceProConfiguration.SecretKey;

                            EXMOOpenOrderResp OpenOrderResult = await _eXMOLPService.GetOpenOrders();
                            EXMOOrderTradeResponse TradeResponse = await _eXMOLPService.GetTradeByOrderId(Request.TrnRefNo);
                            EXMOCancelOrderListResponse CancelResponse = await _eXMOLPService.GetCancelOrderList();
                            string EXMOResponseData = "";
                            if (OpenOrderResult != null && OpenOrderResult.Data != null)
                            {                                
                                EXMOOpenOrderUnit IsExist = OpenOrderResult.Data.Single(e => e.order_id.Equals(Request.TrnRefNo));
                                if(IsExist != null)
                                {
                                    EXMOResponseData = JsonConvert.SerializeObject(OpenOrderResult);
                                    LPProcessTransactionClsObj.RemainingQty = Request.Amount - Convert.ToDecimal(IsExist.quantity);
                                    LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(IsExist.quantity);
                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                    _Resp.ErrorCode = enErrorCode.API_LP_Hold;
                                    _Resp.ReturnCode = enResponseCodeService.Success;
                                    _Resp.ReturnMsg = "Transaction Hold On EXMO";
                                }
                            }
                            else if(TradeResponse != null && TradeResponse.trades != null)
                            {
                                EXMOResponseData = JsonConvert.SerializeObject(TradeResponse);
                                decimal sum = TradeResponse.trades.Sum(e => e.quantity);
                                if(sum == Request.Amount)
                                {
                                    LPProcessTransactionClsObj.RemainingQty = 0;
                                    LPProcessTransactionClsObj.SettledQty = sum;
                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                    _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                    _Resp.ReturnCode = enResponseCodeService.Success;
                                    _Resp.ReturnMsg = "Transaction fully Success On EXMO";
                                }
                                else
                                {
                                    LPProcessTransactionClsObj.RemainingQty = Request.Amount - sum;
                                    LPProcessTransactionClsObj.SettledQty = sum;
                                    LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                    _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                    _Resp.ReturnCode = enResponseCodeService.Success;
                                    _Resp.ReturnMsg = "Transaction partial Success On EXMO";
                                }
                            }
                            else if (CancelResponse != null && CancelResponse.order_id == Convert.ToInt32(Request.TrnRefNo))
                            {
                                EXMOResponseData = JsonConvert.SerializeObject(CancelResponse);
                                LPProcessTransactionClsObj.RemainingQty = Request.Amount - CancelResponse.quantity;
                                LPProcessTransactionClsObj.SettledQty = CancelResponse.quantity;
                                LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                _Resp.ErrorCode = enErrorCode.API_LP_Cancel;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Cancel On EXMO";
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On EXMO";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = EXMOResponseData;
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;

                        case (long)enAppType.Yobit:
                            YobitGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                            YobitGlobalSettings.Secret = ServiceProConfiguration.SecretKey;

                            YobitAPIReqRes.ExchangeOrderResult YobitResult = await _yobitLPService.GetLPStatusCheck(Request.TrnNo.ToString(),Request.Pair);
                            if (YobitResult != null)
                            {
                                if (YobitResult.OrderId != null)
                                {
                                    decimal OriginalQty = YobitResult.Amount / YobitResult.Price;
                                    decimal FilledQty = YobitResult.AmountFilled / YobitResult.Price;
                                    if (YobitResult.Result == YobitAPIReqRes.ExchangeAPIOrderResult.Filled)
                                    {
                                        updateddata.MakeTransactionSuccess();
                                        LPProcessTransactionClsObj.RemainingQty = OriginalQty - FilledQty;
                                        LPProcessTransactionClsObj.SettledQty = FilledQty;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction fully Success On Yobit";
                                    }
                                    else if (YobitResult.Result == YobitAPIReqRes.ExchangeAPIOrderResult.FilledPartially)
                                    {
                                        updateddata.MakeTransactionHold();
                                        LPProcessTransactionClsObj.RemainingQty = OriginalQty - FilledQty;
                                        LPProcessTransactionClsObj.SettledQty = FilledQty;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction partial Success On Yobit";
                                    }
                                    else if (YobitResult.Result == YobitAPIReqRes.ExchangeAPIOrderResult.Canceled)
                                    {
                                        updateddata.MakeTransactionHold();
                                        LPProcessTransactionClsObj.RemainingQty = OriginalQty - FilledQty;
                                        LPProcessTransactionClsObj.SettledQty = FilledQty;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction Fully Cancel On Yobit";
                                    }
                                    else if (YobitResult.Result == YobitAPIReqRes.ExchangeAPIOrderResult.PendingCancel)
                                    {
                                        updateddata.MakeTransactionHold();
                                        LPProcessTransactionClsObj.RemainingQty = OriginalQty - FilledQty;
                                        LPProcessTransactionClsObj.SettledQty = FilledQty;
                                        LPProcessTransactionClsObj.TotalQty = Request.Amount;
                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction Cancellation On Yobit";
                                    }
                                    else
                                    {
                                        _Resp.ErrorCode = enErrorCode.API_LP_Success;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "No update";
                                    }

                                }
                                else
                                {
                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                    _Resp.ReturnCode = enResponseCodeService.Fail;
                                    _Resp.ReturnMsg = YobitResult.error.ToString();
                                    HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, "Status Check Handler arbritage", "", "");
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On CEXIO";
                            }
                            NewtransactionReq.RequestData = "Pair:" + Request.Pair + "Order ID :" + Request.TrnRefNo;
                            NewtransactionReq.ResponseData = JsonConvert.SerializeObject(YobitResult);
                            _transactionStatusCheckRequest.Update(NewtransactionReq);
                            goto SuccessTrade;
                        default:
                            HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlder", "status Check hanlder", "LPStatusCheckSingleHanlder Call web API  not found liquidity provider---" + "##TrnNo:" + Request.TrnNo);
                            break;
                    }

                SuccessTrade:

                    HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlder", "status Check hanlder", "LPStatusCheckSingleHanlder " + "##TrnNo:" + Request.TrnNo + "##Response:" + JsonConvert.SerializeObject(_Resp) + "##APIResponse" + JsonConvert.SerializeObject(LPProcessTransactionClsObj));
                    if (_Resp.ReturnCode == enResponseCodeService.Success && _Resp.ErrorCode == enErrorCode.API_LP_Filled)
                    {
                        NewTradetransaction.APIStatus = "1";
                        BizResponse SettlementResp = await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPI(_Resp, Request.TrnNo, 0, Request.Amount, Request.Price);
                        if (SettlementResp.ReturnCode == enResponseCodeService.Success && SettlementResp.ErrorCode == enErrorCode.Settlement_PartialSettlementDone)
                        {
                            updateddata.CallStatus = 1;
                        }
                        else
                        {
                            updateddata.CallStatus = 0;
                        }
                        HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlder", "status Check hanlder", "LPStatusCheckSingleHanlder " + "##TrnNo:" + Request.TrnNo + "##PROCESSSETLLEMENTAPI Response:" + JsonConvert.SerializeObject(_Resp));
                    }
                    else if (_Resp.ReturnCode == enResponseCodeService.Success && _Resp.ErrorCode == enErrorCode.API_LP_PartialFilled)
                    {
                        NewTradetransaction.APIStatus = "4";
                        BizResponse SettlementResp = await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPI(_Resp, Request.TrnNo, LPProcessTransactionClsObj.RemainingQty, LPProcessTransactionClsObj.SettledQty, Request.Price);
                        if (SettlementResp.ReturnCode == enResponseCodeService.Success && SettlementResp.ErrorCode == enErrorCode.Settlement_PartialSettlementDone)
                        {
                            updateddata.CallStatus = 1;
                        }
                        else
                        {
                            updateddata.CallStatus = 0;
                        }
                        HelperForLog.WriteLogIntoFile("LPStatusCheckSingleHanlder", "status Check hanlder", "LPStatusCheckSingleHanlder " + "##TrnNo:" + Request.TrnNo + "##PROCESSSETLLEMENTAPI Response:" + JsonConvert.SerializeObject(_Resp));

                    }
                    else if (_Resp.ReturnCode == enResponseCodeService.Fail && _Resp.ErrorCode == enErrorCode.API_LP_Cancel)
                    {
                        NewTradetransaction.APIStatus = "5";
                        _transactionQueueCancelOrder.Enqueue(new NewCancelOrderRequestCls()
                        {
                            MemberID = updateddata.MemberID,
                            TranNo = Request.TrnNo,
                            accessToken = "",
                            CancelAll = 0,
                            OrderType = (enTransactionMarketType)Request.Ordertype
                        });
                        Task.Delay(1000).Wait();
                        updateddata.CallStatus = 0;                        
                    }
                    else if (_Resp.ReturnCode == enResponseCodeService.Success && _Resp.ErrorCode == enErrorCode.API_LP_Hold)
                    {
                        NewTradetransaction.APIStatus = "4";
                        updateddata.CallStatus = 0;
                    }
                    else //((_Resp.ReturnCode == enResponseCodeService.Fail && _Resp.ErrorCode == enErrorCode.API_LP_Fail) || (_Resp.ReturnCode == enResponseCodeService.Fail && _Resp.ErrorCode == enErrorCode.API_LP_Order_Not_Found))
                    {
                        NewTradetransaction.APIStatus = "2";
                        updateddata.CallStatus = 0;
                    }
                    _tradeTrnRepositiory.UpdateField(NewTradetransaction, o => o.APIStatus);
                    _trnRepositiory.UpdateField(updateddata, e => e.CallStatus);
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LPStatusCheckSingleHanlder Error:##TrnNo " + Request.TrnNo, ControllerName, ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
}
