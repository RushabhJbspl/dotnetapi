using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider1;
using Worldex.Core.ViewModels.Transaction;
using CoinbasePro.Services.Products.Models.Responses;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Huobi.Net;
using Worldex.Core.Interfaces;
using Worldex.Core.Entities.Transaction;
using Microsoft.Extensions.Options;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Infrastructure.LiquidityProvider;
using ExchangeSharp;

namespace Worldex.Infrastructure.Services
{

    public class BinanceOrderBookHandler : IRequestHandler<BinanceBuySellBook>
    {
        private readonly ISignalRService _SignalRService;

        public BinanceOrderBookHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(BinanceBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            SellerBookList.Add(new GetBuySellBook() { Amount = obj.Quantity, Price = obj.Price });
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Binance);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            BuyerBookList.Add(new GetBuySellBook() { Amount = obj.Quantity, Price = obj.Price });
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Binance);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("BinanceOrderBookHandler", "BinanceOrderBookHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
    // add huobi handler
    public class HuobiOrderBookHandler : IRequestHandler<HuobiBuySellBook>
    {

        private readonly ISignalRService _signalRService;

        public HuobiOrderBookHandler(ISignalRService SignalRService)
        {
            _signalRService = SignalRService;
        }

        public async Task<Unit> Handle(HuobiBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            SellerBookList.Add(new GetBuySellBook() { Amount = obj.Quantity, Price = obj.Price });
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _signalRService.BulkBuyerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Huobi);
                    //await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Huobi);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            BuyerBookList.Add(new GetBuySellBook() { Amount = obj.Quantity, Price = obj.Price });
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _signalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Huobi);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("HuobiOrderBookHandler", "HuobiOrderBookHandler handle", ex);
                return await Task.FromResult(new Unit());
            }

        }
    }

    public class BittrexOrderBookHandler : IRequestHandler<BittrexBuySellBook>
    {
        private readonly ISignalRService _SignalRService;

        public BittrexOrderBookHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(BittrexBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            SellerBookList.Add(new GetBuySellBook() { Amount = obj.Quantity, Price = obj.Quantity });
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Bittrex);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            BuyerBookList.Add(new GetBuySellBook() { Amount = obj.Quantity, Price = obj.Quantity });
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Bittrex);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("BittrexOrderBookHandler", "BittrexOrderBookHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    public class CoinbaseOrderBookHandler : IRequestHandler<CoinbaseBuySellBook>
    {
        private readonly ISignalRService _SignalRService;

        public CoinbaseOrderBookHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(CoinbaseBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            SellerBookList.Add(new GetBuySellBook() { Amount = obj.Size, Price = obj.Price });
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Coinbase);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            BuyerBookList.Add(new GetBuySellBook() { Amount = obj.Size, Price = obj.Price });
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Coinbase);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CoinbaseOrderBookHandler", "CoinbaseOrderBookHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    public class PoloniexOrderBookHandler : IRequestHandler<PoloniexBuySellBook>
    {
        private readonly ISignalRService _SignalRService;

        public PoloniexOrderBookHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(PoloniexBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            if (Convert.ToDecimal(obj[1]) != 0)
                            {
                                SellerBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj[1]), Price = Convert.ToDecimal(obj[0]) });
                            }
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Poloniex);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            if (Convert.ToDecimal(obj[1]) != 0)
                            {
                                BuyerBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj[1]), Price = Convert.ToDecimal(obj[0]) });
                            }
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Poloniex);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PoloniexOrderBookHandler", "PoloniexOrderBookHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    public class TradeSatoshiOrderBookHandler : IRequestHandler<TradesatoshiBuySellBook>
    {
        private readonly ISignalRService _SignalRService;

        public TradeSatoshiOrderBookHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(TradesatoshiBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            SellerBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj.quantity), Price = Convert.ToDecimal(obj.rate) });
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.TradeSatoshi);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            BuyerBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj.quantity), Price = Convert.ToDecimal(obj.rate) });
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.TradeSatoshi);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("TradeSatoshiOrderBookHandler", "TradeSatoshiOrderBookHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    public class UpbitOrderBookHandler : IRequestHandler<UpbitBuySellBook>
    {
        private readonly ISignalRService _SignalRService;

        public UpbitOrderBookHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(UpbitBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            SellerBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj.ask_size), Price = Convert.ToDecimal(obj.ask_price) });
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Upbit);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            BuyerBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj.bid_size), Price = Convert.ToDecimal(obj.bid_price) });
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Upbit);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpbitOrderBookHandler", "UpbitOrderBookHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    public class GeminiOrderBookHandler : IRequestHandler<GeminiBuySellBook>
    {
        private readonly ISignalRService _SignalRService;

        public GeminiOrderBookHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(GeminiBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            SellerBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj.ask_size), Price = Convert.ToDecimal(obj.ask_price) });
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Gemini);
                    //HelperForLog.WriteLogForSocket("GeminiOrderBookHandler ", "LPCommonHandler", "Pair : " + data.Symbol + " SellerBookData: " + Helpers.JsonSerialize(BuyerBookList));
                    //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            BuyerBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj.bid_size), Price = Convert.ToDecimal(obj.bid_price) });
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Gemini);
                    //HelperForLog.WriteLogForSocket("GeminiOrderBookHandler ", "LPCommonHandler", "Pair : " + data.Symbol + " BuyerBookData: " + Helpers.JsonSerialize(BuyerBookList));
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GeminiOrderBookHandler", "GeminiOrderBookHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    public class ExmoOrderBookHandler : IRequestHandler<ExmoBuySellBook>
    {
        private readonly ISignalRService _SignalRService;

        public ExmoOrderBookHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(ExmoBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            SellerBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj.ask_size), Price = Convert.ToDecimal(obj.ask_price) });
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.EXMO);
                    //HelperForLog.WriteLogForSocket("ExmoOrderBookHandler ", "LPCommonHandler", "Pair : " + data.Symbol + " SellerBookData: " + Helpers.JsonSerialize(SellerBookList));
                    //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            BuyerBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj.bid_size), Price = Convert.ToDecimal(obj.bid_price) });
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.EXMO);
                    //HelperForLog.WriteLogForSocket("ExmoOrderBookHandler ", "LPCommonHandler", "Pair : " + data.Symbol + " BuyerBookData: " + Helpers.JsonSerialize(BuyerBookList));
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ExmoOrderBookHandler", "ExmoOrderBookHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    #region OKExOrderBookHandler
    /// <summary>
    /// Add New Handler for integrate new API OKEx by Pushpraj
    /// </summary>
    public class OKExOrderBookHandler : IRequestHandler<OKExBuySellBook>
    {
        private readonly ISignalRService _SignalRService;

        public OKExOrderBookHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(OKExBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            SellerBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj.quantity), Price = Convert.ToDecimal(obj.rate) });
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.OKEx);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            BuyerBookList.Add(new GetBuySellBook() { Amount = Convert.ToDecimal(obj.quantity), Price = Convert.ToDecimal(obj.rate) });
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.OKEx);
                    // HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair);
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("BinanceOrderBookHandler", "BinanceOrderBookHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
    #endregion

    public class KrakenOrderBookHandler : IRequestHandler<KrakenBuySellBook>
    {
        private readonly ISignalRService _SignalRService;

        public KrakenOrderBookHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(KrakenBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            SellerBookList.Add(new GetBuySellBook() { Amount = obj.ask_size, Price = obj.ask_price });
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Kraken);
                    //HelperForLog.WriteLogForSocket("KrakenOrderBookHandler ", "LPCommonHandler", "Pair : " + data.Symbol + " SellerBookData: " + Helpers.JsonSerialize(SellerBookList));
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            BuyerBookList.Add(new GetBuySellBook() { Amount = obj.bid_size, Price = obj.bid_price });
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Kraken);
                    //HelperForLog.WriteLogForSocket("KrakenOrderBookHandler ", "LPCommonHandler", "Pair : " + data.Symbol + " BuyerBookData: " + Helpers.JsonSerialize(BuyerBookList));
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("KrakenOrderBookHandler", "KrakenOrderBookHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    public class BifinexOrderBookHandler : IRequestHandler<BitfinexBuySellBook>
    {
        private readonly ISignalRService _SignalRService;

        public BifinexOrderBookHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(BitfinexBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            SellerBookList.Add(new GetBuySellBook() { Amount = obj.ask_size, Price = obj.ask_price });
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Bitfinex);
                    //HelperForLog.WriteLogForSocket("BifinexOrderBookHandler ", "LPCommonHandler", "Pair : " + data.Symbol + " SellerBookData: " + Helpers.JsonSerialize(SellerBookList));
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            BuyerBookList.Add(new GetBuySellBook() { Amount = obj.bid_size, Price = obj.bid_price });
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Bitfinex);
                    //HelperForLog.WriteLogForSocket("BifinexOrderBookHandler ", "LPCommonHandler", "Pair : " + data.Symbol + " BuyerBookData: " + Helpers.JsonSerialize(BuyerBookList));
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("BifinexOrderBookHandler", "BifinexOrderBookHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    #region "Yobit OrderBook:"
    public class YobitOrderBookHandler : IRequestHandler<YobitBuySellBook>
    {
        private readonly ISignalRService _SignalRService;

        public YobitOrderBookHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(YobitBuySellBook data, CancellationToken cancellationToken)
        {
            List<GetBuySellBook> SellerBookList = new List<GetBuySellBook>();
            List<GetBuySellBook> BuyerBookList = new List<GetBuySellBook>();
            try
            {
                if (data.Asks.Count > 0)
                {
                    var cnt = 0;
                    foreach (var obj in data.Asks)
                    {
                        {
                            SellerBookList.Add(new GetBuySellBook() { Amount = obj.amount, Price = obj.price });
                            cnt += 1;
                            if (cnt == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkSellerBook(SellerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Yobit);
                    //HelperForLog.WriteLogForSocket("BifinexOrderBookHandler ", "LPCommonHandler", "Pair : " + data.Symbol + " SellerBookData: " + Helpers.JsonSerialize(SellerBookList));
                }
                if (data.Bids.Count > 0)
                {
                    var cnt1 = 0;
                    foreach (var obj in data.Bids)
                    {
                        {
                            BuyerBookList.Add(new GetBuySellBook() { Amount = obj.amount, Price = obj.price });
                            cnt1 += 1;
                            if (cnt1 == 10)
                                break;
                        }
                    }
                    await _SignalRService.BulkBuyerBook(BuyerBookList, data.Symbol, Core.Enums.enLiquidityProvider.Yobit);
                    //HelperForLog.WriteLogForSocket("BifinexOrderBookHandler ", "LPCommonHandler", "Pair : " + data.Symbol + " BuyerBookData: " + Helpers.JsonSerialize(BuyerBookList));
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
    #endregion

    public class LiquidityProviderHandler : IRequestHandler<CommonOrderBookRequest>
    {
        private readonly ISignalRService _SignalRService;
        private readonly BinanceLPService _BinanceLPService;
        private readonly BitrexLPService _BitrexLPService;
        private readonly IHuobiLPService _HuobiLPService;
        private readonly IPoloniexService _poloniexService;
        private readonly ITradeSatoshiLPService _TradeSatoshiLPService;
        private readonly ICoinBaseService _coinBaseService;
        //Add New Interface class for OKEx API by Pushpraj
        private readonly IOKExLPService _oKExLPService;
        //End Add New Interface class for OKEx API by Pushpraj
        private readonly IGeminiLPService _GeminiLPService;
        private readonly IUpbitService _upbitService;
        private readonly IKrakenLPService _krakenLPService;
        private readonly IBitfinexLPService _bitfinexLPService;
        private readonly IYobitLPService _yobitLPService; //Add new variable for Yobit Exchange by Pushpraj as on 15-07-2019
        private readonly ICEXIOLPService _cEXIOLPService;
        private readonly IEXMOLPService _ExmoLPService;
        private IMemoryCache _cache;
        private readonly ITransactionConfigService _transactionConfigService;
        private List<GetBuySellBook> OrderBookList;// = new List<GetBuySellBook>();
        private readonly IMediator _mediator;
        ConfigureLP[] Symbol;
        private readonly ICommonRepository<TradingConfiguration> _tradingConfigurationRepository;// mansi-set bit liquidity Provider status is on
        private readonly ICommonRepository<LPFeedConfiguration> _lpFeedConfiguration;//Mansi -set LPConfig status bit

        private readonly IOptions<OrderBookConfig> appSettings;//mansi-set bit for liquidityProvider _OrderBook method call
        private readonly IOptions<TradeHistoryConfig> _appSetting;//mansi-set bit for liquidityProvider _TradeHistory method call


        public LiquidityProviderHandler(ISignalRService SignalRService,
            BinanceLPService BinanceLPService, IHuobiLPService HuobiLPService, BitrexLPService BitrexLPService, IPoloniexService poloniexService,
            ITradeSatoshiLPService TradeSatoshiLPService, ICoinBaseService coinBaseService, IUpbitService upbitService, IEXMOLPService eXMOLPService,
            IMemoryCache Cache, IMediator mediator, ITransactionConfigService TransactionConfigService, IOKExLPService oKExLPService,
            ICommonRepository<TradingConfiguration> TradingConfigurationRepository, ICommonRepository<LPFeedConfiguration> LPFeedConfiguration,
            IOptions<OrderBookConfig> app, IOptions<TradeHistoryConfig> appSetting, IKrakenLPService krakenLPService, IBitfinexLPService bitfinexLPService,
            IGeminiLPService geminiLPService, IYobitLPService yobitLPService, ICEXIOLPService cEXIOLPService)
        {
            _SignalRService = SignalRService;
            _BinanceLPService = BinanceLPService;
            _HuobiLPService = HuobiLPService;
            _BitrexLPService = BitrexLPService;
            _poloniexService = poloniexService;
            _SignalRService = SignalRService;
            _upbitService = upbitService;
            _TradeSatoshiLPService = TradeSatoshiLPService;
            _coinBaseService = coinBaseService;
            _cache = Cache;
            _mediator = mediator;
            _transactionConfigService = TransactionConfigService;
            _oKExLPService = oKExLPService; ///add by Pushpraj as on 11-06-2019 for OKEx Implementation
            _tradingConfigurationRepository = TradingConfigurationRepository;// mansi-set bit liquidity Provider status is on
            _lpFeedConfiguration = LPFeedConfiguration;// mansi-set bit LpConfig status is on
            appSettings = app;//mansi-set bit for liquidityProvider _OrderBook method call
            _appSetting = appSetting;//mansi-set bit for liquidityProvider _TradeHistory method call
            _bitfinexLPService = bitfinexLPService;
            _krakenLPService = krakenLPService;
            _GeminiLPService = geminiLPService;
            _ExmoLPService = eXMOLPService;
            _cEXIOLPService = cEXIOLPService;
            _yobitLPService = yobitLPService; //Add new variable Assignment for Yobit Exchange by Pushpraj as on 15-07-2019
        }

        public async Task<Unit> Handle(CommonOrderBookRequest data, CancellationToken cancellationToken)
        {
            try
            {

                List<TradingConfiguration> TradingConfiguration = _cache.Get<List<TradingConfiguration>>("TradingConfiguration");
                if (TradingConfiguration == null)
                {
                    TradingConfiguration = _tradingConfigurationRepository.List().ToList();
                    _cache.Set<List<TradingConfiguration>>("TradingConfiguration", TradingConfiguration);
                }
                else if (TradingConfiguration.Count() == 0)
                {
                    TradingConfiguration = _tradingConfigurationRepository.List().ToList();
                    _cache.Set<List<TradingConfiguration>>("TradingConfiguration", TradingConfiguration);
                }
                List<LPFeedConfiguration> LPFeedConfiguration = _cache.Get<List<LPFeedConfiguration>>("LPFeedConfiguration");
                if (LPFeedConfiguration == null)
                {
                    LPFeedConfiguration = _lpFeedConfiguration.List().ToList();
                    _cache.Set<List<LPFeedConfiguration>>("LPFeedConfiguration", LPFeedConfiguration);
                }
                else if (LPFeedConfiguration.Count() == 0)
                {
                    LPFeedConfiguration = _lpFeedConfiguration.List().ToList();
                    _cache.Set<List<LPFeedConfiguration>>("LPFeedConfiguration", LPFeedConfiguration);
                }
                var LpStatus = TradingConfiguration.Where(e => e.Name == enTradingType.Liquidity.ToString()).FirstOrDefault();
                if (LpStatus != null && LpStatus.Status == 1)
                {

                    var LpConfigstatus = LPFeedConfiguration.Find(e => e.Name == data.LpType.ToString());
                    if (LpConfigstatus != null)
                    {
                        switch (data.LpType)
                        {
                            case enAppType.Binance:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    //Symbol =  GetPair(_cache.Get<string[]>("BinanceSymbol"),data.LpType);
                                    Symbol = GetPair(data.LpType);
                                    foreach (var Data in Symbol)
                                    {
                                        var obj = await _BinanceLPService.GetOrderBookAsync(Data.OpCode, 10);
                                        if (obj.Data != null)
                                        {
                                            var BinanceOrderBook = new BinanceBuySellBook()
                                            {
                                                Symbol = Data.Pair,
                                                Asks = obj.Data.Asks,
                                                Bids = obj.Data.Bids
                                            };
                                            await _mediator.Send(BinanceOrderBook);
                                        }

                                        //var obj1 = await _BinanceLPService.GetTradeHistoryAsync(Data.OpCode, 10);
                                        //if (obj1.Data != null)
                                        //{
                                        //    var BinanceTradeHistory = new BinanceTradeHistory()
                                        //    {
                                        //        Symbol = Data.Pair,
                                        //        History = obj1.Data.ToList()
                                        //    };
                                        //    _mediator.Send(BinanceTradeHistory);
                                        //}
                                    }
                                }
                                return await Task.FromResult(new Unit());

                            case enAppType.Huobi:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    Symbol = GetPair(data.LpType);
                                    foreach (var Data in Symbol)
                                    {
                                        var obj = await _HuobiLPService.GetOrderBookAsync(Data.OpCode, 10);
                                        if (obj.Data != null)
                                        {
                                            var HuobiOrderBook = new HuobiBuySellBook()
                                            {
                                                Symbol = Data.Pair,
                                                Asks = obj.Data.Asks,
                                                Bids = obj.Data.Bids
                                            };
                                            _mediator.Send(HuobiOrderBook);
                                        }

                                        var obj1 = await _HuobiLPService.GetTradeHistoryAsync(Data.OpCode, 10);
                                        if (obj1.Data != null)
                                        {
                                            var HuobiTradeHistory = new HuobiTradeHistory()
                                            {
                                                Symbol = Data.Pair,
                                                History = obj1.Data.ToList()
                                            };
                                            _mediator.Send(HuobiTradeHistory);
                                        }
                                    }
                                }
                                return await Task.FromResult(new Unit());

                            case enAppType.Bittrex:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    //Symbol = GetPair(_cache.Get<string[]>("BittrexSymbol"), data.LpType);
                                    Symbol = GetPair(data.LpType);

                                    foreach (var Data in Symbol)
                                    {
                                        var book = await _BitrexLPService.GetOrderBookAsync(Data.OpCode);
                                        if (book != null)
                                        {
                                            Bittrex.Net.Objects.BittrexOrderBook data1 = book.Data;
                                            if (data1 != null)
                                            {
                                                var BittrexOrderBook = new BittrexBuySellBook()
                                                {
                                                    Symbol = Data.Pair,
                                                    Asks = data1.Sell,
                                                    Bids = data1.Buy
                                                };
                                                _mediator.Send(BittrexOrderBook);
                                            }
                                        }

                                        var obj1 = await _BitrexLPService.GetTradeHistoryAsync(Data.OpCode);
                                        if (obj1.Data != null)
                                        {
                                            var BittrexTradeHistory = new BittrexTradeHistory()
                                            {
                                                Symbol = Data.Pair,
                                                History = obj1.Data.ToList()
                                            };
                                            _mediator.Send(BittrexTradeHistory);
                                        }
                                    }
                                }
                                return await Task.FromResult(new Unit());

                            case enAppType.Coinbase:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    //Symbol = GetPair(_cache.Get<string[]>("CoinbaseSymbol"), data.LpType);
                                    Symbol = GetPair(data.LpType);
                                    foreach (var Data in Symbol)
                                    {
                                        var res = await _coinBaseService.GetProductOrderBook(Data.Pair);
                                        if (res != null)
                                        {
                                            if (res.ToString() == "NotFound" || res.ToString() == "The operation was canceled.")
                                                continue;
                                            var book = JsonConvert.DeserializeObject<ProductsOrderBookResponse>(JsonConvert.SerializeObject(res));
                                            var CoinbaseOrderBook = new CoinbaseBuySellBook()
                                            {
                                                Symbol = Data.Pair
                                            };
                                            if (book.Asks != null)
                                            {
                                                CoinbaseOrderBook.Asks = book.Asks.ToList();
                                            }
                                            if (book.Bids != null)
                                            {
                                                CoinbaseOrderBook.Bids = book.Bids.ToList();
                                            }
                                            _mediator.Send(CoinbaseOrderBook);
                                        }

                                        var obj1 = await _coinBaseService.GetTrades(Data.Pair, 10);
                                        if (obj1 != null && obj1[0] != null)
                                        {
                                            var CoinbaseTradeHistory = new CoinbaseTradeHistory()
                                            {
                                                Symbol = Data.Pair,
                                                History = obj1[0].ToList()
                                            };
                                            _mediator.Send(CoinbaseTradeHistory);
                                        }
                                    }
                                }
                                return await Task.FromResult(new Unit());

                            case enAppType.Poloniex:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    Symbol = GetPair(data.LpType);
                                    foreach (var Data in Symbol)
                                    {
                                        PoloniexOrderBook Res = new PoloniexOrderBook();
                                        var PoloniexorderBook = await _poloniexService.GetPoloniexOrderBooksAsync(Data.Pair, 10);
                                        if (PoloniexorderBook != null)
                                        {
                                            Res = JsonConvert.DeserializeObject<PoloniexOrderBook>(JsonConvert.SerializeObject(PoloniexorderBook));
                                            if (Res != null)
                                            {
                                                var PoloniexOrderBook = new PoloniexBuySellBook()
                                                {
                                                    Symbol = Data.Pair
                                                };
                                                if (Res.asks != null)
                                                {
                                                    PoloniexOrderBook.Asks = Res.asks;
                                                }
                                                if (Res.bids != null)
                                                {
                                                    PoloniexOrderBook.Bids = Res.bids;
                                                }
                                                _mediator.Send(PoloniexOrderBook);
                                            }

                                            var obj1 = await _poloniexService.GetPoloniexTradeHistoriesV1(Data.Pair, 10);
                                            if (obj1 != null)
                                            {
                                                var PoloniexTradeHistory = new PoloniexTradeHistoryV1()
                                                {
                                                    Symbol = Data.Pair,
                                                    History = obj1
                                                };
                                                _mediator.Send(PoloniexTradeHistory);
                                            }
                                        }
                                    }
                                }
                                return await Task.FromResult(new Unit());

                            case enAppType.TradeSatoshi:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    TradeSatoshiOrderBook orderBook;
                                    //Symbol = GetPair(_cache.Get<string[]>("TradeSatoshiSymbol"), data.LpType);
                                    Symbol = GetPair(data.LpType);
                                    foreach (var Data in Symbol)
                                    {
                                        orderBook = new TradeSatoshiOrderBook();
                                        GetOrderBookReturn res = _TradeSatoshiLPService.GetOrderBookAsync(Data.OpCode, depth: 10).GetAwaiter().GetResult();
                                        if (res != null)
                                        {
                                            if (res.result != null)
                                            {
                                                orderBook = JsonConvert.DeserializeObject<TradeSatoshiOrderBook>(JsonConvert.SerializeObject(res));
                                                if (orderBook != null)
                                                {
                                                    if (orderBook.result != null)
                                                    {
                                                        var TradesatoshiOrderBook = new TradesatoshiBuySellBook()
                                                        {
                                                            Symbol = Data.Pair
                                                        };
                                                        if (orderBook.result.sell != null)
                                                        {
                                                            TradesatoshiOrderBook.Asks = orderBook.result.sell;
                                                        }
                                                        if (orderBook.result.buy != null)
                                                        {
                                                            TradesatoshiOrderBook.Bids = orderBook.result.buy;
                                                        }
                                                        _mediator.Send(TradesatoshiOrderBook);
                                                    }
                                                }
                                            }
                                        }

                                        //var obj1 = await _TradeSatoshiLPService.GetTradeHistoryAsync(Data.OpCode, 10);
                                        //if (obj1.success && obj1.result != null)
                                        //{
                                        //    var TradesatoshiTradeHistory = new TradesatoshiTradeHistory()
                                        //    {
                                        //        Symbol = Data.Pair,
                                        //        History = obj1.result
                                        //    };
                                        //    _mediator.Send(TradesatoshiTradeHistory);
                                        //}
                                    }
                                }
                                return await Task.FromResult(new Unit());

                            case enAppType.UpBit:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {

                                    Symbol = GetPair(data.LpType);

                                    foreach (var Data in Symbol)
                                    {
                                        var res = await _upbitService.GetOrderBookAsync(Data.Pair);
                                        if (res != null)
                                        {
                                            if (res.ToString() == "NotFound" || res.ToString() == "The operation was canceled.")
                                                continue;
                                            var book = JsonConvert.DeserializeObject<UpbitOrderbookResponse>(JsonConvert.SerializeObject(res));
                                            var UpbitOrderBook = new UpbitBuySellBook()
                                            {
                                                Symbol = Data.Pair
                                            };
                                            foreach (var x in book.orderbook_units)
                                            {
                                                List<OrderbookUnit> Aunit = new List<OrderbookUnit>();
                                                List<OrderbookUnit> Bunit = new List<OrderbookUnit>();
                                                if (x.ask_price != null || x.ask_price != 0)
                                                {
                                                    Aunit.Add(x);
                                                }
                                                if (x.bid_price != null || x.bid_price != 0)
                                                {
                                                    Bunit.Add(x);
                                                }
                                                UpbitOrderBook.Asks = Aunit;
                                                UpbitOrderBook.Bids = Bunit;
                                                _mediator.Send(UpbitOrderBook);
                                            }

                                        }
                                        var obj1 = await _upbitService.GetTrandHistory(Data.Pair);
                                        if (obj1 != null)
                                        {

                                            var upbitTradeHistory = new UpbitTradesHistory()
                                            {
                                                Symbol = Data.Pair,
                                                History = obj1.Result.ToList()
                                            };
                                            _mediator.Send(upbitTradeHistory);
                                        }
                                    }
                                }
                                return await Task.FromResult(new Unit());

                            #region "OKEx"
                            ///add by Pushpraj as on 11-06-2019 for OKEx Implementation
                            case enAppType.OKEx:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    //OKExOrderBook OKEXorderBook;
                                    //Symbol = GetPair(_cache.Get<string[]>("TradeSatoshiSymbol"), data.LpType);
                                    Symbol = GetPair(data.LpType);
                                    foreach (var Data in Symbol)
                                    {
                                        //OKEXorderBook = new OKExOrderBook();
                                        var res = _oKExLPService.GetOrderBookAsync(Data.Pair, depth: 10, size: 10).GetAwaiter().GetResult();
                                        if (res != null)
                                        {
                                            OKExGetOrderBookReturn OKEXorderBook = JsonConvert.DeserializeObject<OKExGetOrderBookReturn>(JsonConvert.SerializeObject(res));
                                            if (OKEXorderBook != null)
                                            {
                                                var OKExcOrderBook = new OKExBuySellBook()
                                                {
                                                    Symbol = Data.Pair,
                                                };

                                                List<OKExOrderBookBuySell> AsksArray = new List<OKExOrderBookBuySell>();
                                                List<OKExOrderBookBuySell> BidsArray = new List<OKExOrderBookBuySell>();
                                                foreach (var x in OKEXorderBook.asks)
                                                {
                                                    //var quantity = 0.0m;
                                                    //var rate = 0.0m;                                        
                                                    //x.TryGetValue(0, out quantity);
                                                    //x.TryGetValue(1, out rate);
                                                    AsksArray.Add(new OKExOrderBookBuySell
                                                    {
                                                        quantity = x[1],
                                                        rate = x[0],
                                                    });
                                                }

                                                foreach (var x in OKEXorderBook.bids)
                                                {
                                                    //var quantity = 0.0m;
                                                    //var rate = 0.0m;
                                                    //x.TryGetValue(0, out quantity);
                                                    //x.TryGetValue(1, out rate);
                                                    BidsArray.Add(new OKExOrderBookBuySell
                                                    {
                                                        quantity = x[1],
                                                        rate = x[0],
                                                    });
                                                }

                                                OKExcOrderBook.Asks = AsksArray;
                                                OKExcOrderBook.Bids = BidsArray;
                                                _mediator.Send(OKExcOrderBook);
                                            }
                                        }

                                        var obj1 = await _oKExLPService.GetTradeHistoryAasync(Data.Pair, from: 0, to: 100, limit: 100);

                                        if (obj1 != null)
                                        {

                                            var OKExTradeHistory = new OKExTradeHistory()
                                            {
                                                Symbol = Data.Pair,
                                                History = obj1.result
                                            };
                                            _mediator.Send(OKExTradeHistory);
                                        }
                                    }
                                }
                                return await Task.FromResult(new Unit());
                            #endregion
                            ///End add by Pushpraj as on 11-06-2019 for OKEx Implementation

                            case enAppType.Kraken:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    Symbol = GetPair(data.LpType);
                                    foreach (var Data in Symbol)
                                    {
                                        var res = _krakenLPService.GetOrderBook(Data.Pair).GetAwaiter().GetResult();
                                        if (res != null && res.error.Count == 0 && res.result.Data != null)
                                        {
                                            KrakenGetOrderBookReturn KrakenorderBook = JsonConvert.DeserializeObject<KrakenGetOrderBookReturn>(JsonConvert.SerializeObject(res.result.Data));
                                            if (KrakenorderBook != null)
                                            {
                                                var KrakenOrderBook = new KrakenBuySellBook()
                                                {
                                                    Symbol = Data.Pair,
                                                };

                                                List<OrderbookUnit> AsksArray = new List<OrderbookUnit>();
                                                List<OrderbookUnit> BidsArray = new List<OrderbookUnit>();
                                                foreach (var x in KrakenorderBook.asks)
                                                {
                                                    AsksArray.Add(new OrderbookUnit
                                                    {
                                                        ask_size = x[1],
                                                        ask_price = Convert.ToInt32(x[0]),
                                                    });
                                                }

                                                foreach (var x in KrakenorderBook.bids)
                                                {
                                                    BidsArray.Add(new OrderbookUnit
                                                    {
                                                        bid_size = x[1],
                                                        bid_price = Convert.ToInt32(x[0]),
                                                    });
                                                }
                                                KrakenOrderBook.Asks = AsksArray;
                                                KrakenOrderBook.Bids = BidsArray;
                                                _mediator.Send(KrakenOrderBook);
                                            }
                                        }

                                        var obj1 = await _krakenLPService.GetTradeHistory(Data.Pair);

                                        if (obj1 != null)
                                        {
                                            var KrakenTradeHistory = new KrakenTradeHistory()
                                            {
                                                Symbol = Data.Pair,
                                                History = obj1.result
                                            };
                                            _mediator.Send(KrakenTradeHistory);
                                        }
                                    }
                                }
                                return await Task.FromResult(new Unit());

                            case enAppType.Bitfinex:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    Symbol = GetPair(data.LpType);
                                    foreach (var Data in Symbol)
                                    {
                                        var res = _bitfinexLPService.GetOrderbook(Data.Pair).GetAwaiter().GetResult();
                                        if (res != null && res.Data.asks.Count > 0 && res.Data.bids.Count > 0)
                                        {
                                            //BitfinexGetOrderBookReturn BitfinexorderBook = JsonConvert.DeserializeObject<BitfinexGetOrderBookReturn>(JsonConvert.SerializeObject(res));
                                            if (res.Data != null)
                                            {
                                                var BitfinexOrderBook = new BitfinexBuySellBook()
                                                {
                                                    Symbol = Data.Pair,
                                                };

                                                List<OrderbookUnit> AsksArray = new List<OrderbookUnit>();
                                                List<OrderbookUnit> BidsArray = new List<OrderbookUnit>();
                                                foreach (var x in res.Data.asks)
                                                {
                                                    //var quantity = 0.0m;
                                                    //var rate = 0.0m;                                        
                                                    //x.TryGetValue(0, out quantity);
                                                    //x.TryGetValue(1, out rate);
                                                    AsksArray.Add(new OrderbookUnit
                                                    {
                                                        ask_size = Convert.ToDecimal(x.amount),
                                                        ask_price = Convert.ToInt32(x.price),
                                                    });
                                                }

                                                foreach (var x in res.Data.bids)
                                                {
                                                    //var quantity = 0.0m;
                                                    //var rate = 0.0m;
                                                    //x.TryGetValue(0, out quantity);
                                                    //x.TryGetValue(1, out rate);
                                                    BidsArray.Add(new OrderbookUnit
                                                    {
                                                        bid_size = Convert.ToDecimal(x.amount),
                                                        bid_price = Convert.ToInt32(x.price),
                                                    });
                                                }
                                                BitfinexOrderBook.Asks = AsksArray;
                                                BitfinexOrderBook.Bids = BidsArray;
                                                _mediator.Send(BitfinexOrderBook);
                                            }
                                        }

                                        var obj1 = await _bitfinexLPService.GetTradeHistory(Data.Pair);

                                        if (obj1 != null)
                                        {
                                            var BitfinexTradeHistory = new BitfinexTradeHistoryRes()
                                            {
                                                Symbol = Data.Pair,
                                                History = obj1.result
                                            };
                                            _mediator.Send(BitfinexTradeHistory);
                                        }
                                    }
                                }
                                return await Task.FromResult(new Unit());

                            case enAppType.Gemini:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    Symbol = GetPair(data.LpType);
                                    foreach (var Data in Symbol)
                                    {
                                        var res = _GeminiLPService.GetOrderbook(Data.Pair).GetAwaiter().GetResult();
                                        if (res != null && res.Data.asks.Count > 0 && res.Data.bids.Count > 0)
                                        {
                                            if (res.Data != null)
                                            {
                                                var GeminiOrderBook = new GeminiBuySellBook()
                                                {
                                                    Symbol = Data.Pair,
                                                };

                                                List<GeminiOrderbookUnit> AsksArray = new List<GeminiOrderbookUnit>();
                                                List<GeminiOrderbookUnit> BidsArray = new List<GeminiOrderbookUnit>();
                                                foreach (var x in res.Data.asks)
                                                {
                                                    AsksArray.Add(new GeminiOrderbookUnit
                                                    {
                                                        ask_size = Convert.ToDecimal(x.amount),
                                                        ask_price = Convert.ToDecimal(x.price),
                                                    });
                                                }

                                                foreach (var x in res.Data.bids)
                                                {
                                                    BidsArray.Add(new GeminiOrderbookUnit
                                                    {
                                                        bid_size = Convert.ToDecimal(x.amount),
                                                        bid_price = Convert.ToDecimal(x.price),
                                                    });
                                                }
                                                GeminiOrderBook.Asks = AsksArray;
                                                GeminiOrderBook.Bids = BidsArray;
                                                _mediator.Send(GeminiOrderBook);
                                            }
                                        }
                                        var obj1 = await _GeminiLPService.GetTradeHistory(Data.Pair);

                                        if (obj1 != null)
                                        {
                                            var GeminiTradeHistory = new GeminiTradeHistory()
                                            {
                                                Symbol = Data.Pair,
                                                History = obj1.Data
                                            };
                                            _mediator.Send(GeminiTradeHistory);
                                        }
                                    }
                                }
                                return await Task.FromResult(new Unit());

                            #region "Yobit Order Book"
                            case enAppType.Yobit:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    Symbol = GetPair(data.LpType);
                                    foreach (var Data in Symbol)
                                    {
                                        var res = _yobitLPService.GetOrderBook(Data.Pair, 5).GetAwaiter().GetResult();
                                        if (res != null && res.Asks.Count > 0 && res.Bids.Count > 0)
                                        {
                                            //BitfinexGetOrderBookReturn BitfinexorderBook = JsonConvert.DeserializeObject<BitfinexGetOrderBookReturn>(JsonConvert.SerializeObject(res));
                                            if (res != null)
                                            {
                                                var YobitOrderBook = new YobitBuySellBook()
                                                {
                                                    Symbol = Data.Pair
                                                };

                                                List<YobitOrderBookUnit> AsksArray = new List<YobitOrderBookUnit>();
                                                List<YobitOrderBookUnit> BidsArray = new List<YobitOrderBookUnit>();

                                                foreach (var x in res.Asks)
                                                {
                                                    AsksArray.Add(new YobitOrderBookUnit
                                                    {
                                                        amount = Convert.ToDecimal(x.Value.Amount),
                                                        price = Convert.ToDecimal(x.Value.Price),

                                                    });
                                                }

                                                foreach (var x in res.Bids)
                                                {
                                                    BidsArray.Add(new YobitOrderBookUnit
                                                    {
                                                        amount = Convert.ToDecimal(x.Value.Amount),
                                                        price = Convert.ToDecimal(x.Value.Price),
                                                    });
                                                }
                                                YobitOrderBook.Asks = AsksArray;
                                                YobitOrderBook.Bids = BidsArray;
                                                _mediator.Send(YobitOrderBook);
                                            }
                                        }

                                        var obj1 = await _yobitLPService.GetTradeHistory(Data.Pair);

                                        if (obj1 != null)
                                        {
                                            var YobitTradeHistory = new YobitTradeHistoryResult()
                                            {
                                                Symbol = Data.Pair,
                                                Hisory = obj1,
                                            };
                                            _mediator.Send(YobitTradeHistory);
                                        }
                                    }
                                }
                                return await Task.FromResult(new Unit());
                            #endregion

                            case enAppType.CEXIO:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    Symbol = GetPair(data.LpType);

                                    foreach (var Data in Symbol)
                                    {
                                        var res = _cEXIOLPService.GetOrederBook(Data.Pair).GetAwaiter().GetResult();
                                        if (res != null && res.asks.Count > 0 && res.bids.Count > 0)
                                        {

                                            if (res != null)
                                            {
                                                var CexioOrderBook = new CEXIOBuySellBook()
                                                {
                                                    Symbol = Data.Pair
                                                };

                                                List<OrderbookUnit> AsksArray = new List<OrderbookUnit>();
                                                List<OrderbookUnit> BidsArray = new List<OrderbookUnit>();
                                                if (res.asks != null)
                                                {
                                                    foreach (var x in res.asks)
                                                    {
                                                        AsksArray.Add(new OrderbookUnit
                                                        {
                                                            ask_size = Convert.ToInt32(x[0]),
                                                            ask_price = Convert.ToInt32(x[1])
                                                        });
                                                    }

                                                }
                                                if (res.bids != null)
                                                {
                                                    foreach (var x in res.bids)
                                                    {
                                                        BidsArray.Add(new OrderbookUnit
                                                        {
                                                            bid_size = Convert.ToInt32(x[0]),
                                                            bid_price = Convert.ToInt32(x[0])
                                                        });
                                                    }


                                                }
                                                CexioOrderBook.Asks = AsksArray;
                                                CexioOrderBook.Bids = BidsArray;

                                                _mediator.Send(CexioOrderBook);
                                            }
                                            var obj1 = await _cEXIOLPService.GetCEXIOTradeHistory(Data.Pair);

                                            if (obj1 != null)
                                            {
                                                var CEXIOTradeHistory = new CEXIOTradeHistoryRes()
                                                {
                                                    Symbol = Data.Pair,
                                                    History = obj1.ToList()
                                                };
                                                _mediator.Send(CEXIOTradeHistory);
                                            }

                                        }
                                    }


                                }
                                return await Task.FromResult(new Unit());

                            case enAppType.EXMO:
                                if (LpConfigstatus.Status == (short)ServiceStatus.Active)
                                {
                                    Symbol = GetPair(data.LpType);
                                    foreach (var Data in Symbol)
                                    {
                                        var res = _ExmoLPService.GetOrederBook(Data.Pair).GetAwaiter().GetResult();
                                        if (res != null)
                                        {
                                            if (res.Data != null)
                                            {
                                                var ExmoOrderBook = new ExmoBuySellBook()
                                                {
                                                    Symbol = Data.Pair,
                                                };
                                                List<GeminiOrderbookUnit> AsksArray = new List<GeminiOrderbookUnit>();
                                                List<GeminiOrderbookUnit> BidsArray = new List<GeminiOrderbookUnit>();
                                                foreach (var x in res.Data.ask)
                                                {
                                                    AsksArray.Add(new GeminiOrderbookUnit
                                                    {
                                                        ask_size = Convert.ToDecimal(x[1]),
                                                        ask_price = Convert.ToDecimal(x[0]),
                                                    });
                                                }
                                                foreach (var x in res.Data.bid)
                                                {
                                                    BidsArray.Add(new GeminiOrderbookUnit
                                                    {
                                                        bid_size = Convert.ToDecimal(x[1]),
                                                        bid_price = Convert.ToDecimal(x[0]),
                                                    });
                                                }
                                                ExmoOrderBook.Asks = AsksArray;
                                                ExmoOrderBook.Bids = BidsArray;
                                                _mediator.Send(ExmoOrderBook);
                                            }
                                        }
                                        var obj1 = await _ExmoLPService.GetTradeHistory(Data.Pair);
                                        if (obj1 != null)
                                        {
                                            var ExmoTradeHistory = new ExmoTradeHistory()
                                            {
                                                Symbol = Data.Pair,
                                                History = obj1.Data
                                            };
                                            _mediator.Send(ExmoTradeHistory);
                                        }
                                    }
                                }
                                return await Task.FromResult(new Unit());
                        }
                    }
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LiquidityProviderHandler", "LiquidityProviderHandler", ex);
                return await Task.FromResult(new Unit());
            }
        }

        //public LPConverPairV1[] GetPair(string[] OldSymbol , enAppType LP)
        public ConfigureLP[] GetPair(enAppType LP)
        {
            try
            {                //_cache.Get<string[]>("BinanceSymbol")
                //LPConverPairV1[] symbol = _transactionConfigService.LpPairListConvertorWithLocalPair(OldSymbol, Convert.ToInt16(LP));
                ConfigureLP[] symbol = _transactionConfigService.LpPairListConvertorWithLocalPair(Convert.ToInt16(LP));
                return symbol;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetPair", "LiquidityProviderHandler", ex);
                return null;
            }
        }
    }


    public class BinanceTradeHistoryHandler : IRequestHandler<BinanceTradeHistory>
    {
        private readonly ISignalRService _SignalRService;

        public BinanceTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(BinanceTradeHistory Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.History)
                {
                    var TradeData = new GetTradeHistoryInfoV1();
                    TradeData.TrnNo = data.AggregateTradeId.ToString();
                    if (data.BuyerWasMaker)
                    {
                        TradeData.Type = "Buy";
                    }
                    else
                    {
                        TradeData.Type = "Sell";
                    }
                    TradeData.Price = data.Price;
                    TradeData.Amount = data.Quantity;
                    TradeData.Total = data.Price * data.Quantity;
                    TradeData.DateTime = data.Timestamp;
                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    TradeData.OrderType = "BuyORSell";
                    TradeData.SettledDate = data.Timestamp;
                    TradeData.SettledQty = data.Quantity;
                    TradeData.SettlementPrice = TradeData.Price;
                    //TradeData.Chargecurrency  = 
                    TradeHisotry.Add(TradeData);
                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.Binance);
                //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair + "Response : " + TradeData);
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("BinanceTradeHistoryHandler", "BinanceTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
    public class HuobiTradeHistoryHandler : IRequestHandler<HuobiTradeHistory>
    {
        private readonly ISignalRService _SignalRService;

        public HuobiTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }


        public async Task<Unit> Handle(HuobiTradeHistory Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.History)
                {
                    var TradeData = new GetTradeHistoryInfoV1();
                    TradeData.TrnNo = data.Id.ToString();
                    foreach (var obj in data.Details)
                    {
                        TradeData.Type = obj.Side.ToString();
                        TradeData.Price = obj.Price;
                        TradeData.Amount = obj.Amount;
                        TradeData.Total = obj.Price * obj.Amount;
                        TradeData.DateTime = obj.Timestamp;
                        TradeData.SettledQty = obj.Amount;
                        TradeData.SettlementPrice = TradeData.Price;
                    }


                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    TradeData.OrderType = "BuyORSell";
                    TradeData.SettledDate = data.Timestamp;

                    //TradeData.Chargecurrency  = 
                    TradeHisotry.Add(TradeData);
                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.Huobi);
                //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair + "Response : " + TradeData);
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("HuobiTradeHistoryHandler", "HuobiTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
    public class BittrexTradeHistoryHandler : IRequestHandler<BittrexTradeHistory>
    {
        private readonly ISignalRService _SignalRService;

        public BittrexTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(BittrexTradeHistory Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.History)
                {
                    var TradeData = new GetTradeHistoryInfoV1();
                    TradeData.TrnNo = data.Id.ToString();
                    TradeData.Type = data.FillType.ToString();
                    TradeData.Price = data.Price;
                    TradeData.Amount = data.Quantity;
                    TradeData.Total = data.Total;
                    TradeData.DateTime = data.Timestamp;
                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    TradeData.OrderType = "BuyORSell";
                    TradeData.SettledDate = data.Timestamp;
                    TradeData.SettledQty = data.Quantity;
                    TradeData.SettlementPrice = TradeData.Price;
                    //TradeData.Chargecurrency  = 
                    TradeHisotry.Add(TradeData);
                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.Bittrex);
                //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair + "Response : " + TradeData);
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("BittrexTradeHistoryHandler", "BittrexTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    public class TradeSatoshiTradeHistoryHandler : IRequestHandler<TradesatoshiTradeHistory>
    {
        private readonly ISignalRService _SignalRService;

        public TradeSatoshiTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(TradesatoshiTradeHistory Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.History)
                {
                    var TradeData = new GetTradeHistoryInfoV1();
                    TradeData.TrnNo = data.id.ToString();
                    TradeData.Type = data.orderType.ToString();
                    TradeData.Price = data.price;
                    TradeData.Amount = data.quantity;
                    TradeData.Total = data.price * data.quantity;
                    TradeData.DateTime = data.timeStamp;
                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    TradeData.OrderType = data.orderType;
                    TradeData.SettledDate = data.timeStamp;
                    TradeData.SettledQty = data.quantity;
                    TradeData.SettlementPrice = TradeData.Price;
                    //TradeData.Chargecurrency  = 
                    TradeHisotry.Add(TradeData);
                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.TradeSatoshi);
                //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair + "Response : " + TradeData);
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("TradeSatoshiTradeHistoryHandler", "TradeSatoshiTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
    public class UpbitTradeHistoryHandler : IRequestHandler<UpbitTradesHistory>
    {
        private readonly ISignalRService _SignalRService;

        public UpbitTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(UpbitTradesHistory Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.History)
                {
                    var TradeData = new GetTradeHistoryInfoV1();
                    TradeData.TrnNo = data.sequentialId.ToString();
                    TradeData.Type = data.askBid;
                    TradeData.Price = Convert.ToInt64(data.tradePrice);
                    TradeData.Amount = Convert.ToInt64(data.tradeVolume);
                    TradeData.Total = Convert.ToInt64(data.tradePrice) * Convert.ToInt64(data.tradeVolume);
                    TradeData.DateTime = Convert.ToDateTime(data.tradeTimestamp);
                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    TradeData.OrderType = data.askBid;
                    TradeData.SettledDate = Convert.ToDateTime(data.timestamp);
                    TradeData.SettledQty = Convert.ToInt64(data.tradeVolume);
                    TradeData.SettlementPrice = TradeData.Price;
                    //TradeData.Chargecurrency  = 
                    TradeHisotry.Add(TradeData);
                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.Upbit);
                //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair + "Response : " + TradeData);
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpbitTradeHistoryHandler", "UpbitTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
    public class CoinbaseTradeHistoryHandler : IRequestHandler<CoinbaseTradeHistory>
    {
        private readonly ISignalRService _SignalRService;

        public CoinbaseTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(CoinbaseTradeHistory Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.History)
                {
                    var TradeData = new GetTradeHistoryInfoV1();
                    TradeData.TrnNo = data.TradeId.ToString();
                    TradeData.Type = data.Side.ToString();
                    TradeData.Price = data.Price;
                    TradeData.Amount = data.Size;
                    TradeData.Total = data.Price * data.Size;
                    TradeData.DateTime = data.Time;
                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    TradeData.OrderType = data.Side.ToString();
                    TradeData.SettledDate = data.Time;
                    TradeData.SettledQty = data.Size;
                    TradeData.SettlementPrice = TradeData.Price;
                    //TradeData.Chargecurrency  = 
                    TradeHisotry.Add(TradeData);
                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.Coinbase);
                //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair + "Response : " + TradeData);
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CoinbaseTradeHistoryHandler", "CoinbaseTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    public class PoloniexTradeHistoryHandler : IRequestHandler<PoloniexTradeHistoryV1>
    {
        private readonly ISignalRService _SignalRService;

        public PoloniexTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(PoloniexTradeHistoryV1 Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.History)
                {
                    var TradeData = new GetTradeHistoryInfoV1();
                    TradeData.TrnNo = data.tradeID.ToString();
                    TradeData.Type = data.type.ToString();
                    TradeData.Price = data.rate;
                    TradeData.Amount = data.amount;
                    TradeData.Total = data.rate * data.amount;
                    TradeData.DateTime = data.date;
                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    TradeData.OrderType = data.type;
                    TradeData.SettledDate = data.date;
                    TradeData.SettledQty = data.amount;
                    TradeData.SettlementPrice = TradeData.Price;
                    //TradeData.Chargecurrency  = 
                    TradeHisotry.Add(TradeData);
                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.Poloniex);
                //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair + "Response : " + TradeData);
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PoloniexTradeHistoryHandler", "PoloniexTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    public class GeminiTradeHistoryHandler : IRequestHandler<GeminiTradeHistory>
    {
        private readonly ISignalRService _SignalRService;

        public GeminiTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(GeminiTradeHistory Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.History)
                {
                    var TradeData = new GetTradeHistoryInfoV1();
                    TradeData.TrnNo = data.tid.ToString();
                    TradeData.Type = data.type.ToString();
                    TradeData.Price = Convert.ToDecimal(data.price);
                    TradeData.Amount = Convert.ToDecimal(data.amount);
                    TradeData.Total = Convert.ToDecimal(data.price) * Convert.ToDecimal(data.amount);
                    TradeData.DateTime = Helpers.UTC_To_IST();
                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    TradeData.OrderType = "BuyORSell";
                    TradeData.SettledDate = Helpers.UTC_To_IST();
                    TradeData.SettledQty = Convert.ToDecimal(data.amount);
                    //TradeData.Chargecurrency  = 
                    TradeHisotry.Add(TradeData);
                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.Gemini);
                //HelperForLog.WriteLogForSocket("GeminiTradeHistoryHandler ", "LPCommonHandler", "Pair : " + Request.Symbol + " TradeHistoryData: " + Helpers.JsonSerialize(TradeHisotry));                
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GeminiTradeHistoryHandler", "GeminiTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }

    public class EXMOTradeHistoryHandler : IRequestHandler<ExmoTradeHistory>
    {
        private readonly ISignalRService _SignalRService;

        public EXMOTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(ExmoTradeHistory Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.History)
                {
                    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    origin = origin.AddSeconds(data.date);

                    var TradeData = new GetTradeHistoryInfoV1();
                    TradeData.TrnNo = data.trade_id.ToString();
                    TradeData.Type = data.type.ToString();
                    TradeData.Price = Convert.ToDecimal(data.price);
                    TradeData.Amount = Convert.ToDecimal(data.amount);
                    TradeData.Total = Convert.ToDecimal(data.price) * Convert.ToDecimal(data.amount);
                    TradeData.DateTime = origin;
                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    TradeData.OrderType = "BuyORSell";
                    TradeData.SettledDate = origin;
                    TradeData.SettledQty = Convert.ToDecimal(data.amount);
                    //TradeData.Chargecurrency  = 
                    TradeHisotry.Add(TradeData);
                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.EXMO);
                //HelperForLog.WriteLogForSocket("EXMOTradeHistoryHandler ", "LPCommonHandler", "Pair : " + Request.Symbol + " TradeHistoryData: " + Helpers.JsonSerialize(TradeHisotry));                
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("EXMOTradeHistoryHandler", "EXMOTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }



    #region Get OKEX Trade History    
    public class OKExTradeHistoryHandler : IRequestHandler<OKExTradeHistory>
    {
        private readonly ISignalRService _SignalRService;

        public OKExTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(OKExTradeHistory Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.History)
                {
                    var TradeData = new GetTradeHistoryInfoV1();
                    TradeData.TrnNo = data.trade_id;
                    //TradeData.Type = data.orderType.ToString();
                    TradeData.Price = Decimal.Parse(data.price);
                    TradeData.Amount = Decimal.Parse(data.size);
                    TradeData.Total = Decimal.Parse(data.price) * Decimal.Parse(data.size);
                    TradeData.DateTime = data.timestamp;
                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    TradeData.OrderType = data.side;
                    TradeData.SettledDate = data.timestamp;
                    TradeData.SettledQty = Decimal.Parse(data.size);
                    TradeData.SettlementPrice = TradeData.Price;
                    //TradeData.Chargecurrency  = 
                    TradeHisotry.Add(TradeData);
                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.OKEx);
                //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair + "Response : " + TradeData);
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("OKExTradeHistoryHandler", "OKExTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
    #endregion

    //public class KrakenTradeHistoryHandler : IRequestHandler<KrakenTradeHistory>
    //{
    //    private readonly ISignalRService _SignalRService;

    //    public KrakenTradeHistoryHandler(ISignalRService SignalRService)
    //    {
    //        _SignalRService = SignalRService;
    //    }

    //    public async Task<Unit> Handle(KrakenTradeHistory Request, CancellationToken cancellationToken)
    //    {
    //        List<GetTradeHistoryInfo> TradeHisotry = new List<GetTradeHistoryInfo>();
    //        try
    //        {
    //            var cnt = 0;
    //            foreach (var data in Request.History)
    //            {
    //                var TradeData = new GetTradeHistoryInfo();

    //                TradeData.TrnNo = long.Parse(data.tradeId);
    //                TradeData.Type = data.sideType.ToString();
    //                TradeData.Price = data.price;
    //                TradeData.Amount = data.quantity;
    //                TradeData.Total = data.price * data.quantity;
    //                TradeData.DateTime = DateTime.Parse(data.datetime);
    //                TradeData.Status = 1;
    //                TradeData.StatusText = "Success";
    //                TradeData.PairName = Request.Symbol;
    //                TradeData.ChargeRs = 0;
    //                TradeData.IsCancel = 0;
    //                TradeData.OrderType = data.orderType.ToString();
    //                TradeData.SettledDate = DateTime.Parse(data.datetime);
    //                TradeHisotry.Add(TradeData);

    //                //TradeData.SettledQty  = data.
    //                //TradeData.Chargecurrency  = 

    //                cnt += 1;
    //                if (cnt == 10)
    //                    break;
    //            }
    //            await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.Binance);
    //            //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair + "Response : " + TradeData);
    //            return await Task.FromResult(new Unit());
    //        }
    //        catch (Exception ex)
    //        {
    //            HelperForLog.WriteErrorLog("BinanceOrderBookHandler", "BinanceOrderBookHandler handle", ex);
    //            return await Task.FromResult(new Unit());
    //        }
    //    }
    //}

    #region "Kraken Trade History"
    /// <summary>
    /// Add new Handler for Kraken Exchange by Pushpraj as on 01-07-2019
    /// </summary>
    public class KrakenTradeHistoryHandler : IRequestHandler<KrakenTradeHistory>
    {
        private readonly ISignalRService _SignalRService;

        public KrakenTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(KrakenTradeHistory Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.History.Data)
                {
                    var TradeData = new GetTradeHistoryInfoV1();

                    TradeData.TrnNo = "";
                    TradeData.Type = data[3].ToString();
                    TradeData.Price = decimal.Parse(data[0].ToString());
                    TradeData.Amount = decimal.Parse(data[1].ToString());
                    TradeData.Total = decimal.Parse(data[0].ToString()) * decimal.Parse(data[1].ToString());
                    TradeData.DateTime = DateTime.Now;
                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    TradeData.OrderType = data[4].ToString();
                    TradeData.SettledDate = DateTime.Now;
                    TradeData.SettlementPrice = TradeData.Price;
                    TradeHisotry.Add(TradeData);

                    //TradeData.SettledQty  = data.
                    //TradeData.Chargecurrency  = 

                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.Binance);
                //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair + "Response : " + TradeData);
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("KrakenTradeHistoryHandler", "KrakenTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
    #endregion

    #region "Bitfinex Trade Histroy"
    public class BitfinexTradeHistoryHandler : IRequestHandler<BitfinexTradeHistoryRes>
    {
        private readonly ISignalRService _SignalRService;

        public BitfinexTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(BitfinexTradeHistoryRes Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.History)
                {
                    var TradeData = new GetTradeHistoryInfoV1();

                    TradeData.TrnNo = data.tid.ToString();
                    TradeData.Type = "";
                    TradeData.Price = decimal.Parse(data.price);
                    TradeData.Amount = decimal.Parse(data.amount);
                    TradeData.Total = decimal.Parse(data.price) * decimal.Parse(data.amount);
                    TradeData.DateTime = DateTime.Now;
                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    TradeData.OrderType = data.type;
                    TradeData.SettledDate = DateTime.Now;
                    TradeData.SettlementPrice = TradeData.Price;
                    TradeHisotry.Add(TradeData);

                    //TradeData.SettledQty  = data.
                    //TradeData.Chargecurrency  = 

                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.Binance);
                //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair + "Response : " + TradeData);
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("BitfinexTradeHistoryHandler", "BitfinexTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
    #endregion

    #region "Yobit Trade History"
    public class YobitTradeHistoryHandler : IRequestHandler<YobitTradeHistoryResult>
    {
        private readonly ISignalRService _SignalRService;

        public YobitTradeHistoryHandler(ISignalRService SignalRService)
        {
            _SignalRService = SignalRService;
        }

        public async Task<Unit> Handle(YobitTradeHistoryResult Request, CancellationToken cancellationToken)
        {
            List<GetTradeHistoryInfoV1> TradeHisotry = new List<GetTradeHistoryInfoV1>();
            try
            {
                var cnt = 0;
                foreach (var data in Request.Hisory)
                {
                    var TradeData = new GetTradeHistoryInfoV1();

                    TradeData.TrnNo = data.Id.ToString();
                    TradeData.Type = "";
                    TradeData.Price = data.Price;
                    TradeData.Amount = data.Amount;
                    TradeData.Total = data.Price * data.Amount;
                    TradeData.DateTime = data.Timestamp;
                    TradeData.Status = 1;
                    TradeData.StatusText = "Success";
                    TradeData.PairName = Request.Symbol;
                    TradeData.ChargeRs = 0;
                    TradeData.IsCancel = 0;
                    if (data.IsBuy)
                        TradeData.OrderType = OrderSide.Buy.ToString();
                    else
                        TradeData.OrderType = OrderSide.Sell.ToString();
                    TradeData.SettledDate = DateTime.Now;
                    TradeData.SettlementPrice = TradeData.Price;
                    TradeHisotry.Add(TradeData);

                    //TradeData.SettledQty  = data.
                    //TradeData.Chargecurrency  = 

                    cnt += 1;
                    if (cnt == 10)
                        break;
                }
                await _SignalRService.BulkOrderHistory(TradeHisotry, Request.Symbol, enLiquidityProvider.Yobit);
                //HelperForLog.WriteLogForSocket("DEpth ", "Original Pair", data.Symbol + " New Pair " + Pair + "Response : " + TradeData);
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("BitfinexTradeHistoryHandler", "BitfinexTradeHistoryHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
    #endregion
}