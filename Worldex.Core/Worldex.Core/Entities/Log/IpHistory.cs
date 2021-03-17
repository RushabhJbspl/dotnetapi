using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Log
{
    public class IpHistory : BizBase
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(15)]
        public string IpAddress { get; set; }
        [Required]
        [StringLength(250)]
        public string Location { get; set; }
    }
}
