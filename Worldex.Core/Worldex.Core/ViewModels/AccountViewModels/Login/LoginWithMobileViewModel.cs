using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.Login
{
    public class LoginWithMobileViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Mobile Number, 4012")]
        [Phone(ErrorMessage = "1,Please Enter Valid Mobile Number, 4013")]
        public string Mobile { get; set; }
    }

    public class LoginWithMobileViewModelV2 : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Mobile Number or Email Address, 4012")]       
        public string UserName { get; set; }
    }

    public class LoginWithMobileResponse : BizResponseClass
    {
        public string Appkey { get; set; }
    }

    public class GetLoginWithMobileViewModel
    {
        public string Mobile { get; set; }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
    }

}
