using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldex.Core.Entities.Log;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.AccountViewModels.Log;
using Newtonsoft.Json;

namespace Worldex.Infrastructure.Services.Log
{
    public class DeviceIdService : IDeviceIdService
    {
        private readonly ICustomRepository<DeviceMaster> _deviceMasterRepository;
        private readonly ICommonRepository<DeviceMaster> _deviceMasterCommonRepository;

        private readonly WorldexContext _dbContext;
        public DeviceIdService(ICustomRepository<DeviceMaster> deviceMasterRepository, WorldexContext dbContext, ICommonRepository<DeviceMaster> deviceMasterCommonRepository)
        {
            _deviceMasterRepository = deviceMasterRepository;
            _dbContext = dbContext;
            _deviceMasterCommonRepository = deviceMasterCommonRepository;
        }
        public long AddDeviceId(DeviceMasterViewModel model)
        {
            var getDeviceId = _deviceMasterRepository.Table.FirstOrDefault(i => i.DeviceId.ToUpper() == model.DeviceId.ToUpper() && i.UserId == model.UserId && !i.IsDeleted);
            if (getDeviceId != null)
            {
                return getDeviceId.Id;
            }

            var currentDeviceId = new DeviceMaster
            {
                UserId = model.UserId,
                DeviceId = model.DeviceId.ToUpper(),
                IsEnable = true,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = model.UserId,
                Status = 0,

            };
            _deviceMasterRepository.Insert(currentDeviceId);
            //_dbContext.SaveChanges();

            return currentDeviceId.Id;
        }

        public long GetDeviceByUserIdandId(string DeviceId, long UserId)
        {
            var getDeviceId = _deviceMasterRepository.Table.FirstOrDefault(i => i.DeviceId.ToUpper() == DeviceId.ToUpper() && i.UserId == UserId && !i.IsDeleted);
            if (getDeviceId != null)
            {
                return getDeviceId.Id;
            }

            return 0;
        }

        // Changed by khushali 03-05-2019 for optimization and clean up
        public DeviceIdResponse GetDeviceListByUserId(long UserId, int pageIndex, int pageSize, string device = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {

                var DeviceIdList = _deviceMasterRepository.Table.Where(i => i.UserId == UserId && !i.IsDeleted).OrderByDescending(i => i.CreatedDate).ToList();
                if (DeviceIdList == null)
                {
                    return null;
                }


                // return IpList;

                if (!string.IsNullOrEmpty(device))
                {
                    DeviceIdList = DeviceIdList.Where(x => x.Device == device).ToList();
                }
                if (FromDate != null && ToDate != null)
                {
                    DeviceIdList = DeviceIdList.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }

                var total = DeviceIdList.Count();
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

                DeviceIdList = DeviceIdList.Skip(skip).Take(pageSize).ToList();
                var DeviceList = new List<GetDeviceData>();
                foreach (var item in DeviceIdList)
                {
                    GetDeviceData imodel = new GetDeviceData();
                    imodel.Id = item.Id;
                    imodel.UserId = item.UserId;
                    imodel.Device = item.Device.ToUpper();
                    imodel.DeviceOS = item.DeviceOS.ToUpper();
                    imodel.DeviceId = item.DeviceId.ToUpper();
                    imodel.IsEnable = item.IsEnable;
                    imodel.IsDeleted = item.IsDeleted;
                    imodel.CreatedDate = item.CreatedDate;
                    //imodel.CreatedBy = item.CreatedBy;
                    imodel.UpdatedDate = item.UpdatedDate;
                    //imodel.UpdatedBy = item.UpdatedBy;
                    //imodel.Status = item.Status;

                    DeviceList.Add(imodel);
                }
                DeviceIdResponse DeviceIdget = new DeviceIdResponse()
                {
                    DeviceList = DeviceList,
                    TotalCount = total
                };

                return DeviceIdget;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public DeviceIdResponse GetDeviceDataForAdmin(int PageIndex = 0, int Page_Size = 0, string UserName = null, string DeviceId = null, string Device = null, string DeviceOs = null, DateTime? FromDate = null, DateTime? ToDate = null)
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
                var items = (from rs in _dbContext.DeviceMaster
                             join us in _dbContext.Users on rs.UserId equals us.Id
                             select new GetDeviceData
                             {
                                 Id = rs.Id,
                                 UserId = us.Id,
                                 UserName = us.UserName,
                                 DeviceId = rs.DeviceId,
                                 Device = rs.Device,
                                 DeviceOS = rs.DeviceOS,
                                 IsDeleted = rs.IsDeleted,
                                 IsEnable = rs.IsEnable,
                                 CreatedDate = rs.CreatedDate,
                                 UpdatedDate = rs.UpdatedDate

                             }
                            ).ToList();

                if (!String.IsNullOrEmpty(UserName))
                {
                    items = items.Where(x => x.UserName == UserName).ToList();
                }
                if (!String.IsNullOrEmpty(DeviceId))
                {
                    items = items.Where(x => x.DeviceId.ToUpper() == DeviceId.ToUpper()).ToList();
                }
                if (!String.IsNullOrEmpty(Device))
                {
                    items = items.Where(x => x.Device.ToUpper() == Device.ToUpper()).ToList();
                }
                if (!String.IsNullOrEmpty(DeviceOs))
                {
                    items = items.Where(x => x.DeviceOS.ToUpper() == DeviceOs.ToUpper()).ToList();
                }
                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }
                int TotalCount = items.Count();

                DeviceIdResponse obj = new DeviceIdResponse();
                obj.TotalCount = TotalCount;
                obj.DeviceList = items.OrderByDescending(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }

        }
        public void UpdateDeviceId(DeviceMasterViewModel model)
        {
            var DeviceIddata = _deviceMasterRepository.Table.FirstOrDefault(i => i.DeviceId.ToUpper() == model.DeviceId.ToUpper() && i.UserId == model.UserId && !i.IsDeleted);
            if (DeviceIddata != null)
            {
                var currentDeviceId = new DeviceMaster
                {
                    Id = DeviceIddata.Id,
                    UserId = DeviceIddata.UserId,
                    DeviceId = model.DeviceId.ToUpper(),
                    IsEnable = DeviceIddata.IsEnable,
                    //IsDeleted = IpAddress.IsDeleted,                    
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedBy = DeviceIddata.UserId
                };

                _deviceMasterRepository.Update(currentDeviceId);
                //_dbContext.SaveChanges();

            }
        }

