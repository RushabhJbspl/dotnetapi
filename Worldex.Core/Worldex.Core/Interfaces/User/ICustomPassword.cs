using Worldex.Core.ViewModels.AccountViewModels.Login;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.User
{
    public interface ICustomPassword
    {
        Task<CustomtokenViewModel> AddPassword(CustomtokenViewModel model);
        Task<CustomtokenViewModel> GetPassword(long userid);
        Task<CustomtokenViewModel> IsValidPassword(string appkey, string otp);
        void UpdateOtp(long Id);
        Task<string> Get2FACustomToken(long UserId);
        //2019-6-18
        bool AddActivityTypeLog(TypeLogRequest data);
    }
}
