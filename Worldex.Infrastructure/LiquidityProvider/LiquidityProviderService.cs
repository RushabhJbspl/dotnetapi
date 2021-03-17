
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider1;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.LiquidityProvider;
using CoinbasePro.Services.Products.Models;
using CoinbasePro.Services.Products.Models.Responses;
using CryptoExchange.Net.Objects;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.LiquidityProvider
{
    public interface ILiquidityProviderService
    {
        //void sendBinanceOrderBook();
        //List<GetBuySellBook> GetOrderBook(string Pair);
        Task SendPoloniexOrderBookAsync(string[] Symbol);
        Task SendTradesatoshiOrderBookAsync(string[] Symbol);
        Task SendBinanceOrderBookAsync(string[] Symbol);
        Task SendHuobiOrderBookAsync(string[] Symbol);  
        Task SendBittrexOrderBookAsync(string[] Symbol);
        Task SendCoinBaseOrderBookAsync(string[] Symbol);
        string[] GetPair();
        // Meditor through send Data
        Task SocketBinanceOrderBookAsync(BinanceBuySellBook BinanceOrderBook);
        Task SocketHuobiOrderBookAsync(HuobiBuySellBook BinanceOrderBook);
        Task SocketBittrexOrderBookAsync(BittrexTradeHistory BittrexOrderBook);
        Task SocketCoinBaseOrderBookAsync(CoinbaseBuySellBook CoinbaseOrderBook);
    }
    public class LiquidityProviderService : ILiquidityProviderService
    {
        private RedisConnectionFactory _fact;
        private readonly ILogger<LiquidityProviderService> _logger;
        private readonly IBinanceLPService _BinanceLPService;
        private readonly IHuobiLPService _huobiLPService;
        private readonly IBitrexLPService _BitrexLPService;
        private readonly IPoloniexService _poloniexService;
        private readonly IKrakenLPService _krakenLPService;
        private readonly ISignalRService _SignalRService;
        private readonly ITradeSatoshiLPService _TradeSatoshiLPService;
        private readonly ISignalRQueue _signalRQueue;
        private readonly ICoinBaseService _coinBaseService;
        private readonly ICommonRepository<TradePairMaster> _tradeMasterRepository;
        private IMemoryCache _cache;

        private List<GetBuySellBook> OrderBookList;// = new List<GetBuySellBook>();
        private readonly IMediator _mediator;

        public LiquidityProviderService(IMediator mediator, RedisConnectionFactory Factory, ILogger<LiquidityProviderService> logger, IKrakenLPService krakenLPService,
            IBinanceLPService BinanceLPService, IHuobiLPService huobiLPService, IBitrexLPService BitrexLPService, IPoloniexService poloniexService, ISignalRService SignalRService,
            ITradeSatoshiLPService TradeSatoshiLPService, ISignalRQueue signalRQueue, ICoinBaseService coinBaseService, 
            ICommonRepository<TradePairMaster> TradeMasterRepository, IMemoryCache Cache)
        {
            _fact = Factory;
            _logger = logger;
            _BinanceLPService = BinanceLPService;
            _huobiLPService = huobiLPService;
            _BitrexLPService = BitrexLPService;
            _poloniexService = poloniexService;
            _SignalRService = SignalRService;
            _TradeSatoshiLPService = TradeSatoshiLPService;
            _krakenLPService = krakenLPService;
            _signalRQueue = signalRQueue;
            _coinBaseService = coinBaseService;
            _tradeMasterRepository = TradeMasterRepository;
            _cache = Cache;
            _mediator = mediator;
        }

        //public async void sendBinanceOrderBook()
        //{
        //    //var Redis = new RadisServices<BulkBuySellBook>(this._fact);
        //    //BulkBuySellBook Depth = new BulkBuySellBook();
        //    //string[] symbol = new string[] { "ETHBTC","BNBBTC","LTCBTC", "EOSETH", "BNTETH", "BCCBTC", "BNBETH", "BTCUSDT", "ETHUSDT", "OMGBTC" };
        //    //foreach (var market in symbol)
        //    //{
        //    //    var list = await _BinanceLPService.GetOrderBookAsync(market);
        //    //    List<GetBuySellBook> OrderBookList = new List<GetBuySellBook>();
        //    //    foreach (var obj in list.Data.Asks)
        //    //    {
        //    //        OrderBookList.Add(new GetBuySellBook() { Amount = obj.Quantity, Price = obj.Price });
        //    //    }
        //    //    Depth.OrderBook = OrderBookList;
        //    //    Redis.SaveToHash("Binance_Staging:SellerBook:PairData:" + market, Depth);
        //    //}

        //    //string[] symbol2 = new string[] { "BTC-LTC" };

        //}

        //public List<GetBuySellBook> GetOrderBook(string Pair)
        //{
        //    try
        //    {
        //        //var Redis = new RadisServices<BulkBuySellBook>(this._fact);
        //        List<GetBuySellBook> _Res = new List<GetBuySellBook>();
        //        //var list = Redis.GetData("Binance_Staging:SellerBook:PairData:" + Pair);
        //        //_Res = list.OrderBook;
        //        return _Res;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        public string[] GetPair()
        {
            try
            {
                string[] ConfigurationList = _tradeMasterRepository.GetAll().Select(e => e.PairName).ToArray();                
                return ConfigurationList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
                //throw ex;
            }
        }

        public async Task SendPoloniexOrderBookAsync(string[] Symbol)
        {
            try
            {
                string[] BaseCur = new string[] { "BTC", "USDT", "XMR", "ETH", "USDC" };
                foreach (var p in Symbol)
                {
                    var pair = p;
                    PoloniexOrderBook Res = new PoloniexOrderBook();
                    var orderBook = await _poloniexService.GetPoloniexOrderBooksAsync(pair.Split("_")[1] + "_" + pair.Split("_")[0], 10);
                    if (orderBook != null)
                    {
                        Res = JsonConvert.DeserializeObject<PoloniexOrderBook>(JsonConvert.SerializeObject(orderBook));
                        if (Res != null)
                        {
                            if (Res.asks != null)
                            {
                                OrderBookList = new List<GetBuySellBook>();
                                foreach (var obj in Res.asks)
                                {
                                    if (Convert.ToDecimal(obj[1]) != 0)
                                    {
                                        OrderBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj[1]), Price = Convert.ToDecimal(obj[0]) });
                                    }
                                }
                                //_SignalRService.BulkSellerBook(OrderBookList, pair.Split("_")[0] + "_" + pair.Split("_")[1], Core.Enums.enLiquidityProvider.Poloniex);
                                //_SignalRService.BulkSellerBook(OrderBookList, pair, Core.Enums.enLiquidityProvider.Poloniex);
                            }
                            //if (Res.bids != null)
                            //{
                            //    OrderBookList = new List<GetBuySellBook>();
                            //    foreach (var obj in Res.bids)
                            //    {
                            //        if (Convert.ToDecimal(obj[1]) != 0)
                            //        {
                            //            OrderBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj[1]), Price = Convert.ToDecimal(obj[0]) });
                            //        }
                            //    }
                            //    _SignalRService.BulkBuyerBook(OrderBookList, pair.Split("_")[0] + "_" + pair.Split("_")[1], Core.Enums.enLiquidityProvider.Poloniex);
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        public async Task  SendTradesatoshiOrderBookAsync(string[] Symbol)
        {
            try
            {
                TradeSatoshiOrderBook orderBook;                
                foreach (var pair in Symbol)
                {
                    orderBook = new TradeSatoshiOrderBook();
                    GetOrderBookReturn res = _TradeSatoshiLPService.GetOrderBookAsync(pair, depth: 10).GetAwaiter().GetResult();
                    if(res != null)
                    {
                        if(res.result != null)
                        {
                            orderBook = JsonConvert.DeserializeObject<TradeSatoshiOrderBook>(JsonConvert.SerializeObject(res));
                            if (orderBook != null)
                            {
                                if (orderBook.result != null)
                                {
                                    if (orderBook.result.sell != null)
                                    {
                                        OrderBookList = new List<GetBuySellBook>();
                                        foreach (var obj in orderBook.result.sell)
                                        {
                                            {
                                                OrderBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj.quantity), Price = Convert.ToDecimal(obj.rate) });
                                            }
                                        }
                                        //_SignalRService.BulkSellerBook(OrderBookList, pair, Core.Enums.enLiquidityProvider.TradeSatoshi);
                                    }
                                    //if (orderBook.Result.result.buy != null)
                                    //{
                                    //    OrderBookList = new List<GetBuySellBook>();
                                    //    foreach (var obj in orderBook.Result.result.buy)
                                    //    {
                                    //        {
                                    //            OrderBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj.quantity), Price = Convert.ToDecimal(obj.rate) });
                                    //        }
                                    //    }
                                    //    _SignalRService.BulkBuyerBook(OrderBookList, pair, Core.Enums.enLiquidityProvider.TradeSatoshi);
                                    //}
                                }
                            }
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        public async Task SendBinanceOrderBookAsync(string[] Symbol)
        {
            try
            {
                //string[] Symbol = GetPair();
                foreach (var pair in Symbol)
                {
                    var obj = await _BinanceLPService.GetOrderBookAsync(pair.Replace("_",""), 10);
                    if(obj.Data != null)
                    {
                        if (obj.Data.Asks.Count > 0)
                        {
                            OrderBookList = new List<GetBuySellBook>();
                            foreach (var book in obj.Data.Asks)
                            {
                                OrderBookList.Add(new GetBuySellBook() { Amount = book.Quantity, Price = book.Price });
                            }
                            //_SignalRService.BulkBuyerBook(OrderBookList, Pair += "_" + BaseCurrency, Core.Enums.enLiquidityProvider.Binance);
                            //_SignalRService.BulkSellerBook(OrderBookList, pair, Core.Enums.enLiquidityProvider.Binance);
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        public async Task SendBittrexOrderBookAsync(string[] Symbol)
        {
            try
            {
                //string[] Symbol = GetPair();
                foreach (var pair in Symbol)
                {
                    var cnt = 0;
                    var pair1 = pair.Split("_")[1].ToUpper() + "-" + pair.Split("_")[0].ToUpper();
                    var book = await _BitrexLPService.GetOrderBookAsync(pair1);
                    if(book != null)
                    {
                        if(book.Data != null)
                        {
                           if (book.Data.Sell.Count > 0)
                            {
                                OrderBookList = new List<GetBuySellBook>();
                                foreach (var obj in book.Data.Sell)
                                {
                                    OrderBookList.Add(new GetBuySellBook() { Amount = obj.Quantity, Price = obj.Rate });
                                    cnt += 1;
                                    if (cnt == 10)
                                        break;
                                }
                                //_SignalRService.BulkBuyerBook(OrderBookList, pair.Split("-")[1].ToUpper() + "_" + pair.Split("-")[0].ToUpper(), Core.Enums.enLiquidityProvider.Bittrex);
                                _SignalRService.BulkSellerBook(OrderBookList, pair, Core.Enums.enLiquidityProvider.Bittrex);
                            }
                        }                       
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        public async Task SendCoinBaseOrderBookAsync(string[] Symbol)
        {
            try
            {
                //string[] Symbol = GetPair();
                foreach (var pair in Symbol)
                {
                    var cnt = 0;
                    OrderBookList = new List<GetBuySellBook>();
                    //var Pair = pair.ToLower().Replace("_", " ");
                    //TextInfo info = CultureInfo.CurrentCulture.TextInfo;
                    //Pair = info.ToTitleCase(Pair).Replace(" ", string.Empty);

                    var Pair = pair.Split("_")[0].ToUpper() + "-" + pair.Split("_")[1].ToUpper();
                    var res = await _coinBaseService.GetProductOrderBook(Pair);
                    if(res != null)
                    {
                        if (res.ToString() == "NotFound" || res.ToString() == "The operation was canceled.")
                            continue;
                        var book = JsonConvert.DeserializeObject<ProductsOrderBookResponse>(JsonConvert.SerializeObject(res));
                        if (book.Asks != null)
                        {
                            OrderBookList = new List<GetBuySellBook>();
                            foreach (var obj in book.Asks)
                            {
                                OrderBookList.Add(new GetBuySellBook() { Amount = obj.Size, Price = obj.Price });
                                cnt += 1;
                                if (cnt == 10)
                                    break;
                            }
                            _SignalRService.BulkSellerBook(OrderBookList, pair, Core.Enums.enLiquidityProvider.Coinbase);
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        public async Task SendBitfinexOrderBookAsync(string[] Symbol)
        {
            try
            {
                //string[] Symbol = GetPair();
                foreach (var pair in Symbol)
                {
                    var cnt = 0;
                    OrderBookList = new List<GetBuySellBook>();
                    var Pair = pair.Split("_")[0].ToUpper() + "-" + pair.Split("_")[1].ToUpper();
                    var res = await _coinBaseService.GetProductOrderBook(Pair);
                    if (res != null)
                    {
                        if (res.ToString() == "NotFound" || res.ToString() == "The operation was canceled.")
                            continue;
                        var book = JsonConvert.DeserializeObject<ProductsOrderBookResponse>(JsonConvert.SerializeObject(res));
                        if (book.Asks != null)
                        {
                            OrderBookList = new List<GetBuySellBook>();
                            foreach (var obj in book.Asks)
                            {
                                OrderBookList.Add(new GetBuySellBook() { Amount = obj.Size, Price = obj.Price });
                                cnt += 1;
                                if (cnt == 10)
                                    break;
                            }
                            _SignalRService.BulkSellerBook(OrderBookList, pair, Core.Enums.enLiquidityProvider.Coinbase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        public Task SocketBinanceOrderBookAsync(BinanceBuySellBook BinanceOrderBook)
        {
            try
            {
                Task.Run( () => _mediator.Send(BinanceOrderBook));
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return Task.CompletedTask;
            }
        }

        public Task SocketBittrexOrderBookAsync(BittrexTradeHistory BittrexOrderBook)
        {
            try
            {
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return Task.CompletedTask;
            }
        }

        public Task SocketCoinBaseOrderBookAsync(CoinbaseBuySellBook CoinbaseOrderBook)
        {
            try
            {
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return Task.CompletedTask;
            }
        }

        public Task SocketHuobiOrderBookAsync(HuobiBuySellBook HuobiOrderBook)
        {
            try
            {
                Task.Run(() => _mediator.Send(HuobiOrderBook));
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return Task.CompletedTask;
            }
        }

        public async Task SendHuobiOrderBookAsync(string[] Symbol)
        {
            try
            {
                foreach (var pair in Symbol)
                {
                    // var obj = await _HuobiLPService.GetOrderBookAsync(pair.Replace("_", ""), 10);
                    var obj = await _huobiLPService.GetOrderBookAsync(pair.Replace("_", ""), 10);
                    if (obj.Data != null)
                    {
                        if (obj.Data.Asks.Count > 0)
                        {
                            OrderBookList = new List<GetBuySellBook>();
                            foreach (var book in obj.Data.Asks)
                            {
                                OrderBookList.Add(new GetBuySellBook() { Amount = book.Quantity, Price = book.Price });
                            }
                            //_SignalRService.BulkBuyerBook(OrderBookList, Pair += "_" + BaseCurrency, Core.Enums.enLiquidityProvider.Binance);
                            //_SignalRService.BulkSellerBook(OrderBookList, pair, Core.Enums.enLiquidityProvider.Binance);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


    }
}
