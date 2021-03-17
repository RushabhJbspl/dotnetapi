using Worldex.Core.ViewModels.AccountViewModels.UserKey;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.User
{
    public interface IUserKeyMasterService
    {
        string Get2FACustomToken(long UserId);
        void UpdateOtp(long Id);
        List<UserKeyViewModel> GetUserUniqueKeyList(long userid);
        UserKeyViewModel GetUserUniqueKey(string useruniqueKey);
        UserKeyViewModel AddUniqueKey(UserKeyViewModel model);
    }
}
