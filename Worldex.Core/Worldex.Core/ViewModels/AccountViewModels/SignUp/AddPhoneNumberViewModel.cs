using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.SignUp
{
    public class AddPhoneNumberViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Mobile Number, 4012")]
        [Phone(ErrorMessage = "1,Please Enter Valid Mobile Number, 4013")]

        public string PhoneNumber { get; set; }
    }
}
