using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces.Activity_Log;
using CleanArchitecture.Core.Interfaces.Application;
using CleanArchitecture.Core.Interfaces.Organization;
using CleanArchitecture.Core.ViewModels.Organization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.API.BackOffice
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BackOfficeApplicationController : BaseController
    {
        #region Field
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBackOfficeApplication _backOfficeApplication;
        private readonly IOrganization _IOrganization;

        #endregion

        #region Ctore
        public BackOfficeApplicationController(UserManager<ApplicationUser> userManager, IBackOfficeApplication backOfficeApplication,
            IOrganization IOrganization)
        {
            _userManager = userManager;
            _backOfficeApplication = backOfficeApplication;
            _IOrganization = IOrganization;
        }
        #endregion

        #region Method

        [HttpPost("AddApplication")]
        public async Task<IActionResult> AddApplication([FromBody]ApplicationData model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;

                bool ApplicationExists = _backOfficeApplication.GetApplicationData(model.ApplicationName);
                if (ApplicationExists)
                    return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ApplicationAlreadyExist, ErrorCode = enErrorCode.Status9052ApplicationAlreadyExist });

                ApplicationViewModel imodel = new ApplicationViewModel();
                imodel.ApplicationName = model.ApplicationName;
                imodel.Description = model.Description;
                imodel.CreatedBy = user.Id;

                Guid id = _backOfficeApplication.AddApplication(imodel);

                if (id != Guid.Empty)
                    return Ok(new ApplicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessAddApplicationData });
                else
                    return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ApplicationInsertError, ErrorCode = enErrorCode.Status9053ApplicationInsertError });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("ActiveApplication")]
        public async Task<IActionResult> ActiveApplication([FromBody]ApplicationEnableDisable model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;

                ApplicationEnableDisable imodel = new ApplicationEnableDisable();
                imodel.Id = model.Id;

                bool ActiveApplication = _backOfficeApplication.EnableApplication(imodel);
                if (ActiveApplication)
                    return Ok(new ApplicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessEnableApplication });
                else
                    return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ApplicationUpdateError, ErrorCode = enErrorCode.Status9054ApplicationUpdateError });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpPost("InActiveApplication")]
        public async Task<IActionResult> InActiveApplication([FromBody]ApplicationEnableDisable model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;

                ApplicationEnableDisable imodel = new ApplicationEnableDisable();
                imodel.Id = model.Id;

                bool InActiveDomain = _backOfficeApplication.DisableApplication(imodel);
                if (InActiveDomain)
                    return Ok(new ApplicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDisableApplication });
                else
                    return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ApplicationUpdateError, ErrorCode = enErrorCode.Status9054ApplicationUpdateError });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpGet("GetAllCountApplication")]
        public async Task<IActionResult> GetAllCountApplication()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;

                ApplicationTotalCount model = new ApplicationTotalCount();
                model = _backOfficeApplication.GetTotalApplicationCount();
                if (model != null)
                    return Ok(new ApplicationTotalResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetAllCountApp, TotalCountApplication = model });
                else
                    return Ok(new ApplicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.AppCountNotAvailable, ErrorCode = enErrorCode.Status9055AppCountNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetApplicationList/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetApplicationList(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;

                var ApplicationList = _backOfficeApplication.GetTotalApplicationList(PageIndex, Page_Size);
                if (ApplicationList != null)
                    return Ok(new GetTotalApplicationListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetAppData, GetTotalApplicationList = ApplicationList.GetTotalApplicationList, TotalCount = ApplicationList.TotalCount });
                else
                    return Ok(new ApplicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.AppNotAvailable, ErrorCode = enErrorCode.Status9056AppNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetActiveApplicationList/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetActiveApplicationList(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;

                var ApplicationList = _backOfficeApplication.GetTotalActiveApplicationListV1(PageIndex, Page_Size);
                if (ApplicationList != null)
                    return Ok(new GetTotalActiveApplicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetActiveAppData, GetTotalActiveApplicationList = ApplicationList.GetTotalActiveApplicationList, TotalCount = ApplicationList.TotalCount });
                else
                    return Ok(new ApplicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ActiveAppNotAvailable, ErrorCode = enErrorCode.Status9057ActiveAppNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetDisActiveApplicationList/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetDisActiveApplicationList(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;

                var ApplicationList = _backOfficeApplication.GetTotalDisActiveApplicationList(PageIndex, Page_Size);
                if (ApplicationList != null)
                {
                    return Ok(new GetTotalDisactiveApplicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetDisActiveAppData, GetTotalDisactiveApplicationList = ApplicationList.GetTotalDisactiveApplicationList, TotalCount = ApplicationList.TotalCount });
                }
                else
                    return Ok(new ApplicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DisActiveAppNotAvailable, ErrorCode = enErrorCode.Status9058DisActiveAppNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetAllApplicationData")]
        public async Task<IActionResult> GetAllApplicationData()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;
                var ApplicationDataList = _backOfficeApplication.GetAllApplicationData();
                return Ok(new GetApplicationDataResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.BackOffGetAllApplicationData, GetApplicationData = ApplicationDataList });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetUserWiseDomainData")]
        public async Task<IActionResult> GetUserWiseDomainData()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;
                var UserWiseDomainData = _IOrganization.GetUserWiseDomainData(user.Id);
                return Ok(new GetUserWiseDomainResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.BackOffGetUserWiseDomainData, GetUserWiseDomainData = UserWiseDomainData });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("UserWiseCreateApplication")]
        public async Task<IActionResult> UserWiseCreateApplication([FromBody]CreateUserApplication model)
        {
            try
            {
                //if (model.DomainId == Guid.Empty)
                //    return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffReqDomainId, ErrorCode = enErrorCode.Status9059BackOffReqDomainId });

                //if (model.AppId == Guid.Empty)
                //    return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffReqAppId, ErrorCode = enErrorCode.Status9060BackOffReqAppId });

                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;

                string Domain = model.DomainId;
                Guid DomainId = Guid.Empty;
                try
                {
                    DomainId = new Guid(Domain);
                }
                catch (Exception)
                {
                    return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffReqValidDomainId, ErrorCode = enErrorCode.Status9073BackOffReqValidDomainId });
                }


                string App = model.AppId;
                Guid AppId = Guid.Empty;
                try
                {
                    AppId = new Guid(App);
                }
                catch (Exception)
                {
                    return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffReqValidAppId, ErrorCode = enErrorCode.Status9074BackOffReqValidAppId });
                }


                bool IsValidDomain = _IOrganization.CheckValidDomain(DomainId, user.Id);
                bool IsValidApp = _backOfficeApplication.CheckValidMasterApplication(AppId);
                if (IsValidDomain && IsValidApp)
                {

                    bool ApplicationExists = _backOfficeApplication.CheckUserWiseAppData(DomainId, AppId, model.AppName, user.Id);
                    if (!ApplicationExists)
                        return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserWiseApplicationAlreadyExist, ErrorCode = enErrorCode.Status9067UserWiseApplicationAlreadyExist });

                    CreateUserWiseApplication userapp = new CreateUserWiseApplication();
                    userapp.AppId = AppId;
                    userapp.AppName = model.AppName;
                    userapp.DomainId = DomainId;
                    userapp.UserId = user.Id;
                    Guid Id = _backOfficeApplication.UserWiseAddApplication(userapp);
                    if (Id != Guid.Empty)
                        return Ok(new OrgAppMappingResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessAddUserApplicationData });
                    else
                        return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserWiseApplicationInsertError, ErrorCode = enErrorCode.Status9062UserWiseApplicationInsertError });
                }
                else
                    return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffInvalidDomainorApp, ErrorCode = enErrorCode.Status9061BackOffInvalidDomainorApp });
            }
            catch (Exception ex)
            {
                return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetUserWiseApplicationData")]
        public async Task<IActionResult> GetUserWiseApplicationData(Guid Id)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;
                if (Id != Guid.Empty)
                {
                    var UserData = _backOfficeApplication.GetUserWiseAppdataDet(Id);
                    if (UserData != null)
                        return Ok(new GetUserWiseAppDataResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetUserWiseApplicationData, UserWiseAppData = UserData });
                    else
                        return Ok(new OrgAppMappingResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.BackOffAppIdDataNotAvailable, ErrorCode = enErrorCode.Status9064BackOffAppIdDataNotAvailable });
                }
                else
                    return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffAppIdNotAvailable, ErrorCode = enErrorCode.Status9063BackOffAppIdNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("UpdateUserWiseApplicationData")]
        public async Task<IActionResult> UpdateUserWiseApplicationData([FromBody] UpdateUserWiseAppData model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;
                //if (model.DomainId == Guid.Empty)
                //    return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffReqDomainId, ErrorCode = enErrorCode.Status9059BackOffReqDomainId });

                //if (model.AppId == Guid.Empty)
                //    return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffReqAppId, ErrorCode = enErrorCode.Status9060BackOffReqAppId });
                string Domain = model.DomainId;
                Guid DomainId = Guid.Empty;
                try
                {
                    DomainId = new Guid(Domain);
                }
                catch (Exception)
                {
                    return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffReqValidDomainId, ErrorCode = enErrorCode.Status9073BackOffReqValidDomainId });
                }

                string App = model.AppId;
                Guid AppId = Guid.Empty;
                try
                {
                    AppId = new Guid(App);
                }
                catch (Exception)
                {
                    return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffReqValidAppId, ErrorCode = enErrorCode.Status9074BackOffReqValidAppId });
                }

                UpdateUserGUIDWiseAppData updatemodel = new UpdateUserGUIDWiseAppData();
                updatemodel.Id = model.Id;
                updatemodel.AppName = model.AppName;
                updatemodel.DomainId = DomainId;
                updatemodel.ClientSecret = model.ClientSecret;
                updatemodel.Description = model.Description;
                updatemodel.ApplicationLogo = model.ApplicationLogo;
                updatemodel.AppId = AppId;
                updatemodel.AllowedCallBackURLS = model.AllowedCallBackURLS;
                updatemodel.AllowedWebOrigins = model.AllowedWebOrigins;
                updatemodel.AllowedLogoutURLS = model.AllowedLogoutURLS;
                updatemodel.AllowedOriginsCORS = model.AllowedOriginsCORS;
                updatemodel.JWTExpiration = model.JWTExpiration;

                //if (model.Id != Guid.Empty)
                //{
                Guid Id = _backOfficeApplication.UpdateUserWiseApplication(updatemodel, user.Id);
                if (Id != Guid.Empty)
                    return Ok(new OrgAppMappingResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessUpdateUserApplicationData });
                else
                    return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffAppUpdateError, ErrorCode = enErrorCode.Status9065BackOffAppUpdateError });
                //}
                //else
                //    return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffAppIdNotAvailable, ErrorCode = enErrorCode.Status9063BackOffAppIdNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new OrgAppMappingResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetUserWiseApplicationList/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetUserWiseApplicationList(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;

                var UserApplicationList = _backOfficeApplication.GetUserApplicationList(PageIndex, Page_Size, user.Id);
                if (UserApplicationList != null)
                    return Ok(new GetTotalUserApplicationListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.UserWiseSuccessGetAppData, TotalUserApplicationList = UserApplicationList.TotalUserApplicationList, TotalCount = UserApplicationList.TotalCount });
                else
                    return Ok(new ApplicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.UserWiseAppNotAvailable, ErrorCode = enErrorCode.Status9066UserWiseAppNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Helpers
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion
    }
}