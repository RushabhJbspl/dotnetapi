using Worldex.Core.Entities.Log;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.AccountViewModels.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.Interfaces;

namespace Worldex.Infrastructure.Services.Log
{
    public class IpHistoryService : IipHistory
    {
        private readonly ICustomRepository<IpHistory> _ipHistoryRepository;
        private readonly ICommonRepository<IpMaster> _IPMasterRepository;
        private readonly WorldexContext _dbContext;

        public IpHistoryService(ICustomRepository<IpHistory> ipHistoryRepository, WorldexContext dbContext, ICommonRepository<IpMaster> IPMasterRepository)
        {
            _dbContext = dbContext;
            _IPMasterRepository = IPMasterRepository;
            _ipHistoryRepository = ipHistoryRepository;
        }

        public long AddIpHistory(IpHistoryViewModel model)
        {
            try
            {
                var IpHistory = new IpHistory()
                {
                    UserId = model.UserId,
                    IpAddress = model.IpAddress,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = model.UserId,
                    Status = 0,
                    Location = model.Location
                };
                _ipHistoryRepository.Insert(IpHistory);
                return IpHistory.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        // Changed by khushali 03-05-2019 for optimization and clean up
        public IpHistoryResponse GetIpHistoryListByUserId(long UserId, int pageIndex, int pageSize, string IPAddress = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                var IpHistoryList = _ipHistoryRepository.Table.Where(i => i.UserId == UserId).OrderByDescending(i => i.CreatedDate).ToList();
                if (IpHistoryList == null)
                {
                    return null;
                }

                
                //return IpList;
                if (!string.IsNullOrEmpty(IPAddress))
                {
                    IpHistoryList = IpHistoryList.Where(x => x.IpAddress == IPAddress).ToList();
                }
                if (FromDate != null && ToDate != null)
                {
                    IpHistoryList = IpHistoryList.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }

                var total = IpHistoryList.Count();
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
                IpHistoryList = IpHistoryList.Skip(skip).Take(pageSize).ToList();  // khuhsali 03-05-2019 optimization
                var IpList = new List<IpHistoryDataViewModel>();
                foreach (var item in IpHistoryList)
                {
                    IpHistoryDataViewModel model = new IpHistoryDataViewModel();
                    model.IpAddress = item.IpAddress;
                    model.Location = item.Location;
                    model.Date = item.CreatedDate;
                    IpList.Add(model);
                }

                IpHistoryResponse ipHistoryResponse = new IpHistoryResponse()
                {
                    IpHistoryList = IpList,
                    Totalcount= total
                };
                return ipHistoryResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public bool IsIpHistoryExist(int UserId,string IPAddress)
        {
            var responsedata = _dbContext.IpHistory.Any(x=>x.UserId== UserId && x.IpAddress== IPAddress);
            return responsedata;
        }

        public bool IsIpHistoryExistV1(int UserId, string IPAddress)
        {
            var responsedata = _dbContext.IpHistory.Any(x => x.UserId == UserId && x.IpAddress == IPAddress && x.Status==1);
            return responsedata;
        }

        //Rushabh 28-04-2020
        public bool IsIPWhiteListed(long UserId, string IPAddress)
        {
            try
            {
                var responsedata = _dbContext.IpMaster.Any(x => x.UserId == UserId && x.IpAddress == IPAddress && x.Status == 1 && x.IsEnable == true && x.IsDeleted == false);
                return responsedata;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("IsIPWhiteListed", "IpHistoryService", ex);
                return false;
            }
        }
        //Rushabh 28-04-2020
        public async Task<long> AddIPAddressProcess(int UserId, string IPAddress, Guid AuthorizeToken)
        {
            try
            {
                IpMaster NewObj = new IpMaster();
                //NewObj.GUID = AuthorizeToken;
                NewObj.IpAddress = IPAddress;
                NewObj.IpAliasName = "Auth";
                NewObj.IsEnable = false;
                NewObj.IsDeleted = false;
                NewObj.UserId = UserId;
                NewObj.Status = 0;
                NewObj.CreatedBy = UserId;
                NewObj.CreatedDate = Helpers.UTC_To_IST();
                NewObj = await _IPMasterRepository.AddAsync(NewObj);
                return NewObj.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddIPAddressProcess", "IpHistoryService", ex);
                return 0;
            }
        }
        //Rushabh 28-04-2020 Commented On 07-07-2020
        //public async Task<IpMaster> GetIPMasterDetailsByGuid(string tokenID)
        //{
        //    try
        //    {
        //        IpMaster NewObj = await _IPMasterRepository.GetSingleAsync(x => x.GUID == new Guid(tokenID) && x.Status == 0 && !x.IsEnable && !x.IsDeleted);
        //        return NewObj;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog("GetIPMasterDetailsByGuid", "IpHistoryService", ex);
        //        return null;
        //    }
        //}
        //Rushabh 28-04-2020
        public async Task<bool> UpdateIPAddressAuthorization(long ID, int UserId, short Status)
        {
            try
            {
                IpMaster IsExist = await _IPMasterRepository.GetSingleAsync(x => x.Id == ID);
                if (IsExist != null)
                {
                    IsExist.Status = Status;
                    IsExist.IsEnable = Status == 9 ? false : true;
                    IsExist.IsDeleted = Status == 9 ? true : false;
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    await _IPMasterRepository.UpdateAsync(IsExist);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateIPAddressAuthorization", "IpHistoryService", ex);
                return false;
            }
        }

    }
}
