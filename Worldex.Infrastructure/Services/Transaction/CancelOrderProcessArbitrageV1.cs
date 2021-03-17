//using Binance.Net;
//using Binance.Net.Objects;
//using Bittrex.Net;
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
//using Worldex.Core.ViewModels;
//using Worldex.Core.ViewModels.LiquidityProvider;
//using Worldex.Core.ViewModels.LiquidityProvider1;
//using Worldex.Core.ViewModels.Wallet;
//using Worldex.Infrastructure.BGTask;
//using Worldex.Infrastructure.Data;
//using Worldex.Infrastructure.DTOClasses;
//using Worldex.Infrastructure.Interfaces;
//using Worldex.Infrastructure.LiquidityProvider;
//using Worldex.Infrastructure.LiquidityProvider.TradeSatoshiAPI;
//using CryptoExchange.Net.Authentication;
//using CryptoExchange.Net.Objects;
//using Huobi.Net;
//using Microsoft.AspNetCore.Identity;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Worldex.Infrastructure.LiquidityProvider.OKExAPI;
//using Worldex.Infrastructure.LiquidityProvider.UpbitAPI;
//using Microsoft.Extensions.Caching.Memory;
//using System.Linq;

//namespace Worldex.Infrastructure.Services.Transaction
//{
//    public class CancelOrderProcessArbitrageV1 : ICancelOrderProcessArbitrageV1
//    {
//        private readonly EFCommonRepository<TransactionQueueArbitrage> _TransactionRepository;
//        private readonly EFCommonRepository<TradeTransactionQueueArbitrage> _TradeTransactionRepository;
//        private readonly ICancelOrderRepositoryArbitrage _ICancelOrderRepository;
//        private readonly ICommonRepository<TradeCancelQueueArbitrage> _TradeCancelQueueRepository;
//        private readonly ICommonRepository<TradeBuyerListArbitrageV1> _TradeBuyerList;
//        private readonly ICommonRepository<TradeSellerListArbitrageV1> _TradeSellerList;
//        private readonly ISignalRService _ISignalRService;
//        private readonly ICommonRepository<TradeStopLossArbitrage> _TradeStopLoss;
//        TradeCancelQueueArbitrage tradeCancelQueue;
//        private readonly IArbitrageWalletService _WalletService;//Rita 05-12-18 for release remain wallet amount
//        private readonly IMessageService _messageService;
//        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
//        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
//        private readonly UserManager<ApplicationUser> _userManager;
//        //komal liquidity provider 
//        private readonly EFCommonRepository<ServiceProviderDetailArbitrage> _ServiceProviderDetail;
//        private readonly EFCommonRepository<ArbitrageTransactionRequest> _TranRequestRepository;
//        private readonly IGetWebRequest _IGetWebRequest;
//        private readonly BinanceLPService _binanceLPService;
//        private readonly BitrexLPService _bitrexLPService;
//        private readonly ICoinBaseService _coinBaseService;
//        private readonly IPoloniexService _poloniexService;
//        private readonly ITradeSatoshiLPService _tradeSatoshiLPService;
//        ArbitrageTransactionRequest NewtransactionReq;
//        private readonly IOKExLPService _oKExLPService; //Add new variable for OKEx API by Pushpraj as on 19-06-2019
//        private readonly IHuobiLPService _huobiLPService;
//        private readonly IUpbitService _upbitService;
//        private readonly ICommonRepository<ArbitrageTransactionRequest> _TransactionRequest;
//        private readonly ITradeReconProcessV1 _tradeReconProcessV1;
//        private readonly NewTransactionArbitrageV1 _NewTransactionArbitrageV1;
//        private readonly IWebApiRepository _WebApiRepository;
//        private readonly IWebApiSendRequest _IWebApiSendRequest;
//        WebApiParseResponse _WebApiParseResponseObj;
//        string APIResponse = "";
//        WebAPIParseResponseCls WebAPIParseResponseClsObj = new WebAPIParseResponseCls();

//        //PoolOrder PoolOrderObj;//komal 03 May 2019, Cleanup
//        string ControllerName = "CancelOrderProcessArbitrageV1";
//        decimal CancelOrderPrice = 0, CancelOrderQty = 0, TotalQty = 0;
//        private IMemoryCache _cache;

//        public CancelOrderProcessArbitrageV1(EFCommonRepository<TradeTransactionQueueArbitrage> TradeTransactionRepository,
//            ICommonRepository<TradeCancelQueueArbitrage> TradeCancelQueueRepository, ICancelOrderRepositoryArbitrage ICancelOrderRepository,
//            EFCommonRepository<TransactionQueueArbitrage> TransactionRepository, ICommonRepository<TradeBuyerListArbitrageV1> TradeBuyerList,
//            ICommonRepository<TradeSellerListArbitrageV1> TradeSellerList, ISignalRService ISignalRService, IArbitrageWalletService WalletService,
//            ICommonRepository<TradeStopLossArbitrage> TradeStopLoss, IMessageService messageService, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue,
//            IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, UserManager<ApplicationUser> userManager,
//            EFCommonRepository<ServiceProviderDetailArbitrage> ServiceProviderDetail, EFCommonRepository<ArbitrageTransactionRequest> TranRequestRepository,
//            IGetWebRequest IGetWebRequest, BinanceLPService BinanceLPService, IUpbitService upbitService,
//            BitrexLPService BitrexLPService, ICoinBaseService CoinBaseService, IPoloniexService PoloniexService,
//            ITradeSatoshiLPService TradeSatoshiLPService, ICommonRepository<ArbitrageTransactionRequest> TransactionRequest,
//            ITradeReconProcessV1 tradeReconProcessV1, IOKExLPService oKExLPService, IHuobiLPService huobiLPService, IMemoryCache cache,
//            NewTransactionArbitrageV1 NewTransactionArbitrageV1,IWebApiRepository WebApiRepository, IWebApiSendRequest IWebApiSendRequest,
//            WebApiParseResponse WebApiParseResponseObj)
//        {
//            _upbitService = upbitService;
//            _TradeTransactionRepository = TradeTransactionRepository;
//            _TradeCancelQueueRepository = TradeCancelQueueRepository;
//            _ICancelOrderRepository = ICancelOrderRepository;
//            _TransactionRepository = TransactionRepository;
//            _TradeBuyerList = TradeBuyerList;
//            _TradeSellerList = TradeSellerList;
//            _ISignalRService = ISignalRService;
//            _TradeStopLoss = TradeStopLoss;
//            _WalletService = WalletService;
//            _messageService = messageService;
//            _pushSMSQueue = pushSMSQueue;
//            _pushNotificationsQueue = pushNotificationsQueue;
//            _userManager = userManager;
//            _ServiceProviderDetail = ServiceProviderDetail;
//            _TranRequestRepository = TranRequestRepository;
//            _IGetWebRequest = IGetWebRequest;
//            _binanceLPService = BinanceLPService;
//            _bitrexLPService = BitrexLPService;
//            _coinBaseService = CoinBaseService;
//            _poloniexService = PoloniexService;
//            _tradeSatoshiLPService = TradeSatoshiLPService;
//            _huobiLPService = huobiLPService;
//            _TransactionRequest = TransactionRequest;
//            _tradeReconProcessV1 = tradeReconProcessV1;
//            _oKExLPService = oKExLPService; //Add new variable assignment for OKEx API by Pushpraj as on 19-06-2019
//            _cache = cache;
//            _NewTransactionArbitrageV1 = NewTransactionArbitrageV1;
//            _WebApiRepository = WebApiRepository;
//            _IWebApiSendRequest = IWebApiSendRequest;
//            _WebApiParseResponseObj = WebApiParseResponseObj;
//        }

//        public async Task<BizResponse> ProcessCancelOrderAsyncV1(NewCancelOrderArbitrageRequestCls Req)
//        {
//            BizResponse _Resp = new BizResponse();
//            List<NewCancelOrderArbitrageRequestCls> CancelObjList = new List<NewCancelOrderArbitrageRequestCls>();
//            if (Req.CancelAll == 0) //komal 28-01-2019 for cancel multiple trxn 
//            {
//                CancelObjList.Add(new NewCancelOrderArbitrageRequestCls()
//                {
//                    accessToken = Req.accessToken,
//                    TranNo = Req.TranNo,
//                    IsFromStuckOrder=Req.IsFromStuckOrder
//                });
//            }
//            if (Req.CancelAll == 1) //komal 28-01-2019 for cancel multiple trxn 
//            {
//                var AllTrxn = _TradeTransactionRepository.FindBy(e => e.MemberID == Req.MemberID && e.Status == 4);
//                foreach (var Trxn in AllTrxn)
//                {
//                    CancelObjList.Add(new NewCancelOrderArbitrageRequestCls()
//                    {
//                        accessToken = Req.accessToken,
//                        TranNo = Trxn.TrnNo,
//                        IsFromStuckOrder = Req.IsFromStuckOrder
//                    });
//                }
//            }
//            else if (Req.CancelAll == 2) //komal 28-01-2019 for cancel multiple trxn 
//            {
//                var AllTrxn = _TradeTransactionRepository.FindBy(e => e.MemberID == Req.MemberID && e.Status == 4 && e.ordertype == Convert.ToInt16(Req.OrderType));
//                foreach (var Trxn in AllTrxn)
//                {
//                    CancelObjList.Add(new NewCancelOrderArbitrageRequestCls()
//                    {
//                        accessToken = Req.accessToken,
//                        TranNo = Trxn.TrnNo,
//                        IsFromStuckOrder = Req.IsFromStuckOrder
//                    });
//                }
//            }
//            foreach (var obj in CancelObjList)
//            {
//                _Resp = await MiddleWareForCancellationProcess(obj);
//                Task.Run(() => HelperForLog.WriteLogIntoFile("ProcessCancelOrderAsyncV1 Code " + _Resp.ErrorCode, ControllerName, _Resp.ReturnMsg + " ##TrnNo:" + obj.TranNo));

//                if (_Resp.ReturnCode == enResponseCodeService.Fail)//on validation fail send Notification
//                {
//                    try
//                    {
//                        ActivityNotificationMessage notification = new ActivityNotificationMessage();
//                        //notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);
//                        notification.MsgCode = Convert.ToInt32(_Resp.ErrorCode);//rita 13-6-19 for display proper reason

//                        //notification.Param1 = Req.TranNo.ToString();
//                        notification.Param1 = obj.TranNo.ToString(); //komal 31-01-2019  change Obj.TrnNo
//                        notification.Type = Convert.ToInt16(EnNotificationType.Fail);
//                        //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);
//                        _ISignalRService.SendActivityNotificationV2Arbitrage(notification, Req.MemberID.ToString(), 2);

