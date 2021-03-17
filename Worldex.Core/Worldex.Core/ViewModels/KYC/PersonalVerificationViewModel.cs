using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.KYC
{
    public class PersonalVerificationViewModel : TrackerViewModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }
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
        [StringLength(150)]
        public string ValidIDCardNumber { get; set; }
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
        public long KYCLevelId { get; set; }

    }

    public class PersonalVerificationRequest
    {
        public string Surname { get; set; }
        public string GivenName { get; set; }
        public string ValidIdentityCard { get; set; }
        public string ValidIDCardNumber { get; set; }
        public string FrontImage { get; set; }
        public string BackImage { get; set; }
        public string SelfieImage { get; set; }
        public bool EnableStatus { get; set; }
        public int VerifyStatus { get; set; }
        public long KYCLevelId { get; set; }
    }

    public class PersonalVerificationResponse : BizResponseClass
    {
        public PersonalVerificationRequest UserKYC { get; set; }
    }
    public class UserKYCStatusViewModelResponse : BizResponseClass
    {
        public int KYCStatus { get; set; }
    }
}
