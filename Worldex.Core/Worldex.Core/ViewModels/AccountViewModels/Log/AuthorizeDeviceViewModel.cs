using Worldex.Core.ApiModels;
using System;

namespace Worldex.Core.ViewModels.AccountViewModels.Log
{
    public class AuthorizeDeviceViewModel
    {
        string device;
        string deviceOS;
        string deviceId;
        public int UserId { get; set; }
        public string Location { get; set; }
        public string IPAddress { get; set; }
        public DateTime CurrentTime { get; set; }
        public DateTime Expirytime { get; set; }
       
        public string DeviceName
        {
            get { return device; }
            set { device = value.ToUpper(); }
        }
       
        public string DeviceOS
        {
            get { return deviceOS; }
            set { deviceOS = value.ToUpper(); }
        }
       
        public string DeviceId
        {
            get { return deviceId; }
            set { deviceId = value.ToUpper(); }
        }
    }

    public class AuthorizeDeviceResponse : BizResponseClass
    {
        
    }

    public class AuthorizeDeviceData 
    {
        public string Location { get; set; }
        public string IPAddress { get; set; }
        public string DeviceName { get; set; }
    }

    public class AuthorizeDeviceDataResponse : BizResponseClass
    {
        public AuthorizeDeviceData AuthorizeData { get; set; }
    }

}
