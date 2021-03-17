using Worldex.Core.Entities.Communication;
using Worldex.Core.Helpers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Queues;
using Worldex.Core.Entities.SignalR;
using Worldex.Core.Interfaces.FeedConfiguration;
using Worldex.Core.Interfaces;

namespace Worldex.Infrastructure.BGTask
{
    //SignalR Queue
    public interface ISignalRQueue
    {
        void Enqueue(SignalRData Data);

        Task<SignalRData> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundSignalRTaskQueue : ISignalRQueue
    {
        //private ConcurrentQueue<SignalRData> _workItems =new ConcurrentQueue<SignalRData>();
        private IQueue<SignalRData> _workItems = new InMemoryQueue<SignalRData>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public async Task<SignalRData> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);

            var Data = await  _workItems.DequeueAsync(cancellationToken);
            return Data.Value;
        }

        public void Enqueue(SignalRData Data)
        {
            if (Data == null)
            {
                throw new ArgumentNullException(nameof(Data));
            }

            _workItems.EnqueueAsync(Data);

            _signal.Release();
        }

        //public async Task<SignalRData> DequeueAsync(CancellationToken cancellationToken)
        //{
        //    await _signal.WaitAsync(cancellationToken);

        //    _workItems.TryDequeue(out SignalRData Data);

        //    return Data;
        //}

        //public void Enqueue(SignalRData Data)
        //{
        //    if (Data == null)
        //    {
        //        throw new ArgumentNullException(nameof(Data));
        //    }

        //    _workItems.Enqueue(Data);

        //    _signal.Release();
        //}
    }

    public class SendToEventHandler : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISignalRQueue _signalRQueue;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private Task _dequeueDataTask;
        private readonly IMediator _mediator;

        public SendToEventHandler(IServiceProvider serviceProvider, ISignalRQueue signalRQueue, IMediator mediator)
        {
            _serviceProvider = serviceProvider;
            _signalRQueue = signalRQueue;
            _mediator = mediator;
            
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dequeueDataTask = Task.Run(DequeueWorkItemAsync);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stopTokenSource.Cancel();

            return Task.WhenAny(_dequeueDataTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task DequeueWorkItemAsync()
        {
            while (!_stopTokenSource.IsCancellationRequested)
            {
                SignalRData Data = await _signalRQueue.DequeueAsync(_stopTokenSource.Token);

                if (!_stopTokenSource.IsCancellationRequested)
                {
                    using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                    {
                        ISignalRQueue _signalRQueue =
                                            serviceScope.ServiceProvider.GetRequiredService<ISignalRQueue>();
                        HelperForLog.WriteLogForConnection("DequeueWorkItemAsync", "Before Send Data Call ", " -Data- " + Data );
                        
                         Task.Run(()=> _mediator.Send(Data));

                        HelperForLog.WriteLogForConnection("DequeueWorkItemAsync", "After Send Data Call ", " -Data- " + Data);
                    }

                }
            }

        }
    }

    //FeedLimitCounts update queue.

    public class FeedLimitCountsQueue : IFeedlimitcountQueue
    {
        private IQueue<FeedLimitCounts> _workItems = new InMemoryQueue<FeedLimitCounts>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public async Task<FeedLimitCounts> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            var Data = await _workItems.DequeueAsync(cancellationToken);
            return Data.Value;
        }

        public void Enqueue(FeedLimitCounts Data)
        {
            if (Data == null)
            {
                throw new ArgumentNullException(nameof(Data));
            }
            _workItems.EnqueueAsync(Data);
            _signal.Release();
        }
    }
    public class UpdateFeedLimitCounts : IHostedService
    {
        private Task _dequeueDataTask;
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private readonly IFeedlimitcountQueue _feedlimitcountQueue;
        private readonly IExchangeFeedConfiguration _exchangeFeedConfiguration;
        private ICommonRepository<FeedLimitCounts> _FeedLimitCountRepository;

        public UpdateFeedLimitCounts(IFeedlimitcountQueue feedlimitcountQueue, IServiceProvider serviceProvider,
            IExchangeFeedConfiguration exchangeFeedConfiguration, ICommonRepository<FeedLimitCounts> FeedLimitCountRepository)
        {
            _feedlimitcountQueue = feedlimitcountQueue;
            _serviceProvider = serviceProvider;
            _exchangeFeedConfiguration = exchangeFeedConfiguration;
            _FeedLimitCountRepository = FeedLimitCountRepository;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dequeueDataTask = Task.Run(DequeueWorkItemAsync);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stopTokenSource.Cancel();

            return Task.WhenAny(_dequeueDataTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
        private async Task DequeueWorkItemAsync()
        {
            while (!_stopTokenSource.IsCancellationRequested)
            {
                FeedLimitCounts Data = await _feedlimitcountQueue.DequeueAsync(_stopTokenSource.Token);

                if (!_stopTokenSource.IsCancellationRequested)
                {
                    using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                    {
                        IFeedlimitcountQueue _signalRQueue =
                                            serviceScope.ServiceProvider.GetRequiredService<IFeedlimitcountQueue>();

                        //_exchangeFeedConfiguration.UpdateAndReloadFeedLimitCount(Data);
                        var model = _FeedLimitCountRepository.GetById(Data.Id);
                        model.LimitCount = Data.LimitCount;
                        model.CreatedDate = Data.CreatedDate;
                        model.UpdatedDate = Data.UpdatedDate;
                        _FeedLimitCountRepository.Update(model);
                    }

                }
            }
        }
    }
}
