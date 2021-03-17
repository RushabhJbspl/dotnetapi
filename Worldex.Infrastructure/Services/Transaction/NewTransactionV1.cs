using Binance.Net.Objects;
using Bittrex.Net.Objects;
using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider1;
using Worldex.Core.ViewModels.WalletOperations;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Worldex.Infrastructure.LiquidityProvider;
using Worldex.Infrastructure.LiquidityProvider.TradeSatoshiAPI;
using CoinbasePro.Services.Orders.Models.Responses;
using CryptoExchange.Net.Objects;
using Huobi.Net.Objects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Infrastructure.LiquidityProvider.OKExAPI;
using Worldex.Core.ViewModels.Wallet;
using Microsoft.Extensions.Caching.Memory;
using CCXT.NET.Kraken.Trade;
using Worldex.Infrastructure.LiquidityProvider.KrakenAPI;
using CCXT.NET.CEXIO.Trade;
using EventBusRabbitMQ.Interfaces;
using Worldex.Infrastructure.LiquidityProvider.EXMO;
using Microsoft.Extensions.Configuration;
using Worldex.Core.ViewModels.Transaction.MarketMaker;
using Worldex.Infrastructure.IntegrationEvents;
using Worldex.Infrastructure.LiquidityProvider.Yobit;

namespace Worldex.Infrastructure.Services.Transaction
{
    public class NewTransactionV1 : ITransactionProcessV1
    {
        private readonly ICommonRepository<TransactionQueue> _TransactionRepository;
        private readonly ICommonRepository<TradeTransactionQueue> _TradeTransactionRepository;
        private readonly ICommonRepository<TradeStopLoss> _TradeStopLoss;
        private readonly ICommonRepository<TradeSellerListV1> _TradeSellerList;
        private readonly ICommonRepository<TradeBuyerListV1> _TradeBuyerList;
        private readonly ICommonRepository<TradePairStastics> _tradePairStastics;
        private readonly IWalletService _WalletService;
        private readonly ILPWalletTransaction _LPWalletTransaction;
        private readonly IWebApiRepository _WebApiRepository;
        private readonly ISettlementRepositoryV1<BizResponse> _SettlementRepositoryV1;
        private readonly ISettlementRepositoryAPI<BizResponse> _SettlementRepositoryAPI;
        private readonly ISignalRService _ISignalRService;
        private readonly ITrnMasterConfiguration _trnMasterConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessageService _messageService;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly IMediator _mediator;
        private readonly ICommonWalletFunction _commonWalletFunction;
        private readonly IFrontTrnRepository _frontTrnRepository; // khushali 24-05-2019 for LTP watcher
        public BizResponse _Resp;
        public TradePairMaster _TradePairObj;
        public TradePairDetail _TradePairDetailObj;
        public List<TransactionProviderResponseV1> TxnProviderList;
        TransactionQueue Newtransaction;
        TradeTransactionQueue NewTradetransaction;
        NewTransactionRequestCls Req;
        NewTradeTransactionRequestCls _TradeTransactionObj = new NewTradeTransactionRequestCls();
        ServiceMaster _BaseCurrService;
        ServiceMaster _SecondCurrService;
        TradeSellerListV1 TradeSellerListObj;
        TradeBuyerListV1 TradeBuyerListObj;
        TradeStopLoss TradeStopLossObj;
        private string ControllerName = "TradingTransactionV1";
        // public IServiceProvider Services { get; }  
        short STOPLimitWithSameLTP = 0;
        //Routing using LP
        private readonly IWebApiData _IWebApiData;
        private readonly IGetWebRequest _IGetWebRequest;
        //private readonly IWebApiSendRequest _IWebApiSendRequest; //komal 03 May 2019, Cleanup
        private readonly ICommonRepository<TransactionRequest> _TransactionRequest;
        private readonly BinanceLPService _binanceLPService;
        //add huobi interface
        private readonly HuobiLPService _huobiLPService;

        private readonly BitrexLPService _bitrexLPService;
        private readonly ICoinBaseService _coinBaseService;
        private readonly IPoloniexService _poloniexService;
        private readonly ITradeSatoshiLPService _tradeSatoshiLPService;
        private readonly IUpbitService _upbitService;
        private readonly IOKExLPService _oKExLPService; //Add new variable for OKEx API by Pushpraj as on 17-06-2019
        private readonly IBitfinexLPService _bitfinexLPService; //Add new variable for Bitfinex Exchange by Pushpraj as on 08-07-2019
        private readonly IGeminiLPService _GeminiLPService;
        private readonly IEXMOLPService _eXMOLPService;
        private readonly IYobitLPService _yobitLPService;//Add new variable for Yobit Exchange by Pushpraj as on 16-07-2019
        private readonly IEventBus _iEventBus;
        private readonly IConfiguration _iConfiguration;
        WebApiParseResponse _WebApiParseResponseObj;
        TransactionRequest NewtransactionRequest;
        ProcessTransactionCls _TransactionObj;
        private readonly IResdisTradingManagment _IResdisTradingManagment;//Rita 15-3-19 added for Site Token conversion
        private readonly ICommonRepository<TradingConfiguration> _tradingConfiguration; // khushali 25-05-2019 Trading configuration Type
        public List<TradingConfiguration> TradingConfigurationList;
        int IsOnlyLocalTradeOn = 0;
        int IsOnlyLiquidtyTradeOn = 0;
        int IsOnlyMarketMakingTradeOn = 0;
        int IsMaxProfit = 0;
        int IsProceedInLocal = 0;
        LPProcessTransactionCls LPProcessTransactionCls;
        private IMemoryCache _cache;
        private readonly IKrakenLPService _krakenLPService;
        private readonly ICEXIOLPService _cEXIOLPService; // Add new variable for CEXIO Exchange by Pushpraj as on 13-07-2019
        int marketMakerUserRole = 0;

        public NewTransactionV1(
            ICommonRepository<TransactionQueue> TransactionRepository,
            ICommonRepository<TradeTransactionQueue> TradeTransactionRepository,
            ICommonRepository<TradeStopLoss> tradeStopLoss, IWalletService WalletService, IWebApiRepository WebApiRepository,
            ICommonRepository<TradeSellerListV1> TradeSellerList, IGeminiLPService geminiLPService,
            ICommonRepository<TradeBuyerListV1> TradeBuyerList, ISettlementRepositoryV1<BizResponse> SettlementRepositoryV1,
            ISignalRService ISignalRService, ICommonRepository<TradePairStastics> tradePairStastics,//IServiceProvider services, 
            ITrnMasterConfiguration trnMasterConfiguration, UserManager<ApplicationUser> userManager, IMessageService messageService,
            IPushNotificationsQueue<SendSMSRequest> pushSMSQueue, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, IMediator mediator,
            ICommonWalletFunction commonWalletFunction, BinanceLPService BinanceLPService, HuobiLPService huobiLPService, BitrexLPService BitrexLPService,
            ICoinBaseService CoinBaseService, IPoloniexService PoloniexService, ITradeSatoshiLPService TradeSatoshiLPService, IUpbitService upbitService,
            IWebApiData IWebApiData, IGetWebRequest IGetWebRequest, IWebApiSendRequest WebApiSendRequest,
            WebApiParseResponse WebApiParseResponseObj, ICommonRepository<TransactionRequest> TransactionRequest,
            IResdisTradingManagment IResdisTradingManagment, IFrontTrnRepository FrontTrnRepository,
            ICommonRepository<TradingConfiguration> TradingConfiguration, IEXMOLPService eXMOLPService,
            ISettlementRepositoryAPI<BizResponse> SettlementRepositoryAPI, IOKExLPService oKExLPService, ILPWalletTransaction LPWalletTransaction, IBitfinexLPService bitfinexLPService,
            IMemoryCache cache, IKrakenLPService krakenLPService, ICEXIOLPService cEXIOLPService, IYobitLPService yobitLPService, IEventBus iEventBus, IConfiguration iConfiguration)
        {
            _TransactionRepository = TransactionRepository;
            _TradeTransactionRepository = TradeTransactionRepository;
            _TradeStopLoss = tradeStopLoss;
            _WalletService = WalletService;
            _WebApiRepository = WebApiRepository;
            _TradeSellerList = TradeSellerList;
            _TradeBuyerList = TradeBuyerList;
            _SettlementRepositoryV1 = SettlementRepositoryV1;
            _ISignalRService = ISignalRService;
            _tradePairStastics = tradePairStastics;
            //Services = services;
            _trnMasterConfiguration = trnMasterConfiguration;
            _userManager = userManager;
            _messageService = messageService;
            _pushSMSQueue = pushSMSQueue;
            _pushNotificationsQueue = pushNotificationsQueue;
            _mediator = mediator;
            _commonWalletFunction = commonWalletFunction;
            // khushali khushali liquidity 
            _IGetWebRequest = IGetWebRequest;
            _IWebApiData = IWebApiData;
            _WebApiParseResponseObj = WebApiParseResponseObj;
            _TransactionRequest = TransactionRequest;
            _binanceLPService = BinanceLPService;
            _huobiLPService = huobiLPService;
            _bitrexLPService = BitrexLPService;
            _coinBaseService = CoinBaseService;
            _poloniexService = PoloniexService;
            _upbitService = upbitService;
            _tradeSatoshiLPService = TradeSatoshiLPService;
            _IResdisTradingManagment = IResdisTradingManagment;
            _frontTrnRepository = FrontTrnRepository;
            _tradingConfiguration = TradingConfiguration;
            _SettlementRepositoryAPI = SettlementRepositoryAPI;
            _oKExLPService = oKExLPService; //Add new for OKEx API by Pushpraj as on 17-06-2019
            _LPWalletTransaction = LPWalletTransaction;
            _cache = cache;
            _bitfinexLPService = bitfinexLPService; //Add new variable Assignment for Bitfinex Exchagne by Pushpraj as on 08-07-2019
            _krakenLPService = krakenLPService;
            _GeminiLPService = geminiLPService;
            _eXMOLPService = eXMOLPService;
            _cEXIOLPService = cEXIOLPService; //Add new variable Assignment for CEIOX Exchagne by Pushpraj as on 13-07-2019
            _yobitLPService = yobitLPService; //Add new variable Assignment for Yobit Exchange by Pushpraj as on 16-07-2019
            _iEventBus = iEventBus; //add for publish market maker event -Sahil 16-10-2019 05:50 PM
            _iConfiguration = iConfiguration;
        }

        public async Task<BizResponse> ProcessNewTransactionAsync(NewTransactionRequestCls Req1)
        {
            //_SettlementRepositoryV1.Callsp_TradeSettlement(3518, 3518,10,10,10,13,2);
            //int ss = 1;
            //if (ss == 1)
            //    return new BizResponse();
            Req = Req1;

            //Rushabh 19-07-2019 Stop Price Logic, For Buy order make stop price 3% Up and for Sale order make stop price 3% Down
            #region Stop Price Logic
            //decimal CalculatedPrice = 0;
            //CalculatedPrice = Convert.ToDecimal(Req.Price * 3 / 100);
            //if (Req.TrnType == enTrnType.Buy_Trade)
            //{                
            //    Req.StopPrice = Req.Price + CalculatedPrice;
            //}
            //else if(Req.TrnType == enTrnType.Sell_Trade)
            //{             
            //    Req.StopPrice = Req.Price - CalculatedPrice;
            //}
            #endregion

            _Resp = await CreateTransaction();
            if (_Resp.ReturnCode != enResponseCodeService.Success)
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessNewTransactionAsync", ControllerName, _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo + " GUID:" + Req.GUID, Helpers.UTC_To_IST()));
                return _Resp;
            }
            _Resp = await CombineAllInitTransactionAsync();

            return _Resp;
        }

