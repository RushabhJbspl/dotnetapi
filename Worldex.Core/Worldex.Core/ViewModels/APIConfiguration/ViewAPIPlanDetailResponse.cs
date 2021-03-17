using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class ViewAPIPlanDetailResponse :BizResponseClass
    {
        public List<ViewAPIPlanDetailResponseInfo> Response { get; set; }
    }
    public class ViewAPIPlanDetailResponseInfo
    {
        public long ID { get; set; }
        public long SubscribeID { get; set; }
        public string PlanName { get; set; }
        public string PlanDesc { get; set; }
        public decimal Price { get; set; }
        public decimal Charge { get; set; }
        public int PlanValidity { get; set; }
        public int PlanValidityType { get; set; }
        public int Priority { get; set; }
        public short Status { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int WhitelistEndPoints { get; set; }
        public int HistoricalDataMonth { get; set; }
        public long MaxPerDay { get; set; }
        public long MaxOrderPerSec { get; set; }
        public long MaxPerMinute { get; set; }
        public long MaxPerMonth { get; set; }
        public long MaxRecPerRequest { get; set; }
        public long MaxReqSize { get; set; }
        public long MaxResSize { get; set; }
        public short IsPlanRecursive { get; set; }
        public short IsSubscribePlan { get; set; }
        public DateTime ExpireDate { get; set; }
        public List<String> ReadOnlyAPI { get; set; }
        public List<String> FullAccessAPI { get; set; }
        public string Coin { get; set; }
        public long ServiceID { get; set; }
        
    }
    public class ViewAPIPlanDetailQryResponse
    {
        public long Id { get; set; }
        public string PlanName { get; set; }
        public int PlanValidity { get; set; }
        public decimal Price { get; set; }
        public decimal Charge { get; set; }
        public short PlanStatus { get; set; }
        public long PlanDetailID { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int HistoricalDataMonth { get; set; }
        public long MaxPerDay { get; set; }
        public long MaxOrderPerSec { get; set; }
        public long MaxPerMinute { get; set; }
        public long MaxPerMonth { get; set; }
        public long MaxRecPerRequest { get; set; }
        public long MaxReqSize { get; set; }
        public long MaxResSize { get; set; }
        public short PlanDetailStatus { get; set; }
    }

    public class PlanMethodsQryResponse
    {
        public long ID { get; set; }
        public long RestMethodID { get; set; }
        public long APIPlanMasterID { get; set; }
        public string MethodName { get; set; }
        public short IsFullAccess { get; set; }
        public short IsReadOnly { get; set; }
    }
}
