using System;
using System.Collections.Generic;
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
using Worldex.Core.ViewModels.WalletOpnAdvanced;
using System.Text.RegularExpressions;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.Entities.Charges;
using Worldex.Core.ViewModels.ControlPanel;

namespace Worldex.Infrastructure.Services
{
    public class WalletService : IWalletService
    {
        #region DI
        List<TransactionProviderResponse> transactionProviderResponses;
        ThirdPartyAPIConfiguration thirdPartyAPIConfiguration;
        ThirdPartyAPIRequest thirdPartyAPIRequest;
        private IProfileConfigurationService _profileConfigurationService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly ICommonRepository<WalletTrnLimitConfiguration> _walletTrnLimitConfiguration;
        private readonly ICommonRepository<StakingChargeMaster> _StakingChargeCommonRepo;
        private readonly ICommonRepository<StakingPolicyMaster> _StakingPolicyCommonRepo;
        private readonly ICommonRepository<StakingPolicyDetail> _StakingDetailCommonRepo;
        private readonly ICommonRepository<TokenStakingHistory> _TokenStakingHistoryCommonRepo;
        private readonly ICommonRepository<TokenUnStakingHistory> _TokenUnstakingHistoryCommonRepo;
        private readonly ICommonRepository<WalletMaster> _commonRepository;
        private readonly ICommonRepository<TradingChartData> _TradingChartData;
        private readonly ICommonRepository<UserActivityLog> _UserActivityLogCommonRepo;
        private readonly ICommonRepository<WalletAuthorizeUserMaster> _WalletAuthorizeUserMaster;
        private readonly ICommonRepository<ColdWalletMaster> _ColdWalletMaster;
        private readonly ICommonRepository<WalletLimitConfiguration> _LimitcommonRepository;
        private readonly ICommonRepository<AddressMaster> _addressMstRepository;
        private readonly ICommonRepository<TradeBitGoDelayAddresses> _bitgoDelayRepository;
        private readonly ICommonRepository<BeneficiaryMaster> _BeneficiarycommonRepository;
        private readonly ICommonRepository<UserPreferencesMaster> _UserPreferencescommonRepository;
        private readonly ICommonRepository<MemberShadowBalance> _ShadowBalRepo;
        private readonly ICommonRepository<AddRemoveUserWalletRequest> _AddRemoveUserWalletRequest;
        private readonly ICommonRepository<AllowTrnTypeRoleWise> _AllowTrnTypeRoleWise;
        private readonly ICommonRepository<UserRoleMaster> _UserRoleMaster;
        private readonly ICommonRepository<MemberShadowLimit> _ShadowLimitRepo;
        private readonly ICommonRepository<ActivityTypeHour> _ActivityTypeHour;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommonRepository<ConvertFundHistory> _ConvertFundHistory;
        private readonly IMessageService _messageService;
        private readonly IWalletRepository _walletRepository1;
        private readonly IWebApiRepository _webApiRepository;
        private readonly IWebApiSendRequest _webApiSendRequest;
        private readonly IGetWebRequest _getWebRequest;
        private readonly WebApiParseResponse _WebApiParseResponse;
        private readonly IGenerateAddressQueue<BGTaskAddressGeneration> _IGenerateAddressQueue;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        //vsolanki 8-10-2018 
        private readonly ICommonRepository<WalletTypeMaster> _WalletTypeMasterRepository;
        private readonly ICommonRepository<WithdrawHistory> _WithdrawHistoryRepository;
        private static Random random = new Random((int)DateTime.Now.Ticks);
        //vsolanki 10-10-2018 
        private readonly ICommonRepository<TransactionAccount> _TransactionAccountsRepository;
        private readonly ICommonRepository<ChargeRuleMaster> _chargeRuleMaster;
        private readonly ICommonRepository<ChargeConfigurationDetail> _ChargeConfigrationDetail;
        private readonly ICommonRepository<ChargeConfigurationMaster> _ChargeConfigurationMaster;
        private readonly ISignalRService _signalRService;
        private readonly ICommonWalletFunction _commonWalletFunction;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly IWalletSPRepositories _walletSPRepositories;
        private readonly IWalletTQInsert _WalletTQInsert;
        private readonly ICommonRepository<ThirdPartyAPIConfiguration> _thirdPartyCommonRepository;
        #endregion

