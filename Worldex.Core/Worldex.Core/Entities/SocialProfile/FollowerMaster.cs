using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.SocialProfile
{
    public class FollowerMaster : BizBase
    {
        [Required]
        public long LeaderId { get; set; }
        [Required]
        public long FolowerId { get; set; }
        public bool FllowerStatus { get; set; }

        public bool IsEnable { get; set; }

        public bool IsDeleted { get; set; }

        public void SetFollowStatus()
        {
            FllowerStatus = false;
            Events.Add(new ServiceStatusEvent<FollowerMaster>(this));
        }
        public void SetUnFollowStatus()
        {
            FllowerStatus = true;
            Events.Add(new ServiceStatusEvent<FollowerMaster>(this));
        }

        public void SetEnableStatus()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<FollowerMaster>(this));
        }
        public void SetDisableStatus()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<FollowerMaster>(this));
        }

        public void SetInActiveStatus()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            Events.Add(new ServiceStatusEvent<FollowerMaster>(this));
        }
    }
}
