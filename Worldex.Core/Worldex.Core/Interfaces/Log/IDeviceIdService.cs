using System;
using Worldex.Core.ViewModels.AccountViewModels.Log;

namespace Worldex.Core.Interfaces.Log
{
    public interface IDeviceIdService
    {
        long AddDeviceId(DeviceMasterViewModel model);
        DeviceIdResponse GetDeviceListByUserId(long UserId, int pageIndex, int pageSize, string device = null, DateTime? FromDate = null, DateTime? ToDate = null);
        DeviceIdResponse GetDeviceDataForAdmin(int PageIndex = 0, int Page_Size = 0, string UserName = null, string DeviceId = null, string Device = null, string DeviceOs = null, DateTime? FromDate = null, DateTime? ToDate = null);
        void UpdateDeviceId(DeviceMasterViewModel model);
        long DesableDeviceId(DeviceMasterViewModel model);
        long EnableDeviceId(DeviceMasterViewModel model);
        long DeleteDeviceId(DeviceMasterViewModel model);
        long GetDeviceByUserIdandId(string DeviceId, long UserId);
        long AddDeviceProcess(DeviceMasterViewModel model);
        bool CheckValidDevice(int Userid, string Device);

        long DisableDeviceIdByAdmin(DeviceMasterViewModel model, int UserId);
        long EnableDeviceIdByAdmin(DeviceMasterViewModel model,int UserId);
        long DeleteDeviceIdByAdmin(DeviceMasterViewModel model,int UserId);

        long AddDeviceProcessV1(DeviceMasterViewModelV1 model);
        DeviceMasterViewModelV1 GetDeviceDetailsByGuid(string Guid);
        bool UpdateDevaiceAuthorize(long id, long UserID, short Status);
        bool CheckValidDeviceV1(int Userid, string Device, string IP);
        DeviceMasterViewModelV1 CheckValidDeviceV2(int Userid, string Device, string IP);
        void UpdateDevaiceValidation(DeviceMasterViewModelV1 deviceData);
    }
}
