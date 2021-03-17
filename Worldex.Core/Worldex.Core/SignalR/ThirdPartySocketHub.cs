using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Core.ApiModels.Chat;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace Worldex.Core.SignalR
{
    [Authorize]
    public class ThirdPartySocketHub : Hub
    {
        private RedisConnectionFactory _fact;
        private IHubContext<ThirdPartySocketHub> _chatHubContext;
        private readonly ILogger<ThirdPartySocketHub> _logger;
        private readonly IConfiguration Configuration;
        public ThirdPartySocketHub(IHubContext<ThirdPartySocketHub> ChatHubContext, RedisConnectionFactory Factory, 
            ILogger<ThirdPartySocketHub> logger, IConfiguration configuration) 
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
                var subject = Context.User.FindFirst("sub")?.Value;
                if (string.IsNullOrEmpty(subject))
                {
                    return Task.FromResult(0);
                }
                OnClientConnected(subject).Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "BroadCast").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "BuyerBook:").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "SellerBook:").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitBuyerBook:").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "StopLimitSellerBook:").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "TradingHistory:").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "MarketData:").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "ChartData:").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "Price:").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "PairData:").Wait();
                Groups.AddToGroupAsync(Context.ConnectionId, "MarketTicker").Wait();
                return base.OnConnectedAsync();
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
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "BuyerBook:").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "SellerBook:").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "StopLimitBuyerBook:").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "StopLimitSellerBook:").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "TradingHistory:").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "MarketData:").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "ChartData:").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "Price:").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "BroadCast").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "PairData:").Wait();
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "MarketTicker").Wait();
                return base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }

        public Task OnClientConnected(string APIKey)
        {
            System.Exception exception = new Exception();
            try
            {
                var RedisKey = Configuration.GetValue<string>("SignalRKey:RedisClientConnection");
                //var APIKeyData =  _userAPIKeyDetailsRepository.GetAllList().Where(o => o.APIKey == APIKey).FirstOrDefault();
                if(APIKey != null)
                {
                    var Redis = new RadisServices<ConnetedClientList>(this._fact);
                    Redis.SaveToHash(RedisKey + APIKey, new ConnetedClientList { ConnectionId = Context.ConnectionId });
                }
                else
                {
                    return base.OnDisconnectedAsync(exception);
                }
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return base.OnDisconnectedAsync(ex);
            }

        }

        #endregion

        #region "User Specific Updates"
        //Active order
        [AllowAnonymous]
        public async Task<int> ActiveOrder(List<string> Token, string Order)
        {
            var RedisKey = Configuration.GetValue<string>("SignalRKey:RedisClientConnection");
            try
            {                
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                foreach(var Id in Token)
                {
                    //ConnetedClientList Data = Redis.GetData(RedisKey + Id);
                    //if (Data != null && !string.IsNullOrEmpty(Data.ConnectionId))
                    //{
                        _chatHubContext.Clients.Client(Id).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveActiveOrder), Order);
                    //}
                }
                Task.Run(() => HelperForLog.WriteLogForSocket("ActiveOrder" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Order));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        //open order
        [AllowAnonymous]
        public async Task<int> OpenOrder(List<string> Token, string Order)
        {
            var RedisKey = Configuration.GetValue<string>("SignalRKey:RedisClientConnection");
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                foreach (var Id in Token)
                {
                    //ConnetedClientList Data = Redis.GetData(RedisKey + Id);
                    //if (Data != null && !string.IsNullOrEmpty(Data.ConnectionId))
                    //{
                        await _chatHubContext.Clients.Client(Id).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveOpenOrder), Order);
                    //}
                }
                Task.Run(() => HelperForLog.WriteLogForSocket("OpenOrder" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Order));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        //OrderHistory
        [AllowAnonymous]
        public async Task<int> TradeHistory(List<string> Token, string Order)
        {
            var RedisKey = Configuration.GetValue<string>("SignalRKey:RedisClientConnection");
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                foreach (var Id in Token)
                {
                    //ConnetedClientList Data = Redis.GetData(RedisKey + Id);
                    //if (Data != null && !string.IsNullOrEmpty(Data.ConnectionId))
                    //{
                        await _chatHubContext.Clients.Client(Id).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveTradeHistory), Order);
                    //}
                }
                Task.Run(() => HelperForLog.WriteLogForSocket("TradeHistory" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Order));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        //TradeHistoryByUser
        [AllowAnonymous]
        public async Task<int> RecentOrder(List<string> Token, string Order)
        {
            var RedisKey = Configuration.GetValue<string>("SignalRKey:RedisClientConnection");
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                foreach (var Id in Token)
                {
                    //ConnetedClientList Data = Redis.GetData(RedisKey + Token);
                    //if (Data != null && !string.IsNullOrEmpty(Data.ConnectionId))
                    //{
                        await _chatHubContext.Clients.Client(Id).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveRecentOrder), Order);
                    //}
                }
                Task.Run(() => HelperForLog.WriteLogForSocket("RecentOrder" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Order));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        [AllowAnonymous]
        public async Task<int> WalletBalUpdate(List<string> Token, string WalletName, string Data)
        {
            var RedisKey = Configuration.GetValue<string>("SignalRKey:RedisClientConnection");
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                foreach (var Id in Token)
                {
                    //ConnetedClientList Result = Redis.GetData(RedisKey + Token);
                    //if (Result != null && !string.IsNullOrEmpty(Result.ConnectionId))
                    //{
                        await _chatHubContext.Clients.Client(Id).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveWalletBal), Data);
                    //}
                }
                Task.Run(() => HelperForLog.WriteLogForSocket("WalletBalaceChange" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub", " Send Data :" + Data ));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        [AllowAnonymous]
        public async Task<int> ActivityNotification(List<string> Token, string Message)
        {
            var RedisKey = Configuration.GetValue<string>("SignalRKey:RedisClientConnection");
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                foreach (var Id in Token)
                {
                    //ConnetedClientList Result = Redis.GetData(RedisKey + Token);
                    //if (Result != null && !string.IsNullOrEmpty(Result.ConnectionId))
                    //{
                        await _chatHubContext.Clients.Client(Id).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveNotification), Message);
                    //}
                }
                Task.Run(() => HelperForLog.WriteLogForSocket("ActivityNotification" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Message));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return  await Task.FromResult(0);
            }
        }
        [AllowAnonymous]
        public async Task<int> WalletActivity(List<string> Token, string Message)
        {
            var RedisKey = Configuration.GetValue<string>("SignalRKey:RedisClientConnection");
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                foreach (var Id in Token)
                {
                    //ConnetedClientList Result = Redis.GetData(RedisKey + Token);
                    //if (Result != null && !string.IsNullOrEmpty(Result.ConnectionId))
                    //{
                        await _chatHubContext.Clients.Client(Id).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveWalletActivity), Message);
                    //}
                }
                Task.Run(() => HelperForLog.WriteLogForSocket("WalletActivity" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Message));
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
        [AllowAnonymous]
        public async Task<int> BuyerBook(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("BuyerBook:").SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBuyerBook), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("BuyerBook" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        [AllowAnonymous]
        public async Task<int> SellerBook(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("SellerBook:").SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveSellerBook), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("SellerBook" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }

        }
        [AllowAnonymous]
        public async Task<int> StopLimitBuyerBook(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("StopLimitBuyerBook:").SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveStopLimitBuyerBook), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("StopLimitBuyerBook" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        [AllowAnonymous]
        public async Task<int> StopLimitSellerBook(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("StopLimitSellerBook:").SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveStopLimitSellerBook), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("StopLimitSellerBook" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }

        }
        [AllowAnonymous]
        public async Task<int> OrderHistory(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("TradingHistory:").SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveOrderHistory), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("OrderHistory" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        [AllowAnonymous]
        public async Task<int> MarketData(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("MarketData:").SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveMarketData), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("MarketData" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        [AllowAnonymous]
        public async Task<int> LastPrice(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("Price:").SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveLastPrice), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("LastPrice" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Data));
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return await Task.FromResult(0);
            }
        }
        [AllowAnonymous]
        public async Task<int> ChartData(string Pair, string Data)
        {
            try
            {
                _chatHubContext.Clients.Group("ChartData:").SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveChartData), Data);
                Task.Run(() => HelperForLog.WriteLogForSocket("ChartData" + Helpers.Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "ThirdPartySocketHub ", " Data " + Data));
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<int> MarketTicker(string BaseCurrency, string Data)
        {
            try
            {
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
    }
}