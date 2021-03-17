//using Binance.Net.Objects;
//using Bittrex.Net.Objects;
//using Worldex.Core.ApiModels;
//using Worldex.Core.ApiModels.Chat;
//using Worldex.Core.Entities;
//using Worldex.Core.Entities.Configuration;
//using Worldex.Core.Entities.Transaction;
//using Worldex.Core.Entities.User;
//using Worldex.Core.Enums;
//using Worldex.Core.Helpers;
//using Worldex.Core.Interfaces;
//using Worldex.Core.Interfaces.LiquidityProvider;
//using Worldex.Core.Interfaces.Repository;
//using Worldex.Core.ViewModels;
//using Worldex.Core.ViewModels.LiquidityProvider;
//using Worldex.Core.ViewModels.LiquidityProvider1;
//using Worldex.Core.ViewModels.Wallet;
//using Worldex.Core.ViewModels.WalletOperations;
//using Worldex.Infrastructure.BGTask;
//using Worldex.Infrastructure.DTOClasses;
//using Worldex.Infrastructure.Interfaces;
//using Worldex.Infrastructure.LiquidityProvider;
//using Worldex.Infrastructure.LiquidityProvider.TradeSatoshiAPI;
//using CoinbasePro.Services.Orders.Models.Responses;
//using CryptoExchange.Net.Objects;
//using Huobi.Net.Objects;
//using MediatR;
//using Microsoft.AspNetCore.Identity;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Worldex.Infrastructure.LiquidityProvider.OKExAPI;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Configuration;
//using Worldex.Core.ViewModels.Transaction.Arbitrage;

//namespace Worldex.Infrastructure.Services.Transaction
//{
//    public class NewTransactionArbitrageV1 : ITransactionProcessArbitrageV1
//    {
//        private readonly ICommonRepository<TransactionQueueArbitrage> _TransactionRepository;
//        private readonly ICommonRepository<TradeTransactionQueueArbitrage> _TradeTransactionRepository;
//        private readonly ICommonRepository<TradeStopLossArbitrage> _TradeStopLoss;
//        private readonly ICommonRepository<TradeSellerListArbitrageV1> _TradeSellerList;
//        private readonly ICommonRepository<TradeBuyerListArbitrageV1> _TradeBuyerList;
//        private readonly ICommonRepository<TradePairStasticsArbitrage> _tradePairStastics;
//        private readonly IArbitrageWalletService _WalletService;
//        private readonly IWebApiRepository _WebApiRepository;
//        private readonly ISettlementRepositoryArbitrageV1<BizResponse> _SettlementRepositoryV1;
//        private readonly ISettlementRepositoryArbitrageAPI<BizResponse> _SettlementRepositoryAPI;
//        private readonly ISignalRService _ISignalRService;
//        private readonly ITrnMasterConfiguration _trnMasterConfiguration;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly IMessageService _messageService;
//        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
//        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
//        private readonly IMediator _mediator;
//        private readonly ICommonWalletFunction _commonWalletFunction;
//        private readonly IFrontTrnRepository _frontTrnRepository; // khushali 24-05-2019 for LTP watcher
//        private readonly ICommonRepository<AllowesOrderTypeArbitrage> _allowesOrderTypeArbitrageRepository; // khushali 27-05-2019 for  allow order type for  provider validation
//        public BizResponse _Resp;
//        public TradePairMasterArbitrage _TradePairObj;
//        public TradePairDetailArbitrage _TradePairDetailObj;
//        public List<TransactionProviderArbitrageResponse> TxnProviderList;
//        TransactionQueueArbitrage Newtransaction;
//        TradeTransactionQueueArbitrage NewTradetransaction;
//        NewTransactionRequestArbitrageCls Req;
//        NewTradeTransactionRequestCls _TradeTransactionObj = new NewTradeTransactionRequestCls();
//        ServiceMasterArbitrage _BaseCurrService;
//        ServiceMasterArbitrage _SecondCurrService;
//        TradeSellerListArbitrageV1 TradeSellerListObj;
//        TradeBuyerListArbitrageV1 TradeBuyerListObj;
//        TradeStopLossArbitrage TradeStopLossObj;
//        private string ControllerName = "TradingTransactionArbitrageV1";
//        short STOPLimitWithSameLTP = 0;
//        //Routing using LP
//        private readonly IWebApiData _IWebApiData;
//        private readonly IGetWebRequest _IGetWebRequest;
//        private readonly ICommonRepository<ArbitrageTransactionRequest> _TransactionRequest;
//        private readonly BinanceLPService _binanceLPService;
//        private readonly BitrexLPService _bitrexLPService;
//        private readonly ICoinBaseService _coinBaseService;
//        private readonly IPoloniexService _poloniexService;
//        private readonly ITradeSatoshiLPService _tradeSatoshiLPService;
//        private readonly IOKExLPService _oKExLPService; //Add new variable for OKEx API by Pushpraj as on 19-06-2019
//        private readonly HuobiLPService _huobiLPService;
//        WebApiParseResponse _WebApiParseResponseObj;
//        ArbitrageTransactionRequest NewtransactionRequest;
//        //ProcessTransactionCls _TransactionObj; 
//        //private readonly IResdisTradingManagment _IResdisTradingManagment;//Rita 15-3-19 added for Site Token conversion
//        private readonly ICommonRepository<TradingConfiguration> _tradingConfiguration; // khushali 25-05-2019 Trading configuration Type
//        public List<TradingConfiguration> TradingConfigurationList;
//        int IsOnlyArbitrageTradeOn = 0;
//        //int IsMaxProfit = 0;
//        //int IsProceedInLocal = 0;
//        //short IsAPITrade = 0;
//        private IMemoryCache _cache;
//        TransactionProviderArbitrageResponse Provider;
//        private readonly ICommonRepository<ArbitrageTradingAllowToUser> _ArbitrageTradingAllowToUser;
//        short SmaartTradePriority = 0;
//        private readonly IWebApiSendRequest _IWebApiSendRequest;
//        WebAPIParseResponseCls WebAPIParseResponseClsObj = new WebAPIParseResponseCls();
//        private readonly IConfiguration _configuration;

//        public NewTransactionArbitrageV1(
//            ICommonRepository<TransactionQueueArbitrage> TransactionRepository,
//            ICommonRepository<TradeTransactionQueueArbitrage> TradeTransactionRepository,
//            ICommonRepository<TradeStopLossArbitrage> tradeStopLoss, IArbitrageWalletService WalletService, IWebApiRepository WebApiRepository,
//            ICommonRepository<TradeSellerListArbitrageV1> TradeSellerList,
//            ICommonRepository<TradeBuyerListArbitrageV1> TradeBuyerList, ISettlementRepositoryArbitrageV1<BizResponse> SettlementRepositoryV1,
//            ISignalRService ISignalRService, ICommonRepository<TradePairStasticsArbitrage> tradePairStastics,//IServiceProvider services, 
//            ITrnMasterConfiguration trnMasterConfiguration, UserManager<ApplicationUser> userManager, IMessageService messageService,
//            IPushNotificationsQueue<SendSMSRequest> pushSMSQueue, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, IMediator mediator,
//            ICommonWalletFunction commonWalletFunction, BinanceLPService BinanceLPService, BitrexLPService BitrexLPService,
//            ICoinBaseService CoinBaseService, IPoloniexService PoloniexService, ITradeSatoshiLPService TradeSatoshiLPService,
//            IWebApiData IWebApiData, IGetWebRequest IGetWebRequest, IWebApiSendRequest WebApiSendRequest,
//            WebApiParseResponse WebApiParseResponseObj, ICommonRepository<ArbitrageTransactionRequest> TransactionRequest,
//             IFrontTrnRepository FrontTrnRepository, //IResdisTradingManagment IResdisTradingManagment,
//            ICommonRepository<TradingConfiguration> TradingConfiguration,
//            ISettlementRepositoryArbitrageAPI<BizResponse> SettlementRepositoryAPI, HuobiLPService HuobiLPService, IOKExLPService oKExLPService,
//             IMemoryCache cache, IWebApiSendRequest IWebApiSendRequest, ICommonRepository<AllowesOrderTypeArbitrage> AllowesOrderTypeArbitrageRepository,
//             IConfiguration configuration,ICommonRepository<ArbitrageTradingAllowToUser> ArbitrageTradingAllowToUser)
//        {
//            _TransactionRepository = TransactionRepository;
//            _TradeTransactionRepository = TradeTransactionRepository;
//            _TradeStopLoss = tradeStopLoss;
//            _WalletService = WalletService;
//            _WebApiRepository = WebApiRepository;
//            _TradeSellerList = TradeSellerList;
//            _TradeBuyerList = TradeBuyerList;
//            _SettlementRepositoryV1 = SettlementRepositoryV1;
//            _ISignalRService = ISignalRService;
//            _tradePairStastics = tradePairStastics;
//            //Services = services;
//            _trnMasterConfiguration = trnMasterConfiguration;
//            _userManager = userManager;
//            _messageService = messageService;
//            _pushSMSQueue = pushSMSQueue;
//            _pushNotificationsQueue = pushNotificationsQueue;
//            _mediator = mediator;
//            _commonWalletFunction = commonWalletFunction;
//            // khushali khushali liquidity 
//            _IGetWebRequest = IGetWebRequest;
//            _IWebApiData = IWebApiData;
//            _WebApiParseResponseObj = WebApiParseResponseObj;
//            _TransactionRequest = TransactionRequest;
//            _binanceLPService = BinanceLPService;
//            _bitrexLPService = BitrexLPService;
//            _coinBaseService = CoinBaseService;
//            _poloniexService = PoloniexService;
//            _tradeSatoshiLPService = TradeSatoshiLPService;
//            _huobiLPService = HuobiLPService;
//            //_IResdisTradingManagment = IResdisTradingManagment;
//            _frontTrnRepository = FrontTrnRepository;
//            _tradingConfiguration = TradingConfiguration;
//            _SettlementRepositoryAPI = SettlementRepositoryAPI;
//            _oKExLPService = oKExLPService; // Add new variable assignment for OKEx API by Pushpraj as on 19-06-2019
//            _cache = cache;
//            _ArbitrageTradingAllowToUser = ArbitrageTradingAllowToUser;
//            _IWebApiSendRequest = IWebApiSendRequest;
//            _allowesOrderTypeArbitrageRepository = AllowesOrderTypeArbitrageRepository;
//            _configuration = configuration;
//        }
//        public async Task<BizResponse> ProcessNewTransactionArbitrageAsync(NewTransactionRequestArbitrageCls Req1)
//        {
//            Req = Req1;           

//            _Resp = await CreateTransaction();
//            if (_Resp.ReturnCode != enResponseCodeService.Success)
//            {
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessNewTransactionAsync", ControllerName, _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo + " GUID:" + Req.GUID, Helpers.UTC_To_IST()));
//                return _Resp;
//            }
//            _Resp = await CombineAllInitTransactionAsync();

//            return _Resp;
//        }

//        public async Task<BizResponse> CombineAllInitTransactionAsync()
//        {
//            _Resp = new BizResponse();
//            try
//            {                 
//                //Deduct balance here
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync Wallet", ControllerName, "Balance Deduction Start " + Req.GUID + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));

//                var DebitResult1 = _WalletService.GetArbitrageWalletHoldNew(Req.SMSCode, Helpers.GetTimeStamp(), Req.Amount,
//                    Req.DebitAccountID, Req.TrnNo, enServiceType.Trading, Req.TrnType == enTrnType.Buy_Trade ? enWalletTrnType.BuyTrade : enWalletTrnType.SellTrade, Req.TrnType, (EnAllowedChannels)Req.TrnMode, Req.accessToken, (enWalletDeductionType)((short)Req.ordertype)); //NTRIVEDI 07-12-2018               

//                //TradingDataInsert(_Resp);
//                var DebitResult = await DebitResult1;
//                //2019-4-29 addedd charge 
//                Newtransaction.ChargeRs = DebitResult.Charge;
//                Newtransaction.ChargeCurrency = DebitResult.ChargeCurrency;
//                //var DebitResult = await DebitResults;
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync Wallet", ControllerName, "Deduction End" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));

//                if (DebitResult.ReturnCode != enResponseCode.Success)
//                {
//                    _Resp.ReturnMsg = DebitResult.ReturnMsg;//EnResponseMessage.ProcessTrn_WalletDebitFailMsg;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ErrorCode = DebitResult.ErrorCode;//enErrorCode.ProcessTrn_WalletDebitFail;
//                    MarkTransactionSystemFail(_Resp.ReturnMsg, _Resp.ErrorCode);
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "Balance Deduction Fail" + _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
//                    return _Resp;
//                }

//                if (Provider.LPType != (short)enAppType.COINTTRADINGLocal)//Rita 24-6-19 do not hold for Local trading
//                {
//                    HelperForLog.WriteLogIntoFileAsyncDtTm("LP balance Hold START ", ControllerName, "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST());

//                    //Make Provider balance hold as goes in LP
//                    LPHoldDr WalletProviderHoldObj = new LPHoldDr();
//                    WalletProviderHoldObj.SerProID = Provider.ProviderID;
//                    WalletProviderHoldObj.CoinName = Req.SMSCode;
//                    WalletProviderHoldObj.Timestamp = Helpers.GetTimeStamp();
//                    WalletProviderHoldObj.Amount = Req.Amount;
//                    WalletProviderHoldObj.TrnRefNo = Req.TrnNo;
//                    WalletProviderHoldObj.trnType = Req.TrnType == enTrnType.Buy_Trade ? enWalletTrnType.BuyTrade : enWalletTrnType.SellTrade;
//                    WalletProviderHoldObj.PairId = Req.PairID;//2019-6-28 vsolanki addredd resquest param pair id for hold sp
//                    WalletProviderHoldObj.enWalletDeductionType = enWalletDeductionType.Normal;


//                    WalletDrCrResponse WalletProviderHoldResp = await _WalletService.ArbitrageLPGetWalletHoldNew(WalletProviderHoldObj);
//                    HelperForLog.WriteLogIntoFileAsyncDtTm("LP Balance hold END response ", ControllerName, "##ErrorCode:" + WalletProviderHoldResp.ErrorCode + " ##ReturnCode:" + WalletProviderHoldResp.ReturnCode + " ##ReturnMsg:" + WalletProviderHoldResp.ReturnMsg + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST());

//                    if (WalletProviderHoldResp.ReturnCode != enResponseCode.Success)
//                    {
//                        //release wallet call , Rita 26-6-19 if LP fail then release member's balance
//                        HelperForLog.WriteLogIntoFileAsyncDtTm("LP balance HOLD fail so release Member's balance START ", ControllerName, "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST());
//                        Task<WalletDrCrResponse> WalletResult = _WalletService.ArbitrageGetReleaseHoldNew(NewTradetransaction.Order_Currency, Helpers.GetTimeStamp(), Req.Amount,
//                                                     NewTradetransaction.OrderAccountID, NewTradetransaction.TrnNo, enServiceType.Trading,
//                                                     enWalletTrnType.ReleaseBlockAmount, (enTrnType)Newtransaction.TrnType, (EnAllowedChannels)Newtransaction.TrnMode,
//                                                     Req.accessToken);
//                        var WalletResp = await WalletResult;
//                        HelperForLog.WriteLogIntoFileAsyncDtTm("LP balance HOLD fail so release Member's balance END ", ControllerName, "##ErrorCode:" + WalletResp.ErrorCode + " ##ReturnCode:" + WalletResp.ReturnCode + " ##ReturnMsg:" + WalletResp.ReturnMsg + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST());

//                        _Resp.ReturnMsg = WalletProviderHoldResp.ReturnMsg;//EnResponseMessage.ProcessTrn_WalletDebitFailMsg;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ErrorCode = WalletProviderHoldResp.ErrorCode;//enErrorCode.ProcessTrn_WalletDebitFail;
//                        MarkTransactionSystemFail(_Resp.ReturnMsg, _Resp.ErrorCode);
//                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "LP Balance Deduction Fail" + _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
//                        return _Resp;
//                    }
//                }

//                //===================================Make txn HOLD as balance debited=======================               


//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "HOLD Start" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
//                MarkTransactionHold(EnResponseMessage.ProcessTrn_HoldMsg, enErrorCode.ProcessTrn_Hold, Provider);//Rita 4-3-19 remove task.run for speed execution status not update as settlement reverse

