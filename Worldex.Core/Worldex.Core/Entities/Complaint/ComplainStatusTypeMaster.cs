using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Complaint
{
    public class ComplainStatusTypeMaster : BizBase
    {
        [Required]
        [StringLength(100)]
        public string CompainStatusType { get; set; }

        public bool IsEnable { get; set; }

        public bool IsDeleted { get; set; }
    }
}
