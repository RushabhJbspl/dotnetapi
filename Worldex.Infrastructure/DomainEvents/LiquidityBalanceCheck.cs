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
using Worldex.Core.ApiModels;
using Worldex.Infrastructure.Interfaces;
using Worldex.Infrastructure.Data.Transaction;
using Worldex.Core.ViewModels.CCXT;
using System.Text.RegularExpressions;
using Worldex.Core.Entities.Transaction;

namespace Worldex.Infrastructure.Services
{

    public class LiquidityBalanceCheckHandler : IRequestHandler<LPBalanceCheck, LPBalanceCheck>
    {
        private readonly IFrontTrnRepository _frontTrnRepository;
        private readonly BinanceLPService _binanceLPService;
        private readonly BitrexLPService _bitrexLPService;
        private readonly ICoinBaseService _coinBaseService;
        private readonly IPoloniexService _poloniexService;
        private readonly IUpbitService _upbitService;
        private readonly IHuobiLPService _huobiLPService;
        private readonly IKrakenLPService _krakenLPService;
        private readonly ITradeSatoshiLPService _tradeSatoshiLPService;
        private readonly IOKExLPService _oKExLPService; //Add new Interface object for OKEx By Pushpraj as on 12-06-2019
        private readonly IGeminiLPService _geminiLPService;
        private readonly IEXMOLPService _eXMOLPService;
        private readonly IBitfinexLPService _bitfinexLPService;
        private readonly ICEXIOLPService _cEXIOLPService;
        private readonly IYobitLPService _yobitLPService; //Add new Interface object for Yobit Exchange by Pushpraj as on 15-07-2019

        public LiquidityBalanceCheckHandler(IFrontTrnRepository FrontTrnRepository, IKrakenLPService krakenLPService, IGeminiLPService geminiLPService,
        BinanceLPService BinanceLPService, BitrexLPService BitrexLPService, IUpbitService upbitService, IHuobiLPService huobiLPService,
        ICoinBaseService CoinBaseService, IPoloniexService PoloniexService, ITradeSatoshiLPService TradeSatoshiLPService, IOKExLPService oKExLPService,
        IBitfinexLPService bitfinexLPService, IYobitLPService yobitLPService, ICEXIOLPService cEXIOLPService, IEXMOLPService eXMOLPService)
        {
            _frontTrnRepository = FrontTrnRepository;
            _binanceLPService = BinanceLPService;
            _bitrexLPService = BitrexLPService;
            _coinBaseService = CoinBaseService;
            _poloniexService = PoloniexService;
            _upbitService = upbitService;
            _huobiLPService = huobiLPService;
            _tradeSatoshiLPService = TradeSatoshiLPService;
            _oKExLPService = oKExLPService; ///Add new Interface object for OKEx By Pushpraj as on 12-06-2019
            _krakenLPService = krakenLPService;
            _bitfinexLPService = bitfinexLPService;
            _geminiLPService = geminiLPService;
            _eXMOLPService = eXMOLPService;
            _cEXIOLPService = cEXIOLPService;
            _yobitLPService = yobitLPService; //Add new Interface object for Yobit Exchange by Pushpraj as on 15-07-2019
        }

        public async Task<LPBalanceCheck> Handle(LPBalanceCheck Request, CancellationToken cancellationToken)
        {
            try
            {
                LPKeyVault LPKeyVaultObj = _frontTrnRepository.BalanceCheckLP(Request.SerProID);// _frontTrnRepository.BalanceCheckLP(Request.SerProID);

                switch (LPKeyVaultObj.AppTypeID)
                {
                    case (long)enAppType.Binance:
                        await BalanceCheckOnBinance(Request, LPKeyVaultObj);
                        break;
                    case (long)enAppType.Huobi:
                        await BalanceCheckOnHuobi(Request, LPKeyVaultObj);
                        break;

                    case (long)enAppType.Bittrex:
                        await BalanceCheckOnBittrex(Request, LPKeyVaultObj);
                        break;

                    case (long)enAppType.TradeSatoshi:
                        await BalanceCheckOnTradeSatoshi(Request, LPKeyVaultObj);
                        break;
                    case (long)enAppType.Poloniex:
                        await BalanceCheckOnPoloniex(Request, LPKeyVaultObj);
                        break;
                    case (long)enAppType.Coinbase:
                        await BalanceCheckOnCoinbase(Request, LPKeyVaultObj);
                        break;
                    case (long)enAppType.UpBit:
                        await BalanceCheckOnUpbit(Request, LPKeyVaultObj);
                        break;

                    //Add New Case for OKEx by Pushpraj as on 12-06-2019
                    case (long)enAppType.OKEx:
                        await BalanceCheckOnOKEx(Request, LPKeyVaultObj);
                        break;
                    case (long)enAppType.Kraken:
                        await BalanceCheckOnKraken(Request, LPKeyVaultObj);
                        break;
                    case (long)enAppType.Bitfinex:
                        await BalanceCheckonBinfinex(Request, LPKeyVaultObj);
                        break;
                    case (long)enAppType.Gemini:
                        await BalanceCheckonGemini(Request, LPKeyVaultObj);
                        break;
                    case (long)enAppType.CEXIO:
                        await BalanceCheckonCEXIO(Request, LPKeyVaultObj);
                        break;
                    case (long)enAppType.EXMO:
                        await BalanceCheckonEXMO(Request, LPKeyVaultObj);
                        break;

                    case (long)enAppType.Yobit:
                        await BalanceCheckonYobit(Request, LPKeyVaultObj);
                        break;
                    default:
                        Request.Balance = 0;
                        HelperForLog.WriteLogIntoFile("LiquidityConfiguration", this.GetType().Name, "--3--LiquidityConfiguration Call web API  not found proper liquidity provider---" + "##Provider Type:" + LPKeyVaultObj.AppTypeID);
                        break;
                }
                HelperForLog.WriteLogIntoFile("LPBalanceCheck", "LiquidityBalanceCheckHandler", "Request Body : " + Helpers.JsonSerialize(Request));
                return await Task.FromResult(Request);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return await Task.FromResult(Request);
            }
        }

