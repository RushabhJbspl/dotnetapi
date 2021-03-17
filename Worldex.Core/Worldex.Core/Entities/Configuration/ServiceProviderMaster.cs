using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.Enums;

namespace Worldex.Core.Entities.Configuration
{
    public class ServiceProviderMaster :BizBase
    {
        [Required]
        [StringLength(60)]
        public string ProviderName { get; set; }

        public void DisableProvider()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceProviderEvent<ServiceProviderMaster>(this));
        }

        public void EnableProvider()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceProviderEvent<ServiceProviderMaster>(this));
        }
    }

    //khushali 04-06-2019 for Arbitrage trading 
    public class ServiceProviderMasterArbitrage : BizBase
    {
        [Required]
        [StringLength(60)]
        public string ProviderName { get; set; }

        public void DisableProvider()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceProviderEvent<ServiceProviderMasterArbitrage>(this));
        }

        public void EnableProvider()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceProviderEvent<ServiceProviderMasterArbitrage>(this));
        }
    }
}
