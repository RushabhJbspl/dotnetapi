using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
    public class CityMaster : BizBase
    {
        [Required]
        [StringLength(30)]
        public string CityName { get; set; }
        [Required]
        public long StateID { get; set; }
    }
}
