using System;

namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
    public class AffiliatePromotionLinkViewModel
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public DateTime CurrentTime { get; set; }

        public DateTime Expirytime { get; set; }

        public string ReferCode { get; set; }

        public long PromotionType { get; set; }
    }
}
