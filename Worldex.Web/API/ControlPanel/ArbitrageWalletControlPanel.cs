using CleanArchitecture.Core.ApiModels;
using System.Threading.Tasks;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.ViewModels.Wallet;
using CleanArchitecture.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CleanArchitecture.Core.ViewModels.ControlPanel;
using Microsoft.AspNetCore.Http;
using System;

namespace CleanArchitecture.Web.API.ControlPanel
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class ArbitrageWalletControlPanel : Controller
    {
        #region Cotr
        private readonly IArbitrageWalletService _ArbitrageService;
        private readonly IArbitrageWalletServiceCharge _ArbitrageServiceCharge; //Chirag 11-06-2019
        private readonly UserManager<ApplicationUser> _userManager;
        public ArbitrageWalletControlPanel(UserManager<ApplicationUser> userManager, IArbitrageWalletService ArbitrageService, IArbitrageWalletServiceCharge ArbitrageServiceCharge)
        {
            _userManager = userManager;
            _ArbitrageService = ArbitrageService;
            _ArbitrageServiceCharge = ArbitrageServiceCharge;
            //_walletTranx = walletTranx;
        }
        #endregion

        #region Balance
        [HttpGet]
        //[AllowAnonymous]
        public async Task<IActionResult> ArbitrageProviderBalance(string SMSCode, int SerProID = 0)
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 28;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ArbitrageServiceProBalanceResponse Response = new ArbitrageServiceProBalanceResponse();
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
                    Response = await _ArbitrageService.GetArbitrageProviderBalance(SerProID, SMSCode);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Arbitrage Address

        [HttpPost]
        public async Task<IActionResult> InsertUpdateArbitrageAddress([FromBody]InsertUpdateAddressReq Request)
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 1;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            InsertUpdateAddressRes Response = new InsertUpdateAddressRes();
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
                    Response = _ArbitrageService.InsertUpdateArbitrageAddress(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListArbitrageAddress(string Address, long? WalletTypeId, long? ServiceProviderId)
        {
            ApplicationUser user = new ApplicationUser(); user.Id = 1;
            //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListArbitrageAddressRes Response = new ListArbitrageAddressRes();
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
                    Response = _ArbitrageService.ListArbitrageAddress(Address, WalletTypeId, ServiceProviderId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Withdraw

        [HttpPost]
        // [AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> AddDepositFund([FromBody]ArbitrageWithdrawReq Req)
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 1;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ArbitrageWithdrawRes Response = new ArbitrageWithdrawRes();
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
                    Response = _ArbitrageService.ArbitrageWithdraw(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageNo}/{PageSize}")]
        [Authorize]
        public async Task<IActionResult> TopUpHistory(int PageNo,int PageSize,string Address, string CoinName, long? FromServiceProviderId, long? ToServiceProviderId,string TrnId,DateTime? FromDate,DateTime? ToDate,short? Status)
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 1;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListArbitrageTopUpHistory Response = new ListArbitrageTopUpHistory();
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
                    Response = _ArbitrageService.TopUpHistory(PageNo, PageSize, Address, CoinName, FromServiceProviderId, ToServiceProviderId, TrnId, FromDate, ToDate, user.Id, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Arbitrage Charge
        [HttpPost]
        public async Task<IActionResult> InsertUpdateArbitrageChargeConfigurationMaster([FromBody]InsertUpdateArbitrageChargeConfigurationMasterReq Request)
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 1;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
            try
            {
                //var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _ArbitrageServiceCharge.InsertUpdateArbitrageChargeConfigurationMaster(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListArbitrageChargeConfigurationMaster(long WalletTypeId = 0, long SerProId = 0, long PairID = 0)
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 1;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListArbitrageChargeConfigurationMasterRes Response = new ListArbitrageChargeConfigurationMasterRes();
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
                    Response = _ArbitrageServiceCharge.ListArbitrageChargeConfigurationMaster(WalletTypeId, SerProId, PairID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        [HttpGet("{FromDate}/{ToDate}/{WalletId}/{Page}/{PageSize}")]
        public async Task<IActionResult> GetArbitrageProviderWalletLedger(DateTime FromDate, DateTime ToDate, long WalletId, int Page, int PageSize)
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 35;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListProviderWalletLedgerResv1 Response = new ListProviderWalletLedgerResv1();
            //Response.BizResponseObj = new Core.ApiModels.BizResponseClass();
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
                    Response = _ArbitrageServiceCharge.GetProviderArbitrageWalletLedger(FromDate, ToDate, WalletId, Page, PageSize);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListProviderWallet(Int16 Status = 0, long SerProId = 0, string SMSCode = "")
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 1;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListProviderWalletRes Response = new ListProviderWalletRes();
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
                    Response = _ArbitrageServiceCharge.ListProviderWallet(Status, SerProId, SMSCode);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListAllArbitrageWalletTypeMaster()
        {
            ApplicationUser user = new ApplicationUser(); user.Id = 1;
            //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListProviderWalletRes Response = new ListProviderWalletRes();
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
                    var items = _ArbitrageServiceCharge.ListAllWalletTypeMaster();
                    return Ok(items);
                }
                return BadRequest(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass
                {
                    ReturnCode = enResponseCode.InternalError,
                    ReturnMsg = ex.ToString(),
                    ErrorCode = enErrorCode.Status500InternalServerError
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertUpdateArbitrageWalletTypeMaster([FromBody]InsertUpdateArbitrageWalletTypeMasterReq Request)
        {
            ApplicationUser user = new ApplicationUser(); user.Id = 1;
            //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
            try
            {
                //var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _ArbitrageServiceCharge.InsertUpdateArbitrageWalletTypeMaster(Request, user.Id);
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
