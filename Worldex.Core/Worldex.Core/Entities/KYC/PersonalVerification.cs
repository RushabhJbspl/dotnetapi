using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.KYC
{
    public class PersonalVerification : BizBase
    {

        public int UserID { get; set; }
        [Required]
        [StringLength(150)]
        public string Surname { get; set; }
        [Required]
        [StringLength(150)]
        public string GivenName { get; set; }        
        [Required]
        [StringLength(150)]
        public string ValidIdentityCard { get; set; }
        [Required]
        [StringLength(100)]
        public string IdentityDocNumber { get; set; }
        [Required]
        [StringLength(500)]
        public string FrontImage { get; set; }
        [Required]
        [StringLength(500)]
        public string BackImage { get; set; }
        [Required]
        [StringLength(500)]
        public string SelfieImage { get; set; }
        public bool EnableStatus { get; set; }
        public int VerifyStatus { get; set; }
        [StringLength(2000)]
        public string Remark { get; set; }
        public long KYCLevelId{ get; set; }
    }
}
