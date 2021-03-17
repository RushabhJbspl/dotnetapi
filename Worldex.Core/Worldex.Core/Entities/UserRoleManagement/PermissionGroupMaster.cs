using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.UserRoleManagement
{
    public class PermissionGroupMaster : BizBase
    {
        [Required]
        [StringLength(50)]
        public string GroupName { get; set; }

        [StringLength(100)]
        public string GroupDescription { get; set; }

        public string IPAddress { get; set; }
    }
}
