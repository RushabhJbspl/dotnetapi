using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces.FeedConfiguration;
using CleanArchitecture.Core.ViewModels.Configuration.FeedConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Web.API.Configuration
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExchangeFeedConfigurationController : ControllerBase
    {
        private readonly ILogger<ExchangeFeedConfigurationController> _logger;
        private readonly IExchangeFeedConfiguration _exchangeFeedConfiguration;
        public ExchangeFeedConfigurationController(ILogger<ExchangeFeedConfigurationController> logger, IExchangeFeedConfiguration exchangeFeedConfiguration)
        {
            _logger = logger;
            _exchangeFeedConfiguration = exchangeFeedConfiguration;
        }

        [HttpGet("GetSocketMethods")]
        public ActionResult<SocketMethodResponse> GetSocketMethods()
        {
            SocketMethodResponse Response = new SocketMethodResponse();
            try
            {
                return _exchangeFeedConfiguration.GetSocketMethods();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetExchangeFeedLimitType")]
        public ActionResult<ExchangeLimitTypeResponse> GetExchangeFeedLimitType()
        {
            ExchangeLimitTypeResponse Response = new ExchangeLimitTypeResponse();
            try
            {
                return _exchangeFeedConfiguration.GetExchangeFeedLimitType();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetFeedLimitList")]
        public ActionResult<SocketFeedLimitsResponse> GetFeedLimitList()
        {
            SocketFeedLimitsResponse Response = new SocketFeedLimitsResponse();
            try
            {
                return _exchangeFeedConfiguration.GetAllFeedConfigurationLimit();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetFeedLimitListV2")]
        public ActionResult<SocketFeedLimitsListResponse> GetFeedLimitListV2()
        {
            SocketFeedLimitsListResponse Response = new SocketFeedLimitsListResponse();
            try
            {
                return _exchangeFeedConfiguration.GetSocketFeedLimitsLists();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("AddExchangeFeedLimit")]
        public ActionResult<BizResponseClass> AddExchangeFeedLimit([FromBody]SocketFeedLimitsRequest Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                return _exchangeFeedConfiguration.AddFeedConfigurationLimit(Request,2);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("UpdateExchangeFeedLimit")]
        public ActionResult<BizResponseClass> UpdateExchangeFeedLimit([FromBody]SocketFeedLimitsRequest Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                return _exchangeFeedConfiguration.UpdateFeedConfigurationLimit(Request, 2);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        

        [HttpGet("GetAllFeedConfiguration")]
        public ActionResult<SocketFeedConfigResponse> GetAllFeedConfiguration()
        {
            SocketFeedConfigResponse Response = new SocketFeedConfigResponse();
            try
            {
                return _exchangeFeedConfiguration.GetAllFeedConfiguration();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("AddFeedConfiguration")]
        public ActionResult<BizResponseClass> AddFeedConfiguration([FromBody]SocketFeedConfigurationRequest Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                return _exchangeFeedConfiguration.AddSocketFeedConfig(Request,2);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("UpdateFeedConfiguration")]
        public ActionResult<BizResponseClass> UpdateFeedConfiguration([FromBody]SocketFeedConfigurationRequest Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                return _exchangeFeedConfiguration.UpdateSocketFeedConfig(Request, 2);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
    }
}