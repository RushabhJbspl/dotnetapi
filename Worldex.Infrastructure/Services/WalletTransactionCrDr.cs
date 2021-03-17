using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Worldex.Core.Entities;
using Worldex.Core.ApiModels;
using Worldex.Infrastructure.Interfaces;
using Worldex.Core.Interfaces;
using Worldex.Infrastructure.Data;
using Worldex.Core.Enums;
using Worldex.Infrastructure.DTOClasses;
using System.Threading.Tasks;
using Worldex.Core.ViewModels.WalletOperations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.Entities.Wallet;
using System.Linq;
using Worldex.Core.ViewModels.WalletConfiguration;
using System.Collections;
using Worldex.Core.Helpers;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities.User;
using Worldex.Core.ViewModels;
using Worldex.Infrastructure.BGTask;
using Microsoft.AspNetCore.Identity;
using Worldex.Core.Entities.NewWallet;

namespace Worldex.Infrastructure.Services
{
    public class WalletTransactionCrDr : BasePage, IWalletTransactionCrDr
    {
        private readonly ISignalRService _signalRService;
        private readonly ICommonRepository<WalletMaster> _commonRepository;
        private readonly ICommonRepository<WalletTrnLimitConfiguration> _walletTrnLimitConfiguration;
        private readonly ICommonRepository<WalletOrder> _walletOrderRepository;
        private readonly ICommonRepository<AddressMaster> _addressMstRepository;
        private readonly ICommonRepository<TrnAcBatch> _trnBatch;
        private readonly IWalletRepository _walletRepository1;
        private readonly IMessageService _messageService;
        private readonly ICommonRepository<WalletTypeMaster> _WalletTypeMasterRepository;
        private readonly ICommonRepository<TransactionAccount> _TransactionAccountsRepository;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private readonly ICommonWalletFunction _commonWalletFunction;
        private readonly ICommonRepository<ChargeRuleMaster> _chargeRuleMaster;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly IWalletSPRepositories _walletSPRepositories;
        private readonly IWalletTQInsert _WalletTQInsert;


