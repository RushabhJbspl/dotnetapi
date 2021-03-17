using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
    public class StateMaster : BizBase
    {
        [Required]
        [StringLength(30)]
        public string StateName { get; set; }
        [Required]
        [StringLength(2)]
        public string StateCode { get; set; }
        [Required]
        public long CountryID { get; set; }
    }
}
