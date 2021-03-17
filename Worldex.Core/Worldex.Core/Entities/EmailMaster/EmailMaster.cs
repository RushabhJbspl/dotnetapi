using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.EmailMaster
{
    public class EmailMaster : BizBaseExtended
    {
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
        [Required]
        public int UserId { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsDeleted { get; set; }

    }
}
