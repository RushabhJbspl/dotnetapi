using Bittrex.Net;
using Bittrex.Net.Objects;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coinbase;
using CryptoExchange.Net.Objects;
using Worldex.Core.Helpers;
using Newtonsoft.Json;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.Interfaces;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Enums;    
using Worldex.Infrastructure.BGTask;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.ViewModels.LiquidityProvider;
using MediatR;

namespace Worldex.Infrastructure.LiquidityProvider
{   

    public class BitrexLPService : IBitrexLPService
    {
        public BittrexClient _client = new BittrexClient();
        public BittrexClientV3 _Clientv3 = new BittrexClientV3();
        public BitrexLPService(BittrexClient Client, BittrexClientV3 ClientV3)
        {
            _client = Client;
            _Clientv3 = ClientV3;
        }

        public async Task<WebCallResult<object>> CancelOrderAsync(Guid guid)
        {
            //string resp = "{\"success\":true,\"message\":\"''\",\"result\":{\"uuid\":\"614c34e4-8d71-11e3-94b5-425861b86ab6\"}}";            
            WebCallResult<object> Result =await _client.CancelOrderAsync(guid);
            return Result;
        }

        public Task GetBalancesAsync()
        {
            var Result = _client.GetBalancesAsync();
            return Task.CompletedTask;
        }

        public async Task<CallResult<BittrexBalance>> GetBalanceAsync(string currency)
        {
            CallResult<BittrexBalance> Result = await _client.GetBalanceAsync(currency);
            return Result;
        }

        public Task GetMarketSummaryAsync(string market)
        {
            var Result = _client.GetMarketSummaryAsync(market);
            return Task.CompletedTask;
        }

        public Task GetOpenOrdersAsync(string market = null)
        {
            var Result = _client.GetOpenOrdersAsync(market);
            return Task.CompletedTask;
        }

        public Task<WebCallResult<BittrexOrderBook>> GetOrderBookAsync(string market)
        {
            var Result = _client.GetOrderBookAsync(market);
            return Result;
        }

        public Task GetOrderHistoryAsync(string market = null)
        {
            var Result = _client.GetOrderHistoryAsync(market);
            return Task.CompletedTask;
        }

        public Task<CallResult<BittrexAccountOrder>> GetOrderInfoAsync(Guid guid) // order status
        {
            //string Resp = "{  \"success\": true,  \"message\": \"''\",  \"result\": [    {      \"Uuid\": \"string (uuid)\",      \"OrderUuid\": \"8925d746-bc9f-4684-b1aa-e507467aaa99\",      \"Exchange\": \"BTC-LTC\",      \"OrderType\": \"string\",      \"Quantity\": 100000,      \"QuantityRemaining\": 100000,      \"Limit\": 1e-8,      \"CommissionPaid\": 0,      \"Price\": 0,      \"PricePerUnit\": null,      \"Opened\": \"2014-07-09T03:55:48.583\",      \"Closed\": null,      \"CancelInitiated\": \"boolean\",      \"ImmediateOrCancel\": \"boolean\",      \"IsConditional\": \"boolean\"    }  ]}";
            CallResult<BittrexAccountOrder> Result = _client.GetOrder(guid);
            return Task.FromResult(Result);
        }

        public Task<CallResult<BittrexMarketHistory[]>> GetTradeHistoryAsync(string market)
        {
            CallResult<BittrexMarketHistory[]> Result = _client.GetMarketHistory(market);
            return Task.FromResult(Result);
        }

        public Task<CallResult<BittrexGuid>> PlaceOrderAsync(Bittrex.Net.Objects.OrderSide side, string market, decimal quantity, decimal rate)
        {
            CallResult<BittrexGuid> Result = _client.PlaceOrder(side, market, quantity,rate);            
            return Task.FromResult(Result);
        }

