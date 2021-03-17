using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Backoffice.RoleManagement;
using Worldex.Core.Entities.GroupModuleManagement;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.BackOffice;
using Worldex.Core.Interfaces.RoleManagement;
using Worldex.Core.ViewModels.BackOffice;
using Worldex.Infrastructure.Data;
using Worldex.Infrastructure.Data.RoleManagement;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.BackOffice
{
    public class RuleManageService : IRuleManageService
    {
        private readonly IAsyncRepository<UserAccessRights> _roleRepository;
        private readonly IAsyncRepository<ModuleMaster> _ModuleRepository;
        private readonly IAsyncRepository<SubModuleMaster> _SubModuleRepository;
        private readonly IAsyncRepository<FieldMaster> _FieldRepository;
        private readonly IAsyncRepository<ToolMaster> _ToolRepository;
        private readonly IAsyncRepositoryV1<UserAssignModule> _userAssignModuleRepository;
        private readonly IAsyncRepositoryV2<UserAssignSubModule> _userAssignSubModuleRepository;
        private readonly IAsyncRepositoryV3<UserAssignFieldRights> _userAssignFieldRepository; 
        private readonly IAsyncRepositoryV4<UserAssignToolRights> _userAssignToolRepository;
        private readonly EFCommonRepository<ModuleGroupMaster> _ModuleGroupMasterRepository;
        private readonly ControlPanelRepository _ControlPanelRepository;


        public RuleManageService(IAsyncRepository<UserAccessRights> RoleRepository, IAsyncRepository<ModuleMaster> ModuleRepository,
            IAsyncRepository<SubModuleMaster> SubModuleRepository , IAsyncRepository<FieldMaster> FieldRepository ,
            IAsyncRepository<ToolMaster> ToolRepository, IAsyncRepositoryV1<UserAssignModule> UserAssignModuleRepository, 
            IAsyncRepositoryV2<UserAssignSubModule> userAssignSubModuleRepository, IAsyncRepositoryV3<UserAssignFieldRights> userAssignFieldRepository,
            IAsyncRepositoryV4<UserAssignToolRights> userAssignToolRepository, ControlPanelRepository ControlPanelRepository,
            EFCommonRepository<ModuleGroupMaster> ModuleGroupMasterRepository)
        {
            _roleRepository = RoleRepository;
            _ModuleRepository = ModuleRepository;
            _SubModuleRepository = SubModuleRepository;
            _FieldRepository = FieldRepository;
            _ToolRepository = ToolRepository;
            _userAssignModuleRepository = UserAssignModuleRepository;
            _userAssignSubModuleRepository = userAssignSubModuleRepository;
            _userAssignFieldRepository = userAssignFieldRepository;
            _userAssignToolRepository = userAssignToolRepository;
            _ControlPanelRepository = ControlPanelRepository;
            _ModuleGroupMasterRepository = ModuleGroupMasterRepository;
        }

        //khushali 21-02-2019
        #region "Access Rules Integration With Permission Group"

        public async Task<UserAccessRightsViewModel> GetAssignedRule(long GroupID)
        {
            UserAccessRightsViewModel Response = new UserAccessRightsViewModel();
            try
            {
                List<UserAccessRights> Data = await _roleRepository.ListAsync(new UserAccessRightsSpecification(GroupID));
                if (Data != null)
                {
                    if (Data.Count > 0)
                    {
                        // return Data[0];
                        UserAccessRights Record = Data[0];
                        Response.Modules = new List<UserAssignModuleViewModel>();
                        Response.GroupID = GroupID;
                        Response.ID = Record.Id;
                        Record.Modules = Record.Modules.OrderBy(o => o.ModuleName).ToList();
                        //var modules = await _ModuleRepository.ListAllAsync();
                        foreach (var m in Record.Modules)
                        {
                            //if (m.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                            //{
                                UserAssignModuleViewModel userAssignModule = new UserAssignModuleViewModel();
                                userAssignModule.SubModules = new List<UserAssignSubModuleViewModel>();
                                userAssignModule.ModuleID = m.ModuleID;
                                userAssignModule.Status = m.Status;
                                userAssignModule.ID = m.ID;
                                userAssignModule.ModuleName = m.ModuleName;
                                userAssignModule.ParentID = m.ParentID;
                                m.SubModules = m.SubModules.OrderBy(o => o.SubModuleName).ToList();
                                //var submodules = await _SubModuleRepository.ListAsync(new ModuleSpecification2(m.Id));
                                foreach (var s in m.SubModules)
                                {
                                    //if (s.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                                    //{
                                    UserAssignSubModuleViewModel userAssignsubModule = new UserAssignSubModuleViewModel();
                                    userAssignsubModule.Fields = new List<UserAssignFieldRightsViewModel>();
                                    userAssignsubModule.Tools = new List<UserAssignToolRightsViewModel>();
                                    userAssignsubModule.SubModuleID = s.SubModuleID;
                                    userAssignsubModule.ID = s.ID;
                                    userAssignsubModule.View = 0;
                                    userAssignsubModule.Edit = 0;
                                    userAssignsubModule.Delete = 0;
                                    userAssignsubModule.Create = 0;
                                    userAssignsubModule.Status = s.Status;
                                    userAssignsubModule.ParentID = s.ParentID;
                                    userAssignsubModule.SubModuleName = s.SubModuleName;
                                    //var Fields = await _FieldRepository.ListAsync(new ModuleSpecification3(s.Id));
                                    //var Tools = await _ToolRepository.ListAsync(new ModuleSpecification4(s.Id));
                                    s.Fields = s.Fields.OrderBy(o => o.FieldName).ToList();
                                    s.Tools = s.Tools.OrderBy(o => o.ToolName).ToList();
                                    foreach (var a in s.Fields)
                                    {
                                        //if (a.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                                        //{
                                        var k = new UserAssignFieldRightsViewModel
                                        {
                                            FieldID = a.FieldID,
                                            Status = a.Status,
                                            FieldName = a.FieldName,
                                            ID = a.ID
                                        };
                                        userAssignsubModule.Fields.Add(k);
                                        //}
                                    }
                                    foreach (var b in s.Tools)
                                    {
                                        //if (b.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                                        //{
                                        var l = new UserAssignToolRightsViewModel
                                        {
                                            ToolID = b.ToolID,
                                            Status = b.Status,
                                            ToolName = b.ToolName,
                                            ID = b.ID
                                        };
                                        userAssignsubModule.Tools.Add(l);
                                        //}
                                    }
                                    userAssignModule.SubModules.Add(userAssignsubModule);
                                    //}

                                }
                                Response.Modules.Add(userAssignModule);
                            //}
                        }
                        return Response;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<UserAccessRightsViewModel> GetAssignedRuleV1(long GroupID)
        {
            UserAccessRightsViewModel Response = new UserAccessRightsViewModel();
            try
            {
                List<UserAccessRights> Data = await _roleRepository.ListAsync(new UserAccessRightsSpecification(GroupID));
                if(Data != null)
                {
                    if(Data.Count > 0)
                    {
                        // return Data[0];
                        UserAccessRights Record = Data[0];
                        Response.Modules = new List<UserAssignModuleViewModel>();
                        Response.GroupID = GroupID;
                        Response.ID = Record.Id;
                        //Record.Modules = Record.Modules.Where(o => o.ParentID != 0).ToList();
                        //var modules = await _ModuleRepository.ListAllAsync();
                        Record.Modules = Record.Modules.OrderBy(o => o.ModuleName).ToList();
                        foreach (var m in Record.Modules)
                        {
                            //if (m.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                            //{
                            UserAssignModuleViewModel userAssignModule = new UserAssignModuleViewModel();
                            userAssignModule.SubModules = new List<UserAssignSubModuleViewModel>();
                            userAssignModule.ModuleID = m.ModuleID;
                            userAssignModule.Status = m.Status;
                            userAssignModule.ID = m.ID;
                            userAssignModule.ModuleName = m.ModuleName;
                            userAssignModule.ParentID = m.ParentID;
                            //var submodules = await _SubModuleRepository.ListAsync(new ModuleSpecification2(m.Id));
                            m.SubModules = m.SubModules.OrderBy(o => o.SubModuleName).ToList();
                            foreach (var s in m.SubModules)
                            {
                                //if (s.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                                //{
                                UserAssignSubModuleViewModel userAssignsubModule = new UserAssignSubModuleViewModel();
                                userAssignsubModule.Fields = new List<UserAssignFieldRightsViewModel>();
                                userAssignsubModule.Tools = new List<UserAssignToolRightsViewModel>();
                                userAssignsubModule.SubModuleID = s.SubModuleID;
                                userAssignsubModule.ID = s.ID;
                                userAssignsubModule.View = s.View;
                                userAssignsubModule.Edit = s.Edit;
                                userAssignsubModule.Delete = s.Delete;
                                userAssignsubModule.Create = s.Create;
                                userAssignsubModule.Status = s.Status;
                                userAssignsubModule.ParentID = s.ParentID;
                                userAssignsubModule.SubModuleName = s.SubModuleName;
                                //var Fields = await _FieldRepository.ListAsync(new ModuleSpecification3(s.Id));
                                //var Tools = await _ToolRepository.ListAsync(new ModuleSpecification4(s.Id));
                                s.Fields = s.Fields.OrderBy(o => o.FieldName).ToList();
                                s.Tools = s.Tools.OrderBy(o => o.ToolName).ToList();
                                foreach (var a in s.Fields)
                                {
                                    //if (a.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                                    //{
                                    var k = new UserAssignFieldRightsViewModel
                                    {
                                        FieldID = a.FieldID,
                                        Status = a.Status,
                                        FieldName = a.FieldName,
                                        ID =a.ID,
                                        IsVisibility = a.IsVisibility
                                    };
                                    userAssignsubModule.Fields.Add(k);
                                    //}
                                }
                                foreach (var b in s.Tools)
                                {
                                    //if (b.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                                    //{
                                    var l = new UserAssignToolRightsViewModel
                                    {
                                        ToolID = b.ToolID,
                                        Status = b.Status,
                                        ToolName = b.ToolName,
                                        ID = b.ID
                                    };
                                    userAssignsubModule.Tools.Add(l);
                                    //}
                                }
                                userAssignModule.SubModules.Add(userAssignsubModule);
                                //}

                            }
                            // khushali 07-03-2019 for parent child flatten tree
                            List<UserAssignSubModuleViewModel> newList = FlatToHierarchy(userAssignModule.SubModules);
                            userAssignModule.SubModules = newList;
                            Response.Modules.Add(userAssignModule);
                            //}
                        }
                        return Response;
                    }
                }
                return null;
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<UserAccessRightsViewModel> GetMenuAssignedRule(long GroupID)
        {
            UserAccessRightsViewModel Response = new UserAccessRightsViewModel();
            try
            {
                List<UserAccessRights> Data = await _roleRepository.ListAsync(new UserAccessRightsSpecification(GroupID));
                if (Data != null)
                {
                    if (Data.Count > 0)
                    {
                        // return Data[0];
                        UserAccessRights Record = Data[0];
                        Response.Modules = new List<UserAssignModuleViewModel>();
                        Response.GroupID = GroupID;
                        Response.ID = Record.Id;
                        //Record.Modules = Record.Modules.Where(o => o.ParentID != 0).ToList();
                        //var modules = await _ModuleRepository.ListAllAsync();
                        foreach (var m in Record.Modules)
                        {
                            //if (m.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                            //{
                            UserAssignModuleViewModel userAssignModule = new UserAssignModuleViewModel();
                            userAssignModule.SubModules = new List<UserAssignSubModuleViewModel>();
                            userAssignModule.ModuleID = m.ModuleID;
                            userAssignModule.Status = m.Status;
                            userAssignModule.ID = m.ID;
                            userAssignModule.ModuleName = m.ModuleName;
                            userAssignModule.ParentID = m.ParentID;
                            //var submodules = await _SubModuleRepository.ListAsync(new ModuleSpecification2(m.Id));
                            //foreach (var s in m.SubModules)
                            //{
                            //    //if (s.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                            //    //{
                            //    UserAssignSubModuleViewModel userAssignsubModule = new UserAssignSubModuleViewModel();
                            //    userAssignsubModule.Fields = new List<UserAssignFieldRightsViewModel>();
                            //    userAssignsubModule.Tools = new List<UserAssignToolRightsViewModel>();
                            //    userAssignsubModule.SubModuleID = s.SubModuleID;
                            //    userAssignsubModule.ID = s.ID;
                            //    userAssignsubModule.View = 0;
                            //    userAssignsubModule.Edit = 0;
                            //    userAssignsubModule.Delete = 0;
                            //    userAssignsubModule.Create = 0;
                            //    userAssignsubModule.Status = s.Status;
                            //    userAssignsubModule.ParentID = s.ParentID;
                            //    userAssignsubModule.SubModuleName = s.SubModuleName;
                            //    //var Fields = await _FieldRepository.ListAsync(new ModuleSpecification3(s.Id));
                            //    //var Tools = await _ToolRepository.ListAsync(new ModuleSpecification4(s.Id));

                            //    foreach (var a in s.Fields)
                            //    {
                            //        //if (a.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                            //        //{
                            //        var k = new UserAssignFieldRightsViewModel
                            //        {
                            //            FieldID = a.FieldID,
                            //            Status = a.Status,
                            //            FieldName = a.FieldName,
                            //            ID = a.ID
                            //        };
                            //        userAssignsubModule.Fields.Add(k);
                            //        //}
                            //    }
                            //    foreach (var b in s.Tools)
                            //    {
                            //        //if (b.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                            //        //{
                            //        var l = new UserAssignToolRightsViewModel
                            //        {
                            //            ToolID = b.ToolID,
                            //            Status = b.Status,
                            //            ToolName = b.ToolName,
                            //            ID = b.ID
                            //        };
                            //        userAssignsubModule.Tools.Add(l);
                            //        //}
                            //    }
                            //    userAssignModule.SubModules.Add(userAssignsubModule);
                            //    //}

                            //}
                            // khushali 07-03-2019 for parent child flatten tree
                            
                            Response.Modules.Add(userAssignModule);
                            //}
                        }
                        List<UserAssignModuleViewModel> newList = FlatToHierarchyMenu(Response.Modules);
                        Response.Modules = newList;
                        return Response;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        // list ConfigurePermissions for create permission  group
        public async Task<UserAccessRightsViewModel> GetConfigurePermissions()
        {
            UserAccessRightsViewModel Response = new UserAccessRightsViewModel();
            try
            {
                //UserAccessRights userAccessRight = new UserAccessRights();
                Response.Modules = new List<UserAssignModuleViewModel>();
                Response.GroupID = 0;
                var modules = await _ModuleRepository.ListAllAsync();
                modules = modules.OrderBy(o => o.ModuleName).ToList();
                foreach (var m in modules)
                {
                    //if (m.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                    //{
                        UserAssignModuleViewModel userAssignModule = new UserAssignModuleViewModel();
                        userAssignModule.SubModules = new List<UserAssignSubModuleViewModel>();
                        userAssignModule.ModuleID = m.Id;
                        userAssignModule.Status = Convert.ToInt16(enVelocityRuleStatus.Active);
                        userAssignModule.ModuleName = m.ModuleName;
                        userAssignModule.ParentID = m.ParentID;
                        var submodules = await _SubModuleRepository.ListAsync(new ModuleSpecification2(m.Id));
                        submodules = submodules.OrderBy(o => o.SubModuleName).ToList();
                        foreach (var s in submodules)
                        {
                            //if (s.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                            //{
                                UserAssignSubModuleViewModel userAssignsubModule = new UserAssignSubModuleViewModel();
                                userAssignsubModule.Fields = new List<UserAssignFieldRightsViewModel>();
                                userAssignsubModule.Tools = new List<UserAssignToolRightsViewModel>();
                                userAssignsubModule.SubModuleID = s.Id;
                                userAssignsubModule.View = 0;
                                userAssignsubModule.Edit = 0;
                                userAssignsubModule.Delete = 0;
                                userAssignsubModule.Create = 0;
                                userAssignsubModule.Status = Convert.ToInt16(enVelocityRuleStatus.Active);
                                userAssignsubModule.ParentID = s.ParentID;
                                userAssignsubModule.SubModuleName = s.SubModuleName;
                                var Fields = await _FieldRepository.ListAsync(new ModuleSpecification3(s.Id));
                                var Tools = await _ToolRepository.ListAsync(new ModuleSpecification4(s.Id));
                                Fields = Fields.OrderBy(o => o.FieldName).ToList();
                                Tools = Tools.OrderBy(o => o.ToolName).ToList();
                                foreach (var a in Fields)
                                {
                                    //if (a.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                                    //{
                                        var k = new UserAssignFieldRightsViewModel
                                        {
                                            FieldID = a.Id,
                                            Status = Convert.ToInt16(enVelocityRuleStatus.Active),
                                            FieldName = a.FieldName
                                        };
                                        userAssignsubModule.Fields.Add(k);
                                    //}
                                }
                                foreach (var b in Tools)
                                {
                                    //if (b.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                                    //{
                                        var l = new UserAssignToolRightsViewModel
                                        {
                                            ToolID = b.Id,
                                            Status = Convert.ToInt16(enVelocityRuleStatus.Active),
                                            ToolName = b.ToolName
                                        };
                                        userAssignsubModule.Tools.Add(l);
                                    //}
                                }
                                userAssignModule.SubModules.Add(userAssignsubModule);
                            //}

                        }
                        Response.Modules.Add(userAssignModule);
                    //}
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        
        // list ConfigurePermissions for create permission  group - parent Child wise
        public async Task<UserAccessRightsViewModel> GetConfigurePermissionsV2()
        {
            UserAccessRightsViewModel Response = new UserAccessRightsViewModel();
            try
            {
                //UserAccessRights userAccessRight = new UserAccessRights();
                Response.Modules = new List<UserAssignModuleViewModel>();
                Response.GroupID = 0;
                var modules = await _ModuleRepository.ListAsync(new ModuleChildSpecification()); // all child of main menu 
                foreach (var m in modules)
                {
                    if (m.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                    {
                        UserAssignModuleViewModel userAssignModule = new UserAssignModuleViewModel();
                        userAssignModule.SubModules = new List<UserAssignSubModuleViewModel>();
                        userAssignModule.ModuleID = m.Id;
                        userAssignModule.Status = m.Status;
                        userAssignModule.ModuleName = m.ModuleName;
                        //var submodules = await _SubModuleRepository.ListAsync(new SubModuleParentSpecification(m.Id));     
                        //var Allsubmodules = await _SubModuleRepository.ListAllAsync();                        

                        var submodules = await _SubModuleRepository.ListAsync(new ModuleSpecification2(m.Id));
                        foreach (var s in submodules) // all parent
                        {
                            //var list = Allsubmodules.Select(u => new UserAssignSubModuleViewModel
                            //{
                            //    SubModuleID = u.Id,
                            //    ParentID = u.ParentID,
                            //    Create  = 0,
                            //    View  = 0,
                            //    Edit  = 0,
                            //    Delete  = 0,
                            //    SubModuleName = u.SubModuleName,
                            //    Status = u.Status,
                            //    Fields = _FieldRepository.ListAsync(new ModuleSpecification3(s.Id)).Result.Select(a  => 
                            //        new UserAssignFieldRightsViewModel
                            //        {
                            //            FieldID = a.Id,
                            //            Status = a.Status,
                            //            FieldName = a.FieldName
                            //        }).ToList(),
                            //    Tools = _ToolRepository.ListAsync(new ModuleSpecification4(s.Id)).Result.Select(b =>
                            //        new UserAssignToolRightsViewModel
                            //        {
                            //            ToolID = b.Id,
                            //            Status = b.Status,
                            //            ToolName = b.ToolName
                            //        }).ToList(),
                            //}).Where(u => u.ParentID == s.Id).ToList();
                            //if (s.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                            //{
                            UserAssignSubModuleViewModel userAssignsubModule = new UserAssignSubModuleViewModel();
                            userAssignsubModule.Fields = new List<UserAssignFieldRightsViewModel>();
                            userAssignsubModule.Tools = new List<UserAssignToolRightsViewModel>();
                            //userAssignsubModule.ChildNodes = list;
                            userAssignsubModule.SubModuleID = s.Id;
                            userAssignsubModule.View = 0;
                            userAssignsubModule.Edit = 0;
                            userAssignsubModule.Delete = 0;
                            userAssignsubModule.Create = 0;
                            userAssignsubModule.Status = s.Status;
                            userAssignsubModule.ParentID = s.ParentID;
                            userAssignsubModule.SubModuleName = s.SubModuleName;
                            var Fields = await _FieldRepository.ListAsync(new ModuleSpecification3(s.Id));
                            var Tools = await _ToolRepository.ListAsync(new ModuleSpecification4(s.Id));
                            
                            foreach (var a in Fields)
                            {
                                //if (a.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                                //{
                                var k = new UserAssignFieldRightsViewModel
                                {
                                    FieldID = a.Id,
                                    Status = a.Status,
                                    FieldName = a.FieldName
                                };
                                userAssignsubModule.Fields.Add(k);
                                //}
                            }
                            foreach (var b in Tools)
                            {
                                //if (b.Status == Convert.ToInt16(enVelocityRuleStatus.Active))
                                //{
                                var l = new UserAssignToolRightsViewModel
                                {
                                    ToolID = b.Id,
                                    Status = b.Status,
                                    ToolName = b.ToolName
                                };
                                userAssignsubModule.Tools.Add(l);
                                //}
                            }
                            userAssignModule.SubModules.Add(userAssignsubModule);
                            //}

                        }
                        List<UserAssignSubModuleViewModel> newList = FlatToHierarchy(userAssignModule.SubModules);
                        userAssignModule.SubModules = newList;
                        Response.Modules.Add(userAssignModule);
                    }
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }       

        public async Task<BizResponseClass> CreateAssignedRuleV2(UserAccessRightsViewModel Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                List<UserAccessRights> CheckRecordExist = await _roleRepository.ListAsync(new UserAccessRightsSpecification(Request.GroupID));
                if (CheckRecordExist.Count > 0)
                {
                    Response.ErrorCode = enErrorCode.RecordAlreadyExist;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = "Already Permission Group exist";
                    return Response;
                }
                UserAccessRights userAccessRight = new UserAccessRights();
                userAccessRight.Modules = new List<UserAssignModule>();
                userAccessRight.GroupID = Request.GroupID;
                userAccessRight.CreatedBy = 1;
                userAccessRight.CreatedDate = Helpers.UTC_To_IST();
                foreach (var Module in Request.Modules)
                {
                    UserAssignModule userAssignModule = new UserAssignModule();
                    userAssignModule.SubModules = new List<UserAssignSubModule>();
                    userAssignModule.ModuleID = Module.ModuleID;
                    userAssignModule.ModuleName = Module.ModuleName;
                    userAssignModule.Status = Module.Status;
                    userAssignModule.ParentID = Module.ParentID;
                    foreach (var SubModule in Module.SubModules)
                    {
                        UserAssignSubModule userAssignsubModule = new UserAssignSubModule();
                        userAssignsubModule.Fields = new List<UserAssignFieldRights>();
                        userAssignsubModule.Tools = new List<UserAssignToolRights>();
                        userAssignsubModule.SubModuleID = SubModule.SubModuleID;
                        userAssignsubModule.SubModuleName = SubModule.SubModuleName;
                        userAssignsubModule.View = SubModule.View;
                        userAssignsubModule.Edit = SubModule.Edit;
                        userAssignsubModule.Delete = SubModule.Delete;
                        userAssignsubModule.Create = SubModule.Create;
                        userAssignsubModule.Status = SubModule.Status;
                        userAssignsubModule.ParentID = SubModule.ParentID;
                        foreach (var Field in SubModule.Fields)
                        {
                            var k = new UserAssignFieldRights
                            {
                                FieldID = Field.FieldID,
                                Status = Field.Status,
                                FieldName = Field.FieldName,
                                IsVisibility = Field.IsVisibility
                            };
                            userAssignsubModule.Fields.Add(k);
                        }
                        foreach (var Tool in SubModule.Tools)
                        {
                            var l = new UserAssignToolRights
                            {
                                ToolID = Tool.ToolID,
                                ToolName = Tool.ToolName,
                                Status = Tool.Status
                            };
                            userAssignsubModule.Tools.Add(l);
                        }
                        userAssignModule.SubModules.Add(userAssignsubModule);
                    }
                    userAccessRight.Modules.Add(userAssignModule);
                }
                var x = await _roleRepository.AddAsync(userAccessRight);
                Response.ErrorCode = enErrorCode.RecordCreatedSuccessfully;
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Record Added Successfully";
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> UpdateAssignedRule(UserAccessRightsViewModel Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                //var Data = await _roleRepository.GetByIdAsync(Request.ID);
                List<UserAccessRights> DataList = await _roleRepository.ListAsync(new UserAccessRightsSpecification(Request.GroupID));
                if (DataList != null)
                {
                    if(DataList.Count >= 0)
                    {
                        UserAccessRights Data = DataList.Where(x => x.Id == Request.ID).SingleOrDefault();
                        Data.UpdatedBy = 1;
                        Data.UpdatedDate = Helpers.UTC_To_IST();
                        foreach (var Module in Request.Modules)
                        {
                            var UpdateModule = Data.Modules.Where(x => x.ID == Module.ID).SingleOrDefault();
                            UpdateModule.Status = Module.Status;
                            foreach (var SubModule in Module.SubModules)
                            {
                                var UpdateSubModule = UpdateModule.SubModules.Where(x => x.ID == SubModule.ID).SingleOrDefault();
                                UpdateSubModule.View = SubModule.View;
                                UpdateSubModule.Edit = SubModule.Edit;
                                UpdateSubModule.Delete = SubModule.Delete;
                                UpdateSubModule.Create = SubModule.Create;
                                UpdateSubModule.Status = SubModule.Status;
                                foreach (var Field in SubModule.Fields)
                                {
                                    var UpdateField = UpdateSubModule.Fields.Where(x => x.ID == Field.ID).SingleOrDefault();
                                    UpdateField.Status = Field.Status;
                                    UpdateField.IsVisibility = Field.IsVisibility;
                                }
                                foreach (var Tool in SubModule.Tools)
                                {
                                    var UpdateTool = UpdateSubModule.Tools.Where(x => x.ID == Tool.ID).SingleOrDefault();
                                    UpdateTool.Status = Tool.Status;
                                }
                            }
                        }
                        await _roleRepository.UpdateAsync(Data);
                        Response.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ReturnMsg = "Record Updated Successfully";
                        return Response;
                    }                    
                }
                Response.ErrorCode = enErrorCode.NoRecordFound;
                Response.ReturnCode = enResponseCode.Fail;
                Response.ReturnMsg = "Record Not Found";
                return Response;                
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //Not Used Now
        public async Task<UserAccessRightViewModel> CreateAssignedRule(long GroupID)
        {
            UserAccessRightViewModel Response = new UserAccessRightViewModel();
            try
            {
                var CheckRecordExist = await _roleRepository.ListAsync(new UserAccessRightsSpecification(GroupID));
                if (CheckRecordExist.Count > 0)
                {
                    Response.ErrorCode = enErrorCode.RecordAlreadyExist;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = "Already Permission Group exist";
                    return Response;
                }
                UserAccessRights userAccessRight = new UserAccessRights();
                userAccessRight.Modules = new List<UserAssignModule>();
                userAccessRight.GroupID = GroupID;
                var modules = await _ModuleRepository.ListAllAsync();
                foreach (var m in modules)
                {
                    UserAssignModule userAssignModule = new UserAssignModule();
                    userAssignModule.SubModules = new List<UserAssignSubModule>();
                    userAssignModule.ModuleID = m.Id;
                    userAssignModule.Status = 0;
                    var submodules = await _SubModuleRepository.ListAsync(new ModuleSpecification2(m.Id));
                    foreach (var s in submodules)
                    {
                        UserAssignSubModule userAssignsubModule = new UserAssignSubModule();
                        userAssignsubModule.Fields = new List<UserAssignFieldRights>();
                        userAssignsubModule.Tools = new List<UserAssignToolRights>();
                        userAssignsubModule.SubModuleID = s.Id;
                        userAssignsubModule.View = 0;
                        userAssignsubModule.Edit = 0;
                        userAssignsubModule.Delete = 0;
                        userAssignsubModule.Create = 0;
                        userAssignsubModule.Status = 0;
                        var Fields = await _FieldRepository.ListAsync(new ModuleSpecification3(s.Id));
                        var Tools = await _ToolRepository.ListAsync(new ModuleSpecification4(s.Id));

                        foreach (var a in Fields)
                        {
                            var k = new UserAssignFieldRights
                            {
                                FieldID = a.Id,
                                Status = 0
                            };
                            userAssignsubModule.Fields.Add(k);
                        }
                        foreach (var b in Tools)
                        {
                            var l = new UserAssignToolRights
                            {
                                ToolID = b.Id,
                                Status = 0
                            };
                            userAssignsubModule.Tools.Add(l);
                        }
                        userAssignModule.SubModules.Add(userAssignsubModule);
                    }
                    userAccessRight.Modules.Add(userAssignModule);
                }
                var x = await _roleRepository.AddAsync(userAccessRight);
                Response.ErrorCode = enErrorCode.RecordCreatedSuccessfully;
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Record Added Successfully";
                Response.Result = x;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        // khushali 07-03-2019 for flatten Hierarchy - parent child node
        public List<UserAssignSubModuleViewModel> FlatToHierarchy(List<UserAssignSubModuleViewModel> list)
        {
            // hashtable lookup that allows us to grab references to containers based on id
            var lookup = new Dictionary<long, UserAssignSubModuleViewModel>();
            // actual nested collection to return
            var nested = new List<UserAssignSubModuleViewModel>();

            foreach (UserAssignSubModuleViewModel item in list)
            {
                if (lookup.ContainsKey(item.ParentID))
                {
                    // add to the parent's child list 
                    lookup[item.ParentID].ChildNodes.Add(item);
                }
                else
                {
                    // no parent added yet (or this is the first time)
                    nested.Add(item);
                }
                lookup.Add(item.SubModuleID, item);
            }

            return nested;
        }

        // khushali 07-03-2019 for flatten Hierarchy - parent child node
        public List<UserAssignModuleViewModel> FlatToHierarchyMenu(List<UserAssignModuleViewModel> list)
        {
            // hashtable lookup that allows us to grab references to containers based on id
            var lookup = new Dictionary<long, UserAssignModuleViewModel>();
            // actual nested collection to return
            var nested = new List<UserAssignModuleViewModel>();

            foreach (UserAssignModuleViewModel item in list)
            {
                if (lookup.ContainsKey(item.ParentID))
                {
                    // add to the parent's child list 
                    lookup[item.ParentID].ChildNode.Add(item);
                }
                else
                {
                    // no parent added yet (or this is the first time)
                    nested.Add(item);
                }
                lookup.Add(item.ModuleID, item);
            }

            return nested;
        }
        #endregion

        #region "Add module, submodule , field , tool Data in Access layer integration on CURD"

        public async Task<bool> AdduserAssignModuleOnExistingGroup(UserAssignModule Request)
        {
            try
            {

                var AllRecords = await _roleRepository.ListAsync(new UserAccessRightsSpecificationV1());                
                if(AllRecords != null && AllRecords.Count > 0)
                {
                    foreach (var Record in AllRecords)
                    {
                        if(Record.Modules != null)
                        {
                            bool z = Record.Modules.Exists(o => o.ModuleID == Request.ModuleID);
                            if (!z)
                            {
                                Request.UserAccessRightsId = Record.Id;
                                await _userAssignModuleRepository.AddAsync(Request);
                            }
                        }                        
                    }                    
                }
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        // change by khuhsali 21-05-2019 -- for CURD operation
        public async Task<bool> AdduserAssignSubModuleOnExistingGroup(UserAssignSubModule Request,long ModuleID)
        {
            try
            {
                //var AllRecords = await _roleRepository.ListAsync(new UserAccessRightsSpecificationV1());
                //if (AllRecords != null && AllRecords.Count > 0)
                //{
                //    foreach (var Record in AllRecords)
                //    {
                //        if (Record.Modules != null)
                //        {
                //            bool z = Record.Modules.Exists(o => o.ModuleID == ModuleID);
                //            if (z)
                //            {
                //                foreach (var module in Record.Modules)
                //                {
                //                    if (module.SubModules != null)
                //                    {
                //                        bool y = module.SubModules.Exists(o => o.SubModuleID == Request.SubModuleID);
                //                        if (!y)
                //                        {
                //                            Request.UserAssignModuleID = module.ID;
                //                            await _userAssignSubModuleRepository.AddAsync(Request);
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                var List = _ModuleGroupMasterRepository.GetAll();
                foreach(var GroupID in List)
                {
                    _ControlPanelRepository.InsertSubmoduleEntryGroupWise(Request.SubModuleID, GroupID.Id);
                }                
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        // change by khuhsali 21-05-2019 -- for CURD operation
        public async Task<bool> AdduserAssignFieldOnExistingGroup(UserAssignFieldRights Request,long ModuleID,long SubModuleID)
        {
            try
            {
                //var AllRecords = await _roleRepository.ListAsync(new UserAccessRightsSpecificationV1());
                //if (AllRecords != null && AllRecords.Count > 0)
                //{
                //    foreach (var Record in AllRecords)
                //    {
                //        if (Record.Modules != null)
                //        {
                //            bool z = Record.Modules.Exists(o => o.ModuleID == ModuleID);
                //            if (z)
                //            {
                //                foreach (var module in Record.Modules)
                //                {
                //                    if (module.SubModules != null)
                //                    {
                //                        bool y = module.SubModules.Exists(o => o.SubModuleID == SubModuleID);
                //                        if (y)
                //                        {
                //                            foreach (var subModule in module.SubModules)
                //                            {
                //                                if (subModule.Fields != null)
                //                                {
                //                                    bool a = subModule.Fields.Exists(o => o.FieldID == Request.FieldID);
                //                                    if (!a)
                //                                    {
                //                                        Request.UserAssignSubModuleID = subModule.ID;
                //                                        await _userAssignFieldRepository.AddAsync(Request);
                //                                    }
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                _ControlPanelRepository.InsertFieldEntryGroupWise(Request.FieldID);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        public async Task<bool> AdduserAssignToolsOnExistingGroup(UserAssignToolRights Request, long ModuleID, long SubModuleID)
        {
            try
            {
                var AllRecords = await _roleRepository.ListAsync(new UserAccessRightsSpecificationV1());
                if (AllRecords != null && AllRecords.Count > 0)
                {
                    foreach (var Record in AllRecords)
                    {
                        if (Record.Modules != null)
                        {
                            bool z = Record.Modules.Exists(o => o.ModuleID == ModuleID);
                            if (z)
                            {
                                foreach (var module in Record.Modules)
                                {
                                    if (module.SubModules != null)
                                    {
                                        bool y = module.SubModules.Exists(o => o.SubModuleID == SubModuleID);
                                        if (y)
                                        {
                                            foreach (var subModule in module.SubModules)
                                            {
                                                if (subModule.Tools != null)
                                                {
                                                    bool a = subModule.Tools.Exists(o => o.ToolID == Request.ToolID);
                                                    if (!a)
                                                    {
                                                        Request.UserAssignSubModuleID = subModule.ID;
                                                        await _userAssignToolRepository.AddAsync(Request);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        //khushali 13-02-2019
        #region "Module Master"

        public async Task<ListModuleDataViewModel> GetAllModuleData(int PageNo,int PageSize,short? AllData = null, bool IsParentList = false)
        {
            ListModuleDataViewModel Response = new ListModuleDataViewModel();
            Response.Result = new List<ModuleDataViewModel>();
            try
            {
                //IReadOnlyList<ModuleMaster> list = await _ModuleRepository.ListAsync(new ModuleSpecification());
                IReadOnlyList<ModuleMaster> list = await _ModuleRepository.ListAllAsync();
                if (AllData == 1 && IsParentList)
                {
                    list = list.Where(o => o.ParentID == 0).ToList();
                }
                else if(AllData == 1 && !IsParentList)
                {
                    list = list.Where(o => o.ParentID != 0).ToList();
                }                
                foreach (ModuleMaster model in list)
                {                    
                    Response.Result.Add(new ModuleDataViewModel
                    {
                        ModuleID = model.Id,
                        ModuleName = model.ModuleName,
                        Status = (EnAccessStatus)model.Status,
                        ParentID = model.ParentID
                    });
                }
                Response.TotalCount = list.Count();
                if(AllData == null)
                {               
                    if (PageNo > 0)
                    {
                        int skip = PageSize * (PageNo - 1);
                        Response.Result = Response.Result.OrderByDescending(o => o.ModuleID).Skip(skip).Take(PageSize).ToList();
                    }
                }
                else
                {
                    Response.Result = Response.Result.OrderBy(o => o.ModuleName).ToList();
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<ListModuleDataViewModel> GetAllModuleDataV2(int PageNo, int PageSize)
        {
            ListModuleDataViewModel Response = new ListModuleDataViewModel();
            try
            {
                IReadOnlyList<ModuleMaster> list = await _ModuleRepository.ListAsync(new ModuleSpecification());
                foreach (ModuleMaster model in list)
                {
                    Response.Result.Add(new ModuleDataViewModel
                    {
                        ModuleID = model.Id,
                        ModuleName = model.ModuleName,
                        Status = (EnAccessStatus)model.Status,
                        ParentID = model.ParentID
                    });
                }
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
                return null;
            }
        }

        public async Task<ModuleDataViewModel> GetModuleDataByID(long Id)
        {
            try
            {

                ModuleMaster Result = await _ModuleRepository.GetByIdAsync(Id);
                if (Result != null)
                {
                    var model = new ModuleDataViewModel()
                    {
                        ModuleID = Result.Id,
                        ModuleName = Result.ModuleName,
                        Status = (EnAccessStatus)Result.Status,
                        ParentID = Result.ParentID
                    };
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<long> AddModuleData(ModuleDataViewModel request, long UserId)
        {
            try
            {
                var Data = await _ModuleRepository.ListAsync(new CheckModuleExistSpecification1(request.ModuleName));
                if(Data.Count <= 0)
                {
                    var model = new ModuleMaster()
                    {
                        CreatedBy = UserId,
                        CreatedDate = Helpers.UTC_To_IST(),
                        Status = Convert.ToInt16(request.Status),
                        ModuleName = request.ModuleName,
                        ParentID = request.ParentID
                    };
                    var newModel = await _ModuleRepository.AddAsync(model);
                    UserAssignModule userAssignModule = new UserAssignModule
                    {
                        ModuleID = newModel.Id,
                        ModuleName = request.ModuleName,
                        Status = Convert.ToInt16(request.Status),
                        ParentID = request.ParentID
                    };
                    await AdduserAssignModuleOnExistingGroup(userAssignModule);
                    return newModel.Id;
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public async Task<BizResponseClass> UpdateModuleData(ModuleDataViewModel request, long UserId)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                var model = await _ModuleRepository.GetByIdAsync(request.ModuleID);
                if (model == null)
                {
                    Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Response;
                }
                if(model.ModuleName == request.ModuleName)
                {
                    model.ModuleName = request.ModuleName;
                    model.UpdatedDate = Helpers.UTC_To_IST();
                    model.Status = Convert.ToInt16(request.Status);
                    model.UpdatedBy = UserId;
                    model.ParentID = request.ParentID;
                    await _ModuleRepository.UpdateAsync(model);
                    Response.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                    return Response;
                }
                else
                {
                    var Data = await _ModuleRepository.ListAsync(new CheckModuleExistSpecification1(request.ModuleName));
                    if (Data.Count <= 0)
                    {
                        model.ModuleName = request.ModuleName;
                        model.UpdatedDate = Helpers.UTC_To_IST();
                        model.Status = Convert.ToInt16(request.Status);
                        model.UpdatedBy = UserId;
                        model.ParentID = request.ParentID;
                        await _ModuleRepository.UpdateAsync(model);
                        Response.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                        return Response;
                    }
                    else
                    {
                        Response.ReturnMsg = EnResponseMessage.ModuleNameAlreadyExist;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.ModuleNameAlreadyExist;
                        return Response;
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<bool> ChangeStatusModuleData(long id, short Status)
        {
            try
            {
                ModuleMaster model = await _ModuleRepository.GetByIdAsync(id);
                if (model != null)
                {
                    model.Status = Status;
                    await _ModuleRepository.UpdateAsync(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        #endregion

        //khushali 13-02-2019
        #region "Sub Module Master"

        public async Task<ListSubModuleDataViewModel> GetAllSubModuleData(int PageNo,int PageSize,short? AllData = null,bool IsParentList = false)
        {
            ListSubModuleDataViewModel Response = new ListSubModuleDataViewModel();
            Response.Result = new List<SubModuleDataViewModel>();
            try
            {
                IReadOnlyList<SubModuleMaster> list = await _SubModuleRepository.ListAllAsync();
                if (AllData == 1 && IsParentList)
                {
                    list = list.Where(o => o.ParentID == 0).ToList();
                }
                else if (AllData == 1 && !IsParentList)
                {
                    list = list.Where(o => o.ParentID != 0).ToList();
                }
                foreach (SubModuleMaster model in list)
                {
                    Response.Result.Add(new SubModuleDataViewModel
                    {
                        SubModuleID = model.Id,
                        SubModuleName = model.SubModuleName,
                        Status = (EnAccessStatus)model.Status,
                        //ModuleID = 0,
                        ParentGUID = model.ParentGUID,
                        UtilityTypes = model.UtilityTypes,
                        ModuleDomainType = model.ModuleDomainType,
                        Type = model.Type,
                        GUID = model.GUID,                        
                        CrudTypes = model.CrudTypes
                    });
                }
                Response.TotalCount = list.Count();
                if (AllData == null)
                {
                    if (PageNo > 0)
                    {
                        int skip = PageSize * (PageNo - 1);
                        Response.Result = Response.Result.OrderByDescending(o => o.SubModuleID).Skip(skip).Take(PageSize).ToList();
                    }
                }
                else
                {
                    Response.Result = Response.Result.OrderBy(o => o.SubModuleName).ToList();
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<SubModuleDataViewModel> GetSubModuleDataByID(long Id)
        {
            try
            {

                SubModuleMaster Result = await _SubModuleRepository.GetByIdAsync(Id);
                if (Result != null)
                {
                    var model = new SubModuleDataViewModel()
                    {
                        SubModuleID = Result.Id,
                        SubModuleName = Result.SubModuleName,
                        Status = (EnAccessStatus)Result.Status,
                        //ModuleID = Result.ModuleID,
                        ParentGUID = Result.ParentGUID,
                        UtilityTypes = Result.UtilityTypes,
                        ModuleDomainType = Result.ModuleDomainType,
                        Type = Result.Type,
                        GUID = Result.GUID,
                        CrudTypes = Result.CrudTypes
                    };
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<long> AddSubModuleData(SubModuleDataViewModel request, long UserId)
        {
            try
            {
                //var Data = await _SubModuleRepository.ListAsync(new CheckSubModuleExistSpecification2(request.SubModuleName));
                //if (Data.Count <= 0)
                //{
                var IsExist = _SubModuleRepository.FindBy(e => e.GUID == request.GUID);
                if (IsExist != null && IsExist.Count() > 0)
                {
                    return 0;
                }
                var ParentData =  _SubModuleRepository.FindBy(e => e.GUID  ==  request.ParentGUID).FirstOrDefault();
                if(ParentData != null)
                {
                    var model = new SubModuleMaster()
                    {
                        CreatedBy = UserId,
                        CreatedDate = Helpers.UTC_To_IST(),
                        Status = Convert.ToInt16(request.Status),
                        SubModuleName = request.SubModuleName,
                        ModuleID = 0,
                        ParentID = ParentData.Id,
                        UtilityTypes = request.UtilityTypes,
                        ModuleDomainType = request.ModuleDomainType,
                        Type = request.Type,
                        GUID = request.GUID,
                        CrudTypes = request.CrudTypes,
                        ParentGUID = request.ParentGUID
                    };
                    var newModel = await _SubModuleRepository.AddAsync(model);
                    UserAssignSubModule userAssignSubModule = new UserAssignSubModule
                    {
                        SubModuleID = newModel.Id,
                        SubModuleName = request.SubModuleName,
                        Status = Convert.ToInt16(request.Status),
                        ParentID = ParentData.Id
                    };
                    await AdduserAssignSubModuleOnExistingGroup(userAssignSubModule, 0);
                    return newModel.Id;
                }
                else if(request.GUID.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    var model = new SubModuleMaster()
                    {
                        CreatedBy = UserId,
                        CreatedDate = Helpers.UTC_To_IST(),
                        Status = Convert.ToInt16(request.Status),
                        SubModuleName = request.SubModuleName,
                        ModuleID = 0,
                        ParentID = 0,
                        UtilityTypes = request.UtilityTypes,
                        ModuleDomainType = request.ModuleDomainType,
                        Type = request.Type,
                        GUID = request.GUID,
                        CrudTypes = request.CrudTypes,
                        ParentGUID = request.ParentGUID
                    };
                    var newModel = await _SubModuleRepository.AddAsync(model);
                    UserAssignSubModule userAssignSubModule = new UserAssignSubModule
                    {
                        SubModuleID = newModel.Id,
                        SubModuleName = request.SubModuleName,
                        Status = Convert.ToInt16(request.Status),
                        ParentID = 0
                    };
                    await AdduserAssignSubModuleOnExistingGroup(userAssignSubModule, 0);
                    return newModel.Id;
                }
                else
                {
                    return 0;
                }
                //}
                
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public async Task<BizResponseClass> UpdateSubModuleData(SubModuleDataViewModel request, long UserId)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (request.SubModuleID == 0)
                {
                    Response.ReturnMsg = "Invalid SubModule ID!";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Response;
                }
                var model = await _SubModuleRepository.GetByIdAsync(request.SubModuleID);
                if (model == null)
                {
                    Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Response;
                }
                var ParentData = _SubModuleRepository.FindBy(e => e.GUID == request.ParentGUID).FirstOrDefault();
                if (ParentData != null)
                {
                    //if(model.SubModuleName == request.SubModuleName)
                    //{
                    model.SubModuleName = request.SubModuleName;
                    model.UpdatedDate = Helpers.UTC_To_IST();
                    model.Status = Convert.ToInt16(request.Status);
                    model.UpdatedBy = UserId;
                    model.ModuleID = 0;
                    model.ParentID = ParentData.Id;
                    model.UtilityTypes = request.UtilityTypes;
                    model.ModuleDomainType = request.ModuleDomainType;
                    model.Type = request.Type;
                    model.GUID = request.GUID;
                    model.CrudTypes = request.CrudTypes;
                    model.ParentGUID = request.ParentGUID;
                    await _SubModuleRepository.UpdateAsync(model);
                    Response.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                    return Response;
                }
                else if (request.GUID.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    model.SubModuleName = request.SubModuleName;
                    model.UpdatedDate = Helpers.UTC_To_IST();
                    model.Status = Convert.ToInt16(request.Status);
                    model.UpdatedBy = UserId;
                    model.ModuleID = 0;
                    model.ParentID = 0;
                    model.UtilityTypes = request.UtilityTypes;
                    model.ModuleDomainType = request.ModuleDomainType;
                    model.Type = request.Type;
                    model.GUID = request.GUID;
                    model.CrudTypes = request.CrudTypes;
                    model.ParentGUID = request.ParentGUID;
                    await _SubModuleRepository.UpdateAsync(model);
                    Response.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                    return Response;
                }
                else
                {
                    Response.ReturnMsg = "Parent Data not found";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Response;
                }
                
                //}
                //else
                //{
                //    var Data = await _SubModuleRepository.ListAsync(new CheckSubModuleExistSpecification2(request.SubModuleName));
                //    if (Data.Count <= 0)
                //    {
                //        model.SubModuleName = request.SubModuleName;
                //        model.UpdatedDate = Helpers.UTC_To_IST();
                //        model.Status = Convert.ToInt16(request.Status);
                //        model.UpdatedBy = UserId;
                //        model.ModuleID = request.ModuleID;
                //        model.ParentID = request.ParentID;
                //        await _SubModuleRepository.UpdateAsync(model);
                //        Response.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                //        Response.ReturnCode = enResponseCode.Success;
                //        Response.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                //        return Response;
                //    }
                //    else
                //    {
                //        Response.ReturnMsg = EnResponseMessage.SubModuleNameAlreadyExist;
                //        Response.ReturnCode = enResponseCode.Fail;
                //        Response.ErrorCode = enErrorCode.SubModuleNameAlreadyExist;
                //        return Response;
                //    }
                //}
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<bool> ChangeStatusSubModuleData(long id, short Status)
        {
            try
            {
                SubModuleMaster model = await _SubModuleRepository.GetByIdAsync(id);
                if (model != null)
                {
                    model.Status = Status;
                    await _SubModuleRepository.UpdateAsync(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        #endregion


        //khushali 13-02-2019
        #region "Field Master"

        public async Task<ListFieldDataViewModel> GetAllFieldData(int PageNo, int PageSize,short? AllData = null)
        {
            ListFieldDataViewModel Response = new ListFieldDataViewModel();
            Response.Result = new List<FieldDataViewModel>();
            try
            {
                IReadOnlyList<FieldMaster> list = await _FieldRepository.ListAllAsync();
                foreach (FieldMaster model in list)
                {
                    var ParentData = _SubModuleRepository.FindBy(e => e.Id == model.SubModuleID).FirstOrDefault();
                    if (ParentData != null)
                    {
                        Response.Result.Add(new FieldDataViewModel
                        {
                            FieldID = model.Id,
                            FieldName = model.FieldName,
                            Status = (EnAccessStatus)model.Status,
                            ModulleGUID = ParentData.GUID,
                            Required = model.Required,
                            AccressRight = model.AccressRight,
                            GUID = model.GUID
                            //Visibility = (EnVisibilityMode)model.Visibility
                        });
                    }
                }
                Response.TotalCount = list.Count();
                if (AllData == null)
                {
                    if (PageNo > 0)
                    {
                        int skip = PageSize * (PageNo - 1);
                        Response.Result = Response.Result.OrderByDescending(o => o.FieldID).Skip(skip).Take(PageSize).ToList();
                    }
                }
                else
                {
                    Response.Result = Response.Result.OrderBy(o => o.FieldName).ToList();
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<FieldDataViewModel> GetFieldDataByID(long Id)
        {
            try
            {

                FieldMaster Result = await _FieldRepository.GetByIdAsync(Id);
                if (Result != null)
                {
                    var ParentData = _SubModuleRepository.FindBy(e => e.Id == Result.SubModuleID).FirstOrDefault();
                    if (ParentData != null)
                    {
                        var model = new FieldDataViewModel()
                        {
                            FieldID = Result.Id,
                            FieldName = Result.FieldName,
                            Status = (EnAccessStatus)Result.Status,
                            ModulleGUID = ParentData.GUID,
                            Required = Result.Required,
                            AccressRight = Result.AccressRight,
                            GUID = Result.GUID
                        };
                        return model;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<long> AddFieldData(FieldDataViewModel request, long UserId)
        {
            try
            {
                //var Data = await _FieldRepository.ListAsync(new CheckFieldExistSpecification3(request.FieldName));
                //if (Data.Count <= 0)
                //{
                var IsExist = _FieldRepository.FindBy(e => e.GUID == request.GUID);
                if (IsExist != null && IsExist.Count() > 0)
                {
                    return 0;
                }
                var ParentData = _SubModuleRepository.FindBy(e => e.GUID == request.ModulleGUID).FirstOrDefault();
                if (ParentData != null)
                {
                    var model = new FieldMaster()
                    {
                        CreatedBy = UserId,
                        CreatedDate = Helpers.UTC_To_IST(),
                        Status = Convert.ToInt16(request.Status),
                        FieldName = request.FieldName,
                        //Visibility = Convert.ToInt16(request.Visibility),
                        SubModuleID = ParentData.Id,
                        Required = request.Required,
                        AccressRight = request.AccressRight,
                        GUID = request.GUID
                    };
                    var newModel = await _FieldRepository.AddAsync(model);
                    UserAssignFieldRights userAssignField = new UserAssignFieldRights
                    {
                        FieldID = newModel.Id,
                        FieldName = request.FieldName,
                        Status = Convert.ToInt16(request.Status)

                    };
                    await AdduserAssignFieldOnExistingGroup(userAssignField, ParentData.Id, 1);
                    return newModel.Id;
                }
                else
                {
                    return 0;
                }
                //}
                //return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public async Task<BizResponseClass> UpdateFieldData(FieldDataViewModel request, long UserId)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (request.FieldID == 0)
                {
                    Response.ReturnMsg = "Invalid Feild ID!";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Response;
                }
                var model = await _FieldRepository.GetByIdAsync(request.FieldID);
                if (model == null)
                {
                    Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Response;
                }
                //if(model.FieldName == request.FieldName)
                //{
                var ParentData = _SubModuleRepository.FindBy(e => e.GUID == request.ModulleGUID).FirstOrDefault();
                if (ParentData != null)
                {
                    model.FieldName = request.FieldName;
                    model.UpdatedDate = Helpers.UTC_To_IST();
                    model.Status = Convert.ToInt16(request.Status);
                    model.UpdatedBy = UserId;
                    //model.Visibility = Convert.ToInt16(request.Visibility);
                    model.SubModuleID = ParentData.Id;
                    model.Required = request.Required;
                    model.AccressRight = request.AccressRight;
                    model.GUID = request.GUID;
                    await _FieldRepository.UpdateAsync(model);
                    Response.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                    return Response;
                }
                else
                {
                    Response.ReturnMsg = "Parent Data not found";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Response;
                }
                //}
                //else
                //{
                //    var Data = await _FieldRepository.ListAsync(new CheckFieldExistSpecification3(request.FieldName));
                //    if (Data.Count <= 0)
                //    {

                //        model.FieldName = request.FieldName;
                //        model.UpdatedDate = Helpers.UTC_To_IST();
                //        model.Status = Convert.ToInt16(request.Status);
                //        model.UpdatedBy = UserId;
                //        //model.Visibility = Convert.ToInt16(request.Visibility);
                //        model.SubModuleID = request.SubModuleID;

                //        await _FieldRepository.UpdateAsync(model);
                //        Response.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                //        Response.ReturnCode = enResponseCode.Success;
                //        Response.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                //        return Response;
                //    }
                //    else
                //    {
                //        Response.ReturnMsg = EnResponseMessage.FieldNameAlreadyExist;
                //        Response.ReturnCode = enResponseCode.Fail;
                //        Response.ErrorCode = enErrorCode.FieldNameAlreadyExist;
                //        return Response;
                //    }
                //}
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<bool> ChangeStatusFieldData(long id, short Status)
        {
            try
            {
                FieldMaster model = await _FieldRepository.GetByIdAsync(id);
                if (model != null)
                {
                    model.Status = Status;
                    await _FieldRepository.UpdateAsync(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        #endregion

        //khushali 13-02-2019
        #region "Tool Master"

        public async Task<ListToolDataViewModel> GetAllToolData(int PageNo,int PageSize,short? AllData = null)
        {
            ListToolDataViewModel Response = new ListToolDataViewModel();
            Response.Result = new List<ToolDataViewModel>();
            try
            {
                IReadOnlyList<ToolMaster> list = await _ToolRepository.ListAllAsync();
                foreach (ToolMaster model in list)
                {
                    Response.Result.Add(new ToolDataViewModel
                    {
                        ToolID = model.Id,
                        ToolName = model.ToolName,
                        Status = (EnAccessStatus)model.Status,
                        SubModuleID = model.SubModuleID

                    });
                }
                Response.TotalCount = list.Count();
                if (AllData == null)
                {
                    if (PageNo > 0)
                    {
                        int skip = PageSize * (PageNo - 1);
                        Response.Result = Response.Result.OrderByDescending(o => o.ToolID).Skip(skip).Take(PageSize).ToList();
                    }
                }
                else
                {
                    Response.Result = Response.Result.OrderBy(o => o.ToolName).ToList();
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<ToolDataViewModel> GetToolDataByID(long Id)
        {
            try
            {

                ToolMaster Result = await _ToolRepository.GetByIdAsync(Id);
                if (Result != null)
                {
                    var model = new ToolDataViewModel()
                    {
                        ToolID = Result.Id,
                        ToolName = Result.ToolName,
                        Status = (EnAccessStatus)Result.Status,
                        SubModuleID = Result.SubModuleID
                    };
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<long> AddToolData(ToolDataViewModel request, long UserId)
        {
            try
            {
                var Data = await _ToolRepository.ListAsync(new CheckToolExistSpecification4(request.ToolName));
                if (Data.Count <= 0)
                {
                    var model = new ToolMaster()
                    {
                        CreatedBy = UserId,
                        CreatedDate = Helpers.UTC_To_IST(),
                        Status = Convert.ToInt16(request.Status),
                        ToolName = request.ToolName,
                        SubModuleID = request.SubModuleID
                    };
                    var newModel = await _ToolRepository.AddAsync(model);
                    UserAssignToolRights userAssignTool = new UserAssignToolRights
                    {
                        ToolID = newModel.Id,
                        ToolName = request.ToolName,
                        Status = Convert.ToInt16(request.Status)

                    };
                    await AdduserAssignToolsOnExistingGroup(userAssignTool, request.SubModuleID, 1);
                    return newModel.Id;
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public async Task<BizResponseClass> UpdateToolData(ToolDataViewModel request, long UserId)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                var model = await _ToolRepository.GetByIdAsync(request.ToolID);
                if (model == null)
                {
                    Response.ReturnMsg = EnResponseMessage.RuleManagementNoDataFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoRecordFound;
                    return Response;
                }
                if(model.ToolName == request.ToolName)
                {
                    model.ToolName = request.ToolName;
                    model.UpdatedDate = Helpers.UTC_To_IST();
                    model.Status = Convert.ToInt16(request.Status);
                    model.UpdatedBy = UserId;
                    model.SubModuleID = request.SubModuleID;
                    await _ToolRepository.UpdateAsync(model);
                    Response.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                    return Response;
                }
                else
                {
                    var Data = await _ToolRepository.ListAsync(new CheckToolExistSpecification4(request.ToolName));
                    if (Data.Count <= 0)
                    {
                        model.ToolName = request.ToolName;
                        model.UpdatedDate = Helpers.UTC_To_IST();
                        model.Status = Convert.ToInt16(request.Status);
                        model.UpdatedBy = UserId;
                        model.SubModuleID = request.SubModuleID;
                        await _ToolRepository.UpdateAsync(model);
                        Response.ReturnMsg = EnResponseMessage.CommRecordUpdateSuccess;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                        return Response;
                    }
                    else
                    {
                        Response.ReturnMsg = EnResponseMessage.ToolNameAlreadyExist;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.ToolNameAlreadyExist;
                        return Response;
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<bool> ChangeStatusToolData(long id, short Status)
        {
            try
            {
                ToolMaster model = await _ToolRepository.GetByIdAsync(id);
                if (model != null)
                {
                    model.Status = Status;
                    await _ToolRepository.UpdateAsync(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        #endregion
    }

    public class CacheRuleManageService : IRuleManageService
    {
        private readonly IMemoryCache _cache;
        private readonly RuleManageService _ruleManageService;
        //private static readonly string _moduleKey = "ModuleData"; //komal 03 May 2019, Cleanup
        //private static readonly string _velocityRuleKey = "velocityRules";        
        //private static readonly string _SubModuleKey = "SubModuleData";
        //private static readonly string _FieldKey = "FieldData";
        //private static readonly string _ToolKey = "ToolData";
        //private static readonly string _itemsKeyTemplate = "items-{0}-{1}-{2}-{3}";
        private static readonly TimeSpan _defaultCacheDuration = TimeSpan.FromSeconds(30);

        public CacheRuleManageService(IMemoryCache cache, RuleManageService ruleManageService)
        {
            _cache = cache;
            _ruleManageService = ruleManageService;
        }

        //khushali 21-02-2019
        #region "Access Rights Integartion"

        public async Task<UserAccessRightsViewModel> GetAssignedRule(long GroupID)
        {
            try
            {
                return await _ruleManageService.GetAssignedRule(GroupID);
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }            
        }

        public async Task<UserAccessRightsViewModel> GetAssignedRuleV1(long GroupID)
        {
            try
            {
                return await _ruleManageService.GetAssignedRuleV1(GroupID);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<UserAccessRightsViewModel> GetMenuAssignedRule(long GroupID)
        {
            try
            {
                return await _ruleManageService.GetMenuAssignedRule(GroupID);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<UserAccessRightsViewModel> GetConfigurePermissions()
        {
            try
            {
                //return await _cache.GetOrCreateAsync(_velocityRuleKey, async entry =>
                //{
                //    entry.SlidingExpiration = _defaultCacheDuration;
                    return await _ruleManageService.GetConfigurePermissions();
                //});
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<UserAccessRightViewModel> CreateAssignedRule(long GroupID)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                return await _ruleManageService.CreateAssignedRule(GroupID);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }            
        }

        public async Task<BizResponseClass> CreateAssignedRuleV2(UserAccessRightsViewModel Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                return await _ruleManageService.CreateAssignedRuleV2(Request);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }            
        }

        public async Task<BizResponseClass> UpdateAssignedRule(UserAccessRightsViewModel Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                return await _ruleManageService.UpdateAssignedRule(Request);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        #endregion

        //khushali 13-02-2019
        #region "Module Master"

        //public async Task<ListModuleDataViewModel> CacheDataAllModuleData()
        //{
        //    try
        //    {
        //        _cache.Remove(_moduleKey);
        //        return await _cache.GetOrCreateAsync(_moduleKey, async entry =>
        //        {
        //            entry.SlidingExpiration = _defaultCacheDuration;
        //            return await _ruleManageService.GetAllModuleData();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        return null;
        //    }
        //}
        //int PageNo, int PageSize
        public async Task<ListModuleDataViewModel> GetAllModuleData(int PageNo, int PageSize, short? AllData = null, bool IsParentList = false)
        {
            try
            {
                //return await _cache.GetOrCreateAsync(_moduleKey, async entry =>
                //{
                //    entry.SlidingExpiration = _defaultCacheDuration;
                //    return await _ruleManageService.GetAllModuleData(PageNo, PageSize);
                //});
                return await _ruleManageService.GetAllModuleData(PageNo, PageSize,AllData,IsParentList);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<ListModuleDataViewModel> GetAllModuleDataV2(int PageNo, int PageSize)
        {
            try
            {
                return await _ruleManageService.GetAllModuleData(PageNo, PageSize);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<ModuleDataViewModel> GetModuleDataByID(long Id)
        {
            try
            {
                return await _ruleManageService.GetModuleDataByID(Id);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<long> AddModuleData(ModuleDataViewModel request, long UserId)
        {
            try
            {                
                var newModel = await _ruleManageService.AddModuleData(request,UserId);
                //await CacheDataAllModuleData();
                return newModel;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public async Task<BizResponseClass> UpdateModuleData(ModuleDataViewModel request, long UserId)
        {
            try
            {
                var Result = await _ruleManageService.UpdateModuleData(request, UserId);
                //await CacheDataAllModuleData();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<bool> ChangeStatusModuleData(long id, short Status)
        {
            try
            {
                var Result = await _ruleManageService.ChangeStatusModuleData(id, Status);
                //await CacheDataAllModuleData();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        #endregion

        //khushali 13-02-2019
        #region "Sub Module Master"

        //public async Task<ListSubModuleDataViewModel> CacheDataAllSubModuleData()
        //{
        //    try
        //    {
        //        _cache.Remove(_SubModuleKey);
        //        return await _cache.GetOrCreateAsync(_SubModuleKey, async entry =>
        //        {
        //            entry.SlidingExpiration = _defaultCacheDuration;
        //            return await _ruleManageService.GetAllSubModuleData();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        return null;
        //    }
        //}

        public async Task<ListSubModuleDataViewModel> GetAllSubModuleData(int PageNo, int PageSize, short? AllData = null, bool IsParentList = false)
        {
            try
            {
                //return await _cache.GetOrCreateAsync(_SubModuleKey, async entry =>
                //{
                //    entry.SlidingExpiration = _defaultCacheDuration;
                //    return await _ruleManageService.GetAllSubModuleData(PageNo, PageSize);
                //});
                return await _ruleManageService.GetAllSubModuleData(PageNo, PageSize,AllData,IsParentList);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<SubModuleDataViewModel> GetSubModuleDataByID(long Id)
        {
            try
            {
                return await _ruleManageService.GetSubModuleDataByID(Id);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<long> AddSubModuleData(SubModuleDataViewModel request, long UserId)
        {
            try
            {
                var Result =  await _ruleManageService.AddSubModuleData(request, UserId);
                //await CacheDataAllSubModuleData();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public async Task<BizResponseClass> UpdateSubModuleData(SubModuleDataViewModel request, long UserId)
        {
            try
            {
                var Result  = await _ruleManageService.UpdateSubModuleData(request, UserId);
                //await CacheDataAllSubModuleData();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<bool> ChangeStatusSubModuleData(long id, short Status)
        {
            try
            {
                var Result = await _ruleManageService.ChangeStatusSubModuleData(id, Status);
                //await CacheDataAllSubModuleData();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        #endregion

        //khushali 13-02-2019
        #region "Field Master"

        //public async Task<List<FieldDataViewModel>> CacheDataAllFieldData()
        //{
        //    try
        //    {
        //        _cache.Remove(_FieldKey);
        //        return await _cache.GetOrCreateAsync(_FieldKey, async entry =>
        //        {
        //            entry.SlidingExpiration = _defaultCacheDuration;
        //            return await _ruleManageService.GetAllFieldData();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        return null;
        //    }
        //}

        public async Task<ListFieldDataViewModel> GetAllFieldData(int PageNo, int PageSize, short? AllData = null)
        {
            ListFieldDataViewModel Response = new ListFieldDataViewModel();
            try
            {
                //return await _cache.GetOrCreateAsync(_FieldKey, async entry =>
                //{
                //    entry.SlidingExpiration = _defaultCacheDuration;
                //    return await _ruleManageService.GetAllFieldData(PageNo, PageSize);
                //});
                return await _ruleManageService.GetAllFieldData(PageNo, PageSize,AllData);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<FieldDataViewModel> GetFieldDataByID(long Id)
        {
            try
            {
                return await _ruleManageService.GetFieldDataByID(Id);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<long> AddFieldData(FieldDataViewModel request, long UserId)
        {
            try
            {
                var Result =  await _ruleManageService.AddFieldData(request, UserId);
                //await CacheDataAllFieldData();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public async Task<BizResponseClass> UpdateFieldData(FieldDataViewModel request, long UserId)
        {
            try
            {
                var Result = await _ruleManageService.UpdateFieldData(request, UserId);
                //await CacheDataAllFieldData();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<bool> ChangeStatusFieldData(long id, short Status)
        {
            try
            {
                var Result =  await _ruleManageService.ChangeStatusFieldData(id, Status);
                //await CacheDataAllFieldData();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        #endregion

        //khushali 13-02-2019
        #region "Tool Master"

        //public async Task<List<ToolDataViewModel>> CacheDataAllToolData()
        //{
        //    try
        //    {
        //        _cache.Remove(_ToolKey);
        //        return await _cache.GetOrCreateAsync(_ToolKey, async entry =>
        //        {
        //            entry.SlidingExpiration = _defaultCacheDuration;
        //            return await _ruleManageService.GetAllToolData();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        return null;
        //    }
        //}

        public async Task<ListToolDataViewModel> GetAllToolData(int PageNo, int PageSize, short? AllData = null)
        {
            try
            {
                //return await _cache.GetOrCreateAsync(_ToolKey, async entry =>
                //{
                //    entry.SlidingExpiration = _defaultCacheDuration;
                //    return await _ruleManageService.GetAllToolData(PageNo, PageSize);
                //});
                return await _ruleManageService.GetAllToolData(PageNo, PageSize,AllData);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<ToolDataViewModel> GetToolDataByID(long Id)
        {
            try
            {
                var Result =  await _ruleManageService.GetToolDataByID(Id);
                //await CacheDataAllToolData();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<long> AddToolData(ToolDataViewModel request, long UserId)
        {
            try
            {
                var Result =  await _ruleManageService.AddToolData(request, UserId);
                //await CacheDataAllToolData();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public async Task<BizResponseClass> UpdateToolData(ToolDataViewModel request, long UserId)
        {
            try
            {
                var Result =  await _ruleManageService.UpdateToolData(request, UserId);
                //await CacheDataAllToolData();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<bool> ChangeStatusToolData(long id, short Status)
        {
            try
            {
                var Result =  await _ruleManageService.ChangeStatusToolData(id, Status);
                //await CacheDataAllToolData();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        #endregion
        
    }
}
