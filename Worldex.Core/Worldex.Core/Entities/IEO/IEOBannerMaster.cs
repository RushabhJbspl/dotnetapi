using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.IEO
{
    public class IEOBannerMaster : BizBase
    {
        [Required]
        [StringLength(50)]
        public string GUID { get; set; }
        [Required]
        public string BannerPath { get; set; }
        [Required]
        [StringLength(50)]
        public string BannerName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string TermsAndCondition { get; set; }
        [Required]
        public short IsKYCReuired { get; set; }
    }
}