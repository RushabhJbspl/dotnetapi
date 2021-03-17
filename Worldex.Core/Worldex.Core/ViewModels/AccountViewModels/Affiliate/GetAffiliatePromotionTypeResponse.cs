using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
    public class GetAffiliatePromotionTypeResponse : BizResponseClass
    {
        public List<AffiliatePromotionTypeResponse> Response { get; set; }
    }

    public class AffiliatePromotionTypeResponse
    {
        public long Id { get; set; }
        public string PromotionType { get; set; }
    }
}
