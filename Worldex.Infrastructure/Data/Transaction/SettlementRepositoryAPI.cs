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
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Data.Transaction
{
    public class SettlementRepositoryAPI: ISettlementRepositoryAPI<BizResponse>
    {
        private readonly WorldexContext _dbContext;
        private readonly ICommonRepository<TradeBuyerListV1> _TradeBuyerList;
        private readonly ICommonRepository<TradeSellerListV1> _TradeSellerList;
        //private readonly ICommonRepository<TradePairStastics> _tradePairStastics;//komal 03 May 2019, Cleanup
        private readonly ISignalRService _ISignalRService;
        private readonly IFrontTrnService _IFrontTrnService;
        private readonly UserManager<ApplicationUser> _userManager;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly ILPWalletTransaction _WalletService;
        string ControllerName = "SettlementRepositoryAPI";
        TradePoolQueueV1 TradePoolQueueObj;
        APIOrderSettlement APIOrderSettlementObj;
        private readonly IMessageService _messageService;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        decimal sAPIRemainQty = 0;
        decimal sAPISettledQty = 0;
        decimal sAPIPrice = 0;
        WalletDrCrResponse CreditWalletResult;

        public SettlementRepositoryAPI(WorldexContext dbContext, ILPWalletTransaction WalletService, ISignalRService ISignalRService,
             IFrontTrnService IFrontTrnService, UserManager<ApplicationUser> userManager, ICommonRepository<TradeBuyerListV1> TradeBuyerList,
            ICommonRepository<TradeSellerListV1> TradeSellerList, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue,
            IMessageService messageService, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue)
        {
            _dbContext = dbContext;
            _TradeBuyerList = TradeBuyerList;
            _TradeSellerList = TradeSellerList;
            _WalletService = WalletService;
            _ISignalRService = ISignalRService;
            _IFrontTrnService = IFrontTrnService;
            _userManager = userManager;
            _pushNotificationsQueue = pushNotificationsQueue;
            _messageService = messageService;
            _pushSMSQueue = pushSMSQueue;
        }

        #region ==============================API PROCESS SETLLEMENT========================      
        public async Task<BizResponse> PROCESSSETLLEMENTAPI(BizResponse _Resp, long TrnNo,decimal APIRemainQty,decimal APISettledQty, decimal APIPrice)
        {
            try
            {
                sAPIRemainQty = Helpers.DoRoundForTrading(APIRemainQty, 18);//Rita 20-8-19 for only taken 18 digits
                sAPISettledQty = Helpers.DoRoundForTrading(APISettledQty, 18);

                sAPIPrice = APIPrice;
                TransactionQueue TransactionQueueObj = _dbContext.Set<TransactionQueue>().FirstOrDefault(item => item.Id == TrnNo);
                TradeTransactionQueue TradeTranQueueObj = _dbContext.Set<TradeTransactionQueue>().FirstOrDefault(item => item.TrnNo == TrnNo);
                TradeStopLoss TradeStopLossObj = _dbContext.Set<TradeStopLoss>().FirstOrDefault(item => item.TrnNo == TrnNo);

                if (TradeTranQueueObj.Status!=4)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPI", ControllerName, " Status not in pending state:" + TradeTranQueueObj.Status + "##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));
                    _Resp.ErrorCode = enErrorCode.API_OrderNotPending;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Status not in pending state";
                    return _Resp;
                }
                if (TradeTranQueueObj.IsCancelled == 1 || TradeTranQueueObj.IsAPICancelled ==1)
                {
                    _Resp.ErrorCode = enErrorCode.CancelOrder_OrderalreadyCancelled;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Cancellation request is already in processing";
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPI", ControllerName, _Resp.ReturnMsg + " ##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));
                    return _Resp;
                }
                if (TradeTranQueueObj.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                {
                    TradeBuyerListV1 TradeBuyerListObj = _dbContext.Set<TradeBuyerListV1>().FirstOrDefault(item => item.TrnNo == TrnNo);                    
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPI BUYER", ControllerName,"##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));
                    _Resp = await PROCESSSETLLEMENTAPIBuy(_Resp, TradeTranQueueObj, TransactionQueueObj, TradeStopLossObj, TradeBuyerListObj, "");
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPIBuy", ControllerName," ErrorCode:" + _Resp.ErrorCode + " Msg:" + _Resp.ReturnMsg + " ##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));
                }
                else
                {
                    TradeSellerListV1 TradeSellerListObj = _dbContext.Set<TradeSellerListV1>().FirstOrDefault(item => item.TrnNo == TrnNo);
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPI SELLER", ControllerName,"##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));
                    _Resp = await PROCESSSETLLEMENTAPISell(_Resp, TradeTranQueueObj, TransactionQueueObj, TradeStopLossObj, TradeSellerListObj, "");
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPISell", ControllerName, " ErrorCode:" + _Resp.ErrorCode + " Msg:" + _Resp.ReturnMsg + " ##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));
                }
                return _Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PROCESSSETLLEMENTAPI:##TrnNo " + TrnNo, ControllerName, ex);
                return _Resp;
            }            
        }
        //Rita 29-05-19 declare new method which call from NewTransactionV1 class , with object pass, bcz new declaration here gives error
        //The instance of entity type 'TradeSellerListV1' cannot be tracked because another instance with the same key value for {'TrnNo'} is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached
        public async Task<BizResponse> PROCESSSETLLEMENTAPIFromInit(BizResponse _Resp, long TrnNo, decimal APIRemainQty, decimal APISettledQty, decimal APIPrice, TransactionQueue TransactionQueueObj, TradeTransactionQueue TradeTranQueueObj, TradeStopLoss TradeStopLossObj, TradeBuyerListV1 TradeBuyerListObj, TradeSellerListV1 TradeSellerListObj)
        {
            try
            {
                sAPIRemainQty = Helpers.DoRoundForTrading(APIRemainQty, 18);//Rita 20-8-19 for only taken 18 digits
                sAPISettledQty = Helpers.DoRoundForTrading(APISettledQty, 18);

                sAPIPrice = APIPrice;
                //TransactionQueue TransactionQueueObj = _dbContext.Set<TransactionQueue>().FirstOrDefault(item => item.Id == TrnNo);
                //TradeTransactionQueue TradeTranQueueObj = _dbContext.Set<TradeTransactionQueue>().FirstOrDefault(item => item.TrnNo == TrnNo);
                //TradeStopLoss TradeStopLossObj = _dbContext.Set<TradeStopLoss>().FirstOrDefault(item => item.TrnNo == TrnNo);

                if (TradeTranQueueObj.Status != 4)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPI", ControllerName, " Status not in pending state:" + TradeTranQueueObj.Status + "##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));
                    _Resp.ErrorCode = enErrorCode.API_OrderNotPending;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Status not in pending state";
                    return _Resp;
                }
                if (TradeTranQueueObj.IsCancelled == 1 || TradeTranQueueObj.IsAPICancelled == 1)
                {
                    _Resp.ErrorCode = enErrorCode.CancelOrder_OrderalreadyCancelled;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Transaction Cancellation request is already in processing";
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPI", ControllerName, _Resp.ReturnMsg + " ##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));
                    return _Resp;
                }
                if (TradeTranQueueObj.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                {
                    //TradeBuyerListV1 TradeBuyerListObj = _dbContext.Set<TradeBuyerListV1>().FirstOrDefault(item => item.TrnNo == TrnNo);
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPI BUYER", ControllerName, "##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));
                    _Resp = await PROCESSSETLLEMENTAPIBuy(_Resp, TradeTranQueueObj, TransactionQueueObj, TradeStopLossObj, TradeBuyerListObj, "");
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPIBuy", ControllerName, " ErrorCode:" + _Resp.ErrorCode + " Msg:" + _Resp.ReturnMsg + " ##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));
                }
                else
                {
                    //TradeSellerListV1 TradeSellerListObj = _dbContext.Set<TradeSellerListV1>().FirstOrDefault(item => item.TrnNo == TrnNo);
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPI SELLER", ControllerName, "##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));
                    _Resp = await PROCESSSETLLEMENTAPISell(_Resp, TradeTranQueueObj, TransactionQueueObj, TradeStopLossObj, TradeSellerListObj, "");
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPISell", ControllerName, " ErrorCode:" + _Resp.ErrorCode + " Msg:" + _Resp.ReturnMsg + " ##TrnNo:" + TrnNo, Helpers.UTC_To_IST()));
                }
                return _Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PROCESSSETLLEMENTAPIFromInit:##TrnNo " + TrnNo, ControllerName, ex);
                return _Resp;
            }
        }

        public async Task<BizResponse> PROCESSSETLLEMENTAPIBuy(BizResponse _Resp, TradeTransactionQueue TradeTransactionQueueObj,TransactionQueue TransactionQueueObj, TradeStopLoss TradeStopLossObj, TradeBuyerListV1 CurrentTradeBuyerListObj,string accessToken = "")
        {           
            //short TrackBit=0;
            //short TrackDebitBit = 0;
            decimal GSettlementQty = 0;
            decimal GBaseCurrQty = 0;
            try
            {
                if (CurrentTradeBuyerListObj.IsProcessing == 1)
                {
                    _Resp.ErrorCode = enErrorCode.API_IsProcessing;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Is Under Processing";
                    return _Resp;
                }
                if (CurrentTradeBuyerListObj.RemainQty == 0)
                {
                    _Resp.ErrorCode = enErrorCode.Settlement_AlreadySettled;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "ALready Settled";
                    return _Resp;
                }
                if (CurrentTradeBuyerListObj.IsAPITrade != 1 || TradeTransactionQueueObj.IsAPITrade != 1)
                {
                    _Resp.ErrorCode = enErrorCode.API_NotAPIOrder;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Not API order";
                    return _Resp;
                }
                if (CurrentTradeBuyerListObj.Status != 4 && CurrentTradeBuyerListObj.Status != 0)
                {
                    _Resp.ErrorCode = enErrorCode.API_BuyerOrderNotPending;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Buyer Order Status not in pending state";
                    return _Resp;
                }
                if (CurrentTradeBuyerListObj.RemainQty == sAPIRemainQty)
                {
                    _Resp.ErrorCode = enErrorCode.API_NoRemainQtyChange;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "No Settlement Change in Remain Qty";
                    return _Resp;
                }
                if (CurrentTradeBuyerListObj.DeliveredQty == sAPISettledQty)
                {
                    _Resp.ErrorCode = enErrorCode.API_NoSettledQtyChange;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "No Settlement Change in Settled Qty";
                    return _Resp;
                }
                decimal SettlementQty1 = 0;
                decimal SettlementPrice1 = sAPIPrice;
                decimal BaseCurrQty1 = 0;

                if (SettlementPrice1 == 0)
                    SettlementPrice1 = CurrentTradeBuyerListObj.Price;

                SettlementQty1 = CurrentTradeBuyerListObj.RemainQty - sAPIRemainQty;

                if (SettlementQty1<=0)
                {
                    _Resp.ErrorCode = enErrorCode.API_InvalidSettlementQty;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Invalid Remain Qty gives wrong calculation";
                    return _Resp;
                }
                if (SettlementQty1 != sAPISettledQty - CurrentTradeBuyerListObj.DeliveredQty)
                {
                    //"filled":"0.0038669999999999998","remaining":"0.0000000000000000004336808690" issue , total : 0.003867000000000000
                    if (SettlementQty1 < sAPISettledQty - CurrentTradeBuyerListObj.DeliveredQty)//local < API
                    {
                        _Resp.ErrorCode = enErrorCode.API_InvalidSettledRemainQty;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "Settled and Remail Qty Invalid";
                        return _Resp;
                    }
                    else //API < local , make full settlement
                    {
                        //SettlementQty1 = sAPISettledQty - CurrentTradeBuyerListObj.DeliveredQty;
                    }
                }
                //if (SettlementQty1 != sAPISettledQty - CurrentTradeBuyerListObj.DeliveredQty)
                //{
                //    _Resp.ErrorCode = enErrorCode.API_InvalidSettledRemainQty;
                //    _Resp.ReturnCode = enResponseCodeService.Fail;
                //    _Resp.ReturnMsg = "Settled and Remail Qty Invalid";
                //    return _Resp;
                //}

                GSettlementQty = SettlementQty1;
                BaseCurrQty1 = Helpers.DoRoundForTrading(SettlementQty1 * SettlementPrice1, 18);
                GBaseCurrQty = BaseCurrQty1;

                string SerProName = _dbContext.Set<ServiceProviderMaster>().Where(e => e.Id == TransactionQueueObj.SerProID).FirstOrDefault().ProviderName;

                //CurrentTradeBuyerListObj.IsProcessing = 1;//Make unser processing               
                //_TradeBuyerList.UpdateField(CurrentTradeBuyerListObj, e => e.IsProcessing);

                if (SettlementQty1 < CurrentTradeBuyerListObj.RemainQty)//Partial Settlement
                {
                    decimal TakeDisc = 0;
                   APIOrderSettlementEntry(TradeTransactionQueueObj, TradeTransactionQueueObj.BidPrice,TradeTransactionQueueObj.BuyQty, CurrentTradeBuyerListObj.RemainQty,CurrentTradeBuyerListObj.RemainQty - SettlementQty1, SettlementPrice1, SettlementQty1, sAPISettledQty, sAPIRemainQty,4);
                   InsertTradePoolQueue(TransactionQueueObj.MemberID, CurrentTradeBuyerListObj.PairID, 0, SettlementQty1, SettlementPrice1, CurrentTradeBuyerListObj.TrnNo, SettlementQty1, SettlementPrice1, TakeDisc, 0, "SELL", "BUY");// SellerList.RemainQty//CurrentTradeBuyerListObj.Price , buy on price of seller's price 
                        
                    //CurrentTradeBuyerListObj.RemainQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.RemainQty - SettlementQty1, 18);
                    //CurrentTradeBuyerListObj.DeliveredQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.DeliveredQty + SettlementQty1, 18);

                    //WalletDrCrResponse CreditWalletResult = new WalletDrCrResponse();
                    Task<bool> WalletResult = WalletProcessBuy(TradeTransactionQueueObj, 0, SettlementQty1, BaseCurrQty1, 0, TransactionQueueObj.TrnMode, TransactionQueueObj.SerProID, SerProName, CreditWalletResult);

                    //TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
                    //TransactionQueueObj.SetTransactionStatusMsg("Partial Settlement Done");
                    //TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
                    //TradeTransactionQueueObj.SetTransactionStatusMsg("Partial Settlement Done");
                    //TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + SettlementQty1, 18);
                    //TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + BaseCurrQty1, 18);
                    

                    if (await WalletResult == false)
                    {   
                        _dbContext.Entry(CurrentTradeBuyerListObj).Reload();
                        _dbContext.Entry(TransactionQueueObj).Reload();
                        _dbContext.Entry(TradeTransactionQueueObj).Reload();                        
                       
                        //Task WalletFail = WalletFailOrderProcessBUY(CurrentTradeBuyerListObj.TrnNo, SellerList, TradeTQMakerObj, TQMakerObj, CreditWalletResult);
                        //TrackDebitBit = 0;
                        _Resp.ErrorCode = enErrorCode.Settlement_PartialSettlementRollback;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Partial Settlement RollBack ##TrnNo " + CurrentTradeBuyerListObj.TrnNo + " SettlementQty1 " + SettlementQty1;
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT reverse result", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
                        //await WalletFail;
                    }
                    else
                    {
                        CurrentTradeBuyerListObj.RemainQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.RemainQty - SettlementQty1, 18);
                        CurrentTradeBuyerListObj.DeliveredQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.DeliveredQty + SettlementQty1, 18);
                        
                        TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
                        TransactionQueueObj.SetTransactionStatusMsg("Partial Settlement Done");
                        TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
                        TradeTransactionQueueObj.SetTransactionStatusMsg("Partial Settlement Done");
                        TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + SettlementQty1, 18);
                        TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + BaseCurrQty1, 18);


                        _dbContext.Database.BeginTransaction();
                        _dbContext.Entry(CurrentTradeBuyerListObj).State = EntityState.Modified;
                        _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
                        _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;
                        
                        TradePoolQueueObj.UpdatedDate = Helpers.UTC_To_IST();
                        _dbContext.Set<TradePoolQueueV1>().Add(TradePoolQueueObj);

                        APIOrderSettlementObj.UpdatedDate = Helpers.UTC_To_IST();
                        _dbContext.Set<APIOrderSettlement>().Add(APIOrderSettlementObj);

                        _dbContext.SaveChanges();
                        _dbContext.Database.CommitTransaction();
                        //TrackDebitBit = 0;                       

                        Task.Run(() => PartilaSettleBuySignalR(CurrentTradeBuyerListObj.TrnNo, TransactionQueueObj, TradeTransactionQueueObj, TradeStopLossObj, accessToken, SettlementPrice1, SettlementQty1, CurrentTradeBuyerListObj.OrderType));

                        _Resp.ErrorCode = enErrorCode.Settlement_PartialSettlementDone;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Partial Settlement Done of ##TrnNo " + CurrentTradeBuyerListObj.TrnNo + " Settled: " + CurrentTradeBuyerListObj.DeliveredQty + " Remain:" + CurrentTradeBuyerListObj.RemainQty;
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPIBUY", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
                    }
                }
                else if (SettlementQty1 == CurrentTradeBuyerListObj.RemainQty)//Full Settlement
                {   
                    decimal TakeDisc = 0;
                    APIOrderSettlementEntry(TradeTransactionQueueObj, TradeTransactionQueueObj.BidPrice, TradeTransactionQueueObj.BuyQty, CurrentTradeBuyerListObj.RemainQty, CurrentTradeBuyerListObj.RemainQty - SettlementQty1, SettlementPrice1, SettlementQty1, sAPISettledQty, sAPIRemainQty, 1);
                    InsertTradePoolQueue(TransactionQueueObj.MemberID, CurrentTradeBuyerListObj.PairID, 0, SettlementQty1, SettlementPrice1, CurrentTradeBuyerListObj.TrnNo, SettlementQty1, SettlementPrice1, TakeDisc, 0, "SELL", "BUY");//+ SettlementQty//CurrentTradeBuyerListObj.Price , buy on price of seller's price

                    //WalletDrCrResponse CreditWalletResult = new WalletDrCrResponse();
                    Task<bool> WalletResult = WalletProcessBuy(TradeTransactionQueueObj, 0, SettlementQty1, BaseCurrQty1, 1, TransactionQueueObj.TrnMode, TransactionQueueObj.SerProID, SerProName, CreditWalletResult);

                    //CurrentTradeBuyerListObj.RemainQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.RemainQty - SettlementQty1, 18);
                    //CurrentTradeBuyerListObj.DeliveredQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.DeliveredQty + SettlementQty1, 18);
                    //CurrentTradeBuyerListObj.MakeTransactionSuccess();
                    //TransactionQueueObj.MakeTransactionSuccess();
                    
                    //TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
                    //TransactionQueueObj.SetTransactionStatusMsg("Full Settlement Done");

                    //TradeTransactionQueueObj.MakeTransactionSuccess();
                    //TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
                    //TradeTransactionQueueObj.SetTransactionStatusMsg("Full Settlement Done");
                    //TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + SettlementQty1, 18);
                    //TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + BaseCurrQty1, 18);
                    //Task<SettledTradeTransactionQueue> SettledTradeTQResult = MakeTransactionSettledEntry(TradeTransactionQueueObj, SettlementPrice1);


                    if (await WalletResult == false)
                    {                        
                        _dbContext.Entry(CurrentTradeBuyerListObj).Reload();
                        _dbContext.Entry(TransactionQueueObj).Reload();
                        _dbContext.Entry(TradeTransactionQueueObj).Reload();  
                        //Task WalletFail = WalletFailOrderProcessBUY(CurrentTradeBuyerListObj.TrnNo, SellerList, TradeTQMakerObj, TQMakerObj, CreditWalletResult);
                        //TrackDebitBit = 0;
                        _Resp.ErrorCode = enErrorCode.Settlement_FullSettlementRollback;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Full Settlement Rollback ##TrnNo:" + TradeTransactionQueueObj.TrnNo + " SettlementQty1 " + SettlementQty1;
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPIBUY reverse Result ", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
                       // await WalletFail;
                    }
                    else
                    {
                        CurrentTradeBuyerListObj.RemainQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.RemainQty - SettlementQty1, 18);
                        CurrentTradeBuyerListObj.DeliveredQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.DeliveredQty + SettlementQty1, 18);
                        CurrentTradeBuyerListObj.MakeTransactionSuccess();
                        TransactionQueueObj.MakeTransactionSuccess();

                        TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
                        TransactionQueueObj.SetTransactionStatusMsg("Full Settlement Done");

                        TradeTransactionQueueObj.MakeTransactionSuccess();
                        TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
                        TradeTransactionQueueObj.SetTransactionStatusMsg("Full Settlement Done");
                        TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + SettlementQty1, 18);
                        TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + BaseCurrQty1, 18);
                        Task<SettledTradeTransactionQueue> SettledTradeTQResult = MakeTransactionSettledEntry(TradeTransactionQueueObj, SettlementPrice1);

                        _dbContext.Database.BeginTransaction();                       
                        _dbContext.Entry(CurrentTradeBuyerListObj).State = EntityState.Modified;
                        _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
                        _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;
                        TradePoolQueueObj.UpdatedDate = Helpers.UTC_To_IST();
                        _dbContext.Set<TradePoolQueueV1>().Add(TradePoolQueueObj);
                        APIOrderSettlementObj.UpdatedDate = Helpers.UTC_To_IST();
                        _dbContext.Set<APIOrderSettlement>().Add(APIOrderSettlementObj);

                        SettledTradeTransactionQueue SettledTradeTQObj = await SettledTradeTQResult;
                        _dbContext.Set<SettledTradeTransactionQueue>().Add(SettledTradeTQObj);                       
                        _dbContext.SaveChanges();
                        _dbContext.Database.CommitTransaction();
                        //TrackDebitBit = 0;                      
                       
                        Task.Run(() => FullSettleBuySignalR(CurrentTradeBuyerListObj.TrnNo, TransactionQueueObj, TradeTransactionQueueObj, TradeStopLossObj,accessToken, SettlementPrice1, SettlementQty1, CurrentTradeBuyerListObj.OrderType));

                        _Resp.ErrorCode = enErrorCode.Settlement_FullSettlementDone;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Full Settlement Done of ##TrnNo " + CurrentTradeBuyerListObj.TrnNo + " Settled: " + CurrentTradeBuyerListObj.DeliveredQty + " Remain:" + CurrentTradeBuyerListObj.RemainQty;
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPIBUY", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
                                                
                    }
                }
                //else not posible as SettlementQty1 = CurrentTradeBuyerListObj.RemainQty - sAPIRemainQty;  
                
                //CurrentTradeBuyerListObj.IsProcessing = 0;//Release Buy Order                 
                //_TradeBuyerList.UpdateField(CurrentTradeBuyerListObj, e => e.IsProcessing);
               
               
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PROCESSSETLLEMENTAPIBUY:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
                //CurrentTradeBuyerListObj.IsProcessing = 0;//Release Buy Order             
                //_dbContext.Entry(CurrentTradeBuyerListObj).Property(p=>p.IsProcessing).IsModified = true;
                _dbContext.SaveChanges();                
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.Settlement_SettlementInternalError;
            }
            return _Resp;
        }

        public async Task<BizResponse> PROCESSSETLLEMENTAPISell(BizResponse _Resp, TradeTransactionQueue TradeTransactionQueueObj,TransactionQueue TransactionQueueObj, TradeStopLoss TradeStopLossObj,TradeSellerListV1 CurrentTradeSellerListObj, string accessToken = "")
        {
            //short TrackBit=0;
            //short TrackDebitBit = 0;
            decimal GSettlementQty = 0;
            decimal GBaseCurrQty = 0;
            try
            { 
                if (CurrentTradeSellerListObj.IsProcessing == 1)
                {
                    _Resp.ErrorCode = enErrorCode.API_IsProcessing;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Is Under Processing";
                    return _Resp;
                }
                if (CurrentTradeSellerListObj.RemainQty == 0)
                {
                    _Resp.ErrorCode = enErrorCode.Settlement_AlreadySettled;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "ALready Settled";
                    return _Resp;
                }
                if (CurrentTradeSellerListObj.IsAPITrade != 1 || TradeTransactionQueueObj.IsAPITrade != 1)
                {
                    _Resp.ErrorCode = enErrorCode.API_NotAPIOrder;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Not API order";
                    return _Resp;
                }
                if (CurrentTradeSellerListObj.Status != 4 && CurrentTradeSellerListObj.Status != 0)
                {
                    _Resp.ErrorCode = enErrorCode.API_BuyerOrderNotPending;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Seller Order Status not in pending state";
                    return _Resp;
                }
                if (CurrentTradeSellerListObj.RemainQty == sAPIRemainQty)
                {
                    _Resp.ErrorCode = enErrorCode.API_NoRemainQtyChange;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "No Settlement Change in Remain Qty";
                    return _Resp;
                }
                if (CurrentTradeSellerListObj.SelledQty == sAPISettledQty)
                {
                    _Resp.ErrorCode = enErrorCode.API_NoSettledQtyChange;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "No Settlement Change in Settled Qty";
                    return _Resp;
                }
                decimal SettlementQty1 = 0;
                decimal SettlementPrice1 = sAPIPrice;
                decimal BaseCurrQty1 = 0;

                if (SettlementPrice1 == 0)
                    SettlementPrice1 = CurrentTradeSellerListObj.Price;

                SettlementQty1 = CurrentTradeSellerListObj.RemainQty - sAPIRemainQty;

                if (SettlementQty1 <= 0)
                {
                    _Resp.ErrorCode = enErrorCode.API_InvalidSettlementQty;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ReturnMsg = "Invalid Remain Qty gives wrong calculation";
                    return _Resp;
                }
                if (SettlementQty1 != sAPISettledQty - CurrentTradeSellerListObj.SelledQty)
                {
                    //"filled":"0.0038669999999999998","remaining":"0.0000000000000000004336808690" issue , total : 0.003867000000000000
                    if (SettlementQty1 < sAPISettledQty - CurrentTradeSellerListObj.SelledQty)//local < API
                    {
                        _Resp.ErrorCode = enErrorCode.API_InvalidSettledRemainQty;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ReturnMsg = "Settled and Remail Qty Invalid";
                        return _Resp;
                    }
                    else //API < local , make full settlement
                    {
                        //SettlementQty1 = sAPISettledQty - CurrentTradeBuyerListObj.DeliveredQty;
                    }

                }
                //if (SettlementQty1 != sAPISettledQty - CurrentTradeSellerListObj.SelledQty)
                //{
                //    _Resp.ErrorCode = enErrorCode.API_InvalidSettledRemainQty;
                //    _Resp.ReturnCode = enResponseCodeService.Fail;
                //    _Resp.ReturnMsg = "Settled and Remail Qty Invalid";
                //    return _Resp;
                //}

                GSettlementQty = SettlementQty1;
                BaseCurrQty1 = Helpers.DoRoundForTrading(SettlementQty1 * SettlementPrice1, 18);
                GBaseCurrQty = BaseCurrQty1;

                string SerProName = _dbContext.Set<ServiceProviderMaster>().Where(e => e.Id == TransactionQueueObj.SerProID).FirstOrDefault().ProviderName;

                //CurrentTradeSellerListObj.IsProcessing = 1;//Make Under processing               
                //_TradeSellerList.UpdateField(CurrentTradeSellerListObj, e => e.IsProcessing);

                if (SettlementQty1 < CurrentTradeSellerListObj.RemainQty)
                {
                    decimal TakeDisc = 0;
                    APIOrderSettlementEntry(TradeTransactionQueueObj, TradeTransactionQueueObj.AskPrice, TradeTransactionQueueObj.SellQty, CurrentTradeSellerListObj.RemainQty, CurrentTradeSellerListObj.RemainQty - SettlementQty1, SettlementPrice1, SettlementQty1, sAPISettledQty, sAPIRemainQty, 4);
                    InsertTradePoolQueue(TransactionQueueObj.MemberID, CurrentTradeSellerListObj.PairID, 0, SettlementQty1, SettlementPrice1, CurrentTradeSellerListObj.TrnNo, SettlementQty1, SettlementPrice1, TakeDisc, 0, "BUY", "SELL");
                      
                    //CurrentTradeSellerListObj.RemainQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.RemainQty - SettlementQty1, 18);
                    //CurrentTradeSellerListObj.SelledQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.SelledQty + SettlementQty1, 18);

                    //WalletDrCrResponse CreditWalletResult = new WalletDrCrResponse();
                    Task<bool> WalletResult = WalletProcessSell(TradeTransactionQueueObj, 0, SettlementQty1, BaseCurrQty1, 0, TransactionQueueObj.TrnMode, TransactionQueueObj.SerProID, SerProName, CreditWalletResult);

                    //TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
                    //TransactionQueueObj.SetTransactionStatusMsg("Partial Settlement Done");
                    //TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
                    //TradeTransactionQueueObj.SetTransactionStatusMsg("Partial Settlement Done");
                    //TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + SettlementQty1, 18);
                    //TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + BaseCurrQty1, 18);


                    if (await WalletResult == false)
                    {   
                        _dbContext.Entry(CurrentTradeSellerListObj).Reload();
                        _dbContext.Entry(TransactionQueueObj).Reload();
                        _dbContext.Entry(TradeTransactionQueueObj).Reload();                      
                      //Task WalletFail = WalletFailOrderProcessSELL(CurrentTradeSellerListObj.TrnNo, BuyerList, TradeTQMakerObj, TQMakerObj, CreditWalletResult);
                        //TrackDebitBit = 0;
                        _Resp.ErrorCode = enErrorCode.Settlement_PartialSettlementRollback;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Partial Settlement SELL RollBack ##TrnNo " + CurrentTradeSellerListObj.TrnNo + " SettlementQty1: " + SettlementQty1;
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPISELL reverse result ", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
                        //await WalletFail;
                    }
                    else
                    {//last loop records does not revert, and commit in currecnt txn , as it is present in contex
                        CurrentTradeSellerListObj.RemainQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.RemainQty - SettlementQty1, 18);
                        CurrentTradeSellerListObj.SelledQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.SelledQty + SettlementQty1, 18);

                        TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
                        TransactionQueueObj.SetTransactionStatusMsg("Partial Settlement Done");
                        TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
                        TradeTransactionQueueObj.SetTransactionStatusMsg("Partial Settlement Done");
                        TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + SettlementQty1, 18);
                        TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + BaseCurrQty1, 18);

                        _dbContext.Database.BeginTransaction();
                        _dbContext.Entry(CurrentTradeSellerListObj).State = EntityState.Modified;
                        _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
                        _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;                       
                        TradePoolQueueObj.UpdatedDate = Helpers.UTC_To_IST();
                        _dbContext.Set<TradePoolQueueV1>().Add(TradePoolQueueObj);
                        APIOrderSettlementObj.UpdatedDate = Helpers.UTC_To_IST();
                        _dbContext.Set<APIOrderSettlement>().Add(APIOrderSettlementObj);
                        //await PartDataResult;
                        _dbContext.SaveChanges();
                        _dbContext.Database.CommitTransaction();
                        //TrackDebitBit = 0;  
                        Task.Run(() => PartilaSettleSellSignalR(CurrentTradeSellerListObj.TrnNo, TransactionQueueObj, TradeTransactionQueueObj, TradeStopLossObj, accessToken, SettlementPrice1, SettlementQty1, CurrentTradeSellerListObj.OrderType));

                        _Resp.ErrorCode = enErrorCode.Settlement_PartialSettlementDone;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Partial Settlement SELL Done of ##TrnNo " + CurrentTradeSellerListObj.TrnNo + " Settled: " + CurrentTradeSellerListObj.SelledQty + " Remain:" + CurrentTradeSellerListObj.RemainQty;
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPISELL", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
                    }
                }
                else if (SettlementQty1 == CurrentTradeSellerListObj.RemainQty)
                { 
                    decimal TakeDisc = 0;
                    APIOrderSettlementEntry(TradeTransactionQueueObj, TradeTransactionQueueObj.AskPrice, TradeTransactionQueueObj.SellQty, CurrentTradeSellerListObj.RemainQty, CurrentTradeSellerListObj.RemainQty - SettlementQty1, SettlementPrice1, SettlementQty1, sAPISettledQty, sAPIRemainQty, 1);
                    InsertTradePoolQueue(TransactionQueueObj.MemberID, CurrentTradeSellerListObj.PairID, 0, SettlementQty1, SettlementPrice1, CurrentTradeSellerListObj.TrnNo, SettlementQty1, SettlementPrice1, TakeDisc, 0, "BUY", "SELL");

                    //WalletDrCrResponse CreditWalletResult = new WalletDrCrResponse();
                    Task<bool> WalletResult = WalletProcessSell(TradeTransactionQueueObj, 0, SettlementQty1, BaseCurrQty1, 1, TransactionQueueObj.TrnMode, TransactionQueueObj.SerProID, SerProName, CreditWalletResult);//BuyerList.RemainQty rita 4-1-19 issue when two time debited from original buyer and this make wallet settled then last txn may fail due to insufficient balance

                    //CurrentTradeSellerListObj.RemainQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.RemainQty - SettlementQty1, 18);
                    //CurrentTradeSellerListObj.SelledQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.SelledQty + SettlementQty1, 18);
                    //CurrentTradeSellerListObj.MakeTransactionSuccess();
                    //TransactionQueueObj.MakeTransactionSuccess();
                    //TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
                    //TransactionQueueObj.SetTransactionStatusMsg("Full Settlement Done");

                    //TradeTransactionQueueObj.MakeTransactionSuccess();
                    //TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
                    //TradeTransactionQueueObj.SetTransactionStatusMsg("Full Settlement Done");
                    //TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + SettlementQty1, 18);
                    //TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + BaseCurrQty1, 18);
                    //Task<SettledTradeTransactionQueue> SettledTradeTQResult = MakeTransactionSettledEntry(TradeTransactionQueueObj, SettlementPrice1);

                    if (await WalletResult == false)
                    {
                        _dbContext.Entry(CurrentTradeSellerListObj).Reload();
                        _dbContext.Entry(TransactionQueueObj).Reload();
                        _dbContext.Entry(TradeTransactionQueueObj).Reload();
                     
                       // Task WalletFail = WalletFailOrderProcessSELL(CurrentTradeSellerListObj.TrnNo, BuyerList, TradeTQMakerObj, TQMakerObj, CreditWalletResult);
                        //TrackDebitBit = 0;
                        _Resp.ErrorCode = enErrorCode.Settlement_FullSettlementRollback;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Full Settlement SELL Rollback ##TrnNo:" + TradeTransactionQueueObj.TrnNo + " SettlementQty1:" + SettlementQty1;
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPISELL reverse result ", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
                       // await WalletFail;
                    }
                    else
                    {
                        CurrentTradeSellerListObj.RemainQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.RemainQty - SettlementQty1, 18);
                        CurrentTradeSellerListObj.SelledQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.SelledQty + SettlementQty1, 18);
                        CurrentTradeSellerListObj.MakeTransactionSuccess();
                        TransactionQueueObj.MakeTransactionSuccess();
                        TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
                        TransactionQueueObj.SetTransactionStatusMsg("Full Settlement Done");

                        TradeTransactionQueueObj.MakeTransactionSuccess();
                        TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
                        TradeTransactionQueueObj.SetTransactionStatusMsg("Full Settlement Done");
                        TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + SettlementQty1, 18);
                        TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + BaseCurrQty1, 18);
                        Task<SettledTradeTransactionQueue> SettledTradeTQResult = MakeTransactionSettledEntry(TradeTransactionQueueObj, SettlementPrice1);

                        _dbContext.Database.BeginTransaction();
                        _dbContext.Entry(CurrentTradeSellerListObj).State = EntityState.Modified;
                        _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
                        _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;
                                              
                        TradePoolQueueObj.UpdatedDate = Helpers.UTC_To_IST();
                        _dbContext.Set<TradePoolQueueV1>().Add(TradePoolQueueObj);
                        APIOrderSettlementObj.UpdatedDate = Helpers.UTC_To_IST();
                        _dbContext.Set<APIOrderSettlement>().Add(APIOrderSettlementObj);

                        SettledTradeTransactionQueue SettledTradeTQObj = await SettledTradeTQResult;
                        _dbContext.Set<SettledTradeTransactionQueue>().Add(SettledTradeTQObj);
                        //await PartDataResult;
                        _dbContext.SaveChanges();
                        _dbContext.Database.CommitTransaction();
                        //TrackDebitBit = 0;

                        Task.Run(() => FullSettleSellSignalR(CurrentTradeSellerListObj.TrnNo, TransactionQueueObj, TradeTransactionQueueObj, TradeStopLossObj, accessToken, SettlementPrice1, SettlementQty1, CurrentTradeSellerListObj.OrderType));

                        _Resp.ErrorCode = enErrorCode.Settlement_FullSettlementDone;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ReturnMsg = "Full Settlement SELL Done of ##TrnNo " + CurrentTradeSellerListObj.TrnNo + " Settled: " + CurrentTradeSellerListObj.SelledQty + " Remain:" + CurrentTradeSellerListObj.RemainQty + " SettlementQty1:" + SettlementQty1;
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTAPISELL", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
                        //break;//record settled                           
                    }
                }
                //else not posible as SettlementQty1 = CurrentTradeSellerListObj.RemainQty - sAPIRemainQty;  
              
                //CurrentTradeSellerListObj.IsProcessing = 0;//Release Sell Order             
                //_TradeSellerList.UpdateField(CurrentTradeSellerListObj,e=>e.IsProcessing);
               
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PROCESSSETLLEMENTAPISELL:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);              
                //CurrentTradeSellerListObj.IsProcessing = 0;//Release Sell Order   
                //_dbContext.Entry(CurrentTradeSellerListObj).Property(p => p.IsProcessing).IsModified = true;              
                _dbContext.SaveChanges();
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = ex.Message;
                _Resp.ErrorCode = enErrorCode.Settlement_SettlementInternalError;
            }
            return _Resp;
        }

        public async Task PartilaSettleBuySignalR(long TrnNo, TransactionQueue TransactionQueueObj,TradeTransactionQueue TradeTransactionQueueObj,TradeStopLoss TradeStopLossObj,string accessToken,decimal SettlementPrice,decimal SettlementQty,short OrderType)
        {
            try
            {
                if(OrderType!=3)//SPOT order no partial order possible , so no send any data
                {//do not send buyer-seller book , bcz no any order with this type of order
                    _ISignalRService.OnStatusPartialSuccess(Convert.ToInt16(enTransactionStatus.Success), TransactionQueueObj, TradeTransactionQueueObj, "", TradeStopLossObj.ordertype);
                   
                    Task.Run(() => SMSSendSettledTransaction(TransactionQueueObj.Id, TransactionQueueObj.MemberMobile, SettlementPrice, SettlementQty, 1));
                          
                    Task.Run(() => EmailSendPartialSettledTransaction(TransactionQueueObj.Id, TransactionQueueObj.MemberID.ToString(), TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.BuyQty, TradeTransactionQueueObj.TrnDate.ToString(),
                        TradeTransactionQueueObj.BuyQty - TradeTransactionQueueObj.SettledBuyQty, TradeTransactionQueueObj.SettledBuyQty, TradeTransactionQueueObj.BidPrice, 0, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType));
                }  
                _ISignalRService.OnLtpChange(SettlementPrice, TradeTransactionQueueObj.PairID, TradeTransactionQueueObj.PairName);//rita 11-1-19 for Stop and Limit add/remove in book               
                
            }
            catch (Exception ex)
            {
                //HelperForLog.WriteLogIntoFile("PartilaSettleBuySignalR", ControllerName, "Partial Settlement Error " + ex.Message + "##TrnNo:" + TrnNo);
                HelperForLog.WriteErrorLog("PartilaSettleBuySignalR:##TrnNo " + TrnNo, ControllerName, ex);
            }
        }

        public async Task FullSettleBuySignalR(long TrnNo, TransactionQueue TransactionQueueObj, TradeTransactionQueue TradeTransactionQueueObj,TradeStopLoss TradeStopLossObj,string accessToken, decimal SettlementPrice, decimal SettlementQty, short OrderType)
        {
            try
            {               
                _IFrontTrnService.GetPairAdditionalVal(TradeTransactionQueueObj.PairID, SettlementPrice, TradeTransactionQueueObj.TrnNo, SettlementQty, TradeTransactionQueueObj.TrnDate);//12-3-18 take settlement price and Qty
                _ISignalRService.OnStatusSuccess(Convert.ToInt16(enTransactionStatus.Success), TransactionQueueObj, TradeTransactionQueueObj, "", TradeStopLossObj.ordertype,SettlementPrice);//komal
              
                _ISignalRService.OnLtpChange(SettlementPrice, TradeTransactionQueueObj.PairID, TradeTransactionQueueObj.PairName);//rita 11-1-19 for Stop and Limit add/remove in book
                Task.Run(() => SMSSendSettledTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberMobile, SettlementPrice, SettlementQty, 2));

              
                Task.Run(() => EmailSendPartialSettledTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID.ToString(), TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.BuyQty,
                    TradeTransactionQueueObj.TrnDate.ToString(), 0, 0, TradeTransactionQueueObj.BidPrice, 0, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType, 1));
            }
            catch (Exception ex)
            {                
                HelperForLog.WriteErrorLog("FullSettleBuySignalR:##TrnNo " + TrnNo, ControllerName, ex);
            }
        }
        
        public async Task PartilaSettleSellSignalR(long TrnNo, TransactionQueue TransactionQueueObj, TradeTransactionQueue TradeTransactionQueueObj,TradeStopLoss TradeStopLossObj,string accessToken, decimal SettlementPrice, decimal SettlementQty, short OrderType)
        {
            try
            {
                if (OrderType != 3)//SPOT order no partial order possible , so no send any data
                {//do not send buyer-seller book , bcz no any order with this type of order
                    _ISignalRService.OnStatusPartialSuccess(Convert.ToInt16(enTransactionStatus.Success), TransactionQueueObj, TradeTransactionQueueObj, "", TradeStopLossObj.ordertype);
                   
                    Task.Run(() => SMSSendSettledTransaction(TransactionQueueObj.Id, TransactionQueueObj.MemberMobile, SettlementPrice, SettlementQty, 1));
                    //Uday 08-12-2018 Send Email When Transction is Partially Settled             
                    Task.Run(() => EmailSendPartialSettledTransaction(TransactionQueueObj.Id, TransactionQueueObj.MemberID.ToString(), TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.SellQty, TradeTransactionQueueObj.TrnDate.ToString(),
                        TradeTransactionQueueObj.SellQty - TradeTransactionQueueObj.SettledSellQty, TradeTransactionQueueObj.SettledSellQty, TradeTransactionQueueObj.AskPrice, 0, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType));
                }
                //==============Volume update only after success,Maker/Seller's Txn Success
                _ISignalRService.OnLtpChange(SettlementPrice, TradeTransactionQueueObj.PairID, TradeTransactionQueueObj.PairName);//rita 11-1-19 for Stop and Limit add/remove in book
                                
            }
            catch (Exception ex)
            {                
                HelperForLog.WriteErrorLog("PartilaSettleSellSignalR:##TrnNo " + TrnNo, ControllerName, ex);
            }
        }

        public async Task FullSettleSellSignalR(long TrnNo, TransactionQueue TransactionQueueObj, TradeTransactionQueue TradeTransactionQueueObj,TradeStopLoss TradeStopLossObj,string accessToken, decimal SettlementPrice, decimal SettlementQty, short OrderType)
        {
            try
            {
                _IFrontTrnService.GetPairAdditionalVal(TradeTransactionQueueObj.PairID, SettlementPrice, TradeTransactionQueueObj.TrnNo, SettlementQty, TradeTransactionQueueObj.TrnDate);//12-3-18 take settlement price and Qty
                _ISignalRService.OnStatusSuccess(Convert.ToInt16(enTransactionStatus.Success), TransactionQueueObj, TradeTransactionQueueObj, "", TradeStopLossObj.ordertype,SettlementPrice);//komal
                _ISignalRService.OnLtpChange(SettlementPrice, TradeTransactionQueueObj.PairID, TradeTransactionQueueObj.PairName);//rita 11-1-19 for Stop and Limit add/remove in book
               
                Task.Run(() => SMSSendSettledTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberMobile, SettlementPrice, SettlementQty, 2));

            }
            catch (Exception ex)
            {                
                HelperForLog.WriteErrorLog("FullSettleSellSignalR:##TrnNo " + TrnNo, ControllerName, ex);
            }
        }

        public async Task InsertTradePoolQueue(long MemberID, long PairID, long MakerTrnNo, decimal MakerQty, decimal MakerPrice, long TakerTrnNo, decimal TakerQty, decimal TakerPrice, decimal TakerDisc, decimal TakerLoss, string MakerType, string TakerType)
        {
            try
            {
                TradePoolQueueObj = new TradePoolQueueV1()
                {
                    CreatedDate = Helpers.UTC_To_IST(),
                    CreatedBy = MemberID,
                    PairID = PairID,
                    MakerTrnNo = MakerTrnNo,
                    MakerQty = MakerQty,
                    MakerPrice = MakerPrice,
                    TakerTrnNo = TakerTrnNo,
                    TakerQty = TakerQty,
                    TakerPrice = TakerPrice,
                    TakerDisc = TakerDisc,
                    TakerLoss = TakerLoss,
                    MakerType = MakerType,
                    TakerType = TakerType,
                    Status = Convert.ToInt16(enTransactionStatus.Success),//always etry after settlement done
                    IsAPITrade = 1,
                };                
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertTradePoolQueue:##TrnNo " + TakerTrnNo, ControllerName, ex);
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
                throw ex;
            }

        }

        private async Task<bool> WalletProcessBuy(TradeTransactionQueue TradeTQueueCurrentObj, decimal Price, decimal SettledQty, decimal BaseCurrQty, short CurrIsFullSettled, short TrnMode, long SerProID, string SerProName, WalletDrCrResponse CreditWalletResult1)
        {
            CreditWalletResult = new WalletDrCrResponse();//here object pass not working in async method
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT API WalletProcessBuy", ControllerName, "Start Wallet Operation With #SerProID:" + SerProName + "##TrnNo:" + TradeTQueueCurrentObj.TrnNo, Helpers.UTC_To_IST()));

                ArbitrageCommonClassCrDr WalletSettlementObj = new ArbitrageCommonClassCrDr();

                WalletSettlementObj.SerProID = SerProID;
                WalletSettlementObj.Amount = SettledQty; //main qty
                WalletSettlementObj.HoldAmount = BaseCurrQty;//baseqty
                WalletSettlementObj.Coin = TradeTQueueCurrentObj.Delivery_Currency; //Cr
                WalletSettlementObj.HoldCoin = TradeTQueueCurrentObj.Order_Currency; //Dr                
                WalletSettlementObj.TrnRefNo = TradeTQueueCurrentObj.TrnNo;
                WalletSettlementObj.WalletId = TradeTQueueCurrentObj.DeliveryWalletID;// Cr
                WalletSettlementObj.trnType = enWalletTrnType.BuyTrade;//buy sell
                WalletSettlementObj.isFullSettled = CurrIsFullSettled;
                WalletSettlementObj.UserID = TradeTQueueCurrentObj.MemberID;//memberid
                WalletSettlementObj.OrderType = enWalletDeductionType.Limit;//limit

                CreditWalletResult = await _WalletService.GetLPWalletCreditDrForHoldNewAsyncFinal(WalletSettlementObj, Helpers.GetTimeStamp(),
                                          enServiceType.Trading, (EnAllowedChannels)TrnMode);

                if (CreditWalletResult.ReturnCode != enResponseCode.Success)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT API  WalletProcessBuy", ControllerName, "Wallet Operation fail " + CreditWalletResult.ReturnMsg + " With #SerProID:" + SerProName + "##TrnNo:" + TradeTQueueCurrentObj.TrnNo, Helpers.UTC_To_IST()));
                    return false;
                }
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT API  WalletProcessBuy", ControllerName, "End Wallet Operation With #SerProID:" + SerProName + "##TrnNo:" + TradeTQueueCurrentObj.TrnNo, Helpers.UTC_To_IST()));

                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WalletProcessBuy:##TrnNo " + TradeTQueueCurrentObj.TrnNo, ControllerName, ex);
                return false;
            }
        }
        private async Task<bool> WalletProcessSell(TradeTransactionQueue TradeTQueueCurrentObj, decimal Price, decimal SettledQty, decimal BaseCurrQty, short CurrIsFullSettled, short TrnMode, long SerProID, string SerProName, WalletDrCrResponse CreditWalletResult1)
        {
            CreditWalletResult = new WalletDrCrResponse();//here object pass not working in async method
            try
            {

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT API WalletProcessSell", ControllerName, "Start Wallet Operation With #SerProID:" + SerProName + "##TrnNo:" + TradeTQueueCurrentObj.TrnNo, Helpers.UTC_To_IST()));

                ArbitrageCommonClassCrDr WalletSettlementObj = new ArbitrageCommonClassCrDr();

                WalletSettlementObj.SerProID = SerProID;
                WalletSettlementObj.Amount = BaseCurrQty; //Cr qty
                WalletSettlementObj.HoldAmount = SettledQty;//Dr Qty
                WalletSettlementObj.Coin = TradeTQueueCurrentObj.Delivery_Currency; //Cr
                WalletSettlementObj.HoldCoin = TradeTQueueCurrentObj.Order_Currency; //Dr                
                WalletSettlementObj.TrnRefNo = TradeTQueueCurrentObj.TrnNo;
                WalletSettlementObj.WalletId = TradeTQueueCurrentObj.DeliveryWalletID;// Cr
                WalletSettlementObj.trnType = enWalletTrnType.SellTrade;//buy sell
                WalletSettlementObj.isFullSettled = CurrIsFullSettled;
                WalletSettlementObj.UserID = TradeTQueueCurrentObj.MemberID;//memberid
                WalletSettlementObj.OrderType = enWalletDeductionType.Limit;//limit

                CreditWalletResult = await _WalletService.GetLPWalletCreditDrForHoldNewAsyncFinal(WalletSettlementObj, Helpers.GetTimeStamp(),
                                          enServiceType.Trading, (EnAllowedChannels)TrnMode);

                if (CreditWalletResult.ReturnCode != enResponseCode.Success)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT API WalletProcessSell", ControllerName, "Wallet Operation fail " + CreditWalletResult.ReturnMsg + " With #SerProID:" + SerProName + "##TrnNo:" + TradeTQueueCurrentObj.TrnNo, Helpers.UTC_To_IST()));
                    return false;
                }
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT API WalletProcessSell", ControllerName, "End Wallet Operation With #SerProID:" + SerProName + "##TrnNo:" + TradeTQueueCurrentObj.TrnNo, Helpers.UTC_To_IST()));
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WalletProcessSell:##TrnNo " + TradeTQueueCurrentObj.TrnNo, ControllerName, ex);
                return false;
            }
        }

        public async Task<APIOrderSettlement> APIOrderSettlementEntry(TradeTransactionQueue TradeTransactionQueueObj,decimal Price, decimal Qty, decimal OldQty, decimal NewQty, decimal APIPrice, decimal SettledQty, decimal APISettledQty, decimal APIRemainQty,short NewStatus)
        {           
            try
            {
                APIOrderSettlementObj = new APIOrderSettlement()
                {
                    CreatedDate = Helpers.UTC_To_IST(),
                    CreatedBy = TradeTransactionQueueObj.MemberID,
                    TrnNo = TradeTransactionQueueObj.TrnNo,                   
                    PairID = TradeTransactionQueueObj.PairID,
                    PairName = TradeTransactionQueueObj.PairName,                   
                    Status = TradeTransactionQueueObj.Status,
                    OldStatus = TradeTransactionQueueObj.Status,
                    NewStatus = NewStatus,
                    Price = Price,
                    Qty = Qty,
                    OldQty = OldQty,
                    NewQty = NewQty,
                    SettledQty = SettledQty,
                    APIPrice = APIPrice,
                    APISettledQty = APISettledQty,
                    APIRemainQty = APIRemainQty,
                };
                return APIOrderSettlementObj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("APIOrderSettlementEntry:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
                return null;
            }
        }

        public async Task<SettledTradeTransactionQueue> MakeTransactionSettledEntry(TradeTransactionQueue TradeTransactionQueueObj, decimal Price, short IsCancelled=0)
        {
            decimal BidPrice= TradeTransactionQueueObj.BidPrice;
            decimal AskPrice= TradeTransactionQueueObj.AskPrice;
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
                SettledTradeTransactionQueue SettledTradeTQObj = new SettledTradeTransactionQueue()
                {
                    TrnNo= TradeTransactionQueueObj.TrnNo,
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
                    IsCancelled= IsCancelled
                };
                return SettledTradeTQObj;
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog("MakeTransactionSettledEntry:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
                throw ex;
            }
        }
        #endregion

        #region Send SMS And Email
        public async Task SMSSendSettledTransaction(long TrnNo, string MobileNumber, decimal Price, decimal Qty, int SettleType)
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
                    communicationParamater.Param4 = (Price * Qty) + "";

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SendSMSTransaction - SMSSendSettledTransaction", ControllerName, " ##TrnNo : " + TrnNo + " ##MobileNo : " + MobileNumber + " ##Price : " + Price + " ##Qty : " + Qty + " ##Type : " + SettleType, Helpers.UTC_To_IST()));

                    if (SettleType == 1) // Partially Settelement
                    {
                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_TransactionPartiallySettled, communicationParamater, enCommunicationServiceType.SMS).Result;
                    }
                    else if (SettleType == 2) // Full Sattelement
                    {
                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_TransactionSettled, communicationParamater, enCommunicationServiceType.SMS).Result;
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
                Task.Run(() => HelperForLog.WriteErrorLog("SMSSendSettledTransaction:##TrnNo " + TrnNo, ControllerName, ex));
            }
        }

        public async Task EmailSendPartialSettledTransaction(long TrnNo, string UserId, string Pair, decimal Qty, string TrnDate, decimal RemainingQty, decimal SettleQty, decimal Amount, decimal Fee, short OrderType, short TrnType,short EmailType = 0)
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
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SendEmailTransaction - EmailSendPartialSettledTransaction", ControllerName, " ##TrnNo : " + TrnNo, Helpers.UTC_To_IST()));

                    //Uday 03-01-2019 Transaction Full Success email merge with this function

                    if (EmailType == 0)  // Partial Email
                    {
                        communicationParamater.Param1 = User.UserName;
                        communicationParamater.Param2 = Pair;
                        communicationParamater.Param3 = Helpers.DoRoundForTrading(Qty, 18).ToString();
                        communicationParamater.Param4 = Pair.Split("_")[1];
                        communicationParamater.Param5 = TrnDate;
                        communicationParamater.Param6 = Helpers.DoRoundForTrading(RemainingQty, 18).ToString();
                        communicationParamater.Param7 = Helpers.DoRoundForTrading(SettleQty, 18).ToString();
                        communicationParamater.Param8 = Helpers.DoRoundForTrading(Amount, 18).ToString();
                        communicationParamater.Param9 = Helpers.DoRoundForTrading(Fee, 18).ToString();
                        communicationParamater.Param10 = Helpers.DoRoundForTrading(((Amount * SettleQty) + Fee), 18).ToString();  //Uday 01-01-2019 In Final Price Calculation Change
                        communicationParamater.Param11 = ((enTransactionMarketType)OrderType).ToString();  //Uday 01-01-2019 Add OrderType In Email
                        communicationParamater.Param12 = ((enTrnType)TrnType).ToString();  //Uday 01-01-2019 Add TranType In Email
                        communicationParamater.Param13 = TrnNo.ToString(); //Uday 01-01-2019 Add TrnNo In Email

                        if (OrderType == 2) //Uday 04-01-2019 if ordertype is market than not give price and total
                        {
                            EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.Email_MArket_TransactionPartialSuccess, communicationParamater, enCommunicationServiceType.Email).Result;
                        }
                        else
                        {
                            EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_TransactionPartialSuccess, communicationParamater, enCommunicationServiceType.Email).Result;
                        }
                     }
                    else if (EmailType == 1)  // Full Success Email
                    {
                        communicationParamater.Param1 = User.UserName;
                        communicationParamater.Param2 = Pair;
                        communicationParamater.Param3 = Helpers.DoRoundForTrading(Qty, 18).ToString();
                        communicationParamater.Param4 = Pair.Split("_")[1];
                        communicationParamater.Param5 = TrnDate;
                        communicationParamater.Param6 = "Success";
                        communicationParamater.Param8 = Helpers.DoRoundForTrading(Amount, 18).ToString();
                        communicationParamater.Param9 = Helpers.DoRoundForTrading(Fee, 18).ToString();
                        communicationParamater.Param10 = Helpers.DoRoundForTrading(((Amount * Qty) + Fee), 18).ToString();
                        communicationParamater.Param11 = ((enTransactionMarketType)OrderType).ToString();
                        communicationParamater.Param12 = ((enTrnType)TrnType).ToString();
                        communicationParamater.Param13 = TrnNo.ToString();

                        if (OrderType == 2) //Uday 04-01-2019 if ordertype is market than not give price and total
                        {
                            EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.Email_Market_TransactionSuccess, communicationParamater, enCommunicationServiceType.Email).Result;
                        }
                        else
                        {
                            EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.TransactionSuccess, communicationParamater, enCommunicationServiceType.Email).Result;
                        }
                    }
                    else if (EmailType == 2)  //Rita 25-1-19 Cancel Order for STOP Order
                    {
                        communicationParamater.Param8 = User.UserName + "";
                        communicationParamater.Param1 = Pair + "";
                        communicationParamater.Param2 = Helpers.DoRoundForTrading(Qty, 18).ToString();
                        communicationParamater.Param3 = Pair.Split("_")[1];
                        communicationParamater.Param4 = TrnDate;
                        communicationParamater.Param5 = Helpers.DoRoundForTrading(Amount, 18).ToString();
                        communicationParamater.Param6 = Helpers.DoRoundForTrading(Fee, 18).ToString();
                        communicationParamater.Param7 = Helpers.DoRoundForTrading((Fee + (Amount * Qty)), 18).ToString();
                        communicationParamater.Param11 = ((enTransactionMarketType)OrderType).ToString();
                        communicationParamater.Param12 = ((enTrnType)TrnType).ToString();
                        communicationParamater.Param13 = TrnNo.ToString();

                        short CancelType = 1;//here used only for Fully cancelled
                        if (CancelType == 1) // Cancel Success
                        {
                            EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_OrderCancel, communicationParamater, enCommunicationServiceType.Email).Result;
                        }
                        //else if (CancelType == 2) // Cancel Partially
                        //{
                        //    communicationParamater.Param9 = Helpers.DoRoundForTrading(SettledQty, 8).ToString();
                        //    communicationParamater.Param10 = Helpers.DoRoundForTrading(CancelQty, 8).ToString();
                        //    communicationParamater.Param7 = Helpers.DoRoundForTrading((fee + (price * SettledQty)), 8).ToString();     //Uday 01-01-2019 In Final Price Calculation Change

                        //    EmailData = _messageService.SendMessageAsync(EnTemplateType.EMAIL_PartialOrderCancel, communicationParamater, enCommunicationServiceType.Email).Result;
                        //}
                        //else if (CancelType == 3) // Cancel Failed
                        //{
                        //    communicationParamater.Param7 = Helpers.DoRoundForTrading(0, 8).ToString();
                        //    EmailData = _messageService.SendMessageAsync(EnTemplateType.EMAIL_OrderCancelFailed, communicationParamater, enCommunicationServiceType.Email).Result;
                        //}
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
