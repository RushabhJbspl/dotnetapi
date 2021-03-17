using System.ComponentModel.DataAnnotations;
using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels.ManageViewModels
{
    public class IndexViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Phone]
        [Display(Name = "Mobile number")]
        public string MobileNo { get; set; }

        public bool TwoFactorEnabled { get; set; }
        public short IsDeviceEnabled { get; set; } //add by mansi
        public string SocialProfile { get; set; }
    }

    public class UserInfoResponse : BizResponseClass
    {
        public IndexViewModel UserData { get; set; }
    }

    public class OrganizationUserDataViewModel
    {
        [StringLength(500)]
        public string OrganizationName { get; set; }
        
        [Required(ErrorMessage = "1,Please Enter Email Id,9012")]
        [StringLength(50, ErrorMessage = "1,Please Enter Valid Email Id,9013")]
        [RegularExpression(@"^[-a-zA-Z0-9~!$%^&*_=+}{\'?]+(\.[-a-zA-Z0-9~!$%^&*_=+}{\'?]+)*@([a-zA-Z0-9_][-a-zA-Z0-9_]*(\.[-a-zA-Z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-zA-Z]{2,3})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$", ErrorMessage = "1,Please enter a valid Email Address,9014")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Phone]
        [Display(Name = "Mobile number")]
        public string MobileNo { get; set; }

        [Display(Name = "Fax")]
        public string Fax { get; set; }

        [StringLength(500)]
        public string Website { get; set; }

        public int LanguageId { get; set; }

        [Phone]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [StringLength(1000)]
        public string Street { get; set; }

        [StringLength(250)]
        public string City { get; set; }

        [StringLength(50)]
        public string PinCode { get; set; }

        public int CountryId { get; set; }

        public int StateId { get; set; }
    }

    public class OrganizationUserResponse : BizResponseClass
    {
        
    }

    public class OrganizationUserDataResponse : BizResponseClass
    {
        public OrganizationUserDataViewModel UserData { get; set; }
    }

}
