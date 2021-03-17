using System;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.KYCConfiguration;
using Worldex.Core.Services;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Worldex.Web.API.ControlPanel
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class WalletControlPanelController : Controller
    {
        #region Constructor
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IControlPanelServices _controlPanelServices;

        public WalletControlPanelController(
            UserManager<ApplicationUser> userManager, IControlPanelServices controlPanelServices)
        {
            _userManager = userManager;
            _controlPanelServices = controlPanelServices;
        }

        #endregion

        #region Method
        [HttpGet]
        [AllowAnonymous]//ntrivedi 17-07-2019 as front team use before login
        public async Task<IActionResult> ListChargesTypeWise(string WalletTypeName, long? TrnTypeId)
        {
            try
            {
                ListChargesTypeWise Response = new ListChargesTypeWise();
                Response = _controlPanelServices.ListChargesTypeWise(WalletTypeName, TrnTypeId);
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