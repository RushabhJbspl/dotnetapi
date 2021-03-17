using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.AccountViewModels.Log
{
    public class IpHistoryViewModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class IpHistoryDataViewModel
    {
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
    }


    public class IpHistoryResponse : BizResponseClass
    {
        public int Totalcount { get; set; }
        public List<IpHistoryDataViewModel> IpHistoryList { get; set; }
    }
}
