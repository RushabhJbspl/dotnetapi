using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using Worldex.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System;

namespace Worldex.Core.Entities.Transaction
{
    public class ProductConfiguration : BizBase
    {
        [Required]
        [StringLength(30)]
        public string ProductName { get; set; }
        [Required]
        public long ServiceID { get; set; } // ntrivedi added 03-11-2018
        [Required]
        public long CountryID { get; set; }   

        public void SetActiveProduct()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<ProductConfiguration>(this));
        }
        public void SetInActiveProduct()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            Events.Add(new ServiceStatusEvent<ProductConfiguration>(this));
        }
    }

    //Darshan Dholakiya added this entity for the arbitrage service related changes:10-06-2019
    public class ProductConfigurationArbitrage : BizBase
    {
        [Required]
        [StringLength(30)]
        public string ProductName { get; set; }
        [Required]
        public long ServiceID { get; set; } // ntrivedi added 03-11-2018

        [Required]
        public long CountryID { get; set; }

        public void SetActiveProduct()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<ProductConfigurationArbitrage>(this));
        }
        public void SetInActiveProduct()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            Events.Add(new ServiceStatusEvent<ProductConfigurationArbitrage>(this));
        }
    }
}
