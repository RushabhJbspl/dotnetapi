using System.Threading.Tasks;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;

namespace Worldex.Core.Interfaces.User
{
    public interface ITempOtpService
    {
        Task<TempOtpViewModel> AddTempOtp(int UserId, int RegTypeId);
        Task<TempOtpViewModel> GetTempData(int Id);
        void Update(long Id);
    }
}
