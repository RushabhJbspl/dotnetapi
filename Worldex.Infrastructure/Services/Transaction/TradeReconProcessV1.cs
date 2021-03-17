using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Data;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Transaction
{
    public class TradeReconProcessV1 : ITradeReconProcessV1
    {
        private readonly ICommonRepository<TradeBuyRequest> _TradeBuyRequest;
        private readonly EFCommonRepository<TransactionQueue> _TransactionRepository;
        private readonly EFCommonRepository<TradeTransactionQueue> _TradeTransactionRepository;
        //private readonly EFCommonRepository<TradeBuyerListV1> _TradeBuyerListV1Repository;
        //private readonly EFCommonRepository<TradeSellerListV1> _TradeSellerListV1Repository;
        private readonly ICancelOrderRepository _ICancelOrderRepository;
        private readonly ICommonRepository<TradeCancelQueue> _TradeCancelQueueRepository;
        private readonly EFCommonRepository<TradeBuyerListV1> _TradeBuyerList;
        private readonly EFCommonRepository<TradeSellerListV1> _TradeSellerList;
        private readonly ISignalRService _ISignalRService;
        private readonly ICommonRepository<TradeStopLoss> _TradeStopLoss;
        private readonly IWalletService _WalletService;//Rita 05-12-18 for release remain wallet amount
        private readonly IMessageService _messageService;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly UserManager<ApplicationUser> _userManager;
        //komal liquidity provider 
        private readonly EFCommonRepository<ServiceProviderDetail> _ServiceProviderDetail;
        private readonly EFCommonRepository<TransactionRequest> _TranRequestRepository;
        private readonly IGetWebRequest _IGetWebRequest;
        private readonly ICommonRepository<TransactionRequest> _TransactionRequest;
        //private readonly ICancelOrderProcessV1 _cancelOrderProcess;
        private readonly ICommonRepository<TradingRecon> _tradingRecon;
        private readonly EFCommonRepository<SettledTradeTransactionQueue> _settledTradeTransactionRepository;
        private readonly ISettlementRepositoryV1<BizResponse> _settlementRepositoryV1;
        private readonly ICommonRepository<TradePoolQueueV1> _tradePoolQueueV1;
        private readonly IMediator _mediator;


        string ControllerName = "TradeReconProcessV1";
        TradeSellerListV1 SellerList;
        TradeBuyerListV1 BuyerList;
        SettledTradeTransactionQueue settledTradeObj;
        TradeBuyerListV1 TradeBuyerListObj;
        TradeSellerListV1 TradeSellerListObj;

        #region ctor

        public TradeReconProcessV1(ICommonRepository<TradeBuyRequest> TradeBuyRequest, EFCommonRepository<TradeTransactionQueue> TradeTransactionRepository, 
            ICommonRepository<TradeCancelQueue> TradeCancelQueueRepository,ICancelOrderRepository ICancelOrderRepository,
            EFCommonRepository<TransactionQueue> TransactionRepository, EFCommonRepository<TradeBuyerListV1> TradeBuyerList,
            EFCommonRepository<TradeSellerListV1> TradeSellerList, ISignalRService ISignalRService, IWalletService WalletService,
            ICommonRepository<TradeStopLoss> TradeStopLoss, IMessageService messageService, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue, 
            IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, UserManager<ApplicationUser> userManager,
            EFCommonRepository<ServiceProviderDetail> ServiceProviderDetail, EFCommonRepository<TransactionRequest> TranRequestRepository,
            IGetWebRequest IGetWebRequest, ICommonRepository<TransactionRequest> TransactionRequest, //ICancelOrderProcessV1 CancelOrderProcess,
            ICommonRepository<TradingRecon> TradingRecon,
            EFCommonRepository<SettledTradeTransactionQueue> SettledTradeTransactionRepository , 
            ISettlementRepositoryV1<BizResponse> SettlementRepositoryV1, ICommonRepository<TradePoolQueueV1> TradePoolQueueV1, IMediator mediator)
        {
            _TradeBuyRequest = TradeBuyRequest;
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
            _ServiceProviderDetail = ServiceProviderDetail;
            _TranRequestRepository = TranRequestRepository;
            _IGetWebRequest = IGetWebRequest;
            _TransactionRequest = TransactionRequest;
            //_cancelOrderProcess = CancelOrderProcess;
            _tradingRecon = TradingRecon;
            //_TradeBuyerListV1Repository = TradeBuyerListV1Repository;
            //_TradeSellerListV1Repository = TradeSellerListV1Repository;
            _settledTradeTransactionRepository = SettledTradeTransactionRepository;
            _settlementRepositoryV1 = SettlementRepositoryV1;
            _tradePoolQueueV1 = TradePoolQueueV1;
            _mediator = mediator;
        }

        #endregion

        #region "Transaction Process on Action stage"

        public async Task<BizResponseClass> ProcessCancelMarkOrderAsync(BizResponseClass Response, TradeTransactionQueue TrnObj, string AccessToken)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessCancelMarkOrderAsync", ControllerName, "Process Cancel MarkOrder start " + "##TrnNo:" + TrnObj.TrnNo, Helpers.UTC_To_IST()));

                if (TrnObj.Status != Convert.ToInt16(enTransactionStatus.Hold))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidTransactionStatus;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionStatus;
                    return Response;
                }
                var cancelOrderRequest = new NewCancelOrderRequestClsV2()
                {
                    TranNo = TrnObj.TrnNo,
                    accessToken = AccessToken
                };
                //var response = await _cancelOrderProcess.ProcessCancelOrderAsyncV1(cancelOrderRequest);
                var response = await _mediator.Send(cancelOrderRequest);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessCancelMarkOrderAsync", ControllerName, "Process Cancel MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE###" + response.ReturnMsg, Helpers.UTC_To_IST()));

                Response.ReturnCode = (enResponseCode)enResponseCodeService.Parse(typeof(enResponseCodeService), response.ReturnCode.ToString());
                Response.ErrorCode = response.ErrorCode;
                Response.ReturnMsg = response.ReturnMsg;
                return Response;
                
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }            
        }

        public async Task<BizResponseClass> ProcessFailMarkOrderAsync(BizResponseClass Response, TradeTransactionQueue TrnObj, string AccessToken)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessFailMarkOrderAsync", ControllerName, "Process Fail MarkOrder start " + "##TrnNo:" + TrnObj.TrnNo, Helpers.UTC_To_IST()));

                if (!(TrnObj.Status == Convert.ToInt16(enTransactionStatus.Hold) || TrnObj.Status == Convert.ToInt16(enTransactionStatus.Success)))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidTransactionStatus;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionStatus;
                    return Response;
                }
                
                TrnObj.Status = Convert.ToInt16(enTransactionStatus.OperatorFail);
                UpdateTransactionSettledEntry(TrnObj);// update or create Settle entry
                UpdateBuyerSellerListEntry(TrnObj);  // update buyer seller list status
                Task.WaitAll();

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessFailMarkOrderAsync", ControllerName, "Process Fail MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### :Success", Helpers.UTC_To_IST()));

                Response.ReturnCode = enResponseCode.Success;
                Response.ErrorCode = enErrorCode.TradeRecon_RequestFail;
                Response.ReturnMsg = EnResponseMessage.TradeRecon_RequestFail;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> ProcessReInitMarkOrderAsync(BizResponseClass Response, TradeTransactionQueue TrnObj, string AccessToken)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessReInitMarkOrderAsync", ControllerName, "Process ReInit Mark Order start " + "##TrnNo:" + TrnObj.TrnNo, Helpers.UTC_To_IST()));

                if (!(TrnObj.Status == Convert.ToInt16(enTransactionStatus.Success) || TrnObj.Status == Convert.ToInt16(enTransactionStatus.OperatorFail) || TrnObj.Status == Convert.ToInt16(enTransactionStatus.SystemFail)))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidTransactionStatus;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionStatus;
                    return Response;
                }

                if(TrnObj.Status == Convert.ToInt16(enTransactionStatus.OperatorFail) || TrnObj.Status == Convert.ToInt16(enTransactionStatus.SystemFail))
                {
                    // Wallet operation - direct Debit wallet and deduct charge
                    var DebitResult = await _WalletService.GetWalletHoldNew(TrnObj.Order_Currency, Helpers.GetTimeStamp(), TrnObj.OrderTotalQty,
                        TrnObj.OrderAccountID, TrnObj.TrnNo, enServiceType.Trading, TrnObj.TrnType == (short)enTrnType.Buy_Trade ? enWalletTrnType.BuyTrade : enWalletTrnType.SellTrade, (enTrnType)(TrnObj.TrnType), EnAllowedChannels.Web, AccessToken, (enWalletDeductionType)((short)TrnObj.ordertype),(TrnObj.TrnType == (short)enTrnType.Buy_Trade ? TrnObj.Order_Currency : TrnObj.Delivery_Currency));//2019-9-17 add market currency in request as per worldex proj requirement

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessReInitMarkOrderAsync", ControllerName, "Process ReInit Mark Order End 1" + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE###" + DebitResult.ReturnMsg, Helpers.UTC_To_IST()));


                    if (DebitResult.ReturnCode == enResponseCode.Success)
                    {
                        goto UpdateTradeEntry;                        
                    }
                    else if (DebitResult.ReturnCode == enResponseCode.Fail && DebitResult.ErrorCode == enErrorCode.sp_InsufficientBalanceForCharge)
                    {
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessReInitMarkOrderAsync", ControllerName, "Process ReInit Mark  MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : FAIL", Helpers.UTC_To_IST()));

                        Response.ReturnCode = DebitResult.ReturnCode;
                        Response.ErrorCode = enErrorCode.ReconInsufficientBalanceForCharge;
                        Response.ReturnMsg = DebitResult.ReturnMsg;
                        return Response;
                    }
                    else if (DebitResult.ReturnCode == enResponseCode.Fail && DebitResult.ErrorCode == enErrorCode.sp_InsufficientBalance)
                    {
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessSuccessAndDebitOrderAsync", ControllerName, "Process Success and Debit MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : FAIL", Helpers.UTC_To_IST()));

                        Response.ReturnCode = DebitResult.ReturnCode;
                        Response.ErrorCode = enErrorCode.ReconInsufficientBalance;
                        Response.ReturnMsg = DebitResult.ReturnMsg;
                        return Response;
                    }
                    else if (DebitResult.ReturnCode == enResponseCode.Fail && DebitResult.ErrorCode == enErrorCode.AlredyExist)
                    {
                        goto UpdateTradeEntry;
                    }
                    else
                    {
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessSuccessAndDebitOrderAsync", ControllerName, "Process Success and Debit MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : " + DebitResult.ReturnMsg, Helpers.UTC_To_IST()));
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = DebitResult.ErrorCode;
                        Response.ReturnMsg = EnResponseMessage.TradeRecon_ActionFailed;
                        return Response;
                    }
                }
                else if(TrnObj.Status == Convert.ToInt16(enTransactionStatus.Success) && TrnObj.IsAPITrade != 0)
                {
                    goto UpdateTradeEntry;
                }
                else if (TrnObj.Status == Convert.ToInt16(enTransactionStatus.Success) && TrnObj.IsAPITrade == 0)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessReInitMarkOrderAsync", ControllerName, "Process ReInit Mark Order End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : Failed", Helpers.UTC_To_IST()));

                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.TradeRecon_ActionFailedForLocalTrade;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_ActionFailedForLocalTrade;
                    return Response;
                }
                else
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessReInitMarkOrderAsync", ControllerName, "Process ReInit Mark Order End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : Failed", Helpers.UTC_To_IST()));

                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.TradeRecon_ActionFailed;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_ActionFailed;
                    return Response;
                }

