using Castle.Core.Logging;
using CleanArchitecture.Core.ApiModels;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Web.BackgroundTask
{
    internal class TimedHostedLPStatusCheckArbitrage : IHostedService, IDisposable
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private Timer _timer;
        private readonly IMediator _mediator;

        public TimedHostedLPStatusCheckArbitrage(ILogger<TimedHostedService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                _mediator.Send(new LPStatusCheckClsArbitrage { uuid = Guid.NewGuid() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service For PaiCalculation is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
