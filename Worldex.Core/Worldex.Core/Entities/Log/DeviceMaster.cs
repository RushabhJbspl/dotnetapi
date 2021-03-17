using System;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Log
{
    public class DeviceMaster : BizBase
    {
        string device;
        string deviceOS;
        string deviceId;

        public int UserId { get; set; }
        
        [StringLength(250)]
        public string Device
        {
            get { return device; }
            set { device = value.ToUpper(); }
        }

        [Required]
        [StringLength(250)]
        public string DeviceOS
        {
            get { return deviceOS; }
            set { deviceOS = value.ToUpper(); }
        }

        [StringLength(250)]
        public string DeviceId
        {
            get { return deviceId; }
            set { deviceId = value.ToUpper(); }
        }

        public bool IsEnable { get; set; }
        public bool IsDeleted { get; set; }

        //komal 18-06-2019 for validate device authorize
        public Guid Guid { get; set; }        
        public DateTime ExpiryTime { get; set; }

        // Removed IP and Location from here. We will be maintain it in seperate table to avoid duplication. -Nishit Jani on A-H 2019-07-06 3:17 AM
        // Uncommented temporary to solve merge issue in staging. -Nishit Jani on A 2019-07-08 12:52 PM
        public string Location { get; set; }
        public string IPAddress { get; set; }

        public void SetAsIpDeletetatus()
        {
            IsDeleted = true;
            Status = 0;
            Events.Add(new ServiceStatusEvent<DeviceMaster>(this));
        }

        public void SetAsIsDisabletatus()
        {
            IsEnable = false;
            Status = 0;
            Events.Add(new ServiceStatusEvent<DeviceMaster>(this));
        }

        public void SetAsIsEnabletatus()
        {
            IsEnable = true;
            Status = 1;
            Events.Add(new ServiceStatusEvent<DeviceMaster>(this));
        }
    }
}
