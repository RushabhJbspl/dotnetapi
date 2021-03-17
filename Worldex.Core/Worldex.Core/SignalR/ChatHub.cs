using Microsoft.AspNetCore.Authorization;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Enums;
using Worldex.Core.Services.RadisDatabase;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Worldex.Core.ViewModels.Chat;
using Worldex.Core.Entities.Communication;
using Microsoft.Extensions.Configuration;

namespace Worldex.Core.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly PresenceTracker presenceTracker;
        private RedisConnectionFactory _fact;
        private IHubContext<ChatHub> _chatHubContext;
        private readonly ILogger<ChatHub> _logger;
        private readonly IConfiguration Configuration;

        public ChatHub(ILogger<ChatHub> Logger, RedisConnectionFactory Factory, PresenceTracker presenceTracker, IHubContext<ChatHub> ChatHubContext, IConfiguration configuration)
        {
            _fact = Factory;
            this.presenceTracker = presenceTracker;
            _chatHubContext = ChatHubContext;
            _logger = Logger;
            Configuration = configuration;
        }

        public override async Task OnConnectedAsync()
        {
            //var conetx = Context.User.Identity;

            if (Context.User != null)
            {
                await ConnectionOpenedAsync(Context.User.Identity.Name);
            }

            await base.OnConnectedAsync();
        }

        public async Task ConnectionOpenedAsync(string Username)
        {
            try
            {
                var result = presenceTracker.ConnectionOpened(Username, Context.ConnectionId);
                var Redis = new RadisServices<USerDetail>(this._fact);
                long Count = Redis.SortedSetLength(Configuration.GetValue<string>("SignalRKey:RedisOnlineUserList"));
                SignalRComm<long> CommonData1 = new SignalRComm<long>();
                CommonData1.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Nofification);
                CommonData1.Method = Enum.GetName(typeof(enMethodName), enMethodName.OnlineUserCount);
                CommonData1.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.GetOnlineUserCount);
                CommonData1.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData1.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.UserInfo);
                CommonData1.Data = Count;
                CommonData1.Parameter = Username;
                string OnlineUserCount = JsonConvert.SerializeObject(CommonData1);
                await _chatHubContext.Clients.All.SendAsync("GetOnlineUserCount", OnlineUserCount);
                ConnectionOpenedResult Result = await result;
                BlockedUserViewModel blockedUserView = new BlockedUserViewModel()
                {
                    IsBlocked = Result.Blocked
                };
                SignalRComm<BlockedUserViewModel> CommonData = new SignalRComm<BlockedUserViewModel>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Nofification);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.MarkBlockUnblockUser);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBlockUnblockUser);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.UserInfo);
                CommonData.Data = blockedUserView;
                CommonData.Parameter = Username;
                string Data = JsonConvert.SerializeObject(CommonData);

                if (Result.Blocked)
                {
                    await _chatHubContext.Clients.Clients(Context.ConnectionId).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBlockUnblockUser), Data);
                }
                else
                {
                    await _chatHubContext.Clients.Clients(Context.ConnectionId).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBlockUnblockUser), Data);
                }
                //return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        public async Task ConnectionClosedAsync(string Username)
        {
            try
            {
                var result = await presenceTracker.ConnectionClosed(Username);
                var Redis = new RadisServices<USerDetail>(this._fact);
                long Count = Redis.SortedSetLength(Configuration.GetValue<string>("SignalRKey:RedisOnlineUserList"));
                SignalRComm<long> CommonData1 = new SignalRComm<long>();
                CommonData1.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Nofification);
                CommonData1.Method = Enum.GetName(typeof(enMethodName), enMethodName.OnlineUserCount);
                CommonData1.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.GetOnlineUserCount);
                CommonData1.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData1.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.UserInfo);
                CommonData1.Data = Count;
                CommonData1.Parameter = Username;
                string OnlineUserCount = JsonConvert.SerializeObject(CommonData1);
                await _chatHubContext.Clients.All.SendAsync("GetOnlineUserCount", OnlineUserCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
            //return await Task.FromResult(0);
        }

        public async Task MarkUserBlock(long UserID, string Data)
        {
            try
            {
                var Redis = new RadisServices<USerDetail>(this._fact);
                RedisValue[] byScore = Redis.GetSortedSetDataByScore(Configuration.GetValue<string>("SignalRKey:RedisOnlineUserList"), UserID, UserID);
                string s = byScore[0];
                USerDetail user = JsonConvert.DeserializeObject<USerDetail>(s);
                await _chatHubContext.Clients.Clients(user.ConnectionID).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBlockUnblockUser), Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        public async Task BlockedUserUpdate(string Data)
        {
            try
            {
                await _chatHubContext.Clients.All.SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBlockUserUpdate), Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User != null)
            {
                await ConnectionClosedAsync(Context.User.Identity.Name);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            try
            {
                var Redis = new RadisServices<ChatHistory>(this._fact);
                Redis.SaveToSortedSet(Configuration.GetValue<string>("SignalRKey:RedisChatHistory"), obj: new ChatHistory { Name = Context.User.Identity.Name, Message = message, Id = Guid.NewGuid(), Time = Helpers.Helpers.GetUTCTime() }, Tag: Context.User.Identity.Name);
                await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        public Task GetChatHistory()
        {
            try
            {
                var Redis = new RadisServices<ChatHistory>(this._fact);
                string Data = Redis.GetSortedSet(Configuration.GetValue<string>("SignalRKey:RedisChatHistory"));
                ChatHistory[] Result = JsonConvert.DeserializeObject<ChatHistory[]>(Data);
                SignalRComm<ChatHistory[]> CommonData = new SignalRComm<ChatHistory[]>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Nofification);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ChatHistory);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveChatHistory);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.UserInfo);
                CommonData.Data = Result;
                CommonData.Parameter = Context.User.Identity.Name;
                string Response = JsonConvert.SerializeObject(CommonData);
                _chatHubContext.Clients.Client(Context.ConnectionId).SendAsync("RecieveChatHistory", Response);
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Task.FromResult(0);
            }
        }
    }
}
