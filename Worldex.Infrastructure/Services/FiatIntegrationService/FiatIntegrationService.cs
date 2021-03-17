using Worldex.Core.Entities.FiatBankIntegration;
using Worldex.Core.ViewModels.Fiat_Bank_Integration;
using System;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Interfaces;
using Worldex.Core.Enums;
using Worldex.Infrastructure.Interfaces;
using Worldex.Core.Helpers;
using Worldex.Core.ViewModels.FiatBankIntegration;
using Worldex.Core.Entities.FiatBankIntegration;
using Worldex.Core.Entities.KYC;
using Worldex.Core.Entities.Wallet;
using System.Linq;
using System.Collections.Generic;
using Worldex.Infrastructure.Data;
using Microsoft.Extensions.Caching.Memory;

namespace Worldex.Infrastructure.Services.FiatIntegrationService
{
    public class FiatIntegrationService : IFiatIntegration
    {
        public static string ControllerName = "FiatIntegrationService";
        private readonly ICommonRepository<UserBankRequest> _BankRepository;
        private readonly IFiatBankIntegrationRepository _IfiatBankIntegrationRepository;
        private readonly ICommonRepository<UserBankMaster> _UserBankMasterRepo;
        private readonly ICommonRepository<PersonalVerification> _PersonalVerificationRepo;
        private readonly ICommonRepository<FiatTradeConfigurationMaster> _FiatTradeConfigurationMasterRepo;
        private readonly ICommonRepository<FiatCoinConfiguration> _FiatCoinConfiguration;
        private readonly ICommonRepository<FiatCurrencyMaster> _FiatCurrencyMaster;
        private readonly ICommonRepository<WalletTypeMaster> _WalletTypeMaster;
        private readonly IWalletService _walletService;
        private FiatIntegrateRepository _fiatIntegrateRepository;
        private IMemoryCache _cache { get; set; }
        IFiatIntegrateService _fiatIntegrateService;

        public FiatIntegrationService(ICommonRepository<UserBankRequest> BankRepository, IFiatBankIntegrationRepository IfiatBankIntegrationRepository, IWalletService walletService, IMemoryCache Cache,
            ICommonRepository<UserBankMaster> UserBankMasterRepo, IFiatIntegrateService fiatIntegrateService,
            ICommonRepository<PersonalVerification> PersonalVerificationRepo,
            ICommonRepository<FiatTradeConfigurationMaster> FiatTradeConfigurationMasterRepo, ICommonRepository<FiatCoinConfiguration> FiatCoinConfiguration, ICommonRepository<WalletTypeMaster> WalletTypeMaster, FiatIntegrateRepository fiatIntegrateRepository,
               ICommonRepository<FiatCurrencyMaster> FiatCurrencyMaster)
        {
            _cache = Cache;
            _BankRepository = BankRepository;
            _UserBankMasterRepo = UserBankMasterRepo;
            _IfiatBankIntegrationRepository = IfiatBankIntegrationRepository;
            _PersonalVerificationRepo = PersonalVerificationRepo;
            _FiatTradeConfigurationMasterRepo = FiatTradeConfigurationMasterRepo;
            _FiatCoinConfiguration = FiatCoinConfiguration;
            _WalletTypeMaster = WalletTypeMaster;
            _FiatCurrencyMaster = FiatCurrencyMaster;
            _walletService = walletService;
            _fiatIntegrateService = fiatIntegrateService;
            _fiatIntegrateRepository = fiatIntegrateRepository;
        }

