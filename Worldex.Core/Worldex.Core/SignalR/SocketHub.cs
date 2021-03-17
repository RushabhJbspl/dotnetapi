using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Core.ApiModels.Chat;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Microsoft.Extensions.Configuration;

namespace Worldex.Core.SignalR
{
    public class SocketHub : Hub
    {
        private RedisConnectionFactory _fact;
        private IHubContext<SocketHub> _chatHubContext;
        private readonly ILogger<SocketHub> _logger;
        private readonly IConfiguration Configuration;
        public SocketHub(IHubContext<SocketHub> ChatHubContext, RedisConnectionFactory Factory, ILogger<SocketHub> logger, IConfiguration configuration)
        {
            _fact = Factory;
            _chatHubContext = ChatHubContext;
            _logger = logger;
            Configuration = configuration;
        }

        #region "For Testing Connection"
        public Task SendToAll(string name, string message)
        {
            try
            {
                _chatHubContext.Clients.All.SendAsync("sendToAll", name, message);
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }
        #endregion

        #region "For Connection Management By default from Hub Class"

        public override Task OnConnectedAsync()
        {
            try
            {
                string Pair = Configuration.GetValue<string>("SignalRKey:DefaultPair");
                string BaseCurrency = Configuration.GetValue<string>("SignalRKey:DefaultBaseCurrency");
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                Redis.SaveTagsToSetMember(Configuration.GetValue<string>("SignalRKey:RedisPairs") + Pair, Context.ConnectionId, Pair);
                Redis.SaveTagsToSetMember(Configuration.GetValue<string>("SignalRKey:RedisMarket") + BaseCurrency, Context.ConnectionId, BaseCurrency);
                Groups.AddToGroupAsync(Context.ConnectionId, "BroadCast").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "GroupMessage").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "BuyerBook:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "SellerBook:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitBuyerBook:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitSellerBook:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "TradingHistory:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "MarketData:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "ChartData:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "Price:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "PairData:" + BaseCurrency).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "MarketTicker").Wait();

