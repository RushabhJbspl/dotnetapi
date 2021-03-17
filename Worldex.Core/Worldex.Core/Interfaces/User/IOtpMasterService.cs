using System.Threading.Tasks;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;

namespace Worldex.Core.Interfaces.User
{
    public partial interface IOtpMasterService
    {
        Task<OtpMasterViewModel> AddOtp(int UserId,string Email=null,string Mobile =null);
        Task<OtpMasterViewModel> GetOtpData(int Id, int OTPType);
        void UpdateOtp(long Id, short Status, string Message);        
        Task<OtpMasterViewModel> AddOtpForSignupuser(int UserId, string Email = null, string Mobile = null);
        void UpdateEmailAndMobileOTP(long id);
        int GetOTPCountByType(int Id, int OTPType);
        int GetTotalOTPCount(int Id);
    }
}
