using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.ViewModels.APIConfiguration;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.DomainEvents
{
    public class APIStatisticsHandler : IRequestHandler<APIStatistics>
    {
        private readonly IAPIStatistics _IAPIStatistics;
        private readonly ICommonRepository<RestMethods> _restMethodsRepository;

        public APIStatisticsHandler(IAPIStatistics APIStatistics, ICommonRepository<RestMethods> RestMethodsRepository)
        {
            _IAPIStatistics = APIStatistics;
            _restMethodsRepository = RestMethodsRepository;
        }

        public Task<Unit> Handle(APIStatistics request, CancellationToken cancellationToken)
        {
            try
            {
                APIReqResStatistics APIStatistics = new APIReqResStatistics();
                PublicAPIReqResLog PublicAPILog = new PublicAPIReqResLog();
                long MethodID = _restMethodsRepository.FindBy(e => e.Path == request.Path && e.Status == 1).Select(o => o.Id).SingleOrDefault();
                APIStatistics.IPId = 0;
                APIStatistics.MethodID = MethodID;
                APIStatistics.Status = 1;
                APIStatistics.SuccessCount = request.IsSuccessFaliure == 1 ? 1 : 0;
                APIStatistics.FaliureCount = request.IsSuccessFaliure == 0 ? 1 : 0;
                APIStatistics.CreatedBy = request.UserID;
                APIStatistics.CreatedDate = DateTime.UtcNow;
                APIStatistics.UserID = request.UserID;

                PublicAPILog.Status = 1;
                PublicAPILog.CreatedBy = request.UserID;
                PublicAPILog.CreatedDate = DateTime.UtcNow;
                PublicAPILog.Browser = request.Device;
                PublicAPILog.Device = request.Mode;
                PublicAPILog.Host = request.Host;
                PublicAPILog.HTTPErrorCode = request.HTTPErrorCode;
                PublicAPILog.HTTPStatusCode = request.HTTPStatusCode;
                PublicAPILog.IPAddress = request.IPAddress;
                PublicAPILog.MethodID = MethodID;
                PublicAPILog.MethodType = request.MethodType;
                PublicAPILog.Path = request.Path;
                PublicAPILog.WhitelistIP = request.WhitelistIP;

                _IAPIStatistics.APIReqResStatistics(APIStatistics);
                _IAPIStatistics.PublicAPIReqResLog(PublicAPILog);
                return Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                ex.ToString();
                return Task.FromResult(new Unit());
            }
        }
    }
}
