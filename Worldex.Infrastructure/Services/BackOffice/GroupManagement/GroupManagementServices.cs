using Worldex.Core.ApiModels;
using Worldex.Core.Entities.GroupModuleManagement;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.BackOffice;
using Worldex.Core.Interfaces.ControlPanel;
using Worldex.Core.Interfaces.RoleManagement;
using Worldex.Core.ViewModels.GroupManagement;
using Worldex.Core.ViewModels.RoleConfig;
using Worldex.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.BackOffice.GroupManagement
{
    public class GroupManagementServices : BasePage, IGroupManagementServices
    {
        private readonly IAsyncRepositoryV5<ModuleGroupMaster> _ModuleGroupMaster;
        private readonly IControlPanelRepository _controlPanelRepository; // khushali 29-04-2019 initial level master entry for group access right 

        public GroupManagementServices(ILogger<BasePage> logger, IAsyncRepositoryV5<ModuleGroupMaster> ModuleGroupMaster, 
            IControlPanelRepository controlPanelRepository) : base(logger)
        {
            _ModuleGroupMaster = ModuleGroupMaster;
            _controlPanelRepository = controlPanelRepository;
        }

        public async Task<BizResponseClass> CreateGroupAsync(CreateGroupRequest Req, long UserId)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                //bool IsGroupExist = _ModuleGroupMaster.IsExist(Req.GroupName);
                //var mGroupMaster = await _ModuleGroupMaster.FindByAsync(g => g.GroupName.ToLower().Equals(Req.GroupName.ToLower()));
                var checkExist = await _ModuleGroupMaster.IsExist(g => g.GroupName.ToLower().Equals(Req.GroupName.ToLower()));
                //if (IsGroupExist == true)
                if (checkExist != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.Alredy_Exist;
                    Resp.ErrorCode = enErrorCode.RecordAlreadyExist;
                }
                else
                {
                    ModuleGroupMaster moduleGroupMaster = new ModuleGroupMaster
                    {
                        CreatedBy = UserId,
                        CreatedDate = UTC_To_IST(), 
                        GroupName = Req.GroupName,
                        ModuleDomainID = Req.DomainID,
                        //RoleID = Req.RoleID,
                        //Status = Convert.ToInt16(Req.Status),
                        Status = 1,
                        GroupDescription = Req.Description
                    };
                    await _ModuleGroupMaster.AddAsync(moduleGroupMaster);

                    // khushali 29-04-2019 initial level master entry for group access right
                    if(moduleGroupMaster.Id != 0)
                    {
                        var Res = await _controlPanelRepository.Callsp_AddGroupAccessRight(moduleGroupMaster.Id,UserId);
                    }

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordCreated;
                    Resp.ErrorCode = enErrorCode.RecordCreatedSuccessfully;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateGroupAsync", "GroupManagementServices", ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> ChangeGroupAsync(ChangeGroupRequest Req, long UserId)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();                
                var checkExist = await _ModuleGroupMaster.IsExist(g => g.Id == Req.GroupID);

                if (checkExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.RoleNotExist;
                    return Resp;
                }

                // Removed below condition as Disabled group won't able to get Active.
                //if (checkExist.Status != Convert.ToInt16(ServiceStatus.Active))
                //{
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.RoleIsDisabled;
                //    Resp.ErrorCode = enErrorCode.RoleIsDisabled;
                //    return Resp;
                //}

                // Below code removed as change of status was denied on same group.
                //var IsDuplicate = await _ModuleGroupMaster.IsExist(g => g.GroupName.ToLower().Equals(Req.GroupName.ToLower()));
                //if (IsDuplicate != null)
                //{
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.RoleNameAlreadyExist;
                //    Resp.ErrorCode = enErrorCode.RoleNameAlreadyExist;
                //    return Resp;
                //}
                
                // Need to confirm whether we will allow to change group name or not.
                checkExist.GroupName = Req.GroupName;
                checkExist.GroupDescription = Req.Description;
                checkExist.Status = Convert.ToInt16(Req.Status);
                //checkExist.RoleID = (int)Req.RoleId;
                checkExist.ModuleDomainID = Req.DomainID;
                checkExist.UpdatedBy = UserId;
                checkExist.UpdatedDate = UTC_To_IST();
                await _ModuleGroupMaster.UpdateAsync(checkExist);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateGroupAsync", "GroupManagementServices", ex);
                throw ex;
            }
        }

        public async Task<GroupList> GetGroupList()
        {
            try
            {
                GroupList Resp = new GroupList();     
                Resp.GroupListData = new List<ChangeGroupRequest>();

                var getList = await _ModuleGroupMaster.ListAllAsync();

                if (getList == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.RoleNotExist;
                    return Resp;
                }

                foreach(var data in getList)
                {
                    ChangeGroupRequest moduleGroupData = new ChangeGroupRequest();
                    moduleGroupData.Description = data.GroupDescription;
                    moduleGroupData.GroupName = data.GroupName;
                    moduleGroupData.GroupID = (int)data.Id;
                    moduleGroupData.DomainID = data.ModuleDomainID;
                    moduleGroupData.RoleId = data.RoleID;
                    moduleGroupData.Status = (ServiceStatus)data.Status;

                    Resp.GroupListData.Add(moduleGroupData);
                }
                
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.ErrorCode = enErrorCode.RecordFoundSuccessfully;
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateGroupAsync", "GroupManagementServices", ex);
                throw ex;
            }
        }
    }
}
