using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class NotificationQueueRes
    {
        public short Status { get; set; }
        public string NotificationDate { get; set; }
        public string StrStatus { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string  DeviceID { get; set; }
        public string ContentTitle { get; set; }
        public string TickerText { get; set; }
        public string UserName { get; set; }
        public long  NotificationID { get; set; }
    }

    public class ListNotificationQueueRes : BizResponseClass
    {
        public List<NotificationQueueRes> NotificationQueueObj { get; set; }
        public long TotalPage { get; set; }
        public long PageSize { get; set; }
        public long Count { get; set; }
    }
}