        public async Task<BizResponseClass> AcceptRejectUserBankRequest(AdminApprovalReq req, long UserId)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsReqExist = await _BankRepository.GetSingleAsync(i => i.GUID == req.Guid.ToString());
                if (IsReqExist == null)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                else if (IsReqExist.Status != 0)
                {
                    Resp.ErrorCode = enErrorCode.ReqNotPending;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.RequestNotPending;
                    return Resp;
                }
                else
                {
                    if (req.Bit == ApprovalStatus.Accept)
                    {
                        var IsUpdate = await _UserBankMasterRepo.GetSingleAsync(i => i.UserId == IsReqExist.UserId);
                        if (IsUpdate == null)
                        {
                            //Add Bank Master Entry
                            UserBankMaster AddNew = new UserBankMaster
                            {
                                BankAccountHolderName = IsReqExist.BankAcountHolderName,
                                BankAccountNumber = IsReqExist.BankAccountNumber,
                                BankCode = IsReqExist.BankCode,
                                BankName = IsReqExist.BankName,
                                CountryCode = IsReqExist.CountryCode,
                                CurrencyCode = IsReqExist.CurrencyCode,
                                UserId = IsReqExist.UserId,
                                Status = Convert.ToInt16(ServiceStatus.Active),
                                CreatedBy = IsReqExist.UserId,
                                CreatedDate = Helpers.UTC_To_IST(),
                                GUID = Guid.NewGuid().ToString()
                            };
                            await _UserBankMasterRepo.AddAsync(AddNew);
                            Resp.ErrorCode = enErrorCode.Success;
                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RequestAccepted;
                        }
                        else
                        {
                            //Update Bank Master Entry
                            IsUpdate.BankAccountHolderName = IsReqExist.BankAcountHolderName;
                            IsUpdate.BankAccountNumber = IsReqExist.BankAccountNumber;
                            IsUpdate.BankCode = IsReqExist.BankCode;
                            IsUpdate.BankName = IsReqExist.BankName;
                            IsUpdate.CountryCode = IsReqExist.CountryCode;
                            IsUpdate.CurrencyCode = IsReqExist.CurrencyCode;
                            IsUpdate.UserId = IsReqExist.UserId;
                            IsUpdate.Status = Convert.ToInt16(ServiceStatus.Active);
                            IsUpdate.UpdatedBy = IsReqExist.UserId;
                            IsUpdate.UpdatedDate = Helpers.UTC_To_IST();
                            //IsUpdate.GUID = Guid.NewGuid().ToString();

                            _UserBankMasterRepo.UpdateWithAuditLog(IsUpdate);
                            Resp.ErrorCode = enErrorCode.Success;
                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RequestAccepted;
                        }
                    }
                    else
                    {
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RequestRejected;
                    }
                    //Update Request Table Entry
                    if (!string.IsNullOrEmpty(req.Remarks))
                    {
                        IsReqExist.Remarks = req.Remarks;
                    }

                    //2019-10-22 add email

                    string AccNum = IsReqExist.BankAccountNumber;
                    var length = AccNum.Length;
                    string AccNuma = "".PadLeft(length - 4, '*');
                    AccNum = AccNuma + AccNum.Substring(AccNum.Length - 4);
                    if (Convert.ToInt16(req.Bit) == 1)
                    {
                        _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_BankRequestAccepted, IsReqExist.UserId.ToString(), (Convert.ToInt16(req.Bit) == 1 ? "accepted" : "rejected"), AccNum, IsReqExist.BankCode, IsReqExist.BankName, (string.IsNullOrEmpty(req.Remarks) ? "Bank Request " + (Convert.ToInt16(req.Bit) == 1 ? "accepted" : "rejected") : IsReqExist.Remarks), IsReqExist.CountryCode, IsReqExist.CurrencyCode);
                    }
                    else if (Convert.ToInt16(req.Bit) == 9)
                    {
                        _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_BankRequestRejected, IsReqExist.UserId.ToString(), (Convert.ToInt16(req.Bit) == 1 ? "accepted" : "rejected"), AccNum, IsReqExist.BankCode, IsReqExist.BankName, (string.IsNullOrEmpty(req.Remarks) ? "Bank Request " + (Convert.ToInt16(req.Bit) == 1 ? "accepted" : "rejected") : IsReqExist.Remarks), IsReqExist.CountryCode, IsReqExist.CurrencyCode);
                    }
                    ////
                    IsReqExist.Status = Convert.ToInt16(req.Bit);
                    IsReqExist.UpdatedBy = UserId;
                    IsReqExist.UpdatedDate = Helpers.UTC_To_IST();
                    await _BankRepository.UpdateAsync(IsReqExist);
                    return Resp;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AcceptRejectUserBankRequest", ControllerName, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> AddUpdateFiatTradeConfiguration(FiatTradeConfigurationReq req, int id)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                var data = _FiatTradeConfigurationMasterRepo.GetAllList();
                if (data != null && data.Count > 0)
                {
                    data[0].IsBuyEnable = req.IsBuyEnable;
                    data[0].IsSellEnable = req.IsSellEnable;
                    data[0].BuyFee = req.BuyFee;
                    data[0].SellFee = req.SellFee;
                    data[0].BuyFeeType = req.BuyFeeType;
                    data[0].SellFeeType = req.SellFeeType;
                    data[0].BuyNotifyURL = req.BuyNotifyURL;
                    data[0].SellNotifyURL = req.SellNotifyURL;
                    data[0].CallBackURL = req.CallBackURL;
                    data[0].EncryptionKey = req.EncryptionKey;
                    data[0].FiatCurrencyName = req.FiatCurrencyName;
                    data[0].FiatCurrencyRate = req.FiatCurrencyRate;
                    data[0].MaxLimit = req.MaxLimit;
                    data[0].MinLimit = req.MinLimit;
                    data[0].Platform = req.Platform;
                    data[0].SellCallBackURL = req.SellCallBackURL;
                    data[0].WithdrawURL = req.WithdrawURL;
                    data[0].Status = Convert.ToInt16(req.Status == null ? ServiceStatus.Active : req.Status);
                    data[0].TermsAndCondition = req.TermsAndCondition;
                    data[0].UpdatedBy = id;
                    data[0].UpdatedDate = Helpers.UTC_To_IST();
                    _FiatTradeConfigurationMasterRepo.UpdateWithAuditLog(data[0]);

                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                }
                else
                {
                    FiatTradeConfigurationMaster AddNew = new FiatTradeConfigurationMaster
                    {
                        IsBuyEnable = req.IsBuyEnable,
                        IsSellEnable = req.IsSellEnable,
                        BuyFee = req.BuyFee,
                        SellFee = req.SellFee,
                        BuyFeeType = req.BuyFeeType,
                        SellFeeType = req.SellFeeType,
                        BuyNotifyURL = req.BuyNotifyURL,
                        SellNotifyURL = req.SellNotifyURL,
                        CallBackURL = req.CallBackURL,
                        EncryptionKey = req.EncryptionKey,
                        Status = Convert.ToInt16(req.Status == null ? ServiceStatus.Active : req.Status),
                        TermsAndCondition = req.TermsAndCondition,
                        FiatCurrencyName = req.FiatCurrencyName,
                        FiatCurrencyRate = req.FiatCurrencyRate,
                        MaxLimit = req.MaxLimit,
                        MinLimit = req.MinLimit,
                        Platform = req.Platform,
                        SellCallBackURL = req.SellCallBackURL,
                        WithdrawURL = req.WithdrawURL,
                        CreatedBy = id,
                        CreatedDate = Helpers.UTC_To_IST()
                    };
                    await _FiatTradeConfigurationMasterRepo.AddAsync(AddNew);

                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddUpdateFiatTradeConfiguration", ControllerName, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> AddUserBankDetail(AddBankDetailReq Req, long UserId)
        {
            short ReqBit = 0;
            if (Req == null)
            {
                throw new ArgumentNullException(nameof(Req));
            }
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var KYCStatus = await _PersonalVerificationRepo.GetSingleAsync(i => i.UserID == UserId);
                if (KYCStatus == null)
                {
                    Resp.ErrorCode = enErrorCode.KYCDataNotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.FiatNonKYC;
                    return Resp;
                }
                else
                {
                    if (KYCStatus.VerifyStatus != 1)
                    {
                        Resp.ErrorCode = enErrorCode.NeedtoCmpltKYC;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidKYCStatus;
                        return Resp;
                    }
                }
                var AnyPending = await _BankRepository.GetSingleAsync(i => i.UserId == UserId && i.Status == 0);
                if (AnyPending != null)
                {
                    Resp.ErrorCode = enErrorCode.PrevReqAlreadyInProcess;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.PrevReqAlreadyInProcess;
                    return Resp;
                }
                if (Req.BankID == null || Req.BankID.ToString().Equals("00000000-0000-0000-0000-000000000000"))
                {
                    ReqBit = 1;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                }
                else
                {
                    var IsExist = await _UserBankMasterRepo.GetSingleAsync(i => i.GUID == Req.BankID.ToString() && i.UserId == UserId);
                    if (IsExist == null)
                    {
                        Resp.ErrorCode = enErrorCode.FiatBankReqNoRecFound;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                        return Resp;
                    }
                    else
                    {
                        ReqBit = 2;
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                    }
                }
                UserBankRequest AddNew = new UserBankRequest
                {
                    BankAccountNumber = Req.BankAccountNumber,
                    BankAcountHolderName = Req.BankAcountHolderName,
                    BankCode = Req.BankCode,
                    BankName = Req.BankName,
                    CountryCode = Req.CountryCode,
                    CurrencyCode = Req.CurrencyCode,
                    UserId = UserId,
                    CreatedBy = UserId,
                    CreatedDate = Helpers.UTC_To_IST(),
                    RequestType = ReqBit,
                    Status = Convert.ToInt16(ServiceStatus.InActive),
                    GUID = Guid.NewGuid().ToString()
                };
                await _BankRepository.AddAsync(AddNew);
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddUserBankAsync", ControllerName, ex);
                throw;
            }
        }

