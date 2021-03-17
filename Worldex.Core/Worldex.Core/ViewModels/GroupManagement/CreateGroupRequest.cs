using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.GroupManagement
{
    public class GroupList : BizResponseClass
    {
        public IList<ChangeGroupRequest> GroupListData { get; set; }
    }

    public class CreateGroupRequest
    {
        public int? RoleId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4169")]
        [StringLength(250)]
        public string GroupName { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4170")]
        [StringLength(250)]
        public string Description { get; set; }        

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4170")]        
        public int DomainID { get; set; }
    }

    public class ChangeGroupRequest
    {
        public int? RoleId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4169")]
        [StringLength(250)]
        public string GroupName { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4170")]
        [StringLength(250)]
        public string Description { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4174")]
        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Fail,4176")]
        public ServiceStatus Status { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4170")]
        public int DomainID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4170")]
        public int GroupID { get; set; }
    }
}
