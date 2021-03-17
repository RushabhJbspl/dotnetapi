using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.Login
{
    public class UnLockUserViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter User Id,4072")]
        public long UserId { get; set; }
    }

    public class UnLockUserResponseViewModel : BizResponseClass
    {
    }
}
