using System;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.SharedKernel;
using Worldex.Core.Events;
using Worldex.Core.Enums;

namespace Worldex.Core.Entities.Communication
{
    public class MessagingQueue : BizBase
    {
        [Required]
        [Phone]
        public long MobileNo { get; set; }

        [Required]
        [StringLength(200)]
        public string SMSText { get; set; }

        [StringLength(1000)]
        public string RespText { get; set; }
        
        public short SMSServiceID { get; set; }
        
        public short SMSSendBy { get; set; }

        public void FailMessage()
        {
            Status = Convert.ToInt16(MessageStatusType.Fail);
            Events.Add(new ServiceStatusEvent<MessagingQueue>(this));
        }

        public void InQueueMessage()
        {
            Status = Convert.ToInt16(MessageStatusType.Pending);
            Events.Add(new ServiceStatusEvent<MessagingQueue>(this));
        }

        public void SentMessage()
        {
            Status = Convert.ToInt16(MessageStatusType.Success);
            Events.Add(new ServiceStatusEvent<MessagingQueue>(this));
        }
    }
}
