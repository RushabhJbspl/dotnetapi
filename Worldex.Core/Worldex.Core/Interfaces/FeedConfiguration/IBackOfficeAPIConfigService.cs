using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.APIConfiguration;

namespace Worldex.Core.Interfaces.FeedConfiguration
{
    public interface IBackOfficeAPIConfigService
    {
        #region API Plan
        BizResponseClass AddAPIPlane(APIPlanMasterRequest Request, long UserID);
        BizResponseClass UpdatePlane(APIPlanMasterRequestV1 Request, long UserID);
        APIPlanMasterResponse GetAPIPlan();
        BizResponseClass EnableDisableAPIPlan(long PlanID, short AllowAPIKey, short Status, long UserID);
        void ReloadAPIPlan();
        void ReloadAPIPlanDetails();
        #endregion

        #region API Methods
        BizResponseClass AddAPIMethod(APIMethodsRequest Request, long UserID);
        BizResponseClass UpdateAPIMethod(APIMethodsRequest2 Request, long UserID);
        void ReloadAPIPlanMethodConfiguration();
        APIMethodResponse GetAPIMethods();
        APIMethodResponseV2 GetRestMethodsReadOnly();
        APIMethodResponseV2 GetRestMethodsFullAccess();
        RestMethodResponse GetRestMethods();
        void ReloadRestMethods();
        #endregion

        #region History Method
        APIPlanUserCountResponse GetAPIPlanUserCount(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? Status, long? PlanID);
        UserSubscribeHistoryBKResponse GetUserSubscribeHistoryBK(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? Status, long? PlanID);
        ViewAPIPlanConfigHistoryResponse ViewAPIPlanConfiguration(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? PlanID);
        ViewPublicAPIKeysResponse ViewPublicAPIKeysBK(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? Status, long? PlanID);
        #endregion

        #region API Key policy
        BizResponseClass UpdatePublicAPIKeyPolicy(PublicAPIKeyPolicyRequest Request, long UserID);
        PublicAPIKeyPolicyResponse GetPublicAPIKeyPolicy();
        #endregion

        #region Dashboard method
        APIRequestStatisticsCountResponse APIRequestStatisticsCount();
        UserWiseAPIReqCounResponse UserWiseAPIReqCount(long Pagesize, long PageNo, short status);
        FrequentUseAPIRespons GetFrequentUseAPI(long Pagesize, string FromDate, string ToDate);
        MostActiveIPAddressResponse MostActiveIPAddress(long Pagesize, string FromDate, string ToDate);
        APIPlanConfigurationCountResponse APIPlanConfigurationCount();
        HTTPErrorsReportResponse GetHTTPErrorReport(long Pagesize, long PageNo, string FromDate, string ToDate, long? ErrorCode);
        MostActiveIPWiseReportResponse GetIPAddressWiseReport(long Pagesize, long PageNo, string FromDate, string ToDate, string IPAddress, long MemberID);
        FrequentUseAPIWiseReportResponse FrequentUseAPIReport(long Pagesize, long PageNo, string FromDate, string ToDate, long MemberID);
        #endregion
    }
}