        private async Task<LPBalanceCheck> BalanceCheckonEXMO(LPBalanceCheck Request, LPKeyVault lPKeyVaultObj)
        {
            try
            {
                EXMOGlobalSettings.API_Key = lPKeyVaultObj.APIKey;
                EXMOGlobalSettings.Secret = lPKeyVaultObj.SecretKey;
                var Response = await _eXMOLPService.GetBalance(Request.Currency.ToUpper());
                if (Response != null)
                {
                    if (Response.balances != null)
                    {
                        Request.Balance = Convert.ToDecimal(Response.balances.Currency);
                    }
                }
                else
                {
                    Request.Balance = 0;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog("BalanceCheckonEXMO", "CryptoWatcherHandler handle", ex);
                return Request;
            }

        }

        private async Task<LPBalanceCheck> BalanceCheckonCEXIO(LPBalanceCheck Request, LPKeyVault lPKeyVaultObj)
        {
            try
            {
                var Response = await _cEXIOLPService.GetBalance(Request.Currency.ToUpper());

                if (Response != null)
                {

                    if (Response.symbol.available != null)
                    {
                        Request.Balance = Convert.ToDecimal(Response.symbol.available);
                    }

                }
                else
                {
                    Request.Balance = 0;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return Request;
            }

        }

        private async Task<LPBalanceCheck> BalanceCheckOnHuobi(LPBalanceCheck request, LPKeyVault lPKeyVaultObj)
        {
            try
            {
                HuobiClient.SetDefaultOptions(new HuobiClientOptions()
                {
                    ApiCredentials = new ApiCredentials(lPKeyVaultObj.APIKey, lPKeyVaultObj.SecretKey)
                });

                WebCallResult<List<HuobiBalance>> HuobiResult = await _huobiLPService.GetBalancesAsync(request.SerProID);

                if (HuobiResult != null)
                {
                    foreach (var balance in HuobiResult.Data)
                    {
                        if (balance.Currency.ToUpper() == request.Currency.ToUpper())
                        {
                            request.Balance = Convert.ToDecimal(balance.Balance);
                        }
                    }
                }
                else
                {
                    request.Balance = 0;
                }
                return request;
            }
            catch (Exception ex)
            {
                request.Balance = 0;
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return request;
            }

        }

        private async Task<LPBalanceCheck> BalanceCheckOnBinance(LPBalanceCheck Request, LPKeyVault LPKeyVaultObj)
        {
            try
            {
                //BinanceClient.SetDefaultOptions(new BinanceClientOptions()
                //{
                //    ApiCredentials = new ApiCredentials(LPKeyVaultObj.APIKey, LPKeyVaultObj.SecretKey)
                //});
                _binanceLPService._client.SetApiCredentials(LPKeyVaultObj.APIKey, LPKeyVaultObj.SecretKey);
                CallResult<BinanceAccountInfo> BinanceResult = await _binanceLPService.GetBalancesAsync();
                if (BinanceResult != null && BinanceResult?.Data != null && BinanceResult.Data?.Balances != null && BinanceResult.Success)
                {
                    foreach (var balance in BinanceResult.Data.Balances)
                    {
                        if (balance.Asset.ToUpper() == Request.Currency.ToUpper())
                        {
                            Request.Balance = balance.Free;
                        }
                    }
                }
                else
                {
                    Request.Balance = 0;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return Request;
            }
        }

        private async Task<LPBalanceCheck> BalanceCheckOnBittrex(LPBalanceCheck Request, LPKeyVault LPKeyVaultObj)
        {
            try
            {
                //BittrexClient.SetDefaultOptions(new BittrexClientOptions()
                //{
                //    ApiCredentials = new ApiCredentials(LPKeyVaultObj.APIKey, LPKeyVaultObj.SecretKey)
                //});
                _bitrexLPService._client.SetApiCredentials(LPKeyVaultObj.APIKey, LPKeyVaultObj.SecretKey);
                CallResult<BittrexBalance> BittrexResult = await _bitrexLPService.GetBalanceAsync(Request.Currency.ToUpper());
                if (BittrexResult != null && BittrexResult.Success && BittrexResult.Data != null && BittrexResult.Data?.Available != null)
                {
                    Request.Balance = Convert.ToDecimal(BittrexResult.Data.Available);
                }
                else
                {
                    Request.Balance = 0;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return Request;
            }
        }

        private async Task<LPBalanceCheck> BalanceCheckOnTradeSatoshi(LPBalanceCheck Request, LPKeyVault LPKeyVaultObj)
        {
            try
            {
                GlobalSettings.API_Key = LPKeyVaultObj.APIKey;
                GlobalSettings.Secret = LPKeyVaultObj.SecretKey;
                //GetBalancesReturn TradeSatoshiResult = await _tradeSatoshiLPService.GetBalancesAsync();
                GetBalancesReturn TradeSatoshiResult = await _tradeSatoshiLPService.GetBalanceAsync(Request.Currency.ToString());
                if (TradeSatoshiResult != null && TradeSatoshiResult.success && TradeSatoshiResult.result != null)
                {
                    //foreach (var balance in TradeSatoshiResult.result)
                    if (TradeSatoshiResult.result.available != null)
                    {
                        if (TradeSatoshiResult.result.currency.ToUpper() == Request.Currency.ToUpper())

                        {
                            Request.Balance = TradeSatoshiResult.result.available;
                        }
                    }
                }
                else
                {
                    Request.Balance = 0;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return Request;
            }
        }

        /// <summary>
        /// Add new Class for Call the BalanceCheck Method Of OKEx API By Pushpraj as on 12-0-2019
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="LPKeyVaultObj"></param>
        /// <returns></returns>
        #region "OKEx Balance Check Method call class"
        private async Task<LPBalanceCheck> BalanceCheckOnOKEx(LPBalanceCheck Request, LPKeyVault LPKeyVaultObj)
        {
            try
            {
                OKEXGlobalSettings.API_Key = LPKeyVaultObj.APIKey;
                OKEXGlobalSettings.Secret = LPKeyVaultObj.SecretKey;
                OKEXGlobalSettings.PassPhrase = "paRo@1$##";

                OKEBalanceResult OKExResult = await _oKExLPService.GetWalletBalanceAsync();
                if (OKExResult.Data != null)
                {
                    foreach (var bal in OKExResult.Data)
                    {
                        var PairArray = Request.Currency.ToString().ToUpper().Split("_");
                        if (bal.currency.ToString().ToUpper() == PairArray[0].ToString().ToUpper())
                        {
                            Request.Balance = Convert.ToDecimal(bal.available);
                        }
                    }
                }
                else
                {
                    Request.Balance = 0;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return Request;
            }
        }
        #endregion
        /// <summary>
        /// End Add new Class for Call the BalanceCheck Method Of OKEx API By Pushpraj as on 12-0-2019
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="LPKeyVaultObj"></param>
        /// <returns></returns>

        private async Task<LPBalanceCheck> BalanceCheckOnCoinbase(LPBalanceCheck Request, LPKeyVault LPKeyVaultObj)
        {
            try
            {
                CoinBaseGlobalSettings.API_Key = LPKeyVaultObj.APIKey;
                CoinBaseGlobalSettings.Secret = LPKeyVaultObj.SecretKey;

                IEnumerable<CoinbasePro.Services.Accounts.Models.Account> CoinbaseResult = await _coinBaseService.GetAllAccountsAsync();
                if (CoinbaseResult != null && CoinbaseResult.Count() > 0)
                {
                    foreach (var balance in CoinbaseResult)
                    {
                        if (balance.Currency.ToString().ToUpper() == Request.Currency.ToUpper())
                        {
                            Request.Balance = balance.Available;
                        }
                    }
                }
                else
                {
                    Request.Balance = 0;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return Request;
            }
        }

        private async Task<LPBalanceCheck> BalanceCheckOnPoloniex(LPBalanceCheck Request, LPKeyVault LPKeyVaultObj)
        {
            try
            {
                PoloniexGlobalSettings.API_Key = LPKeyVaultObj.APIKey;
                PoloniexGlobalSettings.Secret = LPKeyVaultObj.SecretKey;

                Dictionary<string, decimal> PoloniexResult = await _poloniexService.PoloniexGetBalance();
                if (PoloniexResult != null)
                {
                    foreach (var balance in PoloniexResult)
                    {
                        if (balance.Key.ToUpper() == Request.Currency.ToUpper())
                        {
                            Request.Balance = balance.Value;
                        }
                    }
                }
                else
                {
                    Request.Balance = 0;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return Request;
            }
        }

        private async Task<LPBalanceCheck> BalanceCheckOnUpbit(LPBalanceCheck Request, LPKeyVault LPKeyVaultObj)
        {
            try
            {
                //PoloniexGlobalSettings.API_Key = LPKeyVaultObj.APIKey;
                //PoloniexGlobalSettings.Secret = LPKeyVaultObj.SecretKey;

                var UpbitResult = await _upbitService.GetCurrenciesAsync();
                if (UpbitResult != null)
                {
                    foreach (var balance in UpbitResult.Result)
                    {
                        if (balance.currency.ToUpper() == Request.Currency.ToUpper())
                        {
                            Request.Balance = Convert.ToDecimal(balance.balance);
                        }
                    }
                }
                else
                {
                    Request.Balance = 0;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog("BalanceCheckOnUpbit", "CryptoWatcherHandler handle", ex);
                return Request;
            }
        }

        private async Task<LPBalanceCheck> BalanceCheckOnKraken(LPBalanceCheck Request, LPKeyVault LPKeyVaultObj)
        {
            try
            {
                KrakenGlobalSettings.API_Key = LPKeyVaultObj.APIKey;
                KrakenGlobalSettings.Secret = LPKeyVaultObj.SecretKey;

                var KrakenResult = await _krakenLPService.GetBalances();
                if (KrakenResult != null)
                {
                    foreach (var balance in KrakenResult.result.Data.wsname)
                    {
                        //if (balance.ToString().ToUpper() == Request.Currency.ToUpper())
                        //{
                        //Request.Balance = Convert.ToDecimal(balance);
                        //}
                        Request.Balance = Convert.ToDecimal(balance);
                    }
                }
                else
                {
                    Request.Balance = 0;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
                return Request;
            }
        }

        private async Task<LPBalanceCheck> BalanceCheckonBinfinex(LPBalanceCheck Request, LPKeyVault lPKeyVaultObj)
        {
            try
            {
                var BinfinexResult = await _bitfinexLPService.GetBalanceData(Request.Currency.ToString());
                if (BinfinexResult != null && BinfinexResult.balance != null)
                {

                    if (BinfinexResult.currency.ToUpper() == Request.Currency.ToUpper())
                    {
                        Request.Balance = Convert.ToDecimal(BinfinexResult.balance);
                    }
                }
                else
                {
                    Request.Balance = 0;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog("BalanceCheckonBinfinex", "CryptoWatcherHandler handle", ex);
                return Request;
            }
        }

        private async Task<LPBalanceCheck> BalanceCheckonGemini(LPBalanceCheck Request, LPKeyVault lPKeyVaultObj)
        {
            try
            {
                var GeminiResult = await _geminiLPService.GetBalancesAsync();
                if (GeminiResult != null && GeminiResult.Data != null)
                {
                    foreach (var balance in GeminiResult.Data)
                    {
                        if (balance.currency.ToUpper() == Request.Currency.ToUpper())
                        {
                            Request.Balance = Convert.ToDecimal(balance.available);
                        }
                    }
                }
                else
                {
                    Request.Balance = 0;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog("BalanceCheckonGemini", "CryptoWatcherHandler handle", ex);
                return Request;
            }
        }

        #region "Yobit Balance Check"
        private async Task<LPBalanceCheck> BalanceCheckonYobit(LPBalanceCheck Request, LPKeyVault lPKeyVaultObj)
        {
            try
            {
                YobitGlobalSettings.API_Key = lPKeyVaultObj.APIKey;
                YobitGlobalSettings.Secret = lPKeyVaultObj.SecretKey;
                var YobitResult = await _yobitLPService.GetBalance();
                if (YobitResult != null)
                {
                    if (YobitResult.@return.funds != null || YobitResult.@return.funds_incl_orders != null)
                    {
                        if (YobitResult.@return.funds_incl_orders.Values.ToString() == Request.Currency.ToUpper())
                        {
                            Request.Balance = Convert.ToDecimal(YobitResult.@return.funds.Values);
                        }
                        else
                        {
                            Request.Balance = 0;
                        }
                    }
                    else
                    {
                        Request.Balance = 0;
                    }
                }
                else
                {
                    Request.Balance = 0;
                    return Request;
                }
                return Request;
            }
            catch (Exception ex)
            {
                Request.Balance = 0;
                HelperForLog.WriteErrorLog("YobitBalanceCheck", "CryptoWatcherHandler handle", ex);
                return Request;
            }
        }

        #endregion
    }
   
    //public class ArbitrageLiquidityBalanceCheckHandler : IRequestHandler<LPBalanceCheckArbitrage, LPBalanceCheckArbitrage>
    //{
    //    private readonly IFrontTrnRepository _frontTrnRepository;
    //    private readonly BinanceLPService _binanceLPService;
    //    private readonly BitrexLPService _bitrexLPService;
    //    private readonly ICoinBaseService _coinBaseService;
    //    private readonly IPoloniexService _poloniexService;
    //    private readonly IUpbitService _upbitService;
    //    private readonly IHuobiLPService _huobiLPService;
    //    private readonly ITradeSatoshiLPService _tradeSatoshiLPService;
    //    private readonly IOKExLPService _oKExLPService;
    //    private readonly ICEXIOLPService _cEXIOLPService;
    //    private readonly IEXMOLPService _eXMOLPService;
    //    private readonly IKrakenLPService _krakenLPService;
    //    private IMemoryCache _cache;
    //    private readonly IWebApiRepository _WebApiRepository;
    //    private readonly ITransactionProcessArbitrageV1 _transactionProcessArbitrageV1;
    //    private readonly IGetWebRequest _IGetWebRequest;
    //    private readonly IWebApiSendRequest _IWebApiSendRequest;
    //    private GetDataForParsingAPI txnWebAPIParsingData;
    //    private readonly WebApiDataRepository _webapiDataRepository;
    //    WebApiParseResponse _WebApiParseResponseObj;

    //    public ArbitrageLiquidityBalanceCheckHandler(IFrontTrnRepository FrontTrnRepository,
    //    BinanceLPService BinanceLPService, BitrexLPService BitrexLPService, IUpbitService upbitService, IHuobiLPService huobiLPService,
    //    ICoinBaseService CoinBaseService, IPoloniexService PoloniexService, ITradeSatoshiLPService TradeSatoshiLPService, IOKExLPService oKExLPService,
    //    IKrakenLPService krakenLPService, ICEXIOLPService cEXIOLPService, IEXMOLPService eXMOLPService, IMemoryCache cache,
    //    IWebApiRepository WebApiRepository, ITransactionProcessArbitrageV1 transactionProcessArbitrageV1, IGetWebRequest IGetWebRequest,
    //    IWebApiSendRequest IWebApiSendRequest, WebApiDataRepository webapiDataRepository, WebApiParseResponse WebApiParseResponseObj)
    //    {
    //        _frontTrnRepository = FrontTrnRepository;
    //        _binanceLPService = BinanceLPService;
    //        _bitrexLPService = BitrexLPService;
    //        _coinBaseService = CoinBaseService;
    //        _poloniexService = PoloniexService;
    //        _upbitService = upbitService;
    //        _huobiLPService = huobiLPService;
    //        _tradeSatoshiLPService = TradeSatoshiLPService;
    //        _oKExLPService = oKExLPService;
    //        _krakenLPService = krakenLPService;
    //        _cEXIOLPService = cEXIOLPService;
    //        _eXMOLPService = eXMOLPService;
    //        _cache = cache;
    //        _WebApiRepository = WebApiRepository;
    //        _transactionProcessArbitrageV1 = transactionProcessArbitrageV1;
    //        _IGetWebRequest = IGetWebRequest;
    //        _IWebApiSendRequest = IWebApiSendRequest;
    //        _webapiDataRepository = webapiDataRepository;
    //        _WebApiParseResponseObj = WebApiParseResponseObj;
    //    }

    //    public async Task<LPBalanceCheckArbitrage> Handle(LPBalanceCheckArbitrage Request, CancellationToken cancellationToken)
    //    {
    //        TransactionProviderArbitrageResponse ProviderObj = new TransactionProviderArbitrageResponse();
    //        string cacheToken = "";
    //        ThirdPartyAPIRequestArbitrage ThirdPartyAPIRequestOnj = new ThirdPartyAPIRequestArbitrage();
    //        short IsAPIProceed = 0;
    //        string APIResponse = "";
    //        CCXTBalanceObj balanceObj = new CCXTBalanceObj();
    //        try
    //        {
    //            var list = _cache.Get<List<TransactionProviderArbitrageResponse>>("RouteDataArbitrage");
    //            if (list == null)
    //            {
    //                var GetProListResult = _WebApiRepository.GetProviderDataListArbitrageAsync(new TransactionApiConfigurationRequest { PairID = 0, trnType = 4, LPType = 0 });
    //                list = _cache.Get<List<TransactionProviderArbitrageResponse>>("RouteDataArbitrage");
    //            }
    //            if (list != null)
    //            {
    //                ProviderObj = list.Where(e => e.TrnType == 4 && e.TrnTypeID == 4 && e.ProTypeID == 7 && e.ProviderID == Request.SerProID).FirstOrDefault();
    //                if (ProviderObj != null)
    //                {
    //                    cacheToken = _cache.Get<string>("LPType" + ProviderObj.LPType);
    //                    if (cacheToken == null)
    //                    {
    //                        cacheToken = await _transactionProcessArbitrageV1.ConnectToExchangeAsync(ProviderObj, new TransactionQueueArbitrage(), 0);
    //                        _cache.Set<string>("LPType" + ProviderObj.LPType, cacheToken);
    //                    }
    //                    txnWebAPIParsingData = new GetDataForParsingAPI();
    //                    txnWebAPIParsingData = _webapiDataRepository.ArbitrageGetDataForParsingAPI(ProviderObj.ThirdPartyAPIID);
    //                    txnWebAPIParsingData.BalanceRegex = txnWebAPIParsingData.BalanceRegex.Replace("#COIN#", Request.Currency);
    //                    ThirdPartyAPIRequestOnj = _IGetWebRequest.ArbitrageMakeWebRequest(ProviderObj.RouteID, ProviderObj.ThirdPartyAPIID, ProviderObj.SerProDetailID, IsValidateUrl: 2, Token: cacheToken);
    //                    APIResponse = await _IWebApiSendRequest.SendRequestAsyncLPArbitrage(ThirdPartyAPIRequestOnj.RequestURL, ref IsAPIProceed, ThirdPartyAPIRequestOnj.RequestBody, ThirdPartyAPIRequestOnj.MethodType, ThirdPartyAPIRequestOnj.ContentType, ThirdPartyAPIRequestOnj.keyValuePairsHeader, 30000, IsWrite: false);
                        
    //                    try
    //                    {
    //                        string MatchRegex = Regex.Match(APIResponse, txnWebAPIParsingData.BalanceRegex, new RegexOptions()).Value;
    //                        if (!string.IsNullOrEmpty(MatchRegex))
    //                        {
    //                            balanceObj = JsonConvert.DeserializeObject<CCXTBalanceObj>(MatchRegex);
    //                        }
    //                        Request.Balance = Convert.ToDecimal(balanceObj.total);
    //                        Request.Free = Convert.ToDecimal(balanceObj.free);
    //                        Request.Hold = Convert.ToDecimal(balanceObj.used);
    //                        Request.ReturnMsg = "Success";
    //                        Request.ErrorCode = enErrorCode.Success;
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        HelperForLog.WriteErrorLog(" ArbitrageLiquidityBalanceCheckHandler ", " Exchange Name : " + ProviderObj.ProviderName + " " + APIResponse, ex);
    //                        Request.ReturnMsg = "Fail";
    //                        Request.ErrorCode = enErrorCode.InternalError;
    //                        return await Task.FromResult(Request);
    //                    }
    //                    HelperForLog.WriteLogIntoFile(" ArbitrageLiquidityBalanceCheckHandler ", " Exchange Name : " + ProviderObj.ProviderName, JsonConvert.SerializeObject(Request));
    //                    return await Task.FromResult(Request);                        
    //                }
    //            }

    //            LPKeyVault LPKeyVaultObj = _frontTrnRepository.BalanceCheckLPArbitrage(Request.SerProID);

    //            switch (LPKeyVaultObj.AppTypeID)
    //            {
    //                case (long)enAppType.Binance:
    //                    await BalanceCheckOnBinance(Request, LPKeyVaultObj);
    //                    break;
    //                case (long)enAppType.Huobi:
    //                    await BalanceCheckOnHuobi(Request, LPKeyVaultObj);
    //                    break;
    //                case (long)enAppType.Bittrex:
    //                    await BalanceCheckOnBittrex(Request, LPKeyVaultObj);
    //                    break;
    //                case (long)enAppType.TradeSatoshi:
    //                    await BalanceCheckOnTradeSatoshi(Request, LPKeyVaultObj);
    //                    break;
    //                case (long)enAppType.Poloniex:
    //                    await BalanceCheckOnPoloniex(Request, LPKeyVaultObj);
    //                    break;
    //                case (long)enAppType.Coinbase:
    //                    await BalanceCheckOnCoinbase(Request, LPKeyVaultObj);
    //                    break;
    //                case (long)enAppType.UpBit:
    //                    await BalanceCheckOnUpbit(Request, LPKeyVaultObj);
    //                    break;
    //                //Add New Case for OKEx by Pushpraj as on 12-06-2019
    //                case (long)enAppType.OKEx:
    //                    await BalanceCheckOnOKEx(Request, LPKeyVaultObj);
    //                    break;
    //                case (long)enAppType.Kraken:
    //                    await BalanceCheckOnKraken(Request, LPKeyVaultObj);
    //                    break;
    //                case (long)enAppType.CEXIO:
    //                    await BalanceCheckonCEXIO(Request, LPKeyVaultObj);
    //                    break;
    //                case (long)enAppType.EXMO:
    //                    await BalanceCheckonEXMO(Request, LPKeyVaultObj);
    //                    break;
    //                default:
    //                    Request.Balance = 0;
    //                    HelperForLog.WriteLogIntoFile("LiquidityConfiguration", this.GetType().Name, "--3--LiquidityConfiguration Call web API  not found proper liquidity provider---" + "##Provider Type:" + LPKeyVaultObj.AppTypeID);
    //                    break;
    //            }
    //            HelperForLog.WriteLogIntoFile("LPBalanceCheckArbitrage", "ArbitrageLiquidityBalanceCheckHandler", "Request Body : " + Helpers.JsonSerialize(Request));
    //            return await Task.FromResult(Request);
    //        }
    //        catch (Exception ex)
    //        {
    //            HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
    //            return await Task.FromResult(Request);
    //        }
    //    }

    //    private async Task<LPBalanceCheckArbitrage> BalanceCheckOnHuobi(LPBalanceCheckArbitrage request, LPKeyVault lPKeyVaultObj)
    //    {
    //        try
    //        {
    //            HuobiClient.SetDefaultOptions(new HuobiClientOptions()
    //            {
    //                ApiCredentials = new ApiCredentials(lPKeyVaultObj.APIKey, lPKeyVaultObj.SecretKey)
    //            });
    //            WebCallResult<List<HuobiBalance>> HuobiResult = await _huobiLPService.GetBalancesAsync(request.SerProID);
    //            if (HuobiResult != null)
    //            {
    //                foreach (var balance in HuobiResult.Data)
    //                {
    //                    if (balance.Currency.ToUpper() == request.Currency.ToUpper())
    //                    {
    //                        request.Balance = Convert.ToDecimal(balance.Balance);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                request.Balance = 0;
    //            }
    //            return request;
    //        }
    //        catch (Exception ex)
    //        {
    //            request.Balance = 0;
    //            HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
    //            return request;
    //        }
    //    }
    //    private async Task<LPBalanceCheckArbitrage> BalanceCheckOnBinance(LPBalanceCheckArbitrage Request, LPKeyVault LPKeyVaultObj)
    //    {
    //        try
    //        {
    //            //BinanceClient.SetDefaultOptions(new BinanceClientOptions()
    //            //{
    //            //    ApiCredentials = new ApiCredentials(LPKeyVaultObj.APIKey, LPKeyVaultObj.SecretKey)
    //            //});
    //            _binanceLPService._client.SetApiCredentials(LPKeyVaultObj.APIKey, LPKeyVaultObj.SecretKey);
    //            CallResult<BinanceAccountInfo> BinanceResult = await _binanceLPService.GetBalancesAsync();
    //            if (BinanceResult != null && BinanceResult?.Data != null && BinanceResult.Data?.Balances != null && BinanceResult.Success)
    //            {
    //                foreach (var balance in BinanceResult.Data.Balances)
    //                {
    //                    if (balance.Asset.ToUpper() == Request.Currency.ToUpper())
    //                    {
    //                        Request.Balance = balance.Free;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                Request.Balance = 0;
    //            }
    //            HelperForLog.WriteLogIntoFile("BalanceCheckOnBinance", "LPBalanceCheck", JsonConvert.SerializeObject(BinanceResult));
    //            return Request;
    //        }
    //        catch (Exception ex)
    //        {
    //            Request.Balance = 0;
    //            HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
    //            return Request;
    //        }
    //    }
    //    private async Task<LPBalanceCheckArbitrage> BalanceCheckOnBittrex(LPBalanceCheckArbitrage Request, LPKeyVault LPKeyVaultObj)
    //    {
    //        try
    //        {
    //            //BittrexClient.SetDefaultOptions(new BittrexClientOptions()
    //            //{
    //            //    ApiCredentials = new ApiCredentials(LPKeyVaultObj.APIKey, LPKeyVaultObj.SecretKey)
    //            //});
    //            _bitrexLPService._client.SetApiCredentials(LPKeyVaultObj.APIKey, LPKeyVaultObj.SecretKey);
    //            CallResult<BittrexBalance> BittrexResult = await _bitrexLPService.GetBalanceAsync(Request.Currency.ToUpper());
    //            if (BittrexResult != null && BittrexResult.Success && BittrexResult.Data != null && BittrexResult.Data?.Available != null)
    //            {
    //                Request.Balance = Convert.ToDecimal(BittrexResult.Data.Available);
    //            }
    //            else
    //            {
    //                Request.Balance = 0;
    //            }
    //            return Request;
    //        }
    //        catch (Exception ex)
    //        {
    //            Request.Balance = 0;
    //            HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
    //            return Request;
    //        }
    //    }
    //    private async Task<LPBalanceCheckArbitrage> BalanceCheckOnTradeSatoshi(LPBalanceCheckArbitrage Request, LPKeyVault LPKeyVaultObj)
    //    {
    //        try
    //        {
    //            GlobalSettings.API_Key = LPKeyVaultObj.APIKey;
    //            GlobalSettings.Secret = LPKeyVaultObj.SecretKey;
    //            //GetBalancesReturn TradeSatoshiResult = await _tradeSatoshiLPService.GetBalancesAsync();
    //            GetBalancesReturn TradeSatoshiResult = await _tradeSatoshiLPService.GetBalanceAsync(Request.Currency.ToString());
    //            if (TradeSatoshiResult != null && TradeSatoshiResult.success && TradeSatoshiResult.result != null)
    //            {
    //                //foreach (var balance in TradeSatoshiResult.result)
    //                if (TradeSatoshiResult.result.available != null)
    //                {
    //                    if (TradeSatoshiResult.result.currency.ToUpper() == Request.Currency.ToUpper())

    //                    {
    //                        Request.Balance = TradeSatoshiResult.result.available;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                Request.Balance = 0;
    //            }
    //            return Request;
    //        }
    //        catch (Exception ex)
    //        {
    //            Request.Balance = 0;
    //            HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
    //            return Request;
    //        }
    //    }
    //    private async Task<LPBalanceCheckArbitrage> BalanceCheckOnCoinbase(LPBalanceCheckArbitrage Request, LPKeyVault LPKeyVaultObj)
    //    {
    //        try
    //        {
    //            CoinBaseGlobalSettings.API_Key = LPKeyVaultObj.APIKey;
    //            CoinBaseGlobalSettings.Secret = LPKeyVaultObj.SecretKey;
    //            IEnumerable<CoinbasePro.Services.Accounts.Models.Account> CoinbaseResult = await _coinBaseService.GetAllAccountsAsync();
    //            if (CoinbaseResult != null && CoinbaseResult.Count() > 0)
    //            {
    //                foreach (var balance in CoinbaseResult)
    //                {
    //                    if (balance.Currency.ToString().ToUpper() == Request.Currency.ToUpper())
    //                    {
    //                        Request.Balance = balance.Available;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                Request.Balance = 0;
    //            }
    //            return Request;
    //        }
    //        catch (Exception ex)
    //        {
    //            Request.Balance = 0;
    //            HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
    //            return Request;
    //        }
    //    }
    //    private async Task<LPBalanceCheckArbitrage> BalanceCheckOnPoloniex(LPBalanceCheckArbitrage Request, LPKeyVault LPKeyVaultObj)
    //    {
    //        try
    //        {
    //            PoloniexGlobalSettings.API_Key = LPKeyVaultObj.APIKey;
    //            PoloniexGlobalSettings.Secret = LPKeyVaultObj.SecretKey;
    //            Dictionary<string, decimal> PoloniexResult = await _poloniexService.PoloniexGetBalance();
    //            if (PoloniexResult != null)
    //            {
    //                foreach (var balance in PoloniexResult)
    //                {
    //                    if (balance.Key.ToUpper() == Request.Currency.ToUpper())
    //                    {
    //                        Request.Balance = balance.Value;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                Request.Balance = 0;
    //            }
    //            return Request;
    //        }
    //        catch (Exception ex)
    //        {
    //            Request.Balance = 0;
    //            HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
    //            return Request;
    //        }
    //    }
    //    private async Task<LPBalanceCheckArbitrage> BalanceCheckOnUpbit(LPBalanceCheckArbitrage Request, LPKeyVault LPKeyVaultObj)
    //    {
    //        try
    //        {
    //            //PoloniexGlobalSettings.API_Key = LPKeyVaultObj.APIKey;
    //            //PoloniexGlobalSettings.Secret = LPKeyVaultObj.SecretKey;
    //            var UpbitResult = await _upbitService.GetCurrenciesAsync();
    //            if (UpbitResult != null)
    //            {
    //                foreach (var balance in UpbitResult.Result)
    //                {

    //                    if (balance.currency.ToUpper() == Request.Currency.ToUpper())
    //                    {
    //                        Request.Balance = Convert.ToDecimal(balance.balance);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                Request.Balance = 0;
    //            }
    //            return Request;
    //        }
    //        catch (Exception ex)
    //        {
    //            Request.Balance = 0;
    //            HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
    //            return Request;
    //        }
    //    }
    //    private async Task<LPBalanceCheckArbitrage> BalanceCheckOnOKEx(LPBalanceCheckArbitrage Request, LPKeyVault LPKeyVaultObj)
    //    {
    //        try
    //        {
    //            OKEXGlobalSettings.API_Key = LPKeyVaultObj.APIKey;
    //            OKEXGlobalSettings.Secret = LPKeyVaultObj.SecretKey;
    //            OKEXGlobalSettings.PassPhrase = "paRo@1$##";
    //            OKEBalanceResult OKExResult = await _oKExLPService.GetWalletBalanceAsync();
    //            if (OKExResult.Data != null)
    //            {
    //                foreach (var bal in OKExResult.Data)
    //                {
    //                    var PairArray = Request.Currency.ToString().ToUpper().Split("_");
    //                    if (bal.currency.ToString().ToUpper() == PairArray[0].ToString().ToUpper())
    //                    {
    //                        Request.Balance = Convert.ToDecimal(bal.available);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                Request.Balance = 0;
    //            }
    //            return Request;
    //        }
    //        catch (Exception ex)
    //        {
    //            Request.Balance = 0;
    //            HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
    //            return Request;
    //        }
    //    }
    //    private async Task<LPBalanceCheckArbitrage> BalanceCheckonEXMO(LPBalanceCheckArbitrage Request, LPKeyVault lPKeyVaultObj)
    //    {
    //        try
    //        {
    //            EXMOGlobalSettings.API_Key = lPKeyVaultObj.APIKey;
    //            EXMOGlobalSettings.Secret = lPKeyVaultObj.SecretKey;
    //            var Response = await _eXMOLPService.GetBalance(Request.Currency.ToUpper());
    //            if (Response != null)
    //            {
    //                if (Response.balances != null)
    //                {
    //                    Request.Balance = Convert.ToDecimal(Response.balances.Currency);
    //                }
    //            }
    //            else
    //            {
    //                Request.Balance = 0;
    //            }
    //            return Request;
    //        }
    //        catch (Exception ex)
    //        {
    //            Request.Balance = 0;
    //            HelperForLog.WriteErrorLog("BalanceCheckonEXMO", "CryptoWatcherHandler handle", ex);
    //            return Request;
    //        }

    //    }
    //    private async Task<LPBalanceCheckArbitrage> BalanceCheckonCEXIO(LPBalanceCheckArbitrage Request, LPKeyVault lPKeyVaultObj)
    //    {
    //        try
    //        {
    //            var Response = await _cEXIOLPService.GetBalance(Request.Currency.ToUpper());

    //            if (Response != null)
    //            {

    //                if (Response.symbol.available != null)
    //                {
    //                    Request.Balance = Convert.ToDecimal(Response.symbol.available);
    //                }

    //            }
    //            else
    //            {
    //                Request.Balance = 0;
    //            }
    //            return Request;
    //        }
    //        catch (Exception ex)
    //        {
    //            Request.Balance = 0;
    //            HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
    //            return Request;
    //        }

    //    }
    //    private async Task<LPBalanceCheckArbitrage> BalanceCheckOnKraken(LPBalanceCheckArbitrage Request, LPKeyVault LPKeyVaultObj)
    //    {
    //        try
    //        {
    //            KrakenGlobalSettings.API_Key = LPKeyVaultObj.APIKey;
    //            KrakenGlobalSettings.Secret = LPKeyVaultObj.SecretKey;

    //            var KrakenResult = await _krakenLPService.GetBalances();
    //            if (KrakenResult != null)
    //            {
    //                foreach (var balance in KrakenResult.result.Data.wsname)
    //                {
    //                    //if (balance.ToString().ToUpper() == Request.Currency.ToUpper())
    //                    //{
    //                    //Request.Balance = Convert.ToDecimal(balance);
    //                    //}
    //                    Request.Balance = Convert.ToDecimal(balance);
    //                }
    //            }
    //            else
    //            {
    //                Request.Balance = 0;
    //            }
    //            return Request;
    //        }
    //        catch (Exception ex)
    //        {
    //            Request.Balance = 0;
    //            HelperForLog.WriteErrorLog("CryptoWatcherHandler", "CryptoWatcherHandler handle", ex);
    //            return Request;
    //        }
    //    }
    //}

    
}