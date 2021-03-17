using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
    public class ZipCodeMaster : BizBase
    {
        [Required]
        public long ZipCode { get; set; }
        [Required]
        [StringLength(30)]
        public string ZipAreaName { get; set; }
        [Required]
        public long CityID { get; set; }
    }
}
