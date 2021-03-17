using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.AccountViewModels.Affiliate;
using Worldex.Core.ViewModels.BackOfficeAffiliate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.Affiliate
{
    public interface IAffiliateBackOfficeService
    {
        AffiliateDashboardCount GetAffiliateDashboardCount();
        GetAffiateUserRegisteredResponse GetAffiateUserRegistered(string FromDate,string ToDate,int Status,int SchemeType,long ParentId,string SCondition,int PageNo,int PageSize);
        GetReferralLinkClickResponse GetReferralLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize);
        GetFacebookLinkClickResponse GetFacebookLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize);
        GetTwitterLinkClickResponse GetTwitterLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize);
        GetEmailSentResponse GetEmailSent(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize);
        GetSMSSentResponse GetSMSSent(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize);
        List<GetAllAffiliateUserData> GetAllAffiliateUser();
        bool IsValidDateFormate(string date);

        Task<BizResponseClass> AddAffiliateScheme(AffiliateSchemeMasterReqRes request, long UserId);
        Task<BizResponseClass> UpdateAffiliateScheme(AffiliateSchemeMasterReqRes request, long Userid);
        Task<BizResponseClass> ChangeAffiliateSchemeStatus(ChangeAffiliateSchemeStatus request, long id);
        Task<GetAffiliateSchemeMasterRes> GetAffiliateSchemeById(long affiliateSchemeId, long id);
        Task<ListAffiliateSchemeMasterRes> ListAffiliateSchemeById(int PageNo, int? PageSize);
        Task<BizResponseClass> AddAffiliateSchemeType(AffiliateSchemeTypeMasterReq request, long id);
        Task<BizResponseClass> UpdateAffiliateSchemeType(AffiliateSchemeTypeMasterReq request, long id);
        Task<BizResponseClass> ChangeAffiliateSchemeTypeStatus(ChangeAffiliateSchemeTypeStatus request, long id);
        Task<GetAffiliateSchemeTypeMasterRes> GetAffiliateSchemeTypeById(long schemeTypeId, long id);
        Task<ListAffiliateSchemeTypeMasterRes> ListAffiliateSchemeTypeById(int PageNo, int? PageSize);

        Task<BizResponseClass> AddAffiliatePromotion(AffiliatePromotionMasterReq request, long id);
        Task<BizResponseClass> UpdateAffiliatePromotion(AffiliatePromotionMasterReq request, long id);
        Task<BizResponseClass> ChangeAffiliatePromotionStatus(ChangeAffiliatePromotionStatus request, long id);
        Task<GetAffiliatePromotionMasterRes> GetAffiliatePromotionById(long PromotionId, long id);
        Task<ListAllAffiliatePromotionMasterRes> ListAffiliatePromotion(int PageNo, int? PageSize);
        BizResponseClass AddAffiliateSchemeTypeMapping(AffiliateSchemeTypeMappingViewModel model, long UserID);
        BizResponseClass UpdateAffiliateSchemeTypeMapping(AffiliateSchemeTypeMappingUpdateViewModel model, long UserID);
        GetByIdSchemeTypeMapping GetAffiliateSchemeTypeMapping(long Id);
        AffiliateSchemeTypeMappingListResponse ListAffiliateSchemeTypeMapping(long? SchemeId, long? SchemeTypeId, int PageNo, int? PageSize);
        Task<BizResponseClass> ChangeAffiliateSchemeTypeMappingStatus(SchemeTypeMappingChangeStatusViewModel request, long id);

        Task<ListAffiliateShemeDetailRes> ListAffiliateSchemeDetail(int PageNo, int? PageSize);
        Task<BizResponseClass> ChangeAffiliateShemeDetailStatus(ChangeAffiliateSchemeDetailStatus request, long id);
        Task<GetAffiliateShemeDetailResById> GetAffiliateSchemeDetail(long Id);
        ListAffiliateCommissionHistoryReport AffiliateCommissionHistoryReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, long? TrnUserId, long? AffiliateUserId, long? SchemeMappingId, long? TrnRefNo);
        Task<BizResponseClass> AddAffiliateSchemeDetail(AffiliateShemeDetailReq request, long id);
        Task<BizResponseClass> UpdateAffiliateSchemeDetail(AffiliateShemeDetailReq request, long id);
        ListInviteFrdClaas GetAffiliateInviteFrieds();

        ListGetMonthWiseCommissionData GetMonthWiseCommissionChartDetail(int? Year);
    }
}
