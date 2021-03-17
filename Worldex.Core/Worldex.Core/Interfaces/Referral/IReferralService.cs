using System.Collections.Generic;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Referral;

namespace Worldex.Core.Interfaces.Referral
{
   public interface IReferralService
    {
        long AddReferralService(ReferralServiceViewModel model, long UserID);
        long UpdateReferralService(ReferralServiceUpdateViewModel model, long UserID);
        ReferralServiceUpdateViewModel GetReferralServiceById(long Id);
        ReferralServiceUpdateViewModel GetReferralService();
        ReferralServiceListResponse ListReferralService(int PageIndex = 0, int Page_Size = 0);
        bool DisableReferralService(ReferralServiceStatusViewModel model, long UserId);
        bool EnableReferralService(ReferralServiceStatusViewModel model, long UserId);
        List<ReferralServiceDropDownViewModel> DropDownReferralService(int PayTypeId);
        bool ReferralServiceExist(int ServiceId);
        long ReferralServiceId();
        Task<BizResponseClass> AddUpdateReferralSchemeTypeMapping(AddReferralSchemeTypeMappingReq request, long id);
        Task<BizResponseClass> ChangeReferralSchemeTypeMappingStatus(long id, ServiceStatus status, long UserId);
        Task<GetReferralSchemeTypeMappingRes> GetReferralSchemeTypeMappingById(long id);
        Task<ListReferralSchemeTypeMappingRes> ListReferralSchemeTypeMapping(long? payTypeId, long? serviceTypeMstId, short? status);
        Task<BizResponseClass> AddUpdateReferralServiceDetail(AddServiceDetail request, long id);
        Task<BizResponseClass> ChangeReferralServiceDetailStatus(long id, ServiceStatus status, int userid);
        Task<GetReferralServiceDetailRes> GetReferralServiceDetailByid(long id);
        Task<ListReferralServiceDetailRes> ReferralServiceDetail(long? schemeTypeMappingId, long? creditWalletTypeId, short? status);
    }
}
