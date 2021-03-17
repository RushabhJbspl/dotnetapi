using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.ViewModels.Transaction.BackOfficeCount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.API
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

        //[HttpGet("GetOrderSummaryCount")]
        //public ActionResult<OrderSummaryCountResponse> GetOrderSummaryCount()
        //{
        //    try
        //    {
        //        OrderSummaryCountResponse Response = new OrderSummaryCountResponse();

        //        Response.Response = _backOfficeCountTrnService.GetOrderSummaryCount();
        //        Response.ErrorCode = enErrorCode.Success;
        //        Response.ReturnCode = enResponseCode.Success;

        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

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

        //[HttpGet("GetProfitSummaryCount")]
        //public ActionResult<GetProfitSummaryCountResponse> GetProfitSummaryCount()
        //{
        //    try
        //    {
        //        GetProfitSummaryCountResponse Response = new GetProfitSummaryCountResponse();

        //        Response.Response = _backOfficeCountTrnService.GetProfitSummaryCount();
        //        Response.ErrorCode = enErrorCode.Success;
        //        Response.ReturnCode = enResponseCode.Success;

        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

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

        #region Arbitrage Method

        [HttpGet("GetActiveTradeUserCountArbitrage/{Status}")]
        public ActionResult<ActiveTradeUserCountResponse> GetActiveTradeUserCountArbitrage(string Status)
        {
            ActiveTradeUserCountResponse Response = new ActiveTradeUserCountResponse();
            try
            {
                if(!Status.Equals("All") && !Status.Equals("Active") && !Status.Equals("SuccessCancel"))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidStatusType;
                    Response.ReturnMsg = "Fail";
                    return Ok(Response);
                }
                Response.Response = _backOfficeCountTrnService.GetActiveTradeUserCountArbitrage(Status);
                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Success";
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetConfigurationCountArbitrage")]
        public ActionResult<ConfigurationCountResponse> GetConfigurationCountArbitrage()
        {
            try
            {
                ConfigurationCountResponse Response = new ConfigurationCountResponse();
                Response = _backOfficeCountTrnService.GetConfigurationCountArbitrage();
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetTradeSummaryCountArbitrage")]
        public ActionResult<TradeSummaryCountResponse> GetTradeSummaryCountArbitrage()
        {
            try
            {
                TradeSummaryCountResponse Response = new TradeSummaryCountResponse();
                Response = _backOfficeCountTrnService.GetTradeSummaryCountArbitrage();
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [AllowAnonymous]
        [HttpGet("GetTradeUserMarketTypeCountArbitrage/{Type}/{Status}")]
        public ActionResult<TradeUserMarketTypeCountResponse> GetTradeUserMarketTypeCountArbitrage(string Type, string Status)
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
                if (!Status.Equals("All") && !Status.Equals("Active") && !Status.Equals("SuccessCancel"))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidStatusType;
                    Response.ReturnMsg = "Fail";
                    return Ok(Response);
                }
                Response.Response = _backOfficeCountTrnService.GetTradeUserMarketTypeCountArbitrage(Type, Status);
                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnCode = enResponseCode.Success;

                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion
    }
}