using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.KYC
{
    public class KYCLevelMaster : BizBase
    {
        [Required]
        [StringLength(150)]
        public string KYCName { get; set; }

        public int Level { get; set; }

        public bool EnableStatus { get; set; }

        public bool IsDelete { get; set; }
    }
}
