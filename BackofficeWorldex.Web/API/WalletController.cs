using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.ViewModels.Wallet;
using CleanArchitecture.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CleanArchitecture.Web.Helper;
using CleanArchitecture.Infrastructure.DTOClasses;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.ViewModels.ControlPanel;

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly UserManager<ApplicationUser> _userManager;

        public WalletController(UserManager<ApplicationUser> userManager, IWalletService walletService)
        {
            _userManager = userManager;
            _walletService = walletService;
        }

        #region"Methods"
        
        [HttpGet]
        public async Task<IActionResult> ListWallet()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListWalletResponse Response = new ListWalletResponse();
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.ListWallet(user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }            
        }

        [HttpPost("{Coin}")]
        public async Task<IActionResult> CreateWallet([FromBody]CreateWalletRequest Request, string Coin)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            CreateWalletResponse Response = new CreateWalletResponse();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _walletService.InsertIntoWalletMaster(Request.WalletName, Coin, Request.IsDefaultWallet, Request.AllowTrnType, Convert.ToInt64(user.Id), accessToken, 0, Request.OrgID, Request.ExpiryDate);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.ReturnCode = enResponseCode.InternalError;
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{AccWalletID}")]
        public async Task<IActionResult> UpdateWalletDetails(string AccWalletID, string WalletName, short? Status, byte? IsDefaultWallet)
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
                else
                {
                    Response = await _walletService.UpdateWalletDetail(AccWalletID, WalletName, Status, IsDefaultWallet, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListChargesTypeWise(string WalletTypeName, long? TrnTypeId)
        {
            ListChargesTypeWise Response = new ListChargesTypeWise();
            try
            {
                ApplicationUser User = await _userManager.GetUserAsync(HttpContext.User);
                if (User == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {                    
                    Response = _walletService.ListChargesTypeWise(WalletTypeName, TrnTypeId,User.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{Coin}")]
        public async Task<IActionResult> GetWalletByType(string Coin)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListWalletResponse Response = new ListWalletResponse();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.GetWalletByCoin(user.Id, Coin);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{Coin}/{WalletId}")]
        public async Task<IActionResult> GetWalletByWalletId(string Coin, string WalletId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListWalletResponse Response = new ListWalletResponse();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.GetWalletById(user.Id, Coin, WalletId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{FromDate}/{ToDate}")]
        public async Task<IActionResult> DepositHistoy(DateTime FromDate, DateTime ToDate, string Coin,string TrnNo ,decimal? Amount, byte? Status, int PageNo)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            WithdrawHistoryResponse response = new WithdrawHistoryResponse();
            try
            {
                if (user == null)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    response = _walletService.DepositHistoy(FromDate, ToDate, Coin, Amount, Status, TrnNo, user.Id, PageNo);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{FromDate}/{ToDate}")]
        public async Task<IActionResult> WithdrawalHistoy(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status, int PageNo)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            WithdrawHistoryNewResponse response = new WithdrawHistoryNewResponse();
            try
            {
                if (user == null)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    response = _walletService.WithdrawalHistoy(FromDate, ToDate, Coin, Amount, Status, user.Id, PageNo);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{WalletId}")]
        public async Task<IActionResult> GetAvailableBalance(string WalletId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListBalanceResponse Response = new ListBalanceResponse();
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
                    Response = _walletService.GetAvailableBalance(user.Id, WalletId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAvailableBalance()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
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

        [HttpGet("{WalletId}")]
        public async Task<IActionResult> GetUnSettledBalance(string WalletId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListBalanceResponse Response = new ListBalanceResponse();
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
                    Response = _walletService.GetUnSettledBalance(user.Id, WalletId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUnSettledBalance()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListBalanceResponse Response = new ListBalanceResponse();
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
                    Response = _walletService.GetAllUnSettledBalance(user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{WalletId}")]
        public async Task<IActionResult> GetUnClearedBalance(string WalletId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListBalanceResponse Response = new ListBalanceResponse();
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
                    Response = _walletService.GetUnClearedBalance(user.Id, WalletId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUnClearedBalance()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListBalanceResponse Response = new ListBalanceResponse();
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
                    Response = _walletService.GetAllUnClearedBalance(user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{WalletId}")]
        public async Task<IActionResult> GetStackingBalance(string WalletId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListStackingBalanceRes Response = new ListStackingBalanceRes();
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
                    Response = _walletService.GetStackingBalance(user.Id, WalletId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStackingBalance()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListStackingBalanceRes Response = new ListStackingBalanceRes();
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
                    Response = _walletService.GetAllStackingBalance(user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{WalletId}")]
        public async Task<IActionResult> GetShadowBalance(string WalletId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListBalanceResponse Response = new ListBalanceResponse();
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
                    Response = _walletService.GetShadowBalance(user.Id, WalletId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShadowBalance()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListBalanceResponse Response = new ListBalanceResponse();
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
                    Response = _walletService.GetAllShadowBalance(user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{WalletId}")]
        public async Task<IActionResult> GetAllBalances(string WalletId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            AllBalanceResponse Response = new AllBalanceResponse();
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
                    Response = _walletService.GetAllBalances(user.Id, WalletId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailbleBalTypeWise()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BalanceResponseWithLimit Response = new BalanceResponseWithLimit();
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
                    Response = _walletService.GetAvailbleBalTypeWise(user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{WalletType}")]
        public async Task<IActionResult> GetAllBalancesTypeWise(string WalletType)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListAllBalanceTypeWiseRes Response = new ListAllBalanceTypeWiseRes();
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
                    Response = _walletService.GetAllBalancesTypeWise(user.Id, WalletType);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{FromDate}/{ToDate}/{WalletId}")]
        public async Task<IActionResult> GetWalletLedger(DateTime FromDate, DateTime ToDate, string WalletId, int Page)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListWalletLedgerRes Response = new ListWalletLedgerRes();
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
                    Response = _walletService.GetWalletLedger(FromDate, ToDate, WalletId, Page);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{FromDate}/{ToDate}/{WalletId}/{Page}/{PageSize}")]
        public async Task<IActionResult> GetWalletLedgerV1(DateTime FromDate, DateTime ToDate, string WalletId, int Page, int PageSize)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListWalletLedgerResv1 Response = new ListWalletLedgerResv1();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.GetWalletLedgerV1(FromDate, ToDate, WalletId, Page, PageSize);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDefaulWallet()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _walletService.CreateDefaulWallet(user.Id, accessToken);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{WalletType}")]
        public async Task<IActionResult> CreateWalletForAllUser_NewService(string WalletType)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.CreateWalletForAllUser_NewService(WalletType);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddBizUserTypeMapping([FromBody] AddBizUserTypeMappingReq req)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.AddBizUserTypeMapping(req);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetIncomingTransaction(string Coin)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListIncomingTrnRes Response = new ListIncomingTrnRes();
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
                    Response = _walletService.GetIncomingTransaction(user.Id, Coin);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{TrnType}/{CoinName}")]
        public async Task<IActionResult> GetServiceLimitChargeValue(enWalletTrnType TrnType, string CoinName)
        {
            ServiceLimitChargeValueResponse Response = new ServiceLimitChargeValueResponse();
            ServiceLimitChargeValueResponseStr ResponseStr = new ServiceLimitChargeValueResponseStr();
            ServiceLimitChargeValueStr subObj = new ServiceLimitChargeValueStr();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    ResponseStr.ReturnCode = enResponseCode.Fail;
                    ResponseStr.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    ResponseStr.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {

                    var responseDataResult = _walletService.GetServiceLimitChargeValueV2(TrnType, CoinName, user.Id);
                    var responseData = responseDataResult.Result;
                    if (responseData != null)
                    {
                        Response.response = responseData;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ReturnMsg = EnResponseMessage.FindRecored;
                        Response.ErrorCode = enErrorCode.Success;
                        subObj.ChargeType = Response.response.ChargeType;
                        subObj.ChargeValue = Response.response.ChargeValue;
                        subObj.CoinName = Response.response.CoinName;
                        subObj.MaxAmount = Response.response.MaxAmount.ToString();
                        subObj.MinAmount = Response.response.MinAmount.ToString();
                        subObj.ChargeType = Response.response.ChargeType;
                        subObj.DeductWalletTypeName = Response.response.DeductWalletTypeName;
                        subObj.ChargeWalletBalance = Response.response.ChargeWalletBalance;
                        ResponseStr.ReturnCode = Response.ReturnCode;
                        ResponseStr.ErrorCode = Response.ErrorCode;
                        ResponseStr.ReturnMsg = Response.ReturnMsg;
                        ResponseStr.response = subObj;

                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.NotFound;
                        Response.ErrorCode = enErrorCode.NotFound;
                        ResponseStr.ReturnCode = enResponseCode.Fail;
                        ResponseStr.ReturnMsg = EnResponseMessage.NotFound;
                        ResponseStr.ErrorCode = enErrorCode.NotFound;
                    }
                }
                return Ok(ResponseStr);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOutGoingTransaction(string Coin)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListOutgoingTrnRes Response = new ListOutgoingTrnRes();
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
                    Response = _walletService.GetOutGoingTransaction(user.Id, Coin);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{FromDate}/{ToDate}")]
        public async Task<IActionResult> ConvertFundHistory(DateTime FromDate, DateTime ToDate, string Coin)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListTokenConvertHistoryRes Response = new ListTokenConvertHistoryRes();
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
                    Response = _walletService.ConvertFundHistory(user.Id, FromDate, ToDate, Coin);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{SourcePrice}")]
        public async Task<IActionResult> ConvertFund(decimal SourcePrice)
        {
            decimal DestinationPrice = 0;
            BizResponseClass c = null;
            try
            {
                DestinationPrice = _walletService.ConvertFund(SourcePrice);
                if (DestinationPrice == 0)
                {
                    c = new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmount };
                }
                else
                {
                    return Ok("DestinationPrice:" + DestinationPrice);
                }
                return Ok(c);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddIntoConvertFund(ConvertTockenReq Request)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.AddIntoConvertFund(Request, user.Id, accessToken);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{TrnType}")]
        public async Task<IActionResult> GetTransactionPolicy(long TrnType)
        {
            GetTransactionPolicyRes Response = new GetTransactionPolicyRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.ListTransactionPolicy(TrnType, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion
    }
}
