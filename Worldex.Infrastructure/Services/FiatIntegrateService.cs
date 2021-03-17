using Newtonsoft.Json;
using RSAEncryption;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.FiatBankIntegration;
using Worldex.Core.Entities.KYC;
using Worldex.Core.Entities.User;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.FiatBankIntegration;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.Interfaces;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Infrastructure.Services.Transaction;
using Worldex.Core.Entities.NewWallet;
using System.Threading;
using Worldex.Core.Entities.Configuration;

namespace Worldex.Infrastructure.Services
{
    /// <summary>
    /// vsolanki 2019-10-9 Added New Service implentation for Fiat COnfiguration
    /// </summary>
    public class FiatIntegrateService : IFiatIntegrateService
    {
        #region COTR
        private readonly TrnMasterConfiguration _trnMasterConfiguration;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private ICommonRepository<UserBankMaster> _UserBankMaster;
        private ICommonRepository<BuySellTopUpRequest> _BuySellTopUpRequest;
        private ICommonRepository<TransactionQueue> _TransactionQueue;
        private ICommonRepository<SellTopUpRequest> _SellTopUpRequest;
        private ICommonRepository<WalletTypeMaster> _WalletTypeMaster;
        private ICommonRepository<WalletMaster> _WalletMaster;
        private ICommonRepository<PersonalVerification> _PersonalVerification;
        private ICommonRepository<FiatTradeConfigurationMaster> _FiatTradeConfigurationMaster;
        private ICommonRepository<FiatCoinConfiguration> _FiatCoinConfiguration;
        private ICommonRepository<AddressMaster> _AddressMaster;
        private ICommonRepository<WithdrawHistory> _WithdrawHistory;
        private ICommonRepository<FiatCurrencyMaster> _FiatCurrencyMaster;
        private IFiatIntegrateRepository _IFiatIntegrateRepository;
        private IWalletService _walletService;
        private readonly IWebApiSendRequest _webApiSendRequest;
        private readonly IWithdrawTransactionV1 _withdrawTransactionV1;
        private readonly IBackOfficeTrnService _backOfficeTrnService;
        private readonly IFrontTrnService _frontTrnService;
        private readonly IResdisTradingManagment _IResdisTradingManagment;
        private ICommonRepository<CurrencyRateMaster> _CurrencyRateMaster;
        private ICommonRepository<ServiceMaster> _ServiceMaster;


        public FiatIntegrateService(ICommonRepository<BuySellTopUpRequest> BuySellTopUpRequest, ICommonRepository<WalletTypeMaster> WalletTypeMaster, TrnMasterConfiguration trnMasterConfiguration,
            ICommonRepository<WalletMaster> WalletMaster, ICommonRepository<AddressMaster> AddressMaster,
            Microsoft.Extensions.Configuration.IConfiguration configuration, IFiatIntegrateRepository IFiatIntegrateRepository, IBackOfficeTrnService backOfficeTrnService,
            ICommonRepository<PersonalVerification> PersonalVerification, IWalletService walletService,
            ICommonRepository<FiatTradeConfigurationMaster> FiatTradeConfiguationMaster, ICommonRepository<SellTopUpRequest> SellTopUpRequest, ICommonRepository<TransactionQueue> TransactionQueue,
            ICommonRepository<UserBankMaster> UserBankMaster, IWebApiSendRequest webApiSendRequest, ICommonRepository<WithdrawHistory> WithdrawHistory, ICommonRepository<FiatCoinConfiguration> FiatCoinConfiguration, ICommonRepository<FiatCurrencyMaster> FiatCurrencyMaster, IWithdrawTransactionV1 withdrawTransactionV1, IFrontTrnService frontTrnService, IResdisTradingManagment ResdisTradingManagment, ICommonRepository<CurrencyRateMaster> CurrencyRateMaster, ICommonRepository<ServiceMaster> ServiceMaster)
        {
            _BuySellTopUpRequest = BuySellTopUpRequest;
            _WalletTypeMaster = WalletTypeMaster;
            _WalletMaster = WalletMaster;
            _AddressMaster = AddressMaster;
            _configuration = configuration;
            _IFiatIntegrateRepository = IFiatIntegrateRepository;
            _PersonalVerification = PersonalVerification;
            _walletService = walletService;
            _FiatTradeConfigurationMaster = FiatTradeConfiguationMaster;
            _UserBankMaster = UserBankMaster;
            _SellTopUpRequest = SellTopUpRequest;
            _webApiSendRequest = webApiSendRequest;
            // _SellTopUpRequest = SellTopUpRequest;
            _TransactionQueue = TransactionQueue;
            _WithdrawHistory = WithdrawHistory;
            _FiatCoinConfiguration = FiatCoinConfiguration;
            _FiatCurrencyMaster = FiatCurrencyMaster;
            _withdrawTransactionV1 = withdrawTransactionV1;
            _backOfficeTrnService = backOfficeTrnService;
            _frontTrnService = frontTrnService;
            _IResdisTradingManagment = ResdisTradingManagment;
            _CurrencyRateMaster = CurrencyRateMaster;
            _trnMasterConfiguration = trnMasterConfiguration;
            _ServiceMaster = ServiceMaster;
        }
        #endregion

