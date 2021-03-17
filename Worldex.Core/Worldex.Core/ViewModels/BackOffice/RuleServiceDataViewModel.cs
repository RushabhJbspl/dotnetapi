using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Backoffice.RoleManagement;
using Worldex.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.BackOffice
{
    // Added region to seperate class entities. -Nishit Jani on A 2019-03-28 6:16 PM
    #region "Existing clases"
    public class ModuleDataViewModel
    {
        public long ModuleID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14005")]
        public string ModuleName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14006")]
        [EnumDataType(typeof(EnAccessStatus), ErrorMessage = "1,Invalid Status,14017")]
        public EnAccessStatus Status { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14057")]
        public long ParentID { get; set; } // khushali 07-03-2019 for parent Child tree
    }

    public class ListModuleDataViewModel : BizResponseClass
    {
        public List<ModuleDataViewModel> Result { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class ResponseModuleDataViewModel : BizResponseClass
    {
        public ModuleDataViewModel Result { get; set; }
    }

    public class SubModuleDataViewModel
    {
        public long SubModuleID { get; set; }
        //[Required(ErrorMessage = "1,Please Enter Required Parameter,14007")]
        //public long ModuleID { get; set; };
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14008")]
        public string SubModuleName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14009")]
        [EnumDataType(typeof(EnAccessStatus), ErrorMessage = "1,Invalid Status,14017")]
        public EnAccessStatus Status { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14057")]
        public Guid ParentGUID { get; set; } // khushali 07-03-2019 for parent Child tree
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14057")]
        public Guid GUID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14057")]
        public short Type { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14057")]
        public short ModuleDomainType { get; set; }
        public string MethodName { get; set; }
        public string Path { get; set; }
        public string Controller { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14057")]
        public string UtilityTypes { get; set; } = "100";
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14057")]
        public string CrudTypes { get; set; } = "100";
    }

    public class SubModuleDataViewModelv1
    {
        public string SubModuleName { get; set; }
        public short Status { get; set; } = 99;
        public Guid ParentGUID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14057")]
        public Guid GUID { get; set; }
        public short Type { get; set; } = 99;
        public string UtilityTypes { get; set; }
        public string CrudTypes { get; set; }
    }

    public class ListSubModuleDataViewModel : BizResponseClass
    {
        public List<SubModuleDataViewModel> Result { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class ResponseSubModuleDataViewModel : BizResponseClass
    {
        public SubModuleDataViewModel Result { get; set; }
    }

    public class FieldDataViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14010")]
        public Guid ModulleGUID { get; set; }
        public long FieldID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14011")]
        public string FieldName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14012")]
        [EnumDataType(typeof(EnAccessStatus), ErrorMessage = "1,Invalid Status,14017")]
        public EnAccessStatus Status { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14018")]
        public short Required { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14019")]
        public short AccressRight { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14020")]
        public Guid GUID { get; set; }
    }

    public class FieldDataViewModelv1
    {
        public string FieldName { get; set; }
        public short Status { get; set; } = 99;
        public short Required { get; set; } = 99;
        public short AccressRight { get; set; } = 99;
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14011")]
        public Guid GUID { get; set; }
    }

    public class ListFieldDataViewModel : BizResponseClass
    {
        public List<FieldDataViewModel> Result { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class ResponseFieldDataViewModel : BizResponseClass
    {
        public FieldDataViewModel Result { get; set; }
    }

    public class ToolDataViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14013")]
        public long SubModuleID { get; set; }
        public long ToolID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14014")]
        public string ToolName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14015")]
        [EnumDataType(typeof(EnAccessStatus), ErrorMessage = "1,Invalid Status,14017")]
        public EnAccessStatus Status { get; set; }
    }

    public class ListToolDataViewModel : BizResponseClass
    {
        public List<ToolDataViewModel> Result { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class ResponseToolDataViewModel : BizResponseClass
    {
        public ToolDataViewModel Result { get; set; }
    }

    public class UserAccessRightViewModel : BizResponseClass
    {
        public UserAccessRights Result { get; set; }
    }

    public class UserAccessRightRequest
    {
        public UserAccessRights Data { get; set; }
        public long PermissionGroupID { get; set; }
    }

    public class UserAccessRightsViewModel
    {
        public long GroupID { get; set; }
        public long ID { get; set; }
        public virtual List<UserAssignModuleViewModel> Modules { get; set; }
    }




    public class UserAccessRightsResponse : BizResponseClass
    {
        public UserAccessRightsViewModel Result { get; set; }
    }

    public class UserAssignModuleViewModel
    {
        public long ModuleID { get; set; }
        public long ID { get; set; }
        public short Status { get; set; }
        public string ModuleName { get; set; }
        public long ParentID { get; set; }// khushali 07-03-2019 for parent Child tree
        public virtual List<UserAssignSubModuleViewModel> SubModules { get; set; }
        public List<UserAssignModuleViewModel> ChildNode = new List<UserAssignModuleViewModel>();
    }

    public class UserAssignSubModuleViewModel
    {
        public long SubModuleID { get; set; }
        public long ID { get; set; }
        public short Status { get; set; }
        public string SubModuleName { get; set; }
        public short View { get; set; }
        public short Create { get; set; }
        public short Edit { get; set; }
        public short Delete { get; set; }
        public long ParentID { get; set; }// khushali 07-03-2019 for parent Child tree
        public List<UserAssignFieldRightsViewModel> Fields = new List<UserAssignFieldRightsViewModel>(); // { get; set; }
        public List<UserAssignToolRightsViewModel> Tools = new List<UserAssignToolRightsViewModel>(); // { get; set; }
        public List<UserAssignSubModuleViewModel> ChildNodes = new List<UserAssignSubModuleViewModel>(); // { get; set; }
    }

    public class UserAssignFieldRightsViewModel
    {
        public long FieldID { get; set; }
        public long ID { get; set; }
        public string FieldName { get; set; }
        public short Status { get; set; }
        public short IsVisibility { get; set; }
    }

    public class UserAssignToolRightsViewModel
    {
        public long ToolID { get; set; }
        public long ID { get; set; }
        public string ToolName { get; set; }
        public short Status { get; set; }
    }

    public class ReInviteUserViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14055")]
        public string Email { get; set; }
    }

    public class MasterUserAssignSubModuleViewModel : UserAssignSubModuleViewModel
    {
        public new List<UserAssignSubModuleViewModel> ChildNodes;
    }

    #endregion

    // Added region to seperate class entities. -Nishit Jani on A 2019-03-28 6:16 PM
    #region "Menu Access View Model Classes by OpenID Connect"
    // Added by Nishit Jani on A 2019-03-28 4:50 PM 
    // As there are some change in Array    
    public class MenuAccessResponse : BizResponseClass
    {
        public MenuAccessDetailViewModel Result { get; set; }
    }

    public class MenuAccessDetailViewModel
    {
        public long GroupID { get; set; }
        public long ID { get; set; }
        public virtual List<MenuSubDetailViewModel> Modules { get; set; }

    }

    public class MenuSubDetailViewModel
    {
        public long ModuleGroupAccessID { get; set; } = 0; // khushali 30-04-2019 for CURD (update)
        public long ModuelID { get; set; }
        public long ID { get; set; }
        public short Status { get; set; }
        public string ModuleName { get; set; }
        public long ParentID { get; set; }
        public string Type { get; set; }
        public List<string> Utility { get; set; }
        public List<string> CrudOption { get; set; }
        public string GUID { get; set; }
        public string ParentGUID { get; set; }
        public bool HasChild { get; set; }
        public virtual List<ChildNodeViewModel> ChildNodes { get; set; }
        public bool HasField { get; set; }
        public bool HasNext { get; set; } = false;
        public virtual List<ChildNodeFiledViewModel> Fields { get; set; }
    }

    public class ChildNodeViewModel
    {
        public long ModuelID { get; set; }
        public long ID { get; set; }
        public int Status { get; set; }
        public string ModuleName { get; set; }
        public long ParentID { get; set; }
        public string Type { get; set; }
        public List<string> Utility { get; set; }
        public List<string> CrudOption { get; set; }
        public string GUID { get; set; }
        public string ParentGUID { get; set; }
        public bool HasChild { get; set; } = false;
        public virtual List<InnerChildNodeViewModel> ChildNodes { get; set; }
        public bool HasField { get; set; }
        public virtual List<ChildNodeFiledViewModel> Fields { get; set; }
    }

    public class InnerChildNodeViewModel
    {
        public List<string> Tools { get; set; }
        public long SubModuleID { get; set; }
        public long ID { get; set; }
        public int Status { get; set; }
        public string SubModuleName { get; set; }
        public long ParentID { get; set; }
        public string Type { get; set; }
        public List<string> Utility { get; set; }
        public List<string> CrudOption { get; set; }
        public string GUID { get; set; }
        public string ParentGUID { get; set; }
        public bool HasChild { get; set; } = false;
        public List<InnerChildNodeViewModel> ChildNodes { get; set; }
        public bool HasFields { get; set; } = false;
        public virtual List<ChildNodeFiledViewModel> Fields { get; set; }
    }

    public class ChildNodeFiledViewModel //komal 16-04-2019 change parameter 
    {
        public long ModuleFormAccessID { get; set; } = 0;// khushali 30-04-2019 for CURD (update)
        public long FiledID { get; set; }
        public string FieldName { get; set; }
        public string Required { get; set; }
        public string AccessRight { get; set; }
        public string Visibility { get; set; }
        public Guid GUID { get; set; }
    }

    public class ModuleGroupAccessQryRes
    {
        public long Id { get; set; }
        public int GroupID { get; set; }
        public string UtilityTypes { get; set; } = "100";
        public string CrudTypes { get; set; } = "100";
        public int SubModuleID { get; set; }
        public short Status { get; set; }
    }
    #endregion

    #region CURD for Access Rights 
    // khushali 30-04-2019 for CURD (Get Data)
    public class MenuSubDetailViewModelV2
    {
        public long ModuleGroupAccessID { get; set; }
        public long ModuelID { get; set; }
        public long ID { get; set; }
        public short Status { get; set; }
        public string ModuleName { get; set; }
        public long ParentID { get; set; }
        public string Type { get; set; }
        public string Utility { get; set; }
        public string CrudOption { get; set; }
        public Guid GUID { get; set; }
        public Guid ParentGUID { get; set; }
        public int HasChild { get; set; }
        public int HasField { get; set; }
    }

    public class MenuSubDetailReq
    {
        public long GroupID { get; set; }
        public List<MenuSubDetail> Data { get; set; }
    }

    public class MenuSubDetail
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14062")]
        public long ModuleGroupAccessID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14063")]
        public long ModuelID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14064")]
        public short Status { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14065")]
        public List<string> Utility { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14066")]
        public List<string> CrudOption { get; set; }
    }

    public class ChildNodeFiledReq
    {
        public List<ChildNodeFiled> Data { get; set; }
    }

    public class ChildNodeFiled
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14067")]
        public long ModuleGroupAccessID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14068")]
        public long ModuleFormAccessID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14069")]
        public long FiledID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14070")]
        public string Visibility { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14071")]
        public string AccessRights { get; set; }
    }

    #endregion
}