                //Arbitrage Connection
                string PairArbitrage = Configuration.GetValue<string>("SignalRKey:DefaultPairArbitrage");
                string BaseCurrencyArbitrage = Configuration.GetValue<string>("SignalRKey:DefaultBaseCurrencyArbitrage");
                Redis.SaveTagsToSetMember(Configuration.GetValue<string>("SignalRKey:RedisPairsArbitrage") + PairArbitrage, Context.ConnectionId, PairArbitrage);
                Redis.SaveTagsToSetMember(Configuration.GetValue<string>("SignalRKey:RedisMarketArbitrage") + BaseCurrencyArbitrage, Context.ConnectionId, BaseCurrencyArbitrage);
                Groups.AddToGroupAsync(Context.ConnectionId, "BuyerBookArbitrage:" + PairArbitrage).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "SellerBookArbitrage:" + PairArbitrage).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitBuyerBookArbitrage:" + PairArbitrage).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitSellerBookArbitrage:" + PairArbitrage).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "TradingHistoryArbitrage:" + PairArbitrage).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "MarketDataArbitrage:" + PairArbitrage).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "ChartDataArbitrage:" + PairArbitrage).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "PriceArbitrage:" + PairArbitrage).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "ProviderMarketDataArbitrage:" + PairArbitrage).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "ProfitIndicatorArbitrage:" + PairArbitrage).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "ExchangeListSmartArbitrage:" + PairArbitrage).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "PairDataArbitrage:" + BaseCurrencyArbitrage).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "MarketTickerArbitrage").Wait();
                return base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }

        }

        public Task OnConnected(string AccessToken)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                Redis.SaveToHash(Configuration.GetValue<string>("SignalRKey:RedisUsers") + Context.ConnectionId, new ConnetedClientToken { Token = AccessToken }, AccessToken);
                //Task.Run(() => HelperForLog.WriteLogForSocket("OnConnected-AccessToken" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + AccessToken));
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }

        }

        public Task OnDefaultPairConnectionArbitrage(string ArbitragePair, string ArbitrageMarket)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                Redis.SaveTagsToSetMember(Configuration.GetValue<string>("SignalRKey:RedisPairsArbitrage") + ArbitragePair, Context.ConnectionId, ArbitragePair);
                Redis.SaveTagsToSetMember(Configuration.GetValue<string>("SignalRKey:RedisMarketArbitrage") + ArbitrageMarket, Context.ConnectionId, ArbitrageMarket);
                Groups.AddToGroupAsync(Context.ConnectionId, "BuyerBookArbitrage:" + ArbitragePair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "SellerBookArbitrage:" + ArbitragePair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitBuyerBookArbitrage:" + ArbitragePair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitSellerBookArbitrage:" + ArbitragePair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "TradingHistoryArbitrage:" + ArbitragePair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "MarketDataArbitrage:" + ArbitragePair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "ChartDataArbitrage:" + ArbitragePair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "PriceArbitrage:" + ArbitragePair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "ProviderMarketDataArbitrage:" + ArbitragePair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "ProfitIndicatorArbitrage:" + ArbitragePair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "ExchangeListSmartArbitrage:" + ArbitragePair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "PairDataArbitrage:" + ArbitrageMarket).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "MarketTickerArbitrage").Wait();
                //Task.Run(() => HelperForLog.WriteLogForActivity("OnDefaultPairConnectionArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "OnDefaultPairConnectionArbitrage 1 :", " " + ArbitragePair + " Market " + ArbitrageMarket + " Connection ID :" + Context.ConnectionId.ToString()));
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }
        public Task OnDefaultPairConnection(string Pair, string Market)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                Redis.SaveTagsToSetMember(Configuration.GetValue<string>("SignalRKey:RedisPairs") + Pair, Context.ConnectionId, Pair);
                Redis.SaveTagsToSetMember(Configuration.GetValue<string>("SignalRKey:RedisMarket") + Market, Context.ConnectionId, Market);
                Groups.AddToGroupAsync(Context.ConnectionId, "BroadCast").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "GroupMessage").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "BuyerBook:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "SellerBook:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitBuyerBook:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitSellerBook:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "TradingHistory:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "MarketData:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "ChartData:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "Price:" + Pair).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "PairData:" + Market).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "MarketTicker").Wait();
                //Task.Run(() => HelperForLog.WriteLogForActivity("OnDefaultPairConnection" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "OnDefaultPairConnection 1 :", " " + Pair + " Market " + Market + " Connection ID :" + Context.ConnectionId.ToString()));
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        public Task OnDisconnectConnectionArbitrage(string PairArbitrage, string BaseCurrencyArbitrage)
        {
            try
            {
                RemoveArbitragePairSubscription(PairArbitrage);
                RemoveArbitrageMarketSubscription(BaseCurrencyArbitrage);
                Task.Run(() => HelperForLog.WriteLogForActivity("OnDisconnectConnectionArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "OnDisconnectConnection ", " " + PairArbitrage + " Market " + BaseCurrencyArbitrage + " Connection ID :" + Context.ConnectionId.ToString()));
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }
        public Task OnDisconnectConnection(string Pair, string BaseCurrency)
        {
            try
            {
                RemovePairSubscription(Pair);
                RemoveMarketSubscription(BaseCurrency);
                Task.Run(() => HelperForLog.WriteLogForActivity("OnDisconnectConnection" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "OnDisconnectConnection ", " " + Pair + " Market " + BaseCurrency + " Connection ID :" + Context.ConnectionId.ToString()));
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }
        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                string Pair = Redis.GetPairOrMarketData(Context.ConnectionId, ":", Configuration.GetValue<string>("SignalRKey:RedisPairswithoutExpr"));
                string BaseCurrency = Redis.GetPairOrMarketData(Context.ConnectionId, ":", Configuration.GetValue<string>("SignalRKey:RedisMarketwithoutExpr"));

                ConnetedClientToken Client = new ConnetedClientToken();
                Client = Redis.GetData(Configuration.GetValue<string>("SignalRKey:RedisUsers") + Context.ConnectionId);
                if (Client != null)
                {
                    Redis.DeleteTag(Configuration.GetValue<string>("SignalRKey:RedisUsers") + Context.ConnectionId, Client.Token);
                }
                Redis.DeleteTag(Configuration.GetValue<string>("SignalRKey:RedisPairs") + Pair, Context.ConnectionId);
                Redis.DeleteTag(Configuration.GetValue<string>("SignalRKey:RedisMarket") + BaseCurrency, Context.ConnectionId);
                Redis.DeleteHash(Configuration.GetValue<string>("SignalRKey:RedisUsers") + Context.ConnectionId);
                Redis.RemoveSetMember(Configuration.GetValue<string>("SignalRKey:RedisPairs") + Pair, Context.ConnectionId);
                Redis.RemoveSetMember(Configuration.GetValue<string>("SignalRKey:RedisMarket") + BaseCurrency, Context.ConnectionId);
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "BuyerBook:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "SellerBook:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "StopLimitBuyerBook:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "StopLimitSellerBook:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "TradingHistory:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "MarketData:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "ChartData:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "Price:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "BroadCast").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "GroupMessage").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "PairData:" + BaseCurrency).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "MarketTicker").Wait();

                // remove Arbitrage connection
                string PairArbitrage = Redis.GetPairOrMarketData(Context.ConnectionId, ":", Configuration.GetValue<string>("SignalRKey:RedisPairswithoutExprArbitrage"));
                string BaseCurrencyArbitrage = Redis.GetPairOrMarketData(Context.ConnectionId, ":", Configuration.GetValue<string>("SignalRKey:RedisMarketwithoutExprArbitrage"));

                Redis.DeleteTag(Configuration.GetValue<string>("SignalRKey:RedisPairsArbitrage") + PairArbitrage, Context.ConnectionId);
                Redis.DeleteTag(Configuration.GetValue<string>("SignalRKey:RedisMarketArbitrage") + BaseCurrencyArbitrage, Context.ConnectionId);
                Redis.RemoveSetMember(Configuration.GetValue<string>("SignalRKey:RedisPairsArbitrage") + PairArbitrage, Context.ConnectionId);
                Redis.RemoveSetMember(Configuration.GetValue<string>("SignalRKey:RedisMarketArbitrage") + BaseCurrencyArbitrage, Context.ConnectionId);
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "BuyerBookArbitrage:" + PairArbitrage).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "SellerBookArbitrage:" + PairArbitrage).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "StopLimitBuyerBookArbitrage:" + PairArbitrage).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "StopLimitSellerBookArbitrage:" + PairArbitrage).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "TradingHistoryArbitrage:" + PairArbitrage).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "MarketDataArbitrage:" + PairArbitrage).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "ChartDataArbitrage:" + PairArbitrage).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "PriceArbitrage:" + PairArbitrage).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "PairDataArbitrage:" + BaseCurrencyArbitrage).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "MarketTickerArbitrage").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "ProviderMarketDataArbitrage" + PairArbitrage).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "ProfitIndicatorArbitrage" + PairArbitrage).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "ExchangeListSmartArbitrage" + PairArbitrage).Wait();

                return base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        public IReadOnlyList<string> GetConnectedClient(string Pair)
        {
            try
            {
                var Redis = new RadisServices<string>(this._fact);
                IReadOnlyList<string> ConnectedClient = Redis.GetSetList(Pair);
                return ConnectedClient;
            }
            catch (Exception ex)
            {
                List<string> ConnectedClient = new List<string>();
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return ConnectedClient.AsReadOnly();
            }

        }

        #endregion

        #region "Subscription Managemnet"
        // Remove From Subscription Channel
        public Task RemovePairSubscription(string Pair)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                Redis.RemoveSetMember(Configuration.GetValue<string>("SignalRKey:RedisPairs") + Pair, Context.ConnectionId);
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "BuyerBook:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "SellerBook:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "StopLimitBuyerBook:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "StopLimitSellerBook:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "TradingHistory:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "MarketData:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "ChartData:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "Price:" + Pair).Wait();
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        // Add to Subscription Channel
        public Task AddPairSubscription(string Pair, string OldPair)
        {
            try
            {
                if (!string.IsNullOrEmpty(Pair) && !string.IsNullOrEmpty(OldPair))
                {
                    if (Pair != OldPair)
                    {
                        var Redis = new RadisServices<ConnetedClientList>(this._fact);
                        Redis.SaveTagsToSetMember(Configuration.GetValue<string>("SignalRKey:RedisPairs") + Pair, Context.ConnectionId, Context.ConnectionId);
                        Groups.AddToGroupAsync(Context.ConnectionId, "BuyerBook:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "SellerBook:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitBuyerBook:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitSellerBook:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "TradingHistory:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "MarketData:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "ChartData:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "Price:" + Pair).Wait();
                        RemovePairSubscription(OldPair);
                    }
                }
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        // Remove From Subscription Channel
        public Task RemoveMarketSubscription(string BaseCurrency)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                Redis.RemoveSetMember(Configuration.GetValue<string>("SignalRKey:RedisMarket") + BaseCurrency, Context.ConnectionId);
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "PairData:" + BaseCurrency).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "MarketTicker").Wait();
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        // Add to Subscription Channel
        public Task AddMarketSubscription(string BaseCurrency, string OldBaseCurrency)
        {
            try
            {
                if (!string.IsNullOrEmpty(BaseCurrency) && !string.IsNullOrEmpty(OldBaseCurrency))
                {
                    if (BaseCurrency != OldBaseCurrency)
                    {
                        var Redis = new RadisServices<ConnetedClientList>(this._fact);
                        Redis.SaveTagsToSetMember(Configuration.GetValue<string>("SignalRKey:RedisMarket") + BaseCurrency, Context.ConnectionId, Context.ConnectionId);
                        Groups.AddToGroupAsync(Context.ConnectionId, "PairData:" + BaseCurrency).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "MarketTicker").Wait();
                        RemoveMarketSubscription(OldBaseCurrency);
                    }
                }
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        #endregion

        // khushali  11-03-2019 comment Chat module , seprated Chat module (chathub)
        
        #region "Global Updates For Time , News , Announce"

        //public void getTime(string countryZone)
        public Task GetServerTime()
        {
            try
            {
                _chatHubContext.Clients.Client(Context.ConnectionId).SendAsync("SetTime", Helpers.Helpers.GetUTCTime());
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        public Task GetTime(string Data)
        {
            try
            {
                _chatHubContext.Clients.All.SendAsync("SetTime", Data);
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        public Task BroadCastData(string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("BroadCast").SendAsync("BroadcastMessage", Data);
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        public Task BroadCastNews(string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("BroadCast").SendAsync("RecieveNews", Data);
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        public Task BroadCastAnnouncement(string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("BroadCast").SendAsync("RecieveAnnouncement", Data);
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        #endregion

        #region "User Specific Updates"
        public async Task<int> RemoveActiveOrder(string Token, string Order)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveActiveOrderRemove), Order);
                    Task.Run(() => HelperForLog.WriteLogForSocket("RemoveActiveOrder" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Order));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        //Active order
        public async Task<int> ActiveOrder(string Token, string Order)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveActiveOrder), Order);
                    Task.Run(() => HelperForLog.WriteLogForSocket("ActiveOrder" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Order));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        //open order
        public async Task<int> OpenOrder(string Token, string Order)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    await _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveOpenOrder), Order);
                    Task.Run(() => HelperForLog.WriteLogForSocket("OpenOrder" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Order));
                }
               return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        //OrderHistory
        public async Task<int> TradeHistory(string Token, string Order)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveTradeHistory), Order);
                    Task.Run(() => HelperForLog.WriteLogForSocket("TradeHistory" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Order));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        //TradeHistoryByUser
        public async Task<int> RecentOrder(string Token, string Order)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveRecentOrder), Order);
                    Task.Run(() => HelperForLog.WriteLogForSocket("RecentOrder" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Order));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> WalletBalUpdate(string Token, string WalletName, string Data)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> ClientList = Redis.GetKey(Token);
                foreach (string s in ClientList.ToList())
                {
                    var Key = s;
                    Key = Key.Split(":")[1].ToString();
                    string Pair = Redis.GetPairOrMarketData(Key, ":", Configuration.GetValue<string>("SignalRKey:RedisPairswithoutExpr"));
                    //if (Pair.Split("_")[1].ToString() == WalletName)
                    //{
                    _chatHubContext.Clients.Client(Key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveWalletBal), Data);
                    //Task.Run(() => HelperForLog.WriteLogForSocket("BuyerSideWalletBal" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub", " Send Data :"+ Data));
                    Task.Run(() => HelperForLog.WriteLogForSocket("WalletBalaceChange" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub", " Send Data :" + Data + " key :  " + Key));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> ActivityNotification(string Token, string Message)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    await _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveNotification), Message);
                    Task.Run(() => HelperForLog.WriteLogForSocket("ActivityNotification" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Message));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return  await Task.FromResult(0);
            }
        }

        public async Task<int> WalletActivity(string Token, string Message)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    await _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveWalletActivity), Message);
                    Task.Run(() => HelperForLog.WriteLogForSocket("WalletActivity" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Message));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> OnSessionExpired(string Token, string Message)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    await _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.ReceiveSessionExpired), Message);
                    Task.Run(() => HelperForLog.WriteLogForSocket("ReceiveSessionExpired" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Message));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> EnvironmentMode(string Message)
        {
            try
            {
                
                await _chatHubContext.Clients.All.SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.ReceiveEnvironmentMode), Message);
                Task.Run(() => HelperForLog.WriteLogForSocket("ReceiveEnvironmentMode" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Message));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        #endregion

        #region "Pair Wise Global Updates"

        public async Task<int> BuyerBook(string Pair, string Data)
        {
            try
            {
                //_chatHubContext.Clients.Clients(GetConnectedClient(Pair)).SendAsync("RecieveBuyerBook", Helpers.Helpers.JsonSerialize(Data));
                _chatHubContext.Clients.Group("BuyerBook:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBuyerBook), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("BuyerBook" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> SellerBook(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("SellerBook:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveSellerBook), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("SellerBook" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }

        }

        public async Task<int> OrderHistoryLP(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("TradingHistory:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.ReceiveBulkOrderHistory), Data);
                //Task.Run(() => HelperForLog.WriteLogForSocket("BuyerBook" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> BuyerBookLP(string Pair, string Data)
        {
            bool WriteLog = Configuration.GetValue<bool>("IsWriteSocketLog");
            try
            {
                await _chatHubContext.Clients.Group("BuyerBook:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.ReceiveBulkBuyerBook), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("BuyerBook" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data, IsWrite: WriteLog));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> SellerBookLP(string Pair, string Data)
        {
            bool WriteLog = Configuration.GetValue<bool>("IsWriteSocketLog");
            try
            {
                await _chatHubContext.Clients.Group("SellerBook:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.ReceiveBulkSellerBook), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("SellerBook" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data, IsWrite: WriteLog));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }

        }

        public async Task<int> StopLimitBuyerBook(string Pair, string Data)
        {
            try
            {
                //_chatHubContext.Clients.Clients(GetConnectedClient(Pair)).SendAsync("RecieveBuyerBook", Helpers.Helpers.JsonSerialize(Data));
                _chatHubContext.Clients.Group("StopLimitBuyerBook:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveStopLimitBuyerBook), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("StopLimitBuyerBook" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> StopLimitSellerBook(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("StopLimitSellerBook:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveStopLimitSellerBook), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("StopLimitSellerBook" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }

        }

        // Global Trades settelment
        //TradeHistoryByPair

        public async Task<int> OrderHistory(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("TradingHistory:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveOrderHistory), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("OrderHistory" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> MarketData(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("MarketData:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveMarketData), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("MarketData" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> LastPrice(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("Price:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveLastPrice), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("LastPrice" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> ChartData(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("ChartData:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveChartData), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("ChartData" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        #endregion

        #region "Base Market"

        public async Task<int> PairData(string BaseCurrency, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("PairData:" + BaseCurrency).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecievePairData), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("PairData" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> MarketTicker(string BaseCurrency, string Data)
        {
            try
            {
                //_chatHubContext.Clients.Group("MarketTicker:" + BaseCurrency).SendAsync("RecieveMarketTicker", Data);
                _chatHubContext.Clients.Group("MarketTicker").SendAsync("RecieveMarketTicker", Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("MarketTicker" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        #endregion

        #region "Arbitrage Subscription Managemnet"
        // Arbitrage Subscription Channel
        public Task AddArbitragePairSubscription(string Pair, string OldPair)
        {
            try
            {
                if (!string.IsNullOrEmpty(Pair) && !string.IsNullOrEmpty(OldPair))
                {
                    if (Pair != OldPair)
                    {
                        var Redis = new RadisServices<ConnetedClientList>(this._fact);
                        Redis.SaveTagsToSetMember(Configuration.GetValue<string>("SignalRKey:RedisPairsArbitrage") + Pair, Context.ConnectionId, Context.ConnectionId);
                        Groups.AddToGroupAsync(Context.ConnectionId, "BuyerBookArbitrage:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "SellerBookArbitrage:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitBuyerBookArbitrage:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitSellerBookArbitrage:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "TradingHistoryArbitrage:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "MarketDataArbitrage:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "ChartDataArbitrage:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "PriceArbitrage:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "ProviderMarketDataArbitrage:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "ProfitIndicatorArbitrage:" + Pair).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "ExchangeListSmartArbitrage:" + Pair).Wait();
                        RemoveArbitragePairSubscription(OldPair);
                    }
                }
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        public Task RemoveArbitragePairSubscription(string Pair)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                //Redis.DeleteTag(Configuration.GetValue<string>("SignalRKey:RedisPairs") + Pair, Context.ConnectionId);
                Redis.RemoveSetMember(Configuration.GetValue<string>("SignalRKey:RedisPairsArbitrage") + Pair, Context.ConnectionId);
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "BuyerBookArbitrage:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "SellerBookArbitrage:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "StopLimitBuyerBookArbitrage:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "StopLimitSellerBookArbitrage:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "TradingHistoryArbitrage:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "MarketDataArbitrage:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "ChartDataArbitrage:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "PriceArbitrage:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "ProviderMarketDataArbitrage:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "ProfitIndicatorArbitrage:" + Pair).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "ExchangeListSmartArbitrage:" + Pair).Wait();
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        public Task AddArbitrageMarketSubscription(string BaseCurrency, string OldBaseCurrency)
        {
            try
            {
                if (!string.IsNullOrEmpty(BaseCurrency) && !string.IsNullOrEmpty(OldBaseCurrency))
                {
                    if (BaseCurrency != OldBaseCurrency)
                    {
                        var Redis = new RadisServices<ConnetedClientList>(this._fact);
                        Redis.SaveTagsToSetMember(Configuration.GetValue<string>("SignalRKey:RedisMarketArbitrage") + BaseCurrency, Context.ConnectionId, Context.ConnectionId);
                        Groups.AddToGroupAsync(Context.ConnectionId, "PairDataArbitrage:" + BaseCurrency).Wait();
                        //Groups.AddToGroupAsync(Context.ConnectionId, "MarketTicker:" + BaseCurrency).Wait();
                        Groups.AddToGroupAsync(Context.ConnectionId, "MarketTickerArbitrage").Wait();
                        RemoveArbitrageMarketSubscription(OldBaseCurrency);
                    }
                }
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        public Task RemoveArbitrageMarketSubscription(string BaseCurrency)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                Redis.RemoveSetMember(Configuration.GetValue<string>("SignalRKey:RedisMarketArbitrage") + BaseCurrency, Context.ConnectionId);
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "PairDataArbitrage:" + BaseCurrency).Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "MarketTickerArbitrage").Wait();
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }
        #endregion

        #region "Arbitrage User Specific Updates"

        public async Task<int> ActiveOrderArbitrage(string Token, string Order)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveActiveOrderArbitrage), Order);
                    Task.Run(() => HelperForLog.WriteLogForSocket("ActiveOrderArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Order));
                }return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        
        public async Task<int> TradeHistoryArbitrage(string Token, string Order)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveTradeHistoryArbitrage), Order);
                    Task.Run(() => HelperForLog.WriteLogForSocket("TradeHistoryArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Order));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        
        public async Task<int> RecentOrderArbitrage(string Token, string Order)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveRecentOrderArbitrage), Order);
                    Task.Run(() => HelperForLog.WriteLogForSocket("RecentOrderArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Order));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> WalletBalUpdateArbitrage(string Token, string WalletName, string Data)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> ClientList = Redis.GetKey(Token);
                foreach (string s in ClientList.ToList())
                {
                    var Key = s;
                    Key = Key.Split(":")[1].ToString();
                    string Pair = Redis.GetPairOrMarketData(Key, ":", Configuration.GetValue<string>("SignalRKey:RedisPairswithoutExprArbitrage"));
                    _chatHubContext.Clients.Client(Key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveWalletBalArbitrage), Data);
                    Task.Run(() => HelperForLog.WriteLogForSocket("WalletBalUpdateArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub", " Send Data :" + Data + " key :  " + Key));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> ActivityNotificationArbitrage(string Token, string Message)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    await _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveNotificationArbitrage), Message);
                    Task.Run(() => HelperForLog.WriteLogForSocket("ActivityNotificationArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Message));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> WalletActivityArbitrage(string Token, string Message)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                IEnumerable<string> str = Redis.GetKey(Token);
                foreach (string s in str.ToList())
                {
                    var key = s;
                    key = key.Split(":")[1].ToString();
                    await _chatHubContext.Clients.Client(key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveWalletActivityArbitrage), Message);
                    Task.Run(() => HelperForLog.WriteLogForSocket("WalletActivityArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Message));
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        #endregion

        #region "Arbitrage Pair Wise Global Updates"

        public async Task<int> BuyerBookArbitrage(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("BuyerBookArbitrage:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBuyerBookArbitrage), Data);
               // Task.Run(() => HelperForLog.WriteLogForActivity("BuyerBookArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> SellerBookArbitrage(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("SellerBookArbitrage:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveSellerBookArbitrage), Data);
                //Task.Run(() => HelperForLog.WriteLogForActivity("SellerBookArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }

        }

        public async Task<int> StopLimitBuyerBookArbitrage(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("StopLimitBuyerBookArbitrage:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveStopLimitBuyerBookArbitrage), Data);
                //Task.Run(() => HelperForLog.WriteLogForSocket("StopLimitBuyerBookArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> StopLimitSellerBookArbitrage(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("StopLimitSellerBookArbitrage:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveStopLimitSellerBookArbitrage), Data);
                //Task.Run(() => HelperForLog.WriteLogForSocket("StopLimitSellerBookArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }

        }

        public async Task<int> OrderHistoryArbitrage(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("TradingHistoryArbitrage:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveOrderHistoryArbitrage), Data);
                //Task.Run(() => HelperForLog.WriteLogForSocket("OrderHistoryArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> MarketDataArbitrage(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("MarketDataArbitrage:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveMarketDataArbitrage), Data);
                //Task.Run(() => HelperForLog.WriteLogForSocket("MarketDataArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> ProviderMarketDataArbitrage(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("ProviderMarketDataArbitrage:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveProviderMarketDataArbitrage), Data);
                //Task.Run(() => HelperForLog.WriteLogForActivity("ProviderMarketDataArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> LastPriceArbitrage(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("PriceArbitrage:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveLastPriceArbitrage), Data);
                //Task.Run(() => HelperForLog.WriteLogForActivity("LastPriceArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> ChartDataArbitrage(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("ChartDataArbitrage:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveChartDataArbitrage), Data);
                //Task.Run(() => HelperForLog.WriteLogForSocket("ChartDataArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> ProfitIndicatorArbitrage(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("ProfitIndicatorArbitrage:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveProfitIndicatorArbitrage), Data);
                //Task.Run(() => HelperForLog.WriteLogForActivity("ProfitIndicatorArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> ExchangeListSmartArbitrage(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("ExchangeListSmartArbitrage:" + Pair).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveExchangeListSmartArbitrage), Data);
                //Task.Run(() => HelperForLog.WriteLogForActivity("ExchangeListSmartArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        #endregion

        #region "Arbitrage Base Market"

        public async Task<int> PairDataArbitrage(string BaseCurrency, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("PairDataArbitrage:" + BaseCurrency).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecievePairDataArbitrage), Data);
                //Task.Run(() => HelperForLog.WriteLogForSocket("PairDataArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        public async Task<int> MarketTickerArbitrage(string BaseCurrency, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("MarketTickerArbitrage").SendAsync("RecieveMarketTickerArbitrage", Data);
                //Task.Run(() => HelperForLog.WriteLogForSocket("MarketTickerArbitrage" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }

        #endregion
    }
}