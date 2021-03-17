using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Entities.SignalR;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.FeedConfiguration;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Configuration.FeedConfiguration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.Services.Configuration
{
    public class ExchangeFeedConfiguration : IExchangeFeedConfiguration
    {
        private readonly ILogger<ExchangeFeedConfiguration> _logger;
        private ICommonRepository<FeedLimitCounts> _FeedLimitCountRepository;
        private ICommonRepository<SocketFeedConfiguration> _SocketFeedConfigRepository;
        private ICommonRepository<SocketFeedLimits> _SocketFeedLimitsRepository;
        private ICommonRepository<SocketMethods> _SocketMethodsRepository;
        private IMemoryCache _cache { get; set; }
        private static List<SocketFeedConfiguration> feedConfigurationsList = new List<SocketFeedConfiguration>();
        private static List<SocketFeedLimits> feedLimitsList = new List<SocketFeedLimits>();
        private static List<FeedLimitCounts> feedLimitCounts = new List<FeedLimitCounts>();
        private FeedLimitCounts LimitCount;
        private readonly IFeedlimitcountQueue _feedlimitcountQueue;
        private readonly IBackOfficeTrnRepository _backOfficeTrnRepository;
        private static long[] invokeCount;
        //static long cnt = 0;

        public ExchangeFeedConfiguration(ILogger<ExchangeFeedConfiguration> logger, ICommonRepository<FeedLimitCounts> FeedLimitCountRepository,
            ICommonRepository<SocketFeedConfiguration> SocketFeedConfigRepository, ICommonRepository<SocketFeedLimits> SocketFeedLimitsRepository,
            IMemoryCache cache, IFeedlimitcountQueue feedlimitcountQueue, ICommonRepository<SocketMethods> SocketMethodsRepository, IBackOfficeTrnRepository backOfficeTrnRepository)
        {
            _logger = logger;
            _FeedLimitCountRepository = FeedLimitCountRepository;
            _SocketFeedConfigRepository = SocketFeedConfigRepository;
            _SocketFeedLimitsRepository = SocketFeedLimitsRepository;
            _cache = cache;
            _feedlimitcountQueue = feedlimitcountQueue;
            _SocketMethodsRepository = SocketMethodsRepository;
            _backOfficeTrnRepository = backOfficeTrnRepository;

            feedConfigurationsList = _cache.Get<List<SocketFeedConfiguration>>("feedConfigurationsList");
            if (feedConfigurationsList == null)
            {
                _cache.Set("feedConfigurationsList", _SocketFeedConfigRepository.List());
            }

            feedLimitsList = _cache.Get<List<SocketFeedLimits>>("feedLimitsList");
            if (feedLimitsList == null)
            {
                _cache.Set("feedLimitsList", _SocketFeedLimitsRepository.List());
            }

            feedLimitCounts = _cache.Get<List<FeedLimitCounts>>("feedLimitCounts");
            if (feedLimitsList == null)
            {
                var list = _FeedLimitCountRepository.List();
                _cache.Set("feedLimitCounts", list);
                invokeCount = new long[list.Count];
                invokeCount= Array.ConvertAll<FeedLimitCounts, long>(list.ToArray(), x => (long)x.LimitCount);
                foreach (var obj in list)
                {
                    _cache.Set("feedLimitCounts"+obj.MethodID, obj);
                }
            }
        }

        #region cacheMethod

        public List<SocketFeedConfiguration> GetFeedConfigurations()
        {
            return _cache.Get<List<SocketFeedConfiguration>>("feedConfigurationsList");
        }

        public List<SocketFeedLimits> GetFeedLimits()
        {
            return _cache.Get<List<SocketFeedLimits>>("feedLimitsList");
        }

        public List<FeedLimitCounts> GetLimitCounts()
        {
            return _cache.Get<List<FeedLimitCounts>>("feedLimitCounts");
        }

        public void UpdateAndReloadFeedLimitCount(FeedLimitCounts Data)
        {
            try
            {
                var model = _FeedLimitCountRepository.GetById(Data.Id);
                model.LimitCount = Data.LimitCount;
                model.CreatedDate = Data.CreatedDate;
                model.UpdatedDate = Data.UpdatedDate;
                _FeedLimitCountRepository.Update(model);
                //this.ReloadFeedLimitCount();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        public void ReloadFeedLimitCount()
        {
            try
            {
                var list = _FeedLimitCountRepository.List();
                _cache.Set("feedLimitCounts", list);
                foreach (var obj in list)
                {
                    _cache.Set("feedLimitCounts" + obj.MethodID, obj);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        public void ReloadFeedConfigurations()
        {
            try
            {
                feedConfigurationsList = _SocketFeedConfigRepository.List();
                _cache.Set("feedConfigurationsList",feedConfigurationsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        public void ReloadFeedLimits()
        {
            try
            {
                feedLimitsList = _SocketFeedLimitsRepository.List();
                _cache.Set("feedLimitsList", feedLimitsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        #endregion

        #region Limit Check method
        public BizResponseClass CheckFeedLimit(short MethodID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                _Res.ReturnCode = enResponseCode.Fail;
                _Res.ReturnMsg = "Fail";

                long cnt = (long)invokeCount.GetValue(MethodID-1);
                var configObj= feedConfigurationsList.Find(e => e.MethodID == Convert.ToInt16(MethodID) && e.Status==1);
                
                if(configObj == null)
                {
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnMsg = "Success";
                    return _Res;
                }
                var limitID = configObj.FeedLimitID;
                var LimitConfig = feedLimitsList.Find(e => e.Id == limitID && e.Status == 1);
                if (LimitConfig == null)
                {
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnMsg = "Success";
                    return _Res;
                }
                LimitCount = new FeedLimitCounts();
                LimitCount = _cache.Get<FeedLimitCounts>("feedLimitCounts"+MethodID);
                
                switch (Convert.ToInt16(LimitConfig.LimitType))
                {
                    case (short)enMethodLimitType.None:
                        break;
                    case (short)enMethodLimitType.AllTime:
                        if (cnt >= LimitConfig.MaxLimit)
                        {
                            _Res.ErrorCode = enErrorCode.DataPerSecValidationFail;
                            return _Res;
                        }
                        else
                        {
                            cnt = cnt + 1;
                            HelperForLog.WriteLogForActivity("ExchangeFeedConfiguration ", " ExchangeFeedConfiguration", "cnt " + cnt + " Method ID:" + MethodID);
                            invokeCount.SetValue(cnt, MethodID - 1);
                            LimitCount.LimitCount = cnt;
                            LimitCount.UpdatedDate = Helpers.UTC_To_IST();
                            //_feedlimitcountQueue.Enqueue(LimitCount);
                        }
                        break;
                    case (short)enMethodLimitType.PerSec:
                        if (cnt >= LimitConfig.MaxLimit)
                        {
                            if ((Helpers.UTC_To_IST().AddSeconds(-1) - LimitCount.CreatedDate).TotalSeconds >= 1)
                            {
                                invokeCount.SetValue(0, MethodID - 1);
                                LimitCount.UpdatedDate = null;
                                LimitCount.LimitCount = 0;
                                LimitCount.CreatedDate = Helpers.UTC_To_IST();
                            }
                            else
                            {
                                _Res.ErrorCode = enErrorCode.DataPerSecValidationFail;
                                return _Res;
                            }
                        }
                        else
                        {
                            cnt = cnt + 1;
                            HelperForLog.WriteLogForActivity("ExchangeFeedConfiguration ", " ExchangeFeedConfiguration", "cnt " + cnt + " Method ID:" + MethodID);
                            invokeCount.SetValue(cnt, MethodID - 1);
                            LimitCount.LimitCount = cnt;
                            LimitCount.UpdatedDate = Helpers.UTC_To_IST();
                            //_feedlimitcountQueue.Enqueue(LimitCount);
                        }
                        break;
                    case (short)enMethodLimitType.PerMin:
                        if (cnt >= LimitConfig.MaxLimit)
                        {
                            var diff = (Helpers.UTC_To_IST().AddMinutes(-1) - LimitCount.CreatedDate).TotalSeconds;
                            if ((Helpers.UTC_To_IST().AddMinutes(-1) - LimitCount.CreatedDate).TotalSeconds >= 1)
                            {
                                invokeCount.SetValue(0, MethodID - 1);
                                LimitCount.UpdatedDate = null;
                                LimitCount.LimitCount = 0;
                                LimitCount.CreatedDate = Helpers.UTC_To_IST();
                                _feedlimitcountQueue.Enqueue(LimitCount);
                            }
                            else
                            {
                                _Res.ErrorCode = enErrorCode.DataPerMinValidationFail;
                                return _Res;
                            }
                        }
                        else
                        {
                            cnt = cnt + 1;
                            HelperForLog.WriteLogForActivity("ExchangeFeedConfiguration ", " ExchangeFeedConfiguration", "cnt " + cnt + " Method ID:"+MethodID);
                            invokeCount.SetValue(cnt, MethodID - 1);
                            LimitCount.LimitCount = cnt;
                            LimitCount.UpdatedDate = Helpers.UTC_To_IST();
                            //_feedlimitcountQueue.Enqueue(LimitCount);
                        }
                        break;
                    case (short)enMethodLimitType.PerHr:

                        if (cnt >= LimitConfig.MaxLimit)
                        {
                            //var diff = (Helpers.UTC_To_IST().AddHours(-1) - LimitCount.CreatedDate).TotalSeconds;
                            if ((Helpers.UTC_To_IST().AddHours(-1) - LimitCount.CreatedDate).TotalSeconds >= 1)
                            {
                                invokeCount.SetValue(0, MethodID - 1);
                                LimitCount.UpdatedDate = null;
                                LimitCount.LimitCount = 0;
                                LimitCount.CreatedDate = Helpers.UTC_To_IST();
                                _feedlimitcountQueue.Enqueue(LimitCount);
                            }
                            else
                            {
                                _Res.ErrorCode = enErrorCode.DataPerHrValidationFail;
                                return _Res;
                            }
                        }
                        else
                        {
                            cnt = cnt + 1;
                            HelperForLog.WriteLogForActivity("ExchangeFeedConfiguration ", " ExchangeFeedConfiguration", "cnt " + cnt + " Method ID:" + MethodID);
                            invokeCount.SetValue(cnt, MethodID - 1);
                            LimitCount.LimitCount = cnt;
                            LimitCount.UpdatedDate = Helpers.UTC_To_IST();
                            _feedlimitcountQueue.Enqueue(LimitCount);
                        }
                        break;
                    case (short)enMethodLimitType.PerDay:
                        if (cnt >= LimitConfig.MaxLimit)
                        {
                            if ((Helpers.UTC_To_IST().AddDays(-1) - LimitCount.CreatedDate).TotalSeconds >= 24)
                            {
                                invokeCount.SetValue(0, MethodID - 1);
                                LimitCount.UpdatedDate = null;
                                LimitCount.LimitCount = 0;
                                LimitCount.CreatedDate = Helpers.UTC_To_IST();
                                _feedlimitcountQueue.Enqueue(LimitCount);
                            }
                            else
                            {
                                _Res.ErrorCode = enErrorCode.InvalidLimitPerDay;
                                return _Res;
                            }
                        }
                        else
                        {
                            cnt = cnt + 1;
                            HelperForLog.WriteLogForActivity("ExchangeFeedConfiguration ", " ExchangeFeedConfiguration", "cnt " + cnt + " Method ID:" + MethodID);
                            invokeCount.SetValue(cnt, MethodID - 1);
                            LimitCount.LimitCount = cnt;
                            LimitCount.UpdatedDate = Helpers.UTC_To_IST();
                            _feedlimitcountQueue.Enqueue(LimitCount);
                        }
                        break;
                    case (short)enMethodLimitType.PerWeek:
                        if (cnt >= LimitConfig.MaxLimit)
                        {
                            if ((Helpers.UTC_To_IST().AddDays(-7) - LimitCount.CreatedDate).TotalSeconds >= 7)
                            {
                                invokeCount.SetValue(0, MethodID - 1);
                                LimitCount.UpdatedDate = null;
                                LimitCount.LimitCount = 0;
                                LimitCount.CreatedDate = Helpers.UTC_To_IST();
                                _feedlimitcountQueue.Enqueue(LimitCount);
                            }
                            else
                            {
                                _Res.ErrorCode = enErrorCode.DataPerWeekValidationFail;
                                return _Res;
                            }
                        }
                        else
                        {
                            cnt = cnt + 1;
                            HelperForLog.WriteLogForActivity("ExchangeFeedConfiguration ", " ExchangeFeedConfiguration", "cnt " + cnt + " Method ID:" + MethodID);
                            invokeCount.SetValue(cnt, MethodID - 1);
                            LimitCount.LimitCount = cnt;
                            LimitCount.UpdatedDate = Helpers.UTC_To_IST();
                            _feedlimitcountQueue.Enqueue(LimitCount);
                        }
                        break;
                    case (short)enMethodLimitType.PerMonth:
                        if (cnt >= LimitConfig.MaxLimit)
                        {
                            if ((Helpers.UTC_To_IST().AddMonths(-1) - LimitCount.CreatedDate).TotalSeconds >= 30)
                            {
                                invokeCount.SetValue(0, MethodID - 1);
                                LimitCount.UpdatedDate = null;
                                LimitCount.LimitCount = 0;
                                LimitCount.CreatedDate = Helpers.UTC_To_IST();
                                _feedlimitcountQueue.Enqueue(LimitCount);
                            }
                            else
                            {
                                _Res.ErrorCode = enErrorCode.DataPerMonthValidationFail;
                                return _Res;
                            }
                        }
                        else
                        {
                            cnt = cnt + 1;
                            HelperForLog.WriteLogForActivity("ExchangeFeedConfiguration ", " ExchangeFeedConfiguration", "cnt " + cnt + " Method ID:" + MethodID);
                            invokeCount.SetValue(cnt, MethodID - 1);
                            LimitCount.LimitCount = cnt;
                            LimitCount.UpdatedDate = Helpers.UTC_To_IST();
                            _feedlimitcountQueue.Enqueue(LimitCount);
                        }
                        break;
                    case (short)enMethodLimitType.PerYear:
                        if (cnt >= LimitConfig.MaxLimit)
                        {
                            if ((Helpers.UTC_To_IST().AddYears(-1) - LimitCount.CreatedDate).TotalSeconds >= 365)
                            {
                                invokeCount.SetValue(0, MethodID - 1);
                                LimitCount.UpdatedDate = null;
                                LimitCount.LimitCount = 0;
                                LimitCount.CreatedDate = Helpers.UTC_To_IST();
                                _feedlimitcountQueue.Enqueue(LimitCount);
                            }
                            else
                            {
                                _Res.ErrorCode = enErrorCode.DataPerYearValidationFail;
                                return _Res;
                            }
                        }
                        else
                        {
                            cnt = cnt + 1;
                            HelperForLog.WriteLogForActivity("ExchangeFeedConfiguration ", " ExchangeFeedConfiguration", "cnt " + cnt + " Method ID:" + MethodID);
                            invokeCount.SetValue(cnt, MethodID - 1);
                            LimitCount.LimitCount = cnt;
                            LimitCount.UpdatedDate = Helpers.UTC_To_IST();
                            _feedlimitcountQueue.Enqueue(LimitCount);
                        }
                        break;
                    case (short)enMethodLimitType.PerUser:
                        break;
                }
                //this.ReloadFeedLimitCount();
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

        public BizResponseClass CheckFeedDataLimit(long DataSize, short MethodID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                //_Res.ReturnCode = enResponseCode.Fail;
                //_Res.ReturnMsg = "Fail";
                ////var DataSize = System.Text.ASCIIEncoding.ASCII.GetByteCount(request.DataObj);
                //var configObj = feedConfigurationsList.Find(e => e.MethodID == Convert.ToInt16(MethodID) && e.Status == 1);

                //if (configObj == null)
                //{
                //    _Res.ReturnCode = enResponseCode.Success;
                //    _Res.ErrorCode = enErrorCode.Success;
                //    _Res.ReturnMsg = "Success";
                //    return _Res;
                //}
                //var limitID = configObj.FeedLimitID;
                //var LimitConfig = feedLimitsList.Find(e => e.Id == limitID && e.Status == 1);
                //if (LimitConfig == null)
                //{
                //    _Res.ReturnCode = enResponseCode.Success;
                //    _Res.ErrorCode = enErrorCode.Success;
                //    _Res.ReturnMsg = "Success";
                //    return _Res;
                //}

                //if (LimitConfig.MaxSize != 0 && DataSize > LimitConfig.MaxSize)
                //{
                //    _Res.ErrorCode = enErrorCode.DataSizeValidationFail;
                //    HelperForLog.WriteLogForActivity("CheckFeedDataLimit ", " CheckFeedDataLimit", " Data size exceeded Method ID:" + MethodID);
                //    return _Res;
                //}

                //if (DataSize > LimitConfig.MaxRowCount * LimitConfig.RowLenghtSize)
                //{
                //    _Res.ErrorCode = enErrorCode.DataRowSizeValidationFail;
                //    HelperForLog.WriteLogForActivity("CheckFeedDataLimit ", " CheckFeedDataLimit", " Data Row length exceeded Method ID:" + MethodID);
                //    return _Res;
                //}
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

        #region CRUD method
        public SocketMethodResponse GetSocketMethods()
        {
            SocketMethodResponse _Res = new SocketMethodResponse();
            List<SocketMethodViewModel> Methodlist = new List<SocketMethodViewModel>();
            try
            {
                var list = _SocketMethodsRepository.FindBy(e => e.Status == 1);
                if (list == null)
                {
                    _Res.Response = Methodlist;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                foreach (var obj in list)
                {
                    Methodlist.Add(new SocketMethodViewModel()
                    {
                        ID=obj.Id,
                        MethodName = obj.MethodName,
                        ReturnMethodName = obj.ReturnMethodName,
                        PublicOrPrivate = obj.PublicOrPrivate,
                        EnumCode = obj.EnumCode,
                        Status=obj.Status
                    });
                }
                _Res.Response = Methodlist;
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

        public ExchangeLimitTypeResponse GetExchangeFeedLimitType()
        {
            ExchangeLimitTypeResponse _Res = new ExchangeLimitTypeResponse();
            List<ExchangeLimitType> limitTypes = new List<ExchangeLimitType>();
            try
            {
                foreach (enMethodLimitType foo in Enum.GetValues(typeof(enMethodLimitType)))
                {
                    limitTypes.Add(new ExchangeLimitType { ID = (int)foo, LimitType = foo.ToString() });
                }
                _Res.Response = limitTypes;
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

        public BizResponseClass AddFeedConfigurationLimit(SocketFeedLimitsRequest Request,long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                //var IsExistLimit = _SocketFeedLimitsRepository.FindBy(e => e.LimitType == Request.LimitType);
                //if(IsExistLimit!=null)
                //{
                //    _Res.ErrorCode = enErrorCode.FeedLimit_Already_Exist;
                //    _Res.ReturnCode = enResponseCode.Fail;
                //    _Res.ReturnMsg = "Feed Limit Already Exist.";
                //    return _Res;
                //}
                var newModel = _SocketFeedLimitsRepository.Add(new SocketFeedLimits() {
                    CreatedBy=UserID,
                    CreatedDate=DateTime.UtcNow,
                    LimitDesc=Request.LimitDesc,
                    LimitType=Request.LimitType,
                    MaxLimit=Request.MaxLimit,
                    MaxRecordCount=Request.MaxRecordCount,
                    MaxRowCount=Request.MaxRowCount,
                    MaxSize=Request.MaxSize,
                    MinLimit=Request.MinLimit,
                    MinRecordCount=Request.MinRecordCount,
                    MinSize=Request.MinSize,
                    RowLenghtSize=Request.RowLenghtSize,
                    Status=Request.Status
                });

                if (newModel.Id > 0)
                {
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

        public BizResponseClass UpdateFeedConfigurationLimit(SocketFeedLimitsRequest Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                if(Request.ID==null)
                {
                    _Res.ErrorCode = enErrorCode.InValid_ID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Please Insert ID.";
                    return _Res;
                }
                var Model = _SocketFeedLimitsRepository.GetById(Convert.ToInt64( Request.ID));
                Model.MaxLimit = Request.MaxLimit;
                Model.MaxRecordCount = Request.MaxRecordCount;
                Model.MaxRowCount = Request.MaxRowCount;
                Model.MaxSize = Request.MaxSize;
                Model.MinLimit = Request.MinLimit;
                Model.LimitType = Request.LimitType;
                Model.LimitDesc = Request.LimitDesc;
                Model.MinRecordCount = Request.MinRecordCount;
                Model.MinSize = Request.MinSize;
                Model.RowLenghtSize = Request.RowLenghtSize;
                Model.Status = Request.Status;
                Model.UpdatedBy = UserID;
                Model.UpdatedDate = DateTime.UtcNow;

                try
                {
                    _SocketFeedLimitsRepository.Update(Model);
                }
                catch (Exception e)
                {
                    _Res.ErrorCode = enErrorCode.DataUpdateFail;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Update Fail.";
                    return _Res;
                }

                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Data Update Successfully.";

                this.ReloadFeedLimits();

                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public SocketFeedLimitsResponse GetAllFeedConfigurationLimit()
        {
            SocketFeedLimitsResponse _Res = new SocketFeedLimitsResponse();
            List<SocketFeedLimitsViewModel> feedList = new List<SocketFeedLimitsViewModel>();
            try
            {
                var list = _SocketFeedLimitsRepository.List();
                if(list.Count==0)
                {
                    _Res.Response = feedList;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "No data Found.";
                    return _Res;
                }
                foreach(var obj in list)
                {
                    feedList.Add(new SocketFeedLimitsViewModel() {
                        ID = obj.Id,
                        LimitDesc = obj.LimitDesc,
                        LimitType = obj.LimitType,
                        MaxLimit = obj.MaxLimit,
                        MaxRecordCount = obj.MaxRecordCount,
                        MaxRowCount = obj.MaxRowCount,
                        MaxSize = obj.MaxSize,
                        MinLimit = obj.MinLimit,
                        MinRecordCount = obj.MinRecordCount,
                        MinSize = obj.MinSize,
                        RowLenghtSize = obj.RowLenghtSize,
                        Status = obj.Status
                    });
                }
                _Res.Response = feedList;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success.";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public SocketFeedLimitsListResponse GetSocketFeedLimitsLists()
        {
            SocketFeedLimitsListResponse _Res = new SocketFeedLimitsListResponse();
            List<SocketFeedLimitsListInfo> feedList = new List<SocketFeedLimitsListInfo>();
            try
            {
                var list = _SocketFeedLimitsRepository.FindBy(e=>e.Status==1);
                if (list == null)
                {
                    _Res.Response = feedList;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "No data Found.";
                    return _Res;
                }
                foreach(var obj in list)
                {
                    string  limit = "Limit "+obj.MinLimit+"-"+obj.MaxLimit +", Size "+obj.MinSize+"-"+obj.MaxSize;
                    feedList.Add(new SocketFeedLimitsListInfo()
                    {
                        ID = obj.Id,
                        LimitDesc = limit,
                        Status = obj.Status
                    });
                }
                _Res.Response = feedList;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success.";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public BizResponseClass AddSocketFeedConfig(SocketFeedConfigurationRequest Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var IsExistMethod = _SocketFeedConfigRepository.GetSingle(e => e.MethodID == Request.MethodID);
                if (IsExistMethod !=null )
                {
                    _Res.ErrorCode = enErrorCode.MethodLimit_Alredy_Exist;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Method Limit Already Exist.";
                    return _Res;
                }
                var newModel = _SocketFeedConfigRepository.Add(new SocketFeedConfiguration()
                {
                    MethodID=Request.MethodID,
                    FeedLimitID=Request.FeedLimitID,
                    Status=Request.Status,
                    CreatedBy=UserID,
                    CreatedDate=DateTime.UtcNow                 
                });
                if (newModel.Id > 0)
                {
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "Data Insert Successfully.";
                    this.ReloadFeedConfigurations();
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

        public BizResponseClass UpdateSocketFeedConfig(SocketFeedConfigurationRequest Request, long UserID)
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
                var Model = _SocketFeedConfigRepository.GetById(Convert.ToInt64(Request.ID));
                Model.FeedLimitID = Request.FeedLimitID;
                Model.Status = Request.Status;
                Model.UpdatedBy = UserID;
                Model.UpdatedDate = DateTime.UtcNow;
                try
                {
                    invokeCount.SetValue(0, Request.MethodID - 1);
                    var limitCountData = _FeedLimitCountRepository.GetSingle(e => e.MethodID == Request.MethodID);
                    limitCountData.LimitCount = 0;
                    limitCountData.UpdatedDate = DateTime.UtcNow;
                    limitCountData.CreatedDate = DateTime.UtcNow;
                    _FeedLimitCountRepository.Update(limitCountData);
                    _SocketFeedConfigRepository.Update(Model);
                }
                catch (Exception e)
                {
                    _Res.ErrorCode = enErrorCode.DataUpdateFail;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Data Update Fail.";
                    return _Res;
                }

                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Data Update Successfully.";
                this.ReloadFeedConfigurations();
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public SocketFeedConfigResponse GetAllFeedConfiguration()
        {
            SocketFeedConfigResponse _Res = new SocketFeedConfigResponse();
            List<SocketFeedConfigResData> FeedConfigList = new List<SocketFeedConfigResData>();
            try
            {
                var list = _backOfficeTrnRepository.GetAllFeedConfiguration();
                if(list.Count==0)
                {
                    _Res.Response = FeedConfigList;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                foreach(var obj in list)
                {
                    FeedConfigList.Add(new SocketFeedConfigResData()
                    {
                        EnumCode=obj.EnumCode,
                        Id=obj.Id,
                        LimitDesc=obj.LimitDesc,
                        LimitType=obj.LimitType,
                        MaxLimit=obj.MaxLimit,
                        MaxRecordCount=obj.MaxRecordCount,
                        MaxRowCount=obj.MaxRowCount,
                        MaxSize=obj.MaxSize,
                        MethodName=obj.MethodName,
                        MinLimit=obj.MinLimit,
                        MinSize=obj.MinSize,
                        RowLenghtSize=obj.RowLenghtSize,
                        Status=obj.Status,
                        LimitID=obj.LimitID,
                        MethodID=obj.MethodID
                    });
                }
                _Res.Response = FeedConfigList;
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
    }
}
