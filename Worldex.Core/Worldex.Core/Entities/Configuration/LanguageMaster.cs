using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
    public class LanguageMaster : BizBaseExtended
    {
        [Required]
        [StringLength(100)]

        public string Languagename { get; set; }
    }
}