        public WalletTransactionCrDr(ICommonRepository<WalletMaster> commonRepository, ICommonRepository<WalletTrnLimitConfiguration> walletTrnLimitConfiguration,
           ICommonRepository<TrnAcBatch> BatchLogger, ICommonRepository<WalletOrder> walletOrderRepository, IWalletRepository walletRepository,
           ICommonRepository<TradeBitGoDelayAddresses> bitgoDelayRepository, ICommonRepository<AddressMaster> addressMaster, IWalletSPRepositories walletSPRepositories,
           ILogger<BasePage> logger, ICommonRepository<WalletTypeMaster> WalletTypeMasterRepository,
          ICommonRepository<WalletLimitConfiguration> WalletLimitConfig, ICommonRepository<TransactionAccount> TransactionAccountsRepository, ICommonWalletFunction commonWalletFunction, IMessageService messageService, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue,
           ICommonRepository<ChargeRuleMaster> chargeRuleMaster, ISignalRService signalRService, IWalletTQInsert WalletTQInsert) : base(logger)
        {
            _walletSPRepositories = walletSPRepositories;
            _walletTrnLimitConfiguration = walletTrnLimitConfiguration;
            _pushSMSQueue = pushSMSQueue;
            _commonRepository = commonRepository;
            _walletOrderRepository = walletOrderRepository;
            _trnBatch = BatchLogger;
            _walletRepository1 = walletRepository;
            _addressMstRepository = addressMaster;
            _WalletTypeMasterRepository = WalletTypeMasterRepository;
            _TransactionAccountsRepository = TransactionAccountsRepository;
            _signalRService = signalRService;
            _commonWalletFunction = commonWalletFunction;
            _chargeRuleMaster = chargeRuleMaster;
            _messageService = messageService;
            _pushNotificationsQueue = pushNotificationsQueue;
            _WalletTQInsert = WalletTQInsert;//ntrivedi 15-02-2019
        }
        public WalletDrCrResponse DepositionWalletOperation(string timestamp, string address, string coinName, decimal amount, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enWalletTranxOrderType enWalletTranx, enWalletLimitType enWalletLimit, enTrnType routeTrnType, string Token = "", string RefGuid = "")
        {
            try
            {
                WalletMaster dWalletobj, cWalletObj;
                string DrRemarks = "", CrRemarks = "";
                WalletTypeMaster walletTypeMaster;
                WalletTransactionQueue objTQDr, objTQCr;
                //long walletTypeID;
                WalletDrCrResponse resp = new WalletDrCrResponse();
                long owalletID, orgID;
                WalletTransactionOrder woObj;
                HelperForLog.WriteLogIntoFile("DepositionWalletOperation", "WalletTransactionCrDr", "timestamp:" + timestamp + "," + "coinName:" + coinName + ",TrnRefNo=" + TrnRefNo.ToString() + ",address=" + address + ",amount=" + amount.ToString() + ",Token=" + Token);

                owalletID = GetWalletByAddress(address);
                //--call new SP 2019-1-22
                long Trnno = 0;

                /// ====
                //if (owalletID == 0)
                //{
                //    return new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAddress, ErrorCode = enErrorCode.InvalidAddress };
                //}
                cWalletObj = _commonRepository.GetById(owalletID);
                orgID = _walletRepository1.getOrgID();
                //if (orgID == 0)
                //{
                //    return new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.OrgIDNotFound, ErrorCode = enErrorCode.OrgIDNotFound };
                //}
                dWalletobj = _commonRepository.FindBy(e => e.WalletTypeID == cWalletObj.WalletTypeID && e.UserID == orgID && e.IsDefaultWallet == 1 && e.Status == 1 && e.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)).FirstOrDefault();
                if (dWalletobj == null)
                {
                    //tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid().ToString(), orderType, amount, TrnRefNo, UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, 2, EnResponseMessage.InvalidWallet);
                    return new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet };
                }
                //ntrivedi 15-02-2019 for margin wallet not use other wallet
                if (cWalletObj.WalletUsageType != Convert.ToInt16(EnWalletUsageType.Trading_Wallet))
                {
                    objTQCr = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, amount, TrnRefNo, UTC_To_IST(), null, cWalletObj.Id, coinName, cWalletObj.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.MarginWalletCanNotBeUsedForThis, trnType, RefGuid);
                    objTQCr = _walletRepository1.AddIntoWalletTransactionQueue(objTQCr, 1);
                    return new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.MarginWalletCanNotBeUsedForThis, ErrorCode = enErrorCode.MarginWalletCanNotBeUsedForThis };

                }
                //ntrivedi 03-11-2018
                var charge = _commonWalletFunction.GetServiceLimitChargeValue(enWalletTrnType.Deposit, coinName);
                if (charge.MaxAmount < amount && charge.MinAmount > amount)
                {
                    objTQCr = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, amount, TrnRefNo, UTC_To_IST(), null, dWalletobj.Id, coinName, dWalletobj.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.ProcessTrn_AmountBetweenMinMaxMsg, trnType, RefGuid);
                    objTQCr = _walletRepository1.AddIntoWalletTransactionQueue(objTQCr, 1);
                    return new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ProcessTrn_AmountBetweenMinMaxMsg, ErrorCode = enErrorCode.ProcessTrn_AmountBetweenMinMax };
                }
                //resp = InsertWalletTQDebit(timestamp, dWalletobj.Id, coinName, amount, TrnRefNo, serviceType, enWalletTrnType.Deposit, enWalletTranx, enWalletLimit);
                //if (resp.ReturnCode != 0 || resp.Status != enTransactionStatus.Initialize)
                //{
                //    return new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = resp.StatusMsg, ErrorCode = resp.ErrorCode };
                //}
                if (cWalletObj.Status != 1 || cWalletObj.IsValid == false)
                {
                    // insert with status=2 system failed
                    objTQCr = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, amount, TrnRefNo, UTC_To_IST(), null, dWalletobj.Id, coinName, dWalletobj.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidWallet, trnType, RefGuid);
                    objTQCr = _walletRepository1.AddIntoWalletTransactionQueue(objTQCr, 1);
                    return new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TrnNo = objTQCr.TrnNo, Status = objTQCr.Status, StatusMsg = objTQCr.StatusMsg };
                }
                var response = _walletSPRepositories.Callsp_DepositionProcess(ref Trnno, EnAllowedChannels.Web, timestamp, coinName, trnType, amount, TrnRefNo);
                if (response.ReturnCode != enResponseCode.Success)
                {
                    return new WalletDrCrResponse { ReturnCode = response.ReturnCode, ReturnMsg = response.ReturnMsg, ErrorCode = response.ErrorCode };
                }
                //objTQDr = _walletRepository1.GetTransactionQueue(resp.TrnNo);
                //TrnAcBatch batchObj = _trnBatch.Add(new TrnAcBatch());
                //DrRemarks = "Debit for Deposition TrnNo:" + TrnRefNo;
                //WalletLedger walletLedgerDr = GetWalletLedgerObj(dWalletobj.Id, cWalletObj.Id, amount, 0, enWalletTrnType.Deposit, serviceType, objTQDr.TrnNo, DrRemarks, dWalletobj.Balance, 1);
                //TransactionAccount tranxAccounDrt = GetTransactionAccount(dWalletobj.Id, 1, batchObj.Id, amount, 0, objTQDr.TrnNo, DrRemarks, 1);
                //dWalletobj.DebitBalance(amount);
                //objTQDr.Status = enTransactionStatus.Success;
                //objTQDr.StatusMsg = "Success";
                //DrRemarks = "Credit for Deposition TrnNo:" + TrnRefNo;
                //objTQCr = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, amount, TrnRefNo, UTC_To_IST(), null, cWalletObj.Id, coinName, cWalletObj.UserID, timestamp, 0, "Inserted", trnType);
                //objTQCr = _walletRepository1.AddIntoWalletTransactionQueue(objTQCr, 1);
                //woObj = InsertIntoWalletTransactionOrder(null, UTC_To_IST(), cWalletObj.Id, dWalletobj.Id, amount, coinName, objTQCr.TrnNo, objTQDr.TrnNo, 0, "Inserted");
                //woObj = _walletRepository1.AddIntoWalletTransactionOrder(woObj, 1);
                //WalletLedger walletLedgerCr = GetWalletLedgerObj(cWalletObj.Id, dWalletobj.Id, 0, amount, trnType, serviceType, objTQCr.TrnNo, DrRemarks, cWalletObj.Balance, 1);
                //TransactionAccount tranxAccountCr = GetTransactionAccount(cWalletObj.Id, 1, batchObj.Id, 0, amount, objTQCr.TrnNo, DrRemarks, 1);
                //cWalletObj.CreditBalance(amount);
                ////var objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, TotalAmount, TrnRefNo, UTC_To_IST(), null, cWalletobj.Id, coinName, userID, timestamp, 1, "Updated");
                //objTQCr.Status = enTransactionStatus.Success;
                //objTQCr.StatusMsg = "Success";
                //objTQCr.UpdatedDate = UTC_To_IST();
                //woObj.Status = enTransactionStatus.Success;
                //woObj.StatusMsg = "Deposition success for RefNo :" + TrnRefNo;
                //woObj.UpdatedDate = UTC_To_IST();
                //objTQDr.SettedAmt = amount;
                //_walletRepository1.WalletCreditDebitwithTQ(walletLedgerDr, walletLedgerCr, tranxAccountCr, tranxAccounDrt, dWalletobj, cWalletObj, objTQCr, objTQDr, woObj);
                //ntrivedi temperory
                // 2018-11-1---------------socket method   --------------------------
                WalletMasterResponse walletMasterObj = new WalletMasterResponse();
                walletMasterObj.AccWalletID = cWalletObj.AccWalletID;
                walletMasterObj.Balance = cWalletObj.Balance;
                walletMasterObj.WalletName = cWalletObj.Walletname;
                walletMasterObj.PublicAddress = cWalletObj.PublicAddress;
                walletMasterObj.AccWalletID = cWalletObj.AccWalletID;
                walletMasterObj.CoinName = coinName;
                walletMasterObj.OutBoundBalance = cWalletObj.OutBoundBalance;

                ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.CreditWalletMsgNotification);
                ActivityNotification.Param1 = coinName;
                ActivityNotification.Param2 = routeTrnType.ToString();
                ActivityNotification.Param3 = RefGuid.ToString();
                ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Success);
                try
                {
                    _signalRService.OnWalletBalChange(walletMasterObj, coinName, cWalletObj.UserID.ToString(), 2);
                    //_signalRService.SendActivityNotification(msg, cWalletObj.UserID.ToString(), 2);
                    _signalRService.SendActivityNotificationV2(ActivityNotification, cWalletObj.UserID.ToString(), 2);

                    var trn = trnType.ToString().Contains("Cr_") ? trnType.ToString().Replace("Cr_", "") : trnType.ToString().Replace("Dr_", "");
                    ApplicationUser User = new ApplicationUser();
                    try
                    {
                        User = _walletRepository1.GetUserById(Convert.ToInt64(cWalletObj.UserID.ToString())).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        HelperForLog.WriteErrorLog("User ex" + System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                    }
                    //2018-12-7 for EMAIL //amount,coin,date,refno
                    EmailSendAsyncV1(EnTemplateType.EMAIL_WalletCredited, cWalletObj.UserID.ToString(), amount.ToString(), coinName, UTC_To_IST().ToString(), RefGuid.ToString(), trn, "", "", "", "", User);


                    //Task.Run(() => Parallel.Invoke(() => _signalRService.SendActivityNotificationV2(ActivityNotification, cWalletObj.UserID.ToString(), 2),
                    //    () => _signalRService.OnWalletBalChange(walletMasterObj, coinName, cWalletObj.UserID.ToString(), 2),
                    //    () => _walletService.SMSSendAsyncV1(EnTemplateType.SMS_WalletCredited, cWalletObj.UserID.ToString(), null, null, null, null, coinName, routeTrnType.ToString(), TrnRefNo.ToString())));


                    Parallel.Invoke(() => _signalRService.SendActivityNotificationV2(ActivityNotification, cWalletObj.UserID.ToString(), 2),
                       () => _signalRService.OnWalletBalChange(walletMasterObj, coinName, cWalletObj.UserID.ToString(), 2),
                       () => SMSSendAsyncV1(EnTemplateType.SMS_WalletCredited, cWalletObj.UserID.ToString(), null, null, null, null, coinName, routeTrnType.ToString(), RefGuid.ToString()));

                }
                catch (Exception ex2)
                {
                    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex2);

                }
                return new WalletDrCrResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDebit, ErrorCode = enErrorCode.Success, TrnNo = resp.TrnNo, Status = resp.Status, StatusMsg = resp.StatusMsg };

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        protected WalletTransactionQueue InsertIntoWalletTransactionQueue(Guid Guid, enWalletTranxOrderType TrnType, decimal Amount, long TrnRefNo, DateTime TrnDate, DateTime? UpdatedDate,
            long WalletID, string WalletType, long MemberID, string TimeStamp, enTransactionStatus Status, string StatusMsg, enWalletTrnType enWalletTrnType, string RefGuid)
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

        protected int CheckTrnRefNo(long TrnRefNo, enWalletTranxOrderType TrnType, enWalletTrnType wType)
        {
            var count = _walletRepository1.CheckTrnRefNo(TrnRefNo, TrnType, wType);
            return count;
        }

        public long GetWalletByAddress(string address)
        {
            try
            {
                var addressObj = _addressMstRepository.FindBy(e => e.OriginalAddress == address).FirstOrDefault();
                if (addressObj == null)
                {
                    return 0;
                }
                else
                {
                    return addressObj.WalletId;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public WalletTransactionOrder InsertIntoWalletTransactionOrder(DateTime? UpdatedDate, DateTime TrnDate, long OWalletID, long DWalletID, decimal Amount, string WalletType, long OTrnNo, long DTrnNo, enTransactionStatus Status, string StatusMsg)
        {
            WalletTransactionOrder walletTransactionOrder = new WalletTransactionOrder();
            walletTransactionOrder.UpdatedDate = UpdatedDate;
            walletTransactionOrder.TrnDate = TrnDate;
            walletTransactionOrder.OWalletID = OWalletID;
            walletTransactionOrder.DWalletID = DWalletID;
            walletTransactionOrder.Amount = Amount;
            walletTransactionOrder.WalletType = WalletType;
            walletTransactionOrder.OTrnNo = OTrnNo;
            walletTransactionOrder.DTrnNo = DTrnNo;
            walletTransactionOrder.Status = Status;
            walletTransactionOrder.StatusMsg = StatusMsg;
            return walletTransactionOrder;
        }

        public WalletLedger GetWalletLedgerObj(long WalletID, long toWalletID, decimal drAmount, decimal crAmount, enWalletTrnType trnType, enServiceType serviceType, long trnNo, string remarks, decimal currentBalance, byte status)
        {
            try
            {
                var walletLedger2 = new WalletLedger();
                walletLedger2.ServiceTypeID = serviceType;
                walletLedger2.TrnType = trnType;
                walletLedger2.CrAmt = crAmount;
                walletLedger2.CreatedBy = WalletID;
                walletLedger2.CreatedDate = UTC_To_IST();
                walletLedger2.DrAmt = drAmount;
                walletLedger2.TrnNo = trnNo;
                walletLedger2.Remarks = remarks;
                walletLedger2.Status = status;
                walletLedger2.TrnDate = UTC_To_IST();
                walletLedger2.UpdatedBy = WalletID;
                walletLedger2.WalletId = WalletID;
                walletLedger2.ToWalletId = toWalletID;
                if (drAmount > 0)
                {
                    walletLedger2.PreBal = currentBalance;
                    walletLedger2.PostBal = currentBalance - drAmount;
                }
                else
                {
                    walletLedger2.PreBal = currentBalance;
                    walletLedger2.PostBal = currentBalance + crAmount;
                }
                return walletLedger2;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TransactionAccount GetTransactionAccount(long WalletID, short isSettled, long batchNo, decimal drAmount, decimal crAmount, long trnNo, string remarks, byte status)
        {
            try
            {
                var walletLedger2 = new TransactionAccount();
                walletLedger2.CreatedBy = WalletID;
                walletLedger2.CreatedDate = UTC_To_IST();
                walletLedger2.DrAmt = drAmount;
                walletLedger2.CrAmt = crAmount;
                walletLedger2.RefNo = trnNo;
                walletLedger2.Remarks = remarks;
                walletLedger2.Status = status;
                walletLedger2.TrnDate = UTC_To_IST();
                walletLedger2.UpdatedBy = WalletID;
                walletLedger2.WalletID = WalletID;
                walletLedger2.IsSettled = isSettled;
                walletLedger2.BatchNo = batchNo;
                return walletLedger2;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        ////vsolanki 2018-10-29
        public bool CheckUserBalance(long WalletId, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet)
        {
            try
            {
                WalletMaster walletObjectTask = _commonRepository.GetById(WalletId);
                var crsum = _TransactionAccountsRepository.GetSum(e => e.WalletID == WalletId && e.IsSettled == 1 && e.Type == enBalanceType.AvailableBalance, f => f.CrAmt);
                var drsum = _TransactionAccountsRepository.GetSum(e => e.WalletID == WalletId && e.IsSettled == 1 && e.Type == enBalanceType.AvailableBalance, f => f.DrAmt);
                var total = crsum - drsum;
                //ntrivedi 13-02-2019 added so margin wallet do not use in other transaction
                if (walletObjectTask.WalletUsageType != Convert.ToInt16(enWalletUsageType))
                {
                    HelperForLog.WriteLogIntoFileAsync("CheckUserBalanceAsync", "WalletId=" + WalletId.ToString() + "WalletUsageType Mismatching :" + enWalletUsageType);
                    return false;
                }
                if (total == walletObjectTask.Balance && total >= 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //public Task<bool> CheckUserBalanceAsync(long WalletId, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet)
        //{
        //    try
        //    {
        //        WalletMaster walletObjectTask = _commonRepository.GetById(WalletId);
        //        var crsum = _TransactionAccountsRepository.GetSum(e => e.WalletID == WalletId && e.IsSettled == 1 && e.Type == enBalanceType.AvailableBalance, f => f.CrAmt);
        //        var drsum = _TransactionAccountsRepository.GetSum(e => e.WalletID == WalletId && e.IsSettled == 1 && e.Type == enBalanceType.AvailableBalance, f => f.DrAmt);
        //        var total = crsum - drsum;
        //        //ntrivedi 13-02-2019 added so margin wallet do not use in other transaction
        //        if (walletObjectTask.WalletUsageType != Convert.ToInt16(enWalletUsageType))
        //        {
        //            HelperForLog.WriteLogIntoFileAsync("CheckUserBalanceAsync", "WalletId=" + WalletId.ToString() + "WalletUsageType Mismatching :" + enWalletUsageType);
        //            return Task.FromResult(false);
        //        }
        //        if (total == walletObjectTask.Balance && total >= 0)
        //        {
        //            return Task.FromResult(true);
        //        }
        //        return Task.FromResult(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

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
                return false;
            }
        }
        public ServiceLimitChargeValue GetServiceLimitChargeValue(enWalletTrnType TrnType, string CoinName)
        {
            try
            {
                ServiceLimitChargeValue response;
                var walletType = _WalletTypeMasterRepository.GetSingle(x => x.WalletTypeName == CoinName);
                if (walletType != null)
                {
                    response = new ServiceLimitChargeValue();
                    var limitData = _walletTrnLimitConfiguration.GetSingle(x => x.TrnType == Convert.ToInt16(TrnType) && x.WalletType == walletType.Id);
                    var chargeData = _chargeRuleMaster.GetSingle(x => x.TrnType == TrnType && x.WalletType == walletType.Id);

                    if (limitData != null && chargeData != null)
                    {
                        response.CoinName = walletType.WalletTypeName;
                        response.TrnType = (enWalletTrnType)limitData.TrnType;
                        response.MinAmount = limitData.MinAmount;
                        response.MaxAmount = limitData.MaxAmount;
                        response.ChargeType = chargeData.ChargeType;
                        response.ChargeValue = chargeData.ChargeValue;
                    }
                    return response;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<WalletDrCrResponse> GetWalletCreditNewAsync(string coinName, string timestamp, enWalletTrnType trnType, decimal TotalAmount, long userID, string crAccWalletID, CreditWalletDrArryTrnID[] arryTrnID, long TrnRefNo, short isFullSettled, enWalletTranxOrderType orderType, enServiceType serviceType, enTrnType routeTrnType, string Token = "", string RefGuid = "")
        {
            WalletTransactionQueue tqObj = new WalletTransactionQueue();
            WalletTransactionOrder woObj = new WalletTransactionOrder();
            try
            {
                WalletMaster cWalletobj;
                string remarks = "";
                WalletTypeMaster walletTypeMaster;
                long TrnNo = 0;
                WalletDrCrResponse resp = new WalletDrCrResponse();
                HelperForLog.WriteLogIntoFile("GetWalletCreditNew", "WalletTransactionCrDr", "timestamp:" + timestamp + "," + "coinName:" + coinName + ",TrnRefNo=" + TrnRefNo.ToString() + ",userID=" + userID + ",amount=" + TotalAmount.ToString());

                if (string.IsNullOrEmpty(crAccWalletID) || coinName == string.Empty || userID == 0)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidWalletOrUserIDorCoinName }, "Credit");
                }
                walletTypeMaster = await _WalletTypeMasterRepository.GetSingleAsync(e => e.WalletTypeName == coinName);
                if (walletTypeMaster == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidCoinName }, "Credit");
                }

                //ntrivedi 15-02-2019 for margin wallet not use other wallet
                cWalletobj = await _commonRepository.GetSingleAsync(e => e.UserID == userID && e.WalletTypeID == walletTypeMaster.Id && e.AccWalletID == crAccWalletID && e.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

                if (cWalletobj == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.UserIDWalletIDDidNotMatch }, "Credit");
                }
                if (cWalletobj.Status != 1 || cWalletobj.IsValid == false)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet }, "Credit");
                }
                if (orderType != enWalletTranxOrderType.Credit) // buy 
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTrnType, ErrorCode = enErrorCode.InvalidTrnType }, "Credit");
                }

                if (TrnRefNo == 0) // buy
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo }, "Credit");
                }
                var bal1 = CheckUserBalanceAsync(0,cWalletobj.Id);
                var bal = await bal1;
                if (!bal)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, TotalAmount, TrnRefNo, UTC_To_IST(), null, cWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType, RefGuid);
                    tqObj = _walletRepository1.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedBalanceMismatch, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                }
                if (TotalAmount <= 0)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, TotalAmount, TrnRefNo, UTC_To_IST(), null, cWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidAmt, trnType, RefGuid);
                    tqObj = _walletRepository1.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmount, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                }
                int count = CheckTrnRefNoForCredit(TrnRefNo, enWalletTranxOrderType.Debit); // check whether for this refno wallet is pre decuted or not
                if (count == 0)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, TotalAmount, TrnRefNo, UTC_To_IST(), null, cWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTradeRefNo, trnType, RefGuid);
                    tqObj = _walletRepository1.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AlredyExist, ErrorCode = enErrorCode.AlredyExist }, "Credit");
                }
                //validate insp
                //bool checkArray = CheckarryTrnID(arryTrnID, coinName);
                //if (checkArray == false)//fail
                //{
                //    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, TotalAmount, TrnRefNo, UTC_To_IST(), null, cWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, "Amount and DebitRefNo matching failure", trnType, RefGuid);
                //    tqObj = _walletRepository1.AddIntoWalletTransactionQueue(tqObj, 1);

                //    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                //}

                BizResponseClass bizResponse = _walletSPRepositories.Callsp_CreditWallet(cWalletobj, timestamp, serviceType, TotalAmount, coinName, EnAllowedChannels.Web, walletTypeMaster.Id, TrnRefNo, cWalletobj.Id, cWalletobj.UserID, routeTrnType, trnType, ref TrnNo, enWalletDeductionType.Normal);
                if (bizResponse.ReturnCode != enResponseCode.Success)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = bizResponse.ReturnMsg, ErrorCode = bizResponse.ErrorCode, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "GetWalletCreditNewAsync");
                }

                // 2018-11-1---------------socket method   --------------------------
                WalletMasterResponse walletMasterObj = new WalletMasterResponse();
                walletMasterObj.AccWalletID = cWalletobj.AccWalletID;
                walletMasterObj.Balance = cWalletobj.Balance;
                walletMasterObj.WalletName = cWalletobj.Walletname;
                walletMasterObj.PublicAddress = cWalletobj.PublicAddress;
                walletMasterObj.IsDefaultWallet = cWalletobj.IsDefaultWallet;
                walletMasterObj.CoinName = coinName;
                walletMasterObj.OutBoundBalance = cWalletobj.OutBoundBalance;

                ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.CreditWalletMsgNotification);
                ActivityNotification.Param1 = coinName;
                ActivityNotification.Param2 = routeTrnType.ToString();
                ActivityNotification.Param3 = RefGuid.ToString();
                ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Success);

                var trn = trnType.ToString().Contains("Cr_") ? trnType.ToString().Replace("Cr_", "") : trnType.ToString().Replace("Dr_", "");

                ApplicationUser User = new ApplicationUser();
                try
                {
                    User = _walletRepository1.GetUserById(Convert.ToInt64(userID.ToString())).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog("User ex" + System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                }
                Parallel.Invoke(
                                 () => _signalRService.SendActivityNotificationV2(ActivityNotification, userID.ToString(), 2),

                                   () => _signalRService.OnWalletBalChange(walletMasterObj, coinName, userID.ToString(), 2),//ntrivedi signal r call with userid 08-02-2019

                                   () => SMSSendAsyncV1(EnTemplateType.SMS_WalletCredited, userID.ToString(), null, null, null, null, coinName, routeTrnType.ToString(), RefGuid.ToString()),
                                   () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletCredited, userID.ToString(), TotalAmount.ToString(), coinName, UTC_To_IST().ToString(), RefGuid.ToString(), trn, "", "", "", "", User));

                try
                {
                    decimal charge = _walletRepository1.FindChargeValueDeduct(timestamp, TrnRefNo);
                    long ChargewalletId = _walletRepository1.FindChargeValueReleaseWalletId(timestamp, TrnRefNo);
                    string ChargeCurrency = _walletRepository1.FindChargeCurrencyDeduct(TrnRefNo);

                    HelperForLog.WriteLogIntoFileAsync("GetWalletCreditNewAsync before", "Get walletid and currency walletid=" + ChargewalletId.ToString() + "Currency : " + ChargeCurrency.ToString() + "Charge: " + charge.ToString() + "TrnRefNo: " + RefGuid.ToString() + "timestamp : " + timestamp.ToString());
                    if (charge > 0 && ChargewalletId > 0 && (ChargeCurrency != null || ChargeCurrency != ""))
                    {
                        var ChargeWalletObj = _commonRepository.GetById(ChargewalletId);
                        if (ChargeWalletObj != null)
                        {
                            HelperForLog.WriteLogIntoFileAsync("GetWalletCreditNewAsync before", "Get walletid and currency walletid=" + ChargewalletId.ToString() + "Currency : " + ChargeCurrency.ToString() + "Charge: " + charge.ToString() + "TrnRefNo: " + TrnRefNo.ToString() + "timestamp : " + timestamp.ToString());

                            ActivityNotificationMessage ActivityNotificationCharge = new ActivityNotificationMessage();
                            ActivityNotificationCharge.MsgCode = Convert.ToInt32(enErrorCode.CreditWalletMsgNotification);
                            ActivityNotificationCharge.Param1 = ChargeCurrency;
                            ActivityNotificationCharge.Param2 = routeTrnType.ToString();
                            ActivityNotificationCharge.Param3 = RefGuid.ToString();
                            ActivityNotificationCharge.Type = Convert.ToInt16(EnNotificationType.Success);

                            WalletMasterResponse walletMasterObjCharge = new WalletMasterResponse();
                            walletMasterObjCharge.AccWalletID = ChargeWalletObj.AccWalletID;
                            walletMasterObjCharge.Balance = ChargeWalletObj.Balance;
                            walletMasterObjCharge.WalletName = ChargeWalletObj.Walletname;
                            walletMasterObjCharge.PublicAddress = ChargeWalletObj.PublicAddress;
                            walletMasterObjCharge.IsDefaultWallet = ChargeWalletObj.IsDefaultWallet;
                            walletMasterObjCharge.CoinName = ChargeCurrency;
                            walletMasterObjCharge.OutBoundBalance = ChargeWalletObj.OutBoundBalance;

                            Parallel.Invoke(() => EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApplyrefund, userID.ToString(), charge.ToString(), ChargeCurrency, UTC_To_IST().ToString(), RefGuid.ToString(), "refunded", "", "", "", "", User),
                          () => _signalRService.SendActivityNotificationV2(ActivityNotificationCharge, userID.ToString(), 2),
                            () => _signalRService.OnWalletBalChange(walletMasterObjCharge, ChargeCurrency, userID.ToString(), 2)
                      );
                        }
                    }
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog("Charge ex timestamp: " + timestamp + System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                }
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessCredit, ErrorCode = enErrorCode.Success, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "GetWalletCreditNew");

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError, TrnNo = 0, Status = 0, StatusMsg = "" }, "Credit");
            }
        }

        public WalletDrCrResponse GetCrDRResponse(WalletDrCrResponse obj, string extras)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFile(extras, "WalletTransactionCrDr", "Time Stamp:" + obj.TimeStamp + " " + "ReturnCode=" + obj.ReturnCode + ",ErrorCode=" + obj.ErrorCode + ", ReturnMsg=" + obj.ReturnMsg + ",StatusMsg=" + obj.StatusMsg + ",TrnNo=" + obj.TrnNo));
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return obj;
            }
        }

        public WalletLedger GetWalletLedger(long WalletID, long toWalletID, decimal drAmount, decimal crAmount, enWalletTrnType trnType, enServiceType serviceType, long trnNo, string remarks, decimal currentBalance, byte status)
        {
            try
            {
                var walletLedger2 = new WalletLedger();
                walletLedger2.ServiceTypeID = serviceType;
                walletLedger2.TrnType = trnType;
                walletLedger2.CrAmt = crAmount;
                walletLedger2.CreatedBy = WalletID;
                walletLedger2.CreatedDate = UTC_To_IST();
                walletLedger2.DrAmt = drAmount;
                walletLedger2.TrnNo = trnNo;
                walletLedger2.Remarks = remarks;
                walletLedger2.Status = status;
                walletLedger2.TrnDate = UTC_To_IST();
                walletLedger2.UpdatedBy = WalletID;
                walletLedger2.WalletId = WalletID;
                walletLedger2.ToWalletId = toWalletID;
                if (drAmount > 0)
                {
                    walletLedger2.PreBal = currentBalance;
                    walletLedger2.PostBal = currentBalance - drAmount;
                }
                else
                {
                    walletLedger2.PreBal = currentBalance;
                    walletLedger2.PostBal = currentBalance + crAmount;
                }
                return walletLedger2;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public int CheckTrnRefNoForCredit(long TrnRefNo, enWalletTranxOrderType TrnType)
        {
            var count = _walletRepository1.CheckTrnRefNoForCredit(TrnRefNo, TrnType);
            return count;
        }

        public bool CheckarryTrnID(CreditWalletDrArryTrnID[] arryTrnID, string coinName)
        {
            bool obj = _walletRepository1.CheckarryTrnID(arryTrnID, coinName);
            return obj;
        }

        public async Task<WalletDrCrResponse> GetWalletDeductionNew(string coinName, string timestamp, enWalletTranxOrderType orderType, decimal amount, long userID, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, string Token = "", string RefGuid = "")
        {
            try
            {
                WalletMaster dWalletobj;
                string remarks = "";
                WalletTypeMaster walletTypeMaster;
                WalletTransactionQueue objTQ;
                //long walletTypeID;
                WalletDrCrResponse resp = new WalletDrCrResponse();
                bool CheckUserBalanceFlag = false;
                long trnno = 0;
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "WalletTransactionCrDr", "timestamp:" + timestamp + "," + "coinName:" + coinName + ",accWalletID=" + accWalletID + ",TrnRefNo=" + TrnRefNo.ToString() + ",userID=" + userID + ",amount=" + amount.ToString());

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

                //ntrivedi 15-02-2019 for margin wallet not use other wallet
                Task<WalletMaster> dWalletobjTask = _commonRepository.GetSingleAsync(e => e.UserID == userID && e.WalletTypeID == walletTypeMaster.Id && e.AccWalletID == accWalletID && e.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (orderType != enWalletTranxOrderType.Debit) // sell 13-10-2018
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTrnType, trnType, RefGuid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTrnType, ErrorCode = enErrorCode.InvalidTrnType, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                if (TrnRefNo == 0) // sell 13-10-2018
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTradeRefNo, trnType, RefGuid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                if (amount <= 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidAmt, trnType, RefGuid);
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
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidWallet, trnType, RefGuid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "CheckUserBalance pre Balance=" + dWalletobj.Balance.ToString() + ", TrnNo=" + TrnRefNo.ToString() + ",TimeStamp=" + timestamp);
                //CheckUserBalanceFlag = await flagTask;
                CheckUserBalanceFlag = flagTask;
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "CheckUserBalance Post TrnNo=" + TrnRefNo.ToString() + ",TimeStamp=" + timestamp);

                dWalletobj = _commonRepository.GetById(dWalletobj.Id); // ntrivedi fetching fresh balance for multiple request at a time 
                // Wallet Limit Check
                var msg = _commonWalletFunction.CheckWalletLimitAsyncV1(enWalletLimitType.WithdrawLimit, dWalletobj.Id, amount);

                if (dWalletobj.Balance < amount)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, trnType, RefGuid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficantBal, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                if (!CheckUserBalanceFlag)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType, RefGuid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedBalanceMismatch, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                var limitres = await msg;
                if (limitres.ErrorCode != enErrorCode.Success)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletLimitExceed, trnType, RefGuid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse
                    {
                        ReturnCode = enResponseCode.Fail,
                        ReturnMsg = limitres.ReturnMsg,  //Uday 11-02-2019 Give Particular Limit Validation Message
                        //ErrorCode = limitres,
                        ErrorCode = limitres.ErrorCode,
                        TrnNo = objTQ.TrnNo,
                        Status = objTQ.Status,
                        StatusMsg = objTQ.StatusMsg,
                        MinimumAmount = limitres.MinimumAmounts,
                        MaximumAmount = limitres.MaximumAmounts
                    }, "Debit");
                }

                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "Check ShadowLimit done TrnNo=" + TrnRefNo.ToString() + ",TimeStamp=" + timestamp);

                //int count = await countTask;
                CheckTrnRefNoRes count1 = await countTask1;
                if (count1.TotalCount != 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.AlredyExist, trnType, RefGuid);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AlredyExist, ErrorCode = enErrorCode.AlredyExist, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "CheckTrnRefNo TrnNo=" + TrnRefNo.ToString() + ",TimeStamp=" + timestamp);

                BizResponseClass bizResponse = _walletSPRepositories.Callsp_DebitWallet(dWalletobj, timestamp, serviceType, amount, coinName, EnAllowedChannels.Web, walletTypeMaster.Id, TrnRefNo, dWalletobj.Id, dWalletobj.UserID, routeTrnType, trnType, ref trnno, enWalletDeductionType.Normal);
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "WalletDeductionwithTQ sp call done TrnNo=" + TrnRefNo.ToString());

                if (bizResponse.ReturnCode != enResponseCode.Success)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = bizResponse.ReturnMsg, ErrorCode = bizResponse.ErrorCode, TrnNo = trnno, Status = 0, StatusMsg = "" }, "Debit");
                }

                //vsolanki 2018-11-1---------------socket method   --------------------------
                WalletMasterResponse walletMasterObj = new WalletMasterResponse();
                walletMasterObj.AccWalletID = dWalletobj.AccWalletID;
                walletMasterObj.Balance = dWalletobj.Balance;
                walletMasterObj.WalletName = dWalletobj.Walletname;
                walletMasterObj.PublicAddress = dWalletobj.PublicAddress;
                walletMasterObj.IsDefaultWallet = dWalletobj.IsDefaultWallet;
                walletMasterObj.CoinName = coinName;
                walletMasterObj.OutBoundBalance = dWalletobj.OutBoundBalance;

                ApplicationUser User = new ApplicationUser();
                try
                {
                    User = _walletRepository1.GetUserById(Convert.ToInt64(userID.ToString())).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog("User ex" + System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                }

                Task.Run(() => WalletDeductionNewNotificationSend(timestamp, dWalletobj, coinName, amount, RefGuid, (byte)routeTrnType, userID, Token, trnType.ToString(), walletMasterObj, User));

                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "OnWalletBalChange + SendActivityNotificationV2 Done TrnNo=" + TrnRefNo.ToString());

                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDebit, ErrorCode = enErrorCode.Success, TrnNo = trnno, Status = enTransactionStatus.Hold, StatusMsg = bizResponse.ReturnMsg }, "Debit");
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWalletDeductionNew", "WalletTransactionCrDr", ex);
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError, TrnNo = 0, Status = 0, StatusMsg = "", TimeStamp = timestamp }, "Debit");
            }
        }
        //2018-12-10
        public async Task SMSSendAsyncV1(EnTemplateType templateType, string UserID, string WalletName = null, string SourcePrice = null, string DestinationPrice = null, string ONOFF = null, string Coin = null, string TrnType = null, string TrnNo = null)
        {
            try
            {
                HelperForLog.WriteLogIntoFile("WalletTransactionCrDr", "0 SMSSendAsyncV1", " -Data- " + templateType.ToString());
                CommunicationParamater communicationParamater = new CommunicationParamater();
                ApplicationUser User = new ApplicationUser();
                User = await _walletRepository1.GetUserById(Convert.ToInt64(UserID));
                HelperForLog.WriteLogIntoFile("WalletTransactionCrDr", "0 SMSSendAsyncV1", " -Data- " + UserID.ToString() + "UserName : " + User.UserName);
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
                                HelperForLog.WriteLogIntoFile("WalletTransactionCrDr", "0 SMSSendAsyncV1", " -Data- " + SmsData.Content);
                                _pushSMSQueue.Enqueue(Request);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " -Data- " + templateType.ToString(), this.GetType().Name, ex);
            }
        }

        //2018-12-10
        public async Task EmailSendAsyncV1(EnTemplateType templateType, string UserID, string Param1 = "", string Param2 = "", string Param3 = "", string Param4 = "", string Param5 = "", string Param6 = "", string Param7 = "", string Param8 = "", string Param9 = "", ApplicationUser User = null)
        {
            try
            {
                HelperForLog.WriteLogIntoFile("WalletTransactionCrDr", "0 EmailSendAsyncV1", " -Data- " + templateType.ToString());
                CommunicationParamater communicationParamater = new CommunicationParamater();
                SendEmailRequest Request = new SendEmailRequest();
                HelperForLog.WriteLogIntoFile("WalletTransactionCrDr", "0 EmailSendAsyncV1", " -Data- " + UserID.ToString() + "UserName : " + User.UserName);
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
                            HelperForLog.WriteLogIntoFile("WalletTransactionCrDr", "0 EmailSendAsyncV1", " -Data- " + EmailData.Content.ToString());
                            Request.Recepient = User.Email;
                            HelperForLog.WriteLogForSocket("WalletTransactionCrDr", "0 EmailSendAsyncV1", " -Data- " + EmailData);
                            _pushNotificationsQueue.Enqueue(Request);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " -Data- " + templateType.ToString(), this.GetType().Name, ex);
            }
        }

        public async Task WalletDeductionNewNotificationSend(string timestamp, WalletMaster dWalletobj, string coinName, decimal amount, string TrnRefNo, byte routeTrnType, long userID, string Token, string Wtrntype, WalletMasterResponse walletMasterObj, ApplicationUser User)
        {
            try
            {
                var trnType = Wtrntype.Contains("Cr_") ? Wtrntype.Replace("Cr_", "") : Wtrntype.Replace("Dr_", "");

                #region SMS_Email
                ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.DebitWalletMsgNotification);
                ActivityNotification.Param1 = coinName;
                ActivityNotification.Param2 = trnType; //ntrivedi 08-02-20019 "6" instead of Withdrawal in notification  routeTrnType.ToString();
                ActivityNotification.Param3 = TrnRefNo.ToString();
                ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Success);
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "OnWalletBalChange + SendActivityNotificationV2 pre TrnNo=" + TrnRefNo.ToString());

                Parallel.Invoke(() => _signalRService.SendActivityNotificationV2(ActivityNotification, dWalletobj.UserID.ToString(), 2),
                   () => _signalRService.OnWalletBalChange(walletMasterObj, coinName, dWalletobj.UserID.ToString(), 2),
                   () => SMSSendAsyncV1(EnTemplateType.SMS_WalletDebited, userID.ToString(), null, null, null, null, coinName, trnType, TrnRefNo.ToString()),
                   () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, userID.ToString(), amount.ToString(), coinName, UTC_To_IST().ToString(), TrnRefNo.ToString(), trnType, "", "", "", "", User));
                #endregion
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WalletHoldNotificationSend Timestamp=" + timestamp, "WalletTransactionCrDr", ex);
            }
        }
    }
}
