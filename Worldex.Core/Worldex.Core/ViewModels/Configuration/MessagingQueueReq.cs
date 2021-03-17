using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class MessagingQueueRes
    {
        public short Status { get; set; }
        public long MobileNo { get; set; }
        public string SMSDate { get; set; }
        public string SMSText { get; set; }
        public string StrStatus { get; set; }
        public long MessageID { get; set; }
    }

    public class ListMessagingQueueRes : BizResponseClass
    {
        public List<MessagingQueueRes> MessagingQueueObj { get; set; }
        public long TotalPage { get; set; }
        public long PageSize { get; set; }
        public long Count { get; set; }
    }
}
