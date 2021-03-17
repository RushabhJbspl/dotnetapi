using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.User
{
   public class AllowedIPAddress : BizBaseExtended
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(30)]
        public string FromIPAddress { get; set; }
        [Required]
        [StringLength(30)]
        public string ToIPAddress { get; set; }
    }
}
