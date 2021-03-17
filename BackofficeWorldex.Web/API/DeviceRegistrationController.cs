using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.ViewModels.Wallet;
using CleanArchitecture.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CleanArchitecture.Web.Helper;
using CleanArchitecture.Infrastructure.DTOClasses;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.ViewModels;
using MediatR;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]")]
    [Authorize]
    public class DeviceRegistrationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<WalletController> _logger;
        public DeviceRegistrationController(ILogger<WalletController> logger, UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _logger = logger;
            _userManager = userManager;
            _mediator = mediator;
        }

        #region"Methods"        

        /// <summary>
        /// khushali 03-11-2018 Subscribe Device
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [HttpPost("SubscribePushNotification")]
        public async Task<IActionResult> SubscribePushNotification([FromBody]DeviceRegistrationDTO request)
        {
            ApplicationUser user =await _userManager.GetUserAsync(HttpContext.User);
            DeviceRegistrationRequest Request = new DeviceRegistrationRequest();
            CommunicationResponse Response = new CommunicationResponse();
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
                    Request.DeviceID = request.DeviceID;
                    Request.UserID = user.Id;
                    Request.SubsscrptionType = EnDeviceSubsscrptionType.Subsscribe;
                    Response = await _mediator.Send(Request);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.ReturnCode = enResponseCode.InternalError;
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        /// <summary>
        /// khushali 03-11-2018 Unsubscribe Device
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>

        [HttpPost("UnsubscribePushNotification")]
        public async Task<IActionResult> UnsubscribePushNotification([FromBody]DeviceRegistrationDTO request)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            DeviceRegistrationRequest Request = new DeviceRegistrationRequest();
            CommunicationResponse Response = new CommunicationResponse();
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
                    Request.DeviceID = request.DeviceID;
                    Request.UserID = user.Id;
                    Request.SubsscrptionType = EnDeviceSubsscrptionType.UnSubsscribe;
                    Response = await _mediator.Send(Request);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.ReturnCode = enResponseCode.InternalError;
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion
    }
}
