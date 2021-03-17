using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.SharedKernel;
using Worldex.Core.Events;
using Worldex.Core.Enums;

namespace Worldex.Core.Entities.Communication
{
    public class NotificationQueue : BizBase
    {
        [StringLength(50)]
        public string Subject { get; set; }

        [Required]
        [StringLength(200)]
        public string Message { get; set; }

        [Required]
        [StringLength(500)]
        public string DeviceID { get; set; }

        [Required]
        [StringLength(200)]
        public string TickerText { get; set; }

        [Required]
        [StringLength(200)]
        public string ContentTitle { get; set; }

        public void FailMessage()
        {
            Status = Convert.ToInt16(MessageStatusType.Fail);
            Events.Add(new ServiceStatusEvent<NotificationQueue>(this));
        }

        public void InQueueMessage()
        {
            Status = Convert.ToInt16(MessageStatusType.Pending);
            Events.Add(new ServiceStatusEvent<NotificationQueue>(this));
        }

        public void SentMessage()
        {
            Status = Convert.ToInt16(MessageStatusType.Success);
            Events.Add(new ServiceStatusEvent<NotificationQueue>(this));
        }
    }

    public class DeviceStore : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Key]
        [Required]
        public long UserID { get; set; }

        [Required]
        [StringLength(500)]
        public string DeviceID { get; set; }        

        public void InActive()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<DeviceStore>(this));
        }        

        public void Active(string DeviceID)
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            this.DeviceID = DeviceID;
            Events.Add(new ServiceStatusEvent<DeviceStore>(this));
        }
    }
}