        public long DesableDeviceId(DeviceMasterViewModel model)
        {
            var Devicedata = _deviceMasterRepository.Table.FirstOrDefault(i => i.Id == model.Id && i.UserId == model.UserId && !i.IsDeleted);
            if (Devicedata != null)
            {
                // False Status
                Devicedata.SetAsIsDisabletatus();
                _deviceMasterRepository.Update(Devicedata);
                //_dbContext.SaveChanges();
                return Devicedata.Id;
            }
            return 0;
        }

        public long EnableDeviceId(DeviceMasterViewModel model)
        {
            var Devicedata = _deviceMasterRepository.Table.FirstOrDefault(i => i.Id == model.Id && i.UserId == model.UserId && !i.IsEnable && !i.IsDeleted);

            if (Devicedata != null)
            {
                // True Status
                Devicedata.SetAsIsEnabletatus();
                _deviceMasterRepository.Update(Devicedata);
                //_dbContext.SaveChanges();
                return Devicedata.Id;
            }
            return 0;
        }

        public long DeleteDeviceId(DeviceMasterViewModel model)
        {
            var DeviceData = _deviceMasterRepository.Table.FirstOrDefault(i => i.Id == model.Id && i.UserId == model.UserId && !i.IsDeleted);
            if (DeviceData != null)
            {
                //var currentIpAddress = new IpMaster
                //{
                //    Id = IpAddress.Id,
                //    UserId = IpAddress.UserId,
                //    //IpAddress = IpAddress.IpAddress,
                //    //IsEnable = IpAddress.IsEnable,
                //    IsDeleted = true,
                //    UpdatedDate = DateTime.UtcNow,
                //    UpdatedBy = IpAddress.UserId
                //};
                DeviceData.SetAsIpDeletetatus();
                _deviceMasterRepository.Update(DeviceData);
                //_dbContext.SaveChanges();
                return DeviceData.Id;
            }
            return 0;
        }


        public long DisableDeviceIdByAdmin(DeviceMasterViewModel model, int UserId)
        {
            var Devicedata = _deviceMasterRepository.Table.FirstOrDefault(i => i.Id == model.Id);
            if (Devicedata != null)
            {

                Devicedata.SetAsIsDisabletatus();
                Devicedata.UpdatedBy = UserId;
                Devicedata.UpdatedDate = DateTime.UtcNow;
                _deviceMasterRepository.Update(Devicedata);

                return Devicedata.Id;
            }
            return 0;
        }

