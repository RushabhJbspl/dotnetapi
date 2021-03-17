using Castle.Core.Logging;
using CleanArchitecture.Core.Entities.Configuration.FeedConfiguration;
using CleanArchitecture.Core.Helpers;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Infrastructure.DTOClasses;
using Hangfire.Logging;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Web.BackgroundTask
{
    public class MarketCapService : IHostedService, IDisposable
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private Timer _timer;
        private Timer _timer1;
        private Timer _timer2;
        private Timer _timer3;
        private Timer _timer4;
        private Timer _timer5;
        private Timer _timer6;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IMediator _mediator;
        // private readonly IMarketCap _IMarketCap;

        public MarketCapService(ILogger<MarketCapService> logger, IMediator mediator, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
            ///_IMarketCap = IMarketCap;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {

                _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(5));

                _timer1 = new Timer(DoWorkProfit, null, TimeSpan.Zero,
                TimeSpan.FromHours(1));

                _timer2 = new Timer(DoWorkActiveAPIPlan, null, TimeSpan.Zero,
                TimeSpan.FromHours(1));

                _timer3 = new Timer(DoWorkRecurringCharge, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(15));

                _timer4 = new Timer(DoWorkReferralCommission, null, TimeSpan.Zero,
                TimeSpan.FromHours(1));

                _timer5 = new Timer(DoWorkStaking, null, TimeSpan.Zero,
                TimeSpan.FromHours(1));

                _timer6 = new Timer(DoWorkForceWithdrwLoan, null, TimeSpan.Zero,
             TimeSpan.FromMinutes(2));

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.CompletedTask;
            }
        }

        private void DoWork(object state)
        {
            try
            {
                HelperForLog.WriteLogIntoFile("MarketCapService", "DoWork", "CallAPIForMarketCap At : " + Helpers.UTC_To_IST());
                _mediator.Send(new MarketCapHandleTemp { strMarketCapHandleTemp = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, Microsoft.Extensions.Logging.LogLevel.Error);
            }
        }

        private void DoWorkProfit(object state)
        {
            try
            {
                var today = Helpers.UTC_To_IST();
                int Hours = Convert.ToInt32(_configuration["Hours"]);
                if (today.TimeOfDay.Hours == Hours)
                {
                    HelperForLog.WriteLogIntoFile("MarketCapService", "DoWorkProfit", "DoWorkProfit At : " + Helpers.UTC_To_IST());
                    _mediator.Send(new ProfitTemp { Date = today });
                }
                //DateTime reportAt = Helpers.UTC_To_IST().AddDays(1).Date;
                //var tonight = reportAt.AddHours(00).AddMinutes(00).AddSeconds(00);
                // _mediator.Send(new ProfitTemp { Date = date });
                //if (today == tonight)
                //{
                //    _mediator.Send(new ProfitTemp { Date = today });
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, Microsoft.Extensions.Logging.LogLevel.Error);
            }
        }

        private void DoWorkRecurringCharge(object state)
        {
            try
            {
                var today = Helpers.UTC_To_IST();
                var CurrentHour = today.AddMinutes(15).Hour;
                int RecurringChargeHour = Convert.ToInt32(_configuration["RecurringChargeHour"]);
                if (RecurringChargeHour == CurrentHour)
                {
                    HelperForLog.WriteLogIntoFile("MarketCapService", "DoWorkRecurringCharge", "DoWorkRecurringCharge At : " + Helpers.UTC_To_IST());
                    _mediator.Send(new RecurringChargeCalculation { Hour = RecurringChargeHour });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, Microsoft.Extensions.Logging.LogLevel.Error);
            }
        }

        private void DoWorkActiveAPIPlan(object state)
        {
            try
            {
                var today = Helpers.UTC_To_IST();
                int Hours = Convert.ToInt32(_configuration["Hours"]);
                if (today.TimeOfDay.Hours == Hours)
                {
                    _mediator.Send(new InvokeAPIPlanCroneObj { Date = today });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, Microsoft.Extensions.Logging.LogLevel.Error);
            }
        }

        private void DoWorkReferralCommission(object state)
        {
            try
            {
                int ReferralCommissionHour = Convert.ToInt32(_configuration["ReferralCommissionHour"]);
                if (ReferralCommissionHour > 0)
                {
                    HelperForLog.WriteLogIntoFile("MarketCapService", "DoWorkReferralCommission", "DoWorkReferralCommission At : " + Helpers.UTC_To_IST());
                    _mediator.Send(new RefferralCommissionTask { Hour = ReferralCommissionHour });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, Microsoft.Extensions.Logging.LogLevel.Error);
            }
        }

        private void DoWorkStaking(object state)
        {
            try
            {
                int UnstakingHour = Convert.ToInt32(_configuration["StakingHour"]);
                int Hour = Helpers.UTC_To_IST().Hour;

                if (UnstakingHour > 0)
                {
                    if (UnstakingHour == Hour)
                    {
                        _mediator.Send(new StakingReqRes { IsReqFromAdmin = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, Microsoft.Extensions.Logging.LogLevel.Error);
            }
        }

        private void DoWorkForceWithdrwLoan(object state)
        {
            try
            {
                _mediator.Send(new ForceWithdrwLoanv2Req { Hour = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, Microsoft.Extensions.Logging.LogLevel.Error);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {

                _timer?.Change(Timeout.Infinite, 0);
                _timer1?.Change(Timeout.Infinite, 0);
                _timer2?.Change(Timeout.Infinite, 0);
                _timer3?.Change(Timeout.Infinite, 0);
                _timer4?.Change(Timeout.Infinite, 0);
                _timer5?.Change(Timeout.Infinite, 0);
                _timer6?.Change(Timeout.Infinite, 0);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, Microsoft.Extensions.Logging.LogLevel.Error);
                return Task.CompletedTask;
            }
        }

        public void Dispose()
        {
            try
            {
                _timer?.Dispose();
                _timer1?.Dispose();
                _timer2?.Dispose();
                _timer3?.Dispose();
                _timer4?.Dispose();
                _timer5?.Dispose();
                _timer6?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, Microsoft.Extensions.Logging.LogLevel.Error);
            }
        }
    }
}
