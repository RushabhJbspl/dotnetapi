using Worldex.Core.SharedKernel;
using System;

namespace Worldex.Core.ViewModels.AccountViewModels.SignUp
{
    public class TempOtpViewModel : BizBase
    {
        public int UserId { get; set; }        
        public int RegTypeId { get; set; }
        public string OTP { get; set; }
        public DateTime CreatedTime { get; set; }        
        public DateTime ExpirTime { get; set; }
        public bool EnableStatus { get; set; }
    }
}
