using Worldex.Core.Entities.Configuration.FeedConfiguration;

namespace Worldex.Core.Interfaces.Log
{
    public interface IAPIStatistics
    {
        long APIReqResStatistics(APIReqResStatistics model);
        long PublicAPIReqResLog(PublicAPIReqResLog model);
    }
}
