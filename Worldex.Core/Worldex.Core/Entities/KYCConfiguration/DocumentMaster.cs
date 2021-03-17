using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.KYCConfiguration
{
    public class DocumentMaster : BizBaseExtended
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
