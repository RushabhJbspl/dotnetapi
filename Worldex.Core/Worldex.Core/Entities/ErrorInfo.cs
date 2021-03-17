using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities
{
    public class ErrorInfo:BizBase
    {
        [Required]
        [StringLength(50)]
        public string FunctionName { get; set; }

        [Required]
        public long RefNo { get; set; }

        [Required]
        [StringLength(500)]
        public string ErrorMsg { get; set; } 
    }
}
