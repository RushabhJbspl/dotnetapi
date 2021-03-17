using System;
using System.Threading.Tasks;
using Worldex.Core.ViewModels.AccountViewModels.Log;

namespace Worldex.Core.Interfaces.Log
{
    public interface IipAddressService
    {
        Task<long> AddIpAddress(IpMasterViewModel model);
        Task<IpMasterViewModel> GetIpAddressById(long Id);
        Task<IpMasterGetResponse> GetIpAddressListByUserId(long UserId, int pageIndex, int pageSize, string IPAddress = null, DateTime? FromDate = null, DateTime? ToDate = null);
        Task<long> UpdateIpAddress(IpMasterViewModel model);
        Task<long> DesableIpAddress(IpMasterViewModel model);
        Task<long> EnableIpAddress(IpMasterViewModel model);
        Task<long> DeleteIpAddress(IpMasterViewModel model);
        Task<long> GetIpAddressByUserIdandAddress(string IpAddress, long UserId);
        bool CheckValidIpaddress(int UserId,string Ipaddress );

        Task<long> AddIpAddressV1(IpMasterViewModel model);
        bool CheckIPExist(long UserId, string IP);
    }
}
