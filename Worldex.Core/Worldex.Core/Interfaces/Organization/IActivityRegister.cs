using Worldex.Core.ViewModels.Organization;

namespace Worldex.Core.Interfaces.Organization
{
    public interface IActivityRegister
    {
        void AddActivityLog(ActivityRegisterViewModel activityRegisterViewModel,ActivityRegisterDetViewModel activityRegisterDetViewModel);
        void UpdateActivityLog(ActivityRegisterViewModel activityRegisterViewModel, ActivityRegisterDetViewModel activityRegisterDetViewModel);
    }
}
