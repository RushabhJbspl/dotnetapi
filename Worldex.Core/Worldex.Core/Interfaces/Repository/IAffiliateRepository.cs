using Worldex.Core.ViewModels.AccountViewModels.Affiliate;
using System;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Repository
{
    public interface IAffiliateRepository
    {
        List<GetAffiliateSchemePlan> GetDetailAffiliateSchemeType();
        List<AffiliateAvailablePromotionLink> GetAffiliatePromotionLink(long UserId);
        AffiliatePromotionLimitCount GetAffiliatePromotionLimitCount(long UserId, long PromotionType);
        AffiliateDashboardCount GetAffiliateDashboardCount(long UserId);
        List<GetAffiateUserRegisteredData> GetAffiateUserRegistered(long UserId, string FromDate, string ToDate, int Status, int SchemeType, long ParentId, string SCondition, int PageSize, int PageNo, ref long TotalCount, ref int PageSize1, ref long TotalPages);
        List<GetReferralLinkClickData> GetReferralLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageSize, int PageNo, ref long TotalCount, ref int PageSize1, ref long TotalPages);
        List<GetFacebookLinkClickData> GetFacebookLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageSize, int PageNo, ref long TotalCount, ref int PageSize1, ref long TotalPages);
        List<GetTwitterLinkClickData> GetTwitterLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageSize, int PageNo, ref long TotalCount, ref int PageSize1, ref long TotalPages);
        List<GetEmailSentData> GetEmailSent(string FromDate, string ToDate, long UserId, string SCondition, int PageSize, int PageNo, ref long TotalCount, ref int PageSize1, ref long TotalPages);
        List<GetSMSSentData> GetSMSSent(string FromDate, string ToDate, long UserId, string SCondition, int PageSize, int PageNo, ref long TotalCount, ref int PageSize1, ref long TotalPages);
        List<GetAllAffiliateUserData> GetAllAffiliateUser();
        List<AffiliateCommissionHistoryReport> AffiliateCommissionHistoryReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, long? TrnUserId, long? AffiliateUserId, long? SchemeMappingId, long? TrnRefNo, ref int TotalCount);
        InviteFrdClaas GetAffiliateInviteFrieds(long UserId);
        GetMonthWiseCommissionDataV1 GetMonthWiseCommissionChartDetail(int? Year, long UserId);
    }
}
