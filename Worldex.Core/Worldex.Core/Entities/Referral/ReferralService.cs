using System;
using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Referral
{
    public class ReferralService : BizBase
    {       
        [Required]        
        public int ReferralServiceTypeId { get; set; }

        [Required]
        public int ReferralPayTypeId { get; set; }

        [Required]
        public long CurrencyId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int ReferMinCount { get; set; }

        [Required]
        public int ReferMaxCount { get; set; }

        [Required]
        public decimal RewardsPay { get; set; }
        

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ActiveDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ExpireDate { get; set; }
    }
}
