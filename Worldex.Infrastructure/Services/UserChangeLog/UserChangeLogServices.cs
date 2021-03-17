using Worldex.Core.Entities.UserChangeLog;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.UserChangeLog;
using Worldex.Core.ViewModels.ManageViewModels.UserChangeLog;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.UserChangeLog
{
  public  class UserChangeLogServices : IUserChangeLog
    {
        private readonly WorldexContext _dbContext;
     
        private readonly ICustomRepository<UserLogChange> _customRepository;
        private readonly ILogger<UserLogChange> _logger;

        public UserChangeLogServices(WorldexContext dbContext, 
            ICustomRepository<UserLogChange> customRepository,
            //IMessageRepository<Customtoken> customRepository,
            ILogger<UserLogChange> logger)
        {
            _dbContext = dbContext;
            
            _customRepository = customRepository;
            _logger = logger;
        }

        public long AddPassword(UserChangeLogViewModel model)
        {
            try
            {
                var userchangeLog = new UserLogChange
                {
                    UserId=model.Id,
                    CreatedDate= DateTime.UtcNow,
                    CreatedBy= model.Id,
                    Type=model.Type,
                    Oldvalue=model.Oldvalue,
                    Newvalue=model.Newvalue,
                    
                };

                _customRepository.Insert(userchangeLog);
                return model.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw;
            }
        }
    }
}
