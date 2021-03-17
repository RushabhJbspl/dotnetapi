using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Backoffice.RoleManagement;
using Worldex.Core.Entities.User;
using Worldex.Core.Entities.UserRoleManagement;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.BackOffice;
using Worldex.Core.Interfaces.ControlPanel;
using Worldex.Core.ViewModels.RoleConfig;
using Worldex.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using MediatR;
using Worldex.Core.ViewModels;
using Worldex.Core.Services;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using Newtonsoft.Json;
using Worldex.Core.ApiModels.Chat;
using Worldex.Infrastructure.BGTask;
using Worldex.Core.ViewModels.BackOffice;
using Worldex.Core.Interfaces.RoleManagement;
using Worldex.Core.Entities.GroupModuleManagement;
using Worldex.Infrastructure.Data.RoleManagement;
using Worldex.Core.ViewModels.GroupManagement;
using AutoMapper;
using Worldex.Core.ViewModels.AccountViewModels.Log;
using Worldex.Core.Interfaces.User;
using Worldex.Core.Interfaces.Log;

namespace Worldex.Infrastructure.Services.BackOffice.RoleManagement
{
    public class RoleManagementServices : BasePage, IRoleManagementServices
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommonRepository<RoleHistory> _RoleHistorycommonRepo;
        private readonly ICommonRepository<UserAccessRights> _UserAccessRights;
        private readonly IControlPanelRepository _controlPanelRepository;
        private readonly IRuleManageService _ruleManageService;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IMessageService _messageService;
        private readonly ICommonRepository<PermissionGroupMaster> _PermissionGroupMasterCommonRepo;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;

        // Added by Nishit Jani on A 2019-03-28 4:36 PM
        private readonly IAsyncRepository<SubModuleMaster> _SubModuleRepository;
        private readonly IAsyncRepository<ModuleGroupAccess> _moduleGroupAccess;
        private readonly ICommonRepository<ModuleGroupMaster> _moduleGroupMaster;
        private readonly IAsyncRepository<SubModuleFormMaster> _subModuleFormMaster;
        private readonly IAsyncRepository<FieldMaster> _fieldMaster;
        private readonly IAsyncRepository<ModuleVisibilityMaster> _moduleVisibilityMaster;
        private readonly IAsyncRepository<ModuleFieldRequirerMaster> _moduleFieldRequirerMaster;
        private readonly IAsyncRepository<ModuleAccessRightsMaster> _moduleAccessRightsMaster;

        private readonly IAsyncRepository<ModuleCRUDOptMaster> _moduleCRUDOptMaster;
        private readonly IAsyncRepository<ModuleDomainMaster> _moduleDomainMaster;
        private readonly IAsyncRepository<ModuleUtilityMaster> _moduleUtilityMaster;
        private readonly IAsyncRepository<ModuleTypeMaster> _moduleTypeMaster;

        private static IEnumerable<ModuleTypeMaster> ModuleTypeMasterListV2 = null;
        private static IEnumerable<ModuleCRUDOptMaster> CRUDOptMasterListV2 = null;
        private static IEnumerable<ModuleUtilityMaster> UtilityMasterListV2 = null;
        private static IEnumerable<SubModuleMaster> SubModuleMasterListV2 = null;
        private static IEnumerable<SubModuleMaster> SubModuleMasterListV2Full = null;
        private static IEnumerable<SubModuleFormMaster> SubModuleFormMasterListV2 = null;
        private static IEnumerable<ModuleGroupAccess> ModuleGroupAccessListV2 = null;
        private static IEnumerable<FieldMaster> FieldMasterListV2 = null;
        private static IEnumerable<ModuleVisibilityMaster> ModuleVisibilityMasterListV2 = null;
        private static IEnumerable<ModuleFieldRequirerMaster> ModuleFieldRequirerMasterListV2 = null;
        private static IEnumerable<ModuleAccessRightsMaster> ModuleAccessRightsMasterListV2 = null;
        private static IEnumerable<SubModuleMaster> moduleMasterListV2 = null;
        private readonly IRegisterTypeService _registerTypeService;
        private readonly ISignupLogService _IsignupLogService;
        private readonly IUserService _userdata;

        public RoleManagementServices(ILogger<BasePage> logger, ICommonRepository<UserAccessRights> userAccessRights, EncyptedDecrypted encdecAEC,
            IControlPanelRepository controlPanelRepository, UserManager<ApplicationUser> userManager, IRuleManageService RuleManageService,
            RoleManager<ApplicationRole> roleManager, ICommonRepository<RoleHistory> RoleHistorycommonRepo, IMessageService MessageService,
            WorldexContext dbcontext, ICommonRepository<PermissionGroupMaster> PermissionGroupMasterCommonRepo,
            IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            IAsyncRepository<ModuleGroupAccess> ModuleGroupAccess, ICommonRepository<ModuleGroupMaster> ModuleGroupMaster,
            IAsyncRepository<SubModuleFormMaster> SubModuleFormMaster, IAsyncRepository<ModuleCRUDOptMaster> ModuleCRUDOptMaster,
            IAsyncRepository<ModuleDomainMaster> ModuleDomainMaster, IAsyncRepository<ModuleUtilityMaster> ModuleUtilityMaster,
            IAsyncRepository<SubModuleMaster> SubModuleRepository, IAsyncRepository<ModuleTypeMaster> moduleTypeMaster, IAsyncRepository<FieldMaster> fieldMaster,
            IUserService userdata,
            IAsyncRepository<ModuleVisibilityMaster> moduleVisibilityMaster, IAsyncRepository<ModuleFieldRequirerMaster> moduleFieldRequirerMaster,
            IAsyncRepository<ModuleAccessRightsMaster> moduleAccessRightsMaster, IRegisterTypeService registerTypeService, ISignupLogService IsignupLogService)
            //IUserRoleStore<ApplicationUser> UserRoleManager1)
            : base(logger)
        {
            _roleManager = roleManager;
            _ruleManageService = RuleManageService;
            _userManager = userManager;
            _UserAccessRights = userAccessRights;
            _encdecAEC = encdecAEC;
            _controlPanelRepository = controlPanelRepository;
            _RoleHistorycommonRepo = RoleHistorycommonRepo;
            _PermissionGroupMasterCommonRepo = PermissionGroupMasterCommonRepo;
            _configuration = configuration;
            _messageService = MessageService;
            _pushNotificationsQueue = pushNotificationsQueue;
            _moduleGroupAccess = ModuleGroupAccess;
            _moduleGroupMaster = ModuleGroupMaster;
            _subModuleFormMaster = SubModuleFormMaster;
            _moduleCRUDOptMaster = ModuleCRUDOptMaster;
            _moduleDomainMaster = ModuleDomainMaster;
            _moduleUtilityMaster = ModuleUtilityMaster;
            _SubModuleRepository = SubModuleRepository;
            _moduleTypeMaster = moduleTypeMaster;
            _fieldMaster = fieldMaster;
            _moduleVisibilityMaster = moduleVisibilityMaster;
            _moduleFieldRequirerMaster = moduleFieldRequirerMaster;
            _moduleAccessRightsMaster = moduleAccessRightsMaster;
            _IsignupLogService = IsignupLogService;
            _registerTypeService = registerTypeService;
            _userdata = userdata;
        }

        #region Role Methods

