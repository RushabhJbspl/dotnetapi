using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using Worldex.Infrastructure.Interfaces;
//using Worldex.Web.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Worldex.Core.ViewModels.Transaction.MarketMaker;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackofficeWorldex.Web.API
{
    [Route("api/[controller]")]
    //[ApiController] 
    [Authorize] //komal 10-06-2019 make authorize
    public class TransactionBackOfficeController : Controller
    {
        private readonly IBackOfficeTrnService _backOfficeService;
        private readonly IFrontTrnService _frontTrnService;
        private readonly UserManager<ApplicationUser> _userManager; 

        public TransactionBackOfficeController(
            IBackOfficeTrnService backOfficeService,
            IFrontTrnService frontTrnService,
            UserManager<ApplicationUser> userManager)
        {
            _backOfficeService = backOfficeService;
            _frontTrnService = frontTrnService;
            _userManager = userManager;
        }

        #region History method
        [HttpPost("TradingSummary")]
        public ActionResult<TradingSummaryResponse> TradingSummary([FromBody]TradingSummaryRequest request)
        {
            Int16 trnType = 999, marketType = 999;
            long PairId = 999;
            try
            {
                if (!string.IsNullOrEmpty(request.Pair))
                {
                    var Res = _frontTrnService.ValidatePairCommonMethod(request.Pair,ref PairId, request.IsMargin);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new TradingSummaryResponse() { ErrorCode=Res.ErrorCode,ReturnCode=Res.ReturnCode,ReturnMsg=Res.ReturnMsg };
                }
                if (!string.IsNullOrEmpty(request.Trade))
                {
                    trnType = _frontTrnService.IsValidTradeType(request.Trade);
                    if (trnType == 999)
                        return new TradingSummaryResponse() { ErrorCode = enErrorCode.InValidTrnType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                }
                if (!string.IsNullOrEmpty(request.MarketType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.MarketType);
                    if (marketType == 999)
                        return new TradingSummaryResponse() { ErrorCode = enErrorCode.InvalidMarketType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                }
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new TradingSummaryResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg, Response = new List<TradingSummaryViewModel>() };
                }
                if (request.Status != 0 && request.Status != 91 && request.Status != 92 && request.Status != 95 && request.Status != 94 && request.Status != 96 && request.Status != 97 && request.Status != 93 && request.Status != 99)
                {
                    return new TradingSummaryResponse() { ErrorCode = enErrorCode.InvalidStatusType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                }
                return _backOfficeService.GetTradingSummaryV1(request.MemberID, request.FromDate, request.ToDate, request.TrnNo, request.Status, request.SMSCode, PairId, trnType, marketType, request.PageSize, request.PageNo, request.IsMargin);
                
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpPost("TradingSummaryLPWise")]
        public ActionResult<TradingSummaryLPResponse> TradingSummaryLPWise([FromBody]TradingSummaryRequest request)
        {
            Int16 trnType = 999, marketType = 999;
            long PairId = 999;
            try
            {
                if (!string.IsNullOrEmpty(request.Pair))
                {
                    var Res = _frontTrnService.ValidatePairCommonMethod(request.Pair, ref PairId, request.IsMargin);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new TradingSummaryLPResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg};
                }
                if (!string.IsNullOrEmpty(request.Trade))
                {
                    trnType = _frontTrnService.IsValidTradeType(request.Trade);
                    if (trnType == 999)
                        return new TradingSummaryLPResponse() { ErrorCode = enErrorCode.InValidTrnType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                }
                if (!string.IsNullOrEmpty(request.MarketType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.MarketType);
                    if (marketType == 999)
                        return new TradingSummaryLPResponse() { ErrorCode = enErrorCode.InvalidMarketType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                }
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new TradingSummaryLPResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                }
                if (request.Status != 0 && request.Status != 91 && request.Status != 92 && request.Status != 95 && request.Status != 94 && request.Status != 96 && request.Status != 97 && request.Status != 93)// && request .Status != 93)
                {
                    return new TradingSummaryLPResponse() { ErrorCode = enErrorCode.InvalidStatusType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                }

                return  _backOfficeService.GetTradingSummaryLPV1(request.MemberID, request.FromDate, request.ToDate, request.TrnNo, request.Status, request.SMSCode, PairId, trnType, marketType, request.PageSize, request.PageNo, request.LPType);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpPost("TradingReconHistory")]
        public ActionResult<TradingReconHistoryResponse> TradingReconHistory([FromBody]TradingReconHistoryRequest request)
        {
            Int16 trnType = 999, marketType = 999;
            long PairId = 999;
            try
            {
                if (!string.IsNullOrEmpty(request.Pair))
                {
                    var Res = _frontTrnService.ValidatePairCommonMethod(request.Pair, ref PairId, request.IsMargin);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new TradingReconHistoryResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                }
                if (!string.IsNullOrEmpty(request.Trade))
                {
                    trnType = _frontTrnService.IsValidTradeType(request.Trade);
                    if (trnType == 999)
                        return new TradingReconHistoryResponse() { ErrorCode = enErrorCode.InValidTrnType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                    
                }
                if (!string.IsNullOrEmpty(request.MarketType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.MarketType);
                    if (marketType == 999)
                        return new TradingReconHistoryResponse() { ErrorCode = enErrorCode.InvalidMarketType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                    
                }
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new TradingReconHistoryResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                }
                return  _backOfficeService.GetTradingReconHistoryV1(request.MemberID, request.FromDate, request.ToDate, request.TrnNo, request.Status, PairId, trnType, marketType, request.PageSize, request.PageNo, request.LPType, request.IsProcessing);
               
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("WithdrawalSummary")]
        public ActionResult<WithdrawalSummaryResponse> WithdrawalSummary([FromBody]WithdrawalSummaryRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new WithdrawalSummaryResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                }
                if (request.Status != 0 && request.Status != 81 && request.Status != 82 && request.Status != 83 && request.Status != 84 && request.Status != 85 && request.Status != 86)
                {
                    return new WithdrawalSummaryResponse() { ErrorCode = enErrorCode.InvalidStatusType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                }
                if (string.IsNullOrEmpty(request.SMSCode))
                {
                    request.SMSCode = "";
                }
                return _backOfficeService.GetWithdrawalSummary(request);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpPost("TradeSettledHistory")]
        public ActionResult<TradeSettledHistoryResponseV1> TradeSettledHistory([FromBody]TradeSettledHistoryRequest request)
        {
            try
            {
                Int16 trnType = 999, marketType = 999;
                long PairId = 999;
                if (!string.IsNullOrEmpty(request.PairName))
                {
                    var Res = _frontTrnService.ValidatePairCommonMethod(request.PairName, ref PairId, request.IsMargin);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new TradeSettledHistoryResponseV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                }
                if (!string.IsNullOrEmpty(request.TrnType))
                {
                    trnType = _frontTrnService.IsValidTradeType(request.TrnType);
                    if (trnType == 999)
                        return new TradeSettledHistoryResponseV1() { ErrorCode = enErrorCode.InValidTrnType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                    
                }
                if (!string.IsNullOrEmpty(request.OrderType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.OrderType);
                    if (marketType == 999)
                        return new TradeSettledHistoryResponseV1() { ErrorCode = enErrorCode.InvalidMarketType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                    
                }
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new TradeSettledHistoryResponseV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                
                }
                return  _backOfficeService.TradeSettledHistoryV1(PageSize: request.PageSize, PageNo: request.PageNo, PairID: PairId, TrnType: trnType, FromDate: request.FromDate, Todate: request.ToDate, OrderType: marketType, MemberID: request.MemberID, TrnNo: request.TrnNo, IsMargin: request.IsMargin);
                
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
            try
            {
                long id = 999;

                var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref id, IsMargin);
                if (Res.ErrorCode != enErrorCode.Success)
                    return new GetGraphDetailReponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

                _frontTrnService.GetIntervalTimeValue(Interval, ref IntervalTime, ref IntervalData);
                if (IntervalTime == 0)
                     return new GetGraphDetailReponse() { ErrorCode = enErrorCode.Graph_InvalidIntervalTime, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                
                var responsedata = _frontTrnService.GetGraphDetail(id, IntervalTime, IntervalData, IsMargin);
                if (responsedata != null && responsedata.Count != 0)
                    return new GetGraphDetailReponse() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success",response=responsedata };
                else
                    return new GetGraphDetailReponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
              
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Recon method
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
                    return Ok(new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed });
                }
                if (request.TranNo == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionNo;
                    Response.ErrorCode = enErrorCode.TradeRecon_InvalidTransactionNo;
                    return Ok(Response);
                }
                var UserId = user.Id;
                Response = await _backOfficeService.TradeReconV1(request.ActionType, request.TranNo, request.ActionMessage, UserId, accessToken);

                return Ok(Response);
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
                    return Ok(new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed });
                }
                if (request.TrnNo == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionNo;
                    Response.ErrorCode = enErrorCode.TradeRecon_InvalidTransactionNo;
                }
                else
                {
                    //var UserId = 20;
                    if (request.ActionType == enWithdrawalReconActionType.Refund)//add refund condition by mansi 11-11-2019
                    {
                        Response.ErrorCode = enErrorCode.WithdrawalRecon_InvalidActionType;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.WithdrawalRecon_InvalidActionType;
                        return Ok(Response);
                    }
                    var response = _backOfficeService.WithdrawalRecon(request, user.Id, accessToken);
                    return Ok(response);
                }

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

      

        #endregion

        #region TopGainer And TopLooser BackOffice
        [HttpGet("GetTopGainerPair/{Type}")]
        public ActionResult<TopLooserGainerPairDataResponse> GetTopGainerPair(int Type, short IsMargin = 0)
        {
            try
            {
                //Uday 01-01-2019  Top Gainer Pair Data give with Different Filteration
                if (Type == Convert.ToInt32(EnTopLossGainerFilterType.VolumeWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangePerWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.LTPWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangeValueWise))
                {
                    var Data = _backOfficeService.GetTopGainerPair(Type, IsMargin);

                    if (Data.Count != 0)
                        return new TopLooserGainerPairDataResponse() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success",Response=Data };
                    else
                        return new TopLooserGainerPairDataResponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                }
                else
                    return new TopLooserGainerPairDataResponse() { ErrorCode = enErrorCode.InValidTopLossGainerFilterType, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InValidTopLossGainerFilterType };
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
                if (Type == Convert.ToInt32(EnTopLossGainerFilterType.VolumeWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangePerWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.LTPWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangeValueWise))
                {
                    var Data = _backOfficeService.GetTopLooserPair(Type, IsMargin);
                    if (Data.Count != 0)
                        return new TopLooserGainerPairDataResponse() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success", Response = Data };
                    else
                        return new TopLooserGainerPairDataResponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                }
                else
                    return new TopLooserGainerPairDataResponse() { ErrorCode = enErrorCode.InValidTopLossGainerFilterType, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InValidTopLossGainerFilterType };
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
                var Data = _backOfficeService.GetTopLooserGainerPair(IsMargin);
                if (Data.Count != 0)
                    return new TopLooserGainerPairDataResponse() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success", Response = Data };
                else
                    return new TopLooserGainerPairDataResponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
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
            Int16 trnType = 999;
            long PairId = 999;
            try
            {
                if (!string.IsNullOrEmpty(request.Pair) || !string.IsNullOrEmpty(request.TrnType) || !string.IsNullOrEmpty(request.FromDate))
                {
                    if (!string.IsNullOrEmpty(request.Pair))
                    {
                        var Res1 = _frontTrnService.ValidatePairCommonMethod(request.Pair, ref PairId, 0);
                        if (Res1.ErrorCode != enErrorCode.Success)
                            return new CopiedLeaderOrdersResponse() { ErrorCode = Res1.ErrorCode, ReturnCode = Res1.ReturnCode, ReturnMsg = Res1.ReturnMsg };
                    }
                    if (!string.IsNullOrEmpty(request.TrnType))
                    {
                        trnType = _frontTrnService.IsValidTradeType(request.TrnType);
                        if (trnType == 999)
                            return new CopiedLeaderOrdersResponse() { ErrorCode = enErrorCode.InValidTrnType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                    }
                    if (!string.IsNullOrEmpty(request.FromDate))
                    {
                        var Res1 = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
                        if (Res1.ErrorCode != enErrorCode.Success)
                            return new CopiedLeaderOrdersResponse() { ErrorCode = Res1.ErrorCode, ReturnCode = Res1.ReturnCode, ReturnMsg = Res1.ReturnMsg };
                    }
                }
                if (request.UserID == null)
                    request.UserID = 0;
                if (request.FollowingTo == null)
                    request.FollowingTo = 0;
                return  _frontTrnService.GetCopiedLeaderOrders((long)request.UserID, request.FromDate, request.ToDate, PairId, trnType, FollowTradeType: request.FollowTradeType, FollowingTo: (long)request.FollowingTo, PageNo: request.PageNo, PageSize: request.PageSize);
                
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
                    var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(Request.FromDate, Request.ToDate);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new SiteTokenConvertFundResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
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
                    return new TradingConfigurationList() { Data = new List<TradingConfigurationViewModel>(), ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed};
                
                Response = _frontTrnService.TradingConfiguration();
                if (Response == null)
                    return new TradingConfigurationList() { Data = new List<TradingConfigurationViewModel>(), ReturnCode = enResponseCode.Fail, ReturnMsg = "NoDataFound", ErrorCode = enErrorCode.NoDataFound };
                
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
                    return Ok(new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed });

                if (Request.Id == 0)
                    return Ok(new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed });

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

        #region Not in use Method
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
                    var Res = _frontTrnService.ValidatePairCommonMethod(request.Pair, ref PairId, request.IsMargin);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new PairTradeSummaryResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
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

        [HttpPost("TransactionChargeSummary")]
        public ActionResult<TransactionChargeResponse> TransactionChargeSummary(TransactionChargeRequest request)
        {
            TransactionChargeResponse Response = new TransactionChargeResponse();
            Int16 trnType = 999;
            try
            {
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new TransactionChargeResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
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

        #endregion

        #region Market Maker performance
        [HttpPost("MarketMakerBalance")]
        public async Task<ActionResult<MarketMakerBalancePerformanceResponse>> MarketMakerBalance()
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                    return new MarketMakerBalancePerformanceResponse() { Response = new List<MarketMakerBalancePerformanceViewModel>(), ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };

                return _backOfficeService.GetMarketMakerBalancePerformance();
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("MarketMakerTradePerformance")]
        public async Task<ActionResult<MarketMakerTradePerformanceResponse>> MarketMakerTradePerformance(string PairName = null, string FromDate = null, string Todate = null)
        {
            long PairId = 0;
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                    return new MarketMakerTradePerformanceResponse() { Response = new List<MarketMakerTradePerformance>(), ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };

                if (!string.IsNullOrEmpty(FromDate))
                {
                    var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(FromDate, Todate);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new MarketMakerTradePerformanceResponse() { Response = new List<MarketMakerTradePerformance>(), ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                }
                if (!string.IsNullOrEmpty(PairName))
                {
                    var Res1 = _frontTrnService.ValidatePairCommonMethod(PairName, ref PairId, 0);
                    if (Res1.ErrorCode != enErrorCode.Success)
                        return new MarketMakerTradePerformanceResponse() { Response = new List<MarketMakerTradePerformance>(), ErrorCode = Res1.ErrorCode, ReturnCode = Res1.ReturnCode, ReturnMsg = Res1.ReturnMsg };
                }
                return _backOfficeService.MarketMakerTradePerformance(PairId, FromDate, Todate);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion
    }
}