using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net;

using Binance.Net.Objects;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.Transaction;
using CryptoExchange.Net.Objects;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StructureMap;

namespace Worldex.Infrastructure.LiquidityProvider
{
    public class BinanceLPService : IBinanceLPService
    {
        public BinanceClient _client;
        public BinanceLPService(BinanceClient Client)
        {
            _client = Client;
        }

        public Task<CallResult<BinanceCanceledOrder>> CancelOrderAsync(string symbol, long? orderId = null, string origClientOrderId = null, string newClientOrderId = null, long? receiveWindow = null)
        {
            CallResult<BinanceCanceledOrder> Result = _client.CancelOrder(symbol, orderId, origClientOrderId, newClientOrderId, receiveWindow);
            return Task.FromResult(Result);
        }

        public Task<WebCallResult<BinanceAccountInfo>> GetBalancesAsync()
        {
            var Result = _client.GetAccountInfo();
            return Task.FromResult(Result);
        }

        public Task GetMarketSummaryAsync(string market)
        {
            var Result = _client.Get24HPriceAsync(market);
            return Task.CompletedTask;
        }

        public Task GetOpenOrdersAsync(string market = null, int? receiveWindow = null)
        {
            var Result = _client.GetOpenOrders(market);
            return Task.CompletedTask;
        }

        public Task<CallResult<BinanceOrderBook>> GetOrderBookAsync(string market, int? limit = 100)
        {
            CallResult<BinanceOrderBook> Result = _client.GetOrderBook(market, limit);
            return Task.FromResult(Result);
        }

        public Task GetOrderHistoryAsync(string market = null)
        {
            var Result = _client.GetMyTradesAsync(market);
            return Task.CompletedTask;
        }

        public Task<WebCallResult<BinanceOrder>> GetOrderInfoAsync(string symbol, long? orderId = null, string origClientOrderId = null, long? receiveWindow = null) // order status
        {
            var Result = _client.QueryOrder(symbol, orderId);
            return Task.FromResult(Result);
        }

        public Task GetSystemStatusAsync()
        {
            var Result = _client.GetSystemStatusAsync();
            return Task.CompletedTask;
        }

        public Task GetTradeFeeAsync(string market = null, int? receiveWindow = null)
        {
            var Result = _client.GetTradeFeeAsync(market);
            return Task.CompletedTask;
        }

        public Task<WebCallResult<BinanceAggregatedTrades[]>> GetTradeHistoryAsync(string market, int? limit = null)
        {
            var Result = _client.GetAggregatedTrades(market, limit: limit);
            return Task.FromResult(Result);
        }

        public Task<CallResult<BinancePlacedOrder>> PlaceOrderAsync(Binance.Net.Objects.OrderSide side, string market, OrderType type, decimal quantity, string newClientOrderId = null, decimal? price = null, TimeInForce? timeInForce = null, decimal? stopPrice = null, decimal? icebergQty = null, OrderResponseType? orderResponseType = null, int? receiveWindow = null)
        {
            //Rushabh 19-07-2019 added stop price parameter
            //CallResult<BinancePlacedOrder> Result = _client.PlaceOrder(market, side, type, quantity,price : price,timeInForce: timeInForce,stopPrice: stopPrice);
            CallResult<BinancePlacedOrder> Result = _client.PlaceOrder(market, side, type, quantity, price: price, timeInForce: timeInForce);
            return Task.FromResult(Result);
        }

        public Task<WebCallResult<BinanceExchangeInfo>> GetExchangeInfoAsync()
        {
            var Result = _client.GetExchangeInfoAsync();
            return Result;
        }

        public CallResult<BinanceTradeFee[]> GetTradeFee(string symbol)
        {
            var Result = _client.GetTradeFee(symbol);
            return Result;
        }



    }

    public interface IBinanceScopedProcessingService
    {
        void DoWork();
    }

    public class BinanceScopedProcessingService : IBinanceScopedProcessingService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        List<GetBuySellBook> OrderBookList;// = new List<GetBuySellBook>();
        private readonly ITransactionConfigService _transactionConfigService;
        private IMemoryCache _cache;
        //private readonly IMediator _mediator;
        private readonly ILiquidityProviderService _liquidityProviderService;

