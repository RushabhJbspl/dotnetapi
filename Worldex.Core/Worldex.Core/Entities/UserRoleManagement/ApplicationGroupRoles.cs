using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.UserRoleManagement
{
    public class ApplicationGroupRoles
    {
        public long PermissionGroupId { get; set; }

        [Key]
        public long RoleId { get; set; }
    }
}
