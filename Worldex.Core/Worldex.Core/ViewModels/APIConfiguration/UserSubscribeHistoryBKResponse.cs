using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class UserSubscribeHistoryBKRequest
    {
        public long? UserID { get; set; }
        public long? PlanID { get; set; }
        public short? Status { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public long? Pagesize { get; set; }
        public long? PageNo { get; set; }
    }
    public class UserSubscribeHistoryBKResponse : BizResponseClass
    {
        public long TotalCount { get; set; }
        public long PageCount { get; set; }
        public List<UserSubscribeHistoryBKInfo> Response { get; set; }    
    }
    public class UserSubscribeHistoryBKInfo
    {
        public long UserID { get; set; }
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public decimal Charge { get; set; }
        public decimal TotalAmt { get; set; }
        public short Status { get; set; }
        public string Perticuler { get; set; }
        public short IsAutoRenew { get; set; }
        public int PlanValidity { get; set; }
        public int PlanValidityType { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public short? PaymentStatus { get; set; }
        public short RenewStatus { get; set; }
        public int WhitelistedEndPointsCount { get; set; }
        public int ConcurrentEndPointsCount { get; set; }
        public int ConcurrentCount { get; set; }
        public int KeyCount { get; set; }
        public int WhitelistedEndPoints { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int KeyTotCount { get; set; }
    }
    public class UserSubscribeHistoryBKQryRes
    {
        public long Id { get; set; }
        public long UserID { get; set; }
        public string PlanName { get; set; }
        public Decimal Price { get; set; }
        public Decimal Charge { get; set; }
        public Decimal TotalAmt { get; set; }
        public short Status { get; set; }
        public string Perticuler { get; set; }
        public short IsAutoRenew { get; set; }
        public int PlanValidity { get; set; }
        public int PlanValidityType { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public short? PaymentStatus { get; set; }
        public short RenewStatus { get; set; }
        public int WhitelistedEndPoints { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int KeyTotCount { get; set; }
    }
    public class IPWhitelistCountQryRes
    {
        public long id { get; set; }
        public long UserId { get; set; }
        public string IPAddress { get; set; }
        public short IPType { get; set; }
        public long APIplanID { get; set; }
    }

    public class APIKeyCountQryRes
    {
        public long id { get; set; }
        public long UserId { get; set; }
        public long APIPlanMasterID { get; set; }
        public short IPAccess{get;set;}
    }
}
