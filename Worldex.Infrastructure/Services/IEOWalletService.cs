using Worldex.Core.ApiModels;
using Worldex.Core.Entities.IEO;
using Worldex.Core.Entities.NewWallet;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.IEOWallet;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace Worldex.Infrastructure.Services
{
    public class IEOWalletService : IIEOWalletService
    {
        private readonly IIEOWalletRepository _iEOWalletRepository;
        private readonly IIEOWalletSPRepositories _IEOwalletSPRepositories;
        private readonly IWalletService _IWalletService;
        private readonly ICommonRepository<IEOBannerMaster> _IEOBannerMaster;
        private readonly ICommonRepository<IEOPurchaseHistory> _IEOPurchaseHistory;
        private readonly ICommonRepository<IEOCurrencyMaster> _IEOCurrencyMaster;
        private readonly ICommonRepository<WalletTypeMaster> _WalletTypeMaster;
        private readonly ICommonRepository<WalletMaster> _WalletMaster;
        private readonly ICommonRepository<WalletAuthorizeUserMaster> _WalletAuthorizeUserMaster;
        Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly ICommonRepository<IEORoundMaster> _IEORoundMaster;
        private readonly ICommonRepository<IEOSlabMaster> _IEOSlabMaster;
        private readonly ICommonRepository<IEOCurrencyPairMapping> _IEOCurrencyPairMapping;

        private static Random random = new Random((int)DateTime.Now.Ticks);

        public IEOWalletService(IIEOWalletRepository IEOWalletRepository, IIEOWalletSPRepositories IEOwalletSPRepositories, IWalletService IWalletService, ICommonRepository<IEOBannerMaster> IEOBannerMaster, Microsoft.Extensions.Configuration.IConfiguration configuration, ICommonRepository<IEOCurrencyMaster> IEOCurrencyMaster, ICommonRepository<WalletAuthorizeUserMaster> WalletAuthorizeUserMaster, ICommonRepository<WalletMaster> WalletMaster, ICommonRepository<WalletTypeMaster> WalletTypeMaster, ICommonRepository<IEORoundMaster> IEORoundMaster, ICommonRepository<IEOSlabMaster> IEOSlabMaster, ICommonRepository<IEOCurrencyPairMapping> IEOCurrencyPairMapping, ICommonRepository<IEOPurchaseHistory> IEOPurchaseHistory)
        {
            _iEOWalletRepository = IEOWalletRepository;
            _IWalletService = IWalletService;
            _IEOwalletSPRepositories = IEOwalletSPRepositories;
            _IEOBannerMaster = IEOBannerMaster;
            _configuration = configuration;
            _IEOCurrencyMaster = IEOCurrencyMaster;
            _WalletAuthorizeUserMaster = WalletAuthorizeUserMaster;
            _WalletMaster = WalletMaster;
            _WalletTypeMaster = WalletTypeMaster;
            _IEORoundMaster = IEORoundMaster;
            _IEOCurrencyPairMapping = IEOCurrencyPairMapping;
            _IEOSlabMaster = IEOSlabMaster;
            _IEOPurchaseHistory = IEOPurchaseHistory;
        }

        public PreConfirmResponse Confirmation(string PaidCurrencyWallet, decimal PaidQauntity, string PaidCurrency, string DeliveryCurrency, string RoundID, string Remarks, long UserID)
        {
            PreConfirmResponse PreConfirmResponse = new PreConfirmResponse();
            try
            {
                PreConfirmResponse = _IEOwalletSPRepositories.CallSP_ConfirmTrn(PaidCurrencyWallet, PaidQauntity, PaidCurrency, DeliveryCurrency, RoundID, Remarks, UserID);
                if (PreConfirmResponse.ReturnCode == 0)
                {
                    var history = _IEOPurchaseHistory.GetSingle(i => i.Guid == PreConfirmResponse.RefNo);
                    if (history != null)
                    {
                        _IWalletService.EmailSendAsyncV1(EnTemplateType.EMAIL_IEOTransaction, UserID.ToString(), DeliveryCurrency, PaidCurrency, Helpers.DoRoundForTrading(PreConfirmResponse.InstantDeliverdQuantity, 8).ToString(), Helpers.DoRoundForTrading(PreConfirmResponse.MaxDeliverQuantity, 8).ToString(), PreConfirmResponse.RefNo, Helpers.DoRoundForTrading(history.MaximumDeliveredQuantiyWOBonus, 8).ToString(), Helpers.DoRoundForTrading(history.BonusPercentage, 8).ToString(), Helpers.DoRoundForTrading(history.BonusAmount, 8).ToString());
                    }
                }
                return PreConfirmResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                PreConfirmResponse.ReturnCode = enResponseCode.InternalError;
                return PreConfirmResponse;
            }
        }

        public ListIEOPurchaseHistoryResponse ListPurchaseHistory(DateTime FromDate, DateTime ToDate, int Page, int PageSize, long PaidCurrency, long DeliveryCurrency, int UserID)
        {
            ListIEOPurchaseHistoryResponse ListIEOPurchaseHistoryResponse = new ListIEOPurchaseHistoryResponse();
            try
            {
                DateTime newToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                int TotalCount = 0;
                var PurchaseHistory = _iEOWalletRepository.ListPurchaseHistory(FromDate, newToDate, Page + 1, PageSize, PaidCurrency, DeliveryCurrency, UserID, ref TotalCount);
                ListIEOPurchaseHistoryResponse.TotalCount = TotalCount;
                ListIEOPurchaseHistoryResponse.PageNo = Page;
                ListIEOPurchaseHistoryResponse.PageSize = PageSize;
                if (PurchaseHistory.Count == 0)
                {
                    ListIEOPurchaseHistoryResponse.ReturnCode = enResponseCode.Fail;
                    ListIEOPurchaseHistoryResponse.ReturnMsg = EnResponseMessage.NotFound;
                    ListIEOPurchaseHistoryResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    ListIEOPurchaseHistoryResponse.PurchaseHistory = PurchaseHistory;
                    ListIEOPurchaseHistoryResponse.ReturnCode = enResponseCode.Success;
                    ListIEOPurchaseHistoryResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    ListIEOPurchaseHistoryResponse.ErrorCode = enErrorCode.Success;

                }
                return ListIEOPurchaseHistoryResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                ListIEOPurchaseHistoryResponse.ReturnCode = enResponseCode.InternalError;
                return ListIEOPurchaseHistoryResponse;
            }
        }

        public ListIEOPurchaseHistoryResponseBO ListPurchaseHistoryBO(DateTime FromDate, DateTime ToDate, int Page, int PageSize, long PaidCurrency, long DeliveryCurrency, string Email)
        {
            ListIEOPurchaseHistoryResponseBO ListIEOPurchaseHistoryResponse = new ListIEOPurchaseHistoryResponseBO();
            try
            {
                DateTime newToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                int TotalCount = 0;
                var PurchaseHistory = _iEOWalletRepository.ListPurchaseHistoryBO(FromDate, newToDate, Page + 1, PageSize, PaidCurrency, DeliveryCurrency, Email, ref TotalCount);
                ListIEOPurchaseHistoryResponse.TotalCount = TotalCount;
                ListIEOPurchaseHistoryResponse.PageNo = Page;
                ListIEOPurchaseHistoryResponse.PageSize = PageSize;
                if (PurchaseHistory.Count == 0)
                {
                    ListIEOPurchaseHistoryResponse.ReturnCode = enResponseCode.Fail;
                    ListIEOPurchaseHistoryResponse.ReturnMsg = EnResponseMessage.NotFound;
                    ListIEOPurchaseHistoryResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    ListIEOPurchaseHistoryResponse.PurchaseHistory = PurchaseHistory;
                    ListIEOPurchaseHistoryResponse.ReturnCode = enResponseCode.Success;
                    ListIEOPurchaseHistoryResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    ListIEOPurchaseHistoryResponse.ErrorCode = enErrorCode.Success;

                }
                return ListIEOPurchaseHistoryResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                ListIEOPurchaseHistoryResponse.ReturnCode = enResponseCode.InternalError;
                return ListIEOPurchaseHistoryResponse;
            }
        }

        public ListIEOWalletResponse ListWallet(Int16 Status)
        {
            ListIEOWalletResponse ListIEOWalletResponse = new ListIEOWalletResponse();
            try
            {
                var walletResponse = _iEOWalletRepository.ListIEOWallet(Status);
                if (walletResponse.Count == 0)
                {
                    ListIEOWalletResponse.ReturnCode = enResponseCode.Fail;
                    ListIEOWalletResponse.ReturnMsg = EnResponseMessage.NotFound;
                    ListIEOWalletResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    foreach (IEOWalletResponse IEOWalletResponse in walletResponse)
                    {
                        IEOWalletResponse.IconPath = _configuration["IEOGetCurrencyImagePath"].ToString() + "/" + IEOWalletResponse.IconPath;
                    }
                    ListIEOWalletResponse.Wallets = walletResponse;
                    ListIEOWalletResponse.ReturnCode = enResponseCode.Success;
                    ListIEOWalletResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    ListIEOWalletResponse.ErrorCode = enErrorCode.Success;

                }
                return ListIEOWalletResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                ListIEOWalletResponse.ReturnCode = enResponseCode.InternalError;
                return ListIEOWalletResponse;
            }
        }

        public PreConfirmResponseV2 PreConfirmation(string PaidCurrencyWallet, decimal PaidQauntity, string PaidCurrency, string DeliveryCurrency, string RoundID, string Remarks, Int64 USerID)
        {
            PreConfirmResponseV2 PreConfirmResponse = new PreConfirmResponseV2();
            try
            {
                PreConfirmResponse = _IEOwalletSPRepositories.CallSP_PreConfirm(PaidCurrencyWallet, PaidQauntity, PaidCurrency, DeliveryCurrency, RoundID, Remarks, USerID);
                return PreConfirmResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                PreConfirmResponse.ReturnCode = enResponseCode.InternalError;
                return PreConfirmResponse;
            }
        }

        #region Banner Configuration

        public BizResponseClass InsertUpdateBannerConfiguration(IEOBannerRequest Request, long UserId, string FilePath)
        {
            BizResponseClass response = new BizResponseClass();
            try
            {
                var IsExist = _IEOBannerMaster.GetSingle(i => i.Status != 9);

                if (IsExist == null)
                {
                    IEOBannerMaster Obj = _IEOBannerMaster.Add(new IEOBannerMaster()
                    {
                        GUID = Guid.NewGuid().ToString().Replace("-", ""),
                        BannerPath = FilePath,
                        BannerName = Request.BannerName,
                        Description = Request.Description,
                        Message = Request.Message,
                        TermsAndCondition = Request.TermsAndCondition,
                        IsKYCReuired = Request.IsKYCReuired,
                        Status = Request.Status,
                        CreatedBy = UserId,
                        CreatedDate = Helpers.UTC_To_IST(),
                    });
                    response.ReturnCode = enResponseCode.Success;
                    response.ReturnMsg = EnResponseMessage.RecordAdded;
                    response.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    IsExist.BannerPath = (FilePath == "" ? IsExist.BannerPath : FilePath);
                    IsExist.BannerName = Request.BannerName;
                    IsExist.Description = Request.Description;
                    IsExist.Message = Request.Message;
                    IsExist.TermsAndCondition = Request.TermsAndCondition;
                    IsExist.IsKYCReuired = Request.IsKYCReuired;
                    IsExist.Status = Request.Status;
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();

                    _IEOBannerMaster.UpdateWithAuditLog(IsExist);
                    response.ReturnCode = enResponseCode.Success;
                    response.ReturnMsg = EnResponseMessage.RecordUpdated;
                    response.ErrorCode = enErrorCode.Success;
                }
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                response.ReturnCode = enResponseCode.InternalError;
                return response;
            }
        }

        public GetIEOBannerRes GetBannerConfiguration()
        {
            GetIEOBannerRes response = new GetIEOBannerRes();
            try
            {
                var data = _IEOBannerMaster.GetSingle(i => i.Status != 9);
                if (data == null)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.NotFound;
                    response.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    string webRootPath = _configuration["IEOGetBannerImagePath"].ToString();
                    response = new GetIEOBannerRes()
                    {
                        Id = data.Id,
                        GUID = data.GUID,
                        BannerPath = webRootPath + "/" + data.BannerPath,
                        BannerName = data.BannerName,
                        Description = data.Description,
                        Message = data.Message,
                        TermsAndCondition = data.TermsAndCondition,
                        IsKYCReuired = data.IsKYCReuired,
                        Status = data.Status,
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.FindRecored,
                        ErrorCode = enErrorCode.Success
                    };
                }
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                response.ReturnCode = enResponseCode.InternalError;
                return response;
            }
        }
        #endregion

        public BizResponseClass InsertUpdateAdminWalletConfiguration(IEOAdminWalletRequest Request, long UserId)
        {
            BizResponseClass response = new BizResponseClass();
            try
            {
                var IsExist = _IEOCurrencyMaster.GetSingle(i => i.Status != 9 && i.Id == Request.Id);

                if (IsExist == null)
                {
                    var samenameObj = _IEOCurrencyMaster.GetSingle(i => i.Status != 9 && i.CurrencyName == Request.ShortCode);
                    if (samenameObj != null)
                    {
                        if (samenameObj.CurrencyName == Request.ShortCode)
                        {
                            response.ReturnCode = enResponseCode.Fail;
                            response.ReturnMsg = "CurrencyName Already Exist!";
                            response.ErrorCode = enErrorCode.AlredyExist;
                            return response;
                        }
                    }

                    IEOCurrencyMaster Obj = new IEOCurrencyMaster();

                    Obj.Guid = Guid.NewGuid().ToString().Replace("-", "");
                    Obj.IEOTokenTypeName = Request.CoinType;
                    Obj.CurrencyName = Request.ShortCode;
                    Obj.Description = Request.Description;
                    Obj.Rounds = Request.Rounds;
                    Obj.IconPath = Request.WalletPath;
                    Obj.Status = Request.Status;
                    Obj.CreatedBy = UserId;
                    Obj.CreatedDate = Helpers.UTC_To_IST();
                    Obj.Rate = Request.Rate;
                    _IEOCurrencyMaster.Add(Obj);

                    WalletTypeMaster wallettypeObj = new WalletTypeMaster();

                    var wallettypeObjExist = _WalletTypeMaster.GetSingle(i => i.WalletTypeName == Request.ShortCode);
                    if (wallettypeObjExist == null)
                    {
                        wallettypeObj.WalletTypeName = Request.ShortCode;
                        wallettypeObj.Description = Request.WalletName;
                        wallettypeObj.IsDefaultWallet = Convert.ToInt16(0);
                        wallettypeObj.IsLocal = Convert.ToInt16(0);
                        wallettypeObj.IsWithdrawalAllow = Convert.ToInt16(0);
                        wallettypeObj.IsTransactionWallet = Convert.ToInt16(0);
                        wallettypeObj.IsDepositionAllow = Convert.ToInt16(0);
                        wallettypeObj.ConfirmationCount = Convert.ToInt16(3);
                        wallettypeObj.CurrencyTypeID = Convert.ToInt64(1);
                        wallettypeObj.Status = Request.Status;
                        wallettypeObj.CreatedBy = UserId;
                        wallettypeObj.CreatedDate = Helpers.UTC_To_IST();

                        wallettypeObj = _WalletTypeMaster.Add(wallettypeObj);
                    }
                    else
                    {
                        // wallettypeobj.WalletTypeName = Request.ShortCode;
                        //wallettypeObjExist.Description = Request.WalletName;
                        //wallettypeObjExist.Status = Request.Status;
                        //wallettypeObjExist.UpdatedBy = UserId;
                        //wallettypeObjExist.UpdatedDate = Helpers.UTC_To_IST();

                        //_WalletTypeMaster.UpdateWithAuditLog(wallettypeObjExist);
                    }

                    WalletMaster walletObj = new WalletMaster();
                    walletObj.Walletname = Request.ShortCode + " IEO Wallet";
                    walletObj.OrgID = 1;
                    walletObj.ExpiryDate = Helpers.UTC_To_IST().AddYears(1);
                    walletObj.IsValid = true;
                    walletObj.UserID = _iEOWalletRepository.getOrgID();
                    walletObj.WalletTypeID = (wallettypeObj == null ? wallettypeObjExist.Id : wallettypeObj.Id);
                    walletObj.Balance = 0;
                    walletObj.PublicAddress = "";
                    walletObj.IsDefaultWallet = 1;
                    walletObj.CreatedBy = UserId;
                    walletObj.CreatedDate = Helpers.UTC_To_IST();
                    walletObj.Status = Convert.ToInt16(ServiceStatus.Active);
                    walletObj.AccWalletID = RandomGenerateAccWalletId(UserId, 1);
                    walletObj.WalletUsageType = Convert.ToInt16(EnWalletUsageType.IEOWallet);
                    walletObj = _WalletMaster.Add(walletObj);

                    WalletAuthorizeUserMaster authuserObj = new WalletAuthorizeUserMaster();
                    authuserObj.RoleID = 1;
                    authuserObj.UserID = UserId;
                    authuserObj.Status = 1;
                    authuserObj.CreatedBy = UserId;
                    authuserObj.CreatedDate = Helpers.UTC_To_IST();
                    authuserObj.WalletID = walletObj.Id;
                    authuserObj.OrgID = Convert.ToInt64(walletObj.OrgID);
                    _WalletAuthorizeUserMaster.Add(authuserObj);

                    response.ReturnCode = enResponseCode.Success;
                    response.ReturnMsg = EnResponseMessage.RecordAdded;
                    response.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    IsExist.IEOTokenTypeName = Request.CoinType;
                    //IsExist.CurrencyName = Request.ShortCode;
                    IsExist.Description = Request.Description;
                    IsExist.Rounds = Request.Rounds;
                    IsExist.IconPath = (Request.WalletPath == "" ? IsExist.IconPath : Request.WalletPath);
                    IsExist.Status = Request.Status;
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    IsExist.Rate = Request.Rate;

                    _IEOCurrencyMaster.UpdateWithAuditLog(IsExist);

                    var wallettypeobj = _WalletTypeMaster.GetSingle(i => i.Status == 1 && i.WalletTypeName == IsExist.CurrencyName);
                    if (wallettypeobj != null)
                    {
                        //// wallettypeobj.WalletTypeName = Request.ShortCode;
                        wallettypeobj.Description = Request.WalletName;
                        //wallettypeobj.Status = Request.Status;
                        wallettypeobj.UpdatedBy = UserId;
                        wallettypeobj.UpdatedDate = Helpers.UTC_To_IST();

                        _WalletTypeMaster.UpdateWithAuditLog(wallettypeobj);

                        var walletobj = _WalletMaster.GetSingle(i => i.Status == 1 && i.WalletTypeID == wallettypeobj.Id && i.UserID == UserId && i.WalletUsageType == Convert.ToInt16(EnWalletUsageType.IEOWallet));
                        if (walletobj != null)
                        {
                            walletobj.Status = Request.Status;
                            walletobj.UpdatedBy = UserId;
                            walletobj.UpdatedDate = Helpers.UTC_To_IST();

                            _WalletMaster.UpdateWithAuditLog(walletobj);
                        }
                    }

                    response.ReturnCode = enResponseCode.Success;
                    response.ReturnMsg = EnResponseMessage.RecordUpdated;
                    response.ErrorCode = enErrorCode.Success;
                }
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                response.ReturnCode = enResponseCode.InternalError;
                return response;
            }
        }

        public ListGetIEOAdminWalletRes GetAdminWalletConfiguration(long UserId)
        {
            ListGetIEOAdminWalletRes response = new ListGetIEOAdminWalletRes();
            try
            {
                var data = _iEOWalletRepository.GetAdminWalletConfiguration(UserId);
                string webRootPath = _configuration["IEOGetWalletImagePath"].ToString();
                data.ForEach(i => { i.WalletPath = webRootPath + "/" + i.WalletPath; });
                if (data.Count == 0)
                {
                    data = new List<GetIEOAdminWalletRes>();
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.NotFound;
                    response.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    response.Data = data;
                    response.ReturnCode = enResponseCode.Success;
                    response.ReturnMsg = EnResponseMessage.FindRecored;
                    response.ErrorCode = enErrorCode.Success;
                }
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                response.ReturnCode = enResponseCode.InternalError;
                return response;
            }
        }

        public string RandomGenerateAccWalletId(long userID, byte isDefaultWallet)
        {
            try
            {
                long maxValue = 999999999;
                long minValue = 100000000;
                long x = (long)Math.Round(random.NextDouble() * (maxValue - minValue - 1)) + minValue;
                string userIDStr = x.ToString() + userID.ToString().PadLeft(6, '0') + isDefaultWallet.ToString();
                return userIDStr;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BizResponseClass InsertRoundConfiguration(InsertRoundConfigurationReq Request, long UserId, string fileName)
        {
            BizResponseClass response = new BizResponseClass();
            try
            {
                var IEOObj = _IEOCurrencyMaster.GetSingle(i => i.Id == Request.IEOCurrencyId);
                if (IEOObj == null)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.InvalidCoin;
                    response.ErrorCode = enErrorCode.InvalidCoinName;
                    return response;
                }
                //2019-8-12 added total round and validate
                var TotalRound = _IEORoundMaster.FindBy(i => i.Status == 1 && i.IEOCurrencyId == IEOObj.Id).Select(i => i.Id).ToList();
                if (TotalRound.Count >= IEOObj.Rounds)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = "Not Allowed more than" + IEOObj.Rounds + " Rounds";
                    response.ErrorCode = enErrorCode.IEOTotalRoundExceed;
                    return response;
                }
                if (Request.Bonus > 100)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = "Bonus Percentage is not more than 100.";
                    response.ErrorCode = enErrorCode.IEOTotalBonusExceed;
                    return response;
                }
                IEORoundMaster roundObj = new IEORoundMaster();
                roundObj.Status = Request.Status;
                roundObj.CreatedBy = UserId;
                roundObj.CreatedDate = Helpers.UTC_To_IST();
                roundObj.MaximumPurchaseAmt = Request.MaxLimit;
                roundObj.MinimumPurchaseAmt = Request.MinLimit;
                roundObj.IEOCurrencyId = IEOObj.Id;
                roundObj.StartDate = Request.FromDate;
                roundObj.EndDate = Request.ToDate;
                roundObj.TotalSupply = Request.TotalSupply;
                roundObj.AllocatedSupply = 0;
                roundObj.CurrencyRate = 0;
                roundObj.OccurrenceLimit = Request.MaxOccurence;
                roundObj.Bonus = Request.Bonus;
                roundObj.Guid = Guid.NewGuid().ToString().Replace("-", "");
                roundObj.BGPath = fileName;

                roundObj = _IEORoundMaster.Add(roundObj);

                decimal totalsum = 0;

                foreach (var data in Request.AllocationDetail)
                {
                    totalsum = totalsum + data.AllocationPercentage;
                }
                if (totalsum != 100)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = "Total Allocation percentage must be 100.";
                    response.ErrorCode = enErrorCode.IEOInvalidTotal;
                    return response;
                }
                //insert
                foreach (var data in Request.ExchangeCurrency)
                {
                    var wallettypeobj = _WalletTypeMaster.GetSingle(i => i.Id == data.PaidCurrencyId);
                    if (wallettypeobj != null)
                    {
                        IEOCurrencyPairMapping mappingobj = new IEOCurrencyPairMapping();
                        mappingobj.Status = Request.Status;
                        mappingobj.CreatedBy = UserId;
                        mappingobj.CreatedDate = Helpers.UTC_To_IST();
                        mappingobj.IEOWalletTypeId = Request.IEOCurrencyId;
                        mappingobj.PaidWalletTypeId = data.PaidCurrencyId;
                        mappingobj.PurchaseRate = (data.Rate == 0 ? 1 : data.Rate);
                        mappingobj.InstantPercentage = 0;
                        mappingobj.ConvertCurrencyType = data.IsUSD;//percentage
                        mappingobj.RoundId = roundObj.Id;
                        mappingobj.Guid = Guid.NewGuid().ToString().Replace("-", "");

                        mappingobj = _IEOCurrencyPairMapping.Add(mappingobj);
                    }
                }

                foreach (var data in Request.AllocationDetail)
                {
                    IEOSlabMaster slabobj = new IEOSlabMaster();
                    slabobj.Status = Request.Status;
                    slabobj.CreatedBy = UserId;
                    slabobj.CreatedDate = Helpers.UTC_To_IST();
                    slabobj.Guid = Guid.NewGuid().ToString().Replace("-", "");
                    slabobj.RoundId = roundObj.Id;
                    slabobj.Value = data.AllocationPercentage;
                    slabobj.Priority = 1;
                    slabobj.DurationType = data.AllocationPeriodType;
                    slabobj.Bonus = data.Bonus;
                    if (slabobj.DurationType == 0)
                    {
                        slabobj.Duration = 0;
                    }
                    else if (slabobj.DurationType == 1)
                    {
                        slabobj.Duration = data.AllocationNoofPeriod;
                    }
                    else if (slabobj.DurationType == 2)
                    {
                        slabobj.Duration = data.AllocationNoofPeriod * 30;
                    }
                    else
                    {
                        slabobj.Duration = 30;
                    }
                    slabobj = _IEOSlabMaster.Add(slabobj);
                }
                response.ReturnCode = enResponseCode.Success;
                response.ReturnMsg = EnResponseMessage.RecordAdded;
                response.ErrorCode = enErrorCode.Success;
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                response.ReturnCode = enResponseCode.InternalError;
                return response;
            }
        }

        public BizResponseClass UpdateRoundConfiguration(UpdateRoundConfigurationReq Request, long UserId, string fileName)
        {
            BizResponseClass response = new BizResponseClass();
            try
            {
                var IEOObj = _IEOCurrencyMaster.GetSingle(i => i.Id == Request.IEOCurrencyId);
                if (IEOObj == null)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.InvalidCoin;
                    response.ErrorCode = enErrorCode.InvalidCoinName;
                    return response;
                }
                if (Request.Bonus > 100)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = "Max Limit Exceed for Bonus Percentage.";
                    response.ErrorCode = enErrorCode.IEOTotalBonusExceed;
                    return response;
                }

                IEORoundMaster roundObj = _IEORoundMaster.GetSingle(i => i.Guid == Request.RoundId);
                if (roundObj == null)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = "Round Configuration Not Found.";
                    response.ErrorCode = enErrorCode.NotFound;
                    return response;
                }
                roundObj.Status = Request.Status;
                roundObj.UpdatedBy = UserId;
                roundObj.UpdatedDate = Helpers.UTC_To_IST();
                roundObj.MaximumPurchaseAmt = Request.MaxLimit;
                roundObj.MinimumPurchaseAmt = Request.MinLimit;
                //roundObj.IEOCurrencyId = IEOObj.Id;
                roundObj.StartDate = Request.FromDate;
                roundObj.EndDate = Request.ToDate;
                roundObj.TotalSupply = Request.TotalSupply;
                roundObj.OccurrenceLimit = Request.MaxOccurence;
                roundObj.Bonus = Request.Bonus;
                roundObj.BGPath = (fileName == "" ? roundObj.BGPath : fileName);

                _IEORoundMaster.UpdateWithAuditLog(roundObj);

                decimal totalsum = 0;

                foreach (var data in Request.AllocationDetail)
                {
                    totalsum = totalsum + data.AllocationPercentage;
                }
                if (totalsum != 100)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = "Total Allocation percentage must be 100.";
                    response.ErrorCode = enErrorCode.IEOInvalidTotal;
                    return response;
                }
                ///2019-8-10 added update -remove logic
                var newExistpairobj = _IEOCurrencyPairMapping.FindBy(i => i.RoundId == roundObj.Id).Select(i => i.Guid).ToList();
                foreach (var data in Request.ExchangeCurrency)
                {
                    if (data.ExchangeId != null)
                    {
                        if (newExistpairobj.Contains(data.ExchangeId))
                        {
                            IEOCurrencyPairMapping mappingobj = _IEOCurrencyPairMapping.GetSingle(i => i.Guid == data.ExchangeId);
                            if (mappingobj != null)
                            {
                                mappingobj.Status = Request.Status;
                                mappingobj.UpdatedBy = UserId;
                                mappingobj.UpdatedDate = Helpers.UTC_To_IST();
                                mappingobj.ConvertCurrencyType = data.IsUSD;
                                //mappingobj.IEOWalletTypeId = Request.IEOCurrencyId;
                                mappingobj.PaidWalletTypeId = data.PaidCurrencyId;
                                mappingobj.PurchaseRate = (data.Rate == 0 ? 1 : data.Rate);

                                _IEOCurrencyPairMapping.UpdateWithAuditLog(mappingobj);
                            }
                            newExistpairobj.Remove(data.ExchangeId);
                        }
                    }
                    else
                    {
                        IEOCurrencyPairMapping newmappingobj = new IEOCurrencyPairMapping();
                        newmappingobj.Status = Request.Status;
                        newmappingobj.CreatedBy = UserId;
                        newmappingobj.CreatedDate = Helpers.UTC_To_IST();
                        newmappingobj.IEOWalletTypeId = Request.IEOCurrencyId;
                        newmappingobj.PaidWalletTypeId = data.PaidCurrencyId;
                        newmappingobj.PurchaseRate = (data.Rate == 0 ? 1 : data.Rate);
                        newmappingobj.InstantPercentage = 0;
                        newmappingobj.ConvertCurrencyType = data.IsUSD;//percentage
                        newmappingobj.RoundId = roundObj.Id;
                        newmappingobj.Guid = Guid.NewGuid().ToString().Replace("-", "");

                        newmappingobj = _IEOCurrencyPairMapping.Add(newmappingobj);
                    }
                }
                foreach (var DisableObj in newExistpairobj)
                {
                    var route3 = _IEOCurrencyPairMapping.GetSingle(i => i.Guid == DisableObj);
                    route3.Status = 9;
                    route3.UpdatedDate = Helpers.UTC_To_IST();
                    route3.UpdatedBy = UserId;
                    _IEOCurrencyPairMapping.Update(route3);
                }

                var newExistexchangeobj = _IEOSlabMaster.FindBy(i => i.RoundId == roundObj.Id).Select(i => i.Guid).ToList();
                foreach (var data in Request.AllocationDetail)
                {
                    if (data.DetailId != null)
                    {
                        if (newExistexchangeobj.Contains(data.DetailId))
                        {
                            IEOSlabMaster slabobj = _IEOSlabMaster.GetSingle(i => i.Guid == data.DetailId);
                            if (slabobj != null)
                            {
                                slabobj.Status = Request.Status;
                                slabobj.UpdatedBy = UserId;
                                slabobj.UpdatedDate = Helpers.UTC_To_IST();
                                slabobj.Value = data.AllocationPercentage;
                                slabobj.DurationType = data.AllocationPeriodType;
                                slabobj.Bonus = data.Bonus;
                                if (slabobj.DurationType == 0)
                                {
                                    slabobj.Duration = 0;
                                }
                                else if (slabobj.DurationType == 1)
                                {
                                    slabobj.Duration = data.AllocationNoofPeriod;
                                }
                                else if (slabobj.DurationType == 2)
                                {
                                    slabobj.Duration = data.AllocationNoofPeriod * 30;
                                }
                                else
                                {
                                    slabobj.Duration = 30;
                                }
                                _IEOSlabMaster.UpdateWithAuditLog(slabobj);
                            }
                            newExistexchangeobj.Remove(data.DetailId);
                        }
                    }
                    else
                    {
                        IEOSlabMaster newslabobj = new IEOSlabMaster();
                        newslabobj.Status = Request.Status;
                        newslabobj.CreatedBy = UserId;
                        newslabobj.CreatedDate = Helpers.UTC_To_IST();
                        newslabobj.Guid = Guid.NewGuid().ToString().Replace("-", "");
                        newslabobj.RoundId = roundObj.Id;
                        newslabobj.Value = data.AllocationPercentage;
                        newslabobj.Priority = 1;
                        newslabobj.DurationType = data.AllocationPeriodType;
                        newslabobj.Bonus = data.Bonus;
                        if (newslabobj.DurationType == 0)
                        {
                            newslabobj.Duration = 0;
                        }
                        else if (newslabobj.DurationType == 1)
                        {
                            newslabobj.Duration = data.AllocationNoofPeriod;
                        }
                        else if (newslabobj.DurationType == 2)
                        {
                            newslabobj.Duration = data.AllocationNoofPeriod * 30;
                        }
                        else
                        {
                            newslabobj.Duration = 30;
                        }
                        newslabobj = _IEOSlabMaster.Add(newslabobj);
                    }
                }
                foreach (var DisableObj in newExistexchangeobj)
                {
                    var route3 = _IEOSlabMaster.GetSingle(i => i.Guid == DisableObj);
                    route3.Status = 9;
                    route3.UpdatedDate = Helpers.UTC_To_IST();
                    route3.UpdatedBy = UserId;
                    _IEOSlabMaster.Update(route3);
                }

                //old
                #region Old
                //insert
                //foreach (var data in Request.ExchangeCurrency)
                //{
                //    IEOCurrencyPairMapping mappingobj = _IEOCurrencyPairMapping.GetSingle(i => i.Guid == data.ExchangeId);
                //    if (mappingobj != null)
                //    {
                //        mappingobj.Status = Request.Status;
                //        mappingobj.UpdatedBy = UserId;
                //        mappingobj.UpdatedDate = Helpers.UTC_To_IST();
                //        //mappingobj.IEOWalletTypeId = Request.IEOCurrencyId;
                //        mappingobj.PaidWalletTypeId = data.PaidCurrencyId;
                //        mappingobj.PurchaseRate = (data.Rate == 0 ? 1 : data.Rate);

                //        _IEOCurrencyPairMapping.UpdateWithAuditLog(mappingobj);
                //    }
                //    else
                //    {
                //        IEOCurrencyPairMapping newmappingobj = new IEOCurrencyPairMapping();
                //        newmappingobj.Status = Request.Status;
                //        newmappingobj.CreatedBy = UserId;
                //        newmappingobj.CreatedDate = Helpers.UTC_To_IST();
                //        newmappingobj.IEOWalletTypeId = Request.IEOCurrencyId;
                //        newmappingobj.PaidWalletTypeId = data.PaidCurrencyId;
                //        newmappingobj.PurchaseRate = (data.Rate == 0 ? 1 : data.Rate);
                //        newmappingobj.InstantPercentage = 0;
                //        newmappingobj.ConvertCurrencyType = 1;//percentage
                //        newmappingobj.RoundId = roundObj.Id;
                //        newmappingobj.Guid = Guid.NewGuid().ToString().Replace("-", "");

                //        newmappingobj = _IEOCurrencyPairMapping.Add(newmappingobj);
                //    }
                //}
                #endregion

                #region Old
                //foreach (var data in Request.AllocationDetail)
                //{
                //    IEOSlabMaster slabobj = _IEOSlabMaster.GetSingle(i => i.Guid == data.DetailId);
                //    if (slabobj != null)
                //    {
                //        slabobj.Status = Request.Status;
                //        slabobj.UpdatedBy = UserId;
                //        slabobj.UpdatedDate = Helpers.UTC_To_IST();
                //        slabobj.Value = data.AllocationPercentage;
                //        slabobj.DurationType = data.AllocationPeriodType;
                //        slabobj.Bonus = data.Bonus;
                //        if (slabobj.DurationType == 0)
                //        {
                //            slabobj.Duration = 0;
                //        }
                //        else if (slabobj.DurationType == 1)
                //        {
                //            slabobj.Duration = data.AllocationNoofPeriod;
                //        }
                //        else if (slabobj.DurationType == 2)
                //        {
                //            slabobj.Duration = data.AllocationNoofPeriod * 30;
                //        }
                //        else
                //        {
                //            slabobj.Duration = 30;
                //        }
                //        _IEOSlabMaster.UpdateWithAuditLog(slabobj);
                //    }
                //    else
                //    {
                //        IEOSlabMaster newslabobj = new IEOSlabMaster();
                //        newslabobj.Status = Request.Status;
                //        newslabobj.CreatedBy = UserId;
                //        newslabobj.CreatedDate = Helpers.UTC_To_IST();
                //        newslabobj.Guid = Guid.NewGuid().ToString().Replace("-", "");
                //        newslabobj.RoundId = roundObj.Id;
                //        newslabobj.Value = data.AllocationPercentage;
                //        newslabobj.Priority = 1;
                //        newslabobj.DurationType = data.AllocationPeriodType;
                //        newslabobj.Bonus = data.Bonus;
                //        if (newslabobj.DurationType == 0)
                //        {
                //            newslabobj.Duration = 0;
                //        }
                //        else if (newslabobj.DurationType == 1)
                //        {
                //            newslabobj.Duration = data.AllocationNoofPeriod;
                //        }
                //        else if (newslabobj.DurationType == 2)
                //        {
                //            newslabobj.Duration = data.AllocationNoofPeriod * 30;
                //        }
                //        else
                //        {
                //            newslabobj.Duration = 30;
                //        }
                //        newslabobj = _IEOSlabMaster.Add(newslabobj);
                //    }
                //}
                #endregion

                response.ReturnCode = enResponseCode.Success;
                response.ReturnMsg = EnResponseMessage.RecordUpdated;
                response.ErrorCode = enErrorCode.Success;
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                response.ReturnCode = enResponseCode.InternalError;
                return response;
            }
        }

        public ListRoundConfigurationResponse ListIEORoundConfiguration(short Status)
        {
            ListRoundConfigurationResponse ListRoundConfigurationResponse = new ListRoundConfigurationResponse();
            string webRootPath = _configuration["IEOGetCurrencyImagePath"].ToString();
            try
            {
                var walletResponse = _iEOWalletRepository.ListIEORounds(Status);
                if (walletResponse.Count == 0)
                {
                    ListRoundConfigurationResponse.ReturnCode = enResponseCode.Fail;
                    ListRoundConfigurationResponse.ReturnMsg = EnResponseMessage.NotFound;
                    ListRoundConfigurationResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    ListRoundConfigurationResponse.RoundDetails = walletResponse;
                    foreach (IEORoundResponse IEORoundResponse in ListRoundConfigurationResponse.RoundDetails)
                    {
                        IEORoundResponse.BGPath = webRootPath + "/" + IEORoundResponse.BGPath;
                    }
                    ListRoundConfigurationResponse.ReturnCode = enResponseCode.Success;
                    ListRoundConfigurationResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    ListRoundConfigurationResponse.ErrorCode = enErrorCode.Success;

                }
                return ListRoundConfigurationResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                ListRoundConfigurationResponse.ReturnCode = enResponseCode.InternalError;
                return ListRoundConfigurationResponse;
            }
        }

        public BizResponseClass IEOAdminWalletDeposit(IEOAdminWalletCreditReq Req)
        {
            BizResponseClass response = new BizResponseClass();
            try
            {
                response = _IEOwalletSPRepositories.Callsp_IEOAdminWalletCreditBalance(Req);
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                response.ReturnCode = enResponseCode.InternalError;
                return response;
            }
        }

        public ListAllocateTokenCountRes IEOTokenCount(short IsAllocate)
        {
            ListAllocateTokenCountRes response = new ListAllocateTokenCountRes();
            try
            {
                var data = _iEOWalletRepository.IEOTokenCount(IsAllocate);
                if (data.Count == 0)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.NotFound;
                    response.ErrorCode = enErrorCode.NotFound;
                    return response;
                }
                else
                {
                    response.Data = data;
                    response.ReturnCode = enResponseCode.Success;
                    response.ReturnMsg = EnResponseMessage.FindRecored;
                    response.ErrorCode = enErrorCode.Success;
                    return response;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                response.ReturnCode = enResponseCode.InternalError;
                return response;
            }
        }

        public ListTokenCountRes IEOTradeTokenCount()
        {
            ListTokenCountRes response = new ListTokenCountRes();
            try
            {
                var data = _iEOWalletRepository.IEOTradeTokenCount();
                if (data.Count == 0)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.NotFound;
                    response.ErrorCode = enErrorCode.NotFound;
                    return response;
                }
                else
                {
                    response.Data = data;
                    response.ReturnCode = enResponseCode.Success;
                    response.ReturnMsg = EnResponseMessage.FindRecored;
                    response.ErrorCode = enErrorCode.Success;
                    return response;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                response.ReturnCode = enResponseCode.InternalError;
                return response;
            }
        }

        public ListIEOTokenReportDataRes IEOTokenReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, string Email, string PaidCurrency, string DeliveredCurrency, short? Status, string TrnRefNo)
        {
            ListIEOTokenReportDataRes response = new ListIEOTokenReportDataRes();
            try
            {
                response.PageNo = PageNo;
                response.PageSize = PageSize;
                var data = _iEOWalletRepository.IEOTokenReport(PageNo + 1, PageSize, FromDate, ToDate, Email, PaidCurrency, DeliveredCurrency, Status, TrnRefNo);
                response.TotalCount = data.TotalCount;
                if (data.Data.Count == 0)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.NotFound;
                    response.ErrorCode = enErrorCode.NotFound;
                    return response;
                }
                else
                {
                    response.Data = data.Data;
                    response.ReturnCode = enResponseCode.Success;
                    response.ReturnMsg = EnResponseMessage.FindRecored;
                    response.ErrorCode = enErrorCode.Success;
                    return response;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                response.ReturnCode = enResponseCode.InternalError;
                return response;
            }
        }

        public ListIEOAllocatedTokenReportDataRes IEOAllocatedTokenReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, string Email, string PaidCurrency, string DeliveredCurrency, short? Status, string TrnRefNo)
        {
            ListIEOAllocatedTokenReportDataRes response = new ListIEOAllocatedTokenReportDataRes();
            try
            {
                response.PageNo = PageNo;
                response.PageSize = PageSize;
                var data = _iEOWalletRepository.IEOAllocatedTokenReport(PageNo + 1, PageSize, FromDate, ToDate, Email, PaidCurrency, DeliveredCurrency, Status, TrnRefNo);
                response.TotalCount = data.TotalCount;
                if (data.Data.Count == 0)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.NotFound;
                    response.ErrorCode = enErrorCode.NotFound;
                    return response;
                }
                else
                {
                    response.Data = data.Data;
                    response.ReturnCode = enResponseCode.Success;
                    response.ReturnMsg = EnResponseMessage.FindRecored;
                    response.ErrorCode = enErrorCode.Success;
                    return response;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                response.ReturnCode = enResponseCode.InternalError;
                return response;
            }
        }
    }
}
