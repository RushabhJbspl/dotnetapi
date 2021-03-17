using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.ViewModels.WalletOperations;
using CleanArchitecture.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.ViewModels.Wallet;
using Microsoft.AspNetCore.Identity;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Web.Helper;
using Microsoft.AspNetCore.Authentication;

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class WalletOperationsController : Controller
    {
        private readonly IBasePage _basePage;
        private readonly ILogger<WalletOperationsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWalletService _walletService;
        public WalletOperationsController(ILogger<WalletOperationsController> logger, UserManager<ApplicationUser> userManager, IBasePage basePage, IWalletService walletService)
        {
            _logger = logger;
            _userManager = userManager;
            _walletService = walletService;
            _basePage = basePage;
        }

        [HttpPost("{Coin}/{AccWalletID}")]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateWalletAddress(string Coin, string AccWalletID)
        {
            CreateWalletAddressRes Response = new CreateWalletAddressRes();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                //var user = new ApplicationUser();user.Id = 35;
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = await _walletService.GenerateAddress(AccWalletID, Coin, accessToken, 0, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{Address}/{ConvertedAddress}")]
        public async Task<IActionResult> AddConvertedAddress(string Address, string ConvertedAddress)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser User = await _userManager.GetUserAsync(HttpContext.User);
                if (User == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _walletService.AddConvertedAddress(Address, ConvertedAddress, User.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{AccWalletID}")]
        public async Task<IActionResult> ListWalletAddress(string AccWalletID)
        {
            try
            {
                var data = _walletService.GetYearwiseWalletStatistics(71, 2018);
                ListWalletAddressResponse Response = _walletService.ListAddress(AccWalletID);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{AccWalletID}")]
        public async Task<IActionResult> GetDefaultWalletAddress(string AccWalletID)
        {
            try
            {
                ListWalletAddressResponse Response = _walletService.GetAddress(AccWalletID);
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{AccWalletID}")]
        public async Task<IActionResult> GetWalletLimit(string AccWalletID)
        {
            try
            {
                LimitResponse Response = _walletService.GetWalletLimitConfig(AccWalletID);
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{AccWalletID}")]
        public async Task<IActionResult> SetWalletLimit(string AccWalletID, [FromBody]WalletLimitConfigurationReq Request)
        {
            LimitResponse response = new LimitResponse();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    response = await _walletService.SetWalletLimitConfig(AccWalletID, Request, user.Id, accessToken);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{coin}/{accWalletID}")]
        [Authorize]
        public async Task<IActionResult> Withdrawal(string coin, string accWalletID, [FromBody]WithdrawalReq Request)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            HelperForLog.WriteLogIntoFile(1, _basePage.UTC_To_IST(), this.ControllerContext.RouteData.Values["action"].ToString(), this.GetType().Name, JsonConvert.SerializeObject(Request), accessToken);
            WithdrawalRes Response = new WithdrawalRes();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _walletService.Withdrawl(coin, accWalletID, Request, user.Id);
                }
                HelperForLog.WriteLogIntoFile(2, _basePage.UTC_To_IST(), this.ControllerContext.RouteData.Values["action"].ToString(), this.GetType().Name, JsonConvert.SerializeObject(Response), "");
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{CoinName}/{BeneficiaryAddress}/{BeneficiaryName}/{WhitelistingBit}")]
        public async Task<IActionResult> AddBeneficiary(string CoinName, short WhitelistingBit, string BeneficiaryAddress, string BeneficiaryName)
        {
            BeneficiaryResponse response = new BeneficiaryResponse();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    response.BizResponse.ReturnCode = enResponseCode.Fail;
                    response.BizResponse.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    response.BizResponse.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    response = _walletService.AddBeneficiary(CoinName, WhitelistingBit, BeneficiaryName, BeneficiaryAddress, user.Id, accessToken);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> WhitelistBeneficiary([FromBody] BulkBeneUpdateReq Request)
        {
            BeneficiaryResponse response = new BeneficiaryResponse();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    response.BizResponse.ReturnCode = enResponseCode.Fail;
                    response.BizResponse.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    response.BizResponse.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    response = _walletService.UpdateBulkBeneficiary(Request, user.Id, accessToken);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{AccWalletID}")]
        public async Task<IActionResult> UpdateBeneficiaryDetails(string AccWalletID, [FromBody] BeneficiaryUpdateReq Request)
        {
            BeneficiaryResponse response = new BeneficiaryResponse();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    response.BizResponse.ReturnCode = enResponseCode.Fail;
                    response.BizResponse.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    response.BizResponse.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    response = _walletService.UpdateBeneficiaryDetails(Request, AccWalletID, user.Id, accessToken);
                }
                var respObj = JsonConvert.SerializeObject(response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{AccWalletID}")]
        public async Task<IActionResult> GetWhitelistedBeneficiaries(string AccWalletID)
        {
            BeneficiaryResponse1 response = new BeneficiaryResponse1();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    response.BizResponse.ReturnCode = enResponseCode.Fail;
                    response.BizResponse.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    response.BizResponse.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    response = _walletService.ListWhitelistedBeneficiary(AccWalletID, user.Id);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBeneficiaries()
        {
            BeneficiaryResponse response = new BeneficiaryResponse();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    response.BizResponse.ReturnCode = enResponseCode.Fail;
                    response.BizResponse.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    response.BizResponse.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    response = _walletService.ListBeneficiary(user.Id);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{GlobalBit}")]
        public async Task<IActionResult> SetUserPreferences(short GlobalBit)
        {
            UserPreferencesRes response = new UserPreferencesRes();
            response.BizResponse = new BizResponseClass();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    response.BizResponse.ReturnCode = enResponseCode.Fail;
                    response.BizResponse.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    response.BizResponse.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    response = _walletService.SetPreferences(user.Id, Convert.ToInt16(GlobalBit), accessToken);
                }
                var respObj = JsonConvert.SerializeObject(response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPreferences()
        {
            UserPreferencesRes response = new UserPreferencesRes();
            response.BizResponse = new BizResponseClass();
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    response.BizResponse.ReturnCode = enResponseCode.Fail;
                    response.BizResponse.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    response.BizResponse.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    response = _walletService.GetPreferences(user.Id);
                }
                var respObj = JsonConvert.SerializeObject(response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{Coin}/{AddressCount}")]
        public async Task<IActionResult> CreateETHAddress(string Coin, int AddressCount)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            try
            {
                CreateWalletAddressRes responseClass = new CreateWalletAddressRes();
                if (user == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    responseClass = await _walletService.CreateETHAddress(Coin, AddressCount, user.Id, accessToken);
                }
                return Ok(responseClass);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #region Staking Policy

        [HttpGet("{StatkingTypeID}/{CurrencyTypeID}")]
        public async Task<IActionResult> GetStackingPolicyDetail(short StatkingTypeID, short CurrencyTypeID)
        {
            ListStakingPolicyDetailRes Response = new ListStakingPolicyDetailRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _walletService.GetStakingPolicy(StatkingTypeID, CurrencyTypeID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UserStackingPolicyRequest([FromBody] StakingHistoryReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser User = await _userManager.GetUserAsync(HttpContext.User);
                if (User == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _walletService.UserStackingRequest(Request, User.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPreStackingConfirmationData([FromBody]PreStackingConfirmationReq Request)
        {
            ListPreStackingConfirmationRes Response = new ListPreStackingConfirmationRes();
            try
            {
                ApplicationUser User = await _userManager.GetUserAsync(HttpContext.User);
                if (User == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _walletService.GetPreStackingData(Request, User.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageSize}/{PageNo}")]
        public async Task<IActionResult> GetStackingHistory(DateTime? FromDate, DateTime? ToDate, EnStakeUnStake? Type, int PageSize, int PageNo, EnStakingSlabType? Slab, EnStakingType? StakingType)
        {
            ListStakingHistoryRes Response = new ListStakingHistoryRes();
            try
            {
                ApplicationUser User = await _userManager.GetUserAsync(HttpContext.User);
                if (User == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _walletService.GetStackingHistoryData(FromDate, ToDate, Type, PageSize, PageNo, Slab, StakingType, User.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPreUnStackingConfirmationData([FromBody]PreUnstackingConfirmationReq Request)
        {
            UnstakingDetailRes Response = new UnstakingDetailRes();
            try
            {
                ApplicationUser User = await _userManager.GetUserAsync(HttpContext.User);
                if (User == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _walletService.GetPreUnstackingData(Request, User.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UserUnstackingRequest([FromBody] UserUnstakingReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser User = await _userManager.GetUserAsync(HttpContext.User);
                if (User == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _walletService.UserUnstackingRequest(Request, User.Id);
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