        public async Task<ListFiatTradeConfigurationRes> GetFiatTradeConfiguration(short? status)
        {
            try
            {
                ListFiatTradeConfigurationRes Detail = new ListFiatTradeConfigurationRes();
                var data = _IfiatBankIntegrationRepository.ListFiatTradeConfiguration(status);
                Detail.Data = data;
                if (data.Count > 0)
                {
                    Detail.ErrorCode = enErrorCode.Success;
                    Detail.ReturnCode = enResponseCode.Success;
                    Detail.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Detail.ErrorCode = enErrorCode.NotFound;
                    Detail.ReturnCode = enResponseCode.Fail;
                    Detail.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Detail;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListUserBankAsync", ControllerName, ex);
                throw;
            }
        }

        public async Task<ListUserBankReq> ListUserBankDetail(short? Status, short? RequestType, long UserId)
        {
            try
            {
                ListUserBankReq Detail = new ListUserBankReq();
                var data = _IfiatBankIntegrationRepository.ListUserBankDetail(Status, RequestType, UserId);
                Detail.Data = data;
                if (data.Count > 0)
                {
                    Detail.ErrorCode = enErrorCode.Success;
                    Detail.ReturnCode = enResponseCode.Success;
                    Detail.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Detail.ErrorCode = enErrorCode.NotFound;
                    Detail.ReturnCode = enResponseCode.Fail;
                    Detail.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Detail;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListUserBankAsync", ControllerName, ex);
                throw;
            }
        }

        public GetBankDetail GetUserbankDetails(long UserId)
        {
            try
            {
                GetBankDetail Detail = new GetBankDetail();

                var UserBankObj = _UserBankMasterRepo.GetSingle(i => i.UserId == UserId && i.Status == 1);

                if (UserBankObj != null)
                {
                    Detail.BankAccountNumber = UserBankObj.BankAccountNumber;
                    Detail.BankAccountHolderName = UserBankObj.BankAccountHolderName;
                    Detail.BankCode = UserBankObj.BankCode;
                    Detail.BankId = UserBankObj.GUID;
                    Detail.BankName = UserBankObj.BankName;
                    Detail.CountryCode = UserBankObj.CountryCode;
                    Detail.CurrencyCode = UserBankObj.CurrencyCode;
                    Detail.ErrorCode = enErrorCode.Success;
                    Detail.ReturnCode = enResponseCode.Success;
                    Detail.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Detail.BankAccountNumber = "";
                    Detail.BankAccountHolderName = "";
                    Detail.BankCode = "";
                    Detail.BankId = "";
                    Detail.BankName = "";
                    Detail.CountryCode = "";
                    Detail.CurrencyCode = "";
                    Detail.ErrorCode = enErrorCode.NotFound;
                    Detail.ReturnCode = enResponseCode.Fail;
                    Detail.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Detail;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetUserbankDetails", ControllerName, ex);
                return null;
            }
        }

        public InsertUpdateCoinRes InsertUpdateFiatConfiguration(ListFiatCoinConfigurationReq Req, long UserId)
        {
            int Count = 0;
            try
            {
                InsertUpdateCoinRes Resp = new InsertUpdateCoinRes();
                if (Req == null)
                {
                    throw new ArgumentNullException(nameof(Req));
                }

                var newExistobj = _FiatCoinConfiguration.GetAllList().Select(i => i.Id).ToList();

                int objIndexMinMax = Req.Request.FindIndex(x => (x.MinAmount > 0 && x.MaxAmount > 0 && (x.MinAmount > x.MaxAmount)) || (x.MinQty > 0 && x.MaxQty > 0 && (x.MinQty > x.MaxQty)));
                if (objIndexMinMax != -1)
                {
                    Resp.Index = objIndexMinMax;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.FiatInvalidMinLimit;
                    Resp.ErrorCode = enErrorCode.FiatInvalidMinLimit;
                    return Resp;
                }
                //int objIndexSameCoin = Req.Request.FindIndex(x => x.FromCurrencyId == x.ToCurrencyId && x.Id == 0);
                //if (objIndexSameCoin != -1)
                //{
                //    Resp.Index = objIndexSameCoin;
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = "Invalid Request detail";
                //    Resp.ErrorCode = enErrorCode.InvalidCurrencyTypeID;
                //    return Resp;
                //}

                foreach (var data in Req.Request)
                {
                    if (data.Id == 0)//insert
                    {
                        var FromWalletTypeObj = _WalletTypeMaster.GetSingle(i => i.Status == 1 && i.Id == data.FromCurrencyId);
                        if (FromWalletTypeObj == null)
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ErrorCode = enErrorCode.InvalidWalletType;
                            Resp.ReturnMsg = EnResponseMessage.InvalidWalletType;
                            continue;
                        }
                        var ToWalletTypeObj = _WalletTypeMaster.GetSingle(i => i.Status == 1 && i.Id == data.ToCurrencyId);
                        if (ToWalletTypeObj == null)
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ErrorCode = enErrorCode.InvalidWalletType;
                            Resp.ReturnMsg = EnResponseMessage.InvalidWalletType;
                            continue;
                        }
                        var IsExist = _FiatCoinConfiguration.GetSingle(i => i.ToCurrencyId == data.ToCurrencyId && i.FromCurrencyId == data.FromCurrencyId && i.TransactionType == data.TransactionType);
                        if (IsExist != null)
                        {
                            if (IsExist.Status == 1)
                            {
                                Resp.ReturnCode = enResponseCode.Fail;
                                Resp.ErrorCode = enErrorCode.Alredy_Exist;
                                Resp.ReturnMsg = EnResponseMessage.Alredy_Exist;
                            }
                            else
                            {
                                //update 
                                IsExist.Rate = data.Rate;
                                IsExist.MinRate = data.MinRate;
                                IsExist.MaxAmount = data.MaxAmount;
                                IsExist.MinAmount = data.MinAmount;
                                IsExist.MaxQty = data.MaxQty;
                                IsExist.MinQty = data.MinQty;
                                IsExist.Status = data.Status;
                                IsExist.UpdatedBy = UserId;
                                IsExist.UpdatedDate = Helpers.UTC_To_IST();

                                _FiatCoinConfiguration.UpdateWithAuditLog(IsExist);
                                Count = Count + 1;
                                Resp.Index = Count;
                                Resp.ReturnCode = enResponseCode.Success;
                                Resp.ErrorCode = enErrorCode.Success;
                                Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                            }
                        }
                        else
                        {
                            //insert
                            FiatCoinConfiguration NewObj = new FiatCoinConfiguration();
                            NewObj.Rate = data.Rate;
                            NewObj.MinRate = data.MinRate;
                            NewObj.MaxAmount = data.MaxAmount;
                            NewObj.MinAmount = data.MinAmount;
                            NewObj.MaxQty = data.MaxQty;
                            NewObj.MinQty = data.MinQty;
                            NewObj.Status = data.Status;
                            NewObj.CreatedBy = UserId;
                            NewObj.CreatedDate = Helpers.UTC_To_IST();
                            NewObj.FromCurrencyId = data.FromCurrencyId;
                            NewObj.ToCurrencyId = data.ToCurrencyId;
                            NewObj.TransactionType = data.TransactionType;

                            _FiatCoinConfiguration.Add(NewObj);
                            Count = Count + 1;
                            Resp.Index = Count;
                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ErrorCode = enErrorCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                            //return Resp;
                        }
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                        continue;
                    }
                    else
                    {
                        if (newExistobj.Contains(data.Id))
                        {
                            var IsExist = _FiatCoinConfiguration.GetSingle(i => i.Id == data.Id);
                            if (IsExist != null)
                            {
                                //update   
                                IsExist.Rate = data.Rate;
                                IsExist.MinRate = data.MinRate;
                                IsExist.MaxAmount = data.MaxAmount;
                                IsExist.MinAmount = data.MinAmount;
                                IsExist.MaxQty = data.MaxQty;
                                IsExist.MinQty = data.MinQty;
                                IsExist.Status = data.Status;
                                IsExist.UpdatedBy = UserId;
                                IsExist.UpdatedDate = Helpers.UTC_To_IST();

                                _FiatCoinConfiguration.UpdateWithAuditLog(IsExist);
                                Count = Count + 1;
                                Resp.Index = Count;
                                Resp.ReturnCode = enResponseCode.Success;
                                Resp.ErrorCode = enErrorCode.Success;
                                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                            }
                            else
                            {
                                //record not found
                                //Count++;
                                Resp.ReturnCode = enResponseCode.Fail;
                                Resp.ErrorCode = enErrorCode.NotFound;
                                Resp.ReturnMsg = EnResponseMessage.NotFound;
                                // return Resp;
                            }
                            newExistobj.Remove(data.Id);
                        }
                        foreach (var DisableObj in newExistobj)
                        {
                            var route3 = _FiatCoinConfiguration.GetSingle(i => i.Id == DisableObj);
                            route3.Status = 9;
                            route3.UpdatedDate = Helpers.UTC_To_IST();
                            route3.UpdatedBy = UserId;
                            _FiatCoinConfiguration.UpdateWithAuditLog(route3);
                        }
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                        continue;
                    }
                }
                // khushali 18-11-2019 handle An unhandled error occurred 
                List<LPTPairFiat> ConfigurationList = _fiatIntegrateRepository.GetPairForBinnance();
                _cache.Set<List<LPTPairFiat>>("LPTPairFiat", ConfigurationList);
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertUpdateFiatConfiguration", ControllerName, ex);
                return null;
            }
        }

        public BizResponseClass InsertUpdateFiatCurrency(FiatCurrencyConfigurationReq Req, long UserId)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                if (Req == null)
                {
                    throw new ArgumentNullException(nameof(Req));
                }

                if (Req.Id == 0)//insert
                {
                    var IsExist = _FiatCurrencyMaster.GetSingle(i => i.CurrencyCode == Req.CurrencyCode);
                    if (IsExist != null)
                    {

                        //update 
                        IsExist.Name = Req.Name;
                        IsExist.USDRate = Req.USDRate;
                        IsExist.Status = Req.Status;
                        IsExist.SellFee = Req.SellFee;
                        IsExist.BuyFee = Req.BuyFee;
                        IsExist.SellFeeType = Req.SellFeeType;
                        IsExist.BuyFeeType = Req.BuyFeeType;
                        IsExist.UpdatedBy = UserId;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        _FiatCurrencyMaster.UpdateWithAuditLog(IsExist);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                        return Resp;

                    }
                    else
                    {
                        //insert
                        FiatCurrencyMaster NewObj = new FiatCurrencyMaster();
                        NewObj.Name = Req.Name;
                        NewObj.CurrencyCode = Req.CurrencyCode;
                        NewObj.USDRate = Req.USDRate;
                        NewObj.Status = Req.Status;
                        NewObj.SellFee = Req.SellFee;
                        NewObj.BuyFee = Req.BuyFee;
                        NewObj.SellFeeType = Req.SellFeeType;
                        NewObj.BuyFeeType = Req.BuyFeeType;
                        NewObj.CreatedBy = UserId;
                        NewObj.CreatedDate = Helpers.UTC_To_IST();


                        _FiatCurrencyMaster.Add(NewObj);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                        return Resp;
                    }
                }
                else
                {
                    var IsExist = _FiatCurrencyMaster.GetSingle(i => i.Id == Req.Id);
                    if (IsExist != null)
                    {
                        //update 
                        IsExist.Name = Req.Name;
                        // IsExist.CurrencyCode = Req.CurrencyCode;
                        IsExist.USDRate = Req.USDRate;
                        IsExist.Status = Req.Status;
                        IsExist.SellFee = Req.SellFee;
                        IsExist.BuyFee = Req.BuyFee;
                        IsExist.SellFeeType = Req.SellFeeType;
                        IsExist.BuyFeeType = Req.BuyFeeType;
                        IsExist.UpdatedBy = UserId;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();

                        _FiatCurrencyMaster.UpdateWithAuditLog(IsExist);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                        return Resp;
                    }
                    else
                    {
                        //record not found
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.NotFound;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                        return Resp;
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertUpdateFiatCurrency", ControllerName, ex);
                return null;
            }
        }

        //public BizResponseClass InsertUpdateFiatConfiguration(FiatCoinConfigurationReq Req, long UserId)
        //{
        //    try
        //    {
        //        BizResponseClass Resp = new BizResponseClass();
        //        if (Req == null)
        //        {
        //            throw new ArgumentNullException(nameof(Req));
        //        }

        //        if (Req.Id == 0)//insert
        //        {
        //            var FromWalletTypeObj = _WalletTypeMaster.GetSingle(i => i.Status == 1 && i.Id == Req.FromCurrencyId);
        //            if (FromWalletTypeObj == null)
        //            {
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ErrorCode = enErrorCode.InvalidWalletType;
        //                Resp.ReturnMsg = EnResponseMessage.InvalidWalletType;
        //                return Resp;
        //            }

        //            var ToWalletTypeObj = _WalletTypeMaster.GetSingle(i => i.Status == 1 && i.Id == Req.ToCurrencyId);
        //            if (ToWalletTypeObj == null)
        //            {
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ErrorCode = enErrorCode.InvalidWalletType;
        //                Resp.ReturnMsg = EnResponseMessage.InvalidWalletType;
        //                return Resp;
        //            }
        //            var IsExist = _FiatCoinConfiguration.GetSingle(i => i.ToCurrencyId == Req.ToCurrencyId && i.FromCurrencyId == Req.FromCurrencyId);
        //            if (IsExist != null)
        //            {
        //                if (IsExist.Status == 1)
        //                {
        //                    //already exist
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ErrorCode = enErrorCode.Alredy_Exist;
        //                    Resp.ReturnMsg = EnResponseMessage.Alredy_Exist;
        //                    return Resp;
        //                }
        //                else
        //                {
        //                    //update 
        //                    //IsExist.BuyFee = Req.BuyFee;
        //                    //IsExist.SellFee = Req.SellFee;
        //                    IsExist.MaxAmount = Req.MaxAmount;
        //                    IsExist.MinAmount = Req.MinAmount;
        //                    IsExist.MaxQty = Req.MaxQty;
        //                    IsExist.MinQty = Req.MinQty;
        //                    IsExist.Status = Req.Status;
        //                    IsExist.UpdatedBy = UserId;
        //                    IsExist.UpdatedDate = Helpers.UTC_To_IST();

        //                    _FiatCoinConfiguration.UpdateWithAuditLog(IsExist);

        //                    Resp.ReturnCode = enResponseCode.Success;
        //                    Resp.ErrorCode = enErrorCode.Success;
        //                    Resp.ReturnMsg = EnResponseMessage.RecordAdded;
        //                    return Resp;
        //                }
        //            }
        //            else
        //            {
        //                //insert
        //                FiatCoinConfiguration NewObj = new FiatCoinConfiguration();
        //                //NewObj.BuyFee = Req.BuyFee;
        //                //NewObj.SellFee = Req.SellFee;
        //                NewObj.MaxAmount = Req.MaxAmount;
        //                NewObj.MinAmount = Req.MinAmount;
        //                NewObj.MaxQty = Req.MaxQty;
        //                NewObj.MinQty = Req.MinQty;
        //                NewObj.Status = Req.Status;
        //                NewObj.CreatedBy = UserId;
        //                NewObj.CreatedDate = Helpers.UTC_To_IST();
        //                NewObj.FromCurrencyId = Req.FromCurrencyId;
        //                NewObj.ToCurrencyId = Req.ToCurrencyId;

        //                _FiatCoinConfiguration.Add(NewObj);

        //                Resp.ReturnCode = enResponseCode.Success;
        //                Resp.ErrorCode = enErrorCode.Success;
        //                Resp.ReturnMsg = EnResponseMessage.RecordAdded;
        //                return Resp;
        //            }
        //        }
        //        else
        //        {
        //            var IsExist = _FiatCoinConfiguration.GetSingle(i => i.Id == Req.Id);
        //            if (IsExist != null)
        //            {
        //                //update 
        //                //IsExist.BuyFee = Req.BuyFee;
        //                //IsExist.SellFee = Req.SellFee;
        //                IsExist.MaxAmount = Req.MaxAmount;
        //                IsExist.MinAmount = Req.MinAmount;
        //                IsExist.MaxQty = Req.MaxQty;
        //                IsExist.MinQty = Req.MinQty;
        //                IsExist.Status = Req.Status;
        //                IsExist.UpdatedBy = UserId;
        //                IsExist.UpdatedDate = Helpers.UTC_To_IST();

        //                _FiatCoinConfiguration.UpdateWithAuditLog(IsExist);

        //                Resp.ReturnCode = enResponseCode.Success;
        //                Resp.ErrorCode = enErrorCode.Success;
        //                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
        //                return Resp;
        //            }
        //            else
        //            {
        //                //record not found
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ErrorCode = enErrorCode.NotFound;
        //                Resp.ReturnMsg = EnResponseMessage.NotFound;
        //                return Resp;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog("InsertUpdateFiatConfiguration", ControllerName, ex);
        //        return null;
        //    }
        //}

        public ListFiatCoinConfigurationRes ListFiatConfiguration(long? FromCurrencyId, long? ToCurrencyId, short? Status, short? TransactionType)
        {
            try
            {
                ListFiatCoinConfigurationRes Detail = new ListFiatCoinConfigurationRes();
                var data = _IfiatBankIntegrationRepository.ListFiatConfiguration(FromCurrencyId, ToCurrencyId, Status, TransactionType);
                Detail.Data = data;
                if (data.Count > 0)
                {
                    Detail.ErrorCode = enErrorCode.Success;
                    Detail.ReturnCode = enResponseCode.Success;
                    Detail.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Detail.ErrorCode = enErrorCode.NotFound;
                    Detail.ReturnCode = enResponseCode.Fail;
                    Detail.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Detail;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListFiatConfiguration", ControllerName, ex);
                return null;
            }
        }
    }
}
