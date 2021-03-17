using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
    public class ServiceProviderType : BizBase
    {
        [Required]
        [StringLength(20)]
        public string ServiveProTypeName { get; set; }

        public void DisableProviderType()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<ServiceProviderType>(this));
        }
        public void EnableProviderType()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<ServiceProviderType>(this));
        }
    }

    //khushali 04-06-2019 for Arbitrage trading 
    public class ServiceProviderTypeArbitrage : BizBase
    {
        [Required]
        [StringLength(20)]
        public string ServiveProTypeName { get; set; }

        public void DisableProviderType()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<ServiceProviderTypeArbitrage>(this));
        }
        public void EnableProviderType()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<ServiceProviderTypeArbitrage>(this));
        }
    }
}
