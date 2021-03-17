using Worldex.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Backoffice.RoleManagement
{
    public class ModuleMaster : BizBase
    {
        public string ModuleName { get; set; }        
        public long ParentID { get; set; }         
    }

    public class SubModuleMaster : BizBase
    {
        public string SubModuleName { get; set; }
        public long ModuleID { get; set; }
        public long ParentID { get; set; } // khushali 07-03-2019 for parent Child tree
        public Guid GUID { get; set; }
        public Guid ParentGUID { get; set; }
        public short Type { get; set; }
        public short ModuleDomainType { get; set; }
        public string MethodName { get; set; }
        public string Path { get; set; }
        public string Controller { get; set; }
        public string UtilityTypes { get; set; } = "100";
        public string CrudTypes { get; set; } = "100";
    }

    public class FieldMaster : BizBase
    {
        public string FieldName { get; set; }
        public long SubModuleID { get; set; }// foreign key=>ModuleGroupAccess->ID
        public short Required { get; set; }     //komal 3-04-2019 add foreign key=> ModuleFieldRequirerMaster
        public short AccressRight { get; set; } //komal 3-04-2019 add foreign key=> add ModuleAccessRightsMaster
        public Guid GUID { get; set; } //komal 3-04-2019 add 
    }

    public class ToolMaster : BizBase
    {
        public string ToolName { get; set; }
        public long SubModuleID { get; set; }
    }

    public class UserAccessRights : BizBase
    {
        public long GroupID { get; set; }
        public virtual List<UserAssignModule> Modules { get; set; }
        // Added to associate Group Id with User. -Nishit Jani on A 2019-03-27 3:00 PM
        public long UserID { get; set; }
    }

    public class UserAssignModule
    {
        [Key]
        public long ID { get; set; }
        public long ModuleID { get; set; }
        public long UserAccessRightsId { get; set; }
        public short Status { get; set; }
        public string ModuleName { get; set; }
        public long ParentID { get; set; } // khushali 07-03-2019 for parent Child tree
        public virtual List<UserAssignSubModule> SubModules { get; set; }
    }

    public class UserAssignSubModule
    {
        [Key]
        public long ID { get; set; }
        public long SubModuleID { get; set; }
        public long UserAssignModuleID { get; set; }
        public short Status { get; set; }
        public string SubModuleName { get; set; }
        public short View { get; set; }
        public short Create { get; set; }
        public short Edit { get; set; }
        public short Delete { get; set; }
        public long ParentID { get; set; } // khushali 07-03-2019 for parent Child tree
        public List<UserAssignFieldRights> Fields { get; set; }
        public List<UserAssignToolRights> Tools { get; set; }
    }

    public class UserAssignFieldRights
    {
        [Key]
        public long ID { get; set; }
        public long FieldID { get; set; }
        public long UserAssignSubModuleID { get; set; }
        public string FieldName { get; set; }
        public short Status { get; set; }
        public short IsVisibility { get; set; }
    }

    public class UserAssignToolRights
    {
        [Key]
        public long ID { get; set; }
        public long ToolID { get; set; }
        public long UserAssignSubModuleID { get; set; }
        public string ToolName { get; set; }
        public short Status { get; set; }
    }

    public class AddUserAccessRights
    {
        public long GroupID { get; set; }
        //public virtual ICollection<UserAssignModule> Modules { get; set; }
        public virtual List<UserAssignModule> Modules { get; set; }

    }

    public class ModuleTypeMaster : BizBase
    {
        [StringLength(20)]
        public string TypeName { get; set; }
        [StringLength(20)]
        public string TypeValue { get; set; }
    }
    public class ModuleUtilityMaster : BizBase
    {
        [StringLength(20)]
        public string UtilityName { get; set; }
        [StringLength(20)]
        public string UtilityValue { get; set; }
    }
    public class ModuleCRUDOptMaster : BizBase
    {
        [StringLength(20)]
        public string OptName { get; set; }
        [StringLength(20)]
        public string OptValue { get; set; }
    }
    public class ModuleDomainMaster : BizBase
    {
        [StringLength(20)]
        public string DomainName { get; set; }
    }

    public class ModuleVisibilityMaster : BizBase
    {
        [StringLength(20)]
        public string VisibilityName { get; set; }
        [StringLength(20)]
        public string VisibilityValue { get; set; }
    }

    public class ModuleFieldRequirerMaster : BizBase
    {
        [StringLength(20)]
        public string Name { get; set; }
        [StringLength(20)]
        public string Value { get; set; }
    }
    public class ModuleAccessRightsMaster : BizBase
    {
        [StringLength(20)]
        public string AccessRightName { get; set; }
        [StringLength(20)]
        public string AccessRightValue { get; set; }
    }
}
