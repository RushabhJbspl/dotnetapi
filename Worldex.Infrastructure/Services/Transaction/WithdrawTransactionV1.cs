using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Entities.User;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Data;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.Interfaces.User;
using Worldex.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Worldex.Infrastructure.Services.Transaction
{
    //Uday 09-01-2019  withdraw Process separate with mail confirmation and withdraw internal transfer allow
    public class WithdrawTransactionV1 : IWithdrawTransactionV1
    {
        private readonly ICommonRepository<SellTopUpRequest> _SellTopUpRequest;
        private readonly EFCommonRepository<TransactionQueue> _TransactionRepository;
        private readonly EFCommonRepository<WithdrawHistory> _WithdrawHistoryRepository;
        private readonly ICommonRepository<ServiceMaster> _ServiceConfi;
        private readonly ICommonRepository<AddressMaster> _AddressMasterRepository;
        private readonly ICommonRepository<ActivityTypeHour> _ActivityTypeHour;
        private readonly ICommonRepository<WalletTypeMaster> _WalletTypeMaster;
        private readonly IWalletService _WalletService;
        private readonly IWebApiRepository _WebApiRepository;
        private readonly ICommonRepository<TransactionRequest> _TransactionRequest;
        private readonly ICommonRepository<TransactionStatus> _TransactionStatus;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly IGetWebRequest _IGetWebRequest;
        private readonly IWebApiSendRequest _IWebApiSendRequest;
        private readonly IWebApiData _IWebApiData;
        private readonly ISignalRService _ISignalRService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessageConfiguration _messageConfiguration;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue; //24-11-2018 komal make Email Enqueue
        WebApiParseResponse _WebApiParseResponseObj;
        private readonly IMessageService _messageService;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private readonly ITrnMasterConfiguration _trnMasterConfiguration;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IBackOfficeTrnService _backOfficeTrnService;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;
        private readonly ICommonWalletFunction _commonWalletFunction;
        private readonly WithdrawProcessService _withdrawProcessService;
        private readonly ICommonRepository<WithdrawERCTokenQueue> _WithdrawERCTokenQueueRepository;
        private readonly IWalletTransactionCrDr _walletTransactionCrDr;
        private readonly ICommonRepository<WithdrawAdminRequest> _WithdrawAdminReqCommonRepo;
        // IFiatIntegration _fiatIntegrateService;
        public BizResponse _Resp;
        public List<TransactionProviderResponse> TxnProviderList;
        public List<TransactionProviderResponseForWithdraw> TxnProviderListForWithdraw;
        IUserService _userService;
        TransactionQueue Newtransaction;
        //   string ChargeCurrency;
        NewWithdrawRequestCls Req;
        ProcessTransactionCls _TransactionObj;
        ServiceMaster _TrnService;
        TransactionRequest NewtransactionReq;

        private string ControllerName = "WithdrawTransactionV1";

        public WithdrawTransactionV1(EFCommonRepository<TransactionQueue> TransactionRepository, EncyptedDecrypted encdecAEC,
            ICommonRepository<ServiceMaster> ServiceConfi, ICommonRepository<AddressMaster> AddressMasterRepository,
             IWalletService WalletService, IWebApiRepository WebApiRepository, IUserService userService,
            ICommonRepository<TransactionRequest> TransactionRequest, IGetWebRequest IGetWebRequest,
            IWebApiSendRequest WebApiSendRequest, WebApiParseResponse WebApiParseResponseObj, IWebApiData IWebApiData,
            ISignalRService ISignalRService, UserManager<ApplicationUser> userManager,
            IMessageConfiguration messageConfiguration, ICommonRepository<WithdrawAdminRequest> WithdrawAdminReqCommonRepo, ICommonRepository<SellTopUpRequest> SellTopUpRequest,
            IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, IMessageService messageService,
            IPushNotificationsQueue<SendSMSRequest> pushSMSQueue, ITrnMasterConfiguration trnMasterConfiguration, EFCommonRepository<WithdrawHistory> WithdrawHistoryRepository, Microsoft.Extensions.Configuration.IConfiguration configuration, ICommonRepository<TransactionStatus> TransactionStatus,
            IBackOfficeTrnService backOfficeTrnService, IWithdrawTransactionRepository withdrawTransactionRepository, ICommonWalletFunction commonWalletFunction, WithdrawProcessService withdrawProcessService, ICommonRepository<WithdrawERCTokenQueue> WithdrawERCTokenQueueRepository, IWalletTransactionCrDr walletTransactionCrDr, ICommonRepository<WalletTypeMaster> WalletTypeMaster, ICommonRepository<ActivityTypeHour> ActivityTypeHour)
        {
            _userService = userService;
            _SellTopUpRequest = SellTopUpRequest;
            _TransactionRepository = TransactionRepository;
            _WithdrawAdminReqCommonRepo = WithdrawAdminReqCommonRepo;
            _ServiceConfi = ServiceConfi;
            _AddressMasterRepository = AddressMasterRepository;
            _WalletService = WalletService;
            _WebApiRepository = WebApiRepository;
            _TransactionRequest = TransactionRequest;
            _IGetWebRequest = IGetWebRequest;
            _IWebApiSendRequest = WebApiSendRequest;
            _ActivityTypeHour = ActivityTypeHour;
            _WebApiParseResponseObj = WebApiParseResponseObj;
            _IWebApiData = IWebApiData;
            _ISignalRService = ISignalRService;
            _userManager = userManager;
            _messageConfiguration = messageConfiguration;
            _pushNotificationsQueue = pushNotificationsQueue;
            _messageService = messageService;
            _pushSMSQueue = pushSMSQueue;
            _trnMasterConfiguration = trnMasterConfiguration;
            _WithdrawHistoryRepository = WithdrawHistoryRepository;
            _configuration = configuration;
            _TransactionStatus = TransactionStatus;
            _backOfficeTrnService = backOfficeTrnService;
            _withdrawTransactionRepository = withdrawTransactionRepository;
            _commonWalletFunction = commonWalletFunction;
            _withdrawProcessService = withdrawProcessService;
            _WithdrawERCTokenQueueRepository = WithdrawERCTokenQueueRepository;
            _walletTransactionCrDr = walletTransactionCrDr;
            _WalletTypeMaster = WalletTypeMaster;
            _encdecAEC = encdecAEC;
            // _fiatIntegrateService = fiatIntegrateService;
        }


        #region Process1 Withdrwal Debit Process Only Debit And Send Email
        //Uday 09-01-2019 Withdrwal Debit Process When Success Send Email And Next Process Will be done from email
        public async Task<BizResponse> WithdrawTransactionTransactionAsync(NewWithdrawRequestCls Req1)
        {
            Req = Req1;

            _Resp = await CreateTransaction();
            if (_Resp.ReturnCode != enResponseCodeService.Success)
            {
                HelperForLog.WriteLogIntoFile("ProcessNewTransactionAsync", ControllerName, _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo);
                return _Resp;
            }
            return await CombineAllInitTransactionAsync();
        }

        public async Task<BizResponse> FiatWithdrawTransactionTransactionAsync(NewWithdrawRequestCls Req1)
        {
            Req = Req1;

            _Resp = await FIATCreateTransaction();
            if (_Resp.ReturnCode != enResponseCodeService.Success)
            {
                HelperForLog.WriteLogIntoFile("FiatWithdrawTransactionTransactionAsync", ControllerName, _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo);
                return _Resp;
            }
            return await FiatCombineAllInitTransactionAsync();
        }
        public async Task<BizResponse> FiatCombineAllInitTransactionAsync()
        {
            _Resp = new BizResponse();
            _Resp = new BizResponse();
            try
            {
                var Validation = await ValidateTransaction(_Resp);

                if (!Validation)
                {
                    HelperForLog.WriteLogIntoFile("FiatCombineAllInitTransactionAsync", ControllerName, "Validation fail" + _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo);
                    Task.Run(() => MarkTransactionSystemFail(_Resp.ReturnMsg, _Resp.ErrorCode, Newtransaction));
                    return _Resp;
                }
                Req.ServiceType = (enServiceType)_TrnService.ServiceType;
                Req.WalletTrnType = enWalletTrnType.Withdrawal;

                var DebitResult = await _WalletService.GetWalletDeductionNew(Req.SMSCode, Helpers.GetTimeStamp(), enWalletTranxOrderType.Debit, Req.Amount, Req.MemberID,
                    Req.DebitAccountID, Req.TrnNo, Req.ServiceType, Req.WalletTrnType, Req.TrnType, Req.accessToken);
                Newtransaction.ChargeRs = DebitResult.Charge; //2019-3-6 assign charge param
                Newtransaction.ChargeCurrency = DebitResult.ChargeCurrency; //2019-3-6 assign charge param
                HelperForLog.WriteLogIntoFile("Charges data", ControllerName, "Charge" + DebitResult.Charge + "##ChargeCurrency:" + DebitResult.ChargeCurrency + " #Helpers.GetTimeStamp()#" + Helpers.GetTimeStamp() + "Req.TrnNo :" + Req.TrnNo);
                if (DebitResult.ReturnCode != enResponseCode.Success)
                {
                    _Resp.ReturnMsg = DebitResult.ReturnMsg;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = DebitResult.ErrorCode;
                    Task.Run(() => MarkTransactionSystemFail(_Resp.ReturnMsg, _Resp.ErrorCode, Newtransaction, DebitResult.MinimumAmount, DebitResult.MaximumAmount));
                    HelperForLog.WriteLogIntoFile("FiatCombineAllInitTransactionAsync", ControllerName, "Balance Deduction Fail" + _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo);
                    return _Resp;
                }
                //===================================Make txn HOLD as balance debited=======================
                MarkTransactionHold(EnResponseMessage.ProcessTrn_HoldMsg, enErrorCode.ProcessTrn_Hold, Newtransaction);
                HelperForLog.WriteLogIntoFile("FiatCombineAllInitTransactionAsync", ControllerName, "Transaction/Withdraw Process Start" + "##TrnNo:" + Req.TrnNo);

                Newtransaction.StatusMsg = "Waiting for request confirmation by you";
                _TransactionRepository.Update(Newtransaction);

                _Resp.ReturnMsg = "Hold";
                _Resp.ReturnCode = enResponseCodeService.Success;
                _Resp.ErrorCode = enErrorCode.ProcessTrn_Hold;

                var IsExistFiat = _SellTopUpRequest.GetSingle(i => i.Guid == Req.TrnRefNo);
                if (IsExistFiat != null)
                {
                    IsExistFiat.TrnNo = Req.TrnNo;
                    _SellTopUpRequest.UpdateWithAuditLog(IsExistFiat);
                }


                //await EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Req.SMSCode, Req.TransactionAccount, 4, Newtransaction.GUID + "", "", Newtransaction.ChargeCurrency);

                return _Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("FiatCombineAllInitTransactionAsync:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionProcessInternalError });
            }
        }

        public async Task<BizResponse> CombineAllInitTransactionAsync()
        {
            _Resp = new BizResponse();
            try
            {
                var Validation = await ValidateTransaction(_Resp);

                if (!Validation) //Validation.Result//validation and balance check success
                {
                    HelperForLog.WriteLogIntoFile("CombineAllInitTransactionAsync", ControllerName, "Validation fail" + _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo);
                    Task.Run(() => MarkTransactionSystemFail(_Resp.ReturnMsg, _Resp.ErrorCode, Newtransaction));
                    //return Task.FromResult(_Resp);
                    return _Resp;
                }
                //Rita 16-1-19 change enum as wallet team changed enums
                Req.ServiceType = (enServiceType)_TrnService.ServiceType;
                Req.WalletTrnType = enWalletTrnType.Withdrawal;
                //if (Req.TrnType == enTrnType.Transaction)
                //{
                //    Req.ServiceType = (enServiceType)_TrnService.ServiceType;
                //    //Req.WalletTrnType = enWalletTrnType.;
                //}
                //else if (Req.TrnType == enTrnType.Withdraw)
                //{
                //    Req.ServiceType = (enServiceType)_TrnService.ServiceType;
                //    Req.WalletTrnType = enWalletTrnType.Dr_Withdrawal;
                //}
                //else//trading
                //{
                //    Req.ServiceType = enServiceType.Trading;
                //    Req.WalletTrnType = enWalletTrnType.Dr_Trade;
                //}
                //ntrivedi 15-11-2018
                var DebitResult = await _WalletService.GetWalletDeductionNew(Req.SMSCode, Helpers.GetTimeStamp(), enWalletTranxOrderType.Debit, Req.Amount, Req.MemberID,
                    Req.DebitAccountID, Req.TrnNo, Req.ServiceType, Req.WalletTrnType, Req.TrnType, Req.accessToken);
                Newtransaction.ChargeRs = DebitResult.Charge; //2019-3-6 assign charge param
                Newtransaction.ChargeCurrency = DebitResult.ChargeCurrency; //2019-3-6 assign charge param
                HelperForLog.WriteLogIntoFile("Charges data", ControllerName, "Charge" + DebitResult.Charge + "##ChargeCurrency:" + DebitResult.ChargeCurrency + " #Helpers.GetTimeStamp()#" + Helpers.GetTimeStamp() + "Req.TrnNo :" + Req.TrnNo);
                //DebitResult.ReturnCode = enResponseCode.Success;//komal test only
                if (DebitResult.ReturnCode != enResponseCode.Success)
                {
                    _Resp.ReturnMsg = DebitResult.ReturnMsg;//EnResponseMessage.ProcessTrn_WalletDebitFailMsg;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = DebitResult.ErrorCode;//enErrorCode.ProcessTrn_WalletDebitFail;
                    Task.Run(() => MarkTransactionSystemFail(_Resp.ReturnMsg, _Resp.ErrorCode, Newtransaction, DebitResult.MinimumAmount, DebitResult.MaximumAmount));
                    HelperForLog.WriteLogIntoFile("CombineAllInitTransactionAsync", ControllerName, "Balance Deduction Fail" + _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo);
                    return _Resp;
                }
                //===================================Make txn HOLD as balance debited=======================
                MarkTransactionHold(EnResponseMessage.ProcessTrn_HoldMsg, enErrorCode.ProcessTrn_Hold, Newtransaction);
                HelperForLog.WriteLogIntoFile("CombineAllInitTransactionAsync", ControllerName, "Transaction/Withdraw Process Start" + "##TrnNo:" + Req.TrnNo);

                Newtransaction.StatusMsg = "Waiting for request confirmation by you";
                _TransactionRepository.Update(Newtransaction);

                //Send Success Response And Send Email For Next API Call Process
                _Resp.ReturnMsg = "Hold";
                _Resp.ReturnCode = enResponseCodeService.Success;
                _Resp.ErrorCode = enErrorCode.ProcessTrn_Hold;

                //Send email for next api process
                //2019-3-6 add fee param
                await EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Req.SMSCode, Req.TransactionAccount, 4, Newtransaction.GUID + "", "", Newtransaction.ChargeCurrency);

                return _Resp;
            }
            catch (Exception ex)
            {
                //_log.LogError(ex, "exception,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog("CombineAllInitTransactionAsync:##TrnNo " + Req.TrnNo, ControllerName, ex);
                //return Task.FromResult((new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionProcessInternalError }));
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionProcessInternalError });
            }
        }

        #endregion

        #region RegionInitTransaction    

        public async Task<BizResponse> CreateTransaction()
        {
            decimal Amount2 = 0;
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFile("CreateTransaction", ControllerName, "--1-- Transaction Process For" + Req.TrnType + "##TrnNo:" + Req.TrnNo));

                // var GetWalletIDResult1 = _WalletService.GetWalletID(Req.DebitAccountID);
                //var DupTQResult =_TransactionRepository.GetSingleAsync(item => item.MemberMobile == Req.MemberMobile && item.TransactionAccount == Req.TransactionAccount && item.Amount == Req.Amount && item.TrnDate.AddMinutes(10) > Helpers.UTC_To_IST() && (item.Status == Convert.ToInt16(enTransactionStatus.Hold) || item.Status == Convert.ToInt16(enTransactionStatus.Success)));

                //2019-11-5 added user validation in withdraw
                var UserObj = await _userService.UserValidationForWithdraw(Req.MemberID);
                if (UserObj.ReturnCode != enResponseCode.Success)
                {
                    Req.StatusMsg = UserObj.ReturnMsg;
                    return MarkSystemFailTransaction(UserObj.ErrorCode);
                }

                _TrnService = _trnMasterConfiguration.GetServices().Where(item => item.SMSCode == Req.SMSCode && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                //_TrnService = _ServiceConfi.GetSingle(item => item.SMSCode == Req.SMSCode && item.Status == Convert.ToInt16(ServiceStatus.Active));

                if (_TrnService == null)
                {
                    Req.StatusMsg = EnResponseMessage.ProcessTrn_ServiceProductNotAvailableMsg;
                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_ServiceProductNotAvailable);
                }
                var GetWalletIDResult = _WalletService.GetTransactionWalletByCoin(Req.MemberID, _TrnService.SMSCode);
                //Req.DebitWalletID = await GetWalletIDResult1;
                //var AddResult = _AddressMasterRepository.GetSingleAsync(item => item.WalletId == Req.DebitWalletID && item.Address == Req.TransactionAccount && item.Status == Convert.ToInt16(ServiceStatus.Active));//in withdraw , TransactionAccount has address
                var GetWalletIDResult1 = await GetWalletIDResult;
                if (GetWalletIDResult1.ReturnCode != enResponseCode.Success)
                {
                    Req.StatusMsg = EnResponseMessage.InValidDebitAccountIDMsg;
                    return MarkSystemFailTransaction(enErrorCode.InValidDebitAccountID);
                }
                Task.Run(() => HelperForLog.WriteLogIntoFile("CreateTransaction", ControllerName, "Debit WalletID" + Req.DebitWalletID + "##TrnNo:" + Req.TrnNo));
                Req.DebitWalletID = GetWalletIDResult1.Wallet.id;
                Req.DebitAccountID = GetWalletIDResult1.Wallet.AccWalletID;

                //Check For InternalTransactionAllow Or Not
                var InterTransactionAllowRes = _commonWalletFunction.CheckWithdrawBeneficiary(Req.TransactionAccount, Req.MemberID, Req.SMSCode);
                var InterTransactionAllowResObj = await InterTransactionAllowRes;
                if (InterTransactionAllowResObj.ReturnCode == enResponseCode.Success)
                {
                    if (InterTransactionAllowResObj.ErrorCode == enErrorCode.CheckWithdrawBeneficiary_AddressNotFound)
                    {
                        Req.IsInternalTrn = Convert.ToInt16(EnWithdrwalInternalTransaction.OutSideTransaction);
                    }
                    else if (InterTransactionAllowResObj.ErrorCode == enErrorCode.CheckWithdrawBeneficiary_InternalAddressFound)
                    {
                        Req.IsInternalTrn = Convert.ToInt16(EnWithdrwalInternalTransaction.InternalTransaction);
                    }
                    else if (InterTransactionAllowResObj.ErrorCode == enErrorCode.CheckWithdrawBeneficiary_SelfAddressFound)
                    {
                        Req.StatusMsg = EnResponseMessage.CreateTrn_NoSelfAddressWithdrawAllowMsg;
                        return MarkSystemFailTransaction(enErrorCode.CreateTrn_NoSelfAddressWithdrawAllow);
                    }
                    else//2019-6-29 added else condition
                    {
                        Req.IsInternalTrn = 0;
                        Req.StatusMsg = EnResponseMessage.CreateTrn_CheckWithdrawBeneficiary_Fail;
                        return MarkSystemFailTransaction(enErrorCode.CreateTrn_CheckWithdrawBeneficiary_Fail);
                    }
                }
                else
                {
                    Req.IsInternalTrn = Convert.ToInt16(EnWithdrwalInternalTransaction.InternalTransaction);
                    Req.StatusMsg = EnResponseMessage.CreateTrn_NoSelfAddressWithdrawAllowMsg;
                    return MarkSystemFailTransaction(InterTransactionAllowResObj.ErrorCode);
                }

                //if (Req.DebitWalletID == 0)
                //{
                //    Req.StatusMsg = EnResponseMessage.InValidDebitAccountIDMsg;
                //    return MarkSystemFailTransaction(enErrorCode.InValidDebitAccountID);
                //}


                var DupTQResult = _TransactionRepository.GetSingleAsync(item => item.MemberMobile == Req.MemberMobile && item.TransactionAccount == Req.TransactionAccount && item.Amount == Req.Amount && item.TrnDate.AddMinutes(10) > Helpers.UTC_To_IST() && (item.Status == Convert.ToInt16(enTransactionStatus.Hold) || item.Status == Convert.ToInt16(enTransactionStatus.Success)));
                var DuplicateTxn = await DupTQResult;//DupTQResult
                //var DupTxnResult = _TransactionRepository.GetSingleAsync(item => item.MemberID == Req.MemberID && item.TrnRefNo == Req.TrnRefNo && item.TrnType == Convert.ToInt16(enTrnType.Transaction));
                //var WalletWhiteListResult1 = _WalletService.CheckWithdrawalBene(Req.DebitWalletID, Req.AddressLabel, Req.TransactionAccount, Req.WhitelistingBit);

                if (DuplicateTxn != null)
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnDuplicateTrnMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrnDuplicateTrn);
                }

                if (Req.Amount <= 0) // ntrivedi 02-11-2018 if amount =0 then also invalid
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnInvalidAmountMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrnInvalidAmount);
                }
                //if (Req.TrnRefNo != "0" || !string.IsNullOrEmpty(Req.TrnRefNo))//Rita 13-12-18 no need to check this as already checked above
                //{
                //    var DuplicateTxnWithRefNo = await DupTxnResult;
                //    if (DuplicateTxnWithRefNo != null)
                //    {
                //        Req.StatusMsg = EnResponseMessage.CreateTrnDuplicateTrnMsg;
                //        return MarkSystemFailTransaction(enErrorCode.CreateTrnDuplicateTrn);
                //    }
                //}

                //2019-6-29 commneted , its already check in above method.
                //var AddResult = _AddressMasterRepository.GetSingleAsync(item => item.WalletId == Req.DebitWalletID && item.Address == Req.TransactionAccount && item.Status == Convert.ToInt16(ServiceStatus.Active));//in withdraw , TransactionAccount has address
                //var _AddressMasterObj = await AddResult;
                //if (_AddressMasterObj != null)
                //{
                //    Req.StatusMsg = EnResponseMessage.CreateTrn_NoSelfAddressWithdrawAllowMsg;
                //    return MarkSystemFailTransaction(enErrorCode.CreateTrn_NoSelfAddressWithdrawAllow);
                //}

                //Uday 24-01-2019 Check This Condition in before all fail case, so comment here and this code in above
                //Check For InternalTransactionAllow Or Not
                //var InterTransactionAllowRes =  _commonWalletFunction.CheckWithdrawBeneficiary(Req.TransactionAccount, Req.MemberID, Req.SMSCode);
                //var InterTransactionAllowResObj = await InterTransactionAllowRes;
                //if (InterTransactionAllowResObj.ReturnCode == enResponseCode.Success)
                //{
                //    if(InterTransactionAllowResObj.ErrorCode == enErrorCode.CheckWithdrawBeneficiary_AddressNotFound)
                //    {
                //        Req.IsInternalTrn = Convert.ToInt16(EnWithdrwalInternalTransaction.OutSideTransaction);
                //    }
                //    else if(InterTransactionAllowResObj.ErrorCode == enErrorCode.CheckWithdrawBeneficiary_InternalAddressFound)
                //    {
                //        Req.IsInternalTrn = Convert.ToInt16(EnWithdrwalInternalTransaction.InternalTransaction);
                //    }
                //    else if(InterTransactionAllowResObj.ErrorCode == enErrorCode.CheckWithdrawBeneficiary_SelfAddressFound)
                //    {
                //        Req.StatusMsg = EnResponseMessage.CreateTrn_NoSelfAddressWithdrawAllowMsg;
                //        return MarkSystemFailTransaction(enErrorCode.CreateTrn_NoSelfAddressWithdrawAllow);
                //    }
                //}
                //else
                //{
                //    Req.StatusMsg = EnResponseMessage.CreateTrn_NoSelfAddressWithdrawAllowMsg;
                //    return MarkSystemFailTransaction(InterTransactionAllowResObj.ErrorCode);
                //}

                //2019-7-26 vsolanki remove check bene for Internal transaction
                //Check Whilte listed Address 
                if (Req.IsInternalTrn != Convert.ToInt16(EnWithdrwalInternalTransaction.InternalTransaction))
                {
                    var WalletWhiteListResult1 = _WalletService.CheckWithdrawalBene(Req.DebitWalletID, Req.AddressLabel, Req.TransactionAccount, Req.WhitelistingBit);
                    var WalletWhiteListResult = await WalletWhiteListResult1;
                    if (!WalletWhiteListResult.Equals(enErrorCode.Success))
                    {
                        if (WalletWhiteListResult == enErrorCode.WithdrawNotAllowdBeforehrBene)
                        {
                            int ActivityHour = 24;//2019-6-29 add 24 hour insted of 0 hr
                            var activityhourObj = _ActivityTypeHour.GetSingle(i => i.ActivityType == (int)enActivityType.Benificiary);
                            if (activityhourObj != null)
                            {
                                ActivityHour = activityhourObj.ActivityHour;
                            }

                            Req.StatusMsg = EnResponseMessage.WithdrawNotAllowdBeforeBene;
                            Req.StatusMsg = Req.StatusMsg.Replace("#Hour#", ActivityHour.ToString());
                            return MarkSystemFailTransaction(WalletWhiteListResult, Convert.ToDecimal(ActivityHour));

                        }
                        else
                        {
                            Req.StatusMsg = ((enErrorCode)WalletWhiteListResult).ToString();
                            return MarkSystemFailTransaction(WalletWhiteListResult);

                        }

                        //return MarkSystemFailTransaction(WalletWhiteListResult);
                    }
                }
                //Check Whilte listed Address 
                //var WalletWhiteListResult = await WalletWhiteListResult1;
                //if (!WalletWhiteListResult.Equals(enErrorCode.Success))
                //{
                //    if (WalletWhiteListResult == enErrorCode.WithdrawNotAllowdBeforehrBene)
                //    {
                //        int ActivityHour = 24;//2019-6-29  add 24 hour insted of 0 hr
                //        var activityhourObj = _ActivityTypeHour.GetSingle(i => i.ActivityType == (int)enActivityType.Benificiary);
                //        if (activityhourObj != null)
                //        {
                //            ActivityHour = activityhourObj.ActivityHour;
                //        }

                //        Req.StatusMsg = EnResponseMessage.WithdrawNotAllowdBeforeBene;
                //        Req.StatusMsg = Req.StatusMsg.Replace("#Hour#", ActivityHour.ToString());
                //        return MarkSystemFailTransaction(WalletWhiteListResult, Convert.ToDecimal(ActivityHour));

                //    }
                //    else
                //    {
                //        Req.StatusMsg = ((enErrorCode)WalletWhiteListResult).ToString();
                //        return MarkSystemFailTransaction(WalletWhiteListResult);

                //    }

                //    //return MarkSystemFailTransaction(WalletWhiteListResult);
                //}


                var TxnProviderListResult = _WebApiRepository.GetProviderDataListAsyncForWithdraw(new TransactionApiConfigurationRequest { amount = Req.Amount, SMSCode = Req.SMSCode, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(Req.TrnType) });

                //Uday 02-11-2018 Check Wallet Amount Limit
                //2019-6-29 commneted , its already check in sp
                //var _ServiceLimitCheckObjResult = _WalletService.GetServiceLimitChargeValue(enWalletTrnType.Withdrawal, Req.SMSCode);
                //var _ServiceLimitCheckObj = await _ServiceLimitCheckObjResult;
                //if ((Req.Amount > _ServiceLimitCheckObj.MaxAmount && _ServiceLimitCheckObj.MaxAmount != 0) || (Req.Amount < _ServiceLimitCheckObj.MinAmount && _ServiceLimitCheckObj.MinAmount != 0)) //ntrivedi if limit value is 0 measn not checking value
                //{
                //    Req.StatusMsg = EnResponseMessage.CreateTrn_WithdrawAmountBetweenMinAndMax.Replace("@MIN", _ServiceLimitCheckObj.MinAmount.ToString()).Replace("@MAX", _ServiceLimitCheckObj.MaxAmount.ToString());
                //    return MarkSystemFailTransaction(enErrorCode.TransactionLimitValidationFail, _ServiceLimitCheckObj.MaxAmount, _ServiceLimitCheckObj.MinAmount); //ntrivedi 10-12-2018
                //}
                Req.Status = enTransactionStatus.Initialize;
                Req.StatusCode = Convert.ToInt64(enErrorCode.TransactionInsertSuccess);
                TxnProviderListForWithdraw = await TxnProviderListResult;
                if (TxnProviderListForWithdraw.Count > 0)
                {
                    if (TxnProviderListForWithdraw[0].IsOnlyIntAmountAllow == 1)//IsIntAmountAllow =1 ,only int value is allow not decimal 2019-6-7
                    {
                        decimal d = (decimal)Req.Amount;
                        if ((d % 1) > 0)
                        {
                            //is decimal
                            Req.StatusMsg = EnResponseMessage.CreateTrnInvalidIntAmountMsg;
                            return MarkSystemFailTransaction(enErrorCode.InvalidTronAddress);
                        }
                    }
                    //---2019-5-15 Tron start
                    var WalletTypeObj = _WalletTypeMaster.GetSingle(i => i.WalletTypeName == Req.SMSCode && i.Status == 1);
                    if (WalletTypeObj != null)
                    {
                        if (WalletTypeObj.IsLocal == 2 || WalletTypeObj.IsLocal == 3 || WalletTypeObj.IsLocal == 4)//2-TRX,3-TRC 10,4-TRC-20
                        {
                            var ThirdPartyNewObj = _IGetWebRequest.MakeWebRequestV2("", Req.TransactionAccount, TxnProviderListForWithdraw[0].RouteID, TxnProviderListForWithdraw[0].ThirPartyAPIID, TxnProviderListForWithdraw[0].SerProDetailID, Newtransaction, null, 1);

                            string apiResponse = _IWebApiSendRequest.SendAPIRequestAsync(ThirdPartyNewObj.RequestURL, ThirdPartyNewObj.RequestBody, "application/json", 180000, ThirdPartyNewObj.keyValuePairsHeader, "post");

                            WebAPIParseResponseCls ParsedResponse = _WebApiParseResponseObj.TransactionParseResponse(apiResponse, TxnProviderListForWithdraw[0].ThirPartyAPIID);
                            if (ParsedResponse.Status != enTransactionStatus.Hold)
                            {
                                Req.StatusMsg = EnResponseMessage.CreateTrnInvalidAddressMsg;
                                return MarkSystemFailTransaction(enErrorCode.InvalidTronAddress);
                            }
                            string status = ParsedResponse.ResponseMsg;
                            if (status != "true")
                            {
                                Req.StatusMsg = EnResponseMessage.CreateTrnInvalidAddressMsg;
                                return MarkSystemFailTransaction(enErrorCode.InvalidTronAddress);
                            }
                        }
                    }

                    //---2019-5-15 end

                    //-------2019-6-17


                    var IsValidAddress = await _WalletService.ValidateAddress(Req.TransactionAccount, TxnProviderListForWithdraw[0].AccountNoLen, TxnProviderListForWithdraw[0].AccNoStartsWith, TxnProviderListForWithdraw[0].AccNoValidationRegex);
                    if (IsValidAddress.ErrorCode != enErrorCode.Success) // RUSHABH 23-03-2019 if invalid address 
                    {
                        Req.StatusMsg = EnResponseMessage.CreateTrnInvalidAddressMsg;
                        return MarkSystemFailTransaction(IsValidAddress.ErrorCode);
                    }
                }
                var ForgotPswdObj = _WalletService.CheckActivityLog(Req.MemberID, (int)enActivityType.ForgotPassword);
                if (ForgotPswdObj != 0)//1=false
                {
                    Req.StatusMsg = EnResponseMessage.WithdrawNotAllowdBeforehrForgotPswd;
                    Req.StatusMsg = Req.StatusMsg.Replace("#Hour#", ForgotPswdObj.ToString());
                    return MarkSystemFailTransaction(enErrorCode.WithdrawNotAllowdBeforehrForgotPswd, Convert.ToDecimal(ForgotPswdObj));
                }

                var ResetPswdObj = _WalletService.CheckActivityLog(Req.MemberID, (int)enActivityType.ResetPassword);
                if (ResetPswdObj != 0)//1=false
                {
                    Req.StatusMsg = EnResponseMessage.WithdrawNotAllowdBeforehrResetPswd;
                    Req.StatusMsg = Req.StatusMsg.Replace("#Hour#", ResetPswdObj.ToString());
                    return MarkSystemFailTransaction(enErrorCode.WithdrawNotAllowdBeforehrResetPswd, Convert.ToDecimal(ResetPswdObj));
                }

                //2019-7-16 remove device change checking
                //var ChangeDeviceObj = _WalletService.CheckActivityLog(Req.MemberID, (int)enActivityType.DeviceChange);
                //if (ChangeDeviceObj != 0)//1=false
                //{
                //    Req.StatusMsg = EnResponseMessage.WithdrawNotAllowdBeforehrChangeDevice;
                //    //ChangeDeviceObj = (int)ChangeDeviceObj;
                //    Req.StatusMsg = Req.StatusMsg.Replace("#Hour#", ChangeDeviceObj.ToString());
                //    return MarkSystemFailTransaction(enErrorCode.WithdrawNotAllowdBeforehrChangeDevice, Convert.ToDecimal(ChangeDeviceObj));
                //}
                //-----

                InsertTransactionInQueue();
                return (new BizResponse { ReturnMsg = "", ReturnCode = enResponseCodeService.Success });
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateTransaction:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionInsertInternalError });
            }

        }

        public BizResponse MarkSystemFailTransaction(enErrorCode ErrorCode, decimal MaxAmount = 0, decimal MinAmout = 0)
        {
            try
            {
                Req.Status = enTransactionStatus.SystemFail;
                Req.StatusCode = Convert.ToInt64(ErrorCode);
                InsertTransactionInQueue();

                //DI of SMS here
                //Uday 07-12-2018 send sms when transaction is failed
                SMSSendWithdrwalTransaction(Req.GUID.ToString(), Req.Amount, Req.SMSCode, Req.TransactionAccount, Newtransaction.MemberMobile, 3);

                //Uday 07-12-2018 send email when transaction is failed
                //changhe acWalletId to TransactionAccount 
                EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Req.SMSCode, Req.TransactionAccount, 3, "", "", Newtransaction.ChargeCurrency).GetAwaiter();
                try
                {
                    //komal 16-11-2018 add Activity Notifiation v2
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    //notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);
                    notification.MsgCode = Convert.ToInt32(ErrorCode);  // Uday 06-02-2019  Set Particular Error Code in validation fail
                    notification.Param1 = Req.TrnNo.ToString();
                    notification.Param2 = MinAmout.ToString();
                    notification.Param3 = MaxAmount.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Fail);

                    //Task.Run(() => Parallel.Invoke(()=> SMSSendWithdrwalTransaction(Req.TrnNo, Req.Amount, Req.SMSCode, Req.TransactionAccount, Newtransaction.MemberMobile, 3),
                    //    ()=> _ISignalRService.SendActivityNotificationV2(notification, Req.accessToken),
                    //    ()=> EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, 0, Req.SMSCode, Req.TransactionAccount, 3)));
                    //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);
                    _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2);
                    //_ISignalRService.SendActivityNotification("Transaction Validation Fail TrnNo:" + Req.TrnNo, Req.accessToken);
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteLogIntoFile("ISignalRService Notification Error-MarkSystemFailTransaction", ControllerName, ex.Message + "##TrnNo:" + Newtransaction.Id);
                }
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.Fail, ErrorCode = ErrorCode });
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MarkSystemFailTransaction:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionInsertInternalError });
            }
        }

        public BizResponse InsertTransactionInQueue()//ref long TrnNo
        {
            //_Resp = new BizResponse();
            try
            {
                Newtransaction = new TransactionQueue()
                {
                    TrnDate = Helpers.UTC_To_IST(),
                    GUID = Req.GUID,//Guid.NewGuid(),
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
                    DebitAccountID = Req.DebitAccountID,
                    IsInternalTrn = Req.IsInternalTrn,
                    EmailSendDate = Helpers.UTC_To_IST(), // Uday 15-01-2019  Resend Confirmation Email
                };
                Newtransaction = _TransactionRepository.Add(Newtransaction);
                Req.TrnNo = Newtransaction.Id;
                //Req.GUID = Newtransaction.GUID;

                return (new BizResponse { ReturnMsg = EnResponseMessage.CommSuccessMsgInternal, ReturnCode = enResponseCodeService.Success });
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertTransactionInQueue:##TrnNo " + Req.TrnNo, ControllerName, ex);
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
                throw ex;
            }

        }

        #endregion

        #region RegionProcessTransaction

        public Task<Boolean> ValidateTransaction(BizResponse _Resp)
        {
            //Member Service Disable check here for regular txn
            try
            {
                //TxnProviderList = _WebApiRepository.GetProviderDataList(new TransactionApiConfigurationRequest { amount = Req.Amount, SMSCode = Req.SMSCode, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(Req.TrnType)});

                //Uday  21-01-2019 Not check for provider when transaction is initialize, it will check if transaction is not internal.
                //if (TxnProviderList.Count == 0) //Uday 05-11-2018 check condition for no record
                //{
                //    _Resp.ReturnMsg = EnResponseMessage.ProcessTrn_ServiceProductNotAvailableMsg;
                //    _Resp.ErrorCode = enErrorCode.ProcessTrn_ServiceProductNotAvailable;
                //    _Resp.ReturnCode = enResponseCodeService.Fail;
                //    return Task.FromResult(false);
                //}

                //Make Transaction Initialise
                //var Txn = _TransactionRepository.GetById(Req.TrnNo);
                Newtransaction.MakeTransactionInProcess();
                Newtransaction.SetTransactionStatusMsg(EnResponseMessage.ProcessTrn_InitializeMsg);
                Newtransaction.SetTransactionCode(Convert.ToInt64(enErrorCode.ProcessTrn_Initialize));
                _TransactionRepository.Update(Newtransaction);
                //Take Provider Configuration           

                ////Uday 07-12-2018 send sms when transaction is failed
                //SMSSendWithdrwalTransaction(Req.TrnNo, Req.Amount, Req.SMSCode, Req.DebitAccountID, Newtransaction.MemberMobile, 3);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ValidateTransaction:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return Task.FromResult(false);
            }
        }

        public async void MarkTransactionSystemFail(string StatusMsg, enErrorCode ErrorCode, TransactionQueue Newtransaction, string MinAmount = "", string MaxAmount = "")
        {
            try
            {
                //var Txn = _TransactionRepository.GetById(Req.TrnNo);
                Newtransaction.MakeTransactionSystemFail();
                Newtransaction.SetTransactionStatusMsg(StatusMsg);
                Newtransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
                _TransactionRepository.Update(Newtransaction);
                try
                {
                    //komal 16-11-2018 add Activity Notifiation v2
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    //notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);//for error in display
                    notification.MsgCode = Convert.ToInt32(ErrorCode); // Uday 06-02-2019  Set Particular Error Code in validation fail
                    notification.Param1 = Req.TrnNo.ToString();
                    notification.Param2 = MinAmount;
                    notification.Param3 = MaxAmount;
                    notification.Type = Convert.ToInt16(EnNotificationType.Fail);
                    //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);
                    _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2);
                    //_ISignalRService.SendActivityNotification("Transaction Validation Fail TrnNo:" + Req.TrnNo, Req.accessToken);
                    //Uday 07-12-2018 send sms when transaction is failed
                    SMSSendWithdrwalTransaction(Req.GUID.ToString(), Req.Amount, Req.SMSCode, Req.DebitAccountID, Newtransaction.MemberMobile, 3);
                    //changhe acWalletId to TransactionAccount 
                    await EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Req.SMSCode, Req.TransactionAccount, 3, "", "", Newtransaction.ChargeCurrency);

                    //Task.Run(() => Parallel.Invoke(() => _ISignalRService.SendActivityNotification("Transaction Validation Fail TrnNo:" + Req.TrnNo, Req.accessToken),
                    //    () => SMSSendWithdrwalTransaction(Req.TrnNo, Req.Amount, Req.SMSCode, Req.DebitAccountID, Newtransaction.MemberMobile, 3),
                    //    () => EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, 0, Req.SMSCode, Req.TransactionAccount, 3)));
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteLogIntoFile("ISignalRService Notification Error-MarkTransactionSystemFail", ControllerName, ex.Message + "##TrnNo:" + Newtransaction.Id);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MarkTransactionSystemFail:##TrnNo " + Req.TrnNo, ControllerName, ex);
                // throw ex;
            }
        }

        public async Task MarkTransactionHold(string StatusMsg, enErrorCode ErrorCode, TransactionQueue Newtransaction)
        {

            try
            {
                //var Txn = _TransactionRepository.GetById(Req.TrnNo);
                Newtransaction.MakeTransactionHold();
                Newtransaction.SetTransactionStatusMsg(StatusMsg);
                Newtransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
                _TransactionRepository.Update(Newtransaction);
                try
                {
                    //Rita 26-11-2018 add Activity Notifiation v2
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRWithdrawTrnSuccessfullyCreated);
                    notification.Type = Convert.ToInt16(EnNotificationType.Success);
                    notification.Param1 = Req.TrnNo.ToString();
                    notification.Param2 = Req.Amount.ToString();
                    notification.Param3 = Req.SMSCode.ToString();
                    notification.Param4 = Req.DebitAccountID;
                    //Task.Run(()=>_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken));
                    Task.Run(() => _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2));

                    //changhe acWalletId to TransactionAccount 

                    //Task.Run(()=>Parallel.Invoke(()=> _ISignalRService.SendActivityNotificationV2(notification, Req.accessToken),
                    //    ()=> SMSSendWithdrwalTransaction(Newtransaction.Id, Req.Amount, Req.SMSCode, Req.TransactionAccount, Newtransaction.MemberMobile, 1),
                    //    ()=> EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, 0, Req.SMSCode, Req.TransactionAccount, 1)));

                    //Uday 07-12-2018 send sms when transaction is successfully created
                    SMSSendWithdrwalTransaction(Newtransaction.GUID.ToString(), Req.Amount, Req.SMSCode, Req.TransactionAccount, Newtransaction.MemberMobile, 1);

                    //Uday 07-12-2018 send email when transaction is successfully created
                    await EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Req.SMSCode, Req.TransactionAccount, 1, "", "", Newtransaction.ChargeCurrency);
                }
                catch (Exception ex)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFile("ISignalRService Notification Error-MarkTransactionSystemFail", ControllerName, ex.Message + "##TrnNo:" + Req.TrnNo));
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MarkTransactionHold:##TrnNo " + Req.TrnNo, ControllerName, ex);
                throw ex;
            }
        }

        public async Task MarkTransactionOperatorFail(string StatusMsg, enErrorCode ErrorCode, TransactionQueue Newtransaction)
        {
            //CreditWalletDrArryTrnID[] CreditWalletDrArryTrnIDObj = new CreditWalletDrArryTrnID[1];
            List<CreditWalletDrArryTrnID> CreditWalletDrArryTrnIDList = new List<CreditWalletDrArryTrnID>();
            try
            {
                ////replace req to Newtransaction
                //var Txn = _TransactionRepository.GetById(Req.TrnNo);
                Newtransaction.MakeTransactionOperatorFail();
                Newtransaction.SetTransactionStatusMsg(StatusMsg);
                Newtransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
                _TransactionRepository.Update(Newtransaction);

                //Cr Amount to member back
                //CreditWalletDrArryTrnIDObj[0].DrTrnRefNo = Req.TrnNo;
                //CreditWalletDrArryTrnIDObj[0].Amount = Req.Amount;
                HelperForLog.WriteLogIntoFile("MarkTransactionOperatorFail", "WalletTransactionCrDr", "Newtransaction:" + Helpers.JsonSerialize(Newtransaction));

                CreditWalletDrArryTrnIDList.Add(new CreditWalletDrArryTrnID { DrTrnRefNo = Newtransaction.Id, Amount = Newtransaction.Amount });

                //Uday 06-02-2019  Comment GetWalletCreditNew Function, used new fucntion GetWalletCreditNewAsync as per instruction of nupoora mam
                //enWalletTrnType.Cr_Refund
                //_WalletService.GetWalletCreditNew(Req.SMSCode, Helpers.GetTimeStamp(), enWalletTrnType.Refund, Req.Amount, Req.MemberID,
                //Req.DebitAccountID, CreditWalletDrArryTrnIDList.ToArray(), Req.TrnNo,1, enWalletTranxOrderType.Credit, Req.ServiceType, (enTrnType)Newtransaction.TrnType);
                HelperForLog.WriteLogIntoFile("MarkTransactionOperatorFail", "WalletTransactionCrDr", "Newtransaction: Before" + Helpers.JsonSerialize(Newtransaction));

                var response = await _walletTransactionCrDr.GetWalletCreditNewAsync(Newtransaction.SMSCode, Helpers.GetTimeStamp(), enWalletTrnType.Withdrawal, Newtransaction.Amount, Newtransaction.MemberID,
                Newtransaction.DebitAccountID, CreditWalletDrArryTrnIDList.ToArray(), Newtransaction.Id, 1, enWalletTranxOrderType.Credit, enServiceType.WalletService, (enTrnType)Newtransaction.TrnType, "", Newtransaction.GUID.ToString());

                HelperForLog.WriteLogIntoFile("MarkTransactionOperatorFail", "WalletTransactionCrDr", "Newtransaction: after" + Helpers.JsonSerialize(response));

                try
                {
                    //komal 16-11-2018 add Activity Notifiation v2
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionFailed);
                    notification.Param1 = Newtransaction.Id.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Fail);
                    //Task.Run(()=>_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken));
                    Task.Run(() => _ISignalRService.SendActivityNotificationV2(notification, Newtransaction.MemberID.ToString(), 2));
                    //_ISignalRService.SendActivityNotification("Transaction Failed TrnNo:" + Req.TrnNo, Req.accessToken);

                    //Task.Run(() => Parallel.Invoke(() => _ISignalRService.SendActivityNotificationV2(notification, Req.accessToken),
                    //    ()=> SMSSendWithdrwalTransaction(Req.TrnNo, Req.Amount, Req.SMSCode, Req.TransactionAccount, Newtransaction.MemberMobile, 3),
                    //    ()=> EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, 0, Req.SMSCode, Req.TransactionAccount, 3)));

                    //Uday 07-12-2018 send sms when transaction is Failed
                    SMSSendWithdrwalTransaction(Newtransaction.GUID.ToString(), Newtransaction.Amount, Newtransaction.SMSCode, Newtransaction.TransactionAccount, Newtransaction.MemberMobile, 3);
                    //changhe acWalletId to TransactionAccount 

                    //Uday 07-12-2018 send email when transaction is Failed
                    await EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Newtransaction.Id, Newtransaction.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Newtransaction.SMSCode, Newtransaction.TransactionAccount, 3, "", "", Newtransaction.ChargeCurrency);
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteLogIntoFile("ISignalRService Notification Error--Operator fail", ControllerName, ex.Message + "##TrnNo:" + Newtransaction.Id);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MarkTransactionOperatorFail:##TrnNo " + Newtransaction.Id, ControllerName, ex);
                throw ex;
            }
        }

        public async Task MarkTransactionOperatorFailv2(string StatusMsg, enErrorCode ErrorCode, TransactionQueue Newtransaction)
        {
            //CreditWalletDrArryTrnID[] CreditWalletDrArryTrnIDObj = new CreditWalletDrArryTrnID[1];
            List<CreditWalletDrArryTrnID> CreditWalletDrArryTrnIDList = new List<CreditWalletDrArryTrnID>();
            try
            {
                //var Txn = _TransactionRepository.GetById(Req.TrnNo);
                Newtransaction.MakeTransactionOperatorFail();
                Newtransaction.SetTransactionStatusMsg(StatusMsg);
                Newtransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
                _TransactionRepository.Update(Newtransaction);

                //Cr Amount to member back
                //CreditWalletDrArryTrnIDObj[0].DrTrnRefNo = Req.TrnNo;
                //CreditWalletDrArryTrnIDObj[0].Amount = Req.Amount;
                CreditWalletDrArryTrnIDList.Add(new CreditWalletDrArryTrnID { DrTrnRefNo = Newtransaction.Id, Amount = Newtransaction.Amount });


                await _walletTransactionCrDr.GetWalletCreditNewAsync(Newtransaction.SMSCode, Helpers.GetTimeStamp(), enWalletTrnType.Withdrawal, Newtransaction.Amount, Newtransaction.MemberID,
                Newtransaction.DebitAccountID, CreditWalletDrArryTrnIDList.ToArray(), Newtransaction.Id, 1, enWalletTranxOrderType.Credit, enServiceType.Recharge, (enTrnType)Newtransaction.TrnType, "", Newtransaction.GUID.ToString());

                try
                {
                    //komal 16-11-2018 add Activity Notifiation v2
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionFailed);
                    notification.Param1 = Req.TrnNo.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Fail);

                    Task.Run(() => _ISignalRService.SendActivityNotificationV2(notification, Newtransaction.MemberID.ToString(), 2));

                    //Uday 07-12-2018 send sms when transaction is Failed
                    SMSSendWithdrwalTransaction(Newtransaction.GUID.ToString(), Newtransaction.Amount, Newtransaction.SMSCode, Newtransaction.TransactionAccount, Newtransaction.MemberMobile, 3);
                    //changhe acWalletId to TransactionAccount 

                    //Uday 07-12-2018 send email when transaction is Failed
                    await EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Newtransaction.Id, Newtransaction.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Newtransaction.SMSCode, Newtransaction.TransactionAccount, 3, "", "", Newtransaction.ChargeCurrency);
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteLogIntoFile("ISignalRService Notification Error--Operator fail", ControllerName, ex.Message + "##TrnNo:" + Newtransaction.Id);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MarkTransactionOperatorFail:##TrnNo " + Req.TrnNo, ControllerName, ex);
                throw ex;
            }
        }

        public async void MarkTransactionSuccess(string StatusMsg, TransactionQueue Newtransaction)
        {
            try
            {
                Newtransaction.MakeTransactionSuccess();
                Newtransaction.SetTransactionStatusMsg(StatusMsg);//WebAPIParseResponseClsObj.StatusMsg)
                _TransactionRepository.Update(Newtransaction);
                try
                {
                    //komal 16-11-2018 add Activity Notifiation v2
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionSuccess);
                    notification.Param1 = Req.TrnNo.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Success);
                    //Task.Run(() => _ISignalRService.SendActivityNotificationV2(notification, Req.accessToken));
                    Task.Run(() => _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2));
                    //_ISignalRService.SendActivityNotification("Transaction Success TrnNo:"+ Req.TrnNo, Req.accessToken);

                    //Uday 07-12-2018 send sms when transaction is success
                    SMSSendWithdrwalTransaction(Req.GUID.ToString(), Req.Amount, Req.SMSCode, Req.TransactionAccount, Newtransaction.MemberMobile, 2);
                    //changhe acWalletId to TransactionAccount 

                    //Uday 07-12-2018 send email when transaction is Success
                    await EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Req.SMSCode, Req.TransactionAccount, 2, "", Newtransaction.TrnDate.ToString(), Newtransaction.ChargeCurrency);
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteLogIntoFile("ISignalRService Notification Error-Success", ControllerName, ex.Message + "##TrnNo:" + Newtransaction.Id);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MarkTransactionSuccess:##TrnNo " + Req.TrnNo, ControllerName, ex);
                throw ex;
            }
        }

        public void InsertIntoWithdrawHistory(long SerProID, string RouteTag, string TrnID, string ProviderWalletID, NewWithdrawRequestCls Req)
        {
            try
            {
                WithdrawHistory WH = new WithdrawHistory();
                WH.SMSCode = Req.SMSCode;
                WH.TrnID = TrnID;
                WH.WalletId = Req.DebitWalletID;
                WH.Address = Req.TransactionAccount;
                WH.ToAddress = "";
                WH.Confirmations = 0;
                WH.Value = 0;
                WH.Amount = Req.Amount;
                WH.Charge = 0;
                WH.Status = 0; // ntrivedi status=6 to pick from deposition tps
                WH.confirmedTime = "";
                WH.unconfirmedTime = "";
                WH.CreatedDate = Helpers.UTC_To_IST();
                WH.State = 0;
                WH.IsProcessing = 0;
                WH.TrnNo = Req.TrnNo;
                WH.RouteTag = RouteTag;
                WH.UserId = Req.MemberID;
                WH.SerProID = SerProID;
                WH.TrnDate = Helpers.UTC_To_IST();
                WH.APITopUpRefNo = "";
                WH.createdTime = Helpers.UTC_To_IST().ToString();
                WH.SystemRemarks = "Initial entry Created";
                WH.ProviderWalletID = ProviderWalletID;
                WH.GUID = Guid.NewGuid().ToString().Replace("-", "");//2019-7-13 added GUID in insert

                _WalletService.InsertIntoWithdrawHistory(WH);
                HelperForLog.WriteLogIntoFile("InsertIntoWithdrawHistory", ControllerName, "--1--Insert Withdarw History---" + "##TrnNo:" + Req.TrnNo);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertIntoWithdrawHistory Error-Fail:##TrnNo " + Req.TrnNo, ControllerName, ex);
            }
        }

        public void UpdateIntoWithdrawHistory(NewWithdrawRequestCls Req, short status, string StatusText, string ProviderWalletId = null, string trnid = "")
        {
            try
            {
                var WH = _WithdrawHistoryRepository.FindBy(e => e.TrnNo == Req.TrnNo).OrderByDescending(e => e.Id).FirstOrDefault();
                WH.Status = status;
                WH.SystemRemarks = StatusText;
                WH.ProviderWalletID = ProviderWalletId;
                if (trnid != "") //ntrivedi 04-01-2018
                {
                    WH.TrnID = trnid;
                }
                _WithdrawHistoryRepository.Update(WH);
                HelperForLog.WriteLogIntoFile("UpdateIntoWithdrawHistory", ControllerName, "--1--Update Withdarw History---" + "##TrnNo:" + Req.TrnNo);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertIntoWithdrawHistory Error-Fail:##TrnNo " + Req.TrnNo, ControllerName, ex);
            }
        }

        #endregion

        #region Process2 Withdrwal API Call Process From Email

        public async Task<BizResponse> WithdrawTransactionAPICallProcessAsync(WithdrawalConfirmationRequest Request, long UserId, short IsReqFromAdmin, ApplicationUser user = null)
        {
            try
            {
                BizResponse _Resp = new BizResponse();
                //var fiatObj = _SellTopUpRequest.GetSingle(i => i.Guid == Request.RefNo && i.APIStatus == 0);
                //if (fiatObj != null)
                //{
                //    FiatSellConfirmReq reqobj = new FiatSellConfirmReq();
                //    reqobj.TransactionBit = Convert.ToInt16(Request.TransactionBit);
                //    reqobj.TrnId = Request.RefNo;
                //    var responseObj = _fiatIntegrateService.FiatSellRequestConfirmation(reqobj, user);
                //    _Resp.ReturnCode = (enResponseCodeService)responseObj.ReturnCode;
                //    _Resp.ReturnMsg = responseObj.ReturnMsg;
                //    _Resp.ErrorCode = responseObj.ErrorCode;
                //    return _Resp;
                //}

                //get transaction from transactionqueue              
                var tranTQobj = _TransactionRepository.GetSingle(x => x.GUID.ToString() == Request.RefNo);
                Newtransaction = new TransactionQueue();
                Newtransaction = tranTQobj;

                if (tranTQobj == null)
                {
                    _Resp.ReturnMsg = "Transaction Not Found";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_TranNoNotFound;
                    return _Resp;
                }

                //Validate User
                if (Newtransaction.MemberID != UserId)
                {
                    _Resp.ReturnMsg = "You have not authorized to doing this transaction";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_NotAuthorizeToDoingTransaction;

                    //2019-6-29 updated to dont allow to doing transction to unauthorized user
                    Newtransaction.Status = 3;
                    Newtransaction.StatusMsg = _Resp.ReturnMsg;
                    Newtransaction.StatusCode = Convert.ToInt64(_Resp.ErrorCode);
                    _TransactionRepository.Update(Newtransaction);

                    return _Resp;
                }

                //Check Already Processed or not
                if (IsReqFromAdmin != 1 && Newtransaction.IsVerified != 0)
                {
                    _Resp.ReturnMsg = "Transaction Already Verified";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionAlreadyVerified;
                    return _Resp;
                }

                //Validate Transaction
                if (Newtransaction.Status != 4)
                {
                    _Resp.ReturnMsg = "Transaction Not In Hold State";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_TranNoHoldState;

                    return _Resp;
                }

                //Check Bit Value
                if (Request.TransactionBit != 1 && Request.TransactionBit != 2)
                {
                    _Resp.ReturnMsg = "Invalid Transaction Bit Value";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_InvalidTransactionBitValue;

                    //2019-6-29 updated to dont allow to doing transction if another bit is in request
                    Newtransaction.Status = 2;
                    Newtransaction.StatusMsg = _Resp.ReturnMsg;
                    Newtransaction.StatusCode = Convert.ToInt64(_Resp.ErrorCode);
                    _TransactionRepository.Update(Newtransaction);

                    return _Resp;
                }
                if (Request.TransactionBit == 2)
                {
                    //Update IsVerify Bit
                    Newtransaction.IsVerified = 9;
                    _TransactionRepository.Update(Newtransaction);
                }
                else if (Request.TransactionBit == 1)
                {
                    //Update IsVerify Bit
                    Newtransaction.IsVerified = 1;
                    Newtransaction.StatusMsg = "Transaction Verified By Email";
                    _TransactionRepository.Update(Newtransaction);
                }
                else
                {
                    //2019-6-29 updated to dont allow to doing transction if another bit is in request
                    Newtransaction.Status = 2;
                    Newtransaction.StatusMsg = "Invalid TransactionBit.";
                    Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.Withdrwal_InvalidTransactionBitValue);
                    _TransactionRepository.Update(Newtransaction);
                }
                if (Request.TransactionBit == 2) //Reject Transaction Process
                {
                    //Process For Credit Process
                    WithdrawalReconRequest withdrawalReconRequest = new WithdrawalReconRequest();
                    withdrawalReconRequest.ActionType = enWithdrawalReconActionType.Refund;
                    withdrawalReconRequest.ActionMessage = "Withdrwal Transaction Request Cancel By User";
                    withdrawalReconRequest.TrnNo = Newtransaction.Id;

                    var ReconResponse = _backOfficeTrnService.WithdrawalRecon(withdrawalReconRequest, UserId, "");

                    if (ReconResponse.ReturnCode == enResponseCode.Success)
                    {
                        _Resp.ReturnMsg = "Transaction Cancel Success.";
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionCancelSuccess;

                        //Update Message Bit
                        _TransactionRepository.ReloadEntity(Newtransaction);    //Uday 28-01-2019 reload tq object bcas recon chnages not reflect in this object
                        Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.WithdrwalCancelByUser);
                        Newtransaction.StatusMsg = "Cancelled By User";
                        _TransactionRepository.Update(Newtransaction);

                        //Send Email For Reject Process
                        await EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Newtransaction.Id, Newtransaction.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Newtransaction.SMSCode, Newtransaction.TransactionAccount, 5, "", Newtransaction.TrnDate.ToString(), Newtransaction.ChargeCurrency);
                    }
                    else
                    {
                        //Update Message Bit
                        Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.WithdrwalCancelProcessFail);
                        Newtransaction.StatusMsg = "Withdrwal Cancel Process Fail";
                        _TransactionRepository.Update(Newtransaction);

                        _Resp.ReturnMsg = "Transaction Cancel Fail.";
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionCancelFail;

                        HelperForLog.WriteErrorLog("WithdrawTransactionAPICallProcessAsync #WithdrawTransactionCancelProcessFail#: ##RefNo " + Request.RefNo + " ", ControllerName + " ErrorCode : " + ReconResponse.ErrorCode + "  ReturnMsg : " + ReconResponse.ReturnMsg, new Exception());
                    }
                }
                else if (Request.TransactionBit == 1) //Accept Transaction Process
                {
                    //Check Transaction TimeOut
                    //Uday 15-01-2019 Resend Email, So Check with EmailSendDate Insted Of TrnDate
                    //If Request Through Email Then & Only Then Check This Condition Rushabh 29-05-2019
                    if (IsReqFromAdmin == 0)
                    {
                        var TranTimeOut = _configuration["WithdrawTimeLimit"].ToString();
                        if (Newtransaction.EmailSendDate.AddMinutes(Convert.ToInt32(TranTimeOut)) <= Helpers.UTC_To_IST())
                        {
                            _Resp.ReturnMsg = "The link is expired. Please resend confirmation mail from hisotry.";
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionTimeOut;

                            //Uday 28-01-2019 When Transaction timeout user also able to regenrate the mail
                            Newtransaction.StatusMsg = _Resp.ReturnMsg;
                            Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.Withdrwal_TransactionTimeOut);
                            Newtransaction.IsVerified = 0;

                            ////Uday 28-01-2019  Transaction timeout so update the tq statuscode and statusmsg
                            //Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.Withdrwal_TransactionTimeOut);
                            //Newtransaction.StatusMsg = "Transaction TimeOut";
                            _TransactionRepository.Update(Newtransaction);

                            return _Resp;
                        }
                    }
                    var GetWalletIDResult1 = _WalletService.GetWalletID(Newtransaction.DebitAccountID);
                    Req = new NewWithdrawRequestCls();
                    Req.DebitWalletID = await GetWalletIDResult1;
                    Req.TrnType = enTrnType.Withdraw;
                    Req.SMSCode = Newtransaction.SMSCode;
                    Req.Amount = Newtransaction.Amount;
                    Req.TrnNo = Newtransaction.Id;
                    Req.TrnRefNo = Newtransaction.TrnRefNo;
                    Req.MemberID = Newtransaction.MemberID;
                    Req.DebitAccountID = Newtransaction.DebitAccountID;
                    Req.TransactionAccount = Newtransaction.TransactionAccount;

                    ////Check For Admin Approval
                    //var TxnProviderListResult = _WebApiRepository.GetProviderDataListAsyncForWithdraw(new TransactionApiConfigurationRequest { amount = Newtransaction.Amount, SMSCode = Newtransaction.SMSCode, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.Withdraw) });
                    //TxnProviderListForWithdraw = await TxnProviderListResult;                    


                    //Check For Internal Transfer Allow Process
                    //Newtransaction.IsInternalTrn = 2;
                    if ((EnWithdrwalInternalTransaction)Newtransaction.IsInternalTrn == EnWithdrwalInternalTransaction.InternalTransaction)
                    {
                        //Insert Into Withdraw History
                        InsertIntoWithdrawHistory(0, "", Guid.NewGuid().ToString(), "", Req);

                        //Call Internal Transfer Allow SP
                        var InternalTransferResp = _withdrawTransactionRepository.WithdrwalInteranlTransferProcess(Request.RefNo, Helpers.GetTimeStamp(), 21);

                        if (InternalTransferResp != null)
                        {
                            if (InternalTransferResp.ReturnCode == enResponseCode.Success)
                            {
                                MarkTransactionSuccess("Internal Transaction", Newtransaction);
                                Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.WithdrwalInternalTransactionSuccess);
                                _TransactionRepository.Update(Newtransaction);

                                _Resp.ReturnMsg = "Transaction Confirm Success.";
                                _Resp.ReturnCode = enResponseCodeService.Success;
                                _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionConfirmSuccess;

                                UpdateIntoWithdrawHistory(Req, 1, "Success", "", "");
                            }
                            else
                            {
                                MarkTransactionSystemFail("Fail", InternalTransferResp.ErrorCode, Newtransaction);

                                _Resp.ReturnMsg = "Transaction Confirm Fail.";
                                _Resp.ReturnCode = enResponseCodeService.Fail;
                                _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionConfirmFail;

                                UpdateIntoWithdrawHistory(Req, 2, "Fail", "");

                                HelperForLog.WriteErrorLog("WithdrawTransactionAPICallProcessAsync #WithdrawTransactionInternalAllow#: ##RefNo " + Request.RefNo + " ", ControllerName + " ErrorCode : " + InternalTransferResp.ErrorCode + "  ReturnMsg : " + InternalTransferResp.ReturnMsg, new Exception());
                            }
                        }
                        else
                        {
                            MarkTransactionSystemFail("Fail", InternalTransferResp.ErrorCode, Newtransaction);

                            _Resp.ReturnMsg = "Transaction Confirm Fail.";
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionConfirmFail;

                            UpdateIntoWithdrawHistory(Req, 2, "Fail", "");

                            HelperForLog.WriteErrorLog("WithdrawTransactionAPICallProcessAsync #WithdrawTransactionSPError#: ##RefNo " + Request.RefNo + " ", ControllerName, new Exception());
                        }
                    }
                    else if ((EnWithdrwalInternalTransaction)Newtransaction.IsInternalTrn == EnWithdrwalInternalTransaction.OutSideTransaction)
                    {
                        var TransactionRequestData = _TransactionRequest.GetSingle(x => x.TrnNo == Newtransaction.Id);
                        if (TransactionRequestData != null)
                        {
                            _Resp.ReturnMsg = "Transaction Already Requested.";
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionAlreadyRequested;

                            return _Resp;
                        }

                        var TxnProviderListResult = _WebApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { amount = tranTQobj.Amount, SMSCode = tranTQobj.SMSCode, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(tranTQobj.TrnType) });
                        TxnProviderList = await TxnProviderListResult;
                        //Added By Rushabh 29-05-2019 For Admin Approval Process
                        if (TxnProviderList.Count > 0)
                        {
                            if (TxnProviderList[0].IsAdminApprovalRequired == 1)
                            {
                                if (Newtransaction.IsVerifiedByAdmin == 0) //29-05-2019 rushabh 
                                {
                                    WithdrawAdminRequest NewObj = new WithdrawAdminRequest
                                    {
                                        TrnNo = Newtransaction.Id,
                                        CreatedBy = Newtransaction.MemberID,
                                        CreatedDate = Helpers.UTC_To_IST(),
                                        Status = 0
                                    };
                                    _WithdrawAdminReqCommonRepo.Add(NewObj);

                                    Newtransaction.IsVerifiedByAdmin = 0;
                                    Newtransaction.StatusMsg = "Waiting For Admin Approval";
                                    _TransactionRepository.Update(Newtransaction);

                                    _Resp.ErrorCode = enErrorCode.WithdrawalReqSentToAdmin;
                                    _Resp.ReturnCode = enResponseCodeService.Success;
                                    _Resp.ReturnMsg = "Withdrawal Request Has Been Sent For Admin Approval";

                                    //need to add signal-r call
                                    ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                                    ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.WithdrawalReqSentToAdmin);
                                    ActivityNotification.Param1 = Newtransaction.Id.ToString();
                                    ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);
                                    Parallel.Invoke(() => _ISignalRService.SendActivityNotificationV2(ActivityNotification, UserId.ToString(), 0));

                                    //Send Email For Admin Approval Notification
                                    await EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Newtransaction.Id, Newtransaction.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Newtransaction.SMSCode, Newtransaction.TransactionAccount, 6, "", Newtransaction.TrnDate.ToString(), Newtransaction.ChargeCurrency);
                                    return _Resp;
                                }
                                else
                                {
                                    _Resp = await CallWebAPI(_Resp);
                                }
                            }
                            _Resp = await CallWebAPI(_Resp); //end 29-05-2019
                        }
                        //_Resp = await CallWebAPI(_Resp);
                    }
                }
                else
                {
                    _Resp.ReturnMsg = "Invalid Transaction Bit Value";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_InvalidTransactionBitValue;

                    //2019-6-29 updated to dont allow to doing transction if another bit is in request
                    Newtransaction.Status = 2;
                    Newtransaction.StatusMsg = "Invalid TransactionBit.";
                    Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.Withdrwal_InvalidTransactionBitValue);
                    _TransactionRepository.Update(Newtransaction);
                }
                return _Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WithdrawTransactionAPICallProcessAsync:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionProcessInternalError });
            }
        }

        public async Task<BizResponse> CallWebAPI(BizResponse _Resp)
        {
            //TransactionRequest TransactionRequestObj=new TransactionRequest(); 
            ThirdPartyAPIRequest ThirdPartyAPIRequestOnj;
            WebApiConfigurationResponse WebApiConfigurationResponseObj;
            WebAPIParseResponseCls WebAPIParseResponseClsObj = new WebAPIParseResponseCls();
            _TransactionObj = new ProcessTransactionCls();
            long WithdrawERCTokenQueueId = 0;

            //long TxnRequestID = 0;
            short IsTxnProceed = 0;
            try
            {
                //2019-11-21 vsolanki check balance at call thirdpaty api
                var GetWalletIDResult = _WalletService.GetTransactionWalletByCoin(Newtransaction.MemberID, Newtransaction.SMSCode);
                var GetWalletIDResult1 = await GetWalletIDResult;
                if (GetWalletIDResult1.ReturnCode != enResponseCode.Success)
                {
                    Req.StatusMsg = EnResponseMessage.InValidDebitAccountIDMsg;
                    return MarkSystemFailTransaction(enErrorCode.InValidDebitAccountID);
                }
                if (GetWalletIDResult1.Wallet.Balance < 0 || GetWalletIDResult1.Wallet.OutBoundBalance < 0)
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnInvalidAmountMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrnInvalidAmount);
                }

                foreach (TransactionProviderResponse Provider in TxnProviderList)//Make txn on every API
                {

                    Newtransaction.SetServiceProviderData(Provider.ServiceID, Provider.ServiceProID, Provider.ProductID, Provider.RouteID, Provider.SerProDetailID, 0);
                    _TransactionRepository.Update(Newtransaction);

                    WebApiConfigurationResponseObj = _IWebApiData.GetAPIConfiguration(Provider.ThirPartyAPIID);
                    if (WebApiConfigurationResponseObj == null)
                    {
                        _Resp.ReturnMsg = EnResponseMessage.ProcessTrn_ThirdPartyDataNotFoundMsg;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ErrorCode = enErrorCode.ProcessTrn_ThirdPartyDataNotFound;
                        //MarkTransactionOperatorFail(_Resp.ReturnMsg, _Resp.ErrorCode, Newtransaction);
                        continue;
                    }

                    if (Req.TrnType == enTrnType.Withdraw)
                        InsertIntoWithdrawHistory(Provider.ServiceProID, Provider.RouteName, WebAPIParseResponseClsObj.TrnRefNo, Provider.ProviderWalletID, Req);//rita 4-1-19 added ProviderWalletID ,also add in inner join configuration

                    HelperForLog.WriteLogIntoFile("InsertIntoWithdrawHistory", ControllerName, "--2--Insert Withdarw History---" + "##TrnNo:" + Req.TrnNo);
                    ThirdPartyAPIRequestOnj = _IGetWebRequest.MakeWebRequest(Provider.RouteID, Provider.ThirPartyAPIID, Provider.SerProDetailID, Newtransaction);
                    //Insert API request Data
                    _TransactionObj.TransactionRequestID = InsertTransactionRequest(Provider, ThirdPartyAPIRequestOnj.RequestURL + "::" + ThirdPartyAPIRequestOnj.RequestBody);
                    //vsolanki 2019-11-21 added into status table for securiry
                    InsertTransactionStatus(Provider, ThirdPartyAPIRequestOnj.RequestURL + "::" + ThirdPartyAPIRequestOnj.RequestBody);

                    switch (Provider.AppTypeID)
                    {
                        case (long)enAppType.WebSocket:

                        case (long)enAppType.JsonRPC:
                            _TransactionObj.APIResponse = _IWebApiSendRequest.SendJsonRpcAPIRequestAsync(ThirdPartyAPIRequestOnj.RequestURL, ThirdPartyAPIRequestOnj.RequestBody, ThirdPartyAPIRequestOnj.keyValuePairsHeader);
                            break;
                        case (long)enAppType.TCPSocket:

                        case (long)enAppType.RestAPI:
                            _TransactionObj.APIResponse = _IWebApiSendRequest.SendAPIRequestAsync(ThirdPartyAPIRequestOnj.RequestURL, ThirdPartyAPIRequestOnj.RequestBody, WebApiConfigurationResponseObj.ContentType, 30000, ThirdPartyAPIRequestOnj.keyValuePairsHeader, WebApiConfigurationResponseObj.MethodType);
                            break;
                        case (long)enAppType.HttpApi:
                            _TransactionObj.APIResponse = _IWebApiSendRequest.SendAPIRequestAsync(ThirdPartyAPIRequestOnj.RequestURL, ThirdPartyAPIRequestOnj.RequestBody, WebApiConfigurationResponseObj.ContentType, 90000, ThirdPartyAPIRequestOnj.keyValuePairsHeader, WebApiConfigurationResponseObj.MethodType);//ntrivedi 07-08-2019 timeout 30000 to 90000 as 114 server slow
                            break;
                        case (long)enAppType.SocketApi:

                        case (long)enAppType.BitcoinDeamon:

                        case (long)enAppType.Erc20Withdraw: //For Erc20 Provider Withdraw, So Change its makerequest

                            //Get AdminAddress 
                            var AdminAddress = _withdrawProcessService.GetWithdrawERCAdminAddress(Provider.ServiceID, Provider.ThirPartyAPIID);

                            if (AdminAddress != null)
                            {
                                //Insert Into WQueue
                                WithdrawERCTokenQueueId = InsertIntoWithdrawERCTokenQueue(NewtransactionReq.TrnNo, AdminAddress);

                                ThirdPartyAPIRequestOnj = _IGetWebRequest.MakeWebRequest(Provider.RouteID, Provider.ThirPartyAPIID, Provider.SerProDetailID, Newtransaction, AdminAddress);

                                //Update API request Data  
                                UpdateTransactionRequest(_TransactionObj.TransactionRequestID, ThirdPartyAPIRequestOnj.RequestBody);

                                //Make Thirdparty API Call ntrivedi 14-03-2019 as per conversion with cinmay bhai for local coin dealy issue 
                                _TransactionObj.APIResponse = _IWebApiSendRequest.SendAPIRequestAsync(ThirdPartyAPIRequestOnj.RequestURL, ThirdPartyAPIRequestOnj.RequestBody, WebApiConfigurationResponseObj.ContentType, 600000, ThirdPartyAPIRequestOnj.keyValuePairsHeader, WebApiConfigurationResponseObj.MethodType);
                            }
                            else
                            {
                                await MarkTransactionOperatorFail("Fail", enErrorCode.Withdrwal_FromAddressNotFound, Newtransaction);
                                HelperForLog.WriteLogIntoFile("MarkTransactionOperatorFail AddressNotFound", ControllerName, "##TrnNo:" + Req.TrnNo);
                                _Resp.ErrorCode = enErrorCode.Withdrwal_FromAddressNotFound;
                                //return Task.FromResult(_Resp);
                                return _Resp;
                            }
                            break;
                        default:
                            Console.WriteLine("Default case");
                            break;
                    }
                    NewtransactionReq.SetResponse(_TransactionObj.APIResponse);
                    NewtransactionReq.SetResponseTime(Helpers.UTC_To_IST());
                    if (string.IsNullOrEmpty(_TransactionObj.APIResponse))
                    {
                        _Resp.ReturnMsg = EnResponseMessage.ProcessTrn_HoldMsg;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ErrorCode = enErrorCode.ProcessTrn_GettingResponseBlank;
                        MarkTransactionHold(_Resp.ReturnMsg, _Resp.ErrorCode, Newtransaction);
                        IsTxnProceed = 1;//no further call next API
                        break;
                    }
                    //_TransactionObj.APIResponse = "{\"success\":1,\"message\":\"\",\"return\":\"success\",\"data\":{\"transfer\":{\"id\":\"5c2f01f1ced9549f07e82fdd2782d137\",\"coin\":\"fun\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xd548efcbbd40f0d624748bdc567433758ebcf7bb7cc97277292455e25cd89a1f\",\"height\":999999999,\"date\":\"2019-01-04T06:49:21.595Z\",\"type\":\"send\",\"value\":0,\"valueString\":\"0\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":0,\"usdRate\":0,\"state\":\"signed\",\"instant\":false,\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2019-01-04T06:49:21.595Z\",\"action\":\"signed\"},{\"date\":\"2019-01-04T06:49:21.345Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-200000000,\"valueString\":\"-200000000\"},{\"address\":\"0x65b8e3c2b429bf912d9d2109539b48dada560845\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":200000000,\"valueString\":\"200000000\"}],\"signedTime\":\"2019-01-04T06:49:21.595Z\",\"createdTime\":\"2019-01-04T06:49:21.345Z\"},\"txid\":\"0xd548efcbbd40f0d624748bdc567433758ebcf7bb7cc97277292455e25cd89a1f\",\"tx\":\"f901ad82138285037e11d6008307a12094e6b8b5c49d83619e91a57d50d169eabee8f91f6b80b901440dcd7a6c00000000000000000000000065b8e3c2b429bf912d9d2109539b48dada560845000000000000000000000000000000000000000000000000000000000bebc200000000000000000000000000419d0d8bdd9af5e606ae2232ed285aff190e711b000000000000000000000000000000000000000000000000000000005c383c7000000000000000000000000000000000000000000000000000000000000000d000000000000000000000000000000000000000000000000000000000000000c000000000000000000000000000000000000000000000000000000000000000418100cc80d0e1db10f399c441b3ed32cdf6e46b93e46087535acea850894892923ba71f4b71aaca9a88e875ae443c7d547b12edc9255967172909ee3088dbfc5f1b000000000000000000000000000000000000000000000000000000000000001ba0409c57acf7f63ec39048c5c9c49802b66f5b2e2763a134b77768e9148dd32566a0680fb4497890ad4534d4ef000f7f2fa5b697466d5a330c8331a77924def742d2\",\"status\":\"signed\"}}";
                    WebAPIParseResponseClsObj = _WebApiParseResponseObj.TransactionParseResponse(_TransactionObj.APIResponse, Provider.ThirPartyAPIID);
                    NewtransactionReq.SetTrnID(WebAPIParseResponseClsObj.TrnRefNo);
                    NewtransactionReq.SetOprTrnID(WebAPIParseResponseClsObj.OperatorRefNo);
                    _TransactionRequest.Update(NewtransactionReq);


                    //WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;//komal test 
                    if ((WebAPIParseResponseClsObj.Status == enTransactionStatus.Success) || (WebAPIParseResponseClsObj.Status == enTransactionStatus.Hold))
                    {
                        if (Req.TrnType == enTrnType.Withdraw)
                            UpdateIntoWithdrawHistory(Req, 6, "Process entry", ThirdPartyAPIRequestOnj.walletID, WebAPIParseResponseClsObj.TrnRefNo);
                    }
                    else
                    {
                        if (Req.TrnType == enTrnType.Withdraw)
                            UpdateIntoWithdrawHistory(Req, 2, "Fail", ThirdPartyAPIRequestOnj.walletID);
                    }
                    HelperForLog.WriteLogIntoFile("UpdateIntoWithdrawHistory", ControllerName, "--2--Update Withdarw History---" + "##TrnNo:" + Req.TrnNo);

                    //Uday 30-01-2019  Update TxnId In WithdrawERCTokenQueue
                    if (Provider.AppTypeID == (long)enAppType.Erc20Withdraw)
                    {
                        UpdateWithdrawERCTokenQueue(WebAPIParseResponseClsObj.TrnRefNo, WithdrawERCTokenQueueId, Convert.ToInt16(WebAPIParseResponseClsObj.Status)); // //ntrivedi 06-03-219  update thirdparty api status in erc20queue not fix success
                    }

                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
                    {
                        MarkTransactionSuccess("Success" + WebAPIParseResponseClsObj.ResponseMsg, Newtransaction);
                        Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.WithdrwalOutsideTransactionSuccess);
                        _TransactionRepository.Update(Newtransaction);
                        IsTxnProceed = 1;//no further call next API
                        _Resp.ReturnMsg = "Transaction Confirm Success.";
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionConfirmSuccess;
                        break;
                    }
                    else if (WebAPIParseResponseClsObj.Status == enTransactionStatus.OperatorFail)
                    {
                        continue;
                    }
                    else
                    {
                        Newtransaction.SetTransactionStatusMsg(WebAPIParseResponseClsObj.ResponseMsg);
                        _TransactionRepository.Update(Newtransaction);
                        Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.WithdrwalTransactionHold);
                        _TransactionRepository.Update(Newtransaction);
                        _Resp.ReturnMsg = "Hold";
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ErrorCode = enErrorCode.ProcessTrn_Hold;
                        IsTxnProceed = 1;//no further call next API
                        break;
                    }
                }
                if (IsTxnProceed == 0)
                {
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.ProcessTrn_OprFail;
                    _Resp.ReturnMsg = "Fail";
                    HelperForLog.WriteLogIntoFile("MarkTransactionOperatorFail Refund", ControllerName, "##TrnNo:" + Req.TrnNo);
                    await MarkTransactionOperatorFail("Fail", _Resp.ErrorCode, Newtransaction);
                }
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
            //return Task.FromResult(_Resp);
            return _Resp;
        }

        public long InsertTransactionStatus(TransactionProviderResponse listObj, string Request)
        {
            try
            {
                TransactionStatus transactionStatus = new TransactionStatus();
                transactionStatus.CreatedBy = 1;
                transactionStatus.CreatedDate = Helpers.UTC_To_IST();
                transactionStatus.SerProID = listObj.ServiceProID;
                transactionStatus.ServiceID = listObj.ServiceID;
                transactionStatus.Status = 1;
                transactionStatus.StatusMsg = "Initialize";
                transactionStatus.TrnNo = Req.TrnNo;
                transactionStatus.UpdatedBy = 1;

                transactionStatus = _TransactionStatus.Add(transactionStatus);
                return transactionStatus.Id;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InserttransactionStatus:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return 0;
            }
        }

        public long InsertTransactionRequest(TransactionProviderResponse listObj, string Request)
        {
            try
            {
                NewtransactionReq = new TransactionRequest()
                {
                    TrnNo = Req.TrnNo,
                    ServiceID = listObj.ServiceID,
                    SerProID = listObj.ServiceProID,
                    SerProDetailID = listObj.SerProDetailID,
                    CreatedDate = Helpers.UTC_To_IST(),
                    RequestData = Request
                };
                NewtransactionReq = _TransactionRequest.Add(NewtransactionReq);
                return NewtransactionReq.Id;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertTransactionRequest:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return 0;
            }
        }

        public long InsertIntoTransactionRequest(InsertIntoTransactionRequyest listObj)
        {
            try
            {
                NewtransactionReq = new TransactionRequest()
                {
                    TrnNo = 0,
                    ServiceID = 0,
                    SerProID = listObj.TrnNo,
                    SerProDetailID = 0,
                    CreatedDate = Helpers.UTC_To_IST(),
                    RequestData = listObj.RequestBody
                };
                NewtransactionReq = _TransactionRequest.Add(NewtransactionReq);
                return NewtransactionReq.Id;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertTransactionRequest:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return 0;
            }
        }

        public void UpdateTransactionRequest(long ApiId, string RequestBody)
        {
            try
            {
                var transactionRequest = _TransactionRequest.GetById(ApiId);
                transactionRequest.RequestData = RequestBody;
                _TransactionRequest.Update(transactionRequest);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateTransactionRequest:##TrnNo " + Req.TrnNo, ControllerName, ex);
            }
        }

        public void UpdateIntoTransactionRequest(long ApiId, string RequestBody)
        {
            try
            {
                var transactionRequest = _TransactionRequest.GetById(ApiId);
                transactionRequest.ResponseData = RequestBody;
                _TransactionRequest.Update(transactionRequest);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateTransactionRequest:##TrnNo " + Req.TrnNo, ControllerName, ex);
            }
        }

        public long InsertIntoWithdrawERCTokenQueue(long TrnNo, WithdrawERCAdminAddress AdminAddress)
        {
            try
            {
                WithdrawERCTokenQueue WithdrawERCTokenQueueRequest = new WithdrawERCTokenQueue();

                WithdrawERCTokenQueueRequest = new WithdrawERCTokenQueue()
                {
                    AddressId = AdminAddress.AddressId,
                    AdminAddress = AdminAddress.Address,
                    TrnNo = TrnNo,
                    CreatedDate = Helpers.UTC_To_IST(),
                };

                WithdrawERCTokenQueueRequest = _WithdrawERCTokenQueueRepository.Add(WithdrawERCTokenQueueRequest);
                return WithdrawERCTokenQueueRequest.Id;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertIntoWithdrawERCTokenQueue:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return 0;
            }
        }

        public void UpdateWithdrawERCTokenQueue(string TxnId, long WithdrawERCTokenQueueId, short status)
        {
            try
            {
                var WithdrawERCTokenRequest = _WithdrawERCTokenQueueRepository.GetById(WithdrawERCTokenQueueId);
                WithdrawERCTokenRequest.TrnRefNo = TxnId;
                WithdrawERCTokenRequest.Status = status;//1;  //ntrivedi 06-03-219  update thirdparty api status in erc20queue not fix success
                _WithdrawERCTokenQueueRepository.Update(WithdrawERCTokenRequest);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateWithdrawERCTokenQueue:##TrnNo " + Req.TrnNo, ControllerName, ex);
            }
        }

        #endregion

        #region Send SMS And Email

        //public async Task EmailSendAsync(string UserID, int Status, string PairName, string BaseMarket
        //   , string TrnDate, decimal ReqAmount = 0, decimal Amount = 0, decimal Fees = 0)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(UserID) && !string.IsNullOrEmpty(PairName) && !string.IsNullOrEmpty(BaseMarket) &&
        //            ReqAmount != 0 && !string.IsNullOrEmpty(TrnDate) && Amount != 0 && Status == 1)
        //        {
        //            SendEmailRequest Request = new SendEmailRequest();
        //            ApplicationUser User = new ApplicationUser();
        //            User = await _userManager.FindByIdAsync(UserID);
        //            if (!string.IsNullOrEmpty(User.Email))
        //            {
        //                IQueryable Result = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.TransactionSuccess), 0);
        //                foreach (TemplateMasterData Provider in Result)
        //                {
        //                    Provider.Content = Provider.Content.Replace("###USERNAME###", User.Name);
        //                    Provider.Content = Provider.Content.Replace("###TYPE###", PairName);
        //                    Provider.Content = Provider.Content.Replace("###REQAMOUNT###", ReqAmount.ToString());
        //                    Provider.Content = Provider.Content.Replace("###STATUS###", "Success");
        //                    Provider.Content = Provider.Content.Replace("###USER###", User.Name);
        //                    Provider.Content = Provider.Content.Replace("###CURRENCY###", BaseMarket);
        //                    Provider.Content = Provider.Content.Replace("###DATETIME###", TrnDate);
        //                    Provider.Content = Provider.Content.Replace("###AMOUNT###", Amount.ToString());
        //                    Provider.Content = Provider.Content.Replace("###FEES###", Fees.ToString());
        //                    Provider.Content = Provider.Content.Replace("###FINAL###", (Amount + Fees).ToString());
        //                    Request.Body = Provider.Content;
        //                    Request.Subject = Provider.AdditionalInfo;
        //                }
        //                Request.Recepient = User.Email;
        //                Request.EmailType = 0;
        //                _pushNotificationsQueue.Enqueue(Request); //24-11-2018 komal make Email Enqueue
        //                //await _mediator.Send(Request);

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog("Settlement - EmailSendAsync Error ", ControllerName, ex);
        //    }
        //}

        public async Task SMSSendWithdrwalTransaction(string TrnNo, decimal Amount, string CoinName, string WalletId, string MobileNumber, int CancelType)
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
                    communicationParamater.Param2 = Amount + "";
                    communicationParamater.Param3 = CoinName + "";
                    communicationParamater.Param4 = WalletId + "";


                    Task.Run(() => HelperForLog.WriteLogIntoFile("SendSMSTransaction - SMSSendWithdrwalTransaction", ControllerName, " ##TrnNo : " + TrnNo + " ##MobileNo : " + MobileNumber + " ##Amount : " + Amount + " ##Coinname : " + CoinName + " ##WalletId : " + WalletId + " ##Type : " + CancelType));

                    if (CancelType == 1) // Transaction Create
                    {
                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_WithdrwalTransactionCreate, communicationParamater, enCommunicationServiceType.SMS).Result;
                    }
                    else if (CancelType == 2) // Transaction Success
                    {
                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_WithdrwalTransactionSuccess, communicationParamater, enCommunicationServiceType.SMS).Result;
                    }
                    else if (CancelType == 3) // Transaction Failed
                    {
                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_WithdrwalTransactionFailed, communicationParamater, enCommunicationServiceType.SMS).Result;
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
                Task.Run(() => HelperForLog.WriteErrorLog("SMSSendWithdrwalTransaction:##TrnNo " + TrnNo, ControllerName, ex));
            }
        }

        public async Task EmailSendWithdrwalTransaction(string UserID, long TrnNo, decimal Amount, decimal fees, string CoinName, string DestAddress, int CancelType, string guid = "", string DateTime = "", string ChargeCurrency = "")
        {
            try
            {
                SendEmailRequest Request = new SendEmailRequest();
                ApplicationUser User = new ApplicationUser();
                TemplateMasterData EmailData = new TemplateMasterData();
                CommunicationParamater communicationParamater = new CommunicationParamater();

                User = await _userManager.FindByIdAsync(UserID);
                if (!string.IsNullOrEmpty(User.Email))
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFile("SendEmailTransaction - EmailSendWithdrwalTransaction", ControllerName, " ##TrnNo : " + TrnNo + " ##Type : " + CancelType + "ChargeCurrency: " + ChargeCurrency));


                    communicationParamater.Param1 = User.UserName.ToLower() + "";
                    communicationParamater.Param2 = Helpers.DoRoundForTrading(Amount, 8).ToString();
                    communicationParamater.Param3 = Helpers.DoRoundForTrading(fees, 8).ToString() + " " + ChargeCurrency;
                    //  communicationParamater.Param4 = Helpers.DoRoundForTrading((Amount + fees),8).ToString();
                    communicationParamater.Param4 = DestAddress;
                    communicationParamater.Param5 = CoinName;
                    // communicationParamater.Param6 = DestAddress;

                    if (CancelType == 1) // Transaction Create
                    {
                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_WithdrwalTransactionCreate, communicationParamater, enCommunicationServiceType.Email).Result;
                    }
                    else if (CancelType == 2) // Transaction Success
                    {
                        communicationParamater.Param1 = User.UserName.ToLower() + "";
                        communicationParamater.Param2 = Helpers.DoRoundForTrading(Amount, 8).ToString();
                        communicationParamater.Param3 = "successed";
                        communicationParamater.Param4 = CoinName;
                        communicationParamater.Param5 = DateTime;
                        communicationParamater.Param6 = Helpers.DoRoundForTrading(fees, 8).ToString() + " " + ChargeCurrency;
                        //  communicationParamater.Param7 = Helpers.DoRoundForTrading(Amount, 8).ToString();
                        communicationParamater.Param7 = DestAddress;

                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_WithdrwalTransactionStatus, communicationParamater, enCommunicationServiceType.Email).Result;
                        //EmailData = _messageService.SendMessageAsync(EnTemplateType.EMAIL_WithdrwalTransactionSuccess, communicationParamater, enCommunicationServiceType.Email).Result;
                    }
                    else if (CancelType == 3) // Transaction Failed
                    {
                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_WithdrwalTransactionFailed, communicationParamater, enCommunicationServiceType.Email).Result;
                    }
                    if (CancelType == 4) // Withdrwal Transaction Confiramtion Mail
                    {
                        //Rushabh 13-12-2019 As per client requirement remove authorization from confirmation mail so link encryption code added.
                        //var url = _configuration["WithdrawConfirmUrl"].ToString();
                        //communicationParamater.Param6 = url + "?RefNo=" + guid + "&Bit=1"; //Bit = 1 Accept
                        //communicationParamater.Param7 = url + "?RefNo=" + guid + "&Bit=2";  //Bit = 2 Reject

                        ConfirmationMailLinkToken TranDetail = new ConfirmationMailLinkToken
                        {
                            UserId = Convert.ToInt64(UserID),
                            RefNo = guid
                        };
                        byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                        string UserDetails = JsonConvert.SerializeObject(TranDetail);
                        string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);
                        byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);

                        //Bit = 1 Accept
                        communicationParamater.Param6 = _configuration["WithdrawConfirmUrl"].ToString() + "?RefNo=" + Convert.ToBase64String(plainTextBytes) + "&Bit=1";
                        //Bit = 2 Reject
                        communicationParamater.Param7 = _configuration["WithdrawConfirmUrl"].ToString() + "?RefNo=" + Convert.ToBase64String(plainTextBytes) + "&Bit=2";
                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_WithdrwalTransactionConfirmation, communicationParamater, enCommunicationServiceType.Email).Result;
                    }
                    if (CancelType == 5) // Transaction Reject
                    {
                        communicationParamater.Param1 = User.UserName.ToLower() + "";
                        communicationParamater.Param2 = Helpers.DoRoundForTrading(Amount, 8).ToString();
                        communicationParamater.Param3 = "cancelled";
                        communicationParamater.Param4 = CoinName;
                        communicationParamater.Param5 = DateTime;
                        communicationParamater.Param6 = Helpers.DoRoundForTrading(fees, 8).ToString() + " " + ChargeCurrency;
                        // communicationParamater.Param7 = Helpers.DoRoundForTrading((Amount + fees), 8).ToString();
                        communicationParamater.Param7 = DestAddress;

                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_WithdrwalTransactionStatus, communicationParamater, enCommunicationServiceType.Email).Result;
                    }
                    if (CancelType == 6) // Withdrwal Transaction Admin Confiramtion Mail
                    {
                        communicationParamater.Param1 = User.UserName.ToLower() + "";
                        communicationParamater.Param2 = Helpers.DoRoundForTrading(Amount, 8).ToString();
                        communicationParamater.Param3 = "Sent For Admin Approval";
                        communicationParamater.Param4 = CoinName;
                        communicationParamater.Param5 = DateTime;
                        communicationParamater.Param6 = Helpers.DoRoundForTrading(fees, 8).ToString() + " " + ChargeCurrency;
                        // communicationParamater.Param7 = Helpers.DoRoundForTrading((Amount + fees), 8).ToString();
                        communicationParamater.Param7 = DestAddress;

                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_WithdrwalTransactionConfirmation, communicationParamater, enCommunicationServiceType.Email).Result;
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
                Task.Run(() => HelperForLog.WriteErrorLog("EmailSendWithdrwalTransaction:##TrnNo " + TrnNo, ControllerName, ex));
            }
        }

        #endregion

        #region Resend Email For Withdrwal Confirmation
        public async Task<BizResponse> ResendEmailWithdrawalConfirmation(long TrnNo, long UserId)
        {
            try
            {
                BizResponse _Resp = new BizResponse();

                //get transaction from transactionqueue              
                var tranTQobj = _TransactionRepository.GetById(TrnNo);
                Newtransaction = new TransactionQueue();
                Newtransaction = tranTQobj;

                if (tranTQobj == null)
                {
                    _Resp.ReturnMsg = "Transaction Not Found";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_TranNoNotFound;

                    return _Resp;
                }

                //Validate User
                if (Newtransaction.MemberID != UserId)
                {
                    _Resp.ReturnMsg = "You have not authorized to doing this transaction";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_NotAuthorizeToDoingTransaction;

                    return _Resp;
                }

                //Check Already Processed or not
                if (Newtransaction.IsVerified != 0)
                {
                    _Resp.ReturnMsg = "Transaction Already Verified";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionAlreadyVerified;

                    return _Resp;
                }

                //Validate Transaction
                if (Newtransaction.Status != 4)
                {
                    _Resp.ReturnMsg = "Transaction Not In Hold State";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_TranNoHoldState;

                    return _Resp;
                }

                //Check Resend Mail After 15 Minutes
                var TranTimeOut = _configuration["WithdrawTimeLimit"].ToString();
                if (Newtransaction.EmailSendDate.AddMinutes(Convert.ToInt32(TranTimeOut)) >= Helpers.UTC_To_IST())
                {
                    _Resp.ReturnMsg = "Send Email After " + TranTimeOut + " Minutes";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_SendEmailAfterSomeTime;

                    return _Resp;
                }

                //Update EmailSendDate
                Newtransaction.EmailSendDate = Helpers.UTC_To_IST();
                _TransactionRepository.Update(Newtransaction);

                //
                var fiatObj = _SellTopUpRequest.GetSingle(i => i.TrnNo == Newtransaction.Id && i.Status == 0);
                if (fiatObj != null)
                {
                    var url = _configuration["FiatWithdrawConfirmUrl"].ToString();

                    var Param6 = url + "?RefNo=" + fiatObj.Guid + "&Bit=1"; //Bit = 1 Accept
                    var Param8 = url + "?RefNo=" + fiatObj.Guid + "&Bit=2";  //Bit = 2 Reject

                    _WalletService.EmailSendAsyncV1(EnTemplateType.EMAIL_FiatSellRequest, fiatObj.UserId.ToString(), fiatObj.FromCurrency, fiatObj.ToCurrency, Helpers.DoRoundForTrading(fiatObj.ToAmount, 8).ToString(), Helpers.DoRoundForTrading(fiatObj.FromAmount, 8).ToString(), Param6, fiatObj.Guid.ToString(), Param8, fiatObj.CreatedDate.ToString());
                }
                else
                {
                    //Send email for next api process
                    await EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Newtransaction.Id, Newtransaction.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Newtransaction.SMSCode, Newtransaction.TransactionAccount, 4, Newtransaction.GUID + "", "", Newtransaction.ChargeCurrency);
                }
                ///


                _Resp.ReturnMsg = "Email Send successfully!";
                _Resp.ReturnCode = enResponseCodeService.Success;
                _Resp.ErrorCode = enErrorCode.Withdrwal_ResendConfirmationEmailSuccess;

                return _Resp;
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("EmailSendWithdrwalTransaction:##TrnNo " + TrnNo, ControllerName, ex));
                throw ex;
            }
        }
        #endregion

        public async Task<BizResponse> FIATCreateTransaction()
        {
            decimal Amount2 = 0;
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFile("FIATCreateTransaction", ControllerName, "--1-- Transaction Process For" + Req.TrnType + "##TrnNo:" + Req.TrnNo));

                _TrnService = _trnMasterConfiguration.GetServices().Where(item => item.SMSCode == Req.SMSCode && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();

                //2019-11-5 added user validation in withdraw
                var UserObj = await _userService.UserValidationForWithdraw(Req.MemberID);
                if (UserObj.ReturnCode != enResponseCode.Success)
                {
                    Req.StatusMsg = UserObj.ReturnMsg;
                    return MarkSystemFailTransaction(UserObj.ErrorCode);
                }

                if (_TrnService == null)
                {
                    Req.StatusMsg = EnResponseMessage.ProcessTrn_ServiceProductNotAvailableMsg;
                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_ServiceProductNotAvailable);
                }
                var GetWalletIDResult = _WalletService.GetTransactionWalletByCoin(Req.MemberID, _TrnService.SMSCode);
                var GetWalletIDResult1 = await GetWalletIDResult;
                if (GetWalletIDResult1.ReturnCode != enResponseCode.Success)
                {
                    Req.StatusMsg = EnResponseMessage.InValidDebitAccountIDMsg;
                    return MarkSystemFailTransaction(enErrorCode.InValidDebitAccountID);
                }
                Task.Run(() => HelperForLog.WriteLogIntoFile("FIATCreateTransaction", ControllerName, "Debit WalletID" + Req.DebitWalletID + "##TrnNo:" + Req.TrnNo));
                Req.DebitWalletID = GetWalletIDResult1.Wallet.id;
                Req.DebitAccountID = GetWalletIDResult1.Wallet.AccWalletID;
                //FiatCreateTrnDuplicateTrn
                var DupTQResult = _TransactionRepository.GetSingleAsync(item => item.MemberMobile == Req.MemberMobile && item.Amount == Req.Amount && item.TrnDate.AddMinutes(10) > Helpers.UTC_To_IST() && (item.Status == Convert.ToInt16(enTransactionStatus.Hold) || item.Status == Convert.ToInt16(enTransactionStatus.Success)));
                var DuplicateTxn = await DupTQResult;//DupTQResult

                if (DuplicateTxn != null)
                {
                    Req.StatusMsg = "Duplicate Transaction, Please Try After 10 Minutes";
                    return MarkSystemFailTransaction(enErrorCode.FiatCreateTrnDuplicateTrn);//FiatCreateTrnDuplicateTrn
                }

                if (Req.Amount <= 0)
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnInvalidAmountMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrnInvalidAmount);
                }
                InsertTransactionInQueue();
                return (new BizResponse { ReturnMsg = "", ReturnCode = enResponseCodeService.Success });
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("FIATCreateTransaction:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionInsertInternalError });
            }

        }

        public async Task<BizResponse> FiatWithdrawTransactionAPICallProcessAsync(WithdrawalConfirmationRequest Request, long UserId, short IsReqFromAdmin, ApplicationUser user = null)
        {
            try
            {
                BizResponse _Resp = new BizResponse();

                //get transaction from transactionqueue              
                var tranTQobj = _TransactionRepository.GetSingle(x => x.GUID.ToString() == Request.RefNo);
                Newtransaction = new TransactionQueue();
                Newtransaction = tranTQobj;

                if (tranTQobj == null)
                {
                    _Resp.ReturnMsg = "Transaction Not Found";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_TranNoNotFound;

                    return _Resp;
                }

                //Validate User
                if (Newtransaction.MemberID != UserId)
                {
                    _Resp.ReturnMsg = "You have not authorized to doing this transaction";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_NotAuthorizeToDoingTransaction;

                    //2019-6-29 updated to dont allow to doing transction to unauthorized user
                    Newtransaction.Status = 3;
                    Newtransaction.StatusMsg = _Resp.ReturnMsg;
                    Newtransaction.StatusCode = Convert.ToInt64(_Resp.ErrorCode);
                    _TransactionRepository.Update(Newtransaction);

                    return _Resp;
                }

                //Check Already Processed or not
                if (IsReqFromAdmin != 1 && Newtransaction.IsVerified != 0)
                {
                    _Resp.ReturnMsg = "Transaction Already Verified";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionAlreadyVerified;
                    return _Resp;
                }

                //Validate Transaction
                if (Newtransaction.Status != 4)
                {
                    _Resp.ReturnMsg = "Transaction Not In Hold State";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_TranNoHoldState;

                    return _Resp;
                }

                //Check Bit Value
                if (Request.TransactionBit != 1 && Request.TransactionBit != 2)
                {
                    _Resp.ReturnMsg = "Invalid Transaction Bit Value";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_InvalidTransactionBitValue;

                    //2019-6-29 updated to dont allow to doing transction if another bit is in request
                    Newtransaction.Status = 2;
                    Newtransaction.StatusMsg = _Resp.ReturnMsg;
                    Newtransaction.StatusCode = Convert.ToInt64(_Resp.ErrorCode);
                    _TransactionRepository.Update(Newtransaction);

                    return _Resp;
                }
                if (Request.TransactionBit == 2)
                {
                    //Update IsVerify Bit
                    Newtransaction.IsVerified = 9;
                    _TransactionRepository.Update(Newtransaction);
                }
                else if (Request.TransactionBit == 1)
                {
                    //Update IsVerify Bit
                    Newtransaction.IsVerified = 1;
                    Newtransaction.StatusMsg = "Transaction Verified By Email";
                    _TransactionRepository.Update(Newtransaction);
                }
                else
                {
                    //2019-6-29 updated to dont allow to doing transction if another bit is in request
                    Newtransaction.Status = 2;
                    Newtransaction.StatusMsg = "Invalid TransactionBit.";
                    Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.Withdrwal_InvalidTransactionBitValue);
                    _TransactionRepository.Update(Newtransaction);
                }
                if (Request.TransactionBit == 2) //Reject Transaction Process
                {
                    //Process For Credit Process
                    WithdrawalReconRequest withdrawalReconRequest = new WithdrawalReconRequest();
                    withdrawalReconRequest.ActionType = enWithdrawalReconActionType.Refund;
                    withdrawalReconRequest.ActionMessage = "Withdrwal Transaction Request Cancel By User";
                    withdrawalReconRequest.TrnNo = Newtransaction.Id;

                    var ReconResponse = _backOfficeTrnService.WithdrawalRecon(withdrawalReconRequest, UserId, "");

                    if (ReconResponse.ReturnCode == enResponseCode.Success)
                    {
                        _Resp.ReturnMsg = "Transaction Cancel Success.";
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionCancelSuccess;

                        //Update Message Bit
                        _TransactionRepository.ReloadEntity(Newtransaction);    //Uday 28-01-2019 reload tq object bcas recon chnages not reflect in this object
                        Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.WithdrwalCancelByUser);
                        Newtransaction.StatusMsg = "Cancelled By User";
                        _TransactionRepository.Update(Newtransaction);

                        //Send Email For Reject Process
                        await EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Newtransaction.Id, Newtransaction.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Newtransaction.SMSCode, Newtransaction.TransactionAccount, 5, "", Newtransaction.TrnDate.ToString(), Newtransaction.ChargeCurrency);
                    }
                    else
                    {
                        //Update Message Bit
                        Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.WithdrwalCancelProcessFail);
                        Newtransaction.StatusMsg = "Withdrwal Cancel Process Fail";
                        _TransactionRepository.Update(Newtransaction);

                        _Resp.ReturnMsg = "Transaction Cancel Fail.";
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionCancelFail;

                        HelperForLog.WriteErrorLog("WithdrawTransactionAPICallProcessAsync #WithdrawTransactionCancelProcessFail#: ##RefNo " + Request.RefNo + " ", ControllerName + " ErrorCode : " + ReconResponse.ErrorCode + "  ReturnMsg : " + ReconResponse.ReturnMsg, new Exception());
                    }
                }
                else if (Request.TransactionBit == 1) //Accept Transaction Process
                {
                    //Check Transaction TimeOut
                    // Resend Email, So Check with EmailSendDate Insted Of TrnDate
                    //If Request Through Email Then & Only Then Check This Condition Rushabh 29-05-2019
                    if (IsReqFromAdmin == 0)
                    {
                        var TranTimeOut = _configuration["WithdrawTimeLimit"].ToString();
                        if (Newtransaction.EmailSendDate.AddMinutes(Convert.ToInt32(TranTimeOut)) <= Helpers.UTC_To_IST())
                        {
                            _Resp.ReturnMsg = "The link is expired. Please resend confirmation mail from hisotry.";
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionTimeOut;

                            //Uday 28-01-2019 When Transaction timeout user also able to regenrate the mail
                            Newtransaction.StatusMsg = _Resp.ReturnMsg;
                            Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.Withdrwal_TransactionTimeOut);
                            Newtransaction.IsVerified = 0;

                            ////Uday 28-01-2019  Transaction timeout so update the tq statuscode and statusmsg
                            //Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.Withdrwal_TransactionTimeOut);
                            //Newtransaction.StatusMsg = "Transaction TimeOut";
                            _TransactionRepository.Update(Newtransaction);

                            return _Resp;
                        }
                    }
                    var GetWalletIDResult1 = _WalletService.GetWalletID(Newtransaction.DebitAccountID);
                    Req = new NewWithdrawRequestCls();
                    Req.DebitWalletID = await GetWalletIDResult1;
                    Req.TrnType = enTrnType.Withdraw;
                    Req.SMSCode = Newtransaction.SMSCode;
                    Req.Amount = Newtransaction.Amount;
                    Req.TrnNo = Newtransaction.Id;
                    Req.TrnRefNo = Newtransaction.TrnRefNo;
                    Req.MemberID = Newtransaction.MemberID;
                    Req.DebitAccountID = Newtransaction.DebitAccountID;
                    Req.TransactionAccount = Newtransaction.TransactionAccount;

                    if ((EnWithdrwalInternalTransaction)Newtransaction.IsInternalTrn == EnWithdrwalInternalTransaction.OutSideTransaction)
                    {
                        //2019-11-13
                        var TransactionRequestData = _TransactionRequest.GetSingle(x => x.TrnNo == Newtransaction.Id && !string.IsNullOrEmpty(x.TrnID));
                        if (TransactionRequestData != null)
                        {
                            _Resp.ReturnMsg = "Transaction Already Requested.";
                            _Resp.ReturnCode = enResponseCodeService.Fail;
                            _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionAlreadyRequested;

                            return _Resp;
                        }

                        var TxnProviderListResult = _WebApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { amount = tranTQobj.Amount, SMSCode = tranTQobj.SMSCode, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(tranTQobj.TrnType) });
                        TxnProviderList = await TxnProviderListResult;
                        //Added By Rushabh 29-05-2019 For Admin Approval Process
                        if (TxnProviderList.Count > 0)
                        {
                            #region not used
                            //if (TxnProviderList[0].IsAdminApprovalRequired == 1)
                            //{
                            //    if (Newtransaction.IsVerifiedByAdmin == 0) //29-05-2019 rushabh 
                            //    {
                            //        WithdrawAdminRequest NewObj = new WithdrawAdminRequest
                            //        {
                            //            TrnNo = Newtransaction.Id,
                            //            CreatedBy = Newtransaction.MemberID,
                            //            CreatedDate = Helpers.UTC_To_IST(),
                            //            Status = 0
                            //        };
                            //        _WithdrawAdminReqCommonRepo.Add(NewObj);

                            //        Newtransaction.IsVerifiedByAdmin = 0;
                            //        Newtransaction.StatusMsg = "Waiting For Admin Approval";
                            //        _TransactionRepository.Update(Newtransaction);

                            //        _Resp.ErrorCode = enErrorCode.WithdrawalReqSentToAdmin;
                            //        _Resp.ReturnCode = enResponseCodeService.Success;
                            //        _Resp.ReturnMsg = "Withdrawal Request Has Been Sent For Admin Approval";

                            //        //need to add signal-r call
                            //        ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                            //        ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.WithdrawalReqSentToAdmin);
                            //        ActivityNotification.Param1 = Newtransaction.Id.ToString();
                            //        ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);
                            //        Parallel.Invoke(() => _ISignalRService.SendActivityNotificationV2(ActivityNotification, UserId.ToString(), 0));

                            //        //Send Email For Admin Approval Notification
                            //        await EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Newtransaction.Id, Newtransaction.Amount, Convert.ToDecimal(Newtransaction.ChargeRs), Newtransaction.SMSCode, Newtransaction.TransactionAccount, 6, "", Newtransaction.TrnDate.ToString(), Newtransaction.ChargeCurrency);
                            //        return _Resp;
                            //    }
                            //    else
                            //    {
                            //        _Resp = await CallWebAPI(_Resp);
                            //    }
                            //}
                            #endregion
                            _Resp = await FiatCallWebAPI(_Resp); //end 29-05-2019
                        }
                    }
                }
                else
                {
                    _Resp.ReturnMsg = "Invalid Transaction Bit Value";
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.Withdrwal_InvalidTransactionBitValue;

                    //2019-6-29 updated to dont allow to doing transction if another bit is in request
                    Newtransaction.Status = 2;
                    Newtransaction.StatusMsg = "Invalid TransactionBit.";
                    Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.Withdrwal_InvalidTransactionBitValue);
                    _TransactionRepository.Update(Newtransaction);
                }
                return _Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WithdrawTransactionAPICallProcessAsync:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionProcessInternalError });
            }
        }

        public async Task<BizResponse> FiatCallWebAPI(BizResponse _Resp)
        {
            //TransactionRequest TransactionRequestObj=new TransactionRequest(); 
            ThirdPartyAPIRequest ThirdPartyAPIRequestOnj;
            WebApiConfigurationResponse WebApiConfigurationResponseObj;
            WebAPIParseResponseCls WebAPIParseResponseClsObj = new WebAPIParseResponseCls();
            _TransactionObj = new ProcessTransactionCls();
            long WithdrawERCTokenQueueId = 0;

            //long TxnRequestID = 0;
            short IsTxnProceed = 0;
            try
            {
                foreach (TransactionProviderResponse Provider in TxnProviderList)//Make txn on every API
                {

                    Newtransaction.SetServiceProviderData(Provider.ServiceID, Provider.ServiceProID, Provider.ProductID, Provider.RouteID, Provider.SerProDetailID, 0);
                    _TransactionRepository.Update(Newtransaction);

                    WebApiConfigurationResponseObj = _IWebApiData.GetAPIConfiguration(Provider.ThirPartyAPIID);
                    if (WebApiConfigurationResponseObj == null)
                    {
                        _Resp.ReturnMsg = EnResponseMessage.ProcessTrn_ThirdPartyDataNotFoundMsg;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ErrorCode = enErrorCode.ProcessTrn_ThirdPartyDataNotFound;
                        //MarkTransactionOperatorFail(_Resp.ReturnMsg, _Resp.ErrorCode, Newtransaction);
                        continue;
                    }

                    if (Req.TrnType == enTrnType.Withdraw)
                        InsertIntoWithdrawHistory(Provider.ServiceProID, Provider.RouteName, WebAPIParseResponseClsObj.TrnRefNo, Provider.ProviderWalletID, Req);//rita 4-1-19 added ProviderWalletID ,also add in inner join configuration

                    HelperForLog.WriteLogIntoFile("InsertIntoWithdrawHistory", ControllerName, "--2--Insert Withdarw History---" + "##TrnNo:" + Req.TrnNo);
                    ThirdPartyAPIRequestOnj = _IGetWebRequest.MakeWebRequest(Provider.RouteID, Provider.ThirPartyAPIID, Provider.SerProDetailID, Newtransaction);
                    //Insert API request Data
                    _TransactionObj.TransactionRequestID = InsertTransactionRequest(Provider, ThirdPartyAPIRequestOnj.RequestURL + "::" + ThirdPartyAPIRequestOnj.RequestBody);
                    

                    switch (Provider.AppTypeID)
                    {
                        case (long)enAppType.WebSocket:

                        case (long)enAppType.JsonRPC:
                            _TransactionObj.APIResponse = _IWebApiSendRequest.SendJsonRpcAPIRequestAsync(ThirdPartyAPIRequestOnj.RequestURL, ThirdPartyAPIRequestOnj.RequestBody, ThirdPartyAPIRequestOnj.keyValuePairsHeader);
                            break;
                        case (long)enAppType.TCPSocket:

                        case (long)enAppType.RestAPI:
                            _TransactionObj.APIResponse = _IWebApiSendRequest.SendAPIRequestAsync(ThirdPartyAPIRequestOnj.RequestURL, ThirdPartyAPIRequestOnj.RequestBody, WebApiConfigurationResponseObj.ContentType, 30000, ThirdPartyAPIRequestOnj.keyValuePairsHeader, WebApiConfigurationResponseObj.MethodType);
                            break;
                        case (long)enAppType.HttpApi:
                            _TransactionObj.APIResponse = _IWebApiSendRequest.SendAPIRequestAsync(ThirdPartyAPIRequestOnj.RequestURL, ThirdPartyAPIRequestOnj.RequestBody, WebApiConfigurationResponseObj.ContentType, 90000, ThirdPartyAPIRequestOnj.keyValuePairsHeader, WebApiConfigurationResponseObj.MethodType);//ntrivedi 07-08-2019 timeout 30000 to 90000 as 114 server slow
                            break;
                        case (long)enAppType.SocketApi:

                        case (long)enAppType.BitcoinDeamon:

                        case (long)enAppType.Erc20Withdraw: //For Erc20 Provider Withdraw, So Change its makerequest

                            //Get AdminAddress 
                            var AdminAddress = _withdrawProcessService.GetWithdrawERCAdminAddress(Provider.ServiceID, Provider.ThirPartyAPIID);

                            if (AdminAddress != null)
                            {
                                //Insert Into WQueue
                                WithdrawERCTokenQueueId = InsertIntoWithdrawERCTokenQueue(NewtransactionReq.TrnNo, AdminAddress);

                                ThirdPartyAPIRequestOnj = _IGetWebRequest.MakeWebRequest(Provider.RouteID, Provider.ThirPartyAPIID, Provider.SerProDetailID, Newtransaction, AdminAddress);

                                //Update API request Data  
                                UpdateTransactionRequest(_TransactionObj.TransactionRequestID, ThirdPartyAPIRequestOnj.RequestBody);

                                //Make Thirdparty API Call ntrivedi 14-03-2019 as per conversion with cinmay bhai for local coin dealy issue 
                                _TransactionObj.APIResponse = _IWebApiSendRequest.SendAPIRequestAsync(ThirdPartyAPIRequestOnj.RequestURL, ThirdPartyAPIRequestOnj.RequestBody, WebApiConfigurationResponseObj.ContentType, 600000, ThirdPartyAPIRequestOnj.keyValuePairsHeader, WebApiConfigurationResponseObj.MethodType);
                            }
                            else
                            {
                                // await MarkTransactionOperatorFail("Fail", enErrorCode.Withdrwal_FromAddressNotFound, Newtransaction);//remove operator fail 2019-11-13
                                HelperForLog.WriteLogIntoFile("MarkTransactionOperatorFail AddressNotFound", ControllerName, "##TrnNo:" + Req.TrnNo);
                                _Resp.ErrorCode = enErrorCode.Withdrwal_FromAddressNotFound;
                                //return Task.FromResult(_Resp);
                                return _Resp;
                            }
                            break;
                        default:
                            Console.WriteLine("Default case");
                            break;
                    }
                    NewtransactionReq.SetResponse(_TransactionObj.APIResponse);
                    NewtransactionReq.SetResponseTime(Helpers.UTC_To_IST());
                    if (string.IsNullOrEmpty(_TransactionObj.APIResponse))
                    {
                        _Resp.ReturnMsg = EnResponseMessage.ProcessTrn_HoldMsg;
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ErrorCode = enErrorCode.ProcessTrn_GettingResponseBlank;
                        MarkTransactionHold(_Resp.ReturnMsg, _Resp.ErrorCode, Newtransaction);
                        IsTxnProceed = 1;//no further call next API
                        break;
                    }
                    //_TransactionObj.APIResponse = "{\"success\":1,\"message\":\"\",\"return\":\"success\",\"data\":{\"transfer\":{\"id\":\"5c2f01f1ced9549f07e82fdd2782d137\",\"coin\":\"fun\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xd548efcbbd40f0d624748bdc567433758ebcf7bb7cc97277292455e25cd89a1f\",\"height\":999999999,\"date\":\"2019-01-04T06:49:21.595Z\",\"type\":\"send\",\"value\":0,\"valueString\":\"0\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":0,\"usdRate\":0,\"state\":\"signed\",\"instant\":false,\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2019-01-04T06:49:21.595Z\",\"action\":\"signed\"},{\"date\":\"2019-01-04T06:49:21.345Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-200000000,\"valueString\":\"-200000000\"},{\"address\":\"0x65b8e3c2b429bf912d9d2109539b48dada560845\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":200000000,\"valueString\":\"200000000\"}],\"signedTime\":\"2019-01-04T06:49:21.595Z\",\"createdTime\":\"2019-01-04T06:49:21.345Z\"},\"txid\":\"0xd548efcbbd40f0d624748bdc567433758ebcf7bb7cc97277292455e25cd89a1f\",\"tx\":\"f901ad82138285037e11d6008307a12094e6b8b5c49d83619e91a57d50d169eabee8f91f6b80b901440dcd7a6c00000000000000000000000065b8e3c2b429bf912d9d2109539b48dada560845000000000000000000000000000000000000000000000000000000000bebc200000000000000000000000000419d0d8bdd9af5e606ae2232ed285aff190e711b000000000000000000000000000000000000000000000000000000005c383c7000000000000000000000000000000000000000000000000000000000000000d000000000000000000000000000000000000000000000000000000000000000c000000000000000000000000000000000000000000000000000000000000000418100cc80d0e1db10f399c441b3ed32cdf6e46b93e46087535acea850894892923ba71f4b71aaca9a88e875ae443c7d547b12edc9255967172909ee3088dbfc5f1b000000000000000000000000000000000000000000000000000000000000001ba0409c57acf7f63ec39048c5c9c49802b66f5b2e2763a134b77768e9148dd32566a0680fb4497890ad4534d4ef000f7f2fa5b697466d5a330c8331a77924def742d2\",\"status\":\"signed\"}}";
                    WebAPIParseResponseClsObj = _WebApiParseResponseObj.TransactionParseResponse(_TransactionObj.APIResponse, Provider.ThirPartyAPIID);
                    NewtransactionReq.SetTrnID(WebAPIParseResponseClsObj.TrnRefNo);
                    NewtransactionReq.SetOprTrnID(WebAPIParseResponseClsObj.OperatorRefNo);
                    _TransactionRequest.Update(NewtransactionReq);


                    //WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;//komal test 
                    if ((WebAPIParseResponseClsObj.Status == enTransactionStatus.Success) || (WebAPIParseResponseClsObj.Status == enTransactionStatus.Hold))
                    {
                        if (Req.TrnType == enTrnType.Withdraw)
                            UpdateIntoWithdrawHistory(Req, 6, "Process entry", ThirdPartyAPIRequestOnj.walletID, WebAPIParseResponseClsObj.TrnRefNo);
                    }
                    else
                    {
                        // if (Req.TrnType == enTrnType.Withdraw)
                        // UpdateIntoWithdrawHistory(Req, 2, "Fail", ThirdPartyAPIRequestOnj.walletID);
                        //remove operator fail 2019-11-13
                    }
                    HelperForLog.WriteLogIntoFile("UpdateIntoWithdrawHistory", ControllerName, "--2--Update Withdarw History---" + "##TrnNo:" + Req.TrnNo);

                    //Uday 30-01-2019  Update TxnId In WithdrawERCTokenQueue
                    if (Provider.AppTypeID == (long)enAppType.Erc20Withdraw)
                    {
                        UpdateWithdrawERCTokenQueue(WebAPIParseResponseClsObj.TrnRefNo, WithdrawERCTokenQueueId, Convert.ToInt16(WebAPIParseResponseClsObj.Status)); // //ntrivedi 06-03-219  update thirdparty api status in erc20queue not fix success
                    }

                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
                    {
                        MarkTransactionSuccess("Success" + WebAPIParseResponseClsObj.ResponseMsg, Newtransaction);
                        Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.WithdrwalOutsideTransactionSuccess);
                        _TransactionRepository.Update(Newtransaction);
                        IsTxnProceed = 1;//no further call next API
                        _Resp.ReturnMsg = "Transaction Confirm Success.";
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ErrorCode = enErrorCode.Withdrwal_TransactionConfirmSuccess;
                        break;
                    }
                    else if (WebAPIParseResponseClsObj.Status == enTransactionStatus.OperatorFail)
                    {
                        continue;
                    }
                    else
                    {
                        Newtransaction.SetTransactionStatusMsg(WebAPIParseResponseClsObj.ResponseMsg);
                        _TransactionRepository.Update(Newtransaction);
                        Newtransaction.StatusCode = Convert.ToInt64(enErrorCode.WithdrwalTransactionHold);
                        _TransactionRepository.Update(Newtransaction);
                        _Resp.ReturnMsg = "Hold";
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ErrorCode = enErrorCode.ProcessTrn_Hold;
                        IsTxnProceed = 1;//no further call next API
                        break;
                    }
                }
                if (IsTxnProceed == 0)
                {
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = enErrorCode.ProcessTrn_OprFail;
                    _Resp.ReturnMsg = "Fail";
                    HelperForLog.WriteLogIntoFile("MarkTransactionOperatorFail Refund", ControllerName, "##TrnNo:" + Req.TrnNo);
                    // await MarkTransactionOperatorFail("Fail", _Resp.ErrorCode, Newtransaction);////remove operator fail 2019-11-13
                }
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
            //return Task.FromResult(_Resp);
            return _Resp;
        }

    }
}
