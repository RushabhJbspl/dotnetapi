using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.User;

namespace Worldex.Infrastructure.Services.User
{
    public class RegisterTypeService : IRegisterTypeService
    {
        private readonly WorldexContext _dbContext;

        public RegisterTypeService(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> GetRegisterType(string Type)
        {
            var RegTypedata = _dbContext.RegisterType.Where(i => i.Type == Type).FirstOrDefault();
            if (RegTypedata?.Type == Type)
                return true;
            else
                return false;
        }

        public void AddRegisterType(RegisterType model)
        {
            _dbContext.Add(model);
            _dbContext.SaveChanges();
        }

        public async Task<int> GetRegisterId(enRegisterType registertype)
        {
            var registerType = _dbContext.RegisterType.Where(i => i.Type == registertype.ToString()).FirstOrDefault();
            return Convert.ToInt16(registerType.Id);
        }
    }
}
