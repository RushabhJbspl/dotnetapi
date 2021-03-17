using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.FeedConfiguration;
using Worldex.Core.ViewModels.APIConfiguration;
using Microsoft.Extensions.Logging;
using OpenIddict.Core;
using OpenIddict.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TwoFactorAuthNet;

namespace Worldex.Infrastructure.Services.Configuration
{
    public class APIConfigurationService : IAPIConfigurationService
    {
        private readonly ILogger<APIConfigurationService> _logger;
        private readonly IAPIConfigurationRepository _APIconfiRepository;
        private readonly IBackOfficeAPIConfigService _backOfficeAPIConfigService;
        private ICommonRepository<APIPlanMaster> _APIPlanMasterRepository;
        private ICommonRepository<APIPlanDetail> _APIPlanDetailRepository;
        private ICommonRepository<UserSubscribeAPIPlan> _SubScribePlanRepository;
        private ICommonRepository<UserAPIKeyDetails> _UserAPIKeyDetailsRepository;
        private ICommonRepository<PublicAPIKeyPolicy> _PublicAPIKeyPolicyRepository;
        private ICommonRepository<WhiteListIPEndPoint> _WhiteListIPEndPointRepository;
        private ICommonRepository<APIKeyWhitelistIPConfig> _APIKeyWhitelistIPConfigRepository;
        private ICommonRepository<APIPlanMethodConfiguration> _APIPlanMethodCongifRepo;
        private ICommonRepository<APIReqResStatistics> _APIReqResStatisticsRepository;
        private ICommonRepository<PublicAPIReqResLog> _PublicAPIReqResLogRepository;
        private readonly OpenIddictApplicationManager<OpenIddictApplication> _openIddictApplicationManager;


        public APIConfigurationService(ILogger<APIConfigurationService> logger, IAPIConfigurationRepository APIconfiRepository,
            ICommonRepository<APIPlanMaster> APIPlanMasterRepository, ICommonRepository<UserSubscribeAPIPlan> SubScribePlanRepository,
            ICommonRepository<UserAPIKeyDetails> UserAPIKeyDetailsRepository, ICommonRepository<PublicAPIKeyPolicy> PublicAPIKeyPolicyRepository,
            ICommonRepository<WhiteListIPEndPoint> WhiteListIPEndPointRepository, ICommonRepository<APIKeyWhitelistIPConfig> APIKeyWhitelistIPConfigRepository,
            ICommonRepository<APIPlanDetail> APIPlanDetailRepository, ICommonRepository<APIPlanMethodConfiguration> APIPlanMethodCongifRepo,
            ICommonRepository<APIReqResStatistics> APIReqResStatisticsRepository, ICommonRepository<PublicAPIReqResLog> PublicAPIReqResLogRepository,
            OpenIddictApplicationManager<OpenIddictApplication> openIddictApplicationManager, IBackOfficeAPIConfigService backOfficeAPIConfigService)
        {
            _logger = logger;
            _APIconfiRepository = APIconfiRepository;
            _APIPlanMasterRepository = APIPlanMasterRepository;
            _SubScribePlanRepository = SubScribePlanRepository;
            _UserAPIKeyDetailsRepository = UserAPIKeyDetailsRepository;
            _PublicAPIKeyPolicyRepository = PublicAPIKeyPolicyRepository;
            _WhiteListIPEndPointRepository = WhiteListIPEndPointRepository;
            _APIKeyWhitelistIPConfigRepository = APIKeyWhitelistIPConfigRepository;
            _APIPlanDetailRepository = APIPlanDetailRepository;
            _APIPlanMethodCongifRepo = APIPlanMethodCongifRepo;
            _APIReqResStatisticsRepository = APIReqResStatisticsRepository;
            _PublicAPIReqResLogRepository = PublicAPIReqResLogRepository;
            _openIddictApplicationManager = openIddictApplicationManager;
            _backOfficeAPIConfigService = backOfficeAPIConfigService;
        }

