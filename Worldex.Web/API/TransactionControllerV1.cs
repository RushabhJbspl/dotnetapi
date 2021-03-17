using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Helpers;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Interfaces.Configuration;
using CleanArchitecture.Core.ViewModels;
using CleanArchitecture.Core.ViewModels.Configuration;
using CleanArchitecture.Core.ViewModels.Transaction;
using CleanArchitecture.Core.ViewModels.Transaction.Arbitrage;
using CleanArchitecture.Core.ViewModels.Transaction.BackOffice;
using CleanArchitecture.Infrastructure.BGTask;
using CleanArchitecture.Infrastructure.Data.Transaction;
using CleanArchitecture.Infrastructure.DTOClasses;
using CleanArchitecture.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
//komal for Worldex, removed Arbitrage Method
namespace CleanArchitecture.Web.API
{

    //[ApiExplorerSettings(IgnoreApi =true)]
    //[ApiExplorerSettings(GroupName = "v2")]//rita-komal 31-12-18  for versioning saperation use
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionControllerV1 : Controller
    {
        private readonly IBasePage _basePage;
        private readonly ILogger<TransactionControllerV1> _logger;
        private readonly IFrontTrnService _frontTrnService;
        private readonly IWithdrawTransactionV1 _WithdrawTransactionV1;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITransactionQueue<NewTransactionRequestCls> _iTransactionQueue;
        private readonly ITransactionQueue<NewTransactionRequestMarginCls> _iTransactionMarginQueue;
        private readonly ITransactionQueue<NewTransactionRequestArbitrageCls> _iTransactionArbitrageQueue;
        private readonly ITransactionQueue<NewWithdrawRequestCls> _TransactionsQueue;
        private readonly ITransactionQueue<NewCancelOrderRequestCls> _TransactionQueueCancelOrder;
        private readonly ITransactionQueue<NewCancelOrderArbitrageRequestCls> _TransactionQueueCancelOrderArbitrage; //komal 07-06-2019 cancel arbitrage Trade
        private readonly IBackOfficeTrnService _backOfficeService;
        private readonly ITransactionConfigService _transactionConfigService;
        private readonly ISiteTokenConversion _ISiteTokenConversion;//Rita 9-2-19 added for Site Token conversion
        private readonly IResdisTradingManagment _IResdisTradingManagment;//Rita 15-3-19 added for Site Token conversion
        private readonly IMarginClosePosition _MarginClosePosition;

        public TransactionControllerV1(ILogger<TransactionControllerV1> logger, IBasePage basePage, IFrontTrnService frontTrnService,
            UserManager<ApplicationUser> userManager,
            ITransactionQueue<NewTransactionRequestCls> iTransactionQueue, IBackOfficeTrnService backOfficeService,// ntrivedi 08-07-2019 now using withdrawtransactionv1
             ITransactionQueue<NewWithdrawRequestCls> TransactionsQueue, ITransactionQueue<NewCancelOrderRequestCls> TransactionQueueCancelOrder,
             ITransactionConfigService transactionConfigService, IWithdrawTransactionV1 WithdrawTransactionV1,
             ISiteTokenConversion ISiteTokenConversion, ITransactionQueue<NewTransactionRequestMarginCls> iTransactionMarginQueue,
             IResdisTradingManagment IResdisTradingManagment, IMarginClosePosition MarginClosePosition,
             ITransactionQueue<NewTransactionRequestArbitrageCls> iTransactionArbitrageQueue,
             ITransactionQueue<NewCancelOrderArbitrageRequestCls> TransactionQueueCancelOrderArbitrage)
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
            _iTransactionArbitrageQueue = iTransactionArbitrageQueue;
            _TransactionQueueCancelOrderArbitrage = TransactionQueueCancelOrderArbitrage;
        }

        #region "Transaction Process Methods"

        [HttpPost("CreateTransactionOrderBG/{Pair}")]
        [Authorize]
        public async Task<ActionResult> CreateTransactionOrderBG([FromBody]CreateTransactionRequest Request, string Pair)
        {
            try
            {
                Task<ApplicationUser> userResult = _userManager.GetUserAsync(HttpContext.User);
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
                    accessToken = accessToken//accessToken
                });
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

