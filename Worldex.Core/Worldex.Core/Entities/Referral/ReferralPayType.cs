using System.ComponentModel.DataAnnotations;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Referral
{
   public class ReferralPayType : BizBase
    {       
        [Required]
        [StringLength(50)]
        public string PayTypeName { get; set; }
    }
}
