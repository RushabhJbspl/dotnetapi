using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class UserAPICustomeLimitRequest
    {
        public long? LimitID { get; set; } //for Update only
        public long SubscribeID { get; set; } //for add only
        public long MaxPerMinute { get; set; }
        public long MaxPerDay { get; set; }
        public long MaxPerMonth { get; set; }
        public long MaxOrderPerSec { get; set; }
        public long MaxRecPerRequest { get; set; }
        public long MaxReqSize { get; set; }
        public long MaxResSize { get; set; }
        public int WhitelistedEndPoints { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int HistoricalDataMonth { get; set; }
        public List<long> ReadonlyAPI { get; set; }
        public List<long> FullAccessAPI { get; set; }
    }
    public class UserAPICustomeLimitResponseInfo
    {
        public long LimitID { get; set; } 
        public long MaxPerMinute { get; set; }
        public long MaxPerDay { get; set; }
        public long MaxPerMonth { get; set; }
        public long MaxOrderPerSec { get; set; }
        public long MaxRecPerRequest { get; set; }
        public long MaxReqSize { get; set; }
        public long MaxResSize { get; set; }
        public int WhitelistedEndPoints { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int HistoricalDataMonth { get; set; }
        public Dictionary<long, String> ReadOnlyAPI { get; set; }
        public Dictionary<long, String> FullAccessAPI { get; set; }
    }
    public class UserAPICustomeLimitResponse : BizResponseClass
    {
        public UserAPICustomeLimitResponseInfo Response { get; set; }
    }
}
