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

    public interface ITransactionQueue<T>
    {
        void Enqueue(T Req);

        Task<T> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BGTaskTransactionQueue<T> : ITransactionQueue<T>
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

    public class ProcessAllTransactionDequeuer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITransactionQueue<NewTransactionRequestCls> _TransactionsQueue;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private readonly IMediator _mediator;
        private Task _dequeueMessagesTask;

        public ProcessAllTransactionDequeuer(IServiceProvider serviceProvider,
            ITransactionQueue<NewTransactionRequestCls> TransactionsQueue, IMediator mediator)//ITransactionProcess transactionProcess
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
                NewTransactionRequestCls Req = await _TransactionsQueue.DequeueAsync(_stopTokenSource.Token);

                if (!_stopTokenSource.IsCancellationRequested)
                {
                    using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                    {
                        //ITransactionProcess _transactionProcess =
                        //                    serviceScope.ServiceProvider.GetRequiredService<ITransactionProcess>();
                        //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("Enque Start", "TxnEnqueue", "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));
                        Task.Run(()=>_mediator.Send(Req));
                        Task.Delay(1000).Wait();

                        //if (Req.ISFollowersReq == 0)//only for leader's allow this
                        //{
                        //    FollowersOrderRequestCls request = new FollowersOrderRequestCls { Req = Req, Delivery_Currency = "LTC", Order_Currency = "BTC" };
                        //    Task.Run(() => _mediator.Send(request));
                        //}
                        //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("Enque END", "TxnEnqueue", "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));
                        //Thread.Sleep(2000);//rita 10-12-18 as milisecond txn create issue
                    }
                }
            }

        }
    }

    public class ProcessAllTransactionMarginDequeuer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITransactionQueue<NewTransactionRequestMarginCls> _TransactionsQueue;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private readonly IMediator _mediator;
        private Task _dequeueMessagesTask;

        public ProcessAllTransactionMarginDequeuer(IServiceProvider serviceProvider,
            ITransactionQueue<NewTransactionRequestMarginCls> TransactionsQueue, IMediator mediator)//ITransactionProcess transactionProcess
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
                NewTransactionRequestMarginCls Req = await _TransactionsQueue.DequeueAsync(_stopTokenSource.Token);

                if (!_stopTokenSource.IsCancellationRequested)
                {
                    using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                    {
                        Task.Run(() => _mediator.Send(Req));
                        Task.Delay(1000).Wait();
                    }
                }
            }

        }
    }

    //Rita 4-6-19 for arbitrage trading
    public class ProcessAllTransactionArbitrageDequeuer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITransactionQueue<NewTransactionRequestArbitrageCls> _TransactionsQueue;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private readonly IMediator _mediator;
        private Task _dequeueMessagesTask;

        public ProcessAllTransactionArbitrageDequeuer(IServiceProvider serviceProvider,
            ITransactionQueue<NewTransactionRequestArbitrageCls> TransactionsQueue, IMediator mediator)
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
                NewTransactionRequestArbitrageCls Req = await _TransactionsQueue.DequeueAsync(_stopTokenSource.Token);

                if (!_stopTokenSource.IsCancellationRequested)
                {
                    using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                    {
                        Task.Run(() => _mediator.Send(Req));
                        Task.Delay(1000).Wait();
                    }
                }
            }

        }
    }

    //=======================================================Withdraw===================================
    public class ProcessAllWithdrawDequeuer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITransactionQueue<NewWithdrawRequestCls> _TransactionsQueue;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private readonly IMediator _mediator;
        private Task _dequeueMessagesTask;

        public ProcessAllWithdrawDequeuer(IServiceProvider serviceProvider,
            ITransactionQueue<NewWithdrawRequestCls> TransactionsQueue, IMediator mediator)
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
                NewWithdrawRequestCls Req = await _TransactionsQueue.DequeueAsync(_stopTokenSource.Token);

                if (!_stopTokenSource.IsCancellationRequested)
                {
                    using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                    {
                        //ITransactionProcess _transactionProcess =
                        //                    serviceScope.ServiceProvider.GetRequiredService<ITransactionProcess>();
                        Task.Run(() => _mediator.Send(Req));
                        //await _transactionProcess.ProcessNewTransactionAsync(Req);
                        //Task<BizResponse> MethodRespTsk = _transactionProcess.ProcessNewTransactionAsync(Req);
                        //BizResponse MethodResp = await MethodRespTsk;
                    }

                }
            }

        }
    }

    //=======================================================Cancel Order===================================
    public class ProcessAllCancelOrderDequeuer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITransactionQueue<NewCancelOrderRequestCls> _TransactionQueueCancelOrder;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private readonly IMediator _mediator;
        private Task _dequeueMessagesTask;

        public ProcessAllCancelOrderDequeuer(IServiceProvider serviceProvider,
            ITransactionQueue<NewCancelOrderRequestCls> TransactionQueueCancelOrder, IMediator mediator)
        {
            _serviceProvider = serviceProvider;
            _TransactionQueueCancelOrder = TransactionQueueCancelOrder;
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
                NewCancelOrderRequestCls Req = await _TransactionQueueCancelOrder.DequeueAsync(_stopTokenSource.Token);

                if (!_stopTokenSource.IsCancellationRequested)
                {
                    using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                    {
                        //ITransactionProcess _transactionProcess =
                        //                    serviceScope.ServiceProvider.GetRequiredService<ITransactionProcess>();
                        Task.Run(() => _mediator.Send(Req));
                        //await _transactionProcess.ProcessNewTransactionAsync(Req);
                        //Task<BizResponse> MethodRespTsk = _transactionProcess.ProcessNewTransactionAsync(Req);
                        //BizResponse MethodResp = await MethodRespTsk;
                    }

                }
            }

        }
    }

    public class ProcessAllCancelOrderArbitrageDequeuer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITransactionQueue<NewCancelOrderArbitrageRequestCls> _TransactionQueueCancelOrder;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private readonly IMediator _mediator;
        private Task _dequeueMessagesTask;

        public ProcessAllCancelOrderArbitrageDequeuer(IServiceProvider serviceProvider,
            ITransactionQueue<NewCancelOrderArbitrageRequestCls> TransactionQueueCancelOrder, IMediator mediator)
        {
            _serviceProvider = serviceProvider;
            _TransactionQueueCancelOrder = TransactionQueueCancelOrder;
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
                NewCancelOrderArbitrageRequestCls Req = await _TransactionQueueCancelOrder.DequeueAsync(_stopTokenSource.Token);

                if (!_stopTokenSource.IsCancellationRequested)
                {
                    using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                    {
                        //ITransactionProcess _transactionProcess =
                        //                    serviceScope.ServiceProvider.GetRequiredService<ITransactionProcess>();
                        Task.Run(() => _mediator.Send(Req));
                        //await _transactionProcess.ProcessNewTransactionAsync(Req);
                        //Task<BizResponse> MethodRespTsk = _transactionProcess.ProcessNewTransactionAsync(Req);
                        //BizResponse MethodResp = await MethodRespTsk;
                    }

                }
            }

        }
    }
}