//                //=====================For Routing Enable-uncomment TradingConfiguration(), and comment below code block===================================================                              
//                if (Req.TrnType == enTrnType.Buy_Trade)
//                {
//                    TradeBuyerListObj.TrnNo = Req.TrnNo;
//                    TradeBuyerListObj = _TradeBuyerList.Add(TradeBuyerListObj);
//                }
//                else
//                {
//                    TradeSellerListObj.TrnNo = Req.TrnNo;
//                    TradeSellerListObj = _TradeSellerList.Add(TradeSellerListObj);
//                }


//                _Resp = await TradingDataInsertV2(_Resp, Provider, Req.Price);

//                if (Provider.LPType != (short)enAppType.COINTTRADINGLocal)
//                {//LP reponse in Notification
//                    try
//                    {
//                        if (_Resp.ReturnCode == enResponseCodeService.Fail)
//                        {
//                            ActivityNotificationMessage notification = new ActivityNotificationMessage();
//                            notification.MsgCode = Convert.ToInt32(_Resp.ErrorCode);
//                            notification.Param1 = Req.TrnNo.ToString();
//                            notification.Type = Convert.ToInt16(EnNotificationType.Info);
//                            await _ISignalRService.SendActivityNotificationV2Arbitrage(notification, Req.MemberID.ToString(), 2);
//                            await _ISignalRService.OnStatusCancelArbitrage(NewTradetransaction.Status, Newtransaction, NewTradetransaction, "", NewTradetransaction.ordertype);
//                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("TradingDataInsertV2 Done Notification Send " + notification.MsgCode, ControllerName, "##TrnNo:" + Newtransaction.Id, Helpers.UTC_To_IST()));
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("TradingDataInsertV2 Done Notification Error", ControllerName, ex.Message + "##TrnNo:" + NewTradetransaction.TrnNo, Helpers.UTC_To_IST()));
//                    }
//                }

//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync Settlement END Now wait for 20 sec", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
//                Task.Delay(10000).Wait();//rita 3-1-19 wait for all operations done //Uday 15-01-2019 change from 10000 to 20000 //rita 27-2-19 change from 20000 to 10000

//                return _Resp;
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("CombineAllInitTransactionAsync Internal Error:##TrnNo " + Req.TrnNo, ControllerName, ex);
//                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionProcessInternalError });
//            }
//        }

//        #region RegionInitTransaction    
//        public async Task<BizResponse> CreateTransaction()
//        {
//            try
//            {
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Transaction Process For" + Req.TrnType + "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));
//                _TradePairObj = _trnMasterConfiguration.GetTradePairMasterArbitrage().Where(item => item.Id == Req.PairID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
//                if (_TradePairObj == null)
//                {
//                    Req.StatusMsg = EnResponseMessage.CreateTrnNoPairSelectedMsg;
//                    return MarkSystemFailTransaction(enErrorCode.CreateTrn_NoPairSelected);
//                }
//                var pairStasticsResult = _tradePairStastics.GetSingleAsync(pair => pair.PairId == Req.PairID);
//                _TradeTransactionObj.PairName = _TradePairObj.PairName;
//                var LoadDataResult = LoadAllMasterDataParaller();

//                var GetWalletIDResult1 = _WalletService.GetWalletID(Req.DebitAccountID);
//                if (Req.TrnType == enTrnType.Buy_Trade)
//                {
//                    _TradeTransactionObj.TrnTypeName = "BUY";
//                }
//                else
//                {
//                    _TradeTransactionObj.TrnTypeName = "SELL";
//                }
//                if (Convert.ToInt16(Req.ordertype) < 1 || Convert.ToInt16(Req.ordertype) > 5)
//                {
//                    Req.StatusMsg = EnResponseMessage.InValidOrderTypeMsg;
//                    return MarkSystemFailTransaction(enErrorCode.InValidOrderType);
//                }
//                var GetWalletIDResult2 = _WalletService.GetWalletID(Req.CreditAccountID);

//                Req.DebitWalletID = await GetWalletIDResult1;
//                if (Req.DebitWalletID == 0)
//                {
//                    Req.StatusMsg = EnResponseMessage.InValidDebitAccountIDMsg;
//                    return MarkSystemFailTransaction(enErrorCode.InValidDebitAccountID);
//                }
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Debit WalletID" + Req.DebitWalletID + "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));
//                var pairStastics = await pairStasticsResult;
//                //IF @PairID <> 0 ntrivedi 18-04-2018  check inside @TrnType (4,5) @TradeWalletMasterID will be 0 or null
//                if (Req.ordertype == enTransactionMarketType.MARKET)
//                {
//                    // var pairStastics =await _tradePairStastics.GetSingleAsync(pair => pair.PairId == Req.PairID);                   
//                    Req.Price = pairStastics.LTP;
//                }
//                _TradeTransactionObj.OrderWalletID = Req.DebitWalletID;

//                Req.CreditWalletID = await GetWalletIDResult2;
//                if (Req.CreditWalletID == 0)
//                {
//                    Req.StatusMsg = EnResponseMessage.InValidCreditAccountIDMsg;
//                    return MarkSystemFailTransaction(enErrorCode.InValidCreditAccountID);
//                }
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Credit WalletID" + Req.CreditWalletID + "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));

//                _TradeTransactionObj.DeliveryWalletID = Req.CreditWalletID;
//                if (Req.Qty <= 0 || (Req.Price <= 0 && Req.ordertype != enTransactionMarketType.MARKET) || (Req.StopPrice == 0 && Req.ordertype == enTransactionMarketType.STOP_Limit))
//                {
//                    Req.StatusMsg = EnResponseMessage.CreateTrnInvalidQtyPriceMsg;
//                    return MarkSystemFailTransaction(enErrorCode.CreateTrnInvalidQtyPrice);
//                }
//                await LoadDataResult;

//                if (_TradePairDetailObj == null)
//                {
//                    Req.StatusMsg = EnResponseMessage.CreateTrnNoPairSelectedMsg;
//                    return MarkSystemFailTransaction(enErrorCode.CreateTrn_NoPairSelected);
//                }
//                if (Req.TrnType == enTrnType.Buy_Trade)
//                {
//                    _TradeTransactionObj.BuyQty = Req.Qty;
//                    _TradeTransactionObj.BidPrice = Req.ordertype == enTransactionMarketType.MARKET ? 0 : Req.Price;
//                    var AssRes = AssignDataBuy();
//                    if (_TradeTransactionObj.BuyQty < _TradePairDetailObj.BuyMinQty || _TradeTransactionObj.BuyQty > _TradePairDetailObj.BuyMaxQty)
//                    {
//                        Req.StatusMsg = EnResponseMessage.ProcessTrn_AmountBetweenMinMaxMsg.Replace("@MIN", _TradePairDetailObj.BuyMinQty.ToString()).Replace("@MAX", _TradePairDetailObj.BuyMaxQty.ToString());
//                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_AmountBetweenMinMax, _TradePairDetailObj.BuyMinQty.ToString(), _TradePairDetailObj.BuyMaxQty.ToString());
//                    }
//                    if ((_TradeTransactionObj.BidPrice < _TradePairDetailObj.BuyMinPrice || _TradeTransactionObj.BidPrice > _TradePairDetailObj.BuyMaxPrice) && Req.ordertype != enTransactionMarketType.MARKET)
//                    {
//                        Req.StatusMsg = EnResponseMessage.ProcessTrn_PriceBetweenMinMaxMsg.Replace("@MIN", _TradePairDetailObj.BuyMinPrice.ToString()).Replace("@MAX", _TradePairDetailObj.BuyMaxPrice.ToString());
//                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_PriceBetweenMinMax, _TradePairDetailObj.BuyMinPrice.ToString(), _TradePairDetailObj.BuyMaxPrice.ToString());
//                    }
//                    await AssRes;
//                    InsertBuyerList();
//                }
//                else if (Req.TrnType == enTrnType.Sell_Trade)
//                {
//                    _TradeTransactionObj.SellQty = Req.Qty;
//                    _TradeTransactionObj.AskPrice = Req.ordertype == enTransactionMarketType.MARKET ? 0 : Req.Price;
//                    var AssRes = AssignDataSell();
//                    if (_TradeTransactionObj.SellQty < _TradePairDetailObj.SellMinQty || _TradeTransactionObj.SellQty > _TradePairDetailObj.SellMaxQty)
//                    {
//                        Req.StatusMsg = EnResponseMessage.ProcessTrn_AmountBetweenMinMaxMsg.Replace("@MIN", _TradePairDetailObj.SellMinQty.ToString()).Replace("@MAX", _TradePairDetailObj.SellMaxQty.ToString());
//                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_AmountBetweenMinMax, _TradePairDetailObj.SellMinQty.ToString(), _TradePairDetailObj.SellMaxQty.ToString());
//                    }
//                    if ((_TradeTransactionObj.AskPrice < _TradePairDetailObj.SellMinPrice || _TradeTransactionObj.AskPrice > _TradePairDetailObj.SellMaxPrice) && Req.ordertype != enTransactionMarketType.MARKET)
//                    {
//                        Req.StatusMsg = EnResponseMessage.ProcessTrn_PriceBetweenMinMaxMsg.Replace("@MIN", _TradePairDetailObj.SellMinPrice.ToString()).Replace("@MAX", _TradePairDetailObj.SellMaxPrice.ToString());
//                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_PriceBetweenMinMax, _TradePairDetailObj.SellMinPrice.ToString(), _TradePairDetailObj.SellMaxPrice.ToString());
//                    }
//                    await AssRes;
//                    InsertSellerList();
//                }

//                //int PriceLength = Req.Price.ToString().Substring(Req.Price.ToString().IndexOf(".")).Length;
//                //int QtyLength = Req.Qty.ToString().Substring(Req.Qty.ToString().IndexOf(".")).Length;


//                decimal RoundedPrice = Math.Round(Req.Price, _TradePairDetailObj.PriceLength);
//                decimal RoundedQty = Math.Round(Req.Qty, _TradePairDetailObj.QtyLength);
//                decimal RoundedAmt = Math.Round(Req.Qty * Req.Price, _TradePairDetailObj.AmtLength);

//                if (RoundedQty != Req.Qty && _TradePairDetailObj.QtyLength!=0)//rita 29-7-19 if 0 then do not check
//                {
//                    Req.StatusMsg = EnResponseMessage.ProcessTrn_QtyBadPrecision;
//                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_QtyBadPrecision);
//                }
//                else if(RoundedPrice != Req.Price && _TradePairDetailObj.PriceLength != 0)//rita 29-7-19 if 0 then do not check
//                {
//                    Req.StatusMsg = EnResponseMessage.ProcessTrn_PriceBadPrecision;
//                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_PriceBadPrecision);
//                }
//                else if (RoundedAmt != Req.Qty * Req.Price && _TradePairDetailObj.AmtLength != 0)//rita 29-7-19 if 0 then do not check
//                {
//                    Req.StatusMsg = EnResponseMessage.ProcessTrn_AmtBadPrecision;
//                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_NotionalBadPrecision);
//                }

//                var GetProListResult = _WebApiRepository.GetProviderDataListArbitrageAsync(new TransactionApiConfigurationRequest
//                {
//                    amount = Req.Qty,//Req.Amount Rita 4-4-19 pass Qty , as base on Qty routing place
//                    SMSCode = Req.SMSCode,
//                    APIType = enWebAPIRouteType.TransactionAPI,
//                    trnType = Req.TrnType == enTrnType.Sell_Trade ? Convert.ToInt32(enTrnType.Sell_Trade) : Convert.ToInt32(enTrnType.Buy_Trade),
//                    OrderType = Convert.ToInt16(Req.ordertype),
//                    PairID = Req.PairID,
//                    LPType = Req.LPType //khushali 11-06-19  For Specific LP Route ID 
//                });

//                //var walletLimit = _commonWalletFunction.CheckWalletLimitAsync(enWalletLimitType.TradingLimit, Req.DebitWalletID, Req.Amount);
//                var walletLimit = _commonWalletFunction.CheckWalletLimitAsyncV1(enWalletLimitType.TradingLimit, Req.DebitWalletID, Req.Amount);

//                if (_TradeTransactionObj.OrderTotalQty < (decimal)(0.000000000000000001) || _TradeTransactionObj.DeliveryTotalQty < (decimal)(0.000000000000000001))
//                {
//                    Req.StatusMsg = EnResponseMessage.CreateTrnInvalidQtyNAmountMsg;
//                    return MarkSystemFailTransaction(enErrorCode.CreateTrnInvalidQtyNAmount);
//                }
//                if (Req.Amount <= 0) // ntrivedi 02-11-2018 if amount =0 then also invalid
//                {
//                    Req.StatusMsg = EnResponseMessage.CreateTrnInvalidAmountMsg;
//                    return MarkSystemFailTransaction(enErrorCode.CreateTrnInvalidAmount);
//                }
//                WalletTrnLimitResponse WalletLimitRes = await walletLimit;//komal 25-01-2019 check transaction limit
//                if (WalletLimitRes.ErrorCode != enErrorCode.Success)
//                {
//                    //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Debit WalletID" + Req.DebitWalletID + "##TrnNo:" + Req.GUID+ "check transaction limit : " + WalletLimitRes.ToString(), Helpers.UTC_To_IST()));
//                    //Req.StatusMsg = WalletLimitRes.ReturnMsg.ToString();
//                    //return MarkSystemFailTransaction(WalletLimitRes.ErrorCode);//2019-3-7 add new param for notification msg
//                }
//                //khushali 11-06-19 Check Arbitrage Trading configuration bit
//                TradingConfigurationList = _tradingConfiguration.FindBy(e => e.Status == 1).ToList();
//                foreach (var TradeConfig in TradingConfigurationList)
//                {
//                    if (TradeConfig.Name == enTradingType.ArbitrageTrading.ToString())
//                    {
//                        IsOnlyArbitrageTradeOn = 1;
//                    }
//                }
//                if (IsOnlyArbitrageTradeOn == 0)//Rita 25-5-19 if bit not set then make Txn fail
//                {
//                    Req.StatusMsg = "Arbitrage Transaction is currenctly stopped";
//                    return MarkSystemFailTransaction(enErrorCode.CreateTxnTradingIsStopped);
//                }
//                //Rita 17-7-19 added for based on User wise BuyFirst,Sell First or none value
//                ArbitrageTradingAllowToUser ArbitrageAllowRecord = _ArbitrageTradingAllowToUser.GetSingle(e => e.UserId == Req.MemberID);
//                if (ArbitrageAllowRecord != null && Req.IsSmartArbitrage == 1)//Set Default None value
//                {
//                    SmaartTradePriority = ArbitrageAllowRecord.SmaartTradePriority;
//                    if ((SmaartTradePriority == 1 || SmaartTradePriority == 2)) 
//                    {
//                        CheckArbitrageTransactionStatus StatusObj = new CheckArbitrageTransactionStatus();
//                        if ((SmaartTradePriority == 2 && Req.TrnType == enTrnType.Buy_Trade) || (SmaartTradePriority == 1 && Req.TrnType == enTrnType.Sell_Trade))
//                        {
//                            DateTime tTime = Helpers.UTC_To_IST().Add(new TimeSpan(0, 0, 0, 10, 0));//loop for 10 second only
//                            while (Helpers.UTC_To_IST() < tTime)
//                            {
//                                await Task.Delay(300);
//                                StatusObj = _frontTrnRepository.CheckArbitrageTransactionStatus(Req.GUID.ToString(), SmaartTradePriority);
//                                if (StatusObj != null)
//                                {
//                                    if (StatusObj.status == 4 || StatusObj.status == 1)
//                                        break;
//                                    else if (StatusObj.status == 2 || StatusObj.status == 3)
//                                    {
//                                        Req.StatusMsg = EnResponseMessage.ProcessTrn_ArbitrageFirstTransactionFail;
//                                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_ArbitrageFirstTransactionFail);
//                                    }
//                                }
//                            }
//                            if (StatusObj == null)
//                            {
//                                Req.StatusMsg = EnResponseMessage.ProcessTrn_ArbitrageFirstTransactionFail;
//                                return MarkSystemFailTransaction(enErrorCode.ProcessTrn_ArbitrageFirstTransactionFail);
//                            }
//                        }
//                    }
//                }
//                TxnProviderList = await GetProListResult;
                
//                if (TxnProviderList.Count == 0)
//                {
//                    Req.StatusMsg = EnResponseMessage.ProcessTrn_ServiceProductNotAvailableMsg;
//                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_ServiceProductNotAvailable);
//                }
//                Provider = TxnProviderList.FirstOrDefault();
//                if ((string.IsNullOrEmpty(Provider.APIKey) || string.IsNullOrEmpty(Provider.SecretKey)) && Provider.LPType != (short)enAppType.COINTTRADINGLocal)
//                {
//                    Req.StatusMsg = EnResponseMessage.ProcessTrn_ServiceProductNotAvailableMsg;
//                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_ServiceProductNotAvailable);
//                }

