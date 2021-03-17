using Binance.Net;
using Binance.Net.Objects;
using Bittrex.Net.Objects;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.Arbitrage;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.LiquidityProvider;
using Worldex.Infrastructure.LiquidityProvider.TradeSatoshiAPI;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Huobi.Net;
using Huobi.Net.Objects;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Worldex.Infrastructure.LiquidityProvider.OKExAPI;
using Worldex.Core.Entities.Configuration;
using Worldex.Infrastructure.LiquidityProvider.KrakenAPI;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Infrastructure.LiquidityProvider.Yobit;
using Worldex.Infrastructure.LiquidityProvider.EXMO;

namespace Worldex.Infrastructure.Services
{
    public class CryptoWatcherHandler : IRequestHandler<CryptoWatcherReq>
    {
        private IMemoryCache _cache;
        private readonly IMediator _mediator;
        private readonly ITransactionConfigService _transactionConfigService;
        private readonly IFrontTrnRepository _frontTrnRepository;
        private readonly ICommonRepository<CronMaster> _cronMaster;

        public CryptoWatcherHandler(IMemoryCache Cache, IMediator mediator, ITransactionConfigService TransactionConfigService, IFrontTrnRepository FrontTrnRepository,
            ICommonRepository<CronMaster> CronMaster)
        {
            _cache = Cache;
            _mediator = mediator;
            _transactionConfigService = TransactionConfigService;
            _frontTrnRepository = FrontTrnRepository;
            _cronMaster = CronMaster;

        }

        public async Task<Unit> Handle(CryptoWatcherReq data, CancellationToken cancellationToken)
        {
            CronMaster cronMaster = new CronMaster();
            try
            {
                List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
                if (cronMasterList == null)
                {
                    cronMasterList = _cronMaster.List();
                    _cache.Set("CronMaster", cronMasterList);
                }
                else if (cronMasterList.Count() == 0)
                {
                    cronMasterList = _cronMaster.List();
                    _cache.Set("CronMaster", cronMasterList);
                }
                cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.CryptoWatcher).FirstOrDefault();
                //cronMaster = _cronMaster.FindBy(e => e.Id == (short)enCronMaster.CryptoWatcher).FirstOrDefault();
                if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
                {
                    List<ConfigureLP> symbol = GetPair().ToList();
                    foreach (var LPData in symbol)
                    {
                        LTPcls Req = new LTPcls()
                        {
                            Pair = LPData.Pair,
                            LpType = LPData.LPType,
                            Price = 0.0m
                        };
                        var Res = await _mediator.Send(Req);
                        //Req.Price = Res.Price;
                        if (Req.Price > 0)
                        {
                            var ResponseFromUpdateLTP = _frontTrnRepository.UpdateLTPData(Req);
                            if (!ResponseFromUpdateLTP)
                            {
                                _frontTrnRepository.InsertLTPData(Req);
                            }
                        }
                    }
                    // _frontTrnRepository.GetLocalConfigurationData(Convert.ToInt16(enAppType.COINTTRADINGLocal));
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }

        public ConfigureLP[] GetPair()
        {
            try
            {
                ConfigureLP[] symbol = _transactionConfigService.TradePairConfigurationV1();
                return symbol;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return null;
            }
        }
    }

    public class CommonPriceTickerHandler : IRequestHandler<RealTimeLtpChecker, RealTimeLtpChecker>
    {
        private readonly IMediator _mediator;

        public CommonPriceTickerHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RealTimeLtpChecker> Handle(RealTimeLtpChecker Request, CancellationToken cancellationToken)
        {
            string Url = "https://api.cryptowat.ch/markets/#exchange#/#Pair#/price"; // pair - ltcbtc - Ltc Base           
            try
            {
                foreach (var data in Request.List)
                {
                    data.Pair = Request.Pair;
                    var data1 = await _mediator.Send(data);
                    data.Price = data1.Price;
                }

                return await Task.FromResult(Request);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return await Task.FromResult(Request);
            }
        }
    }

    public class PriceTickerHandler : IRequestHandler<LTPcls, LTPcls>
    {
        public IWebApiSendRequest _WebAPISendRequest { get; set; }

        public PriceTickerHandler(IWebApiSendRequest WebAPISendRequest)
        {
            _WebAPISendRequest = WebAPISendRequest;
        }

        public async Task<LTPcls> Handle(LTPcls data, CancellationToken cancellationToken)
        {
            CryptoWatcherAPIResponse ResponseData = new CryptoWatcherAPIResponse();
            string Response = string.Empty;
            string Url = "https://api.cryptowat.ch/markets/#exchange#/#Pair#/price";
            try
            {

                var Pair = data.Pair.Split("_");

                switch (data.LpType)
                {
                    case (short)enAppType.Binance:
                        Url = "https://api.binance.com//api/v3/ticker/price?symbol=#Pair#";
                        Url = Url.Replace("#exchange#", "binance");
                        Url = Url.Replace("#Pair#", Pair[0].ToUpper() + Pair[1].ToUpper());
                        Response = await _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false);
                        if (!string.IsNullOrEmpty(Response))
                        {
                            BinanceWatcherAPIResponse BinanceResponseData = new BinanceWatcherAPIResponse();
                            try
                            {
                                BinanceResponseData = JsonConvert.DeserializeObject<BinanceWatcherAPIResponse>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Binance ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }
                            if (BinanceResponseData != null)
                            {
                                data.Price = BinanceResponseData.price;
                            }
                        }
                        break;
                    case (short)enAppType.Bittrex:
                        Url = "https://api.bittrex.com/api/v1.1/public/getticker?market=#Pair#";
                        Url = Url.Replace("#exchange#", "bittrex");
                        Url = Url.Replace("#Pair#", Pair[1].ToUpper() + "-" + Pair[0].ToUpper());
                        Response = await _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false);
                        if (!string.IsNullOrEmpty(Response))
                        {
                            CommonWatcherAPIResponse BittrexResponseData = new CommonWatcherAPIResponse();
                            try
                            {
                                BittrexResponseData = JsonConvert.DeserializeObject<CommonWatcherAPIResponse>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Bittrex ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }
                            if (BittrexResponseData != null && BittrexResponseData.success)
                            {
                                data.Price = BittrexResponseData.result.last;
                            }
                        }
                        break;
                    case (short)enAppType.Coinbase:
                        Url = "https://api-public.sandbox.pro.coinbase.com/products/#Pair#/ticker";
                        Url = Url.Replace("#exchange#", "coinbase-pro");
                        Url = Url.Replace("#Pair#", Pair[0].ToUpper() + "-" + Pair[1].ToUpper());
                        //                    web.Headers["User-Agent"] =
                        //"Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
                        //"(compatible; MSIE 6.0; Windows NT 5.1; " +
                        //".NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                        //            }
                        WebHeaderCollection HeaderCollection = new WebHeaderCollection();
                        HeaderCollection.Add(string.Format("User-Agent: {0}", ".NET CLR 1.1.4322; .NET CLR 2.0.50727;"));
                        Response = await _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 90000, false);
                        if (!string.IsNullOrEmpty(Response))
                        {
                            try
                            {
                                ResponseData = JsonConvert.DeserializeObject<CryptoWatcherAPIResponse>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Coinbase ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }
                            if (string.IsNullOrEmpty(ResponseData.error) && ResponseData.result != null)
                            {
                                data.Price = ResponseData.result.price;
                            }
                        }
                        break;
                    case (short)enAppType.Poloniex:
                        Url = "https://poloniex.com/public?command=returnTicker";
                        Url = Url.Replace("#exchange#", "poloniex");
                        Url = Url.Replace("#Pair#", Pair[1].ToLower() + "_" + Pair[0].ToLower());
                        Response = await _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false);
                        if (!string.IsNullOrEmpty(Response))
                        {
                            Dictionary<string, poloniexWatcherAPIResponse> poloniexResponseData = new Dictionary<string, poloniexWatcherAPIResponse>();
                            try
                            {
                                poloniexResponseData = JsonConvert.DeserializeObject<Dictionary<string, poloniexWatcherAPIResponse>>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Poloniex ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }
                            if (poloniexResponseData != null)
                            {
                                poloniexWatcherAPIResponse poloniexWatcher = new poloniexWatcherAPIResponse();
                                poloniexResponseData.TryGetValue(Pair[1].ToUpper() + "_" + Pair[0].ToUpper(), out poloniexWatcher);
                                if (poloniexWatcher?.last != 0)
                                {
                                    //poloniexResponseData.TryGetValue(Pair[1].ToUpper() + "_" + Pair[0].ToUpper(), out poloniexWatcher);
                                    data.Price = poloniexWatcher.last;
                                }
                            }
                        }
                        break;
                    case (short)enAppType.TradeSatoshi:
                        Url = "https://tradesatoshi.com/api/public/getticker?market=#Pair#";
                        Url = Url.Replace("#Pair#", data.Pair);
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            CommonWatcherAPIResponse TradesatoshiResponseData = new CommonWatcherAPIResponse();
                            try
                            {
                                TradesatoshiResponseData = JsonConvert.DeserializeObject<CommonWatcherAPIResponse>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " TradeSatoshi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (TradesatoshiResponseData != null && TradesatoshiResponseData.success && TradesatoshiResponseData.result != null)
                            {
                                data.Price = TradesatoshiResponseData.result.last;
                            }
                        }
                        break;

