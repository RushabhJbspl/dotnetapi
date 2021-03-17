using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
    public class DemonConfiguration : BizBase
    {
        [Required]
        [StringLength(15)]
        public String IPAdd { get; set; }

        [Required]
        public int PortAdd { get; set; }

        [Required]
        [StringLength(200)]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        public void DisableConfiguration()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceProviderEvent<DemonConfiguration>(this));
        }
        public void EnableConfiguration()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceProviderEvent<DemonConfiguration>(this));
        }
    }

    //Darshan Dholakiya added this entity for the Arbitrage provider method related changes:17-06-2019
    public class DemonConfigurationArbitrage : BizBase
    {
        [Required]
        [StringLength(15)]
        public String IPAdd { get; set; }

        [Required]
        public int PortAdd { get; set; }

        [Required]
        [StringLength(200)]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        public void DisableConfiguration()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceProviderEvent<DemonConfigurationArbitrage>(this));
        }
        public void EnableConfiguration()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceProviderEvent<DemonConfigurationArbitrage>(this));
        }
    }
}
