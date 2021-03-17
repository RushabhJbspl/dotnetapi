using Worldex.Core.Entities.Log;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.AccountViewModels.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.Log
{
    public class LoginHistoryService : ILoginHistory
    {
        private readonly ICustomRepository<LoginHistory> _CustomLoginRepository;
        public LoginHistoryService(ICustomRepository<LoginHistory> customRepository)
        {
            _CustomLoginRepository = customRepository;
        }

        // Changed by khushali 03-05-2019 for optimization and clean up
        public LoginHistoryResponse GetLoginHistoryByUserId(long UserId, int pageIndex, int pageSize, string IPAddress = null,string Device=null,string Location=null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                var LoginHistoryList = _CustomLoginRepository.Table.Where(i => i.UserId == UserId).OrderByDescending(i =>i.CreatedDate).ToList();
                if (LoginHistoryList == null)
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(IPAddress))
                {
                    LoginHistoryList = LoginHistoryList.Where(x => x.IpAddress == IPAddress).ToList();
                }
                if (!string.IsNullOrEmpty(Device))
                {
                    LoginHistoryList = LoginHistoryList.Where(x => x.Device == Device).ToList();
                }
                if (!string.IsNullOrEmpty(Location))
                {
                    LoginHistoryList = LoginHistoryList.Where(x => x.Location == Location).ToList();
                }
                if (FromDate != null && ToDate != null)
                {
                    LoginHistoryList = LoginHistoryList.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }                

                var total = LoginHistoryList.Count();
                //var pageSize = 10; // set your page size, which is number of records per page

                //var page = 1; // set current page number, must be >= 1
                //if (pageIndex == 0)
                //{
                //    pageIndex = 1;
                //}

                if (pageSize == 0)
                {
                    pageSize = 10;
                }

                var skip = pageSize * (pageIndex);

                //var canPage = skip < total;

                //if (canPage) // do what you wish if you can page no further
                //    return null;
                LoginHistoryList = LoginHistoryList.Skip(skip).Take(pageSize).ToList(); // khuhsali 03-05-2019 optimization

                var LoginHistory = new List<LoginHistoryDataViewModel>();
                foreach (var item in LoginHistoryList)
                {
                    LoginHistoryDataViewModel loginhistoryViewModel = new LoginHistoryDataViewModel()
                    {
                        IpAddress = item.IpAddress,
                        Device = item.Device,
                        Location = item.Location,
                        Date = item.CreatedDate
                    };
                    LoginHistory.Add(loginhistoryViewModel);
                }

                LoginHistoryResponse loginHistory = new LoginHistoryResponse()
                {
                    //LoginHistoryList = LoginHistory.Skip(skip).Take(pageSize).ToList(),
                    LoginHistoryList = LoginHistory,
                    TotalCount = total
                };
                return loginHistory;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }

        }

        public long AddLoginHistory(LoginhistoryViewModel model)
        {
            try
            {
                var LoginHistory = new LoginHistory()
                {
                    UserId = model.UserId,
                    IpAddress = model.IpAddress,
                    Device = model.Device,
                    Location = model.Location,
                    Status = 0,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = model.UserId
                };
                _CustomLoginRepository.Insert(LoginHistory);
                return LoginHistory.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }

        }
    }
}
