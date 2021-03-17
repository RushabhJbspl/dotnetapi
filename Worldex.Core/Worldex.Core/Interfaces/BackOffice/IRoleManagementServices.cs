using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.BackOffice;
using Worldex.Core.ViewModels.GroupManagement;
using Worldex.Core.ViewModels.RoleConfig;

namespace Worldex.Core.Interfaces.BackOffice
{
    public interface IRoleManagementServices
    {
        Task<BizResponseClass> CreateRole(CreateRoleReq Req, long UserId);
        Task<BizResponseClass> UpdateRoleDetail(CreateRoleReq req, long roleId, long id);
        Task<ListRoleDetail> ListRoleDetail(int PageNo, int PageSize, short? status,short? AllRecords);
        Task<BizResponseClass> ChangeUserRoleStatus(long roleId, ServiceStatus status, long id);
        Task<BizResponseClass> AssignRoleDetail(long roleId, long userId, long id, bool IsRemoveRole = false);
        Task<BizResponseClass> CreatePermissionGroup(CreatePermissionGrpReq req, long id);
        Task<BizResponseClass> ChangePermissionGroupStatus(long permissionGrpID, ServiceStatus status, long id);
        Task<BizResponseClass> UpdatePermissionGroup(long PermissionGrpID, CreatePermissionGrpReq req, long userid);
        Task<ListPermissionGrp> ListPermissionGroup(int PageNo, int PageSize, short? AllRecords,DateTime? FromDate, DateTime? Todate, long? RoleId, short? Status);
        Task<ListRoleHistoryData> GetRoleHistory(int PageNo, int PageSize, long? UserId, DateTime? FromDate, DateTime? ToDate, long? ModuleId,short? Status);
        Task<GetRoleDetail22> GetRoleDetail(long roleId);
        Task<GetPermissionGroupRes> GetPermissionGroupDetailById(long permissionGroupId);
        Task<BizResponseClass> CreateNewUser(CreateUserReq req, long id);
        Task<BizResponseClass> EditUserDetail(EditUserReq req, long id);
        Task<BizResponseClass> ChangeUserStatus(long userId, EnMyAccountUserStatus status);
        // Task<ListUserDetails> ListUserDetail(int PageNo, int PageSize, short? AllRecords);
        Task<ListUserDetails> ListUserDetail(int PageNo, int PageSize, short? AllRecords, string UserName = null, string Mobile = null, string Email = null, DateTime? FromDate = null, DateTime? ToDate = null, int IsAdmin = 9);//add by mansi
        Task<GetUserDetailResp> GetUserDetailById(long userId);
        Task<ListUserDetails2> SearchUserDetail(string searchText);
        Task<GetPermissionGroupIDByLinkedRoleRes> GetPermissionGroupIDByLinkedRole(long RoleId); // khushali 04-03-2019  -- Get permission Group ID by Linked Role
        Task<ListViewUnAssignedUserRes> ViewUnassignedUsers(int PageNo, int PageSize, string UserName, DateTime? FromDate, DateTime? ToDate, short? Status);// khushali 04-03-2019  -- View Unassigned Users        
        Task<MenuAccessResponse> GetMenuWithDetailsAsync(long GroupID);
        bool CheckUserHaveMenuAccess(long GroupID, string MethodName, string ControllerName);
        bool IsSkipAllow(string MethodName, string ControllerName);
        Task<BizResponseClass> CreatePermissionGroupV1(CreateGroupRequest req, long id);
        Task<BizResponseClass> UpdatePermissionGroupV1(CreateGroupRequest req, long userid);
        Task<MenuAccessResponse> GetMenuWithDetailsAsyncV2(long GroupID);
        Task<MenuAccessResponse> GetDefaultMenus();
        Task<MenuAccessResponse> GetMenuWithDetailsAsyncV2MainMenu(long GroupID, Guid ParentID);
        Task<MenuAccessResponse> GetMasterList(Guid ParentID);
        Task<MenuAccessResponse> GetMasterListLight(Guid ParentID);
        Task<MenuAccessResponse> GetDefaultMenusV2();
        Task<MenuAccessResponse> GetGroupAccessRightsGroupWise(long GroupID, string ParentID, bool IscCheckStatus = false, bool HasChildMenu = false);
        Task<BizResponseClass> UpdateModuleGroupAccess(List<MenuSubDetail> Data, long GroupID, long UserID);
        Task<BizResponseClass> UpdateModuleFieldpAccess(List<ChildNodeFiled> Data, long UserID);
        Task<BizResponseClass> UpdateFieldDataTest(FieldDataViewModelv1 request, long UserId);
        Task<BizResponseClass> UpdateSubmoduleDataTest(SubModuleDataViewModelv1 request, long UserId);
    }

