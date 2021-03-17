using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.Login
{
    public class StandardLoginViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please enter a User Name,4001")]
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage = "1,Please enter a valid User Name,4002")]
        public string Username { get; set; }

        [Required(ErrorMessage = "1,Please Enter Password,4010")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "1,The Password must be at least 6 and at max 50 characters long,17322", MinimumLength = 6)]//change by mansi-05-11-2019
        //[StringLength(50, ErrorMessage = "1,The {0} must be at least {2} and at max {1} characters long,4011", MinimumLength = 6)]
        [Display(Name = "Password")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$", ErrorMessage = "1,Please Enter valid username or Password,4028")] // khushali 05-04-2019 chnage message at sigin time 
        public string Password { get; set; }
    }

    public class StandardLoginResponse : BizResponseClass
    {
        public string PreferedLanguage { get; set; }
    }

    public class StandardLogin2FAResponse : BizResponseClass
    {
        public string TwoFAToken { get; set; }
        public string AllowToken { get; set; }
    }
    public class StandardLoginAuthorizeFailResponse : BizResponseClass
    {
        public string AllowAuthorizeToken { get; set; }
    }

    public class StandardSuccessLoginResponse : BizResponseClass
    {
        public bool Thememode { get; set; }
        public string PreferedLanguage { get; set; }
    }

    public class GetUserData
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }

    public class GetUserDataResponse : BizResponseClass
    {
        public List<GetUserData> GetUserData { get; set; }
    }

    public class StandardLoginV2ViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please enter a User Name,4001")]
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage = "1,Please enter a valid User Name,4002")]
        public string Username { get; set; }

    }

}
