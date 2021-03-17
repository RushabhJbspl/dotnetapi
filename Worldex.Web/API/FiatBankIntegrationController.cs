using Worldex.Core.ViewModels.Fiat_Bank_Integration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.FiatBankIntegration;
using Worldex.Infrastructure.Interfaces;

namespace Worldex.Web.API
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class FiatBankIntegrationController : Controller
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly IFiatIntegration _FiatIntegration;
        public FiatBankIntegrationController(UserManager<ApplicationUser> UserManager, IFiatIntegration FiatIntegration)
        {
            _UserManager = UserManager;
            _FiatIntegration = FiatIntegration;
        }

        [HttpPost]
        public async Task<IActionResult> AddBankDetail([FromBody][Required] AddBankDetailReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _FiatIntegration.AddUserBankDetail(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListUserBankRequest(short? Status)
        {
            ListUserBankReq Response = new ListUserBankReq();
            try
            {
                ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _FiatIntegration.ListUserBankDetail(Status,0,user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserbankDetails()
        {
            GetBankDetail Response = new GetBankDetail();
            try
            {
                ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _FiatIntegration.GetUserbankDetails(user.Id);
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