using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.IEOWallet;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Web.API
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class IEOWalletController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIEOWalletService _iEOWalletService;

        public IEOWalletController(UserManager<ApplicationUser> userManager, IIEOWalletService IEOWalletService)
        {
            _userManager = userManager;
            _iEOWalletService = IEOWalletService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ListWallet(Int16 Status = 0)
        {
            ApplicationUser user = new ApplicationUser();
            user.Id = 5;//await _userManager.GetUserAsync(HttpContext.User);
            ListIEOWalletResponse Response = new ListIEOWalletResponse();
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
                    Response = _iEOWalletService.ListWallet(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{FromDate}/{ToDate}/{Page}/{PageSize}")]
        public async Task<IActionResult> TransactionHistory(DateTime FromDate, DateTime ToDate, int Page, int PageSize, Int64 PaidCurrency = 0, Int64 DeliveryCurrency = 0)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListIEOPurchaseHistoryResponse Response = new ListIEOPurchaseHistoryResponse();
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
                    Response = _iEOWalletService.ListPurchaseHistory(FromDate, ToDate, Page, PageSize, PaidCurrency, DeliveryCurrency, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> IEOPreConfirmation([FromBody] PreConfirmRequest PreConfirmRequest)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            PreConfirmResponseV2 Response = new PreConfirmResponseV2();
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
                    Response = _iEOWalletService.PreConfirmation(PreConfirmRequest.PaidAccWalletId, PreConfirmRequest.PaidQauntity, PreConfirmRequest.PaidCurrency, PreConfirmRequest.DeliveredCurrency, PreConfirmRequest.RoundGuid, PreConfirmRequest.Remarks, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> IEOConfirmTransaction([FromBody] PreConfirmRequest ConfirmRequest)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            PreConfirmResponse Response = new PreConfirmResponse();
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
                    Response = _iEOWalletService.Confirmation(ConfirmRequest.PaidAccWalletId, ConfirmRequest.PaidQauntity, ConfirmRequest.PaidCurrency, ConfirmRequest.DeliveredCurrency, ConfirmRequest.RoundGuid, ConfirmRequest.Remarks, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #region Banner Configuration
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetBannerConfiguration()
        {
            ApplicationUser user = new ApplicationUser(); user.Id = 35; //await _userManager.GetUserAsync(HttpContext.User);
            GetIEOBannerRes Response = new GetIEOBannerRes();
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
                    Response = _iEOWalletService.GetBannerConfiguration();
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