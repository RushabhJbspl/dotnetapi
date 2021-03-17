using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
    public class AppType :BizBase
    {
        [Required]
        [StringLength(20)]
        public string AppTypeName { get; set; }
        
        public int AppTypeID { get; set; }

        public void DisableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<AppType>(this));
        }

        public void EnableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<AppType>(this));
        }
    }
}
