using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Referral
{
    public class ReferralUserClick : BizBase
    {
        public long UserId { get; set; }

        public long ReferralServiceId { get; set; }

        public long ReferralChannelTypeId { get; set; }

        public string IPAddress { get; set; }
    }
}
