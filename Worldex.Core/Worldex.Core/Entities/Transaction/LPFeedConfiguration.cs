using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Transaction
{
    public class LPFeedConfiguration : BizBase 
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
    }
}
