using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.User
{
    public partial interface IUserConfiguration
    {
        void Add(int UserId, string Type, string ConfigurationValue, bool EnableStatus);

        Task<UserConfigurationMasterViewModel> Get(int Id);

        void update(int Id);
    }
}
