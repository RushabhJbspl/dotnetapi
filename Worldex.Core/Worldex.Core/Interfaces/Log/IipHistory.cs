using Worldex.Core.ViewModels.AccountViewModels.Log;
using System;
using System.Threading.Tasks;
using Worldex.Core.Entities.Log;

namespace Worldex.Core.Interfaces.Log
{
    public interface IipHistory
    {
        long AddIpHistory(IpHistoryViewModel model);
        IpHistoryResponse GetIpHistoryListByUserId(long UserId, int pageIndex, int pageSize, string IPAddress = null, DateTime? FromDate = null, DateTime? ToDate = null);
        bool IsIpHistoryExist(int UserId,string IPAddress);
        bool IsIpHistoryExistV1(int UserId, string IPAddress);
        bool IsIPWhiteListed(long UserId, string IPAddress);
        Task<long> AddIPAddressProcess(int UserId, string IPAddress, Guid AuthorizeToken);
        //Task<IpMaster> GetIPMasterDetailsByGuid(string tokenID);
        Task<bool> UpdateIPAddressAuthorization(long ID, int UserId, short Status);
    }
}
