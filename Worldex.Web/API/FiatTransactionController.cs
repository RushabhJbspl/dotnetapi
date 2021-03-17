using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;

namespace Worldex.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiatTransactionController : ControllerBase
    {

        #region COTR
        private readonly IFiatIntegrateService _fiatIntegrateService;
        private readonly UserManager<ApplicationUser> _userManager;

        public FiatTransactionController(UserManager<ApplicationUser> userManager, IFiatIntegrateService fiatIntegrateService)
        {
            _userManager = userManager;
            _fiatIntegrateService = fiatIntegrateService;
        }
        #endregion

        [HttpPost("UpdateTransactionHash/{Guid}/{TransactionHash}")]
        [Authorize]
        public async Task<IActionResult> UpdateTransactionHash(string Guid,string TransactionHash)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
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
                    Response =_fiatIntegrateService.UpdateTransactionHash(Guid, TransactionHash, user.Id);
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