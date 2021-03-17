using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
    public class UserConfigurationMapping : BizBaseExtended
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public Guid ConfigurationMasterId { get; set; }
        public bool IsconfigurationEnable { get; set; }
    }
}
