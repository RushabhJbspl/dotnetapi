using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.AccountViewModels.Log
{
   public class LoginhistoryViewModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }

        public string IpAddress { get; set; }
        public string Device { get; set; }
        public string Location { get; set; }
    }

    public class LoginHistoryDataViewModel
    {
        public string IpAddress { get; set; }
        public string Device { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
    }


    public class LoginHistoryResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<LoginHistoryDataViewModel> LoginHistoryList { get; set; }
    }
}