                    case (short)enAppType.UpBit:
                        Url = "https://api.upbit.com/v1/trades/ticks?market=#Pair#";
                        Url = Url.Replace("#Pair#", data.Pair);
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            List<UpbitWatcherAPIResponse> UpbitResponseData = new List<UpbitWatcherAPIResponse>();
                            try
                            {
                                UpbitResponseData = JsonConvert.DeserializeObject<List<UpbitWatcherAPIResponse>>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " TradeSatoshi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }
                            if (UpbitResponseData != null || UpbitResponseData.Count > 0)
                            {
                                data.Price = Convert.ToDecimal(UpbitResponseData[0].trade_price);
                            }
                        }
                        break;
                    case (short)enAppType.Huobi:
                        Url = "https://api.huobi.com/market/detail/merged?symbol=#Pair#";
                        Url = Url.Replace("#Pair#", Pair[0].ToLower() + Pair[1].ToLower());
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        //Response = "{\"status\":\"ok\",\"ch\":\"market.ethusdt.detail.merged\",\"ts\":1560505048373,\"tick\":{\"amount\":2838.6856972704545,\"open\":257.98,\"close\":255.13,\"high\":261.74,\"id\":101372929968,\"count\":1238,\"low\":252.0,\"version\":101372929968,\"ask\":[255.13,4.415570016850465],\"vol\":730750.5870850132,\"bid\":[255.1,10.0016]}}";
                        if (!string.IsNullOrEmpty(Response))
                        {
                            HuboiTickResult HuboiResponseData = new HuboiTickResult();
                            try
                            {
                                HuboiResponseData = JsonConvert.DeserializeObject<HuboiTickResult>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }
                            if (HuboiResponseData != null && HuboiResponseData.status.ToLower() == "ok")
                            {
                                data.Price = Convert.ToDecimal(HuboiResponseData.tick.close);
                            }
                        }
                        break;
                    /// Add new case for OKEx By Pushpraj as on 12-06-2019
                    case (short)enAppType.OKEx:
                        Url = "https://www.okex.com/api/spot/v3/instruments/#Pair#/ticker";
                        Url = Url.Replace("#Pair#", data.Pair);
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            OKExGetTokenPairDetailReturn OKExResponseData = new OKExGetTokenPairDetailReturn();
                            try
                            {
                                OKExResponseData = JsonConvert.DeserializeObject<OKExGetTokenPairDetailReturn>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (OKExResponseData != null && OKExResponseData.last != null)
                            {
                                data.Price = Decimal.Parse(OKExResponseData.last);
                                data.Pair = Pair[0].ToUpper() + Pair[1].ToUpper();
                            }
                        }
                        break;
                    ///End Add new case for OKEx By Pushpraj as on 12-06-2019
                    ///Add new case for Kraken Exchange by Pushpraj as on 02-07-2019
                    case (short)enAppType.Kraken:
                        Url = "https://api.kraken.com/0/public/Ticker?pair=###";
                        string Market = data.Pair;
                        Market = Market.Replace("-", "");
                        Url = Url.Replace("###", Market);
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            KrakenLTPCheckResponse KrakenResponseData = new KrakenLTPCheckResponse();
                            try
                            {
                                var Res = Response;
                                Res = Res.Replace(Market, "Data");
                                KrakenResponseData = JsonConvert.DeserializeObject<KrakenLTPCheckResponse>(Res);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (KrakenResponseData != null && KrakenResponseData.result  != null && KrakenResponseData.result.Data != null && KrakenResponseData.result.Data.l != null)
                            {
                                data.Price = Decimal.Parse(KrakenResponseData.result.Data.l[0]);
                            }
                        }
                        break;
                    case (short)enAppType.Bitfinex:
                        Url = "https://api.bitfinex.com/v1/pubticker/#Pair#";
                        data.Pair = data.Pair.Replace("_", "");
                        Url = Url.Replace("#Pair#", data.Pair.ToLower());
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            BitfinexTLTPResponse BitfinexResponseData = new BitfinexTLTPResponse();
                            try
                            {
                                BitfinexResponseData = JsonConvert.DeserializeObject<BitfinexTLTPResponse>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (BitfinexResponseData != null && BitfinexResponseData.last_price != null)
                            {
                                data.Price = Decimal.Parse(BitfinexResponseData.last_price);
                            }
                        }
                        break;
                    case (short)enAppType.Yobit:
                        Url = "https://yobit.net/api/3/ticker/#Pair#";
                        Url = Url.Replace("#Pair#", data.Pair.ToLower());
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            YobitAPIReqRes.YobitLTPCheckRespopnse YobitResponseData = new YobitAPIReqRes.YobitLTPCheckRespopnse();
                            try
                            {
                                Response = Response.Replace(data.Pair.ToLower(), "result");
                                YobitResponseData = JsonConvert.DeserializeObject<YobitAPIReqRes.YobitLTPCheckRespopnse>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (YobitResponseData != null && YobitResponseData.result.last != null)
                            {
                                data.Price = YobitResponseData.result.last;
                            }
                        }
                        break;
                    case (short)enAppType.EXMO:
                        Url = "https://api.exmo.com/v1/ticker/";
                        //Url = Url.Replace("#Pair#", data.Pair.ToLower());
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        //Response = "{  \"ARISTO_BTC\": {    \"buy_price\": \"10857.86202772\",    \"sell_price\": \"10886.56666667\",    \"last_trade\": \"10855.99837404\",    \"high\": \"11200\",    \"low\": \"10188.41046088\",    \"avg\": \"10697.1654344\",    \"vol\": \"923.62751737\",    \"vol_curr\": \"10026898.82679032\",    \"updated\": 1563253281  },  \"BTC_EUR\": {    \"buy_price\": \"10466.1378598\",    \"sell_price\": \"10539.5229907\",    \"last_trade\": \"10478.271\",    \"high\": \"10868.72333079\",    \"low\": \"9732.26093557\",    \"avg\": \"10268.83349735\",    \"vol\": \"97.16833554\",    \"vol_curr\": \"1018156.15251106\",    \"updated\": 1563253281  },  \"BTC_RUB\": {    \"buy_price\": \"687761.56570164\",    \"sell_price\": \"691853.31704932\",    \"last_trade\": \"691863.7\",    \"high\": \"709478.14005204\",    \"low\": \"654943.43250456\",    \"avg\": \"681094.1752638\",    \"vol\": \"385.66668201\",    \"vol_curr\": \"266828777.58820823\",    \"updated\": 1563253281  },  \"BTC_UAH\": {    \"buy_price\": \"283767.37185201\",    \"sell_price\": \"284561.635932\",    \"last_trade\": \"284373.05\",    \"high\": \"290915.68\",    \"low\": \"264500.00000001\",    \"avg\": \"280004.83537997\",    \"vol\": \"61.25790241\",    \"vol_curr\": \"17420096.54654336\",    \"updated\": 1563253278  },  \"BTC_PLN\": {    \"buy_price\": \"40471\",    \"sell_price\": \"41680.71631135\",    \"last_trade\": \"41241.613\",    \"high\": \"42434.371\",    \"low\": \"37899.267\",    \"avg\": \"40538.91716888\",    \"vol\": \"20.02135959\",    \"vol_curr\": \"825713.16423134\",    \"updated\": 1563253279  },  \"BTC_TRY\": {    \"buy_price\": \"65329.3300993\",    \"sell_price\": \"65695.6208342\",    \"last_trade\": \"65512.443\",    \"high\": \"66421.597\",    \"low\": \"64152.8147018\",    \"avg\": \"65141.22896655\",    \"vol\": \"16.23043758\",    \"vol_curr\": \"1063295.6174226\",    \"updated\": 1563253278  },  \"ROOBEE_BTC\": {    \"buy_price\": \"0.00000126\",    \"sell_price\": \"100\",    \"last_trade\": \"0.00000126\",    \"high\": \"0.00000126\",    \"low\": \"0.00000126\",    \"avg\": \"0.00000126\",    \"vol\": \"52521.89527195\",    \"vol_curr\": \"0.06617758\",    \"updated\": 1559851071  },  \"ROOBEE_ETH\": {    \"buy_price\": \"0.000001\",    \"sell_price\": \"100\",    \"last_trade\": \"0.0038\",    \"high\": \"0.0038\",    \"low\": \"0.0038\",    \"avg\": \"0.0038\",    \"vol\": \"2\",    \"vol_curr\": \"0.0076\",    \"updated\": 1559158999  },  \"DCR_BTC\": {    \"buy_price\": \"0.00269408\",    \"sell_price\": \"0.00273421\",    \"last_trade\": \"0.00271929\",    \"high\": \"0.00278\",    \"low\": \"0.0026487\",    \"avg\": \"0.00272368\",    \"vol\": \"560.40693082\",    \"vol_curr\": \"1.52390896\",    \"updated\": 1563253280  },  \"DCR_RUB\": {    \"buy_price\": \"1835.00000001\",    \"sell_price\": \"1900\",    \"last_trade\": \"1850.7307\",    \"high\": \"1928\",    \"low\": \"1816\",    \"avg\": \"1859.65768984\",    \"vol\": \"447.24192665\",    \"vol_curr\": \"827724.36397838\",    \"updated\": 1563253278  },  \"DCR_UAH\": {    \"buy_price\": \"740\",    \"sell_price\": \"780\",    \"last_trade\": \"771.688\",    \"high\": \"816\",    \"low\": \"728.2796\",    \"avg\": \"764.77468596\",    \"vol\": \"571.09374844\",    \"vol_curr\": \"440706.1925472\",    \"updated\": 1563253278  },  \"XTZ_BTC\": {    \"buy_price\": \"0.00000005\",    \"sell_price\": \"0.1\",    \"last_trade\": \"0\",    \"high\": \"0\",    \"low\": \"0\",    \"avg\": \"0\",    \"vol\": \"0\",    \"vol_curr\": \"0\",    \"updated\": 1560457219  },  \"XTZ_ETH\": {    \"buy_price\": \"0.0000005\",    \"sell_price\": \"9.6\",    \"last_trade\": \"0\",    \"high\": \"0\",    \"low\": \"0\",    \"avg\": \"0\",    \"vol\": \"0\",    \"vol_curr\": \"0\",    \"updated\": 1560457219  },  \"XTZ_USD\": {    \"buy_price\": \"0.0005\",    \"sell_price\": \"960\",    \"last_trade\": \"0\",    \"high\": \"0\",    \"low\": \"0\",    \"avg\": \"0\",    \"vol\": \"0\",    \"vol_curr\": \"0\",    \"updated\": 1560457219  },  \"XTZ_RUB\": {    \"buy_price\": \"0.00005\",    \"sell_price\": \"95000\",    \"last_trade\": \"0\",    \"high\": \"0\",    \"low\": \"0\",    \"avg\": \"0\",    \"vol\": \"0\",    \"vol_curr\": \"0\",    \"updated\": 1560457219  },  \"KICK_RUB\": {    \"buy_price\": \"0.08541\",    \"sell_price\": \"0.085999\",    \"last_trade\": \"0.08599999\",    \"high\": \"0.08959999\",    \"low\": \"0.07400009\",    \"avg\": \"0.08566396\",    \"vol\": \"4947163.15349906\",    \"vol_curr\": \"425455.98172928\",    \"updated\": 1563253277  },  \"USDC_BTC\": {    \"buy_price\": \"0.00009248\",    \"sell_price\": \"0.00009325\",    \"last_trade\": \"0.00009281\",    \"high\": \"0.00010633\",    \"low\": \"0.00009073\",    \"avg\": \"0.00009424\",    \"vol\": \"25310.09521955\",    \"vol_curr\": \"2.34902993\",    \"updated\": 1563253280  },  \"USDC_ETH\": {    \"buy_price\": \"0.00434614\",    \"sell_price\": \"0.00439269\",    \"last_trade\": \"0.00435603\",    \"high\": \"0.00458395\",    \"low\": \"0.00385563\",    \"avg\": \"0.00431678\",    \"vol\": \"21349.03167974\",    \"vol_curr\": \"92.99702246\",    \"updated\": 1563253221  },  \"USDC_USD\": {    \"buy_price\": \"1.00622713\",    \"sell_price\": \"1.01499999\",    \"last_trade\": \"1.0130258\",    \"high\": \"1.02507897\",    \"low\": \"0.97504475\",    \"avg\": \"1.00123916\",    \"vol\": \"32819.90258725\",    \"vol_curr\": \"33247.40807437\",    \"updated\": 1563253272  },  \"USDC_USDT\": {    \"buy_price\": \"0.98050867\",    \"sell_price\": \"0.9999522\",    \"last_trade\": \"0.98250414\",    \"high\": \"1.0079\",    \"low\": \"0.9779\",    \"avg\": \"0.99158544\",    \"vol\": \"29053.46844874\",    \"vol_curr\": \"28545.15303225\",    \"updated\": 1563253235  },  \"ETZ_BTC\": {    \"buy_price\": \"0.00001503\",    \"sell_price\": \"0.0000168\",    \"last_trade\": \"0.00001526\",    \"high\": \"0.000017\",    \"low\": \"0.00001507\",    \"avg\": \"0.00001551\",    \"vol\": \"72083.67704748\",    \"vol_curr\": \"1.09999691\",    \"updated\": 1563253256  },  \"ETZ_ETH\": {    \"buy_price\": \"0.000715\",    \"sell_price\": \"0.00074998\",    \"last_trade\": \"0.00074566\",    \"high\": \"0.00075\",    \"low\": \"0.00068443\",    \"avg\": \"0.0007133\",    \"vol\": \"50501.7076742\",    \"vol_curr\": \"37.65710334\",    \"updated\": 1563253278  },  \"ETZ_USDT\": {    \"buy_price\": \"0.15400001\",    \"sell_price\": \"0.17856956\",    \"last_trade\": \"0.17151515\",    \"high\": \"0.18\",    \"low\": \"0.15214958\",    \"avg\": \"0.1616465\",    \"vol\": \"50525.30259436\",    \"vol_curr\": \"8665.85485326\",    \"updated\": 1563253279  },  \"PTI_BTC\": {    \"buy_price\": \"0.00000029\",    \"sell_price\": \"0.0000003\",    \"last_trade\": \"0.00000029\",    \"high\": \"0.00000035\",    \"low\": \"0.00000028\",    \"avg\": \"0.0000003\",    \"vol\": \"1366662.12177255\",    \"vol_curr\": \"0.39633201\",    \"updated\": 1563253194  },  \"PTI_USDT\": {    \"buy_price\": \"0.0031\",    \"sell_price\": \"0.00319999\",    \"last_trade\": \"0.00314779\",    \"high\": \"0.00357724\",    \"low\": \"0.0031\",    \"avg\": \"0.00328829\",    \"vol\": \"2921623.79865926\",    \"vol_curr\": \"9196.65817718\",    \"updated\": 1563253271  },  \"PTI_EOS\": {    \"buy_price\": \"0.00071701\",    \"sell_price\": \"0.00076765\",    \"last_trade\": \"0.00075404\",    \"high\": \"0.00083588\",    \"low\": \"0.000717\",    \"avg\": \"0.00077034\",    \"vol\": \"2897888.68831581\",    \"vol_curr\": \"2185.12398653\",    \"updated\": 1563253281  },  \"ATMCASH_BTC\": {    \"buy_price\": \"0.00000001\",    \"sell_price\": \"0.00000002\",    \"last_trade\": \"0.00000002\",    \"high\": \"0.00000003\",    \"low\": \"0.00000001\",    \"avg\": \"0.00000002\",    \"vol\": \"10891915.99890614\",    \"vol_curr\": \"0.21783831\",    \"updated\": 1563253271  },  \"TRX_UAH\": {    \"buy_price\": \"0.66610184\",    \"sell_price\": \"0.68936327\",    \"last_trade\": \"0.66861947\",    \"high\": \"0.75\",    \"low\": \"0.64368589\",    \"avg\": \"0.67116854\",    \"vol\": \"474661.04137642\",    \"vol_curr\": \"317367.61391475\",    \"updated\": 1563252668  },  \"ETH_TRY\": {    \"buy_price\": \"1391.06449886\",    \"sell_price\": \"1400.81899089\",    \"last_trade\": \"1393.7569\",    \"high\": \"1423.1994\",    \"low\": \"1391.06449886\",    \"avg\": \"1405.5739026\",    \"vol\": \"161.21853553\",    \"vol_curr\": \"224699.4463092\",    \"updated\": 1563253274  },  \"XRP_TRY\": {    \"buy_price\": \"1.909481\",    \"sell_price\": \"1.91366592\",    \"last_trade\": \"1.9118204\",    \"high\": \"1.9778624\",    \"low\": \"1.8908163\",    \"avg\": \"1.9342533\",    \"vol\": \"19285.55421456\",    \"vol_curr\": \"36870.51597272\",    \"updated\": 1563253202  },  \"XLM_TRY\": {    \"buy_price\": \"0.52302037\",    \"sell_price\": \"0.52668792\",    \"last_trade\": \"0.52381487\",    \"high\": \"0.54313894\",    \"low\": \"0.52010371\",    \"avg\": \"0.52992613\",    \"vol\": \"40663.35673729\",    \"vol_curr\": \"21300.07092311\",    \"updated\": 1563253224  },  \"MNC_BTC\": {    \"buy_price\": \"0.00000042\",    \"sell_price\": \"0.00000044\",    \"last_trade\": \"0.00000042\",    \"high\": \"0.00000046\",    \"low\": \"0.00000042\",    \"avg\": \"0.00000043\",    \"vol\": \"29376.67404235\",    \"vol_curr\": \"0.0123382\",    \"updated\": 1563251894  },  \"MNC_ETH\": {    \"buy_price\": \"0.00001968\",    \"sell_price\": \"0.0000209\",    \"last_trade\": \"0.00002\",    \"high\": \"0.00002109\",    \"low\": \"0.00002\",    \"avg\": \"0.00002055\",    \"vol\": \"24907.68785049\",    \"vol_curr\": \"0.49815375\",    \"updated\": 1563253206  },  \"MNC_USD\": {    \"buy_price\": \"0.00461316\",    \"sell_price\": \"0.00474998\",    \"last_trade\": \"0.00474999\",    \"high\": \"0.00475928\",    \"low\": \"0.00450474\",    \"avg\": \"0.00470854\",    \"vol\": \"6902.10260251\",    \"vol_curr\": \"32.78491834\",    \"updated\": 1563253263  },  \"DAI_BTC\": {    \"buy_price\": \"0.00008884\",    \"sell_price\": \"0.00009227\",    \"last_trade\": \"0.00009\",    \"high\": \"0.00009746\",    \"low\": \"0.00008798\",    \"avg\": \"0.00009143\",    \"vol\": \"32325.58730048\",    \"vol_curr\": \"2.90930285\",    \"updated\": 1563253276  },  \"DAI_ETH\": {    \"buy_price\": \"0.00418315\",    \"sell_price\": \"0.00427\",    \"last_trade\": \"0.00419882\",    \"high\": \"0.00441041\",    \"low\": \"0.0040654\",    \"avg\": \"0.00423553\",    \"vol\": \"24157.38780371\",    \"vol_curr\": \"101.43252305\",    \"updated\": 1563253270  },  \"DAI_USD\": {    \"buy_price\": \"0.98\",    \"sell_price\": \"0.98882999\",    \"last_trade\": \"0.98289341\",    \"high\": \"1\",    \"low\": \"0.97080001\",    \"avg\": \"0.98435781\",    \"vol\": \"19610.78440903\",    \"vol_curr\": \"19275.31076057\",    \"updated\": 1563253271  },  \"DAI_RUB\": {    \"buy_price\": \"61.37409302\",    \"sell_price\": \"62\",    \"last_trade\": \"61.6723\",    \"high\": \"63.45\",    \"low\": \"61.192921\",    \"avg\": \"62.37284851\",    \"vol\": \"22536.12706678\",    \"vol_curr\": \"1395574.30059767\",    \"updated\": 1563253280  },  \"MKR_BTC\": {    \"buy_price\": \"0.055\",    \"sell_price\": \"0.05522143\",    \"last_trade\": \"0.05519697\",    \"high\": \"0.05704476\",    \"low\": \"0.055\",    \"avg\": \"0.05580101\",    \"vol\": \"24.33492591\",    \"vol_curr\": \"1.34321417\",    \"updated\": 1563253267  },  \"MKR_DAI\": {    \"buy_price\": \"587.84922836\",    \"sell_price\": \"641.88\",    \"last_trade\": \"617.55589\",    \"high\": \"642.3347\",    \"low\": \"583.36733\",    \"avg\": \"612.27935727\",    \"vol\": \"26.92243216\",    \"vol_curr\": \"16626.10655721\",    \"updated\": 1563253179  },  \"QTUM_BTC\": {    \"buy_price\": \"0.00029139\",    \"sell_price\": \"0.00029362\",    \"last_trade\": \"0.00029189\",    \"high\": \"0.00031328\",    \"low\": \"0.00028786\",    \"avg\": \"0.00029543\",    \"vol\": \"8480.47943784\",    \"vol_curr\": \"2.47536714\",    \"updated\": 1563253278  },  \"QTUM_ETH\": {    \"buy_price\": \"0.013698\",    \"sell_price\": \"0.0138389\",    \"last_trade\": \"0.01377852\",    \"high\": \"0.01436913\",    \"low\": \"0.01329118\",    \"avg\": \"0.01367182\",    \"vol\": \"6512.67889097\",    \"vol_curr\": \"89.73507635\",    \"updated\": 1563253256  },  \"QTUM_USD\": {    \"buy_price\": \"3.16339218\",    \"sell_price\": \"3.2122246\",    \"last_trade\": \"3.1960147\",    \"high\": \"3.44962631\",    \"low\": \"2.99074585\",    \"avg\": \"3.16954568\",    \"vol\": \"8290.79397567\",    \"vol_curr\": \"26497.49942091\",    \"updated\": 1563253281  },  \"HB_BTC\": {    \"buy_price\": \"0.00000048\",    \"sell_price\": \"0.00000051\",    \"last_trade\": \"0.00000048\",    \"high\": \"0.0000005\",    \"low\": \"0.00000048\",    \"avg\": \"0.00000048\",    \"vol\": \"14518.57153366\",    \"vol_curr\": \"0.00696891\",    \"updated\": 1563253281  },  \"SMART_BTC\": {    \"buy_price\": \"0.00000056\",    \"sell_price\": \"0.00000057\",    \"last_trade\": \"0.00000057\",    \"high\": \"0.00000064\",    \"low\": \"0.00000055\",    \"avg\": \"0.00000059\",    \"vol\": \"724928.71010098\",    \"vol_curr\": \"0.41320936\",    \"updated\": 1563253277  },  \"SMART_USD\": {    \"buy_price\": \"0.00620229\",    \"sell_price\": \"0.00643299\",    \"last_trade\": \"0.00633146\",    \"high\": \"0.00650722\",    \"low\": \"0.00610005\",    \"avg\": \"0.00629207\",    \"vol\": \"1734514.04531627\",    \"vol_curr\": \"10982.00629735\",    \"updated\": 1563253265  },  \"SMART_EUR\": {    \"buy_price\": \"0.005946\",    \"sell_price\": \"0.00630273\",    \"last_trade\": \"0.00600595\",    \"high\": \"0.00665658\",    \"low\": \"0.00573002\",    \"avg\": \"0.0062016\",    \"vol\": \"1628028.863283\",    \"vol_curr\": \"9777.85995143\",    \"updated\": 1563253278  },  \"SMART_RUB\": {    \"buy_price\": \"0.39522863\",    \"sell_price\": \"0.40750071\",    \"last_trade\": \"0.40373281\",    \"high\": \"0.43581124\",    \"low\": \"0.39000003\",    \"avg\": \"0.40406996\",    \"vol\": \"1998601.52979945\",    \"vol_curr\": \"806901.01169623\",    \"updated\": 1563253256  },  \"XEM_BTC\": {    \"buy_price\": \"0.00000648\",    \"sell_price\": \"0.00000652\",    \"last_trade\": \"0.0000065\",    \"high\": \"0.00000675\",    \"low\": \"0.00000595\",    \"avg\": \"0.00000626\",    \"vol\": \"609963.14364499\",    \"vol_curr\": \"3.96476043\",    \"updated\": 1563253273  },  \"XEM_USD\": {    \"buy_price\": \"0.07038481\",    \"sell_price\": \"0.07085713\",    \"last_trade\": \"0.0707523\",    \"high\": \"0.07448192\",    \"low\": \"0.06012907\",    \"avg\": \"0.06701544\",    \"vol\": \"318110.85521383\",    \"vol_curr\": \"22507.07466134\",    \"updated\": 1563253260  },  \"XEM_EUR\": {    \"buy_price\": \"0.06749464\",    \"sell_price\": \"0.06868996\",    \"last_trade\": \"0.06785818\",    \"high\": \"0.07197482\",    \"low\": \"0.05633706\",    \"avg\": \"0.06426488\",    \"vol\": \"282042.66062126\",    \"vol_curr\": \"19138.90163211\",    \"updated\": 1563253278  },  \"GUSD_BTC\": {    \"buy_price\": \"0.00008971\",    \"sell_price\": \"0.00009274\",    \"last_trade\": \"0.00009156\",    \"high\": \"0.00009895\",    \"low\": \"0.00008854\",    \"avg\": \"0.0000933\",    \"vol\": \"3392.20311679\",    \"vol_curr\": \"0.31059011\",    \"updated\": 1563253280  },  \"GUSD_USD\": {    \"buy_price\": \"0.99400001\",    \"sell_price\": \"1.00890999\",    \"last_trade\": \"0.99280659\",    \"high\": \"1.009\",    \"low\": \"0.99\",    \"avg\": \"0.99702013\",    \"vol\": \"3304.85863794\",    \"vol_curr\": \"3281.08543477\",    \"updated\": 1563253279  },  \"GUSD_RUB\": {    \"buy_price\": \"63.17084276\",    \"sell_price\": \"64.58056677\",    \"last_trade\": \"64.39891\",    \"high\": \"64.88570051\",    \"low\": \"62.658086\",    \"avg\": \"63.6868259\",    \"vol\": \"1929.97519586\",    \"vol_curr\": \"124209.75088048\",    \"updated\": 1563253281  },  \"LSK_BTC\": {    \"buy_price\": \"0.00011516\",    \"sell_price\": \"0.00011624\",    \"last_trade\": \"0.00011573\",    \"high\": \"0.00012438\",    \"low\": \"0.00011529\",    \"avg\": \"0.00011908\",    \"vol\": \"17151.02030277\",    \"vol_curr\": \"1.98488757\",    \"updated\": 1563253268  },  \"LSK_USD\": {    \"buy_price\": \"1.26\",    \"sell_price\": \"1.27085244\",    \"last_trade\": \"1.2624432\",    \"high\": \"1.330925\",    \"low\": \"1.2316202\",    \"avg\": \"1.27471023\",    \"vol\": \"10794.93019124\",    \"vol_curr\": \"13627.98621441\",    \"updated\": 1563253266  },  \"LSK_RUB\": {    \"buy_price\": \"79.3605154\",    \"sell_price\": \"80.66824352\",    \"last_trade\": \"80.039026\",    \"high\": \"86.054957\",    \"low\": \"79.29567\",    \"avg\": \"81.59424259\",    \"vol\": \"10392.83693961\",    \"vol_curr\": \"831832.54602364\",    \"updated\": 1563253281  },  \"NEO_BTC\": {    \"buy_price\": \"0.00112261\",    \"sell_price\": \"0.00112843\",    \"last_trade\": \"0.00112649\",    \"high\": \"0.00116655\",    \"low\": \"0.00111261\",    \"avg\": \"0.00114249\",    \"vol\": \"7424.71988429\",    \"vol_curr\": \"8.3638727\",    \"updated\": 1563253279  },  \"NEO_USD\": {    \"buy_price\": \"12.13450576\",    \"sell_price\": \"12.28569518\",    \"last_trade\": \"12.156151\",    \"high\": \"12.87999996\",    \"low\": \"11.690943\",    \"avg\": \"12.22721552\",    \"vol\": \"6048.86055881\",    \"vol_curr\": \"73530.86233092\",    \"updated\": 1563253280  },  \"NEO_RUB\": {    \"buy_price\": \"775\",    \"sell_price\": \"781.65940262\",    \"last_trade\": \"776.34347\",    \"high\": \"860\",    \"low\": \"750.89816\",    \"avg\": \"784.09997262\",    \"vol\": \"2817.20240478\",    \"vol_curr\": \"2187116.6906253\",    \"updated\": 1563253281  },  \"ADA_BTC\": {    \"buy_price\": \"0.0000055\",    \"sell_price\": \"0.00000554\",    \"last_trade\": \"0.00000551\",    \"high\": \"0.00000568\",    \"low\": \"0.00000541\",    \"avg\": \"0.00000554\",    \"vol\": \"1062624.34559662\",    \"vol_curr\": \"5.85506014\",    \"updated\": 1563253158  },  \"ADA_USD\": {    \"buy_price\": \"0.05976384\",    \"sell_price\": \"0.06005187\",    \"last_trade\": \"0.05998478\",    \"high\": \"0.06153392\",    \"low\": \"0.05642998\",    \"avg\": \"0.05956925\",    \"vol\": \"1369779.30373123\",    \"vol_curr\": \"82165.91018287\",    \"updated\": 1563253278  },  \"ADA_ETH\": {    \"buy_price\": \"0.00025914\",    \"sell_price\": \"0.00026093\",    \"last_trade\": \"0.0002577\",    \"high\": \"0.0002606\",    \"low\": \"0.00025098\",    \"avg\": \"0.00025708\",    \"vol\": \"373379.35434356\",    \"vol_curr\": \"96.21985961\",    \"updated\": 1563253277  },  \"ZRX_BTC\": {    \"buy_price\": \"0.00002409\",    \"sell_price\": \"0.0000243\",    \"last_trade\": \"0.00002416\",    \"high\": \"0.0000255\",    \"low\": \"0.00002101\",    \"avg\": \"0.00002248\",    \"vol\": \"78506.30022231\",    \"vol_curr\": \"1.89671221\",    \"updated\": 1563253280  },  \"ZRX_ETH\": {    \"buy_price\": \"0.0011356\",    \"sell_price\": \"0.00114537\",    \"last_trade\": \"0.00113764\",    \"high\": \"0.00118788\",    \"low\": \"0.00096605\",    \"avg\": \"0.00104062\",    \"vol\": \"52914.06223405\",    \"vol_curr\": \"60.19715375\",    \"updated\": 1563253279  },  \"GNT_BTC\": {    \"buy_price\": \"0.00000574\",    \"sell_price\": \"0.00000583\",    \"last_trade\": \"0.00000578\",    \"high\": \"0.00000596\",    \"low\": \"0.00000562\",    \"avg\": \"0.00000582\",    \"vol\": \"15670.15134611\",    \"vol_curr\": \"0.09057347\",    \"updated\": 1563253238  },  \"GNT_ETH\": {    \"buy_price\": \"0.00027072\",    \"sell_price\": \"0.00027638\",    \"last_trade\": \"0.00027148\",    \"high\": \"0.00027683\",    \"low\": \"0.00026604\",    \"avg\": \"0.00026981\",    \"vol\": \"8044.46922498\",    \"vol_curr\": \"2.1839125\",    \"updated\": 1563253278  },  \"TRX_BTC\": {    \"buy_price\": \"0.00000233\",    \"sell_price\": \"0.00000234\",    \"last_trade\": \"0.00000233\",    \"high\": \"0.00000245\",    \"low\": \"0.0000023\",    \"avg\": \"0.00000237\",    \"vol\": \"3333945.44388611\",    \"vol_curr\": \"7.76809288\",    \"updated\": 1563253279  },  \"TRX_USD\": {    \"buy_price\": \"0.02527\",    \"sell_price\": \"0.02566796\",    \"last_trade\": \"0.02538029\",    \"high\": \"0.02653181\",    \"low\": \"0.02450003\",    \"avg\": \"0.02539374\",    \"vol\": \"4433570.29845488\",    \"vol_curr\": \"112525.29991017\",    \"updated\": 1563253281  },  \"TRX_RUB\": {    \"buy_price\": \"1.6065711\",    \"sell_price\": \"1.6298598\",    \"last_trade\": \"1.6272749\",    \"high\": \"1.68137551\",    \"low\": \"1.59\",    \"avg\": \"1.62734447\",    \"vol\": \"3018369.5272107\",    \"vol_curr\": \"4911716.97055484\",    \"updated\": 1563253281  },  \"GAS_BTC\": {    \"buy_price\": \"0.0002083\",    \"sell_price\": \"0.00021274\",    \"last_trade\": \"0.00020978\",    \"high\": \"0.00023424\",    \"low\": \"0.0002076\",    \"avg\": \"0.00021696\",    \"vol\": \"10198.14331093\",    \"vol_curr\": \"2.1393665\",    \"updated\": 1563253245  },  \"GAS_USD\": {    \"buy_price\": \"2.28\",    \"sell_price\": \"2.36\",    \"last_trade\": \"2.3186144\",    \"high\": \"2.5384728\",    \"low\": \"2.2555\",    \"avg\": \"2.35963893\",    \"vol\": \"8668.80752826\",    \"vol_curr\": \"20099.62196587\",    \"updated\": 1563253281  },  \"INK_BTC\": {    \"buy_price\": \"0.0000006\",    \"sell_price\": \"0.00000065\",    \"last_trade\": \"0.00000059\",    \"high\": \"0.00000068\",    \"low\": \"0.00000059\",    \"avg\": \"0.00000065\",    \"vol\": \"15681.96318939\",    \"vol_curr\": \"0.00925235\",    \"updated\": 1563253266  },  \"INK_ETH\": {    \"buy_price\": \"0.00002603\",    \"sell_price\": \"0.00002949\",    \"last_trade\": \"0.000026\",    \"high\": \"0.00003175\",    \"low\": \"0.000026\",    \"avg\": \"0.00002853\",    \"vol\": \"9100.13127704\",    \"vol_curr\": \"0.23660341\",    \"updated\": 1563253227  },  \"INK_USD\": {    \"buy_price\": \"0.00643636\",    \"sell_price\": \"0.00663407\",    \"last_trade\": \"0.0067\",    \"high\": \"0.0071\",    \"low\": \"0.0061001\",    \"avg\": \"0.00665577\",    \"vol\": \"46848.82865829\",    \"vol_curr\": \"313.88715201\",    \"updated\": 1563253079  },  \"MNX_BTC\": {    \"buy_price\": \"0.00000312\",    \"sell_price\": \"0.00000314\",    \"last_trade\": \"0.00000313\",    \"high\": \"0.00000357\",    \"low\": \"0.00000306\",    \"avg\": \"0.00000321\",    \"vol\": \"213589.46758206\",    \"vol_curr\": \"0.66853503\",    \"updated\": 1563253278  },  \"MNX_ETH\": {    \"buy_price\": \"0.00014576\",    \"sell_price\": \"0.00014985\",    \"last_trade\": \"0.00014892\",    \"high\": \"0.00016199\",    \"low\": \"0.000145\",    \"avg\": \"0.00015128\",    \"vol\": \"307222.65967497\",    \"vol_curr\": \"45.75159847\",    \"updated\": 1563253280  },  \"MNX_USD\": {    \"buy_price\": \"0.03393301\",    \"sell_price\": \"0.034\",    \"last_trade\": \"0.03465204\",    \"high\": \"0.03698552\",    \"low\": \"0.03250001\",    \"avg\": \"0.03472884\",    \"vol\": \"311172.04199532\",    \"vol_curr\": \"10782.7460461\",    \"updated\": 1563253270  },  \"OMG_BTC\": {    \"buy_price\": \"0.00014351\",    \"sell_price\": \"0.00014535\",    \"last_trade\": \"0.0001446\",    \"high\": \"0.00015157\",    \"low\": \"0.00014272\",    \"avg\": \"0.00014748\",    \"vol\": \"21684.77720381\",    \"vol_curr\": \"3.13561878\",    \"updated\": 1563253262  },  \"OMG_ETH\": {    \"buy_price\": \"0.00676535\",    \"sell_price\": \"0.0068389\",    \"last_trade\": \"0.00680883\",    \"high\": \"0.00696647\",    \"low\": \"0.00665353\",    \"avg\": \"0.00682938\",    \"vol\": \"12078.81225686\",    \"vol_curr\": \"82.24257925\",    \"updated\": 1563253278  },  \"OMG_USD\": {    \"buy_price\": \"1.56117519\",    \"sell_price\": \"1.56999999\",    \"last_trade\": \"1.5629262\",    \"high\": \"1.67319999\",    \"low\": \"1.4729\",    \"avg\": \"1.57451791\",    \"vol\": \"13443.21226167\",    \"vol_curr\": \"21010.74865593\",    \"updated\": 1563253278  },  \"XLM_BTC\": {    \"buy_price\": \"0.000008\",    \"sell_price\": \"0.00000804\",    \"last_trade\": \"0.00000802\",    \"high\": \"0.00000846\",    \"low\": \"0.00000783\",    \"avg\": \"0.00000815\",    \"vol\": \"773986.36750963\",    \"vol_curr\": \"6.20737066\",    \"updated\": 1563253266  },  \"XLM_USD\": {    \"buy_price\": \"0.08667156\",    \"sell_price\": \"0.08716484\",    \"last_trade\": \"0.08705222\",    \"high\": \"0.08916467\",    \"low\": \"0.08426159\",    \"avg\": \"0.08703824\",    \"vol\": \"879739.4660424\",    \"vol_curr\": \"76583.2735406\",    \"updated\": 1563253281  },  \"XLM_RUB\": {    \"buy_price\": \"5.50685651\",    \"sell_price\": \"5.53596444\",    \"last_trade\": \"5.5298349\",    \"high\": \"5.68499395\",    \"low\": \"5.405957\",    \"avg\": \"5.56466835\",    \"vol\": \"399198.91716158\",    \"vol_curr\": \"2207504.10416234\",    \"updated\": 1563253267  },  \"EOS_BTC\": {    \"buy_price\": \"0.00039598\",    \"sell_price\": \"0.00039792\",    \"last_trade\": \"0.00039697\",    \"high\": \"0.00041932\",    \"low\": \"0.00039398\",    \"avg\": \"0.00040693\",    \"vol\": \"37206.44849908\",    \"vol_curr\": \"14.76984386\",    \"updated\": 1563253280  },  \"EOS_USD\": {    \"buy_price\": \"4.30554622\",    \"sell_price\": \"4.35745274\",    \"last_trade\": \"4.3355226\",    \"high\": \"4.48635984\",    \"low\": \"4.2043268\",    \"avg\": \"4.35514774\",    \"vol\": \"34540.27785214\",    \"vol_curr\": \"149750.15523826\",    \"updated\": 1563253281  },  \"STQ_BTC\": {    \"buy_price\": \"0.00000001\",    \"sell_price\": \"0.00000002\",    \"last_trade\": \"0.00000002\",    \"high\": \"0.00000002\",    \"low\": \"0.00000002\",    \"avg\": \"0.00000002\",    \"vol\": \"727894\",    \"vol_curr\": \"0.01455788\",    \"updated\": 1555397131  },  \"STQ_USD\": {    \"buy_price\": \"0.00012008\",    \"sell_price\": \"0.00012393\",    \"last_trade\": \"0.00012404\",    \"high\": \"0.00012404\",    \"low\": \"0.00012004\",    \"avg\": \"0.00012085\",    \"vol\": \"113346.24135071\",    \"vol_curr\": \"14.05946777\",    \"updated\": 1563253280  },  \"STQ_EUR\": {    \"buy_price\": \"0.00011581\",    \"sell_price\": \"0.00012652\",    \"last_trade\": \"0.0001157\",    \"high\": \"0.000116\",    \"low\": \"0.0001157\",    \"avg\": \"0.00011585\",    \"vol\": \"10897.36054546\",    \"vol_curr\": \"1.26082461\",    \"updated\": 1563253277  },  \"STQ_RUB\": {    \"buy_price\": \"0.0078623\",    \"sell_price\": \"0.008\",    \"last_trade\": \"0.008\",    \"high\": \"0.00819\",    \"low\": \"0.0078\",    \"avg\": \"0.00794967\",    \"vol\": \"988546.80196596\",    \"vol_curr\": \"7908.37441572\",    \"updated\": 1563253223  },  \"BTG_BTC\": {    \"buy_price\": \"0.0025536\",    \"sell_price\": \"0.0025663\",    \"last_trade\": \"0.00255837\",    \"high\": \"0.00272189\",    \"low\": \"0.0025381\",    \"avg\": \"0.00261502\",    \"vol\": \"3664.76384731\",    \"vol_curr\": \"9.37582188\",    \"updated\": 1563253278  },  \"BTG_USD\": {    \"buy_price\": \"27.76414852\",    \"sell_price\": \"27.99999997\",    \"last_trade\": \"27.894971\",    \"high\": \"28.681769\",    \"low\": \"26.75\",    \"avg\": \"27.96391806\",    \"vol\": \"6578.92960266\",    \"vol_curr\": \"183519.05047729\",    \"updated\": 1563253278  },  \"HBZ_BTC\": {    \"buy_price\": \"0.00000003\",    \"sell_price\": \"0.00000004\",    \"last_trade\": \"0.00000004\",    \"high\": \"0.00000004\",    \"low\": \"0.00000003\",    \"avg\": \"0.00000003\",    \"vol\": \"590308.31882012\",    \"vol_curr\": \"0.02361233\",    \"updated\": 1563253271  },  \"HBZ_ETH\": {    \"buy_price\": \"0.00000172\",    \"sell_price\": \"0.00000188\",    \"last_trade\": \"0.00000188\",    \"high\": \"0.00000191\",    \"low\": \"0.00000166\",    \"avg\": \"0.00000181\",    \"vol\": \"3585570.50688673\",    \"vol_curr\": \"6.74087255\",    \"updated\": 1563253262  },  \"HBZ_USD\": {    \"buy_price\": \"0.000402\",    \"sell_price\": \"0.00043399\",    \"last_trade\": \"0.00040101\",    \"high\": \"0.00043599\",    \"low\": \"0.00035\",    \"avg\": \"0.00039397\",    \"vol\": \"5496729.24401904\",    \"vol_curr\": \"2204.24339414\",    \"updated\": 1563253267  },  \"DXT_BTC\": {    \"buy_price\": \"0.00000036\",    \"sell_price\": \"0.00000038\",    \"last_trade\": \"0.00000037\",    \"high\": \"0.0000004\",    \"low\": \"0.00000037\",    \"avg\": \"0.00000037\",    \"vol\": \"117116.33262044\",    \"vol_curr\": \"0.04333304\",    \"updated\": 1563253230  },  \"DXT_USD\": {    \"buy_price\": \"0.004\",    \"sell_price\": \"0.00412494\",    \"last_trade\": \"0.00409683\",    \"high\": \"0.00412446\",    \"low\": \"0.00380142\",    \"avg\": \"0.00402624\",    \"vol\": \"578422.86111328\",    \"vol_curr\": \"2369.70013009\",    \"updated\": 1563253155  },  \"BTCZ_BTC\": {    \"buy_price\": \"0.00000002\",    \"sell_price\": \"0.00000003\",    \"last_trade\": \"0.00000003\",    \"high\": \"0.00000003\",    \"low\": \"0.00000002\",    \"avg\": \"0.00000002\",    \"vol\": \"595508.66666665\",    \"vol_curr\": \"0.01786525\",    \"updated\": 1563253278  },  \"BCH_BTC\": {    \"buy_price\": \"0.02879775\",    \"sell_price\": \"0.02889999\",    \"last_trade\": \"0.02889033\",    \"high\": \"0.03065\",    \"low\": \"0.02764601\",    \"avg\": \"0.02895293\",    \"vol\": \"1368.81507865\",    \"vol_curr\": \"39.54551933\",    \"updated\": 1563253281  },  \"BCH_USD\": {    \"buy_price\": \"314.36385926\",    \"sell_price\": \"315.12335725\",    \"last_trade\": \"314.9352\",    \"high\": \"330\",    \"low\": \"283.41933\",    \"avg\": \"310.56737406\",    \"vol\": \"2005.95209277\",    \"vol_curr\": \"631744.9235292\",    \"updated\": 1563253281  },  \"BCH_RUB\": {    \"buy_price\": \"19877.5488609\",    \"sell_price\": \"20016.93520121\",    \"last_trade\": \"19925.843\",    \"high\": \"20980.5790075\",    \"low\": \"18227.14908451\",    \"avg\": \"19733.0347478\",    \"vol\": \"579.2294333\",    \"vol_curr\": \"11541634.74909331\",    \"updated\": 1563253279  },  \"BCH_ETH\": {    \"buy_price\": \"1.35644644\",    \"sell_price\": \"1.363\",    \"last_trade\": \"1.3599652\",    \"high\": \"1.39891146\",    \"low\": \"1.26552065\",    \"avg\": \"1.34655347\",    \"vol\": \"445.15985802\",    \"vol_curr\": \"605.40191535\",    \"updated\": 1563253278  },  \"DASH_BTC\": {    \"buy_price\": \"0.011232\",    \"sell_price\": \"0.01128711\",    \"last_trade\": \"0.01124797\",    \"high\": \"0.01182\",    \"low\": \"0.01097563\",    \"avg\": \"0.01144952\",    \"vol\": \"1138.37227653\",    \"vol_curr\": \"12.80437721\",    \"updated\": 1563253278  },  \"DASH_USD\": {    \"buy_price\": \"121.8827839\",    \"sell_price\": \"122.2261\",    \"last_trade\": \"122.04304\",    \"high\": \"126.04376384\",    \"low\": \"118.41244249\",    \"avg\": \"122.50312639\",    \"vol\": \"628.40443479\",    \"vol_curr\": \"76692.38757172\",    \"updated\": 1563253269  },  \"DASH_RUB\": {    \"buy_price\": \"7773.2375788\",    \"sell_price\": \"7794.99484902\",    \"last_trade\": \"7786.7236\",    \"high\": \"8106.1826766\",    \"low\": \"7569.48750815\",    \"avg\": \"7830.16645862\",    \"vol\": \"353.54856329\",    \"vol_curr\": \"2752984.94158294\",    \"updated\": 1563253281  },  \"ETH_BTC\": {    \"buy_price\": \"0.021221\",    \"sell_price\": \"0.02128593\",    \"last_trade\": \"0.02126969\",    \"high\": \"0.02205669\",    \"low\": \"0.02104402\",    \"avg\": \"0.02161293\",    \"vol\": \"3798.36749838\",    \"vol_curr\": \"80.79009919\",    \"updated\": 1563253281  },  \"ETH_LTC\": {    \"buy_price\": \"2.54573998\",    \"sell_price\": \"2.55937879\",    \"last_trade\": \"2.5472117\",    \"high\": \"2.57796756\",    \"low\": \"2.48942726\",    \"avg\": \"2.53872194\",    \"vol\": \"565.26980395\",    \"vol_curr\": \"1439.86185828\",    \"updated\": 1563253213  },  \"ETH_USD\": {    \"buy_price\": \"231.29\",    \"sell_price\": \"231.39319197\",    \"last_trade\": \"231.35939\",    \"high\": \"238.99999999\",    \"low\": \"221.68222214\",    \"avg\": \"230.51403509\",    \"vol\": \"6541.90285391\",    \"vol_curr\": \"1513530.6537204\",    \"updated\": 1563253281  },  \"ETH_EUR\": {    \"buy_price\": \"221.7994784\",    \"sell_price\": \"223.04306362\",    \"last_trade\": \"222.47559\",    \"high\": \"227.769483\",    \"low\": \"213.42341074\",    \"avg\": \"221.72647104\",    \"vol\": \"900.83541951\",    \"vol_curr\": \"200413.8914503\",    \"updated\": 1563253280  },  \"ETH_RUB\": {    \"buy_price\": \"14634.22037688\",    \"sell_price\": \"14684\",    \"last_trade\": \"14643.769\",    \"high\": \"15181.79752504\",    \"low\": \"14264.19390234\",    \"avg\": \"14713.1159808\",    \"vol\": \"2211.42286229\",    \"vol_curr\": \"32383565.55669385\",    \"updated\": 1563253280  },  \"ETH_UAH\": {    \"buy_price\": \"6001.49360157\",    \"sell_price\": \"6043.57756559\",    \"last_trade\": \"6016.8829\",    \"high\": \"6255.92858383\",    \"low\": \"5738.91962196\",    \"avg\": \"6039.08756333\",    \"vol\": \"771.10769222\",    \"vol_curr\": \"4639664.68742463\",    \"updated\": 1563253277  },  \"ETH_PLN\": {    \"buy_price\": \"847.00000163\",    \"sell_price\": \"893.60999985\",    \"last_trade\": \"865.91947\",    \"high\": \"906.00023\",    \"low\": \"822.34696\",    \"avg\": \"874.23085488\",    \"vol\": \"164.81893595\",    \"vol_curr\": \"142719.92566848\",    \"updated\": 1563253270  },  \"ETC_BTC\": {    \"buy_price\": \"0.00053388\",    \"sell_price\": \"0.00053711\",    \"last_trade\": \"0.00053388\",    \"high\": \"0.00054897\",    \"low\": \"0.00051923\",    \"avg\": \"0.00053444\",    \"vol\": \"15983.49326334\",    \"vol_curr\": \"8.53326738\",    \"updated\": 1563253276  },  \"ETC_USD\": {    \"buy_price\": \"5.78961208\",    \"sell_price\": \"5.82\",    \"last_trade\": \"5.8080334\",    \"high\": \"5.95671618\",    \"low\": \"5.4162586\",    \"avg\": \"5.71179772\",    \"vol\": \"22316.81680218\",    \"vol_curr\": \"129616.81736879\",    \"updated\": 1563253281  },  \"ETC_RUB\": {    \"buy_price\": \"367.12435767\",    \"sell_price\": \"370.98886917\",    \"last_trade\": \"368.74364\",    \"high\": \"379.98999999\",    \"low\": \"345.79958398\",    \"avg\": \"364.17382881\",    \"vol\": \"11947.34932062\",    \"vol_curr\": \"4405509.07683924\",    \"updated\": 1563253280  },  \"LTC_BTC\": {    \"buy_price\": \"0.00830251\",    \"sell_price\": \"0.00834049\",    \"last_trade\": \"0.00832481\",    \"high\": \"0.00874705\",    \"low\": \"0.0082338\",    \"avg\": \"0.00851038\",    \"vol\": \"5863.35215514\",    \"vol_curr\": \"48.81129265\",    \"updated\": 1563253277  },  \"LTC_USD\": {    \"buy_price\": \"90.03188154\",    \"sell_price\": \"90.58676758\",    \"last_trade\": \"90.136134\",    \"high\": \"94.20394118\",    \"low\": \"87\",    \"avg\": \"90.97236841\",    \"vol\": \"7238.39354751\",    \"vol_curr\": \"652440.81074354\",    \"updated\": 1563253281  },  \"LTC_EUR\": {    \"buy_price\": \"87.11953544\",    \"sell_price\": \"87.36338279\",    \"last_trade\": \"87.217196\",    \"high\": \"89.33960834\",    \"low\": \"84.12733087\",    \"avg\": \"87.09094787\",    \"vol\": \"2158.65109396\",    \"vol_curr\": \"188271.49555767\",    \"updated\": 1563253274  },  \"LTC_RUB\": {    \"buy_price\": \"5715.58003181\",    \"sell_price\": \"5755.08356902\",    \"last_trade\": \"5730.7754\",    \"high\": \"6000\",    \"low\": \"5606.83296587\",    \"avg\": \"5801.30881071\",    \"vol\": \"2688.14626966\",    \"vol_curr\": \"15405162.51378536\",    \"updated\": 1563253280  },  \"ZEC_BTC\": {    \"buy_price\": \"0.0073705\",    \"sell_price\": \"0.0074077\",    \"last_trade\": \"0.00737929\",    \"high\": \"0.0077219\",    \"low\": \"0.0072735\",    \"avg\": \"0.00747028\",    \"vol\": \"2949.69390988\",    \"vol_curr\": \"21.76664677\",    \"updated\": 1563252567  },  \"ZEC_USD\": {    \"buy_price\": \"80.28372383\",    \"sell_price\": \"80.50803515\",    \"last_trade\": \"80.478294\",    \"high\": \"82.74445869\",    \"low\": \"77.847805\",    \"avg\": \"79.84613956\",    \"vol\": \"3128.24842334\",    \"vol_curr\": \"251756.09631931\",    \"updated\": 1563253279  },  \"ZEC_EUR\": {    \"buy_price\": \"77.00956404\",    \"sell_price\": \"77.47003572\",    \"last_trade\": \"77.148345\",    \"high\": \"79.466556\",    \"low\": \"74.856346\",    \"avg\": \"76.6277108\",    \"vol\": \"756.89805144\",    \"vol_curr\": \"58393.4320027\",    \"updated\": 1563253275  },  \"ZEC_RUB\": {    \"buy_price\": \"5095.9\",    \"sell_price\": \"5102.26101451\",    \"last_trade\": \"5100.9904\",    \"high\": \"5261.3210588\",    \"low\": \"4982.43872678\",    \"avg\": \"5099.75157823\",    \"vol\": \"1586.49296061\",    \"vol_curr\": \"8092685.3617609\",    \"updated\": 1563253272  },  \"XRP_BTC\": {    \"buy_price\": \"0.00002911\",    \"sell_price\": \"0.00002924\",    \"last_trade\": \"0.00002917\",    \"high\": \"0.0000309\",    \"low\": \"0.00002857\",    \"avg\": \"0.0000298\",    \"vol\": \"1609654.334624\",    \"vol_curr\": \"46.95361694\",    \"updated\": 1563253279  },  \"XRP_USD\": {    \"buy_price\": \"0.317\",    \"sell_price\": \"0.31767428\",    \"last_trade\": \"0.31712075\",    \"high\": \"0.3264416\",    \"low\": \"0.31174369\",    \"avg\": \"0.31843136\",    \"vol\": \"2101593.68996282\",    \"vol_curr\": \"666458.96715627\",    \"updated\": 1563253096  },  \"XRP_RUB\": {    \"buy_price\": \"20.08294142\",    \"sell_price\": \"20.16037612\",    \"last_trade\": \"20.14397\",    \"high\": \"20.8\",    \"low\": \"19.96819505\",    \"avg\": \"20.3350201\",    \"vol\": \"810393.68129346\",    \"vol_curr\": \"16324546.00416519\",    \"updated\": 1563253280  },  \"XMR_BTC\": {    \"buy_price\": \"0.0079968\",    \"sell_price\": \"0.00804286\",    \"last_trade\": \"0.00803144\",    \"high\": \"0.00826995\",    \"low\": \"0.00751359\",    \"avg\": \"0.00790811\",    \"vol\": \"1109.75588056\",    \"vol_curr\": \"8.89190789\",    \"updated\": 1563253280  },  \"XMR_USD\": {    \"buy_price\": \"86.8421005\",    \"sell_price\": \"87.42818391\",    \"last_trade\": \"87.267327\",    \"high\": \"91.17301939\",    \"low\": \"77.63252167\",    \"avg\": \"84.37889143\",    \"vol\": \"1974.58958211\",    \"vol_curr\": \"172317.15475354\",    \"updated\": 1563253207  },  \"XMR_EUR\": {    \"buy_price\": \"84.00935635\",    \"sell_price\": \"84.50752695\",    \"last_trade\": \"84.348087\",    \"high\": \"87.73010308\",    \"low\": \"74.73649854\",    \"avg\": \"81.00552984\",    \"vol\": \"548.19587349\",    \"vol_curr\": \"46239.27323079\",    \"updated\": 1563253256  },  \"BTC_USDT\": {    \"buy_price\": \"10710\",    \"sell_price\": \"10769.99999998\",    \"last_trade\": \"10741.46\",    \"high\": \"11051.599\",    \"low\": \"10045.51\",    \"avg\": \"10581.60285415\",    \"vol\": \"63.54757821\",    \"vol_curr\": \"682593.76948112\",    \"updated\": 1563253276  },  \"ETH_USDT\": {    \"buy_price\": \"227.36940598\",    \"sell_price\": \"228.39\",    \"last_trade\": \"228.13297\",    \"high\": \"235.45614739\",    \"low\": \"217.92\",    \"avg\": \"227.83203544\",    \"vol\": \"807.55115497\",    \"vol_curr\": \"184229.04341088\",    \"updated\": 1563253276  },  \"USDT_USD\": {    \"buy_price\": \"1.01195993\",    \"sell_price\": \"1.01708148\",    \"last_trade\": \"1.0151933\",    \"high\": \"1.029\",    \"low\": \"1.00912463\",    \"avg\": \"1.01417846\",    \"vol\": \"288510.42118097\",    \"vol_curr\": \"292893.8465631\",    \"updated\": 1563253281  },  \"USDT_RUB\": {    \"buy_price\": \"64.2\",    \"sell_price\": \"64.61107412\",    \"last_trade\": \"64.46409\",    \"high\": \"66.468\",    \"low\": \"63.40510008\",    \"avg\": \"64.78631703\",    \"vol\": \"96072.68405005\",    \"vol_curr\": \"6193238.15114432\",    \"updated\": 1563253281  },  \"USD_RUB\": {    \"buy_price\": \"63.30355134\",    \"sell_price\": \"63.612129\",    \"last_trade\": \"63.450634\",    \"high\": \"64.7\",    \"low\": \"63.22500011\",    \"avg\": \"63.91593005\",    \"vol\": \"411855.05718375\",    \"vol_curr\": \"26132464.49441529\",    \"updated\": 1563253277  },  \"DOGE_BTC\": {    \"buy_price\": \"0.00000028\",    \"sell_price\": \"0.00000029\",    \"last_trade\": \"0.00000028\",    \"high\": \"0.00000029\",    \"low\": \"0.00000028\",    \"avg\": \"0.00000028\",    \"vol\": \"4438161.97822225\",    \"vol_curr\": \"1.24268535\",    \"updated\": 1563253268  },  \"WAVES_BTC\": {    \"buy_price\": \"0.00013956\",    \"sell_price\": \"0.0001401\",    \"last_trade\": \"0.00014001\",    \"high\": \"0.00015161\",    \"low\": \"0.00013049\",    \"avg\": \"0.00013976\",    \"vol\": \"220277.12004154\",    \"vol_curr\": \"30.84099957\",    \"updated\": 1563253275  },  \"WAVES_RUB\": {    \"buy_price\": \"96.35965368\",    \"sell_price\": \"96.68252614\",    \"last_trade\": \"96.402289\",    \"high\": \"102.49999999\",    \"low\": \"89\",    \"avg\": \"94.51004837\",    \"vol\": \"61277.84305722\",    \"vol_curr\": \"5907324.33569886\",    \"updated\": 1563253278  },  \"KICK_BTC\": {    \"buy_price\": \"0.00000012\",    \"sell_price\": \"0.00000013\",    \"last_trade\": \"0.00000012\",    \"high\": \"0.00000016\",    \"low\": \"0.00000011\",    \"avg\": \"0.00000012\",    \"vol\": \"16263474.31220915\",    \"vol_curr\": \"1.95161691\",    \"updated\": 1563253273  },  \"KICK_ETH\": {    \"buy_price\": \"0.0000056\",    \"sell_price\": \"0.00000585\",    \"last_trade\": \"0.00000582\",    \"high\": \"0.00000591\",    \"low\": \"0.00000514\",    \"avg\": \"0.0000056\",    \"vol\": \"10549786.67937868\",    \"vol_curr\": \"61.39975847\",    \"updated\": 1563253280  },  \"KICK_USDT\": {    \"buy_price\": \"0.00132001\",    \"sell_price\": \"0.00136998\",    \"last_trade\": \"0.00135147\",    \"high\": \"0.00136415\",    \"low\": \"0.00115875\",    \"avg\": \"0.00129001\",    \"vol\": \"1757064.91010955\",    \"vol_curr\": \"2374.62051406\",    \"updated\": 1563253191  },  \"EOS_EUR\": {    \"buy_price\": \"4.13088521\",    \"sell_price\": \"4.19398665\",    \"last_trade\": \"4.1374387\",    \"high\": \"4.39777426\",    \"low\": \"3.97366904\",    \"avg\": \"4.17888796\",    \"vol\": \"2517.44575586\",    \"vol_curr\": \"10415.77749545\",    \"updated\": 1563253277  },  \"BCH_EUR\": {    \"buy_price\": \"302.4559935\",    \"sell_price\": \"304.02001346\",    \"last_trade\": \"303.41001\",    \"high\": \"314.63011\",    \"low\": \"272.72952705\",    \"avg\": \"298.10735216\",    \"vol\": \"408.0901002\",    \"vol_curr\": \"123818.62138505\",    \"updated\": 1563253137  },  \"XRP_EUR\": {    \"buy_price\": \"0.3048835\",    \"sell_price\": \"0.30700868\",    \"last_trade\": \"0.30666949\",    \"high\": \"0.3105666\",    \"low\": \"0.29896175\",    \"avg\": \"0.30476626\",    \"vol\": \"156544.6550188\",    \"vol_curr\": \"47947.9308882\",    \"updated\": 1563253281  },  \"XRP_UAH\": {    \"buy_price\": \"8.25440977\",    \"sell_price\": \"8.31228406\",    \"last_trade\": \"8.2674585\",    \"high\": \"8.53999146\",    \"low\": \"8.16259636\",    \"avg\": \"8.33803581\",    \"vol\": \"124276.47447521\",    \"vol_curr\": \"1027450.59525016\",    \"updated\": 1563253267  },  \"XEM_UAH\": {    \"buy_price\": \"1.8379077\",    \"sell_price\": \"1.86582233\",    \"last_trade\": \"1.8530141\",    \"high\": \"2.21523216\",    \"low\": \"1.585581\",    \"avg\": \"1.76851808\",    \"vol\": \"164667.45423706\",    \"vol_curr\": \"305131.11451238\",    \"updated\": 1563253279  },  \"BCH_USDT\": {    \"buy_price\": \"309.22745418\",    \"sell_price\": \"310.34999999\",    \"last_trade\": \"310.59259\",    \"high\": \"324.54458\",    \"low\": \"279.96498\",    \"avg\": \"306.94994768\",    \"vol\": \"490.34415949\",    \"vol_curr\": \"152297.26248848\",    \"updated\": 1563253278  },  \"DASH_USDT\": {    \"buy_price\": \"120.49\",    \"sell_price\": \"121.2032427\",    \"last_trade\": \"120.63797\",    \"high\": \"124.29199727\",    \"low\": \"117.21548\",    \"avg\": \"120.96907989\",    \"vol\": \"265.98335169\",    \"vol_curr\": \"32087.69160249\",    \"updated\": 1563253278  },  \"BCH_UAH\": {    \"buy_price\": \"8168.6077686\",    \"sell_price\": \"8223.41754134\",    \"last_trade\": \"8214.0341\",    \"high\": \"8672.10489708\",    \"low\": \"7425.58251771\",    \"avg\": \"8138.7939422\",    \"vol\": \"137.07452153\",    \"vol_curr\": \"1125934.79410922\",    \"updated\": 1563253245  },  \"XRP_USDT\": {    \"buy_price\": \"0.31235\",    \"sell_price\": \"0.31377\",    \"last_trade\": \"0.31351475\",    \"high\": \"0.3224736\",    \"low\": \"0.30758505\",    \"avg\": \"0.31432394\",    \"vol\": \"395171.87003971\",    \"vol_curr\": \"123892.21004253\",    \"updated\": 1563253279  },  \"USDT_UAH\": {    \"buy_price\": \"26.31613869\",    \"sell_price\": \"26.50067403\",    \"last_trade\": \"26.45344\",    \"high\": \"27.04587287\",    \"low\": \"26.31611237\",    \"avg\": \"26.55778178\",    \"vol\": \"50136.07041102\",    \"vol_curr\": \"1326271.53045374\",    \"updated\": 1563253279  },  \"USDT_EUR\": {    \"buy_price\": \"0.97469071\",    \"sell_price\": \"0.98152535\",    \"last_trade\": \"0.97619766\",    \"high\": \"1.0053\",    \"low\": \"0.960001\",    \"avg\": \"0.97288045\",    \"vol\": \"44809.323076\",    \"vol_curr\": \"43742.75633297\",    \"updated\": 1563253272  },  \"ZRX_USD\": {    \"buy_price\": \"0.25885631\",    \"sell_price\": \"0.26265326\",    \"last_trade\": \"0.26131248\",    \"high\": \"0.27084441\",    \"low\": \"0.21649426\",    \"avg\": \"0.24061234\",    \"vol\": \"41099.99633564\",    \"vol_curr\": \"10739.94197045\",    \"updated\": 1563253281  },  \"BTG_ETH\": {    \"buy_price\": \"0.11995005\",    \"sell_price\": \"0.12114995\",    \"last_trade\": \"0.12078229\",    \"high\": \"0.12490941\",    \"low\": \"0.117\",    \"avg\": \"0.12090011\",    \"vol\": \"2043.87560853\",    \"vol_curr\": \"246.86397647\",    \"updated\": 1563253267  },  \"WAVES_USD\": {    \"buy_price\": \"1.51736824\",    \"sell_price\": \"1.51966553\",    \"last_trade\": \"1.5183052\",    \"high\": \"1.58730321\",    \"low\": \"1.38000001\",    \"avg\": \"1.48037194\",    \"vol\": \"89314.06101773\",    \"vol_curr\": \"135606.00327634\",    \"updated\": 1563253278  },  \"DOGE_USD\": {    \"buy_price\": \"0.003053\",    \"sell_price\": \"0.0030542\",    \"last_trade\": \"0.003039\",    \"high\": \"0.003177\",    \"low\": \"0.00293\",    \"avg\": \"0.00305891\",    \"vol\": \"3561525.98298867\",    \"vol_curr\": \"10823.4774623\",    \"updated\": 1563253281  },  \"XRP_ETH\": {    \"buy_price\": \"0.00136898\",    \"sell_price\": \"0.001377\",    \"last_trade\": \"0.00137213\",    \"high\": \"0.00142561\",    \"low\": \"0.0013462\",    \"avg\": \"0.00137859\",    \"vol\": \"401906.86414788\",    \"vol_curr\": \"551.4684655\",    \"updated\": 1563253279  },  \"DASH_UAH\": {    \"buy_price\": \"3178.47667651\",    \"sell_price\": \"3200.76494958\",    \"last_trade\": \"3184.3612\",    \"high\": \"3300.74033848\",    \"low\": \"3134.34936341\",    \"avg\": \"3207.42691491\",    \"vol\": \"179.74678787\",    \"vol_curr\": \"572378.69714315\",    \"updated\": 1563253278  },  \"XMR_ETH\": {    \"buy_price\": \"0.37644031\",    \"sell_price\": \"0.377054\",    \"last_trade\": \"0.3765658\",    \"high\": \"0.39096881\",    \"low\": \"0.34656565\",    \"avg\": \"0.36766598\",    \"vol\": \"757.06800118\",    \"vol_curr\": \"285.08591751\",    \"updated\": 1563253279  },  \"WAVES_ETH\": {    \"buy_price\": \"0.00655662\",    \"sell_price\": \"0.00658936\",    \"last_trade\": \"0.00657151\",    \"high\": \"0.00693885\",    \"low\": \"0.00605073\",    \"avg\": \"0.00644244\",    \"vol\": \"70768.80525412\",    \"vol_curr\": \"465.05791141\",    \"updated\": 1563253278  },  \"EXM_BTC\": {    \"buy_price\": \"0.00000001\",    \"sell_price\": \"1\",    \"last_trade\": \"0.00000085\",    \"high\": \"0.00000085\",    \"low\": \"0.00000085\",    \"avg\": \"0.00000085\",    \"vol\": \"2\",    \"vol_curr\": \"0.0000017\",    \"updated\": 1562875738  }}";
                        if (!string.IsNullOrEmpty(Response))
                        {
                            EXMOAPIReqRes.EXMOLTPCheckResponse EXMOResponseData = new EXMOAPIReqRes.EXMOLTPCheckResponse();
                            try
                            {
                                Response = Response.Replace(data.Pair, "symbol");
                                //HelperForLog.WriteLogIntoFile("EXMO Handle", "PriceTickerHandler", Response);
                                EXMOResponseData = JsonConvert.DeserializeObject<EXMOAPIReqRes.EXMOLTPCheckResponse>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " EXMO ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (EXMOResponseData != null && EXMOResponseData.symbol.last_trade != null)
                            {
                                data.Price = decimal.Parse(EXMOResponseData.symbol.last_trade);
                            }
                        }
                        break;

                    case (short)enAppType.Gemini:
                        Url = "https://api.gemini.com/v1/pubticker/#Pair#";
                        string[] symbols = data.Pair.Split("_");
                        data.Pair = data.Pair.Replace("_", "");
                        Url = Url.Replace("#Pair#", data.Pair.ToLower());
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            Response = Response.Replace(symbols[0], "C1");
                            Response = Response.Replace(symbols[1], "C2");
                            GeminiLTPResponse GeminiResponseData = new GeminiLTPResponse();
                            try
                            {
                                GeminiResponseData = JsonConvert.DeserializeObject<GeminiLTPResponse>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }
                            if (GeminiResponseData != null && GeminiResponseData.last != null)
                            {
                                //HelperForLog.WriteLogIntoFile("GeminiLTPCheker", "PriceTickerHandler", "Pair:" + data.Pair + " Response:" Response);
                                data.Price = Decimal.Parse(GeminiResponseData.last);
                            }
                        }
                        break;

