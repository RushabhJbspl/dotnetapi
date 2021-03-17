using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces.Complaint;
using CleanArchitecture.Core.Interfaces.User;
using CleanArchitecture.Core.ViewModels.AccountViewModels.Login;
using CleanArchitecture.Core.ViewModels.BackOfficeComplain;
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
    public class BackOfficeComplainController : BaseController
    {
        #region Field

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IComplainmaster _Icomplainmaster;
        private readonly ICompainTrail _IcompainTrail;
        private readonly IUserService _IuserService;
        #endregion

        #region Ctore

        public BackOfficeComplainController(UserManager<ApplicationUser> userManager, IComplainmaster Icomplainmaster, ICompainTrail IcompainTrail, IUserService IuserService)
        {
            _userManager = userManager;
            _Icomplainmaster = Icomplainmaster;
            _IcompainTrail = IcompainTrail;
            _IuserService = IuserService;
        }

        #endregion

        #region Method
        [HttpGet("GetAllComplainReport/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetAllComplainReport(int PageIndex, int Page_Size, long ComplainId, string EmailId, string MobileNo, long Status, long TypeId, int PriorityId, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;
                var GetAllComplainData = _Icomplainmaster.GetBackofficeAllData(user.Id, PageIndex, Page_Size, ComplainId, EmailId, MobileNo, Status, TypeId, PriorityId,FromDate,ToDate);
                //int TotalRowCount = 0;
                if (GetAllComplainData != null)
                    return Ok(new GetBackOffComRptResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetBackOffComData, GetTotalCompList = GetAllComplainData.GetTotalCompList, TotalCount = GetAllComplainData.TotalCount });
                else
                    return Ok(new BackOffComplainRptResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.BackOffComDataNotAvailable, ErrorCode = enErrorCode.Status9021BackOffComDataNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new BackOffComplainRptResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetComplain")]
        public async Task<IActionResult> GetComplain(long ComplainId)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;
                if (ComplainId > 0)
                {
                    var GetAllTrailComplainData = _Icomplainmaster.GetBackOfficeComplain(ComplainId);
                    if (GetAllTrailComplainData != null)
                        return Ok(new GetComplainTrailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetBackoffCom, CompainAllData = GetAllTrailComplainData });
                    else
                        return Ok(new BackOffComplainRptResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.BackOffComIdDataNotAvailable, ErrorCode = enErrorCode.Status9022BackOffComIdDataNotAvailable });
                }
                else
                    return BadRequest(new BackOffComplainRptResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffComIdNotAvailable, ErrorCode = enErrorCode.Status9027BackOffComIdNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new BackOffComplainRptResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("AddRiseComplain")]
        public async Task<IActionResult> AddRiseComplain([FromBody]BackOffAddCom model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;
                if (model.ComplainId > 0)
                {
                    long Id = _IcompainTrail.AddBackOffComMaster(model, user.Id);
                    if (Id > 0)
                        return Ok(new BackOffAddComResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessBackOffComTrail });
                    else
                        return BadRequest(new BackOffAddComResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SuccessBackOffComTrailError, ErrorCode = enErrorCode.Status9023SuccessBackOffComTrailError });
                }
                else
                    return BadRequest(new BackOffAddComResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackOffComIdNotAvailable, ErrorCode = enErrorCode.Status9027BackOffComIdNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new BackOffAddComResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetTotalNoCount")]
        public async Task<IActionResult> GetTotalNoCount(long Type = 0, short ComplainStatus = 0, int UserId = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var UserIid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserIid] = user.Id;
                var GetTotalCount = _Icomplainmaster.GetTotalComplainCount(Type, ComplainStatus, UserId);
                if (GetTotalCount != null)
                    return Ok(new GetTotalCountResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetAllComCount, TotalCountDetails = GetTotalCount });
                else
                    return Ok(new GetTotalCountResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ComplainCountNotAvailable, ErrorCode = enErrorCode.Status9026ComplainCountNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new BackOffAddComResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetAllUserData")]
        public async Task<IActionResult> GetAllUserData()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var UserIid = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserIid] = user.Id;
                var UserDataList = _IuserService.GetAllUserData();
                return Ok(new GetUserDataResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.BackOffGetAllUserData, GetUserData = UserDataList });
            }
            catch (Exception ex)
            {
                return BadRequest(new BackOffAddComResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
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