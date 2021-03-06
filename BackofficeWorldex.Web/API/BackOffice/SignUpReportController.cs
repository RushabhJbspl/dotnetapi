using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.BackOfficeReport;
using Worldex.Core.ViewModels.BackOfficeReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackofficeWorldex.Web.API.BackOffice
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpReportController : ControllerBase
    {
        private readonly IBackOfficeReport _backOfficeReport;
        public SignUpReportController(IBackOfficeReport backOfficeReport)
        {
            _backOfficeReport = backOfficeReport;
        }
        [HttpGet ("GetUserSignUPreport")]
        public IActionResult GetUserSignUPreport( int PageIndex = 0, int Page_Size = 0, string EmailAddress = null, string Username = null,
            string Mobile = null, string Filtration = null,DateTime ? FromDate=null, DateTime? ToDate=null)
        {
            try
            {
                var SignupData= _backOfficeReport.GetSignUpReport(PageIndex, Page_Size, EmailAddress, Username, Mobile, Filtration,FromDate, ToDate);
                if(SignupData.SignReportList.Count > 0)
                {
                    return Ok(new SignReportViewmodelResponse { ReturnCode = enResponseCode.Success, SignReportViewmodes = SignupData, ReturnMsg = EnResponseMessage.GetSuccessFullySignupList });
                }
                return Ok(new SignReportViewmodelResponse { ReturnCode = enResponseCode.Success, SignReportViewmodes = SignupData, ReturnMsg = EnResponseMessage.NotFound });
            }
            catch (Exception ex)
            {
                return BadRequest(new SignReportViewmodelResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
                
            }
        }

        [HttpGet("GetUserSignUpCount")]
        public IActionResult GetUserSignUpCount()
        {
            try
            {
                var SignupData = _backOfficeReport.GetUserReportCount();
                return Ok(new SignReportCountResponseViewmodel { ReturnCode = enResponseCode.Success, signReportCountViewmodels = SignupData, ReturnMsg = EnResponseMessage.GetSuccessFullyCustomerReport });
            }
            catch (Exception ex)
            {

                return BadRequest(new SignReportViewmodelResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
    }
}