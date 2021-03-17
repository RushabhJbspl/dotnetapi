using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Affiliate
{
    public class AffiliateSchemeMaster : BizBase
    {
        [Required]
        public string SchemeType { get; set; }

        [Required]
        public string SchemeName { get; set; }
    }
}
