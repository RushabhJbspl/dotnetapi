using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Data.Transaction;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.ViewModels.Wallet;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Worldex.Web.API
{

    //[ApiExplorerSettings(IgnoreApi =true)]
    //[ApiExplorerSettings(GroupName = "v2")]//rita-komal 31-12-18  for versioning saperation use
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly IBasePage _basePage;
        private readonly ILogger<TransactionController> _logger;
        private readonly IFrontTrnService _frontTrnService;
        private readonly IWithdrawTransactionV1 _WithdrawTransactionV1;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITransactionQueue<NewTransactionRequestCls> _iTransactionQueue;
        private readonly ITransactionQueue<NewTransactionRequestMarginCls> _iTransactionMarginQueue;
        private readonly ITransactionQueue<NewWithdrawRequestCls> _TransactionsQueue;
        private readonly ITransactionQueue<NewCancelOrderRequestCls> _TransactionQueueCancelOrder;
        private readonly IBackOfficeTrnService _backOfficeService;
        private readonly ITransactionConfigService _transactionConfigService;
        private readonly ISiteTokenConversion _ISiteTokenConversion;//Rita 9-2-19 added for Site Token conversion
        private readonly IResdisTradingManagment _IResdisTradingManagment;//Rita 15-3-19 added for Site Token conversion
        private readonly IMarginClosePosition _MarginClosePosition;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IipAddressService _IipAddressService;
        private readonly IWalletService _walletService;

        public TransactionController(ILogger<TransactionController> logger, IBasePage basePage, IFrontTrnService frontTrnService,
            UserManager<ApplicationUser> userManager, 
            ITransactionQueue<NewTransactionRequestCls> iTransactionQueue, IBackOfficeTrnService backOfficeService,// ntrivedi 08-07-2019 now using withdrawtransactionv1
             ITransactionQueue<NewWithdrawRequestCls> TransactionsQueue, ITransactionQueue<NewCancelOrderRequestCls> TransactionQueueCancelOrder,
             ITransactionConfigService transactionConfigService, IWithdrawTransactionV1 WithdrawTransactionV1,
             ISiteTokenConversion ISiteTokenConversion, ITransactionQueue<NewTransactionRequestMarginCls> iTransactionMarginQueue,
             IResdisTradingManagment IResdisTradingManagment, IMarginClosePosition MarginClosePosition,
             Microsoft.Extensions.Configuration.IConfiguration configuration, IipAddressService IipAddressService, IWalletService walletService)
        {
            _logger = logger;
            _basePage = basePage;
            _frontTrnService = frontTrnService;
            _backOfficeService = backOfficeService;
            _userManager = userManager;
            _iTransactionQueue = iTransactionQueue;
            _iTransactionMarginQueue = iTransactionMarginQueue;
            _TransactionsQueue = TransactionsQueue;
            _TransactionQueueCancelOrder = TransactionQueueCancelOrder;
            _transactionConfigService = transactionConfigService;
            _WithdrawTransactionV1 = WithdrawTransactionV1;
            _ISiteTokenConversion = ISiteTokenConversion;
            _IResdisTradingManagment = IResdisTradingManagment;
            _MarginClosePosition = MarginClosePosition;
            _configuration = configuration;
            _IipAddressService = IipAddressService;
            _walletService = walletService;
        }

       
        [HttpPost("CreateTransactionOrderBG/{Pair}")]
        [Authorize]
        public async Task<ActionResult> CreateTransactionOrderBG([FromBody]CreateTransactionRequest Request, string Pair)
        {
            try
            {
                Task<ApplicationUser> userResult;
                var UserID = "";
                if (!HttpContext.Items.Keys.Contains("APIUserID"))
                {
                    return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = "AuthenticationFail", ErrorCode = enErrorCode.AuthenticationFail });
                }
                UserID = HttpContext.Items["APIUserID"].ToString();
                userResult = _userManager.FindByIdAsync(UserID);
                Task<string> accessTokenResult = HttpContext.GetTokenAsync("access_token");
                
                Guid NewTrnGUID = Guid.NewGuid();
                ApplicationUser user = await userResult;
                string accessToken = await accessTokenResult;
                _iTransactionQueue.Enqueue(new NewTransactionRequestCls()
                {
                    TrnMode = Request.TrnMode,
                    TrnType = Request.OrderSide,
                    ordertype = Request.OrderType,
                    SMSCode = Pair,
                    TransactionAccount = Request.CurrencyPairID.ToString(),
                    Amount = Request.Total,
                    PairID = Request.CurrencyPairID,
                    Price = Request.Price,
                    Qty = Helpers.DoRoundForTrading(Request.Amount, 18), //Rita 24-6-19 as greater then 18 digit amt come
                    DebitAccountID = Request.DebitWalletID,
                    CreditAccountID = Request.CreditWalletID,
                    StopPrice = Request.StopPrice,
                    GUID = NewTrnGUID,
                    MemberID = user.Id,
                    MemberMobile = user.Mobile,
                    //MemberID = 16,
                    //MemberMobile = "8128748841",
                    accessToken = accessToken,//accessToken,
                    IsPrivateAPITrade = 1  //komal added for private API
                });
                HelperForLog.WriteLogIntoFile("CreateTransactionOrderBG", "TransactionController", " remoteip: " + HttpContext.Connection.RemoteIpAddress.ToString() + " ### userid " + user.Id + " ### Guid " + NewTrnGUID);
                CreateTransactionResponse Response = new CreateTransactionResponse();
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Order Created";
                Response.ErrorCode = enErrorCode.TransactionProcessSuccess;

                Response.response = new CreateOrderInfo()
                {
                    TrnID = NewTrnGUID
                };
                return await Task.FromResult(Ok(Response));
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpGet("GetBuyerBook/{Pair}")]
        public ActionResult<GetBuySellBookResponse> GetBuyerBook(string Pair, short IsMargin = 0)
        {
            try
            {
                long PairId = 999;
                var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref PairId, IsMargin);
                if (Res.ErrorCode != enErrorCode.Success)
                    return new GetBuySellBookResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

                return _frontTrnService.GetBuyerSellerBookV1(PairId,4, IsMargin);//Rita 22-2-19 for Margin Trading Data bit
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpGet("GetSellerBook/{Pair}")]
        public ActionResult<GetBuySellBookResponse> GetSellerBook(string Pair, short IsMargin = 0)
        {
            GetBuySellBookResponse Response = new GetBuySellBookResponse();
            try
            {
                long PairId = 999;
                var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref PairId, IsMargin);
                if (Res.ErrorCode != enErrorCode.Success)
                    return new GetBuySellBookResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

                return _frontTrnService.GetBuyerSellerBookV1(PairId,5, IsMargin);//Rita 22-2-19 for Margin Trading Data bit
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpPost("ListPair")]
        public ActionResult<ListPairResponse> ListPair(short IsMargin = 0)
        {
            try
            {
                ListPairResponse Response = new ListPairResponse();
                Response = _transactionConfigService.ListPair(IsMargin);
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ListCurrency")]
        public ActionResult<GetServiceByBaseReasponse> ListCurrency(short IsMargin = 0)
        {
            GetServiceByBaseReasponse Response = new GetServiceByBaseReasponse();
            try
            {
                if (IsMargin == 1)
                    Response = _transactionConfigService.GetCurrencyMargin(1);
                else
                    Response = _transactionConfigService.GetCurrency(1);

                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        
        [HttpPost("GetTradeHistory")]
        [Authorize]
        public async Task<ActionResult<GetTradeHistoryResponseV1>> GetTradeHistory([FromBody] TradeHistoryRequest request)
        {
            Int16 trnType = 999, marketType = 999, status = 999;
            long PairId = 999;
            string sCondition = "1=1";
            try
            {
                var UserID = "";
                if (!HttpContext.Items.Keys.Contains("APIUserID"))
                {
                    return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = "AuthenticationFail", ErrorCode = enErrorCode.AuthenticationFail });
                }
                UserID = HttpContext.Items["APIUserID"].ToString();

                if (!string.IsNullOrEmpty(request.Pair))
                {
                    var Res = _frontTrnService.ValidatePairCommonMethod(request.Pair, ref PairId, request.IsMargin);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new GetTradeHistoryResponseV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

                    sCondition += " And TTQ.PairID=" + PairId;
                }
                if (!string.IsNullOrEmpty(request.Trade) || !string.IsNullOrEmpty(request.MarketType) || !string.IsNullOrEmpty(request.FromDate))
                {
                    if (!string.IsNullOrEmpty(request.Trade))
                    {
                        trnType = _frontTrnService.IsValidTradeType(request.Trade);
                        if (trnType == 999)
                        {
                            return new GetTradeHistoryResponseV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InValidTrnType, ReturnMsg = "Fail", response = new List<GetTradeHistoryInfoV1>() };
                        }
                        sCondition += " AND TTQ.TrnType=" + trnType;
                    }
                    if (!string.IsNullOrEmpty(request.MarketType))
                    {
                        marketType = _frontTrnService.IsValidMarketType(request.MarketType);
                        if (marketType == 999)
                        {
                            return new GetTradeHistoryResponseV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InvalidMarketType, ReturnMsg = "Fail", response = new List<GetTradeHistoryInfoV1>() };
                        }
                        sCondition += " AND OT.ID=" + marketType;
                    }
                    if (!string.IsNullOrEmpty(request.FromDate))
                    {
                        var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
                        if (Res.ErrorCode != enErrorCode.Success)
                            return new GetTradeHistoryResponseV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

                        sCondition += "AND TTQ.SettledDate Between {0} AND {1} ";
                    }
                }
                if ((request.Status.ToString()) == "0")
                {
                    status = 999;
                }
                else
                {
                    if (request.Status != 1 && request.Status != 2 && request.Status != 9)
                    {
                        return new GetTradeHistoryResponseV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InvalidStatusType, ReturnMsg = "Fail", response = new List<GetTradeHistoryInfoV1>() };
                    }
                    status = Convert.ToInt16(request.Status);
                }
                var response = _frontTrnService.GetTradeHistoryV1(Convert.ToInt64(UserID), sCondition, request.FromDate, request.ToDate, request.Page, status, request.IsMargin);//Rita 22-2-19 for Margin Trading Data bit
                if (response.Count == 0)
                {
                    return new GetTradeHistoryResponseV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail", response = new List<GetTradeHistoryInfoV1>() };
                }
                return new GetTradeHistoryResponseV1() { ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success", response = response };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = "InternalError", ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpPost("GetActiveOrder")]
        [Authorize]
        public async Task<ActionResult<GetActiveOrderResponseV1>> GetActiveOrder([FromBody]GetActiveOrderRequest request)
        {
            Int16 trnType = 999;
            long PairId = 999;
            try
            {
                var UserID = "";
                if (!HttpContext.Items.Keys.Contains("APIUserID"))
                {
                    return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = "AuthenticationFail", ErrorCode = enErrorCode.AuthenticationFail });
                }
                UserID = HttpContext.Items["APIUserID"].ToString();

                if (!string.IsNullOrEmpty(request.Pair))
                {
                    var Res = _frontTrnService.ValidatePairCommonMethod(request.Pair, ref PairId, request.IsMargin);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new GetActiveOrderResponseV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                }
                if (!string.IsNullOrEmpty(request.OrderType) || !string.IsNullOrEmpty(request.FromDate))
                {
                    if (!string.IsNullOrEmpty(request.OrderType))
                    {
                        trnType = _frontTrnService.IsValidTradeType(request.OrderType);
                        if (trnType == 999)
                        {
                            return new GetActiveOrderResponseV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InValidTrnType, ReturnMsg = "Fail", response = new List<ActiveOrderInfoV1>() };
                        }
                    }
                    if (!string.IsNullOrEmpty(request.FromDate))
                    {
                        var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
                        if (Res.ErrorCode != enErrorCode.Success)
                            return new GetActiveOrderResponseV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                    }
                }
                var response = _frontTrnService.GetActiveOrderV1(Convert.ToInt64(UserID), request.FromDate, request.ToDate, PairId, request.Page, trnType, request.IsMargin);//Rita 22-2-19 for Margin Trading Data bit
                if (response.Count == 0)
                {
                    return new GetActiveOrderResponseV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail", response = new List<ActiveOrderInfoV1>() };
                }
                return new GetActiveOrderResponseV1() { ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success", response = response };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = "InternalError", ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpPost("GetRecentOrder")]
        [Authorize]
        public async Task<ActionResult<GetRecentTradeResponceV1>> GetRecentOrder(string Pair = "999", short IsMargin = 0)
        {
            long PairId = 999;
            try
            {
                var UserID = "";
                if (!HttpContext.Items.Keys.Contains("APIUserID"))
                {
                    return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = "AuthenticationFail", ErrorCode = enErrorCode.AuthenticationFail });
                }
                UserID = HttpContext.Items["APIUserID"].ToString();

                if (Pair != "999")
                {
                    var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref PairId, IsMargin);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new GetRecentTradeResponceV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                }
                var response = _frontTrnService.GetRecentOrderV1(PairId, Convert.ToInt64(UserID), IsMargin);//Rita 22-2-19 for Margin Trading Data bit
                if (response.Count == 0)
                {
                    return new GetRecentTradeResponceV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail", response = new List<RecentOrderInfoV1>() };
                }
                return new GetRecentTradeResponceV1() { ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success", response = response };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = "InternalError", ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetOrderhistory")]
        public ActionResult<GetOrderHistoryResponse> GetOrderhistory(string Pair = "999", short IsMargin = 0)
        {
            long PairId = 999;
            try
            {
                if (Pair != "999")
                {
                    var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref PairId, IsMargin);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new GetOrderHistoryResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                }
                var response = _frontTrnService.GetOrderHistory(PairId, IsMargin);
                return new GetOrderHistoryResponse() { ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success", response = response };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = "InternalError", ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpPost("CancelOrder")]
        [Authorize]
        public async Task<ActionResult> CancelOrder([FromBody]CancelOrderRequest Request)
        {
            try
            {
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                //var accessToken = await HttpContext.GetTokenAsync("access_token");
                Task<ApplicationUser> userResult;
                var UserID = "";
                if (!HttpContext.Items.Keys.Contains("APIUserID"))
                {
                    return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = "AuthenticationFail", ErrorCode = enErrorCode.AuthenticationFail });
                }
                UserID = HttpContext.Items["APIUserID"].ToString();
                userResult = _userManager.FindByIdAsync(UserID);
                Task<string> accessTokenResult = HttpContext.GetTokenAsync("access_token");
                ApplicationUser user = await userResult;
                string accessToken = await accessTokenResult;
                if (Request.CancelAll == 0)
                {
                    if (Request.TranNo == 0)
                        return Ok(new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = "Enter Valid Transaction No", ErrorCode = enErrorCode.CancelOrder_EnterValidTransactionNo });

                }
                else if (Request.CancelAll == 2)
                {
                    if (Convert.ToInt32(Request.OrderType) == 0)
                        return Ok(new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = "Enter Valid Maket Type", ErrorCode = enErrorCode.InValidOrderType });

                }
                _TransactionQueueCancelOrder.Enqueue(new NewCancelOrderRequestCls()
                {
                    MemberID = user.Id,
                    TranNo = Request.TranNo,
                    accessToken = accessToken,
                    CancelAll = Request.CancelAll,
                    OrderType = Request.OrderType,
                    IsMargin = Request.IsMargin//Rita 21-2-19 for margin trading
                });
                return Ok(new BizResponseClass() { ReturnCode = enResponseCode.Success, ReturnMsg = "Cancel Order Process Initialize", ErrorCode = enErrorCode.CancelOrder_ProccedSuccess });
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetAllAvailableBalance")]
        [Authorize]
        public async Task<IActionResult> GetAllAvailableBalance()
        {
            Task<ApplicationUser> userResult;
            var UserID = "";
            if (!HttpContext.Items.Keys.Contains("APIUserID"))
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = "AuthenticationFail", ErrorCode = enErrorCode.AuthenticationFail });
            }
            UserID = HttpContext.Items["APIUserID"].ToString();
            userResult = _userManager.FindByIdAsync(UserID);
            Task<string> accessTokenResult = HttpContext.GetTokenAsync("access_token");
            ApplicationUser user = await userResult;
            string accessToken = await accessTokenResult;
            TotalBalanceRes Response = new TotalBalanceRes();
            Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
            try
            {
                if (user == null)
                {
                    Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
                    Response.BizResponseObj.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.BizResponseObj.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.GetAllAvailableBalance(user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

    }
}