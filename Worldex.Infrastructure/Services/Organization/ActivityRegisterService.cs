using Worldex.Core.Entities.Complaint;
using Worldex.Core.Entities.Organization;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Organization;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Organization
{
    public class ActivityRegisterService : IActivityLogProcess
    {
        private readonly ICustomRepository<Typemaster> _IcommonRepository;
        //private readonly ICustomExtendedRepository<HostURLMaster> _serviceProviderMasterRepo;
        private readonly WorldexContext _dbContext;
        private readonly ICustomExtendedRepository<ActivityType_Master> _IcustomExtendedRepository;
        private readonly ICustomExtendedRepository<ActivityRegister> _IActivityRepository;
        private readonly IActivityMasterConfiguration _IactivityMasterConfiguration;

        public ActivityRegisterService(ICustomRepository<Typemaster> IcommonRepository, WorldexContext dbContext, ICustomExtendedRepository<ActivityType_Master> IcustomExtendedRepository,
            ICustomExtendedRepository<ActivityRegister> IActivityRepository, IActivityMasterConfiguration IactivityMasterConfiguration)
        {
            _IcommonRepository = IcommonRepository;
            _dbContext = dbContext;
            _IcustomExtendedRepository = IcustomExtendedRepository;
            _IActivityRepository = IActivityRepository;
            _IactivityMasterConfiguration = IactivityMasterConfiguration;
        }

        //public void AddActivityLog(ActivityRegisterViewModel activityRegisterViewModel, ActivityRegisterDetViewModel activityDet)
        //{
        //    try
        //    {
        //        if (activityRegisterViewModel != null)
        //        {
        //            var activitylog = new ActivityRegister();
        //            activitylog.Id = activityRegisterViewModel.Id;
        //            activitylog.AccessToken = activityRegisterViewModel.AccessToken;
        //            activitylog.ActivityTypeId = _IcustomExtendedRepository.Table.Where(i => i.TypeMaster == activityRegisterViewModel.ActivityType && i.AliasName == activityRegisterViewModel.AliasName).Count() > 0 ? _IcustomExtendedRepository.Table.Where(i => i.TypeMaster == activityRegisterViewModel.ActivityType && i.AliasName == activityRegisterViewModel.AliasName).FirstOrDefault().Id : Guid.Empty;
        //            activitylog.AliasName = activityRegisterViewModel.AliasName;
        //            activitylog.ApplicationId = activityRegisterViewModel.ApplicationId;
        //            activitylog.Channel = activityRegisterViewModel.Channel;
        //            activitylog.Connection = activityRegisterViewModel.Connection;
        //            activitylog.CreatedBy = 0;
        //            activitylog.CreatedDate = DateTime.UtcNow;
        //            activitylog.DeviceId = activityRegisterViewModel.DeviceId;
        //            activitylog.ErrorCode = activityRegisterViewModel.ErrorCode;
        //            activitylog.HostURLId = _dbContext.HostURLMaster.Where(i => i.HostURL == activityRegisterViewModel.HostURLName).Count() > 0 ? _dbContext.HostURLMaster.Where(i => i.HostURL == activityRegisterViewModel.HostURLName).FirstOrDefault().Id : Guid.Empty;
        //            activitylog.IPAddress = activityRegisterViewModel.IPAddress;
        //            activitylog.DeviceId = activityRegisterViewModel.DeviceId;
        //            activitylog.Remark = activityRegisterViewModel.Remark;
        //            activitylog.ModuleTypeId = _IcommonRepository.Table.Where(i => i.SubType == activityRegisterViewModel.ModuleTypeName).Count() > 0 ? _IcommonRepository.Table.Where(i => i.SubType == activityRegisterViewModel.ModuleTypeName).FirstOrDefault().Id : 0;
        //            activitylog.Status = true;
        //            _IActivityRepository.Insert(activitylog);

        //            var activitylogdet = new ActivityRegisterDetail();
        //            activitylogdet.Id = activityDet.Id;
        //            activitylogdet.ActivityId = activitylog.Id;
        //            activitylogdet.Request = activityDet.Request;
        //            _dbContext.ActivityRegisterDetail.Add(activitylogdet);
        //            _dbContext.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        throw;
        //    }
        //}

        public void AddActivityLog(ActivityReqRes activityReqRes)
        {
            try
            {
                if (activityReqRes != null)
                {
                    var activitylog = new ActivityRegister();
                    activitylog.Id = activityReqRes.Id;
                    activitylog.AccessToken = activityReqRes.AccessToken;
                    //activitylog.ActivityTypeId = _IcustomExtendedRepository.Table.Where(i => i.TypeMaster == activityReqRes.ActivityType && i.AliasName == activityReqRes.AliasName).Count() > 0 ? _IcustomExtendedRepository.Table.Where(i => i.TypeMaster == activityReqRes.ActivityType && i.AliasName == activityReqRes.AliasName).FirstOrDefault().Id : Guid.Empty;

                    //Rita 25-2-19 write only table-active method's log
                    ActivityType_Master ActivityTypeId =_IactivityMasterConfiguration.GetActivityTypeData().Where(i => activityReqRes.ActivityType.ToLower().Contains(i.TypeMaster.ToLower()) && i.AliasName == activityReqRes.AliasName && i.Status==true).FirstOrDefault();
                    if (ActivityTypeId == null)
                    {
                        return;
                    }
                    activitylog.ActivityTypeId = ActivityTypeId.Id;//_IactivityMasterConfiguration.GetActivityTypeData().Where(i => i.TypeMaster == activityReqRes.ActivityType && i.AliasName == activityReqRes.AliasName).Count() > 0 ? _IactivityMasterConfiguration.GetActivityTypeData().Where(i => i.TypeMaster == activityReqRes.ActivityType && i.AliasName == activityReqRes.AliasName).FirstOrDefault().Id : Guid.Empty;
                    activitylog.AliasName = activityReqRes.AliasName;
                    activitylog.ApplicationId = activityReqRes.ApplicationId;
                    activitylog.Channel = activityReqRes.Channel;
                    activitylog.Connection = activityReqRes.Connection;
                    activitylog.CreatedBy = 0;
                    activitylog.CreatedDate = Helpers.UTC_To_IST();// DateTime.UtcNow; //Rita 25-2-19 for Indian time
                    activitylog.DeviceId = activityReqRes.DeviceId;
                    activitylog.ErrorCode = activityReqRes.ErrorCode;
                    //activitylog.HostURLId = _dbContext.HostURLMaster.Where(i => i.HostURL == activityReqRes.HostURLName).Count() > 0 ? _dbContext.HostURLMaster.Where(i => i.HostURL == activityReqRes.HostURLName).FirstOrDefault().Id : Guid.Empty;
                    activitylog.HostURLId = _IactivityMasterConfiguration.GetHostURLData().Where(i => i.HostURL == activityReqRes.HostURLName).Count() > 0 ? _IactivityMasterConfiguration.GetHostURLData().Where(i => i.HostURL == activityReqRes.HostURLName).FirstOrDefault().Id : Guid.Empty;
                    activitylog.IPAddress = activityReqRes.IPAddress;
                    activitylog.DeviceId = activityReqRes.DeviceId;
                    activitylog.Remark = activityReqRes.Remark;
                    //activitylog.ModuleTypeId = _IcommonRepository.Table.Where(i => i.SubType == activityReqRes.ModuleTypeName).Count() > 0 ? _IcommonRepository.Table.Where(i => i.SubType == activityReqRes.ModuleTypeName).FirstOrDefault().Id : 0;
                    activitylog.ModuleTypeId = _IactivityMasterConfiguration.GetTypeMasterData().Where(i => i.SubType == activityReqRes.ModuleTypeName).Count() > 0 ? _IactivityMasterConfiguration.GetTypeMasterData().Where(i => i.SubType == activityReqRes.ModuleTypeName).FirstOrDefault().Id : 0;
                    activitylog.Status = true;
                    _IActivityRepository.Insert(activitylog);

                    var activitylogdet = new ActivityRegisterDetail();
                    activitylogdet.Id = activityReqRes.ActivityDetId;
                    activitylogdet.ActivityId = activityReqRes.Id;
                    activitylogdet.Request = activityReqRes.Request;
                    _dbContext.ActivityRegisterDetail.Add(activitylogdet);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                //ex.ToString();
                HelperForLog.WriteErrorLog("AddActivityLogError", "ActivityRegisterService", ex);
                //throw;
            }
        }

        //public void UpdateActivityLog(ActivityRegisterViewModel activityRegisterViewModel, ActivityRegisterDetViewModel activityRegisterDetViewModel)
        //{
        //    try
        //    {
        //        if (activityRegisterViewModel != null)
        //        {
        //            var activitylog = _IActivityRepository.Table.FirstOrDefault(i => i.Id == activityRegisterViewModel.Id);
        //            var activitydetlog = _dbContext.ActivityRegisterDetail.FirstOrDefault(i => i.ActivityId == activitylog.Id);
        //            if (activitylog != null && activitydetlog != null)
        //            {
        //                activitylog.ReturnCode = activityRegisterViewModel.ReturnCode;
        //                activitylog.ReturnMsg = activityRegisterViewModel.ReturnMsg;
        //                activitylog.ErrorCode = activityRegisterViewModel.ErrorCode;
        //                activitylog.StatusCode = activityRegisterViewModel.StatusCode;
        //                activitylog.Id = activityRegisterViewModel.Id;
        //                activitylog.UpdatedBy = 0;
        //                activitylog.UpdatedDate = DateTime.UtcNow;
        //                _IActivityRepository.Update(activitylog);

        //                activitydetlog.ActivityId = activitylog.Id;
        //                activitydetlog.Response = activityRegisterDetViewModel.Response;
        //                _dbContext.ActivityRegisterDetail.Update(activitydetlog);
        //                _dbContext.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        throw;
        //    }
        //}

        public void UpdateActivityLogAsync(ActivityRes activityReqRes)
        {
            try
            {
                //var activitylog = (dynamic)null;
                var activitydetlog = (dynamic)null;
                if (activitydetlog == null)
                {
                    DateTime tTime = Helpers.UTC_To_IST().Add(new TimeSpan(0, 0, 0, 5, 0));//loop for 5 second only
                    while (Helpers.UTC_To_IST() < tTime)
                    {
                        Task.Delay(300);
                        //activitylog = _IActivityRepository.Table.Where(i => i.Id == activityReqRes.Id).FirstOrDefault();
                        activitydetlog = _dbContext.ActivityRegisterDetail.Where(i => i.ActivityId == activityReqRes.Id).FirstOrDefault();
                        if (activitydetlog == null)
                            continue;//still wait
                        else
                            break;//exit loop
                    }
                }

                if (activityReqRes != null)
                {
                    var activitylog = _IActivityRepository.Table.Where(i => i.Id == activityReqRes.Id).FirstOrDefault();
                    if (activitydetlog == null)
                        activitydetlog = _dbContext.ActivityRegisterDetail.Where(i => i.ActivityId == activityReqRes.Id).FirstOrDefault();
                    if (activitylog != null && activitydetlog != null)
                    {
                        activitylog.ReturnCode = activityReqRes.ReturnCode;
                        activitylog.ReturnMsg = activityReqRes.ReturnMsg;
                        activitylog.ErrorCode = activityReqRes.ErrorCode;
                        activitylog.StatusCode = activityReqRes.StatusCode;
                        activitylog.Id = activityReqRes.Id;
                        activitylog.UpdatedBy = 0;
                        activitylog.CreatedBy = activityReqRes.CreatedBy;
                        activitylog.UpdatedDate = Helpers.UTC_To_IST();

                        
                        _IActivityRepository.Update(activitylog); // update activity log with status code

                        activitydetlog.ActivityId = activitylog.Id;
                        activitydetlog.Response = activityReqRes.Response;
                        activitydetlog.Id = activitydetlog.Id;
                        _dbContext.ActivityRegisterDetail.Update(activitydetlog); // updated with response data in user wise.
                        _dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
    }
}
