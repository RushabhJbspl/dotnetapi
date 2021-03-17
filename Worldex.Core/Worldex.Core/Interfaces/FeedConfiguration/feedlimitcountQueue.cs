using Worldex.Core.Entities.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.FeedConfiguration
{
    public interface IFeedlimitcountQueue
    {
        void Enqueue(FeedLimitCounts Data);

        Task<FeedLimitCounts> DequeueAsync(CancellationToken cancellationToken);
    }
}