        public Task<WebCallResult<Bittrex.Net.Objects.V3.BittrexOrderV3>> PlaceConditionalOrder(Bittrex.Net.Objects.OrderSide side, string market, decimal quantity, decimal rate)
        {
            //CallResult<BittrexOrderResult> Result = _client.PlaceConditionalOrder(side, TimeInEffect.GoodTillCancelled, market, quantity, rate, ConditionType.None, 0);
            WebCallResult<Bittrex.Net.Objects.V3.BittrexOrderV3> Result = _Clientv3.PlaceOrder(market,side,OrderTypeV3.Limit,quantity: quantity, timeInForce: TimeInForce.GoodTillCancelled,limit: rate);
            return Task.FromResult(Result);
        }

        public Task GetBuyOrderBook(string market)
        {
            var Result = _client.GetBuyOrderBook(market);
            return Task.CompletedTask;
        }

        public Task GetSellOrderBook(string market)
        {
            var Result = _client.GetSellOrderBook(market);
            return Task.CompletedTask;
        }
    }

    public interface IBitrexScopedProcessingService
    {
        Task DoWork();
    }

    public class BitrexScopedProcessingService : IBitrexScopedProcessingService
    {
        private readonly ILogger _logger;
        private readonly ISignalRService _SignalRService;
        private List<GetBuySellBook> OrderBookList;//= new List<GetBuySellBook>();
        private readonly ITransactionConfigService _transactionConfigService;
        private readonly IMediator _mediator;

        public BitrexScopedProcessingService(ILogger<BitrexScopedProcessingService> logger, IMediator mediator,
            ISignalRService SignalRService, ITransactionConfigService TransactionConfigService)
        {
            _logger = logger;
            _SignalRService = SignalRService;
            _transactionConfigService = TransactionConfigService;
            _mediator = mediator;
        }

        public async Task DoWork()
        {
            try
            {
            CallResult<BittrexMarket[]> markets;
            using (var client = new BittrexClient())
            {
                // public
                markets = await client.GetMarketsAsync();                
            }
            
            string[] Symbol = new string[markets.Data.Length];
            Symbol = Array.ConvertAll<BittrexMarket, string>(markets.Data, x => (string)x.MarketName.Replace("-","_"));
            string[] symbol = _transactionConfigService.LpPairListConvertorV1(Symbol, Convert.ToInt16(enAppType.Bittrex));
            //string data = string.Join("\",\"", symbol);

            // Websocket
            //var socketClient = new BittrexSocketClient();
 
            //foreach(var x in symbol)
            //{
            //    var subscription2 = await socketClient.SubscribeToExchangeStateUpdatesAsync(x, state =>
            //    {
            //        try
            //        {
            //            if (state != null)
            //            {
            //                var BittrexOrderBook = new BittrexBuySellBook()
            //                {
            //                    Symbol = state.MarketName,
            //                    Asks = state.Sells,
            //                    Bids = state.Buys
            //                };
            //                _mediator.Send(BittrexOrderBook);
            //                if (state.Fills != null)
            //                {
            //                    var BittrexStream = new BittrexTradeHistory();
            //                    BittrexStream.Fills = state.Fills;
            //                    BittrexStream.Pair = state.MarketName;
            //                    _mediator.Send(BittrexStream);
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            //        }                    
            //    });
            //}

            _logger.LogInformation("Bitrex Scoped Processing Service is working.");
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }
    }
    public class BitrexConsumeScopedServiceHostedService : IHostedService
    {
        private readonly ILogger _logger;

        public BitrexConsumeScopedServiceHostedService(IServiceProvider services,
            ILogger<BitrexConsumeScopedServiceHostedService> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Bitrex Consume Scoped Service Hosted Service is starting.");

            DoWork();

            return Task.CompletedTask;
        }

        private void DoWork()
        {
            _logger.LogInformation(
                "Bitrex Consume Scoped Service Hosted Service is working.");

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<IBitrexScopedProcessingService>();

                scopedProcessingService.DoWork();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Bitrex Consume Scoped Service Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