//                if (Provider.MinPrice > Req.Price || Provider.MaxPrice < Req.Price)
//                {
//                    Req.StatusMsg = EnResponseMessage.ProcessTrn_PriceBetweenMinMaxMsg.Replace("@MIN", Provider.MinPrice.ToString()).Replace("@MAX", Provider.MaxPrice.ToString());
//                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_PriceBetweenMinMax, Provider.MinPrice.ToString(), Provider.MaxPrice.ToString());
//                }
//                else if (Provider.MinQty > Req.Qty || Provider.MaxQty < Req.Qty)
//                {
//                    Req.StatusMsg = EnResponseMessage.ProcessTrn_AmountBetweenMinMaxMsg.Replace("@MIN", Provider.MinQty.ToString()).Replace("@MAX", Provider.MaxQty.ToString());
//                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_AmountBetweenMinMax, Provider.MinQty.ToString(), Provider.MaxQty.ToString());
//                }
//                else if (Provider.MinNotional > Req.Price * Req.Qty || Provider.MaxNotional < Req.Price * Req.Qty)
//                {
//                    Req.StatusMsg = EnResponseMessage.ProcessTrn_TotalFailure.Replace("@MIN", Provider.MinNotional.ToString()).Replace("@MAX", Provider.MaxNotional.ToString());
//                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_TotalFailure, Provider.MinNotional.ToString(), Provider.MaxNotional.ToString());
//                }

//                // Khushali 27-07-2019 for allow  order type validation
//                List<AllowesOrderTypeArbitrage> allowesOrderTypes = _allowesOrderTypeArbitrageRepository.FindBy(e => e.SerProDetailID == Provider.SerProDetailID && e.OrderType == (long)Req.ordertype && e.Status==1).ToList();
//                if (allowesOrderTypes.Count == 0)
//                {
//                    Req.StatusMsg = EnResponseMessage.InValidOrderTypeMsg;
//                    return MarkSystemFailTransaction(enErrorCode.InValidOrderType);
//                }

//                //Khushali 26-07-2019 Stop Price Logic, For Buy order make stop price 3% Up and for Sale order make stop price 3% Down
//                //#region Stop Price Logic
//                //if (Provider.IsStoplossOrder == "1")
//                //{

//                //    decimal CalculatedPrice = 0;
//                //    if (Req.ordertype == enTransactionMarketType.LIMIT)
//                //    {
//                //        CalculatedPrice = Convert.ToDecimal(Req.Price * 3 / 100);
//                //        if (Req.TrnType == enTrnType.Buy_Trade)
//                //        {
//                //            Req.StopPrice = Req.Price + CalculatedPrice;
//                //        }
//                //        else if (Req.TrnType == enTrnType.Sell_Trade)
//                //        {
//                //            Req.StopPrice = Req.Price - CalculatedPrice;
//                //        }
//                //    }                    
//                //}
//                //#endregion


//                Req.Status = enTransactionStatus.Initialize;
//                Req.StatusCode = Convert.ToInt64(enErrorCode.TransactionInsertSuccess);
//                await InsertTransactionInQueue();
//                await InsertTradeTransactionInQueue();
//                await InsertTradeStopLoss(pairStastics.LTP);
//                return new BizResponse { ReturnMsg = "", ReturnCode = enResponseCodeService.Success };
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("CreateTransaction:##TrnNo " + Req.TrnNo + " GUID:" + Req.GUID, ControllerName, ex);
//                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionInsertInternalError });
//            }

//        }
//        public async Task AssignDataBuy()
//        {
//            try
//            {
//                _TradeTransactionObj.DeliveryTotalQty = Req.Qty;
//                _TradeTransactionObj.OrderTotalQty = Helpers.DoRoundForTrading(Req.Qty * Req.Price, 18);//235.415001286,8 =  235.41500129                         
//                _TradeTransactionObj.Order_Currency = _BaseCurrService.SMSCode;
//                _TradeTransactionObj.Delivery_Currency = _SecondCurrService.SMSCode;
//                Req.SMSCode = _TradeTransactionObj.Order_Currency;
//                Req.Amount = _TradeTransactionObj.OrderTotalQty;
//            }
//            catch (Exception ex)
//            {
//                Task.Run(() => HelperForLog.WriteErrorLog("AssignDataBuy:##TrnNo " + Req.TrnNo, ControllerName, ex));
//                throw ex;
//            }
//        }
//        public async Task AssignDataSell()
//        {
//            try
//            {
//                _TradeTransactionObj.OrderTotalQty = Req.Qty;
//                _TradeTransactionObj.DeliveryTotalQty = Helpers.DoRoundForTrading(Req.Qty * Req.Price, 18);//235.415001286,8 =  235.41500129                        
//                _TradeTransactionObj.Order_Currency = _SecondCurrService.SMSCode;
//                _TradeTransactionObj.Delivery_Currency = _BaseCurrService.SMSCode;
//                Req.SMSCode = _TradeTransactionObj.Order_Currency;
//                Req.Amount = _TradeTransactionObj.OrderTotalQty;
//            }
//            catch (Exception ex)
//            {
//                Task.Run(() => HelperForLog.WriteErrorLog("AssignDataBuy:##TrnNo " + Req.TrnNo, ControllerName, ex));
//                throw ex;
//            }
//        }
//        public async Task LoadAllMasterDataParaller()
//        {
//            try
//            {
//                _BaseCurrService = _trnMasterConfiguration.GetServicesArbitrage().Where(item => item.Id == _TradePairObj.BaseCurrencyId).FirstOrDefault();
//                _SecondCurrService = _trnMasterConfiguration.GetServicesArbitrage().Where(item => item.Id == _TradePairObj.SecondaryCurrencyId).FirstOrDefault();
//                _TradePairDetailObj = _trnMasterConfiguration.GetTradePairDetailArbitrage().Where(item => item.PairId == Req.PairID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("LoadAllMasterDataParaller:##TrnNo " + Req.TrnNo, ControllerName, ex);
//                throw ex;
//            }
//        }
//        public BizResponse MarkSystemFailTransaction(enErrorCode ErrorCode, string Param2 = "", string Param3 = "")
//        {
//            try
//            {
//                Req.Status = enTransactionStatus.SystemFail;
//                Req.StatusCode = Convert.ToInt64(ErrorCode);
//                InsertTransactionInQueue();
//                InsertTradeStopLoss(0);
//                try//as some para null in starting so error occured here ,only in case of system fail
//                {
//                    InsertTradeTransactionInQueue();
//                }
//                catch (Exception ex)
//                {
//                    Task.Run(() => HelperForLog.WriteErrorLog("MarkSystemFailTransaction Trade TQ Error:##TrnNo " + Req.TrnNo, ControllerName, ex));
//                }
//                //DI of SMS here
//                //Uday 06-12-2018  Send SMS When Transaction is Failed
//                SMSSendTransactionHoldOrFailed(Newtransaction.Id, Newtransaction.MemberMobile, Req.Price, Req.Qty, 2);

//                //Uday 06-12-2018  Send Email When Transaction is Failed
//                EmailSendTransactionHoldOrFailed(Newtransaction.Id, Newtransaction.MemberID + "", Req.PairID, Req.Qty, Newtransaction.TrnDate + "", Req.Price, 0, 2, Convert.ToInt16(Req.ordertype), Convert.ToInt16(Req.TrnType));

//                try
//                {
//                    //Rita 26-11-2018 add Activity Notifiation v2
//                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
//                    //notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);
//                    notification.MsgCode = Convert.ToInt32(ErrorCode); //komal 05-02-2019 set validation error code
//                    notification.Param1 = Req.TrnNo.ToString();
//                    notification.Param2 = Param2;
//                    notification.Param3 = Param3;
//                    notification.Param4 = "USD";
//                    notification.Type = Convert.ToInt16(EnNotificationType.Fail);
//                    _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2);//Req.accessToken
//                    //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("MarkSystemFailTransaction Notification Send " + notification.MsgCode, ControllerName, "##TrnNo:" + Newtransaction.Id, Helpers.UTC_To_IST()));
//                }
//                catch (Exception ex)
//                {
//                    Task.Run(() => HelperForLog.WriteErrorLog("ISignalRService Notification Error-MarkSystemFailTransaction ##TrnNo:" + Newtransaction.Id, ControllerName, ex));
//                }
//                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.Fail, ErrorCode = ErrorCode });
//            }
//            catch (Exception ex)
//            {
//                Task.Run(() => HelperForLog.WriteErrorLog("MarkSystemFailTransaction:##TrnNo " + Req.TrnNo, ControllerName, ex));
//                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionInsertInternalError });
//            }
//        }
//        public async Task InsertTransactionInQueue()//ref long TrnNo
//        {
//            try
//            {
//                Newtransaction = new TransactionQueueArbitrage()
//                {
//                    TrnDate = Helpers.UTC_To_IST(),
//                    //GUID = Guid.NewGuid(),
//                    GUID = Req.GUID,
//                    TrnMode = Req.TrnMode,
//                    TrnType = Convert.ToInt16(Req.TrnType),
//                    MemberID = Req.MemberID,
//                    MemberMobile = Req.MemberMobile,
//                    TransactionAccount = Req.TransactionAccount,
//                    SMSCode = Req.SMSCode,
//                    Amount = Req.Amount,
//                    Status = Convert.ToInt16(Req.Status),
//                    StatusCode = Req.StatusCode,
//                    StatusMsg = Req.StatusMsg,
//                    TrnRefNo = Req.TrnRefNo,
//                    AdditionalInfo = Req.AdditionalInfo,
//                    DebitAccountID = Req.DebitAccountID,//rita 03-12-18 added as required in withdraw process
//                    LPType = Req.LPType,//rita 13-6-19 assign initially
//                    IsSmartArbitrage = Req.IsSmartArbitrage,
//                    CallStatus = 1
//                };
//                Newtransaction = _TransactionRepository.Add(Newtransaction);
//                Req.TrnNo = Newtransaction.Id;
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("InsertTransactionInQueue:##TrnNo " + Req.TrnNo, ControllerName, ex);
//                throw ex;
//            }

//        }
//        public async Task InsertTradeTransactionInQueue()
//        {
//            try
//            {
//                NewTradetransaction = new TradeTransactionQueueArbitrage()
//                {
//                    TrnDate = Helpers.UTC_To_IST(),
//                    TrnType = Convert.ToInt16(Req.TrnType),
//                    TrnTypeName = _TradeTransactionObj.TrnTypeName,
//                    MemberID = Req.MemberID,
//                    PairID = Req.PairID,
//                    PairName = _TradeTransactionObj.PairName,
//                    OrderWalletID = _TradeTransactionObj.OrderWalletID,
//                    DeliveryWalletID = _TradeTransactionObj.DeliveryWalletID,
//                    OrderAccountID = Req.DebitAccountID,
//                    DeliveryAccountID = Req.CreditAccountID,
//                    BuyQty = _TradeTransactionObj.BuyQty,
//                    BidPrice = _TradeTransactionObj.BidPrice,
//                    SellQty = _TradeTransactionObj.SellQty,
//                    AskPrice = _TradeTransactionObj.AskPrice,
//                    Order_Currency = _TradeTransactionObj.Order_Currency,
//                    OrderTotalQty = _TradeTransactionObj.OrderTotalQty,
//                    Delivery_Currency = _TradeTransactionObj.Delivery_Currency,
//                    DeliveryTotalQty = _TradeTransactionObj.DeliveryTotalQty,
//                    SettledBuyQty = _TradeTransactionObj.SettledBuyQty,
//                    SettledSellQty = _TradeTransactionObj.SettledSellQty,
//                    Status = Convert.ToInt16(Req.Status),
//                    StatusCode = Req.StatusCode,
//                    StatusMsg = Req.StatusMsg,
//                    ordertype = Convert.ToInt16(Req.ordertype),
//                    TrnNo = Req.TrnNo,//NewTradetransactionReq.TrnNo,
//                    IsAPITrade = Convert.ToInt16(Req.LPType == (short)enAppType.COINTTRADINGLocal ? 0 : 1),
//                    IsExpired = 0,//Rita 30-1-19 for API level changes
//                    APIStatus = "",
//                    IsAPICancelled = 0
//                };
//                NewTradetransaction = _TradeTransactionRepository.Add(NewTradetransaction);
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("InsertTradeTransactionInQueue:##TrnNo " + Req.TrnNo, ControllerName, ex);
//                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
//                throw ex;
//            }

//        }
//        public async Task InsertTradeStopLoss(decimal LTP)
//        {
//            try
//            {
//                TradeStopLossObj = new TradeStopLossArbitrage()
//                {
//                    ordertype = Convert.ToInt16(Req.ordertype),
//                    StopPrice = Req.StopPrice,
//                    Status = Convert.ToInt16(enTransactionStatus.Success),
//                    LTP = LTP,
//                    PairID = Req.PairID,
//                    TrnNo = Req.TrnNo,
//                    ISFollowersReq = Req.ISFollowersReq,//Rita 12-1-19 main req always 0
//                    FollowingTo = Req.FollowingTo,//Rita 12-1-19 main req always 0
//                    LeaderTrnNo = Req.LeaderTrnNo,//Rita 21-1-19 main req always 0
//                    FollowTradeType = Req.FollowTradeType//Rita 22-1-19 main req always blank
//                };
//                if (Req.ordertype == enTransactionMarketType.STOP_Limit)
//                {
//                    if (Req.StopPrice <= LTP)//250 - 300 Low
//                    {
//                        if (Req.StopPrice == LTP)
//                            STOPLimitWithSameLTP = 1;

//                        TradeStopLossObj.RangeMin = Req.StopPrice;
//                        TradeStopLossObj.RangeMax = LTP;
//                        TradeStopLossObj.MarketIndicator = 0;
//                    }
//                    else if (Req.StopPrice > LTP)//300 - 350 High
//                    {
//                        TradeStopLossObj.RangeMin = LTP;
//                        TradeStopLossObj.RangeMax = Req.StopPrice;
//                        TradeStopLossObj.MarketIndicator = 1;
//                    }
//                }
//                TradeStopLossObj = _TradeStopLoss.Add(TradeStopLossObj);
//                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommSuccessMsgInternal, ReturnCode = enResponseCodeService.Success });
//            }
//            catch (Exception ex)
//            {
//                Task.Run(() => HelperForLog.WriteErrorLog("InsertTradeStopLossArbitrage:##TrnNo " + Req.TrnNo, ControllerName, ex));
//                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
//                throw ex;
//            }

//        }
//        #endregion        

//        #region RegionProcessTransaction        
//        public async void MarkTransactionSystemFail(string StatusMsg, enErrorCode ErrorCode)
//        {
//            try
//            {
//                //var Txn = _TransactionRepository.GetById(Req.TrnNo);
//                Newtransaction.MakeTransactionSystemFail();
//                Newtransaction.SetTransactionStatusMsg(StatusMsg);
//                Newtransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
//                _TransactionRepository.UpdateAsync(Newtransaction);

//                //var TradeTxn = _TradeTransactionRepository.GetById(Req.TrnNo);
//                NewTradetransaction.MakeTransactionSystemFail();
//                NewTradetransaction.SetTransactionStatusMsg(StatusMsg);
//                NewTradetransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
//                _TradeTransactionRepository.Update(NewTradetransaction);
//                try
//                {
//                    //Rita 26-11-2018 add Activity Notifiation v2
//                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
//                    notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);

//                    if (ErrorCode == enErrorCode.sp_InsufficientBalanceForCharge)//Rita 13-03-19 In this case only send diff ErrorCode as per front and wallet
//                        notification.MsgCode = Convert.ToInt32(ErrorCode);

//                    //notification.MsgCode = Convert.ToInt32(ErrorCode);
//                    notification.Param1 = Req.TrnNo.ToString();
//                    notification.Type = Convert.ToInt16(EnNotificationType.Fail);
//                    _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2);//Req.accessToken
//                                                                                                          //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);

//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("MarkTransactionSystemFail Notification Send " + notification.MsgCode, ControllerName, "##TrnNo:" + Newtransaction.Id, Helpers.UTC_To_IST()));
//                }
//                catch (Exception ex)
//                {
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ISignalRService Notification Error-MarkTransactionSystemFail", ControllerName, ex.Message + "##TrnNo:" + NewTradetransaction.TrnNo, Helpers.UTC_To_IST()));
//                }
//            }
//            catch (Exception ex)
//            {
//                Task.Run(() => HelperForLog.WriteErrorLog("MarkTransactionSystemFail:##TrnNo " + Req.TrnNo, ControllerName, ex));
//                // throw ex;
//            }
//        }
//        public async Task MarkTransactionHold(string StatusMsg, enErrorCode ErrorCode, TransactionProviderArbitrageResponse Provider)
//        {
//            try
//            {
//                if (Newtransaction.Status != 0)//Rita 22-2-19 this update after settlement so overrights status, error solved
//                    return;

