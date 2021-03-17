using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities
{
    public class ServiceTypeMaster : BizBase
    {
        [Required]
        public long ServiceTypeID { get; set; }

        [Required]
        [StringLength(60)]
        public string ServiceTypeName { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<ServiceTypeMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<ServiceTypeMaster>(this));
        }
    }
}
