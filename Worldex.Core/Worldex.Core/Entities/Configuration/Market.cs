using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Communication
{
    public class Market : BizBase
    {
        [Required]
        public string CurrencyName { get; set; }
        public short isBaseCurrency { get; set; }
        public long ServiceID { get; set; }

        public short Priority { get; set; }//rita 01-5-19 for manage list

        public void DisableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<Market>(this));
        }
        public void EnableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<Market>(this));
        }
    }
    public class MarketMargin : BizBase
    {
        [Required]
        public string CurrencyName { get; set; }
        public short isBaseCurrency { get; set; }
        public long ServiceID { get; set; }
        public short Priority { get; set; }//rita 01-5-19 for manage list

        public void DisableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<MarketMargin>(this));
        }
        public void EnableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<MarketMargin>(this));
        }
    }

    public class MarketArbitrage : BizBase
    {
        [Required]
        public string CurrencyName { get; set; }
        public short isBaseCurrency { get; set; }
        public long ServiceID { get; set; }
        public short Priority { get; set; }//rita 01-5-19 for manage list

        public void DisableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<MarketArbitrage>(this));
        }
        public void EnableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<MarketArbitrage>(this));
        }
    }
}
