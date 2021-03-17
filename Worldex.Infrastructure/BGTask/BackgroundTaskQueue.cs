using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.SharedKernel;
using Worldex.Core.ViewModels;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.BGTask
{   

    public interface IPushNotificationsQueue<T>
    {
        void Enqueue(T message);

        Task<T> DequeueAsync(CancellationToken cancellationToken);
    }

    public class PushNotificationsQueue<T> : IPushNotificationsQueue<T>
    {
        private readonly ConcurrentQueue<T> _messages = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim _messageEnqueuedSignal = new SemaphoreSlim(0);

        public void Enqueue(T message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _messages.Enqueue(message);

            _messageEnqueuedSignal.Release();
        }

        public async Task<T> DequeueAsync(CancellationToken cancellationToken)
        {
            await _messageEnqueuedSignal.WaitAsync(cancellationToken);

            _messages.TryDequeue(out T message);

            return message;
        }
    }

    public class PushEmailDequeuer<T> : IHostedService where T : SendEmailRequest
        //public class PushEmailDequeuer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPushNotificationsQueue<T> _messagesQueue;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private readonly IMediator _mediator;
        private Task _dequeueMessagesTask;
        private readonly ILogger<PushEmailDequeuer<T>> _logger;

        public PushEmailDequeuer(ILogger<PushEmailDequeuer<T>> logger, IServiceProvider serviceProvider,
            IPushNotificationsQueue<T> messagesQueue, IMediator mediator)
        {
            _serviceProvider = serviceProvider;
            _messagesQueue = messagesQueue;
            _logger = logger;
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dequeueMessagesTask = Task.Run(DequeueMessagesAsync);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stopTokenSource.Cancel();

            return Task.WhenAny(_dequeueMessagesTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task DequeueMessagesAsync()
        {
            try
            {
                while (!_stopTokenSource.IsCancellationRequested)
                {
                    T Email = await _messagesQueue.DequeueAsync(_stopTokenSource.Token);

                    if (!_stopTokenSource.IsCancellationRequested)
                    {
                        using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                        {
                            Task.Run(()  => {
                                 _mediator.Send(Email);
                            });
                            HelperForLog.WriteLogIntoFile("DequeueMessagesAsync", "0 SendEmail", " -Data- ");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
    }

    
    public class PushSMSDequeuer<T> : IHostedService where T : SendSMSRequest
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPushNotificationsQueue<T> _messagesQueue;
        //private readonly IPushNotificationService _notificationService;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private Task _dequeueSMSTask;
        private readonly IMediator _mediator;
        private readonly ILogger<PushSMSDequeuer<T>> _logger;

        public PushSMSDequeuer(ILogger<PushSMSDequeuer<T>> logger,IServiceProvider serviceProvider,
            IPushNotificationsQueue<T> messagesQueue, IMediator Mediator)
        {
            _serviceProvider = serviceProvider;
            _messagesQueue = messagesQueue;
            _logger = logger;
            _mediator = Mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dequeueSMSTask = Task.Run(DequeueMessagesAsync);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stopTokenSource.Cancel();

            return Task.WhenAny(_dequeueSMSTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task DequeueMessagesAsync()
        {
            try
            {
                while (!_stopTokenSource.IsCancellationRequested)
                {
                    T Message = await _messagesQueue.DequeueAsync(_stopTokenSource.Token);

                    if (!_stopTokenSource.IsCancellationRequested)
                    {
                        using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                        {
                            Task.Run(() => {
                                _mediator.Send(Message);
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
    }

    public class PushNotificationDequeuer<T> : IHostedService where T : SendNotificationRequest
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPushNotificationsQueue<T> _messagesQueue;
        //private readonly IPushNotificationService _notificationService;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private Task _dequeueSMSTask;
        private readonly IMediator _mediator;
        private readonly ILogger<PushNotificationDequeuer<T>> _logger;

        public PushNotificationDequeuer(ILogger<PushNotificationDequeuer<T>> logger, IServiceProvider serviceProvider,
            IPushNotificationsQueue<T> messagesQueue, IMediator Mediator)
        {
            _serviceProvider = serviceProvider;
            _messagesQueue = messagesQueue;
            _logger = logger;
            _mediator = Mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dequeueSMSTask = Task.Run(DequeueMessagesAsync);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stopTokenSource.Cancel();

            return Task.WhenAny(_dequeueSMSTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task DequeueMessagesAsync()
        {
            try
            {
                while (!_stopTokenSource.IsCancellationRequested)
                {
                    T Message = await _messagesQueue.DequeueAsync(_stopTokenSource.Token);

                    if (!_stopTokenSource.IsCancellationRequested)
                    {
                        using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                        {
                            Task.Run(() => {
                                _mediator.Send(Message);
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
    }
}
