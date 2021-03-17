using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Web.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.ViewModels.MasterConfiguration;
using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Core.ViewModels.Configuration;
using CleanArchitecture.Core.ViewModels.Wallet;
using CleanArchitecture.Core.ViewModels.BackOfficeReports;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Entities.Configuration;

namespace CleanArchitecture.Web.API.Configuration
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class MasterConfigurationController : Controller
    {
        #region "Ctor"
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMasterConfiguration _masterConfiguration;
        private readonly ICommunicationService _communicationService;
        private readonly ICommonRepository<CountryMaster> _commonRepoCountry;

        public MasterConfigurationController(UserManager<ApplicationUser> userManager, 
            IBasePage basePage, IMasterConfiguration masterConfiguration, 
            ICommunicationService communicationService , ICommonRepository<CountryMaster> commonRepoCountry)
        {
            _userManager = userManager;
            _masterConfiguration = masterConfiguration;
            _communicationService = communicationService;
            _commonRepoCountry = commonRepoCountry;
        }
        #endregion

        #region "CommServiceTypeMaster"

        //vsoalnki 13-11-2018
        [HttpGet]
        public async Task<IActionResult> GetAllCommServiceType()
        {
            CommServiceTypeRes Response = new CommServiceTypeRes();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.GetAllCommServiceTypeMaster();
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region "TemplateMaster"

        //vsoalnki 13-11-2018
        [HttpGet]
        public async Task<IActionResult> GetAllTemplate()
        {
            ListTemplateMasterRes Response = new ListTemplateMasterRes();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.GetAllTemplateMaster();
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListTemplateType()
        {
            //TempRes Response = new TempRes();
            TemplateRes Response = new TemplateRes();            
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.ListTemplate();
                    if (Response.Result.Count()==0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.NotFound;
                        Response.ErrorCode = enErrorCode.NotFound;
                        return Ok(Response);
                    }
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        //vsoalnki 13-11-2018
        [HttpPost]
        public async Task<IActionResult> AddTemplate([FromBody]TemplateMasterReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.AddTemplateMaster(Request, user.Id);
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsoalnki 13-11-2018
        [HttpPost]
        public async Task<IActionResult> UpdateTemplate([FromBody]TemplateMasterReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.UpdateTemplateMaster(Request, user.Id);
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsoalnki 13-11-2018
        [HttpPost("{Id}/{Status}")]
        public async Task<IActionResult> ChangeTemplateStatus(long Id, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.DisableTemplateMaster(Id, Convert.ToInt16(Status));
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsoalnki 13-11-2018
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetTemplateById(long Id)
        {
            TemplateMasterRes Response = new TemplateMasterRes();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.GetTemplateMasterById(Id);
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        // khushali 10-01-2019
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetTemplateByCategory(long Id)
        {
            TemplateCategoryMasterRes Response = new TemplateCategoryMasterRes();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.GetTemplateMasterByCategory(Id);
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        // khushali 10-01-2019
        [HttpPost("{TemplateType}/{Status}/{TemplateID}")]
        public async Task<IActionResult> UpdateTemplateCategory(long TemplateType, ServiceStatus Status,long TemplateID)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.UpdateTemplateCategory(TemplateType, Convert.ToInt16(Status),TemplateID);
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        // khushali 12-01-2019
        [HttpGet]
        public async Task<IActionResult> TemplateParameterInfo(long? Id)
        {
            ListTemplateParameterInfoRes Response = new ListTemplateParameterInfoRes();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.TemplateParameterInfo(Id);
                    if (Response.templateParameterInfoList.Count() == 0)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.NotFound;
                        Response.ErrorCode = enErrorCode.NotFound;
                        return Ok(Response);
                    }
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                    Response.ErrorCode = enErrorCode.Success;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region "MessagingQueue"

        //vsolanki 13-11-2018
        [HttpGet("{FromDate}/{ToDate}/{Page}")]
        public async Task<IActionResult> GetMessagingQueue(DateTime FromDate, DateTime ToDate, short? Status, long? MobileNo, int Page, int? PageSize)
        {
            ListMessagingQueueRes Response = new ListMessagingQueueRes();
            try
            {
                //Page = Page + 1; // khushali - 17-01-2019 removed change as per front team for pagination
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.GetMessagingQueue(FromDate, ToDate, Status, MobileNo, Page, PageSize);
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region "EmailQueue"

        //vsolanki 14-11-2018
        [HttpGet("{FromDate}/{ToDate}/{Page}")]
        public async Task<IActionResult> GetEmailQueue(DateTime FromDate, DateTime ToDate, short? Status, string Email, int Page, int? PageSize)
        {
            ListEmailQueueRes Response = new ListEmailQueueRes();
            try
            {
                //Page = Page + 1; // khushali - 17-01-2019 removed change as per front team for pagination
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.GetEmailQueue(FromDate, ToDate, Status, Email, Page, PageSize);
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region "WalletLedger"

        //vsolanki 14-11-2018
        [HttpGet("{FromDate}/{ToDate}/{WalletId}/{Page}/{PageSize}")]
        public async Task<IActionResult> GetWalletLedger(DateTime FromDate, DateTime ToDate, string WalletId, int Page, int PageSize)
        {
            ListWalletLedgerResponse Response = new ListWalletLedgerResponse();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.GetWalletLedger(FromDate, ToDate, WalletId, Page, PageSize);
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        #endregion

        //#region "LimitRuleMaster"

        ////vsoalnki 14-11-2018
        //[HttpGet]
        //public async Task<IActionResult> GetAllLimitRule()
        //{
        //    ListLimitRuleMasterRes Response = new ListLimitRuleMasterRes();
        //    try
        //    {
        //        ApplicationUser user = new ApplicationUser(); user.Id = 1;
        //        //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _communicationService.GetAllLimitRule();
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        ////vsoalnki 14-11-2018
        //[HttpPost]
        //public async Task<IActionResult> AddLimitRule([FromBody]LimitRuleMasterReq Request)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        ApplicationUser user = new ApplicationUser(); user.Id = 1;
        //        //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _communicationService.AddLimitRule(Request, user.Id);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        ////vsoalnki 14-11-2018
        //[HttpPost("{LimitRuleMasterId}")]
        //public async Task<IActionResult> UpdateLimitRule([FromBody]LimitRuleMasterReq Request, long LimitRuleMasterId)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        ApplicationUser user = new ApplicationUser(); user.Id = 1;
        //        //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _communicationService.UpdateLimitRule(LimitRuleMasterId, Request, user.Id);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        ////vsoalnki 14-11-2018
        //[HttpPost("{LimitRuleMasterId}")]
        //public async Task<IActionResult> DisableLimitRule(long LimitRuleMasterId)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        ApplicationUser user = new ApplicationUser(); user.Id = 1;
        //        //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _communicationService.DisableLimitRule(LimitRuleMasterId);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        ////vsoalnki 14-11-2018
        //[HttpGet("{LimitRuleMasterId}")]
        //public async Task<IActionResult> GetLimitRuleById(long LimitRuleMasterId)
        //{
        //    ListLimitRuleMasterRes Response = new ListLimitRuleMasterRes();
        //    try
        //    {
        //        ApplicationUser user = new ApplicationUser(); user.Id = 1;
        //        //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _communicationService.GetLimitRuleById(LimitRuleMasterId);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //#endregion

        //#region "ChargeRuleMaster"

        ////vsoalnki 14-11-2018
        //[HttpGet]
        //public async Task<IActionResult> GetAllChargeRule()
        //{
        //    ListChargeRuleMasterRes Response = new ListChargeRuleMasterRes();
        //    try
        //    {
        //        ApplicationUser user = new ApplicationUser(); user.Id = 1;
        //        //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _communicationService.GetAllChargeRule();
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        ////vsoalnki 14-11-2018
        //[HttpPost]
        //public async Task<IActionResult> AddChargeRule([FromBody]ChargeRuleMasterReq Request)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        ApplicationUser user = new ApplicationUser(); user.Id = 1;
        //        //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _communicationService.AddChargeRule(Request, user.Id);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        ////vsoalnki 14-11-2018
        //[HttpPost("{ChargeRuleMasterId}")]
        //public async Task<IActionResult> UpdateChargeRule([FromBody]ChargeRuleMasterReq Request, long ChargeRuleMasterId)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        ApplicationUser user = new ApplicationUser(); user.Id = 1;
        //        //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _communicationService.UpdateChargeRule(ChargeRuleMasterId, Request, user.Id);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        ////vsoalnki 14-11-2018
        //[HttpPost("{ChargeRuleMasterId}")]
        //public async Task<IActionResult> DisableChargeRule(long ChargeRuleMasterId)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        ApplicationUser user = new ApplicationUser(); user.Id = 1;
        //        //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _communicationService.DisableChargeRule(ChargeRuleMasterId);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        ////vsoalnki 14-11-2018
        //[HttpGet("{ChargeRuleMasterId}")]
        //public async Task<IActionResult> GetChargeRuleById(long ChargeRuleMasterId)
        //{
        //    ListChargeRuleMasterRes Response = new ListChargeRuleMasterRes();
        //    try
        //    {
        //        ApplicationUser user = new ApplicationUser(); user.Id = 1;
        //        //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _communicationService.GetChargeRuleById(ChargeRuleMasterId);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //#endregion

        #region "NotificationQueue"

        //vsolanki 14-11-2018
        [HttpGet("{FromDate}/{ToDate}/{Page}")]
        public async Task<IActionResult> GetNotificationQueue(DateTime FromDate, DateTime ToDate, short? Status, int Page, int? PageSize)
        {
            ListNotificationQueueRes Response = new ListNotificationQueueRes();
            try
            {
                //Page = Page + 1; // khushali - 17-01-2019 removed change as per front team for pagination
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    Response = _communicationService.GetNotificationQueue(FromDate, ToDate, Status, Page, PageSize);
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region MasterConfiguration

        //#region Insert Method
        //[HttpPost]
        //public async Task<IActionResult> AddCountry(AddCountryReq Request)//[Required]string CountryName, [Required]string CountryCode
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _masterConfiguration.AddCountry(Request,user.Id);//Request.CountryName,Request.CountryCode,user.Id,Request.Status
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddState(AddStateReq Request)//string StateName, string StateCode, long CountryID
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _masterConfiguration.AddState(Request, user.Id);//Request.StateName, Request.StateCode, Request.CountryID
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddCity(AddCityReq Request)//string CityName, long StateID
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _masterConfiguration.AddCity(Request, user.Id);//user.Id--71   Request.CityName, Request.StateID
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddZipCode(AddZipCodeReq Request)//long ZipCode, string AreaName, long CityID
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _masterConfiguration.AddZipCode(Request, user.Id);//Request.ZipCode, Request.AreaName, Request.CityID
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //#endregion

        //#region Update Methods

        //[HttpPost]
        //public async Task<IActionResult> UpdateCountryDetail(AddCountryReq Request)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else if(Request.CountryID == null || Request.CountryID == 0)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.MasterConfig;
        //            Response.ErrorCode = enErrorCode.InvalidMasterID;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _masterConfiguration.UpdateCountry(Request, user.Id);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> UpdateStateDetail(AddStateReq Request)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else if (Request.StateID == null || Request.StateID == 0)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.MasterConfig;
        //            Response.ErrorCode = enErrorCode.InvalidMasterID;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _masterConfiguration.UpdateState(Request, user.Id);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> UpdateCityDetail(AddCityReq Request)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else if (Request.CityID == null || Request.CityID == 0)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.MasterConfig;
        //            Response.ErrorCode = enErrorCode.InvalidMasterID;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _masterConfiguration.UpdateCity(Request, user.Id);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> UpdateZipCodeDetail(AddZipCodeReq Request)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else if (Request.ZipCodeID == null || Request.ZipCodeID == 0)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.MasterConfig;
        //            Response.ErrorCode = enErrorCode.InvalidMasterID;
        //        }
        //        else
        //        {
        //            var accessToken = await HttpContext.GetTokenAsync("access_token");
        //            Response = _masterConfiguration.UpdateZipCode(Request, user.Id);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //#endregion

        //#region Get Methods

        //[HttpGet]
        //public async Task<IActionResult> GetCountryDetail([Required]long CountryID)
        //{
        //    Countries Response = new Countries();
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
        //            Response.BizResponseObj.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.BizResponseObj.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            Response = _masterConfiguration.GetCountry(CountryID);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetStateDetail([Required]long StateID)
        //{
        //    States Response = new States();
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
        //            Response.BizResponseObj.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.BizResponseObj.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            Response = _masterConfiguration.GetState(StateID);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetCityDetail([Required]long CityID)
        //{
        //    Cities Response = new Cities();
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.BizResponseObj.ReturnCode = enResponseCode.Fail;
        //            Response.BizResponseObj.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.BizResponseObj.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            Response = _masterConfiguration.GetCity(CityID);
        //        }
        //        var respObj = JsonConvert.SerializeObject(Response);
        //        dynamic respObjJson = JObject.Parse(respObj);
        //        return Ok(respObjJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //#endregion

        #endregion

        #region Report

        [HttpGet("{FromDate}/{ToDate}/{PageNo}/{PageSize}")]
        //[Authorize]
        public async Task<IActionResult> WithdrawalReport(DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, string CoinName, long? UserID, short? Status, string Address, string TrnID, string TrnNo, long? OrgId)
        {
            RptWithdrawalRes Response = new RptWithdrawalRes();
            BizResponseClass obj = new BizResponseClass();
            try
            {
                // var user = await _userManager.GetUserAsync(HttpContext.User);
                var user = new ApplicationUser(); user.Id = 1;
                if (user == null)
                {
                    obj.ReturnCode = enResponseCode.Fail;
                    obj.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    obj.ErrorCode = enErrorCode.StandardLoginfailed;
                    Response.BizResponseObj = obj;
                }
                else
                {
                    Response = _masterConfiguration.WithdrawReport(FromDate, ToDate, PageNo, PageSize, CoinName, UserID, Status, Address, TrnID, TrnNo, OrgId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpGet("{FromDate}/{ToDate}/{PageNo}/{PageSize}")]
        // [Authorize]
        public async Task<IActionResult> DepositionReport(DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, string CoinName, long? UserID, short? Status, string Address, string TrnID, long? OrgId)
        {
            RptDepositionRes Response = new RptDepositionRes();
            BizResponseClass obj = new BizResponseClass();
            try
            {
                var user = new ApplicationUser(); user.Id = 1;
                // var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    obj.ReturnCode = enResponseCode.Fail;
                    obj.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    obj.ErrorCode = enErrorCode.StandardLoginfailed;
                    Response.BizResponseObj = obj;
                }
                else
                {
                    Response = _masterConfiguration.DepositReport(FromDate, ToDate, PageNo, PageSize, CoinName, UserID, Status, Address, TrnID, OrgId);

                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        #endregion

        // khushali 18-01-2019
        #region "Email API manager"

        #region "Request Fromate API"

        [HttpGet("GetAllRequestFormat")]
        public IActionResult GetAllRequestFormat()
        {
            RequestFormatResAllData res = new RequestFormatResAllData();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    res.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    res.Result = _communicationService.GetAllRequestFormat();
                    if (res.Result.Count == 0)
                    {
                        res.ReturnCode = enResponseCode.Fail;
                        res.ErrorCode = enErrorCode.NoDataFound;
                        return Ok(res);
                    }
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                    return Ok(res);
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpGet("GetRequestFormatById/{Id:long}")]
        public IActionResult GetRequestFormatById(long Id)
        {
            RequestFormatResponse res = new RequestFormatResponse();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    res.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    res.Result = _communicationService.GetRequestFormatById(Id);
                    if (res.Result == null)
                    {
                        res.ReturnCode = enResponseCode.Fail;
                        res.ErrorCode = enErrorCode.NoDataFound;
                        return Ok(res);
                    }
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                    return Ok(res);
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("AddRequestFormat")]
        public async Task<IActionResult> AddRequestFormat([FromBody]RequestFormatRequest Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var response = _communicationService.AddRequestFormat(Request, user.Id);
                    if (response != 0)
                    {
                        Response.ReturnMsg = EnResponseMessage.CommRecordInsertSuccess;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InternalError;
                    }
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("UpdateRequestFormat")]
        public async Task<IActionResult> UpdateRequestFormat([FromBody]RequestFormatRequest request)
        {
            BizResponseClass res = new BizResponseClass();
            bool state = false;
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    res.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    if (request.RequestID == 0)
                    {
                        res.ReturnCode = enResponseCode.Fail;
                        res.ErrorCode = enErrorCode.InValid_ID;
                        return Ok(res);
                    }

                    state = _communicationService.UpdateRequestFormat(request, user.Id);
                    if (state == false)
                    {
                        res.ReturnCode = enResponseCode.Fail;
                        res.ErrorCode = enErrorCode.NoDataFound;
                        return Ok(res);
                    }
                    //res.response = _transactionConfigService.GetThirdPartyAPIConfigById(request.Id);
                    //if (res.response == null)
                    //{
                    //    res.ReturnCode = enResponseCode.Fail;
                    //    res.ErrorCode = enErrorCode.NoDataFound;
                    //    return Ok(res);
                    //}
                    res.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("ChangeStatusRequestFormat")]
        public IActionResult ChangeStatusRequestFormat(long id,short Status)
        {
            BizResponseClass res = new BizResponseClass();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    res.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var response = _communicationService.ChangeStatusRequestFormat(id, Status);
                    if (response == true)
                    {
                        res.ReturnCode = enResponseCode.Success;
                        res.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        res.ReturnCode = enResponseCode.Fail;
                        res.ErrorCode = enErrorCode.NoDataFound;
                    }                    
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        #endregion

        #region "Email service provider"

        [HttpGet("GetCommunicationServiceType")]
        public IActionResult GetCommunicationServiceType()
        {
            CommunicationServiceResponse res = new CommunicationServiceResponse();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    res.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    res.Result = _communicationService.GetCommunicationServiceType();
                    if (res.Result.Count == 0)
                    {
                        res.ReturnCode = enResponseCode.Fail;
                        res.ErrorCode = enErrorCode.NoDataFound;
                        return Ok(res);
                    }
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpGet("GetCommunicationServiceConfig/{ServiceType:long}")]
        public IActionResult GetCommunicationServiceConfiguration(long ServiceType)
        {
            CommunicationServiceConfigAllData res = new CommunicationServiceConfigAllData();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    res.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    res.Result = _communicationService.GetCommunicationServiceConfiguration(ServiceType);
                    if (res.Result.Count == 0)
                    {
                        res.ReturnCode = enResponseCode.Fail;
                        res.ErrorCode = enErrorCode.NoDataFound;
                        return Ok(res);
                    }
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpGet("GetCommunicationServiceConfigById/{APIID:long}")]
        public IActionResult GetCommunicationServiceConfigurationById(long APIID)
        {
            CommunicationServiceConfigRes res = new CommunicationServiceConfigRes();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    res.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    res.Result = _communicationService.GetCommunicationServiceConfigurationById(APIID);
                    if (res.Result == null)
                    {
                        res.ReturnCode = enResponseCode.Fail;
                        res.ErrorCode = enErrorCode.NoDataFound;
                        return Ok(res);
                    }
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("AddCommunicationServiceConfig")]
        public async Task<IActionResult> AddCommunicationServiceConfig([FromBody]CommunicationServiceConfigRequest Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var response = _communicationService.AddCommunicationServiceConfig(Request, user.Id);
                    if (response != 0)
                    {
                        Response.ReturnMsg = EnResponseMessage.CommRecordInsertSuccess;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.InternalError;
                    }
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("UpdateCommunicationServiceConfig")]
        public async Task<IActionResult> UpdateCommunicationServiceConfig([FromBody]CommunicationServiceConfigRequest request)
        {
            BizResponseClass res = new BizResponseClass();
            bool state = false;
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    res.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    if (request.APID == 0 || request.SerproID == 0 || request.ServiceID == 0)
                    {
                        res.ReturnCode = enResponseCode.Fail;
                        res.ErrorCode = enErrorCode.InValid_ID;
                        return Ok(res);
                    }
                    state = _communicationService.UpdateCommunicationServiceConfig(request, user.Id);
                    if (state == false)
                    {
                        res.ReturnCode = enResponseCode.Fail;
                        res.ErrorCode = enErrorCode.NoDataFound;
                        return Ok(res);
                    }
                    res.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }

        }

        [HttpPost("ChangeStatusCommunicationServiceConfig/{APIID:long}/{ServiceID:long}/{SerProID:long}/{Status:int}")]
        public IActionResult ChangeStatusCommunicationServiceConfig(long APIID, long ServiceID, long SerProID, short Status)
        {
            BizResponseClass res = new BizResponseClass();
            try
            {
                ApplicationUser user = new ApplicationUser(); //user.Id = 1;
                //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    res.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    var response = _communicationService.ChangeStatusCommunicationServiceConfig(APIID, ServiceID, SerProID, Status);
                    if (response == true)
                    {
                        res.ReturnCode = enResponseCode.Success;
                        res.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        res.ReturnCode = enResponseCode.Fail;
                        res.ErrorCode = enErrorCode.NoDataFound;
                    }
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        #endregion

        #region khushali 02-05-2019 Add entry in table From Country json 
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> AddCountry()
        {
            string ReqData = @"{""BD"": ""880"", ""BE"": ""32"", ""BF"": ""226"", ""BG"": ""359"", ""BA"": ""387"", ""BB"": ""+1-246"", ""WF"": ""681"", ""BL"": ""590"", ""BM"": ""+1-441"", ""BN"": ""673"", ""BO"": ""591"", ""BH"": ""973"", ""BI"": ""257"", ""BJ"": ""229"", ""BT"": ""975"", ""JM"": ""+1-876"", ""BV"": """", ""BW"": ""267"", ""WS"": ""685"", ""BQ"": ""599"", ""BR"": ""55"", ""BS"": ""+1-242"", ""JE"": ""+44-1534"", ""BY"": ""375"", ""BZ"": ""501"", ""RU"": ""7"", ""RW"": ""250"", ""RS"": ""381"", ""TL"": ""670"", ""RE"": ""262"", ""TM"": ""993"", ""TJ"": ""992"", ""RO"": ""40"", ""TK"": ""690"", ""GW"": ""245"", ""GU"": ""+1-671"", ""GT"": ""502"", ""GS"": """", ""GR"": ""30"", ""GQ"": ""240"", ""GP"": ""590"", ""JP"": ""81"", ""GY"": ""592"", ""GG"": ""+44-1481"", ""GF"": ""594"", ""GE"": ""995"", ""GD"": ""+1-473"", ""GB"": ""44"", ""GA"": ""241"", ""SV"": ""503"", ""GN"": ""224"", ""GM"": ""220"", ""GL"": ""299"", ""GI"": ""350"", ""GH"": ""233"", ""OM"": ""968"", ""TN"": ""216"", ""JO"": ""962"", ""HR"": ""385"", ""HT"": ""509"", ""HU"": ""36"", ""HK"": ""852"", ""HN"": ""504"", ""HM"": "" "", ""VE"": ""58"", ""PR"": ""+1-787 and 1-939"", ""PS"": ""970"", ""PW"": ""680"", ""PT"": ""351"", ""SJ"": ""47"", ""PY"": ""595"", ""IQ"": ""964"", ""PA"": ""507"", ""PF"": ""689"", ""PG"": ""675"", ""PE"": ""51"", ""PK"": ""92"", ""PH"": ""63"", ""PN"": ""870"", ""PL"": ""48"", ""PM"": ""508"", ""ZM"": ""260"", ""EH"": ""212"", ""EE"": ""372"", ""EG"": ""20"", ""ZA"": ""27"", ""EC"": ""593"", ""IT"": ""39"", ""VN"": ""84"", ""SB"": ""677"", ""ET"": ""251"", ""SO"": ""252"", ""ZW"": ""263"", ""SA"": ""966"", ""ES"": ""34"", ""ER"": ""291"", ""ME"": ""382"", ""MD"": ""373"", ""MG"": ""261"", ""MF"": ""590"", ""MA"": ""212"", ""MC"": ""377"", ""UZ"": ""998"", ""MM"": ""95"", ""ML"": ""223"", ""MO"": ""853"", ""MN"": ""976"", ""MH"": ""692"", ""MK"": ""389"", ""MU"": ""230"", ""MT"": ""356"", ""MW"": ""265"", ""MV"": ""960"", ""MQ"": ""596"", ""MP"": ""+1-670"", ""MS"": ""+1-664"", ""MR"": ""222"", ""IM"": ""+44-1624"", ""UG"": ""256"", ""TZ"": ""255"", ""MY"": ""60"", ""MX"": ""52"", ""IL"": ""972"", ""FR"": ""33"", ""IO"": ""246"", ""SH"": ""290"", ""FI"": ""358"", ""FJ"": ""679"", ""FK"": ""500"", ""FM"": ""691"", ""FO"": ""298"", ""NI"": ""505"", ""NL"": ""31"", ""NO"": ""47"", ""NA"": ""264"", ""VU"": ""678"", ""NC"": ""687"", ""NE"": ""227"", ""NF"": ""672"", ""NG"": ""234"", ""NZ"": ""64"", ""NP"": ""977"", ""NR"": ""674"", ""NU"": ""683"", ""CK"": ""682"", ""XK"": """", ""CI"": ""225"", ""CH"": ""41"", ""CO"": ""57"", ""CN"": ""86"", ""CM"": ""237"", ""CL"": ""56"", ""CC"": ""61"", ""CA"": ""1"", ""CG"": ""242"", ""CF"": ""236"", ""CD"": ""243"", ""CZ"": ""420"", ""CY"": ""357"", ""CX"": ""61"", ""CR"": ""506"", ""CW"": ""599"", ""CV"": ""238"", ""CU"": ""53"", ""SZ"": ""268"", ""SY"": ""963"", ""SX"": ""599"", ""KG"": ""996"", ""KE"": ""254"", ""SS"": ""211"", ""SR"": ""597"", ""KI"": ""686"", ""KH"": ""855"", ""KN"": ""+1-869"", ""KM"": ""269"", ""ST"": ""239"", ""SK"": ""421"", ""KR"": ""82"", ""SI"": ""386"", ""KP"": ""850"", ""KW"": ""965"", ""SN"": ""221"", ""SM"": ""378"", ""SL"": ""232"", ""SC"": ""248"", ""KZ"": ""7"", ""KY"": ""+1-345"", ""SG"": ""65"", ""SE"": ""46"", ""SD"": ""249"", ""DO"": ""+1-809 and 1-829"", ""DM"": ""+1-767"", ""DJ"": ""253"", ""DK"": ""45"", ""VG"": ""+1-284"", ""DE"": ""49"", ""YE"": ""967"", ""DZ"": ""213"", ""US"": ""1"", ""UY"": ""598"", ""YT"": ""262"", ""UM"": ""1"", ""LB"": ""961"", ""LC"": ""+1-758"", ""LA"": ""856"", ""TV"": ""688"", ""TW"": ""886"", ""TT"": ""+1-868"", ""TR"": ""90"", ""LK"": ""94"", ""LI"": ""423"", ""LV"": ""371"", ""TO"": ""676"", ""LT"": ""370"", ""LU"": ""352"", ""LR"": ""231"", ""LS"": ""266"", ""TH"": ""66"", ""TF"": """", ""TG"": ""228"", ""TD"": ""235"", ""TC"": ""+1-649"", ""LY"": ""218"", ""VA"": ""379"", ""VC"": ""+1-784"", ""AE"": ""971"", ""AD"": ""376"", ""AG"": ""+1-268"", ""AF"": ""93"", ""AI"": ""+1-264"", ""VI"": ""+1-340"", ""IS"": ""354"", ""IR"": ""98"", ""AM"": ""374"", ""AL"": ""355"", ""AO"": ""244"", ""AQ"": """", ""AS"": ""+1-684"", ""AR"": ""54"", ""AU"": ""61"", ""AT"": ""43"", ""AW"": ""297"", ""IN"": ""91"", ""AX"": ""+358-18"", ""AZ"": ""994"", ""IE"": ""353"", ""ID"": ""62"", ""UA"": ""380"", ""QA"": ""974"", ""MZ"": ""258""}";
            BizResponseClass Response = new BizResponseClass();
            Dictionary<string, string> Data = JsonConvert.DeserializeObject<Dictionary<string,string>>(ReqData);
            try
            {                
                foreach (var a in Data)
                {
                    var request = _commonRepoCountry.FindBy(e => e.CountryCode == a.Key).FirstOrDefault();
                    if(request != null)
                    {
                        var s = a.Value.Split(" ")[0].Replace("+1-", "");
                        if(string.IsNullOrEmpty(s))
                        {
                            s = "0";
                        }
                        request.CountryDialingCode = Convert.ToInt64(s);
                        await _commonRepoCountry.UpdateAsync(request);
                    }
                }
                Response.ReturnCode = enResponseCode.Success;
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion
        #endregion
    }
}