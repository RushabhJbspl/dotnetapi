using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.GroupModuleManagement
{
    public class ModuleGroupMaster : BizBase
    {
        [Required]
        [StringLength(50)]
        public string GroupName { get; set; }

        [Required]
        [StringLength(100)]
        public string GroupDescription { get; set; }

        // Created this column for future use. If we supposed to use Role with Group then we can use this column - Nishit Jani on A 2019-03-27 12:36 PM
        public int RoleID { get; set; } = 0;

        // Created this column to map Front or Back with Group. Future use, not cosidered currently. -Nishit Jani on A 2019-03-27 12:41 PM
        public int ModuleDomainID { get; set; } = 0;
    }
}
