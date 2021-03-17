using Worldex.Core.Enums;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.CCXT;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Infrastructure.LiquidityProvider;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Web.BackgroundTask
{
    public class TimeHostedLiqidityProviderService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;       
        private readonly IMediator _mediator;
        private readonly ISignalRService _signalRService;
        //public readonly ILiquidityProviderService _liquidityProviderService;
        public string[] Symbol;
        private readonly IConfiguration _configuration;

        public TimeHostedLiqidityProviderService(ILogger<TimeHostedLiqidityProviderService> logger, IMediator mediator, 
            ISignalRService signalRService , IConfiguration configuration
            ) //,ILiquidityProviderService liquidityProviderService)
        {
            _logger = logger;
            _mediator = mediator;
            _signalRService = signalRService;
            _configuration = configuration;
            //_liquidityProviderService = liquidityProviderService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                dynamic a = 1000;
                dynamic b = 10;
                //Symbol = _liquidityProviderService.GetPair();
                string liquidityProvider = _configuration["liquidityProviderOrderBookOnOff"] == null ? "false" : _configuration["liquidityProviderOrderBookOnOff"];
                if(liquidityProvider.ToLower() == "true")
                {
                    //_timerPoloniex = new Timer(SendPoloniex, null, TimeSpan.Zero,
                    //TimeSpan.FromSeconds(5));
                    //_timerTardeSatoshi = new Timer(SendTardeSatoshi, null, TimeSpan.Zero,
                    //    TimeSpan.FromSeconds(5));
                    _timer = new Timer(SendOrderbook, null, TimeSpan.Zero,
                        TimeSpan.FromSeconds(5));
                    //_timerBittrex = new Timer(SendBittrex, null, TimeSpan.Zero,
                    //    TimeSpan.FromSeconds(5));
                    //_timerCoinbase = new Timer(SendCoinBase, null, TimeSpan.Zero,
                    //    TimeSpan.FromSeconds(5));
                    ///////add new timer start for OKEx by Pushpraj as on 11-06-2019
                    //_timerOKEx = new Timer(SendOKEx, null, TimeSpan.Zero,
                    //    TimeSpan.FromSeconds(5));
                    //_timerUpbit = new Timer(SendUpbit, null, TimeSpan.Zero,
                    //    TimeSpan.FromSeconds(5));
                    //_timerHuobi = new Timer(SendHuobi, null, TimeSpan.Zero,
                    //  TimeSpan.FromSeconds(5));
                    //_timerBitfinex = new Timer(SendBitfinex, null, TimeSpan.Zero,
                    //    TimeSpan.FromSeconds(5));
                    //_timerGemini = new Timer(SendGemini, null, TimeSpan.Zero,
                    //    TimeSpan.FromSeconds(5));
                    ////Add new timer for Yobit exchange by Pushpraj as on 12 - 07 - 2019
                    //_timerYobit = new Timer(SendYobit, null, TimeSpan.Zero,
                    //    TimeSpan.FromSeconds(5));
                    //_timerEXMO = new Timer(SendEXMO, null, TimeSpan.Zero,
                    //    TimeSpan.FromSeconds(5));
                    //_timerGemini = new Timer(SendGemini, null, TimeSpan.Zero,
                    //    TimeSpan.FromSeconds(5));
                    //_timerCEXIO = new Timer(sendCEXIO, null, TimeSpan.Zero,
                    //   TimeSpan.FromSeconds(5));
                }                
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.CompletedTask;
            }
        }
        
        private void SendOrderbook(object state)
        {
            try
            {
                SendBinance();
                SendTardeSatoshi();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        private void sendCEXIO()
        {
            try
            {
                //_liquidityProviderService.SendBinanceOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.CEXIO
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        private void SendHuobi()
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
        private void SendGemini()
        {
            try
            {
                //_liquidityProviderService.SendBinanceOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.Gemini
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        //Add new send state for Yobit exchange by Pushpraj as on 12-07-2019
        private void SendYobit()
        {
            try
            {
                //_liquidityProviderService.SendBinanceOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.Yobit
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        private void SendEXMO()
        {
            try
            {                
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.EXMO
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        private void SendPoloniex()
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
        private void SendTardeSatoshi()
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
        private void SendOKEx()
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
        private void SendKraken()
        {
            try
            {
                //_liquidityProviderService.SendTradesatoshiOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.Kraken
                };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        private void SendBinance()
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
        private void SendBittrex()
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
        private void SendCoinBase()
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
        private void SendUpbit()
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
        private void SendBitfinex()
        {
            try
            {
                //_liquidityProviderService.SendCoinBaseOrderBookAsync(Symbol);
                CommonOrderBookRequest Req = new CommonOrderBookRequest()
                {
                    LpType = enAppType.Bitfinex
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

    public class TimeHostedCryptoWatcher : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private Timer _timer1;
        private Timer _timer2;
        private Timer _timer3;
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
            int CCXTTickerSecond = Convert.ToInt32(_configuration["CCXTTickerSecond"]);
            int CCXTStatusCheckSecond = 10;

            try
            {
                _timer = new Timer(CryptoWatcher, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(Second));

                //komal 10-06-2019 for Arbitrage Trading
                //komal stop this method
                //_timer1 = new Timer(CryptoWatcherArbitrage, null, TimeSpan.Zero,
                //    TimeSpan.FromSeconds(Second));

                //_timer2 = new Timer(CCXTTicker, null, TimeSpan.Zero,
                //    TimeSpan.FromSeconds(CCXTTickerSecond));

                //if (_configuration["CCXTStatusCheckSecond"] != null)
                //    CCXTStatusCheckSecond = Convert.ToInt32(_configuration["CCXTStatusCheckSecond"]);

                //_timer3 = new Timer(CCXTStatusCheck, null, TimeSpan.Zero,
                //    TimeSpan.FromSeconds(CCXTStatusCheckSecond));
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
        private void CCXTTicker(object state)
        {
            try
            {
                CCXTTickerHandlerRequest Req = new CCXTTickerHandlerRequest() { };
                _mediator.Send(Req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }
        private void CCXTStatusCheck(object state)
        {
            try
            {
                CCXTStatusCheckHandlerRequest Req = new CCXTStatusCheckHandlerRequest() { };
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

