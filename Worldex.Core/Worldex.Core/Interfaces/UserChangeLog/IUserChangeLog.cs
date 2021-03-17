using Worldex.Core.ViewModels.ManageViewModels.UserChangeLog;

namespace Worldex.Core.Interfaces.UserChangeLog
{
    public interface IUserChangeLog
    {
        long AddPassword(UserChangeLogViewModel model);
    }
}
