using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;

namespace Worldex.Core.Entities.Configuration
{
    public class SiteTokenRateType : BizBase
    {
        public string TokenType { get; set; }

        public void DisableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<SiteTokenRateType>(this));
        }
        public void EnableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<SiteTokenRateType>(this));
        }
    }
}