        #region APIPlan
        public ViewAPIPlanDetailResponse ViewAPIPlanDetail(long UserID)
        {
            ViewAPIPlanDetailResponse _Res = new ViewAPIPlanDetailResponse();
            List<ViewAPIPlanDetailResponseInfo> planList = new List<ViewAPIPlanDetailResponseInfo>();
            try
            {
                //planList = _APIconfiRepository.ViewAPIPlanDetail();
                var list = _APIPlanMasterRepository.FindBy(e => e.Status == 1).OrderBy(e=>e.Priority).ToList();
                if (list.Count == 0)
                {
                    _Res.Response = planList;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
               
                var list2 = _APIconfiRepository.GetPlanMethods();
                var PlanHistory = _SubScribePlanRepository.FindBy(e => e.UserID == UserID && e.Status == 1).FirstOrDefault();

                foreach (var obj in list)
                {
                    short SubScribePlan = 0;
                    long Subscribeid = 0;
                    DateTime ExpireDate= new DateTime();
                    var ReadonlyList = list2.Where(e => e.APIPlanMasterID == obj.Id && e.IsReadOnly == 1).Select(e=>e.MethodName).ToList();
                    var FullAccessList = list2.Where(e => e.APIPlanMasterID == obj.Id && e.IsFullAccess == 1).Select(e => e.MethodName).ToList();
                    if (PlanHistory != null)
                    {
                        if (obj.Id == PlanHistory.APIPlanMasterID)
                        {
                            SubScribePlan = 1;
                            ExpireDate = Convert.ToDateTime(PlanHistory.ExpiryDate);
                            Subscribeid = PlanHistory.Id;
                        }
                    }
                    planList.Add(new ViewAPIPlanDetailResponseInfo()
                    {
                        Charge = obj.Charge,
                        SubscribeID= Subscribeid,
                        ID = obj.Id,
                        PlanDesc = obj.PlanDesc,
                        PlanName = obj.PlanName,
                        Priority = obj.Priority,
                        PlanValidity = obj.PlanValidity,
                        ConcurrentEndPoints = obj.ConcurrentEndPoints,
                        WhitelistEndPoints=obj.WhitelistedEndPoints,
                        IsPlanRecursive = obj.IsPlanRecursive,
                        Price = obj.Price,
                        Status = obj.Status,
                        HistoricalDataMonth = obj.HistoricalDataMonth,
                        MaxOrderPerSec = obj.MaxOrderPerSec,
                        MaxPerDay = obj.MaxPerDay,
                        MaxPerMinute = obj.MaxPerMinute,
                        MaxPerMonth = obj.MaxPerMonth,
                        MaxRecPerRequest = obj.MaxRecPerRequest,
                        MaxReqSize = obj.MaxReqSize,
                        MaxResSize = obj.MaxResSize,
                        PlanValidityType = obj.PlanValidityType,
                        FullAccessAPI = FullAccessList,
                        ReadOnlyAPI = ReadonlyList,
                        IsSubscribePlan = SubScribePlan,
                        ExpireDate = ExpireDate,
                        Coin=obj.Currency,
                        ServiceID=obj.ServiceID
                    });
                }
               
                _Res.Response = planList;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public ViewActivePlanDetailResponse ViewUserActivePlan(long UserID)
        {
            ViewActivePlanDetailResponse _Res = new ViewActivePlanDetailResponse();
            ViewActivePlanDetailInfo APIDetail = new ViewActivePlanDetailInfo();
            //ViewActivePlanDetailInfoV1 APIDetailV1 = new ViewActivePlanDetailInfoV1();
            try
            {

                APIDetail = _APIconfiRepository.ViewUserActivePlan(UserID);
                if (APIDetail == null)
                {
                    _Res.Response = null;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                var list2 = _APIconfiRepository.GetPlanMethods(0);
                var ReadonlyList = list2.Where(e => e.APIPlanMasterID == APIDetail.PlanID && e.IsReadOnly == 1).ToDictionary(e=>e.RestMethodID,e=>e.MethodName);
                var FullAccessList = list2.Where(e => e.APIPlanMasterID == APIDetail.PlanID && e.IsFullAccess == 1).ToDictionary(e => e.RestMethodID, e => e.MethodName);

                var ExpiryDate = this.GetCurrentPlanExpiryDate(APIDetail.SubscribeID, UserID);

                if (APIDetail.IsAutoRenew == 1)
                    APIDetail.RenewDate = ExpiryDate.AddDays(-APIDetail.RenewDays);
                else
                    APIDetail.RenewDate = null;

                _Res.Response = new ViewActivePlanDetailInfoV1() {
                    ActivationDate= APIDetail.ActivationDate,
                    Charge=APIDetail.Charge,
                    ConcurrentEndPoints= APIDetail.ConcurrentEndPoints,
                    ExpiryDate= ExpiryDate,
                    PlanID = APIDetail.PlanID,
                    Priority=APIDetail.Priority,
                    FullAccessAPI= FullAccessList,
                    HistoricalDataMonth= APIDetail.HistoricalDataMonth,
                    IsAutoRenew= APIDetail.IsAutoRenew,
                    IsPlanRecursive= APIDetail.IsPlanRecursive,
                    MaxOrderPerSec= APIDetail.MaxOrderPerSec,
                    MaxPerDay= APIDetail.MaxPerDay,
                    MaxPerMinute= APIDetail.MaxPerMinute,
                    MaxPerMonth= APIDetail.MaxPerMonth,
                    MaxRecPerRequest= APIDetail.MaxRecPerRequest,
                    MaxReqSize= APIDetail.MaxReqSize,
                    MaxResSize= APIDetail.MaxResSize,
                    PaidAmt= APIDetail.PaidAmt,
                    PaymentStatus= APIDetail.PaymentStatus,
                    PlanName= APIDetail.PlanName,
                    PlanPrice= APIDetail.PlanPrice,
                    PlanStatus= APIDetail.PlanStatus,
                    PlanValidity= APIDetail.PlanValidity,
                    PlanValidityType= APIDetail.PlanValidityType,
                    Price= APIDetail.Price,
                    ReadOnlyAPI=ReadonlyList,
                    RenewDate= APIDetail.RenewDate,
                    RenewStatus= APIDetail.RenewStatus,
                    RequestedDate= APIDetail.RequestedDate,
                    SubscribeID= APIDetail.SubscribeID,
                    SubScribeStatus= APIDetail.SubScribeStatus,
                    TotalAmt= APIDetail.TotalAmt,
                    WhitelistedEndPoints= APIDetail.WhitelistedEndPoints
                };
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public DateTime GetCurrentPlanExpiryDate(long CurplanID,long UserID)
        {
            DateTime ExpiryDate;
            try
            {
                var CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Id == CurplanID && e.UserID == UserID).SingleOrDefault();
                //if (CurrentPlanHistory == null)
                //    return null;
                var renewStatus = CurrentPlanHistory.RenewStatus;
                ExpiryDate =Convert.ToDateTime(CurrentPlanHistory.ExpiryDate);
                while (renewStatus == 1)
                {
                    CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Id == CurrentPlanHistory.NextAutoRenewId && e.Status == 0).SingleOrDefault();
                    if (CurrentPlanHistory == null)
                        break;

                    renewStatus = CurrentPlanHistory.RenewStatus;
                    ExpiryDate = Convert.ToDateTime(CurrentPlanHistory.ExpiryDate);
                }

                return ExpiryDate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public GetAutoRenewDetailResponse GetAutoRenewDetail(long UserID)
        {
            GetAutoRenewDetailResponse _Res = new GetAutoRenewDetailResponse();
            try
            {
                var APIDetail = _APIconfiRepository.GetAutoRenewDetail(UserID);
                if (APIDetail == null)
                {
                    _Res.Response = APIDetail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }

                var ExpiryDate = this.GetCurrentPlanExpiryDate(APIDetail.SubscribeID, UserID);
                APIDetail.ExpiryDate = ExpiryDate;
                APIDetail.NextRenewDate = ExpiryDate.AddDays(-APIDetail.Days);
                _Res.Response = APIDetail;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public BizResponseClass StopAutoRenew(StopAutoRenewRequest Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Id == Request.SubscribeID && e.Status == 1 ).SingleOrDefault();
                if (CurrentPlanHistory == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidSubscribeID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "InValid Subscribe ID";
                    return _Res;
                }
                //var NextAutoRenewObj = _SubScribePlanRepository.FindBy(e => e.Id == CurrentPlanHistory.NextAutoRenewId && e.Status == 0).SingleOrDefault();
                //if (NextAutoRenewObj == null)
                //{
                //    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InvalidNextAutoRenewID;
                //    _Res.ReturnCode = enResponseCode.Fail;
                //    _Res.ReturnMsg = "InValid Subscribe ID";
                //    return _Res;
                //}

                //NextAutoRenewObj.Status = (short)enTransactionStatus.InActive;
                //CurrentPlanHistory.IsAutoRenew = 0;
                //CurrentPlanHistory.RenewDate = null;
                //CurrentPlanHistory.RenewStatus = 0;
                //_SubScribePlanRepository.Update(NextAutoRenewObj);
                //_SubScribePlanRepository.Update(CurrentPlanHistory);


                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public UserAPIPlanHistoryResponse GetUserPlanHistory(UserAPIPlanHistoryRequest request,long UserID)
        {
            UserAPIPlanHistoryResponse _Res = new UserAPIPlanHistoryResponse();
            List<UserAPIPlanHistoryResponseInfo> HistoryList = new List<UserAPIPlanHistoryResponseInfo>();
            try
            {
                var list = _APIconfiRepository.GetUserAPISubscribeHistory(UserID, request.PlanID, request.PaymentStatus);
                if (list.Count == 0)
                {
                    _Res.TotalCount = 0;
                    _Res.Response = HistoryList;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                _Res.TotalCount = list.Count();
                foreach (var obj in list.Skip(Convert.ToInt32(request.Pagesize)* Convert.ToInt32(request.PageNo)).Take(Convert.ToInt32(request.Pagesize)))
                {
                    HistoryList.Add(new UserAPIPlanHistoryResponseInfo() {
                        ActivationDate=obj.ActivationDate,
                        Charge=obj.Charge,
                        ExpiryDate=obj.ExpiryDate,
                        PaymentStatus=obj.PaymentStatus,
                        Perticuler=obj.Perticuler,
                        PlanName=obj.PlanName,
                        Price=obj.Price,
                        Status=obj.Status,
                        TotalAmt=obj.TotalAmt
                    });
                }
                _Res.Response = HistoryList;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        #endregion

        #region APIKey
        public async Task<GenerateAPIKeyResponse> GenerateAPIKey(GenerateAPIKeyRequest Request, ApplicationUser user)
        {
            TwoFactorAuth TFAuth = new TwoFactorAuth();
            GenerateAPIKeyResponse _Res = new GenerateAPIKeyResponse();
            GenerateAPIKeyResponseInfo Response = new GenerateAPIKeyResponseInfo();
            long FrequencyCount = 0;
            try
            {
                if(Request.APIAccess!=1 && Request.APIAccess != 0)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidIPAccess;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Enter Valid IPAccess";
                    return _Res;
                }
                var PlanDetail = _APIPlanMasterRepository.FindBy(e => e.Id == Request.PlanID && e.Status == 1).SingleOrDefault();
                if (PlanDetail == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidRequestedPlanOrDisable;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = " In Valid Requested Plan";
                    return _Res;
                }
                var CurrentPlanHistory = _SubScribePlanRepository.FindBy(e =>e.Status==1 && e.UserID== user.Id && e.APIPlanMasterID==Request.PlanID).SingleOrDefault();
                if (CurrentPlanHistory == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_ActivePlanNotFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Active Plan Not Found";
                    return _Res;
                }
                var KeyPolicy = _PublicAPIKeyPolicyRepository.FindBy(e => e.Status == 1).FirstOrDefault();
                if (KeyPolicy != null)
                {
                    var list = _UserAPIKeyDetailsRepository.FindBy(e => e.UserID == user.Id && e.APIPlanMasterID == PlanDetail.Id).ToList();
                    var TotalCount = list.Where(e => e.Status == 1).Count();
                    if (TotalCount >= KeyPolicy.AddMaxLimit)
                    {
                        _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_AddLimitExceed;
                        _Res.ReturnCode = enResponseCode.Fail;
                        _Res.ReturnMsg = "Add Limit Exceed.";
                        return _Res;
                    }
                    var DayCount = list.Where(e => e.Status == 1 && e.CreatedDate > DateTime.UtcNow.AddDays(-1)).Count();
                    if (DayCount >= KeyPolicy.AddPerDayFrequency)
                    {
                        _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_PerDayAddLimitExceed;
                        _Res.ReturnCode = enResponseCode.Fail;
                        _Res.ReturnMsg = "Per Day Add Limit Exceed.";
                        return _Res;
                    }
                    if (KeyPolicy.AddFrequencyType == (short)enFrequencyType.Month)
                    {
                        FrequencyCount = list.Where(e => e.Status == 1 && e.CreatedDate > DateTime.UtcNow.AddMonths(-1)).Count();
                        if (FrequencyCount >= KeyPolicy.AddFrequency)
                        {
                            _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_AddFrequencyLimitExceed;
                            _Res.ReturnCode = enResponseCode.Fail;
                            _Res.ReturnMsg = "Add Frequency Limit Exceed.";
                            return _Res;
                        }
                    }
                    if (KeyPolicy.AddFrequencyType == (short)enFrequencyType.week)
                    {
                        FrequencyCount = list.Where(e => e.Status == 1 && e.CreatedDate > DateTime.UtcNow.AddDays(-7)).Count();
                        if (FrequencyCount >= KeyPolicy.AddFrequency)
                        {
                            _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_AddFrequencyLimitExceed;
                            _Res.ReturnCode = enResponseCode.Fail;
                            _Res.ReturnMsg = "Add Frequency Limit Exceed.";
                            return _Res;
                        }
                    }
                }
                var ExistAlias = _UserAPIKeyDetailsRepository.FindBy(e => e.AliasName == Request.AliasName && e.UserID== user.Id && e.APIPlanMasterID== PlanDetail.Id).SingleOrDefault();
                if (ExistAlias != null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_DuplicateAliasName;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Duplicate Alias Name";
                    return _Res;
                }

                var apiKey = this.GetAPIKey("Cooldex", Request.AliasName);
                var Secretkey = this.GetSecretKey("CooldexCoolDex", apiKey);
                var sKey = TFAuth.CreateSecret(160);
                var AuthenticatorUri = TFAuth.GetQrCodeImageAsDataUri(Secretkey, sKey);

                var NewModel = _UserAPIKeyDetailsRepository.Add(new UserAPIKeyDetails()
                {
                    AliasName=Request.AliasName,
                    APIKey= apiKey,
                    APIPermission=Request.APIAccess,
                    APIPlanMasterID= PlanDetail.Id,
                    QRCode= AuthenticatorUri,
                    CreatedBy =user.Id,
                    CreatedDate=DateTime.UtcNow,
                    IPAccess=0,
                    SecretKey= Secretkey,
                    Status=1,
                    UserID=user.Id
                });
                if (NewModel.Id > 0)
                {
                    if (await _openIddictApplicationManager.FindByClientIdAsync(apiKey) == null)
                    {
                        var descriptor = new OpenIddictApplicationDescriptor
                        {
                            ClientId = apiKey,
                            ClientSecret = Secretkey,
                            DisplayName = Request.AliasName,
                            Permissions =
                            {
                            OpenIddictConstants.Permissions.Endpoints.Token,
                            OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
                            }
                        };
                        await _openIddictApplicationManager.CreateAsync(descriptor);
                    }
                    Response.AliasName = NewModel.AliasName;
                    Response.APIKey = NewModel.APIKey;
                    Response.SecretKey = NewModel.SecretKey;
                    Response.QRCode = AuthenticatorUri;
                    _Res.Response = Response;
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "Data Insert Successfully.";
                }
                else
                {
                    _Res.ErrorCode = enErrorCode.DataInsertFail;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Insert Fail.";
                }
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public string GetSecretKey(string Sign, string APIKey)
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] keyBytes = encoding.GetBytes(APIKey);
                Byte[] textBytes = encoding.GetBytes(Sign);
                Byte[] hashBytes;
                using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                    hashBytes = hash.ComputeHash(textBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public string GetAPIKey(string Sign, string AliasName)
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] keyBytes = encoding.GetBytes("Cooldex" + AliasName + Sign);

                using (var generator = RandomNumberGenerator.Create())
                    generator.GetBytes(keyBytes);

                string apiKey = Convert.ToBase64String(keyBytes);
                apiKey = apiKey.Replace("=", "");
                apiKey = apiKey.Replace("+", "");
                apiKey = apiKey.Replace("/", "");
                return apiKey;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public BizResponseClass UpdateAPIKey(long KeyID, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var Model = _UserAPIKeyDetailsRepository.FindBy(e => e.Id == KeyID && e.Status == 1).SingleOrDefault();
                if (Model == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidAPIKey;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "In Valid APIKey";
                    return _Res;
                }
                if(Model.IPAccess==(short)enAPIIPAccess.Restrict)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_SettingFreezed;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Setting Freezed";
                    return _Res;
                }

                Model.IPAccess = 1;
                Model.UpdatedBy = UserID;
                Model.UpdatedDate = DateTime.UtcNow;

                _UserAPIKeyDetailsRepository.Update(Model);


                var iplist = _WhiteListIPEndPointRepository.FindBy(e => e.UserID == UserID && e.Status == 1 && e.IPType == (short)enIPType.Whitelist && e.APIPlanID==Model.APIPlanMasterID).ToList();
                foreach(var obj in iplist)
                {
                    var NewModel = _APIKeyWhitelistIPConfigRepository.Add(new APIKeyWhitelistIPConfig()
                    {
                        APIKeyID=KeyID,
                        IPId=obj.Id,
                        CreatedBy = UserID,
                        CreatedDate = DateTime.UtcNow,
                        Status = 1
                    });
                }

                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public BizResponseClass DeleteAPIKey(long KeyID, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            long FrequencyCount = 0;
            try
            {
                var Model = _UserAPIKeyDetailsRepository.FindBy(e => e.Id == KeyID && e.Status == 1).SingleOrDefault();
                if (Model == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidAPIKey;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "In Valid APIKey";
                    return _Res;
                }

                var KeyPolicy = _PublicAPIKeyPolicyRepository.FindBy(e => e.Status == 1).FirstOrDefault();
                if (KeyPolicy != null)
                {
                    var list = _UserAPIKeyDetailsRepository.FindBy(e => e.UserID == UserID && e.APIPlanMasterID == Model.APIPlanMasterID).ToList();
                    var TotalCount = list.Where(e => e.Status == 9).Count();
                    if (TotalCount >= KeyPolicy.AddMaxLimit)
                    {
                        _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_AddLimitExceed;
                        _Res.ReturnCode = enResponseCode.Fail;
                        _Res.ReturnMsg = "Add Limit Exceed.";
                        return _Res;
                    }
                    var DayCount = list.Where(e => e.Status == 9 && e.UpdatedDate > DateTime.UtcNow.AddDays(-1)).Count();
                    if (DayCount >= KeyPolicy.AddPerDayFrequency)
                    {
                        _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_PerDayDeleteLimitExceed;
                        _Res.ReturnCode = enResponseCode.Fail;
                        _Res.ReturnMsg = "Per Day Delete Limit Exceed.";
                        return _Res;
                    }
                    if (KeyPolicy.AddFrequencyType == (short)enFrequencyType.Month)
                    {
                        FrequencyCount = list.Where(e => e.Status == 9 && e.UpdatedDate > DateTime.UtcNow.AddMonths(-1)).Count();
                        if (FrequencyCount >= KeyPolicy.AddFrequency)
                        {
                            _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_DeleteFrequencyLimitExceed;
                            _Res.ReturnCode = enResponseCode.Fail;
                            _Res.ReturnMsg = "Delete Frequency Limit Exceed.";
                            return _Res;
                        }
                    }
                    if (KeyPolicy.AddFrequencyType == (short)enFrequencyType.week)
                    {
                        FrequencyCount = list.Where(e => e.Status == 9 && e.UpdatedDate > DateTime.UtcNow.AddDays(-7)).Count();
                        if (FrequencyCount >= KeyPolicy.AddFrequency)
                        {
                            _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_DeleteFrequencyLimitExceed;
                            _Res.ReturnCode = enResponseCode.Fail;
                            _Res.ReturnMsg = "Delete Frequency Limit Exceed.";
                            return _Res;
                        }
                    }
                }
                
                Model.Status = 9;
                Model.UpdatedBy = UserID;
                Model.UpdatedDate = DateTime.UtcNow;
                _UserAPIKeyDetailsRepository.Update(Model);
                _APIconfiRepository.DisableWhiteListIPConfigurationKeywise(KeyID, UserID);
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public APIKeyListPResponse GetAPIKeyList(long APIId, long UserID)
        {
            APIKeyListPResponse _Res = new APIKeyListPResponse();
            List<APIKeyListPResponseInfo> KeyList = new List<APIKeyListPResponseInfo>();
            List<WhitelistIPList> iPLists;
            try
            {
                var list = _UserAPIKeyDetailsRepository.FindBy(e => e.UserID == UserID && e.Status == 1 && e.APIPlanMasterID == APIId).ToList();
                var iplist2 = _APIconfiRepository.GetKeyWiseIPList(UserID, APIId);
                var KeyPolicy = _PublicAPIKeyPolicyRepository.FindBy(e => e.Status == 1).FirstOrDefault();
                _Res.APIKeyLimit = KeyPolicy.AddMaxLimit;
                _Res.APIKeyCount = list.Count;
                if(list.Count>0)
                {
                    foreach(var obj in list)
                    {
                        var iplist3 = iplist2.Where(e => e.APIKeyID == obj.Id).ToList();
                        iPLists = new List<WhitelistIPList>();
                        if (iplist3.Count > 0)
                        {
                            foreach (var ipDetail in iplist3)
                            {
                                iPLists.Add(new WhitelistIPList()
                                {
                                    AliasName = ipDetail.AliasName,
                                    CreatedDate = ipDetail.CreatedDate,
                                    IPAddress = ipDetail.IPAddress,
                                    IPId = ipDetail.ID,
                                    IPType=ipDetail.IPType
                                });
                            }
                        }
                        string result = obj.APIKey.Substring(obj.APIKey.Length-2).PadLeft(obj.APIKey.Length-2, 'X');
                        string result2 = obj.SecretKey.Substring(obj.SecretKey.Length-2).PadLeft(obj.SecretKey.Length-2, 'X');
                        KeyList.Add(new APIKeyListPResponseInfo()
                        {
                            AliasName=obj.AliasName,
                            APIAccess=obj.APIPermission,
                            APIKey= result,
                            CreatedDate=obj.CreatedDate,
                            KeyId=obj.Id,
                            SecretKey= result2,
                            IPAccess=obj.IPAccess,
                            QRCode=obj.QRCode,
                            IPList= iPLists
                        });
                    }
                }
                _Res.Response = KeyList;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public APIKeyListPResponseV2 GetAPIKeyByID(long KeyID, long UserID)
        {
            APIKeyListPResponseV2 _Res = new APIKeyListPResponseV2();
            APIKeyListPResponseInfo keyDetail = new APIKeyListPResponseInfo();
            List<WhitelistIPList> iPLists;
            try
            {
                var APIKeyDetail = _UserAPIKeyDetailsRepository.FindBy(e => e.UserID == UserID && e.Status == 1 && e.Id==KeyID).FirstOrDefault();
                if(APIKeyDetail == null)
                {
                    _Res.Response = keyDetail;
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidAPIKey;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "In Valid APIKey";
                    return _Res;
                }
                var iplist2 = _APIconfiRepository.GetKeyWiseIPList(UserID, APIKeyDetail.APIPlanMasterID);
                keyDetail.AliasName = APIKeyDetail.AliasName;
                keyDetail.APIAccess = APIKeyDetail.APIPermission;
                keyDetail.APIKey = APIKeyDetail.APIKey;
                keyDetail.CreatedDate = APIKeyDetail.CreatedDate;
                keyDetail.IPAccess = APIKeyDetail.IPAccess;
                keyDetail.KeyId = APIKeyDetail.Id;
                keyDetail.QRCode = APIKeyDetail.QRCode;
                keyDetail.SecretKey = APIKeyDetail.SecretKey;

                var iplist3 = iplist2.Where(e => e.APIKeyID == KeyID).ToList();
                iPLists = new List<WhitelistIPList>();
                if (iplist3.Count > 0)
                {
                    foreach (var ipDetail in iplist3)
                    {
                        iPLists.Add(new WhitelistIPList()
                        {
                            AliasName = ipDetail.AliasName,
                            CreatedDate = ipDetail.CreatedDate,
                            IPAddress = ipDetail.IPAddress,
                            IPId = ipDetail.ID,
                            IPType=ipDetail.IPType
                        });
                    }
                }
                keyDetail.IPList = iPLists;
                _Res.Response = keyDetail;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        #endregion

        #region WhitelistIP

        public BizResponseClass WhitelistIP(IPWhiteListRequest Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            long WhiteListIPLimit = 0, ConCurrentIPLimit=0;
            try
            {
                if(Request.IPList.Count==0)
                {
                    _Res.ErrorCode = enErrorCode.IPListRequirer;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "IP List Requirer";
                    return _Res;
                }
                var CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Status == 1 && e.UserID == UserID && e.APIPlanMasterID == Request.PlanID).SingleOrDefault();
                if (CurrentPlanHistory == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_ActivePlanNotFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Active Plan Not Found";
                    return _Res;
                }
                var PlanDetail = _APIPlanMasterRepository.FindBy(e => e.Id == Request.PlanID && e.Status == 1).SingleOrDefault();
                if (PlanDetail == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidRequestedPlanOrDisable;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = " In Valid Requested Plan";
                    return _Res;
                }
                var APIKey = _UserAPIKeyDetailsRepository.FindBy(e => e.APIPlanMasterID == Request.PlanID && e.UserID == UserID && e.Status == 1 && e.Id == Request.APIKeyID).FirstOrDefault();
                if (APIKey == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidAPIKey;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "InValid API Key";
                    return _Res;
                }
                if(APIKey.IPAccess==(short) enAPIIPAccess.Restrict)
                {
                    _Res.ErrorCode = enErrorCode.APIKeyFreezed;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "API Key Freezed";
                    return _Res;
                }

                if (Request.IPList.Where(e => e.ID == 0).Count() > 0)
                {
                    var WhitelistIpCount = _WhiteListIPEndPointRepository.FindBy(e => e.UserID == UserID && e.Status == 1 && e.IPType == (short)enIPType.Whitelist && e.APIPlanID == Request.PlanID).ToList();
                    var ConCurrIpCount = _WhiteListIPEndPointRepository.FindBy(e => e.UserID == UserID && e.Status == 1 && e.IPType == (short)enIPType.Concurrent && e.APIPlanID == Request.PlanID).ToList();
                    WhiteListIPLimit = PlanDetail.WhitelistedEndPoints;
                    ConCurrentIPLimit = PlanDetail.ConcurrentEndPoints;
                    if (CurrentPlanHistory.CustomeLimitId != 0)
                    {
                        WhiteListIPLimit = _APIPlanDetailRepository.FindBy(e => e.Id == CurrentPlanHistory.CustomeLimitId).FirstOrDefault().WhitelistedEndPoints;
                        ConCurrentIPLimit= _APIPlanDetailRepository.FindBy(e => e.Id == CurrentPlanHistory.CustomeLimitId).FirstOrDefault().ConcurrentEndPoints;
                    }
                    if ((Request.IPList.Where(e => e.ID == 0 && e.IPType== enIPType.Whitelist).Count() + WhitelistIpCount.Count()) > WhiteListIPLimit)
                    {
                        _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_IPWhitelistLimitExceed;
                        _Res.ReturnCode = enResponseCode.Fail;
                        _Res.ReturnMsg = "IP Whitelist Limit Exceed";
                        return _Res;
                    }
                    if ((Request.IPList.Where(e => e.ID == 0 && e.IPType == enIPType.Concurrent).Count() + ConCurrIpCount.Count()) > ConCurrentIPLimit)
                    {
                        _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_ConCurrentIPLimitExceed;
                        _Res.ReturnCode = enResponseCode.Fail;
                        _Res.ReturnMsg = "Concurrent IP Limit Exceed";
                        return _Res;
                    }
                    foreach (var IPDetail in Request.IPList.Where(e => e.ID == 0))
                    {
                        var ExistAlias = _WhiteListIPEndPointRepository.FindBy(e => e.AliasName == IPDetail.AliasName && e.UserID==UserID && e.APIPlanID== Request.PlanID).SingleOrDefault();
                        if (ExistAlias != null)
                        {
                            _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_DuplicateAliasName;
                            _Res.ReturnCode = enResponseCode.Fail;
                            _Res.ReturnMsg = "Duplicate Alias Name";
                            return _Res;
                        }
                        var ExistIP = _WhiteListIPEndPointRepository.FindBy(e => e.IPAddress == IPDetail.IPAddress && e.UserID == UserID && e.APIPlanID == Request.PlanID).SingleOrDefault();
                        if (ExistIP != null)
                        {
                            _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_DuplicateIPAddress;
                            _Res.ReturnCode = enResponseCode.Fail;
                            _Res.ReturnMsg = "Duplicate IP Address";
                            return _Res;
                        }
                    }
                    foreach (var IPDetail in Request.IPList.Where(e => e.ID == 0))
                    {
                        var NewModel = _WhiteListIPEndPointRepository.Add(new WhiteListIPEndPoint()
                        {
                            AliasName = IPDetail.AliasName,
                            APIPlanID = Request.PlanID,
                            CreatedBy = UserID,
                            CreatedDate = DateTime.UtcNow,
                            IPAddress = IPDetail.IPAddress,
                            UserID = UserID,
                            IPType = (short)IPDetail.IPType,
                            Status = 1
                        });
                        IPDetail.ID = NewModel.Id;
                    }
                }
                
                foreach (var obj in Request.IPList)
                {
                    var IsExistKey = _APIKeyWhitelistIPConfigRepository.FindBy(e => e.APIKeyID == Request.APIKeyID && e.IPId == obj.ID).FirstOrDefault();
                    if (IsExistKey == null)
                    {
                        var subModel = _APIKeyWhitelistIPConfigRepository.Add(new APIKeyWhitelistIPConfig()
                        {
                            CreatedBy = UserID,
                            CreatedDate = DateTime.UtcNow,
                            APIKeyID = Request.APIKeyID,
                            IPId = obj.ID,
                            Status = 1
                        });
                    }
                }
                APIKey.IPAccess = 1;
                APIKey.UpdatedBy = UserID;
                APIKey.UpdatedDate = DateTime.UtcNow;

                _UserAPIKeyDetailsRepository.Update(APIKey);

                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public WhitelistIPListResponse GetWhitelistIP(long PlanId, long UserID,long? KeyID)
        {
            WhitelistIPListResponse _Res = new WhitelistIPListResponse();
            List<WhitelistIPList> whitelistIPs = new List<WhitelistIPList>();
            long IPLimit = 0;
            try
            {
                var CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Status == 1 && e.UserID == UserID && e.APIPlanMasterID == PlanId).SingleOrDefault();
                if (CurrentPlanHistory == null)
                {
                    _Res.Response = whitelistIPs;
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_ActivePlanNotFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Active Plan Not Found";
                    return _Res;
                }
                var PlanDetail = _APIPlanMasterRepository.FindBy(e => e.Id == PlanId && e.Status == 1).SingleOrDefault();
                if (PlanDetail == null)
                {
                    _Res.Response = whitelistIPs;
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidRequestedPlanOrDisable;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = " In Valid Requested Plan";
                    return _Res;
                }
                IPLimit = PlanDetail.WhitelistedEndPoints;
                if (CurrentPlanHistory.CustomeLimitId != 0)
                    IPLimit = _APIPlanDetailRepository.FindBy(e => e.Id == CurrentPlanHistory.CustomeLimitId).FirstOrDefault().WhitelistedEndPoints;

                if (KeyID!=null)
                {
                    var APIKeyDetail = _UserAPIKeyDetailsRepository.FindBy(e => e.UserID == UserID && e.Status == 1 && e.Id ==Convert.ToInt64(KeyID)).FirstOrDefault();
                    if (APIKeyDetail == null)
                    {
                        _Res.Response = whitelistIPs;
                        _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidAPIKey;
                        _Res.ReturnCode = enResponseCode.Fail;
                        _Res.ReturnMsg = "In Valid APIKey";
                        return _Res;
                    }
                    var iplist2 = _APIconfiRepository.GetKeyWiseIPList(UserID, APIKeyDetail.APIPlanMasterID);
                    var iplist3 = iplist2.Where(e => e.APIKeyID == KeyID).ToList();
                    if(iplist3.Count>0)
                    {
                        foreach (var obj in iplist3)
                        {
                            whitelistIPs.Add(new WhitelistIPList()
                            {
                                IPId = obj.ID,
                                AliasName = obj.AliasName,
                                CreatedDate = obj.CreatedDate,
                                IPAddress = obj.IPAddress,
                                IPType = obj.IPType
                            });
                        }
                    }
                    _Res.IPLimitCount = IPLimit;
                    _Res.IPCount = iplist3.Count();
                    _Res.Response = whitelistIPs;
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "Success";
                    return _Res;
                }
                //var keyid = _UserAPIKeyDetailsRepository.FindBy(e => e.APIPlanMasterID == PlanDetail.Id && e.UserID == UserID && e.Status == 1).ToList().FirstOrDefault();
                var iplist = _WhiteListIPEndPointRepository.FindBy(e => e.UserID == UserID && e.Status == 1  && e.APIPlanID== PlanId);
                //if (iplist.Count() > 0)
                //{
                    foreach (var obj in iplist)
                    {
                        whitelistIPs.Add(new WhitelistIPList()
                        {
                            IPId=obj.Id,
                            AliasName = obj.AliasName,
                            CreatedDate = obj.CreatedDate,
                            IPAddress = obj.IPAddress,
                            IPType=obj.IPType
                        });
                    }
                //}
                

                _Res.IPLimitCount = IPLimit;
                _Res.IPCount = iplist.Count();
                _Res.Response = whitelistIPs;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public BizResponseClass DeleteWhitelistIP(long IPId, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var Model = _WhiteListIPEndPointRepository.GetById(IPId);
                if (Model == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InvalidIP;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "In Valid APIKey";
                    return _Res;
                }

                Model.Status = 9;
                Model.UpdatedBy = UserID;
                Model.UpdatedDate = DateTime.UtcNow;
                _WhiteListIPEndPointRepository.Update(Model);
                _APIconfiRepository.DisableWhiteListIPConfigurationIPwise(IPId, UserID);

                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        #endregion

        #region UserAPICustomeLimit
        public BizResponseClass SetUserAPICustomeLimit(UserAPICustomeLimitRequest Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                //if (Request.SubscribeID==null)
                //{
                //    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_EnterCurrentSubscribeID;
                //    _Res.ReturnCode = enResponseCode.Fail;
                //    _Res.ReturnMsg = "Enter Current Subscribe ID";
                //    return _Res;
                //}
                var CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Status == 1 && e.UserID == UserID && e.Id==Request.SubscribeID).SingleOrDefault();
                if (CurrentPlanHistory == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_ActivePlanNotFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Active Plan Not Found";
                    return _Res;
                }
                if(CurrentPlanHistory.CustomeLimitId!=0)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_CustomeLimitAlreadyExist;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Custome Limit Already Exist";
                    return _Res;
                }
                var Checklimit = this.CheckCustomeLimit(Request, CurrentPlanHistory.APIPlanMasterID);
                if (Checklimit.ReturnCode != enResponseCode.Success)
                    return Checklimit;
                var newModel=_APIPlanDetailRepository.Add(new APIPlanDetail() {
                    APIPlanID=CurrentPlanHistory.APIPlanMasterID,
                    ConcurrentEndPoints=Request.ConcurrentEndPoints,
                    CreatedBy=UserID,
                    CreatedDate=DateTime.UtcNow,
                    HistoricalDataMonth=Request.HistoricalDataMonth,
                    MaxOrderPerSec=Request.MaxOrderPerSec,
                    MaxPerDay=Request.MaxPerDay,
                    MaxPerMinute=Request.MaxPerMinute,
                    MaxPerMonth=Request.MaxPerMonth,
                    MaxRecPerRequest=Request.MaxRecPerRequest,
                    MaxReqSize=Request.MaxReqSize,
                    MaxResSize=Request.MaxResSize,
                    Status=1,
                    WhitelistedEndPoints=Request.WhitelistedEndPoints,
                    UserId=UserID
                });
                if (Request.ReadonlyAPI.Count > 0)
                {
                    foreach (var methodId in Request.ReadonlyAPI)
                    {
                        var newModel2 = _APIPlanMethodCongifRepo.Add(new APIPlanMethodConfiguration()
                        {
                            APIPlanMasterID = CurrentPlanHistory.APIPlanMasterID,
                            CustomeLimitId = newModel.Id,
                            CreatedBy = UserID,
                            CreatedDate = DateTime.UtcNow,
                            RestMethodID = methodId,
                            Status =1
                        });
                    }
                }
                if (Request.FullAccessAPI.Count > 0)
                {
                    foreach (var methodId in Request.FullAccessAPI)
                    {
                        var newModel2 = _APIPlanMethodCongifRepo.Add(new APIPlanMethodConfiguration()
                        {
                            APIPlanMasterID=CurrentPlanHistory.APIPlanMasterID,
                            CustomeLimitId = newModel.Id,
                            CreatedBy = UserID,
                            CreatedDate = DateTime.UtcNow,
                            RestMethodID = methodId,
                            Status = 1
                        });
                    }
                }
                CurrentPlanHistory.CustomeLimitId = newModel.Id;
                var renewStatus = CurrentPlanHistory.RenewStatus;
                _SubScribePlanRepository.Update(CurrentPlanHistory);
                while (renewStatus == 1)
                {
                    CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Id == CurrentPlanHistory.NextAutoRenewId && e.Status == 0).SingleOrDefault();
                    if (CurrentPlanHistory == null)
                        break;
                    CurrentPlanHistory.CustomeLimitId = newModel.Id;
                    renewStatus = CurrentPlanHistory.RenewStatus;
                    _SubScribePlanRepository.Update(CurrentPlanHistory);
                }

                _backOfficeAPIConfigService.ReloadAPIPlanMethodConfiguration();
                _backOfficeAPIConfigService.ReloadAPIPlanDetails();
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public BizResponseClass UpdateUserAPICustomeLimit(UserAPICustomeLimitRequest Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                if (Request.LimitID == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InvalidCustimeLimitID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Invalid Custime Limit ID";
                    return _Res;
                }
                //if (Request.SubscribeID == null)
                //{
                //    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_EnterCurrentSubscribeID;
                //    _Res.ReturnCode = enResponseCode.Fail;
                //    _Res.ReturnMsg = "Enter Current Subscribe ID";
                //    return _Res;
                //}
                var CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Status == 1 && e.UserID == UserID && e.Id == Request.SubscribeID).SingleOrDefault();
                if (CurrentPlanHistory == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_ActivePlanNotFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Active Plan Not Found";
                    return _Res;
                }
                if (CurrentPlanHistory.CustomeLimitId!=Request.LimitID)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InvalidCustimeLimitID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Invalid Custime Limit ID";
                    return _Res;
                }

                var Checklimit = this.CheckCustomeLimit(Request, CurrentPlanHistory.APIPlanMasterID);
                if (Checklimit.ReturnCode != enResponseCode.Success)
                    return Checklimit;

                var Model = _APIPlanDetailRepository.GetById(Convert.ToInt64(Request.LimitID));

                Model.ConcurrentEndPoints = Request.ConcurrentEndPoints;
                Model.HistoricalDataMonth = Request.HistoricalDataMonth;
                Model.MaxOrderPerSec = Request.MaxOrderPerSec;
                Model.MaxPerDay = Request.MaxPerDay;
                Model.MaxPerMinute = Request.MaxPerMinute;
                Model.MaxPerMonth = Request.MaxPerMonth;
                Model.MaxRecPerRequest = Request.MaxRecPerRequest;
                Model.MaxReqSize = Request.MaxReqSize;
                Model.MaxResSize = Request.MaxResSize;
                Model.UpdatedBy = UserID;
                Model.UpdatedDate = DateTime.UtcNow;

                _APIPlanDetailRepository.Update(Model);

                _APIconfiRepository.SetDisablePlanMethodsStatus(CurrentPlanHistory.APIPlanMasterID, UserID,Convert.ToInt64( Request.LimitID));
                var list = _APIPlanMethodCongifRepo.FindBy(e => e.CustomeLimitId == Convert.ToInt64(Request.LimitID)).ToList();
                var methodlist = Request.FullAccessAPI.Concat(Request.ReadonlyAPI);
                foreach (var obj in methodlist)
                {
                    var method = list.Where(e => e.RestMethodID == obj).SingleOrDefault();
                    if (method == null)
                    {
                        var newModel2 = _APIPlanMethodCongifRepo.Add(new APIPlanMethodConfiguration()
                        {
                            APIPlanMasterID = CurrentPlanHistory.APIPlanMasterID,
                            CustomeLimitId =Convert.ToInt64(Request.LimitID),
                            CreatedBy = UserID,
                            CreatedDate = DateTime.UtcNow,
                            RestMethodID = obj,
                            Status = 1
                        });
                    }
                    else
                    {
                        var Model2 = _APIPlanMethodCongifRepo.GetById(method.Id);
                        Model2.Status = 1;
                        Model2.UpdatedBy = UserID;
                        Model2.UpdatedDate = DateTime.UtcNow;
                        _APIPlanMethodCongifRepo.Update(Model2);
                        
                    }
                }
                _backOfficeAPIConfigService.ReloadAPIPlanMethodConfiguration();
                _backOfficeAPIConfigService.ReloadAPIPlanDetails();
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public UserAPICustomeLimitResponse GetCustomeLimit(long SubscribeID, long UserID)
        {
            UserAPICustomeLimitResponse _Res = new UserAPICustomeLimitResponse();
            UserAPICustomeLimitResponseInfo APICustomeLimit = new UserAPICustomeLimitResponseInfo();
            Dictionary<long, String> ReadonlyList = new Dictionary<long, string>();
            Dictionary<long, String> FullAccessList = new Dictionary<long, string>();
            try
            {
                var CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Id == SubscribeID && e.Status==1 && e.UserID== UserID).FirstOrDefault();
                if (CurrentPlanHistory == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_ActivePlanNotFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Active Plan Not Found";
                    return _Res;
                }
                var CustomeLimit = _APIPlanDetailRepository.FindBy(e => e.Id == CurrentPlanHistory.CustomeLimitId).FirstOrDefault();
                if(CustomeLimit==null)
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                var list2 = _APIconfiRepository.GetPlanMethods(CustomeLimit.Id);

                APICustomeLimit.ConcurrentEndPoints = CustomeLimit.ConcurrentEndPoints;
                APICustomeLimit.HistoricalDataMonth = CustomeLimit.HistoricalDataMonth;
                APICustomeLimit.WhitelistedEndPoints = CustomeLimit.WhitelistedEndPoints;
                APICustomeLimit.LimitID = CustomeLimit.Id;
                APICustomeLimit.MaxOrderPerSec = CustomeLimit.MaxOrderPerSec;
                APICustomeLimit.MaxPerDay = CustomeLimit.MaxPerDay;
                APICustomeLimit.MaxPerMinute = CustomeLimit.MaxPerMinute;
                APICustomeLimit.MaxPerMonth = CustomeLimit.MaxPerMonth;
                APICustomeLimit.MaxRecPerRequest = CustomeLimit.MaxRecPerRequest;
                APICustomeLimit.MaxReqSize = CustomeLimit.MaxReqSize;
                APICustomeLimit.MaxResSize = CustomeLimit.MaxResSize;

                ReadonlyList = list2.Where(e =>  e.IsReadOnly == 1).ToDictionary(e => e.RestMethodID, e => e.MethodName);
                FullAccessList = list2.Where(e =>  e.IsFullAccess == 1).ToDictionary(e => e.RestMethodID, e => e.MethodName);

                APICustomeLimit.ReadOnlyAPI = ReadonlyList;
                APICustomeLimit.FullAccessAPI = FullAccessList;

                _Res.Response = APICustomeLimit;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public BizResponseClass SetDefaultAPILimits (long LimitId)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                
                var CustomeLimit = _APIPlanDetailRepository.FindBy(e => e.Id == LimitId && e.Status==1).FirstOrDefault();
                if (CustomeLimit == null)
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                var APIPlanDetail = _APIPlanMasterRepository.GetById(CustomeLimit.APIPlanID);
                if(APIPlanDetail==null)
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }

                CustomeLimit.ConcurrentEndPoints = APIPlanDetail.ConcurrentEndPoints;
                CustomeLimit.HistoricalDataMonth = APIPlanDetail.HistoricalDataMonth;
                CustomeLimit.WhitelistedEndPoints = APIPlanDetail.WhitelistedEndPoints;
                CustomeLimit.MaxOrderPerSec = APIPlanDetail.MaxOrderPerSec;
                CustomeLimit.MaxPerDay = APIPlanDetail.MaxPerDay;
                CustomeLimit.MaxPerMinute = APIPlanDetail.MaxPerMinute;
                CustomeLimit.MaxPerMonth = APIPlanDetail.MaxPerMonth;
                CustomeLimit.MaxRecPerRequest = APIPlanDetail.MaxRecPerRequest;
                CustomeLimit.MaxReqSize = APIPlanDetail.MaxReqSize;
                CustomeLimit.MaxResSize = APIPlanDetail.MaxResSize;

                _APIPlanDetailRepository.Update(CustomeLimit);

                var MethodList = _APIPlanMethodCongifRepo.FindBy(e => e.APIPlanMasterID == CustomeLimit.APIPlanID && e.CustomeLimitId == 0 && e.Status==1).ToList();
                if(MethodList.Count>0)
                {
                    _APIconfiRepository.SetDefaultConfigurationMethod(CustomeLimit.APIPlanID, LimitId);
                }
                _backOfficeAPIConfigService.ReloadAPIPlanMethodConfiguration();
                _backOfficeAPIConfigService.ReloadAPIPlanDetails();
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public BizResponseClass CheckCustomeLimit(UserAPICustomeLimitRequest Request, long PlanID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var PlanDetail = _APIPlanMasterRepository.GetById(PlanID);
                if (Request.ConcurrentEndPoints>PlanDetail.ConcurrentEndPoints)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_ConcurrentEndPointsLimitExceed;
                    _Res.ReturnMsg = "Concurrent End Points Limit Exceed";
                }
                else if(Request.HistoricalDataMonth>PlanDetail.HistoricalDataMonth)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_HistoricalDataMonthLimitExceed;
                    _Res.ReturnMsg = "Historical Data Month Limit Exceed";
                }
                else if(Request.MaxOrderPerSec>PlanDetail.MaxOrderPerSec)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_MaxOrderPerSecLimitExceed;
                    _Res.ReturnMsg = "Max Order Per-Sec Limit Exceed";
                }
                else if(Request.MaxPerDay>PlanDetail.MaxPerDay)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_MaxPerDayLimitExceed;
                    _Res.ReturnMsg = "Max Per-Day Limit Exceed";
                }
                else if(Request.MaxPerMinute>PlanDetail.MaxPerMinute)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_MaxPerMinuteLimitExceed;
                    _Res.ReturnMsg = "Max Per-Minute Limit Exceed";
                }
                else if (Request.MaxPerMonth > PlanDetail.MaxPerMonth)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_MaxPerMonthLimitExceed;
                    _Res.ReturnMsg = "Max Per-Month Limit Exceed";
                }
                else if (Request.MaxRecPerRequest > PlanDetail.MaxRecPerRequest)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_MaxRecPerRequestLimitExceed;
                    _Res.ReturnMsg = "Max Record PerRequest Limit Exceed";
                }
                else if (Request.MaxReqSize > PlanDetail.MaxReqSize)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_MaxReqSizeLimitExceed;
                    _Res.ReturnMsg = "Max Request Size Limit Exceed";
                }
                else if (Request.MaxResSize > PlanDetail.MaxResSize)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_MaxResSizeLimitExceed;
                    _Res.ReturnMsg = "Max Response Size Limit Exceed";
                }
                else if (Request.WhitelistedEndPoints > PlanDetail.WhitelistedEndPoints)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_WhitelistedEndPointsLimitExceed;
                    _Res.ReturnMsg = "Whitelisted EndPoints Limit Exceed";
                }
                else
                {
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "Success";
                    return _Res;
                }
                _Res.ReturnCode = enResponseCode.Fail;
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        #endregion

        
    }
}