UpdateTradeEntry:
                if (TrnObj.Status == Convert.ToInt16(enTransactionStatus.OperatorFail))
                    TrnObj.IsCancelled = 0;

                TrnObj.Status = Convert.ToInt16(enTransactionStatus.Hold);
                UpdateTransactionSettledEntry(TrnObj, true);// update or create Settle entry
                UpdateBuyerSellerListEntry(TrnObj);  // update buyer seller list status                

                Task.WaitAll();

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessReInitMarkOrderAsync", ControllerName, "Process ReInit Mark Order End 1" + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : SUCCESS" , Helpers.UTC_To_IST()));

                Response.ReturnCode = enResponseCode.Success;
                Response.ErrorCode = enErrorCode.TradeRecon_RequestInProcess;
                Response.ReturnMsg = EnResponseMessage.TradeRecon_RequestInProcess;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> ProcessReleaseStuckOrderOrderAsync(BizResponseClass Response, TradeTransactionQueue TrnObj, long UserID, short IsFromCancellation = 0)
        {
           
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessReleaseStuckOrderOrderAsync", ControllerName, "Process Release InProcess MarkOrder start " + "##TrnNo:" + TrnObj.TrnNo, Helpers.UTC_To_IST()));

                if (TrnObj.Status != Convert.ToInt16(enTransactionStatus.Hold))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidTransactionStatus;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionStatus;
                    return Response;
                }

                if (CheckBuyerSellerListIsProcessing(TrnObj) != 1)
                {
                    //Response.ReturnCode = enResponseCode.Fail;
                    //Response.ErrorCode = enErrorCode.TransactionNotInIsProcessing;
                    //Response.ReturnMsg = EnResponseMessage.TransactionNotInIsProcessing;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.TradeRecon_RequestReleaseStuckOrder;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_RequestReleaseStuckOrder;
                    return Response;
                }

                bool IsCallRevertTxnSP = true;
                decimal SettlementQty = 0;
                decimal BaseCurrQty = 0;
                // Reverse order  operation -  call sp  'Sp_TradeSettlement'
                //rita 26-6-19 append tolist() , as iEnumerable execute query on evry loop and inner side same object update statement gives error
                //There is already an open DataReader associated with this Command which must be closed first
                var TradePoolData =  _tradePoolQueueV1.FindBy(e => e.TakerTrnNo == TrnObj.TrnNo && e.Status == 0 && e.CreatedDate < Helpers.UTC_To_IST().AddMinutes(-10)).ToList();//rita 6-5-19 take only 10 minutes old
                if (TradePoolData == null)
                {
                    goto UpdateBuyerSellerListEntry;                    
                }
               

                foreach (var TradePoolResult in TradePoolData)
                {
                    IsCallRevertTxnSP = true;
                    
                    //Rita 25-04-19 check txn proceed at wallet side or not
                    if (_WalletService.CheckSettlementProceed(TradePoolResult.MakerTrnNo, TradePoolResult.TakerTrnNo))
                    {
                        //update TQ and buyer/seller list
                        //does not call release sp , only status update and release order                        
                        IsCallRevertTxnSP = false;
                    }
                    SettlementQty = TradePoolResult.TakerQty;
                    BaseCurrQty = Helpers.DoRoundForTrading(TradePoolResult.TakerQty * TradePoolResult.TakerPrice, 18);
                    short ActionType = TrnObj.TrnType == (short)enTrnType.Buy_Trade ? (short)enSP_TradeSettlementActionType.Buy : (short)enSP_TradeSettlementActionType.Sell;
                    bool CallSpResult;
                    if (IsCallRevertTxnSP)
                        CallSpResult = await _settlementRepositoryV1.Callsp_TradeSettlement(TrnObj.TrnNo, TradePoolResult.MakerTrnNo, TradePoolResult.TakerQty, BaseCurrQty, TradePoolResult.TakerPrice, ActionType, 2, UpdatedBy: UserID);
                    else
                    {
                        CallSpResult = true;//direct assign for release stuck order
                        TradePoolResult.Status = Convert.ToInt16(enOrderStatus.Success); // khushali 21-06-2019 Record updated if  Wallet side settlement done  as per disccussion with Rita mam
                        _tradePoolQueueV1.UpdateField(TradePoolResult, e => e.Status); // khushali 21-06-2019
                    }                       

                    enTransactionStatus WalletTransactionStatus = _WalletService.CheckTransactionSuccessOrNot(TradePoolResult.MakerTrnNo);
                    if(WalletTransactionStatus == enTransactionStatus.Success)
                    {
                        var MakeTradeTransaction = _TradeTransactionRepository.GetSingle(e => e.TrnNo == TradePoolResult.MakerTrnNo);
                        MakeTradeTransaction.Status = 1;
                        _TradeTransactionRepository.Update(MakeTradeTransaction);
                        await UpdateBuyerSellerListEntry(MakeTradeTransaction);
                        await UpdateTransactionSettledEntry(MakeTradeTransaction);
                    }

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessReleaseStuckOrderOrderAsync", ControllerName, "Process  Release Stuck Order End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE###" + CallSpResult.ToString(), Helpers.UTC_To_IST()));

                    if (CallSpResult)
                    {
                        goto UpdateBuyerSellerListEntry;
                    }
                    else
                    {                        
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.TradeRecon_ActionFailed;
                        Response.ReturnMsg = EnResponseMessage.TradeRecon_ActionFailed;
                        return Response;
                    }
                }                

