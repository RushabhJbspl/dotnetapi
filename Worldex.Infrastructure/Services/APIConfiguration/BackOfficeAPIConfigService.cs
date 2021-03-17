using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.FeedConfiguration;
using Worldex.Core.ViewModels.APIConfiguration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.Configuration
{
    public class BackOfficeAPIConfigService : IBackOfficeAPIConfigService
    {
        private readonly ILogger<BackOfficeAPIConfigService> _logger;
        private IMemoryCache _cache { get; set; }
        private readonly ICommonRepository<APIPlanMaster> _APIPlanMasterRepository;
        private readonly ICommonRepository<APIPlanDetail> _APIPlanDetailRepository;
        private readonly ICommonRepository<RestMethods> _RestMethodRepository;
        private readonly ICommonRepository<APIMethods> _APIMethodsRepository;
        private readonly ICommonRepository<APIMethodConfiguration> _APIMethodConfigRepository;
        private readonly ICommonRepository<APIPlanMethodConfiguration> _APIPlanMethodCongifRepo;
        private readonly ICommonRepository<APIPlanConfigurationHistory> _APIPlanConfigHistoryRepo;
        private readonly ICommonRepository<APIPlanMethodConfigurationHistory> _APIPlanMethodConfigHistoryRepository;
        private readonly ICommonRepository<PublicAPIKeyPolicy> _PublicAPIKeyPolicyRepository;
        private readonly IAPIConfigurationRepository _APIconfiRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommonRepository<ServiceMaster> _serviceMasterRepository;


        public BackOfficeAPIConfigService(ILogger<BackOfficeAPIConfigService> logger, ICommonRepository<APIPlanMaster> APIPlanMasterRepository,
            ICommonRepository<APIPlanDetail> APIPlanDetailRepository,// ICommonRepository<RestMethods> RestMethodRepository,
            ICommonRepository<APIPlanMethodConfiguration> APIPlanMethodCongifRepo,
            IAPIConfigurationRepository APIconfiRepository, ICommonRepository<APIPlanConfigurationHistory> APIPlanConfigHistoryRepo,
            ICommonRepository<APIPlanMethodConfigurationHistory> APIPlanMethodConfigRepository, ICommonRepository<PublicAPIKeyPolicy> PublicAPIKeyPolicyRepository,
            ICommonRepository<APIMethods> APIMethodsRepository, IMemoryCache cache, ICommonRepository<APIMethodConfiguration> APIMethodConfigRepository,
            ICommonRepository<RestMethods> RestMethodRepository, ICommonRepository<ServiceMaster> serviceMasterRepository, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _cache = cache;
            _APIPlanMasterRepository = APIPlanMasterRepository;
            _APIPlanDetailRepository = APIPlanDetailRepository;
            //_RestMethodRepository = RestMethodRepository;
            _APIPlanMethodCongifRepo = APIPlanMethodCongifRepo;
            _APIconfiRepository = APIconfiRepository;
            _APIMethodsRepository = APIMethodsRepository;
            _APIMethodConfigRepository = APIMethodConfigRepository;
            _APIPlanConfigHistoryRepo = APIPlanConfigHistoryRepo;
            _APIPlanMethodConfigHistoryRepository = APIPlanMethodConfigRepository;
            _PublicAPIKeyPolicyRepository = PublicAPIKeyPolicyRepository;
            _RestMethodRepository = RestMethodRepository;
            _serviceMasterRepository = serviceMasterRepository;
            _userManager = userManager;
        }

        #region APIPlanMaster
        public BizResponseClass AddAPIPlane(APIPlanMasterRequest Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                if(Request.PlanValidityType!=1 && Request.PlanValidityType != 2 && Request.PlanValidityType != 3)
                {
                    _Res.ErrorCode = enErrorCode.InValidValidityType;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "InValidValidityType.";
                    return _Res;
                }
                if(Request.Priority==0)
                {
                    _Res.ErrorCode = enErrorCode.InValidPriority;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Invalid Priority.";
                    return _Res;
                }
                var priorityCheck = _APIPlanMasterRepository.FindBy(e => e.Priority == Request.Priority && e.Status != 9).SingleOrDefault();
                if (priorityCheck != null)
                {
                    _Res.ErrorCode = enErrorCode.PriorityExist;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "PriorityExist.";
                    return _Res;
                }
                var Service = _serviceMasterRepository.FindBy(e => e.Id == Request.ServiceID && e.Status == 1).FirstOrDefault();
                if(Service==null)
                {
                    _Res.ErrorCode = enErrorCode.ServiceNotExist;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Service Not Exist.";
                    return _Res;
                }
                var newModel = _APIPlanMasterRepository.Add(new APIPlanMaster()
                {
                    Charge = Request.Charge,
                    PlanDesc = Request.PlanDesc,
                    PlanName = Request.PlanName,
                    PlanValidity = Request.PlanValidity,
                    IsPlanRecursive = Request.IsPlanRecursive,
                    Priority = Request.Priority,
                    Price = Request.Price,
                    ConcurrentEndPoints = Request.ConcurrentEndPoints,
                    HistoricalDataMonth = Request.HistoricalDataMonth,
                    MaxOrderPerSec = Request.MaxOrderPerSec,
                    MaxPerDay = Request.MaxPerDay,
                    MaxPerMinute = Request.MaxPerMinute,
                    MaxPerMonth = Request.MaxPerMonth,
                    MaxRecPerRequest = Request.MaxRecPerRequest,
                    MaxReqSize = Request.MaxReqSize,
                    MaxResSize = Request.MaxResSize,
                    WhitelistedEndPoints = Request.WhitelistedEndPoints,
                    CreatedIPAddress = Request.CreatedIPAddress,
                    PlanValidityType=Request.PlanValidityType,
                    ServiceID=Service.Id,
                    Currency=Service.SMSCode,
                    Status = Request.Status,
                    CreatedBy = UserID,
                    CreatedDate = DateTime.UtcNow
                });
                if (newModel.Id > 0)
                {
                    if(Request.ReadonlyAPI.Count>0)
                    {
                        foreach (var methodId in Request.ReadonlyAPI)
                        { 
                            var newModel2 = _APIPlanMethodCongifRepo.Add(new APIPlanMethodConfiguration()
                                {
                                    APIPlanMasterID = newModel.Id,
                                    CreatedBy = UserID,
                                    CreatedDate = DateTime.UtcNow,
                                    RestMethodID = methodId,
                                    Status = Request.Status
                                });
                        }
                    }
                    if (Request.FullAccessAPI.Count > 0)
                    {
                        foreach (var methodId in Request.FullAccessAPI)
                        {
                            var newModel2 = _APIPlanMethodCongifRepo.Add(new APIPlanMethodConfiguration()
                            {
                                APIPlanMasterID = newModel.Id,
                                CreatedBy = UserID,
                                CreatedDate = DateTime.UtcNow,
                                RestMethodID = methodId,
                                Status = Request.Status
                            });
                        }
                    }
                    this.ReloadAPIPlan();
                    this.ReloadAPIPlanMethodConfiguration();
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

        public BizResponseClass UpdatePlane(APIPlanMasterRequestV1 Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                if (Request.ID == null)
                {
                    _Res.ErrorCode = enErrorCode.InValid_ID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Please Insert ID.";
                    return _Res;
                }
                var Model = _APIPlanMasterRepository.GetById(Convert.ToInt64(Request.ID));
                if (Model == null)
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }

                var priorityCheck = _APIPlanMasterRepository.FindBy(e => e.Priority == Request.Priority && e.Status != 9 && e.Id != Convert.ToInt64(Request.ID)).SingleOrDefault();
                if (priorityCheck != null)
                {
                    _Res.ErrorCode = enErrorCode.PriorityExist;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Insert Fail.";
                    return _Res;
                }
                var Service = _serviceMasterRepository.FindBy(e => e.Id == Request.ServiceID && e.Status == 1).FirstOrDefault();
                if (Service == null)
                {
                    _Res.ErrorCode = enErrorCode.ServiceNotExist;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Service Not Exist.";
                    return _Res;
                }
                var HistoryModel= _APIPlanConfigHistoryRepo.Add(new APIPlanConfigurationHistory()
                {
                    PlanID=Model.Id,
                    PlanDesc = Model.PlanDesc,
                    PlanName = Model.PlanName,
                    PlanValidity = Model.PlanValidity,
                    PlanValidityType = Model.PlanValidityType,
                    Priority = Model.Priority,
                    IsPlanRecursive = Model.IsPlanRecursive,
                    Price = Model.Price,
                    Charge = Model.Charge,
                    ConcurrentEndPoints = Model.ConcurrentEndPoints,
                    HistoricalDataMonth = Model.HistoricalDataMonth,
                    MaxOrderPerSec = Model.MaxOrderPerSec,
                    MaxPerDay = Model.MaxPerDay,
                    MaxPerMinute = Model.MaxPerMinute,
                    MaxPerMonth = Model.MaxPerMonth,
                    MaxRecPerRequest = Model.MaxRecPerRequest,
                    MaxReqSize = Model.MaxReqSize,
                    MaxResSize = Model.MaxResSize,
                    WhitelistedEndPoints = Model.WhitelistedEndPoints,
                    ServiceID=Model.ServiceID,
                    Currency=Model.Currency,
                    CreatedBy= Model.CreatedBy,
                    CreatedDate=Model.CreatedDate,
                    Status=Model.Status,
                    CreatedIPAddress=Model.CreatedIPAddress,
                    UpdatedBy = Model.UpdatedBy,
                    UpdatedDate =Model.UpdatedDate,
                    LastModifyBy=UserID,
                    LastModifyDate=DateTime.UtcNow,
                    ModifyDetails="Update API Plan Details"
                });
                var Restmethodlist = _APIPlanMethodCongifRepo.FindBy(e => e.APIPlanMasterID == Convert.ToInt64(Request.ID) && e.CustomeLimitId == 0).ToList();

                if(Restmethodlist.Count>0)
                {
                    foreach (var obj in Restmethodlist)
                    {
                        var data = _APIPlanMethodConfigHistoryRepository.Add(new APIPlanMethodConfigurationHistory()
                        {
                            APIPlanHistoryID= HistoryModel.Id,
                            APIPlanMasterID=obj.APIPlanMasterID,
                            CreatedBy=obj.CreatedBy,
                            CreatedDate=obj.CreatedDate,
                            CustomeLimitId=obj.CustomeLimitId,
                            RestMethodID=obj.RestMethodID,
                            Status=obj.Status,
                            UpdatedBy=obj.UpdatedBy,
                            UpdatedDate=obj.UpdatedDate
                        });
                    }
                }

                Model.PlanDesc = Request.PlanDesc;
                Model.PlanName = Request.PlanName;
                Model.PlanValidity = Request.PlanValidity;
                Model.PlanValidityType = Request.PlanValidityType;
                Model.Priority = Request.Priority;
                Model.IsPlanRecursive = Request.IsPlanRecursive;
                Model.Price = Request.Price;
                Model.Charge = Request.Charge;
                Model.ConcurrentEndPoints = Request.ConcurrentEndPoints;
                Model.HistoricalDataMonth = Request.HistoricalDataMonth;
                Model.MaxOrderPerSec = Request.MaxOrderPerSec;
                Model.MaxPerDay = Request.MaxPerDay;
                Model.MaxPerMinute = Request.MaxPerMinute;
                Model.MaxPerMonth = Request.MaxPerMonth;
                Model.MaxRecPerRequest = Request.MaxRecPerRequest;
                Model.MaxReqSize = Request.MaxReqSize;
                Model.MaxResSize = Request.MaxResSize;
                Model.WhitelistedEndPoints = Request.WhitelistedEndPoints;
                Model.UpdatedBy = UserID;
                Model.UpdatedDate = DateTime.UtcNow;
                Model.ServiceID = Service.Id;
                Model.Currency = Service.SMSCode;
                try
                {
                    _APIPlanMasterRepository.Update(Model);
                }
                catch (Exception e)
                {
                    _Res.ErrorCode = enErrorCode.DataUpdateFail;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Update Fail.";
                    return _Res;
                }
               
                
                //logic for Add-remove methods
                //if (Request.Status == 1)
                //{
                    _APIconfiRepository.SetDisablePlanMethodsStatus(Convert.ToInt64(Request.ID), UserID);
                    var list = _APIPlanMethodCongifRepo.FindBy(e => e.APIPlanMasterID == Request.ID && e.CustomeLimitId==0).ToList();
                    var methodlist = Request.FullAccessAPI.Concat(Request.ReadonlyAPI);
                    foreach (var obj in methodlist)
                    {
                        var method = list.Where(e => e.RestMethodID == obj).SingleOrDefault();
                        if (method == null)
                        {
                            var newModel2 = _APIPlanMethodCongifRepo.Add(new APIPlanMethodConfiguration()
                            {
                                APIPlanMasterID = Convert.ToInt64(Request.ID),
                                CreatedBy = UserID,
                                CreatedDate = DateTime.UtcNow,
                                RestMethodID = obj,
                                Status = 1
                            });
                        }
                        else
                        {
                            //if (method.Status != Request.Status)
                            //{
                                var Model2 = _APIPlanMethodCongifRepo.GetById(method.Id);
                                Model2.Status = 1;
                                Model2.UpdatedBy = UserID;
                                Model2.UpdatedDate = DateTime.UtcNow;
                                _APIPlanMethodCongifRepo.Update(Model2);
                            //}
                        }
                    }
                //}
                this.ReloadAPIPlan();
                this.ReloadAPIPlanMethodConfiguration();
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public APIPlanMasterResponse GetAPIPlan()
        {
            APIPlanMasterResponse _Res = new APIPlanMasterResponse();
            List<APIPlanMasterResponseInfo> APIPlanList = new List<APIPlanMasterResponseInfo>();
            try
            {
                Dictionary<long, String> ReadonlyList = new Dictionary<long, string>();
                Dictionary<long, String> FullAccessList = new Dictionary<long, string>();
                //APIPlanList = _APIconfiRepository.ViewAPIPlanDetailBackOffice();
                var list = _APIPlanMasterRepository.List();
                if (list == null)
                {
                    _Res.Response = APIPlanList;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }

                var list2 = _APIconfiRepository.GetPlanMethods();
                //ReadonlyList = list2.ToDictionary(e => e.ID, e => e.MethodName);

                foreach (var obj in list)
                {
                    var UserName = "";
                    ReadonlyList = list2.Where(e => e.APIPlanMasterID == obj.Id && e.IsReadOnly == 1).ToDictionary(e => e.RestMethodID, e => e.MethodName);
                    FullAccessList = list2.Where(e => e.APIPlanMasterID == obj.Id && e.IsFullAccess == 1).ToDictionary(e => e.RestMethodID, e => e.MethodName);
                    if (!string.IsNullOrEmpty( obj.UpdatedBy.ToString()))
                    {
                        var user = _userManager.FindByIdAsync(obj.UpdatedBy.ToString()).Result;
                        UserName = user.UserName;
                    }
                    APIPlanList.Add(new APIPlanMasterResponseInfo()
                    {
                        Charge = obj.Charge,
                        CreatedDate = obj.CreatedDate,
                        ID = obj.Id,
                        PlanDesc = obj.PlanDesc,
                        PlanName = obj.PlanName,
                        Priority = obj.Priority,
                        PlanValidity = obj.PlanValidity,
                        ConcurrentEndPoints = obj.ConcurrentEndPoints,
                        IsPlanRecursive = obj.IsPlanRecursive,
                        CreatedBy=obj.CreatedBy,
                        CreatedIPAddress=obj.CreatedIPAddress,
                        Price = obj.Price,
                        UpdatedDate = obj.UpdatedDate,
                        Status = obj.Status,
                        HistoricalDataMonth=obj.HistoricalDataMonth,
                        Whitelistendpoints=obj.WhitelistedEndPoints,
                        MaxOrderPerSec=obj.MaxOrderPerSec,
                        MaxPerDay=obj.MaxPerDay,
                        MaxPerMinute=obj.MaxPerMinute,
                        MaxPerMonth=obj.MaxPerMonth,
                        MaxRecPerRequest=obj.MaxRecPerRequest,
                        MaxReqSize=obj.MaxReqSize,
                        MaxResSize=obj.MaxResSize,
                        PlanValidityType=obj.PlanValidityType,
                        ServiceID=obj.ServiceID,
                        UpdatedBy= UserName,
                        FullAccessAPI= FullAccessList,
                        ReadOnlyAPI= ReadonlyList
                    });
                }
                _Res.Response = APIPlanList;
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

        public BizResponseClass EnableDisableAPIPlan(long PlanID, short AllowAPIKey,short Status ,long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            short status = 0;
            var msg = "";
            try
            {
                var Model = _APIPlanMasterRepository.FindBy(e => e.Id == PlanID).FirstOrDefault();
                if (Model == null)
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                if(Status==0)
                {
                    status = 0;
                    msg = "Disable API Plan";
                }
                else if(Status==1)
                {
                    status = 1;
                    msg = "Enable API Plan";
                }
                _APIPlanConfigHistoryRepo.Add(new APIPlanConfigurationHistory()
                {
                    PlanID = Model.Id,
                    PlanDesc = Model.PlanDesc,
                    PlanName = Model.PlanName,
                    PlanValidity = Model.PlanValidity,
                    PlanValidityType = Model.PlanValidityType,
                    Priority = Model.Priority,
                    IsPlanRecursive = Model.IsPlanRecursive,
                    Price = Model.Price,
                    Charge = Model.Charge,
                    ConcurrentEndPoints = Model.ConcurrentEndPoints,
                    HistoricalDataMonth = Model.HistoricalDataMonth,
                    MaxOrderPerSec = Model.MaxOrderPerSec,
                    MaxPerDay = Model.MaxPerDay,
                    MaxPerMinute = Model.MaxPerMinute,
                    MaxPerMonth = Model.MaxPerMonth,
                    MaxRecPerRequest = Model.MaxRecPerRequest,
                    MaxReqSize = Model.MaxReqSize,
                    MaxResSize = Model.MaxResSize,
                    WhitelistedEndPoints = Model.WhitelistedEndPoints,
                    CreatedBy = Model.CreatedBy,
                    CreatedDate = Model.CreatedDate,
                    Status = Model.Status,
                    CreatedIPAddress = Model.CreatedIPAddress,
                    UpdatedBy = Model.UpdatedBy,
                    UpdatedDate = Model.UpdatedDate,
                    LastModifyBy = UserID,
                    LastModifyDate = DateTime.UtcNow,
                    ModifyDetails = msg,
                    
                });

                Model.Status = Status;
                Model.UpdatedBy = UserID;
                Model.UpdatedDate = DateTime.UtcNow;
                try
                {
                    _APIPlanMasterRepository.Update(Model);
                }
                catch (Exception e)
                {
                    _Res.ErrorCode = enErrorCode.DataUpdateFail;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Update Fail.";
                    return _Res;
                }

                //Disable-Enable all the Pending Plan
                _APIconfiRepository.CancelAllPendingPlan(PlanID, UserID,Status);
                
                if(AllowAPIKey==1 && Status==0)
                {
                    _APIconfiRepository.DisableAllPlanAPIKey(PlanID,UserID);
                }
                if(Status ==1)
                    _APIconfiRepository.EnableAllPlanAPIKey(PlanID, UserID);

                this.ReloadAPIPlan();
                this.ReloadAPIPlanMethodConfiguration();
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public void ReloadAPIPlan()
        {
            try
            {
               var list= _cache.Set("aPIPlans", _APIPlanMasterRepository.List());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public void ReloadAPIPlanDetails()
        {
            try
            {
                var list = _cache.Set("aPIPlanDetails", _APIPlanDetailRepository.List());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        #endregion

        #region APIPlanDetail
        //public APIPlanMasterResponseV2 GetAPIPlanV2()
        //{
        //    APIPlanMasterResponseV2 _Res = new APIPlanMasterResponseV2();
        //    List<APIPlanMasterResponseInfoV2> APIPlanList = new List<APIPlanMasterResponseInfoV2>();
        //    try
        //    {
        //        var list = _APIPlanMasterRepository.FindBy(e => e.Status == 1);
        //        if (list == null)
        //        {
        //            _Res.Response = APIPlanList;
        //            _Res.ErrorCode = enErrorCode.NoDataFound;
        //            _Res.ReturnCode = enResponseCode.Success;
        //            _Res.ReturnMsg = "NoDataFound";
        //            return _Res;
        //        }
        //        foreach (var obj in list)
        //        {
        //            APIPlanList.Add(new APIPlanMasterResponseInfoV2()
        //            {
        //                ID = obj.Id,
        //                PlanName = obj.PlanName,
        //                Status = obj.Status
        //            });
        //        }
        //        _Res.Response = APIPlanList;
        //        _Res.ErrorCode = enErrorCode.Success;
        //        _Res.ReturnCode = enResponseCode.Success;
        //        _Res.ReturnMsg = "Success";
        //        return _Res;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        throw ex;
        //    }
        //}
        //public BizResponseClass AddAPIPlanDetail(APIPlanDetailRequest Request, long UserID)
        //{
        //    BizResponseClass _Res = new BizResponseClass();
        //    try
        //    {
        //        var newModel = _APIPlanDetailRepository.Add(new APIPlanDetail()
        //        {
        //            APIPlanMasterID = Request.APIPlanMasterID,
        //            ConcurrentEndPoints = Request.ConcurrentEndPoints,
        //            HistoricalDataMonth = Request.HistoricalDataMonth,
        //            MaxOrderPerSec = Request.MaxOrderPerSec,
        //            MaxPerDay = Request.MaxPerDay,
        //            MaxPerMinute = Request.MaxPerMinute,
        //            MaxPerMonth = Request.MaxPerMonth,
        //            MaxRecPerRequest = Request.MaxRecPerRequest,
        //            MaxReqSize = Request.MaxReqSize,
        //            MaxResSize = Request.MaxResSize,
        //            WhitelistedEndPoints = Request.WhitelistedEndPoints,
        //            Status = Request.Status,
        //            CreatedBy = UserID,
        //            CreatedDate = DateTime.UtcNow
        //        });
        //        if (newModel.Id > 0)
        //        {
        //            _Res.ErrorCode = enErrorCode.Success;
        //            _Res.ReturnCode = enResponseCode.Success;
        //            _Res.ReturnMsg = "Data Insert Successfully.";
        //        }
        //        else
        //        {
        //            _Res.ErrorCode = enErrorCode.DataInsertFail;
        //            _Res.ReturnCode = enResponseCode.Fail;
        //            _Res.ReturnMsg = "Data Insert Fail.";
        //        }
        //        return _Res;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        throw ex;
        //    }
        //}
        //public BizResponseClass UpdateAPIPlanDetail(APIPlanDetailRequest Request, long UserID)
        //{
        //    BizResponseClass _Res = new BizResponseClass();
        //    try
        //    {
        //        if (Request.ID == null)
        //        {
        //            _Res.ErrorCode = enErrorCode.InValid_ID;
        //            _Res.ReturnCode = enResponseCode.Fail;
        //            _Res.ReturnMsg = "Please Insert ID.";
        //            return _Res;
        //        }
        //        var Model = _APIPlanDetailRepository.GetById(Convert.ToInt64(Request.ID));
        //        if (Model == null)
        //        {
        //            _Res.ErrorCode = enErrorCode.NoDataFound;
        //            _Res.ReturnCode = enResponseCode.Success;
        //            _Res.ReturnMsg = "NoDataFound";
        //            return _Res;
        //        }
        //        Model.ConcurrentEndPoints = Request.ConcurrentEndPoints;
        //        Model.HistoricalDataMonth = Request.HistoricalDataMonth;
        //        Model.MaxOrderPerSec = Request.MaxOrderPerSec;
        //        Model.MaxPerDay = Request.MaxPerDay;
        //        Model.MaxPerMinute = Request.MaxPerMinute;
        //        Model.MaxPerMonth = Request.MaxPerMonth;
        //        Model.MaxRecPerRequest = Request.MaxRecPerRequest;
        //        Model.MaxReqSize = Request.MaxReqSize;
        //        Model.MaxResSize = Request.MaxResSize;
        //        Model.WhitelistedEndPoints = Request.WhitelistedEndPoints;
        //        Model.UpdatedBy = UserID;
        //        Model.UpdatedDate = DateTime.UtcNow;

        //        try
        //        {
        //            _APIPlanDetailRepository.Update(Model);
        //        }
        //        catch (Exception e)
        //        {
        //            _Res.ErrorCode = enErrorCode.DataUpdateFail;
        //            _Res.ReturnCode = enResponseCode.Fail;
        //            _Res.ReturnMsg = "Data Update Fail.";
        //            return _Res;
        //        }

        //        _Res.ReturnCode = enResponseCode.Success;
        //        _Res.ErrorCode = enErrorCode.Success;
        //        _Res.ReturnMsg = "Success";
        //        return _Res;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        throw ex;
        //    }
        //}
        //public APIPlanDetailResponse GetAPIPlanDetails()
        //{
        //    APIPlanDetailResponse _Res = new APIPlanDetailResponse();
        //    List<APIPlanDetailResponseInfo> APIPlanDetailsList = new List<APIPlanDetailResponseInfo>();
        //    try
        //    {

        //        var list = _APIPlanDetailRepository.List();
        //        if (list == null)
        //        {
        //            _Res.Response = APIPlanDetailsList;
        //            _Res.ErrorCode = enErrorCode.NoDataFound;
        //            _Res.ReturnCode = enResponseCode.Success;
        //            _Res.ReturnMsg = "NoDataFound";
        //            return _Res;
        //        }
        //        var list2 = _APIPlanMasterRepository.List().ToList();
        //        foreach (var obj in list)
        //        {
        //            var APIPlanMaster = list2.Where(e => e.Id == obj.Id).SingleOrDefault();
        //            var APIName = APIPlanMaster == null ? "" : APIPlanMaster.PlanName;
        //            APIPlanDetailsList.Add(new APIPlanDetailResponseInfo()
        //            {
        //                ID = obj.Id,
        //                APIPlanMasterID = obj.APIPlanMasterID,
        //                APIPlanName= APIName,
        //                ConcurrentEndPoints =obj.ConcurrentEndPoints,
        //                HistoricalDataMonth=obj.HistoricalDataMonth,
        //                MaxOrderPerSec=obj.MaxOrderPerSec,
        //                MaxPerDay=obj.MaxPerDay,
        //                MaxPerMinute=obj.MaxPerMinute,
        //                MaxPerMonth=obj.MaxPerMonth,
        //                MaxRecPerRequest=obj.MaxRecPerRequest,
        //                MaxReqSize=obj.MaxReqSize,
        //                MaxResSize=obj.MaxResSize,
        //                WhitelistedEndPoints=obj.WhitelistedEndPoints,
        //                CreatedDate = obj.CreatedDate,
        //                UpdatedDate =obj.UpdatedDate,
        //                Status=obj.Status
        //            });
        //        }

        //        _Res.Response = APIPlanDetailsList;
        //        _Res.ErrorCode = enErrorCode.Success;
        //        _Res.ReturnCode = enResponseCode.Success;
        //        _Res.ReturnMsg = "Success";
        //        return _Res;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        throw ex;
        //    }
        //}
        #endregion

        #region RestMethods
        public BizResponseClass AddAPIMethod(APIMethodsRequest Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var newModel = _APIMethodsRepository.Add(new APIMethods()
                {
                    CreatedBy = UserID,
                    CreatedDate = DateTime.UtcNow,
                    IsFullAccess=Request.IsFullAccess,
                    IsReadOnly=Request.IsReadOnly,
                    MethodName=Request.MethodName,
                    Status =Request.Status
                });
                if (newModel.Id > 0)
                {
                    if(Request.RestMethods.Count>0)
                    {
                        foreach(var Methodid in Request.RestMethods)
                        {
                            _APIMethodConfigRepository.Add(new APIMethodConfiguration() {
                                CreatedBy=UserID,
                                CreatedDate=DateTime.UtcNow,
                                MethodID= Methodid,
                                ParentID= newModel.Id,
                                MethodType=1,
                                Status= Request.Status
                            });
                        }
                    }
                    if (Request.SocketMethods.Count > 0)
                    {
                        foreach (var Methodid in Request.SocketMethods)
                        {
                            _APIMethodConfigRepository.Add(new APIMethodConfiguration()
                            {
                                CreatedBy = UserID,
                                CreatedDate = DateTime.UtcNow,
                                MethodID = Methodid,
                                ParentID = newModel.Id,
                                MethodType = 2,
                                Status = Request.Status
                            });
                        }
                    }
                    this.ReloadAPIMethodConfiguration();
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

        public BizResponseClass UpdateAPIMethod(APIMethodsRequest2 Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                if (Request.ID == null)
                {
                    _Res.ErrorCode = enErrorCode.InValid_ID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Please Insert ID.";
                    return _Res;
                }
                var Model = _APIMethodsRepository.GetById(Convert.ToInt64(Request.ID));
                if (Model == null)
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                Model.IsFullAccess = Request.IsFullAccess;
                Model.IsReadOnly = Request.IsReadOnly;
                Model.UpdatedBy = UserID;
                Model.UpdatedDate = DateTime.UtcNow;
                Model.Status = Request.Status;
                try
                {
                    _APIMethodsRepository.Update(Model);
                    _APIconfiRepository.DisableAPIMethodsConfiguration(Convert.ToInt64(Request.ID));
                    if (Request.SocketMethods.Count>0)
                    {
                        foreach(var methodid in Request.SocketMethods)
                        {
                            var MethodData = _APIMethodConfigRepository.FindBy(e => e.MethodID == methodid && e.ParentID == Convert.ToInt64(Request.ID) && e.MethodType==2).FirstOrDefault();
                            if(MethodData!=null)
                            {
                                MethodData.Status =Request.Status;
                                _APIMethodConfigRepository.Update(MethodData);
                            }
                            else
                            {
                                _APIMethodConfigRepository.Add(new APIMethodConfiguration()
                                {
                                    CreatedBy = UserID,
                                    CreatedDate = DateTime.UtcNow,
                                    MethodID = methodid,
                                    ParentID = Convert.ToInt64(Request.ID),
                                    MethodType = 2,
                                    Status = Request.Status
                                });
                            }
                        }
                    }
                    if (Request.RestMethods.Count > 0)
                    {
                        foreach (var methodid in Request.RestMethods)
                        {
                            var MethodData = _APIMethodConfigRepository.FindBy(e => e.MethodID == methodid && e.ParentID == Convert.ToInt64(Request.ID) && e.MethodType == 1).FirstOrDefault();
                            if (MethodData != null)
                            {
                                MethodData.Status = Request.Status;
                                _APIMethodConfigRepository.Update(MethodData);
                            }
                            else
                            {
                                _APIMethodConfigRepository.Add(new APIMethodConfiguration()
                                {
                                    CreatedBy = UserID,
                                    CreatedDate = DateTime.UtcNow,
                                    MethodID = methodid,
                                    ParentID = Convert.ToInt64(Request.ID),
                                    MethodType = 1,
                                    Status = Request.Status
                                });
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    _Res.ErrorCode = enErrorCode.DataUpdateFail;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Update Fail.";
                    return _Res;
                }
                this.ReloadAPIMethodConfiguration();
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public APIMethodResponse GetAPIMethods()
        {
            APIMethodResponse _Res = new APIMethodResponse();
            List<APIMethodsViewModel2> RestMethodList = new List<APIMethodsViewModel2>();
            Dictionary<long, String> SocketMethods=new Dictionary<long, string>();
            Dictionary<long, String> RestMethods=new Dictionary<long, string>() ;
            try
            {
                var list = _APIMethodsRepository.List();
                var MethodList = _APIconfiRepository.GetAPIMethodConfigListBK();
                if (list == null)
                {
                    _Res.Response = RestMethodList;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                foreach(var obj in list)
                {
                    SocketMethods = MethodList.Where(e => e.MethodType == 2 && e.ParentID == obj.Id).ToDictionary(e => e.MethodID, e => e.MethodName);
                    RestMethods= MethodList.Where(e => e.MethodType == 1 && e.ParentID == obj.Id).ToDictionary(e => e.MethodID, e => e.MethodName);

                    RestMethodList.Add(new APIMethodsViewModel2()
                    {
                        ID = obj.Id,
                        IsFullAccess = obj.IsFullAccess,
                        IsReadOnly = obj.IsReadOnly,
                        MethodName = obj.MethodName,
                        Status = obj.Status,
                        RestMethods = RestMethods,
                        SocketMethods = SocketMethods
                    });
                }
                _Res.Response = RestMethodList;
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

        public APIMethodResponseV2 GetRestMethodsReadOnly()
        {
            APIMethodResponseV2 _Res = new APIMethodResponseV2();
            List<APIMethodsInfoV2> RestMethodList = new List<APIMethodsInfoV2>();
            try
            {
                var list = _APIMethodsRepository.FindBy(e => e.Status == 1 && e.IsReadOnly==1);
                if (list == null)
                {
                    _Res.Response = RestMethodList;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                foreach (var obj in list)
                {
                    RestMethodList.Add(new APIMethodsInfoV2()
                    {
                        ID = obj.Id,
                        MethodName = obj.MethodName,
                        Status = obj.Status
                    });
                }
                _Res.Response = RestMethodList;
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

        public APIMethodResponseV2 GetRestMethodsFullAccess()
        {
            APIMethodResponseV2 _Res = new APIMethodResponseV2();
            List<APIMethodsInfoV2> RestMethodList = new List<APIMethodsInfoV2>();
            try
            {
                var list = _APIMethodsRepository.FindBy(e => e.Status == 1 && e.IsFullAccess == 1);
                if (list == null)
                {
                    _Res.Response = RestMethodList;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                foreach (var obj in list)
                {
                    RestMethodList.Add(new APIMethodsInfoV2()
                    {
                        ID = obj.Id,
                        MethodName = obj.MethodName,
                        Status = obj.Status
                    });
                }
                _Res.Response = RestMethodList;
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

        public void  ReloadAPIMethodConfiguration()
        {
            try
            {
                var list= _cache.Set("MethodConfig", _APIMethodConfigRepository.List());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public RestMethodResponse GetRestMethods()
        {
            RestMethodResponse _Res = new RestMethodResponse();
            List<RestMethodViewModel> RestMethodList = new List<RestMethodViewModel>();
            try
            {
                var list = _RestMethodRepository.FindBy(e => e.Status == 1).ToList();
                if(list.Count==0)
                {
                    _Res.Response = RestMethodList;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                foreach(var obj in list)
                {
                    RestMethodList.Add(new RestMethodViewModel() {
                        ID=obj.Id,
                        MethodName=obj.MethodName,
                        Path=obj.Path
                    });
                }

                _Res.Response = RestMethodList;
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

        public void ReloadRestMethods()
        {
            try
            {
                _cache.Set("restMethods", _RestMethodRepository.List());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        
        #endregion

        #region APIPlanMethodConfiguration
        public BizResponseClass AddToMethodConfig(APIPlanMethodConfigAddRequest Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var list = _APIPlanMethodCongifRepo.FindBy(e => e.APIPlanMasterID == Request.APIPlanMasterID).ToList();

                foreach(var methodId in Request.MethodID)
                {
                    var isExist = list.Where(e => e.RestMethodID == methodId).SingleOrDefault();
                    if(isExist==null)
                    {
                        var newModel = _APIPlanMethodCongifRepo.Add(new APIPlanMethodConfiguration()
                        {
                            APIPlanMasterID=Request.APIPlanMasterID,
                            CreatedBy= UserID,
                            CreatedDate= DateTime.UtcNow,
                            RestMethodID=methodId,
                            Status=Request.Status
                        });
                    }
                }
                this.ReloadAPIPlanMethodConfiguration();
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Data Insert Successfully.";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public BizResponseClass UpdateToMethodConfiguraton(APIRestMethodUpdateRequest Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                if (Request.ID == null)
                {
                    _Res.ErrorCode = enErrorCode.InValid_ID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Please Insert ID.";
                    return _Res;
                }
                var Model = _APIPlanMethodCongifRepo.GetById(Convert.ToInt64(Request.ID));
                if (Model == null)
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                Model.Status = Request.Status;
                Model.UpdatedBy = UserID;
                Model.UpdatedDate = DateTime.UtcNow;

                try
                {
                    _APIPlanMethodCongifRepo.Update(Model);
                    this.ReloadAPIPlanMethodConfiguration();
                }
                catch (Exception e)
                {
                    _Res.ErrorCode = enErrorCode.DataUpdateFail;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Update Fail.";
                    return _Res;
                }

                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public void ReloadAPIPlanMethodConfiguration()
        {
            try
            {
                //var list = _APIPlanMethodCongifRepo.List();
                var list=_cache.Set("PlanMethod", _APIPlanMethodCongifRepo.List());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        #endregion

        #region HistoryMethods
        public APIPlanUserCountResponse GetAPIPlanUserCount(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? Status, long? PlanID)
        {
            try
            {
                return _APIconfiRepository.GetAPIPlanUserCount(Pagesize,PageNo,FromDate,ToDate,UserId,Status,PlanID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public UserSubscribeHistoryBKResponse GetUserSubscribeHistoryBK(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? Status, long? PlanID)
        {
            try
            {
                return _APIconfiRepository.GetUserSubscribeHistoryBK(Pagesize, PageNo, FromDate, ToDate, UserId, Status, PlanID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public ViewAPIPlanConfigHistoryResponse ViewAPIPlanConfiguration(long Pagesize, long PageNo, string FromDate, string ToDate,long? UserId, long? PlanID)
        {
            try
            {
                return _APIconfiRepository.ViewAPIPlanConfigHistoryBK(Pagesize, PageNo, FromDate, ToDate, UserId, PlanID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public ViewPublicAPIKeysResponse ViewPublicAPIKeysBK(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? Status, long? PlanID)
        {
            try
            {
                return _APIconfiRepository.ViewPublicAPIKeysBK(Pagesize, PageNo, FromDate, ToDate, UserId, Status, PlanID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        #endregion

        #region PublicAPIKeyPolicy
        public BizResponseClass UpdatePublicAPIKeyPolicy(PublicAPIKeyPolicyRequest Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                if (Request.AddFrequencyType != 1 && Request.AddFrequencyType != 2 && Request.AddFrequencyType != 3 && Request.AddFrequencyType != 4 &&
                    Request.DeleteFrequencyType != 1 && Request.DeleteFrequencyType != 2 && Request.DeleteFrequencyType != 3 && Request.DeleteFrequencyType != 4)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InvalidFrequencyType;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "InValidValidityType.";
                    return _Res;
                }
                var Model = _PublicAPIKeyPolicyRepository.GetById(Convert.ToInt64(Request.ID));
                if (Model == null)
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                
                Model.AddFrequency = Request.AddFrequency;
                Model.AddFrequencyType = Request.AddFrequencyType;
                Model.AddMaxLimit = Request.AddMaxLimit;
                Model.AddPerDayFrequency = Request.AddPerDayFrequency;
                Model.DeleteFrequency = Request.DeleteFrequency;
                Model.DeleteFrequencyType = Request.DeleteFrequencyType;
                Model.DeleteMaxLimit = Request.DeleteMaxLimit;
                Model.DeletePerDayFrequency = Request.DeletePerDayFrequency;
                Model.UpdatedBy = UserID;
                //Model.Status = Request.Status;

                _PublicAPIKeyPolicyRepository.Update(Model);
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public PublicAPIKeyPolicyResponse GetPublicAPIKeyPolicy()
        {
            PublicAPIKeyPolicyResponse _Res = new PublicAPIKeyPolicyResponse();
            PublicAPIKeyPolicyResponseInfo Response = new PublicAPIKeyPolicyResponseInfo();
            try
            {
                var Model = _PublicAPIKeyPolicyRepository.GetAll().FirstOrDefault();
                if(Model==null)
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                Response.AddFrequency = Model.AddFrequency;
                Response.AddFrequencyType = Model.AddFrequencyType;
                Response.AddMaxLimit = Model.AddMaxLimit;
                Response.AddPerDayFrequency = Model.AddPerDayFrequency;
                Response.CreatedBy = Model.CreatedBy;
                Response.CreatedDate = Model.CreatedDate;
                Response.DeleteFrequency = Model.DeleteFrequency;
                Response.DeleteFrequencyType = Model.DeleteFrequencyType;
                Response.DeleteMaxLimit = Model.DeleteMaxLimit;
                Response.DeletePerDayFrequency = Model.DeletePerDayFrequency;
                Response.ID = Model.Id;
                Response.Status = Model.Status;
                Response.UpdatedBy = Model.UpdatedBy;
                Response.UpdatedDate = Model.UpdatedDate;
                _Res.Response = Response;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
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

        #region Dashboard Method
        public APIRequestStatisticsCountResponse APIRequestStatisticsCount()
        {
            try
            {
                return _APIconfiRepository.APIRequestStatisticsCount();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public UserWiseAPIReqCounResponse UserWiseAPIReqCount(long Pagesize, long PageNo, short status)
        {
            try
            {
                return _APIconfiRepository.UserWiseAPIReqCount(Pagesize, PageNo, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public FrequentUseAPIRespons GetFrequentUseAPI(long Pagesize, string FromDate, string ToDate)
        {
            try
            {
                return _APIconfiRepository.GetFrequentUseAPI(Pagesize, FromDate, ToDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public MostActiveIPAddressResponse MostActiveIPAddress(long Pagesize, string FromDate, string ToDate)
        {
            try
            {
                return _APIconfiRepository.MostActiveIPAddress(Pagesize, FromDate, ToDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public APIPlanConfigurationCountResponse APIPlanConfigurationCount()
        {
            try
            {
                return _APIconfiRepository.APIPlanConfigurationCount();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public HTTPErrorsReportResponse GetHTTPErrorReport(long Pagesize, long PageNo, string FromDate, string ToDate, long? ErrorCode)
        {
            try
            {
                return _APIconfiRepository.GetHTTPErrorReport(Pagesize, PageNo, FromDate, ToDate, ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public MostActiveIPWiseReportResponse GetIPAddressWiseReport(long Pagesize, long PageNo, string FromDate, string ToDate, string IPAddress, long MemberID)
        {
            try
            {
                return _APIconfiRepository.GetIPAddressWiseReport(Pagesize, PageNo, FromDate, ToDate, IPAddress, MemberID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public FrequentUseAPIWiseReportResponse FrequentUseAPIReport(long Pagesize, long PageNo, string FromDate, string ToDate, long MemberID)
        {
            try
            {
                return _APIconfiRepository.FrequentUseAPIReport(Pagesize, PageNo, FromDate, ToDate, MemberID);
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
