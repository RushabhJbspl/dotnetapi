using Binance.Net.Objects;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Infrastructure.BGTask;
using Coinbase;
using Coinbase.Pro;
using Coinbase.Pro.Models;
using Coinbase.Pro.WebSockets;
using CoinbasePro.Network.Authentication;
using CoinbasePro.Services.Accounts;
using CoinbasePro.Services.Currencies.Models;
using CoinbasePro.Services.Fills.Models.Responses;
using CoinbasePro.Services.Orders.Models.Responses;
using CoinbasePro.Services.Orders.Types;
using CoinbasePro.Services.Products.Models;
using CoinbasePro.Services.Products.Models.Responses;
using CoinbasePro.Services.Products.Types;
using CoinbasePro.Shared.Types;
using CoinbasePro.WebSocket;
using CoinbasePro.WebSocket.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider;
using System.Net.Http;
using Flurl;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Worldex.Core.Interfaces.Configuration;
using System.Linq;

namespace Worldex.Infrastructure.LiquidityProvider
{
    public class CoinBaseGlobalSettings
    {
        public static string API_Key;
        public static string Secret;
    }

    public class CoinBaseService : ICoinBaseService
    {
        private readonly ILogger _logger;
        CoinbasePro.CoinbaseProClient coinbaseProClient;
        Authenticator authenticator;
        //string APIKey = "c2fb5263c0d0e8ee6a01aba1283930b9";
        //string SeckretKey = "ROk/hPHZRRoNS5+NRODs44IElPS7D9AMF8LIuuJXJLtdEHDQp2CC5eFpc5uMDCXU6zkHriebdT/vL6JClnymhw==";
        string APIKey = CoinBaseGlobalSettings.API_Key;
        string SeckretKey = CoinBaseGlobalSettings.Secret;
        string Passphrase = "jtpf94wlwgo";

        public CoinBaseService(ILogger<CoinBaseService> logger)
        {
            _logger = logger;
            authenticator = new Authenticator(APIKey, SeckretKey, Passphrase);
            coinbaseProClient = new CoinbasePro.CoinbaseProClient(authenticator, true);
        }