//                Newtransaction.MakeTransactionHold();
//                Newtransaction.SetTransactionStatusMsg(StatusMsg);
//                Newtransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
//                Newtransaction.SetServiceProviderData(0, Provider.ProviderID, 0, Provider.RouteID, Provider.SerProDetailID, Convert.ToInt16(Provider.LPType));
//                _TransactionRepository.UpdateAsync(Newtransaction);

//                //var Txn = _TransactionRepository.GetById(Req.TrnNo);
//                //rita 28-12-18 remove active inactive as txn considers in settlement time
//                //if (Req.ordertype == enTransactionMarketType.STOP_Limit && STOPLimitWithSameLTP == 0)//Rita 26-12-18 for STOP & limit Order
//                //{
//                //    NewTradetransaction.MakeTransactionInActive();
//                //}
//                //else
//                //{
//                //    NewTradetransaction.MakeTransactionHold();
//                //}
//                if (NewTradetransaction.Status != 0)
//                    return;

//                NewTradetransaction.MakeTransactionHold();
//                NewTradetransaction.SetTransactionStatusMsg(StatusMsg);
//                NewTradetransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
//                if (NewTradetransaction.Status == 1)
//                    return;
//                _TradeTransactionRepository.Update(NewTradetransaction);

//                //if (Req.ordertype == enTransactionMarketType.STOP_Limit && STOPLimitWithSameLTP == 0)//Rita 26-12-18 for STOP & limit Order
//                //{
//                //    ActivityNotificationMessage notification = new ActivityNotificationMessage();
//                //    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRTrnSuccessfullyCreated);
//                //    notification.Param1 = Req.Price.ToString();
//                //    notification.Param2 = Req.Qty.ToString();
//                //    notification.Type = Convert.ToInt16(EnNotificationType.Success);
//                //    _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2);//Req.accessToken    
//                //    return;//for Inactive Order no need to send Book,history etc
//                //}

//                try
//                {
//                    var CopyNewtransaction = new TransactionQueueArbitrage();
//                    CopyNewtransaction = (TransactionQueueArbitrage)Newtransaction.Clone();
//                    //CopyNewtransaction.MakeTransactionHold();

//                    var CopyNewTradetransaction = new TradeTransactionQueueArbitrage();
//                    CopyNewTradetransaction = (TradeTransactionQueueArbitrage)NewTradetransaction.Clone();
//                    //CopyNewTradetransaction.MakeTransactionHold();
//                    //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ISignalRService", ControllerName, "parallel execution pre ##TrnNo:",Helpers.UTC_To_IST()));
//                    Parallel.Invoke(() => _ISignalRService.OnStatusHoldArbitrage(Convert.ToInt16(enTransactionStatus.Success), CopyNewtransaction, CopyNewTradetransaction, "", TradeStopLossObj.ordertype));
//                    //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ISignalRService", ControllerName, "parallel execution complete ##TrnNo:",Helpers.UTC_To_IST()));

//                    //Uday 06-12-2018  Send SMS When Transaction is Hold
//                    SMSSendTransactionHoldOrFailed(Newtransaction.Id, Newtransaction.MemberMobile, Req.Price, Req.Qty, 1);
//                }
//                catch (Exception ex)
//                {
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ISignalRService", ControllerName, "Trading Hold Error " + ex.Message + "##TrnNo:" + NewTradetransaction.TrnNo, Helpers.UTC_To_IST()));
//                }
//            }
//            catch (Exception ex)
//            {
//                Task.Run(() => HelperForLog.WriteErrorLog("MarkTransactionHold:##TrnNo " + Req.TrnNo, ControllerName, ex));
//                throw ex;
//            }
//        }
//        #endregion

//        #region Settlement Insert Data        
//        public async Task InsertSellerList()
//        {
//            try
//            {
//                TradeSellerListObj = new TradeSellerListArbitrageV1()
//                {
//                    CreatedBy = Req.MemberID,
//                    TrnNo = Req.TrnNo,
//                    PairID = Req.PairID,
//                    PairName = _TradeTransactionObj.PairName,
//                    Price = Req.Price,
//                    Qty = Req.Qty,
//                    ReleasedQty = Req.Qty,
//                    SelledQty = 0,
//                    RemainQty = Req.Qty,
//                    IsProcessing = Convert.ToInt16(Req.LPType == (short)enAppType.COINTTRADINGLocal ? 1 : 0),//Rita 25-6-19 remove for LP
//                    OrderType = Convert.ToInt16(Req.ordertype),
//                    Status = Convert.ToInt16(enTransactionStatus.Initialize),//txn type status
//                    IsAPITrade = Convert.ToInt16(Req.LPType == (short)enAppType.COINTTRADINGLocal ? 0 : 1),//Rita 30-1-19 for API level changes in settlement , do not pick in local settlement
//                };
//                if (Req.ordertype == enTransactionMarketType.STOP_Limit && STOPLimitWithSameLTP == 0)
//                {
//                    //TradeSellerListObj.Status = Convert.ToInt16(enTransactionStatus.InActive);
//                    TradeSellerListObj.IsProcessing = 0;
//                }
//                //TradeSellerListObj =_TradeSellerList.Add(TradeSellerListObj);
//                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommSuccessMsgInternal, ReturnCode = enResponseCodeService.Success });
//            }
//            catch (Exception ex)
//            {
//                Task.Run(() => HelperForLog.WriteErrorLog("Tradepoolmaster:##TrnNo " + Req.TrnNo, ControllerName, ex));
//                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
//                throw ex;
//            }
//        }

//        public async Task InsertBuyerList()
//        {
//            try
//            {
//                TradeBuyerListObj = new TradeBuyerListArbitrageV1()
//                {
//                    CreatedBy = Req.MemberID,
//                    TrnNo = Req.TrnNo,
//                    PairID = Req.PairID,
//                    PairName = _TradeTransactionObj.PairName,
//                    Price = Req.Price,
//                    Qty = Req.Qty, //same as request as one entry per one request
//                    DeliveredQty = 0,
//                    RemainQty = Req.Qty,
//                    IsProcessing = Convert.ToInt16(Req.LPType == (short)enAppType.COINTTRADINGLocal ? 1 : 0),//Rita 25-6-19 remove for LP,
//                    OrderType = Convert.ToInt16(Req.ordertype),
//                    Status = Convert.ToInt16(enTransactionStatus.Initialize),//txn type status
//                    IsAPITrade = Convert.ToInt16(Req.LPType == (short)enAppType.COINTTRADINGLocal ? 0 : 1),//Rita 30-1-19 for API level changes in settlement , do not pick in local settlement
//                };
//                if (Req.ordertype == enTransactionMarketType.STOP_Limit && STOPLimitWithSameLTP == 0)
//                {
//                    //TradeBuyerListObj.Status = Convert.ToInt16(enTransactionStatus.InActive);
//                    TradeBuyerListObj.IsProcessing = 0;
//                }
//                //TradeBuyerListObj = _TradeBuyerList.Add(TradeBuyerListObj);
//                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommSuccessMsgInternal, ReturnCode = enResponseCodeService.Success });
//            }
//            catch (Exception ex)
//            {
//                Task.Run(() => HelperForLog.WriteErrorLog("InsertBuyerList:##TrnNo " + Req.TrnNo, ControllerName, ex));
//                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
//                throw ex;
//            }

//        }

//        #endregion

//        public async Task<BizResponse> TradingDataInsertV2(BizResponse _Resp, TransactionProviderArbitrageResponse Provider, decimal LTP)
//        {
//            ProcessTransactionCls _TransactionObj = new ProcessTransactionCls();
//            //WebAPIParseResponseCls WebAPIParseResponseClsObj = new WebAPIParseResponseCls();
//            RealTimeLtpChecker RealTimeLtpCheckerobj = new RealTimeLtpChecker();
//            LPProcessTransactionCls LPProcessTransactionClsObj = new LPProcessTransactionCls();
//            short IsTxnProceed = 0;
//            _Resp = new BizResponse();
//            try
//            {
//                // khushali 10-07-2019 Move to first  and check validation first
//                //var ServiceProConfiguration = _IGetWebRequest.GetServiceProviderConfigurationArbitrage(Provider.SerProDetailID);
//                //if ((ServiceProConfiguration == null || string.IsNullOrEmpty(ServiceProConfiguration.APIKey) || string.IsNullOrEmpty(ServiceProConfiguration.SecretKey)) && Provider.LPType != (short)enAppType.COINTTRADINGLocal)
//                //{
//                //    _Resp.ReturnMsg = EnResponseMessage.ProcessTrn_ThirdPartyDataNotFoundMsg;
//                //    _Resp.ReturnCode = enResponseCodeService.Fail;
//                //    _Resp.ErrorCode = enErrorCode.ProcessTrn_ThirdPartyDataNotFound;
//                //    IsTxnProceed = 0;
//                //    Task<WalletDrCrResponse> WalletResult = _WalletService.ArbitrageGetReleaseHoldNew(NewTradetransaction.Order_Currency, Helpers.GetTimeStamp(), Req.Amount,
//                //                                 NewTradetransaction.OrderAccountID, NewTradetransaction.TrnNo, enServiceType.Trading,
//                //                                 enWalletTrnType.ReleaseBlockAmount, (enTrnType)Newtransaction.TrnType, (EnAllowedChannels)Newtransaction.TrnMode,
//                //                                 Req.accessToken);
//                //    var WalletResp = await WalletResult;
//                //    HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessTrn_ThirdPartyDataNotFoundMsg Arbitrage", ControllerName, "##ErrorCode:" + WalletResp.ErrorCode + " ##ReturnCode:" + WalletResp.ReturnCode + " ##ReturnMsg:" + WalletResp.ReturnMsg + " ##LpType:" + Provider.LPType + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST());
//                //    return await Task.FromResult(_Resp);
//                //}
//                var ServiceProConfiguration = new ServiceProConfigurationArbitrage();
//                ServiceProConfiguration.APIKey = Provider.APIKey;
//                ServiceProConfiguration.SecretKey = Provider.SecretKey;

//                HelperForLog.WriteLogIntoFileAsyncDtTm("LiquidityConfiguration Arbitrage", ControllerName, "LiquidityConfiguration Call Web API ##LPType:" + Provider.LPType + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST());

//                //Insert API request Data
//                _TransactionObj.TransactionRequestID = InsertTransactionRequest(Provider, "Price : " + Req.Price + "API Price : " + LTP + "  Qty : " + Req.Qty + " OrderType :" + Req.ordertype);


//                //Rita 13-08-19 paper trading/demo trading , take user from config and skip live trading only make vertual transaction
//                string PaperDemoIDss = _configuration["PaperDemoIDs"];
//                if (PaperDemoIDss != null)
//                {
//                    string[] PaperDemoIDs = PaperDemoIDss.Split(',');
//                    foreach (string id in PaperDemoIDs)
//                    {
//                        if (Req.MemberID.ToString() == id)//make settle
//                        {
//                            WebAPIParseResponseClsObj.TrnRefNo = "99999";
//                            _TransactionObj.APIResponse = "Demo settled";
//                            WebAPIParseResponseClsObj.OperatorRefNo="99999";
//                            WebAPIParseResponseClsObj.StatusMsg = "Demo settled";
//                            _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                            IsTxnProceed = 1;
//                            goto TransactionComplete;
//                        }
//                    }
//                }
//                //=====================================================Transaction Process=========================================
//                if (Provider.ProTypeID == Convert.ToInt64(enWebAPIRouteType.CCXTLpProvider) && Provider.LPType != (short)enAppType.COINTTRADINGLocal)//Rita 22-7-19 Implemented CCXT API
//                {
//                    _Resp = await CCXTCreateTransactionAsync(Newtransaction, NewTradetransaction, Provider,_TransactionObj, LPProcessTransactionClsObj);
                    
//                    if (_Resp.ReturnCode != enResponseCodeService.Success)//Move on next provider
//                        IsTxnProceed = 0;
//                    else
//                        IsTxnProceed = 1;//does not fail this order

//                    HelperForLog.WriteLogIntoFile("CCXT Arbitrage Response", ControllerName, "ReturnCode:" + _Resp.ReturnCode + " ReturnMsg:" + _Resp.ReturnMsg + " ErrorCode:" + _Resp.ErrorCode + " ##IsTxnProceed:" + IsTxnProceed + "##TrnNo:" + Req.TrnNo);
//                }
//                else
//                {
//                    switch (Provider.LPType)
//                    {
//                        //khushali 11-06-19 Add Route  for loacl Trading
//                        case (short)enAppType.COINTTRADINGLocal:
//                            if (Req.TrnType == enTrnType.Buy_Trade)
//                            {
//                                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "END Buyer" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
//                                if (Req.ordertype != enTransactionMarketType.STOP_Limit || STOPLimitWithSameLTP == 1)
//                                {
//                                    _Resp = await _SettlementRepositoryV1.PROCESSSETLLEMENTBuy(_Resp, NewTradetransaction, Newtransaction, TradeStopLossObj, TradeBuyerListObj, Req.accessToken, 0);
//                                }
//                            }
//                            else
//                            {
//                                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "END Seller" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
//                                if (Req.ordertype != enTransactionMarketType.STOP_Limit || STOPLimitWithSameLTP == 1)
//                                {
//                                    _Resp = await _SettlementRepositoryV1.PROCESSSETLLEMENTSell(_Resp, NewTradetransaction, Newtransaction, TradeStopLossObj, TradeSellerListObj, Req.accessToken, 0);
//                                }
//                            }
//                            if (_Resp.ReturnCode != enResponseCodeService.Success)//Move on next provider
//                                IsTxnProceed = 0;
//                            else
//                                IsTxnProceed = 1;//does not fail this order
//                            break;
//                        case (short)enAppType.Binance:
//                            await ProcessTransactionOnBinance(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
//                            if (_Resp.ReturnCode != enResponseCodeService.Success)
//                                IsTxnProceed = 0;//check next loop provider
//                            else
//                                IsTxnProceed = 1;//does not fail this order  
//                            break;
//                        case (short)enAppType.Bittrex:
//                            await ProcessTransactionOnBittrex(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
//                            if (_Resp.ReturnCode != enResponseCodeService.Success)
//                                IsTxnProceed = 0;//check next loop provider
//                            else
//                                IsTxnProceed = 1;//does not fail this order 
//                            break;
//                        case (short)enAppType.TradeSatoshi:
//                            await ProcessTransactionOnTradeSatoshi(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
//                            if (_Resp.ReturnCode != enResponseCodeService.Success)
//                                IsTxnProceed = 0;//check next loop provider
//                            else
//                                IsTxnProceed = 1;//does not fail this order
//                            break;
//                        case (short)enAppType.Poloniex:
//                            await ProcessTransactionOnTradePoloniex(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
//                            if (_Resp.ReturnCode != enResponseCodeService.Success)
//                                IsTxnProceed = 0;//check next loop provider
//                            else
//                                IsTxnProceed = 1;//does not fail this order
//                            break;
//                        case (short)enAppType.Coinbase:
//                            await ProcessTransactionOnTradeCoinbase(_Resp, ServiceProConfiguration, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);
//                            if (_Resp.ReturnCode != enResponseCodeService.Success)
//                                IsTxnProceed = 0;//check next loop provider
//                            else
//                                IsTxnProceed = 1;//does not fail this order
//                            break;
//                        case (short)enAppType.Huobi:
//                            var huobiExchangeInfoResp = _huobiLPService.GetExchangeInfoAsync();
//                            await ProcessTransactionOnHuobi(_Resp, ServiceProConfiguration, huobiExchangeInfoResp.Result.Data, WebAPIParseResponseClsObj, Provider, Req.TrnNo, _TransactionObj, LTP, LPProcessTransactionClsObj);

//                            if (_Resp.ReturnCode != enResponseCodeService.Success)
//                                IsTxnProceed = 0;//check next loop provider
//                            else
//                                IsTxnProceed = 1;//does not fail this order
//                            break;

//                        default:
//                            HelperForLog.WriteLogIntoFile("LiquidityConfiguration Arbitrage", ControllerName, "--3--LiquidityConfiguration Call web API  not found proper liquidity provider---" + "##TrnNo:" + Req.TrnNo);
//                            break;
//                    }