                    case (short)enAppType.CEXIO:
                        //string symbol1 = data.Pair.Split(data.Pair[]);
                        Url = "https://cex.io/api/last_price/#symbol1#";
                        data.Pair = data.Pair.Contains("-") ? data.Pair.Replace("-", "/") : data.Pair.Contains("_") ? data.Pair.Replace("_", "/") : data.Pair;
                        Url = Url.Replace("#symbol1#", data.Pair);

                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();

                        if (!string.IsNullOrEmpty(Response))
                        {
                            CEXIOTikerReturn CEXIOResponseData = new CEXIOTikerReturn();
                            try
                            {
                                CEXIOResponseData = JsonConvert.DeserializeObject<CEXIOTikerReturn>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (CEXIOResponseData != null && CEXIOResponseData.lprice != null)
                            {
                                data.Price = Decimal.Parse(CEXIOResponseData.lprice);
                            }
                        }
                        break;
                }
                return await Task.FromResult(data);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return await Task.FromResult(data);
            }
        }
    }

    //komal 10-06-2016 for Arbitrage
    public class CryptoWatcherArbitrageHandler : IRequestHandler<CryptoWatcherArbitrageReq>
    {
        private IMemoryCache _cache;
        private readonly IMediator _mediator;
        private readonly ITransactionConfigService _transactionConfigService;
        private readonly IFrontTrnRepository _frontTrnRepository;
        private readonly ISignalRService _iSignalRService;
        private readonly ITrnMasterConfiguration _ITrnMasterConfiguration;
        private readonly IBinanceLPService _binanceLPService;
        private readonly IFrontTrnService _frontTrnService;
        private readonly ICommonRepository<CronMaster> _cronMaster;

        public CryptoWatcherArbitrageHandler(IMemoryCache Cache, IMediator mediator, ITransactionConfigService TransactionConfigService,
            IFrontTrnRepository FrontTrnRepository, ISignalRService iSignalRService, ITrnMasterConfiguration ITrnMasterConfiguration,
            IBinanceLPService binanceLPService, IFrontTrnService frontTrnService,
            ICommonRepository<CronMaster> CronMaster)
        {
            _cache = Cache;
            _mediator = mediator;
            _transactionConfigService = TransactionConfigService;
            _frontTrnRepository = FrontTrnRepository;
            _iSignalRService = iSignalRService;
            _ITrnMasterConfiguration = ITrnMasterConfiguration;
            _binanceLPService = binanceLPService;
            _frontTrnService = frontTrnService;
            _cronMaster = CronMaster;
        }

        public async Task<Unit> Handle(CryptoWatcherArbitrageReq data, CancellationToken cancellationToken)
        {
            ArbitrageBuySellViewModel BuySellmodel;
            ExchangeProviderListArbitrage exchangeProvider;
            LastPriceViewModelArbitrage lastPriceObj;
            CronMaster cronMaster = new CronMaster();
            try
            {
                List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
                if (cronMasterList == null)
                {
                    cronMasterList = _cronMaster.List();
                    _cache.Set("CronMaster", cronMasterList);
                }
                else if (cronMasterList.Count() == 0)
                {
                    cronMasterList = _cronMaster.List();
                    _cache.Set("CronMaster", cronMasterList);
                }
                cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.CryptoWatcherArbitrage).FirstOrDefault();
                //cronMaster = _cronMaster.FindBy(e => e.Id == (short)enCronMaster.CryptoWatcherArbitrage).FirstOrDefault();
                if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
                {
                    List<ConfigureLPArbitrage> symbol = GetPair().ToList();
                    foreach (var LPData in symbol)
                    {
                        ArbitrageLTPCls Req = new ArbitrageLTPCls()
                        {
                            PairID = LPData.PairID,
                            Pair = LPData.Pair.Trim(),
                            LpType = LPData.LPType,
                            ProviderPair = LPData.ProviderPair
                        };
                        var Res = await _mediator.Send(Req);
                        if (Req.Price > 0)
                        {
                            var ResponseFromUpdateLTP = await _frontTrnRepository.UpdateLTPDataArbitrage(Req);
                            //HelperForLog.WriteLogForSocket(" Price Change ", " Price Change 1 ", "ProviderName : " + LPData.ProviderName + " ResponseFromUpdateLTP.Price =" + ResponseFromUpdateLTP.Price + " Req.Price =" + Req.Price);
                            if (ResponseFromUpdateLTP == null)
                            {
                                switch (Req.LpType)
                                {
                                    case (short)enAppType.Binance:
                                        Req.Fees = GetTradeFeesOnBinance(Req.Pair, Req.LpType);
                                        break;
                                }
                                ResponseFromUpdateLTP = _frontTrnRepository.InsertLTPDataArbitrage(Req);
                            }
                            //HelperForLog.WriteLogForSocket(" Price Change ", " Price Change 1 ", "ResponseFromUpdateLTP.Price =" + ResponseFromUpdateLTP.Price + "Req.Price =" + Req.Price);
                            if (ResponseFromUpdateLTP != null)
                            {
                                //if (Db_LTP != LP_LTP)
                                //{
                                //HelperForLog.WriteLogForSocket(" Price Change ", " Price Change 2", "ResponseFromUpdateLTP.Price =" + ResponseFromUpdateLTP.Price + "Req.Price =" + Req.Price);

                                lastPriceObj = new LastPriceViewModelArbitrage();
                                lastPriceObj.LastPrice = ResponseFromUpdateLTP.Price;
                                lastPriceObj.UpDownBit = ResponseFromUpdateLTP.UpDownBit;
                                lastPriceObj.LPType = ResponseFromUpdateLTP.LpType;
                                lastPriceObj.ExchangeName = LPData.ProviderName;
                                _iSignalRService.LastPriceArbitrage(lastPriceObj, Req.Pair, "0");

                                BuySellmodel = new ArbitrageBuySellViewModel();
                                BuySellmodel.LPType = ResponseFromUpdateLTP.LpType;
                                BuySellmodel.LTP = ResponseFromUpdateLTP.Price;
                                BuySellmodel.ProviderName = LPData.ProviderName;
                                BuySellmodel.Fees = ResponseFromUpdateLTP.Fees;
                                _iSignalRService.BuyerBookArbitrage(BuySellmodel, Req.Pair, "0");
                                _iSignalRService.SellerBookArbitrage(BuySellmodel, Req.Pair, "0");

                                exchangeProvider = new ExchangeProviderListArbitrage();
                                exchangeProvider.LPType = ResponseFromUpdateLTP.LpType;
                                exchangeProvider.LTP = ResponseFromUpdateLTP.Price;
                                exchangeProvider.ProviderName = LPData.ProviderName;
                                exchangeProvider.UpDownBit = ResponseFromUpdateLTP.UpDownBit;
                                exchangeProvider.Volume = ResponseFromUpdateLTP.Volume;
                                exchangeProvider.ChangePer = ResponseFromUpdateLTP.ChangePer;
                                _iSignalRService.ProviderMarketDataArbitrage(exchangeProvider, Req.Pair);

                                //Rita 17-6-19 send Profit Indicator data and also smart arbitrage data                           
                                ProfitIndicatorInfo responsedata = _frontTrnService.GetProfitIndicatorArbitrage(Req.PairID, 0);
                                if (responsedata != null)
                                {
                                    //ProfitIndicatorResponse Response = new ProfitIndicatorResponse();
                                    //Response.response = responsedata;
                                    //Response.ReturnCode = enResponseCode.Success;
                                    //Response.ErrorCode = enErrorCode.Success;
                                    //Response.ReturnMsg = "Success";
                                    _iSignalRService.ProfitIndicatorArbitrage(responsedata, Req.Pair);
                                }
                                List<ExchangeListSmartArbitrage> responsedata1 = _frontTrnService.ExchangeListSmartArbitrageService(Req.PairID, Req.Pair, 5, 0);
                                if (responsedata1 != null)
                                {
                                    //ExchangeListSmartArbitrageResponse Response = new ExchangeListSmartArbitrageResponse();
                                    //Response.response = responsedata1;
                                    //Response.ReturnCode = enResponseCode.Success;
                                    //Response.ErrorCode = enErrorCode.Success;
                                    //Response.ReturnMsg = "Success";
                                    _iSignalRService.ExchangeListSmartArbitrage(responsedata1, Req.Pair);
                                }
                                //====================================================================
                                //}
                            }
                        }

                    }
                    //_frontTrnRepository.GetLocalConfigurationDataArbitrage(Convert.ToInt16(enAppType.COINTTRADINGLocal));
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return await Task.FromResult(new Unit());
            }
        }

        public ConfigureLPArbitrage[] GetPair()
        {
            try
            {
                ConfigureLPArbitrage[] symbol = _transactionConfigService.TradePairConfigurationArbitrageV1();
                return symbol;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return null;
            }
        }

        public Decimal GetTradeFeesOnBinance(string Pair, short LPType)
        {
            try
            {
                var PairArray = Pair.Split("_");
                LPKeyVault LPKeyVaultObj = _frontTrnRepository.GetTradeFeesLPArbitrage(LPType);
                BinanceClient Client = new BinanceClient(new BinanceClientOptions()
                {
                    ApiCredentials = new ApiCredentials(LPKeyVaultObj.APIKey, LPKeyVaultObj.SecretKey)
                });
                CallResult<BinanceTradeFee[]> BinanceResult = Client.GetTradeFee(PairArray[0].ToUpper() + PairArray[1].ToUpper());
                if (BinanceResult != null && BinanceResult?.Data != null && BinanceResult.Success)
                {
                    return BinanceResult.Data.FirstOrDefault().MakerFee;
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return 0;
            }
        }
    }    

    public class PriceTickerArbitrageHandler : IRequestHandler<ArbitrageLTPCls, ArbitrageLTPCls>
    {
        public IWebApiSendRequest _WebAPISendRequest { get; set; }

        public PriceTickerArbitrageHandler(IWebApiSendRequest WebAPISendRequest)
        {
            _WebAPISendRequest = WebAPISendRequest;
        }

        public async Task<ArbitrageLTPCls> Handle(ArbitrageLTPCls data, CancellationToken cancellationToken)
        {
            ArbitrageCoinbaseCryptoWatcherCls ResponseData = new ArbitrageCoinbaseCryptoWatcherCls();
            string Response = string.Empty;
            string Url = "https://api.cryptowat.ch/markets/#exchange#/#Pair#/price";
            try
            {
                switch (data.LpType)
                {
                    case (short)enAppType.Binance:
                        Url = "https://api.binance.com/api/v1/ticker/24hr?symbol=#Pair#";
                        Url = Url.Replace("#Pair#", data.ProviderPair);
                        Response = await _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false);
                        if (!string.IsNullOrEmpty(Response))
                        {
                            ArbitrageBinanceCryptoWatcherCls BinanceResponseData = new ArbitrageBinanceCryptoWatcherCls();
                            try
                            {
                                BinanceResponseData = JsonConvert.DeserializeObject<ArbitrageBinanceCryptoWatcherCls>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Binance ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (BinanceResponseData != null)
                            {
                                data.Price = Convert.ToDecimal(BinanceResponseData.lastPrice);
                                data.Volume = Convert.ToDecimal(BinanceResponseData.quoteVolume);
                                data.ChangePer = Convert.ToDecimal(BinanceResponseData.priceChangePercent);
                            }
                        }
                        break;
                    case (short)enAppType.Bittrex:
                        Url = "https://bittrex.com/api/v1.1/public/getmarketsummary?market=#Pair#";
                        Url = Url.Replace("#Pair#", data.ProviderPair);
                        Response = await _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false);
                        if (!string.IsNullOrEmpty(Response))
                        {
                            ArbitrageBittrexCryptoWatcherCls BittrexResponseData = new ArbitrageBittrexCryptoWatcherCls();
                            try
                            {
                                BittrexResponseData = JsonConvert.DeserializeObject<ArbitrageBittrexCryptoWatcherCls>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Bittrex ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (BittrexResponseData != null && BittrexResponseData.success)
                            {
                                data.Price = Convert.ToDecimal(BittrexResponseData.result.FirstOrDefault().Last);
                                data.Volume = Convert.ToDecimal(BittrexResponseData.result.FirstOrDefault().BaseVolume);
                                data.ChangePer = 0;
                            }
                        }
                        break;
                    case (short)enAppType.Coinbase:
                        Url = "https://api.pro.coinbase.com/products/#Pair#/ticker";
                        Url = Url.Replace("#Pair#", data.ProviderPair);
                        //                    web.Headers["User-Agent"] =
                        //"Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
                        //"(compatible; MSIE 6.0; Windows NT 5.1; " +
                        //".NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                        //            }
                        WebHeaderCollection HeaderCollection = new WebHeaderCollection();
                        HeaderCollection.Add(string.Format("User-Agent: {0}", ".NET CLR 1.1.4322; .NET CLR 2.0.50727;"));
                        Response = await _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", HeaderCollection, 90000, false);
                        if (!string.IsNullOrEmpty(Response))
                        {
                            try
                            {
                                ResponseData = JsonConvert.DeserializeObject<ArbitrageCoinbaseCryptoWatcherCls>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Coinbase ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (ResponseData != null)
                            {
                                data.Price = Convert.ToDecimal(ResponseData.price);
                                data.Volume = Convert.ToDecimal(ResponseData.volume);
                                data.ChangePer = 0;
                            }
                        }
                        break;
                    case (short)enAppType.Poloniex:
                        Url = "https://poloniex.com/public?command=returnTicker";
                        Response = await _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false);
                        if (!string.IsNullOrEmpty(Response))
                        {
                            Dictionary<string, poloniexWatcherAPIResponse> poloniexResponseData = new Dictionary<string, poloniexWatcherAPIResponse>();
                            try
                            {
                                poloniexResponseData = JsonConvert.DeserializeObject<Dictionary<string, poloniexWatcherAPIResponse>>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Poloniex ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (poloniexResponseData != null)
                            {
                                poloniexWatcherAPIResponse poloniexWatcher = new poloniexWatcherAPIResponse();
                                poloniexResponseData.TryGetValue(data.ProviderPair, out poloniexWatcher);
                                if (poloniexWatcher?.last != 0)
                                {
                                    //poloniexResponseData.TryGetValue(Pair[1].ToUpper() + "_" + Pair[0].ToUpper(), out poloniexWatcher);
                                    data.Price = poloniexWatcher.last;
                                    data.Volume = poloniexWatcher.quoteVolume;
                                    data.ChangePer = poloniexWatcher.percentChange;
                                }
                            }
                        }
                        break;
                    case (short)enAppType.TradeSatoshi:
                        Url = "https://tradesatoshi.com/api/public/getmarketsummary?market=#Pair#";
                        Url = Url.Replace("#Pair#", data.ProviderPair);
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            ArbitrageTradeSatoshiCryptoWatcherCls TradesatoshiResponseData = new ArbitrageTradeSatoshiCryptoWatcherCls();
                            try
                            {
                                TradesatoshiResponseData = JsonConvert.DeserializeObject<ArbitrageTradeSatoshiCryptoWatcherCls>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " TradeSatoshi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (TradesatoshiResponseData != null && TradesatoshiResponseData.result != null)
                            {
                                data.Price = Convert.ToDecimal(TradesatoshiResponseData.result.last);
                                data.Volume = Convert.ToDecimal(TradesatoshiResponseData.result.baseVolume);
                                data.ChangePer = Convert.ToDecimal(TradesatoshiResponseData.result.change);
                            }
                        }
                        break;

                    case (short)enAppType.UpBit:
                        Url = "https://api.upbit.com/v1/trades/ticks?market=#Pair#";
                        Url = Url.Replace("#Pair#", data.ProviderPair);
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            List<UpbitWatcherAPIResponse> UpbitResponseData = new List<UpbitWatcherAPIResponse>();
                            try
                            {
                                UpbitResponseData = JsonConvert.DeserializeObject<List<UpbitWatcherAPIResponse>>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " UpBit ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }
                            if (UpbitResponseData != null || UpbitResponseData.Count > 0)
                            {
                                data.Price = Convert.ToDecimal(UpbitResponseData[0].trade_price);
                                data.Volume = Convert.ToDecimal(UpbitResponseData[0].trade_volume);
                                data.ChangePer = Convert.ToDecimal(UpbitResponseData[0].change_price);
                            }
                        }
                        break;
                    case (short)enAppType.Huobi:
                        Url = "https://api.huobi.com/market/detail/merged?symbol=#Pair#";
                        Url = Url.Replace("#Pair#", data.ProviderPair.ToLower());
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            HuboiTickResult HuboiResponseData = new HuboiTickResult();
                            try
                            {
                                HuboiResponseData = JsonConvert.DeserializeObject<HuboiTickResult>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (HuboiResponseData != null && HuboiResponseData.status.ToLower() == "ok")
                            {
                                data.Price = Convert.ToDecimal(HuboiResponseData.tick.close);
                                data.Volume = Convert.ToDecimal(HuboiResponseData.tick.vol);
                                data.ChangePer = 0;
                            }
                        }
                        break;
                    /// Add new case for OKEx By Pushpraj as on 12-06-2019
                    case (short)enAppType.OKEx:
                        Url = "https://www.okex.com/api/spot/v3/instruments/#Pair#/ticker";
                        Url = Url.Replace("#Pair#", data.ProviderPair);
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            OKExGetTokenPairDetailReturn OKExResponseData = new OKExGetTokenPairDetailReturn();
                            try
                            {
                                OKExResponseData = JsonConvert.DeserializeObject<OKExGetTokenPairDetailReturn>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (OKExResponseData != null && OKExResponseData.last != null)
                            {
                                data.Price = Decimal.Parse(OKExResponseData.last);
                                data.Volume = decimal.Parse(OKExResponseData.quote_volume_24h);
                                data.ChangePer = 0;
                            }
                        }
                        break;
                    case (short)enAppType.Kraken:
                        Url = "https://api.kraken.com/0/public/Ticker?pair=###";
                        var Market = data.ProviderPair.Split("/");
                        Url = Url.Replace("###", Market[0]);
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            KrakenLTPCheckResponse KrakenResponseData = new KrakenLTPCheckResponse();
                            try
                            {
                                var Res = Response;
                                Res = Res.Replace(Market[1], "Data");
                                KrakenResponseData = JsonConvert.DeserializeObject<KrakenLTPCheckResponse>(Res);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if(KrakenResponseData != null && KrakenResponseData.result.Data.c != null)
                            {
                                data.Price = Decimal.Parse(KrakenResponseData.result.Data.c[0]);
                                data.Volume = decimal.Parse(KrakenResponseData.result.Data.v[0]);
                                data.ChangePer = 0;
                            }
                        }
                        break;
                    case (short)enAppType.CEXIO:
                        Url = "https://cex.io/api/last_price/#symbol1#";
                        Url = Url.Replace("#symbol1#", data.ProviderPair);
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            CEXIOTikerReturn CEXIOResponseData = new CEXIOTikerReturn();
                            try
                            {
                                CEXIOResponseData = JsonConvert.DeserializeObject<CEXIOTikerReturn>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (CEXIOResponseData != null && CEXIOResponseData.lprice != null)
                            {
                                data.Price = Decimal.Parse(CEXIOResponseData.lprice);
                                data.ChangePer = 0;
                            }
                        }
                        break;
                    case (short)enAppType.EXMO:
                        Url = "https://api.exmo.com/v1/ticker/";
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            EXMOAPIReqRes.EXMOLTPCheckResponse EXMOResponseData = new EXMOAPIReqRes.EXMOLTPCheckResponse();
                            try
                            {
                                Response = Response.Replace(data.ProviderPair, "symbol");
                                EXMOResponseData = JsonConvert.DeserializeObject<EXMOAPIReqRes.EXMOLTPCheckResponse>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " EXMO ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (EXMOResponseData != null && EXMOResponseData.symbol.last_trade != null)
                            {
                                data.Price = decimal.Parse(EXMOResponseData.symbol.last_trade);
                                data.Volume = decimal.Parse(EXMOResponseData.symbol.vol);
                                data.ChangePer = 0;
                            }
                        }
                        break;
                    case (short)enAppType.Bitfinex:
                        Url = "https://api.bitfinex.com/v1/pubticker/#Pair#";
                        Url = Url.Replace("#Pair#", data.ProviderPair.ToUpper().ToString());
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            BitfinexTLTPResponse BitfinexResponseData = new BitfinexTLTPResponse();
                            try
                            {
                                BitfinexResponseData = JsonConvert.DeserializeObject<BitfinexTLTPResponse>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (BitfinexResponseData != null && BitfinexResponseData.last_price != null)
                            {
                                data.Price = Decimal.Parse(BitfinexResponseData.last_price);
                                data.Volume = decimal.Parse(BitfinexResponseData.volume);
                                data.ChangePer = 0;
                            }
                        }
                        break;
                    case (short)enAppType.Yobit:
                        Url = "https://yobit.net/api/3/ticker/#Pair#";
                        Url = Url.Replace("#Pair#", data.ProviderPair.ToLower());
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            YobitAPIReqRes.YobitLTPCheckRespopnse YobitResponseData = new YobitAPIReqRes.YobitLTPCheckRespopnse();
                            try
                            {
                                Response = Response.Replace(data.ProviderPair.ToLower(), "result");
                                YobitResponseData = JsonConvert.DeserializeObject<YobitAPIReqRes.YobitLTPCheckRespopnse>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }

                            if (YobitResponseData != null && YobitResponseData.result != null && YobitResponseData.result.last != null)
                            {
                                data.Price = YobitResponseData.result.last;
                                data.Volume = YobitResponseData.result.vol;
                                data.ChangePer = 0;
                            }
                        }
                        break;
                    case (short)enAppType.Gemini:
                        Url = "https://api.gemini.com/v1/pubticker/#Pair#";
                        string[] symbols = data.Pair.Split("_");
                        Url = Url.Replace("#Pair#", data.ProviderPair.ToLower());
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000, false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(Response))
                        {
                            Response = Response.Replace(symbols[0], "C1");
                            Response = Response.Replace(symbols[1], "C2");
                            GeminiLTPResponse GeminiResponseData = new GeminiLTPResponse();
                            try
                            {
                                GeminiResponseData = JsonConvert.DeserializeObject<GeminiLTPResponse>(Response);
                            }
                            catch (Exception ex)
                            {
                                //HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " Huobi ", this.GetType().Name, ex);
                                return await Task.FromResult(data);
                            }
                            if (GeminiResponseData != null && GeminiResponseData.last != null)
                            {
                                data.Price = Decimal.Parse(GeminiResponseData.last);
                                data.Volume = decimal.Parse(GeminiResponseData.volume.C2);
                                data.ChangePer = 0;
                            }
                        }
                        break;
                }
                return await Task.FromResult(data);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return await Task.FromResult(data);
            }
        }
    }
}