        #region Method
        public BuyTopUpResponse FiatBuyTopUpRequest(BuyTopUpRequest Req, ApplicationUser user)
        {
            BuyTopUpResponse Res = new BuyTopUpResponse();
            try
            {
                var IsKYC = _PersonalVerification.GetSingle(i => i.UserID == user.Id && i.VerifyStatus == 1);//success
                if (IsKYC == null)
                {
                    Res.ReturnMsg = "Need to Complete KYC!!";
                    Res.ErrorCode = enErrorCode.NeedtoCmpltKYC;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                if (string.IsNullOrEmpty(user.Mobile))
                {
                    Res.ReturnMsg = "Mobile is Required";
                    Res.ReturnCode = enResponseCode.Fail;
                    Res.ErrorCode = enErrorCode.MobileIsRequired;
                    return Res;
                }
                var FromWalletTypeObj = _FiatCurrencyMaster.GetSingle(i => i.CurrencyCode == Req.FromCurrency && i.Status == 1);//fait
                if (FromWalletTypeObj == null)
                {
                    Res.ReturnMsg = EnResponseMessage.InvalidCoinName;
                    Res.ErrorCode = enErrorCode.InvalidCoinName;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                var ToWalletTypeObj = _WalletTypeMaster.GetSingle(i => i.Status == 1 && i.WalletTypeName == Req.ToCurrency && i.CurrencyTypeID == 1);//crypto
                if (ToWalletTypeObj == null)
                {
                    Res.ReturnMsg = EnResponseMessage.InvalidCoinName;
                    Res.ErrorCode = enErrorCode.InvalidCoinName;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                var ToWalletObj = _WalletMaster.GetSingle(i => i.Status == 1 && i.WalletTypeID == ToWalletTypeObj.Id && i.UserID == user.Id && i.IsDefaultWallet == 1);
                if (ToWalletObj == null)
                {
                    Res.ReturnMsg = EnResponseMessage.InvalidWallet;
                    Res.ErrorCode = enErrorCode.InvalidWallet;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                var ToWalletAddressObj = _AddressMaster.GetSingle(i => i.Status == 1 && i.WalletId == ToWalletObj.Id && i.OriginalAddress == Req.Address);
                if (ToWalletAddressObj == null)
                {
                    Res.ReturnMsg = EnResponseMessage.InvalidAddress;
                    Res.ErrorCode = enErrorCode.InvalidAddress;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }

                var CoinObj = _FiatCoinConfiguration.GetSingle(i => i.FromCurrencyId == ToWalletTypeObj.Id && i.Status == 1 && i.TransactionType == 1);
                if (CoinObj == null)
                {
                    Res.ReturnMsg = "Coin Configuration data Not Found.";
                    Res.ErrorCode = enErrorCode.CoinConfigurationDataNotFond;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                else
                {
                    if (CoinObj.MinQty > 0 && CoinObj.MinQty > Req.ToAmount)
                    {
                        Res.ReturnMsg = "Min Quantity limit Exceeded";
                        Res.ErrorCode = enErrorCode.MinQtyExccedd;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                    if (CoinObj.MaxQty > 0 && CoinObj.MaxQty < Req.ToAmount)
                    {
                        Res.ReturnMsg = "Max Quantity limit Exceeded";
                        Res.ErrorCode = enErrorCode.MaxQtyExccedd;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                    if (CoinObj.MinAmount > 0 && CoinObj.MinAmount > Req.FromAmount)
                    {
                        Res.ReturnMsg = "Min Amount limit Exceeded";
                        Res.ErrorCode = enErrorCode.MinAmtExccedd;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                    if (CoinObj.MaxAmount > 0 && CoinObj.MaxAmount < Req.FromAmount)
                    {
                        Res.ReturnMsg = "Max Amount limit Exceeded";
                        Res.ErrorCode = enErrorCode.MaxAmtExccedd;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                }

                var Key = "";
                var tradeObj = _FiatTradeConfigurationMaster.GetSingle(i => i.Status == 1);
                if (tradeObj != null)
                {
                    //if ((tradeObj.MinLimit > Req.ToAmount) || (tradeObj.MaxLimit < Req.ToAmount))
                    //{
                    //    Res.ReturnMsg = EnResponseMessage.InsufficantBal;
                    //    Res.ErrorCode = enErrorCode.InsufficantBal;
                    //    Res.ReturnCode = enResponseCode.Fail;
                    //    return Res;
                    //}
                    //if (tradeObj.MinLimit != 0)
                    //{
                    //    if (tradeObj.MinLimit > Req.FromAmount)
                    //    {
                    //        Res.ReturnMsg = "Cann't Exceed MinLimit";
                    //       Res.ErrorCode = enErrorCode.InvalidMinLimit;
                    //        Res.ReturnCode = enResponseCode.Fail;
                    //        return Res;
                    //    }
                    //}
                    //if (tradeObj.MaxLimit != 0)
                    //{
                    //    if (tradeObj.MaxLimit < Req.FromAmount)
                    //    {
                    //        Res.ReturnMsg = "Cann't Exceed MaxLimit";
                    //        Res.ErrorCode = enErrorCode.InvalidMaxLimit;
                    //        Res.ReturnCode = enResponseCode.Fail;
                    //        return Res;
                    //    }
                    //}
                    Res.NotifyURL = tradeObj.BuyNotifyURL;
                    Key = tradeObj.EncryptionKey;
                }
                else
                {
                    Res.NotifyURL = "";
                }
                BuySellTopUpRequest newObj = new BuySellTopUpRequest();
                newObj.Guid = Guid.NewGuid().ToString().Replace("-", "");
                newObj.FromAmount = Req.FromAmount;
                newObj.ToAmount = Req.ToAmount;
                newObj.CoinRate = Req.CoinRate;
                newObj.FiatConverationRate = Req.FiatConverationRate;
                newObj.Fee = Req.Fee;
                newObj.UserId = user.Id;
                newObj.FromCurrency = Req.FromCurrency;
                newObj.ToCurrency = Req.ToCurrency;
                newObj.TransactionHash = Req.TransactionHash;
                newObj.NotifyUrl = Res.NotifyURL;
                newObj.TransactionId = Req.TransactionId;
                newObj.TransactionCode = Req.TransactionCode;
                newObj.UserGuid = user.Id + "X" + Helpers.GetTimeStamp();
                newObj.Platform = Req.Platform;
                newObj.Type = 1;//buy
                newObj.FromBankId = 0;
                newObj.ToBankId = 0;
                newObj.Code = Req.Code;
                newObj.Address = Req.Address;
                newObj.Status = 0;
                newObj.CreatedBy = user.Id;
                newObj.CreatedDate = Helpers.UTC_To_IST();
                newObj.Remarks = "Pending";

                newObj = _BuySellTopUpRequest.Add(newObj);

                ///2019-10-12
                RootObjectBuyRequest newReq = new RootObjectBuyRequest();
                newReq.coin_address = Req.Address;
                newReq.coin_amount = Req.ToAmount;
                newReq.coin_name = Req.ToCurrency;
                newReq.currency_code = Req.FromCurrency;
                newReq.email = user.Email;
                newReq.notify_url = Res.NotifyURL;
                newReq.coin_amount = Req.ToAmount;
                newReq.phone = user.Mobile;
                newReq.platform = Req.Platform;
                newReq.return_url = tradeObj.CallBackURL;
                newReq.total = Req.FromAmount;
                newReq.transaction_hash = "";
                newReq.transaction_id = newObj.Guid;
                newReq.user_id = newObj.UserGuid;
                ///

                Res.ResponseTag = CryptoJS.OpenSSLEncrypt(Helpers.JsonSerialize(newReq), Key);

                Res.TrnId = newObj.Guid;
                Res.UserId = newObj.UserGuid;
                Res.ReturnMsg = EnResponseMessage.RecordAdded;
                Res.ErrorCode = enErrorCode.Success;
                Res.ReturnCode = enResponseCode.Success;
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public ListGetLTP GetFiatLTP(short? TransactionType)
        {
            ListGetLTP Response = new ListGetLTP();
            try
            {
                var res = _IFiatIntegrateRepository.GetFiatLTP(TransactionType);
                if (res.Count > 0)
                {
                    Response.Data = res;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnCode = enResponseCode.Success;
                }
                else
                {
                    Response.Data = new List<GetLTP>();
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BizResponseClass NotifyDeposit(NotifyDepositReq Request)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                var IsExist = _BuySellTopUpRequest.GetSingle(i => i.Status == 0 && i.Guid == Request.Data);

                //string text = "U2FsdGVkX19KndJZywle26R/dcgeeLi7kZ0Gsum4M0H2a/auBqZgNkTqSY/3N1CER8GRZIBRCap+zQ+h14TzoAAzFinUiTxe6u7yn8GBwJOco72w2VsnMLr/koB730Cjn8MPgHeZMBTX67hKJ+2kVCHINZ6vlKKR/oUYOV5SbS0YNZVpiX2ssevnoUJO9D/x2Vc0bmuDB/TbcreMYuZiv0Y6hCv7aMe06SJpLcAyoi+JHhpaF535lc+JzicbZ9PzwaB+lkTCKCGipLX+5qQeOSbfM9VhgclG2sHDv6DpwbDM+uBW0zC5vMnmo1coxG7zCoEZlFC6MeKe8+1s+Z89MKPlLmL5moq7ELw76fZj/hk6Y5mL5CmKPAe1aPusupLfWv3VV3CwvYVhb+oltOTd0v4viN3uaJVKIVYDZ+LR7LbZZXqv2IJx90AUQd/rAZJnPB+KApIid4EBeHNgmyFrabn71Jf4CsAMH1LzLwuXbxPhEFA7WakfQcai00I+8rleMU8vRkm1NdOfqFAX1muOYclmfbBiGA+CJvuLD7pQlCAfAhr/S440kYXRCJJ2tPWMRtrlwSrG8qxABpaHaaByo7EcEo0IUMVdbFTgXOY2OATgLJSwB8ULo62iMP3B9r1Zw2bhJb0G5JJAf4mXigodSAPwTIcsaoAGjQKpajiUPlbT1mNdjw1h0kS/OP/My5lsRQC5rxjO+hOn3pVystJuKW4E5to7WDaF7O80uPN1SAk=";
                //var mytext = CryptoJS.OpenSSLDecrypt(text, "45fdljiaw54ASDw3vbnmtyiroufgsdlb");

                if (IsExist != null)
                {
                    IsExist.Status = (Request.Status == true ? Convert.ToInt16(1) : Convert.ToInt16(2));
                    IsExist.UpdatedBy = 9999;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    IsExist.Remarks = "Verified by NotifyURL";

                    _BuySellTopUpRequest.UpdateWithAuditLog(IsExist);

                    Response.ReturnMsg = EnResponseMessage.RecordUpdated;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnCode = enResponseCode.Success;
                }
                else
                {
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public ListFiatBuyHistory FiatBuyHistory(string FromCurrency, string ToCurrency, short? Status, string TrnId, string Email, DateTime? FromDate, DateTime? ToDate)
        {
            ListFiatBuyHistory Response = new ListFiatBuyHistory();
            try
            {
                if (FromDate != null && ToDate != null)
                {
                    if (FromDate > ToDate)
                    {
                        Response.ReturnMsg = EnResponseMessage.InvalidFromDate_ToDate;
                        Response.ErrorCode = enErrorCode.InvalidFromDate_ToDate;
                        Response.ReturnCode = enResponseCode.Fail;
                        return Response;
                    }
                }
                var res = _IFiatIntegrateRepository.FiatBuyHistory(FromCurrency, ToCurrency, Status, TrnId, Email, FromDate, ToDate);
                if (res.Count > 0)
                {
                    Response.Data = res;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnCode = enResponseCode.Success;
                }
                else
                {
                    Response.Data = new List<FiatBuyHistory>();
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public ListFiatSellHistory FiatSellHistory(string FromCurrency, string ToCurrency, short? Status, string TrnId, string Email, DateTime? FromDate, DateTime? ToDate)
        {
            ListFiatSellHistory Response = new ListFiatSellHistory();
            try
            {
                if (FromDate != null && ToDate != null)
                {
                    if (FromDate > ToDate)
                    {
                        Response.ReturnMsg = EnResponseMessage.InvalidFromDate_ToDate;
                        Response.ErrorCode = enErrorCode.InvalidFromDate_ToDate;
                        Response.ReturnCode = enResponseCode.Fail;
                        return Response;
                    }
                }
                var res = _IFiatIntegrateRepository.FiatSellHistory(FromCurrency, ToCurrency, Status, TrnId, Email, FromDate, ToDate);
                if (res.Count > 0)
                {
                    Response.Data = res;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnCode = enResponseCode.Success;
                }
                else
                {
                    Response.Data = new List<FiatSellHistory>();
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public ListGetFiatTradeInfo GetFiatTradeInfo()
        {
            ListGetFiatTradeInfo Response = new ListGetFiatTradeInfo();
            try
            {
                var res = _IFiatIntegrateRepository.GetFiatTradeInfo();
                if (res != null)
                {
                    Response.Data = res;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnCode = enResponseCode.Success;
                }
                else
                {
                    Response.Data = new GetFiatTradeInfo();
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BizResponseClass BuyCallBackUpdate(InputBuyCallBackUpdateReq reqInput, long UserId)
        {
            try
            {
                string Key = "";
                var tradeObj = _FiatTradeConfigurationMaster.GetSingle(i => i.Status < 9);
                if (tradeObj != null)
                {
                    Key = tradeObj.EncryptionKey;
                }

                BuyCallBackUpdateReq req = JsonConvert.DeserializeObject<BuyCallBackUpdateReq>(CryptoJS.OpenSSLDecrypt(reqInput.data, Key));

                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, JsonConvert.SerializeObject(req));

                BizResponseClass Response = new BizResponseClass();
                var IsExist = _BuySellTopUpRequest.GetSingle(i => i.Status != 1 && i.Status != 2 && i.Guid == req.transaction_id && i.ToCurrency == req.coin_name && i.Address == req.coin_address && i.ToAmount == req.coin_amount && i.FromAmount == req.total);
                if (IsExist != null)
                {
                    IsExist.BankId = req.bank._id;
                    IsExist.BankName = req.bank.bank_name;
                    IsExist.Code = req.code;
                    IsExist.CurrencyId = req.currency._id;
                    IsExist.CurrencyName = req.currency.currency;
                    IsExist.TransactionId = req._id;
                    IsExist.UpdatedBy = IsExist.UserId;

                    if (req.status == "pending")
                    {
                        IsExist.Status = 6;
                        IsExist.Remarks = "Pending";
                    }
                    else if (req.status == "approved")
                    {
                        IsExist.Status = 1;
                        IsExist.Remarks = "Approved";
                    }
                    else if (req.status == "rejected")
                    {
                        IsExist.Status = 2;
                        IsExist.Remarks = "Rejected";
                    }
                    else
                    {
                        IsExist.Status = 6;
                        IsExist.Remarks = "Pending";
                    }
                    if (!string.IsNullOrEmpty(req.notes))
                    {
                        IsExist.Remarks = req.notes;
                    }
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();

                    _BuySellTopUpRequest.UpdateWithAuditLog(IsExist);

                    HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, JsonConvert.SerializeObject(req) + "Inside");

                    //2019-10-12 sent email to user
                    if (req.status == "pending")
                    {
                        _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_FiatBuyRequest, IsExist.UserId.ToString(), IsExist.FromCurrency, IsExist.ToCurrency, Helpers.DoRoundForTrading(IsExist.ToAmount, 8).ToString(), Helpers.DoRoundForTrading(IsExist.FromAmount, 8).ToString(), IsExist.CoinRate.ToString(), IsExist.Guid.ToString(), IsExist.Fee.ToString(), IsExist.CreatedDate.ToString());
                    }
                    else if ((req.status == "approved") || (req.status == "rejected"))
                    {
                        _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_FiatBuyRequestSuccess, IsExist.UserId.ToString(), IsExist.FromCurrency, IsExist.ToCurrency, Helpers.DoRoundForTrading(IsExist.ToAmount, 8).ToString(), Helpers.DoRoundForTrading(IsExist.FromAmount, 8).ToString(),
                            ((req.status == "approved") ? "Successful" : "Fail"), IsExist.Guid.ToString(), IsExist.Fee.ToString(), IsExist.CreatedDate.ToString());
                    }
                    Response.ReturnMsg = EnResponseMessage.RecordUpdated;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnCode = enResponseCode.Success;
                    return Response;
                }
                Response.ReturnMsg = EnResponseMessage.NotFound;
                Response.ErrorCode = enErrorCode.NotFound;
                Response.ReturnCode = enResponseCode.Fail;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public SellResponse FiatSellTopUpRequest(SellRequest Req, ApplicationUser user)
        {
            SellResponse Res = new SellResponse();
            try
            {
                var IsKYC = _PersonalVerification.GetSingle(i => i.UserID == user.Id && i.VerifyStatus == 1);//success
                if (IsKYC == null)
                {
                    Res.ReturnMsg = "Need to Complete KYC!!";
                    Res.ErrorCode = enErrorCode.NeedtoCmpltKYC;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                var FromWalletTypeObj = _WalletTypeMaster.GetSingle(i => i.Status == 1 && i.WalletTypeName == Req.FromCurrency && i.CurrencyTypeID == 1);//crypto
                if (FromWalletTypeObj == null)
                {
                    Res.ReturnMsg = EnResponseMessage.InvalidCoinName;
                    Res.ErrorCode = enErrorCode.InvalidCoinName;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                var ToWalletTypeObj = _FiatCurrencyMaster.GetSingle(i => i.CurrencyCode == Req.ToCurrency && i.Status == 1);//fait
                if (ToWalletTypeObj == null)
                {
                    Res.ReturnMsg = EnResponseMessage.InvalidCoinName;
                    Res.ErrorCode = enErrorCode.InvalidCoinName;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }

                var BankObj = _UserBankMaster.GetSingle(i => i.Status == 1 && i.GUID == Req.BankId && i.UserId == user.Id);
                if (BankObj == null)
                {
                    Res.ReturnMsg = "Invalid Bank";
                    Res.ErrorCode = enErrorCode.InvalidBank;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                var FromWalletObj = _WalletMaster.GetSingle(i => i.Status == 1 && i.WalletTypeID == FromWalletTypeObj.Id && i.UserID == user.Id && i.IsDefaultWallet == 1);
                if (FromWalletObj == null)
                {
                    Res.ReturnMsg = EnResponseMessage.InvalidWallet;
                    Res.ErrorCode = enErrorCode.InvalidWallet;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                if (FromWalletObj.Balance < Req.FromAmount)
                {
                    Res.ReturnMsg = EnResponseMessage.InsufficantBal;
                    Res.ErrorCode = enErrorCode.InsufficantBal;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                var CoinObj = _FiatCoinConfiguration.GetSingle(i => i.FromCurrencyId == FromWalletTypeObj.Id && i.Status == 1 && i.TransactionType == 2);
                if (CoinObj == null)
                {
                    Res.ReturnMsg = "Coin Configuration data Not Found.";
                    Res.ErrorCode = enErrorCode.CoinConfigurationDataNotFond;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                else
                {
                    if (CoinObj.MinQty > 0 && CoinObj.MinQty > Req.FromAmount)
                    {
                        Res.ReturnMsg = "Min Quantity limit Exceeded";
                        Res.ErrorCode = enErrorCode.MinQtyExccedd;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                    if (CoinObj.MaxQty > 0 && CoinObj.MaxQty < Req.FromAmount)
                    {
                        Res.ReturnMsg = "Max Quantity limit Exceeded";
                        Res.ErrorCode = enErrorCode.MaxQtyExccedd;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                    if (CoinObj.MinAmount > 0 && CoinObj.MinAmount > Req.ToAmount)
                    {
                        Res.ReturnMsg = "Min Amount limit Exceeded";
                        Res.ErrorCode = enErrorCode.MinAmtExccedd;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                    if (CoinObj.MaxAmount > 0 && CoinObj.MaxAmount < Req.ToAmount)
                    {
                        Res.ReturnMsg = "Max Amount limit Exceeded";
                        Res.ErrorCode = enErrorCode.MaxAmtExccedd;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                }

                var Key = "";
                var WithdrawURL = "";
                InternalSellTopUpRequest InternalReq = new InternalSellTopUpRequest();
                var tradeObj = _FiatTradeConfigurationMaster.GetSingle(i => i.Status == 1);
                if (tradeObj != null)
                {
                    //if (tradeObj.MinLimit != 0)
                    //{
                    //    if (tradeObj.MinLimit > Req.FromAmount)
                    //    {
                    //        Res.ReturnMsg = "Cann't Exceed MinLimit";
                    //        Res.ErrorCode = enErrorCode.InvalidMinLimit;
                    //        Res.ReturnCode = enResponseCode.Fail;
                    //        return Res;
                    //    }
                    //}
                    //if (tradeObj.MaxLimit != 0)
                    //{
                    //    if (tradeObj.MaxLimit < Req.FromAmount)
                    //    {
                    //        Res.ReturnMsg = "Cann't Exceed MaxLimit";
                    //        Res.ErrorCode = enErrorCode.InvalidMaxLimit;
                    //        Res.ReturnCode = enResponseCode.Fail;
                    //        return Res;
                    //    }
                    //}
                    InternalReq.notify_url = tradeObj.SellNotifyURL;
                    InternalReq.platform = tradeObj.Platform;
                    InternalReq.return_url = tradeObj.SellCallBackURL;
                    WithdrawURL = tradeObj.WithdrawURL;
                    Key = tradeObj.EncryptionKey;
                }
                else
                {
                    InternalReq.platform = "";
                    InternalReq.notify_url = "";
                    InternalReq.return_url = "";
                }
                SellTopUpRequest newObj = new SellTopUpRequest();
                newObj.Guid = Guid.NewGuid().ToString().Replace("-", "");
                newObj.FromAmount = Req.FromAmount;
                newObj.ToAmount = Req.ToAmount;
                newObj.CoinRate = Req.CoinRate;
                newObj.FiatConverationRate = Req.FiatConverationRate;
                newObj.Fee = Req.Fee;
                newObj.UserId = user.Id;
                newObj.FromCurrency = Req.FromCurrency;
                newObj.ToCurrency = Req.ToCurrency;
                newObj.TransactionHash = "";
                newObj.NotifyUrl = InternalReq.notify_url;
                newObj.TransactionId = "";
                newObj.TransactionCode = "";
                newObj.UserGuid = (user.Id.ToString() + Helpers.GenerateBatch().ToString());
                newObj.Platform = InternalReq.platform;
                newObj.Type = 2;//sell
                newObj.FromBankId = 0;
                newObj.ToBankId = 0;
                newObj.Code = "";
                newObj.Address = "";
                newObj.Status = 0;
                newObj.CreatedBy = user.Id;
                newObj.CreatedDate = Helpers.UTC_To_IST();
                newObj.Remarks = "Pending";
                newObj.user_bank_account_number = BankObj.BankAccountNumber;
                newObj.user_bank_acount_holder_name = BankObj.BankAccountHolderName;
                newObj.user_bank_name = BankObj.BankName;
                newObj.user_currency_code = BankObj.CurrencyCode;

                newObj = _SellTopUpRequest.Add(newObj);

                //request bind
                InternalReq.phone = (user.Mobile == null ? "" : user.Mobile);
                InternalReq.email = user.Email;
                InternalReq.coin_amount = Req.FromAmount;
                InternalReq.coin_name = Req.FromCurrency;
                InternalReq.total = Req.ToAmount;
                InternalReq.transaction_id = newObj.Guid;
                InternalReq.user_bank_account_number = newObj.Guid;
                InternalReq.transaction_id = newObj.Guid;
                InternalReq.user_bank_account_number = BankObj.BankAccountNumber;
                InternalReq.user_bank_acount_holder_name = BankObj.BankAccountHolderName;
                InternalReq.user_bank_name = BankObj.BankName;
                InternalReq.user_currency_code = BankObj.CurrencyCode;

                var length = user.Id.ToString().Length;
                // string AccNuma = "".PadLeft(length - 3, '*');
                string users = (new Random()).Next(10, 100) + user.Id.ToString().PadLeft(3, '0').Substring(user.Id.ToString().PadLeft(3, '0').Length - 3);
                InternalReq.user_id = Convert.ToInt64(users);

                ///
                var RequestTag = CryptoJS.OpenSSLEncrypt(Helpers.JsonSerialize(InternalReq), Key);
                InternalSellTopUpReq newRequestObj = new InternalSellTopUpReq();
                newRequestObj.order = RequestTag;

                string apiResponse = _webApiSendRequest.SendAPIRequestAsync(WithdrawURL, Helpers.JsonSerialize(newRequestObj), "application/json", 180000, new System.Net.WebHeaderCollection(), "POST");
                if (!String.IsNullOrEmpty(apiResponse))
                {
                    var jsonObj = JsonConvert.DeserializeObject<InternalSellTopUpRes>(apiResponse);
                    if (jsonObj == null)
                    {
                        Res.TransactionId = "";
                        Res.Address = "";
                        Res.ReturnMsg = "Getting response Blank";
                        Res.ErrorCode = enErrorCode.ProcessTrn_GettingResponseBlank;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                    if (jsonObj.status == true)
                    {
                        var res = JsonConvert.DeserializeObject<RootObjectInternalSellTopUpRes>(CryptoJS.OpenSSLDecrypt(jsonObj.data, Key));
                        if (res == null)
                        {
                            Res.TransactionId = "";
                            Res.Address = "";
                            Res.ReturnMsg = "Getting response Blank";
                            Res.ErrorCode = enErrorCode.ProcessTrn_GettingResponseBlank;
                            Res.ReturnCode = enResponseCode.Fail;
                            return Res;
                        }
                        newObj.TransactionId = res._id;
                        newObj.Address = res.coin_address;

                        Res.TransactionId = newObj.TransactionId;
                        Res.Address = newObj.Address;
                    }
                    else
                    {
                        Res.TransactionId = "";
                        Res.Address = "";
                        newObj.Status = 2;
                        newObj.Remarks = "Fail";
                    }
                    newObj.UpdatedBy = user.Id;
                    newObj.UpdatedDate = Helpers.UTC_To_IST();
                    _SellTopUpRequest.UpdateWithAuditLog(newObj);

                    Res.TrnId = newObj.Guid;
                    Res.ReturnMsg = EnResponseMessage.RecordAdded;
                    Res.ErrorCode = enErrorCode.Success;
                    Res.ReturnCode = enResponseCode.Success;
                    return Res;
                }
                else
                {
                    Res.TransactionId = "";
                    Res.Address = "";
                    Res.ReturnMsg = "Getting response Blank";
                    Res.ErrorCode = enErrorCode.ProcessTrn_GettingResponseBlank;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> SellCallBackUpdate(InputBuyCallBackUpdateReq request, int id)
        {
            try
            {
                string Key = "";
                var tradeObj = _FiatTradeConfigurationMaster.GetSingle(i => i.Status < 9);
                if (tradeObj != null)
                {
                    Key = tradeObj.EncryptionKey;
                }

                SellCallBackUpdateReq req = JsonConvert.DeserializeObject<SellCallBackUpdateReq>(CryptoJS.OpenSSLDecrypt(request.data, Key));

                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, JsonConvert.SerializeObject(req));
                var InsertIsExistObj = _SellTopUpRequest.GetSingle(i => i.Guid == req.transaction_id);

                //2019-11-8 add log
                long TrnReqId = 0;

                InsertIntoTransactionRequyest requestInsert = new InsertIntoTransactionRequyest();
                requestInsert.TrnNo = (InsertIsExistObj == null) ? 0 : InsertIsExistObj.TrnNo;
                requestInsert.RequestBody = "Url:" + "SellCallback" + "####ReQuest:" +
                    Helpers.JsonSerialize(req);
                TrnReqId = _withdrawTransactionV1.InsertIntoTransactionRequest(requestInsert);

                ///i.Status != 3 && i.Status != 2 && i.Status != 8 && i.Status != 9 &&
                BizResponseClass Response = new BizResponseClass();
                var IsExist = _SellTopUpRequest.GetSingle(i => i.Guid == req.transaction_id && i.FromCurrency == req.coin_name && i.FromAmount == req.coin_amount && i.ToAmount == req.total);//&& i.Address == req.coin_address
                if (IsExist != null)
                {
                    if (IsExist.Status == 3 || IsExist.Status == 2 || IsExist.Status == 8 || IsExist.Status == 9 || IsExist.Status == 4 || IsExist.Status == 7)
                    {
                        Response.ReturnMsg = EnResponseMessage.NotFound;
                        Response.ErrorCode = enErrorCode.NotFound;
                        Response.ReturnCode = enResponseCode.Fail;
                        _withdrawTransactionV1.UpdateIntoTransactionRequest(TrnReqId, Helpers.JsonSerialize(Response));
                        return Response;
                    }
                    if (!string.IsNullOrEmpty(IsExist.Address))
                    {
                        if (IsExist.Address != req.coin_address)
                        {
                            Response.ReturnMsg = EnResponseMessage.NotFound;
                            Response.ErrorCode = enErrorCode.NotFound;
                            Response.ReturnCode = enResponseCode.Fail;
                            _withdrawTransactionV1.UpdateIntoTransactionRequest(TrnReqId, Helpers.JsonSerialize(Response));
                            return Response;
                        }
                    }
                    if (!string.IsNullOrEmpty(IsExist.TransactionId))
                    {
                        if (IsExist.TransactionId != req._id)
                        {
                            Response.ReturnMsg = EnResponseMessage.NotFound;
                            Response.ErrorCode = enErrorCode.NotFound;
                            Response.ReturnCode = enResponseCode.Fail;
                            _withdrawTransactionV1.UpdateIntoTransactionRequest(TrnReqId, Helpers.JsonSerialize(Response));
                            return Response;
                        }
                    }

                    //IsExist.Platform = req.platform;
                    //IsExist.user_bank_acount_holder_name = req.user_bank_account_number;
                    //IsExist.user_bank_account_number = req.user_bank_name;
                    //IsExist.user_bank_name = req.user_bank_name;
                    //IsExist.user_currency_code = req.user_currency_code;
                    //IsExist.NotifyUrl = req.notify_url;
                    IsExist.Address = req.coin_address;
                    IsExist.TransactionId = req._id;
                    IsExist.payus_transaction_id = req.payus_transaction_id;
                    //IsExist.payus_amount_usd = req.payus_amount_usd;
                    IsExist.payus_amount_crypto = req.payus_amount_crypto;
                    IsExist.payus_mining_fees = req.payus_mining_fees;
                    IsExist.payus_fees = req.payus_fees;
                    //IsExist.payus_total_payable_amount = req.payus_total_payable_amount;
                    IsExist.payus_total_fees = req.payus_total_fees;
                    IsExist.payus_usd_rate = req.payus_usd_rate;
                    IsExist.payus_expire_datetime = req.payus_expire_datetime;
                    IsExist.payus_payment_tracking = req.payus_payment_tracking;

                    //IsExist.Address = req.coin_address;

                    IsExist.Code = "";
                    IsExist.CurrencyId = "";
                    IsExist.CurrencyName = req.coin_name;
                    //IsExist.TransactionId = req._id;

                    if (!string.IsNullOrEmpty(req.notes))
                    {
                        IsExist.Remarks = req.notes;
                    }

                    IsExist.UpdatedBy = IsExist.UserId;

                    var tranTQobj = _TransactionQueue.GetSingle(x => x.Id == IsExist.TrnNo);
                    //2019-11-6 changes related status update
                    if (req.status == "approved" && IsExist.Status == 0)
                    {
                        IsExist.Status = 6;
                    }
                    else if (req.status == "completed" || req.status == "complete")
                    {
                        IsExist.Status = 8;
                    }
                    else if (req.status == "rejected" && IsExist.Status != 6)
                    {
                        if (string.IsNullOrEmpty(IsExist.TransactionHash) && IsExist.Status == 0)
                        {
                            if (tranTQobj != null && tranTQobj.Status == 4)
                            {
                                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, "Recon start");
                                var ReconResponse = _withdrawTransactionV1.MarkTransactionOperatorFail("Operator Fail", enErrorCode.ProcessTrn_OprFails, tranTQobj).GetAwaiter();
                                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, "Before" + "Recon");
                            }
                        }
                        IsExist.Status = 4;//rejected
                    }
                    else if (req.status == "expired" && IsExist.Status != 6)
                    {
                        if (string.IsNullOrEmpty(IsExist.TransactionHash) && (IsExist.Status == 6 || IsExist.Status == 0))
                        {
                            if (tranTQobj != null && tranTQobj.Status == 4 && IsExist.APIStatus != 2)
                            {
                                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, "Recon start");
                                var ReconResponse = _withdrawTransactionV1.MarkTransactionOperatorFail("expired", enErrorCode.ProcessTrn_Oprexpired, tranTQobj).GetAwaiter();
                                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, "Before" + "Recon");
                            }
                        }
                        IsExist.Status = 9;//expired
                    }
                    //else
                    //{
                    //    IsExist.Status = 6;
                    //}

                    //if (req.status == "pending")
                    //{
                    //    IsExist.Status = 6;

                    //}
                    //else if (req.status == "approved")
                    //{
                    //    IsExist.Status = 1;
                    //}
                    //else if (req.status == "rejected")
                    //{
                    //    IsExist.Status = 2;
                    //}
                    //else
                    //{
                    //    IsExist.Status = 6;
                    //}
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();

                    _SellTopUpRequest.UpdateWithAuditLog(IsExist);
                    HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, JsonConvert.SerializeObject(req) + "Inside");

                    //2019-10-12 sent email to user
                    //if (req.status == "pending")
                    //{
                    //    await _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_FiatSellRequest, IsExist.UserId.ToString(), IsExist.FromCurrency, IsExist.ToCurrency, Helpers.DoRoundForTrading(IsExist.ToAmount, 8).ToString(), Helpers.DoRoundForTrading(IsExist.FromAmount, 8).ToString(), IsExist.CoinRate.ToString(), IsExist.Guid.ToString(), IsExist.Fee.ToString(), IsExist.CreatedDate.ToString());
                    //}
                    //else 
                    //2019-11-6 changes related status update
                    if ((req.status == "completed") || (req.status == "rejected") || (req.status == "expired") || (req.status == "complete"))
                    {
                        await _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_FiatSellRequestSuccess, IsExist.UserId.ToString(), IsExist.FromCurrency, IsExist.ToCurrency, Helpers.DoRoundForTrading(IsExist.ToAmount, 8).ToString(), Helpers.DoRoundForTrading(IsExist.FromAmount, 8).ToString(), ((req.status == "complete" || req.status == "completed") ? "completed" : (req.status == "rejected") ? "rejected" : "expired"), IsExist.Guid.ToString(), IsExist.Fee.ToString(), IsExist.CreatedDate.ToString());
                    }
                    Response.ReturnMsg = EnResponseMessage.RecordUpdated;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnCode = enResponseCode.Success;
                    _withdrawTransactionV1.UpdateIntoTransactionRequest(TrnReqId, Helpers.JsonSerialize(Response));

                    return Response;
                }
                Response.ReturnMsg = EnResponseMessage.NotFound;
                Response.ErrorCode = enErrorCode.NotFound;
                Response.ReturnCode = enResponseCode.Fail;
                _withdrawTransactionV1.UpdateIntoTransactionRequest(TrnReqId, Helpers.JsonSerialize(Response));

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BizResponseClass UpdateTransactionHash(string Guid, string TransactionHash, long UserId)
        {
            BizResponseClass Response = new BizResponseClass();
            string Url = "";
            try
            {
                var SellTopUpReqModel = _SellTopUpRequest.FindBy(e => e.Guid.Equals(Guid)).FirstOrDefault();
                if (SellTopUpReqModel == null)
                {
                    Response.ReturnMsg = "Fiat InvalidID Record Not Found";
                    Response.ErrorCode = enErrorCode.FiatInvalidIDRecordNotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    return Response;
                }
                var CheckExistHash = _SellTopUpRequest.FindBy(e => e.TransactionHash.Equals(TransactionHash)).FirstOrDefault();
                if (CheckExistHash != null)
                {
                    Response.ReturnMsg = "Fiat Duplicate Transaction Has";
                    Response.ErrorCode = enErrorCode.FiatDuplicateTransactionHash;
                    Response.ReturnCode = enResponseCode.Fail;
                    return Response;
                }
                if (SellTopUpReqModel.TransactionId.Equals(null) || SellTopUpReqModel.TransactionId.Equals(""))
                {
                    Response.ReturnMsg = "TransactionID Not Found";
                    Response.ErrorCode = enErrorCode.FiatTransactionIDNotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    return Response;
                }
                var IsExistWithdrwHistory = _WithdrawHistory.FindBy(e => e.UserId == UserId && e.TrnID == TransactionHash && e.Address == SellTopUpReqModel.Address).FirstOrDefault();
                if (IsExistWithdrwHistory == null)
                {
                    Response.ReturnMsg = "Withdraw Transaction not found for this user";
                    Response.ErrorCode = enErrorCode.FiatWithdrawTransactionNotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    return Response;
                }
                if (!string.IsNullOrEmpty(SellTopUpReqModel.TransactionHash))
                {
                    Response.ReturnMsg = "Transaction Hash Alreay Exist";
                    Response.ErrorCode = enErrorCode.FiatTransactionHashAlredyExist;
                    Response.ReturnCode = enResponseCode.Fail;
                    return Response;
                }
                var FiatTradeConfiguration = _FiatTradeConfigurationMaster.FindBy(e => e.Status == 1).FirstOrDefault();
                if (FiatTradeConfiguration == null)
                {
                    Response.ReturnMsg = "Fiat Trade Configuration not Found";
                    Response.ErrorCode = enErrorCode.FiatTradeConfigurationNotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    return Response;
                }
                Url = FiatTradeConfiguration.WithdrawURL + "/" + SellTopUpReqModel.TransactionId;

                UpdateTransactionHashViewModel Request = new UpdateTransactionHashViewModel();
                Request.coin_address = SellTopUpReqModel.Address;
                Request.transaction_hash = TransactionHash;


                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, Url + "TrnNo::" + Guid + "::Request::" + Request);
                var AESEncryptedData = CryptoJS.OpenSSLEncrypt(Helpers.JsonSerialize(Request), FiatTradeConfiguration.EncryptionKey);
                var EncrypetdReq = "{\"order\":\"" + AESEncryptedData + "\"}";
                string APIResponse = _webApiSendRequest.SendAPIRequestAsync(Url, EncrypetdReq, "application/json", 180000, new System.Net.WebHeaderCollection(), "POST");

                //make web api call
                if (APIResponse != "")
                {
                    TransactionHasAPIResponse ParceResponse = JsonConvert.DeserializeObject<TransactionHasAPIResponse>(APIResponse);

                    if (ParceResponse.status == "true")
                    {
                        var mytext = CryptoJS.OpenSSLDecrypt(ParceResponse.data, FiatTradeConfiguration.EncryptionKey);
                        HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, Url + "TrnNo::" + Guid + "::mytext::" + mytext);
                        SellTopUpReqModel.TransactionHash = TransactionHash;
                        SellTopUpReqModel.Status = 1;

                        SellTopUpReqModel.UpdatedBy = UserId;
                        SellTopUpReqModel.UpdatedDate = DateTime.UtcNow;
                        SellTopUpReqModel.Remarks = "Success";
                        _SellTopUpRequest.Update(SellTopUpReqModel);
                        Response.ReturnMsg = "Success";
                        Response.ErrorCode = enErrorCode.Success;
                        Response.ReturnCode = enResponseCode.Success;
                        return Response;
                    }
                }
                SellTopUpReqModel.Remarks = APIResponse;
                _SellTopUpRequest.Update(SellTopUpReqModel);
                Response.ReturnMsg = "Update fail";
                Response.ErrorCode = enErrorCode.FiatTransactionHashUpdateFail;
                Response.ReturnCode = enResponseCode.Fail;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public ListFiatCurrencyInfo GetFiatCurrencyInfo()
        {
            ListFiatCurrencyInfo Response = new ListFiatCurrencyInfo();
            try
            {
                var res = _IFiatIntegrateRepository.GetFiatCurrencyInfo();
                if (res.Count != 0)
                {
                    Response.Data = res;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnCode = enResponseCode.Success;
                }
                else
                {
                    Response.Data = new List<GetFiatCurrencyInfo>();
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public ListFiatCurrencyInfo GetFiatCurrencyInfoBO(short? Status)
        {
            ListFiatCurrencyInfo Response = new ListFiatCurrencyInfo();
            try
            {
                var res = _IFiatIntegrateRepository.GetFiatCurrencyInfoBO(Status);
                if (res.Count != 0)
                {
                    Response.Data = res;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnCode = enResponseCode.Success;
                }
                else
                {
                    Response.Data = new List<GetFiatCurrencyInfo>();
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public SellResponseV2 FiatSellTopUpRequestV1(SellRequest Req, ApplicationUser user)
        {
            SellResponseV2 Res = new SellResponseV2();
            try
            {
                var IsKYC = _PersonalVerification.GetSingle(i => i.UserID == user.Id && i.VerifyStatus == 1);//success
                if (IsKYC == null)
                {
                    Res.ReturnMsg = "Need to Complete KYC!!";
                    Res.ErrorCode = enErrorCode.NeedtoCmpltKYC;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                if (string.IsNullOrEmpty(user.Mobile))
                {
                    Res.ReturnMsg = "Mobile is Required";
                    Res.ReturnCode = enResponseCode.Fail;
                    Res.ErrorCode = enErrorCode.MobileIsRequired;
                    return Res;
                }
                var FromWalletTypeObj = _WalletTypeMaster.GetSingle(i => i.Status == 1 && i.WalletTypeName == Req.FromCurrency && i.CurrencyTypeID == 1);//crypto
                if (FromWalletTypeObj == null)
                {
                    Res.ReturnMsg = EnResponseMessage.InvalidCoinName;
                    Res.ErrorCode = enErrorCode.InvalidCoinName;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                var ToWalletTypeObj = _FiatCurrencyMaster.GetSingle(i => i.CurrencyCode == Req.ToCurrency && i.Status == 1);//fait
                if (ToWalletTypeObj == null)
                {
                    Res.ReturnMsg = EnResponseMessage.InvalidCoinName;
                    Res.ErrorCode = enErrorCode.InvalidCoinName;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }

                var BankObj = _UserBankMaster.GetSingle(i => i.Status == 1 && i.GUID == Req.BankId && i.UserId == user.Id);
                if (BankObj == null)
                {
                    Res.ReturnMsg = "Invalid Bank";
                    Res.ErrorCode = enErrorCode.InvalidBank;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                var FromWalletObj = _WalletMaster.GetSingle(i => i.Status == 1 && i.WalletTypeID == FromWalletTypeObj.Id && i.UserID == user.Id && i.IsDefaultWallet == 1);
                if (FromWalletObj == null)
                {
                    Res.ReturnMsg = EnResponseMessage.InvalidWallet;
                    Res.ErrorCode = enErrorCode.InvalidWallet;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                if (FromWalletObj.Balance < Req.FromAmount)
                {
                    Res.ReturnMsg = EnResponseMessage.InsufficantBal;
                    Res.ErrorCode = enErrorCode.InsufficantBal;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                var serviceObj = _ServiceMaster.GetSingle(i => i.WalletTypeID == FromWalletTypeObj.Id);
                if (serviceObj != null && serviceObj.IsIntAmountAllow == 1) //IsIntAmountAllow = 1 ,only int value is allow not decimal
                {
                    decimal d = (decimal)Req.FromAmount;
                    if ((d % 1) > 0)
                    {
                        //is decimal
                        Res.ReturnMsg = EnResponseMessage.CreateTrnInvalidIntAmountMsg;
                        Res.ErrorCode = enErrorCode.CreateTrnInvalidIntAmountMsg;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                }
                var CoinObj = _FiatCoinConfiguration.GetSingle(i => i.FromCurrencyId == FromWalletTypeObj.Id && i.Status == 1 && i.TransactionType == 2);
                if (CoinObj == null)
                {
                    Res.ReturnMsg = "Coin Configuration data Not Found.";
                    Res.ErrorCode = enErrorCode.CoinConfigurationDataNotFond;
                    Res.ReturnCode = enResponseCode.Fail;
                    return Res;
                }
                else
                {
                    if (CoinObj.MinQty > 0 && CoinObj.MinQty > Req.FromAmount)
                    {
                        Res.ReturnMsg = "Min Quantity limit Exceeded";
                        Res.ErrorCode = enErrorCode.MinQtyExccedd;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                    if (CoinObj.MaxQty > 0 && CoinObj.MaxQty < Req.FromAmount)
                    {
                        Res.ReturnMsg = "Max Quantity limit Exceeded";
                        Res.ErrorCode = enErrorCode.MaxQtyExccedd;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                    if (CoinObj.MinAmount > 0 && CoinObj.MinAmount > Req.ToAmount)
                    {
                        Res.ReturnMsg = "Min Amount limit Exceeded";
                        Res.ErrorCode = enErrorCode.MinAmtExccedd;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                    if (CoinObj.MaxAmount > 0 && CoinObj.MaxAmount < Req.ToAmount)
                    {
                        Res.ReturnMsg = "Max Amount limit Exceeded";
                        Res.ErrorCode = enErrorCode.MaxAmtExccedd;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }
                }

                var Key = "";
                var WithdrawURL = "";
                InternalSellTopUpRequest InternalReq = new InternalSellTopUpRequest();
                var tradeObj = _FiatTradeConfigurationMaster.GetSingle(i => i.Status == 1);
                if (tradeObj != null)
                {
                    InternalReq.notify_url = tradeObj.SellNotifyURL;
                    InternalReq.platform = tradeObj.Platform;
                    InternalReq.return_url = tradeObj.SellCallBackURL;
                    WithdrawURL = tradeObj.WithdrawURL;
                    Key = tradeObj.EncryptionKey;
                }
                else
                {
                    InternalReq.platform = "";
                    InternalReq.notify_url = "";
                    InternalReq.return_url = "";
                }
                SellTopUpRequest newObj = new SellTopUpRequest();
                newObj.Guid = Guid.NewGuid().ToString().Replace("-", "");
                newObj.FromAmount = Req.FromAmount;
                newObj.ToAmount = Req.ToAmount;
                newObj.CoinRate = Req.CoinRate;
                newObj.FiatConverationRate = Req.FiatConverationRate;
                newObj.Fee = Req.Fee;
                newObj.UserId = user.Id;
                newObj.FromCurrency = Req.FromCurrency;
                newObj.ToCurrency = Req.ToCurrency;
                newObj.TransactionHash = "";
                newObj.NotifyUrl = InternalReq.notify_url;
                newObj.TransactionId = "";
                newObj.TransactionCode = "";
                newObj.UserGuid = (user.Id.ToString() + Helpers.GenerateBatch().ToString());
                newObj.Platform = InternalReq.platform;
                newObj.Type = 2;//sell
                newObj.FromBankId = 0;
                newObj.ToBankId = 0;
                newObj.Code = "";
                newObj.Address = "";
                newObj.Status = 0;
                newObj.CreatedBy = user.Id;
                newObj.CreatedDate = Helpers.UTC_To_IST();
                newObj.Remarks = "Initial";
                newObj.user_bank_account_number = BankObj.BankAccountNumber;
                newObj.user_bank_acount_holder_name = BankObj.BankAccountHolderName;
                newObj.user_bank_name = BankObj.BankName;
                newObj.user_currency_code = BankObj.CurrencyCode;
                newObj.APIStatus = 0;
                newObj.TrnNo = 0;
                newObj = _SellTopUpRequest.Add(newObj);

                NewWithdrawRequestCls newWithdrawRequest = new NewWithdrawRequestCls();
                newWithdrawRequest.accessToken = "";
                newWithdrawRequest.AdditionalInfo = "";
                newWithdrawRequest.AddressLabel = "";
                newWithdrawRequest.Amount = Req.FromAmount;
                newWithdrawRequest.DebitAccountID = FromWalletObj.AccWalletID;
                newWithdrawRequest.DebitWalletID = FromWalletObj.Id;
                newWithdrawRequest.GUID = Guid.NewGuid();
                newWithdrawRequest.IsInternalTrn = 2;
                newWithdrawRequest.MemberID = FromWalletObj.UserID;
                newWithdrawRequest.MemberMobile = user.Mobile;
                newWithdrawRequest.ServiceType = enServiceType.Trading;
                newWithdrawRequest.SMSCode = FromWalletTypeObj.WalletTypeName;
                newWithdrawRequest.Status = 0;
                newWithdrawRequest.StatusCode = 0;
                newWithdrawRequest.StatusMsg = "Initial";
                newWithdrawRequest.TransactionAccount = "N/A";
                newWithdrawRequest.TrnMode = 1;
                newWithdrawRequest.TrnNo = 0;
                newWithdrawRequest.TrnRefNo = newObj.Guid;
                newWithdrawRequest.TrnType = enTrnType.Withdraw;
                newWithdrawRequest.WalletTrnType = enWalletTrnType.Withdrawal;
                newWithdrawRequest.WhitelistingBit = enWhiteListingBit.ON;
                var withdrawTrnRes = _withdrawTransactionV1.FiatWithdrawTransactionTransactionAsync(newWithdrawRequest).GetAwaiter().GetResult();
                if (withdrawTrnRes.ReturnCode != enResponseCodeService.Success)
                {
                    newObj.Status = 3;
                    newObj.Remarks = withdrawTrnRes.ReturnMsg;
                    _SellTopUpRequest.UpdateWithAuditLog(newObj);
                }
                if (withdrawTrnRes.ReturnCode == enResponseCodeService.Success)
                {
                    var url = _configuration["FiatWithdrawConfirmUrl"].ToString();

                    var Param6 = url + "?RefNo=" + newObj.Guid + "&Bit=1"; //Bit = 1 Accept
                    var Param8 = url + "?RefNo=" + newObj.Guid + "&Bit=2";  //Bit = 2 Reject

                    _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_FiatSellRequest, newObj.UserId.ToString(), newObj.FromCurrency, newObj.ToCurrency, Helpers.DoRoundForTrading(newObj.ToAmount, 8).ToString(), Helpers.DoRoundForTrading(newObj.FromAmount, 8).ToString(), Param6, newObj.Guid.ToString(), Param8, newObj.CreatedDate.ToString());
                }

                Res.TrnId = newObj.Guid;
                Res.ReturnMsg = EnResponseMessage.RecordAdded;
                Res.ErrorCode = enErrorCode.Success;
                Res.ReturnCode = enResponseCode.Success;
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public GetWithdrawalTransactionResponse FiatSellRequestConfirmation(FiatSellConfirmReq Req, ApplicationUser user)
        {
            GetWithdrawalTransactionResponse Res = new GetWithdrawalTransactionResponse();
            //Res.TransactionId = "";
            //Res.Address = "";
            try
            {
                if (Req.TransactionBit != 1 && Req.TransactionBit != 2)
                {
                    Res.ReturnMsg = "Invalid Transaction Bit Value";
                    Res.ReturnCode = enResponseCode.Fail;
                    Res.ErrorCode = enErrorCode.Withdrwal_InvalidTransactionBitValue;

                    return Res;
                }

                var IsExist = _SellTopUpRequest.GetSingle(i => i.Guid == Req.TrnId && i.APIStatus == 0 && i.UserId == user.Id);
                if (IsExist != null)
                {
                    //update api status=9
                    IsExist.APIStatus = 9;
                    _SellTopUpRequest.UpdateWithAuditLog(IsExist);
                    var tranTQobj = _TransactionQueue.GetSingle(x => x.Id == IsExist.TrnNo);
                    if (tranTQobj == null)
                    {
                        //Res.TransactionId = "";
                        //Res.Address = "";
                        Res.ReturnMsg = "Request data Not Found";
                        Res.ErrorCode = enErrorCode.NotFound;
                        Res.ReturnCode = enResponseCode.Fail;
                        return Res;
                    }

                    if (Req.TransactionBit == 2) //Reject Transaction Process
                    {
                        WithdrawalConfirmationRequest Request = new WithdrawalConfirmationRequest();
                        Request.RefNo = tranTQobj.GUID.ToString();
                        Request.TransactionBit = 2;
                        var response = _withdrawTransactionV1.WithdrawTransactionAPICallProcessAsync(Request, user.Id, 0).GetAwaiter().GetResult();
                        HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name + "Recon::: ", this.GetType().Name, JsonConvert.SerializeObject(response));
                        if (response.ReturnCode != enResponseCodeService.Success)
                        {
                            IsExist.Status = 3;
                            IsExist.Remarks = response.ReturnMsg;
                            Res.ReturnMsg = response.ReturnMsg;
                            Res.ErrorCode = response.ErrorCode;
                            Res.ReturnCode = enResponseCode.Fail;
                            _SellTopUpRequest.UpdateWithAuditLog(IsExist);
                            var withdrareses1 = _frontTrnService.GetWithdrawalTransaction(tranTQobj.GUID.ToString());
                            withdrareses1.FinalAmount = withdrareses1.Amount;
                            return Res;
                        }
                        tranTQobj.IsVerified = 9;
                        //tranTQobj.TransactionAccount = res.coin_address;
                        _TransactionQueue.UpdateWithAuditLog(tranTQobj);
                        IsExist.Status = 5;//refund
                        IsExist.Remarks = "Refund Success";
                        _SellTopUpRequest.UpdateWithAuditLog(IsExist);
                        var withdrareses = _frontTrnService.GetWithdrawalTransaction(tranTQobj.GUID.ToString());
                        withdrareses.FinalAmount = withdrareses.Amount;
                        Res.Response = withdrareses;
                        Res.ReturnMsg = response.ReturnMsg;
                        Res.ErrorCode = response.ErrorCode;
                        Res.ReturnCode = enResponseCode.Success;
                        //return Res;
                    }
                    else if (Req.TransactionBit == 1)
                    {
                        tranTQobj.IsVerified = 1;
                        //tranTQobj.TransactionAccount = res.coin_address;
                        _TransactionQueue.UpdateWithAuditLog(tranTQobj);

                        var Key = "";
                        var WithdrawURL = "";
                        var BankObj = _UserBankMaster.GetSingle(i => i.Status == 1 && i.BankAccountNumber == IsExist.user_bank_account_number && i.UserId == user.Id);
                        if (BankObj == null)
                        {
                            Res.ReturnMsg = "Invalid Bank";
                            Res.ErrorCode = enErrorCode.InvalidBank;
                            Res.ReturnCode = enResponseCode.Fail;
                            return Res;
                        }
                        InternalSellTopUpRequest InternalReq = new InternalSellTopUpRequest();
                        var tradeObj = _FiatTradeConfigurationMaster.GetSingle(i => i.Status == 1);
                        if (tradeObj != null)
                        {
                            InternalReq.notify_url = tradeObj.SellNotifyURL;
                            InternalReq.platform = tradeObj.Platform;
                            InternalReq.return_url = tradeObj.SellCallBackURL;
                            WithdrawURL = tradeObj.WithdrawURL;
                            Key = tradeObj.EncryptionKey;
                        }
                        else
                        {
                            InternalReq.platform = "";
                            InternalReq.notify_url = "";
                            InternalReq.return_url = "";
                        }

                        //request bind
                        InternalReq.phone = (user.Mobile == null ? "" : user.Mobile);
                        InternalReq.email = user.Email;
                        InternalReq.coin_amount = IsExist.FromAmount;
                        InternalReq.coin_name = IsExist.FromCurrency;
                        InternalReq.total = IsExist.ToAmount;
                        InternalReq.transaction_id = IsExist.Guid;
                        InternalReq.user_bank_account_number = IsExist.Guid;
                        InternalReq.transaction_id = IsExist.Guid;
                        InternalReq.user_bank_account_number = BankObj.BankAccountNumber;
                        InternalReq.user_bank_acount_holder_name = BankObj.BankAccountHolderName;
                        InternalReq.user_bank_name = BankObj.BankName;
                        InternalReq.user_currency_code = BankObj.CurrencyCode;

                        var length = user.Id.ToString().Length;
                        // string AccNuma = "".PadLeft(length - 3, '*');
                        string users = (new Random()).Next(10, 100) + user.Id.ToString().PadLeft(3, '0').Substring(user.Id.ToString().PadLeft(3, '0').Length - 3);
                        InternalReq.user_id = Convert.ToInt64(users);

                        ///
                        var RequestTag = CryptoJS.OpenSSLEncrypt(Helpers.JsonSerialize(InternalReq), Key);
                        InternalSellTopUpReq newRequestObj = new InternalSellTopUpReq();
                        newRequestObj.order = RequestTag;
                        //2019-11-8 add log
                        long TrnReqId = 0;

                        InsertIntoTransactionRequyest requestInsert = new InsertIntoTransactionRequyest();
                        requestInsert.TrnNo = tranTQobj.Id;
                        requestInsert.RequestBody = "Url:" + WithdrawURL + "####ReQuest:" + Helpers.JsonSerialize(newRequestObj);
                        TrnReqId = _withdrawTransactionV1.InsertIntoTransactionRequest(requestInsert);

                        string apiResponse = _webApiSendRequest.SendAPIRequestAsync(WithdrawURL, Helpers.JsonSerialize(newRequestObj), "application/json", 180000, new System.Net.WebHeaderCollection(), "POST");

                        if (!String.IsNullOrEmpty(apiResponse))
                        {
                            var jsonObj = JsonConvert.DeserializeObject<InternalSellTopUpRes>(apiResponse);
                            //2019-11-8 add log
                            _withdrawTransactionV1.UpdateIntoTransactionRequest(TrnReqId, Helpers.JsonSerialize(jsonObj));

                            if (jsonObj == null)
                            {
                                //Res.TransactionId = "";
                                //Res.Address = "";
                                Res.ReturnMsg = "Getting response Blank";
                                Res.ErrorCode = enErrorCode.ProcessTrn_GettingResponseBlank;
                                Res.ReturnCode = enResponseCode.Fail;
                                return Res;
                            }
                            if (jsonObj.status == true)
                            {
                                var res = JsonConvert.DeserializeObject<RootObjectInternalSellTopUpRes>(CryptoJS.OpenSSLDecrypt(jsonObj.data, Key));
                                if (res == null)
                                {
                                    //Res.TransactionId = "";
                                    //Res.Address = "";
                                    Res.ReturnMsg = "Getting response Blank";
                                    Res.ErrorCode = enErrorCode.ProcessTrn_GettingResponseBlank;
                                    Res.ReturnCode = enResponseCode.Fail;
                                    return Res;
                                }
                                IsExist.TransactionId = res._id;
                                IsExist.Address = res.coin_address;
                                IsExist.APIStatus = 1;
                                tranTQobj.IsVerified = 1;
                                tranTQobj.TransactionAccount = res.coin_address;
                                _TransactionQueue.UpdateWithAuditLog(tranTQobj);
                                //Res.TransactionId = IsExist.TransactionId;
                                //Res.Address = IsExist.Address;
                            }
                            else
                            {
                                //Res.TransactionId = "";
                                //Res.Address = "";
                                ///Refund
                                //WithdrawalReconRequest withdrawalReconRequest = new WithdrawalReconRequest();
                                //withdrawalReconRequest.ActionType = enWithdrawalReconActionType.Refund;
                                //withdrawalReconRequest.ActionMessage = "Withdrwal Transaction Request Cancel By User";
                                //withdrawalReconRequest.TrnNo = IsExist.TrnNo;

                                var ReconResponse = _withdrawTransactionV1.MarkTransactionOperatorFail("Operator Fail", enErrorCode.ProcessTrn_OprFails, tranTQobj);
                                RemarksClassObj Remarkstag = new RemarksClassObj();
                                IsExist.Status = 2;
                                try
                                {
                                    Remarkstag = JsonConvert.DeserializeObject<RemarksClassObj>(CryptoJS.OpenSSLDecrypt(jsonObj.data, Key));
                                }
                                catch (Exception exx)
                                {
                                    Remarkstag.message = "Operator Fail";
                                }
                                IsExist.Remarks = (Remarkstag.message != null) ? Remarkstag.message : "Operator Fail";
                            }
                            IsExist.UpdatedBy = user.Id;
                            IsExist.UpdatedDate = Helpers.UTC_To_IST();
                            _SellTopUpRequest.UpdateWithAuditLog(IsExist);

                            // Res.TrnId = IsExist.Guid;
                            Res.ReturnMsg = "Withdrawal transaction has been confirmed.";
                            Res.ErrorCode = enErrorCode.Withdrwal_TransactionConfirmSuccess;
                            Res.ReturnCode = enResponseCode.Success;
                            //return Res;
                        }
                        else
                        {
                            //Res.TransactionId = "";
                            //Res.Address = "";
                            Res.ReturnMsg = "Getting response Blank";
                            Res.ErrorCode = enErrorCode.ProcessTrn_GettingResponseBlank;
                            Res.ReturnCode = enResponseCode.Fail;
                            //return Res;
                        }
                    }
                    else
                    {
                        Res.ReturnMsg = "Invalid Transaction Bit Value";
                        Res.ReturnCode = enResponseCode.Fail;
                        Res.ErrorCode = enErrorCode.Withdrwal_InvalidTransactionBitValue;
                        //return Res;
                    }
                    var withdrares = _frontTrnService.GetWithdrawalTransaction(tranTQobj.GUID.ToString());
                    withdrares.FinalAmount = withdrares.Amount;
                    Res.Response = withdrares;
                    return Res;
                }
                else
                {
                    //    Res.TransactionId = "";
                    //    Res.Address = "";
                    Res.ReturnMsg = "Transaction Already Verified";
                    Res.ReturnCode = enResponseCode.Fail;
                    Res.ErrorCode = enErrorCode.Withdrwal_TransactionAlreadyVerified;
                    return Res;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //komal 1 Nov 2019 1:40 for fiatSellWithdraw
        public async Task FiatSellWithdraw()
        {
            BizResponseClass Response = new BizResponseClass();
            string TrnNo = "";
            try
            {
                var list = _IFiatIntegrateRepository.GetSellWithdrawTrxn();
                if (list.Count > 0)
                {
                    foreach (var WithdrawTrnObj in list)
                    {
                        long tmpUserid = 0;
                        TrnNo = WithdrawTrnObj.WithdrawRefNo.ToString();
                        WithdrawalConfirmationRequest Request = new WithdrawalConfirmationRequest() { RefNo = WithdrawTrnObj.RefNo.ToString(), TransactionBit = 1 };
                        var response = await _withdrawTransactionV1.FiatWithdrawTransactionAPICallProcessAsync(Request, WithdrawTrnObj.Memberid, 1);
                        HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name + "nupoora test", this.GetType().Name, JsonConvert.SerializeObject(response) + "##Request:" + JsonConvert.SerializeObject(Request));

                        if (response.ErrorCode == enErrorCode.ProcessTrn_OprFail)
                        {
                            var sellObj = _SellTopUpRequest.GetSingle(i => i.TrnNo == WithdrawTrnObj.Trnno);
                            if (sellObj != null)
                            {
                                sellObj.Status = 7;//change new status from 2 to 7
                                sellObj.Remarks = "Withdraw Reinitiate";
                                sellObj.APIStatus = 1;
                                sellObj.UpdatedDate = Helpers.UTC_To_IST();
                                _SellTopUpRequest.UpdateWithAuditLog(sellObj);
                            }
                        }

                        tmpUserid = WithdrawTrnObj.Trnno;

                        //string data = _IFiatIntegrateRepository.GetWithdrawTrnId(WithdrawTrnObj.Trnno);
                        //if (!string.IsNullOrEmpty(data))
                        //{
                        //    if (!string.IsNullOrEmpty(data))
                        //    {
                        //        Response = UpdateTransactionHash(WithdrawTrnObj.WithdrawRefNo.ToString(), data, WithdrawTrnObj.Memberid);
                        //        HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name + "nupoora test", this.GetType().Name, JsonConvert.SerializeObject(response));
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name + "Trnno:" + TrnNo, ex);
            }
        }

        public void UpdateHashFiat()
        {
            BizResponseClass Response = new BizResponseClass();

            var list = _IFiatIntegrateRepository.GetSellWithdrawPendingTrxn();
            if (list.Count > 0)
            {
                foreach (var WithdrawTrnObj in list)
                {
                    string data = _IFiatIntegrateRepository.GetWithdrawTrnId(WithdrawTrnObj.Trnno);
                    if (!string.IsNullOrEmpty(data))
                    {
                        if (!string.IsNullOrEmpty(data))
                        {
                            Response = UpdateTransactionHash(WithdrawTrnObj.WithdrawRefNo.ToString(), data, WithdrawTrnObj.Memberid);
                            HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name + "nupoora testssss", this.GetType().Name, JsonConvert.SerializeObject(Response));
                        }
                    }
                }
            }
        }

        public async Task FiatBinnanceLTPUpate()
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                var list = _trnMasterConfiguration.LPTPairFiat();
                // HelperForLog.WriteLogIntoFile("FiatBinnanceLTPUpate", this.GetType().Name, Helpers.JsonSerialize(list));
                if (list.Count > 0)
                {
                    foreach (var WithdrawTrnObj in list)
                    {
                        TickerData tickerData = await _IResdisTradingManagment.GetTickerDataAsync("BINANCE", WithdrawTrnObj.PairName);
                        // HelperForLog.WriteLogIntoFile("FiatBinnanceLTPUpate", this.GetType().Name, Helpers.JsonSerialize(tickerData));
                        if (tickerData != null)
                        {
                            CurrencyRateMaster CurrencyRateObj = _CurrencyRateMaster.GetSingle(e => e.CurrencyName == WithdrawTrnObj.PairName);
                            if (CurrencyRateObj != null)
                            {
                                CurrencyRateObj.UpdatedDate = Helpers.UTC_To_IST();
                                CurrencyRateObj.CurrentRate = Convert.ToDecimal(tickerData.LTP);
                                _CurrencyRateMaster.Update(CurrencyRateObj);
                            }
                            else
                            {
                                CurrencyRateMaster CurrencyRateObjN = new CurrencyRateMaster();
                                CurrencyRateObjN.Status = 1;
                                CurrencyRateObjN.CreatedBy = 1;
                                CurrencyRateObjN.UpdatedDate = Helpers.UTC_To_IST();
                                CurrencyRateObjN.CreatedDate = Helpers.UTC_To_IST();
                                CurrencyRateObjN.UpdatedBy = 1;
                                CurrencyRateObjN.WalletTypeId = 0;
                                CurrencyRateObjN.CurrencyName = WithdrawTrnObj.PairName;
                                _CurrencyRateMaster.Add(CurrencyRateObjN);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

        #endregion
    }
}