//                    HelperForLog.WriteLogIntoFile("LiquidityConfiguration Arbitrage", ControllerName, "--2--LiquidityConfiguration Call web API--- ##IsTxnProceed:" + IsTxnProceed + "##TrnNo:" + Req.TrnNo);
//                }
//                //=====================================================Transaction END=========================================

//                TransactionComplete:
//                if (IsTxnProceed == 0)//Here add logic of SPOT order is pending then make release and remove code of release from settlementrepository
//                {
//                    if (Provider.LPType != (short)enAppType.COINTTRADINGLocal)
//                    {
//                        Newtransaction.Status = 2;
//                        Newtransaction.CallStatus = 0;
//                        NewTradetransaction.Status = 2;
//                        NewTradetransaction.IsAPICancelled = 1;
//                        NewTradetransaction.IsCancelled = 1;
//                        NewTradetransaction.SettledDate = Helpers.UTC_To_IST();

//                        Newtransaction.SetTransactionStatusMsg(_Resp.ReturnMsg);
//                        NewTradetransaction.IsAPITrade = 1;
//                        NewTradetransaction.APIPrice = Req.Price;
//                        NewTradetransaction.SetTransactionStatusMsg(_Resp.ReturnMsg);
//                        NewTradetransaction.SetTransactionCode(Convert.ToInt64(_Resp.ErrorCode));//rita 25-7-19 set for fail case
//                        Newtransaction.SetTransactionCode(Convert.ToInt64(_Resp.ErrorCode));//rita 25-7-19 set for fail case
//                        NewTradetransaction.APIStatus = "2";
//                        _TransactionRepository.Update(Newtransaction);
//                        _TradeTransactionRepository.Update(NewTradetransaction);

//                        NewtransactionRequest.Status = NewTradetransaction.Status;
//                        NewtransactionRequest.SetResponse(_TransactionObj.APIResponse);
//                        NewtransactionRequest.SetResponseTime(Helpers.UTC_To_IST());
//                        _TransactionRequest.Update(NewtransactionRequest);
//                    }

//                    if (Req.TrnType == enTrnType.Buy_Trade)
//                    {
//                        TradeBuyerListObj.Status = NewTradetransaction.Status;
//                        TradeBuyerListObj.IsAPITrade = NewTradetransaction.IsAPITrade;
//                        TradeBuyerListObj.IsProcessing = 0;
//                        _TradeBuyerList.Update(TradeBuyerListObj);
//                    }
//                    else if (Req.TrnType == enTrnType.Sell_Trade)
//                    {
//                        TradeSellerListObj.Status = NewTradetransaction.Status;
//                        TradeSellerListObj.IsAPITrade = NewTradetransaction.IsAPITrade;
//                        TradeSellerListObj.IsProcessing = 0;
//                        _TradeSellerList.Update(TradeSellerListObj);
//                    }

//                    //release wallet call
//                    Task<WalletDrCrResponse> WalletResult = _WalletService.ArbitrageGetReleaseHoldNew(NewTradetransaction.Order_Currency, Helpers.GetTimeStamp(), Req.Amount,
//                                                 NewTradetransaction.OrderAccountID, NewTradetransaction.TrnNo, enServiceType.Trading,
//                                                 enWalletTrnType.ReleaseBlockAmount, (enTrnType)Newtransaction.TrnType, (EnAllowedChannels)Newtransaction.TrnMode,
//                                                 Req.accessToken);
//                    var WalletResp = await WalletResult;
//                    HelperForLog.WriteLogIntoFileAsyncDtTm("Wallet Release-Operator Fail Arbitrage", ControllerName, "##ErrorCode:" + WalletResp.ErrorCode + " ##ReturnCode:" + WalletResp.ReturnCode + " ##ReturnMsg:" + WalletResp.ReturnMsg + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST());
//                    //if (WalletResp.ReturnCode != enResponseCode.Success)
//                    //{

//                    //}
//                    //else
//                    //{

//                    //}

//                }
//                else if (IsTxnProceed == 1 && Provider.LPType != (short)enAppType.COINTTRADINGLocal)
//                {
//                    HelperForLog.WriteLogIntoFileAsyncDtTm("Transaction Proceed in LP ", ControllerName, "##IsTxnProceed:" + IsTxnProceed + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST());

//                    //Make Provider balance hold as goes in LP
//                    //LPHoldDr WalletProviderHoldObj = new LPHoldDr();
//                    //WalletProviderHoldObj.SerProID = Provider.ProviderID;
//                    //WalletProviderHoldObj.CoinName = Req.SMSCode;
//                    //WalletProviderHoldObj.Timestamp = Helpers.GetTimeStamp();
//                    //WalletProviderHoldObj.Amount = Req.Amount;
//                    //WalletProviderHoldObj.TrnRefNo = Req.TrnNo;
//                    //WalletProviderHoldObj.trnType = Req.TrnType == enTrnType.Buy_Trade ? enWalletTrnType.BuyTrade : enWalletTrnType.SellTrade;
//                    //WalletProviderHoldObj.enWalletDeductionType = enWalletDeductionType.Normal;

//                    //WalletDrCrResponse WalletProviderHoldResp = await _WalletService.ArbitrageLPGetWalletHoldNew(WalletProviderHoldObj);
//                    // HelperForLog.WriteLogIntoFileAsyncDtTm("Wallet Operator Balance hold END response ", ControllerName, "##ErrorCode:" + WalletProviderHoldResp.ErrorCode + " ##ReturnCode:" + WalletProviderHoldResp.ReturnCode + " ##ReturnMsg:" + WalletProviderHoldResp.ReturnMsg + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST());
//                    //==========================================================

//                    NewtransactionRequest.Status = NewTradetransaction.Status;//rita 25-7-19 set status from tradeTQ

//                    NewtransactionRequest.SetResponse(_TransactionObj.APIResponse);
//                    NewtransactionRequest.SetResponseTime(Helpers.UTC_To_IST());

//                    NewtransactionRequest.SetTrnID(WebAPIParseResponseClsObj.TrnRefNo);
//                    NewtransactionRequest.SetOprTrnID(WebAPIParseResponseClsObj.OperatorRefNo);
//                    _TransactionRequest.Update(NewtransactionRequest);

//                    NewTradetransaction.IsAPITrade = 1;
//                    NewTradetransaction.APIPrice = Req.Price;
//                    NewTradetransaction.SetTransactionStatusMsg(WebAPIParseResponseClsObj.StatusMsg);
//                    if (_Resp.ErrorCode == enErrorCode.API_LP_Filled)
//                    {
//                        NewTradetransaction.APIStatus = "1";
//                    }
//                    else
//                    {
//                        NewTradetransaction.APIStatus = "4";                        
//                    }
//                    _TradeTransactionRepository.Update(NewTradetransaction);
//                    Newtransaction.TrnRefNo = WebAPIParseResponseClsObj.TrnRefNo;
//                    _TransactionRepository.Update(Newtransaction);
//                    if (Req.TrnType == enTrnType.Buy_Trade)
//                    {
//                        //TradeBuyerListObj.Status = NewTradetransaction.Status;
//                        TradeBuyerListObj.IsAPITrade = NewTradetransaction.IsAPITrade;
//                        TradeBuyerListObj.IsProcessing = 0;
//                        _TradeBuyerList.Update(TradeBuyerListObj);
//                    }
//                    else if (Req.TrnType == enTrnType.Sell_Trade)
//                    {
//                        //TradeSellerListObj.Status = NewTradetransaction.Status;
//                        TradeSellerListObj.IsAPITrade = NewTradetransaction.IsAPITrade;
//                        TradeSellerListObj.IsProcessing = 0;
//                        _TradeSellerList.Update(TradeSellerListObj);
//                    }
//                    if (_Resp.ErrorCode == enErrorCode.API_LP_Filled)
//                    {
//                        //await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPI(_Resp, Req.TrnNo,0,Req.Amount,Req.Price);
//                        await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPIFromInit(_Resp, Req.TrnNo, 0, Req.Qty, Req.Price, Newtransaction, NewTradetransaction, TradeStopLossObj, TradeBuyerListObj, TradeSellerListObj);
//                        Newtransaction.CallStatus = 1;
//                    }
//                    else if (_Resp.ErrorCode == enErrorCode.API_LP_PartialFilled)
//                    {
//                        //await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPI(_Resp, Req.TrnNo,0,Req.Amount,Req.Price);
//                        await _SettlementRepositoryAPI.PROCESSSETLLEMENTAPIFromInit(_Resp, Req.TrnNo, LPProcessTransactionClsObj.RemainingQty, LPProcessTransactionClsObj.SettledQty, Req.Price, Newtransaction, NewTradetransaction, TradeStopLossObj, TradeBuyerListObj, TradeSellerListObj);
//                        Newtransaction.CallStatus = 0;
//                    }
//                    else
//                    {
//                        Newtransaction.CallStatus = 0;
//                    }
//                    _TransactionRepository.UpdateField(Newtransaction, e => e.CallStatus);
//                }
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("TradingDataInsertV2 Internal Error: ##IsTxnProceed:" + IsTxnProceed + " ##TrnNo " + Req.TrnNo, ControllerName, ex);
//                if (IsTxnProceed == 0)
//                {
//                    _Resp.ReturnMsg = "Error occured";
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ErrorCode = enErrorCode.ProcessTrn_APICallInternalError;
//                }
//                else
//                {
//                    _Resp.ReturnMsg = "Hold";
//                    _Resp.ReturnCode = enResponseCodeService.Success;

//                    _Resp.ErrorCode = enErrorCode.ProcessTrn_Hold;
//                }

//            }
//            return await Task.FromResult(_Resp);
//        }

//        private async Task<BizResponse> ProcessTransactionOnHuobi(BizResponse _Resp, ServiceProConfigurationArbitrage serviceProConfiguration, List<HuobiSymbol> data, WebAPIParseResponseCls webAPIParseResponseClsObj, TransactionProviderArbitrageResponse provider, long trnNo, ProcessTransactionCls transactionObj, decimal lTP, LPProcessTransactionCls lPProcessTransactionClsObj)
//        {
//            short IsProcessedBit = 0;
//            try
//            {
//                _huobiLPService._client.SetApiCredentials(serviceProConfiguration.APIKey, serviceProConfiguration.SecretKey);
//                string LocalPair = _BaseCurrService.SMSCode + _SecondCurrService.SMSCode;

//                foreach (var obj in data)
//                {
//                    if (LocalPair.ToLower() == obj.Symbol)
//                    {
//                        WebCallResult<long> HuobiResult = await _huobiLPService.PlaceOrder(Req.AccountID, obj.Symbol, Req.TrnType == enTrnType.Sell_Trade ? Huobi.Net.Objects.HuobiOrderType.MarketSell : Huobi.Net.Objects.HuobiOrderType.MarketBuy, Req.Qty, Req.Price);

//                        // var result= "{  \"account-id\": \"100009\",\"amount\": \"10.1\", \"price\": \"100.1\",\"source\": \"api\",\"symbol\": \"ethusdt\"  \"type\": \"buy-limit\"}";
//                        if (HuobiResult == null)
//                        {
//                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                            _Resp.ReturnCode = enResponseCodeService.Fail;
//                            _Resp.ReturnMsg = "Transaction Fail On huobi";
//                            return _Resp;
//                        }
//                        if (HuobiResult.Success)
//                        {
//                            IsProcessedBit = 1;
//                            transactionObj.APIResponse = JsonConvert.SerializeObject(HuobiResult);
//                            //_TransactionObj.APIResponse = JsonConvert.SerializeObject(result);
//                            if (HuobiResult.Success)
//                            {

//                                // CallResult<BittrexAccountOrder> BittrexResult2 = await _bitrexLPService.GetOrderInfoAsync(BittrexResult1.Data.Uuid);
//                                WebCallResult<HuobiOrder> webCall = await _huobiLPService.GetOrderInfoAsync(HuobiResult.Data);

//                                webAPIParseResponseClsObj.TrnRefNo = HuobiResult.Data.ToString();
//                                webAPIParseResponseClsObj.OperatorRefNo = webCall.Data.Id.ToString();
//                                if (webCall.Success)

//                                {

//                                    if (webCall.Data.State == HuobiOrderState.Filled)
//                                    {
//                                        lPProcessTransactionClsObj.RemainingQty = 0;
//                                        lPProcessTransactionClsObj.SettledQty = webCall.Data.FilledAmount;
//                                        lPProcessTransactionClsObj.TotalQty = Req.Qty;
//                                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                                        _Resp.ReturnCode = enResponseCodeService.Success;
//                                        _Resp.ReturnMsg = "Transaction fully Success On Huobi";
//                                        return _Resp;

//                                    }
//                                    else if (webCall.Data.State == HuobiOrderState.PartiallyFilled)
//                                    {
//                                        lPProcessTransactionClsObj.RemainingQty = webCall.Data.Amount - webCall.Data.FilledAmount;
//                                        lPProcessTransactionClsObj.SettledQty = webCall.Data.FilledAmount;
//                                        lPProcessTransactionClsObj.TotalQty = Req.Qty;
//                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
//                                        _Resp.ReturnCode = enResponseCodeService.Success;
//                                        _Resp.ReturnMsg = "Transaction partial Success On huobi";
//                                        return _Resp;
//                                    }
//                                    else if (webCall.Data.State == HuobiOrderState.PartiallyCanceled)
//                                    {
//                                        lPProcessTransactionClsObj.RemainingQty = webCall.Data.FilledAmount;
//                                        lPProcessTransactionClsObj.SettledQty = webCall.Data.FilledAmount;
//                                        lPProcessTransactionClsObj.TotalQty = Req.Qty;
//                                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
//                                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                                        _Resp.ReturnMsg = "Transaction partial FAIL On huobi";
//                                        return _Resp;
//                                    }
//                                    else if (webCall.Data.State == HuobiOrderState.Canceled)
//                                    {
//                                        webAPIParseResponseClsObj.Status = enTransactionStatus.OperatorFail;
//                                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                                        _Resp.ReturnMsg = "Transaction Fail On huobi";
//                                    }
//                                    else if (webCall.Data.State == HuobiOrderState.Created)
//                                    {
//                                        _Resp.ErrorCode = enErrorCode.API_LP_Success;
//                                        _Resp.ReturnCode = enResponseCodeService.Success;
//                                        _Resp.ReturnMsg = "Transaction processing Success On huobi";
//                                        return _Resp;
//                                    }
//                                    else
//                                    {
//                                        IsProcessedBit = 0;
//                                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                                        _Resp.ReturnMsg = "Transaction Fail On huobi";
//                                        return _Resp;
//                                    }


//                                }
//                            }
//                        }
//                    }
//                }
//                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                _Resp.ReturnCode = enResponseCodeService.Fail;
//                _Resp.ReturnMsg = "Transaction Fail On Huobi";

//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("ProcessTransactionHuobi:##TrnNo " + trnNo, ControllerName, ex);
//                if (IsProcessedBit == 1)//Does not proceed on next API
//                {
//                    if (webAPIParseResponseClsObj.Status == enTransactionStatus.Success)
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "Transaction Success On Huobi";
//                        return _Resp;
//                    }
//                    else
//                    {
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                    }
//                }
//                else
//                {
//                    _Resp.ReturnCode = enResponseCodeService.Fail;

//                    _Resp.ReturnMsg = ex.Message;
//                    _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
//                }
//            }
//            return _Resp;
//        }

//        private async Task<BizResponse> ProcessTransactionOnBinance(BizResponse _Resp, ServiceProConfigurationArbitrage ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderArbitrageResponse Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
//        {
//            short IsProcessedBit = 0;
//            try
//            {
//                var BinancePair = _SecondCurrService.SMSCode + _BaseCurrService.SMSCode;
                
//                _binanceLPService._client.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);                
//                Binance.Net.Objects.OrderType type = Req.ordertype == enTransactionMarketType.LIMIT ? Binance.Net.Objects.OrderType.Limit
//                                        : (Req.ordertype == enTransactionMarketType.MARKET ? Binance.Net.Objects.OrderType.Market
//                                        : (Req.ordertype == enTransactionMarketType.STOP ? Binance.Net.Objects.OrderType.StopLoss
//                                        : (Req.ordertype == enTransactionMarketType.STOP_Limit ? Binance.Net.Objects.OrderType.StopLossLimit : Binance.Net.Objects.OrderType.TakeProfit
//                                        )));

