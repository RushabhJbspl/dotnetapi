using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.ViewModels.Wallet;
using CleanArchitecture.Core.ViewModels.WalletOpnAdvanced;
using CleanArchitecture.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class WalletOpnAdvancedController : Controller
    {
        #region Cotr
        private readonly IWalletService _walletService;
        private readonly IBasePage _basePage;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<WalletController> _logger;
        public WalletOpnAdvancedController(ILogger<WalletController> logger, IBasePage basePage, UserManager<ApplicationUser> userManager, IWalletService walletService)
        {
            _logger = logger;
            _basePage = basePage;
            _userManager = userManager;
            _walletService = walletService;
        }
        #endregion

        #region Method

        [HttpGet]
        public async Task<IActionResult> ListUserWalletRequest()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListAddWalletRequest Response = new ListAddWalletRequest();
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
                    Response = _walletService.ListAddUserWalletRequest(user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{Status}/{RequestId}")]
        public async Task<IActionResult> UpdateUserWalletPendingRequest(ServiceStatus Status, long RequestId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
            if (Status == 0)
            {
                Response.ReturnCode = enResponseCode.Fail;
                Response.ReturnMsg = EnResponseMessage.InvalidStatus;
                Response.ErrorCode = enErrorCode.InvalidStatus;
                return Ok(Response);
            }
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
                    Response = await _walletService.UpdateUserWalletPendingRequest(Convert.ToInt16(Status), RequestId, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertUserWalletPendingRequest([FromBody]InsertWalletRequest request)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
            Regex regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            bool isValid = regex.IsMatch(request.Email.Trim());
            if (!isValid)
            {
                Response.ReturnCode = enResponseCode.Fail;
                Response.ReturnMsg = EnResponseMessage.EmailFail;
                Response.ErrorCode = enErrorCode.Status4087EmailFail;
                return Ok(Response);
            }
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
                    Response = await _walletService.InsertUserWalletPendingRequest(request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{WalletId}")]
        public async Task<IActionResult> ListUserWalletWise(string WalletId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListUserWalletWise Response = new ListUserWalletWise();
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
                    Response = await _walletService.ListUserWalletWise(WalletId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListWalletNew(string Coin)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListWalletMasterRes Response = new ListWalletMasterRes();
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
                    Response = await _walletService.ListWalletMasterResponseNew(user.Id, Coin);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{Coin}")]
        public async Task<IActionResult> GetWalletByTypeNew(string Coin)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListWalletResNew Response = new ListWalletResNew();
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
                    Response = _walletService.GetWalletByCoinNew(user.Id, Coin);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{WalletId}")]
        public async Task<IActionResult> GetWalletByWalletIdNew(string WalletId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListWalletResNew Response = new ListWalletResNew();
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
                    Response = _walletService.GetWalletByIdNew(user.Id, WalletId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Cold Wallet
        [HttpPost("{Coin}")]
        public async Task<IActionResult> CreateColdWallet(string Coin, [FromBody]InsertColdWalletRequest request)
        {
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
                    Response = await _walletService.ColdWallet(Coin, request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region ERC-20

        [HttpPost("{Coin}/{AccWalletId}")]
        public async Task<IActionResult> CreateERC20Address(string Coin, string AccWalletId)
        {
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
                    Response = _walletService.CreateERC20Address(user.Id, Coin, AccWalletId,1);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Leader Board

        [HttpGet]
        public async Task<IActionResult> LeaderBoard(int? UserCount)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListLeaderBoardRes Response = new ListLeaderBoardRes();
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
                    Response = _walletService.LeaderBoard(UserCount);
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
