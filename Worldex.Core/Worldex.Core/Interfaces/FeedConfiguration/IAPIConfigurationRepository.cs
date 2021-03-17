using Worldex.Core.ViewModels.APIConfiguration;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.FeedConfiguration
{
    public interface IAPIConfigurationRepository
    {
        #region API Plan
        //List<ViewAPIPlanDetailResponseInfo> ViewAPIPlanDetail();
        //List<APIPlanMasterResponseInfo> ViewAPIPlanDetailBackOffice();
        List<PlanMethodsQryResponse> GetPlanMethods(long LimitID = 0);
        void SetDisablePlanMethodsStatus(long PlanID, long UserID, long limitId = 0);
        void DisablePlanService(long PlanID, long UserID, long SubscribeID, string Keys);
        void DisableAPIMethodsConfiguration(long MethodId);
        void ExpireOldRenewPlan(long PlanID, long UserID);
        ViewActivePlanDetailInfo ViewUserActivePlan(long UserID);
        GetAutoRenewDetailInfo GetAutoRenewDetail(long UserID);
        List<APIMethodConfigListQryRes> GetAPIMethodConfigListBK();
        List<GetKeyWiseIPQryRes> GetKeyWiseIPList(long UserID, long PlanID);
        #endregion

        #region enableDisable Key
        void CancelAllPendingPlan(long PlanID, long UserID,short Status);
        void DisableAllPlanAPIKey(long PlanID, long UserID);
        void EnableAllPlanAPIKey(long PlanID, long UserID);
        void SetDefaultConfigurationMethod(long PlanID, long LimitID);
        void DisableWhiteListIPConfigurationIPwise(long IPID, long UserID);
        void DisableWhiteListIPConfigurationKeywise(long APIKeyID, long UserID);
        void ExpireAllPlanAPIKey(long PlanID, long UserID);
        #endregion

        #region Back Office History Method
        List<UserAPIPlanHistoryQryRes> GetUserAPISubscribeHistory(long UserID, long? PlanID, short? PaymentStatus);
        APIPlanUserCountResponse GetAPIPlanUserCount(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? Status, long? PlanID);
        UserSubscribeHistoryBKResponse GetUserSubscribeHistoryBK(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? Status, long? PlanID);
        ViewAPIPlanConfigHistoryResponse ViewAPIPlanConfigHistoryBK(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? PlanID);
        ViewPublicAPIKeysResponse ViewPublicAPIKeysBK(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? Status, long? PlanID);
        #endregion

        #region Dashboard Method
        APIRequestStatisticsCountResponse APIRequestStatisticsCount();
        UserWiseAPIReqCounResponse UserWiseAPIReqCount(long Pagesize, long PageNo, short status);
        FrequentUseAPIRespons GetFrequentUseAPI(long Pagesize,string FromDate, string ToDate);
        MostActiveIPAddressResponse MostActiveIPAddress(long Pagesize, string FromDate, string ToDate);
        APIPlanConfigurationCountResponse APIPlanConfigurationCount();
        HTTPErrorsReportResponse GetHTTPErrorReport(long Pagesize, long PageNo, string FromDate, string ToDate, long? ErrorCode);
        MostActiveIPWiseReportResponse GetIPAddressWiseReport(long Pagesize, long PageNo, string FromDate, string ToDate, string IPAddress, long MemberID);
        FrequentUseAPIWiseReportResponse FrequentUseAPIReport(long Pagesize, long PageNo, string FromDate, string ToDate, long MemberID);
        #endregion
    }
}
