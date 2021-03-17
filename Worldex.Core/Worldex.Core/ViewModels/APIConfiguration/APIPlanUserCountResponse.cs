using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class APIPlanUserCountResponse : BizResponseClass
    {
        public long TotalCount { get; set; }
        public List<APIPlanUserCountResponseInfo> Response { get; set; }
    }
    public class APIPlanUserCountResponseInfo
    {
        public int Users { get; set; }
        public long APIPlanMasterID { get; set; }
        public string PlanName { get; set; }
        public int PurchasePlan { get; set; }
        public Decimal Earnings { get; set; }
    }
    public class APIPlanUserCountRequest
    {
        public long? UserID { get; set; }
        public long? PlanID { get; set; }
        public short? Status { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public long? Pagesize { get; set; }
        public long? PageNo { get; set; }
    }
}
