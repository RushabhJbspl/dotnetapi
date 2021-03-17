using Worldex.Core.ApiModels;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Web.BackgroundTask
{
    internal class TimedHostedLPStatusCheckArbitrage : IHostedService, IDisposable
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private Timer _timer;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public TimedHostedLPStatusCheckArbitrage(ILogger<TimedHostedService> logger, IMediator mediator, IConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {            
            try
            {
                int Second = Convert.ToInt32(_configuration["LPStatusCheckArbitrage"]);
                _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(Second));
                //TimeSpan.FromMinutes(15));
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + "TimedHostedLPStatusCheckArbitrage" + "\nClassname=" + "StartAsync", LogLevel.Error);
                return Task.CompletedTask;
            }
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