//                    }
//                    catch (Exception ex)
//                    {
//                        //Task.Run(() => HelperForLog.WriteLogIntoFile("SendPushNotification ISignalRService Notification Error-ProcessCancelOrderAsyncV1", ControllerName, ex.Message + "##TrnNo:" + Req.TranNo));
//                        Task.Run(() => HelperForLog.WriteErrorLog("SendPushNotification ISignalRService Notification Error-ProcessCancelOrderAsyncV1 " + "##TrnNo:" + Req.TranNo, ControllerName, ex));
//                    }
//                }
//            }
//            //Task.Delay(5000).Wait();//Rita 27-2-19 wait for all calls
//            return _Resp;
//        }

//        public async Task<BizResponse> MiddleWareForCancellationProcess(NewCancelOrderArbitrageRequestCls Req)
//        {
//            BizResponse _Resp = new BizResponse();
//            try
//            {
//                TransactionQueueArbitrage TransactionQueueObj = _TransactionRepository.GetSingle(item => item.Id == Req.TranNo);
//                TradeTransactionQueueArbitrage TradeTranQueueObj = _TradeTransactionRepository.GetSingle(item => item.TrnNo == Req.TranNo);
//                if (TradeTranQueueObj == null)
//                {
//                    _Resp.ErrorCode = enErrorCode.CancelOrder_NoRecordFound;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "No Record Found";
//                    return _Resp;
//                }
//                Req.MemberID = TradeTranQueueObj.MemberID;//rita 3-1-19 instead of access tocken
//                if (TradeTranQueueObj.Status != Convert.ToInt16(enTransactionStatus.Hold))
//                {
//                    _Resp.ErrorCode = enErrorCode.CancelOrder_TrnNotHold;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Order is not in pending State";
//                    return _Resp;
//                }
//                if (TradeTranQueueObj.IsCancelled == 1 || TradeTranQueueObj.IsAPICancelled == 1)//Rita 5-2-19 , this bit only set for API trading cancellation
//                {
//                    _Resp.ErrorCode = enErrorCode.CancelOrder_OrderalreadyCancelled;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Transaction Cancellation request is already in processing";
//                    return _Resp;
//                }

//                if (TradeTranQueueObj.IsAPITrade == 0)//Local Trade
//                {
//                    _Resp = await CancellationProcessV1(_Resp, TransactionQueueObj, TradeTranQueueObj, Req.accessToken);
//                }
//                else//API trade
//                {
//                    _Resp = await CancellationProcessAPIV1(_Resp, TransactionQueueObj, TradeTranQueueObj, Req.accessToken,Req.IsFromStuckOrder);
//                }

//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("MiddleWareForCancellationProcess " + "##TrnNo:" + Req.TranNo, ControllerName, ex);
//                _Resp.ErrorCode = enErrorCode.CancelOrder_InternalError;
//                _Resp.ReturnCode = enResponseCodeService.Fail;
//                _Resp.ReturnMsg = "Internal Error";
//            }
//            return _Resp;
//        }

//        public async Task<BizResponse> CancellationProcessV1(BizResponse _Resp, TransactionQueueArbitrage TransactionQueueObj, TradeTransactionQueueArbitrage TradeTransactionQueueObj, string accessToken)
//        {
//            short ISPartialSettled = 0;
//            CancelOrderPrice = 0; CancelOrderQty = 0;
//            TradeBuyerListArbitrageV1 BuyerListObj = new TradeBuyerListArbitrageV1();
//            TradeSellerListArbitrageV1 SellerListObj = new TradeSellerListArbitrageV1();
//            try
//            {
//                await CancellQueueEntry(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.ServiceID, 0, 0, 0, 0, TradeTransactionQueueObj.MemberID);
//                tradeCancelQueue.Status = 6;

//                //TradeTransactionQueueObj.SetTransactionStatusMsg("Cancellation Initiated");
//                //TradeTransactionQueueObj.IsCancelled = 1;
//                decimal ReleaseAmt = 0;

//                if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
//                {
//                    //BuyerListObj = _TradeBuyerList.GetSingle(e => e.IsProcessing == 0 && e.TrnNo == TradeTransactionQueueObj.TrnNo);
//                    BuyerListObj = _TradeBuyerList.GetSingle(e => e.TrnNo == TradeTransactionQueueObj.TrnNo);
//                    if (BuyerListObj == null)//No list Found , then skip and cancel order
//                    {
//                        ISPartialSettled = 0;// not any settlement proceed this type of txn
//                        tradeCancelQueue.DeliverQty = 0;
//                        tradeCancelQueue.PendingBuyQty = TradeTransactionQueueObj.BuyQty;   //total pending                    
//                        CancelOrderPrice = TradeTransactionQueueObj.BidPrice;
//                        CancelOrderQty = TradeTransactionQueueObj.BuyQty;
//                        TotalQty = TradeTransactionQueueObj.BuyQty;
//                    }
//                    else // exist entry in buyer queue
//                    {
//                        //Rita 6-5-19 remove && BuyerListObj.Status!=0
//                        if (BuyerListObj.IsProcessing == 1 && BuyerListObj.CreatedDate > Helpers.UTC_To_IST().AddMinutes(-10))//Rita 25-4-19 15 minute old,make cancel
//                        {
//                            //Uday 07-12-2018 Send SMS For Order Cancel failed
//                            SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TradeTransactionQueueObj.BidPrice, TradeTransactionQueueObj.BuyQty, TransactionQueueObj.MemberMobile, 3);

//                            //Uday 07-12-2018 Send Email For Order Cancel failed
//                            EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.BuyQty, TradeTransactionQueueObj.TrnDate + "", TradeTransactionQueueObj.BidPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

//                            _Resp.ErrorCode = enErrorCode.CancelOrder_UnderProcessing;
//                            _Resp.ReturnCode = enResponseCodeService.Fail;
//                            _Resp.ReturnMsg = "Order is under processing,try after some time";
//                            return _Resp;
//                        }
//                        else if (BuyerListObj.IsProcessing == 1)//Rita 26-4-19 does not update before this code , as this method updates entity in DB
//                        {
//                            // await _tradeReconProcessV1.ProcessReleaseStuckOrderOrderAsync(new BizResponseClass(), TradeTransactionQueueObj, TradeTransactionQueueObj.MemberID, 1);
//                        }

//                        if (BuyerListObj.RemainQty == BuyerListObj.Qty)//Not Procced
//                        {
//                            ISPartialSettled = 0;
//                            BuyerListObj.MakeTransactionOperatorFail();
//                        }
//                        else//Partial Settled
//                        {
//                            ISPartialSettled = 1;
//                            BuyerListObj.MakeTransactionSuccess();
//                        }
//                        tradeCancelQueue.DeliverQty = BuyerListObj.DeliveredQty;
//                        tradeCancelQueue.PendingBuyQty = BuyerListObj.RemainQty;

//                        //Uday 07-12-2018 Send SMS For Order Cancel
//                        CancelOrderPrice = BuyerListObj.Price;
//                        CancelOrderQty = BuyerListObj.RemainQty;
//                        TotalQty = TradeTransactionQueueObj.BuyQty;
//                    }
//                    BuyerListObj.IsProcessing = 0;//Rita 24-6-19 release isprocesssing
//                    //ReleaseAmt = BuyerListObj.RemainQty * BuyerListObj.Price;//Second curr canculate base on remain Qty
//                    ReleaseAmt = TradeTransactionQueueObj.OrderTotalQty - TradeTransactionQueueObj.SettledSellQty;//rita 18-12-18 for market order price is zero
//                }
//                else if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
//                {
//                    //SellerListObj = _TradeSellerList.GetSingle(e => e.IsProcessing == 0 && e.TrnNo == TradeTransactionQueueObj.TrnNo);
//                    SellerListObj = _TradeSellerList.GetSingle(e => e.TrnNo == TradeTransactionQueueObj.TrnNo);
//                    if (SellerListObj == null)//No list Found , then skip and cancel order
//                    {
//                        ISPartialSettled = 0;// not any settlement proceed this type of txn
//                        tradeCancelQueue.DeliverQty = 0;
//                        tradeCancelQueue.PendingBuyQty = TradeTransactionQueueObj.SellQty;
//                        CancelOrderPrice = TradeTransactionQueueObj.AskPrice;
//                        CancelOrderQty = TradeTransactionQueueObj.SellQty;
//                        TotalQty = TradeTransactionQueueObj.SellQty;
//                        ReleaseAmt = TradeTransactionQueueObj.SellQty;
//                    }
//                    else // exist entry in buyer queue
//                    {
//                        //Rita 6-5-19 remove && BuyerListObj.Status!=0
//                        if (SellerListObj.IsProcessing == 1 && SellerListObj.CreatedDate > Helpers.UTC_To_IST().AddMinutes(-10))//Rita 25-4-19 15 minute old,make cancel
//                        {
//                            //Uday 07-12-2018 Send SMS For Order Cancel failed
//                            SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TradeTransactionQueueObj.AskPrice, TradeTransactionQueueObj.SellQty, TransactionQueueObj.MemberMobile, 3);

//                            //Uday 07-12-2018 Send Email For Order Cancel failed
//                            EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.SellQty, TradeTransactionQueueObj.TrnDate + "", TradeTransactionQueueObj.AskPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

//                            _Resp.ErrorCode = enErrorCode.CancelOrder_UnderProcessing;
//                            _Resp.ReturnCode = enResponseCodeService.Fail;
//                            _Resp.ReturnMsg = "Order is under processing,try after some time";
//                            return _Resp;
//                        }
//                        else if (SellerListObj.IsProcessing == 1)//Rita 26-4-19 does not update before this code , as this method updates entity in DB
//                        {
//                            //await _tradeReconProcessV1.ProcessReleaseStuckOrderOrderAsync(new BizResponseClass(), TradeTransactionQueueObj, TradeTransactionQueueObj.MemberID, 1);
//                        }

//                        if (SellerListObj.RemainQty == SellerListObj.Qty)//Not Procced
//                        {
//                            ISPartialSettled = 0;
//                            SellerListObj.MakeTransactionOperatorFail();
//                        }
//                        else//Partial Settled
//                        {
//                            ISPartialSettled = 1;
//                            SellerListObj.MakeTransactionSuccess();
//                        }
//                        tradeCancelQueue.DeliverQty = SellerListObj.SelledQty;
//                        tradeCancelQueue.PendingBuyQty = SellerListObj.RemainQty;
//                        ReleaseAmt = SellerListObj.RemainQty;//First curr direct
//                        //Uday 07-12-2018 Send SMS For Order Cancel
//                        CancelOrderPrice = SellerListObj.Price;
//                        CancelOrderQty = SellerListObj.RemainQty;
//                        TotalQty = TradeTransactionQueueObj.SellQty;
//                    }
//                    SellerListObj.IsProcessing = 0;//Rita 24-6-19 release isprocesssing
//                }
//                else
//                {
//                    //Uday 07-12-2018 Send SMS For Order Cancel failed
//                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TradeTransactionQueueObj.AskPrice, TradeTransactionQueueObj.SellQty, TransactionQueueObj.MemberMobile, 3);