        public long EnableDeviceIdByAdmin(DeviceMasterViewModel model, int UserId)
        {
            var Devicedata = _deviceMasterRepository.Table.FirstOrDefault(i => i.Id == model.Id && !i.IsEnable && !i.IsDeleted);

            if (Devicedata != null)
            {
                // True Status
                Devicedata.SetAsIsEnabletatus();
                Devicedata.UpdatedBy = UserId;
                Devicedata.UpdatedDate = DateTime.UtcNow;
                _deviceMasterRepository.Update(Devicedata);
                //_dbContext.SaveChanges();
                return Devicedata.Id;
            }
            return 0;
        }

        public long DeleteDeviceIdByAdmin(DeviceMasterViewModel model, int UserId)
        {
            var DeviceData = _deviceMasterRepository.Table.FirstOrDefault(i => i.Id == model.Id && !i.IsDeleted);
            if (DeviceData != null)
            {
                //var currentIpAddress = new IpMaster
                //{
                //    Id = IpAddress.Id,
                //    UserId = IpAddress.UserId,
                //    //IpAddress = IpAddress.IpAddress,
                //    //IsEnable = IpAddress.IsEnable,
                //    IsDeleted = true,
                //    UpdatedDate = DateTime.UtcNow,
                //    UpdatedBy = IpAddress.UserId
                //};
                DeviceData.SetAsIpDeletetatus();
                DeviceData.UpdatedBy = UserId;
                DeviceData.UpdatedDate = DateTime.UtcNow;
                _deviceMasterRepository.Update(DeviceData);
                //_dbContext.SaveChanges();
                return DeviceData.Id;
            }
            return 0;
        }


