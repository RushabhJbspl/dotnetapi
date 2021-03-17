using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.Transaction.BackOfficeCount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackofficeWorldex.Web.API
{
    [Route("api/[controller]")]
    [Authorize] //for test only //komal 10-06-2019 make authorize
    public class TransactionBackOfficeCountController : ControllerBase
    {
        private readonly IBackOfficeCountTrnService _backOfficeCountTrnService;

        public TransactionBackOfficeCountController(IBackOfficeCountTrnService backOfficeCountTrnService)
        {
            _backOfficeCountTrnService = backOfficeCountTrnService;
        }

        #region Count and Margin Method
        [HttpGet("GetActiveTradeUserCount")]
        public ActionResult<ActiveTradeUserCountResponse> GetActiveTradeUserCount(short IsMargin = 0)
        {
            try
            {
                ActiveTradeUserCountResponse Response = new ActiveTradeUserCountResponse();

                Response.Response = _backOfficeCountTrnService.GetActiveTradeUserCount(IsMargin);
                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnCode = enResponseCode.Success;

                return Ok(Response);
            }
            catch(Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetConfigurationCount")]
        public ActionResult<ConfigurationCountResponse> GetConfigurationCount(short IsMargin=0)
        {
            try
            {
                ConfigurationCountResponse Response = new ConfigurationCountResponse();
                Response = _backOfficeCountTrnService.GetConfigurationCount(IsMargin);
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetLedgerCount")]
        public ActionResult<LedgerCountResponse> GetLedgerCount(short IsMargin = 0)
        {
            try
            {
                LedgerCountResponse response = new LedgerCountResponse();
                response = _backOfficeCountTrnService.GetLedgerCount(IsMargin);                 
                return response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpGet("GetTradeSummaryCount")]
        public ActionResult<TradeSummaryCountResponse> GetTradeSummaryCount(short IsMargin = 0)
        {
            try
            {
                TradeSummaryCountResponse Response = new TradeSummaryCountResponse();
                Response= _backOfficeCountTrnService.GetTradeSummaryCount(IsMargin);
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetTradeUserMarketTypeCount/{Type}")]
        public ActionResult<TradeUserMarketTypeCountResponse> GetTradeUserMarketTypeCount(string Type, short IsMargin = 0)
        {
            try
            {
                TradeUserMarketTypeCountResponse Response = new TradeUserMarketTypeCountResponse();

                if (!Type.Equals("Today") && !Type.Equals("Week") && !Type.Equals("Month") && !Type.Equals("Year"))
                {
                    Response.ErrorCode = enErrorCode.InvalidTimeType;
                    Response.ReturnCode = enResponseCode.Fail;
                    return Response;
                }

                Response.Response = _backOfficeCountTrnService.GetTradeUserMarketTypeCount(Type, IsMargin);
                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnCode = enResponseCode.Success;

                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetReportCount")]
        public ActionResult<TransactionReportCountResponse> GetReportCount()
        {
            try
            {
                return _backOfficeCountTrnService.TransactionReportCount();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

    }
}