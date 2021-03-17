using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces.KYC;
using CleanArchitecture.Core.Interfaces.KYCConfiguration;
using CleanArchitecture.Core.Interfaces.User;
using CleanArchitecture.Core.ViewModels.KYC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class KYCController : ControllerBase
    {
        #region Field
        //private IHostingEnvironment _hostingEnvironment;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IPersonalVerificationService _personalVerificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IKYCConfiguration _kYCConfiguration;
        private readonly IDocumentMaster _documentMaster;

        #endregion

        #region Ctore
        public KYCController(//IHostingEnvironment hostingEnvironment,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            IPersonalVerificationService personalVerificationService, UserManager<ApplicationUser> userManager, IUserService userService,
            IKYCConfiguration kYCConfiguration, IDocumentMaster documentMaster)
        {
            //_hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _personalVerificationService = personalVerificationService;
            _userManager = userManager;
            _userService = userService;
            _kYCConfiguration = kYCConfiguration;
            _documentMaster = documentMaster;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Thid method are used KYC 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("PersonalVerification")]
        // [AllowAnonymous]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> PersonalVerification() //(PersonalVerificationViewModel model)
        {
            try
            {
                var httpRequest = Request.Form;
                var user = await GetCurrentUserAsync();   //After Implementation done please uncommit this code
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                int userid = user.Id;
                //int userid = 167;
                PersonalVerificationViewModel model = new PersonalVerificationViewModel();
                //model.Id = ;
                //model.UserID = 

                if (String.IsNullOrEmpty(httpRequest["IPAddress"].ToString()))
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                //// Ip Address Validate or not
                string CountryCode = await _userService.GetCountryByIP(httpRequest["IPAddress"].ToString());
                if (!string.IsNullOrEmpty(CountryCode) && CountryCode == "fail")
                {
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                }
                if (String.IsNullOrEmpty(httpRequest["DeviceId"].ToString()))
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeviceIdNotFound, ErrorCode = enErrorCode.Status4015DeviceIdNotFound });

                if (String.IsNullOrEmpty(httpRequest["Mode"].ToString()))
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ModeNotFound, ErrorCode = enErrorCode.Status4017ModeNotFound });

                if (String.IsNullOrEmpty(httpRequest["HostName"].ToString()))
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.HostNameNotFound, ErrorCode = enErrorCode.Status4021HostNameNotFound });



                if (httpRequest.Files.Count == 0)
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ImageNotAvailable, ErrorCode = enErrorCode.Status4115ImageNotUpload });
                if (String.IsNullOrEmpty(httpRequest["Surname"].ToString()))
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Surname, ErrorCode = enErrorCode.Status4126SuranName });
                if (String.IsNullOrEmpty(httpRequest["GivenName"].ToString()))
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.GivenName, ErrorCode = enErrorCode.Status4127GivenName });
                if (String.IsNullOrEmpty(httpRequest["ValidIdentityCard"].ToString()))
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ValidIdentityCard, ErrorCode = enErrorCode.Status4128ValidIdentityCard });


                model.UserId = userid;
                model.Surname = httpRequest["Surname"].ToString();
                model.GivenName = httpRequest["GivenName"].ToString();
                model.ValidIdentityCard = httpRequest["ValidIdentityCard"].ToString();
                model.EnableStatus = false;
                model.VerifyStatus = 4;

                var DocumentData = _kYCConfiguration.CheckDocumentFormat(model.ValidIdentityCard);
                if (DocumentData == null)
                {
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IdentiryNotDound, ErrorCode = enErrorCode.Status4147IdentityNotFound });

                }

                var FormatList = _documentMaster.GetFormatList(DocumentData.DocumentMasterId);
                if (FormatList == null)

                {
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FileFormatNotDound, ErrorCode = enErrorCode.Status4148FileFormatnotfound });
                }

                foreach (var file in httpRequest.Files)
                {
                    string data = System.IO.Path.GetExtension(file.FileName);
                    data = data.ToUpper();
                    data = data.Substring(1);
                    model.ValidIdentityCard = DocumentData.ID.ToString();
                    var k = FormatList.Where(f => f.DocumentName == data.ToUpper());
                    if (k == null)
                    {
                        if (file.Name == "Front")
                        {
                            return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FrontImageformatNotvalid, ErrorCode = enErrorCode.Status4142FrontImageFormatNotValid });
                        }
                        if (file.Name == "Back")
                        {
                            return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackImageformatNotvalid, ErrorCode = enErrorCode.Status4143BackImageFormatNotValid });
                        }
                        if (file.Name == "Selfie")
                        {
                            return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SelfieImageformatNotvalid, ErrorCode = enErrorCode.Status4144SelfieImageFormatNotValid });
                        }
                    }
                    var postedFile = httpRequest.Files[file.Name];
                    if (file.Length > Convert.ToInt64(_configuration["KYCImageSize"]))
                    {
                        if (file.Name == "Front")
                        {
                            return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FrontImageSizeLarger, ErrorCode = enErrorCode.Status4123FrontImageLarger });
                        }
                        if (file.Name == "Back")
                        {
                            return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.BackImageSizeLarger, ErrorCode = enErrorCode.Status4124BackImageLarger });
                        }
                        if (file.Name == "Selfie")
                        {
                            return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SelfieImageSizeLarger, ErrorCode = enErrorCode.Status4125SelfiImageLarger });
                        }

                    }

                }
                foreach (var file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file.Name];
                    string folderDirctory = model.UserId.ToString();
                    //string webRootPath = _hostingEnvironment.WebRootPath;
                    string webRootPath = _configuration["KYCImagePath"].ToString();
                    //string newPath = Path.Combine(webRootPath, folderDirctory);
                    string newPath = webRootPath + "/" + folderDirctory;
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    string Extension = System.IO.Path.GetExtension(file.FileName);
                    string fileName = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');
                    fileName = fileName.Replace(fileName, Convert.ToString(System.Guid.NewGuid() + Extension));

                    string fullPath = newPath + "/" + fileName;
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        // postedFile.CopyTo(stream);
                        await postedFile.CopyToAsync(stream);
                    }

                    if (file.Name == "Front")
                    {
                        model.FrontImage = fullPath;
                    }
                    if (file.Name == "Back")
                    {
                        model.BackImage = fullPath;
                    }
                    if (file.Name == "Selfie")
                    {
                        model.SelfieImage = fullPath;
                    }
                }

                var VerificationKYCInserted = _personalVerificationService.IsUserKYCExist(model);
                if (VerificationKYCInserted == null || VerificationKYCInserted.VerifyStatus == Convert.ToInt16(KYCStatus.Reject))
                {

                    var verifyId = await _personalVerificationService.AddPersonalVerification(model);
                    if (verifyId > 0)
                    {
                        return Ok(new PersonalVerificationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.PersonalIdentityInsertSuccessfull });
                    }
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PersonalIdentityNotInserted, ErrorCode = enErrorCode.Status4129PersonalIdentityNotInserted });
                }
                else
                {
                    if (VerificationKYCInserted.VerifyStatus == Convert.ToInt16(KYCStatus.Approval))
                    {

                        return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AllreadyKYCApproval, ErrorCode = enErrorCode.Status4145PersonalIdentityAllreadyApprove });
                    }
                    else
                    {
                        return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AllreadyKYCSubmited, ErrorCode = enErrorCode.Status4146PersonalIdentityAllreadySubmited });
                    }


                }
            }
            catch (Exception ex)
            {
                return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        /// <summary>
        /// Thid method are used GET KYC details
        /// </summary>
        /// <param name="model"></param>
        [HttpGet("GetKYCData")]
        public async Task<IActionResult> GetKYCData()
        {
            try
            {
                string HostURL = Request.Scheme + "://" + HttpContext.Request.Host.ToString();

                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                if (user != null)
                {
                    var userkycdata = _personalVerificationService.GetPersonalVerification(user.Id);
                    if (userkycdata != null)
                    {
                        PersonalVerificationRequest userdata = new PersonalVerificationRequest();

                        userdata.Surname = userkycdata.Surname;
                        userdata.GivenName = userkycdata.GivenName;
                        userdata.ValidIdentityCard = userkycdata.ValidIdentityCard;
                        userdata.FrontImage = HostURL + _configuration["KYCImageGetPath"].ToString() + userkycdata.FrontImage;
                        userdata.BackImage = HostURL + _configuration["KYCImageGetPath"].ToString() + userkycdata.BackImage;
                        userdata.SelfieImage = HostURL + _configuration["KYCImageGetPath"].ToString() + userkycdata.SelfieImage;
                        userdata.EnableStatus = userkycdata.EnableStatus;
                        userdata.VerifyStatus = userkycdata.VerifyStatus;
                        userdata.KYCLevelId = userkycdata.KYCLevelId;

                        return Ok(new PersonalVerificationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetPersonalIdentity, UserKYC = userdata });
                    }
                    else
                        return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PersonalIdentityNotavailable, ErrorCode = enErrorCode.Status4130PersonalIdentityNotavailable });

                }
                else
                    return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SignUpUser, ErrorCode = enErrorCode.Status4063UserNotRegister });
            }

            catch (Exception ex)
            {
                return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }
        #endregion
        [HttpGet("CheckUserKYCStatus")]
        public async Task<IActionResult> CheckUserKYCStatus()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new UserKYCStatusViewModelResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });

                int UserKYCStatus = _personalVerificationService.UserKYCStatus(user.Id);
                if (UserKYCStatus > 0)
                {
                    if (UserKYCStatus == Convert.ToInt16(KYCStatus.Approval))
                        return Ok(new UserKYCStatusViewModelResponse { ReturnCode = enResponseCode.Success, KYCStatus = Convert.ToInt32(KYCStatus.Approval), ReturnMsg = EnResponseMessage.GetUserKYCStatus });
                    else if (UserKYCStatus == Convert.ToInt16(KYCStatus.Pending))
                        return Ok(new UserKYCStatusViewModelResponse { ReturnCode = enResponseCode.Success, KYCStatus = Convert.ToInt32(KYCStatus.Pending), ReturnMsg = EnResponseMessage.GetUserKYCStatus });
                    else if (UserKYCStatus == Convert.ToInt16(KYCStatus.Reject))
                        return Ok(new UserKYCStatusViewModelResponse { ReturnCode = enResponseCode.Success, KYCStatus = Convert.ToInt32(KYCStatus.Reject), ReturnMsg = EnResponseMessage.GetUserKYCStatus });
                    else
                        return Ok(new UserKYCStatusViewModelResponse { ReturnCode = enResponseCode.Success, KYCStatus = 0, ReturnMsg = EnResponseMessage.GetUserKYCStatus });
                }
                else
                    return Ok(new UserKYCStatusViewModelResponse { ReturnCode = enResponseCode.Success, KYCStatus = 0, ReturnMsg = EnResponseMessage.GetUserKYCStatus });
            }
            catch (Exception ex)
            {
                return BadRequest(new PersonalVerificationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #region Helpers
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion
    }
}