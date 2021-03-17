using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.SignalR
{
    public class FeedLimitCounts : BizBase
    {
        public long MethodID { get; set; }
        public long LimitCount { get; set; }
        public long UserID { get; set; }
    }
}
