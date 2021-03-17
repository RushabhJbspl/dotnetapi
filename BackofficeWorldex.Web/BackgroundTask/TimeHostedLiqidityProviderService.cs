using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.ViewModels.LiquidityProvider;
using CleanArchitecture.Infrastructure.LiquidityProvider;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Web.BackgroundTask
{
    public class TimeHostedLiqidityProviderService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timerPoloniex;
        private Timer _timerTardeSatoshi;
        private Timer _timerCoinbase;
        private Timer _timerBinance;
        private Timer _timerHuobi;  
        private Timer _timerBittrex;
        private Timer _timerUpbit;
        private Timer _timerOKEx; //Add new timer for OKEx by Pushpraj as on 11-06-2019
        private readonly IMediator _mediator;
        private readonly ISignalRService _signalRService;
        //public readonly ILiquidityProviderService _liquidityProviderService;
        public string[] Symbol;

        public TimeHostedLiqidityProviderService(ILogger<TimeHostedLiqidityProviderService> logger, IMediator mediator, ISignalRService signalRService
            ) //,ILiquidityProviderService liquidityProviderService)
        {
            _logger = logger;
            _mediator = mediator;
            _signalRService = signalRService;
            //_liquidityProviderService = liquidityProviderService;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                dynamic a = 1000;
                dynamic b = 10;
                //Symbol = _liquidityProviderService.GetPair();
                _timerPoloniex = new Timer(SendPoloniex, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(5));
                _timerTardeSatoshi = new Timer(SendTardeSatoshi, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(5));
                _timerBinance = new Timer(SendBinance, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(5));
                _timerBittrex = new Timer(SendBittrex, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(5));
                _timerCoinbase = new Timer(SendCoinBase, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(5));
                /////add new timer start for OKEx by Pushpraj as on 11-06-2019
                _timerOKEx = new Timer(SendOKEx, null, TimeSpan.Zero, 
                    TimeSpan.FromSeconds(5));
                _timerUpbit = new Timer(SendUpbit, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(5));
                _timerHuobi = new Timer(SendHuobi, null, TimeSpan.Zero,
                  TimeSpan.FromSeconds(5));
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.CompletedTask;
            }
        }

        private void SendHuobi(object state)
        {
            try
            {
                //_liquidityProviderService.SendBinanceOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.Huobi
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        private void SendPoloniex(object state)
        {
            try
            {

                //_liquidityProviderService.SendPoloniexOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.Poloniex
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        private void SendTardeSatoshi(object state)
        {
            try
            {
                //_liquidityProviderService.SendTradesatoshiOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.TradeSatoshi
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        #region
        /// <summary>
        /// Add new Call for OKEx API by Pushpraj as on 11-06-2019
        /// </summary>
        /// <param name="state"></param>
        private void SendOKEx(object state)
        {
            try
            {
                //_liquidityProviderService.SendTradesatoshiOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.OKEx
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        #endregion


        private void SendBinance(object state)
        {
            try
            {
                //_liquidityProviderService.SendBinanceOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.Binance
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        private void SendBittrex(object state)
        {
            try
            {
                //_liquidityProviderService.SendBittrexOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.Bittrex
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        private void SendCoinBase(object state)
        {
            try
            {
                //_liquidityProviderService.SendCoinBaseOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.Coinbase
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        private void SendUpbit(object state)
        {
            try
            {
                //_liquidityProviderService.SendCoinBaseOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.UpBit
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Timed Background Service is stopping.");

                _timerTardeSatoshi?.Change(Timeout.Infinite, 0);
                _timerPoloniex?.Change(Timeout.Infinite, 0);
                _timerBinance?.Change(Timeout.Infinite, 0);
                _timerBittrex?.Change(Timeout.Infinite, 0);
                _timerCoinbase?.Change(Timeout.Infinite, 0);
                _timerOKEx?.Change(Timeout.Infinite, 0); //add new timer stop for OKEx by Pushpraj as on 11-06-2019
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.CompletedTask;
            }

        }

        public void Dispose()
        {
            try
            {
                _timerTardeSatoshi?.Dispose();
                _timerPoloniex?.Dispose();
                _timerBinance?.Dispose();
                _timerBittrex?.Dispose();
                _timerCoinbase?.Dispose();
                _timerOKEx?.Dispose();   //add new timer disposed for OKEx by Pushpraj as on 11-06-2019
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
    }


    public class TimeHostedCryptoWatcher : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private Timer _timer1;
        private readonly IMediator _mediator; 
        public string[] Symbol;
        private readonly IConfiguration _configuration;

        public TimeHostedCryptoWatcher(ILogger<TimeHostedLiqidityProviderService> logger, IMediator mediator, IConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            int Second = Convert.ToInt32(_configuration["CryptoWatcherSecond"]);
            try
            {
                _timer = new Timer(CryptoWatcher, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(Second));

                //komal 10-06-2019 for Arbitrage Trading
                _timer1 = new Timer(CryptoWatcherArbitrage, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(Second));
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.CompletedTask;
            }
        }

        private void CryptoWatcher(object state)
        {
            try
            {
                CryptoWatcherReq Req = new CryptoWatcherReq()
                {
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        private void CryptoWatcherArbitrage(object state)
        {
            try
            {
                CryptoWatcherArbitrageReq Req = new CryptoWatcherArbitrageReq()
                {
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        //private void SendCoinBase(object state)
        //{
        //    try
        //    {
        //        //_liquidityProviderService.SendCoinBaseOrderBookAsync(Symbol);
        //        CommonOrderBookRequest Req = new CommonOrderBookRequest()
        //        {
        //            LpType = enAppType.Coinbase
        //        };
        //        _mediator.Send(Req);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //    }
        //}

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Timed Background Service is stopping.");

                _timer?.Change(Timeout.Infinite, 0);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.CompletedTask;
            }

        }

        public void Dispose()
        {
            try
            {
                _timer?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
    }
}