        public long AddDeviceProcess(DeviceMasterViewModel model)
        {
            try
            {
                var getDeviceId = _deviceMasterRepository.Table.FirstOrDefault(i => i.DeviceId.ToUpper() == model.DeviceId.ToUpper() && i.Device.ToUpper() == model.Device.ToUpper() && i.DeviceOS.ToUpper() == model.DeviceOS.ToUpper() && i.UserId == model.UserId && !i.IsDeleted);
                if (getDeviceId != null)
                {
                    if (!getDeviceId.IsEnable)
                    {
                        getDeviceId.IsEnable = true;
                        _deviceMasterRepository.Update(getDeviceId);
                    }
                    return getDeviceId.Id;
                }

                var currentDeviceId = new DeviceMaster
                {
                    UserId = model.UserId,
                    DeviceId = model.DeviceId.ToUpper(),
                    Device = model.Device.ToUpper(),
                    DeviceOS = model.DeviceOS.ToUpper(),
                    IsEnable = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = model.UserId,
                    Status = 0,
                };
                _deviceMasterRepository.Insert(currentDeviceId);
                return currentDeviceId.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public bool CheckValidDevice(int Userid, string Device)
        {
            try
            {
                string[] DeviceDetails = null;
                if (Device.Contains("|"))
                    DeviceDetails = Device.Split('|');
                //&& i.Status==1
                if (DeviceDetails != null)
                {
                    string DeviceName = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                    string DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                    string DeviceID = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                    var CheckValidDevice = _deviceMasterRepository.Table.Where(i => i.UserId == Userid && i.Device.ToUpper() == DeviceName.ToUpper() && i.DeviceOS.ToUpper() == DeviceOS.ToUpper() && i.DeviceId.ToUpper() == DeviceID.ToUpper() && i.IsEnable == true && !i.IsDeleted && i.Status == 1).FirstOrDefault();
                    if (CheckValidDevice != null)
                        return true;
                    else
                        return false;
                }
                else
                {
                    var CheckValidDeviceDet = _deviceMasterRepository.Table.Where(i => i.UserId == Userid && i.Device.ToUpper() == string.Empty && i.DeviceOS.ToUpper() == string.Empty && i.DeviceId.ToUpper() == string.Empty && i.IsEnable == true && !i.IsDeleted).FirstOrDefault();
                    if (CheckValidDeviceDet != null)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        public long AddDeviceProcessV1(DeviceMasterViewModelV1 model)
        {
            try
            {
                var currentDeviceId = new DeviceMaster
                {
                    UserId = model.UserId,
                    DeviceId = model.DeviceId.ToUpper(),
                    Device = model.Device.ToUpper(),
                    DeviceOS = model.DeviceOS.ToUpper(),
                    IsEnable = false,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = model.UserId,
                    Status = model.Status,
                    Guid = model.AuthorizeToken,
                    ExpiryTime = model.Expirytime,
                    IPAddress = model.IPAddress,
                    Location = model.Location
                };
                _deviceMasterRepository.Insert(currentDeviceId);
                return currentDeviceId.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public DeviceMasterViewModelV1 GetDeviceDetailsByGuid(string Guid)
        {
            DeviceMasterViewModelV1 _Res = new DeviceMasterViewModelV1();
            try
            {
                var model = _deviceMasterCommonRepository.FindBy(e => e.Guid == new Guid(Guid) && e.Status == 0 && e.IsDeleted == false).FirstOrDefault();
                if (model != null)
                {
                    _Res.AuthorizeToken = model.Guid;
                    _Res.Device = model.Device;
                    _Res.DeviceId = model.DeviceId;
                    _Res.DeviceOS = model.DeviceOS;
                    _Res.Expirytime = model.ExpiryTime;
                    _Res.IPAddress = model.IPAddress;
                    _Res.Location = model.Location;
                    _Res.UserId = model.UserId;
                    _Res.ID = model.Id;
                    return _Res;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public bool UpdateDevaiceAuthorize(long id, long UserID, short Status)
        {
            try
            {
                var model = _deviceMasterCommonRepository.FindBy(e => e.Id == id).FirstOrDefault();
                if (model == null)
                    return false;
                model.Status = Status;
                model.IsEnable = Status == 9 ? false : true;
                model.UpdatedBy = UserID;
                model.UpdatedDate = DateTime.UtcNow;
                _deviceMasterCommonRepository.Update(model);
                return true;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        public void UpdateDevaiceValidation(DeviceMasterViewModelV1 deviceData)
        {
            try
            {
                var model = _deviceMasterCommonRepository.FindBy(e => e.Id == deviceData.ID).FirstOrDefault();                
                model.Guid = deviceData.Guid;
                model.ExpiryTime = deviceData.Expirytime;
                model.UpdatedDate = DateTime.UtcNow;
                _deviceMasterCommonRepository.Update(model);                
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);                
            }
        }

        public bool CheckValidDeviceV1(int Userid, string Device, string IP)
        {
            try
            {
                string[] DeviceDetails = null;
                if (Device.Contains("|"))
                    DeviceDetails = Device.Split('|');
                if (DeviceDetails != null)
                {
                    string DeviceName = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                    string DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                    string DeviceID = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                    //Rushabh 28-04-2020 Remove IP Address Condition As It Will Be Checked Separately.
                    //var CheckValidDevice = _deviceMasterRepository.Table.Where(i => i.UserId == Userid && i.Device.ToUpper() == DeviceName.ToUpper() && i.DeviceOS.ToUpper() == DeviceOS.ToUpper() && i.DeviceId.ToUpper() == DeviceID.ToUpper() && i.IsEnable == true && !i.IsDeleted && i.Status == 1).FirstOrDefault();
                    var CheckValidDevice = _deviceMasterRepository.Table.Where(i => i.UserId == Userid && i.Device.ToUpper() == DeviceName.ToUpper() && i.DeviceOS.ToUpper() == DeviceOS.ToUpper() && i.DeviceId.ToUpper() == DeviceID.ToUpper() && i.IsEnable == true && !i.IsDeleted && i.Status == 1 && i.IPAddress == IP).FirstOrDefault();
                    if (CheckValidDevice != null)
                        return true;
                    else
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        public DeviceMasterViewModelV1 CheckValidDeviceV2(int Userid, string Device, string IP)
        {
            DeviceMasterViewModelV1 deviceData = new DeviceMasterViewModelV1();
            try
            {
                string[] DeviceDetails = null;
                if (Device.Contains("|"))
                    DeviceDetails = Device.Split('|');
                if (DeviceDetails != null)
                {
                    string DeviceName = DeviceDetails != null ? DeviceDetails[0].ToUpper() : string.Empty;
                    string DeviceOS = DeviceDetails != null ? DeviceDetails[1].ToUpper() : string.Empty;
                    string DeviceID = DeviceDetails != null ? DeviceDetails[2].ToUpper() : string.Empty;
                    var CheckValidDevice = _deviceMasterRepository.Table.Where(i => i.UserId == Userid && i.Device == DeviceName && i.DeviceOS == DeviceOS && i.DeviceId == DeviceID).FirstOrDefault();
                    if (CheckValidDevice != null)
                        deviceData = JsonConvert.DeserializeObject<DeviceMasterViewModelV1>(JsonConvert.SerializeObject(CheckValidDevice));

                }
                return deviceData;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return deviceData;
            }
        }
    }
}
