using System;
using System.Collections.Generic;
using Worldex.Core.Entities;
using Worldex.Core.ApiModels;
using Worldex.Infrastructure.Interfaces;
using Worldex.Core.Interfaces;
using Worldex.Core.Enums;
using Worldex.Infrastructure.DTOClasses;
using System.Threading.Tasks;
using Worldex.Core.ViewModels.WalletOperations;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.Entities.Wallet;
using System.Linq;
using Worldex.Core.Helpers;
using Microsoft.AspNetCore.Identity;
using Worldex.Core.Entities.User;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.ViewModels;
using Worldex.Infrastructure.BGTask;
using Worldex.Core.Entities.NewWallet;
using System.Text.RegularExpressions;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.Entities.Charges;

namespace Worldex.Infrastructure.Services
{
    public class WalletServiceV2 : IWalletServiceV2
    {
        #region DI
        private readonly ICommonRepository<WalletMaster> _commonRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessageService _messageService;
        private readonly IWalletRepository _walletRepository1;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private readonly ICommonRepository<WalletTypeMaster> _WalletTypeMasterRepository;
        private readonly ISignalRService _signalRService;
        private readonly ICommonWalletFunction _commonWalletFunction;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly IWalletSPRepositories _walletSPRepositories;
        private readonly IWalletTQInsert _WalletTQInsert;
        ISettlementWalletQueue<SettelementWalletReqRes> _SettlementEnqueue;
        #endregion

        #region Constructor
        public WalletServiceV2(ICommonRepository<WalletMaster> commonRepository,
            IWalletRepository walletRepository,
            ICommonRepository<WalletTypeMaster> WalletTypeMasterRepository, UserManager<ApplicationUser> userManager,
            IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, ISignalRService signalRService,
            ICommonWalletFunction commonWalletFunction,
            ICommonRepository<WithdrawHistory> WithdrawHistoryRepository, IWalletSPRepositories walletSPRepositories, IMessageService messageService, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue,
            IWalletTQInsert WalletTQInsert, ISettlementWalletQueue<SettelementWalletReqRes> SettlementEnqueue)
        {
            _userManager = userManager;
            _commonRepository = commonRepository;
            _pushNotificationsQueue = pushNotificationsQueue;
            _walletRepository1 = walletRepository;
            _WalletTypeMasterRepository = WalletTypeMasterRepository;
            _signalRService = signalRService;
            _commonWalletFunction = commonWalletFunction;
            _walletSPRepositories = walletSPRepositories;
            _messageService = messageService;
            _pushSMSQueue = pushSMSQueue;
            _WalletTQInsert = WalletTQInsert;
            _SettlementEnqueue = SettlementEnqueue;
        }
        #endregion

        public WalletTransactionQueue InsertIntoWalletTransactionQueue(Guid Guid, enWalletTranxOrderType TrnType, decimal Amount, long TrnRefNo, DateTime TrnDate, DateTime? UpdatedDate, long WalletID, string WalletType, long MemberID, string TimeStamp, enTransactionStatus Status, string StatusMsg, enWalletTrnType enWalletTrnType, string RefGuid)
        {
            try
            {
                WalletTransactionQueue walletTransactionQueue = new WalletTransactionQueue();
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
                walletTransactionQueue.RefGuid = RefGuid;
                return walletTransactionQueue;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<bool> CheckUserBalanceAsync(decimal amount, long WalletId, enBalanceType enBalance = enBalanceType.AvailableBalance, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet)
        {
            try
            {
                GetMemberBalRes getMemberBalRes = new GetMemberBalRes();
                getMemberBalRes = _walletSPRepositories.Callsp_GetMemberBalance(WalletId, 0, 0, Convert.ToInt16(enBalance), amount, Convert.ToInt32(enWalletUsageType));
                if (getMemberBalRes.ReturnCode == 0)
                {
                    return true;
                }
                else
                {
                    HelperForLog.WriteLogIntoFileAsync("CheckUserBalance failed.", "WalletService WalletID=" + WalletId.ToString(), Helpers.JsonSerialize(getMemberBalRes));
                    return false;

                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CheckUserBalanceAsync", "WalletService", ex);
                throw ex;
            }
        }

        public WalletDrCrResponse GetCrDRResponse(WalletDrCrResponse obj, string extras)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFile(extras, "WalletService", "timestamp:" + obj.TimeStamp + ",ReturnCode=" + obj.ReturnCode + ",ErrorCode=" + obj.ErrorCode + ", ReturnMsg=" + obj.ReturnMsg + ",StatusMsg=" + obj.StatusMsg + ",TrnNo=" + obj.TrnNo));
                return obj;
            }
            catch (Exception ex)
            {
                return obj;
            }
        }