        #region Constructor
        public WalletService(IGenerateAddressQueue<BGTaskAddressGeneration> IgenerateAddressQueue, ICommonRepository<WalletAuthorizeUserMaster> WalletAuthorizeUserMaster, ICommonRepository<UserRoleMaster> UserRoleMaster, ICommonRepository<ColdWalletMaster> ColdWalletMaster, ICommonRepository<ChargeConfigurationDetail> ChargeConfigrationDetail, ICommonRepository<ChargeConfigurationMaster> ChargeConfigurationMaster,
            ICommonRepository<AllowTrnTypeRoleWise> AllowTrnTypeRoleWise,
            ICommonRepository<StakingChargeMaster> StakingChargeCommonRepo,
            ICommonRepository<StakingPolicyMaster> StakingPolicyCommonRepo,
            ICommonRepository<StakingPolicyDetail> StakingDetailCommonRepo,
            ICommonRepository<TokenStakingHistory> TokenStakingHistoryCommonRepo,
            ICommonRepository<ActivityTypeHour> ActivityTypeHour,
            ICommonRepository<TokenUnStakingHistory> TokenUnstakingHistoryCommonRepo, ICommonRepository<WalletTrnLimitConfiguration> walletTrnLimitConfiguration,
            ICommonRepository<WalletMaster> commonRepository, ICommonRepository<WalletMaster> commonRepositoryTest, ICommonRepository<AddRemoveUserWalletRequest> AddRemoveUserWalletRequest,
            WebApiParseResponse WebApiParseResponse, ICommonRepository<ThirdPartyAPIConfiguration> thirdPartyCommonRepository,
            IWalletRepository walletRepository, ICommonRepository<WalletMaster> commonRepositoryNew, IWebApiRepository webApiRepository, ICommonRepository<TradingChartData> TradingChartData,
            IWebApiSendRequest webApiSendRequest,
            IGetWebRequest getWebRequest, ICommonRepository<TradeBitGoDelayAddresses> bitgoDelayRepository,
            ICommonRepository<AddressMaster> addressMaster,
            ICommonRepository<WalletTypeMaster> WalletTypeMasterRepository, IProfileConfigurationService profileConfigurationService,

            ICommonRepository<MemberShadowLimit> ShadowLimitRepo, ICommonRepository<MemberShadowBalance> ShadowBalRepo,
            ICommonRepository<BeneficiaryMaster> BeneficiaryMasterRepo,
            ICommonRepository<UserPreferencesMaster> UserPreferenceRepo, ICommonRepository<WalletLimitConfiguration> WalletLimitConfig,
            ICommonRepository<ChargeRuleMaster> chargeRuleMaster,
            ICommonRepository<TransactionAccount> TransactionAccountsRepository, UserManager<ApplicationUser> userManager,
            IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, ISignalRService signalRService,
            ICommonRepository<UserActivityLog> UserActivityLogCommonRepo, Microsoft.Extensions.Configuration.IConfiguration configuration,
            ICommonWalletFunction commonWalletFunction, ICommonRepository<ConvertFundHistory> ConvertFundHistory,
            ICommonRepository<WithdrawHistory> WithdrawHistoryRepository, IWalletSPRepositories walletSPRepositories, IMessageService messageService, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue,
            IWalletTQInsert WalletTQInsert)
        {
            _TradingChartData = TradingChartData;
            _ColdWalletMaster = ColdWalletMaster;
            _walletTrnLimitConfiguration = walletTrnLimitConfiguration;
            _AllowTrnTypeRoleWise = AllowTrnTypeRoleWise;
            _profileConfigurationService = profileConfigurationService;
            _configuration = configuration;
            _UserRoleMaster = UserRoleMaster;
            _StakingChargeCommonRepo = StakingChargeCommonRepo;
            _StakingPolicyCommonRepo = StakingPolicyCommonRepo;
            _StakingDetailCommonRepo = StakingDetailCommonRepo;
            _TokenStakingHistoryCommonRepo = TokenStakingHistoryCommonRepo;
            _TokenUnstakingHistoryCommonRepo = TokenUnstakingHistoryCommonRepo;
            _WalletAuthorizeUserMaster = WalletAuthorizeUserMaster;
            _AddRemoveUserWalletRequest = AddRemoveUserWalletRequest;
            _IGenerateAddressQueue = IgenerateAddressQueue;
            _UserActivityLogCommonRepo = UserActivityLogCommonRepo;
            _ChargeConfigurationMaster = ChargeConfigurationMaster;
            _ChargeConfigrationDetail = ChargeConfigrationDetail;
            _userManager = userManager;
            _commonRepository = commonRepository;
            _ActivityTypeHour = ActivityTypeHour;
            _pushNotificationsQueue = pushNotificationsQueue;
            _bitgoDelayRepository = bitgoDelayRepository;
            _walletRepository1 = walletRepository;
            _webApiRepository = webApiRepository;
            _webApiSendRequest = webApiSendRequest;
            _getWebRequest = getWebRequest;
            _addressMstRepository = addressMaster;
            _WalletTypeMasterRepository = WalletTypeMasterRepository;
            _WebApiParseResponse = WebApiParseResponse;
            _LimitcommonRepository = WalletLimitConfig;
            _BeneficiarycommonRepository = BeneficiaryMasterRepo;
            _UserPreferencescommonRepository = UserPreferenceRepo;
            _ShadowBalRepo = ShadowBalRepo;
            _ShadowLimitRepo = ShadowLimitRepo;
            _chargeRuleMaster = chargeRuleMaster;
            _TransactionAccountsRepository = TransactionAccountsRepository;
            _signalRService = signalRService;
            _commonWalletFunction = commonWalletFunction;
            _ConvertFundHistory = ConvertFundHistory;
            _WithdrawHistoryRepository = WithdrawHistoryRepository;
            _walletSPRepositories = walletSPRepositories;
            _messageService = messageService;
            _pushSMSQueue = pushSMSQueue;
            _WalletTQInsert = WalletTQInsert;//ntrivedi 22-01-2018
            _thirdPartyCommonRepository = thirdPartyCommonRepository;
        }
        #endregion
        //Rushabh 26-10-2018
        public async Task<long> GetWalletID(string AccWalletID)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                Task<WalletMaster> obj1 = _commonRepository.GetSingleAsync(item => item.AccWalletID == AccWalletID && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                WalletMaster obj = await obj1;
                if (obj != null)//Rita for object ref error
                    return obj.Id;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        //Rita 9-1-19 need for social trading
        public async Task<string> GetDefaultAccWalletID(string SMSCode, long UserID)
        {
            try
            {
                WalletTypeMaster obj1 = _WalletTypeMasterRepository.GetSingle(e => e.WalletTypeName == SMSCode);
                //2019-2-15 added condi for only used trading wallet
                WalletMaster obj = await _commonRepository.GetSingleAsync(item => item.WalletTypeID == obj1.Id && item.UserID == UserID && item.IsDefaultWallet == 1 && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

                if (obj != null)//Rita for object ref error
                    return obj.AccWalletID;
                else
                    return "";

            }
            catch (Exception ex)
            {
                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                Task.Run(() => HelperForLog.WriteErrorLog("MarkTransactionHold:##SMSCode " + SMSCode, "WalletServicce", ex));
                return "";
            }
        }

        //Rushabh 27-10-2018
        public async Task<enErrorCode> CheckWithdrawalBene(long WalletID, string Name, string DestinationAddress, enWhiteListingBit bit)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var Walletobj = await _commonRepository.GetSingleAsync(item => item.Id == WalletID && item.Status == Convert.ToInt16(ServiceStatus.Active) && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (Walletobj != null)
                {
                    //Rushabh 02-12-2019 Added UserId Condition As Same Beneficiary Is Found For Multiple User
                    long UserId = Walletobj.UserID;
                    var Beneobj = _BeneficiarycommonRepository.GetSingle(item => item.WalletTypeID == Walletobj.WalletTypeID && item.Address == DestinationAddress && item.UserID == UserId && item.Status == Convert.ToInt16(ServiceStatus.Active));

                 
                    if (Beneobj != null)
                    {
                        if (Beneobj.Address == DestinationAddress && Beneobj.IsWhiteListed == Convert.ToInt16(enWhiteListingBit.ON))
                        {
                            //-----2019-6-17
                            DateTime date = Helpers.UTC_To_IST();
                            TimeSpan difference = date - Beneobj.CreatedDate;
                            double diffHour = difference.TotalHours;

                            int ActivityHour = 24;//2019-6-29  add 24 hour insted of 0 hr
                            var activityhourObj = _ActivityTypeHour.GetSingle(i => i.ActivityType == (int)enActivityType.Benificiary);
                            if (activityhourObj != null)
                            {
                                ActivityHour = activityhourObj.ActivityHour;
                            }
                            if (ActivityHour >= diffHour)//Convert.ToInt32(_configuration["WithdrawHour"])
                            {
                                return enErrorCode.WithdrawNotAllowdBeforehrBene;
                            }
                            //-------
                            return enErrorCode.Success;
                        }
                        else
                        {
                            return enErrorCode.AddressNotFoundOrWhitelistingBitIsOff;
                        }
                    }
                    return enErrorCode.BeneficiaryNotFound;
                }
                return enErrorCode.WalletNotFound;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public async Task<BizResponseClass> ValidateAddress(string TrnAccountNo, int Length, string StartsWith, string AccNoValidationRegex)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                Regex NumRegex = new Regex(@"^[0-9]+$", RegexOptions.Compiled);
                Regex AlphaRegex = new Regex(@"^[a-zA-Z]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                Regex SpecialCharRegex = new Regex(@"^[~`!@#$%^&*()-+=|\{}':;.,<>/?]$", RegexOptions.Compiled);
                Regex AddressRegex;


                if (String.IsNullOrEmpty(TrnAccountNo))
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.TrnAccNoRequired;
                    Resp.ErrorCode = enErrorCode.TrnAccNoRequired;
                    return Resp;
                }
                if (NumRegex.IsMatch(TrnAccountNo))
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InvalidTrnAccNo;
                    Resp.ErrorCode = enErrorCode.InvalidTrnAccNoOnlyDigit;
                    return Resp;
                }
                if (AlphaRegex.IsMatch(TrnAccountNo))
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InvalidTrnAccNo;
                    Resp.ErrorCode = enErrorCode.InvalidTrnAccNoOnlyAlphabet;
                    return Resp;
                }
                if (SpecialCharRegex.IsMatch(TrnAccountNo))
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InvalidTrnAccNo;
                    Resp.ErrorCode = enErrorCode.InvalidTrnAccNoOnlySpecialChars;
                    return Resp;
                }
                if (Length > 0)
                {
                    if (TrnAccountNo.Length != Length)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidTrnAccNo;
                        Resp.ErrorCode = enErrorCode.InvalidTrnAccNoLength;
                        return Resp;
                    }
                }
                if (!String.IsNullOrEmpty(StartsWith))
                {
                    if (!TrnAccountNo.StartsWith(StartsWith))
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidTrnAccNo;
                        Resp.ErrorCode = enErrorCode.InvalidTrnAccNoStartsWithChar;
                        return Resp;
                    }
                }
                if (!String.IsNullOrEmpty(AccNoValidationRegex))
                {
                    AddressRegex = new Regex(AccNoValidationRegex, RegexOptions.Compiled);
                    if (!AddressRegex.IsMatch(TrnAccountNo))
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidTrnAccNo;
                        Resp.ErrorCode = enErrorCode.AddressRegexValidationFail;
                        return Resp;
                    }
                }
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.ValidTrnAccNo;
                Resp.ErrorCode = enErrorCode.Success;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ValidateAddress", this.GetType().Name, ex);
                return null;
            }
        }

        public ListChargesTypeWise ListChargesTypeWise(string WalletTypeName, long? TrnTypeId, long UserId)
        {
            var typeObj = _WalletTypeMasterRepository.GetSingle(i => i.WalletTypeName == WalletTypeName);
            ListChargesTypeWise Resp = new ListChargesTypeWise();
            List<ChargeWalletType> walletTypes = new List<ChargeWalletType>();
            try
            {
                long? Id = null;
                if (typeObj != null)
                {
                    Id = typeObj.Id;
                }
                var res = _walletRepository1.GetChargeWalletType(Id);
                var StakingCharges = _StakingChargeCommonRepo.GetSingle(k => k.UserID == UserId && k.Status == 1);
                for (int i = 0; i <= res.Count - 1; i++)
                {
                    ChargeWalletType a = new ChargeWalletType();
                    a.WalletTypeName = res[i].WalletTypeName;
                    a.WalletTypeId = res[i].WalletTypeId;
                    a.Charges = new List<ChargesTypeWise>();
                    var data = _walletRepository1.ListChargesTypeWise(res[i].WalletTypeId, TrnTypeId);
                    a.Charges = data;
                    if (StakingCharges != null)
                    {
                        a.Charges.ForEach(item =>
                        {
                            if (item.MakerCharge > 0 && item.TakerCharge > 0)
                            {
                                item.MakerCharge = StakingCharges.MakerCharge;
                                item.TakerCharge = StakingCharges.TakerCharge;
                            }
                        });
                    }
                    walletTypes.Add(a);
                }
                if (walletTypes.Count == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.Data = walletTypes;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<CreateWalletAddressRes> GenerateAddress(string walletID, string coin, string Token, int GenaratePendingbit = 0, long UseriD = 0)
        {
            try
            {
                UserActivityLog activityLog = new UserActivityLog();
                TradeBitGoDelayAddresses delayAddressesObj, delayGeneratedAddressesObj;
                var walletMasterobj = _commonRepository.GetSingleAsync(item => item.AccWalletID == walletID && item.Status != Convert.ToInt16(ServiceStatus.Disable) && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));//05-12-2018

                AddressMaster addressMaster;
                string address = "";
                string Respaddress = null;

                var wallettype = _WalletTypeMasterRepository.GetSingleAsync(t => t.WalletTypeName == coin);
                WalletTypeMaster walletTypeMaster = await wallettype;
                if ((walletTypeMaster.IsLocal != 0))
                {
                    if (walletTypeMaster.IsLocal != 8)
                    {
                        var res = CreateERC20Address(UseriD, coin, walletID, Convert.ToInt16(walletTypeMaster.IsLocal));
                        return res;
                    }
                }
                //2019-7-23 remove duplicatecode
                //if (walletTypeMaster.IsLocal == 2 || walletTypeMaster.IsLocal == 3 || walletTypeMaster.IsLocal == 4 || walletTypeMaster.IsLocal == 6 || walletTypeMaster.IsLocal == 7)//2-TRX,3-TRC 10,4-TRC-20,5=neo,6 =sox ,7-usdx
                //{
                //    var res = CreateERC20Address(UseriD, coin, walletID, Convert.ToInt16(walletTypeMaster.IsLocal));
                //    return res;
                //}
                //if (walletTypeMaster.IsLocal == 5)//2-TRX,3-TRC 10,4-TRC-20,5=neo
                //{
                //    var res = CreateNeoAddress(UseriD, coin, walletID, Convert.ToInt16(walletTypeMaster.IsLocal));
                //    return res;
                //}
                var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = coin.ToLower(), amount = 0, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.Generate_Address) });//05-12-2018
                WalletMaster walletMaster = await walletMasterobj;//05-12-2018

               
                if (walletTypeMaster.Id != walletMaster.WalletTypeID)
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidWallet, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet };
                }
                if (walletMaster == null)
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidWallet, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet };
                }
                else if (walletMaster.Status != Convert.ToInt16(ServiceStatus.Active))
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidWallet, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet };
                }

                transactionProviderResponses = await providerdata;
                if (transactionProviderResponses == null || transactionProviderResponses.Count == 0)
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.ItemNotFoundForGenerateAddress, ReturnCode = enResponseCode.Fail, ReturnMsg = "Please try after sometime." };
                }
                if (transactionProviderResponses[0].ThirPartyAPIID == 0)
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                var addressObj = _addressMstRepository.GetSingle(i => i.WalletId == walletMaster.Id && i.Status == 1 && i.SerProID == transactionProviderResponses[0].ServiceProID);
                if (addressObj != null)
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.AddressExist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AddressExist };
                }
                var apiconfig = _thirdPartyCommonRepository.GetByIdAsync(transactionProviderResponses[0].ThirPartyAPIID);

                thirdPartyAPIConfiguration = await apiconfig;
                if (thirdPartyAPIConfiguration == null || transactionProviderResponses.Count == 0)
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                
                thirdPartyAPIRequest = _getWebRequest.MakeWebRequest(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID);
                string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);
                // parse response logic 


                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses[0].ThirPartyAPIID);
                if (!String.IsNullOrEmpty(ParsedResponse.Param3))
                {
                    Respaddress = ParsedResponse.TrnRefNo + "?dt=" + ParsedResponse.Param3;
                }
                else
                {
                    Respaddress = ParsedResponse.TrnRefNo;
                }

                var Key = ParsedResponse.Param2;
                var dt = ParsedResponse.Param3;
                if (!string.IsNullOrEmpty(apiResponse) && !string.IsNullOrEmpty(Respaddress))
                {
                    if (string.IsNullOrEmpty(walletMaster.PublicAddress))
                    {
                        walletMaster.PublicAddress = Respaddress;
                        _commonRepository.UpdateFieldAsync(walletMaster, e => e.PublicAddress);
                    }
                }
                if (!string.IsNullOrEmpty(apiResponse) && thirdPartyAPIRequest.DelayAddress == 1)
                {
                    delayAddressesObj = GetTradeBitGoDelayAddresses(0, walletMaster.WalletTypeID, ParsedResponse.StatusMsg, "", thirdPartyAPIRequest.walletID, walletMaster.CreatedBy, ParsedResponse.Param1, 1, 0, coin);
                    delayAddressesObj = _bitgoDelayRepository.Add(delayAddressesObj);

                    if (GenaratePendingbit == 0)
                    {
                        delayGeneratedAddressesObj = _walletRepository1.GetUnassignedETH();

                        if (delayGeneratedAddressesObj == null)
                        {
                            return new CreateWalletAddressRes { address = Respaddress, ErrorCode = enErrorCode.UnAssignedAddressFetchFail, ReturnCode = enResponseCode.Fail, ReturnMsg = "please try after some time" };
                        }
                        address = delayGeneratedAddressesObj.Address;
                        Respaddress = delayGeneratedAddressesObj.Address;

                        delayGeneratedAddressesObj.WalletId = walletMaster.Id;
                        delayGeneratedAddressesObj.UpdatedBy = walletMaster.UserID;
                        delayGeneratedAddressesObj.UpdatedDate = Helpers.UTC_To_IST();
                        _bitgoDelayRepository.Update(delayGeneratedAddressesObj);

                    }
                    else
                    {
                        return new CreateWalletAddressRes { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.CreateAddressSuccessMsg };
                    }
                }
                if (!string.IsNullOrEmpty(Respaddress))
                {
                    addressMaster = GetAddressObj(walletMaster.Id, transactionProviderResponses[0].ServiceProID, Respaddress, "Self Address", walletMaster.UserID, 0, 1, Key, dt);

                    activityLog.ActivityType = Convert.ToInt16(EnUserActivityType.GenerateAddress);
                    activityLog.CreatedBy = UseriD;
                    activityLog.CreatedDate = Helpers.UTC_To_IST();
                    activityLog.UserID = UseriD;
                    activityLog.WalletID = walletMaster.Id;
                    activityLog.Remarks = "Address Generated For " + coin;

                    BGTaskAddressGeneration obj = new BGTaskAddressGeneration();
                    obj.AccWalletId = walletMaster.AccWalletID;
                    obj.Address = addressMaster;
                    obj.PublicAddress = addressMaster.Address;
                    obj.Amount = walletMaster.Balance;
                    obj.UID = UseriD.ToString();
                    obj.WalletName = walletMaster.Walletname;
                    obj.Coin = coin;
                    obj.Token = UseriD.ToString();
                    obj.Date = Helpers.UTC_To_IST().ToString();
                    obj.userActivityLog = activityLog;
                    _IGenerateAddressQueue.Enqueue(obj);

                    string responseString = Respaddress;
                    return new CreateWalletAddressRes { address = Respaddress, ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.CreateAddressSuccessMsg };
                }
                else
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.AddressGenerationFailed, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GenerateAddress ", "WalletService", ex);
                return null;
            }
        }

        public AddressMaster GetAddressObj(long walletID, long serproID, string address, string addressName, long createdBy, byte isDefaultAdd, short status, string GUID, string Dt)
        {
            try
            {
                AddressMaster addressMaster = new AddressMaster();
                addressMaster.Address = address;
                addressMaster.OriginalAddress = address;
                addressMaster.AddressLable = addressName;
                addressMaster.CreatedBy = createdBy;
                addressMaster.CreatedDate = Helpers.UTC_To_IST();
                addressMaster.IsDefaultAddress = isDefaultAdd;
                addressMaster.SerProID = serproID;
                addressMaster.Status = status;
                addressMaster.WalletId = walletID;
                addressMaster.GUID = GUID;
                addressMaster.DestinationTag = Dt;
                return addressMaster;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public TradeBitGoDelayAddresses GetTradeBitGoDelayAddresses(long walletID, long WalletTypeId, string TrnID, string address, string BitgoWalletId, long createdBy, string CoinSpecific, short status, byte generatebit, string coin)
        {
            try
            {
                TradeBitGoDelayAddresses addressMaster = new TradeBitGoDelayAddresses
                {
                    CoinSpecific = CoinSpecific,
                    Address = address,
                    BitgoWalletId = BitgoWalletId,
                    CoinName = coin,
                    CreatedBy = createdBy,
                    CreatedDate = Helpers.UTC_To_IST(),
                    GenerateBit = generatebit,
                    Status = status,
                    TrnID = TrnID,
                    WalletId = walletID,
                    WalletTypeId = WalletTypeId
                };

                return addressMaster;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
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

        //vsolanki 10-10-2018 Insert into WalletMaster table
        public async Task<CreateWalletResponse> InsertIntoWalletMaster(string Walletname, string CoinName, byte IsDefaultWallet, int[] AllowTrnType, long userId, string accessToken = null, int isBaseService = 0, long OrgId = 0, DateTime? ExpiryDate = null)
        {
            bool IsValid = true;
            decimal Balance = 0;
            string PublicAddress = "";
            WalletMaster walletMaster = new WalletMaster();
            CreateWalletResponse createWalletResponse = new CreateWalletResponse();
            try
            {
                var walletMasters = _WalletTypeMasterRepository.GetSingle(item => item.WalletTypeName == CoinName);
                if (walletMasters == null)
                {
                    createWalletResponse.ReturnCode = enResponseCode.Fail;
                    createWalletResponse.ReturnMsg = EnResponseMessage.InvalidCoin;
                    createWalletResponse.ErrorCode = enErrorCode.InvalidCoinName;
                    return createWalletResponse;
                }

                //2019-2-15 added condi for only used trading wallet
                var ISExist = _commonRepository.GetSingle(i => i.UserID == userId && i.WalletTypeID == walletMasters.Id && i.IsDefaultWallet == 1 && i.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (ISExist != null && IsDefaultWallet == 1)
                {
                    ISExist.IsDefaultWallet = 0;
                    _commonRepository.Update(ISExist);
                }
                //add data in walletmaster tbl
                walletMaster.Walletname = Walletname;
                walletMaster.OrgID = OrgId;
                walletMaster.ExpiryDate = (ExpiryDate == null ? Helpers.UTC_To_IST().AddYears(1) : ExpiryDate);
                walletMaster.IsValid = IsValid;
                walletMaster.UserID = userId;
                walletMaster.WalletTypeID = walletMasters.Id;
                walletMaster.Balance = Balance;
                walletMaster.PublicAddress = PublicAddress;
                walletMaster.IsDefaultWallet = IsDefaultWallet;
                walletMaster.CreatedBy = userId;
                walletMaster.CreatedDate = Helpers.UTC_To_IST();
                walletMaster.Status = Convert.ToInt16(ServiceStatus.Active);
                walletMaster.OrgID = 1;
                walletMaster.AccWalletID = RandomGenerateAccWalletId(userId, IsDefaultWallet);
                walletMaster = _commonRepository.Add(walletMaster);

                WalletAuthorizeUserMaster obj = new WalletAuthorizeUserMaster();
                obj.RoleID = 1;
                obj.UserID = userId;
                obj.Status = 1;
                obj.CreatedBy = userId;
                obj.CreatedDate = Helpers.UTC_To_IST();
                obj.UpdatedDate = Helpers.UTC_To_IST();
                obj.WalletID = walletMaster.Id;
                obj.OrgID = Convert.ToInt64(walletMaster.OrgID);
                _WalletAuthorizeUserMaster.Add(obj);//add new enrty

                //genrate address and update in walletmaster
                if (isBaseService == 0)
                {
                    var addressClass = await GenerateAddress(walletMaster.AccWalletID, CoinName, accessToken);
                    if (addressClass.address != null)
                    {
                        walletMaster.WalletPublicAddress(addressClass.address);
                    }
                    else
                    {
                        walletMaster.WalletPublicAddress("NotGenerate");
                    }
                    _commonRepository.Update(walletMaster);
                }
                createWalletResponse.AccWalletID = walletMaster.AccWalletID;
                createWalletResponse.PublicAddress = walletMaster.PublicAddress;

                createWalletResponse.ReturnCode = enResponseCode.Success;
                createWalletResponse.ReturnMsg = EnResponseMessage.CreateWalletSuccessMsg;
                createWalletResponse.ErrorCode = enErrorCode.Success;

                #region MSG_Email
                ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.NewCreateWalletSuccessMsg);
                ActivityNotification.Param1 = walletMaster.Walletname;
                ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);

                Parallel.Invoke(() => _signalRService.SendActivityNotificationV2(ActivityNotification, userId.ToString(), 2),
                    () => SMSSendAsyncV1(EnTemplateType.SMS_WalletCreate, userId.ToString(), walletMaster.Walletname),
                  () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletCreate, userId.ToString(), walletMaster.Walletname, CoinName, walletMaster.AccWalletID, Helpers.UTC_To_IST().ToString(), walletMaster.Balance.ToString()));

                #endregion
                return createWalletResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertIntoWalletMaster", "WalletService", ex);
                return null;
            }
        }

        //vsolanki 12-10-2018 Select WalletMaster table 
        public ListWalletResponse ListWallet(long userid)
        {
            ListWalletResponse listWalletResponse = new ListWalletResponse();
            try
            {
                var walletResponse = _walletRepository1.ListWalletMasterResponse(userid);
                if (walletResponse.Count == 0)
                {
                    listWalletResponse.ReturnCode = enResponseCode.Fail;
                    listWalletResponse.ReturnMsg = EnResponseMessage.NotFound;
                    listWalletResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    listWalletResponse.Wallets = walletResponse;
                    listWalletResponse.ReturnCode = enResponseCode.Success;
                    listWalletResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    listWalletResponse.ErrorCode = enErrorCode.Success;

                }
                return listWalletResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                listWalletResponse.ReturnCode = enResponseCode.InternalError;
                return listWalletResponse;
            }
        }

        //vsolanki 12-10-2018 Select WalletMaster table ByCoin
        public ListWalletResponse GetWalletByCoin(long userid, string coin)
        {
            ListWalletResponse listWalletResponse = new ListWalletResponse();
            try
            {
                var walletResponse = _walletRepository1.GetWalletMasterResponseByCoin(userid, coin);
                var UserPrefobj = _UserPreferencescommonRepository.FindBy(item => item.UserID == userid && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (walletResponse.Count == 0)
                {
                    listWalletResponse.ReturnCode = enResponseCode.Fail;
                    listWalletResponse.ReturnMsg = EnResponseMessage.NotFound;
                    listWalletResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    if (UserPrefobj != null)
                    {
                        listWalletResponse.IsWhitelisting = UserPrefobj.IsWhitelisting;
                    }
                    else
                    {
                        listWalletResponse.IsWhitelisting = 0;
                    }
                    listWalletResponse.Wallets = walletResponse;
                    listWalletResponse.ReturnCode = enResponseCode.Success;
                    listWalletResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    listWalletResponse.ErrorCode = enErrorCode.Success;

                }
                return listWalletResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                listWalletResponse.ReturnCode = enResponseCode.InternalError;
                return listWalletResponse;
            }
        }

        //vsolanki 12-10-2018 Select WalletMaster table ByCoin
        public ListWalletResponse GetWalletById(long userid, string coin, string walletId)
        {
            ListWalletResponse listWalletResponse = new ListWalletResponse();
            try
            {
                var walletResponse = _walletRepository1.GetWalletMasterResponseById(userid, coin, walletId);
                if (walletResponse.Count == 0)
                {
                    listWalletResponse.ReturnCode = enResponseCode.Fail;
                    listWalletResponse.ReturnMsg = EnResponseMessage.NotFound;
                    listWalletResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    listWalletResponse.Wallets = walletResponse;
                    listWalletResponse.ReturnCode = enResponseCode.Success;
                    listWalletResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    listWalletResponse.ErrorCode = enErrorCode.Success;
                }
                return listWalletResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                listWalletResponse.ReturnCode = enResponseCode.InternalError;
                return listWalletResponse;
            }
        }

        public WalletTransactionQueue InsertIntoWalletTransactionQueue(Guid Guid, enWalletTranxOrderType TrnType, decimal Amount, long TrnRefNo, DateTime TrnDate, DateTime? UpdatedDate,
            long WalletID, string WalletType, long MemberID, string TimeStamp, enTransactionStatus Status, string StatusMsg, enWalletTrnType enWalletTrnType)
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
                return walletTransactionQueue;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<WalletDrCrResponse> GetWalletDeductionNew(string coinName, string timestamp, enWalletTranxOrderType orderType, decimal amount, long userID, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, string Token = "")
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
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "WalletService", "timestamp:" + timestamp + "," + "coinName:" + coinName + ",accWalletID=" + accWalletID + ",TrnRefNo=" + TrnRefNo.ToString() + ",userID=" + userID + ",amount=" + amount.ToString());

                //Task<int> countTask = _walletRepository1.CheckTrnRefNoAsync(TrnRefNo, orderType, trnType); //CheckTrnRefNo(TrnRefNo, orderType, trnType);
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
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTrnType, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTrnType, ErrorCode = enErrorCode.InvalidTrnType, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                if (TrnRefNo == 0) // sell 13-10-2018
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTradeRefNo, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                if (amount <= 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidAmt, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmount, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                dWalletobj = await dWalletobjTask;
                //Task<bool> flagTask = CheckUserBalanceAsync(dWalletobj.Id);
                bool flagTask = _walletRepository1.CheckUserBalanceV1(dWalletobj.Id);
                if (dWalletobj == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TimeStamp = timestamp }, "Debit");
                }
                if (dWalletobj.Status != 1 || dWalletobj.IsValid == false)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidWallet, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "CheckUserBalance pre Balance=" + dWalletobj.Balance.ToString() + ", TrnNo=" + TrnRefNo.ToString() + ",TimeStamp=" + timestamp);
                //CheckUserBalanceFlag = await flagTask;
                CheckUserBalanceFlag = flagTask;
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "CheckUserBalance Post TrnNo=" + TrnRefNo.ToString() + ",TimeStamp=" + timestamp);

                dWalletobj = _commonRepository.GetById(dWalletobj.Id); // ntrivedi fetching fresh balance for multiple request at a time 
                var msg = _commonWalletFunction.CheckWalletLimitAsyncV1(enWalletLimitType.WithdrawLimit, dWalletobj.Id, amount, TrnRefNo);

                if (dWalletobj.Balance < amount)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficantBal, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }
                if (!CheckUserBalanceFlag)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedBalanceMismatch, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "Debit");
                }

                var limitres = await msg;
                if (limitres.ErrorCode != enErrorCode.Success)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletLimitExceed, trnType);
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
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.AlredyExist, trnType);
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
                    Task.Run(() => WalletDeductionNewNotificationSend(timestamp, dWalletobj, coinName, amount, TrnRefNo, (byte)routeTrnType, userID, Token, trnType.ToString(), walletMasterObj, charge, DeductWalletType, ChargeWalletObj));

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

        //Rushabh 15-10-2018 List All Addresses Of Specified Wallet
        public Core.ViewModels.WalletOperations.ListWalletAddressResponse ListAddress(string AccWalletID)
        {
            ListWalletAddressResponse AddressResponse = new ListWalletAddressResponse();
            try
            {
                var WalletAddResponse = _walletRepository1.ListAddressMasterResponse(AccWalletID);
                if (WalletAddResponse.Count == 0)
                {
                    AddressResponse.ReturnCode = enResponseCode.Fail;
                    AddressResponse.ReturnMsg = EnResponseMessage.NotFound;
                    AddressResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    AddressResponse.AddressList = WalletAddResponse;
                    AddressResponse.ReturnCode = enResponseCode.Success;
                    AddressResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    AddressResponse.ErrorCode = enErrorCode.Success;
                }
                return AddressResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                AddressResponse.ReturnCode = enResponseCode.InternalError;
                return AddressResponse;
            }
        }

        public Core.ViewModels.WalletOperations.ListWalletAddressResponse GetAddress(string AccWalletID)
        {
            ListWalletAddressResponse AddressResponse = new ListWalletAddressResponse();
            try
            {
                var WalletAddResponse = _walletRepository1.GetAddressMasterResponse(AccWalletID);
                if (WalletAddResponse.Count == 0)
                {
                    AddressResponse.ReturnCode = enResponseCode.Fail;
                    AddressResponse.ReturnMsg = EnResponseMessage.NotFound;
                    AddressResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    AddressResponse.AddressList = WalletAddResponse;
                    AddressResponse.ReturnCode = enResponseCode.Success;
                    AddressResponse.ErrorCode = enErrorCode.Success;
                    AddressResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    AddressResponse.ErrorCode = enErrorCode.Success;
                }
                return AddressResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                AddressResponse.ReturnCode = enResponseCode.InternalError;
                return AddressResponse;
            }
        }

        public WithdrawHistoryResponse DepositHistoy(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status, string TrnNo, long Userid, int PageNo)
        {
            try
            {
                WithdrawHistoryResponse response2 = new WithdrawHistoryResponse();
                DateTime Mindate = DateTime.Parse("2000/1/1");
                DateTime Maxdate = DateTime.Parse("9999/1/1");
                //DateTime dt;
                if ((FromDate.Year <= Mindate.Year && FromDate.Year < Maxdate.Year) || (ToDate.Year <= Mindate.Year && ToDate.Year < Maxdate.Year))
                {
                    response2.ReturnCode = enResponseCode.Fail;
                    response2.ErrorCode = enErrorCode.InvalidDate;
                    response2.ReturnMsg = "InValid Date";
                    return response2;
                }
                DateTime newTodate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                WithdrawHistoryResponse response = _walletRepository1.DepositHistoy(FromDate, newTodate, Coin, TrnNo, Amount, Status, Userid, PageNo);
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public WithdrawHistoryResponsev2 DepositHistoyv2(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status, string TrnId, long Userid, int PageNo)
        {
            try
            {
                WithdrawHistoryResponsev2 response2 = new WithdrawHistoryResponsev2();
                DateTime Mindate = DateTime.Parse("2000/1/1");
                DateTime Maxdate = DateTime.Parse("9999/1/1");
                //DateTime dt;
                if ((FromDate.Year <= Mindate.Year && FromDate.Year < Maxdate.Year) || (ToDate.Year <= Mindate.Year && ToDate.Year < Maxdate.Year))
                {
                    response2.ReturnCode = enResponseCode.Fail;
                    response2.ErrorCode = enErrorCode.InvalidDate;
                    response2.ReturnMsg = "InValid Date";
                    return response2;
                }
                DateTime newTodate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                WithdrawHistoryResponsev2 response = _walletRepository1.DepositHistoyv2(FromDate, newTodate, Coin, TrnId, Amount, Status, Userid, PageNo);
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public WithdrawHistoryNewResponse WithdrawalHistoy(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status, long Userid, int PageNo, short? IsInternalTransfer)
        {
            try
            {
                WithdrawHistoryNewResponse response2 = new WithdrawHistoryNewResponse();
                DateTime Mindate = DateTime.Parse("2000/1/1");
                DateTime Maxdate = DateTime.Parse("9999/1/1");
                //DateTime dt;
                if ((FromDate.Year <= Mindate.Year && FromDate.Year < Maxdate.Year) || (ToDate.Year <= Mindate.Year && ToDate.Year < Maxdate.Year))
                {
                    response2.ReturnCode = enResponseCode.Fail;
                    response2.ErrorCode = enErrorCode.InvalidDate;
                    response2.ReturnMsg = "InValid Date";
                    return response2;
                }
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                DateTime NewTodate = ToDate.AddHours(23);
                NewTodate = NewTodate.AddMinutes(59);
                NewTodate = NewTodate.AddSeconds(59);
                WithdrawHistoryNewResponse response = _walletRepository1.WithdrawalHistoy(FromDate, NewTodate, Coin, Amount, Status, Userid, PageNo, IsInternalTransfer);
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public WithdrawHistoryNewResponsev2 WithdrawalHistoyv2(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status, long Userid, int PageNo, short? IsInternalTransfer)
        {
            try
            {
                WithdrawHistoryNewResponsev2 response2 = new WithdrawHistoryNewResponsev2();
                DateTime Mindate = DateTime.Parse("2000/1/1");
                DateTime Maxdate = DateTime.Parse("9999/1/1");
                //DateTime dt;
                if ((FromDate.Year <= Mindate.Year && FromDate.Year < Maxdate.Year) || (ToDate.Year <= Mindate.Year && ToDate.Year < Maxdate.Year))
                {
                    response2.ReturnCode = enResponseCode.Fail;
                    response2.ErrorCode = enErrorCode.InvalidDate;
                    response2.ReturnMsg = "InValid Date";
                    return response2;
                }
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                DateTime NewTodate = ToDate.AddHours(23);
                NewTodate = NewTodate.AddMinutes(59);
                NewTodate = NewTodate.AddSeconds(59);
                WithdrawHistoryNewResponsev2 response = _walletRepository1.WithdrawalHistoyv2(FromDate, NewTodate, Coin, Amount, Status, Userid, PageNo, IsInternalTransfer);
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<LimitResponse> SetWalletLimitConfig(string accWalletID, WalletLimitConfigurationReq request, long Userid, string Token)
        {
            //Devide startTimeUnix and EndTimeUnix by 1000 because value is goes out of range of double by Rushali (26-12-2019)
            if (request.StartTimeUnix != null && request.EndTimeUnix != null)
            {
                request.StartTimeUnix = Convert.ToDouble(request.StartTimeUnix / 1000);
                request.EndTimeUnix = Convert.ToDouble(request.EndTimeUnix / 1000);
            }
            int type = Convert.ToInt16(request.TrnType);
            if (type == Convert.ToInt32(enWalletLimitType.WithdrawLimit))
            {
                type = Convert.ToInt32(enWalletTrnType.Withdrawal);
            }
            else if (type == Convert.ToInt32(enWalletLimitType.DepositLimit))
            {
                type = Convert.ToInt32(enWalletTrnType.Deposit);
            }
            WalletLimitConfiguration IsExist = new WalletLimitConfiguration();
            LimitResponse Response = new LimitResponse();
            try
            {
                var walletMasters = _commonRepository.GetSingle(item => item.AccWalletID == accWalletID && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

                if (walletMasters == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.InvalidWallet;
                    Response.ErrorCode = enErrorCode.InvalidWalletId;
                    return Response;
                }
                var MasterConfigTask = _walletTrnLimitConfiguration.GetSingleAsync(item => item.TrnType == type && item.WalletType == walletMasters.WalletTypeID);

                var walletType = _WalletTypeMasterRepository.GetSingle(item => item.Id == walletMasters.WalletTypeID);
                IsExist = _LimitcommonRepository.GetSingle(item => item.TrnType == type && item.WalletId == walletMasters.Id);

                UserActivityLog activityLog = new UserActivityLog();
                activityLog.ActivityType = Convert.ToInt16(EnUserActivityType.SetWalletLimit);
                activityLog.CreatedBy = Userid;
                activityLog.CreatedDate = Helpers.UTC_To_IST();
                activityLog.UserID = Userid;
                activityLog.WalletID = walletMasters.Id;

                var MasterConfig = await MasterConfigTask;
                if (MasterConfig == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.MasterDataNotFound;
                    Response.ErrorCode = enErrorCode.MasterDataNotFound;
                    return Response;
                }

                if (IsExist == null)
                {
                    if ((request.LimitPerDay <= MasterConfig.DailyTrnAmount) || (MasterConfig.DailyTrnAmount == 0))
                    {
                        if ((request.LimitPerHour <= MasterConfig.HourlyTrnAmount) || (MasterConfig.HourlyTrnAmount == 0))
                        {
                            if ((request.LimitPerTransaction <= MasterConfig.MaxAmount || request.LimitPerTransaction == 0) || (MasterConfig.MaxAmount == 0))
                            {
                                if ((request.LimitPerTransaction > MasterConfig.MinAmount || request.LimitPerTransaction == 0) || (MasterConfig.MinAmount == 0))
                                {
                                    WalletLimitConfiguration newobj = new WalletLimitConfiguration
                                    {
                                        WalletId = walletMasters.Id,
                                        TrnType = type,
                                        LimitPerHour = request.LimitPerHour,
                                        LimitPerDay = request.LimitPerDay,
                                        LimitPerTransaction = request.LimitPerTransaction,
                                        CreatedBy = Userid,
                                        CreatedDate = Helpers.UTC_To_IST(),
                                        Status = Convert.ToInt16(ServiceStatus.Active),
                                        StartTimeUnix = (request.StartTimeUnix == null ? 0 : request.StartTimeUnix),
                                        LifeTime = request.LifeTime,
                                        EndTimeUnix = (request.EndTimeUnix == null ? 0 : request.EndTimeUnix)
                                    };
                                    newobj = _LimitcommonRepository.Add(newobj);
                                    activityLog.Remarks = "New Limit Created For " + request.TrnType.ToString();
                                    _UserActivityLogCommonRepo.Add(activityLog);
                                    #region SMS_Email
                                    ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                                    ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.CWalletLimitNotification);
                                    ActivityNotification.Param1 = walletMasters.Walletname;
                                    ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);
                                    HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, "Before : " + Helpers.UTC_To_IST().ToString());
                                    Task.Run(() => _signalRService.SendActivityNotificationV2(ActivityNotification, Userid.ToString(), 2));
                                    HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, "After : " + Helpers.UTC_To_IST().ToString());
                                    Response.ReturnMsg = EnResponseMessage.SetWalletLimitCreateMsg;
                                    Response.ReturnCode = enResponseCode.Success;
                                    Response.ErrorCode = enErrorCode.Success;


                                    DateTime dtMStart = new DateTime();
                                    DateTime dtMEnd = new DateTime();
                                    string startT = "", endT = "";
                                    if (request.StartTimeUnix != null && request.EndTimeUnix != null)
                                    {
                                        dtMStart = dtMStart.AddSeconds(Convert.ToDouble(request.StartTimeUnix)).ToLocalTime();
                                        dtMEnd = dtMEnd.AddSeconds(Convert.ToDouble(request.EndTimeUnix)).ToLocalTime();
                                        TimeSpan MStartTime = dtMStart.TimeOfDay;
                                        TimeSpan MEndTime = dtMEnd.TimeOfDay;

                                        startT = dtMStart.ToString("hh:mm tt");
                                        endT = dtMEnd.ToString("hh:mm tt");
                                    }
                                    else
                                    {
                                        startT = "N/A";
                                        endT = "N/A";
                                    }
                                    EmailSendAsyncV1(EnTemplateType.EMAIL_WalletLimitCreated, Userid.ToString(), walletType.WalletTypeName, walletMasters.Walletname, request.TrnType.ToString(), request.LimitPerHour.ToString(), request.LimitPerDay.ToString(), request.LimitPerTransaction.ToString(), request.LifeTime.ToString(), startT, endT);
                                    SMSSendAsyncV1(EnTemplateType.SMS_WalletLimitCreated, Userid.ToString(), walletMasters.Walletname);

                                    #endregion
                                }
                                else
                                {
                                    Response.ReturnCode = enResponseCode.Fail;
                                    Response.ReturnMsg = EnResponseMessage.LimitPerTransactionMinExceed;
                                    Response.ReturnMsg = Response.ReturnMsg.Replace("@Limit", MasterConfig.MinAmount.ToString());
                                    Response.ErrorCode = enErrorCode.LimitPerTransactionMinExceed;
                                    #region
                                    ActivityNotificationMessage ActivityNotificationErr = new ActivityNotificationMessage();
                                    ActivityNotificationErr.MsgCode = Convert.ToInt32(enErrorCode.SignalR_LimitPerTransactionMinExceed);
                                    ActivityNotificationErr.Param1 = MasterConfig.MinAmount.ToString();
                                    ActivityNotificationErr.Type = Convert.ToInt16(EnNotificationType.Info);
                                    Task.Run(() => _signalRService.SendActivityNotificationV2(ActivityNotificationErr, Userid.ToString(), 2));
                                    #endregion
                                }
                            }
                            else
                            {
                                Response.ReturnCode = enResponseCode.Fail;
                                Response.ReturnMsg = EnResponseMessage.LimitPerTransactionMaxExceed;
                                Response.ReturnMsg = Response.ReturnMsg.Replace("@Limit", MasterConfig.MaxAmount.ToString());
                                Response.ErrorCode = enErrorCode.LimitPerTransactionMaxExceed;
                                #region
                                ActivityNotificationMessage ActivityNotificationErr = new ActivityNotificationMessage();
                                ActivityNotificationErr.MsgCode = Convert.ToInt32(enErrorCode.SignalR_LimitPerTransactionMaxExceed);
                                ActivityNotificationErr.Param1 = MasterConfig.MaxAmount.ToString();
                                ActivityNotificationErr.Type = Convert.ToInt16(EnNotificationType.Info);
                                Task.Run(() => _signalRService.SendActivityNotificationV2(ActivityNotificationErr, Userid.ToString(), 2));
                                #endregion
                            }
                        }
                        else
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ReturnMsg = EnResponseMessage.LimitPerHourMaxExceed;
                            Response.ReturnMsg = Response.ReturnMsg.Replace("@Limit", MasterConfig.HourlyTrnAmount.ToString());
                            Response.ErrorCode = enErrorCode.LimitPerHourMaxExceed;
                            #region
                            ActivityNotificationMessage ActivityNotificationErr = new ActivityNotificationMessage();
                            ActivityNotificationErr.MsgCode = Convert.ToInt32(enErrorCode.SignalR_LimitPerHourMaxExceed);
                            ActivityNotificationErr.Param1 = MasterConfig.HourlyTrnAmount.ToString();
                            ActivityNotificationErr.Type = Convert.ToInt16(EnNotificationType.Info);
                            Task.Run(() => _signalRService.SendActivityNotificationV2(ActivityNotificationErr, Userid.ToString(), 2));
                            #endregion
                        }
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.LimitPerDayMaxExceed;
                        Response.ReturnMsg = Response.ReturnMsg.Replace("@Limit", MasterConfig.DailyTrnAmount.ToString());
                        Response.ErrorCode = enErrorCode.LimitPerDayMaxExceed;
                        #region
                        ActivityNotificationMessage ActivityNotificationErr = new ActivityNotificationMessage();
                        ActivityNotificationErr.MsgCode = Convert.ToInt32(enErrorCode.SignalR_LimitPerDayMaxExceed);
                        ActivityNotificationErr.Param1 = MasterConfig.DailyTrnAmount.ToString();
                        ActivityNotificationErr.Type = Convert.ToInt16(EnNotificationType.Info);
                        Task.Run(() => _signalRService.SendActivityNotificationV2(ActivityNotificationErr, Userid.ToString(), 2));
                        #endregion
                    }
                }
                else
                {
                    if ((request.LimitPerDay <= MasterConfig.DailyTrnAmount) || (MasterConfig.DailyTrnAmount == 0))
                    {
                        if ((request.LimitPerHour <= MasterConfig.HourlyTrnAmount) || (MasterConfig.HourlyTrnAmount == 0))
                        {
                            if ((request.LimitPerTransaction <= MasterConfig.MaxAmount || request.LimitPerTransaction == 0) || (MasterConfig.MaxAmount == 0))
                            {
                                if ((request.LimitPerTransaction > MasterConfig.MinAmount || request.LimitPerTransaction == 0) || (MasterConfig.MinAmount == 0))
                                {
                                    IsExist.LimitPerHour = request.LimitPerHour;
                                    IsExist.LimitPerDay = request.LimitPerDay;
                                    IsExist.LifeTime = request.LifeTime;
                                    IsExist.LimitPerTransaction = request.LimitPerTransaction;
                                    IsExist.StartTimeUnix = (request.StartTimeUnix == null ? 0 : request.StartTimeUnix);
                                    IsExist.EndTimeUnix = (request.EndTimeUnix == null ? 0 : request.EndTimeUnix);
                                    IsExist.UpdatedBy = Userid;
                                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                                    _LimitcommonRepository.UpdateWithAuditLog(IsExist);
                                    activityLog.Remarks = "New Limit Created For " + request.TrnType.ToString();
                                    _UserActivityLogCommonRepo.Add(activityLog);


                                    Response.ReturnMsg = EnResponseMessage.SetWalletLimitUpdateMsg;
                                    Response.ReturnCode = enResponseCode.Success;
                                    Response.ErrorCode = enErrorCode.Success;
                                    #region
                                    ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                                    ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.UWalletLimitNotification);
                                    ActivityNotification.Param1 = walletMasters.Walletname;
                                    ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);
                                    Task.Run(() => _signalRService.SendActivityNotificationV2(ActivityNotification, Userid.ToString(), 2));
                                    #endregion
                                }
                                else
                                {
                                    Response.ReturnCode = enResponseCode.Fail;
                                    Response.ReturnMsg = EnResponseMessage.LimitPerTransactionMinExceed;
                                    Response.ReturnMsg = Response.ReturnMsg.Replace("@Limit", MasterConfig.MinAmount.ToString());
                                    Response.ErrorCode = enErrorCode.MLimitPerTransactionMinExceed;
                                    #region
                                    ActivityNotificationMessage ActivityNotificationErr = new ActivityNotificationMessage();
                                    ActivityNotificationErr.MsgCode = Convert.ToInt32(enErrorCode.SignalR_LimitPerTransactionMinExceed);
                                    ActivityNotificationErr.Param1 = MasterConfig.MinAmount.ToString();
                                    ActivityNotificationErr.Type = Convert.ToInt16(EnNotificationType.Info);
                                    Task.Run(() => _signalRService.SendActivityNotificationV2(ActivityNotificationErr, Userid.ToString(), 2));
                                    #endregion
                                }
                            }
                            else
                            {
                                Response.ReturnCode = enResponseCode.Fail;
                                Response.ReturnMsg = EnResponseMessage.LimitPerTransactionMaxExceed;
                                Response.ReturnMsg = Response.ReturnMsg.Replace("@Limit", MasterConfig.MaxAmount.ToString());
                                Response.ErrorCode = enErrorCode.MLimitPerTransactionMaxExceed;
                                #region
                                ActivityNotificationMessage ActivityNotificationErr = new ActivityNotificationMessage();
                                ActivityNotificationErr.MsgCode = Convert.ToInt32(enErrorCode.SignalR_LimitPerTransactionMaxExceed);
                                ActivityNotificationErr.Param1 = MasterConfig.MaxAmount.ToString();
                                ActivityNotificationErr.Type = Convert.ToInt16(EnNotificationType.Info);
                                Task.Run(() => _signalRService.SendActivityNotificationV2(ActivityNotificationErr, Userid.ToString(), 2));
                                #endregion
                            }
                        }
                        else
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ReturnMsg = EnResponseMessage.LimitPerHourMaxExceed;
                            Response.ReturnMsg = Response.ReturnMsg.Replace("@Limit", MasterConfig.HourlyTrnAmount.ToString());
                            Response.ErrorCode = enErrorCode.MLimitPerHourMaxExceed;
                            #region
                            ActivityNotificationMessage ActivityNotificationErr = new ActivityNotificationMessage();
                            ActivityNotificationErr.MsgCode = Convert.ToInt32(enErrorCode.SignalR_LimitPerHourMaxExceed);
                            ActivityNotificationErr.Param1 = MasterConfig.HourlyTrnAmount.ToString();
                            ActivityNotificationErr.Type = Convert.ToInt16(EnNotificationType.Info);
                            Task.Run(() => _signalRService.SendActivityNotificationV2(ActivityNotificationErr, Userid.ToString(), 2));
                            #endregion
                        }
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.LimitPerDayMaxExceed;
                        Response.ReturnMsg = Response.ReturnMsg.Replace("@Limit", MasterConfig.DailyTrnAmount.ToString());
                        Response.ErrorCode = enErrorCode.MLimitPerDayMaxExceed;
                        #region
                        ActivityNotificationMessage ActivityNotificationErr = new ActivityNotificationMessage();
                        ActivityNotificationErr.MsgCode = Convert.ToInt32(enErrorCode.SignalR_LimitPerDayMaxExceed);
                        ActivityNotificationErr.Param1 = MasterConfig.DailyTrnAmount.ToString();
                        ActivityNotificationErr.Type = Convert.ToInt16(EnNotificationType.Info);
                        Task.Run(() => _signalRService.SendActivityNotificationV2(ActivityNotificationErr, Userid.ToString(), 2));
                        #endregion
                    }
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public LimitResponse GetWalletLimitConfig(string accWalletID)
        {
            LimitResponse LimitResponse = new LimitResponse();

            try
            {
                var WalletLimitResponse = _walletRepository1.GetWalletLimitResponse(accWalletID);
                if (WalletLimitResponse.Count == 0)
                {
                    LimitResponse.ReturnCode = enResponseCode.Fail;
                    LimitResponse.ReturnMsg = EnResponseMessage.NotFound;
                    LimitResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    LimitResponse.WalletLimitConfigurationRes = WalletLimitResponse;
                    LimitResponse.ReturnCode = enResponseCode.Success;
                    LimitResponse.ErrorCode = enErrorCode.Success;
                    LimitResponse.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return LimitResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                LimitResponse.ReturnCode = enResponseCode.InternalError;
                return LimitResponse;
            }
        }

        //vsolanki 24-10-2018
        public ListBalanceResponse GetAvailableBalance(long userid, string walletId)
        {
            ListBalanceResponse Response = new ListBalanceResponse();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var wallet = _commonRepository.GetSingle(item => item.AccWalletID == walletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (wallet == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.InvalidWalletId;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }
                var response = _walletRepository1.GetAvailableBalance(userid, wallet.Id);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.Response = response;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        //vsolanki 24-10-2018
        public TotalBalanceRes GetAllAvailableBalance(long userid)
        {
            TotalBalanceRes Response = new TotalBalanceRes();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                var response = _walletRepository1.GetAllAvailableBalance(userid);
                decimal total = _walletRepository1.NewGetTotalAvailbleBal(userid);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.Response = response;
                Response.TotalBalance = total;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //Rushabh 13-12-2019 Added New Method As Per Client New Requirement
        public ListAllBalanceTypeWiseResLat GetAllBalancesTypeWiseLat(long userId, string WalletType)
        {
            try
            {
                ListAllBalanceTypeWiseResLat res = new ListAllBalanceTypeWiseResLat();

                List<AllBalanceTypeWiseResLat> Response = new List<AllBalanceTypeWiseResLat>();
                res.BizResponseObj = new BizResponseClass();

                var listWallet = _walletRepository1.ListWalletMasterResponse(userId);
                if (listWallet.Count() == 0)
                {
                    res.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    res.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    res.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    return res;
                }
                for (int i = 0; i <= listWallet.Count - 1; i++)
                {
                    AllBalanceTypeWiseResLat a = new AllBalanceTypeWiseResLat();
                    a.Balance = new BalanceLat();
                    var wallet = _commonRepository.GetSingle(item => item.AccWalletID == listWallet[i].AccWalletID && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                    var response = _walletRepository1.GetAllBalancesV1Lat(userId, wallet.Id);

                    a.AccWalletID = listWallet[i].AccWalletID;
                    a.PublicAddress = listWallet[i].PublicAddress;
                    a.WalletName = listWallet[i].WalletName;
                    a.IsDefaultWallet = listWallet[i].IsDefaultWallet;
                    a.TypeName = listWallet[i].CoinName;

                    a.Balance = response;
                    Response.Add(a);
                }
                if (Response.Count() == 0)
                {
                    res.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    res.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    res.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    return res;
                }
                decimal total = _walletRepository1.NewGetTotalAvailbleBal(userId);
                res.Wallets = Response;
                res.TotalBalance = total;
                res.BizResponseObj.ReturnCode = enResponseCode.Success;
                res.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                res.BizResponseObj.ErrorCode = enErrorCode.Success;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //vsolanki 24-10-2018
        public ListBalanceResponse GetUnSettledBalance(long userid, string walletId)
        {
            ListBalanceResponse Response = new ListBalanceResponse();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var wallet = _commonRepository.GetSingle(item => item.AccWalletID == walletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (wallet == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.InvalidWalletId;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }
                var response = _walletRepository1.GetUnSettledBalance(userid, wallet.Id);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.Response = response;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        //vsolanki 24-10-2018
        public ListBalanceResponse GetAllUnSettledBalance(long userid)
        {
            ListBalanceResponse Response = new ListBalanceResponse();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                var response = _walletRepository1.GetAllUnSettledBalance(userid);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.Response = response;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        //vsolanki 24-10-2018
        public ListBalanceResponse GetUnClearedBalance(long userid, string walletId)
        {
            ListBalanceResponse Response = new ListBalanceResponse();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var wallet = _commonRepository.GetSingle(item => item.AccWalletID == walletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (wallet == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.InvalidWalletId;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }
                var response = _walletRepository1.GetUnClearedBalance(userid, wallet.Id);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.Response = response;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        //vsolanki 24-10-2018
        public ListBalanceResponse GetAllUnClearedBalance(long userid)
        {
            ListBalanceResponse Response = new ListBalanceResponse();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                var response = _walletRepository1.GetUnAllClearedBalance(userid);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.Response = response;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        //vsolanki 24-10-2018
        public ListStackingBalanceRes GetStackingBalance(long userid, string walletId)
        {
            ListStackingBalanceRes Response = new ListStackingBalanceRes();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var wallet = _commonRepository.GetSingle(item => item.AccWalletID == walletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (wallet == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.InvalidWalletId;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }
                var response = _walletRepository1.GetStackingBalance(userid, wallet.Id);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.Response = response;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        //vsolanki 24-10-2018
        public ListStackingBalanceRes GetAllStackingBalance(long userid)
        {
            ListStackingBalanceRes Response = new ListStackingBalanceRes();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                var response = _walletRepository1.GetAllStackingBalance(userid);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.Response = response;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        //vsolanki 24-10-2018
        public ListBalanceResponse GetShadowBalance(long userid, string walletId)
        {
            ListBalanceResponse Response = new ListBalanceResponse();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var wallet = _commonRepository.GetSingle(item => item.AccWalletID == walletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (wallet == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.InvalidWalletId;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }
                var response = _walletRepository1.GetShadowBalance(userid, wallet.Id);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.Response = response;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        //vsolanki 24-10-2018
        public ListBalanceResponse GetAllShadowBalance(long userid)
        {
            ListBalanceResponse Response = new ListBalanceResponse();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                var response = _walletRepository1.GetAllShadowBalance(userid);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.Response = response;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        //vsolanki 24-10-2018
        public AllBalanceResponse GetAllBalances(long userid, string walletId)
        {

            AllBalanceResponse allBalanceResponse = new AllBalanceResponse();
            allBalanceResponse.BizResponseObj = new BizResponseClass();
            try
            {
                //2019-2-18 added condi for only used trading
                var wallet = _commonRepository.GetSingle(item => item.AccWalletID == walletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (wallet == null)
                {
                    allBalanceResponse.BizResponseObj.ErrorCode = enErrorCode.InvalidWallet;
                    allBalanceResponse.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    allBalanceResponse.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return allBalanceResponse;
                }
                var response = _walletRepository1.GetAllBalances(userid, wallet.Id);
                if (response == null)
                {
                    allBalanceResponse.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    allBalanceResponse.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    allBalanceResponse.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return allBalanceResponse;
                }
                allBalanceResponse.BizResponseObj.ReturnCode = enResponseCode.Success;
                allBalanceResponse.BizResponseObj.ErrorCode = enErrorCode.Success;
                allBalanceResponse.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                allBalanceResponse.Balance = response;
                //vsolanki 2018-10-27 //for withdraw limit
                var limit = _LimitcommonRepository.GetSingle(item => item.TrnType == Convert.ToInt64(enWalletTrnType.Withdrawal) && item.WalletId == wallet.Id);
                if (limit == null)
                {
                    allBalanceResponse.WithdrawalDailyLimit = 0;
                }
                else if (limit.LimitPerDay < 0) //ntrivedi 21-11-2018 if limit null then exception so add else if instead of only if
                {
                    allBalanceResponse.WithdrawalDailyLimit = 0;
                }
                else
                {
                    allBalanceResponse.WithdrawalDailyLimit = limit.LimitPerDay;

                }
                var walletType = _WalletTypeMasterRepository.GetById(wallet.WalletTypeID);
                allBalanceResponse.WalletType = walletType.WalletTypeName;
                allBalanceResponse.WalletName = wallet.Walletname;
                allBalanceResponse.IsDefaultWallet = wallet.IsDefaultWallet;
                return allBalanceResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BeneficiaryResponse AddBeneficiary(string CoinName, short WhitelistingBit, string Name, string BeneficiaryAddress, long UserId, string Token)
        {
            BeneficiaryMaster IsExist = new BeneficiaryMaster();
            BeneficiaryResponse Response = new BeneficiaryResponse();
            UserActivityLog activityLog = new UserActivityLog();
            try
            {
                var userPreference = _UserPreferencescommonRepository.GetSingle(item => item.UserID == UserId);
                var walletMasters = _WalletTypeMasterRepository.GetSingle(item => item.WalletTypeName == CoinName);
                Response.BizResponse = new BizResponseClass();
                if (walletMasters == null)
                {
                    Response.BizResponse.ReturnCode = enResponseCode.Fail;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.InvalidWallet;
                    Response.BizResponse.ErrorCode = enErrorCode.InvalidWalletId;
                    return Response;
                }
                IsExist = _BeneficiarycommonRepository.GetSingle(item => item.UserID == UserId && item.Address == BeneficiaryAddress && item.WalletTypeID == walletMasters.Id);


                int cnt = _walletRepository1.IsSelfAddress(BeneficiaryAddress, UserId, walletMasters.WalletTypeName);
                if (cnt > 0)
                {
                    Response.BizResponse.ReturnCode = enResponseCode.Fail;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.SelfAddressFound;
                    Response.BizResponse.ErrorCode = enErrorCode.SelfAddressFound;
                    return Response;
                }

                if (IsExist == null)
                {
                    BeneficiaryMaster AddNew = new BeneficiaryMaster();
                    //if (userPreference != null)
                    //{
                    //    if (userPreference.IsWhitelisting == 1)
                    //    {
                    //        AddNew.IsWhiteListed = 1;
                    //    }
                    //}
                    //else
                    //{
                    //    AddNew.IsWhiteListed = 0;
                    //}
                    AddNew.Status = Convert.ToInt16(ServiceStatus.Active);
                    AddNew.CreatedBy = UserId;
                    AddNew.CreatedDate = Helpers.UTC_To_IST();
                    AddNew.UserID = UserId;
                    AddNew.Address = BeneficiaryAddress;
                    AddNew.Name = Name;
                    AddNew.IsWhiteListed = WhitelistingBit;
                    AddNew.WalletTypeID = walletMasters.Id;
                    AddNew = _BeneficiarycommonRepository.Add(AddNew);

                    activityLog.ActivityType = Convert.ToInt16(EnUserActivityType.AddBeneficiary);
                    activityLog.CreatedBy = UserId;
                    activityLog.CreatedDate = Helpers.UTC_To_IST();
                    activityLog.UserID = UserId;
                    activityLog.WalletID = walletMasters.Id;
                    activityLog.Remarks = "New Beneficiary Added For " + CoinName.ToString();
                    _UserActivityLogCommonRepo.Add(activityLog);

                    #region EMAIL_SMS
                    ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                    ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.AddBeneNotification);
                    ActivityNotification.Param1 = walletMasters.WalletTypeName;
                    ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);
                    Task.Run(() => _signalRService.SendActivityNotificationV2(ActivityNotification, UserId.ToString(), 2));

                    Response.BizResponse.ReturnMsg = EnResponseMessage.RecordAdded;
                    Response.BizResponse.ErrorCode = enErrorCode.Success;
                    Response.BizResponse.ReturnCode = enResponseCode.Success;

                    Parallel.Invoke(() => SMSSendAsyncV1(EnTemplateType.SMS_WalletBeneficiaryAdded, UserId.ToString(), walletMasters.WalletTypeName), () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletBeneficiaryAdded, UserId.ToString(), CoinName, Name, (WhitelistingBit == 1 ? "On" : "Off"), BeneficiaryAddress, Helpers.UTC_To_IST().ToString()));
                    #endregion
                }
                else
                {
                    //if (IsExist.Status == 9)
                    //{
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    IsExist.Status = Convert.ToInt16(ServiceStatus.Active);
                    IsExist.IsWhiteListed = WhitelistingBit;
                    IsExist.Name = Name;
                    _BeneficiarycommonRepository.UpdateWithAuditLog(IsExist);

                    activityLog.ActivityType = Convert.ToInt16(EnUserActivityType.AddBeneficiary);
                    activityLog.CreatedBy = UserId;
                    activityLog.CreatedDate = Helpers.UTC_To_IST();
                    activityLog.UserID = UserId;
                    activityLog.WalletID = walletMasters.Id;
                    activityLog.Remarks = "New Beneficiary Added For " + CoinName.ToString();
                    _UserActivityLogCommonRepo.Add(activityLog);

                    #region EMAIL_SMS
                    ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                    ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.AddBeneNotification);
                    ActivityNotification.Param1 = walletMasters.WalletTypeName;
                    ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);
                    Task.Run(() => _signalRService.SendActivityNotificationV2(ActivityNotification, UserId.ToString(), 2));

                    Response.BizResponse.ReturnMsg = EnResponseMessage.RecordAdded;
                    Response.BizResponse.ErrorCode = enErrorCode.Success;
                    Response.BizResponse.ReturnCode = enResponseCode.Success;

                    Parallel.Invoke(() => SMSSendAsyncV1(EnTemplateType.SMS_WalletBeneficiaryAdded, UserId.ToString(), walletMasters.WalletTypeName), () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletBeneficiaryAdded, UserId.ToString(), CoinName, Name, (WhitelistingBit == 1 ? "On" : "Off"), BeneficiaryAddress, Helpers.UTC_To_IST().ToString()));
                    #endregion
                    //}
                    //else
                    //{
                    //    Response.BizResponse.ReturnMsg = EnResponseMessage.AlredyExist;
                    //    Response.BizResponse.ErrorCode = enErrorCode.AlredyExist;
                    //    Response.BizResponse.ReturnCode = enResponseCode.Fail;
                    //}
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BeneficiaryResponse1 ListWhitelistedBeneficiary(string AccWalletID, long UserId, short? IsInternalAddress)
        {
            BeneficiaryResponse1 Response = new BeneficiaryResponse1();
            Response.BizResponse = new BizResponseClass();
            try
            {
                //2019-2-18 added condi for only used trading
                var walletMasters = _commonRepository.GetSingle(item => item.AccWalletID == AccWalletID && item.UserID == UserId && item.Status == Convert.ToInt16(ServiceStatus.Active) && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

                if (walletMasters == null)
                {
                    Response.BizResponse.ReturnCode = enResponseCode.Fail;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.InvalidWallet;
                    Response.BizResponse.ErrorCode = enErrorCode.InvalidWalletId;
                    return Response;
                }
                var BeneficiaryMasterRes = _walletRepository1.GetAllWhitelistedBeneficiaries(walletMasters.WalletTypeID, walletMasters.UserID, IsInternalAddress, walletMasters.Id);
                if (BeneficiaryMasterRes.Count == 0)
                {
                    Response.BizResponse.ReturnCode = enResponseCode.Fail;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.NotFound;
                    Response.BizResponse.ErrorCode = enErrorCode.NotFound;
                    return Response;
                }
                else
                {
                    Response.Beneficiaries = BeneficiaryMasterRes;
                    Response.BizResponse.ReturnCode = enResponseCode.Success;
                    Response.BizResponse.ErrorCode = enErrorCode.Success;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    return Response;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BeneficiaryResponse ListBeneficiary(long UserId)
        {
            BeneficiaryResponse Response = new BeneficiaryResponse();
            Response.BizResponse = new BizResponseClass();
            try
            {
                var BeneficiaryMasterRes = _walletRepository1.GetAllBeneficiaries(UserId);
                if (BeneficiaryMasterRes.Count == 0)
                {
                    Response.BizResponse.ReturnCode = enResponseCode.Fail;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.NotFound;
                    Response.BizResponse.ErrorCode = enErrorCode.NotFound;
                    return Response;
                }
                else
                {
                    Response.Beneficiaries = BeneficiaryMasterRes;
                    Response.BizResponse.ReturnCode = enResponseCode.Success;
                    Response.BizResponse.ErrorCode = enErrorCode.Success;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    return Response;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //vsolanki 25-10-2018
        public BalanceResponseWithLimit GetAvailbleBalTypeWise(long userid)
        {
            BalanceResponseWithLimit Response = new BalanceResponseWithLimit();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                var response = _walletRepository1.GetAvailbleBalTypeWise(userid);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                decimal total = _walletRepository1.NewGetTotalAvailbleBal(userid);
                //vsolanki 26-10-2018
                var walletType = _WalletTypeMasterRepository.GetSingle(item => item.IsDefaultWallet == 1);
                if (walletType == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.InvalidCoinName;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidCoin;
                    return Response;
                }
                //2019-2-15 added condi for only used trading wallet
                var wallet = _commonRepository.GetSingle(item => item.IsDefaultWallet == 1 && item.WalletTypeID == walletType.Id && item.UserID == userid && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (wallet == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.InvalidWallet;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }

                var limit = _LimitcommonRepository.GetSingle(item => item.TrnType == Convert.ToInt32(enWalletLimitType.TradingLimit) && item.WalletId == wallet.Id);//for withdraw

                if (limit == null)
                {
                    var masterLimit = _walletTrnLimitConfiguration.GetSingle(item => item.TrnType == Convert.ToInt32(enWalletLimitType.TradingLimit) && item.WalletType == wallet.WalletTypeID);

                    Response.DailyLimit = (masterLimit == null ? 0 : masterLimit.DailyTrnAmount);
                }
                else
                {
                    Response.DailyLimit = limit.LimitPerDay;
                }
                //get amt from  tq
                var amt = _walletRepository1.GetTodayAmountOfTQ(userid, wallet.Id);

                if (response.Count == 0)
                {
                    Response.UsedLimit = 0;
                }
                else
                {
                    Response.UsedLimit = amt;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.Response = response;
                Response.TotalBalance = total;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //vsolanki 25-10-2018
        public ListAllBalanceTypeWiseRes GetAllBalancesTypeWise(long userId, string WalletType)
        {
            try
            {
                ListAllBalanceTypeWiseRes res = new ListAllBalanceTypeWiseRes();

                List<AllBalanceTypeWiseRes> Response = new List<AllBalanceTypeWiseRes>();
                res.BizResponseObj = new Core.ApiModels.BizResponseClass();

                var listWallet = _walletRepository1.GetWalletMasterResponseByCoin(userId, WalletType);
                if (listWallet.Count() == 0)
                {
                    res.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    res.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    res.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    return res;
                }
                for (int i = 0; i <= listWallet.Count - 1; i++)
                {
                    AllBalanceTypeWiseRes a = new AllBalanceTypeWiseRes();
                    a.Wallet = new WalletResponse();
                    a.Wallet.Balance = new Balance();
                    //2019-2-18 added condi for only used trading wallet
                    var wallet = _commonRepository.GetSingle(item => item.AccWalletID == listWallet[i].AccWalletID && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                    var response = _walletRepository1.GetAllBalancesV1(userId, wallet.Id);

                    a.Wallet.AccWalletID = listWallet[i].AccWalletID;
                    a.Wallet.PublicAddress = listWallet[i].PublicAddress;
                    a.Wallet.WalletName = listWallet[i].WalletName;
                    a.Wallet.IsDefaultWallet = listWallet[i].IsDefaultWallet;
                    a.Wallet.TypeName = listWallet[i].CoinName;

                    a.Wallet.Balance = response;
                    Response.Add(a);
                }
                if (Response.Count() == 0)
                {
                    res.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    res.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    res.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    return res;
                }
                res.Wallets = Response;
                res.BizResponseObj.ReturnCode = enResponseCode.Success;
                res.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                res.BizResponseObj.ErrorCode = enErrorCode.Success;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public UserPreferencesRes SetPreferences(long Userid, int GlobalBit, string Token)
        {
            UserPreferencesRes Response = new UserPreferencesRes();
            Response.BizResponse = new BizResponseClass();
            try
            {
                UserPreferencesMaster IsExist = _UserPreferencescommonRepository.GetSingle(item => item.UserID == Userid);
                if (IsExist == null)
                {
                    UserPreferencesMaster newobj = new UserPreferencesMaster();
                    UserActivityLog activityLog = new UserActivityLog();
                    newobj.UserID = Userid;
                    newobj.IsWhitelisting = Convert.ToInt16(GlobalBit);
                    newobj.CreatedBy = Userid;
                    newobj.CreatedDate = Helpers.UTC_To_IST();
                    newobj.Status = Convert.ToInt16(ServiceStatus.Active);
                    newobj = _UserPreferencescommonRepository.Add(newobj);

                    activityLog.ActivityType = Convert.ToInt16(EnUserActivityType.AddBeneficiary);
                    activityLog.CreatedBy = Userid;
                    activityLog.CreatedDate = Helpers.UTC_To_IST();
                    activityLog.UserID = Userid;
                    activityLog.Remarks = "User Preference is set to " + GlobalBit.ToString();
                    _UserActivityLogCommonRepo.Add(activityLog);


                    Response.BizResponse.ReturnCode = enResponseCode.Success;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.SetUserPrefSuccessMsg;
                    Response.BizResponse.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    IsExist.IsWhitelisting = Convert.ToInt16(GlobalBit);
                    IsExist.UpdatedBy = Userid;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    _UserPreferencescommonRepository.UpdateWithAuditLog(IsExist);
                    Response.BizResponse.ReturnMsg = EnResponseMessage.SetUserPrefUpdateMsg;
                    Response.BizResponse.ErrorCode = enErrorCode.Success;
                }
                Response.BizResponse.ReturnCode = enResponseCode.Success;
                #region SMS_Email
                ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.UserPreferencesNotification);
                if (GlobalBit == 1)
                {
                    ActivityNotification.Param1 = "ON";
                }
                else
                {
                    ActivityNotification.Param1 = "OFF";
                }
                ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);
                Parallel.Invoke(() => _signalRService.SendActivityNotificationV2(ActivityNotification, Userid.ToString(), 2), () => SMSSendAsyncV1(EnTemplateType.SMS_WhitelistingOnOff, Userid.ToString(), null, null, null, (GlobalBit == 1) ? "On" : "Off"), () => EmailSendAsyncV1(EnTemplateType.EMAIL_WhitelistingOnOff, Userid.ToString(), (GlobalBit == 1) ? "On" : "Off"));
                #endregion
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public UserPreferencesRes GetPreferences(long Userid)
        {
            UserPreferencesRes Response = new UserPreferencesRes();
            Response.BizResponse = new BizResponseClass();
            try
            {
                UserPreferencesMaster IsExist = _UserPreferencescommonRepository.GetSingle(item => item.UserID == Userid);
                if (IsExist == null)
                {
                    Response.BizResponse.ReturnCode = enResponseCode.Fail;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.NotFound;
                    Response.BizResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    Response.IsWhitelisting = IsExist.IsWhitelisting;
                    Response.UserID = IsExist.UserID;
                    Response.BizResponse.ReturnCode = enResponseCode.Success;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.BizResponse.ErrorCode = enErrorCode.Success;
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BeneficiaryResponse UpdateBulkBeneficiary(BulkBeneUpdateReq Request, long ID, string Token)
        {
            BeneficiaryResponse Response = new BeneficiaryResponse();
            Response.BizResponse = new BizResponseClass();
            try
            {
                string beneid = string.Join(",", Request.ID);//Request.ID.ToString();
                short WhitelistingBit = Request.WhitelistingBit;
                var state = _walletRepository1.BeneficiaryBulkEdit(beneid, WhitelistingBit);
                if (state.AffectedRows > 0)
                {
                    ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                    if (Request.WhitelistingBit == 1)
                    {
                        ActivityNotification.Param1 = "Activated";
                        ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.UpdateBeneNotificationActive);
                        Response.BizResponse.ReturnMsg = EnResponseMessage.RecordUpdated;
                    }
                    else if (Request.WhitelistingBit == 9)
                    {
                        ActivityNotification.Param1 = "Deleted";
                        ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.UpdateBeneNotificationActive);
                        Response.BizResponse.ReturnMsg = EnResponseMessage.RecordDeleted;
                    }
                    else
                    {
                        ActivityNotification.Param1 = "Inactivated";
                        ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.UpdateBeneNotificationActive);
                        Response.BizResponse.ReturnMsg = EnResponseMessage.RecordDisable;
                    }
                    ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);
                    _signalRService.SendActivityNotificationV2(ActivityNotification, ID.ToString(), 2);

                    Response.BizResponse.ReturnCode = enResponseCode.Success;
                    Response.BizResponse.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Response.BizResponse.ReturnCode = enResponseCode.Fail;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.NotFound;
                    Response.BizResponse.ErrorCode = enErrorCode.InvalidBeneficiaryID;
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BeneficiaryResponse UpdateBeneficiaryDetails(BeneficiaryUpdateReq request, string AccWalletID, long UserID, string Token)
        {
            BeneficiaryResponse Response = new BeneficiaryResponse();
            BeneficiaryMaster IsExist = new BeneficiaryMaster();
            Response.BizResponse = new BizResponseClass();
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var walletMasters = _commonRepository.GetSingle(item => item.AccWalletID == AccWalletID && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (walletMasters == null)
                {
                    Response.BizResponse.ReturnCode = enResponseCode.Fail;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.InvalidWallet;
                    Response.BizResponse.ErrorCode = enErrorCode.InvalidWalletId;
                    return Response;
                }
                IsExist = _BeneficiarycommonRepository.GetSingle(item => item.Id == request.BenefifiaryID && item.WalletTypeID == walletMasters.WalletTypeID && item.UserID == UserID);
                var walletType = _WalletTypeMasterRepository.GetSingle(i => i.Id == walletMasters.WalletTypeID);
                if (IsExist != null)
                {
                    IsExist.Name = request.Name;
                    IsExist.Status = Convert.ToInt16(request.Status);
                    IsExist.IsWhiteListed = Convert.ToInt16(request.WhitelistingBit);
                    IsExist.UpdatedBy = UserID;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    _BeneficiarycommonRepository.UpdateWithAuditLog(IsExist);

                    UserActivityLog activityLog = new UserActivityLog();
                    activityLog.ActivityType = Convert.ToInt16(EnUserActivityType.AddBeneficiary);
                    activityLog.CreatedBy = UserID;
                    activityLog.CreatedDate = Helpers.UTC_To_IST();
                    activityLog.UserID = UserID;
                    activityLog.WalletID = IsExist.Id;
                    activityLog.Remarks = "Beneficiary Updated";
                    _UserActivityLogCommonRepo.Add(activityLog);

                    Response.BizResponse.ReturnMsg = EnResponseMessage.RecordUpdated;
                    Response.BizResponse.ReturnCode = enResponseCode.Success;
                    Response.BizResponse.ErrorCode = enErrorCode.Success;
                    #region SMS_Email               
                    ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                    ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.UpBeneNotification);
                    ActivityNotification.Param1 = walletMasters.Walletname;
                    ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);

                    Parallel.Invoke(
                        () => _signalRService.SendActivityNotificationV2(ActivityNotification, UserID.ToString(), 2),
                        () => SMSSendAsyncV1(EnTemplateType.SMS_BeneficiaryUpdated, UserID.ToString(), walletType.WalletTypeName),
                        () => EmailSendAsyncV1(EnTemplateType.EMAIL_BeneficiaryUpdated, UserID.ToString(), walletType.WalletTypeName, (request.Name), (request.WhitelistingBit == 1 ? "On" : "Off"), IsExist.Address, Helpers.UTC_To_IST().ToString()));
                    #endregion

                }
                else
                {
                    Response.BizResponse.ReturnCode = enResponseCode.Fail;
                    Response.BizResponse.ReturnMsg = EnResponseMessage.NotFound;
                    Response.BizResponse.ErrorCode = enErrorCode.NotFound;
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public ListWalletLedgerRes GetWalletLedger(DateTime FromDate, DateTime ToDate, string WalletId, int page)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var wallet = _commonRepository.GetSingle(item => item.AccWalletID == WalletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

                ListWalletLedgerRes Response = new ListWalletLedgerRes();
                Response.BizResponseObj = new BizResponseClass();
                if (wallet == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.InvalidWallet;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }
                DateTime newToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                var wl = _walletRepository1.GetWalletLedger(FromDate, newToDate, wallet.Id, page);
                if (wl.Count() == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.WalletLedgers = wl;
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        //vsoalnki 26-10-2018
        public ListWalletLedgerResv1 GetWalletLedgerV1(DateTime FromDate, DateTime ToDate, string WalletId, int page, int PageSize)
        {
            try
            {
                var wallet = _commonRepository.GetSingle(item => item.AccWalletID == WalletId);

                ListWalletLedgerResv1 Response = new ListWalletLedgerResv1();
                Response.PageNo = page;
                Response.PageSize = PageSize;
                if (wallet == null)
                {
                    Response.ErrorCode = enErrorCode.InvalidWallet;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }
                DateTime newToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                int TotalCount = 0;
                var wl = _walletRepository1.GetWalletLedgerV1(FromDate, newToDate, wallet.Id, page + 1, PageSize, ref TotalCount);
                Response.TotalCount = TotalCount;
                if (wl.Count() == 0)
                {
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.WalletLedgers = wl;
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = EnResponseMessage.FindRecored;
                Response.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public ListWalletLedgerResponse GetWalletLedgerv2(DateTime FromDate, DateTime ToDate, string WalletId, int page, int PageSize)
        {
            try
            {
                var wallet = _commonRepository.GetSingle(item => item.AccWalletID == WalletId);

                ListWalletLedgerResponse Response = new ListWalletLedgerResponse();
                Response.PageNo = page;
                Response.PageSize = PageSize;
                if (wallet == null)
                {
                    Response.ErrorCode = enErrorCode.InvalidWallet;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }
                DateTime newToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                int TotalCount = 0;
                //var wl = _walletRepository1.GetWalletLedgerv2(FromDate, newToDate, wallet.Id, page + 1, PageSize, ref TotalCount);
                var wl = _walletSPRepositories.CallGetWalletLedger(FromDate, newToDate, wallet.Id, page + 1, PageSize, ref TotalCount);
                Response.TotalCount = TotalCount;
                if (wl.Count() == 0)
                {
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.WalletLedgers = wl;
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = EnResponseMessage.FindRecored;
                Response.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //vsolanki 27-10-2018
        public async Task<BizResponseClass> CreateDefaulWallet(long UserID, string accessToken = null)
        {
            try
            {
                var res1 = _walletRepository1.CreateDefaulWallet(UserID);
                var res = await res1;
                if (res != 1)
                {
                    return new BizResponseClass
                    {
                        ErrorCode = enErrorCode.InternalError,
                        ReturnMsg = EnResponseMessage.CreateWalletFailMsg,
                        ReturnCode = enResponseCode.Fail
                    };
                }
                AddBizUserTypeMappingReq req = new AddBizUserTypeMappingReq();
                req.UserID = UserID;
                req.UserType = enUserType.User;
                AddBizUserTypeMapping(req);

                UserActivityLog activityLog = new UserActivityLog();
                activityLog.ActivityType = Convert.ToInt16(EnUserActivityType.AddBeneficiary);
                activityLog.CreatedBy = UserID;
                activityLog.CreatedDate = Helpers.UTC_To_IST();
                activityLog.UserID = UserID;
                activityLog.Remarks = "Default Wallet Creation";
                _UserActivityLogCommonRepo.Add(activityLog);

                #region EMAIL_SMS
                ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.DefaultCreateWalletSuccessMsg);
                ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);
                _signalRService.SendActivityNotificationV2(ActivityNotification, UserID.ToString(), 2);
                SMSSendAsyncV1(EnTemplateType.SMS_DefaultWalletCreate, UserID.ToString());
                EmailSendAsyncV1(EnTemplateType.EMAIL_DefaultWalletCreate, UserID.ToString());
                //Task.Delay(5000);
                #endregion

                return new BizResponseClass
                {
                    ErrorCode = enErrorCode.Success,
                    ReturnMsg = EnResponseMessage.CreateWalletSuccessMsg,
                    ReturnCode = enResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return new BizResponseClass
                {
                    ErrorCode = enErrorCode.InternalError,
                    ReturnMsg = EnResponseMessage.CreateWalletFailMsg,
                    ReturnCode = enResponseCode.Fail
                };
            }
        }

        //vsolanki 27-10-2018
        public BizResponseClass CreateWalletForAllUser_NewService(string WalletType)
        {
            try
            {
                var walletType = _WalletTypeMasterRepository.GetSingle(item => item.WalletTypeName == WalletType);
                if (walletType == null)
                {
                    return new BizResponseClass
                    {
                        ErrorCode = enErrorCode.InvalidCoinName,
                        ReturnMsg = EnResponseMessage.InvalidCoin,
                        ReturnCode = enResponseCode.Fail
                    };
                }
                var res = _walletRepository1.CreateWalletForAllUser_NewService(WalletType);
                if (res != 1)
                {
                    return new BizResponseClass
                    {
                        ErrorCode = enErrorCode.InternalError,
                        ReturnMsg = EnResponseMessage.CreateWalletFailMsg,
                        ReturnCode = enResponseCode.InternalError
                    };
                }
                return new BizResponseClass
                {
                    ErrorCode = enErrorCode.Success,
                    ReturnMsg = EnResponseMessage.CreateWalletSuccessMsg,
                    ReturnCode = enResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //vsolanki 2018-10-29
        public BizResponseClass AddBizUserTypeMapping(AddBizUserTypeMappingReq req)
        {
            try
            {
                BizUserTypeMapping bizUser = new BizUserTypeMapping();
                bizUser.UserID = req.UserID;
                bizUser.UserType = Convert.ToInt16(req.UserType);
                var res = _walletRepository1.AddBizUserTypeMapping(bizUser);
                if (res == 0)
                {
                    return new BizResponseClass
                    {
                        ErrorCode = enErrorCode.DuplicateRecord,
                        ReturnMsg = EnResponseMessage.DuplicateRecord,
                        ReturnCode = enResponseCode.Fail
                    };
                }
                return new BizResponseClass
                {
                    ErrorCode = enErrorCode.Success,
                    ReturnMsg = EnResponseMessage.RecordAdded,
                    ReturnCode = enResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //vsolanki 2018-10-29
        public ListIncomingTrnRes GetIncomingTransaction(long Userid, string Coin)
        {
            try
            {
                ListIncomingTrnRes Response = new ListIncomingTrnRes();
                Response.BizResponseObj = new BizResponseClass();
                var depositHistories = _walletRepository1.GetIncomingTransaction(Userid, Coin);
                if (depositHistories.Count() == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.IncomingTransactions = depositHistories;
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }

        }

        public ListIncomingTrnResv2 GetIncomingTransactionv2(long Userid, string Coin)
        {
            try
            {
                ListIncomingTrnResv2 Response = new ListIncomingTrnResv2();
                Response.BizResponseObj = new BizResponseClass();
                var depositHistories = _walletRepository1.GetIncomingTransactionv2(Userid, Coin);
                if (depositHistories.Count() == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.IncomingTransactions = depositHistories;
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
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
                return false;
            }
        }

        public async Task<ServiceLimitChargeValue> GetServiceLimitChargeValue(enWalletTrnType TrnType, string CoinName)
        {
            try
            {
                ServiceLimitChargeValue response;
                WalletLimitConfiguration usrlimitObj = null;
                var walletType = await _WalletTypeMasterRepository.GetSingleAsync(x => x.WalletTypeName == CoinName);
                if (walletType != null)
                {
                    response = new ServiceLimitChargeValue();
                    var limitData = _walletTrnLimitConfiguration.GetSingle(x => x.TrnType == Convert.ToInt16(TrnType) && x.WalletType == walletType.Id);
                    var chargeData = _chargeRuleMaster.GetSingle(x => x.TrnType == TrnType && x.WalletType == walletType.Id);

                    if (chargeData != null) //ntrivedi 14-12-2018 make individual condition instead of end condition
                    {
                        response.ChargeType = chargeData.ChargeType;
                        response.ChargeValue = chargeData.ChargeValue;
                    }
                    else
                    {
                        response.ChargeType = enChargeType.Fixed;
                        response.ChargeValue = 0;
                    }
                    response.CoinName = walletType.WalletTypeName;

                    if (limitData != null && usrlimitObj != null)
                    {
                        response.TrnType = (enWalletTrnType)limitData.TrnType;
                        response.MinAmount = limitData.MinAmount;
                        if (limitData.MaxAmount >= usrlimitObj.LimitPerTransaction && usrlimitObj.LimitPerTransaction > 0)
                        {
                            response.MaxAmount = usrlimitObj.LimitPerTransaction;
                        }
                        else
                        {
                            response.MaxAmount = limitData.MaxAmount;
                        }
                    }
                    else if (limitData != null)
                    {
                        response.TrnType = (enWalletTrnType)limitData.TrnType;
                        response.MinAmount = limitData.MinAmount;
                        response.MaxAmount = limitData.MaxAmount;
                    }
                    else
                    {
                        response.TrnType = TrnType;
                        response.MinAmount = 0;
                        response.MaxAmount = 0;
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
                HelperForLog.WriteErrorLog("GetServiceLimitChargeValue", "WalletService", ex);
                return null;
            }
        }

        public async Task<ServiceLimitChargeValue> GetServiceLimitChargeValueV2(enWalletTrnType TrnType, string CoinName, long UserId)
        {
            try
            {
                ServiceLimitChargeValue response;
                WalletLimitConfiguration usrlimitObj = null;
                var walletType = await _WalletTypeMasterRepository.GetSingleAsync(x => x.WalletTypeName == CoinName);
                if (walletType != null)
                {
                    response = new ServiceLimitChargeValue();
                    var limitData = _walletTrnLimitConfiguration.GetSingle(x => x.TrnType == Convert.ToInt16(TrnType) && x.WalletType == walletType.Id);

                    var wallet = _commonRepository.GetSingle(i => i.WalletTypeID == walletType.Id && i.IsDefaultWallet == 1 && i.UserID == UserId && i.Status == 1 && i.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

                    if (wallet != null)
                    {
                        usrlimitObj = _LimitcommonRepository.GetSingle(x => x.TrnType == Convert.ToInt16(TrnType) && x.WalletId == wallet.Id);
                    }
                    var mstrchargeData = _ChargeConfigurationMaster.GetSingle(x => x.TrnType == Convert.ToInt64(TrnType) && x.WalletTypeID == walletType.Id);
                    if (mstrchargeData != null)
                    {
                        var chargeData = _ChargeConfigrationDetail.GetSingle(x => x.ChargeConfigurationMasterID == mstrchargeData.Id);
                        if (chargeData != null) //ntrivedi 14-12-2018 make individual condition instead of end condition
                        {
                            var ChargeWallet = _commonRepository.GetSingle(i => i.WalletTypeID == chargeData.DeductionWalletTypeId && i.IsDefaultWallet == 1 && i.UserID == UserId && i.Status == 1 && i.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                            if (ChargeWallet != null)
                            {
                                response.ChargeWalletBalance = ChargeWallet.Balance;
                            }
                            else
                            {
                                response.ChargeWalletBalance = 0;
                            }
                            var walletTypeObj = _WalletTypeMasterRepository.GetSingle(i => i.Id == chargeData.DeductionWalletTypeId);
                            if (walletTypeObj != null)
                            {
                                response.DeductWalletTypeName = walletTypeObj.WalletTypeName;
                            }
                            else
                            {
                                response.DeductWalletTypeName = "N/A";
                            }
                            response.ChargeType = (enChargeType)chargeData.ChargeType;
                            response.ChargeValue = chargeData.ChargeValue;
                        }
                        else
                        {
                            response.ChargeType = enChargeType.Fixed;
                            response.ChargeValue = 0;
                        }
                    }
                    response.CoinName = walletType.WalletTypeName;

                    if (limitData != null && usrlimitObj != null)
                    {
                        response.TrnType = (enWalletTrnType)limitData.TrnType;
                        response.MinAmount = limitData.MinAmount;
                        if (limitData.MaxAmount >= usrlimitObj.LimitPerTransaction && usrlimitObj.LimitPerTransaction > 0)
                        {
                            response.MaxAmount = usrlimitObj.LimitPerTransaction;
                        }
                        else
                        {
                            response.MaxAmount = limitData.MaxAmount;
                        }
                    }
                    else if (limitData != null)
                    {
                        response.TrnType = (enWalletTrnType)limitData.TrnType;
                        response.MinAmount = limitData.MinAmount;
                        response.MaxAmount = limitData.MaxAmount;
                    }
                    else
                    {
                        response.TrnType = TrnType;
                        response.MinAmount = 0;
                        response.MaxAmount = 0;
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
                HelperForLog.WriteErrorLog("GetServiceLimitChargeValueV2", "WalletService", ex);
                return null;
            }
        }

        public async Task<CreateWalletAddressRes> CreateETHAddress(string Coin, int AddressCount, long UserId, string token)
        {
            try
            {
                CreateWalletAddressRes addr = new CreateWalletAddressRes();
                var orgid = _walletRepository1.getOrgID();
                if (orgid != UserId)
                {
                    return new CreateWalletAddressRes { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.OrgIDNotFound, ErrorCode = enErrorCode.OrgIDNotFound };
                }
                var type = _WalletTypeMasterRepository.GetSingle(t => t.WalletTypeName == Coin);
                if (type == null)
                {
                    return new CreateWalletAddressRes { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidCoin, ErrorCode = enErrorCode.InvalidCoinName };
                }
                var walletObj = _commonRepository.FindBy(t => t.UserID == orgid && t.IsDefaultWallet == 1 && t.WalletTypeID == type.Id && t.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)).FirstOrDefault();
                if (walletObj == null)
                {
                    return new CreateWalletAddressRes { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet };
                }
                for (int i = 1; i <= AddressCount; i++)
                {
                    addr = await GenerateAddress(walletObj.AccWalletID, Coin, token, 1);
                }
                return addr;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateETHAddress", "WalletService", ex);
                return null;
            }
        }

        //vsolanki 2018-11-02
        public ListOutgoingTrnRes GetOutGoingTransaction(long Userid, string Coin)
        {
            try
            {
                ListOutgoingTrnRes Response = new ListOutgoingTrnRes();
                Response.BizResponseObj = new BizResponseClass();
                var type = _WalletTypeMasterRepository.GetSingle(i => i.WalletTypeName == Coin);
                if (Coin != null)
                {
                    if (type == null)
                    {
                        Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                        Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidCoin;
                        Response.BizResponseObj.ErrorCode = enErrorCode.InvalidCoinName;
                        return Response;
                    }
                }
                var Histories = _walletRepository1.GetOutGoingTransaction(Userid, Coin);
                if (Histories.Count() == 0 || Histories == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.OutGoingTransactions = Histories;
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }

        }

        public ListOutgoingTrnResv2 GetOutGoingTransactionv2(long Userid, string Coin)
        {
            try
            {
                ListOutgoingTrnResv2 Response = new ListOutgoingTrnResv2();
                Response.BizResponseObj = new BizResponseClass();
                var type = _WalletTypeMasterRepository.GetSingle(i => i.WalletTypeName == Coin);
                if (Coin != null)
                {
                    if (type == null)
                    {
                        Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                        Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidCoin;
                        Response.BizResponseObj.ErrorCode = enErrorCode.InvalidCoinName;
                        return Response;
                    }
                }
                var Histories = _walletRepository1.GetOutGoingTransactionv2(Userid, Coin);
                if (Histories.Count() == 0 || Histories == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.OutGoingTransactions = Histories;
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }

        }
        public bool InsertIntoWithdrawHistory(WithdrawHistory req)
        {
            try
            {
                if (req != null)
                {
                    _WithdrawHistoryRepository.Add(req);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
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

        public async Task<WalletDrCrResponse> GetWalletHoldNew(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, string Token = "", enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal, string MarketCurrency = "")
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
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTradeRefNo, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }
                if (amount <= 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidAmt, trnType);
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
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidWallet, trnType);
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
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficantBal, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }

                if (!CheckUserBalanceFlag)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedBalanceMismatch, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }

                HelperForLog.WriteLogIntoFileAsync("GetWalletHoldNew", "Check ShadowLimit done TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);
                //int count = await countTask;
                CheckTrnRefNoRes count1 = await countTask1;
                if (count1.TotalCount != 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.AlredyExist, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AlredyExist, ErrorCode = enErrorCode.AlredyExist, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }

                HelperForLog.WriteLogIntoFileAsync("GetWalletHoldNew", "CheckTrnRefNo TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);

                BizResponseClass bizResponse = _walletSPRepositories.Callsp_HoldWallet(dWalletobj, timestamp, serviceType, amount, coinName, allowedChannels, walletTypeMaster.Id, TrnRefNo, dWalletobj.Id, dWalletobj.UserID, routeTrnType, trnType, ref TrnNo, enWalletDeductionType, "", MarketCurrency);

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
                        Task.Run(() => WalletHoldNotificationSend(timestamp, dWalletobj, coinName, amount, TrnRefNo, (byte)routeTrnType, charge, walletId, ChargeWalletObj, ChargewalletType));
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
                //bool CheckUserCrBalanceFlag = false;
                //bool CheckUserDrBalanceFlag = false;
                //bool CheckUserCrBalanceFlag1 = false;
                //bool CheckUserDrBalanceFlag1 = false;
                bool checkDebitRefNo, checkDebitRefNo1;
                Task<bool> checkDebitRefNoTask, checkDebitRefNoTask1;
                BizResponseClass bizResponseClassFC, bizResponseClassSC;

                //Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal first currency", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString() + ",Amount=" + firstCurrObj.Amount + ",Coin=" + firstCurrObj.Coin + ", CR WalletID=" + firstCurrObj.creditObject.WalletId + ",Dr WalletID=" + firstCurrObj.debitObject.WalletId + " cr full settled=" + firstCurrObj.creditObject.isFullSettled.ToString() + ",Dr full settled=" + firstCurrObj.debitObject.isFullSettled.ToString() + ",Dr MarketTrade" + firstCurrObj.debitObject.isMarketTrade));
                //Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal second currency", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + secondCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + secondCurrObj.debitObject.TrnRefNo.ToString() + ",Amount=" + secondCurrObj.Amount + ",Coin=" + secondCurrObj.Coin + ", CR WalletID=" + secondCurrObj.creditObject.WalletId + ",Dr WalletID=" + secondCurrObj.debitObject.WalletId + " cr full settled=" + secondCurrObj.creditObject.isFullSettled.ToString() + ",Dr full settled=" + secondCurrObj.debitObject.isFullSettled.ToString() + ",Dr MarketTrade" + secondCurrObj.debitObject.isMarketTrade));

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal first currency", "WalletService", "timestamp:" + timestamp + Helpers.JsonSerialize(firstCurrObj)));
                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal second currency", "WalletService", "timestamp:" + timestamp + Helpers.JsonSerialize(secondCurrObj)));

                //secondCurrObj.debitObject.IsMaker = 1; // ntrivedi temperory 23-01-2019
                //secondCurrObj.creditObject.IsMaker = 2; // ntrivedi temperory 23-01-2019


                // check amount for both object
                // check coin name for both object
                // check refno for all 4 object
                // check walletid for all 4 object

                // call CheckTrnIDDrForHoldAsync for both debit trn object

                // check shadow balance for both debit walletid and amount
                //having sufficient balance for debit walletid both
                //wallet status for all walletid should be enable 

                //2019-2-18 added condi for only used trading wallet
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
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObj.creditObject.WalletId, firstCurrObj.Coin, firstCurrObj.creditObject.UserID, timestamp, enTransactionStatus.SystemFail, "Amount and DebitRefNo matching failure", firstCurrObj.creditObject.trnType);
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

                //ntrivedi moved to sp ntrivedi 08-07-2019 rita sometimes found settled balance mismatch so sp may run faster
                //Task<bool> CheckUserCrBalanceFlagTask = CheckUserBalanceAsync(firstCurrObj.Amount, firstCurrObj.creditObject.WalletId);
                Task<WalletTypeMaster> walletTypeFirstCurrTask = _WalletTypeMasterRepository.GetSingleAsync(e => e.WalletTypeName == firstCurrObj.Coin);
                firstCurrObjCrWM = await firstCurrObjCrWMTask;
                firstCurrObj.creditObject.UserID = firstCurrObjCrWM.UserID;

                firstCurrObjDrWM = await firstCurrObjDrWMTask;
                firstCurrObj.debitObject.UserID = firstCurrObjDrWM.UserID;
                //ntrivedi moved to sp ntrivedi 08-07-2019 rita sometimes found settled balance mismatch so sp may run faster
                //CheckUserCrBalanceFlag = await CheckUserCrBalanceFlagTask;
                //if (!CheckUserCrBalanceFlag)
                //{
                //    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, firstCurrObj.Amount, firstCurrObj.creditObject.TrnRefNo,Helpers.UTC_To_IST(), null, firstCurrObj.creditObject.WalletId, firstCurrObj.Coin, firstCurrObj.creditObject.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, firstCurrObj.creditObject.trnType);
                //    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                //    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.CrDrCredit_SettledBalMismatchCrWallet, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                //}
                //ntrivedi moved to sp ntrivedi 08-07-2019 rita sometimes found settled balance mismatch so sp may run faster
                //Task<bool> CheckUserDrBalanceFlagTask = CheckUserBalanceAsync(firstCurrObj.Amount, firstCurrObj.debitObject.WalletId, enBalanceType.OutBoundBalance);
                Task<WalletTypeMaster> walletTypeSecondCurrTask = _WalletTypeMasterRepository.GetSingleAsync(e => e.WalletTypeName == secondCurrObj.Coin);
                firstCurrObjCrWM = await firstCurrObjCrWMTask;
                firstCurrObj.creditObject.UserID = firstCurrObjCrWM.UserID;
                //ntrivedi moved to sp ntrivedi 08-07-2019 rita sometimes found settled balance mismatch so sp may run faster
                //CheckUserDrBalanceFlag = await CheckUserDrBalanceFlagTask;
                //if (!CheckUserDrBalanceFlag)
                //{
                //    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo,Helpers.UTC_To_IST(), null, firstCurrObj.debitObject.WalletId, firstCurrObj.Coin, firstCurrObj.debitObject.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, firstCurrObj.debitObject.trnType);
                //    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                //    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.CrDrCredit_SettledBalMismatchDrWallet, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                //}

                firstCurrObjDrWM = await firstCurrObjDrWMTask;
                firstCurrObj.debitObject.UserID = firstCurrObjDrWM.UserID;

                //ntrivedi moved to sp ntrivedi 08-07-2019 rita sometimes found settled balance mismatch so sp may run faster
                //Task<bool> CheckUserCrBalanceFlagTask1 = CheckUserBalanceAsync(secondCurrObj.Amount, secondCurrObj.creditObject.WalletId);

                firstCurrObjCrWM = await firstCurrObjCrWMTask;
                firstCurrObj.creditObject.UserID = firstCurrObjCrWM.UserID;

                firstCurrObjDrWM = await firstCurrObjDrWMTask;
                firstCurrObj.debitObject.UserID = firstCurrObjDrWM.UserID;
                //ntrivedi moved to sp ntrivedi 08-07-2019 rita sometimes found settled balance mismatch so sp may run faster
                //CheckUserCrBalanceFlag1 = await CheckUserCrBalanceFlagTask1;
                //if (!CheckUserCrBalanceFlag1)
                //{
                //    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo,Helpers.UTC_To_IST(), null, secondCurrObj.debitObject.WalletId, secondCurrObj.Coin, secondCurrObj.debitObject.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, secondCurrObj.debitObject.trnType);
                //    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                //    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.CrDrCredit_SettledBalMismatchCrWalletSecCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                //}

                //Task<bool> CheckUserDrBalanceFlagTask1 = CheckUserBalanceAsync(secondCurrObj.Amount, secondCurrObj.debitObject.WalletId, enBalanceType.OutBoundBalance);


                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("GetWalletCreditDrForHoldNewAsyncFinal before await1", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));


                firstCurrObjCrWM = await firstCurrObjCrWMTask;
                if (firstCurrObjCrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjCrWM.Id, secondCurrObj.Coin, firstCurrObjCrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, secondCurrObj.debitObject.trnType);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.FirstCurrCrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                firstCurrObj.creditObject.UserID = firstCurrObjCrWM.UserID;

                firstCurrObjDrWM = await firstCurrObjDrWMTask;
                if (firstCurrObjDrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, secondCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, secondCurrObj.debitObject.trnType);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.FirstCurrDrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }
                firstCurrObj.debitObject.UserID = firstCurrObjDrWM.UserID;

                secondCurrObjCrWM = await secondCurrObjCrWMTask;
                if (secondCurrObjCrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjCrWM.Id, secondCurrObj.Coin, secondCurrObjCrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, secondCurrObj.debitObject.trnType);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SecondCurrCrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }
                secondCurrObj.creditObject.UserID = secondCurrObjCrWM.UserID;

                secondCurrObjDrWM = await secondCurrObjDrWMTask;
                if (secondCurrObjDrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, secondCurrObj.debitObject.trnType);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SecondCurrDrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }
                secondCurrObj.debitObject.UserID = secondCurrObjDrWM.UserID;

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal after await1", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));


                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal before await2", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));

                checkDebitRefNo1 = await checkDebitRefNoTask1;
                if (checkDebitRefNo1 == false)//fail
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObj.creditObject.WalletId, secondCurrObj.Coin, secondCurrObj.creditObject.UserID, timestamp, enTransactionStatus.SystemFail, "Amount and DebitRefNo matching failure", secondCurrObj.creditObject.trnType);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNoSecCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                }

                if (firstCurrObj.debitObject.isMarketTrade == 1 && firstCurrObj.debitObject.differenceAmount > 0)
                {
                    if (firstCurrObjDrWM.Balance < firstCurrObj.debitObject.differenceAmount)
                    {
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, firstCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficientMarketInternalBalanceCheckFirstCurrencyForDifferentialAmountFailed, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                    bizResponseClassFC = _walletSPRepositories.Callsp_HoldWallet_MarketTrade(firstCurrObjDrWM, timestamp, serviceType, firstCurrObj.debitObject.differenceAmount, firstCurrObj.Coin, allowedChannels, firstCurrObjDrWM.WalletTypeID, firstCurrObj.debitObject.WTQTrnNo, firstCurrObj.debitObject.WalletId, firstCurrObj.debitObject.UserID, enTrnType.Buy_Trade, firstCurrObj.debitObject.trnType, enWalletDeductionType.Market);
                    if (bizResponseClassFC.ReturnCode != enResponseCode.Success)
                    {
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, firstCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.FirstCurrDifferentialAmountHoldFailed, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                }
                if (secondCurrObj.debitObject.isMarketTrade == 1 && secondCurrObj.debitObject.differenceAmount > 0)
                {
                    if (secondCurrObjDrWM.Balance < secondCurrObj.debitObject.differenceAmount)
                    {
                        // insert with status=2 system failed
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType);
                        tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                        return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficientMarketInternalBalanceCheckSecondCurrencyForDifferentialAmountFailed, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg }, "Credit");
                    }
                    bizResponseClassSC = _walletSPRepositories.Callsp_HoldWallet_MarketTrade(secondCurrObjDrWM, timestamp, serviceType, secondCurrObj.debitObject.differenceAmount, secondCurrObj.Coin, allowedChannels, secondCurrObjDrWM.WalletTypeID, secondCurrObj.debitObject.WTQTrnNo, secondCurrObj.debitObject.WalletId, secondCurrObj.debitObject.UserID, enTrnType.Buy_Trade, secondCurrObj.debitObject.trnType, enWalletDeductionType.Market);
                    if (bizResponseClassSC.ReturnCode != enResponseCode.Success)
                    {
                        tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, firstCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType);
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
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjDrWM.Id, firstCurrObj.Coin, firstCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, firstCurrObj.debitObject.trnType);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficientOutgoingBalFirstCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                if (secondCurrObjDrWM.OutBoundBalance < secondCurrObj.Amount)// ntrivedi checking outbound balance
                {
                    // insert with status=2 system failed
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, secondCurrObj.debitObject.trnType);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficietOutgoingBalSecondCur, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                if (firstCurrObjDrWM.Status != 1)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, secondCurrObj.debitObject.trnType);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.FirstCurrWalletStatusDisable, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                if (secondCurrObjDrWM.Status != 1)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, secondCurrObj.Amount, secondCurrObj.debitObject.TrnRefNo, Helpers.UTC_To_IST(), null, secondCurrObjDrWM.Id, secondCurrObj.Coin, secondCurrObjDrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, secondCurrObj.debitObject.trnType);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SecondCurrWalletStatusDisable, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal after await3", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));

                //ntrivedi moved to sp ntrivedi 08-07-2019 rita sometimes found settled balance mismatch so sp may run faster
                //CheckUserDrBalanceFlag1 = await CheckUserDrBalanceFlagTask1;
                //if (!CheckUserDrBalanceFlag1)
                //{
                //    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Credit, secondCurrObj.Amount, secondCurrObj.creditObject.TrnRefNo,Helpers.UTC_To_IST(), null, secondCurrObj.creditObject.WalletId, secondCurrObj.Coin, secondCurrObj.creditObject.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, secondCurrObj.creditObject.trnType);
                //    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                //    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.CrDrCredit_SettledBalMismatchDrWalletSecDr, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                //}


                Task.Run(() => HelperForLog.WriteLogIntoFile("GetWalletCreditDrForHoldNewAsyncFinal before Wallet operation", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.creditObject.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.debitObject.TrnRefNo.ToString()));


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
                HelperForLog.WriteErrorLog("SMSSendAsyncV1" + " - Data- " + templateType.ToString(), "WalletService", ex);
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
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " -Data- " + templateType.ToString(), "WalletService", ex);
            }
        }

        public async Task<WalletDrCrResponse> GetReleaseHoldNew(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, string Token = "")
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
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTradeRefNo, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
                if (amount <= 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidAmt, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmount, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
                dWalletobj = await dWalletobjTask;
                if (dWalletobj == null)
                {
                    HelperForLog.WriteLogIntoFile("GetReleaseHoldNew","WalletService","DebitWallet Null For TrnNo="+ TrnRefNo.ToString() + "##WTID=" + walletTypeMaster.Id + "AccountId" + accWalletID);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
                userID = dWalletobj.UserID;
                Task<bool> flagTask = CheckUserBalanceAsync(amount, dWalletobj.Id);

                if (dWalletobj.Status != 1 || dWalletobj.IsValid == false)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidWallet, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    HelperForLog.WriteLogIntoFile("GetReleaseHoldNew", "WalletService", "DebitWallet Status Not Active Or IsValid False For TrnNo=" + TrnRefNo.ToString() + "##WTID=" + walletTypeMaster.Id + "AccountId" + accWalletID);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }

                HelperForLog.WriteLogIntoFileAsync("GetReleaseHoldNew", "CheckUserBalance pre Balance=" + dWalletobj.Balance.ToString() + ", TrnNo=" + TrnRefNo.ToString());
                CheckUserBalanceFlag = await flagTask;

                HelperForLog.WriteLogIntoFileAsync("GetReleaseHoldNew", "CheckUserBalance Post TrnNo=" + TrnRefNo.ToString());
                dWalletobj = _commonRepository.GetById(dWalletobj.Id); // ntrivedi fetching fresh balance for multiple request at a time 

                if (!CheckUserBalanceFlag)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    HelperForLog.WriteLogIntoFile("GetReleaseHoldNew", "WalletService", "DebitWallet Balance Flag False For TrnNo=" + TrnRefNo.ToString() + "##WTID=" + walletTypeMaster.Id + "AccountId" + accWalletID);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedBalanceMismatch, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
                Task<bool> flagTask1 = CheckUserBalanceAsync(amount, dWalletobj.Id, enBalanceType.OutBoundBalance);
                CheckUserBalanceFlag = await flagTask1;
                if (!CheckUserBalanceFlag)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    HelperForLog.WriteLogIntoFile("GetReleaseHoldNew", "WalletService", "DebitWallet OutBoundBalance Flag False For TrnNo=" + TrnRefNo.ToString() + "##WTID=" + walletTypeMaster.Id + " AccWalletId=" + accWalletID + " Amount=" + amount.ToString());
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedOutgoingBalanceMismatch, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "GetReleaseHoldNew");
                }
                HelperForLog.WriteLogIntoFileAsync("GetReleaseHoldNew", "before Check ShadowLimit TrnNo=" + TrnRefNo.ToString());

                HelperForLog.WriteLogIntoFileAsync("GetReleaseHoldNew", "Check ShadowLimit done TrnNo=" + TrnRefNo.ToString());

                if (dWalletobj.OutBoundBalance < amount)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, trnType);
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

                        ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                        ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.HoldBalanceReleaseNotification);
                        ActivityNotification.Param1 = coinName;
                        ActivityNotification.Param2 = amount.ToString();
                        ActivityNotification.Param3 = TrnRefNo.ToString();
                        ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);

                        HelperForLog.WriteLogIntoFileAsync("GetReleaseHoldNew", "OnWalletBalChange + SendActivityNotificationV2 pre TrnNo=" + TrnRefNo.ToString());

                        //komal 11-11-2019 12:12 PM remove unwanted alert
                        Task.Run(() => Parallel.Invoke(//() => _signalRService.SendActivityNotificationV2(ActivityNotification, dWalletobj.UserID.ToString(), 2),
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
                                    ActivityNotificationMessage ActivityNotificationCharge = new ActivityNotificationMessage();
                                    ActivityNotificationCharge.MsgCode = Convert.ToInt32(enErrorCode.ChargeReleasedWallet);
                                    ActivityNotificationCharge.Param1 = ChargewalletType.WalletTypeName;
                                    ActivityNotificationCharge.Param2 = charge.ToString();
                                    ActivityNotificationCharge.Param3 = TrnRefNo.ToString();
                                    ActivityNotificationCharge.Type = Convert.ToInt16(EnNotificationType.Info);

                                    WalletMasterResponse walletMasterChargeObj = new WalletMasterResponse();
                                    walletMasterObj.AccWalletID = ChargeWalletObj.AccWalletID;
                                    walletMasterObj.Balance = ChargeWalletObj.Balance;
                                    walletMasterObj.WalletName = ChargeWalletObj.Walletname;
                                    walletMasterObj.PublicAddress = ChargeWalletObj.PublicAddress;
                                    walletMasterObj.IsDefaultWallet = ChargeWalletObj.IsDefaultWallet;
                                    walletMasterObj.CoinName = coinName;
                                    walletMasterObj.OutBoundBalance = ChargeWalletObj.OutBoundBalance;
                                    //komal 11-11-2019 12:12 PM remove unwanted alert
                                    Parallel.Invoke(() => EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, dWalletobj.UserID.ToString(), charge.ToString(), ChargewalletType.WalletTypeName, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString(), "released"),
                                  //() => _signalRService.SendActivityNotificationV2(ActivityNotificationCharge, dWalletobj.UserID.ToString(), 2),
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
                //throw ex;
            }
        }


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

                ActivityNotificationMessage ActivityNotificationCr = new ActivityNotificationMessage();
                ActivityNotificationCr.MsgCode = Convert.ToInt32(enErrorCode.CreditWalletMsgNotification);
                ActivityNotificationCr.Param1 = firstCurrObj.Coin;
                ActivityNotificationCr.Param2 = firstCurrObj.creditObject.trnType.ToString();
                ActivityNotificationCr.Param3 = firstCurrObj.creditObject.TrnRefNo.ToString();
                ActivityNotificationCr.Type = Convert.ToInt16(EnNotificationType.Info);

                ActivityNotificationMessage ActivityNotificationCr1 = new ActivityNotificationMessage();
                ActivityNotificationCr1.MsgCode = Convert.ToInt32(enErrorCode.CreditWalletMsgNotification);
                ActivityNotificationCr1.Param1 = secondCurrObj.Coin;
                ActivityNotificationCr1.Param2 = secondCurrObj.creditObject.trnType.ToString();
                ActivityNotificationCr1.Param3 = secondCurrObj.creditObject.TrnRefNo.ToString();
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
                ActivityNotificationdr.Param3 = firstCurrObj.debitObject.TrnRefNo.ToString();
                ActivityNotificationdr.Type = Convert.ToInt16(EnNotificationType.Info);

                ActivityNotificationMessage ActivityNotificationdr1 = new ActivityNotificationMessage();
                ActivityNotificationdr1.MsgCode = Convert.ToInt32(enErrorCode.DebitWalletMsgNotification);
                ActivityNotificationdr1.Param1 = secondCurrObj.Coin;
                ActivityNotificationdr1.Param2 = secondCurrObj.debitObject.trnType.ToString();
                ActivityNotificationdr1.Param3 = secondCurrObj.debitObject.TrnRefNo.ToString();
                ActivityNotificationdr1.Type = Convert.ToInt16(EnNotificationType.Info);

                Task.Run(() => HelperForLog.WriteLogIntoFile("CreditNotificationSend Activity:Without Token", "WalletService", "msg=" + ActivityNotificationdr.MsgCode.ToString() + ",User=" + firstCurrObjCrWM.UserID.ToString() + "WalletID" + firstCurrObjCrWM.AccWalletID + ",Balance" + firstCurrObjCrWM.Balance.ToString()));

                var firstCurrObjCrType = firstCurrObj.creditObject.trnType.ToString().Contains("Cr_") ? firstCurrObj.creditObject.trnType.ToString().Replace("Cr_", "") : firstCurrObj.creditObject.trnType.ToString().Replace("Dr_", "");
                var firstCurrObjDrType = firstCurrObj.debitObject.trnType.ToString().Contains("Cr_") ? firstCurrObj.debitObject.trnType.ToString().Replace("Cr_", "") : firstCurrObj.debitObject.trnType.ToString().Replace("Dr_", "");
                var secCurrObjCrType = secondCurrObj.creditObject.trnType.ToString().Contains("Cr_") ? secondCurrObj.creditObject.trnType.ToString().Replace("Cr_", "") : secondCurrObj.creditObject.trnType.ToString().Replace("Dr_", "");
                var secCurrObjDrType = secondCurrObj.debitObject.trnType.ToString().Contains("Cr_") ? secondCurrObj.debitObject.trnType.ToString().Replace("Cr_", "") : secondCurrObj.debitObject.trnType.ToString().Replace("Dr_", "");

                //komal 11-11-2019 12:12 PM remove unwanted alert
                Parallel.Invoke(//() => _signalRService.SendActivityNotificationV2(ActivityNotificationCr, firstCurrObjCrWM.UserID.ToString(), 2, firstCurrObj.creditObject.TrnRefNo + " timestamp : " + timestamp),
                                           () => _signalRService.OnWalletBalChange(walletMasterObjCr, firstCurrObj.Coin, firstCurrObjCrWM.UserID.ToString(), 2, firstCurrObj.creditObject.TrnRefNo + " timestamp : " + timestamp),
                                           //() => _signalRService.SendActivityNotificationV2(ActivityNotificationCr1, secondCurrObjCrWM.UserID.ToString(), 2, secondCurrObj.creditObject.TrnRefNo + " timestamp : " + timestamp),
                                           () => _signalRService.OnWalletBalChange(walletMasterObjCr1, secondCurrObj.Coin, secondCurrObjCrWM.UserID.ToString(), 2, secondCurrObj.creditObject.TrnRefNo + " timestamp : " + timestamp),
                                           //() => _signalRService.SendActivityNotificationV2(ActivityNotificationdr, firstCurrObjDrWM.UserID.ToString(), 2, firstCurrObj.debitObject.TrnRefNo + " timestamp : " + timestamp),
                                           () => _signalRService.OnWalletBalChange(walletMasterObjDr, firstCurrObj.Coin, firstCurrObjDrWM.UserID.ToString(), 2, firstCurrObj.debitObject.TrnRefNo + " timestamp : " + timestamp),
                                           //() => _signalRService.SendActivityNotificationV2(ActivityNotificationdr1, secondCurrObjDrWM.UserID.ToString(), 2, secondCurrObj.debitObject.TrnRefNo + " timestamp : " + timestamp),
                                           () => _signalRService.OnWalletBalChange(walletMasterObjDr1, secondCurrObj.Coin, secondCurrObjDrWM.UserID.ToString(), 2, secondCurrObj.debitObject.TrnRefNo + " timestamp : " + timestamp),
                                           () => SMSSendAsyncV1(EnTemplateType.SMS_WalletCredited, firstCurrObjCrWM.UserID.ToString(), null, null, null, null, firstCurrObj.Coin, firstCurrObjCrType, firstCurrObj.creditObject.TrnRefNo.ToString()),
                                           () => SMSSendAsyncV1(EnTemplateType.SMS_WalletCredited, secondCurrObjCrWM.UserID.ToString(), null, null, null, null, secondCurrObj.Coin, secCurrObjCrType, secondCurrObj.creditObject.TrnRefNo.ToString()),
                                            () => SMSSendAsyncV1(EnTemplateType.SMS_WalletDebited, firstCurrObjDrWM.UserID.ToString(), null, null, null, null, firstCurrObj.Coin, firstCurrObjDrType, firstCurrObj.debitObject.TrnRefNo.ToString()),
                                            () => SMSSendAsyncV1(EnTemplateType.SMS_WalletDebited, secondCurrObjDrWM.UserID.ToString(), null, null, null, null, secondCurrObj.Coin, secCurrObjDrType, secondCurrObj.debitObject.TrnRefNo.ToString()),
                                            () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletCredited, secondCurrObjCrWM.UserID.ToString(), secondCurrObj.Amount.ToString(), secondCurrObj.Coin, Helpers.UTC_To_IST().ToString(), secondCurrObj.creditObject.TrnRefNo.ToString(), secCurrObjCrType),
                                            () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletCredited, firstCurrObjCrWM.UserID.ToString(), firstCurrObj.Amount.ToString(), firstCurrObj.Coin, Helpers.UTC_To_IST().ToString(), firstCurrObj.creditObject.TrnRefNo.ToString(), firstCurrObjCrType),
                                            () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, secondCurrObjDrWM.UserID.ToString(), secondCurrObj.Amount.ToString(), secondCurrObj.Coin, Helpers.UTC_To_IST().ToString(), secondCurrObj.debitObject.TrnRefNo.ToString(), secCurrObjDrType),
                                            () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, firstCurrObjDrWM.UserID.ToString(), firstCurrObj.Amount.ToString(), firstCurrObj.Coin, Helpers.UTC_To_IST().ToString(), firstCurrObj.debitObject.TrnRefNo.ToString(), firstCurrObjDrType)
                                           );

                if (ChargefirstCur > 0 && ChargesecondCur > 0)
                {
                    ActivityNotificationMessage ActivityNotificationCrChargeSec = new ActivityNotificationMessage();
                    ActivityNotificationCrChargeSec.MsgCode = Convert.ToInt32(enErrorCode.ChargeDeductedWallet);
                    ActivityNotificationCrChargeSec.Param1 = secondCurrObj.Coin;
                    ActivityNotificationCrChargeSec.Param2 = ChargefirstCur.ToString();
                    ActivityNotificationCrChargeSec.Param3 = secondCurrObj.creditObject.TrnRefNo.ToString();
                    ActivityNotificationCrChargeSec.Type = Convert.ToInt16(EnNotificationType.Info);

                    ActivityNotificationMessage ActivityNotificationDrChargeSec = new ActivityNotificationMessage();
                    ActivityNotificationDrChargeSec.MsgCode = Convert.ToInt32(enErrorCode.ChargeDeductedWallet);
                    ActivityNotificationDrChargeSec.Param1 = secondCurrObj.Coin;
                    ActivityNotificationDrChargeSec.Param2 = ChargesecondCur.ToString();
                    ActivityNotificationDrChargeSec.Param3 = secondCurrObj.debitObject.TrnRefNo.ToString();
                    ActivityNotificationDrChargeSec.Type = Convert.ToInt16(EnNotificationType.Info);

                    //komal 11-11-2019 12:12 PM remove unwanted alert
                    Parallel.Invoke(() => EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, firstCurrObjDrWM.UserID.ToString(), ChargefirstCur.ToString(), firstCurrObj.Coin, Helpers.UTC_To_IST().ToString(), firstCurrObj.debitObject.TrnRefNo.ToString(), "Deducted"),
                    () => EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, secondCurrObjDrWM.UserID.ToString(), ChargesecondCur.ToString(), secondCurrObj.Coin, Helpers.UTC_To_IST().ToString(), secondCurrObj.debitObject.TrnRefNo.ToString(), "Deducted")
                  //() => _signalRService.SendActivityNotificationV2(ActivityNotificationCrChargeSec, firstCurrObjDrWM.UserID.ToString(), 2),
                    //) => _signalRService.SendActivityNotificationV2(ActivityNotificationDrChargeSec, firstCurrObjDrWM.UserID.ToString(), 2)
                    );
                }
                #endregion
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreditNotificationSend" + "TimeStamp:" + timestamp, "WalletService", ex);

                //throw ex;
            }
        }

        public async Task WalletHoldNotificationSend(string timestamp, WalletMaster dWalletobj, string coinName, decimal amount, long TrnRefNo, byte routeTrnType, decimal charge, long walletId, WalletMaster WalletlogObj, WalletTypeMaster DeductCoinName)
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

                ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.HoldBalanceNotification);
                ActivityNotification.Param1 = coinName;
                ActivityNotification.Param2 = amount.ToString();
                ActivityNotification.Param3 = TrnRefNo.ToString();
                ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);

                HelperForLog.WriteLogIntoFileAsync("WalletHoldNotificationSend", "OnWalletBalChange + SendActivityNotificationV2 pre timestamp=" + timestamp.ToString());

                //komal 11-11-2019 12:12 PM remove unwanted alert
                Parallel.Invoke(//() => _signalRService.SendActivityNotificationV2(ActivityNotification, dWalletobj.UserID.ToString(), 2),
                    () => _signalRService.OnWalletBalChange(walletMasterObj, coinName, dWalletobj.UserID.ToString(), 2),

                    () => SMSSendAsyncV1(EnTemplateType.SMS_WalletDebited, dWalletobj.UserID.ToString(), null, null, null, null, coinName, routeTrnType.ToString(), TrnRefNo.ToString()),
                    () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, dWalletobj.UserID.ToString(), amount.ToString(), coinName, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString()));

                if (charge > 0 && walletId > 0 && WalletlogObj != null && (DeductCoinName != null))
                {
                    ActivityNotificationMessage ActivityNotificationCharge = new ActivityNotificationMessage();
                    ActivityNotificationCharge.MsgCode = Convert.ToInt32(enErrorCode.ChargeHoldWallet);
                    ActivityNotificationCharge.Param1 = DeductCoinName.WalletTypeName;
                    ActivityNotificationCharge.Param2 = charge.ToString();
                    ActivityNotificationCharge.Param3 = TrnRefNo.ToString();
                    ActivityNotificationCharge.Type = Convert.ToInt16(EnNotificationType.Info);

                    WalletMasterResponse walletMasterObjCharge = new WalletMasterResponse();
                    walletMasterObjCharge.AccWalletID = WalletlogObj.AccWalletID;
                    walletMasterObjCharge.Balance = WalletlogObj.Balance;
                    walletMasterObjCharge.WalletName = WalletlogObj.Walletname;
                    walletMasterObjCharge.PublicAddress = WalletlogObj.PublicAddress;
                    walletMasterObjCharge.IsDefaultWallet = WalletlogObj.IsDefaultWallet;
                    walletMasterObjCharge.CoinName = DeductCoinName.WalletTypeName;
                    walletMasterObjCharge.OutBoundBalance = WalletlogObj.OutBoundBalance;

                    //komal 11-11-2019 12:12 PM remove unwanted alert
                    Parallel.Invoke(
                      //() => _signalRService.SendActivityNotificationV2(ActivityNotificationCharge, dWalletobj.UserID.ToString(), 2),
                      () => _signalRService.OnWalletBalChange(walletMasterObjCharge, DeductCoinName.WalletTypeName, dWalletobj.UserID.ToString(), 2),
                      () => EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, dWalletobj.UserID.ToString(), charge.ToString(), DeductCoinName.WalletTypeName, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString(), "hold"));
                }
                #endregion
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WalletHoldNotificationSend Timestamp=" + timestamp, "WalletService", ex);
            }
        }

        public async Task CreditWalletNotificationSend(string timestamp, WalletMasterResponse walletMasterObj, string coinName, decimal TotalAmount, long TrnRefNo, byte routeTrnType, long userID, string Token, string Wtrntype, decimal charge, WalletMaster ChargeWalletObj, string DeductWalletType)
        {
            try
            {
                #region SMS_EMail     
                ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.CreditWalletMsgNotification);
                ActivityNotification.Param1 = coinName;
                ActivityNotification.Param2 = routeTrnType.ToString();
                ActivityNotification.Param3 = TrnRefNo.ToString();
                ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);
                HelperForLog.WriteLogIntoFile("GetWalletCreditNew Activity:With Token", "WalletService", "msg=" + ActivityNotification.MsgCode.ToString() + "," + "WalletID" + walletMasterObj.AccWalletID + ",Balance" + walletMasterObj.Balance.ToString());

                var trnType = Wtrntype.Contains("Cr_") ? Wtrntype.Replace("Cr_", "") : Wtrntype.Replace("Dr_", "");

                Parallel.Invoke(
                  () => _signalRService.SendActivityNotificationV2(ActivityNotification, userID.ToString(), 2),

                   () => _signalRService.OnWalletBalChange(walletMasterObj, coinName, userID.ToString(), 2),

                   () => SMSSendAsyncV1(EnTemplateType.SMS_WalletCredited, userID.ToString(), null, null, null, null, coinName, trnType, TrnRefNo.ToString()),
                   () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletCredited, userID.ToString(), TotalAmount.ToString(), coinName, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString(), trnType));

                if (charge > 0 && ChargeWalletObj != null && (DeductWalletType != null))
                {
                    ActivityNotificationMessage ActivityNotificationCharge = new ActivityNotificationMessage();
                    ActivityNotificationCharge.MsgCode = Convert.ToInt32(enErrorCode.ChargeRefundedWallet);
                    ActivityNotificationCharge.Param1 = DeductWalletType;
                    ActivityNotificationCharge.Param2 = charge.ToString();
                    ActivityNotificationCharge.Param3 = TrnRefNo.ToString();
                    ActivityNotificationCharge.Type = Convert.ToInt16(EnNotificationType.Info);

                    WalletMasterResponse walletMasterObjCharge = new WalletMasterResponse();
                    walletMasterObjCharge.AccWalletID = ChargeWalletObj.AccWalletID;
                    walletMasterObjCharge.Balance = ChargeWalletObj.Balance;
                    walletMasterObjCharge.WalletName = ChargeWalletObj.Walletname;
                    walletMasterObjCharge.PublicAddress = ChargeWalletObj.PublicAddress;
                    walletMasterObjCharge.IsDefaultWallet = ChargeWalletObj.IsDefaultWallet;
                    walletMasterObjCharge.CoinName = DeductWalletType;
                    walletMasterObjCharge.OutBoundBalance = ChargeWalletObj.OutBoundBalance;

                    Parallel.Invoke(() => EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApplyrefund, userID.ToString(), charge.ToString(), DeductWalletType, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString(), "refunded"),
                  () => _signalRService.SendActivityNotificationV2(ActivityNotificationCharge, userID.ToString(), 2),
                 () => _signalRService.OnWalletBalChange(walletMasterObjCharge, DeductWalletType, userID.ToString(), 2));
                }
                #endregion
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WalletHoldNotificationSend Timestamp=" + timestamp, "WalletService", ex);
            }
        }

        public async Task WalletDeductionNewNotificationSend(string timestamp, WalletMaster dWalletobj, string coinName, decimal amount, long TrnRefNo, byte routeTrnType, long userID, string Token, string Wtrntype, WalletMasterResponse walletMasterObj, decimal charge, string DeductWalletType, WalletMasterResponse ChargeWallet)
        {
            try
            {
                var trnType = Wtrntype.Contains("Cr_") ? Wtrntype.Replace("Cr_", "") : Wtrntype.Replace("Dr_", "");
                #region SMS_Email             
                ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.DebitWalletMsgNotification);
                ActivityNotification.Param1 = coinName;
                ActivityNotification.Param2 = trnType; //ntrivedi 08-02-20019 "6" instead of Withdrawal in notification
                ActivityNotification.Param3 = TrnRefNo.ToString();
                ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);

                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew", "OnWalletBalChange + SendActivityNotificationV2 pre TrnNo=" + TrnRefNo.ToString());

                Parallel.Invoke(
                   () => _signalRService.SendActivityNotificationV2(ActivityNotification, dWalletobj.UserID.ToString(), 2),
                   () => _signalRService.OnWalletBalChange(walletMasterObj, coinName, dWalletobj.UserID.ToString(), 2),
                   () => SMSSendAsyncV1(EnTemplateType.SMS_WalletDebited, userID.ToString(), null, null, null, null, coinName, trnType, TrnRefNo.ToString()),
                   () => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, userID.ToString(), amount.ToString(), coinName, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString(), trnType));
                HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew twice", "WalletNewTest");
                //object reference error solving ntrivedi 26-07-2019
                //HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew mail before", "Get walletid and currency walletid=" + ChargeWallet.AccWalletID.ToString() + "Currency : " + DeductWalletType.ToString() + "Charge: " + charge.ToString());

                if (charge > 0 && (DeductWalletType != null) && ChargeWallet != null)
                {
                    HelperForLog.WriteLogIntoFileAsync("GetWalletDeductionNew mail after", "Get walletid and currency walletid=" + ChargeWallet.AccWalletID.ToString() + "Currency : " + DeductWalletType.ToString() + "Charge: " + charge.ToString());

                    ActivityNotificationMessage ActivityNotificationCharge = new ActivityNotificationMessage();
                    ActivityNotificationCharge.MsgCode = Convert.ToInt32(enErrorCode.ChargeDeductedWallet);
                    ActivityNotificationCharge.Param1 = DeductWalletType;
                    ActivityNotificationCharge.Param2 = charge.ToString();
                    ActivityNotificationCharge.Param3 = TrnRefNo.ToString();
                    ActivityNotificationCharge.Type = Convert.ToInt16(EnNotificationType.Info);

                    Parallel.Invoke(
                         () => _signalRService.OnWalletBalChange(ChargeWallet, DeductWalletType, dWalletobj.UserID.ToString(), 2),
                         () => _signalRService.SendActivityNotificationV2(ActivityNotificationCharge, dWalletobj.UserID.ToString(), 2),
                         //6.Action(1.Hold 2.Released 3.Deduct)
                         () => EmailSendAsyncV1(EnTemplateType.EMAIL_ChrgesApply, userID.ToString(), charge.ToString(), DeductWalletType, Helpers.UTC_To_IST().ToString(), TrnRefNo.ToString(), "deducted"));
                }
                #endregion
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WalletHoldNotificationSend Timestamp=" + timestamp, "WalletService", ex);
                //throw ex;
            }
        }

        public async Task<BizResponseClass> UpdateWalletDetail(string AccWalletID, string walletName, short? status, byte? isDefaultWallet, long UserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                bool flag = false;

                //2019-2-18 added condi for only used trading wallet
                var IsExist = await _commonRepository.GetSingleAsync(item => item.AccWalletID == AccWalletID && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (IsExist != null)
                {
                    IsExist.Walletname = (walletName == null ? IsExist.Walletname : walletName);
                    IsExist.Status = Convert.ToInt16(status == null ? IsExist.Status : status);
                    IsExist.UpdatedBy = UserID;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    IsExist.IsDefaultWallet = Convert.ToByte(isDefaultWallet == null ? IsExist.IsDefaultWallet : isDefaultWallet);

                    if (isDefaultWallet != null && isDefaultWallet > 0)
                    {
                        var DefaultWallets = _walletRepository1.UpdateDefaultWallets(IsExist.WalletTypeID, UserID);
                        if (DefaultWallets.AffectedRows > 0)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        _commonRepository.UpdateWithAuditLog(IsExist);
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.InternalError;
                        Resp.ReturnMsg = EnResponseMessage.InternalError;
                    }
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateWalletDetail", "WalletService", ex);
                return null;
            }
        }

        #region AddUserWalletRequest

        //2018-12-20
        public ListAddWalletRequest ListAddUserWalletRequest(long UserID)
        {
            try
            {
                ListAddWalletRequest Resp = new ListAddWalletRequest();
                var data = _walletRepository1.ListAddUserWalletRequest(UserID);
                Resp.Data = data;
                if (data.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> InsertUserWalletPendingRequest(InsertWalletRequest request, long UserId)
        {
            try
            {

                //2019-2-18 added condi for only used trading wallet
                var walletObj1 = _commonRepository.GetSingleAsync(i => i.AccWalletID == request.WalletID && i.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                var roleObj1 = _UserRoleMaster.GetSingleAsync(i => i.Id == request.RoleId);
                var userObj1 = _userManager.FindByEmailAsync(request.Email);
                var userObj = await userObj1;
                if (userObj == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.EmailNotExist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.EmailNotExist };
                }
                if (userObj.Id == UserId)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.NotShareWallet, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotShareWallet };
                }
                var walletObj = await walletObj1;

                var authoriseObj = _WalletAuthorizeUserMaster.GetSingle(i => i.WalletID == walletObj.Id && i.UserID == userObj.Id);
                var roleObj = await roleObj1;
                if (authoriseObj != null)
                {
                    if (authoriseObj.Status == 1 && request.RequestType == 1)
                    {
                        string Msg = EnResponseMessage.AlredyExistWithRole + roleObj.RoleType + "!!";
                        return new BizResponseClass { ErrorCode = enErrorCode.AlredyExistWithRole, ReturnCode = enResponseCode.Fail, ReturnMsg = Msg };
                    }
                }

                if (authoriseObj == null && request.RequestType == 2)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.NotRemoveUser, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotRemoveUser };
                }

                var IsExist1 = _AddRemoveUserWalletRequest.GetSingleAsync(i => i.WalletID == walletObj.Id && i.ToUserId == userObj.Id && i.FromUserId == UserId && i.Type == request.RequestType && i.Status != 9);
                var walletTypeObj1 = _WalletTypeMasterRepository.GetSingleAsync(i => i.Id == walletObj.WalletTypeID);
                var FromUser1 = _userManager.FindByIdAsync(UserId.ToString());

                var FromUser = await FromUser1;
                var walletTypeObj = await walletTypeObj1;
                var IsExist = await IsExist1;

                //call sp
                BizResponseClass BizResponseClassObj = _walletSPRepositories.Callsp_IsValidWalletTransaction(walletObj.Id, UserId, walletTypeObj.Id, request.ChannelId, Convert.ToInt64(enWalletTrnType.AddUser));

                if (BizResponseClassObj.ReturnCode == 0)
                {
                    if (IsExist != null)
                    {
                        string Msg = (roleObj == null) ? EnResponseMessage.AlredyExistWithRole : EnResponseMessage.AlredyExistWithRole + roleObj.RoleType + "!!";
                        if (IsExist.Status == 1)
                        {
                            return new BizResponseClass { ErrorCode = enErrorCode.AlredyExistWithRole, ReturnCode = enResponseCode.Fail, ReturnMsg = Msg };
                        }
                        if (IsExist.Status == 0)
                        {
                            return new BizResponseClass { ErrorCode = enErrorCode.RequestPending, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.RequestPending };
                        }
                    }
                    AddRemoveUserWalletRequest Obj = new AddRemoveUserWalletRequest();
                    Obj.Status = 0;//pending
                    Obj.CreatedBy = UserId;
                    Obj.CreatedDate = Helpers.UTC_To_IST();
                    Obj.UpdatedDate = Helpers.UTC_To_IST();
                    Obj.RecieverApproveBy = userObj.Id;
                    Obj.RecieverApproveDate = Helpers.UTC_To_IST();
                    Obj.WalletID = walletObj.Id;
                    Obj.ToUserId = userObj.Id;
                    Obj.WalletOwnerUserID = Convert.ToInt64(walletObj.UserID);
                    Obj.RoleId = request.RoleId;
                    Obj.Message = request.Message;
                    Obj.ReceiverEmail = request.Email;
                    Obj.OwnerApprovalDate = Helpers.UTC_To_IST();
                    Obj.OwnerApprovalBy = (walletObj.UserID == UserId) ? UserId : walletObj.UserID;
                    Obj.OwnerApprovalStatus = walletObj.UserID == UserId ? Convert.ToInt16(1) : Convert.ToInt16(0);
                    Obj.Type = request.RequestType;
                    Obj.FromUserId = UserId;
                    _AddRemoveUserWalletRequest.Add(Obj);

                    if (walletObj.UserID != UserId)
                    {
                        //first sent mail to owner-> after verification sent to other
                        EmailSendAsyncV1(EnTemplateType.Email_AddUserOwnerApproval, walletObj.UserID.ToString(), walletObj.Walletname, walletTypeObj.WalletTypeName, "https://www.google.com/", (request.RequestType == 1 ? "addition" : "removal"));
                    }
                    else
                    {
                        //for sender
                        EmailSendAsyncV1(EnTemplateType.Email_Sender, UserId.ToString(), userObj.Email, roleObj.RoleType, walletTypeObj.WalletTypeName, walletObj.Walletname, "https://www.google.com/", (request.RequestType == 1 ? "addition" : "removal"));

                        //send notification

                        var listActivity = ListAddUserWalletRequest(userObj.Id);
                        _signalRService.SendWalletActivityList(listActivity, userObj.Id.ToString());
                        //for reciever  
                        EmailSendAsyncV1(EnTemplateType.Email_Reciever, userObj.Id.ToString(), FromUser.Email, roleObj.RoleType, walletTypeObj.WalletTypeName, walletObj.Walletname, "https://www.google.com/", (request.RequestType == 1 ? "addition" : "removal"));
                    }
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RequestAdded };
                }

                return BizResponseClassObj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertUserWalletPendingRequest", "WalletService", ex);
                throw;
            }
        }

        //2018-12-20
        public async Task<BizResponseClass> UpdateUserWalletPendingRequest(short Status, long RequestId, long UserId)
        {
            try
            {
                var requestObj = _AddRemoveUserWalletRequest.GetSingle(i => i.Id == RequestId && i.Status == 0);
                if (requestObj == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                }

                //2019-2-18 added condi for only used trading wallet
                var walletObj1 = _commonRepository.GetSingleAsync(i => i.Id == requestObj.WalletID && i.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                var roleObjs1 = _UserRoleMaster.GetSingleAsync(i => i.Id == requestObj.RoleId);
                var walletObj = await walletObj1;
                var typeObj1 = _WalletTypeMasterRepository.GetSingleAsync(i => i.Id == walletObj.WalletTypeID);
                var userObj1 = _userManager.FindByEmailAsync(requestObj.ReceiverEmail);
                var FromUser1 = _userManager.FindByIdAsync(requestObj.FromUserId.ToString());
                var ownerUser1 = _userManager.FindByIdAsync(requestObj.WalletOwnerUserID.ToString());

                var roleObjs = await roleObjs1;
                var typeObj = await typeObj1;
                var userObj = await userObj1;
                var FromUser = await FromUser1;
                var ownerUser = await ownerUser1;

                if (requestObj.WalletOwnerUserID == UserId)//for owner approval
                {
                    if (requestObj.OwnerApprovalStatus == 0)
                    {
                        requestObj.UpdatedBy = UserId;
                        requestObj.UpdatedDate = Helpers.UTC_To_IST();
                        requestObj.OwnerApprovalStatus = Status;
                        requestObj.OwnerApprovalDate = Helpers.UTC_To_IST();
                        requestObj.OwnerApprovalBy = UserId;
                        if (requestObj.Type == 2)
                        {
                            requestObj.Status = Status;
                            requestObj.RecieverApproveDate = Helpers.UTC_To_IST();
                            requestObj.RecieverApproveBy = UserId;

                            var WalletAuthorizeUserMasterObj = _WalletAuthorizeUserMaster.GetSingle(i => i.WalletID == requestObj.WalletID && i.UserID == UserId);
                            if (Status == 1)
                            {
                                WalletAuthorizeUserMasterObj.Status = 9;
                                WalletAuthorizeUserMasterObj.UpdatedDate = Helpers.UTC_To_IST();
                                WalletAuthorizeUserMasterObj.UpdatedBy = UserId;
                                _WalletAuthorizeUserMaster.UpdateWithAuditLog(WalletAuthorizeUserMasterObj);
                            }
                        }
                        _AddRemoveUserWalletRequest.Update(requestObj);//update AddRemoveUserWalletRequest entity

                        if (Status == 1)
                        {
                            //for reciever  to adding/removing req
                            EmailSendAsyncV1(EnTemplateType.Email_Reciever, userObj.Id.ToString(), FromUser.Email, roleObjs.RoleType, typeObj.WalletTypeName, walletObj.Walletname, "https://www.google.com/", (requestObj.Type == 1 ? "addition" : "removal"));
                        }

                        //send mail to owner for suucess approval to add/remove user
                        EmailSendAsyncV1(EnTemplateType.Email_OwnerApproval, requestObj.WalletOwnerUserID.ToString(), requestObj.ReceiverEmail, roleObjs.RoleType, typeObj.WalletTypeName, walletObj.Walletname, (requestObj.Type == 1 ? "addition" : "removal"), (Status == 1 ? "accepted" : "rejected"), FromUser.Email);

                        //for sender
                        EmailSendAsyncV1(EnTemplateType.Email_RequestApproval, requestObj.FromUserId.ToString(), requestObj.ReceiverEmail, roleObjs.RoleType, typeObj.WalletTypeName, walletObj.Walletname, (requestObj.Type == 1 ? "addition" : "removal"), (Status == 1 ? "Accepted By Owner" : "Rejected By Owner") + (ownerUser.Email));

                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                    }
                    else
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.AlredyApproved, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AlredyApproved };
                    }
                }

                else if (requestObj.ToUserId == UserId && requestObj.Type == 1)
                {
                    if (requestObj.OwnerApprovalStatus == 1)//must approved by owner
                    {
                        requestObj.UpdatedBy = UserId;
                        requestObj.UpdatedDate = Helpers.UTC_To_IST();
                        requestObj.Status = Status;
                        requestObj.RecieverApproveDate = Helpers.UTC_To_IST();
                        requestObj.RecieverApproveBy = UserId;

                        var WalletAuthorizeUserMasterObj = _WalletAuthorizeUserMaster.GetSingle(i => i.WalletID == requestObj.WalletID && i.UserID == UserId);
                        if (Status == 1 && requestObj.Type == 1)
                        {
                            //add entry in authorised tbl
                            if (WalletAuthorizeUserMasterObj == null)
                            {
                                WalletAuthorizeUserMaster obj = new WalletAuthorizeUserMaster();
                                obj.RoleID = requestObj.RoleId;
                                obj.UserID = UserId;
                                obj.Status = 1;
                                obj.CreatedBy = UserId;
                                obj.CreatedDate = Helpers.UTC_To_IST();
                                obj.UpdatedDate = Helpers.UTC_To_IST();
                                obj.WalletID = requestObj.WalletID;
                                obj.OrgID = Convert.ToInt64(walletObj.OrgID);
                                _WalletAuthorizeUserMaster.Add(obj);//add new enrty
                            }
                            else
                            {
                                WalletAuthorizeUserMasterObj.Status = 1;
                                WalletAuthorizeUserMasterObj.UpdatedDate = Helpers.UTC_To_IST();
                                WalletAuthorizeUserMasterObj.UpdatedBy = UserId;
                                _WalletAuthorizeUserMaster.Add(WalletAuthorizeUserMasterObj);//update enrty
                            }
                        }
                        else if (Status == 1 && requestObj.Type == 2)
                        {
                            WalletAuthorizeUserMasterObj.Status = 9;
                            WalletAuthorizeUserMasterObj.UpdatedDate = Helpers.UTC_To_IST();
                            WalletAuthorizeUserMasterObj.UpdatedBy = UserId;
                            _WalletAuthorizeUserMaster.Add(WalletAuthorizeUserMasterObj);//update enrty
                        }

                        _AddRemoveUserWalletRequest.Update(requestObj);//update AddRemoveUserWalletRequest entity

                        //send mail to both From and To user
                        EmailSendAsyncV1(EnTemplateType.Email_RequestApproval, requestObj.FromUserId.ToString(), requestObj.ReceiverEmail, roleObjs.RoleType, typeObj.WalletTypeName, walletObj.Walletname, (requestObj.Type == 1 ? "addition" : "removal"), (Status == 1 ? "Accepted" : "Rejected"));

                        EmailSendAsyncV1(EnTemplateType.Email_RequestApproval, UserId.ToString(), requestObj.ReceiverEmail, roleObjs.RoleType, typeObj.WalletTypeName, walletObj.Walletname, (requestObj.Type == 1 ? "addition" : "removal"), (Status == 1 ? "Accepted" : "Rejected"));

                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                    }
                    else
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.ApprovedByOwner, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ApprovedByOwner };
                    }
                }
                else
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.NotRequestApproved, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotRequestApproved };
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateUserWalletPendingRequest", "WalletService", ex);
                throw;
            }
        }

        public async Task<ListUserWalletWise> ListUserWalletWise(string WalletId)
        {
            try
            {
                ListUserWalletWise Resp = new ListUserWalletWise();
                //2019-2-18 added condi for only used trading wallet
                var walletObj = _commonRepository.GetSingle(i => i.AccWalletID == WalletId && i.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (walletObj == null)
                {
                    Resp.ErrorCode = enErrorCode.InvalidWallet;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InvalidWallet;
                }
                var data = await _walletRepository1.ListUserWalletWise(walletObj.Id);
                Resp.Data = data;
                if (data.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region Staking

        public async Task<ListStakingPolicyDetailRes> GetStakingPolicy(short statkingTypeID, short currencyTypeID)
        {
            try
            {
                ListStakingPolicyDetailRes Resp = new ListStakingPolicyDetailRes();
                var data = _walletRepository1.GetStakingPolicyData(statkingTypeID, currencyTypeID);
                if (data.Count > 0)
                {
                    Resp.Details = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound_StakingScheme;//ntrivedi 24-07-2019 new error code to have different proper message
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListStakingPolicy", "WalletService", ex);
                return null;
            }
        }

        public async Task<ListStakingPolicyDetailResV2> GetStakingPolicyV2(short statkingTypeID, short currencyTypeID)
        {
            try
            {
                ListStakingPolicyDetailResV2 Resp = new ListStakingPolicyDetailResV2();
                var data = _walletRepository1.GetStakingPolicyDataV2(statkingTypeID, currencyTypeID);
                if (data.Count > 0)
                {
                    Resp.Details = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListStakingPolicy", "WalletService", ex);
                return null;
            }
        }

        public async Task<ListPreStackingConfirmationRes> GetPreStackingData(PreStackingConfirmationReq Req, long Userid)
        {
            try
            {
                ListPreStackingConfirmationRes Resp = new ListPreStackingConfirmationRes();
                PreStackingConfirmationRes1 MaturityDetails = new PreStackingConfirmationRes1();
                var data = _walletRepository1.GetPreStackingData(Req.PolicyDetailID);
                bool UpgradeFlag = false; decimal OldStakAmount = 0;
                if (data != null)
                {
                    decimal MaturityAmount = 0, InterestAmount = 0, historyAmount = 0;
                    DateTime MaturityDate = Helpers.UTC_To_IST().AddDays(data.DurationWeek * 7).AddMonths(data.DurationMonth);

                    if (data.StakingType == 1)//Fixed Deposit
                    {
                        if (data.SlabType == 1)//Fixed
                        {
                            if (Req.Amount != data.MinAmount)
                            {
                                Resp.ReturnCode = enResponseCode.Fail;
                                Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                                Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                                return Resp;
                            }
                        }
                        else
                        {
                            if (Req.Amount < data.MinAmount || Req.Amount > data.MaxAmount)
                            {
                                Resp.ReturnCode = enResponseCode.Fail;
                                Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                                Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                                return Resp;
                            }
                        }
                        if (data.InterestType == 1)//Fixed
                        {
                            InterestAmount = data.InterestValue;
                            if (data.InterestWalletTypeID == data.WalletTypeID)
                            {
                                MaturityAmount = Req.Amount + InterestAmount;
                            }
                            else
                            {
                                MaturityAmount = Req.Amount;
                            }
                        }
                        else//Percentage
                        {
                            InterestAmount = ((Req.Amount * data.InterestValue) / 100);
                            if (data.InterestWalletTypeID == data.WalletTypeID)
                            {
                                MaturityAmount = Req.Amount + InterestAmount;
                            }
                            else
                            {
                                MaturityAmount = Req.Amount;
                            }
                        }
                    }
                    else//Charge Type
                    {
                        var detailData = _StakingDetailCommonRepo.GetSingle(i => i.Id == Req.PolicyDetailID);
                        var masterData = _StakingPolicyCommonRepo.GetSingle(i => i.Id == detailData.StakingPolicyID);
                        var historydata = _TokenStakingHistoryCommonRepo.GetSingle(i => i.StakingType == masterData.StakingType && i.UserID == Userid && i.WalletTypeID == masterData.WalletTypeID && i.Status == 1);
                        if (historydata != null)
                        {
                            historyAmount = historydata.MinAmount;
                            OldStakAmount = historydata.StakingAmount;
                        }
                        if (historyAmount > 0)
                        {
                            UpgradeFlag = true;
                        }
                        if (data.SlabType == 2 && Req.Amount < data.MinAmount || Req.Amount > data.MaxAmount)
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                            Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                            return Resp;
                        }
                        if (data.InterestType == 1)
                        {
                            InterestAmount = data.EnableStakingBeforeMaturityCharge;
                            if (data.InterestWalletTypeID == data.WalletTypeID)
                            {
                                MaturityAmount = Req.Amount - InterestAmount;
                            }
                            else
                            {
                                MaturityAmount = Req.Amount;
                            }
                        }
                        else
                        {
                            if (data.EnableStakingBeforeMaturityCharge > 0)
                            {
                                InterestAmount = ((Req.Amount * data.EnableStakingBeforeMaturityCharge) / 100);
                            }
                            if (data.InterestWalletTypeID == data.WalletTypeID)
                            {
                                MaturityAmount = Req.Amount - InterestAmount;
                            }
                            else
                            {
                                MaturityAmount = Req.Amount;
                            }
                        }
                    }
                    if (UpgradeFlag)
                    {
                        MaturityDetails.DeductionAmount = Req.Amount - OldStakAmount;
                    }
                    else
                    {
                        MaturityDetails.DeductionAmount = Req.Amount;
                    }
                    if (MaturityDetails.DeductionAmount <= 0)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidSlabSelection;
                        Resp.ErrorCode = enErrorCode.InvalidSlabSelection;
                        Resp.MaturityDetail = null;
                        Resp.StakingDetails = null;
                        return Resp;
                    }
                    MaturityDetails.IsUpgrade = UpgradeFlag;
                    MaturityDetails.Amount = Req.Amount;
                    MaturityDetails.InterestAmount = InterestAmount;
                    MaturityDetails.MaturityAmount = MaturityAmount;
                    MaturityDetails.MaturityDate = MaturityDate.Date;

                    Resp.StakingDetails = data;
                    Resp.MaturityDetail = MaturityDetails;

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetPreStackingData", "WalletService", ex);
                return null;
            }
        }

        public async Task<ListPreStackingConfirmationRes> GetPreStackingDataForDegrade(PreStackingConfirmationReq Req, long Userid)
        {
            try
            {
                ListPreStackingConfirmationRes Resp = new ListPreStackingConfirmationRes();
                PreStackingConfirmationRes1 MaturityDetails = new PreStackingConfirmationRes1();
                var data = _walletRepository1.GetPreStackingData(Req.PolicyDetailID);
                bool UpgradeFlag = false; decimal OldStakAmount = 0;
                if (data != null)
                {
                    decimal MaturityAmount = 0, InterestAmount = 0, historyAmount = 0;
                    DateTime MaturityDate = Helpers.UTC_To_IST().AddDays((data.DurationWeek * 7) + 1).AddMonths(data.DurationMonth);

                    if (data.StakingType == 1)
                    {
                        if (data.SlabType == 1)
                        {
                            if (Req.Amount != data.MinAmount)
                            {
                                Resp.ReturnCode = enResponseCode.Fail;
                                Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                                Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                                return Resp;
                            }
                        }
                        else
                        {
                            if (Req.Amount < data.MinAmount || Req.Amount > data.MaxAmount)
                            {
                                Resp.ReturnCode = enResponseCode.Fail;
                                Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                                Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                                return Resp;
                            }
                        }
                        if (data.InterestType == 1)
                        {
                            InterestAmount = data.InterestValue;
                            MaturityAmount = Req.Amount + InterestAmount;
                        }
                        else
                        {
                            InterestAmount = ((Req.Amount * data.InterestValue) / 100);
                            MaturityAmount = Req.Amount + InterestAmount;
                        }
                    }
                    else
                    {
                        var detailData = _StakingDetailCommonRepo.GetSingle(i => i.Id == Req.PolicyDetailID);
                        var masterData = _StakingPolicyCommonRepo.GetSingle(i => i.Id == detailData.StakingPolicyID && i.Status == 1);
                        var historydata = _TokenStakingHistoryCommonRepo.GetSingle(i => i.StakingType == masterData.StakingType && i.UserID == Userid && i.WalletTypeID == masterData.WalletTypeID && i.Status == 1);
                        if (historydata != null)
                        {
                            historyAmount = historydata.MinAmount;
                            OldStakAmount = historydata.StakingAmount;
                        }

                        if (data.SlabType == 2 && Req.Amount < data.MinAmount || Req.Amount > data.MaxAmount)
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                            Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                            return Resp;
                        }
                        if (data.InterestType == 1)
                        {
                            InterestAmount = data.EnableStakingBeforeMaturityCharge;
                            MaturityAmount = Req.Amount - InterestAmount;
                        }
                        else
                        {
                            if (data.EnableStakingBeforeMaturityCharge > 0)
                            {
                                InterestAmount = ((Req.Amount * data.EnableStakingBeforeMaturityCharge) / 100);
                            }
                            MaturityAmount = Req.Amount - InterestAmount;
                        }
                    }

                    MaturityDetails.DeductionAmount = 0;

                    MaturityDetails.IsUpgrade = UpgradeFlag;
                    MaturityDetails.Amount = Req.Amount;
                    MaturityDetails.InterestAmount = InterestAmount;
                    MaturityDetails.MaturityAmount = MaturityAmount;
                    MaturityDetails.MaturityDate = MaturityDate.Date;

                    Resp.StakingDetails = data;
                    Resp.MaturityDetail = MaturityDetails;

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetPreStackingData", "WalletService", ex);
                return null;
            }
        }

        public async Task<BizResponseClass> UserStackingRequest(StakingHistoryReq stakingHistoryReq, long UserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                //2019-2-18 added condi for only used trading wallet
                var WalletObj = await _commonRepository.GetSingleAsync(item => item.AccWalletID == stakingHistoryReq.AccWalletID && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (WalletObj != null)
                {
                    var data = _walletSPRepositories.Callsp_StakingSchemeRequest(stakingHistoryReq, UserID, WalletObj.Id, WalletObj.WalletTypeID);
                    if (data != null)
                    {
                        Resp.ReturnCode = data.ReturnCode;
                        Resp.ReturnMsg = data.ReturnMsg;
                        Resp.ErrorCode = data.ErrorCode;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InternalError;
                        Resp.ErrorCode = enErrorCode.InternalError;
                    }
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.WalletNotFound;
                    Resp.ErrorCode = enErrorCode.WalletNotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UserStackingRequest", "WalletService", ex);
                return null;
            }
        }

        public async Task<BizResponseClass> UserStackingRequestv2(StakingHistoryReq stakingHistoryReq, long UserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                //2019-2-18 added condi for only used trading wallet
                var WalletObj = await _commonRepository.GetSingleAsync(item => item.AccWalletID == stakingHistoryReq.AccWalletID && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (WalletObj != null)
                {
                    var data = _walletSPRepositories.Callsp_StakingSchemeRequestv2(stakingHistoryReq, UserID, WalletObj.Id, WalletObj.WalletTypeID);
                    if (data != null)
                    {
                        Resp.ReturnCode = data.ReturnCode;
                        Resp.ReturnMsg = data.ReturnMsg;
                        Resp.ErrorCode = data.ErrorCode;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InternalError;
                        Resp.ErrorCode = enErrorCode.InternalError;
                    }
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.WalletNotFound;
                    Resp.ErrorCode = enErrorCode.WalletNotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UserStackingRequest", "WalletService", ex);
                return null;
            }
        }


        public async Task<ListStakingHistoryRes> GetStackingHistoryData(DateTime? FromDate, DateTime? ToDate, EnStakeUnStake? Type, int PageSize, int PageNo, EnStakingSlabType? Slab, EnStakingType? StakingType, long UserID)
        {
            try
            {
                ListStakingHistoryRes Resp = new ListStakingHistoryRes();
                Resp.PageNo = PageNo;
                PageNo = PageNo + 1;
                if (PageNo <= 0 || PageSize <= 0)
                {
                    Resp.ErrorCode = enErrorCode.InValidPage;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InValidPage;
                    return Resp;
                }
                int TotalCount = 0;
                var obj = _walletRepository1.GetStackingHistoryData(FromDate, ToDate, Type, PageSize, PageNo, Slab, StakingType, UserID, ref TotalCount);
                Resp.Stakings = obj;
                Resp.TotalCount = TotalCount;

                Resp.PageSize = PageSize;

                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetStackingHistoryData", "WalletService", ex);
                return null;
            }
        }

        public async Task<ListStakingHistoryResv2> GetStackingHistoryDatav2(DateTime? FromDate, DateTime? ToDate, EnStakeUnStake? Type, int PageSize, int PageNo, EnStakingSlabType? Slab, EnStakingType? StakingType, long UserID)
        {
            try
            {
                ListStakingHistoryResv2 Resp = new ListStakingHistoryResv2();
                Resp.PageNo = PageNo;
                PageNo = PageNo + 1;
                if (PageNo <= 0 || PageSize <= 0)
                {
                    Resp.ErrorCode = enErrorCode.InValidPage;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InValidPage;
                    return Resp;
                }
                int TotalCount = 0;
                var obj = _walletRepository1.GetStackingHistoryDatav2(FromDate, ToDate, Type, PageSize, PageNo, Slab, StakingType, UserID, ref TotalCount);
                Resp.Stakings = obj;
                Resp.TotalCount = TotalCount;

                Resp.PageSize = PageSize;

                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetStackingHistoryData", "WalletService", ex);
                return null;
            }
        }

        public async Task<UnstakingDetailRes> GetPreUnstackingData(PreUnstackingConfirmationReq Request, long UserID)
        {
            try
            {
                UnstakingDetailRes Resp = new UnstakingDetailRes();
                PreUnstackingConfirmationRes data = new PreUnstackingConfirmationRes();

                //PreUnstackingConfirmationRes UnstakingDetail = new PreUnstackingConfirmationRes();
                PreStackingConfirmationRes1 MaturityDetails = new PreStackingConfirmationRes1();
                PreStackingConfirmationRes StakingDetail = new PreStackingConfirmationRes();

                var HistoryData = await _TokenStakingHistoryCommonRepo.GetSingleAsync(item => item.Id == Request.StakingHistoryId && item.Status == 1);//status always 1
                if (HistoryData == null)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                if (HistoryData.StakingType == 1)//fd
                {
                    if (Request.UnstakingType != EnUnstakeType.Full)
                    {
                        Resp.ErrorCode = enErrorCode.InvalidUnstakeType;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.OnlyFullUnstakeAvailable;
                        return Resp;
                    }
                }
                if (Request.UnstakingType == EnUnstakeType.Full)
                {
                    if (HistoryData.EnableAutoUnstaking != 1)
                    {
                        Resp.ErrorCode = enErrorCode.FullUnstakingNotAvailable;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.UnstakingNotAvailable;
                        return Resp;
                    }
                    if (Helpers.UTC_To_IST() < HistoryData.MaturityDate)
                    {
                        data.CreditAmount = HistoryData.StakingAmount - HistoryData.EnableStakingBeforeMaturityCharge;
                    }
                    else
                    {
                        data.CreditAmount = HistoryData.StakingAmount;
                    }
                    data.BeforeMaturityChargeDeduction = HistoryData.EnableStakingBeforeMaturityCharge;
                    data.StakingAmount = HistoryData.StakingAmount;
                    data.StakingHistoryId = HistoryData.Id;
                    Resp.UnstakingDetail = data;

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                    if (HistoryData.EnableStakingBeforeMaturity == 1)
                    {
                        return Resp;
                    }
                    else
                    {
                        Resp.ErrorCode = enErrorCode.UnstakingNotAllowed;
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.UnstakingNotAllowed;
                        return Resp;
                    }
                }
                else
                {
                    if (Request.DegradePolicyDetailID <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.DegradePolicyDetailIDrequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidStakingPolicyDetailID;
                        return Resp;
                    }
                    if (Request.Amount <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                        return Resp;
                    }
                    if (Request.ChannelId <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.ChannelIDRequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidChannel;
                        return Resp;
                    }

                    var NewPolicyData = await _StakingDetailCommonRepo.GetSingleAsync(i => i.Id == Request.DegradePolicyDetailID && i.Status == 1);

                    PreStackingConfirmationReq req = new PreStackingConfirmationReq();

                    if (NewPolicyData == null)
                    {
                        Resp.ErrorCode = enErrorCode.NoDataFound;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                        return Resp;
                    }
                    else
                    {
                        req.Amount = Request.Amount;
                        req.PolicyDetailID = NewPolicyData.Id;
                        var PreStakeRes = await GetPreStackingDataForDegrade(req, UserID);

                        if (PreStakeRes.ErrorCode != enErrorCode.Success)
                        {
                            Resp.ErrorCode = PreStakeRes.ErrorCode;
                            Resp.ReturnCode = PreStakeRes.ReturnCode;
                            Resp.ReturnMsg = PreStakeRes.ReturnMsg;
                            return Resp;
                        }
                        StakingDetail = PreStakeRes.StakingDetails;
                        MaturityDetails = PreStakeRes.MaturityDetail;
                        if (Helpers.UTC_To_IST() < HistoryData.MaturityDate)
                        {
                            data.NewStakingAmountDeduction = Request.Amount;
                            data.CreditAmount = (HistoryData.StakingAmount - (HistoryData.EnableStakingBeforeMaturityCharge + MaturityDetails.Amount));
                        }
                        data.BeforeMaturityChargeDeduction = HistoryData.EnableStakingBeforeMaturityCharge;
                        data.StakingAmount = HistoryData.StakingAmount;
                        data.StakingHistoryId = HistoryData.Id;

                        Resp.UnstakingDetail = data;
                        Resp.NewMaturityDetail = MaturityDetails;
                        Resp.NewStakingDetails = StakingDetail;

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.FindRecored;
                        Resp.ErrorCode = enErrorCode.Success;

                        if (HistoryData.EnableStakingBeforeMaturity == 1)
                        {
                            return Resp;
                        }
                        else
                        {
                            Resp.ErrorCode = enErrorCode.UnstakingNotAllowed;
                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.UnstakingNotAllowed;
                            return Resp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetPreUnstackingData", "WalletService", ex);
                return null;
            }
        }

        public async Task<UnstakingDetailResv2> GetPreUnstackingDatav2(PreUnstackingConfirmationReqv2 Request, long UserID)
        {
            try
            {
                UnstakingDetailResv2 Resp = new UnstakingDetailResv2();
                PreUnstackingConfirmationResv2 data = new PreUnstackingConfirmationResv2();

                //PreUnstackingConfirmationRes UnstakingDetail = new PreUnstackingConfirmationRes();
                PreStackingConfirmationRes1 MaturityDetails = new PreStackingConfirmationRes1();
                PreStackingConfirmationRes StakingDetail = new PreStackingConfirmationRes();

                var HistoryData = await _TokenStakingHistoryCommonRepo.GetSingleAsync(item => item.GUID == Request.StakingHistoryId && item.Status == 1);//status always 1
                if (HistoryData == null)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                if (HistoryData.StakingType == 1)//fd
                {
                    if (Request.UnstakingType != EnUnstakeType.Full)
                    {
                        Resp.ErrorCode = enErrorCode.InvalidUnstakeType;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.OnlyFullUnstakeAvailable;
                        return Resp;
                    }
                }
                if (Request.UnstakingType == EnUnstakeType.Full)
                {
                    if (HistoryData.EnableAutoUnstaking != 1)
                    {
                        Resp.ErrorCode = enErrorCode.FullUnstakingNotAvailable;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.UnstakingNotAvailable;
                        return Resp;
                    }
                    if (Helpers.UTC_To_IST() < HistoryData.MaturityDate)
                    {
                        data.CreditAmount = HistoryData.StakingAmount - HistoryData.EnableStakingBeforeMaturityCharge;
                    }
                    else
                    {
                        data.CreditAmount = HistoryData.StakingAmount;
                    }
                    data.BeforeMaturityChargeDeduction = HistoryData.EnableStakingBeforeMaturityCharge;
                    data.StakingAmount = HistoryData.StakingAmount;
                    data.StakingHistoryId = HistoryData.GUID;
                    Resp.UnstakingDetail = data;

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                    if (HistoryData.EnableStakingBeforeMaturity == 1)
                    {
                        return Resp;
                    }
                    else
                    {
                        Resp.ErrorCode = enErrorCode.UnstakingNotAllowed;
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.UnstakingNotAllowed;
                        return Resp;
                    }
                }
                else
                {
                    if (Request.DegradePolicyDetailID <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.DegradePolicyDetailIDrequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidStakingPolicyDetailID;
                        return Resp;
                    }
                    if (Request.Amount <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                        return Resp;
                    }
                    if (Request.ChannelId <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.ChannelIDRequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidChannel;
                        return Resp;
                    }

                    var NewPolicyData = await _StakingDetailCommonRepo.GetSingleAsync(i => i.Id == Request.DegradePolicyDetailID && i.Status == 1);

                    PreStackingConfirmationReq req = new PreStackingConfirmationReq();

                    if (NewPolicyData == null)
                    {
                        Resp.ErrorCode = enErrorCode.NoDataFound;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                        return Resp;
                    }
                    else
                    {
                        req.Amount = Request.Amount;
                        req.PolicyDetailID = NewPolicyData.Id;
                        var PreStakeRes = await GetPreStackingDataForDegrade(req, UserID);

                        if (PreStakeRes.ErrorCode != enErrorCode.Success)
                        {
                            Resp.ErrorCode = PreStakeRes.ErrorCode;
                            Resp.ReturnCode = PreStakeRes.ReturnCode;
                            Resp.ReturnMsg = PreStakeRes.ReturnMsg;
                            return Resp;
                        }
                        StakingDetail = PreStakeRes.StakingDetails;
                        MaturityDetails = PreStakeRes.MaturityDetail;
                        if (Helpers.UTC_To_IST() < HistoryData.MaturityDate)
                        {
                            data.NewStakingAmountDeduction = Request.Amount;
                            data.CreditAmount = (HistoryData.StakingAmount - (HistoryData.EnableStakingBeforeMaturityCharge + MaturityDetails.Amount));
                        }
                        data.BeforeMaturityChargeDeduction = HistoryData.EnableStakingBeforeMaturityCharge;
                        data.StakingAmount = HistoryData.StakingAmount;
                        data.StakingHistoryId = HistoryData.GUID;

                        Resp.UnstakingDetail = data;
                        Resp.NewMaturityDetail = MaturityDetails;
                        Resp.NewStakingDetails = StakingDetail;

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.FindRecored;
                        Resp.ErrorCode = enErrorCode.Success;

                        if (HistoryData.EnableStakingBeforeMaturity == 1)
                        {
                            return Resp;
                        }
                        else
                        {
                            Resp.ErrorCode = enErrorCode.UnstakingNotAllowed;
                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.UnstakingNotAllowed;
                            return Resp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetPreUnstackingData", "WalletService", ex);
                return null;
            }
        }

        public async Task<BizResponseClass> UserUnstackingRequest(UserUnstakingReq request, long UserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                var NewPolicyObj = await _StakingDetailCommonRepo.GetSingleAsync(item => item.Id == request.StakingPolicyDetailId);
                var HistoryObj = await _TokenStakingHistoryCommonRepo.GetSingleAsync(item => item.Id == request.StakingHistoryId);

                if (request.StakingPolicyDetailId > 0 && NewPolicyObj == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                    return Resp;
                }
                if (HistoryObj != null)
                {
                    if (request.Type != EnUnstakeType.Full)
                    {
                        if (NewPolicyObj.MinAmount >= HistoryObj.MinAmount)
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidSlabSelectionForUnstak;
                            Resp.ErrorCode = enErrorCode.InvalidSlabSelection;
                            return Resp;
                        }
                    }
                    #region Old Logic
                    //DateTime TodayDate = Helpers.UTC_To_IST().AddHours(0).AddMinutes(0).AddSeconds(0);
                    //if (HistoryObj.EnableStakingBeforeMaturity != 1 || HistoryObj.MaturityDate > TodayDate)
                    //{
                    // TokenUnStakingHistory obj = new TokenUnStakingHistory
                    // {
                    // TokenStakingHistoryID = HistoryObj.Id,
                    // UnstakeType = Convert.ToInt16(request.Type),
                    // Status = Convert.ToInt16(ServiceStatus.InActive),
                    // DegradeStakingHistoryRequestID = request.StakingPolicyDetailId,
                    // DegradeStakingAmount = request.StakingAmount,
                    // ChargeBeforeMaturity = HistoryObj.EnableStakingBeforeMaturityCharge,
                    // InterestCreditedValue = 0,
                    // AmountCredited = 0,
                    // CreatedBy = UserID,
                    // CreatedDate = Helpers.UTC_To_IST()
                    // };
                    // await _TokenUnstakingHistoryCommonRepo.AddAsync(obj);

                    // HistoryObj.Status = 4;
                    // HistoryObj.Remarks = "Requested For Unstaking To Admin";
                    // HistoryObj.UpdatedBy = UserID;
                    // HistoryObj.UpdatedDate = Helpers.UTC_To_IST();
                    // _TokenStakingHistoryCommonRepo.UpdateWithAuditLog(HistoryObj);

                    // Resp.ReturnCode = enResponseCode.Success;
                    // Resp.ReturnMsg = EnResponseMessage.UnstakingSuccessfully;
                    // Resp.ErrorCode = enErrorCode.Success;
                    //}
                    //else
                    //{
                    // var data = _walletSPRepositories.Callsp_UnstakingSchemeRequest(request, UserID, 0);
                    // if (data != null)
                    // {
                    // Resp.ReturnCode = data.ReturnCode;
                    // Resp.ReturnMsg = data.ReturnMsg;
                    // Resp.ErrorCode = data.ErrorCode;
                    // }
                    // else
                    // {
                    // Resp.ReturnCode = enResponseCode.Fail;
                    // Resp.ReturnMsg = EnResponseMessage.InternalError;
                    // Resp.ErrorCode = enErrorCode.InternalError;
                    // }
                    //}
                    #endregion
                    var data = _walletSPRepositories.Callsp_UnstakingSchemeRequest(request, UserID, 0);
                    if (data != null)
                    {
                        Resp.ReturnCode = data.ReturnCode;
                        Resp.ReturnMsg = data.ReturnMsg;
                        Resp.ErrorCode = data.ErrorCode;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InternalError;
                        Resp.ErrorCode = enErrorCode.InternalError;
                    }
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UserUnstackingRequest", "WalletService", ex);
                return null;
            }
        }

        public async Task<BizResponseClass> UserUnstackingRequestv2(UserUnstakingReqv2 request, long UserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                var NewPolicyObj = await _StakingDetailCommonRepo.GetSingleAsync(item => item.Id == request.StakingPolicyDetailId);
                var HistoryObj = await _TokenStakingHistoryCommonRepo.GetSingleAsync(item => item.GUID == request.StakingHistoryId);

                if (request.StakingPolicyDetailId > 0 && NewPolicyObj == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                    return Resp;
                }
                if (HistoryObj != null)
                {
                    if (request.Type != EnUnstakeType.Full)
                    {
                        if (NewPolicyObj.MinAmount >= HistoryObj.MinAmount)
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidSlabSelectionForUnstak;
                            Resp.ErrorCode = enErrorCode.InvalidSlabSelection;
                            return Resp;
                        }
                    }
                    DateTime TodayDate = Helpers.UTC_To_IST().AddHours(0).AddMinutes(0).AddSeconds(0);
                    if (HistoryObj.EnableStakingBeforeMaturity != 1 || HistoryObj.MaturityDate > TodayDate)
                    {
                        TokenUnStakingHistory obj = new TokenUnStakingHistory
                        {
                            TokenStakingHistoryID = HistoryObj.Id,
                            UnstakeType = Convert.ToInt16(request.Type),
                            Status = Convert.ToInt16(ServiceStatus.InActive),
                            DegradeStakingHistoryRequestID = request.StakingPolicyDetailId,
                            DegradeStakingAmount = request.StakingAmount,
                            ChargeBeforeMaturity = HistoryObj.EnableStakingBeforeMaturityCharge,
                            InterestCreditedValue = 0,
                            AmountCredited = 0,
                            CreatedBy = UserID,
                            CreatedDate = Helpers.UTC_To_IST()
                        };
                        await _TokenUnstakingHistoryCommonRepo.AddAsync(obj);

                        HistoryObj.Status = 4;
                        HistoryObj.Remarks = "Requested For Unstaking To Admin";
                        HistoryObj.UpdatedBy = UserID;
                        HistoryObj.UpdatedDate = Helpers.UTC_To_IST();
                        _TokenStakingHistoryCommonRepo.UpdateWithAuditLog(HistoryObj);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.UnstakingSuccessfully;
                        Resp.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        UserUnstakingReq datareq = new UserUnstakingReq();
                        datareq.ChannelID = request.ChannelID;
                        datareq.StakingAmount = request.StakingAmount;
                        datareq.StakingHistoryId = HistoryObj.Id;
                        datareq.StakingPolicyDetailId = request.StakingPolicyDetailId;
                        datareq.Type = request.Type;
                        var data = _walletSPRepositories.Callsp_UnstakingSchemeRequestv2(datareq, UserID, 0);
                        if (data != null)
                        {
                            Resp.ReturnCode = data.ReturnCode;
                            Resp.ReturnMsg = data.ReturnMsg;
                            Resp.ErrorCode = data.ErrorCode;
                        }
                        else
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InternalError;
                            Resp.ErrorCode = enErrorCode.InternalError;
                        }
                    }
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UserUnstackingRequest", "WalletService", ex);
                return null;
            }
        }

        #endregion

        #region Wallet Sharing Methods
        public async Task<ListWalletMasterRes> ListWalletMasterResponseNew(long UserId, string Coin)
        {
            ListWalletMasterRes Resp = new ListWalletMasterRes();
            try
            {
                var data = await _walletRepository1.ListWalletMasterResponseNew(UserId, Coin);
                Resp.Data = data;
                if (data.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public ListWalletResNew GetWalletByCoinNew(long userid, string coin)
        {
            ListWalletResNew listWalletResponse = new ListWalletResNew();
            try
            {
                var walletResponse = _walletRepository1.GetWalletMasterResponseByCoinNew(userid, coin).GetAwaiter().GetResult();
                var UserPrefobj = _UserPreferencescommonRepository.FindBy(item => item.UserID == userid && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (walletResponse.Count == 0)
                {
                    listWalletResponse.ReturnCode = enResponseCode.Fail;
                    listWalletResponse.ReturnMsg = EnResponseMessage.NotFound;
                    listWalletResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    if (UserPrefobj != null)
                    {
                        listWalletResponse.IsWhitelisting = UserPrefobj.IsWhitelisting;
                    }
                    else
                    {
                        listWalletResponse.IsWhitelisting = 0;
                    }
                    listWalletResponse.Wallets = walletResponse;
                    listWalletResponse.ReturnCode = enResponseCode.Success;
                    listWalletResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    listWalletResponse.ErrorCode = enErrorCode.Success;

                }
                return listWalletResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public ListWalletResNew GetWalletByIdNew(long userid, string walletId)
        {
            ListWalletResNew listWalletResponse = new ListWalletResNew();
            try
            {
                var walletResponse = _walletRepository1.GetWalletMasterResponseByIdNew(userid, walletId).GetAwaiter().GetResult();
                if (walletResponse.Count == 0)
                {
                    listWalletResponse.ReturnCode = enResponseCode.Fail;
                    listWalletResponse.ReturnMsg = EnResponseMessage.NotFound;
                    listWalletResponse.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    listWalletResponse.Wallets = walletResponse;
                    listWalletResponse.ReturnCode = enResponseCode.Success;
                    listWalletResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    listWalletResponse.ErrorCode = enErrorCode.Success;
                }
                return listWalletResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                listWalletResponse.ReturnCode = enResponseCode.InternalError;
                return listWalletResponse;
            }
        }

        public ListAllBalanceTypeWiseRes GetAllBalancesTypeWiseNew(long userId, string WalletType)
        {
            try
            {
                ListAllBalanceTypeWiseRes res = new ListAllBalanceTypeWiseRes();

                List<AllBalanceTypeWiseRes> Response = new List<AllBalanceTypeWiseRes>();
                res.BizResponseObj = new Core.ApiModels.BizResponseClass();

                var listWallet = _walletRepository1.GetWalletMasterResponseByCoinNew(userId, WalletType).GetAwaiter().GetResult();
                if (listWallet.Count() == 0)
                {
                    res.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    res.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    res.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    return res;
                }
                for (int i = 0; i <= listWallet.Count - 1; i++)
                {
                    AllBalanceTypeWiseRes a = new AllBalanceTypeWiseRes();
                    a.Wallet = new WalletResponse();
                    a.Wallet.Balance = new Balance();

                    //2019-2-18 added condi for only used trading wallet
                    var wallet = _commonRepository.GetSingle(item => item.AccWalletID == listWallet[i].AccWalletID && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                    var response = _walletRepository1.GetAllBalancesNew(userId, wallet.Id);

                    a.Wallet.AccWalletID = listWallet[i].AccWalletID;
                    a.Wallet.PublicAddress = listWallet[i].PublicAddress;
                    a.Wallet.WalletName = listWallet[i].WalletName;
                    a.Wallet.IsDefaultWallet = listWallet[i].IsDefaultWallet;
                    a.Wallet.TypeName = listWallet[i].CoinName;

                    a.Wallet.Balance = response;
                    Response.Add(a);
                }
                if (Response.Count() == 0)
                {
                    res.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    res.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    res.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    return res;
                }
                res.Wallets = Response;
                res.BizResponseObj.ReturnCode = enResponseCode.Success;
                res.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                res.BizResponseObj.ErrorCode = enErrorCode.Success;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public AllBalanceResponse GetAllBalancesNew(long userid, string walletId)
        {

            AllBalanceResponse allBalanceResponse = new AllBalanceResponse();
            allBalanceResponse.BizResponseObj = new BizResponseClass();
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var wallet = _commonRepository.GetSingle(item => item.AccWalletID == walletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (wallet == null)
                {
                    allBalanceResponse.BizResponseObj.ErrorCode = enErrorCode.InvalidWallet;
                    allBalanceResponse.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    allBalanceResponse.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return allBalanceResponse;
                }
                var response = _walletRepository1.GetAllBalancesNew(userid, wallet.Id);
                if (response == null)
                {
                    allBalanceResponse.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    allBalanceResponse.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    allBalanceResponse.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return allBalanceResponse;
                }
                allBalanceResponse.BizResponseObj.ReturnCode = enResponseCode.Success;
                allBalanceResponse.BizResponseObj.ErrorCode = enErrorCode.Success;
                allBalanceResponse.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                allBalanceResponse.Balance = response;
                //vsolanki 2018-10-27 //for withdraw limit
                var limit = _LimitcommonRepository.GetSingle(item => item.TrnType == 2 && item.WalletId == wallet.Id);
                if (limit == null)
                {
                    allBalanceResponse.WithdrawalDailyLimit = 0;
                }
                else if (limit.LimitPerDay < 0) //ntrivedi 21-11-2018 if limit null then exception so add else if instead of only if
                {
                    allBalanceResponse.WithdrawalDailyLimit = 0;
                }
                else
                {
                    allBalanceResponse.WithdrawalDailyLimit = limit.LimitPerDay;

                }
                // var wallet = _commonRepository.GetById(walletId);
                var walletType = _WalletTypeMasterRepository.GetById(wallet.WalletTypeID);
                allBalanceResponse.WalletType = walletType.WalletTypeName;
                allBalanceResponse.WalletName = wallet.Walletname;
                allBalanceResponse.IsDefaultWallet = wallet.IsDefaultWallet;
                return allBalanceResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public ListBalanceResponse GetAvailableBalanceNew(long userid, string walletId)
        {
            ListBalanceResponse Response = new ListBalanceResponse();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var wallet = _commonRepository.GetSingle(item => item.AccWalletID == walletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (wallet == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.InvalidWalletId;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }
                var response = _walletRepository1.GetAvailableBalanceNew(userid, wallet.Id);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.Response = response;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public TotalBalanceRes GetAllAvailableBalanceNew(long userid)
        {
            TotalBalanceRes Response = new TotalBalanceRes();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                var response = _walletRepository1.GetAllAvailableBalanceNew(userid);
                decimal total = _walletRepository1.GetTotalAvailbleBalNew(userid);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.Response = response;
                Response.TotalBalance = total;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BalanceResponseWithLimit GetAvailbleBalTypeWiseNew(long userid)
        {
            BalanceResponseWithLimit Response = new BalanceResponseWithLimit();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                var response = _walletRepository1.GetAvailbleBalTypeWiseNew(userid);
                if (response.Count == 0)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.NotFound;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                decimal total = _walletRepository1.GetTotalAvailbleBalNew(userid);
                //vsolanki 26-10-2018
                var walletType = _WalletTypeMasterRepository.GetSingle(item => item.IsDefaultWallet == 1);
                if (walletType == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.InvalidCoinName;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidCoin;
                    return Response;
                }
                var wallet = _commonRepository.GetSingle(item => item.IsDefaultWallet == 1 && item.WalletTypeID == walletType.Id && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (wallet == null)
                {
                    Response.BizResponseObj.ErrorCode = enErrorCode.InvalidWallet;
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }

                var limit = _LimitcommonRepository.GetSingle(item => item.TrnType == 9 && item.WalletId == wallet.Id);//for withdraw

                if (limit == null)
                {
                    Response.DailyLimit = 0;

                }
                else
                {
                    Response.DailyLimit = limit.LimitPerDay;

                }
                //get amt from  tq
                var amt = _walletRepository1.GetTodayAmountOfTQ(userid, wallet.Id);

                if (response.Count == 0)
                {
                    Response.UsedLimit = 0;

                }
                else
                {
                    Response.UsedLimit = amt;
                }
                Response.BizResponseObj.ReturnCode = enResponseCode.Success;
                Response.BizResponseObj.ReturnMsg = EnResponseMessage.FindRecored;
                Response.BizResponseObj.ErrorCode = enErrorCode.Success;
                Response.Response = response;
                Response.TotalBalance = total;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        #endregion

        public async Task<BizResponseClass> AddConvertedAddress(string address, string convertedAddress, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _addressMstRepository.GetSingleAsync(item => item.Address == address && item.CreatedBy == id);
                if (IsExist != null)
                {
                    IsExist.Address = convertedAddress;
                    IsExist.UpdatedBy = id;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    _addressMstRepository.UpdateWithAuditLog(IsExist);

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddConvertedAddress", "WalletService", ex);
                return null;
            }
        }

        #region ColdWallet
        //2019-1-15 create cold wallet
        public async Task<BizResponseClass> ColdWallet(string Coin, InsertColdWalletRequest req, long UserId)
        {
            try
            {
                var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = Coin.ToLower(), amount = 0, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.GenerateColdWallet) });
                transactionProviderResponses = await providerdata;

                var apiconfig = _thirdPartyCommonRepository.GetByIdAsync(transactionProviderResponses[0].ThirPartyAPIID);
                thirdPartyAPIConfiguration = await apiconfig;
                //request
                string reqBody = "{\"label\":\"" + req.WalletLabel + "\",\"passphrase\":\"" + req.Password + "\"}";
                thirdPartyAPIRequest = _getWebRequest.MakeWebRequestColdWallet(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID, reqBody, Coin);
                ////response
                string apiResponse = _webApiSendRequest.SendAPIRequestAsyncWallet(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);

                var jsonResponse = JsonConvert.DeserializeObject<RootObject>(apiResponse);
                var jsonErrorResponse = JsonConvert.DeserializeObject<ErrorRootObject>(apiResponse);
                if (jsonErrorResponse.error != null)
                {
                    HelperForLog.WriteLogForConnection(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, jsonErrorResponse.error + "Msg" + jsonErrorResponse.message);
                    return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg, ErrorCode = enErrorCode.InternalError };
                }
                //insert into walletmaster
                #region InsertIntoDB
                WalletMaster wm = new WalletMaster();
                wm.CreatedDate = Helpers.UTC_To_IST();
                wm.CreatedBy = UserId;
                wm.UpdatedBy = UserId;
                wm.Status = 10;
                wm.Balance = jsonResponse.balance;
                wm.WalletTypeID = _WalletTypeMasterRepository.GetSingle(i => i.WalletTypeName == Coin).Id;
                wm.IsValid = true;
                wm.UpdatedDate = Helpers.UTC_To_IST();
                wm.UserID = UserId;
                wm.Walletname = req.WalletLabel;
                wm.AccWalletID = RandomGenerateAccWalletId(UserId, 0);
                wm.IsDefaultWallet = 0;
                wm.PublicAddress = jsonResponse.receiveAddress.address;
                wm.InBoundBalance = 0;
                wm.OutBoundBalance = 0;
                wm.ExpiryDate = Helpers.UTC_To_IST().AddYears(1);
                wm.OrgID = 1;
                wm.WalletUsageType = Convert.ToInt16(EnWalletUsageType.Cold_Wallet);
                var wMaster = _commonRepository.Add(wm);
                ColdWalletMaster cm = new ColdWalletMaster();
                cm.CreatedDate = Helpers.UTC_To_IST();
                cm.CreatedBy = UserId;
                cm.UpdatedBy = UserId;
                cm.Status = 10;
                cm.UpdatedDate = Helpers.UTC_To_IST();
                cm.KeyId1 = jsonResponse.keys[0];
                cm.KeyId2 = jsonResponse.keys[1];
                cm.KeyId3 = jsonResponse.keys[2];
                cm.BackUpKey = jsonResponse.keySignatures.backupPub;
                cm.PublicKey = jsonResponse.keySignatures.bitgoPub;
                cm.UserKey = "";
                cm.Recoverable = Convert.ToInt16(jsonResponse.recoverable);
                cm.WalletId = wMaster.Id;
                _ColdWalletMaster.Add(cm);
                #endregion
                return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.CreateWalletSuccessMsg, ErrorCode = enErrorCode.Success };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        #endregion

        public StatisticsDetailData GetMonthwiseWalletStatistics(long UserID, short Month, short Year)
        {
            try
            {
                StatisticsDetailData Resp = new StatisticsDetailData();
                var trandata = _walletRepository1.GetWalletStatisticsdata(UserID, Month, Year);
                var baldata = _walletSPRepositories.Callsp_GetWalletBalanceStatistics(UserID, Month, Year);

                if (trandata.Count > 0 && baldata != null)
                {
                    WalletStatisticsData balobj = new WalletStatisticsData();
                    balobj.StartingBalance = baldata.StartingBalance;
                    balobj.EndingBalance = baldata.EndingBalance;
                    balobj.TranAmount = trandata;
                    Resp.Balances = balobj;
                    Resp.BaseCurrency = _configuration["BaseCurrencyName"].ToString();

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetMonthwiseWalletStatistics", "WalletService", ex);
                return null;
            }
        }

        public StatisticsDetailData2 GetYearwiseWalletStatistics(long UserID, short Year)
        {
            try
            {
                StatisticsDetailData2 Resp = new StatisticsDetailData2();
                var trandata = _walletRepository1.GetYearlyWalletStatisticsdata(UserID, Year).GroupBy(e => e.Month);
                var baldata = _walletSPRepositories.Callsp_GetWalletBalanceStatistics(UserID, 0, Year);
                if (trandata != null && baldata != null)
                {
                    WalletStatisticsData2 balobj = new WalletStatisticsData2();
                    TempClass Monthwise = new TempClass();
                    List<MonthWiseData> Month;
                    List<WalletTransactiondata> data;
                    balobj.StartingBalance = baldata.StartingBalance;
                    balobj.EndingBalance = baldata.EndingBalance;
                    Month = new List<MonthWiseData>();
                    foreach (var monthdata in trandata)
                    {
                        data = new List<WalletTransactiondata>();
                        foreach (var x in monthdata)
                        {
                            data.Add(new WalletTransactiondata()
                            {
                                TotalAmount = x.TotalAmount,
                                TotalCount = x.TotalCount,
                                TrnTypeId = x.TrnTypeID,
                                TrnTypeName = x.TrnTypeName
                            });
                        }
                        Month.Add(new MonthWiseData()
                        {
                            Data = data,
                            Month = monthdata.Key
                        });

                    }
                    Monthwise.TranAmount = Month;
                    balobj.MonthwiseData = Monthwise;
                    Resp.Balances = balobj;
                    Resp.BaseCurrency = _configuration["BaseCurrencyName"].ToString();
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetYearwiseWalletStatistics", "WalletService", ex);
                return null;
            }
        }

        #region ERC20

        public CreateWalletAddressRes CreateERC20Address(long UserId, string Coin, string AccWalletId, short IsLocal = 0)
        {
            try
            {
                UserActivityLog activityLog = new UserActivityLog();
                string password = "";
                string sitename = null;
                string siteid = null;
                string Respaddress = null;

                var wallettype = _WalletTypeMasterRepository.GetSingle(t => t.WalletTypeName == Coin && t.IsLocal == IsLocal);
                if (wallettype == null)
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidCoinName, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidCoin };
                }
                //2019-2-18 added condi for only used trading wallet
                var walletMasterobj = _commonRepository.GetSingle(item => item.AccWalletID == AccWalletId && item.Status == Convert.ToInt16(ServiceStatus.Active) && item.WalletTypeID == wallettype.Id && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

                if (walletMasterobj == null)
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidWallet, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet };
                }

                var addressObj = _addressMstRepository.GetSingle(i => i.WalletId == walletMasterobj.Id && i.Status == 1);
                if (addressObj != null)
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.AddressExist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AddressExist };
                }

                var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = Coin.ToLower(), amount = 0, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.Generate_Address) });

                transactionProviderResponses = providerdata.GetAwaiter().GetResult();
                if (transactionProviderResponses == null || transactionProviderResponses.Count == 0)
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.ItemNotFoundForGenerateAddress, ReturnCode = enResponseCode.Fail, ReturnMsg = "Please try after sometime." };
                }
                if (transactionProviderResponses[0].ThirPartyAPIID == 0)
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                var apiconfig = _thirdPartyCommonRepository.GetById(transactionProviderResponses[0].ThirPartyAPIID);

                thirdPartyAPIConfiguration = apiconfig;
                if (thirdPartyAPIConfiguration == null || transactionProviderResponses.Count == 0)
                {
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                password = RandomGeneratePaasword(UserId);
                sitename = _configuration["sitename"].ToString();
                siteid = _configuration["site_id"].ToString();

                thirdPartyAPIRequest = _getWebRequest.MakeWebRequestERC20(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID, password, sitename, siteid);

                string apiResponse = "";
                if (IsLocal == 5)
                {
                    apiResponse = _webApiSendRequest.SendJsonRpcAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody);
                }
                else
                {
                    apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);
                }
                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses[0].ThirPartyAPIID, 0);
                if (IsLocal == 5)
                {
                    if (string.IsNullOrEmpty(Respaddress))
                    {
                        return new CreateWalletAddressRes { ErrorCode = enErrorCode.AddressGenerationFailed, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
                    }
                }
                else
                {
                    if (ParsedResponse.Status != enTransactionStatus.Success)
                    {
                        return new CreateWalletAddressRes { ErrorCode = enErrorCode.AddressGenerationFailed, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
                    }
                }

                Respaddress = ParsedResponse.Param1;
                var Key = ParsedResponse.Param2;
                if (Respaddress != null)
                {
                    string responseString = Respaddress;
                    var Obj = _walletRepository1.AddAddressIntoDB(UserId, responseString, ParsedResponse.TrnRefNo, Key, transactionProviderResponses[0].SerProDetailID, IsLocal);
                    if (Obj == true)
                    {
                        #region SMS_Email
                        ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
                        ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.GenerateAddressNotification);
                        ActivityNotification.Param1 = walletMasterobj.Walletname;
                        ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);

                        SMSSendAsyncV1(EnTemplateType.SMS_WalletAddressCreated, walletMasterobj.UserID.ToString(), walletMasterobj.Walletname);
                        Parallel.Invoke(() => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletAddressCreated, walletMasterobj.UserID.ToString(), wallettype.WalletTypeName, walletMasterobj.Walletname, Helpers.UTC_To_IST().ToString(), walletMasterobj.PublicAddress, walletMasterobj.Balance.ToString()),
                           () => _signalRService.SendActivityNotificationV2(ActivityNotification, walletMasterobj.UserID.ToString(), 2)
                           );
                        #endregion

                        return new CreateWalletAddressRes { address = responseString, ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.CreateAddressSuccessMsg };
                    }
                    return new CreateWalletAddressRes { ErrorCode = enErrorCode.AddressGenerationFailed, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
                }
                return new CreateWalletAddressRes { ErrorCode = enErrorCode.AddressGenerationFailed, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateERC20Address", "WalletService", ex);
                return null;
            }
        }

        //public CreateWalletAddressRes CreateNeoAddress(long UserId, string Coin, string AccWalletId, short IsLocal = 0)
        //{
        //    try
        //    {
        //        UserActivityLog activityLog = new UserActivityLog();
        //        string password = "";
        //        string sitename = null;
        //        string siteid = null;
        //        string Respaddress = null;

        //        var wallettype = _WalletTypeMasterRepository.GetSingle(t => t.WalletTypeName == Coin && t.IsLocal == IsLocal);
        //        if (wallettype == null)
        //        {
        //            return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidCoinName, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidCoin };
        //        }
        //        //2019-2-18 added condi for only used trading wallet
        //        var walletMasterobj = _commonRepository.GetSingle(item => item.AccWalletID == AccWalletId && item.Status == Convert.ToInt16(ServiceStatus.Active) && item.WalletTypeID == wallettype.Id && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

        //        if (walletMasterobj == null)
        //        {
        //            return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidWallet, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet };
        //        }

        //        var addressObj = _addressMstRepository.GetSingle(i => i.WalletId == walletMasterobj.Id && i.Status == 1);
        //        if (addressObj != null)
        //        {
        //            return new CreateWalletAddressRes { ErrorCode = enErrorCode.AddressExist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AddressExist };
        //        }

        //        var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = Coin.ToLower(), amount = 0, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.Generate_Address) });

        //        transactionProviderResponses = providerdata.GetAwaiter().GetResult();
        //        if (transactionProviderResponses == null || transactionProviderResponses.Count == 0)
        //        {
        //            return new CreateWalletAddressRes { ErrorCode = enErrorCode.ItemNotFoundForGenerateAddress, ReturnCode = enResponseCode.Fail, ReturnMsg = "Please try after sometime." };
        //        }
        //        if (transactionProviderResponses[0].ThirPartyAPIID == 0)
        //        {
        //            return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
        //        }
        //        var apiconfig = _thirdPartyCommonRepository.GetById(transactionProviderResponses[0].ThirPartyAPIID);

        //        thirdPartyAPIConfiguration = apiconfig;
        //        if (thirdPartyAPIConfiguration == null || transactionProviderResponses.Count == 0)
        //        {
        //            return new CreateWalletAddressRes { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
        //        }
        //        password = RandomGeneratePaasword(UserId);
        //        sitename = _configuration["sitename"].ToString();
        //        siteid = _configuration["site_id"].ToString();

        //        thirdPartyAPIRequest = _getWebRequest.MakeWebRequestERC20(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID, password, sitename, siteid);
        //        string apiResponse = _webApiSendRequest.SendJsonRpcAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody);
        //        WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses[0].ThirPartyAPIID, 0);

        //        Respaddress = ParsedResponse.Param1;
        //        if (string.IsNullOrEmpty(Respaddress))
        //        {
        //            return new CreateWalletAddressRes { ErrorCode = enErrorCode.AddressGenerationFailed, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
        //        }
        //        var Key = ParsedResponse.Param2;
        //        if (Respaddress != null)
        //        {
        //            string responseString = Respaddress;
        //            var Obj = _walletRepository1.AddAddressIntoDB(UserId, responseString, ParsedResponse.TrnRefNo, Key, transactionProviderResponses[0].SerProDetailID, IsLocal);
        //            if (Obj == true)
        //            {
        //                #region SMS_Email
        //                ActivityNotificationMessage ActivityNotification = new ActivityNotificationMessage();
        //                ActivityNotification.MsgCode = Convert.ToInt32(enErrorCode.GenerateAddressNotification);
        //                ActivityNotification.Param1 = walletMasterobj.Walletname;
        //                ActivityNotification.Type = Convert.ToInt16(EnNotificationType.Info);

        //                SMSSendAsyncV1(EnTemplateType.SMS_WalletAddressCreated, walletMasterobj.UserID.ToString(), walletMasterobj.Walletname);
        //                Parallel.Invoke(() => EmailSendAsyncV1(EnTemplateType.EMAIL_WalletAddressCreated, walletMasterobj.UserID.ToString(), wallettype.WalletTypeName, walletMasterobj.Walletname, Helpers.UTC_To_IST().ToString(), walletMasterobj.PublicAddress, walletMasterobj.Balance.ToString()),
        //                   () => _signalRService.SendActivityNotificationV2(ActivityNotification, walletMasterobj.UserID.ToString(), 2)
        //                   );
        //                #endregion

        //                return new CreateWalletAddressRes { address = responseString, ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.CreateAddressSuccessMsg };
        //            }
        //            return new CreateWalletAddressRes { ErrorCode = enErrorCode.AddressGenerationFailed, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
        //        }
        //        return new CreateWalletAddressRes { ErrorCode = enErrorCode.AddressGenerationFailed, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog("CreateERC20Address", "WalletService", ex);
        //        return null;
        //    }
        //}

        public string RandomGeneratePaasword(long userID)
        {
            try
            {
                long maxValue = 99999;
                long minValue = 10000;
                long x = (long)Math.Round(random.NextDouble() * (maxValue - minValue - 1)) + minValue;
                string userIDStr = x.ToString() + userID.ToString().PadLeft(5, '0');
                return userIDStr;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        #endregion

        #region Leader Board 

        public ListLeaderBoardRes LeaderBoard(int? UserCount)
        {
            try
            {
                ListLeaderBoardRes Resp = new ListLeaderBoardRes();
                var list = _profileConfigurationService.GetFrontLeaderList(0, 0, 2);
                list = _profileConfigurationService.GetFrontLeaderList(0, list.TotalCount, 2);
                if (list.LeaderList.Count() == 0) //khushali 31-07-2019 for leader list validation
                {
                    Resp.Data = new List<LeaderBoardRes>();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                    return Resp;
                }
                long[] LeaderId = list.LeaderList.Select(x => (long)x.LeaderId).ToArray();
                var data = _walletRepository1.LeaderBoard(UserCount, LeaderId);
                if (data.Count() == 0)
                {
                    Resp.Data = new List<LeaderBoardRes>();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                    return Resp;
                }
                Resp.Data = data;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.ErrorCode = enErrorCode.Success;

                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetYearwiseWalletStatistics", "WalletService", ex);
                return null;
            }
        }

        //2019-2-4
        public ListLeaderBoardRes LeaderBoardWeekWiseTopFive(long[] LeaderId, DateTime Date, short IsGainer, int Count)
        {
            try
            {
                ListLeaderBoardRes Resp = new ListLeaderBoardRes();
                var data = _walletRepository1.LeaderBoardWeekWiseTopFive(LeaderId, Date, IsGainer, Count);
                if (data.Count() == 0)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                    return Resp;
                }
                Resp.Data = data;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.ErrorCode = enErrorCode.Success;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LeaderBoardWeekWiseTopFive", "WalletService", ex);
                return null;
            }
        }
        #endregion

        public GetTransactionPolicyRes ListTransactionPolicy(long TrnType, long userId)
        {
            GetTransactionPolicyRes Resp = new GetTransactionPolicyRes();
            try
            {
                var obj = _walletRepository1.ListTransactionPolicy(TrnType, userId);
                Resp.Data = obj;
                if (obj != null)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        // khushali 23-03-2019 For Success and Debit Reocn Process
        public async Task<WalletDrCrResponse> GetDebitWalletWithCharge(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, string Token = "", enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal)
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
                enWalletTranxOrderType orderType = enWalletTranxOrderType.Credit;
                long userID = 0, TrnNo = 0;

                HelperForLog.WriteLogIntoFileAsync("GetDebitWalletWithCharge", "WalletService", "timestamp:" + timestamp + "," + "coinName:" + coinName + ",accWalletID=" + accWalletID + ",TrnRefNo=" + TrnRefNo.ToString() + ",userID=" + userID + ",amount=" + amount.ToString());

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
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTradeRefNo, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }
                if (amount <= 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, 0, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidAmt, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmount, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }
                dWalletobj = await dWalletobjTask;
                if (dWalletobj == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TimeStamp = timestamp }, "Debit");
                }
                userID = dWalletobj.UserID;
                //ntrivedi 15-02-2019 removed and moved to stored procedure
                var flagTask = CheckUserBalanceAsync(amount, dWalletobj.Id);
                if (dWalletobj.Status != 1 || dWalletobj.IsValid == false)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidWallet, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }

                HelperForLog.WriteLogIntoFileAsync("GetDebitWalletWithCharge", "CheckUserBalance pre Balance=" + dWalletobj.Balance.ToString() + ", TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);
                //CheckUserBalanceFlag = await flagTask;
                CheckUserBalanceFlag = await flagTask;

                HelperForLog.WriteLogIntoFileAsync("GetDebitWalletWithCharge", "CheckUserBalance Post TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);
                dWalletobj = _commonRepository.GetById(dWalletobj.Id); // ntrivedi fetching fresh balance for multiple request at a time 
                if (dWalletobj.Balance < amount)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InsufficantBal, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InsufficantBal, ErrorCode = enErrorCode.InsufficantBal, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }
                if (!CheckUserBalanceFlag)
                {
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.BalMismatch, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.SettedBalanceMismatch, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }

                HelperForLog.WriteLogIntoFileAsync("GetDebitWalletWithCharge", "Check ShadowLimit done TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);

                CheckTrnRefNoRes count1 = await countTask1;
                if (count1.TotalCount != 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, amount, TrnRefNo, Helpers.UTC_To_IST(), null, dWalletobj.Id, coinName, userID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.AlredyExist, trnType);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);

                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AlredyExist, ErrorCode = enErrorCode.AlredyExist, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = timestamp }, "DebitForHold");
                }

                HelperForLog.WriteLogIntoFileAsync("GetDebitWalletWithCharge", "CheckTrnRefNo TrnNo=" + TrnRefNo.ToString() + " timestamp:" + timestamp);

                BizResponseClass bizResponse = _walletSPRepositories.Callsp_ReconSuccessAndDebitWalletWithCharge(dWalletobj, timestamp, serviceType, amount, coinName, allowedChannels, walletTypeMaster.Id, TrnRefNo, dWalletobj.Id, dWalletobj.UserID, routeTrnType, trnType, ref TrnNo, enWalletDeductionType);

                if (bizResponse.ReturnCode == enResponseCode.Success)
                {
                    try
                    {
                        decimal charge = _walletRepository1.FindChargeValueHold(timestamp, TrnRefNo);
                        long walletId = _walletRepository1.FindChargeValueWalletId(timestamp, TrnRefNo);
                        WalletMaster ChargeWalletObj = null;
                        WalletTypeMaster ChargewalletType = null;
                        //charge = 0;
                        if (charge > 0 && walletId > 0)
                        {
                            ChargeWalletObj = _commonRepository.GetById(walletId);
                            ChargewalletType = _WalletTypeMasterRepository.GetSingle(i => i.Id == ChargeWalletObj.WalletTypeID);
                        }
                        Task.Run(() => WalletHoldNotificationSend(timestamp, dWalletobj, coinName, amount, TrnRefNo, (byte)routeTrnType, charge, walletId, ChargeWalletObj, ChargewalletType));
                    }
                    catch (Exception ex)
                    {
                        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + "Timestamp:" + timestamp, this.GetType().Name, ex);
                    }
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDebit, ErrorCode = enErrorCode.Success, TrnNo = TrnNo, Status = enTransactionStatus.Hold, StatusMsg = bizResponse.ReturnMsg, TimeStamp = timestamp }, "DebitForHold");

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

        //khushali 11-04-2019 Process for Release Stuck Order - wallet side   
        public enTransactionStatus CheckTransactionSuccessOrNot(long TrnRefNo)
        {
            try
            {
                return _walletRepository1.CheckTransactionSuccessOrNot(TrnRefNo);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }
        //komal 03-07-2019 get Settled Qty
        public Decimal GetTransactionSettledQty(long TrnRefNo)
        {
            try
            {
                return _walletRepository1.GetTransactionSettledQty(TrnRefNo);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }
        //Rita for check settlement process status
        public bool CheckSettlementProceed(long MakerTrnNo, long TakerTrnNo)
        {
            try
            {
                return _walletRepository1.CheckSettlementProceed(MakerTrnNo, TakerTrnNo);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        public ListUserUnstakingReq2 GetUnstackingCroneData()
        {
            try
            {
                ListUserUnstakingReq2 Resp = new ListUserUnstakingReq2();
                var data = _walletRepository1.GetStakingdataForChrone();
                Resp.Data = data;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public int CheckActivityLog(long UserId, int Type)
        {
            try
            {
                var data = _walletRepository1.CheckActivityLog(UserId, Type);
                if (data == null)
                {
                    return 0;//true                
                }
                var date = Helpers.UTC_To_IST();

                TimeSpan difference = date - data.Date;
                double diffHour = difference.TotalHours;
                var activityhourObj = _ActivityTypeHour.GetSingle(i => i.ActivityType == Type);
                if (activityhourObj == null)
                {
                    return 24;//2019-6-29  add 24 hour insted of 0 hr
                }
                if (data != null)
                {
                    if (activityhourObj.ActivityHour >= diffHour)//Convert.ToInt32(_configuration["WithdrawHour"])
                    {
                        return activityhourObj.ActivityHour;//false
                    }
                }
                return 0;//true                
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public BizResponseClass AddTradingChartData(TradingChartDataReq req, long UserId)
        {
            try
            {
                var IsExist = _TradingChartData.GetSingle(i => i.PairId == req.PairId && i.UserId == UserId && i.Status == 1);
                if (IsExist != null)
                {
                    //update
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    IsExist.Data = req.RequestData;
                    _TradingChartData.UpdateWithAuditLog(IsExist);

                    return new BizResponseClass
                    {
                        ErrorCode = enErrorCode.Success,
                        ReturnMsg = EnResponseMessage.RecordUpdated,
                        ReturnCode = enResponseCode.Success
                    };
                }
                else
                {
                    //insert
                    TradingChartData NewObj = new TradingChartData();
                    NewObj.CreatedBy = UserId;
                    NewObj.CreatedDate = Helpers.UTC_To_IST();
                    NewObj.Status = 1;
                    NewObj.PairId = req.PairId;
                    NewObj.UserId = UserId;
                    NewObj.Data = req.RequestData;
                    _TradingChartData.Add(NewObj);

                    return new BizResponseClass
                    {
                        ErrorCode = enErrorCode.Success,
                        ReturnMsg = EnResponseMessage.RecordAdded,
                        ReturnCode = enResponseCode.Success
                    };
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public TradingChartDataRes GetTradingChartData(long PairId, long UserId)
        {
            try
            {
                var responseData = _TradingChartData.GetSingle(i => i.PairId == PairId && i.UserId == UserId && i.Status == 1);
                if (responseData != null)
                {
                    return new TradingChartDataRes
                    {
                        RequestData = responseData.Data,
                        PairId = responseData.PairId,
                        ErrorCode = enErrorCode.Success,
                        ReturnMsg = EnResponseMessage.FindRecored,
                        ReturnCode = enResponseCode.Success
                    };
                }
                return new TradingChartDataRes
                {
                    RequestData = "",
                    PairId = 0,
                    ErrorCode = enErrorCode.NotFound,
                    ReturnMsg = EnResponseMessage.NotFound,
                    ReturnCode = enResponseCode.Fail
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //2019-7-25 vsolanki added new mwthod to validate user
        public ValidateUserForInternalTransferRes ValidateUserForInternalTransfer(string Email, string CoinName, ApplicationUser user)
        {
            ValidateUserForInternalTransferRes Response = new ValidateUserForInternalTransferRes();
            try
            {
                if (user.Email == Email)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.SameEMail;
                    Response.ErrorCode = enErrorCode.SameEMail;
                    return Response;
                }
                var emailObj = _userManager.FindByEmailAsync(Email).GetAwaiter().GetResult();
                if (emailObj != null)
                {
                    if (emailObj.IsEnabled != true)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.UserNotActive;
                        Response.ErrorCode = enErrorCode.Status4168UserNotActive;
                        return Response;
                    }
                    if (emailObj.EmailConfirmed != true)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.EmailVerifyPending;
                        Response.ErrorCode = enErrorCode.EmailVerifyPending;
                        return Response;
                    }
                }
                else
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.EmailNotExist;
                    Response.ErrorCode = enErrorCode.EmailNotExist;
                    return Response;
                }
                var CoinObj = _WalletTypeMasterRepository.GetSingle(i => i.WalletTypeName == CoinName && i.Status == 1);
                if (CoinObj == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.InvalidCoinName;
                    Response.ErrorCode = enErrorCode.InvalidCoinName;
                    return Response;
                }
                var walletObj = _commonRepository.GetSingle(i => i.WalletTypeID == CoinObj.Id && i.Status == 1 && i.UserID == emailObj.Id && i.WalletUsageType == 0);
                if (walletObj == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.InvalidWallet;
                    Response.ErrorCode = enErrorCode.InvalidWallet;
                    return Response;
                }
                var addressObj = _addressMstRepository.GetSingle(i => i.WalletId == walletObj.Id && i.Status == 1);
                if (addressObj == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.InvalidWallet;
                    Response.ErrorCode = enErrorCode.InvalidWallet;
                    return Response;
                }
                Response.Address = addressObj.Address;
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = EnResponseMessage.FindRecored;
                Response.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //komal 30-09-2019
        public async Task<TransactionWalletResponse> GetTransactionWalletByCoin(long userid, string coin)
        {
            TransactionWalletResponse _Res = new TransactionWalletResponse();
            WalletMasterResponsev2 Wallet = new WalletMasterResponsev2();
            try
            {
                var walletResponse = _walletRepository1.GetTransactionWalletMasterResponseByCoin(userid, coin).Where(e => e.IsDefaultWallet == 1).FirstOrDefault();
                //var UserPrefobj = _UserPreferencescommonRepository.FindBy(item => item.UserID == userid && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (walletResponse == null)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = EnResponseMessage.NotFound;
                    _Res.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    _Res.Wallet = walletResponse;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = EnResponseMessage.FindRecored;
                    _Res.ErrorCode = enErrorCode.Success;
                }
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                _Res.ReturnCode = enResponseCode.InternalError;
                return _Res;
            }
        }
    }
}