    public interface IRuleManageService
    {
        //khushali 13-02-2019 CURD for Velocity Rule
        Task<UserAccessRightsViewModel> GetMenuAssignedRule(long GroupID);
        Task<UserAccessRightsViewModel> GetAssignedRule(long GroupID);
        Task<UserAccessRightsViewModel> GetAssignedRuleV1(long GroupID);
        Task<UserAccessRightsViewModel> GetConfigurePermissions();
        Task<UserAccessRightViewModel> CreateAssignedRule(long GroupID);
        Task<BizResponseClass> CreateAssignedRuleV2(UserAccessRightsViewModel Request);
        Task<BizResponseClass> UpdateAssignedRule(UserAccessRightsViewModel Request);
        //Task<List<ModuleDataViewModel>> GetAllModuleData();
        Task<ListModuleDataViewModel> GetAllModuleData(int PageNo, int PageSize, short? AllData = null,bool IsParentList = false);
        Task<ListModuleDataViewModel> GetAllModuleDataV2(int PageNo, int PageSize);
        Task<ModuleDataViewModel> GetModuleDataByID(long Id);
        Task<long> AddModuleData(ModuleDataViewModel request, long UserId);
        Task<BizResponseClass> UpdateModuleData(ModuleDataViewModel request, long UserId);
        Task<bool> ChangeStatusModuleData(long id, short Status);
        Task<ListSubModuleDataViewModel> GetAllSubModuleData(int PageNo, int PageSize, short? AllData = null, bool IsParentList = false);
        Task<SubModuleDataViewModel> GetSubModuleDataByID(long Id);
        Task<long> AddSubModuleData(SubModuleDataViewModel request, long UserId);
        Task<BizResponseClass> UpdateSubModuleData(SubModuleDataViewModel request, long UserId);
        Task<bool> ChangeStatusSubModuleData(long id, short Status);
        Task<ListFieldDataViewModel> GetAllFieldData(int PageNo, int PageSize, short? AllData = null);
        Task<FieldDataViewModel> GetFieldDataByID(long Id);
        Task<long> AddFieldData(FieldDataViewModel request, long UserId);
        Task<BizResponseClass> UpdateFieldData(FieldDataViewModel request, long UserId);
        Task<bool> ChangeStatusFieldData(long id, short Status);
        Task<ListToolDataViewModel> GetAllToolData(int PageNo, int PageSize,short? AllData = null);
        Task<ToolDataViewModel> GetToolDataByID(long Id);
        Task<long> AddToolData(ToolDataViewModel request, long UserId);
        Task<BizResponseClass> UpdateToolData(ToolDataViewModel request, long UserId);
        Task<bool> ChangeStatusToolData(long id, short Status);        
    }

    public interface IGroupManagementServices
    {
        Task<BizResponseClass> CreateGroupAsync(CreateGroupRequest Req, long UserId);
        Task<BizResponseClass> ChangeGroupAsync(ChangeGroupRequest Req, long UserId);
        Task<GroupList> GetGroupList();
    }
}