        public void Connect()
        {
            try
            {
                authenticator = new Authenticator(APIKey, SeckretKey, Passphrase);
                coinbaseProClient = new CoinbasePro.CoinbaseProClient(authenticator, true);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

        public async Task<object> GetAllOrders(CoinbasePro.Services.Orders.Types.OrderStatus[] orderStatus, int limit = 100, int numberOfPages = 0)
        {
            try
            {
                var orderData = await coinbaseProClient.OrdersService.GetAllOrdersAsync(orderStatus, limit, numberOfPages);
                return orderData;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return ex.Message;
            }
        }

        public async Task<OrderResponse> GetOrderById(string id)
        {
            try
            {
                OrderResponse orderResponse = new OrderResponse();
                var orderData = coinbaseProClient.OrdersService.GetOrderByIdAsync(id);
                return await Task.FromResult(orderResponse);
            }
            catch (Exception ex)
            {
                OrderResponse orderResponse = new OrderResponse();
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                orderResponse.Status = CoinbasePro.Services.Orders.Types.OrderStatus.Rejected;
                return await Task.FromResult(orderResponse);
            }
        }

        public async Task<object> GetProductOrderBook(string Pair)
        {
            //try
            //{
            //    //var value = (CoinbasePro.Shared.Types.ProductType)Enum.Parse(typeof(CoinbasePro.Shared.Types.ProductType), Pair, true);
            //    ////var value = (CoinbasePro.Shared.Types.ProductType)Enum.Parse(typeof(CoinbasePro.Shared.Types.ProductType), Pair.Replace("-", ""), true);
            //    var orderBook = await coinbaseProClient.ProductsService.GetProductOrderBookAsync(value, CoinbasePro.Services.Products.Types.ProductLevel.Three);

            //    return orderBook;
            //}
            //catch (Exception ex)
            //{
            //    //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            //    return ex.Message;
            //}
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string ProductsEndpoint = CoinbasePro.Shared.ApiUris.ApiUri.AppendPathSegment("products").AppendPathSegments(Pair, "book").SetQueryParam("level", 1);
                    //string Url = "https://api.pro.coinbase.com/products/#Pair#/book?level=2";
                    //Url = Url.Replace("#Pair#", Pair);
                    client.DefaultRequestHeaders.Add("User-Agent", "CoinBase");
                    var Result = await client.GetAsync(ProductsEndpoint).Result.Content.ReadAsStringAsync();
                    if (!Result.Contains("<!DOCTYPE html>") && Result != null && !Result.Contains("message"))
                    {
                        try
                        {
                            var Response = JsonConvert.DeserializeObject<ProductsOrderBookResponse>(Result);
                            return Response;
                        }
                        catch (Exception e)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public async Task<OrderResponse> PlaceOrder(enTransactionMarketType marketType, CoinbasePro.Services.Orders.Types.OrderSide side, string Pair, decimal size, decimal limitPrice, decimal stopPrice, bool postOnly = false, Guid? clientOid = null)
        {
            try
            {

                var value = (CoinbasePro.Shared.Types.ProductType)Enum.Parse(typeof(CoinbasePro.Shared.Types.ProductType), Pair.Replace("_", ""), true);
                OrderResponse orderResponse = new OrderResponse();
                if (marketType == enTransactionMarketType.LIMIT)
                {
                    orderResponse = await coinbaseProClient.OrdersService.PlaceLimitOrderAsync(side, value, size, limitPrice, CoinbasePro.Services.Orders.Types.GoodTillTime.Day);

                }
                else if (marketType == enTransactionMarketType.MARKET)
                {
                    orderResponse = await coinbaseProClient.OrdersService.PlaceMarketOrderAsync(side, value, size, MarketOrderAmountType.Size);
                    
                }
                else if (marketType == enTransactionMarketType.STOP)
                {
                    orderResponse = await coinbaseProClient.OrdersService.PlaceStopOrderAsync(side, value, size, limitPrice);
                    
                }
                else if (marketType == enTransactionMarketType.STOP_Limit)
                {
                    orderResponse = await coinbaseProClient.OrdersService.PlaceStopLimitOrderAsync(side, value, size, stopPrice, limitPrice);
                    
                }
                //return null;
                return await Task.FromResult(orderResponse);
            }
            catch (Exception ex)
            {
                OrderResponse orderResponse = new OrderResponse();
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                orderResponse.Status = CoinbasePro.Services.Orders.Types.OrderStatus.Rejected;
                orderResponse.DoneReason = ex.Message.ToString();
                return await Task.FromResult(orderResponse);
                //return ex.Message;
            }
        }

        public async Task<object> CancelAllOrders()
        {
            try
            {
                var cancelOrder = await coinbaseProClient.OrdersService.CancelAllOrdersAsync();
                return cancelOrder;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return ex.Message;
            }
        }

        public async Task<CoinbaseCancelOrderRes> CancelOrderById(string id)
        {
            CoinbaseCancelOrderRes _res = new CoinbaseCancelOrderRes();
            try
            {
                //CancelOrderResponse cancelOrder = JsonConvert.DeserializeObject<CancelOrderResponse>("{\"OrderIds\":[\"144c6f8e-713f-4682-8435-5280fbe8b2b4\"]}");
                CancelOrderResponse cancelOrder = await coinbaseProClient.OrdersService.CancelOrderByIdAsync(id);
                _res.Result = cancelOrder;
                _res.ErrorCode = enErrorCode.Success;
                return await Task.FromResult(_res);
            }
            catch (Exception ex)
            {
                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                _res.ErrorCode = enErrorCode.API_Respose_Fail;
                _res.ErrorMsg = ex.Message;
                return _res;
            }
        }

        public async Task<object> GetAllProducts()
        {
            try
            {
                var products = await coinbaseProClient.ProductsService.GetAllProductsAsync();
                return products;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return ex.Message;
            }
        }

        public async Task<object> GetAllCurrencies()
        {
            try
            {
                var Currency = await coinbaseProClient.CurrenciesService.GetAllCurrenciesAsync();
                return Currency;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return ex.Message;
            }
        }

        public async Task<object> GetProductStats(string Pair)
        {
            try
            {
                CoinbasePro.Shared.Types.ProductType value = (CoinbasePro.Shared.Types.ProductType)Enum.Parse(typeof(CoinbasePro.Shared.Types.ProductType), Pair.Replace("_", ""), true);
                var ProductStats = await coinbaseProClient.ProductsService.GetProductStatsAsync(value);
                return ProductStats;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return ex.Message;
            }
        }

        public async Task<object> GetProductTicker(string Pair)
        {
            try
            {
                CoinbasePro.Shared.Types.ProductType value = (CoinbasePro.Shared.Types.ProductType)Enum.Parse(typeof(CoinbasePro.Shared.Types.ProductType), Pair.Replace("_", ""), true);
                var ProductTicker = await coinbaseProClient.ProductsService.GetProductTickerAsync(value);
                return ProductTicker;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return ex.Message;
            }
        }

        public async Task<IList<IList<ProductTrade>>> GetTrades(string Pair, int limit = 100, int numberOfPages = 0)
        {
            try
            {
                CoinbasePro.Shared.Types.ProductType value = Enum.Parse<CoinbasePro.Shared.Types.ProductType>(Pair);
                CoinBaseGlobalSettings.API_Key = "c2fb5263c0d0e8ee6a01aba1283930b9";
                CoinBaseGlobalSettings.Secret = "ROk/hPHZRRoNS5+NRODs44IElPS7D9AMF8LIuuJXJLtdEHDQp2CC5eFpc5uMDCXU6zkHriebdT/vL6JClnymhw==";
                //string APIKey = CoinBaseGlobalSettings.API_Key;
                //string SeckretKey = CoinBaseGlobalSettings.Secret;
                var Trades = await coinbaseProClient.ProductsService.GetTradesAsync(ProductType.EtcBtc, limit, numberOfPages);
                return Trades;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<object> GetFillsByProductId(string Pair, int limit = 100, int numberOfPages = 0)
        {
            try
            {
                CoinbasePro.Shared.Types.ProductType value = (CoinbasePro.Shared.Types.ProductType)Enum.Parse(typeof(CoinbasePro.Shared.Types.ProductType), Pair.Replace("_", ""), true);
                var FillTrade = await coinbaseProClient.FillsService.GetFillsByProductIdAsync(value, limit, numberOfPages);
                return FillTrade;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return ex.Message;
            }
        }

        public async Task<IEnumerable<CoinbasePro.Services.Accounts.Models.Account>> GetAllAccountsAsync()
        {
            try
            {
                IEnumerable<CoinbasePro.Services.Accounts.Models.Account> Data = await coinbaseProClient.AccountsService.GetAllAccountsAsync();
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
    }

    public interface ICoinBaseLP
    {
        Task DoWorKAsync();
    }
    public class CoinBaseLP : ICoinBaseLP
    {
        private readonly ILogger _logger;
        //private CoinbaseClient client;//komal 03 May 2019, Cleanup
        //private CoinbaseProWebSocket socket;
        private readonly ICoinBaseService _coinBaseService;
        private readonly ISignalRService _SignalRService;
        private string[] baseCur;
        private List<GetBuySellBook> OrderBookList;// = new List<GetBuySellBook>();
        private readonly IMediator _mediator;
        private IMemoryCache _cache;
        private readonly ITransactionConfigService _transactionConfigService;

        public CoinBaseLP(ILogger<CoinBaseLP> logger, ICoinBaseService coinBaseService, ISignalRService SignalRService , IMemoryCache Cache, IMediator mediator, ITransactionConfigService TransactionConfigService)
        {
            _logger = logger;
            _coinBaseService = coinBaseService;
            _SignalRService = SignalRService;
            _mediator = mediator;
            _cache = Cache;
            _transactionConfigService = TransactionConfigService;
        }

        public async Task DoWorKAsync()
        {
            try
            {                

                CoinbasePro.CoinbaseProClient coinbaseProClient = new CoinbasePro.CoinbaseProClient();
                //baseCur = new string[] { "Btc", "Gbp", "Usd" };
                _cache.Set<string[]>("CoinbaseSymbol", Enum.GetNames(typeof(ProductType)));
                string[] symbol = _transactionConfigService.LpPairListConvertorV1(_cache.Get<string[]>("CoinbaseSymbol"), Convert.ToInt16(enAppType.Coinbase));
                List<ProductType> products = Array.ConvertAll<string, ProductType>(symbol, x => Enum.Parse<ProductType>((string)x)).ToList();
                //var products = new List<ProductType>() { ProductType.BchBtc,ProductType.BchGbp,ProductType.BchUsd,ProductType.BtcGbp,ProductType.BtcUsd,ProductType.EtcBtc,ProductType.EtcGbp,ProductType.EtcUsd,ProductType.EthBtc,ProductType.EthGbp,ProductType.EthUsd,
                //    ProductType.LtcBtc,ProductType.LtcGbp,ProductType.LtcUsd,ProductType.ZrxBtc,ProductType.ZrxUsd};
                _cache.Set<List<ProductType>>("CoinbaseTradePair", products);
                //var channels = new List<ChannelType>() { ChannelType.Full, ChannelType.User, ChannelType.Ticker, ChannelType.Heartbeat, ChannelType.Level2 };
                //coinbaseProClient.WebSocket.Start(_cache.Get<List<ProductType>>("CoinbaseTradePair"), channels);
                //coinbaseProClient.WebSocket.OnLevel2UpdateReceived += WebSocket_OnLevel2UpdateReceived; //Order Book
                //coinbaseProClient.WebSocket.OnHeartbeatReceived += WebSocket_OnHeartbeatReceived;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        //private void WebSocket_OnHeartbeatReceived(object sender, CoinbasePro.WebSocket.Models.Response.WebfeedEventArgs<CoinbasePro.WebSocket.Models.Response.Heartbeat> e)
        //{
        //    //Console.WriteLine(e.ToString());
        //    //throw new NotImplementedException();
        //}

        //private void WebSocket_OnLevel2UpdateReceived(object sender, CoinbasePro.WebSocket.Models.Response.WebfeedEventArgs<CoinbasePro.WebSocket.Models.Response.Level2> e)
        //{
        //    try
        //    {
        //        if (e.LastOrder.Changes.Count > 0)
        //        {
        //            BulkBuySellBook OrderBook = new BulkBuySellBook();
        //            //foreach (var c in baseCur)
        //            //{
        //            //    BaseCurrency = c;
        //            //    if (e.LastOrder.ProductId.ToString().Contains(c))
        //            //    {
        //            //        Pair = e.LastOrder.ProductId.ToString().Split(c)[0].ToString();
        //            //        if (!string.IsNullOrEmpty(Pair))
        //            //            break;
        //            //    }
        //            //}
        //            //Pair += "_" + BaseCurrency;
        //            //HelperForLog.WriteLogForSocket("Depth ", "Original Pair", e.LastOrder.ProductId.ToString() + " New Pair " + Pair);
        //            CoinbaseBuySellBook buySellBook = new CoinbaseBuySellBook();
        //            buySellBook.Symbol = e.LastOrder.ProductId.ToString();
        //            buySellBook.Asks = new List<GetBuySellBook>();
        //            buySellBook.Bids = new List<GetBuySellBook>();
        //            //if (Convert.ToDecimal(e.LastOrder.Changes[0][1]) != 0)
        //            //{
        //            //    OrderBookList = new List<GetBuySellBook>();
        //            //    OrderBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(e.LastOrder.Changes[0][2]), Price = Convert.ToDecimal(e.LastOrder.Changes[0][1]) });
        //            //    if (e.LastOrder.Changes[0][0].ToString().ToUpper() == "SELL")
        //            //    {
        //            //        buySellBook.Asks = OrderBookList;
        //            //    }
        //            //    if (e.LastOrder.Changes[0][0].ToString().ToUpper() == "BUY")
        //            //    {
        //            //        buySellBook.Bids = OrderBookList;
        //            //    }
        //            //    _mediator.Send(buySellBook);
        //            //}
        //            if (e.LastOrder.Changes[0] != null)
        //            {
        //                foreach(var Data in e.LastOrder.Changes[0])
        //                {
        //                    GetBuySellBook OrderBookResult = new GetBuySellBook() { Amount = Convert.ToDecimal(Data[2]), Price = Convert.ToDecimal(Data[1]) };
        //                    if (Data[0].ToString().ToUpper() == "SELL")
        //                    {
        //                        buySellBook.Asks.Add(OrderBookResult);
        //                    }
        //                    if (Data[0].ToString().ToUpper() == "BUY")
        //                    {
        //                        buySellBook.Bids.Add(OrderBookResult);
        //                    }
                            
        //                }
        //                _mediator.Send(buySellBook);
        //            }
        //        }
        //    } 
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //    }            
        //}
    }
    public class CoinbaseConsumeScopedServiceHostedService : IHostedService
    {
        private readonly ILogger _logger;

        public CoinbaseConsumeScopedServiceHostedService(IServiceProvider services,
            ILogger<CoinbaseConsumeScopedServiceHostedService> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Coinbase Consume Scoped Service Hosted Service is starting.");

            DoWork();

            return Task.CompletedTask;
        }

        private void DoWork()
        {
            _logger.LogInformation(
                "Coinbase Consume Scoped Service Hosted Service is working.");

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<ICoinBaseLP>();

                scopedProcessingService.DoWorKAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Coinbase Consume Scoped Service Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
    
    public class CoinBaseOrderBook
    {
        public string Id { get; set; }
        public string BaseCurrency { get; set; }
        public string QuoteCurrency { get; set; }
        public double BaseMinSize { get; set; }
        public double BaseMaxSize { get; set; }
        public double QuoteIncrement { get; set; }
    }
}