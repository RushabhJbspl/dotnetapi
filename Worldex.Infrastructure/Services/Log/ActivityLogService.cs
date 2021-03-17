using Worldex.Core.Entities.Organization;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.AccountViewModels.Log;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Worldex.Infrastructure.Services.Log
{
    public class ActivityLogService : IActivityLog
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomRepository<Worldex.Core.Entities.Log.ActivityLog> _CustomLoginRepository;
        private readonly IActivityMasterConfiguration _IactivityMasterConfiguration;
        public ActivityLogService(ICustomRepository<Worldex.Core.Entities.Log.ActivityLog> customRepository, IActivityMasterConfiguration IactivityMasterConfiguration, WorldexContext dbContext)
        {
            _dbContext = dbContext;
            _CustomLoginRepository = customRepository;
            _IactivityMasterConfiguration = IactivityMasterConfiguration;
        }

        public long AddActivityLog(ActivityLogViewModel model)
        {
            try
            {
                ActivityType_Master ActivityTypeId = _IactivityMasterConfiguration.GetActivityTypeData().Where(i => model.HostName.ToLower().Contains(i.TypeMaster.ToLower()) && i.Status == true).FirstOrDefault();
                if (ActivityTypeId == null)
                {
                    return 0;
                }
                var ActivityLogHistory = new Worldex.Core.Entities.Log.ActivityLog
                {
                    Action = model.Action,
                    DeviceID = model.Device,
                    Mode = model.Mode,
                    IPAddress = model.IpAddress,
                    Location = model.Location,
                    HostName = model.HostName,
                    Status = 0,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = model.UserId,
                };
                _CustomLoginRepository.Insert(ActivityLogHistory);
                return ActivityLogHistory.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }

        }

        // Changed by khushali 03-05-2019 for optimization and clean up
        public ActivityLogResponse GetActivityLogHistoryByUserId(int UserId, int PageIndex, int Page_Size,string Device=null,string Mode=null,string Location=null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                //if (PageIndex == 0)
                //{
                //    PageIndex = 1;
                //}
                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }
                var skip = Page_Size * (PageIndex);
                var items = (from rs in _dbContext.ActivityLog
                             join us in _dbContext.Users on rs.UserId equals us.Id
                             where rs.UserId.Equals(UserId)
                             select new ActivityLogListViewModel
                             {
                                 Action = rs.Action,
                                 Device = rs.DeviceID,
                                 Mode = rs.Mode,
                                 IpAddress = rs.IPAddress,
                                 Location = rs.Location,
                                 HostName = rs.HostName,
                                 Date = rs.CreatedDate,
                                 Id = rs.Id ,
                                 UserId = rs.UserId,
                                 UserName=us.UserName
                             }
                           ).ToList();
                if (!string.IsNullOrEmpty(Device))
                {
                    items = items.Where(x => x.Device == Device).ToList();
                }
                if (!string.IsNullOrEmpty(Mode))
                {
                    items = items.Where(x => x.Mode.ToLower() == Mode.ToLower()).ToList();
                }
                if (!string.IsNullOrEmpty(Location))
                {
                    items = items.Where(x => x.Location.ToLower() == Location.ToLower()).ToList();
                }
                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.Date.Date >= FromDate && x.Date.Date <= ToDate).ToList();
                }

                int TotalCount = items.Count();

                ActivityLogResponse obj = new ActivityLogResponse();
                obj.TotalRow = TotalCount;
                obj.ActivityLogHistoryList = items.OrderByDescending(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;


                //string username = string.Empty;
                //var ActivityHistoryList = _CustomLoginRepository.Table.Where(i => i.UserId == UserId).ToList();
                //if (ActivityHistoryList == null)
                //{
                //    return null;
                //}

                //var ActivityLogHistory = new List<ActivityLogDataViewModel>();
                //foreach (var item in ActivityHistoryList)
                //{
                //    ActivityLogDataViewModel activityLogDataViewModel = new ActivityLogDataViewModel()
                //    {
                //        Action = item.Action,
                //        Device = item.DeviceID,
                //        Mode = item.Mode,
                //        IpAddress = item.IPAddress,
                //        Location = item.Location,
                //        HostName = item.HostName,
                //        Date = item.CreatedDate
                //    };
                //    ActivityLogHistory.Add(activityLogDataViewModel);
                //}

                //var total = ActivityLogHistory.Count();
                //if (pageIndex == 0)
                //{
                //    pageIndex = 1;
                //}
                //if (pageSize == 0)
                //{
                //    pageSize = 10;
                //}
                //var skip = pageSize * (pageIndex - 1);

                //return ActivityLogHistory.Skip(skip).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }


        public ActivityLogResponse GetActivityLogHistoryAdmin( int PageIndex = 0, int Page_Size = 0,string Action = null, string Mode = null, string IPAddress = null, string Location = null, string UserName = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }
                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);
              
                var items = (from rs in _dbContext.ActivityLog
                             join us in _dbContext.Users on rs.UserId equals us.Id
                             select new ActivityLogListViewModel
                             {
                                 Action = rs.Action,
                                 Device = rs.DeviceID,
                                 Mode = rs.Mode,
                                 IpAddress = rs.IPAddress,
                                 Location = rs.Location,
                                 HostName = rs.HostName,
                                 Date = rs.CreatedDate,
                                 Id = rs.Id,
                                 UserId = us.Id,
                                 CreatedDate = rs.CreatedDate,
                                 Status = rs.Status,
                                 UserName = us.UserName
                             }
                          ).ToList();

                

                if (Action != null)
                {
                    items = items.Where(x => x.Action == Action).ToList();
                    
                }
                if (Mode != null)
                {
                    items = items.Where(x => x.Mode == Mode).ToList();
                }
                if (IPAddress != null)
                {
                    items = items.Where(x => x.IpAddress == IPAddress).ToList();
                }
                if (Location != null)
                {
                    items = items.Where(x => x.Location == Location).ToList();
                }
                if (!String.IsNullOrEmpty(UserName))
                {
                    items = items.Where(x => x.UserName == UserName).ToList();
                }
                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }
                int TotalCount = items.Count();

                

                ActivityLogResponse obj = new ActivityLogResponse();
                obj.TotalRow = TotalCount;
                obj.ActivityLogHistoryList = items.OrderByDescending(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;

                //ActivityLogResponse obj = new ActivityLogResponse();
                //obj.TotalRow = TotalCount;
                //obj.ActivityLogHistoryList = items.ToList();
                //return obj;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }




        public long UpdateActivityLog(long Id, long UserId)
        {
            var GetUpdate = _CustomLoginRepository.Table.Where(i => i.Id == Id).FirstOrDefault();
            if (GetUpdate != null)
            {
                GetUpdate.UserId =Convert.ToInt32(UserId);               
                GetUpdate.UpdatedDate = DateTime.UtcNow;
                GetUpdate.UpdatedBy = UserId;
                _CustomLoginRepository.Update(GetUpdate);
                return GetUpdate.Id;
            }
            return 0;
        }
    }
}
