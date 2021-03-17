using Worldex.Core.ApiModels;
using System;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class GetAutoRenewDetailInfo
    {
        public Decimal Amount { get; set; }
        public Decimal Fees { get; set; }
        public Decimal TotalAmt { get; set; }
        public long Days { get; set; }
        public DateTime NextRenewDate { get; set; }
        public long RenewID { get; set; }
        public long SubscribeID { get; set; }
        public string PlanName { get; set; }
        public short Status { get; set; }
        public int Validity { get; set; }
        public int PlanValidityType { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
    public class GetAutoRenewDetailResponse : BizResponseClass
    {
        public GetAutoRenewDetailInfo Response { get; set; }
    }
    public class AutoRenewDetailQryRes
    {
        public Decimal Price { get; set; }
        public Decimal Charge { get; set; }
        public Decimal TotalAmt { get; set; }
        public long RenewDays { get; set; }
        public short Status { get; set; }
        public string PlanName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public long? nextAutoRenewID { get; set; }
        public long SubscribeID { get; set; }
        public int PlanValidity { get; set; }
        public int PlanValidityType { get; set; }
    }
    public class GetActivationDate
    {
        public DateTime ActivationDate { get; set; }
        public long RenewDays { get; set; }
    }
}
