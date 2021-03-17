using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Profile_Management
{
    public class ProfileLevelMaster : BizBase
    {
        [Required]
        [StringLength(250)]
        public string ProfileName { get; set; }
    }
}
