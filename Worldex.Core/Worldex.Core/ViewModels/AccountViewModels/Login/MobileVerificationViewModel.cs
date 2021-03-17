using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.Login
{
    public class MobileVerificationViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Mobile Number, 4012")]
        [Phone(ErrorMessage = "1,Please Enter Valid Mobile Number, 4013")]
        public string Mobile { get; set; }
    }

    public class MobileVerificationResponse : BizResponseClass
    {

    }
}
