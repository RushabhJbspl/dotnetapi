using System;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Worldex.Web.API.Configuration
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class WalletConfigurationController : Controller
    {
        #region "DI"

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
        #endregion

        #endregion
    }
}
