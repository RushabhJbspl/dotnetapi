using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Complaint
{
   public class Typemaster : BizBase 
    {
        [Required]
        [StringLength(100)]
        public string Type { get; set; }
        [Required]
        [StringLength(150)]
        public string SubType { get; set; }
    }
}