        public BinanceScopedProcessingService(IMemoryCache Cache, ILogger<BinanceScopedProcessingService> logger, // IMediator mediator , ILiquidityProviderService LiquidityProviderService,
           IServiceProvider serviceProvider, ITransactionConfigService TransactionConfigService)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            //_mediator = mediator;
            _transactionConfigService = TransactionConfigService;
            _cache = Cache;
            //_liquidityProviderService = LiquidityProviderService;
        }

        public void DoWork()
        {
            try
            {

                //CallResult<BinanceExchangeInfo> exchangeInfo;

                //using (var client = new BinanceClient())
                //{
                //    exchangeInfo = client.GetExchangeInfo();
                //}

                //string[] Symbol = new string[exchangeInfo.Data.Symbols.Length];

                //Symbol = Array.ConvertAll<BinanceSymbol, string>(exchangeInfo.Data.Symbols, x => (string)x.Name);
                //_cache.Set<string[]>("BinanceSymbol", Symbol);
                string[] Symbol = new string[] { "bnbbtc" }; ;
                string[] symbol = _transactionConfigService.LpPairListConvertorV1(Symbol, Convert.ToInt16(enAppType.Binance));
                string[] baseCur = new string[] { "BTC", "ETH", "USDT", "BNB", "USD", "PAX" };
                _cache.Set<string[]>("BinanceTradePair", symbol);

                var socketClient = new BinanceSocketClient();
                // Streams
                //var successDepth = socketClient.SubscribeToDepthStreamAsync(_cache.Get<string[]>("BinanceTradePair"), (data) =>
                ////var successDepth = socketClient.SubscribeToDepthStreamAsync(Symbol, async (data) => // for testing 
                //{
                //    try
                //    {
                //        if(data != null)
                //        {
                //            BinanceBuySellBook BinanceOrderBook = new BinanceBuySellBook()
                //            {
                //                Symbol = data.Symbol,
                //                Asks = data.Asks,
                //                Bids = data.Bids
                //            };
                //            //var container = new Container(_ => { _.For<IMediator>().Use<Mediator>(); });

                //            //using (var nested = container.GetNestedContainer())
                //            //{
                //            //    // This object is disposed when the nested container
                //            //    // is disposed
                //            //    var worker = nested.GetInstance<IMediator>();
                //            //    worker.Send(BinanceOrderBook);
                //            //}
                //            //await _liquidityProviderService.SocketBinanceOrderBookAsync(BinanceOrderBook);
                //            await _mediator.Send(BinanceOrderBook);
                //            //using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                //            //{

                //            //HelperForLog.WriteLogIntoFile("DequeueMessagesAsync", "0 SendEmail", " -Data- ");
                //            //}

                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //    }
                //});

                var successAggregatedTrade = socketClient.SubscribeToAggregatedTradesStreamAsync(_cache.Get<string[]>("BinanceTradePair"), (data) =>
                {
                    try
                    {
                        if (data != null)
                        {
                            var BinanceTradeHistory = new BinanceTradeHistory();
                            CopyClass.CopyObject(data, ref BinanceTradeHistory);
                            HelperForLog.WriteLogIntoFile("BinanceTradeHistory", "", " -Data- " + Helpers.JsonSerialize(BinanceTradeHistory));
                            //_mediator.Send(BinanceTradeHistory);
                        }
                    }
                    catch (Exception ex)
                    {
                        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                    }
                });
                _logger.LogInformation("Binance Scoped Processing Service is working.");
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }
    }

    public class BinanceConsumeScopedServiceHostedService : IHostedService
    {
        private readonly ILogger _logger;

        public BinanceConsumeScopedServiceHostedService(IServiceProvider services,
            ILogger<BinanceConsumeScopedServiceHostedService> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Binance Consume Scoped Service Hosted Service is starting.");

            DoWork();

            return Task.CompletedTask;
        }

        private void DoWork()
        {
            _logger.LogInformation(
                "Binance Consume Scoped Service Hosted Service is working.");

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<IBinanceScopedProcessingService>();

                scopedProcessingService.DoWork();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Binance Consume Scoped Service Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
