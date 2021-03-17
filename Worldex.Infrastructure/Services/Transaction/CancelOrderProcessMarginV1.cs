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
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Data;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Transaction
{
    public class CancelOrderProcessMarginV1 : ICancelOrderProcessMarginV1
    {
        private readonly EFCommonRepository<TransactionQueueMargin> _TransactionRepository;
        private readonly EFCommonRepository<TradeTransactionQueueMargin> _TradeTransactionRepository;
        private readonly ICancelOrderRepositoryMargin _ICancelOrderRepository;
        private readonly ICommonRepository<TradeCancelQueueMargin> _TradeCancelQueueRepository;
        private readonly ICommonRepository<TradeBuyerListMarginV1> _TradeBuyerList;
        private readonly ICommonRepository<TradeSellerListMarginV1> _TradeSellerList;
        private readonly ISignalRService _ISignalRService;
        private readonly ICommonRepository<TradeStopLossMargin> _TradeStopLoss;
        TradeCancelQueueMargin tradeCancelQueue;
        private readonly IMarginTransactionWallet _WalletService;//Rita 05-12-18 for release remain wallet amount
        private readonly IMessageService _messageService;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITradeReconProcessMarginV1 _tradeReconProcessV1;//Rita 15-5-19 for release stuck order

        //PoolOrder PoolOrderObj; //komal 03 May 2019, Cleanup
        string ControllerName = "CancelOrderProcessMarginV1";
        decimal CancelOrderPrice = 0, CancelOrderQty = 0, TotalQty = 0;

        public CancelOrderProcessMarginV1(EFCommonRepository<TradeTransactionQueueMargin> TradeTransactionRepository, 
            ICommonRepository<TradeCancelQueueMargin> TradeCancelQueueRepository, ICancelOrderRepositoryMargin ICancelOrderRepository,
            EFCommonRepository<TransactionQueueMargin> TransactionRepository, ICommonRepository<TradeBuyerListMarginV1> TradeBuyerList, 
            ICommonRepository<TradeSellerListMarginV1> TradeSellerList, ISignalRService ISignalRService, IMarginTransactionWallet WalletService,
            ICommonRepository<TradeStopLossMargin> TradeStopLoss, IMessageService messageService, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue, 
            IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, UserManager<ApplicationUser> userManager,
            ITradeReconProcessMarginV1 tradeReconProcessV1)
        {
            _TradeTransactionRepository = TradeTransactionRepository;
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
            _tradeReconProcessV1 = tradeReconProcessV1;
        }     

        public async Task<BizResponse> ProcessCancelOrderAsyncV1(NewCancelOrderRequestCls Req)
        {
            BizResponse _Resp = new BizResponse();
            try//Rita 11-4-19 added try catch
            {                
                List<NewCancelOrderRequestCls> CancelObjList = new List<NewCancelOrderRequestCls>();
                if (Req.CancelAll == 0) //komal 28-01-2019 for cancel multiple trxn 
                {
                    CancelObjList.Add(new NewCancelOrderRequestCls()
                    {
                        accessToken = Req.accessToken,
                        TranNo = Req.TranNo,
                        MemberID = Req.MemberID,
                        CancelAll = Req.CancelAll,
                        OrderType = Req.OrderType,
                        IsMargin= Req.IsMargin,
                        TrnRefNo= Req.TrnRefNo,
                        TrnMode= Req.TrnMode
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
                            TranNo = Trxn.TrnNo,
                            MemberID = Req.MemberID,
                            CancelAll = Req.CancelAll,
                            OrderType = Req.OrderType,
                            IsMargin = Req.IsMargin,
                            TrnRefNo = Req.TrnRefNo,
                            TrnMode = Req.TrnMode
                        });
                    }
                }
                else if (Req.CancelAll == 2) //komal 28-01-2019 for cancel multiple trxn 
                {
                    var AllTrxn = _TradeTransactionRepository.FindBy(e => e.MemberID == Req.MemberID && e.Status == 4 && e.ordertype == Convert.ToInt16(Req.OrderType));
                    foreach (var Trxn in AllTrxn)
                    {
                        CancelObjList.Add(new NewCancelOrderRequestCls()
                        {
                            accessToken = Req.accessToken,
                            TranNo = Trxn.TrnNo,
                            MemberID = Req.MemberID,
                            CancelAll = Req.CancelAll,
                            OrderType = Req.OrderType,
                            IsMargin = Req.IsMargin,
                            TrnRefNo = Req.TrnRefNo,
                            TrnMode = Req.TrnMode
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
                    else//Rita 4-10-19 ping wallet for Re-calculate Margin wallet as Order cancelled and release amount successfully
                    {
                        if (Req.TrnRefNo != "" && Req.TrnMode == EnAllowedChannels.WalletSystemInternal)
                        {
                            BizResponseClass bizResponse = await _WalletService.SettleMarketOrderForCharge(Convert.ToInt64(Req.TrnRefNo));
                        }
                    }
                }
                //Task.Delay(2000).Wait();//Rita 27-2-19 wait for all calls
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessCancelOrderAsyncV1:##TrnNo " + Req.TranNo, ControllerName, ex);
            }
            return _Resp;
        }
        public async Task<BizResponse> MiddleWareForCancellationProcess(NewCancelOrderRequestCls Req)
        {
            BizResponse _Resp = new BizResponse();
            try
            {
                TransactionQueueMargin TransactionQueueObj = _TransactionRepository.GetSingle(item => item.Id == Req.TranNo);
                TradeTransactionQueueMargin TradeTranQueueObj = _TradeTransactionRepository.GetSingle(item => item.TrnNo == Req.TranNo);
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
                if (TradeTranQueueObj.IsCancelled == 1|| TradeTranQueueObj.IsAPICancelled == 1)//Rita 5-2-19 , this bit only set for API trading cancellation
                {
                    _Resp.ErrorCode = enErrorCode.CancelOrder_OrderalreadyCancelled;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Cancellation request is already in processing";                    
                    return _Resp;
                }
                //Rita System Internal Stop and limit order does not fail by user side
                if (TradeTranQueueObj.ordertype==4 && Req.TrnMode!=EnAllowedChannels.TrnSystemInternal && TradeTranQueueObj.IsWithoutAmtHold==1 && TradeTranQueueObj.ISOrderBySystem == 1)
                {
                    _Resp.ErrorCode = enErrorCode.CancelOrder_SystemOrderCanNotCancelByUser;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "You can not cancel System Order";
                    return _Resp;
                }

                if (TradeTranQueueObj.IsAPITrade==0)//Local Trade
                {
                    _Resp = await CancellationProcessV1(_Resp, TransactionQueueObj, TradeTranQueueObj, Req.accessToken);
                }
                //else//API trade
                //{
                //    _Resp = await CancellationProcessAPIV1(_Resp, TransactionQueueObj, TradeTranQueueObj, Req.accessToken);
                //}               

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
        public async Task<BizResponse> CancellationProcessV1(BizResponse _Resp, TransactionQueueMargin TransactionQueueObj, TradeTransactionQueueMargin TradeTransactionQueueObj,string accessToken)
        {
            short ISPartialSettled = 0;
            CancelOrderPrice = 0; CancelOrderQty = 0;
            TradeBuyerListMarginV1 BuyerListObj = new TradeBuyerListMarginV1();
            TradeSellerListMarginV1 SellerListObj = new TradeSellerListMarginV1();
            try
            {
                await CancellQueueEntry(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.ServiceID, 0, 0, 0, 0, TradeTransactionQueueObj.MemberID);                             
                tradeCancelQueue.Status = 6;

                TradeTransactionQueueObj.SetTransactionStatusMsg("Cancellation Initiated");
                TradeTransactionQueueObj.IsCancelled = 1;
                decimal ReleaseAmt = 0;

                if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                {
                    //BuyerListObj = _TradeBuyerList.GetSingle(e => e.IsProcessing == 0 && e.TrnNo == TradeTransactionQueueObj.TrnNo);
                    BuyerListObj = _TradeBuyerList.GetSingle(e => e.TrnNo == TradeTransactionQueueObj.TrnNo);
                    if (BuyerListObj == null)
                    {
                        ISPartialSettled = 0;// not any settlement proceed this type of txn
                        tradeCancelQueue.DeliverQty = 0;
                        tradeCancelQueue.PendingBuyQty = TradeTransactionQueueObj.BuyQty;   //total pending                    
                        CancelOrderPrice = TradeTransactionQueueObj.BidPrice;
                        CancelOrderQty = TradeTransactionQueueObj.BuyQty;
                        TotalQty = TradeTransactionQueueObj.BuyQty;                       
                    }
                    else
                    {
                        //Rita 6-5-19 remove && BuyerListObj.Status!=0
                        if (BuyerListObj.IsProcessing == 1 && BuyerListObj.CreatedDate > Helpers.UTC_To_IST().AddMinutes(-3))//Rita 25-4-19 15 minute old,make cancel
                        {
                            //Uday 07-12-2018 Send SMS For Order Cancel failed
                            SMSSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TradeTransactionQueueObj.BidPrice, TradeTransactionQueueObj.BuyQty, TransactionQueueObj.MemberMobile, 3);

                            //Uday 07-12-2018 Send Email For Order Cancel failed
                            EmailSendCancelTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID + "", TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.BuyQty, TradeTransactionQueueObj.TrnDate + "", TradeTransactionQueueObj.BidPrice, 0, 3, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType);

                            _Resp.ErrorCode = enErrorCode.CancelOrder_UnderProcessing;
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ReturnMsg = "Order is under processing,try after some time";
                            return _Resp;
                        }
                        else if (BuyerListObj.IsProcessing == 1)//Rita 26-4-19 does not update before this code , as this method updates entity in DB
                        {
                            await _tradeReconProcessV1.ProcessReleaseStuckOrderOrderAsync(new BizResponseClass(), TradeTransactionQueueObj, TradeTransactionQueueObj.MemberID,1);//rita 17-5-19 last bit=1 added
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
                       
                        //Uday 07-12-2018 Send SMS For Order Cancel
                        CancelOrderPrice = BuyerListObj.Price;
                        CancelOrderQty = BuyerListObj.RemainQty;
                        TotalQty = TradeTransactionQueueObj.BuyQty;
                    }
                    //ReleaseAmt = BuyerListObj.RemainQty * BuyerListObj.Price;//Second curr canculate base on remain Qty
                    ReleaseAmt = TradeTransactionQueueObj.OrderTotalQty - TradeTransactionQueueObj.SettledSellQty;//rita 18-12-18 for market order price is zero
                }
                else if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
                {
                    //SellerListObj = _TradeSellerList.GetSingle(e => e.IsProcessing == 0 && e.TrnNo == TradeTransactionQueueObj.TrnNo);
                    SellerListObj = _TradeSellerList.GetSingle(e=>e.TrnNo == TradeTransactionQueueObj.TrnNo);
                    if (SellerListObj == null)
                    {
                        ISPartialSettled = 0;// not any settlement proceed this type of txn
                        tradeCancelQueue.DeliverQty = 0;
                        tradeCancelQueue.PendingBuyQty = TradeTransactionQueueObj.SellQty;
                        CancelOrderPrice = TradeTransactionQueueObj.AskPrice;
                        CancelOrderQty = TradeTransactionQueueObj.SellQty;
                        TotalQty = TradeTransactionQueueObj.SellQty;
                        ReleaseAmt = TradeTransactionQueueObj.SellQty;
                    }
                    else
                    {
                        //Rita 6-5-19 remove && BuyerListObj.Status!=0
                        if (SellerListObj.IsProcessing == 1 && SellerListObj.CreatedDate > Helpers.UTC_To_IST().AddMinutes(-3))//Rita 25-4-19 15 minute old,make cancel
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
                        else if (SellerListObj.IsProcessing == 1)//Rita 26-4-19 does not update before this code , as this method updates entity in DB
                        {
                            await _tradeReconProcessV1.ProcessReleaseStuckOrderOrderAsync(new BizResponseClass(), TradeTransactionQueueObj, TradeTransactionQueueObj.MemberID,1);
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
                Task<WalletDrCrResponse> WalletResult = _WalletService.MarginGetReleaseHoldNew(TradeTransactionQueueObj.Order_Currency, Helpers.GetTimeStamp(), ReleaseAmt,
                                                 TradeTransactionQueueObj.OrderAccountID, TradeTransactionQueueObj.TrnNo, enServiceType.Trading,
                                                 enMarginWalletTrnType.ReleaseBlockAmount, (enTrnType)TransactionQueueObj.TrnType, (EnAllowedChannels)TransactionQueueObj.TrnMode,
                                                 accessToken);

                SettledTradeTransactionQueueMargin SettledTradeTQObj = new SettledTradeTransactionQueueMargin();
                if (ISPartialSettled == 1)//Make Success
                {
                    Task<SettledTradeTransactionQueueMargin> SettledTradeTQResult = MakeTransactionSettledEntry(TradeTransactionQueueObj, CancelOrderPrice, 1);
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
                
                //Rita 13-4-19 IF system Order,so amount is not debited so direct make fail
                if (WalletResp.ReturnCode != enResponseCode.Success && TradeTransactionQueueObj.ISOrderBySystem != 1 && TradeTransactionQueueObj.IsWithoutAmtHold != 1)
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
                    _ISignalRService.OnStatusCancelMargin(TradeTransactionQueueObj.Status, TransactionQueueObj, TradeTransactionQueueObj, "", MakeTradeStopLossObj.ordertype, ISPartialSettled);

                    //await EmailSendAsync(TransactionQueueObj.MemberID.ToString(), Convert.ToInt16(enTransactionStatus.Success), TradeTransactionQueueObj.PairName,
                    //          TradeTransactionQueueObj.PairName.Split("_")[1], TradeTransactionQueueObj.TrnDate.ToString(),
                    //          TradeTransactionQueueObj.DeliveryTotalQty, TradeTransactionQueueObj.OrderTotalQty, 0);
                }
                catch (Exception ex)
                {
                    //HelperForLog.WriteLogIntoFile("ISignalRService CancellationProcessV1", ControllerName, "Error " + ex.Message + "##TrnNo:" + TradeTransactionQueueObj.TrnNo);
                    HelperForLog.WriteErrorLog("ISignalRService CancellationProcessV1 Error ##TrnNo:" + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
                }
                Task.Delay(5000).Wait();//Rita 27-2-19 wait for all calls
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

        public async Task CancellQueueEntry(long TrnNo, long DeliverServiceID, decimal PendingBuyQty, decimal DeliverQty, short OrderType, decimal DeliverBidPrice, long UserID)
        {
            try
            {
                tradeCancelQueue = new TradeCancelQueueMargin()
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

        public async Task<SettledTradeTransactionQueueMargin> MakeTransactionSettledEntry(TradeTransactionQueueMargin TradeTransactionQueueObj,decimal Price,short IsCancelled = 0)
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
                var SettledTradeTQObj = new SettledTradeTransactionQueueMargin()
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
                        communicationParamater.Param7 = Helpers.DoRoundForTrading(0, 8).ToString();
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
