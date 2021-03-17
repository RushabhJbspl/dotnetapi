using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Affiliate
{
    public class AffiliatePromotionUserTypeMapping : BizBase
    {
        [Required]
        public long AffiliateUserId { get; set; } // Reference From AffiliateUserMaster

        [Required]
        public long PromotionTypeId { get; set; } // Reference From AffiliatePromotionMaster

        public string PromotionLink { get;set; }

        public string DecryptedCode { get; set; }

        public string ShortLink { get; set; } // Uday 19-02-2019 Store the short link of promotional link
    }
}
