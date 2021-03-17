using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.BackOffice;
using Worldex.Core.Interfaces.BackOffice.ComplaintSALConfiguration;
using Worldex.Core.Interfaces.BackOfficeReport;
using Worldex.Core.Interfaces.EmailMaster;
using Worldex.Core.Interfaces.PhoneMaster;
using Worldex.Core.Interfaces.User;
using Worldex.Core.ViewModels.BackOffice;
using Worldex.Core.ViewModels.BackOffice.ComplaintSALConfiguration;
using Worldex.Core.ViewModels.BackOfficeReports;
using Worldex.Core.ViewModels.EmailMaster;
using Worldex.Core.ViewModels.MobileMaster;
using Worldex.Core.ViewModels.SecurityQuestion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackofficeWorldex.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BackOfficeController : ControllerBase
    {
        private readonly IEmailMaster _IemailMaster;
        private readonly Iphonemaster _Iphonemaster;
        private readonly ISecurityQuestion _IsecurityQuestion;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userdata;
        private readonly IIPRange _IIPRange;
        private readonly IComplaintPriorityMaster _ComplaintPriorityMaster;
        private readonly IBackOfficeReport _IBackOfficeReport;


        public BackOfficeController(IEmailMaster IemailMaster, Iphonemaster iphonemaster,
            ISecurityQuestion securityQuestion, UserManager<ApplicationUser> userManager, IUserService userdata, IIPRange iIPRange,
            IComplaintPriorityMaster ComplaintPriorityMaster, IBackOfficeReport IBackOfficeReport)
        {
            _IemailMaster = IemailMaster;
            _Iphonemaster = iphonemaster;
            _IsecurityQuestion = securityQuestion;
            _userManager = userManager;
            _userdata = userdata;
            _IIPRange = iIPRange;
            _ComplaintPriorityMaster = ComplaintPriorityMaster;
            _IBackOfficeReport = IBackOfficeReport;
        }

        [HttpPost("EmailAddressAdd")]
        public async Task<IActionResult> EmailAddressAdd(EmailMasterViewModel emailMasterViewModel) //[FromHeader] string RedisDBKey)
        {
            try
            {
                Guid AllreadyEmailExist = _IemailMaster.IsEmailExist(emailMasterViewModel.Email);
                if (AllreadyEmailExist != Guid.Empty)
                {
                    return BadRequest(new EmailMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpBizUserEmailExist, ErrorCode = enErrorCode.Status4098BizUserEmailExist });
                }
                else
                {
                    var user = await GetCurrentUserAsync();
                    EmailMasterReqViewModel emailMasterReqViewModel = new EmailMasterReqViewModel();
                    {
                        emailMasterReqViewModel.Email = emailMasterViewModel.Email;
                        emailMasterReqViewModel.IsPrimary = emailMasterViewModel.IsPrimary;
                        emailMasterReqViewModel.Userid = user.Id;
                    }
                    Guid emailid = _IemailMaster.Add(emailMasterReqViewModel);
                    if (emailid != null)
                    {
                        return Ok(new EmailMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessAddEmail });
                    }
                    else
                    {
                        return BadRequest(new EmailMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AddEmailFail, ErrorCode = enErrorCode.Status8001AddEmailInsertFail });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new EmailMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }
        [HttpPost("EmailAddressUpdate")]
        public async Task<IActionResult> EmailAddressUpdate(EmailMasterUpdateViewModel emailMasterViewModel) //[FromHeader] string RedisDBKey)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                EmailMasterUpdateReqViewModel emailMasterUpdateReqViewModel = new EmailMasterUpdateReqViewModel();
                emailMasterUpdateReqViewModel.Id = emailMasterViewModel.Id;
                emailMasterUpdateReqViewModel.Userid = user.Id;
                emailMasterUpdateReqViewModel.IsPrimary = emailMasterViewModel.IsPrimary;
                emailMasterUpdateReqViewModel.Email = emailMasterViewModel.Email;

                Guid emailid = _IemailMaster.Update(emailMasterUpdateReqViewModel);
                if (emailid != Guid.Empty)
                {
                    return Ok(new EmailMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessupdateEmail });
                }
                else
                {
                    return BadRequest(new EmailMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.updateEmailFail, ErrorCode = enErrorCode.Status8003AddEmailUpdateFail });
                }

            }
            catch (Exception ex)
            {

                return BadRequest(new EmailMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("EmailAddressDelete")]
        public async Task<IActionResult> EmailAddressDelete(EmailMasterDeleteViewModel emailMasterViewModel) //[FromHeader] string RedisDBKey)
        {
            try
            {
                Guid emailid = _IemailMaster.Delete(emailMasterViewModel);
                if (emailid != Guid.Empty)
                {
                    return Ok(new EmailMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDeleteEmail });
                }
                else
                {
                    return BadRequest(new EmailMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeleteEmailFail, ErrorCode = enErrorCode.Status8008EmailDeleteFail });
                }

            }
            catch (Exception ex)
            {

                return BadRequest(new EmailMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpGet("EmailAddressList")]
        public async Task<IActionResult> EmailAddressList() //[FromHeader] string RedisDBKey)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var EmailList = _IemailMaster.GetuserWiseEmailList(user.Id);
                return Ok(new EmailListResponse { ReturnCode = enResponseCode.Success, emailListViewModels = EmailList, ReturnMsg = EnResponseMessage.Getemailaddresslist });
            }
            catch (Exception ex)
            {

                return BadRequest(new EmailMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("MobileNumberAddressAdd")]
        public async Task<IActionResult> MobileNumberAddressAdd(PhoneMasterViewModel phoneMasterViewModel) //[FromHeader] string RedisDBKey)
        {
            try
            {


                Guid AllreadyPhoneExist = _Iphonemaster.IsPhoneNumberExist(phoneMasterViewModel.MobileNumber);


                if (AllreadyPhoneExist != Guid.Empty)
                {
                    return Ok(new PhoneMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.PhoneAllreadyExist });
                }

                else
                {
                    bool isValidNumber = await _userdata.IsValidPhoneNumber(phoneMasterViewModel.MobileNumber, phoneMasterViewModel.CountryCode);
                    if (!isValidNumber)
                    {
                        return BadRequest(new PhoneMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardSignUpPhonevalid, ErrorCode = enErrorCode.Status4013MobileInvalid });
                    }

                    var user = await GetCurrentUserAsync();
                    PhoneMasterReqViewModel phoneMasterReqViewModel = new PhoneMasterReqViewModel();
                    phoneMasterReqViewModel.IsPrimary = phoneMasterViewModel.IsPrimary;
                    phoneMasterReqViewModel.MobileNumber = phoneMasterViewModel.MobileNumber;
                    phoneMasterReqViewModel.Userid = user.Id;


                    Guid phoneID = _Iphonemaster.Add(phoneMasterReqViewModel);
                    if (phoneID != null)
                    {
                        return Ok(new PhoneMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessAddmobile });
                    }
                    else
                    {
                        return BadRequest(new PhoneMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AddMobileNumberFail, ErrorCode = enErrorCode.Status8006AddPhoneUpdateFail });
                    }
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new PhoneMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("MobileNumberUpdate")]
        public async Task<IActionResult> MobileNumberUpdate(PhoneMasterUpdateViewModel phoneMasterUpdateViewModel) //[FromHeader] string RedisDBKey)
        {
            try
            {

                bool isValidNumber = await _userdata.IsValidPhoneNumber(phoneMasterUpdateViewModel.MobileNumber, phoneMasterUpdateViewModel.CountryCode);
                if (!isValidNumber)
                {
                    return BadRequest(new PhoneMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardSignUpPhonevalid, ErrorCode = enErrorCode.Status4013MobileInvalid });
                }

                var user = await GetCurrentUserAsync();
                PhoneMasterUpdateReqViewModel phoneMasterUpdateReqViewModel = new PhoneMasterUpdateReqViewModel();
                phoneMasterUpdateReqViewModel.Id = phoneMasterUpdateViewModel.Id;
                phoneMasterUpdateReqViewModel.IsPrimary = phoneMasterUpdateViewModel.IsPrimary;
                phoneMasterUpdateReqViewModel.MobileNumber = phoneMasterUpdateViewModel.MobileNumber;
                phoneMasterUpdateReqViewModel.Userid = user.Id;

                Guid PhoneId = _Iphonemaster.Update(phoneMasterUpdateReqViewModel);
                if (PhoneId != Guid.Empty)
                {
                    return Ok(new PhoneMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessupdatePhoneNumber });
                }
                else
                {
                    return BadRequest(new PhoneMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.updatePhonenumberFail, ErrorCode = enErrorCode.Status8007PnoneNumberUpdateFail });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new PhoneMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }
        [HttpGet("MobileNumberList")]
        public async Task<IActionResult> MobileNumberList() //[FromHeader] string RedisDBKey)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var PhoneNumberList = _Iphonemaster.GetuserWiseMolibenumberList(user.Id);

                return Ok(new PhoneListResponse { ReturnCode = enResponseCode.Success, PhonenumberListViewModels = PhoneNumberList, ReturnMsg = EnResponseMessage.GetMobileNumberlist });


            }
            catch (Exception ex)
            {

                return BadRequest(new PhoneMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("PhoneNumberDelete")]
        public async Task<IActionResult> PhoneNumberDelete(PhoneMasterDeleteViewModel phoneMasterDeleteViewModel) //[FromHeader] string RedisDBKey)
        {
            try
            {
                Guid emailid = _Iphonemaster.Delete(phoneMasterDeleteViewModel);
                if (emailid != Guid.Empty)
                {
                    return Ok(new PhoneMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessDeleteEmail });
                }
                else
                {
                    return BadRequest(new PhoneMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeleteEmailFail, ErrorCode = enErrorCode.Status8008EmailDeleteFail });
                }

            }
            catch (Exception ex)
            {

                return BadRequest(new PhoneMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("SecurityQuestionAdd")]
        public async Task<IActionResult> SecurityQuestionAdd(SecurityQuestionMasterViewModel securityQuestionMasterViewModel)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                SecurityQuestionMasterReqViewModel securityQuestionMasterReqViewModel = new SecurityQuestionMasterReqViewModel();
                securityQuestionMasterReqViewModel.Userid = user.Id;
                securityQuestionMasterReqViewModel.Answer = securityQuestionMasterViewModel.Answer;
                securityQuestionMasterReqViewModel.SecurityQuestion = securityQuestionMasterViewModel.SecurityQuestion;
                Guid id = _IsecurityQuestion.Add(securityQuestionMasterReqViewModel);
                if (id != Guid.Empty)
                {
                    return Ok(new SecurityQuestionMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SecurityQuestion });
                }
                else
                {
                    return BadRequest(new SecurityQuestionMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AddSecurityQuestionFail, ErrorCode = enErrorCode.Status8009SecurityQuestionFail });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new SecurityQuestionMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }
        /// <summary>
        /// Created By pankaj for add the  ip range
        ///  Created date :  03-01-2018 
        /// </summary>
        /// <param name="IPRangeViewModel"></param>
        /// <returns></returns>
        [HttpPost("AllowIpRange")]

        public async Task<IActionResult> AllowIpRange(IPRangeViewModel IPRangeViewModel)
        {
            try
            {

                var user = await GetCurrentUserAsync();

                long ipStart = BitConverter.ToInt32(IPAddress.Parse(IPRangeViewModel.StartIp).GetAddressBytes().Reverse().ToArray(), 0);

                long ipEnd = BitConverter.ToInt32(IPAddress.Parse(IPRangeViewModel.EndIp).GetAddressBytes().Reverse().ToArray(), 0);

                if (ipStart >= ipEnd)
                {
                    return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InValidIPrange, ErrorCode = enErrorCode.Status8036InvalidIPRange });
                }

                ////// Ip Address Validate or not
                var CheckStartIp = await _userdata.GetIPWiseData(IPRangeViewModel.StartIp);
                //string CountryCode = await _userService.GetCountryByIP(model.IPAddress);
                if (!string.IsNullOrEmpty(CheckStartIp.CountryCode) && CheckStartIp.CountryCode == "fail")
                {
                    return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StartIP, ErrorCode = enErrorCode.Status8034KYCrecordnotfound });
                }


                ////// Ip Address Validate or not
                var CheckEndIp = await _userdata.GetIPWiseData(IPRangeViewModel.EndIp);
                if (!string.IsNullOrEmpty(CheckEndIp.CountryCode) && CheckEndIp.CountryCode == "fail")
                {
                    return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.EndIP, ErrorCode = enErrorCode.Status8035KYCrecordnotfound });
                }
                IPRangeAddViewModel iPRangeAddViewModel = new IPRangeAddViewModel()
                {
                    UserId = user.Id,
                    StartIp = IPRangeViewModel.StartIp,
                    EndIp = IPRangeViewModel.EndIp
                };
                Guid AllreadyRangeExist = _IIPRange.ISIPRangeExist(iPRangeAddViewModel);
                if (AllreadyRangeExist != Guid.Empty)
                {
                    return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AlreadyIprangeExist, ErrorCode = enErrorCode.Status8038IPrangeAlreadyExist });
                }
                Guid IPRange = _IIPRange.AddIPRange(iPRangeAddViewModel);
                if (IPRange == Guid.Empty)
                {
                    return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IPRangeNotInsert, ErrorCode = enErrorCode.Status8037NotInsertIpRange });
                }
                else
                {
                    return Ok(new IPRangeResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.AddIPrange });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }
        /// <summary>
        /// Delete the user ip range
        /// </summary>
        /// <param name="IPRangeViewModel"></param>
        /// <returns></returns>
        [HttpPost("DeleteIpRange")]

        public async Task<IActionResult> DeleteIpRange(IPRangeDeleteViewModel IPRangeDelete)
        {
            try
            {
                var user = await GetCurrentUserAsync();

                IPRangeDeleteReqViewModel iPRangeDelete = new IPRangeDeleteReqViewModel()
                {
                    Id = IPRangeDelete.Id,
                    Userid = user.Id
                };
                Guid RangeID = _IIPRange.DeleteRange(iPRangeDelete);
                if (RangeID != Guid.Empty)
                {
                    return Ok(new IPRangeResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.IPRangeDelete });
                }
                else
                {
                    return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IPRangeNotDelete, ErrorCode = enErrorCode.Status8039IPrangeNotDeleted });
                }


            }
            catch (Exception ex)
            {

                return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpGet("GetIpRange")]
        public async Task<IActionResult> GetIpRange(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var IpRangeList = _IIPRange.GetIPRange(PageIndex, Page_Size);
                return Ok(new IPRangeGetdataResponse { ReturnCode = enResponseCode.Success, TotalCount = IpRangeList.TotalCount, IPRangeGet = IpRangeList.IPRangeGet, ReturnMsg = EnResponseMessage.IPRangeGet });
            }
            catch (Exception ex)
            {
                return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        /// <summary>
        /// Created by pankaj kathiriya for add the configuration for complaint priority set
        /// Date :07-01-2019
        /// </summary>
        /// <param name="ComplaintPriority"></param>
        /// <returns></returns>
        [HttpPost("ComplaintPriorityAdd")]
        public async Task<IActionResult> ComplaintPriorityAdd(ComplaintPriorityMasterViewModel ComplaintPriority)
        {
            try
            {

                var user = await GetCurrentUserAsync();
                ComplaintPriorityMasterreqViewModel ComplaintPriorityMaster = new ComplaintPriorityMasterreqViewModel()
                {
                    UserId = user.Id,
                    Priority = ComplaintPriority.Priority,
                    PriorityTime = ComplaintPriority.PriorityTime
                };
                long AllreadyComplaintPriorityExist = _ComplaintPriorityMaster.IsComplaintPriorityExist(ComplaintPriorityMaster);
                if (AllreadyComplaintPriorityExist > 0)
                {
                    return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ComplaintPriorityExist, ErrorCode = enErrorCode.Status8040ComplaintPriorityExist });
                }
                long ComplaintPriorityAdd = _ComplaintPriorityMaster.Add(ComplaintPriorityMaster);
                if (ComplaintPriorityAdd == 0)
                {
                    return BadRequest(new ComplaintPriorityMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ComplaintpriorityNotInsert, ErrorCode = enErrorCode.Status8041NotInsertComplaintPriority });
                }
                else
                {
                    return Ok(new ComplaintPriorityMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.AddComplaintpriority });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new ComplaintPriorityMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }
        /// <summary>
        /// Created by pankaj kathiriya for update  the configuration for complaint priority 
        /// Date :07-01-2019
        /// </summary>
        /// <param name="ComplaintPriority"></param>
        /// <returns></returns>

        [HttpPost("ComplaintPriorityUpdate")]
        public async Task<IActionResult> ComplaintPriorityUpdate(ComplaintPriorityMasterupdateViewModel ComplaintPriorityMaster)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                ComplaintPriorityMasterupdatereqViewModel PriorityMasterupdate = new ComplaintPriorityMasterupdatereqViewModel()
                {
                    Id = ComplaintPriorityMaster.Id,
                    Priority = ComplaintPriorityMaster.Priority,
                    PriorityTime = ComplaintPriorityMaster.PriorityTime,
                    UserId = user.Id
                };
                long PriorityMasterid = _ComplaintPriorityMaster.Update(PriorityMasterupdate);
                if (PriorityMasterid > 0)
                {
                    return Ok(new ComplaintPriorityMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessupdateComplaintPriority });
                }
                else
                {
                    return BadRequest(new ComplaintPriorityMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.updateComplaintPriority, ErrorCode = enErrorCode.Status8042NotupdateComplaintPriority });
                }

            }
            catch (Exception ex)
            {

                return BadRequest(new ComplaintPriorityMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        /// <summary>
        /// Created by pankaj kathiriya for delete  the configuration for complaint priority 
        /// Date :07-01-2019
        /// </summary>
        /// <param name="ComplaintPriority"></param>
        /// <returns></returns>

        [HttpPost("ComplaintPriorityDelete")]
        public async Task<IActionResult> ComplaintPriorityDelete(ComplaintPriorityMasterDeleteViewModel ComplaintPriorityDelete)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                ComplaintPriorityMasterDeleteReqViewModel PriorityMasterDelete = new ComplaintPriorityMasterDeleteReqViewModel()
                {
                    Id = ComplaintPriorityDelete.Id,
                    UserId = user.Id
                };
                long PriorityMasterid = _ComplaintPriorityMaster.Delete(PriorityMasterDelete);
                if (PriorityMasterid > 0)
                {
                    return Ok(new ComplaintPriorityMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccesseDeleteComplaintPriority });
                }
                else
                {
                    return BadRequest(new ComplaintPriorityMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeleteComplaintPriority, ErrorCode = enErrorCode.Status8043NotDeleteComplaintPriority });
                }

            }
            catch (Exception ex)
            {

                return BadRequest(new ComplaintPriorityMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        /// <summary>
        /// Created by pankaj kathiriya for get the list of complaint priority configuration
        /// Date :07-01-2019
        /// </summary>
        /// <param name="ComplaintPriority"></param>
        /// <returns></returns>

        [HttpGet("GetComplaintPriority")]
        public async Task<IActionResult> GetComplaintPriority(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var ComplaintPriorityList = _ComplaintPriorityMaster.GetComplaintPriority(PageIndex, Page_Size);

                return Ok(new ComplaintPrioritygetdataResponse { ReturnCode = enResponseCode.Success, TotalCount = ComplaintPriorityList.TotalCount, ComplaintPriorityGet = ComplaintPriorityList.ComplaintPriorityGet, ReturnMsg = EnResponseMessage.ComplaintPriorityGet });
            }
            catch (Exception ex)
            {

                return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }
        private Task<ApplicationUser> GetCurrentUserAsync()

        {
            return _userManager.GetUserAsync(HttpContext.User);
        }


        [HttpGet("GetSubscribeNewLetter")]
        [AllowAnonymous]
        public ActionResult<GetSubscribeNewLetterResponse> GetSubscribeNewLetter(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                if (Page_Size == 0)
                    Page_Size = Helpers.PageSize;

                return _IBackOfficeReport.GetSubscribeNewLetter(Page_Size, PageIndex);
            }
            catch (Exception ex)
            {

                return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }
        [HttpGet("GetSubscribeNewLetterCount")]
        [AllowAnonymous]
        public ActionResult<GetSubscribeNewLetterCountResponse> GetSubscribeNewLetterCount()
        {
            try
            {
                return _IBackOfficeReport.GetSubscribeNewLetterCount();
            }
            catch (Exception ex)
            {

                return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("RemoveSubscribeNewsLetter/{Id}")]
        [AllowAnonymous]
        public ActionResult<BizResponseClass> RemoveSubscribeNewsLetter(long Id)
        {
            try
            {
                return _IBackOfficeReport.RemoveSubscribeNewsLetter(Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new IPRangeResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
    }
}