//                // commented for testing purpose
//                ////CallResult<BinancePlacedOrder> BinanceResult = await _binanceLPService.PlaceOrderAsync(Req.TrnType == enTrnType.Sell_Trade ? Binance.Net.Objects.OrderSide.Sell : Binance.Net.Objects.OrderSide.Buy, symbol.Name, type, Req.Qty, price: Req.Price, stopPrice: Req.StopPrice, timeInForce: TimeInForce.ImmediateOrCancel);

//                CallResult<BinancePlacedOrder> BinanceResult = await _binanceLPService.PlaceOrderAsync(Req.TrnType == enTrnType.Sell_Trade ? Binance.Net.Objects.OrderSide.Sell : Binance.Net.Objects.OrderSide.Buy, BinancePair, type, quantity: Req.Qty, price: LTP, stopPrice: Req.StopPrice, timeInForce: Binance.Net.Objects.TimeInForce.GoodTillCancel, receiveWindow: 7000);
//                //Khushali 26-07-2019 Stop Price Logic, For Buy order make stop price 3% Up and for Sale order make stop price 3% Down
//                //CallResult<BinancePlacedOrder> BinanceResult = await _binanceLPService.PlaceOrderAsync(Req.TrnType == enTrnType.Sell_Trade ? Binance.Net.Objects.OrderSide.Sell : Binance.Net.Objects.OrderSide.Buy, BinancePair, Binance.Net.Objects.OrderType.StopLoss, quantity: Req.Qty, price: LTP, stopPrice: Req.StopPrice, timeInForce: Binance.Net.Objects.TimeInForce.GoodTillCancel, receiveWindow: 7000);


