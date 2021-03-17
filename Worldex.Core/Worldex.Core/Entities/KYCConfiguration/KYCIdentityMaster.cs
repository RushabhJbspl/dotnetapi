using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.KYCConfiguration
{
    public class KYCIdentityMaster : BizBaseExtended
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(500)]
        public string DocumentMasterId { get; set; }
    }
}
