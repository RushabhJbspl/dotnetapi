using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Data;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Transaction
{
    public class CancelOrderProcess : ICancelOrderProcess
    {
        private readonly ICommonRepository<TradeBuyRequest> _TradeBuyRequest;
        private readonly EFCommonRepository<TransactionQueue> _TransactionRepository;
        private readonly EFCommonRepository<TradeTransactionQueue> _TradeTransactionRepository;
        private readonly ISettlementRepository<BizResponse> _SettlementRepository;
        private readonly ISettlementRepositoryV1<BizResponse> _SettlementRepositoryV1;
        private readonly ICancelOrderRepository _ICancelOrderRepository;
        private readonly ICommonRepository<TradeCancelQueue> _TradeCancelQueueRepository;
        private readonly ICommonRepository<TradeBuyerListV1> _TradeBuyerList;
        private readonly ICommonRepository<TradeSellerListV1> _TradeSellerList;
        private readonly ISignalRService _ISignalRService;
        private readonly ICommonRepository<TradeStopLoss> _TradeStopLoss;
        TradeCancelQueue tradeCancelQueue;
        private readonly IWalletService _WalletService;//Rita 05-12-18 for release remain wallet amount
        private readonly IMessageService _messageService;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly UserManager<ApplicationUser> _userManager;

        PoolOrder PoolOrderObj;
        string ControllerName = "CancelOrderProcess";
        decimal CancelOrderPrice = 0, CancelOrderQty = 0, TotalQty = 0;

        public CancelOrderProcess(ICommonRepository<TradeBuyRequest> TradeBuyRequest, EFCommonRepository<TradeTransactionQueue> TradeTransactionRepository, 
            ISettlementRepository<BizResponse> SettlementRepository, ICommonRepository<TradeCancelQueue> TradeCancelQueueRepository, 
            ICancelOrderRepository ICancelOrderRepository, ISettlementRepositoryV1<BizResponse> SettlementRepositoryV1,
            EFCommonRepository<TransactionQueue> TransactionRepository, ICommonRepository<TradeBuyerListV1> TradeBuyerList, 
            ICommonRepository<TradeSellerListV1> TradeSellerList, ISignalRService ISignalRService, IWalletService WalletService,
            ICommonRepository<TradeStopLoss> TradeStopLoss, IMessageService messageService, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, UserManager<ApplicationUser> userManager)
        {
            _TradeBuyRequest = TradeBuyRequest;
            _TradeTransactionRepository = TradeTransactionRepository;
            _SettlementRepository = SettlementRepository;
            _SettlementRepositoryV1 = SettlementRepositoryV1;
            _TradeCancelQueueRepository = TradeCancelQueueRepository;
            _ICancelOrderRepository = ICancelOrderRepository;
            _TransactionRepository = TransactionRepository;
            _TradeBuyerList = TradeBuyerList;
            _TradeSellerList = TradeSellerList;
            _ISignalRService = ISignalRService;
            _TradeStopLoss = TradeStopLoss;
            _WalletService = WalletService;
            _messageService = messageService;
            _pushSMSQueue = pushSMSQueue;
            _pushNotificationsQueue = pushNotificationsQueue;
            _userManager = userManager;
        }
        public async Task<BizResponse> ProcessCancelOrderAsync(CancelOrderRequest Req,string accessToken)
        {
            BizResponse _Resp = new BizResponse();
            decimal DeliverQty = 0;
            try
            {
                var TransactionQueueObj = _TransactionRepository.GetSingle(item=>item.Id==Req.TranNo);
                var TradeTranQueueObj = _TradeTransactionRepository.GetSingle(item=>item.TrnNo==Req.TranNo);
                if (TradeTranQueueObj == null)
                {
                    _Resp.ErrorCode = enErrorCode.CancelOrder_NoRecordFound;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg ="No Record Found";
                    return _Resp;
                }
                if (TradeTranQueueObj.Status != Convert.ToInt16(enTransactionStatus.Hold))
                {
                    _Resp.ErrorCode = enErrorCode.CancelOrder_TrnNotHold;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Order is not in pending State";
                    return _Resp;
                }
                if (TradeTranQueueObj.IsCancelled==1)
                {
                    _Resp.ErrorCode = enErrorCode.CancelOrder_OrderalreadyCancelled;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Cancellation request is already in processing";
                    return _Resp;
                }               

                var NewBuyRequestObj = _TradeBuyRequest.GetSingle(item => item.TrnNo == Req.TranNo &&
                                                                    (item.Status == Convert.ToInt16(enTransactionStatus.Hold) ||
                                                                    item.Status == Convert.ToInt16(enTransactionStatus.Pending)));
                if (NewBuyRequestObj != null)
                {
                    if(NewBuyRequestObj.IsProcessing==1)
                    {
                        _Resp.ErrorCode = enErrorCode.CancelOrder_YourOrderInProcessMode;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "Your Order is in process mode,please try again";
                        return _Resp;
                    }
                    if (NewBuyRequestObj.PendingQty == 0)
                    {
                        _Resp.ErrorCode = enErrorCode.CancelOrder_Yourorderfullyexecuted;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "Can not initiate Cancellation Request.Your order is fully executed";
                        return _Resp;
                    }

                    DeliverQty = Helpers.DoRoundForTrading(TransactionQueueObj.Amount * NewBuyRequestObj.PendingQty / NewBuyRequestObj.Qty, 18);//@TotalSellQty

                    if (DeliverQty == 0 || DeliverQty < 0 || DeliverQty > TransactionQueueObj.Amount)
                    {
                        _Resp.ErrorCode = enErrorCode.CancelOrder_InvalidDeliveryamount;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "Invalid Delivery amount";
                        return _Resp;
                    }

                   await CancellQueueEntry(NewBuyRequestObj.TrnNo, TransactionQueueObj.ServiceID, NewBuyRequestObj.PendingQty, DeliverQty, 0, 0, NewBuyRequestObj.UserID);
                   await CreatePoolOrderForSettlement(NewBuyRequestObj.UserID, NewBuyRequestObj.SellStockID, NewBuyRequestObj.UserID, NewBuyRequestObj.SellStockID, NewBuyRequestObj.TrnNo, DeliverQty, 0, "");
                    
                    NewBuyRequestObj.IsCancel = 1;
                    TradeTranQueueObj.SetTransactionStatusMsg("Cancellation Initiated");
                    TradeTranQueueObj.IsCancelled = 1; 

                    var ResultBool=_ICancelOrderRepository.UpdateDataObjectWithBeginTransaction(tradeCancelQueue,TradeTranQueueObj,PoolOrderObj,NewBuyRequestObj);
                    if(!ResultBool)
                    {
                        _Resp.ErrorCode = enErrorCode.CancelOrder_CancelProcessFail;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "Cancel Process Fail";
                        return _Resp;
                    }

                    tradeCancelQueue.OrderID = PoolOrderObj.Id;
                    _TradeCancelQueueRepository.Update(tradeCancelQueue);
                    //var HoldTrnNosNotExec = new List<long> { };
                    ParallelProcessTrns ParallelTrnsObj = new ParallelProcessTrns();
                    _Resp = await _SettlementRepository.PROCESSSETLLEMENT(_Resp, NewBuyRequestObj, ParallelTrnsObj, accessToken,1);
                }
                else
                {
                    _Resp.ErrorCode = enErrorCode.CancelOrder_NoRecordFound;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "No Record Found";
                    return _Resp;
                }
                
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                _Resp.ErrorCode = enErrorCode.CancelOrder_InternalError;
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = "Internal Error";
            }
            return _Resp;
        }

        public async Task CancellQueueEntry(long TrnNo, long DeliverServiceID, decimal PendingBuyQty, decimal DeliverQty, short OrderType,decimal DeliverBidPrice, long UserID)
        {
            try
            {
                tradeCancelQueue = new TradeCancelQueue()
                {
                    TrnNo = TrnNo,
                    DeliverServiceID = DeliverServiceID,
                    TrnDate = Helpers.UTC_To_IST(),
                    PendingBuyQty = PendingBuyQty,
                    DeliverQty = DeliverQty,
                    OrderType = OrderType,
                    DeliverBidPrice = DeliverBidPrice,
                    Status = 0,
                    OrderID = 0,
                    SettledDate = Helpers.UTC_To_IST(),
                    StatusMsg = "Cancel Order",
                    CreatedBy = UserID,
                    CreatedDate = Helpers.UTC_To_IST()
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CancellQueueEntry:##TrnNo " + TrnNo, ControllerName, ex);
            }
        }

        public async Task CreatePoolOrderForSettlement(long OMemberID, long DMemberID, long UserID, long PoolID, long TrnNo, decimal Amount, long CreditWalletID, string CreditAccountID)
        {
            try
            {
                PoolOrderObj = new PoolOrder()
                {
                    CreatedDate = Helpers.UTC_To_IST(),
                    CreatedBy = UserID,
                    UserID = UserID,
                    DMemberID = DMemberID, //Pool gives Amount to Member/User
                    OMemberID = OMemberID, //Member/User Take Amount from Pool
                    TrnNo = TrnNo,
                    TrnMode = 0,
                    PayMode = Convert.ToInt16(enWebAPIRouteType.TradeServiceLocal),
                    ORemarks = "Order Created",
                    OrderAmt = Amount,
                    DeliveryAmt = Amount,
                    DiscPer = 0,
                    DiscRs = 0,
                    Status = Convert.ToInt16(enTransactionStatus.Initialize),//txn type status
                    UserWalletID = CreditWalletID,
                    UserWalletAccID = CreditAccountID,
                };                
                //return PoolOrderObj;
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommSuccessMsgInternal, ReturnCode = enResponseCodeService.Success });
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreatePoolOrder:##TrnNo " + TrnNo, ControllerName, ex);
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
                throw ex;
            }

        }

        public async Task<BizResponse> ProcessCancelOrderAsyncV1(NewCancelOrderRequestCls Req)
        {
            BizResponse _Resp= new BizResponse();
            List<NewCancelOrderRequestCls> CancelObjList = new List<NewCancelOrderRequestCls>();
            if(Req.CancelAll==0) //komal 28-01-2019 for cancel multiple trxn 
            {
                CancelObjList.Add(new NewCancelOrderRequestCls()
                {
                    accessToken = Req.accessToken,
                    TranNo = Req.TranNo
                });
            }
            if (Req.CancelAll == 1) //komal 28-01-2019 for cancel multiple trxn 
            {
                var AllTrxn = _TradeTransactionRepository.FindBy(e => e.MemberID == Req.MemberID && e.Status == 4);
                foreach (var Trxn in AllTrxn)
                {
                    CancelObjList.Add(new NewCancelOrderRequestCls()
                    {
                        accessToken = Req.accessToken,
                        TranNo = Trxn.TrnNo
                    });
                }
            }
            else if(Req.CancelAll == 2) //komal 28-01-2019 for cancel multiple trxn 
            {
                var AllTrxn = _TradeTransactionRepository.FindBy(e => e.MemberID == Req.MemberID && e.Status == 4 && e.ordertype ==Convert.ToInt16(Req.OrderType));
                foreach (var Trxn in AllTrxn)
                {
                    CancelObjList.Add(new NewCancelOrderRequestCls()
                    {
                        accessToken = Req.accessToken,
                        TranNo = Trxn.TrnNo
                    });
                }
            }
            foreach (var obj in CancelObjList)
            {
                _Resp = await MiddleWareForCancellationProcess(obj);
                Task.Run(() => HelperForLog.WriteLogIntoFile("ProcessCancelOrderAsyncV1 Code " + _Resp.ErrorCode, ControllerName, _Resp.ReturnMsg + " ##TrnNo:" + obj.TranNo));

                if (_Resp.ReturnCode == enResponseCodeService.Fail)//on validation fail send Notification
                {
                    try
                    {
                        ActivityNotificationMessage notification = new ActivityNotificationMessage();
                        notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);

                        //notification.Param1 = Req.TranNo.ToString();
                        notification.Param1 = obj.TranNo.ToString(); //komal 31-01-2019  change Obj.TrnNo
                        notification.Type = Convert.ToInt16(EnNotificationType.Fail);
                        //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);
                        _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2);

                    }
                    catch (Exception ex)
                    {
                        //Task.Run(() => HelperForLog.WriteLogIntoFile("SendPushNotification ISignalRService Notification Error-ProcessCancelOrderAsyncV1", ControllerName, ex.Message + "##TrnNo:" + Req.TranNo));
                        Task.Run(() => HelperForLog.WriteErrorLog("SendPushNotification ISignalRService Notification Error-ProcessCancelOrderAsyncV1 " + "##TrnNo:" + Req.TranNo, ControllerName, ex));
                    }
                }
            }
            return _Resp;
        }
        public async Task<BizResponse> MiddleWareForCancellationProcess(NewCancelOrderRequestCls Req)
        {
            BizResponse _Resp = new BizResponse();
            try
            {
                TransactionQueue TransactionQueueObj = _TransactionRepository.GetSingle(item => item.Id == Req.TranNo);
                TradeTransactionQueue TradeTranQueueObj = _TradeTransactionRepository.GetSingle(item => item.TrnNo == Req.TranNo);
                if (TradeTranQueueObj == null)
                {
                    _Resp.ErrorCode = enErrorCode.CancelOrder_NoRecordFound;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "No Record Found";                    
                    return _Resp;
                }
                Req.MemberID = TradeTranQueueObj.MemberID;//rita 3-1-19 instead of access tocken
                if (TradeTranQueueObj.Status != Convert.ToInt16(enTransactionStatus.Hold))
                {
                    _Resp.ErrorCode = enErrorCode.CancelOrder_TrnNotHold;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Order is not in pending State";                    
                    return _Resp;
                }
                if (TradeTranQueueObj.IsCancelled == 1)
                {
                    _Resp.ErrorCode = enErrorCode.CancelOrder_OrderalreadyCancelled;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Cancellation request is already in processing";                    
                    return _Resp;
                }

                _Resp = await CancellationProcessV1(_Resp, TransactionQueueObj, TradeTranQueueObj, Req.accessToken);

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MiddleWareForCancellationProcess " + "##TrnNo:" + Req.TranNo, ControllerName, ex);
                _Resp.ErrorCode = enErrorCode.CancelOrder_InternalError;
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = "Internal Error";
            }
            return _Resp;
        }
        public async Task<BizResponse> CancellationProcessV1(BizResponse _Resp, TransactionQueue TransactionQueueObj, TradeTransactionQueue TradeTransactionQueueObj,string accessToken)
        {
            short ISPartialSettled = 0;
            CancelOrderPrice = 0; CancelOrderQty = 0;
            TradeBuyerListV1 BuyerListObj = new TradeBuyerListV1();
            TradeSellerListV1 SellerListObj = new TradeSellerListV1();
            try
            {
                await CancellQueueEntry(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.ServiceID, 0, 0, 0, 0, TradeTransactionQueueObj.MemberID);                             
                tradeCancelQueue.Status = 6;

                TradeTransactionQueueObj.SetTransactionStatusMsg("Cancellation Initiated");
                TradeTransactionQueueObj.IsCancelled = 1;
                decimal ReleaseAmt = 0;

                if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                {
                    BuyerListObj = _TradeBuyerList.GetSingle(e => e.IsProcessing == 0 && e.TrnNo == TradeTransactionQueueObj.TrnNo);
                    if (BuyerListObj == null)
                    {
                        //Uday 07-12-2018 Send SMS For Order Cancel failed
                        SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TradeTransactionQueueObj.BidPrice, TradeTransactionQueueObj.BuyQty, TransactionQueueObj.MemberMobile, 3);

                        //Uday 07-12-2018 Send Email For Order Cancel failed
                        EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID +"", TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.BuyQty, TradeTransactionQueueObj.TrnDate + "", TradeTransactionQueueObj.BidPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

                        _Resp.ErrorCode = enErrorCode.CancelOrder_UnderProcessing;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "Order is under processing,try after some time";
                        return _Resp;
                    }
                    if (BuyerListObj.RemainQty == BuyerListObj.Qty)//Not Procced
                    {
                        ISPartialSettled = 0;
                        BuyerListObj.MakeTransactionOperatorFail();
                    }
                    else//Partial Settled
                    {
                        ISPartialSettled = 1;
                        BuyerListObj.MakeTransactionSuccess();
                    }
                    tradeCancelQueue.DeliverQty = BuyerListObj.DeliveredQty;
                    tradeCancelQueue.PendingBuyQty = BuyerListObj.RemainQty;
                    //ReleaseAmt = BuyerListObj.RemainQty * BuyerListObj.Price;//Second curr canculate base on remain Qty
                    ReleaseAmt = TradeTransactionQueueObj.OrderTotalQty - TradeTransactionQueueObj.SettledSellQty;//rita 18-12-18 for market order price is zero

                    //Uday 07-12-2018 Send SMS For Order Cancel
                    CancelOrderPrice = BuyerListObj.Price;
                    CancelOrderQty = BuyerListObj.RemainQty;
                    TotalQty = TradeTransactionQueueObj.BuyQty;
                }
                else if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
                {
                    SellerListObj = _TradeSellerList.GetSingle(e => e.IsProcessing == 0 && e.TrnNo == TradeTransactionQueueObj.TrnNo);
                    if (SellerListObj == null)
                    {
                        //Uday 07-12-2018 Send SMS For Order Cancel failed
                        SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TradeTransactionQueueObj.AskPrice, TradeTransactionQueueObj.SellQty, TransactionQueueObj.MemberMobile, 3);

                        //Uday 07-12-2018 Send Email For Order Cancel failed
                        EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.SellQty, TradeTransactionQueueObj.TrnDate + "", TradeTransactionQueueObj.AskPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

                        _Resp.ErrorCode = enErrorCode.CancelOrder_UnderProcessing;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "Order is under processing,try after some time";
                        return _Resp;
                    }
                    if (SellerListObj.RemainQty == SellerListObj.Qty)//Not Procced
                    {
                        ISPartialSettled = 0;
                        SellerListObj.MakeTransactionOperatorFail();
                    }
                    else//Partial Settled
                    {
                        ISPartialSettled = 1;
                        SellerListObj.MakeTransactionSuccess();
                    }
                    tradeCancelQueue.DeliverQty = SellerListObj.SelledQty;
                    tradeCancelQueue.PendingBuyQty = SellerListObj.RemainQty;
                    ReleaseAmt = SellerListObj.RemainQty;//First curr direct

                    //Uday 07-12-2018 Send SMS For Order Cancel
                    CancelOrderPrice = SellerListObj.Price;
                    CancelOrderQty = SellerListObj.RemainQty;
                    TotalQty = TradeTransactionQueueObj.SellQty;
                }
                else
                {
                    //Uday 07-12-2018 Send SMS For Order Cancel failed
                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TradeTransactionQueueObj.AskPrice, TradeTransactionQueueObj.SellQty, TransactionQueueObj.MemberMobile, 3);

                    //Uday 07-12-2018 Send Email For Order Cancel failed
                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.SellQty, TradeTransactionQueueObj.TrnDate + "", TradeTransactionQueueObj.AskPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

                    _Resp.ErrorCode = enErrorCode.CancelOrder_StockErrorOccured;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Stock Error Occured";
                    return _Resp;
                }

                //Add Wallet Call here as Release Remain Wallet Amount
                //enWalletTrnType.Cr_ReleaseBlockAmount Rita 16-1-18 change enum as waller team changes enums
                Task<WalletDrCrResponse> WalletResult = _WalletService.GetReleaseHoldNew(TradeTransactionQueueObj.Order_Currency, Helpers.GetTimeStamp(), ReleaseAmt,
                                                 TradeTransactionQueueObj.OrderAccountID, TradeTransactionQueueObj.TrnNo, enServiceType.Trading,
                                                 enWalletTrnType.ReleaseBlockAmount, (enTrnType)TransactionQueueObj.TrnType, (EnAllowedChannels)TransactionQueueObj.TrnMode,
                                                 accessToken);

                SettledTradeTransactionQueue SettledTradeTQObj = new SettledTradeTransactionQueue();
                if (ISPartialSettled == 1)//Make Success
                {
                    Task<SettledTradeTransactionQueue> SettledTradeTQResult = MakeTransactionSettledEntry(TradeTransactionQueueObj, CancelOrderPrice, 1);
                    TradeTransactionQueueObj.MakeTransactionSuccess();
                    TradeTransactionQueueObj.SetTransactionStatusMsg("Success with partial cancellation");
                    TradeTransactionQueueObj.SettledDate = Helpers.UTC_To_IST();
                    TransactionQueueObj.MakeTransactionSuccess();
                    TransactionQueueObj.SetTransactionStatusMsg("Success with partial cancellation");
                    SettledTradeTQObj = await SettledTradeTQResult;                    
                }
                else//Make Fail
                {
                    TradeTransactionQueueObj.MakeTransactionOperatorFail();
                    TradeTransactionQueueObj.SetTransactionStatusMsg("Full Order Cancellation");
                    TradeTransactionQueueObj.SettledDate = Helpers.UTC_To_IST();
                    TransactionQueueObj.MakeTransactionOperatorFail();
                    TransactionQueueObj.SetTransactionStatusMsg("Full Order Cancellation");
                }
                //rita 29-11-18 update errorcode
                TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
                TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));

                tradeCancelQueue.Status = 1;
                tradeCancelQueue.SettledDate = Helpers.UTC_To_IST();
                tradeCancelQueue.UpdatedDate = Helpers.UTC_To_IST();
                tradeCancelQueue.StatusMsg = "Cancellation Successful.";
                var WalletResp = await WalletResult;

                if (WalletResp.ReturnCode != enResponseCode.Success)
                {
                    //Uday 07-12-2018 Send SMS For Order Cancel failed
                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, CancelOrderPrice,CancelOrderQty, TransactionQueueObj.MemberMobile, 3);

                    //Uday 07-12-2018 Send Email For Order Cancel failed
                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, CancelOrderQty, TradeTransactionQueueObj.TrnDate + "", CancelOrderPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

                    _Resp.ErrorCode = enErrorCode.CancelOrder_CancelProcessFail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Cancel Process Fail as Wallet Process Fail " + WalletResp.ReturnMsg;
                    return _Resp;
                }

                var ResultBool = _ICancelOrderRepository.UpdateDataObjectWithBeginTransactionV1(tradeCancelQueue, TransactionQueueObj,TradeTransactionQueueObj, BuyerListObj, SellerListObj, SettledTradeTQObj, ISPartialSettled);
                if (!ResultBool)
                {
                    //Uday 07-12-2018 Send SMS For Order Cancel failed
                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, CancelOrderPrice, CancelOrderQty, TransactionQueueObj.MemberMobile, 3);

                    //Uday 07-12-2018 Send Email For Order Cancel failed
                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, CancelOrderQty, TradeTransactionQueueObj.TrnDate + "", CancelOrderPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

                    _Resp.ErrorCode = enErrorCode.CancelOrder_CancelProcessFail;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Cancel Process Fail";
                    return _Resp;
                }

                //Uday 07-12-2018 Send SMS and email When Order Cancel
                if (ISPartialSettled != 1) // Full Cancel
                {
                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo,CancelOrderPrice,CancelOrderQty,TransactionQueueObj.MemberMobile, 2);

                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, CancelOrderQty, TradeTransactionQueueObj.TrnDate + "", CancelOrderPrice, 0, 1, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);
                }
                else // Partial Cancel
                {
                    SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo,CancelOrderPrice,CancelOrderQty, TransactionQueueObj.MemberMobile, 1);

                    EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, TotalQty, TradeTransactionQueueObj.TrnDate + "", CancelOrderPrice, 0, 2, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType,(TotalQty-CancelOrderQty),CancelOrderQty);
                }

                try
                {
                    var MakeTradeStopLossObj = _TradeStopLoss.GetSingle(e => e.TrnNo == TransactionQueueObj.Id);
                    _ISignalRService.OnStatusCancel(TradeTransactionQueueObj.Status, TransactionQueueObj, TradeTransactionQueueObj, "", MakeTradeStopLossObj.ordertype, ISPartialSettled);

                    //await EmailSendAsync(TransactionQueueObj.MemberID.ToString(), Convert.ToInt16(enTransactionStatus.Success), TradeTransactionQueueObj.PairName,
                    //          TradeTransactionQueueObj.PairName.Split("_")[1], TradeTransactionQueueObj.TrnDate.ToString(),
                    //          TradeTransactionQueueObj.DeliveryTotalQty, TradeTransactionQueueObj.OrderTotalQty, 0);
                }
                catch (Exception ex)
                {
                    //HelperForLog.WriteLogIntoFile("ISignalRService CancellationProcessV1", ControllerName, "Error " + ex.Message + "##TrnNo:" + TradeTransactionQueueObj.TrnNo);
                    HelperForLog.WriteErrorLog("ISignalRService CancellationProcessV1 Error ##TrnNo:" + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
                }
                _Resp.ErrorCode = enErrorCode.CancelOrder_ProccedSuccess;
                _Resp.ReturnCode = enResponseCodeService.Success;
                _Resp.ReturnMsg = "Order Cancellled Successfully";
                return _Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CancellationProcess V1:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
                //_dbContext.Database.RollbackTransaction();
            }
            return _Resp;
        }
        public async Task<SettledTradeTransactionQueue> MakeTransactionSettledEntry(TradeTransactionQueue TradeTransactionQueueObj,decimal Price,short IsCancelled = 0)
        {
            decimal BidPrice = TradeTransactionQueueObj.BidPrice;
            decimal AskPrice = TradeTransactionQueueObj.AskPrice;
            //rita 7-1-18 need for graph canculation
            //rita 3-1-18 set same price also in market order
            if (TradeTransactionQueueObj.ordertype == 2)
            {
                if (TradeTransactionQueueObj.TrnType == 4)//Buy
                {
                    BidPrice = Price;
                }
                else//Sell
                {
                    AskPrice = Price;
                }
            }
            try
            {
                var SettledTradeTQObj = new SettledTradeTransactionQueue()
                {
                    TrnNo = TradeTransactionQueueObj.TrnNo,
                    TrnDate = TradeTransactionQueueObj.TrnDate,
                    SettledDate = Helpers.UTC_To_IST(),
                    TrnType = TradeTransactionQueueObj.TrnType,
                    TrnTypeName = TradeTransactionQueueObj.TrnTypeName,
                    MemberID = TradeTransactionQueueObj.MemberID,
                    PairID = TradeTransactionQueueObj.PairID,
                    PairName = TradeTransactionQueueObj.PairName,
                    OrderWalletID = TradeTransactionQueueObj.OrderWalletID,
                    DeliveryWalletID = TradeTransactionQueueObj.DeliveryWalletID,
                    OrderAccountID = TradeTransactionQueueObj.OrderAccountID,
                    DeliveryAccountID = TradeTransactionQueueObj.DeliveryAccountID,
                    BuyQty = TradeTransactionQueueObj.BuyQty,
                    BidPrice = BidPrice,
                    SellQty = TradeTransactionQueueObj.SellQty,
                    AskPrice = AskPrice,
                    Order_Currency = TradeTransactionQueueObj.Order_Currency,
                    OrderTotalQty = TradeTransactionQueueObj.OrderTotalQty,
                    Delivery_Currency = TradeTransactionQueueObj.Delivery_Currency,
                    DeliveryTotalQty = TradeTransactionQueueObj.DeliveryTotalQty,
                    SettledBuyQty = TradeTransactionQueueObj.SettledBuyQty,
                    SettledSellQty = TradeTransactionQueueObj.SettledSellQty,
                    Status = 1,//TradeTransactionQueueObj.Status,//rita 3-1-19
                    StatusCode = TradeTransactionQueueObj.StatusCode,
                    StatusMsg = TradeTransactionQueueObj.StatusMsg,
                    IsCancelled = IsCancelled
                };
                return SettledTradeTQObj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MakeTransactionSettledEntry:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
                throw ex;
            }
        }

        #region Send SMS And Email
        public async Task SMSSendCancelTransaction(long TrnNo,decimal Price,decimal Qty,string MobileNumber,int CancelType)
        {
            try
            {
                if (!string.IsNullOrEmpty(MobileNumber))
                {
                    SendSMSRequest SendSMSRequestObj = new SendSMSRequest();
                    ApplicationUser User = new ApplicationUser();
                    TemplateMasterData SmsData = new TemplateMasterData();

                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    communicationParamater.Param1 = TrnNo + "";
                    communicationParamater.Param2 = Price + "";
                    communicationParamater.Param3 = Qty + "";
                

                    Task.Run(() => HelperForLog.WriteLogIntoFile("SMSSendCancelTransaction", ControllerName, " ##TrnNo : " + TrnNo ));

                    if (CancelType == 1) // Partially Cancel
                    {
                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_PartialOrderCancel, communicationParamater, enCommunicationServiceType.SMS).Result;
                    }
                    else if (CancelType == 2) // Full Cancel
                    {
                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_OrderCancel, communicationParamater, enCommunicationServiceType.SMS).Result;
                    }
                    else if (CancelType == 3) // Cancel Fail
                    {
                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_OrderCancelFailed, communicationParamater, enCommunicationServiceType.SMS).Result;
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
                Task.Run(() => HelperForLog.WriteErrorLog("SMSSendCancelTransaction:##TrnNo " + TrnNo, ControllerName, ex));
            }
        }

        public async Task EmailSendCancelTransaction(long TrnNo,string UserId,string pair,decimal qty,string datetime,decimal price,decimal fee,int CancelType,short OrderType,short TrnType,decimal SettledQty = 0,decimal CancelQty = 0)
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
                    Task.Run(() => HelperForLog.WriteLogIntoFile("SendEmailTransaction - EmailSendCancelTransaction", ControllerName, " ##TrnNo : " + TrnNo + " ##Type : " + CancelType));


                    communicationParamater.Param8 = User.UserName + "";
                    communicationParamater.Param1 = pair + "";
                    communicationParamater.Param2 = Helpers.DoRoundForTrading(qty, 8).ToString();
                    communicationParamater.Param3 = pair.Split("_")[1];
                    communicationParamater.Param4 = datetime;
                    communicationParamater.Param5 = Helpers.DoRoundForTrading(price, 8).ToString();
                    communicationParamater.Param6 = Helpers.DoRoundForTrading(fee, 8).ToString();
                    communicationParamater.Param7 = Helpers.DoRoundForTrading((fee + (price * qty)), 8).ToString();   //Uday 01-01-2019 In Final Price Calculation Change
                    communicationParamater.Param11 = ((enTransactionMarketType)OrderType).ToString();  //Uday 01-01-2019 Add OrderType In Email
                    communicationParamater.Param12 = ((enTrnType)TrnType).ToString();  //Uday 01-01-2019 Add TranType In Email
                    communicationParamater.Param13 = TrnNo.ToString(); //Uday 01-01-2019 Add TrnNo In Email

                    if (CancelType == 1) // Cancel Success
                    {
                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_OrderCancel, communicationParamater, enCommunicationServiceType.Email).Result;
                    }
                    else if (CancelType == 2) // Cancel Partially
                    {
                        communicationParamater.Param9 = Helpers.DoRoundForTrading(SettledQty, 8).ToString();
                        communicationParamater.Param10 = Helpers.DoRoundForTrading(CancelQty, 8).ToString();
                        communicationParamater.Param7 = Helpers.DoRoundForTrading((fee + (price * SettledQty)), 8).ToString();     //Uday 01-01-2019 In Final Price Calculation Change

                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_PartialOrderCancel, communicationParamater, enCommunicationServiceType.Email).Result;
                    }
                    else if (CancelType == 3) // Cancel Failed
                    {
                        communicationParamater.Param7 = Helpers.DoRoundForTrading(0, 18).ToString();
                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_OrderCancelFailed, communicationParamater, enCommunicationServiceType.Email).Result;
                    }

                    if (EmailData != null)
                    {
                        Request.Body = EmailData.Content;
                        Request.Subject = EmailData.AdditionalInfo;
                        Request.Recepient =  User.Email;
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
