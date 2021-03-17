using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.ViewModels.APIConfiguration;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.FeedConfiguration
{
    public interface IAPIConfigurationService
    {
        ViewAPIPlanDetailResponse ViewAPIPlanDetail(long UserID);
        ViewActivePlanDetailResponse ViewUserActivePlan(long UserID);
        GetAutoRenewDetailResponse GetAutoRenewDetail(long UserID);
        BizResponseClass StopAutoRenew(StopAutoRenewRequest Request, long UserID);
        Task<GenerateAPIKeyResponse> GenerateAPIKey(GenerateAPIKeyRequest Request, ApplicationUser user);
        BizResponseClass UpdateAPIKey(long KeyID, long UserID);
        BizResponseClass DeleteAPIKey(long KeyID, long UserID);
        BizResponseClass WhitelistIP(IPWhiteListRequest Request, long UserID);
        APIKeyListPResponse GetAPIKeyList(long APIId, long UserID);
        WhitelistIPListResponse GetWhitelistIP(long PlanId, long UserID, long? KeyID);
        UserAPIPlanHistoryResponse GetUserPlanHistory(UserAPIPlanHistoryRequest request, long UserID);
        BizResponseClass DeleteWhitelistIP(long IPId, long UserID);
        APIKeyListPResponseV2 GetAPIKeyByID(long KeyID, long UserID);

        BizResponseClass SetUserAPICustomeLimit(UserAPICustomeLimitRequest Request, long UserID);
        BizResponseClass UpdateUserAPICustomeLimit(UserAPICustomeLimitRequest Request, long UserID);
        BizResponseClass SetDefaultAPILimits(long LimitId);
        UserAPICustomeLimitResponse GetCustomeLimit(long SubscribeID, long UserID);
    }
}
