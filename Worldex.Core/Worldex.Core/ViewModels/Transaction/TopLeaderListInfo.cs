using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class TopLeaderListInfo
    {
        public long LeaderId { get; set; }
        public string LeaderName { get; set; }
        public int NoOfFollowers { get; set; }
        public string UserDefaultVisible { get; set; }
        public bool IsFollow { get; set; }
        public bool IsWatcher { get; set; }
    }
    public class TopLeadersListResponse: BizResponseClass
    {
        public List<TopLeaderListInfo> Response { get; set; }
    }
    public class TradeWatchListsQryRes
    {
        public long LeaderId { get; set; }
        public long Total { get; set; }
        public int buy { get; set; }
        public int sell { get; set; }
        public string LeaderName { get; set; }
    }
    public class TradeWatchLists
    {
        public long LeaderId { get; set; }
        public string LeaderName { get; set; }
        public long Total { get; set; }
        public string TrnType { get; set; }
        public decimal MaxTrade { get; set; }
        public decimal Percentage { get; set; }
    }
    public class TradeWatchListResponse : BizResponseClass
    {
        public List<TradeWatchLists> Response { get; set; }
    }

    public class TopProfitGainerLoser
    {
        public long LeaderId { get; set; }
        public string LeaderName { get; set; }
        public decimal Profit { get; set; }
        //public long AutoId { get; set; }
        public string Email { get; set; }
        public decimal ProfitPer { get; set; }
    }
    public class TopProfitGainerLoserResponse : BizResponseClass
    {
        public List<TopProfitGainerLoser> Response { get; set; }
    }
    
}
