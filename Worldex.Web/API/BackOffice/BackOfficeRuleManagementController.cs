using System;
using System.Threading.Tasks;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Helpers;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Interfaces.BackOffice;
using CleanArchitecture.Core.ViewModels;
using CleanArchitecture.Core.ViewModels.BackOffice;
using CleanArchitecture.Core.ViewModels.BackOfficeUser;
using CleanArchitecture.Infrastructure.BGTask;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.API.BackOffice
{
    //  khuhsali 15-02-2015
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BackOfficeRuleManagementController : BaseController
    {
        #region Field

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessageService _messageService;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private readonly ISignalRService _signalRService;
        private readonly IMediator _mediator;
        private readonly IRuleManageService _ruleManageService;


        #endregion

        #region Ctore

        public BackOfficeRuleManagementController(UserManager<ApplicationUser> userManager, IMessageService MessageService,
            IPushNotificationsQueue<SendSMSRequest> PushSMSQueue, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue,
            ISignalRService signalRService, IMediator mediator, IRuleManageService RuleManageService)
        {
            _userManager = userManager;            
            _messageService = MessageService;
            _pushNotificationsQueue = pushNotificationsQueue;
            _pushSMSQueue = PushSMSQueue;
            _signalRService = signalRService;
            _mediator = mediator;
            _ruleManageService = RuleManageService;
        }

        #endregion

        #region Module Method khuhsali 15-02-2015
        [HttpGet("GetAllModuleData/{PageNo}")]
        public async Task<IActionResult> GetAllModuleData(int PageNo, int? PageSize, short? AllRecords = null, bool IsParentList = false)
        {
            ListModuleDataViewModel Response = new ListModuleDataViewModel();
            try
            {
                int PageSizeDef = Helpers.PageSize;
                if(PageSize != null)
                {
                    PageSizeDef = Convert.ToInt32(PageSize);
                }
                var Data =await _ruleManageService.GetAllModuleData(PageNo+1, PageSizeDef,AllRecords, IsParentList);
                Response = Data;
                Response.PageNo = PageNo;
                Response.PageSize = PageSizeDef;
                if (Data == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.BackofficeModuleDataInternalError;
                    Response.ErrorCode = enErrorCode.Status500InternalServerError;
                    return Ok(Response);
                }
                else if(Data.Result.Count == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Ok(Response);
                }
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetAllModuleDataOldLogic/{PageNo}")]
        public async Task<IActionResult> GetAllModuleDataV2(int PageNo, int? PageSize)
        {
            ListModuleDataViewModel Response = new ListModuleDataViewModel();
            try
            {
                int PageSizeDef = Helpers.PageSize;
                if (PageSize != null)
                {
                    PageSizeDef = Convert.ToInt32(PageSize);
                }
                var Data = await _ruleManageService.GetAllModuleDataV2(PageNo + 1, PageSizeDef);
                Response = Data;
                Response.PageNo = PageNo;
                Response.PageSize = PageSizeDef;
                if (Data == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.BackofficeModuleDataInternalError;
                    Response.ErrorCode = enErrorCode.Status500InternalServerError;
                    return Ok(Response);
                }
                else if (Data.Result.Count == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Ok(Response);
                }
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetModuleDataByID/{Id:long}")]
        public async Task<IActionResult> GetModuleDataByID(long Id)
        {
            ResponseModuleDataViewModel res = new ResponseModuleDataViewModel();
            try
            {
                res.Result = await _ruleManageService.GetModuleDataByID(Id);
                if (res.Result == null)
                {
                    res.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.RecordNotFound;
                    return Ok(res);
                }
                res.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                res.ReturnCode = enResponseCode.Success;
                res.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("AddModuleData")]
        public async Task<IActionResult> AddModuleData([FromBody]ModuleDataViewModel Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                var response = await _ruleManageService.AddModuleData(Request, 1);
                if (response != 0)
                {
                    Response.ReturnMsg = EnResponseMessage.CommRecordInsertSuccess;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.RecordCreatedSuccessfully;
                }
                else if(response == 0)
                {
                    Response.ReturnMsg = EnResponseMessage.RuleManagementRecordAlredyExist;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.RecordAlreadyExist;
                }
                else
                {
                    Response.ReturnMsg = EnResponseMessage.BackofficeModuleDataInternalError;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InternalError;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("UpdateModuleData")]
        public async Task<IActionResult> UpdateModuleData([FromBody]ModuleDataViewModel request)
        {
            BizResponseClass res = new BizResponseClass();
            //bool state = false;//komal 03 May 2019, Cleanup
            try
            {
                if (request.ModuleID == 0)
                {
                    res.ReturnMsg = "InValid ID";
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.InValid_ID;
                    return Ok(res);
                }

                res = await _ruleManageService.UpdateModuleData(request, 1);
                if (res == null)
                {
                    res.ReturnMsg = "Internal Error";
                    res.ReturnCode = enResponseCode.InternalError;
                    res.ErrorCode = enErrorCode.Status500InternalServerError;
                    return Ok(res);
                }                
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("ChangeStatusModuleData")]
        public async Task<IActionResult> ChangeStatusModuleData(long id, short Status)
        {
            BizResponseClass res = new BizResponseClass();
            try
            {
                var response = await _ruleManageService.ChangeStatusModuleData(id, Status);
                if (response == true)
                {
                    res.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                }
                else
                {
                    res.ReturnMsg = EnResponseMessage.BackofficeModuleDataInternalError;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.NoRecordFound;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }
        #endregion

        #region Sub Module Method   khushali 15-02-2015
        [HttpGet("GetAllSubModuleData/{PageNo}")]
        public async Task<IActionResult> GetAllSubModuleData(int PageNo, int? PageSize,short? AllRecords = null, bool IsParentList = false)
        {
            ListSubModuleDataViewModel Response = new ListSubModuleDataViewModel();
            try
            {
                int PageSizeDef = Helpers.PageSize;
                if (PageSize != null)
                {
                    PageSizeDef = Convert.ToInt32(PageSize);
                }
                var Data = await _ruleManageService.GetAllSubModuleData(PageNo + 1, PageSizeDef,AllRecords, IsParentList);
                Response = Data;
                Response.PageNo = PageNo;
                Response.PageSize = PageSizeDef;
                if (Data == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.BackofficeSubModuleDataInternalError;
                    Response.ErrorCode = enErrorCode.Status500InternalServerError;
                    return Ok(Response);
                }
                else if (Data.Result.Count == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Ok(Response);
                }
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }        

        [HttpGet("GetSubModuleDataByID/{Id:long}")]
        public async Task<IActionResult> GetSubModuleDataByID(long Id)
        {
            ResponseSubModuleDataViewModel res = new ResponseSubModuleDataViewModel();
            try
            {
                res.Result = await _ruleManageService.GetSubModuleDataByID(Id);
                if (res.Result == null)
                {
                    res.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.NoDataFound;
                    return Ok(res);
                }
                res.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                res.ReturnCode = enResponseCode.Success;
                res.ErrorCode = enErrorCode.Success;
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("AddSubModuleData")]
        public async Task<IActionResult> AddSubModuleData([FromBody]SubModuleDataViewModel Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (Request.ParentGUID == null)
                {
                    Response.ReturnMsg = "Please Enter Required Parameter";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InternalError;
                    return Ok(Response);
                }
                var response = await _ruleManageService.AddSubModuleData(Request, 1);
                if (response != 0)
                {
                    Response.ReturnMsg = EnResponseMessage.CommRecordInsertSuccess;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                }
                else if (response == 0)
                {
                    Response.ReturnMsg = EnResponseMessage.RuleManagementRecordAlredyExist;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.RecordAlreadyExist;
                }
                else
                {
                    Response.ReturnMsg = EnResponseMessage.BackofficeModuleDataInternalError;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InternalError;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("UpdateSubModuleData")]
        public async Task<IActionResult> UpdateSubModuleData([FromBody]SubModuleDataViewModel request)
        {
            BizResponseClass res = new BizResponseClass();
            //bool state = false; //komal 03 May 2019, Cleanup
            try
            {
                if (request.SubModuleID == 0)
                {
                    res.ReturnMsg = "Invalid ID";
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.InValid_ID;
                    return Ok(res);
                }

                res = await _ruleManageService.UpdateSubModuleData(request, 1);
                if (res == null)
                {
                    res.ReturnMsg = "Internal Error";
                    res.ReturnCode = enResponseCode.InternalError;
                    res.ErrorCode = enErrorCode.Status500InternalServerError;
                    return Ok(res);
                }                
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }

        }

        [HttpPost("ChangeStatusSubModuleData")]
        public async Task<IActionResult> ChangeStatusSubModuleData(long id, short Status)
        {
            BizResponseClass res = new BizResponseClass();
            try
            {
                var response = await _ruleManageService.ChangeStatusSubModuleData(id, Status);
                if (response == true)
                {
                    res.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    res.ReturnMsg = EnResponseMessage.BackofficeModuleDataInternalError;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.NoDataFound;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }
        #endregion

        #region Field Method  khuhsali 15-02-2015
        [HttpGet("GetAllFieldData/{PageNo}")]
        public async Task<IActionResult> GetAllFieldData(int PageNo, int? PageSize, short? AllRecords = null)
        {
            ListFieldDataViewModel Response = new ListFieldDataViewModel();
            try
            {
                int PageSizeDef = Helpers.PageSize;
                if (PageSize != null)
                {
                    PageSizeDef = Convert.ToInt32(PageSize);
                }
                var Data = await _ruleManageService.GetAllFieldData(PageNo + 1, PageSizeDef,AllRecords);
                Response = Data;
                Response.PageNo = PageNo;
                Response.PageSize = PageSizeDef;
                if (Data == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.BackofficeFieldDataInternalError;
                    Response.ErrorCode = enErrorCode.Status500InternalServerError;
                    return Ok(Response);
                }
                else if (Data.Result.Count == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    Response.ErrorCode = enErrorCode.RecordNotFound;
                    return Ok(Response);
                }
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetFieldDataByID/{Id:long}")]
        public async Task<IActionResult> GetFieldDataByID(long Id)
        {
            ResponseFieldDataViewModel res = new ResponseFieldDataViewModel();
            try
            {
                res.Result = await _ruleManageService.GetFieldDataByID(Id);
                if (res.Result == null)
                {
                    res.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.NoRecordFound;
                    return Ok(res);
                }
                res.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                res.ReturnCode = enResponseCode.Success;
                res.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("AddFieldData")]
        public async Task<IActionResult> AddFieldData([FromBody]FieldDataViewModel Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (Request.ModulleGUID == null)
                {
                    Response.ReturnMsg = "Please Enter Required Parameter";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InternalError;
                    return Ok(Response);
                }
                var response = await _ruleManageService.AddFieldData(Request, 1);
                if (response != 0)
                {
                    Response.ReturnMsg = EnResponseMessage.CommRecordInsertSuccess;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.RecordCreatedSuccessfully;
                }
                else if (response == 0)
                {
                    Response.ReturnMsg = EnResponseMessage.RuleManagementRecordAlredyExist;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.RecordAlreadyExist;
                }
                else
                {
                    Response.ReturnMsg = EnResponseMessage.BackofficeModuleDataInternalError;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InternalError;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("UpdateFieldData")]
        public async Task<IActionResult> UpdateFieldData([FromBody]FieldDataViewModel request)
        {
            BizResponseClass res = new BizResponseClass();
            //bool state = false; //komal 03 May 2019, Cleanup
            try
            {
                if (request.FieldID == 0)
                {
                    res.ReturnMsg = "Invalid ID";
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.InValid_ID;
                    return Ok(res);
                }

                res = await _ruleManageService.UpdateFieldData(request, 1);
                if (res == null)
                {
                    res.ReturnMsg = "Internal Error";
                    res.ReturnCode = enResponseCode.InternalError;
                    res.ErrorCode = enErrorCode.Status500InternalServerError;
                    return Ok(res);
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }

        }

        [HttpPost("ChangeStatusFieldData")]
        public async Task<IActionResult> ChangeStatusFieldData(long id, short Status)
        {
            BizResponseClass res = new BizResponseClass();
            try
            {
                var response = await _ruleManageService.ChangeStatusFieldData(id, Status);
                if (response == true)
                {
                    res.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                }
                else
                {
                    res.ReturnMsg = EnResponseMessage.BackofficeModuleDataInternalError;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.NoRecordFound;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }
        #endregion

        #region Tool Method  khuhsali 15-02-2015
        [HttpGet("GetAllToolData/{PageNo}")]
        public async Task<IActionResult> GetAllToolData(int PageNo, int? PageSize, short? AllRecords = null)
        {
            ListToolDataViewModel Response = new ListToolDataViewModel();
            try
            {
                int PageSizeDef = Helpers.PageSize;
                if (PageSize != null)
                {
                    PageSizeDef = Convert.ToInt32(PageSize);
                }
                var Data = await _ruleManageService.GetAllToolData(PageNo + 1, PageSizeDef, AllRecords);
                Response = Data;
                Response.PageNo = PageNo;
                Response.PageSize = PageSizeDef;
                if (Data == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.BackofficeToolDataInternalError;
                    Response.ErrorCode = enErrorCode.Status500InternalServerError;
                    return Ok(Response);
                }
                else if (Data.Result.Count == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Ok(Response);
                }
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                Response.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BackOfficeUserResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetToolDataByID/{Id:long}")]
        public async Task<IActionResult> GetToolDataByID(long Id)
        {
            ResponseToolDataViewModel res = new ResponseToolDataViewModel();
            try
            {
                res.Result = await _ruleManageService.GetToolDataByID(Id);
                if (res.Result == null)
                {
                    res.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.NoRecordFound;
                    return Ok(res);
                }
                res.ReturnMsg = EnResponseMessage.RuleManagementDataFound;
                res.ReturnCode = enResponseCode.Success;
                res.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("AddToolData")]
        public async Task<IActionResult> AddToolData([FromBody]ToolDataViewModel Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (Request.SubModuleID == null || Request.SubModuleID == 0)
                {
                    Response.ReturnMsg = "Please Enter Required Parameter";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InternalError;
                    return Ok(Response);
                }
                var response = await _ruleManageService.AddToolData(Request, 1);
                if (response != 0)
                {
                    Response.ReturnMsg = EnResponseMessage.CommRecordInsertSuccess;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.RecordCreatedSuccessfully;
                }
                else if (response == 0)
                {
                    Response.ReturnMsg = EnResponseMessage.RuleManagementRecordAlredyExist;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.RecordAlreadyExist;
                }
                else
                {
                    Response.ReturnMsg = EnResponseMessage.BackofficeModuleDataInternalError;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InternalError;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }

        [HttpPost("UpdateToolData")]
        public async Task<IActionResult> UpdateToolData([FromBody]ToolDataViewModel request)
        {
            BizResponseClass res = new BizResponseClass();
            //bool state = false;//komal 03 May 2019, Cleanup
            try
            {
                if (request.ToolID == 0)
                {
                    res.ReturnMsg = "Invalid ID";
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.InValid_ID;
                    return Ok(res);
                }

                res = await _ruleManageService.UpdateToolData(request, 1);
                if (res == null)
                {
                    res.ReturnMsg = "Internal Error";
                    res.ReturnCode = enResponseCode.InternalError;
                    res.ErrorCode = enErrorCode.Status500InternalServerError;
                    return Ok(res);
                }                
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }

        }

        [HttpPost("ChangeStatusToolData")]
        public async Task<IActionResult> ChangeStatusToolData(long id, short Status)
        {
            BizResponseClass res = new BizResponseClass();
            try
            {
                var response = await _ruleManageService.ChangeStatusToolData(id, Status);
                if (response == true)
                {
                    res.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                }
                else
                {
                    res.ReturnMsg = EnResponseMessage.BackofficeModuleDataInternalError;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.NoRecordFound;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });

            }
        }
        #endregion

        //#region Testing Methods    

       

        //[HttpGet("ModuleuserAssign")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ModuleuserAssign()
        //{
        //    //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //    var Data = await _ruleManageService.GetAssignedRule(Convert.ToInt64(57));            
        //    return Ok(Data);
        //}


        //[HttpGet("CreateAssignedRule")]
        //[AllowAnonymous]
        //public async Task<IActionResult> CreateAssignedRule()
        //{
        //    //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //    var Data = await _ruleManageService.CreateAssignedRule(57);

        //    return Ok(Data);
        //}

        //[HttpGet("CreateAssignedRuleV2")]
        //[AllowAnonymous]
        //public async Task<IActionResult> CreateAssignedRuleV2(UserAccessRightsViewModel Request)
        //{
        //    //ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //    var Data = await _ruleManageService.CreateAssignedRuleV2(Request);
        //    return Ok(Data);
        //}
        
        //#endregion
    }
}