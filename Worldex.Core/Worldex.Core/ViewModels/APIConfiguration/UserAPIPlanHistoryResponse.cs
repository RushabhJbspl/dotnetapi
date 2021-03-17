using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class UserAPIPlanHistoryResponse :BizResponseClass
    {
        public long TotalCount { get; set; }
        public List<UserAPIPlanHistoryResponseInfo> Response { get; set; }
    }
    public class UserAPIPlanHistoryResponseInfo
    {
        public string PlanName { get; set; }
        public short Status { get; set; }
        public string Perticuler { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Decimal Price { get; set; }
        public Decimal Charge { get; set; }
        public Decimal TotalAmt { get; set; }
        public short? PaymentStatus { get; set; }
    }
    public class UserAPIPlanHistoryQryRes
    {
        public string PlanName { get; set; }
        public short Status { get; set; }
        public string Perticuler { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Decimal Price { get; set; }
        public Decimal Charge { get; set; }
        public Decimal TotalAmt { get; set; }
        public short? PaymentStatus { get; set; }
        public short RenewStatus { get; set; }
    }

    public class UserAPIPlanHistoryRequest
    {
        public long? PlanID { get; set; }
        public short? PaymentStatus { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public long? Pagesize { get; set; }
        public long? PageNo { get; set; }
    }
}
