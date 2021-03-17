using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.ViewModels.Configuration;
using CleanArchitecture.Core.ViewModels.Transaction;
using CleanArchitecture.Core.ViewModels.Transaction.BackOffice;
using CleanArchitecture.Infrastructure.Interfaces;
using CleanArchitecture.Web.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]")]
    //[ApiController] 
    [Authorize] //for test only //komal 10-06-2019 make authorize
    public class TransactionBackOfficeController : Controller
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly IBackOfficeTrnService _backOfficeService;
        private readonly IFrontTrnService _frontTrnService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBasePage _basePage;

        public TransactionBackOfficeController(
            ILogger<TransactionController> logger,
            IBackOfficeTrnService backOfficeService,
            IFrontTrnService frontTrnService,
            UserManager<ApplicationUser> userManager,
            IBasePage basePage)
        {
            _logger = logger;
            _backOfficeService = backOfficeService;
            _frontTrnService = frontTrnService;
            _userManager = userManager;
            _basePage = basePage;
        }


        [HttpPost("TradingSummary")]
        public async Task<ActionResult<TradingSummaryResponse>> TradingSummary([FromBody]TradingSummaryRequest request)
        {
            TradingSummaryResponse Response = new TradingSummaryResponse();
            Int16 trnType = 999, marketType = 999;//, status = 999;
            long PairId = 999;
            //string sCondition = "1=1 ";
            try
            {
                if (!string.IsNullOrEmpty(request.Pair))
                {
                    if (!_frontTrnService.IsValidPairName(request.Pair))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                    PairId = _frontTrnService.GetPairIdByName(request.Pair, request.IsMargin);
                    if (PairId == 0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                }
                if (!string.IsNullOrEmpty(request.Trade))
                {
                    trnType = _frontTrnService.IsValidTradeType(request.Trade);
                    if (trnType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InValidTrnType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.MarketType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.MarketType);
                    if (marketType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidMarketType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    if (string.IsNullOrEmpty(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        return BadRequest(Response);
                    }
                }
                if (request.Status != 0 && request.Status != 91 && request.Status != 92 && request.Status != 95 && request.Status != 94 && request.Status != 96 && request.Status != 97 && request.Status != 93)// && request .Status != 93)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidStatusType;
                    return Response;
                }

                Response = _backOfficeService.GetTradingSummary(request.MemberID, request.FromDate, request.ToDate, request.TrnNo, request.Status, request.SMSCode, PairId, trnType, marketType, request.PageSize, request.PageNo, request.IsMargin);
                //Response.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //Response.ReturnCode = enResponseCode.InternalError;
                //return Ok(Response);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }
        [HttpPost("TradingSummaryLPWise")]
        public async Task<ActionResult<TradingSummaryLPResponse>> TradingSummaryLPWise([FromBody]TradingSummaryRequest request)
        {
            TradingSummaryLPResponse Response = new TradingSummaryLPResponse();
            Int16 trnType = 999, marketType = 999;//, status = 999;
            long PairId = 999;
            //string sCondition = "1=1 ";
            try
            {
                if (!string.IsNullOrEmpty(request.Pair))
                {
                    if (!_frontTrnService.IsValidPairName(request.Pair))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                    PairId = _frontTrnService.GetPairIdByName(request.Pair);
                    if (PairId == 0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                }
                if (!string.IsNullOrEmpty(request.Trade))
                {
                    trnType = _frontTrnService.IsValidTradeType(request.Trade);
                    if (trnType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InValidTrnType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.MarketType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.MarketType);
                    if (marketType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidMarketType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    if (string.IsNullOrEmpty(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        return BadRequest(Response);
                    }
                }
                if (request.Status != 0 && request.Status != 91 && request.Status != 92 && request.Status != 95 && request.Status != 94 && request.Status != 96 && request.Status != 97 && request.Status != 93)// && request .Status != 93)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidStatusType;
                    return Response;
                }

                Response = _backOfficeService.GetTradingSummaryLP(request.MemberID, request.FromDate, request.ToDate, request.TrnNo, request.Status, request.SMSCode, PairId, trnType, marketType, request.PageSize, request.PageNo, request.LPType);
                //Response.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //Response.ReturnCode = enResponseCode.InternalError;
                //return Ok(Response);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpPost("TradingReconHistory")]
        public async Task<ActionResult<TradingReconHistoryResponse>> TradingReconHistory([FromBody]TradingReconHistoryRequest request)
        {
            TradingReconHistoryResponse Response = new TradingReconHistoryResponse();
            Int16 trnType = 999, marketType = 999;//, status = 999;
            long PairId = 999;
            //string sCondition = "1=1 ";
            try
            {
                if (!string.IsNullOrEmpty(request.Pair))
                {
                    if (!_frontTrnService.IsValidPairName(request.Pair))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                    PairId = _frontTrnService.GetPairIdByName(request.Pair);
                    if (PairId == 0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                }
                if (!string.IsNullOrEmpty(request.Trade))
                {
                    trnType = _frontTrnService.IsValidTradeType(request.Trade);
                    if (trnType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InValidTrnType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.MarketType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.MarketType);
                    if (marketType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidMarketType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    if (string.IsNullOrEmpty(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        return BadRequest(Response);
                    }
                }
                //if (request.Status != 0 && request.Status != 91 && request.Status != 92 && request.Status != 95 && request.Status != 94 && request.Status != 96 && request.Status != 97 && request.Status != 93)// && request .Status != 93)
                //{
                //    Response.ReturnCode = enResponseCode.Fail;
                //    Response.ErrorCode = enErrorCode.InvalidStatusType;
                //    return Response;
                //}

                Response = _backOfficeService.GetTradingReconHistory(request.MemberID, request.FromDate, request.ToDate, request.TrnNo, request.Status, PairId, trnType, marketType, request.PageSize, request.PageNo, request.LPType, request.IsProcessing);
                //Response.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //Response.ReturnCode = enResponseCode.InternalError;
                //return Ok(Response);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //Rita 27-3-19 not used now as V1 added

        //[HttpPost("TradeRecon")]
        //public async Task<IActionResult> TradeRecon([FromBody]TradeReconRequest request)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        var accessToken = await HttpContext.GetTokenAsync("access_token");

        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        if (request.TranNo == 0)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionNo;
        //            Response.ErrorCode = enErrorCode.TradeRecon_InvalidTransactionNo;
        //        }
        //        if(request.ActionType != enTradeReconActionType.CancelMark)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidActionType;
        //            Response.ErrorCode = enErrorCode.TradeRecon_InvalidActionType;
        //        }
        //        else
        //        {
        //            var UserId = user.Id;
        //            var response = _backOfficeService.TradeRecon(request.TranNo, request.ActionMessage, UserId, accessToken);

        //            return Ok(response.Result);
        //        }

        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        // HelperForLog.WriteErrorLog(_basePage.UTC_To_IST(), this.ControllerContext.RouteData.Values["action"].ToString(), this.GetType().Name, ex.ToString());
        //        //return BadRequest();
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        [HttpPost("TradeReconV1")]
        [Authorize]
        public async Task<IActionResult> TradeReconV1([FromBody]TradeReconRequest request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                if (request.TranNo == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionNo;
                    Response.ErrorCode = enErrorCode.TradeRecon_InvalidTransactionNo;
                    return Ok(Response);
                }
                //if (request.ActionType != enTradeReconActionType.CancelMark)
                //{
                //    Response.ReturnCode = enResponseCode.Fail;
                //    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidActionType;
                //    Response.ErrorCode = enErrorCode.TradeRecon_InvalidActionType;
                //    return Ok(Response);
                //}

                var UserId = user.Id;
                Response = await _backOfficeService.TradeReconV1(request.ActionType, request.TranNo, request.ActionMessage, UserId, accessToken);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                // HelperForLog.WriteErrorLog(_basePage.UTC_To_IST(), this.ControllerContext.RouteData.Values["action"].ToString(), this.GetType().Name, ex.ToString());
                //return BadRequest();
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("TransactionChargeSummary")]
        public ActionResult<TransactionChargeResponse> TransactionChargeSummary(TransactionChargeRequest request)
        {
            TransactionChargeResponse Response = new TransactionChargeResponse();
            Int16 trnType = 999;
            try
            {
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    if (string.IsNullOrEmpty(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.Trade))
                {
                    trnType = _frontTrnService.IsValidTradeType(request.Trade);
                    if (trnType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InValidTrnType;
                        return Response;
                    }
                }
                Response = _backOfficeService.ChargeSummary(request.FromDate, request.ToDate, trnType);
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("WithdrawalRecon")]
        public async Task<ActionResult> WithdrawalRecon([FromBody]WithdrawalReconRequest request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                if (request.TrnNo == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionNo;
                    Response.ErrorCode = enErrorCode.TradeRecon_InvalidTransactionNo;
                }
                else
                {
                    var UserId = 20;
                    var response = _backOfficeService.WithdrawalRecon(request, UserId, accessToken);

                    return Ok(response);
                }

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("WithdrawalSummary")]
        public ActionResult<WithdrawalSummaryResponse> WithdrawalSummary([FromBody]WithdrawalSummaryRequest request)
        {
            WithdrawalSummaryResponse Response = new WithdrawalSummaryResponse();
            try
            {
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    if (string.IsNullOrEmpty(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        return BadRequest(Response);
                    }
                }
                if (request.Status != 0 && request.Status != 81 && request.Status != 82 && request.Status != 83 && request.Status != 84 && request.Status != 85 && request.Status != 86)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidStatusType;
                    return Response;
                }
                if (string.IsNullOrEmpty(request.SMSCode))
                {
                    request.SMSCode = "";
                }

                Response = _backOfficeService.GetWithdrawalSummary(request);

                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }


        [HttpPost("PairTradeSummary")]
        public async Task<ActionResult<PairTradeSummaryResponse>> PairTradeSummary([FromBody]PairTradeSummaryRequest request)
        {
            PairTradeSummaryResponse Response = new PairTradeSummaryResponse();
            Int16 marketType = 999, Range = 999;
            long PairId = 999;
            try
            {
                if (!string.IsNullOrEmpty(request.Pair))
                {
                    if (!_frontTrnService.IsValidPairName(request.Pair))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                    PairId = _frontTrnService.GetPairIdByName(request.Pair, request.IsMargin);
                    if (PairId == 0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                }
                if (!string.IsNullOrEmpty(request.MarketType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.MarketType);
                    if (marketType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidMarketType;
                        return Response;
                    }
                }

                if (request.Range != 0)
                {
                    if (request.Range < 0 || request.Range > 6)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InValidRange;
                        return Response;
                    }
                    Range = request.Range;
                }
                Response = _backOfficeService.pairTradeSummary(PairId, marketType, Range, request.IsMargin);
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpPost("TradeSettledHistory")]
        public ActionResult<TradeSettledHistoryResponse> TradeSettledHistory([FromBody]TradeSettledHistoryRequest request)
        {
            try
            {
                Int16 trnType = 999, marketType = 999;//, status = 999;
                long PairId = 999;

                TradeSettledHistoryResponse Response = new TradeSettledHistoryResponse();
                //if (!string.IsNullOrEmpty(request.MemberID.ToString()))
                //    request.MemberID = 0;
                //if (!string.IsNullOrEmpty(request.TrnNo.ToString()))
                //    request.TrnNo = 0;
                if (!string.IsNullOrEmpty(request.PairName))
                {
                    if (!_frontTrnService.IsValidPairName(request.PairName))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return Response;
                    }
                    PairId = _frontTrnService.GetPairIdByName(request.PairName, request.IsMargin);
                    if (PairId == 0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                }
                if (!string.IsNullOrEmpty(request.TrnType))
                {
                    trnType = _frontTrnService.IsValidTradeType(request.TrnType);
                    if (trnType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InValidTrnType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.OrderType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.OrderType);
                    if (marketType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidMarketType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    if (string.IsNullOrEmpty(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        return Response;
                    }
                }
                Response = _backOfficeService.TradeSettledHistory(PageSize: request.PageSize, PageNo: request.PageNo, PairID: PairId, TrnType: trnType, FromDate: request.FromDate, Todate: request.ToDate, OrderType: marketType, MemberID: request.MemberID, TrnNo: request.TrnNo, IsMargin: request.IsMargin);
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetBackOfficeGraphDetail/{Pair}/{Interval}")]
        public ActionResult<GetGraphDetailReponse> GetBackOfficeGraphDetail(string Pair, string Interval, short IsMargin = 0)
        {
            int IntervalTime = 0;
            string IntervalData = "";
            GetGraphDetailReponse Response = new GetGraphDetailReponse();
            try
            {
                if (!_frontTrnService.IsValidPairName(Pair))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidPairName;
                    Response.ReturnMsg = "Fail";
                    return Response;
                }
                long id = _frontTrnService.GetPairIdByName(Pair, IsMargin);
                if (id == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "Fail";
                    return Response;
                }
                _frontTrnService.GetIntervalTimeValue(Interval, ref IntervalTime, ref IntervalData);
                if (IntervalTime == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.Graph_InvalidIntervalTime;
                    Response.ReturnMsg = "Fail";
                    return Response;
                }
                var responsedata = _frontTrnService.GetGraphDetail(id, IntervalTime, IntervalData, IsMargin);
                if (responsedata != null && responsedata.Count != 0)
                {
                    Response.response = responsedata;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = "Success";
                }
                else
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "Fail";
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #region TopGainer And TopLooser BackOffice
        [HttpGet("GetTopGainerPair/{Type}")]
        public ActionResult<TopLooserGainerPairDataResponse> GetTopGainerPair(int Type, short IsMargin = 0)
        {
            try
            {
                //Uday 01-01-2019  Top Gainer Pair Data give with Different Filteration
                TopLooserGainerPairDataResponse Response = new TopLooserGainerPairDataResponse();

                if (Type == Convert.ToInt32(EnTopLossGainerFilterType.VolumeWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangePerWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.LTPWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangeValueWise))
                {
                    var Data = _backOfficeService.GetTopGainerPair(Type, IsMargin);

                    if (Data.Count != 0)
                    {
                        Response.Response = Data;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ErrorCode = enErrorCode.Success;
                        Response.ReturnMsg = "Success";
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.NoDataFound;
                        Response.ReturnMsg = "Fail";
                    }
                }
                else
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InValidTopLossGainerFilterType;
                    Response.ReturnMsg = EnResponseMessage.InValidTopLossGainerFilterType;
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetTopLooserPair/{Type}")]
        public ActionResult<TopLooserGainerPairDataResponse> GetTopLooserPair(short Type, short IsMargin = 0)
        {
            try
            {
                //Uday 01-01-2019  Top Looser Pair Data give with Different Filteration
                TopLooserGainerPairDataResponse Response = new TopLooserGainerPairDataResponse();

                if (Type == Convert.ToInt32(EnTopLossGainerFilterType.VolumeWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangePerWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.LTPWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangeValueWise))
                {
                    var Data = _backOfficeService.GetTopLooserPair(Type, IsMargin);
                    if (Data.Count != 0)
                    {
                        Response.Response = Data;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ErrorCode = enErrorCode.Success;
                        Response.ReturnMsg = "Success";
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.NoDataFound;
                        Response.ReturnMsg = "Fail";
                    }
                }
                else
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InValidTopLossGainerFilterType;
                    Response.ReturnMsg = EnResponseMessage.InValidTopLossGainerFilterType;
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetTopLooserGainerPair")]
        public ActionResult<TopLooserGainerPairDataResponse> GetTopLooserGainerPair(short IsMargin = 0)
        {
            try
            {
                //Uday 01-01-2019  Top Gainer/Looser All Pair Data with name wise ascending order
                TopLooserGainerPairDataResponse Response = new TopLooserGainerPairDataResponse();

                var Data = _backOfficeService.GetTopLooserGainerPair(IsMargin);
                if (Data.Count != 0)
                {
                    Response.Response = Data;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = "Success";
                }
                else
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "Fail";
                }

                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("GetCopiedLeaderOrders")]
        public ActionResult<CopiedLeaderOrdersResponse> GetCopiedLeaderOrders([FromBody]CopiedLeaderOrdersBKRequest request)
        {
            CopiedLeaderOrdersResponse Response = new CopiedLeaderOrdersResponse();
            List<CopiedLeaderOrdersInfo> Res = new List<CopiedLeaderOrdersInfo>();
            Int16 trnType = 999;//, marketType = 999;
            long PairId = 999;
            try
            {
                if (!string.IsNullOrEmpty(request.Pair) || !string.IsNullOrEmpty(request.TrnType) || !string.IsNullOrEmpty(request.FromDate))
                {
                    if (!string.IsNullOrEmpty(request.Pair))
                    {
                        if (!_frontTrnService.IsValidPairName(request.Pair))
                        {
                            Response.Response = Res;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidPairName;
                            return Response;
                        }
                        PairId = _frontTrnService.GetPairIdByName(request.Pair);//request.IsMargin//Rita 22-2-19 for Margin Trading Data bit
                        if (PairId == 0)
                        {
                            Response.Response = Res;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidPairName;
                            return Response;
                        }
                    }
                    if (!string.IsNullOrEmpty(request.TrnType))
                    {
                        trnType = _frontTrnService.IsValidTradeType(request.TrnType);
                        if (trnType == 999)
                        {
                            Response.Response = Res;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InValidTrnType;
                            return Response;
                        }
                    }
                    if (!string.IsNullOrEmpty(request.FromDate))
                    {
                        if (string.IsNullOrEmpty(request.ToDate))
                        {
                            Response.Response = Res;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            return Response;
                        }
                        if (!_frontTrnService.IsValidDateFormate(request.FromDate))
                        {
                            Response.Response = Res;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                            return Response;
                        }
                        if (!_frontTrnService.IsValidDateFormate(request.ToDate))
                        {
                            Response.Response = Res;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                            return Response;
                        }
                    }
                }
                if (request.UserID == null)
                    request.UserID = 0;
                if (request.FollowingTo == null)
                    request.FollowingTo = 0;
                Response = _frontTrnService.GetCopiedLeaderOrders((long)request.UserID, request.FromDate, request.ToDate, PairId, trnType, FollowTradeType: request.FollowTradeType, FollowingTo: (long)request.FollowingTo, PageNo: request.PageNo, PageSize: request.PageSize);
                //if (Response.Response.Count == 0)
                //{
                //    Response.ErrorCode = enErrorCode.NoDataFound;
                //    Response.ReturnCode = enResponseCode.Success;
                //    Response.ReturnMsg = "Success";
                //    return Response;
                //}
                //Response.ErrorCode = enErrorCode.Success;
                //Response.ReturnCode = enResponseCode.Success;
                //Response.ReturnMsg = "Success";
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region  SiteToken
        [HttpGet("GetSiteTokenConversionDataBK")]
        public ActionResult<SiteTokenConvertFundResponse> GetSiteTokenConversionDataBK(SiteTokenConvertFundRequest Request)
        {
            SiteTokenConvertFundResponse Response = new SiteTokenConvertFundResponse();
            try
            {
                if (!string.IsNullOrEmpty(Request.FromDate))
                {
                    if (string.IsNullOrEmpty(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(Request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        return BadRequest(Response);
                    }
                }
                return _frontTrnService.GetSiteTokenConversionData(Request.UserID, Request.SourceCurrency, Request.TargetCurrency, Request.FromDate, Request.ToDate);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Liquidity Configuration
        [HttpGet("TradingConfigurationList")]
        [Authorize]
        public async Task<ActionResult<TradingConfigurationList>> TradingConfigurationList()
        {
            TradingConfigurationList Response = new TradingConfigurationList();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.Data = new List<TradingConfigurationViewModel>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Response;
                }
                Response = _frontTrnService.TradingConfiguration();
                if (Response == null)
                {
                    Response.Data = new List<TradingConfigurationViewModel>();
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "NoDataFound";
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ChangeTradingConfigurationStatus")]
        [Authorize]
        public async Task<ActionResult<BizResponseClass>> ChangeTradingConfigurationStatus([FromBody]TradingConfigurationReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Response;
                }
                Response = _frontTrnService.ChangeTradingConfigurationStatus(Request.Id, Convert.ToInt16(Request.Status), user.Id);
                if (Response == null)
                {
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "NoDataFound";
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Arbitrage
        //Darshan Dholakiya added this region for Arbitrage related methods:06-06-2019
        [HttpPost("TradingSummaryArbitrage")]
        public async Task<ActionResult<TradingSummaryResponse>> TradingSummaryArbitrage([FromBody]TradingSummaryRequest request)
        {
            TradingSummaryResponse Response = new TradingSummaryResponse();
            Int16 trnType = 999, marketType = 999;//, status = 999;
            long PairId = 999;
            //string sCondition = "1=1 ";
            try
            {
                if (!string.IsNullOrEmpty(request.Pair))
                {
                    if (!_frontTrnService.IsValidPairName(request.Pair))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                    PairId = _frontTrnService.GetPairIdByNameArbitrage(request.Pair, request.IsMargin);
                    if (PairId == 0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                }
                if (!string.IsNullOrEmpty(request.Trade))
                {
                    trnType = _frontTrnService.IsValidTradeType(request.Trade);
                    if (trnType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InValidTrnType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.MarketType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.MarketType);
                    if (marketType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidMarketType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    if (string.IsNullOrEmpty(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        return BadRequest(Response);
                    }
                }
                if (request.Status != 0 && request.Status != 91 && request.Status != 92 && request.Status != 95 && request.Status != 94 && request.Status != 96 && request.Status != 97 && request.Status != 93)// && request .Status != 93)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidStatusType;
                    return Response;
                }

                Response = _backOfficeService.GetTradingSummaryArbitrage(request.MemberID, request.FromDate, request.ToDate, request.TrnNo, request.Status, request.SMSCode, PairId, trnType, marketType, request.PageSize, request.PageNo, request.IsMargin);
                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnCode = enResponseCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //Response.ReturnCode = enResponseCode.InternalError;
                //return Ok(Response);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("TradingSummaryLPWiseArbitrage")]
        public async Task<ActionResult<TradingSummaryLPResponse>> TradingSummaryLPWiseArbitrage([FromBody]TradingSummaryRequest request)
        {
            TradingSummaryLPResponse Response = new TradingSummaryLPResponse();
            Int16 trnType = 999, marketType = 999;//, status = 999;
            long PairId = 999;
            //string sCondition = "1=1 ";
            try
            {
                if (!string.IsNullOrEmpty(request.Pair))
                {
                    if (!_frontTrnService.IsValidPairName(request.Pair))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                    PairId = _frontTrnService.GetPairIdByNameArbitrage(request.Pair);
                    if (PairId == 0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                }
                if (!string.IsNullOrEmpty(request.Trade))
                {
                    trnType = _frontTrnService.IsValidTradeType(request.Trade);
                    if (trnType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InValidTrnType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.MarketType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.MarketType);
                    if (marketType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidMarketType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    if (string.IsNullOrEmpty(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return BadRequest(Response);
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        return BadRequest(Response);
                    }
                }
                if (request.Status != 0 && request.Status != 91 && request.Status != 92 && request.Status != 95 && request.Status != 94 && request.Status != 96 && request.Status != 97 && request.Status != 93)// && request .Status != 93)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidStatusType;
                    return Response;
                }
                Response = _backOfficeService.GetTradingSummaryLPArbitrage(request.MemberID, request.FromDate, request.ToDate, request.TrnNo, request.Status, request.SMSCode, PairId, trnType, marketType, request.PageSize, request.PageNo, request.LPType);
                //Response.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //Response.ReturnCode = enResponseCode.InternalError;
                //return Ok(Response);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }
        //Darshan Dholakiya added this method for TradeSettledHistoryArbitrage Changes : 08-06-2019
        [HttpPost("TradeSettledHistoryArbitrage")]
        public ActionResult<TradeSettledHistoryResponse> TradeSettledHistoryArbitrage([FromBody]TradeSettledHistoryRequest request)
        {
            try
            {
                Int16 trnType = 999, marketType = 999;//, status = 999;
                long PairId = 999;

                TradeSettledHistoryResponse Response = new TradeSettledHistoryResponse();
                //if (!string.IsNullOrEmpty(request.MemberID.ToString()))
                //    request.MemberID = 0;
                //if (!string.IsNullOrEmpty(request.TrnNo.ToString()))
                //    request.TrnNo = 0;
                if (!string.IsNullOrEmpty(request.PairName))
                {
                    if (!_frontTrnService.IsValidPairName(request.PairName))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return Response;
                    }
                    PairId = _frontTrnService.GetPairIdByNameArbitrage(request.PairName, request.IsMargin);
                    if (PairId == 0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidPairName;
                        return BadRequest(Response);
                    }
                }
                if (!string.IsNullOrEmpty(request.TrnType))
                {
                    trnType = _frontTrnService.IsValidTradeType(request.TrnType);
                    if (trnType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InValidTrnType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.OrderType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.OrderType);
                    if (marketType == 999)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidMarketType;
                        return Response;
                    }
                }
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    if (string.IsNullOrEmpty(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.FromDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidFromDateFormate;
                        return Response;
                    }
                    if (!_frontTrnService.IsValidDateFormate(request.ToDate))
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InvalidToDateFormate;
                        return Response;
                    }
                }
                Response = _backOfficeService.TradeSettledHistoryArbitrage(PageSize: request.PageSize, PageNo: request.PageNo, PairID: PairId, TrnType: trnType, FromDate: request.FromDate, Todate: request.ToDate, OrderType: marketType, MemberID: request.MemberID, TrnNo: request.TrnNo, IsMargin: request.IsMargin);
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
