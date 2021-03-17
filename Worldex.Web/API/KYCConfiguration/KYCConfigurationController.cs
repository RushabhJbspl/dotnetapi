using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces.KYC;
using CleanArchitecture.Core.Interfaces.KYCConfiguration;
using CleanArchitecture.Core.ViewModels.KYC;
using CleanArchitecture.Core.ViewModels.KYCConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.API.KYCConfiguration
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KYCConfigurationController : ControllerBase
    {
        readonly private IKYCConfiguration _IKYCConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDocumentMaster _DocumentMaster;
        private readonly IKYCLevelMaster _KYCLevelMaster;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        public KYCConfigurationController(IKYCConfiguration kYCConfiguration, UserManager<ApplicationUser> userManager,
            IDocumentMaster documentMaster, IKYCLevelMaster KYCLevelMaster, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _userManager = userManager;
            _IKYCConfiguration = kYCConfiguration;
            _DocumentMaster = documentMaster;
            _KYCLevelMaster = KYCLevelMaster;
            _configuration = configuration;
        }
        [HttpPost("KYCIdentityMasterAdd")]
        public async Task<IActionResult> KYCIdentityMasterAdd(KYCIdentityMasterInsertViewModel kYCIdentityMasterInsertViewModel)
        {
            try
            {
                Guid AllreadyEmailExist = _IKYCConfiguration.IsKYCConfigurationExist(kYCIdentityMasterInsertViewModel.Name);
                if (AllreadyEmailExist != Guid.Empty)
                {
                    return BadRequest(new KYCIdentityMasterInsertResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.KYCConfigurationExist, ErrorCode = enErrorCode.Status8013KYCConfigurationExist });
                }
                else
                {
                    var user = await GetCurrentUserAsync();
                    KYCIdentityMasterInsertReqViewModel kYCIdentityMasterInsertReqViewModel = new KYCIdentityMasterInsertReqViewModel()
                    {
                        Name = kYCIdentityMasterInsertViewModel.Name,
                        UserId = user.Id,
                        DocumentMasterId = kYCIdentityMasterInsertViewModel.DocumentMasterId

                    };
                    _IKYCConfiguration.Add(kYCIdentityMasterInsertReqViewModel);
                    return Ok(new KYCIdentityMasterInsertResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessAddKYCConfiguration });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new KYCIdentityMasterInsertResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        [HttpPost("KYCIdentityMasterUpdate")]
        public async Task<IActionResult> KYCIdentityMasterUpdate(KYCIdentityMasterUpdateViewModel kYCIdentityMasterUpdateViewModel)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                KYCIdentityMasterUpdateReqViewModel kYCIdentityMasterInsertReqViewModel = new KYCIdentityMasterUpdateReqViewModel()
                {
                    Name = kYCIdentityMasterUpdateViewModel.Name,
                    UserId = user.Id,
                    Id = kYCIdentityMasterUpdateViewModel.Id,
                    Stutas = kYCIdentityMasterUpdateViewModel.Stutas,
                    DocumentMasterId = kYCIdentityMasterUpdateViewModel.DocumentMasterId
                };
                Guid Configuration = _IKYCConfiguration.Update(kYCIdentityMasterInsertReqViewModel);
                if (Configuration != Guid.Empty)
                {
                    return Ok(new KYCIdentityMasterInsertResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessUpdateKYCConfiguration });
                }

                else
                {
                    return BadRequest(new KYCIdentityMasterInsertResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.updateKYCConfigurationFail, ErrorCode = enErrorCode.Status8014KYCConfigurationUpdateFail });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new KYCIdentityMasterInsertResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("KYCIndentityGetList")]
        public async Task<IActionResult> KYCIndentityGetList()
        {
            try
            {
                var KYCIdentirylist = _IKYCConfiguration.KYCIdentityGetList();
                return Ok(new KYCIndentitylistDataResponseViewModel { ReturnCode = enResponseCode.Success, KYCIndentitylistViewModels = KYCIdentirylist, ReturnMsg = EnResponseMessage.GetSuccessFullyIdentitylist });
            }
            catch (Exception ex)
            {

                return BadRequest(new KYCIndentityConfigurationList { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("UserKYCIdentityMappingAdd")]
        public async Task<IActionResult> UserKYCIdentityMappingAdd(UserKYCConfigurationMappingViewModel userKYCConfigurationMappingReqViewModel)
        {
            try
            {

                var user = await GetCurrentUserAsync();
                UserKYCConfigurationMappingReqViewModel userKYCConfigurationMappingViewModel = new UserKYCConfigurationMappingReqViewModel()
                {
                    UserId = user.Id,
                    KYCConfigurationMasterId = userKYCConfigurationMappingReqViewModel.KYCConfigurationMasterId,
                    LevelId = userKYCConfigurationMappingReqViewModel.LevelId
                };
                Guid IKYCConfiguration = _IKYCConfiguration.IsKYCConfigurationmappingExist(userKYCConfigurationMappingViewModel);

                if (IKYCConfiguration != Guid.Empty)
                {
                    return BadRequest(new UserKYCConfigurationMappingresponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ADDKYCConfigurationUserMappingExist, ErrorCode = enErrorCode.Status8017KYCConfigurationUserMappingAllreadyExist });
                }
                else
                {
                    Guid ConfigurationMapping = _IKYCConfiguration.AddUserKYCMappingConfiguration(userKYCConfigurationMappingViewModel);
                    if (ConfigurationMapping != Guid.Empty)
                    {
                        return Ok(new UserKYCConfigurationMappingresponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessAddUserConfigurationMapping });
                    }
                    else
                    {
                        return BadRequest(new UserKYCConfigurationMappingresponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ADDKYCConfigurationMappingFail, ErrorCode = enErrorCode.Status8017KYCConfigurationUserMappingAllreadyExist });
                    }
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new UserKYCConfigurationMappingresponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("UserKYCIdentityMappingUpdate")]
        public async Task<IActionResult> UserKYCIdentityMappingUpdate(UserKYCConfigurationMappingUpdateViewModel userKYCConfigurationMappingUpdateViewModel)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                UserKYCConfigurationMappingUpdateReqViewModel userKYCConfigurationMappingUpdateReqViewModel = new UserKYCConfigurationMappingUpdateReqViewModel()
                {
                    UserId = user.Id,
                    Id = userKYCConfigurationMappingUpdateViewModel.Id,
                    Status = userKYCConfigurationMappingUpdateViewModel.Status,
                    LevelId = userKYCConfigurationMappingUpdateViewModel.LevelId

                };
                Guid IKYCConfiguration = _IKYCConfiguration.UpdateKYCMappig(userKYCConfigurationMappingUpdateReqViewModel);

                if (IKYCConfiguration != Guid.Empty)
                {
                    return Ok(new UserKYCConfigurationMappingresponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessUpdateKYCConfigurationmapping });
                }
                else
                {
                    return BadRequest(new UserKYCConfigurationMappingresponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UpdateUserKYCConfigurationMappingFail, ErrorCode = enErrorCode.Status8019KYCConfigurationUserMappingnotupdate });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new UserKYCConfigurationMappingresponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("KYCIndentityConfigurationList")]
        public async Task<IActionResult> KYCIndentityConfigurationList()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var Configurationlist = _IKYCConfiguration.KYCIndentityConfigurationlist(user.Id);
                return Ok(new KYCIndentityConfigurationList { ReturnCode = enResponseCode.Success, kYCIndentitylistViewModel = Configurationlist, ReturnMsg = EnResponseMessage.GetSuccessFullyKYCConfigurationlist });
            }
            catch (Exception ex)
            {

                return BadRequest(new KYCIndentityConfigurationList { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("DocumasterAdd")]
        public async Task<IActionResult> DocumasterAdd(DocumentMasterViewMaster documentMasterViewMaster)
        {
            try
            {
                var DocumentExist = _DocumentMaster.IsDocumentExist(documentMasterViewMaster.Name);
                if (DocumentExist != Guid.Empty)
                {
                    return BadRequest(new DocumentMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DocumentMasterExist, ErrorCode = enErrorCode.Status8021DocumentmasterExist });
                }
                else
                {
                    var user = await GetCurrentUserAsync();
                    DocumentmasterReqViewMode documentmasterReqViewMode = new DocumentmasterReqViewMode()
                    {
                        Name = documentMasterViewMaster.Name,
                        UserId = user.Id
                    };
                    Guid DocumentId = _DocumentMaster.AddDocumentMaster(documentmasterReqViewMode);
                    if (DocumentId != Guid.Empty)
                    {
                        return Ok(new DocumentMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DocumentAddSuccessfully });
                    }
                    else
                    {
                        return BadRequest(new DocumentMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DocumentMasterNotAdded, ErrorCode = enErrorCode.Status8022DocumentMasterNotAdded });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new DocumentMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("DocumasterUpdate")]
        public async Task<IActionResult> DocumasterUpdate(DocumentMasterUpdateViewModel documentMasterUpdateViewModel)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                DocumentMasterUpdateReqViewModel documentMasterUpdateReqViewModel = new DocumentMasterUpdateReqViewModel()
                {
                    Name = documentMasterUpdateViewModel.Name,
                    UserId = user.Id,
                    Id = documentMasterUpdateViewModel.Id,
                    Status = documentMasterUpdateViewModel.Status

                };
                Guid DocumentId = _DocumentMaster.UpdateDocumentMaster(documentMasterUpdateReqViewModel);
                if (DocumentId != Guid.Empty)
                {
                    return Ok(new DocumentMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DocumentupdateSuccessfully });
                }
                else
                {
                    return BadRequest(new DocumentMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DocumentMasterNotAdded, ErrorCode = enErrorCode.Status8023DocumentMasterNotupdate });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new DocumentMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }



        [HttpPost("DocumasterList")]
        public async Task<IActionResult> DocumasterList()
        {
            try
            {
                var DocumentMasterList = _DocumentMaster.KYCDocumentGetList();
                return Ok(new DocumentMasterListResponse { ReturnCode = enResponseCode.Success, DocumentMasterListViewModels = DocumentMasterList, ReturnMsg = EnResponseMessage.DocumentListSuccessfullyGet });
            }
            catch (Exception ex)
            {

                return BadRequest(new DocumentMasterListResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpGet("KYCConfigurationUserWiseList")]
        public async Task<IActionResult> KYCConfigurationUserWiseList()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var KYCDocumentList = _DocumentMaster.KYCConfigurationUserWiseDocumentList(user.Id);
                return Ok(new KYCDocumentConfigurationListDisplayResponseViewmodel { ReturnCode = enResponseCode.Success, KYCDocumentConfigurationListDisplayViewmodels = KYCDocumentList, ReturnMsg = EnResponseMessage.GetSuccessFullyKYCConfigurationUserwiselist });
            }
            catch (Exception ex)
            {
                return BadRequest(new KYCDocumentConfigurationListDisplayResponseViewmodel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("KYCLevelAdd")]
        public async Task<IActionResult> KYCLevelAdd(KYCLevelInsertViewModel kYCLevelInsertViewModel)
        {
            try
            {
                long Id = _KYCLevelMaster.IsKYCKYCLevelExist(kYCLevelInsertViewModel.KYCName);
                if (Id > 0)
                {
                    return BadRequest(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IsKYCNameExist, ErrorCode = enErrorCode.Status8026IsKYCNameExist });
                }
                else
                {
                    var user = await GetCurrentUserAsync();
                    KYCLevelInsertReqViewModel kYCLevelInsertReqViewModel = new KYCLevelInsertReqViewModel()
                    {
                        DeviceId = kYCLevelInsertViewModel.DeviceId,
                        HostName = kYCLevelInsertViewModel.HostName,
                        IPAddress = kYCLevelInsertViewModel.IPAddress,
                        Userid = user.Id,
                        KYCName = kYCLevelInsertViewModel.KYCName,
                        Level = kYCLevelInsertViewModel.Level,
                        Mode = kYCLevelInsertViewModel.Mode
                    };
                    long id = _KYCLevelMaster.ADDKYCLevel(kYCLevelInsertReqViewModel);
                    if (id > 0)
                        return Ok(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessAddKYCLevel });
                    else
                        return BadRequest(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.AddKYCLevelNotAdded, ErrorCode = enErrorCode.Status8025LevekMasterNotAdded });

                }
            }
            catch (Exception ex)
            {

                return BadRequest(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("KYCLevelUpdate")]
        public async Task<IActionResult> KYCLevelUpdate(KYCLevelUpdateViewModel KYCLevelUpdateViewModel)
        {
            try
            {

                var user = await GetCurrentUserAsync();
                KYCLevelUpdateReqViewModel KYCLevelUpdateReqViewModel = new KYCLevelUpdateReqViewModel()
                {
                    Id = KYCLevelUpdateViewModel.Id,
                    status = KYCLevelUpdateViewModel.status,
                    DeviceId = KYCLevelUpdateViewModel.DeviceId,
                    HostName = KYCLevelUpdateViewModel.HostName,
                    IPAddress = KYCLevelUpdateViewModel.IPAddress,
                    UserId = user.Id,
                    KYCName = KYCLevelUpdateViewModel.KYCName,
                    Level = KYCLevelUpdateViewModel.Level,
                    Mode = KYCLevelUpdateViewModel.Mode
                };
                long id = _KYCLevelMaster.UpdateKYCLevel(KYCLevelUpdateReqViewModel);
                if (id > 0)
                    return Ok(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessUpdateKYCLevel });
                else
                    return BadRequest(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.KYCLevelNotUpdated, ErrorCode = enErrorCode.Status8027AddKYCLevelNotUpdated });


            }
            catch (Exception ex)
            {
                return BadRequest(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }


        }
        [HttpGet("GetKYCLevelList")]
        public async Task<IActionResult> GetKYCLevelList(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var KYCFilterData = _KYCLevelMaster.GetKYCLevelList(PageIndex, Page_Size);
                return Ok(new KYCLevelListResponse { ReturnCode = enResponseCode.Success, TotalCount = KYCFilterData.TotalCount, KYCLevelList = KYCFilterData.KYCLevelList, ReturnMsg = EnResponseMessage.SuccessFullyGetKYCLevelList });
            }
            catch (Exception ex)
            {
                return BadRequest(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpGet("GetKYCLevelDropDownList")]
        public async Task<IActionResult> GetKYCLevelDropDownList()
        {
            try
            {
                var KYCFilterData = _KYCLevelMaster.GetKYCLevelList();
                return Ok(new KYCLevelDropDownListResponse { ReturnCode = enResponseCode.Success, KYCLevelList = KYCFilterData, ReturnMsg = EnResponseMessage.SuccessFullyGetKYCLevelList });
            }
            catch (Exception ex)
            {
                return BadRequest(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("KYCLevelWiseCount")]
        public async Task<IActionResult> KYCUserWiseLevelCount(int level)
        {
            try
            {
                int KYCLevelData = _KYCLevelMaster.KYCUserWiseLevelCount(level);
                return Ok(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.Success, Count = KYCLevelData, ReturnMsg = EnResponseMessage.GetSuccessfullykycLevel });

            }
            catch (Exception ex)
            {
                return BadRequest(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetKYCList")]
        public async Task<IActionResult> GetKYCList(DateTime? FromDate, DateTime? ToDate, int PageIndex = 0, int Page_Size = 0, int Status = 0, string Mobile = null, string EmailAddress = null)
        {
            try
            {
                string HostURL = Request.Scheme + "://" + HttpContext.Request.Host.ToString() + _configuration["KYCImageGetPath"].ToString();
                var KYCFilterData = _IKYCConfiguration.GetKYCList(FromDate, ToDate, PageIndex, Page_Size, Status, Mobile, EmailAddress, HostURL);
                return Ok(new KYCListFilterationDataListResponseViewModel { ReturnCode = enResponseCode.Success, TotalCount = KYCFilterData.TotalCount, kYCListFilterationDataViewModels = KYCFilterData.kYCListFilterationDataViewModels, ReturnMsg = EnResponseMessage.SuccessFullyGetKYCFilterationList });

            }
            catch (Exception ex)
            {
                return BadRequest(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("KYCStatusUpdate")]
        public async Task<IActionResult> KYCStatusUpdate(KYCUpdateViewModel kYCUpdateViewModel)
        {
            try
            {
                if (Convert.ToInt16(KYCStatus.Reject) == kYCUpdateViewModel.VerifyStatus)
                {
                    if (string.IsNullOrEmpty(kYCUpdateViewModel.Remark))
                    {
                        return BadRequest(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.KYCRemark, ErrorCode = enErrorCode.Status8028AddKYCRemark });
                    }
                }

                long VerificationId = _IKYCConfiguration.KYCVerification(kYCUpdateViewModel);
                if (VerificationId > 0)
                {
                    return Ok(new KYCListFilterationDataListResponseViewModel { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessFullyupdateKYC });
                }
                else
                {
                    return BadRequest(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.KYCRecordNotFound, ErrorCode = enErrorCode.Status8029KYCrecordnotfound });
                }


            }
            catch (Exception ex)
            {
                return BadRequest(new KYCLevelResponseViewModel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


    }
}