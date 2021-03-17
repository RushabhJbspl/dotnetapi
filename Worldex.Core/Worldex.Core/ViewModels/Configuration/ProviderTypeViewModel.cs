using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class ProviderTypeViewModel
    {
        public long Id { get; set; }

        [Required]
        [StringLength(20)]
        public string ServiveProTypeName { get; set; }
    }
}
