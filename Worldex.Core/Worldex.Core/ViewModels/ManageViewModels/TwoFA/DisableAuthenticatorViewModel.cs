using Worldex.Core.ApiModels;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.ManageViewModels.TwoFA
{
   public class DisableAuthenticatorViewModel
    {
        [Required(ErrorMessage = "1,Please enter veryfication code,4080")]
        //[StringLength(7, ErrorMessage = "1,The {0} must be at least {2} and at max {1} characters long.,4011,", MinimumLength = 6)]
        [StringLength(7, ErrorMessage = "1,The Verification Code must be at least 6 and at max 7 characters long.,17325", MinimumLength = 6)]//change by mansi-05-11-2019
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }
    }

    public class DisableAuthenticatorResponse : BizResponseClass
    {
    }

    public class DisableAuthenticatorViewModelV1 //add by mansi 06-09-2019
    {
        [Required(ErrorMessage = "1,Please Enter User Id,4072")]
        public int UserId { get; set; }
    }
}
