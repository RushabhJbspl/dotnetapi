using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
    public class AffiliateCommissionHistoryReport
    {
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public long TrnRefNo { get; set; }
        public long CronRefNo { get; set; }
        public long FromWalletId { get; set; }
        public long ToWalletId { get; set; }
        public string FromWalletName { get; set; }
        public string ToWalletName { get; set; }
        public string FromAccWalletId { get; set; }
        public string ToAccWalletId { get; set; }
        public decimal Amount { get; set; }
        public long AffiliateUserId { get; set; }
        public string AffiliateUserName { get; set; }
        public string AffiliateEmail { get; set; }
        public long SchemeMappingId { get; set; }
        public string SchemeMappingName { get; set; }
        public long TrnUserId { get; set; }
        public string TrnUserName { get; set; }
        public string TrnEmail { get; set; }
        public string Remarks { get; set; }
        public long TrnWalletTypeId { get; set; }
        public string TrnWalletTypeName { get; set; }
        public decimal CommissionPer { get; set; }
        public short Level { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime TrnDate { get; set; }
    }

    public class ListAffiliateCommissionHistoryReport : BizResponseClass
    {
        public List<AffiliateCommissionHistoryReport> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class InviteFrdClaas
    {
        public decimal AffiliateLink { get; set; }
        public decimal Facebook { get; set; }
        public decimal Twitter { get; set; }
        public decimal Email { get; set; }
        public decimal SMS { get; set; }
    }
    public class ListInviteFrdClaas : BizResponseClass
    {
        public InviteFrdClaas Data { get; set; }
    }

    public class GetMonthWiseCommissionData
    {
        public long SchemeTypeId { get; set; }
        public string SchemeTypeName { get; set; }
        public string DataString { get; set; }
        [NotMapped]
        public decimal[] Data { get; set; }
    }
    public class GetMonthWiseCommissionDataArray
    {
        public long SchemeTypeId { get; set; }
        public string SchemeTypeName { get; set; }
        public decimal[] Data { get; set; }
    }
    public class ListGetMonthWiseCommissionData : BizResponseClass
    {
        public GetMonthWiseCommissionDataV1 Response { get; set; }
    }
    public class GetMonthWiseCommissionDataV1
    {
        public decimal[] Deposition { get; set; }
        public decimal[] BuyTrading { get; set; }
        public decimal[] SignUp { get; set; }
        public decimal[] SellTrading { get; set; }
    }

}
