using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Referral
{
    public class ReferralChannel : BizBase
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public int ReferralChannelTypeId { get; set; }
        
        [Required]
        public int ReferralChannelServiceId { get; set; }
                        
        [StringLength(1000)]
        public string ReferralReceiverAddress { get; set; }

    }
}
