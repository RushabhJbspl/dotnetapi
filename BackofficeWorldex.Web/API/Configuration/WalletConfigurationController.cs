using System;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.ViewModels.WalletConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackofficeWorldex.Web.API.Configuration
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class WalletConfigurationController : Controller
    {
        //vsolanki 11-10-2018
        #region DI

        private readonly IWalletConfigurationService _walletConfigurationService;
        private readonly UserManager<ApplicationUser> _userManager;

        #endregion

        #region cotr

        public WalletConfigurationController(IWalletConfigurationService walletConfigurationService, UserManager<ApplicationUser> userManager)
        {
            _walletConfigurationService = walletConfigurationService;
            _userManager = userManager;
        }

        #endregion

        #region Methods

        //vsolanki 11-10-2018

        #region WalletTypeMaster

        [HttpGet]
        public IActionResult ListAllWalletTypeMaster()
        {
            try
            {
                var items = _walletConfigurationService.ListAllWalletTypeMaster();
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
                var items = _walletConfigurationService.AddWalletTypeMaster(addWalletTypeMasterRequest, Userid);
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
                var items = _walletConfigurationService.UpdateWalletTypeMaster(updateWalletTypeMasterRequest, Userid, WalletTypeId);
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
                var items = _walletConfigurationService.DisableWalletTypeMaster(WalletTypeId);
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
                var items = _walletConfigurationService.GetWalletTypeMasterById(WalletTypeId);
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

        #region Transfer In/Out

        //vsoalnki 2018-11-2
        [HttpGet("{Page}/{PageSize}/{Coin}")]
        public async Task<IActionResult> GetTransferIn(int Page,int PageSize, string Coin,long? UserId,string Address, string TrnID,long? OrgId)
        {
            try
            {
                var items = _walletConfigurationService.GetTransferIn(Coin, Page, PageSize, UserId, Address, TrnID, OrgId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //Rushabh 02-11-2018
        [HttpGet("{Page}/{PageSize}/{Coin}")]
        public  async Task<IActionResult> GetTransferOut(int Page, int PageSize,string Coin,long? UserId, string Address,string TrnID, long? OrgId)
        {
            try
            {
                var items = _walletConfigurationService.GetTransferOutHistory(Coin, Page, PageSize ,UserId, Address, TrnID, OrgId);
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

        #endregion
    }
}
