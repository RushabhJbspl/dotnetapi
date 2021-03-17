using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
  public  class CronMaster : BizBase
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
