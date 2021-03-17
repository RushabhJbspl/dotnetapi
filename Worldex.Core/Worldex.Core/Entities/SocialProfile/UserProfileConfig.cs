using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.SocialProfile
{
   public class UserProfileConfig : BizBase
    {
        [Required]
        public int UserId { get; set; }

        public long LeaderId { get; set; }

        [Required]
        public long ProfileConfigId { get; set; }

        [Required]
        [StringLength(250)]
        public string ConfigValue { get; set; }

        public bool IsEnable { get; set; }

        public bool IsDeleted { get; set; }

        public void SetEnableStatus()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<UserProfileConfig>(this));
        }
        public void SetDisableStatus()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<UserProfileConfig>(this));
        }

        public void SetUnDeleteStatus()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            Events.Add(new ServiceStatusEvent<UserProfileConfig>(this));
        }
    }
}
