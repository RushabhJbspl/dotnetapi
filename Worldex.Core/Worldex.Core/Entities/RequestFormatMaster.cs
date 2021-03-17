using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities
{
    public  class RequestFormatMaster : BizBase
    {
        [Required]
        [StringLength(60)]        
        public string RequestName { get; set; }

        [Required]
        [StringLength(60)]
        public string ContentType { get; set; }

        [Required]
        [StringLength(20)]
        public string MethodType { get; set; }

        [Required]
        [StringLength(500)]
        public string RequestFormat { get; set; }

        [Required]
        public long RequestType { get; set; } // khushali -- 30-01-2019  check Apptype wise 

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<RequestFormatMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<RequestFormatMaster>(this));
        }
    }
}
