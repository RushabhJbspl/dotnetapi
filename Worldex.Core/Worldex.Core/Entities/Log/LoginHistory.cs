using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Log
{
    public class LoginHistory : BizBase
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(15)]
        public string IpAddress { get; set; }
        [Required]
        [StringLength(2000)]
        public string Device { get; set; }
        [Required]
        [StringLength(2000)]
        public string Location { get; set; }
    }
}
