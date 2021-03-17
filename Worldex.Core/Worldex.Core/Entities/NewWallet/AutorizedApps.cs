using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.NewWallet
{
    public class AutorizedApps : BizBase
    {
        [Required]
        [StringLength(100)]
        public string AppName { get; set; }

        [Required]
        [StringLength(100)]
        public string SiteURL { get; set; }

        [StringLength(100)]
        [Required]
        public string SecretKey { get; set; }
    }
}
