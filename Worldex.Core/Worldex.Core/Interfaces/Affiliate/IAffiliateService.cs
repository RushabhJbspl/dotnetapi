using Worldex.Core.Entities.User;
using Worldex.Core.ViewModels.AccountViewModels.Affiliate;
using System;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Affiliate
{
    public interface IAffiliateService
    {
        long GetAffiliateParentUser(string ReferCode);
        long AddAffiliateUser(long ParentId, long PromotionType, short UserBit, long SchemeId, long BizUserId);
        long GetAdminUser();
        List<AffiliatePromotionTypeResponse> GetAffiliatePromotionType();
        long AddAffiliatePromotionType(AddAffiliatePromotionTypeRequest Request, ApplicationUser UserData);
        void ActiveAffiliateAccount(long UserId);
        int GeAffiliateSchemeType(long SchemeId);
        List<AffiliateSchemeTypeResponseData> GetAffiliateSchemeType();
        List<AffiliateSchemeAvailableSchemeData> GetDetailAffiliateSchemeType();
        dynamic GetAffiliatePromotionLink(ApplicationUser User);
        int SendAffiliateEmail(SendAffiliateEmailRequest Request, ApplicationUser User);
        int SendAffiliateSMS(SendAffiliateSMSRequest Request, ApplicationUser User);
        int AddPromotionLinkClick(AddPromotionLinkClickRequest Request, string PassData);

        //2019-3-15
        AffiliateDashboardCount GetAffiliateDashboardCount(long UserId);
        GetAffiateUserRegisteredResponse GetAffiateUserRegistered(long UserId, string FromDate, string ToDate, int Status, int SchemeType, long ParentId, string SCondition, int PageNo, int PageSize);
        GetReferralLinkClickResponse GetReferralLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize);
        GetFacebookLinkClickResponse GetFacebookLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize);
        GetTwitterLinkClickResponse GetTwitterLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize);
        GetEmailSentResponse GetEmailSent(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize);
        GetSMSSentResponse GetSMSSent(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize);
        List<GetAllAffiliateUserData> GetAllAffiliateUser();
        bool IsValidDateFormate(string date);

        ListAffiliateCommissionHistoryReport AffiliateCommissionHistoryReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, long? TrnUserId, long? AffiliateUserId, long? SchemeMappingId, long? TrnRefNo);

        ListInviteFrdClaas GetAffiliateInviteFrieds(long UserId);
        ListGetMonthWiseCommissionData GetMonthWiseCommissionChartDetail(int? Year,long UserId);
    }
}
