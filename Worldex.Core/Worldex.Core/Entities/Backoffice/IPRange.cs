using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Backoffice
{
   public class IPRange : BizBaseExtended
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string StartIp { get; set; }

        [Required]
        [StringLength(20)]
        public string EndIp { get; set; }
    }
}
