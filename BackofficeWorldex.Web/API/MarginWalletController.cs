 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.ViewModels.ControlPanel;
using CleanArchitecture.Core.ViewModels.Wallet;
using CleanArchitecture.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class MarginWalletController : Controller
    {
        #region Cotr
        private readonly IMarginWalletService _walletService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMarginTransactionWallet _walletTranx;
        public MarginWalletController(UserManager<ApplicationUser> userManager, IMarginWalletService walletService, IMarginTransactionWallet walletTranx)
        {
            _userManager = userManager;
            _walletService = walletService;
            _walletTranx = walletTranx;
        }
        #endregion

        #region MarginTrading

        [HttpGet("{WalletTypeId}/{Amount}/{AccWalletID}")]
       // [AllowAnonymous]
        public async Task<IActionResult> GetMarginPreConfirmationData(long WalletTypeId, decimal Amount, string AccWalletID,decimal? Leverage)
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 35;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            MarginPreConfirmationRes Response = new MarginPreConfirmationRes();
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
                    Response = _walletService.GetMarginPreConfirmationData(WalletTypeId, Amount, AccWalletID, user.Id,0, Convert.ToDecimal((Leverage == null) ? 0 : Leverage));
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{WalletTypeId}/{Amount}/{AccWalletID}")]
       // [AllowAnonymous]
        public async Task<IActionResult> InsertMarginRequest(long WalletTypeId, decimal Amount, string AccWalletID,decimal Leverage) //making leverage compulsory
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 35;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
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
                    Response = _walletService.InsertMarginRequest(WalletTypeId, Amount, AccWalletID, user.Id, 0, Leverage);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //ntrivedi comment due to move to control panel
        //2019-02-23 move to control panel
        //[HttpPost("{IsApproved}/{ReuestId}")]
        ////[AllowAnonymous]
        //public async Task<IActionResult> MarginLeverageRequestAdminApproval(short IsApproved, long ReuestId,string Remarks)
        //{
        //  //  ApplicationUser user = new ApplicationUser(); user.Id = 35;
        //    ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        var accessToken = await HttpContext.GetTokenAsync("access_token");
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            Response = _walletService.AdminMarginChargeRequestApproval(IsApproved, ReuestId, Remarks);
        //        }
        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        #endregion

        #region Margin Wallet Create

        // [AllowAnonymous]
        [HttpPost("{WalletTypeId}")]
        public async Task<IActionResult> CreateMarginWallet(long WalletTypeId)
        {
            // ApplicationUser user = new ApplicationUser(); user.Id = 28;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
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
                    Response = _walletService.CreateMarginWallet(WalletTypeId, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<IActionResult> ListMarginWalletMaster(long? WalletTypeId, EnWalletUsageType? WalletUsageType, short? Status, string AccWalletId)
        {
            // ApplicationUser user = new ApplicationUser(); user.Id = 35;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListMarginWallet Response = new ListMarginWallet();
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
                    Response = await _walletService.ListMarginWalletMaster(WalletTypeId, WalletUsageType, Status, AccWalletId, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region WalletType MAster

        [HttpGet]
        public IActionResult ListAllWalletTypeMaster()
        {
            try
            {
                var items = _walletService.ListAllWalletTypeMaster();
                return Ok(items);
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

        #endregion

        #region Leverage Report

        [HttpGet("{PageNo}/{PageSize}")]
        //[AllowAnonymous]
        public async Task<IActionResult> LeverageRequestReport(long? WalletTypeId,DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, short? Status)
        {
            try
            {
                ListLeaverageRes Response = new ListLeaverageRes();
              //  ApplicationUser user = new ApplicationUser(); user.Id = 35;
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.LeverageRequestReport(WalletTypeId,user.Id, FromDate, ToDate, PageNo, PageSize, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Margin Wallet Ledger

        [HttpGet("{FromDate}/{ToDate}/{WalletId}/{Page}/{PageSize}")]
       // [AllowAnonymous]
        public async Task<IActionResult> GetMarginWalletLedger(DateTime FromDate, DateTime ToDate, string WalletId, int Page, int PageSize)
        {
          //  ApplicationUser user = new ApplicationUser(); user.Id = 35;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListWalletLedgerResv1 Response = new ListWalletLedgerResv1();
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
                    Response = _walletService.GetMarginWalletLedger(FromDate, ToDate, WalletId, Page, PageSize);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        //ntrivedi 05-03-2019
        [HttpGet]
        public async Task<IActionResult> GetPairLeverageDetail(string FirstCurrency,string SecondCurrency)
        {
            PairLeverageDetailRes Response = new PairLeverageDetailRes();
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
                Response = _walletService.GetPairLeverageDetail(FirstCurrency, SecondCurrency, user.Id);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError});
            }
        }

        //ntrivedi 27-03-2019
        [HttpGet]
        public async Task<IActionResult> ListLeverageBaseCurrency(long? WalletTypeId, short? Status)
        {
            ListLeverageRes Response = new ListLeverageRes();
            try
            {
                //ApplicationUser user = new ApplicationUser(); user.Id = 1;
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _walletService.ListLeverageBaseCurrency(WalletTypeId, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MarginWithdrawPreConfirm([Required]string Currency)
        {
            MarginWithdrawPreConfirmResponse Response = new MarginWithdrawPreConfirmResponse();
            try
            {
                //ApplicationUser user = new ApplicationUser(); user.Id = 1;
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.MarginWithdrawPreConfirm(user.Id,Currency);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> WithdrawMigration([Required]string Currency)
        {
            MarginWithdrawPreConfirmResponse Response = new MarginWithdrawPreConfirmResponse();
            try
            {
                //ApplicationUser user = new ApplicationUser(); user.Id = 1;
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                     Response = _walletService.MarginWithdraw(user.Id,Currency);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpgradeLoan(long LoanID,decimal LeverageX)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                //ApplicationUser user = new ApplicationUser(); user.Id = 1;
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.UpgradeLoan(user.Id,LoanID, LeverageX);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageNo}")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetProfitNLossReportData(int PageNo,int? PageSize,long? PairId,string CurrencyName)
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 35;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            PNLAccountRes Response = new PNLAccountRes();
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
                    Response = _walletService.GetProfitNLossData(PageNo, PageSize, PairId, CurrencyName,user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<IActionResult> GetOpenPosition(long? PairId)
        {
            //ApplicationUser user = new ApplicationUser(); user.Id = 35;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            OpenPositionRes Response = new OpenPositionRes();
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
                    Response = _walletService.GetOpenPosition(PairId,user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //ntrivedi 29-04-2019 use this method for internal use if some tranx failed for batchno in charge margin collected manually without cron
        [HttpGet]
        public async Task<IActionResult> MarginCreateChargeSettleLeverageBalanceOrder([Required]long BatchNo)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                //ApplicationUser user = new ApplicationUser(); user.Id = 1;
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    bool flag = await _walletTranx.ReleaseMarginWalletforSettleLeverageBalance(BatchNo);
                    Response.ReturnCode = enResponseCode.Success;                    
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = flag.ToString();
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
