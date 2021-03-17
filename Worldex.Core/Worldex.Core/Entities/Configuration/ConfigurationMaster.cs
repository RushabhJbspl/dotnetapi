using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
  public  class ConfigurationMaster : BizBaseExtended
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
