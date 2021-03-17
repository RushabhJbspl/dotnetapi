using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Affiliate
{
    public class AffiliatePromotionLimitConfiguration : BizBase
    {
        public long PromotionType { get; set; }
        public long HourlyLimit { get; set; }
        public long DailyLimit { get; set; }
    }
}
