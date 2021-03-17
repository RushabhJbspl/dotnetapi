using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
    public class AffiliateDashboardCountResponse : BizResponseClass
    {
        public AffiliateDashboardCount Response { get; set; }
    }

    public class AffiliateDashboardCount
    {
        public long UserCount { get; set; }

        public long UserTodayCount { get; set; }

        public long AffiliateCount { get; set; }

        public long AffiliateTodayUserCount { get; set; }

        public long CommissionCount { get; set; }

        public long CommissionTodayCount { get; set; }

        public long ReferralLinkCount { get; set; }

        public long ReferralLinkTodayCount { get; set; }

        public long FacebookLinkCount { get; set; }

        public long FacebookLinkTodayCount { get; set; }

        public long GooglePlusLinkCount { get; set; }

        public long GooglePlusTodayCount { get; set; }

        public long TwitterLinkCount { get; set; }

        public long TwitterLinkTodayCount { get; set; }

        public long EmailSentCount { get; set; }

        public long EmailSentTodayCount { get; set; }

        public long SMSSentCount { get; set; }

        public long SMSSentTodayCount { get; set; }
    }
}
