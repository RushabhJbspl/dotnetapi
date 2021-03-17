using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Log
{
    public class SignUpLog : BizBase
    {
        public int TempUserId { get; set; }

        public int UserId { get; set; }

        public int RegisterType { get; set; }

        [StringLength(2000)]
        public string DeviceID { get; set; }

        [StringLength(10)]
        public string Mode { get; set; }

        
        [StringLength(15)]
        public string IPAddress { get; set; }


        [StringLength(2000)]
        public string Location { get; set; }

        [StringLength(250)]
        public string HostName { get; set; }

        public bool RegisterStatus { get; set; }

        public void SetAsUpdateDate(int userid)
        {
            UpdatedDate = DateTime.UtcNow;
            UpdatedBy = userid;
            RegisterStatus = true;
            UserId = userid;
            Events.Add(new ServiceStatusEvent<SignUpLog>(this));
        }

    }
}
