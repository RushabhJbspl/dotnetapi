using Worldex.Core.Enums;
using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.UserRoleManagement
{
    public class RoleHistory : BizBase
    {
        [Required]
        [StringLength(250)]
        public string ModificationDetail { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        public EnModuleType Module { get; set; }

        public string IPAddress { get; set; }
    }
}
