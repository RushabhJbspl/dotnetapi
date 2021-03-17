using System.Threading.Tasks;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;

namespace Worldex.Core.Interfaces.User
{
    public interface IRegisterTypeService
    {
        Task<bool> GetRegisterType(string Type);
        void AddRegisterType(RegisterType model);
        Task<int> GetRegisterId(enRegisterType registertype);
    }
}
