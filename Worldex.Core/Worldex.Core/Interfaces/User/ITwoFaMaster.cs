using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.User
{
    public partial interface ITwoFaMaster
    {
        Task<TwoFaMasterViewModel> AddOtpAsync(int UserId, string Email = null, string Mobile = null);
        Task<TwoFaMasterViewModel> GetOtpData(int Id);
        void UpdateOtp(long Id);
    }
}
