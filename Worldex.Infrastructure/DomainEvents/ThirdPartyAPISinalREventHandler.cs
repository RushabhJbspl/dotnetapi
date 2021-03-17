using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Entities.SignalR;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.FeedConfiguration;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Core.SignalR;
using Worldex.Core.ViewModels;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.DomainEvents
{
    public class ThirdPartyAPISinalREventHandler : IRequestHandler<ThirdPartyAPISinalR, CommunicationResponse>
    {
        private ThirdPartySocketHub _chat;
        private RedisConnectionFactory _fact;
        private IMemoryCache _cache { get; set; }
        private readonly IConfiguration _configuration;
        private readonly IExchangeFeedConfiguration _exchangeFeed;
        private ICommonRepository<SocketMethods> _SocketMethodsRepository;
        private ICommonRepository<UserAPIKeyDetails> _userAPIKeyDetailsRepository;
        private ICommonRepository<UserSubscribeAPIPlan> _SubScribePlanRepository;
        private ICommonRepository<APIMethodConfiguration> _APIMethodConfiguration;
        private ICommonRepository<APIPlanMethodConfiguration> _APIPlanMethodConfigHistoryRepository;

        private static List<APIMethodConfiguration> MethodConfig = new List<APIMethodConfiguration>();
        private static List<SocketMethods> socketMethod = new List<SocketMethods>();
        private static List<APIPlanMethodConfiguration> PlanMethod = new List<APIPlanMethodConfiguration>();

        public ThirdPartyAPISinalREventHandler(ThirdPartySocketHub chat, ICommonRepository<UserAPIKeyDetails> userAPIKeyDetailsRepository,
            RedisConnectionFactory fact, IConfiguration configuration, ICommonRepository<SocketMethods> SocketMethodsRepository,
            IMemoryCache cache, IExchangeFeedConfiguration exchangeFeed, ICommonRepository<UserSubscribeAPIPlan> SubScribePlanRepository,
            ICommonRepository<APIMethodConfiguration> APIMethodConfiguration, ICommonRepository<APIPlanMethodConfiguration> APIPlanMethodConfigHistoryRepository)
        {
            _chat = chat;
            _fact = fact;
            _configuration = configuration;
            _cache = cache;
            _exchangeFeed = exchangeFeed;
            _userAPIKeyDetailsRepository = userAPIKeyDetailsRepository;
            _SocketMethodsRepository = SocketMethodsRepository;
            _SubScribePlanRepository = SubScribePlanRepository;
            _APIMethodConfiguration = APIMethodConfiguration;
            _APIPlanMethodConfigHistoryRepository = APIPlanMethodConfigHistoryRepository;

            socketMethod = _cache.Get<List<SocketMethods>>("socketMethod");
            if (socketMethod == null)
            {
                socketMethod = _SocketMethodsRepository.List();
                _cache.Set("socketMethod", _SocketMethodsRepository.List());
            }
            MethodConfig = _cache.Get<List<APIMethodConfiguration>>("MethodConfig");
            if (MethodConfig == null)
            {
                MethodConfig = _APIMethodConfiguration.List();
                _cache.Set("MethodConfig", _APIMethodConfiguration.List());
            }
            PlanMethod = _cache.Get<List<APIPlanMethodConfiguration>>("PlanMethod");
            if (PlanMethod == null)
            {
                PlanMethod = _APIPlanMethodConfigHistoryRepository.List();
                _cache.Set("PlanMethod", _APIPlanMethodConfigHistoryRepository.List());
            }
        }
        public async System.Threading.Tasks.Task<CommunicationResponse> Handle(ThirdPartyAPISinalR request, CancellationToken cancellationToken)
        {
            CommunicationResponse response = new CommunicationResponse();
            try
            {
                long CustomeID = 0;
                List<string> APIkeys = null;
                if(string.IsNullOrEmpty(request.UserID))
                    return await Task.FromResult(response);

                var CurrentPlan = _SubScribePlanRepository.FindBy(e => e.UserID == Convert.ToInt64(request.UserID) && e.Status == 1).FirstOrDefault();
                if (CurrentPlan == null)
                    return await Task.FromResult(response);

                if (CurrentPlan.CustomeLimitId != 0)
                    CustomeID = CurrentPlan.CustomeLimitId;

                var PlanMethodList = PlanMethod.Where(e => e.APIPlanMasterID == CurrentPlan.APIPlanMasterID && e.CustomeLimitId == CustomeID && e.Status == 1).ToList();
                if (PlanMethodList.Count > 0)
                {
                    var KeyArr = Array.ConvertAll<APIPlanMethodConfiguration, long>(PlanMethodList.ToArray(), x => (long)x.RestMethodID);
                    var ParentID = MethodConfig.Find(e => e.MethodID == Convert.ToInt64(request.Method) && e.MethodType == 2);
                    if (ParentID == null)
                        return await Task.FromResult(response);
                    if (!KeyArr.Contains(ParentID.ParentID))
                        return await Task.FromResult(response);
                }
                else
                    return await Task.FromResult(response);
                //var res = _exchangeFeed.CheckFeedLimit(Convert.ToInt16(request.Method));
                //if (res.ReturnCode != enResponseCode.Success)
                //    return await Task.FromResult(response);
                if (request.IsPrivate == 1)
                {
                    var APIkeyTask = this.GetClientAPIKeyByUserID(request.UserID.ToString());
                    APIkeys = await APIkeyTask;
                    if (APIkeys == null && APIkeys.Count == 0)
                        return await Task.FromResult(response);
                }
                switch (request.Method)
                {
                    case enMethodName.BuyerBook:
                        await _chat.BuyerBook(request.Parameter, request.DataObj); break;
                    case enMethodName.SellerBook:
                        await _chat.SellerBook(request.Parameter, request.DataObj); break;
                    case enMethodName.StopLimitBuyerBook:
                        await _chat.StopLimitBuyerBook(request.Parameter, request.DataObj); break;
                    case enMethodName.StopLimitSellerBook:
                        await _chat.StopLimitSellerBook(request.Parameter, request.DataObj); break;
                    case enMethodName.OrderHistory:
                        await _chat.OrderHistory(request.Parameter, request.DataObj); break;
                    case enMethodName.ChartData:
                        await _chat.ChartData(request.Parameter, request.DataObj); break;
                    case enMethodName.MarketData:
                        await _chat.MarketData(request.Parameter, request.DataObj); break;
                    case enMethodName.ActiveOrder:
                        await _chat.ActiveOrder(APIkeys, request.DataObj); break;
                    case enMethodName.OpenOrder:
                        await _chat.OpenOrder(APIkeys, request.DataObj); break;
                    case enMethodName.TradeHistory:
                        await _chat.TradeHistory(APIkeys, request.DataObj); break;
                    case enMethodName.RecentOrder:
                        await _chat.RecentOrder(APIkeys, request.DataObj); break;
                    case enMethodName.BuyerSideWallet:
                        await _chat.WalletBalUpdate(APIkeys, request.WalletName, request.DataObj); break;
                    case enMethodName.SellerSideWallet:
                        await _chat.WalletBalUpdate(APIkeys, request.WalletName, request.DataObj); break;
                    case enMethodName.Price:
                        await _chat.LastPrice(request.Parameter, request.DataObj); break;
                    case enMethodName.PairData:
                        await _chat.PairData(request.Parameter, request.DataObj); break;
                    case enMethodName.MarketTicker:
                        await _chat.MarketTicker(request.Parameter, request.DataObj); break;
                    case enMethodName.ActivityNotification:
                        await _chat.ActivityNotification(APIkeys, request.DataObj); break;
                    case enMethodName.WalletActivity:
                        await _chat.WalletActivity(APIkeys, request.DataObj); break;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(response);
            }
        }
        public async Task<List<string>> GetClientAPIKeyByUserID(string ID)
        {
            List<string> ClientAPIKeyList = new List<string>();
            try
            {
                var Redis = new RadisServices<ConnetedClientList>(this._fact);
                var APIKeyList = await _userAPIKeyDetailsRepository.FindByAsync(o => o.UserID == Convert.ToInt64(ID) && o.Status == 1);
                if (APIKeyList != null)
                {
                    foreach (var keyDetails in APIKeyList)
                    {
                        ConnetedClientList Cleint = Redis.GetData(_configuration.GetValue<string>("SignalRKey:RedisClientConnection") + keyDetails.APIKey);
                        if (Cleint != null && !string.IsNullOrEmpty(Cleint.ConnectionId))
                        {
                            ClientAPIKeyList.Add(Cleint.ConnectionId);
                        }
                    }
                }
                return await Task.FromResult(ClientAPIKeyList);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
    }
}
