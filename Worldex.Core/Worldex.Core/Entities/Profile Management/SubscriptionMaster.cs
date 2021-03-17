using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Profile_Management
{
    public class SubscriptionMaster : BizBase
    {
        public int UserId { get; set; }

        public long ProfileId { get; set; }

        [StringLength(2000)]       
        public string AccessibleFeatures { get; set; }
    }
}