        public async Task<BizResponseClass> CreateRole(CreateRoleReq Req, long UserId)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                var IsExist = await _roleManager.RoleExistsAsync(Req.RoleName);
                var dt = await _roleManager.FindByNameAsync(Req.RoleName);
                if (IsExist == true)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.Alredy_Exist;
                    Resp.ErrorCode = enErrorCode.RecordAlreadyExist;
                }
                else
                {
                    ApplicationRole NewObj = new ApplicationRole
                    {
                        Name = Req.RoleName,
                        Description = Req.RoleDescription,
                        //PermissionGroupID = Req.PermissionGroupID,
                        CreatedBy = UserId,
                        CreatedDate = UTC_To_IST(),
                        Status = Convert.ToInt16(Req.Status)
                    };
                    await _roleManager.CreateAsync(NewObj);

                    RoleHistory Newobj = new RoleHistory
                    {
                        CreatedBy = UserId,
                        CreatedDate = UTC_To_IST(),
                        ModificationDetail = "New Role : " + Req.RoleName + " Created",
                        UpdatedDate = Helpers.UTC_To_IST(),
                        Status = Convert.ToInt16(Req.Status),
                        UserId = UserId,
                        Module = EnModuleType.Role
                    };
                    await _RoleHistorycommonRepo.AddAsync(Newobj);

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordCreated;
                    Resp.ErrorCode = enErrorCode.RecordCreatedSuccessfully;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateRole", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> UpdateRoleDetail(CreateRoleReq Req, long RoleId, long UserId)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                var IsExist = await _roleManager.FindByIdAsync(RoleId.ToString());
                //var IsExist = await _roleManager.FindByNameAsync(Req.RoleName);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.RoleNotExist;
                    return Resp;
                }
                if (IsExist.Status != Convert.ToInt16(ServiceStatus.Active))
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.RoleIsDisabled;
                    Resp.ErrorCode = enErrorCode.RoleIsDisabled;
                    return Resp;
                }
                var IsDuplicate = await _roleManager.FindByNameAsync(Req.RoleName);
                if (IsDuplicate != null && IsDuplicate.Id != RoleId)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.RoleNameAlreadyExist;
                    Resp.ErrorCode = enErrorCode.RoleNameAlreadyExist;
                    return Resp;
                }
                IsExist.Name = Req.RoleName;
                IsExist.Description = Req.RoleDescription;
                IsExist.Status = Convert.ToInt16(Req.Status);
                //IsExist.PermissionGroupID = Req.PermissionGroupID;
                IsExist.UpdatedBy = UserId;
                IsExist.UpdatedDate = UTC_To_IST();
                await _roleManager.UpdateAsync(IsExist);

                //RoleHistory Newobj = new RoleHistory()
                //{
                //    CreatedBy = UserId,
                //    CreatedDate = UTC_To_IST(),
                //    ModificationDetail = "New Role : " + Req.RoleName + " Created",
                //    UpdatedDate = Helpers.UTC_To_IST(),
                //    Status = 1,
                //    UserId = UserId,
                //    Module = EnModuleType.Role
                //};
                //await _RoleHistorycommonRepo.AddAsync(Newobj);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateRoleDetail", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> ChangeUserRoleStatus(long RoleId, ServiceStatus Status, long UserId)
        {
            //CancellationToken cancellationToken = new CancellationToken();//komal 03 May 2019, Cleanup
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                if (RoleId <= 0)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InvalidRoleId;
                    Resp.ErrorCode = enErrorCode.RoleNotExist;
                    return Resp;
                }
                var IsExist = await _roleManager.FindByIdAsync(RoleId.ToString());
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.RoleNotExist;
                    return Resp;
                }
                IsExist.Status = Convert.ToInt16(Status);
                IsExist.UpdatedBy = UserId;
                IsExist.UpdatedDate = UTC_To_IST();
                await _roleManager.UpdateAsync(IsExist);

                //List<ApplicationUser> AssociatedUsers = _controlPanelRepository.GetAssociatedUser(RoleId);
                //foreach (var i in AssociatedUsers)
                //{
                //    await _userManager.RemoveFromRoleAsync(i, IsExist.Name);
                //}
                //List<IdentityUserRole<int>> UserRoles = _dbContext.UserRoles.Where(u => u.RoleId == Convert.ToInt32(RoleId)).ToList();

                //khushali 22-02-2019   Remove Role from Associated Users 
                var Users = await _userManager.GetUsersInRoleAsync(IsExist.Name);
                foreach (var User in Users)
                {
                    await _userManager.RemoveFromRoleAsync(User, IsExist.Name);
                    // Call disable service for users 
                }
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ChangeRoleStatus", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<GetRoleDetail22> GetRoleDetail(long roleId)
        {
            try
            {
                GetRoleDetail22 Resp = new GetRoleDetail22();
                var data = await _roleManager.FindByIdAsync(roleId.ToString());
                if (data != null)
                {
                    Resp.RoleDetail = new GetRoleDetailRes
                    {
                        RoleID = data.Id,
                        RoleName = data.Name,
                        RoleDescription = data.Description,
                        Status = data.Status
                    };

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.RecordFoundSuccessfully;

                    return Resp;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;

                    return Resp;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetRoleDetail", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<ListRoleDetail> ListRoleDetail(int PageNo, int PageSize, short? Status, short? AllRecords)
        {
            try
            {
                ListRoleDetail Resp = new ListRoleDetail();
                var data = _controlPanelRepository.ListRoleDetails(Status);
                if (data.Count > 0)
                {
                    Resp.TotalCount = data.Count();
                    Resp.PageNo = PageNo - 1;
                    Resp.PageSize = PageSize;
                    Resp.Details = data;
                    if (AllRecords == null)
                    {
                        if (PageNo > 0)
                        {
                            int skip = PageSize * (PageNo - 1);
                            Resp.Details = Resp.Details.Skip(skip).Take(PageSize).ToList();
                        }
                    }
                    else
                    {
                        Resp.Details = Resp.Details.OrderBy(i => i.RoleName).ToList();
                    }
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListRoleDetail", "RoleManagementServices", ex);
                throw ex;
            }

        }

        public async Task<BizResponseClass> AssignRoleDetail(long roleId, long userId, long Createdby, bool IsRemoveRole = false)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var roledata = await _roleManager.FindByIdAsync(roleId.ToString());
                var userdata = await _userManager.FindByIdAsync(userId.ToString());
                if (roledata == null || userdata == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.UserOrRoleNotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                    return Resp;
                }
                if (roledata.Status != Convert.ToInt16(ServiceStatus.Active))
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.RoleIsDisable;
                    Resp.ErrorCode = enErrorCode.YouCanNotAssignDisabledRole;
                    return Resp;
                }
                if (userdata.Status != Convert.ToInt16(ServiceStatus.Active))
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.UserIsDisable;
                    Resp.ErrorCode = enErrorCode.YouCanNotAssignDisabledUser;
                    return Resp;
                }
                //var singlerole = _controlPanelRepository.CheckUserExistWithAnyRole(userdata.Id);
                //if (singlerole == true)
                //{
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.MultipleRoleNotAllow;
                //    Resp.ErrorCode = enErrorCode.MultipleRoleNotAllow;
                //    return Resp;
                //}
                if (IsRemoveRole)
                {
                    var RoleList = await _userManager.GetRolesAsync(userdata);
                    await _userManager.RemoveFromRolesAsync(userdata, RoleList);
                }
                else
                {
                    var IsExist = await _userManager.IsInRoleAsync(userdata, roledata.Name.ToString());
                    if (IsExist == true)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.UserWithRoleAlreadyExist;
                        Resp.ErrorCode = enErrorCode.UserWithRoleAlreadyExist;
                        return Resp;
                    }
                }

                var roleAddResult = await _userManager.AddToRoleAsync(userdata, roledata.Name.ToString());
                if (roleAddResult.Succeeded)
                {
                    RoleHistory Newobj = new RoleHistory
                    {
                        CreatedBy = Createdby,
                        CreatedDate = Helpers.UTC_To_IST(),
                        ModificationDetail = "New User Added For : " + roledata.Name.ToString() + " Role",
                        UpdatedDate = Helpers.UTC_To_IST(),
                        Status = 1,
                        UserId = userdata.Id,
                        Module = EnModuleType.Role
                    };
                    await _RoleHistorycommonRepo.AddAsync(Newobj);
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                    Resp.ErrorCode = enErrorCode.RecordCreatedSuccessfully;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InternalError;
                    Resp.ErrorCode = enErrorCode.InternalError;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AssignRoleDetail", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<ListRoleHistoryData> GetRoleHistory(int PageNo, int PageSize, long? UserId, DateTime? FromDate, DateTime? ToDate, long? ModuleId, short? Status)
        {
            ListRoleHistoryData Resp = new ListRoleHistoryData();
            try
            {
                Resp.Data = _controlPanelRepository.GetRoleHistoryData(UserId, FromDate, ToDate, ModuleId, Status);
                if (Resp.Data.Count > 0)
                {
                    Resp.TotalCount = Resp.Data.Count();
                    Resp.PageNo = PageNo - 1;
                    Resp.PageSize = PageSize;

                    if (PageNo > 0)
                    {
                        int skip = PageSize * (PageNo - 1);
                        Resp.Data = Resp.Data.Skip(skip).Take(PageSize).ToList();
                    }

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetRoleHistory", "RoleManagementServices", ex);
                throw ex;
            }
        }

        // khushali 04-03-2019  -- View Unassigned Users
        public async Task<ListViewUnAssignedUserRes> ViewUnassignedUsers(int PageNo, int PageSize, string UserName, DateTime? FromDate, DateTime? ToDate, short? Status)
        {
            ListViewUnAssignedUserRes Response = new ListViewUnAssignedUserRes();
            try
            {
                Response.Result = _controlPanelRepository.ViewUnassignedUsers(UserName, FromDate, ToDate, Status);
                Response.TotalCount = Response.Result.Count();
                if (PageNo > 0)
                {
                    int skip = PageSize * (PageNo - 1);
                    Response.Result = Response.Result.Skip(skip).Take(PageSize).ToList();
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
                return null;
            }
        }


        #endregion

        #region Permission Group Methods

        public async Task<BizResponseClass> CreatePermissionGroup(CreatePermissionGrpReq req, long userid)
        {
            BizResponseClass Resp = new BizResponseClass();
            //ApplicationRole RoleExist = null;
            //bool Flag = false;
            try
            {
                var IsExist = await _PermissionGroupMasterCommonRepo.GetSingleAsync(i => i.GroupName == req.GroupName);
                if (IsExist != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.Already_Exsits_Group;
                    Resp.ErrorCode = enErrorCode.Status12041Already_Exsits_Group;
                    return Resp;
                }
                #region Old Code
                //if (req.AccessRightId == 0)
                //{
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.InvalidAccessRightOrRoleID;
                //    Resp.ErrorCode = enErrorCode.InvalidAccessRightOrRoleId;
                //    return Resp;
                //}

                //var RightExist = _UserAccessRights.GetActiveById(req.AccessRightId);

                //if (RightExist != null)
                //{
                //if (req.LinkedRoleId != 0)
                //{
                //    RoleExist = await _roleManager.FindByIdAsync(req.LinkedRoleId.ToString());
                //    if (RoleExist == null)
                //    {
                //        Resp.ReturnCode = enResponseCode.Fail;
                //        Resp.ReturnMsg = EnResponseMessage.InvalidAccessRightOrRoleID;
                //        Resp.ErrorCode = enErrorCode.InvalidAccessRightOrRoleId;
                //        return Resp;
                //    }
                //var DuplicateRole = _controlPanelRepository.IsWithDuplicateRole(RoleExist.Id);
                //if (DuplicateRole != null && DuplicateRole.Count > 0)
                //{
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.DuplicateGroupRole;
                //    Resp.ErrorCode = enErrorCode.RecordAlreadyExist;
                //    return Resp;
                //}
                //    Flag = true;
                //}
                //    var DuplicateRole = _controlPanelRepository.IsWithDuplicateRole(RoleExist.Id);
                //    if (DuplicateRole != null && DuplicateRole.Count > 0)
                //    {
                //        Resp.ReturnCode = enResponseCode.Fail;
                //        Resp.ReturnMsg = EnResponseMessage.DuplicateGroupRole;
                //        Resp.ErrorCode = enErrorCode.RecordAlreadyExist;
                //        return Resp;
                //    }
                //    PermissionGroupMaster NewObj = new PermissionGroupMaster
                //    {
                //        //AccessRightId = req.AccessRightId,
                //        GroupDescription = req.Description,
                //        GroupName = req.GroupName,
                //        //LinkedRoles = req.LinkedRoleId,
                //        IPAddress = req.IPAddress,
                //        CreatedBy = userid,
                //        CreatedDate = UTC_To_IST(),
                //        Status = Convert.ToInt16(req.Status)
                //    };
                //    await _PermissionGroupMasterCommonRepo.AddAsync(NewObj);
                //    long id = NewObj.Id;

                //    req.Data.GroupID = id;
                //    await _ruleManageService.CreateAssignedRuleV2(req.Data);
                //    Resp.ReturnCode = enResponseCode.Success;
                //    Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                //    Resp.ErrorCode = enErrorCode.RecordCreatedSuccessfully;
                //    return Resp;
                //}
                //else
                //{
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.NotFound;
                //    Resp.ErrorCode = enErrorCode.NoRecordFound;
                //    return Resp;
                //}

                //var DuplicateRole = _controlPanelRepository.IsWithDuplicateRole(RoleExist.Id);
                //if (DuplicateRole != null && DuplicateRole.Count > 0)
                //{
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.DuplicateGroupRole;
                //    Resp.ErrorCode = enErrorCode.RecordAlreadyExist;
                //    return Resp;
                //}
                #endregion
                PermissionGroupMaster NewObj = new PermissionGroupMaster
                {
                    //AccessRightId = req.AccessRightId,
                    GroupDescription = req.Description,
                    GroupName = req.GroupName,
                    //LinkedRoles = req.LinkedRoleId,
                    IPAddress = req.IPAddress,
                    CreatedBy = userid,
                    CreatedDate = UTC_To_IST(),
                    Status = Convert.ToInt16(req.Status)
                };
                if (req.Data != null)
                {
                    if (req.Data.Modules != null)
                    {
                        await _PermissionGroupMasterCommonRepo.AddAsync(NewObj);
                        long id = NewObj.Id;
                        req.Data.GroupID = id;
                        var config = await _ruleManageService.CreateAssignedRuleV2(req.Data);

                        if (config.ErrorCode != enErrorCode.RecordCreatedSuccessfully)
                        {
                            Resp.ReturnCode = config.ReturnCode;
                            Resp.ReturnMsg = config.ReturnMsg;
                            Resp.ErrorCode = config.ErrorCode;
                            return Resp;
                        }
                    }
                    else
                    {
                        return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = "Configure Permissions not Found", ErrorCode = enErrorCode.Status14059ConfigurePermissionsNotFound };
                    }
                }
                else
                {
                    return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = "Configure Permissions not Found", ErrorCode = enErrorCode.Status14059ConfigurePermissionsNotFound };
                }
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                Resp.ErrorCode = enErrorCode.RecordCreatedSuccessfully;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreatePermissionGroup", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> CreatePermissionGroupV1(CreateGroupRequest req, long userid)
        {
            BizResponseClass Resp = new BizResponseClass();
            //ApplicationRole RoleExist = null;
            //bool Flag = false;
            try
            {
                var IsExist = await _moduleGroupMaster.GetSingleAsync(m => m.GroupName == req.GroupName);
                if (IsExist != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.Already_Exsits_Group;
                    Resp.ErrorCode = enErrorCode.Status12041Already_Exsits_Group;
                    return Resp;
                }

                ModuleGroupMaster NewObj = new ModuleGroupMaster
                {
                    GroupDescription = req.Description,
                    GroupName = req.GroupName,
                    RoleID = (int)req.RoleId,
                    ModuleDomainID = req.DomainID,
                    CreatedBy = userid,
                    CreatedDate = UTC_To_IST()
                    //Status = Convert.ToInt16(req.Status)
                };
                await _moduleGroupMaster.AddAsync(NewObj);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                Resp.ErrorCode = enErrorCode.RecordCreatedSuccessfully;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreatePermissionGroupV1", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> UpdatePermissionGroup(long PermissionGrpID, CreatePermissionGrpReq req, long userid)
        {
            BizResponseClass Resp = new BizResponseClass();
            ApplicationRole RoleExist = null;
            try
            {
                var IsExist = await _PermissionGroupMasterCommonRepo.GetSingleAsync(i => i.Id == PermissionGrpID);
                if (IsExist.Status != Convert.ToInt16(ServiceStatus.Active))
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.PermissionGroupIsDisable;
                    Resp.ErrorCode = enErrorCode.PermissionGroupIsDisable;
                    return Resp;
                }
                //if (req.LinkedRoleId <= 0)
                //{
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.RoleIdRequired;
                //    Resp.ErrorCode = enErrorCode.RoleIdRequiredAndCanNotBeZero;
                //    return Resp;
                //}
                if (IsExist != null)
                {
                    //if (req.AccessRightId == 0)
                    //{
                    //    Resp.ReturnCode = enResponseCode.Fail;
                    //    Resp.ReturnMsg = EnResponseMessage.InvalidAccessRightOrRoleID;
                    //    Resp.ErrorCode = enErrorCode.InvalidAccessRightOrRoleId;
                    //    return Resp;
                    //}
                    var duplicatedata = await _PermissionGroupMasterCommonRepo.GetSingleAsync(i => i.Id != PermissionGrpID && i.GroupName == req.GroupName && i.Status == 1);// && i.AccessRightId == req.AccessRightId && i.LinkedRoles == req.LinkedRoleId
                    //var RightExist = _UserAccessRights.GetActiveById(req.AccessRightId);
                    //var duplicatedata = await IsDuplicate;
                    if (duplicatedata != null)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.Already_Exsits_Group;
                        Resp.ErrorCode = enErrorCode.Status12041Already_Exsits_Group;
                        return Resp;
                    }
                    if (req.LinkedRoleId > 0)
                    {
                        RoleExist = await _roleManager.FindByIdAsync(req.LinkedRoleId.ToString());
                        if (RoleExist == null)
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidAccessRightOrRoleID;
                            Resp.ErrorCode = enErrorCode.InvalidAccessRightOrRoleId;
                            return Resp;
                        }
                        var DuplicateRole = _controlPanelRepository.IsWithDuplicateRole(RoleExist.Id).SingleOrDefault();
                        if (DuplicateRole != null)
                        {
                            if (DuplicateRole.PermissionGroupId != req.PermissionGroupID)
                            {
                                Resp.ReturnCode = enResponseCode.Fail;
                                Resp.ReturnMsg = EnResponseMessage.DuplicateGroupRole;
                                Resp.ErrorCode = enErrorCode.RecordAlreadyExist;
                                return Resp;
                            }
                        }
                    }

                    IsExist.GroupName = req.GroupName;
                    IsExist.GroupDescription = req.Description;
                    IsExist.IPAddress = req.IPAddress;
                    //IsExist.LinkedRoles = req.LinkedRoleId;
                    IsExist.UpdatedBy = userid;
                    IsExist.UpdatedDate = UTC_To_IST();
                    IsExist.Status = Convert.ToInt16(req.Status);

                    req.Data.GroupID = IsExist.Id;
                    var ruleresp = await _ruleManageService.UpdateAssignedRule(req.Data);
                    if (ruleresp == null)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.RuleDataUpdationFail;
                        Resp.ErrorCode = enErrorCode.RuleDataUpdationFail;
                        return Resp;
                    }
                    if (ruleresp.ErrorCode != enErrorCode.RecordUpdatedSuccessfully)
                    {
                        Resp.ReturnCode = ruleresp.ReturnCode;
                        Resp.ReturnMsg = ruleresp.ReturnMsg;
                        Resp.ErrorCode = ruleresp.ErrorCode;
                        return Resp;
                    }
                    await _PermissionGroupMasterCommonRepo.UpdateAsync(IsExist);
                    if (req.LinkedRoleId > 0)
                    {
                        _controlPanelRepository.AddGroupRoleMappingData(IsExist.Id, RoleExist.Id);
                    }
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                    Resp.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                    return Resp;
                    //if (RightExist != null)
                    //{
                    //if (req.LinkedRoleId != 0)
                    //{
                    //    RoleExist = await _roleManager.FindByIdAsync(req.LinkedRoleId.ToString());
                    //    if (RoleExist == null)
                    //    {
                    //        Resp.ReturnCode = enResponseCode.Fail;
                    //        Resp.ReturnMsg = EnResponseMessage.InvalidAccessRightOrRoleID;
                    //        Resp.ErrorCode = enErrorCode.InvalidAccessRightOrRoleId;
                    //        return Resp;
                    //    }

                    //}
                    //IsExist.GroupName = req.GroupName;
                    //IsExist.GroupDescription = req.Description;
                    //IsExist.IPAddress = req.IPAddress;
                    //IsExist.LinkedRoles = req.LinkedRoleId;
                    //IsExist.UpdatedBy = userid;
                    //IsExist.UpdatedDate = UTC_To_IST();
                    //IsExist.Status = Convert.ToInt16(req.Status);                        

                    //req.Data.GroupID = IsExist.Id;
                    //var ruleresp = await _ruleManageService.UpdateAssignedRule(req.Data);
                    //if(ruleresp == null)
                    //{
                    //    Resp.ReturnCode = enResponseCode.Fail;
                    //    Resp.ReturnMsg = EnResponseMessage.RuleDataUpdationFail;
                    //    Resp.ErrorCode = enErrorCode.RuleDataUpdationFail;
                    //    return Resp;
                    //}
                    //if(ruleresp.ErrorCode != enErrorCode.RecordUpdatedSuccessfully)
                    //{
                    //    Resp.ReturnCode = ruleresp.ReturnCode;
                    //    Resp.ReturnMsg = ruleresp.ReturnMsg;
                    //    Resp.ErrorCode = ruleresp.ErrorCode;
                    //    return Resp;
                    //}
                    //await _PermissionGroupMasterCommonRepo.UpdateAsync(IsExist);
                    //Resp.ReturnCode = enResponseCode.Success;
                    //Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                    //Resp.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                    //return Resp;
                    //}
                    //else
                    //{
                    //    Resp.ReturnCode = enResponseCode.Fail;
                    //    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    //    Resp.ErrorCode = enErrorCode.NoRecordFound;
                    //    return Resp;
                    //}
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                    return Resp;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdatePermissionGroup", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> UpdatePermissionGroupV1(CreateGroupRequest req, long userid)
        {
            BizResponseClass Resp = new BizResponseClass();
            //ApplicationRole RoleExist = null;//komal 03 May 2019, Cleanup
            try
            {
                var IsExist = await _moduleGroupMaster.GetSingleAsync(i => i.GroupName == req.GroupName);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                    return Resp;
                }

                if (IsExist == null || IsExist.Status != Convert.ToInt16(ServiceStatus.Active))
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.PermissionGroupIsDisable;
                    Resp.ErrorCode = enErrorCode.PermissionGroupIsDisable;
                    return Resp;
                }

                IsExist.GroupName = req.GroupName;
                IsExist.GroupDescription = req.Description;
                IsExist.RoleID = (int)req.RoleId;
                IsExist.UpdatedBy = userid;
                IsExist.UpdatedDate = UTC_To_IST();
                //IsExist.Status = Convert.ToInt16(req.Status);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdatePermissionGroupV1", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> ChangePermissionGroupStatus(long PermissionGrpID, ServiceStatus Status, long UserId)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                //var da = await ListPermissionGroup();
                var IsExist = await _PermissionGroupMasterCommonRepo.GetSingleAsync(i => i.Id == PermissionGrpID);
                if (Status == ServiceStatus.Disable)
                {
                    var IsInUse = _controlPanelRepository.CheckGroupInUse(PermissionGrpID);
                    if (IsInUse.Count > 0)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.PermissionGroupIsAssignToGroup;
                        Resp.ErrorCode = enErrorCode.PermissionGroupIsAssignToGroup;
                        return Resp;
                    }
                }
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                    return Resp;
                }
                else
                {
                    IsExist.Status = Convert.ToInt16(Status);
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = UTC_To_IST();
                    _PermissionGroupMasterCommonRepo.UpdateWithAuditLog(IsExist);
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                    Resp.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                    return Resp;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ChangePermissionGroupStatus", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<ListPermissionGrp> ListPermissionGroup(int PageNo, int PageSize, short? AllRecords, DateTime? FromDate, DateTime? Todate, long? RoleId, short? Status)
        {
            ListPermissionGrp Resp = new ListPermissionGrp();
            try
            {
                Resp.Data = _controlPanelRepository.ListPermissionGrpDetail(FromDate, Todate, RoleId, Status);
                if (Resp.Data.Count > 0)
                {
                    Resp.TotalCount = Resp.Data.Count();
                    Resp.PageNo = PageNo - 1;
                    Resp.PageSize = PageSize;
                    if (AllRecords == null)
                    {
                        if (PageNo > 0)
                        {
                            int skip = PageSize * (PageNo - 1);
                            Resp.Data = Resp.Data.Skip(skip).Take(PageSize).ToList();
                        }
                    }
                    else
                    {
                        Resp.Data = Resp.Data.OrderBy(i => i.GroupName).ToList();
                    }
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListPermissionGroup", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<GetPermissionGroupRes> GetPermissionGroupDetailById(long permissionGroupId)
        {
            try
            {
                GetPermissionGroupRes Resp = new GetPermissionGroupRes();

                var Data = _controlPanelRepository.GetPermissionGrpDetail(permissionGroupId);
                if (Data != null)
                {
                    Resp.Data = new GetPermissionGroup2();
                    Resp.Data = Data;
                    //Resp.Id = Data.Id;
                    //Resp.GroupName = Data.GroupName;
                    //Resp.GroupDescription = Data.GroupDescription;
                    //Resp.IPAddress = Data.IPAddress;
                    //Resp.LinkedRole = Data.LinkedRole;
                    //Resp.CreatedDate = Data.CreatedDate;
                    //Resp.Status = Data.Status;


                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetPermissionGroupDetailById", "RoleManagementServices", ex);
                throw ex;
            }
        }

        // khushali 04-03-2019  -- Get permission Group ID by Linked Role
        public async Task<GetPermissionGroupIDByLinkedRoleRes> GetPermissionGroupIDByLinkedRole(long RoleId)
        {
            GetPermissionGroupIDByLinkedRoleRes Resp = new GetPermissionGroupIDByLinkedRoleRes();
            try
            {
                ApplicationGroupRoles GroupRole = _controlPanelRepository.GetPermissionGroupIDByLinkedRole(RoleId).FirstOrDefault();
                PermissionGroupMaster IsExist = await _PermissionGroupMasterCommonRepo.GetSingleAsync(i => i.Id == GroupRole.PermissionGroupId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NoPermissionGroupByLinkedRole;
                    Resp.ErrorCode = enErrorCode.NoPermissionGroupByLinkedRole;
                    return Resp;
                }
                else
                {
                    if (IsExist.Status == Convert.ToInt16(ServiceStatus.Active))
                    {
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.FindRecored;
                        Resp.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                        Resp.GroupID = IsExist.Id;
                        return Resp;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.PermissionGroupNotActive;
                        Resp.ErrorCode = enErrorCode.PermissionGroupNotActive;
                        return Resp;
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ChangePermissionGroupStatus", "RoleManagementServices", ex);
                throw ex;
                //return null;
            }
        }

        #endregion

        #region User Management

        public async Task<BizResponseClass> CreateNewUser(CreateUserReq req, long userid)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsEmailExist = await _userManager.FindByEmailAsync(req.Email);
                if (IsEmailExist != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.SignUpTempUserEmailExist;
                    Resp.ErrorCode = enErrorCode.EmailAlreadyExistWithUser;
                    return Resp;
                }
                var IsUserNameExist = await _userManager.FindByNameAsync(req.Username);
                if (IsUserNameExist != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.SignUpTempUserNameExist;
                    Resp.ErrorCode = enErrorCode.UserNameAlreadyExist;
                    return Resp;
                }
                //var IsMobileExist = await _userManager.FindByNameAsync(req.Mobile);
                //if (IsMobileExist != null)
                //{
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.SignUPMobileValidation;
                //    Resp.ErrorCode = enErrorCode.MobileNoAlreadyExist;
                //    return Resp;
                //}
                var IsMobileExist = await _userdata.FindByMobileNumber(req.Mobile);
                if (IsMobileExist != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.SignUPMobileValidation;
                    Resp.ErrorCode = enErrorCode.MobileNoAlreadyExist;
                    return Resp;
                }
                byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                ApplicationUser NewUser = new ApplicationUser()
                {
                    Email = req.Email,
                    FirstName = req.Firstname,
                    LastName = req.Lastname,
                    IsEnabled = true,
                    Mobile = req.Mobile,
                    UserName = req.Username,
                    Status = Convert.ToInt16(EnMyAccountUserStatus.UnConfirmed),
                    IsCreatedByAdmin = Convert.ToInt16(req.IsAdmin),
                    GroupID = req.GroupID, // Added by Nishit Jani on 2019-05-16 4:42 PM
                    //PasswordHash = req.Password, // Removed as UserManager it self manages it. -Nishit Jani on A 2019-04-01 3:35 PM
                    CreatedDate = UTC_To_IST()
                };
                var user = await _userManager.CreateAsync(NewUser, req.Password);
                if (user.Succeeded)
                {
                    var Currentuser = await _userManager.FindByNameAsync(req.Username);

                    //add- mansi 30-07-2019
                    #region add record in SignUpLog table 
                    var SignUplog = new SignUpLogViewModel()
                    {
                        TempUserId = Convert.ToInt32(Currentuser.Id),
                        RegisterType = await _registerTypeService.GetRegisterId(enRegisterType.Standerd),
                        Device = string.Empty,
                        Mode = "web",
                        IpAddress = string.Empty,
                        Location = string.Empty,
                        HostName = string.Empty,
                        RegisterStatus = false,
                    };
                    _IsignupLogService.AddSignUpLog(SignUplog);
                    #endregion

                    //Rushabh 21-08-2019
                    #region Add Into Role History

                    RoleHistory Newobj = new RoleHistory
                    {
                        CreatedBy = userid,//id of the log in user.
                        CreatedDate = Helpers.UTC_To_IST(),
                        ModificationDetail = "New User Created : " + Currentuser.FirstName + " " + Currentuser.LastName,
                        UpdatedDate = Helpers.UTC_To_IST(),
                        Status = 1,
                        UserId = Currentuser.Id,//Id of the user which is created.
                        Module = EnModuleType.Users
                    };
                    await _RoleHistorycommonRepo.AddAsync(Newobj);

                    #endregion

                    #region Email Integration Code                    

                    EmailLinkTokenViewModel linkToken = new EmailLinkTokenViewModel();
                    linkToken.Id = Currentuser.Id;
                    linkToken.Username = req.Username;
                    linkToken.Email = req.Email;
                    //linkToken.Firstname = req.Firstname;
                    //linkToken.Lastname = req.Lastname;
                    //linkToken.Mobile = req.Mobile;
                    //linkToken.CurrentTime = DateTime.UtcNow;
                    //linkToken.Expirytime = DateTime.UtcNow + TimeSpan.FromHours(2);
                    linkToken.CurrentTime = UTC_To_IST();
                    linkToken.Expirytime = UTC_To_IST() + TimeSpan.FromHours(2);
                    //linkToken.CountryCode = model.CountryCode;
                    //linkToken.Device = DeviceDetails != null ? DeviceDetails[0] : string.Empty;
                    //linkToken.DeviceOS = DeviceDetails != null ? DeviceDetails[1] : string.Empty;
                    //linkToken.DeviceID = DeviceDetails != null ? DeviceDetails[2] : string.Empty;
                    //linkToken.IpAddress = req.IPAddress;


                    string UserDetails = JsonConvert.SerializeObject(linkToken);
                    string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);



                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
                    //_configuration["InviteMailURL"].ToString()
                    string ctokenlink = _configuration["InviteMailURL"].ToString() + Convert.ToBase64String(plainTextBytes);

                    // khushali 30-01-2019 for Common Template Method call 
                    TemplateMasterData TemplateData = new TemplateMasterData();
                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    SendEmailRequest request = new SendEmailRequest();
                    if (!string.IsNullOrEmpty(Currentuser.UserName))
                        communicationParamater.Param1 = Currentuser.UserName;
                    else
                        communicationParamater.Param1 = string.Empty;
                    communicationParamater.Param2 = ctokenlink;
                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_SendInvitation, communicationParamater, enCommunicationServiceType.Email).Result;
                    if (TemplateData != null)
                    {
                        if (TemplateData.IsOnOff == 1)
                        {
                            request.Recepient = req.Email;
                            request.Body = TemplateData.Content;
                            request.Subject = TemplateData.AdditionalInfo;
                            _pushNotificationsQueue.Enqueue(request);
                        }
                    }

                    #endregion

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordCreated;
                    Resp.ErrorCode = enErrorCode.RecordCreatedSuccessfully;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InternalError;
                    Resp.ErrorCode = enErrorCode.InternalError;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateNewUser", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> EditUserDetail(EditUserReq req, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            
            try
            {
                if (req.UserId == 0)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InvalidUserId;
                    Resp.ErrorCode = enErrorCode.UserIdRequiredAndCanNotBeZero;
                    return Resp;
                }
                var IsExist = await _userManager.FindByIdAsync(req.UserId.ToString());
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                    return Resp;
                }
                //|| IsExist.IsEnabled == false || IsExist.IsBlocked == true
                if (IsExist.Status != Convert.ToInt16(ServiceStatus.Active))
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.CanNotUpdateDisableUser;
                    Resp.ErrorCode = enErrorCode.YouCanNotUpdateDisableUser;
                    return Resp;
                }
                //Rushabh 22-08-2019 Add Into Role History
                bool InsertIntoRoleHistory = req.IsAdmin != IsExist.IsCreatedByAdmin ? true : false;
                IsExist.FirstName = req.Firstname;
                IsExist.LastName = req.Lastname;
                IsExist.Status = Convert.ToInt16(req.Status);
                IsExist.GroupID = req.GroupID;
                IsExist.IsCreatedByAdmin = Convert.ToInt16(req.IsAdmin);
                //mansi 11-09-2019 set Lockoutend bit
                if (req.IsLockOut == 0)
                {
                    IsExist.LockoutEnd = null;
                }
                else
                {
                    // IsExist.LockoutEnd = DateTime.UtcNow.AddHours(Convert.ToInt64(_configuration["DefaultLockoutTimeSpan"]));
                    IsExist.LockoutEnd = DateTime.UtcNow.AddHours(6);
                }

                IsExist.TwoFactorEnabled = Convert.ToBoolean(req.TwoFactorEnabled);
                await _userManager.UpdateAsync(IsExist);

                if(InsertIntoRoleHistory)
                {
                    RoleHistory Newobj = new RoleHistory
                    {
                        CreatedBy = id,//id of the log in user.
                        CreatedDate = Helpers.UTC_To_IST(),
                        ModificationDetail = req.IsAdmin == 1 ? "User : " + IsExist.FirstName + " " + IsExist.LastName + " Added As Admin" : "User : " + IsExist.FirstName + " " + IsExist.LastName + " Removed From Admin",
                        UpdatedDate = Helpers.UTC_To_IST(),
                        Status = 1,
                        UserId = IsExist.Id,//Id of the user which is edited.
                        Module = EnModuleType.Users
                    };
                    await _RoleHistorycommonRepo.AddAsync(Newobj);
                }               

                //var roledata = await _roleManager.FindByIdAsync(req.RoleId.ToString());
                //if (roledata == null)
                //{
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.UserOrRoleNotFound;
                //    Resp.ErrorCode = enErrorCode.NoRecordFound;
                //    return Resp;
                //}
                //var roleAddResult = await _userManager.AddToRoleAsync(IsExist, roledata.Name.ToString());

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;

                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("EditUserDetail", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> ChangeUserStatus(long userId, EnMyAccountUserStatus status)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _userManager.FindByIdAsync(userId.ToString());
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                    return Resp;
                }
                if (status == EnMyAccountUserStatus.Active || status == EnMyAccountUserStatus.InActive)
                {
                    IsExist.IsEnabled = Convert.ToBoolean(status == EnMyAccountUserStatus.Active ? true : false);
                }
                if (status == EnMyAccountUserStatus.Blocked)
                {
                    IsExist.IsBlocked = true;
                }
                IsExist.Status = Convert.ToInt16(status);
                await _userManager.UpdateAsync(IsExist);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;

                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ChangeUserStatus", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<ListUserDetails> ListUserDetail(int PageNo, int PageSize, short? AllRecords, string Username, string Mobile = null, string Email = null, DateTime? FromDate = null, DateTime? ToDate = null, int IsAdmin = 9)//changes for filtration by mansi 02-09-2019
        {
            ListUserDetails Resp = new ListUserDetails();
            try
            {
                //var data = _controlPanelRepository.GetUserListData();
                var data = _controlPanelRepository.GetUserListData(Username, Mobile, Email, FromDate, ToDate, IsAdmin);
                Resp.Data = new List<GetUserDetail>();

                if (data.Count > 0)
                {
                    Resp.TotalCount = data.Count();
                    Resp.PageNo = PageNo - 1;
                    Resp.PageSize = PageSize;
                    Resp.Data = data;
                    if (AllRecords == null)
                    {
                        if (PageNo > 0)
                        {
                            int skip = PageSize * (PageNo - 1);
                            Resp.Data = Resp.Data.Skip(skip).Take(PageSize).ToList();
                        }
                    }
                    else
                    {
                        Resp.Data = Resp.Data.OrderBy(i => i.UserName).ToList();
                    }
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListUserDetail", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<GetUserDetailResp> GetUserDetailById(long UserId)
        {
            GetUserDetailResp Resp = new GetUserDetailResp();
            try
            {
                var data = _controlPanelRepository.GetUserDataById(UserId);
                // Chnaged method as default method is already available. -Nishit Jani on A 2019-04-01 3:46 PM
                //var data = await _userManager.FindByIdAsync(UserId.ToString());
                if (data != null)
                {
                    Resp.Data = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetUserDetailById", "RoleManagementServices", ex);
                throw ex;
            }
        }

        public async Task<ListUserDetails2> SearchUserDetail(string searchText)
        {
            ListUserDetails2 Resp = new ListUserDetails2();
            try
            {
                var data = _controlPanelRepository.SearchUserData(searchText);
                if (data != null && data.Count > 0)
                {
                    Resp.Data = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoRecordFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SearchUserDetail", "RoleManagementServices", ex);
                throw ex;
            }
        }

        #endregion

        #region "Get Menu Access List of User by their Group -Nishit Jani on A 2019-03-28 12:46 PM
        public async Task<MenuAccessResponse> GetMenuWithDetailsAsync(long GroupID)
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            Response.Result = new MenuAccessDetailViewModel();
            try
            {
                List<ModuleGroupAccess> moduleAccessList = await _moduleGroupAccess.GetAll(GroupID);
                if (moduleAccessList != null)
                {
                    if (moduleAccessList.Count > 0)
                    {
                        //Rita 29-3-19 take master entry
                        IEnumerable<ModuleTypeMaster> ModuleTypeMasterList = _moduleTypeMaster.FindBy(e => e.Status == 1);
                        IEnumerable<ModuleCRUDOptMaster> CRUDOptMasterList = _moduleCRUDOptMaster.FindBy(e => e.Status == 1);
                        IEnumerable<ModuleUtilityMaster> UtilityMasterList = _moduleUtilityMaster.FindBy(e => e.Status == 1);
                        IEnumerable<SubModuleMaster> SubModuleMasterList = _SubModuleRepository.FindBy(e => e.Status == 1);
                        IEnumerable<SubModuleFormMaster> SubModuleFormMasterList = _subModuleFormMaster.FindBy(e => e.Status == 1);

                        // Need to change with actual status code and error code. -Nishit Jani on A 2019-03-28 6:51 PM
                        Response.ErrorCode = 0;
                        Response.ReturnCode = 0;
                        Response.ReturnMsg = "Got records of main Menu";
                        Response.Result.GroupID = GroupID;
                        //Response.Result.ID = GroupID;

                        Response.Result.Modules = new List<MenuSubDetailViewModel>();
                        foreach (var modeulAccessListData in moduleAccessList)
                        {
                            Response.Result.ID = modeulAccessListData.SubModuleID;

                            //SubModuleMaster subModuleMaster = await _SubModuleRepository.GetByIdAsync(Response.Result.ID); //komal 29-03-2019 remove query
                            SubModuleMaster subModuleMaster = SubModuleMasterList.Where(e => e.Id == Response.Result.ID).FirstOrDefault();
                            if (subModuleMaster != null)
                            {
                                MenuSubDetailViewModel menuSubDetailViewModel = new MenuSubDetailViewModel();

                                // Below is SubModuleMaster table's data.
                                menuSubDetailViewModel.ID = subModuleMaster.Id;
                                menuSubDetailViewModel.Status = subModuleMaster.Status;
                                menuSubDetailViewModel.ModuleName = subModuleMaster.SubModuleName;
                                menuSubDetailViewModel.ModuelID = subModuleMaster.ModuleID;
                                menuSubDetailViewModel.ParentID = subModuleMaster.ParentID;
                                menuSubDetailViewModel.GUID = subModuleMaster.GUID.ToString();
                                menuSubDetailViewModel.ParentGUID = subModuleMaster.ParentGUID.ToString();
                                menuSubDetailViewModel.Type = ModuleTypeMasterList.Where(e => e.Id == Convert.ToInt64(subModuleMaster.Type)).FirstOrDefault().TypeValue;//subModuleMaster.Type.ToString();

                                //menuSubDetailViewModel.ModuleDomainType = subModuleMaster.ModuleDomainType;

                                // Below is data of SubModuleFormMaster for CRUD operation etc.                           
                                string[] crudTypes = modeulAccessListData.CrudTypes.Split(",");
                                menuSubDetailViewModel.CrudOption = new List<string>();
                                //for (int i = 0; i < crudTypes.Length; i++)
                                //{
                                //   menuSubDetailViewModel.CrudOption.Add(crudTypes[i]);

                                //}
                                //Rita 29-3-19 remove i loop
                                foreach (string CrudOpt in crudTypes)
                                {
                                    menuSubDetailViewModel.CrudOption.Add(CRUDOptMasterList.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                                }

                                string[] utilityTypes = modeulAccessListData.CrudTypes.Split(",");
                                menuSubDetailViewModel.Utility = new List<string>();
                                //for (int j = 0; j < utilityTypes.Length; j++)
                                //{
                                //    menuSubDetailViewModel.Utility.Add(utilityTypes[j]);
                                //}
                                //Rita 29-3-19 remove i loop
                                foreach (string utilityType in utilityTypes)
                                {
                                    menuSubDetailViewModel.Utility.Add(UtilityMasterList.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                                }

                                //List<SubModuleMaster> subModuleMasterChild = await _SubModuleRepository.ListAsync(new GetChildNode(menuSubDetailViewModel.ID)); //komal 29-03-2019 remove query
                                List<SubModuleMaster> subModuleMasterChild = SubModuleMasterList.Where(e => e.ParentID == menuSubDetailViewModel.ID).ToList();
                                if (subModuleMasterChild.Count > 0)
                                {
                                    menuSubDetailViewModel.HasChild = true;
                                }
                                menuSubDetailViewModel.ChildNodes = new List<ChildNodeViewModel>();
                                foreach (var subChildNodeData in subModuleMasterChild)
                                {
                                    ChildNodeViewModel childNodeViewModel = new ChildNodeViewModel();
                                    childNodeViewModel.ID = subChildNodeData.Id;
                                    childNodeViewModel.Status = subChildNodeData.Status;
                                    childNodeViewModel.ModuleName = subChildNodeData.SubModuleName;
                                    childNodeViewModel.ModuelID = subChildNodeData.ModuleID;
                                    childNodeViewModel.ParentID = subChildNodeData.ParentID;
                                    childNodeViewModel.GUID = subChildNodeData.GUID.ToString();
                                    childNodeViewModel.ParentGUID = subChildNodeData.ParentGUID.ToString();
                                    childNodeViewModel.Type = ModuleTypeMasterList.Where(e => e.Id == Convert.ToInt64(subChildNodeData.Type)).FirstOrDefault().TypeValue;//subChildNodeData.Type.ToString();
                                                                                                                                                                         //childNodeViewModel.ModuleDomainType = subChildNodeData.ModuleDomainType;


                                    // Below is data of SubModuleFormMaster for CRUD operation etc.                           
                                    string[] crudTypesInner = modeulAccessListData.CrudTypes.Split(",");
                                    childNodeViewModel.CrudOption = new List<string>();
                                    //for (int k = 0; k < crudTypesInner.Length; k++)
                                    //{
                                    //    childNodeViewModel.CrudOption.Add(crudTypesInner[k]);                                    
                                    //}
                                    //Rita 29-3-19 remove i loop
                                    foreach (string CrudOpt in crudTypesInner)
                                    {
                                        childNodeViewModel.CrudOption.Add(CRUDOptMasterList.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                                    }

                                    string[] utilityTypesInner = modeulAccessListData.CrudTypes.Split(",");
                                    childNodeViewModel.Utility = new List<string>();
                                    //for (int l = 0; l < utilityTypesInner.Length; l++)
                                    //{
                                    //    childNodeViewModel.Utility.Add(utilityTypesInner[l]);
                                    //}
                                    //Rita 29-3-19 remove i loop
                                    foreach (string utilityType in utilityTypesInner)
                                    {
                                        childNodeViewModel.Utility.Add(UtilityMasterList.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                                    }

                                    ///
                                    //List<SubModuleMaster> subModuleMasterChildInner = await _SubModuleRepository.ListAsync(new GetChildNode(childNodeViewModel.ID));//komal 29-03-2019 remove query
                                    List<SubModuleMaster> subModuleMasterChildInner = SubModuleMasterList.Where(e => e.ParentID == childNodeViewModel.ID).ToList();
                                    if (subModuleMasterChild.Count > 0)
                                    {
                                        childNodeViewModel.HasChild = true;
                                    }
                                    childNodeViewModel.ChildNodes = new List<InnerChildNodeViewModel>();
                                    foreach (var subChildNodeInnerData in subModuleMasterChildInner)
                                    {
                                        InnerChildNodeViewModel childNodeInnerViewModel = new InnerChildNodeViewModel();
                                        childNodeInnerViewModel.ID = subChildNodeInnerData.Id;
                                        childNodeInnerViewModel.Status = subChildNodeInnerData.Status;
                                        childNodeInnerViewModel.SubModuleName = subChildNodeInnerData.SubModuleName;
                                        childNodeInnerViewModel.SubModuleID = subChildNodeInnerData.ModuleID;
                                        childNodeInnerViewModel.ParentID = subChildNodeInnerData.ParentID;
                                        childNodeInnerViewModel.GUID = subChildNodeInnerData.GUID.ToString();
                                        childNodeInnerViewModel.ParentGUID = subChildNodeInnerData.ParentGUID.ToString();
                                        childNodeInnerViewModel.Type = ModuleTypeMasterList.Where(e => e.Id == Convert.ToInt64(subChildNodeInnerData.Type)).FirstOrDefault().TypeValue;//subChildNodeInnerData.Type.ToString();
                                                                                                                                                                                       //childNodeInnerViewModel.ModuleDomainType = subChildNodeInnerData.ModuleDomainType;


                                        // Below is data of SubModuleFormMaster for CRUD operation etc.                           
                                        string[] crudTypesInnerChild = modeulAccessListData.CrudTypes.Split(",");
                                        childNodeInnerViewModel.CrudOption = new List<string>();
                                        //for (int m = 0; m < crudTypesInnerChild.Length; m++)
                                        //{
                                        //    childNodeInnerViewModel.CrudOption.Add(crudTypesInnerChild[m]);
                                        //}
                                        //Rita 29-3-19 remove i loop
                                        foreach (string CrudOpt in crudTypesInnerChild)
                                        {
                                            childNodeInnerViewModel.CrudOption.Add(CRUDOptMasterList.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                                        }

                                        string[] utilityTypesInnerChild = modeulAccessListData.CrudTypes.Split(",");
                                        childNodeInnerViewModel.Utility = new List<string>();
                                        //for (int n = 0; n < utilityTypesInnerChild.Length; n++)
                                        //{
                                        //    childNodeInnerViewModel.Utility.Add(utilityTypesInnerChild[n]);
                                        //}
                                        //Rita 29-3-19 remove i loop
                                        foreach (string utilityType in utilityTypesInnerChild)
                                        {
                                            childNodeInnerViewModel.Utility.Add(UtilityMasterList.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                                        }
                                        List<SubModuleMaster> subModuleMasterChild3rdLevel = SubModuleMasterList.Where(e => e.ParentID == childNodeInnerViewModel.ID).ToList();
                                        if (subModuleMasterChild3rdLevel.Count > 0)
                                        {
                                            childNodeInnerViewModel.HasChild = true;
                                            childNodeInnerViewModel.ChildNodes = new List<InnerChildNodeViewModel>();
                                            foreach (var subChildNode3rdInnerData in subModuleMasterChild3rdLevel)
                                            {
                                                InnerChildNodeViewModel childNode4thLevelInnerViewModel = new InnerChildNodeViewModel();
                                                childNode4thLevelInnerViewModel.ID = subChildNode3rdInnerData.Id;
                                                childNode4thLevelInnerViewModel.Status = subChildNode3rdInnerData.Status;
                                                childNode4thLevelInnerViewModel.SubModuleName = subChildNode3rdInnerData.SubModuleName;
                                                childNode4thLevelInnerViewModel.SubModuleID = subChildNode3rdInnerData.ModuleID;
                                                childNode4thLevelInnerViewModel.ParentID = subChildNode3rdInnerData.ParentID;
                                                childNode4thLevelInnerViewModel.GUID = subChildNode3rdInnerData.GUID.ToString();
                                                childNode4thLevelInnerViewModel.ParentGUID = subChildNode3rdInnerData.ParentGUID.ToString();
                                                childNode4thLevelInnerViewModel.Type = ModuleTypeMasterList.Where(e => e.Id == Convert.ToInt64(subChildNode3rdInnerData.Type)).FirstOrDefault().TypeValue;//subChildNodeInnerData.Type.ToString();
                                                                                                                                                                                                          //childNodeInnerViewModel.ModuleDomainType = subChildNodeInnerData.ModuleDomainType;

                                                string[] crudTypes4thInnerChild = modeulAccessListData.CrudTypes.Split(",");
                                                childNode4thLevelInnerViewModel.CrudOption = new List<string>();
                                                //for (int m = 0; m < crudTypesInnerChild.Length; m++)
                                                //{
                                                //    childNodeInnerViewModel.CrudOption.Add(crudTypesInnerChild[m]);
                                                //}
                                                //Rita 29-3-19 remove i loop
                                                foreach (string CrudOpt in crudTypesInnerChild)
                                                {
                                                    childNodeInnerViewModel.CrudOption.Add(CRUDOptMasterList.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                                                }

                                                string[] utilityTypes4thInnerChild = modeulAccessListData.CrudTypes.Split(",");
                                                childNode4thLevelInnerViewModel.Utility = new List<string>();

                                                List<SubModuleMaster> subModuleMasterChild4thLevel = SubModuleMasterList.Where(e => e.ParentID == childNode4thLevelInnerViewModel.ID).ToList();
                                                if (subModuleMasterChild4thLevel.Count > 0)
                                                {
                                                    childNode4thLevelInnerViewModel.HasChild = true;
                                                    childNode4thLevelInnerViewModel.ChildNodes = new List<InnerChildNodeViewModel>();
                                                    foreach (var subChildNode5thInnerData in subModuleMasterChild4thLevel)
                                                    {
                                                        InnerChildNodeViewModel childNode5thLevelInnerViewModel = new InnerChildNodeViewModel();
                                                        childNode5thLevelInnerViewModel.ID = subChildNode5thInnerData.Id;
                                                        childNode5thLevelInnerViewModel.Status = subChildNode5thInnerData.Status;
                                                        childNode5thLevelInnerViewModel.SubModuleName = subChildNode5thInnerData.SubModuleName;
                                                        childNode5thLevelInnerViewModel.SubModuleID = subChildNode5thInnerData.ModuleID;
                                                        childNode5thLevelInnerViewModel.ParentID = subChildNode5thInnerData.ParentID;
                                                        childNode5thLevelInnerViewModel.GUID = subChildNode5thInnerData.GUID.ToString();
                                                        childNode5thLevelInnerViewModel.ParentGUID = subChildNode5thInnerData.ParentGUID.ToString();
                                                        childNode5thLevelInnerViewModel.Type = ModuleTypeMasterList.Where(e => e.Id == Convert.ToInt64(subChildNode5thInnerData.Type)).FirstOrDefault().TypeValue;//subChildNodeInnerData.Type.ToString();
                                                                                                                                                                                                                  //childNodeInnerViewModel.ModuleDomainType = subChildNodeInnerData.ModuleDomainType;

                                                        string[] crudTypes5thInnerChild = modeulAccessListData.CrudTypes.Split(",");
                                                        childNode5thLevelInnerViewModel.CrudOption = new List<string>();
                                                        //for (int m = 0; m < crudTypesInnerChild.Length; m++)
                                                        //{
                                                        //    childNodeInnerViewModel.CrudOption.Add(crudTypesInnerChild[m]);
                                                        //}
                                                        //Rita 29-3-19 remove i loop
                                                        foreach (string CrudOpt in crudTypesInnerChild)
                                                        {
                                                            childNodeInnerViewModel.CrudOption.Add(CRUDOptMasterList.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                                                        }

                                                        string[] utilityTypes5thInnerChild = modeulAccessListData.CrudTypes.Split(",");
                                                        childNode5thLevelInnerViewModel.Utility = new List<string>();

                                                        List<SubModuleMaster> subModuleMasterChild5thLevel = SubModuleMasterList.Where(e => e.ParentID == childNode5thLevelInnerViewModel.ID).ToList();
                                                        if (subModuleMasterChild5thLevel.Count > 0)
                                                        {
                                                            childNode5thLevelInnerViewModel.HasChild = true;
                                                            childNode5thLevelInnerViewModel.ChildNodes = new List<InnerChildNodeViewModel>();
                                                            foreach (var subChildNode6thInnerData in subModuleMasterChild5thLevel)
                                                            {
                                                                InnerChildNodeViewModel childNode6thLevelInnerViewModel = new InnerChildNodeViewModel();
                                                                childNode6thLevelInnerViewModel.ID = subChildNode6thInnerData.Id;
                                                                childNode6thLevelInnerViewModel.Status = subChildNode6thInnerData.Status;
                                                                childNode6thLevelInnerViewModel.SubModuleName = subChildNode6thInnerData.SubModuleName;
                                                                childNode6thLevelInnerViewModel.SubModuleID = subChildNode6thInnerData.ModuleID;
                                                                childNode6thLevelInnerViewModel.ParentID = subChildNode6thInnerData.ParentID;
                                                                childNode6thLevelInnerViewModel.GUID = subChildNode6thInnerData.GUID.ToString();
                                                                childNode6thLevelInnerViewModel.ParentGUID = subChildNode6thInnerData.ParentGUID.ToString();
                                                                childNode6thLevelInnerViewModel.Type = ModuleTypeMasterList.Where(e => e.Id == Convert.ToInt64(subChildNode6thInnerData.Type)).FirstOrDefault().TypeValue;//subChildNodeInnerData.Type.ToString();

                                                                string[] crudTypes6thInnerChild = modeulAccessListData.CrudTypes.Split(",");
                                                                childNode6thLevelInnerViewModel.CrudOption = new List<string>();
                                                                //for (int m = 0; m < crudTypesInnerChild.Length; m++)
                                                                //{
                                                                //    childNodeInnerViewModel.CrudOption.Add(crudTypesInnerChild[m]);
                                                                //}
                                                                //Rita 29-3-19 remove i loop
                                                                foreach (string CrudOpt in crudTypesInnerChild)
                                                                {
                                                                    childNodeInnerViewModel.CrudOption.Add(CRUDOptMasterList.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                                                                }

                                                                string[] utilityTypes6thInnerChild = modeulAccessListData.CrudTypes.Split(",");
                                                                childNode6thLevelInnerViewModel.Utility = new List<string>();

                                                                List<SubModuleFormMaster> subModule6thLevelFormMasterList = SubModuleFormMasterList.Where(e => e.ModuleGroupAccessID == childNode6thLevelInnerViewModel.SubModuleID).ToList();
                                                                if (subModule6thLevelFormMasterList.Count > 0)
                                                                {
                                                                    childNode6thLevelInnerViewModel.HasFields = true;

                                                                    foreach (var subModuelFormData in subModule6thLevelFormMasterList)
                                                                    {
                                                                        ChildNodeFiledViewModel childNodeFieldViewModel = new ChildNodeFiledViewModel();
                                                                        //ModuleUtilityMaster moduleUtilityMaster = await _moduleUtilityMaster.GetByIdAsync(Convert.ToInt32(subModuelFormData.FieldID)); //komal 29-03-2019 remove query
                                                                        //ModuleUtilityMaster moduleUtilityMaster = UtilityMasterList.Where(e => e.Id == Convert.ToInt64(subModuelFormData.FieldID)).FirstOrDefault();
                                                                        //childNodeFieldViewModel.FieldName = moduleUtilityMaster.UtilityName;
                                                                        //childNodeFieldViewModel.FiledID = moduleUtilityMaster.Id;
                                                                        //childNodeFieldViewModel.ID = moduleUtilityMaster.Id;
                                                                        ////childNodeFieldViewModel.IsVisibility = moduleUtilityMaster.
                                                                        //childNodeFieldViewModel.Status = moduleUtilityMaster.Status;
                                                                        //childNodeInnerViewModel.Fields.Add(childNodeFieldViewModel);
                                                                    }
                                                                    childNode5thLevelInnerViewModel.ChildNodes.Add(childNode6thLevelInnerViewModel);
                                                                }
                                                                childNode5thLevelInnerViewModel.ChildNodes.Add(childNode6thLevelInnerViewModel);
                                                            }
                                                        }
                                                        childNode5thLevelInnerViewModel.Fields = new List<ChildNodeFiledViewModel>();

                                                        //List<SubModuleFormMaster> subModuleFormMasterList = await _subModuleFormMaster.ListAsync(new SubFormModuleList(childNodeInnerViewModel.SubModuleID));//komal 29-03-2019 remove query
                                                        List<SubModuleFormMaster> subModuleFormMasterList = SubModuleFormMasterList.Where(e => e.ModuleGroupAccessID == childNode5thLevelInnerViewModel.SubModuleID).ToList();
                                                        if (subModuleFormMasterList.Count > 0)
                                                        {
                                                            childNode5thLevelInnerViewModel.HasFields = true;
                                                            foreach (var subModuelFormData in subModuleFormMasterList)
                                                            {
                                                                ChildNodeFiledViewModel childNodeFieldViewModel = new ChildNodeFiledViewModel();
                                                                //ModuleUtilityMaster moduleUtilityMaster = await _moduleUtilityMaster.GetByIdAsync(Convert.ToInt32(subModuelFormData.FieldID)); //komal 29-03-2019 remove query
                                                                ModuleUtilityMaster moduleUtilityMaster = UtilityMasterList.Where(e => e.Id == Convert.ToInt64(subModuelFormData.FieldID)).FirstOrDefault();
                                                                childNodeFieldViewModel.FieldName = moduleUtilityMaster.UtilityName;
                                                                childNodeFieldViewModel.FiledID = moduleUtilityMaster.Id;
                                                                //childNodeFieldViewModel.ID = moduleUtilityMaster.Id;
                                                                //childNodeFieldViewModel.IsVisibility = moduleUtilityMaster.
                                                                //childNodeFieldViewModel.Status = moduleUtilityMaster.Status;
                                                                childNodeInnerViewModel.Fields.Add(childNodeFieldViewModel);
                                                            }
                                                        }
                                                        childNode4thLevelInnerViewModel.ChildNodes.Add(childNode5thLevelInnerViewModel);
                                                    }
                                                }


                                                childNodeInnerViewModel.ChildNodes.Add(childNode4thLevelInnerViewModel);
                                            }
                                        }

                                        childNodeViewModel.ChildNodes.Add(childNodeInnerViewModel);
                                    }
                                    menuSubDetailViewModel.ChildNodes.Add(childNodeViewModel);
                                }
                                // If main module have fields.
                                //List<SubModuleFormMaster> subModuleFormMasters = await _subModuleFormMaster.ListAsync(new SubFormModuleList(menuSubDetailViewModel.ID));//komal 29-03-2019 remove query
                                List<SubModuleFormMaster> subModuleFormMasters = SubModuleFormMasterList.Where(e => e.ModuleGroupAccessID == menuSubDetailViewModel.ID).ToList();
                                menuSubDetailViewModel.Fields = new List<ChildNodeFiledViewModel>();
                                if (subModuleFormMasters.Count > 0)
                                {
                                    menuSubDetailViewModel.HasField = true;
                                }
                                foreach (var subModuleFormMastersData in subModuleFormMasters)
                                {
                                    ChildNodeFiledViewModel childNodeFieldViewModel = new ChildNodeFiledViewModel();
                                    //ModuleUtilityMaster moduleUtilityMaster = await _moduleUtilityMaster.GetByIdAsync(Convert.ToInt32(subModuleFormMastersData.FieldID)); //komal 29-03-2019 remove query
                                    ModuleUtilityMaster moduleUtilityMaster = UtilityMasterList.Where(e => e.Id == Convert.ToInt64(subModuleFormMastersData.FieldID)).FirstOrDefault();
                                    if (moduleUtilityMaster != null)
                                    {
                                        childNodeFieldViewModel.FieldName = moduleUtilityMaster.UtilityName;
                                        childNodeFieldViewModel.FiledID = moduleUtilityMaster.Id;
                                        //childNodeFieldViewModel.ID = moduleUtilityMaster.Id;
                                        //childNodeFieldViewModel.IsVisibility = moduleUtilityMaster.
                                        //childNodeFieldViewModel.Status = moduleUtilityMaster.Status;
                                        menuSubDetailViewModel.Fields.Add(childNodeFieldViewModel);
                                    }
                                }
                                Response.Result.Modules.Add(menuSubDetailViewModel);
                            }
                        }

                        return Response;
                    }
                    else
                    {
                        Response.Result = null;
                        Response.ErrorCode = 0;
                        Response.ReturnCode = 0;
                        Response.ReturnMsg = "No record for this group submodulemaster";
                    }
                }
                else
                {
                    // Need to change below data based on our Standardization. -Nishit Jani on A 2019-03-28 6:48 PM
                    Response.Result = null;
                    Response.ErrorCode = 0;
                    Response.ReturnCode = 0;
                    Response.ReturnMsg = "No record at all";
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        #endregion

        public bool CheckUserHaveMenuAccess(long GroupID, string MethodName, string ControllerName)
        {
            try
            {
                //long SubModuleID = _SubModuleRepository.FindByAsync(m => m.MethodName == MethodName && m.ControllerName == ControllerName).Result.Select(i => i.Id);
                var SubModuleData = _SubModuleRepository.FindBy(m => m.MethodName == MethodName && m.Controller == ControllerName).FirstOrDefault();
                if (SubModuleData == null)
                {
                    return false;
                }
                long HasAccess = _moduleGroupAccess.FindBy(a => a.SubModuleID == SubModuleData.Id && a.GroupID == GroupID).FirstOrDefault().Id;
                if (string.IsNullOrEmpty(HasAccess.ToString()))
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        public bool IsSkipAllow(string MethodName, string ControllerName)
        {
            try
            {
                //long SubModuleID = _SubModuleRepository.FindByAsync(m => m.MethodName == MethodName && m.ControllerName == ControllerName).Result.Select(i => i.Id);
                var SubModuleData = _SubModuleRepository.FindBy(m => m.MethodName == MethodName && m.Controller == ControllerName).FirstOrDefault();
                if (SubModuleData == null)
                {
                    return false;
                }
                // Add column to SubModuleMaster to add bit of Skip for Perticular Method.
                if (string.IsNullOrEmpty(SubModuleData.ToString()))
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        public async Task<MenuAccessResponse> GetMenuWithDetailsAsyncV2(long GroupID)
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            Response.Result = new MenuAccessDetailViewModel();
            Response.Result.Modules = new List<MenuSubDetailViewModel>();
            try
            {
                ModuleTypeMasterListV2 = _moduleTypeMaster.FindBy(e => e.Status == 1);
                CRUDOptMasterListV2 = _moduleCRUDOptMaster.FindBy(e => e.Status == 1);
                UtilityMasterListV2 = _moduleUtilityMaster.FindBy(e => e.Status == 1);
                SubModuleMasterListV2 = _SubModuleRepository.FindBy(e => e.Status == 1);
                SubModuleFormMasterListV2 = _subModuleFormMaster.FindBy(e => e.Status == 1);
                ModuleGroupAccessListV2 = _moduleGroupAccess.FindBy(e => e.Status == 1);
                FieldMasterListV2 = _fieldMaster.FindBy(e => e.Status == 1);
                ModuleVisibilityMasterListV2 = _moduleVisibilityMaster.FindBy(e => e.Status == 1);
                ModuleFieldRequirerMasterListV2 = _moduleFieldRequirerMaster.FindBy(e => e.Status == 1);
                ModuleAccessRightsMasterListV2 = _moduleAccessRightsMaster.FindBy(e => e.Status == 1);
                List<ModuleGroupAccess> moduleAccessList = await _moduleGroupAccess.GetAll(GroupID);
                if (moduleAccessList == null)
                {
                    Response.Result = null;
                    Response.ErrorCode = 0;
                    Response.ReturnCode = 0;
                    Response.ReturnMsg = "No record at all";
                    return Response;
                }
                Response.Result.GroupID = GroupID;
                Response.Result.Modules = new List<MenuSubDetailViewModel>();
                foreach (var modeulAccessListData in moduleAccessList)
                {
                    SubModuleMaster subModuleMaster = SubModuleMasterListV2.Where(e => e.Id == modeulAccessListData.SubModuleID).FirstOrDefault();
                    if (subModuleMaster != null)
                    {
                        MenuSubDetailViewModel menuSubDetailViewModel = new MenuSubDetailViewModel();
                        menuSubDetailViewModel.ID = subModuleMaster.Id;
                        menuSubDetailViewModel.Status = subModuleMaster.Status;
                        menuSubDetailViewModel.ModuleName = subModuleMaster.SubModuleName;
                        menuSubDetailViewModel.ModuelID = subModuleMaster.ModuleID;
                        menuSubDetailViewModel.ParentID = subModuleMaster.ParentID;
                        menuSubDetailViewModel.GUID = subModuleMaster.GUID.ToString();
                        menuSubDetailViewModel.ParentGUID = subModuleMaster.ParentGUID.ToString();
                        menuSubDetailViewModel.CrudOption = new List<string>();
                        menuSubDetailViewModel.Utility = new List<string>();
                        menuSubDetailViewModel.Fields = new List<ChildNodeFiledViewModel>();
                        menuSubDetailViewModel.ChildNodes = new List<ChildNodeViewModel>();
                        menuSubDetailViewModel.Type = ModuleTypeMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuleMaster.Type)).FirstOrDefault().TypeValue;//subModuleMaster.Type.ToString();

                        string[] crudTypes = modeulAccessListData.CrudTypes.Split(",");

                        foreach (string CrudOpt in crudTypes)
                        {
                            menuSubDetailViewModel.CrudOption.Add(CRUDOptMasterListV2.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                        }
                        string[] utilityTypes = modeulAccessListData.UtilityTypes.Split(",");

                        foreach (string utilityType in utilityTypes)
                        {
                            menuSubDetailViewModel.Utility.Add(UtilityMasterListV2.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                        }
                        List<SubModuleFormMaster> subModuleFormMastersV1 = SubModuleFormMasterListV2.Where(e => e.ModuleGroupAccessID == menuSubDetailViewModel.ID).ToList();

                        if (subModuleFormMastersV1.Count > 0)
                        {
                            menuSubDetailViewModel.HasField = true;
                        }
                        foreach (var subModuleFormMastersData in subModuleFormMastersV1)
                        {
                            //komal 05 - 04 - 2019 Comment for solve error

                            ChildNodeFiledViewModel childNodeFieldViewModelV1 = new ChildNodeFiledViewModel();
                            FieldMaster fieldMasterV1 = FieldMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuleFormMastersData.FieldID)).FirstOrDefault();
                            if (fieldMasterV1 != null)
                            {
                                childNodeFieldViewModelV1.FieldName = fieldMasterV1.FieldName;
                                childNodeFieldViewModelV1.FiledID = fieldMasterV1.Id;
                                childNodeFieldViewModelV1.AccessRight = ModuleVisibilityMasterListV2.Where(e => e.Id == fieldMasterV1.AccressRight).FirstOrDefault().VisibilityValue;
                                childNodeFieldViewModelV1.GUID = fieldMasterV1.GUID;
                                childNodeFieldViewModelV1.Required = ModuleFieldRequirerMasterListV2.Where(e => e.Id == fieldMasterV1.Required).FirstOrDefault().Value;
                                childNodeFieldViewModelV1.Visibility = ModuleAccessRightsMasterListV2.Where(e => e.Id == subModuleFormMastersData.Visibility).FirstOrDefault().AccessRightValue;
                                menuSubDetailViewModel.Fields.Add(childNodeFieldViewModelV1);
                            }
                        }

                        List<SubModuleMaster> subModuleMasterChild = SubModuleMasterListV2.Where(e => e.ParentID == menuSubDetailViewModel.ID).ToList();
                        if (subModuleMasterChild.Count > 0)
                        {
                            menuSubDetailViewModel.HasChild = true;
                            foreach (var subChildNodeData in subModuleMasterChild)
                            {
                                ChildNodeViewModel childNodeViewModel = new ChildNodeViewModel();
                                childNodeViewModel.ID = subChildNodeData.Id;
                                childNodeViewModel.Status = subChildNodeData.Status;
                                childNodeViewModel.ModuleName = subChildNodeData.SubModuleName;
                                childNodeViewModel.ModuelID = subChildNodeData.ModuleID;
                                childNodeViewModel.ParentID = subChildNodeData.ParentID;
                                childNodeViewModel.GUID = subChildNodeData.GUID.ToString();
                                childNodeViewModel.ParentGUID = subChildNodeData.ParentGUID.ToString();
                                childNodeViewModel.ChildNodes = new List<InnerChildNodeViewModel>();
                                childNodeViewModel.CrudOption = new List<string>();
                                childNodeViewModel.Utility = new List<string>();
                                childNodeViewModel.Fields = new List<ChildNodeFiledViewModel>();
                                childNodeViewModel.Type = ModuleTypeMasterListV2.Where(e => e.Id == Convert.ToInt64(subChildNodeData.Type)).FirstOrDefault().TypeValue;

                                var CrudTypes = ModuleGroupAccessListV2.Where(e => e.SubModuleID == subChildNodeData.Id).FirstOrDefault();
                                if (CrudTypes != null)
                                {
                                    string[] crudTypesInner = CrudTypes.CrudTypes.Split(",");

                                    foreach (string CrudOpt in crudTypesInner)
                                    {
                                        childNodeViewModel.CrudOption.Add(CRUDOptMasterListV2.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                                    }
                                }
                                var UtilityTypes = ModuleGroupAccessListV2.Where(e => e.SubModuleID == subChildNodeData.Id).FirstOrDefault();
                                if (UtilityTypes != null)
                                {
                                    string[] utilityTypesInner = UtilityTypes.UtilityTypes.Split(",");
                                    foreach (string utilityType in utilityTypesInner)
                                    {
                                        childNodeViewModel.Utility.Add(UtilityMasterListV2.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                                    }
                                }
                                List<SubModuleFormMaster> subModuleFormMastersChild = SubModuleFormMasterListV2.Where(e => e.ModuleGroupAccessID == childNodeViewModel.ID).ToList();
                                if (subModuleFormMastersChild.Count > 0)
                                {
                                    childNodeViewModel.HasField = true;
                                }
                                foreach (var subModuleFormMastersDataV2 in subModuleFormMastersChild)
                                {
                                    //komal 05 - 04 - 2019 Comment for solve error

                                    ChildNodeFiledViewModel childNodeFieldViewModelV2 = new ChildNodeFiledViewModel();
                                    FieldMaster fieldMasterChildV2 = FieldMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuleFormMastersDataV2.FieldID)).FirstOrDefault();
                                    if (fieldMasterChildV2 != null)
                                    {
                                        childNodeFieldViewModelV2.FieldName = fieldMasterChildV2.FieldName;
                                        childNodeFieldViewModelV2.FiledID = fieldMasterChildV2.Id;
                                        childNodeFieldViewModelV2.AccessRight = ModuleVisibilityMasterListV2.Where(e => e.Id == fieldMasterChildV2.AccressRight).FirstOrDefault().VisibilityValue;
                                        childNodeFieldViewModelV2.GUID = fieldMasterChildV2.GUID;
                                        childNodeFieldViewModelV2.Required = ModuleFieldRequirerMasterListV2.Where(e => e.Id == fieldMasterChildV2.Required).FirstOrDefault().Value;
                                        childNodeFieldViewModelV2.Visibility = ModuleAccessRightsMasterListV2.Where(e => e.Id == subModuleFormMastersDataV2.Visibility).FirstOrDefault().AccessRightValue;
                                        childNodeViewModel.Fields.Add(childNodeFieldViewModelV2);
                                    }
                                }
                                InnerChildNodeViewModel SubMenu = this.GetSubMenuRecursive(childNodeViewModel.ID, modeulAccessListData.CrudTypes);
                                if (SubMenu != null)
                                {
                                    childNodeViewModel.HasChild = true;
                                    childNodeViewModel.ChildNodes = SubMenu.ChildNodes;
                                }
                                menuSubDetailViewModel.ChildNodes.Add(childNodeViewModel);
                            }
                        }
                        Response.Result.Modules.Add(menuSubDetailViewModel);
                    }

                    //Response.Result.Modules.Add(this.GetSubMenuRecursive(modeulAccessListData.SubModuleID));

                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public InnerChildNodeViewModel GetSubMenuRecursive(long ID, String CRUDType)
        {
            InnerChildNodeViewModel menuSubDetailView = new InnerChildNodeViewModel();
            try
            {
                ModuleAccessRightsMasterListV2 = _moduleAccessRightsMaster.FindBy(e => e.Status == 1);
                List<SubModuleMaster> subModuleMasterChild = SubModuleMasterListV2.Where(e => e.ParentID == ID).ToList();
                if (subModuleMasterChild.Count > 0)
                {
                    menuSubDetailView.HasChild = true;
                    menuSubDetailView.ChildNodes = new List<InnerChildNodeViewModel>();

                    foreach (var subChildNodeData in subModuleMasterChild)
                    {

                        InnerChildNodeViewModel InnerchildNodeViewModel = new InnerChildNodeViewModel();
                        InnerchildNodeViewModel.ID = subChildNodeData.Id;
                        InnerchildNodeViewModel.Status = subChildNodeData.Status;
                        InnerchildNodeViewModel.SubModuleName = subChildNodeData.SubModuleName;
                        InnerchildNodeViewModel.SubModuleID = subChildNodeData.ModuleID;
                        InnerchildNodeViewModel.ParentID = subChildNodeData.ParentID;
                        InnerchildNodeViewModel.GUID = subChildNodeData.GUID.ToString();
                        InnerchildNodeViewModel.ParentGUID = subChildNodeData.ParentGUID.ToString();
                        InnerchildNodeViewModel.CrudOption = new List<string>();
                        InnerchildNodeViewModel.Utility = new List<string>();
                        InnerchildNodeViewModel.Fields = new List<ChildNodeFiledViewModel>();
                        InnerchildNodeViewModel.ChildNodes = new List<InnerChildNodeViewModel>();
                        InnerchildNodeViewModel.Tools = new List<string>();
                        InnerchildNodeViewModel.Type = ModuleTypeMasterListV2.Where(e => e.Id == Convert.ToInt64(subChildNodeData.Type)).FirstOrDefault().TypeValue;

                        var CrudTypes = ModuleGroupAccessListV2.Where(e => e.SubModuleID == subChildNodeData.Id).FirstOrDefault();
                        if (CrudTypes != null)
                        {
                            string[] crudTypesInner = CrudTypes.CrudTypes.Split(",");

                            foreach (string CrudOpt in crudTypesInner)
                            {
                                InnerchildNodeViewModel.CrudOption.Add(CRUDOptMasterListV2.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                            }
                        }
                        var UtilityTypes = ModuleGroupAccessListV2.Where(e => e.SubModuleID == subChildNodeData.Id).FirstOrDefault();
                        if (UtilityTypes != null)
                        {
                            string[] utilityTypesInner = UtilityTypes.UtilityTypes.Split(",");

                            foreach (string utilityType in utilityTypesInner)
                            {
                                InnerchildNodeViewModel.Utility.Add(UtilityMasterListV2.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                            }
                        }

                        List<SubModuleFormMaster> subModuleFormMasterList = SubModuleFormMasterListV2.Where(e => e.ModuleGroupAccessID == InnerchildNodeViewModel.ID).ToList();
                        if (subModuleFormMasterList.Count > 0)
                        {
                            InnerchildNodeViewModel.HasFields = true;

                            foreach (var subModuelFormData in subModuleFormMasterList)
                            {
                                //komal 05-04-2019 Comment for solve error
                                ChildNodeFiledViewModel childNodeFieldViewModel = new ChildNodeFiledViewModel();
                                FieldMaster fieldMaster = FieldMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuelFormData.FieldID)).FirstOrDefault();
                                if (fieldMaster != null)
                                {
                                    childNodeFieldViewModel.FieldName = fieldMaster.FieldName;
                                    childNodeFieldViewModel.FiledID = fieldMaster.Id;
                                    childNodeFieldViewModel.AccessRight = ModuleVisibilityMasterListV2.Where(e => e.Id == fieldMaster.AccressRight).FirstOrDefault().VisibilityValue;
                                    childNodeFieldViewModel.GUID = fieldMaster.GUID;
                                    childNodeFieldViewModel.Required = ModuleFieldRequirerMasterListV2.Where(e => e.Id == fieldMaster.Required).FirstOrDefault().Value;
                                    childNodeFieldViewModel.Visibility = ModuleAccessRightsMasterListV2.Where(e => e.Id == subModuelFormData.Visibility).FirstOrDefault().AccessRightValue;
                                    InnerchildNodeViewModel.Fields.Add(childNodeFieldViewModel);
                                }
                            }
                        }
                        var submenu = this.GetSubMenuRecursive(InnerchildNodeViewModel.ID, CRUDType);
                        if (submenu != null)
                        {
                            InnerchildNodeViewModel.HasChild = true;

                            InnerchildNodeViewModel.ChildNodes = submenu.ChildNodes;
                        }

                        menuSubDetailView.ChildNodes.Add(InnerchildNodeViewModel);
                    }
                }
                else
                    return null;

                return menuSubDetailView;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<MenuAccessResponse> GetDefaultMenus()
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            Response.Result = new MenuAccessDetailViewModel();
            Response.Result.Modules = new List<MenuSubDetailViewModel>();
            try
            {
                ModuleTypeMasterListV2 = _moduleTypeMaster.FindBy(e => e.Status == 1);
                CRUDOptMasterListV2 = _moduleCRUDOptMaster.FindBy(e => e.Status == 1);
                UtilityMasterListV2 = _moduleUtilityMaster.FindBy(e => e.Status == 1);
                moduleMasterListV2 = _SubModuleRepository.FindBy(e => e.Status == 1 && e.ParentID == 0);
                SubModuleMasterListV2 = _SubModuleRepository.FindBy(e => e.Status == 1);
                SubModuleFormMasterListV2 = _subModuleFormMaster.FindBy(e => e.Status == 1);
                FieldMasterListV2 = _fieldMaster.FindBy(e => e.Status == 1);
                ModuleVisibilityMasterListV2 = _moduleVisibilityMaster.FindBy(e => e.Status == 1);
                ModuleFieldRequirerMasterListV2 = _moduleFieldRequirerMaster.FindBy(e => e.Status == 1);
                ModuleAccessRightsMasterListV2 = _moduleAccessRightsMaster.FindBy(e => e.Status == 1);
                if (moduleMasterListV2 == null || SubModuleMasterListV2 == null)
                {
                    Response.Result = null;
                    Response.ErrorCode = 0;
                    Response.ReturnCode = 0;
                    Response.ReturnMsg = "No record at all";
                    return Response;
                }
                Response.Result.GroupID = 0;
                Response.Result.Modules = new List<MenuSubDetailViewModel>();
                foreach (var modeulAccessListData in moduleMasterListV2)
                {
                    MenuSubDetailViewModel menuSubDetailViewModel = new MenuSubDetailViewModel();
                    menuSubDetailViewModel.ID = modeulAccessListData.Id;
                    menuSubDetailViewModel.Status = modeulAccessListData.Status;
                    menuSubDetailViewModel.ModuleName = modeulAccessListData.SubModuleName;
                    menuSubDetailViewModel.ModuelID = modeulAccessListData.ModuleID;
                    menuSubDetailViewModel.ParentID = modeulAccessListData.ParentID;
                    menuSubDetailViewModel.GUID = modeulAccessListData.GUID.ToString();
                    menuSubDetailViewModel.ParentGUID = modeulAccessListData.ParentGUID.ToString();
                    menuSubDetailViewModel.CrudOption = new List<string>();
                    menuSubDetailViewModel.Utility = new List<string>();
                    menuSubDetailViewModel.Fields = new List<ChildNodeFiledViewModel>();
                    menuSubDetailViewModel.ChildNodes = new List<ChildNodeViewModel>();
                    menuSubDetailViewModel.Type = ModuleTypeMasterListV2.Where(e => e.Id == Convert.ToInt64(modeulAccessListData.Type)).FirstOrDefault().TypeValue;//subModuleMaster.Type.ToString();

                    string[] crudTypes = modeulAccessListData.CrudTypes.Split(",");

                    foreach (string CrudOpt in crudTypes)
                    {
                        if (CrudOpt.ToLower() != "null")
                        {
                            menuSubDetailViewModel.CrudOption.Add(CRUDOptMasterListV2.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                        }
                        else
                        {
                            menuSubDetailViewModel.CrudOption.Add("NULL");
                        }
                    }
                    string[] utilityTypes = modeulAccessListData.UtilityTypes.Split(",");

                    foreach (string utilityType in utilityTypes)
                    {
                        if (utilityType.ToLower() != "5")
                        {
                            menuSubDetailViewModel.Utility.Add(UtilityMasterListV2.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                        }
                        else
                        {
                            menuSubDetailViewModel.Utility.Add("NULL");
                        }
                    }
                    List<SubModuleFormMaster> subModuleFormMastersV1 = SubModuleFormMasterListV2.Where(e => e.ModuleGroupAccessID == menuSubDetailViewModel.ID).ToList();

                    if (subModuleFormMastersV1.Count > 0)
                    {
                        menuSubDetailViewModel.HasField = true;
                    }
                    foreach (var subModuleFormMastersData in subModuleFormMastersV1)
                    {
                        //komal 05 - 04 - 2019 Comment for solve error

                        ChildNodeFiledViewModel childNodeFieldViewModelV1 = new ChildNodeFiledViewModel();
                        FieldMaster fieldMasterV1 = FieldMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuleFormMastersData.FieldID)).FirstOrDefault();
                        if (fieldMasterV1 != null)
                        {
                            childNodeFieldViewModelV1.FieldName = fieldMasterV1.FieldName;
                            childNodeFieldViewModelV1.FiledID = fieldMasterV1.Id;
                            childNodeFieldViewModelV1.AccessRight = ModuleVisibilityMasterListV2.Where(e => e.Id == fieldMasterV1.AccressRight).FirstOrDefault().VisibilityValue;
                            childNodeFieldViewModelV1.GUID = fieldMasterV1.GUID;
                            childNodeFieldViewModelV1.Required = ModuleFieldRequirerMasterListV2.Where(e => e.Id == fieldMasterV1.Required).FirstOrDefault().Value;
                            childNodeFieldViewModelV1.Visibility = ModuleAccessRightsMasterListV2.Where(e => e.Id == subModuleFormMastersData.Visibility).FirstOrDefault().AccessRightValue;
                            menuSubDetailViewModel.Fields.Add(childNodeFieldViewModelV1);
                        }
                    }

                    List<SubModuleMaster> subModuleMasterChild = SubModuleMasterListV2.Where(e => e.ParentID == menuSubDetailViewModel.ID).ToList();
                    if (subModuleMasterChild.Count > 0)
                    {
                        menuSubDetailViewModel.HasChild = true;
                        foreach (var subChildNodeData in subModuleMasterChild)
                        {
                            ChildNodeViewModel childNodeViewModel = new ChildNodeViewModel();
                            childNodeViewModel.ID = subChildNodeData.Id;
                            childNodeViewModel.Status = subChildNodeData.Status;
                            childNodeViewModel.ModuleName = subChildNodeData.SubModuleName;
                            childNodeViewModel.ModuelID = subChildNodeData.ModuleID;
                            childNodeViewModel.ParentID = subChildNodeData.ParentID;
                            childNodeViewModel.GUID = subChildNodeData.GUID.ToString();
                            childNodeViewModel.ParentGUID = subChildNodeData.ParentGUID.ToString();
                            childNodeViewModel.ChildNodes = new List<InnerChildNodeViewModel>();
                            childNodeViewModel.CrudOption = new List<string>();
                            childNodeViewModel.Utility = new List<string>();
                            childNodeViewModel.Fields = new List<ChildNodeFiledViewModel>();
                            childNodeViewModel.Type = ModuleTypeMasterListV2.Where(e => e.Id == Convert.ToInt64(subChildNodeData.Type)).FirstOrDefault().TypeValue;

                            string[] crudTypesInner = subChildNodeData.CrudTypes.Split(",");

                            foreach (string CrudOpt in crudTypesInner)
                            {
                                if (CrudOpt.ToLower() != "null")
                                {
                                    childNodeViewModel.CrudOption.Add(CRUDOptMasterListV2.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                                }
                                else
                                {
                                    childNodeViewModel.CrudOption.Add("NULL");
                                }
                            }

                            string[] utilityTypesInner = subChildNodeData.UtilityTypes.Split(",");
                            foreach (string utilityType in utilityTypesInner)
                            {
                                if (utilityType.ToLower() != "5")
                                {
                                    childNodeViewModel.Utility.Add(UtilityMasterListV2.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                                }
                                else
                                {
                                    childNodeViewModel.Utility.Add("NULL");
                                }
                            }

                            List<SubModuleFormMaster> subModuleFormMastersChild = SubModuleFormMasterListV2.Where(e => e.ModuleGroupAccessID == childNodeViewModel.ID).ToList();
                            if (subModuleFormMastersChild.Count > 0)
                            {
                                childNodeViewModel.HasField = true;
                            }
                            foreach (var subModuleFormMastersDataV2 in subModuleFormMastersChild)
                            {
                                //komal 05 - 04 - 2019 Comment for solve error

                                ChildNodeFiledViewModel childNodeFieldViewModelV2 = new ChildNodeFiledViewModel();
                                FieldMaster fieldMasterChildV2 = FieldMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuleFormMastersDataV2.FieldID)).FirstOrDefault();
                                if (fieldMasterChildV2 != null)
                                {
                                    childNodeFieldViewModelV2.FieldName = fieldMasterChildV2.FieldName;
                                    childNodeFieldViewModelV2.FiledID = fieldMasterChildV2.Id;
                                    childNodeFieldViewModelV2.AccessRight = ModuleVisibilityMasterListV2.Where(e => e.Id == fieldMasterChildV2.AccressRight).FirstOrDefault().VisibilityValue;
                                    childNodeFieldViewModelV2.GUID = fieldMasterChildV2.GUID;
                                    childNodeFieldViewModelV2.Required = ModuleFieldRequirerMasterListV2.Where(e => e.Id == fieldMasterChildV2.Required).FirstOrDefault().Value;
                                    childNodeFieldViewModelV2.Visibility = ModuleAccessRightsMasterListV2.Where(e => e.Id == subModuleFormMastersDataV2.Visibility).FirstOrDefault().AccessRightValue;
                                    childNodeViewModel.Fields.Add(childNodeFieldViewModelV2);
                                }
                            }
                            InnerChildNodeViewModel SubMenu = this.GetSubMenuRecursive(childNodeViewModel.ID);
                            if (SubMenu != null)
                            {
                                childNodeViewModel.HasChild = true;
                                childNodeViewModel.ChildNodes = SubMenu.ChildNodes;
                            }
                            menuSubDetailViewModel.ChildNodes.Add(childNodeViewModel);
                        }
                    }
                    Response.Result.Modules.Add(menuSubDetailViewModel);
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public InnerChildNodeViewModel GetSubMenuRecursive(long ID)
        {
            InnerChildNodeViewModel menuSubDetailView = new InnerChildNodeViewModel();
            try
            {
                ModuleAccessRightsMasterListV2 = _moduleAccessRightsMaster.FindBy(e => e.Status == 1);
                List<SubModuleMaster> subModuleMasterChild = SubModuleMasterListV2.Where(e => e.ParentID == ID).ToList();
                if (subModuleMasterChild.Count > 0)
                {
                    menuSubDetailView.HasChild = true;
                    menuSubDetailView.ChildNodes = new List<InnerChildNodeViewModel>();

                    foreach (var subChildNodeData in subModuleMasterChild)
                    {
                        InnerChildNodeViewModel InnerchildNodeViewModel = new InnerChildNodeViewModel();
                        InnerchildNodeViewModel.ID = subChildNodeData.Id;
                        InnerchildNodeViewModel.Status = subChildNodeData.Status;
                        InnerchildNodeViewModel.SubModuleName = subChildNodeData.SubModuleName;
                        InnerchildNodeViewModel.SubModuleID = subChildNodeData.ModuleID;
                        InnerchildNodeViewModel.ParentID = subChildNodeData.ParentID;
                        InnerchildNodeViewModel.GUID = subChildNodeData.GUID.ToString();
                        InnerchildNodeViewModel.ParentGUID = subChildNodeData.ParentGUID.ToString();
                        InnerchildNodeViewModel.CrudOption = new List<string>();
                        InnerchildNodeViewModel.Utility = new List<string>();
                        InnerchildNodeViewModel.Fields = new List<ChildNodeFiledViewModel>();
                        InnerchildNodeViewModel.ChildNodes = new List<InnerChildNodeViewModel>();
                        InnerchildNodeViewModel.Tools = new List<string>();
                        InnerchildNodeViewModel.Type = ModuleTypeMasterListV2.Where(e => e.Id == Convert.ToInt64(subChildNodeData.Type)).FirstOrDefault().TypeValue;

                        string[] crudTypesInner = subChildNodeData.CrudTypes.Split(",");

                        foreach (string CrudOpt in crudTypesInner)
                        {
                            if (CrudOpt.ToLower() != "null")
                            {
                                InnerchildNodeViewModel.CrudOption.Add(CRUDOptMasterListV2.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                            }
                            else
                            {
                                InnerchildNodeViewModel.CrudOption.Add("NULL");
                            }
                        }

                        string[] utilityTypesInner = subChildNodeData.UtilityTypes.Split(",");

                        foreach (string utilityType in utilityTypesInner)
                        {
                            if (utilityType.ToLower() != "5")
                            {
                                InnerchildNodeViewModel.Utility.Add(UtilityMasterListV2.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                            }
                            else
                            {
                                InnerchildNodeViewModel.Utility.Add("NULL");
                            }
                        }


                        List<SubModuleFormMaster> subModuleFormMasterList = SubModuleFormMasterListV2.Where(e => e.ModuleGroupAccessID == InnerchildNodeViewModel.ID).ToList();
                        if (subModuleFormMasterList.Count > 0)
                        {
                            InnerchildNodeViewModel.HasFields = true;

                            foreach (var subModuelFormData in subModuleFormMasterList)
                            {
                                //komal 05-04-2019 Comment for solve error
                                ChildNodeFiledViewModel childNodeFieldViewModel = new ChildNodeFiledViewModel();
                                FieldMaster fieldMaster = FieldMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuelFormData.FieldID)).FirstOrDefault();
                                if (fieldMaster != null)
                                {
                                    childNodeFieldViewModel.FieldName = fieldMaster.FieldName;
                                    childNodeFieldViewModel.FiledID = fieldMaster.Id;
                                    childNodeFieldViewModel.AccessRight = ModuleVisibilityMasterListV2.Where(e => e.Id == fieldMaster.AccressRight).FirstOrDefault().VisibilityValue;
                                    childNodeFieldViewModel.GUID = fieldMaster.GUID;
                                    childNodeFieldViewModel.Required = ModuleFieldRequirerMasterListV2.Where(e => e.Id == fieldMaster.Required).FirstOrDefault().Value;
                                    childNodeFieldViewModel.Visibility = ModuleAccessRightsMasterListV2.Where(e => e.Id == subModuelFormData.Visibility).FirstOrDefault().AccessRightValue;
                                    InnerchildNodeViewModel.Fields.Add(childNodeFieldViewModel);
                                }
                            }
                        }
                        var submenu = this.GetSubMenuRecursive(InnerchildNodeViewModel.ID);
                        if (submenu != null)
                        {
                            InnerchildNodeViewModel.HasChild = true;

                            InnerchildNodeViewModel.ChildNodes = submenu.ChildNodes;
                        }

                        menuSubDetailView.ChildNodes.Add(InnerchildNodeViewModel);
                    }
                }
                else
                    return null;

                return menuSubDetailView;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<MenuAccessResponse> GetMenuWithDetailsAsyncV2MainMenu(long GroupID, Guid ParentID)
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            Response.Result = new MenuAccessDetailViewModel();
            Response.Result.Modules = new List<MenuSubDetailViewModel>();
            try
            {
                ModuleTypeMasterListV2 = _moduleTypeMaster.FindBy(e => e.Status == 1);
                CRUDOptMasterListV2 = _moduleCRUDOptMaster.FindBy(e => e.Status == 1);
                UtilityMasterListV2 = _moduleUtilityMaster.FindBy(e => e.Status == 1);
                SubModuleMasterListV2 = _SubModuleRepository.FindBy(e => e.Status == 1 && e.ParentGUID == ParentID);
                SubModuleFormMasterListV2 = _subModuleFormMaster.FindBy(e => e.Status == 1);
                ModuleGroupAccessListV2 = _moduleGroupAccess.FindBy(e => e.Status == 1);
                FieldMasterListV2 = _fieldMaster.FindBy(e => e.Status == 1);
                ModuleVisibilityMasterListV2 = _moduleVisibilityMaster.FindBy(e => e.Status == 1);
                ModuleFieldRequirerMasterListV2 = _moduleFieldRequirerMaster.FindBy(e => e.Status == 1);
                ModuleAccessRightsMasterListV2 = _moduleAccessRightsMaster.FindBy(e => e.Status == 1);
                List<ModuleGroupAccess> moduleAccessList = await _moduleGroupAccess.GetAllByParentIDV1(GroupID, ParentID);//komal 6th May,2019
                //List<ModuleGroupAccess> moduleAccessList = await _moduleGroupAccess.GetAllByParentID(GroupID, ParentID);
                if (moduleAccessList.Count == 0)
                {
                    Response.Result = null;
                    Response.ErrorCode = 0;
                    Response.ReturnCode = 0;
                    Response.ReturnMsg = "No record at all";
                    return Response;
                }
                Response.Result.GroupID = GroupID;
                Response.Result.Modules = new List<MenuSubDetailViewModel>();
                foreach (var modeulAccessListData in moduleAccessList)
                {
                    SubModuleMaster subModuleMaster = SubModuleMasterListV2.Where(e => e.Id == modeulAccessListData.SubModuleID).FirstOrDefault();
                    if (subModuleMaster != null)
                    {
                        MenuSubDetailViewModel menuSubDetailViewModel = new MenuSubDetailViewModel();
                        menuSubDetailViewModel.ID = subModuleMaster.Id;
                        menuSubDetailViewModel.Status = subModuleMaster.Status;
                        menuSubDetailViewModel.ModuleName = subModuleMaster.SubModuleName;
                        menuSubDetailViewModel.ModuelID = subModuleMaster.ModuleID;
                        menuSubDetailViewModel.ParentID = subModuleMaster.ParentID;
                        menuSubDetailViewModel.GUID = subModuleMaster.GUID.ToString();
                        menuSubDetailViewModel.ParentGUID = subModuleMaster.ParentGUID.ToString();
                        menuSubDetailViewModel.CrudOption = new List<string>();
                        menuSubDetailViewModel.Utility = new List<string>();
                        menuSubDetailViewModel.Fields = new List<ChildNodeFiledViewModel>();
                        menuSubDetailViewModel.ChildNodes = new List<ChildNodeViewModel>();
                        //int counts = _moduleGroupAccess.HasChild(GroupID, Guid.Parse(menuSubDetailViewModel.GUID));
                        //List<ModuleGroupAccess> moduleAccessListChild = await _moduleGroupAccess.GetAllByParentID(GroupID, Guid.Parse(menuSubDetailViewModel.GUID));
                        List<ModuleGroupAccess> moduleAccessListChild = await _moduleGroupAccess.GetAllByParentIDV1(GroupID, Guid.Parse(menuSubDetailViewModel.GUID)); //komal 6th May,2019
                        menuSubDetailViewModel.Type = ModuleTypeMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuleMaster.Type)).FirstOrDefault().TypeValue;//subModuleMaster.Type.ToString();
                        menuSubDetailViewModel.HasChild = (moduleAccessListChild.Count() > 0 ? true : false);
                        string[] crudTypes = modeulAccessListData.CrudTypes.Split(",");

                        foreach (string CrudOpt in crudTypes)
                        {
                            menuSubDetailViewModel.CrudOption.Add(CRUDOptMasterListV2.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                        }
                        string[] utilityTypes = modeulAccessListData.UtilityTypes.Split(",");

                        foreach (string utilityType in utilityTypes)
                        {
                            menuSubDetailViewModel.Utility.Add(UtilityMasterListV2.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                        }

                        //List<SubModuleFormMaster> subModuleFormMastersV1 = SubModuleFormMasterListV2.Where(e => e.ModuleGroupAccessID == menuSubDetailViewModel.ID).ToList();
                        // Modified query as getting wrong result. Corrected with help of Komal. -Nishit Jani on A 2019-05-03 3:00 PM
                        List<SubModuleFormMaster> subModuleFormMastersV1 = SubModuleFormMasterListV2.Where(e => e.ModuleGroupAccessID == modeulAccessListData.Id).ToList();

                        if (subModuleFormMastersV1.Count > 0)
                        {
                            menuSubDetailViewModel.HasField = true;
                        }
                        foreach (var subModuleFormMastersData in subModuleFormMastersV1)
                        {
                            //komal 05 - 04 - 2019 Comment for solve error

                            ChildNodeFiledViewModel childNodeFieldViewModelV1 = new ChildNodeFiledViewModel();
                            FieldMaster fieldMasterV1 = FieldMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuleFormMastersData.FieldID)).FirstOrDefault();
                            if (fieldMasterV1 != null)
                            {
                                childNodeFieldViewModelV1.FieldName = fieldMasterV1.FieldName;
                                childNodeFieldViewModelV1.FiledID = fieldMasterV1.Id;
                                childNodeFieldViewModelV1.AccessRight = ModuleVisibilityMasterListV2.Where(e => e.Id == fieldMasterV1.AccressRight).FirstOrDefault().VisibilityValue;
                                childNodeFieldViewModelV1.GUID = fieldMasterV1.GUID;
                                childNodeFieldViewModelV1.Required = ModuleFieldRequirerMasterListV2.Where(e => e.Id == fieldMasterV1.Required).FirstOrDefault().Value;
                                childNodeFieldViewModelV1.Visibility = ModuleAccessRightsMasterListV2.Where(e => e.Id == subModuleFormMastersData.Visibility).FirstOrDefault().AccessRightValue;
                                menuSubDetailViewModel.Fields.Add(childNodeFieldViewModelV1);
                            }
                        }
                        Response.Result.Modules.Add(menuSubDetailViewModel);
                    }

                    //Response.Result.Modules.Add(this.GetSubMenuRecursive(modeulAccessListData.SubModuleID));

                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<MenuAccessResponse> GetMasterList(Guid ParentID)
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            Response.Result = new MenuAccessDetailViewModel();
            Response.Result.Modules = new List<MenuSubDetailViewModel>();
            try
            {
                ModuleTypeMasterListV2 = _moduleTypeMaster.FindBy(e => e.Status == 1);
                CRUDOptMasterListV2 = _moduleCRUDOptMaster.FindBy(e => e.Status == 1);
                UtilityMasterListV2 = _moduleUtilityMaster.FindBy(e => e.Status == 1);
                moduleMasterListV2 = _SubModuleRepository.FindBy(e => e.Status == 1 && e.ParentGUID == ParentID);
                SubModuleMasterListV2 = _SubModuleRepository.FindBy(e => e.Status == 1);
                SubModuleFormMasterListV2 = _subModuleFormMaster.FindBy(e => e.Status == 1);
                FieldMasterListV2 = _fieldMaster.FindBy(e => e.Status == 1);
                ModuleVisibilityMasterListV2 = _moduleVisibilityMaster.FindBy(e => e.Status == 1);
                ModuleFieldRequirerMasterListV2 = _moduleFieldRequirerMaster.FindBy(e => e.Status == 1);
                ModuleAccessRightsMasterListV2 = _moduleAccessRightsMaster.FindBy(e => e.Status == 1);
                if (moduleMasterListV2 == null || SubModuleMasterListV2 == null)
                {
                    Response.Result = null;
                    Response.ErrorCode = 0;
                    Response.ReturnCode = 0;
                    Response.ReturnMsg = "No record at all";
                    return Response;
                }
                Response.Result.GroupID = 0;
                Response.Result.Modules = new List<MenuSubDetailViewModel>();
                foreach (var modeulAccessListData in moduleMasterListV2)
                {
                    MenuSubDetailViewModel menuSubDetailViewModel = new MenuSubDetailViewModel();
                    menuSubDetailViewModel.ID = modeulAccessListData.Id;
                    menuSubDetailViewModel.Status = modeulAccessListData.Status;
                    menuSubDetailViewModel.ModuleName = modeulAccessListData.SubModuleName;
                    menuSubDetailViewModel.ModuelID = modeulAccessListData.ModuleID;
                    menuSubDetailViewModel.ParentID = modeulAccessListData.ParentID;
                    menuSubDetailViewModel.GUID = modeulAccessListData.GUID.ToString();
                    menuSubDetailViewModel.ParentGUID = modeulAccessListData.ParentGUID.ToString();
                    menuSubDetailViewModel.CrudOption = new List<string>();
                    menuSubDetailViewModel.Utility = new List<string>();
                    menuSubDetailViewModel.Fields = new List<ChildNodeFiledViewModel>();
                    menuSubDetailViewModel.ChildNodes = new List<ChildNodeViewModel>();
                    menuSubDetailViewModel.Type = ModuleTypeMasterListV2.Where(e => e.Id == Convert.ToInt64(modeulAccessListData.Type)).FirstOrDefault().TypeValue;//subModuleMaster.Type.ToString();

                    string[] crudTypes = modeulAccessListData.CrudTypes.Split(",");

                    foreach (string CrudOpt in crudTypes)
                    {
                        if (CrudOpt.ToLower() != "null")
                        {
                            menuSubDetailViewModel.CrudOption.Add(CRUDOptMasterListV2.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                        }
                        else
                        {
                            menuSubDetailViewModel.CrudOption.Add("NULL");
                        }
                    }
                    string[] utilityTypes = modeulAccessListData.UtilityTypes.Split(",");

                    foreach (string utilityType in utilityTypes)
                    {
                        if (utilityType.ToLower() != "5")
                        {
                            menuSubDetailViewModel.Utility.Add(UtilityMasterListV2.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                        }
                        else
                        {
                            menuSubDetailViewModel.Utility.Add("NULL");
                        }
                    }
                    List<SubModuleFormMaster> subModuleFormMastersV1 = SubModuleFormMasterListV2.Where(e => e.ModuleGroupAccessID == menuSubDetailViewModel.ID).ToList();

                    if (subModuleFormMastersV1.Count > 0)
                    {
                        menuSubDetailViewModel.HasField = true;
                    }
                    foreach (var subModuleFormMastersData in subModuleFormMastersV1)
                    {
                        //komal 05 - 04 - 2019 Comment for solve error

                        ChildNodeFiledViewModel childNodeFieldViewModelV1 = new ChildNodeFiledViewModel();
                        FieldMaster fieldMasterV1 = FieldMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuleFormMastersData.FieldID)).FirstOrDefault();
                        if (fieldMasterV1 != null)
                        {
                            childNodeFieldViewModelV1.FieldName = fieldMasterV1.FieldName;
                            childNodeFieldViewModelV1.FiledID = fieldMasterV1.Id;
                            childNodeFieldViewModelV1.AccessRight = ModuleVisibilityMasterListV2.Where(e => e.Id == fieldMasterV1.AccressRight).FirstOrDefault().VisibilityValue;
                            childNodeFieldViewModelV1.GUID = fieldMasterV1.GUID;
                            childNodeFieldViewModelV1.Required = ModuleFieldRequirerMasterListV2.Where(e => e.Id == fieldMasterV1.Required).FirstOrDefault().Value;
                            childNodeFieldViewModelV1.Visibility = ModuleAccessRightsMasterListV2.Where(e => e.Id == subModuleFormMastersData.Visibility).FirstOrDefault().AccessRightValue;
                            menuSubDetailViewModel.Fields.Add(childNodeFieldViewModelV1);
                        }
                    }

                    List<SubModuleMaster> subModuleMasterChild = SubModuleMasterListV2.Where(e => e.ParentID == menuSubDetailViewModel.ID).ToList();
                    if (subModuleMasterChild.Count > 0)
                    {
                        menuSubDetailViewModel.HasChild = true;
                        foreach (var subChildNodeData in subModuleMasterChild)
                        {
                            ChildNodeViewModel childNodeViewModel = new ChildNodeViewModel();
                            childNodeViewModel.ID = subChildNodeData.Id;
                            childNodeViewModel.Status = subChildNodeData.Status;
                            childNodeViewModel.ModuleName = subChildNodeData.SubModuleName;
                            childNodeViewModel.ModuelID = subChildNodeData.ModuleID;
                            childNodeViewModel.ParentID = subChildNodeData.ParentID;
                            childNodeViewModel.GUID = subChildNodeData.GUID.ToString();
                            childNodeViewModel.ParentGUID = subChildNodeData.ParentGUID.ToString();
                            childNodeViewModel.ChildNodes = new List<InnerChildNodeViewModel>();
                            childNodeViewModel.CrudOption = new List<string>();
                            childNodeViewModel.Utility = new List<string>();
                            childNodeViewModel.Fields = new List<ChildNodeFiledViewModel>();
                            childNodeViewModel.Type = ModuleTypeMasterListV2.Where(e => e.Id == Convert.ToInt64(subChildNodeData.Type)).FirstOrDefault().TypeValue;

                            string[] crudTypesInner = subChildNodeData.CrudTypes.Split(",");

                            foreach (string CrudOpt in crudTypesInner)
                            {
                                if (CrudOpt.ToLower() != "null")
                                {
                                    childNodeViewModel.CrudOption.Add(CRUDOptMasterListV2.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                                }
                                else
                                {
                                    childNodeViewModel.CrudOption.Add("NULL");
                                }
                            }

                            string[] utilityTypesInner = subChildNodeData.UtilityTypes.Split(",");
                            foreach (string utilityType in utilityTypesInner)
                            {
                                if (utilityType.ToLower() != "5")
                                {
                                    childNodeViewModel.Utility.Add(UtilityMasterListV2.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                                }
                                else
                                {
                                    childNodeViewModel.Utility.Add("NULL");
                                }
                            }

                            List<SubModuleFormMaster> subModuleFormMastersChild = SubModuleFormMasterListV2.Where(e => e.ModuleGroupAccessID == childNodeViewModel.ID).ToList();
                            if (subModuleFormMastersChild.Count > 0)
                            {
                                childNodeViewModel.HasField = true;
                            }
                            foreach (var subModuleFormMastersDataV2 in subModuleFormMastersChild)
                            {
                                //komal 05 - 04 - 2019 Comment for solve error

                                ChildNodeFiledViewModel childNodeFieldViewModelV2 = new ChildNodeFiledViewModel();
                                FieldMaster fieldMasterChildV2 = FieldMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuleFormMastersDataV2.FieldID)).FirstOrDefault();
                                if (fieldMasterChildV2 != null)
                                {
                                    childNodeFieldViewModelV2.FieldName = fieldMasterChildV2.FieldName;
                                    childNodeFieldViewModelV2.FiledID = fieldMasterChildV2.Id;
                                    childNodeFieldViewModelV2.AccessRight = ModuleVisibilityMasterListV2.Where(e => e.Id == fieldMasterChildV2.AccressRight).FirstOrDefault().VisibilityValue;
                                    childNodeFieldViewModelV2.GUID = fieldMasterChildV2.GUID;
                                    childNodeFieldViewModelV2.Required = ModuleFieldRequirerMasterListV2.Where(e => e.Id == fieldMasterChildV2.Required).FirstOrDefault().Value;
                                    childNodeFieldViewModelV2.Visibility = ModuleAccessRightsMasterListV2.Where(e => e.Id == subModuleFormMastersDataV2.Visibility).FirstOrDefault().AccessRightValue;
                                    childNodeViewModel.Fields.Add(childNodeFieldViewModelV2);
                                }
                            }
                            if (ParentID != new Guid("00000000-0000-0000-0000-000000000000"))
                            {
                                InnerChildNodeViewModel SubMenu = this.GetSubMenuRecursive(childNodeViewModel.ID);
                                if (SubMenu != null)
                                {
                                    childNodeViewModel.HasChild = true;
                                    childNodeViewModel.ChildNodes = SubMenu.ChildNodes;
                                }
                                menuSubDetailViewModel.ChildNodes.Add(childNodeViewModel);
                            }
                        }
                    }
                    Response.Result.Modules.Add(menuSubDetailViewModel);
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<MenuAccessResponse> GetMasterListLight(Guid ParentID)
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            Response.Result = new MenuAccessDetailViewModel();
            Response.Result.Modules = new List<MenuSubDetailViewModel>();
            try
            {
                ModuleTypeMasterListV2 = _moduleTypeMaster.FindBy(e => e.Status == 1);
                CRUDOptMasterListV2 = _moduleCRUDOptMaster.FindBy(e => e.Status == 1);
                UtilityMasterListV2 = _moduleUtilityMaster.FindBy(e => e.Status == 1);
                SubModuleMasterListV2 = _SubModuleRepository.FindBy(e => e.Status == 1 && e.ParentGUID == ParentID);
                SubModuleMasterListV2Full = _SubModuleRepository.FindBy(e => e.Status == 1);
                SubModuleFormMasterListV2 = _subModuleFormMaster.FindBy(e => e.Status == 1);
                ModuleGroupAccessListV2 = _moduleGroupAccess.FindBy(e => e.Status == 1);
                FieldMasterListV2 = _fieldMaster.FindBy(e => e.Status == 1);
                ModuleVisibilityMasterListV2 = _moduleVisibilityMaster.FindBy(e => e.Status == 1);
                ModuleFieldRequirerMasterListV2 = _moduleFieldRequirerMaster.FindBy(e => e.Status == 1);
                ModuleAccessRightsMasterListV2 = _moduleAccessRightsMaster.FindBy(e => e.Status == 1);
                if (SubModuleMasterListV2 == null)
                {
                    Response.Result = null;
                    Response.ErrorCode = 0;
                    Response.ReturnCode = 0;
                    Response.ReturnMsg = "No record at all";
                    return Response;
                }
                Response.Result.GroupID = 0;
                Response.Result.Modules = new List<MenuSubDetailViewModel>();
                foreach (var modeulAccessListData in SubModuleMasterListV2)
                {
                    MenuSubDetailViewModel menuSubDetailViewModel = new MenuSubDetailViewModel();
                    menuSubDetailViewModel.ID = modeulAccessListData.Id;
                    menuSubDetailViewModel.Status = modeulAccessListData.Status;
                    menuSubDetailViewModel.ModuleName = modeulAccessListData.SubModuleName;
                    menuSubDetailViewModel.ModuelID = modeulAccessListData.ModuleID;
                    menuSubDetailViewModel.ParentID = modeulAccessListData.ParentID;
                    menuSubDetailViewModel.GUID = modeulAccessListData.GUID.ToString();
                    menuSubDetailViewModel.ParentGUID = modeulAccessListData.ParentGUID.ToString();
                    menuSubDetailViewModel.CrudOption = new List<string>();
                    menuSubDetailViewModel.Utility = new List<string>();
                    menuSubDetailViewModel.Fields = new List<ChildNodeFiledViewModel>();
                    menuSubDetailViewModel.ChildNodes = new List<ChildNodeViewModel>();
                    menuSubDetailViewModel.Type = ModuleTypeMasterListV2.Where(e => e.Id == Convert.ToInt64(modeulAccessListData.Type)).FirstOrDefault().TypeValue;//subModuleMaster.Type.ToString();

                    string[] crudTypes = modeulAccessListData.CrudTypes.Split(",");

                    foreach (string CrudOpt in crudTypes)
                    {
                        menuSubDetailViewModel.CrudOption.Add(CRUDOptMasterListV2.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                    }
                    string[] utilityTypes = modeulAccessListData.UtilityTypes.Split(",");

                    foreach (string utilityType in utilityTypes)
                    {
                        menuSubDetailViewModel.Utility.Add(UtilityMasterListV2.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                    }
                    List<SubModuleFormMaster> subModuleFormMastersV1 = SubModuleFormMasterListV2.Where(e => e.ModuleGroupAccessID == menuSubDetailViewModel.ID).ToList();

                    if (subModuleFormMastersV1.Count > 0)
                    {
                        menuSubDetailViewModel.HasField = true;
                    }
                    foreach (var subModuleFormMastersData in subModuleFormMastersV1)
                    {
                        //komal 05 - 04 - 2019 Comment for solve error

                        ChildNodeFiledViewModel childNodeFieldViewModelV1 = new ChildNodeFiledViewModel();
                        FieldMaster fieldMasterV1 = FieldMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuleFormMastersData.FieldID)).FirstOrDefault();
                        if (fieldMasterV1 != null)
                        {
                            menuSubDetailViewModel.HasChild = true;
                            childNodeFieldViewModelV1.FieldName = fieldMasterV1.FieldName;
                            childNodeFieldViewModelV1.FiledID = fieldMasterV1.Id;
                            childNodeFieldViewModelV1.AccessRight = ModuleVisibilityMasterListV2.Where(e => e.Id == fieldMasterV1.AccressRight).FirstOrDefault().VisibilityValue;
                            childNodeFieldViewModelV1.GUID = fieldMasterV1.GUID;
                            childNodeFieldViewModelV1.Required = ModuleFieldRequirerMasterListV2.Where(e => e.Id == fieldMasterV1.Required).FirstOrDefault().Value;
                            childNodeFieldViewModelV1.Visibility = ModuleAccessRightsMasterListV2.Where(e => e.Id == subModuleFormMastersData.Visibility).FirstOrDefault().AccessRightValue;
                            menuSubDetailViewModel.Fields.Add(childNodeFieldViewModelV1);
                        }
                    }
                    List<SubModuleMaster> subModuleMasterChild = SubModuleMasterListV2Full.Where(e => e.ParentID == menuSubDetailViewModel.ID).ToList();
                    if (subModuleMasterChild.Count > 0)
                    {
                        menuSubDetailViewModel.HasChild = true;
                        foreach (var subChildNodeData in subModuleMasterChild)
                        {
                            ChildNodeViewModel childNodeViewModel = new ChildNodeViewModel();
                            childNodeViewModel.ID = subChildNodeData.Id;
                            childNodeViewModel.Status = subChildNodeData.Status;
                            childNodeViewModel.ModuleName = subChildNodeData.SubModuleName;
                            childNodeViewModel.ModuelID = subChildNodeData.ModuleID;
                            childNodeViewModel.ParentID = subChildNodeData.ParentID;
                            childNodeViewModel.GUID = subChildNodeData.GUID.ToString();
                            childNodeViewModel.ParentGUID = subChildNodeData.ParentGUID.ToString();
                            childNodeViewModel.ChildNodes = new List<InnerChildNodeViewModel>();
                            childNodeViewModel.CrudOption = new List<string>();
                            childNodeViewModel.Utility = new List<string>();
                            childNodeViewModel.Fields = new List<ChildNodeFiledViewModel>();
                            childNodeViewModel.Type = ModuleTypeMasterListV2.Where(e => e.Id == Convert.ToInt64(subChildNodeData.Type)).FirstOrDefault().TypeValue;

                            string[] crudTypesInner = subChildNodeData.CrudTypes.Split(",");

                            foreach (string CrudOpt in crudTypesInner)
                            {
                                if (CrudOpt.ToLower() != "null")
                                {
                                    childNodeViewModel.CrudOption.Add(CRUDOptMasterListV2.Where(e => e.Id == Convert.ToInt64(CrudOpt)).FirstOrDefault().OptValue);
                                }
                                else
                                {
                                    childNodeViewModel.CrudOption.Add("NULL");
                                }
                            }

                            string[] utilityTypesInner = subChildNodeData.UtilityTypes.Split(",");
                            foreach (string utilityType in utilityTypesInner)
                            {
                                if (utilityType.ToLower() != "5")
                                {
                                    childNodeViewModel.Utility.Add(UtilityMasterListV2.Where(e => e.Id == Convert.ToInt64(utilityType)).FirstOrDefault().UtilityValue);
                                }
                                else
                                {
                                    childNodeViewModel.Utility.Add("NULL");
                                }
                            }

                            List<SubModuleFormMaster> subModuleFormMastersChild = SubModuleFormMasterListV2.Where(e => e.ModuleGroupAccessID == childNodeViewModel.ID).ToList();
                            if (subModuleFormMastersChild.Count > 0)
                            {
                                childNodeViewModel.HasField = true;
                            }
                            foreach (var subModuleFormMastersDataV2 in subModuleFormMastersChild)
                            {
                                //komal 05 - 04 - 2019 Comment for solve error

                                ChildNodeFiledViewModel childNodeFieldViewModelV2 = new ChildNodeFiledViewModel();
                                FieldMaster fieldMasterChildV2 = FieldMasterListV2.Where(e => e.Id == Convert.ToInt64(subModuleFormMastersDataV2.FieldID)).FirstOrDefault();
                                if (fieldMasterChildV2 != null)
                                {
                                    childNodeFieldViewModelV2.FieldName = fieldMasterChildV2.FieldName;
                                    childNodeFieldViewModelV2.FiledID = fieldMasterChildV2.Id;
                                    childNodeFieldViewModelV2.AccessRight = ModuleVisibilityMasterListV2.Where(e => e.Id == fieldMasterChildV2.AccressRight).FirstOrDefault().VisibilityValue;
                                    childNodeFieldViewModelV2.GUID = fieldMasterChildV2.GUID;
                                    childNodeFieldViewModelV2.Required = ModuleFieldRequirerMasterListV2.Where(e => e.Id == fieldMasterChildV2.Required).FirstOrDefault().Value;
                                    childNodeFieldViewModelV2.Visibility = ModuleAccessRightsMasterListV2.Where(e => e.Id == subModuleFormMastersDataV2.Visibility).FirstOrDefault().AccessRightValue;
                                    childNodeViewModel.Fields.Add(childNodeFieldViewModelV2);
                                }
                            }
                            menuSubDetailViewModel.ChildNodes.Add(childNodeViewModel);
                            //if (ParentID != new Guid("00000000-0000-0000-0000-000000000000"))
                            //{
                            //    InnerChildNodeViewModel SubMenu = this.GetSubMenuRecursive(childNodeViewModel.ID);
                            //    if (SubMenu != null)
                            //    {
                            //        childNodeViewModel.HasChild = true;
                            //        childNodeViewModel.ChildNodes = SubMenu.ChildNodes;
                            //    }
                            //    menuSubDetailViewModel.ChildNodes.Add(childNodeViewModel);
                            //}
                        }
                    }

                    Response.Result.Modules.Add(menuSubDetailViewModel);
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<MenuAccessResponse> GetDefaultMenusV2()
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            Response.Result = new MenuAccessDetailViewModel();
            Response.Result.Modules = new List<MenuSubDetailViewModel>();
            SubModuleMaster subM = new SubModuleMaster();
            try
            {
                ModuleTypeMasterListV2 = _moduleTypeMaster.FindBy(e => e.Status == 1);
                CRUDOptMasterListV2 = _moduleCRUDOptMaster.FindBy(e => e.Status == 1);
                UtilityMasterListV2 = _moduleUtilityMaster.FindBy(e => e.Status == 1);
                moduleMasterListV2 = _SubModuleRepository.FindBy(e => e.Status == 1 && e.ParentID == 0);
                SubModuleMasterListV2 = _SubModuleRepository.FindBy(e => e.Status == 1);
                SubModuleFormMasterListV2 = _subModuleFormMaster.FindBy(e => e.Status == 1);
                FieldMasterListV2 = _fieldMaster.FindBy(e => e.Status == 1);
                ModuleVisibilityMasterListV2 = _moduleVisibilityMaster.FindBy(e => e.Status == 1);
                ModuleFieldRequirerMasterListV2 = _moduleFieldRequirerMaster.FindBy(e => e.Status == 1);
                ModuleAccessRightsMasterListV2 = _moduleAccessRightsMaster.FindBy(e => e.Status == 1);
                if (moduleMasterListV2 == null || SubModuleMasterListV2 == null)
                {
                    Response.Result = null;
                    Response.ErrorCode = 0;
                    Response.ReturnCode = 0;
                    Response.ReturnMsg = "No record at all";
                    return Response;
                }
                Response.Result.GroupID = 0;
                Response.Result.Modules = new List<MenuSubDetailViewModel>();
                Response.Result.Modules.Add((MenuSubDetailViewModel)moduleMasterListV2);
                //return DepthFirstTreeTraversal(start, c=>c.Children).ToList();
                //return DepthFirstTreeTraversal(subM, c =>c.Id).ToList();

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        static IEnumerable<T> DepthFirstTreeTraversal<T>(T root, Func<T, IEnumerable<T>> children)
        {
            var stack = new Stack<T>();
            stack.Push(root);
            while (stack.Count != 0)
            {
                var current = stack.Pop();
                // If you don't care about maintaining child order then remove the Reverse.
                foreach (var child in children(current).Reverse())
                    stack.Push(child);
                yield return current;
            }
        }

        //khushali 30-04-2019  optimization Group Access Right
        public async Task<MenuAccessResponse> GetGroupAccessRightsGroupWise(long GroupID, string ParentID, bool IscCheckStatus = false, bool HasChildMenu = false)
        {
            MenuAccessResponse Response = new MenuAccessResponse();
            Response.Result = new MenuAccessDetailViewModel();
            Response.Result.Modules = new List<MenuSubDetailViewModel>();
            try
            {
                Response.Result.GroupID = GroupID;
                var Result = _controlPanelRepository.GetGroupAccessRightsGroupWise(GroupID, ParentID, IscCheckStatus);
                foreach (var Data in Result)
                {
                    var NewModule = new MenuSubDetailViewModel()
                    {
                        CrudOption = new List<string>(),
                        Utility = new List<string>(),
                        Fields = new List<ChildNodeFiledViewModel>(),
                        ChildNodes = new List<ChildNodeViewModel>(),
                        GUID = Data.GUID.ToString(),
                        ParentGUID = Data.ParentGUID.ToString(),
                        ParentID = Data.ParentID,
                        HasChild = Data.HasChild == 1 ? true : false,
                        HasField = Data.HasField == 1 ? true : false,
                        ID = Data.ID,
                        ModuelID = Data.ModuelID,
                        ModuleName = Data.ModuleName,
                        Status = Data.Status,
                        Type = Data.Type,
                        HasNext = Data.Status == 1 ? true : false,
                        ModuleGroupAccessID = Data.ModuleGroupAccessID
                    };
                    if (!string.IsNullOrEmpty(Data.CrudOption))
                    {
                        NewModule.CrudOption = Data.CrudOption.Split(',').ToList();
                    }
                    if (!string.IsNullOrEmpty(Data.Utility))
                    {
                        NewModule.Utility = Data.Utility.Split(',').ToList();
                    }
                    if (NewModule.HasField)
                    {
                        var GetFeildData = _controlPanelRepository.GetFormAccessRightsGroupWise(Data.ModuleGroupAccessID, IscCheckStatus);
                        if (GetFeildData != null && GetFeildData.Count() > 0)
                        {
                            NewModule.Fields = GetFeildData;
                        }
                    }

                    if (NewModule.HasChild && HasChildMenu)
                    {
                        var Result1 = _controlPanelRepository.GetGroupAccessRightsGroupWise(GroupID, Data.GUID.ToString(), IscCheckStatus);
                        foreach (var Data1 in Result1)
                        {
                            var NewModule1 = new ChildNodeViewModel()
                            {
                                CrudOption = new List<string>(),
                                Utility = new List<string>(),
                                Fields = new List<ChildNodeFiledViewModel>(),
                                GUID = Data1.GUID.ToString(),
                                ParentGUID = Data1.ParentGUID.ToString(),
                                ParentID = Data1.ParentID,
                                HasChild = Data1.HasChild == 1 ? true : false,
                                HasField = Data1.HasField == 1 ? true : false,
                                ID = Data1.ID,
                                ModuelID = Data1.ModuelID,
                                ModuleName = Data1.ModuleName,
                                Status = Data1.Status,
                                Type = Data1.Type,
                                ChildNodes = new List<InnerChildNodeViewModel>()
                            };
                            if (!string.IsNullOrEmpty(Data1.CrudOption))
                            {
                                NewModule1.CrudOption = Data1.CrudOption.Split(',').ToList();
                            }
                            if (!string.IsNullOrEmpty(Data1.Utility))
                            {
                                NewModule1.Utility = Data1.Utility.Split(',').ToList();
                            }
                            NewModule.ChildNodes.Add(NewModule1);
                        }
                    }
                    Response.Result.Modules.Add(NewModule);
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> UpdateModuleGroupAccess(List<MenuSubDetail> Data, long GroupID, long UserID)
        {
            BizResponseClass Resp = new BizResponseClass();
            string CrudOption = string.Empty;
            string Utility = string.Empty;
            try
            {
                var CrudOptionList = _moduleCRUDOptMaster.ListAllAsync().Result;
                var UtilityList = _moduleUtilityMaster.ListAllAsync().Result;

                foreach (var AccessRight in Data)
                {
                    var DifCrudOptionList = CrudOptionList.Where(a => AccessRight.CrudOption.Any(a1 => a1 == a.OptValue)).Select(e => e.Id.ToString()).ToList();
                    var DifUtilityList = UtilityList.Where(a => AccessRight.Utility.Any(a1 => a1 == a.UtilityValue)).Select(e => e.Id.ToString()).ToList();
                    if (DifCrudOptionList.Count > 0)
                    {
                        CrudOption = String.Join(",", DifCrudOptionList);
                    }
                    else
                    {
                        CrudOption = "100";
                    }
                    if (DifUtilityList.Count > 0)
                    {
                        Utility = String.Join(",", DifUtilityList);
                    }
                    else
                    {
                        Utility = "100";
                    }
                    var Result = _controlPanelRepository.UpdateModuleGroupAccess(UserID, GroupID, AccessRight.ModuleGroupAccessID, AccessRight.Status, CrudOption, Utility);
                }
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> UpdateModuleFieldpAccess(List<ChildNodeFiled> Data, long UserID)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var Visibility = _moduleVisibilityMaster.ListAllAsync().Result;
                var AccessRights = _moduleAccessRightsMaster.ListAllAsync().Result;
                foreach (var AccessRight in Data)
                {
                    var DifVisibilityValue = Visibility.Where(a => a.VisibilityValue == AccessRight.Visibility).Select(e => e.Id).FirstOrDefault();
                    var DifAccessRightsValue = AccessRights.Where(a => a.AccessRightValue == AccessRight.AccessRights).Select(e => e.Id).FirstOrDefault();
                    _controlPanelRepository.UpdateModuleFieldpAccess(UserID, AccessRight.ModuleGroupAccessID, AccessRight.ModuleFormAccessID, Convert.ToInt16(DifVisibilityValue), Convert.ToInt16(DifAccessRightsValue));
                }
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> UpdateFieldDataTest(FieldDataViewModelv1 request, long UserId)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                var model = _fieldMaster.FindBy(e => e.GUID == request.GUID).FirstOrDefault();
                if (model == null)
                {
                    Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Response;
                }
                var updatedata = _subModuleFormMaster.FindBy(e => Convert.ToInt64(e.FieldID) == model.Id).FirstOrDefault();

                if (request.FieldName != null)
                {
                    model.FieldName = request.FieldName;
                }
                if (request.Status != 99)
                {
                    model.Status = Convert.ToInt16(request.Status);
                    updatedata.Visibility = request.Status;
                }
                if (request.Required != 99)
                {
                    model.Required = request.Required;
                }
                if (request.AccressRight != 99)
                {
                    model.AccressRight = request.AccressRight;
                    updatedata.Status = model.AccressRight;
                }

                model.UpdatedDate = Helpers.UTC_To_IST();
                model.UpdatedBy = 999;
                await _fieldMaster.UpdateAsync(model);
                await _subModuleFormMaster.UpdateAsync(updatedata);

                Response.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                Response.ReturnCode = enResponseCode.Success;
                Response.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                return Response;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> UpdateSubmoduleDataTest(SubModuleDataViewModelv1 request, long UserId)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                var model = _SubModuleRepository.FindBy(e => e.GUID == request.GUID).FirstOrDefault();
                if (model == null)
                {
                    Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Response;
                }
                var updatedata = _moduleGroupAccess.FindBy(e => Convert.ToInt64(e.SubModuleID) == model.Id).FirstOrDefault();

                if (request.SubModuleName != null)
                {
                    model.SubModuleName = request.SubModuleName;
                }
                if (request.Status != 99)
                {
                    model.Status = Convert.ToInt16(request.Status);
                    updatedata.Status = request.Status;
                }
                if (request.UtilityTypes != null)
                {
                    model.UtilityTypes = request.UtilityTypes;
                    updatedata.UtilityTypes = request.UtilityTypes;
                }
                if (request.CrudTypes != null)
                {
                    model.CrudTypes = request.CrudTypes;
                    updatedata.CrudTypes = model.CrudTypes;
                }
                if (request.Type != 99)
                {
                    model.Type = request.Type;
                }
                if (request.ParentGUID != null)
                {
                    model.ParentGUID = request.ParentGUID;
                }

                model.UpdatedDate = Helpers.UTC_To_IST();
                model.UpdatedBy = 999;
                await _SubModuleRepository.UpdateAsync(model);
                await _moduleGroupAccess.UpdateAsync(updatedata);

                Response.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                Response.ReturnCode = enResponseCode.Success;
                Response.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                return Response;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
    }
}
