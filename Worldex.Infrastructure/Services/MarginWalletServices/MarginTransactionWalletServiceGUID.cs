using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.MarginWallet;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.MarginWalletServices
{
    public class MarginTransactionWalletServiceGUID : IMarginTransactionWalletGUID
    {
        #region DI      
        private readonly IMarginWalletTQInsert _WalletTQInsert;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly IMarginSPRepositories _walletSPRepositories;
        private readonly ISignalRService _signalRService;
        private readonly ICommonRepository<MarginWalletTypeMaster> _WalletTypeMasterRepository;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private readonly IMarginWalletRepository _walletRepository1;
        private readonly IWebApiRepository _webApiRepository;
        private readonly IMessageService _messageService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IMessageConfiguration _messageConfiguration;
        private readonly ICommonRepository<MarginWalletAuthorizeUserMaster> _WalletAuthorizeUserMaster;
        private readonly ICommonRepository<MarginWalletMaster> _commonRepository;
        private readonly ICommonRepository<MarginTransactionAccount> _TransactionAccountsRepository;
        private readonly ICommonRepository<Worldex.Core.Entities.NewWallet.LeverageMaster> _LeverageMaster;
        private readonly ICommonRepository<MarginChargeOrder> _MarginChargeOrder;
        private readonly IMarginCreateOrderFromWallet _CreateOrder;
        private readonly ICommonRepository<MarginCloseUserPositionWallet> _ClosePosition;

        #endregion

        public MarginTransactionWalletServiceGUID(

            ICommonRepository<MarginWalletMaster> commonRepository,
            IMarginWalletRepository walletRepository, IWebApiRepository webApiRepository,
            IWebApiSendRequest webApiSendRequest, IMessageConfiguration messageConfiguration,
            ICommonRepository<MarginWalletTypeMaster> WalletTypeMasterRepository, UserManager<ApplicationUser> userManager,
            IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, ISignalRService signalRService,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
           IMarginSPRepositories walletSPRepositories, IMessageService messageService, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue,
            ICommonRepository<MarginWalletAuthorizeUserMaster> WalletAuthorizeUserMaster, IMarginWalletTQInsert WalletTQInsert,
            ICommonRepository<MarginTransactionAccount> TransactionAccountsRepository, ICommonRepository<Worldex.Core.Entities.NewWallet.LeverageMaster> LeverageMaster,
            ICommonRepository<MarginChargeOrder> MarginChargeOrder, IMarginCreateOrderFromWallet CreateOrder, ICommonRepository<MarginCloseUserPositionWallet> closePosition)
        {
            _TransactionAccountsRepository = TransactionAccountsRepository;
            _configuration = configuration;
            _messageConfiguration = messageConfiguration;
            _userManager = userManager;
            _commonRepository = commonRepository;
            _pushNotificationsQueue = pushNotificationsQueue;
            _walletRepository1 = walletRepository;
            _webApiRepository = webApiRepository;
            _WalletTypeMasterRepository = WalletTypeMasterRepository;
            _walletSPRepositories = walletSPRepositories;
            _messageService = messageService;
            _pushSMSQueue = pushSMSQueue;
            _signalRService = signalRService;
            _WalletAuthorizeUserMaster = WalletAuthorizeUserMaster;
            _WalletTQInsert = WalletTQInsert;
            _LeverageMaster = LeverageMaster;
            _MarginChargeOrder = MarginChargeOrder;
            _CreateOrder = CreateOrder;
            _ClosePosition = closePosition;
        }

        public async Task<WalletDrCrResponse> MarginGetWalletHoldNew(long requestUserID, string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enMarginWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal, string RefGuid = "")
        {
            try
            {
                MarginWalletMaster dWalletobj;
                MarginWalletTypeMaster walletTypeMaster;
                MarginWalletTransactionQueue objTQ;
                WalletDrCrResponse resp = new WalletDrCrResponse();
                enWalletTranxOrderType orderType = enWalletTranxOrderType.Credit;
                long userID = 0, TrnNo = 0;

                HelperForLog.WriteLogIntoFileAsync("MarginGetWalletHoldNew", "MarginTransactionWalletService", "timestamp:" + timestamp + "," + "coinName:" + coinName + ",accWalletID=" + accWalletID + ",TrnRefNo=" + TrnRefNo.ToString() + ",userID=" + requestUserID + ",amount=" + amount.ToString());

                if (string.IsNullOrEmpty(accWalletID) || coinName == string.Empty)
                {
                    return new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidWalletOrUserIDorCoinName, TimeStamp = timestamp };
                }
                walletTypeMaster = _WalletTypeMasterRepository.GetSingle(e => e.WalletTypeName == coinName);
                if (walletTypeMaster == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidCoinName, TimeStamp = timestamp }, "Debit");
                }
                //2019-2-18 added condi for only used trading wallet
                Task<MarginWalletMaster> dWalletobjTask = _commonRepository.GetSingleAsync(e => e.WalletTypeID == walletTypeMaster.Id && e.AccWalletID == accWalletID && e.WalletUsageType == EnWalletUsageType.Margin_Trading_Wallet && e.UserID == requestUserID);

                if (TrnRefNo == 0) // sell 13-10-2018
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTradeRefNo, trnType, RefGuid, Convert.ToInt64(enErrorCode.InvalidTradeRefNo));
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                if (amount <= 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidAmt, trnType, RefGuid,Convert.ToInt64(enErrorCode.InvalidAmount));
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmount, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                dWalletobj = await dWalletobjTask;
                if (dWalletobj == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TimeStamp = timestamp }, "Debit");
                }
                userID = dWalletobj.UserID;
                HelperForLog.WriteLogIntoFileAsync("MarginGetWalletHoldNew", "CheckUserBalance Post TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);
                //dWalletobj = _commonRepository.GetById(dWalletobj.Id); // ntrivedi fetching fresh balance for multiple request at a time 
                if (dWalletobj.Balance < amount && enWalletDeductionType != enWalletDeductionType.InternalSquareOffOrder) //ntrivedi 09-05-2018 internal square off order duplicate may be so already hold and it will be release in sp 
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, trnType,RefGuid, Convert.ToInt64(enErrorCode.InsufficantBal));
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficantBal, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }
                HelperForLog.WriteLogIntoFileAsync("MarginGetWalletHoldNew", "Check ShadowLimit done TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);
                HelperForLog.WriteLogIntoFileAsync("MarginGetWalletHoldNew", "CheckTrnRefNo TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);
                BizResponseClass bizResponse = _walletSPRepositories.Callsp_HoldWallet(dWalletobj, timestamp, serviceType, amount, coinName, allowedChannels, walletTypeMaster.Id, TrnRefNo, dWalletobj.Id, dWalletobj.UserID, routeTrnType, trnType, ref TrnNo, enWalletDeductionType);

                decimal charge = 0;
                MarginWalletTypeMaster ChargewalletType = null;

                if (bizResponse.ReturnCode == enResponseCode.Success)
                {
                    try
                    {
                        charge = _walletRepository1.FindChargeValueHold(timestamp, TrnRefNo);
                        var walletId = _walletRepository1.FindChargeValueWalletId(timestamp, TrnRefNo);
                        MarginWalletMaster WalletlogObj = null;
                        if (charge > 0 && walletId > 0)
                        {
                            WalletlogObj = _commonRepository.GetById(walletId);
                            ChargewalletType = _WalletTypeMasterRepository.GetSingle(i => i.Id == WalletlogObj.WalletTypeID);
                        }
                        Task.Run(() => WalletHoldNotificationSend(timestamp, dWalletobj, coinName, amount, RefGuid, (byte)routeTrnType, charge, walletId, WalletlogObj));
                    }
                    catch (Exception ex)
                    {
                        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                    }
                    return GetCrDRResponse(new WalletDrCrResponse { Charge = charge, ChargeCurrency = (ChargewalletType == null ? "" : ChargewalletType.WalletTypeName), ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDebit, ErrorCode = enErrorCode.Success, TrnNo = TrnNo, Status = enTransactionStatus.Hold, StatusMsg = bizResponse.ReturnMsg, TimeStamp = timestamp }, "DebitForHold");

                }
                else
                {
                    // ntrivedi 12-02-2018 status message changed
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = bizResponse.ReturnMsg, ErrorCode = bizResponse.ErrorCode, TrnNo = TrnNo, Status = enTransactionStatus.Initialize, StatusMsg = bizResponse.ReturnMsg, TimeStamp = timestamp }, "DebitForHold");
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError, TrnNo = 0, Status = 0, StatusMsg = EnResponseMessage.InternalError, TimeStamp = timestamp }, "DebitForHold");
            }
        }


        public async Task<WalletDrCrResponse> MarginGetWalletCreditDrForHoldNewAsyncFinal(MarginPNL PNLObj, MarginCommonClassCrDr firstCurrObj, MarginCommonClassCrDr secondCurrObj, string timestamp, enServiceType serviceType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web,string RefGuid="")
        {
            try
            {
                MarginWalletTransactionQueue tqObj;
                MarginWalletMaster firstCurrObjCrWM, firstCurrObjDrWM, secondCurrObjCrWM, secondCurrObjDrWM;
                MarginWalletTypeMaster walletTypeFirstCurr, walletTypeSecondCurr;
                bool checkDebitRefNo, checkDebitRefNo1;
                Task<bool> checkDebitRefNoTask;
                Task<bool> checkDebitRefNoTask1;
                BizResponseClass bizResponseClassFC, bizResponseClassSC;
                Task.Run(() => HelperForLog.WriteLogIntoFile("MarginGetWalletCreditDrForHoldNewAsyncFinal first currency", "MarginTransactionWalletService", "timestamp:" + timestamp + Helpers.JsonSerialize(firstCurrObj)));
                Task.Run(() => HelperForLog.WriteLogIntoFile("MarginGetWalletCreditDrForHoldNewAsyncFinal second currency", "MarginTransactionWalletService", "timestamp:" + timestamp + Helpers.JsonSerialize(secondCurrObj)));
                Task.Run(() => HelperForLog.WriteLogIntoFile("MarginGetWalletCreditDrForHoldNewAsyncFinal MarginPNL", "MarginTransactionWalletService", "timestamp:" + timestamp + Helpers.JsonSerialize(PNLObj)));
                var firstCurrObjCrWMTask = _commonRepository.GetSingleAsync(item => item.Id == firstCurrObj.creditObject.WalletId && item.WalletUsageType == EnWalletUsageType.Margin_Trading_Wallet);
                if (firstCurrObj.debitObject.isMarketTrade == 1 && firstCurrObj.debitObject.OrderType != enWalletDeductionType.InternalSquareOffOrder)
                {
                    checkDebitRefNoTask = _walletRepository1.CheckTrnIDDrForMarketAsync(firstCurrObj);
                }
                else if (firstCurrObj.debitObject.OrderType != enWalletDeductionType.InternalSquareOffOrder)
                {
                    checkDebitRefNoTask = _walletRepository1.CheckTrnIDDrForHoldAsync(firstCurrObj);
                }
                else
                {
                    checkDebitRefNoTask = null;
                }
                //2019-2-18 added condi for only used trading wallet
                var firstCurrObjDrWMTask = _commonRepository.GetSingleAsync(item => item.Id == firstCurrObj.debitObject.WalletId && item.WalletUsageType == EnWalletUsageType.Margin_Trading_Wallet);

                if (secondCurrObj.debitObject.isMarketTrade == 1 && secondCurrObj.debitObject.OrderType != enWalletDeductionType.InternalSquareOffOrder)
                {
                    checkDebitRefNoTask1 = _walletRepository1.CheckTrnIDDrForMarketAsync(secondCurrObj);
                }
                else if (secondCurrObj.debitObject.OrderType != enWalletDeductionType.InternalSquareOffOrder)
                {
                    checkDebitRefNoTask1 = _walletRepository1.CheckTrnIDDrForHoldAsync(secondCurrObj);
                }
                else
                {
                    checkDebitRefNoTask1 = null;
                }
                //2019-2-18 added condi for only used trading wallet
                var secondCurrObjCrWMTask = _commonRepository.GetSingleAsync(item => item.Id == secondCurrObj.creditObject.WalletId && item.WalletUsageType == EnWalletUsageType.Margin_Trading_Wallet);

                //2019-2-18 added condi for only used trading wallet
                var secondCurrObjDrWMTask = _commonRepository.GetSingleAsync(item => item.Id == secondCurrObj.debitObject.WalletId && item.WalletUsageType == EnWalletUsageType.Margin_Trading_Wallet);
                Task<MarginWalletTypeMaster> walletTypeFirstCurrTask = _WalletTypeMasterRepository.GetSingleAsync(e => e.WalletTypeName == firstCurrObj.Coin);
                firstCurrObjCrWM = await firstCurrObjCrWMTask;
                firstCurrObj.creditObject.UserID = firstCurrObjCrWM.UserID;

                firstCurrObjDrWM = await firstCurrObjDrWMTask;
                firstCurrObj.debitObject.UserID = firstCurrObjDrWM.UserID;

                Task<MarginWalletTypeMaster> walletTypeSecondCurrTask = _WalletTypeMasterRepository.GetSingleAsync(e => e.WalletTypeName == secondCurrObj.Coin);
                firstCurrObjCrWM = await firstCurrObjCrWMTask;
                firstCurrObj.creditObject.UserID = firstCurrObjCrWM.UserID;

                firstCurrObjDrWM = await firstCurrObjDrWMTask;
                firstCurrObj.debitObject.UserID = firstCurrObjDrWM.UserID;

                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("MarginGetWalletCreditDrForHoldNewAsyncFinal before await1", "MarginTransactionWalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));

                firstCurrObjCrWM = await firstCurrObjCrWMTask;
                if (firstCurrObjCrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjCrWM.Id, secondCurrObj.Coin, firstCurrObjCrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, secondCurrObj.debitObject.trnType,RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.FirstCurrCrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                firstCurrObj.creditObject.UserID = firstCurrObjCrWM.UserID;

                firstCurrObjDrWM = await firstCurrObjDrWMTask;
                if (firstCurrObjDrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, secondCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, secondCurrObj.debitObject.trnType, RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.FirstCurrDrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }
                firstCurrObj.debitObject.UserID = firstCurrObjDrWM.UserID;

                secondCurrObjCrWM = await secondCurrObjCrWMTask;
                if (secondCurrObjCrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjCrWM.Id, secondCurrObj.Coin, secondCurrObjCrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, secondCurrObj.debitObject.trnType, RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SecondCurrCrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }
                secondCurrObj.creditObject.UserID = secondCurrObjCrWM.UserID;

                secondCurrObjDrWM = await secondCurrObjDrWMTask;
                if (secondCurrObjDrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, secondCurrObj.debitObject.trnType, RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SecondCurrDrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }
                secondCurrObj.debitObject.UserID = secondCurrObjDrWM.UserID;

                Task.Run(() => HelperForLog.WriteLogIntoFile("MarginGetWalletCreditDrForHoldNewAsyncFinal after await1", "MarginTransactionWalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));
                Task.Run(() => HelperForLog.WriteLogIntoFile("MarginGetWalletCreditDrForHoldNewAsyncFinal before await2", "MarginTransactionWalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));
                if (firstCurrObj.debitObject.OrderType != enWalletDeductionType.InternalSquareOffOrder) //condition added 08-05-2019
                {
                    checkDebitRefNo = await checkDebitRefNoTask;
                    if (checkDebitRefNo == false)//fail
                    {
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObj.creditObject.WalletId, firstCurrObj.Coin, firstCurrObj.creditObject.UserID, timestamp, enTransactionStatus.SystemFail, "Amount and DebitRefNo matching failure", firstCurrObj.creditObject.trnType, RefGuid);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNoFirCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                }
                if (secondCurrObj.debitObject.OrderType != enWalletDeductionType.InternalSquareOffOrder) //condition added 08-05-2019
                {
                    checkDebitRefNo1 = await checkDebitRefNoTask1;
                    if (checkDebitRefNo1 == false)//fail
                    {
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObj.creditObject.WalletId, secondCurrObj.Coin, secondCurrObj.creditObject.UserID, timestamp, enTransactionStatus.SystemFail, "Amount and DebitRefNo matching failure", secondCurrObj.creditObject.trnType, RefGuid);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNoSecCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                }
                //30-05-2019 move here for second operation started error so take before MarginGetWalletHoldNew call
                walletTypeFirstCurr = await walletTypeFirstCurrTask;
                walletTypeSecondCurr = await walletTypeSecondCurrTask;

                if (walletTypeFirstCurr == null || walletTypeSecondCurr == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidCoinName }, "Credit");
                }

                //ntrivedi InternalSquareOffOrder need to full deduct at sattlement time 02-04-2019 move above 08-05-2019
                if (firstCurrObj.debitObject.OrderType == enWalletDeductionType.InternalSquareOffOrder)
                {
                    WalletDrCrResponse walletcrdrResponseFirstCurr = await MarginGetWalletHoldNew(firstCurrObj.debitObject.UserID, firstCurrObj.Coin, timestamp, firstCurrObj.Amount, firstCurrObjDrWM.AccWalletID, firstCurrObj.debitObject.TrnRefNo, enServiceType.Trading, firstCurrObj.debitObject.trnType, enTrnType.Sell_Trade, EnAllowedChannels.Web, enWalletDeductionType.InternalSquareOffOrder);
                    if (walletcrdrResponseFirstCurr.ReturnCode != 0) //ntrivedi checking returncode 30-04-2019
                    {
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = walletcrdrResponseFirstCurr.ReturnCode, ReturnMsg = walletcrdrResponseFirstCurr.ReturnMsg, ErrorCode = walletcrdrResponseFirstCurr.ErrorCode, TrnNo = walletcrdrResponseFirstCurr.TrnNo, Status = walletcrdrResponseFirstCurr.Status, StatusMsg = walletcrdrResponseFirstCurr.StatusMsg }, "Credit");
                    }
                    checkDebitRefNo = await _walletRepository1.CheckTrnIDDrForHoldAsync(firstCurrObj);
                    //checkDebitRefNo = await checkDebitRefNoTask;
                    if (checkDebitRefNo == false)//fail
                    {
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObj.creditObject.WalletId, firstCurrObj.Coin, firstCurrObj.creditObject.UserID, timestamp, enTransactionStatus.SystemFail, "Amount and DebitRefNo matching failure", firstCurrObj.creditObject.trnType, RefGuid);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNoFirCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                }
                if (secondCurrObj.debitObject.OrderType == enWalletDeductionType.InternalSquareOffOrder)
                {
                    WalletDrCrResponse walletcrdrResponseSecondCurr = await MarginGetWalletHoldNew(secondCurrObj.debitObject.UserID, secondCurrObj.Coin, timestamp, secondCurrObj.Amount, secondCurrObjDrWM.AccWalletID, secondCurrObj.debitObject.TrnRefNo, enServiceType.Trading, secondCurrObj.debitObject.trnType, enTrnType.Buy_Trade, EnAllowedChannels.Web, enWalletDeductionType.InternalSquareOffOrder);
                    if (walletcrdrResponseSecondCurr.ReturnCode != 0) //ntrivedi checking returncode 30-04-2019
                    {
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = walletcrdrResponseSecondCurr.ReturnCode, ReturnMsg = walletcrdrResponseSecondCurr.ReturnMsg, ErrorCode = walletcrdrResponseSecondCurr.ErrorCode, TrnNo = walletcrdrResponseSecondCurr.TrnNo, Status = walletcrdrResponseSecondCurr.Status, StatusMsg = walletcrdrResponseSecondCurr.StatusMsg }, "Credit");
                    }
                    checkDebitRefNo1 = await _walletRepository1.CheckTrnIDDrForHoldAsync(secondCurrObj);
                    if (checkDebitRefNo1 == false)//fail
                    {
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObj.creditObject.WalletId, secondCurrObj.Coin, secondCurrObj.creditObject.UserID, timestamp, enTransactionStatus.SystemFail, "Amount and DebitRefNo matching failure", secondCurrObj.creditObject.trnType, RefGuid);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNoSecCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                }
                if (firstCurrObj.debitObject.isMarketTrade == 1 && firstCurrObj.debitObject.differenceAmount > 0)
                {
                    if (firstCurrObjDrWM.Balance < firstCurrObj.debitObject.differenceAmount)
                    {
                        // insert with status=2 system failed
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, firstCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType, RefGuid);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficientMarketInternalBalanceCheckFirstCurrencyForDifferentialAmountFailed, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit first currency");
                    }
                    bizResponseClassFC = _walletSPRepositories.Callsp_HoldWallet_MarketTrade(firstCurrObjDrWM, timestamp, serviceType, firstCurrObj.debitObject.differenceAmount, firstCurrObj.Coin, allowedChannels, firstCurrObjDrWM.WalletTypeID, firstCurrObj.debitObject.WTQTrnNo, firstCurrObj.debitObject.WalletId, firstCurrObj.debitObject.UserID, enTrnType.Buy_Trade, firstCurrObj.debitObject.trnType, enWalletDeductionType.Market);
                    if (bizResponseClassFC.ReturnCode != enResponseCode.Success)
                    {
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, firstCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType, RefGuid);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.FirstCurrDifferentialAmountHoldFailed, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit second currency");
                    }
                }
                if (secondCurrObj.debitObject.isMarketTrade == 1 && secondCurrObj.debitObject.differenceAmount > 0)
                {
                    if (secondCurrObjDrWM.Balance < secondCurrObj.debitObject.differenceAmount)
                    {
                        // insert with status=2 system failed
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType, RefGuid);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficientMarketInternalBalanceCheckSecondCurrencyForDifferentialAmountFailed, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                    bizResponseClassSC = _walletSPRepositories.Callsp_HoldWallet_MarketTrade(secondCurrObjDrWM, timestamp, serviceType, secondCurrObj.debitObject.differenceAmount, secondCurrObj.Coin, allowedChannels, secondCurrObjDrWM.WalletTypeID, secondCurrObj.debitObject.WTQTrnNo, secondCurrObj.debitObject.WalletId, secondCurrObj.debitObject.UserID, enTrnType.Buy_Trade, secondCurrObj.debitObject.trnType, enWalletDeductionType.Market);
                    if (bizResponseClassSC.ReturnCode != enResponseCode.Success)
                    {
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, firstCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType, RefGuid);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.SecondCurrDifferentialAmountHoldFailed, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                }

                Task.Run(() => HelperForLog.WriteLogIntoFile("MarginGetWalletCreditDrForHoldNewAsyncFinal after await2", "MarginTransactionWalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));

                if (firstCurrObj.Coin == string.Empty || secondCurrObj.Coin == string.Empty)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidWalletOrUserIDorCoinName }, "Credit");
                }
                if (firstCurrObj.Amount <= 0 || secondCurrObj.Amount <= 0) // ntrivedi amount -ve check
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmt }, "Credit");
                }
                if (firstCurrObj.creditObject.TrnRefNo == 0 || secondCurrObj.creditObject.TrnRefNo == 0)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNoCr, ErrorCode = enErrorCode.InvalidTradeRefNoCr }, "Credit");
                }
                if (firstCurrObj.debitObject.TrnRefNo == 0 || secondCurrObj.debitObject.TrnRefNo == 0)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNoDr, ErrorCode = enErrorCode.InvalidTradeRefNoDr }, "Debit");
                }
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("MarginGetWalletCreditDrForHoldNewAsyncFinal before await3", "MarginTransactionWalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));

                if (firstCurrObjDrWM.OutBoundBalance < firstCurrObj.Amount) // ntrivedi checking outbound balance
                {
                    // insert with status=2 system failed
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, firstCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType, RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficientOutgoingBalFirstCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                if (secondCurrObjDrWM.OutBoundBalance < secondCurrObj.Amount)// ntrivedi checking outbound balance
                {
                    // insert with status=2 system failed
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, secondCurrObj.debitObject.trnType, RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficietOutgoingBalSecondCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                if (firstCurrObjDrWM.Status != 1)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, secondCurrObj.debitObject.trnType, RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.FirstCurrWalletStatusDisable, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                if (secondCurrObjDrWM.Status != 1)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, secondCurrObj.debitObject.trnType, RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SecondCurrWalletStatusDisable, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }
                Task.Run(() => HelperForLog.WriteLogIntoFile("MarginGetWalletCreditDrForHoldNewAsyncFinal after await3", "MarginTransactionWalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));



                Task.Run(() => HelperForLog.WriteLogIntoFile("MarginGetWalletCreditDrForHoldNewAsyncFinal before Wallet operation", "MarginTransactionWalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));

                BizResponseClass bizResponse = _walletSPRepositories.Callsp_CrDrWalletForHold(PNLObj, firstCurrObj, secondCurrObj, timestamp, serviceType, walletTypeFirstCurr.Id, walletTypeSecondCurr.Id, (long)allowedChannels);

                _walletRepository1.ReloadEntity(firstCurrObjCrWM, secondCurrObjCrWM, firstCurrObjDrWM, secondCurrObjDrWM);

                if (bizResponse.ReturnCode != enResponseCode.Success)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = bizResponse.ReturnMsg, ErrorCode = bizResponse.ErrorCode, TrnNo = 0, Status = enTransactionStatus.Initialize, StatusMsg = bizResponse.ReturnMsg, TimeStamp = timestamp }, "Credit");
                }

                decimal ChargefirstCur = 0, ChargesecondCur = 0;
                //ntrivedi added for try catch 05-03-2019
                try
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal before WaitAll", "MartinTransactionWalletService", "timestamp:" + timestamp));
                    Task.WaitAll();
                    Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal after WaitAll", "MartinTransactionWalletService", "timestamp:" + timestamp));
                    ChargefirstCur = _walletRepository1.FindChargeValueDeduct(timestamp, secondCurrObj.creditObject.TrnRefNo);
                    ChargesecondCur = _walletRepository1.FindChargeValueDeduct(timestamp, secondCurrObj.debitObject.TrnRefNo);
                    secondCurrObj.debitObject.Charge = ChargesecondCur;
                    firstCurrObj.debitObject.Charge = ChargefirstCur;
                }
                catch (Exception ex1)
                {
                    HelperForLog.WriteErrorLog("GetWalletCreditDrForHoldNewAsyncFinal charge exception  Timestamp" + timestamp, this.GetType().Name, ex1);
                }

                Task.Run(() => HelperForLog.WriteLogIntoFile("MarginGetWalletCreditDrForHoldNewAsyncFinal after Wallet operation", "MarginTransactionWalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));

                Task.Run(() => CreditDebitNotificationSend(timestamp, firstCurrObj, secondCurrObj, firstCurrObjCrWM, firstCurrObjDrWM, secondCurrObjCrWM, secondCurrObjCrWM, ChargefirstCur, ChargesecondCur));

                Task.Run(() => HelperForLog.WriteLogIntoFile("MarginGetWalletCreditDrForHoldNewAsyncFinal:Without Token done", "MarginTransactionWalletService", ",timestamp =" + timestamp));


                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessCredit, ErrorCode = enErrorCode.Success, TrnNo = 0, Status = 0, StatusMsg = "", TimeStamp = timestamp }, "MarginGetWalletCreditDrForHoldNewAsyncFinal");


            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError, TrnNo = 0, Status = 0, StatusMsg = "", TimeStamp = timestamp }, "MarginGetWalletCreditDrForHoldNewAsyncFinal");
            }
        }

        public async Task<WalletDrCrResponse> MarginGetReleaseHoldNew(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enMarginWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, string Token = "",string RefGuid="")
        {
            try
            {
                MarginWalletMaster dWalletobj;
                string remarks = "";
                MarginWalletTypeMaster walletTypeMaster;
                MarginWalletTransactionQueue objTQ;
                //long walletTypeID;
                WalletDrCrResponse resp = new WalletDrCrResponse();
                bool CheckUserBalanceFlag = false;
                enWalletTranxOrderType orderType = enWalletTranxOrderType.Credit;
                long userID = 0, TrnNo = 0;

                HelperForLog.WriteLogIntoFileAsync("MarginGetReleaseHoldNew", "MarginTransactionWalletService", "timestamp:" + timestamp + "," + "coinName:" + coinName + ",accWalletID=" + accWalletID + ",TrnRefNo=" + TrnRefNo.ToString() + ",userID=" + userID + ",amount=" + amount.ToString());

                if (string.IsNullOrEmpty(accWalletID) || coinName == string.Empty)
                {
                    return new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidWalletOrUserIDorCoinName, TimeStamp = timestamp };
                }
                walletTypeMaster = _WalletTypeMasterRepository.GetSingle(e => e.WalletTypeName == coinName);
                if (walletTypeMaster == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidCoinName }, "Release Hold");
                }

                //2019-2-18 added condi for only used trading wallet
                Task<MarginWalletMaster> dWalletobjTask = _commonRepository.GetSingleAsync(e => e.WalletTypeID == walletTypeMaster.Id && e.AccWalletID == accWalletID && e.WalletUsageType == EnWalletUsageType.Margin_Trading_Wallet);

                if (TrnRefNo == 0) // sell 13-10-2018
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTradeRefNo, trnType, RefGuid, Convert.ToInt64(enErrorCode.InvalidTradeRefNo));
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Release Hold");
                }
                if (amount <= 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidAmt, trnType, RefGuid,Convert.ToInt64(enErrorCode.InvalidAmount));
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmount, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Release Hold");
                }
                dWalletobj = await dWalletobjTask;
                if (dWalletobj == null)
                {
                    //tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid().ToString(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, 2, EnResponseMessage.InvalidWallet);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TimeStamp = timestamp }, "Release Hold");
                }
                userID = dWalletobj.UserID;
                Task<bool> flagTask = CheckUserBalanceAsync(amount, dWalletobj.Id);
                if (dWalletobj.Status != 1 || dWalletobj.IsValid == false)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidWallet, trnType, RefGuid, Convert.ToInt64(enErrorCode.InvalidWallet));
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Release Hold");
                }

                HelperForLog.WriteLogIntoFileAsync("MarginGetReleaseHoldNew", "CheckUserBalance pre Balance=" + dWalletobj.Balance.ToString() + "timestamp:" + timestamp + ", TrnNo=" + TrnRefNo.ToString());
                CheckUserBalanceFlag = await flagTask;

                HelperForLog.WriteLogIntoFileAsync("MarginGetReleaseHoldNew", "CheckUserBalance Post TrnNo=" + TrnRefNo.ToString() + "timestamp:" + timestamp);
                dWalletobj = _commonRepository.GetById(dWalletobj.Id); // ntrivedi fetching fresh balance for multiple request at a time 

                if (!CheckUserBalanceFlag)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType, RefGuid, Convert.ToInt64(enErrorCode.SettedBalanceMismatch));
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedBalanceMismatch, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Release Hold");
                }
                Task<bool> flagTask1 = CheckUserBalanceAsync(amount, dWalletobj.Id, enBalanceType.OutBoundBalance);
                CheckUserBalanceFlag = await flagTask1;
                if (!CheckUserBalanceFlag)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType, RefGuid, Convert.ToInt64(enErrorCode.SettedOutgoingBalanceMismatch));
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedOutgoingBalanceMismatch, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Release Hold");
                }
                HelperForLog.WriteLogIntoFileAsync("MarginGetReleaseHoldNew", "before Check ShadowLimit TrnNo=" + TrnRefNo.ToString() + "timestamp:" + timestamp);

                if (dWalletobj.OutBoundBalance < amount)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, trnType, RefGuid, Convert.ToInt64(enErrorCode.InsufficientOutboundBalance));
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficientOutboundBalance, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Release Hold");
                }
                HelperForLog.WriteLogIntoFileAsync("MarginGetReleaseHoldNew", "CheckTrnRefNo TrnNo=" + TrnRefNo.ToString() + "timestamp:" + timestamp);

                BizResponseClass bizResponse = _walletSPRepositories.Callsp_ReleaseHoldWallet(dWalletobj, timestamp, serviceType, amount, coinName, allowedChannels, walletTypeMaster.Id, TrnRefNo, dWalletobj.Id, dWalletobj.UserID, routeTrnType, trnType, ref TrnNo);

                if (bizResponse.ReturnCode == enResponseCode.Success)
                {
                    WalletMasterResponse walletMasterObj = new WalletMasterResponse();
                    walletMasterObj.AccWalletID = dWalletobj.AccWalletID;
                    walletMasterObj.Balance = dWalletobj.Balance;
                    walletMasterObj.WalletName = dWalletobj.Walletname;
                    walletMasterObj.PublicAddress = dWalletobj.PublicAddress;
                    walletMasterObj.IsDefaultWallet = dWalletobj.IsDefaultWallet;
                    walletMasterObj.CoinName = coinName;
                    walletMasterObj.OutBoundBalance = dWalletobj.OutBoundBalance;

                    ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                    ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.HoldBalanceReleaseNotification);
                    ActivityNotification.Param1 = coinName;
                    ActivityNotification.Param2 = amount.ToString();
                    ActivityNotification.Param3 = RefGuid.ToString();
                    ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);

                    decimal charge = _walletRepository1.FindChargeValueDeduct(timestamp, TrnRefNo);

                    ActivityNotificationMessage ActivityNotificationCharge = new ActivityNotificationMessage();
                    ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.ChargeReleasedWallet);
                    ActivityNotificationCharge.Param1 = coinName;
                    ActivityNotificationCharge.Param2 = charge.ToString();
                    ActivityNotificationCharge.Param3 = RefGuid.ToString();
                    ActivityNotificationCharge.Type = Convert.ToInt16(EnNotificationType.Info);

                    HelperForLog.WriteLogIntoFileAsync("GetReleaseHoldNew", "OnWalletBalChange + SendActivityNotificationV2 pre TrnNo=" + TrnRefNo.ToString());

                    Task.Run(() => Parallel.Invoke(() => _signalRService.SendActivityNotificationV2(ActivityNotification, dWalletobj.UserID.ToString(), 2, "", 1),

                        () => _signalRService.OnWalletBalChange(walletMasterObj, coinName, dWalletobj.UserID.ToString(), 2, "", 1)
                      ));

                    if (charge > 0)
                    {
                        Parallel.Invoke(
                       () => _signalRService.SendActivityNotificationV2(ActivityNotificationCharge, dWalletobj.UserID.ToString(), 2, "", 1),
                        () => EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, dWalletobj.UserID.ToString(), charge.ToString(), coinName, Helpers.UTC_To_IST().ToString(), RefGuid.ToString(), "Released."));
                    }
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDebit, ErrorCode = enErrorCode.Success, TrnNo = TrnNo, Status = enTransactionStatus.Hold, StatusMsg = bizResponse.ReturnMsg, TimeStamp = timestamp }, "Release Hold");

                }
                else
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = bizResponse.ReturnMsg, ErrorCode = bizResponse.ErrorCode, TrnNo = TrnNo, Status = enTransactionStatus.Initialize, StatusMsg = bizResponse.ReturnMsg, TimeStamp = timestamp }, "Release Hold");
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MarginMarginGetReleaseHoldNew", "MarginTransactionWalletService TimeStamp:" + timestamp, ex);
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError, TrnNo = 0, Status = 0, StatusMsg = "", TimeStamp = timestamp }, "Release Hold");
            }
        }

        public async Task WalletHoldNotificationSend(string timestamp, MarginWalletMaster dWalletobj, string coinName, decimal amount, string TrnRefNo, byte routeTrnType, decimal charge, long walletId, MarginWalletMaster WalletlogObj)
        {
            try
            {
                WalletMasterResponse walletMasterObj = new WalletMasterResponse();
                walletMasterObj.AccWalletID = dWalletobj.AccWalletID;
                walletMasterObj.Balance = dWalletobj.Balance;
                walletMasterObj.WalletName = dWalletobj.Walletname;
                walletMasterObj.PublicAddress = dWalletobj.PublicAddress;
                walletMasterObj.IsDefaultWallet = dWalletobj.IsDefaultWallet;
                walletMasterObj.CoinName = coinName;
                walletMasterObj.OutBoundBalance = dWalletobj.OutBoundBalance;
                #region EMAIL_SMS
                ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.HoldBalanceNotification);
                ActivityNotification.Param1 = coinName;
                ActivityNotification.Param2 = amount.ToString();
                ActivityNotification.Param3 = TrnRefNo.ToString();
                ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);

                ActivityNotificationMessage ActivityNotificationCharge = new ActivityNotificationMessage();
                ActivityNotificationCharge.MsgCode = Convert.ToInt32(enErrorCode.ChargeHoldWallet);
                ActivityNotificationCharge.Param1 = coinName;
                ActivityNotificationCharge.Param2 = charge.ToString();
                ActivityNotificationCharge.Param3 = TrnRefNo.ToString();
                ActivityNotificationCharge.Type = Convert.ToInt16(EnNotificationType.Info);

                HelperForLog.WriteLogIntoFileAsync("WalletHoldNotificationSend", "OnWalletBalChange + SendActivityNotificationV2 pre timestamp=" + timestamp.ToString());

                Parallel.Invoke(() => _signalRService.SendActivityNotificationV2(ActivityNotification, dWalletobj.UserID.ToString(), 2, "", 1),

                    () => _signalRService.OnWalletBalChange(walletMasterObj, coinName, dWalletobj.UserID.ToString(), 2, "", 1),
                    () => SMSSendAsyncV1(EnTemplateType.SMS_WalletDebited, dWalletobj.UserID.ToString(), null, null, null, null, coinName, routeTrnType.ToString(), TrnRefNo.ToString()),
                    () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, dWalletobj.UserID.ToString(), amount.ToString(), coinName, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString())
        );

                if (charge > 0 && walletId > 0 && WalletlogObj != null)
                {
                    WalletMasterResponse walletMasterObjCharge = new WalletMasterResponse();
                    walletMasterObjCharge.AccWalletID = WalletlogObj.AccWalletID;
                    walletMasterObjCharge.Balance = WalletlogObj.Balance;
                    walletMasterObjCharge.WalletName = WalletlogObj.Walletname;
                    walletMasterObjCharge.PublicAddress = WalletlogObj.PublicAddress;
                    walletMasterObjCharge.IsDefaultWallet = WalletlogObj.IsDefaultWallet;
                    walletMasterObjCharge.CoinName = coinName;
                    walletMasterObjCharge.OutBoundBalance = WalletlogObj.OutBoundBalance;

                    Parallel.Invoke(
                 () => _signalRService.SendActivityNotificationV2(ActivityNotificationCharge, WalletlogObj.UserID.ToString(), 2, "", 1),
                   () => _signalRService.OnWalletBalChange(walletMasterObjCharge, coinName, WalletlogObj.UserID.ToString(), 2, "", 1),
                   () => EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, WalletlogObj.UserID.ToString(), charge.ToString(), coinName, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString(), "Hold."));

                }
                #endregion 
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WalletHoldNotificationSend Timestamp=" + timestamp, "MarginWalletService", ex);
            }
        }
        //2018-12-6
        public async Task SMSSendAsyncV1(EnTemplateType templateType, string UserID, string WalletName = null, string SourcePrice = null, string DestinationPrice = null, string ONOFF = null, string Coin = null, string TrnType = null, string TrnNo = null)
        {
            try
            {
                CommunicationParamater communicationParamater = new CommunicationParamater();
                ApplicationUser User = new ApplicationUser();
                User = await _userManager.FindByIdAsync(UserID);
                if (!string.IsNullOrEmpty(UserID))
                {
                    if (!string.IsNullOrEmpty(User.Mobile) && Convert.ToInt16(templateType) != 0)
                    {
                        if (!string.IsNullOrEmpty(WalletName))
                        {
                            communicationParamater.Param1 = WalletName;  //1.WalletName for CreateWallet and address 2.WalletType for Beneficiary method                                               
                        }
                        if (!string.IsNullOrEmpty(SourcePrice) && !string.IsNullOrEmpty(DestinationPrice))
                        {
                            communicationParamater.Param1 = SourcePrice;
                            communicationParamater.Param2 = DestinationPrice;
                        }
                        if (!string.IsNullOrEmpty(ONOFF))// for whitelisted bit
                        {
                            communicationParamater.Param1 = ONOFF;
                        }
                        if (!string.IsNullOrEmpty(Coin) && !string.IsNullOrEmpty(TrnType) && !string.IsNullOrEmpty(TrnNo))//for credit or debit
                        {
                            communicationParamater.Param1 = Coin;
                            communicationParamater.Param2 = TrnType;
                            communicationParamater.Param3 = TrnNo;
                        }

                        var SmsData = _messageService.ReplaceTemplateMasterData(templateType, communicationParamater, enCommunicationServiceType.SMS).Result;
                        if (SmsData != null)
                        {
                            if (SmsData.IsOnOff == 1)
                            {
                                //SmsData.Content
                                SendSMSRequest Request = new SendSMSRequest();
                                Request.Message = SmsData.Content;
                                Request.MobileNo = Convert.ToInt64(User.Mobile);
                                _pushSMSQueue.Enqueue(Request);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SMSSendAsyncV1" + " - Data- " + templateType.ToString(), "MarginTransactionWalletService", ex);
            }
        }

        //2018-12-6
        public async Task EmailSendAsyncV1(EnTemplateType templateType, string UserID, string Param1 = "", string Param2 = "", string Param3 = "", string Param4 = "", string Param5 = "", string Param6 = "", string Param7 = "", string Param8 = "", string Param9 = "")
        {
            try
            {
                CommunicationParamater communicationParamater = new CommunicationParamater();
                SendEmailRequest Request = new SendEmailRequest();
                ApplicationUser User = new ApplicationUser();
                User = await _userManager.FindByIdAsync(UserID);
                if (!string.IsNullOrEmpty(UserID))
                {
                    if (!string.IsNullOrEmpty(User.Email) && Convert.ToInt16(templateType) != 0)
                    {
                        communicationParamater.Param1 = User.UserName;
                        if (!string.IsNullOrEmpty(Param1))
                        {
                            communicationParamater.Param2 = Param1;
                            communicationParamater.Param3 = Param2;
                            communicationParamater.Param4 = Param3;
                            communicationParamater.Param5 = Param4;
                            communicationParamater.Param6 = Param5;
                            communicationParamater.Param7 = Param6;
                            communicationParamater.Param8 = Param7;
                            communicationParamater.Param9 = Param8;
                            communicationParamater.Param10 = Param9;
                        }

                        var EmailData = _messageService.ReplaceTemplateMasterData(templateType, communicationParamater, enCommunicationServiceType.Email).Result;
                        if (EmailData != null)
                        {
                            Request.Body = EmailData.Content;
                            Request.Subject = EmailData.AdditionalInfo;
                            Request.EmailType = Convert.ToInt16(EnEmailType.Template);
                            Request.Recepient = User.Email;
                            _pushNotificationsQueue.Enqueue(Request);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " -Data- " + templateType.ToString(), "MarginTransactionWalletService", ex);
            }
        }

        public WalletDrCrResponse GetCrDRResponse(WalletDrCrResponse obj, string extras)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFile(extras, "MarginTransactionWalletService", "timestamp:" + obj.TimeStamp + ",ReturnCode=" + obj.ReturnCode + ",ErrorCode=" + obj.ErrorCode + ", ReturnMsg=" + obj.ReturnMsg + ",StatusMsg=" + obj.StatusMsg + ",TrnNo=" + obj.TrnNo));
                return obj;
            }
            catch (Exception ex)
            {
                return obj;
            }
        }
        public MarginWalletTransactionQueue InsertIntoWalletTransactionQueue(Guid Guid, enWalletTranxOrderType TrnType, decimal Amount, long TrnRefNo, DateTime TrnDate, DateTime? UpdatedDate,
           long WalletID, string WalletType, long MemberID, string TimeStamp, enTransactionStatus Status, string StatusMsg, enMarginWalletTrnType enWalletTrnType, string RefGuid, long errorcode = 0)
        {
            try
            {
                MarginWalletTransactionQueue walletTransactionQueue = new MarginWalletTransactionQueue();
                walletTransactionQueue.Guid = Guid;
                walletTransactionQueue.TrnType = TrnType;
                walletTransactionQueue.Amount = Amount;
                walletTransactionQueue.TrnRefNo = TrnRefNo;
                walletTransactionQueue.TrnDate = TrnDate;
                walletTransactionQueue.UpdatedDate = UpdatedDate;
                walletTransactionQueue.WalletID = WalletID;
                walletTransactionQueue.WalletType = WalletType;
                walletTransactionQueue.MemberID = MemberID;
                walletTransactionQueue.TimeStamp = TimeStamp;
                walletTransactionQueue.Status = Status;
                walletTransactionQueue.StatusMsg = StatusMsg;
                walletTransactionQueue.WalletTrnType = enWalletTrnType;
                walletTransactionQueue.ErrorCode = errorcode;
                walletTransactionQueue.RefGuid = RefGuid;
                return walletTransactionQueue;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task CreditDebitNotificationSend(string timestamp, MarginCommonClassCrDr firstCurrObj, MarginCommonClassCrDr secondCurrObj, MarginWalletMaster firstCurrObjCrWM, MarginWalletMaster firstCurrObjDrWM, MarginWalletMaster secondCurrObjCrWM, MarginWalletMaster secondCurrObjDrWM, decimal ChargefirstCur, decimal ChargesecondCur)
        {
            try
            {
                #region SMS_Email
                WalletMasterResponse walletMasterObjCr = new WalletMasterResponse();
                walletMasterObjCr.AccWalletID = firstCurrObjCrWM.AccWalletID;
                walletMasterObjCr.Balance = firstCurrObjCrWM.Balance;
                walletMasterObjCr.WalletName = firstCurrObjCrWM.Walletname;
                walletMasterObjCr.PublicAddress = firstCurrObjCrWM.PublicAddress;
                walletMasterObjCr.IsDefaultWallet = firstCurrObjCrWM.IsDefaultWallet;
                walletMasterObjCr.CoinName = firstCurrObj.Coin;
                walletMasterObjCr.OutBoundBalance = firstCurrObjCrWM.OutBoundBalance;

                WalletMasterResponse walletMasterObjCr1 = new WalletMasterResponse();
                walletMasterObjCr1.AccWalletID = secondCurrObjCrWM.AccWalletID;
                walletMasterObjCr1.Balance = secondCurrObjCrWM.Balance;
                walletMasterObjCr1.WalletName = secondCurrObjCrWM.Walletname;
                walletMasterObjCr1.PublicAddress = secondCurrObjCrWM.PublicAddress;
                walletMasterObjCr1.IsDefaultWallet = secondCurrObjCrWM.IsDefaultWallet;
                walletMasterObjCr1.CoinName = secondCurrObj.Coin;
                walletMasterObjCr1.OutBoundBalance = secondCurrObjCrWM.OutBoundBalance;


                ActivityNotificationMessage ActivityNotificationCr = new ActivityNotificationMessage();
                ActivityNotificationCr.MsgCode = Convert.ToInt32(enErrorCode.CreditWalletMsgNotification);
                ActivityNotificationCr.Param1 = firstCurrObj.Coin;
                ActivityNotificationCr.Param2 = firstCurrObj.creditObject.trnType.ToString();
                ActivityNotificationCr.Param3 = firstCurrObj.creditObject.RefGuid.ToString();
                ActivityNotificationCr.Type = Convert.ToInt16(EnNotificationType.Info);

                ActivityNotificationMessage ActivityNotificationCr1 = new ActivityNotificationMessage();
                ActivityNotificationCr1.MsgCode = Convert.ToInt32(enErrorCode.CreditWalletMsgNotification);
                ActivityNotificationCr1.Param1 = secondCurrObj.Coin;
                ActivityNotificationCr1.Param2 = secondCurrObj.creditObject.trnType.ToString();
                ActivityNotificationCr1.Param3 = secondCurrObj.creditObject.RefGuid.ToString();
                ActivityNotificationCr1.Type = Convert.ToInt16(EnNotificationType.Info);


                WalletMasterResponse walletMasterObjDr = new WalletMasterResponse();
                walletMasterObjDr.AccWalletID = firstCurrObjDrWM.AccWalletID;
                walletMasterObjDr.Balance = firstCurrObjDrWM.Balance;
                walletMasterObjDr.WalletName = firstCurrObjDrWM.Walletname;
                walletMasterObjDr.PublicAddress = firstCurrObjDrWM.PublicAddress;
                walletMasterObjDr.IsDefaultWallet = firstCurrObjDrWM.IsDefaultWallet;
                walletMasterObjDr.CoinName = firstCurrObj.Coin;
                walletMasterObjDr.OutBoundBalance = firstCurrObjDrWM.OutBoundBalance;


                WalletMasterResponse walletMasterObjDr1 = new WalletMasterResponse();
                walletMasterObjDr1.AccWalletID = secondCurrObjDrWM.AccWalletID;
                walletMasterObjDr1.Balance = secondCurrObjDrWM.Balance;
                walletMasterObjDr1.WalletName = secondCurrObjDrWM.Walletname;
                walletMasterObjDr1.PublicAddress = secondCurrObjDrWM.PublicAddress;
                walletMasterObjDr1.IsDefaultWallet = secondCurrObjDrWM.IsDefaultWallet;
                walletMasterObjDr1.CoinName = secondCurrObj.Coin;
                walletMasterObjDr1.OutBoundBalance = secondCurrObjDrWM.OutBoundBalance;

                ActivityNotificationMessage ActivityNotificationdr = new ActivityNotificationMessage();
                ActivityNotificationdr.MsgCode = Convert.ToInt32(enErrorCode.DebitWalletMsgNotification);
                ActivityNotificationdr.Param1 = firstCurrObj.Coin;
                ActivityNotificationdr.Param2 = firstCurrObj.debitObject.trnType.ToString();
                ActivityNotificationdr.Param3 = firstCurrObj.debitObject.RefGuid.ToString();
                ActivityNotificationdr.Type = Convert.ToInt16(EnNotificationType.Info);

                ActivityNotificationMessage ActivityNotificationdr1 = new ActivityNotificationMessage();
                ActivityNotificationdr1.MsgCode = Convert.ToInt32(enErrorCode.DebitWalletMsgNotification);
                ActivityNotificationdr1.Param1 = secondCurrObj.Coin;
                ActivityNotificationdr1.Param2 = secondCurrObj.debitObject.trnType.ToString();
                ActivityNotificationdr1.Param3 = secondCurrObj.debitObject.RefGuid.ToString();
                ActivityNotificationdr1.Type = Convert.ToInt16(EnNotificationType.Info);

                ActivityNotificationMessage ActivityNotificationCrChargeSec = new ActivityNotificationMessage();
                ActivityNotificationCrChargeSec.MsgCode = Convert.ToInt32(enErrorCode.ChargeDeductedWallet);
                ActivityNotificationCrChargeSec.Param1 = secondCurrObj.Coin;
                ActivityNotificationCrChargeSec.Param2 = ChargefirstCur.ToString();
                ActivityNotificationCrChargeSec.Param3 = secondCurrObj.creditObject.RefGuid.ToString();
                ActivityNotificationCrChargeSec.Type = Convert.ToInt16(EnNotificationType.Info);

                ActivityNotificationMessage ActivityNotificationDrChargeSec = new ActivityNotificationMessage();
                ActivityNotificationDrChargeSec.MsgCode = Convert.ToInt32(enErrorCode.ChargeDeductedWallet);
                ActivityNotificationDrChargeSec.Param1 = secondCurrObj.Coin;
                ActivityNotificationDrChargeSec.Param2 = ChargesecondCur.ToString();
                ActivityNotificationDrChargeSec.Param3 = secondCurrObj.debitObject.RefGuid.ToString();
                ActivityNotificationDrChargeSec.Type = Convert.ToInt16(EnNotificationType.Info);

                Task.Run(() => HelperForLog.WriteLogIntoFile("CreditNotificationSend Activity:Without Token", "MartinTransactionWalletService", "msg=" + ActivityNotificationdr.MsgCode.ToString() + ",User=" + firstCurrObjCrWM.UserID.ToString() + "WalletID" + firstCurrObjCrWM.AccWalletID + ",Balance" + firstCurrObjCrWM.Balance.ToString()));

                var firstCurrObjCrType = firstCurrObj.creditObject.trnType.ToString().Contains("Cr_") ? firstCurrObj.creditObject.trnType.ToString().Replace("Cr_", "") : firstCurrObj.creditObject.trnType.ToString().Replace("Dr_", "");
                var firstCurrObjDrType = firstCurrObj.debitObject.trnType.ToString().Contains("Cr_") ? firstCurrObj.debitObject.trnType.ToString().Replace("Cr_", "") : firstCurrObj.debitObject.trnType.ToString().Replace("Dr_", "");
                var secCurrObjCrType = secondCurrObj.creditObject.trnType.ToString().Contains("Cr_") ? secondCurrObj.creditObject.trnType.ToString().Replace("Cr_", "") : secondCurrObj.creditObject.trnType.ToString().Replace("Dr_", "");
                var secCurrObjDrType = secondCurrObj.debitObject.trnType.ToString().Contains("Cr_") ? secondCurrObj.debitObject.trnType.ToString().Replace("Cr_", "") : secondCurrObj.debitObject.trnType.ToString().Replace("Dr_", "");

                Parallel.Invoke(() => _signalRService.SendActivityNotificationV2(ActivityNotificationCr, firstCurrObjCrWM.UserID.ToString(), 2, firstCurrObj.creditObject.RefGuid + " timestamp : " + timestamp, 1),
                                           () => _signalRService.OnWalletBalChange(walletMasterObjCr, firstCurrObj.Coin, firstCurrObjCrWM.UserID.ToString(), 2, firstCurrObj.creditObject.RefGuid + " timestamp : " + timestamp, 1),
                                           () => _signalRService.SendActivityNotificationV2(ActivityNotificationCr1, secondCurrObjCrWM.UserID.ToString(), 2, secondCurrObj.creditObject.RefGuid + " timestamp : " + timestamp, 1),
                                           () => _signalRService.OnWalletBalChange(walletMasterObjCr1, secondCurrObj.Coin, secondCurrObjCrWM.UserID.ToString(), 2, secondCurrObj.creditObject.RefGuid + " timestamp : " + timestamp, 1),
                                           () => _signalRService.SendActivityNotificationV2(ActivityNotificationdr, firstCurrObjDrWM.UserID.ToString(), 2, firstCurrObj.debitObject.RefGuid + " timestamp : " + timestamp, 1),
                                           () => _signalRService.OnWalletBalChange(walletMasterObjDr, firstCurrObj.Coin, firstCurrObjDrWM.UserID.ToString(), 2, firstCurrObj.debitObject.RefGuid + " timestamp : " + timestamp, 1),
                                           () => _signalRService.SendActivityNotificationV2(ActivityNotificationdr1, secondCurrObjDrWM.UserID.ToString(), 2, secondCurrObj.debitObject.RefGuid + " timestamp : " + timestamp, 1),
                                           () => _signalRService.OnWalletBalChange(walletMasterObjDr1, secondCurrObj.Coin, secondCurrObjDrWM.UserID.ToString(), 2, secondCurrObj.debitObject.RefGuid + " timestamp : " + timestamp, 1),
                                           () => SMSSendAsyncV1(EnTemplateType.SMS_WalletCredited, firstCurrObjCrWM.UserID.ToString(), null, null, null, null, firstCurrObj.Coin, firstCurrObjCrType, firstCurrObj.creditObject.RefGuid.ToString()),
                                           () => SMSSendAsyncV1(EnTemplateType.SMS_WalletCredited, secondCurrObjCrWM.UserID.ToString(), null, null, null, null, secondCurrObj.Coin, secCurrObjCrType, secondCurrObj.creditObject.RefGuid.ToString()),
                                            () => SMSSendAsyncV1(EnTemplateType.SMS_WalletDebited, firstCurrObjDrWM.UserID.ToString(), null, null, null, null, firstCurrObj.Coin, firstCurrObjDrType, firstCurrObj.debitObject.RefGuid.ToString()),
                                            () => SMSSendAsyncV1(EnTemplateType.SMS_WalletDebited, secondCurrObjDrWM.UserID.ToString(), null, null, null, null, secondCurrObj.Coin, secCurrObjDrType, secondCurrObj.debitObject.RefGuid.ToString()),
                                            () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletCredited, secondCurrObjCrWM.UserID.ToString(), secondCurrObj.Amount.ToString(), secondCurrObj.Coin, Helpers.UTC_To_IST().ToString(), secondCurrObj.creditObject.RefGuid.ToString(), secCurrObjCrType),
                                            () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletCredited, firstCurrObjCrWM.UserID.ToString(), firstCurrObj.Amount.ToString(), firstCurrObj.Coin, Helpers.UTC_To_IST().ToString(), firstCurrObj.creditObject.RefGuid.ToString(), firstCurrObjCrType),
                                            () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, secondCurrObjDrWM.UserID.ToString(), secondCurrObj.Amount.ToString(), secondCurrObj.Coin, Helpers.UTC_To_IST().ToString(), secondCurrObj.debitObject.RefGuid.ToString(), secCurrObjDrType),
                                            () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, firstCurrObjDrWM.UserID.ToString(), firstCurrObj.Amount.ToString(), firstCurrObj.Coin, Helpers.UTC_To_IST().ToString(), firstCurrObj.debitObject.RefGuid.ToString(), firstCurrObjDrType));

                if (ChargefirstCur > 0 && ChargesecondCur > 0)
                {
                    Parallel.Invoke(() => _signalRService.SendActivityNotificationV2(ActivityNotificationCrChargeSec, firstCurrObjDrWM.UserID.ToString(), 2, "", 1),
                   () => _signalRService.SendActivityNotificationV2(ActivityNotificationDrChargeSec, firstCurrObjDrWM.UserID.ToString(), 2, "", 1),
                                            () => EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, firstCurrObjDrWM.UserID.ToString(), ChargefirstCur.ToString(), firstCurrObj.Coin, Helpers.UTC_To_IST().ToString(), firstCurrObj.debitObject.RefGuid.ToString(), "Deducted"),
                                            () => EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, secondCurrObjDrWM.UserID.ToString(), ChargesecondCur.ToString(), secondCurrObj.Coin, Helpers.UTC_To_IST().ToString(), secondCurrObj.debitObject.RefGuid.ToString(), "Deducted"));
                }
                #endregion
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreditNotificationSend" + "TimeStamp:" + timestamp, "MarginTransactionWalletService", ex);
            }
        }

        public async Task<bool> CheckUserBalanceAsync(decimal amount, long WalletId, enBalanceType enBalance = enBalanceType.AvailableBalance, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Margin_Trading_Wallet)
        {
            try
            {
                decimal crsum, drsum;
                decimal wObjBal;
                MarginWalletMaster walletObject;
                Task<decimal> crsumTask = _TransactionAccountsRepository.GetSumAsync(e => e.WalletID == WalletId && e.IsSettled == 1 && e.Type == enBalance, f => f.CrAmt);
                Task<decimal> drsumTask = _TransactionAccountsRepository.GetSumAsync(e => e.WalletID == WalletId && e.IsSettled == 1 && e.Type == enBalance, f => f.DrAmt);

                //2019-2-18 added condi for only used trading wallet
                Task<MarginWalletMaster> walletObjectTask = _commonRepository.GetSingleAsync(item => item.Id == WalletId && item.WalletUsageType == enWalletUsageType);
                crsum = await crsumTask;
                drsum = await drsumTask;
                walletObject = await walletObjectTask;
                //ntrivedi 13-02-2019 added so margin wallet do not use in other transaction
                if (walletObject.WalletUsageType != enWalletUsageType)
                {
                    HelperForLog.WriteLogIntoFileAsync("CheckUserBalanceAsync", "WalletId=" + WalletId.ToString() + "WalletUsageType Mismatching :" + enWalletUsageType);
                    return false;
                }
                decimal total = crsum - drsum;
                if (enBalance == enBalanceType.AvailableBalance)
                {
                    wObjBal = walletObject.Balance;
                }
                else if (enBalance == enBalanceType.OutBoundBalance)
                {
                    wObjBal = walletObject.OutBoundBalance;
                }
                else if (enBalance == enBalanceType.InBoundBalance)
                {
                    wObjBal = walletObject.InBoundBalance;
                }
                else
                {
                    return false;
                }
                if (wObjBal < 0) //ntrivedi 04-01-2018
                {
                    return false;
                }
                if (total == wObjBal && total >= 0)
                {
                    return true;
                }
                else
                {
                    HelperForLog.WriteLogIntoFileAsync("CheckUserBalance Reload Entity", "WalletId=" + walletObject.Id.ToString() + ",Total=" + total.ToString() + ",dbbalance=" + wObjBal.ToString());
                    _commonRepository.ReloadEntity(walletObject);
                    if (enBalance == enBalanceType.AvailableBalance)
                    {
                        wObjBal = walletObject.Balance;
                        HelperForLog.WriteLogIntoFileAsync("CheckUserBalance", "WalletId=" + WalletId.ToString() + ",Total=" + total.ToString() + ",dbbalance=" + wObjBal.ToString());
                    }
                    else if (enBalance == enBalanceType.OutBoundBalance)
                    {
                        wObjBal = walletObject.OutBoundBalance;
                        HelperForLog.WriteLogIntoFileAsync("CheckUserBalance OutBoundBalance", "WalletId=" + WalletId.ToString() + ",Total=" + total.ToString() + ",dbbalance=" + wObjBal.ToString());
                    }
                    else if (enBalance == enBalanceType.InBoundBalance)
                    {
                        wObjBal = walletObject.InBoundBalance;
                        HelperForLog.WriteLogIntoFileAsync("CheckUserBalance InBoundBalance", "WalletId=" + WalletId.ToString() + ",Total=" + total.ToString() + ",dbbalance=" + wObjBal.ToString());
                    }
                    else
                    {
                        return false;
                    }
                    if (total == wObjBal && total >= 0)
                    {
                        return true;
                    }
                    else
                    {
                        if (Math.Abs(total - wObjBal) % amount == 0)
                        {
                            return true;
                        }
                        HelperForLog.WriteLogIntoFileAsync("CheckUserBalance failed.", "Amount: " + amount.ToString() + "  WalletId=" + WalletId.ToString() + ",Total=" + total.ToString() + ",dbbalance=" + wObjBal.ToString());
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CheckUserBalanceAsync", "MartinTransactionWalletService", ex);
                throw ex;
            }
        }

        //Rushabh 26-10-2018
        public async Task<long> GetWalletID(string AccWalletID)
        {
            try
            {
                Task<MarginWalletMaster> obj1 = _commonRepository.GetSingleAsync(item => item.AccWalletID == AccWalletID);
                MarginWalletMaster obj = await obj1;
                if (obj != null)//Rita for object ref error
                    return obj.Id;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWalletID", this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<string> GetAccWalletID(long WalletID)
        {
            try
            {
                Task<MarginWalletMaster> obj1 = _commonRepository.GetSingleAsync(item => item.Id == WalletID);
                MarginWalletMaster obj = await obj1;
                if (obj != null)//Rita for object ref error
                    return obj.AccWalletID;
                else
                    return "";

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetAccWalletID", this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rita 9-1-19 need for social trading
        public async Task<string> GetDefaultAccWalletID(string SMSCode, long UserID)
        {
            try
            {
                MarginWalletTypeMaster obj1 = _WalletTypeMasterRepository.GetSingle(e => e.WalletTypeName == SMSCode);
                MarginWalletMaster obj = await _commonRepository.GetSingleAsync(item => item.WalletTypeID == obj1.Id && item.UserID == UserID && item.IsDefaultWallet == 1);

                if (obj != null)//Rita for object ref error
                    return obj.AccWalletID;
                else
                    return "";

            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("GetDefaultAccWalletID:##SMSCode " + SMSCode, "WalletServicce", ex));
                return "";
            }
        }

        public async Task<bool> ReleaseMarginWalletforSettleLeverageBalance(long BatchNo)
        {
            try
            {
                IEnumerable<MarginChargeOrder> OrderS = await _MarginChargeOrder.FindByAsync(e => e.BatchNo == BatchNo && e.Status == 0);

                foreach (MarginChargeOrder Order in OrderS)
                {
                    if (Order.MarginChargeCase == MarginChargeCase.InsufficinetLeverage_PlaceOrder)//Market order
                    {
                        await Task.Delay(new Random().Next(100, 900));//rita 10-5-19 as crone run multiple time
                        RespnseToWallet TxnResult = await _CreateOrder.PlaceMarketSELLOrder(EnAllowedChannels.WalletSystemInternal, Order.UserID, "9999999999", Order.PairID, Order.Amount, Order.DebitAccountID, Order.CreditAccountID, Order.Id.ToString(), "");
                        if (TxnResult.ReturnCode != enResponseCodeService.Success)
                        {
                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ReleaseMarginWalletforSettleLeverageBalance Market Order ", "MarginWalletServicce", "PlaceMarketSELLOrder Fail Msg:" + TxnResult.ReturnMsg + " ##BatchNo:" + BatchNo, Helpers.UTC_To_IST()));
                        }
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ReleaseMarginWalletforSettleLeverageBalance  Market Order", "MarginWalletServicce", "PlaceMarketSELLOrder Success ##GUID:" + TxnResult.Guid + " ##TrnRefNo:" + TxnResult.TrnNo + " ##BatchNo:" + BatchNo, Helpers.UTC_To_IST()));

                        Order.Status = 6;
                        Order.Guid = TxnResult.Guid.ToString();
                        Order.UpdatedDate = Helpers.UTC_To_IST();
                        _MarginChargeOrder.Update(Order);
                    }
                    else if (Order.MarginChargeCase == MarginChargeCase.InsufficientLeverage_ButLessLeverageMax)//Release order
                    {
                        await Task.Delay(new Random().Next(100, 900));//rita 10-5-19 as crone run multiple time
                        RespnseToWallet TxnResult = await _CreateOrder.ReleaseOrderForNoOpenPosition(EnAllowedChannels.WalletSystemInternal, Order.UserID, Order.Amount, BatchNo, Order.BaseCurrency, Order.Id.ToString(), "");
                        if (TxnResult.ReturnCode != enResponseCodeService.Success)
                        {
                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ReleaseMarginWalletforSettleLeverageBalance  Cancel Order", "MarginWalletServicce", "PlaceMarketSELLOrder Fail Msg:" + TxnResult.ReturnMsg + " ##BatchNo:" + BatchNo, Helpers.UTC_To_IST()));
                        }
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ReleaseMarginWalletforSettleLeverageBalance Cancel Order ", "MarginWalletServicce", "PlaceMarketSELLOrder Success ##GUID:" + TxnResult.Guid + " ##TrnRefNo:" + TxnResult.TrnNo + " ##BatchNo:" + BatchNo, Helpers.UTC_To_IST()));

                        Order.Status = 6;
                        Order.TrnRefNo = TxnResult.TrnNo;
                        Order.UpdatedDate = Helpers.UTC_To_IST();
                        _MarginChargeOrder.Update(Order);
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ReleaseMarginWalletforSettleLeverageBalance Internal Error:##BatchNo " + BatchNo, "MarginWalletServicce", ex);
                return false;
            }
        }
        public async Task<BizResponseClass> SettleMarketOrderForCharge(long ChargeID)
        {
            try
            {
                MarginChargeOrder marginChargeOrder;
                BizResponseClass bizResponse = new BizResponseClass();
                marginChargeOrder = _MarginChargeOrder.GetById(ChargeID);
                if (marginChargeOrder == null)
                {
                    bizResponse = new BizResponseClass { ErrorCode = enErrorCode.RecordNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SettleMarketOrderForCharge", "MarginTransactionWalletServicce", "SettleMarketOrderForCharge Fail Msg:" + Helpers.JsonSerialize(bizResponse) + " ChargeID=" + ChargeID, Helpers.UTC_To_IST()));
                    return bizResponse;
                }
                bizResponse = _walletSPRepositories.Callsp_MarginProcessLeverageAccountEOD(marginChargeOrder.LoanID, marginChargeOrder.BatchNo, 1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SettleMarketOrderForCharge", "MarginTransactionWalletServicce", "SettleMarketOrderForCharge after sp call Msg:" + Helpers.JsonSerialize(bizResponse) + " ChargeID=" + ChargeID, Helpers.UTC_To_IST()));
                return bizResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SettleMarketOrderForCharge", this.GetType().Name, ex);
                throw ex;
            }
        }
    }
}
