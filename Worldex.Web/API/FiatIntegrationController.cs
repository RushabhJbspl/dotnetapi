using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.Interfaces;
using Worldex.Infrastructure.Services;

namespace Worldex.Web.API
{
    /// <summary>
    /// vsolanki 2019-10-9 Added New Controller for Fiat COnfiguration
    /// </summary>
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class FiatIntegrationController : ControllerBase
    {
        #region COTR
        private readonly IFiatIntegrateService _fiatIntegrateService;
        private readonly IFrontTrnService _frontTrnService;
        private readonly UserManager<ApplicationUser> _userManager;

        public FiatIntegrationController(UserManager<ApplicationUser> userManager, IFiatIntegrateService fiatIntegrateService, IFrontTrnService frontTrnService)
        {
            _userManager = userManager;
            _fiatIntegrateService = fiatIntegrateService;
            _frontTrnService = frontTrnService;
        }
        #endregion

        #region Method

        /// <summary>
        /// vsolanki 2019-10-9 create method for Buy Crypto Currency from Fiat
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FiatBuyRequest([FromBody]BuyTopUpRequest Request)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BuyTopUpResponse Response = new BuyTopUpResponse();
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
                    Response = _fiatIntegrateService.FiatBuyTopUpRequest(Request, user);
                    Response.Email = user.Email;
                    Response.MobileNo = (user.Mobile == null ? "" : user.Mobile);
                    Response.DateOfBirth = "";
                    Response.UserName = user.UserName;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        /// <summary>
        /// vsolanki 2019-10-9 create api for Get Pair LTp for Fait
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetFiatLTP(short? TransactionType)
        {
            ApplicationUser user = new ApplicationUser(); user.Id = 35; //await _userManager.GetUserAsync(HttpContext.User);
            ListGetLTP Response = new ListGetLTP();
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
                    Response = _fiatIntegrateService.GetFiatLTP(TransactionType);
                }
                return Ok(Response);
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

        /// <summary>
        /// vsolanki 2019-10-9 Create APi for Update status of buy request to notify url
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> NotifyDeposit([FromBody] NotifyDepositReq Request)
        {
            ApplicationUser user = new ApplicationUser(); user.Id = 35; //await _userManager.GetUserAsync(HttpContext.User);
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
                    Response = _fiatIntegrateService.NotifyDeposit(Request);
                }
                return Ok(Response);
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

        /// <summary>
        /// vsolanki 2019-10-9 create api for Displayinghistory of Buy request
        /// </summary>
        /// <param name="FromCurrency"></param>
        /// <param name="ToCurrency"></param>
        /// <param name="Status"></param>
        /// <param name="TrnId"></param>
        /// <returns></returns>
        /// Rushabh 12-10-2019 Added Date Filteration

        [HttpGet]
        public async Task<IActionResult> FiatBuyHistory(string FromCurrency, string ToCurrency, short? Status, string TrnId, DateTime? FromDate, DateTime? ToDate)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);//new ApplicationUser(); user.Id = 35; user.Email = "nishant@jbspl.com";//
            ListFiatBuyHistory Response = new ListFiatBuyHistory();
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
                    Response = _fiatIntegrateService.FiatBuyHistory(FromCurrency, ToCurrency, Status, TrnId, user.Email, FromDate, ToDate);
                }
                return Ok(Response);
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


        [HttpGet]
        public async Task<IActionResult> FiatSellHistory(string FromCurrency, string ToCurrency, short? Status, string TrnId, DateTime? FromDate, DateTime? ToDate)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);//new ApplicationUser(); user.Id = 35; user.Email = "nishant@jbspl.com";//
            ListFiatSellHistory Response = new ListFiatSellHistory();
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
                    Response = _fiatIntegrateService.FiatSellHistory(FromCurrency, ToCurrency, Status, TrnId, user.Email, FromDate, ToDate);
                }
                return Ok(Response);
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


        /// <summary>
        /// vsolanki 2019-10-10 create for get trade pair info with chargevalue
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetFiatTradeInfo()
        {
            ApplicationUser user = new ApplicationUser(); user.Id = 35; //await _userManager.GetUserAsync(HttpContext.User);
            ListGetFiatTradeInfo Response = new ListGetFiatTradeInfo();
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
                    Response = _fiatIntegrateService.GetFiatTradeInfo();
                }
                return Ok(Response);
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

        /// <summary>
        /// vsolanki create api for update callback data
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> BuyCallBackUpdate([FromBody]InputBuyCallBackUpdateReq Request)
        {
            ApplicationUser user = new ApplicationUser(); user.Id = 35;//await _userManager.GetUserAsync(HttpContext.User);
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
                    Response = _fiatIntegrateService.BuyCallBackUpdate(Request, user.Id);
                }
                return Ok(Response);
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
        [AllowAnonymous]
        public async Task<IActionResult> SellCallBackUpdate([FromBody]InputBuyCallBackUpdateReq Request)
        {
            ApplicationUser user = new ApplicationUser(); user.Id = 35;//await _userManager.GetUserAsync(HttpContext.User);
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
                    Response = await _fiatIntegrateService.SellCallBackUpdate(Request, user.Id);
                }
                return Ok(Response);
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
        public async Task<IActionResult> FiatSellRequest([FromBody]SellRequest Request)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            SellResponseV2 Response = new SellResponseV2();
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
                    Response = _fiatIntegrateService.FiatSellTopUpRequestV1(Request, user);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListFiatCurrency()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListFiatCurrencyInfo Response = new ListFiatCurrencyInfo();
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
                    Response = _fiatIntegrateService.GetFiatCurrencyInfo();
                }
                return Ok(Response);
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
        public async Task<IActionResult> FiatConfirmationSellRequest([FromBody]FiatSellConfirmReq Request)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            GetWithdrawalTransactionResponse Response = new GetWithdrawalTransactionResponse();
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
                    Response = _fiatIntegrateService.FiatSellRequestConfirmation(Request, user);
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