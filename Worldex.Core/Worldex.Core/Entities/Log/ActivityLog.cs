using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Log
{
    public class ActivityLog : BizBase
    {
        public int UserId { get; set; }

        [StringLength(250)]
        public string Action { get; set; }

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
    }
}
