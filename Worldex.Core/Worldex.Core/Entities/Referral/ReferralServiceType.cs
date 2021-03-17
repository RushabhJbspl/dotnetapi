using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Referral
{
   public class ReferralServiceType :BizBase
    {       
        [Required]
        [StringLength(256)]
        public string ServiceTypeName { get; set; }
    }
}
