using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.Complaint;
using Worldex.Core.ViewModels.Complaint;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackofficeWorldex.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ComplaintController : ControllerBase
    {
        private readonly ITypemaster _typemaster;
        private readonly IComplainmaster _Icomplainmaster;
        private readonly ICompainTrail _IcompainTrail;
        private readonly UserManager<ApplicationUser> _userManager;
        public ComplaintController(ITypemaster typemaster, IComplainmaster complainmaster, ICompainTrail compainTrail,
                 UserManager<ApplicationUser> userManager)
        {
            _typemaster = typemaster;
            _Icomplainmaster = complainmaster;
            _IcompainTrail = compainTrail;
            _userManager = userManager;
        }
        [HttpGet("GetTypeMaster")]
        public async Task<IActionResult> GetTypeMaster(string Type)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items[UserId] = user.Id;

                if (!string.IsNullOrEmpty(Type))
                {
                    var DeviceMasterList = _typemaster.GettypeMaster(Type);
                    if (DeviceMasterList.Count > 0)
                        return Ok(new TypeMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.Gettypemaster, TypeMasterList = DeviceMasterList });
                    else

                        return BadRequest(new TypeMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.complaintTypeNotavailable, ErrorCode = enErrorCode.status4121complaintTypeNotavailable });
                }
                else
                {
                    return BadRequest(new TypeMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Typemasterrequired, ErrorCode = enErrorCode.Typemasterrequired4109 });
                }


            }
            catch (Exception ex)
            {
                return BadRequest(new TypeMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //[AllowAnonymous]
        //[HttpGet("GetComplainStatusType")]
        //public async Task<IActionResult> GetComplainStatusType()
        //{
        //    try
        //    {
        //        var user = await GetCurrentUserAsync();
        //        var UserId = await HttpContext.GetTokenAsync("access_token");
        //        HttpContext.Items[UserId] = user.Id;

        //        var Status = new List<ComplainStatusTypeModel>();
        //        var modelOpen = new ComplainStatusTypeModel();
        //        modelOpen.StatusId = Convert.ToInt32(enComplainStatusType.Open);
        //        modelOpen.ComplainStatus = enComplainStatusType.Open.ToString();
        //        Status.Add(modelOpen);
        //        var modelClose = new ComplainStatusTypeModel();
        //        modelClose.StatusId = Convert.ToInt32(enComplainStatusType.Close);
        //        modelClose.ComplainStatus = enComplainStatusType.Close.ToString();
        //        Status.Add(modelClose);

        //        return Ok(new ComplainStatusTypeResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetComplainStatus, ComplainStatus = Status });


        //        //if (!string.IsNullOrEmpty(Type))
        //        //{
        //        //    var DeviceMasterList = _typemaster.GettypeMaster(Type);
        //        //    if (DeviceMasterList.Count > 0)
        //        //        return Ok(new TypeMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetDeviceData, TypeMasterList = DeviceMasterList });
        //        //    else

        //        //        return BadRequest(new TypeMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.complaintTypeNotavailable, ErrorCode = enErrorCode.status4121complaintTypeNotavailable });
        //        //}
        //        //else
        //        //{
        //        //    return BadRequest(new TypeMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Typemasterrequired, ErrorCode = enErrorCode.Typemasterrequired4109 });
        //        //}


        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ComplainStatusTypeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }

        //}

        //[HttpPost("Raisecomplaint")]
        //public async Task<IActionResult> Raisecomplaint(ComplainmasterViewModel model)
        //{
        //    try
        //    {
        //        if (model != null)
        //        {
        //            var user = await GetCurrentUserAsync();
        //            var UserId = await HttpContext.GetTokenAsync("access_token");
        //            HttpContext.Items[UserId] = user.Id;
        //            var ComplainmasterReqViewModel = new ComplainmasterReqViewModel()
        //            {
        //                Description = model.Description,
        //                Subject = model.Subject,
        //                TypeId = model.TypeId,
        //                UserID = user.Id,
        //                ComplaintPriorityId=model.ComplaintPriorityId

        //            };


        //            long id = _Icomplainmaster.AddComplainmaster(ComplainmasterReqViewModel);

        //            if (id > 0)
        //            {
        //                var compainTrail = new CompainTrailReqVirewModel()
        //                {
        //                    ComplainId = id,
        //                    Description = model.Description,
        //                    UserID = user.Id
        //                };
        //                long idcompainTrail = _IcompainTrail.AddCompainTrail(compainTrail);

        //                return Ok(new ComplainmasterResonse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessAddComplain });
        //            }
        //            else
        //            {
        //                return BadRequest(new ComplainmasterResonse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.TypemasterInsertError, ErrorCode = enErrorCode.Status4110TypemasterInsertError });
        //            }

        //        }
        //        return BadRequest(new ComplainmasterResonse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AddCompainrequired, ErrorCode = enErrorCode.Status4111AddCompainrequired });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ComplainmasterResonse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}


        //[HttpPost("AddCompainTrail")]
        //public async Task<IActionResult> AddCompainTrail(CompainTrailVirewModel model)
        //{
        //    try
        //    {
        //        if (model != null)
        //        {
        //            var user = await GetCurrentUserAsync();

        //            var UserId = await HttpContext.GetTokenAsync("access_token");
        //            HttpContext.Items[UserId] = user.Id;

        //            var compainTrail = new CompainTrailReqVirewModel()
        //            {
        //                ComplainId = model.ComplainId,
        //                Description = model.Description,
        //                UserID = user.Id,
        //                Complainstatus = model.Complainstatus,
        //                Remark = model.Remark
        //            };

        //            long id = _IcompainTrail.AddCompainTrail(compainTrail);
        //            if (id > 0)
        //            {
        //                return Ok(new CompainTrailResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessCompainTrail });
        //            }
        //            else
        //            {
        //                return BadRequest(new CompainTrailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CompainTrailInsertError, ErrorCode = enErrorCode.Status4114CompainTrailInsertError });
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest(new CompainTrailResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AddCompainTrail, ErrorCode = enErrorCode.Status4113AddCompainTrail });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new CompainTrailResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpGet("GetComplain")]
        //public async Task<IActionResult> GetComplain(int ComplainId)
        //{
        //    try
        //    {
        //        var user = await GetCurrentUserAsync();

        //        var UserId = await HttpContext.GetTokenAsync("access_token");
        //        HttpContext.Items[UserId] = user.Id;

        //        var response = _Icomplainmaster.GetComplain(ComplainId);
        //        return Ok(new ComplainmasterDataResonse { ReturnCode = enResponseCode.Success, ComplainViewmodel = response, ReturnMsg = EnResponseMessage.SuccessGetCompainDetail });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new CompainTrailResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpGet("GetUserWiseComplain")]
        //public async Task<IActionResult> GetUserWiseComplain(string Subject = null,DateTime ? FromDate = null, DateTime? ToDate = null)
        //{
        //    try
        //    {
        //        var user = await GetCurrentUserAsync();

        //        var UserId = await HttpContext.GetTokenAsync("access_token");
        //        HttpContext.Items[UserId] = user.Id;
        //        var response = _Icomplainmaster.GetComplainByUserWise(user.Id, Subject, FromDate, ToDate);
        //        return Ok(new ComplainmasterResonse { ReturnCode = enResponseCode.Success, userWiseCompaintDetailResponces = response, ReturnMsg = EnResponseMessage.SuccessGetCompainDetail });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ComplainmasterResonse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}