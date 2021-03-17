using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Complaint
{
   public class Complainmaster  : BizBase
    {
        public int UserID { get; set; }
        public long TypeId { get; set; }
        [Required]
        [StringLength(500)]
        public string Subject { get; set; }
        [Required]
        [StringLength(4000)]
        public string Description { get; set; }
        public long ComplaintPriorityId { get; set; }
        public void SetDoneComplainStatus(int UserId)
        {
            Status = Convert.ToInt16(enComplainStatusType.Open);
            UpdatedBy = UserId;
            UpdatedDate = DateTime.UtcNow;
            Events.Add(new ServiceStatusEvent<Complainmaster>(this));
        }

        public void SetCloseComplainStatus(int UserId)
        {
            Status = Convert.ToInt16(enComplainStatusType.Close);
            UpdatedBy = UserId;
            UpdatedDate = DateTime.UtcNow;
            Events.Add(new ServiceStatusEvent<Complainmaster>(this));
        }

        public void SetPendingComplainStatus(int UserId)
        {
            Status = Convert.ToInt16(enComplainStatusType.Pending);
            UpdatedBy = UserId;
            UpdatedDate = DateTime.UtcNow;
            Events.Add(new ServiceStatusEvent<Complainmaster>(this));
        }
    }
}
