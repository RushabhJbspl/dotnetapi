using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
    public class OrderTypeMaster : BizBase
    {
        [Required]
        [StringLength(20)]
        public String OrderType { get; set; }

        public void DisableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<OrderTypeMaster>(this));
        }

        public void EnableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<OrderTypeMaster>(this));
        }
    }
}
