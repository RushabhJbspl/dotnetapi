using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class APIPlanDetailViewModel
    {
        public long? ID { get; set; }
        public long APIPlanMasterID { get; set; }
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
        public short Status { get; set; }
    }
    public class APIPlanDetailRequest:APIPlanDetailViewModel
    {

    }

    public class APIPlanDetailResponse : BizResponseClass
    {
        public List<APIPlanDetailResponseInfo> Response { get; set; }
    }
    public class APIPlanDetailResponseInfo
    {
        public long? ID { get; set; }
        public long APIPlanMasterID { get; set; }
        public string APIPlanName { get; set; }
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
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public short Status { get; set; }
    }
}
