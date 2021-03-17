using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Entities.Organization;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.AccountViewModels.Log;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Worldex.Infrastructure.Services.Log
{
    public class APIStatisticsService : IAPIStatistics
    {
        private readonly ICommonRepository<APIReqResStatistics> _APIReqResStatisticsRepository;
        private readonly ICommonRepository<PublicAPIReqResLog> _publicAPIReqResLogRepository;

        public APIStatisticsService(ICommonRepository<APIReqResStatistics> APIReqResStatisticsRepository, ICommonRepository<PublicAPIReqResLog> PublicAPIReqResLogRepository)
        {
            _APIReqResStatisticsRepository = APIReqResStatisticsRepository;
            _publicAPIReqResLogRepository = PublicAPIReqResLogRepository;
        }
        

        public long APIReqResStatistics(APIReqResStatistics model)
        {
            try
            {
                var IsExistData = _APIReqResStatisticsRepository.FindBy(e => e.UserID == model.UserID && e.MethodID == model.MethodID).SingleOrDefault();
                if(IsExistData != null)
                {
                    if (model.SuccessCount != 0)
                    {
                        IsExistData.SuccessCount++;
                    }
                    else if (model.FaliureCount != 0)
                    {
                        IsExistData.FaliureCount++;
                    }
                    IsExistData.UpdatedBy = model.UserID;
                    IsExistData.UpdatedDate = DateTime.UtcNow;
                    _APIReqResStatisticsRepository.Update(IsExistData);
                }
                else
                {
                    _APIReqResStatisticsRepository.Add(model);                  
                }                
                return model.Id;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return 0;
            }
        }

        public long PublicAPIReqResLog(PublicAPIReqResLog model)
        {
            try
            {
                _publicAPIReqResLogRepository.Add(model);
                return model.Id;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return 0;
                //throw;
            }
        }
    }
}
