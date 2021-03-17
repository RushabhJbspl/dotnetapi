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
using Worldex.Core.ViewModels;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Data;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Transaction
{
    public class WithdrawTransaction : IWithdrawTransaction
    {
        private readonly EFCommonRepository<TransactionQueue> _TransactionRepository;
        private readonly EFCommonRepository<WithdrawHistory> _WithdrawHistoryRepository;
        private readonly ICommonRepository<ServiceMaster> _ServiceConfi;
        private readonly ICommonRepository<AddressMaster> _AddressMasterRepository;      
        private readonly IWalletService _WalletService;
        private readonly IWebApiRepository _WebApiRepository;
        private readonly ICommonRepository<TransactionRequest> _TransactionRequest;
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

        public BizResponse _Resp;
        public List<TransactionProviderResponse> TxnProviderList;

        TransactionQueue Newtransaction;
        NewWithdrawRequestCls Req;
        ProcessTransactionCls _TransactionObj;
        ServiceMaster _TrnService;
        TransactionRequest NewtransactionReq;
        private readonly IWalletTransactionCrDr _walletTransactionCrDr;

        private string ControllerName = "WithdrawTransaction";

        public WithdrawTransaction(EFCommonRepository<TransactionQueue> TransactionRepository,
            ICommonRepository<ServiceMaster> ServiceConfi, ICommonRepository<AddressMaster> AddressMasterRepository,
             IWalletService WalletService, IWebApiRepository WebApiRepository,
            ICommonRepository<TransactionRequest> TransactionRequest, IGetWebRequest IGetWebRequest,
            IWebApiSendRequest WebApiSendRequest, WebApiParseResponse WebApiParseResponseObj, IWebApiData IWebApiData,
            ISignalRService ISignalRService,UserManager<ApplicationUser> userManager,
            IMessageConfiguration messageConfiguration,
            IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, IMessageService messageService, 
            IPushNotificationsQueue<SendSMSRequest> pushSMSQueue, ITrnMasterConfiguration trnMasterConfiguration, EFCommonRepository<WithdrawHistory> WithdrawHistoryRepository,
             IWalletTransactionCrDr walletTransactionCrDr)
        {
            _TransactionRepository = TransactionRepository;          
            _ServiceConfi = ServiceConfi;
            _AddressMasterRepository = AddressMasterRepository;          
            _WalletService = WalletService;         
            _WebApiRepository = WebApiRepository;
            _TransactionRequest = TransactionRequest;
            _IGetWebRequest = IGetWebRequest;
            _IWebApiSendRequest = WebApiSendRequest;
            _WebApiParseResponseObj = WebApiParseResponseObj;
            _IWebApiData = IWebApiData;               
            _ISignalRService = ISignalRService;
            _userManager = userManager;
            //_mediator = mediator;
            _messageConfiguration = messageConfiguration;
            _pushNotificationsQueue = pushNotificationsQueue;
            _messageService = messageService;
            _pushSMSQueue = pushSMSQueue;
            _trnMasterConfiguration = trnMasterConfiguration;
            _WithdrawHistoryRepository = WithdrawHistoryRepository;
            _walletTransactionCrDr = walletTransactionCrDr;
        }       
        public async Task<BizResponse> WithdrawTransactionTransactionAsync(NewWithdrawRequestCls Req1)
        {
             Req = Req1;        

            _Resp =await CreateTransaction();
            if (_Resp.ReturnCode != enResponseCodeService.Success)
            {
                HelperForLog.WriteLogIntoFile("ProcessNewTransactionAsync", ControllerName, _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo);
                return _Resp;
            }
            return await CombineAllInitTransactionAsync();
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
                    Task.Run(()=>MarkTransactionSystemFail(_Resp.ReturnMsg, _Resp.ErrorCode,Newtransaction));
                    //return Task.FromResult(_Resp);
                    return _Resp;
                }
                if (Req.TrnType == enTrnType.Transaction)
                {
                    Req.ServiceType = (enServiceType)_TrnService.ServiceType;
                    //Req.WalletTrnType = enWalletTrnType.;
                }
                else if (Req.TrnType == enTrnType.Withdraw)
                {
                    Req.ServiceType = (enServiceType)_TrnService.ServiceType;
                    Req.WalletTrnType = enWalletTrnType.Withdrawal;
                }
                else//trading
                {
                    //Req.ServiceType = enServiceType.Trading;
                    //Req.WalletTrnType = enWalletTrnType.Dr_Trade;
                }
                //ntrivedi 15-11-2018
                var DebitResult = await _WalletService.GetWalletDeductionNew(Req.SMSCode, Helpers.GetTimeStamp(), enWalletTranxOrderType.Debit, Req.Amount, Req.MemberID,
                    Req.DebitAccountID, Req.TrnNo, Req.ServiceType, Req.WalletTrnType, Req.TrnType, Req.accessToken);
                //DebitResult.ReturnCode = enResponseCode.Success;//komal test only
                if (DebitResult.ReturnCode != enResponseCode.Success)
                {
                    _Resp.ReturnMsg = DebitResult.ReturnMsg;//EnResponseMessage.ProcessTrn_WalletDebitFailMsg;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = DebitResult.ErrorCode;//enErrorCode.ProcessTrn_WalletDebitFail;
                    Task.Run(()=>MarkTransactionSystemFail(_Resp.ReturnMsg, _Resp.ErrorCode, Newtransaction));
                    HelperForLog.WriteLogIntoFile("CombineAllInitTransactionAsync", ControllerName, "Balance Deduction Fail" + _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo);
                    return _Resp;
                }
                //===================================Make txn HOLD as balance debited=======================
                MarkTransactionHold(EnResponseMessage.ProcessTrn_HoldMsg, enErrorCode.ProcessTrn_Hold, Newtransaction);
                HelperForLog.WriteLogIntoFile("CombineAllInitTransactionAsync", ControllerName, "Transaction/Withdraw Process Start" + "##TrnNo:" + Req.TrnNo);
                _Resp = await CallWebAPI(_Resp);
                return _Resp;
                //=========================UPDATE
                //return null;
            }
            catch (Exception ex)
            {
                //_log.LogError(ex, "exception,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog("CombineAllInitTransactionAsync:##TrnNo " + Req.TrnNo, ControllerName, ex);
                //return Task.FromResult((new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionProcessInternalError }));
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionProcessInternalError });
            }            
        }

        #region RegionInitTransaction    
        public async Task<BizResponse> CreateTransaction()
        {
            //decimal Amount2 = 0;//komal 03 May 2019, Cleanup
            try
            {
                Task.Run(()=>HelperForLog.WriteLogIntoFile("CreateTransaction", ControllerName, "--1-- Transaction Process For" + Req.TrnType + "##TrnNo:" + Req.TrnNo));

                var GetWalletIDResult1 = _WalletService.GetWalletID(Req.DebitAccountID);
                var DupTQResult =_TransactionRepository.GetSingleAsync(item => item.MemberMobile == Req.MemberMobile && item.TransactionAccount == Req.TransactionAccount && item.Amount == Req.Amount && item.TrnDate.AddMinutes(10) > Helpers.UTC_To_IST());

                _TrnService = _trnMasterConfiguration.GetServices().Where(item => item.SMSCode == Req.SMSCode && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                //_TrnService = _ServiceConfi.GetSingle(item => item.SMSCode == Req.SMSCode && item.Status == Convert.ToInt16(ServiceStatus.Active));

                if (_TrnService == null)
                {
                    Req.StatusMsg = EnResponseMessage.ProcessTrn_ServiceProductNotAvailableMsg;
                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_ServiceProductNotAvailable);
                }
                Req.DebitWalletID = await GetWalletIDResult1;
                var AddResult = _AddressMasterRepository.GetSingleAsync(item => item.WalletId == Req.DebitWalletID && item.Address == Req.TransactionAccount && item.Status == Convert.ToInt16(ServiceStatus.Active));//in withdraw , TransactionAccount has address
                if (Req.DebitWalletID==0)
                {
                    Req.StatusMsg = EnResponseMessage.InValidDebitAccountIDMsg;
                    return MarkSystemFailTransaction(enErrorCode.InValidDebitAccountID);
                }
                Task.Run(()=> HelperForLog.WriteLogIntoFile("CreateTransaction", ControllerName, "Debit WalletID" + Req.DebitWalletID + "##TrnNo:" + Req.TrnNo));
                
                var DuplicateTxn = await DupTQResult;
                //var DupTxnResult = _TransactionRepository.GetSingleAsync(item => item.MemberID == Req.MemberID && item.TrnRefNo == Req.TrnRefNo && item.TrnType == Convert.ToInt16(enTrnType.Transaction));
                var WalletWhiteListResult1 = _WalletService.CheckWithdrawalBene(Req.DebitWalletID, Req.AddressLabel, Req.TransactionAccount, Req.WhitelistingBit);

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
                var _ServiceLimitCheckObjResult = _WalletService.GetServiceLimitChargeValue(enWalletTrnType.Withdrawal, Req.SMSCode);

                var _AddressMasterObj = await AddResult;
                if (_AddressMasterObj != null)
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrn_NoSelfAddressWithdrawAllowMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrn_NoSelfAddressWithdrawAllow);
                }

                //Check Whilte listed Address 
                var WalletWhiteListResult = await WalletWhiteListResult1;
                if (!WalletWhiteListResult.Equals(enErrorCode.Success))
                {
                    Req.StatusMsg = WalletWhiteListResult.ToString();
                    return MarkSystemFailTransaction(WalletWhiteListResult);
                }
                var TxnProviderListResult = _WebApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { amount = Req.Amount, SMSCode = Req.SMSCode, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(Req.TrnType) });
                //Uday 02-11-2018 Check Wallet Amount Limit
                var _ServiceLimitCheckObj = await _ServiceLimitCheckObjResult;
                if ((Req.Amount > _ServiceLimitCheckObj.MaxAmount && _ServiceLimitCheckObj.MaxAmount != 0) || (Req.Amount < _ServiceLimitCheckObj.MinAmount && _ServiceLimitCheckObj.MinAmount != 0)) //ntrivedi if limit value is 0 measn not checking value
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrn_WithdrawAmountBetweenMinAndMax.Replace("@MIN", _ServiceLimitCheckObj.MinAmount.ToString()).Replace("@MAX", _ServiceLimitCheckObj.MaxAmount.ToString());
                    return MarkSystemFailTransaction(enErrorCode.TransactionLimitValidationFail); //ntrivedi 10-12-2018
                }

                Req.Status = enTransactionStatus.Initialize;
                Req.StatusCode = Convert.ToInt64(enErrorCode.TransactionInsertSuccess);
                InsertTransactionInQueue();
                TxnProviderList = await TxnProviderListResult;
                return (new BizResponse { ReturnMsg = "", ReturnCode = enResponseCodeService.Success });
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateTransaction:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionInsertInternalError });
            }

        }
        public BizResponse MarkSystemFailTransaction(enErrorCode ErrorCode)
        {
            try
            {
                Req.Status = enTransactionStatus.SystemFail;
                Req.StatusCode = Convert.ToInt64(ErrorCode);
                InsertTransactionInQueue();

                //DI of SMS here
                //Uday 07-12-2018 send sms when transaction is failed
                SMSSendWithdrwalTransaction(Req.TrnNo, Req.Amount, Req.SMSCode, Req.TransactionAccount, Newtransaction.MemberMobile, 3);

                //Uday 07-12-2018 send email when transaction is failed
                //changhe acWalletId to TransactionAccount 
                EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, 0, Req.SMSCode, Req.TransactionAccount, 3);
                try
                {
                    //komal 16-11-2018 add Activity Notifiation v2
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);
                    //notification.MsgCode = Convert.ToInt32(ErrorCode);
                    notification.Param1 = Req.TrnNo.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Fail);

                    //Task.Run(() => Parallel.Invoke(()=> SMSSendWithdrwalTransaction(Req.TrnNo, Req.Amount, Req.SMSCode, Req.TransactionAccount, Newtransaction.MemberMobile, 3),
                    //    ()=> _ISignalRService.SendActivityNotificationV2(notification, Req.accessToken),
                    //    ()=> EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, 0, Req.SMSCode, Req.TransactionAccount, 3)));
                    //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);
                    _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(),2);
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
                    DebitAccountID = Req.DebitAccountID
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
            _TransactionObj =new ProcessTransactionCls();
            //Member Service Disable check here for regular txn
            try
            {                
                //TxnProviderList = _WebApiRepository.GetProviderDataList(new TransactionApiConfigurationRequest { amount = Req.Amount, SMSCode = Req.SMSCode, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(Req.TrnType)});
                if (TxnProviderList.Count == 0) //Uday 05-11-2018 check condition for no record
                {
                    _Resp.ReturnMsg = EnResponseMessage.ProcessTrn_ServiceProductNotAvailableMsg;
                    _Resp.ErrorCode = enErrorCode.ProcessTrn_ServiceProductNotAvailable;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    return Task.FromResult(false);
                }
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
        public async void MarkTransactionSystemFail(string StatusMsg,enErrorCode ErrorCode,TransactionQueue Newtransaction)
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
                    notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);//for error in display
                   //notification.MsgCode = Convert.ToInt32(ErrorCode);
                    notification.Param1 = Req.TrnNo.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Fail);
                    //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);
                    _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2);
                    //_ISignalRService.SendActivityNotification("Transaction Validation Fail TrnNo:" + Req.TrnNo, Req.accessToken);
                    //Uday 07-12-2018 send sms when transaction is failed
                    SMSSendWithdrwalTransaction(Req.TrnNo, Req.Amount, Req.SMSCode, Req.DebitAccountID, Newtransaction.MemberMobile, 3);
                    //changhe acWalletId to TransactionAccount 
                    EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, 0, Req.SMSCode, Req.TransactionAccount, 3);

                    //Task.Run(() => Parallel.Invoke(() => _ISignalRService.SendActivityNotification("Transaction Validation Fail TrnNo:" + Req.TrnNo, Req.accessToken),
                    //    () => SMSSendWithdrwalTransaction(Req.TrnNo, Req.Amount, Req.SMSCode, Req.DebitAccountID, Newtransaction.MemberMobile, 3),
                    //    () => EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, 0, Req.SMSCode, Req.TransactionAccount, 3)));
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteLogIntoFile("ISignalRService Notification Error-MarkTransactionSystemFail", ControllerName, ex.Message + "##TrnNo:" + Newtransaction.Id);
                }
            }
            catch(Exception ex)
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
                    SMSSendWithdrwalTransaction(Newtransaction.Id, Req.Amount, Req.SMSCode, Req.TransactionAccount, Newtransaction.MemberMobile,1);

                    //Uday 07-12-2018 send email when transaction is successfully created
                    EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, 0, Req.SMSCode, Req.TransactionAccount, 1);
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
        public async void MarkTransactionOperatorFail(string StatusMsg, enErrorCode ErrorCode,TransactionQueue Newtransaction)
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
                CreditWalletDrArryTrnIDList.Add(new CreditWalletDrArryTrnID { DrTrnRefNo = Req.TrnNo, Amount = Req.Amount });


                //_WalletService.GetWalletCreditNew(Req.SMSCode, Helpers.GetTimeStamp(), enWalletTrnType.Refund, Req.Amount, Req.MemberID,
                //Req.DebitAccountID, CreditWalletDrArryTrnIDList.ToArray(), Req.TrnNo,1, enWalletTranxOrderType.Credit, Req.ServiceType, (enTrnType)Newtransaction.TrnType);
                //ntrivedi 08-07-2019 GetWalletCreditNew discontinued 
                await _walletTransactionCrDr.GetWalletCreditNewAsync(Req.SMSCode, Helpers.GetTimeStamp(), enWalletTrnType.Withdrawal, Req.Amount, Req.MemberID,
                Req.DebitAccountID, CreditWalletDrArryTrnIDList.ToArray(), Req.TrnNo, 1, enWalletTranxOrderType.Credit, Req.ServiceType, (enTrnType)Newtransaction.TrnType);

                try
                {
                    //komal 16-11-2018 add Activity Notifiation v2
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionFailed);
                    notification.Param1 = Req.TrnNo.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Fail);
                    //Task.Run(()=>_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken));
                    Task.Run(() => _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2));
                    //_ISignalRService.SendActivityNotification("Transaction Failed TrnNo:" + Req.TrnNo, Req.accessToken);

                    //Task.Run(() => Parallel.Invoke(() => _ISignalRService.SendActivityNotificationV2(notification, Req.accessToken),
                    //    ()=> SMSSendWithdrwalTransaction(Req.TrnNo, Req.Amount, Req.SMSCode, Req.TransactionAccount, Newtransaction.MemberMobile, 3),
                    //    ()=> EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, 0, Req.SMSCode, Req.TransactionAccount, 3)));

                    //Uday 07-12-2018 send sms when transaction is Failed
                    SMSSendWithdrwalTransaction(Req.TrnNo, Req.Amount, Req.SMSCode, Req.TransactionAccount, Newtransaction.MemberMobile, 3);
                    //changhe acWalletId to TransactionAccount 

                    //Uday 07-12-2018 send email when transaction is Failed
                    EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, 0, Req.SMSCode, Req.TransactionAccount, 3);
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
                Newtransaction.SetTransactionStatusMsg("Success" + StatusMsg );//WebAPIParseResponseClsObj.StatusMsg)
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
                    //EmailSendAsync(Newtransaction.MemberID.ToString(), Convert.ToInt16(enTransactionStatus.Success), Req.SMSCode, Req.SMSCode, Newtransaction.TrnDate.ToString(), 0, Req.Amount, 0);

                    //Uday 07-12-2018 send sms when transaction is success
                    SMSSendWithdrwalTransaction(Req.TrnNo, Req.Amount, Req.SMSCode, Req.TransactionAccount, Newtransaction.MemberMobile, 2);
                    //changhe acWalletId to TransactionAccount 

                    //Uday 07-12-2018 send email when transaction is Success
                    EmailSendWithdrwalTransaction(Newtransaction.MemberID.ToString(), Req.TrnNo, Req.Amount, 0, Req.SMSCode, Req.TransactionAccount, 2);
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
        public void InsertIntoWithdrawHistory(long SerProID,string RouteTag,string TrnID,string ProviderWalletID, NewWithdrawRequestCls Req)
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
               
                _WalletService.InsertIntoWithdrawHistory(WH);
                HelperForLog.WriteLogIntoFile("InsertIntoWithdrawHistory", ControllerName, "--1--Insert Withdarw History---" + "##TrnNo:" + Req.TrnNo);
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertIntoWithdrawHistory Error-Fail:##TrnNo " + Req.TrnNo, ControllerName, ex);
            }
        }
        public void UpdateIntoWithdrawHistory( NewWithdrawRequestCls Req,short status,string StatusText, string ProviderWalletId = null , string trnid = "")
        {
            try
            {
                var WH = _WithdrawHistoryRepository.GetSingle(e => e.TrnNo == Req.TrnNo);
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

        public Task<BizResponse> CallWebAPI(BizResponse _Resp)
        {
            //TransactionRequest TransactionRequestObj=new TransactionRequest(); 
            ThirdPartyAPIRequest ThirdPartyAPIRequestOnj;
            WebApiConfigurationResponse WebApiConfigurationResponseObj;
            WebAPIParseResponseCls WebAPIParseResponseClsObj=new WebAPIParseResponseCls();
            //long TxnRequestID = 0;
            short IsTxnProceed = 0;
            try
            {
                foreach (TransactionProviderResponse Provider in TxnProviderList)//Make txn on every API
                {

                    Newtransaction.SetServiceProviderData(Provider.ServiceID, Provider.ServiceProID, Provider.ProductID, Provider.RouteID,Provider.SerProDetailID, 0);
                    _TransactionRepository.Update(Newtransaction);

                    WebApiConfigurationResponseObj =_IWebApiData.GetAPIConfiguration(Provider.ThirPartyAPIID);
                    if(WebApiConfigurationResponseObj==null)
                    {
                        _Resp.ReturnMsg = EnResponseMessage.ProcessTrn_ThirdPartyDataNotFoundMsg;
                        _Resp.ReturnCode = enResponseCodeService.Fail;
                        _Resp.ErrorCode = enErrorCode.ProcessTrn_ThirdPartyDataNotFound;
                        Task.Run(()=>MarkTransactionOperatorFail(_Resp.ReturnMsg, _Resp.ErrorCode, Newtransaction));
                        continue;
                    }
                    if (Req.TrnType == enTrnType.Withdraw)
                        Task.Run(() => InsertIntoWithdrawHistory(Provider.ServiceProID, Provider.RouteName, WebAPIParseResponseClsObj.TrnRefNo, Provider.ProviderWalletID, Req));//rita 4-1-19 added ProviderWalletID ,also add in inner join configuration

                    HelperForLog.WriteLogIntoFile("InsertIntoWithdrawHistory", ControllerName, "--2--Insert Withdarw History---" + "##TrnNo:" + Req.TrnNo);
                    ThirdPartyAPIRequestOnj = _IGetWebRequest.MakeWebRequest(Provider.RouteID,Provider.ThirPartyAPIID,Provider.SerProDetailID, Newtransaction);             
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
                            _TransactionObj.APIResponse =_IWebApiSendRequest.SendAPIRequestAsync(ThirdPartyAPIRequestOnj.RequestURL, ThirdPartyAPIRequestOnj.RequestBody, WebApiConfigurationResponseObj.ContentType, 30000, ThirdPartyAPIRequestOnj.keyValuePairsHeader, WebApiConfigurationResponseObj.MethodType);
                            break;
                        case (long)enAppType.SocketApi:

                        case (long)enAppType.BitcoinDeamon:

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
                        Task.Run(()=>MarkTransactionHold(_Resp.ReturnMsg, _Resp.ErrorCode, Newtransaction));
                        IsTxnProceed = 1;//no further call next API
                        break;
                    }
                    //_TransactionObj.APIResponse = "{\"txid\":\"dc0f4454e77cb3bdaa0c707b37eca250dc6ac84a40994cc99c9c063583f50bbc\",\"tx\":\"01000000000101bbdb21bfc18efc9357c2abcbed6e6a6bc47bab2dd78952aa3b1b0aaa48af972c0100000023220020a3dd471b5765c0ac7e7173c00cb8f6d8ede04809442441a78ae71e555f0e3e25ffffffff02ce2a81000000000017a914ed1f54cf86f6b69e860bd118d29b63f6dbb2e17c87988d07000000000017a9141f6b16323e351422509d8f70f2ebe8f77aabc62d87040047304402205871d8dea5a91b9d40b6b8489dfd94184b2cb6338745b95cf71f45c6e23bc12a02202c4d99a057b85a6c0e8fed053278c9e4a1d0faf95931dc7925a52723e57e0f3d01483045022100f52edab9924eb8cc9785f1f27ebf8d53a86c81232ef8bafe7c66615e593283ac02206052b2e7c76f1390a57def1dbca28480ba70b506d20b4245204c8803b2bb85260169522103ca80fd2ca4c7097f386853f52f9224128b15409340524d04bddafc5afdb6b47321026563949e58c2b622d9ad84c53dbd5d1bdc09858ec37d96fbd95af4b49f06a8342102b5208a7ce843e28e2460eff28cd6d770c948a516074f1f0627f3cfb020512c0153aebc310800\",\"status\":\"signed\"}";
                    WebAPIParseResponseClsObj = _WebApiParseResponseObj.TransactionParseResponse(_TransactionObj.APIResponse, Provider.ThirPartyAPIID);
                    NewtransactionReq.SetTrnID(WebAPIParseResponseClsObj.TrnRefNo);
                    NewtransactionReq.SetOprTrnID(WebAPIParseResponseClsObj.OperatorRefNo);
                    Task.Run(()=>_TransactionRequest.Update(NewtransactionReq));


                    //WebAPIParseResponseClsObj.Status = enTransactionStatus.Success;//komal test 
                    if ((WebAPIParseResponseClsObj.Status == enTransactionStatus.Success) || (WebAPIParseResponseClsObj.Status == enTransactionStatus.Hold))
                    {
                        if (Req.TrnType == enTrnType.Withdraw)
                            Task.Run(() => UpdateIntoWithdrawHistory(Req, 6, "Process entry", ThirdPartyAPIRequestOnj.walletID, WebAPIParseResponseClsObj.TrnRefNo));
                    }
                    else
                    {
                        if (Req.TrnType == enTrnType.Withdraw)
                            Task.Run(() => UpdateIntoWithdrawHistory(Req, 2, "Fail", ThirdPartyAPIRequestOnj.walletID));
                    }
                    HelperForLog.WriteLogIntoFile("UpdateIntoWithdrawHistory", ControllerName, "--2--Update Withdarw History---" + "##TrnNo:" + Req.TrnNo);
                    if (WebAPIParseResponseClsObj.Status == enTransactionStatus.Success)
                    {
                        Task.Run(()=>MarkTransactionSuccess(WebAPIParseResponseClsObj.StatusMsg, Newtransaction));
                        IsTxnProceed = 1;//no further call next API
                        break;
                    }
                    else if (WebAPIParseResponseClsObj.Status == enTransactionStatus.OperatorFail)
                    { 
                        continue;
                    }
                    else
                    {
                        Newtransaction.SetTransactionStatusMsg(WebAPIParseResponseClsObj.StatusMsg);
                        Task.Run(()=>_TransactionRepository.Update(Newtransaction));

                        _Resp.ReturnMsg = "Hold";
                        _Resp.ReturnCode = enResponseCodeService.Success;
                        _Resp.ErrorCode = enErrorCode.ProcessTrn_Hold;
                        IsTxnProceed = 1;//no further call next API
                        break;
                    }
                }
                if(IsTxnProceed==0)
                {
                    _Resp.ErrorCode = enErrorCode.ProcessTrn_OprFail;
                    Task.Run(()=>MarkTransactionOperatorFail("Fail" +WebAPIParseResponseClsObj.StatusMsg, _Resp.ErrorCode, Newtransaction));
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
            return Task.FromResult(_Resp);
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
                NewtransactionReq =_TransactionRequest.Add(NewtransactionReq);
                return NewtransactionReq.Id;

            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertTransactionRequest:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return 0;
            }
        }
        #endregion

        // khushali 10-01-2019 unused code
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
        //                    Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), User.Name);
        //                    Provider.Content = Provider.Content.Replace("###TYPE###".ToUpper(), PairName);
        //                    Provider.Content = Provider.Content.Replace("###REQAMOUNT###".ToUpper(), ReqAmount.ToString());
        //                    Provider.Content = Provider.Content.Replace("###STATUS###".ToUpper(), "Success");
        //                    Provider.Content = Provider.Content.Replace("###USER###".ToUpper(), User.Name);
        //                    Provider.Content = Provider.Content.Replace("###CURRENCY###".ToUpper(), BaseMarket);
        //                    Provider.Content = Provider.Content.Replace("###DATETIME###".ToUpper(), TrnDate);
        //                    Provider.Content = Provider.Content.Replace("###AMOUNT###".ToUpper(), Amount.ToString());
        //                    Provider.Content = Provider.Content.Replace("###FEES###".ToUpper(), Fees.ToString());
        //                    Provider.Content = Provider.Content.Replace("###FINAL###".ToUpper(), (Amount + Fees).ToString());
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

        #region Send SMS And Email
        public async Task SMSSendWithdrwalTransaction(long TrnNo, decimal Amount, string CoinName, string WalletId, string MobileNumber, int CancelType)
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

        public async Task EmailSendWithdrwalTransaction(string UserID, long TrnNo,decimal Amount,decimal fees, string CoinName,string DestAddress, int CancelType)
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
                    Task.Run(() => HelperForLog.WriteLogIntoFile("SendEmailTransaction - EmailSendWithdrwalTransaction", ControllerName, " ##TrnNo : " + TrnNo + " ##Type : " + CancelType));


                    communicationParamater.Param1 = User.UserName + "";
                    communicationParamater.Param2 = Amount + "";
                    communicationParamater.Param3 = fees + "";
                    communicationParamater.Param4 = (Amount + fees) + "";
                    communicationParamater.Param5 = CoinName;
                    communicationParamater.Param6 = DestAddress;

                    if (CancelType == 1) // Transaction Create
                    {
                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_WithdrwalTransactionCreate, communicationParamater, enCommunicationServiceType.Email).Result;
                    }
                    else if (CancelType == 2) // Transaction Success
                    {
                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_WithdrwalTransactionSuccess, communicationParamater, enCommunicationServiceType.Email).Result;
                    }
                    else if (CancelType == 3) // Transaction Failed
                    {
                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_WithdrwalTransactionFailed, communicationParamater, enCommunicationServiceType.Email).Result;
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
    }
}
