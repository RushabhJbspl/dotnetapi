using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.AccountViewModels.Log
{
    public class ActivityLogViewModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public string Device { get; set; }
        public string Mode { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public string HostName { get; set; }
    }

    public class ActivityLogDataViewModel
    {
        public string Action { get; set; }
        public string Mode { get; set; }
        public string Device { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public string HostName { get; set; }
        public DateTime Date { get; set; }
    }

    public class ActivityLogListViewModel
    {
        public string Action { get; set; }
        public string Mode { get; set; }
        public string Device { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public string HostName { get; set; }
        public DateTime Date { get; set; }
        public long Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int Status { get; set; }
    }


    public class ActivityLogResponse : BizResponseClass
    {
        public int TotalRow { get; set; }
        public List<ActivityLogListViewModel> ActivityLogHistoryList { get; set; }
    }
}
