using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Affiliate
{
    public class AffiliateUserMaster : BizBase
    {
        [Required]
        public long UserId { get; set; }  // Reference From BizUser

        public long ParentId { get; set; } // Reference From AffiliateUserMaster

        public long UserBit { get; set; } // EnAffiliateUserType

        public long SchemeMstId { get; set; } // Reference From AffiliateSchemeMaster

        public long PromotionTypeId { get; set; } // Reference From AffiliatePromotionMaster

        public string ReferCode { get; set; }

        public void MakeAffiliateUserActive()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }

        public void MakeAffiliateUserInActive()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }

        public void MakeAffiliateUserDisable()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            AddValueChangeEvent();
        }

        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<AffiliateUserMaster>(this));
        }
    }
}