UpdateBuyerSellerListEntry:

                await UpdateBuyerSellerListEntry(TrnObj, true, IsCallRevertTxnSP, SettlementQty,BaseCurrQty, IsFromCancellation);  // update buyer seller list status
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessReleaseStuckOrderOrderAsync", ControllerName, "Process Release StuckOrder Order End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : Success", Helpers.UTC_To_IST()));

                Response.ReturnCode = enResponseCode.Success;
                Response.ErrorCode = enErrorCode.TradeRecon_RequestReleaseStuckOrder;
                Response.ReturnMsg = EnResponseMessage.TradeRecon_RequestReleaseStuckOrder;
                return Response;
                
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> ProcessSuccessMarkOrderAsync(BizResponseClass Response, TradeTransactionQueue TrnObj, string AccessToken)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessSuccessMarkOrderAsync", ControllerName, "Process Success MarkOrder start " + "##TrnNo:" + TrnObj.TrnNo, Helpers.UTC_To_IST()));

                if (!(TrnObj.Status == Convert.ToInt16(enTransactionStatus.Hold) || TrnObj.Status == Convert.ToInt16(enTransactionStatus.OperatorFail)))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidTransactionStatus;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionStatus;
                    return Response;
                }

                if (TrnObj.Status == Convert.ToInt16(enTransactionStatus.OperatorFail))
                    TrnObj.IsCancelled = 0;

                TrnObj.Status = Convert.ToInt16(enTransactionStatus.Success);
                UpdateTransactionSettledEntry(TrnObj);// update or create Settle entry
                UpdateBuyerSellerListEntry(TrnObj);  // update buyer seller list status                

                Task.WaitAll();

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessSuccessMarkOrderAsync", ControllerName, "Process Success MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo+ "###RESPONSE### : SUCCESS", Helpers.UTC_To_IST()));

                Response.ReturnCode = enResponseCode.Success;
                Response.ErrorCode = enErrorCode.TradeRecon_RequestSuccess;
                Response.ReturnMsg = EnResponseMessage.TradeRecon_RequestSuccess;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> ProcessForceCancelOrderAsync(BizResponseClass Response, TradeTransactionQueue TrnObj, string AccessToken)
        {
            int IsProcessing = 0;
            DateTime UpdatedDate = DateTime.UtcNow.AddMinutes(5);
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessForceCancelOrderAsync", ControllerName, "Process ForceCancel MarkOrder start " + "##TrnNo:" + TrnObj.TrnNo, Helpers.UTC_To_IST()));

                if (TrnObj.Status != Convert.ToInt16(enTransactionStatus.Hold))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidTransactionStatus;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionStatus;
                    return Response;
                }

                if (TrnObj.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                {
                    BuyerList = _TradeBuyerList.FindBy(e => e.TrnNo == TrnObj.TrnNo).SingleOrDefault();
                    IsProcessing = BuyerList.IsProcessing;
                    UpdatedDate = BuyerList.UpdatedDate.Value;
                }
                else if (TrnObj.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
                {
                    SellerList = _TradeSellerList.FindBy(e => e.TrnNo == TrnObj.TrnNo).SingleOrDefault();
                    IsProcessing = SellerList.IsProcessing;
                    UpdatedDate = SellerList.UpdatedDate.Value;
                }
                
                if (IsProcessing == 1 && UpdatedDate.AddMinutes(5) < DateTime.UtcNow)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessForceCancelOrderAsync", ControllerName, "Process ForceCancel MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : FAIL", Helpers.UTC_To_IST()));
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.TradeRecon_After5MinTranDontTakeAction;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_After5MinTranDontTakeAction;
                    return Response;
                }

                var cancelOrderRequest = new NewCancelOrderRequestClsV2()
                {
                    TranNo = TrnObj.TrnNo,
                    accessToken = AccessToken
                };
                //var response = await _cancelOrderProcess.ProcessCancelOrderAsyncV1(cancelOrderRequest);
                var response = await _mediator.Send(cancelOrderRequest);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessForceCancelOrderAsync", ControllerName, "Process ForceCancel MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE###" + response.ReturnMsg, Helpers.UTC_To_IST()));

                Response.ReturnCode = (enResponseCode)enResponseCodeService.Parse(typeof(enResponseCodeService), response.ReturnCode.ToString());
                Response.ErrorCode = response.ErrorCode;
                Response.ReturnMsg = response.ReturnMsg;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> ProcessSuccessAndDebitOrderAsync(BizResponseClass Response, TradeTransactionQueue TrnObj, string AccessToken)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessSuccessAndDebitOrderAsync", ControllerName, "Process Success and Debit MarkOrder start " + "##TrnNo:" + TrnObj.TrnNo, Helpers.UTC_To_IST()));

                if (TrnObj.Status != Convert.ToInt16(enTransactionStatus.OperatorFail))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidTransactionStatus;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionStatus;
                    return Response;
                }
                                
                // Wallet operation - direct Debit wallet and deduct charge
                var DebitResult = await _WalletService.GetDebitWalletWithCharge(TrnObj.Order_Currency, Helpers.GetTimeStamp(), TrnObj.OrderTotalQty - TrnObj.SettledSellQty,
                    TrnObj.OrderAccountID, TrnObj.TrnNo, enServiceType.Trading, TrnObj.TrnType == (short)enTrnType.Buy_Trade ? enWalletTrnType.BuyTrade : enWalletTrnType.SellTrade, (enTrnType)(TrnObj.TrnType), EnAllowedChannels.Web, AccessToken, (enWalletDeductionType)((short)TrnObj.ordertype));

                if(DebitResult.ReturnCode == enResponseCode.Success)
                {
                    if (TrnObj.Status == Convert.ToInt16(enTransactionStatus.OperatorFail))
                        TrnObj.IsCancelled = 0;

                    TrnObj.Status = Convert.ToInt16(enTransactionStatus.Success);
                    UpdateTransactionSettledEntry(TrnObj); // update or create Settle entry 
                    UpdateBuyerSellerListEntry(TrnObj);  // update buyer seller list status                    

                    Task.WaitAll();

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessSuccessAndDebitOrderAsync", ControllerName, "Process Success and Debit MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE###" + DebitResult.ReturnMsg, Helpers.UTC_To_IST()));

                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.TradeRecon_RequestSuccessAndDebit;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_RequestSuccessAndDebit;
                    return Response;

                }
                else if(DebitResult.ReturnCode == enResponseCode.Fail && DebitResult.ErrorCode == enErrorCode.sp_InsufficientBalanceForCharge)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessSuccessAndDebitOrderAsync", ControllerName, "Process Success and Debit MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : FAIL", Helpers.UTC_To_IST()));

                    Response.ReturnCode = DebitResult.ReturnCode;
                    Response.ErrorCode = enErrorCode.ReconInsufficientBalanceForCharge;
                    Response.ReturnMsg = DebitResult.ReturnMsg;
                    return Response;
                }
                else if (DebitResult.ReturnCode == enResponseCode.Fail && DebitResult.ErrorCode == enErrorCode.sp_InsufficientBalance)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessSuccessAndDebitOrderAsync", ControllerName, "Process Success and Debit MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : FAIL", Helpers.UTC_To_IST()));

                    Response.ReturnCode = DebitResult.ReturnCode;
                    Response.ErrorCode = enErrorCode.ReconInsufficientBalance;
                    Response.ReturnMsg = DebitResult.ReturnMsg;
                    return Response;
                }
                else if(DebitResult.ReturnCode == enResponseCode.Fail &&  DebitResult.ErrorCode == enErrorCode.AlredyExist)
                {
                    if (TrnObj.Status == Convert.ToInt16(enTransactionStatus.OperatorFail))
                        TrnObj.IsCancelled = 0;

                    TrnObj.Status = Convert.ToInt16(enTransactionStatus.Success);
                    UpdateTransactionSettledEntry(TrnObj);// update or create Settle entry
                    UpdateBuyerSellerListEntry(TrnObj);  // update buyer seller list status                    

                    Task.WaitAll();

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessSuccessAndDebitOrderAsync", ControllerName, "Process Success and Debit MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : SUCCESS", Helpers.UTC_To_IST()));
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.TradeRecon_RequestSuccessAndDebit;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_RequestSuccessAndDebit;
                    return Response;
                }

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessSuccessAndDebitOrderAsync", ControllerName, "Process Success and Debit MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : " + DebitResult.ReturnMsg , Helpers.UTC_To_IST()));

                Response.ReturnCode = enResponseCode.Fail;
                Response.ErrorCode = DebitResult.ErrorCode;
                Response.ReturnMsg = EnResponseMessage.TradeRecon_ActionFailed;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> ProcessInactiveMarkOrderAsync(BizResponseClass Response, TradeTransactionQueue TrnObj, string AccessToken)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessInactiveMarkOrderAsync", ControllerName, "Process InActive MarkOrder start " + "##TrnNo:" + TrnObj.TrnNo, Helpers.UTC_To_IST()));

                if (TrnObj.Status != Convert.ToInt16(enTransactionStatus.Hold))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidTransactionStatus;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionStatus;
                    return Response;
                }
                TrnObj.Status = Convert.ToInt16(enTransactionStatus.InActive);
                UpdateTransactionSettledEntry(TrnObj);// update or create Settle entry
                UpdateBuyerSellerListEntry(TrnObj);  // update buyer seller list status
                Task.WaitAll();

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessInactiveMarkOrderAsync", ControllerName, "Process InActive MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : SUCCESS", Helpers.UTC_To_IST()));

                Response.ReturnCode = enResponseCode.Success;
                Response.ErrorCode = enErrorCode.TradeRecon_RequestInActive;
                Response.ReturnMsg = EnResponseMessage.TradeRecon_RequestInActive;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> ProcessActiveMarkOrderAsync(BizResponseClass Response, TradeTransactionQueue TrnObj, string AccessToken)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessActiveMarkOrderAsync", ControllerName, "Process Active MarkOrder start " + "##TrnNo:" + TrnObj.TrnNo, Helpers.UTC_To_IST()));

                if (TrnObj.Status != Convert.ToInt16(enTransactionStatus.InActive))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidTransactionStatus;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionStatus;
                    return Response;
                }

                //komal 03-07-2019 chechk settled qty
                Decimal SettledQty = _WalletService.GetTransactionSettledQty(TrnObj.TrnNo);
                

                if(SettledQty != TrnObj.SettledSellQty)
                        TrnObj.SettledSellQty = SettledQty;

                TrnObj.Status = Convert.ToInt16(enTransactionStatus.Hold);
                UpdateTransactionSettledEntry(TrnObj);// update or create Settle entry
                UpdateBuyerSellerListEntry(TrnObj);  // update buyer seller list status
                Task.WaitAll();

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessActiveMarkOrderAsync", ControllerName, "Process Active MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : SUCCESS", Helpers.UTC_To_IST()));

                Response.ReturnCode = enResponseCode.Success;
                Response.ErrorCode = enErrorCode.TradeRecon_RequestActive;
                Response.ReturnMsg = EnResponseMessage.TradeRecon_RequestActive;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        // not used  commented by khushali for combining  inproccess mark and Reinit mark case 
        //public async Task<BizResponseClass> ProcessReInitMarkOrderAsync(BizResponseClass Response, TradeTransactionQueue TrnObj, string AccessToken)
        //{
        //    try
        //    {
        //        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessReInitMarkOrderAsync", ControllerName, "Process ReInit MarkOrder start " + "##TrnNo:" + TrnObj.TrnNo, Helpers.UTC_To_IST()));

        //        if (TrnObj.Status != Convert.ToInt16(enTransactionStatus.SystemFail))
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ErrorCode = enErrorCode.InvalidTransactionStatus;
        //            Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionStatus;
        //            return Response;
        //        }

        //        TrnObj.Status = Convert.ToInt16(enTransactionStatus.Hold);

        //        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessReInitMarkOrderAsync", ControllerName, "Process ReInit MarkOrder End " + "##TrnNo:" + TrnObj.TrnNo + "###RESPONSE### : SUCCESS", Helpers.UTC_To_IST()));

        //        Response.ReturnCode = enResponseCode.Success;
        //        Response.ErrorCode = enErrorCode.TradeRecon_RequestReInit;
        //        Response.ReturnMsg = EnResponseMessage.TradeRecon_RequestReInit;
        //        return Response;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        #endregion

        #region "Entry Updation"

        public Task<SettledTradeTransactionQueue> MakeTransactionSettledEntry(TradeTransactionQueue TradeTransactionQueueObj, short IsCancelled = 0)
        {
            decimal BidPrice = TradeTransactionQueueObj.BidPrice;
            decimal AskPrice = TradeTransactionQueueObj.AskPrice;

            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("MakeTransactionSettledEntry", ControllerName, "Make Transaction Settled Entry Start " + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));

                SettledTradeTransactionQueue SettledTradeTQObj = new SettledTradeTransactionQueue()
                {
                    CreatedDate = Helpers.UTC_To_IST(),//rita 29-1-19 added for log
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
                    Status = 1,// TradeTransactionQueueObj.Status,//rita 3-1-19 every time only success save
                    StatusCode = TradeTransactionQueueObj.StatusCode,
                    StatusMsg = TradeTransactionQueueObj.StatusMsg,
                    IsCancelled = IsCancelled
                };
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("MakeTransactionSettledEntry", ControllerName, "Make Transaction Settled Entry End " + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));

                return Task.FromResult(SettledTradeTQObj);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MakeTransactionSettledEntry:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
                return null;
            }
        }

        public async Task UpdateTransactionSettledEntry(TradeTransactionQueue TradeTransactionQueueObj,bool IsReinit = false)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("UpdateTransactionSettledEntry", ControllerName, "Update Transaction Settled Entry Start " + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));

                settledTradeObj = _settledTradeTransactionRepository.FindBy(e => e.TrnNo == TradeTransactionQueueObj.TrnNo).SingleOrDefault();
                if (!IsReinit)
                {
                    if (settledTradeObj == null)
                    {
                        settledTradeObj = MakeTransactionSettledEntry(TradeTransactionQueueObj).Result;
                        _settledTradeTransactionRepository.Add(settledTradeObj);
                    }
                    settledTradeObj.Status = TradeTransactionQueueObj.Status;
                    _settledTradeTransactionRepository.Update(settledTradeObj);
                }
                if(settledTradeObj != null && IsReinit)
                {
                    settledTradeObj.Status = Convert.ToInt16(enTransactionStatus.InActive);
                    _settledTradeTransactionRepository.Update(settledTradeObj);
                }                

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("UpdateTransactionSettledEntry", ControllerName, "Update Transaction Settled Entry End " + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MakeTransactionSettledEntry:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
            }
        }

        public async Task UpdateBuyerSellerListEntry(TradeTransactionQueue TradeTransactionQueueObj,bool IsProcessing = false,bool IsCallRevertTxnSP=true,decimal SettlementQty=0,decimal BaseCurrQty=0,short IsFromCancellation = 0)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("UpdateBuyerSellerListEntry", ControllerName, "Update BuyerSellerList Entry Start " + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));

                if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                {
                    BuyerList = _TradeBuyerList.FindBy(e => e.TrnNo == TradeTransactionQueueObj.TrnNo).SingleOrDefault();
                    if(BuyerList == null)
                    {
                       await InsertBuyerList(TradeTransactionQueueObj);
                    }
                    BuyerList.Status = TradeTransactionQueueObj.Status;
                    //Rita 26-4-19 update Qty/status is settlement done at wallet side
                    if (IsCallRevertTxnSP == false)//txn not revert so update qty and status of current Isprocessing=1 order
                    {
                        TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + SettlementQty, 18);
                        TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + BaseCurrQty, 18);
                        BuyerList.RemainQty = Helpers.DoRoundForTrading(BuyerList.RemainQty - SettlementQty, 18);
                        BuyerList.DeliveredQty = Helpers.DoRoundForTrading(BuyerList.DeliveredQty + SettlementQty, 18);
                        if (BuyerList.RemainQty == 0)
                        {
                            TradeTransactionQueueObj.Status = 1;
                            BuyerList.Status = 1;
                            //rita 26-5-19 only for sucess case make entry
                            if (IsFromCancellation == 0)//if from cancel service then do not add to this table ,as cancellation may fail
                                await UpdateTransactionSettledEntry(TradeTransactionQueueObj);
                        }
                        _TradeTransactionRepository.Update(TradeTransactionQueueObj);
                        //if (IsFromCancellation == 0)//if from cancel service then do not add to this table ,as cancellation may fail
                        //    await UpdateTransactionSettledEntry(TradeTransactionQueueObj);
                    }

                    if (IsProcessing)
                    {
                        BuyerList.IsProcessing = 0;
                    }
                    _TradeBuyerList.Update(BuyerList);
                }
                else if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
                {
                    SellerList = _TradeSellerList.FindBy(e => e.TrnNo == TradeTransactionQueueObj.TrnNo).SingleOrDefault();
                    if(SellerList == null)
                    {
                        await InsertSellerList(TradeTransactionQueueObj);
                    }
                    SellerList.Status = TradeTransactionQueueObj.Status;
                    //Rita 26-4-19 update Qty/status is settlement done at wallet side
                    if (IsCallRevertTxnSP == false)//txn not revert so update qty and status of current Isprocessing=1 order
                    {
                        TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + SettlementQty, 18);
                        TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + BaseCurrQty, 18);
                        SellerList.RemainQty = Helpers.DoRoundForTrading(SellerList.RemainQty - SettlementQty, 18);
                        SellerList.SelledQty = Helpers.DoRoundForTrading(SellerList.SelledQty + SettlementQty, 18);
                        if(SellerList.RemainQty==0)
                        {
                            TradeTransactionQueueObj.Status = 1;
                            SellerList.Status = 1;
                            //rita 26-5-19 only for sucess case make entry
                            if (IsFromCancellation == 0)//if from cancel service then do not add to this table ,as cancellation may fail
                                await UpdateTransactionSettledEntry(TradeTransactionQueueObj);
                        }
                        _TradeTransactionRepository.Update(TradeTransactionQueueObj);
                        //if (IsFromCancellation == 0)//if from cancel service then do not add to this table ,as cancellation may fail
                        //    await UpdateTransactionSettledEntry(TradeTransactionQueueObj);
                    }
                    if (IsProcessing)
                    {
                        SellerList.IsProcessing = 0;
                    }
                    _TradeSellerList.Update(SellerList);
                }


                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("UpdateBuyerSellerListEntry", ControllerName, "Update BuyerSellerList Entry end " + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateBuyerSellerListEntry:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
            }
        }

        public short CheckBuyerSellerListIsProcessing(TradeTransactionQueue TradeTransactionQueueObj)
        {
            short IsProcessing = 0;
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CheckBuyerSellerListIsProcessing", ControllerName, "check BuyerSellerList Entry Start " + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));

                if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                {
                    BuyerList = _TradeBuyerList.FindBy(e => e.TrnNo == TradeTransactionQueueObj.TrnNo).SingleOrDefault();
                    IsProcessing = BuyerList.IsProcessing;
                }
                else if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
                {
                    SellerList = _TradeSellerList.FindBy(e => e.TrnNo == TradeTransactionQueueObj.TrnNo).SingleOrDefault();
                    IsProcessing =  SellerList.IsProcessing;
                }

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CheckBuyerSellerListIsProcessing", ControllerName, "check BuyerSellerList Entry end " + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));

                return IsProcessing;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CheckBuyerSellerListIsProcessing:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
                return IsProcessing;
            }
        }

        public async Task InsertBuyerList(TradeTransactionQueue TradeTransactionObj)
        {
            try
            {
                TradeBuyerListObj = new TradeBuyerListV1()
                {
                    
                    CreatedBy = TradeTransactionObj.MemberID,
                    TrnNo = TradeTransactionObj.TrnNo,
                    PairID = TradeTransactionObj.PairID,
                    PairName = TradeTransactionObj.PairName,
                    Price = TradeTransactionObj.BidPrice,
                    Qty = TradeTransactionObj.BuyQty, //same as request as one entry per one request
                    DeliveredQty = 0,
                    RemainQty = TradeTransactionObj.BuyQty,
                    IsProcessing = 1,
                    OrderType = Convert.ToInt16(TradeTransactionObj.ordertype),
                    Status = Convert.ToInt16(enTransactionStatus.Initialize),//txn type status
                    IsAPITrade = 0,//Rita 30-1-19 for API level changes in settlement , do not pick in local settlement
                };
                if (TradeTransactionObj.ordertype == (short)enTransactionMarketType.STOP_Limit)
                {
                    TradeBuyerListObj.IsProcessing = 0;
                }
                TradeBuyerListObj = _TradeBuyerList.Add(TradeBuyerListObj);
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("InsertBuyerList:##TrnNo " + TradeTransactionObj.TrnNo, ControllerName, ex));
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
            }

        }

        public async Task InsertSellerList(TradeTransactionQueue TradeTransactionObj)
        {
            try
            {
                TradeSellerListObj = new TradeSellerListV1()
                {
                    CreatedBy = TradeTransactionObj.MemberID,
                    TrnNo = TradeTransactionObj.TrnNo,
                    PairID = TradeTransactionObj.PairID,
                    PairName = TradeTransactionObj.PairName,
                    Price = TradeTransactionObj.AskPrice,
                    Qty = TradeTransactionObj.SellQty,
                    ReleasedQty = TradeTransactionObj.SellQty,
                    SelledQty = 0,
                    RemainQty = TradeTransactionObj.SellQty,
                    IsProcessing = 1,
                    OrderType = Convert.ToInt16(TradeTransactionObj.ordertype),
                    Status = Convert.ToInt16(enTransactionStatus.Initialize),//txn type status
                    IsAPITrade = 0,//Rita 30-1-19 for API level changes in settlement , do not pick in local settlement
                };
                if (TradeTransactionObj.ordertype == (short)enTransactionMarketType.STOP_Limit)
                {
                    TradeSellerListObj.IsProcessing = 0;
                }
                TradeSellerListObj =_TradeSellerList.Add(TradeSellerListObj);
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("Tradepoolmaster:##TrnNo " + TradeTransactionObj.TrnNo, ControllerName, ex));
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
            }
        }
        #endregion

        #region "Trade Recon Action stage"

        public async Task<BizResponseClass> TradeReconProcessAsyncV1(enTradeReconActionType ActionType, long TrnNo, string ActionMessage, long UserId, string AccessToken)
        {
            BizResponseClass _Resp = new BizResponseClass();
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("TradeReconProcessAsyncV1", ControllerName, "Trade Recon Process Start " + "##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));

                var TrnObj = _TradeTransactionRepository.FindBy(e => e.TrnNo == TrnNo).SingleOrDefault();
                var OldStatus = TrnObj.Status;
                switch (ActionType)
                {
                    case enTradeReconActionType.CancelMark: // done
                        _Resp = await ProcessCancelMarkOrderAsync(_Resp, TrnObj, AccessToken);
                        break;
                    case enTradeReconActionType.FailMark: // done
                        //4 --> 2 
                        //1 --> 2 settlement table entry status update
                        _Resp = await ProcessFailMarkOrderAsync(_Resp,TrnObj, AccessToken);
                        _TradeTransactionRepository.Update(TrnObj);
                        break;
                    //case enTradeReconActionType.InProccessMark: // wallet
                    //    //1--> 4
                    //    // 2 --> 4
                    //    // wallet hold for both case                        
                    //    _Resp = await ProcessInProccessMarkOrderAsync(_Resp, TrnObj, AccessToken);
                    //    break;
                    case enTradeReconActionType.SuccessMark: // done
                        //2 --> 1
                        //4--> 1
                        //settlement table entry insert or status = 1
                        _Resp = await ProcessSuccessMarkOrderAsync(_Resp, TrnObj, AccessToken);
                        _TradeTransactionRepository.Update(TrnObj);
                        break;
                    case enTradeReconActionType.ForceCancel:
                        // isprocessing = 1 , updated date < 5 min                        
                        _Resp = await ProcessCancelMarkOrderAsync(_Resp,TrnObj, AccessToken);
                        break;
                    case enTradeReconActionType.SuccessAndDebit: // wallet
                        //2 --> 1 // debit wallet
                        //settlement table entry insert or status = 1
                        // wallet debited or not ??                        
                        _Resp = await ProcessSuccessAndDebitOrderAsync(_Resp,TrnObj, AccessToken);
                        _TradeTransactionRepository.Update(TrnObj);
                        break;
                    case enTradeReconActionType.InactiveMark: // done
                        // 4 --> 9
                        _Resp = await ProcessInactiveMarkOrderAsync(_Resp,TrnObj, AccessToken);
                        _TradeTransactionRepository.Update(TrnObj);
                        break;
                    case enTradeReconActionType.ActiveMark: // done
                        // 9 --> 4
                        _Resp = await ProcessActiveMarkOrderAsync(_Resp,TrnObj, AccessToken);
                        _TradeTransactionRepository.Update(TrnObj);
                        break;
                    case enTradeReconActionType.ReleaseStuckOrder:
                        // 4 -> 4 , isproccessing bit = 1
                        _Resp = await ProcessReleaseStuckOrderOrderAsync(_Resp, TrnObj, UserId);
                        _TradeTransactionRepository.Update(TrnObj);
                        break;
                    case enTradeReconActionType.ReInitMark:
                        // 3 -> 4
                        // 1--> 4
                        //  2 --> 4
                        _Resp = await ProcessReInitMarkOrderAsync(_Resp, TrnObj, AccessToken);
                        _TradeTransactionRepository.Update(TrnObj);
                        break;
                }
                
                if (_Resp.ReturnCode == enResponseCode.Success)
                {
                    var NewStatus = TrnObj.Status;
                    TradingRecon tradingRecon = new TradingRecon
                    {
                        CreatedBy = UserId,
                        CreatedDate = DateTime.UtcNow,
                        Remarks = ActionMessage,
                        TrnNo = TrnNo,
                        Status = 1,
                        NewStatus = NewStatus,
                        OldStatus = OldStatus,
                        StatusCode = (long)_Resp.ErrorCode,
                        StatusMsg = _Resp.ReturnMsg
                    };
                    _tradingRecon.Add(tradingRecon);
                }

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("TradeReconProcessAsyncV1", ControllerName, "Trade Recon Process End " + "##TrnNo:" + TrnNo + "Response : " + JsonConvert.SerializeObject(_Resp), Helpers.UTC_To_IST()));

                return _Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        #endregion

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