//                    //Uday 07-12-2018 Send Email For Order Cancel failed
//                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.SellQty, TradeTransactionQueueObj.TrnDate + "", TradeTransactionQueueObj.AskPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

//                    _Resp.ErrorCode = enErrorCode.CancelOrder_StockErrorOccured;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Stock Error Occured";
//                    return _Resp;
//                }
//                TradeTransactionQueueObj.SetTransactionStatusMsg("Cancellation Initiated");
//                TradeTransactionQueueObj.IsCancelled = 1;
//                //Add Wallet Call here as Release Remain Wallet Amount
//                //enWalletTrnType.Cr_ReleaseBlockAmount Rita 16-1-18 change enum as waller team changes enums
//                Task<WalletDrCrResponse> WalletResult = _WalletService.ArbitrageGetReleaseHoldNew(TradeTransactionQueueObj.Order_Currency, Helpers.GetTimeStamp(), ReleaseAmt,
//                                                 TradeTransactionQueueObj.OrderAccountID, TradeTransactionQueueObj.TrnNo, enServiceType.Trading,
//                                                 enWalletTrnType.ReleaseBlockAmount, (enTrnType)TransactionQueueObj.TrnType, (EnAllowedChannels)TransactionQueueObj.TrnMode,
//                                                 accessToken);

//                SettledTradeTransactionQueueArbitrage SettledTradeTQObj = new SettledTradeTransactionQueueArbitrage();
//                if (ISPartialSettled == 1)//Make Success
//                {
//                    Task<SettledTradeTransactionQueueArbitrage> SettledTradeTQResult = MakeTransactionSettledEntry(TradeTransactionQueueObj, CancelOrderPrice, 1);
//                    TradeTransactionQueueObj.MakeTransactionSuccess();
//                    TradeTransactionQueueObj.SetTransactionStatusMsg("Success with partial cancellation");
//                    TradeTransactionQueueObj.SettledDate = Helpers.UTC_To_IST();
//                    TransactionQueueObj.MakeTransactionSuccess();
//                    TransactionQueueObj.SetTransactionStatusMsg("Success with partial cancellation");
//                    SettledTradeTQObj = await SettledTradeTQResult;
//                }
//                else//Make Fail
//                {
//                    TradeTransactionQueueObj.MakeTransactionOperatorFail();
//                    TradeTransactionQueueObj.SetTransactionStatusMsg("Full Order Cancellation");
//                    TradeTransactionQueueObj.SettledDate = Helpers.UTC_To_IST();
//                    TransactionQueueObj.MakeTransactionOperatorFail();
//                    TransactionQueueObj.SetTransactionStatusMsg("Full Order Cancellation");
//                }
//                //rita 29-11-18 update errorcode
//                TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));

//                tradeCancelQueue.Status = 1;
//                tradeCancelQueue.SettledDate = Helpers.UTC_To_IST();
//                tradeCancelQueue.UpdatedDate = Helpers.UTC_To_IST();
//                tradeCancelQueue.StatusMsg = "Cancellation Successful.";
//                var WalletResp = await WalletResult;

//                if (WalletResp.ReturnCode != enResponseCode.Success)
//                {
//                    //Uday 07-12-2018 Send SMS For Order Cancel failed
//                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, CancelOrderPrice, CancelOrderQty, TransactionQueueObj.MemberMobile, 3);

//                    //Uday 07-12-2018 Send Email For Order Cancel failed
//                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, CancelOrderQty, TradeTransactionQueueObj.TrnDate + "", CancelOrderPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

//                    _Resp.ErrorCode = enErrorCode.CancelOrder_CancelProcessFail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Cancel Process Fail as Wallet Process Fail " + WalletResp.ReturnMsg;
//                    return _Resp;
//                }

//                var ResultBool = _ICancelOrderRepository.UpdateDataObjectWithBeginTransactionV1(tradeCancelQueue, TransactionQueueObj, TradeTransactionQueueObj, BuyerListObj, SellerListObj, SettledTradeTQObj, ISPartialSettled);
//                if (!ResultBool)
//                {
//                    //Uday 07-12-2018 Send SMS For Order Cancel failed
//                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, CancelOrderPrice, CancelOrderQty, TransactionQueueObj.MemberMobile, 3);

//                    //Uday 07-12-2018 Send Email For Order Cancel failed
//                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, CancelOrderQty, TradeTransactionQueueObj.TrnDate + "", CancelOrderPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

//                    _Resp.ErrorCode = enErrorCode.CancelOrder_FailInTransactionDataUpdate;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Cancel Process Fail as TransactionData not updated";
//                    return _Resp;
//                }

//                //Uday 07-12-2018 Send SMS and email When Order Cancel
//                if (ISPartialSettled != 1) // Full Cancel
//                {
//                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, CancelOrderPrice, CancelOrderQty, TransactionQueueObj.MemberMobile, 2);

//                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, CancelOrderQty, TradeTransactionQueueObj.TrnDate + "", CancelOrderPrice, 0, 1, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);
//                }
//                else // Partial Cancel
//                {
//                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, CancelOrderPrice, CancelOrderQty, TransactionQueueObj.MemberMobile, 1);

//                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, TotalQty, TradeTransactionQueueObj.TrnDate + "", CancelOrderPrice, 0, 2, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType, (TotalQty - CancelOrderQty), CancelOrderQty);
//                }

//                try
//                {
//                    var MakeTradeStopLossObj = _TradeStopLoss.GetSingle(e => e.TrnNo == TransactionQueueObj.Id);
//                    //Rita 19-07-19 added await for context error
//                    await _ISignalRService.OnStatusCancelArbitrage(TradeTransactionQueueObj.Status, TransactionQueueObj, TradeTransactionQueueObj, "", MakeTradeStopLossObj.ordertype, ISPartialSettled);

