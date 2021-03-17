using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities;
using Worldex.Core.Entities.NewWallet;
using Worldex.Core.Entities.User;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletOperations;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services
{
    public class CommonWalletFunction : ICommonWalletFunction
    {
        private readonly ICommonRepository<WalletMaster> _commonRepository;
        private readonly ICommonRepository<WalletLimitConfiguration> _WalletLimitRepository;
        private readonly ICommonRepository<MemberShadowBalance> _ShadowBalRepo;
        private readonly IWalletRepository _walletRepository1;
        private readonly IWalletRepository _repository;
        private readonly ICommonRepository<MemberShadowLimit> _ShadowLimitRepo;
        private readonly ICommonRepository<WalletTypeMaster> _WalletTypeMasterRepository;
        private readonly ICommonRepository<ChargeRuleMaster> _chargeRuleMaster;
        private readonly ICommonRepository<WalletTrnLimitConfiguration> _walletTrnLimitConfiguration;
        private readonly ICommonRepository<AddressMaster> _addressMaster;
        private readonly IWalletSPRepositories _walletSPRepositories;
        //  private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IMessageService _messageService;
        //private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        //private readonly ISignalRService _signalRService;
        //private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;

        public CommonWalletFunction(ICommonRepository<WalletTrnLimitConfiguration> walletTrnLimitConfiguration, ICommonRepository<WalletLimitConfiguration> WalletLimitRepository, ICommonRepository<WalletMaster> commonRepository, IWalletRepository repository, ICommonRepository<MemberShadowBalance> ShadowBalRepo, IWalletRepository walletRepository, ICommonRepository<MemberShadowLimit> ShadowLimitRepo, ICommonRepository<WalletTypeMaster> WalletTypeMasterRepository, ICommonRepository<ChargeRuleMaster> chargeRuleMaster, ICommonRepository<AddressMaster> addressMaster, IWalletSPRepositories walletSPRepositories)//UserManager<ApplicationUser> userManager,, IMessageService messageService,  ISignalRService signalRService, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue
        {
            _commonRepository = commonRepository;
            _walletSPRepositories = walletSPRepositories;
            _repository = repository;
            _ShadowBalRepo = ShadowBalRepo;
            _walletRepository1 = walletRepository;
            _ShadowLimitRepo = ShadowLimitRepo;
            _WalletTypeMasterRepository = WalletTypeMasterRepository;
            _chargeRuleMaster = chargeRuleMaster;
            _WalletLimitRepository = WalletLimitRepository;
            _addressMaster = addressMaster;
            //    _pushSMSQueue = pushSMSQueue;
            _walletTrnLimitConfiguration = walletTrnLimitConfiguration;
            // _messageService = messageService;
            //    _userManager = userManager;
            //_signalRService = signalRService;
            //_pushNotificationsQueue = pushNotificationsQueue;
        }

        public decimal GetLedgerLastPostBal(long walletId)
        {
            try
            {
                var bal = _repository.GetLedgerLastPostBal(walletId);
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public enErrorCode CheckShadowLimit(long WalletID, decimal Amount, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet)
        {
            try
            {
                //ntrivedi 15-02-2019 for margin wallet
                var Walletobj = _commonRepository.GetSingle(item => item.Id == WalletID && item.WalletUsageType == Convert.ToInt16(enWalletUsageType));
                if (Walletobj != null)
                {
                    var Balobj = _ShadowBalRepo.GetSingle(item => item.WalletID == WalletID);
                    if (Balobj != null)
                    {
                        if ((Balobj.ShadowAmount + Amount) <= Walletobj.Balance)
                        {
                            return enErrorCode.Success;
                        }
                        return enErrorCode.InsufficientBalance;
                    }
                    else
                    {
                        var typeobj = _walletRepository1.GetTypeMappingObj(Walletobj.UserID);
                        if (typeobj != -1) //ntrivedi 04-11-2018 
                        {
                            var Limitobj = _ShadowLimitRepo.GetSingle(item => item.MemberTypeId == typeobj);
                            if (Limitobj != null)
                            {
                                if ((Limitobj.ShadowLimitAmount + Amount) <= Walletobj.Balance)
                                {
                                    return enErrorCode.Success;
                                }
                                return enErrorCode.InsufficientBalance;
                            }
                            return enErrorCode.Success; // IF ENTRY NOT FOUND THEN SUCCESS NTRIVEDI
                        }
                        return enErrorCode.MemberTypeNotFound;
                    }
                }
                return enErrorCode.WalletNotFound;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);

                throw ex;
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

        public async Task<enErrorCode> CheckShadowLimitAsync(long WalletID, decimal Amount, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet)
        {
            try
            {
                Task<WalletMaster> obj1 = _commonRepository.GetSingleAsync(item => item.Id == WalletID && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                WalletMaster Walletobj = await obj1;
                if (Walletobj != null)
                {
                    Task<MemberShadowBalance> Balobj1 = _ShadowBalRepo.GetSingleAsync(item => item.WalletID == WalletID);
                    MemberShadowBalance Balobj = await Balobj1;
                    if (Balobj != null)
                    {
                        if ((Balobj.ShadowAmount + Amount) <= Walletobj.Balance)
                        {
                            return enErrorCode.Success;
                        }
                        return enErrorCode.InsufficientBalance;
                    }
                    else
                    {
                        Task<long> typeobj1 = _walletRepository1.GetTypeMappingObjAsync(Walletobj.UserID);
                        long typeobj = await typeobj1;
                        if (typeobj != -1) //ntrivedi 04-11-2018 
                        {
                            Task<MemberShadowLimit> Limitobj1 = _ShadowLimitRepo.GetSingleAsync(item => item.MemberTypeId == typeobj);
                            MemberShadowLimit Limitobj = await Limitobj1;
                            if (Limitobj != null)
                            {
                                if ((Limitobj.ShadowLimitAmount + Amount) <= Walletobj.Balance)
                                {
                                    return enErrorCode.Success;
                                }
                                return enErrorCode.InsufficientBalance;
                            }
                            return enErrorCode.Success; // IF ENTRY NOT FOUND THEN SUCCESS NTRIVEDI
                        }
                        return enErrorCode.MemberTypeNotFound;
                    }
                }
                return enErrorCode.WalletNotFound;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);

                throw ex;
            }
        }

        //vsolanki 2018-11-24
        public async Task<enErrorCode> InsertUpdateShadowAsync(long WalletID, decimal Amount, string Remarks, long WalleTypeId, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet)
        {
            try
            {
                //ntrivedi 15-02-2019 for margin wallet not use other wallet
                Task<WalletMaster> obj1 = _commonRepository.GetSingleAsync(item => item.Id == WalletID && item.WalletUsageType == Convert.ToInt16(enWalletUsageType));
                Task<MemberShadowBalance> Balobj1 = _ShadowBalRepo.GetSingleAsync(item => item.WalletID == WalletID);
                WalletMaster Walletobj = await obj1;
                if (Walletobj != null)
                {
                    MemberShadowBalance Balobj = await Balobj1;
                    if (Balobj != null)
                    {
                        Balobj.ShadowAmount = Balobj.ShadowAmount + Amount;
                        //update
                        var objs = _ShadowBalRepo.UpdateAsync(Balobj);
                        return enErrorCode.Success;
                    }
                    else
                    {
                        //insert
                        MemberShadowBalance newBalobj = new MemberShadowBalance();
                        newBalobj.CreatedDate = Helpers.UTC_To_IST();
                        newBalobj.CreatedBy = 1;
                        newBalobj.UpdatedBy = 1;
                        newBalobj.UpdatedDate = Helpers.UTC_To_IST();
                        newBalobj.Status = 1;
                        newBalobj.WalletID = WalletID;
                        newBalobj.ShadowAmount = Amount;
                        newBalobj.Remarks = Remarks;
                        newBalobj.MemberShadowLimitId = 0;
                        newBalobj.WalletTypeId = WalleTypeId;
                        _ShadowBalRepo.Add(newBalobj);
                        return enErrorCode.Success;
                    }
                }
                return enErrorCode.WalletNotFound;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);

                throw ex;
            }
        }

        //vsolanki 2018-11-26
        public async Task<enErrorCode> UpdateShadowAsync(long WalletID, decimal Amount, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet)
        {
            try
            {

                //ntrivedi 15-02-2019 for margin wallet not use other wallet
                Task<WalletMaster> obj1 = _commonRepository.GetSingleAsync(item => item.Id == WalletID && item.WalletUsageType == Convert.ToInt16(enWalletUsageType));
                Task<MemberShadowBalance> Balobj1 = _ShadowBalRepo.GetSingleAsync(item => item.WalletID == WalletID);
                WalletMaster Walletobj = await obj1;
                if (Walletobj != null)
                {
                    MemberShadowBalance Balobj = await Balobj1;
                    if (Balobj != null)
                    {
                        if (Balobj.ShadowAmount > Amount)
                        {
                            Balobj.ShadowAmount = Balobj.ShadowAmount - Amount;
                            //update
                            var objs = _ShadowBalRepo.UpdateAsync(Balobj);
                            return enErrorCode.Success;
                        }
                        return enErrorCode.InvalidAmount;
                    }
                }
                return enErrorCode.WalletNotFound;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);

                throw ex;
            }
        }

        public WalletTransactionQueue InsertIntoWalletTransactionQueue(Guid Guid, enWalletTranxOrderType TrnType, decimal Amount, long TrnRefNo, DateTime TrnDate, DateTime? UpdatedDate,
           long WalletID, string WalletType, long MemberID, string TimeStamp, enTransactionStatus Status, string StatusMsg, enWalletTrnType enWalletTrnType,
           Int64 ErrorCode = 0, decimal holdChargeAmount = 0, decimal chargeAmount = 0)
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
            return walletTransactionQueue;
        }

        public async Task<BizResponseClass> CheckWithdrawBeneficiary(string address, long userID, string smscode)
        {
            try
            {
                int cnt = _walletRepository1.IsSelfAddress(address, userID, smscode);
                if (cnt > 0)
                {
                    return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.CheckWithdrawBeneficiary_SelfAddressFound };
                }
                cnt = _walletRepository1.IsInternalAddress(address, userID, smscode);
                if (cnt > 0)
                {
                    return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.CheckWithdrawBeneficiary_InternalAddressFound };
                }
                else
                {
                    return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.CheckWithdrawBeneficiary_AddressNotFound };
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        public async Task<WalletTrnLimitResponse> CheckWalletLimitAsyncV1(enWalletLimitType TrnType, long WalletId, decimal Amount, long TrnNo = 0)
        {
            try
            {
                WalletTrnLimitResponse Resp2 = new WalletTrnLimitResponse();
                Resp2 = _walletSPRepositories.Callsp_CheckWalletTranLimit(Convert.ToInt16(TrnType), WalletId, Amount, TrnNo);
                return Resp2;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task SMSSendAsyncV1(EnTemplateType templateType, string UserID, string WalletName = null, string SourcePrice = null, string DestinationPrice = null, string ONOFF = null, string Coin = null, string TrnType = null, string TrnNo = null)
        {
            try
            {
                CommunicationParamater communicationParamater = new CommunicationParamater();
                ApplicationUser User = new ApplicationUser();
                User = new ApplicationUser();//await _userManager.FindByIdAsync(UserID);
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

                        //var SmsData =  _messageService.ReplaceTemplateMasterData(templateType, communicationParamater, enCommunicationServiceType.SMS).Result;
                        //if (SmsData != null)
                        //{
                        //    if (SmsData.IsOnOff == 1)
                        //    {
                        //        SendSMSRequest Request = new SendSMSRequest();
                        //        Request.Message = SmsData.Content;
                        //        Request.MobileNo = Convert.ToInt64(User.Mobile);
                        //        //_pushSMSQueue.Enqueue(Request);
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SMSSendAsyncV1" + " - Data- " + templateType.ToString(), "WalletService", ex);
            }
        }

        public async Task EmailSendAsyncV1(EnTemplateType templateType, string UserID, string Param1 = "", string Param2 = "", string Param3 = "", string Param4 = "", string Param5 = "", string Param6 = "", string Param7 = "", string Param8 = "", string Param9 = "")
        {
            try
            {
                CommunicationParamater communicationParamater = new CommunicationParamater();
                SendEmailRequest Request = new SendEmailRequest();
                ApplicationUser User = new ApplicationUser();
                User = new ApplicationUser();
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
                        //var EmailData = _messageService.ReplaceTemplateMasterData(templateType, communicationParamater, enCommunicationServiceType.Email).Result;
                        //if (EmailData != null)
                        //{
                        //    Request.Body = EmailData.Content;
                        //    Request.Subject = EmailData.AdditionalInfo;
                        //    Request.EmailType = Convert.ToInt16(EnEmailType.Template);
                        //    Request.Recepient = User.Email;
                        //    _pushNotificationsQueue.Enqueue(Request);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " -Data- " + templateType.ToString(), "WalletService", ex);
            }
        }
    }
}
