using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.PhoneMaster
{
    public class PhoneMaster : BizBaseExtended
    {
        [Required]
        [StringLength(15)]
        public string Mobilenumber { get; set; }
        [Required]
        public int UserId { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsDeleted { get; set; }
    }
}