//                    //await EmailSendAsync(TransactionQueueObj.MemberID.ToString(), Convert.ToInt16(enTransactionStatus.Success), TradeTransactionQueueObj.PairName,
//                    //          TradeTransactionQueueObj.PairName.Split("_")[1], TradeTransactionQueueObj.TrnDate.ToString(),
//                    //          TradeTransactionQueueObj.DeliveryTotalQty, TradeTransactionQueueObj.OrderTotalQty, 0);
//                }
//                catch (Exception ex)
//                {
//                    //HelperForLog.WriteLogIntoFile("ISignalRService CancellationProcessV1", ControllerName, "Error " + ex.Message + "##TrnNo:" + TradeTransactionQueueObj.TrnNo);
//                    HelperForLog.WriteErrorLog("ISignalRService CancellationProcessV1 Error ##TrnNo:" + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                }
//                Task.Delay(5000).Wait();//Rita 27-2-19 wait for all calls
//                _Resp.ErrorCode = enErrorCode.CancelOrder_ProccedSuccess;
//                _Resp.ReturnCode = enResponseCodeService.Success;
//                _Resp.ReturnMsg = "Order Cancellled Successfully";
//                return _Resp;
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("CancellationProcess V1:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                //_dbContext.Database.RollbackTransaction();
//            }
//            return _Resp;
//        }

//        public async Task CancellQueueEntry(long TrnNo, long DeliverServiceID, decimal PendingBuyQty, decimal DeliverQty, short OrderType, decimal DeliverBidPrice, long UserID)
//        {
//            try
//            {
//                tradeCancelQueue = new TradeCancelQueueArbitrage()
//                {
//                    TrnNo = TrnNo,
//                    DeliverServiceID = DeliverServiceID,
//                    TrnDate = Helpers.UTC_To_IST(),
//                    PendingBuyQty = PendingBuyQty,
//                    DeliverQty = DeliverQty,
//                    OrderType = OrderType,
//                    DeliverBidPrice = DeliverBidPrice,
//                    Status = 0,
//                    OrderID = 0,
//                    SettledDate = Helpers.UTC_To_IST(),
//                    StatusMsg = "Cancel Order",
//                    CreatedBy = UserID,
//                    CreatedDate = Helpers.UTC_To_IST()
//                };
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("CancellQueueEntry:##TrnNo " + TrnNo, ControllerName, ex);
//            }
//        }

//        public async Task<SettledTradeTransactionQueueArbitrage> MakeTransactionSettledEntry(TradeTransactionQueueArbitrage TradeTransactionQueueObj, decimal Price, short IsCancelled = 0)
//        {
//            decimal BidPrice = TradeTransactionQueueObj.BidPrice;
//            decimal AskPrice = TradeTransactionQueueObj.AskPrice;
//            //rita 7-1-18 need for graph canculation
//            //rita 3-1-18 set same price also in market order
//            if (TradeTransactionQueueObj.ordertype == 2)
//            {
//                if (TradeTransactionQueueObj.TrnType == 4)//Buy
//                {
//                    BidPrice = Price;
//                }
//                else//Sell
//                {
//                    AskPrice = Price;
//                }
//            }
//            try
//            {
//                var SettledTradeTQObj = new SettledTradeTransactionQueueArbitrage()
//                {
//                    TrnNo = TradeTransactionQueueObj.TrnNo,
//                    TrnDate = TradeTransactionQueueObj.TrnDate,
//                    SettledDate = Helpers.UTC_To_IST(),
//                    TrnType = TradeTransactionQueueObj.TrnType,
//                    TrnTypeName = TradeTransactionQueueObj.TrnTypeName,
//                    MemberID = TradeTransactionQueueObj.MemberID,
//                    PairID = TradeTransactionQueueObj.PairID,
//                    PairName = TradeTransactionQueueObj.PairName,
//                    OrderWalletID = TradeTransactionQueueObj.OrderWalletID,
//                    DeliveryWalletID = TradeTransactionQueueObj.DeliveryWalletID,
//                    OrderAccountID = TradeTransactionQueueObj.OrderAccountID,
//                    DeliveryAccountID = TradeTransactionQueueObj.DeliveryAccountID,
//                    BuyQty = TradeTransactionQueueObj.BuyQty,
//                    BidPrice = BidPrice,
//                    SellQty = TradeTransactionQueueObj.SellQty,
//                    AskPrice = AskPrice,
//                    Order_Currency = TradeTransactionQueueObj.Order_Currency,
//                    OrderTotalQty = TradeTransactionQueueObj.OrderTotalQty,
//                    Delivery_Currency = TradeTransactionQueueObj.Delivery_Currency,
//                    DeliveryTotalQty = TradeTransactionQueueObj.DeliveryTotalQty,
//                    SettledBuyQty = TradeTransactionQueueObj.SettledBuyQty,
//                    SettledSellQty = TradeTransactionQueueObj.SettledSellQty,
//                    Status = 1,//TradeTransactionQueueObj.Status,//rita 3-1-19
//                    StatusCode = TradeTransactionQueueObj.StatusCode,
//                    StatusMsg = TradeTransactionQueueObj.StatusMsg,
//                    IsCancelled = IsCancelled
//                };
//                return SettledTradeTQObj;
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("MakeTransactionSettledEntry:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                throw ex;
//            }
//        }

//        public async Task<BizResponse> CancellationProcessAPIV1(BizResponse _Resp, TransactionQueueArbitrage TransactionQueueObj, TradeTransactionQueueArbitrage TradeTransactionQueueObj, string accessToken, short IsFromStuckOrder=0)
//        {
//            short ISPartialSettled = 0;
//            CancelOrderPrice = 0; CancelOrderQty = 0;
//            TradeBuyerListArbitrageV1 BuyerListObj = new TradeBuyerListArbitrageV1();
//            TradeSellerListArbitrageV1 SellerListObj = new TradeSellerListArbitrageV1();
//            try
//            {
//                await CancellQueueEntry(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.ServiceID, 0, 0, 0, 0, TradeTransactionQueueObj.MemberID);
//                tradeCancelQueue.Status = 6;

//                TradeTransactionQueueObj.SetTransactionStatusMsg("Cancellation Initiated");
//                TradeTransactionQueueObj.IsAPICancelled = 1;
//                decimal ReleaseAmt = 0;

//                if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
//                {
//                    BuyerListObj = _TradeBuyerList.GetSingle(e => e.TrnNo == TradeTransactionQueueObj.TrnNo);
//                    if (BuyerListObj == null)
//                    {
//                        //Uday 07-12-2018 Send SMS For Order Cancel failed
//                        SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TradeTransactionQueueObj.BidPrice, TradeTransactionQueueObj.BuyQty, TransactionQueueObj.MemberMobile, 3);

//                        //Uday 07-12-2018 Send Email For Order Cancel failed
//                        EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.BuyQty, TradeTransactionQueueObj.TrnDate + "", TradeTransactionQueueObj.BidPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

//                        _Resp.ErrorCode = enErrorCode.CancelOrder_UnderProcessing;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = "Order is under processing,try after some time";
//                        return _Resp;
//                    }
//                    if (BuyerListObj.RemainQty == BuyerListObj.Qty)//Not Procced
//                    {
//                        ISPartialSettled = 0;
//                        BuyerListObj.MakeTransactionOperatorFail();
//                    }
//                    else//Partial Settled
//                    {
//                        ISPartialSettled = 1;
//                        BuyerListObj.MakeTransactionSuccess();
//                    }
//                    tradeCancelQueue.DeliverQty = BuyerListObj.DeliveredQty;
//                    tradeCancelQueue.PendingBuyQty = BuyerListObj.RemainQty;
//                    //ReleaseAmt = BuyerListObj.RemainQty * BuyerListObj.Price;//Second curr canculate base on remain Qty
//                    ReleaseAmt = TradeTransactionQueueObj.OrderTotalQty - TradeTransactionQueueObj.SettledSellQty;//rita 18-12-18 for market order price is zero

//                    //Uday 07-12-2018 Send SMS For Order Cancel
//                    CancelOrderPrice = BuyerListObj.Price;
//                    CancelOrderQty = BuyerListObj.RemainQty;
//                    TotalQty = TradeTransactionQueueObj.BuyQty;
//                    BuyerListObj.IsProcessing = 0;//Rita 24-6-19 release isprocesssing
//                }
//                else if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
//                {
//                    SellerListObj = _TradeSellerList.GetSingle(e =>e.TrnNo == TradeTransactionQueueObj.TrnNo);
//                    if (SellerListObj == null)
//                    {
//                        //Uday 07-12-2018 Send SMS For Order Cancel failed
//                        SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TradeTransactionQueueObj.AskPrice, TradeTransactionQueueObj.SellQty, TransactionQueueObj.MemberMobile, 3);

//                        //Uday 07-12-2018 Send Email For Order Cancel failed
//                        EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.SellQty, TradeTransactionQueueObj.TrnDate + "", TradeTransactionQueueObj.AskPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

//                        _Resp.ErrorCode = enErrorCode.CancelOrder_UnderProcessing;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = "Order is under processing,try after some time";
//                        return _Resp;
//                    }
//                    if (SellerListObj.RemainQty == SellerListObj.Qty)//Not Procced
//                    {
//                        ISPartialSettled = 0;
//                        SellerListObj.MakeTransactionOperatorFail();
//                    }
//                    else//Partial Settled
//                    {
//                        ISPartialSettled = 1;
//                        SellerListObj.MakeTransactionSuccess();
//                    }
//                    tradeCancelQueue.DeliverQty = SellerListObj.SelledQty;
//                    tradeCancelQueue.PendingBuyQty = SellerListObj.RemainQty;
//                    ReleaseAmt = SellerListObj.RemainQty;//First curr direct

//                    //Uday 07-12-2018 Send SMS For Order Cancel
//                    CancelOrderPrice = SellerListObj.Price;
//                    CancelOrderQty = SellerListObj.RemainQty;
//                    TotalQty = TradeTransactionQueueObj.SellQty;
//                    SellerListObj.IsProcessing = 0;//Rita 24-6-19 release isprocesssing
//                }
//                else
//                {
//                    //Uday 07-12-2018 Send SMS For Order Cancel failed
//                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TradeTransactionQueueObj.AskPrice, TradeTransactionQueueObj.SellQty, TransactionQueueObj.MemberMobile, 3);

//                    //Uday 07-12-2018 Send Email For Order Cancel failed
//                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.SellQty, TradeTransactionQueueObj.TrnDate + "", TradeTransactionQueueObj.AskPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

//                    _Resp.ErrorCode = enErrorCode.CancelOrder_StockErrorOccured;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Stock Error Occured";
//                    return _Resp;
//                }
//                if (IsFromStuckOrder == 0)//rita 25-6-19 does not check in api if call from crone
//                {
//                    //Call API for cancellation
//                    LPCancelOrdedrResponse APIResponse = LiquidityTradeCancelProcess(TradeTransactionQueueObj.TrnNo, TradeTransactionQueueObj).Result;
//                    //int APIResponse = 0;
//                    if (APIResponse.ReturnCode == enResponseCodeService.Fail)//Fail
//                    {
//                        _Resp.ErrorCode = APIResponse.ErrorCode;// enErrorCode.CancelOrder_APICallFail;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = "API fail";
//                        return _Resp;
//                    }
//                }
//                //If at a time cancel then update this
//                TradeTransactionQueueObj.IsCancelled = 1;//else update after status check

//                //Add Wallet Call here as Release Remain Wallet Amount
//                //enWalletTrnType.Cr_ReleaseBlockAmount Rita 16-1-18 change enum as waller team changes enums
//                Task<WalletDrCrResponse> WalletResult = _WalletService.ArbitrageGetReleaseHoldNew(TradeTransactionQueueObj.Order_Currency, Helpers.GetTimeStamp(), ReleaseAmt,
//                                                 TradeTransactionQueueObj.OrderAccountID, TradeTransactionQueueObj.TrnNo, enServiceType.Trading,
//                                                 enWalletTrnType.ReleaseBlockAmount, (enTrnType)TransactionQueueObj.TrnType, (EnAllowedChannels)TransactionQueueObj.TrnMode,
//                                                 accessToken);

//                SettledTradeTransactionQueueArbitrage SettledTradeTQObj = new SettledTradeTransactionQueueArbitrage();
//                if (ISPartialSettled == 1)//Make Success
//                {
//                    Task<SettledTradeTransactionQueueArbitrage> SettledTradeTQResult = MakeTransactionSettledEntry(TradeTransactionQueueObj, CancelOrderPrice, 1);
//                    TradeTransactionQueueObj.MakeTransactionSuccess();
//                    TradeTransactionQueueObj.SetTransactionStatusMsg("Success with partial cancellation");
//                    TradeTransactionQueueObj.SettledDate = Helpers.UTC_To_IST();
//                    TransactionQueueObj.MakeTransactionSuccess();
//                    TransactionQueueObj.SetTransactionStatusMsg("Success with partial cancellation");
//                    SettledTradeTQObj = await SettledTradeTQResult;
//                }
//                else//Make Fail
//                {
//                    TradeTransactionQueueObj.MakeTransactionOperatorFail();
//                    TradeTransactionQueueObj.SetTransactionStatusMsg("Full Order Cancellation");
//                    TradeTransactionQueueObj.SettledDate = Helpers.UTC_To_IST();
//                    TransactionQueueObj.MakeTransactionOperatorFail();
//                    TransactionQueueObj.SetTransactionStatusMsg("Full Order Cancellation");
//                }
//                //rita 29-11-18 update errorcode
//                TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));

//                tradeCancelQueue.Status = 1;
//                tradeCancelQueue.SettledDate = Helpers.UTC_To_IST();
//                tradeCancelQueue.UpdatedDate = Helpers.UTC_To_IST();
//                tradeCancelQueue.StatusMsg = "Cancellation Successful.";
//                var WalletResp = await WalletResult;

//                //Rita 24-6-19 if no record found then make cancel only
//                if (WalletResp.ReturnCode != enResponseCode.Success && WalletResp.ErrorCode != enErrorCode.sp_ArbitrageReleaseHoldWalletWithCharge_RecordNotFound)
//                {
//                    //Uday 07-12-2018 Send SMS For Order Cancel failed
//                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, CancelOrderPrice, CancelOrderQty, TransactionQueueObj.MemberMobile, 3);

//                    //Uday 07-12-2018 Send Email For Order Cancel failed
//                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, CancelOrderQty, TradeTransactionQueueObj.TrnDate + "", CancelOrderPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

//                    _Resp.ErrorCode = enErrorCode.CancelOrder_CancelProcessFail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Cancel Process Fail as Wallet Process Fail " + WalletResp.ReturnMsg;
//                    return _Resp;
//                }

//                var ResultBool = _ICancelOrderRepository.UpdateDataObjectWithBeginTransactionV1(tradeCancelQueue, TransactionQueueObj, TradeTransactionQueueObj, BuyerListObj, SellerListObj, SettledTradeTQObj, ISPartialSettled);
//                if (!ResultBool)
//                {
//                    //Uday 07-12-2018 Send SMS For Order Cancel failed
//                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, CancelOrderPrice, CancelOrderQty, TransactionQueueObj.MemberMobile, 3);

//                    //Uday 07-12-2018 Send Email For Order Cancel failed
//                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, CancelOrderQty, TradeTransactionQueueObj.TrnDate + "", CancelOrderPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

//                    _Resp.ErrorCode = enErrorCode.CancelOrder_CancelProcessFail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Cancel Process Fail";
//                    return _Resp;
//                }

//                //Uday 07-12-2018 Send SMS and email When Order Cancel
//                if (ISPartialSettled != 1) // Full Cancel
//                {
//                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, CancelOrderPrice, CancelOrderQty, TransactionQueueObj.MemberMobile, 2);

//                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, CancelOrderQty, TradeTransactionQueueObj.TrnDate + "", CancelOrderPrice, 0, 1, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);
//                }
//                else // Partial Cancel
//                {
//                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, CancelOrderPrice, CancelOrderQty, TransactionQueueObj.MemberMobile, 1);

//                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, TotalQty, TradeTransactionQueueObj.TrnDate + "", CancelOrderPrice, 0, 2, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType, (TotalQty - CancelOrderQty), CancelOrderQty);
//                }

//                try
//                {
//                    var MakeTradeStopLossObj = _TradeStopLoss.GetSingle(e => e.TrnNo == TransactionQueueObj.Id);
//                    //Rita 19-07-19 added await for context error
//                    await _ISignalRService.OnStatusCancelArbitrage(TradeTransactionQueueObj.Status, TransactionQueueObj, TradeTransactionQueueObj, "", MakeTradeStopLossObj.ordertype, ISPartialSettled);
//                }
//                catch (Exception ex)
//                {
//                    HelperForLog.WriteErrorLog("ISignalRService CancellationProcessAPIV1 Error ##TrnNo:" + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                }
//                _Resp.ErrorCode = enErrorCode.CancelOrder_ProccedSuccess;
//                _Resp.ReturnCode = enResponseCodeService.Success;
//                _Resp.ReturnMsg = "Order Cancellled Successfully";
//                return _Resp;
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("CancellationProcessAPIV1:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                //_dbContext.Database.RollbackTransaction();
//            }
//            return _Resp;
//        }

//        #region Send SMS And Email
//        public async Task SMSSendCancelTransaction(long TrnNo, decimal Price, decimal Qty, string MobileNumber, int CancelType)
//        {
//            try
//            {
//                if (!string.IsNullOrEmpty(MobileNumber))
//                {
//                    SendSMSRequest SendSMSRequestObj = new SendSMSRequest();
//                    ApplicationUser User = new ApplicationUser();
//                    TemplateMasterData SmsData = new TemplateMasterData();

//                    CommunicationParamater communicationParamater = new CommunicationParamater();
//                    communicationParamater.Param1 = TrnNo + "";
//                    communicationParamater.Param2 = Price + "";
//                    communicationParamater.Param3 = Qty + "";


//                    Task.Run(() => HelperForLog.WriteLogIntoFile("SMSSendCancelTransaction", ControllerName, " ##TrnNo : " + TrnNo));

//                    if (CancelType == 1) // Partially Cancel
//                    {
//                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_PartialOrderCancel, communicationParamater, enCommunicationServiceType.SMS).Result;
//                    }
//                    else if (CancelType == 2) // Full Cancel
//                    {
//                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_OrderCancel, communicationParamater, enCommunicationServiceType.SMS).Result;
//                    }
//                    else if (CancelType == 3) // Cancel Fail
//                    {
//                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_OrderCancelFailed, communicationParamater, enCommunicationServiceType.SMS).Result;
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
//                Task.Run(() => HelperForLog.WriteErrorLog("SMSSendCancelTransaction:##TrnNo " + TrnNo, ControllerName, ex));
//            }
//        }

//        public async Task EmailSendCancelTransaction(long TrnNo, string UserId, string pair, decimal qty, string datetime, decimal price, decimal fee, int CancelType, short OrderType, short TrnType, decimal SettledQty = 0, decimal CancelQty = 0)
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
//                    Task.Run(() => HelperForLog.WriteLogIntoFile("SendEmailTransaction - EmailSendCancelTransaction", ControllerName, " ##TrnNo : " + TrnNo + " ##Type : " + CancelType));


//                    communicationParamater.Param8 = User.UserName + "";
//                    communicationParamater.Param1 = pair + "";
//                    communicationParamater.Param2 = Helpers.DoRoundForTrading(qty, 8).ToString();
//                    communicationParamater.Param3 = pair.Split("_")[1];
//                    communicationParamater.Param4 = datetime;
//                    communicationParamater.Param5 = Helpers.DoRoundForTrading(price, 8).ToString();
//                    communicationParamater.Param6 = Helpers.DoRoundForTrading(fee, 8).ToString();
//                    communicationParamater.Param7 = Helpers.DoRoundForTrading((fee + (price * qty)), 8).ToString();   //Uday 01-01-2019 In Final Price Calculation Change
//                    communicationParamater.Param11 = ((enTransactionMarketType)OrderType).ToString();  //Uday 01-01-2019 Add OrderType In Email
//                    communicationParamater.Param12 = ((enTrnType)TrnType).ToString();  //Uday 01-01-2019 Add TranType In Email
//                    communicationParamater.Param13 = TrnNo.ToString(); //Uday 01-01-2019 Add TrnNo In Email

//                    if (CancelType == 1) // Cancel Success
//                    {
//                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_OrderCancel, communicationParamater, enCommunicationServiceType.Email).Result;
//                    }
//                    else if (CancelType == 2) // Cancel Partially
//                    {
//                        communicationParamater.Param9 = Helpers.DoRoundForTrading(SettledQty, 8).ToString();
//                        communicationParamater.Param10 = Helpers.DoRoundForTrading(CancelQty, 8).ToString();
//                        communicationParamater.Param7 = Helpers.DoRoundForTrading((fee + (price * SettledQty)), 8).ToString();     //Uday 01-01-2019 In Final Price Calculation Change

//                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_PartialOrderCancel, communicationParamater, enCommunicationServiceType.Email).Result;
//                    }
//                    else if (CancelType == 3) // Cancel Failed
//                    {
//                        communicationParamater.Param7 = Helpers.DoRoundForTrading(0, 8).ToString();
//                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_OrderCancelFailed, communicationParamater, enCommunicationServiceType.Email).Result;
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

//        #region LPCancelTrade

//        public async Task<LPCancelOrdedrResponse> LiquidityTradeCancelProcess(long TrnNo, TradeTransactionQueueArbitrage tradeTransactionObj)
//        {
//            var IsCancelTxnProceed = 0;
//            //WebAPIParseResponseCls WebAPIParseResponseClsObj = new WebAPIParseResponseCls();
//            LPCancelOrdedrResponse _Res = new LPCancelOrdedrResponse();
//            //string APIResponse = "";
//            try
//            {
//                NewtransactionReq = new ArbitrageTransactionRequest();
//                ArbitrageTransactionRequest TransactionResponse = _TransactionRequest.FindBy(e => e.TrnNo == tradeTransactionObj.TrnNo).ToList().FirstOrDefault();
//                if (TransactionResponse == null)
//                {
//                    _Res.ReturnCode = enResponseCodeService.Success;
//                    _Res.ReturnMsg = "LP API call not procceed and ResponseData is null and TrnRefNo is null,no request found";
//                    HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #CancelOrder", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : cancel successfully. LP API call not procceed and ResponseData is null and TrnRefNo is null");
//                    return _Res;
//                }
//                else if (string.IsNullOrEmpty(TransactionResponse.ResponseData) && string.IsNullOrEmpty(TransactionResponse.TrnID))
//                {
//                    if(TransactionResponse.CreatedDate > Helpers.UTC_To_IST().AddSeconds(-20))//not 30 sec old then does not proceed, as Txn wait till 30 sec
//                    {
//                        _Res.ReturnCode = enResponseCodeService.Fail;
//                        _Res.ReturnMsg = "LP API call not procceed and ResponseData is null and TrnRefNo is null,and not old then 30 sec";
//                        HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #CancelOrder", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : cancel successfully. LP API call not procceed and ResponseData is null and TrnRefNo is null");
//                        return _Res;
//                    }
//                    _Res.ReturnCode = enResponseCodeService.Success;
//                    _Res.ReturnMsg = "LP API call not procceed and ResponseData is null and TrnRefNo is null";
//                    HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #CancelOrder", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : cancel successfully. LP API call not procceed and ResponseData is null and TrnRefNo is null");
//                    return _Res;
//                }                
//                var TrnData = _TransactionRepository.GetSingle(e => e.Id == TrnNo);
//                if (TrnData == null)
//                {
//                    _Res.ReturnMsg = "CancelOrder_EnterValidTransactionNo";
//                    _Res.ReturnCode = enResponseCodeService.Fail;
//                    _Res.ErrorCode = enErrorCode.CancelOrder_EnterValidTransactionNo;
//                    return _Res;
//                }
//                //var LPProvidetail = _ServiceProviderDetail.GetSingle(e => e.ServiceProID == TrnData.SerProID && e.Status==1);
//                var LPProvidetail = _ServiceProviderDetail.GetSingle(e => e.Id == TrnData.SerProDetailID);//Rita 29-7-19 change direct query on Id
//                if (LPProvidetail == null)
//                {
//                    _Res.ReturnMsg = "Provider Detail not Found";
//                    _Res.ReturnCode = enResponseCodeService.Fail;
//                    _Res.ErrorCode = enErrorCode.CancelOrder_ProviderDetailIDNotFound;
//                    return _Res;
//                }
//                //var LPOredrID = _TranRequestRepository.GetSingle(e => e.TrnNo == TrnNo).TrnID;
//                //Rita 20-8-19 if request not found then return true for direct cancellation ,without API call
//                ArbitrageTransactionRequest Request = _TranRequestRepository.FindBy(e => e.TrnNo == TrnNo).FirstOrDefault();
//                if (Request == null)
//                {
//                    _Res.ReturnMsg = "API Request not found";
//                    _Res.ReturnCode = enResponseCodeService.Success;
//                    _Res.ErrorCode = enErrorCode.Success;
//                    return _Res;
//                }
//                var LPOredrID = Request.TrnID;
//                if (LPOredrID == null)
//                {
//                    _Res.ReturnMsg = "CancelOrder_TrnRefNo_Not_Found";
//                    _Res.ReturnCode = enResponseCodeService.Fail;
//                    _Res.ErrorCode = enErrorCode.CancelOrder_TrnRefNo_Not_Found;
//                    return _Res;
//                }
//                else if (LPOredrID == "")
//                {
//                    _Res.ReturnMsg = "CancelOrder_TrnRefNo_Not_Found";
//                    _Res.ReturnCode = enResponseCodeService.Fail;
//                    _Res.ErrorCode = enErrorCode.CancelOrder_TrnRefNo_Not_Found;
//                    return _Res;
//                }
//                var ServiceProConfiguration = _IGetWebRequest.GetServiceProviderConfigurationArbitrage(LPProvidetail.Id);
//                if (ServiceProConfiguration == null)
//                {
//                    _Res.ReturnMsg = "ThirdPArty Not Found";
//                    _Res.ReturnCode = enResponseCodeService.Fail;
//                    _Res.ErrorCode = enErrorCode.CancelOrder_ThirdPartyDataNotFound;
//                    return _Res;
//                }
//                if(LPProvidetail.ProTypeID==7)//Rita 24-7-19 for ccxt call
//                {
//                    _Res = await CCXTOrderCancellation(LPOredrID, tradeTransactionObj, TrnData.LPType);
//                    if (_Res.ReturnCode != enResponseCodeService.Success)
//                        return _Res;
//                    else
//                        goto SuccessTrade;
//                }


//                switch (LPProvidetail.AppTypeID)
//                {
//                    case (long)enAppType.Binance:
//                        //var BinanceExchangeInfoResp = _binanceLPService.GetExchangeInfoAsync();

//                        //BinanceExchangeInfo BinanceExchangeInfoResult = _cache.Get<BinanceExchangeInfo>("BinanceExchangeInfoResp");
//                        //if (BinanceExchangeInfoResult == null)
//                        //{
//                        //    BinanceExchangeInfoResult = _binanceLPService.GetExchangeInfoAsync().Result.Data;
//                        //    _cache.Set<BinanceExchangeInfo>("BinanceExchangeInfoResp", BinanceExchangeInfoResult);
//                        //}
//                        //else if (BinanceExchangeInfoResult.Symbols.Length == 0)
//                        //{
//                        //    BinanceExchangeInfoResult = _binanceLPService.GetExchangeInfoAsync().Result.Data;
//                        //    _cache.Set<BinanceExchangeInfo>("BinanceExchangeInfoResp", BinanceExchangeInfoResult);
//                        //}
//                        //if (BinanceExchangeInfoResult == null)
//                        //{
//                        //    HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #CancelOrder", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : Not found binance exchange info");

//                        //    _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                        //    _Res.ReturnCode = enResponseCodeService.Fail;
//                        //    _Res.TrnNo = TrnNo;
//                        //    _Res.ReturnMsg = "Not found binance exchange info";
//                        //    return _Res;
//                        //}
                            
//                         _binanceLPService._client.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);
//                        //BinanceExchangeInfo BinanceExchangeInfoResult = BinanceExchangeInfoResp.Result.Data;
//                        //foreach (var symbol in BinanceExchangeInfoResult.Symbols)
//                        //{
//                            var pair = tradeTransactionObj.TrnType == 5 ? tradeTransactionObj.Order_Currency + tradeTransactionObj.Delivery_Currency : tradeTransactionObj.Delivery_Currency + tradeTransactionObj.Order_Currency;
//                            //if (symbol.Name == pair)
//                            //{
//                                CallResult<BinanceCanceledOrder> BinanceResult = await _binanceLPService.CancelOrderAsync(pair, Convert.ToInt64(LPOredrID),receiveWindow : 7000);

//                                HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #CancelOrder", ControllerName," ##TrnNo:" + TrnNo + " ##Response : " + JsonConvert.SerializeObject(BinanceResult));

//                                if (BinanceResult != null)
//                                {
//                                    APIResponse = JsonConvert.SerializeObject(BinanceResult);

//                                    if (BinanceResult.Success)
//                                    {
//                                        IsCancelTxnProceed = 1;
//                                        WebAPIParseResponseClsObj.TrnRefNo = BinanceResult.Data.OrderId.ToString();
//                                        WebAPIParseResponseClsObj.OperatorRefNo = BinanceResult.Data.ClientOrderId;
//                                        WebAPIParseResponseClsObj.Status = BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Canceled ? enTransactionStatus.OperatorFail :
//                                            (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PendingCancel ? enTransactionStatus.Pending : enTransactionStatus.OperatorFail);
//                                        _Res.TrnNo = TrnNo;
//                                        _Res.TrnRefNo = BinanceResult.Data.OriginalClientOrderId;
//                                        _Res.price = BinanceResult.Data.Price;
//                                        _Res.origQty = BinanceResult.Data.OriginalQuantity;
//                                        _Res.executedQty = BinanceResult.Data.ExecutedQuantity;
//                                        _Res.cummulativeQuoteQty = BinanceResult.Data.CummulativeQuoteQuantity;
//                                        _Res.status = BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.Canceled ? enTransactionStatus.Success :
//                                            (BinanceResult.Data.Status == Binance.Net.Objects.OrderStatus.PendingCancel ? enTransactionStatus.Pending : enTransactionStatus.OperatorFail);
//                                        _Res.ReturnCode = enResponseCodeService.Success;
//                                        goto SuccessTrade;
//                                    }
//                                    else
//                                    {
//                                        CallResult<BinanceOrder> BinanceResult2 = await _binanceLPService.GetOrderInfoAsync(pair, Convert.ToInt64(LPOredrID),receiveWindow : 7000);
//                                        HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #GetOrderInfo", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : " + JsonConvert.SerializeObject(BinanceResult2));
//                                        if (BinanceResult2 != null)
//                                        {
//                                            if (BinanceResult2.Success)
//                                            {
//                                                APIResponse = JsonConvert.SerializeObject(BinanceResult2);

//                                                _Res.TrnRefNo = BinanceResult2.Data.OrderId.ToString();
//                                                _Res.TrnNo = TrnNo;
//                                                _Res.price = BinanceResult2.Data.Price;
//                                                _Res.origQty = BinanceResult2.Data.OriginalQuantity;
//                                                _Res.executedQty = BinanceResult2.Data.ExecutedQuantity;
//                                                _Res.cummulativeQuoteQty = BinanceResult2.Data.CummulativeQuoteQuantity;
//                                                _Res.status = BinanceResult2.Data.Status == Binance.Net.Objects.OrderStatus.Canceled ? enTransactionStatus.Success :
//                                                    (BinanceResult2.Data.Status == Binance.Net.Objects.OrderStatus.PendingCancel ? enTransactionStatus.Pending : enTransactionStatus.OperatorFail);
//                                                if (BinanceResult2.Data.Status == Binance.Net.Objects.OrderStatus.Canceled || BinanceResult2.Data.Status == Binance.Net.Objects.OrderStatus.PendingCancel)
//                                                {
//                                                    _Res.ReturnCode = enResponseCodeService.Success;
//                                                    goto SuccessTrade;
//                                                }
//                                                else
//                                                {
//                                                    _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                                                    _Res.ReturnCode = enResponseCodeService.Fail;
//                                                    _Res.TrnNo = TrnNo;
//                                                    _Res.ReturnMsg = "Not found";
//                                                    return _Res;
//                                                }
//                                            }
//                                            else
//                                            {
//                                                _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                                                _Res.ReturnCode = enResponseCodeService.Fail;
//                                                _Res.TrnNo = TrnNo;
//                                                _Res.ReturnMsg = "Not found";
//                                                return _Res;
//                                            }
//                                        }
//                                        else
//                                        {
//                                            _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                                            _Res.ReturnCode = enResponseCodeService.Fail;
//                                            _Res.TrnNo = TrnNo;
//                                            _Res.ReturnMsg = "Not found";
//                                            return _Res;
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                                    _Res.ReturnCode = enResponseCodeService.Fail;
//                                    _Res.TrnNo = TrnNo;
//                                    _Res.ReturnMsg = "Not found";
//                                    return _Res;
//                                }
//                           // }
//                        //}
//                        break;
//                    case (long)enAppType.Bittrex:
//                        //BittrexClient.SetDefaultOptions(new BittrexClientOptions()
//                        //{
//                        //    ApiCredentials = new ApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey)
//                        //});
//                        _bitrexLPService._client.SetApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey);
//                        WebCallResult<object> BittrexResult = await _bitrexLPService.CancelOrderAsync(Guid.Parse(LPOredrID));
//                        //BittrexCancelOrderResponse BittrexResult = null;

//                        HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #CancelOrder", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : " + JsonConvert.SerializeObject(BittrexResult));

//                        if (BittrexResult != null)
//                        {
//                            APIResponse = JsonConvert.SerializeObject(BittrexResult);

//                            if (BittrexResult.Success)
//                            {                                
//                                if (BittrexResult.Data == null)
//                                {
//                                    _Res.TrnRefNo = LPOredrID;
//                                }
//                                else
//                                {
//                                    IsCancelTxnProceed = 1;
//                                    _Res.TrnRefNo = ((CancelOrderResult)BittrexResult.Data).uuid == null ? LPOredrID : ((CancelOrderResult)BittrexResult.Data).uuid;
//                                }
//                                _Res.TrnNo = TrnNo;
//                                //_Res.TrnRefNo = JsonConvert.DeserializeObject<WebCallResult<object>>(JsonConvert.SerializeObject(BittrexResult).ToString()).Data.ToString();
//                                _Res.ReturnCode = enResponseCodeService.Success;
//                                goto SuccessTrade;
//                            }
//                            else
//                            {
//                                CallResult<BittrexAccountOrder> BittrexResult2 = await _bitrexLPService.GetOrderInfoAsync(Guid.Parse(TrnData.TrnRefNo));
//                                HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #GetOrderInfo", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : " + JsonConvert.SerializeObject(BittrexResult2));

//                                if (BittrexResult2 != null)
//                                {
//                                    APIResponse = JsonConvert.SerializeObject(BittrexResult2);
//                                    if (BittrexResult2.Success)
//                                    {
//                                        IsCancelTxnProceed = 1;
//                                        _Res.TrnNo = TrnNo;
//                                        _Res.TrnRefNo = BittrexResult2.Data.OrderUuid.ToString();
//                                        if (BittrexResult2.Data.CancelInitiated)
//                                        {
//                                            _Res.price = BittrexResult2.Data.Price;
//                                            _Res.origQty = BittrexResult2.Data.Quantity;
//                                            _Res.executedQty = BittrexResult2.Data.Quantity - BittrexResult2.Data.QuantityRemaining;
//                                            _Res.cummulativeQuoteQty = BittrexResult2.Data.Quantity - BittrexResult2.Data.QuantityRemaining;
//                                            _Res.ReturnCode = enResponseCodeService.Success;
//                                            goto SuccessTrade;
//                                        }
//                                        else
//                                        {
//                                            _Res.ReturnMsg = BittrexResult2.Error.Message.ToString();
//                                            _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                                            _Res.ReturnCode = enResponseCodeService.Fail;
//                                            _Res.TrnNo = TrnNo;
//                                            _Res.ReturnMsg = "Not found";
//                                            return _Res;
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                                    _Res.ReturnCode = enResponseCodeService.Fail;
//                                    _Res.TrnNo = TrnNo;
//                                    _Res.ReturnMsg = "Not found";
//                                    return _Res;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                            _Res.ReturnCode = enResponseCodeService.Fail;
//                            _Res.TrnNo = TrnNo;
//                            _Res.ReturnMsg = "Not found";
//                            return _Res;
//                        }
//                        break;
//                    case (long)enAppType.Huobi:
//                        HuobiClient.SetDefaultOptions(new HuobiClientOptions()
//                        {
//                            ApiCredentials = new ApiCredentials(ServiceProConfiguration.APIKey, ServiceProConfiguration.SecretKey)
//                        });
//                        WebCallResult<long> HuobiResult = await _huobiLPService.CancelOrderAsync(Convert.ToInt64(LPOredrID));
//                        // CallResult<Object> BittrexResult = await _bitrexLPService.CancelOrderAsync(new Guid(LPOredrID));
//                        //APIResponse = JsonConvert.SerializeObject(BittrexResult);

//                        HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #CancelOrder", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : " + JsonConvert.SerializeObject(HuobiResult));
//                        if(HuobiResult != null)
//                        {
//                            if (HuobiResult.Success)
//                            {
//                                _Res.TrnNo = TrnNo;
//                                _Res.TrnRefNo = HuobiResult.Data.ToString();//JsonConvert.DeserializeObject<BittrexCancelOrderResponse>(JsonConvert.SerializeObject(BittrexResult).ToString()).result.uuid.ToString();
//                                _Res.ReturnCode = enResponseCodeService.Success;
//                                goto SuccessTrade;
//                            }
//                            else
//                            {
//                                _Res.TrnNo = TrnNo;
//                                _Res.ReturnMsg = HuobiResult.Error.Message;
//                                _Res.ReturnCode = enResponseCodeService.Fail;
//                                _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                                return _Res;
//                            }
//                        }
//                        else
//                        {
//                            _Res.TrnNo = TrnNo;
//                            _Res.ReturnMsg = "API Response Fail from Huobi";
//                            _Res.ReturnCode = enResponseCodeService.Fail;
//                            _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                            return _Res;
//                        }
//                    case (long)enAppType.Coinbase:
//                        CoinBaseGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
//                        CoinBaseGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
//                        var CoinbaseResult = await _coinBaseService.CancelOrderById(LPOredrID);
//                        APIResponse = JsonConvert.SerializeObject(CoinbaseResult);

//                        HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #CancelOrder", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : " + JsonConvert.SerializeObject(CoinbaseResult));

//                        if (CoinbaseResult.ErrorCode == enErrorCode.Success)
//                        {
//                            foreach (var id in CoinbaseResult.Result.OrderIds)
//                            {
//                                _Res.TrnRefNo = id.ToString();
//                                break;
//                            }
//                            _Res.ErrorCode = enErrorCode.Success;
//                            _Res.TrnNo = TrnNo;
//                            goto SuccessTrade;
//                        }
//                        else
//                        {
//                            var CoinbaseResult2 = await _coinBaseService.GetOrderById(LPOredrID);
//                            APIResponse = JsonConvert.SerializeObject(CoinbaseResult2);
//                            HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #OrderInfo", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : " + JsonConvert.SerializeObject(CoinbaseResult2));

//                            if (CoinbaseResult2 != null)
//                            {
//                                _Res.TrnRefNo = CoinbaseResult2.Id.ToString();
//                            }
//                            //_Res.ReturnMsg = CoinbaseResult2.;
//                            //_Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                            _Res.ReturnCode = enResponseCodeService.Success;
//                            _Res.TrnNo = TrnNo;
//                            goto SuccessTrade;
//                        }
//                    //break;
//                    case (long)enAppType.Poloniex:
//                        PoloniexGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
//                        PoloniexGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
//                        PoloniexCancelOrderObj PoloniexResult = new PoloniexCancelOrderObj();
//                        PoloniexCancelOrderErrorObj orderErrorObj = new PoloniexCancelOrderErrorObj();
//                        var PoloniexRes = await _poloniexService.CancelPoloniexOrder(LPOredrID);
//                        APIResponse = JsonConvert.SerializeObject(PoloniexRes);
//                        PoloniexResult = JsonConvert.DeserializeObject<PoloniexCancelOrderObj>(JsonConvert.SerializeObject(PoloniexRes));

//                        HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #CancelOrder", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : " + JsonConvert.SerializeObject(PoloniexResult));

//                        orderErrorObj = JsonConvert.DeserializeObject<PoloniexCancelOrderErrorObj>(JsonConvert.SerializeObject(PoloniexRes));
//                        if (orderErrorObj != null)
//                        {
//                            _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                            _Res.ReturnCode = enResponseCodeService.Fail;
//                            _Res.ReturnMsg = orderErrorObj.error.ToString();
//                            _Res.TrnNo = TrnNo;
//                            return _Res;
//                        }
//                        else
//                        {
//                            if (PoloniexResult != null)
//                            {
//                                if (PoloniexResult.success == 1)
//                                {
//                                    _Res.ErrorCode = enErrorCode.Success;
//                                    _Res.executedQty = Convert.ToDecimal(PoloniexResult.amount);
//                                    _Res.TrnRefNo = PoloniexResult.message;
//                                    _Res.TrnNo = TrnNo;
//                                    goto SuccessTrade;
//                                }
//                                else
//                                {
//                                    object PoloniexRes2 = await _poloniexService.GetPoloniexOrderState(LPOredrID);
//                                    JObject Data = JObject.Parse(PoloniexRes2.ToString());
//                                    var Success = Convert.ToUInt16(Data["result"]["success"]);
//                                    APIResponse = JsonConvert.SerializeObject(PoloniexRes2);
//                                    if (Success == 1)
//                                    {
//                                        JToken Result = Data["result"][TrnData.TrnRefNo];
//                                        PoloniexOrderState PoloniexResult2 = JsonConvert.DeserializeObject<PoloniexOrderState>(Result.ToString());
//                                        _Res.ErrorCode = enErrorCode.Success;
//                                        _Res.executedQty = Convert.ToDecimal(PoloniexResult2.amount);
//                                        _Res.TrnRefNo = TrnData.TrnRefNo;
//                                        _Res.TrnNo = TrnNo;
//                                        goto SuccessTrade;
//                                    }
//                                }
//                            }
//                        }
//                        break;
//                    case (long)enAppType.TradeSatoshi:
//                        GlobalSettings.API_Key = ServiceProConfiguration.APIKey;
//                        GlobalSettings.Secret = ServiceProConfiguration.SecretKey;
//                        var TradeSatoshiResult = await _tradeSatoshiLPService.CancelOrderAsync(CancelOrderType.Single, Convert.ToInt64(LPOredrID));

//                        HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #CancelOrder", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : " + JsonConvert.SerializeObject(TradeSatoshiResult));

//                        if (TradeSatoshiResult != null)
//                        {
//                            APIResponse = JsonConvert.SerializeObject(TradeSatoshiResult);
//                            if (TradeSatoshiResult.success)
//                            {
//                                IsCancelTxnProceed = 1;
//                                _Res.TrnRefNo = TradeSatoshiResult.result.CanceledOrders[0].ToString();
//                                _Res.ErrorCode = enErrorCode.Success;
//                                _Res.TrnNo = TrnNo;
//                                goto SuccessTrade;
//                            }
//                            else
//                            {
//                                //var TradeSatoshiResult2 = await _tradeSatoshiLPService.GetOrderInfoAsync(Convert.ToInt64(LPOredrID));

//                                //HelperForLog.WriteLogIntoFile("LiquidityTradeCancelProcess LP Response : #OrderInfo", ControllerName, " ##TrnNo:" + TrnNo + " ##Response : " + JsonConvert.SerializeObject(TradeSatoshiResult2));

//                                //if (TradeSatoshiResult2 != null)
//                                //{
//                                //    APIResponse = JsonConvert.SerializeObject(TradeSatoshiResult);
//                                //    if (TradeSatoshiResult2.success)
//                                //    {                                        
//                                //        if (TradeSatoshiResult2.result?.Status.ToLower() == "cancel")
//                                //        {
//                                //            IsCancelTxnProceed = 1;
//                                //            _Res.TrnRefNo = TradeSatoshiResult2.result.Id.ToString();
//                                //            _Res.price = TradeSatoshiResult2.result.Rate;
//                                //            _Res.origQty = TradeSatoshiResult2.result.Amount;
//                                //            _Res.executedQty = TradeSatoshiResult2.result.Amount - TradeSatoshiResult2.result.Remaining;
//                                //            _Res.cummulativeQuoteQty = TradeSatoshiResult2.result.Amount - TradeSatoshiResult2.result.Remaining;
//                                //            _Res.ReturnCode = enResponseCodeService.Success;
//                                //            goto SuccessTrade;
//                                //        }
//                                //        else
//                                //        {
//                                //            _Res.ReturnMsg = TradeSatoshiResult2.message;
//                                //            _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                                //            _Res.ReturnCode = enResponseCodeService.Fail;
//                                //            _Res.TrnNo = TrnNo;
//                                //            _Res.ReturnMsg = "Not found";
//                                //            return _Res;
//                                //        }
//                                //    }
//                                //}
//                                //else
//                                //{
//                                    _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                                    _Res.ReturnCode = enResponseCodeService.Fail;
//                                    _Res.TrnNo = TrnNo;
//                                    _Res.ReturnMsg = "Not found";
//                                    return _Res;
//                                //}
//                            }
//                        }
//                        else
//                        {
//                            _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                            _Res.ReturnCode = enResponseCodeService.Fail;
//                            _Res.TrnNo = TrnNo;
//                            _Res.ReturnMsg = "Not found";
//                            return _Res;
//                        }
//                        break;
//                    case (long)enAppType.UpBit:
//                        UpBitGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
//                        UpBitGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
//                        var UpbitResult = await _upbitService.CancelOrderAsync(LPOredrID);
//                        APIResponse = JsonConvert.SerializeObject(UpbitResult);
//                        if (UpbitResult != null)
//                        {
//                            if (UpbitResult.state.ToLower() == "cancel")
//                            {
//                                _Res.TrnRefNo = UpbitResult.uuid.ToString();
//                                _Res.ErrorCode = enErrorCode.Success;
//                                _Res.TrnNo = TrnNo;
//                                goto SuccessTrade;
//                            }
//                            else
//                            {
//                                _Res.ReturnMsg = UpbitResult.state;
//                                _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                                _Res.ReturnCode = enResponseCodeService.Fail;
//                                _Res.TrnNo = TrnNo;
//                                return _Res;
//                            }
//                        }
//                        else
//                        {
//                            _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                            _Res.ReturnCode = enResponseCodeService.Fail;
//                            _Res.TrnNo = TrnNo;
//                            _Res.ReturnMsg = "Not found";
//                            return _Res;
//                        }
//                    case (long)enAppType.OKEx:
//                        OKEXGlobalSettings.API_Key = ServiceProConfiguration.APIKey;
//                        OKEXGlobalSettings.Secret = ServiceProConfiguration.SecretKey;
//                        OKEXGlobalSettings.PassPhrase = ServiceProConfiguration.Param1;
//                        var OKExExchangeInfoResp = await _oKExLPService.GetExchangeRateAsync();
//                        var OKEXResult = await _oKExLPService.CancelOrderAsync(OKExExchangeInfoResp.Data[0].instrument_id, LPOredrID,TrnNo.ToString());
//                        APIResponse = JsonConvert.SerializeObject(OKEXResult);
//                        if (OKEXResult.result == true)
//                        {
//                            _Res.TrnRefNo = OKEXResult.order_id;
//                            _Res.ErrorCode = enErrorCode.Success;
//                            _Res.TrnNo = TrnNo;
//                            goto SuccessTrade;
//                        }
//                        else
//                        {
//                            _Res.ReturnMsg = "";
//                            _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                            _Res.ReturnCode = enResponseCodeService.Fail;
//                            _Res.TrnNo = TrnNo;
//                            return _Res;
//                        }
//                }
//                SuccessTrade:
//                NewtransactionReq.Status = Convert.ToInt16(WebAPIParseResponseClsObj.Status);
//                NewtransactionReq.IsCancel = 1;
//                NewtransactionReq.OprTrnID = NewtransactionReq.TrnID = WebAPIParseResponseClsObj.TrnRefNo;
//                NewtransactionReq.TrnNo = TrnNo;
//                NewtransactionReq.ResponseData = APIResponse;
//                NewtransactionReq.SerProDetailID = LPProvidetail.Id;
//                NewtransactionReq.SerProID = LPProvidetail.ServiceProID;
//                NewtransactionReq.ServiceID = TrnData.ServiceID;
//                NewtransactionReq.SetResponseTime(Helpers.UTC_To_IST());
//                NewtransactionReq.RequestData = LPOredrID;
//                Task.Run(() => _TransactionRequest.Add(NewtransactionReq));
//                return _Res;
//            }
//            catch (Exception ex)
//            {
//                if(IsCancelTxnProceed == 1)
//                {
//                    _Res.ReturnCode = enResponseCodeService.Success;
//                }
//                else
//                {
//                    _Res.ErrorCode = enErrorCode.API_Respose_Fail;
//                    _Res.ReturnCode = enResponseCodeService.Fail;
//                    _Res.ReturnMsg = ex.Message.ToString();
//                }                
//                HelperForLog.WriteErrorLog("CancelAPITrade:", ControllerName, ex);
//                //throw ex;
//                return null;
//            }
//        }

//        public async Task<LPCancelOrdedrResponse> CCXTOrderCancellation(string LPOredrID, TradeTransactionQueueArbitrage tradeTransactionObj,short LPType)
//        {
//            short IsCancelTxnProceed = 0;
//            //WebAPIParseResponseCls WebAPIParseResponseClsObj = new WebAPIParseResponseCls();
//            LPCancelOrdedrResponse _Resp = new LPCancelOrdedrResponse();
//            //string APIResponse = "";
//            ThirdPartyAPIRequestArbitrage ThirdPartyAPIRequestOnj;
//            short Retry = 0;
//            try
//            {
//                _Resp.ReturnCode = enResponseCodeService.Fail;//default value
//                var list = _cache.Get<List<TransactionProviderArbitrageResponse>>("RouteDataArbitrage");
//                if (list == null)
//                {
//                    var GetProListResult = _WebApiRepository.GetProviderDataListArbitrageAsync(new TransactionApiConfigurationRequest { PairID = 0, trnType = 4, LPType = 0 });
//                    list = _cache.Get<List<TransactionProviderArbitrageResponse>>("RouteDataArbitrage");
//                }
//                if (list == null)
//                {
//                    _Resp.ErrorCode = enErrorCode.API_AuthenticationFail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Tokane Generation Fail";
//                    _Resp.TrnNo = tradeTransactionObj.TrnNo;
//                    return _Resp;
//                }

//                TransactionProviderArbitrageResponse list1 = list.Where(e => e.TrnType == 4 && e.TrnTypeID == 4 && e.ProTypeID == 7 && e.PairId == tradeTransactionObj.PairID && e.LPType == LPType).FirstOrDefault();

//                RetryWithNewToken:
//                string Token = await _NewTransactionArbitrageV1.ConnectToExchangeAsync(list1, null, tradeTransactionObj.TrnNo);
//                if (!string.IsNullOrEmpty(Token))
//                {
//                    ThirdPartyAPIRequestOnj = _IGetWebRequest.ArbitrageMakeWebRequest(list1.RouteID, list1.ThirdPartyAPIID, list1.SerProDetailID, null, null, Token: Token,IsValidateUrl:5,TrnNo:LPOredrID);
                                        
//                    APIResponse = await _IWebApiSendRequest.SendRequestAsyncLPArbitrage(ThirdPartyAPIRequestOnj.RequestURL, ref IsCancelTxnProceed, ThirdPartyAPIRequestOnj.RequestBody, ThirdPartyAPIRequestOnj.MethodType, ThirdPartyAPIRequestOnj.ContentType, ThirdPartyAPIRequestOnj.keyValuePairsHeader, 30000, tradeTransactionObj.TrnNo);

//                    if (APIResponse == "" && IsCancelTxnProceed == 0)
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_APICallfail;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = "API Call Fail";
//                        return _Resp;
//                    }
//                    else if (APIResponse == "" && IsCancelTxnProceed == 1)//API call done
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_APICallProceeWithNoResponse;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = "API Call Procee With NoResponse";
//                        return _Resp;
//                    }

//                    WebAPIParseResponseClsObj = _WebApiParseResponseObj.ArbitrageTransactionParseResponse(APIResponse, list1.ThirdPartyAPIID, 0);

//                    if (WebAPIParseResponseClsObj.ResponseMsg.ToLower().Contains("invalid token") && Retry == 0)
//                    {
//                        _cache.Set<string>("LPType" + list1.LPType, null);
//                        Retry = 1;

//                        goto RetryWithNewToken;
//                    }
//                    else if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Hold && WebAPIParseResponseClsObj.StatusMsg.ToLower().Contains("true"))
//                    {
//                        _Resp.ErrorCode = enErrorCode.Success;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = WebAPIParseResponseClsObj.ResponseMsg;
//                        return _Resp;
//                    }
//                    else if (WebAPIParseResponseClsObj.Status == enTransactionStatus.OperatorFail &&
//                       (WebAPIParseResponseClsObj.StatusMsg.ToLower().Contains("canceled") || WebAPIParseResponseClsObj.StatusMsg.ToLower().Contains("canceling") ||
//                       WebAPIParseResponseClsObj.StatusMsg.ToLower().Contains("count")
//                       ))//here 1,canceled,canceling considered in Fail ,|| WebAPIParseResponseClsObj.ResponseMsg.ToLower().Contains("unknown order sent") , remove as full settled order may given the same
//                    {
//                        _Resp.ErrorCode = enErrorCode.Success;
//                        _Resp.ReturnCode = enResponseCodeService.Success;
//                        _Resp.ReturnMsg = WebAPIParseResponseClsObj.ResponseMsg;
//                        return _Resp;
//                    }
//                    else if (WebAPIParseResponseClsObj.Status == enTransactionStatus.OperatorFail && WebAPIParseResponseClsObj.ResponseMsg.ToLower().Contains("unknown order sent"))//here 1,canceled,canceling considered in Fail
//                    {
//                        //check perticuler Order's status
//                        ThirdPartyAPIRequestOnj = _IGetWebRequest.ArbitrageMakeWebRequest(list1.RouteID, list1.ThirdPartyAPIID, list1.SerProDetailID, null, null, Token: Token, IsValidateUrl: 6, TrnNo: LPOredrID);

//                        APIResponse = await _IWebApiSendRequest.SendRequestAsyncLPArbitrage(ThirdPartyAPIRequestOnj.RequestURL, ref IsCancelTxnProceed, ThirdPartyAPIRequestOnj.RequestBody, ThirdPartyAPIRequestOnj.MethodType, ThirdPartyAPIRequestOnj.ContentType, ThirdPartyAPIRequestOnj.keyValuePairsHeader, 30000, tradeTransactionObj.TrnNo);

//                        if (APIResponse == "" && IsCancelTxnProceed == 0)
//                        {
//                            _Resp.ErrorCode = enErrorCode.API_APICallfail;
//                            _Resp.ReturnCode = enResponseCodeService.Fail;
//                            _Resp.ReturnMsg = "API Call Fail";
//                            return _Resp;
//                        }
//                        else if (APIResponse == "" && IsCancelTxnProceed == 1)//API call done
//                        {
//                            _Resp.ErrorCode = enErrorCode.API_APICallProceeWithNoResponse;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "API Call Procee With NoResponse";
//                            return _Resp;
//                        }

//                        WebAPIParseResponseClsObj = _WebApiParseResponseObj.ArbitrageTransactionParseResponse(APIResponse, list1.ThirdPartyAPIID, 0);

//                        if (WebAPIParseResponseClsObj.Status == enTransactionStatus.OperatorFail &&
//                       (WebAPIParseResponseClsObj.StatusMsg.ToLower().Contains("canceled") || WebAPIParseResponseClsObj.StatusMsg.ToLower().Contains("canceling")
//                       ))//here 1,canceled,canceling considered in Fail ,|| WebAPIParseResponseClsObj.ResponseMsg.ToLower().Contains("unknown order sent") , remove as full settled order may given the same
//                        {
//                            _Resp.ErrorCode = enErrorCode.Success;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = WebAPIParseResponseClsObj.ResponseMsg;
//                            return _Resp;
//                        }
//                        else
//                        {
//                            _Resp.ErrorCode = enErrorCode.API_ResponseFail;
//                            _Resp.ReturnCode = enResponseCodeService.Fail;
//                            _Resp.ReturnMsg = WebAPIParseResponseClsObj.ResponseMsg;
//                            return _Resp;
//                        }
//                    }
//                    else
//                    {
//                        _Resp.ErrorCode = enErrorCode.API_ResponseFail;
//                        _Resp.ReturnCode = enResponseCodeService.Fail;
//                        _Resp.ReturnMsg = WebAPIParseResponseClsObj.ResponseMsg;
//                        return _Resp;
//                    }

//                }
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("CCXTOrderCancellation:##TrnNo " + tradeTransactionObj.TrnNo, ControllerName, ex);
//                if (IsCancelTxnProceed == 1)
//                {
//                    _Resp.ReturnCode = enResponseCodeService.Success;
//                }
//                else
//                {
//                    _Resp.ErrorCode = enErrorCode.API_Respose_Fail;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = ex.Message.ToString();
//                }
//            }
//            return _Resp;
//        }
//        #endregion
//    }
//}
