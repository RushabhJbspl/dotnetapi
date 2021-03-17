using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class ViewAPIPlanConfigHistoryResponse :BizResponseClass
    {
        public long TotalCount { get; set; }
        public long PageCount { get; set; }
        public List<ViewAPIPlanConfigHistoryInfo> Response { get; set; }
    }
    public class ViewAPIPlanConfigHistoryInfo
    {
        public long Id { get; set; }
        public long PlanID { get; set; }
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public decimal Charge { get; set; }
        public string PlanDesc { get; set; }
        public int Priority { get; set; }
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
        public short IsPlanRecursive { get; set; }
        public int PlanValidity { get; set; }
        public int PlanValidityType { get; set; }
        public string CreatedIPAddress { get; set; }
        public long LastModifyBy { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string ModifyDetails { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public short Status { get; set; }
        public List<APIPlanMethodConfigHistoryRes> MethodList { get; set; }
    }
    public class APIPlanMethodConfigHistoryRes
    {
        public long APIPlanHistoryID { get; set; }
        public long RestMethodID { get; set; }
        public long APIPlanMasterID { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public short Status { get; set; }
    }
    public class ViewAPIPlanConfigHistoryRequest
    {
        public long? UserID { get; set; }
        public long? PlanID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public long? Pagesize { get; set; }
        public long? PageNo { get; set; }
    }
}
