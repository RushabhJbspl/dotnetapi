using System.Threading.Tasks;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;

namespace Worldex.Core.Interfaces.User
{
    public interface ITempUserRegisterService
    {
        bool GetMobileNumberCheck(string MobileNumber);
        bool GetMobileNumber(string MobileNumber);
        Task<TempUserRegisterViewModel> AddTempRegister(TempUserRegisterViewModel model);
        Task<TempUserRegisterViewModel> FindById(long Id);
        void Update(long Id);
        bool GetEmailCheckExist(string Email);
        bool GetEmail(string Email);
        bool GetUserNameCheckExist(string UserName);
        bool GetUserName(string UserName);
        Task<TempUserRegisterViewModel> GetMobileNo(string MobileNo);
        Task<TempUserRegisterViewModel> GetEmailDet(string Email);
    }
}
