using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Worldex.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.BGTask
{   

    public interface ILPStatusCheck<T>
    {
        void Enqueue(T Req);

        Task<T> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BGTaskLPStatusCheck<T> : ILPStatusCheck<T>
    {
        private readonly ConcurrentQueue<T> _transactions = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim _TransactionEnqueuedSignal = new SemaphoreSlim(0);

        public void Enqueue(T Req)
        {
            if (Req == null)
            {
                throw new ArgumentNullException(nameof(Req));
            }

            _transactions.Enqueue(Req);

            _TransactionEnqueuedSignal.Release();
        }

        public async Task<T> DequeueAsync(CancellationToken cancellationToken)
        {
            await _TransactionEnqueuedSignal.WaitAsync(cancellationToken);

            _transactions.TryDequeue(out T Req);

            return Req;
        }
    }

    public class ProcessAllLPTransactionDequeuer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILPStatusCheck<LPStatusCheckData> _TransactionsQueue;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private readonly IMediator _mediator;
        private Task _dequeueMessagesTask;

        public ProcessAllLPTransactionDequeuer(IServiceProvider serviceProvider,
            ILPStatusCheck<LPStatusCheckData> TransactionsQueue, IMediator mediator)
        {
            _serviceProvider = serviceProvider;
            _TransactionsQueue = TransactionsQueue;
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dequeueMessagesTask = Task.Run(DequeueTransactionsAsync);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stopTokenSource.Cancel();

            return Task.WhenAny(_dequeueMessagesTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task DequeueTransactionsAsync()
        {
            while (!_stopTokenSource.IsCancellationRequested)
            {
                LPStatusCheckData Req = await _TransactionsQueue.DequeueAsync(_stopTokenSource.Token);

                if (!_stopTokenSource.IsCancellationRequested)
                {
                    using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                    {
                        Task.Run(()=>_mediator.Send(Req));
                    }
                }
            }

        }
    }
    

}
