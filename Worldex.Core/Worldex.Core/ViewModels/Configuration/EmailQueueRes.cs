using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class EmailQueueRes
    {
        public short Status { get; set; }
        public string RecepientEmail { get; set; }
        public string  EmailDate { get; set; }
        public string Body { get; set; }
        public string StrStatus { get; set; }
        public string Subject { get; set; }
        public string Attachment { get; set; }
        public string EmailType { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public long EmailID { get; set; }
    }
    public class ListEmailQueueRes : BizResponseClass
    {
        public List<EmailQueueRes> EmailQueueObj { get; set; }
        public long TotalPage { get; set; }
        public long PageSize { get; set; }
        public long Count { get; set; }
    }
}
