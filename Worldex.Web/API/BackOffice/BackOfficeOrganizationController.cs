using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces.Organization;
using CleanArchitecture.Core.ViewModels.AccountViewModels.SignUp;
using CleanArchitecture.Core.ViewModels.ManageViewModels;
using CleanArchitecture.Core.ViewModels.Organization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BackOfficeOrganizationController : BaseController
    {
        #region Field

        //IOrganization
        private readonly IOrganization _IOrganization;
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion

        #region Ctore
        public BackOfficeOrganizationController(IOrganization IOrganization, UserManager<ApplicationUser> userManager)
        {
            _IOrganization = IOrganization;
            _userManager = userManager;
            //_IprofileMaster = IprofileMaster;
            //_userManager = userManager;
            //_IsubscriptionMaster = IsubscriptionMaster;
        }
        #endregion

        #region Methods

        [HttpGet("OrganizationInfo")]
        public async Task<IActionResult> OrganizationInfo()
        {
            try
            {
                var user = await GetCurrentUserAsync();

                var UserIid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserIid] = user.Id;

                var UserData = new OrganizationUserDataViewModel
                {
                    //FirstName = user.FirstName,
                    //LastName = user.LastName,
                    //Username = user.UserName,
                    OrganizationName = string.Empty,
                    Email = user.Email,
                    //PhoneNumber = user.PhoneNumber,
                    MobileNo = user.Mobile,
                    Fax = string.Empty,
                    Website = string.Empty,
                    LanguageId = 0,
                    Phone = string.Empty,
                    Street = string.Empty,
                    City = string.Empty,
                    PinCode = string.Empty,
                    CountryId = 0,
                    StateId = 0
                    //CountryCode = user.CountryCode
                };

                return Ok(new OrganizationUserDataResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessfullGetUserData, UserData = UserData });
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("OrganizationInfo")]
        public async Task<IActionResult> OrganizationInfo(OrganizationUserDataViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var UserIid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserIid] = user.Id;
                //user.FirstName = model.FirstName;
                //user.LastName = model.LastName;
                //user.UserName = model.Username;
                user.Mobile = model.MobileNo;
                //user.CountryCode = model.CountryCode;
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return Ok(new OrganizationUserResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.OrganizationUserData });
                else
                    return BadRequest(new OrganizationUserResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.OrganizationUpdateError, ErrorCode = enErrorCode.Status9017OrganizationUpdateError });
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("AddDomain")]
        public async Task<IActionResult> AddDomain([FromBody]DomainViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();

                var UserIid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserIid] = user.Id;

                bool DomainExists = _IOrganization.GetDomainByUserwise(model.DomainName, user.Id);
                if (DomainExists)
                {
                    return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DomainAlreadyExist, ErrorCode = enErrorCode.Status9005DomainAlreadyExist });
                }

                OrganizationViewModel imodel = new OrganizationViewModel();
                imodel.UserId = user.Id;
                imodel.AliasName = model.AliasName;
                imodel.DomainName = model.DomainName;

                Guid id = _IOrganization.AddDomaim(imodel);

                if (id != Guid.Empty)
                {
                    return Ok(new OrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessAddDomainData });
                }
                else
                {
                    return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DomainInsertError, ErrorCode = enErrorCode.Status9006DomainInsertError });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetDomainList/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetDomainList(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();

                var UserIid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserIid] = user.Id;
                //var DomainList = (dynamic)null;
                var DomainList = _IOrganization.GetTotalDomainList(user.Id, user?.UserName, PageIndex, Page_Size);
                //int TotalRowCount = 0;
                if (DomainList != null)
                {
                    return Ok(new GetTotalOrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetDomainData, GetTotalDomainList = DomainList.GetTotalDomainList, TotalCount = DomainList.TotalCount });
                    //if (DomainList.Count > 0)
                    //{
                    //    TotalRowCount = DomainList.Count;

                    //}
                    //else
                    //    return Ok(new OrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DomainNotAvailable, ErrorCode = enErrorCode.Status9007DomainNotAvailable });
                }
                else
                    return Ok(new OrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DomainNotAvailable, ErrorCode = enErrorCode.Status9007DomainNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetActiveDomainList/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetActiveDomainList(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var UserIid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserIid] = user.Id;
                //var ActiveDomainList = (dynamic)null;
                var ActiveDomainList = _IOrganization.GetTotalActiveDomainList(user.Id, user?.UserName, PageIndex, Page_Size);
                //int TotalRowCount = 0;
                if (ActiveDomainList != null)
                {
                    //if (ActiveDomainList.Count > 0)
                    //{
                    //    TotalRowCount = ActiveDomainList.Count;
                    return Ok(new GetTotalActiveOrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetActiveDomainData, GetTotalActiveDomainList = ActiveDomainList.GetTotalDomainList, TotalCount = ActiveDomainList.TotalCount });
                    //}
                    //else
                    //    return Ok(new OrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ActiveDomainNotAvailable, ErrorCode = enErrorCode.Status9008ActiveDomainNotAvailable });
                }
                else
                    return Ok(new OrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ActiveDomainNotAvailable, ErrorCode = enErrorCode.Status9008ActiveDomainNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetDisActiveDomainList/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetDisActiveDomainList(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var UserIid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserIid] = user.Id;
                //var DisActiveDomainList = (dynamic)null;
                var DisActiveDomainList = _IOrganization.GetTotalDisactiveDomainList(user.Id, user?.UserName, PageIndex, Page_Size);
                //int TotalRowCount = 0;
                if (DisActiveDomainList != null)
                {
                    //if (DisActiveDomainList.Count > 0)
                    //{
                    //    TotalRowCount = DisActiveDomainList.Count;
                    return Ok(new GetTotalDisactiveOrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetDisActiveDomainData, GetTotalDisactiveDomainList = DisActiveDomainList.GetTotalDomainList, TotalCount = DisActiveDomainList.TotalCount });
                    //}
                    //else
                    //    return Ok(new OrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DisActiveDomainNotAvailable, ErrorCode = enErrorCode.Status9009DisActiveDomainNotAvailable });
                }
                else
                    return Ok(new OrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DisActiveDomainNotAvailable, ErrorCode = enErrorCode.Status9009DisActiveDomainNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ActiveDomain")]
        public async Task<IActionResult> ActiveDomain([FromBody]OrganizationEnableDisable model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var UserIid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserIid] = user.Id;

                OrganizationViewModel imodel = new OrganizationViewModel();
                imodel.UserId = user.Id;
                imodel.Id = model.Id;

                bool ActiveDomain = _IOrganization.EnableDomain(imodel);
                if (ActiveDomain)
                    return Ok(new OrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessEnableDomain });
                else
                    return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DomainUpdateError, ErrorCode = enErrorCode.Status9010DomainUpdateError });
            }
            catch (Exception ex)
            {
                return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpPost("InActiveDomain")]
        public async Task<IActionResult> InActiveDomain([FromBody]OrganizationEnableDisable model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var UserIid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserIid] = user.Id;

                OrganizationViewModel imodel = new OrganizationViewModel();
                imodel.UserId = user.Id;
                imodel.Id = model.Id;

                bool InActiveDomain = _IOrganization.DisActiveDomain(imodel);
                if (InActiveDomain)
                    return Ok(new OrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDisableDomain });
                else
                    return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DomainUpdateError, ErrorCode = enErrorCode.Status9010DomainUpdateError });
            }
            catch (Exception ex)
            {
                return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetAllCountDomain")]
        public async Task<IActionResult> GetAllCountDomain()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var UserIid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserIid] = user.Id;

                OrganizationTotalDomainCount model = new OrganizationTotalDomainCount();
                model = _IOrganization.GetTotalDomainCount(user.Id);
                if (model != null)
                    return Ok(new OrganizationTotalDomainResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetAllCount, TotalCountDomain = model });
                else
                    return Ok(new OrganizationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DomainCountNotAvailable, ErrorCode = enErrorCode.Status9011DomainCountNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new OrganizationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
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