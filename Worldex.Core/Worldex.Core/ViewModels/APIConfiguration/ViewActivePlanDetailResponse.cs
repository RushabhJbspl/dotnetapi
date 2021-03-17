using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class ViewActivePlanDetailResponse :BizResponseClass
    {
        public ViewActivePlanDetailInfoV1 Response { get; set; }
    }
    public class ViewActivePlanDetailInfoV1 : ViewActivePlanDetailInfo
    {
        public Dictionary<long, String> ReadOnlyAPI { get; set; }
        public Dictionary<long, String> FullAccessAPI { get; set; }
    }
    public class ViewActivePlanDetailInfo
    {
        public long SubscribeID { get; set; }
        public long PlanID { get; set; }
        public string PlanName { get; set; }
        public Decimal PlanPrice { get; set; }
        public short PlanStatus { get; set; }
        public long MaxPerDay { get; set; }
        public long MaxPerMinute { get; set; }
        public long MaxPerMonth { get; set; }
        public long MaxOrderPerSec { get; set; }
        public long MaxRecPerRequest { get; set; }
        public int WhitelistedEndPoints { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public long MaxReqSize { get; set; }
        public long MaxResSize { get; set; }
        public int HistoricalDataMonth { get; set; }
        public Decimal PaidAmt { get; set; }
        public short? PaymentStatus { get; set; }
        public short SubScribeStatus { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime ActivationDate { get; set; }
        public short IsAutoRenew { get; set; }
        public short RenewStatus { get; set; }
        public DateTime? RenewDate { get; set; }
        public int PlanValidity { get; set; }
        public int PlanValidityType { get; set; }
        public short IsPlanRecursive { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public Decimal Price { get; set; }
        public Decimal Charge { get; set; }
        public Decimal TotalAmt { get; set; }
        public int Priority { get; set; }
        public long CustomeLimitId { get; set; }
        public long RenewDays { get; set; }
    }

    public class GetCustomLimitIDQry
    {
        public long CustomeLimitId { get; set; }
    }
}
