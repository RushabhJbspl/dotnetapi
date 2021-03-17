using System;
using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities
{
    public class ThirdPartyAPIResponseConfiguration : BizBase
    {
        public string ParsingName { get; set; } // add By jagdish 12-02-2020
        public string BalanceRegex { get; set; }

        public string StatusRegex { get; set; }

        public string StatusMsgRegex { get; set; }

        public string ResponseCodeRegex { get; set; }

        public string ErrorCodeRegex { get; set; }

        public string TrnRefNoRegex { get; set; }

        public string OprTrnRefNoRegex { get; set; }

        public string Param1Regex { get; set; }
        public string Param2Regex { get; set; }
        public string Param3Regex { get; set; }

        public void SetActive()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<ThirdPartyAPIResponseConfiguration>(this));
        }
        public void SetInActive()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            Events.Add(new ServiceStatusEvent<ThirdPartyAPIResponseConfiguration>(this));
        }
    }

    public class ArbitrageThirdPartyAPIResponseConfiguration : BizBase
    {
        public string BalanceRegex { get; set; }

        public string StatusRegex { get; set; }

        public string StatusMsgRegex { get; set; }

        public string ResponseCodeRegex { get; set; }

        public string ErrorCodeRegex { get; set; }

        public string TrnRefNoRegex { get; set; }

        public string OprTrnRefNoRegex { get; set; }

        public string Param1Regex { get; set; }

        public string Param2Regex { get; set; }

        public string Param3Regex { get; set; }

        public string Param4Regex { get; set; }
        public string Param5Regex { get; set; }
        public string Param6Regex { get; set; }
        public string Param7Regex { get; set; }

        public void SetActive()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<ArbitrageThirdPartyAPIResponseConfiguration>(this));
        }
        public void SetInActive()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            Events.Add(new ServiceStatusEvent<ArbitrageThirdPartyAPIResponseConfiguration>(this));
        }
    }
}