        //Rita 15-2-19 new method for Margin Trading
        [HttpPost("CreateTransactionOrderMargin/{Pair}")]
        [Authorize]
        public async Task<ActionResult> CreateTransactionOrderMargin([FromBody]CreateTransactionRequest Request, string Pair)
        {
            try
            {
                Task<ApplicationUser> userResult = _userManager.GetUserAsync(HttpContext.User);
                Task<string> accessTokenResult = HttpContext.GetTokenAsync("access_token");

                Guid NewTrnGUID = Guid.NewGuid();
                ApplicationUser user = await userResult;
                string accessToken = await accessTokenResult;
                _iTransactionMarginQueue.Enqueue(new NewTransactionRequestMarginCls()
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
                    accessToken = accessToken
                });
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

        //[HttpPost("CreateTransactionOrderArbitrage/{Pair}")]
        //[Authorize]
        //public async Task<ActionResult> CreateTransactionOrderArbitrage([FromBody]CreateTransactionRequest Request, string Pair)
        //{
        //    try
        //    {
        //        Task<ApplicationUser> userResult = _userManager.GetUserAsync(HttpContext.User);
        //        Task<string> accessTokenResult = HttpContext.GetTokenAsync("access_token");

        //        Guid NewTrnGUID = Guid.NewGuid();
        //        ApplicationUser user = await userResult;
        //        string accessToken = await accessTokenResult;
        //        _iTransactionArbitrageQueue.Enqueue(new NewTransactionRequestArbitrageCls()
        //        {
        //            TrnMode = Request.TrnMode,
        //            TrnType = Request.OrderSide,
        //            ordertype = Request.OrderType,
        //            SMSCode = Pair,
        //            TransactionAccount = Request.CurrencyPairID.ToString(),
        //            Amount = Request.Total,
        //            PairID = Request.CurrencyPairID,
        //            Price = Request.Price,
        //            Qty = Helpers.DoRoundForTrading(Request.Amount, 18), //Rita 24-6-19 as greater then 18 digit amt come
        //            DebitAccountID = Request.DebitWalletID,
        //            CreditAccountID = Request.CreditWalletID,
        //            StopPrice = Request.StopPrice,
        //            GUID = NewTrnGUID,
        //            MemberID = user.Id,
        //            MemberMobile = user.Mobile,
        //            accessToken = accessToken,//accessToken
        //            LPType = Request.LPType
        //        });
        //        CreateTransactionResponse Response = new CreateTransactionResponse();

        //        Response.ReturnCode = enResponseCode.Success;
        //        Response.ReturnMsg = "Order Created";
        //        Response.ErrorCode = enErrorCode.TransactionProcessSuccess;

        //        Response.response = new CreateOrderInfo()
        //        {
        //            TrnID = NewTrnGUID
        //        };
        //        return await Task.FromResult(Ok(Response));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }

        //}

        //[HttpPost("CreateTransactionOrderArbitrageBulk/{Pair}")]
        //[Authorize]
        //public async Task<ActionResult> CreateTransactionOrderArbitrageBulk([FromBody]CreateTransactionRequestBulk Request, string Pair)
        //{
        //    try
        //    {
        //        Task<ApplicationUser> userResult = _userManager.GetUserAsync(HttpContext.User);
        //        Task<string> accessTokenResult = HttpContext.GetTokenAsync("access_token");

        //        Guid NewTrnGUID = Guid.NewGuid();
        //        ApplicationUser user = await userResult;
        //        string accessToken = await accessTokenResult;

        //        foreach (CreateTransactionRequest sRequest in Request.MultipleOrderList)
        //        {
        //            _iTransactionArbitrageQueue.Enqueue(new NewTransactionRequestArbitrageCls()
        //            {
        //                TrnMode = sRequest.TrnMode,
        //                TrnType = sRequest.OrderSide,
        //                ordertype = sRequest.OrderType,
        //                SMSCode = Pair,
        //                TransactionAccount = sRequest.CurrencyPairID.ToString(),
        //                Amount = sRequest.Total,
        //                PairID = sRequest.CurrencyPairID,
        //                Price = sRequest.Price,
        //                Qty = Helpers.DoRoundForTrading(sRequest.Amount, 18), //Rita 24-6-19 as greater then 18 digit amt come
        //                DebitAccountID = sRequest.DebitWalletID,
        //                CreditAccountID = sRequest.CreditWalletID,
        //                StopPrice = sRequest.StopPrice,
        //                GUID = NewTrnGUID,
        //                MemberID = user.Id,
        //                MemberMobile = user.Mobile,
        //                accessToken = accessToken,//accessToken
        //                LPType = sRequest.LPType
        //            });
        //            await Task.Delay(300);
        //        }

        //        CreateTransactionResponse Response = new CreateTransactionResponse();

        //        Response.ReturnCode = enResponseCode.Success;
        //        Response.ReturnMsg = "Order Created";
        //        Response.ErrorCode = enErrorCode.TransactionProcessSuccess;

        //        Response.response = new CreateOrderInfo()
        //        {
        //            TrnID = NewTrnGUID
        //        };
        //        return await Task.FromResult(Ok(Response));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }

        //}

        //[HttpPost("CreateTransactionOrderSmartArbitrage/{Pair}")]
        //[Authorize]
        //public async Task<ActionResult> CreateTransactionOrderSmartArbitrage([FromBody]CreateTransactionRequestBulk Request, string Pair)
        //{
        //    try
        //    {
        //        Task<ApplicationUser> userResult = _userManager.GetUserAsync(HttpContext.User);
        //        Task<string> accessTokenResult = HttpContext.GetTokenAsync("access_token");

        //        CreateTransactionResponse Response = new CreateTransactionResponse();

        //        if (Request.MultipleOrderList.Count() != 2)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ErrorCode = enErrorCode.InvalidSmartArbitrageOrder;
        //            Response.ReturnMsg = "Fail";
        //            return await Task.FromResult(Ok(Response));
        //        }
        //        short IsBuy = 0;
        //        short IsSell = 0;
        //        foreach (CreateTransactionRequest sRequest in Request.MultipleOrderList)
        //        {
        //            if (sRequest.OrderSide == enTrnType.Buy_Trade)
        //                IsBuy = 1;
        //            else
        //                IsSell = 1;
        //        }
        //        if (IsBuy == 0 || IsSell == 0)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ErrorCode = enErrorCode.SmartArbitrageOrder_ShouldHave_BothBuyNSell;
        //            Response.ReturnMsg = "Fail";
        //            return await Task.FromResult(Ok(Response));
        //        }
        //        Guid NewTrnGUID = Guid.NewGuid();
        //        ApplicationUser user = await userResult;
        //        string accessToken = await accessTokenResult;
        //        foreach (CreateTransactionRequest sRequest in Request.MultipleOrderList)
        //        {
        //            _iTransactionArbitrageQueue.Enqueue(new NewTransactionRequestArbitrageCls()
        //            {
        //                TrnMode = sRequest.TrnMode,
        //                TrnType = sRequest.OrderSide,
        //                ordertype = sRequest.OrderType,
        //                SMSCode = Pair,
        //                TransactionAccount = sRequest.CurrencyPairID.ToString(),
        //                Amount = sRequest.Total,
        //                PairID = sRequest.CurrencyPairID,
        //                Price = sRequest.Price,
        //                Qty = Helpers.DoRoundForTrading(sRequest.Amount, 18), //Rita 24-6-19 as greater then 18 digit amt come
        //                DebitAccountID = sRequest.DebitWalletID,
        //                CreditAccountID = sRequest.CreditWalletID,
        //                StopPrice = sRequest.StopPrice,
        //                GUID = NewTrnGUID,
        //                MemberID = user.Id,
        //                MemberMobile = user.Mobile,
        //                accessToken = accessToken,//accessToken
        //                LPType = sRequest.LPType,
        //                IsSmartArbitrage = 1
        //            });
        //            await Task.Delay(300);
        //        }
        //        Response.ReturnCode = enResponseCode.Success;
        //        Response.ReturnMsg = "Order Created";
        //        Response.ErrorCode = enErrorCode.TransactionProcessSuccess;

        //        Response.response = new CreateOrderInfo()
        //        {
        //            TrnID = NewTrnGUID
        //        };
        //        return await Task.FromResult(Ok(Response));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }

        //}

        //Rita 13-9-19 Close Open Position User's Pair Wise
        [HttpPost("CloseOpenPostionMargin")]
        [Authorize]
        public async Task<ActionResult<CloseOpenPostionResponseMargin>> CloseOpenPostionMargin([FromBody]CloseOpenPostionRequestMargin Request)
        {
            try
            {
                Task<ApplicationUser> user = _userManager.GetUserAsync(HttpContext.User);
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                return await _MarginClosePosition.CloseOpenPostionMargin(Request.PairID, user.Id, accessToken);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("Withdrawal")]
        [Authorize]
        public async Task<ActionResult> Withdrawal([FromBody]WithdrawalRequest Request)
        {
            try
            {
                CreateTransactionResponse Response = new CreateTransactionResponse();
                //2019-6-29 added user condition 
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return await Task.FromResult(Ok(Response));
                }
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                Guid NewTrnGUID = Guid.NewGuid();
                _TransactionsQueue.Enqueue(new NewWithdrawRequestCls()
                {
                    accessToken = accessToken,
                    TrnMode = Request.TrnMode,
                    TrnType = enTrnType.Withdraw,
                    MemberID = user.Id,
                    MemberMobile = user.Mobile,
                    //MemberID = 16,
                    //MemberMobile = "1234567890",
                    SMSCode = Request.asset,
                    TransactionAccount = Request.address,
                    Amount = Request.Amount,
                    DebitAccountID = Request.DebitWalletID,
                    AddressLabel = Request.AddressLabel,
                    WhitelistingBit = Request.WhitelistingBit,
                    GUID = NewTrnGUID,
                });

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

        [Authorize]
        [HttpPost("WithdrawalTransaction")]
        public async Task<ActionResult<GetWithdrawalTransactionResponse>> WithdrawalTransaction(WithdrawalConfirmationRequest Request)
        {
            try
            {
                GetWithdrawalTransactionResponse Response = new GetWithdrawalTransactionResponse();

                //2019-6-29 added user condition 
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return await Task.FromResult(BadRequest(Response));
                }
                var response = await _WithdrawTransactionV1.WithdrawTransactionAPICallProcessAsync(Request, user.Id, 0);
                var responsedata = _frontTrnService.GetWithdrawalTransaction(Request.RefNo); // Uday 12-01-2019 Add Withdrwal Data In response;

                if (responsedata != null)
                {
                    if (Request.TransactionBit == 1)
                    {
                        responsedata.FinalAmount = responsedata.Amount;
                    }
                    else if (Request.TransactionBit == 2)
                    {
                        responsedata.FinalAmount = responsedata.Amount + responsedata.Fee;
                    }
                }

                Response.Response = responsedata;
                Response.ErrorCode = response.ErrorCode;
                Response.ReturnMsg = response.ReturnMsg;

                if (response.ReturnCode == enResponseCodeService.Fail)
                    Response.ReturnCode = enResponseCode.Fail;
                else
                    Response.ReturnCode = enResponseCode.Success;

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [Authorize]
        [HttpPost("GetWithdrawalTransaction/{RefId}")]
        public ActionResult<GetWithdrawalTransactionResponse> GetWithdrawalTransaction(string RefId)
        {
            try
            {
                var responsedata = _frontTrnService.GetWithdrawalTransaction(RefId);
                if (responsedata != null)
                    return new GetWithdrawalTransactionResponse() { Response = responsedata, ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
                else
                    return new GetWithdrawalTransactionResponse() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail" };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [Authorize]
        [HttpPost("ResendEmailWithdrawalConfirmation/{TrnNo}")]
        public async Task<ActionResult<BizResponse>> ResendEmailWithdrawalConfirmation(long TrnNo)
        {
            try
            {
                // khushali 15-03-2019 for use API Key Authorization
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                    return Ok(new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed });

                return _WithdrawTransactionV1.ResendEmailWithdrawalConfirmation(TrnNo, user.Id).Result;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("CancelOrder")]
        [Authorize]
        public async Task<ActionResult> CancelOrder([FromBody]CancelOrderRequest Request)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var accessToken = await HttpContext.GetTokenAsync("access_token");

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

        ////komal 07-06-2019 cancel arbitrage Trade
        //[HttpPost("CancelOrderArbitrage")]
        //[Authorize]
        //public async Task<ActionResult> CancelOrderArbitrage([FromBody]CancelOrderArbitrageRequest Request)
        //{
        //    try
        //    {
        //        ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        var accessToken = await HttpContext.GetTokenAsync("access_token");
        //        if (Request.CancelAll == 0)
        //        {
        //            if (Request.TranNo == 0)
        //                return Ok(new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = "Enter Valid Transaction No", ErrorCode = enErrorCode.CancelOrder_EnterValidTransactionNo });
        //        }
        //        else if (Request.CancelAll == 2)
        //        {
        //            if (Convert.ToInt32(Request.OrderType) == 0)
        //                return Ok(new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = "Enter Valid Maket Type", ErrorCode = enErrorCode.InValidOrderType });
        //        }
        //        _TransactionQueueCancelOrderArbitrage.Enqueue(new NewCancelOrderArbitrageRequestCls()
        //        {
        //            MemberID = user.Id,
        //            TranNo = Request.TranNo,
        //            accessToken = accessToken,
        //            CancelAll = Request.CancelAll,
        //            OrderType = Request.OrderType
        //        });
        //        return Ok(new BizResponseClass() { ReturnCode = enResponseCode.Success, ReturnMsg = "Cancel Order Process Initialize", ErrorCode = enErrorCode.CancelOrder_ProccedSuccess });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}


        //Rita 9-2-19 Method for Currency Convert Calculation
        [HttpPost("SiteTokenCalculation")]
        [Authorize]
        public async Task<ActionResult<SiteTokenCalculationResponse>> SiteTokenCalculation([FromBody]SiteTokenCalculationRequest Request)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                if (Request.IsMargin == 1)
                    return await _ISiteTokenConversion.SiteTokenCalculationMargin(Request, user.Id, accessToken);
                else
                    return await _ISiteTokenConversion.SiteTokenCalculation(Request, user.Id, accessToken);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //Rita 7-2-19 Method for Currency Convert to Site Token
        [HttpPost("SiteTokenConversion")]
        [Authorize]
        public async Task<ActionResult<SiteTokenConversionResponse>> SiteTokenConversion([FromBody]SiteTokenConversionRequest Request)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (Request.IsMargin == 1)
                    return await _ISiteTokenConversion.SiteTokenConversionAsyncMargin(Request, user.Id, accessToken);
                else
                    return await _ISiteTokenConversion.SiteTokenConversionAsync(Request, user.Id, accessToken);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region "History Method"

        [Authorize]
        [HttpPost("TradeSettledHistory")]
        public async Task<ActionResult<TradeSettledHistoryResponseV1>> TradeSettledHistory([FromBody]TradeSettledHistoryRequestFront request)
        {
            Int16 trnType = 999, marketType = 999;
            long PairId = 999;
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
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
                        return new TradeSettledHistoryResponseV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InValidTrnType, ReturnMsg = "Fail", Response = new List<TradeSettledHistoryV1>() };
                }
                if (!string.IsNullOrEmpty(request.OrderType))
                {
                    marketType = _frontTrnService.IsValidMarketType(request.OrderType);
                    if (marketType == 999)
                        return new TradeSettledHistoryResponseV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InvalidMarketType, ReturnMsg = "Fail", Response = new List<TradeSettledHistoryV1>() };
                }
                if (!string.IsNullOrEmpty(request.FromDate))
                {
                    var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new TradeSettledHistoryResponseV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                }
                return _backOfficeService.TradeSettledHistoryV1(PageSize: request.PageSize, PageNo: request.PageNo, PairID: PairId, TrnType: trnType, FromDate: request.FromDate, Todate: request.ToDate, OrderType: marketType, TrnNo: request.TrnNo, MemberID: user.Id, IsMargin: request.IsMargin);

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [Authorize]
        [HttpPost("GetCopiedLeaderOrders")]
        public async Task<ActionResult<CopiedLeaderOrdersResponseV1>> GetCopiedLeaderOrders([FromBody]CopiedLeaderOrdersRequest request)
        {
            CopiedLeaderOrdersResponseV1 Response = new CopiedLeaderOrdersResponseV1();
            Int16 trnType = 999;
            long PairId = 999;
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (!string.IsNullOrEmpty(request.Pair) || !string.IsNullOrEmpty(request.TrnType) || !string.IsNullOrEmpty(request.FromDate))
                {
                    if (!string.IsNullOrEmpty(request.Pair))
                    {
                        var Res = _frontTrnService.ValidatePairCommonMethod(request.Pair, ref PairId, 0);
                        if (Res.ErrorCode != enErrorCode.Success)
                            return new CopiedLeaderOrdersResponseV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                    }
                    if (!string.IsNullOrEmpty(request.TrnType))
                    {
                        trnType = _frontTrnService.IsValidTradeType(request.TrnType);
                        if (trnType == 999)
                            return new CopiedLeaderOrdersResponseV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InValidTrnType, ReturnMsg = "Fail", Response = new List<CopiedLeaderOrdersInfoV1>() };
                    }
                    if (!string.IsNullOrEmpty(request.FromDate))
                    {
                        var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
                        if (Res.ErrorCode != enErrorCode.Success)
                            return new CopiedLeaderOrdersResponseV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                    }
                }
                return _frontTrnService.GetCopiedLeaderOrdersV1(user.Id, request.FromDate, request.ToDate, PairId, trnType, FollowTradeType: request.FollowTradeType, FollowingTo: request.FollowingTo, PageNo: request.PageNo, PageSize: request.PageSize);

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region "Trading Data Method"
        [HttpGet("GetBuyerBook/{Pair}")]
        public ActionResult<GetBuySellBookResponse> GetBuyerBook(string Pair, short IsMargin = 0)
        {
            try
            {
                long PairId = 999;
                var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref PairId, IsMargin);
                if (Res.ErrorCode != enErrorCode.Success)
                    return new GetBuySellBookResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

                return _frontTrnService.GetBuyerSellerBookV1(PairId, 4, IsMargin);//Rita 22-2-19 for Margin Trading Data bit
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

                return _frontTrnService.GetBuyerSellerBookV1(PairId, 5, IsMargin);//Rita 22-2-19 for Margin Trading Data bit
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpGet("GetVolumeData/{BasePair}")]
        public ActionResult<GetVolumeDataResponse> GetVolumeData(string BasePair, short IsMargin = 0)
        {
            GetVolumeDataResponse Response = new GetVolumeDataResponse();
            try
            {
                long BasePairId = _frontTrnService.GetBasePairIdByName(BasePair, IsMargin);
                if (BasePairId == 0)
                {
                    return new GetVolumeDataResponse() { ErrorCode = enErrorCode.InvalidPairName, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                }
                var responsedata = _frontTrnService.GetVolumeData(BasePairId, IsMargin);
                if (responsedata != null && responsedata.Count != 0)
                    return new GetVolumeDataResponse() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success", response = responsedata };
                else
                    return new GetVolumeDataResponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetTradePairAsset")]
        public ActionResult<TradePairAssetResponce> GetTradePairAsset(short IsMargin = 0)
        {
            TradePairAssetResponce Response = new TradePairAssetResponce();
            try
            {
                List<BasePairResponse> responsedata;
                if (IsMargin == 1)//Rita 22-2-19 for Margin Trading Data bit
                    responsedata = _frontTrnService.GetTradePairAssetMargin();
                else
                    responsedata = _frontTrnService.GetTradePairAsset();

                if (responsedata != null && responsedata.Count != 0)
                    return new TradePairAssetResponce() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success", response = responsedata };
                else
                    return new TradePairAssetResponce() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetTradePairByName/{Pair}")]
        public ActionResult<TradePairByNameResponse> GetTradePairByName(string Pair, short IsMargin = 0)
        {
            TradePairByNameResponse Response = new TradePairByNameResponse();
            try
            {
                long PairId = 999;
                var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref PairId, IsMargin);
                if (Res.ErrorCode != enErrorCode.Success)
                    return new TradePairByNameResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

                if (IsMargin == 1)
                    Response.response = _frontTrnService.GetTradePairByNameMargin(PairId);
                else
                    Response.response = _frontTrnService.GetTradePairByName(PairId);

                Response.ReturnCode = enResponseCode.Success;
                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnMsg = "Success";
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpGet("GetGraphDetail/{Pair}/{Interval}")]
        public ActionResult<GetGraphDetailReponse> GetGraphDetail(string Pair, string Interval, short IsMargin = 0)
        {
            int IntervalTime = 0;
            string IntervalData = "";
            GetGraphDetailReponse Response = new GetGraphDetailReponse();
            try
            {
                long PairId = 999;
                var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref PairId, IsMargin);
                if (Res.ErrorCode != enErrorCode.Success)
                    return new GetGraphDetailReponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

                _frontTrnService.GetIntervalTimeValue(Interval, ref IntervalTime, ref IntervalData);
                if (IntervalTime == 0)
                    return new GetGraphDetailReponse() { ErrorCode = enErrorCode.Graph_InvalidIntervalTime, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };

                var responsedata = _frontTrnService.GetGraphDetail(PairId, IntervalTime, IntervalData, IsMargin);//Rita 22-2-19 for Margin Trading Data bit
                if (responsedata != null && responsedata.Count != 0)
                    return new GetGraphDetailReponse() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success", response = responsedata };
                else
                    return new GetGraphDetailReponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetMarketCap/{Pair}")]
        public ActionResult<MarketCapResponse> GetMarketCap(string Pair, short IsMargin = 0)
        {
            MarketCapResponse Response = new MarketCapResponse();
            try
            {
                long PairId = 999;
                var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref PairId, IsMargin);
                if (Res.ErrorCode != enErrorCode.Success)
                    return new MarketCapResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

                if (IsMargin == 1)
                    Response.response = _frontTrnService.GetMarketCapMargin(PairId);
                else
                    Response.response = _frontTrnService.GetMarketCap(PairId);
                Response.ReturnCode = enResponseCode.Success;
                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnMsg = "Success";
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetVolumeDataByPair/{Pair}")]
        public ActionResult<GetVolumeDataByPairResponse> GetVolumeDataByPair(string Pair, short IsMargin = 0)
        {
            GetVolumeDataByPairResponse Response = new GetVolumeDataByPairResponse();
            try
            {
                long PairId = 999;
                var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref PairId, IsMargin);
                if (Res.ErrorCode != enErrorCode.Success)
                    return new GetVolumeDataByPairResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

                VolumeDataRespose responsedata;
                if (IsMargin == 1)
                    responsedata = _frontTrnService.GetVolumeDataByPairMargin(PairId);
                else
                    responsedata = _frontTrnService.GetVolumeDataByPair(PairId);

                if (responsedata != null)
                    return new GetVolumeDataByPairResponse() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success", response = responsedata };
                else
                    return new GetVolumeDataByPairResponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetPairRates/{Pair}")]
        public ActionResult<GetPairRatesResponse> GetPairRates(string Pair, short IsMargin = 0)
        {
            GetPairRatesResponse Response = new GetPairRatesResponse();
            try
            {
                long PairId = 999;
                var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref PairId, IsMargin);
                if (Res.ErrorCode != enErrorCode.Success)
                    return new GetPairRatesResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

                var responsedata = _frontTrnService.GetPairRates(PairId);
                if (responsedata != null)
                    return new GetPairRatesResponse() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success", response = responsedata };
                else
                    return new GetPairRatesResponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetMarketTicker")]
        public ActionResult<GetMarketTickerResponse> GetMarketTicker(short IsMargin = 0)
        {
            GetMarketTickerResponse Response = new GetMarketTickerResponse();
            try
            {
                var responsedata = _frontTrnService.GetMarketTicker(IsMargin);

                if (responsedata != null && responsedata.Count != 0)
                    return new GetMarketTickerResponse() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success", Response = responsedata };
                else
                    return new GetMarketTickerResponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetMarketTickerSignalR")]
        public ActionResult<BizResponseClass> GetMarketTickerSignalR(short IsMargin = 0)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                var responsedata = _frontTrnService.GetMarketTickerSignalR(IsMargin);

                if (responsedata != 0)
                    return new BizResponseClass() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success" };
                else
                    return new BizResponseClass() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetMarketDepthChart/{Pair}")]
        public ActionResult<GetMarketDepthChartResponse> GetMarketDepthChart(string Pair, short IsMargin = 0)
        {
            //Uday 07-01-2019  MarketDepth Chart based on buyer and seller book.
            GetMarketDepthChartResponse Response = new GetMarketDepthChartResponse();
            try
            {
                long PairId = 999;
                var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref PairId, IsMargin);
                if (Res.ErrorCode != enErrorCode.Success)
                    return new GetMarketDepthChartResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

                GetBuySellMarketBook responsedata;
                if (IsMargin == 1)
                    responsedata = _frontTrnService.GetMarketDepthChartMargin(PairId); //Get market depth chart based on buyer and seller book
                else
                    responsedata = _frontTrnService.GetMarketDepthChart(PairId); //Get market depth chart based on buyer and seller book

                if (responsedata != null)
                    return new GetMarketDepthChartResponse() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success" };
                else
                    return new GetMarketDepthChartResponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [Authorize]
        [HttpGet("GetHistoricalPerformance/{LeaderId}")]
        public async Task<ActionResult<GetHistoricalPerformanceResponse>> GetHistoricalPerformance(long LeaderId)
        {
            List<HistoricalPerformanceYear> Response = new List<HistoricalPerformanceYear>();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                //Uday 30-01-2019 Get historical performance of Leader
                if (LeaderId == 0)  // Get the data of login user
                    Response = _frontTrnService.GetHistoricalPerformance(user.Id);
                else // get the data of leader
                    Response = _frontTrnService.GetHistoricalPerformance(LeaderId);

                if (Response.Count == 0)
                    return Ok(new GetHistoricalPerformanceResponse() { Response = Response, ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.HistoricalPerformance_LeaderNotFound, ReturnMsg = "Leader id not found" });

                return Ok(new GetHistoricalPerformanceResponse() { Response = Response, ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region "Favourite Pair Method"
        [Authorize]
        [HttpPost("AddToFavouritePair/{PairId}")]
        public async Task<ActionResult<BizResponseClass>> AddToFavouritePair(long PairId, short IsMargin = 0)
        {
            try
            {
                // khushali 15-03-2019 for use API Key Authorization
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                if (user == null)
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };

                if (PairId == 0)
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FavPair_InvalidPairId, ErrorCode = enErrorCode.FavPair_InvalidPairId };

                int returnCode;
                if (IsMargin == 1)
                    returnCode = _frontTrnService.AddToFavouritePairMargin(PairId, user.Id);
                else
                    returnCode = _frontTrnService.AddToFavouritePair(PairId, user.Id);

                if (returnCode == 2)
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FavPair_InvalidPairId, ErrorCode = enErrorCode.FavPair_InvalidPairId };

                if (returnCode == 1)
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FavPair_AlreadyAdded, ErrorCode = enErrorCode.FavPair_AlreadyAdded };
                else
                    return new BizResponseClass() { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.FavPair_AddedSuccess, ErrorCode = enErrorCode.FavPair_AddedSuccess };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [Authorize]
        [HttpPost("RemoveFromFavouritePair/{PairId}")]
        public async Task<ActionResult<BizResponseClass>> RemoveFromFavouritePair(long PairId, short IsMargin = 0)
        {
            try
            {
                // khushali 15-03-2019 for use API Key Authorization
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };

                if (PairId == 0)
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FavPair_InvalidPairId, ErrorCode = enErrorCode.FavPair_InvalidPairId };

                int returnCode;
                if (IsMargin == 1)
                    returnCode = _frontTrnService.RemoveFromFavouritePairMargin(PairId, user.Id);
                else
                    returnCode = _frontTrnService.RemoveFromFavouritePair(PairId, user.Id);

                if (returnCode == 1)
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FavPair_InvalidPairId, ErrorCode = enErrorCode.FavPair_InvalidPairId };
                else
                    return new BizResponseClass() { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.FavPair_RemoveSuccess, ErrorCode = enErrorCode.FavPair_RemoveSuccess };
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [Authorize]
        [HttpGet("GetFavouritePair")]
        public async Task<ActionResult<FavoritePairResponse>> GetFavouritePair(short IsMargin = 0)
        {
            FavoritePairResponse Response = new FavoritePairResponse();
            try
            {
                // khushali 15-03-2019 for use API Key Authorization
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                if (user == null)
                {
                    return new FavoritePairResponse() { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    var UserId = user.Id;
                    var response = _frontTrnService.GetFavouritePair(UserId, IsMargin);
                    if (response != null && response.Count != 0)
                    {
                        Response.response = response;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ErrorCode = enErrorCode.Success;
                        Response.ReturnMsg = "Success";
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.FavPair_NoPairFound;
                        Response.ErrorCode = enErrorCode.FavPair_NoPairFound;
                        Response.ReturnMsg = "Fail";
                    }
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region TopGainer And TopLooser Front
        [HttpGet("GetFrontTopGainerPair/{Type}")]
        public ActionResult<TopLooserGainerPairDataResponse> GetFrontTopGainerPair(int Type)
        {
            try
            {
                //Uday 04-01-2019  Top Gainer Pair Data give with Different Filteration
                TopLooserGainerPairDataResponse Response = new TopLooserGainerPairDataResponse();

                if (Type == Convert.ToInt32(EnTopLossGainerFilterType.VolumeWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangePerWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.LTPWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangeValueWise))
                {
                    var Data = _frontTrnService.GetFrontTopGainerPair(Type);

                    if (Data.Count != 0)
                        return new TopLooserGainerPairDataResponse() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success", Response = Data };
                    else
                        return new TopLooserGainerPairDataResponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
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

        [HttpGet("GetFrontTopLooserPair/{Type}")]
        public ActionResult<TopLooserGainerPairDataResponse> GetFrontTopLooserPair(int Type)
        {
            try
            {
                //Uday 04-01-2019  Top Looser Pair Data give with Different Filteration
                TopLooserGainerPairDataResponse Response = new TopLooserGainerPairDataResponse();

                if (Type == Convert.ToInt32(EnTopLossGainerFilterType.VolumeWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangePerWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.LTPWise) || Type == Convert.ToInt32(EnTopLossGainerFilterType.ChangeValueWise))
                {
                    var Data = _frontTrnService.GetFrontTopLooserPair(Type);

                    if (Data.Count != 0)
                        return new TopLooserGainerPairDataResponse() { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success", Response = Data };
                    else
                        return new TopLooserGainerPairDataResponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
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

        [HttpGet("GetFrontTopLooserGainerPair")]
        public ActionResult<TopLooserGainerPairDataResponse> GetFrontTopLooserGainerPair()
        {
            try
            {
                //Uday 04-01-2019  Top Gainer/Looser All Pair Data with name wise ascending order
                TopLooserGainerPairDataResponse Response = new TopLooserGainerPairDataResponse();

                var Data = _frontTrnService.GetFrontTopLooserGainerPair();
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

        [HttpGet("TopProfitGainer/{date}/{size}")]
        public ActionResult<TopProfitGainerLoserResponse> TopProfitGainer(DateTime date, int size)
        {
            TopProfitGainerLoserResponse response = new TopProfitGainerLoserResponse();
            try
            {
                response = _frontTrnService.GetTopProfitGainer(date, size);
                return response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("TopProfitLoser/{date}/{size}")]
        public ActionResult<TopProfitGainerLoserResponse> TopProfitLoser(DateTime date, int size)
        {
            // DateTime? CurDate; //komal 03 May 2019, Cleanup
            TopProfitGainerLoserResponse response = new TopProfitGainerLoserResponse();
            try
            {
                response = _frontTrnService.TopProfitLoser(date, size);
                return response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("TopLeadersList")]
        public ActionResult<TopLeadersListResponse> TopLeadersList()
        {
            TopLeadersListResponse response = new TopLeadersListResponse();
            try
            {
                response = _frontTrnService.TopLeadersList();
                return response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("TradeWatchList")]
        [Authorize]
        public async Task<ActionResult<TradeWatchListResponse>> TradeWatchList()
        {
            TradeWatchListResponse response = new TradeWatchListResponse();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                response = _frontTrnService.getTradeWatchList(user.Id);
                return response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region CoinListRequest
        [Authorize]
        [HttpPost("AddCoinRequest")]
        public async Task<ActionResult<BizResponseClass>> AddCoinRequest([FromBody] CoinListRequestRequest Request)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                BizResponseClass Response = new BizResponseClass();
                Response = _transactionConfigService.AddCoinRequest(Request, user.Id);
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [Authorize]
        [HttpPost("GetUserCoinRequest")]
        public async Task<ActionResult<CoinListRequestResponse>> GetUserCoinRequest()
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                CoinListRequestResponse Response = new CoinListRequestResponse();
                Response = _transactionConfigService.GetUserCoinRequest(user.Id);
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region  SiteToken
        [HttpPost("GetSiteTokenConversionData")]
        [Authorize]
        public async Task<ActionResult<SiteTokenConvertFundResponse>> GetSiteTokenConversionData(short IsMargin = 0)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                return _frontTrnService.GetSiteTokenConversionData(user.Id, "", "", "", "", IsMargin);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetAllSiteToken")]
        //[Authorize]
        public ActionResult<SiteTokenMasterResponse> GetAllSiteToken(short IsMargin = 0)
        {
            try
            {
                if (IsMargin == 1)
                    return _transactionConfigService.GetAllSiteTokenMargin();
                else
                    return _transactionConfigService.GetAllSiteToken(1);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        [HttpGet("TestCache")]
        //[Authorize]
        public ActionResult<BizResponseClass> TestCache()
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                BizResponse _Resp = new BizResponse();
                _IResdisTradingManagment.MakeNewTransactionEntry(_Resp);

                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        #region Listing Methods
        [HttpGet("GetBaseMarket")]
        public ActionResult<MarketResponse> GetBaseMarket(short IsMargin = 0)
        {
            MarketResponse Response = new MarketResponse();
            try
            {
                List<MarketViewModel> responsedata;
                if (IsMargin == 1)
                    responsedata = _transactionConfigService.GetAllMarketDataMargin(1);
                else
                    responsedata = _transactionConfigService.GetAllMarketData(1);

                if (responsedata == null)
                    return new MarketResponse() { Response = responsedata, ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
                else
                    return new MarketResponse() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail" };

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
        #endregion

        //#region "Arbitrage Trading Data Method"
        //[HttpGet("GetBuyerBookArbitrage/{Pair}")]
        //public ActionResult<ArbitrageBuySellResponse> GetBuyerBookArbitrage(string Pair, short IsMargin = 0)
        //{
        //    ArbitrageBuySellResponse Response = new ArbitrageBuySellResponse();

        //    try
        //    {
        //        long PairId = 999;
        //        var Res = _frontTrnService.ValidatePairCommonMethodArbitrage(Pair, ref PairId, IsMargin);
        //        if (Res.ErrorCode != enErrorCode.Success)
        //            return new ArbitrageBuySellResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

        //        var responsedata = _frontTrnService.GetExchangeProviderBuySellBookArbitrage(PairId, 4);
        //        if (responsedata != null && responsedata.Count != 0)
        //            return new ArbitrageBuySellResponse() { Response = responsedata, ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
        //        else
        //            return new ArbitrageBuySellResponse() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail" };

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }

        //}

        //[HttpGet("GetSellerBookArbitrage/{Pair}")]
        //public ActionResult<ArbitrageBuySellResponse> GetSellerBookArbitrage(string Pair, short IsMargin = 0)
        //{
        //    ArbitrageBuySellResponse Response = new ArbitrageBuySellResponse();
        //    try
        //    {
        //        long PairId = 999;
        //        var Res = _frontTrnService.ValidatePairCommonMethodArbitrage(Pair, ref PairId, IsMargin);
        //        if (Res.ErrorCode != enErrorCode.Success)
        //            return new ArbitrageBuySellResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

        //        var responsedata = _frontTrnService.GetExchangeProviderBuySellBookArbitrage(PairId, 5);
        //        if (responsedata != null && responsedata.Count != 0)
        //            return new ArbitrageBuySellResponse() { Response = responsedata, ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
        //        else
        //            return new ArbitrageBuySellResponse() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail" };

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }

        //}

        //[HttpGet("GetGraphDetailArbitrage/{Pair}/{Interval}")]
        //public ActionResult<GetGraphDetailReponse> GetGraphDetailArbitrage(string Pair, string Interval, short IsMargin = 0)
        //{
        //    int IntervalTime = 0;
        //    string IntervalData = "";
        //    GetGraphDetailReponse Response = new GetGraphDetailReponse();
        //    try
        //    {
        //        long PairId = 999;
        //        var Res = _frontTrnService.ValidatePairCommonMethodArbitrage(Pair, ref PairId, IsMargin);
        //        if (Res.ErrorCode != enErrorCode.Success)
        //            return new GetGraphDetailReponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

        //        _frontTrnService.GetIntervalTimeValue(Interval, ref IntervalTime, ref IntervalData);
        //        if (IntervalTime == 0)
        //        {
        //            return new GetGraphDetailReponse() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.Graph_InvalidIntervalTime, ReturnMsg = "Fail" };
        //        }
        //        var responsedata = _frontTrnService.GetGraphDetailArbitrage(PairId, IntervalTime, IntervalData, IsMargin);
        //        if (responsedata != null && responsedata.Count != 0)
        //        {
        //            return new GetGraphDetailReponse() { response = responsedata, ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
        //        }
        //        else
        //        {
        //            return new GetGraphDetailReponse() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpGet("GetProfitIndicatorArbitrage/{Pair}")]
        //public ActionResult<ProfitIndicatorResponse> GetProfitIndicatorArbitrage(string Pair, short IsMargin = 0)
        //{
        //    ProfitIndicatorResponse Response = new ProfitIndicatorResponse();
        //    try
        //    {
        //        long PairId = 999;
        //        var Res = _frontTrnService.ValidatePairCommonMethodArbitrage(Pair, ref PairId, IsMargin);
        //        if (Res.ErrorCode != enErrorCode.Success)
        //            return new ProfitIndicatorResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

        //        var responsedata = _frontTrnService.GetProfitIndicatorArbitrage(PairId, IsMargin);
        //        if (responsedata != null)
        //        {
        //            return new ProfitIndicatorResponse() { response = responsedata, ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
        //        }
        //        else
        //        {
        //            return new ProfitIndicatorResponse() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpGet("ExchangeProviderListArbitrage/{Pair}")]
        //public ActionResult<ExchangeProviderListResponse> ExchangeProviderListArbitrage(string Pair, short IsMargin = 0)
        //{
        //    ExchangeProviderListResponse Response = new ExchangeProviderListResponse();
        //    try
        //    {
        //        long PairId = 999;
        //        var Res = _frontTrnService.ValidatePairCommonMethodArbitrage(Pair, ref PairId, IsMargin);
        //        if (Res.ErrorCode != enErrorCode.Success)
        //            return new ExchangeProviderListResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

        //        var responsedata = _frontTrnService.ExchangeProviderListArbitrage(PairId, IsMargin);
        //        if (responsedata != null)
        //        {
        //            return new ExchangeProviderListResponse() { response = responsedata, ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
        //        }
        //        else
        //        {
        //            return new ExchangeProviderListResponse() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        ////Darshan Dholakiya added this method for arbitrage changes:07-06-2019   
        //[HttpGet("GetTradePairAssetArbitrage")]
        //public ActionResult<TradePairAssetResponce> GetTradePairAssetArbitrage(short IsMargin = 0)
        //{
        //    TradePairAssetResponce Response = new TradePairAssetResponce();
        //    try
        //    {
        //        List<BasePairResponse> responsedata = _frontTrnService.GetTradePairAssetArbitrage();

        //        if (responsedata != null && responsedata.Count != 0)
        //        {
        //            return new TradePairAssetResponce() { response = responsedata, ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
        //        }
        //        else
        //        {
        //            return new TradePairAssetResponce() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpGet("ExchangeListSmartArbitrage/{Pair}")]
        //public ActionResult<ExchangeListSmartArbitrageResponse> ExchangeListSmartArbitrage(string Pair, short ProviderCount = 5, short IsMargin = 0)
        //{
        //    ExchangeListSmartArbitrageResponse Response = new ExchangeListSmartArbitrageResponse();
        //    try
        //    {
        //        long PairId = 999;
        //        var Res = _frontTrnService.ValidatePairCommonMethodArbitrage(Pair, ref PairId, IsMargin);
        //        if (Res.ErrorCode != enErrorCode.Success)
        //            return new ExchangeListSmartArbitrageResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

        //        var responsedata = _frontTrnService.ExchangeListSmartArbitrageService(PairId, Pair, ProviderCount, IsMargin);
        //        if (responsedata != null)
        //        {
        //            return new ExchangeListSmartArbitrageResponse() { response = responsedata, ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
        //        }
        //        else
        //        {
        //            return new ExchangeListSmartArbitrageResponse() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpPost("SmartArbitrageHistory")]
        //public ActionResult<SmartArbitrageHistoryResponse> SmartArbitrageHistory([FromBody]SmartArbitrageHistoryRequest request)
        //{
        //    SmartArbitrageHistoryResponse Response = new SmartArbitrageHistoryResponse();
        //    try
        //    {
        //        long PairId = 999;
        //        Task<ApplicationUser> user1 = _userManager.GetUserAsync(HttpContext.User);

        //        if (request.Pair != "999")
        //        {
        //            var Res = _frontTrnService.ValidatePairCommonMethodArbitrage(request.Pair, ref PairId, request.IsMargin);
        //            if (Res.ErrorCode != enErrorCode.Success)
        //                return new SmartArbitrageHistoryResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
        //        }
        //        if (!string.IsNullOrEmpty(request.FromDate))
        //        {
        //            var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
        //            if (Res.ErrorCode != enErrorCode.Success)
        //                return new SmartArbitrageHistoryResponse() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
        //        }

        //        ApplicationUser user = user1.GetAwaiter().GetResult();
        //        var responsedata = _frontTrnService.SmartArbitrageHistoryList(PairId, user.Id, request.FromDate, request.ToDate, request.IsMargin);
        //        if (responsedata != null)
        //        {
        //            return new SmartArbitrageHistoryResponse() { response = responsedata, ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
        //        }
        //        else
        //        {
        //            return new SmartArbitrageHistoryResponse() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}
        //#endregion =================================================

        #region Optimized method 
        [HttpPost("GetTradeHistory")]
        [Authorize]
        public async Task<ActionResult<GetTradeHistoryResponseV1>> GetTradeHistory([FromBody] TradeHistoryRequest request)
        {
            Int16 trnType = 999, marketType = 999, status = 999;
            long PairId = 999;
            string sCondition = "1=1";
            try
            {
                var user1 = _userManager.GetUserAsync(HttpContext.User);
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
                ApplicationUser user = user1.GetAwaiter().GetResult();
                var response = _frontTrnService.GetTradeHistoryV1(user.Id, sCondition, request.FromDate, request.ToDate, request.Page, status, request.IsMargin);//Rita 22-2-19 for Margin Trading Data bit
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
                Task<ApplicationUser> user1 = _userManager.GetUserAsync(HttpContext.User);
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
                ApplicationUser user = user1.GetAwaiter().GetResult();
                var response = _frontTrnService.GetActiveOrderV1(user.Id, request.FromDate, request.ToDate, PairId, request.Page, trnType, request.IsMargin);//Rita 22-2-19 for Margin Trading Data bit
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
                Task<ApplicationUser> user1 = _userManager.GetUserAsync(HttpContext.User);
                if (Pair != "999")
                {
                    var Res = _frontTrnService.ValidatePairCommonMethod(Pair, ref PairId, IsMargin);
                    if (Res.ErrorCode != enErrorCode.Success)
                        return new GetRecentTradeResponceV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
                }
                ApplicationUser user = user1.GetAwaiter().GetResult();
                var response = _frontTrnService.GetRecentOrderV1(PairId, user.Id, IsMargin);//Rita 22-2-19 for Margin Trading Data bit
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

        //[HttpGet("GetOrderhistoryArbitrage")]
        //public ActionResult<GetOrderHistoryResponseArbitrageV1> GetOrderhistoryArbitrage(string Pair = "999")
        //{
        //    long PairId = 999;
        //    try
        //    {
        //        if (Pair != "999")
        //        {
        //            var Res = _frontTrnService.ValidatePairCommonMethodArbitrage(Pair, ref PairId, 0);
        //            if (Res.ErrorCode != enErrorCode.Success)
        //                return new GetOrderHistoryResponseArbitrageV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
        //        }
        //        var response = _frontTrnService.GetOrderHistoryArbitrageV1(PairId);
        //        return new GetOrderHistoryResponseArbitrageV1() { ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success", response = response };
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = "InternalError", ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpPost("GetTradeHistoryArbitrage")]
        //[Authorize]
        //public async Task<ActionResult<GetTradeHistoryResponseArbitrageV1>> GetTradeHistoryArbitrage([FromBody] TradeHistoryRequest request)
        //{
        //    Int16 trnType = 999, marketType = 999, status = 999;
        //    long PairId = 999;
        //    string sCondition = "1=1";
        //    try
        //    {
        //        var user1 = _userManager.GetUserAsync(HttpContext.User);
        //        if (!string.IsNullOrEmpty(request.Pair))
        //        {
        //            var Res = _frontTrnService.ValidatePairCommonMethodArbitrage(request.Pair, ref PairId, request.IsMargin);
        //            if (Res.ErrorCode != enErrorCode.Success)
        //                return new GetTradeHistoryResponseArbitrageV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

        //            sCondition += " And TTQ.PairID=" + PairId;
        //        }
        //        if (!string.IsNullOrEmpty(request.Trade) || !string.IsNullOrEmpty(request.MarketType) || !string.IsNullOrEmpty(request.FromDate))
        //        {
        //            if (!string.IsNullOrEmpty(request.Trade))
        //            {
        //                trnType = _frontTrnService.IsValidTradeType(request.Trade);
        //                if (trnType == 999)
        //                {
        //                    return new GetTradeHistoryResponseArbitrageV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InValidTrnType, ReturnMsg = "Fail", response = new List<GetTradeHistoryInfoArbitrageV1>() };
        //                }
        //                sCondition += " AND TTQ.TrnType=" + trnType;
        //            }
        //            if (!string.IsNullOrEmpty(request.MarketType))
        //            {
        //                marketType = _frontTrnService.IsValidMarketType(request.MarketType);
        //                if (marketType == 999)
        //                {
        //                    return new GetTradeHistoryResponseArbitrageV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InvalidMarketType, ReturnMsg = "Fail", response = new List<GetTradeHistoryInfoArbitrageV1>() };
        //                }
        //                sCondition += " AND OT.Id=" + marketType;
        //            }
        //            if (!string.IsNullOrEmpty(request.FromDate))
        //            {
        //                var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
        //                if (Res.ErrorCode != enErrorCode.Success)
        //                    return new GetTradeHistoryResponseArbitrageV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };

        //                sCondition += "AND TTQ.SettledDate Between {0} AND {1} ";
        //            }
        //        }
        //        if ((request.Status.ToString()) == "0")
        //        {
        //            status = 999;
        //        }
        //        else
        //        {
        //            if (request.Status != 1 && request.Status != 2 && request.Status != 9)
        //            {
        //                return new GetTradeHistoryResponseArbitrageV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InvalidStatusType, ReturnMsg = "Fail", response = new List<GetTradeHistoryInfoArbitrageV1>() };
        //            }
        //            status = Convert.ToInt16(request.Status);
        //        }
        //        ApplicationUser user = user1.GetAwaiter().GetResult();
        //        var response = _frontTrnService.GetTradeHistoryArbitrageV1(user.Id, sCondition, request.FromDate, request.ToDate, request.Page, status);
        //        if (response.Count == 0)
        //        {
        //            return new GetTradeHistoryResponseArbitrageV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail", response = new List<GetTradeHistoryInfoArbitrageV1>() };
        //        }
        //        return new GetTradeHistoryResponseArbitrageV1() { ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success", response = response };
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = "InternalError", ErrorCode = enErrorCode.Status500InternalServerError });
        //    }

        //}

        //[HttpPost("GetActiveOrderArbitrage")]
        //[Authorize]
        //public async Task<ActionResult<GetActiveOrderResponseArbitrageV1>> GetActiveOrderArbitrage([FromBody]GetActiveOrderRequest request)
        //{
        //    Int16 trnType = 999;
        //    long PairId = 999;
        //    try
        //    {
        //        // khushali 15-03-2019 for use API Key Authorization
        //        Task<ApplicationUser> user1 = _userManager.GetUserAsync(HttpContext.User);

        //        if (!string.IsNullOrEmpty(request.Pair))
        //        {
        //            var Res = _frontTrnService.ValidatePairCommonMethodArbitrage(request.Pair, ref PairId, request.IsMargin);
        //            if (Res.ErrorCode != enErrorCode.Success)
        //                return new GetActiveOrderResponseArbitrageV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
        //        }
        //        if (!string.IsNullOrEmpty(request.OrderType) || !string.IsNullOrEmpty(request.FromDate))
        //        {
        //            if (!string.IsNullOrEmpty(request.OrderType))
        //            {
        //                trnType = _frontTrnService.IsValidTradeType(request.OrderType);
        //                if (trnType == 999)
        //                {
        //                    return new GetActiveOrderResponseArbitrageV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InValidTrnType, ReturnMsg = "Fail", response = new List<ActiveOrderInfoArbitrageV1>() };
        //                }
        //            }
        //            if (!string.IsNullOrEmpty(request.FromDate))
        //            {
        //                var Res = _frontTrnService.ValidateFromDateToDateCommonMethod(request.FromDate, request.ToDate);
        //                if (Res.ErrorCode != enErrorCode.Success)
        //                    return new GetActiveOrderResponseArbitrageV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
        //            }
        //        }
        //        ApplicationUser user = user1.GetAwaiter().GetResult();
        //        var response = _frontTrnService.GetActiveOrderArbitrageV1(user.Id, request.FromDate, request.ToDate, PairId, request.Page, trnType);
        //        if (response.Count == 0)
        //        {
        //            return new GetActiveOrderResponseArbitrageV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail", response = new List<ActiveOrderInfoArbitrageV1>() };
        //        }
        //        return new GetActiveOrderResponseArbitrageV1() { ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success", response = response };
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = "InternalError", ErrorCode = enErrorCode.Status500InternalServerError });
        //    }

        //}

        //[HttpPost("GetRecentOrderArbitrage")]
        //[Authorize]
        //public async Task<ActionResult<GetRecentTradeResponceArbitrageV1>> GetRecentOrderArbitrage(string Pair = "999")
        //{
        //    long PairId = 999;
        //    try
        //    {
        //        var user1 = _userManager.GetUserAsync(HttpContext.User);
        //        if (Pair != "999")
        //        {
        //            var Res = _frontTrnService.ValidatePairCommonMethodArbitrage(Pair, ref PairId, 0);
        //            if (Res.ErrorCode != enErrorCode.Success)
        //                return new GetRecentTradeResponceArbitrageV1() { ErrorCode = Res.ErrorCode, ReturnCode = Res.ReturnCode, ReturnMsg = Res.ReturnMsg };
        //        }
        //        ApplicationUser user = user1.GetAwaiter().GetResult();
        //        var response = _frontTrnService.GetRecentOrderArbitrageV1(PairId, user.Id);
        //        if (response.Count == 0)
        //        {
        //            return new GetRecentTradeResponceArbitrageV1() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.NoDataFound, ReturnMsg = "Fail", response = new List<RecentOrderInfoArbitrageV1>() };
        //        }
        //        return new GetRecentTradeResponceArbitrageV1() { ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success", response = response };
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = "InternalError", ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        #endregion

        [HttpGet("ConnectToExchangeAsync")]
        public async Task<ActionResult> ConnectToExchangeAsync()
        {
            try
            {
                return Ok(_frontTrnService.CCXTBalanceCheckAsync("binance", "MyKey", "usmCya0maPNWpA5ZuRTxCgUscl64YoBVZ1CCuMyxnLtwAkPXnSKtivyICJtzFcUC", "Jw69g605snLLhX6nv1Xe3TfqFIvYxC1EVngPTiEq2a1jGMbSedOYXtYnIt8c0QH2"));
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = "InternalError", ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
    }
}