using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Complaint
{
    public class ComplaintPriorityMaster : BizBase
    {
        [Required]
        [StringLength(50)]
        public string Priority { get; set; }
        [Required]
        [StringLength(50)]
        public string PriorityTime { get; set; }
    }
}
