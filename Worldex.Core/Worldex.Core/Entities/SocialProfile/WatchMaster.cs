using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.SocialProfile
{
   public class WatchMaster : BizBase
    {
        [Required]
        public long GroupId { get; set; }
        [ForeignKey("GroupId")]
        public GroupMaster GroupMaster { get; set; }

        [Required]
        public long LeaderId { get; set; }
        [Required]
        public long WatcherId { get; set; }

        public bool WatcherStatus { get; set; }

        public void DisableWatch()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            Events.Add(new ServiceStatusEvent<WatchMaster>(this));
        }

        public void EnableWatch()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<WatchMaster>(this));
        }
    }
}
