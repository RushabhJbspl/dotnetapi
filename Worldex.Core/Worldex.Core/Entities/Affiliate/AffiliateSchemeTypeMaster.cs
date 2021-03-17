using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Affiliate
{
    public class AffiliateSchemeTypeMaster : BizBase
    {
        [Required]
        public string SchemeTypeName { get; set; }

        public string Description { get; set; }
    }
}
