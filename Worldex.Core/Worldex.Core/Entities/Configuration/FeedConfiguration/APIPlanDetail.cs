using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Configuration.FeedConfiguration
{
    public class APIPlanDetail :BizBase
    {
        public long APIPlanID { get; set; }
        public long MaxPerMinute { get; set; }
        public long MaxPerDay { get; set; }
        public long MaxPerMonth { get; set; }
        public long MaxOrderPerSec { get; set; }
        public long MaxRecPerRequest { get; set; }
        public long MaxReqSize { get; set; }
        public long MaxResSize { get; set; }
        public int WhitelistedEndPoints { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int HistoricalDataMonth { get; set; }
        public long UserId { get; set; }
    }
}