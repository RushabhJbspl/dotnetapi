using Worldex.Core.SharedKernel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Referral
{
    public class ReferralUser : BizBase
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ReferUserId { get; set; }

        [Required]
        public int ReferralServiceId { get; set; }

        public int ReferralChannelTypeId { get; set; }

        [DefaultValue(0)]
        public short IsCommissionCredited { get; set; }//1-credited
    }
}
