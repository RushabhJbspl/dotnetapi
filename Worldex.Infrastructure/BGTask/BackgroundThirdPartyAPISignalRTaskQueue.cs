using Microsoft.Extensions.Configuration;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Services.RadisDatabase;
using Foundatio.Queues;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Worldex.Core.SignalR;

namespace Worldex.Infrastructure.BGTask
{
    public interface ThirdPartyAPISignalRQueue
    {
        void Enqueue(ThirdPartyAPISinalR Data);
        Task<ThirdPartyAPISinalR> DequeueAsync(CancellationToken cancellationToken);
        Task<List<string>> GetClientAPIKeyByUserID(string ID);
    }
    public class BackgroundThirdPartyAPISignalRTaskQueue : ThirdPartyAPISignalRQueue
    {
        private IQueue<ThirdPartyAPISinalR> _workItems = new InMemoryQueue<ThirdPartyAPISinalR>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);
        private ICommonRepository<UserAPIKeyDetails> _userAPIKeyDetailsRepository;
        private RedisConnectionFactory _fact;
        private readonly IConfiguration _configuration;

        public BackgroundThirdPartyAPISignalRTaskQueue(ICommonRepository<UserAPIKeyDetails> UserAPIKeyDetailsRepository,
            RedisConnectionFactory Factory,IConfiguration Configuration)
        {
            _userAPIKeyDetailsRepository = UserAPIKeyDetailsRepository;
            _fact = Factory;
            _configuration = Configuration;
        }

        public async Task<ThirdPartyAPISinalR> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);

            var Data = await _workItems.DequeueAsync(cancellationToken);
            return Data.Value;
        }

        public void Enqueue(ThirdPartyAPISinalR Data)
        {
            if (Data == null)
            {
                throw new ArgumentNullException(nameof(Data));
            }

            _workItems.EnqueueAsync(Data);

            _signal.Release();
        }

        public async Task<List<string>> GetClientAPIKeyByUserID(string ID)
        {
            List<string> ClientAPIKeyList = new List<string>();
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                var APIKeyList = await _userAPIKeyDetailsRepository.FindByAsync(o => o.UserID == Convert.ToInt64(ID) && o.Status == 1);
                if (APIKeyList != null)
                {
                    foreach (var keyDetails in APIKeyList)
                    {
                        ConnetedClientList Cleint = Redis.GetData(_configuration.GetValue<string>("SignalRKey:RedisClientConnection") + keyDetails.APIKey);
                        if (Cleint != null && !string.IsNullOrEmpty(Cleint.ConnectionId))
                        {
                            ClientAPIKeyList.Add(Cleint.ConnectionId);
                        }
                    }
                }
                return await Task.FromResult(ClientAPIKeyList);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
    }

    public class SendToThirdPartyAPISignalREventHandler : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ThirdPartyAPISignalRQueue _ThirdPartyAPISignalRQueue;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private Task _dequeueDataTask;
        private readonly IMediator _mediator;
       

        public SendToThirdPartyAPISignalREventHandler(IServiceProvider serviceProvider, ThirdPartyAPISignalRQueue ThirdPartyAPISignalRQueue, IMediator mediator)
        {
            _serviceProvider = serviceProvider;
            _ThirdPartyAPISignalRQueue = ThirdPartyAPISignalRQueue;
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
                ThirdPartyAPISinalR Data = await _ThirdPartyAPISignalRQueue.DequeueAsync(_stopTokenSource.Token);

                if (!_stopTokenSource.IsCancellationRequested)
                {
                    using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                    {
                        ISignalRQueue _signalRQueue =
                                            serviceScope.ServiceProvider.GetRequiredService<ISignalRQueue>();

                            Task.Run(() => _mediator.Send(Data));
                    }
                }
            }
        }
    }
}
