using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.FeedConfiguration;
using Worldex.Core.ViewModels.APIConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Worldex.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIConfigurationController : ControllerBase
    {
        private readonly ILogger<APIConfigurationController> _logger;
        private readonly IAPIConfigurationService _APIConfigurationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserSubscribeAPIPlanProcess _subscribeAPIPlanProcess;
        private readonly IFrontTrnService _frontTrnService;
        private readonly IBackOfficeAPIConfigService _BackOfficeAPIConfigService;

        public APIConfigurationController(ILogger<APIConfigurationController> logger, IAPIConfigurationService APIConfigurationService,
            UserManager<ApplicationUser> userManager, IUserSubscribeAPIPlanProcess subscribeAPIPlanProcess,IFrontTrnService frontTrnService,
            IBackOfficeAPIConfigService BackOfficeAPIConfigService)
        {
            _logger = logger;
            _APIConfigurationService = APIConfigurationService;
            _userManager = userManager;
            _subscribeAPIPlanProcess = subscribeAPIPlanProcess;
            _frontTrnService = frontTrnService;
            _BackOfficeAPIConfigService = BackOfficeAPIConfigService;
        }


        #region APIPlan
        [HttpGet("ViewAPIPlanDetail")]
        [Authorize]
        public async Task<ActionResult<ViewAPIPlanDetailResponse>> ViewAPIPlanDetail()
        {
            try
            {
                ApplicationUser user =await _userManager.GetUserAsync(HttpContext.User);
                return _APIConfigurationService.ViewAPIPlanDetail(user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("SubscribeAPIPlan")]
        [Authorize]
        public async Task<ActionResult<BizResponseClass>> SubscribeAPIPlan([FromBody] UserAPIPlanSubscribeRequest Request)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                return await _subscribeAPIPlanProcess.UserAPIPlanSubscribe(Request, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ViewUserActivePlan")]
        [Authorize]
        public async Task<ActionResult<ViewActivePlanDetailResponse>> ViewUserActivePlan()
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                return _APIConfigurationService.ViewUserActivePlan(user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("AutoRenewAPIPlan")]
        [Authorize]
        public async Task<ActionResult<BizResponseClass>> AutoRenewAPIPlan([FromBody] AutoRenewPlanRequest Request)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                return await _subscribeAPIPlanProcess.APIPlanAutoRenewProcess(Request, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetAutoRenewPlanDetail")]
        [Authorize]
        public async Task<ActionResult<GetAutoRenewDetailResponse>> GetAutoRenewPlanDetail()
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                return _APIConfigurationService.GetAutoRenewDetail(user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("StopAutoRenewPlan")]
        [Authorize]
        public async Task<ActionResult<BizResponseClass>> StopAutoRenewPlan([FromBody]StopAutoRenewRequest Request)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                // return _APIConfigurationService.StopAutoRenew(Request, user.Id);
                return await _subscribeAPIPlanProcess.StopAutoRenewProcess(Request, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ManualRenewAPIPlan")]
        [Authorize]
        public async Task<ActionResult<BizResponseClass>> ManualRenewAPIPlan([FromBody]ManualRenewAPIPlanRequest Request)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                return await _subscribeAPIPlanProcess.ManualRenewAPIPlan(Request, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("UserAPIPlanSubscribeHistory")]
        [Authorize]
        public async Task<ActionResult<UserAPIPlanHistoryResponse>>  UserAPIPlanSubscribeHistory([FromBody]UserAPIPlanHistoryRequest Request)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (Request.PageNo == null)
                    Request.PageNo = 0;
                if(Request.Pagesize==null)
                    Request.Pagesize= Helpers.PageSize;



                return  _APIConfigurationService.GetUserPlanHistory(Request, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region APIKeys

        [HttpPost("GenerateAPIKey")]
        [Authorize]
        public async Task<ActionResult<GenerateAPIKeyResponse>> GenerateAPIKey([FromBody]GenerateAPIKeyRequest Request)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                return await _APIConfigurationService.GenerateAPIKey(Request, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("UpdateAPIKey/{APIKeyID}")]
        [Authorize]
        public async Task<ActionResult<BizResponseClass>> UpdateAPIKey(long APIKeyID)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                return _APIConfigurationService.UpdateAPIKey(APIKeyID, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpDelete("DeleteAPIKey/{APIKeyID}")]
        [Authorize]
        public async Task<ActionResult<BizResponseClass>> DeleteAPIKey(long APIKeyID)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                return _APIConfigurationService.DeleteAPIKey(APIKeyID, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetAPIKeyList/{PlanID}")]
        [Authorize]
        public async Task<ActionResult<APIKeyListPResponse>> GetAPIKeyList(long PlanID)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                return _APIConfigurationService.GetAPIKeyList(PlanID, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetAPIKeyByID/{KeyID}")]
        [Authorize]
        public async Task<ActionResult<APIKeyListPResponseV2>> GetAPIKeyByID(long KeyID)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                return _APIConfigurationService.GetAPIKeyByID(KeyID, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }



        #endregion

        #region IPWhitelist
        [HttpPost("IPWhitelist")]
        [Authorize]
        public async Task<ActionResult<BizResponseClass>> IPWhitelist(IPWhiteListRequest Request)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                return _APIConfigurationService.WhitelistIP(Request, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetWhitelistIP/{PlanID}")]
        [Authorize]
        public async Task<ActionResult<WhitelistIPListResponse>> GetWhitelistIP(long PlanID,long? KeyID)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                return _APIConfigurationService.GetWhitelistIP(PlanID, user.Id, KeyID);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpDelete("DeleteWhitelistIP/{IPId}")]
        [Authorize]
        public async Task<ActionResult<BizResponseClass>> DeleteWhitelistIP(long IPId)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                return _APIConfigurationService.DeleteWhitelistIP(IPId, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region CustomeLimit

        [HttpPost("SetUserAPICustomLimit")]
        [Authorize]
        public async Task<ActionResult<BizResponseClass>> SetUserAPICustomLimit(UserAPICustomeLimitRequest Request)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                return _APIConfigurationService.SetUserAPICustomeLimit(Request, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("UpdateUserAPICustomLimit")]
        [Authorize]
        public async Task<ActionResult<BizResponseClass>> UpdateUserAPICustomLimit(UserAPICustomeLimitRequest Request)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                return _APIConfigurationService.UpdateUserAPICustomeLimit(Request, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetUserAPICustomLimit/{SubscribeID}")]
        [Authorize]
        public async Task<ActionResult<UserAPICustomeLimitResponse>> GetUserAPICustomLimit(long SubscribeID)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                return _APIConfigurationService.GetCustomeLimit(SubscribeID, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("SetDefaultAPILimits/{LimitID}")]
        [Authorize]
        public ActionResult<BizResponseClass> SetDefaultAPILimits(long LimitID)
        {
            try
            {
                return _APIConfigurationService.SetDefaultAPILimits(LimitID);
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
        public ActionResult<UserWiseAPIReqCounResponse> GetUserWiseAPIRequestCount(short Status,long? PageNo,long? Pagesize)
        {
            try
            {
                if (PageNo == null)
                    PageNo = 0;
                if (Pagesize == null)
                    Pagesize = Helpers.PageSize;
                return _BackOfficeAPIConfigService.UserWiseAPIReqCount(Convert.ToInt64(Pagesize),Convert.ToInt64(PageNo), Status);
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
        #endregion
    }
}