        public async Task<BizResponse> CombineAllInitTransactionAsync()
        {
            _Resp = new BizResponse();
            try
            {
                //Deduct balance here
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync Wallet", ControllerName, "Balance Deduction Start " + Req.GUID + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));

                var DebitResult1 = _WalletService.GetWalletHoldNew(Req.SMSCode, Helpers.GetTimeStamp(), Req.Amount,
                    Req.DebitAccountID, Req.TrnNo, enServiceType.Trading, Req.TrnType == enTrnType.Buy_Trade ? enWalletTrnType.BuyTrade : enWalletTrnType.SellTrade, Req.TrnType, (EnAllowedChannels)Req.TrnMode, Req.accessToken, (enWalletDeductionType)((short)Req.ordertype), (Req.TrnType == enTrnType.Buy_Trade ? NewTradetransaction.Order_Currency : NewTradetransaction.Delivery_Currency)); //NTRIVEDI 07-12-2018    //2019-9-17 add market currency in request as per worldex proj requirement           

                //TradingDataInsert(_Resp);
                WalletDrCrResponse DebitResult = await DebitResult1;
                //2019-4-29 addedd charge 
                Newtransaction.ChargeRs = DebitResult.Charge;
                Newtransaction.ChargeCurrency = DebitResult.ChargeCurrency;
                //var DebitResult = await DebitResults;
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync Wallet", ControllerName, "Deduction End" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                if (DebitResult.ReturnCode != enResponseCode.Success)
                {
                    _Resp.ReturnMsg = DebitResult.ReturnMsg;//EnResponseMessage.ProcessTrn_WalletDebitFailMsg;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = DebitResult.ErrorCode;//enErrorCode.ProcessTrn_WalletDebitFail;
                    MarkTransactionSystemFail(_Resp.ReturnMsg, _Resp.ErrorCode);
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "Balance Deduction Fail" + _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                    return _Resp;
                }
                //===================================Make txn HOLD as balance debited=======================
                //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "Update Service Start" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));

                //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "Trading Data Entry Done " + _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "End Service" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "HOLD Start" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                MarkTransactionHold(EnResponseMessage.ProcessTrn_HoldMsg, enErrorCode.ProcessTrn_Hold);//Rita 4-3-19 remove task.run for speed execution status not update as settlement reverse

                //Rita 8-1-19 for followers trading
                if (Req.ISFollowersReq == 0 && Req.FollowingTo == 0 && Req.LeaderTrnNo == 0)//only for leader's allow this
                {
                    FollowersOrderRequestCls request = new FollowersOrderRequestCls { Req = Req, Delivery_Currency = _TradeTransactionObj.Delivery_Currency, Order_Currency = _TradeTransactionObj.Order_Currency };
                    Task.Run(() => _mediator.Send(request));
                }
                //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "Trading Data Entry Start " + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));

                //Rita 19-3-19 save to Redis cache,as live remove this
                //await _IResdisTradingManagment.TransactionOrderCacheEntry(_Resp, Req.TrnNo, Req.PairID, _TradeTransactionObj.PairName,
                //    Req.Price, Req.Qty, Req.Qty, Convert.ToInt16(Req.ordertype), _TradeTransactionObj.TrnTypeName,0);

                //=====================For Routing Enable-uncomment TradingConfiguration(), and comment below code block===================================================                              
                if (Req.TrnType == enTrnType.Buy_Trade)
                {
                    TradeBuyerListObj.TrnNo = Req.TrnNo;
                    TradeBuyerListObj = _TradeBuyerList.Add(TradeBuyerListObj);
                }
                else
                {
                    TradeSellerListObj.TrnNo = Req.TrnNo;
                    TradeSellerListObj = _TradeSellerList.Add(TradeSellerListObj);
                }

                //IsOnlyLocalTradeOn == 1 && IsOnlyMarketMakingTradeOn==0
                //Rita 25-5-19 If max profit=1 means off then check if provider or route not found then goes in local ,then shoul not go in route
                //Static Set IsMaxProfit = 1 for testing Please remove while live run
                //IsMaxProfit = 1;

                if (IsMaxProfit == 0 || TxnProviderList?.Count == 0 || (IsOnlyLiquidtyTradeOn == 0 && IsOnlyMarketMakingTradeOn == 0))
                {
                    //Rita 3 - 4 - 19 after routing add move this in TradingDataInsertV2
                    if (Req.TrnType == enTrnType.Buy_Trade)
                    {
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "END Buyer" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                        if (Req.ordertype != enTransactionMarketType.STOP_Limit || STOPLimitWithSameLTP == 1)
                        {
                            _Resp = await _SettlementRepositoryV1.PROCESSSETLLEMENTBuy(_Resp, NewTradetransaction, Newtransaction, TradeStopLossObj, TradeBuyerListObj, Req.accessToken, 0);
                        }
                    }
                    else
                    {
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "END Seller" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                        if (Req.ordertype != enTransactionMarketType.STOP_Limit || STOPLimitWithSameLTP == 1)
                        {
                            _Resp = await _SettlementRepositoryV1.PROCESSSETLLEMENTSell(_Resp, NewTradetransaction, Newtransaction, TradeStopLossObj, TradeSellerListObj, Req.accessToken, 0);
                        }
                    }


                    //if (IsOnlyMarketMakingTradeOn == 1 && _Resp.ErrorCode == enErrorCode.Settlement_FullSettlementDone && Req.MemberID == marketMakerUserRole)
                    //{
                    //    PlaceMarketMakerCounterTrade();
                    //    var TradeList = _frontTrnRepository.GetMarketMakerSettledByTakerList(NewTradetransaction.TrnNo, marketMakerUserRole);
                    //    if (TradeList.Count > 0) //case for both market maker trade
                    //        PlaceMarketMakerCounterTradeAsTaker(marketMakerUserRole);
                    //}
                    //else if (IsOnlyMarketMakingTradeOn == 1 && _Resp.ErrorCode == enErrorCode.Settlement_FullSettlementDone)
                    //{
                    //    PlaceMarketMakerCounterTradeAsTaker(marketMakerUserRole);
                    //}

                    //==============================================================================================
                    //if (IsOnlyMarketMakingTradeOn == 1 && _Resp.ErrorCode == enErrorCode.Settlement_FullSettlementDone && Req.MemberID == marketMakerUserRole)
                    //{
                    // PlaceMarketMakerCounterTrade();
                    // var TradeList = _frontTrnRepository.GetMarketMakerSettledByTakerList(NewTradetransaction.TrnNo, marketMakerUserRole);
                    // if (TradeList.Count > 0) //case for both market maker trade
                    // PlaceMarketMakerCounterTradeAsTaker(marketMakerUserRole);
                    //}
                    //else if (IsOnlyMarketMakingTradeOn == 1 && _Resp.ErrorCode == enErrorCode.Settlement_FullSettlementDone)
                    //{
                    // PlaceMarketMakerCounterTradeAsTaker(marketMakerUserRole);
                    //}

                    //rita 26-08-2020 for looping trade issue , and remove counter trade for marketmaker's as maker and as takers settled(both order of market maker)
                    //here Req.MemberID == marketMakerUserRole , for MM as taker , so place counter order only one time
                    if (IsOnlyMarketMakingTradeOn == 1 && _Resp.ErrorCode == enErrorCode.Settlement_FullSettlementDone && Req.MemberID == marketMakerUserRole)
                    {
                        await PlaceMarketMakerCounterTrade();
                        //var TradeList = _frontTrnRepository.GetMarketMakerSettledByTakerList(NewTradetransaction.TrnNo, marketMakerUserRole);
                        //if (TradeList.Count > 0) //case for both market maker trade
                        // PlaceMarketMakerCounterTradeAsTaker(marketMakerUserRole);
                    }
                    //else if (IsOnlyMarketMakingTradeOn == 1 && _Resp.ErrorCode == enErrorCode.Settlement_FullSettlementDone)
                    //{
                    // PlaceMarketMakerCounterTradeAsTaker(marketMakerUserRole);
                    //}
                    //==============================================================================================

                    IsProceedInLocal = 1;//then does not proceed in routing
                }

                if ((IsOnlyLiquidtyTradeOn == 1 || IsOnlyMarketMakingTradeOn == 1) &&
                    (_Resp.ReturnCode != enResponseCodeService.Success || IsProceedInLocal == 0) &&
                    (TxnProviderList.Count != 0 || IsOnlyMarketMakingTradeOn == 1)) //add OR for condition market making is on Trn list not necessary -Sahil 16-10-2019 04:53 PM
                {
                    //Rita 6-3-19 for trade routing
                    await TradingConfiguration(_Resp);
                }
                ////////if (IsOnlyMarketMakingTradeOn == 1 || (IsOnlyLiquidtyTradeOn==1 && TxnProviderList.Count == 0 && _Resp.ReturnCode != enResponseCodeService.Success))//TxnProviderList not checked for IsOnlyLiquidtyTradeOn=1
                //////if (((IsOnlyLiquidtyTradeOn==1 || IsOnlyMarketMakingTradeOn == 1) && IsMaxProfit == 1) || 
                //////    (TxnProviderList.Count != 0 && _Resp.ReturnCode != enResponseCodeService.Success && (IsOnlyLiquidtyTradeOn == 1 || IsOnlyMarketMakingTradeOn == 1)))//TxnProviderList not checked for IsOnlyLiquidtyTradeOn=1
                //////{
                //////    //Rita 6-3-19 for trade routing
                //////    await TradingConfiguration(_Resp);
                //////}   

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync Settlement END Now wait for 20 sec", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
                Task.Delay(10000).Wait();//rita 3-1-19 wait for all operations done //Uday 15-01-2019 change from 10000 to 20000 //rita 27-2-19 change from 20000 to 10000

                return _Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CombineAllInitTransactionAsync Internal Error:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionProcessInternalError });
            }
        }

        #region RegionInitTransaction    
        public async Task<BizResponse> CreateTransaction()
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Transaction Process For" + Req.TrnType + "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));
                _TradePairObj = _trnMasterConfiguration.GetTradePairMaster().Where(item => item.Id == Req.PairID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (_TradePairObj == null)
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnNoPairSelectedMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrn_NoPairSelected);
                }
                var pairStasticsResult = _tradePairStastics.GetSingleAsync(pair => pair.PairId == Req.PairID);
                _TradeTransactionObj.PairName = _TradePairObj.PairName;
                var LoadDataResult = LoadAllMasterDataParaller();
                //place same incoming order for market maker -Sahil 10-10-2019 07:16 PM, komal 21-11-2019 find market maker
                marketMakerUserRole = _frontTrnRepository.GetMarketMakerUserRole();
                //var GetWalletIDResult1 = _WalletService.GetWalletID(Req.DebitAccountID);
                if (Req.TrnType == enTrnType.Buy_Trade)
                {
                    _TradeTransactionObj.TrnTypeName = "BUY";
                }
                else
                {
                    _TradeTransactionObj.TrnTypeName = "SELL";
                }
                //if (Convert.ToInt16(Req.ordertype) == 3) //komal remove SPOT order trading type
                //{
                //    Req.StatusMsg = EnResponseMessage.OrderTypeNotAvailable;
                //    return MarkSystemFailTransaction(enErrorCode.OrderTypeNotAvailable);
                //}
                if (Convert.ToInt16(Req.ordertype) < 1 || Convert.ToInt16(Req.ordertype) > 5)
                {
                    Req.StatusMsg = EnResponseMessage.InValidOrderTypeMsg;
                    return MarkSystemFailTransaction(enErrorCode.InValidOrderType);
                }
                //var GetWalletIDResult2 = _WalletService.GetWalletID(Req.CreditAccountID);

                //Req.DebitWalletID = await GetWalletIDResult1;
                //if (Req.DebitWalletID == 0)
                //{
                //    Req.StatusMsg = EnResponseMessage.InValidDebitAccountIDMsg;
                //    return MarkSystemFailTransaction(enErrorCode.InValidDebitAccountID);
                //}
                //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Debit WalletID" + Req.DebitWalletID + "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));
                var pairStastics = await pairStasticsResult;
                //IF @PairID <> 0 ntrivedi 18-04-2018  check inside @TrnType (4,5) @TradeWalletMasterID will be 0 or null
                if (Req.ordertype == enTransactionMarketType.MARKET)
                {
                    // var pairStastics =await _tradePairStastics.GetSingleAsync(pair => pair.PairId == Req.PairID);                   
                    Req.Price = pairStastics.LTP;
                }
                //_TradeTransactionObj.OrderWalletID = Req.DebitWalletID;

                //Req.CreditWalletID = await GetWalletIDResult2;
                //if (Req.CreditWalletID == 0)
                //{
                //    Req.StatusMsg = EnResponseMessage.InValidCreditAccountIDMsg;
                //    return MarkSystemFailTransaction(enErrorCode.InValidCreditAccountID);
                //}
                //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Credit WalletID" + Req.CreditWalletID + "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));

                //_TradeTransactionObj.DeliveryWalletID = Req.CreditWalletID;


                //_TradePairObj = _trnMasterConfiguration.GetTradePairMaster().Where(item => item.Id == Req.PairID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                //if (_TradePairObj == null)
                //{
                //    Req.StatusMsg = EnResponseMessage.CreateTrnNoPairSelectedMsg;
                //    return MarkSystemFailTransaction(enErrorCode.CreateTrn_NoPairSelected);
                //}
                //_TradeTransactionObj.PairName = _TradePairObj.PairName;
                if (Req.Qty <= 0 || (Req.Price <= 0 && Req.ordertype != enTransactionMarketType.MARKET) || (Req.StopPrice == 0 && Req.ordertype == enTransactionMarketType.STOP_Limit))
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnInvalidQtyPriceMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrnInvalidQtyPrice);
                }
                await LoadDataResult;
                //_BaseCurrService = _trnMasterConfiguration.GetServices().Where(item => item.Id == _TradePairObj.BaseCurrencyId).FirstOrDefault();
                //_SecondCurrService = _trnMasterConfiguration.GetServices().Where(item => item.Id == _TradePairObj.SecondaryCurrencyId).FirstOrDefault();

                //_TradePairDetailObj = _trnMasterConfiguration.GetTradePairDetail().Where(item => item.PairId == Req.PairID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (_TradePairDetailObj == null)
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnNoPairSelectedMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrn_NoPairSelected);
                }
                if (Req.TrnType == enTrnType.Buy_Trade)
                {
                    _TradeTransactionObj.BuyQty = Req.Qty;
                    _TradeTransactionObj.BidPrice = Req.ordertype == enTransactionMarketType.MARKET ? 0 : Req.Price;
                    var AssRes = AssignDataBuy();
                    //_TradeTransactionObj.DeliveryTotalQty = Req.Qty;
                    //_TradeTransactionObj.OrderTotalQty = Helpers.DoRoundForTrading(Req.Qty * Req.Price, 8);//235.415001286,8 =  235.41500129                         
                    //_TradeTransactionObj.Order_Currency = _BaseCurrService.SMSCode;
                    //_TradeTransactionObj.Delivery_Currency = _SecondCurrService.SMSCode;
                    //Req.SMSCode = _TradeTransactionObj.Order_Currency;
                    //Req.Amount = _TradeTransactionObj.OrderTotalQty;
                    if (_TradeTransactionObj.BuyQty < _TradePairDetailObj.BuyMinQty || _TradeTransactionObj.BuyQty > _TradePairDetailObj.BuyMaxQty)
                    {
                        Req.StatusMsg = EnResponseMessage.ProcessTrn_AmountBetweenMinMaxMsg.Replace("@MIN", _TradePairDetailObj.BuyMinQty.ToString()).Replace("@MAX", _TradePairDetailObj.BuyMaxQty.ToString());
                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_AmountBetweenMinMax, _TradePairDetailObj.BuyMinQty.ToString(), _TradePairDetailObj.BuyMaxQty.ToString());
                    }
                    if ((_TradeTransactionObj.BidPrice < _TradePairDetailObj.BuyMinPrice || _TradeTransactionObj.BidPrice > _TradePairDetailObj.BuyMaxPrice) && Req.ordertype != enTransactionMarketType.MARKET)
                    {
                        Req.StatusMsg = EnResponseMessage.ProcessTrn_PriceBetweenMinMaxMsg.Replace("@MIN", _TradePairDetailObj.BuyMinPrice.ToString()).Replace("@MAX", _TradePairDetailObj.BuyMaxPrice.ToString());
                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_PriceBetweenMinMax, _TradePairDetailObj.BuyMinPrice.ToString(), _TradePairDetailObj.BuyMaxPrice.ToString());
                    }
                    await AssRes;
                    InsertBuyerList();
                }
                else if (Req.TrnType == enTrnType.Sell_Trade)
                {
                    _TradeTransactionObj.SellQty = Req.Qty;
                    _TradeTransactionObj.AskPrice = Req.ordertype == enTransactionMarketType.MARKET ? 0 : Req.Price;
                    var AssRes = AssignDataSell();
                    //_TradeTransactionObj.OrderTotalQty = Req.Qty;
                    //_TradeTransactionObj.DeliveryTotalQty = Helpers.DoRoundForTrading(Req.Qty * Req.Price, 8);//235.415001286,8 =  235.41500129                        
                    //_TradeTransactionObj.Order_Currency = _SecondCurrService.SMSCode;
                    //_TradeTransactionObj.Delivery_Currency = _BaseCurrService.SMSCode;
                    //Req.SMSCode = _TradeTransactionObj.Order_Currency;
                    //Req.Amount = _TradeTransactionObj.OrderTotalQty;
                    if (_TradeTransactionObj.SellQty < _TradePairDetailObj.SellMinQty || _TradeTransactionObj.SellQty > _TradePairDetailObj.SellMaxQty)
                    {
                        Req.StatusMsg = EnResponseMessage.ProcessTrn_AmountBetweenMinMaxMsg.Replace("@MIN", _TradePairDetailObj.SellMinQty.ToString()).Replace("@MAX", _TradePairDetailObj.SellMaxQty.ToString());
                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_AmountBetweenMinMax, _TradePairDetailObj.SellMinQty.ToString(), _TradePairDetailObj.SellMaxQty.ToString());
                    }
                    if ((_TradeTransactionObj.AskPrice < _TradePairDetailObj.SellMinPrice || _TradeTransactionObj.AskPrice > _TradePairDetailObj.SellMaxPrice) && Req.ordertype != enTransactionMarketType.MARKET)
                    {
                        Req.StatusMsg = EnResponseMessage.ProcessTrn_PriceBetweenMinMaxMsg.Replace("@MIN", _TradePairDetailObj.SellMinPrice.ToString()).Replace("@MAX", _TradePairDetailObj.SellMaxPrice.ToString());
                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_PriceBetweenMinMax, _TradePairDetailObj.SellMinPrice.ToString(), _TradePairDetailObj.SellMaxPrice.ToString());
                    }
                    await AssRes;
                    InsertSellerList();
                }

                //komal 30-9-2019 Validate user wallet
                var OrderWalletDetail = _WalletService.GetTransactionWalletByCoin(Req.MemberID, _TradeTransactionObj.Order_Currency);
                var DeliveryWalletDetail = await _WalletService.GetTransactionWalletByCoin(Req.MemberID, _TradeTransactionObj.Delivery_Currency);
                if (DeliveryWalletDetail.ReturnCode != enResponseCode.Success)
                {
                    Req.StatusMsg = EnResponseMessage.InValidCreditAccountIDMsg;
                    return MarkSystemFailTransaction(enErrorCode.InValidCreditAccountID);
                }
                Req.CreditAccountID = DeliveryWalletDetail.Wallet.AccWalletID;
                Req.CreditWalletID = DeliveryWalletDetail.Wallet.id;
                _TradeTransactionObj.DeliveryWalletID = DeliveryWalletDetail.Wallet.id;
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Credit WalletID" + Req.CreditWalletID + "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));

                var OrderWalletDetail2 = OrderWalletDetail.GetAwaiter().GetResult();
                if (OrderWalletDetail2.ReturnCode != enResponseCode.Success)
                {
                    Req.StatusMsg = EnResponseMessage.InValidDebitAccountIDMsg;
                    return MarkSystemFailTransaction(enErrorCode.InValidDebitAccountID);
                }
                Req.DebitAccountID = OrderWalletDetail2.Wallet.AccWalletID;
                Req.DebitWalletID = OrderWalletDetail2.Wallet.id;
                _TradeTransactionObj.OrderWalletID = OrderWalletDetail2.Wallet.id;
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Debit WalletID" + Req.DebitWalletID + "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));

                //Rita 28-9-19 if same Wallet id then fail order
                if (_TradeTransactionObj.DeliveryWalletID == _TradeTransactionObj.OrderWalletID)
                {
                    Req.StatusMsg = EnResponseMessage.InValidCreditAccountIDMsg + "..";
                    return MarkSystemFailTransaction(enErrorCode.InValidCreditAccountID);
                }





                decimal RoundedPrice = Math.Round(Req.Price, _TradePairDetailObj.PriceLength);
                decimal RoundedQty = Math.Round(Req.Qty, _TradePairDetailObj.QtyLength);
                decimal RoundedAmt = Math.Round(Req.Qty * Req.Price, _TradePairDetailObj.AmtLength);

                if (RoundedQty != Req.Qty && _TradePairDetailObj.QtyLength != 0)//rita 29-7-19 if 0 then do not check
                {
                    Req.StatusMsg = EnResponseMessage.ProcessTrn_QtyBadPrecision;
                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_QtyBadPrecision);
                }
                else if (RoundedPrice != Req.Price && _TradePairDetailObj.PriceLength != 0)//rita 29-7-19 if 0 then do not check
                {
                    Req.StatusMsg = EnResponseMessage.ProcessTrn_PriceBadPrecision;
                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_PriceBadPrecision);
                }
                else if (RoundedAmt != Req.Qty * Req.Price && _TradePairDetailObj.AmtLength != 0)//rita 29-7-19 if 0 then do not check
                {
                    Req.StatusMsg = EnResponseMessage.ProcessTrn_AmtBadPrecision;
                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_NotionalBadPrecision);
                }

                //Rita 10-8-19 for total Qty validation
                if (_TradePairDetailObj.MinNotional > Req.Price * Req.Qty || _TradePairDetailObj.MaxNotional < Req.Price * Req.Qty)
                {
                    Req.StatusMsg = EnResponseMessage.ProcessTrn_TotalFailure.Replace("@MIN", _TradePairDetailObj.MinNotional.ToString()).Replace("@MAX", _TradePairDetailObj.MaxNotional.ToString());
                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_TotalFailure, _TradePairDetailObj.MinNotional.ToString(), _TradePairDetailObj.MaxNotional.ToString());
                }
                //var GetProListResult = _WebApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest {amount = Req.Amount,SMSCode = Req.SMSCode, APIType = enWebAPIRouteType.TransactionAPI,
                //                        trnType = Req.TrnType == enTrnType.Sell_Trade ? Convert.ToInt32(enTrnType.Buy_Trade) : Convert.ToInt32(Req.TrnType) });
                //Rita 6-3-19 for trade routing
                var GetProListResult = _WebApiRepository.GetProviderDataListV2Async(new TransactionApiConfigurationRequest
                {
                    amount = Req.Qty,//Req.Amount Rita 4-4-19 pass Qty , as base on Qty routing place
                    SMSCode = Req.SMSCode,
                    APIType = enWebAPIRouteType.TransactionAPI,
                    trnType = Req.TrnType == enTrnType.Sell_Trade ? Convert.ToInt32(enTrnType.Sell_Trade) : Convert.ToInt32(enTrnType.Buy_Trade),
                    OrderType = Convert.ToInt16(Req.ordertype),
                    PairID = Req.PairID
                });

                //var walletLimit = _commonWalletFunction.CheckWalletLimitAsync(enWalletLimitType.TradingLimit, Req.DebitWalletID, Req.Amount);
                var walletLimit = _commonWalletFunction.CheckWalletLimitAsyncV1(enWalletLimitType.TradingLimit, Req.DebitWalletID, Req.Amount);

                if (_TradeTransactionObj.OrderTotalQty < (decimal)(0.000000000000000001) || _TradeTransactionObj.DeliveryTotalQty < (decimal)(0.000000000000000001))
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnInvalidQtyNAmountMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrnInvalidQtyNAmount);
                }
                if (Req.Amount <= 0) // ntrivedi 02-11-2018 if amount =0 then also invalid
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnInvalidAmountMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrnInvalidAmount);
                }

                //enErrorCode WalletLimitRes= await walletLimit;//komal 25-01-2019 check transaction limit
                //Rushabh 06-02-2019
                WalletTrnLimitResponse WalletLimitRes = await walletLimit;//komal 25-01-2019 check transaction limit
                if (WalletLimitRes.ErrorCode != enErrorCode.Success)
                {
                    //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Debit WalletID" + Req.DebitWalletID + "##TrnNo:" + Req.GUID+ "check transaction limit : " + WalletLimitRes.ToString(), Helpers.UTC_To_IST()));
                    Req.StatusMsg = WalletLimitRes.ReturnMsg.ToString();
                    //rita 26-6-19 added min/max value from wallet
                    return MarkSystemFailTransaction(WalletLimitRes.ErrorCode, WalletLimitRes.MinimumAmounts, WalletLimitRes.MaximumAmounts);//2019-3-7 add new param for notification msg
                }
                //khushali 25-5-19 check all types of Trading bit
                TradingConfigurationList = _tradingConfiguration.FindBy(e => e.Status == 1).ToList();
                foreach (var TradeConfig in TradingConfigurationList)
                {
                    if (TradeConfig.Name == enTradingType.Regular.ToString())
                    {
                        IsOnlyLocalTradeOn = 1;
                    }
                    else if (TradeConfig.Name == enTradingType.Liquidity.ToString())
                    {
                        IsOnlyLiquidtyTradeOn = 1;
                    }
                    else if (TradeConfig.Name == enTradingType.MarketMaking.ToString())
                    {
                        IsOnlyMarketMakingTradeOn = 1;
                    }
                    else if (TradeConfig.Name == enTradingType.MaxProfit.ToString())
                    {
                        IsMaxProfit = 1;
                    }
                }
                if (IsOnlyLocalTradeOn == 0)//Rita 25-5-19 if bit not set then make Txn fail
                {
                    Req.StatusMsg = "Transaction is currenctly stopped";
                    return MarkSystemFailTransaction(enErrorCode.CreateTxnTradingIsStopped);
                }

                TxnProviderList = await GetProListResult;
                //rita 25-5-19 remove-as if route not present then trading place in local without below error

                //if ((IsOnlyLiquidtyTradeOn == 1 || IsOnlyMarketMakingTradeOn==1) && IsMaxProfit==1)//if MaxProfit on then N then check routing
                //{
                //    TxnProviderList = await GetProListResult;
                //    if (TxnProviderList.Count == 0) //Uday 05-11-2018 check condition for no record
                //    {
                //        Req.StatusMsg = EnResponseMessage.ProcessTrn_ServiceProductNotAvailableMsg;
                //        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_ServiceProductNotAvailable);
                //    }

                //    var AllRoutExist = TxnProviderList.Where(e => e.ProTypeID == Convert.ToInt64(enWebAPIRouteType.TradeServiceLocal) ||
                //                                                e.ProTypeID == Convert.ToInt64(enWebAPIRouteType.LiquidityProvider)).ToList();
                //    if (AllRoutExist.Count == 0) //Rita 4-4-19 if this type not exist then make fail
                //    {
                //        Req.StatusMsg = EnResponseMessage.ProcessTrn_ServiceProductNotAvailableMsg;
                //        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_RouteTypeDefinedWrong);
                //    }
                //}

                Req.Status = enTransactionStatus.Initialize;
                Req.StatusCode = Convert.ToInt64(enErrorCode.TransactionInsertSuccess);
                await InsertTransactionInQueue();
                await InsertTradeTransactionInQueue();
                await InsertTradeStopLoss(pairStastics.LTP);
                return new BizResponse { ReturnMsg = "", ReturnCode = enResponseCodeService.Success };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateTransaction:##TrnNo " + Req.TrnNo + " GUID:" + Req.GUID, ControllerName, ex);
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionInsertInternalError });
            }

        }
        public async Task AssignDataBuy()
        {
            try
            {
                _TradeTransactionObj.DeliveryTotalQty = Req.Qty;
                _TradeTransactionObj.OrderTotalQty = Helpers.DoRoundForTrading(Req.Qty * Req.Price, 18);//235.415001286,8 =  235.41500129                         
                _TradeTransactionObj.Order_Currency = _BaseCurrService.SMSCode;
                _TradeTransactionObj.Delivery_Currency = _SecondCurrService.SMSCode;
                Req.SMSCode = _TradeTransactionObj.Order_Currency;
                Req.Amount = _TradeTransactionObj.OrderTotalQty;
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("AssignDataBuy:##TrnNo " + Req.TrnNo, ControllerName, ex));
                throw ex;
            }
        }
        public async Task AssignDataSell()
        {
            try
            {
                _TradeTransactionObj.OrderTotalQty = Req.Qty;
                _TradeTransactionObj.DeliveryTotalQty = Helpers.DoRoundForTrading(Req.Qty * Req.Price, 18);//235.415001286,8 =  235.41500129                        
                _TradeTransactionObj.Order_Currency = _SecondCurrService.SMSCode;
                _TradeTransactionObj.Delivery_Currency = _BaseCurrService.SMSCode;
                Req.SMSCode = _TradeTransactionObj.Order_Currency;
                Req.Amount = _TradeTransactionObj.OrderTotalQty;
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("AssignDataBuy:##TrnNo " + Req.TrnNo, ControllerName, ex));
                throw ex;
            }
        }
        public async Task LoadAllMasterDataParaller()
        {
            try
            {
                _BaseCurrService = _trnMasterConfiguration.GetServices().Where(item => item.Id == _TradePairObj.BaseCurrencyId).FirstOrDefault();
                _SecondCurrService = _trnMasterConfiguration.GetServices().Where(item => item.Id == _TradePairObj.SecondaryCurrencyId).FirstOrDefault();
                _TradePairDetailObj = _trnMasterConfiguration.GetTradePairDetail().Where(item => item.PairId == Req.PairID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LoadAllMasterDataParaller:##TrnNo " + Req.TrnNo, ControllerName, ex);
                throw ex;
            }
        }
        public BizResponse MarkSystemFailTransaction(enErrorCode ErrorCode, string Param2 = "", string Param3 = "")
        {
            try
            {
                Req.Status = enTransactionStatus.SystemFail;
                Req.StatusCode = Convert.ToInt64(ErrorCode);
                InsertTransactionInQueue();
                InsertTradeStopLoss(0);
                try//as some para null in starting so error occured here ,only in case of system fail
                {
                    InsertTradeTransactionInQueue();
                }
                catch (Exception ex)
                {
                    Task.Run(() => HelperForLog.WriteErrorLog("MarkSystemFailTransaction Trade TQ Error:##TrnNo " + Req.TrnNo, ControllerName, ex));
                }
                if (Newtransaction.MemberID != marketMakerUserRole) //komal 21-11-2019 do not send email, SMS, Notification to market maker
                {
                    //DI of SMS here
                    //Uday 06-12-2018  Send SMS When Transaction is Failed
                    SMSSendTransactionHoldOrFailed(Newtransaction.Id, Newtransaction.MemberMobile, Req.Price, Req.Qty, 2);

                    //Uday 06-12-2018  Send Email When Transaction is Failed
                    EmailSendTransactionHoldOrFailed(Newtransaction.Id, Newtransaction.MemberID + "", Req.PairID, Req.Qty, Newtransaction.TrnDate + "", Req.Price, 0, 2, Convert.ToInt16(Req.ordertype), Convert.ToInt16(Req.TrnType));

                    try
                    {
                        //Rita 26-11-2018 add Activity Notifiation v2
                        ActivityNotificationMessage notification = new ActivityNotificationMessage();
                        //notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);
                        notification.MsgCode = Convert.ToInt32(ErrorCode); //komal 05-02-2019 set validation error code
                        notification.Param1 = Req.TrnNo.ToString();
                        notification.Param2 = Param2;
                        notification.Param3 = Param3;
                        notification.Param4 = "USD";
                        notification.Type = Convert.ToInt16(EnNotificationType.Fail);
                        _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2);//Req.accessToken
                                                                                                              //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("MarkSystemFailTransaction Notification Send " + notification.MsgCode, ControllerName, "##TrnNo:" + Newtransaction.Id, Helpers.UTC_To_IST()));
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() => HelperForLog.WriteErrorLog("ISignalRService Notification Error-MarkSystemFailTransaction ##TrnNo:" + Newtransaction.Id, ControllerName, ex));
                    }
                }
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.Fail, ErrorCode = ErrorCode });
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("MarkSystemFailTransaction:##TrnNo " + Req.TrnNo, ControllerName, ex));
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionInsertInternalError });
            }
        }
        public async Task InsertTransactionInQueue()//ref long TrnNo
        {
            try
            {
                Newtransaction = new TransactionQueue()
                {
                    TrnDate = Helpers.UTC_To_IST(),
                    //GUID = Guid.NewGuid(),
                    GUID = Req.GUID,
                    TrnMode = Req.TrnMode,
                    TrnType = Convert.ToInt16(Req.TrnType),
                    MemberID = Req.MemberID,
                    MemberMobile = Req.MemberMobile,
                    TransactionAccount = Req.TransactionAccount,
                    SMSCode = Req.SMSCode,
                    Amount = Req.Amount,
                    Status = Convert.ToInt16(Req.Status),
                    StatusCode = Req.StatusCode,
                    StatusMsg = Req.StatusMsg,
                    TrnRefNo = Req.TrnRefNo,
                    AdditionalInfo = Req.AdditionalInfo,
                    DebitAccountID = Req.DebitAccountID,//rita 03-12-18 added as required in withdraw process
                    CallStatus = 1
                };
                Newtransaction = _TransactionRepository.Add(Newtransaction);
                Req.TrnNo = Newtransaction.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertTransactionInQueue:##TrnNo " + Req.TrnNo, ControllerName, ex);
                throw ex;
            }

        }
        public async Task InsertTradeTransactionInQueue()
        {
            try
            {
                NewTradetransaction = new TradeTransactionQueue()
                {
                    TrnDate = Helpers.UTC_To_IST(),
                    TrnType = Convert.ToInt16(Req.TrnType),
                    TrnTypeName = _TradeTransactionObj.TrnTypeName,
                    MemberID = Req.MemberID,
                    PairID = Req.PairID,
                    PairName = _TradeTransactionObj.PairName,
                    OrderWalletID = _TradeTransactionObj.OrderWalletID,
                    DeliveryWalletID = _TradeTransactionObj.DeliveryWalletID,
                    OrderAccountID = Req.DebitAccountID,
                    DeliveryAccountID = Req.CreditAccountID,
                    BuyQty = _TradeTransactionObj.BuyQty,
                    BidPrice = _TradeTransactionObj.BidPrice,
                    SellQty = _TradeTransactionObj.SellQty,
                    AskPrice = _TradeTransactionObj.AskPrice,
                    Order_Currency = _TradeTransactionObj.Order_Currency,
                    OrderTotalQty = _TradeTransactionObj.OrderTotalQty,
                    Delivery_Currency = _TradeTransactionObj.Delivery_Currency,
                    DeliveryTotalQty = _TradeTransactionObj.DeliveryTotalQty,
                    SettledBuyQty = _TradeTransactionObj.SettledBuyQty,
                    SettledSellQty = _TradeTransactionObj.SettledSellQty,
                    Status = Convert.ToInt16(Req.Status),
                    StatusCode = Req.StatusCode,
                    StatusMsg = Req.StatusMsg,
                    ordertype = Convert.ToInt16(Req.ordertype),
                    TrnNo = Req.TrnNo,//NewTradetransactionReq.TrnNo,
                    IsAPITrade = 0,
                    IsExpired = 0,//Rita 30-1-19 for API level changes
                    APIStatus = "",
                    IsAPICancelled = 0,
                    IsPrivateAPITarde = Req.IsPrivateAPITrade //komal 3-10-2019 for private api trade
                };
                NewTradetransaction = _TradeTransactionRepository.Add(NewTradetransaction);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertTradeTransactionInQueue:##TrnNo " + Req.TrnNo, ControllerName, ex);
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
                throw ex;
            }

        }
        public async Task InsertTradeStopLoss(decimal LTP)
        {
            try
            {
                TradeStopLossObj = new TradeStopLoss()
                {
                    ordertype = Convert.ToInt16(Req.ordertype),
                    StopPrice = Req.StopPrice,
                    Status = Convert.ToInt16(enTransactionStatus.Success),
                    LTP = LTP,
                    PairID = Req.PairID,
                    TrnNo = Req.TrnNo,
                    ISFollowersReq = Req.ISFollowersReq,//Rita 12-1-19 main req always 0
                    FollowingTo = Req.FollowingTo,//Rita 12-1-19 main req always 0
                    LeaderTrnNo = Req.LeaderTrnNo,//Rita 21-1-19 main req always 0
                    FollowTradeType = Req.FollowTradeType//Rita 22-1-19 main req always blank
                };
                if (Req.ordertype == enTransactionMarketType.STOP_Limit)
                {
                    if (Req.StopPrice <= LTP)//250 - 300 Low
                    {
                        if (Req.StopPrice == LTP)
                            STOPLimitWithSameLTP = 1;

                        TradeStopLossObj.RangeMin = Req.StopPrice;
                        TradeStopLossObj.RangeMax = LTP;
                        TradeStopLossObj.MarketIndicator = 0;
                    }
                    else if (Req.StopPrice > LTP)//300 - 350 High
                    {
                        TradeStopLossObj.RangeMin = LTP;
                        TradeStopLossObj.RangeMax = Req.StopPrice;
                        TradeStopLossObj.MarketIndicator = 1;
                    }
                }
                TradeStopLossObj = _TradeStopLoss.Add(TradeStopLossObj);
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommSuccessMsgInternal, ReturnCode = enResponseCodeService.Success });
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("InsertTradeStopLoss:##TrnNo " + Req.TrnNo, ControllerName, ex));
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
                throw ex;
            }

        }
        #endregion        

        #region RegionProcessTransaction        
        public async void MarkTransactionSystemFail(string StatusMsg, enErrorCode ErrorCode)
        {
            try
            {
                //var Txn = _TransactionRepository.GetById(Req.TrnNo);
                Newtransaction.MakeTransactionSystemFail();
                Newtransaction.SetTransactionStatusMsg(StatusMsg);
                Newtransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
                _TransactionRepository.UpdateAsync(Newtransaction);

                //var TradeTxn = _TradeTransactionRepository.GetById(Req.TrnNo);
                NewTradetransaction.MakeTransactionSystemFail();
                NewTradetransaction.SetTransactionStatusMsg(StatusMsg);
                NewTradetransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
                _TradeTransactionRepository.Update(NewTradetransaction);
                try
                {
                    //Rita 26-11-2018 add Activity Notifiation v2
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);

                    if (ErrorCode == enErrorCode.sp_InsufficientBalanceForCharge)//Rita 13-03-19 In this case only send diff ErrorCode as per front and wallet
                        notification.MsgCode = Convert.ToInt32(ErrorCode);

                    //notification.MsgCode = Convert.ToInt32(ErrorCode);
                    notification.Param1 = Req.TrnNo.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Fail);
                    _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2);//Req.accessToken
                                                                                                          //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("MarkTransactionSystemFail Notification Send " + notification.MsgCode, ControllerName, "##TrnNo:" + Newtransaction.Id, Helpers.UTC_To_IST()));
                }
                catch (Exception ex)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ISignalRService Notification Error-MarkTransactionSystemFail", ControllerName, ex.Message + "##TrnNo:" + NewTradetransaction.TrnNo, Helpers.UTC_To_IST()));
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("MarkTransactionSystemFail:##TrnNo " + Req.TrnNo, ControllerName, ex));
                // throw ex;
            }
        }
        public async Task MarkTransactionHold(string StatusMsg, enErrorCode ErrorCode)
        {
            try
            {
                if (Newtransaction.Status != 0)//Rita 22-2-19 this update after settlement so overrights status, error solved
                    return;

                Newtransaction.MakeTransactionHold();
                Newtransaction.SetTransactionStatusMsg(StatusMsg);
                Newtransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
                _TransactionRepository.UpdateAsync(Newtransaction);

                //var Txn = _TransactionRepository.GetById(Req.TrnNo);
                //rita 28-12-18 remove active inactive as txn considers in settlement time
                //if (Req.ordertype == enTransactionMarketType.STOP_Limit && STOPLimitWithSameLTP == 0)//Rita 26-12-18 for STOP & limit Order
                //{
                //    NewTradetransaction.MakeTransactionInActive();
                //}
                //else
                //{
                //    NewTradetransaction.MakeTransactionHold();
                //}
                if (NewTradetransaction.Status != 0)
                    return;

                NewTradetransaction.MakeTransactionHold();
                NewTradetransaction.SetTransactionStatusMsg(StatusMsg);
                NewTradetransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
                if (NewTradetransaction.Status == 1)
                    return;
                _TradeTransactionRepository.Update(NewTradetransaction);

                //if (Req.ordertype == enTransactionMarketType.STOP_Limit && STOPLimitWithSameLTP == 0)//Rita 26-12-18 for STOP & limit Order
                //{
                //    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                //    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRTrnSuccessfullyCreated);
                //    notification.Param1 = Req.Price.ToString();
                //    notification.Param2 = Req.Qty.ToString();
                //    notification.Type = Convert.ToInt16(EnNotificationType.Success);
                //    _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2);//Req.accessToken    
                //    return;//for Inactive Order no need to send Book,history etc
                //}

                try
                {
                    var CopyNewtransaction = new TransactionQueue();
                    CopyNewtransaction = (TransactionQueue)Newtransaction.Clone();
                    //CopyNewtransaction.MakeTransactionHold();

                    var CopyNewTradetransaction = new TradeTransactionQueue();
                    CopyNewTradetransaction = (TradeTransactionQueue)NewTradetransaction.Clone();
                    //CopyNewTradetransaction.MakeTransactionHold();
                    //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ISignalRService", ControllerName, "parallel execution pre ##TrnNo:",Helpers.UTC_To_IST()));
                    Parallel.Invoke(() => _ISignalRService.OnStatusHold(Convert.ToInt16(enTransactionStatus.Success), CopyNewtransaction, CopyNewTradetransaction, "", TradeStopLossObj.ordertype));
                    //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ISignalRService", ControllerName, "parallel execution complete ##TrnNo:",Helpers.UTC_To_IST()));
                    if (Req.MemberID != marketMakerUserRole)
                    {   //Uday 06-12-2018  Send SMS When Transaction is Hold
                        SMSSendTransactionHoldOrFailed(Newtransaction.Id, Newtransaction.MemberMobile, Req.Price, Req.Qty, 1);
                    }
                }
                catch (Exception ex)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ISignalRService", ControllerName, "Trading Hold Error " + ex.Message + "##TrnNo:" + NewTradetransaction.TrnNo, Helpers.UTC_To_IST()));
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("MarkTransactionHold:##TrnNo " + Req.TrnNo, ControllerName, ex));
                throw ex;
            }
        }
        #endregion
        #region Market Maker Counter Trade
        public async Task PlaceMarketMakerCounterTrade()
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PlaceMarketMakerCounterTrade", ControllerName, "MarketMaker HOLD Order Proces Start" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                //get preference for OrderHoldOrderRateChange from database and manipulate price -Sahil 14-10-2019 03:57 PM
                //Rita 16-10-19 5:26 PM devide qty as per ',' in taken column also make trade as per array count
                string AllrateChanges = _frontTrnRepository.GetMarketMakerHoldOrderRateChange(Req.PairID);

                if (AllrateChanges != null && AllrateChanges != "")//if not configure then not make counter order
                {
                    //ex. total 4 order of this Req.Qty=50 , with price of column detail (same , 0.10, 0.20 ,0.50)
                    decimal[] AllrateChangeArray = Array.ConvertAll(AllrateChanges.Split(','), new Converter<string, decimal>(Decimal.Parse)); ; // 4

                    //if change rate is not eligible with divided Qty for make order skip it and divide Qty according remaining rate -Sahil 21-10-2019 10:49 AM
                    if (Req.TrnType == enTrnType.Buy_Trade)
                    {
                        Array.Sort(AllrateChangeArray);
                    }
                    else if (Req.TrnType == enTrnType.Sell_Trade)
                    {
                        Array.Sort(AllrateChangeArray);
                        Array.Reverse(AllrateChangeArray);
                    }

                    decimal[] AllowedChangeRate = (decimal[])AllrateChangeArray.Clone();

                    foreach (decimal rateChange in AllrateChangeArray)
                    {
                        decimal changedPrice = Req.Price;
                        if (Req.TrnType == enTrnType.Buy_Trade)
                        {
                            changedPrice = changedPrice + ((changedPrice * rateChange) / 100);
                        }
                        else if (Req.TrnType == enTrnType.Sell_Trade)
                        {
                            changedPrice = changedPrice - ((changedPrice * rateChange) / 100);
                        }
                        decimal changeAmt = (Req.Qty / AllowedChangeRate.Count()) * changedPrice;
                        decimal roundedAmt = Math.Round(changeAmt, _TradePairDetailObj.AmtLength);
                        //if (_TradePairDetailObj.MinNotional > Req.Price * Req.Qty || _TradePairDetailObj.MaxNotional < Req.Price * Req.Qty)

                        if (roundedAmt == changeAmt && (_TradePairDetailObj.MinNotional <= changeAmt && _TradePairDetailObj.MaxNotional >= changeAmt))
                            break;
                        AllowedChangeRate = AllowedChangeRate.Skip(1).ToArray();
                    }

                    decimal OrderQty = Helpers.DoRoundForTrading(Req.Qty / AllowedChangeRate.Count(), 18); // 50/4 = 12.5

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PlaceMarketMakerCounterTrade", ControllerName, "MarketMaker HOLD Order Proces OrderQty:" + OrderQty + " AllrateChanges:" + AllrateChanges + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));

                    //create order with eligible rate with respect to divide qty -Sahil 21-10-2019 10:50 AM
                    foreach (decimal rateChange in AllowedChangeRate)
                    {
                        try
                        {
                            if (rateChange > 100)
                                continue;

                            decimal changedPrice = Req.Price;
                            if (Req.TrnType == enTrnType.Sell_Trade) //down price for reverse current sell order (maker buy order)
                                changedPrice = changedPrice - ((changedPrice * rateChange) / 100);
                            else if (Req.TrnType == enTrnType.Buy_Trade) //up price for reverse current buy order (maker sell order)
                                changedPrice = changedPrice + ((changedPrice * rateChange) / 100);

                            await _mediator.Send(new NewTransactionRequestCls()
                            {
                                TrnMode = Req.TrnMode,
                                TrnType = Req.TrnType == enTrnType.Buy_Trade ? enTrnType.Sell_Trade : enTrnType.Buy_Trade,
                                ordertype = Req.ordertype,
                                SMSCode = Req.SMSCode,
                                TransactionAccount = Req.TransactionAccount,
                                Amount = 0,
                                PairID = Req.PairID,
                                Price = Math.Round(changedPrice, _TradePairDetailObj.PriceLength),
                                Qty = Math.Round(OrderQty, _TradePairDetailObj.QtyLength),
                                DebitAccountID = Req.DebitWalletID.ToString(),
                                CreditAccountID = Req.CreditWalletID.ToString(),
                                StopPrice = Req.StopPrice,
                                GUID = Guid.NewGuid(),
                                MemberID = Req.MemberID,
                                MemberMobile = Req.MemberMobile,
                                accessToken = Req.accessToken
                            });

                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PlaceMarketMakerCounterTrade", ControllerName, $"MarketMaker divided order process changedPrice: {changedPrice},  rateChange:{rateChange}, PairID:{Req.PairID}, TrnNo:{Req.TrnNo}", Helpers.UTC_To_IST()));
                        }
                        catch (Exception e)
                        {
                            HelperForLog.WriteErrorLog("PlaceMarketMakerCounterTrade AllrateChangeArray In loop Internal Error:##TrnNo " + Req.TrnNo, ControllerName, e);
                        }
                    }

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PlaceMarketMakerCounterTrade", ControllerName, "MarketMaker HOLD Order Proces END" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                    //HelperForLog.WriteLogIntoFile("CombineAllInitTransactionAsync", "", $"##MarketMaker## success order placed TrnNo:{Req.TrnNo}");
                }
                else
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PlaceMarketMakerCounterTrade", ControllerName, " skip counter order" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                }
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("PlaceMarketMakerCounterTrade Internal Error:##TrnNo " + Req.TrnNo, ControllerName, e);
                //HelperForLog.WriteLogIntoFile("CombineAllInitTransactionAsync", "", $"##MarketMaker## success order placed throw an exception for TrnNo:{Req.TrnNo}, check error log");
                //HelperForLog.WriteErrorLog("CombineAllInitTransactionAsync", "", e);
            }
        }

        public async Task PlaceMarketMakerCounterTradeAsTaker(long memberid)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PlaceMarketMakerCounterTradeAsTaker", ControllerName, "MarketMaker HOLD Order Proces Start" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));

                string AllrateChanges = _frontTrnRepository.GetMarketMakerHoldOrderRateChange(Req.PairID);
                var TradeList = _frontTrnRepository.GetMarketMakerSettledByTakerList(NewTradetransaction.TrnNo, memberid);

                if (AllrateChanges != null && AllrateChanges != "")//if not configure then not make counter order
                {
                    foreach (var obj in TradeList)
                    {
                        decimal[] AllrateChangeArray = Array.ConvertAll(AllrateChanges.Split(','), new Converter<string, decimal>(Decimal.Parse)); ; // 4
                        if (obj.TrnType == (short)enTrnType.Buy_Trade)
                        {
                            Array.Sort(AllrateChangeArray);
                        }
                        else if (obj.TrnType == (short)enTrnType.Sell_Trade)
                        {
                            Array.Sort(AllrateChangeArray);
                            Array.Reverse(AllrateChangeArray);
                        }
                        decimal[] AllowedChangeRate = (decimal[])AllrateChangeArray.Clone();
                        foreach (decimal rateChange in AllrateChangeArray)
                        {
                            decimal changedPrice = obj.Price;
                            if (obj.TrnType == (short)enTrnType.Buy_Trade)
                            {
                                changedPrice = changedPrice + ((changedPrice * rateChange) / 100);
                            }
                            else if (obj.TrnType == (short)enTrnType.Sell_Trade)
                            {
                                changedPrice = changedPrice - ((changedPrice * rateChange) / 100);
                            }
                            decimal changeAmt = (obj.Qty / AllowedChangeRate.Count()) * changedPrice;
                            decimal roundedAmt = Math.Round(changeAmt, _TradePairDetailObj.AmtLength);
                            //if (_TradePairDetailObj.MinNotional > Req.Price * Req.Qty || _TradePairDetailObj.MaxNotional < Req.Price * Req.Qty)

                            if (roundedAmt == changeAmt && (_TradePairDetailObj.MinNotional <= changeAmt && _TradePairDetailObj.MaxNotional >= changeAmt))
                                break;
                            AllowedChangeRate = AllowedChangeRate.Skip(1).ToArray();
                        }
                        AllowedChangeRate = AllowedChangeRate.Take(1).ToArray();
                        foreach (decimal rateChange in AllowedChangeRate)
                        {
                            try
                            {
                                if (rateChange > 100)
                                    continue;

                                decimal changedPrice = obj.Price;
                                if (obj.TrnType == (short)enTrnType.Sell_Trade) //down price for reverse current sell order (maker buy order)
                                    changedPrice = changedPrice - ((changedPrice * rateChange) / 100);
                                else if (obj.TrnType == (short)enTrnType.Buy_Trade) //up price for reverse current buy order (maker sell order)
                                    changedPrice = changedPrice + ((changedPrice * rateChange) / 100);

                                await _mediator.Send(new NewTransactionRequestCls()
                                {
                                    TrnMode = Req.TrnMode,
                                    TrnType = obj.TrnType == (short)enTrnType.Buy_Trade ? enTrnType.Sell_Trade : enTrnType.Buy_Trade,
                                    ordertype = Req.ordertype,
                                    SMSCode = Req.SMSCode,
                                    TransactionAccount = Req.TransactionAccount,
                                    Amount = 0,
                                    PairID = Req.PairID,
                                    Price = Math.Round(changedPrice, _TradePairDetailObj.PriceLength),
                                    Qty = Math.Round(obj.Qty, _TradePairDetailObj.QtyLength),
                                    DebitAccountID = obj.orderWalletID.ToString(),
                                    CreditAccountID = obj.deliveryWalletID.ToString(),
                                    StopPrice = Req.StopPrice,
                                    GUID = Guid.NewGuid(),
                                    MemberID = obj.MemberId,
                                    MemberMobile = Req.MemberMobile,
                                    accessToken = Req.accessToken
                                });

                                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PlaceMarketMakerCounterTradeAsTaker", ControllerName, $"MarketMaker divided order process changedPrice: {changedPrice},  rateChange:{rateChange}, PairID:{Req.PairID}, TrnNo:{Req.TrnNo}", Helpers.UTC_To_IST()));
                            }
                            catch (Exception e)
                            {
                                HelperForLog.WriteErrorLog("PlaceMarketMakerCounterTradeAsTaker AllrateChangeArray In loop Internal Error:##TrnNo " + Req.TrnNo, ControllerName, e);
                            }
                        }
                    }
                }
                else
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PlaceMarketMakerCounterTradeAsTaker", ControllerName, " skip counter order" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                }
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("PlaceMarketMakerCounterTradeAsTaker Internal Error:##TrnNo " + Req.TrnNo, ControllerName, e);
            }
        }
        #endregion
        #region Settlement Insert Data        
        public async Task InsertSellerList()
        {
            try
            {
                TradeSellerListObj = new TradeSellerListV1()
                {
                    CreatedBy = Req.MemberID,
                    TrnNo = Req.TrnNo,
                    PairID = Req.PairID,
                    PairName = _TradeTransactionObj.PairName,
                    Price = Req.Price,
                    Qty = Req.Qty,
                    ReleasedQty = Req.Qty,
                    SelledQty = 0,
                    RemainQty = Req.Qty,
                    IsProcessing = 1,
                    OrderType = Convert.ToInt16(Req.ordertype),
                    Status = Convert.ToInt16(enTransactionStatus.Initialize),//txn type status
                    IsAPITrade = 0,//Rita 30-1-19 for API level changes in settlement , do not pick in local settlement
                };
                if (Req.ordertype == enTransactionMarketType.STOP_Limit && STOPLimitWithSameLTP == 0)
                {
                    //TradeSellerListObj.Status = Convert.ToInt16(enTransactionStatus.InActive);
                    TradeSellerListObj.IsProcessing = 0;
                }
                //TradeSellerListObj =_TradeSellerList.Add(TradeSellerListObj);
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommSuccessMsgInternal, ReturnCode = enResponseCodeService.Success });
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("Tradepoolmaster:##TrnNo " + Req.TrnNo, ControllerName, ex));
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
                throw ex;
            }
        }

        public async Task InsertBuyerList()
        {
            try
            {
                TradeBuyerListObj = new TradeBuyerListV1()
                {
                    CreatedBy = Req.MemberID,
                    TrnNo = Req.TrnNo,
                    PairID = Req.PairID,
                    PairName = _TradeTransactionObj.PairName,
                    Price = Req.Price,
                    Qty = Req.Qty, //same as request as one entry per one request
                    DeliveredQty = 0,
                    RemainQty = Req.Qty,
                    IsProcessing = 1,
                    OrderType = Convert.ToInt16(Req.ordertype),
                    Status = Convert.ToInt16(enTransactionStatus.Initialize),//txn type status
                    IsAPITrade = 0,//Rita 30-1-19 for API level changes in settlement , do not pick in local settlement
                };
                if (Req.ordertype == enTransactionMarketType.STOP_Limit && STOPLimitWithSameLTP == 0)
                {
                    //TradeBuyerListObj.Status = Convert.ToInt16(enTransactionStatus.InActive);
                    TradeBuyerListObj.IsProcessing = 0;
                }
                //TradeBuyerListObj = _TradeBuyerList.Add(TradeBuyerListObj);
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommSuccessMsgInternal, ReturnCode = enResponseCodeService.Success });
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("InsertBuyerList:##TrnNo " + Req.TrnNo, ControllerName, ex));
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
                throw ex;
            }

        }

        //public async Task<BizResponse> TradingDataInsert(BizResponse _Resp)
        //{
        //    try
        //    {                
        //        //foreach (TransactionProviderResponse Provider in TxnProviderList)//Make txn on every API
        //        //{
        //        //    Newtransaction.SetServiceProviderData(Provider.ServiceID, Provider.ServiceProID, Provider.ProductID, Provider.RouteID);
        //        //    _TransactionRepository.Update(Newtransaction);
        //        //    break;
        //        //}                
        //        _Resp.ReturnMsg = "success";
        //        _Resp.ReturnCode = enResponseCodeService.Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        Task.Run(() => HelperForLog.WriteErrorLog("TradingDataInsert:##TrnNo " + Req.TrnNo, ControllerName, ex));
        //        _Resp.ReturnCode = enResponseCodeService.Fail;
        //        _Resp.ReturnMsg = ex.Message;
        //    }
        //    //return Task.FromResult(_Resp);
        //    return _Resp;
        //}

        #endregion

        //khushali 22-05-2019 Seperate code block Trading type wise 
        public async Task<BizResponse> TradingConfiguration(BizResponse _Resp)
        {
            ProcessTransactionCls _TransactionObj = new ProcessTransactionCls();
            WebAPIParseResponseCls WebAPIParseResponseClsObj = new WebAPIParseResponseCls();
            //RealTimeLtpChecker RealTimeLtpCheckerobj = new RealTimeLtpChecker();
            GetLTPDataLPwise GetLTPDataLPwiseObj = new GetLTPDataLPwise();
            List<CryptoWatcher> LTPData = new List<CryptoWatcher>();
            short IsTxnProceed = 0;
            _Resp = new BizResponse();
            try
            {
                //var BinanceExchangeInfoResp = _binanceLPService.GetExchangeInfoAsync();

                //check MarketMaker is on and also user has present with MarketMaker role in database -Sahil 16-10-2019 04:56 PM
                int marketMakerUserRole = _frontTrnRepository.GetMarketMakerUserRole();

                //Rita 16-10-19 6:04 check if currenct Pair is configured for Market Maker or not , if not then continue in LP call, either publish
                int PreferPairCountOfMarketMaker = 0;
                if (Req.TrnType == enTrnType.Sell_Trade && IsOnlyMarketMakingTradeOn == 1)//May be is not table present so add maker condition
                {
                    MarketMakerBuyPreferencesViewModel PreferenceList = _frontTrnRepository.GetMarketMakerUserBuyPreferences(Req.PairID);
                    if (PreferenceList != null)
                        PreferPairCountOfMarketMaker = 1;
                }
                else if (Req.TrnType == enTrnType.Buy_Trade && IsOnlyMarketMakingTradeOn == 1)//May be is not table present so add maker condition
                {
                    MarketMakerSellPreferencesViewModel PreferenceList = _frontTrnRepository.GetMarketMakerUserSellPreferences(Req.PairID);
                    if (PreferenceList != null)
                        PreferPairCountOfMarketMaker = 1;
                }

                if (IsOnlyMarketMakingTradeOn == 1 && marketMakerUserRole != 0 && PreferPairCountOfMarketMaker != 0) //execute when market maker is on and user have role in table -Sahil 03-10-2019 7:30 PM
                {
                    //rita 4-10-19 11:48 AM,  does not publish event if txn is from market maker,also does not call liquidity 
                    if (Req.MemberID == marketMakerUserRole) //execute when userId and MarkerMaker both are same -Sahil 03-10-2019 7:30 PM
                    {
                        _Resp.ReturnMsg = "Success";
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ErrorCode = enErrorCode.Success;

                        return _Resp;
                    }

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("TradingConfiguration", ControllerName, $"MarketMaker HOLD Order, marketMakerUserRole:{marketMakerUserRole}, PreferPairCountOfMarketMaker:{PreferPairCountOfMarketMaker} ##TrnNo:{Req.TrnNo}", Helpers.UTC_To_IST()));

                    TransactionOnHoldCompletedIntegrationEvent @event = new TransactionOnHoldCompletedIntegrationEvent(
                                                                    Req.MemberID,
                                                                    Req.TrnNo,
                                                                    (short)Req.TrnType,
                                                                    Req.Price,
                                                                    Req.Qty,
                                                                    Req.PairID);

                    _iEventBus.Publish(
                        @event,
                        _iConfiguration.GetValue<string>("RabbitMQConfig:BrokerName"),
                        typeof(TransactionOnHoldCompletedIntegrationEvent).Name,
                        _iConfiguration.GetValue<string>("RabbitMQConfig:TypeOfExchange"));

                    _Resp.ReturnMsg = "Success";
                    _Resp.ReturnCode = enResponseCodeService.Success;
                    _Resp.ErrorCode = enErrorCode.Success;

                    return _Resp;
                }


                if ((IsOnlyMarketMakingTradeOn == 1 || IsOnlyLiquidtyTradeOn == 1) && TxnProviderList.Count != 0)//enTradingType.MarketMaking.ToString() == "MarketMaking"
                {
                    #region old logic - Real time check LTP
                    //RealTimeLtpCheckerobj.Pair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                    //foreach (TransactionProviderResponse Provider in TxnProviderList)
                    //{

                    //    RealTimeLtpCheckerobj.List.Add(new LTPcls { LpType = Convert.ToInt16(Provider.AppTypeID), Price = 0 });
                    //}
                    //RealTimeLtpCheckerobj = await _mediator.Send(RealTimeLtpCheckerobj);
                    //if (Req.TrnType == enTrnType.Buy_Trade)
                    //{
                    //    RealTimeLtpCheckerobj.List = RealTimeLtpCheckerobj.List.OrderBy(o => o.Price).ToList();
                    //}
                    //else
                    //{
                    //    RealTimeLtpCheckerobj.List = RealTimeLtpCheckerobj.List.OrderByDescending(o => o.Price).ToList();
                    //}

                    //foreach (var Data in RealTimeLtpCheckerobj.List)//Make txn on every API
                    //{
                    //    TransactionProviderResponse Provider = TxnProviderList.Where(e => (short)e.AppTypeID == Data.LpType).FirstOrDefault();
                    //    IsTxnProceed = await TradingDataInsertV2(_Resp, Provider);
                    //    if (IsTxnProceed == 0)
                    //        continue;

                    //}
                    #endregion

                    GetLTPDataLPwiseObj.Pair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                    foreach (TransactionProviderResponseV1 Provider in TxnProviderList)
                    {
                        if (GetLTPDataLPwiseObj.LpType == null)
                        {
                            GetLTPDataLPwiseObj.LpType = Provider.AppTypeID.ToString();
                        }
                        else
                        {
                            GetLTPDataLPwiseObj.LpType += "," + Provider.AppTypeID.ToString();
                        }
                    }
                    LTPData = _frontTrnRepository.GetPairWiseLTPData(GetLTPDataLPwiseObj);

                    //LTPData = LTPData.Where(e => e.LTP > 0).ToList();

                    if (Req.TrnType == enTrnType.Buy_Trade)
                    {
                        //LTPData = LTPData.Where(e => e.LTP > 0 && e.LTP <= Req.Price).OrderBy(o => o.LTP).ToList();
                        LTPData = LTPData.Where(e => e.LTP > 0).OrderBy(o => o.LTP).ToList();
                    }
                    else
                    {
                        //LTPData = LTPData.Where(e => e.LTP > 0 && e.LTP >= Req.Price).OrderByDescending(o => o.LTP).ToList();
                        LTPData = LTPData.Where(e => e.LTP > 0).OrderByDescending(o => o.LTP).ToList();
                    }

                    //////Static Code for Testing Please remove while live (Pushpraj)
                    ///
                    //////LTPData = LTPData.Where(e => e.LPType==20).Distinct().ToList();
                    foreach (var Data in LTPData)//Make txn on every API
                    {
                        var Price = Data.LTP;
                        if (Data.LPType == (short)enAppType.COINTTRADINGLocal && IsProceedInLocal == 0)
                        {
                            //if and only if Market making and LTP hase first priority for Local trading then N then add below code for local trading

                            if (Req.TrnType == enTrnType.Buy_Trade)
                            {
                                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "END Buyer" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                                if (Req.ordertype != enTransactionMarketType.STOP_Limit || STOPLimitWithSameLTP == 1)
                                {
                                    _Resp = await _SettlementRepositoryV1.PROCESSSETLLEMENTBuy(_Resp, NewTradetransaction, Newtransaction, TradeStopLossObj, TradeBuyerListObj, Req.accessToken, 0);
                                }
                            }
                            else
                            {
                                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "END Seller" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                                if (Req.ordertype != enTransactionMarketType.STOP_Limit || STOPLimitWithSameLTP == 1)
                                {
                                    _Resp = await _SettlementRepositoryV1.PROCESSSETLLEMENTSell(_Resp, NewTradetransaction, Newtransaction, TradeStopLossObj, TradeSellerListObj, Req.accessToken, 0);
                                }
                            }
                            if (_Resp.ReturnCode != enResponseCodeService.Success)//Move on next provider
                            {
                                IsTxnProceed = 0;
                                continue;
                            }
                            IsTxnProceed = 1;//does not fail this order
                            return _Resp;
                        }
                        if (IsMaxProfit == 1)
                        {
                            if (Req.Price == 0 && Req.ordertype == enTransactionMarketType.MARKET)
                            {
                                Price = Helpers.DoRoundForTrading(Data.LTP, 8);
                            }
                            else if (Req.TrnType == enTrnType.Buy_Trade)
                            {

                                if (Req.Price > 0 && Data.LTP <= Req.Price) // 2 LTP -1 
                                {
                                    Price = Helpers.DoRoundForTrading(Data.LTP, 8);
                                }
                                else if (Req.Price > 0 && Data.LTP >= Req.Price) // 1 , LTP - 3
                                {
                                    Price = Req.Price;
                                }
                                else
                                {
                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                    _Resp.ReturnCode = enResponseCodeService.Fail;
                                    _Resp.ReturnMsg = "Transaction Failed";
                                    return _Resp;
                                }

                            }
                            else if (Req.TrnType == enTrnType.Sell_Trade)
                            {
                                if (Req.Price > 0 && Data.LTP >= Req.Price)
                                {
                                    Price = Helpers.DoRoundForTrading(Data.LTP, 8);
                                }
                                else if (Req.Price > 0 && Data.LTP <= Req.Price)
                                {
                                    Price = Req.Price;
                                }
                                else
                                {
                                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                    _Resp.ReturnCode = enResponseCodeService.Fail;
                                    _Resp.ReturnMsg = "Transaction Failed";
                                    return _Resp;
                                }
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Failed";
                                return _Resp;
                            }
                        }
                        else
                        {
                            if (Req.Price == 0 && Req.ordertype == enTransactionMarketType.MARKET)
                            {
                                Price = Helpers.DoRoundForTrading(Data.LTP, 8);
                            }
                            else if (Req.ordertype == enTransactionMarketType.LIMIT)
                            {
                                Price = Req.Price;
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Failed";
                                return _Resp;
                            }
                        }

                        TransactionProviderResponseV1 Provider = TxnProviderList.Where(e => (short)e.AppTypeID == Data.LPType).FirstOrDefault();

                        IsTxnProceed = await TradingDataInsertV2(_Resp, Provider, Price);
                        if (IsTxnProceed == 0)
                            continue;
                        return _Resp;
                    }
                    if (NewTradetransaction.IsAPITrade == 0)//stay in local
                    {
                        if (Req.TrnType == enTrnType.Buy_Trade)
                        {
                            TradeBuyerListObj.IsProcessing = 0;
                            _TradeBuyerList.Update(TradeBuyerListObj);
                        }
                        else
                        {
                            TradeSellerListObj.IsProcessing = 0;
                            _TradeSellerList.Update(TradeSellerListObj);
                        }
                    }
                }
                //else if(enTradingType.Liquidity.ToString() == "Liquidity")
                //{
                //    foreach (TransactionProviderResponse Provider in TxnProviderList)//Make txn on every API
                //    {
                //        IsTxnProceed = await TradingDataInsertV2(_Resp, Provider);
                //        if (IsTxnProceed == 0)
                //            continue;

                //    }
                //}
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CallWebAPI:##TrnNo " + Req.TrnNo, ControllerName, ex);
                if (IsTxnProceed == 0)
                {
                    _Resp.ReturnMsg = "Error occured";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.ProcessTrn_APICallInternalError;
                }
                else
                {
                    _Resp.ReturnMsg = "Hold";
                    _Resp.ReturnCode = enResponseCodeService.Success;
                    _Resp.ErrorCode = enErrorCode.ProcessTrn_Hold;
                }

            }
            return await Task.FromResult(_Resp);
        }

        //khushali 22-05-2019 Seperate code block Trading type wise 
        public async Task<short> TradingDataInsertV2(BizResponse _Resp, TransactionProviderResponseV1 Provider, decimal LTP)
        {
            ProcessTransactionCls _TransactionObj = new ProcessTransactionCls();
            WebAPIParseResponseCls WebAPIParseResponseClsObj = new WebAPIParseResponseCls();
            RealTimeLtpChecker RealTimeLtpCheckerobj = new RealTimeLtpChecker();
            LPProcessTransactionCls LPProcessTransactionClsObj = new LPProcessTransactionCls();
            short IsTxnProceed = 0;
            _Resp = new BizResponse();
            try
            {

                Newtransaction.SetServiceProviderData(Provider.ServiceID, Provider.ServiceProID, Provider.ProductID, Provider.RouteID, Provider.SerProDetailID, (short)Provider.AppTypeID);
                _TransactionRepository.Update(Newtransaction);

                #region "Don't check Provider route for local trading"
                ////if and only if Market making and LTP hase first priority for Local trading then N then add below code for local trading

                //if (Enum.Parse<enWebAPIRouteType>(Provider.ProTypeID.ToString()) == enWebAPIRouteType.TradeServiceLocal && IsProceedInLocal == 0)
                //{
                //    if (Req.TrnType == enTrnType.Buy_Trade)
                //    {
                //        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "END Buyer" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                //        if (Req.ordertype != enTransactionMarketType.STOP_Limit || STOPLimitWithSameLTP == 1)
                //        {
                //            _Resp = await _SettlementRepositoryV1.PROCESSSETLLEMENTBuy(_Resp, NewTradetransaction, Newtransaction, TradeStopLossObj, TradeBuyerListObj, Req.accessToken, 0);
                //        }
                //    }
                //    else
                //    {
                //        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "END Seller" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                //        if (Req.ordertype != enTransactionMarketType.STOP_Limit || STOPLimitWithSameLTP == 1)
                //        {
                //            _Resp = await _SettlementRepositoryV1.PROCESSSETLLEMENTSell(_Resp, NewTradetransaction, Newtransaction, TradeStopLossObj, TradeSellerListObj, Req.accessToken, 0);
                //        }
                //    }
                //    if (_Resp.ReturnCode != enResponseCodeService.Success)//Move on next provider
                //    {
                //        IsTxnProceed = 0;
                //    }
                //    IsTxnProceed = 1;//does not fail this order
                //}                
                //else 
                #endregion

                if (Provider.ProTypeID == (long)enWebAPIRouteType.LiquidityProvider)// for liquidity provider
                {
                    //Task.Delay(5000).Wait();

                    //var ServiceProConfiguration = _IGetWebRequest.GetServiceProviderConfiguration(Provider.SerProDetailID);
                    var ServiceProConfiguration = new ServiceProConfiguration();
                    ServiceProConfiguration.APIKey = Provider.APIKey;
                    ServiceProConfiguration.SecretKey = Provider.SecretKey;
                    //if (ServiceProConfiguration == null || string.IsNullOrEmpty(ServiceProConfiguration.APIKey) || string.IsNullOrEmpty(ServiceProConfiguration.SecretKey))
                    //{
                    //    _Resp.ReturnMsg = EnResponseMessage.ProcessTrn_ThirdPartyDataNotFoundMsg;
                    //    _Resp.ReturnCode = enResponseCodeService.Fail;
                    //    _Resp.ErrorCode = enErrorCode.ProcessTrn_ThirdPartyDataNotFound;
                    //    IsTxnProceed = 0;
                    //    return await Task.FromResult(IsTxnProceed);
                    //}                    

                    HelperForLog.WriteLogIntoFile("LiquidityConfiguration", ControllerName, "--1--LiquidityConfiguration Call Web API---" + "##TrnNo:" + Req.TrnNo);
                    //Insert API request Data
                    _TransactionObj.TransactionRequestID = InsertTransactionRequest(Provider, "Price : " + Req.Price + "API Price : " + LTP + "  Qty : " + Req.Amount + " OrderType :" + Req.ordertype);

                    //Static Code for call any Exchange by Pushpraj  Please remove while Live testing......
                    // Provider.AppTypeID = 20;

                    switch (Provider.AppTypeID)
                    {
                        case (long)enAppType.Binance:
                            await ProcessTransactionOnBinance(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            //await ProcessTransactionOnBinance(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj, Req.StopPrice);
                            if (_TransactionObj.APIResponse != null)
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + _TransactionObj.APIResponse;
                            }
                            else
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + " Return Message : " + _Resp.ReturnMsg.ToString();
                            }
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;

                        case (long)enAppType.Huobi:
                            var huobiExchangeInfoResp = _huobiLPService.GetExchangeInfoAsync();
                            await ProcessTransactionOnHuobi(_Resp, ServiceProConfiguration, huobiExchangeInfoResp.Result.Data, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_TransactionObj.APIResponse != null)
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + _TransactionObj.APIResponse;
                            }
                            else
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + " Return Message : " + _Resp.ReturnMsg.ToString();
                            }
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;

                        case (long)enAppType.Bittrex:
                            await ProcessTransactionOnBittrex(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_TransactionObj.APIResponse != null)
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + _TransactionObj.APIResponse;
                            }
                            else
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + " Return Message : " + _Resp.ReturnMsg.ToString();
                            }
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;
                        case (long)enAppType.TradeSatoshi:
                            await ProcessTransactionOnTradeSatoshi(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_TransactionObj.APIResponse != null)
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + _TransactionObj.APIResponse;
                            }
                            else
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + " Return Message : " + _Resp.ReturnMsg.ToString();
                            }
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;
                        case (long)enAppType.Poloniex:
                            await ProcessTransactionOnTradePoloniex(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_TransactionObj.APIResponse != null)
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + _TransactionObj.APIResponse;
                            }
                            else
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + " Return Message : " + _Resp.ReturnMsg.ToString();
                            }
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;
                        case (long)enAppType.Coinbase:
                            await ProcessTransactionOnTradeCoinbase(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_TransactionObj.APIResponse != null)
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + _TransactionObj.APIResponse;
                            }
                            else
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + " Return Message : " + _Resp.ReturnMsg.ToString();
                            }
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;
                        case (long)enAppType.UpBit:
                            await ProcessTransactionOnTradeUpbit(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_TransactionObj.APIResponse != null)
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + _TransactionObj.APIResponse;
                            }
                            else
                            {
                                _TransactionObj.APIResponse = "ReturnCode : " + _Resp.ErrorCode.ToString() + " Return Message : " + _Resp.ReturnMsg.ToString();
                            }
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;

                        //Add new case for OKEX 
                        case (long)enAppType.OKEx:
                            await ProcessTransactionOnOKEX(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;

                        case (long)enAppType.Kraken:
                            await ProcessTransactionOnTradekrakenAsync(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;

                        //Add new case for Bitfinex Exchange by Pushpraj as on 09-07-2019
                        case (long)enAppType.Bitfinex:
                            await ProcessTransactionOnBitfinex(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;

                        case (long)enAppType.Gemini:
                            await ProcessTransactionOnGemini(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;

                        case (long)enAppType.CEXIO:
                            await ProcessTransactionOnCEXIO(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;
                        //Add new case for Yobit Exchange by Puspraj as on 16-07-2019
                        case (long)enAppType.Yobit:
                            await ProcessTransactionOnYobit(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;
                        case (long)enAppType.EXMO:
                            await ProcessTransactionOnEXMO(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                            if (_Resp.ReturnCode != enResponseCodeService.Success)
                                IsTxnProceed = 0;//check next loop provider
                            else
                                IsTxnProceed = 1;//does not fail this order
                            goto SuccessTrade;

                        //Add new case for EXMO Exchange by Puspraj as on 17-07-2019
                        //case (long)enAppType.EXMO:
                        //    await ProcessTransactionOnEXMO(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
                        //    if (_Resp.ReturnCode != enResponseCodeService.Success)
                        //        IsTxnProceed = 0;//check next loop provider
                        //    else
                        //        IsTxnProceed = 1;//does not fail this order
                        //    goto SuccessTrade;
                        default:
                            HelperForLog.WriteLogIntoFile("LiquidityConfiguration", ControllerName, "--3--LiquidityConfiguration Call web API  not found proper liquidity provider---" + "##TrnNo:" + Req.TrnNo);
                            break;
                    }
                SuccessTrade:

                    HelperForLog.WriteLogIntoFile("LiquidityConfiguration", ControllerName, "--2--LiquidityConfiguration Call web API---" + "##TrnNo:" + Req.TrnNo);


                    if (IsTxnProceed == 1)
                    {
                        NewtransactionRequest.SetResponse(_TransactionObj.APIResponse);
                        NewtransactionRequest.SetResponseTime(Helpers.UTC_To_IST());

                        NewtransactionRequest.SetTrnID(WebAPIParseResponseClsObj.TrnRefNo);
                        NewtransactionRequest.SetOprTrnID(WebAPIParseResponseClsObj.OperatorRefNo);
                        _TransactionRequest.Update(NewtransactionRequest);

                        NewTradetransaction.IsAPITrade = 1;
                        NewTradetransaction.APIPrice = LTP;
                        NewTradetransaction.SetTransactionStatusMsg(WebAPIParseResponseClsObj.StatusMsg);
                        if (_Resp.ErrorCode == enErrorCode.API_LP_Filled)
                        {
                            NewTradetransaction.APIStatus = "1";
                            Newtransaction.CallStatus = 1;
                        }
                        else
                        {
                            NewTradetransaction.APIStatus = "4";
                            Newtransaction.CallStatus = 0;
                        }
                        _TradeTransactionRepository.Update(NewTradetransaction);
                        Newtransaction.TrnRefNo = WebAPIParseResponseClsObj.TrnRefNo;
                        _TransactionRepository.Update(Newtransaction);
                        if (Req.TrnType == enTrnType.Buy_Trade)
                        {
                            TradeBuyerListObj.Status = NewTradetransaction.Status;
                            TradeBuyerListObj.IsAPITrade = NewTradetransaction.IsAPITrade;
                            TradeBuyerListObj.IsProcessing = 0;
                            _TradeBuyerList.Update(TradeBuyerListObj);
                        }
                        else if (Req.TrnType == enTrnType.Sell_Trade)
                        {
                            TradeSellerListObj.Status = NewTradetransaction.Status;
                            TradeSellerListObj.IsAPITrade = NewTradetransaction.IsAPITrade;
                            TradeSellerListObj.IsProcessing = 0;
                            _TradeSellerList.Update(TradeSellerListObj);
                        }
                        try
                        {
                            if (Provider.AppTypeID != (long)enAppType.COINTTRADINGLocal)//After success ,hold LP balance , if fail then manually make hold before status check
                            {
                                HelperForLog.WriteLogIntoFileAsyncDtTm("LP balance Hold START ", ControllerName, "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST());

                                //Make Provider balance hold as goes in LP
                                LPHoldDr WalletProviderHoldObj = new LPHoldDr();
                                WalletProviderHoldObj.SerProID = Provider.ServiceProID;
                                WalletProviderHoldObj.CoinName = Req.SMSCode;
                                WalletProviderHoldObj.Timestamp = Helpers.GetTimeStamp();
                                WalletProviderHoldObj.Amount = Req.Amount;
                                WalletProviderHoldObj.TrnRefNo = Req.TrnNo;
                                WalletProviderHoldObj.trnType = Req.TrnType == enTrnType.Buy_Trade ? enWalletTrnType.BuyTrade : enWalletTrnType.SellTrade;
                                WalletProviderHoldObj.PairId = Req.PairID;//2019-6-28 vsolanki addredd resquest param pair id for hold sp
                                WalletProviderHoldObj.enWalletDeductionType = enWalletDeductionType.Normal;


                                WalletDrCrResponse WalletProviderHoldResp = await _LPWalletTransaction.LPGetWalletHoldNew(WalletProviderHoldObj);
                                HelperForLog.WriteLogIntoFileAsyncDtTm("Regular LP Balance hold END ", ControllerName, "##ErrorCode:" + WalletProviderHoldResp.ErrorCode + " ##ReturnCode:" + WalletProviderHoldResp.ReturnCode + " ##ReturnMsg:" + WalletProviderHoldResp.ReturnMsg + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST());

                                if (WalletProviderHoldResp.ReturnCode != enResponseCode.Success)
                                {
                                    //_Resp.ReturnMsg = WalletProviderHoldResp.ReturnMsg;//EnResponseMessage.ProcessTrn_WalletDebitFailMsg;
                                    //_Resp.ReturnCode = enResponseCodeService.Fail;
                                    //_Resp.ErrorCode = WalletProviderHoldResp.ErrorCode;//enErrorCode.ProcessTrn_WalletDebitFail;                                    
                                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "LP Balance Deduction Fail" + _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            HelperForLog.WriteErrorLog("TradingDataInsertV2 LP Waller HOLD Internal Error: ##IsTxnProceed:" + IsTxnProceed + " ##TrnNo " + Req.TrnNo, ControllerName, ex);
                        }

                        if (_Resp.ErrorCode == enErrorCode.API_LP_Filled)
                        {
                            //await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPI(_Resp, Req.TrnNo,0,Req.Amount,Req.Price);
                            await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPIFromInit(_Resp, Req.TrnNo, 0, Req.Qty, Req.Price, Newtransaction, NewTradetransaction, TradeStopLossObj, TradeBuyerListObj, TradeSellerListObj);
                        }
                        else if (_Resp.ErrorCode == enErrorCode.API_LP_PartialFilled)
                        {
                            //await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPI(_Resp, Req.TrnNo,0,Req.Amount,Req.Price);
                            await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPIFromInit(_Resp, Req.TrnNo, LPProcessTransactionClsObj.RemainingQty, LPProcessTransactionClsObj.SettledQty, Req.Price, Newtransaction, NewTradetransaction, TradeStopLossObj, TradeBuyerListObj, TradeSellerListObj);
                        }

                    }
                }

                if (IsTxnProceed == 0)//Here add logic of SPOT order is pending then make release and remove code of release from settlementrepository
                {
                    Newtransaction.SetServiceProviderData(0, 0, 0, 0, 0, (short)Provider.AppTypeID);
                    _TransactionRepository.Update(Newtransaction);
                    _Resp.ErrorCode = enErrorCode.ProcessTrn_OprFail;
                    if (_TransactionObj.APIResponse != null)
                    {
                        NewtransactionRequest.SetResponse(_TransactionObj.APIResponse);
                        NewtransactionRequest.SetResponseTime(Helpers.UTC_To_IST());
                        _TransactionRequest.Update(NewtransactionRequest);
                    }

                    //if(Req.TrnType == enTrnType.Buy_Trade)
                    //{
                    //    await _SettlementRepositoryV1.ReleaseWalletAmountBuy(TradeBuyerListObj, Newtransaction, NewTradetransaction);
                    //}
                    //else if (Req.TrnType == enTrnType.Sell_Trade)
                    //{
                    //    await _SettlementRepositoryV1.ReleaseWalletAmountSell(TradeSellerListObj, Newtransaction, NewTradetransaction);
                    //}
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("TradingDataInsertV2 Internal Error: ##IsTxnProceed:" + IsTxnProceed + " ##TrnNo " + Req.TrnNo, ControllerName, ex);
                if (IsTxnProceed == 0)
                {
                    _Resp.ReturnMsg = "Error occured";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.ProcessTrn_APICallInternalError;
                }
                else
                {
                    _Resp.ReturnMsg = "Hold";
                    _Resp.ReturnCode = enResponseCodeService.Success;
                    _Resp.ErrorCode = enErrorCode.ProcessTrn_Hold;
                }

            }
            return await Task.FromResult(IsTxnProceed);
        }

        public List<CryptoWatcher> GetPairWiseLTPDataRedis(GetLTPDataLPwise LTPData)
        {
            IQueryable<CryptoWatcher> Result = null;
            string Qry = "";

            try
            {
                //_IResdisTradingManagment

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        // khushali 22-05-2019 Seperate code block Trading type wise - old code
        #region TradingDataInsertV2OLD

        //public async Task<BizResponse> TradingDataInsertV2OLD(BizResponse _Resp)
        //{
        //    ProcessTransactionCls _TransactionObj = new ProcessTransactionCls();
        //    WebAPIParseResponseCls WebAPIParseResponseClsObj = new WebAPIParseResponseCls();
        //    RealTimeLtpChecker RealTimeLtpCheckerobj = new RealTimeLtpChecker();
        //    short IsTxnProceed = 0;
        //    _Resp = new BizResponse();
        //    try
        //    {
        //        var BinanceExchangeInfoResp = _binanceLPService.GetExchangeInfoAsync();
        //        if (enTradingType.MarketMaking.ToString() == "MarketMaking")
        //        {
        //            RealTimeLtpCheckerobj.Pair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
        //            foreach (TransactionProviderResponse Provider in TxnProviderList)
        //            {
        //                RealTimeLtpCheckerobj.List.Add(new LTPcls { LpType = Convert.ToInt16(Provider.AppTypeID), Price = 0 });
        //            }
        //            RealTimeLtpCheckerobj = await _mediator.Send(RealTimeLtpCheckerobj);
        //        }


        //        if (Req.TrnType == enTrnType.Buy_Trade)
        //        {
        //            TradeBuyerListObj.TrnNo = Req.TrnNo;
        //            TradeBuyerListObj = _TradeBuyerList.Add(TradeBuyerListObj);
        //            RealTimeLtpCheckerobj.List = RealTimeLtpCheckerobj.List.OrderBy(o => o.Price).ToList();
        //        }
        //        else
        //        {
        //            TradeSellerListObj.TrnNo = Req.TrnNo;
        //            TradeSellerListObj = _TradeSellerList.Add(TradeSellerListObj);
        //            RealTimeLtpCheckerobj.List = RealTimeLtpCheckerobj.List.OrderByDescending(o => o.Price).ToList();
        //        }


        //        //foreach (TransactionProviderResponse Provider in TxnProviderList)//Make txn on every API
        //        foreach (var Data in RealTimeLtpCheckerobj.List)//Make txn on every API
        //        {
        //            TransactionProviderResponse Provider = TxnProviderList.Where(e => (short)e.AppTypeID == Data.LpType).FirstOrDefault();
        //            Newtransaction.SetServiceProviderData(Provider.ServiceID, Provider.SerProDetailID, Provider.ProductID, Provider.RouteID);
        //            _TransactionRepository.Update(Newtransaction);

        //            if (Enum.Parse<enWebAPIRouteType>(Provider.ProTypeID.ToString()) == enWebAPIRouteType.TradeServiceLocal)
        //            {
        //                if (Req.TrnType == enTrnType.Buy_Trade)
        //                {
        //                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "END Buyer" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
        //                    if (Req.ordertype != enTransactionMarketType.STOP_Limit || STOPLimitWithSameLTP == 1)
        //                    {
        //                        _Resp = await _SettlementRepositoryV1.PROCESSSETLLEMENTBuy(_Resp, NewTradetransaction, Newtransaction, TradeStopLossObj, TradeBuyerListObj, Req.accessToken, 0);
        //                    }
        //                }
        //                else
        //                {
        //                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "END Seller" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
        //                    if (Req.ordertype != enTransactionMarketType.STOP_Limit || STOPLimitWithSameLTP == 1)
        //                    {
        //                        _Resp = await _SettlementRepositoryV1.PROCESSSETLLEMENTSell(_Resp, NewTradetransaction, Newtransaction, TradeStopLossObj, TradeSellerListObj, Req.accessToken, 0);
        //                    }
        //                }
        //                if (_Resp.ReturnCode != enResponseCodeService.Success)//Move on next provider
        //                {
        //                    continue;
        //                }
        //                IsTxnProceed = 1;//does not fail this order
        //                break;
        //            }
        //            else if (Enum.Parse<enWebAPIRouteType>(Provider.ProTypeID.ToString()) == enWebAPIRouteType.LiquidityProvider)// for liquidity provider
        //            {
        //                Task.Delay(5000).Wait();

        //                var ServiceProConfiguration = _IGetWebRequest.GetServiceProviderConfiguration(Provider.SerProDetailID);
        //                if (ServiceProConfiguration == null)
        //                {
        //                    _Resp.ReturnMsg = EnResponseMessage.ProcessTrn_ThirdPartyDataNotFoundMsg;
        //                    _Resp.ReturnCode = enResponseCodeService.Fail;
        //                    _Resp.ErrorCode = enErrorCode.ProcessTrn_ThirdPartyDataNotFound;
        //                    continue;
        //                }

        //                HelperForLog.WriteLogIntoFile("LiquidityConfiguration", ControllerName, "--1--LiquidityConfiguration Call Web API---" + "##TrnNo:" + Req.TrnNo);
        //                //Insert API request Data
        //                _TransactionObj.TransactionRequestID = InsertTransactionRequest(Provider, "Price : " + Req.Price + "  Qty : " + Req.Amount + " OrderType :" + Req.ordertype);

        //                switch (Provider.AppTypeID)
        //                {
        //                    case (long)enAppType.Binance:
        //                        await ProcessTransactionOnBinance(_Resp, ServiceProConfiguration, BinanceExchangeInfoResp.Result.Data, WebAPIParseResponseClsObj, Provider, Req.TrnNo);
        //                        if (_Resp.ReturnCode != enResponseCodeService.Success)
        //                            continue;//check next loop provider

        //                        IsTxnProceed = 1;//does not fail this order
        //                        goto SuccessTrade;

        //                    case (long)enAppType.Bittrex:
        //                        await ProcessTransactionOnBittrex(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo);
        //                        if (_Resp.ReturnCode != enResponseCodeService.Success)
        //                            continue;//check next loop provider

        //                        IsTxnProceed = 1;//does not fail this order
        //                        goto SuccessTrade;

        //                    case (long)enAppType.TradeSatoshi:
        //                        await ProcessTransactionOnTradeSatoshi(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo);
        //                        if (_Resp.ReturnCode != enResponseCodeService.Success)
        //                            continue;//check next loop provider

        //                        IsTxnProceed = 1;//does not fail this order
        //                        goto SuccessTrade;
        //                    case (long)enAppType.Poloniex:
        //                        await ProcessTransactionOnTradePoloniex(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo);
        //                        if (_Resp.ReturnCode != enResponseCodeService.Success)
        //                            continue;//check next loop provider

        //                        IsTxnProceed = 1;//does not fail this order
        //                        goto SuccessTrade;
        //                    case (long)enAppType.Coinbase:
        //                        await ProcessTransactionOnTradeCoinbase(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo);
        //                        if (_Resp.ReturnCode != enResponseCodeService.Success)
        //                            continue;//check next loop provider

        //                        IsTxnProceed = 1;//does not fail this order
        //                        goto SuccessTrade;

        //                    default:
        //                        HelperForLog.WriteLogIntoFile("LiquidityConfiguration", ControllerName, "--3--LiquidityConfiguration Call web API  not found proper liquidity provider---" + "##TrnNo:" + Req.TrnNo);
        //                        break;
        //                }
        //                SuccessTrade:

        //                HelperForLog.WriteLogIntoFile("LiquidityConfiguration", ControllerName, "--2--LiquidityConfiguration Call web API---" + "##TrnNo:" + Req.TrnNo);

        //                NewtransactionRequest.SetResponse(_TransactionObj.APIResponse);
        //                NewtransactionRequest.SetResponseTime(Helpers.UTC_To_IST());

        //                NewtransactionRequest.SetTrnID(WebAPIParseResponseClsObj.TrnRefNo);
        //                NewtransactionRequest.SetOprTrnID(WebAPIParseResponseClsObj.OperatorRefNo);
        //                _TransactionRequest.Update(NewtransactionRequest);
        //                if (IsTxnProceed == 1)
        //                {
        //                    NewTradetransaction.IsAPITrade = 1;
        //                    NewTradetransaction.SetTransactionStatusMsg(WebAPIParseResponseClsObj.StatusMsg);
        //                    _TradeTransactionRepository.Update(NewTradetransaction);
        //                    break;
        //                }
        //            }

        //        }
        //        if (IsTxnProceed == 0)//Here add logic of SPOT order is pending then make release and remove code of release from settlementrepository
        //        {
        //            _Resp.ErrorCode = enErrorCode.ProcessTrn_OprFail;
        //            //if(Req.TrnType == enTrnType.Buy_Trade)
        //            //{
        //            //    await _SettlementRepositoryV1.ReleaseWalletAmountBuy(TradeBuyerListObj, Newtransaction, NewTradetransaction);
        //            //}
        //            //else if (Req.TrnType == enTrnType.Sell_Trade)
        //            //{
        //            //    await _SettlementRepositoryV1.ReleaseWalletAmountSell(TradeSellerListObj, Newtransaction, NewTradetransaction);
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog("CallWebAPI:##TrnNo " + Req.TrnNo, ControllerName, ex);
        //        if (IsTxnProceed == 0)
        //        {
        //            _Resp.ReturnMsg = "Error occured";
        //            _Resp.ReturnCode = enResponseCodeService.Fail;
        //            _Resp.ErrorCode = enErrorCode.ProcessTrn_APICallInternalError;
        //        }
        //        else
        //        {
        //            _Resp.ReturnMsg = "Hold";
        //            _Resp.ReturnCode = enResponseCodeService.Success;
        //            _Resp.ErrorCode = enErrorCode.ProcessTrn_Hold;
        //        }

        //    }
        //    return await Task.FromResult(_Resp);
        //}

        #endregion

        private async Task<BizResponse> ProcessTransactionOnHuobi(BizResponse _Resp, ServiceProConfiguration serviceProConfiguration, List<HuobiSymbol> data, WebAPIParseResponseCls webAPIParseResponseClsObj, TransactionProviderResponseV1 provider, long trnNo, ProcessTransactionCls transactionObj, decimal lTP, LPProcessTransactionCls lPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                _huobiLPService._client.SetApiCredentials(serviceProConfiguration.APIKey, serviceProConfiguration.SecretKey);
                string LocalPair = _BaseCurrService.SMSCode + _SecondCurrService.SMSCode;

                foreach (var obj in data)
                {
                    if (LocalPair.ToLower() == obj.Symbol)
                    {
                        WebCallResult<long> HuobiResult = await _huobiLPService.PlaceOrder(Req.AccountID, obj.Symbol, Req.TrnType == enTrnType.Sell_Trade ? Huobi.Net.Objects.HuobiOrderType.MarketSell : Huobi.Net.Objects.HuobiOrderType.MarketBuy, Req.Qty, Req.Price);

                        // var result= "{  \"account-id\": \"100009\",\"amount\": \"10.1\", \"price\": \"100.1\",\"source\": \"api\",\"symbol\": \"ethusdt\"  \"type\": \"buy-limit\"}";
                        if (HuobiResult == null)
                        {
                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = "Transaction Fail On huobi";
                            return _Resp;
                        }
                        if (HuobiResult.Success)
                        {
                            IsProcessedBit = 1;
                            _TransactionObj.APIResponse = JsonConvert.SerializeObject(HuobiResult);
                            //_TransactionObj.APIResponse = JsonConvert.SerializeObject(result);
                            if (HuobiResult.Success)
                            {

                                // CallResult<BittrexAccountOrder> BittrexResult2 = await _bitrexLPService.GetOrderInfoAsync(BittrexResult1.Data.Uuid);
                                WebCallResult<HuobiOrder> webCall = await _huobiLPService.GetOrderInfoAsync(HuobiResult.Data);

                                webAPIParseResponseClsObj.TrnRefNo = HuobiResult.Data.ToString();
                                webAPIParseResponseClsObj.OperatorRefNo = webCall.Data.Id.ToString();
                                if (webCall.Success)

                                {

                                    if (webCall.Data.State == HuobiOrderState.Filled)
                                    {
                                        lPProcessTransactionClsObj.RemainingQty = 0;
                                        lPProcessTransactionClsObj.SettledQty = webCall.Data.FilledAmount;
                                        lPProcessTransactionClsObj.TotalQty = Req.Qty;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction fully Success On Huobi";
                                        return _Resp;

                                    }
                                    else if (webCall.Data.State == HuobiOrderState.PartiallyFilled)
                                    {
                                        lPProcessTransactionClsObj.RemainingQty = webCall.Data.Amount - webCall.Data.FilledAmount;
                                        lPProcessTransactionClsObj.SettledQty = webCall.Data.FilledAmount;
                                        lPProcessTransactionClsObj.TotalQty = Req.Qty;
                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction partial Success On huobi";
                                        return _Resp;
                                    }
                                    else if (webCall.Data.State == HuobiOrderState.PartiallyCanceled)
                                    {
                                        lPProcessTransactionClsObj.RemainingQty = webCall.Data.FilledAmount;
                                        lPProcessTransactionClsObj.SettledQty = webCall.Data.FilledAmount;
                                        lPProcessTransactionClsObj.TotalQty = Req.Qty;
                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                        _Resp.ReturnCode = enResponseCodeService.Fail;
                                        _Resp.ReturnMsg = "Transaction partial FAIL On huobi";
                                        return _Resp;
                                    }
                                    else if (webCall.Data.State == HuobiOrderState.Canceled)
                                    {
                                        webAPIParseResponseClsObj.Status = enTransactionStatus.OperatorFail;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                        _Resp.ReturnCode = enResponseCodeService.Fail;
                                        _Resp.ReturnMsg = "Transaction Fail On huobi";
                                    }
                                    else if (webCall.Data.State == HuobiOrderState.Created)
                                    {
                                        _Resp.ErrorCode = enErrorCode.API_LP_Success;
                                        _Resp.ReturnCode = enResponseCodeService.Success;
                                        _Resp.ReturnMsg = "Transaction processing Success On huobi";
                                        return _Resp;
                                    }
                                    else
                                    {
                                        IsProcessedBit = 0;
                                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                        _Resp.ReturnCode = enResponseCodeService.Fail;
                                        _Resp.ReturnMsg = "Transaction Fail On huobi";
                                        return _Resp;
                                    }


                                }
                            }
                        }
                    }
                }
                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = "Transaction Fail On Huobi";

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionHuobi:##TrnNo " + trnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                {
                    if (webAPIParseResponseClsObj.Status == enTransactionStatus.Success)
                    {
                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction Success On Huobi";
                        return _Resp;
                    }
                    else
                    {
                        _Resp.ReturnCode = enResponseCodeService.Success;
                    }
                }
                else
                {
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                    _Resp.ReturnMsg = ex.Message;
                    _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
                }
            }
            return _Resp;
        }

        //Rushabh 19-07-2019 Added Stop Price Parameter
        //private async Task<BizResponse> ProcessTransactionOnBinance(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj, decimal StopPrice)
        private async Task<BizResponse> ProcessTransactionOnBinance(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                //BinanceExchangeInfo BinanceExchangeInfoResp = _binanceLPService.GetExchangeInfoAsync().Result.Data;
                //BinanceExchangeInfo BinanceExchangeInfoResult = _cache.Get<BinanceExchangeInfo>("BinanceExchangeInfoResp");
                //if (BinanceExchangeInfoResult == null)
                //{
                //    BinanceExchangeInfoResult = _binanceLPService.GetExchangeInfoAsync().Result.Data;
                //    _cache.Set<BinanceExchangeInfo>("BinanceExchangeInfoResp", BinanceExchangeInfoResult);
                //}
                //else if (BinanceExchangeInfoResult.Symbols.Count() == 0)
                //{
                //    BinanceExchangeInfoResult = _binanceLPService.GetExchangeInfoAsync().Result.Data;
                //    _cache.Set<BinanceExchangeInfo>("BinanceExchangeInfoResp", BinanceExchangeInfoResult);
                //}
                _binanceLPService._client.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);
                //if (BinanceExchangeInfoResult != null)
                //{
                //    foreach (var symbol in BinanceExchangeInfoResult.Symbols)
                //    {
                //        if (symbol.Name == (_SecondCurrService.SMSCode + _BaseCurrService.SMSCode))
                //        {
                //            //if (symbol.MinNotionalFilter.MinNotional <= Req.Price * Req.Qty)
                //            if (symbol.MinNotionalFilter.MinNotional <= LTP * Req.Qty)
                //            {
                //                //if (symbol.PriceFilter.MinPrice <= Req.Price && symbol.PriceFilter.MaxPrice >= Req.Price)
                //                if (symbol.PriceFilter.MinPrice <= LTP && symbol.PriceFilter.MaxPrice >= LTP)
                //                {
                //if (symbol.LotSizeFilter.MinQuantity <= Req.Qty && symbol.LotSizeFilter.MaxQuantity >= Req.Qty)
                //{
                Binance.Net.Objects.OrderType type = Req.ordertype == enTransactionMarketType.LIMIT ? Binance.Net.Objects.OrderType.Limit
                : (Req.ordertype == enTransactionMarketType.MARKET ? Binance.Net.Objects.OrderType.Market
                : (Req.ordertype == enTransactionMarketType.STOP ? Binance.Net.Objects.OrderType.StopLoss
                : (Req.ordertype == enTransactionMarketType.STOP_Limit ? Binance.Net.Objects.OrderType.StopLossLimit : Binance.Net.Objects.OrderType.TakeProfit
                )));

                // commented for testing purpose
                ////CallResult<BinancePlacedOrder> BinanceResult = await _binanceLPService.PlaceOrderAsync(Req.TrnType == enTrnType.Sell_Trade ? Binance.Net.Objects.OrderSide.Sell : Binance.Net.Objects.OrderSide.Buy, symbol.Name, type, Req.Qty, price: Req.Price, stopPrice: Req.StopPrice, timeInForce: TimeInForce.ImmediateOrCancel);

                CallResult<BinancePlacedOrder> BinanceResult = await _binanceLPService.PlaceOrderAsync(Req.TrnType == enTrnType.Sell_Trade ? Binance.Net.Objects.OrderSide.Sell : Binance.Net.Objects.OrderSide.Buy, Provider.OpCode, type, quantity: Req.Qty, price: LTP, stopPrice: Req.StopPrice, timeInForce: Binance.Net.Objects.TimeInForce.ImmediateOrCancel, receiveWindow: 7000);

                // khushali Testing Resposne 
                //var Result = @" {""Data"":{""Symbol"":""ETHBTC"",""OrderId"":372478262,""ClientOrderId"":""qyqWCPYsuEXhCx2ICIhpOj"",""TransactTime"":1559044223603,""Price"":0.03090000,""origQty"":0.10000000,""executedQty"":0.10000000,""cummulativeQuoteQty"":0.00309610,""Status"":""FILLED"",""TimeInForce"":""IOC"",""Type"":""LIMIT"",""Side"":""SELL"",""Fills"":[{""TradeId"":124844138,""Price"":0.03096100,""qty"":0.10000000,""Commission"":0.00000310,""CommissionAsset"":""BTC""}]},""Error"":null,""Success"":true}";
                //CallResult<BinancePlacedOrder> BinanceResult = JsonConvert.DeserializeObject<CallResult<BinancePlacedOrder>>(Result);
                if (BinanceResult == null)
                {
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Fail On Binanace";
                    return _Resp;
                }
                HelperForLog.WriteLogIntoFile("ProcessTransactionOnBinance:##TrnNo " + TrnNo, ControllerName, JsonConvert.SerializeObject(BinanceResult));
                if (!BinanceResult.Success && BinanceResult.Data == null)
                {
                    _TransactionObj.APIResponse = JsonConvert.SerializeObject(BinanceResult.Error);
                    if (BinanceResult.Error.Code == -1021)
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_Timeout;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_Timeout;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Message.ToUpper().Contains("LOT_SIZE"))
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_LOT_SIZE;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_LOT_SIZE;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Message.ToUpper().Contains("PERCENT_PRICE"))
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_PERCENT_PRICE;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_PERCENT_PRICE;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Message.ToUpper().Contains("PRICE_FILTER"))
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_PRICE_FILTER;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_PRICE_FILTER;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Message.ToUpper().Contains("MIN_NOTIONAL"))
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_MIN_NOTIONAL;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_MIN_NOTIONAL;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Message.ToUpper().Contains("ICEBERG_PARTS"))
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_ICEBERG_PARTS;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_ICEBERG_PARTS;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Message.ToUpper().Contains("MARKET_LOT_SIZE"))
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_MARKET_LOT_SIZE;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_MARKET_LOT_SIZE;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Message.ToUpper().Contains("MAX_NUM_ORDERS"))
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_MAX_NUM_ORDERS;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_MAX_NUM_ORDERS;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Message.ToUpper().Contains("MAX_ALGO_ORDERS"))
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_MAX_ALGO_ORDERS;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_MAX_ALGO_ORDERS;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Message.ToUpper().Contains("MAX_NUM_ICEBERG_ORDERS"))
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_MAX_NUM_ICEBERG_ORDERS;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_MAX_NUM_ICEBERG_ORDERS;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Message.ToUpper().Contains("EXCHANGE_MAX_NUM_ORDERS"))
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_EXCHANGE_MAX_NUM_ORDERS;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_EXCHANGE_MAX_NUM_ORDERS;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Message.ToUpper().Contains("EXCHANGE_MAX_ALGO_ORDERS"))
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_EXCHANGE_MAX_ALGO_ORDERS;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_EXCHANGE_MAX_ALGO_ORDERS;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Message.ToUpper().Contains("INSUFFICIENT_FUNDS"))
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_INSUFFICIENT_FUNDS;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_INSUFFICIENT_FUNDS;
                        return _Resp;
                    }
                    else if (BinanceResult.Error.Code == -2010)
                    {
                        _Resp.ErrorCode = enErrorCode.Binance_INSUFFICIENT_FUNDS;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Binance_INSUFFICIENT_FUNDS;
                        return _Resp;
                    }
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    //_Resp.ReturnMsg = "Transaction Fail On Binanace";
                    _Resp.ReturnMsg = BinanceResult.Error != null ? BinanceResult.Error.Message : "Transaction Fail On Binanace";
                    return _Resp;
                }
                if (BinanceResult.Success)
                {
                    _TransactionObj.APIResponse = JsonConvert.SerializeObject(BinanceResult.Data);
                    IsProcessedBit = 1;
                    _TransactionObj.APIResponse = JsonConvert.SerializeObject(BinanceResult);

                    WebAPIParseResponseClsObj.TrnRefNo = BinanceResult.Data.OrderId.ToString();
                    WebAPIParseResponseClsObj.OperatorRefNo = BinanceResult.Data.ClientOrderId;
                    WebAPIParseResponseClsObj.Status = BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.New ? enTransactionStatus.Hold :
                    (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Filled ? enTransactionStatus.Success :
                    (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PartiallyFilled ? enTransactionStatus.Hold :
                    (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Rejected ? enTransactionStatus.OperatorFail :
                    (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Canceled ? enTransactionStatus.OperatorFail : enTransactionStatus.OperatorFail))));

                    if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Filled)
                    {
                        LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
                        LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
                        LPProcessTransactionClsObj.TotalQty = Req.Qty;
                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction fully Success On Binanace";
                        return _Resp;
                    }
                    else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PartiallyFilled)
                    {
                        LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
                        LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
                        LPProcessTransactionClsObj.TotalQty = Req.Qty;
                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction partial Success On Binanace";
                        return _Resp;
                    }
                    else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Expired)
                    {
                        IsProcessedBit = 0;
                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "Transaction Fail On Binanace";
                        return _Resp;
                    }
                    else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.New)
                    {
                        _Resp.ErrorCode = enErrorCode.API_LP_Success;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction processing Success On Binanace";
                        return _Resp;
                    }
                    else
                    {
                        IsProcessedBit = 0;
                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "Transaction Fail On Binanace";
                        return _Resp;
                    }

                }
                //_Resp.ErrorCode = enErrorCode.Binance_LOT_SIZE;
                //_Resp.ReturnCode = enResponseCodeService.Fail;
                //_Resp.ReturnMsg = EnResponseMessage.Binance_LOT_SIZE;
                //return _Resp;
                //}
                //_Resp.ErrorCode = enErrorCode.Binance_PRICE_FILTER;
                //_Resp.ReturnCode = enResponseCodeService.Fail;
                //_Resp.ReturnMsg = EnResponseMessage.Binance_PRICE_FILTER;
                //return _Resp;
                //                }

                //                _Resp.ErrorCode = enErrorCode.Binance_MIN_NOTIONAL;
                //                _Resp.ReturnCode = enResponseCodeService.Fail;
                //                _Resp.ReturnMsg = EnResponseMessage.Binance_MIN_NOTIONAL;
                //                return _Resp;
                //            }
                //        }
                //    }
                //_Resp.ErrorCode = enErrorCode.API_LP_Fail;
                //_Resp.ReturnCode = enResponseCodeService.Fail;
                //_Resp.ReturnMsg = "Transaction Fail On Binanace";
                //}
                //else
                //{
                //    HelperForLog.WriteLogIntoFile("ProcessTransactionOnBinance:##TrnNo ", ControllerName, " ##Response : Not found binance exchange info");
                //    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                //    _Resp.ReturnCode = enResponseCodeService.Fail;
                //    _Resp.ReturnMsg = "Transaction Fail On Binanace";
                //}

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnBinance:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
                    {
                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction Success On Binanace";
                        return _Resp;
                    }
                    else
                    {
                        _Resp.ReturnCode = enResponseCodeService.Success;
                    }
                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }

        //Rushabh 19-07-2019 Added Stop Price Parameter
        //private async Task<BizResponse> ProcessTransactionOnBittrex(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj, decimal StopPrice)
        private async Task<BizResponse> ProcessTransactionOnBittrex(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                //BittrexClient.SetDefaultOptions(new BittrexClientOptions()
                //{
                //    ApiCredentials = new ApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey)
                //});
                _bitrexLPService._client.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);
                _bitrexLPService._Clientv3.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);
                string LocalPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                string BittrexPair = LocalPair.Split("_")[0] + "-" + LocalPair.Split("_")[1];
                //CallResult<BittrexGuid> BittrexResult1 = await _bitrexLPService.PlaceOrderAsync(Req.TrnType == enTrnType.Sell_Trade ? Bittrex.Net.Objects.OrderSide.Sell : Bittrex.Net.Objects.OrderSide.Buy, BittrexPair, Req.Qty, LTP);

                //Rushabh 19-07-2019 added stop price parameter 
                //WebCallResult<Bittrex.Net.Objects.V3.BittrexOrderV3> BittrexResult1 = await _bitrexLPService.PlaceConditionalOrder(Req.TrnType == enTrnType.Sell_Trade ? Bittrex.Net.Objects.OrderSide.Sell : Bittrex.Net.Objects.OrderSide.Buy, BittrexPair, Req.Qty, LTP);
                WebCallResult<Bittrex.Net.Objects.V3.BittrexOrderV3> BittrexResult1 = await _bitrexLPService.PlaceConditionalOrder(Req.TrnType == enTrnType.Sell_Trade ? Bittrex.Net.Objects.OrderSide.Sell : Bittrex.Net.Objects.OrderSide.Buy, BittrexPair, Req.Qty, LTP);
                if (BittrexResult1 == null)
                {
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Fail On Bittrex";
                    return _Resp;
                }
                _TransactionObj.APIResponse = JsonConvert.SerializeObject(BittrexResult1);
                if (!BittrexResult1.Success && BittrexResult1.Data == null)
                {
                    if (BittrexResult1.Error.Message.ToUpper().Contains("INSUFFICIENT_FUNDS"))
                    {
                        _Resp.ErrorCode = enErrorCode.Bittrex_INSUFFICIENT_FUNDS;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Bittrex_INSUFFICIENT_FUNDS;
                        return _Resp;
                    }
                    else if (BittrexResult1.Error.Message.ToUpper().Contains("MIN_TRADE_REQUIREMENT_NOT_MET"))
                    {
                        _Resp.ErrorCode = enErrorCode.Bittrex_MIN_TRADE_REQUIREMENT_NOT_MET;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Bittrex_MIN_TRADE_REQUIREMENT_NOT_MET;
                        return _Resp;
                    }
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = BittrexResult1.Error != null ? BittrexResult1.Error.Message : "Transaction Fail On Bittrex";
                    return _Resp;
                }
                if (BittrexResult1.Success)
                {
                    IsProcessedBit = 1;
                    //CallResult<BittrexAccountOrder> BittrexResult2 = await _bitrexLPService.GetOrderInfoAsync(BittrexResult1.Data.Uuid);
                    //WebAPIParseResponseClsObj.TrnRefNo = BittrexResult1.Data.Uuid.ToString();
                    CallResult<BittrexAccountOrder> BittrexResult2 = await _bitrexLPService.GetOrderInfoAsync(new Guid(BittrexResult1.Data.Id));
                    WebAPIParseResponseClsObj.TrnRefNo = BittrexResult1.Data.Id.ToString();
                    WebAPIParseResponseClsObj.OperatorRefNo = BittrexResult2.Data.OrderUuid.ToString();
                    if (BittrexResult2.Success)
                    {
                        if (BittrexResult2.Data.QuantityRemaining == 0)
                        {
                            LPProcessTransactionClsObj.RemainingQty = BittrexResult2.Data.QuantityRemaining;
                            LPProcessTransactionClsObj.SettledQty = BittrexResult2.Data.Quantity - BittrexResult2.Data.QuantityRemaining;
                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
                            WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;
                            _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                            _Resp.ReturnCode = enResponseCodeService.Success;
                            _Resp.ReturnMsg = "Transaction fully Success On Bittrex";
                            return _Resp;
                        }
                        else if (BittrexResult2.Data.QuantityRemaining < BittrexResult2.Data.Quantity) // partial
                        {
                            LPProcessTransactionClsObj.RemainingQty = BittrexResult2.Data.QuantityRemaining;
                            LPProcessTransactionClsObj.SettledQty = BittrexResult2.Data.Quantity - BittrexResult2.Data.QuantityRemaining;
                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
                            WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
                            _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                            _Resp.ReturnCode = enResponseCodeService.Success;
                            _Resp.ReturnMsg = "Transaction partial Success On Bittrex";
                            return _Resp;
                        }
                        else if (BittrexResult2.Data.QuantityRemaining == BittrexResult2.Data.Quantity) // hold
                        {
                            LPProcessTransactionClsObj.RemainingQty = BittrexResult2.Data.QuantityRemaining;
                            LPProcessTransactionClsObj.SettledQty = BittrexResult2.Data.Quantity - BittrexResult2.Data.QuantityRemaining;
                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
                            WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
                            _Resp.ErrorCode = enErrorCode.API_LP_Success;
                            _Resp.ReturnCode = enResponseCodeService.Success;
                            _Resp.ReturnMsg = "Transaction processing Success On Bittrex";
                            return _Resp;
                        }
                        else if (BittrexResult2.Data.CancelInitiated)
                        {
                            IsProcessedBit = 0;
                            WebAPIParseResponseClsObj.Status = enTransactionStatus.OperatorFail;
                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = "Transaction Fail On Bittrex";
                        }
                    }
                    _Resp.ErrorCode = enErrorCode.API_LP_Success;
                    _Resp.ReturnCode = enResponseCodeService.Success;
                    _Resp.ReturnMsg = "Transaction processing Success On Bittrex";
                    return _Resp;
                }

                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = "Transaction Fail On Bittrex";
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnBittrex:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
                    {
                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction Success On Bittrex";
                        return _Resp;
                    }
                    else
                    {
                        _Resp.ReturnCode = enResponseCodeService.Success;
                    }

                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }

        private async Task<BizResponse> ProcessTransactionOnTradeSatoshi(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {

                GlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                GlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                string TradeSatoshiPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                /// commented -- actual code
                SubmitOrderReturn TradeSatoshiResult = _tradeSatoshiLPService.PlaceOrderAsync(Req.TrnType == enTrnType.Sell_Trade ? Core.Interfaces.LiquidityProvider.OrderSide.Sell : Core.Interfaces.LiquidityProvider.OrderSide.Buy, Provider.OpCode, Req.Qty, LTP).Result; //TradeSatoshiPair

                //var Result = @" {""success"":true,""message"":null,""result"":{""orderId"":140876176,""filled"":[21474705,21474706,21474707,21474708,21474709,21474710,21474711,21474712,21474713,21474714,21474715,21474716,21474717,21474718,21474719,21474720,21474721,21474722,21474723]}}";
                //SubmitOrderReturn TradeSatoshiResult = JsonConvert.DeserializeObject<SubmitOrderReturn>(Result);

                _TransactionObj.APIResponse = JsonConvert.SerializeObject(TradeSatoshiResult);
                if (TradeSatoshiResult.message != null && TradeSatoshiResult.result == null)
                {
                    if (TradeSatoshiResult.message.ToLower().Contains("Insufficient funds"))
                    {
                        _Resp.ErrorCode = enErrorCode.Tradesatoshi_INSUFFICIENT_FUNDS;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Tradesatoshi_INSUFFICIENT_FUNDS;
                        return _Resp;
                    }
                }
                if (TradeSatoshiResult.success)
                {
                    IsProcessedBit = 1;
                    if (TradeSatoshiResult.result.Filled.Count() > 0 && TradeSatoshiResult.result.OrderId == null)
                    {
                        TradeSatoshiResult.result.OrderId = TradeSatoshiResult.result.Filled[0];
                    }
                    WebAPIParseResponseClsObj.TrnRefNo = TradeSatoshiResult.result.OrderId.ToString();
                    WebAPIParseResponseClsObj.OperatorRefNo = TradeSatoshiResult.result.OrderId.ToString();
                    if (TradeSatoshiResult.result.OrderId != null && TradeSatoshiResult.result.Filled.Count() == 0)
                    {
                        GetOrderReturn TradeSatoshiResult1 = await _tradeSatoshiLPService.GetOrderInfoAsync(Convert.ToInt64(TradeSatoshiResult.result.OrderId));

                        //var Result1 = @" {""success"":true,""message"":null,""result"":{""id"":140876176,""market"":""ETH_BTC"",""type"":""Sell"",""amount"":0.01544508,""rate"":0.03198508,""remaining"":0.01094647,""total"":0.00049401,""status"":""Partial"",""timestamp"":""2019-05-29T11:16:11.527"",""isApi"":true}}";
                        //GetOrderReturn TradeSatoshiResult1 = JsonConvert.DeserializeObject<GetOrderReturn>(Result1);

                        //GetOrderReturn TradeSatoshiResult1 = await _tradeSatoshiLPService.GetOrderInfoAsync(TradeSatoshiResult.result.OrderId);
                        if (TradeSatoshiResult1.success)
                        {
                            if (TradeSatoshiResult1.result.Status.ToLower() == "complete")
                            {
                                LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult1.result.Remaining;
                                LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult1.result.Amount;
                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
                                WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;
                                _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                _Resp.ReturnCode = enResponseCodeService.Success;
                                _Resp.ReturnMsg = "Transaction fully Success On TradeSatoshi";
                                return _Resp;
                            }
                            else if (TradeSatoshiResult1.result.Status.ToLower() == "partial")
                            {
                                LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult1.result.Remaining;
                                LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult1.result.Amount - TradeSatoshiResult1.result.Remaining;
                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
                                WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
                                _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                _Resp.ReturnCode = enResponseCodeService.Success;
                                _Resp.ReturnMsg = "Transaction partial Success On TradeSatoshi";
                                return _Resp;
                            }
                            else if (TradeSatoshiResult1.result.Status.ToLower() == "pending")
                            {
                                LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult1.result.Remaining;
                                LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult1.result.Amount - TradeSatoshiResult1.result.Remaining;
                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
                                WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
                                _Resp.ErrorCode = enErrorCode.API_LP_Success;
                                _Resp.ReturnCode = enResponseCodeService.Success;
                                _Resp.ReturnMsg = "Transaction Success On TradeSatoshi";
                                return _Resp;
                            }
                            else
                            {
                                IsProcessedBit = 0;
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On TradeSatoshi";
                                return _Resp;
                            }
                        }
                        else
                        {
                            IsProcessedBit = 0;
                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = "Transaction Fail On TradeSatoshi";
                            return _Resp;
                        }
                    }
                    else if (TradeSatoshiResult.result.Filled.Count() > 0)
                    {
                        LPProcessTransactionClsObj.RemainingQty = 0;
                        LPProcessTransactionClsObj.SettledQty = Req.Qty;
                        LPProcessTransactionClsObj.TotalQty = Req.Qty;
                        WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;
                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction fully Success On TradeSatoshi";
                        return _Resp;

                    }
                    else
                    {
                        IsProcessedBit = 0;
                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "Transaction Fail On TradeSatoshi";
                        return _Resp;
                    }
                }
                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = "Transaction Fail On TradeSatoshi";

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnTradeSatoshi:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
                    {
                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction Success On Bittrex";
                        return _Resp;
                    }
                    else
                    {
                        _Resp.ReturnCode = enResponseCodeService.Success;
                    }
                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }

        private async Task<BizResponse> ProcessTransactionOnTradePoloniex(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                PoloniexGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                PoloniexGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                PoloniexOrderResult PoloniexResult = new PoloniexOrderResult();
                PoloniexErrorObj errorObj = new PoloniexErrorObj();

                var PoloniexRes = await _poloniexService.PlacePoloniexOrder(_BaseCurrService.SMSCode, _SecondCurrService.SMSCode, Req.Qty, LTP, Req.TrnType == enTrnType.Sell_Trade ? enOrderType.SellOrder : enOrderType.BuyOrder);
                if (PoloniexRes == null)
                {
                    errorObj = JsonConvert.DeserializeObject<PoloniexErrorObj>(PoloniexRes);
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Fail On Poloniex";
                    return _Resp;
                }
                if (PoloniexRes != null && PoloniexRes.ToLower().Contains("error"))
                {
                    errorObj = JsonConvert.DeserializeObject<PoloniexErrorObj>(PoloniexRes);
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Fail On Poloniex";
                    return _Resp;
                }
                IsProcessedBit = 1;
                _TransactionObj.APIResponse = PoloniexRes;

                PoloniexResult = JsonConvert.DeserializeObject<PoloniexOrderResult>(PoloniexRes);
                if (PoloniexResult.resultingTrades == null)
                {
                    errorObj = JsonConvert.DeserializeObject<PoloniexErrorObj>(PoloniexRes);
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Fail On Poloniex";
                    return _Resp;
                }
                if (PoloniexResult.orderNumber != null)
                {
                    WebAPIParseResponseClsObj.TrnRefNo = PoloniexResult.orderNumber.ToString();
                    WebAPIParseResponseClsObj.OperatorRefNo = PoloniexResult.orderNumber.ToString();
                    WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;

                    object PoloniexRep = await _poloniexService.GetPoloniexOrderState(WebAPIParseResponseClsObj.TrnRefNo);
                    JObject Data = JObject.Parse(PoloniexRep.ToString());
                    var Success = Convert.ToUInt16(Data["result"]["success"]);
                    if (Success == 1)
                    {
                        JToken Result = Data["result"][WebAPIParseResponseClsObj.TrnRefNo];
                        PoloniexOrderState PoloniexResult1 = JsonConvert.DeserializeObject<PoloniexOrderState>(Result.ToString());
                        if (PoloniexResult1.status == "Partially filled")
                        {
                            LPProcessTransactionClsObj.RemainingQty = PoloniexResult1.amount - PoloniexResult1.startingAmount;
                            LPProcessTransactionClsObj.SettledQty = PoloniexResult1.amount;
                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
                            WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
                            _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                            _Resp.ReturnCode = enResponseCodeService.Success;
                            _Resp.ReturnMsg = "Transaction partial Success On Bittrex";
                            return _Resp;
                        }
                        else if (PoloniexResult1.status == "Filled")
                        {
                            LPProcessTransactionClsObj.RemainingQty = 0;
                            LPProcessTransactionClsObj.SettledQty = PoloniexResult1.startingAmount;
                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
                            WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;
                            _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                            _Resp.ReturnCode = enResponseCodeService.Success;
                            _Resp.ReturnMsg = "Transaction fully Success On Bittrex";
                            return _Resp;
                        }
                    }

                    _Resp.ErrorCode = enErrorCode.API_LP_Success;
                    _Resp.ReturnCode = enResponseCodeService.Success;
                    _Resp.ReturnMsg = "Transaction processing Success On Poloniex";
                    return _Resp;
                }

                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = "Transaction Fail On Poloniex";

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnTradePoloniex:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
                    {
                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction Success On Bittrex";
                        return _Resp;
                    }
                    else
                    {
                        _Resp.ReturnCode = enResponseCodeService.Success;
                    }
                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }

        private async Task<BizResponse> ProcessTransactionOnTradeCoinbase(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                CoinBaseGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                CoinBaseGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                string LocalPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                string CoinBasePair = Char.ToUpperInvariant(LocalPair.Split("_")[1][0]) + LocalPair.Split("_")[1].Substring(1).ToLower() + Char.ToUpperInvariant(LocalPair.Split("_")[0][0]) + LocalPair.Split("_")[0].Substring(1).ToLower();
                OrderResponse CoinbaseResult = await _coinBaseService.PlaceOrder(Req.ordertype, Req.TrnType == enTrnType.Sell_Trade ? CoinbasePro.Services.Orders.Types.OrderSide.Sell : CoinbasePro.Services.Orders.Types.OrderSide.Buy, CoinBasePair, Req.Qty, LTP, Req.StopPrice);
                IsProcessedBit = 1;
                _TransactionObj.APIResponse = JsonConvert.SerializeObject(CoinbaseResult);
                if (CoinbaseResult.Status == CoinbasePro.Services.Orders.Types.OrderStatus.Active)
                {
                    WebAPIParseResponseClsObj.TrnRefNo = CoinbaseResult.Id.ToString();
                    WebAPIParseResponseClsObj.OperatorRefNo = CoinbaseResult.Id.ToString();
                    WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;

                    _Resp.ErrorCode = enErrorCode.API_LP_Success;
                    _Resp.ReturnCode = enResponseCodeService.Success;
                    _Resp.ReturnMsg = "Transaction processing Success On Coinbase";
                    return _Resp;
                }

                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = "Transaction Fail On Coinbase";

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnTradeCoinbase:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    _Resp.ReturnCode = enResponseCodeService.Success;
                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }

        private async Task<BizResponse> ProcessTransactionOnTradeUpbit(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                //UpbitGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                //UpbitGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                string LocalPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                var UpbitRes = await _upbitService.PlaceOrderAsync(LocalPair, Req.TrnType == enTrnType.Sell_Trade ? UpbitOrderSide.ask : UpbitOrderSide.bid, Req.Qty, Req.Price, Req.ordertype == enTransactionMarketType.LIMIT ? UpbitOrderType.limit : Req.ordertype == enTransactionMarketType.MARKET ? UpbitOrderType.market : Req.ordertype == enTransactionMarketType.STOP ? UpbitOrderType.price : UpbitOrderType.limit);

                IsProcessedBit = 1;
                _TransactionObj.APIResponse = JsonConvert.SerializeObject(UpbitRes);
                if (UpbitRes.state == "done")
                {
                    WebAPIParseResponseClsObj.TrnRefNo = UpbitRes.uuid.ToString();
                    WebAPIParseResponseClsObj.OperatorRefNo = UpbitRes.uuid.ToString();
                    WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;

                    _Resp.ErrorCode = enErrorCode.API_LP_Success;
                    _Resp.ReturnCode = enResponseCodeService.Success;
                    _Resp.ReturnMsg = "Transaction processing Success On Upbit";
                    return _Resp;
                }

                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = "Transaction Fail On Coinbase";

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnTradeUpbit:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    _Resp.ReturnCode = enResponseCodeService.Success;
                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }

        #region "Bitfinex Place Order"

        /// <summary>
        ///   //Add new place order for Bitfinex Exchange by Pushpraj as on 08-07-2019
        /// </summary>
        /// <param name="_Resp"></param>
        /// <param name="ServiceProConfiguration"></param>
        /// <param name="WebAPIParseResponseClsObj"></param>
        /// <param name="Provider"></param>
        /// <param name="TrnNo"></param>
        /// <param name="_TransactionObj"></param>
        /// <param name="LTP"></param>
        /// <param name="LPProcessTransactionClsObj"></param>
        /// <returns></returns>
        private async Task<BizResponse> ProcessTransactionOnBitfinex(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                //UpbitGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                //UpbitGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                string LocalPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                //var UpbitRes = await _upbitService.PlaceOrderAsync(LocalPair, Req.TrnType == enTrnType.Sell_Trade ? UpbitOrderSide.ask : UpbitOrderSide.bid, Req.Qty, Req.Price, Req.ordertype == enTransactionMarketType.LIMIT ? UpbitOrderType.limit : Req.ordertype == enTransactionMarketType.MARKET ? UpbitOrderType.market : Req.ordertype == enTransactionMarketType.STOP ? UpbitOrderType.price : UpbitOrderType.limit);
                var BitfinexRes = await _bitfinexLPService.PlaceOrder(LocalPair.Replace("_", ""), Req.Amount, Req.Price, Req.TrnType == enTrnType.Sell_Trade ? BitfinexOrderSide.sell.ToString() : BitfinexOrderSide.buy.ToString(),
                    Req.ordertype == enTransactionMarketType.LIMIT ? BitfinexOrderType.limit.ToString() : Req.ordertype == enTransactionMarketType.MARKET ? BitfinexOrderType.market.ToString() : Req.ordertype == enTransactionMarketType.STOP ? BitfinexOrderType.price.ToString() : BitfinexOrderType.limit.ToString(),
                    "bitfinex", false, false, 0, false, 0, 0, 1);

                if (BitfinexRes == null)
                {
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Fail On Bitfinex";
                    return _Resp;
                }

                _TransactionObj.APIResponse = JsonConvert.SerializeObject(BitfinexRes);
                if (BitfinexRes.order_id != 0 && BitfinexRes == null)
                {
                    if (BitfinexRes.message.ToUpper().Contains("INSUFFICIENT_FUNDS"))
                    {
                        _Resp.ErrorCode = enErrorCode.Bitfinex_INSUFFICIENT_FUNDS;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Bitfinex_INSUFFICIENT_FUNDS;
                        return _Resp;
                    }
                    else if (BitfinexRes.message.ToUpper().Contains("MIN_TRADE_REQUIREMENT_NOT_MET"))
                    {
                        _Resp.ErrorCode = enErrorCode.Bitfinex_MIN_TRADE_REQUIREMENT_NOT_MET;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = EnResponseMessage.Bitfinex_MIN_TRADE_REQUIREMENT_NOT_MET;
                        return _Resp;
                    }
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = BitfinexRes.order_id != 0 ? BitfinexRes.message : "Transaction Fail On Bitfinex";
                }
                if (BitfinexRes.order_id != 0)
                {
                    IsProcessedBit = 1;
                    BitfinexStatusCheckResponse BitfinexResult = await _bitfinexLPService.GetStatusCheck(BitfinexRes.order_id);
                    if (BitfinexResult != null)
                    {
                        if (BitfinexResult.id != 0)
                        {
                            decimal OriginalQty = (decimal.Parse(BitfinexResult.original_amount) / decimal.Parse(BitfinexResult.price));
                            decimal RemainingQty = (decimal.Parse(BitfinexResult.remaining_amount) / decimal.Parse(BitfinexResult.price));
                            if ((OriginalQty - RemainingQty) == 0)
                            {
                                LPProcessTransactionClsObj.RemainingQty = OriginalQty - RemainingQty;
                                LPProcessTransactionClsObj.SettledQty = RemainingQty - OriginalQty;
                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
                                _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                _Resp.ReturnCode = enResponseCodeService.Success;
                                _Resp.ReturnMsg = "Transaction fully Success On Bitfinex";
                            }
                            else if (RemainingQty < OriginalQty)
                            {
                                LPProcessTransactionClsObj.RemainingQty = RemainingQty;
                                LPProcessTransactionClsObj.SettledQty = RemainingQty - OriginalQty;
                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
                                _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                _Resp.ReturnCode = enResponseCodeService.Success;
                                _Resp.ReturnMsg = "Transaction partial Success On Bitfinex";
                            }
                            else if (RemainingQty == OriginalQty)
                            {
                                LPProcessTransactionClsObj.RemainingQty = RemainingQty;
                                LPProcessTransactionClsObj.SettledQty = OriginalQty;
                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
                                _Resp.ErrorCode = enErrorCode.API_LP_Hold;
                                _Resp.ReturnCode = enResponseCodeService.Success;
                                _Resp.ReturnMsg = "Transaction processing Success On Bitfinex";
                            }
                            else if (BitfinexResult.is_cancelled == true)
                            {
                                LPProcessTransactionClsObj.RemainingQty = RemainingQty;
                                LPProcessTransactionClsObj.SettledQty = OriginalQty - RemainingQty;
                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
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
                }
                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = "Transaction Fail On Bitfinex";


            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnBitfinex:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    _Resp.ReturnCode = enResponseCodeService.Success;
                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }

        #endregion


        #region "CEXIO Place Order"
        /// <summary>
        /// Add new place order for CEXIO Exchange by Pushpraj as on 13-07-2019
        /// </summary>
        /// <param name="_Resp"></param>
        /// <param name="ServiceProConfiguration"></param>
        /// <param name="WebAPIParseResponseClsObj"></param>
        /// <param name="Provider"></param>
        /// <param name="TrnNo"></param>
        /// <param name="_TransactionObj"></param>
        /// <param name="LTP"></param>
        /// <param name="LPProcessTransactionClsObj"></param>
        /// <returns></returns>
        private async Task<BizResponse> ProcessTransactionOnCEXIO(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                string LocalPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                var CEXIORes = await _cEXIOLPService.PlaceOrder(_SecondCurrService.SMSCode, _BaseCurrService.SMSCode, Req.Price, Req.TrnType == enTrnType.Sell_Trade ? CEXIOOrderSide.sell.ToString() : CEXIOOrderSide.buy.ToString(), Req.Amount);

                if (CEXIORes == null)
                {
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Fail On CEXIO";
                    return _Resp;
                }

                _TransactionObj.APIResponse = JsonConvert.SerializeObject(CEXIORes);
                //if (CEXIORes.orderId != "0" && CEXIORes == null)
                //{
                //    if (CEXIORes.message.ToUpper().Contains("INSUFFICIENT_FUNDS"))
                //    {
                //        _Resp.ErrorCode = enErrorCode.Bitfinex_INSUFFICIENT_FUNDS;
                //        _Resp.ReturnCode = enResponseCodeService.Fail;
                //        _Resp.ReturnMsg = EnResponseMessage.Bitfinex_INSUFFICIENT_FUNDS;
                //        return _Resp;
                //    }
                //    else if (BitfinexRes.message.ToUpper().Contains("MIN_TRADE_REQUIREMENT_NOT_MET"))
                //    {
                //        _Resp.ErrorCode = enErrorCode.Bitfinex_MIN_TRADE_REQUIREMENT_NOT_MET;
                //        _Resp.ReturnCode = enResponseCodeService.Fail;
                //        _Resp.ReturnMsg = EnResponseMessage.Bitfinex_MIN_TRADE_REQUIREMENT_NOT_MET;
                //        return _Resp;
                //    }
                //    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                //    _Resp.ReturnCode = enResponseCodeService.Fail;
                //    _Resp.ReturnMsg = BitfinexRes.order_id != 0 ? BitfinexRes.message : "Transaction Fail On Bitfinex";
                //}
                if (CEXIORes.orderId != "")
                {
                    IsProcessedBit = 1;
                    COpenOrderItem CEXIOResult = await _cEXIOLPService.GetStatusCheck(int.Parse(CEXIORes.orderId));
                    if (CEXIOResult != null)
                    {
                        if (CEXIOResult.orderId != "")
                        {
                            if (CEXIOResult.orderStatus.ToString() == "2")
                            {
                                LPProcessTransactionClsObj.RemainingQty = CEXIOResult.remaining - CEXIOResult.quantity;
                                LPProcessTransactionClsObj.SettledQty = CEXIOResult.quantity;
                                LPProcessTransactionClsObj.TotalQty = CEXIOResult.quantity;
                                _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                _Resp.ReturnCode = enResponseCodeService.Success;
                                _Resp.ReturnMsg = "Transaction fully Success On CEXIO";
                            }
                            else if (CEXIOResult.orderStatus.ToString() == "1")
                            {
                                LPProcessTransactionClsObj.RemainingQty = CEXIOResult.remaining - CEXIOResult.quantity;
                                LPProcessTransactionClsObj.SettledQty = CEXIOResult.quantity;
                                LPProcessTransactionClsObj.TotalQty = CEXIOResult.quantity;
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
                        _Resp.ReturnMsg = "Transaction Fail On Bitfinex";
                    }
                }
                else
                {
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Fail On CEXIO";
                }

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnCEXIO:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    _Resp.ReturnCode = enResponseCodeService.Success;
                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }
        #endregion


        private async Task<BizResponse> ProcessTransactionOnGemini(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                //UpbitGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                //UpbitGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                string LocalPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                //var UpbitRes = await _GeminiLPService.PlaceOrderAsync(LocalPair, Req.TrnType == enTrnType.Sell_Trade ? UpbitOrderSide.ask : UpbitOrderSide.bid, Req.Qty, Req.Price, Req.ordertype == enTransactionMarketType.LIMIT ? UpbitOrderType.limit : Req.ordertype == enTransactionMarketType.MARKET ? UpbitOrderType.market : Req.ordertype == enTransactionMarketType.STOP ? UpbitOrderType.price : UpbitOrderType.limit);
                var GeminiRes = await _GeminiLPService.PlaceOrderAsync(Req.TrnType == enTrnType.Sell_Trade ? Core.Interfaces.LiquidityProvider.OrderSide.Sell : Core.Interfaces.LiquidityProvider.OrderSide.Buy, LocalPair, Req.Qty, Req.Price);

                IsProcessedBit = 1;
                _TransactionObj.APIResponse = JsonConvert.SerializeObject(GeminiRes);
                if (String.IsNullOrEmpty(GeminiRes.message))
                {
                    WebAPIParseResponseClsObj.TrnRefNo = GeminiRes.order_id.ToString();
                    WebAPIParseResponseClsObj.OperatorRefNo = GeminiRes.id.ToString();
                    WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;

                    _Resp.ErrorCode = enErrorCode.API_LP_Success;
                    _Resp.ReturnCode = enResponseCodeService.Success;
                    _Resp.ReturnMsg = "Transaction processing Success On Gemini";
                    return _Resp;
                }

                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = "Transaction Fail On Gemini";

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnGemini:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    _Resp.ReturnCode = enResponseCodeService.Success;
                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }

        #region "Yobit Place Order"
        private async Task<BizResponse> ProcessTransactionOnYobit(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                YobitGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                YobitGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                string LocalPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                //var UpbitRes = await _GeminiLPService.PlaceOrderAsync(LocalPair, Req.TrnType == enTrnType.Sell_Trade ? UpbitOrderSide.ask : UpbitOrderSide.bid, Req.Qty, Req.Price, Req.ordertype == enTransactionMarketType.LIMIT ? UpbitOrderType.limit : Req.ordertype == enTransactionMarketType.MARKET ? UpbitOrderType.market : Req.ordertype == enTransactionMarketType.STOP ? UpbitOrderType.price : UpbitOrderType.limit);
                var OrderType = "";
                if (Req.TrnType == enTrnType.Sell_Trade)
                    OrderType = "Sell";
                else
                    OrderType = "Buy";

                var YobitRes = await _yobitLPService.PlaceOrder(LocalPair, OrderType, Req.Price, Req.Amount);

                IsProcessedBit = 1;
                _TransactionObj.APIResponse = JsonConvert.SerializeObject(YobitRes);
                if (YobitRes == null)
                {
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Fail On Yobit";
                    return _Resp;
                }
                if (YobitRes.tradeResult != null)
                {
                    IsProcessedBit = 1;
                    YobitAPIReqRes.ExchangeOrderResult YobitResult = await _yobitLPService.GetLPStatusCheck(YobitRes.tradeResult.order_id.ToString(), Req.SMSCode);
                    decimal OriginalQty = YobitResult.Amount / YobitResult.Price;
                    decimal FilledQty = YobitResult.AmountFilled / YobitResult.Price;
                    if (YobitResult.Result == YobitAPIReqRes.ExchangeAPIOrderResult.Filled)
                    {
                        LPProcessTransactionClsObj.RemainingQty = OriginalQty - FilledQty;
                        LPProcessTransactionClsObj.SettledQty = FilledQty;
                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction fully Success On Yobit";
                    }
                    else if (YobitResult.Result == YobitAPIReqRes.ExchangeAPIOrderResult.FilledPartially)
                    {
                        LPProcessTransactionClsObj.RemainingQty = OriginalQty - FilledQty;
                        LPProcessTransactionClsObj.SettledQty = FilledQty;
                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction partial Success On Yobit";
                    }
                    else if (YobitResult.Result == YobitAPIReqRes.ExchangeAPIOrderResult.Canceled)
                    {
                        LPProcessTransactionClsObj.RemainingQty = OriginalQty - FilledQty;
                        LPProcessTransactionClsObj.SettledQty = FilledQty;
                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction Fully Cancel On Yobit";
                    }
                    else if (YobitResult.Result == YobitAPIReqRes.ExchangeAPIOrderResult.PendingCancel)
                    {
                        LPProcessTransactionClsObj.RemainingQty = OriginalQty - FilledQty;
                        LPProcessTransactionClsObj.SettledQty = FilledQty;
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
                    _Resp.ReturnMsg = YobitRes.error.ToString();
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnYobit:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    _Resp.ReturnCode = enResponseCodeService.Success;
                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }

        #endregion

        #region "EXMO Place Order"
        /// <summary>
        /// Exmo Place Order method add by Pushpraj as on 17-07-2019
        /// </summary>
        /// <param name="_Resp"></param>
        /// <param name="ServiceProConfiguration"></param>
        /// <param name="WebAPIParseResponseClsObj"></param>
        /// <param name="Provider"></param>
        /// <param name="TrnNo"></param>
        /// <param name="_TransactionObj"></param>
        /// <param name="LTP"></param>
        /// <param name="LPProcessTransactionClsObj"></param>
        /// <returns></returns>
        private async Task<BizResponse> ProcessTransactionOnEXMOOLD(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                //YobitGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                //YobitGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                string LocalPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                //var UpbitRes = await _GeminiLPService.PlaceOrderAsync(LocalPair, Req.TrnType == enTrnType.Sell_Trade ? UpbitOrderSide.ask : UpbitOrderSide.bid, Req.Qty, Req.Price, Req.ordertype == enTransactionMarketType.LIMIT ? UpbitOrderType.limit : Req.ordertype == enTransactionMarketType.MARKET ? UpbitOrderType.market : Req.ordertype == enTransactionMarketType.STOP ? UpbitOrderType.price : UpbitOrderType.limit);
                var OrderType = "";
                if (Req.TrnType == enTrnType.Sell_Trade)
                    OrderType = "sell";
                else
                    OrderType = "buy";

                var EXMORes = await _eXMOLPService.PlaceOrder(LocalPair, OrderType, Req.Qty, Req.Price);

                IsProcessedBit = 1;
                _TransactionObj.APIResponse = JsonConvert.SerializeObject(EXMORes);
                if (EXMORes == null)
                {
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Fail On EXMO";
                    return _Resp;
                }

                if (EXMORes.result != null)
                {
                    //IsProcessedBit = 1;
                    //YobitAPIReqRes.ExchangeOrderResult YobitResult = await _yobitLPService.GetLPStatusCheck(EXMORes.order_id.ToString(), Req.SMSCode);
                    //decimal OriginalQty = YobitResult.Amount / YobitResult.Price;
                    //decimal FilledQty = YobitResult.AmountFilled / YobitResult.Price;
                    //if (YobitResult.Result == YobitAPIReqRes.ExchangeAPIOrderResult.Filled)
                    //{
                    //    LPProcessTransactionClsObj.RemainingQty = OriginalQty - FilledQty;
                    //    LPProcessTransactionClsObj.SettledQty = FilledQty;
                    //    _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                    //    _Resp.ReturnCode = enResponseCodeService.Success;
                    //    _Resp.ReturnMsg = "Transaction fully Success On Yobit";
                    //}
                    //else if (YobitResult.Result == YobitAPIReqRes.ExchangeAPIOrderResult.FilledPartially)
                    //{
                    //    LPProcessTransactionClsObj.RemainingQty = OriginalQty - FilledQty;
                    //    LPProcessTransactionClsObj.SettledQty = FilledQty;
                    //    _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                    //    _Resp.ReturnCode = enResponseCodeService.Success;
                    //    _Resp.ReturnMsg = "Transaction partial Success On Yobit";
                    //}
                    //else if (YobitResult.Result == YobitAPIReqRes.ExchangeAPIOrderResult.Canceled)
                    //{
                    //    LPProcessTransactionClsObj.RemainingQty = OriginalQty - FilledQty;
                    //    LPProcessTransactionClsObj.SettledQty = FilledQty;
                    //    _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                    //    _Resp.ReturnCode = enResponseCodeService.Success;
                    //    _Resp.ReturnMsg = "Transaction Fully Cancel On Yobit";
                    //}
                    //else if (YobitResult.Result == YobitAPIReqRes.ExchangeAPIOrderResult.PendingCancel)
                    //{
                    //    LPProcessTransactionClsObj.RemainingQty = OriginalQty - FilledQty;
                    //    LPProcessTransactionClsObj.SettledQty = FilledQty;
                    //    _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                    //    _Resp.ReturnCode = enResponseCodeService.Success;
                    //    _Resp.ReturnMsg = "Transaction Cancellation On Yobit";
                    //}
                    //else
                    //{
                    //    _Resp.ErrorCode = enErrorCode.API_LP_Success;
                    //    _Resp.ReturnCode = enResponseCodeService.Success;
                    //    _Resp.ReturnMsg = "No update";
                    //}
                }
                else
                {
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = EXMORes.error.ToString();
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnYobit:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    _Resp.ReturnCode = enResponseCodeService.Success;
                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }
        #endregion

        private async Task<BizResponse> ProcessTransactionOnEXMO(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                EXMOGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                EXMOGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                string LocalPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                var EXMORes = await _eXMOLPService.PlaceOrder(Req.TrnType == enTrnType.Sell_Trade ? Core.Interfaces.LiquidityProvider.OrderSide.Sell : Core.Interfaces.LiquidityProvider.OrderSide.Buy, LocalPair, Req.Qty, Req.Price);

                IsProcessedBit = 1;
                _TransactionObj.APIResponse = JsonConvert.SerializeObject(EXMORes);
                if (String.IsNullOrEmpty(EXMORes.error))
                {
                    WebAPIParseResponseClsObj.TrnRefNo = EXMORes.order_id.ToString();
                    WebAPIParseResponseClsObj.OperatorRefNo = "0";
                    WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;

                    _Resp.ErrorCode = enErrorCode.API_LP_Success;
                    _Resp.ReturnCode = enResponseCodeService.Success;
                    _Resp.ReturnMsg = "Transaction processing Success On EXMO";
                    return _Resp;
                }

                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = "Transaction Fail On EXMO";

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnEXMO:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    _Resp.ReturnCode = enResponseCodeService.Success;
                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }

        public long InsertTransactionRequest(TransactionProviderResponseV1 listObj, string Request)
        {
            try
            {
                NewtransactionRequest = new TransactionRequest()
                {
                    TrnNo = Req.TrnNo,
                    ServiceID = listObj.ServiceID,
                    SerProID = listObj.ServiceProID,
                    SerProDetailID = listObj.SerProDetailID,
                    CreatedDate = Helpers.UTC_To_IST(),
                    RequestData = Request
                };
                NewtransactionRequest = _TransactionRequest.Add(NewtransactionRequest);
                return NewtransactionRequest.Id;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertTransactionRequest:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return 0;
            }
        }


        private async Task<BizResponse> ProcessTransactionOnTradekrakenAsync(BizResponse _Resp, ServiceProConfiguration serviceProConfiguration, WebAPIParseResponseCls webAPIParseResponseClsObj, TransactionProviderResponseV1 provider, long trnNo, ProcessTransactionCls transactionObj, decimal lTP, LPProcessTransactionCls lPProcessTransactionClsObj)
        // pushpraj bhai
        //private async Task<BizResponse> ProcessTransactionOnTradekrakenAsync(BizResponse _Resp, ServiceProConfiguration serviceProConfiguration, WebAPIParseResponseCls webAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long trnNo, ProcessTransactionCls transactionObj, decimal lTP, LPProcessTransactionCls lPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {
                // khushali 13-07-2019 Add dynamic key and secret key configuartion

                KrakenGlobalSettings.API_Key = serviceProConfiguration.APIKey;
                KrakenGlobalSettings.Secret = serviceProConfiguration.SecretKey;


                //string LocalPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode; 
                string LocalPair = "EOS-USD"; // pending - ADD dynamic configuration for kraken - khushali 13-07-2019 for deynamic changes
                string type = " ";
                if (Convert.ToInt32(Req.TrnType) == 4)
                    type = "buy";
                else if (Convert.ToInt32(Req.TrnType) == 5)
                    type = "sell";


                //Req.Qty = NormalizeDouble(Req.Qty / 100000, 5);//m_stdLot=100000


                KrakenPlaceOrderResponse Result = await _krakenLPService.PlaceOrderAsyn(LocalPair, type, Req.ordertype.ToString(), Req.Qty, Req.Price);

                if (Result.error.Count > 0)
                {
                    for (int i = 0; i <= Result.error.Count - 1; i++)
                    {
                        var Message = Result.error[i].ToString();
                        if (Message.Contains("EGeneral:Invalid arguments"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_INVALID_ARGUMENTS;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_INVALID_ARGUMENTS;
                            return _Resp;
                        }
                        else if (Message.Contains("Unavailable"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_UNAVAILABLE;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_UNAVAILABLE;
                            return _Resp;
                        }
                        else if (Message.Contains("Invalid request"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_INVALID_REQUEST;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_INVALID_REQUEST;
                            return _Resp;
                        }
                        else if (Message.Contains("Cannot open position"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_CANNOT_OPEN_POSITION;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_CANNOT_OPEN_POSITION;
                            return _Resp;
                        }
                        else if (Message.Contains("Cannot open opposing position"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_CANNOT_OPEN_OPPOSING_POSITION;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_CANNOT_OPEN_OPPOSING_POSITION;
                            return _Resp;
                        }
                        else if (Message.Contains("Margin allowance exceeded"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_MARGIN_ALLOWED_EXCEEDED;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_MARGIN_ALLOWED_EXCEEDED;
                            return _Resp;
                        }
                        else if (Message.Contains("Margin level too low"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_MARGIN_LEVEL_TO_LOW;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_MARGIN_LEVEL_TO_LOW;
                            return _Resp;
                        }
                        else if (Message.Contains("Insufficient margin"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_INSUFFICIENT_MARGIN;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_INSUFFICIENT_MARGIN;
                            return _Resp;
                        }
                        else if (Message.Contains("Insufficient funds"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_INSUFFICIENT_FUNDS;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_INSUFFICIENT_FUNDS;
                            return _Resp;
                        }
                        else if (Message.Contains("Order minimum not met"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_ORDER_MINIMUM_NOT_MET;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_ORDER_MINIMUM_NOT_MET;
                            return _Resp;
                        }
                        else if (Message.Contains("Orders limit exceeded"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_ORDER_LIMIT_EXCEEDED;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_ORDER_LIMIT_EXCEEDED;
                            return _Resp;
                        }
                        else if (Message.Contains("Positions limit exceeded"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_POSITION_LIMIT_EXCEEDED;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_POSITION_LIMIT_EXCEEDED;
                            return _Resp;
                        }
                        else if (Message.Contains("Rate limit exceeded"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_RATE_LIMIT_EXCEEDED;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_RATE_LIMIT_EXCEEDED;
                            return _Resp;
                        }
                        else if (Message.Contains("Scheduled orders limit exceeded"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_SCHEDULE_ORDER_LIMIT_EXCEEDED;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_SCHEDULE_ORDER_LIMIT_EXCEEDED;
                            return _Resp;
                        }
                        else if (Message.Contains("Unknown position"))
                        {
                            _Resp.ErrorCode = enErrorCode.Kraken_UNKNOWN_POSITION;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = EnResponseMessage.Kraken_UNKNOWN_POSITION;
                            return _Resp;
                        }
                        else
                        {
                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = Message.ToString();
                            return _Resp;
                        }
                    }
                }
                if (Result.result != null)
                {

                    if (Result.transactionIds != null)
                    {

                        IsProcessedBit = 1;

                        _TransactionObj.APIResponse = JsonConvert.SerializeObject(Result);
                        //KMyOrderItem KrakenResult = await _krakenLPService.GetLPStatusCheck(false, Result.description.order, "");
                        KMyOrderItem KrakenResult = await _krakenLPService.GetLPStatusCheck(false, Result.transactionIds[0].ToString(), Result.transactionIds[0].ToString());
                        if (KrakenResult != null)
                        {

                            if (KrakenResult.orderId != null)
                            {
                                if (KrakenResult.status == "Partially")
                                {
                                    // updateddata.MakeTransactionSuccess();
                                    lPProcessTransactionClsObj.RemainingQty = KrakenResult.amount - KrakenResult.filled;
                                    lPProcessTransactionClsObj.SettledQty = KrakenResult.filled;
                                    lPProcessTransactionClsObj.TotalQty = Req.Amount;
                                    _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                    _Resp.ReturnCode = enResponseCodeService.Success;
                                    _Resp.ReturnMsg = "Transaction fully Success On Kraken";
                                }
                                else if (KrakenResult.status == "closed")
                                {
                                    // updateddata.MakeTransactionHold();
                                    lPProcessTransactionClsObj.RemainingQty = 0;
                                    lPProcessTransactionClsObj.SettledQty = KrakenResult.quantity;
                                    lPProcessTransactionClsObj.TotalQty = Req.Amount;
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
                            _Resp.ReturnMsg = "Transaction Fail On Kraken";
                        }
                    }
                    else
                    {
                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "No Transaction created for Kraken.";
                    }
                }
                else
                {

                    _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                    _Resp.ReturnCode = enResponseCodeService.Success;
                    _Resp.ReturnMsg = "Transaction Success On Kraken";
                }

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionKraken:##TrnNo " + trnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                {
                    if (webAPIParseResponseClsObj.Status == enTransactionStatus.Success)
                    {
                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction Success On Kraker";
                        return _Resp;
                    }
                    else
                    {
                        _Resp.ReturnCode = enResponseCodeService.Success;
                    }
                }
                else
                {
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                    _Resp.ReturnMsg = ex.Message;
                    _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
                }
            }
            return _Resp;

        }

        private decimal NormalizeDouble(decimal v1, int v2)
        {

            return v1 * v2;
        }

        #region OKEX Place Order 
        /// <summary>
        /// Add new Place order for OKEX API by Pushpraj as on 17-06-2019
        /// </summary>
        /// <param name="_Resp"></param>
        /// <param name="ServiceProConfiguration"></param>
        /// <param name="WebAPIParseResponseClsObj"></param>
        /// <param name="Provider"></param>
        /// <param name="TrnNo"></param>
        /// <param name="_TransactionObj"></param>
        /// <param name="LTP"></param>
        /// <param name="LPProcessTransactionClsObj"></param>
        /// <returns></returns>
        private async Task<BizResponse> ProcessTransactionOnOKEX(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponseV1 Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
        {
            short IsProcessedBit = 0;
            try
            {

                OKEXGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
                OKEXGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
                OKEXGlobalSettings.PassPhrase = ServiceProConfiguration.Param1;
                string OKEXPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
                /// commented -- actual code
                //SubmitOrderReturn Tradestresult = _tradeSatoshiLPService.PlaceOrderAsync(Req.TrnType == enTrnType.Sell_Trade ? Core.Interfaces.LiquidityProvider.OrderSide.Sell : Core.Interfaces.LiquidityProvider.OrderSide.Buy, TradeSatoshiPair, Req.Qty, LTP).Result;

                OKExPlaceOrderReturn OKExResult = _oKExLPService.PlaceOrderAsync(OKEXPair, Req.TrnType == enTrnType.Sell_Trade ? Core.Interfaces.LiquidityProvider.OrderSide.Sell.ToString().ToLower() : Core.Interfaces.LiquidityProvider.OrderSide.Buy.ToString().ToLower(), LTP, Req.Qty, 10, "y12233456", "0", "limit").Result;

                //var Result = @" {""success"":true,""message"":null,""result"":{""orderId"":140876176,""filled"":[21474705,21474706,21474707,21474708,21474709,21474710,21474711,21474712,21474713,21474714,21474715,21474716,21474717,21474718,21474719,21474720,21474721,21474722,21474723]}}";
                //SubmitOrderReturn TradeSatoshiResult = JsonConvert.DeserializeObject<SubmitOrderReturn>(Result);

                _TransactionObj.APIResponse = JsonConvert.SerializeObject(OKExResult);
                if (OKExResult.result == true)
                {
                    IsProcessedBit = 1;
                    WebAPIParseResponseClsObj.TrnRefNo = OKExResult.order_id.ToString();
                    WebAPIParseResponseClsObj.OperatorRefNo = OKExResult.order_id.ToString();
                    if (OKExResult.order_id != null || OKExResult.order_id != "-1")
                    {
                        OKExGetOrderInfoReturn OKExOrderReturn = await _oKExLPService.GetOrderInfoAsync(OKEXPair, OKExResult.order_id, OKExResult.client_oid);

                        //var Result1 = @" {""success"":true,""message"":null,""result"":{""id"":140876176,""market"":""ETH_BTC"",""type"":""Sell"",""amount"":0.01544508,""rate"":0.03198508,""remaining"":0.01094647,""total"":0.00049401,""status"":""Partial"",""timestamp"":""2019-05-29T11:16:11.527"",""isApi"":true}}";
                        //GetOrderReturn TradeSatoshiResult1 = JsonConvert.DeserializeObject<GetOrderReturn>(Result1);

                        //GetOrderReturn TradeSatoshiResult1 = await _tradeSatoshiLPService.GetOrderInfoAsync(TradeSatoshiResult.result.OrderId);
                        if (OKExOrderReturn != null)
                        {
                            if (OKExOrderReturn.status == "2")
                            {
                                LPProcessTransactionClsObj.RemainingQty = Req.Qty - decimal.Parse(OKExOrderReturn.filled_qty);
                                LPProcessTransactionClsObj.SettledQty = decimal.Parse(OKExOrderReturn.filled_qty);
                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
                                WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;
                                _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                                _Resp.ReturnCode = enResponseCodeService.Success;
                                _Resp.ReturnMsg = "Transaction fully Success On OKEX";
                                return _Resp;
                            }
                            else if (OKExOrderReturn.status == "1")
                            {
                                LPProcessTransactionClsObj.RemainingQty = Req.Qty - decimal.Parse(OKExOrderReturn.filled_qty);
                                LPProcessTransactionClsObj.SettledQty = Req.Qty - decimal.Parse(OKExOrderReturn.filled_qty) - (Req.Qty - decimal.Parse(OKExOrderReturn.filled_qty));
                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
                                WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
                                _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
                                _Resp.ReturnCode = enResponseCodeService.Success;
                                _Resp.ReturnMsg = "Transaction partial Success On OKEX";
                                return _Resp;
                            }
                            else
                            {
                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ReturnMsg = "Transaction Fail On OKEX";
                                return _Resp;
                            }
                        }
                        else
                        {
                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = OKExResult.error_message;
                            return _Resp;
                        }
                    }

                }
                else
                {
                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = OKExResult.error_message;
                    return _Resp;
                }

            }

            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessTransactionOnOKEX:##TrnNo " + TrnNo, ControllerName, ex);
                if (IsProcessedBit == 1)//Does not proceed on next API
                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
                    {
                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Transaction Success On OKEX";
                        return _Resp;
                    }
                    else
                    {
                        _Resp.ReturnCode = enResponseCodeService.Success;
                    }
                else
                    _Resp.ReturnCode = enResponseCodeService.Fail;

                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
            }
            return _Resp;
        }

        #endregion

        #region Send SMS And Email
        public async Task SMSSendTransactionHoldOrFailed(long TrnNo, string MobileNumber, decimal Price, decimal Qty, int type)
        {
            try
            {
                if (!string.IsNullOrEmpty(MobileNumber))
                {
                    TemplateMasterData SmsData = new TemplateMasterData();
                    SendSMSRequest SendSMSRequestObj = new SendSMSRequest();
                    ApplicationUser User = new ApplicationUser();

                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    communicationParamater.Param1 = TrnNo + "";
                    communicationParamater.Param2 = Price + "";
                    communicationParamater.Param3 = Qty + "";

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SendSMSTransaction - SMSSendTransactionHoldOrFailed", ControllerName, " ##TrnNo : " + TrnNo + " ##MobileNo : " + MobileNumber + " ##Price : " + Price + " ##Qty : " + Qty + " ##Type : " + type, Helpers.UTC_To_IST()));

                    if (type == 1) // Transaction Created
                    {
                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_TransactionCreated, communicationParamater, enCommunicationServiceType.SMS).Result;
                    }
                    else if (type == 2) // Transaction Failed
                    {
                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_TransactionFailed, communicationParamater, enCommunicationServiceType.SMS).Result;
                    }

                    if (SmsData != null)
                    {
                        if (SmsData.IsOnOff == 1)
                        {
                            SendSMSRequestObj.Message = SmsData.Content;
                            SendSMSRequestObj.MobileNo = Convert.ToInt64(MobileNumber);
                            _pushSMSQueue.Enqueue(SendSMSRequestObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("SMSSendTransactionHold:##TrnNo " + TrnNo, ControllerName, ex));
            }
        }

        public async Task EmailSendTransactionHoldOrFailed(long TrnNo, string UserId, long pairid, decimal qty, string datetime, decimal price, decimal fee, int Type, short OrderType, short TrnType)
        {
            try
            {
                SendEmailRequest Request = new SendEmailRequest();
                ApplicationUser User = new ApplicationUser();
                TemplateMasterData EmailData = new TemplateMasterData();
                CommunicationParamater communicationParamater = new CommunicationParamater();

                User = await _userManager.FindByIdAsync(UserId);
                if (!string.IsNullOrEmpty(User.Email))
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SendEmailTransaction - EmailSendWithdrwalTransaction", ControllerName, " ##TrnNo : " + TrnNo + " ##Type : " + Type, Helpers.UTC_To_IST()));

                    var pairdata = _trnMasterConfiguration.GetTradePairMaster().Where(x => x.Id == pairid).FirstOrDefault();

                    if (pairdata != null)
                    {
                        communicationParamater.Param1 = pairdata.PairName + "";
                        communicationParamater.Param3 = pairdata.PairName.Split("_")[1];
                    }

                    communicationParamater.Param8 = User.Name + "";
                    communicationParamater.Param2 = Helpers.DoRoundForTrading(qty, 8).ToString();
                    communicationParamater.Param4 = datetime;
                    communicationParamater.Param5 = Helpers.DoRoundForTrading(price, 8).ToString();
                    communicationParamater.Param6 = Helpers.DoRoundForTrading(fee, 8).ToString();
                    communicationParamater.Param7 = Helpers.DoRoundForTrading(0, 8).ToString();  //Uday 01-01-2019  In failed transaction final price as 0
                    communicationParamater.Param9 = ((enTransactionMarketType)OrderType).ToString();  //Uday 01-01-2019 Add OrderType In Email
                    communicationParamater.Param10 = ((enTrnType)TrnType).ToString();  //Uday 01-01-2019 Add TranType In Email
                    communicationParamater.Param11 = TrnNo.ToString(); //Uday 01-01-2019 Add TrnNo In Email

                    //if (CancelType == 1) // Hold
                    //{
                    //    EmailData = _messageService.SendMessageAsync(EnTemplateType.EMAIL_OrderCancel, communicationParamater, enCommunicationServiceType.Email).Result;
                    //}
                    if (Type == 2) // Failed
                    {
                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_TransactionFailed, communicationParamater, enCommunicationServiceType.Email).Result;
                    }

                    if (EmailData != null)
                    {
                        Request.Body = EmailData.Content;
                        Request.Subject = EmailData.AdditionalInfo;
                        Request.Recepient = User.Email;
                        Request.EmailType = 0;
                        _pushNotificationsQueue.Enqueue(Request);
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("EmailSendCancelTransaction:##TrnNo " + TrnNo, ControllerName, ex));
            }
        }
        #endregion
    }
}