        public async Task<WalletDrCrResponse> GetWalletDeductionNew(string coinName, string timestamp, enWalletTranxOrderType orderType, decimal amount, long userID, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, string Token = "", string Refguid = "")
        {
            try
            {
                WalletMaster dWalletobj;
                WalletTypeMaster walletTypeMaster;
                WalletTransactionQueue objTQ;
                WalletDrCrResponse resp = new WalletDrCrResponse();
                bool CheckUserBalanceFlag = false;
                long trnno = 0;
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "WalletService", "timestamp:" + timestamp + "," + "coinName:" + coinName + ",accWalletID=" + accWalletID + ",TrnRefNo=" + TrnRefNo.ToString() + ",userID=" + userID + ",amount=" + amount.ToString());

                Task<CheckTrnRefNoRes> countTask1 = _walletRepository1.CheckTranRefNoAsync(TrnRefNo, orderType, trnType);
                if (string.IsNullOrEmpty(accWalletID) || coinName == string.Empty || userID == 0)
                {
                    return new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidWalletOrUserIDorCoinName };
                }
                walletTypeMaster = _WalletTypeMasterRepository.GetSingle(e => e.WalletTypeName == coinName);
                if (walletTypeMaster == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidCoinName, TimeStamp = timestamp }, "Debit");
                }
                //2019-2-18 added condi for only used trading wallet
                Task<WalletMaster> dWalletobjTask = _commonRepository.GetSingleAsync(e => e.UserID == userID && e.WalletTypeID == walletTypeMaster.Id && e.AccWalletID == accWalletID && e.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (orderType != enWalletTranxOrderType.Debit) // sell 13-10-2018
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTrnType, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTrnType, ErrorCode = enErrorCode.InvalidTrnType, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                if (TrnRefNo == 0) // sell 13-10-2018
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTradeRefNo, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                if (amount <= 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidAmt, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmount, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                dWalletobj = await dWalletobjTask;
                bool flagTask = _walletRepository1.CheckUserBalanceV1(dWalletobj.Id);
                if (dWalletobj == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TimeStamp = timestamp }, "Debit");
                }
                if (dWalletobj.Status != 1 || dWalletobj.IsValid == false)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidWallet, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "CheckUserBalance pre Balance=" + dWalletobj.Balance.ToString() + ", TrnNo=" + TrnRefNo.ToString() + ",TimeStamp=" + timestamp);
                CheckUserBalanceFlag = flagTask;
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "CheckUserBalance Post TrnNo=" + TrnRefNo.ToString() + ",TimeStamp=" + timestamp);

                dWalletobj = _commonRepository.GetById(dWalletobj.Id); // ntrivedi fetching fresh balance for multiple request at a time 
                var msg = _commonWalletFunction.CheckWalletLimitAsyncV1(enWalletLimitType.WithdrawLimit, dWalletobj.Id, amount, TrnRefNo);

                if (dWalletobj.Balance < amount)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficantBal, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                if (!CheckUserBalanceFlag)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedBalanceMismatch, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }

                var limitres = await msg;
                if (limitres.ErrorCode != enErrorCode.Success)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletLimitExceed, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ReturnMsg = limitres.ReturnMsg,  //Uday 11-02-2019 Give Particular Limit Validation Message
                        ErrorCode = limitres.ErrorCode,
                        TrnNo = objTQ.TrnNo,
                        Status = objTQ.Status,
                        StatusMsg = objTQ.StatusMsg,
                        MinimumAmount = limitres.MinimumAmounts,
                        MaximumAmount = limitres.MaximumAmounts,
                        TimeStamp = timestamp
                    }, "Debit");
                }

                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "Check ShadowLimit done TrnNo=" + TrnRefNo.ToString() + ",TimeStamp=" + timestamp);

                CheckTrnRefNoRes count1 = await countTask1;
                if (count1.TotalCount != 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.AlredyExist, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AlredyExist, ErrorCode = enErrorCode.AlredyExist, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "CheckTrnRefNo TrnNo=" + TrnRefNo.ToString() + ",TimeStamp=" + timestamp);

                BizResponseClass bizResponse = _walletSPRepositories.Callsp_DebitWallet(dWalletobj, timestamp, serviceType, amount, coinName, EnAllowedChannels.Web, walletTypeMaster.Id, TrnRefNo, dWalletobj.Id, dWalletobj.UserID, routeTrnType, trnType, ref trnno, enWalletDeductionType.Normal);
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "WalletDeductionwithTQ sp call done TrnNo=" + TrnRefNo.ToString());

                if (bizResponse.ReturnCode != enResponseCode.Success)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = bizResponse.ReturnMsg, ErrorCode = bizResponse.ErrorCode, TrnNo = trnno, Status = 0, StatusMsg = "", TimeStamp = timestamp }, "Debit");
                }
                //2019-3-6 find chharge
                decimal charge = 0;
                string DeductWalletType = "";
                long ChargeWalletID = 0;
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

                    WalletMasterResponse ChargeWalletObj = new WalletMasterResponse();

                    charge = _walletRepository1.FindChargeValueDeduct(timestamp, TrnRefNo);
                    DeductWalletType = _walletRepository1.FindChargeCurrencyDeduct(TrnRefNo);
                    ChargeWalletID = _walletRepository1.FindChargeValueWalletId(timestamp, TrnRefNo);

                    HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew before", "Get walletid and currency walletid=" + ChargeWalletID.ToString() + "Currency : " + DeductWalletType.ToString() + "Charge: " + charge.ToString());

                    if (ChargeWalletID > 0 && (DeductWalletType != null || DeductWalletType != ""))
                    {
                        var ChargeWallet = _commonRepository.GetById(ChargeWalletID);
                        if (ChargeWallet != null)
                        {
                            HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew after", "Get walletid and currency walletid=" + ChargeWalletID.ToString() + "Currency : " + DeductWalletType.ToString() + "Charge: " + charge.ToString());
                            ChargeWalletObj.AccWalletID = ChargeWallet.AccWalletID;
                            ChargeWalletObj.Balance = ChargeWallet.Balance;
                            ChargeWalletObj.WalletName = ChargeWallet.Walletname;
                            ChargeWalletObj.PublicAddress = ChargeWallet.PublicAddress;
                            ChargeWalletObj.IsDefaultWallet = ChargeWallet.IsDefaultWallet;
                            ChargeWalletObj.CoinName = DeductWalletType;
                            ChargeWalletObj.OutBoundBalance = ChargeWallet.OutBoundBalance;
                        }
                    }
                    Task.Run(() => WalletDeductionNewNotificationSend(timestamp, dWalletobj, coinName, amount, Refguid, (byte)routeTrnType, userID, Token, trnType.ToString(), walletMasterObj, charge, DeductWalletType, ChargeWalletObj));

                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog("GetWalletDeductionNew Charge notification", "WalletService", ex);
                }

                return GetCrDRResponse(new WalletDrCrResponse { ChargeCurrency = DeductWalletType, Charge = charge, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDebit, ErrorCode = enErrorCode.Success, TrnNo = trnno, Status = enTransactionStatus.Hold, StatusMsg = bizResponse.ReturnMsg }, "Debit");
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWalletDeductionNew", "WalletService", ex);
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError, TrnNo = 0, Status = 0, StatusMsg = "", TimeStamp = timestamp }, "Debit");
            }
        }

        public async Task<WalletDrCrResponse> GetReleaseHoldNew(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, string Token = "", string Refguid = "")
        {
            try
            {
                WalletMaster dWalletobj;
                WalletTypeMaster walletTypeMaster;
                WalletTransactionQueue objTQ;
                WalletDrCrResponse resp = new WalletDrCrResponse();
                bool CheckUserBalanceFlag = false;
                enWalletTranxOrderType orderType = enWalletTranxOrderType.Credit; //ntrivedi release is credit process (reverse hold)
                long userID = 0, TrnNo = 0;

                HelperForLog.WriteLogIntoFileAsync("GetReleaseHoldNew", "WalletService", "timestamp:" + timestamp + "," + "coinName:" + coinName + ",accWalletID=" + accWalletID + ",TrnRefNo=" + TrnRefNo.ToString() + ",userID=" + userID + ",amount=" + amount.ToString());

                if (string.IsNullOrEmpty(accWalletID) || coinName == string.Empty)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidWalletOrUserIDorCoinName, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
                walletTypeMaster = _WalletTypeMasterRepository.GetSingle(e => e.WalletTypeName == coinName);
                if (walletTypeMaster == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidCoinName, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }

                //2019-2-18 added condi for only used trading wallet
                Task<WalletMaster> dWalletobjTask = _commonRepository.GetSingleAsync(e => e.WalletTypeID == walletTypeMaster.Id && e.AccWalletID == accWalletID && e.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

                if (TrnRefNo == 0) // sell 13-10-2018
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTradeRefNo, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
                if (amount <= 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidAmt, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmount, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
                dWalletobj = await dWalletobjTask;
                if (dWalletobj == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
                userID = dWalletobj.UserID;
                Task<bool> flagTask = CheckUserBalanceAsync(amount, dWalletobj.Id);

                if (dWalletobj.Status != 1 || dWalletobj.IsValid == false)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidWallet, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }

                HelperForLog.WriteLogIntoFileAsync("GetReleaseHoldNew", "CheckUserBalance pre Balance=" + dWalletobj.Balance.ToString() + ", TrnNo=" + TrnRefNo.ToString());
                CheckUserBalanceFlag = await flagTask;

                HelperForLog.WriteLogIntoFileAsync("GetReleaseHoldNew", "CheckUserBalance Post TrnNo=" + TrnRefNo.ToString());
                dWalletobj = _commonRepository.GetById(dWalletobj.Id); // ntrivedi fetching fresh balance for multiple request at a time 

                if (!CheckUserBalanceFlag)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedBalanceMismatch, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
                Task<bool> flagTask1 = CheckUserBalanceAsync(amount, dWalletobj.Id, enBalanceType.OutBoundBalance);
                CheckUserBalanceFlag = await flagTask1;
                if (!CheckUserBalanceFlag)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedOutgoingBalanceMismatch, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
                HelperForLog.WriteLogIntoFileAsync("GetReleaseHoldNew", "before Check ShadowLimit TrnNo=" + TrnRefNo.ToString());

                if (dWalletobj.OutBoundBalance < amount)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficientOutboundBalance, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
                HelperForLog.WriteLogIntoFileAsync("GetReleaseHoldNew", "CheckTrnRefNo TrnNo=" + TrnRefNo.ToString());

                BizResponseClass bizResponse = _walletSPRepositories.Callsp_ReleaseHoldWallet(dWalletobj, timestamp, serviceType, amount, coinName, allowedChannels, walletTypeMaster.Id, TrnRefNo, dWalletobj.Id, dWalletobj.UserID, routeTrnType, trnType, ref TrnNo);

                if (bizResponse.ReturnCode == enResponseCode.Success)
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

                        HelperForLog.WriteLogIntoFileAsync("GetReleaseHoldNew", "OnWalletBalChange + SendActivityNotificationV2 pre TrnNo=" + TrnRefNo.ToString());

                        Task.Run(() => Parallel.Invoke(() => SendNotification(Convert.ToInt32(enErrorCode.HoldBalanceReleaseNotification), dWalletobj.UserID.ToString(), Convert.ToInt16(EnNotificationType.Info), coinName, amount.ToString(), Refguid.ToString()),
                            () => _signalRService.OnWalletBalChange(walletMasterObj, coinName, dWalletobj.UserID.ToString(), 2)
                           ));

                        decimal charge = _walletRepository1.FindChargeValueDeduct(timestamp, TrnRefNo);
                        var ChargewalletId = _walletRepository1.FindChargeValueReleaseWalletId(timestamp, TrnRefNo);
                        WalletMaster ChargeWalletObj = null;
                        if (charge > 0 && ChargewalletId > 0)
                        {
                            ChargeWalletObj = _commonRepository.GetById(ChargewalletId);
                            if (ChargeWalletObj != null)
                            {
                                var ChargewalletType = _WalletTypeMasterRepository.GetSingle(i => i.Id == ChargeWalletObj.WalletTypeID);
                                if (ChargewalletType != null)
                                {
                                    WalletMasterResponse walletMasterChargeObj = new WalletMasterResponse();
                                    walletMasterObj.AccWalletID = ChargeWalletObj.AccWalletID;
                                    walletMasterObj.Balance = ChargeWalletObj.Balance;
                                    walletMasterObj.WalletName = ChargeWalletObj.Walletname;
                                    walletMasterObj.PublicAddress = ChargeWalletObj.PublicAddress;
                                    walletMasterObj.IsDefaultWallet = ChargeWalletObj.IsDefaultWallet;
                                    walletMasterObj.CoinName = coinName;
                                    walletMasterObj.OutBoundBalance = ChargeWalletObj.OutBoundBalance;

                                    Parallel.Invoke(() => _commonWalletFunction.EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, dWalletobj.UserID.ToString(), charge.ToString(), ChargewalletType.WalletTypeName, Helpers.UTC_To_IST().ToString(), Refguid.ToString(), "released"),
                                  () => SendNotification(Convert.ToInt32(enErrorCode.ChargeReleasedWallet), dWalletobj.UserID.ToString(), Convert.ToInt16(EnNotificationType.Info), ChargewalletType.WalletTypeName, charge.ToString(), Refguid.ToString()),
                                 () => _signalRService.OnWalletBalChange(walletMasterChargeObj, coinName, dWalletobj.UserID.ToString(), 2));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        HelperForLog.WriteErrorLog("GetReleaseHoldNew Charge Noti Timestamp:" + timestamp, "WalletService", ex);
                    }
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDebit, ErrorCode = enErrorCode.Success, TrnNo = TrnNo, Status = enTransactionStatus.Hold, StatusMsg = bizResponse.ReturnMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");

                }
                else
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = bizResponse.ReturnMsg, ErrorCode = bizResponse.ErrorCode, TrnNo = TrnNo, Status = enTransactionStatus.Initialize, StatusMsg = bizResponse.ReturnMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetReleaseHoldNew Timestamp:" + timestamp, "WalletService", ex);
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError, TrnNo = 0, Status = 0, StatusMsg = "", TimeStamp = timestamp }, "GetReleaseHoldNew");
            }
        }

        public async Task<WalletDrCrResponse> GetWalletHoldNew(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, string Token = "", enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal, string Refguid = "")
        {
            try
            {
                WalletMaster dWalletobj;
                WalletTypeMaster walletTypeMaster;
                WalletTransactionQueue objTQ;
                WalletDrCrResponse resp = new WalletDrCrResponse();
                bool CheckUserBalanceFlag = false;
                enWalletTranxOrderType orderType = enWalletTranxOrderType.Credit;
                long userID = 0, TrnNo = 0;

                HelperForLog.WriteLogIntoFileAsync("GetWalletHoldNew", "WalletService", "timestamp:" + timestamp + "," + "coinName:" + coinName + ",accWalletID=" + accWalletID + ",TrnRefNo=" + TrnRefNo.ToString() + ",userID=" + userID + ",amount=" + amount.ToString());

                Task<CheckTrnRefNoRes> countTask1 = _walletRepository1.CheckTranRefNoAsync(TrnRefNo, orderType, trnType);
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
                Task<WalletMaster> dWalletobjTask = _commonRepository.GetSingleAsync(e => e.WalletTypeID == walletTypeMaster.Id && e.AccWalletID == accWalletID && e.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

                if (TrnRefNo == 0) // sell 13-10-2018
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTradeRefNo, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }
                if (amount <= 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidAmt, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmount, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }
                dWalletobj = await dWalletobjTask;
                if (dWalletobj == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TimeStamp = timestamp }, "Debit");
                }
                userID = dWalletobj.UserID;
                var flagTask = CheckUserBalanceAsync(amount, dWalletobj.Id);
                if (dWalletobj.Status != 1 || dWalletobj.IsValid == false)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidWallet, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }

                HelperForLog.WriteLogIntoFileAsync("GetWalletHoldNew", "CheckUserBalance pre Balance=" + dWalletobj.Balance.ToString() + ", TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);
                CheckUserBalanceFlag = await flagTask;

                HelperForLog.WriteLogIntoFileAsync("GetWalletHoldNew", "CheckUserBalance Post TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);
                dWalletobj = _commonRepository.GetById(dWalletobj.Id); // ntrivedi fetching fresh balance for multiple request at a time 
                if (dWalletobj.Balance < amount)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficantBal, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }

                if (!CheckUserBalanceFlag)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedBalanceMismatch, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }

                HelperForLog.WriteLogIntoFileAsync("GetWalletHoldNew", "Check ShadowLimit done TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);
                //int count = await countTask;
                CheckTrnRefNoRes count1 = await countTask1;
                if (count1.TotalCount != 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.AlredyExist, trnType, Refguid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AlredyExist, ErrorCode = enErrorCode.AlredyExist, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }

                HelperForLog.WriteLogIntoFileAsync("GetWalletHoldNew", "CheckTrnRefNo TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);

                BizResponseClass bizResponse = _walletSPRepositories.Callsp_HoldWallet(dWalletobj, timestamp, serviceType, amount, coinName, allowedChannels, walletTypeMaster.Id, TrnRefNo, dWalletobj.Id, dWalletobj.UserID, routeTrnType, trnType, ref TrnNo, enWalletDeductionType, Refguid);

                decimal charge = 0;
                WalletTypeMaster ChargewalletType = null;
                if (bizResponse.ReturnCode == enResponseCode.Success)
                {
                    try
                    {
                        charge = _walletRepository1.FindChargeValueHold(timestamp, TrnRefNo);
                        long walletId = _walletRepository1.FindChargeValueWalletId(timestamp, TrnRefNo);
                        WalletMaster ChargeWalletObj = null;

                        if (charge > 0 && walletId > 0)
                        {
                            ChargeWalletObj = _commonRepository.GetById(walletId);
                            ChargewalletType = _WalletTypeMasterRepository.GetSingle(i => i.Id == ChargeWalletObj.WalletTypeID);
                        }
                        Task.Run(() => WalletHoldNotificationSend(timestamp, dWalletobj, coinName, amount, Refguid, (byte)routeTrnType, charge, walletId, ChargeWalletObj, ChargewalletType));
                    }
                    catch (Exception ex)
                    {
                        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + "Timestamp:" + timestamp, this.GetType().Name, ex);
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

        public async Task<WalletDrCrResponse> GetWalletCreditDrForHoldNewAsyncFinal(CommonClassCrDr firstCurrObj, CommonClassCrDr secondCurrObj, string timestamp, enServiceType serviceType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal)
        {
            try
            {
                WalletTransactionQueue tqObj;
                WalletMaster firstCurrObjCrWM, firstCurrObjDrWM, secondCurrObjCrWM, secondCurrObjDrWM;
                WalletTypeMaster walletTypeFirstCurr, walletTypeSecondCurr;
                bool checkDebitRefNo, checkDebitRefNo1;
                Task<bool> checkDebitRefNoTask, checkDebitRefNoTask1;
                BizResponseClass bizResponseClassFC, bizResponseClassSC;

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal first currency", "WalletService", "timestamp:" + timestamp + Helpers.JsonSerialize(firstCurrObj)));
                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal second currency", "WalletService", "timestamp:" + timestamp + Helpers.JsonSerialize(secondCurrObj)));

                var firstCurrObjCrWMTask = _commonRepository.GetSingleAsync(item => item.Id == firstCurrObj.creditObject.WalletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (firstCurrObj.debitObject.isMarketTrade == 1)
                {
                    checkDebitRefNoTask = _walletRepository1.CheckTrnIDDrForMarketAsync(firstCurrObj);
                }
                else
                {
                    checkDebitRefNoTask = _walletRepository1.CheckTrnIDDrForHoldAsync(firstCurrObj);
                }
                //2019-2-18 added condi for only used trading wallet
                var firstCurrObjDrWMTask = _commonRepository.GetSingleAsync(item => item.Id == firstCurrObj.debitObject.WalletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                // to solve second operation started error solving 04-03-2019 ntrivedi await before query in same repository
                checkDebitRefNo = await checkDebitRefNoTask;
                if (checkDebitRefNo == false)//fail
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObj.creditObject.WalletId, firstCurrObj.Coin, firstCurrObj.creditObject.UserID, timestamp, enTransactionStatus.SystemFail, "Amount and DebitRefNo matching failure", firstCurrObj.creditObject.trnType, firstCurrObj.debitObject.RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNoFirCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                }
                if (secondCurrObj.debitObject.isMarketTrade == 1)
                {
                    checkDebitRefNoTask1 = _walletRepository1.CheckTrnIDDrForMarketAsync(secondCurrObj);
                }
                else
                {
                    checkDebitRefNoTask1 = _walletRepository1.CheckTrnIDDrForHoldAsync(secondCurrObj);
                }
                //2019-2-18 added condi for only used trading wallet
                var secondCurrObjCrWMTask = _commonRepository.GetSingleAsync(item => item.Id == secondCurrObj.creditObject.WalletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

                //2019-2-18 added condi for only used trading wallet
                var secondCurrObjDrWMTask = _commonRepository.GetSingleAsync(item => item.Id == secondCurrObj.debitObject.WalletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

                Task<WalletTypeMaster> walletTypeFirstCurrTask = _WalletTypeMasterRepository.GetSingleAsync(e => e.WalletTypeName == firstCurrObj.Coin);
                firstCurrObjCrWM = await firstCurrObjCrWMTask;
                firstCurrObj.creditObject.UserID = firstCurrObjCrWM.UserID;

                firstCurrObjDrWM = await firstCurrObjDrWMTask;
                firstCurrObj.debitObject.UserID = firstCurrObjDrWM.UserID;

                Task<WalletTypeMaster> walletTypeSecondCurrTask = _WalletTypeMasterRepository.GetSingleAsync(e => e.WalletTypeName == secondCurrObj.Coin);
                firstCurrObjCrWM = await firstCurrObjCrWMTask;
                firstCurrObj.creditObject.UserID = firstCurrObjCrWM.UserID;

                firstCurrObjDrWM = await firstCurrObjDrWMTask;
                firstCurrObj.debitObject.UserID = firstCurrObjDrWM.UserID;

                firstCurrObjCrWM = await firstCurrObjCrWMTask;
                firstCurrObj.creditObject.UserID = firstCurrObjCrWM.UserID;

                firstCurrObjDrWM = await firstCurrObjDrWMTask;
                firstCurrObj.debitObject.UserID = firstCurrObjDrWM.UserID;

                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("GetWalletCreditDrForHoldNewAsyncFinal before await1", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));


                firstCurrObjCrWM = await firstCurrObjCrWMTask;
                if (firstCurrObjCrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjCrWM.Id, secondCurrObj.Coin, firstCurrObjCrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, secondCurrObj.debitObject.trnType, secondCurrObj.debitObject.RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.FirstCurrCrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                firstCurrObj.creditObject.UserID = firstCurrObjCrWM.UserID;

                firstCurrObjDrWM = await firstCurrObjDrWMTask;
                if (firstCurrObjDrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, secondCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, secondCurrObj.debitObject.trnType, secondCurrObj.debitObject.RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.FirstCurrDrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }
                firstCurrObj.debitObject.UserID = firstCurrObjDrWM.UserID;

                secondCurrObjCrWM = await secondCurrObjCrWMTask;
                if (secondCurrObjCrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjCrWM.Id, secondCurrObj.Coin, secondCurrObjCrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, secondCurrObj.debitObject.trnType, secondCurrObj.debitObject.RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SecondCurrCrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }
                secondCurrObj.creditObject.UserID = secondCurrObjCrWM.UserID;

                secondCurrObjDrWM = await secondCurrObjDrWMTask;
                if (secondCurrObjDrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, secondCurrObj.debitObject.trnType, secondCurrObj.debitObject.RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SecondCurrDrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }
                secondCurrObj.debitObject.UserID = secondCurrObjDrWM.UserID;

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal after await1", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));


                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal before await2", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));

                checkDebitRefNo1 = await checkDebitRefNoTask1;
                if (checkDebitRefNo1 == false)//fail
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObj.creditObject.WalletId, secondCurrObj.Coin, secondCurrObj.creditObject.UserID, timestamp, enTransactionStatus.SystemFail, "Amount and DebitRefNo matching failure", secondCurrObj.creditObject.trnType, secondCurrObj.debitObject.RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNoSecCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                }

                if (firstCurrObj.debitObject.isMarketTrade == 1 && firstCurrObj.debitObject.differenceAmount > 0)
                {
                    if (firstCurrObjDrWM.Balance < firstCurrObj.debitObject.differenceAmount)
                    {
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, firstCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType, firstCurrObj.debitObject.RefGuid);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficientMarketInternalBalanceCheckFirstCurrencyForDifferentialAmountFailed, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                    bizResponseClassFC = _walletSPRepositories.Callsp_HoldWallet_MarketTrade(firstCurrObjDrWM, timestamp, serviceType, firstCurrObj.debitObject.differenceAmount, firstCurrObj.Coin, allowedChannels, firstCurrObjDrWM.WalletTypeID, firstCurrObj.debitObject.WTQTrnNo, firstCurrObj.debitObject.WalletId, firstCurrObj.debitObject.UserID, enTrnType.Buy_Trade, firstCurrObj.debitObject.trnType, enWalletDeductionType.Market);
                    if (bizResponseClassFC.ReturnCode != enResponseCode.Success)
                    {
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, firstCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType, firstCurrObj.debitObject.RefGuid);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.FirstCurrDifferentialAmountHoldFailed, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                }
                if (secondCurrObj.debitObject.isMarketTrade == 1 && secondCurrObj.debitObject.differenceAmount > 0)
                {
                    if (secondCurrObjDrWM.Balance < secondCurrObj.debitObject.differenceAmount)
                    {
                        // insert with status=2 system failed
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType, firstCurrObj.debitObject.RefGuid);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficientMarketInternalBalanceCheckSecondCurrencyForDifferentialAmountFailed, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                    bizResponseClassSC = _walletSPRepositories.Callsp_HoldWallet_MarketTrade(secondCurrObjDrWM, timestamp, serviceType, secondCurrObj.debitObject.differenceAmount, secondCurrObj.Coin, allowedChannels, secondCurrObjDrWM.WalletTypeID, secondCurrObj.debitObject.WTQTrnNo, secondCurrObj.debitObject.WalletId, secondCurrObj.debitObject.UserID, enTrnType.Buy_Trade, secondCurrObj.debitObject.trnType, enWalletDeductionType.Market);
                    if (bizResponseClassSC.ReturnCode != enResponseCode.Success)
                    {
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, firstCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType, firstCurrObj.debitObject.RefGuid);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.SecondCurrDifferentialAmountHoldFailed, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                }

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal after await2", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));

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
                walletTypeFirstCurr = await walletTypeFirstCurrTask;
                walletTypeSecondCurr = await walletTypeSecondCurrTask;

                if (walletTypeFirstCurr == null || walletTypeSecondCurr == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidCoinName }, "Credit");
                }

                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("GetWalletCreditDrForHoldNewAsyncFinal before await3", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));


                if (firstCurrObjDrWM.OutBoundBalance < firstCurrObj.Amount) // ntrivedi checking outbound balance
                {
                    // insert with status=2 system failed
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, firstCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType, firstCurrObj.debitObject.RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficientOutgoingBalFirstCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                if (secondCurrObjDrWM.OutBoundBalance < secondCurrObj.Amount)// ntrivedi checking outbound balance
                {
                    // insert with status=2 system failed
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, secondCurrObj.debitObject.trnType, secondCurrObj.debitObject.RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficietOutgoingBalSecondCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                if (firstCurrObjDrWM.Status != 1)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, secondCurrObj.debitObject.trnType, secondCurrObj.debitObject.RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.FirstCurrWalletStatusDisable, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                if (secondCurrObjDrWM.Status != 1)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, secondCurrObj.debitObject.trnType, secondCurrObj.debitObject.RefGuid);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SecondCurrWalletStatusDisable, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal after await3", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));

                BizResponseClass bizResponse = _walletSPRepositories.Callsp_CrDrWalletForHold(firstCurrObj, secondCurrObj, timestamp, serviceType, walletTypeFirstCurr.Id, walletTypeSecondCurr.Id, (long)allowedChannels);

                _walletRepository1.ReloadEntity(firstCurrObjCrWM, secondCurrObjCrWM, firstCurrObjDrWM, secondCurrObjDrWM);

                if (bizResponse.ReturnCode != enResponseCode.Success)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = bizResponse.ReturnMsg, ErrorCode = bizResponse.ErrorCode, TrnNo = 0, Status = enTransactionStatus.Initialize, StatusMsg = bizResponse.ReturnMsg, TimeStamp = timestamp }, "Credit");
                }
                decimal ChargefirstCur = 0, ChargesecondCur = 0;
                //ntrivedi added for try catch 05-03-2019
                try
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal before WaitAll", "WalletService", "timestamp:" + timestamp));
                    Task.WaitAll();
                    Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal after WaitAll", "WalletService", "timestamp:" + timestamp));
                    ChargefirstCur = _walletRepository1.FindChargeValueDeduct(timestamp, secondCurrObj.creditObject.TrnRefNo);
                    ChargesecondCur = _walletRepository1.FindChargeValueDeduct(timestamp, secondCurrObj.debitObject.TrnRefNo);
                    secondCurrObj.debitObject.Charge = ChargesecondCur;
                    firstCurrObj.debitObject.Charge = ChargefirstCur;
                }
                catch (Exception ex1)
                {
                    HelperForLog.WriteErrorLog("GetWalletCreditDrForHoldNewAsyncFinal charge exception  Timestamp" + timestamp, this.GetType().Name, ex1);
                }

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal after Wallet operation", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));

                Task.Run(() => CreditDebitNotificationSend(timestamp, firstCurrObj, secondCurrObj, firstCurrObjCrWM, firstCurrObjDrWM, secondCurrObjCrWM, secondCurrObjCrWM, ChargefirstCur, ChargesecondCur));

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal:Without Token done", "WalletService", ",timestamp =" + timestamp));
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessCredit, ErrorCode = enErrorCode.Success, TrnNo = 0, Status = 0, StatusMsg = "", TimeStamp = timestamp }, "GetWalletCreditDrForHoldNewAsyncFinal");
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWalletCreditDrForHoldNewAsyncFinal Timestamp" + timestamp, this.GetType().Name, ex);
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError, TrnNo = 0, Status = 0, StatusMsg = "", TimeStamp = timestamp }, "GetWalletCreditDrForHoldNewAsyncFinal");
                //throw ex;
            }
        }

        #region       
        //public async Task<WalletDrCrResponse> GetWalletCreditDrForHoldNewAsyncFinal(CommonClassCrDr firstCurrObj, CommonClassCrDr secondCurrObj, string timestamp, enServiceType serviceType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal)
        //{
        //    try
        //    {
        //        var obj = new SettelementWalletReqRes();
        //        obj.enWalletDeductionType = enWalletDeductionType;
        //        obj.firstCurrObj = firstCurrObj;
        //        obj.secondCurrObj = secondCurrObj;
        //        obj.timestamp = timestamp;
        //        obj.serviceType = serviceType;
        //        obj.enAllowedChannels = allowedChannels;
        //        _SettlementEnqueue.Enqueue(obj);
        //        return new WalletDrCrResponse() { };
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog("GetWalletCreditDrForHoldNewAsyncFinal Timestamp" + timestamp, this.GetType().Name, ex);
        //        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError, TrnNo = 0, Status = 0, StatusMsg = "", TimeStamp = timestamp }, "GetWalletCreditDrForHoldNewAsyncFinal");
        //    }
        //}
        #endregion
        public async Task CreditDebitNotificationSend(string timestamp, CommonClassCrDr firstCurrObj, CommonClassCrDr secondCurrObj, WalletMaster firstCurrObjCrWM, WalletMaster firstCurrObjDrWM, WalletMaster secondCurrObjCrWM, WalletMaster secondCurrObjDrWM, decimal ChargefirstCur, decimal ChargesecondCur)
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

                var firstCurrObjCrType = firstCurrObj.creditObject.trnType.ToString().Contains("Cr_") ? firstCurrObj.creditObject.trnType.ToString().Replace("Cr_", "") : firstCurrObj.creditObject.trnType.ToString().Replace("Dr_", "");
                var firstCurrObjDrType = firstCurrObj.debitObject.trnType.ToString().Contains("Cr_") ? firstCurrObj.debitObject.trnType.ToString().Replace("Cr_", "") : firstCurrObj.debitObject.trnType.ToString().Replace("Dr_", "");
                var secCurrObjCrType = secondCurrObj.creditObject.trnType.ToString().Contains("Cr_") ? secondCurrObj.creditObject.trnType.ToString().Replace("Cr_", "") : secondCurrObj.creditObject.trnType.ToString().Replace("Dr_", "");
                var secCurrObjDrType = secondCurrObj.debitObject.trnType.ToString().Contains("Cr_") ? secondCurrObj.debitObject.trnType.ToString().Replace("Cr_", "") : secondCurrObj.debitObject.trnType.ToString().Replace("Dr_", "");

                Parallel.Invoke(() => SendNotification(Convert.ToInt32(enErrorCode.CreditWalletMsgNotification), firstCurrObjCrWM.UserID.ToString(), Convert.ToInt16(EnNotificationType.Info), firstCurrObj.Coin, firstCurrObj.creditObject.trnType.ToString(), firstCurrObj.debitObject.RefGuid.ToString() + " timestamp : " + timestamp),
                                           () => _signalRService.OnWalletBalChange(walletMasterObjCr, firstCurrObj.Coin, firstCurrObjCrWM.UserID.ToString(), 2, firstCurrObj.creditObject.RefGuid + " timestamp : " + timestamp),
                                           () => SendNotification(Convert.ToInt32(enErrorCode.DebitWalletMsgNotification), firstCurrObjDrWM.UserID.ToString(), Convert.ToInt16(EnNotificationType.Info), firstCurrObj.Coin, firstCurrObj.debitObject.trnType.ToString(), firstCurrObj.creditObject.RefGuid.ToString() + " timestamp : " + timestamp),
                                           () => _signalRService.OnWalletBalChange(walletMasterObjCr1, secondCurrObj.Coin, secondCurrObjCrWM.UserID.ToString(), 2, secondCurrObj.creditObject.RefGuid + " timestamp : " + timestamp),
                                           () => SendNotification(Convert.ToInt32(enErrorCode.CreditWalletMsgNotification), secondCurrObjCrWM.UserID.ToString(), Convert.ToInt16(EnNotificationType.Info), secondCurrObj.Coin, secondCurrObj.creditObject.trnType.ToString(), secondCurrObj.creditObject.RefGuid.ToString() + " timestamp : " + timestamp),
                                           () => _signalRService.OnWalletBalChange(walletMasterObjDr, firstCurrObj.Coin, firstCurrObjDrWM.UserID.ToString(), 2, firstCurrObj.debitObject.RefGuid + " timestamp : " + timestamp),
                                           () => SendNotification(Convert.ToInt32(enErrorCode.CreditWalletMsgNotification), secondCurrObjCrWM.UserID.ToString(), Convert.ToInt16(EnNotificationType.Info), secondCurrObj.Coin, secondCurrObj.creditObject.trnType.ToString(), secondCurrObj.creditObject.RefGuid.ToString() + " timestamp : " + timestamp),
                                           () => _signalRService.OnWalletBalChange(walletMasterObjDr1, secondCurrObj.Coin, secondCurrObjDrWM.UserID.ToString(), 2, secondCurrObj.debitObject.RefGuid + " timestamp : " + timestamp),
                                           () => _commonWalletFunction.SMSSendAsyncV1(EnTemplateType.SMS_WalletCredited, firstCurrObjCrWM.UserID.ToString(), null, null, null, null, firstCurrObj.Coin, firstCurrObjCrType, firstCurrObj.creditObject.RefGuid.ToString()),
                                           () => _commonWalletFunction.SMSSendAsyncV1(EnTemplateType.SMS_WalletCredited, secondCurrObjCrWM.UserID.ToString(), null, null, null, null, secondCurrObj.Coin, secCurrObjCrType, secondCurrObj.creditObject.RefGuid.ToString()),
                                            () => _commonWalletFunction.SMSSendAsyncV1(EnTemplateType.SMS_WalletDebited, firstCurrObjDrWM.UserID.ToString(), null, null, null, null, firstCurrObj.Coin, firstCurrObjDrType, firstCurrObj.debitObject.RefGuid.ToString()),
                                            () => _commonWalletFunction.SMSSendAsyncV1(EnTemplateType.SMS_WalletDebited, secondCurrObjDrWM.UserID.ToString(), null, null, null, null, secondCurrObj.Coin, secCurrObjDrType, secondCurrObj.debitObject.RefGuid.ToString()),
                                            () => _commonWalletFunction.EmailSendAsyncV1(EnTemplateType.EMAIL_WalletCredited, secondCurrObjCrWM.UserID.ToString(), secondCurrObj.Amount.ToString(), secondCurrObj.Coin, Helpers.UTC_To_IST().ToString(), secondCurrObj.creditObject.RefGuid.ToString(), secCurrObjCrType),
                                            () => _commonWalletFunction.EmailSendAsyncV1(EnTemplateType.EMAIL_WalletCredited, firstCurrObjCrWM.UserID.ToString(), firstCurrObj.Amount.ToString(), firstCurrObj.Coin, Helpers.UTC_To_IST().ToString(), firstCurrObj.creditObject.RefGuid.ToString(), firstCurrObjCrType),
                                            () => _commonWalletFunction.EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, secondCurrObjDrWM.UserID.ToString(), secondCurrObj.Amount.ToString(), secondCurrObj.Coin, Helpers.UTC_To_IST().ToString(), secondCurrObj.debitObject.RefGuid.ToString(), secCurrObjDrType),
                                            () => _commonWalletFunction.EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, firstCurrObjDrWM.UserID.ToString(), firstCurrObj.Amount.ToString(), firstCurrObj.Coin, Helpers.UTC_To_IST().ToString(), firstCurrObj.debitObject.RefGuid.ToString(), firstCurrObjDrType)
                                           );

                if (ChargefirstCur > 0 && ChargesecondCur > 0)
                {
                    Parallel.Invoke(() => _commonWalletFunction.EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, firstCurrObjDrWM.UserID.ToString(), ChargefirstCur.ToString(), firstCurrObj.Coin, Helpers.UTC_To_IST().ToString(), firstCurrObj.debitObject.RefGuid.ToString(), "Deducted"),
                    () => _commonWalletFunction.EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, secondCurrObjDrWM.UserID.ToString(), ChargesecondCur.ToString(), secondCurrObj.Coin, Helpers.UTC_To_IST().ToString(), secondCurrObj.debitObject.RefGuid.ToString(), "Deducted"),
                  () => SendNotification(Convert.ToInt32(enErrorCode.ChargeDeductedWallet), firstCurrObjDrWM.UserID.ToString(), Convert.ToInt16(EnNotificationType.Info), secondCurrObj.Coin, ChargefirstCur.ToString(), secondCurrObj.creditObject.RefGuid.ToString()),
                    () => SendNotification(Convert.ToInt32(enErrorCode.ChargeDeductedWallet), firstCurrObjDrWM.UserID.ToString(), Convert.ToInt16(EnNotificationType.Info), secondCurrObj.Coin, ChargefirstCur.ToString(), secondCurrObj.debitObject.RefGuid.ToString()));
                }
                #endregion
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreditNotificationSend" + "TimeStamp:" + timestamp, "WalletService", ex);
            }
        }

        public async Task WalletHoldNotificationSend(string timestamp, WalletMaster dWalletobj, string coinName, decimal amount, string TrnRefNo, byte routeTrnType, decimal charge, long walletId, WalletMaster WalletlogObj, WalletTypeMaster DeductCoinName)
        {
            try
            {
                #region EMAIL_SMS
                WalletMasterResponse walletMasterObj = new WalletMasterResponse();
                walletMasterObj.AccWalletID = dWalletobj.AccWalletID;
                walletMasterObj.Balance = dWalletobj.Balance;
                walletMasterObj.WalletName = dWalletobj.Walletname;
                walletMasterObj.PublicAddress = dWalletobj.PublicAddress;
                walletMasterObj.IsDefaultWallet = dWalletobj.IsDefaultWallet;
                walletMasterObj.CoinName = coinName;
                walletMasterObj.OutBoundBalance = dWalletobj.OutBoundBalance;

                HelperForLog.WriteLogIntoFileAsync("WalletHoldNotificationSend", "OnWalletBalChange + SendActivityNotificationV2 pre timestamp=" + timestamp.ToString());

                Parallel.Invoke(() => SendNotification(Convert.ToInt32(enErrorCode.HoldBalanceNotification), dWalletobj.UserID.ToString(), Convert.ToInt16(EnNotificationType.Info), coinName, amount.ToString(), TrnRefNo.ToString()),
                    () => _signalRService.OnWalletBalChange(walletMasterObj, coinName, dWalletobj.UserID.ToString(), 2),

                    () => _commonWalletFunction.SMSSendAsyncV1(EnTemplateType.SMS_WalletDebited, dWalletobj.UserID.ToString(), null, null, null, null, coinName, routeTrnType.ToString(), TrnRefNo.ToString()),
                    () => _commonWalletFunction.EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, dWalletobj.UserID.ToString(), amount.ToString(), coinName, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString()));

                if (charge > 0 && walletId > 0 && WalletlogObj != null && (DeductCoinName != null))
                {
                    WalletMasterResponse walletMasterObjCharge = new WalletMasterResponse();
                    walletMasterObjCharge.AccWalletID = WalletlogObj.AccWalletID;
                    walletMasterObjCharge.Balance = WalletlogObj.Balance;
                    walletMasterObjCharge.WalletName = WalletlogObj.Walletname;
                    walletMasterObjCharge.PublicAddress = WalletlogObj.PublicAddress;
                    walletMasterObjCharge.IsDefaultWallet = WalletlogObj.IsDefaultWallet;
                    walletMasterObjCharge.CoinName = DeductCoinName.WalletTypeName;
                    walletMasterObjCharge.OutBoundBalance = WalletlogObj.OutBoundBalance;

                    Parallel.Invoke(
                      () => SendNotification(Convert.ToInt32(enErrorCode.ChargeHoldWallet), dWalletobj.UserID.ToString(), Convert.ToInt16(EnNotificationType.Info), DeductCoinName.WalletTypeName, charge.ToString(), TrnRefNo.ToString()),
                      () => _signalRService.OnWalletBalChange(walletMasterObjCharge, DeductCoinName.WalletTypeName, dWalletobj.UserID.ToString(), 2),
                      () => _commonWalletFunction.EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, dWalletobj.UserID.ToString(), charge.ToString(), DeductCoinName.WalletTypeName, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString(), "hold"));
                }
                #endregion
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WalletHoldNotificationSend Timestamp=" + timestamp, "WalletService", ex);
            }
        }

        public async Task WalletDeductionNewNotificationSend(string timestamp, WalletMaster dWalletobj, string coinName, decimal amount, string TrnRefNo, byte routeTrnType, long userID, string Token, string Wtrntype, WalletMasterResponse walletMasterObj, decimal charge, string DeductWalletType, WalletMasterResponse ChargeWallet)
        {
            try
            {
                var trnType = Wtrntype.Contains("Cr_") ? Wtrntype.Replace("Cr_", "") : Wtrntype.Replace("Dr_", "");
                #region SMS_Email          
                
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "OnWalletBalChange + SendActivityNotificationV2 pre TrnNo=" + TrnRefNo.ToString());

                Parallel.Invoke(
                   () => SendNotification(Convert.ToInt32(enErrorCode.DebitWalletMsgNotification), dWalletobj.UserID.ToString(), Convert.ToInt16(EnNotificationType.Info), coinName, trnType.ToString(), TrnRefNo.ToString()),
                   () => _signalRService.OnWalletBalChange(walletMasterObj, coinName, dWalletobj.UserID.ToString(), 2),
                   () => _commonWalletFunction.SMSSendAsyncV1(EnTemplateType.SMS_WalletDebited, userID.ToString(), null, null, null, null, coinName, trnType, TrnRefNo.ToString()),
                   () => _commonWalletFunction.EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, userID.ToString(), amount.ToString(), coinName, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString(), trnType));
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew twice", "WalletNewTest");

                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew mail before", "Get walletid and currency walletid=" + ChargeWallet.AccWalletID.ToString() + "Currency : " + DeductWalletType.ToString() + "Charge: " + charge.ToString());

                if (charge > 0 && (DeductWalletType != null) && ChargeWallet != null)
                {
                    HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew mail after", "Get walletid and currency walletid=" + ChargeWallet.AccWalletID.ToString() + "Currency : " + DeductWalletType.ToString() + "Charge: " + charge.ToString());

                    Parallel.Invoke(
                         () => _signalRService.OnWalletBalChange(ChargeWallet, DeductWalletType, dWalletobj.UserID.ToString(), 2),
                         () => SendNotification(Convert.ToInt32(enErrorCode.ChargeDeductedWallet), dWalletobj.UserID.ToString(), Convert.ToInt16(EnNotificationType.Info), DeductWalletType, charge.ToString(), TrnRefNo.ToString()),
                         //6.Action(1.Hold 2.Released 3.Deduct)
                         () => _commonWalletFunction.EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, userID.ToString(), charge.ToString(), DeductWalletType, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString(), "deducted"));
                }
                #endregion
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WalletHoldNotificationSend Timestamp=" + timestamp, "WalletService", ex);
                //throw ex;
            }
        }

        public async Task SendNotification(int MsgCode, string UserId, short Type,string Param1, string Param2, string Param3)
        {
            try
            {
                ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.ChargeDeductedWallet);
                ActivityNotification.Param1 = Param1;
                ActivityNotification.Param2 = Param2;
                ActivityNotification.Param3 = Param3;
                ActivityNotification.Type = Type;

                _signalRService.SendActivityNotificationV2(ActivityNotification, UserId, 2);

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SendNotification Timestamp=" + UserId, "WalletServicev2", ex);
            }
        }

        public async Task OnWalletBalanceCall(string UserId, string AccWalletID, decimal Balance, string Walletname, string PublicAddress, short IsDefaultWallet, string WalletTypeName, decimal OutBoundBalance)
        {
            try
            {
                WalletMasterResponse walletMasterObjCharge = new WalletMasterResponse();
                walletMasterObjCharge.AccWalletID = AccWalletID;
                walletMasterObjCharge.Balance = Balance;
                walletMasterObjCharge.WalletName = Walletname;
                walletMasterObjCharge.PublicAddress = PublicAddress;
                walletMasterObjCharge.IsDefaultWallet = Convert.ToByte(IsDefaultWallet);
                walletMasterObjCharge.CoinName = WalletTypeName;
                walletMasterObjCharge.OutBoundBalance = OutBoundBalance;

                _signalRService.OnWalletBalChange(walletMasterObjCharge, WalletTypeName, UserId, 2);

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("OnWalletBalanceCall UserId=" + UserId, "WalletServicev2", ex);
            }
        }
    }
}