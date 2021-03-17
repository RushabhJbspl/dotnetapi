using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.ViewModels.ControlPanel;
using CleanArchitecture.Core.ViewModels.Wallet;
using CleanArchitecture.Core.ViewModels.WalletConfiguration;
using CleanArchitecture.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanArchitecture.Web.API.ControlPanel
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class MarginWalletControlPanelController : Controller
    {
        #region Cotr
        private readonly IMarginWalletService _walletService;
        private readonly UserManager<ApplicationUser> _userManager;
        public MarginWalletControlPanelController(UserManager<ApplicationUser> userManager, IMarginWalletService walletService)
        {
            _userManager = userManager;
            _walletService = walletService;
        }
        #endregion

        #region LeverageMaster

        //2018-12-28
        [HttpGet]
        public async Task<IActionResult> ListLeverage(long? WalletTypeId, short? Status)
        {
            ListLeverageRes Response = new ListLeverageRes();
            try
            {
                ApplicationUser user = new ApplicationUser(); user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _walletService.ListLeverage(WalletTypeId, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-28
        [HttpPost]
        public async Task<IActionResult> InserUpdateLeverage([FromBody]InserUpdateLeverageReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = new ApplicationUser(); user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _walletService.InserUpdateLeverage(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-28
        [HttpPost("{Id}/{Status}")]
        public async Task<IActionResult> ChangeLeverageStatus(long Id, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = new ApplicationUser(); user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _walletService.ChangeLeverageStatus(Convert.ToInt16(Status), user.Id, Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Margin request approval

        [HttpPost("{IsApproved}/{RequestId}")]
        //[AllowAnonymous]
        public async Task<IActionResult> MarginLeverageRequestAdminApproval(short IsApproved, long RequestId, string Remarks)
        {
              ApplicationUser user = new ApplicationUser(); user.Id = 35;
            //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
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
                    Response = _walletService.AdminMarginChargeRequestApproval(IsApproved, RequestId, Remarks);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        
        #endregion

        #region Leaverage Report

        [HttpGet("{PageNo}/{PageSize}")]
        public async Task<IActionResult> LeveragePendingReport(long? WalletTypeId, long? UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize)
        {
            try
            {
                ListLeaverageReportRes Response = new ListLeaverageReportRes();
                ApplicationUser user = new ApplicationUser(); user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.LeveragePendingReport(WalletTypeId, UserId, FromDate, ToDate, PageNo, PageSize);
                }
                return Ok(Response);

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{FromDate}/{ToDate}/{PageNo}/{PageSize}")]
        public async Task<IActionResult> LeverageReport(long? WalletTypeId, long? UserId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, short? Status)
        {
            try
            {
                ListLeaverageRes Response = new ListLeaverageRes();
                ApplicationUser user = new ApplicationUser(); user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.LeverageReport(WalletTypeId,UserId, FromDate, ToDate, PageNo, PageSize, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Charge
        [HttpGet]
        public async Task<IActionResult> ListMarginChargesTypeWise(string WalletTypeName, long? TrnTypeId)
        {
            try
            {
                ListChargesTypeWise Response = new ListChargesTypeWise();
                Response = _walletService.ListMarginChargesTypeWise(WalletTypeName, TrnTypeId);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageNo}/{PageSize}")]
        public async Task<IActionResult> MarginTrnChargeLogReport(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate, long? TrnNo)
        {
            try
            {
                ListTrnChargeLogRes Response = new ListTrnChargeLogRes();
                Response = _walletService.MarginTrnChargeLogReport(PageNo, PageSize, Status, TrnTypeID, WalleTypeId, SlabType, FromDate, ToDate, TrnNo);
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
              ApplicationUser user = new ApplicationUser(); user.Id = 35;
            //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
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

        #region List Margin Wallet
        [HttpGet("{PageNo}/{PageSize}")]
        //[AllowAnonymous]
        public async Task<IActionResult> ListMarginWallet(int PageNo,int PageSize,long? WalletTypeId, EnWalletUsageType? WalletUsageType, short? Status, string AccWalletId,long? UserId)
        {
             ApplicationUser user = new ApplicationUser(); user.Id = 35;
           // ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListMarginWallet2 Response = new ListMarginWallet2();
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
                    Response = await _walletService.ListMarginWallet(PageNo,PageSize,WalletTypeId, WalletUsageType, Status, AccWalletId, UserId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{UserId}")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetMarginWalletByUserId(long UserId)
        {
            ApplicationUser user = new ApplicationUser(); user.Id = 35;
            // ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListMarginWalletByUserIdRes Response = new ListMarginWalletByUserIdRes();
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
                    Response = await _walletService.GetMarginWalletByUserId(UserId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Profit&Loss Reposrt
        [HttpGet]
        //[AllowAnonymous]
        //[Authorize]
        public async Task<IActionResult> GetOpenPosition(long? PairId,long? UserID)
        {
            ApplicationUser user = new ApplicationUser(); user.Id = 35;
            //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
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
                    Response = _walletService.GetOpenPosition(PairId, UserID);
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
       // [Authorize]
        public async Task<IActionResult> GetProfitNLossReportData(int PageNo, int? PageSize, long? PairId, string CurrencyName,long UserID=0)
        {
            ApplicationUser user = new ApplicationUser(); user.Id = 35;
            //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
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
                    Response = _walletService.GetProfitNLossData(PageNo, PageSize, PairId, CurrencyName, UserID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Margin WalletTypeMaster
        [HttpGet]
        public IActionResult ListAllWalletTypeMaster()
        {
            try
            {
                var items = _walletService.ListAllWalletTypeMasterV2();
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

        [HttpPost]
        public async Task<IActionResult> AddWalletTypeMaster([FromBody]WalletTypeMasterRequest addWalletTypeMasterRequest)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                long Userid = 1/*user.Id*/;
                var items = _walletService.AddWalletTypeMaster(addWalletTypeMasterRequest, Userid);
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

        [HttpPut("{WalletTypeId}")]
        public async Task<IActionResult> UpdateWalletTypeMaster([FromBody]WalletTypeMasterUpdateRequest updateWalletTypeMasterRequest, long WalletTypeId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                long Userid = 1/*user.Id*/;
                var items = _walletService.UpdateWalletTypeMaster(updateWalletTypeMasterRequest, Userid, WalletTypeId);
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

        [HttpDelete("{WalletTypeId}")]
        public IActionResult DeleteWalletTypeMaster(long WalletTypeId)
        {
            try
            {
                var items = _walletService.DisableWalletTypeMaster(WalletTypeId);
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

        [HttpGet("{WalletTypeId}")]
        public IActionResult GetWalletTypeMasterById(long WalletTypeId)
        {
            try
            {
                var items = _walletService.GetWalletTypeMasterById(WalletTypeId);
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
    }
}
