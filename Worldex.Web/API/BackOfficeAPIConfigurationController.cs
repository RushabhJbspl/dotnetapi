using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Helpers;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Interfaces.FeedConfiguration;
using CleanArchitecture.Core.ViewModels.APIConfiguration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackOfficeAPIConfigurationController : ControllerBase
    {
        private readonly ILogger<BackOfficeAPIConfigurationController> _logger;
        private readonly IBackOfficeAPIConfigService _BackOfficeAPIConfigService;
        private readonly IFrontTrnService _frontTrnService;

        public BackOfficeAPIConfigurationController(ILogger<BackOfficeAPIConfigurationController> logger, IBackOfficeAPIConfigService BackOfficeAPIConfigService,
            IFrontTrnService frontTrnService)
        {
            _logger = logger;
            _BackOfficeAPIConfigService = BackOfficeAPIConfigService;
            _frontTrnService = frontTrnService;
        }

        #region APIPlanMaster
        [HttpPost("AddAPIPlan")]
        public ActionResult<BizResponseClass> AddAPIPlan([FromBody]APIPlanMasterRequest Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                return _BackOfficeAPIConfigService.AddAPIPlane(Request, 12);

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("UpdateAPIPlan")]
        public ActionResult<BizResponseClass> UpdateAPIPlan([FromBody]APIPlanMasterRequest Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                return _BackOfficeAPIConfigService.UpdatePlane(Request, 12);

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetAPIPlan")]
        public ActionResult<APIPlanMasterResponse> GetAPIPlan()
        {
            APIPlanMasterResponse Response = new APIPlanMasterResponse();
            try
            {
                return _BackOfficeAPIConfigService.GetAPIPlan(); 
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region APIPlanDetail
        //[HttpGet("GetAPIPlanV2")]
        //public ActionResult<APIPlanMasterResponseV2> GetAPIPlanV2()
        //{
        //    APIPlanMasterResponseV2 Response = new APIPlanMasterResponseV2();
        //    try
        //    {
        //        return _BackOfficeAPIConfigService.GetAPIPlanV2();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}
        //[HttpPost("AddAPIPlanDetail")]
        //public ActionResult<BizResponseClass> AddAPIPlanDetail([FromBody]APIPlanDetailRequest Request)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        return _BackOfficeAPIConfigService.AddAPIPlanDetail(Request, 2);

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}
        //[HttpPost("UpdateAPIPlanDetail")]
        //public ActionResult<BizResponseClass> UpdateAPIPlanDetail([FromBody]APIPlanDetailRequest Request)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        return _BackOfficeAPIConfigService.UpdateAPIPlanDetail(Request, 2);

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}
        //[HttpGet("GetAPIPlanDetails")]
        //public ActionResult<APIPlanDetailResponse> GetAPIPlanDetails()
        //{
        //    APIPlanDetailResponse Response = new APIPlanDetailResponse();
        //    try
        //    {
        //        return _BackOfficeAPIConfigService.GetAPIPlanDetails();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}
        #endregion

        #region RestMethods
        [HttpPost("AddAPIMethod")]
        public ActionResult<BizResponseClass> AddAPIMethod([FromBody]APIMethodsRequest Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                return _BackOfficeAPIConfigService.AddAPIMethod(Request, 12);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("UpdateAPIMethod")]
        public ActionResult<BizResponseClass> UpdateAPIMethod([FromBody]APIMethodsRequest2 Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                return _BackOfficeAPIConfigService.UpdateAPIMethod(Request, 12);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetAPIMethods")]
        public ActionResult<APIMethodResponse> GetAPIMethods()
        {
            APIMethodResponse Response = new APIMethodResponse();
            try
            {
                return _BackOfficeAPIConfigService.GetAPIMethods();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetRestMethodsReadOnly")]
        public ActionResult<APIMethodResponseV2> GetRestMethodsReadOnly()
        {
            APIMethodResponseV2 Response = new APIMethodResponseV2();
            try
            {
                return _BackOfficeAPIConfigService.GetRestMethodsReadOnly();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetRestMethodsFullAccess")]
        public ActionResult<APIMethodResponseV2> GetRestMethodsFullAccess()
        {
            APIMethodResponseV2 Response = new APIMethodResponseV2();
            try
            {
                return _BackOfficeAPIConfigService.GetRestMethodsFullAccess();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("ReloadAPIMethodConfiguration")]
        public ActionResult ReloadAPIMethodConfiguration()
        {
            try
            {
                _BackOfficeAPIConfigService.ReloadAPIPlanMethodConfiguration();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetSystemRestMethods")]
        public ActionResult<RestMethodResponse> GetSystemRestMethods()
        {
            try
            {
                return _BackOfficeAPIConfigService.GetRestMethods();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("ReloadSystemRestMethods")]
        public ActionResult ReloadSystemRestMethods()
        {
            try
            {
                _BackOfficeAPIConfigService.ReloadRestMethods();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region HistoryMethods
        [HttpPost("ViewAPIPlanUserCount")]
        public ActionResult<APIPlanUserCountResponse> ViewAPIPlanUserCount([FromBody]APIPlanUserCountRequest Request)
        {
            APIPlanUserCountResponse Response = new APIPlanUserCountResponse();
            try
            {
                if (Request.PageNo == null)
                    Request.PageNo = 0;
                if (Request.Pagesize == null || Request.Pagesize == 0)
                    Request.Pagesize = Helpers.PageSize;

                if (!string.IsNullOrEmpty(Request.FromDate))
                {
                    if (string.IsNullOrEmpty(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                }
                return _BackOfficeAPIConfigService.GetAPIPlanUserCount(Convert.ToInt64(Request.Pagesize),Convert.ToInt64( Request.PageNo), Request.FromDate, Request.ToDate, Request.UserID, Request.Status, Request.PlanID);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ViewUserSubscriptionHistory")]
        public ActionResult<UserSubscribeHistoryBKResponse> ViewUserSubscriptionHistory([FromBody]UserSubscribeHistoryBKRequest Request)
        {
            UserSubscribeHistoryBKResponse Response = new UserSubscribeHistoryBKResponse();
            try
            {
                if (Request.PageNo == null)
                    Request.PageNo = 0;
                if (Request.Pagesize == null || Request.Pagesize==0)
                    Request.Pagesize = Helpers.PageSize;

                if (!string.IsNullOrEmpty(Request.FromDate))
                {
                    if (string.IsNullOrEmpty(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                }

                return _BackOfficeAPIConfigService.GetUserSubscribeHistoryBK(Convert.ToInt64(Request.Pagesize), Convert.ToInt64(Request.PageNo), Request.FromDate, Request.ToDate, Request.UserID, Request.Status, Request.PlanID);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("EnableDisableAPIPlan/{PlanID}/{Status}")]
        public ActionResult<BizResponseClass> EnableDisableAPIPlan(long PlanID,short Status, short? AllowAPIKey)
        {
            try
            {
                return _BackOfficeAPIConfigService.EnableDisableAPIPlan(PlanID,Convert.ToInt16(AllowAPIKey), Status,12);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ViewAPIPlanConfigurationHistory")]
        public ActionResult<ViewAPIPlanConfigHistoryResponse> ViewAPIPlanConfigurationHistory([FromBody]ViewAPIPlanConfigHistoryRequest Request)
        {
            ViewAPIPlanConfigHistoryResponse Response = new ViewAPIPlanConfigHistoryResponse();
            try
            {
                if (Request.PageNo == null)
                    Request.PageNo = 0;
                if (Request.Pagesize == null || Request.Pagesize == 0)
                    Request.Pagesize = Helpers.PageSize;

                if (!string.IsNullOrEmpty(Request.FromDate))
                {
                    if (string.IsNullOrEmpty(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                }
                return _BackOfficeAPIConfigService.ViewAPIPlanConfiguration(Convert.ToInt64(Request.Pagesize), Convert.ToInt64(Request.PageNo), Request.FromDate, Request.ToDate, Request.UserID, Request.PlanID);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ViewPublicAPIKeys")]
        public ActionResult<ViewPublicAPIKeysResponse> ViewPublicAPIKeys([FromBody]ViewPublicAPIKeysRequest Request)
        {
            ViewPublicAPIKeysResponse Response = new ViewPublicAPIKeysResponse();
            try
            {
                if (Request.PageNo == null)
                    Request.PageNo = 0;
                if (Request.Pagesize == null || Request.Pagesize == 0)
                    Request.Pagesize = Helpers.PageSize;

                if (!string.IsNullOrEmpty(Request.FromDate))
                {
                    if (string.IsNullOrEmpty(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                }
                return _BackOfficeAPIConfigService.ViewPublicAPIKeysBK(Convert.ToInt64(Request.Pagesize), Convert.ToInt64(Request.PageNo), Request.FromDate, Request.ToDate, Request.UserID, Request.Status, Request.PlanID);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region APIKeyPolicy
        [HttpPost("UpdatePublicAPIKeyPolicy")]
        public ActionResult<BizResponseClass> UpdatePublicAPIKeyPolicy([FromBody]PublicAPIKeyPolicyRequest Request)
        {
            try
            {
                return _BackOfficeAPIConfigService.UpdatePublicAPIKeyPolicy(Request, 12);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetPublicAPIKeyPolicy")]
        public ActionResult<PublicAPIKeyPolicyResponse> GetPublicAPIKeyPolicy()
        {
            try
            {
                return _BackOfficeAPIConfigService.GetPublicAPIKeyPolicy();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region API Request Statistics dashboard
        [HttpGet("GetAPIRequestStatisticsCount")]
        public ActionResult<APIRequestStatisticsCountResponse> GetAPIRequestStatisticsCount()
        {
            try
            {
                return _BackOfficeAPIConfigService.APIRequestStatisticsCount();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetUserWiseAPIRequestCount/{Status}")]
        public ActionResult<UserWiseAPIReqCounResponse> GetUserWiseAPIRequestCount(short Status, long? PageNo, long? Pagesize)
        {
            try
            {
                if (PageNo == null)
                    PageNo = 0;
                if (Pagesize == null || Pagesize == 0)
                    Pagesize = Helpers.PageSize;
                return _BackOfficeAPIConfigService.UserWiseAPIReqCount(Convert.ToInt64(Pagesize), Convert.ToInt64(PageNo), Status);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetFrequentUseAPI")]
        public ActionResult<FrequentUseAPIRespons> GetFrequentUseAPI(long? RecCount, string FromDate = "", string ToDate = "")
        {
            FrequentUseAPIRespons Response = new FrequentUseAPIRespons();
            try
            {
                if (RecCount == null)
                    RecCount = 10;
                if (!string.IsNullOrEmpty(FromDate))
                {
                    if (string.IsNullOrEmpty(ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                }
                return _BackOfficeAPIConfigService.GetFrequentUseAPI(Convert.ToInt64(RecCount), FromDate, ToDate);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("MostActiveIPAddress")]
        public ActionResult<MostActiveIPAddressResponse> MostActiveIPAddress(long? RecCount, string FromDate = "", string ToDate = "")
        {
            MostActiveIPAddressResponse Response = new MostActiveIPAddressResponse();
            try
            {
                if (RecCount == null)
                    RecCount = 10;
                if (!string.IsNullOrEmpty(FromDate))
                {
                    if (string.IsNullOrEmpty(ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                }
                return _BackOfficeAPIConfigService.MostActiveIPAddress(Convert.ToInt64(RecCount), FromDate, ToDate);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("APIPlanConfigurationCount")]
        public ActionResult<APIPlanConfigurationCountResponse> APIPlanConfigurationCount()
        {
            try
            {
                return _BackOfficeAPIConfigService.APIPlanConfigurationCount();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetHttpErrorCodeReport")]
        public ActionResult<HTTPErrorsReportResponse> GetHttpErrorCodeReport([FromBody]HTTPErrorsReportRequest Request)
        {
            HTTPErrorsReportResponse Response = new HTTPErrorsReportResponse();
            try
            {
                if (Request.PageNo == null)
                    Request.PageNo = 0;
                if (Request.Pagesize == null || Request.Pagesize == 0)
                    Request.Pagesize = Helpers.PageSize;

                if (!string.IsNullOrEmpty(Request.FromDate))
                {
                    if (string.IsNullOrEmpty(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                }
                return _BackOfficeAPIConfigService.GetHTTPErrorReport(Convert.ToInt64(Request.Pagesize), Convert.ToInt64(Request.PageNo), Request.FromDate, Request.ToDate, Request.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetIPAddressWiseReport")]
        public ActionResult<MostActiveIPWiseReportResponse> GetIPAddressWiseReport([FromBody]MostActiveIPWiseReportRequest Request)
        {
            MostActiveIPWiseReportResponse Response = new MostActiveIPWiseReportResponse();
            try
            {
                if (Request.PageNo == null)
                    Request.PageNo = 0;
                if (Request.Pagesize == null || Request.Pagesize == 0)
                    Request.Pagesize = Helpers.PageSize;
                if (Request.MemberID == null)
                    Request.MemberID = 0;

                if (!string.IsNullOrEmpty(Request.FromDate))
                {
                    if (string.IsNullOrEmpty(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                }
                return _BackOfficeAPIConfigService.GetIPAddressWiseReport(Convert.ToInt64(Request.Pagesize), Convert.ToInt64(Request.PageNo), Request.FromDate, Request.ToDate, Request.IPAddress,Convert.ToInt64(Request.MemberID));
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetAPIWiseReport")]
        public ActionResult<FrequentUseAPIWiseReportResponse> GetAPIWiseReport([FromBody]FrequentUseAPIWiseReportRequest Request)
        {
            FrequentUseAPIWiseReportResponse Response = new FrequentUseAPIWiseReportResponse();
            try
            {
                if (Request.PageNo == null)
                    Request.PageNo = 0;
                if (Request.Pagesize == null || Request.Pagesize == 0)
                    Request.Pagesize = Helpers.PageSize;
                if (Request.MemberID == null)
                    Request.MemberID = 0;

                if (!string.IsNullOrEmpty(Request.FromDate))
                {
                    if (string.IsNullOrEmpty(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        Response.ReturnMsg = "Fail";
                        return Response;
                    }
                }
                return _BackOfficeAPIConfigService.FrequentUseAPIReport(Convert.ToInt64(Request.Pagesize), Convert.ToInt64(Request.PageNo), Request.FromDate, Request.ToDate, Convert.ToInt64(Request.MemberID));
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion
    }
}