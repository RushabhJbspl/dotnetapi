using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Entities.SignalR;
using Worldex.Core.ViewModels.Configuration.FeedConfiguration;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.FeedConfiguration
{
    public interface IExchangeFeedConfiguration
    {
        List<SocketFeedConfiguration> GetFeedConfigurations();
        List<SocketFeedLimits> GetFeedLimits();
        List<FeedLimitCounts> GetLimitCounts();
        void ReloadFeedLimitCount();
        void UpdateAndReloadFeedLimitCount(FeedLimitCounts Data);
        BizResponseClass CheckFeedLimit(short MethodID);
        BizResponseClass CheckFeedDataLimit(long DataSize, short MethodID);
        SocketMethodResponse GetSocketMethods();
        ExchangeLimitTypeResponse GetExchangeFeedLimitType();
        BizResponseClass AddFeedConfigurationLimit(SocketFeedLimitsRequest Request, long UserID);
        BizResponseClass UpdateFeedConfigurationLimit(SocketFeedLimitsRequest Request, long UserID);
        SocketFeedLimitsResponse GetAllFeedConfigurationLimit();
        SocketFeedLimitsListResponse GetSocketFeedLimitsLists();
        BizResponseClass AddSocketFeedConfig(SocketFeedConfigurationRequest Request, long UserID);
        BizResponseClass UpdateSocketFeedConfig(SocketFeedConfigurationRequest Request, long UserID);
        SocketFeedConfigResponse GetAllFeedConfiguration();
    }
}
