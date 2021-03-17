using Worldex.Core.SharedKernel;
using System;

namespace Worldex.Core.Entities.Configuration.FeedConfiguration
{
    public class UserAPILimitCount : BizBase
    {
        public long PerMinCount { get; set; }
        public DateTime PerMinUpdatedDate { get; set; }
        public long PerDayCount { get; set; }
        public DateTime PerDayUpdatedDate { get; set; }
        public long PerMonthCount { get; set; }
        public DateTime PerMonthUpdatedDate { get; set; }
        public long TotalCall { get; set; }
        public long UserID { get; set; }
        public long PlanID { get; set; }
        public long SubscribeID { get; set; }

    }
}
