using Worldex.Core.SharedKernel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Referral
{
    public class ReferralUserLevelMapping : BizBase
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public long ReferUserId { get; set; }

        [Required]
        public long Level { get; set; }

        [Required]
        public short IsCommissionCredited { get; set; }//1-credited

        [DefaultValue(0)]
        public short IsTradingCommissionCredited { get; set; }//1-credited
    }
}