//                // khushali Testing Resposne 
//                //var Result = @" {""Data"":{""Symbol"":""ETHBTC"",""OrderId"":372478262,""ClientOrderId"":""qyqWCPYsuEXhCx2ICIhpOj"",""TransactTime"":1559044223603,""Price"":0.03090000,""origQty"":0.10000000,""executedQty"":0.10000000,""cummulativeQuoteQty"":0.00309610,""Status"":""FILLED"",""TimeInForce"":""IOC"",""Type"":""LIMIT"",""Side"":""SELL"",""Fills"":[{""TradeId"":124844138,""Price"":0.03096100,""qty"":0.10000000,""Commission"":0.00000310,""CommissionAsset"":""BTC""}]},""Error"":null,""Success"":true}";
//                //CallResult<BinancePlacedOrder> BinanceResult = JsonConvert.DeserializeObject<CallResult<BinancePlacedOrder>>(Result);
//                if (BinanceResult == null)
//                {
//                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Transaction Fail On Binanace";
//                    return _Resp;
//                }
//                _TransactionObj.APIResponse = JsonConvert.SerializeObject(BinanceResult);
//                if (!BinanceResult.Success && BinanceResult.Data == null)
//                {
//                    // Error code for SignalR Response
//                    if (BinanceResult.Error.Code == -1021)
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_Timeout;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_Timeout;
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Error.Message.ToUpper().Contains("LOT_SIZE"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_LOT_SIZE;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_LOT_SIZE;
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Error.Message.ToUpper().Contains("PERCENT_PRICE"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_PERCENT_PRICE;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_PERCENT_PRICE;
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Error.Message.ToUpper().Contains("PRICE_FILTER"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_PRICE_FILTER;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_PRICE_FILTER;
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Error.Message.ToUpper().Contains("MIN_NOTIONAL"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_MIN_NOTIONAL;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_MIN_NOTIONAL;
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Error.Message.ToUpper().Contains("ICEBERG_PARTS"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_ICEBERG_PARTS;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_ICEBERG_PARTS;
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Error.Message.ToUpper().Contains("MARKET_LOT_SIZE"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_MARKET_LOT_SIZE;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_MARKET_LOT_SIZE;
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Error.Message.ToUpper().Contains("MAX_NUM_ORDERS"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_MAX_NUM_ORDERS;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_MAX_NUM_ORDERS;
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Error.Message.ToUpper().Contains("MAX_ALGO_ORDERS"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_MAX_ALGO_ORDERS;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_MAX_ALGO_ORDERS;
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Error.Message.ToUpper().Contains("MAX_NUM_ICEBERG_ORDERS"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_MAX_NUM_ICEBERG_ORDERS;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_MAX_NUM_ICEBERG_ORDERS;
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Error.Message.ToUpper().Contains("EXCHANGE_MAX_NUM_ORDERS"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_EXCHANGE_MAX_NUM_ORDERS;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_EXCHANGE_MAX_NUM_ORDERS;
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Error.Message.ToUpper().Contains("EXCHANGE_MAX_ALGO_ORDERS"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_EXCHANGE_MAX_ALGO_ORDERS;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_EXCHANGE_MAX_ALGO_ORDERS;
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Error.Message.ToUpper().Contains("INSUFFICIENT_FUNDS"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Binance_INSUFFICIENT_FUNDS;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Binance_INSUFFICIENT_FUNDS;
//                        return _Resp;
//                    }
//                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    //_Resp.ReturnMsg = "Transaction Fail On Binanace";
//                    _Resp.ReturnMsg = BinanceResult.Error != null ? BinanceResult.Error.Message : "Transaction Fail On Binanace";
//                    return _Resp;
//                }
//                if (BinanceResult.Success)
//                {
//                    IsProcessedBit = 1;
//                    HelperForLog.WriteLogIntoFile("ProcessTransactionOnBinance:##TrnNo " + TrnNo, ControllerName, JsonConvert.SerializeObject(BinanceResult));
//                    _TransactionObj.APIResponse = JsonConvert.SerializeObject(BinanceResult);

//                    WebAPIParseResponseClsObj.TrnRefNo = BinanceResult.Data.OrderId.ToString();
//                    WebAPIParseResponseClsObj.OperatorRefNo = BinanceResult.Data.ClientOrderId;
//                    WebAPIParseResponseClsObj.Status = BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.New ? enTransactionStatus.Hold :
//                    (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Filled ? enTransactionStatus.Success :
//                    (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PartiallyFilled ? enTransactionStatus.Hold :
//                    (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Rejected ? enTransactionStatus.OperatorFail :
//                    (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Canceled ? enTransactionStatus.OperatorFail : enTransactionStatus.OperatorFail))));

//                    if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Filled)
//                    {
//                        LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
//                        LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
//                        LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "Transaction fully Success On Binanace";
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PartiallyFilled)
//                    {
//                        LPProcessTransactionClsObj.RemainingQty = BinanceResult.Data.OriginalQuantity - BinanceResult.Data.ExecutedQuantity;
//                        LPProcessTransactionClsObj.SettledQty = BinanceResult.Data.ExecutedQuantity;
//                        LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                        _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "Transaction partial Success On Binanace";
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Expired)
//                    {
//                        IsProcessedBit = 0;
//                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = "Transaction Fail On Binanace";
//                        return _Resp;
//                    }
//                    else if (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.New)
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_LP_Success;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "Transaction processing Success On Binanace";
//                        return _Resp;
//                    }
//                    else
//                    {
//                        IsProcessedBit = 0;
//                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = "Transaction Fail On Binanace";
//                        return _Resp;

//                    }

//                }
//                //_Resp.ErrorCode = enErrorCode.Binance_LOT_SIZE;
//                //_Resp.ReturnCode = enResponseCodeService.Fail;
//                //_Resp.ReturnMsg = EnResponseMessage.Binance_LOT_SIZE;
//                //return _Resp;
//                //}
//                //_Resp.ErrorCode = enErrorCode.Binance_PRICE_FILTER;
//                //_Resp.ReturnCode = enResponseCodeService.Fail;
//                //_Resp.ReturnMsg = EnResponseMessage.Binance_PRICE_FILTER;
//                //return _Resp;
//                //}

//                //_Resp.ErrorCode = enErrorCode.Binance_MIN_NOTIONAL;
//                //_Resp.ReturnCode = enResponseCodeService.Fail;
//                //_Resp.ReturnMsg = EnResponseMessage.Binance_MIN_NOTIONAL;
//                //return _Resp;
//                HelperForLog.WriteLogIntoFile("ProcessTransactionOnBinance:##TrnNo ", ControllerName, " ##Response : Not found binance exchange info");
//                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                _Resp.ReturnCode = enResponseCodeService.Fail;
//                _Resp.ReturnMsg = "Transaction Fail On Binanace";
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("ProcessTransactionOnBinance:##TrnNo " + TrnNo, ControllerName, ex);
//                if (IsProcessedBit == 1)//Does not proceed on next API
//                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "Transaction Success On Binanace";
//                        return _Resp;
//                    }
//                    else
//                    {
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                    }
//                else
//                    _Resp.ReturnCode = enResponseCodeService.Fail;

//                _Resp.ReturnMsg = ex.Message;
//                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
//            }
//            return _Resp;
//        }

//        private async Task<BizResponse> ProcessTransactionOnBittrex(BizResponse _Resp, ServiceProConfigurationArbitrage ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderArbitrageResponse Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
//        {
//            short IsProcessedBit = 0;
//            try
//            {
//                //Khushali 26-07-2019 Stop Price Logic, For Buy order make stop price 3% Up and for Sale order make stop price 3% Down
//                #region Stop Price Logic
//                decimal CalculatedPrice = 0;
//                if (Req.ordertype == enTransactionMarketType.LIMIT)
//                {
//                    CalculatedPrice = Convert.ToDecimal(Req.Price * 3 / 100);
//                    if (Req.TrnType == enTrnType.Buy_Trade)
//                    {
//                        Req.StopPrice = Req.Price + CalculatedPrice;
//                    }
//                    else if (Req.TrnType == enTrnType.Sell_Trade)
//                    {
//                        Req.StopPrice = Req.Price - CalculatedPrice;
//                    }
//                }
//                #endregion

//                _bitrexLPService._client.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);
//                _bitrexLPService._Clientv3.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);
//                string LocalPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
//                string BittrexPair = LocalPair.Split("_")[0] + "-" + LocalPair.Split("_")[1];
//                //CallResult<BittrexGuid> BittrexResult1 = await _bitrexLPService.PlaceOrderAsync(Req.TrnType == enTrnType.Sell_Trade ? Bittrex.Net.Objects.OrderSide.Sell : Bittrex.Net.Objects.OrderSide.Buy, BittrexPair, Req.Qty, LTP);
//                WebCallResult<Bittrex.Net.Objects.V3.BittrexOrderV3> BittrexResult1 = await _bitrexLPService.PlaceConditionalOrder(Req.TrnType == enTrnType.Sell_Trade ? Bittrex.Net.Objects.OrderSide.Sell : Bittrex.Net.Objects.OrderSide.Buy, BittrexPair, Req.Qty, LTP);
//                if (BittrexResult1 == null)
//                {
//                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Transaction Fail On Bittrex";
//                    return _Resp;
//                }
//                _TransactionObj.APIResponse = JsonConvert.SerializeObject(BittrexResult1);
//                if (!BittrexResult1.Success && BittrexResult1.Data == null)
//                {
//                    if (BittrexResult1.Error.Message.ToUpper().Contains("INSUFFICIENT_FUNDS"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Bittrex_INSUFFICIENT_FUNDS;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Bittrex_INSUFFICIENT_FUNDS;
//                        return _Resp;
//                    }
//                    else if (BittrexResult1.Error.Message.ToUpper().Contains("MIN_TRADE_REQUIREMENT_NOT_MET"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Bittrex_MIN_TRADE_REQUIREMENT_NOT_MET;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Bittrex_MIN_TRADE_REQUIREMENT_NOT_MET;
//                        return _Resp;
//                    }
//                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = BittrexResult1.Error != null ? BittrexResult1.Error.Message : "Transaction Fail On Bittrex";
//                    return _Resp;
//                }
//                if (BittrexResult1.Success)
//                {
//                    IsProcessedBit = 1;
//                    //CallResult<BittrexAccountOrder> BittrexResult2 = await _bitrexLPService.GetOrderInfoAsync(BittrexResult1.Data.Uuid);
//                    //WebAPIParseResponseClsObj.TrnRefNo = BittrexResult1.Data.Uuid.ToString();
//                    CallResult<BittrexAccountOrder> BittrexResult2 = await _bitrexLPService.GetOrderInfoAsync(new Guid(BittrexResult1.Data.Id));
//                    WebAPIParseResponseClsObj.TrnRefNo = BittrexResult1.Data.Id.ToString();
//                    WebAPIParseResponseClsObj.OperatorRefNo = BittrexResult2.Data.OrderUuid.ToString();
//                    if (BittrexResult2.Success)
//                    {
//                        if (BittrexResult2.Data.QuantityRemaining == 0)
//                        {
//                            LPProcessTransactionClsObj.RemainingQty = BittrexResult2.Data.QuantityRemaining;
//                            LPProcessTransactionClsObj.SettledQty = BittrexResult2.Data.Quantity - BittrexResult2.Data.QuantityRemaining;
//                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                            WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;
//                            _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Transaction fully Success On Bittrex";
//                            return _Resp;
//                        }
//                        else if (BittrexResult2.Data.QuantityRemaining < BittrexResult2.Data.Quantity) // partial
//                        {
//                            LPProcessTransactionClsObj.RemainingQty = BittrexResult2.Data.QuantityRemaining;
//                            LPProcessTransactionClsObj.SettledQty = BittrexResult2.Data.Quantity - BittrexResult2.Data.QuantityRemaining;
//                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                            WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
//                            _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Transaction partial Success On Bittrex";
//                            return _Resp;
//                        }
//                        else if (BittrexResult2.Data.QuantityRemaining == BittrexResult2.Data.Quantity) // hold
//                        {
//                            LPProcessTransactionClsObj.RemainingQty = BittrexResult2.Data.QuantityRemaining;
//                            LPProcessTransactionClsObj.SettledQty = BittrexResult2.Data.Quantity - BittrexResult2.Data.QuantityRemaining;
//                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                            WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
//                            _Resp.ErrorCode = enErrorCode.API_LP_Success; // hold mark
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Transaction processing Success On Bittrex";
//                            return _Resp;
//                        }
//                        else //if (BittrexResult2.Data.CancelInitiated)
//                        {
//                            IsProcessedBit = 0;
//                            WebAPIParseResponseClsObj.Status = enTransactionStatus.OperatorFail;
//                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                            _Resp.ReturnCode = enResponseCodeService.Fail;
//                            _Resp.ReturnMsg = "Transaction Fail On Bittrex";
//                        }
//                    }
//                    else
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_LP_Success; // hold mark
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "Transaction processing Success On Bittrex";
//                        return _Resp;
//                    }
//                }
//                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                _Resp.ReturnCode = enResponseCodeService.Fail;
//                _Resp.ReturnMsg = "Transaction Fail On Bittrex";
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("ProcessTransactionOnBittrex:##TrnNo " + TrnNo, ControllerName, ex);
//                if (IsProcessedBit == 1)//Does not proceed on next API
//                {

//                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "Transaction Success On Bittrex";
//                        return _Resp;
//                    }
//                    else
//                    {
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                    }
//                }
//                else
//                {
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                }
//                _Resp.ReturnMsg = ex.Message;
//                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
//            }
//            return _Resp;
//        }

//        private async Task<BizResponse> ProcessTransactionOnTradeSatoshi(BizResponse _Resp, ServiceProConfigurationArbitrage ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderArbitrageResponse Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
//        {
//            short IsProcessedBit = 0;
//            try
//            {

//                GlobalSettings.API_Key = ServiceProConfiguration.APIKey;
//                GlobalSettings.Secret = ServiceProConfiguration.SecretKey;
//                string TradeSatoshiPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
//                /// commented -- actual code
//                SubmitOrderReturn TradeSatoshiResult = _tradeSatoshiLPService.PlaceOrderAsync(Req.TrnType == enTrnType.Sell_Trade ? Core.Interfaces.LiquidityProvider.OrderSide.Sell : Core.Interfaces.LiquidityProvider.OrderSide.Buy, TradeSatoshiPair, Req.Qty, LTP).Result;

//                //var Result = @" {""success"":true,""message"":null,""result"":{""orderId"":140876176,""filled"":[21474705,21474706,21474707,21474708,21474709,21474710,21474711,21474712,21474713,21474714,21474715,21474716,21474717,21474718,21474719,21474720,21474721,21474722,21474723]}}";
//                //SubmitOrderReturn TradeSatoshiResult = JsonConvert.DeserializeObject<SubmitOrderReturn>(Result);

//                _TransactionObj.APIResponse = JsonConvert.SerializeObject(TradeSatoshiResult);
//                if (TradeSatoshiResult.message != null && TradeSatoshiResult.result == null)
//                {
//                    if (TradeSatoshiResult.message.ToLower().Contains("Insufficient funds"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Tradesatoshi_INSUFFICIENT_FUNDS;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = EnResponseMessage.Tradesatoshi_INSUFFICIENT_FUNDS;
//                        return _Resp;
//                    }
//                }
//                else if (TradeSatoshiResult.success)
//                {
//                    IsProcessedBit = 1;
//                    if (TradeSatoshiResult.result.Filled.Count() > 0 && TradeSatoshiResult.result.OrderId == null)
//                    {
//                        TradeSatoshiResult.result.OrderId = TradeSatoshiResult.result.Filled[0];
//                    }
//                    WebAPIParseResponseClsObj.TrnRefNo = TradeSatoshiResult.result.OrderId.ToString();
//                    WebAPIParseResponseClsObj.OperatorRefNo = TradeSatoshiResult.result.OrderId.ToString();
//                    if (TradeSatoshiResult.result.OrderId != null && TradeSatoshiResult.result.Filled.Count() == 0)
//                    {
//                        GetOrderReturn TradeSatoshiResult1 = await _tradeSatoshiLPService.GetOrderInfoAsync(Convert.ToInt64(TradeSatoshiResult.result.OrderId));

//                        //var Result1 = @" {""success"":true,""message"":null,""result"":{""id"":140876176,""market"":""ETH_BTC"",""type"":""Sell"",""amount"":0.01544508,""rate"":0.03198508,""remaining"":0.01094647,""total"":0.00049401,""status"":""Partial"",""timestamp"":""2019-05-29T11:16:11.527"",""isApi"":true}}";
//                        //GetOrderReturn TradeSatoshiResult1 = JsonConvert.DeserializeObject<GetOrderReturn>(Result1);

//                        //GetOrderReturn TradeSatoshiResult1 = await _tradeSatoshiLPService.GetOrderInfoAsync(TradeSatoshiResult.result.OrderId);
//                        if (TradeSatoshiResult1.success)
//                        {
//                            if (TradeSatoshiResult1.result.Status.ToLower() == "complete")
//                            {
//                                LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult1.result.Remaining;
//                                LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult1.result.Amount;
//                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                                WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;
//                                _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                                _Resp.ReturnCode = enResponseCodeService.Success;
//                                _Resp.ReturnMsg = "Transaction fully Success On TradeSatoshi";
//                                return _Resp;
//                            }
//                            else if (TradeSatoshiResult1.result.Status.ToLower() == "partial")
//                            {
//                                LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult1.result.Remaining;
//                                LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult1.result.Amount - TradeSatoshiResult1.result.Remaining;
//                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                                WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
//                                _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
//                                _Resp.ReturnCode = enResponseCodeService.Success;
//                                _Resp.ReturnMsg = "Transaction partial Success On TradeSatoshi";
//                                return _Resp;
//                            }
//                            else if (TradeSatoshiResult1.result.Status.ToLower() == "pending")
//                            {
//                                LPProcessTransactionClsObj.RemainingQty = TradeSatoshiResult1.result.Remaining;
//                                LPProcessTransactionClsObj.SettledQty = TradeSatoshiResult1.result.Amount - TradeSatoshiResult1.result.Remaining;
//                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                                WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
//                                _Resp.ErrorCode = enErrorCode.API_LP_Success; // hold mark
//                                _Resp.ReturnCode = enResponseCodeService.Success;
//                                _Resp.ReturnMsg = "Transaction Success On TradeSatoshi";
//                                return _Resp;
//                            }
//                            else
//                            {
//                                IsProcessedBit = 0;
//                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                                _Resp.ReturnCode = enResponseCodeService.Fail;
//                                _Resp.ReturnMsg = "Transaction Fail On TradeSatoshi";
//                                return _Resp;
//                            }
//                        }
//                        else
//                        {
//                            IsProcessedBit = 0;
//                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                            _Resp.ReturnCode = enResponseCodeService.Fail;
//                            _Resp.ReturnMsg = "Transaction Fail On TradeSatoshi";
//                            return _Resp;
//                        }
//                    }
//                    else if (TradeSatoshiResult.result.Filled.Count() > 0)
//                    {
//                        LPProcessTransactionClsObj.RemainingQty = 0;
//                        LPProcessTransactionClsObj.SettledQty = Req.Qty;
//                        LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                        WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;
//                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "Transaction fully Success On TradeSatoshi";
//                        return _Resp;

//                    }
//                    else
//                    {
//                        IsProcessedBit = 0;
//                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = "Transaction Fail On TradeSatoshi";
//                        return _Resp;
//                    }
//                }
//                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                _Resp.ReturnCode = enResponseCodeService.Fail;
//                _Resp.ReturnMsg = "Transaction Fail On TradeSatoshi";
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("ProcessTransactionOnTradeSatoshi:##TrnNo " + TrnNo, ControllerName, ex);
//                if (IsProcessedBit == 1)//Does not proceed on next API
//                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "Transaction Success On Bittrex";
//                        return _Resp;
//                    }
//                    else
//                    {
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                    }
//                else
//                    _Resp.ReturnCode = enResponseCodeService.Fail;

//                _Resp.ReturnMsg = ex.Message;
//                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
//            }
//            return _Resp;
//        }

//        private async Task<BizResponse> ProcessTransactionOnTradePoloniex(BizResponse _Resp, ServiceProConfigurationArbitrage ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderArbitrageResponse Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
//        {
//            short IsProcessedBit = 0;
//            try
//            {
//                PoloniexGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
//                PoloniexGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
//                PoloniexOrderResult PoloniexResult = new PoloniexOrderResult();
//                PoloniexErrorObj errorObj = new PoloniexErrorObj();

//                var PoloniexRes = await _poloniexService.PlacePoloniexOrder(_BaseCurrService.SMSCode, _SecondCurrService.SMSCode, Req.Qty, LTP, Req.TrnType == enTrnType.Sell_Trade ? enOrderType.SellOrder : enOrderType.BuyOrder);
//                if (PoloniexRes == null)
//                {
//                    errorObj = JsonConvert.DeserializeObject<PoloniexErrorObj>(PoloniexRes);
//                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Transaction Fail On Poloniex";
//                    return _Resp;
//                }
//                if (PoloniexRes != null && PoloniexRes.ToLower().Contains("error"))
//                {
//                    errorObj = JsonConvert.DeserializeObject<PoloniexErrorObj>(PoloniexRes);
//                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Transaction Fail On Poloniex";
//                    return _Resp;
//                }
//                IsProcessedBit = 1;
//                _TransactionObj.APIResponse = PoloniexRes;

//                PoloniexResult = JsonConvert.DeserializeObject<PoloniexOrderResult>(PoloniexRes);
//                if (PoloniexResult.resultingTrades == null)
//                {
//                    errorObj = JsonConvert.DeserializeObject<PoloniexErrorObj>(PoloniexRes);
//                    _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Transaction Fail On Poloniex";
//                    return _Resp;
//                }
//                if (PoloniexResult.orderNumber != null)
//                {
//                    WebAPIParseResponseClsObj.TrnRefNo = PoloniexResult.orderNumber.ToString();
//                    WebAPIParseResponseClsObj.OperatorRefNo = PoloniexResult.orderNumber.ToString();
//                    WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;

//                    object PoloniexRep = await _poloniexService.GetPoloniexOrderState(WebAPIParseResponseClsObj.TrnRefNo);
//                    JObject Data = JObject.Parse(PoloniexRep.ToString());
//                    var Success = Convert.ToUInt16(Data["result"]["success"]);
//                    if (Success == 1)
//                    {
//                        JToken Result = Data["result"][WebAPIParseResponseClsObj.TrnRefNo];
//                        PoloniexOrderState PoloniexResult1 = JsonConvert.DeserializeObject<PoloniexOrderState>(Result.ToString());
//                        if (PoloniexResult1.status == "Partially filled")
//                        {
//                            LPProcessTransactionClsObj.RemainingQty = PoloniexResult1.amount - PoloniexResult1.startingAmount;
//                            LPProcessTransactionClsObj.SettledQty = PoloniexResult1.amount;
//                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                            WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
//                            _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Transaction partial Success On Bittrex";
//                            return _Resp;
//                        }
//                        else if (PoloniexResult1.status == "Filled")
//                        {
//                            LPProcessTransactionClsObj.RemainingQty = 0;
//                            LPProcessTransactionClsObj.SettledQty = PoloniexResult1.startingAmount;
//                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                            WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;
//                            _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Transaction fully Success On Bittrex";
//                            return _Resp;
//                        }
//                    }

//                    _Resp.ErrorCode = enErrorCode.API_LP_Success;
//                    _Resp.ReturnCode = enResponseCodeService.Success;
//                    _Resp.ReturnMsg = "Transaction processing Success On Poloniex";
//                    return _Resp;
//                }

//                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                _Resp.ReturnCode = enResponseCodeService.Fail;
//                _Resp.ReturnMsg = "Transaction Fail On Poloniex";

//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("ProcessTransactionOnTradePoloniex:##TrnNo " + TrnNo, ControllerName, ex);
//                if (IsProcessedBit == 1)//Does not proceed on next API
//                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "Transaction Success On Bittrex";
//                        return _Resp;
//                    }
//                    else
//                    {
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                    }
//                else
//                    _Resp.ReturnCode = enResponseCodeService.Fail;

//                _Resp.ReturnMsg = ex.Message;
//                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
//            }
//            return _Resp;
//        }

//        private async Task<BizResponse> ProcessTransactionOnTradeCoinbase(BizResponse _Resp, ServiceProConfigurationArbitrage ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderArbitrageResponse Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
//        {
//            short IsProcessedBit = 0;
//            try
//            {
//                CoinBaseGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
//                CoinBaseGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
//                string LocalPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
//                string CoinBasePair = Char.ToUpperInvariant(LocalPair.Split("_")[1][0]) + LocalPair.Split("_")[1].Substring(1).ToLower() + Char.ToUpperInvariant(LocalPair.Split("_")[0][0]) + LocalPair.Split("_")[0].Substring(1).ToLower();
//                OrderResponse CoinbaseResult = await _coinBaseService.PlaceOrder(Req.ordertype, Req.TrnType == enTrnType.Sell_Trade ? CoinbasePro.Services.Orders.Types.OrderSide.Sell : CoinbasePro.Services.Orders.Types.OrderSide.Buy, CoinBasePair, Req.Qty, LTP, Req.StopPrice);
//                IsProcessedBit = 1;
//                _TransactionObj.APIResponse = JsonConvert.SerializeObject(CoinbaseResult);
//                if (CoinbaseResult.Status == CoinbasePro.Services.Orders.Types.OrderStatus.Active)
//                {
//                    WebAPIParseResponseClsObj.TrnRefNo = CoinbaseResult.Id.ToString();
//                    WebAPIParseResponseClsObj.OperatorRefNo = CoinbaseResult.Id.ToString();
//                    WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;

//                    _Resp.ErrorCode = enErrorCode.API_LP_Success;
//                    _Resp.ReturnCode = enResponseCodeService.Success;
//                    _Resp.ReturnMsg = "Transaction processing Success On Coinbase";
//                    return _Resp;
//                }

//                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                _Resp.ReturnCode = enResponseCodeService.Fail;
//                _Resp.ReturnMsg = "Transaction Fail On Coinbase";

//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("ProcessTransactionOnTradeCoinbase:##TrnNo " + TrnNo, ControllerName, ex);
//                if (IsProcessedBit == 1)//Does not proceed on next API
//                    _Resp.ReturnCode = enResponseCodeService.Success;
//                else
//                    _Resp.ReturnCode = enResponseCodeService.Fail;

//                _Resp.ReturnMsg = ex.Message;
//                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
//            }
//            return _Resp;
//        }

//        #region OKEX Place Order 
//        /// <summary>
//        /// Add new Place order for OKEX API by Pushpraj as on 19-06-2019
//        /// </summary>
//        /// <param name="_Resp"></param>
//        /// <param name="ServiceProConfiguration"></param>
//        /// <param name="WebAPIParseResponseClsObj"></param>
//        /// <param name="Provider"></param>
//        /// <param name="TrnNo"></param>
//        /// <param name="_TransactionObj"></param>
//        /// <param name="LTP"></param>
//        /// <param name="LPProcessTransactionClsObj"></param>
//        /// <returns></returns>
//        private async Task<BizResponse> ProcessTransactionOnOKEX(BizResponse _Resp, ServiceProConfiguration ServiceProConfiguration, WebAPIParseResponseCls WebAPIParseResponseClsObj, TransactionProviderResponse Provider, long TrnNo, ProcessTransactionCls _TransactionObj, decimal LTP, LPProcessTransactionCls LPProcessTransactionClsObj)
//        {
//            short IsProcessedBit = 0;
//            try
//            {

//                OKEXGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
//                OKEXGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
//                OKEXGlobalSettings.PassPhrase = ServiceProConfiguration.Param1;
//                string OKEXPair = _SecondCurrService.SMSCode + "_" + _BaseCurrService.SMSCode;
//                /// commented -- actual code
//                //SubmitOrderReturn Tradestresult = _tradeSatoshiLPService.PlaceOrderAsync(Req.TrnType == enTrnType.Sell_Trade ? Core.Interfaces.LiquidityProvider.OrderSide.Sell : Core.Interfaces.LiquidityProvider.OrderSide.Buy, TradeSatoshiPair, Req.Qty, LTP).Result;

//                OKExPlaceOrderReturn OKExResult = _oKExLPService.PlaceOrderAsync(OKEXPair, Req.TrnType == enTrnType.Sell_Trade ? Core.Interfaces.LiquidityProvider.OrderSide.Sell.ToString() : Core.Interfaces.LiquidityProvider.OrderSide.Buy.ToString(), LTP, Req.Qty, 10, "y12233456", "0", "0").Result;

//                //var Result = @" {""success"":true,""message"":null,""result"":{""orderId"":140876176,""filled"":[21474705,21474706,21474707,21474708,21474709,21474710,21474711,21474712,21474713,21474714,21474715,21474716,21474717,21474718,21474719,21474720,21474721,21474722,21474723]}}";
//                //SubmitOrderReturn TradeSatoshiResult = JsonConvert.DeserializeObject<SubmitOrderReturn>(Result);

//                _TransactionObj.APIResponse = JsonConvert.SerializeObject(OKExResult);
//                if (OKExResult.result == true)
//                {
//                    IsProcessedBit = 1;
//                    WebAPIParseResponseClsObj.TrnRefNo = OKExResult.order_id.ToString();
//                    WebAPIParseResponseClsObj.OperatorRefNo = OKExResult.order_id.ToString();
//                    if (OKExResult.order_id != null)
//                    {
//                        OKExGetOrderInfoReturn OKExOrderReturn = await _oKExLPService.GetOrderInfoAsync(OKEXPair, OKExResult.order_id, OKExResult.client_oid);

//                        //var Result1 = @" {""success"":true,""message"":null,""result"":{""id"":140876176,""market"":""ETH_BTC"",""type"":""Sell"",""amount"":0.01544508,""rate"":0.03198508,""remaining"":0.01094647,""total"":0.00049401,""status"":""Partial"",""timestamp"":""2019-05-29T11:16:11.527"",""isApi"":true}}";
//                        //GetOrderReturn TradeSatoshiResult1 = JsonConvert.DeserializeObject<GetOrderReturn>(Result1);

//                        //GetOrderReturn TradeSatoshiResult1 = await _tradeSatoshiLPService.GetOrderInfoAsync(TradeSatoshiResult.result.OrderId);
//                        if (OKExOrderReturn != null)
//                        {
//                            if (OKExOrderReturn.status == "2")
//                            {
//                                LPProcessTransactionClsObj.RemainingQty = Req.Qty - decimal.Parse(OKExOrderReturn.filled_qty);
//                                LPProcessTransactionClsObj.SettledQty = decimal.Parse(OKExOrderReturn.filled_qty);
//                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                                WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;
//                                _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                                _Resp.ReturnCode = enResponseCodeService.Success;
//                                _Resp.ReturnMsg = "Transaction fully Success On OKEX";
//                                return _Resp;
//                            }
//                            else if (OKExOrderReturn.status == "1")
//                            {
//                                LPProcessTransactionClsObj.RemainingQty = Req.Qty - decimal.Parse(OKExOrderReturn.filled_qty);
//                                LPProcessTransactionClsObj.SettledQty = Req.Qty - decimal.Parse(OKExOrderReturn.filled_qty) - (Req.Qty - decimal.Parse(OKExOrderReturn.filled_qty));
//                                LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                                WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
//                                _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
//                                _Resp.ReturnCode = enResponseCodeService.Success;
//                                _Resp.ReturnMsg = "Transaction partial Success On OKEX";
//                                return _Resp;
//                            }
//                            else
//                            {
//                                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                                _Resp.ReturnCode = enResponseCodeService.Fail;
//                                _Resp.ReturnMsg = "Transaction Fail On OKEX";
//                                return _Resp;
//                            }
//                        }
//                        else
//                        {
//                            _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                            _Resp.ReturnCode = enResponseCodeService.Fail;
//                            _Resp.ReturnMsg = "Transaction Fail On OKEX";
//                            return _Resp;
//                        }
//                    }
//                    else
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = "Transaction Fail On OKEX";
//                        return _Resp;
//                    }
//                }

//                _Resp.ErrorCode = enErrorCode.API_LP_Fail;
//                _Resp.ReturnCode = enResponseCodeService.Fail;
//                _Resp.ReturnMsg = "Transaction Fail On OKEX";

//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("ProcessTransactionOnOKEX:##TrnNo " + TrnNo, ControllerName, ex);
//                if (IsProcessedBit == 1)//Does not proceed on next API
//                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "Transaction Success On OKEX";
//                        return _Resp;
//                    }
//                    else
//                    {
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                    }
//                else
//                    _Resp.ReturnCode = enResponseCodeService.Fail;

//                _Resp.ReturnMsg = ex.Message;
//                _Resp.ErrorCode = enErrorCode.API_LP_InternalError;
//            }
//            return _Resp;
//        }

//        #endregion

//        #region ==============================CCXT TRANSACTION======================== 

//        public async Task<BizResponse> CCXTCreateTransactionAsync(TransactionQueueArbitrage TQ, TradeTransactionQueueArbitrage TradeTQ, TransactionProviderArbitrageResponse Provider, ProcessTransactionCls _TransactionObj, LPProcessTransactionCls LPProcessTransactionClsObj)
//        {
//            //string APIResponse = string.Empty;
//            BizResponse _Resp = new BizResponse();
//            ThirdPartyAPIRequestArbitrage ThirdPartyAPIRequestOnj;
//            //WebAPIParseResponseCls WebAPIParseResponseClsObj = new WebAPIParseResponseCls();
//            short IsAPIProceed = 0;
//            short Retry = 0;
//            try
//            {
//                RetryWithNewToken:
//                string Token = await ConnectToExchangeAsync(Provider, TQ, TQ.Id);
//                if (!string.IsNullOrEmpty(Token))
//                {

//                    ThirdPartyAPIRequestOnj = _IGetWebRequest.ArbitrageMakeWebRequest(Provider.RouteID, Provider.ThirdPartyAPIID, Provider.SerProDetailID, TQ, TradeTQ, Token: Token);

//                    //APIResponse = _IWebApiSendRequest.SendAPIRequestAsync(ThirdPartyAPIRequestOnj.RequestURL, ThirdPartyAPIRequestOnj.RequestBody, ThirdPartyAPIRequestOnj.ContentType, 30000, ThirdPartyAPIRequestOnj.keyValuePairsHeader, ThirdPartyAPIRequestOnj.MethodType);
//                    _TransactionObj.APIResponse = await _IWebApiSendRequest.SendRequestAsyncLPArbitrage(ThirdPartyAPIRequestOnj.RequestURL, ref IsAPIProceed, ThirdPartyAPIRequestOnj.RequestBody, ThirdPartyAPIRequestOnj.MethodType, ThirdPartyAPIRequestOnj.ContentType, ThirdPartyAPIRequestOnj.keyValuePairsHeader, 15000, TQ.Id);

//                    if (_TransactionObj.APIResponse == "" && IsAPIProceed == 0)
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_APICallfail;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = "API Call Fail";
//                        return _Resp;
//                    }
//                    else if (_TransactionObj.APIResponse == "" && IsAPIProceed == 1)//API call done
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_APICallProceeWithNoResponse;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "API Call Procee With NoResponse";
//                        return _Resp;
//                    }

//                    WebAPIParseResponseClsObj = _WebApiParseResponseObj.ArbitrageTransactionParseResponse(_TransactionObj.APIResponse, Provider.ThirdPartyAPIID,0);

//                    //Token expired then create new
//                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.OperatorFail && WebAPIParseResponseClsObj.ResponseMsg.ToLower().Contains("invalid token") && Retry==0)
//                    {                        
//                        _cache.Set<string>("LPType" + Provider.LPType, null);
//                        Retry = 1;

//                        goto RetryWithNewToken;
//                        //_Resp.ErrorCode = enErrorCode.API_ResponseFail;
//                        //_Resp.ReturnCode = enResponseCodeService.Fail;
//                        //_Resp.ReturnMsg = WebAPIParseResponseClsObj.ResponseMsg;
//                        //return _Resp;
//                    }
//                   else if (WebAPIParseResponseClsObj.Status == enTransactionStatus.OperatorFail)
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_ResponseFail;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = WebAPIParseResponseClsObj.ResponseMsg;
//                        return _Resp;
//                    }
//                    else if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success || WebAPIParseResponseClsObj.Status == enTransactionStatus.Hold)
//                    {
//                        decimal Param1=0;
//                        decimal Param2=0;
//                        //nor working for decimal value
//                        //if (!WebAPIParseResponseClsObj.Param1.All(char.IsNumber) || !WebAPIParseResponseClsObj.Param2.All(char.IsNumber))
//                        //{
//                        //    _Resp.ErrorCode = enErrorCode.API_ResponseParsingFail;
//                        //    _Resp.ReturnCode = enResponseCodeService.Success;
//                        //    _Resp.ReturnMsg = "Response Parsing Fail";
//                        //    return _Resp;

//                        //}  
//                        if(WebAPIParseResponseClsObj.Param1.Contains("E")|| WebAPIParseResponseClsObj.Param1.Contains("e"))
//                        {
//                            WebAPIParseResponseClsObj.Param1 = decimal.Parse(WebAPIParseResponseClsObj.Param1, System.Globalization.NumberStyles.Float).ToString();
//                        }
//                        if (WebAPIParseResponseClsObj.Param2.Contains("E") || WebAPIParseResponseClsObj.Param2.Contains("e"))
//                        {
//                            WebAPIParseResponseClsObj.Param2 = decimal.Parse(WebAPIParseResponseClsObj.Param2, System.Globalization.NumberStyles.Float).ToString();
//                        }
//                        if (!Helpers.IsValidDecimal(WebAPIParseResponseClsObj.Param1,ref Param1) || !Helpers.IsValidDecimal(WebAPIParseResponseClsObj.Param2, ref Param2))
//                        {
//                            _Resp.ErrorCode = enErrorCode.API_ResponseParsingFail;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Response Parsing Fail";
//                            return _Resp;

//                        }
//                        if (WebAPIParseResponseClsObj.StatusMsg.ToLower() == "closed")
//                        {
//                            LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(WebAPIParseResponseClsObj.Param2);//remaining
//                            LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(WebAPIParseResponseClsObj.Param1);//Filled
//                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                            _Resp.ErrorCode = enErrorCode.API_LP_Filled;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Transaction fully Success On API";
//                            return _Resp;
//                        }
//                        else if (WebAPIParseResponseClsObj.StatusMsg.ToLower() == "open" && WebAPIParseResponseClsObj.Param1=="0")//Hold
//                        {
//                            LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(WebAPIParseResponseClsObj.Param2);//remaining
//                            LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(WebAPIParseResponseClsObj.Param1);//Filled
//                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                            _Resp.ErrorCode = enErrorCode.API_LP_Hold;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Transaction Hold On API";
//                            return _Resp;
//                        }
//                        else if (WebAPIParseResponseClsObj.StatusMsg.ToLower() == "open" && WebAPIParseResponseClsObj.Param1 != "0")//Partial settlement
//                        {
//                            LPProcessTransactionClsObj.RemainingQty = Convert.ToDecimal(WebAPIParseResponseClsObj.Param2);//remaining
//                            LPProcessTransactionClsObj.SettledQty = Convert.ToDecimal(WebAPIParseResponseClsObj.Param1);//Filled
//                            LPProcessTransactionClsObj.TotalQty = Req.Qty;
//                            _Resp.ErrorCode = enErrorCode.API_LP_PartialFilled;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Transaction partial Success On API";
//                            return _Resp;
//                        }
//                        else
//                        {
//                            _Resp.ErrorCode = enErrorCode.API_ResponseParsingFail;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Response Parsing Fail.";
//                            return _Resp;
//                        }

//                    }
                    
//                }
//                else
//                {
//                    _Resp.ErrorCode = enErrorCode.API_AuthenticationFail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Tokane Generation Fail";
//                    return _Resp;
//                }
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("CCXTCreateTransactionAsync:##TrnNo " + TQ.Id, ControllerName, ex);
//                _Resp.ErrorCode = enErrorCode.API_LPCreateOrderInternalError;
//                _Resp.ReturnMsg = "CreateOrder InternalError";
//                if (IsAPIProceed == 1)
//                {
//                    WebAPIParseResponseClsObj.Status = enTransactionStatus.Hold;
//                    _Resp.ReturnCode = enResponseCodeService.Success;
//                }
//                else
//                {
//                    WebAPIParseResponseClsObj.Status = enTransactionStatus.OperatorFail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                }
//            }
//            return _Resp;
//        }

//        public async Task<string> ConnectToExchangeAsync(TransactionProviderArbitrageResponse Provider, TransactionQueueArbitrage TQ , long TrnNo)
//        {
//            string APIResponse = string.Empty;
//            ThirdPartyAPIRequestArbitrage ThirdPartyAPIRequestOnj;
//            WebAPIParseResponseCls WebAPIParseResponseClsObj = new WebAPIParseResponseCls();
//            short IsAPIProceed = 0;

//            try
//            {
//                string cacheToken = _cache.Get<string>("LPType" + Provider.LPType);
//                if (cacheToken == null || cacheToken=="")
//                {

//                    ThirdPartyAPIRequestOnj = _IGetWebRequest.ArbitrageMakeWebRequest(Provider.RouteID, Provider.ThirdPartyAPIID, Provider.SerProDetailID, TQ, null, IsValidateUrl: 1,TrnNo: TrnNo.ToString());

//                    APIResponse = await _IWebApiSendRequest.SendRequestAsyncLPArbitrage(ThirdPartyAPIRequestOnj.RequestURL, ref IsAPIProceed, ThirdPartyAPIRequestOnj.RequestBody, ThirdPartyAPIRequestOnj.MethodType, ThirdPartyAPIRequestOnj.ContentType, ThirdPartyAPIRequestOnj.keyValuePairsHeader, 15000, TrnNo,false);

//                    WebAPIParseResponseClsObj = _WebApiParseResponseObj.ArbitrageTransactionParseResponse(APIResponse, Provider.ThirdPartyAPIID, 0);

//                    _cache.Set("LPType" + Provider.LPType, WebAPIParseResponseClsObj.OperatorRefNo);
//                    return WebAPIParseResponseClsObj.OperatorRefNo;
//                }
//                else
//                {
//                    return cacheToken;

//                }                

//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
//                return "";
//            }
//        }
//        #endregion

//        public long InsertTransactionRequest(TransactionProviderArbitrageResponse listObj, string Request)
//        {
//            try
//            {
//                NewtransactionRequest = new ArbitrageTransactionRequest()
//                {
//                    TrnNo = Req.TrnNo,
//                    ServiceID = 0,
//                    SerProID = listObj.ProviderID,
//                    SerProDetailID = listObj.SerProDetailID,
//                    CreatedDate = Helpers.UTC_To_IST(),
//                    RequestData = Request
//                };
//                NewtransactionRequest = _TransactionRequest.Add(NewtransactionRequest);
//                return NewtransactionRequest.Id;

//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("InsertTransactionRequest:##TrnNo " + Req.TrnNo, ControllerName, ex);
//                return 0;
//            }
//        }

//        #region Send SMS And Email
//        public async Task SMSSendTransactionHoldOrFailed(long TrnNo, string MobileNumber, decimal Price, decimal Qty, int type)
//        {
//            try
//            {
//                if (!string.IsNullOrEmpty(MobileNumber))
//                {
//                    TemplateMasterData SmsData = new TemplateMasterData();
//                    SendSMSRequest SendSMSRequestObj = new SendSMSRequest();
//                    ApplicationUser User = new ApplicationUser();

//                    CommunicationParamater communicationParamater = new CommunicationParamater();
//                    communicationParamater.Param1 = TrnNo + "";
//                    communicationParamater.Param2 = Price + "";
//                    communicationParamater.Param3 = Qty + "";

//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SendSMSTransaction - SMSSendTransactionHoldOrFailed", ControllerName, " ##TrnNo : " + TrnNo + " ##MobileNo : " + MobileNumber + " ##Price : " + Price + " ##Qty : " + Qty + " ##Type : " + type, Helpers.UTC_To_IST()));

//                    if (type == 1) // Transaction Created
//                    {
//                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_TransactionCreated, communicationParamater, enCommunicationServiceType.SMS).Result;
//                    }
//                    else if (type == 2) // Transaction Failed
//                    {
//                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_TransactionFailed, communicationParamater, enCommunicationServiceType.SMS).Result;
//                    }

//                    if (SmsData != null)
//                    {
//                        if (SmsData.IsOnOff == 1)
//                        {
//                            SendSMSRequestObj.Message = SmsData.Content;
//                            SendSMSRequestObj.MobileNo = Convert.ToInt64(MobileNumber);
//                            _pushSMSQueue.Enqueue(SendSMSRequestObj);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Task.Run(() => HelperForLog.WriteErrorLog("SMSSendTransactionHold:##TrnNo " + TrnNo, ControllerName, ex));
//            }
//        }

//        public async Task EmailSendTransactionHoldOrFailed(long TrnNo, string UserId, long pairid, decimal qty, string datetime, decimal price, decimal fee, int Type, short OrderType, short TrnType)
//        {
//            try
//            {
//                SendEmailRequest Request = new SendEmailRequest();
//                ApplicationUser User = new ApplicationUser();
//                TemplateMasterData EmailData = new TemplateMasterData();
//                CommunicationParamater communicationParamater = new CommunicationParamater();

//                User = await _userManager.FindByIdAsync(UserId);
//                if (!string.IsNullOrEmpty(User.Email))
//                {
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SendEmailTransaction - EmailSendWithdrwalTransaction", ControllerName, " ##TrnNo : " + TrnNo + " ##Type : " + Type, Helpers.UTC_To_IST()));

//                    var pairdata = _trnMasterConfiguration.GetTradePairMaster().Where(x => x.Id == pairid).FirstOrDefault();

//                    if (pairdata != null)
//                    {
//                        communicationParamater.Param1 = pairdata.PairName + "";
//                        communicationParamater.Param3 = pairdata.PairName.Split("_")[1];
//                    }

//                    communicationParamater.Param8 = User.Name + "";
//                    communicationParamater.Param2 = Helpers.DoRoundForTrading(qty, 8).ToString();
//                    communicationParamater.Param4 = datetime;
//                    communicationParamater.Param5 = Helpers.DoRoundForTrading(price, 8).ToString();
//                    communicationParamater.Param6 = Helpers.DoRoundForTrading(fee, 8).ToString();
//                    communicationParamater.Param7 = Helpers.DoRoundForTrading(0, 8).ToString();  //Uday 01-01-2019  In failed transaction final price as 0
//                    communicationParamater.Param9 = ((enTransactionMarketType)OrderType).ToString();  //Uday 01-01-2019 Add OrderType In Email
//                    communicationParamater.Param10 = ((enTrnType)TrnType).ToString();  //Uday 01-01-2019 Add TranType In Email
//                    communicationParamater.Param11 = TrnNo.ToString(); //Uday 01-01-2019 Add TrnNo In Email

//                    //if (CancelType == 1) // Hold
//                    //{
//                    //    EmailData = _messageService.SendMessageAsync(EnTemplateType.EMAIL_OrderCancel, communicationParamater, enCommunicationServiceType.Email).Result;
//                    //}
//                    if (Type == 2) // Failed
//                    {
//                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_TransactionFailed, communicationParamater, enCommunicationServiceType.Email).Result;
//                    }

//                    if (EmailData != null)
//                    {
//                        Request.Body = EmailData.Content;
//                        Request.Subject = EmailData.AdditionalInfo;
//                        Request.Recepient = User.Email;
//                        Request.EmailType = 0;
//                        _pushNotificationsQueue.Enqueue(Request);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Task.Run(() => HelperForLog.WriteErrorLog("EmailSendCancelTransaction:##TrnNo " + TrnNo, ControllerName, ex));
//            }
//        }
//        #endregion
//    }
//}
