using Worldex.Core.ViewModels.AccountViewModels.Log;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.Log
{
    public interface ISignupLogService
    {
        long AddSignUpLog(SignUpLogViewModel model);
        Task<SignUpLogResponse> GetSignUpLogHistoryByUserId(long UserId, int pageIndex, int pageSize);
        void UpdateVerifiedUser(int TempUserId, int UserId);
    }
}
