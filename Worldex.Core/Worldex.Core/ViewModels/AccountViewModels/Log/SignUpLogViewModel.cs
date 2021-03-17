using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.AccountViewModels.Log
{
    public class SignUpLogViewModel
    {
        public long Id { get; set; }
        public int TempUserId { get; set; }
        public int UserId { get; set; }
        public int RegisterType { get; set; }
        public string Device { get; set; }
        public string Mode { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public string HostName { get; set; }
        public bool RegisterStatus { get; set; }
    }

    public class SignUpLogDataViewModel
    {
        public string UserName { get; set; }
        public string RegisterType { get; set; }
        public string Device { get; set; }
        public string Mode { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public string HostName { get; set; }
        public DateTime Date { get; set; }
    }

    public class SignUpLogResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<SignUpLogDataViewModel> SignUpLogHistoryList { get; set; }
    }
}
