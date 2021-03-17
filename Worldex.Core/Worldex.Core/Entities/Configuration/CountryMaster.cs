using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
    public class CountryMaster : BizBase
    {
        [Required]
        [StringLength(100)]
        public string CountryName { get; set; }
        [Required]
        [StringLength(2)]
        public string CountryCode { get; set; }

        public long CountryDialingCode { get; set; } = 0;
    }
}
